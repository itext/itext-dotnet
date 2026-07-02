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
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Commons.Datastructures;
using iText.Commons.Internal.Runtime;
using iText.Kernel.Exceptions;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout.Element;
using iText.Layout.Exceptions;
using iText.Layout.Logs;
using iText.Layout.Properties;
using iText.Layout.Properties.Margins;
using iText.Layout.Renderer;

namespace iText.Layout {
    /// <summary>Document is the default root element when creating a self-sufficient PDF.</summary>
    /// <remarks>
    /// Document is the default root element when creating a self-sufficient PDF. It
    /// mainly operates high-level operations e.g. setting page size and rotation,
    /// adding elements, and writing text at specific coordinates. It has no
    /// knowledge of the actual PDF concepts and syntax.
    /// <para />
    /// A
    /// <see cref="Document"/>
    /// 's rendering behavior can be modified by extending
    /// <see cref="iText.Layout.Renderer.DocumentRenderer"/>
    /// and setting an instance of this newly created with
    /// <see cref="SetRenderer(iText.Layout.Renderer.DocumentRenderer)"></see>.
    /// </remarks>
    public class Document : RootElement<iText.Layout.Document> {
        private static readonly ILogger LOGGER = ITextLogManager.GetLogger(typeof(iText.Layout.Document));

        private readonly IDictionary<int, PageMarginBoxes> pageMargins = new Dictionary<int, PageMarginBoxes>();

        private readonly IList<Tuple2<Predicate<int>, PageMarginBoxes>> pageMarginsRules = new List<Tuple2<Predicate
            <int>, PageMarginBoxes>>();

        private readonly IList<Func<int, PageMarginBoxes>> pageMarginsFunctions = new List<Func<int, PageMarginBoxes
            >>();

        /// <summary>
        /// Creates a document from a
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>.
        /// </summary>
        /// <remarks>
        /// Creates a document from a
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// . Initializes the first page
        /// with the
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// 's current default
        /// <see cref="iText.Kernel.Geom.PageSize"/>.
        /// </remarks>
        /// <param name="pdfDoc">the in-memory representation of the PDF document</param>
        public Document(PdfDocument pdfDoc)
            : this(pdfDoc, pdfDoc.GetDefaultPageSize()) {
        }

        /// <summary>
        /// Creates a document from a
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// with a manually set
        /// <see cref="iText.Kernel.Geom.PageSize"/>.
        /// </summary>
        /// <param name="pdfDoc">the in-memory representation of the PDF document</param>
        /// <param name="pageSize">the page size</param>
        public Document(PdfDocument pdfDoc, PageSize pageSize)
            : this(pdfDoc, pageSize, true) {
        }

        /// <summary>
        /// Creates a document from a
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// with a manually set
        /// <see cref="iText.Kernel.Geom.PageSize"/>.
        /// </summary>
        /// <param name="pdfDoc">the in-memory representation of the PDF document</param>
        /// <param name="pageSize">the page size</param>
        /// <param name="immediateFlush">
        /// if true, write pages and page-related instructions
        /// to the
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// as soon as possible.
        /// </param>
        public Document(PdfDocument pdfDoc, PageSize pageSize, bool immediateFlush)
            : base() {
            this.pdfDocument = pdfDoc;
            this.pdfDocument.SetDefaultPageSize(pageSize);
            this.immediateFlush = immediateFlush;
            this.EnsureRootTagIsCreated();
        }

        /// <summary>Closes the document and associated PdfDocument.</summary>
        public override void Close() {
            if (rootRenderer != null) {
                rootRenderer.Close();
            }
            pdfDocument.Close();
        }

        /// <summary>Terminates the current element, usually a page.</summary>
        /// <remarks>
        /// Terminates the current element, usually a page. Sets the next element
        /// to be the size specified in the argument.
        /// </remarks>
        /// <param name="areaBreak">
        /// an
        /// <see cref="iText.Layout.Element.AreaBreak"/>
        /// , optionally with a specified size
        /// </param>
        /// <returns>this element</returns>
        public virtual iText.Layout.Document Add(AreaBreak areaBreak) {
            CheckClosingStatus();
            childElements.Add(areaBreak);
            EnsureRootRendererNotNull().AddChild(areaBreak.CreateRendererSubTree());
            if (immediateFlush) {
                childElements.JRemoveAt(childElements.Count - 1);
            }
            return this;
        }

