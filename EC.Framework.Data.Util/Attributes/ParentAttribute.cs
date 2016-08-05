using System;
using System.Collections.Generic;
using System.Text;

namespace EC.Framework.Data
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class ParentAttribute : Attribute
    {
        #region Properties

        public string EntityType { get; set; }
    
        /// <summary>
        /// Gets the foreign key.
        /// </summary>
        /// <value>The foreign key.</value>
        public string ForeignKey { get; set; }

        #endregion

        #region Methods
        public ParentAttribute()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ParentAttribute"/> class.
        /// </summary>
        /// <param name="foreignKey">The foreign key.</param>
        public ParentAttribute(string foreignKey)
        {
            ForeignKey = foreignKey;
        }

        public ParentAttribute(string foreignKey, string entityType)
        {
            ForeignKey = foreignKey;
            EntityType = entityType;
        }
        #endregion
    }
}