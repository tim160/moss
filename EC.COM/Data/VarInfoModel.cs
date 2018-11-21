﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EC.COM.Data
{
    public class VarInfoModel
    {
        public int Id { get; set; }
        public string First_nm { get; set; }
        public string Last_nm { get; set; }
        public string Company_nm { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Invitation_code { get; set; }

        /*info from 3rd page*/
        public int? Employee_no { get; set; }
        public int? Non_employee_no { get; set; }
        public int? Customers_no { get; set; }

        public int Contract_length { get; set; }
        /* 3rd page generated price*/
        public decimal Annual_plan_price { get; set; }
        public decimal Non_employee_price { get; set; }
        public decimal Customers_price { get; set; }
        public decimal Onboarding_price { get; set; }
        public decimal Total_price { get; set; }

        public DateTime Created_dt { get; set; }
        /*returned confirmation from Beanstrem - payment successfull*/
        public string Billing_code_confirmation { get; set; }
        /*emailed code to customer - should be unique! and should use to get the info for registration*/
        public string Emailed_code_to_customer { get; set; }

        /*use this section, when company actualy registering itself*/
        /*update to true when company registered*/
        public bool Is_registered { get; set; }
        public DateTime? Registered_dt { get; set; }
        public string Registered_company_nm { get; set; }
        public string Registered_first_nm { get; set; }
        public string Registered_last_nm { get; set; }
        public int Year { get; set; }
    }
}