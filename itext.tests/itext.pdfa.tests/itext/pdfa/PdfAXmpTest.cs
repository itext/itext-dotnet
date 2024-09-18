/*
This file is part of the iText (R) project.
Copyright (c) 1998-2024 Apryse Group NV
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
using iText.Commons.Utils;
using iText.IO.Source;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Kernel.XMP;
using iText.Kernel.XMP.Options;
using iText.Test;

namespace iText.Pdfa {
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfAXmpTest : ExtendedITextTest {
        public static readonly String cmpFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfa/cmp/PdfAXmpTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/pdfa/PdfAXmpTest/";

        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfa/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void KeywordsInfoTestPdfA1b() {
            String outFile = destinationFolder + "keywordsInfoTestPdfA1b.pdf";
            String cmpFile = cmpFolder + "cmp_keywordsInfoTestPdfA1b.pdf";
            Stream @is = FileUtil.GetInputStreamForFile(sourceFolder + "sRGB Color Space Profile.icm");
            PdfOutputIntent outputIntent = new PdfOutputIntent("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1"
                , @is);
            PdfADocument doc = new PdfADocument(new PdfWriter(outFile), PdfAConformance.PDF_A_1B, outputIntent);
            doc.AddNewPage();
            doc.GetDocumentInfo().SetKeywords("key1, key2 , key3;key4,key5");
            doc.Close();
            CompareTool ct = new CompareTool();
            NUnit.Framework.Assert.IsNull(ct.CompareByContent(outFile, cmpFile, destinationFolder));
            NUnit.Framework.Assert.IsNull(ct.CompareDocumentInfo(outFile, cmpFile));
            NUnit.Framework.Assert.IsNull(ct.CompareXmp(outFile, cmpFile, true));
        }

        [NUnit.Framework.Test]
        public virtual void KeywordsInfoTestPdfA2b() {
            String outFile = destinationFolder + "keywordsInfoTestPdfA2b.pdf";
            String cmpFile = cmpFolder + "cmp_keywordsInfoTestPdfA2b.pdf";
            Stream @is = FileUtil.GetInputStreamForFile(sourceFolder + "sRGB Color Space Profile.icm");
            PdfOutputIntent outputIntent = new PdfOutputIntent("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1"
                , @is);
            PdfADocument doc = new PdfADocument(new PdfWriter(outFile), PdfAConformance.PDF_A_2B, outputIntent);
            doc.AddNewPage();
            doc.GetDocumentInfo().SetKeywords("key1, key2 , key3;key4,key5");
            doc.Close();
            CompareTool ct = new CompareTool();
            NUnit.Framework.Assert.IsNull(ct.CompareByContent(outFile, cmpFile, destinationFolder));
            NUnit.Framework.Assert.IsNull(ct.CompareDocumentInfo(outFile, cmpFile));
            NUnit.Framework.Assert.IsNull(ct.CompareXmp(outFile, cmpFile, true));
        }

        [NUnit.Framework.Test]
        public virtual void SaveAndReadDocumentWithCanonicalXmpMetadata() {
            String outFile = destinationFolder + "saveAndReadDocumentWithCanonicalXmpMetadata.pdf";
            String cmpFile = cmpFolder + "cmp_saveAndReadDocumentWithCanonicalXmpMetadata.pdf";
            PdfAConformance conformance = PdfAConformance.PDF_A_2B;
            PdfOutputIntent outputIntent;
            using (Stream @is = FileUtil.GetInputStreamForFile(sourceFolder + "sRGB Color Space Profile.icm")) {
                outputIntent = new PdfOutputIntent("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1", @is);
            }
            using (PdfADocument doc = new PdfADocument(new PdfWriter(outFile), conformance, outputIntent)) {
                doc.AddNewPage();
                XMPMeta xmp = XMPMetaFactory.Create();
                xmp.SetProperty(XMPConst.NS_PDFA_ID, XMPConst.PART, conformance.GetPart(), new PropertyOptions().SetSchemaNode
                    (true));
                xmp.SetProperty(XMPConst.NS_PDFA_ID, XMPConst.CONFORMANCE, conformance.GetLevel(), new PropertyOptions().SetSchemaNode
                    (true));
                SerializeOptions options = new SerializeOptions().SetUseCanonicalFormat(true).SetUseCompactFormat(false);
                doc.SetXmpMetadata(xmp, options);
                doc.SetTagged();
            }
            // Closing document and reopening it to flush it XMP metadata ModifyDate
            using (PdfDocument doc_1 = new PdfDocument(new PdfReader(outFile))) {
                using (PdfDocument cmpDoc = new PdfDocument(new PdfReader(cmpFile))) {
                    byte[] rdf = doc_1.GetXmpMetadataBytes();
                    byte[] expectedRdf = cmpDoc.GetXmpMetadataBytes();
                    // Comparing angle brackets, since it's the main difference between canonical and compact format.
                    NUnit.Framework.Assert.AreEqual(Count(expectedRdf, (byte)'<'), Count(rdf, (byte)'<'));
                    NUnit.Framework.Assert.IsNull(new CompareTool().CompareXmp(cmpFile, outFile, true));
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void TestPdfUAExtensionMetadata() {
            String outFile = destinationFolder + "testPdfUAExtensionMetadata.pdf";
            String cmpFile = cmpFolder + "cmp_testPdfUAExtensionMetadata.pdf";
            using (Stream fos = FileUtil.GetFileOutputStream(outFile)) {
                GeneratePdfAWithUA(fos);
            }
            CompareTool ct = new CompareTool();
            NUnit.Framework.Assert.IsNull(ct.CompareXmp(outFile, cmpFile, true));
        }

        [NUnit.Framework.Test]
        public virtual void TestPdfUAIdSchemaNameSpaceUriIsNotText() {
            ByteArrayOutputStream baos = new ByteArrayOutputStream();
            GeneratePdfAWithUA(baos);
            // check whether the pdfuaid NS URI was properly encoded as a URI with rdf:resource
            PdfDocument readDoc = new PdfDocument(new PdfReader(new MemoryStream(baos.ToArray())));
            String xmpString = iText.Commons.Utils.JavaUtil.GetStringForBytes(readDoc.GetXmpMetadataBytes(), System.Text.Encoding
                .UTF8);
            NUnit.Framework.Assert.IsTrue(xmpString.Contains("<pdfaSchema:namespaceURI rdf:resource=\"http://www.aiim.org/pdfua/ns/id/\"/>"
                ), "Did not find expected namespaceURI definition");
        }

        private void GeneratePdfAWithUA(Stream os) {
            WriterProperties wp = new WriterProperties().AddUAXmpMetadata();
            using (PdfWriter w = new PdfWriter(os, wp)) {
                PdfOutputIntent outputIntent;
                using (Stream @is = FileUtil.GetInputStreamForFile(sourceFolder + "sRGB Color Space Profile.icm")) {
                    outputIntent = new PdfOutputIntent("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1", @is);
                }
                PdfDocument pdfDoc = new PdfADocument(w, PdfAConformance.PDF_A_2A, outputIntent).SetTagged();
                pdfDoc.GetDocumentInfo().SetTitle("Test document");
                pdfDoc.GetCatalog().SetViewerPreferences(new PdfViewerPreferences().SetDisplayDocTitle(true));
                pdfDoc.GetCatalog().SetLang(new PdfString("en"));
                pdfDoc.AddNewPage();
                pdfDoc.Close();
            }
        }

        private int Count(byte[] array, byte b) {
            int counter = 0;
            foreach (byte each in array) {
                if (each == b) {
                    counter++;
                }
            }
            return counter;
        }
    }
}
