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
    
    public partial class report_secondary_type
    {
        public int id { get; set; }
        public int report_id { get; set; }
        public Nullable<int> mandatory_secondary_type_id { get; set; }
        public int secondary_type_id { get; set; }
        public string secondary_type_nm { get; set; }
        public System.DateTime last_update_dt { get; set; }
        public int user_id { get; set; }
    }
}
