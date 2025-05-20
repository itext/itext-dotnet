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
using System.Collections.Generic;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Tagging;
using iText.Kernel.Pdf.Tagutils;
using iText.Layout;
using iText.Layout.Element;
using iText.Pdfua;
using iText.Pdfua.Checkers.Utils;
using iText.Pdfua.Exceptions;
using iText.Test;

namespace iText.Pdfua.Checkers {
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfUAGraphicsTest : ExtendedITextTest {
        private static readonly String DESTINATION_FOLDER = TestUtil.GetOutputPath() + "/pdfua/PdfUAGraphicsTest/";

        private static readonly String DOG = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfua/img/DOG.bmp";

        private static readonly String FONT = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfua/font/FreeSans.ttf";

        [NUnit.Framework.OneTimeSetUp]
        public static void Before() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        public static IList<PdfUAConformance> Data() {
            return UaValidationTestFramework.GetConformanceList();
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void ImageWithoutAlternativeDescription_ThrowsInLayout(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDocument) => {
                Document document = new Document(pdfDocument);
                Image img = LoadImage();
                document.Add(img);
            }
            );
            framework.AssertBothFail("imageNoAltDescription", PdfUAExceptionMessageConstants.IMAGE_SHALL_HAVE_ALT, pdfUAConformance
                );
        }

