using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
//using Clevest.Business.OrderEventLogic.Util;
//using Clevest.Business.Entities;
//using Clevest.OrderType;
using EC.Framework.Logger;

namespace EC.Framework.Email
{
    public class TemplateParser
    {
        //private static readonly ICustomLog m_Log =
        //    CustomLogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        ///// <summary>
        ///// When we know the input entity is a workorder, we can use this method - the fields
        ///// in the template are of the form '[ field ]'
        ///// </summary>
        ///// <param name="order"></param>
        ///// <param name="templateData"></param>
        ///// <param name="isHtml"></param>
        ///// <returns></returns>
        //public static string GetFormattedOrderText(IOrder order, string templateData, bool isHtml)
        //{
        //    IOrderType ot = (order as WorkOrder).GetOrderType();

        //    string[] fields = ParseTemplateFields(templateData);

        //    foreach (string field in fields)
        //    {
        //        templateData = GetFormattedOrderFieldText(order, templateData, isHtml, "[{0}]", ot, field);
        //    }
        //    return templateData;
        //}

        ///// <summary>
        ///// Handle order extension fields and repeatable fields
        ///// </summary>
        ///// <param name="order"></param>
        ///// <param name="templateData"></param>
        ///// <param name="isHtml"></param>
        ///// <param name="findFormat"></param>
        ///// <param name="ot"></param>
        ///// <param name="field"></param>
        ///// <returns></returns>
        //private static string GetFormattedOrderFieldText(IOrder order, string templateData, bool isHtml, string findFormat, IOrderType ot, string field)
        //{
        //    IField otField = ot.GetField(field);
        //    if (otField != null)
        //    {
        //        if (ot.GetField(field).RepeatableTypeName != string.Empty)
        //        {
        //            IRepeatable[] repeatable = order.GetRepeatables(field);
        //            if (repeatable != null && repeatable.Length > 0)
        //            {
        //                if (isHtml)
        //                {
        //                    templateData = templateData.Replace(string.Format(findFormat, field), GetRepeatableHTMLTemplateData(order, repeatable, field));
        //                }
        //                else
        //                {
        //                    templateData = templateData.Replace(string.Format(findFormat, field), GetRepeatableTextTemplateData(order, repeatable));
        //                }
        //            }
        //            else
        //            {
        //                templateData = templateData.Replace(string.Format(findFormat, field), string.Empty);
        //            }
        //        }
        //        else
        //        {
        //            object value = null;
        //            try
        //            {
        //                value = order.GetValue(field);
        //            }
        //            catch (Exception e)
        //            {
        //                m_Log.DebugFormat("GetFormattedOrderText() - Error in GetValue for field = {0}", field);
        //                m_Log.Debug("GetFormattedOrderText(), Error:", e);
        //                value = null;
        //            }
        //            if (value != null)
        //            {
        //                if (value is DateTime)
        //                {

        //                    DateTime dt = ((DateTime)value).ToLocalTime();
        //                    string dtString = dt.ToString("g");
        //                    if (order.TimeZoneIndex.HasValue)
        //                    {
        //                        Clevest.Framework.TimeZone.TimeZoneInfo info = Clevest.Framework.TimeZone.TimeZoneInfo.FromIndex(order.TimeZoneIndex.Value);
        //                        dt = Clevest.Framework.TimeZone.TimeZoneInfo.FromUniversalTime(order.TimeZoneIndex.Value, (DateTime)value);
        //                        dtString = dt.ToString("g");
        //                        dtString += " ";
        //                        dtString += info.ShortStandardName;
        //                    }

        //                    if (isHtml)
        //                    {
        //                        dtString = System.Web.HttpUtility.HtmlEncode(dtString);
        //                    }

        //                    templateData = templateData.Replace(string.Format(findFormat, field), dtString);
        //                }

