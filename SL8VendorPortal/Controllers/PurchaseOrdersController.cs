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


namespace SL8VendorPortal.Controllers
{
    public class PurchaseOrdersController : Controller
    {
        private SytelineDbEntities db = new SytelineDbEntities();


        [HttpGet]
        public ActionResult Search()
        {
            return View("Search");
        }

        [HttpPost]
        public JsonResult Search(JQueryDataTablesModel jQueryDataTablesModel)
        {
            int totalRecordCount;
            int searchRecordCount;
            UsersContext context;
            UserProfile user;
            string strSQL;


            context = new UsersContext();
            user = context.UserProfiles.SingleOrDefault(u => u.UserName == User.Identity.Name);
            //strSQL = QueryDefinitions.GetQuery("SelectPurchaseOrdersByWarehousesAndStatus", new string[] { user.Warehouses.AddSingleQuotes(), "O" });//O is for Ordered, C is for Complete, etc.
            strSQL = QueryDefinitions.GetQuery("SelectPOByLineWarehousesAndStatus", new string[] { user.Warehouses.AddSingleQuotes(), "O" });//This will only bring in Orders where there are corresponding Open Order Lines.

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
            UsersContext context;
            UserProfile user;
            string strSQL;


            context = new UsersContext();
            user = context.UserProfiles.SingleOrDefault(u => u.UserName == User.Identity.Name);
            strSQL = QueryDefinitions.GetQuery("SelectPOLinesByWarehousesAndStatusAndOrderNo", new string[] { user.Warehouses.AddSingleQuotes(), "O", OrderNo });//O is for Ordered, C is for Complete, etc.
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

        [HttpGet]
        public ActionResult NotesViewer()
        {
            return View("NotesViewer");//located in Views/Shared/NotesViewer.cshtml
        }

        [HttpPost]
        public JsonResult NotesViewer(JQueryDataTablesModel jQueryDataTablesModel, string OrderNo, string LineNo, string ReleaseNo)
        {
            int totalRecordCount;
            int searchRecordCount;
            short shTemp;
            List<SytelineNote> objNotes;
            Notes objOrderNotes, objLineNotes, objLineReleaseNotes;


            objNotes = new List<SytelineNote>();
            objOrderNotes = new Notes(OrderNo, NoteType.PO);
            objLineNotes = new Notes(
                OrderNo,
                short.TryParse(LineNo, out shTemp) ? shTemp : (short)0,
                NoteType.POLine);
            objLineReleaseNotes = new Notes(
                OrderNo,
                short.TryParse(LineNo, out shTemp) ? shTemp : (short)0,
                short.TryParse(ReleaseNo, out shTemp) ? shTemp : (short)0,
                NoteType.POLineRelease);
            objNotes.AddRange(objOrderNotes);
            objNotes.AddRange(objLineNotes);
            objNotes.AddRange(objLineReleaseNotes);

            InMemoryNotesRepository.AllNotes = objNotes
                .Where(n => n.IsInternal == 0)
                .ToList();

            var objItems = InMemoryNotesRepository.GetNotes(startIndex: jQueryDataTablesModel.iDisplayStart,
                pageSize: jQueryDataTablesModel.iDisplayLength, sortedColumns: jQueryDataTablesModel.GetSortedColumns(),
                totalRecordCount: out totalRecordCount, searchRecordCount: out searchRecordCount, searchString: jQueryDataTablesModel.sSearch);

            return this.DataTablesJson(items: objItems,
                totalRecords: totalRecordCount,
                totalDisplayRecords: searchRecordCount,
                sEcho: jQueryDataTablesModel.sEcho);
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