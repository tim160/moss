using EC.Models.API.v1.GlobalSettings;
using System.ComponentModel.DataAnnotations;

namespace EC.Models.API.v1.Client
{
    public class UpdateClientModel
    {
        //[Required]
        //public int address_id { get; set; }

        //[Required]
        //public int status_id { get; set; }

        [StringLength(100)]
        [Required]
        public string client_nm { get; set; }

        //[StringLength(100)]
        //public string client_ds { get; set; }

        //[StringLength(255)]
        //public string notepad_tx { get; set; }

        //[Required]
        //public int user_id { get; set; }
        public string PartnerInternalID { get; set; }

        public CreateGlobalSettingsModel globalSettings { get; set; }
    }
}