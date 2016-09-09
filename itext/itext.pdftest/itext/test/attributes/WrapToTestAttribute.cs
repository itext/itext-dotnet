using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iText.Test.Attributes {

    /// <summary>
    /// This attribute can be used to run a class that contains a <code>public static void Main</code> method as a test in the NUnit test runner.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class WrapToTestAttribute : Attribute {
        private string ignoreWithMessage = "";

        /// <summary>
        /// Makes the test runner ignore the annotated class if a non-empty String is
        /// specified. The text should contain a reason for ignoring this test, as it
        /// is fed to the test runner.
        /// 
        /// Defaults to the empty String, which does not trigger ignoring the test.
        /// </summary>
        public virtual string IgnoreWithMessage {
            get { return ignoreWithMessage; }
            set { ignoreWithMessage = value; }
        }
    }
}
