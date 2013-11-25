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


namespace SL8VendorPortal.Controllers
{
    [Authorize(Roles = "Admin,User")]
    public class InventoryController : Controller
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


            using (SL8VendorPortalDb VendorPortalDb = new SL8VendorPortalDb())
            {
                ViewData["RequestCategoryCode"] = new SelectList(VendorPortalDb.RequestCategories.Where(r => r.ID == 3).ToList(),
                    "Code", "Description", "TransferOrderRequest");
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


            CurrentUserProfile = new UsersContext().UserProfiles.SingleOrDefault(u => u.UserName == User.Identity.Name);
            strSQL = QueryDefinitions.GetQuery("SelectItemWhseByWhsesAndPMTCode", new string[] { CurrentUserProfile.Warehouses.AddSingleQuotes(), "M" });//M is for Manufactured, P is for Purchased

            InMemoryItemWhsesRepository.AllItemWhses = db.itemwhses.SqlQuery(strSQL).ToList();


            var objItems = InMemoryItemWhsesRepository.GetItemWhses(startIndex: jQueryDataTablesModel.iDisplayStart,
                pageSize: jQueryDataTablesModel.iDisplayLength, sortedColumns: jQueryDataTablesModel.GetSortedColumns(),
                totalRecordCount: out totalRecordCount, searchRecordCount: out searchRecordCount, searchString: jQueryDataTablesModel.sSearch);

            return this.DataTablesJson(items: objItems,
                totalRecords: totalRecordCount,
                totalDisplayRecords: searchRecordCount,
                sEcho: jQueryDataTablesModel.sEcho);
        }

        public ActionResult GenerateInventoryReport(JQueryDataTablesModel jQueryDataTablesModel)
        {
            int totalRecordCount;
            int searchRecordCount;
            string strSQL;


            CurrentUserProfile = new UsersContext().UserProfiles.SingleOrDefault(u => u.UserName == User.Identity.Name);
            strSQL = QueryDefinitions.GetQuery("SelectItemWhseByWhsesAndPMTCode", new string[] { CurrentUserProfile.Warehouses.AddSingleQuotes(), "M" });//M is for Manufactured, P is for Purchased

            InMemoryItemWhsesRepository.AllItemWhses = db.itemwhses.SqlQuery(strSQL).ToList();

            var objItems = InMemoryItemWhsesRepository.GetItemWhses(startIndex: jQueryDataTablesModel.iDisplayStart,
                pageSize: jQueryDataTablesModel.iDisplayLength, sortedColumns: jQueryDataTablesModel.GetSortedColumns(),
                totalRecordCount: out totalRecordCount, searchRecordCount: out searchRecordCount, searchString: jQueryDataTablesModel.sSearch);

            RenderInventoryReport(objItems);

            return View();
        }

        private void RenderInventoryReport(IList<itemwhse> objItems)
        {
            string strReportType = "Excel";
            LocalReport objLocalReport;
            ReportDataSource ItemWhseDataSource;
            string mimeType;
            string encoding;
            string fileNameExtension;
            string deviceInfo = "";
            Warning[] warnings;
            string[] streams;


            objLocalReport = new LocalReport { ReportPath = Server.MapPath(Settings.ReportDirectory + "Inventory.rdlc") };

            //Give the reportdatasource a name so that we can reference it in our report designer
            ItemWhseDataSource = new ReportDataSource("ItemWhses", objItems);

            objLocalReport.DataSources.Add(ItemWhseDataSource);
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
            Response.AddHeader("content-disposition", "attachment; filename=Inventory" + DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + "." + fileNameExtension);
            Response.BinaryWrite(renderedBytes);
            Response.End();
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