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
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Commons.Utils;
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Tagutils;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Layout;
using iText.Layout.Margincollapse;
using iText.Layout.Minmaxwidth;
using iText.Layout.Properties;
using iText.Layout.Tagging;

namespace iText.Layout.Renderer {
    /// <summary>
    /// This class represents the
    /// <see cref="IRenderer">renderer</see>
    /// object for a
    /// <see cref="iText.Layout.Element.Table"/>
    /// object.
    /// </summary>
    /// <remarks>
    /// This class represents the
    /// <see cref="IRenderer">renderer</see>
    /// object for a
    /// <see cref="iText.Layout.Element.Table"/>
    /// object. It will delegate its drawing operations on to the
    /// <see cref="CellRenderer"/>
    /// instances associated with the
    /// <see cref="iText.Layout.Element.Cell">table cells</see>.
    /// </remarks>
    public class TableRenderer : AbstractRenderer {
        protected internal IList<CellRenderer[]> rows = new List<CellRenderer[]>();

        // Row range of the current renderer. For large tables it may contain only a few rows.
        protected internal Table.RowRange rowRange;

        protected internal iText.Layout.Renderer.TableRenderer headerRenderer;

        protected internal iText.Layout.Renderer.TableRenderer footerRenderer;

        protected internal DivRenderer captionRenderer;

        /// <summary>True for newly created renderer.</summary>
        /// <remarks>True for newly created renderer. For split renderers this is set to false. Used for tricky layout.
        ///     </remarks>
        protected internal bool isOriginalNonSplitRenderer = true;

//\cond DO_NOT_DOCUMENT
        internal TableBorders bordersHandler;
//\endcond

        private float[] columnWidths = null;

        private IList<float> heights = new List<float>();

        private float[] countedColumnWidth = null;

        private float totalWidthForColumns;

        private float topBorderMaxWidth;

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
            SetRowRange(rowRange);
        }

        /// <summary>
        /// Creates a TableRenderer from a
        /// <see cref="iText.Layout.Element.Table"/>.
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
                ILogger logger = ITextLogManager.GetLogger(typeof(iText.Layout.Renderer.TableRenderer));
                logger.LogError("Only CellRenderer could be added");
            }
        }

        protected internal override Rectangle ApplyBorderBox(Rectangle rect, Border[] borders, bool reverse) {
            if (bordersHandler is SeparatedTableBorders) {
                base.ApplyBorderBox(rect, borders, reverse);
            }
            // Do nothing here. Applying border box for tables is indeed difficult operation and is done on #layout()
            return rect;
        }

        protected internal override Rectangle ApplyPaddings(Rectangle rect, UnitValue[] paddings, bool reverse) {
            if (bordersHandler is SeparatedTableBorders) {
                base.ApplyPaddings(rect, paddings, reverse);
            }
            // Do nothing here. Tables with collapsed borders don't have padding.
            return rect;
        }

        public override Rectangle ApplyPaddings(Rectangle rect, bool reverse) {
            if (bordersHandler is SeparatedTableBorders) {
                base.ApplyPaddings(rect, reverse);
            }
            // Do nothing here. Tables with collapsed borders don't have padding.
            return rect;
        }

        /// <summary>Applies the given spacings on the given rectangle</summary>
        /// <param name="rect">a rectangle spacings will be applied on.</param>
        /// <param name="horizontalSpacing">the horizontal spacing to be applied on the given rectangle</param>
        /// <param name="verticalSpacing">the vertical spacing to be applied on the given rectangle</param>
        /// <param name="reverse">
        /// indicates whether the spacings will be applied
        /// inside (in case of false) or outside (in case of false) the rectangle.
        /// </param>
        /// <returns>
        /// a
        /// <see cref="iText.Kernel.Geom.Rectangle">border box</see>
        /// of the renderer
        /// </returns>
        private Rectangle ApplySpacing(Rectangle rect, float horizontalSpacing, float verticalSpacing, bool reverse
            ) {
            if (bordersHandler is SeparatedTableBorders) {
                return rect.ApplyMargins(verticalSpacing / 2, horizontalSpacing / 2, verticalSpacing / 2, horizontalSpacing
                     / 2, reverse);
            }
            // Do nothing here. Tables with collapsed borders don't have spacing.
            return rect;
        }

        /// <summary>Applies the given horizontal or vertical spacing on the given rectangle</summary>
        /// <param name="rect">a rectangle spacings will be applied on.</param>
        /// <param name="spacing">the horizontal or vertical spacing to be applied on the given rectangle</param>
        /// <param name="isHorizontal">defines whether the provided spacing should be applied as a horizontal or a vertical one
        ///     </param>
        /// <param name="reverse">
        /// indicates whether the spacings will be applied
        /// inside (in case of false) or outside (in case of false) the rectangle.
        /// </param>
        /// <returns>
        /// a
        /// <see cref="iText.Kernel.Geom.Rectangle">border box</see>
        /// of the renderer
        /// </returns>
        private Rectangle ApplySingleSpacing(Rectangle rect, float spacing, bool isHorizontal, bool reverse) {
            if (bordersHandler is SeparatedTableBorders) {
                if (isHorizontal) {
                    return rect.ApplyMargins(0, spacing / 2, 0, spacing / 2, reverse);
                }
                else {
                    return rect.ApplyMargins(spacing / 2, 0, spacing / 2, 0, reverse);
                }
            }
            // Do nothing here. Tables with collapsed borders don't have spacing.
            return rect;
        }

//\cond DO_NOT_DOCUMENT
        internal virtual Table GetTable() {
            return (Table)GetModelElement();
        }
