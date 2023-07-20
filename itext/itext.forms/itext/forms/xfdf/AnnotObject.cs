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
using iText.Forms.Exceptions;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;

namespace iText.Forms.Xfdf {
    /// <summary>Represents annotation, a child element of annots tag in Xfdf document structure.</summary>
    /// <remarks>Represents annotation, a child element of annots tag in Xfdf document structure. For more details see part 6.4 in Xfdf specification.
    ///     </remarks>
    public class AnnotObject {
        /// <summary>Represents the type of annotation.</summary>
        /// <remarks>
        /// Represents the type of annotation. Possible values:
        /// <see cref="XfdfConstants.CARET"/>
        /// ,
        /// <see cref="XfdfConstants.CIRCLE"/>
        /// ,
        /// <see cref="XfdfConstants.FILEATTACHMENT"/>
        /// ,
        /// <see cref="XfdfConstants.FREETEXT"/>
        /// ,
        /// <see cref="XfdfConstants.HIGHLIGHT"/>
        /// ,
        /// <see cref="XfdfConstants.INK"/>
        /// ,
        /// <see cref="XfdfConstants.LINE"/>
        /// ,
        /// <see cref="XfdfConstants.POLYGON"/>
        /// ,
        /// <see cref="XfdfConstants.POLYLINE"/>
        /// ,
        /// <see cref="XfdfConstants.SOUND"/>
        /// ,
        /// <see cref="XfdfConstants.SQUARE"/>
        /// ,
        /// <see cref="XfdfConstants.SQUIGGLY"/>
        /// ,
        /// <see cref="XfdfConstants.STAMP"/>
        /// ,
        /// <see cref="XfdfConstants.STRIKEOUT"/>
        /// ,
        /// <see cref="XfdfConstants.TEXT"/>
        /// ,
        /// <see cref="XfdfConstants.UNDERLINE"/>.
        /// </remarks>
        private String name;

        /// <summary>Represents a list of attributes of the annotation.</summary>
        private IList<AttributeObject> attributes;

        /// <summary>Represents contents tag in Xfdf document structure.</summary>
        /// <remarks>
        /// Represents contents tag in Xfdf document structure. Is a child of caret, circle, fileattachment, freetext,
        /// highlight, ink, line, polygon, polyline, sound, square, squiggly, stamp, strikeout, text, and
        /// underline elements.
        /// Corresponds to Contents key in annotation dictionary.
        /// Content model: a string or a rich text string.
        /// For more details see paragraph 6.5.4 in Xfdf document specification.
        /// </remarks>
        private PdfString contents;

        /// <summary>Represents contents-richtext tag in Xfdf document structure.</summary>
        /// <remarks>
        /// Represents contents-richtext tag in Xfdf document structure. Is a child of caret, circle, fileattachment, freetext,
        /// highlight, ink, line, polygon, polyline, sound, square, squiggly, stamp, strikeout, text, and
        /// underline elements.
        /// Corresponds to RC key in annotation dictionary.
        /// Content model: text string.
        /// For more details see paragraph 6.5.5 in Xfdf document specification.
        /// </remarks>
        private PdfString contentsRichText;

        /// <summary>A boolean, indicating if annotation has inner popup element.</summary>
        private bool hasPopup;

        /// <summary>Represents a popup annotation, an inner element of the annotation element.</summary>
        private iText.Forms.Xfdf.AnnotObject popup;

        /// <summary>Represents Action element, a child of OnActivation element of the link annotation.</summary>
        /// <remarks>
        /// Represents Action element, a child of OnActivation element of the link annotation.
        /// Corresponds to the A key in the link annotation dictionary.
        /// </remarks>
        private ActionObject action;

        /// <summary>Represents Dest element, a child element of link, GoTo, GoToR elements.</summary>
        /// <remarks>
        /// Represents Dest element, a child element of link, GoTo, GoToR elements.
        /// Corresponds to the Dest key in link annotation dictionary.
        /// </remarks>
        private DestObject destination;

