/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: Bruno Lowagie, Paulo Soares, et al.

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
