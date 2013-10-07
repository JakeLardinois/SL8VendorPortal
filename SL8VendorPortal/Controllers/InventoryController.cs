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
    public class InventoryController : Controller
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
            strSQL = QueryDefinitions.GetQuery("SelectItemWhseByWhsesAndPMTCode", new string[] { user.Warehouses.AddSingleQuotes(), "M" });//M is for Manufactured, P is for Purchased

            InMemoryItemWhsesRepository.AllItemWhses = db.itemwhses.SqlQuery(strSQL).ToList();


            var objItems = InMemoryItemWhsesRepository.GetItemWhses(startIndex: jQueryDataTablesModel.iDisplayStart,
                pageSize: jQueryDataTablesModel.iDisplayLength, sortedColumns: jQueryDataTablesModel.GetSortedColumns(),
                totalRecordCount: out totalRecordCount, searchRecordCount: out searchRecordCount, searchString: jQueryDataTablesModel.sSearch);

            return this.DataTablesJson(items: objItems,
                totalRecords: totalRecordCount,
                totalDisplayRecords: searchRecordCount,
                sEcho: jQueryDataTablesModel.sEcho);
        }

        #region Unused Code
        ////
        //// GET: /Inventory/

        //public ActionResult Index()
        //{
        //    var context = new UsersContext();
        //    var user = context.UserProfiles.SingleOrDefault(u => u.UserName == User.Identity.Name);
        //    var strSQL = QueryDefinitions.GetQuery("SelectItemWhseByWhsesAndPMTCode", new string[] { user.Warehouses.AddSingleQuotes(), "M" });//M is for Manufactured, P is for Purchased


        //    return View(db.itemwhses.SqlQuery(strSQL).ToList());
        //}

        ////
        //// GET: /Inventory/Details/5

        //public ActionResult Details(string id = null)
        //{
        //    itemwhse itemwhse = db.itemwhses.Find(id);
        //    if (itemwhse == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(itemwhse);
        //}

        ////
        //// GET: /Inventory/Create
        //public ActionResult Create()
        //{
        //    return View();
        //}

        ////
        //// POST: /Inventory/Create

        //[HttpPost]
        //public ActionResult Create(itemwhse itemwhse)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.itemwhses.Add(itemwhse);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    return View(itemwhse);
        //}

        ////
        //// GET: /Inventory/Edit/5

        //public ActionResult Edit(string id = null)
        //{
        //    itemwhse itemwhse = db.itemwhses.Find(id);
        //    if (itemwhse == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(itemwhse);
        //}

        ////
        //// POST: /Inventory/Edit/5

        //[HttpPost]
        //public ActionResult Edit(itemwhse itemwhse)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(itemwhse).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    return View(itemwhse);
        //}

        ////
        //// GET: /Inventory/Delete/5

        //public ActionResult Delete(string id = null)
        //{
        //    itemwhse itemwhse = db.itemwhses.Find(id);
        //    if (itemwhse == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(itemwhse);
        //}

        ////
        //// POST: /Inventory/Delete/5

        //[HttpPost, ActionName("Delete")]
        //public ActionResult DeleteConfirmed(string id)
        //{
        //    itemwhse itemwhse = db.itemwhses.Find(id);
        //    db.itemwhses.Remove(itemwhse);
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