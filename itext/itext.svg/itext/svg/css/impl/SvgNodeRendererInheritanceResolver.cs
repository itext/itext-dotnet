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
            if (subTree is TextSvgBranchRenderer) {
                TextSvgBranchRenderer subTreeAsBranch = (TextSvgBranchRenderer)subTree;
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
