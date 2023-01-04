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
using System.IO;
using iText.IO.Util;
using iText.Kernel.Pdf.Xobject;
using iText.StyledXmlParser.Exceptions;
using iText.StyledXmlParser.Logs;
using iText.Test;
using iText.Test.Attributes;
using NUnit.Framework;

namespace iText.StyledXmlParser.Resolver.Resource {
    class ResourceResolverTest : ExtendedITextTest {
        private static readonly String baseUri = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework
                                                     .TestContext
                                                     .CurrentContext.TestDirectory) +
                                                 "/resources/itext/styledxmlparser/resolver/retrieveStreamTest/";

        private static readonly String bLogoIncorrect =
            "data:image/png;base,iVBORw0KGgoAAAANSUhEUgAAAVoAAAAxCAMAAACsy5FpAAAABGdBTUEAALGPC/xhBQAAAAFzUkdCAK7OHOkAAAAqUExURQAAAPicJAdJdQdJdQdJdficJjBUbPicJgdJdQdJdficJficJQdJdficJlrFe50AAAAMdFJOUwCBe8I/Phe+65/saIJg0K4AAAMOSURBVHja7ZvbmqsgDIU5Bo/v/7q7/WZXsQYNuGy1muuZFH7DIiSglFLU6pZUbGQQNvXpNcC4caoNRvNxOuDUdf80HXk3VYewKp516DHWxuOc/0ye/U00duAwU+/qkWzfh9F9hzIHJxuzNa+fsa4I7Ihx+H+qUFN/sKVhzP7lH+a+qwY1gJHtmwFDPBHK1wLLjLOGTb2jIWhHScAF7RgOGod2CAGTFB8J2JodJ3Dq5kNow95oH3BdtsjGHE6LVu+P9iG5UlVwNjXOndGeRWuZEBBJLtWcMMK11nFoDfDL4TOEMUu0K/leIpNNpUrYFVsrDi2Mbb1DXqv5PV4quWzKHikJKq99utTsoI1dsMjBkr2dctoAMO3XQS2ogrNrJ5vH1OvtU6/ddIPR0k1g9K++bcSKo6Htf8wbdxpK2rnRigJRqAU3WiEylzzVlubCF0TLb/pTyZXH9o1WoKLVoKK8yBbUHS6IdjksZYpxo82WXIzIXhptYtmDRPbQaDXiPBZaaQl26ZBI6pfQ+gZ00A3CxkH6COo2rIwjom12KM/IJRehBUdF2wLrtUWS+56P/Q7aPUrheYnYRpE9LtrwSbSp7cxuJnv1qCWzk9AeEy3t0MAp2ccq93NogWHry3QWowqHPDK0mPSr8aXZAWQzO+hB17ebb9P5ZbDCu2obJPeiNQQWbAUse10VbbKqSLm9yRutQGT/8wO0G6+LdvV2Aaq0eDW0kmI3SHKvhZZkESnoTd5o5SIr+gb0A2g9wGQi67KUw5wdLajNEHymyCqo5B4RLawWHp10XcEC528suBOjJVwDZ2iOca9lBNsSl4jZE6Ntd6jXmtKVzeiIOy/aDzwTydmPZpJrzov2A89EsrKod8mVoq1y0LbsE02Zf/sVQSAObXa5ZSq5UkGoZw9LlqwRNkai5ZT7rRXyHkJgQqioSBipgjhGHPdMYy3hbLx8UDbDPTatndyeeW1HpaXtodxYyUO+zmoDUWjeUnHRB7d5E/KQnazRs0VdbWjI/EluloPnb26+KXIGI+e+7CBt/wAetDeCKwxY6QAAAABJRU5ErkJggg==";

