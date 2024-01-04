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
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Commons.Utils;
using iText.Forms;
using iText.Forms.Fields;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Annot;

namespace iText.Forms.Xfdf {
    public class XfdfObjectFactory {
        private static readonly ILogger logger = ITextLogManager.GetLogger(typeof(XfdfObjectFactory));

        /// <summary>Extracts data from pdf document acroform and annotations into XfdfObject.</summary>
        /// <remarks>
        /// Extracts data from pdf document acroform and annotations into XfdfObject.
        /// *
        /// </remarks>
        /// <param name="document">Pdf document for data extraction.</param>
        /// <param name="filename">The name od pdf document for data extraction.</param>
        /// <returns>XfdfObject containing data from pdf forms and annotations.</returns>
        public virtual XfdfObject CreateXfdfObject(PdfDocument document, String filename) {
            PdfAcroForm form = PdfFormCreator.GetAcroForm(document, false);
            XfdfObject resultXfdf = new XfdfObject();
            FieldsObject xfdfFields = new FieldsObject();
            if (form != null && form.GetRootFormFields() != null && !form.GetRootFormFields().IsEmpty()) {
                foreach (String fieldName in form.GetAllFormFields().Keys) {
                    String delims = ".";
                    StringTokenizer st = new StringTokenizer(fieldName, delims);
                    IList<String> nameParts = new List<String>();
                    while (st.HasMoreTokens()) {
                        nameParts.Add(st.NextToken());
                    }
                    String name = nameParts[nameParts.Count - 1];
                    String value = form.GetField(fieldName).GetValueAsString();
                    FieldObject childField = new FieldObject(name, value, false);
                    if (nameParts.Count > 1) {
                        FieldObject parentField = new FieldObject();
                        parentField.SetName(nameParts[nameParts.Count - 2]);
                        childField.SetParent(parentField);
                    }
                    xfdfFields.AddField(childField);
                }
            }
            resultXfdf.SetFields(xfdfFields);
            String original = XfdfObjectUtils.ConvertIdToHexString(document.GetOriginalDocumentId().GetValue());
            String modified = XfdfObjectUtils.ConvertIdToHexString(document.GetModifiedDocumentId().GetValue());
            IdsObject ids = new IdsObject().SetOriginal(original).SetModified(modified);
            resultXfdf.SetIds(ids);
            FObject f = new FObject(filename);
            resultXfdf.SetF(f);
            AddAnnotations(document, resultXfdf);
            return resultXfdf;
        }

        /// <summary>Extracts data from input stream into XfdfObject.</summary>
        /// <remarks>Extracts data from input stream into XfdfObject. Typically input stream is based on .xfdf file</remarks>
        /// <param name="xfdfInputStream">The input stream containing xml-styled xfdf data.</param>
        /// <returns>XfdfObject containing original xfdf data.</returns>
        public virtual XfdfObject CreateXfdfObject(Stream xfdfInputStream) {
            XfdfObject xfdfObject = new XfdfObject();
            XmlDocument document = XfdfFileUtils.CreateXfdfDocumentFromStream(xfdfInputStream);
            XmlElement root = document.DocumentElement;
            IList<AttributeObject> xfdfRootAttributes = ReadXfdfRootAttributes(root);
            xfdfObject.SetAttributes(xfdfRootAttributes);
            XmlNodeList nodeList = root.ChildNodes;
            VisitChildNodes(nodeList, xfdfObject);
            return xfdfObject;
        }

        private void VisitFNode(XmlNode node, XfdfObject xfdfObject) {
            if (node.Attributes != null) {
                XmlNode href = node.Attributes.GetNamedItem(XfdfConstants.HREF);
                if (href != null) {
                    xfdfObject.SetF(new FObject(href.Value));
                }
                else {
                    logger.LogInformation(XfdfConstants.EMPTY_F_LEMENT);
                }
            }
        }

        private void VisitIdsNode(XmlNode node, XfdfObject xfdfObject) {
            IdsObject idsObject = new IdsObject();
            if (node.Attributes != null) {
                XmlNode original = node.Attributes.GetNamedItem(XfdfConstants.ORIGINAL);
                if (original != null) {
                    idsObject.SetOriginal(original.Value);
                }
                XmlNode modified = node.Attributes.GetNamedItem(XfdfConstants.MODIFIED);
                if (modified != null) {
                    idsObject.SetModified(modified.Value);
                }
                xfdfObject.SetIds(idsObject);
            }
            else {
                logger.LogInformation(XfdfConstants.EMPTY_IDS_ELEMENT);
            }
        }

        private void VisitElementNode(XmlNode node, XfdfObject xfdfObject) {
            if (XfdfConstants.FIELDS.EqualsIgnoreCase(node.Name)) {
                FieldsObject fieldsObject = new FieldsObject();
                ReadFieldList(node, fieldsObject);
                xfdfObject.SetFields(fieldsObject);
            }
            if (XfdfConstants.F.EqualsIgnoreCase(node.Name)) {
                VisitFNode(node, xfdfObject);
            }
            if (XfdfConstants.IDS.EqualsIgnoreCase(node.Name)) {
                VisitIdsNode(node, xfdfObject);
            }
            if (XfdfConstants.ANNOTS.EqualsIgnoreCase(node.Name)) {
                AnnotsObject annotsObject = new AnnotsObject();
                ReadAnnotsList(node, annotsObject);
                xfdfObject.SetAnnots(annotsObject);
            }
        }

        private void VisitChildNodes(XmlNodeList nList, XfdfObject xfdfObject) {
            for (int temp = 0; temp < nList.Count; temp++) {
                XmlNode node = nList.Item(temp);
                if (node.NodeType == System.Xml.XmlNodeType.Element) {
                    VisitElementNode(node, xfdfObject);
                }
            }
        }

