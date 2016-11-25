using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
///using EC.Core.Interfaces;
using EC.Service.DTO;
using EC.Errors;
using EC.Errors.CommonExceptions;

namespace EC.Service.Interfaces
{
    /// <summary>
    /// This service provides operations that deal with the internal state of the service. These entry
    /// points are typically used for debugging.
    /// </summary>
    
    [ServiceContract]
    public interface IAdminService
    {
        /// <summary>
        /// This is a function to test a memory leak!!!!!!
        /// </summary>
        /// <param name="numberOfTimes"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(UnknownFault))]

        string BashNotificationRepoAll(int numberOfTimes);

        /// <summary>
        /// This is a function to test a memory leak!!!!!!
        /// </summary>
        /// <param name="numberOfTimes"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(UnknownFault))]

        string BashPredicateRepoAll(int numberOfTimes);

        /// <summary>
        /// This is a function to test a memory leak!!!!!!
        /// </summary>
        /// <param name="numberOfTimes"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(UnknownFault))]

        string BashCapabilityRepoAll(int numberOfTimes);

        /// <summary>
        /// Force a garbage collection (usually done just before you do a memory dump
        /// of the process to ensure that the memory is as "clean" as possible for
        /// debugging the dump file.
        /// </summary>
        /// <returns>true if the GC succeeded</returns>
        
        [OperationContract]
        [FaultContract(typeof(UnknownFault))]
        
        bool ForceGC();

        /// <summary>
        /// Return all current cache entries for the given user (as a list of tuples containing
        /// the path, the capability, and boolean for whether the capability is granted).
        /// </summary>
        /// <param name="uow">current unit of work</param>
        /// <param name="userName">user to filter by</param>
        /// <returns>list of cache entries (path, capability, allowed?)</returns>
        /// <exception cref="NotFoundFault">if the user cannot be found</exception>

        [OperationContract]
        [FaultContract(typeof(UnknownFault))]
        [FaultContract(typeof(NotFoundFault))]

        List<Tuple<string, string, bool>> GetPermissionsCacheEntriesForUser(string userName);

        /// <summary>
        /// Return all current cache entries for the given user (as a list of tuples containing
        /// the path, the capability, and boolean for whether the capability is granted).
        /// </summary>
        /// <param name="userId">the primary ID for the user</param>
        /// <returns>list of cache entries (path, capability, allowed?)</returns>

        [OperationContract]
        [FaultContract(typeof(UnknownFault))]

        List<Tuple<string, string, bool>> GetPermissionCacheEntriesForUserById(Guid userId);

        [OperationContract]
        [FaultContract(typeof(UnknownFault))]

        void InvalidateAllPermissionsCache();

        /// <summary>
        /// Gets all custom DNS.
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(UnknownFault))]
        [FaultContract(typeof(ConfigurationFault))]

        IList<CustomDNS> GetAllCustomDNS();

        /// <summary>
        /// Updates all custom DNS.
        /// </summary>
        /// <param name="data">The data.</param>
        [OperationContract]
        [FaultContract(typeof(UnknownFault))]
        [FaultContract(typeof(ConfigurationFault))]

        void UpdateAllCustomDNS(IList<CustomDNS> data);


        /// <summary>
        /// Gets the custom DNS by incoming DNS.
        /// </summary>
        /// <param name="incomingDNS">The incoming DNS.</param>
        /// <returns>the custom DNS entry for the incoming DNS entry</returns>
        [OperationContract]
        [FaultContract(typeof(UnknownFault))]
        [FaultContract(typeof(ConfigurationFault))]

        CustomDNS GetCustomDNSByIncomingDNS(string incomingDNS);

        /// <summary>
        /// Gets the redirect url for a custom DNS
        /// </summary>
        /// <param name="incomingHost">The incoming host (CustomDNS)</param>
        /// <param name="contentPath">The content path</param>
        /// <param name="requestIsHttps">Whether request is already Https</param>
        /// <returns>The url to redirect</returns>
        [OperationContract]
        [FaultContract(typeof(UnknownFault))]

        string GetRedirectFor(string incomingHost, string contentPath, bool requestIsHttps);

        /// <summary>
        /// Get CustomDNS DefaultPath
        /// </summary>
        /// <param name="incomingHost">The incoming DNS</param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(UnknownFault))]

        string GetCustomDnsDefaultPathByIncomingHost(string incomingHost);

        /// <summary>
        /// Endpoint verification function for installer. 
        /// </summary>
        /// <returns>true</returns>
        [OperationContract]

        bool IsAdminService();

        /// <summary>
        /// Returns maximum number of failed attempts
        /// </summary>
        /// <param name="orgPath"></param>
        /// <param name="userId"></param>
        /// <returns></returns>

        [OperationContract]
        [FaultContract(typeof(UnknownFault))]
        [FaultContract(typeof(NotFoundFault))]
        
        int AllowedFailedAttempts(string orgPath);

        /// <summary>
        /// Records command execution and its results.
        /// </summary>
        /// <param name="source">Command line client the command was executed for</param>
        /// <param name="command">Command that is executed</param>
        /// <param name="exceptionGenerated">Whether an exception was generated in executing the command</param>
        /// <param name="results">Result of the command</param>
        [OperationContract]
        [FaultContract(typeof(UnknownFault))]
        [FaultContract(typeof(ConfigurationFault))]

        void RecordCommandExecution(string source, string command, bool exceptionGenerated, List<string> results);

        /// <summary>
        /// Dump out command history by pattern and date. The date should be in the format MM/DD/YYYY.
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(UnknownFault))]

        string PrintCommandExecutionHistory(string pattern, DateTime date1, DateTime date2);

        /// <summary>
        /// Ensure at most one non-cancelled exam.
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(UnknownFault))]
        string EnsureUniqueExamInstanceForStudentAndLink(string examLinkPath, Guid userId);

        /// <summary>
        /// Ensure at most one non-cancelled exam.
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(UnknownFault))]
        string EnsureUniqueEvaluationInstanceForStudentAndLink(string examLinkPath, Guid userId);

        /// <summary>
        /// Ensure at most one non-cancelled exam.
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(UnknownFault))]
        string EnsureUniquAssessmentInstanceForId(string assessmentInstanceId);

        /// <summary>
        /// Writes a ClockTimeStamp to the DB using UTCNow and a TickIncrement of 0.
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(UnknownFault))]
        string ForceClock();

        /// <summary>
        /// Prints clock update history.
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(UnknownFault))]
        string GetClockHistory(int days);

        /// <summary>
        /// Prints current monotime & systime.
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(UnknownFault))]
        string GetCurrentTime();

        /// <summary>
        /// Check to ensure both timestamps are within MaxRTAClockDifference of the mono time on the current site.
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(UnknownFault))]
        string CheckCurrentTime(DateTime monoTime, DateTime sysTime);
    }
}
