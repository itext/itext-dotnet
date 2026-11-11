/*
This file is part of jsoup, see NOTICE.txt in the root of the repository.
It may contain modifications beyond the original version.
*/
using System;

namespace iText.StyledXmlParser.Jsoup {
    /// <summary>Signals that a HTTP request resulted in a not OK HTTP response.</summary>
    public class HttpStatusException : System.IO.IOException {
        private int statusCode;

        private String url;

        public HttpStatusException(String message, int statusCode, String url)
            : base(message) {
            this.statusCode = statusCode;
            this.url = url;
        }

        public virtual int GetStatusCode() {
            return statusCode;
        }

        public virtual String GetUrl() {
            return url;
        }

        public override String ToString() {
            return base.ToString() + ". Status=" + statusCode + ", URL=" + url;
        }
    }
}
