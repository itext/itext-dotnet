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
    /// <summary>This renderer represents a collection of elements (simple shapes and paths).</summary>
    /// <remarks>
    /// This renderer represents a collection of elements (simple shapes and paths).
    /// The elements are not drawn visibly, but the union of their shapes will be used
    /// to only show the parts of the drawn objects that fall within the clipping path.
    /// In PDF, the clipping path operators use the intersection of all its elements, not the union (as in SVG);
    /// thus, we need to draw the clipped elements multiple times if the clipping path consists of multiple elements.
    /// </remarks>
    public class ClipPathSvgNodeRenderer : AbstractBranchSvgNodeRenderer {
        private AbstractSvgNodeRenderer clippedRenderer;

        public override ISvgNodeRenderer CreateDeepCopy() {
            AbstractBranchSvgNodeRenderer copy = new ClipPathSvgNodeRenderer();
            DeepCopyAttributesAndStyles(copy);
            DeepCopyChildren(copy);
            return copy;
        }

        public override Rectangle GetObjectBoundingBox(SvgDrawContext context) {
            return null;
        }

//\cond DO_NOT_DOCUMENT
        internal override void PreDraw(SvgDrawContext context) {
        }
//\endcond

        protected internal override void DoDraw(SvgDrawContext context) {
            PdfCanvas currentCanvas = context.GetCurrentCanvas();
            foreach (ISvgNodeRenderer child in GetChildren()) {
                currentCanvas.SaveState();
                if (child is AbstractSvgNodeRenderer) {
                    ((AbstractSvgNodeRenderer)child).SetPartOfClipPath(true);
                }
                child.Draw(context);
                if (child is AbstractSvgNodeRenderer) {
                    ((AbstractSvgNodeRenderer)child).SetPartOfClipPath(false);
                }
                if (clippedRenderer != null) {
                    clippedRenderer.PreDraw(context);
                    clippedRenderer.DoDraw(context);
                    clippedRenderer.PostDraw(context);
                }
                currentCanvas.RestoreState();
            }
        }

        public virtual void SetClippedRenderer(AbstractSvgNodeRenderer clippedRenderer) {
            this.clippedRenderer = clippedRenderer;
        }
    }
}
