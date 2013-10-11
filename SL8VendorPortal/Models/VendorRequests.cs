using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace SL8VendorPortal.Models
{
    //I did have this class separated into 3 classes all deriving from the VendorRequest Abstract class, but I flattened it so that my implementation was easier on the VendorRequest Controller...
    public class VendorRequest
    {
        public VendorRequest():base()
        {
            DateRequested = DateTime.Now;
            DateUpdated = DateTime.Now;
            DateProcessed = SharedVariables.MINDATE;
            Notes = string.Empty;//If the notes aren't initialized to an empty (as opposed to null) value, then when adding a request via the VendorRequestController and the frmAddVendorRequest form causes an error whereby the user
                                    //isn't able to modify the notes in the popup and see the edited changes immediately; most likely due to some error that is thrown while saving a null value for notes...
            Item = " ";
        }

        [Key]
        public virtual int ID { get; set; }

        public string Item { get; set; }

        public virtual bool Processed { get; set; }
        public virtual DateTime DateProcessed { get; set; }

        public virtual string Notes { get; set; }
        [NotMapped]
        public virtual string HTMLNotes
        {
            get
            {
                if (!string.IsNullOrEmpty(Notes))
                    return Notes
                        .Replace(Environment.NewLine, "<br />")
                        .Replace("\n", "<br />");
                else
                    return string.Empty;
            }
        }
        
        public virtual DateTime DateRequested { get; set; } //This is the create date...
        public virtual DateTime DateUpdated { get; set; }

        //[Display(Name = "Source")]
        //[Required(ErrorMessage = "Source Warehouse is required.")]
        public virtual string SourceWarehouse { get; set; }

        //[Display(Name = "Destination")]
        //[Required(ErrorMessage = "Destination Warehouse is required.")]
        public virtual string DestWarehouse { get; set; }//for vendor to request inventory get transferred back to MAIN warehouse

        public virtual string OrderNo { get; set; }
        public virtual short LineNo { get; set; }
        public virtual short ReleaseNo { get; set; }

        //set as one of the VendorRequestCategory enumeration types defined in Models/MyEnums
        //CORequest = 0, PORequest = 1, TORequest = 2 
        //public int RequestCategory { get; set; }
        public virtual int RequestCategoryID { get; set; } 
        //TORequestTypes - TOReciept, TOShipment; CORequestTypes - COShipment, COLateRequest; PORequestTypes: POReciept, POLateRequest
        //public virtual string RequestType { get; set; }// I did have this split out as a separate object on each of the VendorRequest types; However, this caused my datatable to not function 
        public virtual string RequestCategoryCode { get; set; }  //It took me a long time to figure out too since I had no error message from datatables...
        

        public virtual int Qty { get; set; }

        //[Display(Name = "Qty Loss")]
        public virtual int QtyLoss { get; set; }
        public virtual bool Approved { get; set; }//refusing or accepting a vendor request...

        public virtual string Creator { get; set; }
        public virtual string Updater { get; set; }
    }
}