/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 Apryse Group NV
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
using iText.Kernel.Pdf;

namespace iText.Forms.Xfdf {
    internal class XfdfWriter {
        private Stream outputStream;

        private static ILogger logger = ITextLogManager.GetLogger(typeof(iText.Forms.Xfdf.XfdfWriter));

        /// <summary>Creates a XfdfWriter for output stream specified.</summary>
        /// <param name="outputStream">A stream to write xfdf file into.</param>
        internal XfdfWriter(Stream outputStream) {
            this.outputStream = outputStream;
        }

        /// <summary>
        /// Writes data from
        /// <see cref="XfdfObject"/>
        /// into a xfdf data file.
        /// </summary>
        /// <param name="xfdfObject">
        /// 
        /// <see cref="XfdfObject"/>
        /// containing the data.
        /// </param>
        internal virtual void Write(XfdfObject xfdfObject) {
            this.WriteDom(xfdfObject);
        }

        internal static void AddField(FieldObject fieldObject, XmlElement parentElement, XmlDocument document, IList
            <FieldObject> fieldList) {
            IList<FieldObject> childrenFields = FindChildrenFields(fieldObject, fieldList);
            XmlElement field = document.CreateElement("field");
            field.SetAttribute("name", fieldObject.GetName());
            if (!childrenFields.IsEmpty()) {
                foreach (FieldObject childField in childrenFields) {
                    AddField(childField, field, document, fieldList);
                }
            }
            else {
                if (fieldObject.GetValue() != null && !String.IsNullOrEmpty(fieldObject.GetValue())) {
                    XmlElement value = document.CreateElement("value");
                    value.InnerText = fieldObject.GetValue();
                    field.AppendChild(value);
                }
                else {
                    logger.LogInformation(XfdfConstants.EMPTY_FIELD_VALUE_ELEMENT);
                }
            }
            parentElement.AppendChild(field);
        }

        private void WriteDom(XfdfObject xfdfObject) {
            XmlDocument document = XfdfFileUtils.CreateNewXfdfDocument();
            // root xfdf element
            XmlElement root = document.CreateElement("xfdf");
            document.AppendChild(root);
            //write fields
            if (xfdfObject.GetFields() != null && xfdfObject.GetFields().GetFieldList() != null && !xfdfObject.GetFields
                ().GetFieldList().IsEmpty()) {
                XmlElement fields = document.CreateElement("fields");
                root.AppendChild(fields);
                IList<FieldObject> fieldList = xfdfObject.GetFields().GetFieldList();
                foreach (FieldObject fieldObject in fieldList) {
                    if (fieldObject.GetParent() == null) {
                        AddField(fieldObject, fields, document, fieldList);
                    }
                }
            }
            //write annots
            if (xfdfObject.GetAnnots() != null && xfdfObject.GetAnnots().GetAnnotsList() != null && !xfdfObject.GetAnnots
                ().GetAnnotsList().IsEmpty()) {
                XmlElement annots = document.CreateElement("annots");
                root.AppendChild(annots);
                foreach (AnnotObject annotObject in xfdfObject.GetAnnots().GetAnnotsList()) {
                    AddAnnot(annotObject, annots, document);
                }
            }
            //write f
            if (xfdfObject.GetF() != null) {
                XmlElement f = document.CreateElement("f");
                AddFAttributes(xfdfObject.GetF(), f);
                root.AppendChild(f);
            }
            //write ids
            if (xfdfObject.GetIds() != null) {
                XmlElement ids = document.CreateElement("ids");
                AddIdsAttributes(xfdfObject.GetIds(), ids);
                root.AppendChild(ids);
            }
            // create the xml file
            //transform the DOM Object to an XML File
            XfdfFileUtils.SaveXfdfDocumentToFile(document, this.outputStream);
        }

        private static void AddIdsAttributes(IdsObject idsObject, XmlElement ids) {
            if (idsObject.GetOriginal() != null) {
                ids.SetAttribute("original", idsObject.GetOriginal());
            }
            if (idsObject.GetModified() != null) {
                ids.SetAttribute("modified", idsObject.GetModified());
            }
        }

        private static void AddFAttributes(FObject fObject, XmlElement f) {
            if (fObject.GetHref() != null) {
                f.SetAttribute("href", fObject.GetHref());
            }
        }

        private static IList<FieldObject> FindChildrenFields(FieldObject field, IList<FieldObject> fieldList) {
            IList<FieldObject> childrenFields = new List<FieldObject>();
            foreach (FieldObject currentField in fieldList) {
                if (currentField.GetParent() != null && currentField.GetParent().GetName().EqualsIgnoreCase(field.GetName(
                    ))) {
                    childrenFields.Add(currentField);
                }
            }
            return childrenFields;
        }

