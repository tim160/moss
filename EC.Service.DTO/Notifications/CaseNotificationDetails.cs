using EC.Constants;
using System;
using System.Runtime.Serialization;

namespace EC.Service.DTO
{
    /// <summary>
    /// DTO for exam notification. 
    /// </summary>

    [DataContract]

    public class CaseNotificationDetails : NotificationDetails
    {
        [DataMember]
        public string UserName { get; set; }
        [DataMember]
        public string FirstName { get; set; }
        [DataMember]
        public string LastName { get; set; }
        [DataMember]
        public string EmailAddress { get; set; }


        [DataMember]
        public Guid ExamAuditId { get; set; }
        //////[DataMember]
       ///// public ExamTypeEnum ExamType { get; set; }
        [DataMember]
        public Guid ExamId { get; set; }
        [DataMember]
        public string ExamURLName { get; set; }
        [DataMember]
        public string ExamDisplayName { get; set; }

        [DataMember]
        public Guid OfferingId { get; set; }
        [DataMember]
        public string OfferingShortId { get; set; }
        [DataMember]
        public Guid ExamInstanceId { get; set; }
      /////  [DataMember]
      ///// public UserTypeEnum UsersType { get; set; }
        [DataMember]
        public int NumberOfQuestions { get; set; }
        [DataMember]
        public int CorrectQuestions { get; set; }
        [DataMember]
        public int QuestionsAnswered { get; set; }
        [DataMember]
        public int IncorrectQuestions { get; set; }
        [DataMember]
        public int TimeTaken { get; set; }
        [DataMember]
        public DateTime DateTaken { get; set; }
        [DataMember]
        public int Score { get; set; }
    }
}
