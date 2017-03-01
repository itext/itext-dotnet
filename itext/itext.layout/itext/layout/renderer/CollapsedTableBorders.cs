using System;
using System.Collections.Generic;
using iText.IO.Log;
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Canvas;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;

namespace iText.Layout.Renderer {
    public class CollapsedTableBorders : TableBorders {
        private IList<Border> topBorderCollapseWith;

        private IList<Border> bottomBorderCollapseWith;

        private int largeTableIndexOffset;

        public CollapsedTableBorders(IList<CellRenderer[]> rows, int numberOfColumns)
            : base(rows, numberOfColumns) {
            // region constructors
            largeTableIndexOffset = 0;
        }

        public CollapsedTableBorders(IList<CellRenderer[]> rows, int numberOfColumns, Border[] tableBoundingBorders
            )
            : base(rows, numberOfColumns, tableBoundingBorders) {
            topBorderCollapseWith = new List<Border>();
            bottomBorderCollapseWith = new List<Border>();
            largeTableIndexOffset = 0;
        }

        public CollapsedTableBorders(IList<CellRenderer[]> rows, int numberOfColumns, Border[] tableBoundingBorders
            , int startRow)
            : this(rows, numberOfColumns, tableBoundingBorders) {
            this.largeTableIndexOffset = startRow;
        }

        // endregion
        // region collapse
        protected internal virtual iText.Layout.Renderer.CollapsedTableBorders CollapseAllBordersAndEmptyRows() {
            CellRenderer[] currentRow;
            int[] rowsToDelete = new int[numberOfColumns];
            for (int row = rowRange.GetStartRow() - largeTableIndexOffset; row <= rowRange.GetFinishRow() - largeTableIndexOffset
                ; row++) {
                currentRow = rows[row];
                bool hasCells = false;
                for (int col = 0; col < numberOfColumns; col++) {
                    if (null != currentRow[col]) {
                        int colspan = (int)currentRow[col].GetPropertyAsInteger(Property.COLSPAN);
                        PrepareBuildingBordersArrays(currentRow[col], tableBoundingBorders, numberOfColumns, row, col);
                        BuildBordersArrays(currentRow[col], row, col);
                        hasCells = true;
                        if (rowsToDelete[col] > 0) {
                            int rowspan = (int)currentRow[col].GetPropertyAsInteger(Property.ROWSPAN) - rowsToDelete[col];
                            if (rowspan < 1) {
                                ILogger logger = LoggerFactory.GetLogger(typeof(TableRenderer));
                                logger.Warn(iText.IO.LogMessageConstant.UNEXPECTED_BEHAVIOUR_DURING_TABLE_ROW_COLLAPSING);
                                rowspan = 1;
                            }
                            currentRow[col].SetProperty(Property.ROWSPAN, rowspan);
                        }
                        for (int i = 0; i < colspan; i++) {
                            rowsToDelete[col + i] = 0;
                        }
                        col += colspan - 1;
                    }
                    else {
                        if (horizontalBorders[row].Count <= col) {
                            horizontalBorders[row].Add(null);
                        }
                    }
                }
                if (!hasCells) {
                    SetFinishRow(rowRange.GetFinishRow() - 1);
                    rows.Remove(currentRow);
                    row--;
                    for (int i = 0; i < numberOfColumns; i++) {
                        rowsToDelete[i]++;
                    }
                    if (row == rows.Count - 1) {
                        ILogger logger = LoggerFactory.GetLogger(typeof(TableRenderer));
                        logger.Warn(iText.IO.LogMessageConstant.LAST_ROW_IS_NOT_COMPLETE);
                    }
                }
            }
            if (rowRange.GetFinishRow() < rowRange.GetStartRow()) {
                SetFinishRow(rowRange.GetStartRow());
            }
            return this;
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
            borderList = GetHorizontalBorder(rowRange.GetStartRow() + row - rowspan + 1);
            for (int i = col; i < col + colspan; i++) {
                border = borderList[i];
                if (null != border && border.GetWidth() > indents[0]) {
                    indents[0] = border.GetWidth();
                }
            }
            // process right border
            borderList = GetVerticalBorder(col + colspan);
            for (int i = rowRange.GetStartRow() - largeTableIndexOffset + row - rowspan + 1; i < rowRange.GetStartRow(
                ) - largeTableIndexOffset + row + 1; i++) {
                border = borderList[i];
                if (null != border && border.GetWidth() > indents[1]) {
                    indents[1] = border.GetWidth();
                }
            }
            // process bottom border
            borderList = GetHorizontalBorder(rowRange.GetStartRow() + row + 1);
            for (int i = col; i < col + colspan; i++) {
                border = borderList[i];
                if (null != border && border.GetWidth() > indents[2]) {
                    indents[2] = border.GetWidth();
                }
            }
            // process left border
            borderList = GetVerticalBorder(col);
            for (int i = rowRange.GetStartRow() - largeTableIndexOffset + row - rowspan + 1; i < rowRange.GetStartRow(
                ) - largeTableIndexOffset + row + 1; i++) {
                border = borderList[i];
                if (null != border && border.GetWidth() > indents[3]) {
                    indents[3] = border.GetWidth();
                }
            }
            return indents;
        }

