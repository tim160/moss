using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EC.Constants
{
    /// <summary>
    /// Constants for parameter keys for notification templates.
    /// </summary>

    public static class TemplateParameterKeyConstants
    {
        /// <summary>
        /// Time zone name the notification is sent. 
        /// All the DateTime objects should be already converted into this time zone.
        /// </summary>

        public const string TIME_ZONE = "TimeZone";

        /// <summary>
        /// Token Id (Guid) for the forgotten password.
        /// </summary>

        public const string FORGOT_PASSWORD_TOKEN_ID = "ForgotPasswordTokenId";

        /// <summary>
        /// Full url to reset the password (includes the token Id and the url from where the request came from).
        /// </summary>

        public const string FORGOT_PASSWORD_FULL_URL = "ForgotPasswordFullUrl";
        
        /// <summary>
        /// Path for which the feedback is created.
        /// </summary>

        public const string FEEDBACK_PATH = "FeedbackPath";

        /// <summary>
        /// Customer can put any contact information into the feedback form.
        /// </summary>

        public const string FEEDBACK_CONTACT_INFORMATION = "FeedbackContactInformation";

        /// <summary>
        /// Feedback text.
        /// </summary>
        public const string FEEDBACK_TEXT = "FeedbackText";

        /// <summary>
        /// Link url name for which the feedback is created.
        /// </summary>
        public const string FEEDBACK_LINK_URL_NAME = "FeedbackLinkUrlName";

        /// <summary>
        /// Feedback email address.
        /// </summary>
        public const string FEEDBACK_EMAIL_ADDRESS = "FeedbackEmailAddress";

        /// <summary>
        /// Category names separated with ','
        /// </summary>

        public const string CATEGORY_NAMES = "CategoryNames";
        
        /// <summary>
        /// Path to a NavPage/TOC or any other link (e.g. exam).
        /// </summary>

        public const string PATH = "Path";

        /// <summary>
        /// Current user Id - empty or null if no user is set.
        /// </summary>

        public const string CURRENT_USER_ID = "CurrentUserId";

        /// <summary>
        /// Email address.
        /// </summary>

        public const string RECIPIENT_EMAIL_ADDRESS = "RecipientEmailAddress";

        public const string COURSE_OFFERING_ID = "CourseOfferingId";
        public const string COURSE_OFFERING_SHORT_ID = "CourseOfferingShortId";
        public const string COURSE_DISPLAY_NAME = "CourseDisplayName";
        public const string COURSE_START_DATE = "CourseStartDate";
        public const string COURSE_END_DATE = "CourseEndDate";

        /// <summary>
        /// Contact email addresses of the people (instructors or feedback email address), 
        /// separated by ','.
        /// </summary>
                
        public const string COURSE_CONTACT_EMAIL_ADDRESSES = "CourseContactEmailAddresses";
       
        /// <summary>
        /// Full url to a course.
        /// </summary>
       
        public const string COURSE_URL = "CourseUrl";

        #region Initiator...

        /// <summary>
        /// First name of the initiator (who created this notification)
        /// </summary>

        public const string INITIATOR_FIRST_NAME = "InitiatorFirstName";

        /// <summary>
        /// Last name of the initiator (who created this notification).
        /// </summary>

        public const string INITIATOR_LAST_NAME = "InitiatorLastName";

        /// <summary>
        /// User name of the initiator (who created this notification).
        /// </summary>
        /// <remarks>
        /// This can be used if first and last name haven't been set.
        /// </remarks>

        public const string INITIATOR_USER_NAME = "InitiatorUserName";

        #endregion Initiator.

        #region Student...

        /// <summary>
        /// First name of the student the notification data is about.
        /// </summary>
        
        public const string STUDENT_FIRST_NAME = "StudentFirstName";
        
        /// <summary>
        /// Last name of the student the notification data is about.
        /// </summary>
        
        public const string STUDENT_LAST_NAME = "StudentLastName";

        /// <summary>
        /// User name of the student the notification data is about.
        /// </summary>
        /// <remarks>
        /// This can be used if first and last name haven't been set.
        /// </remarks>

        public const string STUDENT_USER_NAME = "StudentUserName";

        /// <summary>
        /// Email address of the student the notification data is about.
        /// </summary>

        public const string STUDENT_EMAIL_ADDRESS = "StudentEmailAddress";

        #endregion Student.

        #region ExamNotification...

        /// <summary>                
        /// Name of an exam (display name)
        /// </summary>
        
        public const string EXAM_DISPLAY_NAME = "ExamDisplayName";
        public const string EXAM_URL_NAME = "ExamUrlName";
        public const string EXAM_STATE = "ExamState ";
        public const string EXAM_AUDIT_ID = "ExamAuditId";
        public const string EXAM_TYPE = "ExamType";
        public const string EXAM_ID = "ExamId" ;
        public const string EXAM_IS_OFFLINE_EXAM = "IsOfflineExam";
        public const string EXAM_INSTANCE_ID = "ExamInstanceId";
        public const string EXAM_USER_TYPE = "UsersType" ;
        public const string EXAM_NUMBER_OF_QUESTIONS = "NumberOfQuestions";
        public const string EXAM_CORRECT_QUESTIONS = "CorrectQuestions";
        public const string EXAM_QUESTIONS_ANSWERED ="QuestionsAnswered" ;
        public const string EXAM_INCORRECT_QUESTIONS = "IncorrectQuestions";
        public const string EXAM_TIME_TAKEN ="TimeTaken" ;
        public const string EXAM_DATE_TAKEN = "DateTaken";
        public const string EXAM_SCORE = "Score" ;
        public const string EXAM_SCORE_IN_PERCENT = "ScoreInPercent";

        #endregion ExamNotification.

        #region AgendaBuilder

        public const string AGENDA_EMAIL_TITLE = "AgendaEmailTitle";
        public const string AGENDA_EMAIL_SUBJECT = "AgendaEmailSubject";
        public const string AGENDA_CREATOR = "AgendaCreator";
        public const string AGENDA_SHARER = "AgendaSharer";
        public const string AGENDA_TITLE = "AgendaTitle";
        public const string AGENDA_URL = "AgendaUrl";
        public const string AGENDA_DASHBOARD_URL = "AgendaDashboardUrl";
        public const string AGENDA_DESCRIPTION = "AgendaDescription";
        public const string AGENDA_DEPARTMENT = "AgendaDepartment";
        public const string AGENDA_POSITION = "AgendaPosition";
        public const string AGENDA_TYPE = "AgendaType";
        public const string AGENDA_VESSEL = "AgendaVessel";
        public const string AGENDA_ROUTE = "AgendaRoute";
        public const string AGENDA_TERMINAL = "AgendaTerminal";
        public const string AGENDA_DATE_OF_TRAINING = "AgendaDateOfTraining";
        public const string AGENDA_DAYS_OF_TRAINING = "AgendaDaysOfTraining";
        public const string AGENDA_USE_EMPLOYEES = "AgendaUseEmployees";

        public const string AGENDA_FEEDBACK_CREATOR = "AgendaFeedbackCreator";
        public const string AGENDA_FEEDBACK_CREATOR_EMAIL = "AgendaFeedbackCreatorEmail";
        public const string AGENDA_FEEDBACK_CONTENT_ID = "AgendaFeedbackContentId";
        public const string AGENDA_FEEDBACK_REFERRING_PAGE = "AgendaFeedbackReferringPage";
        public const string AGENDA_FEEDBACK_REFERRING_PAGE_URL = "AgendaFeedbackReferringPageUrl";
        public const string AGENDA_FEEDBACK_CONTACT_INFO = "AgendaFeedbackContactInfo";
        public const string AGENDA_FEEDBACK_MESSAGE = "AgendaFeedbackMessage";

        #endregion
    }

    public static class DynamicUserTemplateParameterKeyConstants
    {
        /// <summary>
        /// Key for Dynamically filling in the User's First Name
        /// </summary>

        public const string FIRST_NAME = "FirstName";

        /// <summary>
        /// Key for Dynamically filling in the User's Last Name
        /// </summary>

        public const string LAST_NAME = "LastName";

        /// <summary>
        /// Get all Dynamically property names as a list.
        /// </summary>
        /// <returns>All channel names.</returns>

        public static IEnumerable<string> All()
        {
            IList<string> c = new List<string>();

            FieldInfo[] fieldInfos = typeof(DynamicUserTemplateParameterKeyConstants).GetFields();

            foreach (FieldInfo fi in fieldInfos)
            {
                string channelName = fi.GetRawConstantValue() as string; // Get value of the field...
                c.Add(channelName);
            }

            // Return the enumerable of channels...
            return c.AsEnumerable<string>();
        }
    }
}
