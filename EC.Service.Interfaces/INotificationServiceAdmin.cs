using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;
using EC.Common.Interfaces;
using EC.Constants;
using EC.Service.DTO;
using EC.Errors;
using EC.Errors.CommonExceptions;
using EC.Errors.NotificationExceptions;
using EC.Errors.ECExceptions;

namespace EC.Service.Interfaces
{
    /// <summary>
    /// Internal-use only service interface for the notification service.
    /// </summary>
    
    [ServiceContract]

    public interface INotificationServiceAdmin
    {

        /// <summary>
        /// Delete an organization notification rule by Id. 
        /// Do nothing if the rule doesn't exist.
        /// </summary>
        /// <param name="orgNotificationRuleId">Id of the organization notification rule to delete.</param>
        /// <exception cref="UnknownFault">On any error.</exception>

        [OperationContract]
        [FaultContract(typeof(UnknownFault))]
        
        void DeleteOrganizationNotificationRuleById(Guid orgNotificationRuleId);


        /// <summary>
        /// Get all user notification rules.
        /// </summary>
        /// <returns>List of user notification rules. Empty list of no rules exist.</returns>

        [OperationContract]
        [FaultContract(typeof(UnknownFault))]

        List<UserNotificationRule> GetAllUserNotificationRules();

        /// <summary>
        /// Get user notification rules for a <paramref name="userId"/>.
        /// </summary>
        /// <param name="userId">User Id to get the rules from</param>
        /// <returns>List of user notification rules. Empty list if no rules assigned to the user.</returns>
        /// <exception cref="UnknownFault">On any error.</exception>
 
        [OperationContract]
        [FaultContract(typeof(UnknownFault))]

        List<UserNotificationRule> GetUserNotificationRulesByUserId(Guid userId);
        
        /// <summary>
        /// Delete user notification rule, identified by its Id.
        /// </summary>
        /// <param name="userNotificationRuleId">User notification rule Id to delete.</param>
        /// <exception cref="NotFoundFault">If the user notification rule doesn't exist.</exception>
        /// <exception cref="UnknownFault">On any error.</exception>
        
