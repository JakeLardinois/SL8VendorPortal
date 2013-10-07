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
    [Authorize(Roles = "Admin,User")]
    public class TransferOrdersController : Controller
    {
        private SytelineDbEntities db = new SytelineDbEntities();


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