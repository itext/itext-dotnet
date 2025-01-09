/*
This file is part of the iText (R) project.
Copyright (c) 1998-2025 Apryse Group NV
Authors: Apryse Software.

This program is offered under a commercial and under the AGPL license.
For commercial licensing, contact us at https://itextpdf.com/sales.  For AGPL licensing, see below.

AGPL licensing:
This program is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/
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
