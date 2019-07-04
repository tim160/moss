using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EC.COM.Models
{
    public class OnboardingPaymentForm
    {
        public int SessionNumber { get; set; }
        public long Amount { get; set; }
        public Guid CompanyGuid { get; set; }
        public string receiptUrl { get; set; }
  }
}