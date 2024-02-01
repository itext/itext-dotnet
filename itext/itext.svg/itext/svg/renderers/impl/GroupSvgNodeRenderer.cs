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
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Canvas;
using iText.Svg.Renderers;

namespace iText.Svg.Renderers.Impl {
    /// <summary>This renderer represents a branch in an SVG tree.</summary>
    /// <remarks>This renderer represents a branch in an SVG tree. It doesn't do anything aside from calling the superclass doDraw.
    ///     </remarks>
    public class GroupSvgNodeRenderer : AbstractBranchSvgNodeRenderer {
        protected internal override void DoDraw(SvgDrawContext context) {
            PdfCanvas currentCanvas = context.GetCurrentCanvas();
            foreach (ISvgNodeRenderer child in GetChildren()) {
                currentCanvas.SaveState();
                child.Draw(context);
                currentCanvas.RestoreState();
            }
        }

        public override ISvgNodeRenderer CreateDeepCopy() {
            GroupSvgNodeRenderer copy = new GroupSvgNodeRenderer();
            DeepCopyAttributesAndStyles(copy);
            DeepCopyChildren(copy);
            return copy;
        }

        public override Rectangle GetObjectBoundingBox(SvgDrawContext context) {
            return null;
        }
    }
}
