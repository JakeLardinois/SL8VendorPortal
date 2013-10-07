using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SL8VendorPortal.Models
{
    public class POLine
    {
        public string po_num { get; set; }
        public Int16 po_line { get; set; }
        public string vend_num { get; set; }
        public decimal qty_ordered { get; set; }
        public decimal qty_received { get; set; }
    }
}