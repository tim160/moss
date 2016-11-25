using System;
using System.Collections.Generic;
using System.IO;
using System.ServiceModel;
using EC.Constants;
using EC.Errors;
using EC.Errors.CommonExceptions;
using EC.Errors.FileExceptions;
using EC.Errors.ImportAndSyncExceptions;
using EC.Errors.ECExceptions;
using EC.Service.DTO;

namespace EC.Service.Interfaces
{
    /// <summary>
    /// This interface defines the web service operations for accessing information from
    /// the LMS content tree.
    /// </summary>

    [ServiceContract]
    public interface IContentServiceAdmin
    {

     
    
        /// <summary>
        /// Return all registration completion rule attributes. 
        /// </summary>
        /// <param name="coursePath">Course path to get the rules from.</param>
        /// <returns>Return list of rules. Return empty list if no rules exist.</returns>
        /// <exception cref="NotACourseFault">If the <paramref name="coursePath"/> is not a course.</exception>
        /// <exception cref="NotFoundFault">If the <paramref name="coursePath"/> doesn't exist.</exception>
        /// <exception cref="UnknownFault">On any other error</exception>

        [OperationContract]
        [FaultContract(typeof(NotACourseFault))]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(UnknownFault))]

        IList<string> GetRegistrationCompletionRules(string coursePath);

        /// <summary>
        /// Import content/users/audit/offerings/registrations and more.
        /// This entry point is used to restore/clone data. RTA uses this entry point
        /// as well for data synchronization.
        /// </summary>
        /// <param name="parentNavPagePath">Path where the link should be imported to</param>
        /// <param name="linkUrlName">Link URL name of the link on the NavPage</param>
        /// <param name="linkDisplayName">Display name on the parent NavPage for the link</param>
        /// <param name="groupName">Group name where the link should be put in</param>
        /// <param name="directoryPath">Absolute directory from where to read the data</param>
        /// <param name="importMode">Mode of import (e.g. Restore)</param>
        /// <exception cref="DirectoryDoesNotExistFault">If the import directory doesn't exist.</exception>
        /// <exception cref="ImportFault">Any error during import (contains the file/directory path where the error happened)</exception>
        /// <exception cref="ConfigurationFault">If the DB is not setup correctly</exception>
        /// <exception cref="UnknownFault">On any other error</exception>

        [OperationContract]
        [FaultContract(typeof(DirectoryDoesNotExistFault))]
        [FaultContract(typeof(ImportFault))]
        [FaultContract(typeof(ConfigurationFault))]
        [FaultContract(typeof(UnknownFault))]

        void Import(string parentNavPagePath, string linkUrlName, string linkDisplayName, string groupName, string directoryPath, ImportModesEnum importMode);

        /// <summary>
        /// Export content/registrations/audit entries/users and more.
        /// This entry point is used to export data. RTA uses this entry point
        /// as well for data synchronization.
        /// </summary>
        /// <remarks>
        /// Parameters are removed therefor we have added export modes
        /// </remarks>
        /// <param name="path">Path to export</param>
        /// <param name="destDirectoryPath">File system directory to export to (try to create the directory if it doesn't exist).</param>
        /// <param name="exportMode">Mode of export (i.e. Full, MainToRta, Custom,...).</param>
        /// <exception cref="NotFoundFault">If the <paramref name="path"/> doesn't exist.</exception>
        /// <exception cref="DirectoryCreationFault">If the <paramref name="destDirectoryPath"/> couldn't be created.</exception>
        /// <exception cref="DirectoryAlreadyExistsFault">If the export directory already exists - you must choose a non-existing directory.</exception>
        /// <exception cref="ConfigurationFault">If the DB or other configuration is missing</exception>
        /// <exception cref="UnknownFault">On any other error.</exception>

        [OperationContract]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(DirectoryCreationFault))]
        [FaultContract(typeof(DirectoryAlreadyExistsFault))]
        [FaultContract(typeof(ConfigurationFault))]
        [FaultContract(typeof(UnknownFault))]

        void ExportNavPage(string path, string destDirectoryPath, ExportModesEnum exportMode);

        /// <summary>
        /// Gets the path for organization (look along <paramref name="pathIn"/>).
        /// </summary>
        /// <param name="pathIn">The path</param>
        /// <returns>Return organization path. Return <c>null</c> if no organization is found along <paramref name="pathIn"/></returns>

        [OperationContract]
        [FaultContract(typeof(PathFormatFault))]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(ConfigurationFault))]
        [FaultContract(typeof(UnknownFault))]

        string GetPathForOrganization(string pathIn);

        /// <summary>
        /// Will get OrgInfo for current path. Will search up the current path searching for all NavPages tagged with Organization. 
        /// This will pull org info from the closest NavPage first and continue looking up to fill in all org info.
        /// </summary>
        /// <param name="path">current path</param>
        /// <returns>OrgInfo</returns>
        /// <exception cref="PathFormatFault">If the <paramref name="path"/> is faulty</exception>
        /// <exception cref="NotFoundFault">If the path doesn't exist.</exception>
        /// <exception cref="ConfigurationFault">If the DB or other configuration is missing</exception>
        /// <exception cref="UnknownFault">On any other error.</exception>

        [OperationContract]
        [FaultContract(typeof(PathFormatFault))]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(ConfigurationFault))]
        [FaultContract(typeof(UnknownFault))]

        OrgInfo GetOrgInfo(string path);



        /// <summary>
        /// Get paths for all course descendants
        /// </summary>
        /// <exception cref="NullPathException">if the path is null</exception>
        /// <exception cref="PathFormatException">if there are syntactive problems with the path</exception>
        /// <exception cref="NotFoundException">if there is no item corresponding to the path</exception>
        /// <exception cref="WrongLinkTypeException">if a link in the path points to the wrong type of item</exception>

        [OperationContract]
        [FaultContract(typeof(PathFormatFault))]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(WrongLinkTypeFault))]
        [FaultContract(typeof(UnknownFault))]
        CollectionSubset<string> GetDescendantCoursePathsFromPath(string path);


        /// <summary>
        /// Get NavPage path entries for a path. All path entries are returned along the path.
        /// </summary>
        /// <param name="path">Path to get the path entries from</param>
        /// <returns>List of path and sub-paths incl. path entries.</returns>
        /// <exception cref="PathFormatFault">If the <paramref name="path"/> is faulty</exception>
        /// <exception cref="NotFoundFault">If the path doesn't exist.</exception>
        /// <exception cref="UnknownFault">On any other error.</exception>

        //////[OperationContract]
        //////[FaultContract(typeof(PathFormatFault))]
        //////[FaultContract(typeof(NotFoundFault))]
        //////[FaultContract(typeof(UnknownFault))]

        //////List<Tuple<string, List<NavPagePathEntry>>> GetNavPagePathEntriesByPath(string path);

        ///////// <summary>
        /// Remove file collection from <paramref name="path"/> if it exists.
        /// Do nothing if the <paramref name="path"/> doesn't have a file collection.
        /// </summary>
        /// <param name="path">Path must be a NavPage</param>
        /// <exception cref="NotFoundFault">If the item at <paramref name="path"/> doesn't exist.</exception>
        /// <exception cref="PathFormatFault">If the path has a wrong format</exception>
        /// <exception cref="NotANavPageFault">If the <paramref name="path"/> is not a NavPage</exception>
        /// <exception cref="WrongLinkTypeFault">If the link type of <paramref name="path"/> is not a NavPageLink</exception>
        /// <exception cref="UnknownFault">On any other error</exception>

        [OperationContract]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(PathFormatFault))]
        [FaultContract(typeof(NotANavPageFault))]
        [FaultContract(typeof(WrongLinkTypeFault))]
        [FaultContract(typeof(UnknownFault))]

        void DeleteFileCollectionByPath(string path);

        /// <summary>
        /// Validate all GroupPathEntries for NavPageItems of the NavPage at <paramref name="path"/>.
        /// <remarks>
        /// If the NavPage is valid, nothing is returned.
        /// Only in case of an invalid state a an InConsistencyFault is thrown.
        /// </remarks>
        /// </summary>
        /// <param name="path">Path to the NavPage</param>
        /// <exception cref="NotFoundFault">If the <paramref name="path"/> doesn't exist.</exception>
        /// <exception cref="PathFormatFault">If the <paramref name="path"/> has a wrong format</exception>
        /// <exception cref="NotANavPageFault">If the <paramref name="path"/> is not a path to a NavPage</exception>
        /// <exception cref="WrongLinkTypeFault">If the link type of <paramref name="path"/> is not a NavPageLink</exception>
        /// <exception cref="DataConsistencyFault">If a GroupPathEntry reference a non-existing groupId.</exception>
        /// <exception cref="UnknownFault">On an unsuspected error</exception>

        [OperationContract]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(PathFormatFault))]
        [FaultContract(typeof(NotANavPageFault))]
        [FaultContract(typeof(WrongLinkTypeFault))]
        [FaultContract(typeof(DataConsistencyFault))]
        [FaultContract(typeof(UnknownFault))]

        void ValidateNavPage(string path);


        /// <summary>
        /// Return the index page specified by the given path.
        /// </summary>
        /// <remarks>
        /// Details levels available are "default", "links". 
        /// The "links" detail level includes both the content groups and the content links (incl. short summary) for an IndexPage, in display order.
        /// The "links" detail level includes the root entry and entry tree under it, in display order. 
        /// </remarks>
        /// <param name="path">absolute path (path separator is '/')</param>
        /// <param name="detailLevel">Serialization level (see remarks for details).</param>
        /// <returns>The index page</returns>
        /// <exception cref="NotFoundFault">If the path (item) doesn't exist.</exception>
        /// <exception cref="PathFormatFault">If the path has a wrong format.</exception>
        /// <exception cref="UnknownFault">For any other error.</exception>

        [OperationContract]
        [FaultContract(typeof(UnknownFault))]
        [FaultContract(typeof(PathFormatFault))]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(ConfigurationFault))]

        BasicItem GetItemByPath(string path, string detailLevel);


        /// <summary>
        /// Copies a NavPage hierarchy from one location to another. Duplicate NavPages are
        /// created for each page in the hierarchy. 
        /// Attributes(including permissions), FileCollections and QuestionCollections are copied as well.
        /// </summary>
        /// <remarks>
        /// No course offerings and registrations are copied.
        /// </remarks>
        /// <param name="srcPath">Path where to copy the page from</param>
        /// <param name="destPath">Destination NavPage path</param>
        /// <param name="groupId">Optional groupId into which to put the new link</param>
        /// <param name="displayPosition">Display position of the new link.</param>
        /// <param name="newLink">New link which is added to <paramref name="destPath"/> and link to the copy of <paramref name="srcPath"/></param>
        /// <exception cref="NavPageTagConflictException">If the source can't be copied to the destination because the NavPage tags would conflict 
        /// (i.e. the copied NavPage is a Course and the destination path already has a Course.</exception>
        /// <exception cref="InvalidReferenceFault">If a link references an item which is outside the copy boundaries or an attribute value path references outside the copy boundaries. </exception>
        /// <exception cref="NotFoundFault">If either <paramref name="destPath"/>, <paramref name="srcPath"/> or <paramref name="groupId"/> doesn't exist</exception>
        /// <exception cref="PathFormatFault">thrown if either <paramref name="srcPath"/> or <paramref name="destPath"/> is not syntactically correct</exception>
        /// <exception cref="AlreadyExistsFault">If the UrlName of the new link already exists at the destination NavPage.</exception>
        /// <exception cref="UnknownFault">On any other error.</exception>

        //////[OperationContract]
        //////[FaultContract(typeof(NavPageTagConflictFault))]
        //////[FaultContract(typeof(InvalidReferenceFault))]
        //////[FaultContract(typeof(NotFoundFault))]
        //////[FaultContract(typeof(PathFormatFault))]
        //////[FaultContract(typeof(AlreadyExistsFault))]
        //////[FaultContract(typeof(UnknownFault))]

        //////void CopyNavPage(string srcPath, string destPath, Guid? groupId, int displayPosition, NavPageLink newLink);

        /// <summary>
        /// Moves a NavPage hierarchy from one location to another. 
        /// The Attributes (including permissions) are moved to the new link as well.
        /// </summary>
        /// <param name="srcPath">Path to the link which is moved</param>
        /// <param name="destPath">NavPage path to the destination where the link is moved to</param>
        /// <param name="groupId">Optional groupId into which to put the new link</param>
        /// <param name="displayPosition">Display position of the new link</param>
        /// <param name="newLink">New link which is added to <paramref name="destPath"/> and link to the <paramref name="srcPath"/></param>
        /// <exception cref="NavPageTagConflictException">If the source can't be moved to the destination because the NavPage tags would conflict 
        /// (i.e. the moved NavPage is a Course and the destination path already has a Course.</exception>
        /// <exception cref="InvalidReferenceFault"> If a link references an item which is outside the copy boundaries or an attribute value path references outside the move boundaries.</exception>
        /// <exception cref="NotFoundFault">If either <paramref name="destPath"/>, <paramref name="srcPath"/> or <paramref name="groupId"/> doesn't exist</exception>
        /// <exception cref="PathFormatFault">thrown if either <paramref name="srcPath"/> or <paramref name="destPath"/> is not syntactically correct</exception>
        /// <exception cref="AlreadyExistsFault">If the UrlName of the new link already exists at the destination NavPage.</exception>
        /// <exception cref="UnknownFault">On any other error.</exception>

        //////[OperationContract]
        //////[FaultContract(typeof(NavPageTagConflictFault))]
        //////[FaultContract(typeof(InvalidReferenceFault))]
        //////[FaultContract(typeof(NotFoundFault))]
        //////[FaultContract(typeof(PathFormatFault))]
        //////[FaultContract(typeof(AlreadyExistsFault))]
        //////[FaultContract(typeof(UnknownFault))]

        //////void MoveNavPage(string srcPath, string destPath, Guid? groupId, int displayPosition, NavPageLink newLink);

        /// <summary>
        /// Move a file collection disk location using the new <paramref name="baseDirectory"/>.
        /// </summary>
        /// <remarks>
        /// The new <c>RootPath</c> is created from the url name of the NavPage, organization short name
        /// and the existing version number <c>Version</c>.
        /// If no organization exists along the <paramref name="path"/> 'NoOrg' will be used to build the 
        /// new directory. 
        /// </remarks>
        /// <param name="path">Path to the file collection</param>
        /// <param name="baseDirectory"></param>
        /// <returns>Return the full new file collection disk location</returns>
        /// <exception cref="NotANavPageFault"></exception>
        /// <exception cref="ContentTypeMismatchFault"></exception>
        /// <exception cref="NotFoundFault">If the <paramref name="path"/> doesn't exist</exception>
        /// <exception cref="PathFormatFault">thrown if <paramref name="path"/> is not syntactically correct</exception>
        /// <exception cref="FileCollectionNotFoundFault">If no file collection exists at <paramref name="path"/></exception>
        /// <exception cref="RelativePathFault">If the <paramref name="baseDirectory"/> is not rooted (absolute).</exception>
        /// <exception cref="UnknownFault">On any other error.</exception>

        [OperationContract]
        [FaultContract(typeof(NotANavPageFault))]
        [FaultContract(typeof(ContentTypeMismatchFault))]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(PathFormatFault))]
        [FaultContract(typeof(FileCollectionNotFoundFault))]
        [FaultContract(typeof(RelativePathFault))]
        [FaultContract(typeof(UnknownFault))]

        string MoveFileCollection(string path, string baseDirectory);

        /// <summary>
        /// Get the server directory for the file collection at <paramref name="path"/>.
        /// This only returns the server path if the file collection exist.
        /// </summary>
        /// <param name="path">Path to get the file collection server directory</param>
        /// <returns>
        /// Return the server path for the file collection at <paramref name="path"/>.
        /// Return <c>null</c> if no file collection exists, the <paramref name="path"/> doesn't exist or an other error happens.
        /// </returns>
        /// <exception cref="UnknownFault">On any other error.</exception>

        [OperationContract]
        [FaultContract(typeof(UnknownFault))]

        string GetFileCollectionServerDirectory(string path);

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
        [FaultContract(typeof(ConfigurationFault))]
        [FaultContract(typeof(PathFormatFault))]
        [FaultContract(typeof(UnknownFault))]

        string GetCSSPathForPage(string path);

        /// <summary>
        /// Create a file collection on a nav page.
        /// </summary>
        /// <param name="path">the path to the nav page where the file collection will be created</param>
        /// <param name="name">the name of file collection</param>
        /// <returns>the FileCollection object representing the file collection</returns>
        /// <exception cref="NotFoundFault">thrown if the path does not correspond to a ContentItem</exception>
        /// <exception cref="NotANavPageFault">thrown if the element at the <paramref name="path"/> is not a nav page.</exception>
        /// <exception cref="PathFormatFault">thrown if the path is not syntactically correct</exception>
        /// <exception cref="ConfigurationFault">Some error with the DB.</exception>
        /// <exception cref="AlreadyExistsFault">thrown if the file collection for the nav page already exists.</exception>
        /// <exception cref="UnknownFault">For any other error.</exception>

        [OperationContract]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(NotANavPageFault))]
        [FaultContract(typeof(PathFormatFault))]
        [FaultContract(typeof(ConfigurationFault))]
        [FaultContract(typeof(AlreadyExistsFault))]
        [FaultContract(typeof(UnknownFault))]

        FileCollection CreateFileCollection(string path, string name);

        /// <summary>
        /// Create a QuestionCollection on the NavPage specified by the given path. This will
        /// also create an associated FileCollection on the NavPage which will be used for
        /// storing content (text, html, images, etc) that are used for both questions
        /// and answers.
        /// </summary>
        /// <param name="path">path to NavPage on which collection should be created</param>
        /// <param name="name">name for the new collection</param>
        /// <exception cref="ParameterValidationException">if the name for the repository name is invalid</exception>
        /// <exception cref="NotFoundException">if there is not item with the given path</exception>
        /// <exception cref="PathFormatException">if the path is not valid</exception>
        /// <exception cref="NotANavPageException">if the path does not specify a Navpage</exception>

        [OperationContract]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(PathFormatFault))]
        [FaultContract(typeof(NotANavPageFault))]
        [FaultContract(typeof(ParameterValidationFault))]

        void CreateQuestionRepository(string path, string name);

        /// <summary>
        /// Deletes the question collection associated with the NavPage specified by the path.
        /// </summary>
        /// <param name="path">path to current NavPage</param>
        /// <exception cref="NotFoundFault">If the NavPage or QuestionCollection cannot be found</exception>
        /// <exception cref="PathFormatFault">If the path is not valid</exception>
        /// <exception cref="NotANavPageFault">If the item does exist, but it is not a NavPage.</exception>
        /// <exception cref="ConfigurationFault">Some error with the DB.</exception>
        /// <exception cref="WrongLinkTypeFault">if a link on the path points to the wrong type of item</exception>
        /// <exception cref="ContentTypeMismatchFault">If the NavPageLink doesn't point to a NavPage</exception>

        [OperationContract]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(PathFormatFault))]
        [FaultContract(typeof(NotANavPageFault))]
        [FaultContract(typeof(ConfigurationFault))]
        [FaultContract(typeof(WrongLinkTypeFault))]
        [FaultContract(typeof(ContentTypeMismatchFault))]
        [FaultContract(typeof(UnknownFault))]

        void DeleteQuestionRepository(string path);

        /// <summary>
        /// Check a capability on a node by path (and shared Id).
        /// </summary>
        /// <param name="path">Path to a content node.</param>
        /// <param name="sharedNavPageId">Optional shared NavPage Id</param>
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

        bool CheckCapabilityByPath(string path, Guid? sharedNavPageId, string capabilityName);

        /// <summary>
        /// Check a given content item (can be shared) for multiple permissions at once.
        /// </summary>
        /// <param name="path">the item to check</param>
        /// <param name="sharedNavPageId">Optional shared NavPage Id</param>
        /// <param name="capabilities">the capabilities to check</param>
        /// <returns>the set of capabilities that the user has</returns>

        [OperationContract]
        [FaultContract(typeof(CapabilityNotFoundFault))]
        [FaultContract(typeof(InvalidCapabilityFault))]
        [FaultContract(typeof(PathFormatFault))]
        [FaultContract(typeof(UnknownFault))]

        List<string> CheckCapabilitiesByPath(string path, Guid? sharedNavPageId, IList<string> capabilities);

        /// <summary>
        /// Check whether a specific user has a given capability on a given item.
        /// </summary>
        /// <param name="path">path to item to check</param>
        /// <param name="userId">User Id</param>
        /// <param name="capabilityName">the capability to check</param>
        /// <returns>whether the user has the given capability</returns>
        /// <exception cref="NotFoundFault">If the path (item) doesn't exist or the user doesn't exist</exception>
        /// <exception cref="PathFormatFault">If the path has a wrong format.</exception>
        /// <exception cref="WrongLinkTypeFault">If a link in the path points to the wrong type of item</exception>

        [OperationContract]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(PathFormatFault))]
        [FaultContract(typeof(WrongLinkTypeFault))]
        [FaultContract(typeof(UnknownFault))]

        bool CheckCapabilityByPathAndUser(string path, Guid userId, string capabilityName);

        #region NavPage Attributes
        /// <summary>
        /// Add attributes to a NavPage. If attributes already exist - add the new attributes to the existing ones.
        /// <b>Note:</b> Use <see cref="GetNavPageByPath"/> with <c>detailLevel</c> = 'attributes' to get the attributes of a page.
        /// </summary>
        /// <param name="path">Path to the content item.</param>
        /// <param name="attributes">List of attributes to be set for the content item.</param>
        /// <exception cref="NotFoundFault">If the path does not exist.</exception>
        /// <exception cref="PathFormatFault">If the path has a wrong format or is not a content item.</exception>
        /// <exception cref="AlreadyExistsFault">If the exact same attribute already exist (double entry).</exception>
        /// <exception cref="WrongCapabilityAttributeFormatFault">If a capability attribute has a wrong format.</exception>
        /// <exception cref="AttributeFormatFault">If a group definition attribute has a wrong format.</exception>
        /// <exception cref="UnknownFault">For any other error.</exception>

        [OperationContract]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(PathFormatFault))]
        [FaultContract(typeof(AlreadyExistsFault))]
        [FaultContract(typeof(WrongCapabilityAttributeFormatFault))]
        [FaultContract(typeof(AttributeFormatFault))]
        [FaultContract(typeof(UnknownFault))]

        void AddNavPageAttributesByPath(string path, IList<AttributeItem> attributes);

        /// <summary>
        /// Get all NavPage attributes (ordered by their Guid).
        /// </summary>
        /// <param name="path">Path to the NavPage</param>
        /// <returns>Return list of attributes.</returns>
        /// <exception cref="NotANavPageFault">If the <paramref name="path"/> is not a NavPage</exception>
        /// <exception cref="PathFormatFault">If the path has the wrong format.</exception>
        /// <exception cref="NotFoundFault">If the <paramref name="path"/> doesn't exist</exception>
        /// <exception cref="UnknownFault">For any other error.</exception>

        [OperationContract]
        [FaultContract(typeof(NotANavPageFault))]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(PathFormatFault))]
        [FaultContract(typeof(UnknownFault))]

        List<AttributeItem> GetNavPageAttributesByPath(string path);

        /// <summary>
        /// Gets value of nav page attribute
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(NotANavPageFault))]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(PathFormatFault))]
        [FaultContract(typeof(UnknownFault))]
        string GetNavPageAttribute(string navPagePath, string attributeKey);

        /// <summary>
        /// Delete an attribute from a NavPage at the specified <paramref name="attributePosition"/>.
        /// </summary>
        /// <remarks>
        /// Use <see cref="GetNavPageAttributesByPath"/> to get a list of all attributes.
        /// The order they are returned can be used to set the <paramref name="attributePosition"/>
        /// </remarks>
        /// <param name="path">Path to the link</param>
        /// <param name="attributePosition">Zero-based position in the list from <see cref="GetNavPageAttributesByPath"/></param>
        /// <exception cref="NotANavPageFault">If the <paramref name="path"/> is not a NavPage</exception>
        /// <exception cref="PathFormatFault">If the path has the wrong format.</exception>
        /// <exception cref="NotFoundFault">If the <paramref name="path"/> doesn't exist or the attribute at <paramref name="attributePosition"/> doesn't exist.</exception>
        /// <exception cref="UnknownFault">For any other error.</exception>

        [OperationContract]
        [FaultContract(typeof(NotANavPageFault))]
        [FaultContract(typeof(PathFormatFault))]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(UnknownFault))]

        void DeleteNavPageAttribute(string path, int attributePosition);

        /// <summary>
        /// Update attribute for NavPage [path] at [position] with [value]
        /// <para>
        /// Permissions: Requires the CAN_MANAGE_LINK_ATTRIBUTES capability on the given NavPage.
        /// </para>
        /// </summary>
        /// <param name="path">path to the link</param>
        /// <param name="position">zero based position of attribute key, useful when duplicate keys present</param>
        /// <param name="value">value to set</param>

        [OperationContract]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(PathFormatFault))]
        [FaultContract(typeof(UnknownFault))]

        void SetNavPageAttributeByPosition(string path, int position, string value);

        /// <summary>
        /// Sets the value of nav page attribute, creating it if necessary
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(PathFormatFault))]
        [FaultContract(typeof(WrongCapabilityAttributeFormatFault))]
        [FaultContract(typeof(AttributeFormatFault))]
        [FaultContract(typeof(UnknownFault))]
        void SetNavPageAttribute(string navPagePath, string attributeKey, string attributeValue);

        #endregion

        #region Link Attributes
        /// <summary>
        /// Add attributes to a link. If attributes already exist - add the new attributes to the existing ones.
        /// <b>Note:</b> Use <see cref="GetLinkByPath" /> with <c>detailLevel</c> = 'attributes' to get the attributes of the link.
        /// </summary>
        /// <param name="path">Path to the link.</param>
        /// <param name="attributes">List of attributes to be set for the link.</param>
        /// <exception cref="NotFoundFault">If the path does not exist.</exception>
        /// <exception cref="PathFormatFault">If the path has a wrong format.</exception>
        /// <exception cref="AlreadyExistsFault">If the exact same attribute already exist (double entry).</exception>
        /// <exception cref="WrongCapabilityAttributeFormatFault">If a capability attribute has a wrong format.</exception>
        /// <exception cref="UnknownFault">For any other error.</exception>

        [OperationContract]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(PathFormatFault))]
        [FaultContract(typeof(AlreadyExistsFault))]
        [FaultContract(typeof(AttributeFormatFault))]
        [FaultContract(typeof(WrongCapabilityAttributeFormatFault))]
        [FaultContract(typeof(UnknownFault))]

        void AddLinkAttributesByPath(string path, IList<AttributeItem> attributes);

        /// <summary>
        /// Get all link attributes (ordered by their Guid).
        /// </summary>
        /// <param name="path">Path to the link</param>
        /// <returns>Return list of attributes.</returns>
        /// <exception cref="PathFormatFault">If the path has the wrong format.</exception>
        /// <exception cref="NotFoundFault">If the <paramref name="path"/> doesn't exist.</exception>
        /// <exception cref="UnknownFault">On any other error</exception>

        [OperationContract]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(PathFormatFault))]
        [FaultContract(typeof(UnknownFault))]

        List<AttributeItem> GetLinkAttributesByPath(string path);

        /// <summary>
        /// Delete an attribute from a link at the specified <paramref name="attributePosition"/>.
        /// </summary>
        /// <remarks>
        /// Use <see cref="GetLinkAttributesByPath"/> to get a list of all link attributes.
        /// The order they are returned can be used to set the <paramref name="attributePosition"/>
        /// </remarks>
        /// <param name="path">Path to the link</param>
        /// <param name="attributePosition">Zero-based position in the list from <see cref="GetLinkAttributesByPath"/></param>
        /// <exception cref="PathFormatFault">If the path has the wrong format.</exception>
        /// <exception cref="NotFoundFault">If the <paramref name="path"/> doesn't exist or the attribute at <paramref name="attributePosition"/> doesn't exist.</exception>
        /// <exception cref="UnknownFault">On any other error</exception>

        [OperationContract]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(PathFormatFault))]
        [FaultContract(typeof(UnknownFault))]

        void DeleteLinkAttribute(string path, int attributePosition);

        /// <summary>
        /// Delete all attribute with <paramref name="key"/> on <paramref name="path"/> which should lead to the page the attributes are supposed to be deleted from.
        /// </summary>
        /// <param name="path">path to page those attributes live on</param>
        /// <param name="key">key for attributes to be deleted</param>
        /// <param name="valueSubString">subString of attribute value, to be able to narrow the attribute list further down</param>
        /// <returns>Return deleted attribute count</returns>
        /// <exception cref="PathFormatFault">If the path has the wrong format.</exception>
        /// <exception cref="NotFoundFault">If the <paramref name="path"/> doesn't exist.</exception>

        [OperationContract]
        [FaultContract(typeof(PathFormatFault))]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(UnknownFault))]

        int DeleteLinkAttributes(string path, string key, string valueSubString);

        /// <summary>
        /// Update attribute for link [path] (NavPageLink, ExternalLink, ...) at [position] with [value]
        /// <para>
        /// Permissions: Requires the CAN_MANAGE_LINK_ATTRIBUTES capability on the given link.
        /// </para>
        /// </summary>
        /// <param name="path">path to the link</param>
        /// <param name="position">zero based position of attribute key, useful when duplicate keys present</param>
        /// <param name="value">value to set</param>

        [OperationContract]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(PathFormatFault))]
        [FaultContract(typeof(UnknownFault))]

        void SetLinkAttribute(string path, int position, string value);
        #endregion

        /// <summary>
        /// Add attributes to a content item (e.g. NavPage, FileCollection). The attribute keys MUST be a capability.
        /// If attributes already exist - add the new attributes to the existing ones.
        /// The <c>path</c> must point to a content item (e.g. NavPage, FileCollection).
        /// <b>Note:</b> Use <see cref="GetNavPageByPath"/> with <c>detailLevel</c> = 'attributes' to get the attributes of a page.
        /// </summary>
        /// <param name="path">Path to the content item.</param>
        /// <param name="attributes">List of attributes to be set for the content item.</param>
        /// <exception cref="NotFoundFault">If the path does not exist.</exception>
        /// <exception cref="PathFormatFault">If the path has a wrong format or not a content item.</exception>
        /// <exception cref="AlreadyExistsFault">If the exact same attribute already exist (double entry).</exception>
        /// <exception cref="WrongCapabilityAttributeFormatFault">If a capability attribute has a wrong format.</exception>
        /// <exception cref="UnknownFault">For any other error.</exception>

        [OperationContract]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(PathFormatFault))]
        [FaultContract(typeof(AlreadyExistsFault))]
        [FaultContract(typeof(WrongCapabilityAttributeFormatFault))]
        [FaultContract(typeof(UnknownFault))]

        void AddNavPageCapabilitiesByPath(string path, IList<AttributeItem> attributes);

        /// <summary>
        /// Load an entire sub-tree into the content tree off of disk.
        /// </summary>
        /// <remarks>
        /// Users which are imported with the content tree and where deleted or inactive are un-deleted and set to active.
        /// </remarks>
        /// <param name="directoryPath">path to local folder tree with content</param>
        /// <param name="indexPath">path to index page where content will be loaded</param>
        /// <param name="groupName">name of group into which link to imported content should go</param>
        /// <param name="linkName">name of link that will point to imported contents</param>
        /// <param name="userHomePath">Path for the users to set if users are imported. Set to <c>null</c> to leave all paths unchanged or not be set for a new user.</param>
        /// <exception cref="WrongCapabilityAttributeFormatFault">If a capability attribute has a wrong format.</exception>
        /// <exception cref="AttributeFormatFault">If a group definition attribute has a wrong format.</exception>
        /// <exception cref="NotFoundFault"></exception>

        [OperationContract]
        [FaultContract(typeof(UnknownFault))]
        [FaultContract(typeof(PathFormatFault))]
        [FaultContract(typeof(WrongCapabilityAttributeFormatFault))]
        [FaultContract(typeof(AttributeFormatFault))]
        [FaultContract(typeof(NotFoundFault))]

        void LoadContentTree(string directoryPath, string indexPath, string groupName, string linkName, string userHomePath);

        /// <summary>
        /// Sets up the database with all mandatory fields for system to run
        /// </summary>
        /// <param name="configPath">Path to configuration directory</param>

        [OperationContract]
        [FaultContract(typeof(XMLFormatFault))]

        void SetupBaseDB(string configPath);

        /// <summary>
        /// Edit an existing group name of a NavPage.
        /// </summary>
        /// <param name="path">Path to the NavPage</param>
        /// <param name="groupId">Id of the group to edit</param>
        /// <param name="newGroupName">New group name.</param>
        /// <exception cref="NotFoundFault">thrown if the path does not correspond to an item.</exception>
        /// <exception cref="NotANavPageFault">Item is not a NavPage.</exception>
        /// <exception cref="ConfigurationParameterFault">Group name must be specified.</exception>
        /// <exception cref="ConfigurationFault">Some error with the DB.</exception>

        [OperationContract]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(NotANavPageFault))]
        [FaultContract(typeof(PathFormatFault))]
        [FaultContract(typeof(ConfigurationFault))]
        [FaultContract(typeof(ParameterValidationFault))]
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
        /// <param name="externalLinkUrl">Only used if link is external. Will set the url of the external link.</param>
        /// <param name="allowStudentReview"></param>
        /// <param name="allowRetakeExam">Flag whether to allow the student to retake an exam if he failed it</param>
        /// <param name="retakeAfter">Amount of days after the student can retake the exam</param>
        /// <param name="passingPercentage">Percentage to pass the exam</param>
        /// <param name="password">This password is only used for password protected exam type</param>
        /// <param name="examType"></param>
        /// <exception cref="NotFoundFault">thrown if the path does not correspond to the link item or if the link does not link to a nav page.</exception>
        /// <exception cref="ParameterValidationFault">If <paramref name="newUrlName"/> is not set.</exception>
        /// <exception cref="AlreadyExistsFault">If the urlName already exists on the parent page</exception>
        /// <exception cref="ConfigurationFault">Some error with the DB.</exception>
        /// <exception cref="UnknownFault">For any other error.</exception>
        //////[OperationContract]
        //////[FaultContract(typeof(NotFoundFault))]
        //////[FaultContract(typeof(ParameterValidationFault))]
        //////[FaultContract(typeof(ConfigurationFault))]
        //////[FaultContract(typeof(UnknownFault))]
        //////[FaultContract(typeof(AlreadyExistsFault))]

        //////void EditLinkDetails(string path, string newUrlName, string newDisplayText, string newShortSummary, string newLongSummary, string newIconPath,
        //////    bool isMandatory, string externalLinkUrl, bool allowStudentReview, bool allowRetakeExam, int retakeAfter, double passingPercentage, string password, ExamTypeEnum? examType);

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
        /// <exception cref="ConfigurationFault">Some error with the DB.</exception>
        /// <exception cref="UnknownFault">For any other error.</exception>

        [OperationContract]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(ConfigurationFault))]
        [FaultContract(typeof(UnknownFault))]

        void EditLinkPublishMode(string path, bool isPublished);

        /// <summary>
        /// Edit nav page details (content properties).
        /// </summary>
        /// <param name="path">Path to the nav page (=nav page link)</param>
        /// <param name="newTitle">Page title.</param>
        /// <param name="newHeader">New header text.</param>
        /// <param name="htmlContentNavPage">Html Content Code for Script Tags</param>
        /// <exception cref="NotFoundFault">thrown if the path does not correspond to the nav page.</exception>
        /// <exception cref="ConfigurationFault">Some error with the DB.</exception>
        /// <exception cref="ParameterValidationFault">If the parameters are not valid</exception>
        /// <exception cref="UnknownFault">For any other error.</exception>

        [OperationContract]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(NotANavPageFault))]
        [FaultContract(typeof(ParameterValidationFault))]
        [FaultContract(typeof(ConfigurationFault))]
        [FaultContract(typeof(UnknownFault))]

        void EditNavPageDetails(string path, string newTitle, string newHeader, string htmlContentNavPage);

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
        /// <exception cref="ParameterValidationFault">If <paramref name="newUrlName"/> is not set.</exception>
        /// <exception cref="ConfigurationFault">Some error with the DB.</exception>
        /// <exception cref="UnknownFault">For any other error.</exception>

        [OperationContract]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(NotANavPageFault))]
        [FaultContract(typeof(ParameterValidationFault))]
        [FaultContract(typeof(ConfigurationFault))]
        [FaultContract(typeof(UnknownFault))]

        void EditNavPageDefaults(string path, string newUrlName, string newDisplayText, string newShortSummary, string newLongSummary, string newIconPath, bool isMandatory);

        /// <summary>
        /// Reorder the links and groups of a NavPage.
        /// </summary>
        /// <param name="path">Path to the nav page (= nav page link)</param>
        /// <param name="items">new display items for the NavPage</param>
        /// <exception cref="NotFoundFault">thrown if the path does not correspond to an item.</exception>
        /// <exception cref="NotANavPageFault">Item is not a NavPage.</exception>
        /// <exception cref="PathFormatFault">the path is syntactically incorrect</exception>
        /// <exception cref="WrongLinkTypeFault">this path has a link/target inconsistency</exception>
        /// <exception cref="ConfigurationFault">Some error with the DB.</exception>
        /// <exception cref="UnknownFault">Any other error.</exception>        

        //////[OperationContract]
        //////[FaultContract(typeof(NotFoundFault))]
        //////[FaultContract(typeof(NotANavPageFault))]
        //////[FaultContract(typeof(PathFormatFault))]
        //////[FaultContract(typeof(WrongLinkTypeFault))]
        //////[FaultContract(typeof(ConfigurationFault))]
        //////[FaultContract(typeof(UnknownFault))]

        //////void ReplaceNavPageItems(string path, IList<NavPageItem> items);

        /// <summary>
        /// Add a new group item to the nav page.
        /// </summary>
        /// <param name="path">Path to the nav page (= nav page link)</param>
        /// <param name="targetGroupId">Group Id to add the new group to. Set <c>null</c> to add the new group at the top level of the nav page.</param>
        /// <param name="displayOrder">Set display order of the new group item</param>
        /// <param name="groupName"></param>
        /// <returns>Return the newly created item with correct displayOrder and Id.</returns>
        /// <exception cref="NotFoundFault">thrown if the path does not correspond to an item.</exception>
        /// <exception cref="NotANavPageFault">Item is not a NavPage.</exception>
        /// <exception cref="ConfigurationFault">Some error with the DB.</exception>
        /// <exception cref="ParameterValidationFault">if the parameters are not valid</exception>
        /// <exception cref="UnknownFault">Any other error.</exception>        

        ////[OperationContract]
        ////[FaultContract(typeof(NotFoundFault))]
        ////[FaultContract(typeof(NotANavPageFault))]
        ////[FaultContract(typeof(ParameterValidationFault))]
        ////[FaultContract(typeof(ConfigurationFault))]
        ////[FaultContract(typeof(UnknownFault))]

        ////NavPageItem AddNavPageGroup(string path, Guid? targetGroupId, int displayOrder, string groupName);

        /// <summary>
        /// Delete a NavPageItem from a page (along with its sub-items)
        /// </summary>
        /// <param name="path">Path to the nav page (= nav page link)</param>
        /// <param name="itemId">Id of the NavPageItem to delete</param>
        /// <exception cref="NotFoundFault">thrown if the path does not correspond to an item.</exception>
        /// <exception cref="NotANavPageFault">Item is not a NavPage.</exception>
        /// <exception cref="ConfigurationFault">Some error with the DB.</exception>
        /// <exception cref="CannotDeleteFault">Encountered dangling foreign key references, can't delete item with itemId</exception>
        /// <exception cref="ParameterValidationFault">If the parameters are not valid</exception>
        /// <exception cref="UnknownFault">Any other error.</exception>        

        [OperationContract]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(NotANavPageFault))]
        [FaultContract(typeof(ParameterValidationFault))]
        [FaultContract(typeof(ConfigurationFault))]
        [FaultContract(typeof(CannotDeleteFault))]
        [FaultContract(typeof(UnknownFault))]

        void DeleteNavPageItem(string path, Guid itemId);


        /// <summary>
        /// Add new external link to an existing NavPage. It is also possible to set attributes assigned to the link.
        /// <br/>Note: All Ids are assigned with new Ids.
        /// </summary>
        /// <param name="path">Path to the NavPage (= NavPage link)</param>
        /// <param name="targetGroupId">Group Id to add the new link to. Set <c>null</c> to add the new link at the top level of the nav page.</param>
        /// <param name="displayOrder">Set display order of the new link</param>
        /// <param name="newLink">New external link item (all ids are regenerated).</param>
        /// <returns>Return the DisplayLink with the newly added link.</returns>
        /// <exception cref="NotFoundFault">If the item to add the link to doesn't exist.</exception>
        /// <exception cref="NotANavPageFault">If the path is not a NavPage link (=NavPage).</exception>
        /// <exception cref="AlreadyExistsFault">If the link Url name already exists for this NavPage.</exception>
        /// <exception cref="ParameterValidationFault">If no display text for the link has been set.</exception>
        /// <exception cref="ConfigurationFault">Some error with the DB.</exception>
        /// <exception cref="WrongCapabilityAttributeFormatFault">If a capability attribute has a wrong format.</exception>
        /// <exception cref="AttributeFormatFault">If a group definition attribute has a wrong format.</exception>
        /// <exception cref="UnknownFault">Any other error.</exception>

        ////[OperationContract]
        ////[FaultContract(typeof(NotFoundFault))]
        ////[FaultContract(typeof(NotANavPageFault))]
        ////[FaultContract(typeof(AlreadyExistsFault))]
        ////[FaultContract(typeof(ParameterValidationFault))]
        ////[FaultContract(typeof(ConfigurationFault))]
        ////[FaultContract(typeof(WrongCapabilityAttributeFormatFault))]
        ////[FaultContract(typeof(AttributeFormatFault))]
        ////[FaultContract(typeof(UnknownFault))]

        ////NavPageItem AddNavPageExternalLink(string path, Guid? targetGroupId, int displayOrder, ExternalLink newLink);

        /// <summary>
        /// Add a new NavPage and a link to the new page.
        /// </summary>
        /// <remarks>
        /// Default capabilities for the new NavPage are (hard-coded for the moment):
        /// <br/> - CanView = IsInstructor()
        /// <br/> - CanView = IsRegistered()
        /// <br/> - CanDisplayAsLink = IsLoggedIn()
        /// </remarks>
        /// <param name="path">Path to add the link to (must be a NavPage).</param>
        /// <param name="targetGroupId">Group Id to add the new link to. Set <c>null</c> to add the new link at the top level of the nav page.</param>
        /// <param name="displayOrder">Set display order of the new link</param>
        /// <param name="pageUrlName">Url name for the page (becomes part of the path).</param>
        /// <param name="pageTitle">Name of the new page (the DisplayText will initially be set with this title as well). 
        /// This also will be used as default display text for a new links.</param>
        /// <param name="isMandatoryLink">Flag whether the link is mandatory</param>
        /// <param name="isPublished">Flag whether the new page is published instantly (can be changed afterwards).</param>
        /// <param name="createFileCollection">
        /// Set <c>true</c> if a local file collection should be created at the new page.
        /// Set <c>false</c> if no file collection should be created for the new page.
        /// </param>
        /// <param name="isTOCPage">Flag which content will be added for the new page (default for all links).</param>
        /// <returns>Return the new <c>DispalyLink</c> item with the new link in it.</returns>
        /// <exception cref="NotFoundFault">If the item to add the link to doesn't exist.</exception>
        /// <exception cref="NotANavPageFault">If the path is not a NavPage link (=NavPage).</exception>
        /// <exception cref="AlreadyExistsFault">
        /// If the page Url name (which is used as link.UrlName) already exists as a 
        /// link Url name for this NavPage (the link Url name must be unique within a NavPage).
        /// </exception>
        /// <exception cref="ParameterValidationFault">If no <paramref name="pageUrlName"/> or <paramref name="pageTitle"/> has been set.</exception>
        /// <exception cref="ConfigurationFault">Some error with the DB.</exception>
        /// <exception cref="WrongCapabilityAttributeFormatFault">If a capability attribute has a wrong format.</exception>
        /// <exception cref="AttributeFormatFault">If a group definition attribute has a wrong format.</exception>
        /// <exception cref="UnknownFault">Any other error.</exception>

        //////[OperationContract]
        //////[FaultContract(typeof(NotFoundFault))]
        //////[FaultContract(typeof(NotANavPageFault))]
        //////[FaultContract(typeof(AlreadyExistsFault))]
        //////[FaultContract(typeof(ParameterValidationFault))]
        //////[FaultContract(typeof(ConfigurationFault))]
        //////[FaultContract(typeof(WrongCapabilityAttributeFormatFault))]
        //////[FaultContract(typeof(AttributeFormatFault))]
        //////[FaultContract(typeof(UnknownFault))]

        //////NavPageItem AddNavPageAndLink(string path, Guid? targetGroupId, int displayOrder, string pageUrlName,
        //////    string pageTitle, bool isMandatoryLink, bool isPublished, bool createFileCollection, bool isTOCPage);

        /// <summary>
        /// Upload a file via stream to a temporary directory.
        /// </summary>
        /// <remarks>
        /// WCF only allows one parameter for the service call if you want to pass in a stream. 
        /// For example adding a new file to a file collection is a 2 step process: 
        ///   1.) Upload file to temporary location
        ///   2.) Link/add the new uploaded file into the system (e.g. file collection).
        /// </remarks>
        /// <param name="contentStream">Stream to the file.</param>
        /// <returns>Return a unique Id with which the file can be addressed (e.g. to add it to a file collection).</returns>
        /// <exception cref="ConfigurationFault">Some error with the DB.</exception>
        /// <exception cref="UnknownFault">Any other error.</exception>

        [OperationContract]
        [FaultContract(typeof(ConfigurationFault))]
        [FaultContract(typeof(UnknownFault))]

        Guid UploadFileAsStream(Stream contentStream);

        /// <summary>
        /// Create a question category on the given Navpage.
        /// </summary>
        /// <param name="path">NavPage whose question collection should be modified</param>
        /// <param name="category">The new category's name</param>
        /// <exception cref="NotFoundFault">If the item doesn't exist, or the question collection is not present</exception>
        /// <exception cref="NotANavPageFault">If the item does exist, but it is not a NavPage.</exception>
        /// <exception cref="PathFormatFault">If the path is not valid</exception>
        /// <exception cref="ParameterValidationFault">If the category is null</exception>
        /// <exception cref="DuplicateCategoryFault">thrown if you try to add a category that is already there</exception>

        [OperationContract]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(NotANavPageFault))]
        [FaultContract(typeof(PathFormatFault))]
        [FaultContract(typeof(ParameterValidationFault))]
     //   [FaultContract(typeof(DuplicateCategoryFault))]
        [FaultContract(typeof(UnknownFault))]

        void CreateQuestionCategory(string path, string category);


        /// <summary>
        /// Create a question with the given name, and add it to the QuestionCollection
        /// associated with the NavPage specified by the given path.
        /// </summary>
        /// <param name="path">path to NavPage</param>
        /// <param name="urlName">UrlName for new question</param>
        /// <exception cref="NotFoundFault">If the item doesn't exist, or the question collection is not present</exception>
        /// <exception cref="NotANavPageFault">If the item does exist, but it is not a NavPage.</exception>
        /// <exception cref="PathFormatFault">If the path is not valid</exception>
        /// <exception cref="ParameterValidationFault">If the urlName is null</exception>
        /// <exception cref="AlreadyExistsFault">If the urlName is already being used in this collection</exception>

        [OperationContract]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(NotANavPageFault))]
        [FaultContract(typeof(PathFormatFault))]
        [FaultContract(typeof(AlreadyExistsFault))]
        [FaultContract(typeof(ParameterValidationFault))]
        [FaultContract(typeof(UnknownFault))]

        void CreateQuestion(string path, string urlName);

        /// <summary>
        /// Import a set of question definitions to a Question Collection.
        /// </summary>
        /// <param name="path">path to current NavPage</param>
        /// <param name="XML">question definitions</param>
        /// <exception cref="NotFoundFault">If the NavPage or QuestionCollection cannot be found</exception>
        /// <exception cref="NotANavPageFault">If the item does exist, but it is not a NavPage.</exception>
        /// <exception cref="PathFormatFault">If the path is not valid</exception>
        /// <exception cref="WrongLinkTypeFault">if a link on the path points to the wrong type of item</exception>
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
        [FaultContract(typeof(UnknownFault))]

        void ImportQuestions(string path, string XML);

        /// <summary>
        /// Set the display content for a question.
        /// </summary>
        /// <param name="path">path to NavPage where question lives</param>
        /// <param name="urlName">URL Name of question</param>
        /// <param name="content">display content for question</param>
        /// <exception cref="NotFoundFault">If the item doesn't exist, the question collection is not present, or question cannot be found</exception>
        /// <exception cref="NotANavPageFault">If the item does exist, but it is not a NavPage.</exception>
        /// <exception cref="PathFormatFault">If the path is not valid</exception>
        /// <exception cref="ParameterValidationFault">If the content or urlName is null</exception>
        /// <exception cref="AlreadyExistsFault">If the urlName is already being used in this collection</exception>
        /// <exception cref="DirectoryCreationFault">If a directory on disk cannot be created (for storing content)</exception>
        /// <exception cref="FileWriteFault">If a file on disk cannot be written to (for storing content)</exception>

        [OperationContract]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(NotANavPageFault))]
        [FaultContract(typeof(PathFormatFault))]
        [FaultContract(typeof(AlreadyExistsFault))]
        [FaultContract(typeof(ParameterValidationFault))]
        [FaultContract(typeof(DirectoryCreationFault))]
        [FaultContract(typeof(FileWriteFault))]
        [FaultContract(typeof(UnknownFault))]

        void SetQuestionContent(string path, string urlName, string content);

        /// <summary>
        /// Retrieve the question display content for the specified question.
        /// </summary>
        /// <param name="path">path to NavPage</param>
        /// <param name="urlName">urlName of Question</param>
        /// <returns>the display content for the question</returns>
        /// <exception cref="NotFoundFault">If the item doesn't exist, the question collection is not present, or question cannot be found</exception>
        /// <exception cref="NotANavPageFault">If the item does exist, but it is not a NavPage.</exception>
        /// <exception cref="PathFormatFault">If the path is not valid</exception>
        /// <exception cref="ParameterValidationFault">If the content or urlName is null</exception>
        /// <exception cref="AlreadyExistsFault">If the urlName is already being used in this collection</exception>

        [OperationContract]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(NotANavPageFault))]
        [FaultContract(typeof(PathFormatFault))]
        [FaultContract(typeof(AlreadyExistsFault))]
        [FaultContract(typeof(ParameterValidationFault))]
        [FaultContract(typeof(UnknownFault))]

        string GetQuestionContent(string path, string urlName);

        /// <summary>
        /// Assign a category to the given question.
        /// </summary>
        /// <param name="path">page to NavPage for the QuestionCollection</param>
        /// <param name="qName">urlName of question</param>
        /// <param name="categoryName">category name</param>
        /// <exception cref="NotFoundFault">If the NavPage doesn't exist, the question collection is not present, question or category cannot be found</exception>
        /// <exception cref="NotANavPageFault">If the item does exist, but it is not a NavPage.</exception>
        /// <exception cref="PathFormatFault">If the path is not valid</exception>
        /// <exception cref="ParameterValidationFault">If the question name or category name is null/empty</exception>

        [OperationContract]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(NotANavPageFault))]
        [FaultContract(typeof(PathFormatFault))]
        [FaultContract(typeof(AlreadyExistsFault))]
        [FaultContract(typeof(ParameterValidationFault))]
        [FaultContract(typeof(UnknownFault))]

        void AddQuestionCategory(string path, string qName, string categoryName);


        /// <summary>
        /// Return a list of Questions from the Question  Collection associated with the 
        /// specified NavPage. If categoryName is non-null, only questions with that 
        /// category assigned will be returned.
        /// </summary>
        /// <param name="path">path to NavPage</param>
        /// <param name="categoryName">category to filter by (if non-null)</param>
        /// <returns>list of Question DTOs</returns>
        /// <exception cref="NotFoundFault">If the NavPage doesn't exist, the question collection is not present, question or category cannot be found</exception>
        /// <exception cref="NotANavPageFault">If the item does exist, but it is not a NavPage.</exception>
        /// <exception cref="PathFormatFault">If the path is not valid</exception>
        /// <exception cref="ParameterValidationFault">if the parameters are not valid</exception>

        ////[OperationContract]
        ////[FaultContract(typeof(NotFoundFault))]
        ////[FaultContract(typeof(NotANavPageFault))]
        ////[FaultContract(typeof(PathFormatFault))]
        ////[FaultContract(typeof(AlreadyExistsFault))]
        ////[FaultContract(typeof(ParameterValidationFault))]
        ////[FaultContract(typeof(UnknownFault))]

        ////List<Question> GetQuestionsByCategory(string path, string categoryName);

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

        [OperationContract]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(NotANavPageFault))]
        [FaultContract(typeof(PathFormatFault))]
        [FaultContract(typeof(ItemInUseFault))]
        [FaultContract(typeof(WrongLinkTypeFault))]
        [FaultContract(typeof(UnknownFault))]

        void DeleteQuestionCategory(string path, Guid categoryId);

        /// <summary>
        /// Return all question categories defined for the Question Collection on the
        /// specified Navpage. 
        /// <para>
        /// Available detail levels: 
        /// - "UsageCount" ... includes usage counts
        /// </para>
        /// </summary>
        /// <param name="path">path to NavPage</param>
        /// <param name="detailLevel">amount of detail in DTO</param>
        /// <returns>list of categories</returns>
        /// <exception cref="NotFoundFault">if there is not item with the given path</exception>
        /// <exception cref="PathFormatFault">if the path is not valid</exception>
        /// <exception cref="NotANavPageFault">if the path does not specify a NavPage</exception>
        /// <exception cref="WrongLinkTypeFault">if a link on the path has a target of the wrong type</exception>

        ////[OperationContract]
        ////[FaultContract(typeof(NotFoundFault))]
        ////[FaultContract(typeof(NotANavPageFault))]
        ////[FaultContract(typeof(PathFormatFault))]
        ////[FaultContract(typeof(WrongLinkTypeFault))]
        ////[FaultContract(typeof(UnknownFault))]

        ////List<DTO.QuestionCategory> GetQuestionCategoriesByPath(string path, string detailLevel);

        /// <summary>
        /// Create a new answer for a given question.
        /// </summary>
        /// <param name="path">path to NavPage (location of QuestionCollection)</param>
        /// <param name="qName">question name</param>
        /// <param name="position">position of the answer</param>
        /// <param name="content">answer content</param>
        /// <exception cref="NotFoundFault">If the NavPage doesn't exist, the question collection is not present, question or category cannot be found</exception>
        /// <exception cref="NotANavPageFault">If the item does exist, but it is not a NavPage.</exception>
        /// <exception cref="PathFormatFault">If the path is not valid</exception>
        /// <exception cref="AlreadyExistsFault">If the position number is already in use</exception>
        /// <exception cref="DirectoryCreationFault">If a directory can be created for holding the content</exception>
        /// <exception cref="FileWriteFault">If the file for the content cannot be written</exception>

        [OperationContract]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(NotANavPageFault))]
        [FaultContract(typeof(PathFormatFault))]
        [FaultContract(typeof(AlreadyExistsFault))]
        [FaultContract(typeof(DirectoryCreationFault))]
        [FaultContract(typeof(FileWriteFault))]
        [FaultContract(typeof(UnknownFault))]

        void CreateAnswer(string path, string qName, int position, string content);

        /// <summary>
        /// Remove an answer (choice) from a question.
        /// </summary>
        /// <param name="path">path to NavPage (hosting the Question Collection)</param>
        /// <param name="qName">name of question</param>
        /// <param name="position">answer position to delete</param>
        /// <exception cref="NotFoundFault">If the NavPage doesn't exist, the question collection is not present, question or category cannot be found</exception>
        /// <exception cref="NotANavPageFault">If the item does exist, but it is not a NavPage.</exception>
        /// <exception cref="PathFormatFault">If the path is not valid</exception>

        [OperationContract]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(NotANavPageFault))]
        [FaultContract(typeof(PathFormatFault))]
        [FaultContract(typeof(UnknownFault))]

        void DeleteAnswer(string path, string qName, int position);

        /// <summary>
        /// Set the content of the specified answer to the given string.
        /// </summary>
        /// <param name="path">path to NavPage which hosts the Question Collection</param>
        /// <param name="qName">name of the question</param>
        /// <param name="position">position of the answer</param>
        /// <param name="content">content to set</param>
        /// <exception cref="NotFoundFault">If the NavPage doesn't exist, the question collection is not present, question or category cannot be found</exception>
        /// <exception cref="NotANavPageFault">If the item does exist, but it is not a NavPage.</exception>
        /// <exception cref="PathFormatFault">If the path is not valid</exception>
        /// <exception cref="DirectoryCreationFault">If a directory can be created for holding the content</exception>
        /// <exception cref="FileWriteFault">If the file for the content cannot be written</exception>

        [OperationContract]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(NotANavPageFault))]
        [FaultContract(typeof(PathFormatFault))]
        [FaultContract(typeof(DirectoryCreationFault))]
        [FaultContract(typeof(FileWriteFault))]
        [FaultContract(typeof(UnknownFault))]

        void SetAnswerContent(string path, string qName, int position, string content);

        /// <summary>
        /// Creates a test exam. Will create a test exam using Categories and Question from the same nav page. 
        /// Deleted once we have UI to create exams!!!!!
        /// </summary>
        /// <remarks>MUST HAVE PREVIOUSLY CALLED CreateTestQuestionsAndCategories ON SAME NAV PAGE</remarks>
        /// <param name="navPagePath"></param>
        /// <param name="examName"></param>
        [OperationContract]

        void CreateTestExamFromTestQC(string navPagePath, string examName);

        /// <summary>
        /// Use to populate with Test Data, Delete once we have UI for this!!!!!
        /// </summary>
        /// <param name="navPagePath">path to nav page</param>
        [OperationContract]

        void CreateTestQuestionsAndCategories(string navPagePath);

        /// <summary>
        /// Use to populate with Test Data, Delete once we have UI for this!!!!!
        /// </summary>
        /// <param name="navPagePath">path to nav page</param>
        /// <param name="numberOfCategories"></param>
        /// <param name="numberOfQuestions"></param>
        /// <param name="numberOfAnswers"></param>
        [OperationContract]

        void CreateTestQuestionsAndCategoriesByNumber(string navPagePath, int numberOfCategories, int numberOfQuestions, int numberOfAnswers);

        ///////// <summary>
        ///////// Will get exam from navPage by name
        ///////// </summary>
        ///////// <param name="path"></param>
        ///////// <param name="examName"></param>
        //////[OperationContract]

        //////Exam FindExamByName(string path, string examName);

        /// <summary>
        /// Used to get File Details of simple content with no security check
        /// </summary>
        /// <exception cref="NotANavPageFault">if the path does not refer to NavPage</exception>
        /// <exception cref="NotFoundFault">if there is no file collection</exception>
        /// <exception cref="NotFoundFault"> if there is no file at the relative path</exception>

        [OperationContract]
        [FaultContract(typeof(NotANavPageFault))]
        [FaultContract(typeof(NotFoundFault))]

        FileDetails GetFileDetailsByPath(string path);

        /// <summary>
        /// Get a stream for reading the contents specific file at a specific position. 
        /// The path must be of the form  /[path to NavPage]/-File/[relative path]. 
        /// The contents of the file are retrieved from the FileCollection attached 
        /// to the specified NavPage, using the indicated relative path.
        /// </summary>
        /// <remarks>
        /// The paths that are passed into this method are derived from incoming URL's and
        /// hence it is anticipated that sometimes these paths won't actually correspond to a
        /// an actual content item (because the user can type in arbitrary URLs to the
        /// browser). For this reason, logging of NotFoundExceptions is suppressed here.
        /// </remarks>
        /// <param name="path">path to file</param>
        /// <param name="position">the position the stream should be at</param>
        /// <returns>contents of the file as a string</returns>

        [OperationContract]
        [FaultContract(typeof(NotFoundFault))]

        Stream GetFileAsStreamAtPositionByPath(string path, long position);


        /// <summary>
        /// Used to get Simple content as a Stream with no security check
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>

        [OperationContract]
        [FaultContract(typeof(NotFoundFault))]
        Stream GetFileAsStreamByPath(string path);


        /// <summary>
        /// Used to get a thumbnail of the content as a Stream with no security check
        /// </summary>
        /// <exception cref="NotANavPageFault">If the item does exist, but it is not a NavPage.</exception>
        /// <exception cref="ContentTypeMismatchFault">If the NavPageLink doesn't point to a NavPage</exception>
        /// <exception cref="NotFoundFault">If there is no file collection at the NavPage</exception>
        /// <exception cref="NotFoundFault">If there is no file collection at the NavPage</exception>

        [OperationContract]
        [FaultContract(typeof(NotANavPageFault))]
        [FaultContract(typeof(ContentTypeMismatchFault))]
        [FaultContract(typeof(NotFoundFault))]

        Stream GetThumbnailByPath(string path);

        /// <summary>
        /// Used to get convert a video to a desired ext with no security check. NOTE: I have listed the
        /// faults that can be thrown by the underlying implementation ... but we should never see them
        /// because this is declared as a one-way.
        /// </summary>
        /// <exception cref="NotANavPageFault">If the item does exist, but it is not a NavPage.</exception>
        /// <exception cref="ContentTypeMismatchFault">If the NavPageLink doesn't point to a NavPage</exception>
        /// <exception cref="NotFoundFault">If there is no file collection at the NavPage</exception>
        /// <exception cref="NotFoundFault">If there is no file collection at the NavPage</exception>
        /// <exception cref="StillConvertingFault">if the conversion process is still occurring</exception>

        [OperationContract(IsOneWay = true)]

        void ConvertVideoByPath(string path, string ext);

        /// <summary>
        /// Get the entire contents of a Simple Content item as a string.
        /// </summary>
        /// <param name="path">full path for Simple Content item</param>
        /// <exception cref="NotFoundException">thrown if the file cannot be found</exception>
        /// <exception cref="ParameterValidationException">thrown if INavPageLink doesn't have an INavPage as target,
        /// or the file link doesn't have an IFileCollection as target, or if relative path is null, empty, or contains invalid syntax.</exception>
        /// <exception cref="NotAFileException">thrown if relative path is to directory, not file</exception>

        [OperationContract]
        [FaultContract(typeof(UnknownFault))]
        [FaultContract(typeof(ParameterValidationFault))]
        [FaultContract(typeof(NotAFileFault))]
        string GetFileAsStringByPath(string path);

        /// <summary>
        /// Generate a new exam instance from the given exam link.
        /// <para>
        /// NOTE: There are no serialization levels supported currently. An ExamInstance
        /// is always serialized with all of its questions.
        /// </para>
        /// </summary>
        /// <remarks>
        /// The Flush() call following question generation is needed so that StartInstanceExamAudit()
        /// can look the audit up by ID.
        /// </remarks>
        /// <param name="path">path to the exam link</param>
        /// <param name="proctorNonce">optional nonce identifying proctor</param>
        /// <param name="detailLevel">detail level for returned exam</param>
        /// <returns>an ExamInstance DTO</returns>
        /// <exception cref="PathFormatException">if there are syntactive problems with the path</exception>
        /// <exception cref="NotAuthorizedException">thrown if the user or proctor does not have the needed permissions</exception>
        /// <exception cref="NotFoundException">if an item on the path does not exist</exception>
        /// <exception cref="WrongLinkTypeException">if a link on the path points to the wrong type of item</exception>
        /// <exception cref="ContentTypeMismatchException">if the path does not refer to an exam link</exception>
        /// <exception cref="NotACourseException">if the parent of the path does not specify a NavPageLink</exception>
        /// <exception cref="AutomaticRetakeExamDeferredFault">if the user can't automatically retake the exam because they have to wait longer to be able to wait</exception>
        /// <exception cref="UowIsolationException">If retries failed to update the <c>code</c>/deadlock</exception>

        ////////[OperationContract]
        ////////[FaultContract(typeof(NotFoundFault))]
        ////////[FaultContract(typeof(PathFormatFault))]
        ////////[FaultContract(typeof(NotACourseFault))]
        ////////[FaultContract(typeof(WrongLinkTypeFault))]
        ////////[FaultContract(typeof(NotAuthorizedFault))]
        ////////[FaultContract(typeof(ConfigurationFault))]
        ////////[FaultContract(typeof(UnknownFault))]
        ////////[FaultContract(typeof(AlreadyTakenExamFault))]
        ////////[FaultContract(typeof(ExamHasMultipleInstanceAuditsFault))]
        ////////[FaultContract(typeof(AutomaticRetakeExamDeferredFault))]
        ////////[FaultContract(typeof(UowIsolationFault))]

        ////////ExamInstance GenerateExamInstanceByPath(string path, Guid? proctorNonce, string detailLevel);

        /// <summary>
        /// Return questions, optionally filtered by category. This is a paged interface. If the 
        /// category name provided is null, no category filtering is done.
        /// <para>
        /// Detail Levels: This call does not currently support different detail levels. Questions
        /// are always assembled with all answers, question content, and answer content.
        /// </para>
        /// </summary>
        /// <param name="path">path to NavPage on which the Question Collection exists</param>
        /// <param name="categoryName">name of category to filter on (or null for no filtering)</param>
        /// <param name="pageInfo">paging information</param>
        /// <returns>set of questions (paged)</returns>
        /// <exception cref="NotFoundFault">if there is not item with the given path</exception>
        /// <exception cref="PathFormatFault">if the path is not valid</exception>
        /// <exception cref="NotANavPageFault">if the path does not specify a NavPage</exception>
        /// <exception cref="ConfigurationFault">Some error with the DB.</exception>
        /// <exception cref="UnknownFault">For any other error.</exception>

        //////////[OperationContract]
        //////////[FaultContract(typeof(NotFoundFault))]
        //////////[FaultContract(typeof(PathFormatFault))]
        //////////[FaultContract(typeof(NotANavPageFault))]
        //////////[FaultContract(typeof(ConfigurationFault))]
        //////////[FaultContract(typeof(UnknownFault))]

        //////////CollectionSubset<Question> GetQuestionItemsByCategory(string path, string categoryName, PageInfo pageInfo);

        ///////////// <summary>
        ///////////// Update exam instance
        ///////////// </summary>
        ///////////// <param name="navPagePath">path to nav page exam is on</param>
        ///////////// <param name="exam">updated exam</param>
        //////////[OperationContract]

        //////////void UpdateExam(string navPagePath, Exam exam);


        /// <summary>
        /// Remove exam
        /// </summary>
        /// <param name="navPagePath">path to nav page exam is on</param>
        /// <param name="sharedNavPageId">Optional Id of the share NavPage if the exam to remove is on a shared collection.</param>
        /// <param name="examId">Guid Id of exam</param>
        [OperationContract]

        void RemoveExam(string navPagePath, Guid? sharedNavPageId, Guid examId);

        /// <summary>
        /// Create an ExamCollection on the NavPage specified by the given path.
        /// </summary>
        /// <param name="path">path to NavPage on which collection should be created</param>
        /// <param name="name">name for the new collection</param>
        /// <exception cref="ParameterValidationFault">if the name for the repository name is invalid</exception>
        /// <exception cref="NotFoundFault">if there is not item with the given path</exception>
        /// <exception cref="PathFormatFault">if the path is not valid</exception>
        /// <exception cref="NotANavPageFault">if the path does not specify a NavPage</exception>
        /// <exception cref="ConfigurationFault">Some error with the DB.</exception>
        /// <exception cref="UnknownFault">For any other error.</exception>

        [OperationContract]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(PathFormatFault))]
        [FaultContract(typeof(NotANavPageFault))]
        [FaultContract(typeof(ParameterValidationFault))]
        [FaultContract(typeof(ConfigurationFault))]
        [FaultContract(typeof(UnknownFault))]

        void CreateExamCollection(string path, string name);

        /// <summary>
        /// Deletes the exam collection associated with the NavPage specified by the <paramref name="path"/>.
        /// </summary>
        /// <param name="path">path to current NavPage</param>
        /// <exception cref="NotFoundFault">If the NavPage or ExamCollection cannot be found</exception>
        /// <exception cref="PathFormatFault">If the path is not valid</exception>
        /// <exception cref="NotANavPageFault">If the item does exist, but it is not a NavPage.</exception>
        /// <exception cref="ConfigurationFault">Some error with the DB.</exception>
        /// <exception cref="WrongLinkTypeFault">if a link on the path points to the wrong type of item</exception>
        /// <exception cref="ContentTypeMismatchFault">If the NavPageLink doesn't point to a NavPage</exception>

        [OperationContract]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(PathFormatFault))]
        [FaultContract(typeof(NotANavPageFault))]
        [FaultContract(typeof(ConfigurationFault))]
        [FaultContract(typeof(WrongLinkTypeFault))]
        [FaultContract(typeof(ContentTypeMismatchFault))]
        [FaultContract(typeof(UnknownFault))]

        void DeleteExamCollection(string path);

        /// <summary>
        /// Get the NavPage tag types along the given path.
        /// </summary>
        /// <param name="path">path to NavPage from where to start getting the tag types</param>
        /// <returns>Return the tag types for all pages along the <paramref name="path"/>.</returns>
        /// <exception cref="NotFoundFault">if there is not item with the given path</exception>
        /// <exception cref="PathFormatFault">if the path is not valid</exception>
        /// <exception cref="NotANavPageFault">if the path does not specify a NavPage</exception>
        /// <exception cref="ConfigurationFault">Some error with the DB.</exception>
        /// <exception cref="UnknownFault">For any other error.</exception>

        [OperationContract]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(PathFormatFault))]
        [FaultContract(typeof(NotANavPageFault))]
        [FaultContract(typeof(ParameterValidationFault))]
        [FaultContract(typeof(ConfigurationFault))]
        [FaultContract(typeof(UnknownFault))]

        List<Tuple<string, NavPageTagTypesEnum>> GetNavPageTagTypeByPath(string path);

        /// <summary>
        /// Get all paths and tags which are part of an organization.
        /// </summary>
        /// <param name="path">Path within an organization or at the root path of the organization.</param>
        /// <returns>Return the tag types for all pages within an organization.</returns>
        /// <exception cref="NotFoundException">if there is no item with the given path</exception>
        /// <exception cref="PathFormatException">if the path is not valid</exception>
        /// <exception cref="ConfigurationException">If no organization for this path or configuration for the organization is missing.</exception>
        /// <exception cref="ConfigurationException">Some error with the DB.</exception>
        /// <exception cref="Exception">For any other error.</exception>

        [OperationContract]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(PathFormatFault))]
        [FaultContract(typeof(ConfigurationFault))]
        [FaultContract(typeof(ConfigurationParameterFault))]
        [FaultContract(typeof(UnknownFault))]

        List<Tuple<string, NavPageTagTypesEnum>> GetAllTagsForOrganizationByPath(string path);

        /// <summary>
        /// Set the NavPage tags for the given path if possible (see remarks).
        /// <remarks>
        /// If <paramref name="newTags"/> is Organization: 
        /// There must not be another Organization tag on any path up or down from <paramref name="path"/>.
        /// If <paramref name="newTags"/> is Course: 
        /// There must be an Organization further up the path and no Course further down from <paramref name="path"/>.
        /// If <paramref name="newTags"/> is None: 
        /// If the current tag is a Course - go ahead. If the current tag is an Organization - no Course must be further up or down from <paramref name="path"/>.
        /// Note: This doesn't change any group definitions for any link!
        /// </remarks>
        /// </summary>
        /// <param name="path">path to NavPage to set the tags for.</param>
        /// <param name="newTags">Tags to set.</param>
        /// <exception cref="NotFoundFault">if there is not item with the given path</exception>
        /// <exception cref="PathFormatFault">if the path is not valid</exception>
        /// <exception cref="NotANavPageFault">if the path does not specify a NavPage</exception>
        /// <exception cref="ConfigurationFault">Some error with the DB.</exception>
        /// <exception cref="ConfigurationFault">If the tag change would violate the organization configuration.</exception>
        /// <exception cref="UnknownFault">For any other error.</exception>

        [OperationContract]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(PathFormatFault))]
        [FaultContract(typeof(NotANavPageFault))]
        [FaultContract(typeof(ConfigurationFault))]
        [FaultContract(typeof(UnknownFault))]

        void SetNavPageTagsByPath(string path, NavPageTagTypesEnum newTags);

        /// <summary>
        /// Return information about how a given permission result is computed. The intent is that this
        /// call carries out the same logic as CheckCapability(), but returns information on how the
        /// permissions result was arrived at.
        /// </summary>
        /// <param name="path">path to check</param>
        /// <param name="capability">capability to check for</param>
        /// <param name="userId">ID of user to carry out trace for</param>
        /// <returns></returns>

        [OperationContract]
        [FaultContract(typeof(UnknownFault))]
        [FaultContract(typeof(ConfigurationFault))]

        PermissionTrace TracePermission(string path, string capability, Guid userId);

        /// <summary>
        /// Determine if the basic system configuration has been loaded yet. We do this by checking
        /// to see if the root ('/Root') of the content tree exists.
        /// </summary>
        /// <returns>Return <c>true</c> if the NavPage '/Root' exists, otherwise return <c>false</c>.</returns>

        [OperationContract]

        bool IsSystemConfigured();

        /// <summary>
        /// Endpoint verification function for installer. 
        /// </summary>
        /// <returns>true</returns>

        [OperationContract]

        bool IsContentServiceAdmin();

        /// <summary>
        /// Will return question with ShortId
        /// </summary>
        ///////// <param name="shortId">ShortId of question to get</param>
        ///////// <param name="detailLevel"></param>
        ///////// <returns>Question</returns>
        ///////// <exception cref="NotFoundException">if there is no item with the given path</exception>
        //////[OperationContract]
        //////[FaultContract(typeof(NotFoundFault))]

        //////Question GetQuestionByShortId(int shortId, string detailLevel);

        /// <summary>
        /// Get all certificate templates belonging to a given organization (looking up along the <paramref name="path"/>).
        /// </summary>
        /// <param name="path">the path</param>
        /// <param name="detailLevel"></param>
        /// <returns>list of certificate templates</returns>
        /// <exception cref="OrganizationNotFoundFault">If no organization has been found along the path</exception>
        /// <exception cref="PathFormatFault">path is not syntactically correct</exception>
        /// <exception cref="NotFoundFault">item does not exist at path</exception>
        /// <exception cref="WrongLinkTypeFault">element on path of wrong type</exception>
        ////////[OperationContract]
        ////////[FaultContract(typeof(OrganizationNotFoundFault))]
        ////////[FaultContract(typeof(NotFoundFault))]
        ////////[FaultContract(typeof(PathFormatFault))]
        ////////[FaultContract(typeof(WrongLinkTypeFault))]
        ////////[FaultContract(typeof(ConfigurationFault))]
        ////////[FaultContract(typeof(UnknownFault))]

        ////////List<Certificate> GetCertificatesByPath(string path, string detailLevel);

        /////////// <summary>
        /////////// Creates the or update certificate.
        /////////// </summary>
        /////////// <param name="cert">The cert.</param>

        ////////[OperationContract]
        ////////[FaultContract(typeof(NotFoundFault))]
        ////////[FaultContract(typeof(UnknownFault))]

        ////////void CreateOrUpdateCertificate(Certificate cert);

        /////////// <summary>
        /////////// Gets the certificate by identifier.
        /////////// </summary>
        /////////// <param name="certId"></param>
        /////////// <param name="detailLevel">The detail level.</param>
        /////////// <returns></returns>
        /////////// <exception cref="NotFoundException">GetCertificateById: Certificate not found</exception>

        ////////[OperationContract]
        ////////[FaultContract(typeof(NotFoundFault))]
        ////////[FaultContract(typeof(UnknownFault))]

        ////////Certificate GetCertificateById(Guid certId, string detailLevel);

        ///////// <summary>
        ///////// Gets the certificate by shortId, coursePath, offeringshortId and userId
        ///////// </summary>
        ///////// <param name="certificateShortId"></param>
        ///////// <param name="loginId"></param>
        ///////// <param name="coursePath"></param>
        ///////// <param name="offeringShortId"></param>
        ///////// <param name="detailLevel"></param>
        ///////// <returns></returns>
        //////[OperationContract]
        //////[FaultContract(typeof(NotFoundFault))]
        //////[FaultContract(typeof(UnknownFault))]

        //////List<CertificateAudit> GetCertificatesByShortId(string certificateShortId, string loginId, string coursePath, string offeringShortId, string detailLevel);

        ///////// <summary>
        ///////// Return all certificate templates for an organization. 
        ///////// </summary>
        ///////// <param name="orgPath">Organization path to get the certificate template from (look along the path for an organization if necessary)</param>
        ///////// <param name="detailLevel">Detail level to include in the result</param>
        ///////// <returns>Return list of certificate templates. Return empty list if not certificate templates exist at <paramref name="orgPath"/>.</returns>
        ///////// <exception cref="OrganizationNotFoundFault">If the <paramref name="orgPath"/> has no organization along the path.</exception>
        ///////// <exception cref="UnknownFault">On any other error</exception>

        //////[OperationContract]
        //////[FaultContract(typeof(OrganizationNotFoundFault))]
        //////[FaultContract(typeof(UnknownFault))]

        //////List<DTO.Certificate> GetAllCertificatesByPath(string orgPath, string detailLevel);

        /// <summary>
        /// Deletes the certificate.
        /// </summary>
        /// <param name="certId"></param>
        /// <exception cref="NotFoundFault">DeleteCertificate: Certificate not found</exception>

        [OperationContract]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(UnknownFault))]

        void DeleteCertificate(Guid certId);

        /////////// <summary>
        /////////// Gets the certificate from course path.
        /////////// </summary>
        /////////// <param name="path">The path.</param>
        /////////// <returns></returns>

        ////////[OperationContract]
        ////////[FaultContract(typeof(NotFoundFault))]
        ////////[FaultContract(typeof(UnknownFault))]

        ////////Certificate GetCertificateFromCoursePath(string path);


        /// <summary>
        /// Will find all orphaned links.  Will delete all orphan links if <paramref name="deleteLinks"/> is true.
        /// </summary>
        /// <param name="deleteLinks"></param>
        [OperationContract]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(UnknownFault))]

        List<string> FindAndRemoveOrphanLinks(bool deleteLinks);

        /// <summary>
        /// Adds an org group rule for an org at the <paramref name="orgPath"/> for a group at the <paramref name="linkId"/> with
        /// <paramref name="groupName"/>, defined by the <paramref name="expression."/>
        /// An org group rule attribute has the format:
        /// Group(linkId,groupName):predicate(args);predicate(args)
        /// 
        /// Currently available predicates:
        /// ProfileEqual(profileName,profileValue) where profileName is the string form of an org profile field for the org at org path, and profileValue is a string value of the profile field that will match. e.g. ProfileEqual(MyProfileName1,MyProfileValue1)
        /// ProfileMatch(profileName,regExp) as with ProfileEqual, but regExp is a .Net style regular expression string that will match. e.g. ProfileMatch(MyProfileName1,BRM-[0-5]$)
        /// </summary>
        /// <exception cref="RuleFormatFault">If the <paramref name="expression"/> has the wrong format.</exception>
        /// <exception cref="NotFoundFault">If the <paramref name="orgPath"/> doesn't exist or the <paramref name="linkId"/> doesn't exist. Or the <paramref name="groupName"/> doesn't exist at the <paramref name="linkId"/>.</exception>
        /// <exception cref="UnknownFault">On any other error</exception>

        [OperationContract]
        [FaultContract(typeof(RuleFormatFault))]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(UnknownFault))]

        void AddOrgGroupRule(string orgPath, Guid linkId, string groupName, string expression);

        /// <summary>
        /// Delete a org group rule at <paramref name="orgPath"/>.
        /// </summary>
        /// <remarks>
        /// The rules are returned ordered in <see cref="GetOrgGroupRules"/>.
        /// The position in the returned list can be used as <paramref name="position"/> to delete a rule.
        /// <b>Attention</b>: After removing one rule the position of the other rules might change. Use <see cref="GetOrgGroupRules"/>
        /// again to check which rule should be deleted next.
        /// </remarks>
        /// <exception cref="NotFoundFault">If the <paramref name="orgPath"/> doesn't exist or the <paramref name="position"/> doesn't exist</exception>
        /// <exception cref="UnknownFault">On any other error</exception>

        [OperationContract]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(UnknownFault))]

        void DeleteOrgGroupRule(string orgPath, int position);

        /// <summary>
        /// Return all org group rule attributes at the <paramref name="orgPath"/>. 
        /// </summary>
        /// <remarks>
        /// The rules are ordered by Id. The position of the rules can be used to delete a certificate in <see cref="DeleteOrgGroupRule"/>.
        /// The position is zero-based.
        /// </remarks>
        /// <exception cref="NotFoundFault">If the <paramref name="orgPath"/> doesn't exist</exception>
        /// <exception cref="UnknownFault">On any other error</exception>

        [OperationContract]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(UnknownFault))]

        IList<string> GetOrgGroupRules(string orgPath);

        /// <summary>
        /// Evaluates all the org group rules at the <paramref name="orgPath"/>.
        /// These rules will add/remove all org users from a group at a link in the org, if rule predicates are all true, or all false.
        /// </summary>

        [OperationContract]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(UnknownFault))]

        void ReEvaluateOrgGroupRules(string orgPath);

        /// <summary>
        /// For printing purposes only - helps testing dynamic set offering role rules - this generates all permutation based on the
        /// variables the dynamic path includes. 
        /// If no variables are used in the <paramref name="path"/> the permutation IS the <paramref name="path"/>.
        /// </summary>
        /// <param name="orgPath">organizations' path</param>
        /// <param name="type">rule type</param>
        /// <param name="path">dynamic or static course path</param>
        /// <param name="expression">predicate expressions</param>
        /// <param name="leaveRegistered">if true - we are leaving registered students/Instructors in course</param>
        /// <returns>list of permutations</returns>
        [OperationContract]
        [FaultContract(typeof (RuleFormatFault))]
        [FaultContract(typeof (NotACourseFault))]
        [FaultContract(typeof (NotFoundFault))]
        [FaultContract(typeof (UnknownFault))]
        IList<string> GenerateSetOfferingRoleRulePermutations(string orgPath, string type, string path, string expression, bool leaveRegistered);

        /// <summary>
        /// Adds an org set offering role rule for an org at the <paramref name="orgPath"/> for a course at the <paramref name="path"/>,
        /// for the <paramref name="type"/> defined by the <paramref name="expression."/> Note that the <paramref name="type"/> must be
        /// 'Instructor' or 'Student'.
        /// An org registration rule attribute has the format:
        /// Registration(linkId):predicate(args);predicate(args)
        /// 
        /// Currently available predicates:
        /// ProfileEqual(profileName,profileValue) where profileName is the string form of an org profile field for the org at org path, and profileValue is a string value of the profile field that will match. e.g. ProfileEqual(MyProfileName1,MyProfileValue1)
        /// ProfileMatch(profileName,regExp) as with ProfileEqual, but regExp is a .Net style regular expression string that will match. e.g. ProfileMatch(MyProfileName1,BRM-[0-5]$)
        /// </summary>
        /// <exception cref="RuleFormatFault">If the <paramref name="expression"/> has the wrong format.</exception>
        /// <exception cref="NotACourseFault">If the <paramref name="path"/> is not a course link.</exception>
        /// <exception cref="NotFoundFault">If the <paramref name="orgPath"/> doesn't exist or the <paramref name="path"/> doesn't exist. Or there is no course at the <paramref name="path"/>.</exception>
        /// <exception cref="UnknownFault">On any other error</exception>

        [OperationContract]
        [FaultContract(typeof(RuleFormatFault))]
        [FaultContract(typeof(NotACourseFault))]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(UnknownFault))]

        void AddOrgSetOfferingRoleRule(string orgPath, string type, string path, string expression, bool leaveRegistered);

        /// <summary>
        /// Replaces the linkId in existing SetOfferingRoleRules to the corresponding paths, used for rules that can't make full use of the new format (dynamic paths and variables).
        /// </summary>
        /// <exception cref="RuleFormatFault">If the rule has the wrong format.</exception>
        /// <exception cref="NotACourseFault">If the path is not a course link.</exception>
        /// <exception cref="NotFoundFault">If the <paramref name="orgPath"/> doesn't exist or the path doesn't exist.</exception>

        /// <exception cref="UnknownFault">On any other error</exception>

        [OperationContract]
        [FaultContract(typeof(RuleFormatFault))]
        [FaultContract(typeof(NotACourseFault))]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(UnknownFault))]


        IList<string> ChangeLinkIdToPathInSetOfferingRoleRule(string orgPath);

        /// <summary>
        /// Delete a org set offering role rule at <paramref name="orgPath"/>.
        /// </summary>
        /// <remarks>
        /// The rules are returned ordered in <see cref="GetOrgSetOfferingRoleRules"/>.
        /// The position in the returned list can be used as <paramref name="position"/> to delete a rule.
        /// <b>Attention</b>: After removing one rule the position of the other rules might change. Use <see cref="GetOrgSetOfferingRoleRules"/>
        /// again to check which rule should be deleted next.
        /// </remarks>
        /// <exception cref="NotFoundFault">If the <paramref name="orgPath"/> doesn't exist or the <paramref name="position"/> doesn't exist</exception>
        /// <exception cref="UnknownFault">On any other error</exception>

        [OperationContract]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(UnknownFault))]

        void DeleteOrgSetOfferingRoleRule(string orgPath, int position);

        /// <summary>
        /// Bulk delete SetOfferingRoleRules of type <paramref name="type"/> and if <paramref name="byLinkId"/> is true, only those that include linkIds,
        /// if false only those that include string paths will be deleted.
        /// </summary>
        /// <param name="orgPath">path for organization</param>
        /// <param name="byLinkId">if bool is true, delete rules that include linkIds, otherwise delete rules with that include paths</param>
        /// <param name="type">type of rule to bulk delete</param>

        [OperationContract]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(UnknownFault))]
        List<string> DeleteOrgSetOfferingRoleRules(string orgPath, bool byLinkId, string type);

        /// <summary>
        /// Return all org set offering role rule attributes at the <paramref name="orgPath"/>. 
        /// </summary>
        /// <remarks>
        /// The rules are ordered by Id. The position of the rules can be used to delete a rule in <see cref="DeleteOrgSetOfferingRoleRule"/>.
        /// The position is zero-based.
        /// </remarks>
        /// <exception cref="NotFoundFault">If the <paramref name="orgPath"/> doesn't exist</exception>
        /// <exception cref="UnknownFault">On any other error</exception>

        [OperationContract]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(UnknownFault))]

        IList<string> GetOrgSetOfferingRoleRules(string orgPath);

        /// <summary>
        /// Evaluates all the org set offering role rules at the <paramref name="orgPath"/>.
        /// There are 2 current variations of this rule: one that (un)registers users as students and one that adds/removes instructors.
        /// These rules will add/remove a user registration or add/remove an instructor from offerings at a course link in the org, if rule predicates are all true, or all false.
        /// 
        /// It will register a user in an "eternal" offering (one with null start/end dates, if one exists) if they do not have a non-deleted registration in it. Or it will
        /// add an them as instructor to this offering if they are not one.
        /// If there is no such offering it will register the user in the offering with the largest start date (if one exists) if they do not have a non-deleted registration in it.
        /// Or it will add them as an instructor to this offering if they are not one.
        /// 
        /// Finally, it will delete any of the user's active/inactive registrations in all offerings for the course. Or it will remove the user as an instructor for in all offerings
        /// for the course. 
        /// </summary>

        [OperationContract]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(UnknownFault))]

        void ReEvaluateOrgSetOfferingRoleRules(string orgPath);


        //TODO: Task #3267
        /// <summary>
        /// This will create a new organization with generically filled in content and data for testing purposes only.
        /// </summary>
        /// <param name="parentPath"></param>
        /// <param name="orgName"></param>
        /// <param name="orgDepth"></param>
        /// <param name="branchingFactor"></param>
        /// <param name="terminalTOCLinksCount"></param>
        /// <param name="smallFilePath"></param>
        /// <param name="largeFilePath"></param>
        /// <param name="smallFileCount"></param>
        /// <param name="largeFileCount"></param>
        /// <param name="orgUserCount"></param>
        /// <param name="registrationsPerCourse"></param>
        /// <param name="examsPerCourse"></param>
        /// <param name="questionsPerExam"></param>
        /// <param name="totalQuestions"></param>
        /// <param name="completedExamsPerCourse"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(UnknownFault))]

        string SetupTestOrganization(
            string parentPath,
            string orgName,
            int orgDepth,
            int branchingFactor,
            int terminalTOCLinksCount,
            string smallFilePath,
            string largeFilePath,
            int smallFileCount,
            int largeFileCount,
            int orgUserCount,
            int registrationsPerCourse,
            int examsPerCourse,
            int questionsPerExam,
            int totalQuestions,
            int completedExamsPerCourse
            );


        /// <summary>
        /// This will add fake users to the organization base on orgUserCount parameter.
        /// </summary>
        /// <param name="orgName"></param>
        /// <param name="orgPath"></param>
        /// <param name="orgUserCount"></param>
        /// <returns></returns>

        [OperationContract]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(UnknownFault))]

        string AddFakeUsersToOrganization(string orgName, string orgPath, int orgUserCount);

        /// <summary>
        /// This will add some fake questions to question collection of an oranization
        /// </summary>
        /// <param name="orgPath"></param>
        /// <param name="totalQuestions"></param>
        /// <returns></returns>

        [OperationContract]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(UnknownFault))]

        string GenerateQuestions(string orgPath, int totalQuestions);

        /// <summary>
        /// Take test exams for fake organization
        /// </summary>
        /// <param name="orgPath"></param>
        /// <param name="completedExamsPerCourse"></param>
        /// <returns></returns>

        [OperationContract]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(UnknownFault))]

        string TakeExamsForFakeOrganization(string orgPath, int completedExamsPerCourse);

        /// <summary>
        /// Generate registrations for a fake organization
        /// </summary>
        /// <param name="orgPath"></param>
        /// <param name="courseName"></param>
        /// <param name="registrationsPerCourse"></param>
        /// <returns></returns>

        [OperationContract]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(UnknownFault))]
        [FaultContract(typeof(AlreadyExistsFault))]

        string GenerateRegistrationForFakeOrganization(string orgPath, string courseName, int registrationsPerCourse);


        /// <summary>
        /// This will add some small and large files to the file collection of an oranization
        /// </summary>
        /// <param name="orgPath"></param>
        /// <param name="smallFileCount"></param>
        /// <param name="smallFilePath"></param>
        /// <param name="largeFileCount"></param>
        /// <param name="largeFilePath"></param>
        /// <returns></returns>

        [OperationContract]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(UnknownFault))]

        string AddTestFileCollection(string orgPath, int smallFileCount, string smallFilePath, int largeFileCount, string largeFilePath);

        /// <summary>
        /// Gets the latest audit Id that is older than <paramref name="minOutOfDate"/>.
        /// </summary>
        /// <param name="minOutOfDate">Age in minutes that audit must be older than.</param>
        /// <returns>Guid Id of latest audit</returns>

        [OperationContract]
        [FaultContract(typeof(UnknownFault))]

        Guid GetLatestAuditId(int minOutOfDate);

        /// <summary>
        /// Get latest file in the file collection that is older than <paramref name="hoursOutOfDate"/>
        /// </summary>
        /// <param name="hoursOutOfDate">Age in hours that file must be older than</param>
        /// <returns>Relative path of latest file</returns>

        [OperationContract]
        [FaultContract(typeof(UnknownFault))]

        string GetLatestFileInCollections(int hoursOutOfDate);

        #region Tag

        /// <summary>
        /// Get TagSet for a NavPage
        /// </summary>
        /// <param name="path">string path to NavPage</param>
        /// <param name="tagSetNameOrId">optional TagSet Name or Id to filter</param>
        /// <returns>TagSet</returns>

        [OperationContract]
        List<TagSet> GetTagSets(string path, string tagSetNameOrId = null);

        /// <summary>
        /// Add TagSet
        /// </summary>
        /// <param name="path">string path to NavPage</param>
        /// <param name="name"></param>
        /// <param name="displayName">string Display Name</param>
        /// <param name="lifetime">TagLifeTimes enumeration</param>
        /// <returns>Guid id of new TagSet</returns>

        [OperationContract]
        Guid? AddTagSet(string path, string name, string displayName, TagLifeTimes lifetime);

        /// <summary>
        /// Add Tag
        /// </summary>
        /// <param name="path">string path to NavPage</param>
        /// <param name="name"></param>
        /// <param name="displayName">string Display Name</param>
        /// <param name="tagSetId">id of TagSet Owner</param>
        /// <returns>Guid id of new Tag</returns>

        [OperationContract]
        Guid? AddTag(string path, string name, string displayName, Guid tagSetId);

        /// <summary>
        /// Create a new mapping at the given path
        /// </summary>
        /// <param name="path"></param>
        /// <param name="sourceTagName"></param>
        /// <param name="targetTagName"></param>
        /// <param name="name"></param>
        /// <param name="displayName"></param>

        [OperationContract]
        Guid? AddTagMapping(string path, string sourceTagName, string targetTagName, string name, string displayName);

        /// <summary>
        /// Set a mapping between source tag and target tag
        /// </summary>
        /// <param name="path"></param>
        /// <param name="sourceTagName"></param>
        /// <param name="targetTagName"></param>
        /// <param name="tagMappingName"></param>

        [OperationContract]
        Guid? SetTagMapping(string path, string sourceTagName, List<string> targetTagName, string tagMappingName);

        /// <summary>
        /// Get a list of tag mappings for a given path
        /// </summary>
        /// <param name="path"></param>

        [OperationContract]
        List<TagMapping> GetTagMappingsForGivenPath(string path);

        /// <summary>
        /// Update TagSet
        /// </summary>
        /// <param name="path">string path to NavPage</param>
        /// <param name="tagSetDto">TagSet DTO</param>

        [OperationContract]
        void UpdateTagSet(string path, TagSet tagSetDto);

        /// <summary>
        /// Update Tag
        /// </summary>
        /// <param name="path">string path to NavPage</param>
        /// <param name="tagDto">Tag DTO</param>

        [OperationContract]
        void UpdateTag(string path, Tag tagDto);

        /// <summary>
        /// Delete TagSet
        /// </summary>
        /// <param name="path">string path to NavPage</param>
        /// <param name="tagSetDto">TagSet DTO</param>

        void DeleteTagSet(string path, DTO.TagSet tagSetDto);

        /// <summary>
        /// Delete TagSet by Id
        /// </summary>
        /// <param name="path">string path to NavPage</param>
        /// <param name="tagSetId">TagSet Id</param>

        [OperationContract]
        void DeleteTagSetById(string path, Guid tagSetId);

        /// <summary>
        /// Delete Tag
        /// </summary>
        /// <param name="path">string path to NavPage</param>
        /// <param name="tagDto">Tag DTO</param>

        void DeleteTag(string path, DTO.Tag tagDto);

        /// <summary>
        /// Delete Tag by Id
        /// </summary>
        /// <param name="path">string path to NavPage</param>
        /// <param name="tagId">Tag Id</param>

        [OperationContract]
        void DeleteTagById(string path, Guid tagId);

        /// <summary>
        /// Delete tag mapping
        /// </summary>
        /// <param name="path"></param>
        /// <param name="name"></param>

        [OperationContract]
        void DeleteTagMapping(string path, string name);

        #endregion

        /// <summary>
        /// Create course grade book schema
        /// </summary>
        /// <param name="path"></param>
        /// <param name="header"></param>

        [OperationContract]
        [FaultContract(typeof(UnknownFault))]
        [FaultContract(typeof(ParameterValidationException))]

        void CreateGradeBookSchema(string path, string header);

        /// <summary>
        /// Update course grade book schema property
        /// If the schema exists, it will change the header.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="header"></param>

        [OperationContract]
        [FaultContract(typeof(UnknownFault))]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(ParameterValidationException))]

        void UpdateGradeBookSchemaProperty(string path, string header);

        /// <summary>
        /// Delete course grade book schema
        /// </summary>
        /// <param name="path"></param>

        [OperationContract]
        [FaultContract(typeof(UnknownFault))]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(ParameterValidationException))]

        void DeleteGradeBookSchema(string path);

        /// <summary>
        /// Get grade book schema by nav page path
        /// </summary>
        /// <param name="path"></param>

        ////////[OperationContract]
        ////////[FaultContract(typeof(UnknownFault))]
        ////////[FaultContract(typeof(NotFoundFault))]

        ////////DTO.CourseGradeBookSchema GetGradeBookSchema(string path);

        /// <summary>
        /// Add a new course grade book column schema
        /// </summary>
        /// <param name="path"></param>
        /// <param name="displayOrder"></param>
        /// <param name="type"></param>
        /// <param name="shortName"></param>
        /// <param name="header"></param>
        /// <param name="color"></param>

        //////////[OperationContract]
        //////////[FaultContract(typeof(UnknownFault))]

        //////////void AddCourseGradeBookColumnSchema(string path, int displayOrder, CourseGradeBookSchemaTypesEnum type, string shortName, string header, int color);

        /// <summary>
        /// Delete a course grade book column schema based on its order
        /// </summary>
        /// <param name="path"></param>
        /// <param name="order"></param>

        [OperationContract]
        [FaultContract(typeof(UnknownFault))]

        void DeleteGradeBookColumnSchema(string path, int order);

        /// <summary>
        /// Find Course Grade Book Column Schema by it's order
        /// </summary>
        /// <param name="path"></param>
        /// <param name="order"></param>

        ////////[OperationContract]
        ////////[FaultContract(typeof(UnknownFault))]
        ////////[FaultContract(typeof(NotFoundFault))]

        ////////DTO.CourseGradeBookColumnSchema FindCourseGradeBookColumnSchemaByOrder(string path, int order);

        /// <summary>
        /// Update course grade book column schema property by Id
        /// </summary>
        /// <param name="path"></param>
        /// <param name="courseGradeBookColumnSchema"></param>

        ////////[OperationContract]
        ////////[FaultContract(typeof(UnknownFault))]
        ////////[FaultContract(typeof(NotFoundFault))]

        ////////void UpdateGradebookColumnSchemaPropertyById(string path, CourseGradeBookColumnSchema courseGradeBookColumnSchema);

        /// <summary>
        /// Reorder course grade book columns schema
        /// If not all headers in the schema are specified, or if headers not in the schema are specified, this will throw an exception.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="headers"></param>

        [OperationContract]
        [FaultContract(typeof(UnknownFault))]

        void ReorderGradebookColumnSchema(string path, IList<string> headers);
    }
}
