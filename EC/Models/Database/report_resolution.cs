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
    
    public partial class report_resolution
    {
        public int id { get; set; }
        public int report_id { get; set; }
        public int resolution_id { get; set; }
        public int user_id { get; set; }
        public string description { get; set; }
        public System.DateTime created_on { get; set; }
        public Nullable<bool> is_approved { get; set; }
        public Nullable<int> approved_user_id { get; set; }
        public string approved_message { get; set; }
        public Nullable<System.DateTime> approved_date { get; set; }
    }
}
