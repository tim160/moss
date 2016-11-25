using System;
using System.Collections.Generic;
using System.ServiceModel;
using EC.Constants;
using EC.Service.DTO;
using EC.Errors;
using EC.Errors.ECExceptions;
using EC.Errors.CommonExceptions;
using EC.Errors.RegistrationExceptions;
using EC.Errors.UserExceptions;
using EC.Errors.FileExceptions;

namespace EC.Service.Interfaces
{
    /// <summary>
    /// Service interface for registrations and course offerings for admin-use only. 
    /// No permission check is done! 
    /// </summary>
    /// <remarks>
    /// This interface must not be made available from outside!
    /// </remarks>

    [ServiceContract]
    public interface IRegistrationServiceAdmin
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
        /// <exception cref="UnknownFault">If any other error occurs.</exception>

        [OperationContract]
        [FaultContract(typeof(AuthenticationRequiredFault))]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(NotACourseFault))]
        [FaultContract(typeof(UnknownFault))]

        bool? IsCurrentlyRegisteredByPath(string coursePath);

        /// <summary>
        /// Get instructors for the course offering (paged).
        /// </summary>
        ///  /// <remarks>
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
        /// <exception cref="UnknownFault">If any other error occurs.</exception>

        [OperationContract]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(NotACourseFault))]
        [FaultContract(typeof(UnknownFault))]

        CollectionSubset<User> GetInstructorsForCourseOfferingByPath(string coursePath, Guid courseOfferingId, PageInfo pageInfo, string detailLevel);


        /// <summary>
        /// Delete either an active or inactive course offering. If the course offering is already deleted - do nothing.
        /// This marks a course offering as deleted and remain in the DB.
        /// </summary>
        /// <remarks>
        /// Use <see cref="KillCourseOfferingByPath"/> to really delete a course offering.
        /// </remarks>
        /// <param name="coursePath">Course path to which the course offering belongs to</param>
        /// <param name="courseOfferingId">Course offering id.</param>
        /// <exception cref="NotFoundFault">If the course or course offering doesn't exist.</exception>
        /// <exception cref="NotACourseFault">If the path is not a course.</exception>
        /// <exception cref="UnknownFault">Any other error.</exception>

        [OperationContract]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(NotACourseFault))]
        [FaultContract(typeof(UnknownFault))]

        void DeleteCourseOfferingByPath(string coursePath, Guid courseOfferingId);

        /// <summary>
        /// Really delete either an active/inactive or already deleted course offering. 
        /// The course offering is deleted from the DB.
        /// </summary>
        /// <remarks>
        /// Use with caution!
        /// Some reports may not be available after deleting a course offering from the DB.
        /// </remarks>
        /// <param name="coursePath">Course path to which the course offering belongs to</param>
        /// <param name="courseOfferingId">Course offering id.</param>
        /// <exception cref="NotFoundFault">If the course or course offering doesn't exist.</exception>
        /// <exception cref="NotACourseFault">If the path is not a course.</exception>
        /// <exception cref="UnknownFault">Any other error.</exception>

        [OperationContract]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(NotACourseFault))]
        [FaultContract(typeof(UnknownFault))]

        void KillCourseOfferingByPath(string coursePath, Guid courseOfferingId);

        /// <summary>
        /// Register a student in a course offering.
        /// </summary>
        /// <param name="coursePath">Course path for the course offering.</param>
        /// <param name="courseOfferingId">The active course offering.</param>
        /// <param name="userId">the user</param>
        /// <param name="registrationParamters">Registration parameter needed for the registration policy this offering has assigned.</param>
        /// <exception cref="AlreadyRegisteredFault">If the student is already registered for this course offering</exception>
        /// <exception cref="OverlappingRegistrationFault">If the student is already registered for an overlapping offering</exception>
        /// <exception cref="UserFault">If the user doesn't exist.</exception>
        /// <exception cref="NotFoundFault">If the active course offering does not exist or the course does not exist.</exception>
        /// <exception cref="NotACourseFault">If the course path is not a course.</exception>
        /// <exception cref="NotAuthorizedFault">If the current user is not allowed to execute/view this action.</exception>
        /// <exception cref="UnknownFault">Any other error.</exception>

        [OperationContract]
        [FaultContract(typeof(AlreadyRegisteredFault))]
        [FaultContract(typeof(OverlappingRegistrationFault))]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(NotACourseFault))]
        [FaultContract(typeof(UnknownFault))]
        [FaultContract(typeof(RegistrationAgreementFailureFault))]

        void RegisterStudentToCourseOffering(string coursePath, Guid courseOfferingId, Guid userId, IDictionary<string, string> registrationParamters);

        /// <summary>
        /// Add an instructor to a course offering (either active or inactive). 
        /// </summary>
        /// <param name="coursePath">Course path</param>
        /// <param name="courseOfferingId"></param>
        /// <param name="instructorId">User Id to add.</param>
        /// <exception cref="AlreadyExistsFault">If the instructor is already instructor for this course offering.</exception>
        /// <exception cref="NotACourseFault">If the path is not a course.</exception>
        /// <exception cref="NotFoundFault">If the course or course offering doesn't exist.</exception>
        /// <exception cref="UserFault">If the instructor doesn't exist.</exception>
        /// <exception cref="UnknownFault">Any other error.</exception>

        [OperationContract]
        [FaultContract(typeof(AlreadyExistsFault))]
        [FaultContract(typeof(NotACourseFault))]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(UserFault))]
        [FaultContract(typeof(UnknownFault))]

        void AddInstructorToCourseOfferingByPath(string coursePath, Guid courseOfferingId, Guid instructorId);

        /// <summary>
        /// Remove instructor from a course offering (either active or inactive). 
        /// Do nothing if the instructor isn't an instructor for the course offering (e.g. if remove is called twice).
        /// </summary>
        /// <param name="coursePath">Course path.</param>
        /// <param name="courseOfferingId">Course offering id.</param>
        /// <param name="instructorId">Instructor to remove.</param>
        /// <exception cref="NotFoundFault">If the course or course offering doesn't exist.</exception>
        /// <exception cref="NotACourseFault">If the path is not a course.</exception>
        /// <exception cref="UnknownFault">Any other error.</exception>

        [OperationContract]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(NotACourseFault))]
        [FaultContract(typeof(UnknownFault))]

        void RemoveInstructorFromCourseOfferingByPath(string coursePath, Guid courseOfferingId, Guid instructorId);

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
        /// <exception cref="UnknownFault">If any error occurs.</exception>

        [OperationContract]
        [FaultContract(typeof(AttributeNotFoundFault))]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(NotACourseFault))]
        [FaultContract(typeof(UnknownFault))]

        string GetRegistrationPageForPolicy(string coursePath, string policyName);


        /// <summary>
        /// Endpoint verification function for installer. 
        /// </summary>
        /// <returns>true</returns>
        [OperationContract]

        bool IsRegistrationServiceAdmin();


        /// <summary>
        /// Gets the users registrations for course (used by command line)
        /// TODO: This should return DTOs and the command line should build a string.
        /// </summary>
        /// <param name="coursePath">The course path.</param>
        /// <param name="username">the users username.</param>
        /// <returns></returns>
        /// <exception cref="NotFoundException">
        /// Could not find course by path
        /// or
        /// Could not find user by userId
        /// </exception>
        
        [OperationContract]
        [FaultContract(typeof(UnknownFault))]
        [FaultContract(typeof(NotFoundFault))]
        
        string GetUsersRegistrations(string coursePath, string username);

        /// <summary>
        /// Changes the state of the registration.
        /// </summary>
        /// <param name="registrationId">The registration identifier.</param>
        /// <param name="newState">The new state.</param>
        /// <exception cref="NotFoundException">Could not find registration by Id.</exception>
        [OperationContract]
        [FaultContract(typeof(UnknownFault))]
        [FaultContract(typeof(NotFoundFault))]
        void ChangeRegistrationState(Guid registrationId, RegistrationState newState);
    }
}
