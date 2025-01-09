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
using iText.Forms;
using iText.Forms.Exceptions;
using iText.Forms.Form;
using iText.Forms.Form.Renderer;
using iText.Kernel.Pdf.Tagutils;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Layout.Renderer;

namespace iText.Forms.Form.Element {
    /// <summary>
    /// Extension of the
    /// <see cref="FormField{T}"/>
    /// class representing a button so that
    /// a
    /// <see cref="iText.Forms.Form.Renderer.InputFieldRenderer"/>
    /// is used.
    /// </summary>
    public class InputField : FormField<iText.Forms.Form.Element.InputField>, IPlaceholderable {
        /// <summary>Default padding X offset.</summary>
        private const float X_OFFSET = 2;

        /// <summary>The placeholder paragraph.</summary>
        private Paragraph placeholder;

        /// <summary>Field rotation, counterclockwise.</summary>
        /// <remarks>Field rotation, counterclockwise. Must be a multiple of 90 degrees.</remarks>
        private int rotation = 0;

        /// <summary>Creates a new input field.</summary>
        /// <param name="id">the id</param>
        public InputField(String id)
            : base(id) {
            SetProperties();
        }

        /// <summary>Determines, whether the input field will be password.</summary>
        /// <remarks>
        /// Determines, whether the input field will be password.
        /// <para />
        /// Usually means that instead of glyphs '*' will be shown in case of flatten field.
        /// <para />
        /// If the field is not flatten, value will be ignored.
        /// </remarks>
        /// <param name="isPassword">
        /// 
        /// <see langword="true"/>
        /// is this field shall be considered as password,
        /// <see langword="false"/>
        /// otherwise
        /// </param>
        /// <returns>this input field</returns>
        public virtual iText.Forms.Form.Element.InputField UseAsPassword(bool isPassword) {
            SetProperty(FormProperty.FORM_FIELD_PASSWORD_FLAG, isPassword);
            return this;
        }

        /// <summary><inheritDoc/></summary>
        public virtual Paragraph GetPlaceholder() {
            return placeholder;
        }

        /// <summary><inheritDoc/></summary>
        public virtual void SetPlaceholder(Paragraph placeholder) {
            this.placeholder = placeholder;
        }

        /* (non-Javadoc)
        * @see FormField#getDefaultProperty(int)
        */
        public override T1 GetDefaultProperty<T1>(int property) {
            switch (property) {
                case FormProperty.FORM_FIELD_PASSWORD_FLAG: {
                    return (T1)(Object)false;
                }

                case FormProperty.FORM_FIELD_SIZE: {
                    return (T1)(Object)20;
                }

                case FormProperty.TEXT_FIELD_COMB_FLAG: {
                    return (T1)(Object)false;
                }

                case FormProperty.TEXT_FIELD_MAX_LEN: {
                    return (T1)(Object)0;
                }

                default: {
                    return base.GetDefaultProperty<T1>(property);
                }
            }
        }

        /// <summary>Get rotation.</summary>
        /// <returns>rotation value.</returns>
        public virtual int GetRotation() {
            return this.rotation;
        }

        /// <summary>Set rotation of the input field.</summary>
        /// <param name="rotation">
        /// new rotation value, counterclockwise. Must be a multiple of 90 degrees.
        /// It has sense only in interactive mode, see
        /// <see cref="FormField{T}.SetInteractive(bool)"/>
        /// </param>
        /// <returns>
        /// the edited
        /// <see cref="InputField"/>
        /// </returns>
        public virtual iText.Forms.Form.Element.InputField SetRotation(int rotation) {
            if (rotation % 90 != 0) {
                throw new ArgumentException(FormsExceptionMessageConstant.INVALID_ROTATION_VALUE);
            }
            this.rotation = rotation;
            return this;
        }

        /// <summary>
        /// Sets
        /// <c>Comb</c>
        /// flag for the text field.
        /// </summary>
        /// <remarks>
        /// Sets
        /// <c>Comb</c>
        /// flag for the text field. Meaningful only if the MaxLen entry is present in the text field
        /// dictionary and if the Multiline, Password and FileSelect flags are clear.
        /// <para />
        /// If true, the field is automatically divided into as many equally spaced positions, or combs,
        /// as the value of MaxLen, and the text is laid out into those combs.
        /// </remarks>
        /// <param name="isComb">boolean value specifying whether to enable combing</param>
        /// <returns>
        /// this
        /// <see cref="InputField"/>
        /// instance
        /// </returns>
        public virtual iText.Forms.Form.Element.InputField SetComb(bool isComb) {
            SetProperty(FormProperty.TEXT_FIELD_COMB_FLAG, isComb);
            return this;
        }

        /// <summary>Sets the maximum length of the field's text, in characters.</summary>
        /// <param name="maxLen">the current maximum text length</param>
        /// <returns>
        /// this
        /// <see cref="InputField"/>
        /// instance
        /// </returns>
        public virtual iText.Forms.Form.Element.InputField SetMaxLen(int maxLen) {
            SetProperty(FormProperty.TEXT_FIELD_MAX_LEN, maxLen);
            return this;
        }

        /// <summary><inheritDoc/></summary>
        public override AccessibilityProperties GetAccessibilityProperties() {
            if (tagProperties == null) {
                tagProperties = new FormDefaultAccessibilityProperties(FormDefaultAccessibilityProperties.FORM_FIELD_TEXT);
            }
            return tagProperties;
        }

        /* (non-Javadoc)
        * @see com.itextpdf.layout.element.AbstractElement#makeNewRenderer()
        */
        protected override IRenderer MakeNewRenderer() {
            return new InputFieldRenderer(this);
        }

        private void SetProperties() {
            SetProperty(Property.PADDING_LEFT, UnitValue.CreatePointValue(X_OFFSET));
            SetProperty(Property.PADDING_RIGHT, UnitValue.CreatePointValue(X_OFFSET));
            SetProperty(Property.BOX_SIZING, BoxSizingPropertyValue.BORDER_BOX);
        }
    }
}
