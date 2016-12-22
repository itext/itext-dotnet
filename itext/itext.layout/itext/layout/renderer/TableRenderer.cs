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
using iText.IO;
using iText.IO.Log;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Tagutils;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Layout;
using iText.Layout.Margincollapse;
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
                logger.Error("Only CellRenderer could be added");
            }
        }

        protected internal override Rectangle ApplyBorderBox(Rectangle rect, Border[] borders, bool reverse) {
            // Do nothing here. Applying border box for tables is indeed difficult operation and is done on #layout()
            return rect;
        }

        /// <summary><inheritDoc/></summary>
        public override LayoutResult Layout(LayoutContext layoutContext) {
            OverrideHeightProperties();
            bool wasHeightClipped = false;
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
            MarginsCollapseHandler marginsCollapseHandler = null;
            bool marginsCollapsingEnabled = true.Equals(GetPropertyAsBoolean(Property.COLLAPSING_MARGINS));
            if (marginsCollapsingEnabled) {
                marginsCollapseHandler = new MarginsCollapseHandler(this, layoutContext.GetMarginsCollapseInfo());
                marginsCollapseHandler.StartMarginsCollapse(layoutBox);
            }
            ApplyMargins(layoutBox, false);
            Border[] borders;
            int row;
            int col;
            float[] collapsedTableBorderWidths = GetCollapsedBorderWidths(rows, GetBorders(), false);
            float topTableBorderWidth = collapsedTableBorderWidths[0];
            float rightTableBorderWidth = collapsedTableBorderWidths[1];
            float bottomTableBorderWidth = 0;
            float leftTableBorderWidth = collapsedTableBorderWidths[3];
            if (IsPositioned()) {
                float x = (float)this.GetPropertyAsFloat(Property.X);
                float relativeX = IsFixedLayout() ? 0 : layoutBox.GetX();
                layoutBox.SetX(relativeX + x);
            }
            Table tableModel = (Table)GetModelElement();
            float? tableWidth = RetrieveWidth(layoutBox.GetWidth());
            if (tableWidth == null || tableWidth == 0) {
                float totalColumnWidthInPercent = 0;
                for (col = 0; col < tableModel.GetNumberOfColumns(); col++) {
                    UnitValue columnWidth = tableModel.GetColumnWidth(col);
                    if (columnWidth.IsPercentValue()) {
                        totalColumnWidthInPercent += columnWidth.GetValue();
                    }
                }
                tableWidth = layoutBox.GetWidth();
                if (totalColumnWidthInPercent > 0) {
                    tableWidth = layoutBox.GetWidth() * totalColumnWidthInPercent / 100;
                }
            }
            // Float blockHeight = retrieveHeight();
            float? blockMaxHeight = RetrieveMaxHeight();
            if (null != blockMaxHeight && blockMaxHeight < layoutBox.GetHeight() && !true.Equals(GetPropertyAsBoolean(
                Property.FORCED_PLACEMENT))) {
                layoutBox.MoveUp(layoutBox.GetHeight() - (float)blockMaxHeight).SetHeight((float)blockMaxHeight);
                wasHeightClipped = true;
            }
            occupiedArea = new LayoutArea(area.GetPageNumber(), new Rectangle(layoutBox.GetX(), layoutBox.GetY() + layoutBox
                .GetHeight(), (float)tableWidth, 0));
            int numberOfColumns = ((Table)GetModelElement()).GetNumberOfColumns();
            horizontalBorders = new List<List<Border>>();
            verticalBorders = new List<List<Border>>();
            Table footerElement = tableModel.GetFooter();
            if (footerElement != null) {
                borders = GetBorders();
                footerRenderer = (iText.Layout.Renderer.TableRenderer)footerElement.CreateRendererSubTree().SetParent(this
                    );
                if (tableModel.IsEmpty()) {
                    footerRenderer.SetBorders(borders[0], 0);
                    SetBorders(Border.NO_BORDER, 0);
                }
                footerRenderer.SetBorders(borders[1], 1);
                footerRenderer.SetBorders(borders[2], 2);
                footerRenderer.SetBorders(borders[3], 3);
                SetBorders(Border.NO_BORDER, 2);
                collapsedTableBorderWidths = GetCollapsedBorderWidths(footerRenderer.rows, footerRenderer.GetBorders(), false
                    );
                float rightFooterBorderWidth = collapsedTableBorderWidths[1];
                float leftFooterBorderWidth = collapsedTableBorderWidths[3];
                leftTableBorderWidth = Math.Max(leftTableBorderWidth, leftFooterBorderWidth);
                rightTableBorderWidth = Math.Max(rightTableBorderWidth, rightFooterBorderWidth);
                // apply the difference to set footer and table left/right margins identical
                layoutBox.ApplyMargins<Rectangle>(0, Math.Max(0, rightTableBorderWidth - rightFooterBorderWidth) / 2, 0, Math
                    .Max(0, leftTableBorderWidth - leftFooterBorderWidth) / 2, false);
                LayoutResult result = footerRenderer.Layout(new LayoutContext(new LayoutArea(area.GetPageNumber(), layoutBox
                    )));
                if (result.GetStatus() != LayoutResult.FULL) {
                    return new LayoutResult(LayoutResult.NOTHING, null, null, this, result.GetCauseOfNothing());
                }
                float footerHeight = result.GetOccupiedArea().GetBBox().GetHeight();
                footerRenderer.Move(0, -(layoutBox.GetHeight() - footerHeight));
                layoutBox.ApplyMargins<Rectangle>(0, Math.Max(0, rightTableBorderWidth - rightFooterBorderWidth) / 2, 0, Math
                    .Max(0, leftTableBorderWidth - leftFooterBorderWidth) / 2, true);
                layoutBox.MoveUp(footerHeight).DecreaseHeight(footerHeight);
                if (!tableModel.IsEmpty()) {
                    float maxFooterTopBorderWidth = 0;
                    List<Border> footerBorders = footerRenderer.horizontalBorders[0];
                    foreach (Border border in footerBorders) {
                        if (null != border && border.GetWidth() > maxFooterTopBorderWidth) {
                            maxFooterTopBorderWidth = border.GetWidth();
                        }
                    }
                    footerRenderer.occupiedArea.GetBBox().DecreaseHeight(maxFooterTopBorderWidth);
                    layoutBox.MoveDown(maxFooterTopBorderWidth).IncreaseHeight(maxFooterTopBorderWidth);
                }
            }
            Table headerElement = tableModel.GetHeader();
            bool isFirstHeader = rowRange.GetStartRow() == 0 && isOriginalNonSplitRenderer;
            bool headerShouldBeApplied = !rows.IsEmpty() && (!isOriginalNonSplitRenderer || isFirstHeader && !tableModel
                .IsSkipFirstHeader());
            if (headerElement != null && headerShouldBeApplied) {
                borders = GetBorders();
                headerRenderer = (iText.Layout.Renderer.TableRenderer)headerElement.CreateRendererSubTree().SetParent(this
                    );
                if (tableModel.IsEmpty()) {
                    headerRenderer.SetBorders(borders[2], 2);
                    SetBorders(Border.NO_BORDER, 2);
                }
                headerRenderer.SetBorders(borders[0], 0);
                headerRenderer.SetBorders(borders[1], 1);
                headerRenderer.SetBorders(borders[3], 3);
                SetBorders(Border.NO_BORDER, 0);
                collapsedTableBorderWidths = GetCollapsedBorderWidths(headerRenderer.rows, headerRenderer.GetBorders(), false
                    );
                float rightHeaderBorderWidth = collapsedTableBorderWidths[1];
                float leftHeaderBorderWidth = collapsedTableBorderWidths[3];
                leftTableBorderWidth = Math.Max(leftTableBorderWidth, leftHeaderBorderWidth);
                rightTableBorderWidth = Math.Max(rightTableBorderWidth, rightHeaderBorderWidth);
                // apply the difference to set header and table left/right margins identical
                layoutBox.ApplyMargins<Rectangle>(0, Math.Max(0, rightTableBorderWidth - rightHeaderBorderWidth) / 2, 0, Math
                    .Max(0, leftTableBorderWidth - leftHeaderBorderWidth) / 2, false);
                LayoutResult result = headerRenderer.Layout(new LayoutContext(new LayoutArea(area.GetPageNumber(), layoutBox
                    )));
                if (result.GetStatus() != LayoutResult.FULL) {
                    return new LayoutResult(LayoutResult.NOTHING, null, null, this, result.GetCauseOfNothing());
                }
                float headerHeight = result.GetOccupiedArea().GetBBox().GetHeight();
                layoutBox.DecreaseHeight(headerHeight);
                layoutBox.ApplyMargins<Rectangle>(0, Math.Max(0, rightTableBorderWidth - rightHeaderBorderWidth) / 2, 0, Math
                    .Max(0, leftTableBorderWidth - leftHeaderBorderWidth) / 2, true);
                occupiedArea.GetBBox().MoveDown(headerHeight).IncreaseHeight(headerHeight);
                float maxHeaderBottomBorderWidth = 0;
                List<Border> rowBorders = headerRenderer.horizontalBorders[headerRenderer.horizontalBorders.Count - 1];
                foreach (Border border in rowBorders) {
                    if (null != border && maxHeaderBottomBorderWidth < border.GetWidth()) {
                        maxHeaderBottomBorderWidth = border.GetWidth();
                    }
                }
                if (!tableModel.IsEmpty()) {
                    if (maxHeaderBottomBorderWidth < topTableBorderWidth) {
                        // fix
                        headerRenderer.heights[headerRenderer.heights.Count - 1] = headerRenderer.heights[headerRenderer.heights.Count
                             - 1] + (topTableBorderWidth - maxHeaderBottomBorderWidth) / 2;
                        headerRenderer.occupiedArea.GetBBox().MoveDown(topTableBorderWidth - maxHeaderBottomBorderWidth).IncreaseHeight
                            (topTableBorderWidth - maxHeaderBottomBorderWidth);
                        occupiedArea.GetBBox().MoveDown(topTableBorderWidth - maxHeaderBottomBorderWidth).IncreaseHeight(topTableBorderWidth
                             - maxHeaderBottomBorderWidth);
                        layoutBox.DecreaseHeight(topTableBorderWidth - maxHeaderBottomBorderWidth);
                    }
                    else {
                        topTableBorderWidth = maxHeaderBottomBorderWidth;
                    }
                    // correct in a view of table handling
                    layoutBox.IncreaseHeight(topTableBorderWidth);
                    occupiedArea.GetBBox().MoveUp(topTableBorderWidth).DecreaseHeight(topTableBorderWidth);
                }
            }
            // collapse top border
            borders = GetBorders();
            topTableBorderWidth = Math.Max(null == borders[0] ? 0 : borders[0].GetWidth(), topTableBorderWidth);
            bottomTableBorderWidth = null == borders[2] ? 0 : borders[2].GetWidth();
            // Apply halves of the borders. The other halves are applied on a Cell level
            layoutBox.ApplyMargins<Rectangle>(0, rightTableBorderWidth / 2, 0, leftTableBorderWidth / 2, false);
            if (!tableModel.IsEmpty()) {
                layoutBox.DecreaseHeight(topTableBorderWidth / 2);
                occupiedArea.GetBBox().MoveDown(topTableBorderWidth / 2).IncreaseHeight(topTableBorderWidth / 2);
            }
            else {
                if (tableModel.IsComplete() && 0 == tableModel.GetLastRowBottomBorder().Count) {
                    layoutBox.DecreaseHeight(topTableBorderWidth);
                    occupiedArea.GetBBox().MoveDown(topTableBorderWidth).IncreaseHeight(topTableBorderWidth);
                }
            }
            columnWidths = CalculateScaledColumnWidths(tableModel, (float)tableWidth, leftTableBorderWidth, rightTableBorderWidth
                );
            LayoutResult[] splits = new LayoutResult[tableModel.GetNumberOfColumns()];
            // This represents the target row index for the overflow renderer to be placed to.
            // Usually this is just the current row id of a cell, but it has valuable meaning when a cell has rowspan.
            int[] targetOverflowRowIndex = new int[tableModel.GetNumberOfColumns()];
            horizontalBorders.Add(tableModel.GetLastRowBottomBorder());
            for (row = 0; row < rows.Count; row++) {
                // if forced placement was earlier set, this means the element did not fit into the area, and in this case
                // we only want to place the first row in a forced way, not the next ones, otherwise they will be invisible
                if (row == 1 && true.Equals(this.GetOwnProperty<bool?>(Property.FORCED_PLACEMENT))) {
                    DeleteOwnProperty(Property.FORCED_PLACEMENT);
                }
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
                for (col = 0; col < currentRow.Length; col++) {
                    if (currentRow[col] != null) {
                        cellProcessingQueue.AddLast(new TableRenderer.CellRendererInfo(currentRow[col], col, row));
                    }
                }
                // the element which was the first to cause Layout.Nothing
                IRenderer firstCauseOfNothing = null;
                // the width of the widest bottom border of the row
                if (cellProcessingQueue.Count != 0) {
                    bottomTableBorderWidth = 0;
                }
                while (cellProcessingQueue.Count > 0) {
                    TableRenderer.CellRendererInfo currentCellInfo = cellProcessingQueue.JRemoveFirst();
                    col = currentCellInfo.column;
                    CellRenderer cell = currentCellInfo.cellRenderer;
                    int colspan = (int)cell.GetPropertyAsInteger(Property.COLSPAN);
                    int rowspan = (int)cell.GetPropertyAsInteger(Property.ROWSPAN);
                    targetOverflowRowIndex[col] = currentCellInfo.finishRowInd;
                    // This cell came from the future (split occurred and we need to place cell with big rowpsan into the current area)
                    bool currentCellHasBigRowspan = (row != currentCellInfo.finishRowInd);
                    // collapse boundary borders if necessary
                    // notice that bottom border collapse is handled afterwards
                    Border[] cellBorders = cell.GetBorders();
                    if (0 >= row - rowspan + 1) {
                        Border collapsed = GetCollapsedBorder(cellBorders[0], null != headerRenderer ? headerRenderer.horizontalBorders
                            [headerRenderer.horizontalBorders.Count - 1][col] : borders[0]);
                        if (null != headerRenderer && collapsed == cellBorders[0]) {
                            headerRenderer.horizontalBorders[headerRenderer.horizontalBorders.Count - 1][col] = collapsed;
                        }
                        cell.SetProperty(Property.BORDER_TOP, collapsed);
                    }
                    if (0 == col) {
                        cell.SetProperty(Property.BORDER_LEFT, GetCollapsedBorder(cellBorders[3], borders[3]));
                    }
                    if (tableModel.GetNumberOfColumns() == col + colspan) {
                        cell.SetProperty(Property.BORDER_RIGHT, GetCollapsedBorder(cellBorders[1], borders[1]));
                    }
                    BuildBordersArrays(cell, currentCellInfo.finishRowInd, col);
                    float cellWidth = 0;
                    float colOffset = 0;
                    for (int k = col; k < col + colspan; k++) {
                        cellWidth += columnWidths[k];
                    }
                    for (int l = 0; l < col; l++) {
                        colOffset += columnWidths[l];
                    }
                    float rowspanOffset = 0;
                    for (int m = row - 1; m > currentCellInfo.finishRowInd - rowspan && m >= 0; m--) {
                        rowspanOffset += (float)heights[m];
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
                    UnitValue cellWidthProperty = cell.GetProperty<UnitValue>(Property.WIDTH);
                    if (cellWidthProperty != null && cellWidthProperty.IsPercentValue()) {
                        cell.SetProperty(Property.WIDTH, UnitValue.CreatePointValue(cellWidth));
                    }
                    // Increase bottom borders widths up to the table's (or footer's) if necessary to perform #layout() correctly
                    Border oldBottomBorder = cell.GetBorders()[2];
                    Border collapsedBottomBorder = GetCollapsedBorder(oldBottomBorder, footerRenderer != null ? footerRenderer
                        .horizontalBorders[0][col] : borders[2]);
                    if (collapsedBottomBorder != null) {
                        //                    bottomTableBorderWidth = Math.max(bottomTableBorderWidth, collapsedBottomBorder.getWidth());
                        cellArea.GetBBox().ApplyMargins<Rectangle>(0, 0, collapsedBottomBorder.GetWidth() / 2, 0, false);
                        cell.SetProperty(Property.BORDER_BOTTOM, collapsedBottomBorder);
                    }
                    LayoutResult cellResult = cell.SetParent(this).Layout(new LayoutContext(cellArea));
                    if (collapsedBottomBorder != null && null != cellResult.GetOccupiedArea()) {
                        // apply the difference between collapsed table border and own cell border
                        cellResult.GetOccupiedArea().GetBBox().MoveUp((collapsedBottomBorder.GetWidth() - (oldBottomBorder == null
                             ? 0 : oldBottomBorder.GetWidth())) / 2).DecreaseHeight((collapsedBottomBorder.GetWidth() - (oldBottomBorder
                             == null ? 0 : oldBottomBorder.GetWidth())) / 2);
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
                        if (cellResult.GetStatus() != LayoutResult.FULL) {
                            splits[col] = cellResult;
                        }
                        if (cellResult.GetStatus() == LayoutResult.PARTIAL) {
                            currentRow[col] = (CellRenderer)cellResult.GetSplitRenderer();
                        }
                        else {
                            rows[currentCellInfo.finishRowInd][col] = null;
                            currentRow[col] = cell;
                            rowMoves[col] = currentCellInfo.finishRowInd;
                        }
                    }
                    else {
                        if (cellResult.GetStatus() != LayoutResult.FULL) {
                            // first time split occurs
                            if (!split) {
                                int addCol;
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
                                        for (addCol = 0; addCol < currentRow.Length; addCol++) {
                                            if (currentRow[addCol] != null) {
                                                cellProcessingQueue.AddLast(new TableRenderer.CellRendererInfo(currentRow[addCol], addCol, row));
                                            }
                                        }
                                        footerRenderer = null;
                                        continue;
                                    }
                                }
                                // Here we look for a cell with big rowspan (i.e. one which would not be normally processed in
                                // the scope of this row), and we add such cells to the queue, because we need to write them
                                // at least partially into the available area we have.
                                for (addCol = 0; addCol < currentRow.Length; addCol++) {
                                    if (currentRow[addCol] == null) {
                                        // Search for the next cell including rowspan.
                                        for (int addRow = row + 1; addRow < rows.Count; addRow++) {
                                            if (rows[addRow][addCol] != null) {
                                                CellRenderer addRenderer = rows[addRow][addCol];
                                                verticalAlignment = addRenderer.GetProperty<VerticalAlignment?>(Property.VERTICAL_ALIGNMENT);
                                                if (verticalAlignment != null && verticalAlignment.Equals(VerticalAlignment.BOTTOM)) {
                                                    if (row + addRenderer.GetPropertyAsInteger(Property.ROWSPAN) - 1 < addRow) {
                                                        cellProcessingQueue.AddLast(new TableRenderer.CellRendererInfo(addRenderer, addCol, addRow));
                                                        cellWithBigRowspanAdded = true;
                                                    }
                                                    else {
                                                        horizontalBorders[row + 1][addCol] = addRenderer.GetBorders()[2];
                                                        if (addCol == 0) {
                                                            for (int i = row; i >= 0; i--) {
                                                                if (!CheckAndReplaceBorderInArray(verticalBorders, addCol, i, addRenderer.GetBorders()[3], false)) {
                                                                    break;
                                                                }
                                                            }
                                                        }
                                                        else {
                                                            if (addCol == numberOfColumns - 1) {
                                                                for (int i = row; i >= 0; i--) {
                                                                    if (!CheckAndReplaceBorderInArray(verticalBorders, addCol + 1, i, addRenderer.GetBorders()[1], true)) {
                                                                        break;
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                                else {
                                                    if (row + addRenderer.GetPropertyAsInteger(Property.ROWSPAN) - 1 >= addRow) {
                                                        cellProcessingQueue.AddLast(new TableRenderer.CellRendererInfo(addRenderer, addCol, addRow));
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
                            splits[col] = cellResult;
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
                if (row == rows.Count - 1 && null != footerRenderer && tableModel.IsComplete() && tableModel.IsSkipLastFooter
                    ()) {
                    footerRenderer = null;
                    DeleteOwnProperty(Property.BORDER_BOTTOM);
                    borders = GetBorders();
                    if (null != borders[2] && bottomTableBorderWidth < borders[2].GetWidth()) {
                        bottomTableBorderWidth = borders[2].GetWidth();
                    }
                }
                CellRenderer[] lastAddedRow;
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
                        for (col = 0; col < lastAddedRow.Length; col++) {
                            if (null != lastAddedRow[col]) {
                                currentCellHeight = 0;
                                Border cellBottomBorder = lastAddedRow[col].GetBorders()[2];
                                Border collapsedBorder = GetCollapsedBorder(cellBottomBorder, footerRenderer != null ? footerRenderer.horizontalBorders
                                    [0][col] : borders[2]);
                                if (null != collapsedBorder) {
                                    bottomTableBorderWidth = Math.Max(bottomTableBorderWidth, collapsedBorder.GetWidth());
                                }
                                float cellBottomBorderWidth = null == cellBottomBorder ? 0 : cellBottomBorder.GetWidth();
                                float collapsedBorderWidth = null == collapsedBorder ? 0 : collapsedBorder.GetWidth();
                                if (cellBottomBorderWidth < collapsedBorderWidth) {
                                    lastAddedRow[col].SetProperty(Property.BORDER_BOTTOM, collapsedBorder);
                                    for (int i = col; i < col + lastAddedRow[col].GetPropertyAsInteger(Property.COLSPAN); i++) {
                                        horizontalBorders[hasContent || cellWithBigRowspanAdded ? row + 1 : row][i] = collapsedBorder;
                                    }
                                }
                                // apply the difference between collapsed table border and own cell border
                                lastAddedRow[col].occupiedArea.GetBBox().ApplyMargins<Rectangle>(0, 0, (collapsedBorderWidth - cellBottomBorderWidth
                                    ) / 2, 0, true);
                                int cellRowStartIndex = heights.Count - (int)lastAddedRow[col].GetPropertyAsInteger(Property.ROWSPAN);
                                for (int l = cellRowStartIndex > 0 ? cellRowStartIndex : 0; l < heights.Count; l++) {
                                    currentCellHeight += heights[l];
                                }
                                if (currentCellHeight < lastAddedRow[col].occupiedArea.GetBBox().GetHeight()) {
                                    bottomBorderWidthDifference = Math.Max(bottomBorderWidthDifference, (collapsedBorderWidth - cellBottomBorderWidth
                                        ) / 2);
                                }
                            }
                        }
                        heights[heights.Count - 1] = heights[heights.Count - 1] + bottomBorderWidthDifference;
                        occupiedArea.GetBBox().MoveDown(bottomBorderWidthDifference).IncreaseHeight(bottomBorderWidthDifference);
                        layoutBox.DecreaseHeight(bottomBorderWidthDifference);
                    }
                    // Correct occupied areas of all added cells
                    for (int k = 0; k <= row; k++) {
                        currentRow = rows[k];
                        if (k < row || (k == row && (hasContent || cellWithBigRowspanAdded))) {
                            for (col = 0; col < currentRow.Length; col++) {
                                CellRenderer cell = currentRow[col];
                                if (cell == null) {
                                    continue;
                                }
                                float height = 0;
                                int rowspan = (int)cell.GetPropertyAsInteger(Property.ROWSPAN);
                                for (int l = k; l > ((k == row + 1) ? targetOverflowRowIndex[col] : k) - rowspan && l >= 0; l--) {
                                    height += (float)heights[l];
                                }
                                int rowN = k + 1;
                                if (k == row && !hasContent) {
                                    rowN--;
                                }
                                //                            if (horizontalBorders.get(rowN).get(col) == null) {
                                //                                horizontalBorders.get(rowN).set(col, cell.getBorders()[2]);
                                //                            }
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
                if ((split || row == rows.Count - 1) && null != footerRenderer) {
                    int lastRow;
                    if (hasContent || cellWithBigRowspanAdded) {
                        lastRow = row + 1;
                    }
                    else {
                        lastRow = row;
                    }
                    if (!tableModel.IsEmpty()) {
                        int j = 0;
                        int i = 0;
                        while (i < tableModel.GetNumberOfColumns()) {
                            if (null != footerRenderer.rows[j][i]) {
                                if (horizontalBorders[lastRow][i].GetWidth() > footerRenderer.horizontalBorders[0][i].GetWidth()) {
                                    footerRenderer.rows[j][i].SetBorders(horizontalBorders[lastRow][i], 0);
                                }
                                i += ((Cell)footerRenderer.rows[j][i].GetModelElement()).GetColspan();
                                j = 0;
                            }
                            else {
                                j++;
                                if (j == tableModel.GetNumberOfRows()) {
                                    break;
                                }
                            }
                        }
                        layoutBox.IncreaseHeight(bottomTableBorderWidth / 2);
                        occupiedArea.GetBBox().MoveUp(bottomTableBorderWidth / 2).DecreaseHeight(bottomTableBorderWidth / 2);
                    }
                    collapsedTableBorderWidths = GetCollapsedBorderWidths(footerRenderer.rows, footerRenderer.GetBorders(), false
                        );
                    layoutBox.MoveDown(footerRenderer.occupiedArea.GetBBox().GetHeight()).IncreaseHeight(footerRenderer.occupiedArea
                        .GetBBox().GetHeight());
                    // apply the difference to set footer and table left/right margins identical
                    layoutBox.ApplyMargins<Rectangle>(0, -collapsedTableBorderWidths[1] / 2, 0, -collapsedTableBorderWidths[3]
                         / 2, false);
                    footerRenderer.Layout(new LayoutContext(new LayoutArea(area.GetPageNumber(), layoutBox)));
                    layoutBox.ApplyMargins<Rectangle>(0, -collapsedTableBorderWidths[1] / 2, 0, -collapsedTableBorderWidths[3]
                         / 2, true);
                    float footerHeight = footerRenderer.GetOccupiedAreaBBox().GetHeight();
                    footerRenderer.Move(0, -(layoutBox.GetHeight() - footerHeight));
                    layoutBox.MoveUp(footerHeight).DecreaseHeight(footerHeight);
                }
                if (split) {
                    if (marginsCollapsingEnabled) {
                        marginsCollapseHandler.EndMarginsCollapse();
                    }
                    iText.Layout.Renderer.TableRenderer[] splitResult = Split(row, hasContent);
                    int[] rowspans = new int[currentRow.Length];
                    bool[] columnsWithCellToBeEnlarged = new bool[currentRow.Length];
                    for (col = 0; col < currentRow.Length; col++) {
                        if (splits[col] != null) {
                            CellRenderer cellSplit = (CellRenderer)splits[col].GetSplitRenderer();
                            if (null != cellSplit) {
                                rowspans[col] = ((Cell)cellSplit.GetModelElement()).GetRowspan();
                            }
                            if (splits[col].GetStatus() != LayoutResult.NOTHING && (hasContent || cellWithBigRowspanAdded)) {
                                childRenderers.Add(cellSplit);
                            }
                            LayoutArea cellOccupiedArea = currentRow[col].GetOccupiedArea();
                            if (hasContent || cellWithBigRowspanAdded || splits[col].GetStatus() == LayoutResult.NOTHING) {
                                currentRow[col] = null;
                                CellRenderer cellOverflow = (CellRenderer)splits[col].GetOverflowRenderer();
                                if (splits[col].GetStatus() == LayoutResult.PARTIAL) {
                                    cellOverflow.SetBorders(Border.NO_BORDER, 0);
                                    cellSplit.SetBorders(Border.NO_BORDER, 2);
                                }
                                else {
                                    if (Border.NO_BORDER != cellOverflow.GetProperty<Border>(Property.BORDER_TOP)) {
                                        cellOverflow.DeleteOwnProperty(Property.BORDER_TOP);
                                    }
                                }
                                for (int j = col; j < col + cellOverflow.GetPropertyAsInteger(Property.COLSPAN); j++) {
                                    horizontalBorders[!hasContent && splits[col].GetStatus() == LayoutResult.PARTIAL ? row : row + 1][j] = GetBorders
                                        ()[2];
                                }
                                cellOverflow.DeleteOwnProperty(Property.BORDER_BOTTOM);
                                cellOverflow.SetBorders(cellOverflow.GetBorders()[2], 2);
                                rows[targetOverflowRowIndex[col]][col] = (CellRenderer)cellOverflow.SetParent(splitResult[1]);
                            }
                            else {
                                rows[targetOverflowRowIndex[col]][col] = (CellRenderer)currentRow[col].SetParent(splitResult[1]);
                                rows[targetOverflowRowIndex[col]][col].DeleteOwnProperty(Property.BORDER_TOP);
                            }
                            rows[targetOverflowRowIndex[col]][col].occupiedArea = cellOccupiedArea;
                        }
                        else {
                            if (currentRow[col] != null) {
                                rowspans[col] = ((Cell)currentRow[col].GetModelElement()).GetRowspan();
                                if (hasContent) {
                                    columnsWithCellToBeEnlarged[col] = true;
                                    // for the future
                                    splitResult[1].rows[0][col].SetBorders(GetBorders()[0], 0);
                                }
                                else {
                                    if (Border.NO_BORDER != currentRow[col].GetProperty<Border>(Property.BORDER_TOP)) {
                                        splitResult[1].rows[0][col].DeleteOwnProperty(Property.BORDER_TOP);
                                    }
                                }
                                for (int j = col; j < col + currentRow[col].GetPropertyAsInteger(Property.COLSPAN); j++) {
                                    horizontalBorders[row + (!hasContent && rowspans[col] > 1 ? 0 : 1)][j] = GetBorders()[2];
                                }
                            }
                        }
                    }
                    int minRowspan = int.MaxValue;
                    for (col = 0; col < rowspans.Length; col++) {
                        if (0 != rowspans[col]) {
                            minRowspan = Math.Min(minRowspan, rowspans[col]);
                        }
                    }
                    for (col = 0; col < numberOfColumns; col++) {
                        if (columnsWithCellToBeEnlarged[col]) {
                            LayoutArea cellOccupiedArea = currentRow[col].GetOccupiedArea();
                            if (1 == minRowspan) {
                                // Here we use the same cell, but create a new renderer which doesn't have any children,
                                // therefore it won't have any content.
                                Cell overflowCell = ((Cell)currentRow[col].GetModelElement());
                                currentRow[col].isLastRendererForModelElement = false;
                                childRenderers.Add(currentRow[col]);
                                Border topBorder = currentRow[col].GetProperty<Border>(Property.BORDER_TOP);
                                currentRow[col] = null;
                                rows[targetOverflowRowIndex[col]][col] = (CellRenderer)overflowCell.GetRenderer().SetParent(this);
                                rows[targetOverflowRowIndex[col]][col].SetProperty(Property.BORDER_TOP, topBorder);
                            }
                            else {
                                childRenderers.Add(currentRow[col]);
                                // shift all cells in the column up
                                int i = row;
                                for (; i < row + minRowspan && i + 1 < rows.Count && rows[i + 1][col] != null; i++) {
                                    rows[i][col] = rows[i + 1][col];
                                    rows[i + 1][col] = null;
                                }
                                // the number of cells behind is less then minRowspan-1
                                // so we should process the last cell in the column as in the case 1 == minRowspan
                                if (i != row + minRowspan - 1 && null != rows[i][col]) {
                                    Cell overflowCell = ((Cell)rows[i][col].GetModelElement());
                                    Border topBorder = rows[i][col].GetProperty<Border>(Property.BORDER_TOP);
                                    rows[i][col].isLastRendererForModelElement = false;
                                    rows[i][col] = null;
                                    rows[targetOverflowRowIndex[col]][col] = (CellRenderer)overflowCell.GetRenderer().SetParent(this);
                                    rows[targetOverflowRowIndex[col]][col].SetProperty(Property.BORDER_TOP, topBorder);
                                }
                            }
                            rows[targetOverflowRowIndex[col]][col].occupiedArea = cellOccupiedArea;
                        }
                    }
                    if (hasContent || cellWithBigRowspanAdded) {
                        bottomTableBorderWidth = null == GetBorders()[2] ? 0 : GetBorders()[2].GetWidth();
                    }
                    // Apply bottom and top border
                    if (0 != this.childRenderers.Count) {
                        if (null == footerRenderer) {
                            occupiedArea.GetBBox().MoveDown(bottomTableBorderWidth / 2).IncreaseHeight(bottomTableBorderWidth / 2);
                            layoutBox.DecreaseHeight(bottomTableBorderWidth / 2);
                        }
                    }
                    else {
                        occupiedArea.GetBBox().MoveUp(topTableBorderWidth / 2).DecreaseHeight(topTableBorderWidth / 2);
                        layoutBox.IncreaseHeight(topTableBorderWidth / 2);
                    }
                    if (true.Equals(GetPropertyAsBoolean(Property.FILL_AVAILABLE_AREA)) || true.Equals(GetPropertyAsBoolean(Property
                        .FILL_AVAILABLE_AREA_ON_SPLIT))) {
                        ExtendLastRow(currentRow, layoutBox);
                    }
                    AdjustFooterAndFixOccupiedArea(layoutBox);
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
                        return new LayoutResult(LayoutResult.NOTHING, null, null, this, null == firstCauseOfNothing ? this : firstCauseOfNothing
                            );
                    }
                    else {
                        int status = (childRenderers.IsEmpty() && (tableModel.IsComplete() || footerRenderer == null)) ? LayoutResult
                            .NOTHING : LayoutResult.PARTIAL;
                        if ((status == LayoutResult.NOTHING && true.Equals(GetPropertyAsBoolean(Property.FORCED_PLACEMENT))) || wasHeightClipped
                            ) {
                            if (wasHeightClipped) {
                                ILogger logger = LoggerFactory.GetLogger(typeof(iText.Layout.Renderer.TableRenderer));
                                logger.Warn(LogMessageConstant.CLIP_ELEMENT);
                            }
                            return new LayoutResult(LayoutResult.FULL, occupiedArea, splitResult[0], null);
                        }
                        else {
                            if (HasProperty(Property.HEIGHT)) {
                                splitResult[1].SetProperty(Property.HEIGHT, RetrieveHeight() - occupiedArea.GetBBox().GetHeight());
                            }
                            if (status != LayoutResult.NOTHING) {
                                return new LayoutResult(status, occupiedArea, splitResult[0], splitResult[1], null);
                            }
                            else {
                                return new LayoutResult(status, null, splitResult[0], splitResult[1], firstCauseOfNothing);
                            }
                        }
                    }
                }
                else {
                    childRenderers.AddAll(currChildRenderers);
                    currChildRenderers.Clear();
                }
            }
            // check if the last row is incomplete
            if (tableModel.IsComplete() && !tableModel.IsEmpty()) {
                CellRenderer[] lastRow = rows[rows.Count - 1];
                int lastInRow = lastRow.Length - 1;
                while (lastInRow >= 0 && null == lastRow[lastInRow]) {
                    lastInRow--;
                }
                if (lastInRow < 0 || lastRow.Length != lastInRow + lastRow[lastInRow].GetPropertyAsInteger(Property.COLSPAN
                    )) {
                    ILogger logger = LoggerFactory.GetLogger(typeof(iText.Layout.Renderer.TableRenderer));
                    logger.Warn(LogMessageConstant.LAST_ROW_IS_NOT_COMPLETE);
                }
            }
            // if table is empty we still need to process  table borders
            if (0 == childRenderers.Count && null == headerRenderer && null == footerRenderer) {
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
                leftVerticalBorders.Add(borders[3]);
                rightVerticalBorders.Add(borders[1]);
                verticalBorders = new List<List<Border>>();
                verticalBorders.Add(leftVerticalBorders);
                for (int i_1 = 0; i_1 < ((Table)modelElement).GetNumberOfColumns() - 1; i_1++) {
                    verticalBorders.Add(new List<Border>());
                }
                verticalBorders.Add(rightVerticalBorders);
            }
            // Apply bottom and top border
            if (tableModel.IsComplete()) {
                if (null == footerRenderer) {
                    if (childRenderers.Count != 0) {
                        occupiedArea.GetBBox().MoveDown(bottomTableBorderWidth / 2).IncreaseHeight((bottomTableBorderWidth) / 2);
                        layoutBox.DecreaseHeight(bottomTableBorderWidth / 2);
                    }
                    else {
                        IList<Border> borderList = tableModel.GetLastRowBottomBorder();
                        if (borderList.Count > 0) {
                            foreach (Border border in borderList) {
                                if (null != border && border.GetWidth() > bottomTableBorderWidth) {
                                    bottomTableBorderWidth = border.GetWidth();
                                }
                            }
                        }
                        occupiedArea.GetBBox().MoveDown(bottomTableBorderWidth).IncreaseHeight((bottomTableBorderWidth));
                        layoutBox.DecreaseHeight(bottomTableBorderWidth);
                    }
                }
            }
            else {
                if (null == footerRenderer) {
                    if (childRenderers.Count != 0) {
                        occupiedArea.GetBBox().MoveUp(bottomTableBorderWidth / 2).DecreaseHeight((bottomTableBorderWidth / 2));
                        layoutBox.IncreaseHeight(bottomTableBorderWidth / 2);
                    }
                }
                else {
                    occupiedArea.GetBBox().MoveUp(bottomTableBorderWidth).DecreaseHeight((bottomTableBorderWidth));
                    layoutBox.IncreaseHeight(bottomTableBorderWidth);
                }
            }
            if ((true.Equals(GetPropertyAsBoolean(Property.FILL_AVAILABLE_AREA))) && 0 != rows.Count) {
                ExtendLastRow(rows[rows.Count - 1], layoutBox);
            }
            float? blockMinHeight = RetrieveMinHeight();
            if (null != blockMinHeight && blockMinHeight > occupiedArea.GetBBox().GetHeight()) {
                float blockBottom = Math.Max(occupiedArea.GetBBox().GetBottom() - ((float)blockMinHeight - occupiedArea.GetBBox
                    ().GetHeight()), layoutContext.GetArea().GetBBox().GetBottom());
                heights[heights.Count - 1] = heights[heights.Count - 1] + occupiedArea.GetBBox().GetBottom() - blockBottom;
                occupiedArea.GetBBox().IncreaseHeight(occupiedArea.GetBBox().GetBottom() - blockBottom).SetY(blockBottom);
            }
            if (IsPositioned()) {
                float y = (float)this.GetPropertyAsFloat(Property.Y);
                float relativeY = IsFixedLayout() ? 0 : layoutBox.GetY();
                Move(0, relativeY + y - occupiedArea.GetBBox().GetY());
            }
            if (marginsCollapsingEnabled) {
                marginsCollapseHandler.EndMarginsCollapse();
            }
            ApplyMargins(occupiedArea.GetBBox(), true);
            if ((tableModel.IsSkipLastFooter() || !tableModel.IsComplete()) && null != footerRenderer) {
                if (tableModel.GetLastRowBottomBorder().Count > 0) {
                    IList<Border> lastBottomBorders = footerRenderer.horizontalBorders[0];
                    foreach (Border border in lastBottomBorders) {
                        if (null != border && bottomTableBorderWidth < border.GetWidth()) {
                            bottomTableBorderWidth = border.GetWidth();
                        }
                    }
                }
                footerRenderer = null;
                if (0 != rows.Count || tableModel.IsComplete()) {
                    occupiedArea.GetBBox().MoveDown(bottomTableBorderWidth).IncreaseHeight(bottomTableBorderWidth);
                }
            }
            AdjustFooterAndFixOccupiedArea(layoutBox);
            return new LayoutResult(LayoutResult.FULL, occupiedArea, null, null);
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
            DrawBorders(drawContext, null == headerRenderer, null == footerRenderer);
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
            float[] scaledWidths = new float[tableModel.GetNumberOfColumns()];
            float widthSum = 0;
            float totalPointWidth = 0;
            int col;
            for (col = 0; col < tableModel.GetNumberOfColumns(); col++) {
                UnitValue columnUnitWidth = tableModel.GetColumnWidth(col);
                float columnWidth;
                if (columnUnitWidth.IsPercentValue()) {
                    columnWidth = tableWidth * columnUnitWidth.GetValue() / 100;
                    scaledWidths[col] = columnWidth;
                    widthSum += columnWidth;
                }
                else {
                    totalPointWidth += columnUnitWidth.GetValue();
                }
            }
            float freeTableSpaceWidth = tableWidth - widthSum;
            if (totalPointWidth > 0) {
                for (col = 0; col < tableModel.GetNumberOfColumns(); col++) {
                    float columnWidth;
                    UnitValue columnUnitWidth = tableModel.GetColumnWidth(col);
                    if (columnUnitWidth.IsPointValue()) {
                        columnWidth = (freeTableSpaceWidth / totalPointWidth) * columnUnitWidth.GetValue();
                        scaledWidths[col] = columnWidth;
                        widthSum += columnWidth;
                    }
                }
            }
            for (col = 0; col < tableModel.GetNumberOfColumns(); col++) {
                scaledWidths[col] *= (tableWidth - leftBorderWidth / 2 - rightBorderWidth / 2) / widthSum;
            }
            return scaledWidths;
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
            //splitRenderer.verticalBorders.addAll(verticalBorders);
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
            DrawBorders(drawContext, true, true);
        }

        protected internal virtual void DrawBorders(DrawContext drawContext, bool drawTop, bool drawBottom) {
            if (occupiedArea.GetBBox().GetHeight() < EPS || heights.Count == 0) {
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
            // process halves of the borders here
            if (childRenderers.Count == 0) {
                Border[] borders = this.GetBorders();
                if (null != borders[3]) {
                    startX += borders[3].GetWidth() / 2;
                }
                if (null != borders[0]) {
                    startY -= borders[0].GetWidth() / 2;
                    heights[0] = borders[0].GetWidth();
                }
                else {
                    if (null != borders[2]) {
                        startY -= borders[2].GetWidth() / 2;
                    }
                }
            }
            bool isTagged = drawContext.IsTaggingEnabled() && GetModelElement() is IAccessibleElement;
            if (isTagged) {
                drawContext.GetCanvas().OpenTag(new CanvasArtifact());
            }
            if (drawTop) {
                DrawHorizontalBorder(0, startX, startY, drawContext.GetCanvas());
            }
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
            if (drawBottom) {
                DrawHorizontalBorder(horizontalBorders.Count - 1, startX, y1, drawContext.GetCanvas());
            }
            float x1 = startX;
            for (int i_1 = 0; i_1 < verticalBorders.Count; i_1++) {
                DrawVerticalBorder(i_1, startY, x1, drawContext.GetCanvas());
                if (i_1 < columnWidths.Length) {
                    x1 += columnWidths[i_1];
                }
            }
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

        private void BuildBordersArrays(CellRenderer cell, int row, int col) {
            // We should check if the row number is less than horizontal borders array size. It can happen if the cell with
            // big rowspan doesn't fit current area and is going to be placed partial.
            while (row > horizontalBorders.Count) {
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

        private void BuildBordersArrays(CellRenderer cell, int row, bool isNeighbourCell) {
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
                if (!CheckAndReplaceBorderInArray(horizontalBorders, row + 1 - rowspan, colN + i, cellBorders[0], false) &&
                     !isNeighbourCell) {
                    cell.SetBorders(horizontalBorders[row + 1 - rowspan][colN + i], 0);
                }
            }
            // consider bottom border
            for (int i_1 = 0; i_1 < colspan; i_1++) {
                if (!CheckAndReplaceBorderInArray(horizontalBorders, row + 1, colN + i_1, cellBorders[2], true) && !isNeighbourCell
                    ) {
                    cell.SetBorders(horizontalBorders[row + 1][colN + i_1], 2);
                }
            }
            // process big rowspan
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
            // consider left border
            for (int j_1 = row - rowspan + 1; j_1 <= row; j_1++) {
                if (!CheckAndReplaceBorderInArray(verticalBorders, colN, j_1, cellBorders[3], false) && !isNeighbourCell) {
                    cell.SetBorders(verticalBorders[colN][j_1], 3);
                }
            }
            // consider right border
            for (int i_2 = row - rowspan + 1; i_2 <= row; i_2++) {
                if (!CheckAndReplaceBorderInArray(verticalBorders, colN + colspan, i_2, cellBorders[1], true) && !isNeighbourCell
                    ) {
                    cell.SetBorders(verticalBorders[colN + colspan][i_2], 1);
                }
            }
            // process big colspan
            if (colspan > 1) {
                for (int k = colN; k <= colspan + colN; k++) {
                    List<Border> borders = verticalBorders[k];
                    if (borders.Count < row + rowspan) {
                        for (int l = borders.Count; l < row + rowspan; l++) {
                            borders.Add(null);
                        }
                    }
                }
            }
        }

        private float[] GetCollapsedBorderWidths(IList<CellRenderer[]> rowList, Border[] tableBorders, bool collapseTop
            ) {
            // Find left, right and top collapsed borders widths.
            // In order to find left and right border widths we try to consider as few rows ad possible
            // i.e. the borders still can be drawn outside the layout area.
            float[] widths = new float[] { -1, -1, -1, -1 };
            Border border;
            int row = 0;
            while (row < rowList.Count && (-1 == widths[3] || -1 == widths[1])) {
                CellRenderer[] currentRow = rowList[row];
                if (0 == row) {
                    int col = 0;
                    while (col < currentRow.Length) {
                        if (null != rowList[row][col]) {
                            border = rowList[row][col].GetBorders()[0];
                            if (null != border && widths[0] < border.GetWidth()) {
                                widths[0] = border.GetWidth();
                            }
                            col += (int)rowList[row][col].GetPropertyAsInteger(Property.COLSPAN);
                            row = 0;
                        }
                        else {
                            row++;
                            if (row == rowList.Count) {
                                break;
                            }
                        }
                    }
                    row = 0;
                }
                if (0 != currentRow.Length) {
                    if (null != currentRow[0]) {
                        border = currentRow[0].GetBorders()[3];
                        if (null != border && border.GetWidth() > widths[3]) {
                            widths[3] = border.GetWidth();
                        }
                    }
                    // the last cell in a row can have big colspan
                    int lastInRow = currentRow.Length - 1;
                    while (lastInRow >= 0 && null == currentRow[lastInRow]) {
                        lastInRow--;
                    }
                    if (lastInRow >= 0 && currentRow.Length == lastInRow + currentRow[lastInRow].GetPropertyAsInteger(Property
                        .COLSPAN)) {
                        border = currentRow[lastInRow].GetBorders()[1];
                        if (null != border && border.GetWidth() > widths[1]) {
                            widths[1] = border.GetWidth();
                        }
                    }
                }
                row++;
            }
            // collapse with table borders
            if (collapseTop) {
                widths[0] = Math.Max(null == tableBorders[0] ? 0 : tableBorders[0].GetWidth(), widths[0]);
            }
            widths[1] = Math.Max(null == tableBorders[1] ? 0 : tableBorders[1].GetWidth(), widths[1]);
            widths[2] = null == tableBorders[2] ? 0 : tableBorders[2].GetWidth();
            widths[3] = Math.Max(null == tableBorders[3] ? 0 : tableBorders[3].GetWidth(), widths[3]);
            return widths;
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
            , bool hasPriority) {
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
            if (borders.Count < j) {
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

        protected internal virtual void ExtendLastRow(CellRenderer[] lastRow, Rectangle freeBox) {
            if (null != lastRow && 0 != heights.Count) {
                heights[heights.Count - 1] = heights[heights.Count - 1] + freeBox.GetHeight();
                occupiedArea.GetBBox().MoveDown(freeBox.GetHeight()).IncreaseHeight(freeBox.GetHeight());
                foreach (CellRenderer cell in lastRow) {
                    if (null != cell) {
                        cell.occupiedArea.GetBBox().MoveDown(freeBox.GetHeight()).IncreaseHeight(freeBox.GetHeight());
                    }
                }
                freeBox.MoveUp(freeBox.GetHeight()).SetHeight(0);
            }
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
