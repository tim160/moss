using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EC.Models.Database;


namespace EC.Models.ViewModels
{
    public class CaseInvestigationStatusViewModel : EC.Models.Database.report_investigation_status
    {
        public int previous_investigation_status { get; set; }
        public string investigation_status_name { get; set; }
        public string query_new_investigation_status_name { get; set; }
    }
}