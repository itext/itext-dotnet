using System;

namespace iText.Test.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class LogMessageAttribute : Attribute
    {
        private string messageTemplate;
        public int Count;
        public bool Ignore;

        public LogMessageAttribute(string messageTemplate)
        {
            this.messageTemplate = messageTemplate;
            this.Count = 1;
            this.Ignore = false;
        }

        public string GetMessageTemplate()
        {
            return this.messageTemplate;
        }
    }
}