        /// <summary>Represents appearance element,  a child element of stamp element.</summary>
        /// <remarks>
        /// Represents appearance element,  a child element of stamp element.
        /// Corresponds to the AP key in the annotation dictionary.
        /// Content model: Base64 encoded string.
        /// For more details see paragraph 6.5.1 in Xfdf document specification.
        /// </remarks>
        private String appearance;

        /// <summary>Represents the defaultappearance element, a child of the caret and freetext elements.</summary>
        /// <remarks>
        /// Represents the defaultappearance element, a child of the caret and freetext elements.
        /// Corresponds to the DA key in the free text annotation dictionary.
        /// Content model: text string.
        /// For more details see paragraph 6.5.7 in Xfdf document specification.
        /// </remarks>
        private String defaultAppearance;

        /// <summary>Represents defaultstyle element, a child of the freetext element.</summary>
        /// <remarks>
        /// Represents defaultstyle element, a child of the freetext element.
        /// Corresponds to the DS key in the free text annotation dictionary.
        /// Content model : a text string.
        /// For more details see paragraph 6.5.9 in Xfdf document specification.
        /// </remarks>
        private String defaultStyle;

        /// <summary>Represents the BorderStyleAlt element, a child of the link element.</summary>
        /// <remarks>
        /// Represents the BorderStyleAlt element, a child of the link element.
        /// Corresponds to the Border key in the common annotation dictionary.
        /// For more details see paragraph 6.5.3 in Xfdf document specification.
        /// </remarks>
        private BorderStyleAltObject borderStyleAlt;

        /// <summary>Represents the string, containing vertices element, a child of the polygon and polyline elements.
        ///     </summary>
        /// <remarks>
        /// Represents the string, containing vertices element, a child of the polygon and polyline elements.
        /// Corresponds to the Vertices key in the polygon or polyline annotation dictionary.
        /// For more details see paragraph 6.5.31 in Xfdf document specification.
        /// </remarks>
        private String vertices;

        /// <summary>
        /// The reference to the source
        /// <see cref="iText.Kernel.Pdf.Annot.PdfAnnotation"/>.
        /// </summary>
        /// <remarks>
        /// The reference to the source
        /// <see cref="iText.Kernel.Pdf.Annot.PdfAnnotation"/>
        /// . Used for attaching popups in case of reading data from pdf file.
        /// </remarks>
        private PdfIndirectReference @ref;

        public AnnotObject() {
            this.attributes = new List<AttributeObject>();
        }

        /// <summary>Gets the string value of the type of annotation.</summary>
        /// <remarks>
        /// Gets the string value of the type of annotation. Possible values:
        /// <see cref="XfdfConstants.CARET"/>
        /// ,
        /// <see cref="XfdfConstants.CIRCLE"/>
        /// ,
        /// <see cref="XfdfConstants.FILEATTACHMENT"/>
        /// ,
        /// <see cref="XfdfConstants.FREETEXT"/>
        /// ,
        /// <see cref="XfdfConstants.HIGHLIGHT"/>
        /// ,
        /// <see cref="XfdfConstants.INK"/>
        /// ,
        /// <see cref="XfdfConstants.LINE"/>
        /// ,
        /// <see cref="XfdfConstants.POLYGON"/>
        /// ,
        /// <see cref="XfdfConstants.POLYLINE"/>
        /// ,
        /// <see cref="XfdfConstants.SOUND"/>
        /// ,
        /// <see cref="XfdfConstants.SQUARE"/>
        /// ,
        /// <see cref="XfdfConstants.SQUIGGLY"/>
        /// ,
        /// <see cref="XfdfConstants.STAMP"/>
        /// ,
        /// <see cref="XfdfConstants.STRIKEOUT"/>
        /// ,
        /// <see cref="XfdfConstants.TEXT"/>
        /// ,
        /// <see cref="XfdfConstants.UNDERLINE"/>.
        /// </remarks>
        /// <returns>
        /// 
        /// <see cref="System.String"/>
        /// value of the type of annotation
        /// </returns>
        public virtual String GetName() {
            return name;
        }

