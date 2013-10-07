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
    public class CustomerOrdersController : Controller
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
            //strSQL = QueryDefinitions.GetQuery("SelectCustomerOrdersByWarehousesAndStatus", new string[] { user.Warehouses.AddSingleQuotes(), "O" });//O is for Ordered, C is for Complete, etc.
            strSQL = QueryDefinitions.GetQuery("SelectCOByLineWarehousesAndStatus", new string[] { user.Warehouses.AddSingleQuotes(), "O" });//This will only bring in Orders where there are corresponding Open Order Lines.

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
            UsersContext context;
            UserProfile user;
            string strSQL;


            context = new UsersContext();
            user = context.UserProfiles.SingleOrDefault(u => u.UserName == User.Identity.Name);
            strSQL = QueryDefinitions.GetQuery("SelectCOLinesByWarehousesAndStatusAndOrderNo", new string[] { user.Warehouses.AddSingleQuotes(), "O", OrderNo });//O is for Ordered, C is for Complete, etc.
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
            objOrderNotes = new Notes(OrderNo, NoteType.CO);
            objLineNotes = new Notes(
                OrderNo,
                short.TryParse(LineNo, out shTemp) ? shTemp : (short)0,
                NoteType.COLine);
            objLineReleaseNotes = new Notes(
                OrderNo, 
                short.TryParse(LineNo, out shTemp) ? shTemp : (short)0, 
                short.TryParse(ReleaseNo, out shTemp) ? shTemp : (short)0, 
                NoteType.COLineRelease);
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