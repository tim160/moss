using System;
using System.Collections.Generic;
using System.Text;

namespace EC.Common.Util
{
    [Serializable]
    public class ActionResult
    {
        public List<ReturnProblem> ReturnProblems = new List<ReturnProblem>();
        public ReturnCode ReturnCode = ReturnCode.Success;
        public string ReturnMessage = string.Empty;
        public string ExceptionMessage = string.Empty;
        public object ReturnObject = null;

        public ActionResult()
        {
        }

        public ActionResult(ReturnCode returnCode, string returnMessage)
        {
            ReturnCode = returnCode;
            ReturnMessage = returnMessage;
        }

        public void Success(string returnMessage)
        {
            ReturnCode = ReturnCode.Success;
            ReturnMessage = returnMessage;
        }

        public void Fail(string returnMessage)
        {
            Fail(returnMessage, null);
        }

        public void Fail(string returnMessage, List<ReturnProblem> problems)
        {
            ReturnCode = ReturnCode.Fail;
            ReturnMessage = returnMessage;
            if (problems != null && problems.Count > 0)
                ReturnProblems.AddRange(problems);
        }

        public void AddProblem(Guid key, string problemMessage)
        {
            ReturnCode = ReturnCode.Success;
            ReturnMessage = problemMessage;
        }

        public void AddProblem(Guid key, string tag, string problemMessage)
        {
            ReturnProblems.Add(new ReturnProblem(key, tag, problemMessage));
        }

        public void AddProblem(long id, string tag, string problemMessage)
        {
            ReturnProblems.Add(new ReturnProblem(id, tag, problemMessage));
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder("[ActionResult]");
            builder.Append("\r\n");
            builder.Append("ReturnCode: ").Append(ReturnCode);
            builder.Append(", ReturnMessage: ").Append(ReturnMessage);
            if (ExceptionMessage != string.Empty)
                builder.Append(", ExceptionMessage: ").Append(ExceptionMessage);
            if (ReturnProblems.Count > 0)
            {
                builder.Append("\r\n");
                builder.Append("ReturnProblems: ");
                builder.Append("\r\n");
                foreach (ReturnProblem problem in ReturnProblems)
                {
                    builder.Append(problem.ToString());
                    builder.Append("\r\n");
                }
            }
            return builder.ToString();
        }
    }

    public enum ReasonCode
    {
        Unknown = -1,
        Success = 0,
        ObjectDoesNotExist = 1,
        Duplicate = 2,
        ObjectReferenced = 3,
        ObjectDisabled = 4,
        InvalidArea = 5,
        Failed = 99
    }

    public enum ReturnCode
    {
        Success = 0,
        Fail = 1,
        SuccessWithErrors = 2,
        InProgress = 3
    }

    public enum ReturnMessage
    {
        Success,
        Fail
    }

    [Serializable]
    public class ReturnProblem
    {
        private Guid m_Key;
        private Int64 m_Id;
        private string m_Description;
        private string m_Tag = string.Empty;
        private string m_LocalizedTag = string.Empty;

        public Guid Key
        {
            get { return m_Key; }
            set { m_Key = value; }
        }

        public Int64 Id
        {
            get { return m_Id; }
            set { m_Id = value; }
        }

        public string Tag
        {
            get { return m_Tag; }
            set { m_Tag = value; }
        }

        public string LocalizedTag
        {
            get
            {
                if (string.IsNullOrEmpty(m_LocalizedTag))
                {
                    return Tag;
                }
                return m_LocalizedTag;
            }
            set { m_LocalizedTag = value; }
        }

        public string Description
        {
            get { return m_Description; }
            set { m_Description = value; }
        }

        [Obsolete("Try not to use this")]
        public ReturnProblem()
        {
        }

        /// <summary>
        /// Guids
        /// </summary>
        /// <param name="key"></param>
        /// <param name="tag"></param>
        /// <param name="localizedTag"></param>
        /// <param name="description"></param>
        #region Guid
        public ReturnProblem(Guid key, string tag, string localizedTag, string description)
        {
            m_Key = key;
            m_Tag = tag;
            m_LocalizedTag = localizedTag;
            m_Description = description;
        }

        public ReturnProblem(Guid key, string tag, string description)
        {
            m_Key = key;
            m_Tag = tag;
            m_Description = description;
        }

        public ReturnProblem(Guid key, string description)
            : this(key, string.Empty, description)
        {
        }
        #endregion

        /// <summary>
        /// Ids
        /// </summary>
        /// <param name="key"></param>
        /// <param name="tag"></param>
        /// <param name="localizedTag"></param>
        /// <param name="description"></param>
        #region Id
        public ReturnProblem(Int64 id, string tag, string localizedTag, string description)
        {
            m_Id = id;
            m_Tag = tag;
            m_LocalizedTag = localizedTag;
            m_Description = description;
        }

        public ReturnProblem(Int64 id, string tag, string description)
        {
            m_Id = id;
            m_Tag = tag;
            m_Description = description;
        }

        public ReturnProblem(Int64 id, string description)
            : this(id, string.Empty, description)
        {
        }
        #endregion

        public override string ToString()
        {
            return string.Format("Description: {0} - ({1}/{2})", m_Description, m_Key, m_Id);
        }
    }

    public enum OpFailureCode
    {
        None = 0,
        SessionInQuarantine = 1,
        InvalidSession = 2,
        ProcessingFailed = 3,
        ProcessException = 4,
        NoAvailableSession = 5,
        UserDisabled = 6
    }

    /// <summary>
    /// Generic return object for McService.
    /// We will subclass this for webservice methods that return more than one object.
    /// Avoid using object types as other systems may have trouble deserializing.
    /// </summary>
    [Serializable]
    public class OpResult
    {
        public ReturnCode ReturnCode = ReturnCode.Fail;
        public OpFailureCode FailureCode = OpFailureCode.None;
        public string ExceptionMessage = string.Empty;
    }
}