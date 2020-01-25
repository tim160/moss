using EC.Models.ViewModels;
using System.ComponentModel.DataAnnotations;

namespace EC.Models.API.v1.User
{
    public class UserModel
    {
        public int id { get; set; }
        [Required]
        public int company_id { get; set; }
        [Required]
        public int role_id { get; set; }
        [Required]
        [StringLength(250)]
        public string first_nm { get; set; }
        [Required]
        [StringLength(250)]
        public string last_nm { get; set; }
        [Required]
        public string photo_path { get; set; }
        [Required]
        [StringLength(250)]
        public string email { get; set; }
        public string PartnerInternalID { get; set; }
        public UsersUnreadEntitiesNumberViewModel usersUnreadEntities { get; set; }

    }
}