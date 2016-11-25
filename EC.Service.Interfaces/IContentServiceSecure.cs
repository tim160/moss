using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.IO;
using EC.Constants;
using EC.Service.DTO;
using EC.Errors;
using EC.Errors.ECExceptions;
using EC.Errors.CommonExceptions;
using EC.Errors.FileExceptions;
using EC.Errors.UserExceptions;
using FilterReportOptions = EC.Service.DTO.FilterReportOptions;
using OrgInfo = EC.Service.DTO.OrgInfo;

namespace EC.Service.Interfaces
{

    [ServiceContract]
    public interface IContentServiceSecure
    {

        /// <summary>
        /// Get the path to the CSS that should be used for rendering the given page.
        /// <para>
        /// Permissions: None required.
        /// </para>
        /// </summary>
        /// <remarks>
        /// This operation traverses the content tree upwards looking for CSS that
        /// should be applied. It starts at the given page.
        /// </remarks>
        /// <param name="path">path of the page being rendered.</param>
        /// <returns>the path to the CSS file or <c>null</c> if no CSS exist.</returns>

        [OperationContract]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(PathFormatFault))]
        [FaultContract(typeof(ConfigurationFault))]
        [FaultContract(typeof(NotAuthorizedFault))]

        string GetCSSPathForPage(string path);

        /// <summary>
        /// Return all link items (pre-filtered) at this path.
        /// </summary>
        /// <remarks>
        /// The <c>StartIndex</c> will skip the first <c>StartIndex</c> elements.
        /// To deactivate Paging set <c>PageSize</c> to -1 and <c>StartIndex</c> to 0 - all elements are returned.
        /// If the <c>PageSize</c> = -1 and <c>StartIndex</c> > 0 all items, starting with <c>StartIndex</c> are returned.
        /// </remarks>
        /// <param name="path">Path to get links from.</param>
        /// <param name="pageInfo">Range of list to fetch. If <c>null</c>, all items.</param>
        /// <param name="filter">Filter to decide which types of links should be returned.</param>
        /// <param name="detailLevel">Detail level: 'attributes' to include attributes for the links.</param>
        /// <returns>Return paged link items at this path. If no links exist or are filtered - return an empty list.</returns>
        /// <exception cref="NotFoundFault">If the path (item) doesn't exist.</exception>
        /// <exception cref="NotANavPageFault">If the item at the <paramref name="path"/> is not a nav page (link).</exception>
        /// <exception cref="NotAuthorizedFault">If access for the content item is denied.</exception>
        /// <exception cref="UnknownFault">For any other error.</exception>

        //////[OperationContract]
        //////[FaultContract(typeof(NotFoundFault))]
        //////[FaultContract(typeof(NotANavPageFault))]
        //////[FaultContract(typeof(NotAuthorizedFault))]
        //////[FaultContract(typeof(UnknownFault))]

        //////CollectionSubset<Link> GetLinkItemsByPath(string path, DTO.PageInfo pageInfo, LinkTypeFilterEnum filter, string detailLevel);

        /// <summary>
        /// Get imported files (could be folders or files) of a file collection and <paramref name="relativeFolder"/>.
        /// Ordered by Folders first and in alphabetical order.
        /// <remarks>
        /// The <c>StartIndex</c> will skip the first <c>StartIndex</c> elements.
        /// To deactivate Paging set <c>PageSize</c> to -1 and <c>StartIndex</c> to 0 - all elements are returned.
        /// If the <c>PageSize</c> = -1 and <c>StartIndex</c> > 0 all items, starting with <c>StartIndex</c> are returned.
        /// </remarks>
        /// </summary>
        /// <param name="path">Path where to find the file collection (link to a NavPage which has a file collection attached to it)</param>
        /// <param name="pageInfo">Range of list to fetch. If <c>null</c>, all items.</param>
        /// <param name="relativeFolderPath">Set <c>null</c> to get items from root level. Set '/FolderName' to get items from this folder,...</param>
        /// <param name="contentFilter">Filter which items (e.g. only Folders) should be returned.</param>
        /// <returns>Return paged list of imported files at this level (could be folders or files). Return an empty list if no items exist or all items are filtered.</returns>
        /// <exception cref="NotFoundFault">If the path (item) doesn't exist.</exception>
        /// <exception cref="NotANavPageFault">If the item at the <paramref name="path"/> is not a nav page (link).</exception>
        /// <exception cref="FileCollectionNofFoundFault">If the specified item at <paramref name="path"/> doesn't have a file collection assigned.</exception>
        /// <exception cref="NotAuthorizedFault">If access for the content item is denied.</exception>
        /// <exception cref="UnknownFault">For any other error.</exception>

        [OperationContract]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(NotANavPageFault))]
        [FaultContract(typeof(FileCollectionNotFoundFault))]
        [FaultContract(typeof(NotAuthorizedFault))]
        [FaultContract(typeof(UnknownFault))]

        CollectionSubset<FileItem> GetFileCollectionItemsByPath(string path, DTO.PageInfo pageInfo, string relativeFolderPath, FileCollectionContentTypeFilterEnum contentFilter);

        /// <summary>
        /// Get imported files (could be folders or files) of a file collection and <paramref name="relativeFolder"/>.
        /// Ordered by Folders first and in alphabetical order.
        /// <remarks>
        /// The <paramref name="path"/> is a <c>NavPage</c> as a reference point to the shared file collection.
        /// This <c>NavPage</c> is used to check the permissions (CanView). <br/>
        /// The permissions are checked as follows:<br/>
        /// Add the <c>NavPage</c> at <paramref name="navPageId"/> as a link to the <paramref name="path"/> and check the <c>CanView</c>
        /// permission.
        /// <p/>
        /// The <c>NavPage</c> at <paramref name="navPageId"/> must be sharable and have a file collection.
        /// <p/>
        /// The <c>StartIndex</c> will skip the first <c>StartIndex</c> elements.
        /// To deactivate Paging set <c>PageSize</c> to -1 and <c>StartIndex</c> to 0 - all elements are returned.
        /// If the <c>PageSize</c> = -1 and <c>StartIndex</c> > 0 all items, starting with <c>StartIndex</c> are returned.
        /// 
        /// </remarks>
        /// </summary>
        /// <param name="path">Path to the <c>NavPage</c> which is used for permission check to get items from the file collection (see remarks for details).</param>
        /// <param name="navPageId">Id of the <c>NavPage</c> which holds the file collection to get the items from.</param>
        /// <param name="pageInfo">Range of list to fetch. If <c>null</c>, all items.</param>
        /// <param name="relativeFolderPath">Set <c>null</c> to get items from root level. Set '/FolderName' to get items from this folder,...</param>
        /// <param name="contentFilter">Filter which items (e.g. only Folders) should be returned.</param>
        /// <returns></returns>
        /// <exception cref="NotFoundFault">If the <c>NavPage</c> at <paramref name="path"/> or <paramref name="navPageId"/> can't be found.</exception>
        /// <exception cref="NotANavPageFault">If the item at the <paramref name="path"/> or <paramref name="navPageId"/> is not a nav page .</exception>
        /// <exception cref="FileCollectionNotFoundFault">If the file collection at <paramref name="navPageId"/> doesn't exist or is not sharable.</exception>
        /// <exception cref="NotAuthorizedFault">If no access to <paramref name="path"/> or the file collection at <paramref name="navPageId"/></exception>
        /// <exception cref="UnknownFault">For any other error.</exception>

        [OperationContract]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(NotANavPageFault))]
        [FaultContract(typeof(FileCollectionNotFoundFault))]
        [FaultContract(typeof(NotAuthorizedFault))]
        [FaultContract(typeof(UnknownFault))]

        CollectionSubset<FileItem> GetSharedFileCollectionItemsByPathAndId(string path, Guid navPageId, DTO.PageInfo pageInfo, string relativeFolderPath, FileCollectionContentTypeFilterEnum contentFilter);



        /// <summary>
        /// Check a capability on a node by path.
        /// </summary>
        /// <param name="path">Path to a content node.</param>
        /// <param name="capabilityName">Capability to check.</param>
        /// <returns>Return <c>true</c> if the capability is allowed. Return <c>false</c> if the capability is not allowed.</returns>
        /// <exception cref="NotFoundFault">If the path doesn't exist.</exception>
        /// <exception cref="PathFormatFault">If the path has a wrong format or not a content item.</exception>
        /// <exception cref="AuthenticationRequiredFault">If no user is logged in/ logon is required.</exception>
        /// <exception cref="UnknownFault">For any other error.</exception>

        [OperationContract]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(PathFormatFault))]
        [FaultContract(typeof(CapabilityNotFoundFault))]
        [FaultContract(typeof(InvalidCapabilityFault))]
        [FaultContract(typeof(AuthenticationRequiredFault))]
        [FaultContract(typeof(UnknownFault))]

        bool CheckCapabilityByPath(string path, string capabilityName);

        /// <summary>
        /// Find any matched capability in any courses that is under the path or sub-path and has course offerings.
        /// </summary>
        /// <remarks>
        /// In order to award certificates the user must have CanViewCertificates and CanAwardCertificates capabilities.
        /// </remarks>
        /// <param name="path">The path.</param>
        /// <param name="capabilities">All capabilities must apply in order to have permission for the course</param>
        /// <param name="activeOnly">if set to <c>true</c> [active only].</param>
        /// <returns></returns>

        [OperationContract]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(PathFormatFault))]
        [FaultContract(typeof(CapabilityNotFoundFault))]
        [FaultContract(typeof(InvalidCapabilityFault))]
        [FaultContract(typeof(AuthenticationRequiredFault))]
        [FaultContract(typeof(UnknownFault))]

        bool CheckCapabilitiesInCourses(string path, IList<string> capabilities, bool activeOnly);

        /// <summary>
        /// Check a given content item for multiple permissions at once.
        /// </summary>
        /// <param name="path">the item to check</param>
        /// <param name="capabilities">the capabilities to check</param>
        /// <returns>the set of capabilities that the user has</returns>

        [OperationContract]
        [FaultContract(typeof(CapabilityNotFoundFault))]
        [FaultContract(typeof(InvalidCapabilityFault))]
        [FaultContract(typeof(PathFormatFault))]
        [FaultContract(typeof(UnknownFault))]

        List<string> CheckCapabilitiesByPath(string path, IList<string> capabilities);

        /// <summary>
        /// Check if the <paramref name="currentUser"/> has the capability CanEvaluateStudent or 
        /// is instructor in a course (across all offerings) with active registrations.
        /// If there are no active registrations in any offering and the user doesn't have the capability CanEvaluateStudent
        /// return <c>false</c> even if the user is instructor in an offering.
        /// </summary>
        /// <param name="studentEvaluationPath">Path to a student evaluation link</param>
        /// <returns>
        /// Return <c>true</c> if at least one registration is active and the user is instructor in at least one offering of the registrations.
        /// Return <c>false</c> if no registrations are active and the user doesn't have CanEvaluateStudent
        /// Return <c>false</c> if active registrations are available but the user is not instructor in any of those 
        /// </returns>
        /// <exception cref="PathFormatFault">If the <paramref name="studentEvaluationPath"/> has a wrong format</exception>
        /// <exception cref="NotFoundFault">If <paramref name="studentEvaluationPath"/> isn't found</exception>
        /// <exception cref="AuthenticationRequiredFault">If no user is logged on</exception>
        /// <exception cref="NotACourseFault">If the <paramref name="studentEvaluationPath"/> is not inside a course</exception>
        /// <exception cref="UnkownFault">On any other error</exception>

        [OperationContract]
        [FaultContract(typeof(PathFormatFault))]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(AuthenticationRequiredFault))]
        [FaultContract(typeof(NotACourseFault))]
        [FaultContract(typeof(UnknownFault))]

        bool CheckIsInstructorForCourseAndActiveRegistrationsOrCanEvaluateStudent(string studentEvaluationPath);

        /// <summary>
        /// Check to see if student review of the given evaluation is allowed.
        /// </summary>
        /// <exception cref="PathFormatFault">If the <paramref name="studentEvaluationPath"/> has a wrong format</exception>
        /// <exception cref="NotFoundFault">If <paramref name="studentEvaluationPath"/> isn't found</exception>
        /// <exception cref="AuthenticationRequiredFault">If no user is logged on</exception>
        /// <exception cref="NotACourseFault">If the <paramref name="studentEvaluationPath"/> is not inside a course</exception>
        /// <exception cref="ParameterValidationFault">If the path does not correspond to an evaluation link</exception>
        /// <exception cref="UnknownFault">On any other error</exception>

        [OperationContract]
        [FaultContract(typeof(PathFormatFault))]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(AuthenticationRequiredFault))]
        [FaultContract(typeof(NotACourseFault))]
        [FaultContract(typeof(ParameterValidationFault))]
        [FaultContract(typeof(UnknownFault))]

        bool IsEvaluationReviewAllowed(string linkPath);

        /// <summary>
        /// Predicate indicating whether the given path (assumed to be an evaluation link) has a corresponding evaluation.
        /// </summary>
        /// <exception cref="PathFormatFault">If the <paramref name="studentEvaluationPath"/> has a wrong format</exception>
        /// <exception cref="NotFoundFault">If <paramref name="studentEvaluationPath"/> isn't found</exception>
        /// <exception cref="AuthenticationRequiredFault">If no user is logged on</exception>
        /// <exception cref="NotACourseFault">If the <paramref name="studentEvaluationPath"/> is not inside a course</exception>
        /// <exception cref="ParameterValidationFault">if the path does not refer to an evaluatio link</exception>

        [OperationContract]
        [FaultContract(typeof(PathFormatFault))]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(AuthenticationRequiredFault))]
        [FaultContract(typeof(NotACourseFault))]
        [FaultContract(typeof(UnknownFault))]
        bool IsEvaluationMissing(string studentEvaluationPath);


        /// <summary>
        /// Will update the attributes on the passed in item (NavPage, FileCollection,...).  
        /// <para>
        /// Permissions: Requires the CAN_MANAGE_PAGE_ATTRIBUTES capability on the given item.
        /// </para>
        /// </summary>
        /// <param name="attributes">list of attributes to update and create, attributes not in this list but in the DB will be deleted.</param>
        /// <param name="path">path to the item (= the path to the link of the item)</param>
        /// <exception cref="NotFoundFault"></exception>
        /// <exception cref="PathFormatFault"></exception>
        /// <exception cref="NotAuthorizedFault"></exception>
        /// <exception cref="WrongCapabilityAttributeFormatFault">If a capability attribute has a wrong format.</exception>
        /// <exception cref="AttributeFormatFault">If a group definition attribute has a wrong format.</exception>
        /// <exception cref="UnknownFault">For any other error.</exception>
        /// <exception cref="NullAttributeException">Thrown if attribute is null</exception>

        [OperationContract]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(PathFormatFault))]
        [FaultContract(typeof(NotAuthorizedFault))]
        [FaultContract(typeof(WrongCapabilityAttributeFormatFault))]
        [FaultContract(typeof(AttributeFormatFault))]
        [FaultContract(typeof(UnknownFault))]
        [FaultContract(typeof(NullAttributeFault))]

        void SetNavPageAttributes(string path, IList<AttributeItem> attributes);

        /// <summary>
        /// Will update the attributes on the passed in link (NavPageLink, ExternalLink,...).  
        /// <para>
        /// Permissions: Requires the CAN_MANAGE_LINK_ATTRIBUTES capability on the given link.
        /// </para>
        /// </summary>
        /// <param name="attributes">list of attributes to update and create, attributes not in this list but in the DB will be deleted.</param>
        /// <param name="path">path to the link</param>
        /// <exception cref="NotFoundFault"></exception>
        /// <exception cref="PathFormatFault"></exception>
        /// <exception cref="NotAuthorizedFault">thrown if the user does not have permission to set link attributes.</exception>
        /// <exception cref="WrongCapabilityAttributeFormatFault">If a capability attribute has a wrong format.</exception>
        /// <exception cref="AttributeFormatFault">If a group definition attribute has a wrong format.</exception>
        /// <exception cref="UnknownFault">For any other error.</exception>

        [OperationContract]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(PathFormatFault))]
        [FaultContract(typeof(NotAuthorizedFault))]
        [FaultContract(typeof(WrongCapabilityAttributeFormatFault))]
        [FaultContract(typeof(AttributeFormatFault))]
        [FaultContract(typeof(UnknownFault))]

        void SetLinkAttributes(string path, IList<AttributeItem> attributes);

        /// <summary>
        /// Check the formats of all attributes and return a list of all attributes which have the wrong attribute/duplicate values,...).
        /// </summary>
        /// <param name="path">Path where the attributes are located for permission check</param>
        /// <param name="attributes">List of attributes to check</param>
        /// <returns>Dictionary with attributes which have a wrong format/duplicate and their error messages</returns>
        /// <exception cref="NotFoundFault"></exception>
        /// <exception cref="PathFormatFault"></exception>
        /// <exception cref="NotAuthorizedFault">thrown if the user does not have permission to view attributes.</exception>
        /// <exception cref="UnknownFault">For any other error.</exception>

        [OperationContract]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(PathFormatFault))]
        [FaultContract(typeof(NotAuthorizedFault))]
        [FaultContract(typeof(UnknownFault))]

        Dictionary<DTO.AttributeItem, string> CheckAttributesFormat(string path, IList<DTO.AttributeItem> attributes);

        /// <summary>
        /// Create a file collection on a nav page.
        /// </summary>
        /// <param name="path">the path to the nav page where the file collection will be created</param>
        /// <param name="name">the name of file collection</param>
        /// <returns>The FileCollection object representing the file collection (incl. root path of the file system for the files).</returns>
        /// <exception cref="NotFoundFault">thrown if the path does not correspond to a ContentItem</exception>
        /// <exception cref="NotANavPageFault">thrown if the element at the <paramref name="path"/> is not a nav page.</exception>
        /// <exception cref="PathFormatFault">thrown if the path is not syntactically correct</exception>
        /// <exception cref="ConfigurationFault">Some error with the DB.</exception>
        /// <exception cref="NotAuthorizedFault">thrown if the user does not have permission to add repositories</exception>
        /// <exception cref="AlreadyExistsFault">thrown if the file collection for the nav page already exists.</exception>
        /// <exception cref="PageTagException">If page at <paramref name="path"/> is not tagged as 'Course' or 'Organization'</exception>
        /// <exception cref="UnknownFault">For any other error.</exception>

        [OperationContract]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(NotANavPageFault))]
        [FaultContract(typeof(PathFormatFault))]
        [FaultContract(typeof(ConfigurationFault))]
        [FaultContract(typeof(NotAuthorizedFault))]
        [FaultContract(typeof(AlreadyExistsFault))]
        [FaultContract(typeof(PageTagFault))]
        [FaultContract(typeof(UnknownFault))]

        FileCollection CreateFileCollection(string path, string name);

        /// <summary>
        /// Edit an existing group name of a NavPage.
        /// </summary>
        /// <param name="path">Path to the NavPage</param>
        /// <param name="groupId">Id of the group to edit</param>
        /// <param name="newGroupName">New group name.</param>
        /// <exception cref="NotFoundFault">thrown if the path does not correspond to an item.</exception>
        /// <exception cref="NotANavPageFault">Item is not a NavPage.</exception>
        /// <exception cref="NotAuthorizedFault">If the user is not allowed to edit the a group</exception>
        /// <exception cref="ParameterFault">Group name must be specified.</exception>
        /// <exception cref="ConfigurationFault">Some error with the DB.</exception>
        /// <exception cref="UnknownFault">For any other error.</exception>

        [OperationContract]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(NotANavPageFault))]
        [FaultContract(typeof(NotAuthorizedFault))]
        [FaultContract(typeof(ParameterValidationFault))]
        [FaultContract(typeof(ConfigurationFault))]
        [FaultContract(typeof(UnknownFault))]

        void EditNavPageGroupName(string path, Guid groupId, string newGroupName);

        /// <summary>
        /// Edit link details (no matter which type of link it is).
        /// </summary>
        /// <param name="path">Path to the link</param>
        /// <param name="newUrlName">New Url name for the link.</param>
        /// <param name="newDisplayText">Link display text.</param>
        /// <param name="newShortSummary">Short summary for the link.</param>
        /// <param name="newLongSummary">Long summary for the link.</param>
        /// <param name="newIconPath">New path to the icon. Set to <c>null</c> if no icon for the link should be set.</param>
        /// <param name="isMandatory">Flag whether the link is mandatory (<c>true</c>) or optional (<c>false</c>).</param>
        /// <param name="externalLinkUrl">Only used when link is external. Will set the url of the external link.</param>
        /// <param name="allowStudentReview">Flag whether to allow the student review the result</param>
        /// <param name="allowRetakeExam">Flag whether to allow the student to retake an exam if he failed it</param>
        /// <param name="retakeAfter">Amount of days after the student can retake the exam</param>
        /// <param name="passingPercentage">Percentage to pass the exam</param>
        /// <param name="password"></param>
        /// <param name="examType"></param>
        /// <exception cref="NotFoundFault">thrown if the path does not correspond to the link item or if the link does not link to a nav page.</exception>
        /// <exception cref="ParameterValidationFault">If <paramref name="newUrlName"/> is not set.</exception>
        /// <exception cref="AlreadyExistsFault">If the urlName already exists on the parent page</exception>
        /// <exception cref="ConfigurationFault">Some error with the DB.</exception>
        /// <exception cref="UnknownFault">For any other error.</exception>
        //////[OperationContract]
        //////[FaultContract(typeof(NotFoundFault))]
        //////[FaultContract(typeof(NotAuthorizedFault))]
        //////[FaultContract(typeof(ParameterValidationFault))]
        //////[FaultContract(typeof(ConfigurationFault))]
        //////[FaultContract(typeof(UnknownFault))]
        //////[FaultContract(typeof(AlreadyExistsFault))]

        //////void EditLinkDetails(string path, string newUrlName, string newDisplayText, string newShortSummary, string newLongSummary,
        //////    string newIconPath, bool isMandatory, string externalLinkUrl, bool allowStudentReview, bool allowRetakeExam, int retakeAfter, double passingPercentage, string password, ExamTypeEnum? examType);

        /// <summary>
        /// Set the publish mode of a link.
        /// </summary>
        /// <remarks>
        /// The publish mode is indicated by override attributes CanView.! 
        /// and CanDisplayAsLink.! to the predicate IsInstructor().
        /// </remarks>
        /// <param name="path">Path to the link</param>
        /// <param name="isPublished">Flag whether the link should be published or not.</param>
        /// <exception cref="NotFoundFault">thrown if the path does not correspond to the link item.</exception>
        /// <exception cref="NotAuthorizedFault">If the user is not allowed to edit the link</exception>
        /// <exception cref="ConfigurationFault">Some error with the DB.</exception>
        /// <exception cref="UnknownFault">For any other error.</exception>

        [OperationContract]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(NotAuthorizedFault))]
        [FaultContract(typeof(ConfigurationFault))]
        [FaultContract(typeof(UnknownFault))]

        void EditLinkPublishMode(string path, bool isPublished);

        /// <summary>
        /// Edit nav page details (content properties).
        /// </summary>
        /// <param name="path">Path to the nav page (=nav page link)</param>
        /// <param name="newTitle">Page title.</param>
        /// <param name="newHeader">New header text.</param>
        /// <param name="htmlContentNavPage">Html Source Content for NavPage</param>
        /// <exception cref="NotFoundFault">thrown if the path does not correspond to the nav page.</exception>
        /// <exception cref="NotAuthorizedFault">If the user is not allowed to edit a nav page.</exception>
        /// <exception cref="ConfigurationFault">Some error with the DB.</exception>
        /// <exception cref="UnknownFault">For any other error.</exception>

        [OperationContract]
        [FaultContract(typeof(FileCollectionNotFoundFault))]
        [FaultContract(typeof(FileNotCshtmlFault))]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(NotANavPageFault))]
        [FaultContract(typeof(ParameterValidationFault))]
        [FaultContract(typeof(NotAuthorizedFault))]
        [FaultContract(typeof(ConfigurationFault))]
        [FaultContract(typeof(UnknownFault))]

        void EditNavPageDetails(string path, string newTitle, string newHeader, string htmlContentNavPage, string oldTemplatePath, string newTemplatePath, string newFileCollectionPath);

        /// <summary>
        /// Edit nav page defaults (which are copied to a new link when created). 
        /// </summary>
        /// <param name="path">Path to the nav page (=nav page link)</param>
        /// <param name="newUrlName">New Url name for the link (this might break existing link to the underneath structure).</param>
        /// <param name="newDisplayText">Link display text.</param>
        /// <param name="newShortSummary">Short summary for the link.</param>
        /// <param name="newLongSummary">Long summary for the link.</param>
        /// <param name="newIconPath">New path to the icon. Set to <c>null</c> if no icon for the link should be set.</param>
        /// <param name="isMandatory">Flag whether the link is mandatory (<c>true</c>) or optional (<c>false</c>).</param>
        /// <exception cref="NotFoundFault">thrown if the path does not correspond to the link item.</exception>
        /// <exception cref="NotAuthorizedFault">If the user is not allowed to edit a nav page.</exception>
        /// <exception cref="ParameterValidationFault">If <paramref name="newUrlName"/> is not set.</exception>
        /// <exception cref="ConfigurationFault">Some error with the DB.</exception>
        /// <exception cref="UnknownFault">For any other error.</exception>

        [OperationContract]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(NotANavPageFault))]
        [FaultContract(typeof(ParameterValidationFault))]
        [FaultContract(typeof(NotAuthorizedFault))]
        [FaultContract(typeof(ConfigurationFault))]
        [FaultContract(typeof(UnknownFault))]

        void EditNavPageDefaults(string path, string newUrlName, string newDisplayText, string newShortSummary, string newLongSummary, string newIconPath, bool isMandatory);

        /// <summary>
        /// Reorder the links and groups of a NavPage.
        /// </summary>
        /// <param name="path">Path to the nav page (= nav page link)</param>
        /// <param name="items">new set of NavPage items</param>
        /// <exception cref="NotFoundFault">thrown if the path does not correspond to an item.</exception>
        /// <exception cref="NotANavPageFault">Item is not a NavPage.</exception>
        /// <exception cref="NotAuthorizedFault">If the user is not allowed to edit a nav page.</exception>
        /// <exception cref="ConfigurationFault">Some error with the DB.</exception>
        /// <exception cref="UnknownFault">Any other error.</exception>        

        ////////[OperationContract]
        ////////[FaultContract(typeof(NotFoundFault))]
        ////////[FaultContract(typeof(NotANavPageFault))]
        ////////[FaultContract(typeof(ParameterValidationFault))]
        ////////[FaultContract(typeof(NotAuthorizedFault))]
        ////////[FaultContract(typeof(ConfigurationFault))]
        ////////[FaultContract(typeof(UnknownFault))]

        ////////void ReplaceNavPageItems(string path, IList<NavPageItem> items);

        /// <summary>
        /// Delete a NavPageItem from a NavPage (along with its sub-items)
        /// </summary>
        /// <param name="path">Path to the nav page (= nav page link)</param>
        /// <param name="itemId">Id of the item to delete</param>
        /// <exception cref="NotFoundFault">thrown if the path does not correspond to an item.</exception>
        /// <exception cref="NotANavPageFault">Item is not a NavPage.</exception>
        /// <exception cref="NotAuthorizedFault">If the user is not allowed to edit a nav page.</exception>
        /// <exception cref="ConfigurationFault">Some error with the DB.</exception>
        /// <exception cref="CannotDeleteFault">Encountered dangling foreign key references, can't delete item with itemId</exception>
        /// <exception cref="UnknownFault">Any other error.</exception>        

        [OperationContract]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(NotANavPageFault))]
        [FaultContract(typeof(NotAuthorizedFault))]
        [FaultContract(typeof(ConfigurationFault))]
        [FaultContract(typeof(CannotDeleteFault))]
        [FaultContract(typeof(UnknownFault))]

        void DeleteNavPageItem(string path, Guid itemId);

        /// <summary>
        /// Add a new exam and the link (either Exam or StudentEvaluation link - depending on the <paramref name="assessmentType"/>) to the path.
        /// </summary>
        /// <remarks>
        /// Depending on the <paramref name="assessmentType"/> a different type of link will be created:
        /// - 'StudentEvaluation' -> 'StudentEvaluationLink'
        /// - 'Exam' -> 'ExamLink'
        /// </remarks>
        /// <param name="sections">The Exam Sections of the Exam</param>
        /// <param name="displayOrder">The display order of the new Exam Link, -1 if it should be the last</param>
        /// <param name="linkPath">The NavPage path the link should be added to</param>
        /// <param name="targetItemId">The Target link or groupId, null if the link should be added to the root</param>
        /// <param name="examCollectionPath">The Exam Collection to Add the Exam To. Set <c>null</c> if the exam collection is located at <paramref name="linkPath"/></param>
        /// <param name="name">The Name of the Exam</param>
        /// <param name="assessmentType">Type of exam (i.e. Exam, StudentEvaluation)</param>
        /// <param name="examType">Exam only: The Exam Link Type</param>
        /// <param name="password">Password for protected type </param>
        /// <param name="introduction">The Introduction of the Exam</param>
        /// <param name="instructions">The Instructions of the Exam</param>
        /// <param name="timeLimit">Exam only: The Exam Time Limit. 0 indicates no time limit on the Exam</param>
        /// <param name="showSections">show question categories, when set to true</param>
        /// <param name="useDefaultValues">Whether to use the default attributes from the Exam Link</param>
        /// <param name="allowStudentReview">Exam only: Flag whether the student can review the exam</param>
        /// <param name="allowRetakeFailedExam">Exam only: Flag whether to allow a student to retake a failed exam.</param>
        /// <param name="retakeExamAfterDays">Exam only: Positive number of days a student has to wait until he/she is able to retake an exam 
        /// (only valid if <paramref name="allowRetakeFailedExam"/> is <c>true</c>)</param>
        /// <param name="examPassingPercentage">Exam only: Percentage (will be rounded to 2 decimals) needed to pass the exam 
        /// (only valid if <paramref name="allowRetakeFailedExam"/> is <c>true</c>)</param>
        /// <returns></returns>
        /// <exception cref="AlreadyExistsException">If the urlName is being used already by another exam in the same collection</exception>


        ////////////[OperationContract]
        ////////////[FaultContract(typeof(PathFormatFault))]
        ////////////[FaultContract(typeof(WrongLinkTypeFault))]
        ////////////[FaultContract(typeof(NotFoundFault))]
        ////////////[FaultContract(typeof(NotANavPageFault))]
        ////////////[FaultContract(typeof(FileCollectionNotFoundFault))]
        ////////////[FaultContract(typeof(AlreadyExistsFault))]
        ////////////[FaultContract(typeof(ParameterValidationFault))]
        ////////////[FaultContract(typeof(NotAuthorizedFault))]
        ////////////[FaultContract(typeof(ConfigurationFault))]
        ////////////[FaultContract(typeof(UnknownFault))]

        ////////////NavPageItem AddNavPageExamAndLink(string linkPath, Guid? targetItemId, string examCollectionPath, string name, AssessmentTypeEnum assessmentType,
        ////////////    ExamTypeEnum examType, string password, string introduction, string instructions, string htmlContent, List<DTO.ExamSection> sections, int displayOrder, int timeLimit,
        ////////////    bool useDefaultValues, bool showSections, bool allowStudentReview, bool allowRetakeFailedExam, int retakeExamAfterDays, double examPassingPercentage);


        /// <summary>
        /// Add new external link to an existing NavPage. It is also possible to set attributes assigned to the link.
        /// <br/>Note: All Ids are assigned with new Ids.
        /// </summary>
        /// <param name="path">Path to the NavPage (= NavPage link)</param>
        /// <param name="targetGroupId">Group Id to add the new link to. Set <c>null</c> to add the new link at the top level of the nav page.</param>
        /// <param name="displayOrder">Set display order of the new link</param>
        /// <param name="newLink">New external link item (all ids are regenerated).</param>
        /// <returns>Return the DisaplyLink with the newly added link.</returns>
        /// <exception cref="NotFoundFault">If the item to add the link to doesn't exist.</exception>
        /// <exception cref="NotANavPageFault">If the path is not a NavPage link (=NavPage).</exception>
        /// <exception cref="AlreadyExistsFault">If the link Url name already exists for this NavPage.</exception>
        /// <exception cref="ParameterValidationFault">If no display text for the link has been set.</exception>
        /// <exception cref="NotAuthorizedFault">If the user is not allowed to edit a nav page.</exception>
        /// <exception cref="ConfigurationFault">Some error with the DB.</exception>
        /// <exception cref="WrongCapabilityAttributeFormatFault">If a capability attribute has a wrong format.</exception>
        /// <exception cref="AttributeFormatFault">If a group definition attribute has a wrong format.</exception>
        /// <exception cref="UnknownFault">Any other error.</exception>

        //////[OperationContract]
        //////[FaultContract(typeof(NotFoundFault))]
        //////[FaultContract(typeof(NotANavPageFault))]
        //////[FaultContract(typeof(AlreadyExistsFault))]
        //////[FaultContract(typeof(ParameterValidationFault))]
        //////[FaultContract(typeof(NotAuthorizedFault))]
        //////[FaultContract(typeof(ConfigurationFault))]
        //////[FaultContract(typeof(WrongCapabilityAttributeFormatFault))]
        //////[FaultContract(typeof(AttributeFormatFault))]
        //////[FaultContract(typeof(UnknownFault))]

        //////NavPageItem AddNavPageExternalLink(string path, Guid? targetGroupId, int displayOrder, ExternalLink newLink);

        /// <summary>
        /// Add new student evaluation view link to an existing NavPage. It is also possible to set attributes assigned to the link.
        /// <br/>Note: All Ids are assigned with new Ids.
        /// </summary>
        /// <param name="path">Path to the NavPage (= NavPage link)</param>
        /// <param name="targetItemId">Item Id to add the new link to. Set <c>null</c> to add the new link at the top level of the nav page.</param>
        /// <param name="displayOrder">Set display order of the new link</param>
        /// <param name="newLink">New external link item (all ids are regenerated).</param>
        /// <returns>Return the DisplayLink with the newly added link.</returns>
        /// <exception cref="NotFoundFault">If the item to add the link to doesn't exist.</exception>
        /// <exception cref="NotANavPageFault">If the path is not a NavPage link (=NavPage).</exception>
        /// <exception cref="AlreadyExistsFault">If the link Url name already exists for this NavPage.</exception>
        /// <exception cref="ParameterValidationFault">If no display text for the link has been set.</exception>
        /// <exception cref="NotAuthorizedFault">If the user is not allowed to edit a nav page.</exception>
        /// <exception cref="ConfigurationFault">Some error with the DB.</exception>
        /// <exception cref="WrongCapabilityAttributeFormatFault">If a capability attribute has a wrong format.</exception>
        /// <exception cref="AttributeFormatFault">If a group definition attribute has a wrong format.</exception>
        /// <exception cref="UnknownFault">Any other error.</exception>

        ////////////[OperationContract]
        ////////////[FaultContract(typeof(NotFoundFault))]
        ////////////[FaultContract(typeof(NotANavPageFault))]
        ////////////[FaultContract(typeof(AlreadyExistsFault))]
        ////////////[FaultContract(typeof(ParameterValidationFault))]
        ////////////[FaultContract(typeof(NotAuthorizedFault))]
        ////////////[FaultContract(typeof(ConfigurationFault))]
        ////////////[FaultContract(typeof(WrongCapabilityAttributeFormatFault))]
        ////////////[FaultContract(typeof(AttributeFormatFault))]
        ////////////[FaultContract(typeof(UnknownFault))]

        ////////////NavPageItem AddNavPageStudentEvaluationViewLink(string path, Guid? targetItemId, int displayOrder, StudentEvaluationViewLink newLink);


        /// <summary>
        /// Add a new file link to a nav page from a newly uploaded file.
        /// The file must be uploaded with a separate service call <see cref="UploadFileAsStream"/>. The <paramref name="uploadFileId"/> is returned to identify the file.
        /// It is also possible to set attributes assigned to the link.
        /// <br/>Note: All Ids are assigned with new Ids.
        /// </summary>
        /// <remarks>
        /// Default values for a file link item are:
        /// <br/> - UrlName
        /// <br/> - DisplayText
        /// <br/> - IconPath
        /// <br/> - ShortSummary
        /// <br/> - LongSummary
        /// <br/> - IsMandatory
        /// <br/> If <paramref name="useDefaultValues"/> is set to <c>false</c> - the values of the <paramref name="newFileLink"/> are used to set link values.
        /// <br/>
        /// <br/>
        /// <paramref name="newFileLink"/>.RelativePath must be set to place the file into the file collection. Set <c>null</c> to place the file at the root folder of the file collection.
        /// </remarks>
        /// <param name="path">Path to the nav page (= nav page link) where the file link is added.</param>
        /// <param name="targetGroupId">Group Id to add the new link to. Set <c>null</c> to add the new link at the top level of the nav page.</param>
        /// <param name="displayOrder">Set display order of the new link</param>
        /// <param name="newFileLink">
        /// Contains information about the new file link (e.g. attributes, URLName,...). All Ids are assigned with new Ids (e.g. for attributes).
        /// The <c>RelativePath</c> must be set to place the file into the file collection (e.g. /MyFolder/MyFile1.pdf). 
        /// The <c>LinkTargetPath</c> must be set to identify the file collection the new file should be saved.
        /// Set to <c>null</c> if the file should be uploaded into the local file collection at <paramref name="path"/>.
        /// </param>
        /// <param name="useDefaultValues">Use default values from the internal item (e.g. ShortSummary). See remarks for details.</param>
        /// <param name="uploadFileId">File id to be added to the file collection. (use <see cref="UploadFileAsStream"/> to upload the file first).</param>
        /// <returns>Return the <c>DisaplyLink</c> with the newly added link.</returns>
        /// <exception cref="NotFoundFault">If the item to add the link to doesn't exist.</exception>
        /// <exception cref="NotANavPageFault">If the path is not a NavPage link (=NavPage).</exception>
        /// <exception cref="AlreadyExistsFault">If the link Url name already exists for this NavPage.</exception>
        /// <exception cref="ParameterValidationFault">If no display text for the link has been set.</exception>
        /// <exception cref="NotAuthorizedFault">If the user is not allowed to edit a nav page.</exception>
        /// <exception cref="FileCollectionNotFoundFault">If the <paramref name="fileCollectionPath"/> doesn't contain a file collection.</exception>
        /// <exception cref="FileAlreadyExistsFault">If the file name already exists in the file collection folder.</exception>
        /// <exception cref="ConfigurationFault">Some error with the DB.</exception>
        /// <exception cref="WrongCapabilityAttributeFormatFault">If a capability attribute has a wrong format.</exception>
        /// <exception cref="AttributeFormatFault">If a group definition attribute has a wrong format.</exception>
        /// <exception cref="UnknownFault">Any other error.</exception>

        //////[OperationContract]
        //////[FaultContract(typeof(NotFoundFault))]
        //////[FaultContract(typeof(NotANavPageFault))]
        //////[FaultContract(typeof(AlreadyExistsFault))]
        //////[FaultContract(typeof(ParameterValidationFault))]
        //////[FaultContract(typeof(NotAuthorizedFault))]
        //////[FaultContract(typeof(FileCollectionNotFoundFault))]
        //////[FaultContract(typeof(FileAlreadyExistsFault))]
        //////[FaultContract(typeof(ConfigurationFault))]
        //////[FaultContract(typeof(WrongCapabilityAttributeFormatFault))]
        //////[FaultContract(typeof(AttributeFormatFault))]
        //////[FaultContract(typeof(UnknownFault))]

        //////NavPageItem AddNavPageFileLinkToNewFile(string path, Guid? targetGroupId, int displayOrder, FileLink newFileLink, Guid uploadFileId);

        /// <summary>
        /// Upload a file via stream to a temporary directory.
        /// Only logged in users can upload a file.
        /// </summary>
        /// <remarks>
        /// WCF only allows one parameter for the service call if you want to pass in a stream. 
        /// For example adding a new file to a file collection is a 2 step process: 
        ///   1.) Upload file to temporary location
        ///   2.) Link/add the new uploaded file into the system (e.g. file collection).
        /// </remarks>
        /// <param name="contentStream">Stream to the file.</param>
        /// <returns>Return a unique Id with which the file can be addressed (e.g. to add it to a file collection).</returns>
        /// <exception cref="AuthenticationRequiredFault">If no user is logged in.</exception>
        /// <exception cref="ConfigurationFault">Some error with the DB.</exception>
        /// <exception cref="UnknownFault">Any other error.</exception>

        [OperationContract]
        [FaultContract(typeof(AuthenticationRequiredFault))]
        [FaultContract(typeof(ConfigurationFault))]
        [FaultContract(typeof(UnknownFault))]

        Guid UploadFileAsStream(Stream contentStream);


        /// <summary>
        /// Toggles the NavPages view between Links and TOC view.
        /// The page must have the required attributes specified.
        /// </summary>
        /// <param name="path">The path to the NavPage</param>
        /// <returns></returns>
        /// <exception cref="NotFoundFault">If the item doesn't exist, or the question collection is not present</exception>
        /// <exception cref="NotANavPageFault">If the item does exist, but it is not a NavPage.</exception>
        /// <exception cref="PathFormatFault">If the path is not valid</exception>
        /// <exception cref="WrongLinkTypeFault">If a link in the path points to the wrong type of item</exception>
        /// <exception cref="NotAuthorizedFault">if the user does not have permission</exception>
        /// <exception cref="AttributeNotFoundFault">if the page does not have the required attributes</exception>
        /// <exception cref="UnknownFault">For any other error.</exception>

        [OperationContract]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(NotANavPageFault))]
        [FaultContract(typeof(PathFormatFault))]
        [FaultContract(typeof(WrongLinkTypeFault))]
        [FaultContract(typeof(NotAuthorizedFault))]
        [FaultContract(typeof(AttributeNotFoundFault))]
        [FaultContract(typeof(UnknownFault))]

        void ToggleNavPageView(string path, NavPageViewTypes newView);

        /// <summary>
        /// Deletes the file.
        /// </summary>
        /// <param name="path">path to the item</param>
        /// <param name="relativePath">The relative path.</param>
        /// <exception cref="NotAuthorizedFault">If access for the content item is denied.</exception>
        /// <exception cref="NotFoundFault">If the path (item) doesn't exist.</exception>
        /// <exception cref="PathFormatFault">If the path has a wrong format.</exception>
        /// <exception cref="ContentTypeMisMatchFault">if the path does not lead to a NavPageLink or FileLink</exception>

        [OperationContract]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(NotANavPageFault))]
        [FaultContract(typeof(NotAuthorizedFault))]
        [FaultContract(typeof(UnknownFault))]

        void DeleteFile(string path, string relativePath);

        /// <summary>
        /// Gets the display names for each link up the specified path.
        /// <remarks>
        /// The triples contain the following (UrlName, DisplayText, Full Path).
        /// </remarks>
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>A List of Display Names for the Specified Path with Paths. The triples contain the following (UrlName, DisplayText, Full Path)</returns>
        /// <exception cref="NotFoundFault">If the path (item) doesn't exist.</exception>
        /// <exception cref="NotAuthorizedFault">If access for the content item is denied.</exception>
        /// <exception cref="UnknownFault">For any other error.</exception>
        [OperationContract]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(NotAuthorizedFault))]
        [FaultContract(typeof(UnknownFault))]
        List<Tuple<string, string, string>> GetDisplayNamesForPath(string path);

        /// <summary>
        /// Retrieve a QuestionCollection. If sharedPageId is null, then the QuestionCollection 
        /// returned is the one for the NavPage identified by the path. Otherwise, the QuestionCollection
        /// is obtained from the NavPage specified by sharedPageId.
        /// <remarks>
        /// Available details level:
        /// - 'Categories' ... detail level includes all question categories in the question collection
        /// - 'Categories.UsageCount' ... add number of questions per category.
        /// </remarks>
        /// </summary>
        /// <param name="path">path to current NavPage</param>
        /// <param name="detailLevel">Serialization level (see remarks for details).</param>
        /// <returns>the QuestionCollection (may be null)</returns>
        /// <exception cref="NotFoundFault">if there is not item with the given path</exception>
        /// <exception cref="PathFormatFault">if the path is not valid</exception>
        /// <exception cref="NotANavPageFault">if the path does not specify a NavPage</exception>
        /// <exception cref="WrongLinkTypeFault">if a link on the path has the wrong type of target</exception>
        /// <exception cref="NotAuthorizedFault">If the user has no permission to create the question collection.</exception>
        /// <exception cref="ConfigurationFault">Some error with the DB.</exception>
        /// <exception cref="UnknownFault">For any other error.</exception>


        /// <summary>
        /// Create a QuestionCollection on the NavPage specified by the given path. This will
        /// also create an associated FileCollection on the NavPage which will be used for
        /// storing content (text, html, images, etc) that are used for both questions
        /// and answers.
        /// </summary>
        /// <param name="path">path to NavPage on which collection should be created</param>
        /// <param name="name">name for the new collection</param>
        /// <exception cref="NotFoundFault">if there is not item with the given path</exception>
        /// <exception cref="PathFormatFault">if the path is not valid</exception>
        /// <exception cref="NotANavPageFault">if the path does not specify a NavPage</exception>
        /// <exception cref="NotAuthorizedFault">If the user has no permission to create the question collection.</exception>
        /// <exception cref="ConfigurationFault">Some error with the DB.</exception>
        /// <exception cref="PageTagFault">If page at <paramref name="path"/> is not tagged as 'Course' or 'Organization'</exception>
        /// <exception cref="UnknownFault">For any other error.</exception>

        [OperationContract]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(PathFormatFault))]
        [FaultContract(typeof(NotANavPageFault))]
        [FaultContract(typeof(NotAuthorizedFault))]
        [FaultContract(typeof(ConfigurationFault))]
        [FaultContract(typeof(PageTagFault))]
        [FaultContract(typeof(UnknownFault))]

        void CreateQuestionCollection(string path, string name);

        ///////// <summary>
        ///////// Create a new question in the appropriate Question Collection, with the given content and choices.
        ///////// The target Question Collection is specified by the path and sharedPageId. If sharedPageId is null,
        ///////// then the Question Collection is the one attached to the NavPage specified by path. Otherwise,
        ///////// the Question Collection is the one associated with the NavPage specified by sharedPageId. In this
        ///////// latter case, the permissions for the shared NavPage are computed in the context of the current
        ///////// NavPage.
        ///////// </summary>
        ///////// <param name="path">path to current NavPage</param>
        ///////// <param name="sharedPageId">path to shared NavPage (optional)</param>
        ///////// <param name="categoryName">the category this question belongs to</param>
        ///////// <param name="content">display content for question</param>
        ///////// <param name="isDisabled">indicates whether the question is disable or not</param>
        ///////// <param name="choices">list of answer choices for the question</param>
        ///////// <exception cref="PathFormatFault">if there are syntactive problems with the path</exception>
        ///////// <exception cref="NotANavPageFault">the target of the path is not a NavPage</exception>
        ///////// <exception cref="NotFoundFault">if there is no item corresponding to the path</exception>
        ///////// <exception cref="WrongLinkTypeFault">if a link in the path points to the wrong type of item</exception>
        ///////// <exception cref="NotAuthorizedFault">thrown if the user does not have permission</exception>

        //////[OperationContract]
        //////[FaultContract(typeof(NotFoundFault))]
        //////[FaultContract(typeof(PathFormatFault))]
        //////[FaultContract(typeof(NotANavPageFault))]
        //////[FaultContract(typeof(WrongLinkTypeFault))]
        //////[FaultContract(typeof(NotAuthorizedFault))]
        //////[FaultContract(typeof(ConfigurationFault))]
        //////[FaultContract(typeof(UnknownFault))]

        //////void CreateQuestion(string path, Guid? sharedPageId, string categoryName, string content, bool isDisabled, List<Answer> choices);

        /// <summary>
        /// Delete a question. The target Question Collection is specified by the path and sharedPageId. 
        /// If sharedPageId is null, then the Question Collection is the one attached to the NavPage 
        /// specified by path. Otherwise, the Question Collection is the one associated with the NavPage 
        /// specified by sharedPageId. In this latter case, the permissions for the shared NavPage are 
        /// computed in the context of the current NavPage. NOTE: A NotFoundFault will be thrown if
        /// the question does not reside in the target Question Collection.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="sharedPageId"></param>
        /// <param name="questionId"></param>
        /// <exception cref="NotFoundFault">if the question cannot be found</exception>
        /// <exception cref="PathFormatFault">if the path is not syntactically valid</exception>
        /// <exception cref="WrongLinkTypeFault">if a link on the path has the wrong type of target</exception>
        /// <exception cref="NotAuthorizedFault">if the user does not have access</exception>
        /// <exception cref="NotANavPageFault">if the path does not refer to a NavPage</exception>
        /// <exception cref="ItemInUseFault">if the question cannot be deleted because it is in use</exception>

        [OperationContract]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(PathFormatFault))]
        [FaultContract(typeof(NotANavPageFault))]
        [FaultContract(typeof(WrongLinkTypeFault))]
        [FaultContract(typeof(ItemInUseFault))]
        [FaultContract(typeof(NotAuthorizedFault))]
        [FaultContract(typeof(ConfigurationFault))]
        [FaultContract(typeof(UnknownFault))]

        void DeleteQuestion(string path, Guid? sharedPageId, Guid questionId);

        /// <summary>
        /// Modify a question. The list of answer choices given must be complete. That is, it must represent the
        /// complete list of answer choices for the question. Each answer has a nullable Id field. If this field
        /// is set, then the Answer is presumed to exist already and it will be modified according to the other
        /// values in the DTO. If the Id is null, then a new answer will be created based on the values in the
        /// DTO. The target Question Collection is specified by the path and sharedPageId. 
        /// If sharedPageId is null, then the Question Collection is the one attached to the NavPage 
        /// specified by path. Otherwise, the Question Collection is the one associated with the NavPage 
        /// specified by sharedPageId. In this latter case, the permissions for the shared NavPage are 
        /// computed in the context of the current NavPage. NOTE: A NotFoundFault will be thrown if
        /// the question does not reside in the target Question Collection.
        /// </summary>
        /// <param name="path">current NavPage</param>
        /// <param name="sharedPageId">primary ID of shared NavPage</param>
        /// <param name="questionId">primary ID of question to modify</param>
        /// <param name="content">display content for question</param>
        /// <param name="isDisabled">indicates whether question is disabled</param>
        /// <param name="choices">list of answer choices for the question.</param>
        /// <exception cref="PathFormatFault">if there are syntactive problems with the path</exception>
        /// <exception cref="NotANavPageFault">the target of the path is not a NavPage</exception>
        /// <exception cref="NotFoundFault">if there is no item corresponding to the path</exception>
        /// <exception cref="WrongLinkTypeFault">if a link in the path points to the wrong type of item</exception>
        /// <exception cref="NotAuthorizedFault">thrown if the user does not have permission</exception>
        /// <exception cref="DirectoryCreationFault">if a directory cannot be created (to store content)</exception>
        /// <exception cref="FileWriteFault">if a file cannot be written (to store content)</exception>

        //////[OperationContract]
        //////[FaultContract(typeof(NotFoundFault))]
        //////[FaultContract(typeof(PathFormatFault))]
        //////[FaultContract(typeof(NotANavPageFault))]
        //////[FaultContract(typeof(WrongLinkTypeFault))]
        //////[FaultContract(typeof(NotAuthorizedFault))]
        //////[FaultContract(typeof(DirectoryCreationFault))]
        //////[FaultContract(typeof(FileWriteFault))]
        //////[FaultContract(typeof(ConfigurationFault))]
        //////[FaultContract(typeof(UnknownFault))]

        //////void ModifyQuestion(string path, Guid? sharedPageId, Guid questionId, string content, bool isDisabled, List<Answer> choices);

        /// <summary>
        /// Import a set of question definitions to the target Question Collection.
        /// The target Question Collection is specified by the path
        /// The Question Collection is the one attached to the NavPage specified by path. 
        /// </summary>
        /// <param name="path">path to current NavPage</param>
        /// <param name="XML">question definitions</param>
        /// <exception cref="PathFormatFault">if there are syntactive problems with the path</exception>
        /// <exception cref="NotFoundFault">if there is no item corresponding to the path</exception>
        /// <exception cref="WrongLinkTypeFault">if a link in the path points to the wrong type of item</exception>
        /// <exception cref="NotANavPageFault">the target of the path is not a NavPage</exception>
        /// <exception cref="DirectoryCreationFault">if a directory on disk cannot be created</exception>
        /// <exception cref="FileWriteFault">if a file on disk cannot be written to</exception>
        /// <exception cref="XMLFormatFault">if there is a problem parsing the XML</exception>

        [OperationContract]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(PathFormatFault))]
        [FaultContract(typeof(NotANavPageFault))]
        [FaultContract(typeof(WrongLinkTypeFault))]
        [FaultContract(typeof(NotAuthorizedFault))]
        [FaultContract(typeof(DirectoryCreationFault))]
        [FaultContract(typeof(FileWriteFault))]
        [FaultContract(typeof(ConfigurationFault))]
        [FaultContract(typeof(XMLFormatFault))]
        [FaultContract(typeof(UnknownFault))]

        void ImportQuestions(string path, string XML);

        /// <summary>
        /// Update SortPosition for a sorted question collection
        /// </summary>
        /// <param name="navPath">path to NavPage</param>
        /// <param name="questions">List of DTO questions whihc have the current sortposition set</param>
        /// <exception cref="NotFoundFault">if there is not item with the given path</exception>
        /// <exception cref="PathFormatFault">if the path is not valid</exception>
        /// <exception cref="NotANavPageFault">if the path does not specify a NavPage</exception>
        /// <exception cref="WrongLinkTypeFault">if a link on the path has a target of the wrong type</exception>
        /// <exception cref="NotAuthorizedFault">if the current user does not have permission to view questions</exception>

        //////[OperationContract]
        //////[FaultContract(typeof(NotFoundFault))]
        //////[FaultContract(typeof(PathFormatFault))]
        //////[FaultContract(typeof(NotANavPageFault))]
        //////[FaultContract(typeof(WrongLinkTypeFault))]
        //////[FaultContract(typeof(NotAuthorizedFault))]
        //////[FaultContract(typeof(ConfigurationFault))]
        //////[FaultContract(typeof(UnknownFault))]

        //////void UpdateSortPositionForQuestions(string navPath, IList<DTO.Question> questions);

        /// <summary>
        /// Update Sortable flage for category
        /// </summary>
        /// <param name="navPath">path to NavPage</param>
        /// <param name="categoryName">category we need to set sortable for</param>
        /// <param name="sortable">sortable flag value</param>
        /// <exception cref="NotFoundFault">if there is not item with the given path</exception>
        /// <exception cref="PathFormatFault">if the path is not valid</exception>
        /// <exception cref="NotANavPageFault">if the path does not specify a NavPage</exception>
        /// <exception cref="WrongLinkTypeFault">if a link on the path has a target of the wrong type</exception>
        /// <exception cref="NotAuthorizedFault">if the current user does not have permission to view questions</exception>

        [OperationContract]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(PathFormatFault))]
        [FaultContract(typeof(NotANavPageFault))]
        [FaultContract(typeof(WrongLinkTypeFault))]
        [FaultContract(typeof(NotAuthorizedFault))]
        [FaultContract(typeof(ConfigurationFault))]
        [FaultContract(typeof(UnknownFault))]

        void UpdateSortableForCategory(string navPath, string categoryName, bool sortable);


        /// <summary>
        /// Get a Question by its ID. The target Question Collection is specified by the path and sharedPageId. 
        /// If sharedPageId is null, then the Question Collection is the one attached to the NavPage 
        /// specified by path. Otherwise, the Question Collection is the one associated with the NavPage 
        /// specified by sharedPageId. In this latter case, the permissions for the shared NavPage are 
        /// computed in the context of the current NavPage. NOTE: The question must reside in the
        /// target question collection or a NotFoundFault will be thrown.
        /// </summary>
        /// <param name="path">path to current NavPage</param>
        /// <param name="sharedPageId">path to shared NavPage (optional)</param>
        /// <param name="id">the primary ID for the question</param>
        /// <param name="detailLevel">serialization level</param>
        /// <returns>DTO for the Question</returns>
        /// <exception cref="NotFoundFault">if the question cannot be found</exception>
        /// <exception cref="PathFormatFault">if the path is not syntactically valid</exception>
        /// <exception cref="NotAuthorizedFault">if the user does not have access</exception>
        /// <exception cref="NotANavPageFault">if the path does not refer to a NavPage</exception>
        /// <exception cref="WrongLinkTypeFault">if a link on the path has the wrong type of target</exception>

        //////[OperationContract]
        //////[FaultContract(typeof(NotFoundFault))]
        //////[FaultContract(typeof(PathFormatFault))]
        //////[FaultContract(typeof(NotANavPageFault))]
        //////[FaultContract(typeof(WrongLinkTypeFault))]
        //////[FaultContract(typeof(NotAuthorizedFault))]
        //////[FaultContract(typeof(ConfigurationFault))]
        //////[FaultContract(typeof(UnknownFault))]

        //////Question GetQuestionByPathAndId(string path, Guid? sharedPageId, Guid id, string detailLevel);

        /// <summary>
        /// Get a question by its short ID, verifying that it comes from the QuestionCollection identified 
        /// by the given path. The target Question Collection is specified by the path and sharedPageId. 
        /// If sharedPageId is null, then the Question Collection is the one attached to the NavPage 
        /// specified by path. Otherwise, the Question Collection is the one associated with the NavPage 
        /// specified by sharedPageId. In this latter case, the permissions for the shared NavPage are 
        /// computed in the context of the current NavPage. NOTE: A NotFoundFault will be thrown if
        /// the question does not reside in the target Question Collection.
        /// </summary>
        /// <param name="path">path to a NavPage for Question Collection</param>
        /// <param name="sharedPageId">primary ID of shared NavPage (optional)</param>
        /// <param name="shortId">short ID of question</param>
        /// <param name="detailLevel">serialization control</param>
        /// <returns>Question DTO</returns>
        /// <exception cref="NotFoundFault">if the question cannot be found</exception>
        /// <exception cref="PathFormatFault">if the path is not syntactically valid</exception>
        /// <exception cref="WrongLinkTypeFault">if a link on the path has the wrong type of target</exception>
        /// <exception cref="NotAuthorizedFault">if the user does not have access</exception>
        /// <exception cref="NotANavPageFault">if the path does not refer to a NavPage</exception>

        ////[OperationContract]
        ////[FaultContract(typeof(NotFoundFault))]
        ////[FaultContract(typeof(PathFormatFault))]
        ////[FaultContract(typeof(NotANavPageFault))]
        ////[FaultContract(typeof(WrongLinkTypeFault))]
        ////[FaultContract(typeof(NotAuthorizedFault))]
        ////[FaultContract(typeof(ConfigurationFault))]
        ////[FaultContract(typeof(UnknownFault))]

        ////Question GetQuestionByPathAndShortId(string path, Guid? sharedPageId, int shortId, string detailLevel);

        /// <summary>
        /// Get a question from a question collection. 
        /// The <paramref name="filter"/> specifies the list of question collections to look for the question.
        /// <remarks>
        /// <c>NavPage.Path</c> contains the path where the question is located at.
        /// </remarks>
        /// </summary>
        /// <param name="path">path to a NavPage to start looking for the question</param>
        /// <param name="shortId">Short Id of the question</param>
        /// <param name="filter">Filter to tell which question collection(s) should be checked for the question (based on location to <paramref name="path"/>).</param>
        /// <param name="detailLevel">Detail level for the returned <c>Question</c> and <c>NavPage</c> item where the question is located.</param>
        /// <returns>Returns a tuple containing the NavPage and Question item.</returns>
        /// <exception cref="NotFoundFault">if the question cannot be found</exception>
        /// <exception cref="PathFormatFault">if the path is not syntactically valid</exception>
        /// <exception cref="WrongLinkTypeFault">if a link on the path has the wrong type of target</exception>
        /// <exception cref="NotAuthorizedFault">if the user does not have access</exception>
        /// <exception cref="NotANavPageFault">if the path does not refer to a NavPage</exception>


        /// <summary>
        /// Change the name for the given category. If sharePageID is null, then the category affected is located
        /// in the QuestionCollection attached to the NavPage specified by the path. Otherwise, the category affected is
        /// located in the collection on the NavPage specified by the sharePageID, and the path is used for a permissions context.
        /// </summary>
        /// <param name="path">path to current location in tree</param>
        /// <param name="sharedPageId">ID of shared NavPage (if needed)</param>
        /// <param name="categoryId">ID of category to change</param>
        /// <param name="newCategoryName">new name for category</param>
        /// <exception cref="PathFormatFault">path is syntactically incorrect</exception>
        /// <exception cref="WrongLinkTypeFault">path has a terminal link in a non-terminal position</exception>
        /// <exception cref="NotANavPageFault">path does not refer to a NavPage</exception>
        /// <exception cref="NotFoundFault">path does not refer to an item or category not found</exception>
        /// <exception cref="NotAuthorizedFault">user does not have permission to change name</exception>
        /// <exception cref="ConfigurationFault">Some error with the DB.</exception>
        /// <exception cref="UnknownFault">For any other error.</exception>
        /// <exception cref="AlreadyExistsFault">the new name already exists on the current path</exception>

        [OperationContract]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(PathFormatFault))]
        [FaultContract(typeof(NotANavPageFault))]
        [FaultContract(typeof(NotAuthorizedFault))]
        [FaultContract(typeof(WrongLinkTypeFault))]
        [FaultContract(typeof(ConfigurationFault))]
        [FaultContract(typeof(UnknownFault))]
        [FaultContract(typeof(AlreadyExistsFault))]

        void RenameQuestionCategory(string path, Guid? sharedPageId, Guid categoryId, string newCategoryName);

        /// <summary>
        /// Delete a category from the QuestionCollection attached to the NavPage specified by the path.
        /// </summary>
        /// <param name="path">path to NavPage that hosts the Question Collection </param>
        /// <param name="categoryId">primary key of Question Category to delete</param>
        /// <exception cref="NotFoundFault">If the item doesn't exist, or the question collection is not present</exception>
        /// <exception cref="NotANavPageFault">If the item does exist, but it is not a NavPage.</exception>
        /// <exception cref="PathFormatFault">If the path is not valid</exception>
        /// <exception cref="WrongLinkTypeFault">If a link in the path points to the wrong type of item</exception>
        /// <exception cref="ItemInUseFault">If the category is used by one or more exams</exception>
        /// <exception cref="NotAuthorizedFault">if the user does not have permission to create question categories</exception>

        [OperationContract]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(PathFormatFault))]
        [FaultContract(typeof(NotANavPageFault))]
        [FaultContract(typeof(WrongLinkTypeFault))]
        [FaultContract(typeof(ItemInUseFault))]
        [FaultContract(typeof(NotAuthorizedFault))]
        [FaultContract(typeof(ConfigurationFault))]
        [FaultContract(typeof(UnknownFault))]

        void DeleteQuestionCategory(string path, Guid categoryId);

        /// <summary>
        /// Create a question category. If sharePageID is null, then the category is created on the QuestionCollection
        /// attached to the NavPage specified by the path. Otherwise, the category is created on the collection
        /// on the NavPage specified by the sharePageID, and the path is used for a permissions context.
        /// </summary>
        /// <param name="path">path to NavPage that hosts the Question Collection</param>
        /// <param name="sharedPageId">ID of shared NavPage (if needed)</param>
        /// <param name="categoryName">name for the new category</param>
        /// <exception cref="NotFoundFault">If the item doesn't exist, or the question collection is not present</exception>
        /// <exception cref="NotANavPageFault">If the item does exist, but it is not a NavPage.</exception>
        /// <exception cref="PathFormatFault">If the path is not valid</exception>
        /// <exception cref="WrongLinkTypeFault">If a link in the path points to the wrong type of item</exception>
        /// <exception cref="ParameterValidationFault">If the category is null</exception>
        /// <exception cref="AlreadyExistsFault">thrown if you try to add a category that is already there</exception>
        /// <exception cref="NotAuthorizedFault">if the user does not have permission to create question categories</exception>

        [OperationContract]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(PathFormatFault))]
        [FaultContract(typeof(NotANavPageFault))]
        [FaultContract(typeof(ParameterValidationFault))]
        [FaultContract(typeof(NotAuthorizedFault))]
        [FaultContract(typeof(ConfigurationFault))]
        [FaultContract(typeof(UnknownFault))]
        [FaultContract(typeof(AlreadyExistsFault))]

        void CreateQuestionCategory(string path, Guid? sharedPageId, string categoryName);

        /// <summary>
        /// Return the set of categories from a given Question Collection, but only including those
        /// categories which are used by the given exam. The target Question Collection is specified by 
        /// the path. The Question Collection is the one associated with the NavPage.
        /// <para>
        /// Detail levels supported: 
        /// - UsageCount ... include category usage counts
        /// </para>
        /// </summary>
        /// <param name="path">path to current NavPage</param>
        /// <param name="examId">primary ID of the exam to filter by</param>
        /// <param name="detailLevel">serialization level</param>
        /// <returns></returns>
        /// <exception cref="NotFoundFault">if there is not item with the given path</exception>
        /// <exception cref="PathFormatFault">if the path is not valid</exception>
        /// <exception cref="NotANavPageFault">if the path does not specify a NavPage</exception>
        /// <exception cref="WrongLinkTypeFault">if a link on the path has a target of the wrong type</exception>
        /// <exception cref="NotAuthorizedFault">if the current user does not have permission to view questions</exception>

        //////[OperationContract]
        //////[FaultContract(typeof(NotFoundFault))]
        //////[FaultContract(typeof(PathFormatFault))]
        //////[FaultContract(typeof(NotANavPageFault))]
        //////[FaultContract(typeof(WrongLinkTypeFault))]
        //////[FaultContract(typeof(NotAuthorizedFault))]
        //////[FaultContract(typeof(ConfigurationFault))]
        //////[FaultContract(typeof(UnknownFault))]

        //////CollectionSubset<QuestionCategory> GetQuestionCategoriesByExam(string path, Guid examId, string detailLevel, DTO.PageInfo pageInfo, SortDirectionEnum sortDirection, string sortColumnName, string nameFilterText);

        /// <summary>
        /// Create an ExamCollection on the NavPage specified by the given path.
        /// </summary>
        /// <param name="path">path to NavPage on which collection should be created</param>
        /// <param name="name">name for the new collection</param>
        /// <exception cref="NotFoundFault">if there is not item with the given path</exception>
        /// <exception cref="PathFormatFault">if the path is not valid</exception>
        /// <exception cref="NotANavPageFault">if the path does not specify a NavPage</exception>
        /// <exception cref="NotAuthorizedFault">If the user has no permission to create the exam collection.</exception>
        /// <exception cref="ConfigurationFault">Some error with the DB.</exception>
        /// <exception cref="PageTagFault">If page at <paramref name="path"/> is not tagged as 'Course' or 'Organization'</exception>
        /// <exception cref="UnknownFault">For any other error.</exception>

        [OperationContract]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(PathFormatFault))]
        [FaultContract(typeof(NotANavPageFault))]
        [FaultContract(typeof(NotAuthorizedFault))]
        [FaultContract(typeof(ConfigurationFault))]
        [FaultContract(typeof(PageTagFault))]
        [FaultContract(typeof(UnknownFault))]

        void CreateExamCollection(string path, string name);


        /// <summary>
        /// Checks password prtected exam type passwords if it is matches with current password or not
        /// </summary>
        /// <param name="path">The path to the exam link</param>
        /// <param name="password"></param>
        /// <returns>DTO of the exam</returns>
        /// <exception cref="ContentTypeMismatchException">Non-exam link.</exception>
        /// <exception cref="WrongLinkTypeException">Target of ExamLink not an ExamCollection.</exception>
        /// <exception cref="NotFoundException">Exam not found.</exception>
        [OperationContract]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(PathFormatFault))]
        [FaultContract(typeof(NotACourseFault))]
        [FaultContract(typeof(WrongLinkTypeFault))]
        [FaultContract(typeof(ContentTypeMismatchFault))]
        [FaultContract(typeof(NotAuthorizedFault))]
        [FaultContract(typeof(ConfigurationFault))]
        [FaultContract(typeof(UnknownFault))]

        bool CheckExamPassword(string path, string password);

        /// <summary>
        /// Changes password protected exam type password
        /// </summary>
        /// <param name="path">The path to the exam link</param>
        /// <param name="password"></param>
        /// <returns>DTO of the exam</returns>
        /// <exception cref="ContentTypeMismatchException">Non-exam link.</exception>
        /// <exception cref="WrongLinkTypeException">Target of ExamLink not an ExamCollection.</exception>
        /// <exception cref="NotFoundException">Exam not found.</exception>

        [OperationContract]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(PathFormatFault))]
        [FaultContract(typeof(NotACourseFault))]
        [FaultContract(typeof(WrongLinkTypeFault))]
        [FaultContract(typeof(ContentTypeMismatchFault))]
        [FaultContract(typeof(NotAuthorizedFault))]
        [FaultContract(typeof(ConfigurationFault))]
        [FaultContract(typeof(UnknownFault))]

        void ChangeExamPassword(string path, string password);


        /// <summary>
        /// Generate or continue a student evaluation.
        /// </summary>
        /// <remarks>
        /// The currently logged in user must be capable of 'CanEvaluateStudent' or instructor in <paramref name="offeringId"/>.
        /// The registrationId must be either at <paramref name="path"/> or along the <paramref name="path"/> and in active state.
        /// </remarks>
        /// <param name="path">Path where the evaluation is taken (could be an organization, course or below a course)</param>
        /// <param name="registrationId">Registration Id the evaluation is taking place for (must be in active state).</param>
        /// <param name="detailLevel"></param>
        /// <returns></returns>
        /// <exception cref="NotFoundFault">If the <paramref name="registrationId"/> doesn't exist, is not active or <paramref name="path"/> doesn't exist</exception>
        /// <exception cref="AuthenticationRequiredFault">If no user is logged in.</exception>
        /// <exception cref="NotAuthorizedFault">
        /// If the user is not allowed to evaluate a user (either the CanEvaluateStudent capability is missing or the user
        /// is not an instructor for the <paramref name="registrationId"/>.
        /// </exception>
        /// <exception cref="WrongLinkTypeFault">if a link on the path points to the wrong type of item</exception>
        /// <exception cref="ContentTypeMismatchFault">if the path does not refer to a student evaluation link</exception>
        /// <exception cref="NotACourseFault">if the parent of the path does not specify a NavPageLink which is a course</exception>
        /// <exception cref="LinkTypeFault">If the <paramref name="pathAsLinks"/> doesn't contain a student evaluation link</exception>
        /// <exception cref="AlreadyTakenExamFault">If the student already has been evaluated. Only one evaluation per student and course registration is allowed</exception>
        /// <exception cref="ExamHasMultipleInstanceAuditsFault">If the student has multiple evaluations for the registration</exception>
        /// <exception cref="WrongRegistrationFault">If the <paramref name="registrationId"/> is not along the evaluation link path <paramref name="pathAsLinks"/></exception>
        /// <exception cref="PathFormatFault">If <paramref name="path"/> has the wrong format</exception>
        /// <exception cref="ConfigurationFault">If the DB or service are not configured correctly</exception>
        /// <exception cref="UowIsolationFault">If retries failed to update the <c>code</c>/deadlock</exception>
        /// <exception cref="UnknownFault">On any other error</exception>

        ////////////[OperationContract]
        ////////////[FaultContract(typeof(NotFoundFault))]
        ////////////[FaultContract(typeof(AuthenticationRequiredFault))]
        ////////////[FaultContract(typeof(NotAuthorizedFault))]
        ////////////[FaultContract(typeof(WrongLinkTypeFault))]
        ////////////[FaultContract(typeof(ContentTypeMismatchFault))]
        ////////////[FaultContract(typeof(NotACourseFault))]
        ////////////[FaultContract(typeof(LinkTypeFault))]
        ////////////[FaultContract(typeof(AlreadyTakenExamFault))]
        ////////////[FaultContract(typeof(ExamHasMultipleInstanceAuditsFault))]
        ////////////[FaultContract(typeof(WrongCourseOfferingFault))]
        ////////////[FaultContract(typeof(PathFormatFault))]
        ////////////[FaultContract(typeof(ConfigurationFault))]
        ////////////[FaultContract(typeof(UserFault))]
        ////////////[FaultContract(typeof(UowIsolationFault))]
        ////////////[FaultContract(typeof(UnknownFault))]

        ////////////StudentEvaluationInstance GenerateStudentEvaluationInstanceByPath(string path, Guid registrationId, string detailLevel);

        /// <summary>
        /// Return the amount of time left for a given exam (actually, you provide the timer ID
        /// so this will work for any timer, not just exams).
        /// </summary>
        /// <param name="id">ID of the timer to check</param>
        /// <returns>timer remaining on the timer, 0 means it does not exist</returns>

        [OperationContract]
        [FaultContract(typeof(UnknownFault))]

        int TimeRemainingForExam(Guid id);

        /// <summary>
        /// Return the set of exams that reference categories defined in the target
        /// Question Collection. The target Question Collection is specified by the path and sharedPageId. 
        /// If sharedPageId is null, then the Question Collection is the one attached to the NavPage 
        /// specified by path. Otherwise, the Question Collection is the one associated with the NavPage 
        /// specified by sharedPageId. In this latter case, the permissions for the shared NavPage are 
        /// computed in the context of the current NavPage. NOTE: We don't check to see whether you
        /// have permission to the exam collection or exam, and need to make sure that we return only
        /// minimal information about the exam as a result.
        /// </summary>
        /// <param name="path">path to current NavPage</param>
        /// <param name="sharedPageId">optional ID of share NavPage</param>
        /// <returns>List of exams that use categories defined in the Question Collection</returns>
        /// <exception cref="NotFoundFault">If the item doesn't exist, or the question collection is not present</exception>
        /// <exception cref="NotANavPageFault">If the item does exist, but it is not a NavPage.</exception>
        /// <exception cref="PathFormatFault">If the path is not valid</exception>
        /// <exception cref="WrongLinkTypeFault">If a link in the path points to the wrong type of item</exception>
        /// <exception cref="NotAuthorizedFault">if the user does not have permission to get exams</exception>

        //////[OperationContract]
        //////[FaultContract(typeof(NotFoundFault))]
        //////[FaultContract(typeof(PathFormatFault))]
        //////[FaultContract(typeof(NotANavPageFault))]
        //////[FaultContract(typeof(WrongLinkTypeFault))]
        //////[FaultContract(typeof(NotAuthorizedFault))]
        //////[FaultContract(typeof(ConfigurationFault))]
        //////[FaultContract(typeof(UnknownFault))]

        //////List<Exam> GetExamsByCategoryUse(string path, Guid? sharedPageId);

        /// <summary>
        /// Create exam of type exam or evaluation.
        /// </summary>
        /// <param name="navPath">path to nav page the exam is on.</param>
        /// <param name="name">exam name</param>
        /// <param name="introduction">introduction for exam</param>
        /// <param name="instructions">instructions for exam</param>
        /// <param name="assessmentType">Type of exam (assessment, exam,...)</param>
        /// <param name="showSections">show question categories, when set to true</param>
        /// <param name="sections">exam sections</param>
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
        //////[FaultContract(typeof(AlreadyExistsFault))]

        //////void CreateExam(string navPath, string name, string introduction, string instructions, string htmlContent, AssessmentTypeEnum assessmentType, bool showSections, List<ExamSection> sections);

        /// <summary>
        /// Delete exam with examId
        /// </summary>
        /// <param name="navPath">path to nav page the exam is on.</param>
        /// <param name="examId">Id of exam to delete</param>
        /// <exception cref="NotFoundFault">If the item doesn't exist, or the exam collection is not present</exception>
        /// <exception cref="NotANavPageFault">If the item does exist, but it is not a NavPage.</exception>
        /// <exception cref="PathFormatFault">If the path is not valid</exception>
        /// <exception cref="WrongLinkTypeFault">If a link in the path points to the wrong type of item</exception>
        /// <exception cref="NotAuthorizedFault">if the user does not have permission to get exams</exception>
        /// <exception cref="ConfigurationFault">Some error with the DB.</exception>
        /// <exception cref="UnknownFault">For any other error.</exception>

        [OperationContract]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(PathFormatFault))]
        [FaultContract(typeof(NotANavPageFault))]
        [FaultContract(typeof(WrongLinkTypeFault))]
        [FaultContract(typeof(NotAuthorizedFault))]
        [FaultContract(typeof(ConfigurationFault))]
        [FaultContract(typeof(UnknownFault))]

        void DeleteExam(string navPath, Guid examId);



        /// <summary>
        /// Looks up path for closest file collection and returns it's path.
        /// Optionally check a capability.
        /// </summary>
        /// <param name="path">The path to start looking for the file collection.</param>
        /// <param name="capability">This is used to validate the current user against any file collection found. If user does not have capability
        /// we continue lookup up the path for a file collection the user has capability to. Pass <c>null</c> to return first file collection path found with no capability check</param>
        /// <returns>
        /// Return the path to the file collection. Return <c>null</c> if no file collection has been found (either there is no collection or the user doesn't have the capability).
        /// </returns>
        /// <exception cref="NotFoundFault">If the item doesn't exist, or the collection is not present</exception>
        /// <exception cref="NotANavPageFault">If the item does exist, but it is not a NavPage.</exception>
        /// <exception cref="PathFormatFault">If the path is not valid</exception>
        /// <exception cref="WrongLinkTypeFault">If a link in the path points to the wrong type of item</exception>
        /// <exception cref="ConfigurationFault">Some error with the DB.</exception>
        /// <exception cref="AuthenticationRequiredFault">If a capability needs authentication</exception>
        /// <exception cref="UnknownFault">For any other error.</exception>

        [OperationContract]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(PathFormatFault))]
        [FaultContract(typeof(NotANavPageFault))]
        [FaultContract(typeof(WrongLinkTypeFault))]
        [FaultContract(typeof(ConfigurationFault))]
        [FaultContract(typeof(AuthenticationRequiredFault))]
        [FaultContract(typeof(UnknownFault))]

        string FindClosestFileCollectionPath(string path, string capability = null);

        /// <summary>
        /// Looks up path for closest question collection and returns it's path.
        /// Optionally check a capability.
        /// </summary>
        /// <param name="path">The path to start looking for the question collection.</param>
        /// <param name="capability">This is used to validate the current user against any question collection found. If user does not have capability
        /// we continue lookup up the path for a question collection the user has capability to. Pass <c>null</c> to return first question collection path found with no capability check</param>
        /// <returns>
        /// Return the path to the question collection. Return <c>null</c> if no question collection has been found (either there is no collection or the user doesn't have the capability).
        /// </returns>
        /// <exception cref="NotFoundFault">If the item doesn't exist, or the collection is not present</exception>
        /// <exception cref="NotANavPageFault">If the item does exist, but it is not a NavPage.</exception>
        /// <exception cref="PathFormatFault">If the path is not valid</exception>
        /// <exception cref="WrongLinkTypeFault">If a link in the path points to the wrong type of item</exception>
        /// <exception cref="ConfigurationFault">Some error with the DB.</exception>
        /// <exception cref="AuthenticationRequiredFault">If a capability needs authentication</exception>
        /// <exception cref="UnknownFault">For any other error.</exception>

        [OperationContract]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(PathFormatFault))]
        [FaultContract(typeof(NotANavPageFault))]
        [FaultContract(typeof(WrongLinkTypeFault))]
        [FaultContract(typeof(ConfigurationFault))]
        [FaultContract(typeof(AuthenticationRequiredFault))]
        [FaultContract(typeof(UnknownFault))]

        string FindClosestQuestionCollectionPath(string path, string capability = null);

        /// <summary>
        /// Looks up path for closest exam collection and returns it's path.
        /// Optionally check a capability.
        /// </summary>
        /// <param name="path">The path to start looking for the exam collection.</param>
        /// <param name="capability">This is used to validate the current user against any exam collection found. If user does not have capability
        /// we continue lookup up the path for a exam collection the user has capability to. Pass <c>null</c> to return first exam collection path found with no capability check</param>
        /// <returns>
        /// Return the path to the exam collection. Return <c>null</c> if no exam collection has been found (either there is no collection or the user doesn't have the capability).
        /// </returns>
        /// <exception cref="NotFoundFault">If the item doesn't exist, or the collection is not present</exception>
        /// <exception cref="NotANavPageFault">If the item does exist, but it is not a NavPage.</exception>
        /// <exception cref="PathFormatFault">If the path is not valid</exception>
        /// <exception cref="WrongLinkTypeFault">If a link in the path points to the wrong type of item</exception>
        /// <exception cref="ConfigurationFault">Some error with the DB.</exception>
        /// <exception cref="AuthenticationRequiredFault">If a capability needs authentication</exception>
        /// <exception cref="UnknownFault">For any other error.</exception>

        [OperationContract]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(PathFormatFault))]
        [FaultContract(typeof(NotANavPageFault))]
        [FaultContract(typeof(WrongLinkTypeFault))]
        [FaultContract(typeof(ConfigurationFault))]
        [FaultContract(typeof(AuthenticationRequiredFault))]
        [FaultContract(typeof(UnknownFault))]

        string FindClosestExamCollectionPath(string path, string capability = null);

        /// <summary>
        /// Get the server directory for the file collection at <paramref name="path"/>.
        /// This only returns the server path if the currently logged in user is a SuperUser.
        /// </summary>
        /// <param name="path">Path to get the file collection server directory</param>
        /// <returns>
        /// Return the server path for the file collection at <paramref name="path"/> if the current user is a SuperUser.
        /// Return <c>null</c> if no file collection, non-existing path or the user is not a super user.
        /// </returns>
        /// <exception cref="ConfigurationFault">Some error with the DB.</exception>
        /// <exception cref="UnknownFault">For any other error.</exception>

        [OperationContract]
        [FaultContract(typeof(ConfigurationFault))]
        [FaultContract(typeof(UnknownFault))]

        string GetFileCollectionServerDirectory(string path);



        /// <summary>
        /// The 2nd level of the Grades by Student Report. That is, the registrations for a user.
        /// </summary>
        /// <param name="path">The path to run the report on.</param>
        /// <param name="pageInfo">The paging info.</param>
        /// <param name="userId">The user id of the user to find registrations for.</param>
        /// <param name="sortDirection">The sort direction.</param>
        /// <param name="sortBy">The column to sort by.</param>
        /// <param name="filterReportOptions">The filter report options.</param>
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
        //////////[FaultContract(typeof(AuthenticationRequiredFault))]

        //////////CollectionSubset<StudentGradeBookRegistration> FindRegistrationsGradeReport(string path, DTO.PageInfo pageInfo, Guid userId,
        //////////    SortDirectionEnum sortDirection, RegistrationAndStudentGradeBookSortTypesEnum sortBy, FilterReportOptions filterReportOptions);

        /// <summary>
        /// Will get OrgInfo for current path. Will search up the current path searching for all NavPages tagged with Organization. 
        /// This will pull org info from the closest NavPage first and continue looking up to fill in all org info.
        /// </summary>
        /// <param name="path">current path</param>
        /// <returns>OrgInfo</returns>

        [OperationContract]
        [FaultContract(typeof(PathFormatFault))]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(ConfigurationFault))]
        [FaultContract(typeof(UnknownFault))]

        OrgInfo GetOrgInfo(string path);

        /// <summary>
        /// Retrieve customer page sizes attribute and convert its values to a list of int
        /// </summary>
        /// <param name="path">path of calling page</param>
        /// <returns>list of page size choices</returns>

        [OperationContract]
        [FaultContract(typeof(NotANavPageFault))]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(PathFormatFault))]
        [FaultContract(typeof(UnknownFault))]

        List<string> GetPageSizeAttributeList(string path);


        /// <summary>
        /// Send feedback email.
        /// </summary>
        /// <param name="path">Path to send the feedback for</param>
        /// <param name="linkUrlName">Optional: link url name if we have feedback within a page.</param>
        /// <param name="contactInformation">User contact information (set by user)</param>
        /// <param name="feedbackText">Feedback text (set by the user).</param>
        /// <exception cref="NotFoundException">If the path doesn't exist.</exception>
        /// <exception cref="PathFormatFault">If the path has a wrong format.</exception>
        /// <exception cref="MisconfigurationException">If path is not within an organization or no feedback email address has been defined.</exception>

        [OperationContract]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(PathFormatFault))]
        [FaultContract(typeof(ConfigurationFault))]
        [FaultContract(typeof(UnknownFault))]

        void SendFeedback(string path, string linkUrlName, string contactInformation, string feedbackText);


        /// <summary>
        /// Get tagged content for the given source tags and tag mappings.
        /// Tag Mappings can be null. If Mappings was null, source tags are considered as target tags.
        /// If Mapping was not null, we should find target tags based on given source tags and mappings.
        /// </summary>
        /// <param name="path">Organization path</param>
        /// <param name="simpleContentPath">like: Courses/Induction/index_lms.html</param>
        /// <param name="mappings"></param>
        /// <param name="tagSetTagNames">it is a dictionary of TagSetNames and TagNames. key=TagSetName, value=TagName. </param>
        /// <exception cref="PathFormatFault"></exception>
        /// <exception cref="OrganizationNotFoundFault"></exception>
        /// <exception cref="NotAuthorizedFault"></exception>
        /// <exception cref="NotFoundFault"></exception>

        [OperationContract]
        [FaultContract(typeof(OrganizationNotFoundException))]
        [FaultContract(typeof(NotFoundException))]
        [FaultContract(typeof(NotAuthorizedFault))]
        [FaultContract(typeof(PathFormatFault))]

        List<DTO.TaggedContent> GetTaggedContent(string path, string simpleContentPath, List<string> mappings, Dictionary<string, string> tagSetTagNames);

        /// <summary>
        /// Endpoint verification function for installer. 
        /// </summary>
        /// <returns>true</returns>

        [OperationContract]
        [FaultContract(typeof(UnknownFault))]
        [FaultContract(typeof(ConfigurationFault))]

        bool IsContentServiceSecure();

        #region Tag Service Operations

        [OperationContract]
        List<TagSet> GetTagSetTree(string path, string tagSetNameOrId = null);

        [OperationContract]
        Guid? AddTagSet(string path, string name, string displayName, TagLifeTimes tagLifetime);

        [OperationContract]
        Guid? AddTag(string path, string name, string displayName, Guid tagSetId);

        [OperationContract]
        void UpdateTagSet(string path, TagSet tagSet);

        [OperationContract]
        void UpdateTag(string path, Tag tag);

        [OperationContract]
        void DeleteTagSet(string path, TagSet tagSet);

        [OperationContract]
        void DeleteTagSetById(string path, Guid tagSetId);

        [OperationContract]
        void DeleteTag(string path, Tag tag);

        [OperationContract]
        void DeleteTagById(string path, Guid tagId);

        #endregion
    }
}