        [NUnit.Framework.Test]
        public virtual void LayoutCheckUtilTest() {
            NUnit.Framework.Assert.DoesNotThrow(() => new LayoutCheckUtil(null).CheckRenderer(null));
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void ImageWithEmptyAlternativeDescription_ThrowsInLayout(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDocument) => {
                Document document = new Document(pdfDocument);
                Image img = LoadImage();
                img.GetAccessibilityProperties().SetAlternateDescription("");
                document.Add(img);
            }
            );
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothFail("imageWithEmptyAltDescription", PdfUAExceptionMessageConstants.IMAGE_SHALL_HAVE_ALT
                    , pdfUAConformance);
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertOnlyITextFail("imageWithEmptyAltDescription", PdfUAExceptionMessageConstants.IMAGE_SHALL_HAVE_ALT
                        , pdfUAConformance);
                }
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void ImageCustomRole_Ok(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDocument) => {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    PdfNamespace @namespace = new PdfNamespace(StandardNamespaces.PDF_2_0);
                    pdfDocument.GetTagStructureContext().SetDocumentDefaultNamespace(@namespace);
                    pdfDocument.GetStructTreeRoot().AddNamespace(@namespace);
                    @namespace.AddNamespaceRoleMapping("CustomImage", StandardRoles.FIGURE);
                }
                PdfStructTreeRoot root = pdfDocument.GetStructTreeRoot();
                root.AddRoleMapping("CustomImage", StandardRoles.FIGURE);
            }
            );
            framework.AddSuppliers(new _Generator_127());
            framework.AssertBothValid("imageWithCustomRoleOk", pdfUAConformance);
        }

        private sealed class _Generator_127 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_127() {
            }

            public IBlockElement Generate() {
                Image img = PdfUAGraphicsTest.LoadImage();
                img.GetAccessibilityProperties().SetRole("CustomImage");
                img.GetAccessibilityProperties().SetAlternateDescription("ff");
                return new Div().Add(img);
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void ImageCustomDoubleMapping_Ok(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDocument) => {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    PdfNamespace @namespace = new PdfNamespace(StandardNamespaces.PDF_2_0);
                    pdfDocument.GetTagStructureContext().SetDocumentDefaultNamespace(@namespace);
                    pdfDocument.GetStructTreeRoot().AddNamespace(@namespace);
                    @namespace.AddNamespaceRoleMapping("CustomImage", StandardRoles.FIGURE);
                    @namespace.AddNamespaceRoleMapping("CustomImage2", "CustomImage");
                }
                PdfStructTreeRoot root = pdfDocument.GetStructTreeRoot();
                root.AddRoleMapping("CustomImage", StandardRoles.FIGURE);
                root.AddRoleMapping("CustomImage2", "CustomImage");
            }
            );
            framework.AddSuppliers(new _Generator_155());
            framework.AssertBothValid("imageWithDoubleMapping", pdfUAConformance);
        }

        private sealed class _Generator_155 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_155() {
            }

            public IBlockElement Generate() {
                Image img = PdfUAGraphicsTest.LoadImage();
                img.GetAccessibilityProperties().SetRole("CustomImage2");
                img.GetAccessibilityProperties().SetAlternateDescription("ff");
                return new Div().Add(img);
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void ImageCustomRoleNoAlternateDescription_Throws(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDocument) => {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    PdfNamespace @namespace = new PdfNamespace(StandardNamespaces.PDF_2_0);
                    pdfDocument.GetTagStructureContext().SetDocumentDefaultNamespace(@namespace);
                    pdfDocument.GetStructTreeRoot().AddNamespace(@namespace);
                    @namespace.AddNamespaceRoleMapping("CustomImage", StandardRoles.FIGURE);
                }
                PdfStructTreeRoot root = pdfDocument.GetStructTreeRoot();
                root.AddRoleMapping("CustomImage", StandardRoles.FIGURE);
            }
            );
            framework.AddSuppliers(new _Generator_181());
            framework.AssertBothFail("imageWithCustomRoleAndNoDescription", pdfUAConformance);
        }

        private sealed class _Generator_181 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_181() {
            }

            public IBlockElement Generate() {
                Image img = PdfUAGraphicsTest.LoadImage();
                img.GetAccessibilityProperties().SetRole("CustomImage");
                return new Div().Add(img);
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void ImageCustomDoubleMapping_Throws(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDocument) => {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    PdfNamespace @namespace = new PdfNamespace(StandardNamespaces.PDF_2_0);
                    pdfDocument.GetTagStructureContext().SetDocumentDefaultNamespace(@namespace);
                    pdfDocument.GetStructTreeRoot().AddNamespace(@namespace);
                    @namespace.AddNamespaceRoleMapping("CustomImage", StandardRoles.FIGURE);
                    @namespace.AddNamespaceRoleMapping("CustomImage2", "CustomImage");
                }
                PdfStructTreeRoot root = pdfDocument.GetStructTreeRoot();
                root.AddRoleMapping("CustomImage", StandardRoles.FIGURE);
                root.AddRoleMapping("CustomImage2", "CustomImage");
            }
            );
            framework.AddSuppliers(new _Generator_209());
            framework.AssertBothFail("imageCustomDoubleMapping_Throws", pdfUAConformance);
        }

        private sealed class _Generator_209 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_209() {
            }

            public IBlockElement Generate() {
                Image img = PdfUAGraphicsTest.LoadImage();
                img.GetAccessibilityProperties().SetRole("CustomImage2");
                return new Div().Add(img);
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void ImageWithValidAlternativeDescription_OK(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDocument) => {
                Document document = new Document(pdfDocument);
                Image img = LoadImage();
                img.GetAccessibilityProperties().SetAlternateDescription("Alternative description");
                document.Add(img);
            }
            );
            framework.AssertBothValid("imageWithValidAltDescr", pdfUAConformance);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void ImageWithValidActualText_OK(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDocument) => {
                Document document = new Document(pdfDocument);
                Image img = LoadImage();
                img.GetAccessibilityProperties().SetActualText("Actual text");
                document.Add(img);
            }
            );
            framework.AssertBothValid("imageWithValidActualText", pdfUAConformance);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void ImageWithCaption_OK(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDocument) => {
                Document document = new Document(pdfDocument);
                Div imgWithCaption = new Div();
                imgWithCaption.GetAccessibilityProperties().SetRole(StandardRoles.FIGURE);
                imgWithCaption.GetAccessibilityProperties().SetAlternateDescription("Alternative description");
                Image img = LoadImage();
                img.SetNeutralRole();
                Paragraph caption = new Paragraph("Caption");
                try {
                    caption.SetFont(PdfFontFactory.CreateFont(FONT));
                }
                catch (System.IO.IOException e) {
                    throw new Exception(e.Message);
                }
                caption.GetAccessibilityProperties().SetRole(StandardRoles.CAPTION);
                imgWithCaption.Add(img);
                imgWithCaption.Add(caption);
                document.Add(imgWithCaption);
            }
            );
            framework.AssertBothValid("imageWithCaption_OK", pdfUAConformance);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void ImageWithCaptionWithoutAlternateDescription_Throws(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDocument) => {
                Document document = new Document(pdfDocument);
                Div imgWithCaption = new Div();
                imgWithCaption.GetAccessibilityProperties().SetRole(StandardRoles.FIGURE);
                Image img = LoadImage();
                img.SetNeutralRole();
                Paragraph caption = new Paragraph("Caption");
                try {
                    caption.SetFont(PdfFontFactory.CreateFont(FONT));
                }
                catch (System.IO.IOException e) {
                    throw new Exception(e.Message);
                }
                caption.GetAccessibilityProperties().SetRole(StandardRoles.CAPTION);
                imgWithCaption.Add(img);
                imgWithCaption.Add(caption);
                // will not throw in layout but will throw on close this is expected
                document.Add(imgWithCaption);
            }
            );
            framework.AssertBothFail("imageWithCaptionWithoutAltDescr", PdfUAExceptionMessageConstants.IMAGE_SHALL_HAVE_ALT
                , pdfUAConformance);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void ImageWithoutActualText_ThrowsInLayout(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDocument) => {
                Document document = new Document(pdfDocument);
                Image img = LoadImage();
                img.GetAccessibilityProperties().SetActualText(null);
                document.Add(img);
            }
            );
            framework.AssertBothFail("imageWithoutActualText", PdfUAExceptionMessageConstants.IMAGE_SHALL_HAVE_ALT, pdfUAConformance
                );
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void ImageWithEmptyActualText_ThrowsInLayout(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                Document document = new Document(pdfDoc);
                Image img = LoadImage();
                img.GetAccessibilityProperties().SetActualText("");
                document.Add(img);
            }
            );
            framework.AssertBothValid("imageWithEmptyActualText", pdfUAConformance);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void ImageDirectlyOnCanvas_OK(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                try {
                    Document document = new Document(pdfDoc);
                    iText.Layout.Element.Image img = new Image(ImageDataFactory.Create(DOG));
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
                }
                catch (UriFormatException e) {
                    throw new Exception(e.Message);
                }
            }
            );
            framework.AssertBothValid("imageDirectlyOnCanvas", pdfUAConformance);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void ImageDirectlyOnCanvasWithoutAlternateDescription_ThrowsOnClose(PdfUAConformance pdfUAConformance
            ) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                TagTreePointer pointerForImage = new TagTreePointer(pdfDoc);
                PdfPage page = pdfDoc.AddNewPage();
                PdfCanvas canvas = new PdfCanvas(page);
                pointerForImage.SetPageForTagging(page);
                TagTreePointer tmp = pointerForImage.AddTag(StandardRoles.FIGURE);
                canvas.OpenTag(tmp.GetTagReference());
                try {
                    canvas.AddImageAt(ImageDataFactory.Create(DOG), 200, 200, false);
                }
                catch (UriFormatException e) {
                    throw new Exception(e.Message);
                }
                canvas.CloseTag();
            }
            );
            framework.AssertBothFail("canvasWithoutAltDescr", PdfUAExceptionMessageConstants.IMAGE_SHALL_HAVE_ALT, pdfUAConformance
                );
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void ImageDirectlyOnCanvasWithEmptyActualText_OK(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                TagTreePointer pointerForImage = new TagTreePointer(pdfDoc);
                PdfPage page = pdfDoc.AddNewPage();
                PdfCanvas canvas = new PdfCanvas(page);
                pointerForImage.SetPageForTagging(page);
                TagTreePointer tmp = pointerForImage.AddTag(StandardRoles.FIGURE);
                tmp.GetProperties().SetActualText("");
                canvas.OpenTag(tmp.GetTagReference());
                try {
                    canvas.AddImageAt(ImageDataFactory.Create(DOG), 200, 200, false);
                }
                catch (UriFormatException e) {
                    throw new Exception(e.Message);
                }
                canvas.CloseTag();
            }
            );
            framework.AssertBothValid("imageOnCanvasEmptyActualText", pdfUAConformance);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestOverflowImage(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                iText.Layout.Element.Image img = LoadImage();
                Document document = new Document(pdfDoc);
                document.Add(new Div().SetHeight(730).SetBackgroundColor(ColorConstants.CYAN));
                document.Add(img);
            }
            );
            framework.AssertBothFail("overflowImage", pdfUAConformance);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestEmbeddedImageInTable(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                iText.Layout.Element.Image img = LoadImage();
                Document document = new Document(pdfDoc);
                Table table = new Table(2);
                for (int i = 0; i <= 20; i++) {
                    table.AddCell(new Paragraph("Cell " + i));
                }
                table.AddCell(img);
                document.Add(table);
            }
            );
            framework.AssertBothFail("embeddedImageInTable", PdfUAExceptionMessageConstants.IMAGE_SHALL_HAVE_ALT, pdfUAConformance
                );
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestEmbeddedImageInDiv(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                iText.Layout.Element.Image img = LoadImage();
                Document document = new Document(pdfDoc);
                Div div = new Div();
                div.Add(img);
                document.Add(div);
            }
            );
            framework.AssertBothFail("embeddedImageInDiv", PdfUAExceptionMessageConstants.IMAGE_SHALL_HAVE_ALT, pdfUAConformance
                );
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TestEmbeddedImageInParagraph(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                iText.Layout.Element.Image img = LoadImage();
                Document document = new Document(pdfDoc);
                Paragraph paragraph = new Paragraph();
                paragraph.Add(img);
                document.Add(paragraph);
            }
            );
            framework.AssertBothFail("embeddedImageInParagraph", PdfUAExceptionMessageConstants.IMAGE_SHALL_HAVE_ALT, 
                pdfUAConformance);
        }

        private static iText.Layout.Element.Image LoadImage() {
            try {
                return new iText.Layout.Element.Image(ImageDataFactory.Create(DOG));
            }
            catch (UriFormatException e) {
                throw new Exception(e.Message);
            }
        }
    }
}
