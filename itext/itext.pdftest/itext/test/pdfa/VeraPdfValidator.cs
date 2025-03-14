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
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using NUnit.Framework;

namespace iText.Test.Pdfa {
    /// <summary>
    /// This class is used to validate PDF files with VeraPDF.
    /// The class can be used in two modes:
    /// * CLI mode - VeraPDF is executed as a separate process and the result is parsed from the output.
    /// To enable CLI mode, set the environment variable ITEXT_VERAPDFVALIDATOR_ENABLE_SERVER to false or do not set it at all.
    ///  The cli command is java -jar VeraPdfValidatorApp.jar cli {pathB64Encoded}
    /// 
    /// * Server mode - VeraPDF is executed as a separate server and the result is parsed from the response.
    /// To enable server mode, set the environment variable ITEXT_VERAPDFVALIDATOR_ENABLE_SERVER to true and
    /// provide the desired port with ITEXT_VERAPDFVALIDATOR_PORT.
    ///
    /// The server has 3 endpoints:
    ///  /api/validate?pathB64={pathB64Encoded} - validates the PDF file at the given path
    ///  /api/status - checks if the server is online
    ///  /api/shutdown - shuts down the server
    ///
    /// The enduser is responsible for shutting down the server after you are done with it.
    /// 
    /// </summary>
    public class VeraPdfValidator {
        private const string ITEXT_VERAPDFVALIDATOR_PORT_VAR = "ITEXT_VERAPDFVALIDATOR_PORT";
        private const string ITEXT_VERAPDFVALIDATOR_ENABLE_SERVER_VAR = "ITEXT_VERAPDFVALIDATOR_ENABLE_SERVER";

       
        private static readonly string PORT_VALUE = Environment.GetEnvironmentVariable(ITEXT_VERAPDFVALIDATOR_PORT_VAR);
        private static readonly string ENABLE_SERVER_VALUE = Environment.GetEnvironmentVariable(ITEXT_VERAPDFVALIDATOR_ENABLE_SERVER_VAR);
        
        // If this is set to true, we will validate the output of the server and the cli to be the same byte for byte
        private static bool VERIFY_SAME_OUTPUT = false;

        private static string GetBaseUri() {
            return "http://localhost:" + GetPort() + "/api/";
        }
        
        private static int GetPort() {
            string port = PORT_VALUE;
            if (port == null || port.Trim().Length == 0) {
                if (!VERIFY_SAME_OUTPUT) {
                    throw new ArgumentException("Env was set to enable server but no port was provided, please set " +
                                                ITEXT_VERAPDFVALIDATOR_PORT_VAR);
                }
                port = "9090";
            }

            return int.Parse(port);
        }


        private bool IsServerOnline() {
            try {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(GetBaseUri() + "status");
                request.Method = "GET";
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                HttpStatusCode statusCode = response.StatusCode;
                response.Close();
                return statusCode == HttpStatusCode.OK;
            }
            catch (WebException e) {
                return false;
            }
        }


        private void StartServer() {
            string command = "java -jar " + TestContext.CurrentContext.TestDirectory +
                             "\\lib\\VeraPdf\\VeraPdfValidatorApp.jar server " + GetPort();
            Process p = new Process();


            SetCorrectExecutor(p, command);

            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.CreateNoWindow = false;
            p.StartInfo.StandardOutputEncoding = Encoding.UTF8;
            p.StartInfo.StandardErrorEncoding = Encoding.UTF8;
            p.Start();
            //wait sometime for server to start
            System.Threading.Thread.Sleep(1000);

            Console.WriteLine("VeraPDF server started on port " + GetPort());
            //add shutdown hook
            AppDomain.CurrentDomain.ProcessExit += (sender, args) => {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(GetBaseUri() + "stop");
                request.Method = "GET";
                request.GetResponse();
            };
        }

        private string TryUseServer(string dest) {
            if (!IsServerOnline()) {
                StartServer();
            }

            string b64EncodeDest = Convert.ToBase64String(Encoding.UTF8.GetBytes(dest));
            HttpWebRequest request =
                (HttpWebRequest)WebRequest.Create(GetBaseUri() + "validate?pathB64=" + b64EncodeDest);
            request.Method = "GET";

            try {
                string responseFromServer;
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse()) {
                    using (Stream dataStream = response.GetResponseStream()) {
                        using (StreamReader reader = new StreamReader(dataStream)) {
                            responseFromServer = reader.ReadToEnd();
                            if (responseFromServer.Length == 0) {
                                return null;
                            }
                        }
                    }
                }

                //Base64 decode the response
                byte[] data = Convert.FromBase64String(responseFromServer);
                return Encoding.UTF8.GetString(data);
            } catch (WebException e) {
                //Sometimes the server is flaky so we will fallback to cli
                Console.WriteLine( "VeraPDF api server execution failed:  " + e.Message);
                Console.WriteLine("Falling back to cli execution");
                return RunCli(dest);
            }
        }

        private static bool ShouldUseServer() {
            string enableServer = ENABLE_SERVER_VALUE;
            return enableServer != null && enableServer.Equals("true");
        }

        public string Validate(string dest) {
            if (!VERIFY_SAME_OUTPUT) {
                String result = ShouldUseServer() ? TryUseServer(dest) : RunCli(dest);
                return result;
            }
            String cliResult = RunCli(dest);
            String serverResult = TryUseServer(dest);
            if (serverResult != cliResult) {
                Assert.AreEqual(cliResult, serverResult);
            }
            return cliResult;
        }

        private static void SetCorrectExecutor(Process p, string command) {
            // Currently we only support windows 
            p.StartInfo = new ProcessStartInfo("cmd", "/c " + command);
        }

        private static string RunCli(string dest) {
            string command = "java -jar " + TestContext.CurrentContext.TestDirectory +
                             "\\lib\\VeraPdf\\VeraPdfValidatorApp.jar cli " +
                             Convert.ToBase64String(Encoding.UTF8.GetBytes(dest));

            Process p = new Process();
            SetCorrectExecutor(p, command);

            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.StandardOutputEncoding = Encoding.UTF8;
            p.StartInfo.StandardErrorEncoding = Encoding.UTF8;
            p.Start();

            p.WaitForExit();
            string output = p.StandardOutput.ReadToEnd();
            string result = output.Length == 0 ? null : output.Trim();
            return result;
        }

        /// <summary>
        ///  Validates PDF file with VeraPdf expecting success.
        /// </summary>
        /// <param name="filePath"> Path to location  </param>
        public void ValidateFailure(String filePath) {
            Assert.NotNull(Validate(filePath));
        }

        /// <summary>
        ///  Validates PDF file with VeraPdf expecting success.
        /// </summary>
        /// <param name="filePath">Path to location  </param>
        /// <param name="expectedWarning">True when you expect a warning</param>
        public void ValidateWarning(String filePath, String expectedWarning) {
            Assert.AreEqual(expectedWarning, Validate(filePath));
        }
    }
}