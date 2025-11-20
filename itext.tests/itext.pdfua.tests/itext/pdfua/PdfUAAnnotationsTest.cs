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
using iText.Commons.Utils;
using iText.Forms;
using iText.Forms.Fields;
using iText.IO.Font;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Action;
using iText.Kernel.Pdf.Annot;
using iText.Kernel.Pdf.Annot.DA;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Filespec;
using iText.Kernel.Pdf.Tagging;
using iText.Kernel.Pdf.Tagutils;
using iText.Kernel.Pdf.Xobject;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Pdfua.Exceptions;
using iText.Test;

namespace iText.Pdfua {
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfUAAnnotationsTest : ExtendedITextTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfua/PdfUAAnnotationsTest/";

        private static readonly String DESTINATION_FOLDER = TestUtil.GetOutputPath() + "/pdfua/PdfUAAnnotationsTest/";

        private static readonly String FONT = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfua/font/FreeSans.ttf";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        public static IList<PdfUAConformance> Data() {
            return UaValidationTestFramework.GetConformanceList();
        }

        private static PdfFont LoadFont() {
            try {
                return PdfFontFactory.CreateFont(FONT, PdfEncodings.WINANSI, PdfFontFactory.EmbeddingStrategy.FORCE_EMBEDDED
                    );
            }
            catch (System.IO.IOException e) {
                throw new Exception(e.Message);
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void LinkAnnotNotDirectChildOfAnnotLayoutTest(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(new _Generator_124());
            framework.AssertBothValid("linkAnnotNotDirectChildOfAnnotLayoutTest", pdfUAConformance);
        }

        private sealed class _Generator_124 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_124() {
            }

            public IBlockElement Generate() {
                Rectangle rect = new Rectangle(100, 650, 400, 100);
                PdfLinkAnnotation annot = new PdfLinkAnnotation(rect).SetAction(PdfAction.CreateURI("https://itextpdf.com/"
                    ));
                annot.SetContents("link annot");
                Link link = new Link("Link to iText", annot);
                Paragraph paragraph = new Paragraph();
                paragraph.SetFont(PdfUAAnnotationsTest.LoadFont());
                paragraph.Add(link);
                return paragraph;
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void LinkAnnotNotDirectChildOfAnnotKernelTest(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                Rectangle rect = new Rectangle(100, 650, 400, 100);
                PdfLinkAnnotation annot = new PdfLinkAnnotation(rect).SetAction(PdfAction.CreateURI("https://itextpdf.com/"
                    ));
                annot.SetContents("link annot");
                pdfDoc.AddNewPage();
                pdfDoc.GetPage(1).AddAnnotation(annot);
            }
            );
            framework.AssertBothValid("linkAnnotNotDirectChildOfAnnotKernelTest", pdfUAConformance);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void WidgetAnnotNoDirectChildOfAnnotTest(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfAcroForm acroForm = PdfFormCreator.GetAcroForm(pdfDoc, true);
                PdfButtonFormField checkBox = new CheckBoxFormFieldBuilder(pdfDoc, "checkbox").SetWidgetRectangle(new Rectangle
                    (10, 650, 40, 20)).CreateCheckBox();
                checkBox.SetAlternativeName("widget");
                checkBox.GetFirstFormAnnotation().SetAlternativeDescription("widget");
                acroForm.AddField(checkBox);
            }
            );
            framework.AssertBothValid("widgetAnnotNoDirectChildOfAnnotTest", pdfUAConformance);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void WidgetAnnotNoDirectChildOfAnnotAutomaticConformanceLevelTest(PdfUAConformance pdfUAConformance
            ) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfAcroForm acroForm = PdfFormCreator.GetAcroForm(pdfDoc, true);
                PdfButtonFormField checkBox = new CheckBoxFormFieldBuilder(pdfDoc, "checkbox").SetWidgetRectangle(new Rectangle
                    (10, 650, 40, 20)).CreateCheckBox();
                checkBox.SetAlternativeName("widget");
                checkBox.GetFirstFormAnnotation().SetAlternativeDescription("widget");
                acroForm.AddField(checkBox);
            }
            );
            framework.AssertBothValid("widgetAnnotNoDirectChildAutoConformanceLvl", pdfUAConformance);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void PrinterMAnnotNoDirectChildOfAnnotTest(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfPage pdfPage = pdfDoc.AddNewPage();
                PdfFormXObject form = new PdfFormXObject(PageSize.A4);
                PdfCanvas canvas = new PdfCanvas(form, pdfDoc);
                canvas.SaveState().Circle(265, 795, 5).SetColor(ColorConstants.GREEN, true).Fill().RestoreState();
                canvas.Release();
                PdfPrinterMarkAnnotation annot = new PdfPrinterMarkAnnotation(PageSize.A4, form);
                // mark annotation as hidden, because in the scope of the test we check only that PrinterMark isn't
                // enclosed by Annot tag
                annot.SetFlag(PdfAnnotation.HIDDEN);
                pdfPage.AddAnnotation(annot);
            }
            );
            framework.AssertBothValid("printerMAnnotNoDirectChildOfAnnotTest", pdfUAConformance);
            String layoutPdf = "itext_printerMAnnotNoDirectChildOfAnnotTest" + "_UA_" + pdfUAConformance.GetPart() + ".pdf";
            using (PdfDocument pdfDoc = new PdfDocument(new PdfReader(DESTINATION_FOLDER + layoutPdf))) {
                IStructureNode docNode = pdfDoc.GetStructTreeRoot().GetKids()[0];
                NUnit.Framework.Assert.AreEqual(PdfName.Document, docNode.GetRole());
                if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                    NUnit.Framework.Assert.AreEqual(PdfName.PrinterMark, ((PdfObjRef)docNode.GetKids()[0]).GetReferencedObject
                        ().Get(PdfName.Subtype));
                }
                else {
                    if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                        IStructureNode artifactNode = docNode.GetKids()[0];
                        NUnit.Framework.Assert.AreEqual(PdfName.Artifact, artifactNode.GetRole());
                        NUnit.Framework.Assert.AreEqual(PdfName.PrinterMark, ((PdfObjRef)artifactNode.GetKids()[0]).GetReferencedObject
                            ().Get(PdfName.Subtype));
                    }
                }
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void FileAnnotDirectChildOfAnnotTest(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfPage pdfPage = pdfDoc.AddNewPage();
                Rectangle rect = new Rectangle(100, 650, 400, 100);
                PdfFileSpec fs = PdfFileSpec.CreateEmbeddedFileSpec(pdfDoc, "file".GetBytes(System.Text.Encoding.UTF8), "description"
                    , "file.txt", null, null, null);
                PdfFileAttachmentAnnotation annot = new PdfFileAttachmentAnnotation(rect, fs);
                annot.SetContents("Hello world");
                annot.GetPdfObject().Put(PdfName.Type, PdfName.Annot);
                pdfPage.AddAnnotation(annot);
            }
            );
            framework.AssertBothValid("fileAnnotDirectChildOfAnnotTest", pdfUAConformance);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void StampAnnotDirectChildOfAnnotTest(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfPage pdfPage = pdfDoc.AddNewPage();
                PdfStampAnnotation stamp = new PdfStampAnnotation(new Rectangle(0, 0, 100, 50));
                stamp.SetStampName(PdfName.Approved);
                stamp.SetContents("stamp contents");
                stamp.GetPdfObject().Put(PdfName.Type, PdfName.Annot);
                pdfPage.AddAnnotation(stamp);
            }
            );
            framework.AssertBothValid("stampAnnotDirectChildOfAnnotTest", pdfUAConformance);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void ScreenAnnotDirectChildOfAnnotTest(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfPage pdfPage = pdfDoc.AddNewPage();
                PdfScreenAnnotation screen = new PdfScreenAnnotation(new Rectangle(100, 100));
                screen.SetContents("screen annotation");
                pdfPage.AddAnnotation(screen);
            }
            );
            framework.AssertBothValid("screenAnnotDirectChildOfAnnotTest", pdfUAConformance);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void ScreenAnnotWithoutContentsAndAltTest(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfPage pdfPage = pdfDoc.AddNewPage();
                PdfScreenAnnotation screen = new PdfScreenAnnotation(new Rectangle(100, 100));
                pdfPage.AddAnnotation(screen);
            }
            );
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothFail("screenAnnotWithoutContentsAndAltTest", MessageFormatUtil.Format(PdfUAExceptionMessageConstants
                    .ANNOTATION_OF_TYPE_0_SHOULD_HAVE_CONTENTS_OR_ALT_KEY, "Screen"), pdfUAConformance);
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertBothFail("screenAnnotWithoutContentsAndAltTest", PdfUAExceptionMessageConstants.ANNOT_CONTENTS_IS_NULL_OR_EMPTY
                        , pdfUAConformance);
                }
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void PopupWithoutContentOrAltTest(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfPage pdfPage = pdfDoc.AddNewPage();
                PdfPopupAnnotation popup = new PdfPopupAnnotation(new Rectangle(0f, 0f));
                pdfPage.AddAnnotation(popup);
            }
            );
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothValid("popupWithoutContentOrAltTest", pdfUAConformance);
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertBothFail("popupWithoutContentOrAltTest", PdfUAExceptionMessageConstants.POPUP_ANNOTATIONS_ARE_NOT_ALLOWED
                        , pdfUAConformance);
                }
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void StampAnnotWithAltTest(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfPage pdfPage = pdfDoc.AddNewPage();
                PdfStampAnnotation stamp = new PdfStampAnnotation(new Rectangle(0, 0, 100, 50));
                stamp.SetStampName(PdfName.Approved);
                stamp.GetPdfObject().Put(PdfName.Type, PdfName.Annot);
                pdfPage.AddAnnotation(stamp);
                TagTreePointer tagPointer = pdfDoc.GetTagStructureContext().GetAutoTaggingPointer();
                tagPointer.MoveToKid(0);
                tagPointer.GetProperties().SetAlternateDescription("Alt description");
            }
            );
            framework.AssertBothValid("stampAnnotWithAltTest", pdfUAConformance);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void ScreenAnnotWithAltTest(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfPage pdfPage = pdfDoc.AddNewPage();
                PdfScreenAnnotation screen = new PdfScreenAnnotation(new Rectangle(100, 100));
                pdfPage.AddAnnotation(screen);
                TagTreePointer tagPointer = pdfDoc.GetTagStructureContext().GetAutoTaggingPointer();
                tagPointer.MoveToKid(0);
                tagPointer.GetProperties().SetAlternateDescription("Alt description");
            }
            );
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothValid("screenAnnotWithAltTest", pdfUAConformance);
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertBothFail("screenAnnotWithAltTest", PdfUAExceptionMessageConstants.ANNOT_CONTENTS_IS_NULL_OR_EMPTY
                        , pdfUAConformance);
                }
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void InkAnnotDirectChildOfAnnotTest(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfPage pdfPage = pdfDoc.AddNewPage();
                PdfInkAnnotation ink = CreateInkAnnotation();
                pdfPage.AddAnnotation(ink);
            }
            );
            framework.AssertBothValid("inkAnnotDirectChildOfAnnotTest", pdfUAConformance);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void RedactAnnotDirectChildOfAnnotTest(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfPage pdfPage = pdfDoc.AddNewPage();
                PdfRedactAnnotation redact = CreateRedactionAnnotation();
                pdfPage.AddAnnotation(redact);
            }
            );
            framework.AssertBothValid("redactAnnotDirectChildOfAnnotTest", pdfUAConformance);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void Ua3DAnnotDirectChildOfAnnotTest(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfPage pdfPage = pdfDoc.AddNewPage();
                Pdf3DAnnotation annot = Create3DAnnotation();
                TagTreePointer tagPointer = pdfDoc.GetTagStructureContext().GetAutoTaggingPointer();
                // Check that second Annot tag won't be added in PdfPage#addAnnotation
                tagPointer.AddTag(StandardRoles.ANNOT);
                pdfPage.AddAnnotation(annot);
            }
            );
            framework.AssertBothValid("ua3DAnnotDirectChildOfAnnotTest", pdfUAConformance);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void RichAnnotDirectChildOfAnnotTest(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfPage pdfPage = pdfDoc.AddNewPage();
                PdfTextAnnotation annot = CreateRichTextAnnotation();
                pdfDoc.GetTagStructureContext().GetAutoTaggingPointer().AddTag(StandardRoles.SECT);
                pdfPage.AddAnnotation(annot);
            }
            );
            framework.AssertBothValid("richAnnotDirectChildOfAnnotTest", pdfUAConformance);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TrapNetAnnotNotPermittedTest(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfPage pdfPage = pdfDoc.AddNewPage();
                PdfFormXObject form = new PdfFormXObject(PageSize.A4);
                PdfCanvas canvas = new PdfCanvas(form, pdfDoc);
                canvas.SaveState().Circle(272, 795, 5).SetColor(ColorConstants.GREEN, true).Fill().RestoreState();
                canvas.Release();
                form.SetProcessColorModel(PdfName.DeviceN);
                PdfTrapNetworkAnnotation annot = new PdfTrapNetworkAnnotation(PageSize.A4, form);
                annot.SetContents("Some content");
                pdfPage.AddAnnotation(annot);
            }
            );
            if (PdfUAConformance.PDF_UA_1 == pdfUAConformance) {
                framework.AssertBothFail("trapNetAnnotNotPermittedTest", PdfUAExceptionMessageConstants.ANNOT_TRAP_NET_IS_NOT_PERMITTED
                    , pdfUAConformance);
            }
            else {
                if (PdfUAConformance.PDF_UA_2 == pdfUAConformance) {
                    framework.AssertBothFail("trapNetAnnotNotPermittedTest", MessageFormatUtil.Format(PdfUAExceptionMessageConstants
                        .DEPRECATED_ANNOTATIONS_ARE_NOT_ALLOWED, PdfName.TrapNet.GetValue()), pdfUAConformance);
                }
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void InvisibleTrapNetAnnotTest(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfPage pdfPage = pdfDoc.AddNewPage();
                PdfFormXObject form = new PdfFormXObject(PageSize.A4);
                PdfCanvas canvas = new PdfCanvas(form, pdfDoc);
                canvas.SaveState().Circle(272, 795, 5).SetColor(ColorConstants.GREEN, true).Fill().RestoreState();
                canvas.Release();
                form.SetProcessColorModel(PdfName.DeviceN);
                PdfTrapNetworkAnnotation annot = new PdfTrapNetworkAnnotation(PageSize.A4, form);
                annot.SetContents("Some content");
                annot.SetFlag(PdfAnnotation.HIDDEN);
                pdfPage.AddAnnotation(annot);
            }
            );
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothValid("invisibleTrapNetAnnotTest", pdfUAConformance);
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertBothFail("invisibleTrapNetAnnotTest", MessageFormatUtil.Format(PdfUAExceptionMessageConstants
                        .DEPRECATED_ANNOTATIONS_ARE_NOT_ALLOWED, PdfName.TrapNet.GetValue()), pdfUAConformance);
                }
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void SoundAnnotDirectChildOfAnnotTest(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfPage pdfPage = pdfDoc.AddNewPage();
                PdfAnnotation annot = new PdfSoundAnnotation(new Rectangle(100, 100, 100, 100), new PdfStream());
                annot.SetContents("some content");
                pdfDoc.GetTagStructureContext().GetAutoTaggingPointer().AddTag(StandardRoles.PART);
                pdfPage.AddAnnotation(annot);
            }
            );
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothValid("soundAnnotDirectChildOfAnnotTest", pdfUAConformance);
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertBothFail("soundAnnotDirectChildOfAnnotTest", MessageFormatUtil.Format(PdfUAExceptionMessageConstants
                        .DEPRECATED_ANNOTATIONS_ARE_NOT_ALLOWED, PdfName.Sound.GetValue()), pdfUAConformance);
                }
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void PushBtnNestedWithinFormTest(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfAcroForm acroForm = PdfFormCreator.GetAcroForm(pdfDoc, true);
                // The rest of the tests for widgets can be found in com.itextpdf.pdfua.checkers.PdfUAFormFieldsTest
                PdfFormField button = new PushButtonFormFieldBuilder(pdfDoc, "push button").SetWidgetRectangle(new Rectangle
                    (10, 650, 40, 20)).SetFont(LoadFont()).CreatePushButton();
                button.SetAlternativeName("widget");
                button.GetFirstFormAnnotation().SetAlternativeDescription("widget");
                acroForm.AddField(button);
            }
            );
            framework.AssertBothValid("pushBtnNestedWithinFormTest", pdfUAConformance);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void LinkAnnotNotDirectChildOfLinkTest2(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                Rectangle rect = new Rectangle(100, 650, 400, 100);
                PdfLinkAnnotation annot = new PdfLinkAnnotation(rect).SetAction(PdfAction.CreateURI("https://itextpdf.com/"
                    ));
                annot.SetContents("link annot");
                Document doc = new Document(pdfDoc);
                Paragraph p1 = new Paragraph("Text1");
                p1.SetFont(LoadFont());
                p1.GetAccessibilityProperties().SetRole(StandardRoles.LINK);
                Paragraph p2 = new Paragraph("Text");
                p2.SetFont(LoadFont());
                p2.GetAccessibilityProperties().SetRole(StandardRoles.H1);
                p2.SetProperty(Property.LINK_ANNOTATION, annot);
                p1.Add(p2);
                doc.Add(p1);
            }
            );
            framework.AssertBothValid("linkAnnotNotDirectChildOfLinkTest2", pdfUAConformance);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void LinkAnnotNestedWithinLinkTest(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                Rectangle rect = new Rectangle(100, 650, 400, 100);
                PdfLinkAnnotation annot = new PdfLinkAnnotation(rect).SetAction(PdfAction.CreateURI("https://itextpdf.com/"
                    ));
                annot.SetContents("link annot");
                Document doc = new Document(pdfDoc);
                Paragraph p2 = new Paragraph("Text");
                p2.SetFont(LoadFont());
                p2.GetAccessibilityProperties().SetRole(StandardRoles.LINK);
                p2.SetProperty(Property.LINK_ANNOTATION, annot);
                doc.Add(p2);
            }
            );
            framework.AssertBothValid("linkAnnotNestedWithinLinkTest", pdfUAConformance);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void LinkAnnotWithoutContentsTest(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                Rectangle rect = new Rectangle(100, 650, 400, 100);
                PdfLinkAnnotation annot = new PdfLinkAnnotation(rect).SetAction(PdfAction.CreateURI("https://itextpdf.com/"
                    ));
                Document doc = new Document(pdfDoc);
                Paragraph p2 = new Paragraph("Text");
                p2.SetFont(LoadFont());
                p2.GetAccessibilityProperties().SetRole(StandardRoles.LINK).SetAlternateDescription("Alt description");
                p2.SetProperty(Property.LINK_ANNOTATION, annot);
                doc.Add(p2);
            }
            );
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothFail("linkAnnotNestedWithinLinkWithAnAltDescr", PdfUAExceptionMessageConstants.LINK_ANNOTATION_SHOULD_HAVE_CONTENTS_KEY
                    , pdfUAConformance);
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertBothValid("linkAnnotNestedWithinLinkWithAnAltDescr", pdfUAConformance);
                }
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void LinkAnnotNotDirectChildOfLinkButHiddenTest(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfPage page = pdfDoc.AddNewPage();
                Rectangle rect = new Rectangle(100, 650, 400, 100);
                PdfLinkAnnotation annot = new PdfLinkAnnotation(rect).SetAction(PdfAction.CreateURI("https://itextpdf.com/"
                    ));
                annot.SetContents("link annot");
                annot.SetFlag(PdfAnnotation.HIDDEN);
                page.AddAnnotation(annot);
            }
            );
            framework.AssertBothValid("linkAnnotNotDirectChildOfLinkButHiddenTest", pdfUAConformance);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void LinkAnnotNotDirectChildOfLinkButOutsideTest1(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfPage page = pdfDoc.AddNewPage();
                Rectangle rect = new Rectangle(10000, 65000, 400, 100);
                PdfLinkAnnotation annot = new PdfLinkAnnotation(rect).SetAction(PdfAction.CreateURI("https://itextpdf.com/"
                    ));
                annot.SetContents("link annot");
                page.AddAnnotation(annot);
            }
            );
            framework.AssertBothValid("linkAnnotNotDirectChildOfLinkButOutsideTest1", pdfUAConformance);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void LinkAnnotNotDirectChildOfLinkButOutsideTest2(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfPage page = pdfDoc.AddNewPage();
                page.SetCropBox(new Rectangle(1000, 1000, 500, 500));
                Rectangle rect = new Rectangle(100, 650, 400, 100);
                PdfLinkAnnotation annot = new PdfLinkAnnotation(rect).SetAction(PdfAction.CreateURI("https://itextpdf.com/"
                    ));
                annot.SetContents("link annot");
                page.AddAnnotation(annot);
            }
            );
            framework.AssertBothValid("linkAnnotNotDirectChildOfLinkButOutsideTest2", pdfUAConformance);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void ScreenAnnotationWithMediaDataTest(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfPage page = pdfDoc.AddNewPage();
                PdfFileSpec spec = PdfFileSpec.CreateExternalFileSpec(pdfDoc, SOURCE_FOLDER + "sample.wav");
                PdfScreenAnnotation screen = new PdfScreenAnnotation(new Rectangle(100, 100));
                PdfAction action = PdfAction.CreateRendition("sample.wav", spec, "audio/x-wav", screen);
                screen.SetAction(action);
                screen.SetContents("screen annotation");
                action.GetPdfObject().GetAsDictionary(PdfName.R).GetAsDictionary(PdfName.C).Put(PdfName.Alt, new PdfArray(
                    ));
                page.AddAnnotation(screen);
            }
            );
            framework.AssertBothValid("screenAnnotationWithValidMediaDataTest", pdfUAConformance);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void ScreenAnnotationAsAAWithMediaDataTest(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfPage page = pdfDoc.AddNewPage();
                PdfFileSpec spec = PdfFileSpec.CreateExternalFileSpec(pdfDoc, SOURCE_FOLDER + "sample.wav");
                PdfScreenAnnotation screen = new PdfScreenAnnotation(new Rectangle(100, 100));
                PdfAction action = PdfAction.CreateRendition("sample.wav", spec, "audio/x-wav", screen);
                screen.SetAdditionalAction(PdfName.E, action);
                screen.SetContents("screen annotation");
                action.GetPdfObject().GetAsDictionary(PdfName.R).GetAsDictionary(PdfName.C).Put(PdfName.Alt, new PdfArray(
                    ));
                page.AddAnnotation(screen);
            }
            );
            framework.AssertBothValid("screenAnnotationAAWithValidMediaDataTest", pdfUAConformance);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void ScreenAnnotationWithBEMediaDataTest(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfPage page = pdfDoc.AddNewPage();
                String file = "sample.wav";
                String mimeType = "audio/x-wav";
                PdfFileSpec spec = PdfFileSpec.CreateExternalFileSpec(pdfDoc, SOURCE_FOLDER + file);
                PdfScreenAnnotation screen = new PdfScreenAnnotation(new Rectangle(100, 100));
                PdfDictionary be = new PdfDictionary();
                PdfDictionary mediaClipData = new PdfMediaClipData(file, spec, mimeType).GetPdfObject();
                mediaClipData.Put(PdfName.Alt, new PdfArray());
                be.Put(PdfName.C, mediaClipData);
                PdfDictionary rendition = new PdfDictionary();
                rendition.Put(PdfName.S, PdfName.MR);
                rendition.Put(PdfName.N, new PdfString(MessageFormatUtil.Format("Rendition for {0}", file)));
                rendition.Put(PdfName.BE, be);
                PdfAction action = new PdfAction().Put(PdfName.S, PdfName.Rendition).Put(PdfName.OP, new PdfNumber(0)).Put
                    (PdfName.AN, screen.GetPdfObject()).Put(PdfName.R, new PdfRendition(rendition).GetPdfObject());
                screen.SetAction(action);
                screen.SetContents("screen annotation");
                page.AddAnnotation(screen);
            }
            );
            framework.AssertBothValid("screenAnnotationWithBEMediaDataTest", pdfUAConformance);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void ScreenAnnotationWithMHMediaDataTest(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfPage page = pdfDoc.AddNewPage();
                String file = "sample.wav";
                String mimeType = "audio/x-wav";
                PdfFileSpec spec = PdfFileSpec.CreateExternalFileSpec(pdfDoc, SOURCE_FOLDER + file);
                PdfScreenAnnotation screen = new PdfScreenAnnotation(new Rectangle(100, 100));
                PdfDictionary mh = new PdfDictionary();
                PdfDictionary mediaClipData = new PdfMediaClipData(file, spec, mimeType).GetPdfObject();
                mediaClipData.Put(PdfName.Alt, new PdfArray());
                mh.Put(PdfName.C, mediaClipData);
                PdfDictionary rendition = new PdfDictionary();
                rendition.Put(PdfName.S, PdfName.MR);
                rendition.Put(PdfName.N, new PdfString(MessageFormatUtil.Format("Rendition for {0}", file)));
                rendition.Put(PdfName.MH, mh);
                PdfAction action = new PdfAction().Put(PdfName.S, PdfName.Rendition).Put(PdfName.OP, new PdfNumber(0)).Put
                    (PdfName.AN, screen.GetPdfObject()).Put(PdfName.R, new PdfRendition(rendition).GetPdfObject());
                screen.SetAction(action);
                screen.SetContents("screen annotation");
                page.AddAnnotation(screen);
            }
            );
            framework.AssertBothValid("screenAnnotationWithMHMediaDataTest", pdfUAConformance);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void ScreenAnnotationWithMHWithoutAltMediaDataTest(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfPage page = pdfDoc.AddNewPage();
                String file = "sample.wav";
                String mimeType = "audio/x-wav";
                PdfFileSpec spec = PdfFileSpec.CreateExternalFileSpec(pdfDoc, SOURCE_FOLDER + file);
                PdfScreenAnnotation screen = new PdfScreenAnnotation(new Rectangle(100, 100));
                PdfDictionary mh = new PdfDictionary();
                PdfDictionary mediaClipData = new PdfMediaClipData(file, spec, mimeType).GetPdfObject();
                mh.Put(PdfName.C, mediaClipData);
                PdfDictionary rendition = new PdfDictionary();
                rendition.Put(PdfName.S, PdfName.MR);
                rendition.Put(PdfName.N, new PdfString(MessageFormatUtil.Format("Rendition for {0}", file)));
                rendition.Put(PdfName.MH, mh);
                PdfAction action = new PdfAction().Put(PdfName.S, PdfName.Rendition).Put(PdfName.OP, new PdfNumber(0)).Put
                    (PdfName.AN, screen.GetPdfObject()).Put(PdfName.R, new PdfRendition(rendition).GetPdfObject());
                screen.SetAction(action);
                screen.SetContents("screen annotation");
                page.AddAnnotation(screen);
            }
            );
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                // Verapdf doesn't fail here but it should
                framework.AssertOnlyITextFail("screenAnnotationWithInvalidMHMediaDataTest", PdfUAExceptionMessageConstants
                    .CT_OR_ALT_ENTRY_IS_MISSING_IN_MEDIA_CLIP, pdfUAConformance);
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertBothValid("screenAnnotationWithInvalidMHMediaDataTest", pdfUAConformance);
                }
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void ScreenAnnotationWithoutAltInMediaDataTest(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfPage page = pdfDoc.AddNewPage();
                PdfFileSpec spec = PdfFileSpec.CreateExternalFileSpec(pdfDoc, SOURCE_FOLDER + "sample.wav");
                PdfScreenAnnotation screen = new PdfScreenAnnotation(new Rectangle(100, 100));
                PdfAction action = PdfAction.CreateRendition("sample.wav", spec, "audio/x-wav", screen);
                screen.SetAction(action);
                screen.SetContents("screen annotation");
                page.AddAnnotation(screen);
            }
            );
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothFail("screenAnnotationWOAltMediaDataTest", PdfUAExceptionMessageConstants.CT_OR_ALT_ENTRY_IS_MISSING_IN_MEDIA_CLIP
                    , pdfUAConformance);
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertBothValid("screenAnnotationWOAltMediaDataTest", pdfUAConformance);
                }
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void ScreenAnnotationAsAAWithoutAltInMediaDataTest(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfPage page = pdfDoc.AddNewPage();
                PdfFileSpec spec = PdfFileSpec.CreateExternalFileSpec(pdfDoc, SOURCE_FOLDER + "sample.wav");
                PdfScreenAnnotation screen = new PdfScreenAnnotation(new Rectangle(100, 100));
                PdfAction action = PdfAction.CreateRendition("sample.wav", spec, "audio/x-wav", screen);
                screen.SetAdditionalAction(PdfName.E, action);
                screen.SetContents("screen annotation");
                page.AddAnnotation(screen);
            }
            );
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothFail("screenAnnotationAaWoAltMediaDataTest", PdfUAExceptionMessageConstants.CT_OR_ALT_ENTRY_IS_MISSING_IN_MEDIA_CLIP
                    , pdfUAConformance);
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertBothValid("screenAnnotationAaWoAltMediaDataTest", pdfUAConformance);
                }
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void ScreenAnnotationWithoutCTInMediaDataTest(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfPage page = pdfDoc.AddNewPage();
                PdfFileSpec spec = PdfFileSpec.CreateExternalFileSpec(pdfDoc, SOURCE_FOLDER + "sample.wav");
                PdfScreenAnnotation screen = new PdfScreenAnnotation(new Rectangle(100, 100));
                PdfAction action = PdfAction.CreateRendition("sample.wav", spec, "audio/x-wav", screen);
                screen.SetAction(action);
                screen.SetContents("screen annotation");
                action.GetPdfObject().GetAsDictionary(PdfName.R).GetAsDictionary(PdfName.C).Put(PdfName.Alt, new PdfArray(
                    ));
                action.GetPdfObject().GetAsDictionary(PdfName.R).GetAsDictionary(PdfName.C).Remove(PdfName.CT);
                page.AddAnnotation(screen);
            }
            );
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothFail("screenAnnotationWoCtMediaDataTest", PdfUAExceptionMessageConstants.CT_OR_ALT_ENTRY_IS_MISSING_IN_MEDIA_CLIP
                    , pdfUAConformance);
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertBothValid("screenAnnotationWoCtMediaDataTest", pdfUAConformance);
                }
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void UndefinedAnnotTest(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfPage page = pdfDoc.AddNewPage();
                PdfUAAnnotationsTest.PdfCustomAnnot annot = new PdfUAAnnotationsTest.PdfCustomAnnot(new Rectangle(100, 650
                    , 400, 100));
                annot.SetContents("Content of unique annot");
                page.AddAnnotation(annot);
            }
            );
            framework.AssertBothValid("undefinedAnnotTest", pdfUAConformance);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TabsEntryAbsentInPageTest(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfPage pdfPage = pdfDoc.AddNewPage();
                PdfTextAnnotation annot = CreateRichTextAnnotation();
                pdfPage.AddAnnotation(annot);
                pdfPage.GetPdfObject().Remove(PdfName.Tabs);
            }
            );
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothFail("tabsEntryAbsentInPageTest", PdfUAExceptionMessageConstants.PAGE_WITH_ANNOT_DOES_NOT_HAVE_TABS_WITH_S
                    , pdfUAConformance);
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertBothFail("tabsEntryAbsentInPageTest", PdfUAExceptionMessageConstants.PAGE_WITH_ANNOT_DOES_NOT_HAVE_TABS_WITH_VALID_CONTENT
                        , pdfUAConformance);
                }
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void TabsEntryNotSInPageTest(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfPage pdfPage = pdfDoc.AddNewPage();
                PdfTextAnnotation annot = CreateRichTextAnnotation();
                pdfPage.AddAnnotation(annot);
                pdfPage.SetTabOrder(PdfName.O);
            }
            );
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothFail("tabsEntryNotSInPageTest", PdfUAExceptionMessageConstants.PAGE_WITH_ANNOT_DOES_NOT_HAVE_TABS_WITH_S
                    , pdfUAConformance);
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertBothFail("tabsEntryNotSInPageTest", PdfUAExceptionMessageConstants.PAGE_WITH_ANNOT_DOES_NOT_HAVE_TABS_WITH_VALID_CONTENT
                        , pdfUAConformance);
                }
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void InvalidTabsEntryButAnnotInvisibleTest(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfPage pdfPage = pdfDoc.AddNewPage();
                PdfTextAnnotation annot = CreateRichTextAnnotation();
                annot.SetFlag(PdfAnnotation.HIDDEN);
                pdfPage.AddAnnotation(annot);
                pdfPage.SetTabOrder(PdfName.O);
            }
            );
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothFail("invalidTabsEntryButAnnotInvisibleTest", PdfUAExceptionMessageConstants.PAGE_WITH_ANNOT_DOES_NOT_HAVE_TABS_WITH_S
                    , pdfUAConformance);
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertBothFail("invalidTabsEntryButAnnotInvisibleTest", PdfUAExceptionMessageConstants.PAGE_WITH_ANNOT_DOES_NOT_HAVE_TABS_WITH_VALID_CONTENT
                        , pdfUAConformance);
                }
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void PrinterMAnnotIsInLogicalStructureTest(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfPage pdfPage = pdfDoc.AddNewPage();
                PdfFormXObject form = new PdfFormXObject(PageSize.A4);
                PdfCanvas canvas = new PdfCanvas(form, pdfDoc);
                canvas.SaveState().Circle(265, 795, 5).SetColor(ColorConstants.GREEN, true).Fill().RestoreState();
                canvas.Release();
                PdfPrinterMarkAnnotation annot = new PdfPrinterMarkAnnotation(PageSize.A4, form);
                annot.SetContents("link annot");
                TagTreePointer tagPointer = pdfDoc.GetTagStructureContext().GetAutoTaggingPointer();
                tagPointer.AddTag(StandardRoles.ANNOT);
                tagPointer.SetPageForTagging(pdfPage).AddAnnotationTag(annot);
                tagPointer.MoveToParent();
                pdfPage.GetPdfObject().Put(PdfName.Annots, new PdfArray(annot.SetPage(pdfPage).GetPdfObject()));
            }
            );
            if (pdfUAConformance == PdfUAConformance.PDF_UA_1) {
                framework.AssertBothFail("printerMAnnotIsInLogicalStructureTest", PdfUAExceptionMessageConstants.PRINTER_MARK_IS_NOT_PERMITTED
                    , pdfUAConformance);
            }
            else {
                if (pdfUAConformance == PdfUAConformance.PDF_UA_2) {
                    framework.AssertBothFail("printerMAnnotIsInLogicalStructureTest", PdfUAExceptionMessageConstants.PRINTER_MARK_SHALL_BE_AN_ARTIFACT
                        , pdfUAConformance);
                }
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void PrinterMAnnotNotInTagStructureTest(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfPage pdfPage = pdfDoc.AddNewPage();
                PdfFormXObject form = new PdfFormXObject(PageSize.A4);
                PdfCanvas canvas = new PdfCanvas(form, pdfDoc);
                canvas.SaveState().Circle(265, 795, 5).SetColor(ColorConstants.GREEN, true).Fill().RestoreState();
                canvas.Release();
                PdfPrinterMarkAnnotation annot = new PdfPrinterMarkAnnotation(PageSize.A4, form);
                annot.SetContents("link annot");
                // Put false as 3rd parameter to not tag annotation
                pdfPage.AddAnnotation(-1, annot, false);
                PdfUAAnnotationsTest.PdfCustomAnnot annot2 = new PdfUAAnnotationsTest.PdfCustomAnnot(new Rectangle(100, 650
                    , 400, 100));
                annot2.SetContents("Content of unique annot");
                pdfPage.AddAnnotation(annot2);
                PdfStampAnnotation stamp = new PdfStampAnnotation(new Rectangle(0, 0, 100, 50));
                stamp.SetStampName(PdfName.Approved);
                stamp.SetContents("stamp contents");
                stamp.GetPdfObject().Put(PdfName.Type, PdfName.Annot);
                pdfPage.AddAnnotation(stamp);
            }
            );
            framework.AssertBothValid("printerMAnnotNotInTagStructureTest", pdfUAConformance);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void InvisibleAnnotationArtifactTest(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfPage pdfPage = pdfDoc.AddNewPage();
                PdfWatermarkAnnotation annotation = new PdfWatermarkAnnotation(new Rectangle(100, 100));
                annotation.SetContents("Contents");
                annotation.SetFlag(PdfAnnotation.INVISIBLE);
                pdfPage.AddAnnotation(annotation);
            }
            );
            framework.AssertBothValid("invisibleAnnotationArtifact", pdfUAConformance);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void InvisibleAnnotationTest(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfPage pdfPage = pdfDoc.AddNewPage();
                PdfStampAnnotation stamp = new PdfStampAnnotation(new Rectangle(100, 100));
                stamp.SetContents("Contents");
                stamp.SetStampName(PdfName.Approved);
                stamp.SetFlag(PdfAnnotation.INVISIBLE);
                pdfPage.GetPdfObject().Put(PdfName.Annots, new PdfArray(stamp.GetPdfObject()));
                TagTreePointer tagPointer = pdfDoc.GetTagStructureContext().GetAutoTaggingPointer();
                tagPointer.AddTag(StandardRoles.ANNOT);
                tagPointer.SetPageForTagging(pdfPage).AddAnnotationTag(stamp);
            }
            );
            if (PdfUAConformance.PDF_UA_1 == pdfUAConformance) {
                framework.AssertBothValid("invisibleAnnotation", pdfUAConformance);
            }
            else {
                if (PdfUAConformance.PDF_UA_2 == pdfUAConformance) {
                    framework.AssertBothFail("invisibleAnnotation", PdfUAExceptionMessageConstants.INVISIBLE_ANNOT_SHALL_BE_AN_ARTIFACT
                        , false, pdfUAConformance);
                }
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void NoViewAnnotationArtifactTest(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfPage pdfPage = pdfDoc.AddNewPage();
                PdfWatermarkAnnotation annotation = new PdfWatermarkAnnotation(new Rectangle(100, 100));
                annotation.SetContents("Contents");
                annotation.SetFlag(PdfAnnotation.NO_VIEW);
                pdfPage.AddAnnotation(annotation);
            }
            );
            framework.AssertBothValid("noViewAnnotationArtifact", pdfUAConformance);
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void NoViewAnnotationTest(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfPage pdfPage = pdfDoc.AddNewPage();
                PdfStampAnnotation stamp = new PdfStampAnnotation(new Rectangle(100, 100));
                stamp.SetContents("Contents");
                stamp.SetStampName(PdfName.Approved);
                stamp.SetFlag(PdfAnnotation.NO_VIEW);
                pdfPage.GetPdfObject().Put(PdfName.Annots, new PdfArray(stamp.GetPdfObject()));
                TagTreePointer tagPointer = pdfDoc.GetTagStructureContext().GetAutoTaggingPointer();
                tagPointer.AddTag(StandardRoles.ANNOT);
                tagPointer.SetPageForTagging(pdfPage).AddAnnotationTag(stamp);
            }
            );
            if (PdfUAConformance.PDF_UA_1 == pdfUAConformance) {
                framework.AssertBothValid("noViewAnnotation", pdfUAConformance);
            }
            else {
                if (PdfUAConformance.PDF_UA_2 == pdfUAConformance) {
                    framework.AssertBothFail("noViewAnnotation", PdfUAExceptionMessageConstants.NO_VIEW_ANNOT_SHALL_BE_AN_ARTIFACT
                        , false, pdfUAConformance);
                }
            }
        }

        [NUnit.Framework.TestCaseSource("Data")]
        public virtual void ToggleNoViewAnnotationTest(PdfUAConformance pdfUAConformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfPage pdfPage = pdfDoc.AddNewPage();
                PdfStampAnnotation stamp = new PdfStampAnnotation(new Rectangle(100, 100));
                stamp.SetContents("Contents");
                stamp.SetStampName(PdfName.Approved);
                stamp.SetFlag(PdfAnnotation.NO_VIEW);
                stamp.SetFlag(PdfAnnotation.TOGGLE_NO_VIEW);
                pdfPage.AddAnnotation(stamp);
            }
            );
            if (PdfUAConformance.PDF_UA_1 == pdfUAConformance) {
                framework.AssertBothValid("noViewAnnotation", pdfUAConformance);
            }
            else {
                if (PdfUAConformance.PDF_UA_2 == pdfUAConformance) {
                    framework.AssertBothValid("toggleNoViewAnnotation", pdfUAConformance);
                }
            }
        }

        private PdfTextAnnotation CreateRichTextAnnotation() {
            PdfTextAnnotation annot = new PdfTextAnnotation(new Rectangle(100, 100, 100, 100));
            annot.SetContents("Rich media annot");
            PdfDictionary annotPdfObject = annot.GetPdfObject();
            annotPdfObject.Put(PdfName.Subtype, PdfName.RichMedia);
            return annot;
        }

        private PdfInkAnnotation CreateInkAnnotation() {
            float[] array1 = new float[] { 100, 100, 100, 200, 200, 200, 300, 300 };
            PdfArray firstPoint = new PdfArray(array1);
            PdfArray resultArray = new PdfArray();
            resultArray.Add(firstPoint);
            PdfDictionary borderStyle = new PdfDictionary();
            borderStyle.Put(PdfName.Type, PdfName.Border);
            borderStyle.Put(PdfName.W, new PdfNumber(3));
            PdfInkAnnotation ink = new PdfInkAnnotation(new Rectangle(0, 0, 575, 842), resultArray);
            ink.SetBorderStyle(borderStyle);
            float[] rgb = new float[] { 1, 0, 0 };
            PdfArray colors = new PdfArray(rgb);
            ink.SetColor(colors);
            ink.SetContents("ink annotation");
            return ink;
        }

        private Pdf3DAnnotation Create3DAnnotation() {
            PdfStream stream3D = new PdfStream();
            stream3D.Put(PdfName.Type, PdfName._3D);
            stream3D.Put(PdfName.Subtype, new PdfName("U3D"));
            stream3D.SetCompressionLevel(CompressionConstants.UNDEFINED_COMPRESSION);
            Pdf3DAnnotation annot = new Pdf3DAnnotation(new Rectangle(300, 300, 100, 50), stream3D);
            PdfDictionary dict3D = new PdfDictionary();
            dict3D.Put(PdfName.Type, PdfName._3DView);
            dict3D.Put(new PdfName("XN"), new PdfString("Default"));
            dict3D.Put(new PdfName("IN"), new PdfString("Unnamed"));
            dict3D.Put(new PdfName("MS"), PdfName.M);
            dict3D.Put(new PdfName("C2W"), new PdfArray(new float[] { 1, 0, 0, 0, 0, -1, 0, 1, 0, 3, -235, 28 }));
            dict3D.Put(PdfName.CO, new PdfNumber(235));
            annot.SetDefaultInitialView(dict3D);
            annot.SetFlag(PdfAnnotation.PRINT);
            annot.SetAppearance(PdfName.N, new PdfStream());
            annot.SetContents("3D annot");
            return annot;
        }

        private PdfRedactAnnotation CreateRedactionAnnotation() {
            PdfRedactAnnotation redact = new PdfRedactAnnotation(new Rectangle(0, 0, 100, 50)).SetDefaultAppearance(new 
                AnnotationDefaultAppearance().SetColor(DeviceCmyk.MAGENTA).SetFont(StandardAnnotationFont.CourierOblique
                ).SetFontSize(20)).SetOverlayText(new PdfString("Redact CMYK courier-oblique"));
            redact.SetContents("redact annotation");
            return redact;
        }

        private class PdfCustomAnnot : PdfAnnotation {
            protected internal PdfCustomAnnot(Rectangle rect)
                : base(rect) {
            }

            public override PdfName GetSubtype() {
                return new PdfName("CustomUniqueAnnot");
            }
        }
    }
}