        private static readonly String bLogoCorruptedData =
            "data:image/png;base64,,,iVBORw0KGgoAAAANSUhEUgAAAVoAAAAxCAMAAACsy5FpAAAABGdBTUEAALGPC/xhBQAAAAFzUkdCAK7OHOkAAAAqUExURQAAAPicJAdJdQdJdQdJdficJjBUbPicJgdJdQdJdficJficJQdJdficJlrFe50AAAAMdFJOUwCBe8I/Phe+65/saIJg0K4AAAMOSURBVHja7ZvbmqsgDIU5Bo/v/7q7/WZXsQYNuGy1muuZFH7DIiSglFLU6pZUbGQQNvXpNcC4caoNRvNxOuDUdf80HXk3VYewKp516DHWxuOc/0ye/U00duAwU+/qkWzfh9F9hzIHJxuzNa+fsa4I7Ihx+H+qUFN/sKVhzP7lH+a+qwY1gJHtmwFDPBHK1wLLjLOGTb2jIWhHScAF7RgOGod2CAGTFB8J2JodJ3Dq5kNow95oH3BdtsjGHE6LVu+P9iG5UlVwNjXOndGeRWuZEBBJLtWcMMK11nFoDfDL4TOEMUu0K/leIpNNpUrYFVsrDi2Mbb1DXqv5PV4quWzKHikJKq99utTsoI1dsMjBkr2dctoAMO3XQS2ogrNrJ5vH1OvtU6/ddIPR0k1g9K++bcSKo6Htf8wbdxpK2rnRigJRqAU3WiEylzzVlubCF0TLb/pTyZXH9o1WoKLVoKK8yBbUHS6IdjksZYpxo82WXIzIXhptYtmDRPbQaDXiPBZaaQl26ZBI6pfQ+gZ00A3CxkH6COo2rIwjom12KM/IJRehBUdF2wLrtUWS+56P/Q7aPUrheYnYRpE9LtrwSbSp7cxuJnv1qCWzk9AeEy3t0MAp2ccq93NogWHry3QWowqHPDK0mPSr8aXZAWQzO+hB17ebb9P5ZbDCu2obJPeiNQQWbAUse10VbbKqSLm9yRutQGT/8wO0G6+LdvV2Aaq0eDW0kmI3SHKvhZZkESnoTd5o5SIr+gb0A2g9wGQi67KUw5wdLajNEHymyCqo5B4RLawWHp10XcEC528suBOjJVwDZ2iOca9lBNsSl4jZE6Ntd6jXmtKVzeiIOy/aDzwTydmPZpJrzov2A89EsrKod8mVoq1y0LbsE02Zf/sVQSAObXa5ZSq5UkGoZw9LlqwRNkai5ZT7rRXyHkJgQqioSBipgjhGHPdMYy3hbLx8UDbDPTatndyeeW1HpaXtodxYyUO+zmoDUWjeUnHRB7d5E/KQnazRs0VdbWjI/EluloPnb26+KXIGI+e+7CBt/wAetDeCKwxY6QAAAABJRU5ErkJggg==";

        private static readonly String bLogo =
            "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAVoAAAAxCAMAAACsy5FpAAAABGdBTUEAALGPC/xhBQAAAAFzUkdCAK7OHOkAAAAqUExURQAAAPicJAdJdQdJdQdJdficJjBUbPicJgdJdQdJdficJficJQdJdficJlrFe50AAAAMdFJOUwCBe8I/Phe+65/saIJg0K4AAAMOSURBVHja7ZvbmqsgDIU5Bo/v/7q7/WZXsQYNuGy1muuZFH7DIiSglFLU6pZUbGQQNvXpNcC4caoNRvNxOuDUdf80HXk3VYewKp516DHWxuOc/0ye/U00duAwU+/qkWzfh9F9hzIHJxuzNa+fsa4I7Ihx+H+qUFN/sKVhzP7lH+a+qwY1gJHtmwFDPBHK1wLLjLOGTb2jIWhHScAF7RgOGod2CAGTFB8J2JodJ3Dq5kNow95oH3BdtsjGHE6LVu+P9iG5UlVwNjXOndGeRWuZEBBJLtWcMMK11nFoDfDL4TOEMUu0K/leIpNNpUrYFVsrDi2Mbb1DXqv5PV4quWzKHikJKq99utTsoI1dsMjBkr2dctoAMO3XQS2ogrNrJ5vH1OvtU6/ddIPR0k1g9K++bcSKo6Htf8wbdxpK2rnRigJRqAU3WiEylzzVlubCF0TLb/pTyZXH9o1WoKLVoKK8yBbUHS6IdjksZYpxo82WXIzIXhptYtmDRPbQaDXiPBZaaQl26ZBI6pfQ+gZ00A3CxkH6COo2rIwjom12KM/IJRehBUdF2wLrtUWS+56P/Q7aPUrheYnYRpE9LtrwSbSp7cxuJnv1qCWzk9AeEy3t0MAp2ccq93NogWHry3QWowqHPDK0mPSr8aXZAWQzO+hB17ebb9P5ZbDCu2obJPeiNQQWbAUse10VbbKqSLm9yRutQGT/8wO0G6+LdvV2Aaq0eDW0kmI3SHKvhZZkESnoTd5o5SIr+gb0A2g9wGQi67KUw5wdLajNEHymyCqo5B4RLawWHp10XcEC528suBOjJVwDZ2iOca9lBNsSl4jZE6Ntd6jXmtKVzeiIOy/aDzwTydmPZpJrzov2A89EsrKod8mVoq1y0LbsE02Zf/sVQSAObXa5ZSq5UkGoZw9LlqwRNkai5ZT7rRXyHkJgQqioSBipgjhGHPdMYy3hbLx8UDbDPTatndyeeW1HpaXtodxYyUO+zmoDUWjeUnHRB7d5E/KQnazRs0VdbWjI/EluloPnb26+KXIGI+e+7CBt/wAetDeCKwxY6QAAAABJRU5ErkJggg==";

