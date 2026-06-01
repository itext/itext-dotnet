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
using System.Collections.Generic;
using iText.Commons.Utils;
using iText.Kernel.Geom;
using iText.Layout.Element;
using iText.Layout.Layout;

namespace iText.Layout.Renderer {
//\cond DO_NOT_DOCUMENT
    /// <summary>Helper handler class to collect and count footnotes placed on the page.</summary>
    internal class FootnotesCounterHandler {
        private readonly IDictionary<FootnoteAnchor, FootnoteAnchorRenderer> renderers = new Dictionary<FootnoteAnchor
            , FootnoteAnchorRenderer>();

        private readonly IDictionary<Footnote, float?> footnotes = new LinkedDictionary<Footnote, float?>();

        /// <summary>
        /// Creates a new
        /// <see cref="FootnotesCounterHandler"/>
        /// instance.
        /// </summary>
        public FootnotesCounterHandler() {
        }

//\cond DO_NOT_DOCUMENT
        // Empty constructor.
        /// <summary>
        /// Adds footnote anchor info after
        /// <see cref="iText.Layout.Element.FootnoteAnchor"/>
        /// layout.
        /// </summary>
        /// <param name="renderer">
        /// renderer for
        /// <see cref="iText.Layout.Element.FootnoteAnchor"/>
        /// which stores layout information
        /// </param>
        internal static void AddFootnoteAnchor(FootnoteAnchorRenderer renderer) {
            iText.Layout.Renderer.FootnotesCounterHandler footnotesCounterHandler = GetFootnotesCounterHandler(renderer
                );
            if (footnotesCounterHandler != null) {
                FootnoteAnchor footnoteAnchor = (FootnoteAnchor)renderer.modelElement;
                footnotesCounterHandler.renderers.Put(footnoteAnchor, renderer);
            }
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>
        /// Gets
        /// <see cref="FootnotesCounterHandler"/>
        /// used in root
        /// <see cref="DocumentRenderer"/>.
        /// </summary>
        /// <param name="renderer">
        /// 
        /// <see cref="IRenderer"/>
        /// any renderer in the current tree
        /// </param>
        /// <returns>
        /// 
        /// <see cref="FootnotesCounterHandler"/>
        /// used in root
        /// <see cref="DocumentRenderer"/>
        /// </returns>
        internal static iText.Layout.Renderer.FootnotesCounterHandler GetFootnotesCounterHandler(IRenderer renderer
            ) {
            IRenderer rootRenderer = renderer;
            while (rootRenderer.GetParent() != null) {
                rootRenderer = rootRenderer.GetParent();
            }
            if (rootRenderer is DocumentRenderer) {
                return ((DocumentRenderer)rootRenderer).footnotesCounterHandler;
            }
            return null;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>
        /// Resets current
        /// <see cref="FootnotesCounterHandler"/>
        /// before collecting placed footnotes.
        /// </summary>
        internal virtual void Reset() {
            renderers.Clear();
            footnotes.Clear();
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>
        /// Collects footnotes which anchors are placed in the current area
        /// in order their anchors are placed on a page from top to bottom and left to right.
        /// </summary>
        /// <param name="currentArea">
        /// 
        /// <see cref="iText.Layout.Layout.RootLayoutArea"/>
        /// area to collect placed footnote anchors
        /// </param>
        /// <returns>
        /// linked map of
        /// <see cref="iText.Layout.Element.Footnote"/>
        /// and its height float value
        /// </returns>
        internal virtual IDictionary<Footnote, float?> CollectFootnotes(RootLayoutArea currentArea) {
            footnotes.Clear();
            IList<FootnoteAnchor> anchors = new List<FootnoteAnchor>(renderers.Keys);
            JavaCollectionsUtil.Sort(anchors, new FootnotesCounterHandler.FootnoteAnchorComparator(this));
            foreach (FootnoteAnchor footnoteAnchor in anchors) {
                FootnoteAnchorRenderer renderer = renderers.Get(footnoteAnchor);
                if (renderer.occupiedArea == null) {
                    continue;
                }
                int expectedPageNumber = currentArea.GetPageNumber();
                Rectangle intersection = renderer.occupiedArea.GetBBox().GetIntersection(currentArea.GetBBox());
                if (expectedPageNumber == renderer.occupiedArea.GetPageNumber() && intersection != null && renderer.occupiedArea
                    .GetBBox().EqualsWithEpsilon(intersection)) {
                    footnotes.Put(footnoteAnchor.GetFootnote(), renderer.footnoteRenderer.GetOccupiedArea().GetBBox().GetHeight
                        ());
                }
            }
            return footnotes;
        }
//\endcond

        private sealed class FootnoteAnchorComparator : IComparer<FootnoteAnchor> {
            public int Compare(FootnoteAnchor o1, FootnoteAnchor o2) {
                Rectangle rectangle1 = this._enclosing.renderers.Get(o1).occupiedArea.GetBBox();
                Rectangle rectangle2 = this._enclosing.renderers.Get(o2).occupiedArea.GetBBox();
                int result = JavaUtil.FloatCompare(-rectangle1.GetY(), -rectangle2.GetY());
                if (result == 0) {
                    result = JavaUtil.FloatCompare(rectangle1.GetX(), rectangle2.GetX());
                }
                return result;
            }

            internal FootnoteAnchorComparator(FootnotesCounterHandler _enclosing) {
                this._enclosing = _enclosing;
            }

            private readonly FootnotesCounterHandler _enclosing;
        }
    }
//\endcond
}
