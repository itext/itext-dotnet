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
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Canvas;
using iText.StyledXmlParser.Css;
using iText.Svg.Logs;
using iText.Svg.Renderers;

namespace iText.Svg.Renderers.Impl {
    /// <summary>This renderer represents a collection of elements (simple shapes and paths).</summary>
    /// <remarks>
    /// This renderer represents a collection of elements (simple shapes and paths).
    /// The elements are not drawn visibly, but the union of their shapes will be used
    /// to only show the parts of the drawn objects that fall within the clipping path.
    /// <para />
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
            if (clippedRenderer == null) {
                // clipPath element is applicable only for some particular elements, without it, no drawing needed
                return;
            }
            PdfCanvas currentCanvas = context.GetCurrentCanvas();
            foreach (ISvgNodeRenderer child in GetChildren()) {
                if (child is AbstractSvgNodeRenderer && ((AbstractSvgNodeRenderer)child).IsHidden()) {
                    continue;
                }
                currentCanvas.SaveState();
                child.SetParent(this);
                child.Draw(context);
                if (!(child is TextSvgBranchRenderer)) {
                    // TextSvgBranchRenderer by itself will call drawClippedRenderer after each sub-element drawing
                    DrawClippedRenderer(context);
                }
                if (!context.GetClippingElementTransform().IsIdentity()) {
                    context.ResetClippingElementTransform();
                }
                currentCanvas.RestoreState();
            }
        }

        /// <summary>Draw the clipped renderer.</summary>
        /// <param name="context">the context on which clipped renderer will be drawn</param>
        public virtual void DrawClippedRenderer(SvgDrawContext context) {
            if (!context.GetClippingElementTransform().IsIdentity()) {
                try {
                    context.GetCurrentCanvas().ConcatMatrix(context.GetClippingElementTransform().CreateInverse());
                }
                catch (NoninvertibleTransformException) {
                    ILogger logger = ITextLogManager.GetLogger(typeof(ClipPathSvgNodeRenderer));
                    logger.LogWarning(SvgLogMessageConstant.NONINVERTIBLE_TRANSFORMATION_MATRIX_USED_IN_CLIP_PATH);
                }
            }
            clippedRenderer.PreDraw(context);
            clippedRenderer.DoDraw(context);
            clippedRenderer.PostDraw(context);
        }

        // Returning canvas matrix to its original state isn't required
        // because after drawClippedRenderer graphic state will be restored
        /// <summary>Sets the clipped renderer.</summary>
        /// <param name="clippedRenderer">the clipped renderer</param>
        public virtual void SetClippedRenderer(AbstractSvgNodeRenderer clippedRenderer) {
            this.clippedRenderer = clippedRenderer;
        }

        protected internal override bool IsHidden() {
            return CommonCssConstants.NONE.Equals(this.attributesAndStyles.Get(CommonCssConstants.DISPLAY)) && !CommonCssConstants
                .HIDDEN.Equals(this.attributesAndStyles.Get(CommonCssConstants.VISIBILITY));
        }
    }
}
