/*
This file is part of the iText (R) project.
Copyright (c) 1998-2026 Apryse Group NV
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
using iText.Layout.Element;

namespace iText.Layout.Renderer {
    /// <summary>
    /// Renderer for
    /// <see cref="iText.Layout.Element.Footnote"/>
    /// representing a footnote placed at the bottom of the page.
    /// </summary>
    public class FootnoteRenderer : BlockRenderer {
        /// <summary>
        /// Creates a
        /// <see cref="FootnoteRenderer"/>
        /// from its corresponding layout object.
        /// </summary>
        /// <param name="modelElement">
        /// the
        /// <see cref="iText.Layout.Element.Footnote"/>
        /// which this object should manage
        /// </param>
        public FootnoteRenderer(Footnote modelElement)
            : base(modelElement) {
        }

        public override IRenderer GetNextRenderer() {
            LogWarningIfGetNextRendererNotOverridden(typeof(iText.Layout.Renderer.FootnoteRenderer), this.GetType());
            return new iText.Layout.Renderer.FootnoteRenderer((Footnote)modelElement);
        }
    }
}
