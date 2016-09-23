using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EC.Models;
using EC.Models.Database;
using EC.Model.Interfaces;

namespace EC.Models.ECModel
{
    public class BaseEntity
    {
        public ECEntities db = new ECEntities();
        public Int64 PublicID { get; set; }
        public DateTime AddedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string IP { get; set; }
        protected IDateTimeHelper m_DateTimeHelper;

        public GlobalFunctions glb = new GlobalFunctions();
    }
}