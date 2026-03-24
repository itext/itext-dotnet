/*
This file is part of the iText (R) project.
Copyright (c) 1998-2026 Apryse Group NV
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
using System.IO;
using iText.Commons.Utils;
using iText.IO.Font;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Exceptions;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Tagging;
using iText.Kernel.Pdf.Tagutils;
using iText.Kernel.Pdf.Xobject;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Logs;
using iText.Pdfua;
using iText.Pdfua.Exceptions;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Pdfua.Checkers {
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfUACanvasXObjectTest : ExtendedITextTest {
        private static readonly String DESTINATION_FOLDER = TestUtil.GetOutputPath() + "/pdfua/PdfUACanvasXObjectTest/";

        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfua/PdfUACanvasXObjectTest/";

        private static readonly String DOG = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfua/img/DOG.bmp";

        private static readonly String FONT = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfua/font/FreeSans.ttf";

        [NUnit.Framework.OneTimeSetUp]
        public static void Before() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        public static IList<PdfConformance> Data() {
            return UaValidationTestFramework.GetConformanceList();
        }

        [NUnit.Framework.TestCaseSource("Data")]
        [LogMessage(LayoutLogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA, Count = 2)]
        public virtual void CopyPageAsFormXobjectWithTaggedPdf(PdfConformance conformance) {
            String inputPdf = SOURCE_FOLDER + "cmp_manualPdfUaCreation.pdf";
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers((document) => {
                try {
                    PdfDocument inputDoc = new PdfDocument(new PdfReader(inputPdf));
                    PdfFormXObject xObject = inputDoc.GetFirstPage().CopyAsFormXObject(document);
                    Image img = new Image(xObject);
                    img.GetAccessibilityProperties().SetAlternateDescription("Some description");
                    return new Div().Add(img);
                }
                catch (System.IO.IOException e) {
                    throw new PdfException(e);
                }
            }
            );
            if (framework.IsPdf2Based(conformance)) {
                framework.AssertBothValid("xobjectTesting");
            }
            else {
                framework.AssertOnlyVeraPdfFail("xobjectTesting");
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        [LogMessage(LayoutLogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA, Count = 2)]
        public virtual void CopyPageAsFormXobjectWithUnTaggedContentButInvalidBecauseOfFont(PdfConformance conformance
            ) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers((pdfDoc) => {
                try {
                    MemoryStream os = new MemoryStream();
                    PdfDocument dummyDoc = new PdfDocument(new PdfWriter(os));
                    Document document = new Document(dummyDoc);
                    document.Add(new Paragraph("Hello World!"));
                    document.Close();
                    PdfFormXObject xObject = new PdfDocument(new PdfReader(new MemoryStream(os.ToArray()))).GetFirstPage().CopyAsFormXObject
                        (pdfDoc);
                    Image img = new Image(xObject);
                    img.GetAccessibilityProperties().SetAlternateDescription("Some description");
                    return new Div().Add(img);
                }
                catch (Exception e) {
                    throw new PdfException(e);
                }
            }
            );
            //itext should thrown an exception here but it does not.
            // because even if it's not tagged the inner content stream is not compliant as the font is not embeded
            framework.AssertOnlyVeraPdfFail("copyPageAsFormXobjectWithUnTaggedPdf");
        }

        [LogMessage(LayoutLogMessageConstant.ELEMENT_DOES_NOT_FIT_AREA, Count = 2)]
        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void CopyPageAsFormWithUntaggedContentAndCorrectFont(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers((pdfDoc) => {
                try {
                    MemoryStream os = new MemoryStream();
                    PdfDocument dummyDoc = new PdfDocument(new PdfWriter(os));
                    Document document = new Document(dummyDoc);
                    PdfFont font = PdfFontFactory.CreateFont(FONT, PdfEncodings.WINANSI, PdfFontFactory.EmbeddingStrategy.PREFER_EMBEDDED
                        );
                    document.Add(new Paragraph("Hello World!").SetFont(font));
                    document.Close();
                    PdfFormXObject xObject = new PdfDocument(new PdfReader(new MemoryStream(os.ToArray()))).GetFirstPage().CopyAsFormXObject
                        (pdfDoc);
                    Image img = new Image(xObject);
                    img.GetAccessibilityProperties().SetAlternateDescription("Some description");
                    return new Div().Add(img);
                }
                catch (Exception e) {
                    throw new PdfException(e);
                }
            }
            );
            framework.AssertBothValid("copyPageAsFormWithUntaggedContentAndCorrectFont");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void ManuallyAddToCanvasWithUnTaggedContentButBadFont(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddAfterGenerationHook((pdfDoc) => {
                try {
                    MemoryStream os = new MemoryStream();
                    PdfDocument dummyDoc = new PdfDocument(new PdfWriter(os));
                    Document document = new Document(dummyDoc);
                    document.Add(new Paragraph("Hello World!"));
                    document.Close();
                    PdfFormXObject xObject = new PdfDocument(new PdfReader(new MemoryStream(os.ToArray()))).GetFirstPage().CopyAsFormXObject
                        (pdfDoc);
                    PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage());
                    canvas.BeginMarkedContent(PdfName.Artifact);
                    canvas.AddXObject(xObject);
                    canvas.EndMarkedContent();
                }
                catch (Exception e) {
                    throw new PdfException(e);
                }
            }
            );
            framework.AssertOnlyVeraPdfFail("manuallyAddToCanvasWithUnTaggedContentButBadFont");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void ManuallyAddToCanvasCorrectFontAndUnTaggedContent(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
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
                catch (System.IO.IOException e) {
                    throw new PdfException(e);
                }
            }
            );
            if (framework.IsPdf2Based(conformance)) {
                String message = MessageFormatUtil.Format(KernelExceptionMessageConstant.PARENT_CHILD_ROLE_RELATION_IS_NOT_ALLOWED
                    , "Div", "CONTENT");
                framework.AssertBothFail("addToCanvasCorrectFontUnTaggedContent", message);
            }
            else {
                framework.AssertBothValid("addToCanvasCorrectFontUnTaggedContent");
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void ManuallyAddToCanvasAndCorrectFontAndArtifactUnTaggedContent(PdfConformance conformance
            ) {
            //Now we are again adding untagged content with some artifacts and embedded font's so we should also be fine
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
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
                catch (System.IO.IOException e) {
                    throw new PdfException(e);
                }
            }
            );
            if (framework.IsPdf2Based(conformance)) {
                String message = MessageFormatUtil.Format(KernelExceptionMessageConstant.PARENT_CHILD_ROLE_RELATION_IS_NOT_ALLOWED
                    , "Div", "CONTENT");
                framework.AssertBothFail("addToCanvasCorrectFontArtifactUnTaggedContent", message);
            }
            else {
                framework.AssertBothValid("addToCanvasCorrectFontArtifactUnTaggedContent");
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void ManuallyAddToCanvasAndCorrectFontAndArtifactTaggedContent(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddAfterGenerationHook((pdfDoc) => {
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
                    String tag = StandardRoles.ARTIFACT;
                    if (conformance.GetUAConformance() == PdfUAConformance.PDF_UA_1) {
                        tag = StandardRoles.DIV;
                    }
                    TagTreePointer tagPointer = pdfDoc.GetTagStructureContext().GetAutoTaggingPointer().AddTag(tag);
                    tagPointer.SetPageForTagging(pdfDoc.GetPage(1));
                    canvas.OpenTag(tagPointer.GetTagReference());
                    canvas.AddXObject(xObject);
                    canvas.CloseTag();
                }
                catch (Exception e) {
                    throw new PdfException(e);
                }
            }
            );
            if (framework.IsPdf2Based(conformance)) {
                framework.AssertBothValid("manuallyCanvasCorrectFontAndArtifact");
            }
            else {
                framework.AssertOnlyVeraPdfFail("manuallyCanvasCorrectFontAndArtifact");
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void ManuallyAddToCanvasAndCorrectFontAndArtifactTaggedContentInsideArtifact(PdfConformance
             conformance) {
            // We are adding tagged content to an artifact. Looks like Verapdf doesn't check xobject stream at all because
            // page content is marked as artifact. We think it's wrong though.
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
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
                catch (System.IO.IOException e) {
                    throw new PdfException(e);
                }
            }
            );
            framework.AssertBothValid("manuallyAddToCanvasAndCorrectFontInsideArtifact");
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void ManuallyAddToCanvasAndCorrectFontAndArtifactTaggedContentInsideUntaggedPageContent(PdfConformance
             conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
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
                catch (System.IO.IOException e) {
                    throw new PdfException(e);
                }
                canvas.AddXObject(xObject);
            }
            );
            framework.AssertBothFail("untaggedAddXobject", PdfUAExceptionMessageConstants.TAG_HASNT_BEEN_ADDED_BEFORE_CONTENT_ADDING
                , false);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void ManuallyAddToCanvasAtLocationAndCorrectFontAndArtifactTaggedContentInsideUntaggedPageContent
            (PdfConformance conformance) {
            // We are adding untagged content, so we should throw an exception.
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
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
                catch (System.IO.IOException e) {
                    throw new PdfException(e);
                }
                canvas.AddXObjectAt(xObject, 200f, 200f);
            }
            );
            framework.AssertBothFail("untaggedAddXobjectAt", PdfUAExceptionMessageConstants.TAG_HASNT_BEEN_ADDED_BEFORE_CONTENT_ADDING
                , false);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void ManuallyAddToCanvasAtLocationAndCorrectFontAndArtifactTaggedContentInsideUntaggedPageContenta
            (PdfConformance conformance) {
            // We are adding untagged content, so we should throw an exception.
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
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
                catch (System.IO.IOException e) {
                    throw new PdfException(e);
                }
                canvas.AddXObjectFittedIntoRectangle(xObject, new Rectangle(200, 200, 200, 200));
            }
            );
            framework.AssertBothFail("addXObjectFitted", PdfUAExceptionMessageConstants.TAG_HASNT_BEEN_ADDED_BEFORE_CONTENT_ADDING
                , false);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void ManuallyAddToCanvasAtLocationAndCorrectFontAndArtifactTaggedContentInsideUntaggedPageContentab
            (PdfConformance conformance) {
            // We are adding untagged content, so we should throw an exception.
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
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
                catch (System.IO.IOException e) {
                    throw new PdfException(e);
                }
                canvas.AddXObjectWithTransformationMatrix(xObject, 1, 1, 1, 1, 1, 1);
            }
            );
            framework.AssertBothFail("addXObjectWithTransfoMatrix", PdfUAExceptionMessageConstants.TAG_HASNT_BEEN_ADDED_BEFORE_CONTENT_ADDING
                , false);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void AddImageObjectNotInline(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            // We are adding untagged content, so we should throw an exception.
            framework.AddBeforeGenerationHook((pdfDocument) => {
                PdfCanvas canvas = new PdfCanvas(pdfDocument.AddNewPage());
                ImageData imd = null;
                try {
                    imd = ImageDataFactory.Create(DOG);
                }
                catch (System.IO.IOException e) {
                    throw new PdfException(e);
                }
                canvas.AddImageAt(imd, 200, 200, false);
            }
            );
            framework.AssertBothFail("addIMageObjectNotInline", PdfUAExceptionMessageConstants.TAG_HASNT_BEEN_ADDED_BEFORE_CONTENT_ADDING
                , false);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void AddImageObjectInline(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            // We are adding untagged content, so we should throw an exception.
            framework.AddBeforeGenerationHook((pdfDocument) => {
                PdfCanvas canvas = new PdfCanvas(pdfDocument.AddNewPage());
                ImageData imd = null;
                try {
                    imd = ImageDataFactory.Create(DOG);
                }
                catch (System.IO.IOException e) {
                    throw new PdfException(e);
                }
                canvas.AddImageAt(imd, 200, 200, false);
            }
            );
            framework.AssertBothFail("addIMageObjectInline", PdfUAExceptionMessageConstants.TAG_HASNT_BEEN_ADDED_BEFORE_CONTENT_ADDING
                , false);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void AddImageTranformationMatrix(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            // We are adding untagged content, so we should throw an exception.
            framework.AddBeforeGenerationHook((pdfDocument) => {
                PdfCanvas canvas = new PdfCanvas(pdfDocument.AddNewPage());
                ImageData imd = null;
                try {
                    imd = ImageDataFactory.Create(DOG);
                }
                catch (System.IO.IOException e) {
                    throw new PdfException(e);
                }
                canvas.AddImageWithTransformationMatrix(imd, 1, 1, 1, 1, 1, 1, false);
            }
            );
            framework.AssertBothFail("addIMageObjectTransfo", PdfUAExceptionMessageConstants.TAG_HASNT_BEEN_ADDED_BEFORE_CONTENT_ADDING
                , false);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void AddImageFittedIntoRectangle(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            // We are adding untagged content, so we should throw an exception.
            framework.AddBeforeGenerationHook((pdfDocument) => {
                PdfCanvas canvas = new PdfCanvas(pdfDocument.AddNewPage());
                ImageData imd = null;
                try {
                    imd = ImageDataFactory.Create(DOG);
                }
                catch (System.IO.IOException e) {
                    throw new PdfException(e);
                }
                canvas.AddImageFittedIntoRectangle(imd, new Rectangle(200, 200, 200, 200), false);
            }
            );
            framework.AssertBothFail("addImageFittedIntoRectangle", PdfUAExceptionMessageConstants.TAG_HASNT_BEEN_ADDED_BEFORE_CONTENT_ADDING
                , false);
        }
    }
}