        // Constructor tests block

        [Test]
        public virtual void ConstructorWithBaseUriTest() {
            ResourceResolver resolver = new ResourceResolver(null);

            UriResolver uriResolver = new UriResolver("");
            String resolveUrl = resolver.ResolveAgainstBaseUri("").ToString();
            String expectedUrl = uriResolver.ResolveAgainstBaseUri("").ToString();

            Assert.AreEqual(resolveUrl, expectedUrl);
            Assert.AreEqual(typeof(DefaultResourceRetriever), resolver.GetRetriever().GetType());
        }

        [Test]
        public virtual void ConstructorWithBaseUriAndResourceRetrieverTest() {
            ResourceResolver resolver = new ResourceResolver("folder", new CustomResourceRetriever());

            UriResolver uriResolver = new UriResolver("folder");
            String resolveUrl = resolver.ResolveAgainstBaseUri("").ToString();
            String expectedUrl = uriResolver.ResolveAgainstBaseUri("").ToString();

            Assert.AreEqual(resolveUrl, expectedUrl);
            Assert.AreEqual(typeof(CustomResourceRetriever), resolver.GetRetriever().GetType());
        }

        class CustomResourceRetriever : DefaultResourceRetriever {

        }

        // Malformed resource name tests block

        [Test]
        [LogMessage(StyledXmlParserLogMessageConstant.UNABLE_TO_RETRIEVE_STREAM_WITH_GIVEN_BASE_URI,
            LogLevel = LogLevelConstants.ERROR)]
        public virtual void RetrieveStreamByMalformedResourceNameTest() {
            String fileName = "resourceResolverTest .png";
            ResourceResolver resourceResolver = new ResourceResolver(baseUri);
            byte[] bytes = resourceResolver.RetrieveBytesFromResource(fileName);
            Assert.Null(bytes);
        }

        [Test]
        [LogMessage(StyledXmlParserLogMessageConstant.UNABLE_TO_RETRIEVE_STREAM_WITH_GIVEN_BASE_URI,
            LogLevel = LogLevelConstants.ERROR)]
        public virtual void RetrieveStyleSheetByMalformedResourceNameTest() {
            String fileName = "retrieveStyl eSheetTest.css";
            ResourceResolver resourceResolver = new ResourceResolver(baseUri);
            resourceResolver.RetrieveResourceAsInputStream(fileName);
        }

        [Test]
        [LogMessage(StyledXmlParserLogMessageConstant.UNABLE_TO_RETRIEVE_STREAM_WITH_GIVEN_BASE_URI,
            LogLevel = LogLevelConstants.ERROR)]
        public virtual void RetrieveResourceAsInputStreamByMalformedResourceNameTest() {
            String fileName = "retrieveStyl eSheetTest.css";
            ResourceResolver resourceResolver = new ResourceResolver(baseUri);
            Stream stream = resourceResolver.RetrieveResourceAsInputStream(fileName);
            Assert.Null(stream);
        }

        [Test]
        [LogMessage(StyledXmlParserLogMessageConstant.UNABLE_TO_RETRIEVE_STREAM_WITH_GIVEN_BASE_URI,
            LogLevel = LogLevelConstants.ERROR)]
        public virtual void RetrieveBytesFromResourceByMalformedResourceNameTest() {
            String fileName = "retrieveStyl eSheetTest.css";
            ResourceResolver resourceResolver = new ResourceResolver(baseUri);
            byte[] bytes = resourceResolver.RetrieveBytesFromResource(fileName);
            Assert.Null(bytes);
        }