        private static bool IsAnnotSupported(String nodeName) {
            return XfdfConstants.TEXT.EqualsIgnoreCase(nodeName) || XfdfConstants.HIGHLIGHT.EqualsIgnoreCase(nodeName)
                 || XfdfConstants.UNDERLINE.EqualsIgnoreCase(nodeName) || XfdfConstants.STRIKEOUT.EqualsIgnoreCase(nodeName
                ) || XfdfConstants.SQUIGGLY.EqualsIgnoreCase(nodeName) || XfdfConstants.CIRCLE.EqualsIgnoreCase(nodeName
                ) || XfdfConstants.SQUARE.EqualsIgnoreCase(nodeName) || XfdfConstants.POLYLINE.EqualsIgnoreCase(nodeName
                ) || XfdfConstants.POLYGON.EqualsIgnoreCase(nodeName) || XfdfConstants.STAMP.EqualsIgnoreCase(nodeName
                ) || XfdfConstants.LINE
                        //               XfdfConstants.FREETEXT.equalsIgnoreCase(nodeName) ||
                        .EqualsIgnoreCase(nodeName);
        }

        private void ReadAnnotsList(XmlNode node, AnnotsObject annotsObject) {
            XmlNodeList annotsNodeList = node.ChildNodes;
            for (int temp = 0; temp < annotsNodeList.Count; temp++) {
                XmlNode currentNode = annotsNodeList.Item(temp);
                if (currentNode.NodeType == System.Xml.XmlNodeType.Element && IsAnnotationSubtype(currentNode.Name) && IsAnnotSupported
                    (currentNode.Name)) {
                    VisitAnnotationNode(currentNode, annotsObject);
                }
            }
        }

        private void VisitAnnotationNode(XmlNode currentNode, AnnotsObject annotsObject) {
            AnnotObject annotObject = new AnnotObject();
            annotObject.SetName(currentNode.Name);
            if (currentNode.Attributes != null) {
                XmlNamedNodeMap attributes = currentNode.Attributes;
                for (int i = 0; i < attributes.Count; i++) {
                    AddAnnotObjectAttribute(annotObject, attributes.Item(i));
                }
                VisitAnnotationInnerNodes(annotObject, currentNode, annotsObject);
                annotsObject.AddAnnot(annotObject);
            }
        }

        private void VisitAnnotationInnerNodes(AnnotObject annotObject, XmlNode annotNode, AnnotsObject annotsObject
            ) {
            XmlNodeList children = annotNode.ChildNodes;
            for (int temp = 0; temp < children.Count; temp++) {
                XmlNode node = children.Item(temp);
                if (node.NodeType == System.Xml.XmlNodeType.Element) {
                    if (XfdfConstants.CONTENTS.EqualsIgnoreCase(node.Name)) {
                        VisitContentsSubelement(node, annotObject);
                    }
                    if (XfdfConstants.CONTENTS_RICHTEXT.EqualsIgnoreCase(node.Name)) {
                        VisitContentsRichTextSubelement(node, annotObject);
                    }
                    if (XfdfConstants.POPUP.EqualsIgnoreCase(node.Name)) {
                        VisitPopupSubelement(node, annotObject);
                    }
                    if (XfdfConstants.VERTICES.EqualsIgnoreCase(node.Name)) {
                        VisitVerticesSubelement(node, annotObject);
                    }
                    if (IsAnnotationSubtype(node.Name) && IsAnnotSupported(node.Name)) {
                        VisitAnnotationNode(node, annotsObject);
                    }
                }
            }
        }

        private void VisitPopupSubelement(XmlNode popupNode, AnnotObject annotObject) {
            //nothing inside
            //attr list : color, date, flags, name, rect (required), title. open
            AnnotObject popupAnnotObject = new AnnotObject();
            XmlNamedNodeMap attributes = popupNode.Attributes;
            for (int i = 0; i < attributes.Count; i++) {
                AddAnnotObjectAttribute(popupAnnotObject, attributes.Item(i));
            }
            annotObject.SetPopup(popupAnnotObject);
        }

        private void VisitContentsSubelement(XmlNode parentNode, AnnotObject annotObject) {
            //no attributes. inside a text string
            XmlNodeList children = parentNode.ChildNodes;
            for (int temp = 0; temp < children.Count; temp++) {
                XmlNode node = children.Item(temp);
                if (node.NodeType == System.Xml.XmlNodeType.Text) {
                    annotObject.SetContents(new PdfString(node.Value));
                }
            }
        }

        private void VisitContentsRichTextSubelement(XmlNode parentNode, AnnotObject annotObject) {
            // no attributes, inside a text string or rich text string
            XmlNodeList children = parentNode.ChildNodes;
            for (int temp = 0; temp < children.Count; temp++) {
                XmlNode node = children.Item(temp);
                if (node.NodeType == System.Xml.XmlNodeType.Text) {
                    annotObject.SetContentsRichText(new PdfString(node.Value));
                }
            }
        }

        private void VisitVerticesSubelement(XmlNode parentNode, AnnotObject annotObject) {
            //no attributes, inside a text string
            XmlNodeList children = parentNode.ChildNodes;
            for (int temp = 0; temp < children.Count; temp++) {
                XmlNode node = children.Item(temp);
                if (node.NodeType == System.Xml.XmlNodeType.Text) {
                    annotObject.SetVertices(node.Value);
                }
            }
        }

