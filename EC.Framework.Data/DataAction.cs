using System;

namespace EC.Framework.Data
{
    /// <summary>
    /// Specifies the possible data actions of the business objects and collections.
    /// </summary>
    public enum DataAction
    {
        /// <summary>
        /// None.
        /// </summary>
        None = 1,

        /// <summary>
        /// Insert.
        /// </summary>
        Insert = 2,

        /// <summary>
        /// Update.
        /// </summary>
        Update = 3,

        /// <summary>
        /// Delete.
        /// </summary>
        Delete = 4
    }
}