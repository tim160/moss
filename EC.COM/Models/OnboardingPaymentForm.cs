using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EC.COM.Models
{
    public class OnboardingPaymentForm
    {
        public string NameOnCard { get; set; }
        public int CardNumber { get; set; }
        public DateTime CardExpire { get; set; }
        public int CVC { get; set; }
    }
}