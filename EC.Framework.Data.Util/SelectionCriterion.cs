using System;
using System.Reflection;
using System.Text; 

namespace EC.Framework.Data
{
    /// <summary>
    /// SelectionCriterion provides a generic selection criterion mechanism for
    /// business object collections. 
    /// </summary>
    public class SelectionCriterion : ICloneable
    {
        #region Property(s)
        private Type m_Type;
        private OperandType m_OperandType;
        private object m_Value;
        private ComparisonMethod m_ComparisonMethod;
        private PropertyInfo m_Property;
        private string m_PropertyAlias = String.Empty;
        private string m_OriginalPropertyName;
        private string m_WorkTypeName;
        private Type m_DataType;
        private Int32 m_Length = 0;
        private string m_XmlPath = string.Empty;
        private Expression m_Expression = Expression.None;

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        public Type Type
        {
            get { return m_Type; }
            set { m_Type = value; }
        }

        /// <summary>
        /// Gets or sets the type of the operand.
        /// </summary>
        /// <value>The type of the operand.</value>
        public OperandType OperandType
        {
            get { return m_OperandType; }
            set { m_OperandType = value; }
        }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        public object Value
        {
            get { return m_Value; }
            set { m_Value = value; }
        }

        /// <summary>
        /// Gets or sets the comparison method.
        /// </summary>
        /// <value>The comparison method.</value>
        public ComparisonMethod ComparisonMethod
        {
            get { return m_ComparisonMethod; }
            set { m_ComparisonMethod = value; }
        }

        /// <summary>
        /// Gets or sets the property.
        /// </summary>
        /// <value>The property.</value>
        public PropertyInfo Property
        {
            get { return m_Property; }
            set { m_Property = value; }
        }

        /// <summary>
        /// Gets or sets the property alias.
        /// </summary>
        /// <value>The property alias.</value>
        public string PropertyAlias
        {
            get { return m_PropertyAlias; }
            set { m_PropertyAlias = value; }
        }

        /// <summary>
        /// Gets the name of the original property.
        /// </summary>
        /// <value>The name of the original property.</value>
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

        public Expression Expression
        {
            get { return m_Expression; }
            set { m_Expression = value; }            
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="SelectionCriterion"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="value">The value.</param>
        public SelectionCriterion(Type type, string propertyName, object value)
            :this(OperandType.None, type, null, 0, string.Empty, propertyName, value)
        {
        }

        public SelectionCriterion(OperandType operandType, Type type, Type dataType, Int32 length, string xmlPath, string propertyName, object value)
            :this(operandType, type, dataType, length, xmlPath, propertyName, ComparisonMethod.Equals, value)
        {
        }

        public SelectionCriterion(OperandType operandType, Type type, Type dataType, Int32 length, string xmlPath, string propertyName, ComparisonMethod comparisonMethod, object value)
            :this(operandType, type, dataType, length, xmlPath, propertyName, null, comparisonMethod, value)
        {
        }

        public SelectionCriterion(OperandType operandType, Type type, Type dataType, Int32 length, string xmlPath, string propertyName, string workTypeName, ComparisonMethod comparisonMethod, object value)
        {
            Initialize(operandType, type, dataType, length, xmlPath, propertyName, workTypeName, comparisonMethod, value);
        }

        public SelectionCriterion(OperandType operandType, Type type, Type dataType, Int32 length, string xmlPath, PropertyInfo propertyInstance, ComparisonMethod comparisonMethod, object value)
        {
            Initialize(operandType, type, dataType, length, xmlPath, propertyInstance, comparisonMethod, value, false, Expression.None);
        }

        public SelectionCriterion(SelectionCriterion sc)
        {
            m_OriginalPropertyName = sc.OriginalPropertyName;
            Initialize(sc.OperandType, sc.Type, sc.DataType, sc.Length, sc.XmlPath, sc.Property, sc.ComparisonMethod, sc.Value, true, sc.Expression);
        }

        public SelectionCriterion(Expression expression)
        {
            m_Expression = expression;
        }
        #endregion

        #region Method(s)
        private void Initialize(OperandType operandType, Type type, Type dataType, Int32 length, string xmlPath, string propertyName, string wtName, ComparisonMethod comparisonMethod, object value)
        {
            if (propertyName == null)
                throw new ApplicationException("Could not initialize SelectionCriterion; property name is null!");

            PropertyInfo property;

            if (dataType == null)
                property = GetProperty(type, propertyName);
            else
            {
                char[] delimiter = { '.' };
                string[] xmlProperty = propertyName.Split(delimiter);
                property = GetProperty(type, xmlProperty[0]);
            }

            m_OriginalPropertyName = propertyName;
            m_WorkTypeName = wtName;
            Initialize(operandType, type, dataType, length, xmlPath, property, comparisonMethod, value, false, Expression.None);
        }

        private void Initialize(OperandType operandType, Type type, Type dataType, Int32 length, string xmlPath, PropertyInfo propertyInstance, ComparisonMethod comparisonMethod, object value,
            bool isSelect, Expression expression)
        {
            // need to do a check there if it is a expression
            if (expression == Expression.None)
            {
                if (type == null)
                    throw new ApplicationException("Could not initialize selection criterion; type is null!");
                if (propertyInstance == null)
                    throw new ApplicationException(string.Format("Could not initialize selection criterion; property ({0}) does not exist!", m_OriginalPropertyName));
            }

            m_Type = type;
            m_DataType = dataType;
            m_Length = length;
            m_XmlPath = xmlPath;
            m_OperandType = operandType;
            m_Property = propertyInstance;
            m_ComparisonMethod = comparisonMethod;
            if (isSelect && value is string)
                value = (value as string).Replace("'", "''");
            m_Value = value;
            m_Expression = expression;
        }

        private PropertyInfo GetProperty(Type type, string propertyName)
        {
            return type.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic); 
            //PropertyInfo property = null;
            //PropertyInfo[] properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            //foreach (PropertyInfo prop in properties)
            //{
            //    if (prop.Name == propertyName)
            //    {
            //        property = prop;
            //        break;
            //    }
            //}

            //return property;
        }
        #endregion

        public override string ToString()
        {
            return string.Format("[SelectionCriteria] - Type: {0}, OperandType: {1}, Property: {2}, Comparison: {3}, Value: {4}",
                (Type != null) ? Type.ToString() : "null",
                OperandType.ToString(),
                (Property != null) ? Property.Name : "null",
                ComparisonMethod.ToString(),
                (Value != null) ? Value.ToString() : "null"); 
        }

        #region ICloneable
        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns></returns>
        public SelectionCriterion Clone()
        {
            return new SelectionCriterion(this);
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