        /// <summary>Terminates the current page if it's not the first one in the document.</summary>
        /// <remarks>
        /// Terminates the current page if it's not the first one in the document.
        /// Sets the page size and/or page margins specified in the arguments for the next page.
        /// </remarks>
        /// <param name="sectionBreak">
        /// 
        /// <see cref="iText.Layout.Element.SectionBreak"/>
        /// , optionally with a specified page size and/or page margins
        /// </param>
        /// <returns>
        /// this same
        /// <see cref="Document"/>
        /// instance
        /// </returns>
        public virtual iText.Layout.Document Add(SectionBreak sectionBreak) {
            CheckClosingStatus();
            childElements.Add(sectionBreak);
            EnsureRootRendererNotNull().AddChild(sectionBreak.CreateRendererSubTree());
            if (immediateFlush) {
                childElements.JRemoveAt(childElements.Count - 1);
            }
            return this;
        }

        public override iText.Layout.Document Add(IBlockElement element) {
            CheckClosingStatus();
            base.Add(element);
            if (element is ILargeElement) {
                ((ILargeElement)element).SetDocument(this);
                ((ILargeElement)element).FlushContent();
            }
            return this;
        }

        /// <summary>Gets PDF document.</summary>
        /// <returns>the in-memory representation of the PDF document</returns>
        public virtual PdfDocument GetPdfDocument() {
            return pdfDocument;
        }

        /// <summary>
        /// Changes the
        /// <see cref="iText.Layout.Renderer.DocumentRenderer"/>
        /// at runtime.
        /// </summary>
        /// <remarks>
        /// Changes the
        /// <see cref="iText.Layout.Renderer.DocumentRenderer"/>
        /// at runtime. Use this to customize
        /// the Document's
        /// <see cref="iText.Layout.Renderer.IRenderer"/>
        /// behavior.
        /// </remarks>
        /// <param name="documentRenderer">the DocumentRenderer to set</param>
        public virtual void SetRenderer(DocumentRenderer documentRenderer) {
            this.rootRenderer = documentRenderer;
        }

        /// <summary>
        /// Forces all registered renderers (including child element renderers) to
        /// flush their contents to the content stream.
        /// </summary>
        public virtual void Flush() {
            rootRenderer.Flush();
        }

        /// <summary>
        /// Performs an entire recalculation of the document flow, taking into
        /// account all its current child elements.
        /// </summary>
        /// <remarks>
        /// Performs an entire recalculation of the document flow, taking into
        /// account all its current child elements. May become very
        /// resource-intensive for large documents.
        /// <para />
        /// Do not use when you have set
        /// <see cref="RootElement{T}.immediateFlush"/>
        /// to <c>true</c>.
        /// </remarks>
        public virtual void Relayout() {
            if (immediateFlush) {
                throw new InvalidOperationException("Operation not supported with immediate flush");
            }
            if (rootRenderer is DocumentRenderer) {
                ((DocumentRenderer)rootRenderer).GetTargetCounterHandler().PrepareHandlerToRelayout();
            }
            IRenderer nextRelayoutRenderer = rootRenderer != null ? rootRenderer.GetNextRenderer() : null;
            if (nextRelayoutRenderer == null || !(nextRelayoutRenderer is RootRenderer)) {
                nextRelayoutRenderer = new DocumentRenderer(this, immediateFlush);
            }
            // Even though #relayout() only makes sense when immediateFlush=false and therefore no elements
            // should have been written to document, still empty pages are created during layout process
            // because we need to know the effective page size which may differ from page to page.
            // Therefore, we remove all the pages that might have been created before proceeding to relayout elements.
            while (pdfDocument.GetNumberOfPages() > 0) {
                pdfDocument.RemovePage(pdfDocument.GetNumberOfPages());
            }
            rootRenderer = (RootRenderer)nextRelayoutRenderer;
            foreach (IElement element in childElements) {
                CreateAndAddRendererSubTree(element);
            }
        }

        /// <summary>Gets the left margin, measured in points</summary>
        /// <returns>a <c>float</c> containing the left margin value</returns>
        public virtual float GetLeftMargin() {
            float? property = this.GetProperty<float?>(Property.MARGIN_LEFT);
            return (float)(property != null ? property : this.GetDefaultProperty<float>(Property.MARGIN_LEFT));
        }

