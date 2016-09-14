using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace EC.Constants
{
    /// <summary>
    /// Miscellaneous constants for import/export.
    /// </summary>

    public static class ImportExportConstants
    {
        /// <summary>
        /// Prefix on an ImportName for links that points to an existing NavPage path.
        /// </summary>

        public const string ExistingImportNamePathPrefix = "#!#";
    }

    /// <summary>
    /// Contains regular expressions to scan the import structure for files and directories.
    /// </summary>

    public static class MatchFileAndDirExpressions
    {
        /// <summary>
        /// "\\.xml$"
        /// </summary>

        public const string MATCH_XML_FILES = "\\.xml$";

        /// <summary>
        /// "\\.csv$"
        /// </summary>

        public const string MATCH_USER_CSV_FILES = "\\.csv$";

        /// <summary>
        /// "\\.Users$"
        /// </summary>

        public const string MATCH_USER_DIRECTORIES = "\\.Users$";

        /// <summary>
        /// "\\.Users.xml$"
        /// </summary>

        public const string MATCH_USERS_XML_FILES = "\\.Users.xml$";

        /// <summary>
        /// "\\.Notifications.xml$"
        /// </summary>

        public const string MATCH_NOTIFICATIONS_XML_FILES = "\\.Notifications.xml$";

        /// <summary>
        /// "\\.OrgNotificationRules.xml$"
        /// </summary>

        public const string MATCH_ORG_NOTIFICATION_RULES_XML_FILES = "^OrgNotificationRules.xml$";

        /// <summary>
        /// "\\.UserNotificationRules.xml$"
        /// </summary>

        public const string MATCH_USER_NOTIFICATION_RULES_XML_FILES = "^UserNotificationRules.xml$";

        /// <summary>
        /// "\\.Certificates$"
        /// </summary>

        public const string MATCH_CERTIFICATE_DIRECTORIES = "\\.Certificates$";

        /// <summary>
        /// "\\.Certificates.xml$"
        /// </summary>

        public const string MATCH_CERTIFICATES_XML_FILES = "\\.Certificates.xml$";

        /// <summary>
        /// "\\.CertificateAudits.xml$"
        /// </summary>

        public const string MATCH_CERTIFICATE_AUDITS_XML_FILES = "\\.CertificateAudits.xml$";

        /// <summary>
        /// "\\.CertificateAuditStates.xml$"
        /// </summary>

        public const string MATCH_CERTIFICATE_AUDIT_STATES_XML_FILES = "\\.CertificateAuditStates.xml$";

        /// <summary>
        /// "\\.CertificateAuditModes.xml$"
        /// </summary>

        public const string MATCH_CERTIFICATE_AUDIT_MODES_XML_FILES = "\\.CertificateAuditModes.xml$";

        /// <summary>
        /// "\\.CertificateAuditRenderedTemplates.xml$"
        /// </summary>

        public const string MATCH_CERTIFICATE_AUDIT_RENDERED_TEMPLATES_XML_FILES = "\\.CertificateAuditRenderedTemplates.xml$";

        /// <summary>
        /// "^OrgProfileFields.xml$"
        /// </summary>

        public const string MATCH_ORG_PROFILE_FIELDS_XML_FILE = "^OrgProfileFields.xml$";

        /// <summary>
        /// "\\.OrgProfileFieldAudits.xml$"
        /// </summary>

        public const string MATCH_ORG_PROFILE_FIELD_AUDITS_FILES = "\\.OrgProfileFieldAudits.xml$";

        /// <summary>
        /// "\\.OrgProfileValueAudits.xml$"
        /// </summary>

        public const string MATCH_ORG_PROFILE_VALUE_AUDITS_FILES = "\\.OrgProfileValueAudits.xml$";

        /// <summary>
        /// "\\.Offerings$"
        /// </summary>

        public const string MATCH_OFFERINGS_DIRECTORIES = "\\.Offerings$";

        /// <summary>
        /// "^CourseGradeBookSchemas.xml$"
        /// </summary>

        public const string MATCH_COURSE_GRADEBOOK_SCHEMA_DIRECTORY = "^CourseGradeBookSchemas.xml$";

        /// <summary>
        /// "\\.Offerings.xml$"
        /// </summary>

        public const string MATCH_OFFERINGS_XML_FILES = "\\.Offerings.xml$";

        /// <summary>
        /// "\\.Audits$"
        /// </summary>

        public const string MATCH_AUDITS_DIRECTORIES = "\\.Audits$";

        /// <summary>
        /// "\\.Notifications$"
        /// </summary>

        public const string MATCH_NOTIFICATIONS_DIRECTORIES = "\\.Notifications$";

        /// <summary>
        /// "\\.ExamInstanceAudits.xml$"
        /// </summary>

        public const string MATCH_EXAM_INSTANCE_AUDITS_XML_FILES = "\\.ExamInstanceAudits.xml$";

        /// <summary>
        /// "\\CourseOfferingAudits.xml$"
        /// </summary>

        public const string MATCH_COURSE_OFFERING_AUDITS_XML_FILES = "\\.CourseOfferingAudits.xml$";

        /// <summary>
        /// "\\CourseOfferingAuditStates.xml$"
        /// </summary>

        public const string MATCH_COURSE_OFFERING_AUDIT_STATES_XML_FILES = "\\.CourseOfferingAuditStates.xml$";

        /// <summary>
        /// "\\InstructorAudits.xml$"
        /// </summary>

        public const string MATCH_INSTRUCTOR_AUDITS_XML_FILES = "\\.InstructorAudits.xml$";

        /// <summary>
        /// "\\InstructorAuditStates.xml$"
        /// </summary>

        public const string MATCH_INSTRUCTOR_AUDIT_STATES_XML_FILES = "\\.InstructorAuditStates.xml$";

        /// <summary>
        /// "\\RegistrationAudits.xml$"
        /// </summary>

        public const string MATCH_REGISTRATION_AUDITS_XML_FILES = "\\.RegistrationAudits.xml$";

        /// <summary>
        /// "\\RegistrationAuditStates.xml$"
        /// </summary>

        public const string MATCH_REGISTRATION_AUDIT_STATES_XML_FILES = "\\.RegistrationAuditStates.xml$";

        ///// <summary>
        ///// "\\Tracking.OfferingAudits.xml$"
        ///// </summary>

        //public const string MATCH_TRACKING_OFFERING_AUDITS_XML_FILES = "\\.Tracking.OfferingAudits.xml$";  

        /// <summary>
        /// "\\.SessionAudits.xml$"
        /// </summary>

        public const string MATCH_SESSION_AUDITS_XML_FILES = "\\.SessionAudits.xml$";

        /// <summary>
        /// "\\.TrackingAudits.xml$"
        /// </summary>

        public const string MATCH_TRACKING_AUDITS_XML_FILES = "\\.TrackingAudits.xml$";

        /// <summary>
        /// \\.UserItemAudits.xml$"
        /// </summary>

        public const string MATCH_USER_ITEM_AUDITS_XML_FILES = "\\.UserItemAudits.xml$";

        /// <summary>
        /// "\\.AuditUserItemStates.xml$"
        /// </summary>

        public const string MATCH_AUDIT_USER_ITEM_STATES_XML_FILES = "\\.AuditUserItemStates.xml$";

        /// <summary>
        /// "\\.Pg$"
        /// </summary>

        public const string MATCH_PAGE_DIRECTORIES = "\\.Pg$";

        /// <summary>
        /// "\\.Page.xml$"
        /// </summary>

        public const string MATCH_PAGE_FILES = "\\.Page.xml$";

        /// <summary>
        /// "\\.Questions$"
        /// </summary>

        public const string MATCH_QUESTION_COLLECTION_DIRECTORIES = "\\.Questions$";

        /// <summary>
        /// "\\.Exams$"
        /// </summary>

        public const string MATCH_EXAM_COLLECTION_DIRECTORIES = "\\.Exams$";

        /// <summary>
        /// "\\.Repo$"
        /// </summary>

        public const string MATCH_FILE_COLLECTION_DIRECTORIES = "\\.Repo$";

        /// <summary>
        /// "\\.Repository.xml$"
        /// </summary>

        public const string MATCH_FILE_COLLECTION_FILE = "\\.Repository.xml$";

        /// <summary>
        /// Directory name for a file collection content ("^Cnt$").
        /// </summary>

        public const string MATCH_FILE_COLLECTION_CONTENT_DIRECTORY_NAME = "^Cnt$";

        /// <summary>
        /// "\\.Registrations.xml$"
        /// </summary>

        public const string MATCH_REGISTRATION_FILES = "\\.Registrations.csv$";

        /// <summary>
        /// "^ParentLinkAttributes.xml$"
        /// </summary>

        public const string MATCH_PARENT_LINK_ATTRIBUTES = "^ParentLinkAttributes.xml$";

        /// <summary>
        /// "^ParentLinkGroups.xml$"
        /// </summary>

        public const string MATCH_PARENT_LINK_GROUPS = "^ParentLinkGroups.xml$";

        /// <summary>
        /// "^Tags.xml$"
        /// </summary>

        public const string MATCH_TAGS_XML_FILE = "^Tags.xml$";

        /// <summary>
        /// "^TagMappings.xml$"
        /// </summary>

        public const string MATCH_TAG_MAPPINGS_XML_FILE = "^TagMappings.xml$";

        /// <summary>
        /// "\\.Cpms$"
        /// </summary>

        public const string MATCH_CPMS_COLLECTION_DIRECTORIES = "\\.Cpms$";

        /// <summary>
        /// "\\.TrainingCollection.xml$"
        /// </summary>

        public const string MATCH_CPMS_TRAINING_COLLECTION_FILE = "\\.TrainingTemplate.Collection.xml$";

        /// <summary>
        /// "\\.CompetencyCollection.xml$"
        /// </summary>

        public const string MATCH_CPMS_COMPETENCY_COLLECTION_FILE = "\\.CompetencyTemplate.Collection.xml$";
    }

    /// <summary>
    /// File and directory names for exporting data.
    /// </summary>

    public static class ExportFileAndDirTemplates
    {
        /// <summary>
        /// Directory to put course offering files ("{0}.Offerings").
        /// </summary>

        public const string OFFERINGS_DIRECTORY_NAME_TEMPLATE = "{0}.Offerings";

        /// <summary>
        /// Course offering export file ("{0}.Offerings.xml").
        /// </summary>

        public const string OFFERINGS_FILE_NAME_TEMPLATE = "{0}.Offerings.xml";

        /// <summary>
        /// User item audit file name ("{0}.UserItemAudits.xml").
        /// </summary>

        public const string USER_ITEM_AUDITS_FILE_NAME_TEMPLATE = "{0}.UserItemAudits.xml";

        /// <summary>
        /// Audit user item state filename ("{0}.AuditUserItemStates.xml").
        /// </summary>

        public const string AUDIT_USER_ITEM_STATES_FILE_NAME_TEMPLATE = "{0}.AuditUserItemStates.xml";

        /// <summary>
        /// Course offering audit export file ("{0}.CourseOfferingAudits.xml").
        /// </summary>

        public const string COURSE_OFFERING_AUDITS_FILE_NAME_TEMPLATE = "{0}.CourseOfferingAudits.xml";

        /// <summary>
        /// Course offering audit states file ("{0}.CourseOfferingAuditStates.xml")
        /// </summary>

        public const string COURSE_OFFERING_AUDIT_STATES_FILE_NAME_TEMPLATE = "{0}.CourseOfferingAuditStates.xml";

        /// <summary>
        /// Instructor audits file ("{0}.InstructorAudits.xml")
        /// </summary>

        public const string INSTRUCTOR_AUDIT_FILE_NAME_TEMPLATE = "{0}.InstructorAudits.xml";

        /// <summary>
        /// Instructor audit states file ("{0}.InstructorAuditStates.xml")
        /// </summary>

        public const string INSTRUCTOR_AUDIT_STATES_FILE_NAME_TEMPLATE = "{0}.InstructorAuditStates.xml";

        /// <summary>
        /// Registration audit file ("{0}.RegistrationAudits.xml")
        /// </summary>

        public const string REGISTRATION_AUDIT_FILE_NAME_TEMPLATE = "{0}.RegistrationAudits.xml";

        /// <summary>
        /// Registration audit states file ("{0}.RegistrationAuditStates.xml")
        /// </summary>

        public const string REGISTRATION_AUDIT_STATES_FILE_NAME_TEMPLATE = "{0}.RegistrationAuditStates.xml";


        /// <summary>
        /// Exam instance audit export file ("{0}.ExamInstanceAudits.xml").
        /// </summary>

        public const string EXAM_INSTANCE_AUDITS_FILE_NAME_TEMPLATE = "{0}.ExamInstanceAudits.xml";

        /// <summary>
        /// Template directory for audits ("{0}.Audits").
        /// </summary>

        public const string AUDITS_DIRECTORY_NAME_TEMPLATE = "{0}.Audits";

        /// <summary>
        /// Tracking audit export file ("{0}.TrackingAudits.xml").
        /// </summary>

        public const string TRACKING_AUDITS_FILE_NAME_TEMPLATE = "{0}.TrackingAudits.xml";

        /// <summary>
        /// Session audit export file ("{0}.SessionAudits.xml");
        /// </summary>

        public const string SESSION_AUDITS_FILE_NAME_TEMPLATE = "{0}.SessionAudits.xml";

        /// <summary>
        /// Org profile field export file ("OrgProfileFields.xml").
        /// </summary>

        public const string ORG_PROFILE_FIELDS_FILE_NAME = "OrgProfileFields.xml";

        /// <summary>
        /// Org profile field audit
        /// </summary>

        public const string ORG_PROFILE_FIELDS_AUDITS_FILE_NAME_TEMPLATE = "{0}.OrgProfileFieldAudits.xml";

        /// <summary>
        /// Org profile value audit.
        /// </summary>

        public const string ORG_PROFILE_VALUE_AUDITS_FILE_NAME_TEMPLATE = "{0}.OrgProfileValueAudits.xml";

        /// <summary>
        /// Main NavPage directory name template ("{0}.Pg").
        /// </summary>

        public const string PAGE_DIRECTORY_NAME_TEMPLATE = "{0}.Pg";

        /// <summary>
        /// NavPage export file name template ("{0}.Page.xml").
        /// </summary>

        public const string PAGE_FILE_NAME_TEMPLATE = "{0}.Page.xml";

        /// <summary>
        /// Parent link attribute file name ("ParentLinkAttributes.xml").
        /// </summary>

        public const string PARENT_LINK_ATTRIBUTES_FILE_NAME = "ParentLinkAttributes.xml";

        /// <summary>
        /// Parent link group attribute file name ("ParentLinkGroups.xml").
        /// </summary>

        public const string PARENT_LINK_GROUPS_FILE_NAME = "ParentLinkGroups.xml";

        /// <summary>
        /// Directory name template for the file collection ("{0}.Repo").
        /// </summary>

        public const string FILE_COLLECTION_DIRECTORY_NAME_TEMPLATE = "{0}.Repo";

        /// <summary>
        /// File name of the file collection import XML document ("{0}.Repository.xml").
        /// </summary>

        public const string FILE_COLLECTION_FILE_NAME_TEMPLATE = "{0}.Repository.xml";

        /// <summary>
        /// Directory name for a file collection content ("Cnt").
        /// </summary>

        public const string FILE_COLLECTION_CONTENT_DIRECTORY_NAME = "Cnt";

        /// <summary>
        /// Directory name template for the question collection ("{0}.Questions").
        /// </summary>

        public const string QUESTION_COLLECTION_DIRECTORY_NAME_TEMPLATE = "{0}.Questions";

        /// <summary>
        /// Directory name template for the exam collection ("{0}.Exam").
        /// </summary>

        public const string EXAM_COLLECTION_DIRECTORY_NAME_TEMPLATE = "{0}.Exams";

        /// <summary>
        /// Directory name template for the user csv-files ("{0}.Users").
        /// </summary>

        public const string USERS_DIRECTORY_NAME_TEMPLATE = "{0}.Users";

        /// <summary>
        /// Directory name template for the notifications file ("{0}.Notifications").
        /// </summary>

        public const string NOTIFICATIONS_DIRECTORY_NAME_TEMPLATE = "{0}.Notifications";

        /// <summary>
        /// User file name template ("{0}.csv").
        /// </summary>

        public const string USERS_CSV_FILE_NAME_TEMPLATE = "{0}.csv";

        /// <summary>
        /// User file name template ("{0}.Users.xml").
        /// </summary>

        public const string USERS_XML_FILE_NAME_TEMPLATE = "{0}.Users.xml";

        /// <summary>
        /// Notifications file name template ("{0}.Notifications.xml").
        /// </summary>

        public const string NOTIFICATIONS_XML_FILE_NAME_TEMPLATE = "{0}.Notifications.xml";

        /// <summary>
        /// Directory name template for the certificates ("{0}.Certificates").
        /// </summary>

        public const string CERTIFICATE_DIRECTORY_NAME_TEMPLATE = "{0}.Certificates";

        /// <summary>
        /// Template for xml file name ("{0}.Certificates.xml").
        /// The file name usually contains the Organization UrlName as first part of the file name.
        /// </summary>

        public const string CERTIFICATE_FILE_NAME_TEMPLATE = "{0}.Certificates.xml";

        /// <summary>
        /// Template for xml file name ("{0}.CertificateAudits.xml").
        /// The file name usually contains the Organization UrlName as first part of the file name.
        /// </summary>

        public const string CERTIFICATE_AUDITS_FILE_NAME_TEMPLATE = "{0}.CertificateAudits.xml";

        /// <summary>
        /// Template for xml file name ("{0}.CertificateAuditStates.xml").
        /// The file name usually contains the Organization UrlName as first part of the file name.
        /// </summary>

        public const string CERTIFICATE_AUDIT_STATES_FILE_NAME_TEMPLATE = "{0}.CertificateAuditStates.xml";

        /// <summary>
        /// Template for xml file name ("{0}.CertificateAuditModes.xml").
        /// The file name usually contains the Organization UrlName as first part of the file name.
        /// </summary>

        public const string CERTIFICATE_AUDIT_MODES_FILE_NAME_TEMPLATE = "{0}.CertificateAuditModes.xml";

        /// <summary>
        /// Template for xml file name ("{0}.CertificateAuditRenderedTemplates.xml").
        /// The file name usually contains the Organization UrlName as first part of the file name.
        /// </summary>

        public const string CERTIFICATE_AUDIT_RENDERED_TEMPLATES_FILE_NAME_TEMPLATE = "{0}.CertificateAuditRenderedTemplates.xml";

        /// <summary>
        /// Template for xml file name ("{0}.xml").
        /// </summary>

        public const string XML_FILE_NAME_TEMPLATE = "{0}.xml";

        /// <summary>
        /// Template file name for exam collections.
        /// </summary>

        public const string EXAM_COLLECTION_FILE_NAME = "{0}.Exams.xml";

        /// <summary>
        /// Template file name for the question collections.
        /// </summary>

        public const string QUESTION_COLLECTION_FILE_NAME = "{0}.Questions.xml";

        /// <summary>
        /// Template file name for user notification rules.
        /// </summary>

        public const string ORG_NOTIFICATION_RULE_FILE_NAME = "OrgNotificationRules.xml";

        /// <summary>
        /// Template file name for user notification rules.
        /// </summary>

        public const string USER_NOTIFICATION_RULE_FILE_NAME = "UserNotificationRules.xml";

        /// <summary>
        /// Template file name for tag sets
        /// </summary>

        public const string TAGS_FILE_NAME = "Tags.xml";
        
        /// <summary>
        /// Template file name for tag mappings
        /// </summary>

        public const string TAG_MAPPINGs_FILE_NAME = "TagMappings.xml";

        /// <summary>
        /// Template file name for course grade book schema
        /// </summary>

        public const string COURSE_GRADE_BOOK_SCHEMA_FILE_NAME = "CourseGradeBookSchemas.xml";

        /// <summary>
        /// Directory name template for CPMS data
        /// </summary>

        public const string CPMS_DIRECTORY_NAME_TEMPLATE = "{0}.Cpms";

        /// <summary>
        /// Template file name for Training Template Collections
        /// </summary>

        public const string CPMS_TRAINING_COLLECTION_FILE_NAME = "{0}.TrainingTemplate.Collection.xml";

        /// <summary>
        /// Template file name for Competency Template Collections
        /// </summary>

        public const string CPMS_COMPETENCY_COLLECTION_FILE_NAME = "{0}.CompetencyTemplate.Collection.xml";
    }


    /// <summary>
    /// Type of the page on import (either TOC or NavPage).
    /// </summary>

    public enum PageTypeEnum
    {
        /// <summary>
        /// If the imported page is a NavPage.
        /// </summary>

        NavPage = 0,

        /// <summary>
        /// If the imported page is a table of content (TOC).
        /// </summary>

        TOC = 1
    }

    /// <summary>
    /// Different modes for the import process.
    /// </summary>

    [DataContract]
    public enum ImportModesEnum
    {
        /// <summary>
        /// Used to set up the root nav page
        /// </summary>
        /// <remarks>0 used to be ManualPackage</remarks>
        [EnumMember]
        SetupRootNavPage = 0,

        /// <summary>
        /// Used to restore a backup of a customers content tree. No existing content items must match the import Ids (delete previous items).
        /// </summary>

        [EnumMember]
        Restore = 1,


        // 2 used to be CopyLocal but it's been removed.

        /// <summary>
        /// Create duplicate produced on another instance (i.e. to transfer data from live to test).
        /// Every imported item gets a new Id.
        /// </summary>
        /// <remarks>
        /// New primary Ids are generated for the content. If a user doesn't exist - create it with the provided Id.
        /// </remarks>

        [EnumMember]
        Copy = 3,

        /// <summary>
        /// Import data from an RTA to the main site. No content will be imported.
        /// </summary>

        [EnumMember]
        RTAToMain = 4,

        /// <summary>
        /// Import data from the main site to the RTA. Content items will be created and overwritten as needed.
        /// </summary>

        [EnumMember]
        MainToRTA = 5,

        /// <summary>
        /// Import structure only with new Ids.
        /// </summary>
        /// <remarks>
        /// Items imported: 
        /// * NavPages
        /// * Links
        /// * OrgProfileFields
        /// * all types of Collections
        /// * attributes
        /// * org notification rules
        /// * Certificates
        /// 
        /// Items not imported:
        /// * Audits of any kind
        /// * Users
        /// * user notification rules
        /// * Offerings
        /// * Registrations
        /// </remarks>

        [EnumMember]
        StructureOnly = 6,

        /// <summary>
        /// Import structure and offerings with new Ids.
        /// </summary>
        /// <remarks>
        /// Items imported: 
        /// * NavPages
        /// * Links
        /// * Offerings
        /// * OrgProfileFields
        /// * all types of Collections
        /// * attributes
        /// * org notification rules
        /// * Certificates
        /// 
        /// Items not imported:
        /// * Audits of any kind
        /// * Users
        /// * user notification rules
        /// * Registrations
        /// </remarks>

        [EnumMember]
        StructureWithOfferings = 7
    }

    /// <summary>
    /// Different modes for the export process.
    /// </summary>

    [DataContract]
    public enum ExportModesEnum
    {
        /// <summary>
        /// All content, offerings, registrations, and audit records.
        /// </summary>
        /// <remarks>
        /// This automatically sets all export flags.
        /// </remarks>

        [EnumMember]
        Full = 0,

        /// <summary>
        /// Export package for Main to RTA.
        /// </summary>

        [EnumMember]
        MainToRTA = 1,

        /// <summary>
        /// Export package for RTA to Main (Users and performance related audits).
        /// </summary>

        [EnumMember]
        RTAToMain = 3,

        /// <summary>
        /// Custom export - allow export flags to be set manually.
        /// </summary>

        [EnumMember]
        Custom = 4
    }

    /// <summary>
    /// Flags to decide what to export (i.e. content).
    /// Combine multiple items to export multiple parts (i.e. content + offerings).
    /// </summary>
    /// <remarks>
    /// Some flags can't be combined without requiring another flag (i.e. registrations can't be exported without offerings).
    /// <para>
    /// Because DTO enums must start at 0 (and 0 is not a good value for a bitmask), the 0 value is listed as Unused.
    /// </para> 
    /// </remarks>

    [Flags]
    [DataContract]
    public enum ExportFlagsEnum
    {
        /// <summary>
        /// Not used - just there for compatibility purpose.
        /// </summary>

        [EnumMember]
        Unused = 0,

        /// <summary>
        /// The structure consist of NavPages (incl. links), questions, exams, link and page attributes, but not FileCollections!
        /// </summary>

        [EnumMember]
        Structure = 1,

        [EnumMember]
        Users = 2,

        [EnumMember]
        Offerings = 4,

        /// <summary>
        /// OfferingAudits - only separately available on RTAToMain.
        /// </summary>

        [EnumMember]
        OfferingAudits = 8,

        [EnumMember]
        Registrations = 16,

        /// <summary>
        /// RegistrationAudits - only separately available on RTAToMain but in combination with 'OfferingAudits'.
        /// </summary>

        [EnumMember]
        RegistrationAudits = 32,

        [EnumMember]
        ExamAudits = 64,

        [EnumMember]
        TrackingAudits = 128,

        [EnumMember]
        SessionAudits = 256,

        [EnumMember]
        Certificates = 512,

        [EnumMember]
        CertificateAudits = 1024,

        /// <summary>
        /// OrgProfileFields are replaced per organization. All dependent items like OrgProfileValues and Users 
        /// must be updated accordingly.
        /// </summary>
        /// <remarks>
        /// Fields of an organization are referenced by OrgProfileValues. OrgProfileValues are bound to users and therefore
        /// users must be exported/imported if OrgProfileFields and OrgProfileValues are exported/imported.
        /// OrgProfileFieldAudits are exported automatically if OrgProfileFields are exported.
        /// </remarks>

        [EnumMember]
        OrgProfileFields = 2048,

        /// <summary>
        /// Export organization profile values (this means including users).
        /// </summary>

        [EnumMember]
        OrgProfileValues = 4096,

        /// <summary>
        /// This flag indicates that the sync package includes a software update
        /// the software update could be either a full installer or a patch installer
        /// </summary>

        [EnumMember]
        Software = 8192,

        /// <summary>
        /// Export notifications (all types).
        /// </summary>

        [EnumMember]
        Notifications = 16384,

        /// <summary>
        /// Export all file collections.
        /// </summary>

        [EnumMember]
        FileCollections = 32768,

        /// <summary>
        /// Set all export flags of this enum.
        /// </summary>

        [EnumMember]
        All = ~0
    }


    public static class ExportConstants
    {
        /// <summary>
        /// All export flags set.
        /// </summary>

        public static readonly ExportFlagsEnum AllExportFlags = ExportFlagsEnum.All;

        /// <summary>
        /// Export flags used to create a MainToRta package with placeholders as file collections.
        /// </summary>

        public static readonly ExportFlagsEnum MainToRtaWithoutFileCollectionsExportFlags = ExportFlagsEnum.Certificates
                                                             | ExportFlagsEnum.CertificateAudits
                                                             | ExportFlagsEnum.Structure
                                                             | ExportFlagsEnum.Offerings
                                                             | ExportFlagsEnum.OfferingAudits
                                                             | ExportFlagsEnum.Registrations
                                                             | ExportFlagsEnum.RegistrationAudits
                                                             | ExportFlagsEnum.SessionAudits
                                                             | ExportFlagsEnum.TrackingAudits
                                                             | ExportFlagsEnum.ExamAudits
                                                             | ExportFlagsEnum.Users
                                                             | ExportFlagsEnum.OrgProfileFields
                                                             | ExportFlagsEnum.OrgProfileValues;

        /// <summary>
        /// Export flags used to create a MainToRta package with file collections (diffed files with the help of file collection snapshots).
        /// </summary>

        public static readonly ExportFlagsEnum MainToRtaWithFileCollectionsExportFlags = MainToRtaWithoutFileCollectionsExportFlags | ExportFlagsEnum.FileCollections;


        /// <summary>
        /// Export flags used to create an RtaToMain package.
        /// </summary>

        public static readonly ExportFlagsEnum RtaToMainExportFlags = ExportFlagsEnum.ExamAudits
                                                            | ExportFlagsEnum.SessionAudits
                                                            | ExportFlagsEnum.TrackingAudits
                                                            | ExportFlagsEnum.Notifications;

        /// <summary>
        /// Custom export flags to include Structure, Certificates, OrgProfileFields and OrganizationNotificationRules only.
        /// </summary>

        public static readonly ExportFlagsEnum CustomStructureExportFlags = ExportFlagsEnum.Structure
                                                            | ExportFlagsEnum.Certificates
                                                            | ExportFlagsEnum.OrgProfileFields;
        /// <summary>
        /// Custom export flags to include Users and OrgProfileValues.
        /// </summary>
        /// <remarks>
        /// Use those flags only in combination with <see cref="CustomStructureExportFlags"/>.
        /// </remarks>

        public static readonly ExportFlagsEnum CustomExportFlagsUsers = ExportFlagsEnum.Users
                                                            | ExportFlagsEnum.OrgProfileValues;
        /// <summary>
        /// Custom export flags to include all user generated data like Offerings, Registration, Users and Audits.
        /// </summary>
        /// <remarks>
        /// Use those flags only in combination with <see cref="CustomStructureExportFlags"/>.
        /// </remarks>

        public static readonly ExportFlagsEnum CustomExportFlagsUserGeneratedData = ExportFlagsEnum.CertificateAudits
                                                            | ExportFlagsEnum.Offerings
                                                            | ExportFlagsEnum.OfferingAudits
                                                            | ExportFlagsEnum.Registrations
                                                            | ExportFlagsEnum.RegistrationAudits
                                                            | ExportFlagsEnum.SessionAudits
                                                            | ExportFlagsEnum.TrackingAudits
                                                            | ExportFlagsEnum.ExamAudits
                                                            | ExportFlagsEnum.OrgProfileFields
                                                            | ExportFlagsEnum.Users
                                                            | ExportFlagsEnum.OrgProfileValues;

        /// <summary>
        /// Export flags for all audits.
        /// </summary>

        public static ExportFlagsEnum AllAudits = ExportFlagsEnum.CertificateAudits
                                                             | ExportFlagsEnum.OfferingAudits
                                                             | ExportFlagsEnum.RegistrationAudits
                                                             | ExportFlagsEnum.SessionAudits
                                                             | ExportFlagsEnum.TrackingAudits
                                                             | ExportFlagsEnum.ExamAudits;
    }

    /// <summary>
    /// This enum defines the possible types of software to include with the sync package.
    /// </summary>

    [DataContract]
    public enum SoftwareExportOverrideTypes
    {
        [EnumMember]
        NoSoftware = 0,

        [EnumMember]
        NoOverride = 1,

        [EnumMember]
        FullInstaller = 2
    }

}
