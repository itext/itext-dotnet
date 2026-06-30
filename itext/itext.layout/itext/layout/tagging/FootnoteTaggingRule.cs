/*
This file is part of the iText (R) project.
Copyright (c) 1998-2026 Apryse Group NV
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