        /// <summary>Sets the string value of the type of annotation.</summary>
        /// <remarks>
        /// Sets the string value of the type of annotation. Possible values:
        /// <see cref="XfdfConstants.CARET"/>
        /// ,
        /// <see cref="XfdfConstants.CIRCLE"/>
        /// ,
        /// <see cref="XfdfConstants.FILEATTACHMENT"/>
        /// ,
        /// <see cref="XfdfConstants.FREETEXT"/>
        /// ,
        /// <see cref="XfdfConstants.HIGHLIGHT"/>
        /// ,
        /// <see cref="XfdfConstants.INK"/>
        /// ,
        /// <see cref="XfdfConstants.LINE"/>
        /// ,
        /// <see cref="XfdfConstants.POLYGON"/>
        /// ,
        /// <see cref="XfdfConstants.POLYLINE"/>
        /// ,
        /// <see cref="XfdfConstants.SOUND"/>
        /// ,
        /// <see cref="XfdfConstants.SQUARE"/>
        /// ,
        /// <see cref="XfdfConstants.SQUIGGLY"/>
        /// ,
        /// <see cref="XfdfConstants.STAMP"/>
        /// ,
        /// <see cref="XfdfConstants.STRIKEOUT"/>
        /// ,
        /// <see cref="XfdfConstants.TEXT"/>
        /// ,
        /// <see cref="XfdfConstants.UNDERLINE"/>.
        /// </remarks>
        /// <param name="name">
        /// 
        /// <see cref="System.String"/>
        /// value of the type of annotation
        /// </param>
        /// <returns>
        /// 
        /// <see cref="AnnotObject">annotation object</see>
        /// with set name
        /// </returns>
        public virtual iText.Forms.Xfdf.AnnotObject SetName(String name) {
            this.name = name;
            return this;
        }

        /// <summary>Gets a list of all attributes of the annotation.</summary>
        /// <returns>
        /// 
        /// <see cref="System.Collections.IList{E}">list</see>
        /// containing all
        /// <see cref="AttributeObject">attribute objects</see>
        /// of the annotation
        /// </returns>
        public virtual IList<AttributeObject> GetAttributes() {
            return attributes;
        }

        /// <summary>Finds the attribute by name in attributes list.</summary>
        /// <param name="name">The name of the attribute to look for.</param>
        /// <returns>
        /// 
        /// <see cref="AttributeObject"/>
        /// with the given name, or null, if no object with this name was found.
        /// </returns>
        public virtual AttributeObject GetAttribute(String name) {
            foreach (AttributeObject attr in attributes) {
                if (attr.GetName().Equals(name)) {
                    return attr;
                }
            }
            return null;
        }

        /// <summary>Finds the attribute by name in attributes list and return its string value.</summary>
        /// <param name="name">The name of the attribute to look for.</param>
        /// <returns>
        /// the value of the
        /// <see cref="AttributeObject"/>
        /// with the given name, or null, if no object with this name was found.
        /// </returns>
        public virtual String GetAttributeValue(String name) {
            foreach (AttributeObject attr in attributes) {
                if (attr.GetName().Equals(name)) {
                    return attr.GetValue();
                }
            }
            return null;
        }

        /// <summary>Gets the popup annotation, an inner element of the annotation element.</summary>
        /// <returns>
        /// 
        /// <see cref="AnnotObject"/>
        /// representing the inner popup annotation
        /// </returns>
        public virtual iText.Forms.Xfdf.AnnotObject GetPopup() {
            return popup;
        }

        /// <summary>Sets the popup annotation, an inner element of the annotation element.</summary>
        /// <param name="popup">
        /// 
        /// <see cref="AnnotObject">annotation object</see>
        /// representing inner popup annotation
        /// </param>
        /// <returns>
        /// current
        /// <see cref="AnnotObject">annotation object</see>
        /// </returns>
        public virtual iText.Forms.Xfdf.AnnotObject SetPopup(iText.Forms.Xfdf.AnnotObject popup) {
            this.popup = popup;
            return this;
        }

        /// <summary>Gets the boolean, indicating if annotation has an inner popup element.</summary>
        /// <returns>true if annotation has an inner popup element, false otherwise</returns>
        public virtual bool IsHasPopup() {
            return hasPopup;
        }

