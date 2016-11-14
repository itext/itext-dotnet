/*

This file is part of the iText (R) project.
Copyright (c) 1998-2016 iText Group NV
Authors: Bruno Lowagie, Paulo Soares, et al.

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
using iText.IO.Log;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Tagutils;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Layout;
using iText.Layout.Properties;

namespace iText.Layout.Renderer {
    /// <summary>
    /// This class represents the
    /// <see cref="IRenderer">renderer</see>
    /// object for a
    /// <see cref="iText.Layout.Element.Table"/>
    /// object. It will delegate its drawing operations on to the
    /// <see cref="CellRenderer"/>
    /// instances associated with the
    /// <see cref="iText.Layout.Element.Cell">table cells</see>
    /// .
    /// </summary>
    public class TableRenderer : AbstractRenderer {
        protected internal IList<CellRenderer[]> rows = new List<CellRenderer[]>();

        protected internal Table.RowRange rowRange;

        protected internal iText.Layout.Renderer.TableRenderer headerRenderer;

        protected internal iText.Layout.Renderer.TableRenderer footerRenderer;

        /// <summary>True for newly created renderer.</summary>
        /// <remarks>True for newly created renderer. For split renderers this is set to false. Used for tricky layout.
        ///     </remarks>
        protected internal bool isOriginalNonSplitRenderer = true;

        private List<List<Border>> horizontalBorders;

        private List<List<Border>> verticalBorders;

        private float[] columnWidths = null;

        private IList<float> heights = new List<float>();

        private TableRenderer() {
        }

        /// <summary>
        /// Creates a TableRenderer from a
        /// <see cref="iText.Layout.Element.Table"/>
        /// which will partially render
        /// the table.
        /// </summary>
        /// <param name="modelElement">the table to be rendered by this renderer</param>
        /// <param name="rowRange">the table rows to be rendered</param>
        public TableRenderer(Table modelElement, Table.RowRange rowRange)
            : base(modelElement) {
            // Row range of the current renderer. For large tables it may contain only a few rows.
            SetRowRange(rowRange);
        }

        /// <summary>
        /// Creates a TableRenderer from a
        /// <see cref="iText.Layout.Element.Table"/>
        /// .
        /// </summary>
        /// <param name="modelElement">the table to be rendered by this renderer</param>
        public TableRenderer(Table modelElement)
            : this(modelElement, new Table.RowRange(0, modelElement.GetNumberOfRows() - 1)) {
        }

        /// <summary><inheritDoc/></summary>
        public override void AddChild(IRenderer renderer) {
            if (renderer is CellRenderer) {
                // In case rowspan or colspan save cell into bottom left corner.
                // In in this case it will be easier handle row heights in case rowspan.
                Cell cell = (Cell)renderer.GetModelElement();
                rows[cell.GetRow() - rowRange.GetStartRow() + cell.GetRowspan() - 1][cell.GetCol()] = (CellRenderer)renderer;
            }
            else {
                ILogger logger = LoggerFactory.GetLogger(typeof(iText.Layout.Renderer.TableRenderer));
                logger.Error("Only BlockRenderer with Cell layout element could be added");
            }
        }

        protected internal override Rectangle ApplyBorderBox(Rectangle rect, Border[] borders, bool reverse) {
            // Do nothing here. Applying border box for tables is indeed difficult operation and is done on #layout()
            return rect;
        }

        /// <summary><inheritDoc/></summary>
        public override LayoutResult Layout(LayoutContext layoutContext) {
            LayoutArea area = layoutContext.GetArea();
            Rectangle layoutBox = area.GetBBox().Clone();
            if (!((Table)modelElement).IsComplete()) {
                SetProperty(Property.MARGIN_BOTTOM, 0);
            }
            if (rowRange.GetStartRow() != 0) {
                SetProperty(Property.MARGIN_TOP, 0);
            }
            // we can invoke #layout() twice (processing KEEP_TOGETHER for instance)
            // so we need to clear the results of previous #layout() invocation
            heights.Clear();
            childRenderers.Clear();
            // Cells' up moves occured while split processing
            // key is column number (there can be only one move during one split)
            // value is the previous row number of the cell
            IDictionary<int, int?> rowMoves = new Dictionary<int, int?>();
            ApplyMargins(layoutBox, false);
            Border[] borders;
            float leftTableBorderWidth = -1;
            float rightTableBorderWidth = -1;
            float topTableBorderWidth = -1;
            float bottomTableBorderWidth = 0;
            // Find left, right and top collapsed borders widths.
            // In order to find left and right border widths we try to consider as few rows ad possible
            // i.e. the borders still can be drawn outside the layout area.
            int row = 0;
            while (row < rows.Count && (-1 == leftTableBorderWidth || -1 == rightTableBorderWidth)) {
                CellRenderer[] currentRow = rows[row];
                if (0 == row) {
                    for (int i = 0; i < currentRow.Length; i++) {
                        if (null != currentRow[i]) {
                            borders = currentRow[i].GetBorders();
                            topTableBorderWidth = Math.Max(null == borders[0] ? -1 : borders[0].GetWidth(), topTableBorderWidth);
                        }
                    }
                }
                if (0 != currentRow.Length) {
                    if (null != currentRow[0]) {
                        borders = currentRow[0].GetBorders();
                        leftTableBorderWidth = Math.Max(null == borders[3] ? -1 : borders[3].GetWidth(), leftTableBorderWidth);
                    }
                    // the last cell in a row can have big rowspan
                    int lastInRow = currentRow.Length - 1;
                    while (lastInRow >= 0 && null == currentRow[lastInRow]) {
                        lastInRow--;
                    }
                    if (lastInRow >= 0 && currentRow.Length == lastInRow + currentRow[lastInRow].GetPropertyAsInteger(Property
                        .ROWSPAN)) {
                        borders = currentRow[lastInRow].GetBorders();
                        rightTableBorderWidth = Math.Max(null == borders[1] ? -1 : borders[1].GetWidth(), rightTableBorderWidth);
                    }
                }
                row++;
            }
            // collapse with table borders
            borders = GetBorders();
            leftTableBorderWidth = Math.Max(null == borders[3] ? 0 : borders[3].GetWidth(), leftTableBorderWidth);
            rightTableBorderWidth = Math.Max(null == borders[1] ? 0 : borders[1].GetWidth(), rightTableBorderWidth);
            topTableBorderWidth = Math.Max(null == borders[0] ? 0 : borders[0].GetWidth(), topTableBorderWidth);
            if (IsPositioned()) {
                float x = (float)this.GetPropertyAsFloat(Property.X);
                float relativeX = IsFixedLayout() ? 0 : layoutBox.GetX();
                layoutBox.SetX(relativeX + x);
            }
            Table tableModel = (Table)GetModelElement();
            float? tableWidth = RetrieveWidth(layoutBox.GetWidth());
            if (tableWidth == null || tableWidth == 0) {
                tableWidth = layoutBox.GetWidth();
            }
            // Float blockHeight = retrieveHeight();
            float? blockMaxHeight = RetrieveMaxHeight();
            if (null != blockMaxHeight && blockMaxHeight < layoutBox.GetHeight() && !true.Equals(GetPropertyAsBoolean(
                Property.FORCED_PLACEMENT))) {
                layoutBox.MoveUp(layoutBox.GetHeight() - blockMaxHeight).SetHeight(blockMaxHeight);
            }
            float layoutBoxHeight = layoutBox.GetHeight();
            occupiedArea = new LayoutArea(area.GetPageNumber(), new Rectangle(layoutBox.GetX(), layoutBox.GetY() + layoutBox
                .GetHeight() - topTableBorderWidth / 2, (float)tableWidth, 0));
            int numberOfColumns = ((Table)GetModelElement()).GetNumberOfColumns();
            horizontalBorders = new List<List<Border>>();
            verticalBorders = new List<List<Border>>();
            Table headerElement = tableModel.GetHeader();
            bool isFirstHeader = rowRange.GetStartRow() == 0 && isOriginalNonSplitRenderer;
            bool headerShouldBeApplied = !rows.IsEmpty() && (!isOriginalNonSplitRenderer || isFirstHeader && !tableModel
                .IsSkipFirstHeader());
            if (headerElement != null && headerShouldBeApplied) {
                headerElement.SetBorderTop(borders[0]);
                headerElement.SetBorderRight(borders[1]);
                headerElement.SetBorderBottom(borders[2]);
                headerElement.SetBorderLeft(borders[3]);
                headerRenderer = (iText.Layout.Renderer.TableRenderer)headerElement.CreateRendererSubTree().SetParent(this
                    );
                LayoutResult result = headerRenderer.Layout(new LayoutContext(new LayoutArea(area.GetPageNumber(), layoutBox
                    )));
                if (result.GetStatus() != LayoutResult.FULL) {
                    return new LayoutResult(LayoutResult.NOTHING, null, null, this, result.GetCauseOfNothing());
                }
                float headerHeight = result.GetOccupiedArea().GetBBox().GetHeight();
                layoutBox.DecreaseHeight(headerHeight);
                occupiedArea.GetBBox().MoveDown(headerHeight).IncreaseHeight(headerHeight);
            }
            Table footerElement = tableModel.GetFooter();
            if (footerElement != null) {
                footerElement.SetBorderTop(borders[0]);
                footerElement.SetBorderRight(borders[1]);
                footerElement.SetBorderBottom(borders[2]);
                footerElement.SetBorderLeft(borders[3]);
                footerRenderer = (iText.Layout.Renderer.TableRenderer)footerElement.CreateRendererSubTree().SetParent(this
                    );
                LayoutResult result = footerRenderer.Layout(new LayoutContext(new LayoutArea(area.GetPageNumber(), layoutBox
                    )));
                if (result.GetStatus() != LayoutResult.FULL) {
                    return new LayoutResult(LayoutResult.NOTHING, null, null, this, result.GetCauseOfNothing());
                }
                float footerHeight = result.GetOccupiedArea().GetBBox().GetHeight();
                footerRenderer.Move(0, -(layoutBox.GetHeight() - footerHeight));
                layoutBox.MoveUp(footerHeight).DecreaseHeight(footerHeight);
            }
            // Apply halves of the borders. The other halves are applied on a Cell level
            layoutBox.ApplyMargins<Rectangle>(topTableBorderWidth / 2, rightTableBorderWidth / 2, 0, leftTableBorderWidth
                 / 2, false);
            columnWidths = CalculateScaledColumnWidths(tableModel, (float)tableWidth, leftTableBorderWidth, rightTableBorderWidth
                );
            LayoutResult[] splits = new LayoutResult[tableModel.GetNumberOfColumns()];
            // This represents the target row index for the overflow renderer to be placed to.
            // Usually this is just the current row id of a cell, but it has valuable meaning when a cell has rowspan.
            int[] targetOverflowRowIndex = new int[tableModel.GetNumberOfColumns()];
            // complete table with empty cells
            CellRenderer[] lastAddedRow;
            if (0 != rows.Count && null != rows[rows.Count - 1]) {
                lastAddedRow = rows[rows.Count - 1];
                int colIndex = 0;
                while (colIndex < lastAddedRow.Length && null != lastAddedRow[colIndex]) {
                    colIndex += (int)lastAddedRow[colIndex].GetPropertyAsInteger(Property.COLSPAN);
                }
                // complete row if it's not already complete ot totally empty
                if (0 != colIndex && lastAddedRow.Length != colIndex) {
                    while (colIndex < lastAddedRow.Length) {
                        Cell emptyCell = new Cell();
                        emptyCell.SetBorder(Border.NO_BORDER);
                        ((Table)this.GetModelElement()).AddCell(emptyCell);
                        this.AddChild(emptyCell.GetRenderer());
                        colIndex++;
                    }
                }
            }
            horizontalBorders.Add(tableModel.GetLastRowBottomBorder());
            for (row = 0; row < rows.Count; row++) {
                // if forced placement was earlier set, this means the element did not fit into the area, and in this case
                // we only want to place the first row in a forced way, not the next ones, otherwise they will be invisible
                if (row == 1 && true.Equals(this.GetOwnProperty<bool?>(Property.FORCED_PLACEMENT))) {
                    DeleteOwnProperty(Property.FORCED_PLACEMENT);
                }
                // the width of the widest bottom border of the row
                bottomTableBorderWidth = 0;
                CellRenderer[] currentRow = rows[row];
                float rowHeight = 0;
                bool split = false;
                // Indicates that all the cells fit (at least partially after splitting if not forbidden by keepTogether) in the current row.
                bool hasContent = true;
                // Indicates that we have added a cell from the future, i.e. a cell which has a big rowspan and we shouldn't have
                // added it yet, because we add a cell with rowspan only during the processing of the very last row this cell occupied,
                // but now we have area break and we had to force that cell addition.
                bool cellWithBigRowspanAdded = false;
                IList<CellRenderer> currChildRenderers = new List<CellRenderer>();
                // Process in a queue, because we might need to add a cell from the future, i.e. having big rowspan in case of split.
                LinkedList<TableRenderer.CellRendererInfo> cellProcessingQueue = new LinkedList<TableRenderer.CellRendererInfo
                    >();
                for (int col = 0; col < currentRow.Length; col++) {
                    if (currentRow[col] != null) {
                        cellProcessingQueue.AddLast(new TableRenderer.CellRendererInfo(currentRow[col], col, row));
                    }
                }
                // the element which was the first to cause Layout.Nothing
                IRenderer firstCauseOfNothing = null;
                while (cellProcessingQueue.Count > 0) {
                    TableRenderer.CellRendererInfo currentCellInfo = cellProcessingQueue.JRemoveFirst();
                    int col_1 = currentCellInfo.column;
                    CellRenderer cell = currentCellInfo.cellRenderer;
                    int colspan = (int)cell.GetPropertyAsInteger(Property.COLSPAN);
                    int rowspan = (int)cell.GetPropertyAsInteger(Property.ROWSPAN);
                    // collapse boundary borders if necessary
                    // notice that bottom border collapse is handled afterwards
                    Border[] cellBorders = cell.GetBorders();
                    if (0 == row - rowspan + 1) {
                        cell.SetProperty(Property.BORDER_TOP, GetCollapsedBorder(cellBorders[0], borders[0]));
                    }
                    if (0 == col_1) {
                        cell.SetProperty(Property.BORDER_LEFT, GetCollapsedBorder(cellBorders[3], borders[3]));
                    }
                    if (tableModel.GetNumberOfColumns() == col_1 + colspan) {
                        cell.SetProperty(Property.BORDER_RIGHT, GetCollapsedBorder(cellBorders[1], borders[1]));
                    }
                    if (cell != null) {
                        BuildBordersArrays(cell, row, true);
                    }
                    if (row + 1 < rows.Count) {
                        for (int j = 0; j < ((Cell)cell.GetModelElement()).GetColspan(); j++) {
                            CellRenderer nextCell = rows[row + 1][col_1 + j];
                            if (nextCell != null) {
                                BuildBordersArrays(nextCell, row + 1, true);
                            }
                        }
                    }
                    if (col_1 + 1 < rows[row].Length) {
                        CellRenderer nextCell = rows[row][col_1 + 1];
                        if (nextCell != null) {
                            BuildBordersArrays(nextCell, row, true);
                        }
                    }
                    targetOverflowRowIndex[col_1] = currentCellInfo.finishRowInd;
                    // This cell came from the future (split occurred and we need to place cell with big rowpsan into the current area)
                    bool currentCellHasBigRowspan = (row != currentCellInfo.finishRowInd);
                    float cellWidth = 0;
                    float colOffset = 0;
                    for (int i = col_1; i < col_1 + colspan; i++) {
                        cellWidth += columnWidths[i];
                    }
                    for (int i_1 = 0; i_1 < col_1; i_1++) {
                        colOffset += columnWidths[i_1];
                    }
                    float rowspanOffset = 0;
                    for (int i_2 = row - 1; i_2 > currentCellInfo.finishRowInd - rowspan && i_2 >= 0; i_2--) {
                        rowspanOffset += (float)heights[i_2];
                    }
                    float cellLayoutBoxHeight = rowspanOffset + (!currentCellHasBigRowspan || hasContent ? layoutBox.GetHeight
                        () : 0);
                    float cellLayoutBoxBottom = layoutBox.GetY() + (!currentCellHasBigRowspan || hasContent ? 0 : layoutBox.GetHeight
                        ());
                    Rectangle cellLayoutBox = new Rectangle(layoutBox.GetX() + colOffset, cellLayoutBoxBottom, cellWidth, cellLayoutBoxHeight
                        );
                    LayoutArea cellArea = new LayoutArea(layoutContext.GetArea().GetPageNumber(), cellLayoutBox);
                    VerticalAlignment? verticalAlignment = cell.GetProperty<VerticalAlignment?>(Property.VERTICAL_ALIGNMENT);
                    cell.SetProperty(Property.VERTICAL_ALIGNMENT, null);
                    // Increase bottom borders widths up to the table's if necessary to perform #layout() correctly
                    Border oldBottomBorder = cell.GetBorders()[2];
                    Border collapsedBottomBorder = GetCollapsedBorder(oldBottomBorder, borders[2]);
                    if (collapsedBottomBorder != null) {
                        bottomTableBorderWidth = Math.Max(bottomTableBorderWidth, collapsedBottomBorder.GetWidth());
                        cellArea.GetBBox().ApplyMargins<Rectangle>(0, 0, collapsedBottomBorder.GetWidth() / 2, 0, false);
                        cell.SetProperty(Property.BORDER_BOTTOM, collapsedBottomBorder);
                    }
                    LayoutResult cellResult = cell.SetParent(this).Layout(new LayoutContext(cellArea));
                    if (collapsedBottomBorder != null && null != cellResult.GetOccupiedArea()) {
                        // apply the difference between collapsed table border and own cell border
                        cellResult.GetOccupiedArea().GetBBox().ApplyMargins<Rectangle>(0, 0, (collapsedBottomBorder.GetWidth() - (
                            oldBottomBorder == null ? 0 : oldBottomBorder.GetWidth())) / 2, 0, false);
                        cell.SetProperty(Property.BORDER_BOTTOM, oldBottomBorder);
                    }
                    cell.SetProperty(Property.VERTICAL_ALIGNMENT, verticalAlignment);
                    // width of BlockRenderer depends on child areas, while in cell case it is hardly define.
                    if (cellResult.GetStatus() != LayoutResult.NOTHING) {
                        cell.GetOccupiedArea().GetBBox().SetWidth(cellWidth);
                    }
                    else {
                        if (null == firstCauseOfNothing) {
                            firstCauseOfNothing = cellResult.GetCauseOfNothing();
                        }
                    }
                    if (currentCellHasBigRowspan) {
                        // cell from the future
                        if (cellResult.GetStatus() == LayoutResult.PARTIAL) {
                            splits[col_1] = cellResult;
                            currentRow[col_1] = (CellRenderer)cellResult.GetSplitRenderer();
                        }
                        else {
                            rows[currentCellInfo.finishRowInd][col_1] = null;
                            currentRow[col_1] = cell;
                            rowMoves[col_1] = currentCellInfo.finishRowInd;
                        }
                    }
                    else {
                        if (cellResult.GetStatus() != LayoutResult.FULL) {
                            // first time split occurs
                            if (!split) {
                                // This is a case when last footer should be skipped and we might face an end of the table.
                                // We check if we can fit all the rows right now and the split occurred only because we reserved
                                // space for footer before, and if yes we skip footer and write all the content right now.
                                if (footerRenderer != null && tableModel.IsSkipLastFooter() && tableModel.IsComplete()) {
                                    LayoutArea potentialArea = new LayoutArea(area.GetPageNumber(), layoutBox.Clone());
                                    float footerHeight = footerRenderer.GetOccupiedArea().GetBBox().GetHeight();
                                    potentialArea.GetBBox().MoveDown(footerHeight).IncreaseHeight(footerHeight);
                                    if (CanFitRowsInGivenArea(potentialArea, row, columnWidths, heights)) {
                                        layoutBox.IncreaseHeight(footerHeight).MoveDown(footerHeight);
                                        cellProcessingQueue.Clear();
                                        for (int addCol = 0; addCol < currentRow.Length; addCol++) {
                                            if (currentRow[addCol] != null) {
                                                cellProcessingQueue.AddLast(new TableRenderer.CellRendererInfo(currentRow[addCol], addCol, row));
                                            }
                                        }
                                        footerRenderer = null;
                                        continue;
                                    }
                                }
                                // Here we look for a cell with big rowpsan (i.e. one which would not be normally processed in
                                // the scope of this row), and we add such cells to the queue, because we need to write them
                                // at least partially into the available area we have.
                                for (int addCol_1 = 0; addCol_1 < currentRow.Length; addCol_1++) {
                                    if (currentRow[addCol_1] == null) {
                                        // Search for the next cell including rowspan.
                                        for (int addRow = row + 1; addRow < rows.Count; addRow++) {
                                            if (rows[addRow][addCol_1] != null) {
                                                CellRenderer addRenderer = rows[addRow][addCol_1];
                                                verticalAlignment = addRenderer.GetProperty<VerticalAlignment?>(Property.VERTICAL_ALIGNMENT);
                                                if (verticalAlignment != null && verticalAlignment.Equals(VerticalAlignment.BOTTOM)) {
                                                    if (row + addRenderer.GetPropertyAsInteger(Property.ROWSPAN) - 1 < addRow) {
                                                        cellProcessingQueue.AddLast(new TableRenderer.CellRendererInfo(addRenderer, addCol_1, addRow));
                                                        cellWithBigRowspanAdded = true;
                                                    }
                                                    else {
                                                        horizontalBorders[row + 1][addCol_1] = addRenderer.GetBorders()[2];
                                                        if (addCol_1 == 0) {
                                                            for (int i_3 = row; i_3 >= 0; i_3--) {
                                                                if (!CheckAndReplaceBorderInArray(verticalBorders, addCol_1, i_3, addRenderer.GetBorders()[3])) {
                                                                    break;
                                                                }
                                                            }
                                                        }
                                                        else {
                                                            if (addCol_1 == numberOfColumns - 1) {
                                                                for (int i_3 = row; i_3 >= 0; i_3--) {
                                                                    if (!CheckAndReplaceBorderInArray(verticalBorders, addCol_1 + 1, i_3, addRenderer.GetBorders()[1])) {
                                                                        break;
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                                else {
                                                    if (row + addRenderer.GetPropertyAsInteger(Property.ROWSPAN) - 1 >= addRow) {
                                                        cellProcessingQueue.AddLast(new TableRenderer.CellRendererInfo(addRenderer, addCol_1, addRow));
                                                        cellWithBigRowspanAdded = true;
                                                    }
                                                }
                                                break;
                                            }
                                        }
                                    }
                                    else {
                                        // if cell in current row has big rowspan
                                        // we need to process it specially too,
                                        // because some problems (for instance, borders related) can occur
                                        if (((Cell)cell.GetModelElement()).GetRowspan() > 1) {
                                            cellWithBigRowspanAdded = true;
                                        }
                                    }
                                }
                            }
                            split = true;
                            if (cellResult.GetStatus() == LayoutResult.NOTHING) {
                                hasContent = false;
                            }
                            splits[col_1] = cellResult;
                        }
                    }
                    currChildRenderers.Add(cell);
                    if (cellResult.GetStatus() != LayoutResult.NOTHING) {
                        rowHeight = Math.Max(rowHeight, cell.GetOccupiedArea().GetBBox().GetHeight() - rowspanOffset);
                    }
                }
                if (hasContent || cellWithBigRowspanAdded) {
                    heights.Add(rowHeight);
                    occupiedArea.GetBBox().MoveDown(rowHeight);
                    occupiedArea.GetBBox().IncreaseHeight(rowHeight);
                    layoutBox.DecreaseHeight(rowHeight);
                }
                if (split || row == rows.Count - 1) {
                    // Correct layout area of the last row rendered on the page
                    if (heights.Count != 0) {
                        float bottomBorderWidthDifference = 0;
                        if (hasContent || cellWithBigRowspanAdded) {
                            lastAddedRow = currentRow;
                        }
                        else {
                            lastAddedRow = rows[row - 1];
                        }
                        float currentCellHeight;
                        for (int col_1 = 0; col_1 < lastAddedRow.Length; col_1++) {
                            if (null != lastAddedRow[col_1]) {
                                currentCellHeight = 0;
                                Border cellBottomBorder = lastAddedRow[col_1].GetBorders()[2];
                                Border collapsedBorder = GetCollapsedBorder(cellBottomBorder, borders[2]);
                                float cellBottomBorderWidth = null == cellBottomBorder ? 0 : cellBottomBorder.GetWidth();
                                float collapsedBorderWidth = null == collapsedBorder ? 0 : collapsedBorder.GetWidth();
                                if (cellBottomBorderWidth < collapsedBorderWidth) {
                                    lastAddedRow[col_1].SetProperty(Property.BORDER_BOTTOM, collapsedBorder);
                                    horizontalBorders[hasContent || cellWithBigRowspanAdded ? row + 1 : row][col_1] = collapsedBorder;
                                }
                                // apply the difference between collapsed table border and own cell border
                                lastAddedRow[col_1].occupiedArea.GetBBox().ApplyMargins<Rectangle>(0, 0, (collapsedBorderWidth - cellBottomBorderWidth
                                    ) / 2, 0, true);
                                int cellRowStartIndex = heights.Count - (int)lastAddedRow[col_1].GetPropertyAsInteger(Property.ROWSPAN);
                                for (int i = cellRowStartIndex > 0 ? cellRowStartIndex : 0; i < heights.Count; i++) {
                                    currentCellHeight += heights[i];
                                }
                                if (currentCellHeight < lastAddedRow[col_1].occupiedArea.GetBBox().GetHeight()) {
                                    bottomBorderWidthDifference = Math.Max(bottomBorderWidthDifference, (collapsedBorderWidth - cellBottomBorderWidth
                                        ) / 2);
                                }
                            }
                        }
                        heights[heights.Count - 1] = heights[heights.Count - 1] + bottomBorderWidthDifference;
                    }
                    // Correct occupied areas of all added cells
                    for (int k = 0; k <= row; k++) {
                        currentRow = rows[k];
                        if (k < row || (k == row && (hasContent || cellWithBigRowspanAdded))) {
                            for (int col_1 = 0; col_1 < currentRow.Length; col_1++) {
                                CellRenderer cell = currentRow[col_1];
                                if (cell == null) {
                                    continue;
                                }
                                float height = 0;
                                int rowspan = (int)cell.GetPropertyAsInteger(Property.ROWSPAN);
                                for (int i = k; i > ((k == row + 1) ? targetOverflowRowIndex[col_1] : k) - rowspan && i >= 0; i--) {
                                    height += (float)heights[i];
                                }
                                int rowN = k + 1;
                                if (k == row && !hasContent) {
                                    rowN--;
                                }
                                if (horizontalBorders[rowN][col_1] == null) {
                                    horizontalBorders[rowN][col_1] = cell.GetBorders()[2];
                                }
                                // Correcting cell bbox only. We don't need #move() here.
                                // This is because of BlockRenderer's specificity regarding occupied area.
                                float shift = height - cell.GetOccupiedArea().GetBBox().GetHeight();
                                Rectangle bBox = cell.GetOccupiedArea().GetBBox();
                                bBox.MoveDown(shift);
                                bBox.SetHeight(height);
                                cell.ApplyVerticalAlignment();
                            }
                        }
                    }
                    currentRow = rows[row];
                }
                if (split) {
                    iText.Layout.Renderer.TableRenderer[] splitResult = Split(row, hasContent);
                    // Apply bottom border
                    splitResult[0].GetOccupiedArea().GetBBox().ApplyMargins<Rectangle>(0, 0, bottomTableBorderWidth / 2, 0, true
                        );
                    int[] rowspans = new int[currentRow.Length];
                    bool[] columnsWithCellToBeEnlarged = new bool[currentRow.Length];
                    for (int col_1 = 0; col_1 < currentRow.Length; col_1++) {
                        if (splits[col_1] != null) {
                            CellRenderer cellSplit = (CellRenderer)splits[col_1].GetSplitRenderer();
                            if (null != cellSplit) {
                                rowspans[col_1] = ((Cell)cellSplit.GetModelElement()).GetRowspan();
                            }
                            if (splits[col_1].GetStatus() != LayoutResult.NOTHING && (hasContent || cellWithBigRowspanAdded)) {
                                childRenderers.Add(cellSplit);
                            }
                            if (hasContent || cellWithBigRowspanAdded || splits[col_1].GetStatus() == LayoutResult.NOTHING) {
                                currentRow[col_1] = null;
                                CellRenderer cellOverflow = (CellRenderer)splits[col_1].GetOverflowRenderer();
                                if (splits[col_1].GetStatus() == LayoutResult.PARTIAL) {
                                    cellOverflow.SetBorders(((Cell)cellOverflow.GetModelElement()).HasProperty(Property.BORDER_BOTTOM) && null
                                         == ((Cell)cellOverflow.GetModelElement()).GetProperty<Border>(Property.BORDER_BOTTOM) ? null : (Border
                                        )((Cell)cellOverflow.GetModelElement()).GetDefaultProperty<Border>(Property.BORDER), 0);
                                }
                                else {
                                    cellOverflow.DeleteOwnProperty(Property.BORDER_TOP);
                                }
                                horizontalBorders[row + 1][col_1] = GetBorders()[2] == null ? ((Cell)cellOverflow.GetModelElement()).HasProperty
                                    (Property.BORDER_BOTTOM) && null == ((Cell)cellOverflow.GetModelElement()).GetProperty<Border>(Property
                                    .BORDER_BOTTOM) ? null : (Border)((Cell)cellOverflow.GetModelElement()).GetDefaultProperty<Border>(Property
                                    .BORDER) : GetBorders()[2];
                                cellOverflow.DeleteOwnProperty(Property.BORDER_BOTTOM);
                                cellOverflow.SetBorders(cellOverflow.GetBorders()[2], 2);
                                rows[targetOverflowRowIndex[col_1]][col_1] = (CellRenderer)cellOverflow.SetParent(splitResult[1]);
                            }
                            else {
                                rows[targetOverflowRowIndex[col_1]][col_1] = (CellRenderer)currentRow[col_1].SetParent(splitResult[1]);
                            }
                        }
                        else {
                            if (hasContent && currentRow[col_1] != null) {
                                columnsWithCellToBeEnlarged[col_1] = true;
                                horizontalBorders[row + 1][col_1] = GetBorders()[2] == null ? ((Cell)currentRow[col_1].GetModelElement()).
                                    HasProperty(Property.BORDER_BOTTOM) && null == ((Cell)currentRow[col_1].GetModelElement()).GetProperty
                                    <Border>(Property.BORDER_BOTTOM) ? null : (Border)((Cell)currentRow[col_1].GetModelElement()).GetDefaultProperty
                                    (Property.BORDER) : GetBorders()[2];
                                // for the future
                                ((Cell)currentRow[col_1].GetModelElement()).SetBorderTop(GetBorders()[0] == null ? ((Cell)currentRow[col_1
                                    ].GetModelElement()).HasProperty(Property.BORDER_BOTTOM) && null == ((Cell)currentRow[col_1].GetModelElement
                                    ()).GetProperty<Border>(Property.BORDER_BOTTOM) ? null : (Border)((Cell)currentRow[col_1].GetModelElement
                                    ()).GetDefaultProperty<Border>(Property.BORDER) : GetBorders()[0]);
                            }
                        }
                    }
                    int minRowspan = int.MaxValue;
                    for (int col_2 = 0; col_2 < rowspans.Length; col_2++) {
                        if (0 != rowspans[col_2]) {
                            minRowspan = Math.Min(minRowspan, rowspans[col_2]);
                        }
                    }
                    for (int col_3 = 0; col_3 < numberOfColumns; col_3++) {
                        if (columnsWithCellToBeEnlarged[col_3]) {
                            if (1 == minRowspan) {
                                // Here we use the same cell, but create a new renderer which doesn't have any children,
                                // therefore it won't have any content.
                                Cell overflowCell = ((Cell)currentRow[col_3].GetModelElement());
                                currentRow[col_3].isLastRendererForModelElement = false;
                                childRenderers.Add(currentRow[col_3]);
                                currentRow[col_3] = null;
                                rows[targetOverflowRowIndex[col_3]][col_3] = (CellRenderer)overflowCell.GetRenderer().SetParent(this);
                            }
                            else {
                                childRenderers.Add(currentRow[col_3]);
                                // shift all cells in the column up
                                int i = row;
                                for (; i < row + minRowspan && i + 1 < rows.Count && rows[i + 1][col_3] != null; i++) {
                                    rows[i][col_3] = rows[i + 1][col_3];
                                    rows[i + 1][col_3] = null;
                                }
                                // the number of cells behind is less then minRowspan-1
                                // so we should process the last cell in the column as in the case 1 == minRowspan
                                if (i != row + minRowspan - 1 && null != rows[i][col_3]) {
                                    Cell overflowCell = ((Cell)rows[i][col_3].GetModelElement());
                                    rows[i][col_3].isLastRendererForModelElement = false;
                                    rows[i][col_3] = null;
                                    rows[targetOverflowRowIndex[col_3]][col_3] = (CellRenderer)overflowCell.GetRenderer().SetParent(this);
                                }
                            }
                        }
                    }
                    if (row == rowRange.GetFinishRow() && footerRenderer != null) {
                        footerRenderer.GetOccupiedAreaBBox().SetY(splitResult[0].GetOccupiedAreaBBox().GetY() - footerRenderer.GetOccupiedAreaBBox
                            ().GetHeight());
                        foreach (IRenderer renderer in footerRenderer.GetChildRenderers()) {
                            renderer.Move(0, splitResult[0].GetOccupiedAreaBBox().GetY() - renderer.GetOccupiedArea().GetBBox().GetY()
                                 - renderer.GetOccupiedArea().GetBBox().GetHeight());
                        }
                    }
                    else {
                        AdjustFooterAndFixOccupiedArea(layoutBox);
                    }
                    // On the next page we need to process rows without any changes except moves connected to actual cell splitting
                    foreach (KeyValuePair<int, int?> entry in rowMoves) {
                        // Move the cell back to its row if there was no actual split
                        if (null == splitResult[1].rows[(int)entry.Value - splitResult[0].rows.Count][entry.Key]) {
                            splitResult[1].rows[(int)entry.Value - splitResult[0].rows.Count][entry.Key] = splitResult[1].rows[row - splitResult
                                [0].rows.Count][entry.Key];
                            splitResult[1].rows[row - splitResult[0].rows.Count][entry.Key] = null;
                        }
                    }
                    if (IsKeepTogether() && !true.Equals(GetPropertyAsBoolean(Property.FORCED_PLACEMENT))) {
                        return new LayoutResult(LayoutResult.NOTHING, occupiedArea, null, this, null == firstCauseOfNothing ? this
                             : firstCauseOfNothing);
                    }
                    else {
                        int status = (childRenderers.IsEmpty() && footerRenderer == null) ? LayoutResult.NOTHING : LayoutResult.PARTIAL;
                        if ((status == LayoutResult.NOTHING && true.Equals(GetPropertyAsBoolean(Property.FORCED_PLACEMENT))) || (null
                             != blockMaxHeight && layoutBoxHeight == blockMaxHeight)) {
                            return new LayoutResult(LayoutResult.FULL, occupiedArea, splitResult[0], null);
                        }
                        else {
                            if (HasProperty(Property.HEIGHT)) {
                                splitResult[1].SetProperty(Property.HEIGHT, RetrieveHeight() - occupiedArea.GetBBox().GetHeight());
                            }
                            return new LayoutResult(status, occupiedArea, splitResult[0], splitResult[1], LayoutResult.NOTHING == status
                                 ? firstCauseOfNothing : null);
                        }
                    }
                }
                else {
                    childRenderers.AddAll(currChildRenderers);
                    currChildRenderers.Clear();
                }
            }
            // if table is empty we still need to process  table borders
            if (0 == childRenderers.Count) {
            }
            //            ArrayList<Border> topHorizontalBorders = new ArrayList<Border>();
            //            ArrayList<Border> bottomHorizontalBorders = new ArrayList<Border>();
            //            ArrayList<Border> leftVerticalBorders = new ArrayList<Border>();
            //            ArrayList<Border> rightVerticalBorders = new ArrayList<Border>();
            //
            //            for (int i = 0; i < ((Table)modelElement).getNumberOfColumns(); i++) {
            //                bottomHorizontalBorders.add(borders[2]);
            //                topHorizontalBorders.add(borders[0]);
            //            }
            //            horizontalBorders.set(0, topHorizontalBorders);
            //            horizontalBorders.add(bottomHorizontalBorders);
            //            leftVerticalBorders.add(borders[0]);
            //            rightVerticalBorders.add(borders[3]);
            //            verticalBorders = new ArrayList<>();
            //            verticalBorders.add(leftVerticalBorders);
            //            for (int i = 1; i < ((Table)modelElement).getNumberOfColumns()-1; i++) {
            //                verticalBorders.add(new ArrayList<Border>());
            //            }
            //            verticalBorders.add(rightVerticalBorders);
            IRenderer overflowRenderer = null;
            float? blockMinHeight = RetrieveMinHeight();
            if (null != blockMinHeight && blockMinHeight > occupiedArea.GetBBox().GetHeight()) {
                float blockBottom = occupiedArea.GetBBox().GetBottom() - ((float)blockMinHeight - occupiedArea.GetBBox().GetHeight
                    ());
                if (blockBottom >= layoutContext.GetArea().GetBBox().GetBottom()) {
                    if (0 != childRenderers.Count) {
                        heights.Add(blockMinHeight - occupiedArea.GetBBox().GetHeight());
                    }
                    else {
                        heights[heights.Count - 1] = blockMinHeight - occupiedArea.GetBBox().GetHeight();
                    }
                    occupiedArea.GetBBox().SetY(blockBottom).SetHeight((float)blockMinHeight);
                }
                else {
                    if (0 != childRenderers.Count) {
                        heights.Add(occupiedArea.GetBBox().GetBottom() - layoutContext.GetArea().GetBBox().GetBottom());
                    }
                    else {
                        heights[heights.Count - 1] = occupiedArea.GetBBox().GetBottom() - layoutContext.GetArea().GetBBox().GetBottom
                            ();
                    }
                    occupiedArea.GetBBox().IncreaseHeight(occupiedArea.GetBBox().GetBottom() - layoutContext.GetArea().GetBBox
                        ().GetBottom()).SetY(layoutContext.GetArea().GetBBox().GetBottom());
                    overflowRenderer = CreateOverflowRenderer(new Table.RowRange(((Table)modelElement).GetNumberOfRows(), ((Table
                        )modelElement).GetNumberOfRows()));
                    overflowRenderer.SetProperty(Property.MIN_HEIGHT, (float)blockMinHeight - occupiedArea.GetBBox().GetHeight
                        ());
                    overflowRenderer.DeleteOwnProperty(Property.HEIGHT);
                    overflowRenderer.DeleteOwnProperty(Property.MAX_HEIGHT);
                }
                if (0 != childRenderers.Count) {
                    CellRenderer[] currentRow = rows[row - 1];
                    verticalBorders[0].Add(row - 1, borders[3]);
                    verticalBorders[currentRow.Length].Add(row - 1, borders[3]);
                    List<Border> lastRowHorizontalBorders = new List<Border>();
                    for (int i = 0; i < currentRow.Length; i++) {
                        if (null != currentRow[i]) {
                            currentRow[i].DeleteOwnProperty(Property.BORDER_BOTTOM);
                            borders = currentRow[i].GetBorders();
                            lastRowHorizontalBorders.Add(borders[2]);
                        }
                    }
                    horizontalBorders.Add(horizontalBorders.Count - 1, lastRowHorizontalBorders);
                }
                else {
                    List<Border> topHorizontalBorders = new List<Border>();
                    List<Border> bottomHorizontalBorders = new List<Border>();
                    List<Border> leftVerticalBorders = new List<Border>();
                    List<Border> rightVerticalBorders = new List<Border>();
                    for (int i = 0; i < ((Table)modelElement).GetNumberOfColumns(); i++) {
                        bottomHorizontalBorders.Add(borders[2]);
                        topHorizontalBorders.Add(borders[0]);
                    }
                    horizontalBorders[0] = topHorizontalBorders;
                    horizontalBorders.Add(bottomHorizontalBorders);
                    leftVerticalBorders.Add(borders[0]);
                    rightVerticalBorders.Add(borders[3]);
                    verticalBorders = new List<List<Border>>();
                    verticalBorders.Add(leftVerticalBorders);
                    for (int i_1 = 0; i_1 < ((Table)modelElement).GetNumberOfColumns() - 1; i_1++) {
                        verticalBorders.Add(new List<Border>());
                    }
                    verticalBorders.Add(rightVerticalBorders);
                }
            }
            if (IsPositioned()) {
                float y = (float)this.GetPropertyAsFloat(Property.Y);
                float relativeY = IsFixedLayout() ? 0 : layoutBox.GetY();
                Move(0, relativeY + y - occupiedArea.GetBBox().GetY());
            }
            // Apply bottom and top border
            ApplyMargins(occupiedArea.GetBBox(), new float[] { topTableBorderWidth / 2, 0, bottomTableBorderWidth / 2, 
                0 }, true);
            ApplyMargins(occupiedArea.GetBBox(), true);
            if (tableModel.IsSkipLastFooter() || !tableModel.IsComplete()) {
                footerRenderer = null;
            }
            AdjustFooterAndFixOccupiedArea(layoutBox);
            if (null == overflowRenderer) {
                return new LayoutResult(LayoutResult.FULL, occupiedArea, null, null);
            }
            else {
                return new LayoutResult(LayoutResult.PARTIAL, occupiedArea, this, overflowRenderer);
            }
        }

        /// <summary><inheritDoc/></summary>
        public override void Draw(DrawContext drawContext) {
            PdfDocument document = drawContext.GetDocument();
            bool isTagged = drawContext.IsTaggingEnabled() && GetModelElement() is IAccessibleElement;
            bool ignoreTag = false;
            PdfName role = null;
            if (isTagged) {
                role = ((IAccessibleElement)GetModelElement()).GetRole();
                bool isHeaderOrFooter = PdfName.THead.Equals(role) || PdfName.TFoot.Equals(role);
                bool ignoreHeaderFooterTag = document.GetTagStructureContext().GetTagStructureTargetVersion().CompareTo(PdfVersion
                    .PDF_1_5) < 0;
                ignoreTag = isHeaderOrFooter && ignoreHeaderFooterTag;
            }
            if (role != null && !role.Equals(PdfName.Artifact) && !ignoreTag) {
                TagStructureContext tagStructureContext = document.GetTagStructureContext();
                TagTreePointer tagPointer = tagStructureContext.GetAutoTaggingPointer();
                IAccessibleElement accessibleElement = (IAccessibleElement)GetModelElement();
                if (!tagStructureContext.IsElementConnectedToTag(accessibleElement)) {
                    AccessibleAttributesApplier.ApplyLayoutAttributes(role, this, document);
                }
                Table modelElement = (Table)GetModelElement();
                tagPointer.AddTag(accessibleElement, true);
                base.Draw(drawContext);
                tagPointer.MoveToParent();
                bool toRemoveConnectionsWithTag = isLastRendererForModelElement && modelElement.IsComplete();
                if (toRemoveConnectionsWithTag) {
                    tagPointer.RemoveElementConnectionToTag(accessibleElement);
                }
            }
            else {
                base.Draw(drawContext);
            }
        }

        /// <summary><inheritDoc/></summary>
        public override void DrawChildren(DrawContext drawContext) {
            Table modelElement = (Table)GetModelElement();
            if (headerRenderer != null) {
                bool firstHeader = rowRange.GetStartRow() == 0 && isOriginalNonSplitRenderer && !modelElement.IsSkipFirstHeader
                    ();
                bool notToTagHeader = drawContext.IsTaggingEnabled() && !firstHeader;
                if (notToTagHeader) {
                    drawContext.SetTaggingEnabled(false);
                    drawContext.GetCanvas().OpenTag(new CanvasArtifact());
                }
                headerRenderer.Draw(drawContext);
                if (notToTagHeader) {
                    drawContext.GetCanvas().CloseTag();
                    drawContext.SetTaggingEnabled(true);
                }
            }
            bool isTagged = drawContext.IsTaggingEnabled() && GetModelElement() is IAccessibleElement && !childRenderers
                .IsEmpty();
            TagTreePointer tagPointer = null;
            bool shouldHaveFooterOrHeaderTag = modelElement.GetHeader() != null || modelElement.GetFooter() != null;
            if (isTagged) {
                PdfName role = modelElement.GetRole();
                if (role != null && !PdfName.Artifact.Equals(role)) {
                    tagPointer = drawContext.GetDocument().GetTagStructureContext().GetAutoTaggingPointer();
                    bool ignoreHeaderFooterTag = drawContext.GetDocument().GetTagStructureContext().GetTagStructureTargetVersion
                        ().CompareTo(PdfVersion.PDF_1_5) < 0;
                    shouldHaveFooterOrHeaderTag = shouldHaveFooterOrHeaderTag && !ignoreHeaderFooterTag && (!modelElement.IsSkipFirstHeader
                        () || !modelElement.IsSkipLastFooter());
                    if (shouldHaveFooterOrHeaderTag) {
                        if (tagPointer.GetKidsRoles().Contains(PdfName.TBody)) {
                            tagPointer.MoveToKid(PdfName.TBody);
                        }
                        else {
                            tagPointer.AddTag(PdfName.TBody);
                        }
                    }
                }
                else {
                    isTagged = false;
                }
            }
            foreach (IRenderer child in childRenderers) {
                if (isTagged) {
                    int adjustByHeaderRowsNum = 0;
                    if (modelElement.GetHeader() != null && !modelElement.IsSkipFirstHeader() && !shouldHaveFooterOrHeaderTag) {
                        adjustByHeaderRowsNum = modelElement.GetHeader().GetNumberOfRows();
                    }
                    int cellRow = ((Cell)child.GetModelElement()).GetRow() + adjustByHeaderRowsNum;
                    int rowsNum = tagPointer.GetKidsRoles().Count;
                    if (cellRow < rowsNum) {
                        tagPointer.MoveToKid(cellRow);
                    }
                    else {
                        tagPointer.AddTag(PdfName.TR);
                    }
                }
                child.Draw(drawContext);
                if (isTagged) {
                    tagPointer.MoveToParent();
                }
            }
            if (isTagged) {
                if (shouldHaveFooterOrHeaderTag) {
                    tagPointer.MoveToParent();
                }
            }
            DrawBorders(drawContext);
            if (footerRenderer != null) {
                bool lastFooter = isLastRendererForModelElement && modelElement.IsComplete() && !modelElement.IsSkipLastFooter
                    ();
                bool notToTagFooter = drawContext.IsTaggingEnabled() && !lastFooter;
                if (notToTagFooter) {
                    drawContext.SetTaggingEnabled(false);
                    drawContext.GetCanvas().OpenTag(new CanvasArtifact());
                }
                footerRenderer.Draw(drawContext);
                if (notToTagFooter) {
                    drawContext.GetCanvas().CloseTag();
                    drawContext.SetTaggingEnabled(true);
                }
            }
        }

        /// <summary><inheritDoc/></summary>
        public override IRenderer GetNextRenderer() {
            iText.Layout.Renderer.TableRenderer nextTable = new iText.Layout.Renderer.TableRenderer();
            nextTable.modelElement = modelElement;
            return nextTable;
        }

        /// <summary><inheritDoc/></summary>
        public override void Move(float dxRight, float dyUp) {
            base.Move(dxRight, dyUp);
            if (headerRenderer != null) {
                headerRenderer.Move(dxRight, dyUp);
            }
            if (footerRenderer != null) {
                footerRenderer.Move(dxRight, dyUp);
            }
        }

        protected internal virtual float[] CalculateScaledColumnWidths(Table tableModel, float tableWidth, float leftBorderWidth
            , float rightBorderWidth) {
            float[] columnWidths = new float[tableModel.GetNumberOfColumns()];
            float widthSum = 0;
            for (int i = 0; i < tableModel.GetNumberOfColumns(); i++) {
                columnWidths[i] = tableModel.GetColumnWidth(i);
                widthSum += columnWidths[i];
            }
            for (int i_1 = 0; i_1 < tableModel.GetNumberOfColumns(); i_1++) {
                columnWidths[i_1] *= (tableWidth - leftBorderWidth / 2 - rightBorderWidth / 2) / widthSum;
            }
            return columnWidths;
        }

        protected internal virtual iText.Layout.Renderer.TableRenderer[] Split(int row) {
            return Split(row, false);
        }

        protected internal virtual iText.Layout.Renderer.TableRenderer[] Split(int row, bool hasContent) {
            iText.Layout.Renderer.TableRenderer splitRenderer = CreateSplitRenderer(new Table.RowRange(rowRange.GetStartRow
                (), rowRange.GetStartRow() + row));
            splitRenderer.rows = rows.SubList(0, row);
            int rowN = row;
            if (hasContent || row == 0) {
                rowN++;
            }
            splitRenderer.horizontalBorders = new List<List<Border>>();
            //splitRenderer.horizontalBorders.addAll(horizontalBorders);
            for (int i = 0; i <= rowN; i++) {
                splitRenderer.horizontalBorders.Add(horizontalBorders[i]);
            }
            splitRenderer.verticalBorders = new List<List<Border>>();
            //        splitRenderer.verticalBorders.addAll(verticalBorders);
            for (int i_1 = 0; i_1 < verticalBorders.Count; i_1++) {
                splitRenderer.verticalBorders.Add(new List<Border>());
                for (int j = 0; j < rowN; j++) {
                    if (verticalBorders[i_1].Count != 0) {
                        splitRenderer.verticalBorders[i_1].Add(verticalBorders[i_1][j]);
                    }
                }
            }
            splitRenderer.heights = heights;
            splitRenderer.columnWidths = columnWidths;
            iText.Layout.Renderer.TableRenderer overflowRenderer = CreateOverflowRenderer(new Table.RowRange(rowRange.
                GetStartRow() + row, rowRange.GetFinishRow()));
            overflowRenderer.rows = rows.SubList(row, rows.Count);
            splitRenderer.occupiedArea = occupiedArea;
            return new iText.Layout.Renderer.TableRenderer[] { splitRenderer, overflowRenderer };
        }

        protected internal virtual iText.Layout.Renderer.TableRenderer CreateSplitRenderer(Table.RowRange rowRange
            ) {
            iText.Layout.Renderer.TableRenderer splitRenderer = (iText.Layout.Renderer.TableRenderer)GetNextRenderer();
            splitRenderer.rowRange = rowRange;
            splitRenderer.parent = parent;
            splitRenderer.modelElement = modelElement;
            // TODO childRenderers will be populated twice during the relayout.
            // We should probably clean them before #layout().
            splitRenderer.childRenderers = childRenderers;
            splitRenderer.AddAllProperties(GetOwnProperties());
            splitRenderer.headerRenderer = headerRenderer;
            splitRenderer.footerRenderer = footerRenderer;
            splitRenderer.isLastRendererForModelElement = false;
            return splitRenderer;
        }

        protected internal virtual iText.Layout.Renderer.TableRenderer CreateOverflowRenderer(Table.RowRange rowRange
            ) {
            iText.Layout.Renderer.TableRenderer overflowRenderer = (iText.Layout.Renderer.TableRenderer)GetNextRenderer
                ();
            overflowRenderer.SetRowRange(rowRange);
            overflowRenderer.parent = parent;
            overflowRenderer.modelElement = modelElement;
            overflowRenderer.AddAllProperties(GetOwnProperties());
            overflowRenderer.isOriginalNonSplitRenderer = false;
            return overflowRenderer;
        }

        public override void DrawBorder(DrawContext drawContext) {
        }

        // Do nothing here. Itext7 handles cell and table borders collapse and draws result borders during #drawBorders()
        protected internal virtual void DrawBorders(DrawContext drawContext) {
            if (occupiedArea.GetBBox().GetHeight() < EPS) {
                // || childRenderers.size() == 0) {
                return;
            }
            float startX = GetOccupiedArea().GetBBox().GetX();
            float startY = GetOccupiedArea().GetBBox().GetY() + GetOccupiedArea().GetBBox().GetHeight();
            foreach (IRenderer child in childRenderers) {
                CellRenderer cell = (CellRenderer)child;
                if (((Cell)cell.GetModelElement()).GetRow() == this.rowRange.GetStartRow()) {
                    startY = cell.GetOccupiedArea().GetBBox().GetY() + cell.GetOccupiedArea().GetBBox().GetHeight();
                    break;
                }
            }
            foreach (IRenderer child_1 in childRenderers) {
                CellRenderer cell = (CellRenderer)child_1;
                if (((Cell)cell.GetModelElement()).GetCol() == 0) {
                    startX = cell.GetOccupiedArea().GetBBox().GetX();
                    break;
                }
            }
            bool isTagged = drawContext.IsTaggingEnabled() && GetModelElement() is IAccessibleElement;
            if (isTagged) {
                drawContext.GetCanvas().OpenTag(new CanvasArtifact());
            }
            // Notice that we draw boundary borders after all the others are drawn
            float y1 = startY;
            if (heights.Count > 0) {
                y1 -= (float)heights[0];
            }
            for (int i = 1; i < horizontalBorders.Count - 1; i++) {
                DrawHorizontalBorder(i, startX, y1, drawContext.GetCanvas());
                if (i < heights.Count) {
                    y1 -= (float)heights[i];
                }
            }
            float x1 = startX;
            if (columnWidths.Length > 0) {
                x1 += columnWidths[0];
            }
            for (int i_1 = 1; i_1 < verticalBorders.Count; i_1++) {
                DrawVerticalBorder(i_1, startY, x1, drawContext.GetCanvas());
                if (i_1 < columnWidths.Length) {
                    x1 += columnWidths[i_1];
                }
            }
            // draw boundary borders
            DrawVerticalBorder(0, startY, startX, drawContext.GetCanvas());
            DrawHorizontalBorder(0, startX, startY, drawContext.GetCanvas());
            y1 = startY;
            for (int i_2 = 0; i_2 < heights.Count; i_2++) {
                y1 -= heights[i_2];
            }
            DrawHorizontalBorder(horizontalBorders.Count - 1, startX, y1, drawContext.GetCanvas());
            if (isTagged) {
                drawContext.GetCanvas().CloseTag();
            }
        }

        private void DrawHorizontalBorder(int i, float startX, float y1, PdfCanvas canvas) {
            List<Border> borders = horizontalBorders[i];
            float x1 = startX;
            float x2 = x1 + columnWidths[0];
            if (i == 0) {
                if (verticalBorders != null && verticalBorders.Count > 0 && verticalBorders[0].Count > 0 && verticalBorders
                    [verticalBorders.Count - 1].Count > 0) {
                    Border firstBorder = verticalBorders[0][0];
                    if (firstBorder != null) {
                        x1 -= firstBorder.GetWidth() / 2;
                    }
                }
            }
            else {
                if (i == horizontalBorders.Count - 1) {
                    if (verticalBorders != null && verticalBorders.Count > 0 && verticalBorders[0].Count > 0 && verticalBorders
                        [verticalBorders.Count - 1] != null && verticalBorders[verticalBorders.Count - 1].Count > 0 && verticalBorders
                        [0] != null) {
                        Border firstBorder = verticalBorders[0][verticalBorders[0].Count - 1];
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
                        x1 = x2;
                    }
                }
                else {
                    x1 += columnWidths[j - 1];
                    x2 = x1;
                }
                if (curBorder != null) {
                    x2 += columnWidths[j];
                }
            }
            Border lastBorder = borders.Count > j - 1 ? borders[j - 1] : null;
            if (lastBorder != null) {
                if (verticalBorders != null && verticalBorders[j] != null && verticalBorders[j].Count > 0) {
                    if (i == 0) {
                        if (verticalBorders[j][i] != null) {
                            x2 += verticalBorders[j][i].GetWidth() / 2;
                        }
                    }
                    else {
                        if (i == horizontalBorders.Count - 1 && verticalBorders[j].Count >= i - 1 && verticalBorders[j][i - 1] != 
                            null) {
                            x2 += verticalBorders[j][i - 1].GetWidth() / 2;
                        }
                    }
                }
                lastBorder.DrawCellBorder(canvas, x1, y1, x2, y1);
            }
        }

        private void DrawVerticalBorder(int i, float startY, float x1, PdfCanvas canvas) {
            List<Border> borders = verticalBorders[i];
            float y1 = startY;
            float y2 = y1;
            if (!heights.IsEmpty()) {
                y2 = y1 - (float)heights[0];
            }
            int j;
            for (j = 1; j < borders.Count; j++) {
                Border prevBorder = borders[j - 1];
                Border curBorder = borders[j];
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
                return;
            }
            Border lastBorder = borders[j - 1];
            if (lastBorder != null) {
                lastBorder.DrawCellBorder(canvas, x1, y1, x1, y2);
            }
        }

        /// <summary>If there is some space left, we move footer up, because initially footer will be at the very bottom of the area.
        ///     </summary>
        /// <remarks>
        /// If there is some space left, we move footer up, because initially footer will be at the very bottom of the area.
        /// We also adjust occupied area by footer size if it is present.
        /// </remarks>
        /// <param name="layoutBox">the layout box which represents the area which is left free.</param>
        private void AdjustFooterAndFixOccupiedArea(Rectangle layoutBox) {
            if (footerRenderer != null) {
                footerRenderer.Move(0, layoutBox.GetHeight());
                float footerHeight = footerRenderer.GetOccupiedArea().GetBBox().GetHeight();
                occupiedArea.GetBBox().MoveDown(footerHeight).IncreaseHeight(footerHeight);
            }
        }

        /// <summary>This method checks if we can completely fit the rows in the given area, staring from the startRow.
        ///     </summary>
        private bool CanFitRowsInGivenArea(LayoutArea layoutArea, int startRow, float[] columnWidths, IList<float>
             heights) {
            layoutArea = layoutArea.Clone();
            heights = new List<float>(heights);
            for (int row = startRow; row < rows.Count; row++) {
                CellRenderer[] rowCells = rows[row];
                float rowHeight = 0;
                for (int col = 0; col < rowCells.Length; col++) {
                    CellRenderer cell = rowCells[col];
                    if (cell == null) {
                        continue;
                    }
                    int colspan = (int)cell.GetPropertyAsInteger(Property.COLSPAN);
                    int rowspan = (int)cell.GetPropertyAsInteger(Property.ROWSPAN);
                    float cellWidth = 0;
                    float colOffset = 0;
                    for (int i = col; i < col + colspan; i++) {
                        cellWidth += columnWidths[i];
                    }
                    for (int i_1 = 0; i_1 < col; i_1++) {
                        colOffset += columnWidths[i_1];
                    }
                    float rowspanOffset = 0;
                    for (int i_2 = row - 1; i_2 > row - rowspan && i_2 >= 0; i_2--) {
                        rowspanOffset += (float)heights[i_2];
                    }
                    float cellLayoutBoxHeight = rowspanOffset + layoutArea.GetBBox().GetHeight();
                    Rectangle cellLayoutBox = new Rectangle(layoutArea.GetBBox().GetX() + colOffset, layoutArea.GetBBox().GetY
                        (), cellWidth, cellLayoutBoxHeight);
                    LayoutArea cellArea = new LayoutArea(layoutArea.GetPageNumber(), cellLayoutBox);
                    LayoutResult cellResult = cell.SetParent(this).Layout(new LayoutContext(cellArea));
                    if (cellResult.GetStatus() != LayoutResult.FULL) {
                        return false;
                    }
                    rowHeight = Math.Max(rowHeight, cellResult.GetOccupiedArea().GetBBox().GetHeight());
                }
                heights.Add(rowHeight);
                layoutArea.GetBBox().MoveUp(rowHeight).DecreaseHeight(rowHeight);
            }
            return true;
        }

        private void BuildBordersArrays(CellRenderer cell, int row, bool hasContent) {
            int colspan = (int)cell.GetPropertyAsInteger(Property.COLSPAN);
            int rowspan = (int)cell.GetPropertyAsInteger(Property.ROWSPAN);
            int colN = ((Cell)cell.GetModelElement()).GetCol();
            Border[] cellBorders = cell.GetBorders();
            if (row + 1 - rowspan < 0) {
                rowspan = row + 1;
            }
            if (row + 1 - rowspan != 0) {
                for (int i = 0; i < colspan; i++) {
                    if (CheckAndReplaceBorderInArray(horizontalBorders, row + 1 - rowspan, colN + i, cellBorders[0])) {
                        CellRenderer rend = rows[row - rowspan][colN];
                        if (rend != null) {
                            rend.SetBorders(cellBorders[0], 2);
                        }
                    }
                    else {
                        cell.SetBorders(horizontalBorders[row + 1 - rowspan][colN + i], 0);
                    }
                }
            }
            else {
                for (int i = 0; i < colspan; i++) {
                    if (!CheckAndReplaceBorderInArray(horizontalBorders, 0, colN + i, cellBorders[0])) {
                        cell.SetBorders(horizontalBorders[0][colN + i], 0);
                    }
                }
            }
            for (int i_1 = 0; i_1 < colspan; i_1++) {
                if (hasContent) {
                    if (row + 1 == horizontalBorders.Count) {
                        horizontalBorders.Add(new List<Border>());
                    }
                    List<Border> borders = horizontalBorders[row + 1];
                    if (borders.Count <= colN + i_1) {
                        for (int count = borders.Count; count < colN + i_1; count++) {
                            borders.Add(null);
                        }
                        borders.Add(cellBorders[2]);
                    }
                    else {
                        if (borders.Count == colN + i_1) {
                            borders.Add(cellBorders[2]);
                        }
                        else {
                            borders[colN + i_1] = cellBorders[2];
                        }
                    }
                }
                else {
                    if (row == horizontalBorders.Count) {
                        horizontalBorders.Add(new List<Border>());
                    }
                    horizontalBorders[row].Add(colN + i_1, cellBorders[2]);
                }
            }
            if (rowspan > 1) {
                int numOfColumns = ((Table)GetModelElement()).GetNumberOfColumns();
                for (int k = row - rowspan + 1; k <= row; k++) {
                    List<Border> borders = horizontalBorders[k];
                    if (borders.Count < numOfColumns) {
                        for (int j = borders.Count; j < numOfColumns; j++) {
                            borders.Add(null);
                        }
                    }
                }
            }
            if (colN != 0) {
                for (int j = row - rowspan + 1; j <= row; j++) {
                    if (CheckAndReplaceBorderInArray(verticalBorders, colN, j, cellBorders[3])) {
                        CellRenderer rend = rows[j][colN - 1];
                        if (rend != null) {
                            rend.SetBorders(cellBorders[3], 1);
                        }
                    }
                    else {
                        CellRenderer rend = rows[j][colN];
                        if (rend != null) {
                            rend.SetBorders(verticalBorders[colN][row], 3);
                        }
                    }
                }
            }
            else {
                for (int j = row - rowspan + 1; j <= row; j++) {
                    if (verticalBorders.IsEmpty()) {
                        verticalBorders.Add(new List<Border>());
                    }
                    if (verticalBorders[0].Count <= j) {
                        verticalBorders[0].Add(cellBorders[3]);
                    }
                    else {
                        verticalBorders[0][j] = cellBorders[3];
                    }
                }
            }
            for (int i_2 = row - rowspan + 1; i_2 <= row; i_2++) {
                CheckAndReplaceBorderInArray(verticalBorders, colN + colspan, i_2, cellBorders[1]);
            }
            if (colspan > 1) {
                for (int k = colN; k <= colspan + colN; k++) {
                    List<Border> borders = verticalBorders[k];
                    if (borders.Count < row + rowspan) {
                        for (int j = borders.Count; j < row + rowspan; j++) {
                            borders.Add(null);
                        }
                    }
                }
            }
        }

        /// <summary>Returns the collapsed border.</summary>
        /// <remarks>
        /// Returns the collapsed border. We process collapse
        /// if the table border width is strictly greater than cell border width.
        /// </remarks>
        /// <param name="cellBorder">cell border</param>
        /// <param name="tableBorder">table border</param>
        /// <returns>the collapsed border</returns>
        private Border GetCollapsedBorder(Border cellBorder, Border tableBorder) {
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

        private bool CheckAndReplaceBorderInArray(List<List<Border>> borderArray, int i, int j, Border borderToAdd
            ) {
            if (borderArray.Count <= i) {
                for (int count = borderArray.Count; count <= i; count++) {
                    borderArray.Add(new List<Border>());
                }
            }
            List<Border> borders = borderArray[i];
            if (borders.IsEmpty()) {
                for (int count = 0; count < j; count++) {
                    borders.Add(null);
                }
                borders.Add(borderToAdd);
                return true;
            }
            if (borders.Count == j) {
                borders.Add(borderToAdd);
                return true;
            }
            if (borders.Count <= j) {
                for (int count = borders.Count; count <= j; count++) {
                    borders.Add(count, null);
                }
            }
            Border neighbour = borders[j];
            if (neighbour == null) {
                borders[j] = borderToAdd;
                return true;
            }
            else {
                if (neighbour != borderToAdd) {
                    if (borderToAdd != null && neighbour.GetWidth() < borderToAdd.GetWidth()) {
                        borders[j] = borderToAdd;
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>This method is used to set row range for table renderer during creating a new renderer.</summary>
        /// <remarks>
        /// This method is used to set row range for table renderer during creating a new renderer.
        /// The purpose to use this method is to remove input argument RowRange from createOverflowRenderer
        /// and createSplitRenderer methods.
        /// </remarks>
        private void SetRowRange(Table.RowRange rowRange) {
            this.rowRange = rowRange;
            for (int row = rowRange.GetStartRow(); row <= rowRange.GetFinishRow(); row++) {
                rows.Add(new CellRenderer[((Table)modelElement).GetNumberOfColumns()]);
            }
        }

        /// <summary>This is a struct used for convenience in layout.</summary>
        private class CellRendererInfo {
            public CellRenderer cellRenderer;

            public int column;

            public int finishRowInd;

            public CellRendererInfo(CellRenderer cellRenderer, int column, int finishRow) {
                this.cellRenderer = cellRenderer;
                this.column = column;
                // When a cell has a rowspan, this is the index of the finish row of the cell.
                // Otherwise, this is simply the index of the row of the cell in the {@link #rows} array.
                this.finishRowInd = finishRow;
            }
        }
    }
}