        [Test]
        [LogMessage(StyledXmlParserLogMessageConstant.UNABLE_TO_RETRIEVE_IMAGE_WITH_GIVEN_BASE_URI,
            LogLevel = LogLevelConstants.ERROR)]
        public virtual void RetrieveImageExtendedByMalformedResourceNameTest() {
            String fileName = "retrieveStyl eSheetTest.css";
            ResourceResolver resourceResolver = new ResourceResolver(baseUri);
            PdfXObject pdfXObject = resourceResolver.RetrieveImage(fileName);
            Assert.Null(pdfXObject);
        }
        
        [Test]
        public virtual void MalformedResourceNameTest07() {
            String fileName = "%23%5B%5D@!$&'()+,;=._~-/styles09.css";
            Stream expected = new FileStream(baseUri + "#[]@!$&'()+,;=._~-/styles09.css", FileMode.Open, FileAccess.Read);
            ResourceResolver resourceResolver = new ResourceResolver(baseUri);
            Stream stream = resourceResolver.RetrieveResourceAsInputStream(fileName);
            Assert.NotNull(stream);
            Assert.AreEqual(expected.Read(), stream.Read());
        }

        // Boolean method tests block

        [Test]
        public virtual void IsDataSrcTest() {
            Assert.True(ResourceResolver.IsDataSrc(bLogo));
            Assert.True(ResourceResolver.IsDataSrc(bLogoCorruptedData));
            Assert.True(ResourceResolver.IsDataSrc(bLogoIncorrect));
            Assert.False(ResourceResolver.IsDataSrc("https://data.com/data"));
        }

        // Retrieve pdfXObject tests block

        [Test]
        public virtual void RetrieveImageBase64Test() {
            ResourceResolver resourceResolver = new ResourceResolver(baseUri);
            PdfXObject image = resourceResolver.RetrieveImage(bLogo);
            Assert.NotNull(image);
        }
        
        [Test]
        [LogMessage(StyledXmlParserLogMessageConstant.UNABLE_TO_RETRIEVE_IMAGE_WITH_GIVEN_DATA_URI)]
        public virtual void RetrieveImageIncorrectBase64Test() {
            ResourceResolver resourceResolver = new ResourceResolver(baseUri);
            PdfXObject image = resourceResolver.RetrieveImage(bLogoCorruptedData);
            Assert.Null(image);
        }

        [Test]
        [LogMessage(StyledXmlParserLogMessageConstant.UNABLE_TO_RETRIEVE_IMAGE_WITH_GIVEN_DATA_URI,
            LogLevel = LogLevelConstants.ERROR)]
        public virtual void RetrieveImageCorruptedDataBase64Test() {
            ResourceResolver resourceResolver = new ResourceResolver(baseUri);
            PdfXObject image = resourceResolver.RetrieveImage(bLogoCorruptedData);
            Assert.Null(image);
        }

        [Test]
        [LogMessage(StyledXmlParserLogMessageConstant.UNABLE_TO_RETRIEVE_IMAGE_WITH_GIVEN_BASE_URI,
            LogLevel = LogLevelConstants.ERROR)]
        public virtual void RetrieveImageNullTest() {
            ResourceResolver resourceResolver = new ResourceResolver(baseUri);
            PdfXObject image = resourceResolver.RetrieveImage(null);
            Assert.Null(image);
        }

        [Test]
        public virtual void RetrieveImageTest() {
            String fileName = "resourceResolverTest.png";
            ResourceResolver resourceResolver = new ResourceResolver(baseUri);
            PdfXObject image = resourceResolver.RetrieveImage(fileName);
            Assert.NotNull(image);
        }

        // Retrieve byte array tests block

        [Test]
        public virtual void RetrieveBytesFromResourceBase64Test() {
            ResourceResolver resourceResolver = new ResourceResolver(baseUri);
            byte[] bytes = resourceResolver.RetrieveBytesFromResource(bLogo);
            Assert.NotNull(bytes);
        }

        [Test]
        [LogMessage(StyledXmlParserLogMessageConstant.UNABLE_TO_RETRIEVE_STREAM_WITH_GIVEN_BASE_URI,
            LogLevel = LogLevelConstants.ERROR)]
        public virtual void RetrieveBytesFromResourceIncorrectBase64Test() {
            ResourceResolver resourceResolver = new ResourceResolver(baseUri);
            byte[] bytes = resourceResolver.RetrieveBytesFromResource(bLogoIncorrect);
            Assert.Null(bytes);
        }

