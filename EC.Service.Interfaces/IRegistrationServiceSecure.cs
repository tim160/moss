using System;
using System.Collections.Generic;
using System.ServiceModel;
using EC.Service.DTO;
using EC.Constants;
using EC.Errors;
using EC.Errors.CommonExceptions;
using EC.Errors.ECExceptions;
using EC.Errors.RegistrationExceptions;
using EC.Errors.UserExceptions;

namespace EC.Service.Interfaces
{   
    /// <summary>
    /// Service interface for registrations and course offerings.
    /// This service checks permissions for every method whether a user is allowed to execute the task.
    /// </summary>

    [ServiceContract]
    public interface IRegistrationServiceSecure
    {
        /// <summary>
        /// Check if the logged in user is registered at any active course offering of a certain course (indicated by path).
        /// This doesn't check the capabilities.
        /// </summary>
        /// <param name="coursePath">Absolute path of the course</param>
        /// <returns> 
        /// Return <c>true</c> if the user is already registered for an active course offering and therefore can't register again.
        /// Return <c>false</c> if there are active course offerings and the user is not registered yet.
        /// Return <c>null</c> if no active open course offering exists (not possible to check if the user is already registered).
        /// </returns>
        /// <exception cref="AuthenticationRequiredFault">If no user is logged in - logon is required first.</exception>
        /// <exception cref="NotFoundFault">If the course or course offering doesn't exist.</exception>
        /// <exception cref="NotACourseFault">If the path is not a course.</exception>
        /// <exception cref="NotAuthorizedFault">If the current user is not allowed to execute/view this action.</exception>
        /// <exception cref="UnknownFault">If any other error occurs.</exception>

