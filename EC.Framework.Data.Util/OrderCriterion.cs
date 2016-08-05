using System;
using System.Reflection;

namespace EC.Framework.Data
{
    public class OrderCriterion : ICloneable
    {
        #region Property(s)
        private Type m_Type;
        private PropertyInfo m_PropertyInfo;
        private string m_PropertyAlias = String.Empty;
        private string m_OriginalPropertyName;
        private string m_WorkTypeName;
        private string m_SortOrder = "ASC";
        private Type m_DataType = null;
        private Int32 m_Length = 0;
        private string m_XmlPath = string.Empty;
        private AggregateFunction m_AggregateFunction;

        public Type Type
        {
            get { return m_Type; }
            set { m_Type = value; }
        }

        public PropertyInfo PropertyInfo
        {
            get { return m_PropertyInfo; }
            set { m_PropertyInfo = value; }
        }

        public string PropertyAlias
        {
            get { return m_PropertyAlias; }
            set { m_PropertyAlias = value; }
        }

        public string OriginalPropertyName
        {
            get { return m_OriginalPropertyName; }
            set { m_OriginalPropertyName = value; }
        }

        public string WorkTypeName
        {
            get { return m_WorkTypeName; }
            set { m_WorkTypeName = value; }
        }

        public string SortOrder
        {
            get { return m_SortOrder; }
            set { m_SortOrder = value; }
        }

        public Type DataType
        {
            get { return m_DataType; }
            set { m_DataType = value; }
        }

        public Int32 Length
        {
            get { return m_Length; }
            set { m_Length = value; }
        }

        public string XmlPath
        {
            get { return m_XmlPath; }
            set { m_XmlPath = value; }
        }

        public AggregateFunction AggregateFunction
        {
            get { return m_AggregateFunction; }
            set { m_AggregateFunction = value; }
        }
        #endregion

        #region Constructor(s)
        public OrderCriterion(Type type, string propertyName, string sortOrder, AggregateFunction aggregateFunction)
            :this(type, null, 0, string.Empty, propertyName, sortOrder, aggregateFunction)
        {
        }

        public OrderCriterion(Type type, Type dataType, Int32 length, string xmlPath, string propertyName, string sortOrder, AggregateFunction aggregateFunction)
            :this (type, dataType, length, xmlPath, propertyName, null, sortOrder, aggregateFunction)
        {
        }

        public OrderCriterion(Type type, Type dataType, Int32 length, string xmlPath, string propertyName, string wtName, string sortOrder, AggregateFunction aggregateFunction)
        {
            Initialize(type, dataType, length, xmlPath, propertyName, wtName, sortOrder, aggregateFunction);
        }

        public OrderCriterion(Type type, PropertyInfo propertyInfo, string sortOrder, AggregateFunction aggregateFunction)
        {
            Initialize(type, null, 0, string.Empty, propertyInfo, sortOrder, aggregateFunction);
        }

        public OrderCriterion(OrderCriterion orderCriterion)
        {
            m_OriginalPropertyName = orderCriterion.OriginalPropertyName;
            Initialize(orderCriterion.Type, orderCriterion.DataType, orderCriterion.Length, orderCriterion.XmlPath, orderCriterion.PropertyInfo, orderCriterion.SortOrder, orderCriterion.AggregateFunction);
        }
        #endregion

        #region Method(s)
        private void Initialize(Type type, Type dataType, Int32 length, string xmlPath, string propertyName, string workTypeName, string sortOrder, AggregateFunction aggregateFunction)
        {
            if (propertyName == null)
                throw new ApplicationException("Could not initialize OrderCriterion; property name is null!");

            PropertyInfo propertyInfo = null;
            if (dataType == null)
                propertyInfo = GetProperty(type, propertyName);
            else
            {
                char[] delimiter = { '.' };
                string[] xmlProperty = propertyName.Split(delimiter);
                propertyInfo = GetProperty(type, xmlProperty[0]);
            }

            m_OriginalPropertyName = propertyName;
            m_WorkTypeName = workTypeName;
            Initialize(type, dataType, length, xmlPath, propertyInfo, sortOrder, aggregateFunction);
        }

        private void Initialize(Type type, Type dataType, Int32 length, string xmlPath, PropertyInfo propertyInstance, string sortOrder, AggregateFunction aggregateFunction)
        {
            if (type == null)
                throw new ApplicationException("Could not initialize order criterion; type is null!");

            if (dataType == null)
            {
                if (propertyInstance == null)
                {

                    // not sure if we need to throw this
                    //throw new ApplicationException(string.Format("Could not initialize order criterion; property ({0}) does not exist!", m_OriginalPropertyName));
                }
            }

            if (sortOrder == null || sortOrder.Length <= 0)
                sortOrder = "ASC";
            else
            {
                sortOrder = sortOrder.ToUpper();
                if (sortOrder != "ASC" && sortOrder != "DESC")
                    throw new ApplicationException("Could not initialize OrderCriterion; Invalid Sort Order!");
            }

            m_Type = type;
            m_DataType = dataType;
            m_Length = length;
            m_PropertyInfo = propertyInstance;
            m_SortOrder = sortOrder;
            m_XmlPath = xmlPath;
            m_AggregateFunction = aggregateFunction;
        }

        private PropertyInfo GetProperty(Type type, string propertyName)
        {
            return type.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        }
        #endregion

        #region ICloneable
        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns></returns>
        public OrderCriterion Clone()
        {
            return new OrderCriterion(this);
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        object ICloneable.Clone()
        {
            return this.Clone();
        }
        #endregion
    }
}