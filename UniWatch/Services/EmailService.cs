using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;
using UniWatch.Enumerations;

namespace UniWatch.Services
{
    /// <summary>
    /// Represents the email client for SendGrid
    /// </summary>
    public class EmailService
    {
        private readonly SendGridAPIClient _service;

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
        public async Task SendEmail(string from, string to, string subject, string body, EmailContentType type=null)
        {
            // If the user did not select a type,
            // then set the type to plain
            if (type == null)
            {
                type = EmailContentType.Plain;
            }

            var emailFrom = new Email(from);
            var emailTo = new Email(to);
            var content = new Content(type.Value, body);

            // Prepare the email
            var mail = new Mail(emailFrom, subject, emailTo, content);

            // Send the email
            await _service.client.mail.send.post(requestBody: mail.Get());
        }
    }
}