        //                else
        //                {
        //                    string data = value.ToString();
        //                    if (isHtml)
        //                    {
        //                        data = System.Web.HttpUtility.HtmlEncode(data);
        //                    }
        //                    templateData = templateData.Replace(string.Format(findFormat, field), data);
        //                }
        //            }
        //            else
        //            {
        //                templateData = templateData.Replace(string.Format(findFormat, field), string.Empty);
        //            }
        //        }
        //    }
        //    else
        //    {
        //        m_Log.DebugFormat("GetFormattedOrderFieldText() - Field not found, field = {0}", field);
        //        templateData = templateData.Replace(string.Format(findFormat, field), string.Empty);
        //    }
        //    return templateData;
        //}

        ///// <summary>
        ///// Pass in multiple entities - declare the type of each in order to match the template.
        ///// TODO: may want to use an identifier instead of a type name (e.g. to distinguish between
        ///// two entities of the same type)
        ///// </summary>
        ///// <param name="typeList"></param>
        ///// <param name="entities"></param>
        ///// <param name="templateData"></param>
        ///// <param name="isHtml"></param>
        ///// <returns></returns>
        //public static string GetFormattedText(Type[] typeList, object[] entities, string templateData, bool isHtml)
        //{
        //    Dictionary<string, List<string>> fields = ParseTemplateFieldsWithType(templateData);

        //    foreach (string typeName in fields.Keys)
        //    {
        //        for (int i = 0; i < typeList.Length; i++)
        //        {
        //            if (typeList[i].Name.Equals(typeName))
        //            {
        //                List<string> foundFields = fields[typeName];

        //                if (typeName.Equals("WorkOrder"))
        //                {
        //                    IOrderType ot = (entities[i] as WorkOrder).GetOrderType();

        //                    foreach (string field in foundFields)
        //                    {
        //                        templateData = GetFormattedOrderFieldText(entities[i] as IOrder, templateData, isHtml, "[WorkOrder.{0}]", ot, field);
        //                    }
        //                }
        //                else
        //                {
        //                    foreach (string fieldname in foundFields)
        //                    {
        //                        object value = null;
        //                        try
        //                        {
        //                            PropertyInfo prop = typeList[i].GetProperty(fieldname);
        //                            value = prop.GetValue(entities[i], null);
        //                        }
        //                        catch (Exception e)
        //                        {
        //                            m_Log.Warn(string.Format("GetFormattedText {0}", fieldname), e); 
        //                            value = null;
        //                        }

        //                        if (value != null)
        //                        {
        //                            string data = value.ToString();
        //                            if (isHtml)
        //                            {
        //                                data = System.Web.HttpUtility.HtmlEncode(data);
        //                            }
        //                            templateData = templateData.Replace(string.Format("[{0}.{1}]", typeName, fieldname), data);
        //                        }
        //                        else
        //                        {
        //                            templateData = templateData.Replace(string.Format("[{0}.{1}]", typeName, fieldname), string.Empty);
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }

        //    return templateData;
        //}

        ///// <summary>
        ///// Passing in one type and one entity of that type
        ///// NOTE: Use GetOrderFormattedText() for WorkOrders
        ///// </summary>
        ///// <param name="objectType"></param>
        ///// <param name="entity"></param>
        ///// <param name="templateData"></param>
        ///// <param name="isHtml"></param>
        ///// <returns></returns>
        //public static string GetFormattedText(Type objectType, object entity, string templateData, bool isHtml)
        //{
        //    string[] fields = ParseTemplateFields(templateData);

        //    foreach (string field in fields)
        //    {
        //        object value = null;
        //        try
        //        {
        //            PropertyInfo prop = objectType.GetProperty(field);
        //            value = prop.GetValue(entity, null);
        //        }
        //        catch (Exception e)
        //        {
        //            m_Log.Warn(string.Format("GetFormattedText - get property/value error - {0}", field), e); 
        //            value = null;
        //        }