        [Test]
        [LogMessage(StyledXmlParserLogMessageConstant.UNABLE_TO_RETRIEVE_STREAM_WITH_GIVEN_BASE_URI,
            LogLevel = LogLevelConstants.ERROR)]
        public virtual void RetrieveBytesFromResourceCorruptedDataBase64Test() {
            ResourceResolver resourceResolver = new ResourceResolver(baseUri);
            byte[] bytes = resourceResolver.RetrieveBytesFromResource(bLogoCorruptedData);
            Assert.Null(bytes);
        }

        [Test]
        public virtual void RetrieveBytesFromResourcePngImageTest() {
            String fileName = "resourceResolverTest.png";
            ResourceResolver resourceResolver = new ResourceResolver(baseUri);
            byte[] expected = File.ReadAllBytes(baseUri + fileName);
            byte[] bytes = resourceResolver.RetrieveBytesFromResource(fileName);
            Assert.NotNull(bytes);
            Assert.AreEqual(expected.Length, bytes.Length);
        }

        [Test]
        public virtual void RetrieveStreamPngImageTest() {
            String fileName = "resourceResolverTest.png";
            ResourceResolver resourceResolver = new ResourceResolver(baseUri);
            byte[] expected = File.ReadAllBytes(baseUri + fileName);
            byte[] stream = resourceResolver.RetrieveBytesFromResource(fileName);
            Assert.NotNull(resourceResolver.RetrieveBytesFromResource(fileName));
            Assert.AreEqual(expected.Length, stream.Length);
        }

        [Test]
        public virtual void RetrieveBytesFromResourceStyleSheetTest() {
            String fileName = "retrieveStyleSheetTest.css";
            ResourceResolver resourceResolver = new ResourceResolver(baseUri);
            byte[] expected = File.ReadAllBytes(baseUri + fileName);
            byte[] bytes = resourceResolver.RetrieveBytesFromResource(fileName);
            Assert.NotNull(bytes);
            Assert.AreEqual(expected.Length, bytes.Length);
        }

        [Test]
        [LogMessage(StyledXmlParserLogMessageConstant.RESOURCE_WITH_GIVEN_URL_WAS_FILTERED_OUT, LogLevel = LogLevelConstants.WARN)]
        public virtual void AttemptToRetrieveBytesFromResourceStyleSheetWithFilterRetrieverTest() {
            String fileName = "retrieveStyleSheetTest.css";
            ResourceResolver resourceResolver = new ResourceResolver(baseUri);
            resourceResolver.SetRetriever(new FilterResourceRetriever());
            byte[] bytes = resourceResolver.RetrieveBytesFromResource(fileName);
            Assert.Null(bytes);
        }
        
        [Test]
        [LogMessage(StyledXmlParserLogMessageConstant.UNABLE_TO_RETRIEVE_IMAGE_WITH_GIVEN_BASE_URI)]
        public virtual void RetrieveImageWrongPathTest() {
            String fileName = "/itextpdf.com/itis.jpg";
            ResourceResolver resourceResolver = new ResourceResolver(baseUri);
            PdfXObject image = resourceResolver.RetrieveImage(fileName);
            Assert.Null(image);
        }
        
        [Test]
        public virtual void RetrieveImageRightPathTest() {
            String fileName = "itextpdf.com/itis.jpg";
            ResourceResolver resourceResolver = new ResourceResolver(baseUri);
            PdfXObject image = resourceResolver.RetrieveImage(fileName);
            Assert.NotNull(image);
        }

        [Test]
        public virtual void RetrieveImagePathWithSpacesTest() {
            String fileName = "retrieveImagePathWithSpaces.jpg";
            ResourceResolver resourceResolver = new ResourceResolver(baseUri + "path with spaces/");
            PdfXObject image = resourceResolver.RetrieveImage(fileName);
            Assert.NotNull(image);
        }

        [Test]
        [LogMessage(StyledXmlParserLogMessageConstant.UNABLE_TO_RETRIEVE_STREAM_WITH_GIVEN_BASE_URI)]
        public virtual void RetrieveBytesMalformedResourceNameTest() {
            String fileName = "resourceResolverTest .png";
            ResourceResolver resourceResolver = new ResourceResolver(baseUri);
            byte[] bytes =resourceResolver.RetrieveBytesFromResource(fileName);
            Assert.Null(bytes);
        }

