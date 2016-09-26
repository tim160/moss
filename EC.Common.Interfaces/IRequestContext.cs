using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EC.Common.Interfaces
{
    /// <summary>
    /// Provides access to information about the context of the current request.
    /// </summary>
    
    public interface IRequestContext
    {
        /// <summary>
        /// Initialize this request context from another.
        /// </summary>
        /// <param name="src">context to initialize from</param>
        
        void CopyFrom(IRequestContext src);

        /// <summary>
        /// Returns the Id for the user who is making this request. If null, then 
        /// it is an anonymous request.
        /// </summary>
        
        Guid? CurrentUserId { get; set; }

        /// <summary>
        /// Returns the user name of the  user who is making this request. If null, then
        /// it is an anonymous request.
        /// </summary>

        string CurrentUserName { get; set; }

        /// <summary>
        /// Returns an Id which is unique to the current external request that
        /// is being handled.
        /// </summary>
        
        Guid? ConversationId { get; set; }

        /// <summary>
        /// Unique authentication Id to be used in the external service.
        /// After a user has been validated this is the trusted Id to identify subsequent service calls.
        /// </summary>
        /// <remarks>
        /// The authentication Id timeout if there are no requests for a certain amount of time or the
        /// user logged out.
        /// </remarks>

        Guid? AuthenticationId { get; set; }

        /// <summary>
        /// The user .NET time zone. Null, if the default time zone should be used (GMT Standard Time)
        /// </summary>

        string UserTimeZone { get; set; }

        /// <summary>
        /// The .NET user language used to set the culture and UI culture. 
        /// If <c>null</c> the default culture and UI culture of the thread will be used.
        /// </summary>
        string UserLanguage { get; set; }

        /// <summary>
        /// The Url Host address the request came in on
        /// </summary>

        string URLHost { get; set; }

        /// <summary>
        /// Users display name 
        /// </summary>

        string DisplayName { get; set; }
    }
}
