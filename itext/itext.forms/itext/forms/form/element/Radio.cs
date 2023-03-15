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
using iText.Forms.Form;
using iText.Forms.Form.Renderer;
using iText.Layout.Properties;
using iText.Layout.Renderer;

namespace iText.Forms.Form.Element {
    /// <summary>
    /// Extension of the
    /// <see cref="FormField{T}"/>
    /// class representing a radio button so that
    /// a
    /// <see cref="iText.Forms.Form.Renderer.RadioRenderer"/>
    /// is used instead of the default renderer for fields.
    /// </summary>
    public class Radio : FormField<iText.Forms.Form.Element.Radio> {
        /// <summary>
        /// Creates a new
        /// <see cref="Radio"/>
        /// instance.
        /// </summary>
        /// <param name="id">the id.</param>
        public Radio(String id)
            : base(id) {
            // Draw the borders inside the element by default
            SetProperty(Property.BOX_SIZING, BoxSizingPropertyValue.BORDER_BOX);
            // Rounded border
            SetBorderRadius(new BorderRadius(UnitValue.CreatePercentValue(50)));
            // Draw border as a circle by default
            SetProperty(FormProperty.FORM_FIELD_RADIO_BORDER_CIRCLE, true);
        }

        /// <summary>
        /// Creates a new
        /// <see cref="Radio"/>
        /// instance.
        /// </summary>
        /// <param name="id">the id.</param>
        /// <param name="radioGroupName">
        /// the name of the radio group the radio button belongs to. It has sense only in case
        /// this Radio element will not be rendered but Acroform field will be created instead.
        /// </param>
        public Radio(String id, String radioGroupName)
            : this(id) {
            SetProperty(FormProperty.FORM_FIELD_RADIO_GROUP_NAME, radioGroupName);
        }

        /// <summary>Sets the state of the radio button.</summary>
        /// <param name="checked">
        /// 
        /// <see langword="true"/>
        /// if the radio button shall be checked,
        /// <see langword="false"/>
        /// otherwise.
        /// By default, the radio button is unchecked.
        /// </param>
        /// <returns>
        /// this same
        /// <see cref="Radio"/>
        /// button.
        /// </returns>
        public virtual iText.Forms.Form.Element.Radio SetChecked(bool @checked) {
            SetProperty(FormProperty.FORM_FIELD_CHECKED, @checked);
            return this;
        }

        /// <summary>Sets the radio button width and height.</summary>
        /// <param name="size">radio button width and height.</param>
        /// <returns>
        /// this same
        /// <see cref="Radio"/>
        /// button.
        /// </returns>
        public virtual iText.Forms.Form.Element.Radio SetSize(float size) {
            SetProperty(Property.WIDTH, UnitValue.CreatePointValue(size));
            SetProperty(Property.HEIGHT, UnitValue.CreatePointValue(size));
            return this;
        }

        /* (non-Javadoc)
        * @see com.itextpdf.layout.element.AbstractElement#makeNewRenderer()
        */
        protected override IRenderer MakeNewRenderer() {
            return new RadioRenderer(this);
        }
    }
}
