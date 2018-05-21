using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EC.Models.ViewModels
{
    public class QuickUserViewModel
    {
        public int id { get; set; }
        public string first_nm { get; set; }
        public string last_nm { get; set; }
        public string photo_path { get; set; }
        public bool is_owner { get; set; }
        public bool is_signoff { get; set; }

    }
}