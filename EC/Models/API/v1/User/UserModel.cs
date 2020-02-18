using EC.Models.ViewModels;
using System.ComponentModel.DataAnnotations;

namespace EC.Models.API.v1.User
{
    public class UserModel
    {
        public int id { get; set; }
        [Required]
        public int partnerCompanyID { get; set; }
        public string partnerUserID { get; set; }

     //   [Required]
   //     public int roleId { get; set; }
        [Required]
        [StringLength(250)]
        public string firstName { get; set; }
        [Required]
        [StringLength(250)]
        public string lastName { get; set; }
        [Required]
        public string photoPath { get; set; }
        [Required]
        [StringLength(250)]
        public string email { get; set; }

        public UsersUnreadEntitiesNumberViewModel usersUnreadEntities { get; set; }

    }
}