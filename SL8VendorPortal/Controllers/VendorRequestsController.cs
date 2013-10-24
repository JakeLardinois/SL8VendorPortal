using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using jQuery.DataTables.Mvc;
using SL8VendorPortal.Models;
using SL8VendorPortal.Infrastructure;
using System.Collections.ObjectModel; //for  ReadOnlyCollection<string>


namespace SL8VendorPortal.Controllers
{

    [Authorize(Roles = "Admin,User")]
    public class VendorRequestsController : Controller
    {
        private SL8VendorPortalDb VendorPortalDb = new SL8VendorPortalDb();
        private List<string> mobjNewMDataProp = new List<string> { 
                "0", "1", "ID", "RequestCategoryCode", "OrderNo", "Item", "Processed", "LineNo", "ReleaseNo", "SourceWarehouse", "DestWarehouse", "Qty", "QtyLoss", "DateProcessed",
                "DateRequested", "DateUpdated" , "Approved", "Creator", "Updater"};


        [HttpGet]
        public ActionResult Search()
        {
            return View("Search");
        }

        //MaxRecordCount is populated by datatables via the aoData array during the fnServerData call...
        [HttpPost]
        public JsonResult Search(JQueryDataTablesModel jQueryDataTablesModel, int MaxRecordCount)
        {
            int totalRecordCount;
            int searchRecordCount;

            /*I am using datatables server side with ColReorderWithResize.js and multicolumn filtering (jquery.dataTables.columnFilter.js) that uses the sSelector option in the .columnFilter() in the instantiation process (see SearchVendorRequest.js) to redirect my search textboxes to a table that I place on the top of my Search.cshtml page.
             * The problem that this creates is that the sSearch_0, sSearch_1, etc. variables that are populated by the form remain in thier position regardless of where the columns get moved. So if I have column 3 (mdataprop_3) named 'Order' moved to the column 1 position,  then mdataprop_1 will hold the column name 'Order' that was previously in mdataprop_3 
             * however, the search text Ssearch_3 will hold the search variables used for column 1 (mdataprop_1 or previously mdataprop_3). The result of this is that my InMemoryRepositories.sc will be searching the property that took the place of mdataprop_3 ('lineNo' or something) with the search text for 'Order'...  To further complicate things, the sorting
             * is NOT affected by this scenario; Only the searching... I searched everywhere for 2 days on this trying to find a resolution! If the sorting was affected by sSearch_ not following mdataprop_ then I could simply make mdataprop_ static regardless of the column ordering; I could not find any property in Datatables that would give me the original 
             * column order, so I would have had to do just like I did with mobjNewMDataProp above. So the only way i could resolve this issue was to modify my JQueryDataTablesModel class and add another ReadOnlyCollection<string> and call it mDataProp2_ . I then use mDataProp2_ to hold the original column order that sSearch_ correctly corresponds to. I then 
             * went into InMemoryRepositories.cs and changed my searching switch statement to use mDataProp2_.
             * I did find some solutions for this issue where the index of the column search textbox was used with fnVisibleToColumnIndex (datatables api) to leverage fnSearch (datatables function). However, fnSearch applies the search text to sSearch and not the sSearch_ array and so multi-column filtering would not work (which is what I have implemented in InMemoryRepositories.cs
             * and I am not using the textboxes at the bottom or top of my datatables column (I'm using sSelector to redirect the search texts to a table) and the solutions that were offered were based on rewiring the event that datatables automatically wires to the keyup event of the column textboxes; which I never use because of sSelector and so the events would
             * never get fired! Furthermore I couldn't wire events to the textboxes created by sSelector because datatable automatically creates them and so I have no way of knowing what thier ID's are; and it's a moot point since thier indexes would have nothing to do with the columns anyway which is what fnVisibleToColumnIndex leverages.
             * So although the solution I implemented is somewht ugly, it is the best way to maintain all of my required functionality...
             */
            jQueryDataTablesModel.mDataProp2_ = mobjNewMDataProp.AsReadOnly();

            //I put a cap on the number of records that this 
            InMemoryVendorRequestsRepository.AllVendorRequests = VendorPortalDb.VendorRequests.Take(MaxRecordCount).ToList();

            var objItems = InMemoryVendorRequestsRepository.GetVendorRequests(
                totalRecordCount: out totalRecordCount, searchRecordCount: out searchRecordCount, DataTablesModel: jQueryDataTablesModel);

            return this.DataTablesJson(items: objItems,
                totalRecords: totalRecordCount,
                totalDisplayRecords: searchRecordCount,
                sEcho: jQueryDataTablesModel.sEcho);
        }

