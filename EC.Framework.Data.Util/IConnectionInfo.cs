using System;
using System.Collections.Generic;
using System.Data;

namespace EC.Framework.Data
{
    /// <summary>
    /// IConnectionInfo provides configuration data that will 
    /// be used to customize the behavior of an IObjectBroker.
    /// </summary>
    public interface IConnectionInfo
    {
        /// <summary>
        ///	Connection string to the persistent data store that will used by an IObjectBroker.
        /// </summary>
        /// <value>The connection string.</value>
        string ConnectionString
        {
            get;
        }

        string ConnectionType
        {
            get;
        }

        /// <summary>
        /// Character that represents a parameter in a SQL statement (@/:)
        /// </summary>
        string ParameterToken
        {
            get;
        }

        IDbConnection CreateConnection();
        IDbCommand GetCommand();
        IDbDataAdapter GetDataAdapter(); 
        IDbDataParameter GetParameter(string name, object value);
        /// <summary>
        /// Used by DbFactory
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        IDbDataParameter GetNamedParameter(string name, object value);
        IDbDataParameter GetNamedParameterText(string name, object value);
        IDbDataParameter GetNamedParameterXml(IDbCommand command, string name, object value); 
        void SaveTransaction(IDbTransaction transaction, string savePointName);

        string ComparisonText(ComparisonMethod comparisonMethod, string valueString, Type type);
        string ComparisonTextForParameters(ComparisonMethod comparisonMethod, ref object value, int counter);
        string FormatDate(object value); 
        string GetXmlOrder(string persistentTypeName, string[] xmlColumn, string xmlPath, string dataType);
        string GetColumnAlias(string persistentTypeName, string[] xmlColumn, string xmlPath, string dataType, SelectionCriterion sc);
        string GetPredicateTop(string parameter);
        string GetSelectStmt(string predicate, string cols, string from, string filter, string order, string group);
        string GetSelectByRowNumStmt(Type type, long startId, int maxRows, string typeName, string filter, string order);
        /// <summary>
        /// Returns the XQuery needed to extract a given fieldname from the WorkOrder OrderData
        /// </summary>
        /// <param name="fieldName">The name of the field as defined in the Order Type</param>
        /// <returns></returns>
        string GetXmlFieldSelect(string fieldName);
        IDbDataParameter GetNamedParameterVarbinary(string name, object value);
        double GetSqlCost(IDbCommand command, string sql);
        string GetServerName(IDbConnection connection);
        string GetDatabaseName(IDbConnection connection);
        Int64 GetId(IDbCommand command, string entityName);
        string GetDataType(SelectionCriterion selectionCriterion);
        string NameFormat();
        string GetDeleteStmt(string predicate, string from, string filter);
        List<string> Describe(IDbCommand command, string name);
        string GetOrderNumber(IDbCommand command);
    }
}