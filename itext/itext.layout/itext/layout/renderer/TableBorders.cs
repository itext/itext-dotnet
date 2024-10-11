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
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Canvas;
using iText.Layout.Borders;
using iText.Layout.Properties;

namespace iText.Layout.Renderer {
//\cond DO_NOT_DOCUMENT
    internal abstract class TableBorders {
        /// <summary>Horizontal borders of the table.</summary>
        /// <remarks>
        /// Horizontal borders of the table.
        /// It consists of a list, each item of which represents
        /// a horizontal border of a row, each of them is a list of borders of the cells.
        /// The amount of the lists is the number of rows + 1, the size of each of these lists
        /// corresponds to the number of columns.
        /// </remarks>
        protected internal IList<IList<Border>> horizontalBorders = new List<IList<Border>>();

        /// <summary>Vertical borders of the table.</summary>
        /// <remarks>
        /// Vertical borders of the table.
        /// It consists of a list, each item of which represents
        /// a vertical border of a row, each of them is a list of borders of the cells.
        /// The amount of the lists is the number of columns + 1, the size of each of these lists
        /// corresponds to the number of rows.
        /// </remarks>
        protected internal IList<IList<Border>> verticalBorders = new List<IList<Border>>();

        /// <summary>The number of the table's columns.</summary>
        protected internal readonly int numberOfColumns;

        /// <summary>The outer borders of the table (as body).</summary>
        protected internal Border[] tableBoundingBorders = new Border[4];

        /// <summary>All the cells of the table.</summary>
        /// <remarks>
        /// All the cells of the table.
        /// Each item of the list represents a row and consists of its cells.
        /// </remarks>
        protected internal IList<CellRenderer[]> rows;

        /// <summary>The first row, which should be processed on this area.</summary>
        /// <remarks>
        /// The first row, which should be processed on this area.
        /// The value of this field varies from area to area.
        /// It's zero-based and inclusive.
        /// </remarks>
        protected internal int startRow;

        /// <summary>The last row, which should be processed on this area.</summary>
        /// <remarks>
        /// The last row, which should be processed on this area.
        /// The value of this field varies from area to area.
        /// It's zero-based and inclusive. The last border will have index (finishRow+1) because
        /// the number of borders is greater by one than the number of rows
        /// </remarks>
        protected internal int finishRow;

        /// <summary>The width of the widest left border.</summary>
        protected internal float leftBorderMaxWidth;

        /// <summary>The width of the widest right border.</summary>
        protected internal float rightBorderMaxWidth;

        /// <summary>The number of rows flushed to the table.</summary>
        /// <remarks>
        /// The number of rows flushed to the table.
        /// Its value is zero for regular tables. The field makes sense only for large tables.
        /// </remarks>
        protected internal int largeTableIndexOffset = 0;

        public TableBorders(IList<CellRenderer[]> rows, int numberOfColumns, Border[] tableBoundingBorders) {
            this.rows = rows;
            this.numberOfColumns = numberOfColumns;
            SetTableBoundingBorders(tableBoundingBorders);
        }

        public TableBorders(IList<CellRenderer[]> rows, int numberOfColumns, Border[] tableBoundingBorders, int largeTableIndexOffset
            )
            : this(rows, numberOfColumns, tableBoundingBorders) {
            this.largeTableIndexOffset = largeTableIndexOffset;
        }

        // region abstract
        // region draw
        protected internal abstract iText.Layout.Renderer.TableBorders DrawHorizontalBorder(PdfCanvas canvas, TableBorderDescriptor
             borderDescriptor);

        protected internal abstract iText.Layout.Renderer.TableBorders DrawVerticalBorder(PdfCanvas canvas, TableBorderDescriptor
             borderDescriptor);

        // endregion
        // region area occupation
        protected internal abstract iText.Layout.Renderer.TableBorders ApplyTopTableBorder(Rectangle occupiedBox, 
            Rectangle layoutBox, bool isEmpty, bool force, bool reverse);

        protected internal abstract iText.Layout.Renderer.TableBorders ApplyTopTableBorder(Rectangle occupiedBox, 
            Rectangle layoutBox, bool reverse);

        protected internal abstract iText.Layout.Renderer.TableBorders ApplyBottomTableBorder(Rectangle occupiedBox
            , Rectangle layoutBox, bool isEmpty, bool force, bool reverse);

        protected internal abstract iText.Layout.Renderer.TableBorders ApplyBottomTableBorder(Rectangle occupiedBox
            , Rectangle layoutBox, bool reverse);

        protected internal abstract iText.Layout.Renderer.TableBorders ApplyLeftAndRightTableBorder(Rectangle layoutBox
            , bool reverse);

        protected internal abstract iText.Layout.Renderer.TableBorders SkipFooter(Border[] borders);

