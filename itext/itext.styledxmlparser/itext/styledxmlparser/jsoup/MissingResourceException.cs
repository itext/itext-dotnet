using System;

namespace iText.StyledXmlParser.Jsoup {
    public class MissingResourceException : Exception {

        private readonly string className;
        private readonly string key;

        public MissingResourceException(string message, string className, string key) : base(message) {
            this.className = className;
            this.key = key;
        }

        public MissingResourceException(string message, Exception innerException, string className, string key) : base(message, innerException) {
            this.className = className;
            this.key = key;
        }

        public string GetClassName() {
            return className;
        }

        public string GetKey() {
            return key;
        }
    }
}