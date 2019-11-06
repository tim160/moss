using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EC.Models.Database;

namespace EC.Controllers.ViewModel
{
    public class MediatorsView
    {
        public List<user> _involved_mediators_user_list { get; set; }
        public List<user> _mediators_whoHasAccess_toReport { get; set; }
        public List<user> _available_toAssign_mediators { get; set; }
    }
}