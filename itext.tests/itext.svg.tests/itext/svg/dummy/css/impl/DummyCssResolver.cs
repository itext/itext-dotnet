using System;
using System.Collections.Generic;
using iText.StyledXmlParser.Css;
using iText.StyledXmlParser.Node;

namespace iText.Svg.Dummy.Css.Impl {
    public class DummyCssResolver : ICssResolver {
        public virtual IDictionary<String, String> ResolveStyles(INode node, ICssContext context) {
            return new Dictionary<String, String>();
        }
    }
}
