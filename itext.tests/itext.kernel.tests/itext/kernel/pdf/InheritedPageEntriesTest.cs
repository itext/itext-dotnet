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
using iText.IO.Font.Constants;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Utils;
using iText.Test;

namespace iText.Kernel.Pdf {
    [NUnit.Framework.Category("IntegrationTest")]
    public class InheritedPageEntriesTest : ExtendedITextTest {
        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/kernel/pdf/InheritedPageEntriesTest/";

        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/pdf/InheritedPageEntriesTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.OneTimeTearDown]
        public static void AfterClass() {
            CompareTool.Cleanup(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void AddNewPageToDocumentWithInheritedPageRotationTest() {
            //TODO: update cmp-files when DEVSIX-3635 will be fixed
            String inputFileName = sourceFolder + "srcFileTestRotationInheritance.pdf";
            String outputFileName = destinationFolder + "addNewPageToDocumentWithInheritedPageRotation.pdf";
            String cmpFileName = sourceFolder + "cmp_addNewPageToDocumentWithInheritedPageRotation.pdf";
            PdfDocument outFile = new PdfDocument(new PdfReader(inputFileName), CompareTool.CreateTestPdfWriter(outputFileName
                ));
            PdfPage page = outFile.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.BeginText().MoveText(36, 750).SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.HELVETICA), 16
                ).ShowText("Hello Helvetica!").EndText().SaveState();
            outFile.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outputFileName, cmpFileName, destinationFolder
                ));
        }

        [NUnit.Framework.Test]
        public virtual void SetRotationToPageTest() {
            String outputFileName = destinationFolder + "setRotationToPage.pdf";
            String cmpFileName = sourceFolder + "cmp_setRotationToPage.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(sourceFolder + "srcFileTestRotationInheritance.pdf"), CompareTool
                .CreateTestPdfWriter(outputFileName));
            PdfPage page = pdfDoc.GetPage(1);
            page.SetRotation(90);
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outputFileName, cmpFileName, destinationFolder
                ));
        }

        [NUnit.Framework.Test]
        public virtual void CopySeveralPagesToDocumentWithInheritedPageRotationTest() {
            //TODO: update cmp-files when DEVSIX-3635 will be fixed
            String outputFileName = destinationFolder + "copySeveralPagesToDocumentWithInheritedPageRotation.pdf";
            String cmpFileName = sourceFolder + "cmp_copySeveralPagesToDocumentWithInheritedPageRotation.pdf";
            PdfDocument pdfDoc1 = new PdfDocument(new PdfReader(sourceFolder + "noPagesRotation.pdf"));
            PdfDocument pdfDoc2 = new PdfDocument(new PdfReader(sourceFolder + "addSeveralPagesToDocumentWithInheritedPageRotation.pdf"
                ), CompareTool.CreateTestPdfWriter(outputFileName));
            pdfDoc1.CopyPagesTo(1, 2, pdfDoc2);
            pdfDoc1.Close();
            pdfDoc2.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outputFileName, cmpFileName, destinationFolder
                ));
        }

        [NUnit.Framework.Test]
        public virtual void MediaBoxInheritance() {
            String inputFileName = sourceFolder + "mediaBoxInheritanceTestSource.pdf";
            PdfDocument outFile = new PdfDocument(new PdfReader(inputFileName));
            PdfObject mediaBox = outFile.GetPage(1).GetPdfObject().Get(PdfName.MediaBox);
            //Check if MediaBox in Page is absent
            NUnit.Framework.Assert.IsNull(mediaBox);
            PdfArray array = outFile.GetCatalog().GetPdfObject().GetAsDictionary(PdfName.Pages).GetAsArray(PdfName.MediaBox
                );
            Rectangle rectangle = array.ToRectangle();
            Rectangle pageRect = outFile.GetPage(1).GetMediaBox();
            outFile.Close();
            NUnit.Framework.Assert.IsTrue(rectangle.EqualsWithEpsilon(pageRect));
        }

        [NUnit.Framework.Test]
        public virtual void CropBoxInheritance() {
            String inputFileName = sourceFolder + "cropBoxInheritanceTestSource.pdf";
            PdfDocument outFile = new PdfDocument(new PdfReader(inputFileName));
            PdfObject cropBox = outFile.GetPage(1).GetPdfObject().Get(PdfName.CropBox);
            //Check if CropBox in Page is absent
            NUnit.Framework.Assert.IsNull(cropBox);
            PdfArray array = outFile.GetCatalog().GetPdfObject().GetAsDictionary(PdfName.Pages).GetAsArray(PdfName.CropBox
                );
            Rectangle rectangle = array.ToRectangle();
            Rectangle pageRect = outFile.GetPage(1).GetCropBox();
            outFile.Close();
            NUnit.Framework.Assert.IsTrue(rectangle.EqualsWithEpsilon(pageRect));
        }
    }
}
