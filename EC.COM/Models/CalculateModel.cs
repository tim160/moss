using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EC.COM.Models
{
    public class CalculateModel
    {
        public string InvitationCode { get; set; }
        public int NumberOfEmployees { get; set; }
        public int NumberOfNonEmployees { get; set; }
        public int NumberOfClients { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CompanyName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public decimal PriceNE { get; set; }
        public decimal PriceNNE { get; set; }
        public decimal PriceC { get; set; }
        public decimal PriceR { get; set; }
        public decimal PriceCC { get; set; }
        public int Year { get; set; }
        public decimal AnnualyTotal { get; set; }
        public int sessionNumber { get; set; }
        public string sessionN { get; set; }

        public decimal GrandTotal { get; set; }
        public string QuickView { get; set; }
        public bool callCenter { get; set; }
    }
}