        private void AddAnnotObjectAttribute(AnnotObject annotObject, XmlNode attributeNode) {
            if (attributeNode != null) {
                String attributeName = attributeNode.Name;
                switch (attributeName) {
                    case XfdfConstants.PAGE: {
                        //required
                        annotObject.AddFdfAttributes(Convert.ToInt32(attributeNode.Value, System.Globalization.CultureInfo.InvariantCulture
                            ));
                        break;
                    }

                    case XfdfConstants.COLOR:
                    case XfdfConstants.DATE:
                    case XfdfConstants.FLAGS:
                    case XfdfConstants.NAME:
                    case XfdfConstants.RECT:
                    case XfdfConstants.TITLE:
                    case XfdfConstants.CREATION_DATE:
                    case XfdfConstants.OPACITY:
                    case XfdfConstants.SUBJECT:
                    case XfdfConstants.ICON:
                    case XfdfConstants.STATE:
                    case XfdfConstants.STATE_MODEL:
                    case XfdfConstants.IN_REPLY_TO:
                    case XfdfConstants.REPLY_TYPE:
                    case XfdfConstants.OPEN:
                    case XfdfConstants.COORDS:
                    case XfdfConstants.INTENT:
                    case XfdfConstants.INTERIOR_COLOR:
                    case XfdfConstants.HEAD:
                    case XfdfConstants.TAIL:
                    case XfdfConstants.FRINGE:
                    case XfdfConstants.ROTATION:
                    case XfdfConstants.JUSTIFICATION:
                    case XfdfConstants.WIDTH:
                    case XfdfConstants.DASHES:
                    case XfdfConstants.STYLE:
                    case XfdfConstants.INTENSITY: {
                        //required
                        annotObject.AddAttribute(new AttributeObject(attributeName, attributeNode.Value));
                        break;
                    }

                    default: {
                        logger.LogWarning(iText.IO.Logs.IoLogMessageConstant.XFDF_UNSUPPORTED_ANNOTATION_ATTRIBUTE);
                        break;
                    }
                }
            }
        }

        private bool IsAnnotationSubtype(String tag) {
            return XfdfConstants.TEXT.EqualsIgnoreCase(tag) || XfdfConstants.HIGHLIGHT.EqualsIgnoreCase(tag) || XfdfConstants
                .UNDERLINE.EqualsIgnoreCase(tag) || XfdfConstants.STRIKEOUT.EqualsIgnoreCase(tag) || XfdfConstants.SQUIGGLY
                .EqualsIgnoreCase(tag) || XfdfConstants.LINE.EqualsIgnoreCase(tag) || XfdfConstants.CIRCLE.EqualsIgnoreCase
                (tag) || XfdfConstants.SQUARE.EqualsIgnoreCase(tag) || XfdfConstants.CARET.EqualsIgnoreCase(tag) || XfdfConstants
                .POLYGON.EqualsIgnoreCase(tag) || XfdfConstants.POLYLINE.EqualsIgnoreCase(tag) || XfdfConstants.STAMP.
                EqualsIgnoreCase(tag) || XfdfConstants.INK.EqualsIgnoreCase(tag) || XfdfConstants.FREETEXT.EqualsIgnoreCase
                (tag) || XfdfConstants.FILEATTACHMENT.EqualsIgnoreCase(tag) || XfdfConstants.SOUND.EqualsIgnoreCase(tag
                ) || XfdfConstants.LINK.EqualsIgnoreCase(tag) || XfdfConstants.REDACT.EqualsIgnoreCase(tag) || XfdfConstants
                .PROJECTION.EqualsIgnoreCase(tag);
        }

        //projection annotation is not supported in iText
        private void ReadFieldList(XmlNode node, FieldsObject fieldsObject) {
            XmlNodeList fieldNodeList = node.ChildNodes;
            for (int temp = 0; temp < fieldNodeList.Count; temp++) {
                XmlNode currentNode = fieldNodeList.Item(temp);
                if (currentNode.NodeType == System.Xml.XmlNodeType.Element && XfdfConstants.FIELD.EqualsIgnoreCase(currentNode
                    .Name)) {
                    FieldObject fieldObject = new FieldObject();
                    VisitInnerFields(fieldObject, currentNode, fieldsObject);
                }
            }
        }

        private void VisitFieldElementNode(XmlNode node, FieldObject parentField, FieldsObject fieldsObject) {
            if (XfdfConstants.VALUE.EqualsIgnoreCase(node.Name)) {
                XmlNode valueTextNode = node.FirstChild;
                if (valueTextNode != null) {
                    parentField.SetValue(valueTextNode.InnerText);
                }
                else {
                    logger.LogInformation(XfdfConstants.EMPTY_FIELD_VALUE_ELEMENT);
                }
                return;
            }
            if (XfdfConstants.FIELD.EqualsIgnoreCase(node.Name)) {
                FieldObject childField = new FieldObject();
                childField.SetParent(parentField);
                childField.SetName(parentField.GetName() + "." + node.Attributes.Item(0).Value);
                if (node.ChildNodes != null) {
                    VisitInnerFields(childField, node, fieldsObject);
                }
                fieldsObject.AddField(childField);
            }
        }

        private void VisitInnerFields(FieldObject parentField, XmlNode parentNode, FieldsObject fieldsObject) {
            if (parentNode.Attributes.Count != 0) {
                if (parentField.GetName() == null) {
                    parentField.SetName(parentNode.Attributes.Item(0).Value);
                }
            }
            else {
                logger.LogInformation(XfdfConstants.EMPTY_FIELD_NAME_ELEMENT);
            }
            XmlNodeList children = parentNode.ChildNodes;
            for (int temp = 0; temp < children.Count; temp++) {
                XmlNode node = children.Item(temp);
                if (node.NodeType == System.Xml.XmlNodeType.Element) {
                    VisitFieldElementNode(node, parentField, fieldsObject);
                }
            }
            fieldsObject.AddField(parentField);
        }

        private IList<AttributeObject> ReadXfdfRootAttributes(XmlElement root) {
            XmlNamedNodeMap attributes = root.Attributes;
            int length = attributes.Count;
            IList<AttributeObject> attributeObjects = new List<AttributeObject>();
            for (int i = 0; i < length; i++) {
                XmlNode attributeNode = attributes.Item(i);
                attributeObjects.Add(new AttributeObject(attributeNode.Name, attributeNode.Value));
            }
            return attributeObjects;
        }

