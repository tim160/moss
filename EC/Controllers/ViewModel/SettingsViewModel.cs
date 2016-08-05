using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EC.Controllers.ViewModel
{
    public class SettingsViewModel
    {
        public int companyId { set; get; }
        public int userId { set; get; }
        public string data { set; get; }
        public bool flag { set; get; }
        public string newSetting { set; get; }
    }
}