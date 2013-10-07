using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Data.SqlClient;
using System.ComponentModel.DataAnnotations;


namespace SL8VendorPortal.Models
{
    public class SytelineNote : BaseNote
    {
        public virtual Guid RowPointer { get; set; }
        public virtual decimal ObjectNoteToken { get; set; }
        public virtual decimal SpecificNoteToken { get; set; }
        //public virtual DateTime LastUpdated { get; set; }
        [Display(Name = "Description")]
        public virtual string NoteDesc { get; set; }
        //public virtual string NoteContent { get; set; }
        [Display(Name = "Created By")]
        public virtual string CreatedBy { get; set; }
        [Display(Name = "Updated By")]
        public virtual string UpdatedBy { get; set; }
        public virtual decimal NoteHeaderToken { get; set; }
        public virtual byte IsInternal { get; set; }
        public virtual string HTMLNoteContent { 
            get 
            {
                if (!string.IsNullOrEmpty(NoteContent))
                    return NoteContent.Replace(Environment.NewLine, "<br />").Replace("\n", "<br />");
                else
                    return string.Empty;
            } 
        }
        public bool Save(string strUserName)
        {
            DateTime dtmLastUpdate;

            using (var db = new SytelineDbEntities())
            {
                dtmLastUpdate = db.Database.SqlQuery<DateTime>(
                            QueryDefinitions.GetQuery("SelectNoteTimeStamp", new string[] { SpecificNoteToken.ToString() }))
                            .DefaultIfEmpty(SharedVariables.MAXDATE)
                            .FirstOrDefault();

                if (DateTime.Compare(dtmLastUpdate, LastUpdated) <= 0)
                    if (db.Database.ExecuteSqlCommand(QueryDefinitions.GetQuery("UpdateNote", new string[] { SpecificNoteToken.ToString(), NoteContent.Replace("'", "''"), NoteDesc.Replace("'", "''"), strUserName })) == 1)
                        return true;
            }
            return false;
        }
    }

    public class Note : BaseNote
    {
        [Key]
        public virtual int NoteID { get; set; }
    }

    public abstract class BaseNote
    {
        [Display(Name = "Last Updated")]
        public virtual DateTime LastUpdated { get; set; }
        public virtual string NoteContent { get; set; }
    }

    public class Notes : List<SytelineNote>
    {
        public NoteType NoteType { get; set; }
        public string OrderNo { get; set; }
        public short Line { get; set; }
        public short Release { get; set; }


        //This should work for PO's and CO's
        public Notes(string strOrderNo, NoteType objNoteType)
            : base()
        {
            NoteType = objNoteType;
            OrderNo = strOrderNo; ;
            LoadData();
        }

        //This should work for POLines and COLines
        public Notes(string strOrderNo, short shOrderLine, NoteType objNoteType)
            : base()
        {
            NoteType = objNoteType;
            OrderNo = strOrderNo;
            Line = shOrderLine;
            LoadData();
        }

        //This should work for POLineReleases and COLineReleases
        public Notes(string strOrderNo, short shOrderLine, short shOrderRelease, NoteType objNoteType)
            : base()
        {
            NoteType = objNoteType;
            OrderNo = strOrderNo;
            Line = shOrderLine;
            Release = shOrderRelease;
            LoadData();
        }

        private void LoadData()
        {
            this.Clear();
            using (var db = new SytelineDbEntities())
            {
                switch (NoteType)
                {
                    case NoteType.CO:
                        this.AddRange(db.Database.SqlQuery<SytelineNote>(
                            QueryDefinitions.GetQuery("SelectCONotes", new string[] { OrderNo }))
                            .ToList());
                        break;
                    case NoteType.COLine:
                        this.AddRange(db.Database.SqlQuery<SytelineNote>(
                            QueryDefinitions.GetQuery("SelectCOLineNotes", new string[] { OrderNo, Line.ToString() }))
                            .ToList());
                        break;
                    case NoteType.COLineRelease:
                        this.AddRange(db.Database.SqlQuery<SytelineNote>(
                            QueryDefinitions.GetQuery("SelectCOLineReleaseNotes", new string[] { OrderNo, Line.ToString(), Release.ToString() }))
                            .ToList());
                        break;
                    case NoteType.PO:
                        this.AddRange(db.Database.SqlQuery<SytelineNote>(
                            QueryDefinitions.GetQuery("SelectPONotes", new string[] { OrderNo }))
                            .ToList());
                        break;
                    case NoteType.POLine:
                        this.AddRange(db.Database.SqlQuery<SytelineNote>(
                            QueryDefinitions.GetQuery("SelectPOLineNotes", new string[] { OrderNo, Line.ToString() }))
                            .ToList());
                        break;
                    case NoteType.POLineRelease:
                        this.AddRange(db.Database.SqlQuery<SytelineNote>(
                            QueryDefinitions.GetQuery("SelectPOLineReleaseNotes", new string[] { OrderNo, Line.ToString(), Release.ToString() }))
                            .ToList());
                        break;
                    case NoteType.TO:
                        this.AddRange(db.Database.SqlQuery<SytelineNote>(
                            QueryDefinitions.GetQuery("SelectTONotes", new string[] { OrderNo }))
                            .ToList());
                        break;
                    case NoteType.TOLine:
                        this.AddRange(db.Database.SqlQuery<SytelineNote>(
                            QueryDefinitions.GetQuery("SelectTOLineNotes", new string[] { OrderNo, Line.ToString() }))
                            .ToList());
                        break;
                }
            }
        }
    }
}