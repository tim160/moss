//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace EC.Data.Models.Database
{
    using System;
    using System.Collections.Generic;
    
    public partial class task
    {
        public int id { get; set; }
        public int report_id { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public int assigned_to { get; set; }
        public bool is_completed { get; set; }
        public string completed_message { get; set; }
        public Nullable<int> completed_by { get; set; }
        public Nullable<System.DateTime> due_date { get; set; }
        public Nullable<System.DateTime> completed_on { get; set; }
        public System.DateTime created_on { get; set; }
        public int created_by { get; set; }
    }
}