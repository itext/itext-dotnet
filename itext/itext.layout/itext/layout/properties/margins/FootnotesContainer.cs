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
using iText.Kernel.Pdf.Tagging;
using iText.Kernel.Pdf.Tagutils;
using iText.Layout.Element;
using iText.Layout.Renderer;

namespace iText.Layout.Properties.Margins {
//\cond DO_NOT_DOCUMENT
    /// <summary>
    /// Class representing container to store
    /// <see cref="iText.Layout.Element.Footnote"/>
    /// instances.
    /// </summary>
    internal class FootnotesContainer : BlockElement<iText.Layout.Properties.Margins.FootnotesContainer> {
        protected internal DefaultAccessibilityProperties tagProperties;

        /// <summary>
        /// Creates new
        /// <see cref="FootnotesContainer"/>
        /// instance.
        /// </summary>
        public FootnotesContainer() {
        }

        // Empty constructor.
        /// <summary>
        /// Adds
        /// <see cref="iText.Layout.Element.Footnote"/>
        /// to this container.
        /// </summary>
        /// <param name="footnote">
        /// 
        /// <see cref="iText.Layout.Element.Footnote"/>
        /// to add
        /// </param>
        /// <returns>
        /// this same
        /// <see cref="FootnotesContainer"/>
        /// instance
        /// </returns>
        public virtual iText.Layout.Properties.Margins.FootnotesContainer Add(Footnote footnote) {
            this.childElements.Add(footnote);
            return this;
        }

        /// <summary><inheritDoc/></summary>
        /// <returns>
        /// 
        /// <inheritDoc/>
        /// </returns>
        public override AccessibilityProperties GetAccessibilityProperties() {
            if (tagProperties == null) {
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
            return new FootnotesContainerRenderer(this);
        }
    }
//\endcond
}
