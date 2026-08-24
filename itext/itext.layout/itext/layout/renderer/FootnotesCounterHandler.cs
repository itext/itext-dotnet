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
using System.Linq;
using iText.Commons.Internal.Runtime;
using iText.Commons.Utils;
using iText.Kernel.Geom;
using iText.Kernel.Numbering;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties.Margins;

namespace iText.Layout.Renderer {
//\cond DO_NOT_DOCUMENT
    /// <summary>Helper handler class to collect and count footnotes placed on the page.</summary>
    internal class FootnotesCounterHandler {
        private const int DEFAULT_FONT_SIZE = 6;

        private const int DEFAULT_TEXT_RISE = 7;

        private readonly IDictionary<Footnote, FootnoteRenderer> footnotes = new LinkedDictionary<Footnote, FootnoteRenderer
            >();

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
            footnotes.Clear();
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>
        /// Collects footnotes which anchors are placed in the current area
        /// in order their anchors are placed on a page from top to bottom and left to right.
        /// </summary>
        /// <param name="renderer">parent renderer to collect footnotes from</param>
        /// <param name="footnotesAnchorsFound">a list to store the encountered footnote anchors</param>
        /// <returns>
        /// linked map of
        /// <see cref="iText.Layout.Properties.Margins.Footnote"/>
        /// and corresponding renderers.
        /// </returns>
        internal virtual IDictionary<Footnote, FootnoteRenderer> CollectFootnotes(IRenderer renderer, IList<FootnoteAnchorRenderer
            > footnotesAnchorsFound) {
            footnotesAnchorsFound.Clear();
            footnotes.Clear();
            CollectFromTree(renderer, footnotes, footnotesAnchorsFound);
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
        /// <param name="anchorsToNumber">the list of anchors to apply the renumbering on</param>
        internal virtual void UpdateFootnoteNumberingAndStyles(FootnotesProperties footnotesProperties, int latestFootnoteNum
            , ICollection<FootnoteAnchorRenderer> anchorsToNumber) {
            if (footnotesProperties == null) {
                return;
            }
            Style footnoteAnchorLabelStyle = footnotesProperties.GetFootnoteAnchorLabelStyle();
            if (footnoteAnchorLabelStyle != null) {
                foreach (FootnoteAnchorRenderer renderer in anchorsToNumber) {
                    FootnotesUtil.ApplyFootnoteAnchorStyle((FootnoteAnchor)renderer.GetModelElement(), footnoteAnchorLabelStyle
                        );
                }
            }
            // Note: footnote anchor style is applied by FootnoteAnchorRenderer via Property.FOOTNOTES_PROPERTIES
            // so that we do not need to apply anchor styles to the anchor here
            if (footnotesProperties.GetFootnoteNumberingType() == null) {
                return;
            }
            FootnoteNumberingType? footnoteNumberingType = footnotesProperties.GetFootnoteNumberingType();
            IList<FootnoteAnchorRenderer> anchors = anchorsToNumber.Sorted((renderer1, renderer2) => {
                int result = JavaUtil.FloatCompare(-renderer1.yPos, -renderer2.yPos);
                if (result == 0) {
                    Rectangle rectangle1 = renderer1.occupiedArea.GetBBox();
                    Rectangle rectangle2 = renderer2.occupiedArea.GetBBox();
                    result = JavaUtil.FloatCompare(rectangle1.GetX(), rectangle2.GetX());
                }
                return result;
            }
            ).ToList();
            int footnoteNum = latestFootnoteNum + 1;
            foreach (FootnoteAnchorRenderer renderer in anchors) {
                IRenderer currentSymbolRenderer = MakeFootnoteNumSymbolRenderer(footnoteNum, footnoteNumberingType);
                ++footnoteNum;
                renderer.AddSymbolRenderer(currentSymbolRenderer);
            }
        }
//\endcond

        private static void CollectFromTree(IRenderer renderer, IDictionary<Footnote, FootnoteRenderer> footnotes, 
            IList<FootnoteAnchorRenderer> footnotesAnchorsFound) {
            if (renderer == null) {
                return;
            }
            TableRenderer tableRenderer = null;
            if (renderer is TableRenderer) {
                tableRenderer = (TableRenderer)renderer;
                if (tableRenderer.headerRenderer != null) {
                    CollectFromTree(tableRenderer.headerRenderer, footnotes, footnotesAnchorsFound);
                }
            }
            foreach (IRenderer child in renderer.GetChildRenderers()) {
                if (child is FootnoteAnchorRenderer) {
                    footnotesAnchorsFound.Add((FootnoteAnchorRenderer)child);
                    FootnoteRenderer footnoteRenderer = ((FootnoteAnchorRenderer)child).footnoteRenderer;
                    if (footnoteRenderer == null) {
                        continue;
                    }
                    footnotes.Put((Footnote)footnoteRenderer.GetModelElement(), footnoteRenderer);
                }
                else {
                    CollectFromTree(child, footnotes, footnotesAnchorsFound);
                }
            }
            if (tableRenderer != null && tableRenderer.footerRenderer != null) {
                CollectFromTree(tableRenderer.footerRenderer, footnotes, footnotesAnchorsFound);
            }
        }

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
            Text textElement = new Text(numberText);
            return new TextRenderer(textElement);
        }
    }
//\endcond
}
