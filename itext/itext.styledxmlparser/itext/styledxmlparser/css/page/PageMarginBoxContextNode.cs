/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: Bruno Lowagie, Paulo Soares, et al.

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License version 3
as published by the Free Software Foundation with the addition of the
following permission added to Section 15 as permitted in Section 7(a):
FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
OF THIRD PARTY RIGHTS

This program is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU Affero General Public License for more details.
You should have received a copy of the GNU Affero General Public License
along with this program; if not, see http://www.gnu.org/licenses or write to
the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
Boston, MA, 02110-1301 USA, or download the license from the following URL:
http://itextpdf.com/terms-of-use/

The interactive user interfaces in modified source and object code versions
of this program must display Appropriate Legal Notices, as required under
Section 5 of the GNU Affero General Public License.

In accordance with Section 7(b) of the GNU Affero General Public License,
a covered work must retain the producer line in every PDF that is created
or manipulated using iText.

You can be released from the requirements of the license by purchasing
a commercial license. Buying such a license is mandatory as soon as you
develop commercial activities involving the iText software without
disclosing the source code of your own applications.
These activities include: offering paid services to customers as an ASP,
serving PDFs on the fly in a web application, shipping iText with a closed
source product.

For more information, please contact iText Software Corp. at this
address: sales@itextpdf.com
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
