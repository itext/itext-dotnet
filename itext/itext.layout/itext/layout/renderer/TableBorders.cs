using System;
using System.Collections.Generic;
using iText.IO.Log;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;

namespace iText.Layout.Renderer {
    public class TableBorders {
        private IList<IList<Border>> horizontalBorders;

        private IList<IList<Border>> verticalBorders;

        private readonly int numberOfColumns;

        private Border[] tableBoundingBorders;

        private IList<CellRenderer[]> rows;

        private Table.RowRange rowRange;

        private IList<Border> topBorderCollapseWith;

        private IList<Border> bottomBorderCollapseWith;

        public TableBorders(IList<CellRenderer[]> rows, int numberOfColumns) {
            // region constructors
            this.rows = rows;
            this.numberOfColumns = numberOfColumns;
            verticalBorders = new List<IList<Border>>();
            horizontalBorders = new List<IList<Border>>();
            tableBoundingBorders = null;
            topBorderCollapseWith = new List<Border>();
            bottomBorderCollapseWith = new List<Border>();
        }

        public TableBorders(IList<CellRenderer[]> rows, int numberOfColumns, Border[] tableBoundingBorders)
            : this(rows, numberOfColumns) {
            SetTableBoundingBorders(tableBoundingBorders);
        }

        // endregion
        // region collapsing and correction
        protected internal virtual iText.Layout.Renderer.TableBorders CollapseAllBordersAndEmptyRows(IList<CellRenderer
            []> rows, Border[] tableBorders, int startRow, int finishRow) {
            CellRenderer[] currentRow;
            int[] rowsToDelete = new int[numberOfColumns];
            for (int row = startRow; row <= finishRow; row++) {
                currentRow = rows[row];
                bool hasCells = false;
                for (int col = 0; col < numberOfColumns; col++) {
                    if (null != currentRow[col]) {
                        int colspan = (int)currentRow[col].GetPropertyAsInteger(Property.COLSPAN);
                        PrepareBuildingBordersArrays(currentRow[col], tableBorders, numberOfColumns, row, col);
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
                    rows.Remove(currentRow);
                    row--;
                    finishRow--;
                    for (int i = 0; i < numberOfColumns; i++) {
                        rowsToDelete[i]++;
                    }
                    if (row == finishRow) {
                        ILogger logger = LoggerFactory.GetLogger(typeof(TableRenderer));
                        logger.Warn(iText.IO.LogMessageConstant.LAST_ROW_IS_NOT_COMPLETE);
                    }
                }
            }
            return this;
        }

        //    protected TableBorders processEmptyTable(List<Border> lastFlushedBorder) { // FIXNE
        ////        List<Border> topHorizontalBorders = new ArrayList<Border>();
        ////        List<Border> bottomHorizontalBorders = new ArrayList<Border>();
        ////        if (null != lastFlushedBorder && 0 != lastFlushedBorder.size()) {
        ////            topHorizontalBorders = lastFlushedBorder;
        ////        } else {
        ////            for (int i = 0; i < numberOfColumns; i++) {
        ////                topHorizontalBorders.add(null);
        ////            }
        ////        }
        ////
        ////        // collapse with table bottom border
        ////        for (int i = 0; i < topHorizontalBorders.size(); i++) {
        ////            Border border = topHorizontalBorders.get(i);
        ////            if (null == border || (null != tableBoundingBorders[0] && border.getWidth() < tableBoundingBorders[0].getWidth())) {
        ////                topHorizontalBorders.set(i, tableBoundingBorders[0]);
        ////            }
        ////            bottomHorizontalBorders.add(tableBoundingBorders[2]);
        ////        }
        ////        horizontalBorders.set(horizontalBordersIndexOffset, topHorizontalBorders);
        ////        if (horizontalBorders.size() == horizontalBordersIndexOffset + 1) {
        ////            horizontalBorders.add(bottomHorizontalBorders);
        ////        } else {
        ////            horizontalBorders.set(horizontalBordersIndexOffset + 1, bottomHorizontalBorders);
        ////        }
        ////
        ////        if (0 != verticalBorders.size()) {
        ////            verticalBorders.get(0).set(verticalBordersIndexOffset, (tableBoundingBorders[3]));
        ////            for (int i = 1; i < numberOfColumns; i++) {
        ////                verticalBorders.get(i).set(verticalBordersIndexOffset, null);
        ////            }
        ////            verticalBorders.get(verticalBorders.size() - 1).set(verticalBordersIndexOffset, (tableBoundingBorders[1]));
        ////        } else {
        ////            List<Border> tempBorders;
        ////            for (int i = 0; i < numberOfColumns + 1; i++) {
        ////                tempBorders = new ArrayList<Border>();
        ////                tempBorders.add(null);
        ////                verticalBorders.add(tempBorders);
        ////            }
        ////            verticalBorders.get(0).set(0, tableBoundingBorders[3]);
        ////            verticalBorders.get(numberOfColumns).set(0, tableBoundingBorders[1]);
        ////
        ////        }
        ////
        //        return this;
        //    }
        // endregion
        // region intializers
        protected internal virtual void InitializeBorders(IList<Border> lastFlushedRowBottomBorder, bool isFirstOnPage
            ) {
            IList<Border> tempBorders;
            // initialize vertical borders
            if (0 != rows.Count) {
                while (numberOfColumns + 1 > verticalBorders.Count) {
                    tempBorders = new List<Border>();
                    while (rows.Count > tempBorders.Count) {
                        tempBorders.Add(null);
                    }
                    verticalBorders.Add(tempBorders);
                }
            }
            // initialize horizontal borders
            while (rows.Count + 1 > horizontalBorders.Count) {
                tempBorders = new List<Border>();
                while (numberOfColumns > tempBorders.Count) {
                    tempBorders.Add(null);
                }
                horizontalBorders.Add(tempBorders);
            }
            // Notice that the first row on the page shouldn't collapse with the last on the previous one
            if (null != lastFlushedRowBottomBorder && 0 < lastFlushedRowBottomBorder.Count && !isFirstOnPage) {
                // TODO
                tempBorders = new List<Border>();
                foreach (Border border in lastFlushedRowBottomBorder) {
                    tempBorders.Add(border);
                }
                horizontalBorders[0] = tempBorders;
            }
        }

        //endregion
        // region getters
        public virtual float[] GetCellBorderIndents(int row, int col, int rowspan, int colspan, bool forceNotToProcessAsLast
            ) {
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
            for (int i = rowRange.GetStartRow() + row - rowspan + 1; i < rowRange.GetStartRow() + row + 1; i++) {
                border = borderList[i];
                if (null != border && border.GetWidth() > indents[1]) {
                    indents[1] = border.GetWidth();
                }
            }
            // process bottom border
            borderList = GetHorizontalBorder(rowRange.GetStartRow() + row + 1, false, forceNotToProcessAsLast);
            for (int i = col; i < col + colspan; i++) {
                border = borderList[i];
                if (null != border && border.GetWidth() > indents[2]) {
                    indents[2] = border.GetWidth();
                }
            }
            // process left border
            borderList = GetVerticalBorder(col);
            for (int i = rowRange.GetStartRow() + row - rowspan + 1; i < rowRange.GetStartRow() + row + 1; i++) {
                border = borderList[i];
                if (null != border && border.GetWidth() > indents[3]) {
                    indents[3] = border.GetWidth();
                }
            }
            return indents;
        }

        public virtual float[] GetCellBorderIndents(int row, int col, int rowspan, int colspan) {
            return GetCellBorderIndents(row, col, rowspan, colspan, false);
        }

        public virtual Border GetWidestHorizontalBorder(int row, bool forceNotToProcessAsFirst, bool forceNotToProcessWithLast
            ) {
            Border theWidestBorder = null;
            if (row >= 0 && row < horizontalBorders.Count) {
                theWidestBorder = GetWidestBorder(GetHorizontalBorder(row, forceNotToProcessAsFirst, forceNotToProcessWithLast
                    ));
            }
            return theWidestBorder;
        }

        public virtual Border GetWidestHorizontalBorder(int row) {
            return GetWidestHorizontalBorder(row, false, false);
        }

        public virtual Border GetWidestHorizontalBorder(int row, int start, int end, bool forceNotToProcessAsFirst
            , bool forceNotToProcessAsLast) {
            Border theWidestBorder = null;
            if (row >= 0 && row < horizontalBorders.Count) {
                theWidestBorder = GetWidestBorder(GetHorizontalBorder(row, forceNotToProcessAsFirst, forceNotToProcessAsLast
                    ), start, end);
            }
            return theWidestBorder;
        }

        public virtual Border GetWidestHorizontalBorder(int row, int start, int end) {
            return GetWidestHorizontalBorder(row, start, end, false, false);
        }

        public virtual Border GetWidestVerticalBorder(int col) {
            Border theWidestBorder = null;
            if (col >= 0 && col < verticalBorders.Count) {
                theWidestBorder = GetWidestBorder(GetVerticalBorder(col));
            }
            return theWidestBorder;
        }

        public virtual Border GetWidestVerticalBorder(int col, int start, int end) {
            Border theWidestBorder = null;
            if (col >= 0 && col < verticalBorders.Count) {
                theWidestBorder = GetWidestBorder(GetVerticalBorder(col), start, end);
            }
            return theWidestBorder;
        }

        public virtual float GetMaxTopWidth(bool forceNoToProcessAsFirst) {
            float width = 0;
            Border widestBorder = GetWidestHorizontalBorder(rowRange.GetStartRow(), forceNoToProcessAsFirst, false);
            if (null != widestBorder && widestBorder.GetWidth() >= width) {
                width = widestBorder.GetWidth();
            }
            return width;
        }

        public virtual float GetMaxBottomWidth(bool forceNoToProcessAsLast) {
            float width = 0;
            Border widestBorder = GetWidestHorizontalBorder(rowRange.GetFinishRow() + 1, false, forceNoToProcessAsLast
                );
            // TODO
            if (null != widestBorder && widestBorder.GetWidth() >= width) {
                width = widestBorder.GetWidth();
            }
            return width;
        }

        public virtual float GetMaxRightWidth() {
            float width = 0;
            Border widestBorder = GetWidestVerticalBorder(verticalBorders.Count - 1);
            if (null != widestBorder && widestBorder.GetWidth() >= width) {
                width = widestBorder.GetWidth();
            }
            return width;
        }

        public virtual float GetMaxLeftWidth() {
            float width = 0;
            Border widestBorder = GetWidestVerticalBorder(0);
            if (null != widestBorder && widestBorder.GetWidth() >= width) {
                width = widestBorder.GetWidth();
            }
            return width;
        }

        public virtual IList<Border> GetVerticalBorder(int index) {
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

        public virtual IList<Border> GetHorizontalBorder(int index) {
            return GetHorizontalBorder(index, false, false);
        }

        public virtual IList<Border> GetHorizontalBorder(int index, bool forceNotToProcessAsFirst, bool forceNotToProcessAsLast
            ) {
            if (index == rowRange.GetStartRow() && !forceNotToProcessAsFirst) {
                IList<Border> firstBorderOnCurrentPage = GetBorderList(topBorderCollapseWith, tableBoundingBorders[0], numberOfColumns
                    );
                if (0 != rows.Count) {
                    int col = 0;
                    int row = index;
                    while (col < numberOfColumns) {
                        if (null != rows[row][col] && row == (int)rows[row][col].GetPropertyAsInteger(Property.ROWSPAN) + (int)((Cell
                            )rows[row][col].GetModelElement()).GetRow() - 1) {
                            CellRenderer cell = rows[row][col];
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
                if (((index == rowRange.GetFinishRow() + 1 && !forceNotToProcessAsLast) || index == horizontalBorders.Count
                     - 1)) {
                    IList<Border> lastBorderOnCurrentPage = GetBorderList(bottomBorderCollapseWith, tableBoundingBorders[2], numberOfColumns
                        );
                    if (0 != rows.Count) {
                        int col = 0;
                        int row = index - 1;
                        while (col < numberOfColumns) {
                            if (null != rows[row][col]) {
                                // TODO
                                CellRenderer cell = rows[row][col];
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
                    return horizontalBorders[index];
                }
            }
        }

        public virtual IList<Border> GetFirstHorizontalBorder() {
            return GetHorizontalBorder(rowRange.GetStartRow());
        }

        public virtual IList<Border> GetLastHorizontalBorder() {
            return GetHorizontalBorder(rowRange.GetFinishRow() + 1);
        }

        public virtual IList<Border> GetFirstVerticalBorder() {
            return GetVerticalBorder(0);
        }

        public virtual IList<Border> GetLastVerticalBorder() {
            return GetVerticalBorder(verticalBorders.Count - 1);
        }

        public virtual int GetNumberOfColumns() {
            return numberOfColumns;
        }

        public virtual Border[] GetTableBoundingBorders() {
            return tableBoundingBorders;
        }

        public virtual int GetVerticalBordersSize() {
            return verticalBorders.Count;
        }

        public virtual int GetHorizontalBordersSize() {
            return verticalBorders.Count;
        }

        // endregion
        // region setters
        protected internal virtual iText.Layout.Renderer.TableBorders SetTableBoundingBorders(Border[] borders) {
            if (null == tableBoundingBorders) {
                tableBoundingBorders = new Border[borders.Length];
            }
            for (int i = 0; i < borders.Length; i++) {
                tableBoundingBorders[i] = borders[i];
            }
            return this;
        }

        protected internal virtual iText.Layout.Renderer.TableBorders SetRowRange(Table.RowRange rowRange) {
            this.rowRange = rowRange;
            return this;
        }

        protected internal virtual iText.Layout.Renderer.TableBorders SetStartRow(int row) {
            this.rowRange = new Table.RowRange(row, rowRange.GetFinishRow());
            return this;
        }

        protected internal virtual iText.Layout.Renderer.TableBorders SetFinishRow(int row) {
            this.rowRange = new Table.RowRange(rowRange.GetStartRow(), row);
            return this;
        }

        public virtual iText.Layout.Renderer.TableBorders SetTopBorderCollapseWith(IList<Border> topBorderCollapseWith
            ) {
            this.topBorderCollapseWith.Clear();
            if (null != topBorderCollapseWith) {
                this.topBorderCollapseWith.AddAll(topBorderCollapseWith);
            }
            return this;
        }

        public virtual iText.Layout.Renderer.TableBorders SetBottomBorderCollapseWith(IList<Border> bottomBorderCollapseWith
            ) {
            this.bottomBorderCollapseWith.Clear();
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
            // process big rowspan
            if (rowspan > 1) {
                int numOfColumns = numberOfColumns;
                for (int k = row - rowspan + 1; k <= row; k++) {
                    IList<Border> borders = horizontalBorders[k];
                    if (borders.Count < numOfColumns) {
                        for (int j = borders.Count; j < numOfColumns; j++) {
                            borders.Add(null);
                        }
                    }
                }
            }
            // consider left border
            for (int j = row - rowspan + 1; j <= row; j++) {
                CheckAndReplaceBorderInArray(verticalBorders, colN, j, cellBorders[3], false);
            }
            // consider right border
            for (int i = row - rowspan + 1; i <= row; i++) {
                CheckAndReplaceBorderInArray(verticalBorders, colN + colspan, i, cellBorders[1], true);
            }
            // process big colspan
            if (colspan > 1) {
                for (int k = colN; k <= colspan + colN; k++) {
                    IList<Border> borders = verticalBorders[k];
                    if (borders.Count < row + rowspan) {
                        for (int l = borders.Count; l < row + rowspan; l++) {
                            borders.Add(null);
                        }
                    }
                }
            }
        }

        // endregion
        //region static
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

        public static Border GetCellSideBorder(Cell cellModel, int borderType) {
            Border cellModelSideBorder = cellModel.GetProperty(borderType);
            if (null == cellModelSideBorder && !cellModel.HasProperty(borderType)) {
                cellModelSideBorder = cellModel.GetProperty(Property.BORDER);
                if (null == cellModelSideBorder && !cellModel.HasProperty(Property.BORDER)) {
                    cellModelSideBorder = cellModel.GetDefaultProperty(Property.BORDER);
                }
            }
            // TODO Mayvb we need to foresee the possibility of default side border property
            return cellModelSideBorder;
        }

        public static Border GetWidestBorder(IList<Border> borderList) {
            Border theWidestBorder = null;
            if (0 != borderList.Count) {
                foreach (Border border in borderList) {
                    if (null != border && (null == theWidestBorder || border.GetWidth() > theWidestBorder.GetWidth())) {
                        theWidestBorder = border;
                    }
                }
            }
            return theWidestBorder;
        }

        public static Border GetWidestBorder(IList<Border> borderList, int start, int end) {
            Border theWidestBorder = null;
            if (0 != borderList.Count) {
                foreach (Border border in borderList.SubList(start, end)) {
                    if (null != border && (null == theWidestBorder || border.GetWidth() > theWidestBorder.GetWidth())) {
                        theWidestBorder = border;
                    }
                }
            }
            return theWidestBorder;
        }

        private static IList<Border> GetBorderList(Border border, int n) {
            IList<Border> borderList = new List<Border>();
            for (int i = 0; i < n; i++) {
                borderList.Add(border);
            }
            return borderList;
        }

        private static IList<Border> GetBorderList(IList<Border> originalList, Border borderToCollapse, int size) {
            IList<Border> borderList = new List<Border>();
            if (null != originalList) {
                borderList.AddAll(originalList);
            }
            while (borderList.Count < size) {
                borderList.Add(borderToCollapse);
            }
            int end = null == originalList ? size : Math.Min(originalList.Count, size);
            for (int i = 0; i < end; i++) {
                if (null == borderList[i] || (null != borderToCollapse && borderList[i].GetWidth() <= borderToCollapse.GetWidth
                    ())) {
                    borderList[i] = borderToCollapse;
                }
            }
            return borderList;
        }

        // endregion
        // region lowlevel logic
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
        // region update
        //    protected TableBorders updateTopBorder(List<Border> newBorder, boolean[] useOldBorders) {
        ////        updateBorder(horizontalBorders.get(horizontalBordersIndexOffset), newBorder, useOldBorders);
        //        return this;
        //    }
        //
        //    protected TableBorders updateBottomBorder(List<Border> newBorder, boolean[] useOldBorders) {
        //        updateBorder(horizontalBorders.get(horizontalBorders.size() - 1), newBorder, useOldBorders);
        //        return this;
        //    }
        //
        //    protected TableBorders updateBorder(List<Border> oldBorder, List<Border> newBorders, boolean[] isOldBorder) {
        //        for (int i = 0; i < oldBorder.size(); i++) {
        //            if (!isOldBorder[i]) {
        //                oldBorder.set(i, newBorders.get(i));
        //            }
        //        }
        //        return this;
        //    }
        // endregion
    }
}
