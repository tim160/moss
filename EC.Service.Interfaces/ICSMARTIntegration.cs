using System;
using System.Collections.Generic;
using System.ServiceModel;
using EC.Errors;
using EC.Errors.CommonExceptions;
using EC.Errors.ECExceptions;
using EC.Errors.UserExceptions;
using EC.Service.DTO;

namespace EC.Service.Interfaces
{
    /// <summary>
    /// Created separate interface to be able to isolate CSMART 
    /// related methods that are so far spread out in our system
    /// </summary>

    [ServiceContract]
    public interface ICSMARTIntegration
    {
        /// <summary>
        /// Update a user identified by Id and also applies a new optional role.
        /// Update all line operating coordinators at the same time to keep in sync with the CSmart extensions service.
        /// </summary>
        /// <param name="path">Path where the role of the user is applied</param>
        /// <param name="groupDefinitions">All group definitions which should be replaced (the property <c>Path</c> in GroupDefinition is ignored).</param>
        /// <returns>Return updated user.</returns>
        /// <exception cref="PathFormatFault">If the path has a wrong format</exception>
        /// <exception cref="NotFoundFault">If <paramref name="path"/> or a user in <paramref name="groupDefinitions"/> doesn't exist.</exception>
        /// <exception cref="UnknownFault">Throw this exception in any other cases.</exception>

        [OperationContract]
        [FaultContract(typeof(PathFormatFault))]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(UnknownFault))]

        void ReplaceGroupDefinitionsByPath(string path, List<LocalGroupDefinition> groupDefinitions);

        /// <summary>
        /// CSMART-specific service call.
        /// 
        /// Update a user identified by Id and also applies a new optional <paramref name="newRole"/>.
        /// Update all line operating coordinators at the same time to keep in sync with the CSmart extensions service.
        /// </summary>
        /// <param name="orgPath">The organizational path for the user where the role of the user is applied</param>
        /// <param name="user">User to update (identified by Id)</param>
        /// <param name="newRole">
        /// Optional: 
        /// - Set new role (= group name the user is put into) at this <paramref name="orgPath"/>. 
        /// - Set to <c>null</c> if the user should be removed from all <paramref name="possibleRoles"/> at <paramref name="orgPath"/>.
        /// - Set to a group name <paramref name="newRole"/> to add user to this group and remove the user from any group in 
        /// <paramref name="possibleRoles"/>.
        /// </param>
        /// <returns>Return updated user.</returns>
        /// <param name="possibleRoles">All group names (=roles) the user can be member in (except LineAdmin and ExternalLineAdmin).</param>
        /// <param name="lineCoordinatorGroups">All line coordinator groups will be replaced with new users (the <c>Path</c> property in GroupDefinition is ignored).</param>
        /// <exception cref="NotFoundFault">If <paramref name="orgPath"/>, <paramref name="user"/> or a user in <paramref name="lineCoordinatorGroups"/> doesn't exist.</exception>
        /// <exception cref="ParameterValidationFault">If <paramref name="user"/> is <c>null</c></exception>
        /// <exception cref="ContainsWhiteSpacesFault">If any of the strings <paramref name="newRole"/> or <paramref name="possibleRoles"/> contains any white space</exception>
        /// <exception cref="UnknownFault">Throw this exception in any other cases.</exception>
        [OperationContract]
        [FaultContract(typeof(OrganizationLogonStateFault))]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(ParameterValidationFault))]
        [FaultContract(typeof(ContainsWhiteSpacesFault))]
        [FaultContract(typeof(UnknownFault))]
        [FaultContract(typeof(AlreadyOrgMemberFault))]

        User UpdateUserWithRoleById(string orgPath, User user, string newRole, List<string> possibleRoles,
            List<LocalGroupDefinition> lineCoordinatorGroups);


        /// <summary>
        /// Remove registered students from a course offering (either active or inactive). 
        /// Do nothing if the student hasn't been registered (e.g. double deregistering).
        /// CSMART METHOD
        /// </summary>
        /// <remarks>
        /// This a bagged function called from CSMART extensions service
        /// </remarks>
        /// <param name="coursePath">Course path of the offering.</param>
        /// <param name="courseOfferingId">Course offering id</param>
        /// <param name="userIds">Student ids to remove.</param>
        /// <exception cref="NotFoundFault">If the active course offering or course doesn't exist.</exception>
        /// <exception cref="NotACourseFault">If the course path is not a course item</exception>
        /// <exception cref="NotAuthorizedFault">If the current user is not allowed to execute/view this action.</exception>
        /// <exception cref="UnknownFault">Any other error.</exception>

        [OperationContract]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(NotACourseFault))]
        [FaultContract(typeof(NotAuthorizedFault))]
        [FaultContract(typeof(UnknownFault))]

        void DeRegisterStudentsFromCourseOfferingByPath(string coursePath, Guid courseOfferingId, List<Guid> userIds);

        /// <summary>
        /// Add and remove instructors from a course offering (either active or inactive). Also, update the
        /// offerings short ID (and add digits to make it unique if needed).
        /// </summary>
        /// <remarks>
        /// This is a bagged function call for CSMART Extensions service.
        /// </remarks>
        /// <param name="coursePath">Course path</param>
        /// <param name="offeringId">Course offering Id</param>
        /// <param name="instructorToAddIds">Ids of users to add as instructor.</param>
        /// <param name="instructorsToRemoveIds">User Ids of instructors to remove</param>
        /// <param name="shortId">Set short Id string for the course offering</param>
        /// <param name="isActive">Set flag whether the offering is active or not.</param>
        /// <returns>the actual short Id assigned to the offering</returns>
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

        string UpdateOfferingByPath(string coursePath, Guid offeringId, List<Guid> instructorToAddIds, List<Guid> instructorsToRemoveIds, string shortId, bool isActive);

        /// <summary>
        /// Add a user to the given group on the given item.
        /// </summary>
        /// <param name="path">path to the item on which the group definition resides</param>
        /// <param name="loginId">login name of the user to add (user name or email address)</param>
        /// <param name="groupName">name of the group to add them to</param>
        /// <exception cref="PathFormatFault">if there are syntactive problems with the path</exception>
        /// <exception cref="NotFoundFault">if there is no item corresponding to the path, or the login Id</exception>
        /// <exception cref="WrongLinkTypeFault">if a link in the path points to the wrong type of item</exception>

        [OperationContract]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(PathFormatFault))]
        [FaultContract(typeof(WrongLinkTypeFault))]
        [FaultContract(typeof(UnknownFault))]

        void AddUserToGroup(string path, Guid loginId, string groupName);

    }
}
