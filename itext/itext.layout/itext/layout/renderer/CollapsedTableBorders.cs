/*
This file is part of the iText (R) project.
Copyright (c) 1998-2025 Apryse Group NV
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
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Canvas;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;

namespace iText.Layout.Renderer {
//\cond DO_NOT_DOCUMENT
    internal class CollapsedTableBorders : TableBorders {
        /// <summary>
        /// Horizontal borders to be collapsed with
        /// the first-on-the-area row's cell top borders of this TableRenderer instance.
        /// </summary>
        private IList<Border> topBorderCollapseWith = new List<Border>();

        /// <summary>
        /// Horizontal borders to be collapsed with
        /// the last-on-the-area row's cell bottom borders of this TableRenderer instance.
        /// </summary>
        private IList<Border> bottomBorderCollapseWith = new List<Border>();

        // NOTE: Currently body's top border is written at header level and footer's top border is written
        //  at body's level, hence there is no need in the same array for vertical top borders.
        /// <summary>
        /// Vertical borders to be collapsed with
        /// the last-on-the-area row's cell bottom borders of this TableRenderer instance.
        /// </summary>
        private IList<Border> verticalBottomBorderCollapseWith = null;

        private static IComparer<Border> borderComparator = new CollapsedTableBorders.BorderComparator();

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

        /// <summary>Gets vertical borders which cross the top horizontal border.</summary>
        /// <returns>vertical borders which cross the top horizontal border</returns>
        public virtual IList<Border> GetVerticalBordersCrossingTopHorizontalBorder() {
            IList<Border> borders = new List<Border>(numberOfColumns + 1);
            for (int i = 0; i <= numberOfColumns; i++) {
                IList<Border> verticalBorder = GetVerticalBorder(i);
                // the passed index indicates the index of the border on the page, not in the entire document
                Border borderToAdd = startRow - largeTableIndexOffset < verticalBorder.Count ? verticalBorder[startRow - largeTableIndexOffset
                    ] : null;
                borders.Add(borderToAdd);
            }
            return borders;
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
                        [verticalBorders.Count - 1].Count);
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
            , IList<Border> verticalBordersCrossingBottomBorder) {
            this.bottomBorderCollapseWith = new List<Border>();
            if (null != bottomBorderCollapseWith) {
                this.bottomBorderCollapseWith.AddAll(bottomBorderCollapseWith);
            }
            this.verticalBottomBorderCollapseWith = null;
            if (null != verticalBordersCrossingBottomBorder) {
                this.verticalBottomBorderCollapseWith = new List<Border>(verticalBordersCrossingBottomBorder);
            }
            return this;
        }

        //endregion
        protected internal override void BuildBordersArrays(CellRenderer cell, int row, int col) {
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
                     + 1 != row));
                // process only valid cells which hasn't been processed yet
                if (j >= 0 && nextCellRow != rows.Count && nextCellRow > row) {
                    CellRenderer nextCell = rows[nextCellRow][j];
                    BuildBordersArrays(nextCell, nextCellRow);
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
                    BuildBordersArrays(nextCell, nextCellRow);
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
                    BuildBordersArrays(nextCell, nextCellRow);
                }
            }
            // consider current cell
            BuildBordersArrays(cell, row);
        }

        protected internal virtual void BuildBordersArrays(CellRenderer cell, int row) {
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
            IList<Border> borders = borderArray[i];
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
        protected internal override TableBorders DrawHorizontalBorder(PdfCanvas canvas, TableBorderDescriptor borderDescriptor
            ) {
            int i = borderDescriptor.GetBorderIndex();
            float startX = borderDescriptor.GetMainCoordinateStart();
            float y1 = borderDescriptor.GetCrossCoordinate();
            float[] countedColumnWidth = borderDescriptor.GetMainCoordinateWidths();
            IList<Border> horizontalBorder = GetHorizontalBorder(startRow + i);
            float x1 = startX;
            float x2 = x1 + countedColumnWidth[0];
            for (int j = 1; j <= horizontalBorder.Count; j++) {
                Border currentBorder = horizontalBorder[j - 1];
                Border nextBorder = j < horizontalBorder.Count ? horizontalBorder[j] : null;
                if (currentBorder != null) {
                    IList<Border> crossingBordersAtStart = GetCrossingBorders(i, j - 1);
                    float startCornerWidth = GetWidestBorderWidth(crossingBordersAtStart[1], crossingBordersAtStart[3]);
                    IList<Border> crossingBordersAtEnd = GetCrossingBorders(i, j);
                    float endCornerWidth = GetWidestBorderWidth(crossingBordersAtEnd[1], crossingBordersAtEnd[3]);
                    // TODO DEVSIX-5962 Once the ticket is done, remove this workaround, which allows
                    //  horizontal borders to win at vertical-0. Bear in mind that this workaround helps
                    //  in standard cases, when borders are of the same width. If they are not then
                    //  this workaround doesn't help to improve corner collapsing
                    if (1 == j) {
                        crossingBordersAtStart.Add(0, currentBorder);
                    }
                    if (0 == i) {
                        if (1 != j) {
                            crossingBordersAtStart.Add(0, crossingBordersAtStart[3]);
                        }
                        crossingBordersAtEnd.Add(0, crossingBordersAtEnd[3]);
                    }
                    JavaCollectionsUtil.Sort(crossingBordersAtStart, borderComparator);
                    JavaCollectionsUtil.Sort(crossingBordersAtEnd, borderComparator);
                    float x1Offset = currentBorder.Equals(crossingBordersAtStart[0]) ? -startCornerWidth / 2 : startCornerWidth
                         / 2;
                    float x2Offset = currentBorder.Equals(crossingBordersAtEnd[0]) ? endCornerWidth / 2 : -endCornerWidth / 2;
                    currentBorder.DrawCellBorder(canvas, x1 + x1Offset, y1, x2 + x2Offset, y1, Border.Side.NONE);
                    x1 = x2;
                }
                else {
                    // if current border is null, then just skip it's processing.
                    // Border corners will be processed by borders which are not null.
                    x1 += countedColumnWidth[j - 1];
                    x2 = x1;
                }
                if (nextBorder != null && j != horizontalBorder.Count) {
                    x2 += countedColumnWidth[j];
                }
            }
            return this;
        }

        protected internal override TableBorders DrawVerticalBorder(PdfCanvas canvas, TableBorderDescriptor borderDescriptor
            ) {
            int i = borderDescriptor.GetBorderIndex();
            float startY = borderDescriptor.GetMainCoordinateStart();
            float x1 = borderDescriptor.GetCrossCoordinate();
            float[] heights = borderDescriptor.GetMainCoordinateWidths();
            IList<Border> borders = GetVerticalBorder(i);
            float y1 = startY;
            float y2 = y1;
            if (0 != heights.Length) {
                y2 = y1 - heights[0];
            }
            float? y1Offset = null;
            for (int j = 1; j <= heights.Length; j++) {
                Border currentBorder = borders[startRow - largeTableIndexOffset + j - 1];
                Border nextBorder = j < heights.Length ? borders[startRow - largeTableIndexOffset + j] : null;
                if (currentBorder != null) {
                    IList<Border> crossingBordersAtStart = GetCrossingBorders(j - 1, i);
                    float startCornerWidth = GetWidestBorderWidth(crossingBordersAtStart[0], crossingBordersAtStart[2]);
                    // TODO DEVSIX-5962 Once the ticket is done, remove this workaround, which allows
                    //  vertical borders to win at horizontal-0. Bear in mind that this workaround helps
                    //  in standard cases, when borders are of the same width. If they are not then
                    //  this workaround doesn't help to improve corner collapsing
                    if (1 == j) {
                        crossingBordersAtStart.Add(0, currentBorder);
                    }
                    JavaCollectionsUtil.Sort(crossingBordersAtStart, borderComparator);
                    IList<Border> crossingBordersAtEnd = GetCrossingBorders(j, i);
                    float endCornerWidth = GetWidestBorderWidth(crossingBordersAtEnd[0], crossingBordersAtEnd[2]);
                    JavaCollectionsUtil.Sort(crossingBordersAtEnd, borderComparator);
                    // if all the borders are equal, we need to draw them at the end
                    if (!currentBorder.Equals(nextBorder)) {
                        if (null == y1Offset) {
                            y1Offset = currentBorder.Equals(crossingBordersAtStart[0]) ? startCornerWidth / 2 : -startCornerWidth / 2;
                        }
                        float y2Offset = currentBorder.Equals(crossingBordersAtEnd[0]) ? -endCornerWidth / 2 : endCornerWidth / 2;
                        currentBorder.DrawCellBorder(canvas, x1, y1 + (float)y1Offset, x1, y2 + y2Offset, Border.Side.NONE);
                        y1 = y2;
                        y1Offset = null;
                    }
                    else {
                        // if current border equal the next one, we apply an optimization here, which allows us
                        // to draw equal borders at once and not by part. Therefore for the first of such borders
                        // we store its start offset
                        if (null == y1Offset) {
                            y1Offset = currentBorder.Equals(crossingBordersAtStart[0]) ? startCornerWidth / 2 : -startCornerWidth / 2;
                        }
                    }
                }
                else {
                    // if current border is null, then just skip it's processing.
                    // Border corners will be processed by borders which are not null.
                    y1 -= heights[j - 1];
                    y2 = y1;
                }
                if (nextBorder != null) {
                    y2 -= heights[j];
                }
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
            IList<Border> collapsedList = new List<Border>(size);
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
                    // in case of large table and no content (Table#complete is called right after Table#flush)
                    SetTopBorderCollapseWith(((Table)currentRenderer.GetModelElement()).GetLastRowBottomBorder());
                }
                else {
                    SetTopBorderCollapseWith(null);
                    SetBottomBorderCollapseWith(null, null);
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
            SetBottomBorderCollapseWith(null, null);
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
            SetBottomBorderCollapseWith(footerBordersHandler.GetHorizontalBorder(0), ((iText.Layout.Renderer.CollapsedTableBorders
                )footerBordersHandler).GetVerticalBordersCrossingTopHorizontalBorder());
            return this;
        }

        protected internal override TableBorders CollapseTableWithHeader(TableBorders headerBordersHandler, bool updateBordersHandler
            ) {
            ((iText.Layout.Renderer.CollapsedTableBorders)headerBordersHandler).SetBottomBorderCollapseWith(GetHorizontalBorder
                (startRow), GetVerticalBordersCrossingTopHorizontalBorder());
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

//\cond DO_NOT_DOCUMENT
        // endregion
        /// <summary>
        /// Returns the
        /// <see cref="iText.Layout.Borders.Border"/>
        /// instances, which intersect in the specified point.
        /// </summary>
        /// <remarks>
        /// Returns the
        /// <see cref="iText.Layout.Borders.Border"/>
        /// instances, which intersect in the specified point.
        /// <para />
        /// The order of the borders: first the left one, then the top, the right and the bottom ones.
        /// </remarks>
        /// <param name="horizontalIndex">index of horizontal border</param>
        /// <param name="verticalIndex">index of vertical border</param>
        /// <returns>
        /// a list of
        /// <see cref="iText.Layout.Borders.Border"/>
        /// instances, which intersect in the specified point
        /// </returns>
        internal virtual IList<Border> GetCrossingBorders(int horizontalIndex, int verticalIndex) {
            IList<Border> horizontalBorder = GetHorizontalBorder(startRow + horizontalIndex);
            IList<Border> verticalBorder = GetVerticalBorder(verticalIndex);
            IList<Border> crossingBorders = new List<Border>(4);
            crossingBorders.Add(verticalIndex > 0 ? horizontalBorder[verticalIndex - 1] : null);
            crossingBorders.Add(horizontalIndex > 0 ? verticalBorder[startRow - largeTableIndexOffset + horizontalIndex
                 - 1] : null);
            crossingBorders.Add(verticalIndex < numberOfColumns ? horizontalBorder[verticalIndex] : null);
            crossingBorders.Add(horizontalIndex <= finishRow - startRow ? verticalBorder[startRow - largeTableIndexOffset
                 + horizontalIndex] : null);
            // In case the last horizontal border on the page is specified,
            // we need to consider a vertical border of the table's bottom part
            // (f.e., for header it is table's body).
            if (horizontalIndex == finishRow - startRow + 1 && null != verticalBottomBorderCollapseWith) {
                if (IsBorderWider(verticalBottomBorderCollapseWith[verticalIndex], crossingBorders[3])) {
                    crossingBorders[3] = verticalBottomBorderCollapseWith[verticalIndex];
                }
            }
            return crossingBorders;
        }
//\endcond

        /// <summary>
        /// A comparison function to compare two
        /// <see cref="iText.Layout.Borders.Border"/>
        /// instances.
        /// </summary>
        private class BorderComparator : IComparer<Border> {
            public virtual int Compare(Border o1, Border o2) {
                if (o1 == o2) {
                    return 0;
                }
                else {
                    if (null == o1) {
                        return 1;
                    }
                    else {
                        if (null == o2) {
                            return -1;
                        }
                        else {
                            return JavaUtil.FloatCompare(o2.GetWidth(), o1.GetWidth());
                        }
                    }
                }
            }
        }

        /// <summary>Gets the width of the widest border in the specified list.</summary>
        /// <param name="borders">the borders which widths should be considered</param>
        /// <returns>the width of the widest border in the specified list</returns>
        private float GetWidestBorderWidth(params Border[] borders) {
            float maxWidth = 0;
            foreach (Border border in borders) {
                if (null != border && maxWidth < border.GetWidth()) {
                    maxWidth = border.GetWidth();
                }
            }
            return maxWidth;
        }

        /// <summary>Compares borders and defines whether this border is wider than the other.</summary>
        /// <remarks>
        /// Compares borders and defines whether this border is wider than the other.
        /// <para />
        /// Note that by default the comparison will be strict, e.g. if this border
        /// is of the same width as the other border, then false will be returned.
        /// </remarks>
        /// <param name="thisBorder">this border</param>
        /// <param name="otherBorder">the other border to be compared with</param>
        /// <returns>whether this border is wider than the other</returns>
        private static bool IsBorderWider(Border thisBorder, Border otherBorder) {
            return IsBorderWider(thisBorder, otherBorder, true);
        }

        /// <summary>Compares borders and defines whether this border is wider than the other.</summary>
        /// <param name="thisBorder">this border</param>
        /// <param name="otherBorder">the other border to be compared with</param>
        /// <param name="strict">
        /// if true, then in case this border is of the same width as the other border,
        /// true will be returned. If false, it will be checked whether the width
        /// of this border is strictly greater than the other border's width
        /// </param>
        /// <returns>whether this border is wider than the other</returns>
        private static bool IsBorderWider(Border thisBorder, Border otherBorder, bool strict) {
            if (null == thisBorder) {
                return false;
            }
            if (null == otherBorder) {
                return true;
            }
            int comparisonResult = JavaUtil.FloatCompare(thisBorder.GetWidth(), otherBorder.GetWidth());
            return strict ? comparisonResult > 0 : comparisonResult >= 0;
        }
    }
//\endcond
}
