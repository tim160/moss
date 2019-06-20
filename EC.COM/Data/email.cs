using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EC.COM.Data
{
    public partial class email
    {
        public int id { get; set; }
        public int company_id { get; set; }
        public int user_id_from { get; set; }
        public string To { get; set; }
        public string From { get; set; }
        public string cc { get; set; }
        public string bcc { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public int EmailType { get; set; }
        public byte[] attachment { get; set; }
        public System.DateTime created_dt { get; set; }
        public Nullable<bool> isSSL { get; set; }
        public Nullable<System.DateTime> sent_dt { get; set; }
        public Nullable<bool> is_sent { get; set; }
        public int user_id_to { get; set; }
    }
}