//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ServicesExternalEmail.DBModel
{
    using System;
    using System.Collections.Generic;
    
    public partial class billing_option
    {
        public int id { get; set; }
        public string billing_option_en { get; set; }
        public string billing_option_fr { get; set; }
        public string billing_option_es { get; set; }
        public string billing_option_ru { get; set; }
        public string billing_option_ar { get; set; }
        public System.DateTime last_update_dt { get; set; }
        public int user_id { get; set; }
        public string code { get; set; }
    }
}
