using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EC.Models.API.v1.GlobalSettings
{
    public class GlobalSettingsModel
    {
        [Required]
        [StringLength(255)]
        public string customLogoPath { get; set; }
        [Required]
        [StringLength(255)]
        public string headerLinksColorCode { get; set; }
        [Required]
        [StringLength(255)]
        public string headerColorCode { get; set; }
    }
}