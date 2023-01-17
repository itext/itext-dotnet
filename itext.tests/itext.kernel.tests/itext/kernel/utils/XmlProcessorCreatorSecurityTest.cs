/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: iText Software.

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
using System.Text;
using System.Xml;
using iText.Kernel.Exceptions;
using iText.Test;
using NUnit.Framework;

namespace iText.Kernel.Utils
{
    public class XmlProcessorCreatorSecurityTest : ExtendedITextTest
    {
        private const String XML_WITHOUT_DTD = "<?xml version=\"1.0\"?>\n" + "<employees>\n" +
                                               "  <employee>Artem B</employee>\n"
                                               + "  <employee>Nikita K</employee>\n" + "</employees>";

        private const String XML_WITH_DTD = "<?xml version=\"1.0\"?>\n" + "<!DOCTYPE employees [\n" +
                                            "  <!ELEMENT employees (employee)*>\n"
                                            + "  <!ELEMENT employee (#PCDATA)>\n" + "]>\n" + "<employees>\n" +
                                            "  <employee>Artem B</employee>\n"
                                            + "  <employee>Nikita K</employee>\n" + "</employees>";

        private const String XML_WITH_EMPTY_DTD = "<?xml version=\"1.0\"?>\n" + "<!DOCTYPE>\n" + "<employees>\n" +
                                                  "  <employee>Artem B</employee>\n" +
                                                  "  <employee>Nikita K</employee>\n" + "</employees>";

        private const String XML_WITH_INTERNAL_ENTITY = "<?xml version=\"1.0\"?>\n" + "<!DOCTYPE employees [\n" +
                                                        "  <!ELEMENT employees (employee)*>\n" +
                                                        "  <!ELEMENT employee (#PCDATA)>\n" +
                                                        "  <!ENTITY companyname \"Company\">\n"
                                                        + "]>\n" + "<employees>\n" +
                                                        "  <employee>Artem B &companyname;</employee>\n" +
                                                        "  <employee>Nikita K &companyname;</employee>\n"
                                                        + "</employees>";

        private const String XML_WITH_XXE = "<?xml version=\"1.0\"?>\n" + "<!DOCTYPE employees [\n" +
                                            "  <!ELEMENT employees (employee)*>\n"
                                            + "  <!ELEMENT employee (#PCDATA)>\n" + "  <!ENTITY xxe SYSTEM \"{0}\">" +
                                            "]>\n" + "<employees>\n" +
                                            "  <employee>Artem B &xxe;</employee>\n" +
                                            "  <employee>Nikita K &xxe;</employee>\n" + "</employees>";

        [SetUp]
        public virtual void ResetXmlParserFactoryToDefault()
        {
            XmlProcessorCreator.SetXmlParserFactory(null);
        }

        [Test]
        public virtual void XmlWithoutDtdTest()
        {
            XmlDocument document = new XmlDocument();
            Assert.IsNull(document.FirstChild);
            using (Stream inputStream = new MemoryStream(XML_WITHOUT_DTD.GetBytes(Encoding.UTF8)))
            {
                document.Load(XmlProcessorCreator.CreateSafeXmlReader(inputStream));
            }

            Assert.IsNotNull(document.FirstChild);
        }

        [Test]
        public virtual void XmlWithDtdTest()
        {
            XmlDocument document = new XmlDocument();
            using (Stream inputStream = new MemoryStream(XML_WITH_DTD.GetBytes(Encoding.UTF8)))
            {
                Exception e = Assert.Catch(typeof(XmlException), () => document.Load(XmlProcessorCreator.CreateSafeXmlReader(inputStream)));
                Assert.AreEqual(ExceptionTestUtil.GetDoctypeIsDisallowedExceptionMessage(), e.Message);
            }
        }

        [Test]
        public virtual void XmlWithDtdTextReaderTest()
        {
            XmlDocument document = new XmlDocument();
            using (Stream inputStream = new MemoryStream(XML_WITH_DTD.GetBytes(Encoding.UTF8)))
            {
                Exception e = Assert.Catch(typeof(XmlException), () => document.Load(XmlProcessorCreator.CreateSafeXmlReader(new StreamReader(inputStream))));
                Assert.AreEqual(ExceptionTestUtil.GetDoctypeIsDisallowedExceptionMessage(), e.Message);
            }
        }

        [Test]
        public virtual void XmlWithEmptyDtdTest()
        {
            XmlDocument document = new XmlDocument();
            using (Stream inputStream = new MemoryStream(XML_WITH_EMPTY_DTD.GetBytes(Encoding.UTF8)))
            {
                Exception e = Assert.Catch(typeof(XmlException), () => document.Load(XmlProcessorCreator.CreateSafeXmlReader(inputStream)));
                Assert.AreEqual(ExceptionTestUtil.GetDoctypeIsDisallowedExceptionMessage(), e.Message);
            }
        }

        [Test]
        public virtual void XmlWithInternalEntityTest()
        {
            XmlDocument document = new XmlDocument();
            using (Stream inputStream = new MemoryStream(XML_WITH_INTERNAL_ENTITY.GetBytes(Encoding.UTF8)))
            {
                Exception e = Assert.Catch(typeof(XmlException), () => document.Load(XmlProcessorCreator.CreateSafeXmlReader(inputStream)));
                Assert.AreEqual(ExceptionTestUtil.GetDoctypeIsDisallowedExceptionMessage(), e.Message);
            }
        }

        [Test]
        public virtual void XmlWithXxeTest()
        {
            XmlDocument document = new XmlDocument();
            using (Stream inputStream = new MemoryStream(XML_WITH_XXE.GetBytes(Encoding.UTF8)))
            {
                Exception e = Assert.Catch(typeof(XmlException), () => document.Load(XmlProcessorCreator.CreateSafeXmlReader(inputStream)));
                Assert.AreEqual(ExceptionTestUtil.GetDoctypeIsDisallowedExceptionMessage(), e.Message);
            }
        }

        [Test]
        public virtual void XmlWithXxeCustomFactoryTest()
        {
            XmlDocument document = new XmlDocument();
            XmlProcessorCreator.SetXmlParserFactory(new SecurityTestXmlParserFactory());
            using (Stream inputStream = new MemoryStream(XML_WITH_XXE.GetBytes(Encoding.UTF8)))
            {
                Exception e = Assert.Catch(typeof(PdfException), () => document.Load(XmlProcessorCreator.CreateSafeXmlReader(inputStream)));
                Assert.AreEqual("Test message", e.Message);
            }
        }
    }
}