        /// <summary>Sets the left margin, measured in points</summary>
        /// <param name="leftMargin">a <c>float</c> containing the new left margin value</param>
        public virtual void SetLeftMargin(float leftMargin) {
            SetProperty(Property.MARGIN_LEFT, leftMargin);
        }

        /// <summary>Gets the right margin, measured in points</summary>
        /// <returns>a <c>float</c> containing the right margin value</returns>
        public virtual float GetRightMargin() {
            float? property = this.GetProperty<float?>(Property.MARGIN_RIGHT);
            return (float)(property != null ? property : this.GetDefaultProperty<float>(Property.MARGIN_RIGHT));
        }

        /// <summary>Sets the right margin, measured in points</summary>
        /// <param name="rightMargin">a <c>float</c> containing the new right margin value</param>
        public virtual void SetRightMargin(float rightMargin) {
            SetProperty(Property.MARGIN_RIGHT, rightMargin);
        }

        /// <summary>Gets the top margin, measured in points</summary>
        /// <returns>a <c>float</c> containing the top margin value</returns>
        public virtual float GetTopMargin() {
            float? property = this.GetProperty<float?>(Property.MARGIN_TOP);
            return (float)(property != null ? property : this.GetDefaultProperty<float>(Property.MARGIN_TOP));
        }

        /// <summary>Sets the top margin, measured in points</summary>
        /// <param name="topMargin">a <c>float</c> containing the new top margin value</param>
        public virtual void SetTopMargin(float topMargin) {
            SetProperty(Property.MARGIN_TOP, topMargin);
        }

        /// <summary>Gets the bottom margin, measured in points</summary>
        /// <returns>a <c>float</c> containing the bottom margin value</returns>
        public virtual float GetBottomMargin() {
            float? property = this.GetProperty<float?>(Property.MARGIN_BOTTOM);
            return (float)(property != null ? property : this.GetDefaultProperty<float>(Property.MARGIN_BOTTOM));
        }

        /// <summary>Sets the bottom margin, measured in points</summary>
        /// <param name="bottomMargin">a <c>float</c> containing the new bottom margin value</param>
        public virtual void SetBottomMargin(float bottomMargin) {
            SetProperty(Property.MARGIN_BOTTOM, bottomMargin);
        }

        /// <summary>Convenience method to set all margins with one method.</summary>
        /// <param name="topMargin">the upper margin</param>
        /// <param name="rightMargin">the right margin</param>
        /// <param name="leftMargin">the left margin</param>
        /// <param name="bottomMargin">the lower margin</param>
        public virtual void SetMargins(float topMargin, float rightMargin, float bottomMargin, float leftMargin) {
            SetTopMargin(topMargin);
            SetRightMargin(rightMargin);
            SetBottomMargin(bottomMargin);
            SetLeftMargin(leftMargin);
        }

        /// <summary>Gets page margins by specified page number.</summary>
        /// <param name="pageNumber">number of the page to get margins for</param>
        /// <returns>
        /// 
        /// <see cref="iText.Layout.Properties.Margins.PageMarginBoxes"/>
        /// page margins
        /// </returns>
        public virtual PageMarginBoxes GetPageMargins(int pageNumber) {
            if (pageMargins.ContainsKey(pageNumber)) {
                return pageMargins.Get(pageNumber);
            }
            foreach (Tuple2<Predicate<int>, PageMarginBoxes> rule in pageMarginsRules) {
                if (rule.GetFirst()(pageNumber)) {
                    PageMarginBoxes pageMarginBoxes = rule.GetSecond();
                    return pageMarginBoxes != null ? new PageMarginBoxes(pageMarginBoxes) : null;
                }
            }
            foreach (Func<int, PageMarginBoxes> function in pageMarginsFunctions) {
                PageMarginBoxes pageMarginBoxes = function.Invoke(pageNumber);
                if (pageMarginBoxes != null) {
                    return pageMarginBoxes;
                }
            }
            return null;
        }

