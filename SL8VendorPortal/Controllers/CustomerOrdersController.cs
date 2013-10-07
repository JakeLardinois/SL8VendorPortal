using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SL8VendorPortal.Models;

using SL8VendorPortal.Infrastructure;
using jQuery.DataTables.Mvc;
using System.Text;
using Microsoft.Reporting.WebForms; //I needed to add Microsoft.ReportViewer.WebForms reference before it became available to add via "ctr+."


namespace SL8VendorPortal.Controllers
{
    [Authorize(Roles = "Admin,User")]
    public class CustomerOrdersController : Controller
    {
        private SytelineDbEntities db = new SytelineDbEntities();
        private UserProfile CurrentUserProfile;

        [HttpGet]
        public ActionResult Search()
        {
            UsersContext context;
            UserProfile user;
            IEnumerable<string> objUserWhses;


            //Add the request types for the dropdown list
            using (SL8VendorPortalDb VendorPortalDb = new SL8VendorPortalDb())
            {
                ViewData["RequestCategoryCode"] = new SelectList(VendorPortalDb.RequestCategories.Where(r => r.ID == 0).ToList(),
                    "Code", "Description", "COShipment");
            }

            context = new UsersContext();
            user = context.UserProfiles.SingleOrDefault(u => u.UserName == User.Identity.Name);
            objUserWhses = user.Warehouses.SplitNTrim(); //put the users warehouses into a list of trimmed strings

            //Add the users source warehouses to ViewData so the selectlist can access them
            ViewData["SourceWarehouses"] = new SelectList(
                db.whses.Where(w => objUserWhses.Contains(w.whse1)).ToList(),//This is how you go about the SQL IN clause in Linq. 
                "whse1", "name");

            return View("Search");
        }

        [HttpPost]
        public JsonResult Search(JQueryDataTablesModel jQueryDataTablesModel)
        {
            int totalRecordCount;
            int searchRecordCount;
            string strSQL;

            CurrentUserProfile = new UsersContext().UserProfiles.SingleOrDefault(u => u.UserName == User.Identity.Name);
            //strSQL = QueryDefinitions.GetQuery("SelectCustomerOrdersByWarehousesAndStatus", new string[] { user.Warehouses.AddSingleQuotes(), "O" });//O is for Ordered, C is for Complete, etc.
            strSQL = QueryDefinitions.GetQuery("SelectCOByLineWarehousesAndStatus", new string[] { CurrentUserProfile.Warehouses.AddSingleQuotes(), "O" });//This will only bring in Orders where there are corresponding Open Order Lines.

            InMemoryCustomerOrdersRepository.AllCustomerOrders = db.coes.SqlQuery(strSQL).ToList();


            var objItems = InMemoryCustomerOrdersRepository.GetCustomerOrders(startIndex: jQueryDataTablesModel.iDisplayStart,
                pageSize: jQueryDataTablesModel.iDisplayLength, sortedColumns: jQueryDataTablesModel.GetSortedColumns(),
                totalRecordCount: out totalRecordCount, searchRecordCount: out searchRecordCount, searchString: jQueryDataTablesModel.sSearch);

            return this.DataTablesJson(items: objItems,
                totalRecords: totalRecordCount,
                totalDisplayRecords: searchRecordCount,
                sEcho: jQueryDataTablesModel.sEcho);
        }

        [HttpPost]
        public JsonResult SearchCOLinesByOrder(JQueryDataTablesModel jQueryDataTablesModel, string OrderNo)//This gets passed as a querystring by the ajax url in Views/CustomerOrders/Search.cshtml for the nested table
        {
            int totalRecordCount;
            int searchRecordCount;
            string strSQL;


            CurrentUserProfile = new UsersContext().UserProfiles.SingleOrDefault(u => u.UserName == User.Identity.Name);
            strSQL = QueryDefinitions.GetQuery("SelectCOLinesByWarehousesAndStatusAndOrderNo", new string[] { CurrentUserProfile.Warehouses.AddSingleQuotes(), "O", OrderNo });//O is for Ordered, C is for Complete, etc.
            //strSQL = QueryDefinitions.GetQuery("SelectCOLinesByWarehousesAndStatus", new string[] { user.Warehouses.AddSingleQuotes(), "O", OrderNo });//O is for Ordered, C is for Complete, etc.

            InMemoryCustomerOrderLinesRepository.AllCustomerOrderLines = db.coitems.SqlQuery(strSQL).ToList();

            var objItems = InMemoryCustomerOrderLinesRepository.GetCustomerOrderLines(startIndex: jQueryDataTablesModel.iDisplayStart,
                pageSize: jQueryDataTablesModel.iDisplayLength, sortedColumns: jQueryDataTablesModel.GetSortedColumns(),
                totalRecordCount: out totalRecordCount, searchRecordCount: out searchRecordCount, searchString: jQueryDataTablesModel.sSearch);

            return this.DataTablesJson(items: objItems,
                totalRecords: totalRecordCount,
                totalDisplayRecords: searchRecordCount,
                sEcho: jQueryDataTablesModel.sEcho);
        }

