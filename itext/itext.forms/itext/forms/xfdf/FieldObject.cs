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

namespace iText.Forms.Xfdf {
    /// <summary>Represents the field element, a child of the fields and field elements.</summary>
    /// <remarks>
    /// Represents the field element, a child of the fields and field elements.
    /// The field element corresponds to a form field.
    /// Content model: ( field* | value* | ( value? &amp; value-richtext? )).
    /// Required attributes: name.
    /// For more details see paragraph 6.3.2 in XFDF document specification.
    /// </remarks>
    public class FieldObject {
        /// <summary>Represents the name attribute of the field element.</summary>
        /// <remarks>
        /// Represents the name attribute of the field element.
        /// Corresponds to the T key in the field dictionary.
        /// In a hierarchical form field, the name is the partial field name.
        /// For more details see paragraph 6.3.2.2 in XFDF document specification.
        /// </remarks>
        private String name;

        /// <summary>
        /// Represents the value element, a child of the field element and contains the field's value, whose format may
        /// vary depending on the field type.
        /// </summary>
        /// <remarks>
        /// Represents the value element, a child of the field element and contains the field's value, whose format may
        /// vary depending on the field type.
        /// Corresponds to the V key in the FDF field dictionary.
        /// Content model: text string.
        /// For more details see paragraph 6.3.3 in XFDF document specification.
        /// </remarks>
        private String value;

        /// <summary>
        /// Represents the value-richtext element, a child of the field element and contains the field's value formatted as a
        /// rich text string.
        /// </summary>
        /// <remarks>
        /// Represents the value-richtext element, a child of the field element and contains the field's value formatted as a
        /// rich text string.
        /// Corresponds to the RV key in the variable text field dictionary.
        /// Content model: text strign or rich text string.
        /// Attributes: none.
        /// For more details see paragraph 6.3.4 in XFDF document specification.
        /// </remarks>
        private String richTextValue;

        /// <summary>Indicates if a value-richtext element is present inside the field.</summary>
        private bool containsRichText;

        /// <summary>Parent field of current field.</summary>
        private iText.Forms.Xfdf.FieldObject parent;

        public FieldObject() {
        }

        public FieldObject(String name, String value, bool containsRichText) {
            this.name = name;
            this.containsRichText = containsRichText;
            if (containsRichText) {
                this.richTextValue = value;
            }
            else {
                this.value = value;
            }
        }

        /// <summary>Gets the string value of the name attribute of the field element.</summary>
        /// <remarks>
        /// Gets the string value of the name attribute of the field element.
        /// Corresponds to the T key in the field dictionary.
        /// In a hierarchical form field, the name is the partial field name.
        /// For more details see paragraph 6.3.2.2 in XFDF document specification.
        /// </remarks>
        /// <returns>
        /// 
        /// <see cref="System.String"/>
        /// value of field name attribute
        /// </returns>
        public virtual String GetName() {
            return name;
        }

        /// <summary>Sets the string value of the name attribute of the field element.</summary>
        /// <remarks>
        /// Sets the string value of the name attribute of the field element.
        /// Corresponds to the T key in the field dictionary.
        /// In a hierarchical form field, the name is the partial field name.
        /// </remarks>
        /// <param name="name">
        /// 
        /// <see cref="System.String"/>
        /// value of field name attribute
        /// </param>
        public virtual void SetName(String name) {
            this.name = name;
        }

        /// <summary>
        /// Gets the string representation of the value element, a child of the field element and contains the field's value,
        /// whose format may vary depending on the field type.
        /// </summary>
        /// <remarks>
        /// Gets the string representation of the value element, a child of the field element and contains the field's value,
        /// whose format may vary depending on the field type.
        /// Corresponds to the V key in the FDF field dictionary.
        /// For more details see paragraph 6.3.3 in XFDF document specification.
        /// </remarks>
        /// <returns>
        /// 
        /// <see cref="System.String"/>
        /// representation of inner value element of the field
        /// </returns>
        public virtual String GetValue() {
            return value;
        }

        /// <summary>
        /// Sets the string representation of the value element, a child of the field element and contains the field's value,
        /// whose format may vary depending on the field type.
        /// </summary>
        /// <remarks>
        /// Sets the string representation of the value element, a child of the field element and contains the field's value,
        /// whose format may vary depending on the field type.
        /// Corresponds to the V key in the FDF field dictionary.
        /// </remarks>
        /// <param name="value">
        /// 
        /// <see cref="System.String"/>
        /// representation of inner value element of the field
        /// </param>
        public virtual void SetValue(String value) {
            this.value = value;
        }

        /// <summary>
        /// Gets the string representation of the value-richtext element, a child of the field element and contains the
        /// field's value formatted as a rich text string.
        /// </summary>
        /// <remarks>
        /// Gets the string representation of the value-richtext element, a child of the field element and contains the
        /// field's value formatted as a rich text string.
        /// Corresponds to the RV key in the variable text field dictionary.
        /// Content model: text strign or rich text string.
        /// For more details see paragraph 6.3.4 in XFDF document specification.
        /// </remarks>
        /// <returns>
        /// 
        /// <see cref="System.String"/>
        /// representation of inner value-richtext element of the field
        /// </returns>
        public virtual String GetRichTextValue() {
            return richTextValue;
        }

        /// <summary>
        /// Sets the string representation of the value-richtext element, a child of the field element and contains the
        /// field's value formatted as a rich text string.
        /// </summary>
        /// <remarks>
        /// Sets the string representation of the value-richtext element, a child of the field element and contains the
        /// field's value formatted as a rich text string.
        /// Corresponds to the RV key in the variable text field dictionary.
        /// Content model: text string or rich text string.
        /// </remarks>
        /// <param name="richTextValue">
        /// 
        /// <see cref="System.String"/>
        /// representation of inner value-richtext element of the field
        /// </param>
        public virtual void SetRichTextValue(String richTextValue) {
            this.richTextValue = richTextValue;
        }

        /// <summary>Gets a boolean indicating if a value-richtext element is present inside the field.</summary>
        /// <returns>true if a value-richtext element is present inside the field, false otherwise</returns>
        public virtual bool IsContainsRichText() {
            return containsRichText;
        }

        /// <summary>Sets a boolean indicating if a value-richtext element is present inside the field.</summary>
        /// <param name="containsRichText">a boolean indicating if a value-richtext element is present inside the field
        ///     </param>
        public virtual void SetContainsRichText(bool containsRichText) {
            this.containsRichText = containsRichText;
        }

        /// <summary>Gets a parent field of current field.</summary>
        /// <returns>
        /// parent
        /// <see cref="FieldObject">field object</see>
        /// of the current field
        /// </returns>
        public virtual iText.Forms.Xfdf.FieldObject GetParent() {
            return parent;
        }

        /// <summary>Sets a parent field of current field.</summary>
        /// <param name="parent">
        /// 
        /// <see cref="FieldObject">field object</see>
        /// that is a parent of the current field
        /// </param>
        public virtual void SetParent(iText.Forms.Xfdf.FieldObject parent) {
            this.parent = parent;
        }
    }
}
