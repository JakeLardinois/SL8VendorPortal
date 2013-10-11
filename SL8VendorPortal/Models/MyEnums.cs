using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SL8VendorPortal.Models
{
    public enum NoteType { PO, POLine, POLineRelease, CO, COLine, COLineRelease, TO, TOLine };

    public enum VendorRequestCategoryID { 
        CORequest = 0, 
        PORequest = 1, 
        TORequest = 2,
        ItemRequest = 3
    };
}