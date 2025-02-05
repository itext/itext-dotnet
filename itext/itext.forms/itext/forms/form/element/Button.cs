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
using iText.Forms.Form.Renderer;
using iText.Kernel.Colors;
using iText.Kernel.Pdf.Tagutils;
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
        private static readonly VerticalAlignment? DEFAULT_VERTICAL_ALIGNMENT = VerticalAlignment.MIDDLE;

        private static readonly TextAlignment? DEFAULT_TEXT_ALIGNMENT = TextAlignment.CENTER;

        private static readonly Color DEFAULT_BACKGROUND_COLOR = ColorConstants.LIGHT_GRAY;

        /// <summary>Indicates if it's the button with only single line caption.</summary>
        private bool singleLine = false;

        /// <summary>
        /// Creates a new
        /// <see cref="Button"/>
        /// instance.
        /// </summary>
        /// <param name="id">the id</param>
        public Button(String id)
            : base(id) {
            SetTextAlignment(DEFAULT_TEXT_ALIGNMENT);
            SetBackgroundColor(DEFAULT_BACKGROUND_COLOR);
            // Draw the borders inside the element by default
            SetProperty(Property.BOX_SIZING, BoxSizingPropertyValue.BORDER_BOX);
        }

        /// <summary>Adds any block element to the div's contents.</summary>
        /// <param name="element">
        /// a
        /// <see cref="iText.Layout.Element.BlockElement{T}"/>
        /// </param>
        /// <returns>this Element</returns>
        public virtual iText.Forms.Form.Element.Button Add(IBlockElement element) {
            singleLine = false;
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
            singleLine = false;
            childElements.Add(element);
            return this;
        }

        /// <summary>Indicates if this element represents an input with type button in html.</summary>
        /// <returns>true if it's the button with only one line caption.</returns>
        public virtual bool IsSingleLine() {
            return singleLine;
        }

        /// <summary>Sets passed string value to the single line button caption.</summary>
        /// <remarks>
        /// Sets passed string value to the single line button caption.
        /// Value will be clipped if it is not fit into single line. For multiple line value
        /// use
        /// <see cref="SetValue(System.String)"/>
        /// . Note that when adding other elements to the button
        /// after this method is called, this added value can be multiline.
        /// </remarks>
        /// <param name="value">string value to be set as caption.</param>
        /// <returns>
        /// this same
        /// <see cref="Button"/>
        /// instance.
        /// </returns>
        public virtual iText.Forms.Form.Element.Button SetSingleLineValue(String value) {
            SetValue(value);
            SetProperty(Property.KEEP_TOGETHER, false);
            singleLine = true;
            return this;
        }

        /// <summary>Adds passed string value as paragraph to the button.</summary>
        /// <remarks>
        /// Adds passed string value as paragraph to the button.
        /// Value can be multiline if it is not fit into single line. For single line value
        /// use
        /// <see cref="SetSingleLineValue(System.String)"/>
        /// . Note that the new value will replace all already added elements.
        /// </remarks>
        /// <param name="value">string value to be added into button.</param>
        /// <returns>
        /// 
        /// <inheritDoc/>
        /// </returns>
        public override IFormField SetValue(String value) {
            childElements.Clear();
            Paragraph paragraph = new Paragraph(value).SetMargin(0).SetMultipliedLeading(1).SetVerticalAlignment(DEFAULT_VERTICAL_ALIGNMENT
                ).SetTextAlignment(DEFAULT_TEXT_ALIGNMENT);
            if (this.GetProperty<Object>(Property.FONT) != null) {
                paragraph.SetProperty(Property.FONT, this.GetProperty<Object>(Property.FONT));
            }
            if (this.GetProperty<UnitValue>(Property.FONT_SIZE) != null) {
                paragraph.SetFontSize(this.GetProperty<UnitValue>(Property.FONT_SIZE).GetValue());
            }
            return Add(paragraph);
        }

        /// <summary><inheritDoc/></summary>
        /// <returns>
        /// 
        /// <inheritDoc/>
        /// </returns>
        public override T1 GetDefaultProperty<T1>(int property) {
            if (property == Property.KEEP_TOGETHER) {
                return (T1)(Object)true;
            }
            return base.GetDefaultProperty<T1>(property);
        }

        /// <summary><inheritDoc/></summary>
        public override AccessibilityProperties GetAccessibilityProperties() {
            if (tagProperties == null) {
                tagProperties = new FormDefaultAccessibilityProperties(FormDefaultAccessibilityProperties.FORM_FIELD_PUSH_BUTTON
                    );
            }
            return tagProperties;
        }

        /// <summary><inheritDoc/></summary>
        /// <returns>
        /// 
        /// <inheritDoc/>
        /// </returns>
        protected override IRenderer MakeNewRenderer() {
            return new ButtonRenderer(this);
        }
    }
}
