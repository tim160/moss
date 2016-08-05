using System;

namespace EC.Framework.Data
{
    /// <summary>
    /// Gets the connection info.
    /// </summary>
    public abstract class AbstractConnectionInfo 
    {
        protected string m_DbConnectionString;
        protected string m_ConnectionType;

        /// <summary>
        /// Connection string to the persistent data store that will used by an IObjectBroker.
        /// </summary>
        /// <value>The connection string.</value>
        public string ConnectionString
        {
            get { return m_DbConnectionString; }
        }

        public string ConnectionType
        {
            get { return m_ConnectionType; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionInfo"/> class.
        /// </summary>
        /// <param name="connectionType">Type of the connection.</param>
        /// <param name="selectCommandType">Type of the select command.</param>
        /// <param name="brokerType">Type of the broker.</param>
        /// <param name="connectionSettings">The connection settings.</param>
        public AbstractConnectionInfo(string connectionString, string connectionType)
        {
            m_DbConnectionString = connectionString;
            m_ConnectionType = connectionType;
        }
    }
}