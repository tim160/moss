using EC.Models.API.v1.GlobalSettings;
using System.ComponentModel.DataAnnotations;

namespace EC.Models.API.v1.Client
{
    public class CreateClientModel
    {
        public int Id { get; set; }

        [StringLength(100)]
        [Required]
        public string ClientName { get; set; }

        public string PartnerClientId { get; set; }

        public CreateGlobalSettingsModel GlobalSettings { get; set; }
    }
}