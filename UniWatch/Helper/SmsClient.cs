using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using Twilio;

namespace UniWatch.Helper
{
    /// <summary>
    /// Represents the sms client for Twilio.
    /// </summary>
    public class SmsClient
    {
        private readonly TwilioRestClient _client;
        private readonly string _smsFrom;

        /// <summary>
        /// The constructor for the SMS client.
        /// </summary>
        public SmsClient()
        {
            var accountSid = WebConfigurationManager.AppSettings["TwilioAccountSid"];
            var authToken = WebConfigurationManager.AppSettings["TwilioAuthToken"];

            this._client = new TwilioRestClient(accountSid, authToken);
            this._smsFrom = WebConfigurationManager.AppSettings["TwilioSmsNumber"];
        }

        /// <summary>
        /// Send a SMS message
        /// </summary>
        /// <param name="to">
        /// The phone number where the SMS is sent (include area code and country code).
        /// </param>
        /// <param name="body">
        /// The body of the text message.
        /// </param>
        /// <returns></returns>
        public SMSMessage SendMessage(string to, string body)
        {
            return this._client.SendSmsMessage(_smsFrom, to, body);
        }
    }
}