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
using iText.Commons.Internal.Runtime;
using iText.Layout;
using iText.Layout.Renderer;

namespace iText.Layout.Properties.Margins {
    /// <summary>Utility class to process footnotes for internal usage only.</summary>
    public sealed class FootnotesUtil {
        private FootnotesUtil() {
        }

        // Private constructor will prevent the instantiation of this class directly.
        /// <summary>
        /// Adds provided footnotes to the specified page via
        /// <see cref="PageMarginBoxes"/>.
        /// </summary>
        /// <param name="pageNum">page number</param>
        /// <param name="footnotesToAdd">
        /// list of
        /// <see cref="Footnote"/>
        /// instance to add
        /// </param>
        /// <param name="pageMarginBoxes">
        /// 
        /// <see cref="PageMarginBoxes"/>
        /// for the page
        /// </param>
        /// <param name="footnotesProperties">
        /// 
        /// <see cref="FootnotesProperties"/>
        /// to apply for footnotes
        /// </param>
        public static void AddFootnotesToPage(int pageNum, IList<Footnote> footnotesToAdd, PageMarginBoxes pageMarginBoxes
            , FootnotesProperties footnotesProperties) {
            FootnotesContainer footnotesContainer = new FootnotesContainer(pageNum);
            if (footnotesProperties.GetFootnotesContainerStyle() != null) {
                footnotesContainer.AddStyle(footnotesProperties.GetFootnotesContainerStyle());
            }
            foreach (Footnote footnote in footnotesToAdd) {
                footnotesContainer.Add(footnote);
                if (footnote.footnoteAnchor != null) {
                    footnote.anchors.Put(pageNum, footnote.footnoteAnchor);
                    footnote.ResetFootnoteAnchor();
                }
            }
            PageFootnotesContent pageFootnotesContent = new PageFootnotesContent(footnotesContainer).SetPageNumber(pageNum
                );
            pageMarginBoxes.AddFootnotes(pageFootnotesContent);
        }

        /// <summary>Sets parent for footnote renderer in order for it to be layouted with correct properties and styles applied.
        ///     </summary>
        /// <param name="footnoteRenderer">
        /// 
        /// <see cref="iText.Layout.Renderer.FootnoteRenderer"/>
        /// to set parent for
        /// </param>
        /// <param name="documentRenderer">
        /// 
        /// <see cref="iText.Layout.Renderer.DocumentRenderer"/>
        /// root renderer, the parent of footnotes container renderer
        /// </param>
        public static void SetParentForFootnoteRenderer(FootnoteRenderer footnoteRenderer, DocumentRenderer documentRenderer
            ) {
            FootnotesProperties footnotesProperties = ((Document)documentRenderer.GetModelElement()).GetFootnotesProperties
                ();
            FootnotesContainer footnotesContainer = new FootnotesContainer(-1);
            if (footnotesProperties != null && footnotesProperties.GetFootnotesContainerStyle() != null) {
                footnotesContainer.AddStyle(footnotesProperties.GetFootnotesContainerStyle());
            }
            FootnotesContainerRenderer footnotesContainerRenderer = new FootnotesContainerRenderer(footnotesContainer);
            footnoteRenderer.SetParent(footnotesContainerRenderer.SetParent(documentRenderer));
        }

        /// <summary>
        /// Applies
        /// <see cref="iText.Layout.Style"/>
        /// storing style properties for footnote anchor that is placed inside the footnote.
        /// </summary>
        /// <param name="anchor">
        /// 
        /// <see cref="FootnoteAnchor"/>
        /// to apply style for
        /// </param>
        /// <param name="footnoteAnchorLabelStyle">
        /// 
        /// <see cref="iText.Layout.Style"/>
        /// storing properties for footnote anchor inside the footnote
        /// </param>
        public static void ApplyFootnoteAnchorStyle(FootnoteAnchor anchor, Style footnoteAnchorLabelStyle) {
            anchor.SetFootnoteAnchorLabelStyle(footnoteAnchorLabelStyle);
        }
    }
}