        /// <summary>Sets the boolean, indicating if annotation has inner popup element.</summary>
        /// <param name="hasPopup">a boolean indicating if annotation has inner popup element</param>
        /// <returns>
        /// current
        /// <see cref="AnnotObject">annotation object</see>
        /// </returns>
        public virtual iText.Forms.Xfdf.AnnotObject SetHasPopup(bool hasPopup) {
            this.hasPopup = hasPopup;
            return this;
        }

        /// <summary>Gets the string value of contents tag in Xfdf document structure.</summary>
        /// <remarks>
        /// Gets the string value of contents tag in Xfdf document structure. Contents is a child of caret, circle,
        /// fileattachment, freetext, highlight, ink, line, polygon, polyline, sound, square, squiggly, stamp, strikeout,
        /// text, and underline elements.
        /// Corresponds to Contents key in annotation dictionary.
        /// Content model: a string or a rich text string.
        /// For more details see paragraph 6.5.4 in Xfdf document specification.
        /// </remarks>
        /// <returns>
        /// 
        /// <see cref="iText.Kernel.Pdf.PdfString"/>
        /// value of inner contents element of current annotation object
        /// </returns>
        public virtual PdfString GetContents() {
            return contents;
        }

        /// <summary>Sets the string value of contents tag in Xfdf document structure.</summary>
        /// <param name="contents">
        /// 
        /// <see cref="iText.Kernel.Pdf.PdfString">string</see>
        /// value of inner contents element
        /// </param>
        /// <returns>
        /// current
        /// <see cref="AnnotObject">annotation object</see>
        /// </returns>
        public virtual iText.Forms.Xfdf.AnnotObject SetContents(PdfString contents) {
            this.contents = contents;
            return this;
        }

        /// <summary>Gets the string value of contents-richtext tag in Xfdf document structure.</summary>
        /// <remarks>
        /// Gets the string value of contents-richtext tag in Xfdf document structure. It is a child of caret, circle, fileattachment,
        /// freetext, highlight, ink, line, polygon, polyline, sound, square, squiggly, stamp, strikeout, text, and
        /// underline elements.
        /// Corresponds to RC key in annotation dictionary.
        /// Content model: text string.
        /// For more details see paragraph 6.5.5 in Xfdf document specification.
        /// </remarks>
        /// <returns>
        /// 
        /// <see cref="iText.Kernel.Pdf.PdfString"/>
        /// value of inner contents-richtext element of current annotation object
        /// </returns>
        public virtual PdfString GetContentsRichText() {
            return contentsRichText;
        }

        /// <summary>Sets the string value of contents-richtext tag in xfdf document structure.</summary>
        /// <param name="contentsRichRext">
        /// 
        /// <see cref="iText.Kernel.Pdf.PdfString">rich text string</see>
        /// value of inner contents-richtext element
        /// </param>
        /// <returns>
        /// current
        /// <see cref="AnnotObject">annotation object</see>
        /// </returns>
        public virtual iText.Forms.Xfdf.AnnotObject SetContentsRichText(PdfString contentsRichRext) {
            this.contentsRichText = contentsRichRext;
            return this;
        }

        /// <summary>Gets Action element, a child of OnActivation element of the link annotation.</summary>
        /// <remarks>
        /// Gets Action element, a child of OnActivation element of the link annotation.
        /// Corresponds to the A key in the link annotation dictionary.
        /// </remarks>
        /// <returns>
        /// inner
        /// <see cref="ActionObject">action object</see>
        /// of annotation object
        /// </returns>
        public virtual ActionObject GetAction() {
            return action;
        }

        /// <summary>Sets Action element, a child of OnActivation element of the link annotation.</summary>
        /// <remarks>
        /// Sets Action element, a child of OnActivation element of the link annotation.
        /// Corresponds to the A key in the link annotation dictionary.
        /// </remarks>
        /// <param name="action">
        /// 
        /// <see cref="ActionObject">action object</see>
        /// , an inner element of annotation object
        /// </param>
        /// <returns>
        /// current
        /// <see cref="AnnotObject">annotation object</see>
        /// </returns>
        public virtual iText.Forms.Xfdf.AnnotObject SetAction(ActionObject action) {
            this.action = action;
            return this;
        }

