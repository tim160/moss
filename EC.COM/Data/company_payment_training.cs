namespace EC.COM.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("company_payment_training")]
    public partial class company_payment_training
  {
    public int id { get; set; }
    public int company_id { get; set; }
 
    public decimal amount { get; set; }
    public string auth_code { get; set; }
    public string local_invoice_number { get; set; }
    public int user_id { get; set; }

    public DateTime payment_date { get; set; }

    public string payment_code { get; set; }

    public string training_code { get; set; }

    public int onboard_sessions_paid { get; set; }

    public DateTime onboard_sessions_expiry_dt { get; set; }
    public string stripe_receipt_url { get; set; }

    public string stripe_receipt_email { get; set; }

  }
}
