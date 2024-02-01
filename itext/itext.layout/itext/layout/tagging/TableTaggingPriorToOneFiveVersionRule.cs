/*
This file is part of the iText (R) project.
Copyright (c) 1998-2024 Apryse Group NV
Authors: Apryse Software.

This program is offered under a commercial and under the AGPL license.
For commercial licensing, contact us at https://itextpdf.com/sales.  For AGPL licensing, see below.

AGPL licensing:
This program is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/
using System;
using System.Collections.Generic;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Tagging;
using iText.Kernel.Pdf.Tagutils;

namespace iText.Layout.Tagging {
    internal class TableTaggingPriorToOneFiveVersionRule : ITaggingRule {
        private ICollection<TaggingHintKey> finishForbidden = new HashSet<TaggingHintKey>();

        public virtual bool OnTagFinish(LayoutTaggingHelper taggingHelper, TaggingHintKey taggingHintKey) {
            if (taggingHintKey.GetAccessibleElement() != null) {
                String role = taggingHintKey.GetAccessibleElement().GetAccessibilityProperties().GetRole();
                if (StandardRoles.THEAD.Equals(role) || StandardRoles.TFOOT.Equals(role)) {
                    finishForbidden.Add(taggingHintKey);
                    return false;
                }
            }
            foreach (TaggingHintKey hint in taggingHelper.GetAccessibleKidsHint(taggingHintKey)) {
                String role = hint.GetAccessibleElement().GetAccessibilityProperties().GetRole();
                if (StandardRoles.TBODY.Equals(role) || StandardRoles.THEAD.Equals(role) || StandardRoles.TFOOT.Equals(role
                    )) {
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
