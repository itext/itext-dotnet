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
        public virtual void RetrieveResourceConnectTimeoutTest() {
            bool exceptionThrown = false;
            Uri url = new Uri("http://10.255.255.1/");
            DefaultResourceRetriever resourceRetriever = new DefaultResourceRetriever();
            resourceRetriever.SetConnectTimeout(500);

            try {
                // We check 2 possible exceptions
                resourceRetriever.GetInputStreamByUrl(url);
            }
            catch (WebException e) {
                exceptionThrown = true;
                // Do not check exception message because it is localized
            }
            catch (OperationCanceledException e) {
                exceptionThrown = true;
                NUnit.Framework.Assert.AreEqual("the operation was canceled.", StringNormalizer.ToLowerCase(e.Message));
            }

            NUnit.Framework.Assert.True(exceptionThrown);
        }

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
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.RESOURCE_WITH_GIVEN_URL_WAS_FILTERED_OUT, Count = 2)]
        public virtual void FilterOutFilteredResourcesTest() {
            DefaultResourceRetriever resourceRetriever = new DefaultResourceRetrieverTest.FilteredResourceRetriever();
            NUnit.Framework.Assert.IsFalse(resourceRetriever.UrlFilter(new Uri("https://example.com/resource")));
            NUnit.Framework.Assert.IsNull(resourceRetriever.GetInputStreamByUrl(new Uri("https://example.com/resource"
                )));
            NUnit.Framework.Assert.IsNull(resourceRetriever.Get(new Uri("https://example.com/resource"), new byte[0], 
                new Dictionary<String, String>(0)));
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
        public virtual void LoadWithRequestAndHeaders() {
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

        private class FilteredResourceRetriever : DefaultResourceRetriever {
            protected internal override bool UrlFilter(Uri url) {
                return false;
            }
            // Filter all resources
        }
    }
//\endcond
}
