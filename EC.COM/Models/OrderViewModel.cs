using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EC.COM.Data;

namespace EC.COM.Models
{
    public class OrderViewModel
    {
        public VarInfoModel VarInfo { get; set; }
        public string CardNo { get; set; }
        public string NameOnCard { get; set; }
        public int ExpirationMonth { get; set; }
        public int ExpirationYear { get; set; }
        public string CSVCode { get; set; }
        public string QuickView { get; set; }
        public string StripeToken { get; set; }
    }
}