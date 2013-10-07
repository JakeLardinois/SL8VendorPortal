using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace SL8VendorPortal.Models
{
    public class RequestCategory
    {
        //CORequest = 0, PORequest = 1, TORequest = 2 
        [Key, Column(Order = 1)]
        public int ID { get; set; }

        //TORequestTypes - TOReciept, TOShipment; CORequestTypes - COShipment, COLateRequest; PORequestTypes: POReciept, POLateRequest
        [Key, Column(Order = 2)]
        public string Code { get; set; }

        public string Description { get; set; }
    }
}