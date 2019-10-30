using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EC.Models.Database;

using EC.Models.App;
using EC.Core.Common;
using EC.Common.Interfaces;
using EC.Constants;
using log4net;


namespace EC.Models.ViewModels
{
  public class AllPaymentTypesViewModel
  {
    protected IDateTimeHelper m_DateTimeHelper = new DateTimeHelper();

    public string auth_code { get; set; }
    public DateTime payment_date { get; set; }
    public decimal amount { get; set; }
    public string local_invoice_number { get; set; }
    public string description { get; set; }
    public string receipt_url { get; set; }


  }
}