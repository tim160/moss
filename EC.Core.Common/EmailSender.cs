using System;
using System.Net.Mail;
using EC.Common.Interfaces;
using EC.Errors.CommonExceptions;

namespace EC.Core.Common
{
    /// <summary>
    /// Class to send emails.
    /// </summary>
   
    [SingletonType]
    [RegisterAsType(typeof(IEmailSender))]
    public class EmailSender : IEmailSender
    {
        /// <summary>
        /// Sends an email through the email relay configured in the application settings.
        /// </summary>
        /// <param name="from">The from email address</param>
        /// <param name="to">Receiver of the email</param>
        /// <param name="subject">Email subject</param>
        /// <param name="body">Email body as string</param>
        /// <param name="isBodyHtml">Optional: Flag whether the mail message body is in Html</param>
        /// <returns>
        /// Return <c>true</c> if email has been sent.
        /// </returns>
        
        public bool Send(string from, string to, string subject, string body, bool isBodyHtml = false)
        {
           
            if (!string.IsNullOrEmpty(Settings.EmailOverride))
            {
                subject = string.Format("{0} (OVERRIDE: {1})", subject, to);
                to = Settings.EmailOverride;
              
            }

            // Check email addresses...
            addressHelper.CheckEmailAddressFormat(from);
            addressHelper.CheckEmailAddressFormat(to);
            
            //Send the Message
            MailMessage message = new MailMessage(from, to, subject, body);
            message.IsBodyHtml = isBodyHtml;
            SmtpClient smtp = new SmtpClient();
            try
            {
                smtp.Send(message);
                return true;
            }
            catch (Exception ex)
            {
                throw new EmailSendException(string.Format("Can't send email. Reason: {0}", ex.Message), from, to, ex);
            }
        }

        public EmailSender(IEmailAddressHelper eah, IAppSettings ia)
        {
            addressHelper = eah;
            Settings = ia;

        }

        private IEmailAddressHelper addressHelper;
        private IAppSettings Settings;
    }
}
