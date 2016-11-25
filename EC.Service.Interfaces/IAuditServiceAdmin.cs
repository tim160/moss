using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using EC.Common.Interfaces;
using EC.Service.DTO;
using EC.Constants;
using EC.Errors;
using EC.Errors.CommonExceptions;
using EC.Errors.ECExceptions;

namespace EC.Service.Interfaces
{
    /// <summary>
    /// API for access to audit information in EC.
    /// </summary>
    
    [ServiceContract]
    public interface IAuditServiceAdmin
    {
        /// <summary>
        /// Creates a new session in the set of active sessions. Also creates a permanent 
        /// session audit record. No parameters are required as the operation implicitly
        /// uses the user from the current request.
        /// </summary>
        /// <exception cref="UnknownFault">For any error case.</exception>

        [OperationContract]
        [FaultContract(typeof(UnknownFault))]

        void StartSession();

        [OperationContract]
        [FaultContract(typeof(UnknownFault))]
        void CreateSessionAndTrackingAudits(int load, string path);

        /// <summary>
        /// Start a session for a given user (by login name). As with StartSession() this will also
        /// create a permanent session audit record.
        /// </summary>
        /// <param name="loginName">the user's ID</param>
        /// <param name="orgPath">Organization path. Used to login by orgId. If using username or email login then this can be null or an empty string.</param>
        /// <exception cref="UnknownFault">For any error case.</exception>

        [OperationContract]
        [FaultContract(typeof(UnknownFault))]

        void StartSessionForUserByLogin(string loginName, string orgPath);

        /// <summary>
        /// Indicates that the session for the current user is still active. Sessions
        /// that are active for a period of time are destroyed.
        /// </summary>
        /// <exception cref="UnknownFault">For any error case.</exception>

        [OperationContract]
        [FaultContract(typeof(UnknownFault))]

        void SessionKeepAlive();

        /// <summary>
        /// Will get all links user can see
        /// </summary>
        /// <param name="path">path to link item to start looking for links on</param>
        /// <param name="userId">user to get links they can see</param>
        /// <param name="filter">type of links to get for user</param>
        /// <param name="pageInfo">paging info</param>
        /// <param name="offering">offering to get links on</param>
        /// <returns>Returns a CollectionSubset of ListOfViewedLinkItems</returns>
        /// <exception cref="UnknownFault">For any error.</exception>

        ////[OperationContract]
        ////[FaultContract(typeof(UnknownFault))]

        ////CollectionSubset<LinkAuditEntry> GetLinksUserCanSee(string path, Guid userId, AllLinksUserCanSeeFilterEnum filter, DTO.PageInfo pageInfo, CourseOffering offering);

        /// <summary>
        /// Get a course offering progress report for the whole course (all students are considered).
        /// <remarks>
        /// Note that the <paramref name="linkFilter"/> does affect the <c>LinkList</c> items and count, but doesn't affect the total link counts 
        /// like <c>LinkVisitsAsPercentage</c>.
        /// </remarks>
        /// </summary>
        /// <param name="coursePath">Path of the course link the course offering is on</param>
        /// <param name="courseOfferingId">Course offering Id</param>
        /// <param name="pageInfo">Optionally page the returned link progresses.</param>
        /// <param name="sortDirection">Direction to sort by</param>
        /// <param name="sortColumn">Sort by column.</param>
        /// <param name="linkFilter">Filter links according to this filter (optional, mandatory, both).</param>
        /// <returns>Return a report of all visited links with percentages, student count,...</returns>
        /// <exception cref="NotFoundFault">If the course or course offering doesn't exist.</exception>
        /// <exception cref="NotACourseFault">If the item with <c>courseId</c> exists, but it is not a course (index page).</exception>
        /// <exception cref="UnknownFault">In any other cases.</exception>

