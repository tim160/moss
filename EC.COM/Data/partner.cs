namespace EC.COM.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("partner")]
    public partial class partner
  {
        public int id { get; set; }
        [Required]
        public string partner_name { get; set; }
        [Required]
        public string partner_code { get; set; }
       // public int status_id { get; set; }

        [Required]
        [StringLength(250)]
        public string email { get; set; }

        [StringLength(100)]
        public string phone { get; set; }
        public string first_nm { get; set; }
        [StringLength(250)]
        public string last_nm { get; set; }
        public int is_active { get; set; }
    }
}
