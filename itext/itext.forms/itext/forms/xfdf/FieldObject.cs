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
