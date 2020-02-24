using System.ComponentModel.DataAnnotations;

namespace EC.Models.API.v1.GlobalSettings
{
    public class CreateGlobalSettingsModel
    {
        public int ClientId { get; set; }

        [Required]
        [StringLength(255)]
        public string CustomLogoPath { get; set; }

        [Required]
        [StringLength(255)]
        public string HeaderLinksColorCode { get; set; }

        [Required]
        [StringLength(255)]
        public string HeaderColorCode { get; set; }
    }
}