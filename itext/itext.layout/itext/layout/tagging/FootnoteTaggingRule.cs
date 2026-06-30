using iText.Layout.Element;
using iText.Layout.Properties.Margins;

namespace iText.Layout.Tagging {
//\cond DO_NOT_DOCUMENT
    internal class FootnoteTaggingRule : ITaggingRule {
        public FootnoteTaggingRule() {
        }

        //default constructor
        public virtual bool OnTagFinish(LayoutTaggingHelper taggingHelper, TaggingHintKey taggingHintKey) {
            if (taggingHintKey.GetAccessibleElement() is FootnoteAnchor) {
                // get to footnote child
                TaggingHintKey footnoteTag = null;
                foreach (TaggingHintKey child in taggingHelper.GetKidsHint(taggingHintKey)) {
                    if (child.GetAccessibleElement() is Footnote) {
                        footnoteTag = child;
                        break;
                    }
                }
                if (footnoteTag == null) {
                    return true;
                }
                //find paragraph parent
                TaggingHintKey pk = taggingHelper.GetParentHint(taggingHintKey);
                while (pk != null && !(pk.GetAccessibleElement() is Paragraph)) {
                    pk = taggingHelper.GetParentHint(pk);
                }
                if (pk != null) {
                    taggingHelper.MoveKidHint(footnoteTag, pk);
                }
            }
            return true;
        }
    }
//\endcond
}
