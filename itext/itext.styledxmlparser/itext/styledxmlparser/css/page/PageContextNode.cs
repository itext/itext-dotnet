/*
This file is part of the iText (R) project.
Copyright (c) 1998-2025 Apryse Group NV
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
