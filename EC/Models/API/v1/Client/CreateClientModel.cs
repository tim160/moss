using System.ComponentModel.DataAnnotations;

namespace EC.Models.API.v1.Client
{
    public class CreateClientModel
    {
        [Required]
        [StringLength(500)]
        public string Name { get; set; }

        [Required]
        [StringLength(250)]
        public string EmployeeQuantity { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string InvitationCode { get; set; }

        [Required]
        public string EmailedCodeToCustomer { get; set; }
    }
}