using System;
using EC.Core.Common;
using EC.Common.Interfaces;

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