        public virtual Border GetWidestHorizontalBorder(int row, bool forceNotToProcessAsFirst, bool forceNotToProcessWithLast
            ) {
            return GetWidestBorder(GetHorizontalBorder(row, forceNotToProcessAsFirst, forceNotToProcessWithLast));
        }

        public virtual Border GetWidestHorizontalBorder(int row, int start, int end, bool forceNotToProcessAsFirst
            , bool forceNotToProcessAsLast) {
            return GetWidestBorder(GetHorizontalBorder(row, forceNotToProcessAsFirst, forceNotToProcessAsLast), start, 
                end);
        }

        public override Border GetWidestHorizontalBorder(int row, int start, int end) {
            return GetWidestHorizontalBorder(row, start, end, false, false);
        }

        public override Border GetWidestVerticalBorder(int col) {
            Border theWidestBorder = null;
            if (col >= 0 && col < verticalBorders.Count) {
                theWidestBorder = GetWidestBorder(GetVerticalBorder(col));
            }
            return theWidestBorder;
        }

        public override Border GetWidestVerticalBorder(int col, int start, int end) {
            Border theWidestBorder = null;
            if (col >= 0 && col < verticalBorders.Count) {
                theWidestBorder = GetWidestBorder(GetVerticalBorder(col), start, end);
            }
            return theWidestBorder;
        }

        public override IList<Border> GetVerticalBorder(int index) {
            // TODO REFACTOR
            if (index == 0) {
                IList<Border> borderList = GetBorderList(null, tableBoundingBorders[3], verticalBorders[0].Count);
                IList<Border> leftVerticalBorder = verticalBorders[0];
                for (int i = 0; i < leftVerticalBorder.Count; i++) {
                    if (null == borderList[i] || (null != leftVerticalBorder[i] && leftVerticalBorder[i].GetWidth() > borderList
                        [i].GetWidth())) {
                        borderList[i] = leftVerticalBorder[i];
                    }
                }
                return borderList;
            }
            else {
                if (index == numberOfColumns) {
                    IList<Border> borderList = GetBorderList(null, tableBoundingBorders[1], verticalBorders[0].Count);
                    IList<Border> rightVerticalBorder = verticalBorders[verticalBorders.Count - 1];
                    for (int i = 0; i < rightVerticalBorder.Count; i++) {
                        if (null == borderList[i] || (null != rightVerticalBorder[i] && rightVerticalBorder[i].GetWidth() > borderList
                            [i].GetWidth())) {
                            borderList[i] = rightVerticalBorder[i];
                        }
                    }
                    return borderList;
                }
                else {
                    return verticalBorders[index];
                }
            }
        }

        public override float GetMaxTopWidth() {
            float width = 0;
            Border widestBorder = GetWidestHorizontalBorder(rowRange.GetStartRow(), false, false);
            if (null != widestBorder && widestBorder.GetWidth() >= width) {
                width = widestBorder.GetWidth();
            }
            return width;
        }

        public override float GetMaxBottomWidth() {
            float width = 0;
            Border widestBorder = GetWidestHorizontalBorder(rowRange.GetFinishRow() + 1, false, false);
            // TODO
            if (null != widestBorder && widestBorder.GetWidth() >= width) {
                width = widestBorder.GetWidth();
            }
            return width;
        }

        public override IList<Border> GetHorizontalBorder(int index) {
            return GetHorizontalBorder(index, false, false);
        }

