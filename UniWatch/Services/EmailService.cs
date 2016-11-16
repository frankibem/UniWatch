using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using Microsoft.AspNet.Identity;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace UniWatch.Services
{
    /// <summary>
    /// Represents the email client for SendGrid
    /// </summary>
    public class EmailService : IIdentityMessageService
    {
        #region Fields and Properties

        private readonly SendGridAPIClient _service;

        public enum EmailContentType
        {
            Plain,
            Html
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// The constructor for the email service
        /// </summary>
        /// <param name="apiKey">
        /// The API key for the SendGrid account
        /// </param>
        public EmailService(string apiKey)
        {
            _service = new SendGridAPIClient(apiKey);
        }

        /// <summary>
        /// The default constructor for the email service.
        /// Uses the API key in the web config for the SendGrid API
        /// </summary>
        public EmailService() : this(WebConfigurationManager.AppSettings["SendGridApiKey"])
        {       
        }

        /// <summary>
        /// Sends an email
        /// </summary>
        /// <param name="from">
        /// The email address that the recipient will respond to
        /// </param>
        /// <param name="to">
        /// The email address of the recipient
        /// </param>
        /// <param name="subject">
        /// The subject line of the email
        /// </param>
        /// <param name="body">
        /// The contents of the email
        /// </param>
        /// <param name="type">
        /// The type of the contents (plain, html, etc.)
        /// </param>
        /// <returns>
        /// The result of the task
        /// </returns>
        public async Task<dynamic> SendEmail(string from, string to, string subject, string body, 
            EmailContentType type=EmailContentType.Plain)
        {
            var emailFrom = new Email(from);
            var emailTo = new Email(to);
            var content = new Content(GetEmailContentString(type), body);

            // Prepare the email
            var mail = new Mail(emailFrom, subject, emailTo, content);

            // Send the email
            return await _service.client.mail.send.post(requestBody: mail.Get());
        }

        /// <summary>
        /// Expose a way to send emails.
        /// </summary>
        /// <param name="message">
        /// A message with a body, destination, and subject.
        /// </param>
        /// <returns>
        /// The task.
        /// </returns>
        public Task SendAsync(IdentityMessage message)
        {
            var result = this.SendEmail(
                WebConfigurationManager.AppSettings["EmailFrom"],
                message.Destination,
                message.Subject,
                message.Body
            );
            Trace.TraceInformation(result.ToString());

            // Twilio doesn't currently have an async API, so we return success.
            return Task.FromResult(0);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Get the email content type string
        /// Default is "text/plain"
        /// </summary>
        /// <param name="type">
        /// The email content type enumeration
        /// </param>
        /// <returns></returns>
        private static string GetEmailContentString(EmailContentType type)
        {
            string contentString;

            switch (type)
            {
                case EmailContentType.Html:
                    contentString = "text/html";
                    break;
                default:
                    contentString = "text/plain";
                    break;
            }

            return contentString;
        }

        #endregion
    }
}