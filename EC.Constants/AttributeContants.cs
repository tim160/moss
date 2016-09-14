
namespace EC.Constants
{
    public static class AttributeConstants
    {
        /// <summary>
        /// 'System.Styles.CSS.IndexPage' is the attribute for storing a path to a CSS file for a nav page.
        /// </summary>

        public const string CSS_FOR_NAV_PAGE = "System.Styles.CSS.NavPage";

        /// <summary>
        /// 'System.Content.NavPage.Variables.' is the base attribute name for auxiliary variables for a link.
        /// </summary>

        public const string CONTENT_VARIABLES_PREFIX = "System.Content.NavPage.Variables.";

        /// <summary>
        /// 'System.GroupDefinition.' is the base attribute name for a group definition.
        /// A group definition looks like 'System.GroupDefinition.Administrator' for the group name 'Administrator'.
        /// </summary>

        public const string GROUP_DEFINITION_BASE_NAME = "System.GroupDefinition.";

        /// <summary>
        /// 'System.Capability.' is the base attribute name for all capabilities.
        /// This could be used to identify a capability attribute.
        /// </summary>

        public const string CAPABILITY_BASE_NAME = "System.Capability.";

        /// <summary>
        /// Attribute prefix for attributes that control the mapping of registration policies to 
        /// custom UI pages displayed when registering a student with that policy. The value
        /// of the attribute is the absolute path to the CSHTML page in the content tree.
        /// </summary>

        public const string REGISTRATION_POLICY_UI_PAGE = "System.Registration.UIPage.";

        /// <summary>
        /// 'System.Content.NavPage' is the attribute for storing a path to the nav page content.
        /// This only check the local link (no walking through the path), if the attribute is not on the local link.
        /// When creating a new page the default path is used (see <see cref="DEFAULT_CONTENT_FOR_NAV_PAGE"/> and <see cref="DEFAULT_CONTENT_FOR_TOC_PAGE"/>).
        /// </summary>

        public const string CONTENT_FOR_NAV_PAGE = "System.Content.NavPage";

        /// <summary>
        /// 'System.Default.Styles.CSS.LinksView' is the attribute for storing the css for Link View of the Page.
        /// Having this attribute and 'System.Default.Styles.CSS.TOCView' let's the user switch a NavPage between TOC and Links.
        /// </summary>

        public const string DEFAULT_CSS_FOR_LINKS_VIEW = "System.Default.Styles.CSS.LinksView";

        /// <summary>
        /// 'System.Default.Styles.CSS.TOCView' is the attribute for storing the css for TOC View of the Page.
        /// Having this attribute and 'System.Default.Styles.CSS.LinksView' let's the user switch a NavPage between TOC and Links.
        /// </summary>

        public const string DEFAULT_CSS_FOR_TOC_VIEW = "System.Default.Styles.CSS.TOCView";

        /// <summary>
        /// 'System.Content.Default.NavPage'. Default content path for a new NavPage.
        /// </summary>

        public const string DEFAULT_CONTENT_FOR_NAV_PAGE = "System.Content.Default.NavPage";

        /// <summary>
        /// 'System.Default.Content.TOCPage'. Default content path for a new TOCPage (Learning Path).
        /// </summary>

        public const string DEFAULT_CONTENT_FOR_TOC_PAGE = "System.Default.Content.TOCPage";

        /// <summary>
        /// User group name for an organization. This attribute is located at the organization level.
        /// </summary>

        public const string CUSTOMER_USERS_GROUP_NAME = "System.Customer.Users.GroupName";

        /// <summary>
        /// Admin group name for an organization. This attribute is located at the organization level.
        /// </summary>

        public const string CUSTOMER_COURSE_MANAGEMENT_ADMINS_GROUP_NAME = "System.Customer.Course.Management.Admins.GroupName";

        /// <summary>
        /// Admin group name for an organization to administrate users. This attribute is located at the organization level.
        /// </summary>

        public const string CUSTOMER_USER_MANAGEMENT_ADMINS_GROUP_NAME = "System.Customer.User.Management.Admins.GroupName";

        /// <summary>
        /// Path to copyright notice html file. This attribute is located at the organization level. 
        /// </summary>

        public const string CUSTOMER_COPYRIGHT_NOTICE = "System.Customer.CopyrightNotice";

        /// <summary>
        /// Feedback email that will be placed that the bottom of the screen. This attribute is located at the organization level. 
        /// </summary>

        public const string CUSTOMER_FEEDBACK_EMAIL = "System.Customer.FeedbackEmail";

        /// <summary>
        /// If a navPage has this attribute set then they will be able to create and update users using the additional orgId to logon.
        /// If you set this attribute to true you should also set the System.Customer.OrganizationName attribute to the Org's Name.
        /// </summary>

        public const string CUSTOMER_IS_ORGID_ENABLED = "System.Customer.IsOrgIdEnabled";

        /// <summary>
        /// This is the attribute to add a org level page to specify the organizations name. This attribute is required when a org is using orgId Log On.
        /// </summary>

        public const string CUSTOMER_ORG_NAME = "System.Customer.OrganizationName";

