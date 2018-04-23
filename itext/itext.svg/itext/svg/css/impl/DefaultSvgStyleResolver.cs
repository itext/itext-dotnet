/*
    This file is part of the iText (R) project.
    Copyright (c) 1998-2018 iText Group NV
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
using iText.StyledXmlParser;
using iText.StyledXmlParser.Css;
using iText.StyledXmlParser.Css.Media;
using iText.StyledXmlParser.Css.Parse;
using iText.StyledXmlParser.Node;
using iText.Svg;

namespace iText.Svg.Css.Impl {
    /// <summary>Default CSS resolver implementation.</summary>
    public class DefaultSvgStyleResolver : ICssResolver {
        private CssStyleSheet internalStyleSheet;

        /// <summary>Creates a DefaultSvgStyleResolver.</summary>
        /// <remarks>
        /// Creates a DefaultSvgStyleResolver. This constructor will instantiate its internal style sheet and it
        /// will collect the css declarations from the provided node.
        /// </remarks>
        /// <param name="rootNode">node to collect css from</param>
        public DefaultSvgStyleResolver(INode rootNode) {
            internalStyleSheet = new CssStyleSheet();
            CollectCssDeclarations(rootNode);
        }

        public virtual IDictionary<String, String> ResolveStyles(INode node, ICssContext context) {
            IDictionary<String, String> styles = new Dictionary<String, String>();
            //Load in defaults
            //TODO (RND-865): Figure out if defaults are necessary
            //Load in from collected style sheets
            IList<CssDeclaration> styleSheetDeclarations = internalStyleSheet.GetCssDeclarations(node, MediaDeviceDescription
                .CreateDefault());
            foreach (CssDeclaration ssd in styleSheetDeclarations) {
                styles.Put(ssd.GetProperty(), ssd.GetExpression());
            }
            //Load in inherited declarations from parent
            //TODO: RND-880
            //Load in attributes declarations
            if (node is IElementNode) {
                IElementNode eNode = (IElementNode)node;
                foreach (IAttribute attr in eNode.GetAttributes()) {
                    ProcessAttribute(attr, styles);
                }
            }
            return styles;
        }

        private void ProcessAttribute(IAttribute attr, IDictionary<String, String> styles) {
            //Style attribute needs to be parsed further
            if (attr.GetKey().Equals(AttributeConstants.STYLE)) {
                IDictionary<String, String> parsed = ParseStylesFromStyleAttribute(attr.GetValue());
                foreach (KeyValuePair<String, String> style in parsed) {
                    styles.Put(style.Key, style.Value);
                }
            }
            else {
                styles.Put(attr.GetKey(), attr.GetValue());
            }
        }

        private IDictionary<String, String> ParseStylesFromStyleAttribute(String style) {
            IDictionary<String, String> parsed = new Dictionary<String, String>();
            IList<CssDeclaration> declarations = CssRuleSetParser.ParsePropertyDeclarations(style);
            foreach (CssDeclaration declaration in declarations) {
                parsed.Put(declaration.GetProperty(), declaration.GetExpression());
            }
            return parsed;
        }

        private void CollectCssDeclarations(INode rootNode) {
            internalStyleSheet = new CssStyleSheet();
            LinkedList<INode> q = new LinkedList<INode>();
            if (rootNode != null) {
                q.Add(rootNode);
            }
            while (!q.IsEmpty()) {
                INode currentNode = q.JGetFirst();
                q.RemoveFirst();
                if (currentNode is IElementNode) {
                    IElementNode headChildElement = (IElementNode)currentNode;
                    if (headChildElement.Name().Equals(SvgTagConstants.STYLE)) {
                        //XML parser will parse style tag contents as text nodes
                        if (currentNode.ChildNodes().Count > 0 && (currentNode.ChildNodes()[0] is IDataNode || currentNode.ChildNodes
                            ()[0] is ITextNode)) {
                            String styleData;
                            if (currentNode.ChildNodes()[0] is IDataNode) {
                                styleData = ((IDataNode)currentNode.ChildNodes()[0]).GetWholeData();
                            }
                            else {
                                styleData = ((ITextNode)currentNode.ChildNodes()[0]).WholeText();
                            }
                            CssStyleSheet styleSheet = CssStyleSheetParser.Parse(styleData);
                            //TODO(RND-863): mediaquery wrap
                            //styleSheet = wrapStyleSheetInMediaQueryIfNecessary(headChildElement, styleSheet);
                            internalStyleSheet.AppendCssStyleSheet(styleSheet);
                        }
                    }
                }
                //TODO(RND-864): resolution of external style sheets via the link tag
                foreach (INode child in currentNode.ChildNodes()) {
                    if (child is IElementNode) {
                        q.Add(child);
                    }
                }
            }
        }
    }
}