        /// <summary>Sets page margins for page with provided number.</summary>
        /// <param name="pageNumber">number of the page to set margins for</param>
        /// <param name="margins">
        /// 
        /// <see cref="iText.Layout.Properties.Margins.PageMarginBoxes"/>
        /// page margins to set
        /// </param>
        /// <returns>
        /// this same
        /// <see cref="Document"/>
        /// instance
        /// </returns>
        public virtual iText.Layout.Document SetPageMargins(int pageNumber, PageMarginBoxes margins) {
            pageMargins.Put(pageNumber, margins);
            return this;
        }

        /// <summary>Sets page margins for page based on provided condition for page number.</summary>
        /// <param name="condition">matching rule with page number as argument</param>
        /// <param name="margins">
        /// 
        /// <see cref="iText.Layout.Properties.Margins.PageMarginBoxes"/>
        /// page margins to set
        /// </param>
        /// <returns>
        /// this same
        /// <see cref="Document"/>
        /// instance
        /// </returns>
        public virtual iText.Layout.Document SetPageMargins(Predicate<int> condition, PageMarginBoxes margins) {
            pageMarginsRules.Add(new Tuple2<Predicate<int>, PageMarginBoxes>(condition, margins));
            return this;
        }

        /// <summary>Sets page margins for page based on provided function for page number.</summary>
        /// <param name="function">
        /// function with page number as argument, return
        /// <see langword="null"/>
        /// in case result should be ignored
        /// and
        /// <see cref="iText.Layout.Properties.Margins.PageMarginBoxes"/>
        /// page margins return value
        /// </param>
        /// <returns>
        /// this same
        /// <see cref="Document"/>
        /// instance
        /// </returns>
        public virtual iText.Layout.Document SetPageMargins(Func<int, PageMarginBoxes> function) {
            this.pageMarginsFunctions.Add(function);
            return this;
        }

