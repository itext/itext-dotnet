/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: iText Software.

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License version 3
as published by the Free Software Foundation with the addition of the
following permission added to Section 15 as permitted in Section 7(a):
FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
OF THIRD PARTY RIGHTS

This program is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU Affero General Public License for more details.
You should have received a copy of the GNU Affero General Public License
along with this program; if not, see http://www.gnu.org/licenses or write to
the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
Boston, MA, 02110-1301 USA, or download the license from the following URL:
http://itextpdf.com/terms-of-use/

The interactive user interfaces in modified source and object code versions
of this program must display Appropriate Legal Notices, as required under
Section 5 of the GNU Affero General Public License.

In accordance with Section 7(b) of the GNU Affero General Public License,
a covered work must retain the producer line in every PDF that is created
or manipulated using iText.

You can be released from the requirements of the license by purchasing
a commercial license. Buying such a license is mandatory as soon as you
develop commercial activities involving the iText software without
disclosing the source code of your own applications.
These activities include: offering paid services to customers as an ASP,
serving PDFs on the fly in a web application, shipping iText with a closed
source product.

For more information, please contact iText Software Corp. at this
address: sales@itextpdf.com
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
    internal class XfdfReader {
        private static ILogger logger = ITextLogManager.GetLogger(typeof(XfdfReader));

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
            PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDocument, false);
            if (form != null) {
                MergeFields(xfdfObject.GetFields(), form);
                MergeAnnotations(xfdfObject.GetAnnots(), pdfDocument);
            }
        }

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
        }

        private void AddCommonAnnotationAttributes(PdfAnnotation annotation, AnnotObject annotObject) {
            annotation.SetFlags(XfdfObjectUtils.ConvertFlagsFromString(annotObject.GetAttributeValue(XfdfConstants.FLAGS
                )));
            annotation.SetColor(XfdfObjectUtils.ConvertColorFloatsFromString(annotObject.GetAttributeValue(XfdfConstants
                .COLOR)));
            annotation.SetDate(new PdfString(annotObject.GetAttributeValue(XfdfConstants.DATE)));
            annotation.SetName(new PdfString(annotObject.GetAttributeValue(XfdfConstants.NAME)));
            annotation.SetTitle(new PdfString(annotObject.GetAttributeValue(XfdfConstants.TITLE)));
        }

        private void AddMarkupAnnotationAttributes(PdfMarkupAnnotation annotation, AnnotObject annotObject) {
            annotation.SetCreationDate(new PdfString(annotObject.GetAttributeValue(XfdfConstants.CREATION_DATE)));
            annotation.SetSubject(new PdfString(annotObject.GetAttributeValue(XfdfConstants.SUBJECT)));
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
                        pdfTextAnnotation.SetIconName(new PdfName(annotObject.GetAttributeValue(XfdfConstants.ICON)));
                        if (annotObject.GetAttributeValue(XfdfConstants.STATE) != null) {
                            pdfTextAnnotation.SetState(new PdfString(annotObject.GetAttributeValue(XfdfConstants.STATE)));
                        }
                        if (annotObject.GetAttributeValue(XfdfConstants.STATE_MODEL) != null) {
                            pdfTextAnnotation.SetStateModel(new PdfString(annotObject.GetAttributeValue(XfdfConstants.STATE_MODEL)));
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
                        pdfDocument.GetPage(Convert.ToInt32(annotObject.GetAttribute(XfdfConstants.PAGE).GetValue(), System.Globalization.CultureInfo.InvariantCulture
                            )).AddAnnotation(polylineAnnotation);
                        break;
                    }

                    case XfdfConstants.STAMP: {
                        pdfDocument.GetPage(Convert.ToInt32(annotObject.GetAttribute(XfdfConstants.PAGE).GetValue(), System.Globalization.CultureInfo.InvariantCulture
                            )).AddAnnotation(new PdfStampAnnotation(XfdfObjectUtils.ConvertRectFromString(annotObject.GetAttributeValue
                            (XfdfConstants.RECT))));
                        break;
                    }

                    case XfdfConstants.FREETEXT: {
                        //XfdfConstants.INK
                        pdfDocument.GetPage(Convert.ToInt32(annotObject.GetAttribute(XfdfConstants.PAGE).GetValue(), System.Globalization.CultureInfo.InvariantCulture
                            )).AddAnnotation(new PdfFreeTextAnnotation(XfdfObjectUtils.ConvertRectFromString(annotObject.GetAttributeValue
                            (XfdfConstants.RECT)), annotObject.GetContents()));
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
}
