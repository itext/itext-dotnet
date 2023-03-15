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
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Commons.Utils;
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Canvas.Draw;
using iText.Layout.Element;
using iText.Layout.Layout;
using iText.Layout.Properties;

namespace iText.Layout.Renderer {
    public class TabRenderer : AbstractRenderer {
        /// <summary>Creates a TabRenderer from its corresponding layout object</summary>
        /// <param name="tab">
        /// the
        /// <see cref="iText.Layout.Element.Tab"/>
        /// which this object should manage
        /// </param>
        public TabRenderer(Tab tab)
            : base(tab) {
        }

        public override LayoutResult Layout(LayoutContext layoutContext) {
            LayoutArea area = layoutContext.GetArea();
            float? width = RetrieveWidth(area.GetBBox().GetWidth());
            UnitValue height = this.GetProperty<UnitValue>(Property.MIN_HEIGHT);
            occupiedArea = new LayoutArea(area.GetPageNumber(), new Rectangle(area.GetBBox().GetX(), area.GetBBox().GetY
                () + area.GetBBox().GetHeight(), (float)width, (float)height.GetValue()));
            TargetCounterHandler.AddPageByID(this);
            return new LayoutResult(LayoutResult.FULL, occupiedArea, null, null);
        }

        public override void Draw(DrawContext drawContext) {
            if (occupiedArea == null) {
                ILogger logger = ITextLogManager.GetLogger(typeof(iText.Layout.Renderer.TabRenderer));
                logger.LogError(MessageFormatUtil.Format(iText.IO.Logs.IoLogMessageConstant.OCCUPIED_AREA_HAS_NOT_BEEN_INITIALIZED
                    , "Drawing won't be performed."));
                return;
            }
            ILineDrawer leader = this.GetProperty<ILineDrawer>(Property.TAB_LEADER);
            if (leader == null) {
                return;
            }
            bool isTagged = drawContext.IsTaggingEnabled();
            if (isTagged) {
                drawContext.GetCanvas().OpenTag(new CanvasArtifact());
            }
            BeginElementOpacityApplying(drawContext);
            leader.Draw(drawContext.GetCanvas(), occupiedArea.GetBBox());
            EndElementOpacityApplying(drawContext);
            if (isTagged) {
                drawContext.GetCanvas().CloseTag();
            }
        }

        /// <summary>
        /// Gets a new instance of this class to be used as a next renderer, after this renderer is used, if
        /// <see cref="Layout(iText.Layout.Layout.LayoutContext)"/>
        /// is called more than once.
        /// </summary>
        /// <remarks>
        /// Gets a new instance of this class to be used as a next renderer, after this renderer is used, if
        /// <see cref="Layout(iText.Layout.Layout.LayoutContext)"/>
        /// is called more than once.
        /// <para />
        /// If a renderer overflows to the next area, iText uses this method to create a renderer
        /// for the overflow part. So if one wants to extend
        /// <see cref="TabRenderer"/>
        /// , one should override
        /// this method: otherwise the default method will be used and thus the default rather than the custom
        /// renderer will be created.
        /// </remarks>
        /// <returns>new renderer instance</returns>
        public override IRenderer GetNextRenderer() {
            LogWarningIfGetNextRendererNotOverridden(typeof(iText.Layout.Renderer.TabRenderer), this.GetType());
            return new iText.Layout.Renderer.TabRenderer((Tab)modelElement);
        }
    }
}
