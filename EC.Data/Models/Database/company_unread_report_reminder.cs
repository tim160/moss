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
    
    public partial class company_unread_report_reminder
    {
        public int id { get; set; }
        public int company_id { get; set; }
        public int unread_report_reminder_id { get; set; }
        public int user_id { get; set; }
        public System.DateTime created_date { get; set; }
    }
}
