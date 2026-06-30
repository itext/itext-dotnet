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
using iText.Kernel.Pdf.Tagging;
using iText.Kernel.Pdf.Tagutils;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Renderer;
using iText.Layout.Tagging;

namespace iText.Layout.Properties.Margins {
    /// <summary>Class representing an anchor for a footnote which is placed at the bottom of the page.</summary>
    /// <remarks>
    /// Class representing an anchor for a footnote which is placed at the bottom of the page.
    /// Footnote anchor indicates footnote in the text with superscript numbers (or letters or other symbols).
    /// </remarks>
    public class FootnoteAnchor : AbstractElement<iText.Layout.Properties.Margins.FootnoteAnchor>, IAccessibleElement {
        private const int DEFAULT_FONT_SIZE = 6;

        private const int DEFAULT_TEXT_RISE = 7;

        protected internal DefaultAccessibilityProperties tagProperties;

        private readonly Footnote footnote;

        private IElement footnoteAnchor;

        private Style footnoteAnchorLabelStyle = null;

        /// <summary>
        /// Creates new
        /// <see cref="FootnoteAnchor"/>
        /// instance.
        /// </summary>
        /// <param name="footnote">
        /// 
        /// <see cref="Footnote"/>
        /// linked to this anchor
        /// </param>
        public FootnoteAnchor(Footnote footnote)
            // Footnote anchor be set automatically based on FootnoteNumberingType. Asterisk is used as default value.
            : this(new Text("*"), footnote) {
        }

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
            // TODO DEVSIX-10031 Do not specify constant font size by default,
            //  it should depend on parent paragraph font size.
            : this(new Text(text).SetFontSize(DEFAULT_FONT_SIZE).SetTextRise(DEFAULT_TEXT_RISE).SetNeutralRole(), footnote
                ) {
        }

        /// <summary>
        /// Creates new
        /// <see cref="FootnoteAnchor"/>
        /// instance.
        /// </summary>
        /// <param name="text">
        /// 
        /// <see cref="iText.Layout.Element.Text"/>
        /// for anchor to indicate a footnote
        /// </param>
        /// <param name="footnote">
        /// 
        /// <see cref="Footnote"/>
        /// linked to this anchor
        /// </param>
        public FootnoteAnchor(Text text, Footnote footnote) {
            this.footnote = footnote;
            this.SetFootnoteAnchor(text);
        }

        /// <summary>
        /// Creates new
        /// <see cref="FootnoteAnchor"/>
        /// instance.
        /// </summary>
        /// <param name="image">
        /// 
        /// <see cref="iText.Layout.Element.Image"/>
        /// to use as footnote anchor
        /// </param>
        /// <param name="footnote">
        /// 
        /// <see cref="Footnote"/>
        /// linked to this anchor
        /// </param>
        public FootnoteAnchor(Image image, Footnote footnote) {
            this.footnote = footnote;
            this.SetFootnoteAnchor(image);
        }

        /// <summary>
        /// Gets layout element
        /// <see cref="iText.Layout.Element.IElement"/>
        /// representing footnote anchor.
        /// </summary>
        /// <returns>
        /// 
        /// <see cref="iText.Layout.Element.IElement"/>
        /// representing footnote anchor (can be
        /// <see cref="iText.Layout.Element.Text"/>
        /// or
        /// <see cref="iText.Layout.Element.Image"/>
        /// )
        /// </returns>
        public virtual IElement GetFootnoteAnchor() {
            return footnoteAnchor;
        }

        /// <summary>
        /// Sets
        /// <see cref="iText.Layout.Element.Text"/>
        /// layout element representing footnote anchor.
        /// </summary>
        /// <param name="footnoteAnchor">
        /// 
        /// <see cref="iText.Layout.Element.Text"/>
        /// layout element representing footnote anchor
        /// </param>
        /// <returns>
        /// this same
        /// <see cref="FootnoteAnchor"/>
        /// instance
        /// </returns>
        public virtual iText.Layout.Properties.Margins.FootnoteAnchor SetFootnoteAnchor(Text footnoteAnchor) {
            this.footnoteAnchor = footnoteAnchor;
            this.footnote.ApplyFootnoteAnchor(this);
            return this;
        }

        /// <summary>
        /// Sets
        /// <see cref="iText.Layout.Element.Image"/>
        /// layout element representing footnote anchor.
        /// </summary>
        /// <param name="footnoteAnchor">
        /// 
        /// <see cref="iText.Layout.Element.Image"/>
        /// layout element representing footnote anchor
        /// </param>
        /// <returns>
        /// this same
        /// <see cref="FootnoteAnchor"/>
        /// instance
        /// </returns>
        public virtual iText.Layout.Properties.Margins.FootnoteAnchor SetFootnoteAnchor(Image footnoteAnchor) {
            this.footnoteAnchor = footnoteAnchor;
            this.footnote.ApplyFootnoteAnchor(this);
            return this;
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
            if (tagProperties == null) {
                tagProperties = new DefaultAccessibilityProperties(StandardRoles.REFERENCE);
            }
            return tagProperties;
        }

//\cond DO_NOT_DOCUMENT
        /// <summary>
        /// Gets
        /// <see cref="iText.Layout.Style"/>
        /// storing style properties for footnote anchor that is placed inside the footnote.
        /// </summary>
        /// <returns>
        /// 
        /// <see cref="iText.Layout.Style"/>
        /// storing properties for footnote anchor that is inside the footnote
        /// </returns>
        internal virtual Style GetFootnoteAnchorLabelStyle() {
            return footnoteAnchorLabelStyle;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>
        /// Sets
        /// <see cref="iText.Layout.Style"/>
        /// storing style properties for footnote anchor that is placed inside the footnote.
        /// </summary>
        /// <param name="footnoteAnchorLabelStyle">
        /// 
        /// <see cref="iText.Layout.Style"/>
        /// storing properties for footnote anchor inside the footnote
        /// </param>
        /// <returns>
        /// this same
        /// <see cref="FootnoteAnchor"/>
        /// instance
        /// </returns>
        internal virtual iText.Layout.Properties.Margins.FootnoteAnchor SetFootnoteAnchorLabelStyle(Style footnoteAnchorLabelStyle
            ) {
            this.footnoteAnchorLabelStyle = footnoteAnchorLabelStyle;
            this.footnote.ApplyFootnoteAnchor(this);
            return this;
        }
//\endcond

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
