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
using System.Collections.Generic;
using iText.Commons.Utils;
using iText.StyledXmlParser.Css;
using iText.StyledXmlParser.Node;

namespace iText.StyledXmlParser.Css.Page {
    /// <summary>
    /// <see cref="iText.StyledXmlParser.Css.CssContextNode"/>
    /// implementation for page contexts.
    /// </summary>
    public class PageContextNode : CssContextNode {
        /// <summary>The page type name.</summary>
        private String pageTypeName;

        /// <summary>The page classes.</summary>
        private IList<String> pageClasses;

        /// <summary>
        /// Creates a new
        /// <see cref="PageContextNode"/>
        /// instance.
        /// </summary>
        public PageContextNode()
            : this(null) {
        }

        /// <summary>
        /// Creates a new
        /// <see cref="PageContextNode"/>
        /// instance.
        /// </summary>
        /// <param name="parentNode">the parent node</param>
        public PageContextNode(INode parentNode)
            : base(parentNode) {
            this.pageClasses = new List<String>();
        }

        /// <summary>Adds a page class.</summary>
        /// <param name="pageClass">the page class</param>
        /// <returns>the page context node</returns>
        public virtual iText.StyledXmlParser.Css.Page.PageContextNode AddPageClass(String pageClass) {
            this.pageClasses.Add(pageClass.ToLowerInvariant());
            return this;
        }

        /// <summary>Gets the page type name.</summary>
        /// <returns>the page type name</returns>
        public virtual String GetPageTypeName() {
            return this.pageTypeName;
        }

        /// <summary>Sets the page type name.</summary>
        /// <param name="pageTypeName">the page type name</param>
        /// <returns>the page context node</returns>
        public virtual iText.StyledXmlParser.Css.Page.PageContextNode SetPageTypeName(String pageTypeName) {
            this.pageTypeName = pageTypeName;
            return this;
        }

        /// <summary>Gets the list of page classes.</summary>
        /// <returns>the page classes</returns>
        public virtual IList<String> GetPageClasses() {
            return JavaCollectionsUtil.UnmodifiableList(pageClasses);
        }
    }
}