        private static void AddAnnot(AnnotObject annotObject, XmlElement annots, XmlDocument document) {
            if (annotObject.GetName() == null) {
                return;
            }
            XmlElement annot = document.CreateElement(annotObject.GetName());
            foreach (AttributeObject attr in annotObject.GetAttributes()) {
                annot.SetAttribute(attr.GetName(), attr.GetValue());
            }
            if (annotObject.GetPopup() != null) {
                XmlElement popup = document.CreateElement("popup");
                AddPopup(annotObject.GetPopup(), popup, annot);
            }
            if (annotObject.GetContents() != null) {
                XmlElement contents = document.CreateElement("contents");
                contents.InnerText = annotObject.GetContents().ToString().Replace('\r', '\n');
                annot.AppendChild(contents);
            }
            if (annotObject.GetVertices() != null) {
                XmlElement contents = document.CreateElement("vertices");
                contents.InnerText = annotObject.GetVertices();
                annot.AppendChild(contents);
            }
            if (annotObject.GetAppearance() != null) {
                XmlElement appearance = document.CreateElement("appearance");
                appearance.InnerText = annotObject.GetAppearance();
                annot.AppendChild(appearance);
            }
            if (annotObject.GetContentsRichText() != null) {
                // TODO: DEVSIX-7600 - add tests for this code. contentsRichText#setTextContent might be wrong here.
                XmlElement contentsRichText = document.CreateElement("contents-richtext");
                contentsRichText.InnerText = annotObject.GetContentsRichText().GetValue();
                annot.AppendChild(contentsRichText);
            }
            if (XfdfConstants.LINK.EqualsIgnoreCase(annotObject.GetName())) {
                if (annotObject.GetDestination() != null) {
                    AddDest(annotObject.GetDestination(), annot, document);
                }
                else {
                    if (annotObject.GetAction() != null) {
                        XmlElement onActivation = document.CreateElement(XfdfConstants.ON_ACTIVATION);
                        AddActionObject(annotObject.GetAction(), onActivation, document);
                        annot.AppendChild(onActivation);
                    }
                    else {
                        logger.LogError("Dest and OnActivation elements are both missing");
                    }
                }
                if (annotObject.GetBorderStyleAlt() != null) {
                    AddBorderStyleAlt(annotObject.GetBorderStyleAlt(), annot, document);
                }
            }
            if (XfdfConstants.FREETEXT.EqualsIgnoreCase(annotObject.GetName())) {
                String defaultAppearanceString = annotObject.GetDefaultAppearance();
                if (defaultAppearanceString != null) {
                    XmlElement defaultAppearance = document.CreateElement(XfdfConstants.DEFAULT_APPEARANCE);
                    defaultAppearance.InnerText = defaultAppearanceString;
                    annot.AppendChild(defaultAppearance);
                }
                String defaultStyleString = annotObject.GetDefaultStyle();
                if (defaultStyleString != null) {
                    XmlElement defaultStyle = document.CreateElement(XfdfConstants.DEFAULT_STYLE);
                    defaultStyle.InnerText = defaultStyleString;
                    annot.AppendChild(defaultStyle);
                }
            }
            annots.AppendChild(annot);
        }

        private static void AddBorderStyleAlt(BorderStyleAltObject borderStyleAltObject, XmlElement annot, XmlDocument
             document) {
            //BorderStyle alt contains Border style encoded in the format specified in the border style attributes as content
            //has attributes
            XmlElement borderStyleAlt = document.CreateElement(XfdfConstants.BORDER_STYLE_ALT);
            //required attributes
            borderStyleAlt.SetAttribute(XfdfConstants.H_CORNER_RADIUS, XfdfObjectUtils.ConvertFloatToString(borderStyleAltObject
                .GetHCornerRadius()));
            borderStyleAlt.SetAttribute(XfdfConstants.V_CORNER_RADIUS, XfdfObjectUtils.ConvertFloatToString(borderStyleAltObject
                .GetVCornerRadius()));
            borderStyleAlt.SetAttribute(XfdfConstants.WIDTH_CAPITAL, XfdfObjectUtils.ConvertFloatToString(borderStyleAltObject
                .GetWidth()));
            //optional attribute
            if (borderStyleAltObject.GetDashPattern() != null) {
                //TODO DEVSIX-4028 add real conversion from PdfArray (PdfName.D in Border dictionary) to String
                borderStyleAlt.SetAttribute(XfdfConstants.DASH_PATTERN, JavaUtil.ArraysToString(borderStyleAltObject.GetDashPattern
                    ()));
            }
            if (borderStyleAltObject.GetContent() != null) {
                borderStyleAlt.InnerText = borderStyleAltObject.GetContent();
            }
            annot.AppendChild(borderStyleAlt);
        }

