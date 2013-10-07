using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.ComponentModel.DataAnnotations;


namespace SL8VendorPortal.Models
{
    public class PendingTransferOrderTransaction
    {
        [Key]
        public virtual int ID { get; set; }
        public virtual string Description { get; set; }
    }
}