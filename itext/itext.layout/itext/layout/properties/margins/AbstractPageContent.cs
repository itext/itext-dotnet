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
using iText.Kernel.Geom;
using iText.Layout.Element;

namespace iText.Layout.Properties.Margins {
    /// <summary>Abstract class representing page content such as margins of footnotes.</summary>
    public abstract class AbstractPageContent {
        private readonly IElement content;

        private Rectangle rectangle;

        /// <summary>
        /// Creates new
        /// <see cref="AbstractPageContent"/>
        /// instance.
        /// </summary>
        /// <param name="content">
        /// 
        /// <see cref="iText.Layout.Element.IElement"/>
        /// layout element with page content
        /// </param>
        protected internal AbstractPageContent(IElement content) {
            this.content = content;
        }

        /// <summary>
        /// Creates new
        /// <see cref="AbstractPageContent"/>
        /// instance by copying existing one.
        /// </summary>
        /// <param name="other">
        /// 
        /// <see cref="AbstractPageContent"/>
        /// instance to copy
        /// </param>
        protected internal AbstractPageContent(iText.Layout.Properties.Margins.AbstractPageContent other) {
            this.content = other.content;
            this.rectangle = other.rectangle;
        }

        /// <summary>Returns layout element representing page content.</summary>
        /// <returns>
        /// 
        /// <see cref="iText.Layout.Element.IElement"/>
        /// layout element for page margin content
        /// </returns>
        public virtual IElement GetContent() {
            return content;
        }

//\cond DO_NOT_DOCUMENT
        /// <summary>Sets the rectangle in which page content is shown.</summary>
        /// <param name="rectangle">
        /// 
        /// <see cref="iText.Kernel.Geom.Rectangle"/>
        /// defining position and dimensions of the content area
        /// </param>
        internal virtual void SetRectangle(Rectangle rectangle) {
            this.rectangle = rectangle;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Gets the rectangle in which page content should be shown.</summary>
        /// <returns>
        /// the
        /// <see cref="iText.Kernel.Geom.Rectangle"/>
        /// defining position and dimensions of the content area
        /// </returns>
        internal virtual Rectangle GetRectangle() {
            return rectangle;
        }
//\endcond
    }
}
