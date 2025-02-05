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
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Commons.Utils;
using iText.Forms;
using iText.Forms.Fields;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Annot;

namespace iText.Forms.Xfdf {
//\cond DO_NOT_DOCUMENT
    internal class XfdfReader {
        private static readonly ILogger logger = ITextLogManager.GetLogger(typeof(XfdfReader));

        private readonly IDictionary<AnnotObject, PdfTextAnnotation> annotationsWithInReplyTo = new Dictionary<AnnotObject
            , PdfTextAnnotation>();

//\cond DO_NOT_DOCUMENT
        /// <summary>Merges existing XfdfObject into pdf document associated with it.</summary>
        /// <param name="xfdfObject">The object to be merged.</param>
        /// <param name="pdfDocument">The associated pdf document.</param>
        /// <param name="pdfDocumentName">The name of the associated pdf document.</param>
        internal virtual void MergeXfdfIntoPdf(XfdfObject xfdfObject, PdfDocument pdfDocument, String pdfDocumentName
            ) {
            if (xfdfObject.GetF() != null && xfdfObject.GetF().GetHref() != null) {
                if (pdfDocumentName.EqualsIgnoreCase(xfdfObject.GetF().GetHref())) {
                    logger.LogInformation("Xfdf href and pdf name are equal. Continue merge");
                }
                else {
                    logger.LogWarning(iText.IO.Logs.IoLogMessageConstant.XFDF_HREF_ATTRIBUTE_AND_PDF_DOCUMENT_NAME_ARE_DIFFERENT
                        );
                }
            }
            else {
                logger.LogWarning(iText.IO.Logs.IoLogMessageConstant.XFDF_NO_F_OBJECT_TO_COMPARE);
            }
            //TODO DEVSIX-4026 check for ids original/modified compatability with those in pdf document
            PdfAcroForm form = PdfFormCreator.GetAcroForm(pdfDocument, false);
            if (form != null) {
                MergeFields(xfdfObject.GetFields(), form);
                MergeAnnotations(xfdfObject.GetAnnots(), pdfDocument);
            }
        }
//\endcond

        /// <summary>
        /// Merges existing FieldsObject and children FieldObject entities into the form of the pdf document
        /// associated with it.
        /// </summary>
        /// <param name="fieldsObject">object containing acroform fields data to be merged.</param>
        /// <param name="form">acroform to be filled with xfdf data.</param>
        private void MergeFields(FieldsObject fieldsObject, PdfAcroForm form) {
            if (fieldsObject != null && fieldsObject.GetFieldList() != null && !fieldsObject.GetFieldList().IsEmpty()) {
                IDictionary<String, PdfFormField> formFields = form.GetAllFormFields();
                foreach (FieldObject xfdfField in fieldsObject.GetFieldList()) {
                    String name = xfdfField.GetName();
                    if (formFields.Get(name) != null && xfdfField.GetValue() != null) {
                        formFields.Get(name).SetValue(xfdfField.GetValue());
                    }
                    else {
                        logger.LogError(iText.IO.Logs.IoLogMessageConstant.XFDF_NO_SUCH_FIELD_IN_PDF_DOCUMENT);
                    }
                }
            }
        }

        /// <summary>Merges existing XfdfObject into pdf document associated with it.</summary>
        /// <param name="annotsObject">The AnnotsObject with children AnnotObject entities to be mapped into PdfAnnotations.
        ///     </param>
        /// <param name="pdfDocument">The associated pdf document.</param>
        private void MergeAnnotations(AnnotsObject annotsObject, PdfDocument pdfDocument) {
            IList<AnnotObject> annotList = null;
            if (annotsObject != null) {
                annotList = annotsObject.GetAnnotsList();
            }
            if (annotList != null && !annotList.IsEmpty()) {
                foreach (AnnotObject annot in annotList) {
                    AddAnnotationToPdf(annot, pdfDocument);
                }
            }
            SetInReplyTo(pdfDocument);
        }

