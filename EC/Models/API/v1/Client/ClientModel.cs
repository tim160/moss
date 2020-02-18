using EC.Models.API.v1.GlobalSettings;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EC.Models.API.v1.Client
{
    public class ClientModel
    {
        public int id { get; set; }
        [StringLength(100)]
        [Required]
        public string clientName { get; set; }
        [DisplayName("partnerClientId")]
        public string partnerClientId { get; set; }
        public GlobalSettingsModel globalSettings { get; set; }
    }
}