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
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using iText.IO.Util;
using NUnit.Framework;

namespace iText.Test.Pdfa {
    public class VeraPdfValidator {
        private const String CLI_COMMAND = "java -classpath \"<libPath>\\*\" -Dfile.encoding=UTF8 " +
                                    "-XX:+IgnoreUnrecognizedVMOptions -Dapp.name=\"VeraPDF validation GUI\" " +
                                    "-Dapp.repo=\"<libPath>\" -Dapp.home=\"../\" " +
                                    "-Dbasedir=\"\" org.verapdf.apps.GreenfieldCliWrapper --addlogs ";

        public String Validate(String dest) {
            Process p = new Process();
            String currentCommand = CLI_COMMAND.Replace("<libPath>",
                TestContext.CurrentContext.TestDirectory + "\\lib\\VeraPdf");

            p.StartInfo = new ProcessStartInfo("cmd", "/c" + currentCommand + dest );
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.StandardOutputEncoding = Encoding.UTF8;
            p.StartInfo.StandardErrorEncoding = Encoding.UTF8;
            p.Start();

            String result = HandleVeraPdfOutput(p, dest);
            p.WaitForExit();

            return result;
        }

        private string HandleVeraPdfOutput(Process p, String dest) {
            StringBuilder standardOutput = new StringBuilder();
            StringBuilder standardError = new StringBuilder();

            while (!p.HasExited) {
                standardOutput.Append(p.StandardOutput.ReadToEnd());
                standardError.Append(p.StandardError.ReadToEnd());
            }

            String stdErrOutput = standardError.ToString();
            
            /* If JAVA_TOOL_OPTIONS env var is defined JVM will always print its value to stderr. We filter this line
               in order to catch other valuable error output. */
            string javaToolOptionsWarn = "Picked up JAVA_TOOL_OPTIONS: ";
            stdErrOutput = String.Join("\n", 
                stdErrOutput
                    .Split('\n').Where(s => !s.StartsWith(javaToolOptionsWarn))
            );
            
            if (String.IsNullOrEmpty(standardOutput.ToString())) {
                return "VeraPDF execution failed: Standard output is empty";
            }
            
            VeraPdfReportResult reportResult = GenerateReport(standardOutput.ToString(), dest, true);

            if (reportResult.NonCompliantPdfaCount != 0) {
                return reportResult.MessageResult;
            } else if (!String.IsNullOrEmpty(stdErrOutput) && reportResult.VeraPdfLogs == null) {
                return "The following warnings and errors were logged during validation:" + stdErrOutput;
            } else if (!String.IsNullOrEmpty(reportResult.VeraPdfLogs)) {
                Console.WriteLine("The following warnings and errors were logged during validation:\n" + stdErrOutput);
                return "The following warnings and errors were logged during validation:" + reportResult.VeraPdfLogs;
            }
            
            return reportResult.MessageResult;
        }

        private VeraPdfReportResult GenerateReport(String output, String dest, bool toReportSuccess) {
            VeraPdfReportResult veraPdfReportResult = new VeraPdfReportResult();
            XmlDocument document = new XmlDocument();

            try {
                document.LoadXml(output.Trim());
            }
            catch (XmlException exc) {
                veraPdfReportResult.MessageResult = "VeraPDF verification results parsing failed: " + exc.Message;
                return veraPdfReportResult;
            }

            String reportDest = dest.Substring(0, dest.Length - ".pdf".Length) + ".xml";

            XmlAttributeCollection detailsAttributes = document.GetElementsByTagName("details")[0].Attributes;

            if (!detailsAttributes["failedRules"].Value.Equals("0")
                || !detailsAttributes["failedChecks"].Value.Equals("0")) {
                WriteToFile(output, reportDest);
                veraPdfReportResult.MessageResult = "VeraPDF verification failed. See verification results: "
                                                    + UrlUtil.GetNormalizedFileUriString(reportDest);
                return veraPdfReportResult;
            }
            
            detailsAttributes = document.GetElementsByTagName("validationReports")[0].Attributes;
            veraPdfReportResult.NonCompliantPdfaCount = int.Parse(detailsAttributes["nonCompliant"].InnerText);
            if (veraPdfReportResult.NonCompliantPdfaCount != 0) {
                veraPdfReportResult.MessageResult = "VeraPDF verification failed. See verification results: "
                                                    + UrlUtil.GetNormalizedFileUriString(reportDest);
                return veraPdfReportResult;
            }

            if (toReportSuccess) {
                WriteToFile(output, reportDest);
                Console.WriteLine("VeraPDF verification finished. See verification report: "
                                  + UrlUtil.GetNormalizedFileUriString(reportDest));
                
                var logs = new List<string>();
                XmlNodeList elements = document.GetElementsByTagName("logMessage");
                foreach (XmlElement element in elements) {
                    logs.Add(element.Attributes["level"].Value + ": " + element.InnerText);
                }
                logs.Sort();

                foreach (String log in logs) {
                    veraPdfReportResult.VeraPdfLogs += "\n" + log;
                }
            }

            return veraPdfReportResult;
        }
        
        private void WriteToFile(String output, String reportDest) {
            using (FileStream stream = File.Create(reportDest)) {
                stream.Write(new UTF8Encoding(true).GetBytes(output),
                    0, output.Length);
            }
        }
    }
}
