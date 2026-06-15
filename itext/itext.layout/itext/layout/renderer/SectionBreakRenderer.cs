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
using iText.Commons.Utils;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Layout;
using iText.Layout.Logs;
using iText.Layout.Properties;
using iText.Layout.Properties.Margins;

namespace iText.Layout.Renderer {
    /// <summary>
    /// Renderer for the
    /// <see cref="iText.Layout.Element.SectionBreak"/>
    /// layout element.
    /// </summary>
    /// <remarks>
    /// Renderer for the
    /// <see cref="iText.Layout.Element.SectionBreak"/>
    /// layout element.
    /// Will terminate the current page content if any and start a new page.
    /// </remarks>
    public class SectionBreakRenderer : IRenderer {
        private static readonly ILogger LOGGER = ITextLogManager.GetLogger(typeof(iText.Layout.Renderer.SectionBreakRenderer
            ));

        private readonly SectionBreak sectionBreak;

        private IRenderer parent;

        /// <summary>
        /// Creates new
        /// <see cref="SectionBreakRenderer"/>
        /// instance.
        /// </summary>
        /// <param name="sectionBreak">
        /// the
        /// <see cref="iText.Layout.Element.SectionBreak"/>
        /// that will be rendered by this object
        /// </param>
        public SectionBreakRenderer(SectionBreak sectionBreak) {
            this.sectionBreak = sectionBreak;
        }

        /// <summary>
        /// Logs a warning about unexpected use of
        /// <see cref="SectionBreakRenderer"/>
        /// because instances of this class are only used for terminating the current page content.
        /// </summary>
        /// <param name="renderer">
        /// 
        /// <inheritDoc/>
        /// </param>
        public virtual void AddChild(IRenderer renderer) {
            LOGGER.LogWarning(LayoutLogMessageConstant.SECTION_BREAK_UNEXPECTED);
        }

        public virtual LayoutResult Layout(LayoutContext layoutContext) {
            bool anythingPlaced = false;
            bool pageMarginsChanged = false;
            bool pageSizeChanged = false;
            int pageNumber = layoutContext.GetArea().GetPageNumber();
            if (pageNumber == 0) {
                LOGGER.LogWarning(LayoutLogMessageConstant.SECTION_BREAK_LAYOUT_ON_PAGE_0);
                pageNumber = 1;
            }
            IRenderer parentRenderer = GetParent();
            while (parentRenderer != null) {
                if (parentRenderer is DocumentRenderer) {
                    DocumentRenderer documentRenderer = (DocumentRenderer)parentRenderer;
                    PdfPage currentPage = documentRenderer.GetPdfDocument().GetPage(pageNumber);
                    float pageHeight = GetPageEffectiveHeight(currentPage.GetPageSize(), documentRenderer);
                    anythingPlaced = Math.Abs(pageHeight - layoutContext.GetArea().GetBBox().GetHeight()) > AbstractRenderer.EPS;
                    PageSize sectionBreakPageSize = sectionBreak.GetPageSize();
                    pageSizeChanged = !currentPage.GetPageSize().EqualsWithEpsilon(sectionBreakPageSize == null ? documentRenderer
                        .document.GetPdfDocument().GetDefaultPageSize() : sectionBreakPageSize);
                    if (anythingPlaced) {
                        PageMarginBoxes pageMarginBoxes = documentRenderer.document.GetPageMargins(pageNumber);
                        PageMarginBoxes sectionBreakMargins = sectionBreak.GetPageMargins();
                        if (sectionBreakMargins == null) {
                            pageMarginsChanged = pageMarginBoxes != null;
                        }
                        else {
                            pageMarginsChanged = !sectionBreakMargins.Equals(pageMarginBoxes);
                        }
                    }
                    break;
                }
                parentRenderer = parentRenderer.GetParent();
            }
            // We're interested only in bottom coordinate of the already placed content.
            LayoutArea updatedArea = new LayoutArea(pageNumber, new Rectangle(0, layoutContext.GetArea().GetBBox().GetTop
                (), 0, 0));
            SectionBreakUtil.BreakPage(sectionBreak, pageSizeChanged || (anythingPlaced && pageMarginsChanged));
            return new LayoutResult(LayoutResult.NOTHING, anythingPlaced ? updatedArea : null, null, null, this).SetSectionBreak
                (sectionBreak);
        }