        private static void AddPopup(PdfAnnotation pdfAnnot, AnnotsObject annots, int pageNumber) {
            if (((PdfPopupAnnotation)pdfAnnot).GetParentObject() != null) {
                PdfAnnotation parentAnnotation = ((PdfPopupAnnotation)pdfAnnot).GetParent();
                PdfIndirectReference parentRef = parentAnnotation.GetPdfObject().GetIndirectReference();
                bool hasParentAnnot = false;
                foreach (AnnotObject annot in annots.GetAnnotsList()) {
                    if (parentRef.Equals(annot.GetRef())) {
                        hasParentAnnot = true;
                        annot.SetHasPopup(true);
                        annot.SetPopup(CreateXfdfAnnotation(pdfAnnot, pageNumber));
                    }
                }
                if (!hasParentAnnot) {
                    AnnotObject parentAnnot = new AnnotObject();
                    parentAnnot.SetRef(parentRef);
                    parentAnnot.AddFdfAttributes(pageNumber);
                    parentAnnot.SetHasPopup(true);
                    parentAnnot.SetPopup(CreateXfdfAnnotation(pdfAnnot, pageNumber));
                    annots.AddAnnot(parentAnnot);
                }
            }
            else {
                annots.AddAnnot(CreateXfdfAnnotation(pdfAnnot, pageNumber));
            }
        }

        private static void AddAnnotation(PdfAnnotation pdfAnnot, AnnotsObject annots, int pageNumber) {
            bool hasCorrecpondingAnnotObject = false;
            foreach (AnnotObject annot in annots.GetAnnotsList()) {
                if (pdfAnnot.GetPdfObject().GetIndirectReference().Equals(annot.GetRef())) {
                    hasCorrecpondingAnnotObject = true;
                    UpdateXfdfAnnotation(annot, pdfAnnot, pageNumber);
                }
            }
            if (!hasCorrecpondingAnnotObject) {
                annots.AddAnnot(CreateXfdfAnnotation(pdfAnnot, pageNumber));
            }
        }

        private static void AddAnnotations(PdfDocument pdfDoc, XfdfObject resultXfdf) {
            AnnotsObject annots = new AnnotsObject();
            int pageNumber = pdfDoc.GetNumberOfPages();
            for (int i = 1; i <= pageNumber; i++) {
                PdfPage page = pdfDoc.GetPage(i);
                IList<PdfAnnotation> pdfAnnots = page.GetAnnotations();
                foreach (PdfAnnotation pdfAnnot in pdfAnnots) {
                    if (pdfAnnot.GetSubtype() == PdfName.Popup) {
                        AddPopup(pdfAnnot, annots, i);
                    }
                    else {
                        AddAnnotation(pdfAnnot, annots, i);
                    }
                }
            }
            resultXfdf.SetAnnots(annots);
        }

        private static void UpdateXfdfAnnotation(AnnotObject annotObject, PdfAnnotation pdfAnnotation, int pageNumber
            ) {
        }

        //TODO DEVSIX-4132 implement update, refactor createXfdfAnnotation() method to accommodate the change
        private static void AddCommonAnnotationAttributes(AnnotObject annot, PdfAnnotation pdfAnnotation) {
            annot.SetName(pdfAnnotation.GetSubtype().GetValue().ToLowerInvariant());
            if (pdfAnnotation.GetColorObject() != null) {
                annot.AddAttribute(new AttributeObject(XfdfConstants.COLOR, XfdfObjectUtils.ConvertColorToString(pdfAnnotation
                    .GetColorObject().ToFloatArray())));
            }
            annot.AddAttribute(XfdfConstants.DATE, pdfAnnotation.GetDate());
            String flagsString = XfdfObjectUtils.ConvertFlagsToString(pdfAnnotation);
            if (flagsString != null) {
                annot.AddAttribute(new AttributeObject(XfdfConstants.FLAGS, flagsString));
            }
            annot.AddAttribute(XfdfConstants.NAME, pdfAnnotation.GetName());
            //rect attribute is required, however iText can't create an annotation without rect anyway
            annot.AddAttribute(XfdfConstants.RECT, pdfAnnotation.GetRectangle().ToRectangle());
            annot.AddAttribute(XfdfConstants.TITLE, pdfAnnotation.GetTitle());
        }

        private static void AddMarkupAnnotationAttributes(AnnotObject annot, PdfMarkupAnnotation pdfMarkupAnnotation
            ) {
            annot.AddAttribute(XfdfConstants.CREATION_DATE, pdfMarkupAnnotation.GetCreationDate());
            annot.AddAttribute(XfdfConstants.OPACITY, pdfMarkupAnnotation.GetOpacity());
            annot.AddAttribute(XfdfConstants.SUBJECT, pdfMarkupAnnotation.GetSubject());
        }

        private static void AddBorderStyleAttributes(AnnotObject annotObject, PdfNumber width, PdfArray dashes, PdfName
             style) {
            annotObject.AddAttribute(XfdfConstants.WIDTH, width);
            annotObject.AddAttribute(XfdfConstants.DASHES, XfdfObjectUtils.ConvertDashesFromArray(dashes));
            annotObject.AddAttribute(XfdfConstants.STYLE, XfdfObjectUtils.GetStyleFullValue(style));
        }

        private static void CreateTextMarkupAnnotation(PdfAnnotation pdfAnnotation, AnnotObject annot, int pageNumber
            ) {
            PdfTextMarkupAnnotation pdfTextMarkupAnnotation = (PdfTextMarkupAnnotation)pdfAnnotation;
            if (pdfTextMarkupAnnotation.GetQuadPoints() != null) {
                annot.AddAttribute(new AttributeObject(XfdfConstants.COORDS, XfdfObjectUtils.ConvertQuadPointsToCoordsString
                    (pdfTextMarkupAnnotation.GetQuadPoints().ToFloatArray())));
            }
            if (PdfTextMarkupAnnotation.MarkupUnderline.Equals(pdfTextMarkupAnnotation.GetSubtype()) && pdfTextMarkupAnnotation
                .GetIntent() != null) {
                annot.AddAttribute(new AttributeObject(XfdfConstants.INTENT, pdfTextMarkupAnnotation.GetIntent().GetValue(
                    )));
            }
            if (pdfTextMarkupAnnotation.GetContents() != null) {
                annot.SetContents(pdfTextMarkupAnnotation.GetContents());
            }
            if (pdfTextMarkupAnnotation.GetPopup() != null) {
                annot.SetPopup(ConvertPdfPopupToAnnotObject(pdfTextMarkupAnnotation.GetPopup(), pageNumber));
            }
        }