//\endcond

        private void InitializeHeaderAndFooter(bool isFirstOnThePage) {
            Table table = (Table)GetModelElement();
            Border[] tableBorder = GetBorders();
            Table headerElement = table.GetHeader();
            bool isFirstHeader = rowRange.GetStartRow() == 0 && isOriginalNonSplitRenderer;
            bool headerShouldBeApplied = (table.IsComplete() || !rows.IsEmpty()) && (isFirstOnThePage && (!table.IsSkipFirstHeader
                () || !isFirstHeader)) && !true.Equals(this.GetOwnProperty<bool?>(Property.IGNORE_HEADER));
            if (headerElement != null && headerShouldBeApplied) {
                headerRenderer = InitFooterOrHeaderRenderer(false, tableBorder);
            }
            Table footerElement = table.GetFooter();
            // footer can be skipped, but after the table content will be layouted
            bool footerShouldBeApplied = !(table.IsComplete() && 0 != table.GetLastRowBottomBorder().Count && table.IsSkipLastFooter
                ()) && !true.Equals(this.GetOwnProperty<bool?>(Property.IGNORE_FOOTER));
            if (footerElement != null && footerShouldBeApplied) {
                footerRenderer = InitFooterOrHeaderRenderer(true, tableBorder);
            }
        }

        private void InitializeCaptionRenderer(Div caption) {
            if (isOriginalNonSplitRenderer && null != caption) {
                captionRenderer = (DivRenderer)caption.CreateRendererSubTree();
                captionRenderer.SetParent(this.parent);
                LayoutTaggingHelper taggingHelper = this.GetProperty<LayoutTaggingHelper>(Property.TAGGING_HELPER);
                if (taggingHelper != null) {
                    taggingHelper.AddKidsHint(this, JavaCollectionsUtil.SingletonList<IRenderer>(captionRenderer));
                    LayoutTaggingHelper.AddTreeHints(taggingHelper, captionRenderer);
                }
            }
        }

        private bool IsOriginalRenderer() {
            return isOriginalNonSplitRenderer && !IsFooterRenderer() && !IsHeaderRenderer();
        }

        /// <summary><inheritDoc/></summary>
        public override LayoutResult Layout(LayoutContext layoutContext) {
            float? blockMinHeight = RetrieveMinHeight();
            float? blockMaxHeight = RetrieveMaxHeight();
            LayoutArea area = layoutContext.GetArea();
            bool wasParentsHeightClipped = layoutContext.IsClippedHeight();
            bool wasHeightClipped = false;
            Rectangle layoutBox = area.GetBBox().Clone();
            Table tableModel = (Table)GetModelElement();
            if (!tableModel.IsComplete()) {
                SetProperty(Property.MARGIN_BOTTOM, UnitValue.CreatePointValue(0f));
            }
            if (rowRange.GetStartRow() != 0) {
                SetProperty(Property.MARGIN_TOP, UnitValue.CreatePointValue(0f));
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
            bool isFirstOnThePage = 0 == rowRange.GetStartRow() || IsFirstOnRootArea(true);
            if (!IsFooterRenderer() && !IsHeaderRenderer()) {
                if (isOriginalNonSplitRenderer) {
                    bool isSeparated = BorderCollapsePropertyValue.SEPARATE.Equals(this.GetProperty<BorderCollapsePropertyValue?
                        >(Property.BORDER_COLLAPSE));
                    bordersHandler = isSeparated ? (TableBorders)new SeparatedTableBorders(rows, numberOfColumns, GetBorders()
                        , !isAndWasComplete ? rowRange.GetStartRow() : 0) : (TableBorders)new CollapsedTableBorders(rows, numberOfColumns
                        , GetBorders(), !isAndWasComplete ? rowRange.GetStartRow() : 0);
                    bordersHandler.InitializeBorders();
                }
            }
            bordersHandler.SetRowRange(rowRange.GetStartRow(), rowRange.GetFinishRow());
            InitializeHeaderAndFooter(isFirstOnThePage);
            // update
            bordersHandler.UpdateBordersOnNewPage(isOriginalNonSplitRenderer, IsFooterRenderer() || IsHeaderRenderer()
                , this, headerRenderer, footerRenderer);
            if (isOriginalNonSplitRenderer) {
                CorrectRowRange();
            }
            float horizontalBorderSpacing = bordersHandler is SeparatedTableBorders && null != this.GetPropertyAsFloat
                (Property.HORIZONTAL_BORDER_SPACING) ? (float)this.GetPropertyAsFloat(Property.HORIZONTAL_BORDER_SPACING
                ) : 0f;
            float verticalBorderSpacing = bordersHandler is SeparatedTableBorders && null != this.GetPropertyAsFloat(Property
                .VERTICAL_BORDER_SPACING) ? (float)this.GetPropertyAsFloat(Property.VERTICAL_BORDER_SPACING) : 0f;
            if (!isAndWasComplete && !isFirstOnThePage) {
                layoutBox.IncreaseHeight(verticalBorderSpacing);
            }
            if (IsOriginalRenderer()) {
                ApplyMarginsAndPaddingsAndCalculateColumnWidths(layoutBox);
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
            ApplyPaddings(layoutBox, false);
            if (null != blockMaxHeight && blockMaxHeight <= layoutBox.GetHeight() && !true.Equals(GetPropertyAsBoolean
                (Property.FORCED_PLACEMENT))) {
                layoutBox.MoveUp(layoutBox.GetHeight() - (float)blockMaxHeight).SetHeight((float)blockMaxHeight);
                wasHeightClipped = true;
            }
            InitializeCaptionRenderer(GetTable().GetCaption());
            if (captionRenderer != null) {
                float minCaptionWidth = captionRenderer.GetMinMaxWidth().GetMinWidth();
                LayoutResult captionLayoutResult = captionRenderer.Layout(new LayoutContext(new LayoutArea(area.GetPageNumber
                    (), new Rectangle(layoutBox.GetX(), layoutBox.GetY(), Math.Max(tableWidth, minCaptionWidth), layoutBox
                    .GetHeight())), wasHeightClipped || wasParentsHeightClipped));
                if (LayoutResult.FULL != captionLayoutResult.GetStatus()) {
                    return new LayoutResult(LayoutResult.NOTHING, null, null, this, captionLayoutResult.GetCauseOfNothing());
                }
                float captionHeight = captionLayoutResult.GetOccupiedArea().GetBBox().GetHeight();
                if (CaptionSide.BOTTOM.Equals(tableModel.GetCaption().GetProperty<CaptionSide?>(Property.CAPTION_SIDE))) {
                    captionRenderer.Move(0, -(layoutBox.GetHeight() - captionHeight));
                    layoutBox.DecreaseHeight(captionHeight);
                    layoutBox.MoveUp(captionHeight);
                }
                else {
                    layoutBox.DecreaseHeight(captionHeight);
                }
            }
            occupiedArea = new LayoutArea(area.GetPageNumber(), new Rectangle(layoutBox.GetX(), layoutBox.GetY() + layoutBox
                .GetHeight(), (float)tableWidth, 0));
            TargetCounterHandler.AddPageByID(this);
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
                    ), wasHeightClipped || wasParentsHeightClipped));
                if (result.GetStatus() != LayoutResult.FULL) {
                    // we've changed it during footer initialization. However, now we need to process borders again as they were.
                    DeleteOwnProperty(Property.BORDER_BOTTOM);
                    return new LayoutResult(LayoutResult.NOTHING, null, null, this, result.GetCauseOfNothing());
                }
                float footerHeight = result.GetOccupiedArea().GetBBox().GetHeight();
                footerRenderer.Move(0, -(layoutBox.GetHeight() - footerHeight));
                layoutBox.MoveUp(footerHeight).DecreaseHeight(footerHeight);
                // The footer has reserved the space for its top border-spacing.
                // However, since this space is shared with the table, it may be used by the table.
                layoutBox.MoveDown(verticalBorderSpacing).IncreaseHeight(verticalBorderSpacing);
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
                // first row own top border. We will use it while header processing
                topBorderMaxWidth = bordersHandler.GetMaxTopWidth();
                LayoutResult result = headerRenderer.Layout(new LayoutContext(new LayoutArea(area.GetPageNumber(), layoutBox
                    ), wasHeightClipped || wasParentsHeightClipped));
                if (result.GetStatus() != LayoutResult.FULL) {
                    // we've changed it during header initialization. However, now we need to process borders again as they were.
                    DeleteOwnProperty(Property.BORDER_TOP);
                    return new LayoutResult(LayoutResult.NOTHING, null, null, this, result.GetCauseOfNothing());
                }
                float headerHeight = result.GetOccupiedArea().GetBBox().GetHeight();
                layoutBox.DecreaseHeight(headerHeight);
                occupiedArea.GetBBox().MoveDown(headerHeight).IncreaseHeight(headerHeight);
                bordersHandler.FixHeaderOccupiedArea(occupiedArea.GetBBox(), layoutBox);
                // The header has reserved the space for its bottom border-spacing.
                // However, since this space is shared with the table, it may be used by the table.
                layoutBox.IncreaseHeight(verticalBorderSpacing);
                occupiedArea.GetBBox().MoveUp(verticalBorderSpacing).DecreaseHeight(verticalBorderSpacing);
            }
            // Apply spacings. Since occupiedArea was already created it's a bit more difficult for the latter.
            ApplySpacing(layoutBox, horizontalBorderSpacing, verticalBorderSpacing, false);
            ApplySingleSpacing(occupiedArea.GetBBox(), (float)horizontalBorderSpacing, true, false);
            occupiedArea.GetBBox().MoveDown(verticalBorderSpacing / 2);
            topBorderMaxWidth = bordersHandler.GetMaxTopWidth();
            bordersHandler.ApplyLeftAndRightTableBorder(layoutBox, false);
            // Table should have a row and some child elements in order to be considered non empty
            bordersHandler.ApplyTopTableBorder(occupiedArea.GetBBox(), layoutBox, tableModel.IsEmpty() || 0 == rows.Count
                , isAndWasComplete, false);
            if (bordersHandler is SeparatedTableBorders) {
                float bottomBorderWidth = bordersHandler.GetMaxBottomWidth();
                layoutBox.MoveUp(bottomBorderWidth).DecreaseHeight(bottomBorderWidth);
            }
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
                // In the next lines we pretend as if the current row will be the last on the current area:
                // in this case it will be collapsed with the table's bottom border / the footer's top border
                bordersHandler.SetFinishRow(rowRange.GetStartRow() + row);
                IList<Border> rowBottomBorderIfLastOnPage = bordersHandler.GetHorizontalBorder(rowRange.GetStartRow() + row
                     + 1);
                Border widestRowBottomBorder = TableBorderUtil.GetWidestBorder(rowBottomBorderIfLastOnPage);
                float widestRowBottomBorderWidth = null == widestRowBottomBorder ? 0 : widestRowBottomBorder.GetWidth();
                bordersHandler.SetFinishRow(rowRange.GetFinishRow());
                // if cell is in the last row on the page, its borders shouldn't collapse with the next row borders
                while (cellProcessingQueue.Count > 0) {
                    TableRenderer.CellRendererInfo currentCellInfo = cellProcessingQueue.JRemoveFirst();
                    col = currentCellInfo.GetColumn();
                    CellRenderer cell = currentCellInfo.GetCellRenderer();
                    int colspan = (int)cell.GetPropertyAsInteger(Property.COLSPAN);
                    int rowspan = (int)cell.GetPropertyAsInteger(Property.ROWSPAN);
                    if (1 != rowspan) {
                        cellWithBigRowspanAdded = true;
                    }
                    targetOverflowRowIndex[col] = currentCellInfo.GetFinishRowInd();
                    // This cell came from the future (split occurred and we need to place cell with big rowpsan into the current area)
                    bool currentCellHasBigRowspan = (row != currentCellInfo.GetFinishRowInd());
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
                    for (int m = row - 1; m > currentCellInfo.GetFinishRowInd() - rowspan && m >= 0; m--) {
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
                    float[] cellIndents = bordersHandler.GetCellBorderIndents(currentCellInfo.GetFinishRowInd(), col, rowspan, 
                        colspan);
                    if (!(bordersHandler is SeparatedTableBorders)) {
                        // Bottom indent to be applied consists of two parts which should be summed up:
                        // a) half of the border of the current row (in case it is the last row on the area)
                        // b) half of the widest possible bottom border (in case it is the last row on the area)
                        //
                        // The following "image" demonstrates the idea: C represents some content,
                        // 1 represents border, 0 represents not occupied space, - represents
                        // the middle of a horizontal border, | represents vertical border
                        // (the latter could be of customized width as well, however, for the reasons
                        // of this comment it could omitted)
                        // CCCCC|CCCCC
                        // CCCCC|11111
                        // CCCCC|11111
                        // 11111|11111
                        // -----|-----
                        // 11111|11111
                        // 00000|11111
                        // 00000|11111
                        //
                        // The question arises, however: what if the top border of the cell below is wider than the
                        // bottom border of the table. This is already considered: when considering rowHeight
                        // the width of the real collapsed border will be added to it.
                        // It is quite important to understand that in case it is not possible
                        // to add any other row, the current row should be collapsed with the table's bottom
                        // footer's top borders rather than with the next row. If it is the case, iText
                        // will revert collapsing to the one considered in the next calculations.
                        // Be aware that if the col-th border of rowBottomBorderIfLastOnPage is null,
                        // cellIndents[2] might not be null: imagine a table without borders,
                        // a cell with no border (the current cell) and a cell below with some top border.
                        // Nevertheless, a stated above we do not need to consider cellIndents[2] here.
                        float potentialWideCellBorder = null == rowBottomBorderIfLastOnPage[col] ? 0 : rowBottomBorderIfLastOnPage
                            [col].GetWidth();
                        bordersHandler.ApplyCellIndents(cellArea.GetBBox(), cellIndents[0], cellIndents[1], potentialWideCellBorder
                             + widestRowBottomBorderWidth, cellIndents[3], false);
                    }
                    // update cell width
                    cellWidth = cellArea.GetBBox().GetWidth();
                    // create hint for cell if not yet created
                    LayoutTaggingHelper taggingHelper = this.GetProperty<LayoutTaggingHelper>(Property.TAGGING_HELPER);
                    if (taggingHelper != null) {
                        taggingHelper.AddKidsHint(this, JavaCollectionsUtil.SingletonList<IRenderer>(cell));
                        LayoutTaggingHelper.AddTreeHints(taggingHelper, cell);
                    }
                    LayoutResult cellResult = cell.SetParent(this).Layout(new LayoutContext(cellArea, null, childFloatRendererAreas
                        , wasHeightClipped || wasParentsHeightClipped));
                    if (cellWidthProperty != null && cellWidthProperty.IsPercentValue()) {
                        cell.SetProperty(Property.WIDTH, cellWidthProperty);
                        if (null != cellResult.GetOverflowRenderer()) {
                            cellResult.GetOverflowRenderer().SetProperty(Property.WIDTH, cellWidthProperty);
                        }
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
                            if (cellResult.GetStatus() != LayoutResult.NOTHING) {
                                // one should disable cell alignment if it was split
                                splits[col].GetOverflowRenderer().SetProperty(Property.VERTICAL_ALIGNMENT, VerticalAlignment.TOP);
                            }
                        }
                        if (cellResult.GetStatus() == LayoutResult.PARTIAL) {
                            currentRow[col] = (CellRenderer)cellResult.GetSplitRenderer();
                        }
                        else {
                            rows[currentCellInfo.GetFinishRowInd()][col] = null;
                            currentRow[col] = cell;
                            rowMoves.Put(col, currentCellInfo.GetFinishRowInd());
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
                                bool skipLastFooter = null != footerRenderer && tableModel.IsSkipLastFooter() && tableModel.IsComplete() &&
                                     !true.Equals(this.GetOwnProperty<bool?>(Property.FORCED_PLACEMENT));
                                if (skipLastFooter) {
                                    LayoutArea potentialArea = new LayoutArea(area.GetPageNumber(), layoutBox.Clone());
                                    ApplySingleSpacing(potentialArea.GetBBox(), horizontalBorderSpacing, true, true);
                                    // Fix layout area
                                    Border widestRowTopBorder = bordersHandler.GetWidestHorizontalBorder(rowRange.GetStartRow() + row);
                                    if (bordersHandler is CollapsedTableBorders && null != widestRowTopBorder) {
                                        potentialArea.GetBBox().IncreaseHeight((float)widestRowTopBorder.GetWidth() / 2);
                                    }
                                    if (null == headerRenderer) {
                                        potentialArea.GetBBox().IncreaseHeight(bordersHandler.GetMaxTopWidth());
                                    }
                                    bordersHandler.ApplyLeftAndRightTableBorder(potentialArea.GetBBox(), true);
                                    float footerHeight = footerRenderer.GetOccupiedArea().GetBBox().GetHeight();
                                    potentialArea.GetBBox().MoveDown(footerHeight - (float)verticalBorderSpacing / 2).IncreaseHeight(footerHeight
                                        );
                                    iText.Layout.Renderer.TableRenderer overflowRenderer = CreateOverflowRenderer(new Table.RowRange(rowRange.
                                        GetStartRow() + row, rowRange.GetFinishRow()));
                                    overflowRenderer.rows = rows.SubList(row, rows.Count);
                                    overflowRenderer.SetProperty(Property.IGNORE_HEADER, true);
                                    overflowRenderer.SetProperty(Property.IGNORE_FOOTER, true);
                                    overflowRenderer.SetProperty(Property.MARGIN_TOP, UnitValue.CreatePointValue(0));
                                    overflowRenderer.SetProperty(Property.MARGIN_BOTTOM, UnitValue.CreatePointValue(0));
                                    overflowRenderer.SetProperty(Property.MARGIN_LEFT, UnitValue.CreatePointValue(0));
                                    overflowRenderer.SetProperty(Property.MARGIN_RIGHT, UnitValue.CreatePointValue(0));
                                    // we've already applied the top table border on header
                                    if (null != headerRenderer) {
                                        overflowRenderer.SetProperty(Property.BORDER_TOP, Border.NO_BORDER);
                                    }
                                    overflowRenderer.bordersHandler = bordersHandler;
                                    // save old bordersHandler properties
                                    bordersHandler.SkipFooter(overflowRenderer.GetBorders());
                                    if (null != headerRenderer) {
                                        bordersHandler.SkipHeader(overflowRenderer.GetBorders());
                                    }
                                    int savedStartRow = overflowRenderer.bordersHandler.startRow;
                                    overflowRenderer.bordersHandler.SetStartRow(row);
                                    PrepareFooterOrHeaderRendererForLayout(overflowRenderer, potentialArea.GetBBox().GetWidth());
                                    LayoutResult res = overflowRenderer.Layout(new LayoutContext(potentialArea, wasHeightClipped || wasParentsHeightClipped
                                        ));
                                    bordersHandler.SetStartRow(savedStartRow);
                                    if (LayoutResult.FULL == res.GetStatus()) {
                                        if (taggingHelper != null) {
                                            // marking as artifact to get rid of all tagging hints from this renderer
                                            taggingHelper.MarkArtifactHint(footerRenderer);
                                        }
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
                                        if (null != headerRenderer) {
                                            bordersHandler.CollapseTableWithHeader(headerRenderer.bordersHandler, false);
                                        }
                                        bordersHandler.CollapseTableWithFooter(footerRenderer.bordersHandler, false);
                                        bordersHandler.tableBoundingBorders[2] = Border.NO_BORDER;
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
                    if (null != footerRenderer && tableModel.IsComplete() && tableModel.IsSkipLastFooter() && !split && !true.
                        Equals(this.GetOwnProperty<bool?>(Property.FORCED_PLACEMENT))) {
                        LayoutTaggingHelper taggingHelper = this.GetProperty<LayoutTaggingHelper>(Property.TAGGING_HELPER);
                        if (taggingHelper != null) {
                            // marking as artifact to get rid of all tagging hints from this renderer
                            taggingHelper.MarkArtifactHint(footerRenderer);
                        }
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
                    if (!(bordersHandler is SeparatedTableBorders)) {
                        layoutBox.MoveDown(footerRenderer.occupiedArea.GetBBox().GetHeight()).IncreaseHeight(footerRenderer.occupiedArea
                            .GetBBox().GetHeight());
                        // apply the difference to set footer and table left/right margins identical
                        bordersHandler.ApplyLeftAndRightTableBorder(layoutBox, true);
                        PrepareFooterOrHeaderRendererForLayout(footerRenderer, layoutBox.GetWidth());
                        // We've already layouted footer one time in order to know how much place it occupies.
                        // That time, however, we didn't know with which border the top footer's border should be collapsed.
                        // And now, when we possess such knowledge, we are performing the second attempt, but we need to nullify results
                        // from the previous attempt
                        if (bordersHandler is CollapsedTableBorders) {
                            ((CollapsedTableBorders)bordersHandler).SetBottomBorderCollapseWith(null, null);
                        }
                        bordersHandler.CollapseTableWithFooter(footerRenderer.bordersHandler, hasContent || 0 != childRenderers.Count
                            );
                        if (bordersHandler is CollapsedTableBorders) {
                            footerRenderer.SetBorders(CollapsedTableBorders.GetCollapsedBorder(footerRenderer.GetBorders()[2], GetBorders
                                ()[2]), 2);
                        }
                        footerRenderer.Layout(new LayoutContext(new LayoutArea(area.GetPageNumber(), layoutBox), wasHeightClipped 
                            || wasParentsHeightClipped));
                        bordersHandler.ApplyLeftAndRightTableBorder(layoutBox, false);
                        float footerHeight = footerRenderer.GetOccupiedAreaBBox().GetHeight();
                        footerRenderer.Move(0, -(layoutBox.GetHeight() - footerHeight));
                        layoutBox.SetY(footerRenderer.occupiedArea.GetBBox().GetTop()).SetHeight(occupiedArea.GetBBox().GetBottom(
                            ) - layoutBox.GetBottom());
                    }
                }
                if (!split) {
                    childRenderers.AddAll(currChildRenderers);
                    currChildRenderers.Clear();
                }
                if (split && footerRenderer != null) {
                    LayoutTaggingHelper taggingHelper = this.GetProperty<LayoutTaggingHelper>(Property.TAGGING_HELPER);
                    if (taggingHelper != null) {
                        taggingHelper.MarkArtifactHint(footerRenderer);
                    }
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
                                overflowRows.SetCell(targetOverflowRowIndex[col] - row, col, cellOverflow);
                            }
                            else {
                                overflowRows.SetCell(targetOverflowRowIndex[col] - row, col, currentRow[col]);
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
                            EnlargeCell(col, row, minRowspan, currentRow, overflowRows, targetOverflowRowIndex, splitResult);
                        }
                    }
                    ApplySpacing(layoutBox, horizontalBorderSpacing, verticalBorderSpacing, true);
                    ApplySingleSpacing(occupiedArea.GetBBox(), horizontalBorderSpacing, true, true);
                    if (null != footerRenderer) {
                        layoutBox.MoveUp(verticalBorderSpacing).DecreaseHeight(verticalBorderSpacing);
                    }
                    if (null != headerRenderer || !tableModel.IsEmpty()) {
                        layoutBox.DecreaseHeight(verticalBorderSpacing);
                    }
                    if (0 == row && !hasContent && null == headerRenderer) {
                        occupiedArea.GetBBox().MoveUp((float)verticalBorderSpacing / 2);
                    }
                    else {
                        ApplySingleSpacing(occupiedArea.GetBBox(), verticalBorderSpacing, false, true);
                    }
                    // if only footer should be processed
                    if (!isAndWasComplete && null != footerRenderer && 0 == splitResult[0].rows.Count) {
                        layoutBox.IncreaseHeight(verticalBorderSpacing);
                    }
                    // Apply borders if there is no footer
                    if (null == footerRenderer) {
                        // If split renderer does not have any rows, it can mean two things:
                        // - either nothing is placed and the top border, which have been already applied,
                        // should be reverted
                        // - or the only placed row is placed partially.
                        // In the latter case the number of added child renderers should equal to the number of the cells
                        // in the current row (currChildRenderers stands for it)
                        if (!splitResult[0].rows.IsEmpty() || currChildRenderers.Count == childRenderers.Count) {
                            bordersHandler.ApplyBottomTableBorder(occupiedArea.GetBBox(), layoutBox, false);
                        }
                        else {
                            bordersHandler.ApplyTopTableBorder(occupiedArea.GetBBox(), layoutBox, true);
                            // process bottom border of the last added row if there is no footer
                            if (!isAndWasComplete && !isFirstOnThePage) {
                                bordersHandler.ApplyTopTableBorder(occupiedArea.GetBBox(), layoutBox, 0 == childRenderers.Count, true, false
                                    );
                            }
                        }
                    }
                    if (true.Equals(GetPropertyAsBoolean(Property.FILL_AVAILABLE_AREA)) || true.Equals(GetPropertyAsBoolean(Property
                        .FILL_AVAILABLE_AREA_ON_SPLIT))) {
                        ExtendLastRow(splitResult[1].rows[0], layoutBox);
                    }
                    AdjustFooterAndFixOccupiedArea(layoutBox, 0 != heights.Count ? verticalBorderSpacing : 0);
                    AdjustCaptionAndFixOccupiedArea(layoutBox, 0 != heights.Count ? verticalBorderSpacing : 0);
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
                    if (IsKeepTogether(firstCauseOfNothing) && 0 == lastFlushedRowBottomBorder.Count && !true.Equals(GetPropertyAsBoolean
                        (Property.FORCED_PLACEMENT))) {
                        return new LayoutResult(LayoutResult.NOTHING, null, null, this, null == firstCauseOfNothing ? this : firstCauseOfNothing
                            );
                    }
                    else {
                        float footerHeight = null == footerRenderer ? 0 : footerRenderer.GetOccupiedArea().GetBBox().GetHeight();
                        float headerHeight = null == headerRenderer ? 0 : headerRenderer.GetOccupiedArea().GetBBox().GetHeight() -
                             headerRenderer.bordersHandler.GetMaxBottomWidth();
                        float captionHeight = null == captionRenderer ? 0 : captionRenderer.GetOccupiedArea().GetBBox().GetHeight(
                            );
                        float heightDiff = occupiedArea.GetBBox().GetHeight() - footerHeight - headerHeight - captionHeight;
                        int status = JavaUtil.FloatCompare(0, heightDiff) == 0 && (isAndWasComplete || isFirstOnThePage) ? LayoutResult
                            .NOTHING : LayoutResult.PARTIAL;
                        if ((status == LayoutResult.NOTHING && true.Equals(GetPropertyAsBoolean(Property.FORCED_PLACEMENT))) || wasHeightClipped
                            ) {
                            if (wasHeightClipped) {
                                ILogger logger = ITextLogManager.GetLogger(typeof(iText.Layout.Renderer.TableRenderer));
                                logger.LogWarning(iText.IO.Logs.IoLogMessageConstant.CLIP_ELEMENT);
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
                            ApplyPaddings(occupiedArea.GetBBox(), true);
                            ApplyMargins(occupiedArea.GetBBox(), true);
                            LayoutArea editedArea = FloatingHelper.AdjustResultOccupiedAreaForFloatAndClear(this, siblingFloatRendererAreas
                                , layoutContext.GetArea().GetBBox(), clearHeightCorrection, marginsCollapsingEnabled);
                            return new LayoutResult(LayoutResult.FULL, editedArea, splitResult[0], null);
                        }
                        else {
                            UpdateHeightsOnSplit(false, splitResult[0], splitResult[1]);
                            ApplyFixedXOrYPosition(false, layoutBox);
                            ApplyPaddings(occupiedArea.GetBBox(), true);
                            ApplyMargins(occupiedArea.GetBBox(), true);
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
                    ILogger logger = ITextLogManager.GetLogger(typeof(iText.Layout.Renderer.TableRenderer));
                    logger.LogWarning(iText.IO.Logs.IoLogMessageConstant.LAST_ROW_IS_NOT_COMPLETE);
                }
            }
            // process footer renderer with collapsed borders
            if (!(bordersHandler is SeparatedTableBorders) && tableModel.IsComplete() && (0 != lastFlushedRowBottomBorder
                .Count || tableModel.IsEmpty()) && null != footerRenderer) {
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
                footerRenderer.Layout(new LayoutContext(new LayoutArea(area.GetPageNumber(), layoutBox), wasHeightClipped 
                    || wasParentsHeightClipped));
                bordersHandler.ApplyLeftAndRightTableBorder(layoutBox, false);
                float footerHeight = footerRenderer.GetOccupiedAreaBBox().GetHeight();
                footerRenderer.Move(0, -(layoutBox.GetHeight() - footerHeight));
                layoutBox.MoveUp(footerHeight).DecreaseHeight(footerHeight);
            }
            ApplySpacing(layoutBox, horizontalBorderSpacing, verticalBorderSpacing, true);
            ApplySingleSpacing(occupiedArea.GetBBox(), horizontalBorderSpacing, true, true);
            if (null != footerRenderer) {
                layoutBox.MoveUp(verticalBorderSpacing).DecreaseHeight(verticalBorderSpacing);
            }
            if (null != headerRenderer || !tableModel.IsEmpty()) {
                layoutBox.DecreaseHeight(verticalBorderSpacing);
            }
            if (tableModel.IsEmpty() && null == headerRenderer) {
                occupiedArea.GetBBox().MoveUp((float)verticalBorderSpacing / 2);
            }
            else {
                if (isAndWasComplete || 0 != rows.Count) {
                    ApplySingleSpacing(occupiedArea.GetBBox(), verticalBorderSpacing, false, true);
                }
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
            ApplyPaddings(occupiedArea.GetBBox(), true);
            ApplyMargins(occupiedArea.GetBBox(), true);
            // we should process incomplete table's footer only during splitting
            if (!tableModel.IsComplete() && null != footerRenderer) {
                LayoutTaggingHelper taggingHelper = this.GetProperty<LayoutTaggingHelper>(Property.TAGGING_HELPER);
                if (taggingHelper != null) {
                    // marking as artifact to get rid of all tagging hints from this renderer
                    taggingHelper.MarkArtifactHint(footerRenderer);
                }
                footerRenderer = null;
                bordersHandler.SkipFooter(bordersHandler.tableBoundingBorders);
            }
            AdjustFooterAndFixOccupiedArea(layoutBox, null != headerRenderer || !tableModel.IsEmpty() ? verticalBorderSpacing
                 : 0);
            AdjustCaptionAndFixOccupiedArea(layoutBox, null != headerRenderer || !tableModel.IsEmpty() ? verticalBorderSpacing
                 : 0);
            FloatingHelper.RemoveFloatsAboveRendererBottom(siblingFloatRendererAreas, this);
            if (!isAndWasComplete && !isFirstOnThePage && (0 != rows.Count || (null != footerRenderer && tableModel.IsComplete
                ()))) {
                occupiedArea.GetBBox().DecreaseHeight(verticalBorderSpacing);
            }
            LayoutArea editedArea_1 = FloatingHelper.AdjustResultOccupiedAreaForFloatAndClear(this, siblingFloatRendererAreas
                , layoutContext.GetArea().GetBBox(), clearHeightCorrection, marginsCollapsingEnabled);
            return new LayoutResult(LayoutResult.FULL, editedArea_1, null, null, null);
        }

        /// <summary><inheritDoc/></summary>
        public override void Draw(DrawContext drawContext) {
            bool isTagged = drawContext.IsTaggingEnabled();
            LayoutTaggingHelper taggingHelper = null;
            if (isTagged) {
                taggingHelper = this.GetProperty<LayoutTaggingHelper>(Property.TAGGING_HELPER);
                if (taggingHelper == null) {
                    isTagged = false;
                }
                else {
                    TagTreePointer tagPointer = taggingHelper.UseAutoTaggingPointerAndRememberItsPosition(this);
                    if (taggingHelper.CreateTag(this, tagPointer)) {
                        tagPointer.GetProperties().AddAttributes(0, AccessibleAttributesApplier.GetLayoutAttributes(this, tagPointer
                            ));
                    }
                }
            }
            BeginTransformationIfApplied(drawContext.GetCanvas());
            ApplyDestinationsAndAnnotation(drawContext);
            bool relativePosition = IsRelativePosition();
            if (relativePosition) {
                ApplyRelativePositioningTranslation(false);
            }
            BeginElementOpacityApplying(drawContext);
            float captionHeight = null != captionRenderer ? captionRenderer.GetOccupiedArea().GetBBox().GetHeight() : 
                0;
            bool isBottomCaption = CaptionSide.BOTTOM.Equals(0 != captionHeight ? captionRenderer.GetProperty<CaptionSide?
                >(Property.CAPTION_SIDE) : null);
            if (0 != captionHeight) {
                occupiedArea.GetBBox().ApplyMargins(isBottomCaption ? 0 : captionHeight, 0, isBottomCaption ? captionHeight
                     : 0, 0, false);
            }
            DrawBackground(drawContext);
            if (bordersHandler is SeparatedTableBorders && !IsHeaderRenderer() && !IsFooterRenderer()) {
                DrawBorder(drawContext);
            }
            DrawChildren(drawContext);
            DrawPositionedChildren(drawContext);
            if (0 != captionHeight) {
                occupiedArea.GetBBox().ApplyMargins(isBottomCaption ? 0 : captionHeight, 0, isBottomCaption ? captionHeight
                     : 0, 0, true);
            }
            DrawCaption(drawContext);
            EndElementOpacityApplying(drawContext);
            if (relativePosition) {
                ApplyRelativePositioningTranslation(true);
            }
            flushed = true;
            EndTransformationIfApplied(drawContext.GetCanvas());
            if (isTagged) {
                if (isLastRendererForModelElement && ((Table)GetModelElement()).IsComplete()) {
                    taggingHelper.FinishTaggingHint(this);
                }
                taggingHelper.RestoreAutoTaggingPointerPosition(this);
            }
        }

        /// <summary><inheritDoc/></summary>
        public override void DrawChildren(DrawContext drawContext) {
            if (headerRenderer != null) {
                headerRenderer.Draw(drawContext);
            }
            foreach (IRenderer child in childRenderers) {
                child.Draw(drawContext);
            }
            if (bordersHandler is CollapsedTableBorders) {
                DrawBorders(drawContext);
            }
            if (footerRenderer != null) {
                footerRenderer.Draw(drawContext);
            }
        }

        protected internal virtual void DrawBackgrounds(DrawContext drawContext) {
            bool shrinkBackgroundArea = bordersHandler is CollapsedTableBorders && (IsHeaderRenderer() || IsFooterRenderer
                ());
            if (shrinkBackgroundArea) {
                occupiedArea.GetBBox().ApplyMargins(bordersHandler.GetMaxTopWidth() / 2, bordersHandler.GetRightBorderMaxWidth
                    () / 2, bordersHandler.GetMaxBottomWidth() / 2, bordersHandler.GetLeftBorderMaxWidth() / 2, false);
            }
            base.DrawBackground(drawContext);
            if (shrinkBackgroundArea) {
                occupiedArea.GetBBox().ApplyMargins(bordersHandler.GetMaxTopWidth() / 2, bordersHandler.GetRightBorderMaxWidth
                    () / 2, bordersHandler.GetMaxBottomWidth() / 2, bordersHandler.GetLeftBorderMaxWidth() / 2, true);
            }
            if (null != headerRenderer) {
                headerRenderer.DrawBackgrounds(drawContext);
            }
            if (null != footerRenderer) {
                footerRenderer.DrawBackgrounds(drawContext);
            }
        }

        protected internal virtual void DrawCaption(DrawContext drawContext) {
            if (null != captionRenderer && !IsFooterRenderer() && !IsHeaderRenderer()) {
                captionRenderer.Draw(drawContext);
            }
        }

        public override void DrawBackground(DrawContext drawContext) {
            // draw background once for body/header/footer
            if (!IsFooterRenderer() && !IsHeaderRenderer()) {
                DrawBackgrounds(drawContext);
            }
        }

        /// <summary>
        /// Gets a new instance of this class to be used as a next renderer, after this renderer is used, if
        /// <see cref="Layout(iText.Layout.Layout.LayoutContext)"/>
        /// is called more than once.
        /// </summary>
        /// <remarks>
        /// Gets a new instance of this class to be used as a next renderer, after this renderer is used, if
        /// <see cref="Layout(iText.Layout.Layout.LayoutContext)"/>
        /// is called more than once.
        /// <para />
        /// If a renderer overflows to the next area, iText uses this method to create a renderer
        /// for the overflow part. So if one wants to extend
        /// <see cref="TableRenderer"/>
        /// , one should override
        /// this method: otherwise the default method will be used and thus the default rather than the custom
        /// renderer will be created.
        /// </remarks>
        /// <returns>new renderer instance</returns>
        public override IRenderer GetNextRenderer() {
            LogWarningIfGetNextRendererNotOverridden(typeof(iText.Layout.Renderer.TableRenderer), this.GetType());
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
            splitRenderer.occupiedArea = occupiedArea;
            iText.Layout.Renderer.TableRenderer overflowRenderer = CreateOverflowRenderer(new Table.RowRange(rowRange.
                GetStartRow() + row, rowRange.GetFinishRow()));
            if (0 == row && !(hasContent || cellWithBigRowspanAdded) && 0 == rowRange.GetStartRow()) {
                overflowRenderer.isOriginalNonSplitRenderer = isOriginalNonSplitRenderer;
            }
            overflowRenderer.rows = rows.SubList(row, rows.Count);
            overflowRenderer.bordersHandler = bordersHandler;
            return new iText.Layout.Renderer.TableRenderer[] { splitRenderer, overflowRenderer };
        }

        protected internal virtual iText.Layout.Renderer.TableRenderer CreateSplitRenderer(Table.RowRange rowRange
            ) {
            iText.Layout.Renderer.TableRenderer splitRenderer = (iText.Layout.Renderer.TableRenderer)GetNextRenderer();
            splitRenderer.rowRange = rowRange;
            splitRenderer.parent = parent;
            splitRenderer.modelElement = modelElement;
            splitRenderer.childRenderers = childRenderers;
            splitRenderer.AddAllProperties(GetOwnProperties());
            splitRenderer.headerRenderer = headerRenderer;
            splitRenderer.footerRenderer = footerRenderer;
            splitRenderer.isLastRendererForModelElement = false;
            splitRenderer.topBorderMaxWidth = topBorderMaxWidth;
            splitRenderer.captionRenderer = captionRenderer;
            splitRenderer.isOriginalNonSplitRenderer = isOriginalNonSplitRenderer;
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

        public override MinMaxWidth GetMinMaxWidth() {
            if (isOriginalNonSplitRenderer) {
                InitializeTableLayoutBorders();
            }
            float rightMaxBorder = bordersHandler.GetRightBorderMaxWidth();
            float leftMaxBorder = bordersHandler.GetLeftBorderMaxWidth();
            TableWidths tableWidths = new TableWidths(this, MinMaxWidthUtils.GetInfWidth(), true, rightMaxBorder, leftMaxBorder
                );
            float maxColTotalWidth = 0;
            float[] columns = isOriginalNonSplitRenderer ? tableWidths.Layout() : countedColumnWidth;
            foreach (float column in columns) {
                maxColTotalWidth += column;
            }
            float minWidth = isOriginalNonSplitRenderer ? tableWidths.GetMinWidth() : maxColTotalWidth;
            UnitValue marginRightUV = this.GetPropertyAsUnitValue(Property.MARGIN_RIGHT);
            if (!marginRightUV.IsPointValue()) {
                ILogger logger = ITextLogManager.GetLogger(typeof(iText.Layout.Renderer.TableRenderer));
                logger.LogError(MessageFormatUtil.Format(iText.IO.Logs.IoLogMessageConstant.PROPERTY_IN_PERCENTS_NOT_SUPPORTED
                    , Property.MARGIN_RIGHT));
            }
            UnitValue marginLefttUV = this.GetPropertyAsUnitValue(Property.MARGIN_LEFT);
            if (!marginLefttUV.IsPointValue()) {
                ILogger logger = ITextLogManager.GetLogger(typeof(iText.Layout.Renderer.TableRenderer));
                logger.LogError(MessageFormatUtil.Format(iText.IO.Logs.IoLogMessageConstant.PROPERTY_IN_PERCENTS_NOT_SUPPORTED
                    , Property.MARGIN_LEFT));
            }
            float additionalWidth = marginLefttUV.GetValue() + marginRightUV.GetValue() + rightMaxBorder / 2 + leftMaxBorder
                 / 2;
            return new MinMaxWidth(minWidth, maxColTotalWidth, additionalWidth);
        }

        protected internal override bool AllowLastYLineRecursiveExtraction() {
            return false;
        }

        private void InitializeTableLayoutBorders() {
            bool isSeparated = BorderCollapsePropertyValue.SEPARATE.Equals(this.GetProperty<BorderCollapsePropertyValue?
                >(Property.BORDER_COLLAPSE));
            bordersHandler = isSeparated ? (TableBorders)new SeparatedTableBorders(rows, ((Table)GetModelElement()).GetNumberOfColumns
                (), GetBorders()) : (TableBorders)new CollapsedTableBorders(rows, ((Table)GetModelElement()).GetNumberOfColumns
                (), GetBorders());
            bordersHandler.InitializeBorders();
            bordersHandler.SetTableBoundingBorders(GetBorders());
            bordersHandler.SetRowRange(rowRange.GetStartRow(), rowRange.GetFinishRow());
            InitializeHeaderAndFooter(true);
            bordersHandler.UpdateBordersOnNewPage(isOriginalNonSplitRenderer, IsFooterRenderer() || IsHeaderRenderer()
                , this, headerRenderer, footerRenderer);
            CorrectRowRange();
        }

        private void CorrectRowRange() {
            if (rows.Count < rowRange.GetFinishRow() - rowRange.GetStartRow() + 1) {
                rowRange = new Table.RowRange(rowRange.GetStartRow(), rowRange.GetStartRow() + rows.Count - 1);
            }
        }

        public override void DrawBorder(DrawContext drawContext) {
            if (bordersHandler is SeparatedTableBorders) {
                base.DrawBorder(drawContext);
            }
        }

        // Do nothing here. iText handles cell and table borders collapse and draws result borders during #drawBorders()
        protected internal virtual void DrawBorders(DrawContext drawContext) {
            DrawBorders(drawContext, null != headerRenderer, null != footerRenderer);
        }

        private void DrawBorders(DrawContext drawContext, bool hasHeader, bool hasFooter) {
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
                UnitValue topMargin = this.GetPropertyAsUnitValue(Property.MARGIN_TOP);
                if (null != topMargin && !topMargin.IsPointValue()) {
                    ILogger logger = ITextLogManager.GetLogger(typeof(iText.Layout.Renderer.TableRenderer));
                    logger.LogError(MessageFormatUtil.Format(iText.IO.Logs.IoLogMessageConstant.PROPERTY_IN_PERCENTS_NOT_SUPPORTED
                        , Property.MARGIN_LEFT));
                }
                startY -= null == topMargin ? 0 : topMargin.GetValue();
            }
            if (HasProperty(Property.MARGIN_LEFT)) {
                UnitValue leftMargin = this.GetPropertyAsUnitValue(Property.MARGIN_LEFT);
                if (null != leftMargin && !leftMargin.IsPointValue()) {
                    ILogger logger = ITextLogManager.GetLogger(typeof(iText.Layout.Renderer.TableRenderer));
                    logger.LogError(MessageFormatUtil.Format(iText.IO.Logs.IoLogMessageConstant.PROPERTY_IN_PERCENTS_NOT_SUPPORTED
                        , Property.MARGIN_LEFT));
                }
                startX += +(null == leftMargin ? 0 : leftMargin.GetValue());
            }
            // process halves of horizontal bounding borders
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
            bool isTagged = drawContext.IsTaggingEnabled();
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
                        (), ((CollapsedTableBorders)footerRenderer.bordersHandler).GetVerticalBordersCrossingTopHorizontalBorder
                        ());
                }
                else {
                    if (isBottomTablePart) {
                        ((CollapsedTableBorders)bordersHandler).SetBottomBorderCollapseWith(null, null);
                    }
                }
            }
            // we do not need to fix top border, because either this is header or the top border has been already written
            float y1 = startY;
            float[] heightsArray = new float[heights.Count];
            for (int j = 0; j < heights.Count; j++) {
                heightsArray[j] = heights[j];
            }
            // draw vertical borders
            float x1 = startX;
            for (int i = 0; i <= bordersHandler.GetNumberOfColumns(); i++) {
                bordersHandler.DrawVerticalBorder(drawContext.GetCanvas(), new TableBorderDescriptor(i, startY, x1, heightsArray
                    ));
                if (i < countedColumnWidth.Length) {
                    x1 += countedColumnWidth[i];
                }
            }
            // draw horizontal borders
            bool shouldDrawTopBorder = isFooterRendererOfLargeTable || isTopTablePart;
            // if top border is already drawn, we should decrease ordinate
            if (!heights.IsEmpty() && !shouldDrawTopBorder) {
                y1 -= (float)heights[0];
            }
            for (int i = shouldDrawTopBorder ? 0 : 1; i < heights.Count; i++) {
                bordersHandler.DrawHorizontalBorder(drawContext.GetCanvas(), new TableBorderDescriptor(i, startX, y1, countedColumnWidth
                    ));
                y1 -= (float)heights[i];
            }
            // draw bottom border
            // Note for the second condition:
            //!isLastRendererForModelElement is a check that this is a split render. This is the case with the splitting of
            // one cell when part of the cell moves to the next page. Therefore, if such a splitting occurs, a bottom border
            // should be drawn. However, this should not be done for empty renderers that are also created during splitting,
            // but this splitting, if the table does not fit on the page and the next cell is added to the next page.
            // In this case, this code should not be processed, since the border in the above code has already been drawn.
            // TODO DEVSIX-5867 Check hasFooter, so that two footers are not drawn
            if ((!isBottomTablePart && isComplete) || (isBottomTablePart && (isComplete || (!isLastRendererForModelElement
                 && !IsEmptyTableRenderer())))) {
                bordersHandler.DrawHorizontalBorder(drawContext.GetCanvas(), new TableBorderDescriptor(heights.Count, startX
                    , y1, countedColumnWidth));
            }
            if (isTagged) {
                drawContext.GetCanvas().CloseTag();
            }
        }

        private bool IsEmptyTableRenderer() {
            return rows.IsEmpty() && heights.Count == 1 && heights[0] == 0;
        }

        private void ApplyFixedXOrYPosition(bool isXPosition, Rectangle layoutBox) {
            if (IsPositioned()) {
                if (IsFixedLayout()) {
                    if (isXPosition) {
                        float x = (float)this.GetPropertyAsFloat(Property.LEFT);
                        layoutBox.SetX(x);
                    }
                    else {
                        float y = (float)this.GetPropertyAsFloat(Property.BOTTOM);
                        Move(0, y - occupiedArea.GetBBox().GetY());
                    }
                }
            }
        }

        /// <summary>If there is some space left, we will move the footer up, because initially the footer is at the very bottom of the area.
        ///     </summary>
        /// <remarks>
        /// If there is some space left, we will move the footer up, because initially the footer is at the very bottom of the area.
        /// We also will adjust the occupied area by the footer's size if it is present.
        /// </remarks>
        /// <param name="layoutBox">the layout box which represents the area which is left free.</param>
        private void AdjustFooterAndFixOccupiedArea(Rectangle layoutBox, float verticalBorderSpacing) {
            if (footerRenderer != null) {
                footerRenderer.Move(0, layoutBox.GetHeight() + verticalBorderSpacing);
                float footerHeight = footerRenderer.GetOccupiedArea().GetBBox().GetHeight() - verticalBorderSpacing;
                occupiedArea.GetBBox().MoveDown(footerHeight).IncreaseHeight(footerHeight);
            }
        }

        /// <summary>If there is some space left, we will move the caption up, because initially the caption is at the very bottom of the area.
        ///     </summary>
        /// <remarks>
        /// If there is some space left, we will move the caption up, because initially the caption is at the very bottom of the area.
        /// We also will adjust the occupied area by the caption's size if it is present.
        /// </remarks>
        /// <param name="layoutBox">the layout box which represents the area which is left free.</param>
        private void AdjustCaptionAndFixOccupiedArea(Rectangle layoutBox, float verticalBorderSpacing) {
            if (captionRenderer != null) {
                float captionHeight = captionRenderer.GetOccupiedArea().GetBBox().GetHeight();
                occupiedArea.GetBBox().MoveDown(captionHeight).IncreaseHeight(captionHeight);
                if (CaptionSide.BOTTOM.Equals(captionRenderer.GetProperty<CaptionSide?>(Property.CAPTION_SIDE))) {
                    captionRenderer.Move(0, layoutBox.GetHeight() + verticalBorderSpacing);
                }
                else {
                    occupiedArea.GetBBox().MoveUp(captionHeight);
                }
            }
        }

        private void CorrectLayoutedCellsOccupiedAreas(LayoutResult[] splits, int row, int[] targetOverflowRowIndex
            , float? blockMinHeight, Rectangle layoutBox, IList<bool> rowsHasCellWithSetHeight, bool isLastRenderer
            , bool processBigRowspan, bool skip) {
            // Correct last height
            int finish = bordersHandler.GetFinishRow();
            bordersHandler.SetFinishRow(rowRange.GetFinishRow());
            // It's width will be considered only for collapsed borders
            Border currentBorder = bordersHandler.GetWidestHorizontalBorder(finish + 1);
            bordersHandler.SetFinishRow(finish);
            if (skip) {
                // Update bordersHandler
                bordersHandler.tableBoundingBorders[2] = GetBorders()[2];
                bordersHandler.SkipFooter(bordersHandler.tableBoundingBorders);
            }
            float currentBottomIndent = bordersHandler is CollapsedTableBorders ? null == currentBorder ? 0 : currentBorder
                .GetWidth() : 0;
            float realBottomIndent = bordersHandler is CollapsedTableBorders ? bordersHandler.GetMaxBottomWidth() : 0;
            if (0 != heights.Count) {
                heights[heights.Count - 1] = heights[heights.Count - 1] + (realBottomIndent - currentBottomIndent) / 2;
                // Correct occupied area and layoutbox
                occupiedArea.GetBBox().IncreaseHeight((realBottomIndent - currentBottomIndent) / 2).MoveDown((realBottomIndent
                     - currentBottomIndent) / 2);
                layoutBox.DecreaseHeight((realBottomIndent - currentBottomIndent) / 2);
                if (processBigRowspan) {
                    // Process the last row and correct either its height or height of the cell with rowspan
                    CellRenderer[] currentRow = rows[heights.Count];
                    for (int col = 0; col < currentRow.Length; col++) {
                        CellRenderer cell = null == splits[col] ? currentRow[col] : (CellRenderer)splits[col].GetSplitRenderer();
                        if (cell == null) {
                            continue;
                        }
                        float height = 0;
                        int rowspan = (int)cell.GetPropertyAsInteger(Property.ROWSPAN);
                        int colspan = (int)cell.GetPropertyAsInteger(Property.COLSPAN);
                        // Sum the heights of the rows included into the rowspan, except for the last one
                        for (int l = heights.Count - 1 - 1; l > targetOverflowRowIndex[col] - rowspan && l >= 0; l--) {
                            height += (float)heights[l];
                        }
                        float cellHeightInLastRow;
                        float[] indents = bordersHandler.GetCellBorderIndents(bordersHandler is SeparatedTableBorders ? row : targetOverflowRowIndex
                            [col], col, rowspan, colspan);
                        cellHeightInLastRow = cell.GetOccupiedArea().GetBBox().GetHeight() - height + indents[0] / 2 + indents[2] 
                            / 2;
                        if (heights[heights.Count - 1] < cellHeightInLastRow) {
                            // Height of the cell with rowspan is greater than height of the rows included into rowspan
                            if (bordersHandler is SeparatedTableBorders) {
                                float differenceToConsider = cellHeightInLastRow - heights[heights.Count - 1];
                                occupiedArea.GetBBox().MoveDown(differenceToConsider);
                                occupiedArea.GetBBox().IncreaseHeight(differenceToConsider);
                            }
                            heights[heights.Count - 1] = cellHeightInLastRow;
                        }
                        else {
                            // Height of the cell with rowspan is less than height of all the rows included into rowspan
                            float shift = heights[heights.Count - 1] - cellHeightInLastRow;
                            Rectangle bBox = cell.GetOccupiedArea().GetBBox();
                            bBox.MoveDown(shift);
                            bBox.SetHeight(height + heights[heights.Count - 1]);
                            cell.ApplyVerticalAlignment();
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
                // process rowspan
                for (int l = (currentRowIndex < row ? currentRowIndex : heights.Count - 1) - 1; l > (currentRowIndex < row
                     ? currentRowIndex : targetOverflowRowIndex[col]) - rowspan && l >= 0; l--) {
                    height += (float)heights[l];
                    if (false.Equals(rowsHasCellWithSetHeight[l])) {
                        rowspanOffset += additionalCellHeight;
                    }
                }
                height += (float)heights[currentRowIndex < row ? currentRowIndex : heights.Count - 1];
                float[] indents = bordersHandler.GetCellBorderIndents(currentRowIndex < row || bordersHandler is SeparatedTableBorders
                     ? currentRowIndex : targetOverflowRowIndex[col], col, rowspan, colspan);
                height -= indents[0] / 2 + indents[2] / 2;
                // Correcting cell bbox only. We don't need #move() here.
                // This is because of BlockRenderer's specificity regarding occupied area.
                float shift = height - cell.GetOccupiedArea().GetBBox().GetHeight();
                Rectangle bBox = cell.GetOccupiedArea().GetBBox();
                bBox.MoveDown(shift);
                try {
                    cell.Move(0, -(cumulativeShift - rowspanOffset));
                    bBox.SetHeight(height);
                    cell.ApplyVerticalAlignment();
                }
                catch (NullReferenceException) {
                    // TODO Remove try-catch when DEVSIX-1655 is resolved.
                    ILogger logger = ITextLogManager.GetLogger(typeof(iText.Layout.Renderer.TableRenderer));
                    logger.LogError(MessageFormatUtil.Format(iText.IO.Logs.IoLogMessageConstant.OCCUPIED_AREA_HAS_NOT_BEEN_INITIALIZED
                        , "Some of the cell's content might not end up placed correctly."));
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
            bool isSeparated = BorderCollapsePropertyValue.SEPARATE.Equals(this.GetProperty<BorderCollapsePropertyValue?
                >(Property.BORDER_COLLAPSE));
            Table footerOrHeader = footer ? table.GetFooter() : table.GetHeader();
            int innerBorder = footer ? 0 : 2;
            int outerBorder = footer ? 2 : 0;
            iText.Layout.Renderer.TableRenderer renderer = (iText.Layout.Renderer.TableRenderer)footerOrHeader.CreateRendererSubTree
                ().SetParent(this);
            EnsureFooterOrHeaderHasTheSamePropertiesAsParentTableRenderer(renderer);
            bool firstHeader = !footer && rowRange.GetStartRow() == 0 && isOriginalNonSplitRenderer;
            LayoutTaggingHelper taggingHelper = this.GetProperty<LayoutTaggingHelper>(Property.TAGGING_HELPER);
            if (taggingHelper != null) {
                taggingHelper.AddKidsHint(this, JavaCollectionsUtil.SingletonList<IRenderer>(renderer));
                LayoutTaggingHelper.AddTreeHints(taggingHelper, renderer);
                // whether footer is not the last and requires marking as artifact is defined later during table renderer layout
                if (!footer && !firstHeader) {
                    taggingHelper.MarkArtifactHint(renderer);
                }
            }
            if (bordersHandler is SeparatedTableBorders) {
                if (table.IsEmpty()) {
                    // A footer and a header share the same inner border. However it should be processed only ones.
                    if (!footer || null == headerRenderer) {
                        renderer.SetBorders(tableBorders[innerBorder], innerBorder);
                    }
                    bordersHandler.tableBoundingBorders[innerBorder] = Border.NO_BORDER;
                }
                renderer.SetBorders(tableBorders[1], 1);
                renderer.SetBorders(tableBorders[3], 3);
                renderer.SetBorders(tableBorders[outerBorder], outerBorder);
                bordersHandler.tableBoundingBorders[outerBorder] = Border.NO_BORDER;
            }
            else {
                if (bordersHandler is CollapsedTableBorders) {
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
                }
            }
            renderer.bordersHandler = isSeparated ? (TableBorders)new SeparatedTableBorders(renderer.rows, ((Table)renderer
                .GetModelElement()).GetNumberOfColumns(), renderer.GetBorders()) : (TableBorders)new CollapsedTableBorders
                (renderer.rows, ((Table)renderer.GetModelElement()).GetNumberOfColumns(), renderer.GetBorders());
            renderer.bordersHandler.InitializeBorders();
            renderer.bordersHandler.SetRowRange(renderer.rowRange.GetStartRow(), renderer.rowRange.GetFinishRow());
            renderer.bordersHandler.ProcessAllBordersAndEmptyRows();
            renderer.CorrectRowRange();
            return renderer;
        }

        private void EnsureFooterOrHeaderHasTheSamePropertiesAsParentTableRenderer(iText.Layout.Renderer.TableRenderer
             headerOrFooterRenderer) {
            headerOrFooterRenderer.SetProperty(Property.BORDER_COLLAPSE, this.GetProperty<BorderCollapsePropertyValue?
                >(Property.BORDER_COLLAPSE));
            if (bordersHandler is SeparatedTableBorders) {
                headerOrFooterRenderer.SetProperty(Property.HORIZONTAL_BORDER_SPACING, this.GetPropertyAsFloat(Property.HORIZONTAL_BORDER_SPACING
                    ));
                headerOrFooterRenderer.SetProperty(Property.VERTICAL_BORDER_SPACING, this.GetPropertyAsFloat(Property.VERTICAL_BORDER_SPACING
                    ));
                headerOrFooterRenderer.SetProperty(Property.BORDER_LEFT, Border.NO_BORDER);
                headerOrFooterRenderer.SetProperty(Property.BORDER_TOP, Border.NO_BORDER);
                headerOrFooterRenderer.SetProperty(Property.BORDER_RIGHT, Border.NO_BORDER);
                headerOrFooterRenderer.SetProperty(Property.BORDER_BOTTOM, Border.NO_BORDER);
            }
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
            return IsFooterRenderer() && (!((iText.Layout.Renderer.TableRenderer)parent).GetTable().IsComplete() || 0 
                != ((iText.Layout.Renderer.TableRenderer)parent).GetTable().GetLastRowBottomBorder().Count);
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
            if (bordersHandler is SeparatedTableBorders) {
                sum += bordersHandler.GetRightBorderMaxWidth() + bordersHandler.GetLeftBorderMaxWidth();
                float? horizontalSpacing = this.GetPropertyAsFloat(Property.HORIZONTAL_BORDER_SPACING);
                sum += (null == horizontalSpacing) ? 0 : (float)horizontalSpacing;
            }
            else {
                sum += bordersHandler.GetRightBorderMaxWidth() / 2 + bordersHandler.GetLeftBorderMaxWidth() / 2;
            }
            return sum;
        }

        /// <summary>This are a structs used for convenience in layout.</summary>
        private class CellRendererInfo {
            private readonly CellRenderer cellRenderer;

            private readonly int column;

            private readonly int finishRowInd;

            public CellRendererInfo(CellRenderer cellRenderer, int column, int finishRow) {
                this.cellRenderer = cellRenderer;
                this.column = column;
                // When a cell has a rowspan, this is the index of the finish row of the cell.
                // Otherwise, this is simply the index of the row of the cell in the {@link #rows} array.
                this.finishRowInd = finishRow;
            }

            /// <summary>Retrieves the cell renderer.</summary>
            /// <returns>cell renderer</returns>
            public virtual CellRenderer GetCellRenderer() {
                return cellRenderer;
            }

            /// <summary>Retrieves the column.</summary>
            /// <returns>column</returns>
            public virtual int GetColumn() {
                return column;
            }

            /// <summary>Retrieves the finish row index.</summary>
            /// <returns>finish row index</returns>
            public virtual int GetFinishRowInd() {
                return finishRowInd;
            }
        }

        /// <summary>Utility class that copies overflow renderer rows on cell replacement so it won't affect original renderer
        ///     </summary>
        private class OverflowRowsWrapper {
            private TableRenderer overflowRenderer;

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
                overflowRenderer.rows[row] = (CellRenderer[])overflowRenderer.rows[row].Clone();
                return overflowRenderer.rows[row][col] = newCell;
            }
        }

        private void EnlargeCellWithBigRowspan(CellRenderer[] currentRow, TableRenderer.OverflowRowsWrapper overflowRows
            , int row, int col, int minRowspan, TableRenderer[] splitResult, int[] targetOverflowRowIndex) {
            childRenderers.Add(currentRow[col]);
            // shift all cells in the column up
            int i = row;
            for (; i < row + minRowspan && i + 1 < rows.Count && splitResult[1].rows[i + 1 - row][col] != null; i++) {
                overflowRows.SetCell(i - row, col, splitResult[1].rows[i + 1 - row][col]);
                overflowRows.SetCell(i + 1 - row, col, null);
                rows[i][col] = rows[i + 1][col];
                rows[i + 1][col] = null;
            }
            // the number of cells behind is less then minRowspan-1
            // so we should process the last cell in the column as in the case 1 == minRowspan
            if (i != row + minRowspan - 1 && null != rows[i][col]) {
                CellRenderer overflowCell = (CellRenderer)((Cell)rows[i][col].GetModelElement()).GetRenderer().SetParent(this
                    );
                overflowRows.SetCell(i - row, col, null);
                overflowRows.SetCell(targetOverflowRowIndex[col] - row, col, overflowCell);
                CellRenderer originalCell = rows[i][col];
                rows[i][col] = null;
                rows[targetOverflowRowIndex[col]][col] = originalCell;
                originalCell.isLastRendererForModelElement = false;
                overflowCell.SetProperty(Property.TAGGING_HINT_KEY, originalCell.GetProperty<Object>(Property.TAGGING_HINT_KEY
                    ));
            }
        }

        private void EnlargeCell(int col, int row, int minRowspan, CellRenderer[] currentRow, TableRenderer.OverflowRowsWrapper
             overflowRows, int[] targetOverflowRowIndex, TableRenderer[] splitResult) {
            LayoutArea cellOccupiedArea = currentRow[col].GetOccupiedArea();
            if (1 == minRowspan) {
                // Here we use the same cell, but create a new renderer which doesn't have any children,
                // therefore it won't have any content.
                // we will change properties
                CellRenderer overflowCell = (CellRenderer)((Cell)currentRow[col].GetModelElement()).Clone(true).GetRenderer
                    ();
                overflowCell.SetParent(this);
                overflowCell.DeleteProperty(Property.HEIGHT);
                overflowCell.DeleteProperty(Property.MIN_HEIGHT);
                overflowCell.DeleteProperty(Property.MAX_HEIGHT);
                overflowRows.SetCell(0, col, null);
                overflowRows.SetCell(targetOverflowRowIndex[col] - row, col, overflowCell);
                childRenderers.Add(currentRow[col]);
                CellRenderer originalCell = currentRow[col];
                currentRow[col] = null;
                rows[targetOverflowRowIndex[col]][col] = originalCell;
                originalCell.isLastRendererForModelElement = false;
                overflowCell.SetProperty(Property.TAGGING_HINT_KEY, originalCell.GetProperty<Object>(Property.TAGGING_HINT_KEY
                    ));
            }
            else {
                EnlargeCellWithBigRowspan(currentRow, overflowRows, row, col, minRowspan, splitResult, targetOverflowRowIndex
                    );
            }
            overflowRows.GetCell(targetOverflowRowIndex[col] - row, col).occupiedArea = cellOccupiedArea;
        }

//\cond DO_NOT_DOCUMENT
        internal virtual void ApplyMarginsAndPaddingsAndCalculateColumnWidths(Rectangle layoutBox) {
            UnitValue[] margins = GetMargins();
            if (!margins[1].IsPointValue()) {
                ILogger logger = ITextLogManager.GetLogger(typeof(TableRenderer));
                logger.LogError(MessageFormatUtil.Format(iText.IO.Logs.IoLogMessageConstant.PROPERTY_IN_PERCENTS_NOT_SUPPORTED
                    , Property.MARGIN_RIGHT));
            }
            if (!margins[3].IsPointValue()) {
                ILogger logger = ITextLogManager.GetLogger(typeof(TableRenderer));
                logger.LogError(MessageFormatUtil.Format(iText.IO.Logs.IoLogMessageConstant.PROPERTY_IN_PERCENTS_NOT_SUPPORTED
                    , Property.MARGIN_LEFT));
            }
            UnitValue[] paddings = GetPaddings();
            if (!paddings[1].IsPointValue()) {
                ILogger logger = ITextLogManager.GetLogger(typeof(TableRenderer));
                logger.LogError(MessageFormatUtil.Format(iText.IO.Logs.IoLogMessageConstant.PROPERTY_IN_PERCENTS_NOT_SUPPORTED
                    , Property.PADDING_RIGHT));
            }
            if (!paddings[3].IsPointValue()) {
                ILogger logger = ITextLogManager.GetLogger(typeof(TableRenderer));
                logger.LogError(MessageFormatUtil.Format(iText.IO.Logs.IoLogMessageConstant.PROPERTY_IN_PERCENTS_NOT_SUPPORTED
                    , Property.PADDING_LEFT));
            }
            CalculateColumnWidths(layoutBox.GetWidth() - margins[1].GetValue() - margins[3].GetValue() - paddings[1].GetValue
                () - paddings[3].GetValue());
        }
//\endcond
    }
}
