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
    
    public partial class time_zone
    {
        public int id { get; set; }
        public string time_zone_name { get; set; }
        public Nullable<double> time_zone_value { get; set; }
        public string abbreviation { get; set; }
    }
}
