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
using iText.Commons.Utils;
using iText.Kernel.Pdf.Tagging;
using iText.Layout.Element;
using iText.Layout.Properties;

namespace iText.Layout.Tagging {
//\cond DO_NOT_DOCUMENT
    internal class TableTaggingRule : ITaggingRule {
        public virtual bool OnTagFinish(LayoutTaggingHelper taggingHelper, TaggingHintKey tableHintKey) {
            IList<TaggingHintKey> kidKeys = taggingHelper.GetAccessibleKidsHint(tableHintKey);
            IDictionary<int, SortedDictionary<int, TaggingHintKey>> tableTags = new SortedDictionary<int, SortedDictionary
                <int, TaggingHintKey>>();
            IList<TaggingHintKey> tableCellTagsUnindexed = new List<TaggingHintKey>();
            IList<TaggingHintKey> nonCellKids = new List<TaggingHintKey>();
            foreach (TaggingHintKey kidKey in kidKeys) {
                String kidRole = GetKidRole(kidKey, taggingHelper);
                bool isCell = StandardRoles.TD.Equals(kidRole) || StandardRoles.TH.Equals(kidRole);
                if (isCell && kidKey.GetAccessibleElement() is Cell) {
                    Cell cell = (Cell)kidKey.GetAccessibleElement();
                    int rowInd = cell.GetRow();
                    int colInd = cell.GetCol();
                    SortedDictionary<int, TaggingHintKey> rowTags = tableTags.Get(rowInd);
                    if (rowTags == null) {
                        rowTags = new SortedDictionary<int, TaggingHintKey>();
                        tableTags.Put(rowInd, rowTags);
                    }
                    rowTags.Put(colInd, kidKey);
                }
                else {
                    if (isCell) {
                        tableCellTagsUnindexed.Add(kidKey);
                    }
                    else {
                        nonCellKids.Add(kidKey);
                    }
                }
            }
            TaggingDummyElement tbodyTag = GetTbodyTag(tableHintKey);
            foreach (TaggingHintKey nonCellKid in nonCellKids) {
                String kidRole = GetKidRole(nonCellKid, taggingHelper);
                if (!StandardRoles.THEAD.Equals(kidRole) && !StandardRoles.TFOOT.Equals(kidRole) && !StandardRoles.CAPTION
                    .Equals(kidRole)) {
                    // In usual cases it isn't expected that this for loop will work, but it is possible to
                    // create custom tag hierarchy by specifying role, and put any child to tableHintKey
                    taggingHelper.MoveKidHint(nonCellKid, tableHintKey);
                }
            }
            foreach (TaggingHintKey nonCellKid in nonCellKids) {
                if (StandardRoles.THEAD.Equals(GetKidRole(nonCellKid, taggingHelper))) {
                    taggingHelper.MoveKidHint(nonCellKid, tableHintKey);
                }
            }
            taggingHelper.AddKidsHint(tableHintKey, JavaCollectionsUtil.SingletonList<TaggingHintKey>(LayoutTaggingHelper
                .GetOrCreateHintKey(tbodyTag)), -1);
            foreach (TaggingHintKey nonCellKid in nonCellKids) {
                if (StandardRoles.TFOOT.Equals(GetKidRole(nonCellKid, taggingHelper))) {
                    taggingHelper.MoveKidHint(nonCellKid, tableHintKey);
                }
            }
            foreach (SortedDictionary<int, TaggingHintKey> rowTags in tableTags.Values) {
                TaggingDummyElement row = new TaggingDummyElement(StandardRoles.TR);
                TaggingHintKey rowTagHint = LayoutTaggingHelper.GetOrCreateHintKey(row);
                foreach (TaggingHintKey cellTagHint in rowTags.Values) {
                    taggingHelper.MoveKidHint(cellTagHint, rowTagHint);
                }
                if (tableCellTagsUnindexed != null) {
                    foreach (TaggingHintKey cellTagHint in tableCellTagsUnindexed) {
                        taggingHelper.MoveKidHint(cellTagHint, rowTagHint);
                    }
                    tableCellTagsUnindexed = null;
                }
                taggingHelper.AddKidsHint(tbodyTag, JavaCollectionsUtil.SingletonList<TaggingDummyElement>(row), -1);
            }
            foreach (TaggingHintKey nonCellKid in nonCellKids) {
                if (StandardRoles.CAPTION.Equals(GetKidRole(nonCellKid, taggingHelper))) {
                    MoveCaption(taggingHelper, nonCellKid, tableHintKey);
                }
            }
            return true;
        }

        private static String GetKidRole(TaggingHintKey kidKey, LayoutTaggingHelper helper) {
            return helper.GetPdfDocument().GetTagStructureContext().ResolveMappingToStandardOrDomainSpecificRole(kidKey
                .GetAccessibilityProperties().GetRole(), null).GetRole();
        }

        /// <summary>
        /// Creates a dummy element with
        /// <see cref="iText.Kernel.Pdf.Tagging.StandardRoles.TBODY"/>
        /// role if needed.
        /// </summary>
        /// <remarks>
        /// Creates a dummy element with
        /// <see cref="iText.Kernel.Pdf.Tagging.StandardRoles.TBODY"/>
        /// role if needed.
        /// Otherwise, returns a dummy element with a null role.
        /// </remarks>
        /// <param name="tableHintKey">the hint key of the table.</param>
        /// <returns>
        /// a dummy element with
        /// <see cref="iText.Kernel.Pdf.Tagging.StandardRoles.TBODY"/>
        /// role if needed.
        /// </returns>
        private static TaggingDummyElement GetTbodyTag(TaggingHintKey tableHintKey) {
            bool createTBody = true;
            if (tableHintKey.GetAccessibleElement() is Table) {
                Table modelElement = (Table)tableHintKey.GetAccessibleElement();
                createTBody = modelElement.GetHeader() != null && !modelElement.IsSkipFirstHeader() || modelElement.GetFooter
                    () != null && !modelElement.IsSkipLastFooter();
            }
            return new TaggingDummyElement(createTBody ? StandardRoles.TBODY : null);
        }

        private static void MoveCaption(LayoutTaggingHelper taggingHelper, TaggingHintKey caption, TaggingHintKey 
            tableHintKey) {
            if (!(tableHintKey.GetAccessibleElement() is Table)) {
                return;
            }
            Table tableElem = (Table)tableHintKey.GetAccessibleElement();
            Div captionDiv = tableElem.GetCaption();
            if (captionDiv == null) {
                return;
            }
            CaptionSide captionSide;
            if (captionDiv.GetProperty<CaptionSide?>(Property.CAPTION_SIDE) == null) {
                captionSide = CaptionSide.TOP;
            }
            else {
                captionSide = (CaptionSide)captionDiv.GetProperty<CaptionSide?>(Property.CAPTION_SIDE);
            }
            if (CaptionSide.TOP.Equals(captionSide)) {
                taggingHelper.MoveKidHint(caption, tableHintKey, 0);
            }
            else {
                taggingHelper.MoveKidHint(caption, tableHintKey);
            }
        }
    }
//\endcond
}
