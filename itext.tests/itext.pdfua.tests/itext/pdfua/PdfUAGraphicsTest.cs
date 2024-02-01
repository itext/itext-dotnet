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
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Tagging;
using iText.Kernel.Pdf.Tagutils;
using iText.Kernel.Utils;
using iText.Layout;
using iText.Layout.Element;
using iText.Pdfua.Checkers.Utils;
using iText.Pdfua.Exceptions;
using iText.Test;
using iText.Test.Pdfa;

namespace iText.Pdfua {
    // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf\a validation on Android)
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfUAGraphicsTest : ExtendedITextTest {
        private static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/pdfua/PdfUAGraphicsTest/";

        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfua/PdfUAGraphicsTest/";

        private static readonly String DOG = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfua/img/DOG.bmp";

        private static readonly String FONT = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfua/font/FreeSans.ttf";

        [NUnit.Framework.OneTimeSetUp]
        public static void Before() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void ImageWithoutAlternativeDescription_ThrowsInLayout() {
            PdfDocument pdfDoc = new PdfUATestPdfDocument(new PdfWriter(new MemoryStream(), new WriterProperties().AddUAXmpMetadata
                ().SetPdfVersion(PdfVersion.PDF_1_7)));
            Document document = new Document(pdfDoc);
            iText.Layout.Element.Image img = new Image(ImageDataFactory.Create(DOG));
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfUAConformanceException), () => {
                document.Add(img);
            }
            );
            NUnit.Framework.Assert.AreEqual(PdfUAExceptionMessageConstants.IMAGE_SHALL_HAVE_ALT, e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void LayoutCheckUtilTest() {
            NUnit.Framework.Assert.DoesNotThrow(() => LayoutCheckUtil.CheckLayoutElements(null));
        }

        [NUnit.Framework.Test]
        public virtual void ImageWithEmptyAlternativeDescription_ThrowsInLayout() {
            PdfDocument pdfDoc = new PdfUATestPdfDocument(new PdfWriter(new MemoryStream(), new WriterProperties().AddUAXmpMetadata
                ().SetPdfVersion(PdfVersion.PDF_1_7)));
            Document document = new Document(pdfDoc);
            iText.Layout.Element.Image img = new iText.Layout.Element.Image(ImageDataFactory.Create(DOG));
            img.GetAccessibilityProperties().SetAlternateDescription("");
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfUAConformanceException), () => {
                document.Add(img);
            }
            );
            NUnit.Framework.Assert.AreEqual(PdfUAExceptionMessageConstants.IMAGE_SHALL_HAVE_ALT, e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void ImageWithValidAlternativeDescription_OK() {
            String OUTPUT_FILE = DESTINATION_FOLDER + "imageWithValidAlternativeDescription_OK.pdf";
            PdfDocument pdfDoc = new PdfUATestPdfDocument(new PdfWriter(OUTPUT_FILE, new WriterProperties().AddUAXmpMetadata
                ().SetPdfVersion(PdfVersion.PDF_1_7)));
            Document document = new Document(pdfDoc);
            iText.Layout.Element.Image img = new iText.Layout.Element.Image(ImageDataFactory.Create(DOG));
            img.GetAccessibilityProperties().SetAlternateDescription("Alternative description");
            document.Add(img);
            document.Close();
            NUnit.Framework.Assert.IsNull(new VeraPdfValidator().Validate(OUTPUT_FILE));
            // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf\a validation on Android)
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(OUTPUT_FILE, SOURCE_FOLDER + "cmp_imageWithValidAlternativeDescription_OK.pdf"
                , DESTINATION_FOLDER, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void ImageWithValidActualText_OK() {
            String OUTPUT_FILE = DESTINATION_FOLDER + "imageWithValidActualText_OK.pdf";
            PdfDocument pdfDoc = new PdfUATestPdfDocument(new PdfWriter(OUTPUT_FILE, new WriterProperties().AddUAXmpMetadata
                ().SetPdfVersion(PdfVersion.PDF_1_7)));
            Document document = new Document(pdfDoc);
            iText.Layout.Element.Image img = new iText.Layout.Element.Image(ImageDataFactory.Create(DOG));
            img.GetAccessibilityProperties().SetActualText("Actual text");
            document.Add(img);
            document.Close();
            NUnit.Framework.Assert.IsNull(new VeraPdfValidator().Validate(OUTPUT_FILE));
            // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf\a validation on Android)
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(OUTPUT_FILE, SOURCE_FOLDER + "cmp_imageWithValidActualText_OK.pdf"
                , DESTINATION_FOLDER, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void ImageWithCaption_OK() {
            String OUTPUT_FILE = DESTINATION_FOLDER + "imageWithCaption_OK.pdf";
            PdfDocument pdfDoc = new PdfUATestPdfDocument(new PdfWriter(OUTPUT_FILE, new WriterProperties().AddUAXmpMetadata
                ().SetPdfVersion(PdfVersion.PDF_1_7)));
            Document document = new Document(pdfDoc);
            Div imgWithCaption = new Div();
            imgWithCaption.GetAccessibilityProperties().SetRole(StandardRoles.FIGURE);
            imgWithCaption.GetAccessibilityProperties().SetAlternateDescription("Alternative description");
            iText.Layout.Element.Image img = new iText.Layout.Element.Image(ImageDataFactory.Create(DOG));
            img.SetNeutralRole();
            Paragraph caption = new Paragraph("Caption");
            caption.SetFont(PdfFontFactory.CreateFont(FONT));
            caption.GetAccessibilityProperties().SetRole(StandardRoles.CAPTION);
            imgWithCaption.Add(img);
            imgWithCaption.Add(caption);
            document.Add(imgWithCaption);
            document.Close();
            NUnit.Framework.Assert.IsNull(new VeraPdfValidator().Validate(OUTPUT_FILE));
            // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf\a validation on Android)
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(OUTPUT_FILE, SOURCE_FOLDER + "cmp_imageWithCaption_OK.pdf"
                , DESTINATION_FOLDER, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void ImageWithCaptionWithoutAlternateDescription_Throws() {
            PdfDocument pdfDoc = new PdfUATestPdfDocument(new PdfWriter(new MemoryStream(), new WriterProperties().AddUAXmpMetadata
                ().SetPdfVersion(PdfVersion.PDF_1_7)));
            Document document = new Document(pdfDoc);
            Div imgWithCaption = new Div();
            imgWithCaption.GetAccessibilityProperties().SetRole(StandardRoles.FIGURE);
            iText.Layout.Element.Image img = new iText.Layout.Element.Image(ImageDataFactory.Create(DOG));
            img.SetNeutralRole();
            Paragraph caption = new Paragraph("Caption");
            caption.SetFont(PdfFontFactory.CreateFont(FONT));
            caption.GetAccessibilityProperties().SetRole(StandardRoles.CAPTION);
            imgWithCaption.Add(img);
            imgWithCaption.Add(caption);
            // will not throw in layout but will throw on close this is expected
            document.Add(imgWithCaption);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfUAConformanceException), () => {
                document.Close();
            }
            );
            NUnit.Framework.Assert.AreEqual(PdfUAExceptionMessageConstants.IMAGE_SHALL_HAVE_ALT, e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void ImageWithoutActualText_ThrowsInLayout() {
            PdfDocument pdfDoc = new PdfUATestPdfDocument(new PdfWriter(new MemoryStream(), new WriterProperties().AddUAXmpMetadata
                ().SetPdfVersion(PdfVersion.PDF_1_7)));
            Document document = new Document(pdfDoc);
            iText.Layout.Element.Image img = new iText.Layout.Element.Image(ImageDataFactory.Create(DOG));
            img.GetAccessibilityProperties().SetActualText(null);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfUAConformanceException), () => {
                document.Add(img);
            }
            );
            NUnit.Framework.Assert.AreEqual(PdfUAExceptionMessageConstants.IMAGE_SHALL_HAVE_ALT, e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void ImageWithEmptyActualText_ThrowsInLayout() {
            PdfDocument pdfDoc = new PdfUATestPdfDocument(new PdfWriter(new MemoryStream(), new WriterProperties().AddUAXmpMetadata
                ().SetPdfVersion(PdfVersion.PDF_1_7)));
            Document document = new Document(pdfDoc);
            iText.Layout.Element.Image img = new iText.Layout.Element.Image(ImageDataFactory.Create(DOG));
            img.GetAccessibilityProperties().SetActualText("");
            NUnit.Framework.Assert.DoesNotThrow(() => document.Add(img));
        }

        [NUnit.Framework.Test]
        public virtual void ImageDirectlyOnCanvas_OK() {
            String OUTPUT_FILE = DESTINATION_FOLDER + "imageDirectlyOnCanvas_OK.pdf";
            PdfDocument pdfDoc = new PdfUATestPdfDocument(new PdfWriter(OUTPUT_FILE, new WriterProperties().AddUAXmpMetadata
                ().SetPdfVersion(PdfVersion.PDF_1_7)));
            Document document = new Document(pdfDoc);
            iText.Layout.Element.Image img = new iText.Layout.Element.Image(ImageDataFactory.Create(DOG));
            img.GetAccessibilityProperties().SetAlternateDescription("Hello");
            document.Add(img);
            iText.Layout.Element.Image img2 = new iText.Layout.Element.Image(ImageDataFactory.Create(DOG));
            img2.GetAccessibilityProperties().SetActualText("Some actual text on layout img");
            document.Add(img2);
            TagTreePointer pointerForImage = new TagTreePointer(pdfDoc);
            PdfPage page = pdfDoc.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            pointerForImage.SetPageForTagging(page);
            TagTreePointer tmp = pointerForImage.AddTag(StandardRoles.FIGURE);
            tmp.GetProperties().SetActualText("Some text");
            canvas.OpenTag(tmp.GetTagReference());
            canvas.AddImageAt(ImageDataFactory.Create(DOG), 400, 400, false);
            canvas.CloseTag();
            TagTreePointer ttp = pointerForImage.AddTag(StandardRoles.FIGURE);
            ttp.GetProperties().SetAlternateDescription("Alternate description");
            canvas.OpenTag(ttp.GetTagReference());
            canvas.AddImageAt(ImageDataFactory.Create(DOG), 200, 200, false);
            canvas.CloseTag();
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new VeraPdfValidator().Validate(OUTPUT_FILE));
            // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf\a validation on Android)
            new CompareTool().CompareByContent(OUTPUT_FILE, SOURCE_FOLDER + "cmp_imageDirectlyOnCanvas_OK.pdf", DESTINATION_FOLDER
                , "diff_");
        }

        [NUnit.Framework.Test]
        public virtual void ImageDirectlyOnCanvasWithoutAlternateDescription_ThrowsOnClose() {
            PdfDocument pdfDoc = new PdfUATestPdfDocument(new PdfWriter(new MemoryStream(), new WriterProperties().AddUAXmpMetadata
                ().SetPdfVersion(PdfVersion.PDF_1_7)));
            TagTreePointer pointerForImage = new TagTreePointer(pdfDoc);
            PdfPage page = pdfDoc.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            pointerForImage.SetPageForTagging(page);
            TagTreePointer tmp = pointerForImage.AddTag(StandardRoles.FIGURE);
            canvas.OpenTag(tmp.GetTagReference());
            canvas.AddImageAt(ImageDataFactory.Create(DOG), 200, 200, false);
            canvas.CloseTag();
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfUAConformanceException), () => {
                pdfDoc.Close();
            }
            );
            NUnit.Framework.Assert.AreEqual(PdfUAExceptionMessageConstants.IMAGE_SHALL_HAVE_ALT, e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void ImageDirectlyOnCanvasWithEmptyActualText_OK() {
            PdfDocument pdfDoc = new PdfUATestPdfDocument(new PdfWriter(new MemoryStream(), new WriterProperties().AddUAXmpMetadata
                ().SetPdfVersion(PdfVersion.PDF_1_7)));
            TagTreePointer pointerForImage = new TagTreePointer(pdfDoc);
            PdfPage page = pdfDoc.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            pointerForImage.SetPageForTagging(page);
            TagTreePointer tmp = pointerForImage.AddTag(StandardRoles.FIGURE);
            tmp.GetProperties().SetActualText("");
            canvas.OpenTag(tmp.GetTagReference());
            canvas.AddImageAt(ImageDataFactory.Create(DOG), 200, 200, false);
            canvas.CloseTag();
            NUnit.Framework.Assert.DoesNotThrow(() => pdfDoc.Close());
        }

        [NUnit.Framework.Test]
        public virtual void TestOverflowImage() {
            PdfDocument pdfDoc = new PdfUATestPdfDocument(new PdfWriter(new MemoryStream(), new WriterProperties().AddUAXmpMetadata
                ().SetPdfVersion(PdfVersion.PDF_1_7)));
            iText.Layout.Element.Image img = new iText.Layout.Element.Image(ImageDataFactory.Create(DOG));
            Document document = new Document(pdfDoc);
            document.Add(new Div().SetHeight(730).SetBackgroundColor(ColorConstants.CYAN));
            NUnit.Framework.Assert.Catch(typeof(PdfUAConformanceException), () => {
                document.Add(img);
            }
            );
        }

        [NUnit.Framework.Test]
        public virtual void TestEmbeddedImageInTable() {
            PdfDocument pdfDoc = new PdfUATestPdfDocument(new PdfWriter(new MemoryStream(), new WriterProperties().AddUAXmpMetadata
                ().SetPdfVersion(PdfVersion.PDF_1_7)));
            iText.Layout.Element.Image img = new iText.Layout.Element.Image(ImageDataFactory.Create(DOG));
            Document document = new Document(pdfDoc);
            Table table = new Table(2);
            for (int i = 0; i <= 20; i++) {
                table.AddCell(new Paragraph("Cell " + i));
            }
            table.AddCell(img);
            NUnit.Framework.Assert.Catch(typeof(PdfUAConformanceException), () => {
                document.Add(table);
            }
            );
        }

        [NUnit.Framework.Test]
        public virtual void TestEmbeddedImageInDiv() {
            PdfDocument pdfDoc = new PdfUATestPdfDocument(new PdfWriter(new MemoryStream(), new WriterProperties().AddUAXmpMetadata
                ().SetPdfVersion(PdfVersion.PDF_1_7)));
            iText.Layout.Element.Image img = new iText.Layout.Element.Image(ImageDataFactory.Create(DOG));
            Document document = new Document(pdfDoc);
            Div div = new Div();
            div.Add(img);
            NUnit.Framework.Assert.Catch(typeof(PdfUAConformanceException), () => {
                document.Add(div);
            }
            );
        }

        [NUnit.Framework.Test]
        public virtual void TestEmbeddedImageInParagraph() {
            PdfDocument pdfDoc = new PdfUATestPdfDocument(new PdfWriter(new MemoryStream(), new WriterProperties().AddUAXmpMetadata
                ().SetPdfVersion(PdfVersion.PDF_1_7)));
            iText.Layout.Element.Image img = new iText.Layout.Element.Image(ImageDataFactory.Create(DOG));
            Document document = new Document(pdfDoc);
            Div div = new Div();
            div.Add(img);
            NUnit.Framework.Assert.Catch(typeof(PdfUAConformanceException), () => {
                document.Add(div);
            }
            );
        }
    }
}
