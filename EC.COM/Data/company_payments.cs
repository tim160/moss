namespace EC.COM.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

  [Table("company_payments")]
  public partial class company_payments
  {
    public int id { get; set; }
    public int company_id { get; set; }
    public string cc_name { get; set; }
    public string cc_number { get; set; }
    public int cc_month { get; set; }
    public int cc_year { get; set; }
    public int cc_csv { get; set; }
    public decimal amount { get; set; }
    public string auth_code { get; set; }
    public string local_invoice_number { get; set; }
    public int user_id { get; set; }

    public DateTime payment_date { get; set; }
  }
}
