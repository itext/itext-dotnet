using System;
using System.Collections.Generic;
using iText.Svg;
using iText.Svg.Renderers;
using iText.Svg.Renderers.Impl;

namespace iText.Svg.Css.Impl {
    /// <summary>
    /// Style and attribute inheritance resolver for
    /// <see cref="iText.Svg.Renderers.ISvgNodeRenderer"/>
    /// objects
    /// </summary>
    public class SvgNodeRendererInheritanceResolver {
        private StyleResolverUtil sru;

        public SvgNodeRendererInheritanceResolver() {
            sru = new StyleResolverUtil();
        }

        /// <summary>Apply style and attribute inheritance to the tree formed by the root and the subTree</summary>
        /// <param name="root">Renderer to consider as the root of the substree</param>
        /// <param name="subTree">
        /// tree of
        /// <see cref="iText.Svg.Renderers.ISvgNodeRenderer"/>
        /// s
        /// </param>
        public virtual void ApplyInheritanceToSubTree(ISvgNodeRenderer root, ISvgNodeRenderer subTree) {
            //Merge inherited style declarations from parent into child
            ApplyStyles(root, subTree);
            //If subtree, iterate over tree
            if (subTree is AbstractBranchSvgNodeRenderer) {
                AbstractBranchSvgNodeRenderer subTreeAsBranch = (AbstractBranchSvgNodeRenderer)subTree;
                foreach (ISvgNodeRenderer child in subTreeAsBranch.GetChildren()) {
                    ApplyInheritanceToSubTree(subTreeAsBranch, child);
                }
            }
        }

        protected internal virtual void ApplyStyles(ISvgNodeRenderer parent, ISvgNodeRenderer child) {
            if (parent != null && child != null) {
                IDictionary<String, String> childStyles = child.GetAttributeMapCopy();
                if (childStyles == null) {
                    childStyles = new Dictionary<String, String>();
                }
                IDictionary<String, String> parentStyles = parent.GetAttributeMapCopy();
                String parentFontSize = parent.GetAttribute(SvgConstants.Attributes.FONT_SIZE);
                if (parentFontSize == null) {
                    parentFontSize = "0";
                }
                foreach (KeyValuePair<String, String> parentAttribute in parentStyles) {
                    sru.MergeParentStyleDeclaration(childStyles, parentAttribute.Key, parentAttribute.Value, parentFontSize);
                }
                child.SetAttributesAndStyles(childStyles);
            }
        }
    }
}
