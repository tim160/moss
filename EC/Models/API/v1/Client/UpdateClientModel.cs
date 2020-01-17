using System.ComponentModel.DataAnnotations;

namespace EC.Models.API.v1.Client
{
    public class UpdateClientModel
    {
        [Required]
        [StringLength(250)]
        public string EmployeeQuantity { get; set; }
    }
}