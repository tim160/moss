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
    
    public partial class user_role
    {
        public int id { get; set; }
        public string role_en { get; set; }
        public string role_fr { get; set; }
        public string role_es { get; set; }
        public string role_ru { get; set; }
        public string role_ar { get; set; }
        public string role_ds { get; set; }
        public System.DateTime last_update_dt { get; set; }
        public int created_by_user_id { get; set; }
        public string role_weight { get; set; }
        public string code { get; set; }
    }
}