using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EC.Constants
{
    /// <summary>
    /// Constant basic capability names as string.
    /// </summary>

    public static class CapabilityConstants
    {        
        /// <summary>
        /// Capability required for user to view the contents of a content item.
        /// </summary>

        public const string CAN_VIEW = "System.Capability.CanView";

        /// <summary>
        /// Capability required for user to view the attributes of a ContentItem.
        /// </summary>

        public const string CAN_VIEW_ATTRIBUTES = "System.Capability.CanViewAttributes";

        /// <summary>
        /// Capability required for user to edit link attributes.
        /// </summary>

        public const string CAN_MANAGE_LINK_ATTRIBUTES = "System.Capability.CanManageLinkAttributes";

        /// <summary>
        /// Capability required for user to edit page attributes (these attributes are used as defaults for links).
        /// </summary>

        public const string CAN_MANAGE_PAGE_ATTRIBUTES = "System.Capability.CanManagePageAttributes";

        /// <summary>
        /// Capability required to be able to take an exam.
        /// </summary>

        public const string CAN_TAKE_EXAM = "System.Capability.CanTakeExam";

        /// <summary>
        /// Capability required to be able to evaluate a student.
        /// </summary>

        public const string CAN_EVALUATE_STUDENT = "System.Capability.CanEvaluateStudent";

        /// <summary>
        /// Capability that a user needs in order to view tracking audits
        /// </summary>


        public const string CAN_VIEW_TRACKING_AUDITS = "System.Capability.CanViewTrackingAudits";

        /// <summary>
        /// Capability that a user needs in order to view session audits
        /// </summary>

        public const string CAN_VIEW_SESSION_AUDITS = "System.Capability.CanViewSessionAudits";

        /// <summary>
        /// Capability that a user needs in order to create/delete/edit (all types of) links.
        /// </summary>

        public const string CAN_MANAGE_LINKS = "System.Capability.CanManageLinks";

        /// <summary>
        /// Capability that a user needs in order to edit page default values.
        /// </summary>

        public const string CAN_MANAGE_PAGE_DEFAULTS = "System.Capability.CanManagePageDefaults";

        /// <summary>
        /// Capability that a user needs in order to delete a file collection
        /// </summary>

        public const string CAN_DELETE_FILE_COLLECTIONS = "System.Capability.CanDeleteFileCollections";

        /// <summary>
        /// Capability that a user needs in order to create/delete/edit files/folders within
        /// a file collection.
        /// </summary>

        public const string CAN_MANAGE_FILES = "System.Capability.CanManageFiles";

        /// <summary>
        /// Capability that a user needs in order to view files/folders within a file collection.
        /// </summary>

        public const string CAN_VIEW_FILES = "System.Capability.CanViewFiles";

        /// <summary>
        /// Capability that a user needs in order to create/delete/edit questions within
        /// a question collection.
        /// </summary>

        public const string CAN_MANAGE_QUESTIONS = "System.Capability.CanManageQuestions";

        /// <summary>
        /// Capability that a user needs in order to retrieve questions from a question
        /// collection.
        /// </summary>

        public const string CAN_VIEW_QUESTIONS = "System.Capability.CanViewQuestions";

        /// <summary>
        /// Capability that a user needs in order to create/delete/edit exams within
        /// an exam collection.
        /// </summary>

        public const string CAN_MANAGE_EXAMS = "System.Capability.CanManageExams";

        /// <summary>
        /// Capability that a user needs in order to create/delete/edit categories.
        /// </summary>

        public const string CAN_MANAGE_CATEGORIES = "System.Capability.CanManageCategories";

        /// <summary>
        /// Capability that a user needs in order to delete a question collection.
        /// </summary>

        public const string CAN_DELETE_QUESTION_COLLECTIONS = "System.Capability.CanDeleteQuestionCollections";

        /// <summary>
        /// Capability that a user needs in order to delete an exam collection.
        /// </summary>

        public const string CAN_DELETE_EXAM_COLLECTIONS = "System.Capability.CanDeleteExamCollections";

        /// <summary>
        /// Capability that a user needs in order to view the list of registrants
        /// associated with a course.
        /// </summary>

        public const string CAN_VIEW_REGISTRATIONS = "System.Capability.CanViewRegistrations";

        /// <summary>
        /// Capability that a user needs in order to see offerings (viewing registrations/instructors are separate capabilities).
        /// </summary>

        public const string CAN_VIEW_OFFERINGS = "System.Capability.CanViewOfferings";

        /// <summary>
        /// Capability needed to create a course offering.
        /// </summary>

        public const string CAN_CREATE_OFFERINGS = "System.Capability.CanCreateOfferings";

        /// <summary>
        /// Capability that a user needs in order to edit offerings (add/delete instructors are included for the moment for some service calls).
        /// </summary>

        public const string CAN_EDIT_OFFERINGS = "System.Capability.CanEditOfferings";

        /// <summary>
        /// Capability that a user needs in order to add instructors to course offerings.
        /// </summary>

        public const string CAN_ADD_INSTRUCTORS = "System.Capability.CanAddInstructors";

        /// <summary>
        /// Capability that a user needs in order to delete instructors from course offerings.
        /// </summary>

        public const string CAN_DELETE_INSTRUCTORS = "System.Capability.CanDeleteInstructors";

        /// <summary>
        /// Capability that a user needs in order to delete a course offerings.
        /// </summary>

        public const string CAN_DELETE_OFFERINGS = "System.Capability.CanDeleteOfferings";

        /// <summary>
        /// Capability that a user needs in order to register students for an offering.
        /// </summary>

        public const string CAN_REGISTER_STUDENTS = "System.Capability.CanRegisterStudents";

        /// <summary>
        /// Capability that a user needs in order to de-register students for an offering.
        /// </summary>

        public const string CAN_DE_REGISTER_STUDENTS = "System.Capability.CanDeRegisterStudents";

        /// <summary>
        /// 'System.Capability.CanDisplayAsLink' capability name. 
        /// Used to hide internal links on an index page. If the target content item has this capability as an 
        /// attribute, the containing predicate is used to evaluate whether the link is displayed or not.
        /// </summary>

        public const string CAN_DISPLAY_AS_LINK = "System.Capability.CanDisplayAsLink";

        /// <summary>
        /// 'System.Capability.CanViewExamReport' capability name.
        ///  Capability user needs to view reports on exams
        /// </summary>

        public const string CAN_VIEW_EXAM_REPORT = "System.Capability.CanViewExamReport";

        /// <summary>
        /// 'System.Capability.CanOverrideExam' capability name.
        /// Capability user needs to override exams and exam questions 
        /// </summary>

        public const string CAN_OVERRIDE_EXAM = "System.Capability.CanOverrideExam";

        /// <summary>
        /// Capability that a user needs in order to retrieve exams from a exam
        /// collection.
        /// </summary>

        public const string CAN_VIEW_EXAM = "System.Capability.CanViewExam";

        /// <summary>
        /// Capability needed to proctor an exam.
        /// </summary>

        public const string CAN_PROCTOR_EXAM = "System.Capability.CanProctorExam";

        /// <summary>
        /// Capability needed to create a file collection.
        /// </summary>

        public const string CAN_CREATE_FILE_COLLECTIONS = "System.Capability.CanCreateFileCollections";

        /// <summary>
        /// Capability needed to create a question collection.
        /// </summary>

        public const string CAN_CREATE_QUESTION_COLLECTIONS = "System.Capability.CanCreateQuestionCollections";

        /// <summary>
        /// Capability needed to create an exam collection.
        /// </summary>

        public const string CAN_CREATE_EXAM_COLLECTIONS = "System.Capability.CanCreateExamCollections";

        /// <summary>
        /// Capability needed to view users and user reports.
        /// </summary>

        public const string CAN_VIEW_USERS = "System.Capability.CanViewUsers";

        /// <summary>
        /// Capability needed to create users.
        /// </summary>

        public const string CAN_CREATE_USERS = "System.Capability.CanCreateUsers";

        /// <summary>
        /// Capability needed to delete users.
        /// </summary>

        public const string CAN_DELETE_USERS = "System.Capability.CanDeleteUsers";

        /// <summary>
        /// Capability needed to edit users.
        /// </summary>

        public const string CAN_EDIT_USERS = "System.Capability.CanEditUsers";

        /// <summary>
        /// Capability needed to view exam instance audits
        /// </summary>

        public const string CAN_VIEW_EXAM_INSTANCE_AUDITS = "System.Capability.CanViewExamInstanceAudits";
        
        /// <summary>
        /// Capability needed to view certificate templates.
        /// </summary>

        public const string CAN_VIEW_CERTIFICATES = "System.Capability.CanViewCertificates";

        /// <summary>
        /// Capability needed to create certificate templates.
        /// </summary>

        public const string CAN_CREATE_CERTIFICATES = "System.Capability.CanCreateCertificates";

        /// <summary>
        /// Capability needed to edit certificate templates.
        /// </summary>

        public const string CAN_EDIT_CERTIFICATES = "System.Capability.CanEditCertificates";

        /// <summary>
        /// Capability needed to delete certificate templates.
        /// </summary>

        public const string CAN_DELETE_CERTIFICATES = "System.Capability.CanDeleteCertificates";

        /// <summary>
        /// Capability need to manually award a certificate audit.
        /// </summary>

        public const string CAN_AWARD_CERTIFICATE_AUDITS = "System.Capability.CanAwardCertificateAudits";

        /// <summary>
        /// Capability needed to manually revoke a certificate audit.
        /// </summary>

        public const string CAN_REVOKE_CERTIFICATE_AUDITS = "System.Capability.CanRevokeCertificateAudits";

        /// <summary>
        /// Capability needed to view certificate audits.
        /// </summary>

        public const string CAN_VIEW_CERTIFICATE_AUDITS = "System.Capability.CanViewCertificateAudits";

        /// <summary>
        /// Capability needed to view RTAs.
        /// </summary>

        public const string CAN_VIEW_RTA = "System.Capability.CanViewRTA";

        /// <summary>
        /// Capability needed to view course offering audits.
        /// </summary>

        public const string CAN_VIEW_COURSE_OFFERING_AUDITS = "System.Capability.CanViewCourseOfferingAudits";

        /// <summary>
        /// Capability needed to view registration audits.
        /// </summary>

        public const string CAN_VIEW_REGISTRATION_AUDITS = "System.Capability.CanViewRegistrationAudits";

        /// <summary>
        /// Capability needed to view user audits.
        /// </summary>

        public const string CAN_VIEW_USER_AUDITS = "System.Capability.CanViewUserAudits";

        /// <summary>
        /// Capability needed to view org profile field audits.
        /// </summary>

        public const string CAN_VIEW_ORG_PROFILE_FIELD_AUDITS = "System.Capability.CanViewOrgProfileFieldAudits";

        /// <summary>
        /// Capability needed to view org profile value audits.
        /// </summary>

        public const string CAN_VIEW_ORG_PROFILE_VALUE_AUDITS = "System.Capability.CanViewOrgProfileValueAudits";
        
        /// <summary>
        /// Capability needed to create new RTAs.
        /// </summary>

        public const string CAN_CREATE_RTA = "System.Capability.CanCreateRTA";

        /// <summary>
        /// Capability needed to delete RTAs.
        /// </summary>

        public const string CAN_DELETE_RTA = "System.Capability.CanDeleteRTA";

        /// <summary>
        /// Capability needed to edit an RTA.
        /// </summary>

        public const string CAN_EDIT_RTA = "System.Capability.CanEditRTA";

        /// <summary>
        /// Capability needed to create sync packages on Main.
        /// </summary>

        public const string CAN_CREATE_SYNC_PACKAGES_ON_MAIN = "System.Capability.CanCreateSyncPackagesOnMain";

        /// <summary>
        /// Capability needed to create sync packages on an RTA.
        /// </summary>

        public const string CAN_CREATE_SYNC_PACKAGES_ON_RTA = "System.Capability.CanCreateSyncPackagesOnRTA";

        /// <summary>
        /// Capability needed to apply sync packages on Main.
        /// </summary>

        public const string CAN_APPLY_SYNC_PACKAGES_ON_MAIN = "System.Capability.CanApplySyncPackagesOnMain";

        /// <summary>
        /// Capability needed to apply sync packages on an RTA.
        /// </summary>

        public const string CAN_APPLY_SYNC_PACKAGES_ON_RTA = "System.Capability.CanApplySyncPackagesOnRTA";

        /// <summary>
        /// Capability that enables a user to view reports (not execute reports - those are based off other capabilities).
        /// </summary>

        public const string CAN_VIEW_REPORTS = "System.Capability.CanViewReports";



        /// <summary>
        /// List of all 4 sync capabilities.
        /// </summary>

        public static readonly IList<string> AllSyncCapabilities = new List<string>()
        {
            CapabilityConstants.CAN_CREATE_SYNC_PACKAGES_ON_MAIN, 
            CapabilityConstants.CAN_APPLY_SYNC_PACKAGES_ON_MAIN, 
            CapabilityConstants.CAN_CREATE_SYNC_PACKAGES_ON_RTA,
            CapabilityConstants.CAN_APPLY_SYNC_PACKAGES_ON_RTA
        };

        #region CPMS User

        // ---------------------------- Supervisor

        /// <summary>
        /// Capability needed to manage supervisors.
        /// </summary>

        public const string CAN_MANAGE_SUPERVISOR = "System.Capability.CanManageSupervisor";

        // ---------------------------- Mentor

        /// <summary>
        /// Capability needed to manage mentors.
        /// </summary>

        public const string CAN_MANAGE_MENTOR = "System.Capability.CanManageMentor";

        #endregion

        #region CPMS Competency

        // ---------------------------- Collection

        /// <summary>
        /// Capability needed to create new CompetencyTemplateCollections.
        /// </summary>

        public const string CAN_CREATE_COMPETENCYTEMPLATECOLLECTION = "System.Capability.CanCreateCompetencyTemplateCollection";

        /// <summary>
        /// Capability needed to delete CompetencyTemplateCollections.
        /// </summary>

        public const string CAN_DELETE_COMPETENCYTEMPLATECOLLECTION = "System.Capability.CanDeleteCompetencyTemplateCollection";

        /// <summary>
        /// Capability needed to edit an CompetencyTemplateCollection.
        /// </summary>

        public const string CAN_EDIT_COMPETENCYTEMPLATECOLLECTION = "System.Capability.CanEditCompetencyTemplateCollection";

        /// <summary>
        /// Capability needed to view an CompetencyTemplateCollection.
        /// </summary>

        public const string CAN_VIEW_COMPETENCYTEMPLATECOLLECTION = "System.Capability.CanViewCompetencyTemplateCollection";

        // ---------------------------- Category

        /// <summary>
        /// Capability needed to create new CompetencyCategorys.
        /// </summary>

        public const string CAN_CREATE_COMPETENCYCATEGORY = "System.Capability.CanCreateCompetencyCategory";

        /// <summary>
        /// Capability needed to delete CompetencyCategorys.
        /// </summary>

        public const string CAN_DELETE_COMPETENCYCATEGORY = "System.Capability.CanDeleteCompetencyCategory";

        /// <summary>
        /// Capability needed to edit an CompetencyCategory.
        /// </summary>

        public const string CAN_EDIT_COMPETENCYCATEGORY = "System.Capability.CanEditCompetencyCategory";

        /// <summary>
        /// Capability needed to view an CompetencyCategory.
        /// </summary>

        public const string CAN_VIEW_COMPETENCYCATEGORY = "System.Capability.CanViewCompetencyCategory";

        // ---------------------------- Template

        /// <summary>
        /// Capability needed to create new CompetencyTemplates.
        /// </summary>

        public const string CAN_CREATE_COMPETENCYTEMPLATE = "System.Capability.CanCreateCompetencyTemplate";

        /// <summary>
        /// Capability needed to delete CompetencyTemplates.
        /// </summary>

        public const string CAN_DELETE_COMPETENCYTEMPLATE = "System.Capability.CanDeleteCompetencyTemplate";

        /// <summary>
        /// Capability needed to edit an CompetencyTemplate.
        /// </summary>

        public const string CAN_EDIT_COMPETENCYTEMPLATE = "System.Capability.CanEditCompetencyTemplate";

        /// <summary>
        /// Capability needed to view an CompetencyTemplate.
        /// </summary>

        public const string CAN_VIEW_COMPETENCYTEMPLATE = "System.Capability.CanViewCompetencyTemplate";

        // ---------------------------- Instance

        /// <summary>
        /// Capability needed to create new CompetencyInstances.
        /// </summary>

        public const string CAN_ASSIGN_COMPETENCYTEMPLATE = "System.Capability.CanAssignCompetencyTemplate";

        /// <summary>
        /// Capability needed to delete CompetencyInstances.
        /// </summary>

        public const string CAN_DELETE_COMPETENCYINSTANCE = "System.Capability.CanDeleteCompetencyInstance";

        /// <summary>
        /// Capability needed to edit an CompetencyInstance.
        /// </summary>

        public const string CAN_EDIT_COMPETENCYINSTANCE = "System.Capability.CanEditCompetencyInstance";

        /// <summary>
        /// Capability needed to view an CompetencyInstance.
        /// </summary>

        public const string CAN_VIEW_COMPETENCYINSTANCE = "System.Capability.CanViewCompetencyInstance";

        /// <summary>
        /// Capability needed to change CompetencyInstance State.
        /// </summary>

        public const string CAN_CHANGE_COMPETENCYSTATE = "System.Capability.CanChangeCompetencyState";

        #endregion

        #region CPMS Training

        // ---------------------------- Collection

        /// <summary>
        /// Capability needed to create new TrainingTemplateCollections.
        /// </summary>

        public const string CAN_CREATE_TRAININGTEMPLATECOLLECTION = "System.Capability.CanCreateTrainingTemplateCollection";

        /// <summary>
        /// Capability needed to delete TrainingTemplateCollections.
        /// </summary>

        public const string CAN_DELETE_TRAININGTEMPLATECOLLECTION = "System.Capability.CanDeleteTrainingTemplateCollection";

        /// <summary>
        /// Capability needed to edit an TrainingTemplateCollection.
        /// </summary>

        public const string CAN_EDIT_TRAININGTEMPLATECOLLECTION = "System.Capability.CanEditTrainingTemplateCollection";

        /// <summary>
        /// Capability needed to view an TrainingTemplateCollection.
        /// </summary>

        public const string CAN_VIEW_TRAININGTEMPLATECOLLECTION = "System.Capability.CanViewTrainingTemplateCollection";

        // ---------------------------- Category

        /// <summary>
        /// Capability needed to create new TrainingCategorys.
        /// </summary>

        public const string CAN_CREATE_TRAININGCATEGORY = "System.Capability.CanCreateTrainingCategory";

        /// <summary>
        /// Capability needed to delete TrainingCategorys.
        /// </summary>

        public const string CAN_DELETE_TRAININGCATEGORY = "System.Capability.CanDeleteTrainingCategory";

        /// <summary>
        /// Capability needed to edit an TrainingCategory.
        /// </summary>

        public const string CAN_EDIT_TRAININGCATEGORY = "System.Capability.CanEditTrainingCategory";

        /// <summary>
        /// Capability needed to view an TrainingCategory.
        /// </summary>

        public const string CAN_VIEW_TRAININGCATEGORY = "System.Capability.CanViewTrainingCategory";

        // ---------------------------- Template

        /// <summary>
        /// Capability needed to create new TrainingTemplates.
        /// </summary>

        public const string CAN_CREATE_TRAININGTEMPLATE = "System.Capability.CanCreateTrainingTemplate";

        /// <summary>
        /// Capability needed to bind a TrainingTemplate to a CompetencyTemplate
        /// </summary>

        public const string CAN_BIND_TRAININGTEMPLATETOCOMPETENCYTEMPLATE = "System.Capability.CanBindTrainingTemplateToCompetencyTemplate";

        /// <summary>
        /// Capability needed to delete TrainingTemplates.
        /// </summary>

        public const string CAN_DELETE_TRAININGTEMPLATE = "System.Capability.CanDeleteTrainingTemplate";

        /// <summary>
        /// Capability needed to edit an TrainingTemplate.
        /// </summary>

        public const string CAN_EDIT_TRAININGTEMPLATE = "System.Capability.CanEditTrainingTemplate";

        /// <summary>
        /// Capability needed to view an TrainingTemplate.
        /// </summary>

        public const string CAN_VIEW_TRAININGTEMPLATE = "System.Capability.CanViewTrainingTemplate";

        // ---------------------------- Instance

        /// <summary>
        /// Capability needed to create new TrainingInstances.
        /// </summary>

        public const string CAN_ASSIGN_TRAININGTEMPLATE = "System.Capability.CanAssignTrainingTemplate";

        /// <summary>
        /// Capability needed to delete TrainingInstances.
        /// </summary>

        public const string CAN_DELETE_TRAININGINSTANCE = "System.Capability.CanDeleteTrainingInstance";

        /// <summary>
        /// Capability needed to edit an TrainingInstance.
        /// </summary>

        public const string CAN_EDIT_TRAININGINSTANCE = "System.Capability.CanEditTrainingInstance";

        /// <summary>
        /// Capability needed to view an TrainingInstance.
        /// </summary>

        public const string CAN_VIEW_TRAININGINSTANCE = "System.Capability.CanViewTrainingInstance";

        #endregion

        /// <summary>
        /// Capability needed to manage supervisors.
        /// </summary>

        public const string CAN_MANAGE_TAGS = "System.Capability.CanManageTags";
        public const string CAN_VIEW_TAGS = "System.Capability.CanViewTags";


        /// <summary>
        /// Get all pre-defined capability constants as a list.
        /// </summary>
        /// <returns>All capability names.</returns>

        public static IEnumerable<string> All()
        {
            IList<string> c = new List<string>();

            var fieldInfos = typeof(CapabilityConstants).GetFields().Where(f => f.FieldType == typeof(string)).ToList();

            foreach (FieldInfo fi in fieldInfos)
            {
                string capName = fi.GetRawConstantValue() as string; // Get value of the field...
                c.Add(capName);
            }

            // Return the enumerable...
            return c.AsEnumerable<string>();
        }
    }

    public class CapabilityToRTAOverrideMapping
    {
        /// <summary>
        /// Capability to set a mapping on an RTA.
        /// </summary>

        public string Capability { get; set; }

        /// <summary>
        /// Flag whether to override the capability on an RTA.
        /// <c>true</c> to override actual value with <see cref="RTAOverrideValue"/>.
        /// <c>false</c> to take calculated value (<see cref="RTAOverrideValue"/> is ignored).
        /// </summary>

        public bool RTAOverride { get; set; }

        /// <summary>
        /// Value to use for the <see cref="Capability"/> if <see cref="RTAOverride"/> == <c>true</c>.
        /// This is ignored if <see cref="RTAOverride"/> == <c>false</c>.
        /// </summary>

        public bool RTAOverrideValue { get; set; }
    }

    public static class CapabilityRTAOverrides
    {
        public static List<CapabilityToRTAOverrideMapping> mappings = new List<CapabilityToRTAOverrideMapping>()
        {
            new CapabilityToRTAOverrideMapping() { Capability = CapabilityConstants.CAN_ADD_INSTRUCTORS, RTAOverride = true, RTAOverrideValue = false},
            new CapabilityToRTAOverrideMapping() { Capability = CapabilityConstants.CAN_AWARD_CERTIFICATE_AUDITS,RTAOverride=true,RTAOverrideValue=false},
            new CapabilityToRTAOverrideMapping() { Capability = CapabilityConstants.CAN_CREATE_EXAM_COLLECTIONS,RTAOverride=true,RTAOverrideValue=false},
            new CapabilityToRTAOverrideMapping() { Capability = CapabilityConstants.CAN_CREATE_FILE_COLLECTIONS,RTAOverride=true,RTAOverrideValue=false},
            new CapabilityToRTAOverrideMapping() { Capability = CapabilityConstants.CAN_CREATE_OFFERINGS,RTAOverride=true,RTAOverrideValue=false},
            new CapabilityToRTAOverrideMapping() { Capability = CapabilityConstants.CAN_CREATE_QUESTION_COLLECTIONS,RTAOverride=true,RTAOverrideValue=false},
            new CapabilityToRTAOverrideMapping() { Capability = CapabilityConstants.CAN_CREATE_USERS,RTAOverride=true,RTAOverrideValue=false},
            new CapabilityToRTAOverrideMapping() { Capability = CapabilityConstants.CAN_DE_REGISTER_STUDENTS,RTAOverride=true,RTAOverrideValue=false},
            new CapabilityToRTAOverrideMapping() { Capability = CapabilityConstants.CAN_DELETE_EXAM_COLLECTIONS,RTAOverride=true,RTAOverrideValue=false},
            new CapabilityToRTAOverrideMapping() { Capability = CapabilityConstants.CAN_DELETE_FILE_COLLECTIONS,RTAOverride=true,RTAOverrideValue=false},
            new CapabilityToRTAOverrideMapping() { Capability = CapabilityConstants.CAN_DELETE_INSTRUCTORS,RTAOverride=true,RTAOverrideValue=false},
            new CapabilityToRTAOverrideMapping() { Capability = CapabilityConstants.CAN_DELETE_OFFERINGS,RTAOverride=true,RTAOverrideValue=false},
            new CapabilityToRTAOverrideMapping() { Capability = CapabilityConstants.CAN_DELETE_QUESTION_COLLECTIONS,RTAOverride=true,RTAOverrideValue=false},
            new CapabilityToRTAOverrideMapping() { Capability = CapabilityConstants.CAN_DELETE_USERS,RTAOverride=true,RTAOverrideValue=false},
            new CapabilityToRTAOverrideMapping() { Capability = CapabilityConstants.CAN_DISPLAY_AS_LINK,RTAOverride=false,RTAOverrideValue=false},
            new CapabilityToRTAOverrideMapping() { Capability = CapabilityConstants.CAN_EDIT_OFFERINGS,RTAOverride=true,RTAOverrideValue=false},
            new CapabilityToRTAOverrideMapping() { Capability = CapabilityConstants.CAN_EDIT_USERS,RTAOverride=false,RTAOverrideValue=false},
            new CapabilityToRTAOverrideMapping() { Capability = CapabilityConstants.CAN_VIEW_EXAM_INSTANCE_AUDITS,RTAOverride=false,RTAOverrideValue=false},
            new CapabilityToRTAOverrideMapping() { Capability = CapabilityConstants.CAN_MANAGE_CATEGORIES,RTAOverride=true,RTAOverrideValue=false},
            new CapabilityToRTAOverrideMapping() { Capability = CapabilityConstants.CAN_MANAGE_EXAMS,RTAOverride=true,RTAOverrideValue=false},
            new CapabilityToRTAOverrideMapping() { Capability = CapabilityConstants.CAN_MANAGE_FILES,RTAOverride=true,RTAOverrideValue=false},
            new CapabilityToRTAOverrideMapping() { Capability = CapabilityConstants.CAN_MANAGE_LINK_ATTRIBUTES,RTAOverride=true,RTAOverrideValue=false},
            new CapabilityToRTAOverrideMapping() { Capability = CapabilityConstants.CAN_MANAGE_LINKS,RTAOverride=true,RTAOverrideValue=false},
            new CapabilityToRTAOverrideMapping() { Capability = CapabilityConstants.CAN_MANAGE_PAGE_ATTRIBUTES,RTAOverride=true,RTAOverrideValue=false},
            new CapabilityToRTAOverrideMapping() { Capability = CapabilityConstants.CAN_MANAGE_PAGE_DEFAULTS,RTAOverride=true,RTAOverrideValue=false},
            new CapabilityToRTAOverrideMapping() { Capability = CapabilityConstants.CAN_MANAGE_QUESTIONS,RTAOverride=true,RTAOverrideValue=false},
            new CapabilityToRTAOverrideMapping() { Capability = CapabilityConstants.CAN_OVERRIDE_EXAM,RTAOverride=true,RTAOverrideValue=false},
            new CapabilityToRTAOverrideMapping() { Capability = CapabilityConstants.CAN_PROCTOR_EXAM,RTAOverride=false,RTAOverrideValue=false},
            new CapabilityToRTAOverrideMapping() { Capability = CapabilityConstants.CAN_REGISTER_STUDENTS,RTAOverride=true,RTAOverrideValue=false},
            new CapabilityToRTAOverrideMapping() { Capability = CapabilityConstants.CAN_REVOKE_CERTIFICATE_AUDITS, RTAOverride = true, RTAOverrideValue = false },
            new CapabilityToRTAOverrideMapping() { Capability = CapabilityConstants.CAN_TAKE_EXAM, RTAOverride = false, RTAOverrideValue = false },
            new CapabilityToRTAOverrideMapping() { Capability = CapabilityConstants.CAN_EVALUATE_STUDENT, RTAOverride = false, RTAOverrideValue = false },
            new CapabilityToRTAOverrideMapping() { Capability = CapabilityConstants.CAN_VIEW, RTAOverride = false, RTAOverrideValue = false },
            new CapabilityToRTAOverrideMapping() { Capability = CapabilityConstants.CAN_VIEW_ATTRIBUTES,RTAOverride=false,RTAOverrideValue=false},
            new CapabilityToRTAOverrideMapping() { Capability = CapabilityConstants.CAN_VIEW_CERTIFICATE_AUDITS,RTAOverride=false,RTAOverrideValue=false},
            new CapabilityToRTAOverrideMapping() { Capability = CapabilityConstants.CAN_VIEW_EXAM,RTAOverride=false,RTAOverrideValue=false},
            new CapabilityToRTAOverrideMapping() { Capability = CapabilityConstants.CAN_VIEW_EXAM_REPORT,RTAOverride=false,RTAOverrideValue=false},
            new CapabilityToRTAOverrideMapping() { Capability = CapabilityConstants.CAN_VIEW_FILES,RTAOverride=true,RTAOverrideValue=false},
            new CapabilityToRTAOverrideMapping() { Capability = CapabilityConstants.CAN_VIEW_OFFERINGS,RTAOverride=false,RTAOverrideValue=false},
            new CapabilityToRTAOverrideMapping() { Capability = CapabilityConstants.CAN_VIEW_QUESTIONS,RTAOverride=false,RTAOverrideValue=false},
            new CapabilityToRTAOverrideMapping() { Capability = CapabilityConstants.CAN_VIEW_REGISTRATIONS,RTAOverride=false,RTAOverrideValue=false},
            new CapabilityToRTAOverrideMapping() { Capability = CapabilityConstants.CAN_VIEW_SESSION_AUDITS,RTAOverride=false,RTAOverrideValue=false},
            new CapabilityToRTAOverrideMapping() { Capability = CapabilityConstants.CAN_VIEW_TRACKING_AUDITS,RTAOverride=false,RTAOverrideValue=false},
            new CapabilityToRTAOverrideMapping() { Capability = CapabilityConstants.CAN_VIEW_USERS,RTAOverride=false,RTAOverrideValue=false},
            new CapabilityToRTAOverrideMapping() { Capability = CapabilityConstants.CAN_VIEW_RTA,RTAOverride=false,RTAOverrideValue=false},
            new CapabilityToRTAOverrideMapping() { Capability = CapabilityConstants.CAN_CREATE_RTA,RTAOverride=true,RTAOverrideValue=false},
            new CapabilityToRTAOverrideMapping() { Capability = CapabilityConstants.CAN_DELETE_RTA,RTAOverride=true,RTAOverrideValue=false},
            new CapabilityToRTAOverrideMapping() { Capability = CapabilityConstants.CAN_EDIT_RTA,RTAOverride=false,RTAOverrideValue=false},
            new CapabilityToRTAOverrideMapping() { Capability = CapabilityConstants.CAN_CREATE_SYNC_PACKAGES_ON_MAIN,RTAOverride=true,RTAOverrideValue=false},
            new CapabilityToRTAOverrideMapping() { Capability = CapabilityConstants.CAN_CREATE_SYNC_PACKAGES_ON_RTA,RTAOverride=false,RTAOverrideValue=false},
            new CapabilityToRTAOverrideMapping() { Capability = CapabilityConstants.CAN_APPLY_SYNC_PACKAGES_ON_MAIN,RTAOverride=true,RTAOverrideValue=false},
            new CapabilityToRTAOverrideMapping() { Capability = CapabilityConstants.CAN_APPLY_SYNC_PACKAGES_ON_RTA,RTAOverride=false,RTAOverrideValue=false},
            new CapabilityToRTAOverrideMapping() { Capability = CapabilityConstants.CAN_VIEW_CERTIFICATES, RTAOverride = false, RTAOverrideValue= false },
            new CapabilityToRTAOverrideMapping() { Capability = CapabilityConstants.CAN_CREATE_CERTIFICATES, RTAOverride = true, RTAOverrideValue= false },
            new CapabilityToRTAOverrideMapping() { Capability = CapabilityConstants.CAN_EDIT_CERTIFICATES, RTAOverride = true, RTAOverrideValue= false },
            new CapabilityToRTAOverrideMapping() { Capability = CapabilityConstants.CAN_DELETE_CERTIFICATES, RTAOverride = true, RTAOverrideValue= false },
            new CapabilityToRTAOverrideMapping() { Capability = CapabilityConstants.CAN_VIEW_COURSE_OFFERING_AUDITS,RTAOverride=false,RTAOverrideValue=false},
            new CapabilityToRTAOverrideMapping() { Capability = CapabilityConstants.CAN_VIEW_REGISTRATION_AUDITS,RTAOverride=false,RTAOverrideValue=false},
            new CapabilityToRTAOverrideMapping() { Capability = CapabilityConstants.CAN_VIEW_USER_AUDITS,RTAOverride=false,RTAOverrideValue=false},
            new CapabilityToRTAOverrideMapping() { Capability = CapabilityConstants.CAN_VIEW_ORG_PROFILE_FIELD_AUDITS,RTAOverride=false,RTAOverrideValue=false},
            new CapabilityToRTAOverrideMapping() { Capability = CapabilityConstants.CAN_VIEW_ORG_PROFILE_VALUE_AUDITS,RTAOverride=false,RTAOverrideValue=false},
            new CapabilityToRTAOverrideMapping() { Capability = CapabilityConstants.CAN_VIEW_REPORTS,RTAOverride=false,RTAOverrideValue=false}
        };
    }

    /// <summary>
    /// String constants for using capabilities and permissions.
    /// </summary>

    public static class CapabilityParts
    {
        /// <summary>
        /// Name space 'System.Capability' for normal capabilities.
        /// </summary>

        public static string CAPABILITY_NAME_SPACE = "System.Capability";
    }
}