        private static void AddXYZ(FitObject xyzObject, XmlElement dest, XmlDocument document) {
            XmlElement xyz = document.CreateElement(XfdfConstants.XYZ_CAPITAL);
            //all required
            xyz.SetAttribute(XfdfConstants.PAGE_CAPITAL, xyzObject.GetPage().ToString());
            xyz.SetAttribute(XfdfConstants.LEFT, XfdfObjectUtils.ConvertFloatToString(xyzObject.GetLeft()));
            xyz.SetAttribute(XfdfConstants.BOTTOM, XfdfObjectUtils.ConvertFloatToString(xyzObject.GetBottom()));
            xyz.SetAttribute(XfdfConstants.RIGHT, XfdfObjectUtils.ConvertFloatToString(xyzObject.GetRight()));
            xyz.SetAttribute(XfdfConstants.TOP, XfdfObjectUtils.ConvertFloatToString(xyzObject.GetTop()));
            dest.AppendChild(xyz);
        }

        private static void AddFit(FitObject fitObject, XmlElement dest, XmlDocument document) {
            XmlElement fit = document.CreateElement(XfdfConstants.FIT);
            //required
            fit.SetAttribute(XfdfConstants.PAGE_CAPITAL, fitObject.GetPage().ToString());
            dest.AppendChild(fit);
        }

        private static void AddFitB(FitObject fitBObject, XmlElement dest, XmlDocument document) {
            XmlElement fitB = document.CreateElement(XfdfConstants.FIT_B);
            //required
            fitB.SetAttribute(XfdfConstants.PAGE_CAPITAL, fitBObject.GetPage().ToString());
            dest.AppendChild(fitB);
        }

        private static void AddFitBH(FitObject fitBHObject, XmlElement dest, XmlDocument document) {
            XmlElement fitBH = document.CreateElement(XfdfConstants.FIT_BH);
            //all required
            fitBH.SetAttribute(XfdfConstants.PAGE_CAPITAL, fitBHObject.GetPage().ToString());
            fitBH.SetAttribute(XfdfConstants.TOP, XfdfObjectUtils.ConvertFloatToString(fitBHObject.GetTop()));
            dest.AppendChild(fitBH);
        }

        private static void AddFitBV(FitObject fitBVObject, XmlElement dest, XmlDocument document) {
            XmlElement fitBV = document.CreateElement(XfdfConstants.FIT_BV);
            //all required
            fitBV.SetAttribute(XfdfConstants.PAGE_CAPITAL, fitBVObject.GetPage().ToString());
            fitBV.SetAttribute(XfdfConstants.LEFT, XfdfObjectUtils.ConvertFloatToString(fitBVObject.GetLeft()));
            dest.AppendChild(fitBV);
        }

        private static void AddFitH(FitObject fitHObject, XmlElement dest, XmlDocument document) {
            XmlElement fitH = document.CreateElement(XfdfConstants.FIT_H);
            //all required
            fitH.SetAttribute(XfdfConstants.PAGE_CAPITAL, fitHObject.GetPage().ToString());
            fitH.SetAttribute(XfdfConstants.TOP, XfdfObjectUtils.ConvertFloatToString(fitHObject.GetTop()));
            dest.AppendChild(fitH);
        }

        private static void AddFitR(FitObject fitRObject, XmlElement dest, XmlDocument document) {
            XmlElement fitR = document.CreateElement(XfdfConstants.FIT_R);
            //all required
            fitR.SetAttribute(XfdfConstants.PAGE_CAPITAL, fitRObject.GetPage().ToString());
            fitR.SetAttribute(XfdfConstants.LEFT, XfdfObjectUtils.ConvertFloatToString(fitRObject.GetLeft()));
            fitR.SetAttribute(XfdfConstants.BOTTOM, XfdfObjectUtils.ConvertFloatToString(fitRObject.GetBottom()));
            fitR.SetAttribute(XfdfConstants.RIGHT, XfdfObjectUtils.ConvertFloatToString(fitRObject.GetRight()));
            fitR.SetAttribute(XfdfConstants.TOP, XfdfObjectUtils.ConvertFloatToString(fitRObject.GetTop()));
            dest.AppendChild(fitR);
        }

        private static void AddFitV(FitObject fitVObject, XmlElement dest, XmlDocument document) {
            XmlElement fitV = document.CreateElement(XfdfConstants.FIT_V);
            //all required
            fitV.SetAttribute(XfdfConstants.PAGE_CAPITAL, fitVObject.GetPage().ToString());
            fitV.SetAttribute(XfdfConstants.LEFT, XfdfObjectUtils.ConvertFloatToString(fitVObject.GetLeft()));
            dest.AppendChild(fitV);
        }

