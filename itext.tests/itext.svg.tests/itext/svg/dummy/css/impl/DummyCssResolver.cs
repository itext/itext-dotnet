using System;
using System.Collections.Generic;
using iText.StyledXmlParser.Css;
using iText.StyledXmlParser.Node;

namespace iText.Svg.Dummy.Css.Impl {
    public class DummyCssResolver : ICssResolver {
        public virtual IDictionary<String, String> ResolveStyles(INode node, ICssContext context) {
            IDictionary<String, String> styles = new Dictionary<String, String>();
            if (node is IElementNode) {
                IElementNode eNode = (IElementNode)node;
                foreach (IAttribute attr in eNode.GetAttributes()) {
                    styles.Put(attr.GetKey(), attr.GetValue());
                }
            }
            return styles;
        }
    }
}