        [OperationContract] 
        [FaultContract(typeof(AuthenticationRequiredFault))]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(NotACourseFault))]
        [FaultContract(typeof(NotAuthorizedFault))]
        [FaultContract(typeof(UnknownFault))]
               
        bool? IsCurrentlyRegisteredByPath(string coursePath);


        /// <summary>
        /// Get a list of user ids with active and inactive registration for <paramref name="offeringId"/>
        /// </summary>
        /// <returns>list of user ids</returns>

        [OperationContract]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(UnknownFault))]

        List<Guid> GetUserIdsWithActiveAndInActiveRegistrationsByOfferingId(string coursePath, Guid offeringId);

        /// <summary>
        /// Get a dictionary mapping user ids with display names for all users with registrations in <paramref name="offeringId"/>
        /// </summary>
        /// <returns>dictionary mapping user ids with display names</returns>

        [OperationContract]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(UnknownFault))]

        Dictionary<Guid, string> GetUserIdsAndDisplayNamesWithRegistrationsForOffering(string coursePath, Guid offeringId);

        /// <summary>
        /// Get users (students) with active registrations for a course at or along the <paramref name="path"/>.
        /// </summary>
        /// <remarks>
        /// Only return currently active student registrations across all offerings of a course.
        /// The returned result will be filtered according to the capabilities of the currently logged in user:
        /// 1. If the user has the capability CanEvaluateStudent all active student registrations are returned
        /// 2. If the user doesn't have the capability CanEvaluateStudent only active student registrations are returned in which the user is instructor for the offering
        /// 3. If the user has the capability and is instructor - all active registrations (instructor or not) are returned 
        /// <para>
        /// The <c>StartIndex</c> will skip the first <c>StartIndex</c> elements.
        /// To deactivate Paging set <c>PageSize</c> to -1 and <c>StartIndex</c> to 0 - all elements are returned.
        /// If the <c>PageSize</c> = -1 and <c>StartIndex</c> > 0 all items, starting with <c>StartIndex</c> are returned.
        /// </para>
        /// <para>
        /// Detail level:
        /// - Registrations ... get active registrations of the user
        /// - Registrations.Offering ... include the offering per registration
        /// </para>
        /// </remarks>
        /// <param name="path">Path at a course or below a course</param>
        /// <param name="pageInfo">range of list to fetch. If <c>null</c>, all registrations at once.</param>
        /// <param name="sortDirection">Sort direction</param>
        /// <param name="columnToSortBy">Column to sort the registrations</param>
        /// <param name="filterText">Filter by student's first/last name or portions. Set <c>null</c> not to filter any student</param>
        /// <param name="detailLevel">Detail level to include in the DTO</param>
        /// <param name="excludeCurrentUser">Flag to indicate whether to exclude current user</param>
        /// <returns>Return a paged list of users with optional list of their registrations</returns>
        /// <exception cref="NotFoundFault">If the <paramref name="path"/> doesn't exist.</exception>
        /// <exception cref="NotACourseFault">If no course along the path can be found.</exception>
        /// <exception cref="NotAuthorizedFault">If the current user is not allowed to execute/view this action.</exception>
        /// <exception cref="AuthenticationRequiredFault">If no user is logged in - login required</exception>
        /// <exception cref="UnknownFault">If any other error occurs.</exception>

        [OperationContract]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(NotACourseFault))]
        [FaultContract(typeof(NotAuthorizedFault))]
        [FaultContract(typeof(AuthenticationRequiredFault))]
        [FaultContract(typeof(UnknownFault))]

        CollectionSubset<User> GetStudentsWithRegistrationsByPath(string path, PageInfo pageInfo, SortDirectionEnum sortDirection, string columnToSortBy,
            string filterText, string detailLevel, bool excludeCurrentUser);

        /// <summary>
        /// Get instructors for the course offering (paged).
        /// </summary>
        /// <remarks>
        /// The <c>StartIndex</c> will skip the first <c>StartIndex</c> elements.
        /// To deactivate Paging set <c>PageSize</c> to -1 and <c>StartIndex</c> to 0 - all elements are returned.
        /// If the <c>PageSize</c> = -1 and <c>StartIndex</c> > 0 all items, starting with <c>StartIndex</c> are returned.
        /// </remarks>
        /// <param name="coursePath">Course path to which the course offering belongs to.</param>
        /// <param name="courseOfferingId">Course offering Id.</param>
        /// <param name="pageInfo">range of list to fetch. If <c>null</c>, all instructors.</param>
        /// <param name="detailLevel">Detail level to get the users.</param>
        /// <returns>Return a paged list of instructors for a specified course offering.</returns>
        /// <exception cref="NotFoundFault">If the course or course offering doesn't exist.</exception>
        /// <exception cref="NotACourseFault">If the path is not a course.</exception>
        /// <exception cref="NotAuthorizedFault">If the current user is not allowed to execute/view this action.</exception>
        /// <exception cref="UnknownFault">If any other error occurs.</exception>

        [OperationContract]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(NotACourseFault))]
        [FaultContract(typeof(NotAuthorizedFault))]
        [FaultContract(typeof(UnknownFault))]
        
        CollectionSubset<User> GetInstructorsForCourseOfferingByPath(string coursePath, Guid courseOfferingId, PageInfo pageInfo, string detailLevel);


        /// <summary>
        /// Delete either an active or inactive course offering. 
        /// If the course offering is already deleted - do nothing.
        /// This marks a course offering as deleted and remain in the DB.
        /// </summary>
        /// <param name="coursePath">Course path to which the course offering belongs to</param>
        /// <param name="offeringId">Course offering id.</param>
        /// <exception cref="NotFoundFault">If the course or course offering doesn't exist.</exception>
        /// <exception cref="NotACourseFault">If the path is not a course.</exception>
        /// <exception cref="NotAuthorizedFault">If the current user is not allowed to execute/view this action.</exception>
        /// <exception cref="UnknownFault">Any other error.</exception>
        
        [OperationContract]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(NotACourseFault))]
        [FaultContract(typeof(NotAuthorizedFault))]
        [FaultContract(typeof(UnknownFault))]
        
        void DeleteCourseOfferingByPath(string coursePath, Guid offeringId);

        /// <summary>
        /// Register a student in a course offering.
        /// </summary>
        /// <param name="coursePath">Course path for the course offering.</param>
        /// <param name="offeringId">The active course offering.</param>
        /// <param name="userId">the user</param>
        /// <param name="registrationParamters">Registration parameter needed for the registration policy this offering has assigned.</param>
        /// <exception cref="AlreadyRegisteredFault">If the student is already registered for this course offering</exception>
        /// <exception cref="OverlappingRegistrationFault">If the student is already registered for an overlapping offering</exception>
        /// <exception cref="UserFault">If the user doesn't exist.</exception>
        /// <exception cref="NotFoundFault">If the active course offering does not exist or the course does not exist.</exception>
        /// <exception cref="NotACourseFault">If the course path is not a course.</exception>
        /// <exception cref="NotAuthorizedFault">If the current user is not allowed to execute/view this action.</exception>
        /// <exception cref="UowIsolationFault">If retries failed to update the <c>code</c>/deadlock</exception>
        /// <exception cref="UnknownFault">Any other error.</exception>
        
        [OperationContract]
        [FaultContract(typeof(AlreadyRegisteredFault))]
        [FaultContract(typeof(OverlappingRegistrationFault))]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(NotACourseFault))]
        [FaultContract(typeof(UnknownFault))]
        [FaultContract(typeof(NotAuthorizedFault))]
        [FaultContract(typeof(RegistrationAgreementFailureFault))]
        [FaultContract(typeof(UowIsolationFault))]
        
        void RegisterStudentToCourseOffering(string coursePath, Guid offeringId, Guid userId, IDictionary<string, string> registrationParamters);
        
        /// <summary>
        /// Remove registered student from a course offering (either active or inactive). 
        /// Do nothing if the student hasn't been registered (e.g. double deregistering).
        /// </summary>
        /// <param name="coursePath">Course path of the offering.</param>
        /// <param name="offeringId">Course offering id</param>
        /// <param name="userId">Student id to remove.</param>
        /// <exception cref="NotFoundFault">If the active course offering or course doesn't exist.</exception>
        /// <exception cref="NotACourseFault">If the course path is not a course item</exception>
        /// <exception cref="NotAuthorizedFault">If the current user is not allowed to execute/view this action.</exception>
        /// <exception cref="UnknownFault">Any other error.</exception>
        
        [OperationContract]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(NotACourseFault))]
        [FaultContract(typeof(NotAuthorizedFault))]
        [FaultContract(typeof(UnknownFault))]
       
        void DeRegisterStudentFromCourseOfferingByPath(string coursePath, Guid offeringId, Guid userId, Guid RegistrationId);

        /// <summary>
        /// Add an instructor to a course offering (either active or inactive). 
        /// </summary>
        /// <param name="instructorId">User Id to add.</param>
        /// <exception cref="AlreadyExistsFault">If the instructor is already instructor for this course offering.</exception>
        /// <exception cref="NotACourseFault">If the path is not a course.</exception>
        /// <exception cref="NotFoundFault">If the course or course offering doesn't exist.</exception>
        /// <exception cref="UserFault">If the instructor doesn't exist.</exception>
        /// <exception cref="NotAuthorizedFault">If the current user is not allowed to execute/view this action.</exception>
        /// <exception cref="UnknownFault">Any other error.</exception>

        [OperationContract]
        [FaultContract(typeof(AlreadyExistsFault))]
        [FaultContract(typeof(NotACourseFault))]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(UserFault))]
        [FaultContract(typeof(NotAuthorizedFault))]
        [FaultContract(typeof(UnknownFault))]
       
        void AddInstructorToCourseOfferingByPath(string coursePath, Guid offeringId, Guid instructorId);

        /// <summary>
        /// Remove instructor from a course offering (either active or inactive). 
        /// Do nothing if the instructor isn't an instructor for the course offering (e.g. if remove is called twice).
        /// </summary>
        /// <param name="coursePath">Course path.</param>
        /// <param name="offeringId">Course offering id.</param>
        /// <param name="instructorId">Instructor to remove.</param>
        /// <exception cref="NotFoundFault">If the course or course offering doesn't exist.</exception>
        /// <exception cref="NotACourseFault">If the path is not a course.</exception>
        /// <exception cref="NotAuthorizedFault">If the current user is not allowed to execute/view this action.</exception>
        /// <exception cref="UnknownFault">Any other error.</exception>

        [OperationContract]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(NotACourseFault))]
        [FaultContract(typeof(NotAuthorizedFault))]
        [FaultContract(typeof(UnknownFault))]
        
        void RemoveInstructorFromCourseOfferingByPath(string coursePath, Guid offeringId, Guid instructorId);

        /// <summary>
        /// Get the actual contents of the registration UI page for a given policy.
        /// </summary>
        /// <remarks>
        /// The registration UI page may be customized through attributes. An attribute of the
        /// form "System.Registration.UIPage.[policyName]=path" will point to a SimpleContent
        /// item for the page. This code looks upward in the tree from the given
        /// course for an attribute. The page must follow certain conventions with regard to
        /// the naming of form fields (which will depend on what type of policy the page
        /// is for).
        /// </remarks>
        /// <param name="coursePath">path to the course the registration is for</param>
        /// <param name="policyName">name of the policy</param>
        /// <returns>the actual contents of the UI page</returns>
        /// <exception cref="AttributeNotFoundFault">If no attribute for the registration page exists.</exception>
        /// <exception cref="NotFoundFault">If the course doesn't exist.</exception>
        /// <exception cref="NotACourseFault">If the path isn't a course.</exception>
        /// <exception cref="NotAuthorizedFault">If the current user is not allowed to execute/view this action.</exception>
        /// <exception cref="UnknownFault">If any error occurs.</exception>

        [OperationContract]
        [FaultContract(typeof(AttributeNotFoundFault))]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(NotACourseFault))]
        [FaultContract(typeof(NotAuthorizedFault))]
        [FaultContract(typeof(UnknownFault))]
        
        string GetRegistrationPageForPolicy(string coursePath, string policyName);


        /// <summary>
        /// Count numbers of offerings for a given course (paged).
        /// </summary>
        /// <param name="coursePath">Path to check for course offerings</param>
        /// <param name="activeOnly">
        /// Set <c>true</c> to only count active course offerings. 
        /// Set <c>false</c> to count all active and inactive course offerings (no deleted course offerings included).
        /// </param>
        /// <returns>Number of course offerings.</returns>
        /// <exception cref="NotFoundFault">If the course doesn't exist.</exception>
        /// <exception cref="NotACourseFault">If the path is not a course.</exception>
        /// <exception cref="NotAuthorizedFault">If the current user is not allowed to execute/view this action.</exception>
        /// <exception cref="UnknownFault">If any other error occurs.</exception>

        [OperationContract]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(NotACourseFault))]
        [FaultContract(typeof(NotAuthorizedFault))]
        [FaultContract(typeof(UnknownFault))]

        int CountCourseOfferingsByPath(string coursePath, bool activeOnly);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="offeringId">Guid Id of offering up change IsActive bit on</param>
        /// <param name="IsActive">value to change IsActive bit to</param>
        /// <exception cref="UnknownFault">If any error occurs.</exception>
        /// <exception cref="NotFoundFault">If Offering can not be found by Id</exception>

        [OperationContract]
        [FaultContract(typeof(UnknownFault))]
        [FaultContract(typeof(NotFoundFault))]

        void ChangeOfferingIsActive(string path,Guid offeringId, bool IsActive);

        /// <summary>
        /// Endpoint verification function for installer. 
        /// </summary>
        /// <returns>true</returns>
        [OperationContract]

        bool IsRegistrationServiceSecure();
    }
}
