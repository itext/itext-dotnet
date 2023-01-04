/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: iText Software.

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
using iText.StyledXmlParser.Util;
using iText.Svg;
using iText.Svg.Css;
using iText.Svg.Renderers;
using iText.Svg.Renderers.Impl;

namespace iText.Svg.Css.Impl {
    /// <summary>
    /// Style and attribute inheritance resolver for
    /// <see cref="iText.Svg.Renderers.ISvgNodeRenderer"/>
    /// objects.
    /// </summary>
    public sealed class SvgNodeRendererInheritanceResolver {
        private SvgNodeRendererInheritanceResolver() {
        }

        /// <summary>Apply style and attribute inheritance to the tree formed by the root and the subTree.</summary>
        /// <param name="root">the renderer to consider as the root of the subtree</param>
        /// <param name="subTree">
        /// the tree of
        /// <see cref="iText.Svg.Renderers.ISvgNodeRenderer"/>
        /// </param>
        /// <param name="cssContext">the current SVG CSS context</param>
        public static void ApplyInheritanceToSubTree(ISvgNodeRenderer root, ISvgNodeRenderer subTree, SvgCssContext
             cssContext) {
            // Merge inherited style declarations from parent into child
            ApplyStyles(root, subTree, cssContext);
            // If subtree, iterate over tree
            if (subTree is AbstractBranchSvgNodeRenderer) {
                AbstractBranchSvgNodeRenderer subTreeAsBranch = (AbstractBranchSvgNodeRenderer)subTree;
                foreach (ISvgNodeRenderer child in subTreeAsBranch.GetChildren()) {
                    ApplyInheritanceToSubTree(subTreeAsBranch, child, cssContext);
                }
            }
        }

        private static void ApplyStyles(ISvgNodeRenderer parent, ISvgNodeRenderer child, SvgCssContext cssContext) {
            if (parent != null && child != null) {
                IDictionary<String, String> childStyles = child.GetAttributeMapCopy();
                if (childStyles == null) {
                    childStyles = new Dictionary<String, String>();
                }
                IDictionary<String, String> parentStyles = parent.GetAttributeMapCopy();
                String parentFontSize = parent.GetAttribute(SvgConstants.Attributes.FONT_SIZE);
                foreach (KeyValuePair<String, String> parentAttribute in parentStyles) {
                    childStyles = StyleUtil.MergeParentStyleDeclaration(childStyles, parentAttribute.Key, parentAttribute.Value
                        , parentFontSize, SvgStyleResolver.INHERITANCE_RULES);
                }
                SvgStyleResolver.ResolveFontSizeStyle(childStyles, cssContext, parentFontSize);
                child.SetAttributesAndStyles(childStyles);
            }
        }
    }
}
