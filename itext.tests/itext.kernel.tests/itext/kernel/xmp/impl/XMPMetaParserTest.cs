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
using System.Xml;
using iText.IO.Util;
using iText.Test;

namespace iText.Kernel.XMP.Impl {
    public class XMPMetaParserTest : ExtendedITextTest {
        private static readonly String XXE_FILE_PATH = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/xmp/impl/xxe-data.txt";

        private const String XMP_WITH_XXE = "<?xpacket begin=\"\" id=\"W5M0MpCehiHzreSzNTczkc9d\"?>\n" + "<!DOCTYPE foo [ <!ENTITY xxe SYSTEM \"{0}\" > ]>\n"
             + "<x:xmpmeta xmlns:x=\"adobe:ns:meta/\">\n" + "    <rdf:RDF xmlns:rdf=\"http://www.w3.org/1999/02/22-rdf-syntax-ns#\">\n"
             + "        <rdf:Description rdf:about=\"\" xmlns:pdfaid=\"http://www.aiim.org/pdfa/ns/id/\">\n" + "            <pdfaid:part>&xxe;1</pdfaid:part>\n"
             + "            <pdfaid:conformance>B</pdfaid:conformance>\n" + "        </rdf:Description>\n" + "    </rdf:RDF>\n"
             + "</x:xmpmeta>\n" + "<?xpacket end=\"r\"?>";

        private const String EXPECTED_SERIALIZED_XMP = "<?xpacket begin=\"\uFEFF\" id=\"W5M0MpCehiHzreSzNTczkc9d\"?>\n"
             + "<x:xmpmeta xmlns:x=\"adobe:ns:meta/\" x:xmptk=\"Adobe XMP Core 5.1.0-jc003\">\n" + "  <rdf:RDF xmlns:rdf=\"http://www.w3.org/1999/02/22-rdf-syntax-ns#\">\n"
             + "    <rdf:Description rdf:about=\"\"\n" + "        xmlns:pdfaid=\"http://www.aiim.org/pdfa/ns/id/\"\n"
             + "      pdfaid:part=\"1\"\n" + "      pdfaid:conformance=\"B\"/>\n" + "  </rdf:RDF>\n" + "</x:xmpmeta>\n"
             + "                                                                                                    \n"
             + "                                                                                                    \n"
             + "                                                                                                    \n"
             + "                                                                                                    \n"
             + "                                                                                                    \n"
             + "                                                                                                    \n"
             + "                                                                                                    \n"
             + "                                                                                                    \n"
             + "                                                                                                    \n"
             + "                                                                                                    \n"
             + "             \n" + "<?xpacket end=\"w\"?>";

#if !NETSTANDARD2_0 && !NET5_0
        [NUnit.Framework.Test]
        public virtual void XxeTestFromString() {
            String metadataToParse = MessageFormatUtil.Format(XMP_WITH_XXE, XXE_FILE_PATH);
            XMPMeta xmpMeta = XMPMetaParser.Parse(metadataToParse, null);
            NUnit.Framework.Assert.AreEqual(EXPECTED_SERIALIZED_XMP, XMPMetaFactory.SerializeToString(xmpMeta, null));
        }
#endif // !NETSTANDARD2_0
        
#if NETSTANDARD2_0
        [NUnit.Framework.Ignore("DEVSIX-3270")]
        [NUnit.Framework.Test]
        public virtual void XxeTestFromString() {
            String metadataToParse = MessageFormatUtil.Format(XMP_WITH_XXE, XXE_FILE_PATH);
            NUnit.Framework.Assert.That(() =>
                {
                    // the line below is expected to produce an exception
                    XMPMetaParser.Parse(metadataToParse, null);
                }
                , NUnit.Framework.Throws.InstanceOf<XmlException>().With.Message.EqualTo("Reference to undeclared entity 'xxe'. Line 6, position 27."));
        }
#endif // NETSTANDARD2_0
        
#if !NETSTANDARD2_0 && !NET5_0
        [NUnit.Framework.Test]
        public virtual void XxeTestFromByteBuffer() {
            String metadataToParse = MessageFormatUtil.Format(XMP_WITH_XXE, XXE_FILE_PATH);
            XMPMeta xmpMeta = XMPMetaParser.Parse(metadataToParse.GetBytes(System.Text.Encoding.UTF8), null);
            NUnit.Framework.Assert.AreEqual(EXPECTED_SERIALIZED_XMP, XMPMetaFactory.SerializeToString(xmpMeta, null));
        }
#endif // !NETSTANDARD2_0
        
#if NETSTANDARD2_0
        [NUnit.Framework.Ignore("DEVSIX-3270")]
        [NUnit.Framework.Test]
        public virtual void XxeTestFromByteBuffer() {
            String metadataToParse = MessageFormatUtil.Format(XMP_WITH_XXE, XXE_FILE_PATH);
            NUnit.Framework.Assert.That(() =>
                {
                    // the line below is expected to produce an exception
                    XMPMetaParser.Parse(metadataToParse.GetBytes(System.Text.Encoding.UTF8), null);
                }
                , NUnit.Framework.Throws.InstanceOf<XMPException>().With.Message.EqualTo("Unsupported Encoding"));
        }
#endif // NETSTANDARD2_0
        
#if !NETSTANDARD2_0 && !NET5_0
        [NUnit.Framework.Test]
        public virtual void XxeTestFromInputStream() {
            String metadataToParse = MessageFormatUtil.Format(XMP_WITH_XXE, XXE_FILE_PATH);
            Stream inputStream = new MemoryStream(metadataToParse.GetBytes(System.Text.Encoding.UTF8));
            XMPMeta xmpMeta = XMPMetaParser.Parse(inputStream, null);
            NUnit.Framework.Assert.AreEqual(EXPECTED_SERIALIZED_XMP, XMPMetaFactory.SerializeToString(xmpMeta, null));
        }
#endif // !NETSTANDARD2_0
        
#if NETSTANDARD2_0
        [NUnit.Framework.Ignore("DEVSIX-3270")]
        [NUnit.Framework.Test]
        public virtual void XxeTestFromInputStream() {
            String metadataToParse = MessageFormatUtil.Format(XMP_WITH_XXE, XXE_FILE_PATH);
            Stream inputStream = new MemoryStream(metadataToParse.GetBytes(System.Text.Encoding.UTF8));
            NUnit.Framework.Assert.That(() =>
                {
                    // the line below is expected to produce an exception
                    XMPMetaParser.Parse(inputStream, null);
                }
                , NUnit.Framework.Throws.InstanceOf<XMPException>().With.Message.EqualTo("Unsupported Encoding"));
        }
#endif // NETSTANDARD2_0
    }
}
