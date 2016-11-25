using System;
using System.Collections.Generic;
using System.ServiceModel;
using EC.Service.DTO;
using EC.Constants;
using EC.Errors;
using EC.Errors.CommonExceptions;
using EC.Errors.ECExceptions;
using System.IO;

namespace EC.Service.Interfaces
{
    [ServiceContract]
    public interface IAuditServiceSecure
    {
        /// <summary>
        /// Creates the tracking audit. NOTE: The current user must have permission to see an item in order
        /// to be able to create a tracking audit for it. If the user does not have permissions, it will generate
        /// a log entry, but no exception is thrown.
        /// </summary>
        /// <param name="path">The path to the item that is being tracked.</param>

        [OperationContract]
        [FaultContract(typeof(UnknownFault))]

        void CreateTrackingAudit(string path);

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
        /// <exception cref="NotAuthorizedException">thrown if the user does not have permission</exception>
        /// <exception cref="UnknownFault">In any other cases.</exception>

        [OperationContract]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(NotACourseFault))]
        [FaultContract(typeof(NotAuthorizedFault))]
        [FaultContract(typeof(UnknownFault))]

        CourseOfferingProgressAudit GetAuditLogsForCourseOfferingProgressByPath(
            string coursePath, Guid courseOfferingId, PageInfo pageInfo,
            SortDirectionEnum sortDirection, CourseOfferingProgressAuditSortTypesEnum sortColumn,
            AllLinksUserCanSeeFilterEnum linkFilter);

        /// <summary>
        /// Will get all session audits for the passed in offering
        /// </summary>
        /// <param name="coursePath">The path of the course the offering is on</param>
        /// <param name="courseOfferingId">The Id of the offering to get session audits for</param>
        /// <param name="pageInfo">paging object</param>
        /// <param name="sortOrder">the current sort order</param>
        /// <param name="sortDirection">whether the sort is ascending or descending</param>
        /// <returns>CollectionSubset of CourseSessionAudit</returns>
        /// <exception cref="NotFoundFault">path not found</exception>
        /// <exception cref="WrongLinkTypeFault">path link-type consistency problem</exception>
        /// <exception cref="NotAuthorizedFault">User lacks ability to see sessions audits for the given path</exception>

        [OperationContract]
        [FaultContract(typeof(NotAuthorizedFault))]
        [FaultContract(typeof(WrongLinkTypeFault))]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(UnknownFault))]

        CollectionSubset<CourseSessionAudit> GetSessionAuditsForCourseOfferingByPath(string coursePath, Guid courseOfferingId, PageInfo pageInfo,
            CourseSessionsAuditSortTypes sortOrder, SortDirectionEnum sortDirection);

        /// <summary>
        /// Will get all session audits user in date range
        /// </summary>
        /// <param name="userId">Id of user to get session audits for</param>
        /// <param name="pageInfo">Paging object</param>
        /// <param name="sortOrder">sort order</param>
        /// <param name="locationPath">the location in the content tree from which the user is requesting the information</param>
        /// <param name="sortDirection">ascending or descending sort</param>
        /// <param name="startDate">Optional start date filter</param>
        /// <param name="endDate">Optional end date filter</param>
        /// <returns>subset of a user's sessions audit records</returns>
        /// <exception cref="NotFoundFault">path or user not found</exception>
        /// <exception cref="WrongLinkTypeFault">path link-type consistency problem</exception>
        /// <exception cref="NotAuthorizedFault">User lacks ability to see sessions audits for the given path</exception>

        [OperationContract]
        [FaultContract(typeof(NotAuthorizedFault))]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(UnknownFault))]

        CollectionSubset<SessionAudit> GetSessionAuditsForUser(Guid userId, PageInfo pageInfo, SessionAuditSortTypes sortOrder, SortDirectionEnum sortDirection, string locationPath,
            DateTime? startDate = null, DateTime? endDate = null);

        /// <summary>
        /// Will get all links user can see
        /// </summary>
        /// <param name="contentItemPath">path to link item to start looking for links on</param>
        /// <param name="userId">user to get links they can see</param>
        /// <param name="filter">type of links to get for user</param>
        /// <param name="pageInfo">paging info</param>
        /// <param name="offering">offering to get links on</param>
        /// <param name="path">current path</param>
        /// <param name="sortBy">sort order</param>
        /// <param name="sortDirection">sort direction</param>
        /// <returns>Returns a CollectionSubset of ListOfViewedLinkItems</returns>
        /// <exception cref="NotAuthorizedException">thrown if the user does not have permission</exception>
        /// <exception cref="UnknownFault">In any other cases.</exception>

        ////////[OperationContract]
        ////////[FaultContract(typeof(NotAuthorizedFault))]
        ////////[FaultContract(typeof(UnknownFault))]

        ////////CollectionSubset<LinkAuditEntry> GetTrackingAuditsByPathAndUser(string path, Guid userId, AllLinksUserCanSeeFilterEnum filter, PageInfo pageInfo, CourseOffering offering,
        ////////    LinkAuditEntrySort sortBy, SortDirectionEnum sortDirection);

        /////////////////////// <summary>
        /////////////// Get paged exam instances audits. Will return a tuple with exam instances, total average time, then total average grade
        /////////////// </summary>
        /////////////// <param name="navPagePath">nav page path to get exams on </param>
        /////////////// <param name="examId">Id of exam to get instances for</param>
        /////////////// <param name="pageInfo">paging info</param>
        /////////////// <param name="filter">GradeBookFilter filter</param>
        /////////////// <param name="sort">GradeBookSort sort</param>
        /////////////// <param name="offeringId">Offering Id grade book is viewing</param>
        /////////////// <returns>returns Tuple<CollectionSubset<GradesByAssessment>,int,int></returns>
        /////////////// <exception cref="NotAuthorizedException">thrown if the user does not have permission</exception>
        /////////////// <exception cref="UnknownFault">In any other cases.</exception>

        ////////////[OperationContract]
        ////////////[FaultContract(typeof(NotAuthorizedFault))]
        ////////////[FaultContract(typeof(UnknownFault))]

        ////////////Tuple<CollectionSubset<GradesByAssessment>, int, int> GetGradesByAssessment(string navPagePath, Guid examId, PageInfo pageInfo,
        ////////////    GradeBookFilter filter, string sortColumn, Guid offeringId, SortDirectionEnum sortDirection);

        /// <summary>
        /// Record the completion of an exam. 
        /// </summary>
        /// <param name="path">path to the exam link</param>
        /// <param name="id">the unique ID assigned to the exam instance</param>
        /// <param name="timerId">optional timer to kill</param>
        /// <param name="answeredQuestions">list of answers the user selected</param>
        /// <param name="isExpired">Flag from the UI telling that it has been expired (according to the UI clock).</param>
        /// <exception cref="PathFormatFault">if there are syntactive problems with the path</exception>
        /// <exception cref="NotFoundFault">if there is no item corresponding to the path</exception>
        /// <exception cref="WrongLinkTypeFault">if a link in the path points to the wrong type of item</exception>
        /// <exception cref="NotAuthorizedFault">thrown if the user does not have permission</exception>
        /// <exception cref="AuthenticationRequiredFault">If no user is logged in</exception>
        /// <exception cref="ExamAlreadyFinishedFault">thrown if the exam has already been finished</exception>
        /// <exception cref="NotExamTakerFault">If current user is not same user that stated the exam</exception>
        /// <exception cref="UnknownFault">On any other error</exception>

        [OperationContract]
        [FaultContract(typeof(PathFormatFault))]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(WrongLinkTypeFault))]
        [FaultContract(typeof(NotAuthorizedFault))]
        [FaultContract(typeof(AuthenticationRequiredFault))]
        [FaultContract(typeof(ConfigurationFault))]
        [FaultContract(typeof(UnknownFault))]

        void FinishAssessmentInstanceAudit(string path, Guid id, Guid? timerId, Dictionary<Guid, Guid> answeredQuestions, bool isExpired);

        /// <summary>
        /// Will select the chosen answer for the passed in question on an exam/evaluation that has already been started
        /// </summary>
        /// <remarks>
        /// NOTE: ReadCommitted is safe because AnsweredQuestionExamInstanceAudit() does not compute any persistent
        /// values based on the values read from the DB -- it just overwrites whatever is there.
        /// </remarks>
        /// <param name="path">Path of the assessment link</param>
        /// <param name="questionId">the question Id to choose an answer for</param>
        /// <param name="selectedAnswerId">the answer Id that was chosen</param>
        /// <param name="id">Guid Id of this exam instance, used to find exam instance in audits</param>
        /// <param name="sequenceNumber">Sequence number for the answer</param>
        /// <exception cref="PathFormatFault">if there are syntactive problems with the path</exception>
        /// <exception cref="NotFoundFault">if there is no item corresponding to the path</exception>
        /// <exception cref="WrongLinkTypeFault">if a link in the path points to the wrong type of item</exception>
        /// <exception cref="NotAuthorizedFault">thrown if the user does not have permission</exception>
        /// <exception cref="AuthenticationRequiredFault">if no user is logged in</exception>
        /// <exception cref="ConfigurationFault">for wrong service configuration</exception>
        /// <exception cref="UnknownFault">In any other cases.</exception>

        [OperationContract]
        [FaultContract(typeof(PathFormatFault))]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(WrongLinkTypeFault))]
        [FaultContract(typeof(NotAuthorizedFault))]
        [FaultContract(typeof(AuthenticationRequiredFault))]
        [FaultContract(typeof(ConfigurationFault))]
        [FaultContract(typeof(UnknownFault))]

        void AnsweredQuestionExamInstanceAudit(string path, Guid questionId, Guid selectedAnswerId, Guid id, int sequenceNumber);

        /// <summary>
        /// Gets an exam instance audit, the user who took the exam, whether the exam can be overridden, and the current
        /// state of the registration that created the exam. If the registration state is null, then there is no registration
        /// audit associated with this exam, so the state cannot be determined.
        /// </summary>
        /// <param name="coursePath">path to nav page exam is on</param>
        /// <param name="instanceId">ExamInstanceAudit uniqueInstaceId</param>
        /// <param name="auditDetailLevel">Detail level for the <c>ExamInstanceAudit</c>.</param>
        /// <returns>Tuple<ExamInstanceAudit,UserItem,bool(canOverrideExam),RegistrationState?></returns>
        /// <exception cref="NotAuthorizedException">thrown if the user does not have permission</exception>
        /// <exception cref="UnknownFault">In any other cases.</exception>

        ////////[OperationContract]
        ////////[FaultContract(typeof(NotAuthorizedFault))]
        ////////[FaultContract(typeof(UnknownFault))]

        ////////Tuple<AssessmentAudit, bool, RegistrationState?, bool> GetAssessmentInstanceReportInfo(string coursePath, Guid instanceId, string auditDetailLevel);

        /// <summary>
        /// Get user display name for given userId. If no user found then returned user display name for ANONYMOUS_USER_ID.
        /// </summary>
        /// <param name="coursePath">Course path</param>
        /// <param name="userId">user Id</param>
        /// <returns>User Display Name</returns>

        [OperationContract]
        [FaultContract(typeof(NotAuthorizedFault))]
        [FaultContract(typeof(UnknownFault))]
        string GetUserDisplayName(string coursePath, Guid userId);

        /// <summary>
        /// Creates the exam instance for a given path to a StudentEvaluationLink retrieved from a StudentEvaluatioViewLink.
        /// <para>
        /// This method is called if there is no existing ExamInstance and an blank evaluation needs to be
        /// generated.
        /// </para>
        /// </summary>
        /// <param name="examPath">path to StudentEvaluationLink.</param>
        /// <returns>An exam instance audit record</returns>
        /// <exception cref="PathFormatFault">if there are syntactive problems with the path</exception>
        /// <exception cref="NotFoundFault">if there is no item corresponding to the path</exception>
        /// <exception cref="WrongLinkTypeFault">if a link in the path points to the wrong type of item</exception>
        /// <exception cref="NotAuthorizedFault">if the current user does not own </exception>

        ////////[OperationContract]
        ////////[FaultContract(typeof(NotAuthorizedFault))]
        ////////[FaultContract(typeof(UnknownFault))]

        ////////EvaluationInstanceAudit GenerateEvaluationInstanceAuditPreviewByPath(string examPath);

        /// <summary>
        /// Will update the grade for this instance of this exam 
        /// </summary>
        /// <param name="navPath">Path to NavPage exam is on</param>
        /// <param name="id">unique instance Id of ExamInstaceAudit that the score will be changed on</param>
        /// <param name="newScore">new score</param>
        /// <exception cref="NotAuthorizedFault">if the current user does not own </exception>
        /// <exception cref="UnknownFault">In any other cases.</exception>

        [OperationContract]
        [FaultContract(typeof(NotAuthorizedFault))]
        [FaultContract(typeof(UnknownFault))]

        void OverrideExamInstaceGrade(string navPath, Guid id, int newScore);

        /// <summary>
        /// Remove overridden exam grade. Will set IsOverridden to false
        /// </summary>
        /// <param name="navPath">current nav path</param>
        /// <param name="id">unique instance Id of exam to remove overridden score on</param>
        /// <exception cref="NotAuthorizedFault">if the current user does not own </exception>
        /// <exception cref="UnknownFault">In any other cases.</exception>

        [OperationContract]
        [FaultContract(typeof(NotAuthorizedFault))]
        [FaultContract(typeof(UnknownFault))]

        void RemoveOverriddenExamGrade(string navPath, Guid id);



        /// <summary>
        /// TODO: GetQuestionPerformanceData and exception descriptions.
        /// </summary>
        /// <param name="navPath">current nav path</param>
        /// <param name="categoryFilter">Guid category filter</param>
        /// <param name="pageInfo">paging info</param>
        /// <param name="overridefilter">override filter</param>
        /// <param name="textFilter">text filter</param>
        /// <param name="sort">sort by</param>
        /// <param name="sortDirection">Sort Direction</param>
        /// <returns>CollectionSubset<QuestionPerformanceInfo></returns>
        /// <exception cref="NotFoundFault">If the item doesn't exist, or the exam collection is not present</exception>
        /// <exception cref="PathFormatFault">If the path is not valid</exception>
        /// <exception cref="NotANavPageFault">If the item does exist, but it is not a NavPage.</exception>
        /// <exception cref="WrongLinkTypeFault">If a link in the path points to the wrong type of item</exception>
        /// <exception cref="ConfigurationFault">Some error with the DB.</exception>
        /// <exception cref="AuthenticationRequiredFault">if the user is not authenticated.</exception>
        /// <exception cref="NotAuthorizedFault">if the user does not have permission to get the question performance data</exception>
        /// <exception cref="UnknownFault">For any other error.</exception>

        //////[OperationContract]
        //////[FaultContract(typeof(NotFoundFault))]
        //////[FaultContract(typeof(PathFormatFault))]
        //////[FaultContract(typeof(NotANavPageFault))]
        //////[FaultContract(typeof(WrongLinkTypeFault))]
        //////[FaultContract(typeof(ConfigurationFault))]
        //////[FaultContract(typeof(AuthenticationRequiredFault))]
        //////[FaultContract(typeof(NotAuthorizedFault))]
        //////[FaultContract(typeof(UnknownFault))]

        //////CollectionSubset<QuestionPerformanceInfo> GetQuestionPerfomanceData(string navPath, Guid? categoryFilter, PageInfo pageInfo, QuestionPerformanceOverrideFilter overridefilter,
        //////    string textFilter, QuestionPerformanceSort sort, SortDirectionEnum sortDirection);

        /// <summary>
        /// Gets list of question categories that questions belong to
        /// </summary>
        /// <param name="navPath">nav path</param>
        /// <returns>list of QuestionCategroies</returns>
        /// <exception cref="NotFoundFault">If the item doesn't exist, or the exam collection is not present</exception>
        /// <exception cref="NotANavPageFault">If the item does exist, but it is not a NavPage.</exception>
        /// <exception cref="PathFormatFault">If the path is not valid</exception>
        /// <exception cref="WrongLinkTypeFault">If a link in the path points to the wrong type of item</exception>
        /// <exception cref="NotAuthorizedFault">if the user does not have permission to get exams</exception>
        /// <exception cref="ConfigurationFault">Some error with the DB.</exception>
        /// <exception cref="UnknownFault">For any other error.</exception>

        //////////[OperationContract]
        //////////[FaultContract(typeof(NotFoundFault))]
        //////////[FaultContract(typeof(PathFormatFault))]
        //////////[FaultContract(typeof(NotANavPageFault))]
        //////////[FaultContract(typeof(WrongLinkTypeFault))]
        //////////[FaultContract(typeof(NotAuthorizedFault))]
        //////////[FaultContract(typeof(ConfigurationFault))]
        //////////[FaultContract(typeof(UnknownFault))]

        //////////List<QuestionCategory> GetQuestionCategoriesForPerfomanceReport(string navPath);

        /// <summary>
        /// Gets graph data for exam.
        /// </summary>
        /// <param name="navPath">current nav path</param>
        /// <param name="examId">exam to get graph data for</param>
        /// <returns></returns>
        /// <exception cref="NotFoundFault">If the item doesn't exist, or the exam collection is not present</exception>
        /// <exception cref="PathFormatFault">If the path is not valid</exception>
        /// <exception cref="NotANavPageFault">If the item does exist, but it is not a NavPage.</exception>
        /// <exception cref="WrongLinkTypeFault">If a link in the path points to the wrong type of item</exception>
        /// <exception cref="NotAuthorizedFault">if the user does not have permission to get exams</exception>
        /// <exception cref="ConfigurationFault">Some error with the DB.</exception>
        /// <exception cref="AuthenticationRequiredFault">if the user is not authenticated.</exception>
        /// <exception cref="UnknownFault">For any other error.</exception>

        [OperationContract]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(PathFormatFault))]
        [FaultContract(typeof(NotANavPageFault))]
        [FaultContract(typeof(WrongLinkTypeFault))]
        [FaultContract(typeof(NotAuthorizedFault))]
        [FaultContract(typeof(ConfigurationFault))]
        [FaultContract(typeof(AuthenticationRequiredFault))]
        [FaultContract(typeof(UnknownFault))]

        ExamPerformanceGraphData GetExamPerformanceGraphData(string navPath, Guid examId);

        /// <summary>
        /// Gets graph data for question performance.
        /// </summary>
        /// <param name="navPath">current nav path</param>
        /// <param name="questionVersionId">question version to get graph data for</param>
        /// <returns>QuestionPerformanceGraphData</returns>
        /// <exception cref="NotFoundFault">If the item doesn't exist, or the exam collection is not present</exception>
        /// <exception cref="PathFormatFault">If the path is not valid</exception>
        /// <exception cref="NotANavPageFault">If the item does exist, but it is not a NavPage.</exception>
        /// <exception cref="WrongLinkTypeFault">If a link in the path points to the wrong type of item</exception>
        /// <exception cref="NotAuthorizedFault">if the user does not have permission to get exams</exception>
        /// <exception cref="ConfigurationFault">Some error with the DB.</exception>
        /// <exception cref="AuthenticationRequiredFault">if the user is not authenticated.</exception>
        /// <exception cref="UnknownFault">For any other error.</exception>

        [OperationContract]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(PathFormatFault))]
        [FaultContract(typeof(NotANavPageFault))]
        [FaultContract(typeof(WrongLinkTypeFault))]
        [FaultContract(typeof(NotAuthorizedFault))]
        [FaultContract(typeof(ConfigurationFault))]
        [FaultContract(typeof(AuthenticationRequiredFault))]
        [FaultContract(typeof(UnknownFault))]

        QuestionPerformanceGraphData GetQuestionPerformanceGraphData(string navPath, string questionVersionId);


        /// <summary>
        /// Will allow user to continue exam instance
        /// </summary>
        /// <param name="path">nav path</param>
        /// <param name="id">unique instance Id of exam instance audit to change</param>
        /// 
        [OperationContract]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(PathFormatFault))]
        [FaultContract(typeof(NotANavPageFault))]
        [FaultContract(typeof(WrongLinkTypeFault))]
        [FaultContract(typeof(NotAuthorizedFault))]
        [FaultContract(typeof(ConfigurationFault))]
        [FaultContract(typeof(UnknownFault))]

        void AllowUserToContinueExam(string path, Guid id);

        /// <summary>
        /// Will allow user to take exam again and cancel passed in instance
        /// </summary>
        /// <param name="path"></param>
        /// <param name="id"></param>
        [OperationContract]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(PathFormatFault))]
        [FaultContract(typeof(NotANavPageFault))]
        [FaultContract(typeof(WrongLinkTypeFault))]
        [FaultContract(typeof(NotAuthorizedFault))]
        [FaultContract(typeof(ConfigurationFault))]
        [FaultContract(typeof(UnknownFault))]

        void AllowUserToReTakeExam(string path, Guid id);

        /// <summary>
        /// Get paged Registration Report items.
        /// </summary>

        [OperationContract]
        [FaultContract(typeof(PathFormatFault))]
        [FaultContract(typeof(NotANavPageFault))]
        [FaultContract(typeof(WrongLinkTypeFault))]
        [FaultContract(typeof(NotAuthorizedFault))]
        [FaultContract(typeof(ConfigurationFault))]
        [FaultContract(typeof(AuthenticationRequiredFault))]
        [FaultContract(typeof(UnknownFault))]
        CollectionSubset<RegistrationReportItem> GetRegistrationReportItems(string path, string textFilter, RegistrationReportStatusFilter statusFilter, RegistrationReportHasSessionAuditsFilter hasSessionAuditsFilter, List<RegistrationReportItemSort> sortItems, SortDirectionEnum sortDirection, PageInfo pageInfo, FilterReportOptions filterReportOptions);

        /// <summary>
        /// Get paged student grade items, used for student grade report.
        /// </summary>
        /// <param name="path">current path location</param>
        /// <param name="textFilter">text filter for user. Based on first name, last name, username and email</param>
        /// <param name="sort">current sort order</param>
        /// <param name="pageInfo">paging info</param>
        /// <returns>collection subset of student grade items</returns>
        /// <exception cref="NotFoundFault">If the item doesn't exist, or the exam collection is not present</exception>
        /// <exception cref="NotANavPageFault">If the item does exist, but it is not a NavPage.</exception>
        /// <exception cref="PathFormatFault">If the path is not valid</exception>
        /// <exception cref="WrongLinkTypeFault">If a link in the path points to the wrong type of item</exception>
        /// <exception cref="NotAuthorizedFault">if the user does not have permission to get exams</exception>
        /// <exception cref="ConfigurationFault">Some error with the DB.</exception>
        /// <exception cref="UnknownFault">For any other error.</exception>
        /// <exception cref="OrganizationNotFoundFault">if path is not correspond to an organization</exception>

        //////[OperationContract]
        //////[FaultContract(typeof(NotFoundFault))]
        //////[FaultContract(typeof(PathFormatFault))]
        //////[FaultContract(typeof(NotANavPageFault))]
        //////[FaultContract(typeof(WrongLinkTypeFault))]
        //////[FaultContract(typeof(NotAuthorizedFault))]
        //////[FaultContract(typeof(ConfigurationFault))]
        //////[FaultContract(typeof(UnknownFault))]
        //////[FaultContract(typeof(AuthenticationRequiredFault))]
        //////[FaultContract(typeof(OrganizationNotFoundFault))]

        //////CollectionSubset<StudentGradeItem> GetStudentGradeReportItems(string path, string textFilter, StudentGradesSort sort, SortDirectionEnum sortDirection, PageInfo pageInfo, FilterReportOptions filterReportOptions);

        /// <summary>
        /// Gets all student grade report detailed items. for exporting to csv
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="textFilter">The text filter.</param>
        /// <param name="sort">The sort.</param>
        /// <returns></returns>
        /// <exception cref="NotFoundFault">If the item doesn't exist, or the exam collection is not present</exception>
        /// <exception cref="NotANavPageFault">If the item does exist, but it is not a NavPage.</exception>
        /// <exception cref="PathFormatFault">If the path is not valid</exception>
        /// <exception cref="WrongLinkTypeFault">If a link in the path points to the wrong type of item</exception>
        /// <exception cref="NotAuthorizedFault">if the user does not have permission to get exams</exception>
        /// <exception cref="ConfigurationFault">Some error with the DB.</exception>
        /// <exception cref="UnknownFault">For any other error.</exception>

        //////[OperationContract]
        //////[FaultContract(typeof(NotFoundFault))]
        //////[FaultContract(typeof(PathFormatFault))]
        //////[FaultContract(typeof(NotANavPageFault))]
        //////[FaultContract(typeof(WrongLinkTypeFault))]
        //////[FaultContract(typeof(NotAuthorizedFault))]
        //////[FaultContract(typeof(ConfigurationFault))]
        //////[FaultContract(typeof(UnknownFault))]
        //////[FaultContract(typeof(AuthenticationRequiredFault))]

        //////List<StudentGradeItemDetail> GetAllStudentGradeReportDetailedItems(string path, string textFilter, List<StudentGradesDetailSortEntry> sort, FilterReportOptions filterReportOptions, DateTime? startDate, DateTime? endDate);

        /// <summary>
        /// Endpoint verification function for installer. 
        /// </summary>
        /// <returns>true</returns>

        [OperationContract]

        bool IsAuditServiceSecure();


        /// <summary>
        /// Create a manual certificate audit.
        /// </summary>
        /// <param name="path">path to course link offering is on</param>
        /// <param name="userId">Id of user to create certificate for</param>
        /// <param name="certificateShortId">The certificate short identifier.</param>
        /// <param name="offeringId">The offering identifier.</param>

        [OperationContract]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(WrongLinkTypeFault))]
        [FaultContract(typeof(UnknownFault))]
        [FaultContract(typeof(PathFormatFault))]
        [FaultContract(typeof(NotANavPageFault))]
        [FaultContract(typeof(NotAuthorizedFault))]
        [FaultContract(typeof(ConfigurationFault))]

        void CreateCertificateAudit(string path, Guid userId, string certificateShortId, Guid offeringId);

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


        /// <summary>
        /// Generate pdf certificates by path, start date and end date
        /// </summary>
        /// <param name="path"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>

        [OperationContract]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(UnknownFault))]
        [FaultContract(typeof(NotAuthorizedFault))]


        Stream BulkGetCertificatesByPathAndDate(string path, DateTime? startDate, DateTime? endDate);

        /// <summary>
        /// This method counts the number of awarded certificates between a start date and an end date.
        /// It is used to prevent downloading certificates when there is a large number of certificates to download(larger than appSettings.CertificatesMaxDownload).
        /// </summary>
        /// <param name="path"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>

        [OperationContract]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(UnknownFault))]
        [FaultContract(typeof(NotAuthorizedFault))]

        int CountAwardedCertificatesByDate(string path, DateTime? startDate, DateTime? endDate);

        /// <summary>
        /// Generate pdf certificates for a given offering Id.
        /// the zip folder is created in EC\TempFilesCoreSvc.
        /// </summary>
        /// <param name="coursePath"></param>
        /// <param name="offeringId"></param>

        [OperationContract]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(UnknownFault))]
        [FaultContract(typeof(NotAuthorizedFault))]
        Stream BulkGetCertificatesByOffering(string coursePath, Guid offeringId);

        /// <summary>
        /// This method counts the number of awarded certificates for a given offering.
        /// It is used to prevent downloading certificates when there is a large number of them to download(larger than appSettings.CertificatesMaxDownload).
        /// </summary>

        [OperationContract]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(UnknownFault))]
        [FaultContract(typeof(NotAuthorizedFault))]

        int CountAwardedCertificatesByOffering(string coursePath, Guid offeringId);

    }
}
