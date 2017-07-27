using System;
using System.Collections.Generic;
using System.Text;

namespace EC.Common.Util
{
    public class AsyncActionResult
    {
        public ActionResultExtended Result
        {
            get;
            set;
        }


        public int PercentComplete
        {
            get;
            set;
        }

        public Int32 UserId
        {
            get;
            set;
        }
        public string SessionId
        {
            get;
            set;
        }

        /// <summary>
        /// A request id is a unique id of the originating request from the client application so the app can match up the result 
        /// to their original request. The server must accept a guid in it's web service /controller action call so this can be populated.
        /// </summary>
        public Guid? RequestId
        {
            get;
            set;
        }

        public DateTime RaisedDateTime
        {
            get;
            set;
        }


        public AsyncActionResult(ActionResultExtended result, Int32 userId, string sessionId, Guid? requestId = null)
        {
            Result = result;
            UserId = userId;
            SessionId = sessionId;
            RaisedDateTime = DateTime.Now;
            RequestId = requestId;
        }

        public override string ToString()
        {
            return Result.ToString();
        }
    }

}