        [HttpPost]
        public JsonResult VendorRequests(JQueryDataTablesModel jQueryDataTablesModel, string OrderNo, short? LineNo, short? ReleaseNo, string ItemID, string RequestType)
        {
            int totalRecordCount;
            int searchRecordCount;


            switch (RequestType)
            {
                case "CO":
                    InMemoryVendorRequestsRepository.AllVendorRequests = VendorPortalDb.VendorRequests
                        .Where(t => t.OrderNo.Equals(OrderNo) && t.LineNo == LineNo && t.ReleaseNo == ReleaseNo && t.RequestCategoryID == (int)VendorRequestCategoryID.CORequest)
                        .ToList();
                    break;
                case "PO":
                    InMemoryVendorRequestsRepository.AllVendorRequests = VendorPortalDb.VendorRequests
                        .Where(t => t.OrderNo.Equals(OrderNo) && t.LineNo == LineNo && t.ReleaseNo == ReleaseNo && t.RequestCategoryID == (int)VendorRequestCategoryID.PORequest)
                        .ToList();
                    break;
                case "TO":
                    InMemoryVendorRequestsRepository.AllVendorRequests = VendorPortalDb.VendorRequests
                        .Where(t => t.OrderNo.Equals(OrderNo) && t.LineNo == LineNo && t.RequestCategoryID == (int)VendorRequestCategoryID.TORequest)
                        .ToList();
                    break;
                case "Item":
                    InMemoryVendorRequestsRepository.AllVendorRequests = VendorPortalDb.VendorRequests
                        .Where(t => t.Item.Equals(ItemID) && t.Processed == false && t.RequestCategoryID == (int)VendorRequestCategoryID.ItemRequest)
                        .ToList();
                    break;
            }
            


            var objItems = InMemoryVendorRequestsRepository.GetVendorRequests(startIndex: jQueryDataTablesModel.iDisplayStart,
                pageSize: jQueryDataTablesModel.iDisplayLength, sortedColumns: jQueryDataTablesModel.GetSortedColumns(),
                totalRecordCount: out totalRecordCount, searchRecordCount: out searchRecordCount, searchString: jQueryDataTablesModel.sSearch);

            return this.DataTablesJson(items: objItems,
                totalRecords: totalRecordCount,
                totalDisplayRecords: searchRecordCount,
                sEcho: jQueryDataTablesModel.sEcho);
        }

        [HttpPost]
        public JsonResult AllWarehouses()
        {
            using (SytelineDbEntities db = new SytelineDbEntities())
            {
                /*I had to build my array this way in order to send the collection to my javascript in the correct JSON format, 
                 * However, I could just send the collection (using ToList()) to the javascript and then access it via data[1].propertyname from my $.post call*/
                var Warehouses = db.whses
                    .Select(g => new { g.whse1, g.name })
                    .ToList();
                var jsonWarehouses = Warehouses.Select(c => new[] { c.whse1.ToString(),
                    string.IsNullOrEmpty(c.name)? string.Empty:c.name.ToString() });

                return Json(jsonWarehouses);
            }
        }

        [HttpPost]
        public JsonResult UserWarehouses()
        {
            UsersContext context;
            UserProfile user;
            IEnumerable<string> objUserWhses;


            context = new UsersContext();
            user = context.UserProfiles.SingleOrDefault(u => u.UserName == User.Identity.Name);
            objUserWhses = user.Warehouses.SplitNTrim(); //put the users warehouses into a list of trimmed strings

            using (SytelineDbEntities db = new SytelineDbEntities())
            {
                /*I had to build my array this way in order to send the collection to my javascript in the correct JSON format, 
                 * However, I could just send the collection (using ToList()) to the javascript and then access it via data[1].propertyname from my $.post call*/
                var Warehouses = db.whses.Where(w => objUserWhses.Contains(w.whse1))
                    .Select(g => new { g.whse1, g.name })
                    .ToList();
                var jsonWarehouses = Warehouses.Select(c => new[] { c.whse1.ToString(),
                    string.IsNullOrEmpty(c.name)? string.Empty:c.name.ToString() });

                return Json(jsonWarehouses);
            }
        }

