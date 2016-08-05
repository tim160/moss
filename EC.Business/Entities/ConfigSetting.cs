using TimeZoneInfo = EC.Framework.TimeZone.TimeZoneInfo;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using EC.Business.Actions;

namespace EC.Business.Entities
{
    public class ConfigSetting : IComparable
    {
        private string m_Name;
        private string m_Value;
        private string m_Description;
        private string m_DataType;
        private bool m_RestartNeeded;

        public string Name
        {
            get { return m_Name; } 
            set { m_Name = value; } 
        }

        public string Value
        {
            get { return m_Value; }
            set { m_Value = value; } 
        }

        public string Description
        {
            get { return m_Description; }
            set { m_Description = value; } 
        }

        public string DataType
        {
            get { return m_DataType; }
            set { m_DataType = value; }
        }

        public bool RestartNeeded
        {
            get { return m_RestartNeeded; }
            set { m_RestartNeeded = value; }
        }

        public SettingDataType GetSettingDataType()
        {
            if (m_DataType.ToLower() == "boolean")
                return SettingDataType.Boolean;
            else if (m_DataType.ToLower() == "int32")
                return SettingDataType.Int32;
            else if (m_DataType.ToLower() == "decimal")
                return SettingDataType.Decimal;
            else if (m_DataType.ToLower() == "double")
                return SettingDataType.Double;
            else 
                return SettingDataType.String;
        }

       
       

        public void HandleSaveException(Exception ex)
        {
        }

        

        public int CompareTo(object obj)
        {
            if (!(obj is ConfigSetting))
            {
                throw new InvalidCastException("This object is not of type ConfigSetting");
            }
            ConfigSetting setting = (ConfigSetting)obj;
            return this.Name.CompareTo(setting.Name);  // ASC sorting
        }
    }

    public enum SettingDataType
    {
        String,  //default type
        Boolean,
        Int32,
        Decimal,
        Double
    }
}
