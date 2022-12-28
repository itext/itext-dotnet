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
