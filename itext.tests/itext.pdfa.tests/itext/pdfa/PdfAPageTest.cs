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
using iText.Kernel.Events;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Pdfa.Logs;
using iText.Test;
using iText.Test.Attributes;
using iText.Test.Pdfa;

namespace iText.Pdfa {
    // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf\a validation on Android)
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfAPageTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfa/";

        private static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/pdfa/PdfAPageTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        [LogMessage(PdfALogMessageConstant.PDFA_PAGE_FLUSHING_WAS_NOT_PERFORMED)]
        public virtual void CheckThatFlushingPreventedWhenAddingElementToDocument() {
            // Expected log message that page flushing was not performed
            String outPdf = destinationFolder + "checkThatFlushingPreventedWhenAddingElementToDocument.pdf";
            PdfWriter writer = new PdfWriter(outPdf);
            PdfADocument pdfDoc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_1A, new PdfOutputIntent("Custom"
                , "", "http://www.color.org", "sRGB IEC61966-2.1", new FileStream(sourceFolder + "sRGB Color Space Profile.icm"
                , FileMode.Open, FileAccess.Read)));
            pdfDoc.SetTagged();
            pdfDoc.GetCatalog().SetLang(new PdfString("en-US"));
            PdfAPageTest.EndPageEventHandler eventHandler = new PdfAPageTest.EndPageEventHandler();
            pdfDoc.AddEventHandler(PdfDocumentEvent.END_PAGE, eventHandler);
            int pageCount = 3;
            Document document = new Document(pdfDoc, PageSize.A4);
            for (int i = 1; i < pageCount; i++) {
                // Adding a area break causes a new page to be added and an attempt to flush the page will occur,
                // but flushing these pages will be prevented due to a condition added to the PdfAPage#flush method
                document.Add(new AreaBreak());
            }
            // Before closing document have 3 pages, but no one call of end page event
            NUnit.Framework.Assert.AreEqual(pageCount, document.GetPdfDocument().GetNumberOfPages());
            NUnit.Framework.Assert.AreEqual(0, eventHandler.GetCounter());
            document.Close();
            // During the closing event was called on each document page
            NUnit.Framework.Assert.AreEqual(pageCount, eventHandler.GetCounter());
            NUnit.Framework.Assert.IsNull(new VeraPdfValidator().Validate(outPdf));
        }

        // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf\a validation on Android)
        [NUnit.Framework.Test]
        [LogMessage(PdfALogMessageConstant.PDFA_PAGE_FLUSHING_WAS_NOT_PERFORMED)]
        public virtual void CheckThatFlushingPreventedWithFalseFlushResourcesContentStreams() {
            // Expected log message that page flushing was not performed
            String outPdf = destinationFolder + "checkThatFlushingPreventedWithFalseFlushResourcesContentStreams.pdf";
            PdfWriter writer = new PdfWriter(outPdf);
            PdfADocument pdfDoc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_1A, new PdfOutputIntent("Custom"
                , "", "http://www.color.org", "sRGB IEC61966-2.1", new FileStream(sourceFolder + "sRGB Color Space Profile.icm"
                , FileMode.Open, FileAccess.Read)));
            pdfDoc.SetTagged();
            pdfDoc.GetCatalog().SetLang(new PdfString("en-US"));
            PdfAPageTest.EndPageEventHandler eventHandler = new PdfAPageTest.EndPageEventHandler();
            pdfDoc.AddEventHandler(PdfDocumentEvent.END_PAGE, eventHandler);
            int pageCount = 3;
            for (int i = 0; i < pageCount; i++) {
                pdfDoc.AddNewPage().Flush(false);
            }
            NUnit.Framework.Assert.AreEqual(pageCount, pdfDoc.GetNumberOfPages());
            NUnit.Framework.Assert.AreEqual(0, eventHandler.GetCounter());
            pdfDoc.Close();
            NUnit.Framework.Assert.AreEqual(pageCount, eventHandler.GetCounter());
            NUnit.Framework.Assert.IsNull(new VeraPdfValidator().Validate(outPdf));
        }

        // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf\a validation on Android)
        [NUnit.Framework.Test]
        public virtual void CheckFlushingWhenPdfDocumentIsClosing() {
            String outPdf = destinationFolder + "checkFlushingWhenPdfDocumentIsClosing.pdf";
            PdfWriter writer = new PdfWriter(outPdf);
            PdfADocument pdfDoc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_1A, new PdfOutputIntent("Custom"
                , "", "http://www.color.org", "sRGB IEC61966-2.1", new FileStream(sourceFolder + "sRGB Color Space Profile.icm"
                , FileMode.Open, FileAccess.Read)));
            pdfDoc.SetTagged();
            pdfDoc.GetCatalog().SetLang(new PdfString("en-US"));
            PdfAPageTest.EndPageEventHandler eventHandler = new PdfAPageTest.EndPageEventHandler();
            pdfDoc.AddEventHandler(PdfDocumentEvent.END_PAGE, eventHandler);
            int pageCount = 3;
            for (int i = 0; i < pageCount; i++) {
                pdfDoc.AddNewPage();
            }
            NUnit.Framework.Assert.AreEqual(pageCount, pdfDoc.GetNumberOfPages());
            NUnit.Framework.Assert.AreEqual(0, eventHandler.GetCounter());
            pdfDoc.Close();
            NUnit.Framework.Assert.AreEqual(pageCount, eventHandler.GetCounter());
            NUnit.Framework.Assert.IsNull(new VeraPdfValidator().Validate(outPdf));
        }

        // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf\a validation on Android)
        [NUnit.Framework.Test]
        public virtual void CheckFlushingWithTrueFlushResourcesContentStreams() {
            String outPdf = destinationFolder + "checkFlushingWithTrueFlushResourcesContentStreams.pdf";
            PdfWriter writer = new PdfWriter(outPdf);
            PdfADocument pdfDoc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_1A, new PdfOutputIntent("Custom"
                , "", "http://www.color.org", "sRGB IEC61966-2.1", new FileStream(sourceFolder + "sRGB Color Space Profile.icm"
                , FileMode.Open, FileAccess.Read)));
            pdfDoc.SetTagged();
            pdfDoc.GetCatalog().SetLang(new PdfString("en-US"));
            PdfAPageTest.EndPageEventHandler eventHandler = new PdfAPageTest.EndPageEventHandler();
            pdfDoc.AddEventHandler(PdfDocumentEvent.END_PAGE, eventHandler);
            int pageCount = 3;
            for (int i = 0; i < pageCount; i++) {
                pdfDoc.AddNewPage().Flush(true);
            }
            NUnit.Framework.Assert.AreEqual(pageCount, pdfDoc.GetNumberOfPages());
            NUnit.Framework.Assert.AreEqual(pageCount, eventHandler.GetCounter());
            pdfDoc.Close();
            NUnit.Framework.Assert.AreEqual(pageCount, eventHandler.GetCounter());
            NUnit.Framework.Assert.IsNull(new VeraPdfValidator().Validate(outPdf));
        }

        // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf\a validation on Android)
        [NUnit.Framework.Test]
        public virtual void CheckFlushingOfCheckedPage() {
            String outPdf = destinationFolder + "checkFlushingOfCheckedPage.pdf";
            PdfWriter writer = new PdfWriter(outPdf);
            PdfADocument pdfDoc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_1A, new PdfOutputIntent("Custom"
                , "", "http://www.color.org", "sRGB IEC61966-2.1", new FileStream(sourceFolder + "sRGB Color Space Profile.icm"
                , FileMode.Open, FileAccess.Read)));
            pdfDoc.SetTagged();
            pdfDoc.GetCatalog().SetLang(new PdfString("en-US"));
            PdfAPageTest.EndPageEventHandler eventHandler = new PdfAPageTest.EndPageEventHandler();
            pdfDoc.AddEventHandler(PdfDocumentEvent.END_PAGE, eventHandler);
            int pageCount = 3;
            for (int i = 0; i < pageCount; i++) {
                PdfPage page = pdfDoc.AddNewPage();
                pdfDoc.checker.CheckSinglePage(page);
                page.Flush(false);
            }
            NUnit.Framework.Assert.AreEqual(pageCount, pdfDoc.GetNumberOfPages());
            NUnit.Framework.Assert.AreEqual(pageCount, eventHandler.GetCounter());
            pdfDoc.Close();
            NUnit.Framework.Assert.AreEqual(pageCount, eventHandler.GetCounter());
            NUnit.Framework.Assert.IsNull(new VeraPdfValidator().Validate(outPdf));
        }

        // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf\a validation on Android)
        internal class EndPageEventHandler : iText.Kernel.Events.IEventHandler {
            private int counter = 0;

            internal EndPageEventHandler() {
            }

            public virtual int GetCounter() {
                return counter;
            }

            public virtual void HandleEvent(Event @event) {
                counter++;
            }
        }
    }
}
