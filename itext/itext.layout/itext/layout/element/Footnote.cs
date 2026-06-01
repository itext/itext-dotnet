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
using iText.Layout.Renderer;
using iText.Layout.Tagging;

namespace iText.Layout.Element {
    /// <summary>
    /// Class representing a single footnote which is a note placed at the bottom of the page
    /// indicated in the text with superscript numbers (or letters or other symbols).
    /// </summary>
    public class Footnote : AbstractElement<iText.Layout.Element.Footnote>, IAccessibleElement {
        protected internal DefaultAccessibilityProperties tagProperties;

        /// <summary>
        /// Creates new
        /// <see cref="Footnote"/>
        /// instance with text.
        /// </summary>
        /// <param name="text">the textual contents of the footnote</param>
        public Footnote(String text)
            : this(new Paragraph(text).SetMarginTop(0).SetMarginBottom(0)) {
        }

        /// <summary>
        /// Creates new
        /// <see cref="Footnote"/>
        /// instance.
        /// </summary>
        /// <param name="paragraph">
        /// 
        /// <see cref="Paragraph"/>
        /// representing the contents of the footnote
        /// </param>
        public Footnote(Paragraph paragraph)
            : base() {
            childElements.Add(paragraph);
        }

        /// <summary><inheritDoc/></summary>
        /// <returns>
        /// 
        /// <inheritDoc/>
        /// </returns>
        public virtual AccessibilityProperties GetAccessibilityProperties() {
            if (tagProperties == null) {
                // Although we mark is as P here, it'll be an artifact due to PageMarginBoxes#setPageMarginTagRole method.
                // TODO DEVSIX-9997 Support correct footnotes tagging
                tagProperties = new DefaultAccessibilityProperties(StandardRoles.P);
            }
            return tagProperties;
        }

        /// <summary><inheritDoc/></summary>
        /// <returns>
        /// 
        /// <inheritDoc/>
        /// </returns>
        protected internal override IRenderer MakeNewRenderer() {
            return new FootnoteRenderer(this);
        }
    }
}
