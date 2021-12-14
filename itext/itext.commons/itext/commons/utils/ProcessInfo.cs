using System;

namespace iText.Commons.Utils {
    /// <summary>Class contains a process information, such as process exit code and process output.</summary>
    public sealed class ProcessInfo {
        private readonly int exitCode;

        private readonly String processStdOutput;

        private readonly String processErrOutput;

        /// <summary>
        /// Create a new instance, containing a process information,
        /// such as process exit code, process standard and error outputs.
        /// </summary>
        /// <param name="exitCode">exit code of the process.</param>
        /// <param name="processStdOutput">the standard output of the process.</param>
        /// <param name="processErrOutput">the error output of the process.</param>
        public ProcessInfo(int exitCode, String processStdOutput, String processErrOutput) {
            this.exitCode = exitCode;
            this.processStdOutput = processStdOutput;
            this.processErrOutput = processErrOutput;
        }

        /// <summary>Getter for a process exit code.</summary>
        /// <returns>Returns a process exit code.</returns>
        public int GetExitCode() {
            return exitCode;
        }

        /// <summary>Getter for a standard process output.</summary>
        /// <returns>Returns a process standard output string.</returns>
        public String GetProcessStdOutput() {
            return processStdOutput;
        }

        /// <summary>Getter for an error process output.</summary>
        /// <returns>Returns a process error output string.</returns>
        public String GetProcessErrOutput() {
            return processErrOutput;
        }
    }
}
