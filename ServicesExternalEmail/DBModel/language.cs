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
    
    public partial class language
    {
        public int id { get; set; }
        public string language_code { get; set; }
        public string language_name_native { get; set; }
        public System.DateTime last_update_dt { get; set; }
        public int user_id { get; set; }
        public string culture { get; set; }
        public string language_name_english { get; set; }
    }
}
