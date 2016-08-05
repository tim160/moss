using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Xml;
using EC.Framework.Dynamic;
using EC.Framework.Logger;

namespace EC.Framework.Data
{
    /// <summary>
    /// Provides an abstraction of the persistent data store through  
    /// which persistent objects and other data can be exchanged.
    /// </summary>
    public class DataObjectBroker : IObjectBroker, IDisposable
    {
        #region Property(s)
        public const string DefaultGenDLAssemblyName = "EC.Framework.Data.GenDL.{0}.dll";
        private string m_GenDLAssemblyName = DefaultGenDLAssemblyName;
        private static readonly ICustomLog m_Log = CustomLogManager.GetLogger("FrameworkData");
        private Dictionary<string, SelectPredicateAttribute> m_PredicateDictionary = new Dictionary<string, SelectPredicateAttribute>();
        private List<IDbCommand> m_CommandsToDispose = new List<IDbCommand>(); 

        /// <summary>
        /// Provides configuration data about the connected to the
        /// data store used by the broker.
        /// </summary>
        /// <value>The connection info.</value>
        private IConnectionInfo m_ConnectionInfo;
        public IConnectionInfo ConnectionInfo
        {
            get { return m_ConnectionInfo; }
            set { m_ConnectionInfo = value; }
        }

        /// <summary>
        /// Provides a connection between the broker and the back end
        /// data store.
        /// </summary>
        /// <value>The db connection.</value>
        private IDbConnection m_DbConnection;
        public IDbConnection DbConnection
        {
            get { return GetConnection(); }
            set { m_DbConnection = value; }
        }

        /// <summary>
        /// Indicates the transaction object currently used by 
        /// commands maintained by the DataObjectBroker.  
        /// </summary>
        /// <value>The transaction.</value>
        private IDbTransaction m_Transaction;
        private IDbTransaction Transaction
        {
            get { return m_Transaction; }
            set
            {
                m_Transaction = value;
                // Capture the thread that initiated the transaction.
                if (m_Transaction == null)
                {
                    TransactionThread = null;
                }
                else
                {
                    TransactionThread = System.Threading.Thread.CurrentThread;
                }
            }
        }

        /// <summary>
        /// Indicates the thread that initiated a transaction.  This value
        /// is maintained to ensure that the thread context that ends
        /// a transaction matches the one that began the transaction.
        /// </summary>
        /// <value>The transaction thread.</value>
        private Thread m_TransactionThread;
        private Thread TransactionThread
        {
            get { return m_TransactionThread; }
            set { m_TransactionThread = value; }
        }

        /// <summary>
        /// Time (in seconds) in which a command submitted to the data
        /// store will timeout (default time is 30 seconds).  Each
        /// command created or submitted by the DataObjectBroker
        /// instance will utilize this value.
        /// </summary>
        /// <value>The command timeout.</value>
        private int m_CommandTimeout = 30;
        public int CommandTimeout
        {
            get { return m_CommandTimeout; }
            set { m_CommandTimeout = value; }
        }

        /// <summary>
        /// Indicates whether or not the DataObjectBroker calls
        /// AcceptChanges on the PersistableData property of IPersistable
        /// objects after they are saved.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [broker accepts changes]; otherwise, <c>false</c>.
        /// </value>
        private bool m_BrokerAcceptsChanges = true;
        public bool BrokerAcceptsChanges
        {
            get { return m_BrokerAcceptsChanges; }
            set { m_BrokerAcceptsChanges = value; }
        }

        /// <summary>
        /// Gets the name of the database.
        /// </summary>
        /// <value>The name of the database.</value>
        public string DatabaseName
        {
            get { return ConnectionInfo.GetDatabaseName(DbConnection); }
        }

        public string ServerName
        {
            get { return ConnectionInfo.GetServerName(DbConnection); }
        }

        public string UserID
        {
            get
            {
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(ConnectionInfo.ConnectionString);
                return builder.UserID;
            }
        }

        public string Password
        {
            get
            {
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(ConnectionInfo.ConnectionString);
                return builder.Password;
            }
        }

        #endregion

        #region Constructor(s)
        /// <summary>
        /// Default constructor; simply initializes the broker with a new
        /// persistable type dictionary, a data structure which will hold
        /// mapping metadata between persistable business object types and the
        /// data store.
        /// </summary>
        /// <param name="info">The info.</param>
        public DataObjectBroker(IConnectionInfo info)
        {
            m_ConnectionInfo = info;
        }

        public DataObjectBroker(IConnectionInfo info, string GenDLAssemblyName)
        {
            m_ConnectionInfo = info;
            m_GenDLAssemblyName = GenDLAssemblyName;
        }
        #endregion

        #region Method(s)
        /// <summary>
        /// Obtains a connection to the data store.  This connection
        /// will be created according to settings on the ConnectionInfo
        /// property of the object broker.
        /// </summary>
        /// <returns></returns>
        [SecurityPermissionAttribute(SecurityAction.Demand, Infrastructure = true)]
        public IDbConnection GetConnection()
        {
            // If the connection has not yet been established, attempt to create and open a connection
            try
            {
                if (m_DbConnection == null)
                {
                    m_DbConnection = ConnectionInfo.CreateConnection();
                }
                OpenConnection();
            }
            // Handle any exceptions that may have occurred during connection initialization
            catch
            {
                m_DbConnection = null;
                throw;
            }
            // If successful, return the open connection
            return m_DbConnection;
        }

        /// <summary>
        /// Ensures that the broker's connection object to the  
        /// data store is open.
        /// </summary>
        private void OpenConnection()
        {
            if ((m_DbConnection != null) && (m_DbConnection.State != ConnectionState.Open))
            {
                m_DbConnection.Open();

//#if (DEBUG)
//                OpenCalls++;
//#endif
            }
            else
            {
                if (m_DbConnection == null)
                {
//#if (DEBUG)
//                    m_Log.Debug("OpenConnection()");
//                    m_Log.Error("<<< Connection is NULL!  Can't open.");
//#endif
                }
            }
        }

        /// <summary>
        /// Closes the broker's connection to the data store.
        /// </summary>
        public void CloseConnection()
        {
            if ((m_DbConnection != null) && (m_DbConnection.State == ConnectionState.Open))
            {
                m_DbConnection.Close();
                m_DbConnection.Dispose();
                m_DbConnection = null;
//#if (DEBUG)
//                CloseCalls++;
//#endif
            }
            else
            {
                if (m_DbConnection == null)
                {
//#if (DEBUG)
//                    m_Log.Debug("CloseConnection()");
//                    m_Log.Error("<<< Connection is NULL!  Can't close.");
//#endif
                }
            }
        }

        /// <summary>
        /// Creates a command object based on the broker's connection
        /// object and the supplied sql statement string.
        /// </summary>
        /// <param name="sql">The SQL string.</param>
        /// <returns></returns>
        [SecurityPermissionAttribute(SecurityAction.Demand, Infrastructure = true)]
        public IDbCommand GetCommand(string sql)
        {
            sql = CleanSqlStmt(sql);

//#if (DEBUG)
//            m_Log.Debug(sql);
//#endif
            this.OpenConnection();
            IDbCommand command = m_ConnectionInfo.GetCommand();
            command.Connection = DbConnection;
            command.CommandText = sql;

            // Assign the broker's transaction context to the new command.
            command.Transaction = this.m_Transaction;
            command.CommandTimeout = this.CommandTimeout;
            return command;
        }

        [SecurityPermissionAttribute(SecurityAction.Demand, Infrastructure = true)]
        public IDbCommand GetCommand()
        {
            this.OpenConnection();
            IDbCommand command = m_ConnectionInfo.GetCommand();
            command.Connection = DbConnection;

            // Assign the broker's transaction context to the new command.
            command.Transaction = this.m_Transaction;
            command.CommandTimeout = this.CommandTimeout;
            return command;
        }
        #endregion

        #region Delete
        /// <summary>
        /// Deletes data of the specified IPersistable data type from the data store
        /// based on the given filter criteria.   
        /// </summary>
        /// <param name="persistentType">Type of the persistent.</param>
        /// <returns></returns>
        public int DeleteDirect(Type type)
        {
            string sql = ConnectionInfo.GetDeleteStmt(string.Empty, AppInfo.GetTableName(type), string.Empty);

            try
            {
                using (IDbCommand command = GetCommand(sql))
                {
                    // Execute the delete command, returning the number of rows affected.
                    return command.ExecuteNonQuery();
                }
            }
            catch { throw; }
        }

