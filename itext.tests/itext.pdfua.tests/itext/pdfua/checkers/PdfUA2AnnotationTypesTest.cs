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
using iText.Commons.Internal.Runtime;
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

        public static IList<Object[]> MarkupAnnotsTypes() {
            return GenerateAllTypesOfDocuments(JavaUtil.ArraysAsList(PdfName.Text, PdfName.FreeText, PdfName.Line, PdfName
                .Square, PdfName.Circle, PdfName.Polygon, PdfName.PolyLine, PdfName.Highlight, PdfName.Underline, PdfName
                .Squiggly, PdfName.StrikeOut, PdfName.Caret, PdfName.Stamp, PdfName.Ink, PdfName.FileAttachment, PdfName
                .Redaction, PdfName.Projection));
        }

        public static IList<Object[]> AnnotTypesToCheckContents() {
            return GenerateAllTypesOfDocuments(JavaUtil.ArraysAsList(PdfName.Ink, PdfName.Screen, PdfName._3D, PdfName
                .RichMedia));
        }

        public static IList<Object[]> DeprecatedAnnotTypes() {
            return GenerateAllTypesOfDocuments(JavaUtil.ArraysAsList(PdfName.Sound, PdfName.Movie, PdfName.TrapNet));
        }

        public static IList<PdfConformance> Conformances() {
            return UaValidationTestFramework.GetConformanceList(false);
        }

        [NUnit.Framework.TestCaseSource("Conformances")]
        public virtual void AnnotationContentsAndStructureElementAltTest(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
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
                );
        }

        [NUnit.Framework.TestCaseSource("Conformances")]
        public virtual void PageWithTaggedAnnotTabOrderTest(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfPage pdfPage = pdfDoc.AddNewPage();
                pdfPage.SetTabOrder(PdfName.C);
                PdfScreenAnnotation screen = new PdfScreenAnnotation(new Rectangle(100, 100));
                screen.SetContents("Contents description");
                pdfPage.AddAnnotation(screen);
            }
            );
            framework.AssertBothFail("pageWithTaggedAnnotTabOrder", PdfUAExceptionMessageConstants.PAGE_WITH_ANNOT_DOES_NOT_HAVE_TABS_WITH_VALID_CONTENT
                );
        }

        [NUnit.Framework.TestCaseSource("Conformances")]
        public virtual void PageWithNotTaggedAnnotTabOrderTest(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfPage pdfPage = pdfDoc.AddNewPage();
                pdfPage.SetTabOrder(PdfName.R);
                PdfScreenAnnotation screen = new PdfScreenAnnotation(new Rectangle(100, 100));
                screen.SetContents("Contents description");
                pdfPage.GetPdfObject().Put(PdfName.Annots, new PdfArray(screen.GetPdfObject()));
            }
            );
            framework.AssertBothFail("pageWithNotTaggedAnnotTabOrder", PdfUAExceptionMessageConstants.PAGE_WITH_ANNOT_DOES_NOT_HAVE_TABS_WITH_VALID_CONTENT
                );
        }

        [NUnit.Framework.TestCaseSource("MarkupAnnotsTypes")]
        public virtual void MarkupAnnotationIsNotTaggedTest(PdfName annotType, PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
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
                    .MARKUP_ANNOT_IS_NOT_TAGGED_AS_ANNOT);
            }
            else {
                framework.AssertBothFail("markupAnnotationIsNotTagged_" + annotType.GetValue(), PdfUAExceptionMessageConstants
                    .MARKUP_ANNOT_IS_NOT_TAGGED_AS_ANNOT, false);
            }
        }

        [NUnit.Framework.TestCaseSource("MarkupAnnotsTypes")]
        public virtual void MarkupAnnotationIsNotTaggedAsAnnotTest(PdfName annotType, PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
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
                    .MARKUP_ANNOT_IS_NOT_TAGGED_AS_ANNOT);
            }
            else {
                framework.AssertBothFail("markupAnnotationIsNotTaggedAsAnnot_" + annotType.GetValue(), PdfUAExceptionMessageConstants
                    .MARKUP_ANNOT_IS_NOT_TAGGED_AS_ANNOT, false);
            }
        }

        [NUnit.Framework.TestCaseSource("MarkupAnnotsTypes")]
        public virtual void MarkupAnnotationRCAndContentsTest(PdfName annotType, PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
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
            if (!conformance.ConformsTo(PdfConformance.WELL_TAGGED_PDF_FOR_REUSE)) {
                if (PdfName.Redaction.Equals(annotType) || PdfName.Projection.Equals(annotType)) {
                    framework.AssertOnlyITextFail("markupAnnotationRCAndContents_" + annotType.GetValue(), PdfUAExceptionMessageConstants
                        .RC_DIFFERENT_FROM_CONTENTS);
                }
                else {
                    framework.AssertBothFail("markupAnnotationRCAndContents_" + annotType.GetValue(), PdfUAExceptionMessageConstants
                        .RC_DIFFERENT_FROM_CONTENTS, false);
                }
            }
            if (conformance.ConformsTo(PdfConformance.WELL_TAGGED_PDF_FOR_REUSE) && !conformance.ConformsTo(PdfConformance
                .WELL_TAGGED_PDF_FOR_ACCESSIBILITY)) {
                framework.AssertOnlyITextFail("markupAnnotationRCAndContents_" + annotType.GetValue(), PdfUAExceptionMessageConstants
                    .RC_DIFFERENT_FROM_CONTENTS);
            }
        }

        [NUnit.Framework.TestCaseSource("MarkupAnnotsTypes")]
        public virtual void MarkupAnnotationValidTest(PdfName annotType, PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
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
            framework.AssertBothValid("markupAnnotation_" + annotType.GetValue());
        }

        [NUnit.Framework.TestCaseSource("Conformances")]
        public virtual void RubberStampAnnotationNoNameAndContentsTest(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfPage pdfPage = pdfDoc.AddNewPage();
                PdfStampAnnotation stamp = new PdfStampAnnotation(new Rectangle(100, 100));
                pdfPage.AddAnnotation(stamp);
            }
            );
            if (conformance.GetUAConformance() == PdfUAConformance.PDF_UA_2 || conformance.ConformsTo(WellTaggedPdfConformance
                .FOR_ACCESSIBILITY)) {
                framework.AssertBothFail("rubberStampAnnotationNoNameAndContents", PdfUAExceptionMessageConstants.STAMP_ANNOT_SHALL_SPECIFY_NAME_OR_CONTENTS
                    , false);
            }
            else {
                framework.AssertOnlyITextFail("rubberStampAnnotationNoNameAndContents", PdfUAExceptionMessageConstants.STAMP_ANNOT_SHALL_SPECIFY_NAME_OR_CONTENTS
                    );
            }
        }

        [NUnit.Framework.TestCaseSource("AnnotTypesToCheckContents")]
        public virtual void AnnotationNoContentsTest(PdfName annotType, PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
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
            if ((annotType == PdfName._3D || annotType == PdfName.RichMedia || annotType == PdfName.Ink) && conformance
                .ConformsTo(PdfConformance.WELL_TAGGED_PDF_FOR_REUSE) && !conformance.ConformsTo(PdfConformance.WELL_TAGGED_PDF_FOR_ACCESSIBILITY
                )) {
                framework.AssertBothValid("annotationNoContents_" + annotType.GetValue());
            }
            else {
                framework.AssertBothFail("annotationNoContents_" + annotType.GetValue(), PdfUAExceptionMessageConstants.ANNOT_CONTENTS_IS_NULL_OR_EMPTY
                    , false);
            }
        }

        [NUnit.Framework.TestCaseSource("AnnotTypesToCheckContents")]
        public virtual void AnnotationEmptyContentsTest(PdfName annotType, PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
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
            if (conformance.ConformsTo(PdfConformance.WELL_TAGGED_PDF_FOR_REUSE) && !conformance.ConformsTo(PdfConformance
                .WELL_TAGGED_PDF_FOR_ACCESSIBILITY)) {
                if (annotType == PdfName._3D || annotType == PdfName.RichMedia || annotType == PdfName.Ink) {
                    framework.AssertBothValid("annotationEmptyContents_" + annotType.GetValue());
                }
                else {
                    framework.AssertOnlyITextFail("annotationEmptyContents_" + annotType.GetValue(), PdfUAExceptionMessageConstants
                        .ANNOT_CONTENTS_IS_NULL_OR_EMPTY);
                }
            }
            else {
                framework.AssertOnlyITextFail("annotationEmptyContents_" + annotType.GetValue(), PdfUAExceptionMessageConstants
                    .ANNOT_CONTENTS_IS_NULL_OR_EMPTY);
            }
        }

        [NUnit.Framework.TestCaseSource("Conformances")]
        public virtual void PopupAnnotationTaggedAsAnnotTest(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfPage pdfPage = pdfDoc.AddNewPage();
                PdfPopupAnnotation popupAnnotation = new PdfPopupAnnotation(new Rectangle(100, 100));
                pdfPage.AddAnnotation(popupAnnotation);
            }
            );
            framework.AssertBothFail("popupAnnotationTaggedAsAnnot", PdfUAExceptionMessageConstants.POPUP_ANNOTATIONS_ARE_NOT_ALLOWED
                , false);
        }

        [NUnit.Framework.TestCaseSource("Conformances")]
        public virtual void PopupAnnotationTaggedAsArtifactTest(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
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
                , false);
        }

        [NUnit.Framework.TestCaseSource("Conformances")]
        public virtual void FileAttachmentAnnotationValidTest(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfPage pdfPage = pdfDoc.AddNewPage();
                Rectangle rect = new Rectangle(100, 650, 400, 100);
                PdfFileSpec fs = PdfFileSpec.CreateEmbeddedFileSpec(pdfDoc, "file".GetBytes(System.Text.Encoding.UTF8), "description"
                    , "file.txt", null, null, null);
                PdfFileAttachmentAnnotation annot = new PdfFileAttachmentAnnotation(rect, fs);
                pdfPage.AddAnnotation(annot);
            }
            );
            framework.AssertBothValid("fileAttachmentAnnotationValid");
        }

        [NUnit.Framework.TestCaseSource("Conformances")]
        public virtual void FileAttachmentAnnotationInvalidFileSpecTest(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfPage pdfPage = pdfDoc.AddNewPage();
                Rectangle rect = new Rectangle(100, 650, 400, 100);
                PdfFileSpec fs = PdfFileSpec.CreateEmbeddedFileSpec(pdfDoc, "file".GetBytes(System.Text.Encoding.UTF8), "description"
                    , "file.txt", null, null, null);
                PdfDictionary fsDict = (PdfDictionary)fs.GetPdfObject();
                fsDict.Remove(PdfName.AFRelationship);
                PdfFileAttachmentAnnotation annot = new PdfFileAttachmentAnnotation(rect, fs);
                pdfPage.AddAnnotation(annot);
            }
            );
            framework.AssertBothFail("fileAttachmentAnnotationInvalidFileSpec", PdfUAExceptionMessageConstants.FILE_SPEC_SHALL_CONTAIN_AFRELATIONSHIP
                , false);
        }

        [NUnit.Framework.TestCaseSource("DeprecatedAnnotTypes")]
        public virtual void DeprecatedAnnotationTypeTest(PdfName annotType, PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
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
                .DEPRECATED_ANNOTATIONS_ARE_NOT_ALLOWED, annotType.GetValue()), false);
        }

        [NUnit.Framework.TestCaseSource("Conformances")]
        public virtual void WidgetAnnotationZeroWidthAndHeightTaggedAsFormTest(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddSuppliers((document) => {
                CheckBox cb = new CheckBox("name");
                cb.SetAlternativeDescription("Contents");
                cb.SetProperty(Property.WIDTH, UnitValue.CreatePointValue(0));
                cb.SetProperty(Property.HEIGHT, UnitValue.CreatePointValue(0));
                cb.SetPdfConformance(PdfConformance.PDF_UA_2);
                cb.SetInteractive(true);
                return cb;
            }
            );
            framework.AssertBothFail("widgetAnnotationZeroWidthAndHeightTaggedAsForm", PdfUAExceptionMessageConstants.
                WIDGET_WITH_ZERO_HEIGHT_SHALL_BE_AN_ARTIFACT, false);
        }

        [NUnit.Framework.TestCaseSource("Conformances")]
        public virtual void PrinterMarkAnnotationTaggedAsAnnotTest(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
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
                , false);
        }

        [NUnit.Framework.TestCaseSource("Conformances")]
        public virtual void PrinterMarkAnnotationTaggedAsArtifactTest(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
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
            framework.AssertBothValid("printerMarkAnnotationTaggedAsArtifact");
        }

        [NUnit.Framework.TestCaseSource("Conformances")]
        public virtual void PrinterMarkAnnotationNotTaggedTest(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
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
            framework.AssertBothValid("printerMarkAnnotationNotTagged");
        }

        [NUnit.Framework.TestCaseSource("Conformances")]
        public virtual void WatermarkAnnotationAsRealContentTest(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
            framework.AddBeforeGenerationHook((pdfDoc) => {
                PdfPage pdfPage = pdfDoc.AddNewPage();
                PdfWatermarkAnnotation annot = new PdfWatermarkAnnotation(new Rectangle(100, 100));
                annot.SetContents("Contents");
                annot.Put(PdfName.RC, new PdfString("<p>Rich text</p>"));
                pdfPage.AddAnnotation(annot);
            }
            );
            framework.AssertOnlyITextFail("watermarkAnnotationAsRealContent", PdfUAExceptionMessageConstants.RC_DIFFERENT_FROM_CONTENTS
                );
        }

        [NUnit.Framework.TestCaseSource("Conformances")]
        public virtual void WatermarkAnnotationAsArtifactTest(PdfConformance conformance) {
            UaValidationTestFramework framework = new UaValidationTestFramework(DESTINATION_FOLDER, conformance);
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
            framework.AssertBothValid("watermarkAnnotationAsArtifact");
        }

        private static IList<Object[]> GenerateAllTypesOfDocuments(IEnumerable<PdfName> things) {
            IList<Object[]> list = new List<Object[]>();
            foreach (PdfConformance conformance in UaValidationTestFramework.GetConformanceList(false)) {
                foreach (PdfName thing in things) {
                    list.Add(new Object[] { thing, conformance });
                }
            }
            return list;
        }
    }
}
