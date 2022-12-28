/*

This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: Bruno Lowagie, Paulo Soares, et al.

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License version 3
as published by the Free Software Foundation with the addition of the
following permission added to Section 15 as permitted in Section 7(a):
FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
OF THIRD PARTY RIGHTS

This program is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU Affero General Public License for more details.
You should have received a copy of the GNU Affero General Public License
along with this program; if not, see http://www.gnu.org/licenses or write to
the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
Boston, MA, 02110-1301 USA, or download the license from the following URL:
http://itextpdf.com/terms-of-use/

The interactive user interfaces in modified source and object code versions
of this program must display Appropriate Legal Notices, as required under
Section 5 of the GNU Affero General Public License.

In accordance with Section 7(b) of the GNU Affero General Public License,
a covered work must retain the producer line in every PDF that is created
or manipulated using iText.

You can be released from the requirements of the license by purchasing
a commercial license. Buying such a license is mandatory as soon as you
develop commercial activities involving the iText software without
disclosing the source code of your own applications.
These activities include: offering paid services to customers as an ASP,
serving PDFs on the fly in a web application, shipping iText with a closed
source product.

For more information, please contact iText Software Corp. at this
address: sales@itextpdf.com
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