        public virtual IList<Border> GetHorizontalBorder(int index, bool forceNotToProcessAsFirst, bool forceNotToProcessAsLast
            ) {
            if (index == largeTableIndexOffset) {
                IList<Border> firstBorderOnCurrentPage = GetBorderList(topBorderCollapseWith, tableBoundingBorders[0], numberOfColumns
                    );
                return GetCollapsedList(horizontalBorders[index - largeTableIndexOffset], firstBorderOnCurrentPage);
            }
            else {
                if (index - largeTableIndexOffset == horizontalBorders.Count - 1) {
                    IList<Border> lastBorderOnCurrentPage = GetBorderList(bottomBorderCollapseWith, tableBoundingBorders[2], numberOfColumns
                        );
                    return GetCollapsedList(horizontalBorders[index - largeTableIndexOffset], lastBorderOnCurrentPage);
                }
                else {
                    if (index - largeTableIndexOffset == rowRange.GetStartRow() && !forceNotToProcessAsFirst) {
                        IList<Border> firstBorderOnCurrentPage = GetBorderList(topBorderCollapseWith, tableBoundingBorders[0], numberOfColumns
                            );
                        if (0 != rows.Count) {
                            int col = 0;
                            int row = index;
                            while (col < numberOfColumns) {
                                if (null != rows[row - largeTableIndexOffset][col] && row - index + 1 <= (int)((Cell)rows[row - largeTableIndexOffset
                                    ][col].GetModelElement()).GetRowspan()) {
                                    CellRenderer cell = rows[row - largeTableIndexOffset][col];
                                    Border cellModelTopBorder = GetCellSideBorder(((Cell)cell.GetModelElement()), Property.BORDER_TOP);
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
                        if ((index == rowRange.GetFinishRow() + 1 && !forceNotToProcessAsLast)) {
                            IList<Border> lastBorderOnCurrentPage = GetBorderList(bottomBorderCollapseWith, tableBoundingBorders[2], numberOfColumns
                                );
                            if (0 != rows.Count) {
                                int col = 0;
                                int row = index - 1;
                                while (col < numberOfColumns) {
                                    if (null != rows[row - largeTableIndexOffset][col]) {
                                        // TODO
                                        CellRenderer cell = rows[row - largeTableIndexOffset][col];
                                        Border cellModelBottomBorder = GetCellSideBorder(((Cell)cell.GetModelElement()), Property.BORDER_BOTTOM);
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
                            return horizontalBorders[index - Math.Max(largeTableIndexOffset, 0)];
                        }
                    }
                }
            }
        }

        // endregion
        // region setters
        public virtual iText.Layout.Renderer.CollapsedTableBorders SetTopBorderCollapseWith(IList<Border> topBorderCollapseWith
            ) {
            if (null != this.topBorderCollapseWith) {
                this.topBorderCollapseWith.Clear();
            }
            else {
                this.topBorderCollapseWith = new List<Border>();
            }
            if (null != topBorderCollapseWith) {
                this.topBorderCollapseWith.AddAll(topBorderCollapseWith);
            }
            return this;
        }

        public virtual iText.Layout.Renderer.CollapsedTableBorders SetBottomBorderCollapseWith(IList<Border> bottomBorderCollapseWith
            ) {
            if (null != this.bottomBorderCollapseWith) {
                this.bottomBorderCollapseWith.Clear();
            }
            else {
                this.bottomBorderCollapseWith = new List<Border>();
            }
            if (null != bottomBorderCollapseWith) {
                this.bottomBorderCollapseWith.AddAll(bottomBorderCollapseWith);
            }
            return this;
        }

        //endregion
        // region building border arrays
        protected internal virtual void PrepareBuildingBordersArrays(CellRenderer cell, Border[] tableBorders, int
             colNum, int row, int col) {
            Border[] cellBorders = cell.GetBorders();
            int colspan = (int)cell.GetPropertyAsInteger(Property.COLSPAN);
            if (0 == col) {
                cell.SetProperty(Property.BORDER_LEFT, GetCollapsedBorder(cellBorders[3], tableBorders[3]));
            }
            if (colNum == col + colspan) {
                cell.SetProperty(Property.BORDER_RIGHT, GetCollapsedBorder(cellBorders[1], tableBorders[1]));
            }
            if (0 == row) {
                cell.SetProperty(Property.BORDER_TOP, GetCollapsedBorder(cellBorders[0], tableBorders[0]));
            }
            if (rows.Count - 1 == row) {
                cell.SetProperty(Property.BORDER_BOTTOM, GetCollapsedBorder(cellBorders[2], tableBorders[2]));
            }
        }

        protected internal virtual void BuildBordersArrays(CellRenderer cell, int row, int col) {
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
                while (j > 0 && rows.Count != nextCellRow && (j + rows[nextCellRow][j].GetPropertyAsInteger(Property.COLSPAN
                    ) != col || nextCellRow - rows[nextCellRow][j].GetPropertyAsInteger(Property.ROWSPAN) + 1 != row));
                if (j >= 0 && nextCellRow != rows.Count) {
                    CellRenderer nextCell = rows[nextCellRow][j];
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
                if (row == nextCellRow - nextCell.GetPropertyAsInteger(Property.ROWSPAN)) {
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
            IList<Border> borders = GetHorizontalBorder(rowRange.GetStartRow() + i);
            /*- largeTableIndexOffset*/
            float x1 = startX;
            float x2 = x1 + countedColumnWidth[0];
            if (i == 0) {
                if (true) {
                    /*bordersHandler.getFirstVerticalBorder().size() > verticalBordersIndexOffset && bordersHandler.getLastVerticalBorder().size() > verticalBordersIndexOffset*/
                    Border firstBorder = GetFirstVerticalBorder()[rowRange.GetStartRow() - largeTableIndexOffset];
                    if (firstBorder != null) {
                        x1 -= firstBorder.GetWidth() / 2;
                    }
                }
            }
            else {
                if (i == rowRange.GetFinishRow() - rowRange.GetStartRow() + 1) {
                    if (true) {
                        /*bordersHandler.getFirstVerticalBorder().size() > verticalBordersIndexOffset &&
                        bordersHandler.getLastVerticalBorder().size() > verticalBordersIndexOffset*/
                        Border firstBorder = GetFirstVerticalBorder()[rowRange.GetStartRow() - largeTableIndexOffset + rowRange.GetFinishRow
                            () - rowRange.GetStartRow() + 1 - 1];
                        if (firstBorder != null) {
                            x1 -= firstBorder.GetWidth() / 2;
                        }
                    }
                }
            }
            int j;
            for (j = 1; j < borders.Count; j++) {
                Border prevBorder = borders[j - 1];
                Border curBorder = borders[j];
                if (prevBorder != null) {
                    if (!prevBorder.Equals(curBorder)) {
                        prevBorder.DrawCellBorder(canvas, x1, y1, x2, y1);
                        prevBorder.DrawCellBorder(canvas, x1, y1, x2, y1);
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
                if (true) {
                    /*bordersHandler.getVerticalBorder(j).size() > verticalBordersIndexOffset*/
                    if (i == 0) {
                        if (GetVerticalBorder(j)[rowRange.GetStartRow() - largeTableIndexOffset + i] != null) {
                            x2 += GetVerticalBorder(j)[rowRange.GetStartRow() - largeTableIndexOffset + i].GetWidth() / 2;
                        }
                    }
                    else {
                        if (i == rowRange.GetFinishRow() - rowRange.GetStartRow() + 1 && GetVerticalBorder(j).Count >= rowRange.GetStartRow
                            () - largeTableIndexOffset + i - 1 && GetVerticalBorder(j)[rowRange.GetStartRow() - largeTableIndexOffset
                             + i - 1] != null) {
                            x2 += GetVerticalBorder(j)[rowRange.GetStartRow() - largeTableIndexOffset + i - 1].GetWidth() / 2;
                        }
                    }
                }
                lastBorder.DrawCellBorder(canvas, x1, y1, x2, y1);
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
                Border prevBorder = borders[rowRange.GetStartRow() - largeTableIndexOffset + j - 1];
                Border curBorder = borders[rowRange.GetStartRow() - largeTableIndexOffset + j];
                if (prevBorder != null) {
                    if (!prevBorder.Equals(curBorder)) {
                        prevBorder.DrawCellBorder(canvas, x1, y1, x1, y2);
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
            Border lastBorder = borders[rowRange.GetStartRow() - largeTableIndexOffset + j - 1];
            if (lastBorder != null) {
                lastBorder.DrawCellBorder(canvas, x1, y1, x1, y2);
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
        protected internal override TableBorders ApplyTopBorder(Rectangle occupiedBox, Rectangle layoutBox, bool isEmpty
            , bool isComplete, bool reverse) {
            if (!isEmpty) {
                return ApplyTopBorder(occupiedBox, layoutBox, reverse);
            }
            else {
                if (isComplete) {
                    // process empty table
                    ApplyTopBorder(occupiedBox, layoutBox, reverse);
                    return ApplyTopBorder(occupiedBox, layoutBox, reverse);
                }
            }
            return this;
        }

        protected internal override TableBorders ApplyBottomBorder(Rectangle occupiedBox, Rectangle layoutBox, bool
             isEmpty, bool reverse) {
            if (!isEmpty) {
                return ApplyBottomBorder(occupiedBox, layoutBox, reverse);
            }
            return this;
        }

        protected internal override TableBorders ApplyTopBorder(Rectangle occupiedBox, Rectangle layoutBox, bool reverse
            ) {
            float topIndent = (reverse ? -1 : 1) * GetMaxTopWidth();
            layoutBox.DecreaseHeight(topIndent / 2);
            occupiedBox.MoveDown(topIndent / 2).IncreaseHeight(topIndent / 2);
            return this;
        }

        protected internal override TableBorders ApplyBottomBorder(Rectangle occupiedBox, Rectangle layoutBox, bool
             reverse) {
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
        // region footer/header
        protected internal static TableBorders ProcessRendererBorders(TableRenderer renderer) {
            // TODO No sence in this method
            renderer.bordersHandler = new iText.Layout.Renderer.CollapsedTableBorders(renderer.rows, ((Table)renderer.
                GetModelElement()).GetNumberOfColumns());
            renderer.bordersHandler.SetTableBoundingBorders(renderer.GetBorders());
            renderer.bordersHandler.InitializeBorders();
            renderer.bordersHandler.SetRowRange(renderer.rowRange);
            ((iText.Layout.Renderer.CollapsedTableBorders)renderer.bordersHandler).CollapseAllBordersAndEmptyRows();
            return renderer.bordersHandler;
        }

        protected internal override TableBorders UpdateOnNewPage(bool isOriginalNonSplitRenderer, bool isFooterOrHeader
            , TableRenderer currentRenderer, TableRenderer headerRenderer, TableRenderer footerRenderer) {
            if (!isFooterOrHeader) {
                // collapse all cell borders
                if (isOriginalNonSplitRenderer) {
                    if (null != rows) {
                        CollapseAllBordersAndEmptyRows();
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
                ProcessRendererBorders(footerRenderer);
                float rightFooterBorderWidth = footerRenderer.bordersHandler.GetMaxRightWidth();
                float leftFooterBorderWidth = footerRenderer.bordersHandler.GetMaxLeftWidth();
                leftBorderMaxWidth = Math.Max(leftBorderMaxWidth, leftFooterBorderWidth);
                rightBorderMaxWidth = Math.Max(rightBorderMaxWidth, rightFooterBorderWidth);
            }
            if (null != headerRenderer) {
                ProcessRendererBorders(headerRenderer);
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

        protected internal override TableBorders ConsiderFooter(TableBorders footerBordersHandler, bool hasContent
            ) {
            ((iText.Layout.Renderer.CollapsedTableBorders)footerBordersHandler).SetTopBorderCollapseWith(hasContent ? 
                GetLastHorizontalBorder() : GetTopBorderCollapseWith());
            SetBottomBorderCollapseWith(footerBordersHandler.GetHorizontalBorder(0));
            return this;
        }

        protected internal override TableBorders ConsiderHeader(TableBorders headerBordersHandler, bool changeThis
            ) {
            ((iText.Layout.Renderer.CollapsedTableBorders)headerBordersHandler).SetBottomBorderCollapseWith(GetHorizontalBorder
                (rowRange.GetStartRow()));
            if (changeThis) {
                SetTopBorderCollapseWith(headerBordersHandler.GetLastHorizontalBorder());
            }
            return this;
        }

        protected internal override TableBorders ConsiderHeaderOccupiedArea(Rectangle occupiedBox, Rectangle layoutBox
            ) {
            float topBorderMaxWidth = GetMaxTopWidth();
            layoutBox.IncreaseHeight(topBorderMaxWidth);
            occupiedBox.MoveUp(topBorderMaxWidth).DecreaseHeight(topBorderMaxWidth);
            return this;
        }
        // endregion
    }
}