        [HttpPost]
        public JsonResult GetCustomerAddress(string CustNo, int? SeqNo)
        {
            StringBuilder objStrBldr;
            custaddr objCustAddr;


            objCustAddr = db.custaddrs
                .Where(c => c.cust_num.Equals(CustNo) && c.cust_seq == SeqNo)
                .SingleOrDefault();

            objStrBldr = new StringBuilder();
            objStrBldr.Append(string.IsNullOrEmpty(objCustAddr.name) ? string.Empty : objCustAddr.name + "<br />");
            objStrBldr.Append(string.IsNullOrEmpty(objCustAddr.addr__1) ? string.Empty : objCustAddr.addr__1 + "<br />");
            objStrBldr.Append(string.IsNullOrEmpty(objCustAddr.addr__2) ? string.Empty : objCustAddr.addr__2 + "<br />");
            objStrBldr.Append(string.IsNullOrEmpty(objCustAddr.addr__3) ? string.Empty : objCustAddr.addr__3 + "<br />");
            objStrBldr.Append(string.IsNullOrEmpty(objCustAddr.addr__4) ? string.Empty : objCustAddr.addr__4 + "<br />");
            objStrBldr.Append(objCustAddr.city + ", " + objCustAddr.state + " " + objCustAddr.zip + "<br />");
            objStrBldr.Append(objCustAddr.country);


            return Json(objStrBldr.ToString());
        }

        public ActionResult GenerateCOReport(JQueryDataTablesModel jQueryDataTablesModel)
        {
            int totalRecordCount;
            int searchRecordCount;
            string strSQL;
            StringBuilder strbldrCOLineSQL, strbldrAddress;
            List<SytelineNote> objLineAndReleaseNotes;


            CurrentUserProfile = new UsersContext().UserProfiles.SingleOrDefault(u => u.UserName == User.Identity.Name);
            strSQL = QueryDefinitions.GetQuery("SelectCOByLineWarehousesAndStatus", new string[] { CurrentUserProfile.Warehouses.AddSingleQuotes(), "O" });//This will only bring in Orders where there are corresponding Open Order Lines.


            InMemoryCustomerOrdersRepository.AllCustomerOrders = db.coes.SqlQuery(strSQL).ToList();


            var objItems = InMemoryCustomerOrdersRepository.GetCustomerOrders(startIndex: jQueryDataTablesModel.iDisplayStart,
                pageSize: jQueryDataTablesModel.iDisplayLength, sortedColumns: jQueryDataTablesModel.GetSortedColumns(),
                totalRecordCount: out totalRecordCount, searchRecordCount: out searchRecordCount, searchString: jQueryDataTablesModel.sSearch);

            strbldrCOLineSQL = new StringBuilder();
            foreach (co objCO in objItems)
            {
                //Get the lines for each order and add them to the Order...
                strbldrCOLineSQL.Clear();
                strbldrCOLineSQL.Append(QueryDefinitions.GetQuery("SelectCOLinesByWarehousesAndStatusAndOrderNo", new string[] { CurrentUserProfile.Warehouses.AddSingleQuotes(), "O", objCO.co_num }));
                objCO.coitems = db.coitems.SqlQuery(strbldrCOLineSQL.ToString()).ToList();

                //Add the Order Notes
                objCO.Notes = new Notes(objCO.co_num, NoteType.CO);
                foreach (coitem objCOItem in objCO.coitems)
                {
                    objLineAndReleaseNotes = new List<SytelineNote>();
                    //Add the Line Notes
                    objLineAndReleaseNotes.AddRange(new Notes(objCO.co_num, objCOItem.co_line, NoteType.COLine));
                    //Add the Release Notes
                    objLineAndReleaseNotes.AddRange(new Notes(objCO.co_num, objCOItem.co_line, objCOItem.co_release, NoteType.COLineRelease));
                    //add the Line and Release Notes to the Line record
                    objCOItem.Notes = objLineAndReleaseNotes; 

                    //Get the Address object from the database
                    var objCustAddr = db.custaddrs
                        .Where(c => c.cust_num.Equals(objCOItem.cust_num) && c.cust_seq == objCOItem.cust_seq)
                        .SingleOrDefault();
                    //Build my address string
                    strbldrAddress = new StringBuilder();
                    strbldrAddress.Append(string.IsNullOrEmpty(objCustAddr.name) ? string.Empty : objCustAddr.name + Environment.NewLine);
                    strbldrAddress.Append(string.IsNullOrEmpty(objCustAddr.addr__1) ? string.Empty : objCustAddr.addr__1 + Environment.NewLine);
                    strbldrAddress.Append(string.IsNullOrEmpty(objCustAddr.addr__2) ? string.Empty : objCustAddr.addr__2 + Environment.NewLine);
                    strbldrAddress.Append(string.IsNullOrEmpty(objCustAddr.addr__3) ? string.Empty : objCustAddr.addr__3 + Environment.NewLine);
                    strbldrAddress.Append(string.IsNullOrEmpty(objCustAddr.addr__4) ? string.Empty : objCustAddr.addr__4 + Environment.NewLine);
                    strbldrAddress.Append(objCustAddr.city + ", " + objCustAddr.state + " " + objCustAddr.zip + Environment.NewLine);
                    strbldrAddress.Append(objCustAddr.country);
                    //Add the address as a string to the line object
                    objCOItem.CustomerAddress = strbldrAddress.ToString(); 
                }
            }
            RenderCOReport(objItems);


            return View();
        }

