using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using jQuery.DataTables.Mvc;
using SL8VendorPortal.Models;
using SL8VendorPortal.Infrastructure;


namespace SL8VendorPortal.Controllers
{
    [Authorize(Roles = "Admin,User")]
    public class NotesController : Controller
    {

        [HttpGet]
        public ActionResult CONotesViewer()
        {
            return View("NotesViewer");//located in Views/Shared/NotesViewer.cshtml
        }

        [HttpPost]
        public JsonResult CONotesViewer(JQueryDataTablesModel jQueryDataTablesModel, string OrderNo, string LineNo, string ReleaseNo)
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

        [HttpGet]
        public ActionResult PONotesViewer()
        {
            return View("NotesViewer");//located in Views/Shared/NotesViewer.cshtml
        }

        [HttpPost]
        public JsonResult PONotesViewer(JQueryDataTablesModel jQueryDataTablesModel, string OrderNo, string LineNo, string ReleaseNo)
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

        [HttpGet]
        public ActionResult TONotesViewer()
        {
            return View("NotesViewer");//located in Views/Shared/NotesViewer.cshtml
        }

        [HttpPost]
        public JsonResult TONotesViewer(JQueryDataTablesModel jQueryDataTablesModel, string OrderNo, string LineNo)
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

        [HttpPost]
        public JsonResult UpdateNote(string specificnotetoken, string notedesc, string notecontent, string lastupdated)
        {
            decimal decTemp;
            DateTime dtmLastUpdate;


            /*When I */
            SytelineNote objNote = new SytelineNote
            {
                SpecificNoteToken = decimal.TryParse(specificnotetoken, out decTemp) ? decTemp : 0,
                NoteDesc = notedesc,
                NoteContent = notecontent,
                LastUpdated = lastupdated.GetDateTimeFromJSON() //my extension method that converts the JSON time format of /Date(1376625062603)/ to a DateTime object
            };

            // When I changed my datatable format to occur on fnRender for the cell instead of fnRowCallback for the row, my date started getting sent to the controller in the format 1/1/1900 instead of /Date(1376625062603)/
            //This caused problems because not only did my extension method fail to convert the date (due to the standard format), but the new format also didn't contain the time element like the JSON format, so my updates
            //to the notes would always fail. The solution was to reimplement the dateformat on the fnRowCallback for the LastUpdated field on the datatable. One of the main reasons that I had moved away from formatting the 
            //date on fnRowCallback was because when I implemented Column Resizing and Reordering, the date would loose it's formatting as soon as it was moved; however the column reordering and resizing doesn't work for the 
            //child tables and so it is a non issue to reinstate the old formatting method.
            if (objNote.Save(User.Identity.Name))//The save 
            {
                using (var db = new SytelineDbEntities())
                {
                    dtmLastUpdate = db.Database.SqlQuery<DateTime>(
                            QueryDefinitions.GetQuery("SelectNoteTimeStamp", new string[] { specificnotetoken.ToString() }))
                            .DefaultIfEmpty(SharedVariables.MAXDATE)
                            .FirstOrDefault();
                }
                var dtmNow = DateTime.Now;
                //var dood = dtmLastUpdate.GetJSONFromDateTime();
                //var blah = "dood";
                return Json(new { Success = true, LastUpdate = dtmLastUpdate, objNote.HTMLNoteContent });//Note that this Json constructor will creates a JSON object that will convert the  
            }                                                                                               //DateTime value to the JSON format of /Date(1376625062603)/ The subsequent values can be accessed in JavaScript
            else                                                                                            //by using JSON.parse(value) see an example in SearchTransferOrders.js around line 207
                return Json(new { Success = false, objNote.HTMLNoteContent });
        }
    }
}
