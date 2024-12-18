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
using iText.Layout.Renderer;
using iText.Svg.Element;

namespace iText.Svg.Renderers {
    /// <summary>
    /// Represents a renderer for the
    /// <see cref="iText.Svg.Element.SvgImage"/>
    /// layout element.
    /// </summary>
    public class SvgImageRenderer : ImageRenderer {
        /// <summary>Creates an SvgImageRenderer from its corresponding layout object.</summary>
        /// <param name="image">
        /// the
        /// <see cref="iText.Svg.Element.SvgImage"/>
        /// which this object should manage
        /// </param>
        public SvgImageRenderer(SvgImage image)
            : base(image) {
        }

        /// <summary><inheritDoc/></summary>
        public override void Draw(DrawContext drawContext) {
            ((SvgImage)modelElement).Generate(drawContext.GetDocument());
            base.Draw(drawContext);
        }
    }
}