        /// <summary>
        /// Adds new
        /// <see cref="AttributeObject"/>
        /// to the list of annotation attributes.
        /// </summary>
        /// <param name="attr">attribute to be added.</param>
        public virtual void AddAttribute(AttributeObject attr) {
            attributes.Add(attr);
        }

        /// <summary>Adds new attribute with given name and boolean value converted to string.</summary>
        internal virtual void AddAttribute(String name, bool value) {
            String valueString = value ? "yes" : "no";
            attributes.Add(new AttributeObject(name, valueString));
        }

        internal virtual void AddAttribute(String name, float value) {
            attributes.Add(new AttributeObject(name, XfdfObjectUtils.ConvertFloatToString(value)));
        }

        internal virtual void AddAttribute(String name, Rectangle value) {
            String stringValue = XfdfObjectUtils.ConvertRectToString(value);
            attributes.Add(new AttributeObject(name, stringValue));
        }

        /// <summary>Adds new attribute by given name and value.</summary>
        /// <remarks>Adds new attribute by given name and value. If required attribute is present, value of the attribute can't be null.
        ///     </remarks>
        /// <param name="name">
        /// 
        /// <see cref="System.String"/>
        /// attribute name
        /// </param>
        /// <param name="valueObject">
        /// 
        /// <see cref="iText.Kernel.Pdf.PdfObject"/>
        /// attribute value
        /// </param>
        /// <param name="required">boolean indicating if the attribute is required</param>
        internal virtual void AddAttribute(String name, PdfObject valueObject, bool required) {
            if (valueObject == null) {
                if (required) {
                    throw new AttributeNotFoundException(name);
                }
                return;
            }
            String valueString = null;
            if (valueObject.GetObjectType() == PdfObject.BOOLEAN) {
                valueString = ((PdfBoolean)(valueObject)).GetValue() ? "yes" : "no";
            }
            else {
                if (valueObject.GetObjectType() == PdfObject.NAME) {
                    valueString = ((PdfName)(valueObject)).GetValue();
                }
                else {
                    if (valueObject.GetObjectType() == PdfObject.NUMBER) {
                        valueString = XfdfObjectUtils.ConvertFloatToString((float)((PdfNumber)(valueObject)).GetValue());
                    }
                    else {
                        if (valueObject.GetObjectType() == PdfObject.STRING) {
                            valueString = ((PdfString)(valueObject)).GetValue();
                        }
                    }
                }
            }
            attributes.Add(new AttributeObject(name, valueString));
        }

        internal virtual void AddAttribute(String name, PdfObject valueObject) {
            AddAttribute(name, valueObject, false);
        }

        /// <summary>Adds page, required attribute of every annotation.</summary>
        internal virtual void AddFdfAttributes(int pageNumber) {
            this.AddAttribute(new AttributeObject(XfdfConstants.PAGE, pageNumber.ToString()));
        }

        /// <summary>Gets Dest element, a child element of link, GoTo, GoToR elements.</summary>
        /// <remarks>
        /// Gets Dest element, a child element of link, GoTo, GoToR elements.
        /// Corresponds to the Dest key in link annotation dictionary.
        /// </remarks>
        /// <returns>
        /// inner
        /// <see cref="DestObject">destination object</see>
        /// of annotation object
        /// </returns>
        public virtual DestObject GetDestination() {
            return destination;
        }

        /// <summary>Sets Dest element, a child element of link, GoTo, GoToR elements.</summary>
        /// <remarks>
        /// Sets Dest element, a child element of link, GoTo, GoToR elements.
        /// Corresponds to the Dest key in link annotation dictionary.
        /// </remarks>
        /// <param name="destination">
        /// 
        /// <see cref="DestObject">destination object</see>
        /// , an inner element of annotation object
        /// </param>
        /// <returns>
        /// current
        /// <see cref="AnnotObject">annotation object</see>
        /// </returns>
        public virtual iText.Forms.Xfdf.AnnotObject SetDestination(DestObject destination) {
            this.destination = destination;
            return this;
        }

