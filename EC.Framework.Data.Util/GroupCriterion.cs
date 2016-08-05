using System;
using System.Reflection;

namespace EC.Framework.Data
{
    public class GroupCriterion : ICloneable
    {
        #region Property(s)
        private Type m_Type;
        private PropertyInfo m_PropertyInfo;
        private string m_PropertyAlias = String.Empty;
        private string m_OriginalPropertyName;

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
        #endregion

        #region Constructor(s)
        public GroupCriterion(Type type, string propertyName)
        {
            Initialize(type, propertyName);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GroupCriterion"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="propertyInstance">The property instance.</param>
        public GroupCriterion(Type type, PropertyInfo propertyInstance)
        {
            Initialize(type, propertyInstance);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GroupCriterion"/> class.
        /// </summary>
        /// <param name="groupCriterion">The group criterion.</param>
        public GroupCriterion(GroupCriterion groupCriterion)
        {
            m_OriginalPropertyName = groupCriterion.OriginalPropertyName;
            Initialize(groupCriterion.Type, groupCriterion.PropertyInfo);
        }
        #endregion

        #region Method(s)
        private void Initialize(Type type, string propertyName)
        {
            if (propertyName == null)
                throw new ApplicationException("Could not initialize GroupCriterion; property name is null!");

            PropertyInfo propertyInstance = GetProperty(type, propertyName);
            m_OriginalPropertyName = propertyName;
            Initialize(type, propertyInstance);
        }

        private void Initialize(Type type, PropertyInfo propertyInstance)
        {
            if (type == null)
                throw new ApplicationException("Could not initialize group criterion; type is null!");
            if (propertyInstance == null)
                throw new ApplicationException("Could not initialize group criterion; property is null!");

            m_Type = type;
            m_PropertyInfo = propertyInstance;
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
        public GroupCriterion Clone()
        {
            return new GroupCriterion(this);
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