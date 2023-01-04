/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: iText Software.

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License version 3
as published by the Free Software Foundation with the addition of the
following permission added to Section 15 as permitted in Section 7(a):
FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
OF THIRD PARTY RIGHTS

This program is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU Affero General Public License for more details.
You should have received a copy of the GNU Affero General Public License
along with this program; if not, see http://www.gnu.org/licenses or write to
the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
Boston, MA, 02110-1301 USA, or download the license from the following URL:
http://itextpdf.com/terms-of-use/

The interactive user interfaces in modified source and object code versions
of this program must display Appropriate Legal Notices, as required under
Section 5 of the GNU Affero General Public License.

In accordance with Section 7(b) of the GNU Affero General Public License,
a covered work must retain the producer line in every PDF that is created
or manipulated using iText.

You can be released from the requirements of the license by purchasing
a commercial license. Buying such a license is mandatory as soon as you
develop commercial activities involving the iText software without
disclosing the source code of your own applications.
These activities include: offering paid services to customers as an ASP,
serving PDFs on the fly in a web application, shipping iText with a closed
source product.

For more information, please contact iText Software Corp. at this
address: sales@itextpdf.com
*/
using System;
using System.Collections.Generic;
using iText.Commons.Utils;
using iText.Kernel.Pdf.Tagging;
using iText.Layout.Element;

namespace iText.Layout.Tagging {
    internal class TableTaggingRule : ITaggingRule {
        public virtual bool OnTagFinish(LayoutTaggingHelper taggingHelper, TaggingHintKey tableHintKey) {
            IList<TaggingHintKey> kidKeys = taggingHelper.GetAccessibleKidsHint(tableHintKey);
            IDictionary<int, SortedDictionary<int, TaggingHintKey>> tableTags = new SortedDictionary<int, SortedDictionary
                <int, TaggingHintKey>>();
            IList<TaggingHintKey> tableCellTagsUnindexed = new List<TaggingHintKey>();
            IList<TaggingHintKey> nonCellKids = new List<TaggingHintKey>();
            foreach (TaggingHintKey kidKey in kidKeys) {
                if (StandardRoles.TD.Equals(kidKey.GetAccessibleElement().GetAccessibilityProperties().GetRole()) || StandardRoles
                    .TH.Equals(kidKey.GetAccessibleElement().GetAccessibilityProperties().GetRole())) {
                    if (kidKey.GetAccessibleElement() is Cell) {
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
                        tableCellTagsUnindexed.Add(kidKey);
                    }
                }
                else {
                    nonCellKids.Add(kidKey);
                }
            }
            bool createTBody = true;
            if (tableHintKey.GetAccessibleElement() is Table) {
                Table modelElement = (Table)tableHintKey.GetAccessibleElement();
                createTBody = modelElement.GetHeader() != null && !modelElement.IsSkipFirstHeader() || modelElement.GetFooter
                    () != null && !modelElement.IsSkipLastFooter();
            }
            TaggingDummyElement tbodyTag = null;
            tbodyTag = new TaggingDummyElement(createTBody ? StandardRoles.TBODY : null);
            foreach (TaggingHintKey nonCellKid in nonCellKids) {
                String kidRole = nonCellKid.GetAccessibleElement().GetAccessibilityProperties().GetRole();
                if (!StandardRoles.THEAD.Equals(kidRole) && !StandardRoles.TFOOT.Equals(kidRole)) {
                    taggingHelper.MoveKidHint(nonCellKid, tableHintKey);
                }
            }
            foreach (TaggingHintKey nonCellKid in nonCellKids) {
                String kidRole = nonCellKid.GetAccessibleElement().GetAccessibilityProperties().GetRole();
                if (StandardRoles.THEAD.Equals(kidRole)) {
                    taggingHelper.MoveKidHint(nonCellKid, tableHintKey);
                }
            }
            taggingHelper.AddKidsHint(tableHintKey, JavaCollectionsUtil.SingletonList<TaggingHintKey>(LayoutTaggingHelper
                .GetOrCreateHintKey(tbodyTag)), -1);
            foreach (TaggingHintKey nonCellKid in nonCellKids) {
                String kidRole = nonCellKid.GetAccessibleElement().GetAccessibilityProperties().GetRole();
                if (StandardRoles.TFOOT.Equals(kidRole)) {
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
            return true;
        }
    }
}
