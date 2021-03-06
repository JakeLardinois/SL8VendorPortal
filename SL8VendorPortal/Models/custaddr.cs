//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SL8VendorPortal.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class custaddr
    {
        public string cust_num { get; set; }
        public int cust_seq { get; set; }
        public string name { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string zip { get; set; }
        public string county { get; set; }
        public string country { get; set; }
        public string fax_num { get; set; }
        public string telex_num { get; set; }
        public string bal_method { get; set; }
        public string addr__1 { get; set; }
        public string addr__2 { get; set; }
        public string addr__3 { get; set; }
        public string addr__4 { get; set; }
        public Nullable<byte> credit_hold { get; set; }
        public string credit_hold_user { get; set; }
        public Nullable<System.DateTime> credit_hold_date { get; set; }
        public string credit_hold_reason { get; set; }
        public Nullable<decimal> credit_limit { get; set; }
        public string curr_code { get; set; }
        public string corp_cust { get; set; }
        public Nullable<byte> corp_cred { get; set; }
        public Nullable<byte> corp_address { get; set; }
        public Nullable<decimal> amt_over_inv_amt { get; set; }
        public Nullable<short> days_over_inv_due_date { get; set; }
        public string ship_to_email { get; set; }
        public string bill_to_email { get; set; }
        public string internet_url { get; set; }
        public string internal_email_addr { get; set; }
        public string external_email_addr { get; set; }
        public byte NoteExistsFlag { get; set; }
        public System.DateTime RecordDate { get; set; }
        public System.Guid RowPointer { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public System.DateTime CreateDate { get; set; }
        public byte InWorkflow { get; set; }
        public string uf_pref_type { get; set; }
        public string carrier_account { get; set; }
        public Nullable<decimal> carrier_upcharge_pct { get; set; }
        public byte carrier_residential_indicator { get; set; }
        public string carrier_bill_to_transportation { get; set; }
    }
}
