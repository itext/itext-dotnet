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
using iText.Kernel.Pdf.Tagging;
using iText.Kernel.Pdf.Tagutils;
using iText.Layout.Properties;
using iText.Layout.Renderer;
using iText.Layout.Tagging;

namespace iText.Layout.Element {
    /// <summary>
    /// A
    /// <see cref="Text"/>
    /// is a piece of text of any length.
    /// </summary>
    /// <remarks>
    /// A
    /// <see cref="Text"/>
    /// is a piece of text of any length. As a
    /// <see cref="ILeafElement">leaf element</see>
    /// ,
    /// it is the smallest piece of content that may bear specific layout attributes.
    /// </remarks>
    public class Text : AbstractElement<iText.Layout.Element.Text>, ILeafElement, IAccessibleElement {
        protected internal String text;

        protected internal DefaultAccessibilityProperties tagProperties;

        /// <summary>Constructs a Text with its role initialized.</summary>
        /// <param name="text">
        /// the contents, as a
        /// <see cref="System.String"/>
        /// </param>
        public Text(String text) {
            if (null == text) {
                throw new ArgumentException();
            }
            this.text = text;
        }

        /// <summary>Gets the contents of the Text object that will be rendered.</summary>
        /// <returns>the string with the contents</returns>
        public virtual String GetText() {
            return text;
        }

        /// <summary>Sets the contents of the Text object.</summary>
        /// <param name="text">the new contents</param>
        public virtual void SetText(String text) {
            this.text = text;
        }

        /// <summary>Gets the text rise.</summary>
        /// <returns>the vertical distance from the text's default base line, as a float.</returns>
        public virtual float GetTextRise() {
            return (float)this.GetProperty<float?>(Property.TEXT_RISE);
        }

        /// <summary>Sets the text rise.</summary>
        /// <param name="textRise">a vertical distance from the text's default base line.</param>
        /// <returns>this Text</returns>
        public virtual iText.Layout.Element.Text SetTextRise(float textRise) {
            SetProperty(Property.TEXT_RISE, textRise);
            return (iText.Layout.Element.Text)(Object)this;
        }

        /// <summary>
        /// Gets the horizontal scaling property, which determines how wide the text
        /// should be stretched.
        /// </summary>
        /// <returns>the horizontal spacing, as a <c>float</c></returns>
        public virtual float? GetHorizontalScaling() {
            return this.GetProperty<float?>(Property.HORIZONTAL_SCALING);
        }

        /// <summary>Skews the text to simulate italic and other effects.</summary>
        /// <remarks>
        /// Skews the text to simulate italic and other effects. Try <c>alpha=0
        /// </c> and <c>beta=12</c>.
        /// </remarks>
        /// <param name="alpha">the first angle in degrees</param>
        /// <param name="beta">the second angle in degrees</param>
        /// <returns>this <c>Text</c></returns>
        public virtual iText.Layout.Element.Text SetSkew(float alpha, float beta) {
            alpha = (float)Math.Tan(alpha * Math.PI / 180);
            beta = (float)Math.Tan(beta * Math.PI / 180);
            SetProperty(Property.SKEW, new float[] { alpha, beta });
            return this;
        }

        /// <summary>
        /// The horizontal scaling parameter adjusts the width of glyphs by stretching or
        /// compressing them in the horizontal direction.
        /// </summary>
        /// <param name="horizontalScaling">
        /// the scaling parameter. 1 means no scaling will be applied,
        /// 0.5 means the text will be scaled by half.
        /// 2 means the text will be twice as wide as normal one.
        /// </param>
        /// <returns>this Text</returns>
        public virtual iText.Layout.Element.Text SetHorizontalScaling(float horizontalScaling) {
            SetProperty(Property.HORIZONTAL_SCALING, horizontalScaling);
            return (iText.Layout.Element.Text)(Object)this;
        }

        public virtual AccessibilityProperties GetAccessibilityProperties() {
            if (tagProperties == null) {
                tagProperties = new DefaultAccessibilityProperties(StandardRoles.SPAN);
            }
            return tagProperties;
        }

        /// <summary>Give this element a neutral role.</summary>
        /// <remarks>
        /// Give this element a neutral role. See also
        /// <see cref="iText.Kernel.Pdf.Tagutils.AccessibilityProperties.SetRole(System.String)"/>.
        /// </remarks>
        /// <returns>this Element</returns>
        public virtual iText.Layout.Element.Text SetNeutralRole() {
            this.GetAccessibilityProperties().SetRole(null);
            return this;
        }

        protected internal override IRenderer MakeNewRenderer() {
            return new TextRenderer(this, text);
        }
    }
}
