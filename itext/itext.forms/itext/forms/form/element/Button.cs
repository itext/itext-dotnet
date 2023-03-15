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
using iText.Forms.Form.Renderer;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Layout.Renderer;

namespace iText.Forms.Form.Element {
    /// <summary>
    /// Extension of the
    /// <see cref="FormField{T}"/>
    /// class representing a button in html.
    /// </summary>
    public class Button : FormField<iText.Forms.Form.Element.Button> {
        public Button(String id)
            : base(id) {
        }

        protected override IRenderer MakeNewRenderer() {
            return new ButtonRenderer(this);
        }

        /// <summary>Adds any block element to the div's contents.</summary>
        /// <param name="element">
        /// a
        /// <see cref="iText.Layout.Element.BlockElement{T}"/>
        /// </param>
        /// <returns>this Element</returns>
        public virtual iText.Forms.Form.Element.Button Add(IBlockElement element) {
            childElements.Add(element);
            return this;
        }

        /// <summary>Adds an image to the div's contents.</summary>
        /// <param name="element">
        /// an
        /// <see cref="iText.Layout.Element.Image"/>
        /// </param>
        /// <returns>this Element</returns>
        public virtual iText.Forms.Form.Element.Button Add(Image element) {
            childElements.Add(element);
            return this;
        }

        public override T1 GetDefaultProperty<T1>(int property) {
            if (property == Property.KEEP_TOGETHER) {
                return (T1)(Object)true;
            }
            return base.GetDefaultProperty<T1>(property);
        }
    }
}
