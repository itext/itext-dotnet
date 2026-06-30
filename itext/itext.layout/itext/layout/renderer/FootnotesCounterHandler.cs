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
using iText.Commons.Internal.Runtime;
using iText.Commons.Utils;
using iText.Kernel.Geom;
using iText.Kernel.Numbering;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Layout;
using iText.Layout.Properties;
using iText.Layout.Properties.Margins;

namespace iText.Layout.Renderer {
//\cond DO_NOT_DOCUMENT
    /// <summary>Helper handler class to collect and count footnotes placed on the page.</summary>
    internal class FootnotesCounterHandler {
        private const int DEFAULT_FONT_SIZE = 6;

        private const int DEFAULT_TEXT_RISE = 7;

        private readonly IDictionary<FootnoteAnchor, FootnoteAnchorRenderer> renderers = new Dictionary<FootnoteAnchor
            , FootnoteAnchorRenderer>();

        private readonly IDictionary<FootnoteRenderer, float?> footnotes = new LinkedDictionary<FootnoteRenderer, 
            float?>();

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
        /// <see cref="iText.Layout.Properties.Margins.FootnoteAnchor"/>
        /// layout.
        /// </summary>
        /// <param name="renderer">
        /// renderer for
        /// <see cref="iText.Layout.Properties.Margins.FootnoteAnchor"/>
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
        /// <see cref="iText.Layout.Layout.LayoutArea"/>
        /// area to collect placed footnote anchors
        /// </param>
        /// <returns>
        /// linked map of
        /// <see cref="iText.Layout.Properties.Margins.Footnote"/>
        /// and its height float value
        /// </returns>
        internal virtual IDictionary<FootnoteRenderer, float?> CollectFootnotes(LayoutArea currentArea) {
            footnotes.Clear();
            IList<FootnoteAnchor> anchors = new List<FootnoteAnchor>(renderers.Keys);
            JavaCollectionsUtil.Sort(anchors, new FootnotesCounterHandler.FootnoteAnchorComparator(this));
            foreach (FootnoteAnchor footnoteAnchor in anchors) {
                FootnoteAnchorRenderer renderer = renderers.Get(footnoteAnchor);
                if (renderer.occupiedArea == null) {
                    continue;
                }
                int expectedPageNumber = currentArea.GetPageNumber();
                // Check whether footnote anchor is inside the currentArea (if the overlap is greater than 50 percent).
                bool isAnchorInsideCurrentArea = currentArea.GetBBox().Overlaps(renderer.occupiedArea.GetBBox(), 0.5F * Math
                    .Min(renderer.occupiedArea.GetBBox().GetWidth(), renderer.occupiedArea.GetBBox().GetHeight()));
                if (expectedPageNumber == renderer.occupiedArea.GetPageNumber() && isAnchorInsideCurrentArea) {
                    footnotes.Put(renderer.footnoteRenderer, renderer.footnoteRenderer.GetOccupiedArea().GetBBox().GetHeight()
                        );
                }
            }
            return footnotes;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>
        /// Updates footnote anchors using automatic numbering and styles configured via
        /// <see cref="iText.Layout.Properties.Margins.FootnotesProperties"/>.
        /// </summary>
        /// <param name="footnotesProperties">
        /// 
        /// <see cref="iText.Layout.Properties.Margins.FootnotesProperties"/>
        /// with optional
        /// <see cref="iText.Layout.Properties.Margins.FootnoteNumberingType?"/>
        /// specifying type for numbering of the footnote anchors and optional styles for footnote anchors
        /// </param>
        /// <param name="latestFootnoteNum">
        /// the number of the previous placed footnote based on
        /// <see cref="iText.Layout.Properties.Margins.FootnoteNumberingConfig"/>
        /// </param>
        internal virtual void UpdateFootnoteNumberingAndStyles(FootnotesProperties footnotesProperties, int latestFootnoteNum
            ) {
            if (footnotesProperties == null) {
                return;
            }
            Style footnoteAnchorLabelStyle = footnotesProperties.GetFootnoteAnchorLabelStyle();
            if (footnoteAnchorLabelStyle != null) {
                foreach (FootnoteAnchor anchor in renderers.Keys) {
                    FootnotesUtil.ApplyFootnoteAnchorStyle(anchor, footnoteAnchorLabelStyle);
                }
            }
            if (footnotesProperties.GetFootnoteNumberingType() == null) {
                return;
            }
            FootnoteNumberingType? footnoteNumberingType = footnotesProperties.GetFootnoteNumberingType();
            IList<FootnoteAnchor> anchors = new List<FootnoteAnchor>(renderers.Keys);
            JavaCollectionsUtil.Sort(anchors, new FootnotesCounterHandler.FootnoteAnchorComparator(this));
            int footnoteNum = latestFootnoteNum + 1;
            foreach (FootnoteAnchor anchor in anchors) {
                FootnoteAnchorRenderer renderer = renderers.Get(anchor);
                IRenderer currentSymbolRenderer = MakeFootnoteNumSymbolRenderer(footnoteNum, footnoteNumberingType);
                ++footnoteNum;
                renderer.AddSymbolRenderer(currentSymbolRenderer);
            }
        }
//\endcond

        private static IRenderer MakeFootnoteNumSymbolRenderer(int index, FootnoteNumberingType? numberingType) {
            String numberText;
            switch (numberingType) {
                case FootnoteNumberingType.DECIMAL: {
                    numberText = index.ToString();
                    break;
                }

                case FootnoteNumberingType.ROMAN_LOWER: {
                    numberText = RomanNumbering.ToRomanLowerCase(index);
                    break;
                }

                case FootnoteNumberingType.ROMAN_UPPER: {
                    numberText = RomanNumbering.ToRomanUpperCase(index);
                    break;
                }

                case FootnoteNumberingType.ENGLISH_LOWER: {
                    numberText = EnglishAlphabetNumbering.ToLatinAlphabetNumberLowerCase(index);
                    break;
                }

                case FootnoteNumberingType.ENGLISH_UPPER: {
                    numberText = EnglishAlphabetNumbering.ToLatinAlphabetNumberUpperCase(index);
                    break;
                }

                case FootnoteNumberingType.GREEK_LOWER: {
                    numberText = GreekAlphabetNumbering.ToGreekAlphabetNumber(index, false, true);
                    break;
                }

                case FootnoteNumberingType.GREEK_UPPER: {
                    numberText = GreekAlphabetNumbering.ToGreekAlphabetNumber(index, true, true);
                    break;
                }

                default: {
                    throw new InvalidOperationException();
                }
            }
            Style defaultStyle = new Style();
            // TODO DEVSIX-10031 Do not specify constant font size by default,
            //  it should depend on parent paragraph font size.
            defaultStyle.SetProperty(Property.FONT_SIZE, UnitValue.CreatePointValue(DEFAULT_FONT_SIZE));
            defaultStyle.SetProperty(Property.TEXT_RISE, DEFAULT_TEXT_RISE);
            Text textElement = new Text(numberText).AddStyle(defaultStyle);
            return new TextRenderer(textElement);
        }

        private sealed class FootnoteAnchorComparator : IComparer<FootnoteAnchor> {
            public int Compare(FootnoteAnchor o1, FootnoteAnchor o2) {
                FootnoteAnchorRenderer renderer1 = this._enclosing.renderers.Get(o1);
                FootnoteAnchorRenderer renderer2 = this._enclosing.renderers.Get(o2);
                int result = JavaUtil.FloatCompare(-renderer1.yPos, -renderer2.yPos);
                if (result == 0) {
                    Rectangle rectangle1 = renderer1.occupiedArea.GetBBox();
                    Rectangle rectangle2 = renderer2.occupiedArea.GetBBox();
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
