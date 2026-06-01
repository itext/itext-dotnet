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
using iText.Kernel.Pdf.Tagutils;
using iText.Layout.Renderer;
using iText.Layout.Tagging;

namespace iText.Layout.Element {
    /// <summary>Class representing an anchor for a footnote which is placed at the bottom of the page.</summary>
    /// <remarks>
    /// Class representing an anchor for a footnote which is placed at the bottom of the page.
    /// Footnote anchor indicates footnote in the text with superscript numbers (or letters or other symbols).
    /// </remarks>
    public class FootnoteAnchor : AbstractElement<iText.Layout.Element.FootnoteAnchor>, IAccessibleElement {
        private const int DEFAULT_FONT_SIZE = 6;

        private const int DEFAULT_TEXT_RISE = 7;

        private readonly IElement footnoteAnchor;

        private readonly Footnote footnote;

        /// <summary>
        /// Creates new
        /// <see cref="FootnoteAnchor"/>
        /// instance.
        /// </summary>
        /// <param name="text">superscript text for anchor to indicate a footnote</param>
        /// <param name="footnote">
        /// 
        /// <see cref="Footnote"/>
        /// linked to this anchor
        /// </param>
        public FootnoteAnchor(String text, Footnote footnote)
            : this(new Text(text).SetFontSize(DEFAULT_FONT_SIZE).SetTextRise(DEFAULT_TEXT_RISE), footnote) {
        }

        /// <summary>
        /// Creates new
        /// <see cref="FootnoteAnchor"/>
        /// instance.
        /// </summary>
        /// <param name="text">
        /// 
        /// <see cref="Text"/>
        /// for anchor to indicate a footnote
        /// </param>
        /// <param name="footnote">
        /// 
        /// <see cref="Footnote"/>
        /// linked to this anchor
        /// </param>
        public FootnoteAnchor(Text text, Footnote footnote) {
            this.footnoteAnchor = text;
            this.footnote = footnote;
        }

        /// <summary>
        /// Creates new
        /// <see cref="FootnoteAnchor"/>
        /// instance.
        /// </summary>
        /// <param name="image">
        /// 
        /// <see cref="Image"/>
        /// to use as footnote anchor
        /// </param>
        /// <param name="footnote">
        /// 
        /// <see cref="Footnote"/>
        /// linked to this anchor
        /// </param>
        public FootnoteAnchor(Image image, Footnote footnote) {
            this.footnoteAnchor = image;
            this.footnote = footnote;
        }

        /// <summary>
        /// Gets layout element
        /// <see cref="IElement"/>
        /// representing footnote anchor.
        /// </summary>
        /// <returns>
        /// 
        /// <see cref="IElement"/>
        /// representing footnote anchor (can be
        /// <see cref="Text"/>
        /// or
        /// <see cref="Image"/>
        /// )
        /// </returns>
        public virtual IElement GetFootnoteAnchor() {
            return footnoteAnchor;
        }

        /// <summary>
        /// Gets
        /// <see cref="Footnote"/>
        /// linked to this anchor.
        /// </summary>
        /// <returns>
        /// 
        /// <see cref="Footnote"/>
        /// linked to this anchor
        /// </returns>
        public virtual Footnote GetFootnote() {
            return footnote;
        }

        /// <summary><inheritDoc/></summary>
        /// <returns>
        /// 
        /// <inheritDoc/>
        /// </returns>
        public virtual AccessibilityProperties GetAccessibilityProperties() {
            // TODO DEVSIX-9997 Support correct footnotes tagging
            return footnoteAnchor is IAccessibleElement ? ((IAccessibleElement)footnoteAnchor).GetAccessibilityProperties
                () : null;
        }

        /// <summary><inheritDoc/></summary>
        /// <returns>
        /// 
        /// <inheritDoc/>
        /// </returns>
        protected internal override IRenderer MakeNewRenderer() {
            return new FootnoteAnchorRenderer(this);
        }
    }
}