        [HttpPost]
        public JsonResult RequestCategories(int? RequestCategoryID, string ReturnAll)//I modified this action so that I could specify a parameter that would return all of the Request Categories.
        {
            List<RequestCategory> objRequestCategories;

            objRequestCategories = VendorPortalDb.RequestCategories.ToList();

            if (!string.IsNullOrEmpty(ReturnAll) && ReturnAll.ToUpper().Equals("TRUE")){}
            else
            {
                objRequestCategories = objRequestCategories
                .Where(r => r.ID == RequestCategoryID)
                .ToList();
            }
            
            var RequestCategoryList = objRequestCategories
                .Select(g => new { g.Code, g.Description })
                .ToList();

            var jsonRequestCategories = RequestCategoryList
                .Select(c => new[] { c.Code, c.Description });

            return Json(jsonRequestCategories);
        }

        [HttpPost]
        public JsonResult UpdateNote(int? ID, string NoteContent)
        {
            string strSQL;
            bool blnResult;


            //make sure to replace the single quotes with double single quotes in order to escape them
            strSQL = QueryDefinitions.GetQuery("UpdateVendorRequest", new string[] { "Notes", NoteContent.Replace("'", "''"), ID.ToString(), User.Identity.Name });

            if (VendorPortalDb.Database.ExecuteSqlCommand(strSQL) == 1)
                blnResult = true;
            else
                blnResult = false;

            var objVendorRequest = VendorPortalDb.VendorRequests
                    .Where(v => v.ID == ID)
                    .FirstOrDefault();

            return Json(new { Success = blnResult, objVendorRequest.HTMLNotes });
        }

        [HttpPost]
        public string UpdateVR(int? ID, string value, int? rowId, int? columnPosition, int? columnId, string columnName)
        {
            string strSQL;
            bool blnTemp;
            VendorRequest objVendorRequest;
            int intRecordsAffected;


            try
            {
                objVendorRequest = VendorPortalDb.VendorRequests
                .Where(v => v.ID == ID)
                .SingleOrDefault();


                //if the user is not in these roles I must check if the transaction has been processed; Users cannot update a vendor request after it has been processed.
                if (!User.IsInRoles("Admin, QueueAdmin"))
                    if (objVendorRequest.Processed)
                        return "No updates are allowed because Vendor Request has been processed";

                //if this is the processed column being updated, I don't want to have the updater field updated 
                if (columnName.Trim().ToUpper().Equals("PROCESSED"))
                {
                    objVendorRequest.Processed = bool.TryParse(value, out blnTemp) ? blnTemp : false;

                    if (objVendorRequest.Processed)//if processed is being updated from false to true
                        objVendorRequest.DateProcessed = DateTime.Now;
                    else //the transaction is being set to unprocessed and so DateProcessed must reflect that...
                        objVendorRequest.DateProcessed = SharedVariables.MINDATE;

                    intRecordsAffected = VendorPortalDb.SaveChanges();
                }
                else
                {
                    strSQL = QueryDefinitions.GetQuery("UpdateVendorRequest", new string[] { columnName, value, ID.ToString(), User.Identity.Name });

                    intRecordsAffected = VendorPortalDb.Database.ExecuteSqlCommand(strSQL);
                }

                if (intRecordsAffected == 1)//the modified record was successfully updated
                    return value;
                else
                    return "Value Was not Updated!";

            }
            catch (Exception objEx) { return objEx.Message; }
            
        }

        [HttpPost]
        public string DeleteVR(int? ID)
        {
            VendorRequest objVendorRequest;
            

            try
            {
                objVendorRequest = VendorPortalDb.VendorRequests
                    .Where(v => v.ID == ID)
                    .SingleOrDefault();

                if (objVendorRequest.Processed)
                    return "This Vendor Request cannot be deleted since has been processed";

                VendorPortalDb.VendorRequests.Remove(objVendorRequest);
                VendorPortalDb.SaveChanges();
                return "ok";
            }
            catch (Exception objEx) { return objEx.Message; }

        }

        [HttpPost]
        public string AddVR(VendorRequest objRequest)
        {

            try
            {
                objRequest.Creator = User.Identity.Name;
                objRequest.Updater = User.Identity.Name;
                VendorPortalDb.VendorRequests.Add(objRequest);
                VendorPortalDb.SaveChanges();
                var temp = objRequest.ID;
                return objRequest.ID.ToString();
                //return "ok";
            }
            catch (Exception objEx)
            {
                return objEx.Message;
            }
        }
    }
}
