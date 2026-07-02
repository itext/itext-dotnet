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
using iText.Commons.Internal.Runtime;
using iText.Commons.Utils;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Tagging;
using iText.Kernel.Pdf.Tagutils;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Layout;
using iText.Layout.Logs;
using iText.Layout.Minmaxwidth;
using iText.Layout.Properties;
using iText.Layout.Renderer;
using iText.Layout.Tagging;

namespace iText.Layout.Properties.Margins {
    /// <summary>Class to store information about all page margin boxes for a single page.</summary>
    public class PageMarginBoxes {
        private static readonly ILogger LOGGER = ITextLogManager.GetLogger(typeof(iText.Layout.Properties.Margins.PageMarginBoxes
            ));

        private readonly IDictionary<MarginBoxName, PageMarginContent> margins = new LinkedDictionary<MarginBoxName
            , PageMarginContent>();

        private readonly IDictionary<int, PageFootnotesContent> footnotes = new LinkedDictionary<int, PageFootnotesContent
            >();

        private float[] marginSizes = new float[4];

        /// <summary>
        /// Creates new
        /// <see cref="PageMarginBoxes"/>
        /// instance.
        /// </summary>
        /// <param name="margins">
        /// list of
        /// <see cref="PageMarginContent"/>
        /// instances representing page margin content
        /// for corresponding margin box name
        /// <see cref="MarginBoxName"/>
        /// (position on the page)
        /// </param>
        public PageMarginBoxes(IList<PageMarginContent> margins) {
            foreach (PageMarginContent margin in margins) {
                this.margins.Put(margin.GetMarginBoxName(), margin);
            }
        }

        /// <summary>
        /// Creates new
        /// <see cref="PageMarginBoxes"/>
        /// instance by copying existing one.
        /// </summary>
        /// <param name="other">
        /// 
        /// <see cref="PageMarginBoxes"/>
        /// to copy
        /// </param>
        public PageMarginBoxes(iText.Layout.Properties.Margins.PageMarginBoxes other) {
            foreach (KeyValuePair<MarginBoxName, PageMarginContent> margin in other.margins) {
                this.margins.Put(margin.Key, new PageMarginContent(margin.Value));
            }
            foreach (KeyValuePair<int, PageFootnotesContent> footnote in other.footnotes) {
                this.footnotes.Put(footnote.Key, new PageFootnotesContent(footnote.Value));
            }
            this.marginSizes = other.marginSizes;
        }

        /// <summary>Draws all page margins for the page specified via page number.</summary>
        /// <param name="marginRenderer">renderer for the margin content to draw</param>
        /// <param name="marginRect">page margin box rectangle</param>
        /// <param name="documentRenderer">document renderer to use as parent for margin renderer</param>
        /// <param name="document">
        /// 
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// to which content is written
        /// </param>
        /// <param name="pageNumber">page number</param>
        /// <param name="marginBoxName">string value representing margin box name (defining its position on the page)</param>
        public static void Draw(IRenderer marginRenderer, Rectangle marginRect, DocumentRenderer documentRenderer, 
            PdfDocument document, int pageNumber, String marginBoxName) {
            PdfPage page = document.GetPage(pageNumber);
            LayoutResult result = marginRenderer.SetParent(documentRenderer).Layout(new LayoutContext(new LayoutArea(pageNumber
                , marginRect)));
            IRenderer rendererToDraw = result.GetStatus() == LayoutResult.FULL ? marginRenderer : result.GetSplitRenderer
                ();
            if (rendererToDraw == null) {
                // Margin box elements have overflow property set to HIDDEN, therefore it is expected to neither get
                // LayoutResult other than FULL nor get no split renderer (result NOTHING) even if result is not FULL.
                LOGGER.LogError(MessageFormatUtil.Format(LayoutLogMessageConstant.PAGE_CONTENT_CANNOT_BE_DRAWN, marginBoxName
                    , pageNumber));
                return;
            }
            TagTreePointer tagPointer = null;
            TagTreePointer backupPointer = null;
            PdfPage backupPage = null;
            if (document.IsTagged()) {
                tagPointer = document.GetTagStructureContext().GetAutoTaggingPointer();
                backupPage = tagPointer.GetCurrentPage();
                backupPointer = new TagTreePointer(tagPointer);
                tagPointer.MoveToRoot();
                tagPointer.SetPageForTagging(page);
            }
            rendererToDraw.SetParent(documentRenderer).Draw(new DrawContext(page.GetDocument(), new PdfCanvas(page), document
                .IsTagged()));
            if (document.IsTagged() && tagPointer != null) {
                tagPointer.SetPageForTagging(backupPage);
                tagPointer.MoveToPointer(backupPointer);
            }
        }