        [Test]
        public virtual void RetrieveBytesFromResourceWithRetryRetrieverTest() {
            String fileName = "!invalid! StyleSheetName.css";
            ResourceResolver resourceResolver = new ResourceResolver(baseUri, new RetryResourceRetriever(baseUri));
            byte[] expected = File.ReadAllBytes(baseUri + "retrieveStyleSheetTest.css");
            byte[] bytes = resourceResolver.RetrieveBytesFromResource(fileName);
            Assert.NotNull(bytes);
            Assert.AreEqual(expected.Length, bytes.Length);
        }

        [Test]
        [LogMessage(StyledXmlParserLogMessageConstant.UNABLE_TO_RETRIEVE_RESOURCE_WITH_GIVEN_RESOURCE_SIZE_BYTE_LIMIT,
            LogLevel = LogLevelConstants.WARN)]
        public virtual void AttemptToRetrieveBytesFromLocalWithResourceSizeByteLimitTest() {
            String fileName = "retrieveStyleSheetTest.css";
            // retrieveStyleSheetTest.css size is 89 bytes
            IResourceRetriever retriever = new DefaultResourceRetriever().SetResourceSizeByteLimit(88);
            ResourceResolver resourceResolver = new ResourceResolver(baseUri, retriever);
            byte[] bytes = resourceResolver.RetrieveBytesFromResource(fileName);
            Assert.Null(bytes);
        }

        [Test]
        public virtual void RetrieveBytesFromLocalWithResourceSizeByteLimitTest() {
            String fileName = "retrieveStyleSheetTest.css.dat";
            // retrieveStyleSheetTest.css.dat size is 89 bytes
            IResourceRetriever retriever = new DefaultResourceRetriever().SetResourceSizeByteLimit(89);
            ResourceResolver resourceResolver = new ResourceResolver(baseUri, retriever);
            byte[] bytes = resourceResolver.RetrieveBytesFromResource(fileName);
            Assert.NotNull(bytes);
            Assert.AreEqual(((DefaultResourceRetriever) retriever).GetResourceSizeByteLimit(), bytes.Length);
        }

        // Retrieve input stream tests block

        [Test]
        public virtual void AttemptToReadBytesFromLimitedInputStreamTest() {
            String fileName = "retrieveStyleSheetTest.css";
            // retrieveStyleSheetTest.css size is 89 bytes
            IResourceRetriever retriever = new DefaultResourceRetriever().SetResourceSizeByteLimit(40);
            ResourceResolver resourceResolver = new ResourceResolver(baseUri, retriever);
            Stream stream = resourceResolver.RetrieveResourceAsInputStream(fileName);
            for (int i = 0; i < 40; i++) {
                stream.Read();
            } 
            
            Assert.Catch(typeof(ReadingByteLimitException), () => stream.Read());
        }

        [Test]
        public virtual void RetrieveResourceAsInputStreamBase64Test() {
            ResourceResolver resourceResolver = new ResourceResolver(baseUri);
            Stream stream = resourceResolver.RetrieveResourceAsInputStream(bLogo);
            Assert.NotNull(stream);
        }

        [Test]
        public virtual void RetrieveStyleSheetTest() {
            String fileName = "retrieveStyleSheetTest.css";
            Stream expected = new FileStream(baseUri + fileName, FileMode.Open, FileAccess.Read);
            ResourceResolver resourceResolver = new ResourceResolver(baseUri);
            Stream stream = resourceResolver.RetrieveResourceAsInputStream(fileName);
            Assert.NotNull(stream);
            Assert.AreEqual(expected.Read(), stream.Read());
        }

        [Test]
        public virtual void RetrieveResourceAsInputStreamStyleSheetTest() {
            String fileName = "retrieveStyleSheetTest.css";
            Stream expected = new FileStream(baseUri + fileName, FileMode.Open, FileAccess.Read);
            ResourceResolver resourceResolver = new ResourceResolver(baseUri);
            Stream stream = resourceResolver.RetrieveResourceAsInputStream(fileName);
            Assert.NotNull(stream);
            Assert.AreEqual(expected.Read(), stream.Read());
        }