        [OperationContract]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(UnknownFault))]

        void DeleteUserNotificationRuleById(Guid userNotificationRuleId);

        /// <summary>
        /// Add a new user notification rule.
        /// Set the UserId property to assign it to a user (mandatory).
        /// Id must be set to <c>null</c>.
        /// <param name="userId">User Id to add the notification rules</param>
        /// <param name="newRule">Notification rules to replace with. Set <c>null</c> or empty list to remove all rules for the user</param>
        /// <exception cref="CannotCreateItemFault">If the Id of the <paramref name="newRule"/> is not <c>null</c> or <paramref name="newRule"/> is <c>null</c>.</exception>
        /// <exception cref="NotificationExpressionFault">If the notification expression of a notification rule is wrong</exception>
        /// <exception cref="UnknownFault">On any other error.</exception>

        [OperationContract]
        [FaultContract(typeof(NotificationExpressionFault))]
        [FaultContract(typeof(UnknownFault))]

        Guid AddUserNotificationRule(UserNotificationRule newRule);

        /// <summary>
        /// Send an email to the given list of users (as well as creating a notification based on
        /// the provided notification details).
        /// <para>
        /// The list of recipients is specified in a somewhat odd fashion in this call because the
        /// 'to' parameter associates multiple email addresses with a single user ID. The is basically
        /// a hack to support the notion of cc'ing an email. The notification service does not
        /// directly support cc, but it can include multiple email addresses on the 'To:' line.
        /// However, the template parameters used for each email should all be derived from a
        /// single user, and that is the GUID in the directionary. 
        /// </para>
        /// </summary>
        /// <param name="to">list of (user ID, email address) pairs to send to</param>
        /// <param name="body">ad hoc template for body of email</param>
        /// <param name="subject">ad hoc template for subject line of email</param>
        /// <param name="details">details for notification</param>
        /// <exception cref="BadChannelFault">if the channel does not exist</exception>
        /// <exception cref="UnknownFault">On any other error</exception>

        [OperationContract]
        [FaultContract(typeof(BadChannelFault))]
        [FaultContract(typeof(UnknownFault))]
        
        void SendAdHocEmailsByIdsAndEmails(Dictionary<Guid, List<string>> to, string body, string subject, NotificationDetails details);

        /// <summary>
        /// Send an email to the given list of users (as well as creating a notification based on
        /// the provided notification details).
        /// </summary>
        /// <param name="to">list of user ID's to send to</param>
        /// <param name="body">ad hoc template for body of email</param>
        /// <param name="subject">ad hoc template for subject line of email</param>
        /// <param name="details">details for notification</param>
        /// <exception cref="BadChannelFault">if the channel does not exist</exception>
        /// <exception cref="BadReceiverPropertyFault">if the property does not exist on IUser</exception>
        /// <exception cref="UnknownFault">On any other error</exception>
        
        [OperationContract]
        [FaultContract(typeof(BadReceiverPropertyFault))]
        [FaultContract(typeof(BadChannelFault))]
        [FaultContract(typeof(UnknownFault))]

        void SendAdHocEmailByIds(List<Guid> to, string body, string subject, NotificationDetails details);

        /// <summary>
        /// Send an email to the users (as well as creating a notification based on
        /// the provided notification details).
        /// </summary>
        /// <param name="to">the user's Id to send to</param>
        /// <param name="body">ad hoc template for body of email</param>
        /// <param name="subject">ad hoc template for subject line of email</param>
        /// <param name="details">details for notification</param>
        /// <exception cref="BadChannelFault">if the channel does not exist</exception>
        /// <exception cref="BadReceiverPropertyFault">if the property does not exist on IUser</exception>
        /// <exception cref="UnknownFault">On any other error</exception>

        [OperationContract]
        [FaultContract(typeof(BadReceiverPropertyFault))]
        [FaultContract(typeof(BadChannelFault))]
        [FaultContract(typeof(UnknownFault))]

        void SendAdHocEmailById(Guid to, string body, string subject, NotificationDetails details);

        /// <summary>
        /// Send an email to the given list of recipients. The body of the message is CSHTML and can
        /// use model variables included in the templateVariables dictionary. Recipients are specified
        /// as email addresses. NOTE: This operation only be used when SendAdHocEmailById() cannot be
        /// used.
        /// </summary>
        /// <param name="to">list of email addresses</param>
        /// <param name="body">ad hoc template for body of email</param>
        /// <param name="subject">subject line for email</param>
        /// <param name="details">details for notification to be created</param>
        /// <exception cref="EmailFormatFault">if the email address is not valid</exception>
        /// <exception cref="BadChannelFault">if the channel does not exist</exception>
        /// <exception cref="UnknownFault">On any other error</exception>
        
        [OperationContract]
        [FaultContract(typeof(EmailFormatFault))]
        [FaultContract(typeof(BadChannelFault))]
        [FaultContract(typeof(UnknownFault))]

        void SendAdHocEmail(List<string> to, string body, string subject, NotificationDetails details);

        /// <summary>
        /// Send an email which uses one of the built-in/custom notification templates. Recipients are 
        /// specified by primary user ID. 
        /// </summary>
        /// <param name="to">list of user ID's</param>
        /// <param name="details">details about notification to be created</param>
        /// <exception cref="BadChannelFault">if the channel does not exist</exception>
        /// <exception cref="BadReceiverPropertyFault">if the property does not exist on IUser</exception>
        /// <exception cref="UnknownFault">On any other error</exception>
        
        [OperationContract]
        [FaultContract(typeof(BadReceiverPropertyFault))]
        [FaultContract(typeof(BadChannelFault))]
        [FaultContract(typeof(UnknownFault))]

        void SendEmailFromTemplateById(List<Guid> to, NotificationDetails details);

        /// <summary>
        /// Send an email which uses one of the built-in/custom notification templates. Recipients
        /// are specified as a list of email addresses. NOTE: This operation should only be used
        /// when SendEmailFromTemplateById() cannot be used.
        /// </summary>
        /// <param name="to">list of email addresses</param>
        /// <param name="templateName">details for notification to be created</param>
        /// <exception cref="EmailFormatFault">If email address has a wrong format</exception>
        /// <exception cref="TargetBadChannelFault">if the channel does not exist</exception>
        /// <exception cref="UnknownFault">On any other error</exception>
        
        [OperationContract]
        [FaultContract(typeof(EmailFormatFault))]
        [FaultContract(typeof(BadChannelFault))]
        [FaultContract(typeof(UnknownFault))]

        void SendEmailFromTemplate(List<string> to, NotificationDetails details);

        /// <summary>
        /// Generate a notification based on one of the compiled in notification templates.
        /// </summary>
        /// <param name="details">details about the notification to be created</param>
        /// <exception cref="UnknownFault">On any other error</exception>

        [OperationContract]
        [FaultContract(typeof(UnknownFault))]

        void PushNotification(NotificationDetails details);


        /// <summary>
        /// Generate a registration notification based on one of the built-in templates.
        /// This is usually called for a student registration/de-registration from a course offering.
        /// </summary>
        /// <param name="registrationDetails">Registration notification details.</param>
        /// <exception cref="UnknownFault">On any error.</exception>

        [OperationContract]
        [FaultContract(typeof(UnknownFault))]

        void PushRegistrationNotification(RegistrationNotificationDetails registrationDetails);

        /// <summary>
        /// Retrieve a custom template by name and type.
        /// </summary>
        /// <param name="templateName">base template name (no suffix)</param>
        /// <param name="templateType">template type (full/summary/title)</param>
        /// <returns>contents of template</returns>
        /// <exception cref="NotificationConfigFault">if incoming template type cannot be parsed</exception>
        /// <exception cref="NotFoundFault">if the template is not found</exception>
        /// <exception cref="UnknownFault">On any other error</exception>
        
        [OperationContract]
        [FaultContract(typeof(NotificationConfigFault))]
        [FaultContract(typeof(NotFoundFault))]
        [FaultContract(typeof(UnknownFault))]

        string GetCustomTemplate(string templateName, NotificationTemplateTypeEnum templateType);

        /// <summary>
        /// Save a custom template. This will create/overwrite a template.
        /// </summary>
        /// <param name="uow">current unit of work</param>
        /// <param name="templateName">base template name</param>
        /// <param name="templateType">template type (full/summary/title)</param>
        /// <param name="templateContent">content for template</param>
        /// <exception cref="NotificationConfigFault">if incoming template type cannot be parsed</exception>
        /// <exception cref="UnknownFault">On any other error</exception>

        [OperationContract]
        [FaultContract(typeof(NotificationConfigFault))]
        [FaultContract(typeof(UnknownFault))]

        void SaveCustomTemplate(string templateName, NotificationTemplateTypeEnum templateType, string templateContent);

        /// <summary>
        /// Delete a custom template from the system
        /// </summary>
        /// <param name="uow">current unit of work</param>
        /// <param name="templateName">base template name</param>
        /// <param name="templateType">template type (full/summary/title)</param>
        /// <exception cref="NotificationConfigFault">if incoming template type cannot be parsed</exception>
        /// <exception cref="UnknownFault">On any other error</exception>
        
        [OperationContract]
        [FaultContract(typeof(NotificationConfigFault))]
        [FaultContract(typeof(UnknownFault))]

        void DeleteCustomTemplate(string templateName, NotificationTemplateTypeEnum templateType);

        /// <summary>
        /// Endpoint verification function for installer. 
        /// </summary>
        /// <returns>true</returns>
        [OperationContract]

        bool IsNotificationServiceAdmin();
    }
}
