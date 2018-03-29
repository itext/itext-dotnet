using System;
using System.Collections.Generic;
using iText.StyledXmlParser;
using iText.StyledXmlParser.Css;
using iText.StyledXmlParser.Css.Media;
using iText.StyledXmlParser.Css.Parse;
using iText.StyledXmlParser.Node;
using iText.Svg;

namespace iText.Svg.Css.Impl {
    public class DefaultSvgStyleResolver : ICssResolver {
        private CssStyleSheet internalStyleSheet;

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
            //TODO: parent inheritance
            //Load in attributes declarations
            if (node is IElementNode) {
                IElementNode eNode = (IElementNode)node;
                foreach (IAttribute attr in eNode.GetAttributes()) {
                    //TODO: filter out svg tags
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
        //TODO(RND-863): Media query resolution
        //    private CssStyleSheet wrapStyleSheetInMediaQueryIfNecessary(IElementNode headChildElement, CssStyleSheet styleSheet) {
        //
        //        String mediaAttribute = headChildElement.getAttribute(AttributeConstants.MEDIA);
        //        if (mediaAttribute != null && mediaAttribute.length() > 0) {
        //            CssMediaRule mediaRule = new CssMediaRule(mediaAttribute);
        //            mediaRule.addStatementsToBody(styleSheet.getStatements());
        //            styleSheet = new CssStyleSheet();
        //            styleSheet.addStatement(mediaRule);
        //        }
        //        return styleSheet;
        //        throw new NotImplementedException();
        //    }
    }
}
