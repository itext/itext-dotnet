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
using System.Net;
using iText.Test;

namespace iText.StyledXmlParser.Resolver.Resource {
    [NUnit.Framework.Category("IntegrationTest")]
    internal class DefaultResourceRetrieverTest : ExtendedITextTest
    {
        [NUnit.Framework.Test]
        public virtual void RetrieveResourceConnectTimeoutTest()
        {
            bool exceptionThrown = false;
            Uri url = new Uri("http://10.255.255.1/");
            DefaultResourceRetriever resourceRetriever = new DefaultResourceRetriever();
            resourceRetriever.SetConnectTimeout(500);

            try {
                // We check 2 possible exceptions
                resourceRetriever.GetInputStreamByUrl(url);
            } catch(WebException e) {
                exceptionThrown = true;
                // Do not check exception message because it is localized
            } catch(OperationCanceledException e) {
                exceptionThrown = true;
                NUnit.Framework.Assert.AreEqual("the operation was canceled.", e.Message.ToLower());
            }

            NUnit.Framework.Assert.True(exceptionThrown);
        }
    }
}