        private static void CreateTextAnnotation(PdfAnnotation pdfAnnotation, AnnotObject annot, int pageNumber) {
            PdfTextAnnotation pdfTextAnnotation = ((PdfTextAnnotation)pdfAnnotation);
            annot.AddAttribute(XfdfConstants.ICON, pdfTextAnnotation.GetIconName());
            annot.AddAttribute(XfdfConstants.STATE, pdfTextAnnotation.GetState());
            annot.AddAttribute(XfdfConstants.STATE_MODEL, pdfTextAnnotation.GetStateModel());
            if (pdfTextAnnotation.GetReplyType() != null) {
                //inreplyTo is required if replyType is present
                annot.AddAttribute(new AttributeObject(XfdfConstants.IN_REPLY_TO, pdfTextAnnotation.GetInReplyTo().GetName
                    ().GetValue()));
                annot.AddAttribute(new AttributeObject(XfdfConstants.REPLY_TYPE, pdfTextAnnotation.GetReplyType().GetValue
                    ()));
            }
            if (pdfTextAnnotation.GetContents() != null) {
                annot.SetContents(pdfTextAnnotation.GetContents());
            }
            if (pdfTextAnnotation.GetPopup() != null) {
                annot.SetPopup(ConvertPdfPopupToAnnotObject(pdfTextAnnotation.GetPopup(), pageNumber));
            }
        }

        private static void CreateCircleAnnotation(PdfAnnotation pdfAnnotation, AnnotObject annot, int pageNumber) {
            PdfCircleAnnotation pdfCircleAnnotation = (PdfCircleAnnotation)pdfAnnotation;
            PdfDictionary bs = pdfCircleAnnotation.GetBorderStyle();
            if (bs != null) {
                AddBorderStyleAttributes(annot, bs.GetAsNumber(PdfName.W), bs.GetAsArray(PdfName.D), bs.GetAsName(PdfName.
                    S));
            }
            if (pdfCircleAnnotation.GetBorderEffect() != null) {
                annot.AddAttribute(XfdfConstants.INTENSITY, pdfCircleAnnotation.GetBorderEffect().GetAsNumber(PdfName.I));
                if (annot.GetAttribute(XfdfConstants.STYLE) == null) {
                    annot.AddAttribute(XfdfConstants.STYLE, XfdfObjectUtils.GetStyleFullValue(pdfCircleAnnotation.GetBorderEffect
                        ().GetAsName(PdfName.S)));
                }
            }
            if (pdfCircleAnnotation.GetInteriorColor() != null && pdfCircleAnnotation.GetInteriorColor().GetColorValue
                () != null) {
                annot.AddAttribute(new AttributeObject(XfdfConstants.INTERIOR_COLOR, XfdfObjectUtils.ConvertColorToString(
                    pdfCircleAnnotation.GetInteriorColor().GetColorValue())));
            }
            if (pdfCircleAnnotation.GetRectangleDifferences() != null) {
                annot.AddAttribute(new AttributeObject(XfdfConstants.FRINGE, XfdfObjectUtils.ConvertFringeToString(pdfCircleAnnotation
                    .GetRectangleDifferences().ToFloatArray())));
            }
            annot.SetContents(pdfAnnotation.GetContents());
            if (pdfCircleAnnotation.GetPopup() != null) {
                annot.SetPopup(ConvertPdfPopupToAnnotObject(pdfCircleAnnotation.GetPopup(), pageNumber));
            }
        }

        private static void CreateSquareAnnotation(PdfAnnotation pdfAnnotation, AnnotObject annot, int pageNumber) {
            PdfSquareAnnotation pdfSquareAnnotation = (PdfSquareAnnotation)pdfAnnotation;
            PdfDictionary bs = pdfSquareAnnotation.GetBorderStyle();
            if (bs != null) {
                AddBorderStyleAttributes(annot, bs.GetAsNumber(PdfName.W), bs.GetAsArray(PdfName.D), bs.GetAsName(PdfName.
                    S));
            }
            if (pdfSquareAnnotation.GetBorderEffect() != null) {
                annot.AddAttribute(XfdfConstants.INTENSITY, pdfSquareAnnotation.GetBorderEffect().GetAsNumber(PdfName.I));
                if (annot.GetAttribute(XfdfConstants.STYLE) == null) {
                    annot.AddAttribute(XfdfConstants.STYLE, XfdfObjectUtils.GetStyleFullValue(pdfSquareAnnotation.GetBorderEffect
                        ().GetAsName(PdfName.S)));
                }
            }
            if (pdfSquareAnnotation.GetInteriorColor() != null && pdfSquareAnnotation.GetInteriorColor().GetColorValue
                () != null) {
                annot.AddAttribute(new AttributeObject(XfdfConstants.INTERIOR_COLOR, XfdfObjectUtils.ConvertColorToString(
                    pdfSquareAnnotation.GetInteriorColor().GetColorValue())));
            }
            if (pdfSquareAnnotation.GetRectangleDifferences() != null) {
                annot.AddAttribute(new AttributeObject(XfdfConstants.FRINGE, XfdfObjectUtils.ConvertFringeToString(pdfSquareAnnotation
                    .GetRectangleDifferences().ToFloatArray())));
            }
            annot.SetContents(pdfAnnotation.GetContents());
            if (pdfSquareAnnotation.GetPopup() != null) {
                annot.SetPopup(ConvertPdfPopupToAnnotObject(pdfSquareAnnotation.GetPopup(), pageNumber));
            }
        }

