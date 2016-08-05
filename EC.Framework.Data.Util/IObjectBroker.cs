using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;

namespace EC.Framework.Data
{
    /// <summary>
    /// Interface that defines the behavior of an ObjectBroker which
    /// accesses the persistent data store on behalf of applications.
    /// </summary>
    public interface IObjectBroker : IDisposable
    {
        /// <summary>
        /// Get or Set ConnectionInfo object which will be used in the 
        /// creation of connection objects by this object broker.
        /// </summary>
        /// <value>The connection info.</value>
        IConnectionInfo ConnectionInfo
        {
            get;
            set;
        }

        bool TransactionStarted
        {
            get;
        }

        /// <summary>
        /// Obtains a connection to the data store.  This connection
        /// will be created according to settings on the ConnectionInfo
        /// property of the object broker.
        /// </summary>
        IDbConnection GetConnection();

        /// <summary>
        /// Creates a Command object based on the supplied string.
        /// </summary>
        IDbCommand GetCommand(string sqlStatement);

        long GetObjectCount(Type persistentType, FilterCriteria criteria); 

        /// <summary>
        ///	Closes the connection to the persistent data store.
        /// </summary>
        void CloseConnection();

        /// <summary>
        /// Deletes data of the specified IPersistable data type from the data store.
        /// </summary>
        int DeleteDirect(Type persistentType);

        /// <summary>
        /// Deletes data of the specified IPersistable data type from the data store
        /// based on the given filter criteria.  
        /// </summary>
        int DeleteDirect(Type persistentType, FilterCriteria filterCriteria);

        /// <summary>
        /// Instantiates an array of objects from the persistent data
        /// store that are owned by a parent object.  This array
        /// represents all of the available objects of the requested
        /// type that currently reside in the persistent data
        /// store.
        /// </summary>
        object[] Load(Type persistentType, FilterCriteria criteria);
        object[] Load(Type persistentType, FilterCriteria criteria, OrderCriteria orderCriteria);
        object[] Load(Type persistentType, FilterCriteria criteria, OrderCriteria orderCriteria, bool deepLoad);
        object[] Load(Type persistentType, string columnName, FilterCriteria filterCriteria, OrderCriteria orderCriteria, GroupCriteria groupCriteria);
        DataSet Load(Type persistentType, FilterCriteria criteria, string[] fields);
        long[] LoadObjectIds(Type persistentType, FilterCriteria filterCriteria, OrderCriteria orderCriteria, GroupCriteria groupCriteria);
        Guid[] LoadObjectKeys(Type persistentType, FilterCriteria filterCriteria, OrderCriteria orderCriteria, GroupCriteria groupCriteria);

        int UpdateDirect(Type persistentType, Dictionary<string, object> updateDictionary, FilterCriteria filterCriteria);

        /// <summary>
        /// Synchronizes members of the persistable collection with the
        /// data store.
        /// </summary>
        void Save(object[] collection);
        void DeepSave(object[] collection); 

        /// <summary>
        /// Initiates a new transaction for the object broker.  All
        /// business object collections which are persisted through
        /// the broker for the duration of the transaction will be
        /// handled atomically.
        /// </summary>
        void StartTransaction();
        void StartTransaction(System.Data.IsolationLevel isolationLevel); 

        /// <summary>
        /// Rolls back all changes to the data store that have been submitted
        /// through the object broker since the transaction was initiated 
        /// with a call to StartTransaction.
        /// </summary>
        void RollbackTransaction();

        /// <summary>
        /// Commits all changes to the data store that have been submitted
        /// through the object broker since the transaction was initiated
        /// with a call to StartTransaction.
        /// </summary>
        void EndTransaction();

        void SetTypeSelectPredicate(Type type, SelectPredicateAttribute selectPredicateAttribute);

        object[] GetObjectProperty<T>(Type persistentTypeName, string propertyName, FilterCriteria fc);
    }
}