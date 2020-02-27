using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using EC.Models.API.v1.User;

namespace EC.Models.API.v1.Company
{
	public class CreateCompanyModel
	{
        public int Id { get; set; }

        [Required]
        [StringLength(500)]
        public string CompanyName { get; set; }

        [Required]
        public string PartnerCompanyId { get; set; }

        [Required]
        public string PartnerClientId { get; set; }

        [Required]
        public bool OptinCaseAnalytics { get; set; }

        [Required]
        public int EmployeeQuantity { get; set; }

        [Required]
        public string CustomLogoPath { get; set; }

        public List<Location> Locations { get; set; }

        public List<CreateUserModel> Users { get; set; }
    }

    public class Location
    {
        public string Location_nm { get; set; }
    }
}