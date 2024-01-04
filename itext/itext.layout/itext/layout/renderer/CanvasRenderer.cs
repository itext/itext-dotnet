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
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Kernel.Pdf.Tagutils;
using iText.Layout;
using iText.Layout.Layout;
using iText.Layout.Properties;

namespace iText.Layout.Renderer {
    /// <summary>
    /// Represents a renderer for the
    /// <see cref="iText.Layout.Canvas"/>
    /// layout element.
    /// </summary>
    public class CanvasRenderer : RootRenderer {
        protected internal Canvas canvas;

        /// <summary>Creates a CanvasRenderer from its corresponding layout object.</summary>
        /// <remarks>
        /// Creates a CanvasRenderer from its corresponding layout object.
        /// Sets
        /// <see cref="RootRenderer.immediateFlush"/>
        /// to true.
        /// </remarks>
        /// <param name="canvas">
        /// the
        /// <see cref="iText.Layout.Canvas"/>
        /// which this object should manage
        /// </param>
        public CanvasRenderer(Canvas canvas)
            : this(canvas, true) {
        }

        /// <summary>Creates a CanvasRenderer from its corresponding layout object.</summary>
        /// <remarks>
        /// Creates a CanvasRenderer from its corresponding layout object.
        /// Defines whether the content should be flushed immediately after addition
        /// <see cref="AddChild(IRenderer)"/>
        /// or not
        /// </remarks>
        /// <param name="canvas">
        /// the
        /// <see cref="iText.Layout.Canvas"/>
        /// which this object should manage
        /// </param>
        /// <param name="immediateFlush">the value which stands for immediate flushing</param>
        public CanvasRenderer(Canvas canvas, bool immediateFlush) {
            this.canvas = canvas;
            this.modelElement = canvas;
            this.immediateFlush = immediateFlush;
        }

        public override void AddChild(IRenderer renderer) {
            if (true.Equals(GetPropertyAsBoolean(Property.FULL))) {
                ITextLogManager.GetLogger(typeof(iText.Layout.Renderer.CanvasRenderer)).LogWarning(iText.IO.Logs.IoLogMessageConstant
                    .CANVAS_ALREADY_FULL_ELEMENT_WILL_BE_SKIPPED);
            }
            else {
                base.AddChild(renderer);
            }
        }

        /// <summary><inheritDoc/></summary>
        protected internal override void FlushSingleRenderer(IRenderer resultRenderer) {
            LinkRenderToDocument(resultRenderer, canvas.GetPdfDocument());
            Transform transformProp = resultRenderer.GetProperty<Transform>(Property.TRANSFORM);
            if (!waitingDrawingElements.Contains(resultRenderer)) {
                ProcessWaitingDrawing(resultRenderer, transformProp, waitingDrawingElements);
                if (FloatingHelper.IsRendererFloating(resultRenderer) || transformProp != null) {
                    return;
                }
            }
            if (!resultRenderer.IsFlushed()) {
                bool toTag = canvas.GetPdfDocument().IsTagged() && canvas.IsAutoTaggingEnabled();
                TagTreePointer tagPointer = null;
                if (toTag) {
                    tagPointer = canvas.GetPdfDocument().GetTagStructureContext().GetAutoTaggingPointer();
                    tagPointer.SetPageForTagging(canvas.GetPage());
                    bool pageStream = false;
                    for (int i = canvas.GetPage().GetContentStreamCount() - 1; i >= 0; --i) {
                        if (canvas.GetPage().GetContentStream(i).Equals(canvas.GetPdfCanvas().GetContentStream())) {
                            pageStream = true;
                            break;
                        }
                    }
                    if (!pageStream) {
                        tagPointer.SetContentStreamForTagging(canvas.GetPdfCanvas().GetContentStream());
                    }
                }
                resultRenderer.Draw(new DrawContext(canvas.GetPdfDocument(), canvas.GetPdfCanvas(), toTag));
                if (toTag) {
                    tagPointer.SetContentStreamForTagging(null);
                }
            }
        }

        /// <summary><inheritDoc/></summary>
        protected internal override LayoutArea UpdateCurrentArea(LayoutResult overflowResult) {
            if (currentArea == null) {
                int pageNumber = canvas.IsCanvasOfPage() ? canvas.GetPdfDocument().GetPageNumber(canvas.GetPage()) : 0;
                currentArea = new RootLayoutArea(pageNumber, canvas.GetRootArea().Clone());
            }
            else {
                SetProperty(Property.FULL, true);
                currentArea = null;
            }
            return currentArea;
        }

        /// <summary>
        /// For
        /// <see cref="CanvasRenderer"/>
        /// , this has a meaning of the renderer that will be used for relayout.
        /// </summary>
        /// <returns>relayout renderer.</returns>
        public override IRenderer GetNextRenderer() {
            return new iText.Layout.Renderer.CanvasRenderer(canvas, immediateFlush);
        }
    }
}
