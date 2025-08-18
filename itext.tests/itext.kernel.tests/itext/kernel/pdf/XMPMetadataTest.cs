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
using System.IO;
using iText.Kernel.Utils;
using iText.Kernel.XMP;
using iText.Kernel.XMP.Options;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Kernel.Pdf {
    [NUnit.Framework.Category("IntegrationTest")]
    public class XMPMetadataTest : ExtendedITextTest {
        public static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/pdf/XMPMetadataTest/";

        public static readonly String DESTINATION_FOLDER = TestUtil.GetOutputPath() + "/kernel/pdf/XMPMetadataTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.OneTimeTearDown]
        public static void AfterClass() {
            CompareTool.Cleanup(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void CreateEmptyDocumentWithXmp() {
            String filename = "emptyDocumentWithXmp.pdf";
            PdfWriter writer = CompareTool.CreateTestPdfWriter(DESTINATION_FOLDER + filename, new WriterProperties().AddXmpMetadata
                ());
            PdfDocument pdfDoc = new PdfDocument(writer);
            pdfDoc.GetDocumentInfo().SetAuthor("Alexander Chingarev").SetCreator("iText").SetTitle("Empty iText Document"
                );
            pdfDoc.GetDocumentInfo().GetPdfObject().Remove(PdfName.CreationDate);
            pdfDoc.GetDocumentInfo().GetPdfObject().Remove(PdfName.ModDate);
            PdfPage page = pdfDoc.AddNewPage();
            page.Flush();
            pdfDoc.Close();
            PdfReader reader = CompareTool.CreateOutputReader(DESTINATION_FOLDER + filename);
            PdfDocument pdfDocument = new PdfDocument(reader);
            NUnit.Framework.Assert.AreEqual(false, reader.HasRebuiltXref(), "Rebuilt");
            byte[] outBytes = pdfDocument.GetXmpMetadataBytes();
            pdfDocument.Close();
            byte[] cmpBytes = ReadFile(SOURCE_FOLDER + "emptyDocumentWithXmp.xml");
            cmpBytes = RemoveAlwaysDifferentEntries(cmpBytes);
            outBytes = RemoveAlwaysDifferentEntries(outBytes);
            NUnit.Framework.Assert.IsTrue(new CompareTool().CompareXmls(outBytes, cmpBytes));
        }

        [NUnit.Framework.Test]
        public virtual void EmptyDocumentWithXmpAppendMode01() {
            String created = DESTINATION_FOLDER + "emptyDocumentWithXmpAppendMode01.pdf";
            String updated = DESTINATION_FOLDER + "emptyDocumentWithXmpAppendMode01_updated.pdf";
            String updatedAgain = DESTINATION_FOLDER + "emptyDocumentWithXmpAppendMode01_updatedAgain.pdf";
            PdfDocument pdfDocument = new PdfDocument(CompareTool.CreateTestPdfWriter(created));
            pdfDocument.AddNewPage();
            // create XMP metadata
            pdfDocument.GetXmpMetadata(true);
            pdfDocument.Close();
            pdfDocument = new PdfDocument(CompareTool.CreateOutputReader(created), CompareTool.CreateTestPdfWriter(updated
                ), new StampingProperties().UseAppendMode());
            pdfDocument.Close();
            pdfDocument = new PdfDocument(CompareTool.CreateOutputReader(updated), CompareTool.CreateTestPdfWriter(updatedAgain
                ), new StampingProperties().UseAppendMode());
            pdfDocument.Close();
            PdfReader reader = CompareTool.CreateOutputReader(updatedAgain);
            pdfDocument = new PdfDocument(reader);
            NUnit.Framework.Assert.AreEqual(false, reader.HasRebuiltXref(), "Rebuilt");
            NUnit.Framework.Assert.IsNotNull(pdfDocument.GetCatalog().GetPdfObject().GetAsStream(PdfName.Metadata));
            PdfIndirectReference metadataRef = pdfDocument.GetCatalog().GetPdfObject().GetAsStream(PdfName.Metadata).GetIndirectReference
                ();
            NUnit.Framework.Assert.AreEqual(6, metadataRef.GetObjNumber());
            NUnit.Framework.Assert.AreEqual(0, metadataRef.GetGenNumber());
            byte[] outBytes = pdfDocument.GetXmpMetadataBytes();
            pdfDocument.Close();
            byte[] cmpBytes = ReadFile(SOURCE_FOLDER + "emptyDocumentWithXmpAppendMode01.xml");
            cmpBytes = RemoveAlwaysDifferentEntries(cmpBytes);
            outBytes = RemoveAlwaysDifferentEntries(outBytes);
            NUnit.Framework.Assert.IsTrue(new CompareTool().CompareXmls(outBytes, cmpBytes));
        }

        [NUnit.Framework.Test]
        public virtual void EmptyDocumentWithXmpAppendMode02() {
            String created = DESTINATION_FOLDER + "emptyDocumentWithXmpAppendMode02.pdf";
            String updated = DESTINATION_FOLDER + "emptyDocumentWithXmpAppendMode02_updated.pdf";
            String updatedAgain = DESTINATION_FOLDER + "emptyDocumentWithXmpAppendMode02_updatedAgain.pdf";
            PdfDocument pdfDocument = new PdfDocument(CompareTool.CreateTestPdfWriter(created));
            pdfDocument.AddNewPage();
            pdfDocument.Close();
            pdfDocument = new PdfDocument(CompareTool.CreateOutputReader(created), CompareTool.CreateTestPdfWriter(updated
                ), new StampingProperties().UseAppendMode());
            // create XMP metadata
            pdfDocument.GetXmpMetadata(true);
            pdfDocument.Close();
            pdfDocument = new PdfDocument(CompareTool.CreateOutputReader(updated), CompareTool.CreateTestPdfWriter(updatedAgain
                ), new StampingProperties().UseAppendMode());
            pdfDocument.Close();
            PdfReader reader = CompareTool.CreateOutputReader(updatedAgain);
            pdfDocument = new PdfDocument(reader);
            NUnit.Framework.Assert.AreEqual(false, reader.HasRebuiltXref(), "Rebuilt");
            NUnit.Framework.Assert.IsNotNull(pdfDocument.GetCatalog().GetPdfObject().GetAsStream(PdfName.Metadata));
            PdfIndirectReference metadataRef = pdfDocument.GetCatalog().GetPdfObject().GetAsStream(PdfName.Metadata).GetIndirectReference
                ();
            NUnit.Framework.Assert.AreEqual(6, metadataRef.GetObjNumber());
            NUnit.Framework.Assert.AreEqual(0, metadataRef.GetGenNumber());
            byte[] outBytes = pdfDocument.GetXmpMetadataBytes();
            pdfDocument.Close();
            byte[] cmpBytes = ReadFile(SOURCE_FOLDER + "emptyDocumentWithXmpAppendMode02.xml");
            cmpBytes = RemoveAlwaysDifferentEntries(cmpBytes);
            outBytes = RemoveAlwaysDifferentEntries(outBytes);
            NUnit.Framework.Assert.IsTrue(new CompareTool().CompareXmls(outBytes, cmpBytes));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.EXCEPTION_WHILE_UPDATING_XMPMETADATA, Count = 2)]
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
            NUnit.Framework.Assert.IsFalse(reader.HasRebuiltXref(), "Rebuilt");
            NUnit.Framework.Assert.AreEqual("abc".GetBytes(iText.Commons.Utils.EncodingUtil.ISO_8859_1), pdfDocument.GetXmpMetadataBytes
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
            String outPath = DESTINATION_FOLDER + name + ".pdf";
            String cmpPath = SOURCE_FOLDER + "cmp_" + name + ".pdf";
            PdfDocument pdfDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(outPath));
            PdfPage page = pdfDoc.AddNewPage();
            page.Flush();
            pdfDoc.SetXmpMetadata(xmp.GetBytes(iText.Commons.Utils.EncodingUtil.ISO_8859_1));
            pdfDoc.Close();
            CompareTool compareTool = new CompareTool();
            NUnit.Framework.Assert.IsNull(compareTool.CompareByContent(outPath, cmpPath, DESTINATION_FOLDER, "diff_" +
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

        [NUnit.Framework.Test]
        public virtual void BagParsingTest() {
            String xmp = "<x:xmpmeta xmlns:x=\"adobe:ns:meta/\" x:xmptk=\"Adobe XMP Core 6.0-c006 79.dabacbb, " + "2021/04/14-00:39:44        \">\n"
                 + "   <rdf:RDF xmlns:rdf=\"http://www.w3.org/1999/02/22-rdf-syntax-ns#\">\n" + "      <rdf:Description rdf:about=\"\"\n"
                 + "            xmlns:photoshop=\"http://ns.adobe.com/photoshop/1.0/\"\n" + "            xmlns:xmpMM=\"http://ns.adobe.com/xap/1.0/mm/\"\n"
                 + "            xmlns:stEvt=\"http://ns.adobe.com/xap/1.0/sType/ResourceEvent#\"\n" + "            xmlns:stRef=\"http://ns.adobe.com/xap/1.0/sType/ResourceRef#\"\n"
                 + "            xmlns:xmp=\"http://ns.adobe.com/xap/1.0/\"\n" + "            xmlns:xmpGImg=\"http://ns.adobe.com/xap/1.0/g/img/\"\n"
                 + "            xmlns:dc=\"http://purl.org/dc/elements/1.1/\"\n" + "            xmlns:pdf=\"http://ns.adobe.com/pdf/1.3/\">\n"
                 + "         <photoshop:LegacyIPTCDigest>CDCFFA7DA8C7BE09057076AEAF05C34E</photoshop:LegacyIPTCDigest>\n"
                 + "         <photoshop:ColorMode>3</photoshop:ColorMode>\n" + "         <photoshop:ICCProfile>sRGB IEC61966-2.1</photoshop:ICCProfile>\n"
                 + "         <photoshop:DocumentAncestors>\n" + "            <rdf:Bag>\n" + "               <rdf:li>78274CCED3154607AD19599D29855E30</rdf:li>\n"
                 + "               <rdf:li>A61D41481CEE4032AE7A116AD6C942DC</rdf:li>\n" + "               <rdf:li>A2A26CD02B014819824FC4314B4152FF</rdf:li>\n"
                 + "               <rdf:li>425FC234DCE84124A5EEAF17C69CCC62</rdf:li>\n" + "               <rdf:li>D2B4867567A547EA9F87B476EB21147E</rdf:li>\n"
                 + "               <rdf:li>CE995EEDAD734D029F9B27AD04BA7052</rdf:li>\n" + "               <rdf:li>E754B36AD97E49EAABC7E8F7CEA30696</rdf:li>\n"
                 + "               <rdf:li>713B782250904422BDDCAD1723C25C3C</rdf:li>\n" + "               <rdf:li>DC818BB5F9F1421C87DA05C97DEEB2CF</rdf:li>\n"
                 + "               \n" + "               <rdf:li>D2B4867567A547EA9F87B476EB21147E</rdf:li>\n" + "               <rdf:li>CE995EEDAD734D029F9B27AD04BA7052</rdf:li>\n"
                 + "               <rdf:li>E754B36AD97E49EAABC7E8F7CEA30696</rdf:li>\n" + "               <rdf:li>713B782250904422BDDCAD1723C25C3C</rdf:li>\n"
                 + "               <rdf:li>DC818BB5F9F1421C87DA05C97DEEB2CF</rdf:li>\n" + "            </rdf:Bag>\n" +
                 "         </photoshop:DocumentAncestors>      \n" + "      </rdf:Description>\n" + "   </rdf:RDF>\n" 
                + "</x:xmpmeta>";
            NUnit.Framework.Assert.DoesNotThrow(() => XMPMetaFactory.ParseFromBuffer(xmp.GetBytes(System.Text.Encoding
                .UTF8)));
        }

        [NUnit.Framework.Test]
        public virtual void AltParsingTest() {
            String xmp = "<x:xmpmeta xmlns:x=\"adobe:ns:meta/\" x:xmptk=\"Adobe XMP Core 6.0-c006 79.dabacbb, " + "2021/04/14-00:39:44        \">\n"
                 + "   <rdf:RDF xmlns:rdf=\"http://www.w3.org/1999/02/22-rdf-syntax-ns#\">\n" + "      <rdf:Description rdf:about=\"\"\n"
                 + "            xmlns:photoshop=\"http://ns.adobe.com/photoshop/1.0/\"\n" + "            xmlns:xmpMM=\"http://ns.adobe.com/xap/1.0/mm/\"\n"
                 + "            xmlns:stEvt=\"http://ns.adobe.com/xap/1.0/sType/ResourceEvent#\"\n" + "            xmlns:stRef=\"http://ns.adobe.com/xap/1.0/sType/ResourceRef#\"\n"
                 + "            xmlns:xmp=\"http://ns.adobe.com/xap/1.0/\"\n" + "            xmlns:xmpGImg=\"http://ns.adobe.com/xap/1.0/g/img/\"\n"
                 + "            xmlns:dc=\"http://purl.org/dc/elements/1.1/\"\n" + "            xmlns:pdf=\"http://ns.adobe.com/pdf/1.3/\">\n"
                 + "         <photoshop:LegacyIPTCDigest>CDCFFA7DA8C7BE09057076AEAF05C34E</photoshop:LegacyIPTCDigest>\n"
                 + "         <photoshop:ColorMode>3</photoshop:ColorMode>\n" + "         <photoshop:ICCProfile>sRGB IEC61966-2.1</photoshop:ICCProfile>\n"
                 + "         <photoshop:DocumentAncestors>\n" + "            <rdf:Alt>\n" + "               <rdf:li>0006528E7FAD8C1170BB8605BDABA5EC</rdf:li>\n"
                 + "               <rdf:li>00072919E3FCD9243A279B11FA36E751</rdf:li>\n" + "               <rdf:li>D2B4867567A547EA9F87B476EB21147E</rdf:li>\n"
                 + "               <rdf:li>CE995EEDAD734D029F9B27AD04BA7052</rdf:li>\n" + "               <rdf:li>E754B36AD97E49EAABC7E8F7CEA30696</rdf:li>\n"
                 + "               <rdf:li>713B782250904422BDDCAD1723C25C3C</rdf:li>\n" + "               <rdf:li>DC818BB5F9F1421C87DA05C97DEEB2CF</rdf:li>\n"
                 + "               \n" + "               <rdf:li>D2B4867567A547EA9F87B476EB21147E</rdf:li>\n" + "               <rdf:li>CE995EEDAD734D029F9B27AD04BA7052</rdf:li>\n"
                 + "               <rdf:li>E754B36AD97E49EAABC7E8F7CEA30696</rdf:li>\n" + "               <rdf:li>713B782250904422BDDCAD1723C25C3C</rdf:li>\n"
                 + "               <rdf:li>DC818BB5F9F1421C87DA05C97DEEB2CF</rdf:li>\n" + "            </rdf:Alt>\n" +
                 "         </photoshop:DocumentAncestors>      \n" + "      </rdf:Description>\n" + "   </rdf:RDF>\n" 
                + "</x:xmpmeta>";
            NUnit.Framework.Assert.DoesNotThrow(() => XMPMetaFactory.ParseFromBuffer(xmp.GetBytes(System.Text.Encoding
                .UTF8)));
        }

        [NUnit.Framework.Test]
        public virtual void SeqParsingTest() {
            String xmp = "<x:xmpmeta xmlns:x=\"adobe:ns:meta/\" x:xmptk=\"Adobe XMP Core 6.0-c006 79.dabacbb, " + "2021/04/14-00:39:44        \">\n"
                 + "   <rdf:RDF xmlns:rdf=\"http://www.w3.org/1999/02/22-rdf-syntax-ns#\">\n" + "      <rdf:Description rdf:about=\"\"\n"
                 + "            xmlns:photoshop=\"http://ns.adobe.com/photoshop/1.0/\"\n" + "            xmlns:xmpMM=\"http://ns.adobe.com/xap/1.0/mm/\"\n"
                 + "            xmlns:stEvt=\"http://ns.adobe.com/xap/1.0/sType/ResourceEvent#\"\n" + "            xmlns:stRef=\"http://ns.adobe.com/xap/1.0/sType/ResourceRef#\"\n"
                 + "            xmlns:xmp=\"http://ns.adobe.com/xap/1.0/\"\n" + "            xmlns:xmpGImg=\"http://ns.adobe.com/xap/1.0/g/img/\"\n"
                 + "            xmlns:dc=\"http://purl.org/dc/elements/1.1/\"\n" + "            xmlns:pdf=\"http://ns.adobe.com/pdf/1.3/\">\n"
                 + "         <photoshop:LegacyIPTCDigest>CDCFFA7DA8C7BE09057076AEAF05C34E</photoshop:LegacyIPTCDigest>\n"
                 + "         <photoshop:ColorMode>3</photoshop:ColorMode>\n" + "         <photoshop:ICCProfile>sRGB IEC61966-2.1</photoshop:ICCProfile>\n"
                 + "         <photoshop:DocumentAncestors>\n" + "            <rdf:Seq>\n" + "               <rdf:li>78274CCED3154607AD19599D29855E30</rdf:li>\n"
                 + "               <rdf:li>A61D41481CEE4032AE7A116AD6C942DC</rdf:li>\n" + "               <rdf:li>A2A26CD02B014819824FC4314B4152FF</rdf:li>\n"
                 + "               <rdf:li>425FC234DCE84124A5EEAF17C69CCC62</rdf:li>\n" + "               <rdf:li>D2B4867567A547EA9F87B476EB21147E</rdf:li>\n"
                 + "               <rdf:li>CE995EEDAD734D029F9B27AD04BA7052</rdf:li>\n" + "               <rdf:li>E754B36AD97E49EAABC7E8F7CEA30696</rdf:li>\n"
                 + "               <rdf:li>713B782250904422BDDCAD1723C25C3C</rdf:li>\n" + "               <rdf:li>DC818BB5F9F1421C87DA05C97DEEB2CF</rdf:li>\n"
                 + "               \n" + "               <rdf:li>D2B4867567A547EA9F87B476EB21147E</rdf:li>\n" + "               <rdf:li>CE995EEDAD734D029F9B27AD04BA7052</rdf:li>\n"
                 + "               <rdf:li>E754B36AD97E49EAABC7E8F7CEA30696</rdf:li>\n" + "               <rdf:li>713B782250904422BDDCAD1723C25C3C</rdf:li>\n"
                 + "               <rdf:li>DC818BB5F9F1421C87DA05C97DEEB2CF</rdf:li>\n" + "            </rdf:Seq>\n" +
                 "         </photoshop:DocumentAncestors>      \n" + "      </rdf:Description>\n" + "   </rdf:RDF>\n" 
                + "</x:xmpmeta>";
            NUnit.Framework.Assert.DoesNotThrow(() => XMPMetaFactory.ParseFromBuffer(xmp.GetBytes(System.Text.Encoding
                .UTF8)));
        }

        [NUnit.Framework.Test]
        public virtual void ListParsingTest() {
            String xmp = "<x:xmpmeta xmlns:x=\"adobe:ns:meta/\" x:xmptk=\"Adobe XMP Core 6.0-c006 79.dabacbb, " + "2021/04/14-00:39:44        \">\n"
                 + "   <rdf:RDF xmlns:rdf=\"http://www.w3.org/1999/02/22-rdf-syntax-ns#\">\n" + "      <rdf:Description rdf:about=\"\"\n"
                 + "            xmlns:photoshop=\"http://ns.adobe.com/photoshop/1.0/\"\n" + "            xmlns:xmpMM=\"http://ns.adobe.com/xap/1.0/mm/\"\n"
                 + "            xmlns:stEvt=\"http://ns.adobe.com/xap/1.0/sType/ResourceEvent#\"\n" + "            xmlns:stRef=\"http://ns.adobe.com/xap/1.0/sType/ResourceRef#\"\n"
                 + "            xmlns:xmp=\"http://ns.adobe.com/xap/1.0/\"\n" + "            xmlns:xmpGImg=\"http://ns.adobe.com/xap/1.0/g/img/\"\n"
                 + "            xmlns:dc=\"http://purl.org/dc/elements/1.1/\"\n" + "            xmlns:pdf=\"http://ns.adobe.com/pdf/1.3/\">\n"
                 + "         <photoshop:LegacyIPTCDigest>CDCFFA7DA8C7BE09057076AEAF05C34E</photoshop:LegacyIPTCDigest>\n"
                 + "         <photoshop:ColorMode>3</photoshop:ColorMode>\n" + "         <photoshop:ICCProfile>sRGB IEC61966-2.1</photoshop:ICCProfile>\n"
                 + "         <photoshop:ICCProfile>sRGB IEC61966-2.2</photoshop:ICCProfile>\n" + "      </rdf:Description>\n"
                 + "   </rdf:RDF>\n" + "</x:xmpmeta>";
            NUnit.Framework.Assert.Catch(typeof(XMPException), () => XMPMetaFactory.ParseFromBuffer(xmp.GetBytes(System.Text.Encoding
                .UTF8)));
        }

        [NUnit.Framework.Test]
        public virtual void ReadDocumentWithControlCharactersInXMPMetadata() {
            String src = SOURCE_FOLDER + "docWithControlCharactersInXmp.pdf";
            using (PdfDocument document = new PdfDocument(new PdfReader(src), new PdfWriter(new MemoryStream()), new StampingProperties
                ())) {
                NUnit.Framework.Assert.AreEqual(PdfConformance.PDF_A_3A, document.GetConformance());
            }
        }

        [NUnit.Framework.Test]
        public virtual void ReadDocumentWithBrokenControlCharactersInXMPMetadata() {
            String src = SOURCE_FOLDER + "docWithBrokenControlCharactersInXmp.pdf";
            using (PdfDocument document = new PdfDocument(new PdfReader(src), new PdfWriter(new MemoryStream()), new StampingProperties
                ())) {
                NUnit.Framework.Assert.AreEqual(PdfConformance.PDF_A_3A, document.GetConformance());
            }
        }

        [NUnit.Framework.Test]
        public virtual void ReadDocumentWithInvalidConformance() {
            String src = SOURCE_FOLDER + "docWithInvalidConformance.pdf";
            using (PdfDocument document = new PdfDocument(new PdfReader(src), new PdfWriter(new MemoryStream()), new StampingProperties
                ())) {
                NUnit.Framework.Assert.AreEqual(PdfConformance.PDF_NONE_CONFORMANCE, document.GetConformance());
            }
        }

        [LogMessage(iText.IO.Logs.IoLogMessageConstant.EXCEPTION_WHILE_UPDATING_XMPMETADATA)]
        [NUnit.Framework.Test]
        public virtual void ReadDocumentWithInvalidXMPMetadata() {
            String src = SOURCE_FOLDER + "docWithInvalidMetadata.pdf";
            using (PdfDocument document = new PdfDocument(new PdfReader(src), new PdfWriter(new MemoryStream()), new StampingProperties
                ())) {
                NUnit.Framework.Assert.AreEqual(PdfConformance.PDF_NONE_CONFORMANCE, document.GetConformance());
            }
        }
    }
}
