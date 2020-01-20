
using System.ComponentModel.DataAnnotations;

namespace EC.Models.API.v1.GlobalSettings
{
    public class CreateGlobalSettingsModel
    {
        public int client_id { get; set; }
        [Required]
        [StringLength(255)]
        public string custom_logo_path { get; set; }
        //[Required]
        //[StringLength(255)]
        //public string application_name { get; set; }
        [Required]
        [StringLength(255)]
        public string header_links_color_code { get; set; }
        [Required]
        [StringLength(255)]
        public string header_color_code { get; set; }
    }
}