        /// <summary>
        /// Deletes data of the specified IPersistable data type from the data store
        /// based on the given filter criteria.   
        /// </summary>
        /// <param name="persistentType"></param>
        /// <param name="filterCriteria"></param>
        /// <returns></returns>
        public int DeleteDirect(Type persistentType, FilterCriteria filterCriteria)
        {
            string persistentTypeName = AppInfo.GetTableName(persistentType);
            string predicateString = string.Empty;
            if (m_PredicateDictionary.ContainsKey(persistentTypeName))
            {
                predicateString = m_PredicateDictionary[persistentTypeName].GetSqlPredicateStatement(ConnectionInfo);
            }

            // Mutate Sql filter
            SqlFilterCriteria sqlFilterCriteria = null;
            if (filterCriteria != null)
            {
                sqlFilterCriteria = MutateFilterCriteria(filterCriteria);
            }
            string filterString = ((sqlFilterCriteria == null) ? String.Empty : sqlFilterCriteria.WhereClauseForParameters);

            // Create a delete string.
            string sql = ConnectionInfo.GetDeleteStmt(predicateString, persistentTypeName, filterString);

            using (IDbCommand command = GetCommand(sql))
            {
                if (sqlFilterCriteria != null)
                {
                    sqlFilterCriteria.AddParameters(command);
                }
                // Execute the delete command, returning the number of rows affected.
                return command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Deletes the persistable.
        /// </summary>
        /// <param name="persistable">The persistable.</param>
        /// <returns></returns>
        private int DeletePersistable(object entity)
        {
            Type type = entity.GetType();
            string entityName = AppInfo.GetTableName(type);
            this.OpenConnection();
            IDbCommand command = ConnectionInfo.GetCommand();
            command.Connection = this.m_DbConnection;
            command.Transaction = this.m_Transaction;
            command.CommandTimeout = this.m_CommandTimeout;
            command = GenDLFactory.GetInstance(string.Format(m_GenDLAssemblyName, ConnectionInfo.ConnectionType)).GetParameters(command, "CFD_" + entityName, entity);

            try
            {
                // Execute the insert command, returning the number of rows affected.
                return command.ExecuteNonQuery();
            }
            catch { throw; }
        }
        #endregion Delete

        #region Save
        /// <summary>
        /// Synchronizes business objects in the supplied persistable collection  
        /// with the data store.
        /// </summary>
        /// <param name="persistables">The persistable collection.</param>
        protected void Save(object[] persistables, bool resetPersistables)
        {
            if (persistables == null)
            {
                throw new NoNullAllowedException();
            }
//#if (DEBUG)
//            m_Log.DebugFormat("____Connection report Save: Open calls: {0}, Close calls: {1}, Balance: {2}", OpenCalls, CloseCalls, ConnectionBalance);
//#endif

            if (m_PredicateDictionary.ContainsKey(persistables.GetType().Name.Replace("[]", "")))
            {
                throw new InvalidOperationException(String.Format("Cannot save collection {0}; collected type {1} has a select predicate which makes it a read-only business object type.",
                    persistables.GetType().Name,
                    persistables[0].GetType().Name));
            }
            BulkSave(persistables);
            // set all entites
            if (resetPersistables)
            { 
                ResetObjectStates(persistables);
            }
        }

        public virtual void Save(object[] persistables)
        {
            Save(persistables, true);
        }

        protected void ResetObjectStates(object[] persistables)
        {
            foreach (object entity in persistables)
            {
                Type persistentType = entity.GetType();
                GetHandler getHandler = DynamicMethodCompiler.CreateGetHandler(persistentType, persistentType.GetProperty("ObjectState"));
                DataAction dataAction = (DataAction)getHandler(entity);
                if (dataAction != DataAction.Delete)
                {
                    SetHandler setHandler = DynamicMethodCompiler.CreateSetHandler(persistentType, persistentType.GetProperty("ObjectState"));
                    setHandler(entity, DataAction.None);
                }
            }
        }

        /// <summary>
        /// Uses EMS Bulk Sync infrastructure to synchronize a business
        /// object collection against a data store.
        /// </summary>
        /// <param name="persistables">The persistable collection.</param>
        private void BulkSave(object[] entities)
        {
            for (int i = 0; i < entities.Length; i++)
            {
                // each entity can be a different type
                Type persistentType = entities[i].GetType();
                // Create Handler
                GetHandler getHandler = DynamicMethodCompiler.CreateGetHandler(persistentType, persistentType.GetProperty("ObjectState"));

                if ((DataAction)getHandler(entities[i]) == DataAction.Insert)
                {
                    InsertEntity(entities[i]);
                }
                else if ((DataAction)getHandler(entities[i]) == DataAction.Update)
                {
                    UpdatePersistable(entities[i]);
                }
                else if ((DataAction)getHandler(entities[i]) == DataAction.Delete)
                {
                    DeletePersistable(entities[i]);
                }
            }
        }

        /// <summary>
        /// Deeps the save.
        /// </summary>
        /// <param name="persistables">The persistables.</param>
        public virtual void DeepSave(object[] persistables)
        {
            if (persistables == null)
            {
                throw new NoNullAllowedException();
            }
            for (int i = 0; i < persistables.Length; i++)
            {
                object entity = persistables[i];
                List<object> tree = new List<object>();
                Extract(new object[1] { persistables[i] }, ref tree);
                Type persistentType = persistables[i].GetType();
                GetHandler getHandler = DynamicMethodCompiler.CreateGetHandler(persistentType, persistentType.GetProperty("ObjectState"));
                if ((DataAction)getHandler(persistables[i]) == DataAction.Delete)
                {
                    for (int j = 0; j < tree.Count; j++)
                    {
                        if (j != 0)
                        {
                            Type type = tree[j].GetType();
                            type.GetProperty("ObjectState").SetValue(tree[j], DataAction.Delete, BindingFlags.SetProperty, null, null, Thread.CurrentThread.CurrentCulture);
                        }
                    }
                    tree.Reverse();
                }
                Save(tree.ToArray());
            }
        }

        private void Extract(object[] entities, ref List<object> tree)
        {
            if (entities != null)
            {
                for (int i = 0; i < entities.Length; i++)
                {
                    if (entities[i] != null)
                    {
                        tree.Add(entities[i]);
                    }
                    Type type = entities[i].GetType();
                    PropertyInfo[] propertyInfo = AppInfo.GetPropertiesInfo(type);
                    for (int j = 0; j < propertyInfo.Length; j++)
                    {
                        if (propertyInfo[j].GetCustomAttributes(typeof(ChildAttribute), false).Length == 1)
                        {
                            Extract(type.GetProperty(propertyInfo[j].Name).GetValue(entities[i], null) as object[], ref  tree);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Inserts the persistable.
        /// </summary>
        /// <param name="persistable">The persistable.</param>
        /// <returns></returns>
        private int InsertEntity(object entity)
        {
            Type type = entity.GetType();
            string entityName = AppInfo.GetTableName(type);
            this.OpenConnection();
            IDbCommand command = null;

            try
            {
                command = ConnectionInfo.GetCommand();
                command.Connection = this.m_DbConnection;
                command.Transaction = this.m_Transaction;
                command.CommandTimeout = this.m_CommandTimeout;
                command = GenDLFactory.GetInstance(string.Format(m_GenDLAssemblyName, ConnectionInfo.ConnectionType)).GetParameters(command, "CFI_" + entityName, entity);
                // Execute the insert command, returning the number of rows affected.
                int rowsAffected = command.ExecuteNonQuery();
                if (!command.CommandText.Contains(string.Format("({0}Id,", ConnectionInfo.ParameterToken))
                    && !command.CommandText.Contains(string.Format("({0}pId,", ConnectionInfo.ParameterToken)))
                {
                    Int64 id = ConnectionInfo.GetId(command, entityName);
                    SetHandler setHandler = DynamicMethodCompiler.CreateSetHandler(type, type.GetProperty("Id"));
                    setHandler(entity, id);
                }
                return rowsAffected;
            }
            catch { throw; }
            finally
            {
                if (command != null)
                {
                    command.Dispose();
                }
            }
        }

        /// <summary>
        /// Updates the persistable.
        /// </summary>
        /// <param name="persistable">The persistable.</param>
        /// <returns></returns>
        private int UpdatePersistable(object entity)
        {
            Type type = entity.GetType();
            string entityName = AppInfo.GetTableName(type);
            this.OpenConnection();
            IDbCommand command = null;

            try
            {
                command = ConnectionInfo.GetCommand();
                command.Connection = this.m_DbConnection;
                command.Transaction = this.m_Transaction;
                command.CommandTimeout = this.m_CommandTimeout;
                command = GenDLFactory.GetInstance(string.Format(m_GenDLAssemblyName, ConnectionInfo.ConnectionType)).GetParameters(command, "CFU_" + entityName, entity);
                // Execute the insert command, returning the number of rows affected.
                return command.ExecuteNonQuery();
            }
            catch { throw; }
            finally
            {
                if (command != null)
                {
                    command.Dispose();
                }
            }
        }

        /// <summary>
        /// Updates the direct.
        /// </summary>
        /// <param name="persistentType">Type of the persistent.</param>
        public virtual int UpdateDirect(Type persistentType, Dictionary<string, object> updateDictionary, FilterCriteria filterCriteria)
        {
//#if (DEBUG)
//            m_Log.DebugFormat("____Connection report UpdateDirectView: Open calls: {0}, Close calls: {1}, Balance: {2}", OpenCalls, CloseCalls, ConnectionBalance);
//#endif
            updateDictionary["Timestamp"] = DateTime.UtcNow;

            if (!AppInfo.IsView(persistentType))
            {
                return UpdateDirectTable(persistentType, updateDictionary, filterCriteria);
            }
            else
            {
                return UpdateDirectView(persistentType, updateDictionary, filterCriteria);
            }
        }

        private int UpdateDirectTable(Type persistentType, Dictionary<string, object> updateDictionary, FilterCriteria filterCriteria)
        {
            string persistentTypeName = AppInfo.GetTableName(persistentType);

            // Mutate Sql Filter
            SqlFilterCriteria sqlFilterCriteria = null;
            if (filterCriteria != null)
            {
                sqlFilterCriteria = MutateFilterCriteria(filterCriteria);
            }
            StringBuilder setString = new StringBuilder();
            foreach (string key in updateDictionary.Keys)
            {
                setString.AppendFormat("{0} = {1}{0},", key, ConnectionInfo.ParameterToken);
            }
            string updateString = String.Format("UPDATE {0} SET {1} {2}",
                persistentTypeName,
                setString.ToString().Substring(0, setString.Length - 1),
                (sqlFilterCriteria == null) ? String.Empty : sqlFilterCriteria.WhereClauseForParameters);

            IDbCommand command = null;

            try
            {
                command = GetCommand(updateString);
                command = AddParameters(command, updateDictionary, persistentType);
                if (sqlFilterCriteria != null)
                {
                    sqlFilterCriteria.AddParameters(command);
                }
                // Execute the delete command, returning the number of rows affected.
                return command.ExecuteNonQuery();
            }
            catch { throw; }
            finally
            {
                if (command != null)
                {
                    if (!TransactionStarted)
                    {
                        command.Dispose();
                    }
                    else
                    {
                        m_CommandsToDispose.Add(command);
                    }
                }

            }
        }

        private int UpdateDirectView(Type persistentType, Dictionary<string, object> updateDictionary, FilterCriteria filterCriteria)
        {
            string persistentTypeName = AppInfo.GetTableName(persistentType);

            int i = 0;
            foreach (string key in updateDictionary.Keys)
            {
                // Mutate Sql Filter
                SqlFilterCriteria sqlFilterCriteria = null;
                if (filterCriteria != null)
                {
                    sqlFilterCriteria = MutateFilterCriteria(filterCriteria);
                }
                StringBuilder setString = new StringBuilder();
				setString.AppendFormat("{0} = {1}{0}", key, ConnectionInfo.ParameterToken);

                string updateString = String.Format("UPDATE {0} SET {1} {2}",
                    persistentTypeName,
                    setString.ToString(),
                    (sqlFilterCriteria == null) ? String.Empty : sqlFilterCriteria.WhereClauseForParameters);

                IDbCommand command = null;

                try
                {
                    command = GetCommand(updateString);
                    command = AddParameter(command, key, updateDictionary[key], persistentType);
                    if (sqlFilterCriteria != null)
                    {
                        sqlFilterCriteria.AddParameters(command);
                    }
                    // Execute the command, returning the number of rows affected.
                    i = command.ExecuteNonQuery();
                }
                catch { throw; }
                finally
                {
                    if (command != null)
                    {
                        command.Dispose();
                    }
                }
            }

            return i;
        }

        /// <summary>
        /// Adds the parameters.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="updateDictionary">The update dictionary.</param>
        /// <returns></returns>
        private IDbCommand AddParameters(IDbCommand command, Dictionary<string, object> updateDictionary, Type persistentType)
        {
            foreach (string key in updateDictionary.Keys)
            {
                command = AddParameter(command, key, updateDictionary[key], persistentType);
            }
            return command;
        }

        private IDbCommand AddParameter(IDbCommand command, string key, object value, Type persistentType)
        {
            Type propertyType = persistentType.GetProperty(key).PropertyType;
            if (value == null)
            {
                command.Parameters.Add(ConnectionInfo.GetParameter(key, DBNull.Value));
            }
            else if (propertyType == typeof(Guid) || propertyType == typeof(Guid?) || propertyType == typeof(string))
            {
                command.Parameters.Add(ConnectionInfo.GetParameter(key, value.ToString()));
            }
            else if (propertyType == typeof(bool) || propertyType == typeof(bool?))
            {
                string boolVal = "0";
                if (value.ToString() == "1" || value.ToString().ToLower() == bool.TrueString.ToLower())
                {
                    boolVal = "1";
                }
                command.Parameters.Add(ConnectionInfo.GetParameter(key, boolVal));
            }
            else if (propertyType == typeof(DateTime) || propertyType == typeof(DateTime?) || key == "Timestamp")
            {
                if ((DateTime)value != DateTime.MinValue || (DateTime)value != new DateTime(1, 1, 1) || (DateTime)value != new DateTime(1900, 1, 1))
                {
                    command.Parameters.Add(ConnectionInfo.GetParameter(key, value));
                }
                else
                {
                    command.Parameters.Add(ConnectionInfo.GetParameter(key, DateTime.UtcNow));
                }
            }
            else if (propertyType == typeof(XmlDocument) || propertyType == typeof(XmlNode) || propertyType == typeof(XmlElement))
            {
                XmlNode root = value as XmlNode;
                // remove <?xml version="1.0" encoding="utf-16"?>
                if (root != null && root.HasChildNodes)
                {
                    if (root.FirstChild.NodeType == XmlNodeType.XmlDeclaration)
                    {
                        root.RemoveChild(root.FirstChild);
                    }
                }
                // remove xmlns=""
                XmlAttribute attribute = root.FirstChild.Attributes["xmlns"];
                if (attribute != null && string.IsNullOrEmpty(attribute.Value))
                {
                    root.FirstChild.Attributes.Remove(attribute);
                }
                command.Parameters.Add(ConnectionInfo.GetParameter(key, ((XmlNode)root).OuterXml));
            }
            else
            {
                command.Parameters.Add(ConnectionInfo.GetParameter(key, value));
            }
            return command;
        }
        #endregion Save

        #region Load
        /// <summary>
        /// Instantiates a collection of objects from the persistent data
        /// store that are owned by a parent object.  This collection
        /// represents all of the available objects of the requested
        /// type that currently reside in the persistent data
        /// store, are owned by the supplied parent (if any), and meet the
        /// supplied filter criteria.
        /// </summary>
        /// <param name="persistentType">Type of the persistent.</param>
        /// <returns></returns>
        public virtual object[] Load(Type persistentType)
        {
            return Load(persistentType, null, null, true);
        }

        public virtual object[] Load(Type persistentType, bool deepLoad)
        {
            return Load(persistentType, null, null, deepLoad);
        }

        /// <summary>
        /// Instantiates a collection of objects from the persistent data
        /// store that are owned by a parent object.  This collection
        /// represents all of the available objects of the requested
        /// type that currently reside in the persistent data
        /// store, are owned by the supplied parent (if any), and meet the
        /// supplied filter criteria.
        /// </summary>
        /// <param name="persistentType">Type of the persistent.</param>
        /// <param name="filterCriteria">The filter criteria.</param>
        /// <returns></returns>
        public virtual object[] Load(Type persistentType, FilterCriteria filterCriteria)
        {
            return Load(persistentType, filterCriteria, null, true);
        }

        public virtual object[] Load(Type persistentType, FilterCriteria filterCriteria, bool deepLoad)
        {
            return Load(persistentType, filterCriteria, null, deepLoad);
        }

        /// <summary>
        /// Instantiates a collection of objects from the persistent data
        /// store that are owned by a parent object.  This collection
        /// represents all of the available objects of the requested
        /// type that currently reside in the persistent data
        /// store, are owned by the supplied parent (if any), and meet the
        /// supplied filter criteria.
        /// </summary>
        /// <param name="persistentType">Type of the persistent.</param>
        /// <param name="orderCriteria">The order criteria.</param>
        /// <returns></returns>
        public virtual object[] Load(Type persistentType, OrderCriteria orderCriteria)
        {
            return Load(persistentType, null, orderCriteria, true);
        }

        public virtual object[] Load(Type persistentType, OrderCriteria orderCriteria, bool deepLoad)
        {
            return Load(persistentType, null, orderCriteria, deepLoad);
        }

        public virtual object[] Load(Type persistentType, FilterCriteria filterCriteria, OrderCriteria orderCriteria)
        {
            return Load(persistentType, filterCriteria, orderCriteria, true);
        }
        /// <summary>
        /// Loads the specified persistent type.
        /// </summary>
        /// <param name="persistentType">Type of the persistent.</param>
        /// <param name="filterCriteria">The filter criteria.</param>
        /// <param name="orderCriteria">The order criteria.</param>
        /// <param name="groupCriteria">The group criteria.</param>
        /// <returns></returns>
        [SecurityPermissionAttribute(SecurityAction.Demand, Infrastructure = true)]
        public object[] Load(Type persistentType, FilterCriteria filterCriteria, OrderCriteria orderCriteria, bool deepLoad)
        {
//#if (DEBUG)
//            m_Log.DebugFormat("____Connection report Load: Open calls: {0}, Close calls: {1}, Balance: {2}", OpenCalls, CloseCalls, ConnectionBalance);
//#endif

            List<object> objectList = new List<object>();

            try
            {
                using (IDbCommand command = GetCommand())
                {
                    ProcessSelectStmt(command, AppInfo.GetTableName(persistentType), new string[] { }, filterCriteria, orderCriteria, null);
                    objectList = Process(command, persistentType, deepLoad);
                }
            }
            catch { throw; }

            return Transform(persistentType, objectList);
        }

        /// <summary>
        /// Loads the specified persistent type.
        /// </summary>
        /// <param name="persistentType">Type of the persistent.</param>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="filterCriteria">The filter criteria.</param>
        /// <param name="orderCriteria">The order criteria.</param>
        /// <param name="groupCriteria">The group criteria.</param>
        /// <returns></returns>
        public object[] Load(Type persistentType, string columnName, FilterCriteria filterCriteria, OrderCriteria orderCriteria, GroupCriteria groupCriteria)
        {
            object[] objects = new object[0];
            List<object> objectList = new List<object>();

            try
            {
                using (IDbCommand command = GetCommand())
                {
                    ProcessSelectStmt(command, AppInfo.GetTableName(persistentType), new string[] { columnName }, filterCriteria, orderCriteria, groupCriteria);

                    using (IDataReader reader = command.ExecuteReader())
                    {
                        string propertyName = columnName;

                        if (columnName == string.Format("{0}Key", AppInfo.GetTableName(persistentType)))
                            propertyName = "ObjectKey";

                        PropertyInfo propertyInfo = persistentType.GetProperty(propertyName);

                        while (reader.Read())
                        {
                            if (propertyInfo.PropertyType == typeof(String))
                            {
                                object value = reader[columnName];
                                if (value != DBNull.Value)
                                    objectList.Add(reader[columnName] as String);
                            }
                            else if (propertyInfo.PropertyType == typeof(DateTime))
                                objectList.Add((DateTime)reader[columnName]);
                            else if (propertyInfo.PropertyType == typeof(DateTime?))
                            {
                                object value = reader[columnName];
                                if (value != DBNull.Value)
                                    objectList.Add((DateTime?)value);
                            }
                            else if (propertyInfo.PropertyType == typeof(Int16))
                                objectList.Add(Int16.Parse(reader[columnName].ToString()));
                            else if (propertyInfo.PropertyType == typeof(Int16?))
                            {
                                object value = reader[columnName];
                                if (value != DBNull.Value)
                                {
                                    value = Int16.Parse(value.ToString());
                                    objectList.Add((Int16?)value);
                                }
                            }
                            else if (propertyInfo.PropertyType == typeof(Int32))
                                objectList.Add(Int32.Parse(reader[columnName].ToString()));
                            else if (propertyInfo.PropertyType == typeof(Int32?))
                            {
                                object value = reader[columnName];
                                if (value != DBNull.Value)
                                {
                                    value = Int32.Parse(value.ToString());
                                    objectList.Add((Int32?)value);
                                }
                            }
                            else if (propertyInfo.PropertyType == typeof(Int64))
                                objectList.Add(Int64.Parse(reader[columnName].ToString()));
                            else if (propertyInfo.PropertyType == typeof(Int64?))
                            {
                                object value = reader[columnName];
                                if (value != DBNull.Value)
                                {
                                    value = Int64.Parse(value.ToString());
                                    objectList.Add((Int64?)value);
                                }
                            }
                            else if (propertyInfo.PropertyType == typeof(Double))
                                objectList.Add(Double.Parse(reader[columnName].ToString()));
                            else if (propertyInfo.PropertyType == typeof(Double?))
                            {
                                object value = reader[columnName];
                                if (value != DBNull.Value)
                                {
                                    value = Double.Parse(value.ToString());
                                    objectList.Add((Double?)value);
                                }
                            }
                            else if (propertyInfo.PropertyType == typeof(Decimal))
                                objectList.Add(Decimal.Parse(reader[columnName].ToString()));
                            else if (propertyInfo.PropertyType == typeof(Decimal?))
                            {
                                object value = reader[columnName];
                                if (value != DBNull.Value)
                                {
                                    value = Decimal.Parse(value.ToString());
                                    objectList.Add((Decimal?)value);
                                }
                            }
                            else if (propertyInfo.PropertyType == typeof(Byte))
                                objectList.Add((Byte)reader[columnName]);
                            else if (propertyInfo.PropertyType == typeof(Byte?))
                            {
                                object value = reader[columnName];
                                if (value != DBNull.Value)
                                    objectList.Add((Byte?)value);
                            }
                            else if (propertyInfo.PropertyType == typeof(Byte[]))
                                objectList.Add((Byte[])reader[columnName]);
                            else if (propertyInfo.PropertyType == typeof(Guid))
                                objectList.Add(new Guid(reader[columnName].ToString()));
                            else if (propertyInfo.PropertyType == typeof(Guid?))
                            {
                                object value = reader[columnName];
                                if (value != DBNull.Value)
                                {
                                    value = new Guid(value.ToString());
                                    objectList.Add((Guid?)value);
                                }
                            }
                            else if (propertyInfo.PropertyType == typeof(XmlDocument))
                            {
                                XmlDocument xmlDocument = new XmlDocument();
                                xmlDocument.LoadXml((string)reader[columnName]);
                                objectList.Add(xmlDocument);
                            }
                            else if (propertyInfo.PropertyType == typeof(Boolean))
                            {
                                if (reader[columnName].ToString() == "0")
                                    objectList.Add((Boolean)false);
                                else
                                    objectList.Add((Boolean)true);
                            }
                            else if (propertyInfo.PropertyType == typeof(Boolean?))
                            {
                                object value = reader[columnName];
                                if (value != DBNull.Value)
                                {
                                    if (reader[columnName].ToString() == "0")
                                        objectList.Add((Boolean)false);
                                    else
                                        objectList.Add((Boolean)true);
                                }
                            }
                        }
                    }
                }
            }
            catch { throw; }

            objects = objectList.ToArray();

            return objects;
        }

        public long[] LoadObjectIds(Type persistentType, FilterCriteria filterCriteria, OrderCriteria orderCriteria, GroupCriteria groupCriteria)
        {
            List<long> objectList = new List<long>();
            string persistentTypeName = AppInfo.GetTableName(persistentType);
            object id = null;

            try
            {
                using (IDbCommand command = GetCommand())
                {
                    ProcessSelectStmt(command, persistentTypeName, new string[] { "Id" }, filterCriteria, orderCriteria, groupCriteria);

                    using (IDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            id = reader["Id"];
                            objectList.Add(Convert.ToInt64(reader["Id"]));                            
                        }
                    }
                }
            }
            catch {
                m_Log.Error(string.Format("LoadObjectIds() - Id: {0}", id == null || id == DBNull.Value ? string.Empty:id.ToString()));                
                throw; 
            }

            return objectList.ToArray(); 
        }
        /// <summary>
        /// Loads the object keys.
        /// </summary>
        /// <param name="persistentType">Type of the persistent.</param>
        /// <param name="filterCriteria">The filter criteria.</param>
        /// <param name="orderCriteria">The order criteria.</param>
        /// <param name="groupCriteria">The group criteria.</param>
        /// <returns></returns>
        public Guid[] LoadObjectKeys(Type persistentType, FilterCriteria filterCriteria, OrderCriteria orderCriteria, GroupCriteria groupCriteria)
        {
            Guid[] objects = new Guid[0];
            List<Guid> objectList = new List<Guid>();
            string persistentTypeName = AppInfo.GetTableName(persistentType);

            try
            {
                using (IDbCommand command = GetCommand())
                {
                    ProcessSelectStmt(command, persistentTypeName, new string[] { "ObjectKey" }, filterCriteria, orderCriteria, groupCriteria);

                    using (IDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            objectList.Add(new Guid(reader[persistentTypeName + "Key"].ToString()));
                        }
                    }
                }
            }
            catch { throw; }

            objects = objectList.ToArray();

            return objects;
        }
        #endregion Load

        #region Mutating
        /// <summary>
        /// Converts a FilterCriteria to a SqlFilterCriteria.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns></returns>
        public SqlFilterCriteria MutateFilterCriteria(FilterCriteria criteria)
        {
            SqlFilterCriteria sqlCriteria = null;
            if (!(criteria is SqlFilterCriteria))
            {
                sqlCriteria = new SqlFilterCriteria(this);
                sqlCriteria.Criteria = criteria.Criteria.Clone();
            }
            else
                sqlCriteria = (SqlFilterCriteria)criteria;
            return sqlCriteria;
        }

        /// <summary>
        /// Converts a FilterCriteria to a SqlFilterCriteria and uses the specified
        /// persistentTypeName as the name instead of the default table name.
        /// </summary>
        /// <param name="orderCriteria"></param>
        /// <param name="reverse"></param>
        /// <param name="persistentTypeName"></param>
        /// <returns></returns>
        public SqlOrderByCriteria MutateOrderByCriteria(OrderCriteria orderCriteria, bool reverse, string persistentTypeName)
        {
            SqlOrderByCriteria sqlOrderByCriteria = null;
            if (orderCriteria is SqlOrderByCriteria)
            {
                sqlOrderByCriteria = (SqlOrderByCriteria)orderCriteria;
            }
            else
            {
                sqlOrderByCriteria = new SqlOrderByCriteria(orderCriteria, this);
            }
            sqlOrderByCriteria.PersistentTypeName = persistentTypeName;
            sqlOrderByCriteria.Reverse = reverse;
            return sqlOrderByCriteria;
        }

        /// <summary>
        /// Converts a OrderCriteria to a SqlOrderByCriteria.
        /// </summary>
        /// <param name="orderCriteria">The order criteria.</param>
        /// <returns></returns>
        public SqlOrderByCriteria MutateOrderByCriteria(OrderCriteria orderCriteria)
        {
            return MutateOrderByCriteria(orderCriteria, false, null);
        }

        /// <summary>
        /// Converts a OrderCriteria to a SqlOrderByCriteria and uses the specified
        /// persistentTypeName as the name instead of the default table name.
        /// </summary>
        /// <param name="orderCriteria">The order criteria.</param>
        /// <param name="persistentTypeName">The name to use as the persistentTypeName.  Uses table name if not specified.</param>
        /// <returns></returns>
        public SqlOrderByCriteria MutateOrderByCriteria(OrderCriteria orderCriteria, string persistentTypeName)
        {
            return MutateOrderByCriteria(orderCriteria, false, persistentTypeName);
        }

        private SqlGroupByCriteria MutateGroupByCriteria(GroupCriteria groupCriteria)
        {
            SqlGroupByCriteria sqlGroupByCriteria = null;
            if (groupCriteria is SqlGroupByCriteria)
                sqlGroupByCriteria = (SqlGroupByCriteria)groupCriteria;
            else
                sqlGroupByCriteria = new SqlGroupByCriteria(groupCriteria, this);
            return sqlGroupByCriteria;
        }
        #endregion

        #region Transactions
        /// <summary>
        /// Initiates a new transaction for the object broker.  All
        /// business object collections which are persisted through
        /// the broker for the duration of the transaction will be
        /// handled atomically.
        /// </summary>
        public void StartTransaction()
        {
            StartTransaction(IsolationLevel.ReadCommitted);
        }

        public bool TransactionStarted
        {
            get
            {
                return Transaction != null; 
            }
        }

        /// <summary>
        /// Begins a transaction on the connection maintained by the
        /// current instance of DataObjectBroker.
        /// </summary>
        /// <param name="isolationLevel">The isolation level.</param>
        public void StartTransaction(System.Data.IsolationLevel isolationLevel)
        {
            if (this.Transaction != null)
                throw new InvalidOperationException("A transaction is currently open on this data broker.");

            Transaction = DbConnection.BeginTransaction(isolationLevel);
            //if (this.ConnectionType == typeof(SqlConnection))
            //    this.Transaction = ((SqlConnection)DbConnection).BeginTransaction(isolationLevel);
            //else if (this.ConnectionType == typeof(OracleConnection))
            //    this.Transaction = ((OracleConnection)DbConnection).BeginTransaction(isolationLevel);
        }

        /// <summary>
        /// Creates a named transaction savepoint to which further
        /// changes to the data store can be rolled back.  After
        /// rolling back a transaction to a savepoint, further
        /// commands are necessary to either Rollback the entire
        /// transaction or Commit to the initial Begin point.
        /// </summary>
        /// <param name="savePointName">Name of the save point.</param>
        public void SaveTransaction(string savePointName)
        {
            if ((this.Transaction == null) || (this.Transaction.GetType() != typeof(SqlTransaction)))
                throw new InvalidOperationException("Savepoint can only be set on a valid SqlTransaction object.");

            ConnectionInfo.SaveTransaction(Transaction, savePointName);
        }

        /// <summary>
        /// Commits a pending transaction on the DataObjectBroker's
        /// connection to the data store.  If a transaction is not currently
        /// pending, an exception will be thrown.  In
        /// addition, if RollbackTransaction was called on a seperate
        /// thread than the one that called StartTransaction, an
        /// exception is thrown.
        /// </summary>
        public void EndTransaction()
        {
            VerifyTransactionState();
            this.Transaction.Commit();
            Transaction = null;
        }

        /// <summary>
        /// Ensures that the thread that is attempting to end a transaction
        /// is the same as the one that initiated it.  If not, an InvalidOperationException
        /// is thrown.
        /// </summary>
        private void VerifyTransactionState()
        {
            // If the Transaction property is null, thrown an exception.
            if (Transaction == null)
                throw new InvalidOperationException("No transaction is currently pending on this instance of DataObjectBroker.");

            // Obtain the current calling thread.
            Thread currentThread = Thread.CurrentThread;

            // If the calling thread and the transaction initiating thread are 
            // not the same, throw an exception.
            if (currentThread.GetHashCode() != TransactionThread.GetHashCode())
                throw new InvalidOperationException("A transaction must be ended by the same thread that initiated it.");
        }

        /// <summary>
        /// Rolls back all data store changes that have been submitted
        /// since a transaction was initiated with a call to DataObjectBroker.StartTransaction().
        /// If there is no pending transaction, an exception is thrown.  In
        /// addition, if RollbackTransaction was called on a seperate
        /// thread than the one that called StartTransaction, an
        /// exception is thrown.
        /// </summary>
        public void RollbackTransaction()
        {
            VerifyTransactionState();
            Transaction.Rollback();
            Transaction = null;
        }

        /// <summary>
        /// Rolls back all data store changes that have been submitted
        /// since a transaction was initiated with a call to DataObjectBroker.StartTransaction().
        /// If there is no pending transaction, an exception is thrown.  In
        /// addition, if RollbackTransaction was called on a seperate
        /// thread than the one that called StartTransaction, an
        /// exception is thrown.
        /// </summary>
        /// <param name="savePointName">Name of the save point.</param>
        public void RollbackTransaction(string savePointName)
        {
            VerifyTransactionState();
            if (this.Transaction.GetType() != typeof(SqlTransaction))
                throw new InvalidOperationException("Only SqlTransaction objects allow named rollbacks.");
            ((SqlTransaction)Transaction).Rollback(savePointName);
        }
        #endregion

        #region Predicates
        /// <summary>
        /// Sets the type select predicate.
        /// </summary>
        /// <param name="persistentType">Type of the persistent.</param>
        /// <param name="selectPredicate">The select predicate.</param>
        public void SetTypeSelectPredicate(Type persistentType, SelectPredicateAttribute selectPredicate)
        {
            string persistentTypeName = AppInfo.GetTableName(persistentType);
            if (selectPredicate == null)
            {
                if (m_PredicateDictionary.ContainsKey(persistentTypeName))
                    m_PredicateDictionary.Remove(persistentTypeName);
            }
            else
            {
                if (!m_PredicateDictionary.ContainsKey(persistentTypeName))
                    m_PredicateDictionary.Add(persistentTypeName, selectPredicate);
                else
                    m_PredicateDictionary[persistentTypeName] = selectPredicate;
            }
        }
        #endregion

        #region GetCount
        /// <summary>
        /// Gets the count of a business object.
        /// </summary>
        /// <param name="persistableType">Type of the persistable.</param>
        /// <returns></returns>
        public long GetObjectCount(Type type)
        {
            return GetObjectCount(type, new string[0], null);
        }

        public long GetObjectCount(Type type, FilterCriteria filterCriteria)
        {
            return GetObjectCount(type, new string[0], filterCriteria);
        }

        public long GetObjectCount(Type type, string[] fields, FilterCriteria filterCriteria)
        {
//#if (DEBUG)
//            m_Log.DebugFormat("____Connection report GetObjectCount: Open calls: {0}, Close calls: {1}, Balance: {2}", OpenCalls, CloseCalls, ConnectionBalance);
//#endif
            long objectCount = 0;
            // Filter
            SqlFilterCriteria sqlFilterCriteria = null;
            if (filterCriteria != null && filterCriteria.Criteria.Criteria.Count > 0)
            {
                sqlFilterCriteria = MutateFilterCriteria(filterCriteria);
            }
            string filterString = ((sqlFilterCriteria == null) ? String.Empty : sqlFilterCriteria.WhereClauseForParameters);
            // Predicate
            string predicateString = string.Empty;
            if (m_PredicateDictionary.ContainsKey(AppInfo.GetTableName(type)))
            {
                predicateString = m_PredicateDictionary[AppInfo.GetTableName(type)].GetSqlPredicateStatement(ConnectionInfo);
            }
            // Columns
            string columnList = string.Empty;
            foreach (string field in fields)
            {
                columnList += string.Format("{0},", field);
            }
            if (fields.Length > 0)
            {
                columnList = columnList.Substring(0, columnList.Length - 1);
                columnList = columnList.Replace("ObjectKey", AppInfo.GetTableName(type) + "Key");
            }
            if (columnList == string.Empty)
            {
                PropertyInfo objectKeyProperty = type.GetProperty("ObjectKey");
                if (objectKeyProperty != null && objectKeyProperty.GetCustomAttributes(typeof(NotPersistableAttribute), true).Length == 0)
                {
                    columnList = string.Format("{0}Key", AppInfo.GetTableName(type));
                }
                else
                {
                    columnList = "Id";
                }
            }

            string sql = string.Format("SELECT COUNT({0} {1}) FROM {2} {3}", predicateString, columnList, AppInfo.GetTableName(type), filterString);

            try
            {
                using (IDbCommand command = GetCommand(sql.ToString()))
                {
                    if (sqlFilterCriteria != null)
                    {
                        sqlFilterCriteria.AddParameters(command);
                    }
                    objectCount = Convert.ToInt64(command.ExecuteScalar());
                }
            }
            catch { throw; }

            return objectCount;
        }
        #endregion GetCount

        #region CustomLoadByRowNumber
        public object[] CustomLoadByRowNumber(Type persistentType, FilterCriteria filterCriteria, OrderCriteria orderCriteria, int maxRows, long startId)
        {
            return CustomLoadByRowNumber(persistentType, filterCriteria, string.Empty, orderCriteria, maxRows, startId);
        }

        public object[] CustomLoadByRowNumber(Type persistentType, FilterCriteria filterCriteria, string extraFilter, OrderCriteria orderCriteria, int maxRows, long startId)
        {
            List<object> objectList = new List<object>();
            string persistentTypeName = AppInfo.GetTableName(persistentType);

            SqlFilterCriteria sqlFilterCriteria = null;
            SqlOrderByCriteria sqlOrderByCriteria = null;

            // If a filter was passed in, mutate it to the type SqlFilterCriteria.
            if (filterCriteria != null)
                sqlFilterCriteria = MutateFilterCriteria(filterCriteria);
            if (orderCriteria != null)
                sqlOrderByCriteria = MutateOrderByCriteria(orderCriteria);

            string filterString = ((sqlFilterCriteria == null) ? String.Empty : sqlFilterCriteria.WhereClauseForParameters);
            string orderString = ((sqlOrderByCriteria == null) ? "ORDER BY Id ASC" : sqlOrderByCriteria.OrderByClause);

            if (!string.IsNullOrEmpty(extraFilter))
            {
                if (!string.IsNullOrEmpty(filterString))
                {
                    filterString += " AND " + extraFilter;
                }
                else
                {
                    filterString = " WHERE " + extraFilter;
                }
            }

            string sql = ConnectionInfo.GetSelectByRowNumStmt(persistentType, startId, maxRows, persistentTypeName, filterString, orderString);

            try
            {
                using (IDbCommand command = GetCommand(sql))
                {
                    if (sqlFilterCriteria != null)
                    {
                        sqlFilterCriteria.AddParameters(command);
                    }
                    objectList = Process(command, persistentType, true);
                }
            }
            catch { throw; }

            return Transform(persistentType, objectList);
        }
        #endregion CustomLoadByRowNumber


        public object[] GetObjectProperty<T>(Type persistentType, string propertyName, FilterCriteria filterCriteria)
        {
            return GetObjectProperty<T>(persistentType, propertyName, filterCriteria, null, null);
        }
        public object[] GetObjectProperty<T>(Type persistentType, string propertyName, FilterCriteria filterCriteria, OrderCriteria orderCriteria, GroupCriteria groupCriteria)
        {
            List<object> objectList = new List<object>();
            string persistentTypeName = AppInfo.GetTableName(persistentType);
            try
            {
                using (IDbCommand command = GetCommand())
                {
                    ProcessSelectStmt(command, persistentTypeName, new string[] { propertyName }, filterCriteria, orderCriteria, groupCriteria);

                    using (IDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (typeof(T) == typeof(String) ||
                                typeof(T) == typeof(DateTime) ||
                                typeof(T) == typeof(DateTime?) ||
                                typeof(T) == typeof(Byte) ||
                                typeof(T) == typeof(Byte[]) )
                            {
                                object value = reader[propertyName];
                                if (value != DBNull.Value)
                                {
                                    objectList.Add((T)reader[propertyName]);
                                }
                            }
                            else if (typeof(T) == typeof(Int16))
                            {

                                objectList.Add(Int16.Parse(reader[propertyName].ToString()));
                            }
                            else if (typeof(T) == typeof(Int16?))
                            {
                                object value = reader[propertyName];
                                if (value != DBNull.Value)
                                {
                                    value = Int16.Parse(value.ToString());
                                    objectList.Add((Int16?)value);
                                }
                            }
                            else if (typeof(T) == typeof(Int32))
                            {
                                objectList.Add(Int32.Parse(reader[propertyName].ToString()));
                            }
                            else if (typeof(T) == typeof(Int32?))
                            {
                                object value = reader[propertyName];
                                if (value != DBNull.Value)
                                {
                                    value = Int32.Parse(value.ToString());
                                    objectList.Add((Int32?)value);
                                }
                            }
                            else if (typeof(T) == typeof(Int64))
                            {
                                objectList.Add(Int64.Parse(reader[propertyName].ToString()));
                            }
                            else if (typeof(T) == typeof(Int64?))
                            {
                                object value = reader[propertyName];
                                if (value != DBNull.Value)
                                {
                                    value = Int64.Parse(value.ToString());
                                    objectList.Add((Int64?)value);
                                }
                            }
                            else if (typeof(T) == typeof(Double))
                            {
                                objectList.Add(Double.Parse(reader[propertyName].ToString()));
                            }
                            else if (typeof(T) == typeof(Double?))
                            {
                                object value = reader[propertyName];
                                if (value != DBNull.Value)
                                {
                                    value = Double.Parse(value.ToString());
                                    objectList.Add((Double?)value);
                                }
                            }  // DEM 20100127
                            else if (typeof(T) == typeof(float))
                            {
                                objectList.Add(float.Parse(reader[propertyName].ToString()));
                            }
                            else if (typeof(T) == typeof(float?))
                            {
                                object value = reader[propertyName];
                                if (value != DBNull.Value)
                                {
                                    value = float.Parse(value.ToString());
                                    objectList.Add((float?)value);
                                }
                            }
                            else if (typeof(T) == typeof(Decimal))
                            {
                                objectList.Add(Decimal.Parse(reader[propertyName].ToString()));
                            }
                            else if (typeof(T) == typeof(Decimal?))
                            {
                                object value = reader[propertyName];
                                if (value != DBNull.Value)
                                {
                                    value = Decimal.Parse(value.ToString());
                                    objectList.Add((Decimal?)value);
                                }
                            }
                            else if (typeof(T) == typeof(Guid))
                            {
                                objectList.Add(new Guid(reader[propertyName].ToString()));
                            }
                            else if (typeof(T) == typeof(Guid?))
                            {
                                object value = reader[propertyName];
                                if (value != DBNull.Value)
                                {
                                    value = new Guid(value.ToString());
                                    objectList.Add((Guid?)value);
                                }
                            }
                        }
                    }
                }
            }
            catch { throw; }

            return objectList.ToArray(); 
        }

        public object[] Load(Type persistentType, IDbCommand command)
        {
            object[] persistables = new object[0];
            List<object> objectList = new List<object>();

            try
            {
                objectList = Process(command, persistentType, true);
            }
            catch { throw; }

            persistables = (object[])Array.CreateInstance(persistentType, objectList.Count);
            for (int i = 0; i < objectList.Count; i++)
            {
                persistables[i] = objectList[i];
            }

            return persistables;
        }

        #region DataSet
        public DataSet Load(string sql)
        {
            return Load(sql, new List<IDbDataParameter>());
        }

        public DataSet Load(string sql, List<IDbDataParameter> parms)
        {
            DataSet dataSet = new DataSet();
            sql = CleanSqlStmt(sql);
            //#if DEBUG
            //            m_Log.Debug(sql);
            //#endif
            using (IDbCommand command = this.GetConnection().CreateCommand())
            {
                foreach (IDbDataParameter parm in parms)
                {
                    command.Parameters.Add(parm);
                }
                command.CommandTimeout = this.m_CommandTimeout;
                IDbDataAdapter dataAdapter = ConnectionInfo.GetDataAdapter();
                command.CommandText = sql;
                dataAdapter.SelectCommand = command;
                dataAdapter.Fill(dataSet);
            }
            return dataSet;
        }

        public DataSet Load(Type entityType, string[] fields)
        {
            return Load(entityType, null, new OrderCriteria(), fields);
        }

        public DataSet Load(Type entityType, FilterCriteria filterCriteria, string[] fields)
        {
            return Load(entityType, filterCriteria, new OrderCriteria(), fields);
        }

        public DataSet Load(Type type, FilterCriteria filterCriteria, OrderCriteria orderCriteria, string[] fields)
        {
            DataSet dataSet = new DataSet();
            using (IDbCommand command = GetConnection().CreateCommand())
            {
                ProcessSelectStmt(command, AppInfo.GetTableName(type), fields, filterCriteria, orderCriteria, null);
                command.CommandTimeout = m_CommandTimeout;
                IDbDataAdapter dataAdapter = ConnectionInfo.GetDataAdapter();
                dataAdapter.SelectCommand = command;
                dataAdapter.Fill(dataSet);
            }
            return dataSet;
        }
        #endregion

        #region GetSqlCost
        public double GetSqlCost(string sql)
        {
            double cost = 0;
            using (IDbCommand command = GetCommand(""))
            {
                cost = ConnectionInfo.GetSqlCost(command, sql);
            }
            return cost;
        }
        #endregion GetSqlCost

        #region Describe
        public List<string> Describe(string name)
        {
            using (IDbCommand command = GetCommand(""))
            {
                return ConnectionInfo.Describe(command, name);
            }
        }
        #endregion Describe

        #region GetOrderNumber
        public string GetOrderNumber()
        {
            string orderNumber = string.Empty;
            using (IDbCommand command = GetCommand(""))
            {
                orderNumber = ConnectionInfo.GetOrderNumber(command);
            }
            return orderNumber;
        }
        #endregion GetOrderNumber

        #region IDisposable Members
        public void Dispose()
        {
//#if (DEBUG)
//            m_Log.Debug("....Closing from Dispose() - YAY!");
//#endif
            foreach (IDbCommand command in m_CommandsToDispose)
            {
                command.Dispose();
            }
            CloseConnection();
        }
        #endregion

        protected internal List<object> Process(IDbCommand command, Type persistentType, bool deepLoad)
        {
            List<object> objectList = new List<object>();
            using (IDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    objectList.Add(GetObject(reader, persistentType, deepLoad));
                }
            }
            return objectList;
        }

        public object GetObject(IDataReader reader, Type persistentType, bool deepLoad)
        {
            string propertyName = "[unknown]";
            int i = -1;
            PropertyInfo[] propertyInfos = AppInfo.GetPropertiesInfo(persistentType);
            InstantiateObjectHandler instantiateObjectHandler = DynamicMethodCompiler.CreateInstantiateObjectHandler(persistentType);
            object entity = instantiateObjectHandler();

            SetHandler setHandler;

            PropertyInfo objectKeyProperty = persistentType.GetProperty("ObjectKey");
            if (objectKeyProperty != null &&
                objectKeyProperty.GetCustomAttributes(typeof(NotPersistableAttribute), true).Length == 0)
            {
                setHandler = DynamicMethodCompiler.CreateSetHandler(persistentType, objectKeyProperty);
                setHandler(entity, new Guid(reader[AppInfo.GetTableName(persistentType) + "Key"].ToString()));
            }
            PropertyInfo idProperty = persistentType.GetProperty("Id");
            if (idProperty != null)
            {
                setHandler = DynamicMethodCompiler.CreateSetHandler(persistentType, idProperty);
                setHandler(entity, Convert.ToInt64(reader["Id"]));
            }
            try
            {
                for (i = 0; i < propertyInfos.Length; i++)
                {
                    propertyName = propertyInfos[i].Name;

                    if (!propertyName.Equals("ObjectKey") && !propertyName.Equals("ObjectState"))
                    {
                        setHandler = DynamicMethodCompiler.CreateSetHandler(persistentType, propertyInfos[i]);
                        try
                        {
                            if (propertyInfos[i].PropertyType == typeof(String))
                            {
                                object value = reader[propertyName];
                                if (value != DBNull.Value)
                                {
                                    setHandler(entity, reader[propertyName] as String);
                                }
                            }
                            else if (propertyInfos[i].PropertyType == typeof(DateTime))
                            {
                                setHandler(entity, (DateTime)reader[propertyName]);
                            }
                            else if (propertyInfos[i].PropertyType == typeof(DateTime?))
                            {
                                object value = reader[propertyName];
                                if (value != DBNull.Value)
                                {
                                    setHandler(entity, (DateTime?)value);
                                }
                            }
                            else if (propertyInfos[i].PropertyType == typeof(Int16))
                            {
                                setHandler(entity, Int16.Parse(reader[propertyName].ToString()));
                            }
                            else if (propertyInfos[i].PropertyType == typeof(Int16?))
                            {
                                object value = reader[propertyName];
                                if (value != DBNull.Value)
                                {
                                    value = Int16.Parse(value.ToString());
                                    setHandler(entity, (Int16?)value);
                                }
                            }
                            else if (propertyInfos[i].PropertyType == typeof(Int32))
                            {
                                setHandler(entity, Int32.Parse(reader[propertyName].ToString()));
                            }
                            else if (propertyInfos[i].PropertyType == typeof(Int32?))
                            {
                                object value = reader[propertyName];
                                if (value != DBNull.Value)
                                {
                                    value = Int32.Parse(value.ToString());
                                    setHandler(entity, (Int32?)value);
                                }
                            }
                            else if (propertyInfos[i].PropertyType == typeof(Int64))
                            {
                                setHandler(entity, Int64.Parse(reader[propertyName].ToString()));
                            }
                            else if (propertyInfos[i].PropertyType == typeof(Int64?))
                            {
                                object value = reader[propertyName];
                                if (value != DBNull.Value)
                                {
                                    value = Int64.Parse(value.ToString());
                                    setHandler(entity, (Int64?)value);
                                }
                            }
                            else if (propertyInfos[i].PropertyType == typeof(Double))
                            {
                                setHandler(entity, Double.Parse(reader[propertyName].ToString()));
                            }
                            else if (propertyInfos[i].PropertyType == typeof(Double?))
                            {
                                object value = reader[propertyName];
                                if (value != DBNull.Value)
                                {
                                    value = Double.Parse(value.ToString());
                                    setHandler(entity, (Double?)value);
                                }
                            }  // DEM 20100127
                            else if (propertyInfos[i].PropertyType == typeof(float))
                            {
                                setHandler(entity, float.Parse(reader[propertyName].ToString()));
                            }
                            else if (propertyInfos[i].PropertyType == typeof(float?))
                            {
                                object value = reader[propertyName];
                                if (value != DBNull.Value)
                                {
                                    value = float.Parse(value.ToString());
                                    setHandler(entity, (float?)value);
                                }
                            }
                            else if (propertyInfos[i].PropertyType == typeof(Decimal))
                            {
                                setHandler(entity, Decimal.Parse(reader[propertyName].ToString()));
                            }
                            else if (propertyInfos[i].PropertyType == typeof(Decimal?))
                            {
                                object value = reader[propertyName];
                                if (value != DBNull.Value)
                                {
                                    value = Decimal.Parse(value.ToString());
                                    setHandler(entity, (Decimal?)value);
                                }
                            }
                            else if (propertyInfos[i].PropertyType == typeof(Byte))
                            {
                                setHandler(entity, (Byte)reader[propertyName]);
                            }
                            else if (propertyInfos[i].PropertyType == typeof(Byte?))
                            {
                                object value = reader[propertyName];
                                if (value != DBNull.Value)
                                {
                                    setHandler(entity, (Byte?)value);
                                }
                            }
                            else if (propertyInfos[i].PropertyType == typeof(Byte[]))
                            {
                                setHandler(entity, (Byte[])reader[propertyName]);
                            }
                            else if (propertyInfos[i].PropertyType == typeof(Guid))
                            {
                                setHandler(entity, new Guid(reader[propertyName].ToString()));
                            }
                            else if (propertyInfos[i].PropertyType == typeof(Guid?))
                            {
                                object value = reader[propertyName];
                                if (value != DBNull.Value)
                                {
                                    value = new Guid(value.ToString());
                                    setHandler(entity, (Guid?)value);
                                }
                            }
                            else if (propertyInfos[i].PropertyType == typeof(XmlDocument) || propertyInfos[i].PropertyType == typeof(XmlNode))
                            {
                                XmlDocument xmlDocument = new XmlDocument();
                                object value = reader[propertyName];
                                if (value != DBNull.Value)
                                {
                                    xmlDocument.LoadXml((string)reader[propertyName]);
                                    setHandler(entity, xmlDocument);
                                }
                            }
                            else if (propertyInfos[i].PropertyType == typeof(Boolean))
                            {
                                if (reader[propertyName].ToString() == "0")
                                {
                                    setHandler(entity, (Boolean)false);
                                }
                                else
                                {
                                    setHandler(entity, (Boolean)true);
                                }
                            }
                            else if (propertyInfos[i].PropertyType == typeof(Boolean?))
                            {
                                object value = reader[propertyName];
                                if (value != DBNull.Value)
                                {
                                    if (reader[propertyName].ToString() == "0")
                                    {
                                        setHandler(entity, (Boolean)false);
                                    }
                                    else
                                    {
                                        setHandler(entity, (Boolean)true);
                                    }
                                }
                            }
                            else
                            {
                                var customAttributes = propertyInfos[i].GetCustomAttributes(typeof(ChildAttribute), false); 
                                if (customAttributes.Length == 1)
                                {
                                    var childAttribute = (ChildAttribute)customAttributes[0];
                                    // Filling collection within object
                                    Type type = persistentType.GetProperty(propertyName).PropertyType.GetElementType();
                                    if (deepLoad)
                                    {
                                        FilterCriteria fc = new FilterCriteria();
                                        object keyValue;
                                        string parentAttributeName = string.Empty;

                                        if (type.GetProperty(string.Format("Parent{0}", AppInfo.GetTableName(persistentType))) != null)
                                        {
                                            parentAttributeName = string.Format("Parent{0}", AppInfo.GetTableName(persistentType));
                                        }
                                        else if (type.GetProperty(string.Format("Parent{0}", persistentType.Name)) != null)
                                        {
                                            parentAttributeName = string.Format("Parent{0}", persistentType.Name);
                                        }
                                        else
                                        {
                                            throw new NullReferenceException("No parent and child relationship found.");
                                        }
                                        PropertyInfo objKeyPropertyInfo = persistentType.GetProperty("ObjectKey");
                                        if (objKeyPropertyInfo == null)
                                        {
                                            throw new Exception(string.Format("Type {0} missing ObjectKey property", persistentType.Name));
                                        }
                                        keyValue = objKeyPropertyInfo.GetValue(entity, null);
                                        if (keyValue.Equals(Guid.Empty))
                                        {
                                            keyValue = persistentType.GetProperty("Id").GetValue(entity, null).ToString();
                                        }
                                        else
                                        {
                                            if (childAttribute.UseIdAsPrimaryKey)
                                            {
                                                keyValue = persistentType.GetProperty("Id").GetValue(entity, null).ToString();
                                            }
                                            else
                                            {
                                                keyValue = keyValue.ToString();
                                            }
                                        }
                                        var parentAttribute =
                                            ((ParentAttribute[])
                                                type.GetProperty(parentAttributeName)
                                                    .GetCustomAttributes(typeof(ParentAttribute), false))[0];
                                        fc.AddFilter(type,
                                            parentAttribute.ForeignKey,
                                            keyValue);
                                        if (parentAttribute.EntityType != null)
                                        {
                                            fc.AddFilter(type, OperandType.And, "EntityType",
                                                ComparisonMethod.Equals, parentAttribute.EntityType);
                                        }
                                        setHandler(entity, Load(type, fc));
                                    }
                                    else
                                    {
                                        setHandler(entity, (object[])Array.CreateInstance(type, 0));
                                    }
                                }
                            }
                        }
                        catch (IndexOutOfRangeException idxex)
                        {
                            m_Log.Warn(string.Format("Could not set property: {0}, may not exist in entity type {1}", propertyName, AppInfo.GetTableName(persistentType)), idxex);
                        }
                        catch (Exception ex)
                        {
                            m_Log.Error(string.Format("Unexpected error {0}.{1}", persistentType.Name, propertyName), ex);
                            throw;
                        }
                    }
                }

                setHandler = DynamicMethodCompiler.CreateSetHandler(persistentType, persistentType.GetProperty("ObjectState"));
                setHandler(entity, DataAction.None);
            }
            catch (Exception ex)
            {
                m_Log.Error(string.Format("Problem handling {0}", propertyName), ex);
                if (i > -1)
                {
                    m_Log.Error(string.Format("Of type: {0}", propertyInfos[i].PropertyType)); 
                } 
            } 
            return entity;
        }

        protected internal object[] Transform(Type type, List<object> objectList)
        {
            object[] persistables = (object[])System.Array.CreateInstance(type, objectList.Count);
            for (int i = 0; i < objectList.Count; i++)
            {
                persistables[i] = objectList[i];
            }
            return persistables;
        }

        protected internal void ProcessSelectStmt(IDbCommand command, string persistentTypeName, string[] fields, FilterCriteria filterCriteria, OrderCriteria orderCriteria, GroupCriteria groupCriteria)
        {
            // Filter
            SqlFilterCriteria sqlFilterCriteria = null;
            if (filterCriteria != null && filterCriteria.Criteria.Criteria.Count > 0)
            {
                sqlFilterCriteria = MutateFilterCriteria(filterCriteria);
            }
            string filterString = ((sqlFilterCriteria == null) ? String.Empty : sqlFilterCriteria.WhereClauseForParameters);
            // Order
            SqlOrderByCriteria sqlOrderByCriteria = null;
            if (orderCriteria != null && orderCriteria.OrderCriteriaList.Count > 0)
            {
                sqlOrderByCriteria = MutateOrderByCriteria(orderCriteria);
            }
            string orderString = ((sqlOrderByCriteria == null) ? String.Empty : sqlOrderByCriteria.OrderByClause);
            // Group
            SqlGroupByCriteria sqlGroupByCriteria = null;
            if (groupCriteria != null && groupCriteria.GroupCriteriaList.Count > 0)
            {
                sqlGroupByCriteria = MutateGroupByCriteria(groupCriteria);
            }
            string groupString = ((sqlGroupByCriteria == null) ? String.Empty : sqlGroupByCriteria.GroupByClause);
            // Predicate
            string predicateString = string.Empty;
            if (m_PredicateDictionary.ContainsKey(persistentTypeName))
            {
                predicateString = m_PredicateDictionary[persistentTypeName].GetSqlPredicateStatement(ConnectionInfo);
            }
            // Columns
            string columnList = string.Empty;
            foreach (string field in fields)
            {
                if (field.ToUpper().StartsWith("DISTINCT("))
                {
                    columnList += string.Format("{0},", field);
                }
                else if (field.ToUpper().StartsWith("MAX("))
                {
                    columnList += string.Format("{0},", field);
                }
                else
                {
                    columnList += string.Format("{0},", field);
                }
            }
            if (fields.Length > 0)
            {
                columnList = columnList.Substring(0, columnList.Length - 1);
                columnList = columnList.Replace("ObjectKey", persistentTypeName + "Key");
            }
            if (columnList == string.Empty)
            {
                columnList = string.Format("{0}.*", persistentTypeName);
            }

            string sql = ConnectionInfo.GetSelectStmt(predicateString, columnList, persistentTypeName, filterString, orderString, groupString);

            sql = CleanSqlStmt(sql);

#if (DEBUG)
            m_Log.Debug(sql);
#endif

            command.CommandText = sql;
            command.CommandType = CommandType.Text;
            if (sqlFilterCriteria != null)
            {
                sqlFilterCriteria.AddParameters(command);
            }
        }

        protected internal string CleanSqlStmt(string sql)
        {
            while (sql.Contains("  "))
            {
                sql = sql.Replace("  ", " ");
            }
            sql = sql.Replace("( ", "(");
            sql = sql.Replace(" )", ")");
            return sql;
        }
    }
}