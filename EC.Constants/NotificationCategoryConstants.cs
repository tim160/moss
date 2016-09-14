using System;
using System.Runtime.Serialization;

namespace EC.Constants
{
    /// <summary>
    /// Categories of notifications.
    /// </summary>

    [DataContract]
    public enum NotificationCategoriesEnum
    {
        [EnumMember]
        General = 0,

        [EnumMember]
        Exam = 1,  //TODO: rename to Assessment  (MIGRATION: rename notification rule PredicateExpressions)

        [EnumMember]
        Content = 2,

        [EnumMember]
        Discussion = 4,

        [EnumMember]
        User = 8,

        [EnumMember]
        Feedback = 16,

        [EnumMember]
        Registration = 32,

        [EnumMember]
        AgendaBuilder = 64,

        [EnumMember]
        CSMART = 128,

        [EnumMember]
        PassagePlanner = 256
    }
}
