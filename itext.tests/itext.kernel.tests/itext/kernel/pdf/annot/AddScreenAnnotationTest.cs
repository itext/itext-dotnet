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
using iText.Commons.Utils;
using iText.IO.Font.Constants;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Action;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Filespec;
using iText.Kernel.Utils;
using iText.Test;

namespace iText.Kernel.Pdf.Annot {
    [NUnit.Framework.Category("IntegrationTest")]
    public class AddScreenAnnotationTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/pdf/annot/AddScreenAnnotationTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/kernel/pdf/annot/AddScreenAnnotationTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.OneTimeTearDown]
        public static void AfterClass() {
            CompareTool.Cleanup(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void ScreenEmbeddedWavFromPathTest() {
            String filename = destinationFolder + "screenEmbeddedWavFromPathTest.pdf";
            String cmp = sourceFolder + "cmp_" + "screenEmbeddedWavFromPathTest.pdf";
            using (PdfDocument pdfDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(filename))) {
                PdfFileSpec spec = PdfFileSpec.CreateEmbeddedFileSpec(pdfDoc, sourceFolder + "sample.wav", null, "sample.wav"
                    , null, null);
                AddPageWithScreenAnnotation(pdfDoc, spec);
            }
            String errorMessage = new CompareTool().CompareByContent(filename, cmp, destinationFolder);
            if (errorMessage != null) {
                NUnit.Framework.Assert.Fail(errorMessage);
            }
        }

        [NUnit.Framework.Test]
        public virtual void ScreenEmbeddedWavFromStreamTest() {
            String filename = destinationFolder + "screenEmbeddedWavFromStreamTest.pdf";
            String cmp = sourceFolder + "cmp_" + "screenEmbeddedWavFromStreamTest.pdf";
            using (Stream @is = FileUtil.GetInputStreamForFile(sourceFolder + "sample.wav")) {
                using (PdfDocument pdfDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(filename))) {
                    PdfFileSpec spec = PdfFileSpec.CreateEmbeddedFileSpec(pdfDoc, @is, null, "sample.wav", null, null);
                    AddPageWithScreenAnnotation(pdfDoc, spec);
                }
            }
            String errorMessage = new CompareTool().CompareByContent(filename, cmp, destinationFolder);
            if (errorMessage != null) {
                NUnit.Framework.Assert.Fail(errorMessage);
            }
        }

        [NUnit.Framework.Test]
        public virtual void ScreenEmbeddedWavFromBytesTest() {
            String filename = destinationFolder + "screenEmbeddedWavFromBytesTest.pdf";
            String cmp = sourceFolder + "cmp_" + "screenEmbeddedWavFromBytesTest.pdf";
            using (PdfDocument pdfDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(filename))) {
                byte[] fileStore = File.ReadAllBytes(System.IO.Path.Combine(sourceFolder + "sample.wav"));
                PdfFileSpec spec = PdfFileSpec.CreateEmbeddedFileSpec(pdfDoc, fileStore, null, "sample.wav", null, null, null
                    );
                AddPageWithScreenAnnotation(pdfDoc, spec);
            }
            String errorMessage = new CompareTool().CompareByContent(filename, cmp, destinationFolder);
            if (errorMessage != null) {
                NUnit.Framework.Assert.Fail(errorMessage);
            }
        }

        [NUnit.Framework.Test]
        public virtual void ScreenExternalWavTest() {
            String filename = destinationFolder + "screenExternalWavTest.pdf";
            String cmp = sourceFolder + "cmp_" + "screenExternalWavTest.pdf";
            using (PdfDocument pdfDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(filename))) {
                FileUtil.Copy(sourceFolder + "sample.wav", destinationFolder + "sample.wav");
                PdfFileSpec spec = PdfFileSpec.CreateExternalFileSpec(pdfDoc, "sample.wav");
                AddPageWithScreenAnnotation(pdfDoc, spec);
            }
            String errorMessage = new CompareTool().CompareByContent(filename, cmp, destinationFolder);
            if (errorMessage != null) {
                NUnit.Framework.Assert.Fail(errorMessage);
            }
        }

        private void AddPageWithScreenAnnotation(PdfDocument pdfDoc, PdfFileSpec spec) {
            PdfPage page1 = pdfDoc.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page1);
            canvas.SaveState().BeginText().MoveText(36, 105).SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.HELVETICA
                ), 16).ShowText("Click on the area below to play a sound.").EndText().RestoreState();
            PdfScreenAnnotation screen = new PdfScreenAnnotation(new Rectangle(100, 100));
            PdfAction action = PdfAction.CreateRendition("sample.wav", spec, "audio/x-wav", screen);
            screen.SetAction(action);
            page1.AddAnnotation(screen);
            page1.Flush();
        }
    }
}
