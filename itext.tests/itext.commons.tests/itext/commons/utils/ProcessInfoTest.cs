using System;
using iText.Test;

namespace iText.Commons.Utils {
    public class ProcessInfoTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void GetExitCodeTest() {
            int exitCode = 1;
            ProcessInfo processInfo = new ProcessInfo(exitCode, null, null);
            NUnit.Framework.Assert.AreEqual(exitCode, processInfo.GetExitCode());
        }

        [NUnit.Framework.Test]
        public virtual void GetProcessStdOutput() {
            String stdOutput = "output";
            ProcessInfo processInfo = new ProcessInfo(0, stdOutput, null);
            NUnit.Framework.Assert.AreEqual(stdOutput, processInfo.GetProcessStdOutput());
        }

        [NUnit.Framework.Test]
        public virtual void GetProcessErrOutput() {
            String stdOutput = "output";
            ProcessInfo processInfo = new ProcessInfo(0, null, stdOutput);
            NUnit.Framework.Assert.AreEqual(stdOutput, processInfo.GetProcessErrOutput());
        }
    }
}
