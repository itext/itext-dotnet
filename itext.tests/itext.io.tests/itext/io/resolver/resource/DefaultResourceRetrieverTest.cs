/*
This file is part of the iText (R) project.
Copyright (c) 1998-2026 Apryse Group NV
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
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using iText.Commons.Utils;
using iText.IO.Util;
using iText.Test;
using iText.Test.Attributes;
using System.Net;

namespace iText.IO.Resolver.Resource {
//\cond DO_NOT_DOCUMENT
    [NUnit.Framework.Category("IntegrationTest")]
    internal class DefaultResourceRetrieverTest : ExtendedITextTest {
 
        [NUnit.Framework.Test]
        public virtual void ConnectTimeoutTest() {
            DefaultResourceRetriever resourceRetriever = new DefaultResourceRetriever();
            resourceRetriever.SetConnectTimeout(500);
            NUnit.Framework.Assert.AreEqual(500, resourceRetriever.GetConnectTimeout());
        }

        [NUnit.Framework.Test]
        public virtual void ReadTimeoutTest() {
            DefaultResourceRetriever resourceRetriever = new DefaultResourceRetriever();
            resourceRetriever.SetReadTimeout(500);
            NUnit.Framework.Assert.AreEqual(500, resourceRetriever.GetReadTimeout());
        }

        [NUnit.Framework.Test]
        public virtual void SetResourceByLimitTest() {
            DefaultResourceRetriever resourceRetriever = new DefaultResourceRetriever();
            resourceRetriever.SetResourceSizeByteLimit(1000);
            NUnit.Framework.Assert.AreEqual(1000, resourceRetriever.GetResourceSizeByteLimit());
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.RESOURCE_WITH_GIVEN_URL_WAS_FILTERED_OUT)]
        public virtual void FilterOutFilteredResourcesTest() {
            DefaultResourceRetriever resourceRetriever = new DefaultResourceRetrieverTest.FilteredResourceRetriever();
            NUnit.Framework.Assert.IsFalse(resourceRetriever.UrlFilter(new Uri("https://example.com/resource")));
            NUnit.Framework.Assert.IsNull(resourceRetriever.GetInputStreamByUrl(new Uri("https://example.com/resource"
                )));
        }

        [NUnit.Framework.Test]
        public virtual void LoadGetByteArrayByUrl() {
            // Android-Conversion-Ignore-Test DEVSIX-6459 Some different random connect exceptions on Android
            DefaultResourceRetriever resourceRetriever = new DefaultResourceRetriever();
            byte[] data = resourceRetriever.GetByteArrayByUrl(new Uri("https://itextpdf.com/blog/itext-news-technical-notes/get-excited-itext-8-here"
                ));
            NUnit.Framework.Assert.IsNotNull(data);
            NUnit.Framework.Assert.IsTrue(data.Length > 0);
        }

        [NUnit.Framework.Test]
        public virtual void LoadWithRequestAndHeaders()
        {
            // Android-Conversion-Ignore-Test DEVSIX-6459 Some different random connect exceptions on Android
            DefaultResourceRetriever resourceRetriever = new DefaultResourceRetriever();
            IDictionary<String, String> headers = new Dictionary<String, String>(1);
            headers.Put("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/143.0.0.0 Safari/537.36"
                );
            Stream @is = resourceRetriever.Get(new Uri("https://itextpdf.com/blog/itext-news-technical-notes/get-excited-itext-8-here"
                ), new byte[0], headers);
            byte[] data = StreamUtil.InputStreamToArray(@is);
            NUnit.Framework.Assert.IsNotNull(data);
            NUnit.Framework.Assert.IsTrue(data.Length > 0);
        }



        [NUnit.Framework.Test]
        public virtual void GetInputStreamByUrlWithHeadersTest() {
            DefaultResourceRetrieverTest.TestResource testResource = new DefaultResourceRetrieverTest.TestResource();
            testResource.Start();                   
            while (!testResource.IsStarted() && !testResource.IsFailed()) {
                Thread.Sleep(250);
            }
            NUnit.Framework.Assert.IsNull(testResource.GetException());
            NUnit.Framework.Assert.IsFalse(testResource.IsFailed());
            Uri url = new Uri("http://127.0.0.1:" + testResource.GetPort() + "/");
            DefaultResourceRetriever resourceRetriever = new DefaultResourceRetriever();
            IDictionary<String, String> headers = new Dictionary<String, String>();
            headers.Add("User-Agent", "TEST User Agent");
            resourceRetriever.SetRequestHeaders(headers);
            try {
                resourceRetriever.GetInputStreamByUrl(url);
            }
            catch (Exception) {
                // exception is expected here, but we do not care
            }
            while (testResource.GetException() == null && testResource.GetLastRequest() == null)
            {
                Thread.Sleep(100);
            }
            NUnit.Framework.Assert.IsNull(testResource.GetException());
            NUnit.Framework.Assert.IsTrue(testResource.GetLastRequest().Contains("User-Agent: TEST User Agent"));
        }

        [NUnit.Framework.Test]
        public virtual void LoadWithRequestAndMixedHeadersTest()
        {
            DefaultResourceRetrieverTest.TestResource testResource = new DefaultResourceRetrieverTest.TestResource();
            testResource.Start();
            while (!testResource.IsStarted() && !testResource.IsFailed())
            {
                Thread.Sleep(250);
            }
            NUnit.Framework.Assert.IsNull(testResource.GetException());
            NUnit.Framework.Assert.IsFalse(testResource.IsFailed());
            Uri url = new Uri("http://127.0.0.1:" + testResource.GetPort() + "/");
            DefaultResourceRetriever resourceRetriever = new DefaultResourceRetriever();
            IDictionary<String, String> headers = new Dictionary<String, String>();
            headers.Add("User-Agent", "TEST User Agent");
            headers.Add("TEST-HEADER", "DEFAULT test header");
            resourceRetriever.SetRequestHeaders(headers);

            var getHeaders = new Dictionary<String, String>();
            getHeaders.Add("User-Agent", "TEST User Agent");
            try
            {
                resourceRetriever.GetInputStreamByUrl(url);
            }
            catch (Exception)
            {
                // exception is expected here, but we do not care
            }
            while (testResource.GetException() == null && testResource.GetLastRequest() == null)
            {
                Thread.Sleep(100);
            }
            NUnit.Framework.Assert.IsNull(testResource.GetException());
            NUnit.Framework.Assert.IsTrue(testResource.GetLastRequest().Contains("User-Agent: TEST User Agent"));
        }

        private class FilteredResourceRetriever : DefaultResourceRetriever {
            protected internal override bool UrlFilter(Uri url) {
                return false;
            }
            // Filter all resources
        }

        private class TestResource  {
            private int port = 8000;

            private bool started = false;

            private bool failed = false;

            private String request;
            private Exception exception;
            private int responseWaitTime;

            public TestResource(int responseWaitTime)
            {
                this.responseWaitTime = responseWaitTime;
            }

            public TestResource()
            {
                this.responseWaitTime = 0;
            }

            public virtual int GetPort() {
                return port;
            }

            public virtual bool IsStarted() {
                return started;
            }

            public virtual bool IsFailed() {
                return failed;
            }

            public virtual String GetLastRequest() {
                return request;
            }

            public virtual Exception GetException() {
                return exception;
            }   

            private void StartServer() {
                int tryCount = 0;
                while (!started) {
                    try {
                        using (Socket server = new Socket(AddressFamily.InterNetwork,
                                     SocketType.Stream,
                                     ProtocolType.Tcp))
                        {
                            server.ReceiveTimeout = 20000;
                            server.Bind(new System.Net.IPEndPoint(address: System.Net.IPAddress.Loopback, port));
                            server.Listen(10);                            
                            
                            started = true;
                            using (Socket clientSocket = server.Accept())
                            {
                                if (responseWaitTime > 0)
                                {
                                    Thread.Sleep(responseWaitTime);
                                }
                                String response = "HTTP/1.1 OK OK\r\nContent-Type: text/html; charset=UTF-8\r\n\r\n" 
                                    + "<!DOCTYPE html>\n" 
                                    + "<html>\n"
                                    + "<body>\n" 
                                    + "\n" 
                                    + "</body>\n" 
                                    + "</html>\n";                                
                                clientSocket.Send(Encoding.UTF8.GetBytes(response));
                                try
                                {
                                    int attempt = 0;
                                    while (attempt < 10 && request == null)
                                    {
                                        attempt++;
                                        if (clientSocket.Available > 0)
                                        {
                                            byte[] buff = new byte[clientSocket.Available];
                                            clientSocket.Receive(buff);
                                            request = iText.Commons.Utils.JavaUtil.GetStringForBytes(buff, System.Text.Encoding.UTF8);
                                        }
                                        else
                                        {
                                            Thread.Sleep(100);
                                        }
                                    }

                                    if (request == null)
                                    {
                                        exception = new InvalidOperationException("No request data available");
                                    }
                                }
                                catch (Exception e)
                                {
                                    exception = e;
                                    request = e.ToString();
                                }                                
                            }
                        }
                    }
                    catch (Exception ex) {
                        if (tryCount > 100) {
                            failed = true;
                            exception = ex;
                            throw;
                        }
                        port++;
                    }
                }
            }

            internal void Start()
            {
                Thread thread = new Thread(StartServer);
                thread.IsBackground = false;
                thread.Start();
            }
        }
    }
//\endcond
}
