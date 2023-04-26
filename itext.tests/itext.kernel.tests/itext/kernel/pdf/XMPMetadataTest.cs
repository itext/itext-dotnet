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
using iText.Kernel.Utils;
using iText.Kernel.XMP;
using iText.Kernel.XMP.Options;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Kernel.Pdf {
    [NUnit.Framework.Category("IntegrationTest")]
    public class XMPMetadataTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/pdf/XmpWriterTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/kernel/pdf/XmpWriterTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void CreateEmptyDocumentWithXmp() {
            String filename = "emptyDocumentWithXmp.pdf";
            PdfWriter writer = new PdfWriter(destinationFolder + filename, new WriterProperties().AddXmpMetadata());
            PdfDocument pdfDoc = new PdfDocument(writer);
            pdfDoc.GetDocumentInfo().SetAuthor("Alexander Chingarev").SetCreator("iText").SetTitle("Empty iText Document"
                );
            pdfDoc.GetDocumentInfo().GetPdfObject().Remove(PdfName.CreationDate);
            pdfDoc.GetDocumentInfo().GetPdfObject().Remove(PdfName.ModDate);
            PdfPage page = pdfDoc.AddNewPage();
            page.Flush();
            pdfDoc.Close();
            PdfReader reader = new PdfReader(destinationFolder + filename);
            PdfDocument pdfDocument = new PdfDocument(reader);
            NUnit.Framework.Assert.AreEqual(false, reader.HasRebuiltXref(), "Rebuilt");
            byte[] outBytes = pdfDocument.GetXmpMetadata();
            pdfDocument.Close();
            byte[] cmpBytes = ReadFile(sourceFolder + "emptyDocumentWithXmp.xml");
            cmpBytes = RemoveAlwaysDifferentEntries(cmpBytes);
            outBytes = RemoveAlwaysDifferentEntries(outBytes);
            NUnit.Framework.Assert.IsTrue(new CompareTool().CompareXmls(outBytes, cmpBytes));
        }

        [NUnit.Framework.Test]
        public virtual void EmptyDocumentWithXmpAppendMode01() {
            String created = destinationFolder + "emptyDocumentWithXmpAppendMode01.pdf";
            String updated = destinationFolder + "emptyDocumentWithXmpAppendMode01_updated.pdf";
            String updatedAgain = destinationFolder + "emptyDocumentWithXmpAppendMode01_updatedAgain.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(created));
            pdfDocument.AddNewPage();
            // create XMP metadata
            pdfDocument.GetXmpMetadata(true);
            pdfDocument.Close();
            pdfDocument = new PdfDocument(new PdfReader(created), new PdfWriter(updated), new StampingProperties().UseAppendMode
                ());
            pdfDocument.Close();
            pdfDocument = new PdfDocument(new PdfReader(updated), new PdfWriter(updatedAgain), new StampingProperties(
                ).UseAppendMode());
            pdfDocument.Close();
            PdfReader reader = new PdfReader(updatedAgain);
            pdfDocument = new PdfDocument(reader);
            NUnit.Framework.Assert.AreEqual(false, reader.HasRebuiltXref(), "Rebuilt");
            NUnit.Framework.Assert.IsNotNull(pdfDocument.GetCatalog().GetPdfObject().GetAsStream(PdfName.Metadata));
            PdfIndirectReference metadataRef = pdfDocument.GetCatalog().GetPdfObject().GetAsStream(PdfName.Metadata).GetIndirectReference
                ();
            NUnit.Framework.Assert.AreEqual(6, metadataRef.GetObjNumber());
            NUnit.Framework.Assert.AreEqual(0, metadataRef.GetGenNumber());
            byte[] outBytes = pdfDocument.GetXmpMetadata();
            pdfDocument.Close();
            byte[] cmpBytes = ReadFile(sourceFolder + "emptyDocumentWithXmpAppendMode01.xml");
            cmpBytes = RemoveAlwaysDifferentEntries(cmpBytes);
            outBytes = RemoveAlwaysDifferentEntries(outBytes);
            NUnit.Framework.Assert.IsTrue(new CompareTool().CompareXmls(outBytes, cmpBytes));
        }

        [NUnit.Framework.Test]
        public virtual void EmptyDocumentWithXmpAppendMode02() {
            String created = destinationFolder + "emptyDocumentWithXmpAppendMode02.pdf";
            String updated = destinationFolder + "emptyDocumentWithXmpAppendMode02_updated.pdf";
            String updatedAgain = destinationFolder + "emptyDocumentWithXmpAppendMode02_updatedAgain.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(created));
            pdfDocument.AddNewPage();
            pdfDocument.Close();
            pdfDocument = new PdfDocument(new PdfReader(created), new PdfWriter(updated), new StampingProperties().UseAppendMode
                ());
            // create XMP metadata
            pdfDocument.GetXmpMetadata(true);
            pdfDocument.Close();
            pdfDocument = new PdfDocument(new PdfReader(updated), new PdfWriter(updatedAgain), new StampingProperties(
                ).UseAppendMode());
            pdfDocument.Close();
            PdfReader reader = new PdfReader(updatedAgain);
            pdfDocument = new PdfDocument(reader);
            NUnit.Framework.Assert.AreEqual(false, reader.HasRebuiltXref(), "Rebuilt");
            NUnit.Framework.Assert.IsNotNull(pdfDocument.GetCatalog().GetPdfObject().GetAsStream(PdfName.Metadata));
            PdfIndirectReference metadataRef = pdfDocument.GetCatalog().GetPdfObject().GetAsStream(PdfName.Metadata).GetIndirectReference
                ();
            NUnit.Framework.Assert.AreEqual(6, metadataRef.GetObjNumber());
            NUnit.Framework.Assert.AreEqual(0, metadataRef.GetGenNumber());
            byte[] outBytes = pdfDocument.GetXmpMetadata();
            pdfDocument.Close();
            byte[] cmpBytes = ReadFile(sourceFolder + "emptyDocumentWithXmpAppendMode02.xml");
            cmpBytes = RemoveAlwaysDifferentEntries(cmpBytes);
            outBytes = RemoveAlwaysDifferentEntries(outBytes);
            NUnit.Framework.Assert.IsTrue(new CompareTool().CompareXmls(outBytes, cmpBytes));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.EXCEPTION_WHILE_UPDATING_XMPMETADATA)]
        public virtual void CreateEmptyDocumentWithAbcXmp() {
            MemoryStream fos = new MemoryStream();
            PdfWriter writer = new PdfWriter(fos);
            PdfDocument pdfDoc = new PdfDocument(writer);
            pdfDoc.GetDocumentInfo().SetAuthor("Alexander Chingarev").SetCreator("iText").SetTitle("Empty iText Document"
                );
            pdfDoc.GetDocumentInfo().GetPdfObject().Remove(PdfName.CreationDate);
            pdfDoc.GetDocumentInfo().GetPdfObject().Remove(PdfName.ModDate);
            PdfPage page = pdfDoc.AddNewPage();
            page.Flush();
            pdfDoc.SetXmpMetadata("abc".GetBytes(iText.Commons.Utils.EncodingUtil.ISO_8859_1));
            pdfDoc.Close();
            PdfReader reader = new PdfReader(new MemoryStream(fos.ToArray()));
            PdfDocument pdfDocument = new PdfDocument(reader);
            NUnit.Framework.Assert.AreEqual(false, reader.HasRebuiltXref(), "Rebuilt");
            NUnit.Framework.Assert.AreEqual("abc".GetBytes(iText.Commons.Utils.EncodingUtil.ISO_8859_1), pdfDocument.GetXmpMetadata
                ());
            NUnit.Framework.Assert.IsNotNull(pdfDocument.GetPage(1));
            reader.Close();
        }

        [NUnit.Framework.Test]
        [NUnit.Framework.Ignore("DEVSIX-1899: fails in .NET passes in Java")]
        public virtual void CustomXmpTest() {
            RunCustomXmpTest("customXmp", "<?xpacket begin='' id='W5M0MpCehiHzreSzNTczkc9d' bytes='770'?>\n" + "\n" + 
                "<rdf:RDF xmlns:rdf='http://www.w3.org/1999/02/22-rdf-syntax-ns#'\n" + " xmlns:iX='http://ns.adobe.com/iX/1.0/'>\n"
                 + "\n" + " <rdf:Description about=''\n" + "  xmlns='http://ns.adobe.com/pdf/1.3/'\n" + "  xmlns:pdf='http://ns.adobe.com/pdf/1.3/'>\n"
                 + "  <pdf:ModDate>2001-03-28T15:17:00-08:00</pdf:ModDate>\n" + "  <pdf:CreationDate>2001-03-28T15:19:45-08:00</pdf:CreationDate>\n"
                 + " </rdf:Description>\n" + "\n" + " <rdf:Description about=''\n" + "  xmlns='http://ns.adobe.com/xap/1.0/'\n"
                 + "  xmlns:xap='http://ns.adobe.com/xap/1.0/'>\n" + "  <xap:ModifyDate>2001-03-28T15:17:00-08:00</xap:ModifyDate>\n"
                 + "  <xap:CreateDate>2001-03-28T15:19:45-08:00</xap:CreateDate>\n" + "  <xap:MetadataDate>2001-03-28T15:17:00-08:00</xap:MetadataDate>\n"
                 + " </rdf:Description>\n" + "\n" + "</rdf:RDF>\n" + "<?xpacket end='r'?>");
        }

        [NUnit.Framework.Test]
        [NUnit.Framework.Ignore("DEVSIX-1899: fails in .NET passes in Java")]
        public virtual void CustomXmpTest02() {
            RunCustomXmpTest("customXmp02", "<?xpacket begin='' id='W5M0MpCehiHzreSzNTczkc9d' bytes='1026'?><rdf:RDF xmlns:rdf='http://www.w3.org/1999/02/22-rdf-syntax-ns#' xmlns:iX='http://ns.adobe.com/iX/1.0/'><rdf:Description about='' xmlns='http://ns.adobe.com/pdf/1.3/' xmlns:pdf='http://ns.adobe.com/pdf/1.3/' pdf:CreationDate='2016-01-27T13:07:23Z' pdf:ModDate='2016-01-27T13:07:23Z' pdf:Producer='Acrobat Distiller 5.0.5 (Windows)' pdf:Author='Koeck' pdf:Creator='PScript5.dll Version 5.2.2' pdf:Title='Rasant_ACE.indd'/>\n"
                 + "<rdf:Description about='' xmlns='http://ns.adobe.com/xap/1.0/' xmlns:xap='http://ns.adobe.com/xap/1.0/' xap:CreateDate='2016-01-27T13:07:23Z' xap:ModifyDate='2016-01-27T13:07:23Z' xap:Author='Koeck' xap:MetadataDate='2016-01-27T13:07:23Z'><xap:Title><rdf:Alt><rdf:li xml:lang='x-default'>Rasant_ACE.indd</rdf:li></rdf:Alt></xap:Title></rdf:Description>\n"
                 + "<rdf:Description about='' xmlns='http://purl.org/dc/elements/1.1/' xmlns:dc='http://purl.org/dc/elements/1.1/' dc:creator='Koeck' dc:title='Rasant_ACE.indd'/>\n"
                 + "</rdf:RDF><?xpacket end='r'?>");
        }

        private void RunCustomXmpTest(String name, String xmp) {
            String outPath = destinationFolder + name + ".pdf";
            String cmpPath = sourceFolder + "cmp_" + name + ".pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outPath));
            PdfPage page = pdfDoc.AddNewPage();
            page.Flush();
            pdfDoc.SetXmpMetadata(xmp.GetBytes(iText.Commons.Utils.EncodingUtil.ISO_8859_1));
            pdfDoc.Close();
            CompareTool compareTool = new CompareTool();
            NUnit.Framework.Assert.IsNull(compareTool.CompareByContent(outPath, cmpPath, destinationFolder, "diff_" + 
                name + "_"));
            NUnit.Framework.Assert.IsNull(compareTool.CompareDocumentInfo(outPath, cmpPath));
        }

        private byte[] RemoveAlwaysDifferentEntries(byte[] cmpBytes) {
            XMPMeta xmpMeta = XMPMetaFactory.ParseFromBuffer(cmpBytes);
            XMPUtils.RemoveProperties(xmpMeta, XMPConst.NS_XMP, PdfConst.CreateDate, true, true);
            XMPUtils.RemoveProperties(xmpMeta, XMPConst.NS_XMP, PdfConst.ModifyDate, true, true);
            XMPUtils.RemoveProperties(xmpMeta, XMPConst.NS_XMP, PdfConst.MetadataDate, true, true);
            XMPUtils.RemoveProperties(xmpMeta, XMPConst.NS_PDF, PdfConst.Producer, true, true);
            cmpBytes = XMPMetaFactory.SerializeToBuffer(xmpMeta, new SerializeOptions(SerializeOptions.SORT));
            return cmpBytes;
        }
    }
}
