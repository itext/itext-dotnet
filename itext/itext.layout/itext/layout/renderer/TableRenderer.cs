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

        private float[] countedColumnWidth = null;

        private float totalWidthForColumns;

        private float topBorderMaxWidth;

        internal TableBorders bordersHandler;

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

        internal virtual Table GetTable() {
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
            bool headerShouldBeApplied = (table.IsComplete() || !rows.IsEmpty()) && (isFirstOnThePage && (!table.IsSkipFirstHeader
                () || !isFirstHeader)) && !true.Equals(this.GetOwnProperty<bool?>(Property.IGNORE_HEADER));
            if (headerElement != null && headerShouldBeApplied) {
                headerRenderer = InitFooterOrHeaderRenderer(false, tableBorder);
            }
        }

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
            Table tableModel = (Table)GetModelElement();
            if (!tableModel.IsComplete()) {
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
            int row;
            int col;
            int numberOfColumns = ((Table)GetModelElement()).GetNumberOfColumns();
            // The last flushed row. Empty list if the table hasn't been set incomplete
            IList<Border> lastFlushedRowBottomBorder = tableModel.GetLastRowBottomBorder();
            bool isAndWasComplete = tableModel.IsComplete() && 0 == lastFlushedRowBottomBorder.Count;
            if (!IsFooterRenderer() && !IsHeaderRenderer()) {
                if (isOriginalNonSplitRenderer) {
                    bordersHandler = new CollapsedTableBorders(rows, numberOfColumns, GetBorders(), !isAndWasComplete ? rowRange
                        .GetStartRow() : 0);
                    bordersHandler.InitializeBorders();
                }
            }
            bordersHandler.SetRowRange(rowRange.GetStartRow(), rowRange.GetFinishRow());
            InitializeHeaderAndFooter(0 == rowRange.GetStartRow() || area.IsEmptyArea());
            // update
            bordersHandler.UpdateBordersOnNewPage(isOriginalNonSplitRenderer, IsFooterRenderer() || IsHeaderRenderer()
                , this, headerRenderer, footerRenderer);
            if (isOriginalNonSplitRenderer) {
                CorrectRowRange();
            }
            if (IsOriginalRenderer()) {
                float[] margins = GetMargins();
                CalculateColumnWidths(layoutBox.GetWidth() - margins[1] - margins[3]);
            }
            float tableWidth = GetTableWidth();
            MarginsCollapseHandler marginsCollapseHandler = null;
            bool marginsCollapsingEnabled = true.Equals(GetPropertyAsBoolean(Property.COLLAPSING_MARGINS));
            if (marginsCollapsingEnabled) {
                marginsCollapseHandler = new MarginsCollapseHandler(this, layoutContext.GetMarginsCollapseInfo());
            }
            IList<Rectangle> siblingFloatRendererAreas = layoutContext.GetFloatRendererAreas();
            float clearHeightCorrection = FloatingHelper.CalculateClearHeightCorrection(this, siblingFloatRendererAreas
                , layoutBox);
            FloatPropertyValue? floatPropertyValue = this.GetProperty<FloatPropertyValue?>(Property.FLOAT);
            if (FloatingHelper.IsRendererFloating(this, floatPropertyValue)) {
                layoutBox.DecreaseHeight(clearHeightCorrection);
                FloatingHelper.AdjustFloatedTableLayoutBox(this, layoutBox, tableWidth, siblingFloatRendererAreas, floatPropertyValue
                    );
            }
            else {
                clearHeightCorrection = FloatingHelper.AdjustLayoutBoxAccordingToFloats(siblingFloatRendererAreas, layoutBox
                    , tableWidth, clearHeightCorrection, marginsCollapseHandler);
            }
            if (marginsCollapsingEnabled) {
                marginsCollapseHandler.StartMarginsCollapse(layoutBox);
            }
            ApplyMargins(layoutBox, false);
            ApplyFixedXOrYPosition(true, layoutBox);
            if (null != blockMaxHeight && blockMaxHeight < layoutBox.GetHeight() && !true.Equals(GetPropertyAsBoolean(
                Property.FORCED_PLACEMENT))) {
                layoutBox.MoveUp(layoutBox.GetHeight() - (float)blockMaxHeight).SetHeight((float)blockMaxHeight);
                wasHeightClipped = true;
            }
            if (layoutBox.GetWidth() > tableWidth) {
                layoutBox.SetWidth((float)tableWidth + bordersHandler.GetRightBorderMaxWidth() / 2 + bordersHandler.GetLeftBorderMaxWidth
                    () / 2);
            }
            occupiedArea = new LayoutArea(area.GetPageNumber(), new Rectangle(layoutBox.GetX(), layoutBox.GetY() + layoutBox
                .GetHeight(), (float)tableWidth, 0));
            if (footerRenderer != null) {
                // apply the difference to set footer and table left/right margins identical
                PrepareFooterOrHeaderRendererForLayout(footerRenderer, layoutBox.GetWidth());
                // collapse with top footer border
                if (0 != rows.Count || !isAndWasComplete) {
                    bordersHandler.CollapseTableWithFooter(footerRenderer.bordersHandler, false);
                }
                else {
                    if (null != headerRenderer) {
                        headerRenderer.bordersHandler.CollapseTableWithFooter(footerRenderer.bordersHandler, false);
                    }
                }
                LayoutResult result = footerRenderer.Layout(new LayoutContext(new LayoutArea(area.GetPageNumber(), layoutBox
                    )));
                if (result.GetStatus() != LayoutResult.FULL) {
                    // we've changed it during footer initialization. However, now we need to process borders again as they were.
                    DeleteOwnProperty(Property.BORDER_BOTTOM);
                    return new LayoutResult(LayoutResult.NOTHING, null, null, this, result.GetCauseOfNothing());
                }
                float footerHeight = result.GetOccupiedArea().GetBBox().GetHeight();
                footerRenderer.Move(0, -(layoutBox.GetHeight() - footerHeight));
                layoutBox.MoveUp(footerHeight).DecreaseHeight(footerHeight);
                if (!tableModel.IsEmpty()) {
                    float maxFooterTopBorderWidth = footerRenderer.bordersHandler.GetMaxTopWidth();
                    footerRenderer.occupiedArea.GetBBox().DecreaseHeight(maxFooterTopBorderWidth);
                    layoutBox.MoveDown(maxFooterTopBorderWidth).IncreaseHeight(maxFooterTopBorderWidth);
                }
                // we will delete FORCED_PLACEMENT property after adding one row
                // but the footer should be forced placed once more (since we renderer footer twice)
                if (true.Equals(GetPropertyAsBoolean(Property.FORCED_PLACEMENT))) {
                    footerRenderer.SetProperty(Property.FORCED_PLACEMENT, true);
                }
            }
            if (headerRenderer != null) {
                PrepareFooterOrHeaderRendererForLayout(headerRenderer, layoutBox.GetWidth());
                if (0 != rows.Count) {
                    bordersHandler.CollapseTableWithHeader(headerRenderer.bordersHandler, !tableModel.IsEmpty());
                }
                else {
                    if (null != footerRenderer) {
                        footerRenderer.bordersHandler.CollapseTableWithHeader(headerRenderer.bordersHandler, true);
                    }
                }
                topBorderMaxWidth = bordersHandler.GetMaxTopWidth();
                // first row own top border. We will use it while header processing
                LayoutResult result = headerRenderer.Layout(new LayoutContext(new LayoutArea(area.GetPageNumber(), layoutBox
                    )));
                if (result.GetStatus() != LayoutResult.FULL) {
                    // we've changed it during header initialization. However, now we need to process borders again as they were.
                    DeleteOwnProperty(Property.BORDER_TOP);
                    return new LayoutResult(LayoutResult.NOTHING, null, null, this, result.GetCauseOfNothing());
                }
                float headerHeight = result.GetOccupiedArea().GetBBox().GetHeight();
                layoutBox.DecreaseHeight(headerHeight);
                occupiedArea.GetBBox().MoveDown(headerHeight).IncreaseHeight(headerHeight);
                bordersHandler.FixHeaderOccupiedArea(occupiedArea.GetBBox(), layoutBox);
            }
            topBorderMaxWidth = bordersHandler.GetMaxTopWidth();
            bordersHandler.ApplyLeftAndRightTableBorder(layoutBox, false);
            // Table should have a row and some child elements in order to be considered non empty
            bordersHandler.ApplyTopTableBorder(occupiedArea.GetBBox(), layoutBox, tableModel.IsEmpty() || 0 == rows.Count
                , isAndWasComplete, false);
            LayoutResult[] splits = new LayoutResult[numberOfColumns];
            // This represents the target row index for the overflow renderer to be placed to.
            // Usually this is just the current row id of a cell, but it has valuable meaning when a cell has rowspan.
            int[] targetOverflowRowIndex = new int[numberOfColumns];
            // if this is the last renderer, we will use that information to enlarge rows proportionally
            IList<bool> rowsHasCellWithSetHeight = new List<bool>();
            for (row = 0; row < rows.Count; row++) {
                IList<Rectangle> childFloatRendererAreas = new List<Rectangle>();
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
                bool rowHasCellWithSetHeight = false;
                // the element which was the first to cause Layout.Nothing
                IRenderer firstCauseOfNothing = null;
                // the width of the widest bottom border of the row
                bordersHandler.SetFinishRow(rowRange.GetStartRow() + row);
                Border widestRowBottomBorder = bordersHandler.GetWidestHorizontalBorder(rowRange.GetStartRow() + row + 1);
                bordersHandler.SetFinishRow(rowRange.GetFinishRow());
                float widestRowBottomBorderWidth = null == widestRowBottomBorder ? 0 : widestRowBottomBorder.GetWidth();
                // if cell is in the last row on the page, its borders shouldn't collapse with the next row borders
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
                    if (cell.HasOwnOrModelProperty(Property.HEIGHT)) {
                        rowHasCellWithSetHeight = true;
                    }
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
                    // Apply cell borders
                    float[] cellIndents = bordersHandler.GetCellBorderIndents(currentCellInfo.finishRowInd, col, rowspan, colspan
                        );
                    bordersHandler.ApplyCellIndents(cellArea.GetBBox(), cellIndents[0], cellIndents[1], cellIndents[2] + widestRowBottomBorderWidth
                        , cellIndents[3], false);
                    // update cell width
                    cellWidth = cellArea.GetBBox().GetWidth();
                    LayoutResult cellResult = cell.SetParent(this).Layout(new LayoutContext(cellArea, null, childFloatRendererAreas
                        ));
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
                            if (cellResult.GetStatus() != LayoutResult.NOTHING) {
                                // one should disable cell alignment if it was split
                                splits[col].GetOverflowRenderer().SetProperty(Property.VERTICAL_ALIGNMENT, VerticalAlignment.TOP);
                            }
                        }
                        if (cellResult.GetStatus() == LayoutResult.PARTIAL) {
                            currentRow[col] = (CellRenderer)cellResult.GetSplitRenderer();
                        }
                        else {
                            rows[currentCellInfo.finishRowInd][col] = null;
                            currentRow[col] = cell;
                            rowMoves.Put(col, currentCellInfo.finishRowInd);
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
                                bool skipLastFooter = null != footerRenderer && tableModel.IsSkipLastFooter() && tableModel.IsComplete();
                                if (skipLastFooter) {
                                    LayoutArea potentialArea = new LayoutArea(area.GetPageNumber(), layoutBox.Clone());
                                    // Fix layout area
                                    Border widestRowTopBorder = bordersHandler.GetWidestHorizontalBorder(rowRange.GetStartRow() + row);
                                    if (null != widestRowTopBorder) {
                                        potentialArea.GetBBox().MoveDown(widestRowTopBorder.GetWidth() / 2).IncreaseHeight(widestRowTopBorder.GetWidth
                                            () / 2);
                                    }
                                    float footerHeight = footerRenderer.GetOccupiedArea().GetBBox().GetHeight();
                                    potentialArea.GetBBox().MoveDown(footerHeight).IncreaseHeight(footerHeight);
                                    iText.Layout.Renderer.TableRenderer overflowRenderer = CreateOverflowRenderer(new Table.RowRange(rowRange.
                                        GetStartRow() + row, rowRange.GetFinishRow()));
                                    overflowRenderer.rows = rows.SubList(row, rows.Count);
                                    overflowRenderer.SetProperty(Property.IGNORE_HEADER, true);
                                    overflowRenderer.SetProperty(Property.IGNORE_FOOTER, true);
                                    overflowRenderer.SetProperty(Property.MARGIN_TOP, 0);
                                    overflowRenderer.SetProperty(Property.MARGIN_BOTTOM, 0);
                                    overflowRenderer.SetProperty(Property.MARGIN_LEFT, 0);
                                    overflowRenderer.SetProperty(Property.MARGIN_RIGHT, 0);
                                    // we've already applied the top table border on header
                                    if (null != headerRenderer) {
                                        overflowRenderer.SetProperty(Property.BORDER_TOP, Border.NO_BORDER);
                                    }
                                    overflowRenderer.rowRange = new Table.RowRange(0, rows.Count - row - 1);
                                    overflowRenderer.bordersHandler = bordersHandler;
                                    // save old bordersHandler properties
                                    bordersHandler.SkipFooter(overflowRenderer.GetBorders());
                                    if (null != headerRenderer) {
                                        bordersHandler.SkipHeader(overflowRenderer.GetBorders());
                                    }
                                    int savedStartRow = overflowRenderer.bordersHandler.startRow;
                                    overflowRenderer.bordersHandler.SetStartRow(row);
                                    PrepareFooterOrHeaderRendererForLayout(overflowRenderer, layoutBox.GetWidth());
                                    LayoutResult res = overflowRenderer.Layout(new LayoutContext(potentialArea));
                                    bordersHandler.SetStartRow(savedStartRow);
                                    if (LayoutResult.FULL == res.GetStatus()) {
                                        footerRenderer = null;
                                        // fix layout area and table bottom border
                                        layoutBox.IncreaseHeight(footerHeight).MoveDown(footerHeight);
                                        DeleteOwnProperty(Property.BORDER_BOTTOM);
                                        bordersHandler.SetFinishRow(rowRange.GetStartRow() + row);
                                        widestRowBottomBorder = bordersHandler.GetWidestHorizontalBorder(rowRange.GetStartRow() + row + 1);
                                        bordersHandler.SetFinishRow(rowRange.GetFinishRow());
                                        widestRowBottomBorderWidth = null == widestRowBottomBorder ? 0 : widestRowBottomBorder.GetWidth();
                                        cellProcessingQueue.Clear();
                                        currChildRenderers.Clear();
                                        for (addCol = 0; addCol < currentRow.Length; addCol++) {
                                            if (currentRow[addCol] != null) {
                                                cellProcessingQueue.AddLast(new TableRenderer.CellRendererInfo(currentRow[addCol], addCol, row));
                                            }
                                        }
                                        continue;
                                    }
                                    else {
                                        int reusedRows = 0;
                                        if (null != res.GetSplitRenderer()) {
                                            reusedRows = ((iText.Layout.Renderer.TableRenderer)res.GetSplitRenderer()).rows.Count;
                                        }
                                        for (int i = 0; i < numberOfColumns; i++) {
                                            if (null != rows[row + reusedRows][i]) {
                                                rows[row + reusedRows][i] = (CellRenderer)((Cell)rows[row + reusedRows][i].GetModelElement()).CreateRendererSubTree
                                                    ();
                                            }
                                        }
                                        if (null != headerRenderer) {
                                            bordersHandler.CollapseTableWithHeader(headerRenderer.bordersHandler, true);
                                        }
                                        bordersHandler.CollapseTableWithFooter(footerRenderer.bordersHandler, true);
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
                                                if (row + (int)addRenderer.GetPropertyAsInteger(Property.ROWSPAN) - 1 >= addRow) {
                                                    cellProcessingQueue.AddLast(new TableRenderer.CellRendererInfo(addRenderer, addCol, addRow));
                                                }
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                            split = true;
                            splits[col] = cellResult;
                            if (cellResult.GetStatus() == LayoutResult.NOTHING) {
                                hasContent = false;
                                splits[col].GetOverflowRenderer().SetProperty(Property.VERTICAL_ALIGNMENT, verticalAlignment);
                            }
                        }
                    }
                    currChildRenderers.Add(cell);
                    if (cellResult.GetStatus() != LayoutResult.NOTHING) {
                        rowHeight = Math.Max(rowHeight, cellResult.GetOccupiedArea().GetBBox().GetHeight() + bordersHandler.GetCellVerticalAddition
                            (cellIndents) - rowspanOffset);
                    }
                }
                if (hasContent) {
                    heights.Add(rowHeight);
                    rowsHasCellWithSetHeight.Add(rowHasCellWithSetHeight);
                    occupiedArea.GetBBox().MoveDown(rowHeight);
                    occupiedArea.GetBBox().IncreaseHeight(rowHeight);
                    layoutBox.DecreaseHeight(rowHeight);
                }
                if (split || row == rows.Count - 1) {
                    bordersHandler.SetFinishRow(bordersHandler.GetStartRow() + row);
                    if (!hasContent && bordersHandler.GetFinishRow() != bordersHandler.GetStartRow()) {
                        bordersHandler.SetFinishRow(bordersHandler.GetFinishRow() - 1);
                    }
                    bool skip = false;
                    if (null != footerRenderer && tableModel.IsComplete() && tableModel.IsSkipLastFooter() && !split) {
                        footerRenderer = null;
                        if (tableModel.IsEmpty()) {
                            this.DeleteOwnProperty(Property.BORDER_TOP);
                        }
                        skip = true;
                    }
                    // Correct occupied areas of all added cells
                    CorrectLayoutedCellsOccupiedAreas(splits, row, targetOverflowRowIndex, blockMinHeight, layoutBox, rowsHasCellWithSetHeight
                        , !split, !hasContent && cellWithBigRowspanAdded, skip);
                }
                // process footer with collapsed borders
                if ((split || row == rows.Count - 1) && null != footerRenderer) {
                    // maybe the table was incomplete and we can process the footer
                    if (!hasContent && childRenderers.Count == 0) {
                        bordersHandler.ApplyTopTableBorder(occupiedArea.GetBBox(), layoutBox, true);
                    }
                    else {
                        bordersHandler.ApplyBottomTableBorder(occupiedArea.GetBBox(), layoutBox, tableModel.IsEmpty(), false, true
                            );
                    }
                    layoutBox.MoveDown(footerRenderer.occupiedArea.GetBBox().GetHeight()).IncreaseHeight(footerRenderer.occupiedArea
                        .GetBBox().GetHeight());
                    // apply the difference to set footer and table left/right margins identical
                    bordersHandler.ApplyLeftAndRightTableBorder(layoutBox, true);
                    PrepareFooterOrHeaderRendererForLayout(footerRenderer, layoutBox.GetWidth());
                    bordersHandler.CollapseTableWithFooter(footerRenderer.bordersHandler, hasContent || 0 != childRenderers.Count
                        );
                    if (bordersHandler is CollapsedTableBorders) {
                        footerRenderer.SetBorders(CollapsedTableBorders.GetCollapsedBorder(footerRenderer.GetBorders()[2], GetBorders
                            ()[2]), 2);
                    }
                    footerRenderer.Layout(new LayoutContext(new LayoutArea(area.GetPageNumber(), layoutBox)));
                    bordersHandler.ApplyLeftAndRightTableBorder(layoutBox, false);
                    float footerHeight = footerRenderer.GetOccupiedAreaBBox().GetHeight();
                    footerRenderer.Move(0, -(layoutBox.GetHeight() - footerHeight));
                    layoutBox.SetY(footerRenderer.occupiedArea.GetBBox().GetTop()).SetHeight(occupiedArea.GetBBox().GetBottom(
                        ) - layoutBox.GetBottom());
                }
                if (!split) {
                    childRenderers.AddAll(currChildRenderers);
                    currChildRenderers.Clear();
                }
                if (split) {
                    if (marginsCollapsingEnabled) {
                        marginsCollapseHandler.EndMarginsCollapse(layoutBox);
                    }
                    iText.Layout.Renderer.TableRenderer[] splitResult = Split(row, hasContent, cellWithBigRowspanAdded);
                    TableRenderer.OverflowRowsWrapper overflowRows = new TableRenderer.OverflowRowsWrapper(splitResult[1]);
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
                                    CellRenderer originalCell = currentRow[col];
                                    currentRow[col] = null;
                                    rows[targetOverflowRowIndex[col]][col] = originalCell;
                                    overflowRows.SetCell(0, col, null);
                                    overflowRows.SetCell(targetOverflowRowIndex[col] - row, col, (CellRenderer)cellOverflow.SetParent(splitResult
                                        [1]));
                                }
                                else {
                                    overflowRows.SetCell(targetOverflowRowIndex[col] - row, col, (CellRenderer)currentRow[col].SetParent(splitResult
                                        [1]));
                                }
                                overflowRows.GetCell(targetOverflowRowIndex[col] - row, col).occupiedArea = cellOccupiedArea;
                            }
                            else {
                                if (currentRow[col] != null) {
                                    if (hasContent) {
                                        rowspans[col] = ((Cell)currentRow[col].GetModelElement()).GetRowspan();
                                    }
                                    bool isBigRowspannedCell = 1 != ((Cell)currentRow[col].GetModelElement()).GetRowspan();
                                    if (hasContent || isBigRowspannedCell) {
                                        columnsWithCellToBeEnlarged[col] = true;
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
                                    Cell overflowCell = ((Cell)currentRow[col].GetModelElement()).Clone(true);
                                    // we will change properties
                                    CellRenderer originalCellRenderer = currentRow[col];
                                    currentRow[col].isLastRendererForModelElement = false;
                                    childRenderers.Add(currentRow[col]);
                                    currentRow[col] = null;
                                    rows[targetOverflowRowIndex[col]][col] = originalCellRenderer;
                                    overflowRows.SetCell(0, col, null);
                                    overflowRows.SetCell(targetOverflowRowIndex[col] - row, col, (CellRenderer)overflowCell.GetRenderer().SetParent
                                        (this));
                                    overflowRows.GetCell(targetOverflowRowIndex[col] - row, col).DeleteProperty(Property.HEIGHT);
                                    overflowRows.GetCell(targetOverflowRowIndex[col] - row, col).DeleteProperty(Property.MIN_HEIGHT);
                                    overflowRows.GetCell(targetOverflowRowIndex[col] - row, col).DeleteProperty(Property.MAX_HEIGHT);
                                }
                                else {
                                    childRenderers.Add(currentRow[col]);
                                    // shift all cells in the column up
                                    int i = row;
                                    for (; i < row + minRowspan && i + 1 < rows.Count && splitResult[1].rows[i + 1 - row][col] != null; i++) {
                                        overflowRows.SetCell(i - row, col, splitResult[1].rows[i + 1 - row][col]);
                                        overflowRows.SetCell(i + 1 - row, col, null);
                                    }
                                    // the number of cells behind is less then minRowspan-1
                                    // so we should process the last cell in the column as in the case 1 == minRowspan
                                    if (i != row + minRowspan - 1 && null != rows[i][col]) {
                                        Cell overflowCell = ((Cell)rows[i][col].GetModelElement());
                                        overflowRows.GetCell(i - row, col).isLastRendererForModelElement = false;
                                        overflowRows.SetCell(i - row, col, null);
                                        overflowRows.SetCell(targetOverflowRowIndex[col] - row, col, (CellRenderer)overflowCell.GetRenderer().SetParent
                                            (this));
                                    }
                                }
                                overflowRows.GetCell(targetOverflowRowIndex[col] - row, col).occupiedArea = cellOccupiedArea;
                            }
                        }
                    }
                    // Apply borders if there is no footer
                    if (null == footerRenderer) {
                        if (0 != this.childRenderers.Count) {
                            bordersHandler.ApplyBottomTableBorder(occupiedArea.GetBBox(), layoutBox, false);
                        }
                        else {
                            bordersHandler.ApplyTopTableBorder(occupiedArea.GetBBox(), layoutBox, true);
                            // process bottom border of the last added row if there is no footer
                            if (!isAndWasComplete) {
                                bordersHandler.ApplyTopTableBorder(occupiedArea.GetBBox(), layoutBox, 0 == childRenderers.Count, true, false
                                    );
                            }
                        }
                    }
                    if (true.Equals(GetPropertyAsBoolean(Property.FILL_AVAILABLE_AREA)) || true.Equals(GetPropertyAsBoolean(Property
                        .FILL_AVAILABLE_AREA_ON_SPLIT))) {
                        ExtendLastRow(splitResult[1].rows[0], layoutBox);
                    }
                    AdjustFooterAndFixOccupiedArea(layoutBox);
                    // On the next page we need to process rows without any changes except moves connected to actual cell splitting
                    foreach (KeyValuePair<int, int?> entry in rowMoves) {
                        // Move the cell back to its row if there was no actual split
                        if (null == splitResult[1].rows[(int)entry.Value - splitResult[0].rows.Count][entry.Key]) {
                            CellRenderer originalCellRenderer = rows[row][entry.Key];
                            CellRenderer overflowCellRenderer = splitResult[1].rows[row - splitResult[0].rows.Count][entry.Key];
                            rows[(int)entry.Value][entry.Key] = originalCellRenderer;
                            rows[row][entry.Key] = null;
                            overflowRows.SetCell((int)entry.Value - splitResult[0].rows.Count, entry.Key, overflowCellRenderer);
                            overflowRows.SetCell(row - splitResult[0].rows.Count, entry.Key, null);
                        }
                    }
                    if ((IsKeepTogether() && 0 == lastFlushedRowBottomBorder.Count) && !true.Equals(GetPropertyAsBoolean(Property
                        .FORCED_PLACEMENT))) {
                        return new LayoutResult(LayoutResult.NOTHING, null, null, this, null == firstCauseOfNothing ? this : firstCauseOfNothing
                            );
                    }
                    else {
                        int status = ((occupiedArea.GetBBox().GetHeight() - (null == footerRenderer ? 0 : footerRenderer.GetOccupiedArea
                            ().GetBBox().GetHeight()) - (null == headerRenderer ? 0 : headerRenderer.GetOccupiedArea().GetBBox().GetHeight
                            () - headerRenderer.bordersHandler.GetMaxBottomWidth()) == 0) && isAndWasComplete) ? LayoutResult.NOTHING
                             : LayoutResult.PARTIAL;
                        if ((status == LayoutResult.NOTHING && true.Equals(GetPropertyAsBoolean(Property.FORCED_PLACEMENT))) || wasHeightClipped
                            ) {
                            if (wasHeightClipped) {
                                ILogger logger = LoggerFactory.GetLogger(typeof(iText.Layout.Renderer.TableRenderer));
                                logger.Warn(iText.IO.LogMessageConstant.CLIP_ELEMENT);
                                // Process borders
                                if (status == LayoutResult.NOTHING) {
                                    bordersHandler.ApplyTopTableBorder(occupiedArea.GetBBox(), layoutBox, 0 == childRenderers.Count, true, false
                                        );
                                    bordersHandler.ApplyBottomTableBorder(occupiedArea.GetBBox(), layoutBox, 0 == childRenderers.Count, true, 
                                        false);
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
                            ApplyFixedXOrYPosition(false, layoutBox);
                            ApplyMargins(occupiedArea.GetBBox(), true);
                            LayoutArea editedArea = FloatingHelper.AdjustResultOccupiedAreaForFloatAndClear(this, siblingFloatRendererAreas
                                , layoutContext.GetArea().GetBBox(), clearHeightCorrection, marginsCollapsingEnabled);
                            return new LayoutResult(LayoutResult.FULL, editedArea, splitResult[0], null);
                        }
                        else {
                            ApplyFixedXOrYPosition(false, layoutBox);
                            ApplyMargins(occupiedArea.GetBBox(), true);
                            if (HasProperty(Property.HEIGHT)) {
                                splitResult[1].SetProperty(Property.HEIGHT, RetrieveHeight() - occupiedArea.GetBBox().GetHeight());
                            }
                            if (HasProperty(Property.MAX_HEIGHT)) {
                                splitResult[1].SetProperty(Property.MAX_HEIGHT, RetrieveMaxHeight() - occupiedArea.GetBBox().GetHeight());
                            }
                            if (HasProperty(Property.MAX_HEIGHT)) {
                                splitResult[1].SetProperty(Property.MAX_HEIGHT, RetrieveMaxHeight() - occupiedArea.GetBBox().GetHeight());
                            }
                            LayoutArea editedArea = null;
                            if (status != LayoutResult.NOTHING) {
                                editedArea = FloatingHelper.AdjustResultOccupiedAreaForFloatAndClear(this, siblingFloatRendererAreas, layoutContext
                                    .GetArea().GetBBox(), clearHeightCorrection, marginsCollapsingEnabled);
                            }
                            return new LayoutResult(status, editedArea, splitResult[0], splitResult[1], null == firstCauseOfNothing ? 
                                this : firstCauseOfNothing);
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
                if (lastInRow < 0 || lastRow.Length != lastInRow + (int)lastRow[lastInRow].GetPropertyAsInteger(Property.COLSPAN
                    )) {
                    ILogger logger = LoggerFactory.GetLogger(typeof(iText.Layout.Renderer.TableRenderer));
                    logger.Warn(iText.IO.LogMessageConstant.LAST_ROW_IS_NOT_COMPLETE);
                }
            }
            // process footer renderer with collapsed borders
            if (tableModel.IsComplete() && (0 != lastFlushedRowBottomBorder.Count || tableModel.IsEmpty()) && null != 
                footerRenderer) {
                layoutBox.MoveDown(footerRenderer.occupiedArea.GetBBox().GetHeight()).IncreaseHeight(footerRenderer.occupiedArea
                    .GetBBox().GetHeight());
                // apply the difference to set footer and table left/right margins identical
                bordersHandler.ApplyLeftAndRightTableBorder(layoutBox, true);
                PrepareFooterOrHeaderRendererForLayout(footerRenderer, layoutBox.GetWidth());
                if (0 != rows.Count || !isAndWasComplete) {
                    bordersHandler.CollapseTableWithFooter(footerRenderer.bordersHandler, true);
                }
                else {
                    if (null != headerRenderer) {
                        headerRenderer.bordersHandler.CollapseTableWithFooter(footerRenderer.bordersHandler, true);
                    }
                }
                footerRenderer.Layout(new LayoutContext(new LayoutArea(area.GetPageNumber(), layoutBox)));
                bordersHandler.ApplyLeftAndRightTableBorder(layoutBox, false);
                float footerHeight = footerRenderer.GetOccupiedAreaBBox().GetHeight();
                footerRenderer.Move(0, -(layoutBox.GetHeight() - footerHeight));
                layoutBox.MoveUp(footerHeight).DecreaseHeight(footerHeight);
            }
            float bottomTableBorderWidth = bordersHandler.GetMaxBottomWidth();
            // Apply bottom and top border
            if (tableModel.IsComplete()) {
                if (null == footerRenderer) {
                    if (0 != childRenderers.Count) {
                        bordersHandler.ApplyBottomTableBorder(occupiedArea.GetBBox(), layoutBox, false);
                    }
                    else {
                        if (0 != lastFlushedRowBottomBorder.Count) {
                            bordersHandler.ApplyTopTableBorder(occupiedArea.GetBBox(), layoutBox, 0 == childRenderers.Count, true, false
                                );
                        }
                        else {
                            bordersHandler.ApplyBottomTableBorder(occupiedArea.GetBBox(), layoutBox, 0 == childRenderers.Count, true, 
                                false);
                        }
                    }
                }
                else {
                    if (tableModel.IsEmpty() && null != headerRenderer) {
                        float headerBottomBorderWidth = headerRenderer.bordersHandler.GetMaxBottomWidth();
                        headerRenderer.bordersHandler.ApplyBottomTableBorder(headerRenderer.occupiedArea.GetBBox(), layoutBox, true
                            , true, true);
                        occupiedArea.GetBBox().MoveUp(headerBottomBorderWidth).DecreaseHeight(headerBottomBorderWidth);
                    }
                }
            }
            else {
                // the bottom border should be processed and placed lately
                if (0 != heights.Count) {
                    heights[heights.Count - 1] = heights[heights.Count - 1] - bottomTableBorderWidth / 2;
                }
                if (null == footerRenderer) {
                    if (0 != childRenderers.Count) {
                        bordersHandler.ApplyBottomTableBorder(occupiedArea.GetBBox(), layoutBox, 0 == childRenderers.Count, false, 
                            true);
                    }
                }
                else {
                    // occupied area is right here
                    layoutBox.IncreaseHeight(bottomTableBorderWidth);
                }
            }
            if (0 != rows.Count) {
                if (true.Equals(GetPropertyAsBoolean(Property.FILL_AVAILABLE_AREA))) {
                    ExtendLastRow(rows[rows.Count - 1], layoutBox);
                }
            }
            else {
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
            }
            ApplyFixedXOrYPosition(false, layoutBox);
            if (marginsCollapsingEnabled) {
                marginsCollapseHandler.EndMarginsCollapse(layoutBox);
            }
            ApplyMargins(occupiedArea.GetBBox(), true);
            // we should process incomplete table's footer only dureing splitting
            if (!tableModel.IsComplete() && null != footerRenderer) {
                footerRenderer = null;
                bordersHandler.SkipFooter(bordersHandler.tableBoundingBorders);
            }
            AdjustFooterAndFixOccupiedArea(layoutBox);
            FloatingHelper.RemoveFloatsAboveRendererBottom(siblingFloatRendererAreas, this);
            LayoutArea editedArea_1 = FloatingHelper.AdjustResultOccupiedAreaForFloatAndClear(this, siblingFloatRendererAreas
                , layoutContext.GetArea().GetBBox(), clearHeightCorrection, marginsCollapsingEnabled);
            return new LayoutResult(LayoutResult.FULL, editedArea_1, null, null, null);
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
                TagTreePointer tagPointer = document.GetTagStructureContext().GetAutoTaggingPointer();
                IAccessibleElement accessibleElement = (IAccessibleElement)GetModelElement();
                bool alreadyCreated = tagPointer.IsElementConnectedToTag(accessibleElement);
                tagPointer.AddTag(accessibleElement, true);
                if (!alreadyCreated) {
                    PdfDictionary layoutAttributes = AccessibleAttributesApplier.GetLayoutAttributes(role, this, tagPointer);
                    ApplyGeneratedAccessibleAttributes(tagPointer, layoutAttributes);
                }
                base.Draw(drawContext);
                tagPointer.MoveToParent();
                bool toRemoveConnectionsWithTag = isLastRendererForModelElement && ((Table)GetModelElement()).IsComplete();
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
                    int cellRowKidIndex = -1;
                    int foundRowsNum = 0;
                    IList<PdfName> kidsRoles = tagPointer.GetKidsRoles();
                    for (int i = 0; i < kidsRoles.Count; ++i) {
                        PdfName kidRole = kidsRoles[i];
                        if (kidRole == null || PdfName.TR.Equals(kidRole)) {
                            ++foundRowsNum;
                        }
                        if (foundRowsNum - 1 == cellRow) {
                            cellRowKidIndex = i;
                            break;
                        }
                    }
                    if (cellRowKidIndex > -1) {
                        tagPointer.MoveToKid(cellRowKidIndex);
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

        protected internal virtual void DrawBackgrounds(DrawContext drawContext) {
            bool shrinkBackgroundArea = bordersHandler is CollapsedTableBorders && (IsHeaderRenderer() || IsFooterRenderer
                ());
            if (shrinkBackgroundArea) {
                occupiedArea.GetBBox().ApplyMargins<Rectangle>(bordersHandler.GetMaxTopWidth() / 2, bordersHandler.GetRightBorderMaxWidth
                    () / 2, bordersHandler.GetMaxBottomWidth() / 2, bordersHandler.GetLeftBorderMaxWidth() / 2, false);
            }
            base.DrawBackground(drawContext);
            if (shrinkBackgroundArea) {
                occupiedArea.GetBBox().ApplyMargins<Rectangle>(bordersHandler.GetMaxTopWidth() / 2, bordersHandler.GetRightBorderMaxWidth
                    () / 2, bordersHandler.GetMaxBottomWidth() / 2, bordersHandler.GetLeftBorderMaxWidth() / 2, true);
            }
            if (null != headerRenderer) {
                headerRenderer.DrawBackgrounds(drawContext);
            }
            if (null != footerRenderer) {
                footerRenderer.DrawBackgrounds(drawContext);
            }
        }

        public override void DrawBackground(DrawContext drawContext) {
            // draw background once for body/header/footer
            if (!IsFooterRenderer() && !IsHeaderRenderer()) {
                DrawBackgrounds(drawContext);
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

        [System.ObsoleteAttribute(@"Method will be removed in 7.1.")]
        protected internal virtual float[] CalculateScaledColumnWidths(Table tableModel, float tableWidth) {
            return countedColumnWidth;
        }

        protected internal virtual iText.Layout.Renderer.TableRenderer[] Split(int row) {
            return Split(row, false);
        }

        protected internal virtual iText.Layout.Renderer.TableRenderer[] Split(int row, bool hasContent) {
            return Split(row, hasContent, false);
        }

        protected internal virtual iText.Layout.Renderer.TableRenderer[] Split(int row, bool hasContent, bool cellWithBigRowspanAdded
            ) {
            iText.Layout.Renderer.TableRenderer splitRenderer = CreateSplitRenderer(new Table.RowRange(rowRange.GetStartRow
                (), rowRange.GetStartRow() + row));
            splitRenderer.rows = rows.SubList(0, row);
            splitRenderer.bordersHandler = bordersHandler;
            splitRenderer.heights = heights;
            splitRenderer.columnWidths = columnWidths;
            splitRenderer.countedColumnWidth = countedColumnWidth;
            splitRenderer.totalWidthForColumns = totalWidthForColumns;
            iText.Layout.Renderer.TableRenderer overflowRenderer = CreateOverflowRenderer(new Table.RowRange(rowRange.
                GetStartRow() + row, rowRange.GetFinishRow()));
            if (0 == row && !(hasContent || cellWithBigRowspanAdded) && 0 == rowRange.GetStartRow()) {
                overflowRenderer.isOriginalNonSplitRenderer = true;
            }
            overflowRenderer.rows = rows.SubList(row, rows.Count);
            splitRenderer.occupiedArea = occupiedArea;
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
            splitRenderer.topBorderMaxWidth = topBorderMaxWidth;
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

        protected internal override MinMaxWidth GetMinMaxWidth(float availableWidth) {
            InitializeTableLayoutBorders();
            float rightMaxBorder = bordersHandler.GetRightBorderMaxWidth();
            float leftMaxBorder = bordersHandler.GetLeftBorderMaxWidth();
            TableWidths tableWidths = new TableWidths(this, availableWidth, true, rightMaxBorder, leftMaxBorder);
            float[] columns = tableWidths.Layout();
            float minWidth = tableWidths.GetMinWidth();
            CleanTableLayoutBorders();
            float maxColTotalWidth = 0;
            foreach (float column in columns) {
                maxColTotalWidth += column;
            }
            float additionalWidth = (float)this.GetPropertyAsFloat(Property.MARGIN_RIGHT) + (float)this.GetPropertyAsFloat
                (Property.MARGIN_LEFT) + rightMaxBorder / 2 + leftMaxBorder / 2;
            return new MinMaxWidth(additionalWidth, availableWidth, minWidth, maxColTotalWidth);
        }

        private void InitializeTableLayoutBorders() {
            bordersHandler = new CollapsedTableBorders(rows, ((Table)GetModelElement()).GetNumberOfColumns(), GetBorders
                ());
            bordersHandler.InitializeBorders();
            bordersHandler.SetTableBoundingBorders(GetBorders());
            bordersHandler.SetRowRange(rowRange.GetStartRow(), rowRange.GetFinishRow());
            InitializeHeaderAndFooter(true);
            bordersHandler.UpdateBordersOnNewPage(isOriginalNonSplitRenderer, IsFooterRenderer() || IsHeaderRenderer()
                , this, headerRenderer, footerRenderer);
            CorrectRowRange();
        }

        private void CleanTableLayoutBorders() {
            footerRenderer = null;
            headerRenderer = null;
            // we may have deleted empty rows and now need to update table's rowrange
            this.rowRange = new Table.RowRange(rowRange.GetStartRow(), bordersHandler.GetFinishRow());
            //TODO do we need it?
            // delete set properties
            DeleteOwnProperty(Property.BORDER_BOTTOM);
            DeleteOwnProperty(Property.BORDER_TOP);
        }

        private void CorrectRowRange() {
            if (rows.Count < rowRange.GetFinishRow() - rowRange.GetStartRow() + 1) {
                rowRange = new Table.RowRange(rowRange.GetStartRow(), rowRange.GetStartRow() + rows.Count - 1);
            }
        }

        public override void DrawBorder(DrawContext drawContext) {
        }

        // Do nothing here. Itext7 handles cell and table borders collapse and draws result borders during #drawBorders()
        protected internal virtual void DrawBorders(DrawContext drawContext) {
            DrawBorders(drawContext, null != headerRenderer, null != footerRenderer);
        }

        [Obsolete]
        protected internal virtual void DrawBorders(DrawContext drawContext, bool hasHeader, bool hasFooter) {
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
            float startX = GetOccupiedArea().GetBBox().GetX() + bordersHandler.GetLeftBorderMaxWidth() / 2;
            float startY = GetOccupiedArea().GetBBox().GetY() + GetOccupiedArea().GetBBox().GetHeight();
            if (null != headerRenderer) {
                startY -= headerRenderer.occupiedArea.GetBBox().GetHeight();
                startY += topBorderMaxWidth / 2;
            }
            else {
                startY -= topBorderMaxWidth / 2;
            }
            if (HasProperty(Property.MARGIN_TOP)) {
                float? topMargin = this.GetPropertyAsFloat(Property.MARGIN_TOP);
                startY -= null == topMargin ? 0 : (float)topMargin;
            }
            if (HasProperty(Property.MARGIN_LEFT)) {
                float? leftMargin = this.GetPropertyAsFloat(Property.MARGIN_LEFT);
                startX += +(null == leftMargin ? 0 : (float)leftMargin);
            }
            // process halves of the borders here
            if (childRenderers.Count == 0) {
                Border[] borders = bordersHandler.tableBoundingBorders;
                if (null != borders[0]) {
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
                if (0 == heights.Count) {
                    heights.Add(0f);
                }
            }
            bool isTagged = drawContext.IsTaggingEnabled() && GetModelElement() is IAccessibleElement;
            if (isTagged) {
                drawContext.GetCanvas().OpenTag(new CanvasArtifact());
            }
            // considering these values itext will draw table borders correctly
            bool isTopTablePart = IsTopTablePart();
            bool isBottomTablePart = IsBottomTablePart();
            bool isComplete = GetTable().IsComplete();
            bool isFooterRendererOfLargeTable = IsFooterRendererOfLargeTable();
            bordersHandler.SetRowRange(rowRange.GetStartRow(), rowRange.GetStartRow() + heights.Count - 1);
            if (bordersHandler is CollapsedTableBorders) {
                if (hasFooter) {
                    ((CollapsedTableBorders)bordersHandler).SetBottomBorderCollapseWith(footerRenderer.bordersHandler.GetFirstHorizontalBorder
                        ());
                }
                else {
                    if (isBottomTablePart) {
                        ((CollapsedTableBorders)bordersHandler).SetBottomBorderCollapseWith(null);
                    }
                }
            }
            // we do not need to fix top border, because either this is header or the top border has been already written
            float y1 = startY;
            if (isFooterRendererOfLargeTable) {
                bordersHandler.DrawHorizontalBorder(0, startX, y1, drawContext.GetCanvas(), countedColumnWidth);
            }
            if (0 != heights.Count) {
                y1 -= (float)heights[0];
            }
            for (int i = 1; i < heights.Count; i++) {
                bordersHandler.DrawHorizontalBorder(i, startX, y1, drawContext.GetCanvas(), countedColumnWidth);
                if (i < heights.Count) {
                    y1 -= (float)heights[i];
                }
            }
            if (!isBottomTablePart && isComplete) {
                bordersHandler.DrawHorizontalBorder(heights.Count, startX, y1, drawContext.GetCanvas(), countedColumnWidth
                    );
            }
            float x1 = startX;
            if (countedColumnWidth.Length > 0) {
                x1 += countedColumnWidth[0];
            }
            for (int i = 1; i < bordersHandler.GetNumberOfColumns(); i++) {
                bordersHandler.DrawVerticalBorder(i, startY, x1, drawContext.GetCanvas(), heights);
                if (i < countedColumnWidth.Length) {
                    x1 += countedColumnWidth[i];
                }
            }
            // Draw bounding borders. Vertical borders are the last to draw in order to collapse with header / footer
            if (isTopTablePart) {
                bordersHandler.DrawHorizontalBorder(0, startX, startY, drawContext.GetCanvas(), countedColumnWidth);
            }
            if (isBottomTablePart && isComplete) {
                bordersHandler.DrawHorizontalBorder(heights.Count, startX, y1, drawContext.GetCanvas(), countedColumnWidth
                    );
            }
            // draw left
            bordersHandler.DrawVerticalBorder(0, startY, startX, drawContext.GetCanvas(), heights);
            // draw right
            bordersHandler.DrawVerticalBorder(bordersHandler.GetNumberOfColumns(), startY, x1, drawContext.GetCanvas()
                , heights);
            if (isTagged) {
                drawContext.GetCanvas().CloseTag();
            }
        }

        private void ApplyFixedXOrYPosition(bool isXPosition, Rectangle layoutBox) {
            if (IsPositioned()) {
                if (IsFixedLayout()) {
                    if (isXPosition) {
                        float x = (float)this.GetPropertyAsFloat(Property.X);
                        layoutBox.SetX(x);
                    }
                    else {
                        float y = (float)this.GetPropertyAsFloat(Property.Y);
                        Move(0, y - occupiedArea.GetBBox().GetY());
                    }
                }
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

        private void CorrectLayoutedCellsOccupiedAreas(LayoutResult[] splits, int row, int[] targetOverflowRowIndex
            , float? blockMinHeight, Rectangle layoutBox, IList<bool> rowsHasCellWithSetHeight, bool isLastRenderer
            , bool processBigRowspan, bool skip) {
            // correct last height
            int finish = bordersHandler.GetFinishRow();
            bordersHandler.SetFinishRow(rowRange.GetFinishRow());
            Border currentBorder = bordersHandler.GetWidestHorizontalBorder(finish + 1);
            bordersHandler.SetFinishRow(finish);
            if (skip) {
                // Update bordersHandler
                bordersHandler.tableBoundingBorders[2] = GetBorders()[2];
                bordersHandler.SkipFooter(bordersHandler.tableBoundingBorders);
            }
            float currentBottomIndent = null == currentBorder ? 0 : currentBorder.GetWidth();
            float realBottomIndent = bordersHandler.GetMaxBottomWidth();
            if (0 != heights.Count) {
                heights[heights.Count - 1] = heights[heights.Count - 1] + (realBottomIndent - currentBottomIndent) / 2;
                // correct occupied area and layoutbox
                occupiedArea.GetBBox().IncreaseHeight((realBottomIndent - currentBottomIndent) / 2).MoveDown((realBottomIndent
                     - currentBottomIndent) / 2);
                layoutBox.DecreaseHeight((realBottomIndent - currentBottomIndent) / 2);
                if (processBigRowspan) {
                    // process the last row and correct its height
                    CellRenderer[] currentRow = rows[heights.Count];
                    for (int col = 0; col < currentRow.Length; col++) {
                        CellRenderer cell = null == splits[col] ? currentRow[col] : (CellRenderer)splits[col].GetSplitRenderer();
                        if (cell == null) {
                            continue;
                        }
                        float height = 0;
                        int rowspan = (int)cell.GetPropertyAsInteger(Property.ROWSPAN);
                        int colspan = (int)cell.GetPropertyAsInteger(Property.COLSPAN);
                        float[] indents = bordersHandler.GetCellBorderIndents(targetOverflowRowIndex[col], col, rowspan, colspan);
                        for (int l = heights.Count - 1 - 1; l > targetOverflowRowIndex[col] - rowspan && l >= 0; l--) {
                            height += (float)heights[l];
                        }
                        float cellHeightInLastRow = cell.GetOccupiedArea().GetBBox().GetHeight() + indents[0] / 2 + indents[2] / 2
                             - height;
                        if (heights[heights.Count - 1] < cellHeightInLastRow) {
                            heights[heights.Count - 1] = cellHeightInLastRow;
                        }
                    }
                }
            }
            float additionalCellHeight = 0;
            int numOfRowsWithFloatHeight = 0;
            if (isLastRenderer) {
                float additionalHeight = 0;
                if (null != blockMinHeight && blockMinHeight > occupiedArea.GetBBox().GetHeight() + realBottomIndent / 2) {
                    additionalHeight = Math.Min(layoutBox.GetHeight() - realBottomIndent / 2, (float)blockMinHeight - occupiedArea
                        .GetBBox().GetHeight() - realBottomIndent / 2);
                    for (int k = 0; k < rowsHasCellWithSetHeight.Count; k++) {
                        if (false.Equals(rowsHasCellWithSetHeight[k])) {
                            numOfRowsWithFloatHeight++;
                        }
                    }
                }
                additionalCellHeight = additionalHeight / (0 == numOfRowsWithFloatHeight ? heights.Count : numOfRowsWithFloatHeight
                    );
                for (int k = 0; k < heights.Count; k++) {
                    if (0 == numOfRowsWithFloatHeight || false.Equals(rowsHasCellWithSetHeight[k])) {
                        heights[k] = (float)heights[k] + additionalCellHeight;
                    }
                }
            }
            float cumulativeShift = 0;
            // Correct occupied areas of all added cells
            for (int k = 0; k < heights.Count; k++) {
                CorrectRowCellsOccupiedAreas(splits, row, targetOverflowRowIndex, k, rowsHasCellWithSetHeight, cumulativeShift
                    , additionalCellHeight);
                if (isLastRenderer) {
                    if (0 == numOfRowsWithFloatHeight || false.Equals(rowsHasCellWithSetHeight[k])) {
                        cumulativeShift += additionalCellHeight;
                    }
                }
            }
            // extend occupied area, if some rows have been extended
            occupiedArea.GetBBox().MoveDown(cumulativeShift).IncreaseHeight(cumulativeShift);
            layoutBox.DecreaseHeight(cumulativeShift);
        }

        private void CorrectRowCellsOccupiedAreas(LayoutResult[] splits, int row, int[] targetOverflowRowIndex, int
             currentRowIndex, IList<bool> rowsHasCellWithSetHeight, float cumulativeShift, float additionalCellHeight
            ) {
            CellRenderer[] currentRow = rows[currentRowIndex];
            for (int col = 0; col < currentRow.Length; col++) {
                CellRenderer cell = (currentRowIndex < row || null == splits[col]) ? currentRow[col] : (CellRenderer)splits
                    [col].GetSplitRenderer();
                if (cell == null) {
                    continue;
                }
                float height = 0;
                int colspan = (int)cell.GetPropertyAsInteger(Property.COLSPAN);
                int rowspan = (int)cell.GetPropertyAsInteger(Property.ROWSPAN);
                float rowspanOffset = 0;
                float[] indents = bordersHandler.GetCellBorderIndents(currentRowIndex < row ? currentRowIndex : targetOverflowRowIndex
                    [col], col, rowspan, colspan);
                // process rowspan
                for (int l = (currentRowIndex < row ? currentRowIndex : heights.Count - 1) - 1; l > (currentRowIndex < row
                     ? currentRowIndex : targetOverflowRowIndex[col]) - rowspan && l >= 0; l--) {
                    height += (float)heights[l];
                    if (false.Equals(rowsHasCellWithSetHeight[l])) {
                        rowspanOffset += additionalCellHeight;
                    }
                }
                height += (float)heights[currentRowIndex < row ? currentRowIndex : heights.Count - 1];
                height -= indents[0] / 2 + indents[2] / 2;
                // Correcting cell bbox only. We don't need #move() here.
                // This is because of BlockRenderer's specificity regarding occupied area.
                float shift = height - cell.GetOccupiedArea().GetBBox().GetHeight();
                Rectangle bBox = cell.GetOccupiedArea().GetBBox();
                bBox.MoveDown(shift);
                cell.Move(0, -(cumulativeShift - rowspanOffset));
                bBox.SetHeight(height);
                cell.ApplyVerticalAlignment();
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
                renderer.SetBorders(CollapsedTableBorders.GetCollapsedBorder(borders[innerBorder], tableBorders[innerBorder
                    ]), innerBorder);
                bordersHandler.tableBoundingBorders[innerBorder] = Border.NO_BORDER;
            }
            renderer.SetBorders(CollapsedTableBorders.GetCollapsedBorder(borders[1], tableBorders[1]), 1);
            renderer.SetBorders(CollapsedTableBorders.GetCollapsedBorder(borders[3], tableBorders[3]), 3);
            renderer.SetBorders(CollapsedTableBorders.GetCollapsedBorder(borders[outerBorder], tableBorders[outerBorder
                ]), outerBorder);
            bordersHandler.tableBoundingBorders[outerBorder] = Border.NO_BORDER;
            renderer.bordersHandler = new CollapsedTableBorders(renderer.rows, ((Table)renderer.GetModelElement()).GetNumberOfColumns
                (), renderer.GetBorders());
            renderer.bordersHandler.InitializeBorders();
            renderer.bordersHandler.SetRowRange(renderer.rowRange.GetStartRow(), renderer.rowRange.GetFinishRow());
            ((CollapsedTableBorders)renderer.bordersHandler).CollapseAllBordersAndEmptyRows();
            renderer.CorrectRowRange();
            return renderer;
        }

        private iText.Layout.Renderer.TableRenderer PrepareFooterOrHeaderRendererForLayout(iText.Layout.Renderer.TableRenderer
             renderer, float layoutBoxWidth) {
            renderer.countedColumnWidth = countedColumnWidth;
            renderer.bordersHandler.leftBorderMaxWidth = bordersHandler.GetLeftBorderMaxWidth();
            renderer.bordersHandler.rightBorderMaxWidth = bordersHandler.GetRightBorderMaxWidth();
            if (HasProperty(Property.WIDTH)) {
                renderer.SetProperty(Property.WIDTH, UnitValue.CreatePointValue(layoutBoxWidth));
            }
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

        private bool IsFooterRendererOfLargeTable() {
            return IsFooterRenderer() && (!GetTable().IsComplete() || 0 != ((iText.Layout.Renderer.TableRenderer)parent
                ).GetTable().GetLastRowBottomBorder().Count);
        }

        private bool IsTopTablePart() {
            return null == headerRenderer && (!IsFooterRenderer() || (0 == ((iText.Layout.Renderer.TableRenderer)parent
                ).rows.Count && null == ((iText.Layout.Renderer.TableRenderer)parent).headerRenderer));
        }

        private bool IsBottomTablePart() {
            return null == footerRenderer && (!IsHeaderRenderer() || (0 == ((iText.Layout.Renderer.TableRenderer)parent
                ).rows.Count && null == ((iText.Layout.Renderer.TableRenderer)parent).footerRenderer));
        }

        /// <summary>Returns minWidth</summary>
        private void CalculateColumnWidths(float availableWidth) {
            if (countedColumnWidth == null || totalWidthForColumns != availableWidth) {
                TableWidths tableWidths = new TableWidths(this, availableWidth, false, bordersHandler.rightBorderMaxWidth, 
                    bordersHandler.leftBorderMaxWidth);
                countedColumnWidth = tableWidths.Layout();
            }
        }

        private float GetTableWidth() {
            float sum = 0;
            foreach (float column in countedColumnWidth) {
                sum += column;
            }
            return sum + bordersHandler.GetRightBorderMaxWidth() / 2 + bordersHandler.GetLeftBorderMaxWidth() / 2;
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

        /// <summary>Utility class that copies overflow renderer rows on cell replacement so it won't affect original renderer
        ///     </summary>
        private class OverflowRowsWrapper {
            private TableRenderer overflowRenderer;

            private Dictionary<int, bool?> isRowReplaced = new Dictionary<int, bool?>();

            private bool isReplaced = false;

            public OverflowRowsWrapper(TableRenderer overflowRenderer) {
                this.overflowRenderer = overflowRenderer;
            }

            public virtual CellRenderer GetCell(int row, int col) {
                return overflowRenderer.rows[row][col];
            }

            public virtual CellRenderer SetCell(int row, int col, CellRenderer newCell) {
                if (!isReplaced) {
                    overflowRenderer.rows = new List<CellRenderer[]>(overflowRenderer.rows);
                    isReplaced = true;
                }
                if (!true.Equals(isRowReplaced.Get(row))) {
                    overflowRenderer.rows[row] = (CellRenderer[])overflowRenderer.rows[row].Clone();
                }
                return overflowRenderer.rows[row][col] = newCell;
            }
        }
    }
}