        /// <summary>Creates renderer from element excluding page breaks and adds tagging tree hints.</summary>
        /// <param name="element">
        /// 
        /// <see cref="iText.Layout.Element.IElement"/>
        /// to create renderer for
        /// </param>
        /// <param name="documentRenderer">document renderer to use as parent for margin renderer</param>
        /// <param name="pdfDocument">
        /// 
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// to which content is written
        /// </param>
        /// <returns>
        /// created
        /// <see cref="iText.Layout.Renderer.IRenderer"/>
        /// </returns>
        public static IRenderer CreateRendererFromElement(IElement element, DocumentRenderer documentRenderer, PdfDocument
             pdfDocument) {
            if (element == null) {
                return null;
            }
            IRenderer renderer = element.CreateRendererSubTree();
            RemovePageBreaks(renderer);
            renderer.SetParent(documentRenderer);
            if (pdfDocument.IsTagged()) {
                LayoutTaggingHelper taggingHelper = renderer.GetProperty<LayoutTaggingHelper>(Property.TAGGING_HELPER);
                LayoutTaggingHelper.AddTreeHints(taggingHelper, renderer);
            }
            return renderer;
        }

//\cond DO_NOT_DOCUMENT
        /// <summary>Adds footnotes for the page.</summary>
        /// <param name="content">
        /// 
        /// <see cref="PageFootnotesContent"/>
        /// representing footnotes
        /// </param>
        /// <returns>
        /// this same
        /// <see cref="PageMarginBoxes"/>
        /// instance
        /// </returns>
        internal virtual iText.Layout.Properties.Margins.PageMarginBoxes AddFootnotes(PageFootnotesContent content
            ) {
            int pageNum = content.GetPageNumber();
            if (this.footnotes.ContainsKey(pageNum)) {
                PageFootnotesContent existing = this.footnotes.Get(pageNum);
                IElement existingFootnotes = existing.GetContent();
                IElement newFootnotes = content.GetContent();
                content = new PageFootnotesContent(CollectFootnotes(existingFootnotes, newFootnotes)).SetPageNumber(pageNum
                    );
            }
            this.footnotes.Put(pageNum, content);
            return this;
        }
//\endcond

        /// <summary>
        /// Gets page margin content
        /// <see cref="PageMarginContent"/>
        /// by margin box name.
        /// </summary>
        /// <param name="marginBoxName">
        /// 
        /// <see cref="MarginBoxName"/>
        /// margin box name to get content for
        /// </param>
        /// <returns>
        /// page margin content
        /// <see cref="PageMarginContent"/>
        /// by margin box name
        /// </returns>
        public virtual PageMarginContent GetPageMarginContent(MarginBoxName marginBoxName) {
            return this.margins.Get(marginBoxName);
        }

        /// <summary>Gets page margin sizes in top, right, bottom, left order.</summary>
        /// <returns>array of float top, right, bottom, left margin sizes</returns>
        public virtual float[] GetMarginSizes() {
            return marginSizes;
        }

        /// <summary>Sets page margin sizes in top, right, bottom, left order.</summary>
        /// <param name="marginSizes">array of float top, right, bottom, left margin sizes</param>
        /// <returns>
        /// this same
        /// <see cref="PageMarginBoxes"/>
        /// instance
        /// </returns>
        public virtual iText.Layout.Properties.Margins.PageMarginBoxes SetMarginSizes(float[] marginSizes) {
            this.marginSizes = marginSizes;
            return this;
        }