        [Test]
        [LogMessage(StyledXmlParserLogMessageConstant.RESOURCE_WITH_GIVEN_URL_WAS_FILTERED_OUT, LogLevel = LogLevelConstants.WARN)]
        public virtual void AttemptToRetrieveInputStreamWithFilterRetrieverTest() {
            String fileName = "retrieveStyleSheetTest.css";
            ResourceResolver resourceResolver = new ResourceResolver(baseUri);
            resourceResolver.SetRetriever(new FilterResourceRetriever());
            Stream stream = resourceResolver.RetrieveResourceAsInputStream(fileName);
            Assert.Null(stream);
        }

        class FilterResourceRetriever : DefaultResourceRetriever {

            protected internal override bool UrlFilter(Uri url) {
                return url.AbsolutePath.StartsWith("/MyFolderWithUniqName");
            }
        }

        [Test]
        public virtual void RetrieveInputStreamWithRetryRetrieverTest() {
            String fileName = "!invalid! StyleSheetName.css";
            ResourceResolver resourceResolver = new ResourceResolver(baseUri, new RetryResourceRetriever(baseUri));
            Stream expected = new FileStream(baseUri + "retrieveStyleSheetTest.css", FileMode.Open, FileAccess.Read);
            Stream stream = resourceResolver.RetrieveResourceAsInputStream(fileName);
            Assert.NotNull(stream);
            Assert.AreEqual(expected.Read(), stream.Read());
        }

        class RetryResourceRetriever : DefaultResourceRetriever {
            private String baseUri;

            public RetryResourceRetriever(String baseUri) {
                this.baseUri = baseUri;
            }

            public override Stream GetInputStreamByUrl(Uri url) {
                Stream stream = null;
                try {
                    stream = base.GetInputStreamByUrl(url);
                }
                catch (Exception ignored) {
                }

                if (stream == null) {
                    Uri newUrl = new UriResolver(this.baseUri).ResolveAgainstBaseUri("retrieveStyleSheetTest.css");
                    stream = base.GetInputStreamByUrl(newUrl);
                }

                return stream;
            }
        }

        // Absolute path tests block

        [Test]
        public virtual void RetrieveStyleSheetAbsolutePathTest() {
            String fileName = "retrieveStyleSheetTest.css";
            String absolutePath = Path.Combine(baseUri, fileName).ToFile().FullName;

            ResourceResolver resourceResolver = new ResourceResolver(baseUri);
            using (Stream stream = resourceResolver.RetrieveResourceAsInputStream(absolutePath),
                expected = new FileStream(absolutePath, FileMode.Open, FileAccess.Read)) {
                Assert.NotNull(stream);
                Assert.AreEqual(expected.Read(), stream.Read());
            }
        }

        [Test]
        public virtual void RetrieveResourceAsInputStreamAbsolutePathTest() {
            String fileName = "retrieveStyleSheetTest.css";
            String absolutePath = Path.Combine(baseUri, fileName).ToFile().FullName;

            ResourceResolver resourceResolver = new ResourceResolver(baseUri);
            using (Stream stream = resourceResolver.RetrieveResourceAsInputStream(absolutePath),
                expected = new FileStream(absolutePath, FileMode.Open, FileAccess.Read)) {
                Assert.NotNull(stream);
                Assert.AreEqual(expected.Read(), stream.Read());
            }
        }

        [Test]
        public virtual void RetrieveStyleSheetFileUrlTest() {
            String fileName = "retrieveStyleSheetTest.css";
            Uri url = Path.Combine(baseUri, fileName).ToUri().ToUrl();
            String fileUrlString = url.ToExternalForm();

            ResourceResolver resourceResolver = new ResourceResolver(baseUri);
            using (Stream stream = resourceResolver.RetrieveResourceAsInputStream(fileUrlString),
                expected = UrlUtil.OpenStream(url)) {
                Assert.NotNull(stream);
                Assert.AreEqual(expected.Read(), stream.Read());
            }

        }

        [Test]
        public virtual void RetrieveResourceAsInputStreamFileUrlTest() {
            String fileName = "retrieveStyleSheetTest.css";
            Uri url = Path.Combine(baseUri, fileName).ToUri().ToUrl();
            String fileUrlString = url.ToExternalForm();

            ResourceResolver resourceResolver = new ResourceResolver(baseUri);
            using (Stream stream = resourceResolver.RetrieveResourceAsInputStream(fileUrlString),
                expected = UrlUtil.OpenStream(url)) {
                Assert.NotNull(stream);
                Assert.AreEqual(expected.Read(), stream.Read());
            }
        }
    }
}
