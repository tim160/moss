//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace EC.Models.Database
{
    using System;
    using System.Collections.Generic;
    
    public partial class secondary_type_mandatory
    {
        public int id { get; set; }
        public int weight { get; set; }
        public int status_id { get; set; }
        public int type_id { get; set; }
        public string secondary_type_en { get; set; }
        public string secondary_type_fr { get; set; }
        public string secondary_type_es { get; set; }
        public string secondary_type_ru { get; set; }
        public string secondary_type_ar { get; set; }
        public string secondary_type_ja { get; set; }
        public System.DateTime last_update_dt { get; set; }
        public int user_id { get; set; }
    }
}
