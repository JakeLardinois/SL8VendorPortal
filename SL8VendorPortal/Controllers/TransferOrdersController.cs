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
using Microsoft.Reporting.WebForms;
using System.Text;

namespace SL8VendorPortal.Controllers
{
    [Authorize(Roles = "Admin,User")]
    public class TransferOrdersController : Controller
    {
        private SytelineDbEntities db = new SytelineDbEntities();
        private UserProfile CurrentUserProfile;


        [HttpGet]
        public ActionResult Search()
        {
            UsersContext context;
            UserProfile user;
            IEnumerable<string> objUserWhses;
            List<whse> objWarehouseList;

            //Add the request types for the dropdown list
            //ViewData["RequestCategoryCode"] = new SelectList(new[] { new SelectListItem { Text = "TOReciept", Value = "TOReciept" }, 
            //        new SelectListItem { Text = "TOShipment", Value = "TOShipment" }},
            //        "Value", "Text", "TOReciept");
            //I changed the above manual entry of the Request Categorie listing to one that is drawn from the database
            using (SL8VendorPortalDb VendorPortalDb = new SL8VendorPortalDb())
            {
                ViewData["RequestCategoryCode"] = new SelectList(VendorPortalDb.RequestCategories.Where(r => r.ID == 2).ToList(),
                    "Code", "Description", "TOReciept");
            }

            context = new UsersContext();
            user = context.UserProfiles.SingleOrDefault(u => u.UserName == User.Identity.Name);
            objUserWhses = user.Warehouses.SplitNTrim(); //put the users warehouses into a list of trimmed strings

            objWarehouseList = db.whses.ToList();//get all the warehouses from the database

            //Add the users source warehouses to ViewData so the selectlist can access them
            ViewData["SourceWarehouses"] = new SelectList(
                objWarehouseList.Where(w => objUserWhses.Contains(w.whse1)).ToList(),//This is how you go about the SQL IN clause in Linq. 
                "whse1", "name");

            //Add all the warehouses to the destination; a user can send inventory to any of our warehouses
            ViewData["DestWarehouses"] = new SelectList(
                objWarehouseList, "whse1", "name", 
                objWarehouseList.Where(w => w.whse1.Equals("MAIN")).SingleOrDefault().whse1);//Sets the default to the MAIN warehouse which is WTF

            return View("Search");
        }

        [HttpPost]
        public JsonResult Search(JQueryDataTablesModel jQueryDataTablesModel)
        {
            int totalRecordCount;
            int searchRecordCount;
            string strSQL;
            StringBuilder objStrBldr;


            CurrentUserProfile = new UsersContext().UserProfiles.SingleOrDefault(u => u.UserName == User.Identity.Name);
            //strSQL = QueryDefinitions.GetQuery("SelectTransferOrdersByToWarehousesAndStatus", new string[] { user.Warehouses.AddSingleQuotes(), "O, T".AddSingleQuotes() });//O is for Ordered, T is for Transit, etc.
            strSQL = QueryDefinitions.GetQuery("SelectTOByLineToFromWarehousesAndStatuses", new string[] { CurrentUserProfile.Warehouses.AddSingleQuotes(), "O, T".AddSingleQuotes() });//O is for Ordered, T is for Transit, etc.

            //get the pertinant transfer orders
            var objTOList = db.transfers.SqlQuery(strSQL).ToList();

            //instantiate my stringbuilder and then develop my list of co_nums for the query
            objStrBldr = new StringBuilder();
            foreach (var objTO in objTOList)
                objStrBldr.Append(objTO.trn_num + ", ");

            if (objStrBldr.Length == 0)//prevents an error in the scenario where no records are returned...
                objStrBldr.Append("''");

            //Build the trnitem sql
            strSQL = QueryDefinitions.GetQuery("SelectTOLinesByToFromWhsesAndStatusAndOrderNoList", new string[] { CurrentUserProfile.Warehouses.AddSingleQuotes(), "O, T".AddSingleQuotes(), objStrBldr.ToString().AddSingleQuotesAndPadLeft(10) });
            //get the list of trnitems...
            var objTRNItemList = db.trnitems.SqlQuery(strSQL).ToList();

            //Set the trnitems property on each to to the pertinant list 
            foreach (var objTO in objTOList)
                objTO.trnitems = objTRNItemList
                    .Where(t => t.trn_num.Equals(objTO.trn_num));


            InMemoryTransferOrdersRepository.AllTransferOrders = objTOList;

            var objItems = InMemoryTransferOrdersRepository.GetTransferOrders(startIndex: jQueryDataTablesModel.iDisplayStart,
                pageSize: jQueryDataTablesModel.iDisplayLength, sortedColumns: jQueryDataTablesModel.GetSortedColumns(),
                totalRecordCount: out totalRecordCount, searchRecordCount: out searchRecordCount, searchString: jQueryDataTablesModel.sSearch);

            return this.DataTablesJson(items: objItems,
                totalRecords: totalRecordCount,
                totalDisplayRecords: searchRecordCount,
                sEcho: jQueryDataTablesModel.sEcho);
        }

