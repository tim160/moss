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
    
    public partial class report_mediator_involved
    {
        public int id { get; set; }
        public int report_id { get; set; }
        public int mediator_id { get; set; }
        public int status_id { get; set; }
        public string notepad_tx { get; set; }
        public System.DateTime last_update_dt { get; set; }
        public int user_id { get; set; }
    }
}