        private static void CreateStampAnnotation(PdfAnnotation pdfAnnotation, AnnotObject annot, int pageNumber) {
            PdfStampAnnotation pdfStampAnnotation = (PdfStampAnnotation)pdfAnnotation;
            annot.AddAttribute(XfdfConstants.ICON, pdfStampAnnotation.GetIconName());
            if (pdfStampAnnotation.GetRotation() != null) {
                annot.AddAttribute(XfdfConstants.ROTATION, pdfStampAnnotation.GetRotation().IntValue());
            }
            if (pdfStampAnnotation.GetContents() != null) {
                annot.SetContents(pdfStampAnnotation.GetContents());
            }
            if (pdfStampAnnotation.GetPopup() != null) {
                annot.SetPopup(ConvertPdfPopupToAnnotObject(pdfStampAnnotation.GetPopup(), pageNumber));
            }
            if (pdfStampAnnotation.GetAppearanceDictionary() != null) {
                if (pdfAnnotation.GetAppearanceObject(PdfName.N) != null) {
                    annot.SetAppearance(pdfStampAnnotation.GetAppearanceDictionary().Get(PdfName.N).ToString());
                }
                else {
                    if (pdfAnnotation.GetAppearanceObject(PdfName.R) != null) {
                        annot.SetAppearance(pdfStampAnnotation.GetAppearanceDictionary().Get(PdfName.R).ToString());
                    }
                    else {
                        if (pdfAnnotation.GetAppearanceObject(PdfName.D) != null) {
                            annot.SetAppearance(pdfStampAnnotation.GetAppearanceDictionary().Get(PdfName.D).ToString());
                        }
                    }
                }
            }
        }

        private static void CreateFreeTextAnnotation(PdfAnnotation pdfAnnotation, AnnotObject annot) {
            PdfFreeTextAnnotation pdfFreeTextAnnotation = (PdfFreeTextAnnotation)pdfAnnotation;
            PdfDictionary bs = pdfFreeTextAnnotation.GetBorderStyle();
            if (bs != null) {
                AddBorderStyleAttributes(annot, bs.GetAsNumber(PdfName.W), bs.GetAsArray(PdfName.D), bs.GetAsName(PdfName.
                    S));
            }
            if (pdfFreeTextAnnotation.GetRotation() != null) {
                annot.AddAttribute(XfdfConstants.ROTATION, pdfFreeTextAnnotation.GetRotation().IntValue());
            }
            annot.AddAttribute(new AttributeObject(XfdfConstants.JUSTIFICATION, XfdfObjectUtils.ConvertJustificationFromIntegerToString
                ((pdfFreeTextAnnotation.GetJustification()))));
            if (pdfFreeTextAnnotation.GetIntent() != null) {
                annot.AddAttribute(new AttributeObject(XfdfConstants.INTENT, pdfFreeTextAnnotation.GetIntent().GetValue())
                    );
            }
            if (pdfFreeTextAnnotation.GetContents() != null) {
                annot.SetContents(pdfFreeTextAnnotation.GetContents());
            }
            //TODO DEVSIX-3215 add contents-richtext
            if (pdfFreeTextAnnotation.GetDefaultAppearance() != null) {
                annot.SetDefaultAppearance(pdfFreeTextAnnotation.GetDefaultAppearance().GetValue());
            }
            if (pdfFreeTextAnnotation.GetDefaultStyleString() != null) {
                annot.SetDefaultStyle(pdfFreeTextAnnotation.GetDefaultStyleString().GetValue());
            }
        }

        private static void CreateLineAnnotation(PdfAnnotation pdfAnnotation, AnnotObject annot, int pageNumber) {
            PdfLineAnnotation pdfLineAnnotation = (PdfLineAnnotation)pdfAnnotation;
            PdfArray line = pdfLineAnnotation.GetLine();
            if (line != null) {
                annot.AddAttribute(new AttributeObject(XfdfConstants.START, XfdfObjectUtils.ConvertLineStartToString(line.
                    ToFloatArray())));
                annot.AddAttribute(new AttributeObject(XfdfConstants.END, XfdfObjectUtils.ConvertLineEndToString(line.ToFloatArray
                    ())));
            }
            if (pdfLineAnnotation.GetLineEndingStyles() != null) {
                if (pdfLineAnnotation.GetLineEndingStyles().Get(0) != null) {
                    annot.AddAttribute(new AttributeObject(XfdfConstants.HEAD, pdfLineAnnotation.GetLineEndingStyles().Get(0).
                        ToString().Substring(1)));
                }
                if (pdfLineAnnotation.GetLineEndingStyles().Get(1) != null) {
                    annot.AddAttribute(new AttributeObject(XfdfConstants.TAIL, pdfLineAnnotation.GetLineEndingStyles().Get(1).
                        ToString().Substring(1)));
                }
            }
            if (pdfLineAnnotation.GetInteriorColor() != null) {
                annot.AddAttribute(new AttributeObject(XfdfConstants.INTERIOR_COLOR, XfdfObjectUtils.ConvertColorToString(
                    pdfLineAnnotation.GetInteriorColor())));
            }
            annot.AddAttribute(XfdfConstants.LEADER_EXTENDED, pdfLineAnnotation.GetLeaderLineExtension());
            annot.AddAttribute(XfdfConstants.LEADER_LENGTH, pdfLineAnnotation.GetLeaderLineLength());
            annot.AddAttribute(XfdfConstants.CAPTION, pdfLineAnnotation.GetContentsAsCaption());
            annot.AddAttribute(XfdfConstants.INTENT, pdfLineAnnotation.GetIntent());
            annot.AddAttribute(XfdfConstants.LEADER_OFFSET, pdfLineAnnotation.GetLeaderLineOffset());
            annot.AddAttribute(XfdfConstants.CAPTION_STYLE, pdfLineAnnotation.GetCaptionPosition());
            if (pdfLineAnnotation.GetCaptionOffset() != null) {
                annot.AddAttribute(XfdfConstants.CAPTION_OFFSET_H, pdfLineAnnotation.GetCaptionOffset().Get(0));
                annot.AddAttribute(XfdfConstants.CAPTION_OFFSET_V, pdfLineAnnotation.GetCaptionOffset().Get(1));
            }
            else {
                annot.AddAttribute(new AttributeObject(XfdfConstants.CAPTION_OFFSET_H, "0"));
                annot.AddAttribute(new AttributeObject(XfdfConstants.CAPTION_OFFSET_V, "0"));
            }
            PdfDictionary bs = pdfLineAnnotation.GetBorderStyle();
            if (bs != null) {
                AddBorderStyleAttributes(annot, bs.GetAsNumber(PdfName.W), bs.GetAsArray(PdfName.D), bs.GetAsName(PdfName.
                    S));
            }
            annot.SetContents(pdfAnnotation.GetContents());
            if (pdfLineAnnotation.GetPopup() != null) {
                annot.SetPopup(ConvertPdfPopupToAnnotObject(pdfLineAnnotation.GetPopup(), pageNumber));
            }
        }