        /// <summary>Gets the string value of the appearance element, a child element of stamp element.</summary>
        /// <remarks>
        /// Gets the string value of the appearance element, a child element of stamp element.
        /// Corresponds to the AP key in the annotation dictionary.
        /// Content model: Base64 encoded string.
        /// For more details see paragraph 6.5.1 in Xfdf document specification.
        /// </remarks>
        /// <returns>
        /// 
        /// <see cref="System.String"/>
        /// value of inner appearance element
        /// </returns>
        public virtual String GetAppearance() {
            return appearance;
        }

        /// <summary>Gets the string value of the appearance element,  a child element of stamp element.</summary>
        /// <remarks>
        /// Gets the string value of the appearance element,  a child element of stamp element.
        /// Corresponds to the AP key in the annotation dictionary.
        /// Content model: Base64 encoded string.
        /// </remarks>
        /// <param name="appearance">
        /// 
        /// <see cref="System.String"/>
        /// value of inner appearance element of annotation object
        /// </param>
        /// <returns>
        /// current
        /// <see cref="AnnotObject">annotation object</see>
        /// </returns>
        public virtual iText.Forms.Xfdf.AnnotObject SetAppearance(String appearance) {
            this.appearance = appearance;
            return this;
        }

        /// <summary>Gets the string value of the defaultappearance element, a child of the caret and freetext elements.
        ///     </summary>
        /// <remarks>
        /// Gets the string value of the defaultappearance element, a child of the caret and freetext elements.
        /// Corresponds to the DA key in the free text annotation dictionary.
        /// Content model: text string.
        /// For more details see paragraph 6.5.7 in Xfdf document specification.
        /// </remarks>
        /// <returns>
        /// 
        /// <see cref="System.String"/>
        /// value of inner deafultappearance element
        /// </returns>
        public virtual String GetDefaultAppearance() {
            return defaultAppearance;
        }

        /// <summary>Sets the string value of the defaultappearance element, a child of the caret and freetext elements.
        ///     </summary>
        /// <remarks>
        /// Sets the string value of the defaultappearance element, a child of the caret and freetext elements.
        /// Corresponds to the DA key in the free text annotation dictionary.
        /// Content model: text string.
        /// </remarks>
        /// <param name="defaultAppearance">
        /// 
        /// <see cref="System.String"/>
        /// value of inner defaultappearance element of annotation object
        /// </param>
        /// <returns>
        /// current
        /// <see cref="AnnotObject">annotation object</see>
        /// </returns>
        public virtual iText.Forms.Xfdf.AnnotObject SetDefaultAppearance(String defaultAppearance) {
            this.defaultAppearance = defaultAppearance;
            return this;
        }

        /// <summary>Gets the string value of the defaultstyle element, a child of the freetext element.</summary>
        /// <remarks>
        /// Gets the string value of the defaultstyle element, a child of the freetext element.
        /// Corresponds to the DS key in the free text annotation dictionary.
        /// Content model : a text string.
        /// For more details see paragraph 6.5.9 in Xfdf document specification.
        /// </remarks>
        /// <returns>
        /// 
        /// <see cref="System.String"/>
        /// value of inner defaultstyle element
        /// </returns>
        public virtual String GetDefaultStyle() {
            return defaultStyle;
        }

        /// <summary>Sets the string value of the defaultstyle element, a child of the freetext element.</summary>
        /// <remarks>
        /// Sets the string value of the defaultstyle element, a child of the freetext element.
        /// Corresponds to the DS key in the free text annotation dictionary.
        /// Content model : a text string.
        /// </remarks>
        /// <param name="defaultStyle">
        /// 
        /// <see cref="System.String"/>
        /// value of inner defaultstyle element of annotation object
        /// </param>
        /// <returns>
        /// current
        /// <see cref="AnnotObject">annotation object</see>
        /// </returns>
        public virtual iText.Forms.Xfdf.AnnotObject SetDefaultStyle(String defaultStyle) {
            this.defaultStyle = defaultStyle;
            return this;
        }

