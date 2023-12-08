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
using iText.IO.Image;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Xobject;
using iText.Kernel.Utils;
using iText.Layout;
using iText.Layout.Element;
using iText.Pdfa.Logs;
using iText.Test;
using iText.Test.Attributes;
using iText.Test.Pdfa;

namespace iText.Pdfa {
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfAFlushingTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfa/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/pdfa/PdfAFlushingTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        [LogMessage(PdfALogMessageConstant.PDFA_OBJECT_FLUSHING_WAS_NOT_PERFORMED)]
        public virtual void FlushingTest01() {
            String outPdf = destinationFolder + "pdfA1b_flushingTest01.pdf";
            String cmpPdf = sourceFolder + "cmp/PdfAFlushingTest/cmp_pdfA1b_flushingTest01.pdf";
            PdfWriter writer = new PdfWriter(outPdf);
            Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read);
            PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_1B, new PdfOutputIntent("Custom", ""
                , "http://www.color.org", "sRGB IEC61966-2.1", @is));
            PdfCanvas canvas = new PdfCanvas(doc.AddNewPage());
            PdfImageXObject imageXObject = new PdfImageXObject(ImageDataFactory.Create(sourceFolder + "Desert.jpg"));
            imageXObject.MakeIndirect(doc);
            canvas.AddXObjectFittedIntoRectangle(imageXObject, new Rectangle(30, 300, 300, 300));
            imageXObject.Flush();
            if (imageXObject.IsFlushed()) {
                NUnit.Framework.Assert.Fail("Flushing of unchecked objects shall be forbidden.");
            }
            doc.Close();
            CompareResult(outPdf, cmpPdf);
        }

        [NUnit.Framework.Test]
        [LogMessage(PdfALogMessageConstant.PDFA_PAGE_FLUSHING_WAS_NOT_PERFORMED)]
        public virtual void FlushingTest02() {
            String outPdf = destinationFolder + "pdfA2b_flushingTest02.pdf";
            String cmpPdf = sourceFolder + "cmp/PdfAFlushingTest/cmp_pdfA2b_flushingTest02.pdf";
            PdfWriter writer = new PdfWriter(outPdf);
            Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read);
            PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_2B, new PdfOutputIntent("Custom", ""
                , "http://www.color.org", "sRGB IEC61966-2.1", @is));
            PdfCanvas canvas = new PdfCanvas(doc.AddNewPage());
            PdfImageXObject imageXObject = new PdfImageXObject(ImageDataFactory.Create(sourceFolder + "Desert.jpg"));
            imageXObject.MakeIndirect(doc);
            canvas.AddXObjectFittedIntoRectangle(imageXObject, new Rectangle(30, 300, 300, 300));
            PdfPage lastPage = doc.GetLastPage();
            lastPage.Flush();
            if (lastPage.IsFlushed()) {
                NUnit.Framework.Assert.Fail("Flushing of unchecked objects shall be forbidden.");
            }
            doc.Close();
            CompareResult(outPdf, cmpPdf);
        }

        [NUnit.Framework.Test]
        public virtual void FlushingTest03() {
            String outPdf = destinationFolder + "pdfA3b_flushingTest03.pdf";
            String cmpPdf = sourceFolder + "cmp/PdfAFlushingTest/cmp_pdfA3b_flushingTest03.pdf";
            PdfWriter writer = new PdfWriter(outPdf);
            Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read);
            PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_3B, new PdfOutputIntent("Custom", ""
                , "http://www.color.org", "sRGB IEC61966-2.1", @is));
            PdfCanvas canvas = new PdfCanvas(doc.AddNewPage());
            PdfImageXObject imageXObject = new PdfImageXObject(ImageDataFactory.Create(sourceFolder + "Desert.jpg"));
            canvas.AddXObjectFittedIntoRectangle(imageXObject, new Rectangle(30, 300, 300, 300));
            PdfPage lastPage = doc.GetLastPage();
            lastPage.Flush(true);
            if (!imageXObject.IsFlushed()) {
                NUnit.Framework.Assert.Fail("When flushing the page along with it's resources, page check should be performed also page and all resources should be flushed."
                    );
            }
            doc.Close();
            CompareResult(outPdf, cmpPdf);
        }

        [NUnit.Framework.Test]
        [LogMessage(PdfALogMessageConstant.PDFA_OBJECT_FLUSHING_WAS_NOT_PERFORMED, LogLevel = LogLevelConstants.WARN
            )]
        public virtual void TryToFlushFontTest() {
            String outPdf = destinationFolder + "tryToFlushFontTest.pdf";
            String cmpPdf = sourceFolder + "cmp_tryToFlushFontTest.pdf";
            PdfWriter writer = new PdfWriter(outPdf, new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0));
            Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read);
            PdfADocument pdfDoc = (PdfADocument)new PdfADocument(writer, PdfAConformanceLevel.PDF_A_4, new PdfOutputIntent
                ("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1", @is)).SetTagged();
            PdfFont font = PdfFontFactory.CreateFont(sourceFolder + "FreeSans.ttf", "WinAnsi", PdfFontFactory.EmbeddingStrategy
                .FORCE_EMBEDDED);
            font.MakeIndirect(pdfDoc);
            Document document = new Document(pdfDoc);
            document.SetFont(font);
            List list = new List();
            list.Add("123");
            // nothing happen (only log message was written)
            font.Flush();
            document.Add(list);
            NUnit.Framework.Assert.AreEqual(PdfVersion.PDF_2_0, pdfDoc.GetTagStructureContext().GetTagStructureTargetVersion
                ());
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, destinationFolder, "diff"
                ));
            NUnit.Framework.Assert.IsNull(new VeraPdfValidator().Validate(outPdf));
        }

        // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf\a validation on Android)
        [NUnit.Framework.Test]
        [LogMessage(PdfALogMessageConstant.PDFA_OBJECT_FLUSHING_WAS_NOT_PERFORMED)]
        public virtual void AddUnusedStreamObjectsTest() {
            String outPdf = destinationFolder + "pdfA1b_docWithUnusedObjects_3.pdf";
            String cmpPdf = sourceFolder + "cmp/PdfAFlushingTest/cmp_pdfA1b_docWithUnusedObjects_3.pdf";
            PdfWriter writer = new PdfWriter(outPdf);
            Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read);
            PdfADocument pdfDocument = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_1B, new PdfOutputIntent("Custom"
                , "", "http://www.color.org", "sRGB IEC61966-2.1", @is));
            pdfDocument.AddNewPage();
            PdfDictionary unusedDictionary = new PdfDictionary();
            PdfArray unusedArray = (PdfArray)new PdfArray().MakeIndirect(pdfDocument);
            unusedArray.Add(new PdfNumber(42));
            PdfStream stream = new PdfStream(new byte[] { 1, 2, 34, 45 }, 0);
            unusedArray.Add(stream);
            unusedDictionary.Put(new PdfName("testName"), unusedArray);
            unusedDictionary.MakeIndirect(pdfDocument).Flush();
            unusedDictionary.Flush();
            pdfDocument.Close();
            PdfReader testerReader = new PdfReader(outPdf);
            PdfDocument testerDocument = new PdfDocument(testerReader);
            NUnit.Framework.Assert.AreEqual(testerDocument.ListIndirectReferences().Count, 11);
            testerDocument.Close();
            CompareResult(outPdf, cmpPdf);
        }

        private void CompareResult(String outFile, String cmpFile) {
            String differences = new CompareTool().CompareByContent(outFile, cmpFile, destinationFolder, "diff_");
            if (differences != null) {
                NUnit.Framework.Assert.Fail(differences);
            }
        }
    }
}
