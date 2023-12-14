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
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Canvas.Draw;
using iText.Layout.Element;
using iText.Layout.Layout;
using iText.Layout.Properties;

namespace iText.Layout.Renderer {
    public class LineSeparatorRenderer : BlockRenderer {
        /// <summary>Creates a LineSeparatorRenderer from its corresponding layout object.</summary>
        /// <param name="lineSeparator">
        /// the
        /// <see cref="iText.Layout.Element.LineSeparator"/>
        /// which this object should manage
        /// </param>
        public LineSeparatorRenderer(LineSeparator lineSeparator)
            : base(lineSeparator) {
        }

        /// <summary><inheritDoc/></summary>
        public override LayoutResult Layout(LayoutContext layoutContext) {
            Rectangle parentBBox = layoutContext.GetArea().GetBBox().Clone();
            if (this.GetProperty<float?>(Property.ROTATION_ANGLE) != null) {
                parentBBox.MoveDown(AbstractRenderer.INF - parentBBox.GetHeight()).SetHeight(AbstractRenderer.INF);
            }
            ILineDrawer lineDrawer = this.GetProperty<ILineDrawer>(Property.LINE_DRAWER);
            float height = lineDrawer != null ? lineDrawer.GetLineWidth() : 0;
            occupiedArea = new LayoutArea(layoutContext.GetArea().GetPageNumber(), parentBBox.Clone());
            ApplyMargins(occupiedArea.GetBBox(), false);
            float? calculatedWidth = RetrieveWidth(layoutContext.GetArea().GetBBox().GetWidth());
            if (calculatedWidth == null) {
                calculatedWidth = occupiedArea.GetBBox().GetWidth();
            }
            if ((occupiedArea.GetBBox().GetHeight() < height || occupiedArea.GetBBox().GetWidth() < calculatedWidth) &&
                 !HasOwnProperty(Property.FORCED_PLACEMENT)) {
                return new LayoutResult(LayoutResult.NOTHING, null, null, this, this);
            }
            occupiedArea.GetBBox().SetWidth((float)calculatedWidth).MoveUp(occupiedArea.GetBBox().GetHeight() - height
                ).SetHeight(height);
            ApplyMargins(occupiedArea.GetBBox(), true);
            if (this.GetProperty<float?>(Property.ROTATION_ANGLE) != null) {
                ApplyRotationLayout(layoutContext.GetArea().GetBBox().Clone());
                if (IsNotFittingLayoutArea(layoutContext.GetArea())) {
                    if (!true.Equals(GetPropertyAsBoolean(Property.FORCED_PLACEMENT))) {
                        return new LayoutResult(LayoutResult.NOTHING, null, null, this, this);
                    }
                }
            }
            return new LayoutResult(LayoutResult.FULL, occupiedArea, this, null);
        }

        /// <summary><inheritDoc/></summary>
        public override IRenderer GetNextRenderer() {
            return new iText.Layout.Renderer.LineSeparatorRenderer((LineSeparator)modelElement);
        }

        /// <summary><inheritDoc/></summary>
        public override void DrawChildren(DrawContext drawContext) {
            ILineDrawer lineDrawer = this.GetProperty<ILineDrawer>(Property.LINE_DRAWER);
            if (lineDrawer != null) {
                PdfCanvas canvas = drawContext.GetCanvas();
                bool isTagged = drawContext.IsTaggingEnabled();
                if (isTagged) {
                    canvas.OpenTag(new CanvasArtifact());
                }
                Rectangle area = GetOccupiedAreaBBox();
                ApplyMargins(area, false);
                lineDrawer.Draw(canvas, area);
                if (isTagged) {
                    canvas.CloseTag();
                }
            }
        }
    }
}