        /// <summary>Layouts all page margins to calculate their occupied area and page margin sizes.</summary>
        /// <param name="documentRenderer">
        /// 
        /// <see cref="iText.Layout.Renderer.DocumentRenderer"/>
        /// renderer for the document to which content will be written
        /// </param>
        /// <param name="pageNumber">page number</param>
        /// <param name="pageSize">page size</param>
        /// <returns>float array of top, right, bottom, left margin sizes</returns>
        public virtual float[] Layout(DocumentRenderer documentRenderer, int pageNumber, Rectangle pageSize) {
            PageMarginContent topM = this.GetPageMarginContent(MarginBoxName.TOP);
            PageMarginContent rightM = this.GetPageMarginContent(MarginBoxName.RIGHT);
            PageMarginContent bottomM = this.GetPageMarginContent(MarginBoxName.BOTTOM);
            PageMarginContent leftM = this.GetPageMarginContent(MarginBoxName.LEFT);
            // Layout all margins.
            LayoutResult top = null;
            MinMaxWidth rightMinMaxWidth = null;
            LayoutResult bottom = null;
            MinMaxWidth leftMinMaxWidth = null;
            if (topM != null) {
                IRenderer topMargin = topM.GetContent().CreateRendererSubTree();
                top = topMargin.SetParent(documentRenderer).Layout(new LayoutContext(new LayoutArea(pageNumber, pageSize))
                    );
            }
            if (rightM != null) {
                IRenderer rightMargin = rightM.GetContent().CreateRendererSubTree();
                rightMinMaxWidth = ((AbstractRenderer)rightMargin.SetParent(documentRenderer)).GetMinMaxWidth();
            }
            if (bottomM != null) {
                IRenderer bottomMargin = bottomM.GetContent().CreateRendererSubTree();
                bottom = bottomMargin.SetParent(documentRenderer).Layout(new LayoutContext(new LayoutArea(pageNumber, pageSize
                    )));
            }
            if (leftM != null) {
                IRenderer leftMargin = leftM.GetContent().CreateRendererSubTree();
                leftMinMaxWidth = ((AbstractRenderer)leftMargin.SetParent(documentRenderer)).GetMinMaxWidth();
            }
            Document document = (Document)documentRenderer.GetModelElement();
            // Save rectangles for all renderers.
            float leftMargin_1 = leftMinMaxWidth == null ? document.GetLeftMargin() : leftMinMaxWidth.GetMinWidth();
            float rightMargin_1 = rightMinMaxWidth == null ? document.GetRightMargin() : rightMinMaxWidth.GetMinWidth(
                );
            Rectangle topBBox = top == null ? new Rectangle(0, pageSize.GetTop() - document.GetTopMargin(), pageSize.GetWidth
                (), document.GetTopMargin()) : (top.GetOccupiedArea() == null ? new Rectangle(0, 0) : top.GetOccupiedArea
                ().GetBBox());
            Rectangle bottomBBox = bottom == null ? new Rectangle(0, 0, pageSize.GetWidth(), document.GetBottomMargin(
                )) : (bottom.GetOccupiedArea() == null ? new Rectangle(0, 0) : bottom.GetOccupiedArea().GetBBox());
            if (topM != null) {
                topM.SetRectangle(new Rectangle(leftMargin_1, topBBox.GetY(), pageSize.GetWidth() - rightMargin_1 - leftMargin_1
                    , topBBox.GetHeight()));
            }
            if (rightM != null) {
                rightM.SetRectangle(new Rectangle(pageSize.GetRight() - rightMargin_1, bottomBBox.GetHeight(), rightMargin_1
                    , pageSize.GetHeight() - topBBox.GetHeight() - bottomBBox.GetHeight()));
            }
            if (bottomM != null) {
                bottomM.SetRectangle(new Rectangle(leftMargin_1, 0, pageSize.GetWidth() - rightMargin_1 - leftMargin_1, bottomBBox
                    .GetHeight()));
            }
            if (leftM != null) {
                leftM.SetRectangle(new Rectangle(0, bottomBBox.GetHeight(), leftMargin_1, pageSize.GetHeight() - topBBox.GetHeight
                    () - bottomBBox.GetHeight()));
            }
            PageFootnotesContent footnotes = this.GetFootnotes(pageNumber);
            if (footnotes != null) {
                Rectangle footnotesRect = new Rectangle(leftMargin_1, bottomBBox.GetHeight(), pageSize.GetWidth() - rightMargin_1
                     - leftMargin_1, pageSize.GetHeight() - topBBox.GetHeight() - bottomBBox.GetHeight());
                IRenderer footnotesRenderer = footnotes.GetContent().CreateRendererSubTree();
                LayoutResult footnotesResult = footnotesRenderer.SetParent(documentRenderer).Layout(new LayoutContext(new 
                    LayoutArea(pageNumber, footnotesRect)));
                footnotesRect.SetHeight(footnotesResult.GetOccupiedArea().GetBBox().GetHeight());
                footnotes.SetRectangle(footnotesRect);
                bottomBBox.IncreaseHeight(footnotes.GetRectangle().GetHeight());
            }
            return new float[] { topBBox.GetHeight(), rightMargin_1, bottomBBox.GetHeight(), leftMargin_1 };
        }