        private void SetInReplyTo(PdfDocument pdfDocument) {
            foreach (KeyValuePair<AnnotObject, PdfTextAnnotation> annots in annotationsWithInReplyTo) {
                AnnotObject xfdfAnnot = annots.Key;
                String inReplyTo = xfdfAnnot.GetAttributeValue(XfdfConstants.IN_REPLY_TO);
                String replyType = xfdfAnnot.GetAttributeValue(XfdfConstants.REPLY_TYPE);
                foreach (PdfAnnotation pdfAnnotation in pdfDocument.GetPage(Convert.ToInt32(xfdfAnnot.GetAttributeValue(XfdfConstants
                    .PAGE), System.Globalization.CultureInfo.InvariantCulture)).GetAnnotations()) {
                    if (pdfAnnotation.GetName() != null && inReplyTo.Equals(pdfAnnotation.GetName().GetValue())) {
                        annots.Value.SetInReplyTo(pdfAnnotation);
                        if (replyType != null) {
                            annots.Value.SetReplyType(new PdfName(replyType));
                        }
                    }
                }
            }
        }

        private void AddCommonAnnotationAttributes(PdfAnnotation annotation, AnnotObject annotObject) {
            String flags = annotObject.GetAttributeValue(XfdfConstants.FLAGS);
            String color = annotObject.GetAttributeValue(XfdfConstants.COLOR);
            String date = annotObject.GetAttributeValue(XfdfConstants.DATE);
            String name = annotObject.GetAttributeValue(XfdfConstants.NAME);
            String title = annotObject.GetAttributeValue(XfdfConstants.TITLE);
            if (flags != null) {
                annotation.SetFlags(XfdfObjectUtils.ConvertFlagsFromString(flags));
            }
            if (color != null) {
                annotation.SetColor(XfdfObjectUtils.ConvertColorFloatsFromString(annotObject.GetAttributeValue(XfdfConstants
                    .COLOR)));
            }
            if (date != null) {
                annotation.SetDate(new PdfString(annotObject.GetAttributeValue(XfdfConstants.DATE)));
            }
            if (name != null) {
                annotation.SetName(new PdfString(annotObject.GetAttributeValue(XfdfConstants.NAME)));
            }
            if (title != null) {
                annotation.SetTitle(new PdfString(annotObject.GetAttributeValue(XfdfConstants.TITLE)));
            }
        }

        private void AddMarkupAnnotationAttributes(PdfMarkupAnnotation annotation, AnnotObject annotObject) {
            String creationDate = annotObject.GetAttributeValue(XfdfConstants.CREATION_DATE);
            String opacity = annotObject.GetAttributeValue(XfdfConstants.OPACITY);
            String subject = annotObject.GetAttributeValue(XfdfConstants.SUBJECT);
            if (creationDate != null) {
                annotation.SetCreationDate(new PdfString(creationDate));
            }
            if (opacity != null) {
                annotation.SetOpacity(new PdfNumber(Double.Parse(opacity, System.Globalization.CultureInfo.InvariantCulture
                    )));
            }
            if (subject != null) {
                annotation.SetSubject(new PdfString(subject));
            }
        }

        private void AddBorderStyleAttributes(PdfAnnotation annotation, AnnotObject annotObject) {
            PdfDictionary borderStyle = annotation.GetPdfObject().GetAsDictionary(PdfName.BS);
            if (borderStyle == null) {
                borderStyle = new PdfDictionary();
            }
            String width = annotObject.GetAttributeValue(XfdfConstants.WIDTH);
            String dashes = annotObject.GetAttributeValue(XfdfConstants.DASHES);
            String style = annotObject.GetAttributeValue(XfdfConstants.STYLE);
            if (width != null) {
                borderStyle.Put(PdfName.W, new PdfNumber(Double.Parse(width, System.Globalization.CultureInfo.InvariantCulture
                    )));
            }
            if (dashes != null) {
                borderStyle.Put(PdfName.D, XfdfObjectUtils.ConvertDashesFromString(dashes));
            }
            if (style != null && !"cloudy".Equals(style)) {
                borderStyle.Put(PdfName.S, new PdfName(style.JSubstring(0, 1).ToUpperInvariant()));
            }
            if (borderStyle.Size() > 0) {
                annotation.Put(PdfName.BS, borderStyle);
            }
        }