        protected internal abstract iText.Layout.Renderer.TableBorders SkipHeader(Border[] borders);

        protected internal abstract iText.Layout.Renderer.TableBorders CollapseTableWithFooter(iText.Layout.Renderer.TableBorders
             footerBordersHandler, bool hasContent);

        protected internal abstract iText.Layout.Renderer.TableBorders CollapseTableWithHeader(iText.Layout.Renderer.TableBorders
             headerBordersHandler, bool updateBordersHandler);

        protected internal abstract iText.Layout.Renderer.TableBorders FixHeaderOccupiedArea(Rectangle occupiedBox
            , Rectangle layoutBox);

        protected internal abstract iText.Layout.Renderer.TableBorders ApplyCellIndents(Rectangle box, float topIndent
            , float rightIndent, float bottomIndent, float leftIndent, bool reverse);

        // endregion
        // region getters
        public abstract IList<Border> GetVerticalBorder(int index);

        public abstract IList<Border> GetHorizontalBorder(int index);

        protected internal abstract float GetCellVerticalAddition(float[] indents);

        // endregion
        protected internal abstract void BuildBordersArrays(CellRenderer cell, int row, int col);

        protected internal abstract iText.Layout.Renderer.TableBorders UpdateBordersOnNewPage(bool isOriginalNonSplitRenderer
            , bool isFooterOrHeader, TableRenderer currentRenderer, TableRenderer headerRenderer, TableRenderer footerRenderer
            );

        // endregion
        protected internal virtual iText.Layout.Renderer.TableBorders ProcessAllBordersAndEmptyRows() {
            CellRenderer[] currentRow;
            int numOfRowsToRemove = 0;
            if (!rows.IsEmpty()) {
                for (int row = startRow - largeTableIndexOffset; row <= finishRow - largeTableIndexOffset; row++) {
                    currentRow = rows[row];
                    bool hasCells = false;
                    for (int col = 0; col < numberOfColumns; col++) {
                        if (null != currentRow[col]) {
                            if (0 != numOfRowsToRemove) {
                                // Decrease rowspans if necessary
                                UpdateRowspanForNextNonEmptyCellInEachColumn(numOfRowsToRemove, row);
                                // Remove empty rows
                                RemoveRows(row - numOfRowsToRemove, numOfRowsToRemove);
                                row -= numOfRowsToRemove;
                                numOfRowsToRemove = 0;
                            }
                            BuildBordersArrays(currentRow[col], row, col);
                            hasCells = true;
                            int colspan = (int)currentRow[col].GetPropertyAsInteger(Property.COLSPAN);
                            col += colspan - 1;
                        }
                        else {
                            if (horizontalBorders[row].Count <= col) {
                                horizontalBorders[row].Add(null);
                            }
                        }
                    }
                    if (!hasCells) {
                        if (row == rows.Count - 1) {
                            RemoveRows(row - numOfRowsToRemove, numOfRowsToRemove);
                            // delete current row
                            rows.JRemoveAt(row - numOfRowsToRemove);
                            SetFinishRow(finishRow - 1);
                            ILogger logger = ITextLogManager.GetLogger(typeof(TableRenderer));
                            logger.LogWarning(iText.IO.Logs.IoLogMessageConstant.LAST_ROW_IS_NOT_COMPLETE);
                        }
                        else {
                            numOfRowsToRemove++;
                        }
                    }
                }
            }
            if (finishRow < startRow) {
                SetFinishRow(startRow);
            }
            return this;
        }

        // region init
        protected internal virtual iText.Layout.Renderer.TableBorders InitializeBorders() {
            IList<Border> tempBorders;
            // initialize vertical borders
            while (numberOfColumns + 1 > verticalBorders.Count) {
                tempBorders = new List<Border>();
                while ((int)Math.Max(rows.Count, 1) > tempBorders.Count) {
                    tempBorders.Add(null);
                }
                verticalBorders.Add(tempBorders);
            }
            // initialize horizontal borders
            while ((int)Math.Max(rows.Count, 1) + 1 > horizontalBorders.Count) {
                tempBorders = new List<Border>();
                while (numberOfColumns > tempBorders.Count) {
                    tempBorders.Add(null);
                }
                horizontalBorders.Add(tempBorders);
            }
            return this;
        }

        // endregion
        // region setters
        protected internal virtual iText.Layout.Renderer.TableBorders SetTableBoundingBorders(Border[] borders) {
            tableBoundingBorders = new Border[4];
            if (null != borders) {
                for (int i = 0; i < borders.Length; i++) {
                    tableBoundingBorders[i] = borders[i];
                }
            }
            return this;
        }

        protected internal virtual iText.Layout.Renderer.TableBorders SetRowRange(int startRow, int finishRow) {
            this.startRow = startRow;
            this.finishRow = finishRow;
            return this;
        }

