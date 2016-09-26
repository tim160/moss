using EC.Errors.CommonExceptions;

namespace EC.Common.Interfaces
{
    /// <summary>
    /// Simply sends an email
    /// </summary>
    
    public interface IEmailSender
    {
        /// <summary>
        /// Sends an email through the email relay configured in the application settings.
        /// </summary>
        /// <param name="from">The from email address </param>
        /// <param name="to">Receiver of the email</param>
        /// <param name="subject">Email subject</param>
        /// <param name="body">Email body as string</param>
        /// <param name="isBodyHtml">Optional: Flag whether the mail message body is in Html</param>
        /// <returns>Return <c>true</c> if email has been sent.</returns>
        /// <exception cref="EmailSendException">If the email couldn't be sent because of an error.</exception>
        /// <exception cref="EmailFormatException">
        /// If either <paramref name="from"/> or <paramref name="to"/> email address has a wrong format.
        /// </exception>
        bool Send(string from, string to, string subject, string body, bool isBodyHtml = false);
    }
}