using System;
using System.Text.RegularExpressions;
using iTextSharp.IO.Log;
using iTextSharp.Test.Attributes;
using log4net.Appender;
using log4net.Core;
using NUnit.Framework;
using NUnit.Framework.Interfaces;

namespace iTextSharp.Test
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public class LogListener : TestActionAttribute
    {
        private static string LEFT_CURLY_BRACES = "{";
        private static string RIGHT_CURLY_BRACES = "}";
        private Log4NetLogger iLog = new Log4NetLogger();
        private MemoryAppender appender;

		public override void BeforeTest(ITest testDetails)
        {
            Init();
        }

		public override void AfterTest(ITest testDetails)
        {
            CheckLogMessages(testDetails);
        }

        public override ActionTargets Targets
        {
            get { return ActionTargets.Test; }
        }

        private void CheckLogMessages(ITest testDetails)
        {
			LogMessageAttribute[] attributes = testDetails.Method.GetCustomAttributes<LogMessageAttribute>(true);
            if (attributes.Length > 0)
            {
                for (int i = 0; i < attributes.Length; i++)
                {
                    LogMessageAttribute logMessage = attributes[i];
                    if (!logMessage.Ignore)
                    {
                        int foundedCount = Contains(logMessage.GetMessageTemplate());
                        if (foundedCount != logMessage.Count)
                        {
                            Assert.Fail("{0} Some log messages are not found in test execution - {1} messages",
                                testDetails.FullName,
                                logMessage.Count - foundedCount);

                        }
                    }
                }
            }
            else
            {
                if (GetSize() > 0)
                {
                    Assert.Fail("{0}: The test does not check the message logging - {1} messages",
                        testDetails.FullName,
                        GetSize());
                }

            }
        }

        /*
        * compare  parametrized message with  base template, for example:
        *  "Hello fox1 , World  fox2 !" with "Hello {0} , World {1} !"
        * */
        private bool EqualsMessageByTemplate(string message, string template)
        {
            if (template.IndexOf(RIGHT_CURLY_BRACES) > 0 && template.IndexOf(LEFT_CURLY_BRACES) > 0)
            {

                Regex rgx = new Regex("\\{.*?\\} ?");
                String templateWithoutParameters = rgx.Replace(template, "");
                String[] splitTemplate = Regex.Split(templateWithoutParameters, "\\s+");
                int prevPosition = 0;
                for (int i = 0; i < splitTemplate.Length; i++)
                {
                    int foundedIndex = message.IndexOf(splitTemplate[i], prevPosition);
                    if (foundedIndex < 0 && foundedIndex < prevPosition)
                    {
                        return false;
                    }
                    else
                    {
                        prevPosition = foundedIndex;
                    }
                }
                return true;
            }
            else
            {
                return message.Contains(template);
            }
        }

        private int Contains(String loggingStatement)
        {
            LoggingEvent[] eventList = appender.GetEvents();
            int index = 0;
            for (int i = 0; i < eventList.Length; i++)
            {
                if (EqualsMessageByTemplate(eventList[i].RenderedMessage, loggingStatement))
                {
                    index++;
                }
            }
            return index;
        }

        private void Init()
        {
            LoggerFactory.GetInstance().SetLogger(iLog);
            IAppender[] iAppenders = iLog.GetILog().Logger.Repository.GetAppenders();
            appender = iAppenders[0] as MemoryAppender;
            appender.Clear();
        }

        private int GetSize()
        {
            return appender.GetEvents().Length;

        }
    }
}
