using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UniWatch.Enumerations
{
    public class EmailContentType
    {
        public string Value { get; private set; }

        public static EmailContentType Plain => new EmailContentType("text/plain");
        public static EmailContentType Html => new EmailContentType("text/html");

        private EmailContentType(string value)
        {
            this.Value = value;
        }
    }
}