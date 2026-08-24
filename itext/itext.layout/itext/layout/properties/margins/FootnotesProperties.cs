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
using iText.Layout;

namespace iText.Layout.Properties.Margins {
    /// <summary>Class representing properties to customize footnotes.</summary>
    /// <remarks>
    /// Class representing properties to customize footnotes.
    /// <para />
    /// Can be specified via
    /// <see cref="iText.Layout.Document.SetFootnotesProperties(FootnotesProperties)"/>
    /// or
    /// <see cref="iText.Layout.Element.SectionBreak.SetFootnotesProperties(FootnotesProperties)"/>.
    /// </remarks>
    public class FootnotesProperties {
        private FootnoteNumberingType? footnoteNumberingType;

        private FootnoteNumberingConfig footnoteNumberingConfig = FootnoteNumberingConfig.PER_PAGE;

        private Style footnotesContainerStyle = null;

        private Style footnoteAnchorLabelStyle = null;

        private Style footnoteAnchorStyle = null;

        /// <summary>
        /// Creates new
        /// <see cref="FootnotesProperties"/>
        /// instance.
        /// </summary>
        public FootnotesProperties() {
        }

        // Empty constructor in order for default one to not be removed if another one is added.
        /// <summary>
        /// Gets
        /// <see cref="FootnoteNumberingType?"/>
        /// representing numbering type for footnote anchors.
        /// </summary>
        /// <returns>
        /// 
        /// <see cref="FootnoteNumberingType?"/>
        /// numbering type
        /// </returns>
        public virtual FootnoteNumberingType? GetFootnoteNumberingType() {
            return footnoteNumberingType;
        }

        /// <summary>
        /// Sets
        /// <see cref="FootnoteNumberingType?"/>
        /// representing numbering type for footnote anchors.
        /// </summary>
        /// <param name="footnoteNumberingType">
        /// 
        /// <see cref="FootnoteNumberingType?"/>
        /// representing numbering type for footnote anchors
        /// </param>
        /// <returns>
        /// this same
        /// <see cref="FootnotesProperties"/>
        /// instance
        /// </returns>
        public virtual iText.Layout.Properties.Margins.FootnotesProperties SetFootnoteNumberingType(FootnoteNumberingType?
             footnoteNumberingType) {
            this.footnoteNumberingType = footnoteNumberingType;
            return this;
        }

        /// <summary>
        /// Gets
        /// <see cref="FootnoteNumberingConfig"/>
        /// representing numbering configuration for footnotes.
        /// </summary>
        /// <returns>
        /// 
        /// <see cref="FootnoteNumberingConfig"/>
        /// representing footnotes numbering configuration
        /// </returns>
        public virtual FootnoteNumberingConfig GetFootnoteNumberingConfig() {
            return footnoteNumberingConfig;
        }

        /// <summary>
        /// Sets
        /// <see cref="FootnoteNumberingConfig"/>
        /// representing numbering configuration for footnotes.
        /// </summary>
        /// <param name="footnoteNumberingConfig">
        /// 
        /// <see cref="FootnoteNumberingConfig"/>
        /// representing footnotes numbering configuration
        /// </param>
        /// <returns>
        /// this same
        /// <see cref="FootnotesProperties"/>
        /// instance
        /// </returns>
        public virtual iText.Layout.Properties.Margins.FootnotesProperties SetFootnoteNumberingConfig(FootnoteNumberingConfig
             footnoteNumberingConfig) {
            this.footnoteNumberingConfig = footnoteNumberingConfig;
            return this;
        }

        /// <summary>
        /// Gets
        /// <see cref="iText.Layout.Style"/>
        /// storing style properties for footnotes container.
        /// </summary>
        /// <returns>
        /// 
        /// <see cref="iText.Layout.Style"/>
        /// storing properties for footnotes container
        /// </returns>
        public virtual Style GetFootnotesContainerStyle() {
            return footnotesContainerStyle;
        }

        /// <summary>
        /// Sets
        /// <see cref="iText.Layout.Style"/>
        /// storing style properties for footnotes container.
        /// </summary>
        /// <param name="footnotesContainerStyle">
        /// 
        /// <see cref="iText.Layout.Style"/>
        /// storing properties for footnotes container
        /// </param>
        /// <returns>
        /// this same
        /// <see cref="FootnotesProperties"/>
        /// instance
        /// </returns>
        public virtual iText.Layout.Properties.Margins.FootnotesProperties SetFootnotesContainerStyle(Style footnotesContainerStyle
            ) {
            this.footnotesContainerStyle = footnotesContainerStyle;
            return this;
        }

        /// <summary>
        /// Gets
        /// <see cref="iText.Layout.Style"/>
        /// storing style properties for footnote anchors that are placed inside the footnotes container.
        /// </summary>
        /// <returns>
        /// 
        /// <see cref="iText.Layout.Style"/>
        /// storing properties for footnote anchors that are inside the footnotes
        /// </returns>
        public virtual Style GetFootnoteAnchorLabelStyle() {
            return footnoteAnchorLabelStyle;
        }

        /// <summary>
        /// Sets
        /// <see cref="iText.Layout.Style"/>
        /// storing style properties for footnote anchors that are placed inside the footnotes container.
        /// </summary>
        /// <param name="footnoteAnchorLabelStyle">
        /// 
        /// <see cref="iText.Layout.Style"/>
        /// storing properties for footnote anchors inside the footnotes
        /// </param>
        /// <returns>
        /// this same
        /// <see cref="FootnotesProperties"/>
        /// instance
        /// </returns>
        public virtual iText.Layout.Properties.Margins.FootnotesProperties SetFootnoteAnchorLabelStyle(Style footnoteAnchorLabelStyle
            ) {
            this.footnoteAnchorLabelStyle = footnoteAnchorLabelStyle;
            return this;
        }

        /// <summary>
        /// Gets
        /// <see cref="iText.Layout.Style"/>
        /// storing style properties for footnote anchors.
        /// </summary>
        /// <returns>
        /// 
        /// <see cref="iText.Layout.Style"/>
        /// storing properties for footnote anchors
        /// </returns>
        public virtual Style GetFootnoteAnchorStyle() {
            return footnoteAnchorStyle;
        }

        /// <summary>
        /// Sets
        /// <see cref="iText.Layout.Style"/>
        /// storing style properties for footnote anchors.
        /// </summary>
        /// <param name="footnoteAnchorStyle">
        /// 
        /// <see cref="iText.Layout.Style"/>
        /// storing properties for footnote anchors
        /// </param>
        /// <returns>
        /// this same
        /// <see cref="FootnotesProperties"/>
        /// instance
        /// </returns>
        public virtual iText.Layout.Properties.Margins.FootnotesProperties SetFootnoteAnchorStyle(Style footnoteAnchorStyle
            ) {
            this.footnoteAnchorStyle = footnoteAnchorStyle;
            return this;
        }
    }
}
