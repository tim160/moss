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
    
    public partial class country
    {
        public country()
        {
            this.addresses = new HashSet<address>();
        }
    
        public int id { get; set; }
        public string country_nm { get; set; }
        public string country_cl { get; set; }
        public string country_description { get; set; }
        public string country_description_en { get; set; }
        public string country_description_ru { get; set; }
    
        public virtual ICollection<address> addresses { get; set; }
        public virtual status status { get; set; }
    }
}