        //        if (value != null)
        //        {
        //            string data = value.ToString();
        //            if (isHtml)
        //            {
        //                data = System.Web.HttpUtility.HtmlEncode(data);
        //            }
        //            templateData = templateData.Replace("[" + field + "]", data);
        //        }
        //        else
        //        {
        //            templateData = templateData.Replace("[" + field + "]", string.Empty);
        //        }
        //    }

        //    return templateData;
        //}


        //private static string GetRepeatableTextTemplateData(IOrder order, IRepeatable[] repeatable)
        //{
        //    StringBuilder repeatableBuilder = new StringBuilder();
        //    string repeatableHeader = string.Empty;
        //    IOrderType ot = (order as WorkOrder).GetOrderType();

        //    if (repeatable == null)
        //    {
        //        m_Log.Warn("GetRepeatableTextTemplateData() - Repeatable is NULL");
        //        return string.Empty;
        //    }

        //    if (ot != null)
        //    {
                
        //        for (int i = 0; i < repeatable[0].RepeatableFields.Length; i++)
        //        {

        //            string fieldName = repeatable[0].RepeatableFields[i].FieldName;
        //            if (i > 0)
        //                repeatableHeader += ",";

        //            IField field = ot.GetField(fieldName);
        //            if (field != null)
        //            {
        //                repeatableHeader += field.FieldLabel;
        //            }
        //            else 
        //            {
        //                m_Log.DebugFormat("GetRepeatableTextTemplateData() - Field not found, field = {0}", fieldName);
        //            }


        //        }
        //        repeatableBuilder.AppendLine(repeatableHeader);

        //        foreach (IRepeatable rptRecord in repeatable)
        //        {
        //            string valueLine = string.Empty;
        //            for (int x = 0; x < rptRecord.RepeatableFields.Length; x++)
        //            {

        //                object value = rptRecord.RepeatableFields[x].Value;

        //                if (x > 0)
        //                    valueLine += ",";

        //                if (value is DateTime)
        //                {
        //                    DateTime dt = ((DateTime)value).ToLocalTime();
        //                    string dtString = dt.ToString("g");
        //                    if (order.TimeZoneIndex.HasValue)
        //                    {
        //                        Clevest.Framework.TimeZone.TimeZoneInfo info = Clevest.Framework.TimeZone.TimeZoneInfo.FromIndex(order.TimeZoneIndex.Value);
        //                        dt = Clevest.Framework.TimeZone.TimeZoneInfo.FromUniversalTime(order.TimeZoneIndex.Value, (DateTime)value);
        //                        dtString = dt.ToString("g");
        //                        dtString += " ";
        //                        dtString += info.ShortStandardName;
        //                    }

        //                    value = dtString;
        //                }

        //                valueLine += value.ToString();

        //            }
        //            repeatableBuilder.AppendLine(valueLine);
        //        }
        //    }
        //    return repeatableBuilder.ToString();
        //}
        //private static string GetRepeatableHTMLTemplateData(IOrder order, IRepeatable[] repeatable, string fieldname)
        //{
        //    IOrderType ot = (order as WorkOrder).GetOrderType();
        //    if (ot != null && repeatable != null && repeatable.Length > 0)
        //    {
        //        StringBuilder repeatableBuilder = new StringBuilder();
        //        repeatableBuilder.Append(string.Format("<TABLE id=\"{0}\" class=\"repeatable\">", fieldname));
        //        repeatableBuilder.Append("<TR>");
        //        for (int i = 0; i < repeatable[0].RepeatableFields.Length; i++)
        //        {

        //            string fieldName = repeatable[0].RepeatableFields[i].FieldName;
        //            repeatableBuilder.Append("<TH>");
        //            IField field = ot.GetField(fieldName);
        //            if (field != null)
        //            {
        //                repeatableBuilder.Append(ot.GetField(fieldName).FieldLabel);
        //            }
        //            else
        //            {
        //                m_Log.DebugFormat("GetRepeatableHTMLTemplateData() - Field not found, field = {0}", fieldName);
        //            }
        //            repeatableBuilder.Append("</TH>");

