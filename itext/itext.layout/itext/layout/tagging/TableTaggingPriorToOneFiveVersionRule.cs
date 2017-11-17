using System.Collections.Generic;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Tagutils;

namespace iText.Layout.Tagging {
    internal class TableTaggingPriorToOneFiveVersionRule : ITaggingRule {
        private ICollection<TaggingHintKey> finishForbidden = new HashSet<TaggingHintKey>();

        public virtual bool OnTagFinish(LayoutTaggingHelper taggingHelper, TaggingHintKey taggingHintKey) {
            if (taggingHintKey.GetAccessibleElement() != null) {
                PdfName role = taggingHintKey.GetAccessibleElement().GetRole();
                if (PdfName.THead.Equals(role) || PdfName.TFoot.Equals(role)) {
                    finishForbidden.Add(taggingHintKey);
                    return false;
                }
            }
            foreach (TaggingHintKey hint in taggingHelper.GetAccessibleKidsHint(taggingHintKey)) {
                PdfName role = hint.GetAccessibleElement().GetRole();
                if (PdfName.TBody.Equals(role) || PdfName.THead.Equals(role) || PdfName.TFoot.Equals(role)) {
                    // THead and TFoot are not finished thanks to this rule logic, TBody not finished because it's dummy and Table itself not finished
                    RemoveTagUnavailableInPriorToOneDotFivePdf(hint, taggingHelper);
                }
            }
            return true;
        }

        private void RemoveTagUnavailableInPriorToOneDotFivePdf(TaggingHintKey taggingHintKey, LayoutTaggingHelper
             taggingHelper) {
            taggingHelper.ReplaceKidHint(taggingHintKey, taggingHelper.GetAccessibleKidsHint(taggingHintKey));
            PdfDocument pdfDocument = taggingHelper.GetPdfDocument();
            WaitingTagsManager waitingTagsManager = pdfDocument.GetTagStructureContext().GetWaitingTagsManager();
            TagTreePointer tagPointer = new TagTreePointer(pdfDocument);
            if (waitingTagsManager.TryMovePointerToWaitingTag(tagPointer, taggingHintKey)) {
                waitingTagsManager.RemoveWaitingState(taggingHintKey);
                tagPointer.RemoveTag();
            }
            if (finishForbidden.Remove(taggingHintKey)) {
                taggingHintKey.SetFinished();
            }
        }
    }
}
