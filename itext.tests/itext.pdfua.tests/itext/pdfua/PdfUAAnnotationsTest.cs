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
using iText.Kernel.Utils;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Pdfua.Exceptions;
using iText.Test;
using iText.Test.Pdfa;

namespace iText.Pdfua {
    // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf/ua validation on Android)
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfUAAnnotationsTest : ExtendedITextTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfua/PdfUAAnnotationsTest/";

        private static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/pdfua/PdfUAAnnotationsTest/";

        private static readonly String FONT = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfua/font/FreeSans.ttf";

        private UaValidationTestFramework framework;

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.SetUp]
        public virtual void InitializeFramework() {
            framework = new UaValidationTestFramework(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void Ua1LinkAnnotNoDirectChildOfAnnotTest() {
            framework.AddSuppliers(new _Generator_122());
            framework.AssertBothValid("ua1LinkAnnotNoDirectChildOfAnnotTest");
        }

        private sealed class _Generator_122 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_122() {
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

        [NUnit.Framework.Test]
        public virtual void Ua1WidgetAnnotNoDirectChildOfAnnotTest() {
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfAcroForm acroForm = PdfFormCreator.GetAcroForm(pdfDoc, true);
                PdfButtonFormField checkBox = new CheckBoxFormFieldBuilder(pdfDoc, "checkbox").SetWidgetRectangle(new Rectangle
                    (10, 650, 40, 20)).SetGenericConformanceLevel(PdfUAConformanceLevel.PDFUA_1).CreateCheckBox();
                checkBox.SetAlternativeName("widget");
                acroForm.AddField(checkBox);
            }
            );
            framework.AssertBothValid("ua1WidgetAnnotNoDirectChildOfAnnotTest");
        }

        [NUnit.Framework.Test]
        public virtual void Ua1WidgetAnnotNoDirectChildOfAnnotAutomaticConformanceLevelTest() {
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfAcroForm acroForm = PdfFormCreator.GetAcroForm(pdfDoc, true);
                PdfButtonFormField checkBox = new CheckBoxFormFieldBuilder(pdfDoc, "checkbox").SetWidgetRectangle(new Rectangle
                    (10, 650, 40, 20)).CreateCheckBox();
                checkBox.SetAlternativeName("widget");
                acroForm.AddField(checkBox);
            }
            );
            framework.AssertBothValid("ua1WidgetAnnotNoDirectChildOfAnnotAutomaticConformanceLevelTest");
        }

        [NUnit.Framework.Test]
        public virtual void Ua1PrinterMAnnotNoDirectChildOfAnnotTest() {
            String outPdf = DESTINATION_FOLDER + "ua1PrinterMAnnotNoDirectChildOfAnnotTest.pdf";
            using (PdfUATestPdfDocument pdfDoc = new PdfUATestPdfDocument(new PdfWriter(outPdf))) {
                PdfPage pdfPage = pdfDoc.AddNewPage();
                PdfFormXObject form = new PdfFormXObject(PageSize.A4);
                PdfCanvas canvas = new PdfCanvas(form, pdfDoc);
                canvas.SaveState().Circle(265, 795, 5).SetColor(ColorConstants.GREEN, true).Fill().RestoreState();
                canvas.Release();
                PdfPrinterMarkAnnotation annot = new PdfPrinterMarkAnnotation(PageSize.A4, form);
                // mark annotation as hidden, because in the scope of the test we check only that PrinterMark isn't enclosed by Annot tag
                annot.SetFlag(PdfAnnotation.HIDDEN);
                pdfPage.AddAnnotation(annot);
            }
            NUnit.Framework.Assert.IsNull(new VeraPdfValidator().Validate(outPdf));
            // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf\a validation on Android)
            using (PdfDocument pdfDoc_1 = new PdfDocument(new PdfReader(outPdf))) {
                IStructureNode docNode = pdfDoc_1.GetStructTreeRoot().GetKids()[0];
                NUnit.Framework.Assert.AreEqual(PdfName.Document, docNode.GetRole());
                NUnit.Framework.Assert.AreEqual(PdfName.PrinterMark, ((PdfObjRef)docNode.GetKids()[0]).GetReferencedObject
                    ().Get(PdfName.Subtype));
            }
        }

        [NUnit.Framework.Test]
        public virtual void Ua1FileAnnotDirectChildOfAnnotTest() {
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
            framework.AssertBothValid("ua1FileAnnotDirectChildOfAnnotTest");
        }

        [NUnit.Framework.Test]
        public virtual void Ua1StampAnnotDirectChildOfAnnotTest() {
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfPage pdfPage = pdfDoc.AddNewPage();
                PdfStampAnnotation stamp = new PdfStampAnnotation(new Rectangle(0, 0, 100, 50));
                stamp.SetStampName(PdfName.Approved);
                stamp.SetContents("stamp contents");
                stamp.GetPdfObject().Put(PdfName.Type, PdfName.Annot);
                pdfPage.AddAnnotation(stamp);
            }
            );
            framework.AssertBothValid("ua1StampAnnotDirectChildOfAnnotTest");
        }

        [NUnit.Framework.Test]
        public virtual void Ua1ScreenAnnotDirectChildOfAnnotTest() {
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfPage pdfPage = pdfDoc.AddNewPage();
                PdfScreenAnnotation screen = new PdfScreenAnnotation(new Rectangle(100, 100));
                screen.SetContents("screen annotation");
                pdfPage.AddAnnotation(screen);
            }
            );
            framework.AssertBothValid("ua1ScreenAnnotDirectChildOfAnnotTest");
        }

        [NUnit.Framework.Test]
        public virtual void Ua1ScreenAnnotWithoutContentsAndAltTest() {
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfPage pdfPage = pdfDoc.AddNewPage();
                PdfScreenAnnotation screen = new PdfScreenAnnotation(new Rectangle(100, 100));
                pdfPage.AddAnnotation(screen);
            }
            );
            framework.AssertBothFail("ua1ScreenWithoutContentsTest", MessageFormatUtil.Format(PdfUAExceptionMessageConstants
                .ANNOTATION_OF_TYPE_0_SHOULD_HAVE_CONTENTS_OR_ALT_KEY, "Screen"));
        }

        [NUnit.Framework.Test]
        public virtual void Ua1PopupWithoutContentOrAltTest() {
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfPage pdfPage = pdfDoc.AddNewPage();
                PdfPopupAnnotation popup = new PdfPopupAnnotation(new Rectangle(0f, 0f));
                pdfPage.AddAnnotation(popup);
            }
            );
            framework.AssertBothValid("ua1PopupWithoutContentOrAltTest");
        }

        [NUnit.Framework.Test]
        public virtual void Ua1StampAnnotWithAltTest() {
            String outPdf = DESTINATION_FOLDER + "ua1StampAnnotWithAltTest.pdf";
            PdfUATestPdfDocument pdfDoc = new PdfUATestPdfDocument(new PdfWriter(outPdf));
            PdfPage pdfPage = pdfDoc.AddNewPage();
            PdfStampAnnotation stamp = new PdfStampAnnotation(new Rectangle(0, 0, 100, 50));
            stamp.SetStampName(PdfName.Approved);
            stamp.GetPdfObject().Put(PdfName.Type, PdfName.Annot);
            pdfPage.AddAnnotation(stamp);
            stamp.GetPdfObject().Put(PdfName.Alt, new PdfString("Alt description"));
            pdfPage.AddAnnotation(stamp);
            NUnit.Framework.Assert.DoesNotThrow(() => {
                pdfDoc.Close();
            }
            );
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, SOURCE_FOLDER + "cmp_ua1StampAnnotWithAltTest.pdf"
                , DESTINATION_FOLDER, "diff_"));
            NUnit.Framework.Assert.IsNotNull(new VeraPdfValidator().Validate(outPdf));
        }

        // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf/ua validation on Android)
        [NUnit.Framework.Test]
        public virtual void Ua1ScreenAnnotWithAltTest() {
            String outPdf = DESTINATION_FOLDER + "ua1ScreenAnnotWithAltTest.pdf";
            PdfUATestPdfDocument pdfDoc = new PdfUATestPdfDocument(new PdfWriter(outPdf));
            PdfPage pdfPage = pdfDoc.AddNewPage();
            PdfScreenAnnotation screen = new PdfScreenAnnotation(new Rectangle(100, 100));
            pdfPage.AddAnnotation(screen);
            screen.GetPdfObject().Put(PdfName.Alt, new PdfString("Alt description"));
            NUnit.Framework.Assert.DoesNotThrow(() => {
                pdfDoc.Close();
            }
            );
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, SOURCE_FOLDER + "cmp_ua1ScreenAnnotWithAltTest.pdf"
                , DESTINATION_FOLDER, "diff_"));
            NUnit.Framework.Assert.IsNotNull(new VeraPdfValidator().Validate(outPdf));
        }

        // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf/ua validation on Android)
        [NUnit.Framework.Test]
        public virtual void Ua1InkAnnotDirectChildOfAnnotTest() {
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfPage pdfPage = pdfDoc.AddNewPage();
                PdfInkAnnotation ink = CreateInkAnnotation();
                pdfPage.AddAnnotation(ink);
            }
            );
            framework.AssertBothValid("ua1InkAnnotDirectChildOfAnnotTest");
        }

        [NUnit.Framework.Test]
        public virtual void Ua1RedactAnnotDirectChildOfAnnotTest() {
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfPage pdfPage = pdfDoc.AddNewPage();
                PdfRedactAnnotation redact = CreateRedactionAnnotation();
                pdfPage.AddAnnotation(redact);
            }
            );
            framework.AssertBothValid("ua1RedactAnnotDirectChildOfAnnotTest");
        }

        [NUnit.Framework.Test]
        public virtual void Ua13DAnnotDirectChildOfAnnotTest() {
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfPage pdfPage = pdfDoc.AddNewPage();
                Pdf3DAnnotation annot = Create3DAnnotation();
                TagTreePointer tagPointer = pdfDoc.GetTagStructureContext().GetAutoTaggingPointer();
                // Check that second Annot tag won't be added in PdfPage#addAnnotation
                tagPointer.AddTag(StandardRoles.ANNOT);
                pdfPage.AddAnnotation(annot);
            }
            );
            framework.AssertBothValid("ua13DAnnotDirectChildOfAnnotTest");
        }

        [NUnit.Framework.Test]
        public virtual void Ua1RichAnnotDirectChildOfAnnotTest() {
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfPage pdfPage = pdfDoc.AddNewPage();
                PdfTextAnnotation annot = CreateRichTextAnnotation();
                pdfDoc.GetTagStructureContext().GetAutoTaggingPointer().AddTag(StandardRoles.SECT);
                pdfPage.AddAnnotation(annot);
            }
            );
            framework.AssertBothValid("ua1RichAnnotDirectChildOfAnnotTest");
        }

        [NUnit.Framework.Test]
        public virtual void TrapNetAnnotNotPermittedTest() {
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
            framework.AssertBothFail("trapNetAnnotNotPermittedTest", PdfUAExceptionMessageConstants.ANNOT_TRAP_NET_IS_NOT_PERMITTED
                );
        }

        [NUnit.Framework.Test]
        public virtual void InvisibleTrapNetAnnotTest() {
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
            framework.AssertBothValid("invisibleTrapNetAnnotTest");
        }

        [NUnit.Framework.Test]
        public virtual void Ua1SoundAnnotDirectChildOfAnnotTest() {
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfPage pdfPage = pdfDoc.AddNewPage();
                PdfAnnotation annot = new PdfSoundAnnotation(new Rectangle(100, 100, 100, 100), new PdfStream());
                annot.SetContents("some content");
                pdfDoc.GetTagStructureContext().GetAutoTaggingPointer().AddTag(StandardRoles.PART);
                pdfPage.AddAnnotation(annot);
            }
            );
            framework.AssertBothValid("ua1SoundAnnotDirectChildOfAnnotTest");
        }

        [NUnit.Framework.Test]
        public virtual void Ua1PushBtnNestedWithinFormTest() {
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfAcroForm acroForm = PdfFormCreator.GetAcroForm(pdfDoc, true);
                // The rest of the tests for widgets can be found in com.itextpdf.pdfua.checkers.PdfUAFormFieldsTest
                PdfFormField button = new PushButtonFormFieldBuilder(pdfDoc, "push button").SetWidgetRectangle(new Rectangle
                    (10, 650, 40, 20)).SetGenericConformanceLevel(PdfUAConformanceLevel.PDFUA_1).SetFont(LoadFont()).CreatePushButton
                    ();
                button.SetAlternativeName("widget");
                acroForm.AddField(button);
            }
            );
            framework.AssertBothValid("ua1PushBtnNestedWithinFormTest");
        }

        [NUnit.Framework.Test]
        public virtual void LinkAnnotNotDirectChildOfLinkTest1() {
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfPage page = pdfDoc.AddNewPage();
                Rectangle rect = new Rectangle(100, 650, 400, 100);
                PdfLinkAnnotation annot = new PdfLinkAnnotation(rect).SetAction(PdfAction.CreateURI("https://itextpdf.com/"
                    ));
                annot.SetContents("link annot");
                page.AddAnnotation(annot);
            }
            );
            framework.AssertBothFail("linkAnnotNotDirectChildOfLinkTest1", PdfUAExceptionMessageConstants.LINK_ANNOT_IS_NOT_NESTED_WITHIN_LINK
                );
        }

        [NUnit.Framework.Test]
        public virtual void LinkAnnotNotDirectChildOfLinkTest2() {
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
            framework.AssertBothFail("linkAnnotNotDirectChildOfLinkTest2", PdfUAExceptionMessageConstants.LINK_ANNOT_IS_NOT_NESTED_WITHIN_LINK
                );
        }

        [NUnit.Framework.Test]
        public virtual void LinkAnnotNestedWithinLinkTest() {
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
            framework.AssertBothValid("linkAnnotNestedWithinLinkTest");
        }

        [NUnit.Framework.Test]
        public virtual void LinkAnnotWithoutContentsTest() {
            framework.AddBeforeGenerationHook((pdfDoc) => {
                Rectangle rect = new Rectangle(100, 650, 400, 100);
                PdfLinkAnnotation annot = new PdfLinkAnnotation(rect).SetAction(PdfAction.CreateURI("https://itextpdf.com/"
                    ));
                Document doc = new Document(pdfDoc);
                Paragraph p2 = new Paragraph("Text");
                p2.SetFont(LoadFont());
                p2.GetAccessibilityProperties().SetRole(StandardRoles.LINK);
                p2.SetProperty(Property.LINK_ANNOTATION, annot);
                doc.Add(p2);
                doc.GetPdfDocument().GetPage(1).GetPdfObject().GetAsArray(PdfName.Annots).GetAsDictionary(0).Put(PdfName.Alt
                    , new PdfString("Alt description"));
            }
            );
            framework.AssertBothFail("linkAnnotNestedWithinLinkWithAnAlternateDescriptionTest");
        }

        [NUnit.Framework.Test]
        public virtual void LinkAnnotNotDirectChildOfLinkButHiddenTest() {
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
            framework.AssertBothValid("linkAnnotNotDirectChildOfLinkButHiddenTest");
        }

        [NUnit.Framework.Test]
        public virtual void LinkAnnotNotDirectChildOfLinkButOutsideTest1() {
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfPage page = pdfDoc.AddNewPage();
                Rectangle rect = new Rectangle(10000, 65000, 400, 100);
                PdfLinkAnnotation annot = new PdfLinkAnnotation(rect).SetAction(PdfAction.CreateURI("https://itextpdf.com/"
                    ));
                annot.SetContents("link annot");
                page.AddAnnotation(annot);
            }
            );
            framework.AssertBothValid("linkAnnotNotDirectChildOfLinkButOutsideTest1");
        }

        [NUnit.Framework.Test]
        public virtual void LinkAnnotNotDirectChildOfLinkButOutsideTest2() {
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
            framework.AssertBothValid("linkAnnotNotDirectChildOfLinkButOutsideTest2");
        }

        [NUnit.Framework.Test]
        public virtual void ScreenAnnotationWithMediaDataTest() {
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfPage page = pdfDoc.AddNewPage();
                PdfFileSpec spec = PdfFileSpec.CreateExternalFileSpec(pdfDoc, "sample.wav");
                PdfScreenAnnotation screen = new PdfScreenAnnotation(new Rectangle(100, 100));
                PdfAction action = PdfAction.CreateRendition("sample.wav", spec, "audio/x-wav", screen);
                screen.SetAction(action);
                screen.SetContents("screen annotation");
                action.GetPdfObject().GetAsDictionary(PdfName.R).GetAsDictionary(PdfName.C).Put(PdfName.Alt, new PdfArray(
                    ));
                page.AddAnnotation(screen);
            }
            );
            framework.AssertBothValid("screenAnnotationWithValidMediaDataTest");
        }

        [NUnit.Framework.Test]
        public virtual void ScreenAnnotationAsAAWithMediaDataTest() {
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfPage page = pdfDoc.AddNewPage();
                PdfFileSpec spec = PdfFileSpec.CreateExternalFileSpec(pdfDoc, "sample.wav");
                PdfScreenAnnotation screen = new PdfScreenAnnotation(new Rectangle(100, 100));
                PdfAction action = PdfAction.CreateRendition("sample.wav", spec, "audio/x-wav", screen);
                screen.SetAdditionalAction(PdfName.E, action);
                screen.SetContents("screen annotation");
                action.GetPdfObject().GetAsDictionary(PdfName.R).GetAsDictionary(PdfName.C).Put(PdfName.Alt, new PdfArray(
                    ));
                page.AddAnnotation(screen);
            }
            );
            framework.AssertBothValid("screenAnnotationWithValidMediaDataTest");
        }

        [NUnit.Framework.Test]
        public virtual void ScreenAnnotationWithBEMediaDataTest() {
            String outPdf = DESTINATION_FOLDER + "screenAnnotationWithBEMediaDataTest.pdf";
            PdfDocument pdfDoc = new PdfUATestPdfDocument(new PdfWriter(outPdf));
            PdfPage page = pdfDoc.AddNewPage();
            String file = "sample.wav";
            String mimeType = "audio/x-wav";
            PdfFileSpec spec = PdfFileSpec.CreateExternalFileSpec(pdfDoc, file);
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
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, SOURCE_FOLDER + "cmp_screenAnnotationWithBEMediaDataTest.pdf"
                , DESTINATION_FOLDER, "diff_"));
        }

        //Verapdf throws runtime exception, so we don't do this check here.
        [NUnit.Framework.Test]
        public virtual void ScreenAnnotationWithMHMediaDataTest() {
            String outPdf = DESTINATION_FOLDER + "screenAnnotationWithMHMediaDataTest.pdf";
            PdfDocument pdfDoc = new PdfUATestPdfDocument(new PdfWriter(outPdf));
            PdfPage page = pdfDoc.AddNewPage();
            String file = "sample.wav";
            String mimeType = "audio/x-wav";
            PdfFileSpec spec = PdfFileSpec.CreateExternalFileSpec(pdfDoc, file);
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
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, SOURCE_FOLDER + "cmp_screenAnnotationWithMHMediaDataTest.pdf"
                , DESTINATION_FOLDER, "diff_"));
        }

        //Verapdf throws runtime exception, so we don't do this check here.
        [NUnit.Framework.Test]
        public virtual void ScreenAnnotationWithMHWithoutAltMediaDataTest() {
            String outPdf = DESTINATION_FOLDER + "screenAnnotationWithInvalidMHMediaDataTest.pdf";
            PdfDocument pdfDoc = new PdfUATestPdfDocument(new PdfWriter(outPdf));
            PdfPage page = pdfDoc.AddNewPage();
            String file = "sample.wav";
            String mimeType = "audio/x-wav";
            PdfFileSpec spec = PdfFileSpec.CreateExternalFileSpec(pdfDoc, file);
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
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfUAConformanceException), () => {
                pdfDoc.Close();
            }
            );
            NUnit.Framework.Assert.AreEqual(PdfUAExceptionMessageConstants.CT_OR_ALT_ENTRY_IS_MISSING_IN_MEDIA_CLIP, e
                .Message);
        }

        //Verapdf throws runtime exception, so we don't do this check here.
        [NUnit.Framework.Test]
        public virtual void ScreenAnnotationWithoutAltInMediaDataTest() {
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfPage page = pdfDoc.AddNewPage();
                PdfFileSpec spec = PdfFileSpec.CreateExternalFileSpec(pdfDoc, "sample.wav");
                PdfScreenAnnotation screen = new PdfScreenAnnotation(new Rectangle(100, 100));
                PdfAction action = PdfAction.CreateRendition("sample.wav", spec, "audio/x-wav", screen);
                screen.SetAction(action);
                screen.SetContents("screen annotation");
                page.AddAnnotation(screen);
            }
            );
            framework.AssertBothFail("screenAnnotationWithMediaDataTest", PdfUAExceptionMessageConstants.CT_OR_ALT_ENTRY_IS_MISSING_IN_MEDIA_CLIP
                );
        }

        [NUnit.Framework.Test]
        public virtual void ScreenAnnotationAsAAWithoutAltInMediaDataTest() {
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfPage page = pdfDoc.AddNewPage();
                PdfFileSpec spec = PdfFileSpec.CreateExternalFileSpec(pdfDoc, "sample.wav");
                PdfScreenAnnotation screen = new PdfScreenAnnotation(new Rectangle(100, 100));
                PdfAction action = PdfAction.CreateRendition("sample.wav", spec, "audio/x-wav", screen);
                screen.SetAdditionalAction(PdfName.E, action);
                screen.SetContents("screen annotation");
                page.AddAnnotation(screen);
            }
            );
            framework.AssertBothFail("screenAnnotationWithMediaDataTest", PdfUAExceptionMessageConstants.CT_OR_ALT_ENTRY_IS_MISSING_IN_MEDIA_CLIP
                );
        }

        [NUnit.Framework.Test]
        public virtual void ScreenAnnotationWithoutCTInMediaDataTest() {
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfPage page = pdfDoc.AddNewPage();
                PdfFileSpec spec = PdfFileSpec.CreateExternalFileSpec(pdfDoc, "sample.wav");
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
            framework.AssertBothFail("screenAnnotationWithMediaDataTest", PdfUAExceptionMessageConstants.CT_OR_ALT_ENTRY_IS_MISSING_IN_MEDIA_CLIP
                );
        }

        [NUnit.Framework.Test]
        public virtual void LinkAnnotNotDirectChildOfLinkInvalidCropTest() {
            String outPdf = DESTINATION_FOLDER + "linkAnnotNotDirectChildOfLinkInvalidCropTest.pdf";
            PdfUATestPdfDocument pdfDoc = new PdfUATestPdfDocument(new PdfWriter(outPdf));
            PdfPage page = pdfDoc.AddNewPage();
            PdfArray array = new PdfArray();
            array.Add(new PdfString("hey"));
            page.Put(PdfName.CropBox, array);
            Rectangle rect = new Rectangle(10000, 6500, 400, 100);
            PdfLinkAnnotation annot = new PdfLinkAnnotation(rect).SetAction(PdfAction.CreateURI("https://itextpdf.com/"
                ));
            annot.SetContents("link annot");
            page.AddAnnotation(annot);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfUAConformanceException), () => pdfDoc.Close());
            // VeraPdf doesn't complain, but the document is invalid, so it is also accepted behaviour
            NUnit.Framework.Assert.AreEqual(PdfUAExceptionMessageConstants.LINK_ANNOT_IS_NOT_NESTED_WITHIN_LINK, e.Message
                );
        }

        [NUnit.Framework.Test]
        public virtual void UndefinedAnnotTest() {
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfPage page = pdfDoc.AddNewPage();
                PdfUAAnnotationsTest.PdfCustomAnnot annot = new PdfUAAnnotationsTest.PdfCustomAnnot(new Rectangle(100, 650
                    , 400, 100));
                annot.SetContents("Content of unique annot");
                page.AddAnnotation(annot);
            }
            );
            framework.AssertBothValid("undefinedAnnotTest");
        }

        [NUnit.Framework.Test]
        public virtual void TabsEntryAbsentInPageTest() {
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfPage pdfPage = pdfDoc.AddNewPage();
                PdfTextAnnotation annot = CreateRichTextAnnotation();
                pdfPage.AddAnnotation(annot);
                pdfPage.GetPdfObject().Remove(PdfName.Tabs);
            }
            );
            framework.AssertBothFail("tabsEntryAbsentInPageTest", PdfUAExceptionMessageConstants.PAGE_WITH_ANNOT_DOES_NOT_HAVE_TABS_WITH_S
                );
        }

        [NUnit.Framework.Test]
        public virtual void TabsEntryNotSInPageTest() {
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfPage pdfPage = pdfDoc.AddNewPage();
                PdfTextAnnotation annot = CreateRichTextAnnotation();
                pdfPage.AddAnnotation(annot);
                pdfPage.SetTabOrder(PdfName.O);
            }
            );
            framework.AssertBothFail("tabsEntryNotSInPageTest", PdfUAExceptionMessageConstants.PAGE_WITH_ANNOT_DOES_NOT_HAVE_TABS_WITH_S
                );
        }

        [NUnit.Framework.Test]
        public virtual void InvalidTabsEntryButAnnotInvisibleTest() {
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfPage pdfPage = pdfDoc.AddNewPage();
                PdfTextAnnotation annot = CreateRichTextAnnotation();
                annot.SetFlag(PdfAnnotation.HIDDEN);
                pdfPage.AddAnnotation(annot);
                pdfPage.SetTabOrder(PdfName.O);
            }
            );
            framework.AssertBothFail("invalidTabsEntryButAnnotInvisibleTest", PdfUAExceptionMessageConstants.PAGE_WITH_ANNOT_DOES_NOT_HAVE_TABS_WITH_S
                );
        }

        [NUnit.Framework.Test]
        public virtual void Ua1PrinterMAnnotIsInLogicalStructureTest() {
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfPage pdfPage = pdfDoc.AddNewPage();
                PdfFormXObject form = new PdfFormXObject(PageSize.A4);
                PdfCanvas canvas = new PdfCanvas(form, pdfDoc);
                canvas.SaveState().Circle(265, 795, 5).SetColor(ColorConstants.GREEN, true).Fill().RestoreState();
                canvas.Release();
                PdfPrinterMarkAnnotation annot = new PdfPrinterMarkAnnotation(PageSize.A4, form);
                annot.SetContents("link annot");
                pdfPage.AddAnnotation(annot);
            }
            );
            framework.AssertBothFail("ua1PrinterMAnnotIsInLogicalStructureTest", PdfUAExceptionMessageConstants.PRINTER_MARK_IS_NOT_PERMITTED
                );
        }

        [NUnit.Framework.Test]
        public virtual void Ua1PrinterMAnnotNotInTagStructureTest() {
            String outPdf = DESTINATION_FOLDER + "ua1PrinterMAnnotNotInTagStructureTest.pdf";
            PdfDocument pdfDoc = new PdfUATestPdfDocument(new PdfWriter(outPdf));
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
            NUnit.Framework.Assert.DoesNotThrow(() => pdfDoc.Close());
            // VeraPdf complains about the fact that PrinterMark annotation isn't wrapped by Annot tag.
            // But in that test we don't put PrinterMark annot in tag structure at all.
            NUnit.Framework.Assert.IsNotNull(new VeraPdfValidator().Validate(outPdf));
        }

        // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf/ua validation on Android)
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

        private static PdfFont LoadFont() {
            try {
                return PdfFontFactory.CreateFont(FONT, PdfEncodings.WINANSI, PdfFontFactory.EmbeddingStrategy.FORCE_EMBEDDED
                    );
            }
            catch (System.IO.IOException e) {
                throw new Exception(e.Message);
            }
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
