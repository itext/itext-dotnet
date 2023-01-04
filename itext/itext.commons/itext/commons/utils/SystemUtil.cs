/*

This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: Bruno Lowagie, Paulo Soares, et al.

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License version 3
as published by the Free Software Foundation with the addition of the
following permission added to Section 15 as permitted in Section 7(a):
FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
OF THIRD PARTY RIGHTS

This program is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU Affero General Public License for more details.
You should have received a copy of the GNU Affero General Public License
along with this program; if not, see http://www.gnu.org/licenses or write to
the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
Boston, MA, 02110-1301 USA, or download the license from the following URL:
http://itextpdf.com/terms-of-use/

The interactive user interfaces in modified source and object code versions
of this program must display Appropriate Legal Notices, as required under
Section 5 of the GNU Affero General Public License.

In accordance with Section 7(b) of the GNU Affero General Public License,
a covered work must retain the producer line in every PDF that is created
or manipulated using iText.

You can be released from the requirements of the license by purchasing
a commercial license. Buying such a license is mandatory as soon as you
develop commercial activities involving the iText software without
disclosing the source code of your own applications.
These activities include: offering paid services to customers as an ASP,
serving PDFs on the fly in a web application, shipping iText with a closed
source product.

For more information, please contact iText Software Corp. at this
address: sales@itextpdf.com
*/

using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace iText.Commons.Utils {
    /// <summary>
    /// This file is a helper class for internal usage only.
    /// Be aware that its API and functionality may be changed in future.
    /// </summary>
    public class SystemUtil {
        private const String SPLIT_REGEX = "((\".+?\"|[^'\\s]|'.+?')+)\\s*";

        private const int STD_OUTPUT_INDEX = 0;
        private const int ERR_OUTPUT_INDEX = 1;

        public static long GetTimeBasedSeed() {
            return DateTime.Now.Ticks + Environment.TickCount;
        }

        public static int GetTimeBasedIntSeed() {
            return unchecked((int) DateTime.Now.Ticks) + Environment.TickCount;
        }

        /// <summary>
        /// Should be used in relative constructs (for example to check how many milliseconds have passed).
        /// Shouldn't be used in the DateTime creation since the nanoseconds are expected there.
        /// </summary>
        /// <returns>relative time in milliseconds</returns>
        public static long GetRelativeTimeMillis() {
            return DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
        }

        public static long GetFreeMemory() {
            return GC.GetTotalMemory(false);
        }

        /// <summary>
        /// Gets environment variable with given name.
        /// </summary>
        /// <param name="name">the name of environment variable.</param>
        /// <returns>variable value or null if there is no such.</returns>
        public static String GetEnvironmentVariable(String name) {
            return Environment.GetEnvironmentVariable(name);
        }

        public static bool RunProcessAndWait(String exec, String @params) {
            return RunProcessAndWait(exec, @params, null);
        }
        
        public static bool RunProcessAndWait(String exec, String @params, String workingDirPath) {
            return RunProcessAndGetExitCode(exec, @params, workingDirPath) == 0;
        }
        
        public static int RunProcessAndGetExitCode(String exec, String @params)
        {
            return RunProcessAndGetExitCode(exec, @params, null);
        }
        
        public static int RunProcessAndGetExitCode(String exec, String @params, String workingDirPath) {
            using (Process proc = new Process()) {
                SetProcessStartInfo(proc, exec, @params, workingDirPath);
                proc.Start();
                Console.WriteLine(GetProcessOutput(proc));
                proc.WaitForExit();
                return proc.ExitCode;
            }
        }

        public static String RunProcessAndGetOutput(String exec, String @params) {
            String processOutput;
            using (Process proc = new Process()) {
                SetProcessStartInfo(proc, exec, @params, null);
                proc.Start();
                processOutput = GetProcessOutput(proc);
                proc.WaitForExit();
            }

            return processOutput;
        }
        
        public static ProcessInfo RunProcessAndGetProcessInfo(String exec, String @params) {
            using (Process proc = new Process()) {
                SetProcessStartInfo(proc, exec, @params, null);
                proc.Start();
                StringBuilder[] outputStreamStrings = GetProcessOutputBuilders(proc);
                String processStdOutput = outputStreamStrings[STD_OUTPUT_INDEX].ToString();
                String processErrOutput = outputStreamStrings[ERR_OUTPUT_INDEX].ToString();
                proc.WaitForExit();
                return new ProcessInfo(proc.ExitCode, processStdOutput, processErrOutput);
            }
        }

        public static StringBuilder RunProcessAndCollectErrors(String exec, String @params) {
            StringBuilder errorsBuilder;
            using (Process proc = new Process()) {
                SetProcessStartInfo(proc, exec, @params, null);
                proc.Start();
                errorsBuilder = GetProcessErrorsOutput(proc);
                Console.Out.WriteLine(errorsBuilder.ToString());
                proc.WaitForExit();
            }

            return errorsBuilder;
        }

        internal static void SetProcessStartInfo(Process proc, String exec, String @params) {
            SetProcessStartInfo(proc, exec, @params, null);
        }
        
        internal static void SetProcessStartInfo(Process proc, String exec, String @params, String workingDir) {
            String[] processArguments = PrepareProcessArguments(exec, @params);
            proc.StartInfo = new ProcessStartInfo(processArguments[0], processArguments[1]);
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.RedirectStandardError = true;
            proc.StartInfo.CreateNoWindow = true;
            proc.StartInfo.WorkingDirectory = workingDir;
        }

        internal static String[] PrepareProcessArguments(String exec, String @params) {
            bool isExcitingFile;
            try
            {
                isExcitingFile = new FileInfo(exec).Exists;
            }
            catch (Exception)
            {
                isExcitingFile = false;
            }

            return isExcitingFile
                ? new[] {exec, @params.Replace("'", "\"")}
                : SplitIntoProcessArguments(exec, @params);
        }

        internal static String[] SplitIntoProcessArguments(String command, String @params) {
            Regex regex = new Regex(SPLIT_REGEX);
            MatchCollection matches = regex.Matches(command);
            String processCommand = "";
            String processArguments = "";
            if (matches.Count > 0)
            {
                processCommand = matches[0].Value.Trim();
                for (int i = 1; i < matches.Count; i++)
                {
                    Match match = matches[i];
                    processArguments += match.Value;
                }

                processArguments = processArguments + " " + @params;
                processArguments = processArguments.Replace("'", "\"").Trim();
            }

            return new[] {processCommand, processArguments};
        }

        internal static String GetProcessOutput(Process p) {
            StringBuilder[] builders = GetProcessOutputBuilders(p);

            return builders[STD_OUTPUT_INDEX].ToString() 
                   + '\n' 
                   + builders[ERR_OUTPUT_INDEX].ToString();
        }
        
        internal static StringBuilder[] GetProcessOutputBuilders(Process p) {
            StringBuilder bri = new StringBuilder();
            StringBuilder bre = new StringBuilder();
            do {
                bri.Append(p.StandardOutput.ReadToEnd());
                bre.Append(p.StandardError.ReadToEnd());
            } while (!p.HasExited);
            
            Console.Out.WriteLine(bre.ToString());

            StringBuilder[] resultOutputArray = new[] { bri, bre };
            return resultOutputArray;
        }

        internal static StringBuilder GetProcessErrorsOutput(Process p) {
            StringBuilder bre = new StringBuilder();
            do
            {
                bre.Append(p.StandardError.ReadToEnd());
            } while (!p.HasExited);

            return bre;
        }
    }
}
