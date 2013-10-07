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
    public class CustomerOrderLinesController : Controller
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
            strSQL = QueryDefinitions.GetQuery("SelectCOLinesByWarehousesAndStatus", new string[] { user.Warehouses.AddSingleQuotes(), "O" });//O is for Ordered, C is for Complete, etc.

            InMemoryCustomerOrderLinesRepository.AllCustomerOrderLines = db.coitems.SqlQuery(strSQL).ToList();


            var objItems = InMemoryCustomerOrderLinesRepository.GetCustomerOrderLines(startIndex: jQueryDataTablesModel.iDisplayStart,
                pageSize: jQueryDataTablesModel.iDisplayLength, sortedColumns: jQueryDataTablesModel.GetSortedColumns(),
                totalRecordCount: out totalRecordCount, searchRecordCount: out searchRecordCount, searchString: jQueryDataTablesModel.sSearch);

            return this.DataTablesJson(items: objItems,
                totalRecords: totalRecordCount,
                totalDisplayRecords: searchRecordCount,
                sEcho: jQueryDataTablesModel.sEcho);
        }


        #region Unused Code
        ////GET: /CustomerOrderLines/

        //public ActionResult Index()
        //{
        //    var context = new UsersContext();
        //    var user = context.UserProfiles.SingleOrDefault(u => u.UserName == User.Identity.Name);
        //    var strSQL = QueryDefinitions.GetQuery("SelectCOLinesByWarehousesAndStatus", new string[] { user.Warehouses.AddSingleQuotes(), "O" });//O is for Ordered, C is for Complete, etc.


        //    return View(db.coitems.SqlQuery(strSQL).ToList());
        //}

        ////
        //// GET: /CustomerOrderLines/Details/5

        //public ActionResult Details(string id = null)
        //{
        //    coitem coitem = db.coitems.Find(id);
        //    if (coitem == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(coitem);
        //}

        ////
        //// GET: /CustomerOrderLines/Create

        //public ActionResult Create()
        //{
        //    return View();
        //}

        ////
        //// POST: /CustomerOrderLines/Create

        //[HttpPost]
        //public ActionResult Create(coitem coitem)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.coitems.Add(coitem);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    return View(coitem);
        //}

        ////
        //// GET: /CustomerOrderLines/Edit/5

        //public ActionResult Edit(string id = null)
        //{
        //    coitem coitem = db.coitems.Find(id);
        //    if (coitem == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(coitem);
        //}

        ////
        //// POST: /CustomerOrderLines/Edit/5

        //[HttpPost]
        //public ActionResult Edit(coitem coitem)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(coitem).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    return View(coitem);
        //}

        ////
        //// GET: /CustomerOrderLines/Delete/5

        //public ActionResult Delete(string id = null)
        //{
        //    coitem coitem = db.coitems.Find(id);
        //    if (coitem == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(coitem);
        //}

        ////
        //// POST: /CustomerOrderLines/Delete/5

        //[HttpPost, ActionName("Delete")]
        //public ActionResult DeleteConfirmed(string id)
        //{
        //    coitem coitem = db.coitems.Find(id);
        //    db.coitems.Remove(coitem);
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