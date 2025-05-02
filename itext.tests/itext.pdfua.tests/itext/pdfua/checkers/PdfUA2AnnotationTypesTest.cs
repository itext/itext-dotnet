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
using iText.Forms.Form.Element;
using iText.IO.Font;
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Annot;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Filespec;
using iText.Kernel.Pdf.Tagging;
using iText.Kernel.Pdf.Tagutils;
using iText.Kernel.Pdf.Xobject;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Pdfua;
using iText.Pdfua.Exceptions;
using iText.Test;

namespace iText.Pdfua.Checkers {
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfUA2AnnotationTypesTest : ExtendedITextTest {
        private static readonly String DESTINATION_FOLDER = TestUtil.GetOutputPath() + "/pdfua/PdfUA2AnnotationTypesTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        public static IList<PdfName> MarkupAnnotsTypes() {
            return JavaUtil.ArraysAsList(PdfName.Text, PdfName.FreeText, PdfName.Line, PdfName.Square, PdfName.Circle, 
                PdfName.Polygon, PdfName.PolyLine, PdfName.Highlight, PdfName.Underline, PdfName.Squiggly, PdfName.StrikeOut
                , PdfName.Caret, PdfName.Stamp, PdfName.Ink, PdfName.FileAttachment, PdfName.Redaction, PdfName.Projection
                );
        }

        public static IList<PdfName> AnnotTypesToCheckContents() {
            return JavaUtil.ArraysAsList(PdfName.Ink, PdfName.Screen, PdfName._3D, PdfName.RichMedia);
        }

        public static IList<PdfName> DeprecatedAnnotTypes() {
            return JavaUtil.ArraysAsList(PdfName.Sound, PdfName.Movie, PdfName.TrapNet);
        }

