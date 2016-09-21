using EC.Constants;
using EC.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;

namespace EC.Core.Common
{
    [SingletonType]
    [RegisterAsType(typeof(IWCFHeaderHelper))]

    public class WCFHeaderHelper : IWCFHeaderHelper
    {
        /// <summary>
        /// Get header string as nullable Guid. The header must exist.
        /// </summary>
        /// <param name="headerName"></param>
        /// <param name="headers"></param>
        /// <returns>
        /// Return the nullable Guid of the header. 
        /// Return <c>null</c> if the Guid is <c>null</c>, contains only white space or an error occurred
        /// </returns>

        public Guid? GetHeaderAsOptionalGuid(string headerName, MessageHeaders headers)
        {
            string idString = null;
            try
            {
                idString = headers.GetHeader<string>(headerName, WCFHeaderConstants.MLSNamespace);
            }
            catch (MessageHeaderException)
            {
                return null;
            }
            if (string.IsNullOrWhiteSpace(idString)) { return null; }

            Guid id;
            if (Guid.TryParse(idString, out id))
            {
                return id;
            }
            return null;
        }

        /// <summary>
        /// Get header as string value. The header must exist.
        /// </summary>
        /// <param name="headerName"></param>
        /// <param name="headers"></param>
        /// <returns>Return the string read from the header. Return <c>null</c> if the string is <c>null</c> or an error occurred</returns>

        public string GetHeaderAsString(string headerName, MessageHeaders headers)
        {
            string value = null;
            try
            {
                value = headers.GetHeader<string>(headerName, WCFHeaderConstants.MLSNamespace);
                return value;
            }
            catch (MessageHeaderException)
            {
                return null;
            }
        }

        /// <summary>
        /// Get header as string value. The header is optional.
        /// </summary>
        /// <param name="headerName"></param>
        /// <param name="headers"></param>
        /// <returns>Return the string read from the header. Return <c>null</c> if the string is <c>null</c>, only contains of white spaces, is empty or doesn't exist</returns>
        public string GetHeaderAsOptionalString(string headerName, MessageHeaders headers)
        {
            string value = null;
            try
            {
                value = headers.GetHeader<string>(headerName, WCFHeaderConstants.MLSNamespace);
            }
            catch (MessageHeaderException)
            {
                return null;
            }
            if (string.IsNullOrWhiteSpace(value)) { return null; }
            
            return value;
        }


        /// <summary>
        /// Add new header with <paramref name="headerName"/>.
        /// </summary>
        /// <remarks>
        /// If <paramref name="value"/> is <c>null</c>, an empty string is written into the header.
        /// </remarks>
        /// <param name="value">String value to write</param>
        /// <param name="headerName">Header name</param>
        /// <param name="request">Request message to add the header</param>

        public void AddRequestHeader(string value, string headerName, Message request)
        {
            var newHeader = MessageHeader.CreateHeader(headerName, WCFHeaderConstants.MLSNamespace, value == null ? string.Empty : value);
            request.Headers.Add(newHeader);
        }

        /// <summary>
        /// Add new header with <paramref name="headerName"/>.
        /// </summary>
        /// <remarks>
        /// If <paramref name="idValue"/> is <c>null</c>, an empty string is written into the header.
        /// </remarks>
        /// <param name="idValue">Id value. If <c>null</c> an empty string is written into the header</param>
        /// <param name="headerName">Header name</param>
        /// <param name="request">Request message to add the header</param>

        public void AddRequestHeader(Guid? idValue, string headerName, Message request)
        {
            var idString = idValue.HasValue ? idValue.Value.ToString() : string.Empty;
            var newHeader = MessageHeader.CreateHeader(headerName, WCFHeaderConstants.MLSNamespace, idString);
            request.Headers.Add(newHeader);
        }

    }
}
