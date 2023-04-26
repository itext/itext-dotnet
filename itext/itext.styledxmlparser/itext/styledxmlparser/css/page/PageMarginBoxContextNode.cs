/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 Apryse Group NV
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
using System.Collections;
using System.Collections.Generic;
using iText.Commons.Utils;
using iText.Kernel.Geom;
using iText.StyledXmlParser.Css;
using iText.StyledXmlParser.Node;

namespace iText.StyledXmlParser.Css.Page {
    /// <summary>
    /// <see cref="iText.StyledXmlParser.Css.CssContextNode"/>
    /// implementation for page margin box contexts.
    /// </summary>
    public class PageMarginBoxContextNode : CssContextNode, ICustomElementNode {
        /// <summary>The Constant PAGE_MARGIN_BOX_TAG.</summary>
        public const String PAGE_MARGIN_BOX_TAG = "_064ef03_page-margin-box";

        /// <summary>The margin box name.</summary>
        private String marginBoxName;

        private Rectangle pageMarginBoxRectangle;

        private Rectangle containingBlockForMarginBox;

        /// <summary>
        /// Creates a new
        /// <see cref="PageMarginBoxContextNode"/>
        /// instance.
        /// </summary>
        /// <param name="parentNode">the parent node</param>
        /// <param name="marginBoxName">the margin box name</param>
        public PageMarginBoxContextNode(INode parentNode, String marginBoxName)
            : base(parentNode) {
            this.marginBoxName = marginBoxName;
            if (!(parentNode is PageContextNode)) {
                throw new ArgumentException("Page-margin-box context node shall have a page context node as parent.");
            }
        }

        /// <summary>Gets the margin box name.</summary>
        /// <returns>the margin box name</returns>
        public virtual String GetMarginBoxName() {
            return marginBoxName;
        }

        /// <summary>Sets the rectangle in which page margin box contents are shown.</summary>
        /// <param name="pageMarginBoxRectangle">
        /// the
        /// <see cref="iText.Kernel.Geom.Rectangle"/>
        /// defining position and dimensions of the margin box content
        /// area
        /// </param>
        public virtual void SetPageMarginBoxRectangle(Rectangle pageMarginBoxRectangle) {
            this.pageMarginBoxRectangle = pageMarginBoxRectangle;
        }

        /// <summary>Gets the rectangle in which page margin box contents should be shown.</summary>
        /// <returns>
        /// the
        /// <see cref="iText.Kernel.Geom.Rectangle"/>
        /// defining position and dimensions of the margin box content area
        /// </returns>
        public virtual Rectangle GetPageMarginBoxRectangle() {
            return pageMarginBoxRectangle;
        }

        /// <summary>
        /// Sets the containing block rectangle for the margin box, which is used for calculating
        /// some of the margin box properties relative values.
        /// </summary>
        /// <param name="containingBlockForMarginBox">
        /// the
        /// <see cref="iText.Kernel.Geom.Rectangle"/>
        /// which is used as a reference for some
        /// margin box relative properties calculations.
        /// </param>
        public virtual void SetContainingBlockForMarginBox(Rectangle containingBlockForMarginBox) {
            this.containingBlockForMarginBox = containingBlockForMarginBox;
        }

        /// <returns>
        /// the
        /// <see cref="iText.Kernel.Geom.Rectangle"/>
        /// which is used as a reference for some
        /// margin box relative properties calculations.
        /// </returns>
        public virtual Rectangle GetContainingBlockForMarginBox() {
            return containingBlockForMarginBox;
        }

        public virtual String Name() {
            return iText.StyledXmlParser.Css.Page.PageMarginBoxContextNode.PAGE_MARGIN_BOX_TAG;
        }

        public virtual IAttributes GetAttributes() {
            return new PageMarginBoxContextNode.AttributesStub();
        }

        public virtual String GetAttribute(String key) {
            return null;
        }

        public virtual IList<IDictionary<String, String>> GetAdditionalHtmlStyles() {
            return null;
        }

        public virtual void AddAdditionalHtmlStyles(IDictionary<String, String> styles) {
            throw new NotSupportedException();
        }

        public virtual String GetLang() {
            throw new NotSupportedException();
        }

        /// <summary>
        /// A simple
        /// <see cref="iText.StyledXmlParser.Node.IAttributes"/>
        /// implementation.
        /// </summary>
        private class AttributesStub : IAttributes {
            /// <summary><inheritDoc/></summary>
            public virtual String GetAttribute(String key) {
                return null;
            }

            /// <summary><inheritDoc/></summary>
            public virtual void SetAttribute(String key, String value) {
                throw new NotSupportedException();
            }

            /// <summary><inheritDoc/></summary>
            public virtual int Size() {
                return 0;
            }

            /// <summary><inheritDoc/></summary>
            public virtual IEnumerator<IAttribute> GetEnumerator() {
                return JavaCollectionsUtil.EmptyIterator<IAttribute>();
            }

        /// <summary><inheritDoc/></summary>
        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        }
    }
}
