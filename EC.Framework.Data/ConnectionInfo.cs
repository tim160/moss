using System;
//using EC.Framework.Data.Interfaces;

namespace EC.Framework.Data
{
    /// <summary>
    /// Gets the connection info.
    /// </summary>
    public class ConnectionInfo : IConnectionInfo
    {
        #region Property(s)
        private Type m_DbConnectionType;
        private string m_DbConnectionString;
        private Type m_SelectCommandType;
        private Type m_ObjectBrokerType;

        /// <summary>
        /// Type of CommandBuilder object for the IObjectBroker to utilize.
        /// </summary>
        /// <value>The type of the connection.</value>
        public Type ConnectionType
        {
            get { return m_DbConnectionType; }
            set { m_DbConnectionType = value; }
        }

        /// <summary>
        /// Connection string to the persistent data store that will used by an IObjectBroker.
        /// </summary>
        /// <value>The connection string.</value>
        public string ConnectionString
        {
            get { return m_DbConnectionString; }
            set { m_DbConnectionString = value; }
        }

        /// <summary>
        /// Select query command type.
        /// </summary>
        /// <value>The type of the select command.</value>
        public Type SelectCommandType
        {
            get { return m_SelectCommandType; }
            set { m_SelectCommandType = value; }
        }

        /// <summary>
        /// Type of IObjectBroker implementation.
        /// </summary>
        /// <value>The type of the object broker.</value>
        public Type ObjectBrokerType
        {
            get { return m_ObjectBrokerType; }
            set { m_ObjectBrokerType = value; }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Initializes the specified connection type.
        /// </summary>
        /// <param name="connectionType">Type of the connection.</param>
        /// <param name="selectCommandType">Type of the select command.</param>
        /// <param name="brokerType">Type of the broker.</param>
        /// <param name="connectionSettings">The connection settings.</param>
        protected void Initialize(Type connectionType, Type selectCommandType, Type brokerType, string connectionSettings)
        {
            m_DbConnectionType = connectionType;
            m_SelectCommandType = selectCommandType;
            m_ObjectBrokerType = brokerType;
            m_DbConnectionString = connectionSettings;
            DbFactory.ConnectionType = ConnectionType.Name;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionInfo"/> class.
        /// </summary>
        /// <param name="connectionType">Type of the connection.</param>
        /// <param name="connectionSettings">The connection settings.</param>
        public ConnectionInfo(Type connectionType, string connectionSettings)
        {
            switch (connectionType.Name)
            {
                case ("SqlConnection"):
                    {
                        this.Initialize(connectionType, typeof(System.Data.SqlClient.SqlCommand), typeof(DataObjectBroker), connectionSettings);
                        break;
                    }
                case ("OracleConnection"):
                    {
                        this.Initialize(connectionType, typeof(Oracle.DataAccess.Client.OracleCommand), typeof(DataObjectBroker), connectionSettings);
                        break;
                    }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionInfo"/> class.
        /// </summary>
        /// <param name="connectionType">Type of the connection.</param>
        /// <param name="selectCommandType">Type of the select command.</param>
        /// <param name="brokerType">Type of the broker.</param>
        /// <param name="connectionSettings">The connection settings.</param>
        public ConnectionInfo(Type connectionType, Type selectCommandType, Type brokerType, string connectionSettings)
        {
            this.Initialize(connectionType, selectCommandType, brokerType, connectionSettings);
        }
        #endregion
    }
}