        //private void RenderDRTag(CorrectiveAction objCA, int intQty)
        private void RenderCOReport(IList<co> objItems)
        {
            string strReportType = "Excel";
            LocalReport objLocalReport;
            string mimeType;
            string encoding;
            string fileNameExtension;
            string deviceInfo = "";
            Warning[] warnings;
            string[] streams;


            objLocalReport = new LocalReport { ReportPath = Server.MapPath("~/Reports/CustomerOrders.rdlc") };
            //objLocalReport = new LocalReport { ReportPath = Server.MapPath("~/bin/Reports/CustomerOrders.rdlc") };

            //Give the reportdatasource a name so that we can reference it in our report designer
            var CustomerOrdersDataSource = new ReportDataSource("COs", objItems);

            objLocalReport.DataSources.Add(CustomerOrdersDataSource);
            objLocalReport.Refresh();

            //The DeviceInfo settings should be changed based on the reportType
            //http://msdn2.microsoft.com/en-us/library/ms155397.aspx
            deviceInfo = string.Format(
                        "<DeviceInfo>" +
                        "<OmitDocumentMap>True</OmitDocumentMap>" +
                        "<OmitFormulas>True</OmitFormulas>" +
                        "<SimplePageHeaders>True</SimplePageHeaders>" +
                        "</DeviceInfo>", strReportType);

            //Render the report
            var renderedBytes = objLocalReport.Render(
                strReportType,
                deviceInfo,
                out mimeType,
                out encoding,
                out fileNameExtension,
                out streams,
                out warnings);

            //Clear the response stream and write the bytes to the outputstream
            //Set content-disposition to "attachment" so that user is prompted to take an action
            //on the file (open or save)
            Response.Clear();
            Response.ContentType = mimeType;
            Response.AddHeader("content-disposition", "attachment; filename=CustomerOrders" + DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + "." + fileNameExtension);
            Response.BinaryWrite(renderedBytes);
            Response.End();
        }

        #region Unused Code
        ////
        //// GET: /CustomerOrders/
        //public ActionResult Index()
        //{
        //    var context = new UsersContext();
        //    var user = context.UserProfiles.SingleOrDefault(u => u.UserName == User.Identity.Name);
        //    var strSQL = QueryDefinitions.GetQuery("SelectCustomerOrdersByWarehousesAndStatus", new string[] { user.Warehouses.AddSingleQuotes(), "O" });//O is for Ordered, C is for Complete, etc.


        //    return View(db.coes.SqlQuery(strSQL).ToList());
        //}

        ////
        //// GET: /CustomerOrders/Details/5

        //public ActionResult Details(string id = null)
        //{
        //    co co = db.coes.Find(id);
        //    if (co == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(co);
        //}

        ////
        //// GET: /CustomerOrders/Create

        //public ActionResult Create()
        //{
        //    return View();
        //}

        ////
        //// POST: /CustomerOrders/Create

        //[HttpPost]
        //public ActionResult Create(co co)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.coes.Add(co);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    return View(co);
        //}

        ////
        //// GET: /CustomerOrders/Edit/5

        //public ActionResult Edit(string id = null)
        //{
        //    co co = db.coes.Find(id);
        //    if (co == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(co);
        //}

        ////
        //// POST: /CustomerOrders/Edit/5

        //[HttpPost]
        //public ActionResult Edit(co co)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(co).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    return View(co);
        //}

        ////
        //// GET: /CustomerOrders/Delete/5

        //public ActionResult Delete(string id = null)
        //{
        //    co co = db.coes.Find(id);
        //    if (co == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(co);
        //}

        ////
        //// POST: /CustomerOrders/Delete/5

        //[HttpPost, ActionName("Delete")]
        //public ActionResult DeleteConfirmed(string id)
        //{
        //    co co = db.coes.Find(id);
        //    db.coes.Remove(co);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}
#endregion

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}