        [HttpPost]
        public JsonResult SearchTOLinesByOrder(JQueryDataTablesModel jQueryDataTablesModel, string OrderNo)//This gets passed as a querystring by the ajax url in Views/CustomerOrders/Search.cshtml for the nested table
        {
            int totalRecordCount;
            int searchRecordCount;
            string strSQL;


            CurrentUserProfile = new UsersContext().UserProfiles.SingleOrDefault(u => u.UserName == User.Identity.Name);
            strSQL = QueryDefinitions.GetQuery("SelectTOLinesByToFromWhsesAndStatusAndOrderNo", new string[] { CurrentUserProfile.Warehouses.AddSingleQuotes(), "O, T".AddSingleQuotes(), OrderNo });//O is for Ordered, T is for Transit, etc.

            InMemoryTransferOrderLinesRepository.AllTransferOrderLines = db.trnitems.SqlQuery(strSQL).ToList();

            var objItems = InMemoryTransferOrderLinesRepository.GetTransferOrderLines(startIndex: jQueryDataTablesModel.iDisplayStart,
                pageSize: jQueryDataTablesModel.iDisplayLength, sortedColumns: jQueryDataTablesModel.GetSortedColumns(),
                totalRecordCount: out totalRecordCount, searchRecordCount: out searchRecordCount, searchString: jQueryDataTablesModel.sSearch);

            return this.DataTablesJson(items: objItems,
                totalRecords: totalRecordCount,
                totalDisplayRecords: searchRecordCount,
                sEcho: jQueryDataTablesModel.sEcho);
        }

        public ActionResult GenerateTOReport(JQueryDataTablesModel jQueryDataTablesModel)
        {
            int totalRecordCount;
            int searchRecordCount;
            string strSQL;


            CurrentUserProfile = new UsersContext().UserProfiles.SingleOrDefault(u => u.UserName == User.Identity.Name);
            strSQL = QueryDefinitions.GetQuery("SelectTOByLineToFromWarehousesAndStatuses", new string[] { CurrentUserProfile.Warehouses.AddSingleQuotes(), "O, T".AddSingleQuotes() });//This will only bring in Orders where there are corresponding Open Order Lines.

            InMemoryTransferOrdersRepository.AllTransferOrders = db.transfers.SqlQuery(strSQL).ToList();

            var objItems = InMemoryTransferOrdersRepository.GetTransferOrders(startIndex: jQueryDataTablesModel.iDisplayStart,
                pageSize: jQueryDataTablesModel.iDisplayLength, sortedColumns: jQueryDataTablesModel.GetSortedColumns(),
                totalRecordCount: out totalRecordCount, searchRecordCount: out searchRecordCount, searchString: jQueryDataTablesModel.sSearch, isDownloadReport: true);

            //Add the Order Notes
            foreach (transfer objTransfer in objItems)
            {
                objTransfer.Notes = new Notes(objTransfer.trn_num, NoteType.TO);

                //iterate on the notes collection and add the text to the AllNotesText Property...
                foreach (SytelineNote objSLNote in objTransfer.Notes)
                {
                    if (objSLNote.IsInternal == 0)//only add external notes
                        objTransfer.AllNotesText += objSLNote.NoteContent + Environment.NewLine;
                }
            }

            RenderTOReport(objItems);

            return View();
        }

