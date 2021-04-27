/*
This file is part of the iText (R) project.
Copyright (c) 1998-2021 iText Group NV
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
using iText.Kernel;
using iText.Kernel.Utils;
using iText.Kernel.XMP;
using iText.Test;

namespace iText.Kernel.XMP.Impl {
    public class XMPMetaParserSecurityTest : ExtendedITextTest {
        private const String XMP_WITH_XXE = "<?xpacket begin=\"\" id=\"W5M0MpCehiHzreSzNTczkc9d\"?>\n" + "<!DOCTYPE foo [ <!ENTITY xxe SYSTEM \"xxe-data.txt\" > ]>\n"
             + "<x:xmpmeta xmlns:x=\"adobe:ns:meta/\">\n" + "    <rdf:RDF xmlns:rdf=\"http://www.w3.org/1999/02/22-rdf-syntax-ns#\">\n"
             + "        <rdf:Description rdf:about=\"\" xmlns:pdfaid=\"http://www.aiim.org/pdfa/ns/id/\">\n" + "            <pdfaid:part>&xxe;1</pdfaid:part>\n"
             + "            <pdfaid:conformance>B</pdfaid:conformance>\n" + "        </rdf:Description>\n" + "    </rdf:RDF>\n"
             + "</x:xmpmeta>\n" + "<?xpacket end=\"r\"?>";

        private static readonly String DTD_EXCEPTION_MESSAGE = ExceptionTestUtil.GetDoctypeIsDisallowedExceptionMessage
            ();

        [NUnit.Framework.SetUp]
        public virtual void ResetXmlParserFactoryToDefault() {
            XmlProcessorCreator.SetXmlParserFactory(new DefaultSafeXmlParserFactory());
        }

        [NUnit.Framework.Test]
        public virtual void XxeTestFromString() {
            NUnit.Framework.Assert.That(() =>  {
                XMPMetaParser.Parse(XMP_WITH_XXE, null);
            }
            , NUnit.Framework.Throws.InstanceOf<XMPException>().With.Message.EqualTo(DTD_EXCEPTION_MESSAGE))
;
        }

        [NUnit.Framework.Test]
        public virtual void XxeTestFromByteBuffer() {
            NUnit.Framework.Assert.That(() =>  {
                XMPMetaParser.Parse(XMP_WITH_XXE.GetBytes(System.Text.Encoding.UTF8), null);
            }
            , NUnit.Framework.Throws.InstanceOf<XMPException>().With.Message.EqualTo(DTD_EXCEPTION_MESSAGE))
;
        }

        [NUnit.Framework.Test]
        public virtual void XxeTestFromInputStream() {
            using (Stream inputStream = new MemoryStream(XMP_WITH_XXE.GetBytes(System.Text.Encoding.UTF8))) {
                NUnit.Framework.Assert.That(() =>  {
                    XMPMetaParser.Parse(inputStream, null);
                }
                , NUnit.Framework.Throws.InstanceOf<XMPException>().With.Message.EqualTo(DTD_EXCEPTION_MESSAGE))
;
            }
        }

        [NUnit.Framework.Test]
        public virtual void XxeTestFromStringCustomXmlParser() {
            XmlProcessorCreator.SetXmlParserFactory(new SecurityTestXmlParserFactory());
            NUnit.Framework.Assert.That(() =>  {
                XMPMetaParser.Parse(XMP_WITH_XXE, null);
            }
            , NUnit.Framework.Throws.InstanceOf<PdfException>().With.Message.EqualTo("Test message"))
;
        }
    }
}