        [OperationContract]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(NotACourseFault))]
        [FaultContract(typeof(UnknownFault))]
       
        CourseOfferingProgressAudit GetAuditLogsForCourseOfferingProgressByPath(
            string coursePath, Guid courseOfferingId, DTO.PageInfo pageInfo,
            SortDirectionEnum sortDirection, CourseOfferingProgressAuditSortTypesEnum sortColumn,
            AllLinksUserCanSeeFilterEnum linkFilter);

        /// <summary>
        /// Get session summary information for all users over the given time period. This call assumes
        /// that the start date in in the past (since it doesn't make sense to ask for audit records in
        /// the future as they can't exist yet). If end date is given, then it is assumed to be the current
        /// date/time. If paging info is null, then all results are returned.
        /// </summary>
        /// <param name="pageInfo">details on the subset of items that should be returned</param>
        /// <param name="start">start of time range for which results should be returned</param>
        /// <param name="end">optional end of time range for which results should be returned</param>
        /// <returns>a set of session summary records for all users over the given dates</returns>

        [OperationContract]
        [FaultContract(typeof(UnknownFault))]

        CollectionSubset<SessionAuditSummary> GetSessionSummaryForAllUsers(DTO.PageInfo pageInfo, DateTime start, DateTime? end = null);

        /// <summary>
        /// Return summary information about the URLs that a user has visited over a given time period.
        /// </summary>
        /// <param name="userId">which user to get summary info for</param>
        /// <param name="pageInfo">which portion of the collection to return</param>
        /// <param name="start">begining of date range from which results whould be fetched</param>
        /// <param name="end">optional end of date range from which results should be fetched</param>
        /// <returns>the summary information</returns>

        [OperationContract]
        [FaultContract(typeof(UnknownFault))]

        CollectionSubset<UserTrackingSummary> GetTrackingSummaryForUser(Guid userId, DTO.PageInfo pageInfo, DateTime start, DateTime? end = null);

        /// <summary>
        /// Get a summary of accesses to pages. Each summary includes the URL for the page, a count of the number
        /// of unique users who accessed the page, and the total number of accesses to the page.
        /// </summary>
        /// <param name="pageInfo">optional parameter defining the subset to return. Return all if null</param>
        /// <param name="start">date/time defining the start of the search interval</param>
        /// <param name="end">optional date/time defining the end of the search interval</param>
        /// <returns>list of summayr records</returns>

        [OperationContract]
        [FaultContract(typeof(UnknownFault))]

        CollectionSubset<TrackingSummary> GetTrackingSummaryForAllUsers(DTO.PageInfo pageInfo, DateTime start, DateTime? end = null);

        /// <summary>
        /// Will get session audits for the passed in course offering
        /// </summary>
        /// <param name="coursePath">The path of the course link the course offering is on</param>
        /// <param name="courseOfferingId">Id of offering</param>
        /// <param name="pageInfo">paging object</param>
        /// <param name="sortColumn">sort order </param>
        /// <param name="sortDirection">Direction to sort column.</param>
        /// <returns>CollectionSubset of CourseSessionAudit</returns>
        /// <exception cref="WrongLinkTypeFault"></exception>
        /// <exception cref="NotFoundFault"></exception>
        /// <exception cref="UnknownFault">For any error case.</exception>

        [OperationContract]
        [FaultContract(typeof(WrongLinkTypeFault))]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(UnknownFault))]
        
        CollectionSubset<CourseSessionAudit> GetSessionAuditsForCourseOfferingByPath(string coursePath, Guid courseOfferingId, DTO.PageInfo pageInfo, CourseSessionsAuditSortTypes sortColumn, SortDirectionEnum sortDirection);

        /// <summary>
        /// Will get all session audits user in date range
        /// </summary>
        /// <param name="userId">Id of user to get session audits for</param>
        /// <param name="pageInfo">Paging object</param>
        /// <param name="sortOrder">sort order</param>
        /// <param name="sortDirection">ascending or descending sort</param>
        /// <param name="startDate">Optional start date filter</param>
        /// <param name="endDate">Optional end date filter</param>
        /// <returns>subset of user's sessions audit records</returns>
        /// <exception cref="NotFoundFault">if the user is not found</exception>
        /// <exception cref="UnknownFault">For any other error.</exception>

        [OperationContract]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(UnknownFault))]
        
        CollectionSubset<SessionAudit> GetSessionAuditsForUser(Guid userId, DTO.PageInfo pageInfo, SessionAuditSortTypes sortOrder, SortDirectionEnum sortDirection, DateTime? startDate = null, DateTime? endDate = null);

        /// <summary>
        /// Return a paged set of email channel audits filtered by subtring on the 'To' email field
        /// and optional by date. The result set is oredered by date (desceneding).
        /// </summary>
        /// <param name="pageInfo">paging information, null means return the complete set</param>
        /// <param name="receiverMatchStr">substring to match against 'To' field</param>
        /// <param name="startDate">optional date filter, only entries after the given date</param>
        /// <returns>the set of email channel audits</returns>

        [OperationContract]
        [FaultContract(typeof(UnknownFault))]

        CollectionSubset<EmailChannelAudit> GetEmailChannelAuditsByMatchingToField(DTO.PageInfo pageInfo, string receiverMatchStr, DateTime? startDate = null); 

        /// <summary>
        /// Return the set of all channel audits that are targeted for the given user ID. Optionally filtered by date. Resulting
        /// DTO's are correctly assembled into their sub-types so that they can be cast to the more specific type on the client.
        /// </summary>
        /// <param name="userId">user ID to look for</param>
        /// <param name="from">if provided, only include audits produced after the given date</param>

        [OperationContract]
        [FaultContract(typeof(UnknownFault))]

        DTO.CollectionSubset<DTO.BasicChannelAudit> GetChannelAuditsByUserId(DTO.PageInfo pageInfo, Guid userId, DateTime? from = null);

        /// <summary>
        /// Will select the chosen answer for the passed in question on an exam that has already been started
        /// </summary>
        /// <param name="question">the question to choose an answer for</param>
        /// <param name="selectedAnswerId">the answer that was chosen</param>
        /// <param name="uniqueInstanceId">Guid Id of this exam instance, used to find exam instance in audits</param>
        /// <exception cref="UnknownFault">For any error.</exception>
       
        //////[OperationContract]
        //////[FaultContract(typeof(UnknownFault))]

        //////void AnsweredQuestionExamInstaceAudit(Question question, Guid selectedAnswerId, Guid uniqueInstanceId,int sequenceNumber);

        /// <summary>
        /// Will finalize exam instance audit and set to a finished state
        /// </summary>
        /// <param name="uow">unit of work to put exam in finished state</param>
        /// <param name="uniqueInstanceId">Guid Id of this exam instance, used to find exam instance in audits</param>
        /// <param name="answeredQuestions">All answered questions. Dictionary<Guid,Guid> Key = QuestionId & Value = SelectedAnswerId </param>
        /// <exception cref="ExamAlreadyFinishedFault">If ExamState is already Finished</exception>
        /// <exception cref="NotExamTakerFault">If current user is not same user that stated the exam</exception>
        /// <exception cref="UnknownFault">For any error.</exception>
        
        [OperationContract]
        [FaultContract(typeof(UnknownFault))]

        void FinishExamInstanceAudit(Guid uniqueInstanceId,Dictionary<Guid,Guid> answeredQuestions);

        /// <summary>
        /// Get an exam instance by Id
        /// </summary>
        /// <param name="instanceId">Id of exam</param>
        /// <returns>found exam or null</returns>
        /// <exception cref="UnknownFault">For any error.</exception>

        //////[OperationContract]
        //////[FaultContract(typeof(UnknownFault))]

        //////AssessmentAudit GetAssessmentAuditById(Guid instanceId);       
        
        /// <summary>
        /// Used to generate large amounts of test data. Will create a user for each taking and register them in the course the passed in exam is in.
        /// They will get register in the the first offering found on the course and add them to the org group.
        /// </summary>
        
        [OperationContract]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(UnknownFault))]

        void GenerateExamTakings(string navPath, string examName, int numberOfTakings);

        /// <summary>
        /// Endpoint verification function for installer. 
        /// </summary>
        /// <returns>true</returns>
         
        [OperationContract]

        bool IsAuditServiceAdmin();

        /// <summary>
        /// Get notification audit logs of the EmailChannel.
        /// </summary>
        /// <param name="lastXDays">Amount of days to look into the past (e.g. 2 gets the logs for the last 2 days).</param>
        /// <param name="filterByAction">
        /// Set <c>null</c> to not filter any logs. 
        /// Set Created, FailedToProcess, Cancelled, Processed to filter for specific log entries</param>
        /// <returns>List of found audit entries already formatted in a string.</returns>

        [OperationContract]
        [FaultContract(typeof(UnknownFault))]

        List<string> GetAuditLogsForNotificationEmailChannel(int lastXDays, string filterByAction);

        /// <summary>
        /// Will re evaluate certificates for all users registered in the passed in offering 
        /// </summary>
        /// <param name="offeringId"></param>
        /// <param name="evaluateCompletedAndHistoricalRegistrations">true if function should evaluate completed and historical registrations</param>
        /// <exception cref="NotFoundFault">If offering can't be found by Id.</exception>

        [OperationContract]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(UnknownFault))]

        void ReEvaluateCerificatesForOffering(Guid offeringId,bool evaluateCompletedAndHistoricalRegistrations);


        /// <summary>
        /// Will regenerate rendered certificate for a user
        /// </summary>
        ///<remarks>
        /// NOTE: Command assumes that a certificate has already been assigned.
        /// NOTE: Command does not depend on previous certificate renderings.
        /// </remarks>
        /// <param name="coursePath"></param>
        /// <param name="username"></param>
        /// <param name="certificateShortId"></param>
        /// <param name="offeringShortId"></param>
        /// <exception cref="PathFormatFault">Thrown if the path only consists of '/'s, is empty or is <c>null</c>.</exception>
        /// <exception cref="NotFoundFault">thrown if item doesn't exist</exception>
        /// <exception cref="UnknownFault">On any other error</exception>
        [OperationContract]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(UnknownFault))]
        [FaultContract(typeof(PathFormatFault))]
        void RenderCertificateTemplateForUser(string coursePath, string username, string certificateShortId,string offeringShortId);


        /// <summary>
        /// Will evaluate registration rules for all users registered in the passed in offering 
        /// </summary>
        /// <param name="offeringId"></param>
        /// <exception cref="NotFoundFault">If offering can't be found by Id.</exception>

        [OperationContract]
        [FaultContract(typeof (NotFoundFault))]
        [FaultContract(typeof (UnknownFault))]

        void EvaluateRegistrationCompletionRulesForOffering(Guid offeringId);

        /// <summary>
        /// Will get all audit logs filtered by Discriminator and ordered by descending 
        /// </summary>
        /// <param name="discriminator">discriminator to filter audits by</param>
        /// <param name="count">Top number of audits to return</param>
        /// <returns>List<Audit></returns>
        [OperationContract]
        [FaultContract(typeof(UnknownFault))]

        List<Audit> GetAuditLogsByDiscriminator(string discriminator, int count);

        ////////[OperationContract]
        ////////[FaultContract(typeof(UnknownFault))]
        ////////[FaultContract(typeof(PathFormatFault))]
        ////////[FaultContract(typeof(NotANavPageFault))]
        ////////[FaultContract(typeof(WrongLinkTypeFault))]
        ////////[FaultContract(typeof(NotAuthorizedFault))]
        ////////[FaultContract(typeof(ConfigurationFault))]

        ////////List<CertificateAudit> GetCertificateAuditsByCourseAndCurrentState(string path, DateTime? startDate, DateTime? endDate, List<CertificateAuditState> states);

        /// <summary>
        /// Revoke the certificate audit. The parameter 'isRule' determines whether a 'rule revoke'
        /// or a 'manual revoke' is done. The return value indicates whether the revoke succeeded
        /// or not.
        /// </summary>
        /// <remarks>
        /// Because this is an audit item and the path to the correspondent item might not 
        /// exist anymore we check the permission on the portion of the AuditPath which
        /// still exist.
        /// </remarks>
        /// <exception cref="NotFoundFault">If certificate audit doesn't exist.</exception>
        /// <exception cref="NotAuthorizedFault">if the user does not have permission to revoke the certificate audit</exception>
        /// <exception cref="ConfigurationFault">Some error with the DB.</exception>
        /// <exception cref="UnknownFault">On any other error.</exception>

        [OperationContract]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(NotAuthorizedFault))]
        [FaultContract(typeof(ConfigurationFault))]
        [FaultContract(typeof(UnknownFault))]

        bool RevokeCertificateAudit(Guid certificateAuditId, bool isRule);
    }
}



        