        /// <summary>
        /// This page attribute is the short (should not be longer than 4-5 characters) organization name.
        /// The short name is used to create file/question collection directory names.
        /// </summary>
        /// <remarks>
        /// If this page attribute is missing on an organization NavPage an automatically shortened version of the OrganizationName attribute is used instead. 
        /// If both page attributes (OrganizationName, OrganizationShortName) are missing the file/question collections are created within the 'NoOrg' directory name.
        /// </remarks>

        public const string CUSTOMER_ORG_SHORT_NAME = "System.Customer.OrganizationShortName";

        /// <summary>
        /// Customer variable including paging size choices, to be shown where paging is used in the UI.
        /// </summary>

        public const string CUSTOMER_PAGE_SIZES = "System.Customer.PageSizes";

        /// <summary>
        /// This attribute allows for the definition of rule variable mapping. This attribute is used to determine all permutations for SetOfferingRoleRules with dynamic paths/predicates.
        /// </summary>
        /// <remarks>
        /// Format of attribute value:
        /// RuleVariable:PathValue1,PredicateValue1;PathValue2,PredicateValue2;....;PathValueN,PredicateValueN
        /// RuleVariable - rule variable name starts and ends with '$'
        /// PathValue,PredicateValue pairs are separated by semicolon and the pair itself by comma.
        /// </remarks>

        public const string CUSTOMER_RULE_VARIABLE_MAPPING = "System.Customer.RuleVariable.Mapping";

        /// <summary>
        /// This attribute allows for the definition of predicate variables that do not trigger any permutations of the predicates they are used in. Instead they are used as a list of 
        /// values within the predicate.
        /// </summary>
        /// <remarks>
        /// Format of attribute value:
        /// PredicateVariable:Value1|Value2|...|ValueN
        /// PredicateVariale - the predicate variable name starts and end with '|'
        /// Values are seperated by '|'.
        /// </remarks>

        public const string CUSTOMER_PREDICATE_VARIABLE_NO_PERMUTATIONS = "System.Customer.PredicateVariable.NoPermutations";

        /// <summary>
        /// specify where the user will be redirected to once they logout
        /// </summary>

        public const string CUSTOMER_LOGOUT_REDIRECT = "System.Customer.LogoutRedirect";

        /// <summary>
        /// Rule for certificate.  Must be an attribute on a course page.
        /// </summary>
        /// 
        public const string CERTIFICATE_RULE = "System.Certificate.Rule";

        /// <summary>
        /// Rule for group membership addition/deletion.  Must be an attribute on an org page.
        /// </summary>

        public const string GROUP_RULE = "System.Group.Rule";

        /// <summary>
        /// Rule for offering registration addition/deletion.  Must be an attribute on an org page.
        /// </summary>

        public const string SET_OFFERING_ROLE_RULE = "System.SetOfferingRole.Rule";

        /// <summary>
        /// Rule for registration completion.  Must be an attribute on a course page.
        /// </summary>

        public const string REGISTRATION_COMPLETION_RULE = "System.RegistrationCompletion.Rule";

        /// <summary>
        /// Specifies encryption password used in generating sync packages.
        /// Normally associated with Organization root nav page (IE One password for an Organization and all RTA's)
        /// </summary>

        public const string SYNC_ENCRYPTION_PASSWORD = "System.Sync.Encryption.Password";
        
        /// <summary>
        /// Specifies the maximum number of characters to display "as-is"; e.g. in Manage|Attributes
        ///   In this case (Edit Link Attributes), if the Value (of, for example, System.GroupDefinition.ITB-Users) is > MAX_DISPLAY_LENGTH chars,
        ///     REPLACE the attribute's Value with the const string TRUNCATED_DISPLAY_TEXT.
        ///   This is done in the model before the value is displayed in the view.
        ///   On Save(), attributes with the TRUNCATED_DISPLAY_TEXT value are 
        ///     REPLACED with their original value THEN saved back to the DB.
        ///   NOTE: Set the value high enough, say 8000-12000, so that attributes other than System.GroupDefinition.{ORG}-Users are not affected.
        /// </summary>

        public const int MAX_DISPLAY_LENGTH = 8000;

        /// <summary>
        /// Specifies the special text to replace the attribute Value which is being modified for display
        ///   This is done in the model before the value is displayed in the view.
        ///   On Save(), attributes with the TRUNCATED_DISPLAY_TEXT value are 
        ///     REPLACED with their original value THEN saved back to the DB.
        /// </summary>

        public const string TRUNCATED_DISPLAY_TEXT = "*** WARNING ***  The value of this attribute has been modified for display. Do NOT update the value here!";

        public static string CreateGroupDefinitionAttribute(string groupName)
        {
            if (string.IsNullOrWhiteSpace(groupName))
            {
                return null;
            }

            return AttributeConstants.GROUP_DEFINITION_BASE_NAME + groupName;
        }
    }

    /// <summary>
    /// Customer specific attributes.
    /// </summary>

    public static class CustomerAttributeConstants
    {
        /// <summary>
        /// Customer specific attribute for CUK.
        /// This is a Sign Off Rank mapping.
        /// </summary>
        /// <remarks>
        /// StudentRank: SIR1, SIR2, SIR3,...
        /// The ranks are case-insensitive.
        /// </remarks>

        public const string CUSTOMER_CUK_SIR_MAPPING = "Customer.CUK.SIRMapping";
}
}