        private static void CreateLinkAnnotation(PdfAnnotation pdfAnnotation, AnnotObject annot) {
            PdfLinkAnnotation pdfLinkAnnotation = (PdfLinkAnnotation)pdfAnnotation;
            if (pdfLinkAnnotation.GetBorderStyle() != null) {
                annot.AddAttribute(XfdfConstants.STYLE, pdfLinkAnnotation.GetBorderStyle().GetAsString(PdfName.S));
            }
            if (pdfLinkAnnotation.GetHighlightMode() != null) {
                annot.AddAttribute(XfdfConstants.HIGHLIGHT, XfdfObjectUtils.GetHighlightFullValue(pdfLinkAnnotation.GetHighlightMode
                    ()));
            }
            if (pdfLinkAnnotation.GetQuadPoints() != null) {
                annot.AddAttribute(new AttributeObject(XfdfConstants.COORDS, XfdfObjectUtils.ConvertQuadPointsToCoordsString
                    (pdfLinkAnnotation.GetQuadPoints().ToFloatArray())));
            }
            if (pdfLinkAnnotation.GetContents() != null) {
                annot.SetContents(pdfLinkAnnotation.GetContents());
            }
            //in iText pdfLinkAnnotation doesn't have a popup sub-element
            PdfDictionary action = pdfLinkAnnotation.GetAction();
            if (pdfLinkAnnotation.GetAction() != null) {
                PdfName type = action.GetAsName(PdfName.S);
                ActionObject actionObject = new ActionObject(type);
                if (PdfName.URI.Equals(type)) {
                    actionObject.SetUri(action.GetAsString(PdfName.URI));
                    if (action.Get(PdfName.IsMap) != null) {
                        actionObject.SetMap((bool)action.GetAsBool(PdfName.IsMap));
                    }
                }
                annot.SetAction(actionObject);
            }
            PdfArray dest = (PdfArray)pdfLinkAnnotation.GetDestinationObject();
            if (dest != null) {
                CreateDestElement(dest, annot);
            }
            PdfArray border = pdfLinkAnnotation.GetBorder();
            if (border != null) {
                BorderStyleAltObject borderStyleAltObject = new BorderStyleAltObject(border.GetAsNumber(0).FloatValue(), border
                    .GetAsNumber(1).FloatValue(), border.GetAsNumber(2).FloatValue());
                annot.SetBorderStyleAlt(borderStyleAltObject);
            }
        }

