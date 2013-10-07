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
    public class PurchaseOrdersController : Controller
    {
        private SytelineDbEntities db = new SytelineDbEntities();
        private UserProfile CurrentUserProfile;


        [HttpGet]
        public ActionResult Search()
        {
            UsersContext context;
            UserProfile user;
            IEnumerable<string> objUserWhses;


            using (SL8VendorPortalDb VendorPortalDb = new SL8VendorPortalDb())
            {
                ViewData["RequestCategoryCode"] = new SelectList(VendorPortalDb.RequestCategories.Where(r => r.ID == 1).ToList(),
                    "Code", "Description", "POReciept");
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
            //strSQL = QueryDefinitions.GetQuery("SelectPurchaseOrdersByWarehousesAndStatus", new string[] { user.Warehouses.AddSingleQuotes(), "O" });//O is for Ordered, C is for Complete, etc.
            strSQL = QueryDefinitions.GetQuery("SelectPOByLineWarehousesAndStatus", new string[] { CurrentUserProfile.Warehouses.AddSingleQuotes(), "O" });//This will only bring in Orders where there are corresponding Open Order Lines.

            InMemoryPurchaseOrdersRepository.AllPurchaseOrders = db.poes.SqlQuery(strSQL).ToList();


            var objItems = InMemoryPurchaseOrdersRepository.GetPurchaseOrders(startIndex: jQueryDataTablesModel.iDisplayStart,
                pageSize: jQueryDataTablesModel.iDisplayLength, sortedColumns: jQueryDataTablesModel.GetSortedColumns(),
                totalRecordCount: out totalRecordCount, searchRecordCount: out searchRecordCount, searchString: jQueryDataTablesModel.sSearch);

            return this.DataTablesJson(items: objItems,
                totalRecords: totalRecordCount,
                totalDisplayRecords: searchRecordCount,
                sEcho: jQueryDataTablesModel.sEcho);
        }

        [HttpPost]
        public JsonResult SearchPOLinesByOrder(JQueryDataTablesModel jQueryDataTablesModel, string OrderNo)//This gets passed as a querystring by the ajax url in Views/CustomerOrders/Search.cshtml for the nested table
        {
            int totalRecordCount;
            int searchRecordCount;
            string strSQL;


            CurrentUserProfile = new UsersContext().UserProfiles.SingleOrDefault(u => u.UserName == User.Identity.Name);
            strSQL = QueryDefinitions.GetQuery("SelectPOLinesByWarehousesAndStatusAndOrderNo", new string[] { CurrentUserProfile.Warehouses.AddSingleQuotes(), "O", OrderNo });//O is for Ordered, C is for Complete, etc.
            //strSQL = QueryDefinitions.GetQuery("SelectPOLinesByWarehousesAndStatus", new string[] { user.Warehouses.AddSingleQuotes(), "O", OrderNo });//O is for Ordered, C is for Complete, etc.

            InMemoryPurchaseOrderLinesRepository.AllPurchaseOrderLines = db.poitems.SqlQuery(strSQL).ToList();

            var objItems = InMemoryPurchaseOrderLinesRepository.GetPurchaseOrderLines(startIndex: jQueryDataTablesModel.iDisplayStart,
                pageSize: jQueryDataTablesModel.iDisplayLength, sortedColumns: jQueryDataTablesModel.GetSortedColumns(),
                totalRecordCount: out totalRecordCount, searchRecordCount: out searchRecordCount, searchString: jQueryDataTablesModel.sSearch);

            return this.DataTablesJson(items: objItems,
                totalRecords: totalRecordCount,
                totalDisplayRecords: searchRecordCount,
                sEcho: jQueryDataTablesModel.sEcho);
        }

        [HttpPost]
        public JsonResult GetDropShipAddress(string ShipAddr, string DropShipNo, int? SeqNo)
        {
            return Json(BuildDropShipAddress(ShipAddr, DropShipNo, SeqNo, true));
        }

        public ActionResult GeneratePOReport(JQueryDataTablesModel jQueryDataTablesModel)
        {
            int totalRecordCount;
            int searchRecordCount;
            string strSQL;


            CurrentUserProfile = new UsersContext().UserProfiles.SingleOrDefault(u => u.UserName == User.Identity.Name);
            strSQL = QueryDefinitions.GetQuery("SelectPOByLineWarehousesAndStatus", new string[] { CurrentUserProfile.Warehouses.AddSingleQuotes(), "O" });//This will only bring in Orders where there are corresponding Open Order Lines.

            InMemoryPurchaseOrdersRepository.AllPurchaseOrders = db.poes.SqlQuery(strSQL).ToList();

            var objItems = InMemoryPurchaseOrdersRepository.GetPurchaseOrders(startIndex: jQueryDataTablesModel.iDisplayStart,
                pageSize: jQueryDataTablesModel.iDisplayLength, sortedColumns: jQueryDataTablesModel.GetSortedColumns(),
                totalRecordCount: out totalRecordCount, searchRecordCount: out searchRecordCount, searchString: jQueryDataTablesModel.sSearch);

            //Add the Order Notes
            foreach (po objPO in objItems)
            {
                objPO.Notes = new Notes(objPO.po_num, NoteType.PO);

                //iterate on the notes collection and add the text to the AllNotesText Property...
                foreach (SytelineNote objSLNote in objPO.Notes)
                {
                    if (objSLNote.IsInternal == 0)//only add external notes
                        objPO.AllNotesText += objSLNote.NoteContent + Environment.NewLine;
                }
            }

            RenderPOReport(objItems);

            return View();
        }

        private void RenderPOReport(IList<po> objItems)
        {
            string strReportType = "Excel";
            LocalReport objLocalReport;
            ReportDataSource PurchaseOrdersDataSource;
            string mimeType;
            string encoding;
            string fileNameExtension;
            string deviceInfo = "";
            Warning[] warnings;
            string[] streams;


            //objLocalReport = new LocalReport { ReportPath = Server.MapPath("~/Reports/PurchaseOrders.rdlc") };
            //objLocalReport = new LocalReport { ReportPath = Server.MapPath("~/bin/Reports/CustomerOrders.rdlc") };
            objLocalReport = new LocalReport { ReportPath = Server.MapPath(Settings.ReportDirectory + "PurchaseOrders.rdlc") };

            objLocalReport.SubreportProcessing += new SubreportProcessingEventHandler(MySubreportEventHandler);

            //Give the reportdatasource a name so that we can reference it in our report designer
            PurchaseOrdersDataSource = new ReportDataSource("POs", objItems);

            objLocalReport.DataSources.Add(PurchaseOrdersDataSource);
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
            Response.AddHeader("content-disposition", "attachment; filename=PurchaseOrders" + DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + "." + fileNameExtension);
            Response.BinaryWrite(renderedBytes);
            Response.End();
        }

        void MySubreportEventHandler(object sender, SubreportProcessingEventArgs e)
        {
            List<SytelineNote> objLineAndReleaseNotes;
            List<poitem> objPOItems;


            var objParam = e.Parameters.Where(p => p.Name.Equals("OrderNum"))
                .SingleOrDefault();

            //Get the lines for each order and add them to the Order...
            objPOItems = db.poitems.SqlQuery(QueryDefinitions.GetQuery("SelectPOLinesByWarehousesAndStatusAndOrderNo", new string[] { CurrentUserProfile.Warehouses.AddSingleQuotes(), "O", objParam.Values[0] }))
                .ToList();

            foreach (poitem objPOItem in objPOItems)
            {
                objLineAndReleaseNotes = new List<SytelineNote>();
                //Add the Line Notes
                objLineAndReleaseNotes.AddRange(new Notes(objPOItem.po_num, objPOItem.po_line, NoteType.POLine));
                //Add the Release Notes
                objLineAndReleaseNotes.AddRange(new Notes(objPOItem.po_num, objPOItem.po_line, objPOItem.po_release, NoteType.POLineRelease));
                //add the Line and Release Notes to the Line record
                objPOItem.Notes = objLineAndReleaseNotes;

                //iterate on the notes collection and add the text to the AllNotesText Property...
                foreach (SytelineNote objSLNote in objPOItem.Notes)
                {
                    if (objSLNote.IsInternal == 0)//only add external notes
                        objPOItem.AllNotesText += objSLNote.NoteContent + Environment.NewLine;
                }

                objPOItem.DropShipAddress = BuildDropShipAddress(objPOItem.ship_addr, objPOItem.drop_ship_no, objPOItem.drop_seq);
            }

            e.DataSources.Add(new ReportDataSource("POItems", objPOItems));
        }

        private string BuildDropShipAddress(string strShipAddr, string strDropShipNo, int? intSeqNo, bool blnUseHTML = false)
        {
            StringBuilder strbldrAddress;
            string strNewline;


            if (blnUseHTML)
                strNewline = "<br />";
            else
                strNewline = Environment.NewLine;

            strbldrAddress = new StringBuilder();
            switch (strShipAddr)
            {
                case "N": //None
                    break;
                case "W": //Warehouse
                    var objWhse = db.whses
                    .Where(w => w.whse1.Equals(strDropShipNo))
                    .SingleOrDefault();

                    if (objWhse != null)
                    {
                        strbldrAddress.Append(string.IsNullOrEmpty(objWhse.name) ? string.Empty : objWhse.name + strNewline);
                        strbldrAddress.Append(string.IsNullOrEmpty(objWhse.addr__1) ? string.Empty : objWhse.addr__1 + strNewline);
                        strbldrAddress.Append(string.IsNullOrEmpty(objWhse.addr__2) ? string.Empty : objWhse.addr__2 + strNewline);
                        strbldrAddress.Append(string.IsNullOrEmpty(objWhse.addr__3) ? string.Empty : objWhse.addr__3 + strNewline);
                        strbldrAddress.Append(string.IsNullOrEmpty(objWhse.addr__4) ? string.Empty : objWhse.addr__4 + strNewline);
                        strbldrAddress.Append(objWhse.city + ", " + objWhse.state + " " + objWhse.zip + strNewline);
                        strbldrAddress.Append(objWhse.country);
                    }
                    break;
                case "D": //Drop Ship To
                    var objVendAddr = db.vendaddrs
                        .Where(v => v.vend_num.Equals(strDropShipNo))
                        .SingleOrDefault();

                    if (objVendAddr != null)
                    {
                        strbldrAddress.Append(string.IsNullOrEmpty(objVendAddr.name) ? string.Empty : objVendAddr.name + strNewline);
                        strbldrAddress.Append(string.IsNullOrEmpty(objVendAddr.addr__1) ? string.Empty : objVendAddr.addr__1 + strNewline);
                        strbldrAddress.Append(string.IsNullOrEmpty(objVendAddr.addr__2) ? string.Empty : objVendAddr.addr__2 + strNewline);
                        strbldrAddress.Append(string.IsNullOrEmpty(objVendAddr.addr__3) ? string.Empty : objVendAddr.addr__3 + strNewline);
                        strbldrAddress.Append(string.IsNullOrEmpty(objVendAddr.addr__4) ? string.Empty : objVendAddr.addr__4 + strNewline);
                        strbldrAddress.Append(objVendAddr.city + ", " + objVendAddr.state + " " + objVendAddr.zip + strNewline);
                        strbldrAddress.Append(objVendAddr.country);
                    }

                    break;
                case "C": //Custsomer
                    var objCustAddr = db.custaddrs
                        .Where(c => c.cust_num.Equals(strDropShipNo) && c.cust_seq == intSeqNo)
                        .SingleOrDefault();

                    if (objCustAddr != null)
                    {
                        strbldrAddress.Append(string.IsNullOrEmpty(objCustAddr.name) ? string.Empty : objCustAddr.name + strNewline);
                        strbldrAddress.Append(string.IsNullOrEmpty(objCustAddr.addr__1) ? string.Empty : objCustAddr.addr__1 + strNewline);
                        strbldrAddress.Append(string.IsNullOrEmpty(objCustAddr.addr__2) ? string.Empty : objCustAddr.addr__2 + strNewline);
                        strbldrAddress.Append(string.IsNullOrEmpty(objCustAddr.addr__3) ? string.Empty : objCustAddr.addr__3 + strNewline);
                        strbldrAddress.Append(string.IsNullOrEmpty(objCustAddr.addr__4) ? string.Empty : objCustAddr.addr__4 + strNewline);
                        strbldrAddress.Append(objCustAddr.city + ", " + objCustAddr.state + " " + objCustAddr.zip + strNewline);
                        strbldrAddress.Append(objCustAddr.country);
                    }
                    break;
            }

            return strbldrAddress.ToString();
        }

        #region Unused Code
        ////
        //// GET: /PurchaseOrders/

        //public ActionResult Index()
        //{
        //    var context = new UsersContext();
        //    var user = context.UserProfiles.SingleOrDefault(u => u.UserName == User.Identity.Name);
        //    var strSQL = QueryDefinitions.GetQuery("SelectPurchaseOrdersByWarehousesAndStatus", new string[] { user.Warehouses.AddSingleQuotes(), "O" });//O is for Ordered, C is for Complete, etc.


        //    return View(db.poes.SqlQuery(strSQL).ToList());
        //}

        ////
        //// GET: /PurchaseOrders/Details/5

        //public ActionResult Details(string id = null)
        //{
        //    po po = db.poes.Find(id);
        //    if (po == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(po);
        //}

        ////
        //// GET: /PurchaseOrders/Create

        //public ActionResult Create()
        //{
        //    return View();
        //}

        ////
        //// POST: /PurchaseOrders/Create

        //[HttpPost]
        //public ActionResult Create(po po)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.poes.Add(po);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    return View(po);
        //}

        ////
        //// GET: /PurchaseOrders/Edit/5

        //public ActionResult Edit(string id = null)
        //{
        //    po po = db.poes.Find(id);
        //    if (po == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(po);
        //}

        ////
        //// POST: /PurchaseOrders/Edit/5

        //[HttpPost]
        //public ActionResult Edit(po po)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(po).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    return View(po);
        //}

        ////
        //// GET: /PurchaseOrders/Delete/5

        //public ActionResult Delete(string id = null)
        //{
        //    po po = db.poes.Find(id);
        //    if (po == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(po);
        //}

        ////
        //// POST: /PurchaseOrders/Delete/5

        //[HttpPost, ActionName("Delete")]
        //public ActionResult DeleteConfirmed(string id)
        //{
        //    po po = db.poes.Find(id);
        //    db.poes.Remove(po);
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