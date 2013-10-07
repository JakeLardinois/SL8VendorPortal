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
    public class PurchaseOrderLinesController : Controller
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
            strSQL = QueryDefinitions.GetQuery("SelectPOLinesByWarehousesAndStatus", new string[] { user.Warehouses.AddSingleQuotes(), "O" });//O is for Ordered, C is for Complete, etc.

            InMemoryPurchaseOrderLinesRepository.AllPurchaseOrderLines = db.poitems.SqlQuery(strSQL).ToList();


            var objItems = InMemoryPurchaseOrderLinesRepository.GetPurchaseOrderLines(startIndex: jQueryDataTablesModel.iDisplayStart,
                pageSize: jQueryDataTablesModel.iDisplayLength, sortedColumns: jQueryDataTablesModel.GetSortedColumns(),
                totalRecordCount: out totalRecordCount, searchRecordCount: out searchRecordCount, searchString: jQueryDataTablesModel.sSearch);

            return this.DataTablesJson(items: objItems,
                totalRecords: totalRecordCount,
                totalDisplayRecords: searchRecordCount,
                sEcho: jQueryDataTablesModel.sEcho);
        }

        #region Unused Code
        //
        // GET: /PurchaseOrderLines/

        //public ActionResult Index()
        //{
        //    var context = new UsersContext();
        //    var user = context.UserProfiles.SingleOrDefault(u => u.UserName == User.Identity.Name);
        //    var strSQL = QueryDefinitions.GetQuery("SelectPOLinesByWarehousesAndStatus", new string[] { user.Warehouses.AddSingleQuotes(), "O" });//O is for Ordered, C is for Complete, etc.


        //    return View(db.poitems.SqlQuery(strSQL).ToList());
        //}

        ////
        //// GET: /PurchaseOrderLines/Details/5

        //public ActionResult Details(string id = null)
        //{
        //    poitem poitem = db.poitems.Find(id);
        //    if (poitem == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(poitem);
        //}

        ////
        //// GET: /PurchaseOrderLines/Create

        //public ActionResult Create()
        //{
        //    return View();
        //}

        ////
        //// POST: /PurchaseOrderLines/Create

        //[HttpPost]
        //public ActionResult Create(poitem poitem)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.poitems.Add(poitem);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    return View(poitem);
        //}

        ////
        //// GET: /PurchaseOrderLines/Edit/5

        //public ActionResult Edit(string id = null)
        //{
        //    poitem poitem = db.poitems.Find(id);
        //    if (poitem == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(poitem);
        //}

        ////
        //// POST: /PurchaseOrderLines/Edit/5

        //[HttpPost]
        //public ActionResult Edit(poitem poitem)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(poitem).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    return View(poitem);
        //}

        ////
        //// GET: /PurchaseOrderLines/Delete/5

        //public ActionResult Delete(string id = null)
        //{
        //    poitem poitem = db.poitems.Find(id);
        //    if (poitem == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(poitem);
        //}

        ////
        //// POST: /PurchaseOrderLines/Delete/5

        //[HttpPost, ActionName("Delete")]
        //public ActionResult DeleteConfirmed(string id)
        //{
        //    poitem poitem = db.poitems.Find(id);
        //    db.poitems.Remove(poitem);
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