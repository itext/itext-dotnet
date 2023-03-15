/*
    This file is part of the iText (R) project.
    Copyright (c) 1998-2023 Apryse Group NV
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
