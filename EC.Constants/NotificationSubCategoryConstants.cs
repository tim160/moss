using System;
using System.Runtime.Serialization;

namespace EC.Constants
{
    /// <summary>
    /// Sub-categories for notifications (set to 'None' if there is no sub-category).
    /// </summary>

    [DataContract]
    public enum NotificationSubCategoriesEnum
    {
        /// <summary>
        /// If there is no sub-category.
        /// </summary>

        [EnumMember]
        None = 0,

        // ================================== Category: Exam ======================================
        
        /// <summary>
        /// If an exam/evaluation has started.
        /// </summary>

        [EnumMember]
        ExamStarted = 1,

        /// <summary>
        /// If an exam/evaluation has been completed.
        /// </summary>

        [EnumMember]
        ExamCompleted = 2,

        /// <summary>
        /// Student evaluation has started.
        /// </summary>
        
        [EnumMember]
        StudentEvaluationStarted = 18,

        /// <summary>
        /// Student evaluation has been completed.
        /// </summary>

        [EnumMember]
        StudentEvaluationCompleted = 19,

        /// <summary>
        /// If an exam needs attention (e.g. an exam has been generated where there were too few questions in at least one category).
        /// This sub-category 
        /// </summary>

        [EnumMember]
        ExamNeedsMaintenance = 3,   // TODO: Rename to AssessmentNeedsMaintenance (MIGRATION: rename notification rule PredicateExpressions)

        // ================================== Category: Content ===================================

        [EnumMember]
        PageEdited = 4,

        [EnumMember]
        PageDeleted = 5,

        [EnumMember]
        QuestionEdited = 6,

        // ================================== Category: User ======================================

        [EnumMember]
        ForgotPassword = 7,

        // ================================== Category: Registration ===============================

        [EnumMember]
        StudentRegistration = 9,

        [EnumMember]
        StudentDeRegistration = 17,
        
        // ================================== Category: CSMART ===============================

        [EnumMember]
        ParticipantEmail = 10,

        [EnumMember]
        LineAdminEmail = 11,

        /// <summary>
        /// this is the first notification to be sent before course starts. for example default interval is 6 weeks before the event.
        /// </summary>
        [EnumMember]
        PreCourseNotification1 = 12,

        /// <summary>
        /// this is the second notification to be sent before course starts. for example default interval is 4 weeks before the event.
        /// </summary>
        [EnumMember]
        PreCourseNotification2 = 13,

        /// <summary>
        /// this is the third notification to be sent before course starts. for example default interval is 2 weeks before the event.
        /// </summary>
        [EnumMember]
        PreCourseNotification3 = 14,

        /// <summary>
        /// this is the fourth notification to be sent before course starts. for example default interval is 1 weeks before the event.
        /// </summary>
        [EnumMember]
        PreCourseNotification4 = 15,

        [EnumMember]
        EventDeleted = 16,

        // ================================== Category: AgendaBuilder ===============================

        [EnumMember]
        FeedbackSubmission = 20,

        [EnumMember]
        NewAgenda = 21,

        [EnumMember]
        UseAgenda = 22,

        [EnumMember]
        ShareAgenda = 23
    }
}