        /// <summary>Gets the BorderStyleAlt element, a child of the link element.</summary>
        /// <remarks>
        /// Gets the BorderStyleAlt element, a child of the link element.
        /// Corresponds to the Border key in the common annotation dictionary.
        /// For more details see paragraph 6.5.3 in Xfdf document specification.
        /// </remarks>
        /// <returns>
        /// inner
        /// <see cref="BorderStyleAltObject">BorderStyleAlt object</see>
        /// </returns>
        public virtual BorderStyleAltObject GetBorderStyleAlt() {
            return borderStyleAlt;
        }

        /// <summary>Sets the BorderStyleAlt element, a child of the link element.</summary>
        /// <remarks>
        /// Sets the BorderStyleAlt element, a child of the link element.
        /// Corresponds to the Border key in the common annotation dictionary.
        /// </remarks>
        /// <param name="borderStyleAlt">
        /// inner
        /// <see cref="BorderStyleAltObject">BorderStyleAlt object</see>
        /// </param>
        /// <returns>
        /// current
        /// <see cref="AnnotObject">annotation object</see>
        /// </returns>
        public virtual iText.Forms.Xfdf.AnnotObject SetBorderStyleAlt(BorderStyleAltObject borderStyleAlt) {
            this.borderStyleAlt = borderStyleAlt;
            return this;
        }

        /// <summary>Gets the string, containing vertices element, a child of the polygon and polyline elements.</summary>
        /// <remarks>
        /// Gets the string, containing vertices element, a child of the polygon and polyline elements.
        /// Corresponds to the Vertices key in the polygon or polyline annotation dictionary.
        /// For more details see paragraph 6.5.31 in Xfdf document specification.
        /// </remarks>
        /// <returns>
        /// 
        /// <see cref="System.String"/>
        /// value of inner vertices element
        /// </returns>
        public virtual String GetVertices() {
            return vertices;
        }

        /// <summary>Sets the string, containing vertices element, a child of the polygon and polyline elements.</summary>
        /// <remarks>
        /// Sets the string, containing vertices element, a child of the polygon and polyline elements.
        /// Corresponds to the Vertices key in the polygon or polyline annotation dictionary.
        /// </remarks>
        /// <param name="vertices">
        /// 
        /// <see cref="System.String"/>
        /// value of inner vertices element
        /// </param>
        /// <returns>
        /// current
        /// <see cref="AnnotObject">annotation object</see>
        /// </returns>
        public virtual iText.Forms.Xfdf.AnnotObject SetVertices(String vertices) {
            this.vertices = vertices;
            return this;
        }

        /// <summary>
        /// Gets the reference to the source
        /// <see cref="iText.Kernel.Pdf.Annot.PdfAnnotation"/>.
        /// </summary>
        /// <remarks>
        /// Gets the reference to the source
        /// <see cref="iText.Kernel.Pdf.Annot.PdfAnnotation"/>
        /// . Used for attaching popups in case of reading data from pdf file.
        /// </remarks>
        /// <returns>
        /// an
        /// <see cref="iText.Kernel.Pdf.PdfIndirectReference"/>
        /// of the source annotation object.
        /// </returns>
        public virtual PdfIndirectReference GetRef() {
            return @ref;
        }

        /// <summary>
        /// Sets the reference to the source
        /// <see cref="iText.Kernel.Pdf.Annot.PdfAnnotation"/>.
        /// </summary>
        /// <remarks>
        /// Sets the reference to the source
        /// <see cref="iText.Kernel.Pdf.Annot.PdfAnnotation"/>
        /// . Used for attaching popups in case of reading data from pdf file.
        /// </remarks>
        /// <param name="ref">
        /// 
        /// <see cref="iText.Kernel.Pdf.PdfIndirectReference"/>
        /// of the source annotation object.
        /// </param>
        /// <returns>
        /// this
        /// <see cref="AnnotObject"/>
        /// instance.
        /// </returns>
        public virtual iText.Forms.Xfdf.AnnotObject SetRef(PdfIndirectReference @ref) {
            this.@ref = @ref;
            return this;
        }
    }
}
