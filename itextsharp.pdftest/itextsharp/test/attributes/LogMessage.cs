using System;

namespace iTextSharp.Test.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class LogMessage : Attribute
    {
        private string messageTemplate;
        public int Count;
        public bool Ignore;

        public LogMessage(string messageTemplate)
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