        [NUnit.Framework.Test]
        public virtual void AnnotationContentsAndStructureElementAltTest() {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfPage pdfPage = pdfDoc.AddNewPage();
                PdfScreenAnnotation screen = new PdfScreenAnnotation(new Rectangle(100, 100));
                screen.SetContents("Contents description");
                pdfPage.AddAnnotation(screen);
                TagTreePointer tagPointer = pdfDoc.GetTagStructureContext().GetAutoTaggingPointer();
                tagPointer.MoveToKid(0);
                tagPointer.GetProperties().SetAlternateDescription("Alt description");
            }
            );
            framework.AssertBothFail("annotationContentsAndStructureElementAlt", PdfUAExceptionMessageConstants.CONTENTS_AND_ALT_SHALL_BE_IDENTICAL
                , false, PdfUAConformance.PDF_UA_2);
        }

        [NUnit.Framework.Test]
        public virtual void PageWithTaggedAnnotTabOrderTest() {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfPage pdfPage = pdfDoc.AddNewPage();
                pdfPage.SetTabOrder(PdfName.C);
                PdfScreenAnnotation screen = new PdfScreenAnnotation(new Rectangle(100, 100));
                screen.SetContents("Contents description");
                pdfPage.AddAnnotation(screen);
            }
            );
            framework.AssertBothFail("pageWithTaggedAnnotTabOrder", PdfUAExceptionMessageConstants.PAGE_WITH_ANNOT_DOES_NOT_HAVE_TABS_WITH_VALID_CONTENT
                , false, PdfUAConformance.PDF_UA_2);
        }

        [NUnit.Framework.Test]
        public virtual void PageWithNotTaggedAnnotTabOrderTest() {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfPage pdfPage = pdfDoc.AddNewPage();
                pdfPage.SetTabOrder(PdfName.R);
                PdfScreenAnnotation screen = new PdfScreenAnnotation(new Rectangle(100, 100));
                screen.SetContents("Contents description");
                pdfPage.GetPdfObject().Put(PdfName.Annots, new PdfArray(screen.GetPdfObject()));
            }
            );
            framework.AssertBothFail("pageWithNotTaggedAnnotTabOrder", PdfUAExceptionMessageConstants.PAGE_WITH_ANNOT_DOES_NOT_HAVE_TABS_WITH_VALID_CONTENT
                , false, PdfUAConformance.PDF_UA_2);
        }

        [NUnit.Framework.TestCaseSource("MarkupAnnotsTypes")]
        public virtual void MarkupAnnotationIsNotTaggedTest(PdfName annotType) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfPage pdfPage = pdfDoc.AddNewPage();
                PdfDictionary annotation = new PdfDictionary();
                annotation.Put(PdfName.Type, PdfName.Annot);
                annotation.Put(PdfName.Subtype, annotType);
                pdfPage.GetPdfObject().Put(PdfName.Annots, new PdfArray(annotation));
            }
            );
            if (PdfName.Redaction.Equals(annotType) || PdfName.Projection.Equals(annotType)) {
                framework.AssertOnlyITextFail("markupAnnotationIsNotTagged_" + annotType.GetValue(), PdfUAExceptionMessageConstants
                    .MARKUP_ANNOT_IS_NOT_TAGGED_AS_ANNOT, PdfUAConformance.PDF_UA_2);
            }
            else {
                framework.AssertBothFail("markupAnnotationIsNotTagged_" + annotType.GetValue(), PdfUAExceptionMessageConstants
                    .MARKUP_ANNOT_IS_NOT_TAGGED_AS_ANNOT, false, PdfUAConformance.PDF_UA_2);
            }
        }

        [NUnit.Framework.TestCaseSource("MarkupAnnotsTypes")]
        public virtual void MarkupAnnotationIsNotTaggedAsAnnotTest(PdfName annotType) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfPage pdfPage = pdfDoc.AddNewPage();
                PdfDictionary annot = new PdfDictionary();
                annot.Put(PdfName.Type, PdfName.Annot);
                annot.Put(PdfName.Subtype, annotType);
                annot.Put(PdfName.Rect, new PdfArray(new Rectangle(100, 100, 100, 100)));
                PdfAnnotation annotation = PdfAnnotation.MakeAnnotation(annot);
                annotation.SetContents("Contents description");
                pdfPage.AddAnnotation(annotation);
                PdfObjRef objRef = pdfDoc.GetStructTreeRoot().FindObjRefByStructParentIndex(pdfPage.GetPdfObject(), 0);
                TagTreePointer p = pdfDoc.GetTagStructureContext().CreatePointerForStructElem((PdfStructElem)objRef.GetParent
                    ());
                p.SetRole(StandardRoles.ARTIFACT);
            }
            );
            if (PdfName.Redaction.Equals(annotType) || PdfName.Projection.Equals(annotType)) {
                framework.AssertOnlyITextFail("markupAnnotationIsNotTaggedAsAnnot_" + annotType.GetValue(), PdfUAExceptionMessageConstants
                    .MARKUP_ANNOT_IS_NOT_TAGGED_AS_ANNOT, PdfUAConformance.PDF_UA_2);
            }
            else {
                framework.AssertBothFail("markupAnnotationIsNotTaggedAsAnnot_" + annotType.GetValue(), PdfUAExceptionMessageConstants
                    .MARKUP_ANNOT_IS_NOT_TAGGED_AS_ANNOT, false, PdfUAConformance.PDF_UA_2);
            }
        }

        [NUnit.Framework.TestCaseSource("MarkupAnnotsTypes")]
        public virtual void MarkupAnnotationRCAndContentsTest(PdfName annotType) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfPage pdfPage = pdfDoc.AddNewPage();
                String richText = "<body xmlns=\"http://www.w3.org/1999/xhtml\"><p style=\"color:#FF0000;\">Some&#13;</p>"
                     + "<p style=\"color:#1E487C;\">Rich Text&#13;</p></body>";
                PdfDictionary annot = new PdfDictionary();
                annot.Put(PdfName.Type, PdfName.Annot);
                annot.Put(PdfName.Subtype, annotType);
                annot.Put(PdfName.Rect, new PdfArray(new Rectangle(100, 100, 100, 100)));
                annot.Put(PdfName.RC, new PdfString(richText, PdfEncodings.PDF_DOC_ENCODING));
                PdfAnnotation annotation = PdfAnnotation.MakeAnnotation(annot);
                annotation.SetContents("Different");
                pdfPage.AddAnnotation(annotation);
            }
            );
            if (PdfName.Redaction.Equals(annotType) || PdfName.Projection.Equals(annotType)) {
                framework.AssertOnlyITextFail("markupAnnotationRCAndContents_" + annotType.GetValue(), PdfUAExceptionMessageConstants
                    .RC_DIFFERENT_FROM_CONTENTS, PdfUAConformance.PDF_UA_2);
            }
            else {
                framework.AssertBothFail("markupAnnotationRCAndContents_" + annotType.GetValue(), PdfUAExceptionMessageConstants
                    .RC_DIFFERENT_FROM_CONTENTS, false, PdfUAConformance.PDF_UA_2);
            }
        }

        [NUnit.Framework.TestCaseSource("MarkupAnnotsTypes")]
        public virtual void MarkupAnnotationValidTest(PdfName annotType) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfPage pdfPage = pdfDoc.AddNewPage();
                String value = "Red\rBlue\r";
                String richText = "<body xmlns=\"http://www.w3.org/1999/xhtml\"><p style=\"color:#FF0000;\">Red&#13;</p>" 
                    + "<p style=\"color:#1E487C;\">Blue&#13;</p></body>";
                PdfDictionary annot = new PdfDictionary();
                annot.Put(PdfName.Type, PdfName.Annot);
                annot.Put(PdfName.Subtype, annotType);
                annot.Put(PdfName.Rect, new PdfArray(new Rectangle(100, 100, 100, 100)));
                annot.Put(PdfName.RC, new PdfString(richText, PdfEncodings.PDF_DOC_ENCODING));
                PdfAnnotation annotation = PdfAnnotation.MakeAnnotation(annot);
                annotation.SetContents(value);
                pdfPage.AddAnnotation(annotation);
            }
            );
            framework.AssertBothValid("markupAnnotation_" + annotType.GetValue(), PdfUAConformance.PDF_UA_2);
        }

        [NUnit.Framework.Test]
        public virtual void RubberStampAnnotationNoNameAndContentsTest() {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfPage pdfPage = pdfDoc.AddNewPage();
                PdfStampAnnotation stamp = new PdfStampAnnotation(new Rectangle(100, 100));
                pdfPage.AddAnnotation(stamp);
            }
            );
            framework.AssertBothFail("rubberStampAnnotationNoNameAndContents", PdfUAExceptionMessageConstants.STAMP_ANNOT_SHALL_SPECIFY_NAME_OR_CONTENTS
                , false, PdfUAConformance.PDF_UA_2);
        }

        [NUnit.Framework.TestCaseSource("AnnotTypesToCheckContents")]
        public virtual void AnnotationNoContentsTest(PdfName annotType) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfPage pdfPage = pdfDoc.AddNewPage();
                PdfDictionary annot = new PdfDictionary();
                annot.Put(PdfName.Type, PdfName.Annot);
                annot.Put(PdfName.Subtype, annotType);
                annot.Put(PdfName.Rect, new PdfArray(new Rectangle(100, 100, 100, 100)));
                PdfAnnotation annotation = PdfAnnotation.MakeAnnotation(annot);
                pdfPage.AddAnnotation(annotation);
            }
            );
            framework.AssertBothFail("annotationNoContents_" + annotType.GetValue(), PdfUAExceptionMessageConstants.ANNOT_CONTENTS_IS_NULL_OR_EMPTY
                , false, PdfUAConformance.PDF_UA_2);
        }

        [NUnit.Framework.TestCaseSource("AnnotTypesToCheckContents")]
        public virtual void AnnotationEmptyContentsTest(PdfName annotType) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfPage pdfPage = pdfDoc.AddNewPage();
                PdfDictionary annot = new PdfDictionary();
                annot.Put(PdfName.Type, PdfName.Annot);
                annot.Put(PdfName.Subtype, annotType);
                annot.Put(PdfName.Rect, new PdfArray(new Rectangle(100, 100, 100, 100)));
                PdfAnnotation annotation = PdfAnnotation.MakeAnnotation(annot);
                annotation.SetContents("");
                pdfPage.AddAnnotation(annotation);
            }
            );
            framework.AssertOnlyITextFail("annotationEmptyContents_" + annotType.GetValue(), PdfUAExceptionMessageConstants
                .ANNOT_CONTENTS_IS_NULL_OR_EMPTY, PdfUAConformance.PDF_UA_2);
        }

        [NUnit.Framework.Test]
        public virtual void PopupAnnotationTaggedAsAnnotTest() {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfPage pdfPage = pdfDoc.AddNewPage();
                PdfPopupAnnotation popupAnnotation = new PdfPopupAnnotation(new Rectangle(100, 100));
                pdfPage.AddAnnotation(popupAnnotation);
            }
            );
            framework.AssertBothFail("popupAnnotationTaggedAsAnnot", PdfUAExceptionMessageConstants.POPUP_ANNOTATIONS_ARE_NOT_ALLOWED
                , false, PdfUAConformance.PDF_UA_2);
        }

        [NUnit.Framework.Test]
        public virtual void PopupAnnotationTaggedAsArtifactTest() {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfPage pdfPage = pdfDoc.AddNewPage();
                PdfPopupAnnotation popupAnnotation = new PdfPopupAnnotation(new Rectangle(100, 100));
                pdfPage.AddAnnotation(popupAnnotation);
                PdfObjRef objRef = pdfDoc.GetStructTreeRoot().FindObjRefByStructParentIndex(pdfPage.GetPdfObject(), 0);
                TagTreePointer p = pdfDoc.GetTagStructureContext().CreatePointerForStructElem((PdfStructElem)objRef.GetParent
                    ());
                p.SetRole(StandardRoles.ARTIFACT);
            }
            );
            framework.AssertBothFail("popupAnnotationTaggedAsArtifact", PdfUAExceptionMessageConstants.POPUP_ANNOTATIONS_ARE_NOT_ALLOWED
                , false, PdfUAConformance.PDF_UA_2);
        }

        [NUnit.Framework.Test]
        public virtual void FileAttachmentAnnotationValidTest() {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfPage pdfPage = pdfDoc.AddNewPage();
                Rectangle rect = new Rectangle(100, 650, 400, 100);
                PdfFileSpec fs = PdfFileSpec.CreateEmbeddedFileSpec(pdfDoc, "file".GetBytes(), "description", "file.txt", 
                    null, null, null);
                PdfFileAttachmentAnnotation annot = new PdfFileAttachmentAnnotation(rect, fs);
                pdfPage.AddAnnotation(annot);
            }
            );
            framework.AssertBothValid("fileAttachmentAnnotationValid", PdfUAConformance.PDF_UA_2);
        }

        [NUnit.Framework.Test]
        public virtual void FileAttachmentAnnotationInvalidFileSpecTest() {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfPage pdfPage = pdfDoc.AddNewPage();
                Rectangle rect = new Rectangle(100, 650, 400, 100);
                PdfFileSpec fs = PdfFileSpec.CreateEmbeddedFileSpec(pdfDoc, "file".GetBytes(), "description", "file.txt", 
                    null, null, null);
                PdfDictionary fsDict = (PdfDictionary)fs.GetPdfObject();
                fsDict.Remove(PdfName.AFRelationship);
                PdfFileAttachmentAnnotation annot = new PdfFileAttachmentAnnotation(rect, fs);
                pdfPage.AddAnnotation(annot);
            }
            );
            framework.AssertBothFail("fileAttachmentAnnotationInvalidFileSpec", PdfUAExceptionMessageConstants.FILE_SPEC_SHALL_CONTAIN_AFRELATIONSHIP
                , false, PdfUAConformance.PDF_UA_2);
        }

        [NUnit.Framework.TestCaseSource("DeprecatedAnnotTypes")]
        public virtual void DeprecatedAnnotationTypeTest(PdfName annotType) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfPage pdfPage = pdfDoc.AddNewPage();
                PdfDictionary annot = new PdfDictionary();
                annot.Put(PdfName.Type, PdfName.Annot);
                annot.Put(PdfName.Subtype, annotType);
                annot.Put(PdfName.Rect, new PdfArray(new Rectangle(100, 100, 100, 100)));
                PdfAnnotation annotation = PdfAnnotation.MakeAnnotation(annot);
                pdfPage.AddAnnotation(annotation);
            }
            );
            framework.AssertBothFail("deprecatedAnnotationType_" + annotType.GetValue(), MessageFormatUtil.Format(PdfUAExceptionMessageConstants
                .DEPRECATED_ANNOTATIONS_ARE_NOT_ALLOWED, annotType.GetValue()), false, PdfUAConformance.PDF_UA_2);
        }

        [NUnit.Framework.Test]
        public virtual void WidgetAnnotationZeroWidthAndHeightTaggedAsFormTest() {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddSuppliers(new _Generator_363());
            framework.AssertBothFail("widgetAnnotationZeroWidthAndHeightTaggedAsForm", PdfUAExceptionMessageConstants.
                WIDGET_WITH_ZERO_HEIGHT_SHALL_BE_AN_ARTIFACT, false, PdfUAConformance.PDF_UA_2);
        }

        private sealed class _Generator_363 : UaValidationTestFramework.Generator<IBlockElement> {
            public _Generator_363() {
            }

            public IBlockElement Generate() {
                CheckBox cb = new CheckBox("name");
                cb.SetAlternativeDescription("Contents");
                cb.SetProperty(Property.WIDTH, UnitValue.CreatePointValue(0));
                cb.SetProperty(Property.HEIGHT, UnitValue.CreatePointValue(0));
                cb.SetPdfConformance(PdfConformance.PDF_UA_2);
                cb.SetInteractive(true);
                return cb;
            }
        }

        [NUnit.Framework.Test]
        public virtual void PrinterMarkAnnotationTaggedAsAnnotTest() {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfPage pdfPage = pdfDoc.AddNewPage();
                PdfFormXObject form = new PdfFormXObject(PageSize.A4);
                PdfCanvas canvas = new PdfCanvas(form, pdfDoc);
                canvas.SaveState().Circle(265, 795, 5).SetColor(ColorConstants.GREEN, true).Fill().RestoreState();
                canvas.Release();
                PdfPrinterMarkAnnotation printerMark = new PdfPrinterMarkAnnotation(PageSize.A4, form);
                pdfPage.AddAnnotation(printerMark);
                PdfObjRef objRef = pdfDoc.GetStructTreeRoot().FindObjRefByStructParentIndex(pdfPage.GetPdfObject(), 0);
                TagTreePointer p = pdfDoc.GetTagStructureContext().CreatePointerForStructElem((PdfStructElem)objRef.GetParent
                    ());
                p.SetRole(StandardRoles.ANNOT);
            }
            );
            framework.AssertBothFail("printerMarkAnnotationTaggedAsAnnot", PdfUAExceptionMessageConstants.PRINTER_MARK_SHALL_BE_AN_ARTIFACT
                , false, PdfUAConformance.PDF_UA_2);
        }

        [NUnit.Framework.Test]
        public virtual void PrinterMarkAnnotationTaggedAsArtifactTest() {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfPage pdfPage = pdfDoc.AddNewPage();
                PdfFormXObject form = new PdfFormXObject(PageSize.A4);
                PdfCanvas canvas = new PdfCanvas(form, pdfDoc);
                canvas.SaveState().Circle(265, 795, 5).SetColor(ColorConstants.GREEN, true).Fill().RestoreState();
                canvas.Release();
                PdfPrinterMarkAnnotation printerMark = new PdfPrinterMarkAnnotation(PageSize.A4, form);
                pdfPage.AddAnnotation(printerMark);
            }
            );
            framework.AssertBothValid("printerMarkAnnotationTaggedAsArtifact", PdfUAConformance.PDF_UA_2);
        }

        [NUnit.Framework.Test]
        public virtual void PrinterMarkAnnotationNotTaggedTest() {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfPage pdfPage = pdfDoc.AddNewPage();
                PdfFormXObject form = new PdfFormXObject(PageSize.A4);
                PdfCanvas canvas = new PdfCanvas(form, pdfDoc);
                canvas.SaveState().Circle(265, 795, 5).SetColor(ColorConstants.GREEN, true).Fill().RestoreState();
                canvas.Release();
                PdfPrinterMarkAnnotation printerMark = new PdfPrinterMarkAnnotation(PageSize.A4, form);
                pdfPage.GetPdfObject().Put(PdfName.Annots, new PdfArray(printerMark.GetPdfObject()));
            }
            );
            framework.AssertBothValid("printerMarkAnnotationNotTagged", PdfUAConformance.PDF_UA_2);
        }

        [NUnit.Framework.Test]
        public virtual void WatermarkAnnotationAsRealContentTest() {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfPage pdfPage = pdfDoc.AddNewPage();
                PdfWatermarkAnnotation annot = new PdfWatermarkAnnotation(new Rectangle(100, 100));
                annot.SetContents("Contents");
                annot.Put(PdfName.RC, new PdfString("<p>Rich text</p>"));
                pdfPage.AddAnnotation(annot);
            }
            );
            framework.AssertOnlyITextFail("watermarkAnnotationAsRealContent", PdfUAExceptionMessageConstants.RC_DIFFERENT_FROM_CONTENTS
                , PdfUAConformance.PDF_UA_2);
        }

        [NUnit.Framework.Test]
        public virtual void WatermarkAnnotationAsArtifactTest() {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfPage pdfPage = pdfDoc.AddNewPage();
                PdfWatermarkAnnotation annot = new PdfWatermarkAnnotation(new Rectangle(100, 100));
                annot.SetContents("Contents");
                annot.Put(PdfName.RC, new PdfString("<p>Rich text</p>"));
                pdfPage.GetPdfObject().Put(PdfName.Annots, new PdfArray(annot.GetPdfObject()));
                TagTreePointer tagPointer = pdfDoc.GetTagStructureContext().GetAutoTaggingPointer();
                tagPointer.AddTag(StandardRoles.ARTIFACT);
                tagPointer.SetPageForTagging(pdfPage).AddAnnotationTag(annot);
            }
            );
            framework.AssertBothValid("watermarkAnnotationAsArtifact", PdfUAConformance.PDF_UA_2);
        }
    }
}
