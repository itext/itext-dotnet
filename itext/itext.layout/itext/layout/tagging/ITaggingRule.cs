namespace iText.Layout.Tagging {
    internal interface ITaggingRule {
        bool OnTagFinish(LayoutTaggingHelper taggingHelper, TaggingHintKey taggingHintKey);
    }
}