        private void RenderTOReport(IList<transfer> objItems)
        {
            string strReportType = "Excel";
            LocalReport objLocalReport;
            ReportDataSource TransferOrdersDataSource;
            string mimeType;
            string encoding;
            string fileNameExtension;
            string deviceInfo = "";
            Warning[] warnings;
            string[] streams;


            //objLocalReport = new LocalReport { ReportPath = Server.MapPath("~/Reports/PurchaseOrders.rdlc") };
            //objLocalReport = new LocalReport { ReportPath = Server.MapPath("~/bin/Reports/CustomerOrders.rdlc") };
            objLocalReport = new LocalReport { ReportPath = Server.MapPath(Settings.ReportDirectory + "TransferOrders.rdlc") };

            objLocalReport.SubreportProcessing += new SubreportProcessingEventHandler(MySubreportEventHandler);

            //Give the reportdatasource a name so that we can reference it in our report designer
            TransferOrdersDataSource = new ReportDataSource("TOs", objItems);

            objLocalReport.DataSources.Add(TransferOrdersDataSource);
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
            Response.ClearHeaders();
            Response.ClearContent();
            Response.ContentType = mimeType;
            Response.AddHeader("content-disposition", "attachment; filename=TransferOrders" + DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + "." + fileNameExtension);
            Response.BinaryWrite(renderedBytes);
            Response.End();
        }

        void MySubreportEventHandler(object sender, SubreportProcessingEventArgs e)
        {
            List<trnitem> objTOItems;


            var objParam = e.Parameters.Where(p => p.Name.Equals("OrderNum"))
                .SingleOrDefault();

            //Get the lines for each order and add them to the Order...
            objTOItems = db.trnitems.SqlQuery(QueryDefinitions.GetQuery("SelectTOLinesByToFromWhsesAndStatusAndOrderNo", new string[] { CurrentUserProfile.Warehouses.AddSingleQuotes(), "O, T".AddSingleQuotes(), objParam.Values[0] }))
                .ToList();

            foreach (trnitem objTOItem in objTOItems)
            {
                objTOItem.Notes = new Notes(objTOItem.trn_num, objTOItem.trn_line, NoteType.TOLine);

                //iterate on the notes collection and add the text to the AllNotesText Property...
                foreach (SytelineNote objSLNote in objTOItem.Notes)
                {
                    if (objSLNote.IsInternal == 0)//only add external notes
                        objTOItem.AllNotesText += objSLNote.NoteContent + Environment.NewLine;
                }
            }

            e.DataSources.Add(new ReportDataSource("TrnItems", objTOItems));
        }

        #region Unused Code
        //[Authorize]
        //public ActionResult Index()
        //{
        //    var context = new UsersContext();
        //    var user = context.UserProfiles.SingleOrDefault(u => u.UserName == User.Identity.Name);
        //    var strSQL = QueryDefinitions.GetQuery("SelectTransferOrdersByToWarehouses", new string[] { user.Warehouses.AddSingleQuotes() });

            
        //    return View(db.transfers.SqlQuery(strSQL).ToList());
        //}

        ////
        //// GET: /TransferOrders/Details/5
        //public ActionResult Details(string id = null)
        //{
        //    transfer transfer = db.transfers.Find(id);
        //    if (transfer == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(transfer);
        //}


        ////
        //// GET: /TransferOrders/Create

        //public ActionResult Create()
        //{
        //    return View();
        //}

        ////
        //// POST: /TransferOrders/Create

        //[HttpPost]
        //public ActionResult Create(transfer transfer)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.transfers.Add(transfer);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    return View(transfer);
        //}

        ////
        //// GET: /TransferOrders/Edit/5

        //public ActionResult Edit(string id = null)
        //{
        //    transfer transfer = db.transfers.Find(id);
        //    if (transfer == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(transfer);
        //}

        ////
        //// POST: /TransferOrders/Edit/5

        //[HttpPost]
        //public ActionResult Edit(transfer transfer)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(transfer).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    return View(transfer);
        //}

        ////
        //// GET: /TransferOrders/Delete/5

        //public ActionResult Delete(string id = null)
        //{
        //    transfer transfer = db.transfers.Find(id);
        //    if (transfer == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(transfer);
        //}

        ////
        //// POST: /TransferOrders/Delete/5

        //[HttpPost, ActionName("Delete")]
        //public ActionResult DeleteConfirmed(string id)
        //{
        //    transfer transfer = db.transfers.Find(id);
        //    db.transfers.Remove(transfer);
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