        protected internal virtual iText.Layout.Renderer.TableBorders SetStartRow(int row) {
            this.startRow = row;
            return this;
        }

        protected internal virtual iText.Layout.Renderer.TableBorders SetFinishRow(int row) {
            this.finishRow = row;
            return this;
        }

        // endregion
        // region getters
        public virtual float GetLeftBorderMaxWidth() {
            return leftBorderMaxWidth;
        }

        public virtual float GetRightBorderMaxWidth() {
            return rightBorderMaxWidth;
        }

        public virtual float GetMaxTopWidth() {
            float width = 0;
            Border widestBorder = GetWidestHorizontalBorder(startRow);
            if (null != widestBorder && widestBorder.GetWidth() >= width) {
                width = widestBorder.GetWidth();
            }
            return width;
        }

        public virtual float GetMaxBottomWidth() {
            float width = 0;
            Border widestBorder = GetWidestHorizontalBorder(finishRow + 1);
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

        public virtual Border GetWidestVerticalBorder(int col) {
            return TableBorderUtil.GetWidestBorder(GetVerticalBorder(col));
        }

        public virtual Border GetWidestVerticalBorder(int col, int start, int end) {
            return TableBorderUtil.GetWidestBorder(GetVerticalBorder(col), start, end);
        }

        public virtual Border GetWidestHorizontalBorder(int row) {
            return TableBorderUtil.GetWidestBorder(GetHorizontalBorder(row));
        }

        public virtual Border GetWidestHorizontalBorder(int row, int start, int end) {
            return TableBorderUtil.GetWidestBorder(GetHorizontalBorder(row), start, end);
        }

        public virtual IList<Border> GetFirstHorizontalBorder() {
            return GetHorizontalBorder(startRow);
        }

        public virtual IList<Border> GetLastHorizontalBorder() {
            return GetHorizontalBorder(finishRow + 1);
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

        public virtual int GetStartRow() {
            return startRow;
        }

        public virtual int GetFinishRow() {
            return finishRow;
        }

        public virtual Border[] GetTableBoundingBorders() {
            return tableBoundingBorders;
        }

        public virtual float[] GetCellBorderIndents(int row, int col, int rowspan, int colspan) {
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
            for (int i = startRow + row - rowspan + 1; i < startRow + row + 1; i++) {
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
            for (int i = startRow + row - rowspan + 1; i < startRow + row + 1; i++) {
                border = borderList[i];
                if (null != border && border.GetWidth() > indents[3]) {
                    indents[3] = border.GetWidth();
                }
            }
            return indents;
        }

        // endregion
        private void RemoveRows(int startRow, int numOfRows) {
            for (int row = startRow; row < startRow + numOfRows; row++) {
                rows.JRemoveAt(startRow);
                horizontalBorders.JRemoveAt(startRow + 1);
                for (int j = 0; j <= numberOfColumns; j++) {
                    verticalBorders[j].JRemoveAt(startRow + 1);
                }
            }
            SetFinishRow(finishRow - numOfRows);
        }

        private void UpdateRowspanForNextNonEmptyCellInEachColumn(int numOfRowsToRemove, int row) {
            // We go by columns in a current row which is not empty. For each column we look for
            // a non-empty cell going up by rows (going down in a table). For each such cell we
            // collect data to be able to analyze its rowspan.
            // Iterate by columns
            int c = 0;
            while (c < numberOfColumns) {
                int r = row;
                CellRenderer[] cr = null;
                // Look for non-empty cell in a column
                while (r < rows.Count && (cr == null || cr[c] == null)) {
                    cr = rows[r];
                    ++r;
                }
                // Found a cell
                if (cr != null && cr[c] != null) {
                    CellRenderer cell = cr[c];
                    int origRowspan = (int)cell.GetPropertyAsInteger(Property.ROWSPAN);
                    int spansToRestore = 0;
                    // Here we analyze whether current cell's rowspan touches a non-empty row before
                    // numOfRowsToRemove. If it doesn't touch it we will need to 'restore' a few
                    // rowspans which is a difference between the current (non-empty) row and the row
                    // where we found non-empty cell for this column
                    if (row - numOfRowsToRemove < r - origRowspan) {
                        spansToRestore = r - row - 1;
                    }
                    int rowspan = origRowspan;
                    rowspan = rowspan - numOfRowsToRemove;
                    if (rowspan < 1) {
                        rowspan = 1;
                    }
                    rowspan += spansToRestore;
                    rowspan = Math.Min(rowspan, origRowspan);
                    cell.SetProperty(Property.ROWSPAN, rowspan);
                    int colspan = (int)cell.GetPropertyAsInteger(Property.COLSPAN);
                    c += colspan;
                }
                else {
                    ++c;
                }
            }
        }
    }
//\endcond
}
