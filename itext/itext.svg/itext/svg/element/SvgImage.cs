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
using iText.Kernel.Pdf;
using iText.Layout.Element;
using iText.Layout.Renderer;
using iText.Svg.Renderers;
using iText.Svg.Xobject;

namespace iText.Svg.Element {
    /// <summary>A layout element that represents SVG image for inclusion in the document model.</summary>
    public class SvgImage : Image {
        /// <summary>
        /// Creates an
        /// <see cref="SvgImage"/>
        /// from the
        /// <see cref="iText.Svg.Xobject.SvgImageXObject"/>
        /// which represents Form XObject and contains
        /// processor result with the SVG information and resource resolver for the SVG image.
        /// </summary>
        /// <param name="xObject">
        /// an internal
        /// <see cref="iText.Svg.Xobject.SvgImageXObject"/>.
        /// </param>
        public SvgImage(SvgImageXObject xObject)
            : base(xObject) {
        }

        /// <summary>
        /// Draws SVG image to a canvas-like object maintained in the
        /// <see cref="iText.Svg.Renderers.SvgDrawContext"/>.
        /// </summary>
        /// <param name="document">pdf that shall contain the SVG image.</param>
        public virtual void Generate(PdfDocument document) {
            ((SvgImageXObject)xObject).Generate(document);
        }

        /// <summary><inheritDoc/></summary>
        protected override IRenderer MakeNewRenderer() {
            return new SvgImageRenderer(this);
        }
    }
}
