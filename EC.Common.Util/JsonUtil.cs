﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web;

namespace EC.Common.Util
{
    public class JsonUtil
    {
        public static string ListToJsonWithJavaScriptSerializer(List<int> _list)
        {
            //  for(int i )

            //  int[]


            JavaScriptSerializer jsSerializer = new JavaScriptSerializer();

            var json = jsSerializer.Serialize(_list);
            return json;
        }

        public static string DataTableToJSONWithJavaScriptSerializer(DataTable table)
        {
            JavaScriptSerializer jsSerializer = new JavaScriptSerializer();
            List<Dictionary<string, object>> parentRow = new List<Dictionary<string, object>>();
            Dictionary<string, object> childRow;
            foreach (DataRow row in table.Rows)
            {
                childRow = new Dictionary<string, object>();
                foreach (DataColumn col in table.Columns)
                {
                    childRow.Add(col.ColumnName, row[col]);
                }
                parentRow.Add(childRow);
            }
            return jsSerializer.Serialize(parentRow);
        }
    }
}
