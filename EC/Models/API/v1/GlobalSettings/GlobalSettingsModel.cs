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
        public string custom_logo_path { get; set; }
        [Required]
        [StringLength(255)]
        public string header_links_color_code { get; set; }
        [Required]
        [StringLength(255)]
        public string header_color_code { get; set; }
    }
}