using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using EC.Models;

using EC.Controllers.ViewModel;
using EC.Models.Database;
using EC.Models.ECModel;
using EC.Models.Utils;

namespace EC.Controllers.ViewModel
{
    public class ActiveCasesViewModel : BaseViewModel
    {
        public string caseNumber { get; set; }
        public string caseDescription { get; set; }
        public string personName { get; set; }
        public string personLastName { get; set; }
        public string personTitle { get; set; }
        public string personRole { get; set; }
        public int reporterTypeDetail { get; set; }
        public string companyRelationshipOther { get; set; }
        public DateTime dateIncidentHappened { get; set; }
        public DateTime reportedTime { get; set; }
        public DateTime daysLeft { get; set; }
        public DateTime lastActivity { get; set; }
        public int locationsOfIncident { get; set; }
        public int isOnGoing { get; set; }
        
    }
}