        private static void CreateDestElement(PdfArray dest, AnnotObject annot) {
            DestObject destObject = new DestObject();
            PdfName type = dest.GetAsName(1);
            if (PdfName.XYZ.Equals(type)) {
                FitObject xyz = new FitObject(dest.Get(0));
                xyz.SetLeft(dest.GetAsNumber(2).FloatValue()).SetTop(dest.GetAsNumber(3).FloatValue()).SetZoom(dest.GetAsNumber
                    (4).FloatValue());
                destObject.SetXyz(xyz);
            }
            else {
                if (PdfName.Fit.Equals(type)) {
                    FitObject fit = new FitObject(dest.Get(0));
                    destObject.SetFit(fit);
                }
                else {
                    if (PdfName.FitB.Equals(type)) {
                        FitObject fitB = new FitObject(dest.Get(0));
                        destObject.SetFitB(fitB);
                    }
                    else {
                        if (PdfName.FitR.Equals(type)) {
                            FitObject fitR = new FitObject(dest.Get(0));
                            fitR.SetLeft(dest.GetAsNumber(2).FloatValue());
                            fitR.SetBottom(dest.GetAsNumber(3).FloatValue());
                            fitR.SetRight(dest.GetAsNumber(4).FloatValue());
                            fitR.SetTop(dest.GetAsNumber(5).FloatValue());
                            destObject.SetFitR(fitR);
                        }
                        else {
                            if (PdfName.FitH.Equals(type)) {
                                FitObject fitH = new FitObject(dest.Get(0));
                                fitH.SetTop(dest.GetAsNumber(2).FloatValue());
                                destObject.SetFitH(fitH);
                            }
                            else {
                                if (PdfName.FitBH.Equals(type)) {
                                    FitObject fitBH = new FitObject(dest.Get(0));
                                    fitBH.SetTop(dest.GetAsNumber(2).FloatValue());
                                    destObject.SetFitBH(fitBH);
                                }
                                else {
                                    if (PdfName.FitBV.Equals(type)) {
                                        FitObject fitBV = new FitObject(dest.Get(0));
                                        fitBV.SetLeft(dest.GetAsNumber(2).FloatValue());
                                        destObject.SetFitBV(fitBV);
                                    }
                                    else {
                                        if (PdfName.FitV.Equals(type)) {
                                            FitObject fitV = new FitObject(dest.Get(0));
                                            fitV.SetLeft(dest.GetAsNumber(2).FloatValue());
                                            destObject.SetFitV(fitV);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            annot.SetDestination(destObject);
        }

        private static void CreatePolyGeomAnnotation(PdfAnnotation pdfAnnotation, AnnotObject annot, int pageNumber
            ) {
            PdfPolyGeomAnnotation pdfPolyGeomAnnotation = (PdfPolyGeomAnnotation)pdfAnnotation;
            PdfDictionary bs = pdfPolyGeomAnnotation.GetBorderStyle();
            if (bs != null) {
                AddBorderStyleAttributes(annot, bs.GetAsNumber(PdfName.W), bs.GetAsArray(PdfName.D), bs.GetAsName(PdfName.
                    S));
            }
            if (pdfPolyGeomAnnotation.GetBorderEffect() != null) {
                annot.AddAttribute(XfdfConstants.INTENSITY, pdfPolyGeomAnnotation.GetBorderEffect().GetAsNumber(PdfName.I)
                    );
                if (annot.GetAttribute(XfdfConstants.STYLE) == null) {
                    annot.AddAttribute(XfdfConstants.STYLE, XfdfObjectUtils.GetStyleFullValue(pdfPolyGeomAnnotation.GetBorderEffect
                        ().GetAsName(PdfName.S)));
                }
            }
            if (pdfPolyGeomAnnotation.GetInteriorColor() != null) {
                annot.AddAttribute(new AttributeObject(XfdfConstants.INTERIOR_COLOR, XfdfObjectUtils.ConvertColorToString(
                    pdfPolyGeomAnnotation.GetInteriorColor())));
            }
            if (pdfPolyGeomAnnotation.GetIntent() != null) {
                annot.AddAttribute(new AttributeObject(XfdfConstants.INTENT, pdfPolyGeomAnnotation.GetIntent().GetValue())
                    );
            }
            //Head and tail for polyline only
            if (pdfPolyGeomAnnotation.GetLineEndingStyles() != null) {
                if (pdfPolyGeomAnnotation.GetLineEndingStyles().Get(0) != null) {
                    annot.AddAttribute(new AttributeObject(XfdfConstants.HEAD, pdfPolyGeomAnnotation.GetLineEndingStyles().Get
                        (0).ToString().Substring(1)));
                }
                if (pdfPolyGeomAnnotation.GetLineEndingStyles().Get(1) != null) {
                    annot.AddAttribute(new AttributeObject(XfdfConstants.TAIL, pdfPolyGeomAnnotation.GetLineEndingStyles().Get
                        (1).ToString().Substring(1)));
                }
            }
            //in xfdfd: no attributes, inside text string
            annot.SetVertices(XfdfObjectUtils.ConvertVerticesToString(pdfPolyGeomAnnotation.GetVertices().ToFloatArray
                ()));
            annot.SetContents(pdfAnnotation.GetContents());
            if (pdfPolyGeomAnnotation.GetPopup() != null) {
                annot.SetPopup(ConvertPdfPopupToAnnotObject(pdfPolyGeomAnnotation.GetPopup(), pageNumber));
            }
        }

        private static AnnotObject CreateXfdfAnnotation(PdfAnnotation pdfAnnotation, int pageNumber) {
            AnnotObject annot = new AnnotObject();
            annot.SetRef(pdfAnnotation.GetPdfObject().GetIndirectReference());
            annot.AddFdfAttributes(pageNumber);
            if (pdfAnnotation is PdfTextMarkupAnnotation) {
                CreateTextMarkupAnnotation(pdfAnnotation, annot, pageNumber);
            }
            if (pdfAnnotation is PdfTextAnnotation) {
                CreateTextAnnotation(pdfAnnotation, annot, pageNumber);
            }
            if (pdfAnnotation is PdfPopupAnnotation) {
                annot = ConvertPdfPopupToAnnotObject((PdfPopupAnnotation)pdfAnnotation, pageNumber);
            }
            if (pdfAnnotation is PdfCircleAnnotation) {
                CreateCircleAnnotation(pdfAnnotation, annot, pageNumber);
            }
            if (pdfAnnotation is PdfSquareAnnotation) {
                CreateSquareAnnotation(pdfAnnotation, annot, pageNumber);
            }
            if (pdfAnnotation is PdfStampAnnotation) {
                CreateStampAnnotation(pdfAnnotation, annot, pageNumber);
            }
            if (pdfAnnotation is PdfFreeTextAnnotation) {
                CreateFreeTextAnnotation(pdfAnnotation, annot);
            }
            if (pdfAnnotation is PdfLineAnnotation) {
                CreateLineAnnotation(pdfAnnotation, annot, pageNumber);
            }
            if (pdfAnnotation is PdfPolyGeomAnnotation) {
                CreatePolyGeomAnnotation(pdfAnnotation, annot, pageNumber);
            }
            if (pdfAnnotation is PdfLinkAnnotation) {
                CreateLinkAnnotation(pdfAnnotation, annot);
            }
            if (IsSupportedAnnotation(pdfAnnotation)) {
                AddCommonAnnotationAttributes(annot, pdfAnnotation);
                if (pdfAnnotation is PdfMarkupAnnotation) {
                    AddMarkupAnnotationAttributes(annot, (PdfMarkupAnnotation)pdfAnnotation);
                }
            }
            return annot;
        }

        private static AnnotObject ConvertPdfPopupToAnnotObject(PdfPopupAnnotation pdfPopupAnnotation, int pageNumber
            ) {
            AnnotObject annot = new AnnotObject();
            annot.AddFdfAttributes(pageNumber);
            annot.SetName(XfdfConstants.POPUP);
            annot.SetRef(pdfPopupAnnotation.GetPdfObject().GetIndirectReference());
            annot.AddAttribute(XfdfConstants.OPEN, pdfPopupAnnotation.GetOpen());
            return annot;
        }

        private static bool IsSupportedAnnotation(PdfAnnotation pdfAnnotation) {
            return pdfAnnotation is PdfTextMarkupAnnotation || pdfAnnotation is PdfTextAnnotation || pdfAnnotation is 
                PdfCircleAnnotation || pdfAnnotation is PdfSquareAnnotation || pdfAnnotation is PdfStampAnnotation || 
                pdfAnnotation is PdfFreeTextAnnotation || pdfAnnotation is PdfLineAnnotation || pdfAnnotation is PdfPolyGeomAnnotation
                 || pdfAnnotation is PdfLinkAnnotation || pdfAnnotation is PdfPopupAnnotation;
        }
    }
}
