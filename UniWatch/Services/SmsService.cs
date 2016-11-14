using System.Diagnostics;
using System.Threading.Tasks;
using System.Web.Configuration;
using Microsoft.AspNet.Identity;
using Twilio;

namespace UniWatch.Services
{
    /// <summary>
    /// Represents the SMS client for Twilio.
    /// </summary>
    public class SmsService : IIdentityMessageService
    {
        #region Fields and Properties

        private readonly TwilioRestClient _service;

        #endregion

        #region Public Methods

        /// <summary>
        /// The constructor for the SMS service
        /// </summary>
        /// <param name="accountSid">
        /// The Twilio account SID.
        /// </param>
        /// <param name="authToken">
        /// The Twilio auth token.
        /// </param>
        public SmsService(string accountSid, string authToken)
        {
            this._service = new TwilioRestClient(accountSid, authToken);
        }

        /// <summary>
        /// The default constructor for the SMS client.
        /// Uses the Account SID and the Auth Token to create the Twilio REST service.
        /// </summary>
        public SmsService() : this(WebConfigurationManager.AppSettings["TwilioAccountSid"],
                                   WebConfigurationManager.AppSettings["TwilioAuthToken"])
        {
        }

        /// <summary>
        /// Send a SMS message
        /// </summary>
        /// <param name="from">
        /// The phone number to send the SMS (include country code and area code).
        /// </param>
        /// <param name="to">
        /// The phone number where the SMS is sent (include country code and area code).
        /// </param>
        /// <param name="body">
        /// The body of the text message.
        /// </param>
        /// <returns></returns>
        public Message SendMessage(string from, string to, string body)
        {
            return this._service.SendMessage(from, to, body);
        }

        /// <summary>
        /// Expose a way to send SMS messages.
        /// </summary>
        /// <param name="message">
        /// A message with a body, destination, and subject.
        /// </param>
        /// <returns>
        /// The task.
        /// </returns>
        public Task SendAsync(IdentityMessage message)
        {
            var from = WebConfigurationManager.AppSettings["TwilioSmsMessage"];
            var result = this.SendMessage(from, message.Destination, message.Body);

            Trace.TraceInformation(result.Status);

            // Twilio doesn't currently have an async API, so we return success.
            return Task.FromResult(0);
        }

        #endregion
    }
}