        /// <summary>Draws all page margins for the page specified via page number.</summary>
        /// <param name="documentRenderer">document renderer to use as parent for margin renderer</param>
        /// <param name="document">
        /// 
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// to which content is written
        /// </param>
        /// <param name="pageNumber">page number</param>
        public virtual void Draw(DocumentRenderer documentRenderer, PdfDocument document, int pageNumber) {
            PageFootnotesContent footnotes = this.GetFootnotes(pageNumber);
            if (footnotes != null) {
                DrawPageContent(documentRenderer, document, pageNumber, footnotes, true);
            }
            foreach (PageMarginContent margin in margins.Values) {
                DrawPageContent(documentRenderer, document, pageNumber, margin, false);
            }
        }

        /// <summary>Sets the role of the page margin element to use in the document tag tree.</summary>
        /// <param name="element">
        /// 
        /// <see cref="iText.Layout.Element.IElement"/>
        /// to set role for
        /// </param>
        protected internal virtual void SetPageMarginTagRole(IElement element) {
            if (element is IAccessibleElement) {
                ((IAccessibleElement)element).GetAccessibilityProperties().SetRole(StandardRoles.ARTIFACT);
            }
        }

        /// <summary>
        /// Gets rid of all page breaks that might have occurred inside page margin boxes
        /// because of the running/layout elements.
        /// </summary>
        /// <param name="renderer">the root renderer of the renderers subtree</param>
        private static void RemovePageBreaks(IRenderer renderer) {
            IList<IRenderer> pageBreaks = null;
            foreach (IRenderer child in renderer.GetChildRenderers()) {
                if (child is AreaBreakRenderer || child is SectionBreakRenderer) {
                    if (pageBreaks == null) {
                        pageBreaks = new List<IRenderer>();
                    }
                    pageBreaks.Add(child);
                }
                else {
                    RemovePageBreaks(child);
                }
            }
            if (pageBreaks != null) {
                renderer.GetChildRenderers().RemoveAll(pageBreaks);
            }
        }

        private static FootnotesContainer CollectFootnotes(IElement existingFootnotes, IElement newFootnotes) {
            if (!(existingFootnotes is FootnotesContainer) || !(newFootnotes is FootnotesContainer)) {
                throw new ArgumentException("Footnotes must be a FootnotesContainer!");
            }
            FootnotesContainer container = (FootnotesContainer)existingFootnotes;
            container.AddFootnotesFromOtherContainer((FootnotesContainer)newFootnotes);
            return container;
        }

        private PageFootnotesContent GetFootnotes(int pageNumber) {
            return this.footnotes.Get(pageNumber);
        }

        private void DrawPageContent(DocumentRenderer documentRenderer, PdfDocument document, int pageNumber, AbstractPageContent
             pageContent, bool tagged) {
            Rectangle rect = pageContent.GetRectangle();
            if (rect == null) {
                // Margins weren't layouted, we can get here if page is added manually and is empty.
                // Or in case footnotes are added.
                Layout(documentRenderer, pageNumber, document.GetPage(pageNumber).GetPageSize());
                rect = pageContent.GetRectangle();
            }
            IElement element = pageContent.GetContent();
            if (!tagged) {
                SetPageMarginTagRole(element);
            }
            String name = pageContent is PageMarginContent ? (((PageMarginContent)pageContent).GetMarginBoxName().ToString
                () + " margin box") : "footnotes";
            IRenderer renderer = CreateRendererFromElement(element, documentRenderer, document);
            Draw(renderer, rect, documentRenderer, document, pageNumber, name);
        }

        public override bool Equals(Object o) {
            if (this == o) {
                return true;
            }
            if (o == null || GetType() != o.GetType()) {
                return false;
            }
            iText.Layout.Properties.Margins.PageMarginBoxes that = (iText.Layout.Properties.Margins.PageMarginBoxes)o;
            bool result = true;
            foreach (KeyValuePair<MarginBoxName, PageMarginContent> pageMarginEntry in this.margins) {
                MarginBoxName marginBoxName = pageMarginEntry.Key;
                if (!that.margins.ContainsKey(marginBoxName)) {
                    return false;
                }
                result &= Object.Equals(pageMarginEntry.Value, that.margins.Get(marginBoxName));
            }
            return result;
        }

        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(margins);
        }
    }
}
