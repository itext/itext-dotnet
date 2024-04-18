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
using System;
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Kernel.Exceptions;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Xobject;
using iText.Layout.Element;
using iText.Layout.Exceptions;
using iText.Layout.Renderer;

namespace iText.Layout {
    /// <summary>
    /// This class is used for adding content directly onto a specified
    /// <see cref="iText.Kernel.Pdf.Canvas.PdfCanvas"/>.
    /// </summary>
    /// <remarks>
    /// This class is used for adding content directly onto a specified
    /// <see cref="iText.Kernel.Pdf.Canvas.PdfCanvas"/>.
    /// <see cref="Canvas"/>
    /// does not know the concept of a page, so it can't reflow to a 'next'
    /// <see cref="Canvas"/>.
    /// This class effectively acts as a bridge between the high-level <em>layout</em>
    /// API and the low-level <em>kernel</em> API.
    /// </remarks>
    public class Canvas : RootElement<iText.Layout.Canvas> {
        protected internal PdfCanvas pdfCanvas;

        protected internal Rectangle rootArea;

        /// <summary>
        /// Is initialized and used only when Canvas element autotagging is enabled, see
        /// <see cref="EnableAutoTagging(iText.Kernel.Pdf.PdfPage)"/>.
        /// </summary>
        /// <remarks>
        /// Is initialized and used only when Canvas element autotagging is enabled, see
        /// <see cref="EnableAutoTagging(iText.Kernel.Pdf.PdfPage)"/>.
        /// It is also used to determine if autotagging is enabled.
        /// </remarks>
        protected internal PdfPage page;

        private bool isCanvasOfPage;

        /// <summary>Creates a new Canvas to manipulate a specific page content stream.</summary>
        /// <remarks>
        /// Creates a new Canvas to manipulate a specific page content stream. The given page shall not be flushed:
        /// drawing on flushed pages is impossible because their content is already written to the output stream.
        /// Use this constructor to be able to add
        /// <see cref="iText.Layout.Element.Link"/>
        /// elements on it
        /// (using any other constructor would result in inability to add PDF annotations, based on which, for example, links work).
        /// <para />
        /// If the
        /// <see cref="iText.Kernel.Pdf.PdfDocument.IsTagged()"/>
        /// is true, using this constructor would automatically enable
        /// the tagging for the content. Regarding tagging the effect is the same as using
        /// <see cref="EnableAutoTagging(iText.Kernel.Pdf.PdfPage)"/>.
        /// </remarks>
        /// <param name="page">
        /// the page on which this canvas will be rendered, shall not be flushed (see
        /// <see cref="iText.Kernel.Pdf.PdfObjectWrapper{T}.IsFlushed()"/>
        /// ).
        /// </param>
        /// <param name="rootArea">the maximum area that the Canvas may write upon</param>
        public Canvas(PdfPage page, Rectangle rootArea)
            : this(InitPdfCanvasOrThrowIfPageIsFlushed(page), rootArea) {
            this.EnableAutoTagging(page);
            this.isCanvasOfPage = true;
        }

        /// <summary>
        /// Creates a new Canvas to manipulate a specific content stream, which might be for example a page
        /// or
        /// <see cref="iText.Kernel.Pdf.Xobject.PdfFormXObject"/>
        /// stream.
        /// </summary>
        /// <param name="pdfCanvas">the low-level content stream writer</param>
        /// <param name="rootArea">the maximum area that the Canvas may write upon</param>
        public Canvas(PdfCanvas pdfCanvas, Rectangle rootArea)
            : base() {
            this.pdfDocument = pdfCanvas.GetDocument();
            this.pdfCanvas = pdfCanvas;
            this.rootArea = rootArea;
        }

        /// <summary>Creates a new Canvas to manipulate a specific document and page.</summary>
        /// <param name="pdfCanvas">The low-level content stream writer</param>
        /// <param name="rootArea">The maximum area that the Canvas may write upon</param>
        /// <param name="immediateFlush">Whether to flush the canvas immediately after operations, false otherwise</param>
        public Canvas(PdfCanvas pdfCanvas, Rectangle rootArea, bool immediateFlush)
            : this(pdfCanvas, rootArea) {
            this.immediateFlush = immediateFlush;
        }

        /// <summary>
        /// Creates a new Canvas to manipulate a specific
        /// <see cref="iText.Kernel.Pdf.Xobject.PdfFormXObject"/>.
        /// </summary>
        /// <param name="formXObject">the form</param>
        /// <param name="pdfDocument">the document that the resulting content stream will be written to</param>
        public Canvas(PdfFormXObject formXObject, PdfDocument pdfDocument)
            : this(new PdfCanvas(formXObject, pdfDocument), formXObject.GetBBox().ToRectangle()) {
        }

        /// <summary>
        /// Gets the
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// for this canvas.
        /// </summary>
        /// <returns>the document that the resulting content stream will be written to</returns>
        public virtual PdfDocument GetPdfDocument() {
            return pdfDocument;
        }

        /// <summary>Gets the root area rectangle.</summary>
        /// <returns>the maximum area that the Canvas may write upon</returns>
        public virtual Rectangle GetRootArea() {
            return rootArea;
        }

