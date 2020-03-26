using EC.Models.ECModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EC.Services.ProgressLineService
{
    public class CaseHeaderViewModel
    {
        public List<DataProgressLine> ProgressStepsCount { get; set; }

        public string ActionButton { get; set; }

        public List<User> mediators { get; set; }

        //need pctures and names

    }

    public struct DataProgressLine
    {
        public String ProgressStepsText { get; set; }
        public String ProgressStepsClass { get; set; }
    }
}