        /// <summary>Checks whether page margins have been specified for the given page number.</summary>
        /// <remarks>
        /// Checks whether page margins have been specified for the given page number.
        /// <para />
        /// This method returns
        /// <see langword="true"/>
        /// if the margins for the page are determined by
        /// any of the following mechanisms (in order of precedence):
        /// <list type="number">
        /// <item><description>Explicitly set margins for page number via
        /// <see cref="SetPageMargins(int, iText.Layout.Properties.Margins.PageMarginBoxes)"/>
        /// </description></item>
        /// <item><description>Matching rule set via
        /// <see cref="SetPageMargins(System.Predicate{T}, iText.Layout.Properties.Margins.PageMarginBoxes)"/>
        /// </description></item>
        /// <item><description>Function set via
        /// <see cref="SetPageMargins(System.Func{T, R})"/>
        /// </description></item>
        /// </list>
        /// <para />
        /// NOTE: the method returns
        /// <see langword="true"/>
        /// even if the value produced by the mechanisms is
        /// <see langword="null"/>.
        /// Only when none of the above apply and the default static margins are used, this method returns
        /// <see langword="false"/>.
        /// </remarks>
        /// <param name="pageNumber">the page number to check</param>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if margins for the page are defined explicitly by page number,
        /// by matching rule or by function,
        /// <see langword="false"/>
        /// otherwise
        /// </returns>
        public virtual bool IsPageMarginsSpecified(int pageNumber) {
            if (pageMargins.ContainsKey(pageNumber)) {
                return true;
            }
            foreach (Tuple2<Predicate<int>, PageMarginBoxes> rule in pageMarginsRules) {
                if (rule.GetFirst()(pageNumber)) {
                    return true;
                }
            }
            foreach (Func<int, PageMarginBoxes> function in pageMarginsFunctions) {
                PageMarginBoxes pageMarginBoxes = function.Invoke(pageNumber);
                if (pageMarginBoxes != null) {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Gets
        /// <see cref="iText.Layout.Properties.Margins.FootnotesProperties"/>
        /// specified for the document to customize footnotes.
        /// </summary>
        /// <returns>
        /// 
        /// <see cref="iText.Layout.Properties.Margins.FootnotesProperties"/>
        /// specified for the document
        /// </returns>
        public virtual FootnotesProperties GetFootnotesProperties() {
            FootnotesProperties property = this.GetProperty<FootnotesProperties>(Property.FOOTNOTES_PROPERTIES);
            return property != null ? property : this.GetDefaultProperty<FootnotesProperties>(Property.FOOTNOTES_PROPERTIES
                );
        }

        /// <summary>
        /// Sets
        /// <see cref="iText.Layout.Properties.Margins.FootnotesProperties"/>
        /// for the document.
        /// </summary>
        /// <param name="footnotesProperties">
        /// 
        /// <see cref="iText.Layout.Properties.Margins.FootnotesProperties"/>
        /// to customize footnotes
        /// </param>
        public virtual void SetFootnotesProperties(FootnotesProperties footnotesProperties) {
            FootnotesProperties currentProperties = this.GetFootnotesProperties();
            FootnoteNumberingConfig footnoteNumberingConfig = currentProperties.GetFootnoteNumberingConfig();
            if (footnotesProperties != null) {
                if (FootnoteNumberingConfig.PER_DOCUMENT == footnotesProperties.GetFootnoteNumberingConfig()) {
                    if (this.HasOwnProperty(Property.FOOTNOTES_PROPERTIES) && FootnoteNumberingConfig.PER_DOCUMENT != footnoteNumberingConfig
                        ) {
                        LOGGER.LogWarning(LayoutLogMessageConstant.FOOTNOTE_NUM_PER_DOCUMENT_SHOULD_BE_FIRST);
                        footnotesProperties.SetFootnoteNumberingConfig(footnoteNumberingConfig);
                    }
                }
                else {
                    if (FootnoteNumberingConfig.PER_DOCUMENT == footnoteNumberingConfig) {
                        LOGGER.LogWarning(LayoutLogMessageConstant.FOOTNOTE_NUM_PER_DOCUMENT_CANNOT_BE_CHANGED);
                        footnotesProperties.SetFootnoteNumberingConfig(FootnoteNumberingConfig.PER_DOCUMENT);
                    }
                }
            }
            this.SetProperty(Property.FOOTNOTES_PROPERTIES, footnotesProperties);
        }

        /// <summary>
        /// Returns the area that will actually be used to write on the page, given
        /// the current margins.
        /// </summary>
        /// <remarks>
        /// Returns the area that will actually be used to write on the page, given
        /// the current margins. Does not have any side effects on the document.
        /// </remarks>
        /// <param name="pageSize">the size of the page to</param>
        /// <returns>
        /// a
        /// <see cref="iText.Kernel.Geom.Rectangle"/>
        /// with the required dimensions and origin point
        /// </returns>
        public virtual Rectangle GetPageEffectiveArea(PageSize pageSize) {
            float x = pageSize.GetLeft() + GetLeftMargin();
            float y = pageSize.GetBottom() + GetBottomMargin();
            float width = pageSize.GetWidth() - GetLeftMargin() - GetRightMargin();
            float height = pageSize.GetHeight() - GetBottomMargin() - GetTopMargin();
            return new Rectangle(x, y, width, height);
        }

        public override T1 GetDefaultProperty<T1>(int property) {
            switch (property) {
                case Property.FONT: {
                    if (GetPdfDocument().GetConformance() != null && GetPdfDocument().GetConformance().ConformsToAny()) {
                        return (T1)(Object)GetPdfDocument().GetDefaultFont();
                    }
                    return base.GetDefaultProperty<T1>(property);
                }

                case Property.MARGIN_BOTTOM:
                case Property.MARGIN_LEFT:
                case Property.MARGIN_RIGHT:
                case Property.MARGIN_TOP: {
                    return (T1)(Object)36f;
                }

                case Property.FOOTNOTES_PROPERTIES: {
                    return (T1)(Object)new FootnotesProperties();
                }

                default: {
                    return base.GetDefaultProperty<T1>(property);
                }
            }
        }

        protected internal override RootRenderer EnsureRootRendererNotNull() {
            if (rootRenderer == null) {
                rootRenderer = new DocumentRenderer(this, immediateFlush);
            }
            return rootRenderer;
        }

        /// <summary>Checks whether a method is invoked at the closed document</summary>
        protected internal virtual void CheckClosingStatus() {
            if (GetPdfDocument().IsClosed()) {
                throw new PdfException(LayoutExceptionMessageConstant.DOCUMENT_CLOSED_IT_IS_IMPOSSIBLE_TO_EXECUTE_ACTION);
            }
        }
    }
}
