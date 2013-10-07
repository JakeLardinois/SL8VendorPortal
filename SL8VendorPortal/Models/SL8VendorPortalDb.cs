using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Data.Entity;


namespace SL8VendorPortal.Models
{
    public class SL8VendorPortalDb : DbContext
    {
        public DbSet<PendingTransferOrderTransaction> PendingTransferOrderTransactions { get; set; }
    }
}