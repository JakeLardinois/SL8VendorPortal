using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


/*I needed to add these here in order to add them as a datasource in my .rdlc files.  Note that I also needed to add a WebForm (in this case WebForm1.aspx) in order
 * to "see" these options upon adding a DataSet without utilizing a SQL Connection...
 */
namespace SL8VendorPortal.Models
{
    public class CustomerOrders : List<co> { }
    public class CustomerOrderLines : List<coitem> { }

    public class PurchaseOrders : List<po> { }
    public class PurchaseOrderLines : List<poitem> { }

    public class TransferOrders : List<transfer> { }
    public class TransferOrderLines : List<trnitem> { }

    public class ItemWhses : List<itemwhse> { }

    public class VendorRequests : List<VendorRequest> { }
}