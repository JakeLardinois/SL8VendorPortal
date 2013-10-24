using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SL8VendorPortal.Models
{
    public class VendorRequestSearch
    {
        public string ID { get; set; }
        public string Processed { get; set; }
        public string Item { get; set; }

        public DateTime DateProcessedGT { get; set; }
        public DateTime DateProcessedLT { get; set; }

        public string Notes { get; set; }

        public DateTime DateRequestedGT { get; set; }
        public DateTime DateRequestedLT { get; set; }

        public DateTime DateUpdatedGT { get; set; }
        public DateTime DateUpdatedLT { get; set; }

        public string[] SourceWarehouses { get; set; }
        public string[] DestWarehouses { get; set; }
        public string OrderNo { get; set; }
        public string LineNo { get; set; }
        public string ReleaseNo { get; set; }
        public string RequestCategoryID { get; set; } 
        public string [] RequestCategoryCodes { get; set; }
        public int Qty { get; set; }
        public int QtyLoss { get; set; }
        public string Approved { get; set; }



        //public int ID { get; set; }
        //public bool Processed { get; set; }

        //public DateTime DateProcessedGT { get; set; }
        //public DateTime DateProcessedLT { get; set; }

        //public string Notes { get; set; }

        //public DateTime DateRequestedGT { get; set; }
        //public DateTime DateRequestedLT { get; set; }

        //public DateTime DateUpdatedGT { get; set; }
        //public DateTime DateUpdatedLT { get; set; }

        //public string SourceWarehouse { get; set; }
        //public string DestWarehouse { get; set; }
        //public string OrderNo { get; set; }
        //public short LineNo { get; set; }
        //public short ReleaseNo { get; set; }
        //public int RequestCategoryID { get; set; }
        //public string RequestCategoryCode { get; set; }
        //public int Qty { get; set; }
        //public int QtyLoss { get; set; }
        //public bool Approved { get; set; }
    }
}