        //        }
        //        repeatableBuilder.Append("</TR>");

        //        foreach (IRepeatable rptRecord in repeatable)
        //        {
        //            repeatableBuilder.Append("<TR>");
        //            foreach (IRepeatField rptField in rptRecord.RepeatableFields)
        //            {

        //                repeatableBuilder.Append("<TD>");
        //                object value = rptField.Value;
        //                if (value != null)
        //                {
        //                    if (value is DateTime)
        //                    {
        //                        DateTime dt = ((DateTime)value).ToLocalTime();
        //                        string dtString = dt.ToString("g");
        //                        if (order.TimeZoneIndex.HasValue)
        //                        {
        //                            Clevest.Framework.TimeZone.TimeZoneInfo info = Clevest.Framework.TimeZone.TimeZoneInfo.FromIndex(order.TimeZoneIndex.Value);
        //                            dt = Clevest.Framework.TimeZone.TimeZoneInfo.FromUniversalTime(order.TimeZoneIndex.Value, (DateTime)value);
        //                            dtString = dt.ToString("g");
        //                            dtString += " ";
        //                            dtString += info.ShortStandardName;
        //                        }
        //                        value = System.Web.HttpUtility.HtmlEncode(dtString);
        //                    }
        //                    else
        //                    {
        //                        value = System.Web.HttpUtility.HtmlEncode(value.ToString());
        //                    }
        //                    repeatableBuilder.Append(value.ToString());
        //                }
        //                repeatableBuilder.Append("</TD>");

        //            }
        //            repeatableBuilder.Append("</TR>");
        //        }
        //        repeatableBuilder.Append("</TABLE>");

        //        return repeatableBuilder.ToString();
        //    }
        //    else
        //    {
                
        //        return string.Empty;
        //    }
        //}
        //private static string[] ParseTemplateFields(string template)
        //{
        //    bool inBracket = false;
        //    string field = string.Empty;
        //    List<string> fields = new List<string>();
        //    foreach (char chr in template)
        //    {

        //        if (chr == '[')
        //        {
        //            inBracket = true;
        //            continue;
        //        }
        //        if (chr == ']')
        //        {
        //            inBracket = false;
        //            fields.Add(field);
        //            field = string.Empty;
        //        }

        //        if (inBracket)
        //            field += chr;

        //    }

        //    return fields.ToArray();
        //}

        ///// <summary>
        ///// Templates parsed with this method need to have fields in the form [ {type name}.{field name} ]
        ///// </summary>
        ///// <param name="template"></param>
        ///// <returns></returns>
        //public static Dictionary<string, List<string>> ParseTemplateFieldsWithType(string template)
        //{
        //    bool inBracket = false;
        //    string field = string.Empty;

        //    Dictionary<string, List<string>> fieldDictionary = new Dictionary<string, List<string>>();

        //    List<string> fields = new List<string>();
        //    foreach (char chr in template)
        //    {
        //        if (chr == '[')
        //        {
        //            inBracket = true;
        //            continue;
        //        }
        //        if (chr == ']')
        //        {
        //            inBracket = false;

        //            string[] parsedValues = field.Split(new char[] { '.' }, 2);
        //            if (parsedValues.Length == 2)
        //            {
        //                if (!fieldDictionary.ContainsKey(parsedValues[0]))
        //                {
        //                    List<string> typeFieldList = new List<string>();
        //                    typeFieldList.Add(parsedValues[1]);
        //                    fieldDictionary[parsedValues[0]] = typeFieldList;
        //                }
        //                else
        //                {
        //                    fieldDictionary[parsedValues[0]].Add(parsedValues[1]);
        //                }
        //            }
        //            field = string.Empty;
        //        }

        //        if (inBracket)
        //            field += chr;

        //    }

        //    return fieldDictionary;
        //}

    }
}