        private void AddBorderEffectAttributes(PdfAnnotation annotation, AnnotObject annotObject) {
            PdfDictionary borderEffect = annotation.GetPdfObject().GetAsDictionary(PdfName.BE);
            if (borderEffect == null) {
                borderEffect = new PdfDictionary();
            }
            String intensity = annotObject.GetAttributeValue(XfdfConstants.INTENSITY);
            bool isCloudyEffectSet = intensity != null;
            if (isCloudyEffectSet) {
                borderEffect.Put(PdfName.S, new PdfName("C"));
                borderEffect.Put(PdfName.I, new PdfNumber(Double.Parse(intensity, System.Globalization.CultureInfo.InvariantCulture
                    )));
                annotation.Put(PdfName.BE, borderEffect);
            }
        }

        private void AddAnnotationToPdf(AnnotObject annotObject, PdfDocument pdfDocument) {
            String annotName = annotObject.GetName();
            if (annotName != null) {
                switch (annotName) {
                    case XfdfConstants.TEXT: {
                        //TODO DEVSIX-4027 add all attributes properly one by one
                        PdfTextAnnotation pdfTextAnnotation = new PdfTextAnnotation(XfdfObjectUtils.ConvertRectFromString(annotObject
                            .GetAttributeValue(XfdfConstants.RECT)));
                        AddCommonAnnotationAttributes(pdfTextAnnotation, annotObject);
                        AddMarkupAnnotationAttributes(pdfTextAnnotation, annotObject);
                        String icon = annotObject.GetAttributeValue(XfdfConstants.ICON);
                        String state = annotObject.GetAttributeValue(XfdfConstants.STATE);
                        String stateModel = annotObject.GetAttributeValue(XfdfConstants.STATE_MODEL);
                        if (icon != null) {
                            pdfTextAnnotation.SetIconName(new PdfName(icon));
                        }
                        if (stateModel != null) {
                            pdfTextAnnotation.SetStateModel(new PdfString(stateModel));
                            if (state == null) {
                                state = "Marked".Equals(stateModel) ? "Unmarked" : "None";
                            }
                            pdfTextAnnotation.SetState(new PdfString(state));
                        }
                        String inReplyTo = annotObject.GetAttributeValue(XfdfConstants.IN_REPLY_TO);
                        if (inReplyTo != null) {
                            annotationsWithInReplyTo.Put(annotObject, pdfTextAnnotation);
                        }
                        pdfDocument.GetPage(Convert.ToInt32(annotObject.GetAttributeValue(XfdfConstants.PAGE), System.Globalization.CultureInfo.InvariantCulture
                            )).AddAnnotation(pdfTextAnnotation);
                        break;
                    }

                    case XfdfConstants.HIGHLIGHT: {
                        PdfTextMarkupAnnotation pdfHighLightAnnotation = new PdfTextMarkupAnnotation(XfdfObjectUtils.ConvertRectFromString
                            (annotObject.GetAttributeValue(XfdfConstants.RECT)), PdfName.Highlight, XfdfObjectUtils.ConvertQuadPointsFromCoordsString
                            (annotObject.GetAttributeValue(XfdfConstants.COORDS)));
                        AddCommonAnnotationAttributes(pdfHighLightAnnotation, annotObject);
                        AddMarkupAnnotationAttributes(pdfHighLightAnnotation, annotObject);
                        pdfDocument.GetPage(Convert.ToInt32(annotObject.GetAttribute(XfdfConstants.PAGE).GetValue(), System.Globalization.CultureInfo.InvariantCulture
                            )).AddAnnotation(pdfHighLightAnnotation);
                        break;
                    }

                    case XfdfConstants.UNDERLINE: {
                        PdfTextMarkupAnnotation pdfUnderlineAnnotation = new PdfTextMarkupAnnotation(XfdfObjectUtils.ConvertRectFromString
                            (annotObject.GetAttributeValue(XfdfConstants.RECT)), PdfName.Underline, XfdfObjectUtils.ConvertQuadPointsFromCoordsString
                            (annotObject.GetAttributeValue(XfdfConstants.COORDS)));
                        AddCommonAnnotationAttributes(pdfUnderlineAnnotation, annotObject);
                        AddMarkupAnnotationAttributes(pdfUnderlineAnnotation, annotObject);
                        String intent = annotObject.GetAttributeValue(XfdfConstants.INTENT);
                        if (intent != null) {
                            pdfUnderlineAnnotation.SetIntent(new PdfName(intent));
                        }
                        pdfDocument.GetPage(Convert.ToInt32(annotObject.GetAttribute(XfdfConstants.PAGE).GetValue(), System.Globalization.CultureInfo.InvariantCulture
                            )).AddAnnotation(pdfUnderlineAnnotation);
                        break;
                    }

                    case XfdfConstants.STRIKEOUT: {
                        PdfTextMarkupAnnotation pdfStrikeoutAnnotation = new PdfTextMarkupAnnotation(XfdfObjectUtils.ConvertRectFromString
                            (annotObject.GetAttributeValue(XfdfConstants.RECT)), PdfName.StrikeOut, XfdfObjectUtils.ConvertQuadPointsFromCoordsString
                            (annotObject.GetAttributeValue(XfdfConstants.COORDS)));
                        AddCommonAnnotationAttributes(pdfStrikeoutAnnotation, annotObject);
                        AddMarkupAnnotationAttributes(pdfStrikeoutAnnotation, annotObject);
                        pdfDocument.GetPage(Convert.ToInt32(annotObject.GetAttribute(XfdfConstants.PAGE).GetValue(), System.Globalization.CultureInfo.InvariantCulture
                            )).AddAnnotation(pdfStrikeoutAnnotation);
                        break;
                    }

                    case XfdfConstants.SQUIGGLY: {
                        PdfTextMarkupAnnotation pdfSquigglyAnnotation = new PdfTextMarkupAnnotation(XfdfObjectUtils.ConvertRectFromString
                            (annotObject.GetAttributeValue(XfdfConstants.RECT)), PdfName.Squiggly, XfdfObjectUtils.ConvertQuadPointsFromCoordsString
                            (annotObject.GetAttributeValue(XfdfConstants.COORDS)));
                        AddCommonAnnotationAttributes(pdfSquigglyAnnotation, annotObject);
                        AddMarkupAnnotationAttributes(pdfSquigglyAnnotation, annotObject);
                        pdfDocument.GetPage(Convert.ToInt32(annotObject.GetAttribute(XfdfConstants.PAGE).GetValue(), System.Globalization.CultureInfo.InvariantCulture
                            )).AddAnnotation(pdfSquigglyAnnotation);
                        break;
                    }

                    case XfdfConstants.CIRCLE: {
                        //                case XfdfConstants.LINE:
                        //                    pdfDocument.getPage(Integer.parseInt(annotObject.getAttribute(XfdfConstants.PAGE).getValue()))
                        //                            .addAnnotation(new PdfLineAnnotation(XfdfObjectUtils.convertRectFromString(annotObject.getAttributeValue(XfdfConstants.RECT)), XfdfObjectUtils.convertVerticesFromString(annotObject.getVertices())));
                        //                    break;
                        PdfCircleAnnotation pdfCircleAnnotation = new PdfCircleAnnotation(XfdfObjectUtils.ConvertRectFromString(annotObject
                            .GetAttributeValue(XfdfConstants.RECT)));
                        AddCommonAnnotationAttributes(pdfCircleAnnotation, annotObject);
                        AddMarkupAnnotationAttributes(pdfCircleAnnotation, annotObject);
                        AddBorderStyleAttributes(pdfCircleAnnotation, annotObject);
                        AddBorderEffectAttributes(pdfCircleAnnotation, annotObject);
                        if (annotObject.GetAttributeValue(XfdfConstants.INTERIOR_COLOR) != null) {
                            pdfCircleAnnotation.SetInteriorColor(XfdfObjectUtils.ConvertColorFloatsFromString(annotObject.GetAttributeValue
                                (XfdfConstants.INTERIOR_COLOR)));
                        }
                        if (annotObject.GetAttributeValue(XfdfConstants.FRINGE) != null) {
                            pdfCircleAnnotation.SetRectangleDifferences(XfdfObjectUtils.ConvertFringeFromString(annotObject.GetAttributeValue
                                (XfdfConstants.FRINGE)));
                        }
                        pdfDocument.GetPage(Convert.ToInt32(annotObject.GetAttribute(XfdfConstants.PAGE).GetValue(), System.Globalization.CultureInfo.InvariantCulture
                            )).AddAnnotation(pdfCircleAnnotation);
                        break;
                    }

                    case XfdfConstants.SQUARE: {
                        PdfSquareAnnotation pdfSquareAnnotation = new PdfSquareAnnotation(XfdfObjectUtils.ConvertRectFromString(annotObject
                            .GetAttributeValue(XfdfConstants.RECT)));
                        AddCommonAnnotationAttributes(pdfSquareAnnotation, annotObject);
                        AddMarkupAnnotationAttributes(pdfSquareAnnotation, annotObject);
                        AddBorderStyleAttributes(pdfSquareAnnotation, annotObject);
                        AddBorderEffectAttributes(pdfSquareAnnotation, annotObject);
                        if (annotObject.GetAttributeValue(XfdfConstants.INTERIOR_COLOR) != null) {
                            pdfSquareAnnotation.SetInteriorColor(XfdfObjectUtils.ConvertColorFloatsFromString(annotObject.GetAttributeValue
                                (XfdfConstants.INTERIOR_COLOR)));
                        }
                        if (annotObject.GetAttributeValue(XfdfConstants.FRINGE) != null) {
                            pdfSquareAnnotation.SetRectangleDifferences(XfdfObjectUtils.ConvertFringeFromString(annotObject.GetAttributeValue
                                (XfdfConstants.FRINGE)));
                        }
                        pdfDocument.GetPage(Convert.ToInt32(annotObject.GetAttribute(XfdfConstants.PAGE).GetValue(), System.Globalization.CultureInfo.InvariantCulture
                            )).AddAnnotation(pdfSquareAnnotation);
                        break;
                    }

                    case XfdfConstants.POLYGON: {
                        //XfdfConstants.CARET
                        Rectangle rect = XfdfObjectUtils.ConvertRectFromString(annotObject.GetAttributeValue(XfdfConstants.RECT));
                        float[] vertices = XfdfObjectUtils.ConvertVerticesFromString(annotObject.GetVertices());
                        PdfPolyGeomAnnotation polygonAnnotation = PdfPolyGeomAnnotation.CreatePolygon(rect, vertices);
                        AddCommonAnnotationAttributes(polygonAnnotation, annotObject);
                        AddMarkupAnnotationAttributes(polygonAnnotation, annotObject);
                        AddBorderStyleAttributes(polygonAnnotation, annotObject);
                        AddBorderEffectAttributes(polygonAnnotation, annotObject);
                        if (annotObject.GetAttributeValue(XfdfConstants.INTERIOR_COLOR) != null) {
                            polygonAnnotation.SetInteriorColor(XfdfObjectUtils.ConvertColorFloatsFromString(annotObject.GetAttributeValue
                                (XfdfConstants.INTERIOR_COLOR)));
                        }
                        if (annotObject.GetAttributeValue(XfdfConstants.INTENT) != null) {
                            polygonAnnotation.SetIntent(new PdfName(annotObject.GetAttributeValue(XfdfConstants.INTENT)));
                        }
                        pdfDocument.GetPage(Convert.ToInt32(annotObject.GetAttribute(XfdfConstants.PAGE).GetValue(), System.Globalization.CultureInfo.InvariantCulture
                            )).AddAnnotation(polygonAnnotation);
                        break;
                    }

                    case XfdfConstants.POLYLINE: {
                        Rectangle polylineRect = XfdfObjectUtils.ConvertRectFromString(annotObject.GetAttributeValue(XfdfConstants
                            .RECT));
                        float[] polylineVertices = XfdfObjectUtils.ConvertVerticesFromString(annotObject.GetVertices());
                        PdfPolyGeomAnnotation polylineAnnotation = PdfPolyGeomAnnotation.CreatePolyLine(polylineRect, polylineVertices
                            );
                        AddCommonAnnotationAttributes(polylineAnnotation, annotObject);
                        AddMarkupAnnotationAttributes(polylineAnnotation, annotObject);
                        AddBorderStyleAttributes(polylineAnnotation, annotObject);
                        AddBorderEffectAttributes(polylineAnnotation, annotObject);
                        if (annotObject.GetAttributeValue(XfdfConstants.INTERIOR_COLOR) != null) {
                            polylineAnnotation.SetInteriorColor(XfdfObjectUtils.ConvertColorFloatsFromString(annotObject.GetAttributeValue
                                (XfdfConstants.INTERIOR_COLOR)));
                        }
                        if (annotObject.GetAttributeValue(XfdfConstants.INTENT) != null) {
                            polylineAnnotation.SetIntent(new PdfName(annotObject.GetAttributeValue(XfdfConstants.INTENT)));
                        }
                        String head = annotObject.GetAttributeValue(XfdfConstants.HEAD);
                        String tail = annotObject.GetAttributeValue(XfdfConstants.TAIL);
                        if (head != null || tail != null) {
                            PdfArray lineEndingStyles = new PdfArray();
                            lineEndingStyles.Add(new PdfName(head == null ? "None" : head));
                            lineEndingStyles.Add(new PdfName(tail == null ? "None" : tail));
                            polylineAnnotation.SetLineEndingStyles(lineEndingStyles);
                        }
                        pdfDocument.GetPage(Convert.ToInt32(annotObject.GetAttribute(XfdfConstants.PAGE).GetValue(), System.Globalization.CultureInfo.InvariantCulture
                            )).AddAnnotation(polylineAnnotation);
                        break;
                    }

                    case XfdfConstants.STAMP: {
                        PdfStampAnnotation pdfStampAnnotation = new PdfStampAnnotation(XfdfObjectUtils.ConvertRectFromString(annotObject
                            .GetAttributeValue(XfdfConstants.RECT)));
                        AddCommonAnnotationAttributes(pdfStampAnnotation, annotObject);
                        AddMarkupAnnotationAttributes(pdfStampAnnotation, annotObject);
                        if (annotObject.GetAttributeValue(XfdfConstants.ICON) != null) {
                            pdfStampAnnotation.SetIconName(new PdfName(annotObject.GetAttributeValue(XfdfConstants.ICON)));
                        }
                        if (annotObject.GetAttributeValue(XfdfConstants.ROTATION) != null) {
                            pdfStampAnnotation.SetRotation(Convert.ToInt32(annotObject.GetAttributeValue(XfdfConstants.ROTATION), System.Globalization.CultureInfo.InvariantCulture
                                ));
                        }
                        pdfDocument.GetPage(Convert.ToInt32(annotObject.GetAttribute(XfdfConstants.PAGE).GetValue(), System.Globalization.CultureInfo.InvariantCulture
                            )).AddAnnotation(pdfStampAnnotation);
                        break;
                    }

                    case XfdfConstants.FREETEXT: {
                        //XfdfConstants.INK
                        PdfFreeTextAnnotation pdfFreeTextAnnotation = new PdfFreeTextAnnotation(XfdfObjectUtils.ConvertRectFromString
                            (annotObject.GetAttributeValue(XfdfConstants.RECT)), annotObject.GetContents());
                        AddCommonAnnotationAttributes(pdfFreeTextAnnotation, annotObject);
                        AddMarkupAnnotationAttributes(pdfFreeTextAnnotation, annotObject);
                        AddBorderStyleAttributes(pdfFreeTextAnnotation, annotObject);
                        if (annotObject.GetAttributeValue(XfdfConstants.ROTATION) != null) {
                            pdfFreeTextAnnotation.SetRotation(Convert.ToInt32(annotObject.GetAttributeValue(XfdfConstants.ROTATION), System.Globalization.CultureInfo.InvariantCulture
                                ));
                        }
                        if (annotObject.GetAttributeValue(XfdfConstants.JUSTIFICATION) != null) {
                            pdfFreeTextAnnotation.SetJustification(XfdfObjectUtils.ConvertJustificationFromStringToInteger(annotObject
                                .GetAttributeValue(XfdfConstants.JUSTIFICATION)));
                        }
                        if (annotObject.GetAttributeValue(XfdfConstants.INTENT) != null) {
                            pdfFreeTextAnnotation.SetIntent(new PdfName(annotObject.GetAttributeValue(XfdfConstants.INTENT)));
                        }
                        pdfDocument.GetPage(Convert.ToInt32(annotObject.GetAttribute(XfdfConstants.PAGE).GetValue(), System.Globalization.CultureInfo.InvariantCulture
                            )).AddAnnotation(pdfFreeTextAnnotation);
                        break;
                    }

                    default: {
                        //XfdfConstants.FILEATTACHMENT
                        //XfdfConstants.SOUND
                        //XfdfConstants.LINK
                        //XfdfConstants.REDACT
                        //XfdfConstants.PROJECTION
                        logger.LogWarning(MessageFormatUtil.Format(iText.IO.Logs.IoLogMessageConstant.XFDF_ANNOTATION_IS_NOT_SUPPORTED
                            , annotName));
                        break;
                    }
                }
            }
        }
    }
//\endcond
}
