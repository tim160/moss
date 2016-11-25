using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace EC.Service.DTO
{
    /// <summary>
    /// DTO for holding the data of an exam performance report.
    /// </summary>
    
    [DataContract]
    public partial class ExamPerformanceGraphData
    {
        /// <summary>
        /// Start date for graph
        /// </summary>
        [DataMember]
        public DateTime StartDate { get; private set; }

        [DataMember]
        public DateTime StartDateSysTime { get; private set; }

        /// <summary>
        /// End date for graph
        /// </summary>
        [DataMember]
        public DateTime EndDate { get; private set; }

        [DataMember]
        public DateTime EndDateSysTime { get; private set; }

        /// <summary>
        /// Title for data, this will be the name given to the line on the graph.
        /// </summary>
        [DataMember]
        public string DataTitle { get; set; }

        /// <summary>
        /// Data points that correspond to time intervals
        /// </summary>
        [DataMember]
        public List<int> Data { get; set; }

        /// <summary>
        /// Time intervals that correspond to data
        /// </summary>
        [DataMember]
        public List<string> TimeIntervals { get; set; }

        /// <summary>
        /// Title for time intervals. IE:Hours, Days, Months, Years ...
        /// </summary>
        [DataMember]
        public string TimeIntervalTitle { get; set; }

        /// <summary>
        /// The current grouping of data: IE: Daily, Weekly, Monthly, Yearly
        /// </summary>
        [DataMember]
        public string DateGrouping { get; set; }
    }

    /// <summary>
    /// DTO for holding the data of a the answer graph for a single question in the question performance report.
    /// </summary>
    
    [DataContract]
    public partial class QuestionPerformanceGraphData
    {
        /// <summary>
        /// Start date for graph
        /// </summary>
        [DataMember]
        public DateTime StartDate { get; private set; }

        [DataMember]
        public DateTime StartDateSysTime { get; private set; }

        /// <summary>
        /// End date for graph
        /// </summary>
        [DataMember]
        public DateTime EndDate { get; private set; }

        [DataMember]
        public DateTime EndDateSysTime { get; private set; }

        /// <summary>
        /// Title for data, this will be the name given to the line on the graph.
        /// </summary>
        [DataMember]
        public string CorrectDataTitle { get; set; }

        /// <summary>
        /// Data points that correspond to time intervals
        /// </summary>
        [DataMember]
        public List<int> CorrectData { get; set; }

        /// <summary>
        /// Time intervals that correspond to data
        /// </summary>
        [DataMember]
        public List<string> TimeIntervals { get; set; }

        /// <summary>
        /// Title for time intervals. IE:Hours, Days, Months, Years ...
        /// </summary>
        [DataMember]
        public string TimeIntervalTitle { get; set; }

        /// <summary>
        /// The current grouping of data: IE: Daily, Weekly, Monthly, Yearly
        /// </summary>
        [DataMember]
        public string DateGrouping { get; set; }
    }

    /// <summary>
    /// DTO for holding the overall data for the Question Performance Report
    /// </summary>
    
    [DataContract]
    public class QuestionPerformanceInfo
    {
        /// <summary>
        /// Version Id of Question. Questions can change over time, and this captures that.
        /// It is the primary key of the QuestionPerformanceInfo.
        /// </summary>
        [DataMember]
        public string QuestionVersionId { get; set; }

        /// <summary>
        /// Id of Question. This may occur in different QuestionPerformanceInfos, under
        /// different QuestionVersionIds.
        /// </summary>
        [DataMember]
        public Guid QuestionId { get; set; }

        /// <summary>
        /// Number of time question has been used in exams
        /// </summary>
        [DataMember]
        public int TimesUsed { get; set; }

        /// <summary>
        /// Number of times question has been answered correctly
        /// </summary>
        [DataMember]
        public int AnsweredCorrectly { get; set; }

        /// <summary>
        /// Number of times question was answered incorrectly
        /// </summary>
        [DataMember]
        public int AnsweredIncorrectly { get; set; }

        /// <summary>
        /// Number of time question was used but not answered
        /// </summary>
        [DataMember]
        public int Unanswered { get; set; }

        /// <summary>
        /// Cache for question answers
        /// </summary>
        [DataMember]
        public List<QuestionAnswerPerformanceInfo> Answers { get; set; }

        /// <summary>
        /// Question Text.
        /// </summary>
        [DataMember]
        public string QuestionText { get; set; }


        /// <summary>
        /// Number of times question was overridden correct
        /// </summary>
        [DataMember]
        public int OverriddenCorrect { get; set; }

        /// <summary>
        /// Number of times question was overridden incorrect
        /// </summary>
        [DataMember]
        public int OverriddenIncorrect { get; set; }

        /// <summary>
        /// Number of times question has been excluded
        /// </summary>
        [DataMember]
        public int OverriddenExcluded { get; set; }

        /// <summary>
        /// CategroyId question is in 
        /// </summary>
        [DataMember]
        public Guid CategoryId { get; set; }

        /// <summary>
        /// Questions short ID
        /// </summary>
        [DataMember]
        public int ShortId { get; set; }

        /// <summary>
        /// Percentage of time question has been answered correctly
        /// </summary>
        [DataMember]
        public int AnsweredCorrectPercent { get; set; }

        /// <summary>
        /// Percent of time question has been answered incorrectly
        /// </summary>
        [DataMember]
        public int AnsweredIncorrectPercent { get; set; }

        /// <summary>
        /// Percent of time question was not answered
        /// </summary>
        [DataMember]
        public int AnsweredUnansweredPercent { get; set; }

        /// <summary>
        /// The total number of times this question has been overridden
        /// </summary>
        [DataMember]
        public int TotalTimesOverridden { get; set; }

        /// <summary>
        /// True if question has enough data for graph
        /// </summary>
        [DataMember]
        public bool HasValidGraph { get; set; }
    }

    [DataContract]
    public class QuestionAnswerPerformanceInfo
    {
        /// <summary>
        /// Unique id for QuestionAnswerPerformanceInfo entry
        /// </summary>
        [DataMember]
        public string AnswerText { get; set; }

        /// <summary>
        /// Id of the answer cache is for
        /// </summary>
        [DataMember]
        public Guid AnswerId { get; set; }

        /// <summary>
        /// Total number of times answer has been selected
        /// </summary>
        [DataMember]
        public int TotalTimesSelected { get; set; }

        /// <summary>
        /// True if answer is currently the correct answer
        /// </summary>
        [DataMember]
        public bool IsCorrect { get; set; }
    }
}
