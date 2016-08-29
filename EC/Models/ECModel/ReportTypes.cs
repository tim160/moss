using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace EC.Models.ECModel
{
    public class ReportTypes
    {

        private string _secondaryTypesIDStrings;
        private string _reportRelationIDStrings;
        private string _reportDepartmentIDStrings;
        private string _reportLocationIDStrings;

        public string ReportsSecondaryTypesIDStrings
        {
            get { return string.IsNullOrEmpty(_secondaryTypesIDStrings) ? "" : _secondaryTypesIDStrings; }
            set { _secondaryTypesIDStrings = value; }
        }
        public string ReportsRelationTypesIDStrings
        {
            get { return string.IsNullOrEmpty(_reportRelationIDStrings) ? "" : _reportRelationIDStrings; }
            set { _reportRelationIDStrings = value; }
        }
        public string ReportsDepartmentIDStringss 
        {
            get { return string.IsNullOrEmpty(_reportDepartmentIDStrings) ? "" : _reportDepartmentIDStrings; }
            set { _reportDepartmentIDStrings = value; }
        }
        public string ReportsLocationIDStrings
        {
            get { return string.IsNullOrEmpty(_reportLocationIDStrings) ? "" : _reportLocationIDStrings; }
            set { _reportLocationIDStrings = value; }
        }
        public long dateStart { get; set; }
        public long dateEnd { get; set; }
    }
}