        /// <summary>
        /// Logs a warning about unexpected use of
        /// <see cref="SectionBreakRenderer"/>
        /// because instances of this class are only used for terminating the current page content.
        /// </summary>
        /// <param name="drawContext">
        /// 
        /// <inheritDoc/>
        /// </param>
        public virtual void Draw(DrawContext drawContext) {
            LOGGER.LogWarning(LayoutLogMessageConstant.SECTION_BREAK_UNEXPECTED);
        }

        /// <summary>
        /// Throws an UnsupportedOperationException because instances of this
        /// class are only used for terminating the current page content.
        /// </summary>
        /// <remarks>
        /// Throws an UnsupportedOperationException because instances of this
        /// class are only used for terminating the current page content.
        /// <para />
        /// In case there is no current page content, empty area will be returned.
        /// </remarks>
        /// <returns>
        /// 
        /// <inheritDoc/>
        /// </returns>
        public virtual LayoutArea GetOccupiedArea() {
            throw new NotSupportedException();
        }

        public virtual bool HasProperty(int property) {
            return false;
        }

        public virtual bool HasOwnProperty(int property) {
            return false;
        }

        /// <summary>
        /// Always returns <c>null</c> because instances of this
        /// class are only used for terminating the current page content.
        /// </summary>
        /// <param name="property">
        /// 
        /// <inheritDoc/>
        /// </param>
        /// <param name="defaultValue">
        /// 
        /// <inheritDoc/>
        /// </param>
        /// <typeparam name="T1">
        /// 
        /// <inheritDoc/>
        /// </typeparam>
        /// <returns>
        /// 
        /// <inheritDoc/>
        /// </returns>
        public virtual T1 GetProperty<T1>(int property, T1 defaultValue) {
            return (T1)(Object)null;
        }

        public virtual T1 GetProperty<T1>(int key) {
            return (T1)(Object)null;
        }

        public virtual T1 GetOwnProperty<T1>(int property) {
            return (T1)(Object)null;
        }

        public virtual T1 GetDefaultProperty<T1>(int property) {
            return (T1)(Object)null;
        }

        /// <summary>
        /// Logs a warning about unexpected use of
        /// <see cref="SectionBreakRenderer"/>
        /// because instances of this class are only used for terminating the current page content.
        /// </summary>
        /// <param name="property">
        /// 
        /// <inheritDoc/>
        /// </param>
        /// <param name="value">
        /// 
        /// <inheritDoc/>
        /// </param>
        public virtual void SetProperty(int property, Object value) {
            LOGGER.LogWarning(LayoutLogMessageConstant.SECTION_BREAK_UNEXPECTED);
        }

        public virtual void DeleteOwnProperty(int property) {
        }

        // Do nothing.
        public virtual IRenderer SetParent(IRenderer parent) {
            this.parent = parent;
            return this;
        }

        public virtual IPropertyContainer GetModelElement() {
            return sectionBreak;
        }

        public virtual IRenderer GetParent() {
            return this.parent;
        }

        public virtual IList<IRenderer> GetChildRenderers() {
            return JavaCollectionsUtil.EmptyList<IRenderer>();
        }

        public virtual bool IsFlushed() {
            return false;
        }

        /// <summary>
        /// Logs a warning about unexpected use of
        /// <see cref="SectionBreakRenderer"/>
        /// because instances of this class are only used for terminating the current page content.
        /// </summary>
        /// <param name="dx">
        /// 
        /// <inheritDoc/>
        /// </param>
        /// <param name="dy">
        /// 
        /// <inheritDoc/>
        /// </param>
        public virtual void Move(float dx, float dy) {
            LOGGER.LogWarning(LayoutLogMessageConstant.SECTION_BREAK_UNEXPECTED);
        }

        public virtual IRenderer GetNextRenderer() {
            return null;
        }

        private static float GetPageEffectiveHeight(Rectangle pageSize, DocumentRenderer renderer) {
            float bottomMargin = (float)renderer.GetPropertyAsFloat(Property.MARGIN_BOTTOM);
            float topMargin = (float)renderer.GetPropertyAsFloat(Property.MARGIN_TOP);
            return pageSize.GetHeight() - bottomMargin - topMargin;
        }
    }
}
