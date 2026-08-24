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
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Layout.Renderer;
using iText.Layout.Tagging;

namespace iText.Layout.Properties.Margins {
    /// <summary>Utility class to process footnotes for internal usage only.</summary>
    public sealed class FootnotesUtil {
        private const float DEFAULT_FOOTNOTE_ANCHOR_FONT_SIZE_SCALE_FACTOR = 0.5F;

        private const float DEFAULT_FOOTNOTE_ANCHOR_TEXT_RISE_SCALE_FACTOR = 0.6F;

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
        public static void AddFootnotesToPage(int pageNum, IEnumerable<FootnoteRenderer> footnotesToAdd, PageMarginBoxes
             pageMarginBoxes, FootnotesProperties footnotesProperties) {
            FootnotesContainer footnotesContainer = new FootnotesContainer(pageNum);
            if (footnotesProperties.GetFootnotesContainerStyle() != null) {
                footnotesContainer.AddStyle(footnotesProperties.GetFootnotesContainerStyle());
            }
            foreach (FootnoteRenderer footnoteRederer in footnotesToAdd) {
                Footnote footnote = (Footnote)footnoteRederer.GetModelElement();
                footnotesContainer.Add(footnote, footnoteRederer.GetProperty<TaggingHintKey>(Property.TAGGING_HINT_KEY));
                if (footnote.GetInjectedFootnoteAnchor() != null) {
                    footnote.anchors.Put(pageNum, footnote.GetInjectedFootnoteAnchor());
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

        /// <summary>Gets injected footnote anchor element, which is a copy of a footnote anchor in the main content.</summary>
        /// <param name="footnote">
        /// 
        /// <see cref="Footnote"/>
        /// from which injected footnote anchor is retrieved
        /// </param>
        /// <returns>injected footnote anchor element</returns>
        public static IElement GetInjectedFootnoteAnchor(Footnote footnote) {
            return footnote.GetInjectedFootnoteAnchor();
        }

        /// <summary>Indicates whether a default style should be applied to injected footnote anchor copy.</summary>
        /// <param name="footnote">
        /// 
        /// <see cref="Footnote"/>
        /// containing injected anchor copy
        /// </param>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if default style is needed,
        /// <see langword="false"/>
        /// otherwise
        /// </returns>
        public static bool IsDefaultStyleNeededForInjectedFootnoteAnchor(Footnote footnote) {
            return footnote.IsDefaultStyleNeededForInjectedFootnoteAnchor();
        }

        /// <summary>Indicates whether a default style should be applied to the footnote anchor.</summary>
        /// <param name="anchor">
        /// 
        /// <see cref="FootnoteAnchor"/>
        /// to check
        /// </param>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if default style is needed,
        /// <see langword="false"/>
        /// otherwise
        /// </returns>
        public static bool IsDefaultStyleNeeded(FootnoteAnchor anchor) {
            return anchor.IsDefaultStyleNeeded();
        }

        /// <summary>Creates the default style for a footnote anchor in the main content.</summary>
        /// <remarks>
        /// Creates the default style for a footnote anchor in the main content.
        /// <para />
        /// The resulting style uses a reduced font size and positive text rise relative to the parent font size:
        /// <para />
        /// font size = parent font size * 0.5
        /// <para />
        /// text rise = parent font size * 0.6
        /// <para />
        /// If
        /// <paramref name="parentFontSize"/>
        /// is
        /// <see langword="null"/>
        /// 
        /// <c>12pt</c>
        /// is used as the base size.
        /// </remarks>
        /// <param name="parentFontSize">parent font size unit value</param>
        /// <returns>default style for a footnote anchor</returns>
        public static Style CreateDefaultFootnoteAnchorStyle(UnitValue parentFontSize) {
            float fontSize;
            if (parentFontSize == null) {
                fontSize = 12;
            }
            else {
                fontSize = parentFontSize.GetValue();
            }
            Style defaultStyle = new Style();
            float defaultFontSize = fontSize * DEFAULT_FOOTNOTE_ANCHOR_FONT_SIZE_SCALE_FACTOR;
            defaultStyle.SetProperty(Property.FONT_SIZE, UnitValue.CreatePointValue(defaultFontSize));
            float defaultTextRise = fontSize * DEFAULT_FOOTNOTE_ANCHOR_TEXT_RISE_SCALE_FACTOR;
            defaultStyle.SetProperty(Property.TEXT_RISE, defaultTextRise);
            return defaultStyle;
        }
    }
}
