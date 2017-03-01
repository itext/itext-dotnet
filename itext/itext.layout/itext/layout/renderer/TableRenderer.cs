/*

This file is part of the iText (R) project.
Copyright (c) 1998-2017 iText Group NV
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
using iText.Layout.Margincollapse;
using iText.Layout.Minmaxwidth;
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

        private float[] columnWidths = null;

        private IList<float> heights = new List<float>();

        private float[] countedMinColumnWidth;

        private float[] countedMaxColumnWidth;

        private float[] countedColumnWidth = null;

        private float totalWidthForColumns;

        private float leftBorderMaxWidth;

        private float rightBorderMaxWidth;

        private TableBorders bordersHandler;

        protected internal int horizontalBordersIndexOffset;

        protected internal int verticalBordersIndexOffset;

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
            //TODO remove
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

        private Table GetTable() {
            return (Table)GetModelElement();
        }

        private void InitializeHeaderAndFooter(bool isFirstOnThePage) {
            Table table = (Table)GetModelElement();
            Border[] tableBorder = GetBorders();
            Table footerElement = table.GetFooter();
            // footer can be skipped, but after the table content will be layouted
            bool footerShouldBeApplied = !(table.IsComplete() && 0 != table.GetLastRowBottomBorder().Count && table.IsSkipLastFooter
                ()) && !true.Equals(this.GetOwnProperty<bool?>(Property.IGNORE_FOOTER));
            if (footerElement != null && footerShouldBeApplied) {
                footerRenderer = InitFooterOrHeaderRenderer(true, tableBorder);
            }
            Table headerElement = table.GetHeader();
            bool isFirstHeader = rowRange.GetStartRow() == 0 && isOriginalNonSplitRenderer;
            bool headerShouldBeApplied = !rows.IsEmpty() && (isFirstOnThePage && (!table.IsSkipFirstHeader() || !isFirstHeader
                )) && !true.Equals(this.GetOwnProperty<bool?>(Property.IGNORE_HEADER));
            if (headerElement != null && headerShouldBeApplied) {
                headerRenderer = InitFooterOrHeaderRenderer(false, tableBorder);
            }
        }

        private void CollapseAllBorders() {
        }

        // TODO
        //        Border[] tableBorder = getBorders();
        //        int numberOfColumns = getTable().getNumberOfColumns();
        //        // collapse all cell borders
        //        if (null != rows && isOriginalNonSplitRenderer && !isFooterRenderer() && !isHeaderRenderer()) {
        //            collapseAllBordersAndEmptyRows(tableBorder, 0, rows.size() - 1, numberOfColumns);
        //        } else {
        //            updateFirstRowBorders(numberOfColumns);
        //        }
        //
        //        if (isOriginalNonSplitRenderer && !isFooterRenderer() && !isHeaderRenderer()) {
        //            rightBorderMaxWidth = getMaxRightWidth(tableBorder[1]);
        //            leftBorderMaxWidth = getMaxLeftWidth(tableBorder[3]);
        //        }
        //
        //        if (footerRenderer != null) {
        //            footerRenderer.processRendererBorders(numberOfColumns);
        //            float rightFooterBorderWidth = footerRenderer.getMaxRightWidth(footerRenderer.getBorders()[1]);
        //            float leftFooterBorderWidth = footerRenderer.getMaxLeftWidth(footerRenderer.getBorders()[3]);
        //
        //            if (isOriginalNonSplitRenderer && !isFooterRenderer() && !isHeaderRenderer()) {
        //                leftBorderMaxWidth = Math.max(leftBorderMaxWidth, leftFooterBorderWidth);
        //                rightBorderMaxWidth = Math.max(rightBorderMaxWidth, rightFooterBorderWidth);
        //            }
        //        }
        //
        //        if (headerRenderer != null) {
        //            headerRenderer.processRendererBorders(numberOfColumns);
        //            float rightHeaderBorderWidth = headerRenderer.getMaxRightWidth(headerRenderer.getBorders()[1]);
        //            float leftHeaderBorderWidth = headerRenderer.getMaxLeftWidth(headerRenderer.getBorders()[3]);
        //
        //            if (isOriginalNonSplitRenderer && !isHeaderRenderer() && !isFooterRenderer()) {
        //                leftBorderMaxWidth = Math.max(leftBorderMaxWidth, leftHeaderBorderWidth);
        //                rightBorderMaxWidth = Math.max(rightBorderMaxWidth, rightHeaderBorderWidth);
        //            }
        //        }
        private bool IsOriginalRenderer() {
            return isOriginalNonSplitRenderer && !IsFooterRenderer() && !IsHeaderRenderer();
        }

        /// <summary><inheritDoc/></summary>
        public override LayoutResult Layout(LayoutContext layoutContext) {
            OverrideHeightProperties();
            float? blockMinHeight = RetrieveMinHeight();
            float? blockMaxHeight = RetrieveMaxHeight();
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
            int row;
            int col;
            if (IsPositioned()) {
                float x = (float)this.GetPropertyAsFloat(Property.X);
                float relativeX = IsFixedLayout() ? 0 : layoutBox.GetX();
                layoutBox.SetX(relativeX + x);
            }
            Table tableModel = (Table)GetModelElement();
            if (null != blockMaxHeight && blockMaxHeight < layoutBox.GetHeight() && !true.Equals(GetPropertyAsBoolean(
                Property.FORCED_PLACEMENT))) {
                layoutBox.MoveUp(layoutBox.GetHeight() - (float)blockMaxHeight).SetHeight((float)blockMaxHeight);
                wasHeightClipped = true;
            }
            int numberOfColumns = ((Table)GetModelElement()).GetNumberOfColumns();
            // The last flushed row. Empty list if the table hasn't been set incomplete
            IList<Border> lastFlushedRowBottomBorder = tableModel.GetLastRowBottomBorder();
            Border widestLustFlushedBorder = null;
            foreach (Border border in lastFlushedRowBottomBorder) {
                if (null != border && (null == widestLustFlushedBorder || widestLustFlushedBorder.GetWidth() < border.GetWidth
                    ())) {
                    widestLustFlushedBorder = border;
                }
            }
            if (IsOriginalRenderer()) {
                if (!IsFooterRenderer() && !IsHeaderRenderer()) {
                    bordersHandler = new TableBorders(rows, numberOfColumns);
                    bordersHandler.SetTableBoundingBorders(GetBorders());
                }
                bordersHandler.InitializeBorders(lastFlushedRowBottomBorder, area.IsEmptyArea());
                bordersHandler.CollapseAllBordersAndEmptyRows(rows, GetBorders(), 0, rows.Count - 1, numberOfColumns);
            }
            else {
                if (!IsFooterRenderer() && !IsHeaderRenderer()) {
                    bordersHandler.SetRows(rows);
                }
            }
            InitializeHeaderAndFooter(0 == rowRange.GetStartRow() || area.IsEmptyArea());
            //collapseAllBorders();
            if (IsOriginalRenderer()) {
                CalculateColumnWidths(layoutBox.GetWidth(), new float[] { 0, rightBorderMaxWidth, 0, leftBorderMaxWidth });
            }
            float tableWidth = GetTableWidth();
            if (layoutBox.GetWidth() > tableWidth) {
                layoutBox.SetWidth((float)tableWidth + rightBorderMaxWidth / 2 + leftBorderMaxWidth / 2);
            }
            occupiedArea = new LayoutArea(area.GetPageNumber(), new Rectangle(layoutBox.GetX(), layoutBox.GetY() + layoutBox
                .GetHeight(), (float)tableWidth, 0));
            if (footerRenderer != null) {
                // apply the difference to set footer and table left/right margins identical
                PrepareFooterOrHeaderRendererForLayout(footerRenderer, layoutBox.GetWidth());
                LayoutResult result = footerRenderer.Layout(new LayoutContext(new LayoutArea(area.GetPageNumber(), layoutBox
                    )));
                if (result.GetStatus() != LayoutResult.FULL) {
                    return new LayoutResult(LayoutResult.NOTHING, null, null, this, result.GetCauseOfNothing());
                }
                float footerHeight = result.GetOccupiedArea().GetBBox().GetHeight();
                footerRenderer.Move(0, -(layoutBox.GetHeight() - footerHeight));
                layoutBox.MoveUp(footerHeight).DecreaseHeight(footerHeight);
                if (!tableModel.IsEmpty()) {
                    float maxFooterTopBorderWidth = 0;
                    IList<Border> footerTopBorders = footerRenderer.bordersHandler.horizontalBorders[0];
                    foreach (Border border in footerTopBorders) {
                        if (null != border && border.GetWidth() > maxFooterTopBorderWidth) {
                            maxFooterTopBorderWidth = border.GetWidth();
                        }
                    }
                    footerRenderer.occupiedArea.GetBBox().DecreaseHeight(maxFooterTopBorderWidth);
                    layoutBox.MoveDown(maxFooterTopBorderWidth).IncreaseHeight(maxFooterTopBorderWidth);
                }
                // we will delete FORCED_PLACEMENT property after adding one row
                // but the footer should be forced placed once more (since we renderer footer twice)
                if (true.Equals(GetPropertyAsBoolean(Property.FORCED_PLACEMENT))) {
                    footerRenderer.SetProperty(Property.FORCED_PLACEMENT, true);
                }
            }
            float topTableBorderWidth = bordersHandler.GetMaxTopWidth(null);
            // first row own top border. We will use it in header processing
            if (headerRenderer != null) {
                PrepareFooterOrHeaderRendererForLayout(headerRenderer, layoutBox.GetWidth());
                LayoutResult result = headerRenderer.Layout(new LayoutContext(new LayoutArea(area.GetPageNumber(), layoutBox
                    )));
                if (result.GetStatus() != LayoutResult.FULL) {
                    return new LayoutResult(LayoutResult.NOTHING, null, null, this, result.GetCauseOfNothing());
                }
                float headerHeight = result.GetOccupiedArea().GetBBox().GetHeight();
                layoutBox.DecreaseHeight(headerHeight);
                occupiedArea.GetBBox().MoveDown(headerHeight).IncreaseHeight(headerHeight);
                float maxHeaderBottomBorderWidth = 0;
                IList<Border> rowBorders = headerRenderer.bordersHandler.horizontalBorders[headerRenderer.bordersHandler.horizontalBorders
                    .Count - 1];
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
            Border[] borders = GetBorders();
            float bottomTableBorderWidth = null == borders[2] ? 0 : borders[2].GetWidth();
            // TODO
            //        if (null != rows && 0 != rows.size()) {
            //            bordersHandler.correctFirstRowTopBorders(borders[0], numberOfColumns);
            //        }
            if (IsOriginalRenderer()) {
                bordersHandler.UpdateTopBorder();
            }
            topTableBorderWidth = bordersHandler.GetMaxTopWidth(borders[0]);
            // Apply halves of the borders. The other halves are applied on a Cell level
            layoutBox.ApplyMargins<Rectangle>(0, rightBorderMaxWidth / 2, 0, leftBorderMaxWidth / 2, false);
            // Table should have a row and some child elements in order to be considered non empty
            if (!tableModel.IsEmpty() && 0 != rows.Count) {
                layoutBox.DecreaseHeight(topTableBorderWidth / 2);
                occupiedArea.GetBBox().MoveDown(topTableBorderWidth / 2).IncreaseHeight(topTableBorderWidth / 2);
            }
            else {
                if (tableModel.IsComplete() && 0 == lastFlushedRowBottomBorder.Count) {
                    // process empty table
                    layoutBox.DecreaseHeight(topTableBorderWidth);
                    occupiedArea.GetBBox().MoveDown(topTableBorderWidth).IncreaseHeight(topTableBorderWidth);
                }
            }
            LayoutResult[] splits = new LayoutResult[numberOfColumns];
            // This represents the target row index for the overflow renderer to be placed to.
            // Usually this is just the current row id of a cell, but it has valuable meaning when a cell has rowspan.
            int[] targetOverflowRowIndex = new int[numberOfColumns];
            for (row = 0; row < rows.Count; row++) {
                // if forced placement was earlier set, this means the element did not fit into the area, and in this case
                // we only want to place the first row in a forced way, not the next ones, otherwise they will be invisible
                if (row == 1 && true.Equals(this.GetProperty<bool?>(Property.FORCED_PLACEMENT))) {
                    if (true.Equals(this.GetOwnProperty<bool?>(Property.FORCED_PLACEMENT))) {
                        DeleteOwnProperty(Property.FORCED_PLACEMENT);
                    }
                    else {
                        SetProperty(Property.FORCED_PLACEMENT, false);
                    }
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
                bottomTableBorderWidth = 0;
                Border widestRowBottomBorder = bordersHandler.GetWidestBorder(row + 1);
                // if cell is in the last row on the page, its borders shouldn't collapse with the next row borders
                bool processAsLast = false;
                while (cellProcessingQueue.Count > 0) {
                    TableRenderer.CellRendererInfo currentCellInfo = cellProcessingQueue.JRemoveFirst();
                    col = currentCellInfo.column;
                    CellRenderer cell = currentCellInfo.cellRenderer;
                    int colspan = (int)cell.GetPropertyAsInteger(Property.COLSPAN);
                    int rowspan = (int)cell.GetPropertyAsInteger(Property.ROWSPAN);
                    if (1 != rowspan) {
                        cellWithBigRowspanAdded = true;
                    }
                    targetOverflowRowIndex[col] = currentCellInfo.finishRowInd;
                    // This cell came from the future (split occurred and we need to place cell with big rowpsan into the current area)
                    bool currentCellHasBigRowspan = (row != currentCellInfo.finishRowInd);
                    float cellWidth = 0;
                    float colOffset = 0;
                    for (int k = col; k < col + colspan; k++) {
                        cellWidth += countedColumnWidth[k];
                    }
                    for (int l = 0; l < col; l++) {
                        colOffset += countedColumnWidth[l];
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
                    Border oldBottomBorder = cell.GetBorders()[2];
                    Border collapsedBottomBorder = TableBorders.GetCollapsedBorder(oldBottomBorder, footerRenderer != null ? footerRenderer
                        .bordersHandler.horizontalBorders[0][col] : borders[2]);
                    if (collapsedBottomBorder != null) {
                        float diff = Math.Max(collapsedBottomBorder.GetWidth(), null != widestRowBottomBorder ? widestRowBottomBorder
                            .GetWidth() : 0);
                        cellArea.GetBBox().MoveUp(diff / 2).DecreaseHeight(diff / 2);
                        cell.SetProperty(Property.BORDER_BOTTOM, collapsedBottomBorder);
                    }
                    LayoutResult cellResult = cell.SetParent(this).Layout(new LayoutContext(cellArea));
                    // we need to disable collapsing with the next row
                    if (!processAsLast && LayoutResult.NOTHING == cellResult.GetStatus()) {
                        processAsLast = true;
                        // undo collapsing with the next row
                        widestRowBottomBorder = null;
                        for (int tempCol = 0; tempCol < currentRow.Length; tempCol++) {
                            if (null != currentRow[tempCol]) {
                                currentRow[tempCol].DeleteOwnProperty(Property.BORDER_BOTTOM);
                                oldBottomBorder = currentRow[tempCol].GetBorders()[2];
                                if (null != oldBottomBorder && (null == widestRowBottomBorder || widestRowBottomBorder.GetWidth() > oldBottomBorder
                                    .GetWidth())) {
                                    widestRowBottomBorder = oldBottomBorder;
                                }
                            }
                        }
                        cellProcessingQueue.Clear();
                        for (int addCol = 0; addCol < currentRow.Length; addCol++) {
                            if (currentRow[addCol] != null) {
                                cellProcessingQueue.AddLast(new TableRenderer.CellRendererInfo(currentRow[addCol], addCol, row));
                            }
                        }
                        continue;
                    }
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
                                if (null != footerRenderer && tableModel.IsSkipLastFooter() && tableModel.IsComplete()) {
                                }
                                //                                LayoutArea potentialArea = new LayoutArea(area.getPageNumber(), layoutBox.clone());
                                //                                // Fix layout area
                                //                                Border widestRowTopBorder = bordersHandler.getWidestBorder(row);
                                //                                if (null != widestRowTopBorder) {
                                //                                    potentialArea.getBBox().moveDown(widestRowTopBorder.getWidth() / 2).increaseHeight(widestRowTopBorder.getWidth() / 2);
                                //                                }
                                //                                float footerHeight = footerRenderer.getOccupiedArea().getBBox().getHeight();
                                //                                potentialArea.getBBox().moveDown(footerHeight).increaseHeight(footerHeight);
                                //
                                //                                TableRenderer overflowRenderer = createOverflowRenderer(new Table.RowRange(rowRange.getStartRow() + row, rowRange.getFinishRow()));
                                //                                overflowRenderer.rows = rows.subList(row, rows.size());
                                //                                overflowRenderer.setProperty(Property.IGNORE_HEADER, true);
                                //                                overflowRenderer.setProperty(Property.IGNORE_FOOTER, true);
                                //                                overflowRenderer.setProperty(Property.MARGIN_TOP, 0);
                                //                                overflowRenderer.setProperty(Property.MARGIN_BOTTOM, 0);
                                //                                overflowRenderer.setProperty(Property.MARGIN_LEFT, 0);
                                //                                overflowRenderer.setProperty(Property.MARGIN_RIGHT, 0);
                                //                                // init borders
                                //                                overflowRenderer.initializeBorders(new ArrayList<Border>(), true);
                                //                                overflowRenderer.collapseAllBordersAndEmptyRows(overflowRenderer.getBorders(), 0, rowRange.getFinishRow() - rowRange.getStartRow() - row, numberOfColumns);
                                //                                prepareFooterOrHeaderRendererForLayout(overflowRenderer, layoutBox.getWidth());
                                //                                if (LayoutResult.FULL == overflowRenderer.layout(new LayoutContext(potentialArea)).getStatus()) {
                                //                                    footerRenderer = null;
                                //                                    // fix layout area and table bottom border
                                //                                    layoutBox.increaseHeight(footerHeight).moveDown(footerHeight);
                                //                                    deleteOwnProperty(Property.BORDER_BOTTOM);
                                //                                    borders = getBorders();
                                //                                    processAsLast = false;
                                //                                    cellProcessingQueue.clear();
                                //                                    for (addCol = 0; addCol < currentRow.length; addCol++) {
                                //                                        if (currentRow[addCol] != null) {
                                //                                            cellProcessingQueue.addLast(new CellRendererInfo(currentRow[addCol], addCol, row));
                                //                                        }
                                //                                    }
                                //                                    // build borders of current row cells and find the widest one
                                //                                    for (CellRendererInfo curCellInfo : cellProcessingQueue) {
                                //                                        col = curCellInfo.column;
                                //                                        cell = curCellInfo.cellRenderer;
                                //                                        prepareBuildingBordersArrays(cell, borders, tableModel.getNumberOfColumns(), row, col);
                                //                                        buildBordersArrays(cell, curCellInfo.finishRowInd, col);
                                //                                    }
                                //                                    continue;
                                //                                }
                                // Here we look for a cell with big rowspan (i.e. one which would not be normally processed in
                                // the scope of this row), and we add such cells to the queue, because we need to write them
                                // at least partially into the available area we have.
                                for (addCol = 0; addCol < currentRow.Length; addCol++) {
                                    if (currentRow[addCol] == null) {
                                        // Search for the next cell including rowspan.
                                        for (int addRow = row + 1; addRow < rows.Count; addRow++) {
                                            if (rows[addRow][addCol] != null) {
                                                CellRenderer addRenderer = rows[addRow][addCol];
                                                // TODO DEVSIX-1060
                                                verticalAlignment = addRenderer.GetProperty<VerticalAlignment?>(Property.VERTICAL_ALIGNMENT);
                                                //                                            if (verticalAlignment != null && verticalAlignment.equals(VerticalAlignment.BOTTOM)) {
                                                //                                                if (row + addRenderer.getPropertyAsInteger(Property.ROWSPAN) - 1 < addRow) {
                                                //                                                    cellProcessingQueue.addLast(new CellRendererInfo(addRenderer, addCol, addRow));
                                                //                                                } else {
                                                //                                                    horizontalBorders.get(row + 1).set(addCol, addRenderer.getBorders()[2]);
                                                //                                                    if (addCol == 0) {
                                                //                                                        for (int i = row; i >= 0; i--) {
                                                //                                                            if (!checkAndReplaceBorderInArray(verticalBorders, addCol, i, addRenderer.getBorders()[3], false)) {
                                                //                                                                break;
                                                //                                                            }
                                                //                                                        }
                                                //                                                    } else if (addCol == numberOfColumns - 1) {
                                                //                                                        for (int i = row; i >= 0; i--) {
                                                //                                                            if (!checkAndReplaceBorderInArray(verticalBorders, addCol + 1, i, addRenderer.getBorders()[1], true)) {
                                                //                                                                break;
                                                //                                                            }
                                                //                                                        }
                                                //                                                    }
                                                //                                                }
                                                //                                            } else
                                                if (row + addRenderer.GetPropertyAsInteger(Property.ROWSPAN) - 1 >= addRow) {
                                                    cellProcessingQueue.AddLast(new TableRenderer.CellRendererInfo(addRenderer, addCol, addRow));
                                                }
                                                break;
                                            }
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
                // maybe the table was incomplete and we can process the footer
                if (null != footerRenderer && (0 != lastFlushedRowBottomBorder.Count || !tableModel.IsComplete()) && !hasContent
                     && 0 == childRenderers.Count) {
                    layoutBox.IncreaseHeight(occupiedArea.GetBBox().GetHeight());
                    occupiedArea.GetBBox().MoveUp(occupiedArea.GetBBox().GetHeight()).SetHeight(0);
                }
                if (hasContent) {
                    heights.Add(rowHeight);
                    occupiedArea.GetBBox().MoveDown(rowHeight);
                    occupiedArea.GetBBox().IncreaseHeight(rowHeight);
                    layoutBox.DecreaseHeight(rowHeight);
                }
                if (row == rows.Count - 1 && null != footerRenderer && tableModel.IsComplete() && tableModel.IsSkipLastFooter
                    () && !split && !processAsLast) {
                    footerRenderer = null;
                    // delete #layout() related properties
                    DeleteOwnProperty(Property.BORDER_BOTTOM);
                    if (tableModel.IsEmpty()) {
                        this.DeleteOwnProperty(Property.BORDER_TOP);
                    }
                    borders = GetBorders();
                }
                if (split || processAsLast || row == rows.Count - 1) {
                    //if (split) {
                    horizontalBordersIndexOffset = bordersHandler.GetCurrentHorizontalBordersIndexOffset();
                    verticalBordersIndexOffset = bordersHandler.GetCurrentVerticalBordersIndexOffset();
                    //                    bordersHandler.updateTopBorder();
                    bordersHandler.ProcessSplit(row, hasContent, cellWithBigRowspanAdded);
                    //}
                    // Correct layout area of the last row rendered on the page
                    if (heights.Count != 0) {
                        rowHeight = 0;
                        // TODO
                        //                    if (split && (hasContent)) {
                        //                        bordersHandler.addNewBorder(row + 1);
                        //                    }
                        for (col = 0; col < currentRow.Length; col++) {
                            if (hasContent || (cellWithBigRowspanAdded && null == rows[row - 1][col])) {
                                if (null != currentRow[col]) {
                                    cellProcessingQueue.AddLast(new TableRenderer.CellRendererInfo(currentRow[col], col, row));
                                }
                            }
                            else {
                                if (null != rows[row - 1][col]) {
                                    cellProcessingQueue.AddLast(new TableRenderer.CellRendererInfo(rows[row - 1][col], col, row - 1));
                                }
                            }
                        }
                        while (0 != cellProcessingQueue.Count) {
                            TableRenderer.CellRendererInfo cellInfo = cellProcessingQueue.JRemoveFirst();
                            col = cellInfo.column;
                            int rowN = cellInfo.finishRowInd;
                            CellRenderer cell = cellInfo.cellRenderer;
                            float collapsedWithNextRowBorderWidth = null == cell.GetBorders()[2] ? 0 : cell.GetBorders()[2].GetWidth();
                            cell.DeleteOwnProperty(Property.BORDER_BOTTOM);
                            Border cellOwnBottomBorder = cell.GetBorders()[2];
                            Border collapsedWithTableBorder = TableBorders.GetCollapsedBorder(cellOwnBottomBorder, footerRenderer != null
                                 ? footerRenderer.bordersHandler.horizontalBorders[0][col] : borders[2]);
                            if (null != collapsedWithTableBorder && bottomTableBorderWidth < collapsedWithTableBorder.GetWidth()) {
                                bottomTableBorderWidth = collapsedWithTableBorder.GetWidth();
                            }
                            float collapsedBorderWidth = null == collapsedWithTableBorder ? 0 : collapsedWithTableBorder.GetWidth();
                            if (collapsedWithNextRowBorderWidth != collapsedBorderWidth) {
                                cell.SetBorders(collapsedWithTableBorder, 2);
                                // TODO
                                //                            for (int i = col; i < col + cell.getPropertyAsInteger(Property.COLSPAN); i++) {
                                //                                bordersHandler.horizontalBorders.get(rowN + 1).set(i, collapsedWithTableBorder);
                                //                            }
                                // apply the difference between collapsed table border and own cell border
                                cell.occupiedArea.GetBBox().MoveDown((collapsedBorderWidth - collapsedWithNextRowBorderWidth) / 2).IncreaseHeight
                                    ((collapsedBorderWidth - collapsedWithNextRowBorderWidth) / 2);
                            }
                            // fix row height
                            int cellRowStartIndex = ((Cell)cell.GetModelElement()).GetRow();
                            float rowspanOffset = 0;
                            for (int l = cellRowStartIndex; l < heights.Count - 1; l++) {
                                rowspanOffset += heights[l];
                            }
                            if (cell.occupiedArea.GetBBox().GetHeight() > rowHeight + rowspanOffset) {
                                rowHeight = cell.occupiedArea.GetBBox().GetHeight() - rowspanOffset;
                            }
                        }
                        if (rowHeight != heights[heights.Count - 1]) {
                            float heightDiff = rowHeight - heights[heights.Count - 1];
                            heights[heights.Count - 1] = rowHeight;
                            occupiedArea.GetBBox().MoveDown(heightDiff).IncreaseHeight(heightDiff);
                            layoutBox.DecreaseHeight(heightDiff);
                        }
                    }
                    else {
                        if (null != borders[2]) {
                        }
                    }
                    // TODO
                    //                        for (col = 0; col < numberOfColumns; col++) {
                    //                            if (null == bordersHandler.horizontalBorders.get(1).get(col) || bordersHandler.horizontalBorders.get(1).get(col).getWidth() < borders[2].getWidth()) {
                    //                                bordersHandler.horizontalBorders.get(1).set(col, borders[2]);
                    //                            }
                    //                        }
                    // Correct occupied areas of all added cells
                    CorrectCellsOccupiedAreas(splits, row, targetOverflowRowIndex);
                }
                // process footer with collapsed borders
                if ((split || processAsLast || row == rows.Count - 1) && null != footerRenderer) {
                    int lastRow;
                    if (hasContent || cellWithBigRowspanAdded) {
                        lastRow = row + 1;
                    }
                    else {
                        lastRow = row;
                    }
                    bool[] useFooterBorders = new bool[numberOfColumns];
                    if (!tableModel.IsEmpty()) {
                        useFooterBorders = bordersHandler.CollapseFooterBorders(0 != lastFlushedRowBottomBorder.Count && 0 == row ? 
                            lastFlushedRowBottomBorder : bordersHandler.horizontalBorders[lastRow], numberOfColumns, rows.Count);
                        layoutBox.IncreaseHeight(bottomTableBorderWidth / 2);
                        occupiedArea.GetBBox().MoveUp(bottomTableBorderWidth / 2).DecreaseHeight(bottomTableBorderWidth / 2);
                    }
                    footerRenderer.ProcessRendererBorders(numberOfColumns);
                    layoutBox.MoveDown(footerRenderer.occupiedArea.GetBBox().GetHeight()).IncreaseHeight(footerRenderer.occupiedArea
                        .GetBBox().GetHeight());
                    // apply the difference to set footer and table left/right margins identical
                    layoutBox.ApplyMargins<Rectangle>(0, -rightBorderMaxWidth / 2, 0, -leftBorderMaxWidth / 2, false);
                    PrepareFooterOrHeaderRendererForLayout(footerRenderer, layoutBox.GetWidth());
                    footerRenderer.Layout(new LayoutContext(new LayoutArea(area.GetPageNumber(), layoutBox)));
                    layoutBox.ApplyMargins<Rectangle>(0, -rightBorderMaxWidth / 2, 0, -leftBorderMaxWidth / 2, true);
                    float footerHeight = footerRenderer.GetOccupiedAreaBBox().GetHeight();
                    footerRenderer.Move(0, -(layoutBox.GetHeight() - footerHeight));
                    layoutBox.SetY(footerRenderer.occupiedArea.GetBBox().GetTop()).SetHeight(occupiedArea.GetBBox().GetBottom(
                        ) - layoutBox.GetBottom());
                    // fix footer borders
                    if (!tableModel.IsEmpty()) {
                        bordersHandler.FixFooterBorders(0 != lastFlushedRowBottomBorder.Count && 0 == row ? lastFlushedRowBottomBorder
                             : bordersHandler.horizontalBorders[lastRow], numberOfColumns, rows.Count, useFooterBorders);
                    }
                }
                if (!split) {
                    childRenderers.AddAll(currChildRenderers);
                    currChildRenderers.Clear();
                }
                if (split || processAsLast) {
                    if (marginsCollapsingEnabled) {
                        marginsCollapseHandler.EndMarginsCollapse(layoutBox);
                    }
                    iText.Layout.Renderer.TableRenderer[] splitResult = !split && processAsLast ? Split(row + 1, false, cellWithBigRowspanAdded
                        ) : Split(row, hasContent, cellWithBigRowspanAdded);
                    // delete #layout() related properties
                    if (null != headerRenderer || null != footerRenderer) {
                        if (null != headerRenderer || tableModel.IsEmpty()) {
                            splitResult[1].DeleteOwnProperty(Property.BORDER_TOP);
                        }
                        if (null != footerRenderer || tableModel.IsEmpty()) {
                            splitResult[1].DeleteOwnProperty(Property.BORDER_BOTTOM);
                        }
                    }
                    if (split) {
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
                                    CellRenderer cellOverflow = (CellRenderer)splits[col].GetOverflowRenderer();
                                    cellOverflow.DeleteOwnProperty(Property.BORDER_BOTTOM);
                                    cellOverflow.DeleteOwnProperty(Property.BORDER_TOP);
                                    // TODO
                                    //                                if (null != cellSplit) {
                                    //                                    for (int j = col; j < col + cellOverflow.getPropertyAsInteger(Property.COLSPAN); j++) {
                                    //                                        splitResult[0].horizontalBorders.get(row + (hasContent ? 1 : 0)).set(j, currentRow[col].getBorders()[2]);
                                    //                                    }
                                    //                                }
                                    currentRow[col] = null;
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
                                    if (hasContent) {
                                        rowspans[col] = ((Cell)currentRow[col].GetModelElement()).GetRowspan();
                                    }
                                    bool isBigRowspannedCell = 1 != ((Cell)currentRow[col].GetModelElement()).GetRowspan();
                                    if (hasContent || isBigRowspannedCell) {
                                        columnsWithCellToBeEnlarged[col] = true;
                                        if (isBigRowspannedCell && !processAsLast) {
                                            childRenderers.Add(currentRow[col]);
                                        }
                                    }
                                    else {
                                        if (Border.NO_BORDER != currentRow[col].GetProperty<Border>(Property.BORDER_TOP)) {
                                            splitResult[1].rows[0][col].DeleteOwnProperty(Property.BORDER_TOP);
                                        }
                                    }
                                }
                            }
                        }
                        // TODO
                        //                            for (int j = col; j < col + currentRow[col].getPropertyAsInteger(Property.COLSPAN); j++) {
                        //                                horizontalBorders.get(row + (hasContent ? 1 : 0)).set(j, currentRow[col].getBorders()[2]);
                        //                            }
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
                                    Cell overflowCell = ((Cell)currentRow[col].GetModelElement()).Clone(true);
                                    // we will change properties
                                    currentRow[col].isLastRendererForModelElement = false;
                                    childRenderers.Add(currentRow[col]);
                                    currentRow[col] = null;
                                    rows[targetOverflowRowIndex[col]][col] = (CellRenderer)overflowCell.GetRenderer().SetParent(this);
                                    rows[targetOverflowRowIndex[col]][col].DeleteProperty(Property.HEIGHT);
                                    rows[targetOverflowRowIndex[col]][col].DeleteProperty(Property.MIN_HEIGHT);
                                    rows[targetOverflowRowIndex[col]][col].DeleteProperty(Property.MAX_HEIGHT);
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
                                        rows[i][col].isLastRendererForModelElement = false;
                                        rows[i][col] = null;
                                        rows[targetOverflowRowIndex[col]][col] = (CellRenderer)overflowCell.GetRenderer().SetParent(this);
                                    }
                                }
                                rows[targetOverflowRowIndex[col]][col].occupiedArea = cellOccupiedArea;
                            }
                        }
                    }
                    // Apply borders if there is no footer
                    if (null == footerRenderer) {
                        if (0 != this.childRenderers.Count) {
                            occupiedArea.GetBBox().MoveDown(bottomTableBorderWidth / 2).IncreaseHeight(bottomTableBorderWidth / 2);
                            layoutBox.DecreaseHeight(bottomTableBorderWidth / 2);
                        }
                        else {
                            occupiedArea.GetBBox().MoveUp(topTableBorderWidth / 2).DecreaseHeight(topTableBorderWidth / 2);
                            layoutBox.IncreaseHeight(topTableBorderWidth / 2);
                            // process bottom border of the last added row if there is no footer
                            if (!tableModel.IsComplete() || 0 != lastFlushedRowBottomBorder.Count) {
                                bottomTableBorderWidth = null == widestLustFlushedBorder ? 0f : widestLustFlushedBorder.GetWidth();
                                occupiedArea.GetBBox().MoveDown(bottomTableBorderWidth).IncreaseHeight(bottomTableBorderWidth);
                                // TODO
                                //                            splitResult[0].horizontalBorders.clear();
                                //                            splitResult[0].horizontalBorders.add(lastFlushedRowBottomBorder);
                                // hack to process 'margins'
                                splitResult[0].SetBorders(widestLustFlushedBorder, 2);
                                splitResult[0].SetBorders(Border.NO_BORDER, 0);
                            }
                        }
                    }
                    // TODO
                    //                            if (0 != splitResult[0].verticalBorders.size()) {
                    //                                splitResult[0].setBorders(splitResult[0].verticalBorders.get(0).get(0), 3);
                    //                                splitResult[0].setBorders(splitResult[0].verticalBorders.get(verticalBorders.size() - 1).get(0), 1);
                    //                            }
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
                    if ((IsKeepTogether() && 0 == lastFlushedRowBottomBorder.Count) && !true.Equals(GetPropertyAsBoolean(Property
                        .FORCED_PLACEMENT))) {
                        return new LayoutResult(LayoutResult.NOTHING, null, null, this, null == firstCauseOfNothing ? this : firstCauseOfNothing
                            );
                    }
                    else {
                        int status = ((occupiedArea.GetBBox().GetHeight() - (null == footerRenderer ? 0 : footerRenderer.GetOccupiedArea
                            ().GetBBox().GetHeight()) == 0) && (tableModel.IsComplete() && 0 == lastFlushedRowBottomBorder.Count))
                             ? LayoutResult.NOTHING : LayoutResult.PARTIAL;
                        if ((status == LayoutResult.NOTHING && true.Equals(GetPropertyAsBoolean(Property.FORCED_PLACEMENT))) || wasHeightClipped
                            ) {
                            if (wasHeightClipped) {
                                ILogger logger = LoggerFactory.GetLogger(typeof(iText.Layout.Renderer.TableRenderer));
                                logger.Warn(iText.IO.LogMessageConstant.CLIP_ELEMENT);
                                // Process borders
                                if (status == LayoutResult.NOTHING) {
                                    List<Border> topBorders = new List<Border>();
                                    List<Border> bottomBorders = new List<Border>();
                                    for (int i = 0; i < numberOfColumns; i++) {
                                        topBorders.Add(borders[0]);
                                        bottomBorders.Add(borders[2]);
                                    }
                                    // TODO
                                    //                                horizontalBorders.clear();
                                    //                                horizontalBorders.add(topBorders);
                                    //                                horizontalBorders.add(bottomBorders);
                                    float bordersWidth = (null == borders[0] ? 0 : borders[0].GetWidth()) + (null == borders[2] ? 0 : borders[
                                        2].GetWidth());
                                    occupiedArea.GetBBox().MoveDown(bordersWidth).IncreaseHeight(bordersWidth);
                                }
                                // Notice that we extend the table only on the current page
                                if (null != blockMinHeight && blockMinHeight > occupiedArea.GetBBox().GetHeight()) {
                                    float blockBottom = Math.Max(occupiedArea.GetBBox().GetBottom() - ((float)blockMinHeight - occupiedArea.GetBBox
                                        ().GetHeight()), layoutBox.GetBottom());
                                    if (0 == heights.Count) {
                                        heights.Add(((float)blockMinHeight) - occupiedArea.GetBBox().GetHeight() / 2);
                                    }
                                    else {
                                        heights[heights.Count - 1] = heights[heights.Count - 1] + ((float)blockMinHeight) - occupiedArea.GetBBox()
                                            .GetHeight();
                                    }
                                    occupiedArea.GetBBox().IncreaseHeight(occupiedArea.GetBBox().GetBottom() - blockBottom).SetY(blockBottom);
                                }
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
                    logger.Warn(iText.IO.LogMessageConstant.LAST_ROW_IS_NOT_COMPLETE);
                }
            }
            // process footer renderer with collapsed borders
            if (tableModel.IsComplete() && 0 != lastFlushedRowBottomBorder.Count && null != footerRenderer) {
                bool[] useFooterBorders = bordersHandler.CollapseFooterBorders(lastFlushedRowBottomBorder, numberOfColumns
                    , rows.Count);
                footerRenderer.ProcessRendererBorders(numberOfColumns);
                layoutBox.MoveDown(footerRenderer.occupiedArea.GetBBox().GetHeight()).IncreaseHeight(footerRenderer.occupiedArea
                    .GetBBox().GetHeight());
                // apply the difference to set footer and table left/right margins identical
                layoutBox.ApplyMargins<Rectangle>(0, -rightBorderMaxWidth / 2, 0, -leftBorderMaxWidth / 2, false);
                PrepareFooterOrHeaderRendererForLayout(footerRenderer, layoutBox.GetWidth());
                footerRenderer.Layout(new LayoutContext(new LayoutArea(area.GetPageNumber(), layoutBox)));
                layoutBox.ApplyMargins<Rectangle>(0, -rightBorderMaxWidth / 2, 0, -leftBorderMaxWidth / 2, true);
                float footerHeight = footerRenderer.GetOccupiedAreaBBox().GetHeight();
                footerRenderer.Move(0, -(layoutBox.GetHeight() - footerHeight));
                layoutBox.MoveUp(footerHeight).DecreaseHeight(footerHeight);
                // fix borders
                bordersHandler.FixFooterBorders(lastFlushedRowBottomBorder, numberOfColumns, rows.Count, useFooterBorders);
            }
            // if table is empty we still need to process table borders
            if (0 == childRenderers.Count && null == headerRenderer && null == footerRenderer) {
                IList<Border> topHorizontalBorders = new List<Border>();
                IList<Border> bottomHorizontalBorders = new List<Border>();
                for (int i = 0; i < numberOfColumns; i++) {
                    bottomHorizontalBorders.Add(Border.NO_BORDER);
                }
                IList<Border> leftVerticalBorders = new List<Border>();
                IList<Border> rightVerticalBorders = new List<Border>();
                // process bottom border of the last added row
                if (tableModel.IsComplete() && 0 != lastFlushedRowBottomBorder.Count) {
                    bottomHorizontalBorders = lastFlushedRowBottomBorder;
                    // hack to process 'margins'
                    SetBorders(widestLustFlushedBorder, 2);
                    SetBorders(Border.NO_BORDER, 0);
                }
                // collapse with table bottom border
                for (int i = 0; i < bottomHorizontalBorders.Count; i++) {
                    Border border = bottomHorizontalBorders[i];
                    if (null == border || (null != borders[2] && border.GetWidth() < borders[2].GetWidth())) {
                        bottomHorizontalBorders[i] = borders[2];
                    }
                    topHorizontalBorders.Add(borders[0]);
                }
            }
            // TODO
            //            horizontalBorders.set(0, topHorizontalBorders);
            //            horizontalBorders.add(bottomHorizontalBorders);
            //            leftVerticalBorders.add(borders[3]);
            //            rightVerticalBorders.add(borders[1]);
            //            verticalBorders = new ArrayList<>();
            //            verticalBorders.add(leftVerticalBorders);
            //            for (int i = 0; i < numberOfColumns - 1; i++) {
            //                verticalBorders.add(new ArrayList<Border>());
            //            }
            //            verticalBorders.add(rightVerticalBorders);
            // Apply bottom and top border
            if (tableModel.IsComplete()) {
                if (null == footerRenderer) {
                    if (0 != childRenderers.Count) {
                        occupiedArea.GetBBox().MoveDown(bottomTableBorderWidth / 2).IncreaseHeight((bottomTableBorderWidth) / 2);
                        layoutBox.DecreaseHeight(bottomTableBorderWidth / 2);
                    }
                    else {
                        if (0 != lastFlushedRowBottomBorder.Count) {
                            if (null != widestLustFlushedBorder && widestLustFlushedBorder.GetWidth() > bottomTableBorderWidth) {
                                bottomTableBorderWidth = widestLustFlushedBorder.GetWidth();
                            }
                        }
                        occupiedArea.GetBBox().MoveDown(bottomTableBorderWidth).IncreaseHeight((bottomTableBorderWidth));
                        layoutBox.DecreaseHeight(bottomTableBorderWidth);
                    }
                }
            }
            else {
                // the bottom border should be processed and placed lately
                if (0 != heights.Count) {
                    // TODO
                    //                horizontalBorders.get(horizontalBorders.size() - 1).clear();
                    heights[heights.Count - 1] = heights[heights.Count - 1] - bottomTableBorderWidth / 2;
                }
                if (null == footerRenderer) {
                    if (0 != childRenderers.Count) {
                        occupiedArea.GetBBox().MoveUp(bottomTableBorderWidth / 2).DecreaseHeight((bottomTableBorderWidth / 2));
                        layoutBox.IncreaseHeight(bottomTableBorderWidth / 2);
                    }
                }
                else {
                    // occupied area is right here
                    layoutBox.IncreaseHeight(bottomTableBorderWidth);
                }
            }
            if ((true.Equals(GetPropertyAsBoolean(Property.FILL_AVAILABLE_AREA))) && 0 != rows.Count) {
                ExtendLastRow(rows[rows.Count - 1], layoutBox);
            }
            if (null != blockMinHeight && blockMinHeight > occupiedArea.GetBBox().GetHeight()) {
                float blockBottom = Math.Max(occupiedArea.GetBBox().GetBottom() - ((float)blockMinHeight - occupiedArea.GetBBox
                    ().GetHeight()), layoutBox.GetBottom());
                if (0 != heights.Count) {
                    heights[heights.Count - 1] = heights[heights.Count - 1] + occupiedArea.GetBBox().GetBottom() - blockBottom;
                }
                else {
                    heights.Add((occupiedArea.GetBBox().GetBottom() - blockBottom) + occupiedArea.GetBBox().GetHeight() / 2);
                }
                occupiedArea.GetBBox().IncreaseHeight(occupiedArea.GetBBox().GetBottom() - blockBottom).SetY(blockBottom);
            }
            if (IsPositioned()) {
                float y = (float)this.GetPropertyAsFloat(Property.Y);
                float relativeY = IsFixedLayout() ? 0 : layoutBox.GetY();
                Move(0, relativeY + y - occupiedArea.GetBBox().GetY());
            }
            if (marginsCollapsingEnabled) {
                marginsCollapseHandler.EndMarginsCollapse(layoutBox);
            }
            ApplyMargins(occupiedArea.GetBBox(), true);
            // if table is empty or is not complete we should delete footer
            if ((tableModel.IsSkipLastFooter() || !tableModel.IsComplete()) && null != footerRenderer) {
                if (0 != lastFlushedRowBottomBorder.Count) {
                    if (null != widestLustFlushedBorder && widestLustFlushedBorder.GetWidth() > bottomTableBorderWidth) {
                        bottomTableBorderWidth = widestLustFlushedBorder.GetWidth();
                    }
                    // hack to process 'margins'
                    SetBorders(widestLustFlushedBorder, 0);
                }
                footerRenderer = null;
                if (tableModel.IsComplete()) {
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
            return Split(row, false, false);
        }

        protected internal virtual iText.Layout.Renderer.TableRenderer[] Split(int row, bool hasContent, bool cellWithBigRowspanAdded
            ) {
            iText.Layout.Renderer.TableRenderer splitRenderer = CreateSplitRenderer(new Table.RowRange(rowRange.GetStartRow
                (), rowRange.GetStartRow() + row));
            splitRenderer.rows = rows.SubList(0, row);
            int rowN = row;
            if (hasContent) {
                rowN++;
            }
            splitRenderer.bordersHandler = bordersHandler;
            // TODO set rowrange or something
            //        splitRenderer.horizontalBorders = new ArrayList<>();
            //        //splitRenderer.horizontalBorders.addAll(horizontalBorders);
            //        for (int i = 0; i <= rowN; i++) {
            //            splitRenderer.horizontalBorders.add(horizontalBorders.get(i));
            //        }
            //        splitRenderer.verticalBorders = new ArrayList<>();
            //        //splitRenderer.verticalBorders.addAll(verticalBorders);
            //        for (int i = 0; i < verticalBorders.size(); i++) {
            //            splitRenderer.verticalBorders.add(new ArrayList<Border>());
            //            for (int j = 0; j < ((0 == rowN) ? 1 : rowN); j++) {
            //                if (verticalBorders.get(i).size() != 0) {
            //                    splitRenderer.verticalBorders.get(i).add(verticalBorders.get(i).get(j));
            //                }
            //            }
            //        }
            splitRenderer.heights = heights;
            splitRenderer.columnWidths = columnWidths;
            splitRenderer.countedColumnWidth = countedColumnWidth;
            splitRenderer.totalWidthForColumns = totalWidthForColumns;
            iText.Layout.Renderer.TableRenderer overflowRenderer = CreateOverflowRenderer(new Table.RowRange(rowRange.
                GetStartRow() + row, rowRange.GetFinishRow()));
            if (0 == row && !(hasContent || cellWithBigRowspanAdded)) {
                overflowRenderer.isOriginalNonSplitRenderer = true;
            }
            overflowRenderer.rows = rows.SubList(row, rows.Count);
            splitRenderer.occupiedArea = occupiedArea;
            // TODO set rowrange or something
            //        overflowRenderer.horizontalBorders = new ArrayList<>();
            //        //splitRenderer.horizontalBorders.addAll(horizontalBorders);
            //        for (int i = rowN; i < horizontalBorders.size(); i++) {
            //            //TODO
            //            overflowRenderer.horizontalBorders.add((List<Border>) ((ArrayList<Border>) horizontalBorders.get(i)).clone());
            //        }
            //        overflowRenderer.verticalBorders = new ArrayList<>();
            //        //splitRenderer.verticalBorders.addAll(verticalBorders);
            //        for (int i = 0; i < verticalBorders.size(); i++) {
            //            overflowRenderer.verticalBorders.add(new ArrayList<Border>());
            //            for (int j = row; j < verticalBorders.get(i).size(); j++) {
            //                if (verticalBorders.get(i).size() != 0) {
            //                    overflowRenderer.verticalBorders.get(i).add(verticalBorders.get(i).get(j));
            //                }
            //            }
            //        }
            overflowRenderer.bordersHandler = bordersHandler;
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
            overflowRenderer.countedColumnWidth = this.countedColumnWidth;
            overflowRenderer.leftBorderMaxWidth = this.leftBorderMaxWidth;
            overflowRenderer.rightBorderMaxWidth = this.rightBorderMaxWidth;
            return overflowRenderer;
        }

        protected internal override float? RetrieveWidth(float parentBoxWidth) {
            float? tableWidth = base.RetrieveWidth(parentBoxWidth);
            Table tableModel = (Table)GetModelElement();
            if (tableWidth == null || tableWidth == 0) {
                float totalColumnWidthInPercent = 0;
                for (int col = 0; col < tableModel.GetNumberOfColumns(); col++) {
                    UnitValue columnWidth = tableModel.GetColumnWidth(col);
                    if (columnWidth.IsPercentValue()) {
                        totalColumnWidthInPercent += columnWidth.GetValue();
                    }
                }
                tableWidth = parentBoxWidth;
                if (totalColumnWidthInPercent > 0) {
                    tableWidth = parentBoxWidth * totalColumnWidthInPercent / 100;
                }
            }
            return tableWidth;
        }

        internal override MinMaxWidth GetMinMaxWidth(float availableWidth) {
            return CountTableMinMaxWidth(availableWidth, true, false).ToTableMinMaxWidth(availableWidth);
        }

        private TableRenderer.ColumnMinMaxWidth CountTableMinMaxWidth(float availableWidth, bool initializeBorders
            , bool isTableBeingLayouted) {
            Rectangle layoutBox = new Rectangle(availableWidth, AbstractRenderer.INF);
            float? tableWidth = RetrieveWidth(layoutBox.GetWidth());
            ApplyMargins(layoutBox, false);
            if (initializeBorders) {
                // FIXME
                bordersHandler.InitializeBorders(((Table)GetModelElement()).GetLastRowBottomBorder(), true);
                InitializeHeaderAndFooter(true);
                if (!isTableBeingLayouted) {
                    SaveCellsProperties();
                }
                CollapseAllBorders();
            }
            TableRenderer.ColumnMinMaxWidth footerColWidth = null;
            if (footerRenderer != null) {
                footerColWidth = footerRenderer.CountRegionMinMaxWidth(availableWidth - leftBorderMaxWidth / 2 - rightBorderMaxWidth
                     / 2, null, null);
            }
            TableRenderer.ColumnMinMaxWidth headerColWidth = null;
            if (headerRenderer != null) {
                headerColWidth = headerRenderer.CountRegionMinMaxWidth(availableWidth - leftBorderMaxWidth / 2 - rightBorderMaxWidth
                     / 2, null, null);
            }
            // Apply halves of the borders. The other halves are applied on a Cell level
            layoutBox.ApplyMargins<Rectangle>(0, rightBorderMaxWidth / 2, 0, leftBorderMaxWidth / 2, false);
            tableWidth -= rightBorderMaxWidth / 2 + leftBorderMaxWidth / 2;
            TableRenderer.ColumnMinMaxWidth tableColWidth = CountRegionMinMaxWidth(tableWidth, headerColWidth, footerColWidth
                );
            countedMaxColumnWidth = tableColWidth.maxWidth;
            countedMinColumnWidth = tableColWidth.minWidth;
            if (initializeBorders) {
                footerRenderer = null;
                headerRenderer = null;
                rightBorderMaxWidth = 0;
                leftBorderMaxWidth = 0;
                // TODO
                //            horizontalBorders = null;
                //            verticalBorders = null;
                if (!isTableBeingLayouted) {
                    RestoreCellsProperties();
                }
                //TODO do we need it?
                // delete set properties
                DeleteOwnProperty(Property.BORDER_BOTTOM);
                DeleteOwnProperty(Property.BORDER_TOP);
            }
            return tableColWidth.SetLayoutBoxWidth(layoutBox.GetWidth());
        }

        private TableRenderer.ColumnMinMaxWidth CountRegionMinMaxWidth(float availableWidth, TableRenderer.ColumnMinMaxWidth
             headerWidth, TableRenderer.ColumnMinMaxWidth footerWidth) {
            Table tableModel = (Table)GetModelElement();
            int nrow = rows.Count;
            int ncol = tableModel.GetNumberOfColumns();
            MinMaxWidth[][] cellsMinMaxWidth = new MinMaxWidth[nrow][];
            int[][] cellsColspan = new int[nrow][];
            for (int i = 0; i < cellsMinMaxWidth.Length; i++) {
                cellsMinMaxWidth[i] = new MinMaxWidth[ncol];
                cellsColspan[i] = new int[ncol];
            }
            TableRenderer.ColumnMinMaxWidth result = new TableRenderer.ColumnMinMaxWidth(ncol);
            for (int row = 0; row < nrow; ++row) {
                for (int col = 0; col < ncol; ++col) {
                    CellRenderer cell = rows[row][col];
                    if (cell != null) {
                        cell.SetParent(this);
                        int colspan = (int)cell.GetPropertyAsInteger(Property.COLSPAN);
                        int rowspan = (int)cell.GetPropertyAsInteger(Property.ROWSPAN);
                        //We place the width of big cells in each row of in last column its occupied place and save it's colspan for convenience.
                        int finishCol = col + colspan - 1;
                        cellsMinMaxWidth[row][finishCol] = cell.GetMinMaxWidth(MinMaxWidthUtils.GetMax());
                        cellsColspan[row][finishCol] = colspan;
                        for (int i = 1; i < rowspan; ++i) {
                            cellsMinMaxWidth[row - i][finishCol] = cellsMinMaxWidth[row][finishCol];
                            cellsColspan[row - i][finishCol] = colspan;
                        }
                    }
                }
            }
            //The DP is used to count each column width.
            //In next two arrays at the index 'i' will be the sum of corresponding widths of first 'i' columns.
            float[] maxColumnsWidth = new float[ncol + 1];
            float[] minColumnsWidth = new float[ncol + 1];
            minColumnsWidth[0] = 0;
            maxColumnsWidth[0] = 0;
            int curColspan;
            for (int col = 0; col < ncol; ++col) {
                for (int row = 0; row < nrow; ++row) {
                    if (cellsMinMaxWidth[row][col] != null) {
                        curColspan = cellsColspan[row][col];
                        maxColumnsWidth[col + 1] = Math.Max(maxColumnsWidth[col + 1], cellsMinMaxWidth[row][col].GetMaxWidth() + maxColumnsWidth
                            [col - curColspan + 1]);
                        minColumnsWidth[col + 1] = Math.Max(minColumnsWidth[col + 1], cellsMinMaxWidth[row][col].GetMinWidth() + minColumnsWidth
                            [col - curColspan + 1]);
                    }
                    else {
                        maxColumnsWidth[col + 1] = Math.Max(maxColumnsWidth[col + 1], maxColumnsWidth[col]);
                        minColumnsWidth[col + 1] = Math.Max(minColumnsWidth[col + 1], minColumnsWidth[col]);
                    }
                }
            }
            for (int col = 0; col < ncol; ++col) {
                result.minWidth[col] = minColumnsWidth[col + 1] - minColumnsWidth[col];
                result.maxWidth[col] = maxColumnsWidth[col + 1] - maxColumnsWidth[col];
            }
            if (headerWidth != null) {
                result.MergeWith(headerWidth);
            }
            if (footerWidth != null) {
                result.MergeWith(footerWidth);
            }
            return result;
        }

        internal virtual float[] GetMinColumnWidth() {
            return countedMinColumnWidth;
        }

        internal virtual float[] GetMaxColumnWidth() {
            return countedMaxColumnWidth;
        }

        public override void DrawBorder(DrawContext drawContext) {
        }

        // Do nothing here. Itext7 handles cell and table borders collapse and draws result borders during #drawBorders()
        protected internal virtual void DrawBorders(DrawContext drawContext) {
            DrawBorders(drawContext, true, true);
        }

        protected internal virtual void DrawBorders(DrawContext drawContext, bool drawTop, bool drawBottom) {
            float height = occupiedArea.GetBBox().GetHeight();
            if (null != footerRenderer) {
                height -= footerRenderer.occupiedArea.GetBBox().GetHeight();
            }
            if (null != headerRenderer) {
                height -= headerRenderer.occupiedArea.GetBBox().GetHeight();
            }
            if (height < EPS) {
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
            foreach (IRenderer child in childRenderers) {
                CellRenderer cell = (CellRenderer)child;
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
                    if (null != borders[2]) {
                        if (0 == heights.Count) {
                            heights.Add(0, borders[0].GetWidth() / 2 + borders[2].GetWidth() / 2);
                        }
                    }
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
            float y1 = startY;
            if (heights.Count > 0) {
                y1 -= (float)heights[0];
            }
            for (int i = 1; i < heights.Count; i++) {
                DrawHorizontalBorder(horizontalBordersIndexOffset + i, startX, y1, drawContext.GetCanvas());
                if (i < heights.Count) {
                    y1 -= (float)heights[i];
                }
            }
            float x1 = startX;
            if (countedColumnWidth.Length > 0) {
                x1 += countedColumnWidth[0];
            }
            for (int i = 1; i < bordersHandler.numberOfColumns; i++) {
                DrawVerticalBorder(verticalBordersIndexOffset + i, startY, x1, drawContext.GetCanvas());
                if (i < countedColumnWidth.Length) {
                    x1 += countedColumnWidth[i];
                }
            }
            // Draw bounding borders. Vertical borders are the last to draw in order to collapse with header / footer
            if (drawTop) {
                DrawHorizontalBorder(0, startX, startY, drawContext.GetCanvas());
            }
            if (drawBottom) {
                DrawHorizontalBorder(heights.Count, startX, y1, drawContext.GetCanvas());
            }
            // draw left
            DrawVerticalBorder(0, startY, startX, drawContext.GetCanvas());
            // draw right
            DrawVerticalBorder(bordersHandler.numberOfColumns, startY, x1, drawContext.GetCanvas());
            if (isTagged) {
                drawContext.GetCanvas().CloseTag();
            }
        }

        private void DrawHorizontalBorder(int i, float startX, float y1, PdfCanvas canvas) {
            IList<Border> borders = bordersHandler.horizontalBorders[horizontalBordersIndexOffset + i];
            float x1 = startX;
            float x2 = x1 + countedColumnWidth[0];
            if (i == 0) {
                if (bordersHandler.verticalBorders != null && bordersHandler.verticalBorders.Count > 0 && bordersHandler.verticalBorders
                    [0].Count > verticalBordersIndexOffset && bordersHandler.verticalBorders[bordersHandler.numberOfColumns
                    ].Count > verticalBordersIndexOffset) {
                    Border firstBorder = bordersHandler.verticalBorders[0][verticalBordersIndexOffset];
                    if (firstBorder != null) {
                        x1 -= firstBorder.GetWidth() / 2;
                    }
                }
            }
            else {
                if (i == heights.Count) {
                    if (bordersHandler.verticalBorders != null && bordersHandler.verticalBorders.Count > 0 && bordersHandler.verticalBorders
                        [0].Count > verticalBordersIndexOffset && bordersHandler.verticalBorders[bordersHandler.numberOfColumns
                        ] != null && bordersHandler.verticalBorders[bordersHandler.numberOfColumns].Count > verticalBordersIndexOffset
                         && bordersHandler.verticalBorders[0] != null) {
                        Border firstBorder = bordersHandler.verticalBorders[0][verticalBordersIndexOffset + heights.Count - 1];
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
                if (bordersHandler.verticalBorders != null && bordersHandler.verticalBorders.Count > j && bordersHandler.verticalBorders
                    [j] != null && bordersHandler.verticalBorders[j].Count > verticalBordersIndexOffset) {
                    if (i == 0) {
                        if (bordersHandler.verticalBorders[j][verticalBordersIndexOffset + i] != null) {
                            x2 += bordersHandler.verticalBorders[j][verticalBordersIndexOffset + i].GetWidth() / 2;
                        }
                    }
                    else {
                        if (i == heights.Count && bordersHandler.verticalBorders[j].Count >= verticalBordersIndexOffset + i - 1 &&
                             bordersHandler.verticalBorders[j][verticalBordersIndexOffset + i - 1] != null) {
                            x2 += bordersHandler.verticalBorders[j][verticalBordersIndexOffset + i - 1].GetWidth() / 2;
                        }
                    }
                }
                lastBorder.DrawCellBorder(canvas, x1, y1, x2, y1);
            }
        }

        private void DrawVerticalBorder(int i, float startY, float x1, PdfCanvas canvas) {
            IList<Border> borders = bordersHandler.verticalBorders[i];
            float y1 = startY;
            float y2 = y1;
            if (!heights.IsEmpty()) {
                y2 = y1 - (float)heights[0];
            }
            int j;
            for (j = 1; j < heights.Count; j++) {
                Border prevBorder = borders[verticalBordersIndexOffset + j - 1];
                Border curBorder = borders[verticalBordersIndexOffset + j];
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
            Border lastBorder = borders[verticalBordersIndexOffset + j - 1];
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

        private void CorrectCellsOccupiedAreas(LayoutResult[] splits, int row, int[] targetOverflowRowIndex) {
            // Correct occupied areas of all added cells
            for (int k = 0; k <= row; k++) {
                CellRenderer[] currentRow = rows[k];
                if (k < row || (row + 1 == heights.Count)) {
                    for (int col = 0; col < currentRow.Length; col++) {
                        CellRenderer cell = (k < row || null == splits[col]) ? currentRow[col] : (CellRenderer)splits[col].GetSplitRenderer
                            ();
                        if (cell == null) {
                            continue;
                        }
                        float height = 0;
                        int rowspan = (int)cell.GetPropertyAsInteger(Property.ROWSPAN);
                        for (int l = k; l > ((k == row + 1) ? targetOverflowRowIndex[col] : k) - rowspan && l >= 0; l--) {
                            height += (float)heights[l];
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

        private iText.Layout.Renderer.TableRenderer InitFooterOrHeaderRenderer(bool footer, Border[] tableBorders) {
            Table table = (Table)GetModelElement();
            Table footerOrHeader = footer ? table.GetFooter() : table.GetHeader();
            int innerBorder = footer ? 0 : 2;
            int outerBorder = footer ? 2 : 0;
            iText.Layout.Renderer.TableRenderer renderer = (iText.Layout.Renderer.TableRenderer)footerOrHeader.CreateRendererSubTree
                ().SetParent(this);
            Border[] borders = renderer.GetBorders();
            if (table.IsEmpty()) {
                renderer.SetBorders(TableBorders.GetCollapsedBorder(borders[innerBorder], tableBorders[innerBorder]), innerBorder
                    );
                SetBorders(Border.NO_BORDER, innerBorder);
            }
            renderer.SetBorders(TableBorders.GetCollapsedBorder(borders[1], tableBorders[1]), 1);
            renderer.SetBorders(TableBorders.GetCollapsedBorder(borders[3], tableBorders[3]), 3);
            renderer.SetBorders(TableBorders.GetCollapsedBorder(borders[outerBorder], tableBorders[outerBorder]), outerBorder
                );
            SetBorders(Border.NO_BORDER, outerBorder);
            return renderer;
        }

        private iText.Layout.Renderer.TableRenderer PrepareFooterOrHeaderRendererForLayout(iText.Layout.Renderer.TableRenderer
             renderer, float layoutBoxWidth) {
            renderer.countedColumnWidth = countedColumnWidth;
            renderer.leftBorderMaxWidth = leftBorderMaxWidth;
            renderer.rightBorderMaxWidth = rightBorderMaxWidth;
            if (HasProperty(Property.WIDTH)) {
                renderer.SetProperty(Property.WIDTH, UnitValue.CreatePointValue(layoutBoxWidth));
            }
            return this;
        }

        private iText.Layout.Renderer.TableRenderer ProcessRendererBorders(int numberOfColumns) {
            // FIXME
            bordersHandler.InitializeBorders(new List<Border>(), true);
            bordersHandler.CollapseAllBordersAndEmptyRows(rows, GetBorders(), rowRange.GetStartRow(), rowRange.GetFinishRow
                (), numberOfColumns);
            return this;
        }

        private bool IsHeaderRenderer() {
            return parent is iText.Layout.Renderer.TableRenderer && ((iText.Layout.Renderer.TableRenderer)parent).headerRenderer
                 == this;
        }

        private bool IsFooterRenderer() {
            return parent is iText.Layout.Renderer.TableRenderer && ((iText.Layout.Renderer.TableRenderer)parent).footerRenderer
                 == this;
        }

        private void CalculateColumnWidths(float availableWidth, float[] borders) {
            if (countedColumnWidth == null || totalWidthForColumns != availableWidth) {
                TableWidths tableWidths = new TableWidths(this, availableWidth, borders);
                if (tableWidths.HasFixedLayout()) {
                    countedColumnWidth = tableWidths.FixedLayout();
                }
                else {
                    TableRenderer.ColumnMinMaxWidth minMax = CountTableMinMaxWidth(availableWidth, false, true);
                    countedColumnWidth = tableWidths.AutoLayout(minMax.GetMinWidth(), minMax.GetMaxWidth());
                }
            }
        }

        private float GetTableWidth() {
            float sum = 0;
            foreach (float column in countedColumnWidth) {
                sum += column;
            }
            return sum + rightBorderMaxWidth / 2 + leftBorderMaxWidth / 2;
        }

        protected internal virtual iText.Layout.Renderer.TableRenderer SaveCellsProperties() {
            CellRenderer[] currentRow;
            int colN = ((Table)GetModelElement()).GetNumberOfColumns();
            for (int row = 0; row < rows.Count; row++) {
                currentRow = rows[row];
                for (int col = 0; col < colN; col++) {
                    if (null != currentRow[col]) {
                        currentRow[col].SaveProperties();
                    }
                }
            }
            return this;
        }

        protected internal virtual iText.Layout.Renderer.TableRenderer RestoreCellsProperties() {
            CellRenderer[] currentRow;
            int colN = ((Table)GetModelElement()).GetNumberOfColumns();
            for (int row = 0; row < rows.Count; row++) {
                currentRow = rows[row];
                for (int col = 0; col < colN; col++) {
                    if (null != currentRow[col]) {
                        currentRow[col].RestoreProperties();
                    }
                }
            }
            return this;
        }

        /// <summary>This are a structs used for convenience in layout.</summary>
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

        private class ColumnMinMaxWidth {
            private float[] minWidth;

            private float[] maxWidth;

            private float layoutBoxWidth;

            internal virtual float[] GetMinWidth() {
                return minWidth;
            }

            internal virtual float[] GetMaxWidth() {
                return maxWidth;
            }

            internal ColumnMinMaxWidth(int ncol) {
                minWidth = new float[ncol];
                maxWidth = new float[ncol];
            }

            internal virtual void MergeWith(TableRenderer.ColumnMinMaxWidth other) {
                int n = Math.Min(minWidth.Length, other.minWidth.Length);
                for (int i = 0; i < n; ++i) {
                    minWidth[i] = Math.Max(minWidth[i], other.minWidth[i]);
                    maxWidth[i] = Math.Max(maxWidth[i], other.maxWidth[i]);
                }
            }

            internal virtual MinMaxWidth ToTableMinMaxWidth(float availableWidth) {
                float additionalWidth = availableWidth - layoutBoxWidth;
                float minColTotalWidth = 0;
                float maxColTotalWidth = 0;
                for (int i = 0; i < minWidth.Length; ++i) {
                    minColTotalWidth += minWidth[i];
                    maxColTotalWidth += maxWidth[i];
                }
                return new MinMaxWidth(additionalWidth, availableWidth, minColTotalWidth, maxColTotalWidth);
            }

            internal virtual TableRenderer.ColumnMinMaxWidth SetLayoutBoxWidth(float width) {
                this.layoutBoxWidth = width;
                return this;
            }
        }
    }
}
