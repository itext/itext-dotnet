using System.Collections.Generic;
using iText.StyledXmlParser.Css;
using iText.StyledXmlParser.Css.Parse;
using iText.StyledXmlParser.Css.Selector;
using iText.StyledXmlParser.Node;

namespace iText.StyledXmlParser.Css.Selector.Item {
    internal class CssPseudoClassNotSelectorItem : CssPseudoClassSelectorItem {
        private ICssSelector argumentsSelector;

        internal CssPseudoClassNotSelectorItem(ICssSelector argumentsSelector)
            : base(CommonCssConstants.NOT, argumentsSelector.ToString()) {
            this.argumentsSelector = argumentsSelector;
        }

        public virtual IList<ICssSelectorItem> GetArgumentsSelector() {
            return CssSelectorParser.ParseSelectorItems(arguments);
        }

        public override bool Matches(INode node) {
            if (!(node is IElementNode) || node is ICustomElementNode || node is IDocumentNode) {
                return false;
            }
            return !argumentsSelector.Matches(node);
        }
    }
}
