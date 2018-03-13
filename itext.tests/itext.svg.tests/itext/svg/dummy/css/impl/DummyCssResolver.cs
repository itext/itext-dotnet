using System;
using System.Collections.Generic;
using iText.StyledXmlParser.Node;
using iText.Svg.Css;

namespace iText.Svg.Dummy.Css.Impl {
    public class DummyCssResolver : ICssResolver {
        public virtual IDictionary<String, String> ResolveStyles(INode node, CssContext context) {
            return new Dictionary<String, String>();
        }
    }
}
