using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SL8VendorPortal.Models
{
    /*The below lines can be used to populate the POLine class with data. The linker will automatically populate the properties of POLine with the values returned
     * from the query recordset.  I left this here as an example and this code is not used in the application.
     */
    //using (var db = new SytelineDbEntities())
    //{
    //    var POLines = db.Database.SqlQuery<POLine>(
    //        QueryDefinitions.GetQuery("SelectRegularPOLinesByVendor", new string[] { "504".PadLeft(7) }))
    //        .ToList();


    //    var temp = "Nothing";
    //}
    public class POLine
    {
        public string po_num { get; set; }
        public Int16 po_line { get; set; }
        public string vend_num { get; set; }
        public decimal qty_ordered { get; set; }
        public decimal qty_received { get; set; }
    }
}