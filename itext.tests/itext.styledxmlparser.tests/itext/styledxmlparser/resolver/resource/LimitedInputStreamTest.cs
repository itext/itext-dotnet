/*
This file is part of the iText (R) project.
Copyright (c) 1998-2020 iText Group NV
Authors: iText Software.

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
using System.IO;
using iText.IO.Util;
using iText.Test;

namespace iText.StyledXmlParser.Resolver.Resource {
    public class LimitedInputStreamTest : ExtendedITextTest {
        private readonly String baseUri = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/styledxmlparser/resolver/retrieveStreamTest/";

        [NUnit.Framework.Test]
        public virtual void ReadingByteLimitTest() {
            UriResolver uriResolver = new UriResolver(baseUri);
            Uri url = uriResolver.ResolveAgainstBaseUri("retrieveStyleSheetTest.css.dat");
            // retrieveStyleSheetTest.css.dat size is 89 bytes
            Stream stream = new LimitedInputStream(UrlUtil.OpenStream(url), 100);
            // The user can call the reading methods as many times as he want, and if the
            // stream has been read, then should not throw an ReadingByteLimitException exception
            for (int i = 0; i < 101; i++) {
                stream.Read();
            }
        }

        [NUnit.Framework.Test]
        public virtual void ReadingByteArrayWithLimitOfOneLessThenFileSizeTest() {
            UriResolver uriResolver = new UriResolver(baseUri);
            Uri url = uriResolver.ResolveAgainstBaseUri("retrieveStyleSheetTest.css.dat");
            // retrieveStyleSheetTest.css.dat size is 89 bytes
            Stream stream = new LimitedInputStream(UrlUtil.OpenStream(url), 88);
            byte[] bytes = new byte[100];
            // The first time ReadingByteLimitException will be thrown, but we catch it in InputStream#read(byte[])
            // and return 88 bytes, the second time, in the LimitedInputStream#read() method we will throw
            // ReadingByteLimitException anyway, because readingByteLimit was violated.
            int numOfReadBytes = stream.Read(bytes);
            NUnit.Framework.Assert.AreEqual(88, numOfReadBytes);
            NUnit.Framework.Assert.AreEqual(10, bytes[87]);
            NUnit.Framework.Assert.AreEqual(0, bytes[88]);
            NUnit.Framework.Assert.That(() =>  {
                stream.Read(bytes);
            }
            , NUnit.Framework.Throws.InstanceOf<ReadingByteLimitException>())
;
        }
    }
}
