/*

This file is part of the iText (R) project.
Copyright (c) 1998-2020 iText Group NV
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
using System.IO;
using iText.IO.Util;
using iText.Kernel.Pdf.Xobject;
using iText.Test;
using iText.Test.Attributes;

namespace iText.StyledXmlParser.Resolver.Resource {
    class ResourceResolverTest : ExtendedITextTest {
        public static readonly String baseUri = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
           .CurrentContext.TestDirectory) + "/resources/itext/styledxmlparser/resolver/retrieveStreamTest/";

         private static readonly String bLogoIncorrect = "data:image/png;base,iVBORw0KGgoAAAANSUhEUgAAAVoAAAAxCAMAAACsy5FpAAAABGdBTUEAALGPC/xhBQAAAAFzUkdCAK7OHOkAAAAqUExURQAAAPicJAdJdQdJdQdJdficJjBUbPicJgdJdQdJdficJficJQdJdficJlrFe50AAAAMdFJOUwCBe8I/Phe+65/saIJg0K4AAAMOSURBVHja7ZvbmqsgDIU5Bo/v/7q7/WZXsQYNuGy1muuZFH7DIiSglFLU6pZUbGQQNvXpNcC4caoNRvNxOuDUdf80HXk3VYewKp516DHWxuOc/0ye/U00duAwU+/qkWzfh9F9hzIHJxuzNa+fsa4I7Ihx+H+qUFN/sKVhzP7lH+a+qwY1gJHtmwFDPBHK1wLLjLOGTb2jIWhHScAF7RgOGod2CAGTFB8J2JodJ3Dq5kNow95oH3BdtsjGHE6LVu+P9iG5UlVwNjXOndGeRWuZEBBJLtWcMMK11nFoDfDL4TOEMUu0K/leIpNNpUrYFVsrDi2Mbb1DXqv5PV4quWzKHikJKq99utTsoI1dsMjBkr2dctoAMO3XQS2ogrNrJ5vH1OvtU6/ddIPR0k1g9K++bcSKo6Htf8wbdxpK2rnRigJRqAU3WiEylzzVlubCF0TLb/pTyZXH9o1WoKLVoKK8yBbUHS6IdjksZYpxo82WXIzIXhptYtmDRPbQaDXiPBZaaQl26ZBI6pfQ+gZ00A3CxkH6COo2rIwjom12KM/IJRehBUdF2wLrtUWS+56P/Q7aPUrheYnYRpE9LtrwSbSp7cxuJnv1qCWzk9AeEy3t0MAp2ccq93NogWHry3QWowqHPDK0mPSr8aXZAWQzO+hB17ebb9P5ZbDCu2obJPeiNQQWbAUse10VbbKqSLm9yRutQGT/8wO0G6+LdvV2Aaq0eDW0kmI3SHKvhZZkESnoTd5o5SIr+gb0A2g9wGQi67KUw5wdLajNEHymyCqo5B4RLawWHp10XcEC528suBOjJVwDZ2iOca9lBNsSl4jZE6Ntd6jXmtKVzeiIOy/aDzwTydmPZpJrzov2A89EsrKod8mVoq1y0LbsE02Zf/sVQSAObXa5ZSq5UkGoZw9LlqwRNkai5ZT7rRXyHkJgQqioSBipgjhGHPdMYy3hbLx8UDbDPTatndyeeW1HpaXtodxYyUO+zmoDUWjeUnHRB7d5E/KQnazRs0VdbWjI/EluloPnb26+KXIGI+e+7CBt/wAetDeCKwxY6QAAAABJRU5ErkJggg==";

        private static readonly String bLogoCorruptedData = "data:image/png;base64,,,iVBORw0KGgoAAAANSUhEUgAAAVoAAAAxCAMAAACsy5FpAAAABGdBTUEAALGPC/xhBQAAAAFzUkdCAK7OHOkAAAAqUExURQAAAPicJAdJdQdJdQdJdficJjBUbPicJgdJdQdJdficJficJQdJdficJlrFe50AAAAMdFJOUwCBe8I/Phe+65/saIJg0K4AAAMOSURBVHja7ZvbmqsgDIU5Bo/v/7q7/WZXsQYNuGy1muuZFH7DIiSglFLU6pZUbGQQNvXpNcC4caoNRvNxOuDUdf80HXk3VYewKp516DHWxuOc/0ye/U00duAwU+/qkWzfh9F9hzIHJxuzNa+fsa4I7Ihx+H+qUFN/sKVhzP7lH+a+qwY1gJHtmwFDPBHK1wLLjLOGTb2jIWhHScAF7RgOGod2CAGTFB8J2JodJ3Dq5kNow95oH3BdtsjGHE6LVu+P9iG5UlVwNjXOndGeRWuZEBBJLtWcMMK11nFoDfDL4TOEMUu0K/leIpNNpUrYFVsrDi2Mbb1DXqv5PV4quWzKHikJKq99utTsoI1dsMjBkr2dctoAMO3XQS2ogrNrJ5vH1OvtU6/ddIPR0k1g9K++bcSKo6Htf8wbdxpK2rnRigJRqAU3WiEylzzVlubCF0TLb/pTyZXH9o1WoKLVoKK8yBbUHS6IdjksZYpxo82WXIzIXhptYtmDRPbQaDXiPBZaaQl26ZBI6pfQ+gZ00A3CxkH6COo2rIwjom12KM/IJRehBUdF2wLrtUWS+56P/Q7aPUrheYnYRpE9LtrwSbSp7cxuJnv1qCWzk9AeEy3t0MAp2ccq93NogWHry3QWowqHPDK0mPSr8aXZAWQzO+hB17ebb9P5ZbDCu2obJPeiNQQWbAUse10VbbKqSLm9yRutQGT/8wO0G6+LdvV2Aaq0eDW0kmI3SHKvhZZkESnoTd5o5SIr+gb0A2g9wGQi67KUw5wdLajNEHymyCqo5B4RLawWHp10XcEC528suBOjJVwDZ2iOca9lBNsSl4jZE6Ntd6jXmtKVzeiIOy/aDzwTydmPZpJrzov2A89EsrKod8mVoq1y0LbsE02Zf/sVQSAObXa5ZSq5UkGoZw9LlqwRNkai5ZT7rRXyHkJgQqioSBipgjhGHPdMYy3hbLx8UDbDPTatndyeeW1HpaXtodxYyUO+zmoDUWjeUnHRB7d5E/KQnazRs0VdbWjI/EluloPnb26+KXIGI+e+7CBt/wAetDeCKwxY6QAAAABJRU5ErkJggg==";

        private static readonly String bLogo = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAVoAAAAxCAMAAACsy5FpAAAABGdBTUEAALGPC/xhBQAAAAFzUkdCAK7OHOkAAAAqUExURQAAAPicJAdJdQdJdQdJdficJjBUbPicJgdJdQdJdficJficJQdJdficJlrFe50AAAAMdFJOUwCBe8I/Phe+65/saIJg0K4AAAMOSURBVHja7ZvbmqsgDIU5Bo/v/7q7/WZXsQYNuGy1muuZFH7DIiSglFLU6pZUbGQQNvXpNcC4caoNRvNxOuDUdf80HXk3VYewKp516DHWxuOc/0ye/U00duAwU+/qkWzfh9F9hzIHJxuzNa+fsa4I7Ihx+H+qUFN/sKVhzP7lH+a+qwY1gJHtmwFDPBHK1wLLjLOGTb2jIWhHScAF7RgOGod2CAGTFB8J2JodJ3Dq5kNow95oH3BdtsjGHE6LVu+P9iG5UlVwNjXOndGeRWuZEBBJLtWcMMK11nFoDfDL4TOEMUu0K/leIpNNpUrYFVsrDi2Mbb1DXqv5PV4quWzKHikJKq99utTsoI1dsMjBkr2dctoAMO3XQS2ogrNrJ5vH1OvtU6/ddIPR0k1g9K++bcSKo6Htf8wbdxpK2rnRigJRqAU3WiEylzzVlubCF0TLb/pTyZXH9o1WoKLVoKK8yBbUHS6IdjksZYpxo82WXIzIXhptYtmDRPbQaDXiPBZaaQl26ZBI6pfQ+gZ00A3CxkH6COo2rIwjom12KM/IJRehBUdF2wLrtUWS+56P/Q7aPUrheYnYRpE9LtrwSbSp7cxuJnv1qCWzk9AeEy3t0MAp2ccq93NogWHry3QWowqHPDK0mPSr8aXZAWQzO+hB17ebb9P5ZbDCu2obJPeiNQQWbAUse10VbbKqSLm9yRutQGT/8wO0G6+LdvV2Aaq0eDW0kmI3SHKvhZZkESnoTd5o5SIr+gb0A2g9wGQi67KUw5wdLajNEHymyCqo5B4RLawWHp10XcEC528suBOjJVwDZ2iOca9lBNsSl4jZE6Ntd6jXmtKVzeiIOy/aDzwTydmPZpJrzov2A89EsrKod8mVoq1y0LbsE02Zf/sVQSAObXa5ZSq5UkGoZw9LlqwRNkai5ZT7rRXyHkJgQqioSBipgjhGHPdMYy3hbLx8UDbDPTatndyeeW1HpaXtodxYyUO+zmoDUWjeUnHRB7d5E/KQnazRs0VdbWjI/EluloPnb26+KXIGI+e+7CBt/wAetDeCKwxY6QAAAABJRU5ErkJggg==";

        [NUnit.Framework.Test]
        [LogMessage(LogMessageConstant.UNABLE_TO_RETRIEVE_STREAM_WITH_GIVEN_BASE_URI, Count = 1)]
        public virtual void malformedResourceNameTest() { 
            String fileName = "resourceResolverTest .png";
            ResourceResolver resourceResolver = new ResourceResolver(baseUri);
            resourceResolver.RetrieveStream(fileName);
        }

        [NUnit.Framework.Test]
        public virtual void ResourceResolverConstructorTest() {
            ResourceResolver rr = new ResourceResolver(null);
            UriResolver ur = new UriResolver("");
            String rrUrl = rr.ResolveAgainstBaseUri("").ToString();
            String urUrl = ur.ResolveAgainstBaseUri("").ToString();
            NUnit.Framework.Assert.AreEqual(rrUrl, urUrl);
        }
        
        [NUnit.Framework.Test]
        public virtual void malformedResourceNameTest1() {
            NUnit.Framework.Assert.That(() => {
                String fileName = "retrieveStyl eSheetTest.css";
                ResourceResolver resourceResolver = new ResourceResolver(baseUri);
                resourceResolver.RetrieveStyleSheet(fileName);
            }
            , NUnit.Framework.Throws.TypeOf<FileNotFoundException>());
            ;
        }

        [NUnit.Framework.Test]
        public void IsDataSrcCheckTest() {
            ResourceResolver resourceResolver = new ResourceResolver(baseUri);
            NUnit.Framework.Assert.True(resourceResolver.IsDataSrc(bLogoCorruptedData));
            NUnit.Framework.Assert.True(resourceResolver.IsDataSrc(bLogoIncorrect));
            NUnit.Framework.Assert.False(resourceResolver.IsDataSrc("https://data.com/data"));
        }
        
        [NUnit.Framework.Test]
        public void IncorrectBase64Test() {
            ResourceResolver resourceResolver = new ResourceResolver(baseUri);
            PdfXObject pdfXObject = resourceResolver.TryResolveBase64ImageSource(bLogoIncorrect);
            NUnit.Framework.Assert.Null(pdfXObject);
        }
        
        [NUnit.Framework.Test]
        public virtual void RetrieveAsStreamBase64Test() {
            ResourceResolver resourceResolver = new ResourceResolver(baseUri);
            Stream stream = resourceResolver.RetrieveResourceAsInputStream(bLogo);
            NUnit.Framework.Assert.NotNull(stream);
        }

        [NUnit.Framework.Test]
        public void retrieveBytesTest()
        {
            String fileName = "resourceResolverTest.png";
            byte[] expected = File.ReadAllBytes(baseUri + fileName);
            ResourceResolver resourceResolver = new ResourceResolver(baseUri);
            byte[] stream = resourceResolver.RetrieveStream("resourceResolverTest.png");
            NUnit.Framework.Assert.NotNull(stream);
            NUnit.Framework.Assert.AreEqual(expected.Length, stream.Length);
        }

        [NUnit.Framework.Test]
        public virtual void RetrieveStreamTest() {
            String fileName = "resourceResolverTest.png";
            byte[] expected = File.ReadAllBytes(baseUri + fileName);
            ResourceResolver resourceResolver = new ResourceResolver(baseUri);
            byte[] stream = resourceResolver.RetrieveStream("resourceResolverTest.png");
            NUnit.Framework.Assert.NotNull(stream);
            NUnit.Framework.Assert.AreEqual(expected.Length, stream.Length);
        }

        [NUnit.Framework.Test]
        public virtual void RetrieveImageTest() {
            String fileName = "resourceResolverTest.png";
            ResourceResolver resourceResolver = new ResourceResolver(baseUri);
            PdfImageXObject image = resourceResolver.RetrieveImage(fileName);
            NUnit.Framework.Assert.NotNull(image);
            NUnit.Framework.Assert.True(image.IdentifyImageFileExtension().EqualsIgnoreCase("png"));
        }

        [NUnit.Framework.Test]
        [LogMessage(LogMessageConstant.UNABLE_TO_RETRIEVE_STREAM_WITH_GIVEN_BASE_URI, Count = 1)]
        public void RetrieveBytesMalformedResourceNameTest() {
            String fileName = "resourceResolverTest .png";
            ResourceResolver resourceResolver = new ResourceResolver(baseUri);
            byte[] bytes =resourceResolver.RetrieveBytesFromResource(fileName);
            NUnit.Framework.Assert.Null(bytes);
        }

        [NUnit.Framework.Test]
        public virtual void RetrieveStyleSheetTest() {
            string fileName = "retrieveStyleSheetTest.css";

            Stream expected = new FileStream(baseUri + fileName, FileMode.Open, FileAccess.Read);
            ResourceResolver resourceResolver = new ResourceResolver(baseUri);
            Stream stream = resourceResolver.RetrieveStyleSheet("retrieveStyleSheetTest.css");

            NUnit.Framework.Assert.NotNull(stream);
            NUnit.Framework.Assert.AreEqual(expected.Read(), stream.Read());
        }

        [NUnit.Framework.Test]
        [LogMessage(LogMessageConstant.UNABLE_TO_RETRIEVE_IMAGE_WITH_GIVEN_BASE_URI, Count = 1)]
        public virtual void RetrieveImageExtendedNullTest() {
            ResourceResolver resourceResolver = new ResourceResolver(baseUri);
            PdfXObject image = resourceResolver.RetrieveImageExtended(null);
            NUnit.Framework.Assert.Null(image);
        }

        [NUnit.Framework.Test]
        public virtual void RetrieveImageExtendedBase64Test() {
            ResourceResolver resourceResolver = new ResourceResolver(baseUri);
            PdfXObject image = resourceResolver.RetrieveImageExtended(bLogo);
            NUnit.Framework.Assert.NotNull(image);
        }

        [NUnit.Framework.Test]
        [LogMessage(LogMessageConstant.UNABLE_TO_RETRIEVE_IMAGE_WITH_GIVEN_BASE_URI, Count = 1)]
        public virtual void RetrieveImageExtendedIncorrectBase64Test() {
            ResourceResolver resourceResolver = new ResourceResolver(baseUri);
            PdfXObject image = resourceResolver.RetrieveImageExtended(bLogoCorruptedData);
            NUnit.Framework.Assert.Null(image);
        }
        
        [NUnit.Framework.Test]
        public virtual void absolutePathTest() {
            //TODO check this test with on linux or mac with mono!
            String fileName = "retrieveStyleSheetTest.css";
            String absolutePath = UrlUtil.ToNormalizedURI(baseUri).AbsolutePath + fileName;
            Stream expected = new FileStream(absolutePath, FileMode.Open, FileAccess.Read);

            ResourceResolver resourceResolver = new ResourceResolver(baseUri);
            Stream stream = resourceResolver.RetrieveStyleSheet(absolutePath);
            NUnit.Framework.Assert.NotNull(stream);
            NUnit.Framework.Assert.AreEqual(expected.Read(), stream.Read());
        }
        
        [NUnit.Framework.Test]
        public virtual void absolutePathTest2() {
            //TODO check this test with on linux or mac with mono!
            String fileName = "retrieveStyleSheetTest.css";
            String absolutePath = UrlUtil.ToNormalizedURI(baseUri) + fileName;
            //this constructor will fail.
            //Stream expected = new FileStream(absolutePath, FileMode.Open, FileAccess.Read);
        
            ResourceResolver resourceResolver = new ResourceResolver(baseUri);
            Stream stream = resourceResolver.RetrieveStyleSheet(absolutePath);
            NUnit.Framework.Assert.NotNull(stream);
            //NUnit.Framework.Assert.AreEqual(expected.Read(), stream.Read());
        }
        
        [NUnit.Framework.Test]
        public void IsImageTypeSupportedTest() {
            ResourceResolver resourceResolver = new ResourceResolver(baseUri);
            String fileName = "resourceResolverTest.png";
            bool res = resourceResolver.IsImageTypeSupportedByImageDataFactory(fileName);
            NUnit.Framework.Assert.True(res);
            res = resourceResolver.IsImageTypeSupportedByImageDataFactory("test.txt");
            NUnit.Framework.Assert.False(res);
        }

        [NUnit.Framework.Test]
        public void IsImageTypeSupportedMalformedURLTest() {
            ResourceResolver resourceResolver = new ResourceResolver(baseUri);
            bool res = resourceResolver.IsImageTypeSupportedByImageDataFactory("htt://test.png");
            NUnit.Framework.Assert.False(res);
            res = resourceResolver.IsImageTypeSupportedByImageDataFactory("htt://test.png");
            NUnit.Framework.Assert.False(res);
        }
        
        
    }
}
