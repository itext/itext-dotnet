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
using iText.IO.Font;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Tagging;
using iText.Kernel.Pdf.Tagutils;
using iText.Kernel.Pdf.Xobject;
using iText.Kernel.Utils;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Logs;
using iText.Pdfua;
using iText.Pdfua.Exceptions;
using iText.Test;
using iText.Test.Attributes;
using iText.Test.Pdfa;

namespace iText.Pdfua.Checkers {
    // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf/ua validation on Android)
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfUACanvasXObjectTest : ExtendedITextTest {
        private static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/pdfua/PdfUACanvasXObjectTest/";

        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfua/PdfUACanvasXObjectTest/";

        private static readonly String DOG = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfua/img/DOG.bmp";

        private static readonly String FONT = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfua/font/FreeSans.ttf";

        private UaValidationTestFramework framework;

        [NUnit.Framework.OneTimeSetUp]
        public static void Before() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.SetUp]
        public virtual void InitializeFramework() {
            framework = new UaValidationTestFramework(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        [LogMessage(LayoutLogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA, Count = 1)]
        public virtual void CopyPageAsFormXobjectWithTaggedPdf() {
            String outPdf = DESTINATION_FOLDER + "xobjectTesting.pdf";
            String inputPdf = SOURCE_FOLDER + "cmp_manualPdfUaCreation.pdf";
            String cmpFIle = SOURCE_FOLDER + "cmp_xobjectTesting.pdf";
            PdfUATestPdfDocument doc = new PdfUATestPdfDocument(new PdfWriter(outPdf));
            PdfDocument inputDoc = new PdfDocument(new PdfReader(inputPdf));
            PdfFormXObject xObject = inputDoc.GetFirstPage().CopyAsFormXObject(doc);
            Document document = new Document(doc);
            Image img = new Image(xObject);
            img.GetAccessibilityProperties().SetAlternateDescription("Some description");
            document.Add(img);
            document.Close();
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpFIle, DESTINATION_FOLDER, "diff_"
                ));
            VeraPdfValidator validator = new VeraPdfValidator();
            // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf/ua validation on Android)
            // We expect verapdf to fail because we are embedding tagged content which contains artifacts
            NUnit.Framework.Assert.IsNotNull("We expect vera pdf to fail, because we are embedding tagged content which contains artifacts into a tagged item"
                , validator.Validate(outPdf));
        }

        // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf/ua validation on Android)
        [NUnit.Framework.Test]
        [LogMessage(LayoutLogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA, Count = 1)]
        public virtual void CopyPageAsFormXobjectWithUnTaggedContentButInvalidBecauseOfFont() {
            //itext should thrown an exception here but it does not.
            // because even if it's not tagged the inner content stream is not compliant as the font is not embeded
            String outputPdf = DESTINATION_FOLDER + "copyPageAsFormXobjectWithUnTaggedPdf.pdf";
            String cmpFile = SOURCE_FOLDER + "cmp_copyPageAsFormXobjectWithUnTaggedPdf.pdf";
            MemoryStream os = new MemoryStream();
            PdfDocument dummyDoc = new PdfDocument(new PdfWriter(os));
            Document document = new Document(dummyDoc);
            document.Add(new Paragraph("Hello World!"));
            document.Close();
            PdfDocument pdfDoc = new PdfUATestPdfDocument(new PdfWriter(outputPdf));
            PdfFormXObject xObject = new PdfDocument(new PdfReader(new MemoryStream(os.ToArray()))).GetFirstPage().CopyAsFormXObject
                (pdfDoc);
            Image img = new Image(xObject);
            img.GetAccessibilityProperties().SetAlternateDescription("Some description");
            Document doc = new Document(pdfDoc);
            doc.Add(img);
            doc.Close();
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outputPdf, cmpFile, DESTINATION_FOLDER, "diff_"
                ));
            VeraPdfValidator validator = new VeraPdfValidator();
            // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf/ua validation on Android)
            NUnit.Framework.Assert.IsNotNull("Fails are expected because the content inside the xobject isn't valid because of not embedded font, and iText doesn't parse the content streams"
                , validator.Validate(outputPdf));
        }

        // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf/ua validation on Android)
        [LogMessage(LayoutLogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA, Count = 1)]
        [NUnit.Framework.Test]
        public virtual void CopyPageAsFormWithUntaggedContentAndCorrectFont() {
            String outputPdf = DESTINATION_FOLDER + "copyPageAsFormWithCorrectFontXobjectWithUnTaggedPdf.pdf";
            String cmpFile = SOURCE_FOLDER + "cmp_copyPageAsFormWithCorrectFontXobjectWithUnTaggedPdf.pdf";
            MemoryStream os = new MemoryStream();
            PdfDocument dummyDoc = new PdfDocument(new PdfWriter(os));
            Document document = new Document(dummyDoc);
            PdfFont font = PdfFontFactory.CreateFont(FONT, PdfEncodings.WINANSI, PdfFontFactory.EmbeddingStrategy.PREFER_EMBEDDED
                );
            document.Add(new Paragraph("Hello World!").SetFont(font));
            document.Close();
            PdfDocument pdfDoc = new PdfUATestPdfDocument(new PdfWriter(outputPdf));
            PdfFormXObject xObject = new PdfDocument(new PdfReader(new MemoryStream(os.ToArray()))).GetFirstPage().CopyAsFormXObject
                (pdfDoc);
            Image img = new Image(xObject);
            img.GetAccessibilityProperties().SetAlternateDescription("Some description");
            Document doc = new Document(pdfDoc);
            doc.Add(img);
            doc.Close();
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outputPdf, cmpFile, DESTINATION_FOLDER, "diff_"
                ));
            VeraPdfValidator validator = new VeraPdfValidator();
            // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf/ua validation on Android)
            NUnit.Framework.Assert.IsNull(validator.Validate(outputPdf));
        }

        // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf/ua validation on Android)
        [NUnit.Framework.Test]
        public virtual void ManuallyAddToCanvasWithUnTaggedContentButBadFont() {
            String outputPdf = DESTINATION_FOLDER + "manuallyAddToCanvasWithUnTaggedPdf.pdf";
            String cmpFile = SOURCE_FOLDER + "cmp_manuallyAddToCanvasWithUnTaggedPdf.pdf";
            MemoryStream os = new MemoryStream();
            PdfDocument dummyDoc = new PdfDocument(new PdfWriter(os));
            Document document = new Document(dummyDoc);
            document.Add(new Paragraph("Hello World!"));
            document.Close();
            PdfDocument pdfDoc = new PdfUATestPdfDocument(new PdfWriter(outputPdf));
            PdfFormXObject xObject = new PdfDocument(new PdfReader(new MemoryStream(os.ToArray()))).GetFirstPage().CopyAsFormXObject
                (pdfDoc);
            PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage());
            canvas.BeginMarkedContent(PdfName.Artifact);
            canvas.AddXObject(xObject);
            canvas.EndMarkedContent();
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outputPdf, cmpFile, DESTINATION_FOLDER, "diff_"
                ));
            VeraPdfValidator validator = new VeraPdfValidator();
            // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf/ua validation on Android)
            NUnit.Framework.Assert.IsNotNull("Content of the xobject is not valid causing it to be an non compliant", 
                validator.Validate(outputPdf));
        }

        // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf/ua validation on Android)
        [NUnit.Framework.Test]
        public virtual void ManuallyAddToCanvasCorrectFontAndUnTaggedContent() {
            framework.AddBeforeGenerationHook((pdfDoc) => {
                try {
                    MemoryStream os = new MemoryStream();
                    PdfDocument dummyDoc = new PdfDocument(new PdfWriter(os));
                    Document document = new Document(dummyDoc);
                    PdfFont font = PdfFontFactory.CreateFont(FONT, PdfEncodings.WINANSI, PdfFontFactory.EmbeddingStrategy.FORCE_EMBEDDED
                        );
                    document.Add(new Paragraph("Hello World!").SetFont(font));
                    document.Close();
                    PdfFormXObject xObject = null;
                    xObject = new PdfDocument(new PdfReader(new MemoryStream(os.ToArray()))).GetFirstPage().CopyAsFormXObject(
                        pdfDoc);
                    PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage());
                    TagTreePointer tagPointer = pdfDoc.GetTagStructureContext().GetAutoTaggingPointer().AddTag(StandardRoles.DIV
                        );
                    tagPointer.SetPageForTagging(pdfDoc.GetPage(1));
                    canvas.OpenTag(tagPointer.GetTagReference());
                    canvas.AddXObject(xObject);
                    canvas.CloseTag();
                }
                catch (System.IO.IOException) {
                    throw new Exception();
                }
            }
            );
            framework.AssertBothValid("manuallyAddToCanvasCorrectFontAndUnTaggedContent");
        }

        [NUnit.Framework.Test]
        public virtual void ManuallyAddToCanvasAndCorrectFontAndArtifactUnTaggedContent() {
            //Now we are again adding untagged content with some artifacts and embedded font's so we should also be fine
            framework.AddBeforeGenerationHook((pdfDocument) => {
                MemoryStream os = new MemoryStream();
                PdfDocument dummyDoc = new PdfDocument(new PdfWriter(os));
                Document document = new Document(dummyDoc);
                try {
                    PdfFont font = PdfFontFactory.CreateFont(FONT, PdfEncodings.WINANSI, PdfFontFactory.EmbeddingStrategy.FORCE_EMBEDDED
                        );
                    document.Add(new Paragraph("Hello World!").SetFont(font).SetBorder(new SolidBorder(ColorConstants.CYAN, 2)
                        ));
                    document.Close();
                    PdfFormXObject xObject = new PdfDocument(new PdfReader(new MemoryStream(os.ToArray()))).GetFirstPage().CopyAsFormXObject
                        (pdfDocument);
                    PdfCanvas canvas = new PdfCanvas(pdfDocument.AddNewPage());
                    TagTreePointer tagPointer = pdfDocument.GetTagStructureContext().GetAutoTaggingPointer().AddTag(StandardRoles
                        .DIV);
                    tagPointer.SetPageForTagging(pdfDocument.GetPage(1));
                    canvas.OpenTag(tagPointer.GetTagReference());
                    canvas.AddXObject(xObject);
                    canvas.CloseTag();
                }
                catch (System.IO.IOException) {
                    throw new Exception();
                }
            }
            );
            framework.AssertBothValid("manuallyAddToCanvasAndCorrectFontAndArtifactUnTaggedContent");
        }

        [NUnit.Framework.Test]
        public virtual void ManuallyAddToCanvasAndCorrectFontAndArtifactTaggedContent() {
            String outputPdf = DESTINATION_FOLDER + "manuallyAddToCanvasWithUnAndCorrectFontAndArtifactUnPdf.pdf";
            String cmpFile = SOURCE_FOLDER + "cmp_manuallyAddToCanvasWithUnAndCorrectFontUnAndArtifactPdf.pdf";
            MemoryStream os = new MemoryStream();
            PdfDocument dummyDoc = new PdfDocument(new PdfWriter(os));
            dummyDoc.SetTagged();
            Document document = new Document(dummyDoc);
            PdfFont font = PdfFontFactory.CreateFont(FONT, PdfEncodings.WINANSI, PdfFontFactory.EmbeddingStrategy.FORCE_EMBEDDED
                );
            document.Add(new Paragraph("Hello World!").SetFont(font).SetBorder(new SolidBorder(ColorConstants.CYAN, 2)
                ));
            document.Close();
            PdfDocument pdfDoc = new PdfUATestPdfDocument(new PdfWriter(outputPdf));
            PdfFormXObject xObject = new PdfDocument(new PdfReader(new MemoryStream(os.ToArray()))).GetFirstPage().CopyAsFormXObject
                (pdfDoc);
            PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage());
            TagTreePointer tagPointer = pdfDoc.GetTagStructureContext().GetAutoTaggingPointer().AddTag(StandardRoles.DIV
                );
            tagPointer.SetPageForTagging(pdfDoc.GetPage(1));
            canvas.OpenTag(tagPointer.GetTagReference());
            canvas.AddXObject(xObject);
            canvas.CloseTag();
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outputPdf, cmpFile, DESTINATION_FOLDER, "diff_"
                ));
            VeraPdfValidator validator = new VeraPdfValidator();
            // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf/ua validation on Android)
            NUnit.Framework.Assert.IsNotNull("The content is non compliant because it contains both artifacts, and real content"
                , validator.Validate(outputPdf));
        }

        // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf/ua validation on Android)
        [NUnit.Framework.Test]
        public virtual void ManuallyAddToCanvasAndCorrectFontAndArtifactTaggedContentInsideArtifact() {
            // We are adding tagged content to an artifact. Looks like Verapdf doesn't check xobject stream at all because
            // page content is marked as artifact. We think it's wrong though.
            framework.AddBeforeGenerationHook((pdfDoc) => {
                try {
                    MemoryStream os = new MemoryStream();
                    PdfDocument dummyDoc = new PdfDocument(new PdfWriter(os));
                    dummyDoc.SetTagged();
                    Document document = new Document(dummyDoc);
                    PdfFont font = PdfFontFactory.CreateFont(FONT, PdfEncodings.WINANSI, PdfFontFactory.EmbeddingStrategy.FORCE_EMBEDDED
                        );
                    document.Add(new Paragraph("Hello World!").SetFont(font).SetBorder(new SolidBorder(ColorConstants.CYAN, 2)
                        ));
                    document.Close();
                    PdfFormXObject xObject = new PdfDocument(new PdfReader(new MemoryStream(os.ToArray()))).GetFirstPage().CopyAsFormXObject
                        (pdfDoc);
                    PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage());
                    canvas.OpenTag(new CanvasArtifact());
                    canvas.AddXObject(xObject);
                    canvas.CloseTag();
                }
                catch (System.IO.IOException) {
                    throw new Exception();
                }
            }
            );
            framework.AssertBothValid("manuallyAddToCanvasAndCorrectFontInsideArtifact");
        }

        [NUnit.Framework.Test]
        public virtual void ManuallyAddToCanvasAndCorrectFontAndArtifactTaggedContentInsideUntaggedPageContent() {
            MemoryStream os = new MemoryStream();
            PdfDocument dummyDoc = new PdfDocument(new PdfWriter(os));
            dummyDoc.SetTagged();
            Document document = new Document(dummyDoc);
            PdfFont font = PdfFontFactory.CreateFont(FONT, PdfEncodings.WINANSI, PdfFontFactory.EmbeddingStrategy.FORCE_EMBEDDED
                );
            document.Add(new Paragraph("Hello World!").SetFont(font).SetBorder(new SolidBorder(ColorConstants.CYAN, 2)
                ));
            document.Close();
            dummyDoc.Close();
            framework.AddBeforeGenerationHook((pdfDocument) => {
                PdfCanvas canvas = new PdfCanvas(pdfDocument.AddNewPage());
                PdfFormXObject xObject = null;
                try {
                    xObject = new PdfDocument(new PdfReader(new MemoryStream(os.ToArray()))).GetFirstPage().CopyAsFormXObject(
                        pdfDocument);
                }
                catch (System.IO.IOException) {
                    throw new Exception();
                }
                canvas.AddXObject(xObject);
            }
            );
            framework.AssertBothFail("untaggedAddXobject", PdfUAExceptionMessageConstants.TAG_HASNT_BEEN_ADDED_BEFORE_CONTENT_ADDING
                , false);
        }

        [NUnit.Framework.Test]
        public virtual void ManuallyAddToCanvasAtLocationAndCorrectFontAndArtifactTaggedContentInsideUntaggedPageContent
            () {
            //We are adding untagged content we should throw an exception
            MemoryStream os = new MemoryStream();
            PdfDocument dummyDoc = new PdfDocument(new PdfWriter(os));
            dummyDoc.SetTagged();
            Document document = new Document(dummyDoc);
            PdfFont font = PdfFontFactory.CreateFont(FONT, PdfEncodings.WINANSI, PdfFontFactory.EmbeddingStrategy.FORCE_EMBEDDED
                );
            document.Add(new Paragraph("Hello World!").SetFont(font).SetBorder(new SolidBorder(ColorConstants.CYAN, 2)
                ));
            document.Close();
            dummyDoc.Close();
            framework.AddBeforeGenerationHook((pdfDocument) => {
                PdfCanvas canvas = new PdfCanvas(pdfDocument.AddNewPage());
                PdfFormXObject xObject = null;
                try {
                    xObject = new PdfDocument(new PdfReader(new MemoryStream(os.ToArray()))).GetFirstPage().CopyAsFormXObject(
                        pdfDocument);
                }
                catch (System.IO.IOException) {
                    throw new Exception();
                }
                canvas.AddXObjectAt(xObject, 200f, 200f);
            }
            );
            framework.AssertBothFail("untaggedAddXobjectAt", PdfUAExceptionMessageConstants.TAG_HASNT_BEEN_ADDED_BEFORE_CONTENT_ADDING
                , false);
        }

        [NUnit.Framework.Test]
        public virtual void ManuallyAddToCanvasAtLocationAndCorrectFontAndArtifactTaggedContentInsideUntaggedPageContenta
            () {
            //We are adding untagged content we should throw an exception
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            MemoryStream os = new MemoryStream();
            PdfDocument dummyDoc = new PdfDocument(new PdfWriter(os));
            Document document = new Document(dummyDoc);
            PdfFont font = PdfFontFactory.CreateFont(FONT, PdfEncodings.WINANSI, PdfFontFactory.EmbeddingStrategy.FORCE_EMBEDDED
                );
            document.Add(new Paragraph("Hello World!").SetFont(font).SetBorder(new SolidBorder(ColorConstants.CYAN, 2)
                ));
            document.Close();
            dummyDoc.Close();
            framework.AddBeforeGenerationHook((pdfDocument) => {
                PdfCanvas canvas = new PdfCanvas(pdfDocument.AddNewPage());
                PdfFormXObject xObject = null;
                try {
                    xObject = new PdfDocument(new PdfReader(new MemoryStream(os.ToArray()))).GetFirstPage().CopyAsFormXObject(
                        pdfDocument);
                }
                catch (System.IO.IOException) {
                    throw new Exception();
                }
                canvas.AddXObjectFittedIntoRectangle(xObject, new Rectangle(200, 200, 200, 200));
            }
            );
            framework.AssertBothFail("addXObjectFitted", PdfUAExceptionMessageConstants.TAG_HASNT_BEEN_ADDED_BEFORE_CONTENT_ADDING
                , false);
        }

        [NUnit.Framework.Test]
        public virtual void ManuallyAddToCanvasAtLocationAndCorrectFontAndArtifactTaggedContentInsideUntaggedPageContentab
            () {
            //We are adding untagged content we should throw an exception
            MemoryStream os = new MemoryStream();
            PdfDocument dummyDoc = new PdfDocument(new PdfWriter(os));
            Document document = new Document(dummyDoc);
            PdfFont font = PdfFontFactory.CreateFont(FONT, PdfEncodings.WINANSI, PdfFontFactory.EmbeddingStrategy.FORCE_EMBEDDED
                );
            document.Add(new Paragraph("Hello World!").SetFont(font).SetBorder(new SolidBorder(ColorConstants.CYAN, 2)
                ));
            document.Close();
            dummyDoc.Close();
            framework.AddBeforeGenerationHook((pdfDocument) => {
                PdfCanvas canvas = new PdfCanvas(pdfDocument.AddNewPage());
                PdfFormXObject xObject = null;
                try {
                    xObject = new PdfDocument(new PdfReader(new MemoryStream(os.ToArray()))).GetFirstPage().CopyAsFormXObject(
                        pdfDocument);
                }
                catch (System.IO.IOException) {
                    throw new Exception();
                }
                canvas.AddXObjectWithTransformationMatrix(xObject, 1, 1, 1, 1, 1, 1);
            }
            );
            framework.AssertBothFail("addXObjectWithTransfoMatrix", PdfUAExceptionMessageConstants.TAG_HASNT_BEEN_ADDED_BEFORE_CONTENT_ADDING
                , false);
        }

        [NUnit.Framework.Test]
        public virtual void AddImageObjectNotInline() {
            //We are adding untagged content we should throw an exception
            framework.AddBeforeGenerationHook((pdfDocument) => {
                PdfCanvas canvas = new PdfCanvas(pdfDocument.AddNewPage());
                ImageData imd = null;
                try {
                    imd = ImageDataFactory.Create(DOG);
                }
                catch (System.IO.IOException) {
                    throw new Exception();
                }
                canvas.AddImageAt(imd, 200, 200, false);
            }
            );
            framework.AssertBothFail("addIMageObjectNotInline", PdfUAExceptionMessageConstants.TAG_HASNT_BEEN_ADDED_BEFORE_CONTENT_ADDING
                , false);
        }

        [NUnit.Framework.Test]
        public virtual void AddImageObjectInline() {
            //We are adding untagged content we should throw an exception
            framework.AddBeforeGenerationHook((pdfDocument) => {
                PdfCanvas canvas = new PdfCanvas(pdfDocument.AddNewPage());
                ImageData imd = null;
                try {
                    imd = ImageDataFactory.Create(DOG);
                }
                catch (System.IO.IOException) {
                    throw new Exception();
                }
                canvas.AddImageAt(imd, 200, 200, false);
            }
            );
            framework.AssertBothFail("addIMageObjectInline", PdfUAExceptionMessageConstants.TAG_HASNT_BEEN_ADDED_BEFORE_CONTENT_ADDING
                , false);
        }

        [NUnit.Framework.Test]
        public virtual void AddImageTranformationMatrix() {
            //We are adding untagged content we should throw an exception
            framework.AddBeforeGenerationHook((pdfDocument) => {
                PdfCanvas canvas = new PdfCanvas(pdfDocument.AddNewPage());
                ImageData imd = null;
                try {
                    imd = ImageDataFactory.Create(DOG);
                }
                catch (System.IO.IOException) {
                    throw new Exception();
                }
                canvas.AddImageWithTransformationMatrix(imd, 1, 1, 1, 1, 1, 1, false);
            }
            );
            framework.AssertBothFail("addIMageObjectTransfo", PdfUAExceptionMessageConstants.TAG_HASNT_BEEN_ADDED_BEFORE_CONTENT_ADDING
                , false);
        }

        [NUnit.Framework.Test]
        public virtual void AddImageFittedIntoRectangle() {
            //We are adding untagged content we should throw an exception
            framework.AddBeforeGenerationHook((pdfDocument) => {
                PdfCanvas canvas = new PdfCanvas(pdfDocument.AddNewPage());
                ImageData imd = null;
                try {
                    imd = ImageDataFactory.Create(DOG);
                }
                catch (System.IO.IOException) {
                    throw new Exception();
                }
                canvas.AddImageFittedIntoRectangle(imd, new Rectangle(200, 200, 200, 200), false);
            }
            );
            framework.AssertBothFail("addImageFittedIntoRectangle", PdfUAExceptionMessageConstants.TAG_HASNT_BEEN_ADDED_BEFORE_CONTENT_ADDING
                , false);
        }
    }
}
