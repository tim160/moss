//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace EC.Data.Models.ECDbModel
{
    using System;
    using System.Collections.Generic;
    
    public partial class attachment
    {
        public int id { get; set; }
        public Nullable<int> report_id { get; set; }
        public Nullable<int> report_message_id { get; set; }
        public Nullable<int> report_task_id { get; set; }
        public int status_id { get; set; }
        public string path_nm { get; set; }
        public string file_nm { get; set; }
        public string extension_nm { get; set; }
        public System.DateTime effective_dt { get; set; }
        public System.DateTime expiry_dt { get; set; }
        public System.DateTime last_update_dt { get; set; }
        public int user_id { get; set; }
        public Nullable<int> report_task_message_id { get; set; }
    }
}