        /// <summary>
        /// Gets the
        /// <see cref="iText.Kernel.Pdf.Canvas.PdfCanvas"/>.
        /// </summary>
        /// <returns>the low-level content stream writer</returns>
        public virtual PdfCanvas GetPdfCanvas() {
            return pdfCanvas;
        }

        /// <summary>
        /// Sets the
        /// <see cref="iText.Layout.Renderer.IRenderer"/>
        /// for this Canvas.
        /// </summary>
        /// <param name="canvasRenderer">a renderer specific for canvas operations</param>
        public virtual void SetRenderer(CanvasRenderer canvasRenderer) {
            this.rootRenderer = canvasRenderer;
        }

        /// <summary>The page on which this canvas will be rendered.</summary>
        /// <returns>
        /// the specified
        /// <see cref="iText.Kernel.Pdf.PdfPage"/>
        /// instance, might be null if this the page was not set.
        /// </returns>
        public virtual PdfPage GetPage() {
            return page;
        }

        /// <summary>Enables canvas content autotagging.</summary>
        /// <remarks>Enables canvas content autotagging. By default it is disabled.</remarks>
        /// <param name="page">the page, on which this canvas will be rendered.</param>
        public virtual void EnableAutoTagging(PdfPage page) {
            if (IsCanvasOfPage() && this.page != page) {
                ILogger logger = ITextLogManager.GetLogger(typeof(iText.Layout.Canvas));
                logger.LogError(iText.IO.Logs.IoLogMessageConstant.PASSED_PAGE_SHALL_BE_ON_WHICH_CANVAS_WILL_BE_RENDERED);
            }
            this.page = page;
            this.pdfCanvas.SetDrawingOnPage(this.IsAutoTaggingEnabled());
        }

        /// <returns>true if autotagging of canvas content is enabled. Default value - false.</returns>
        public virtual bool IsAutoTaggingEnabled() {
            return page != null;
        }

        /// <summary>Defines if the canvas is exactly the direct content of the page.</summary>
        /// <remarks>
        /// Defines if the canvas is exactly the direct content of the page. This is known definitely only if
        /// this instance was created by
        /// <see cref="Canvas(iText.Kernel.Pdf.PdfPage, iText.Kernel.Geom.Rectangle)"/>
        /// constructor overload,
        /// otherwise this method returns false.
        /// </remarks>
        /// <returns>
        /// true if the canvas on which this instance performs drawing is directly the canvas of the page;
        /// false if the instance of this class was created not with
        /// <see cref="Canvas(iText.Kernel.Pdf.PdfPage, iText.Kernel.Geom.Rectangle)"/>
        /// constructor overload.
        /// </returns>
        public virtual bool IsCanvasOfPage() {
            return isCanvasOfPage;
        }

        /// <summary>
        /// Performs an entire recalculation of the element flow on the canvas,
        /// taking into account all its current child elements.
        /// </summary>
        /// <remarks>
        /// Performs an entire recalculation of the element flow on the canvas,
        /// taking into account all its current child elements. May become very
        /// resource-intensive for large documents.
        /// Do not use when you have set
        /// <see cref="RootElement{T}.immediateFlush"/>
        /// to <c>true</c>.
        /// </remarks>
        public virtual void Relayout() {
            if (immediateFlush) {
                throw new InvalidOperationException("Operation not supported with immediate flush");
            }
            IRenderer nextRelayoutRenderer = rootRenderer != null ? rootRenderer.GetNextRenderer() : null;
            if (nextRelayoutRenderer == null || !(nextRelayoutRenderer is RootRenderer)) {
                nextRelayoutRenderer = new CanvasRenderer(this, immediateFlush);
            }
            rootRenderer = (RootRenderer)nextRelayoutRenderer;
            foreach (IElement element in childElements) {
                CreateAndAddRendererSubTree(element);
            }
        }

        /// <summary>
        /// Forces all registered renderers (including child element renderers) to
        /// flush their contents to the content stream.
        /// </summary>
        public virtual void Flush() {
            rootRenderer.Flush();
        }

        /// <summary>
        /// Closes the
        /// <see cref="Canvas"/>.
        /// </summary>
        /// <remarks>
        /// Closes the
        /// <see cref="Canvas"/>
        /// . Although not completely necessary in all cases, it is still recommended to call this
        /// method when you are done working with
        /// <see cref="Canvas"/>
        /// object, as due to some properties set there might be some
        /// 'hanging' elements, which are waiting other elements to be added and processed.
        /// <see cref="Close()"/>
        /// tells the
        /// <see cref="Canvas"/>
        /// that no more elements will be added and it is time to finish processing all the elements.
        /// </remarks>
        public override void Close() {
            if (rootRenderer != null) {
                rootRenderer.Close();
            }
        }

        protected internal override RootRenderer EnsureRootRendererNotNull() {
            if (rootRenderer == null) {
                rootRenderer = new CanvasRenderer(this, immediateFlush);
            }
            return rootRenderer;
        }

        private static PdfCanvas InitPdfCanvasOrThrowIfPageIsFlushed(PdfPage page) {
            if (page.IsFlushed()) {
                throw new PdfException(LayoutExceptionMessageConstant.CANNOT_DRAW_ELEMENTS_ON_ALREADY_FLUSHED_PAGES);
            }
            return new PdfCanvas(page);
        }
    }
}
