/*
This file is part of the iText (R) project.
Copyright (c) 1998-2019 iText Group NV
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
using iText.Kernel.Geom;
using iText.Kernel.Pdf;

namespace iText.Forms.Xfdf {
    /// <summary>Represents annotation, a child element of annots tag in Xfdf document structure.</summary>
    /// <remarks>Represents annotation, a child element of annots tag in Xfdf document structure. For more details see part 6.4 in Xfdf specification.
    ///     </remarks>
    public class AnnotObject {
        /// <summary>Represents the type of annotation.</summary>
        /// <remarks>
        /// Represents the type of annotation. Possible values: caret, circle, fileattachment, freetext,
        /// highlight, ink, line, polygon, polyline, sound, square, squiggly, stamp, strikeout, text, underline.
        /// </remarks>
        private String name;

        /// <summary>Represents a list of attributes of the annotation.</summary>
        private IList<AttributeObject> attributes;

        /// <summary>Represents contents-richtext tag in Xfdf document structure.</summary>
        /// <remarks>
        /// Represents contents-richtext tag in Xfdf document structure. Is a child of caret, circle, fileattachment, freetext,
        /// highlight, ink, line, polygon, polyline, sound, square, squiggly, stamp, strikeout, text, and
        /// underline elements. corresponds to RC key in annotation dictionary.
        /// Content model: text string.
        /// For more details see paragraph 6.5.4 in Xfdf document specification.
        /// </remarks>
        private PdfString contents;

        /// <summary>Represents contents-richtext tag in Xfdf document structure.</summary>
        /// <remarks>
        /// Represents contents-richtext tag in Xfdf document structure. Is a child of caret, circle, fileattachment, freetext,
        /// highlight, ink, line, polygon, polyline, sound, square, squiggly, stamp, strikeout, text, and
        /// underline elements. corresponds to Contents key in annotation dictionary.
        /// Content model: a string or a rich text string.
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
        /// <see cref="iText.Kernel.Pdf.Annot.PdfAnnotation"/>
        /// . Used for attaching popups in case of reading data from pdf file.
        /// </summary>
        private PdfIndirectReference @ref;

        public AnnotObject() {
            //basically text string
            //should be Base64String
            this.attributes = new List<AttributeObject>();
        }

        public virtual String GetName() {
            return name;
        }

        public virtual iText.Forms.Xfdf.AnnotObject SetName(String name) {
            this.name = name;
            return this;
        }

        public virtual IList<AttributeObject> GetAttributes() {
            return attributes;
        }

        /// <summary>The method finds the attribute by name in attributes list.</summary>
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

        /// <summary>The method finds the attribute by name in attributes list and return its strign value.</summary>
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

        public virtual iText.Forms.Xfdf.AnnotObject GetPopup() {
            return popup;
        }

        public virtual iText.Forms.Xfdf.AnnotObject SetPopup(iText.Forms.Xfdf.AnnotObject popup) {
            this.popup = popup;
            return this;
        }

        public virtual bool IsHasPopup() {
            return hasPopup;
        }

        public virtual iText.Forms.Xfdf.AnnotObject SetHasPopup(bool hasPopup) {
            this.hasPopup = hasPopup;
            return this;
        }

        public virtual PdfString GetContents() {
            return contents;
        }

        public virtual iText.Forms.Xfdf.AnnotObject SetContents(PdfString contents) {
            this.contents = contents;
            return this;
        }

        public virtual PdfString GetContentsRichText() {
            return contentsRichText;
        }

        public virtual iText.Forms.Xfdf.AnnotObject SetContentsRichText(PdfString contentsRichRext) {
            this.contentsRichText = contentsRichRext;
            return this;
        }

        public virtual ActionObject GetAction() {
            return action;
        }

        public virtual iText.Forms.Xfdf.AnnotObject SetAction(ActionObject action) {
            this.action = action;
            return this;
        }

        public virtual void AddAttribute(AttributeObject attr) {
            attributes.Add(attr);
        }

        internal virtual void AddAttribute(String name, bool value) {
            String valueString = value ? "yes" : "no";
            attributes.Add(new AttributeObject(name, valueString));
        }

        internal virtual void AddAttribute(String name, float value) {
            attributes.Add(new AttributeObject(name, value.ToString()));
        }

        internal virtual void AddAttribute(String name, Rectangle value) {
            String stringValue = XfdfObjectUtils.ConvertRectToString(value);
            attributes.Add(new AttributeObject(name, stringValue));
        }

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
                        valueString = ((PdfNumber)(valueObject)).GetValue().ToString();
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

        internal virtual void AddFdfAttributes(int pageNumber) {
            this.AddAttribute(new AttributeObject(XfdfConstants.PAGE, pageNumber.ToString()));
        }

        public virtual DestObject GetDestination() {
            return destination;
        }

        public virtual iText.Forms.Xfdf.AnnotObject SetDestination(DestObject destination) {
            this.destination = destination;
            return this;
        }

        public virtual String GetAppearance() {
            return appearance;
        }

        public virtual iText.Forms.Xfdf.AnnotObject SetAppearance(String appearance) {
            this.appearance = appearance;
            return this;
        }

        public virtual String GetDefaultAppearance() {
            return defaultAppearance;
        }

        public virtual iText.Forms.Xfdf.AnnotObject SetDefaultAppearance(String defaultAppearance) {
            this.defaultAppearance = defaultAppearance;
            return this;
        }

        public virtual String GetDefaultStyle() {
            return defaultStyle;
        }

        public virtual iText.Forms.Xfdf.AnnotObject SetDefaultStyle(String defaultStyle) {
            this.defaultStyle = defaultStyle;
            return this;
        }

        public virtual BorderStyleAltObject GetBorderStyleAlt() {
            return borderStyleAlt;
        }

        public virtual iText.Forms.Xfdf.AnnotObject SetBorderStyleAlt(BorderStyleAltObject borderStyleAlt) {
            this.borderStyleAlt = borderStyleAlt;
            return this;
        }

        public virtual String GetVertices() {
            return vertices;
        }

        public virtual iText.Forms.Xfdf.AnnotObject SetVertices(String vertices) {
            this.vertices = vertices;
            return this;
        }

        public virtual PdfIndirectReference GetRef() {
            return @ref;
        }

        public virtual iText.Forms.Xfdf.AnnotObject SetRef(PdfIndirectReference @ref) {
            this.@ref = @ref;
            return this;
        }
    }
}