        private static void AddDest(DestObject destObject, XmlElement annot, XmlDocument document) {
            XmlElement dest = document.CreateElement(XfdfConstants.DEST);
            if (destObject.GetName() != null) {
                XmlElement named = document.CreateElement(XfdfConstants.NAMED);
                named.SetAttribute(XfdfConstants.NAME, destObject.GetName());
                dest.AppendChild(named);
            }
            else {
                if (destObject.GetXyz() != null) {
                    AddXYZ(destObject.GetXyz(), dest, document);
                }
                else {
                    if (destObject.GetFit() != null) {
                        AddFit(destObject.GetFit(), dest, document);
                    }
                    else {
                        if (destObject.GetFitB() != null) {
                            AddFitB(destObject.GetFitB(), dest, document);
                        }
                        else {
                            if (destObject.GetFitBH() != null) {
                                AddFitBH(destObject.GetFitBH(), dest, document);
                            }
                            else {
                                if (destObject.GetFitBV() != null) {
                                    AddFitBV(destObject.GetFitBV(), dest, document);
                                }
                                else {
                                    if (destObject.GetFitH() != null) {
                                        AddFitH(destObject.GetFitH(), dest, document);
                                    }
                                    else {
                                        if (destObject.GetFitR() != null) {
                                            AddFitR(destObject.GetFitR(), dest, document);
                                        }
                                        else {
                                            if (destObject.GetFitV() != null) {
                                                AddFitV(destObject.GetFitV(), dest, document);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            annot.AppendChild(dest);
        }

        private static void AddActionObject(ActionObject actionObject, XmlElement onActivation, XmlDocument document
            ) {
            //no attributes, children elements URI|Launch|GoTo|GoToR|Named
            XmlElement action = document.CreateElement(XfdfConstants.ACTION);
            if (actionObject.GetUri() != null) {
                XmlElement uri = document.CreateElement(XfdfConstants.URI);
                //no children
                //required attribute Name, optional attribute IsMap
                uri.SetAttribute(XfdfConstants.NAME_CAPITAL, actionObject.GetUri().GetValue());
                if (actionObject.IsMap()) {
                    uri.SetAttribute(XfdfConstants.IS_MAP, "true");
                }
                else {
                    uri.SetAttribute(XfdfConstants.IS_MAP, "false");
                }
                action.AppendChild(uri);
            }
            else {
                if (PdfName.GoTo.Equals(actionObject.GetType())) {
                    XmlElement goTo = document.CreateElement(XfdfConstants.GO_TO);
                    AddDest(actionObject.GetDestination(), goTo, document);
                    action.AppendChild(goTo);
                }
                else {
                    if (PdfName.GoToR.Equals(actionObject.GetType())) {
                        XmlElement goToR = document.CreateElement(XfdfConstants.GO_TO_R);
                        if (actionObject.GetDestination() != null) {
                            AddDest(actionObject.GetDestination(), goToR, document);
                        }
                        else {
                            if (actionObject.GetFileOriginalName() != null) {
                                XmlElement file = document.CreateElement(XfdfConstants.FILE);
                                file.SetAttribute(XfdfConstants.ORIGINAL_NAME, actionObject.GetFileOriginalName());
                                goToR.AppendChild(file);
                            }
                            else {
                                logger.LogError("Dest or File elements are missing.");
                            }
                        }
                        action.AppendChild(goToR);
                    }
                    else {
                        if (PdfName.Named.Equals(actionObject.GetType())) {
                            XmlElement named = document.CreateElement(XfdfConstants.NAMED);
                            named.SetAttribute(XfdfConstants.NAME_CAPITAL, actionObject.GetNameAction().GetValue());
                            action.AppendChild(named);
                        }
                        else {
                            if (PdfName.Launch.Equals(actionObject.GetType())) {
                                XmlElement launch = document.CreateElement(XfdfConstants.LAUNCH);
                                if (actionObject.GetFileOriginalName() != null) {
                                    XmlElement file = document.CreateElement(XfdfConstants.FILE);
                                    file.SetAttribute(XfdfConstants.ORIGINAL_NAME, actionObject.GetFileOriginalName());
                                    launch.AppendChild(file);
                                }
                                else {
                                    logger.LogError("File element is missing");
                                }
                                if (actionObject.IsNewWindow()) {
                                    launch.SetAttribute(XfdfConstants.NEW_WINDOW, "true");
                                }
                                action.AppendChild(launch);
                            }
                        }
                    }
                }
            }
            onActivation.AppendChild(action);
        }

        private static void AddPopup(AnnotObject popupAnnotObject, XmlElement popup, XmlElement annot) {
            foreach (AttributeObject attr in popupAnnotObject.GetAttributes()) {
                popup.SetAttribute(attr.GetName(), attr.GetValue());
            }
            annot.AppendChild(popup);
        }
    }
}
