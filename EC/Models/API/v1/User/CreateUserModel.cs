using System.ComponentModel.DataAnnotations;

namespace EC.Models.API.v1.User
{
    public class CreateUserModel
    {
        [Required]
        [StringLength(250)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(250)]
        public string LastName { get; set; }

        [Required]
        public string PhotoPath { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(250)]
        public string Title { get; set; }

        [Required]
        [StringLength(250)]
        public string Department { get; set; }
        
        [Required]
        public string PartnerCompanyId { get; set; }

        [Required]
        public string PartnerUserId { get; set; }
    }
}