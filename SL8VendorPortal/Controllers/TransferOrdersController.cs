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
    public class TransferOrdersController : Controller
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
            //strSQL = QueryDefinitions.GetQuery("SelectTransferOrdersByToWarehousesAndStatus", new string[] { user.Warehouses.AddSingleQuotes(), "O, T".AddSingleQuotes() });//O is for Ordered, T is for Transit, etc.
            strSQL = QueryDefinitions.GetQuery("SelectTOByLineToFromWarehousesAndStatuses", new string[] { user.Warehouses.AddSingleQuotes(), "O, T".AddSingleQuotes() });//O is for Ordered, T is for Transit, etc.

            InMemoryTransferOrdersRepository.AllTransferOrders = db.transfers.SqlQuery(strSQL).ToList();


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
            UsersContext context;
            UserProfile user;
            string strSQL;


            context = new UsersContext();
            user = context.UserProfiles.SingleOrDefault(u => u.UserName == User.Identity.Name);
            strSQL = QueryDefinitions.GetQuery("SelectTOLinesByToFromWhsesAndStatusAndOrderNo", new string[] { user.Warehouses.AddSingleQuotes(), "O, T".AddSingleQuotes(), OrderNo });//O is for Ordered, T is for Transit, etc.

            InMemoryTransferOrderLinesRepository.AllTransferOrderLines = db.trnitems.SqlQuery(strSQL).ToList();

            var objItems = InMemoryTransferOrderLinesRepository.GetTransferOrderLines(startIndex: jQueryDataTablesModel.iDisplayStart,
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
        public JsonResult NotesViewer(JQueryDataTablesModel jQueryDataTablesModel, string OrderNo, string LineNo)
        {
            int totalRecordCount;
            int searchRecordCount;
            short shTemp;
            List<SytelineNote> objNotes;
            Notes objOrderNotes, objLineNotes;


            objNotes = new List<SytelineNote>();
            objOrderNotes = new Notes(OrderNo, NoteType.TO);
            objLineNotes = new Notes(
                OrderNo,
                short.TryParse(LineNo, out shTemp) ? shTemp : (short)0,
                NoteType.TOLine);
            objNotes.AddRange(objOrderNotes);
            objNotes.AddRange(objLineNotes);

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