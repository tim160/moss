using System;
using System.Collections.Generic;
using System.Text;

namespace EC.Framework.Data
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false)]
    public sealed class TableAttribute : Attribute, ICloneable
    {
        #region Properties
        private readonly string m_TableName;
        private readonly bool m_IsView = false;

        /// <summary>
        /// Gets the name of the table.
        /// </summary>
        /// <value>The name of the table.</value>
        public string TableName
        {
            get { return m_TableName; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is view.
        /// </summary>
        /// <value><c>true</c> if this instance is view; otherwise, <c>false</c>.</value>
        public bool IsView
        {
            get { return m_IsView; }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Initializes a new instance of the <see cref="TableAttribute"/> class.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        public TableAttribute(string tableName)
        {
            m_TableName = tableName;
        }

        public TableAttribute(string tableName, bool isView)
        {
            m_TableName = tableName;
            m_IsView = isView;
        }
        #endregion

        #region ICloneable
        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public object Clone()
        {
            return this.MemberwiseClone();
        }
        #endregion
    }
}