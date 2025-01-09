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
using iText.Forms.Form;
using iText.Kernel.Pdf.Tagutils;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Layout.Tagging;

namespace iText.Forms.Form.Element {
    /// <summary>
    /// Implementation of the
    /// <see cref="iText.Layout.Element.AbstractElement{T}"/>
    /// class for form fields.
    /// </summary>
    /// <typeparam name="T">the generic type of the form field (e.g. input field, button, text area)</typeparam>
    public abstract class FormField<T> : AbstractElement<T>, IFormField, IAccessibleElement
        where T : IFormField {
        /// <summary>The id.</summary>
        private readonly String id;

        /// <summary>The tag properties.</summary>
        protected internal DefaultAccessibilityProperties tagProperties;

//\cond DO_NOT_DOCUMENT
        /// <summary>
        /// Instantiates a new
        /// <see cref="FormField{T}"/>
        /// instance.
        /// </summary>
        /// <param name="id">the id</param>
        internal FormField(String id) {
            if (id == null || id.Contains(".")) {
                throw new ArgumentException("id should not contain '.'");
            }
            this.id = id;
        }
//\endcond

        /// <summary>Sets the form field's width and height.</summary>
        /// <param name="size">form field's width and height.</param>
        /// <returns>
        /// this same
        /// <see cref="FormField{T}"/>
        /// element.
        /// </returns>
        public virtual T SetSize(float size) {
            SetProperty(Property.WIDTH, UnitValue.CreatePointValue(size));
            SetProperty(Property.HEIGHT, UnitValue.CreatePointValue(size));
            return (T)(Object)this;
        }

        /// <summary><inheritDoc/></summary>
        /// <param name="width">
        /// 
        /// <inheritDoc/>
        /// </param>
        /// <returns>
        /// 
        /// <inheritDoc/>
        /// </returns>
        public virtual IFormField SetWidth(float width) {
            SetProperty(Property.WIDTH, UnitValue.CreatePointValue(width));
            return this;
        }

        /// <summary><inheritDoc/></summary>
        /// <param name="height">
        /// 
        /// <inheritDoc/>
        /// </param>
        /// <returns>
        /// 
        /// <inheritDoc/>
        /// </returns>
        public virtual IFormField SetHeight(float height) {
            SetProperty(Property.HEIGHT, UnitValue.CreatePointValue(height));
            return this;
        }

        /// <summary><inheritDoc/></summary>
        /// <param name="value">
        /// 
        /// <inheritDoc/>
        /// </param>
        /// <returns>
        /// 
        /// <inheritDoc/>
        /// </returns>
        public virtual IFormField SetValue(String value) {
            SetProperty(FormProperty.FORM_FIELD_VALUE, value);
            return this;
        }

        /// <summary><inheritDoc/></summary>
        /// <returns>
        /// 
        /// <inheritDoc/>
        /// </returns>
        public virtual String GetId() {
            return id;
        }

        /// <summary><inheritDoc/></summary>
        /// <param name="property">
        /// 
        /// <inheritDoc/>
        /// </param>
        /// <returns>
        /// 
        /// <inheritDoc/>
        /// </returns>
        public override T1 GetDefaultProperty<T1>(int property) {
            switch (property) {
                case FormProperty.FORM_FIELD_FLATTEN: {
                    return (T1)(Object)true;
                }

                case FormProperty.FORM_FIELD_VALUE: {
                    return (T1)(Object)"";
                }

                default: {
                    return base.GetDefaultProperty<T1>(property);
                }
            }
        }

        /// <summary><inheritDoc/></summary>
        /// <param name="interactive">
        /// 
        /// <inheritDoc/>
        /// </param>
        /// <returns>
        /// 
        /// <inheritDoc/>
        /// </returns>
        public virtual IFormField SetInteractive(bool interactive) {
            SetProperty(FormProperty.FORM_FIELD_FLATTEN, !interactive);
            return this;
        }

        public abstract AccessibilityProperties GetAccessibilityProperties();
    }
}
