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
    
    public partial class task_comment
    {
        public int id { get; set; }
        public int task_id { get; set; }
        public int user_id { get; set; }
        public string title { get; set; }
        public string body { get; set; }
        public System.DateTime created_date { get; set; }
    }
}
