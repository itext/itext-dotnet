/*
This file is part of the iText (R) project.
Copyright (c) 1998-2020 iText Group NV
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
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Canvas;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;

namespace iText.Layout.Renderer {
    internal class CollapsedTableBorders : TableBorders {
        /// <summary>
        /// The list of the cells' borders which should be collapsed
        /// with the first border of this TableRenderer instance, to be drawn on the area.
        /// </summary>
        private IList<Border> topBorderCollapseWith = new List<Border>();

        /// <summary>
        /// The list of the cells' borders which should be collapsed
        /// with the last border of this TableRenderer instance, to be drawn on the area.
        /// </summary>
        private IList<Border> bottomBorderCollapseWith = new List<Border>();

        // region constructors
        public CollapsedTableBorders(IList<CellRenderer[]> rows, int numberOfColumns, Border[] tableBoundingBorders
            )
            : base(rows, numberOfColumns, tableBoundingBorders) {
        }

        public CollapsedTableBorders(IList<CellRenderer[]> rows, int numberOfColumns, Border[] tableBoundingBorders
            , int largeTableIndexOffset)
            : base(rows, numberOfColumns, tableBoundingBorders, largeTableIndexOffset) {
        }

        // endregion
        // region getters
        public virtual IList<Border> GetTopBorderCollapseWith() {
            return topBorderCollapseWith;
        }

        public virtual IList<Border> GetBottomBorderCollapseWith() {
            return bottomBorderCollapseWith;
        }

        public override float[] GetCellBorderIndents(int row, int col, int rowspan, int colspan) {
            float[] indents = new float[4];
            IList<Border> borderList;
            Border border;
            // process top border
            borderList = GetHorizontalBorder(startRow + row - rowspan + 1);
            for (int i = col; i < col + colspan; i++) {
                border = borderList[i];
                if (null != border && border.GetWidth() > indents[0]) {
                    indents[0] = border.GetWidth();
                }
            }
            // process right border
            borderList = GetVerticalBorder(col + colspan);
            for (int i = startRow - largeTableIndexOffset + row - rowspan + 1; i < startRow - largeTableIndexOffset + 
                row + 1; i++) {
                border = borderList[i];
                if (null != border && border.GetWidth() > indents[1]) {
                    indents[1] = border.GetWidth();
                }
            }
            // process bottom border
            borderList = GetHorizontalBorder(startRow + row + 1);
            for (int i = col; i < col + colspan; i++) {
                border = borderList[i];
                if (null != border && border.GetWidth() > indents[2]) {
                    indents[2] = border.GetWidth();
                }
            }
            // process left border
            borderList = GetVerticalBorder(col);
            for (int i = startRow - largeTableIndexOffset + row - rowspan + 1; i < startRow - largeTableIndexOffset + 
                row + 1; i++) {
                border = borderList[i];
                if (null != border && border.GetWidth() > indents[3]) {
                    indents[3] = border.GetWidth();
                }
            }
            return indents;
        }

        public override IList<Border> GetVerticalBorder(int index) {
            if (index == 0) {
                IList<Border> borderList = TableBorderUtil.CreateAndFillBorderList(null, tableBoundingBorders[3], verticalBorders
                    [0].Count);
                return GetCollapsedList(verticalBorders[0], borderList);
            }
            else {
                if (index == numberOfColumns) {
                    IList<Border> borderList = TableBorderUtil.CreateAndFillBorderList(null, tableBoundingBorders[1], verticalBorders
                        [0].Count);
                    return GetCollapsedList(verticalBorders[verticalBorders.Count - 1], borderList);
                }
                else {
                    return verticalBorders[index];
                }
            }
        }

        public override IList<Border> GetHorizontalBorder(int index) {
            if (index == startRow) {
                IList<Border> firstBorderOnCurrentPage = TableBorderUtil.CreateAndFillBorderList(topBorderCollapseWith, tableBoundingBorders
                    [0], numberOfColumns);
                if (index == largeTableIndexOffset) {
                    return GetCollapsedList(horizontalBorders[index - largeTableIndexOffset], firstBorderOnCurrentPage);
                }
                if (0 != rows.Count) {
                    int col = 0;
                    int row = index;
                    while (col < numberOfColumns) {
                        if (null != rows[row - largeTableIndexOffset][col] && row - index + 1 <= (int)((Cell)rows[row - largeTableIndexOffset
                            ][col].GetModelElement()).GetRowspan()) {
                            CellRenderer cell = rows[row - largeTableIndexOffset][col];
                            Border cellModelTopBorder = TableBorderUtil.GetCellSideBorder(((Cell)cell.GetModelElement()), Property.BORDER_TOP
                                );
                            int colspan = (int)cell.GetPropertyAsInteger(Property.COLSPAN);
                            if (null == firstBorderOnCurrentPage[col] || (null != cellModelTopBorder && cellModelTopBorder.GetWidth() 
                                > firstBorderOnCurrentPage[col].GetWidth())) {
                                for (int i = col; i < col + colspan; i++) {
                                    firstBorderOnCurrentPage[i] = cellModelTopBorder;
                                }
                            }
                            col += colspan;
                            row = index;
                        }
                        else {
                            row++;
                            if (row == rows.Count) {
                                break;
                            }
                        }
                    }
                }
                return firstBorderOnCurrentPage;
            }
            else {
                if ((index == finishRow + 1)) {
                    IList<Border> lastBorderOnCurrentPage = TableBorderUtil.CreateAndFillBorderList(bottomBorderCollapseWith, 
                        tableBoundingBorders[2], numberOfColumns);
                    if (index - largeTableIndexOffset == horizontalBorders.Count - 1) {
                        return GetCollapsedList(horizontalBorders[index - largeTableIndexOffset], lastBorderOnCurrentPage);
                    }
                    if (0 != rows.Count) {
                        int col = 0;
                        int row = index - 1;
                        while (col < numberOfColumns) {
                            if (null != rows[row - largeTableIndexOffset][col]) {
                                CellRenderer cell = rows[row - largeTableIndexOffset][col];
                                Border cellModelBottomBorder = TableBorderUtil.GetCellSideBorder(((Cell)cell.GetModelElement()), Property.
                                    BORDER_BOTTOM);
                                int colspan = (int)cell.GetPropertyAsInteger(Property.COLSPAN);
                                if (null == lastBorderOnCurrentPage[col] || (null != cellModelBottomBorder && cellModelBottomBorder.GetWidth
                                    () > lastBorderOnCurrentPage[col].GetWidth())) {
                                    for (int i = col; i < col + colspan; i++) {
                                        lastBorderOnCurrentPage[i] = cellModelBottomBorder;
                                    }
                                }
                                col += colspan;
                                row = index - 1;
                            }
                            else {
                                row++;
                                if (row == rows.Count) {
                                    break;
                                }
                            }
                        }
                    }
                    return lastBorderOnCurrentPage;
                }
                else {
                    return horizontalBorders[index - largeTableIndexOffset];
                }
            }
        }

        // endregion
        // region setters
        public virtual iText.Layout.Renderer.CollapsedTableBorders SetTopBorderCollapseWith(IList<Border> topBorderCollapseWith
            ) {
            this.topBorderCollapseWith = new List<Border>();
            if (null != topBorderCollapseWith) {
                this.topBorderCollapseWith.AddAll(topBorderCollapseWith);
            }
            return this;
        }

        public virtual iText.Layout.Renderer.CollapsedTableBorders SetBottomBorderCollapseWith(IList<Border> bottomBorderCollapseWith
            ) {
            this.bottomBorderCollapseWith = new List<Border>();
            if (null != bottomBorderCollapseWith) {
                this.bottomBorderCollapseWith.AddAll(bottomBorderCollapseWith);
            }
            return this;
        }

        //endregion
        protected internal override void BuildBordersArrays(CellRenderer cell, int row, int col, int[] rowspansToDeduct
            ) {
            // We should check if the row number is less than horizontal borders array size. It can happen if the cell with
            // big rowspan doesn't fit current area and is going to be placed partial.
            if (row > horizontalBorders.Count) {
                row--;
            }
            int currCellColspan = (int)cell.GetPropertyAsInteger(Property.COLSPAN);
            int nextCellRow;
            int j;
            // consider the cell on the left side of the current one
            if (col != 0 && null == rows[row][col - 1]) {
                j = col;
                do {
                    j--;
                    nextCellRow = row;
                    while (rows.Count != nextCellRow && null == rows[nextCellRow][j]) {
                        nextCellRow++;
                    }
                }
                while (j > 0 && rows.Count != nextCellRow && (j + (int)rows[nextCellRow][j].GetPropertyAsInteger(Property.
                    COLSPAN) != col || (int)nextCellRow - rows[(int)nextCellRow][j].GetPropertyAsInteger(Property.ROWSPAN)
                     + 1 + rowspansToDeduct[j] != row));
                // process only valid cells which hasn't been processed yet
                if (j >= 0 && nextCellRow != rows.Count && nextCellRow > row) {
                    CellRenderer nextCell = rows[nextCellRow][j];
                    nextCell.SetProperty(Property.ROWSPAN, ((int)nextCell.GetPropertyAsInteger(Property.ROWSPAN)) - rowspansToDeduct
                        [j]);
                    int nextCellColspan = (int)nextCell.GetPropertyAsInteger(Property.COLSPAN);
                    for (int i = j; i < j + nextCellColspan; i++) {
                        rowspansToDeduct[i] = 0;
                    }
                    BuildBordersArrays(nextCell, nextCellRow, true);
                }
            }
            // consider cells under the current one
            j = 0;
            while (j < currCellColspan) {
                nextCellRow = row + 1;
                while (nextCellRow < rows.Count && null == rows[nextCellRow][col + j]) {
                    nextCellRow++;
                }
                if (nextCellRow == rows.Count) {
                    break;
                }
                CellRenderer nextCell = rows[nextCellRow][col + j];
                // otherwise the border was considered previously
                if (row == nextCellRow - (int)nextCell.GetPropertyAsInteger(Property.ROWSPAN)) {
                    BuildBordersArrays(nextCell, nextCellRow, true);
                }
                j += (int)nextCell.GetPropertyAsInteger(Property.COLSPAN);
            }
            // consider cells on the right side of the current one
            if (col + currCellColspan < rows[row].Length) {
                nextCellRow = row;
                while (nextCellRow < rows.Count && null == rows[nextCellRow][col + currCellColspan]) {
                    nextCellRow++;
                }
                if (nextCellRow != rows.Count) {
                    CellRenderer nextCell = rows[nextCellRow][col + currCellColspan];
                    nextCell.SetProperty(Property.ROWSPAN, ((int)nextCell.GetPropertyAsInteger(Property.ROWSPAN)) - rowspansToDeduct
                        [col + currCellColspan]);
                    int nextCellColspan = (int)nextCell.GetPropertyAsInteger(Property.COLSPAN);
                    for (int i = col + currCellColspan; i < col + currCellColspan + nextCellColspan; i++) {
                        rowspansToDeduct[i] = 0;
                    }
                    BuildBordersArrays(nextCell, nextCellRow, true);
                }
            }
            // consider current cell
            BuildBordersArrays(cell, row, false);
        }

        protected internal virtual void BuildBordersArrays(CellRenderer cell, int row, bool isNeighbourCell) {
            int colspan = (int)cell.GetPropertyAsInteger(Property.COLSPAN);
            int rowspan = (int)cell.GetPropertyAsInteger(Property.ROWSPAN);
            int colN = ((Cell)cell.GetModelElement()).GetCol();
            Border[] cellBorders = cell.GetBorders();
            // cell with big rowspan was splitted
            if (row + 1 - rowspan < 0) {
                rowspan = row + 1;
            }
            // consider top border
            for (int i = 0; i < colspan; i++) {
                CheckAndReplaceBorderInArray(horizontalBorders, row + 1 - rowspan, colN + i, cellBorders[0], false);
            }
            // consider bottom border
            for (int i = 0; i < colspan; i++) {
                CheckAndReplaceBorderInArray(horizontalBorders, row + 1, colN + i, cellBorders[2], true);
            }
            // consider left border
            for (int j = row - rowspan + 1; j <= row; j++) {
                CheckAndReplaceBorderInArray(verticalBorders, colN, j, cellBorders[3], false);
            }
            // consider right border
            for (int i = row - rowspan + 1; i <= row; i++) {
                CheckAndReplaceBorderInArray(verticalBorders, colN + colspan, i, cellBorders[1], true);
            }
        }

        // endregion
        // region lowlevel
        protected internal virtual bool CheckAndReplaceBorderInArray(IList<IList<Border>> borderArray, int i, int 
            j, Border borderToAdd, bool hasPriority) {
            //        if (borderArray.size() <= i) {
            //            for (int count = borderArray.size(); count <= i; count++) {
            //                borderArray.add(new ArrayList<Border>());
            //            }
            //        }
            IList<Border> borders = borderArray[i];
            //        if (borders.isEmpty()) {
            //            for (int count = 0; count < j; count++) {
            //                borders.add(null);
            //            }
            //            borders.add(borderToAdd);
            //            return true;
            //        }
            //        if (borders.size() == j) {
            //            borders.add(borderToAdd);
            //            return true;
            //        }
            //        if (borders.size() < j) {
            //            for (int count = borders.size(); count <= j; count++) {
            //                borders.add(count, null);
            //            }
            //        }
            Border neighbour = borders[j];
            if (neighbour == null) {
                borders[j] = borderToAdd;
                return true;
            }
            else {
                if (neighbour != borderToAdd) {
                    if (borderToAdd != null && neighbour.GetWidth() <= borderToAdd.GetWidth()) {
                        if (!hasPriority && neighbour.GetWidth() == borderToAdd.GetWidth()) {
                            return false;
                        }
                        borders[j] = borderToAdd;
                        return true;
                    }
                }
            }
            return false;
        }

        // endregion
        // region draw
        protected internal override TableBorders DrawHorizontalBorder(int i, float startX, float y1, PdfCanvas canvas
            , float[] countedColumnWidth) {
            IList<Border> borders = GetHorizontalBorder(startRow + /*- largeTableIndexOffset*/ i);
            float x1 = startX;
            float x2 = x1 + countedColumnWidth[0];
            if (i == 0) {
                Border firstBorder = GetFirstVerticalBorder()[startRow - largeTableIndexOffset];
                if (firstBorder != null) {
                    x1 -= firstBorder.GetWidth() / 2;
                }
            }
            else {
                if (i == finishRow - startRow + 1) {
                    Border firstBorder = GetFirstVerticalBorder()[startRow - largeTableIndexOffset + finishRow - startRow + 1 
                        - 1];
                    if (firstBorder != null) {
                        x1 -= firstBorder.GetWidth() / 2;
                    }
                }
            }
            int j;
            for (j = 1; j < borders.Count; j++) {
                Border prevBorder = borders[j - 1];
                Border curBorder = borders[j];
                if (prevBorder != null) {
                    if (!prevBorder.Equals(curBorder)) {
                        prevBorder.DrawCellBorder(canvas, x1, y1, x2, y1, Border.Side.NONE);
                        x1 = x2;
                    }
                }
                else {
                    x1 += countedColumnWidth[j - 1];
                    x2 = x1;
                }
                if (curBorder != null) {
                    x2 += countedColumnWidth[j];
                }
            }
            Border lastBorder = borders.Count > j - 1 ? borders[j - 1] : null;
            if (lastBorder != null) {
                if (i == 0) {
                    if (GetVerticalBorder(j)[startRow - largeTableIndexOffset + i] != null) {
                        x2 += GetVerticalBorder(j)[startRow - largeTableIndexOffset + i].GetWidth() / 2;
                    }
                }
                else {
                    if (i == finishRow - startRow + 1 && GetVerticalBorder(j).Count > startRow - largeTableIndexOffset + i - 1
                         && GetVerticalBorder(j)[startRow - largeTableIndexOffset + i - 1] != null) {
                        x2 += GetVerticalBorder(j)[startRow - largeTableIndexOffset + i - 1].GetWidth() / 2;
                    }
                }
                lastBorder.DrawCellBorder(canvas, x1, y1, x2, y1, Border.Side.NONE);
            }
            return this;
        }

        protected internal override TableBorders DrawVerticalBorder(int i, float startY, float x1, PdfCanvas canvas
            , IList<float> heights) {
            IList<Border> borders = GetVerticalBorder(i);
            float y1 = startY;
            float y2 = y1;
            if (!heights.IsEmpty()) {
                y2 = y1 - (float)heights[0];
            }
            int j;
            for (j = 1; j < heights.Count; j++) {
                Border prevBorder = borders[startRow - largeTableIndexOffset + j - 1];
                Border curBorder = borders[startRow - largeTableIndexOffset + j];
                if (prevBorder != null) {
                    if (!prevBorder.Equals(curBorder)) {
                        prevBorder.DrawCellBorder(canvas, x1, y1, x1, y2, Border.Side.NONE);
                        y1 = y2;
                    }
                }
                else {
                    y1 -= (float)heights[j - 1];
                    y2 = y1;
                }
                if (curBorder != null) {
                    y2 -= (float)heights[j];
                }
            }
            if (borders.Count == 0) {
                return this;
            }
            Border lastBorder = borders[startRow - largeTableIndexOffset + j - 1];
            if (lastBorder != null) {
                lastBorder.DrawCellBorder(canvas, x1, y1, x1, y2, Border.Side.NONE);
            }
            return this;
        }

        // endregion
        // region static
        /// <summary>Returns the collapsed border.</summary>
        /// <remarks>
        /// Returns the collapsed border. We process collapse
        /// if the table border width is strictly greater than cell border width.
        /// </remarks>
        /// <param name="cellBorder">cell border</param>
        /// <param name="tableBorder">table border</param>
        /// <returns>the collapsed border</returns>
        public static Border GetCollapsedBorder(Border cellBorder, Border tableBorder) {
            if (null != tableBorder) {
                if (null == cellBorder || cellBorder.GetWidth() < tableBorder.GetWidth()) {
                    return tableBorder;
                }
            }
            if (null != cellBorder) {
                return cellBorder;
            }
            else {
                return Border.NO_BORDER;
            }
        }

        public static IList<Border> GetCollapsedList(IList<Border> innerList, IList<Border> outerList) {
            int size = Math.Min(null == innerList ? 0 : innerList.Count, null == outerList ? 0 : outerList.Count);
            IList<Border> collapsedList = new List<Border>();
            for (int i = 0; i < size; i++) {
                collapsedList.Add(GetCollapsedBorder(innerList[i], outerList[i]));
            }
            return collapsedList;
        }

        // endregion
        // region occupation
        protected internal override TableBorders ApplyLeftAndRightTableBorder(Rectangle layoutBox, bool reverse) {
            if (null != layoutBox) {
                layoutBox.ApplyMargins(0, rightBorderMaxWidth / 2, 0, leftBorderMaxWidth / 2, reverse);
            }
            return this;
        }

        protected internal override TableBorders ApplyTopTableBorder(Rectangle occupiedBox, Rectangle layoutBox, bool
             isEmpty, bool force, bool reverse) {
            if (!isEmpty) {
                return ApplyTopTableBorder(occupiedBox, layoutBox, reverse);
            }
            else {
                if (force) {
                    // process empty table
                    ApplyTopTableBorder(occupiedBox, layoutBox, reverse);
                    return ApplyTopTableBorder(occupiedBox, layoutBox, reverse);
                }
            }
            return this;
        }

        protected internal override TableBorders ApplyBottomTableBorder(Rectangle occupiedBox, Rectangle layoutBox
            , bool isEmpty, bool force, bool reverse) {
            if (!isEmpty) {
                return ApplyBottomTableBorder(occupiedBox, layoutBox, reverse);
            }
            else {
                if (force) {
                    // process empty table
                    ApplyBottomTableBorder(occupiedBox, layoutBox, reverse);
                    return ApplyBottomTableBorder(occupiedBox, layoutBox, reverse);
                }
            }
            return this;
        }

        protected internal override TableBorders ApplyTopTableBorder(Rectangle occupiedBox, Rectangle layoutBox, bool
             reverse) {
            float topIndent = (reverse ? -1 : 1) * GetMaxTopWidth();
            layoutBox.DecreaseHeight(topIndent / 2);
            occupiedBox.MoveDown(topIndent / 2).IncreaseHeight(topIndent / 2);
            return this;
        }

        protected internal override TableBorders ApplyBottomTableBorder(Rectangle occupiedBox, Rectangle layoutBox
            , bool reverse) {
            float bottomTableBorderWidth = (reverse ? -1 : 1) * GetMaxBottomWidth();
            layoutBox.DecreaseHeight(bottomTableBorderWidth / 2);
            occupiedBox.MoveDown(bottomTableBorderWidth / 2).IncreaseHeight(bottomTableBorderWidth / 2);
            return this;
        }

        protected internal override TableBorders ApplyCellIndents(Rectangle box, float topIndent, float rightIndent
            , float bottomIndent, float leftIndent, bool reverse) {
            box.ApplyMargins(topIndent / 2, rightIndent / 2, bottomIndent / 2, leftIndent / 2, false);
            return this;
        }

        protected internal override float GetCellVerticalAddition(float[] indents) {
            return indents[0] / 2 + indents[2] / 2;
        }

        // endregion
        // region update, footer/header
        protected internal override TableBorders UpdateBordersOnNewPage(bool isOriginalNonSplitRenderer, bool isFooterOrHeader
            , TableRenderer currentRenderer, TableRenderer headerRenderer, TableRenderer footerRenderer) {
            if (!isFooterOrHeader) {
                // collapse all cell borders
                if (isOriginalNonSplitRenderer) {
                    if (null != rows) {
                        ProcessAllBordersAndEmptyRows();
                        rightBorderMaxWidth = GetMaxRightWidth();
                        leftBorderMaxWidth = GetMaxLeftWidth();
                    }
                    SetTopBorderCollapseWith(((Table)currentRenderer.GetModelElement()).GetLastRowBottomBorder());
                }
                else {
                    SetTopBorderCollapseWith(null);
                    SetBottomBorderCollapseWith(null);
                }
            }
            if (null != footerRenderer) {
                float rightFooterBorderWidth = footerRenderer.bordersHandler.GetMaxRightWidth();
                float leftFooterBorderWidth = footerRenderer.bordersHandler.GetMaxLeftWidth();
                leftBorderMaxWidth = Math.Max(leftBorderMaxWidth, leftFooterBorderWidth);
                rightBorderMaxWidth = Math.Max(rightBorderMaxWidth, rightFooterBorderWidth);
            }
            if (null != headerRenderer) {
                float rightHeaderBorderWidth = headerRenderer.bordersHandler.GetMaxRightWidth();
                float leftHeaderBorderWidth = headerRenderer.bordersHandler.GetMaxLeftWidth();
                leftBorderMaxWidth = Math.Max(leftBorderMaxWidth, leftHeaderBorderWidth);
                rightBorderMaxWidth = Math.Max(rightBorderMaxWidth, rightHeaderBorderWidth);
            }
            return this;
        }

        protected internal override TableBorders SkipFooter(Border[] borders) {
            SetTableBoundingBorders(borders);
            SetBottomBorderCollapseWith(null);
            return this;
        }

        protected internal override TableBorders SkipHeader(Border[] borders) {
            SetTableBoundingBorders(borders);
            SetTopBorderCollapseWith(null);
            return this;
        }

        protected internal override TableBorders CollapseTableWithFooter(TableBorders footerBordersHandler, bool hasContent
            ) {
            ((iText.Layout.Renderer.CollapsedTableBorders)footerBordersHandler).SetTopBorderCollapseWith(hasContent ? 
                GetLastHorizontalBorder() : GetTopBorderCollapseWith());
            SetBottomBorderCollapseWith(footerBordersHandler.GetHorizontalBorder(0));
            return this;
        }

        protected internal override TableBorders CollapseTableWithHeader(TableBorders headerBordersHandler, bool updateBordersHandler
            ) {
            ((iText.Layout.Renderer.CollapsedTableBorders)headerBordersHandler).SetBottomBorderCollapseWith(GetHorizontalBorder
                (startRow));
            if (updateBordersHandler) {
                SetTopBorderCollapseWith(headerBordersHandler.GetLastHorizontalBorder());
            }
            return this;
        }

        protected internal override TableBorders FixHeaderOccupiedArea(Rectangle occupiedBox, Rectangle layoutBox) {
            float topBorderMaxWidth = GetMaxTopWidth();
            layoutBox.IncreaseHeight(topBorderMaxWidth);
            occupiedBox.MoveUp(topBorderMaxWidth).DecreaseHeight(topBorderMaxWidth);
            return this;
        }
        // endregion
    }
}
