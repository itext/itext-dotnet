using System;
using System.Collections.Generic;
using iText.IO.Log;
using iText.IO.Util;
using iText.Layout.Element;
using iText.Layout.Minmaxwidth;
using iText.Layout.Properties;

namespace iText.Layout.Renderer {
    internal sealed class TableWidths {
        private TableRenderer tableRenderer;

        private int numberOfColumns;

        private float[] collapsedTableBorders;

        private TableWidths.ColumnWidthData[] widths;

        private IList<TableWidths.CellInfo> cells;

        private float tableWidth;

        private bool unspecifiedTableWidth;

        internal TableWidths(TableRenderer tableRenderer, float availableWidth, float[] collapsedTableBorders) {
            this.tableRenderer = tableRenderer;
            numberOfColumns = ((Table)tableRenderer.GetModelElement()).GetNumberOfColumns();
            this.collapsedTableBorders = collapsedTableBorders != null ? collapsedTableBorders : new float[] { 0, 0, 0
                , 0 };
            CalculateTableWidth(availableWidth);
        }

        internal bool HasFixedLayout() {
            if (unspecifiedTableWidth) {
                return false;
            }
            else {
                String layout = tableRenderer.GetProperty<String>(Property.TABLE_LAYOUT, "auto");
                return "fixed".Equals(layout.ToLowerInvariant());
            }
        }

        internal float[] AutoLayout(float[] minWidths, float[] maxWidths) {
            FillWidths(minWidths, maxWidths);
            FillAndSortCells();
            float minSum = 0;
            foreach (TableWidths.ColumnWidthData width in widths) {
                minSum += width.min;
            }
            //region Process cells
            HashSet<int> minColumns = new HashSet<int>(numberOfColumns);
            foreach (TableWidths.CellInfo cell in cells) {
                //NOTE in automatic layout algorithm percents have higher priority
                UnitValue cellWidth = cell.GetWidth();
                if (cellWidth != null && cellWidth.GetValue() >= 0) {
                    if (cellWidth.IsPercentValue()) {
                        //cellWidth has percent value
                        if (cell.GetColspan() == 1) {
                            widths[cell.GetCol()].SetPercents(cellWidth.GetValue());
                        }
                        else {
                            int pointColumns = 0;
                            float percentSum = 0;
                            for (int i = cell.GetCol(); i < cell.GetCol() + cell.GetColspan(); i++) {
                                if (!widths[i].isPercent) {
                                    pointColumns++;
                                }
                                else {
                                    percentSum += widths[i].width;
                                }
                            }
                            float percentAddition = cellWidth.GetValue() - percentSum;
                            if (percentAddition > 0) {
                                if (pointColumns == 0) {
                                    //ok, add percents to each column
                                    for (int i = cell.GetCol(); i < cell.GetCol() + cell.GetColspan(); i++) {
                                        widths[i].AddPercents(percentAddition / cell.GetColspan());
                                    }
                                }
                                else {
                                    // set percent only to cells without one
                                    for (int i = cell.GetCol(); i < cell.GetCol() + cell.GetColspan(); i++) {
                                        if (!widths[i].isPercent) {
                                            widths[i].SetPercents(percentAddition / pointColumns).SetFixed(true);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else {
                        //cellWidth has point value
                        if (cell.GetCol() == 1) {
                            if (!widths[cell.GetCol()].isPercent) {
                                widths[cell.GetCol()].SetPoints(cellWidth.GetValue()).SetFixed(true);
                                if (widths[cell.GetCol()].HasCollision()) {
                                    minColumns.Add(cell.GetCol());
                                }
                            }
                        }
                        else {
                            int flexibleCols = 0;
                            float colspanRemain = cellWidth.GetValue();
                            for (int i = cell.GetCol(); i < cell.GetCol() + cell.GetColspan(); i++) {
                                if (!widths[i].isPercent) {
                                    colspanRemain -= widths[i].width;
                                    if (!widths[i].isFixed) {
                                        flexibleCols++;
                                    }
                                }
                                else {
                                    colspanRemain = -1;
                                    break;
                                }
                            }
                            if (colspanRemain > 0) {
                                if (flexibleCols > 0) {
                                    // check min width in columns
                                    for (int i = cell.GetCol(); i < cell.GetCol() + cell.GetColspan(); i++) {
                                        if (!widths[i].isFixed && widths[i].CheckCollision(colspanRemain / flexibleCols)) {
                                            widths[i].SetPoints(widths[i].min).SetFixed(true);
                                            if ((colspanRemain -= widths[i].min) <= 0 || flexibleCols-- <= 0) {
                                                break;
                                            }
                                        }
                                    }
                                    if (colspanRemain > 0 && flexibleCols > 0) {
                                        for (int k = cell.GetCol(); k < cell.GetCol() + cell.GetColspan(); k++) {
                                            if (!widths[k].isFixed) {
                                                widths[k].AddPoints(colspanRemain / flexibleCols).SetFixed(true);
                                            }
                                        }
                                    }
                                }
                                else {
                                    for (int i = cell.GetCol(); i < cell.GetCol() + cell.GetColspan(); i++) {
                                        widths[i].AddPoints(colspanRemain / cell.GetColspan());
                                    }
                                }
                            }
                        }
                    }
                }
                else {
                    if (!widths[cell.GetCol()].isFixed) {
                        int flexibleCols = 0;
                        float remainWidth = 0;
                        //if there is no information, try to set max width
                        for (int i = cell.GetCol(); i < cell.GetCol() + cell.GetColspan(); i++) {
                            if (!widths[i].isFixed && !widths[i].isPercent) {
                                remainWidth += widths[i].max - widths[i].width;
                                flexibleCols++;
                            }
                        }
                        if (remainWidth > 0) {
                            if (flexibleCols > 0) {
                                for (int i = cell.GetCol(); i < cell.GetCol() + cell.GetColspan(); i++) {
                                    if (!widths[i].isFixed && !widths[i].isPercent) {
                                        widths[i].AddPoints(remainWidth / flexibleCols);
                                    }
                                }
                            }
                            else {
                                for (int k = cell.GetCol(); k < cell.GetCol() + cell.GetColspan(); k++) {
                                    widths[k].AddPoints(remainWidth / cell.GetColspan());
                                }
                            }
                        }
                    }
                }
            }
            foreach (int? col in minColumns) {
                if (!widths[col].isPercent && widths[col].isFixed && widths[col].HasCollision()) {
                    minSum += widths[col].min - widths[col].width;
                    widths[col].SetPoints(widths[col].min);
                }
            }
            //endregion
            //region Process columns
            //TODO add colgroup information.
            for (int i = 0; i < numberOfColumns; i++) {
                UnitValue colWidth = GetTable().GetColumnWidth(i);
                if (colWidth.GetValue() >= 0) {
                    if (colWidth.IsPercentValue()) {
                        if (!widths[i].isPercent && widths[i].isFixed && widths[i].width > widths[i].min) {
                            widths[i].max = widths[i].width;
                            widths[i].SetFixed(false);
                        }
                        if (!widths[i].isPercent) {
                            widths[i].SetPercents(colWidth.GetValue());
                        }
                    }
                    else {
                        if (!widths[i].isPercent && colWidth.GetValue() >= widths[i].min) {
                            if (widths[i].isFixed) {
                                widths[i].SetPoints(colWidth.GetValue());
                            }
                            else {
                                widths[i].ResetPoints(colWidth.GetValue());
                            }
                        }
                    }
                }
            }
            //endregion
            // region recalculate
            if (tableWidth - minSum < 0) {
                for (int i = 0; i < numberOfColumns; i++) {
                    widths[i].finalWidth = widths[i].min;
                }
            }
            else {
                float sumOfPercents = 0;
                // minTableWidth include only non percent columns.
                float minTableWidth = 0;
                float totalNonPercent = 0;
                // validate sumOfPercents, last columns will be set min width, if sum > 100.
                for (int i = 0; i < widths.Length; i++) {
                    if (widths[i].isPercent) {
                        if (sumOfPercents < 100 && sumOfPercents + widths[i].width > 100) {
                            widths[i].width -= sumOfPercents + widths[i].width - 100;
                            sumOfPercents += widths[i].width;
                            Warn100percent();
                        }
                        else {
                            if (sumOfPercents >= 100) {
                                widths[i].ResetPoints(widths[i].min);
                                minTableWidth += widths[i].width;
                                Warn100percent();
                            }
                            else {
                                sumOfPercents += widths[i].width;
                            }
                        }
                    }
                    else {
                        minTableWidth += widths[i].min;
                        totalNonPercent += widths[i].width;
                    }
                }
                System.Diagnostics.Debug.Assert(sumOfPercents <= 100);
                bool toBalance = true;
                if (unspecifiedTableWidth) {
                    float tableWidthBasedOnPercents = sumOfPercents < 100 ? totalNonPercent * 100 / (100 - sumOfPercents) : 0;
                    for (int i = 0; i < numberOfColumns; i++) {
                        if (widths[i].isPercent) {
                            tableWidthBasedOnPercents = Math.Max(widths[i].max * 100 / widths[i].width, tableWidthBasedOnPercents);
                        }
                    }
                    if (tableWidthBasedOnPercents <= tableWidth) {
                        tableWidth = tableWidthBasedOnPercents;
                        //we don't need more space, columns are done.
                        toBalance = false;
                    }
                }
                if (sumOfPercents < 100 && totalNonPercent == 0) {
                    // each column has percent value but sum < 100%
                    // upscale percents
                    for (int i = 0; i < widths.Length; i++) {
                        widths[i].width = 100 * widths[i].width / sumOfPercents;
                    }
                    sumOfPercents = 100;
                }
                if (!toBalance) {
                    for (int i = 0; i < numberOfColumns; i++) {
                        widths[i].finalWidth = widths[i].isPercent ? tableWidth * widths[i].width / 100 : widths[i].width;
                    }
                }
                else {
                    if (sumOfPercents >= 100) {
                        sumOfPercents = 100;
                        bool recalculatePercents = false;
                        float remainingWidth = tableWidth - minTableWidth;
                        for (int i = 0; i < numberOfColumns; i++) {
                            if (widths[i].isPercent) {
                                if (remainingWidth * widths[i].width >= widths[i].min) {
                                    widths[i].finalWidth = remainingWidth * widths[i].width / 100;
                                }
                                else {
                                    widths[i].finalWidth = widths[i].min;
                                    widths[i].isPercent = false;
                                    remainingWidth -= widths[i].min;
                                    sumOfPercents -= widths[i].width;
                                    recalculatePercents = true;
                                }
                            }
                        }
                        if (recalculatePercents) {
                            for (int i = 0; i < numberOfColumns; i++) {
                                if (widths[i].isPercent) {
                                    widths[i].finalWidth = remainingWidth * widths[i].width / sumOfPercents;
                                }
                            }
                        }
                    }
                    else {
                        // We either have some extra space and may extend columns in case fixed table width,
                        // or have to decrease columns to fit table width.
                        //
                        // columns shouldn't be more than its max value in case unspecified table width.
                        //columns shouldn't be more than its percentage value.
                        // opposite to sumOfPercents, which is sum of percent values.
                        float totalPercent = 0;
                        float minTotalNonPercent = 0;
                        float fixedAddition = 0;
                        float flexibleAddition = 0;
                        //sum of non fixed non percent columns.
                        for (int i = 0; i < numberOfColumns; i++) {
                            if (widths[i].isPercent) {
                                if (tableWidth * widths[i].width >= widths[i].min) {
                                    widths[i].finalWidth = tableWidth * widths[i].width / 100;
                                    totalPercent += widths[i].finalWidth;
                                }
                                else {
                                    sumOfPercents -= widths[i].width;
                                    widths[i].ResetPoints(widths[i].min);
                                    widths[i].finalWidth = widths[i].min;
                                    minTotalNonPercent += widths[i].min;
                                }
                            }
                            else {
                                widths[i].finalWidth = widths[i].min;
                                minTotalNonPercent += widths[i].min;
                                float addition = widths[i].width - widths[i].min;
                                if (widths[i].isFixed) {
                                    fixedAddition += addition;
                                }
                                else {
                                    flexibleAddition += addition;
                                }
                            }
                        }
                        if (totalPercent + minTotalNonPercent > tableWidth) {
                            // collision between minWidth and percent value.
                            float extraWidth = tableWidth - minTotalNonPercent;
                            if (sumOfPercents > 0) {
                                for (int i = 0; i < numberOfColumns; i++) {
                                    if (widths[i].isPercent) {
                                        widths[i].finalWidth = extraWidth * widths[i].width / sumOfPercents;
                                    }
                                }
                            }
                        }
                        else {
                            float extraWidth = tableWidth - totalPercent - minTotalNonPercent;
                            if (fixedAddition > 0 && (extraWidth < fixedAddition || flexibleAddition == 0)) {
                                for (int i = 0; i < numberOfColumns; i++) {
                                    if (!widths[i].isPercent && widths[i].isFixed) {
                                        widths[i].finalWidth += (widths[i].width - widths[i].min) * extraWidth / fixedAddition;
                                    }
                                }
                            }
                            else {
                                extraWidth -= fixedAddition;
                                if (extraWidth < flexibleAddition) {
                                    for (int i = 0; i < numberOfColumns; i++) {
                                        if (!widths[i].isPercent) {
                                            if (widths[i].isFixed) {
                                                widths[i].finalWidth = widths[i].width;
                                            }
                                            else {
                                                widths[i].finalWidth += (widths[i].width - widths[i].min) * extraWidth / flexibleAddition;
                                            }
                                        }
                                    }
                                }
                                else {
                                    float totalFixed = 0;
                                    float totalFlexible = 0;
                                    for (int i = 0; i < numberOfColumns; i++) {
                                        if (!widths[i].isPercent) {
                                            if (widths[i].isFixed) {
                                                widths[i].finalWidth = widths[i].width;
                                                totalFixed += widths[i].width;
                                            }
                                            else {
                                                totalFlexible += widths[i].width;
                                            }
                                        }
                                    }
                                    extraWidth = tableWidth - totalPercent - totalFixed;
                                    for (int i = 0; i < numberOfColumns; i++) {
                                        if (!widths[i].isPercent && !widths[i].isFixed) {
                                            widths[i].finalWidth = widths[i].width * extraWidth / totalFlexible;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            //endregion
            return ExtractWidths();
        }

        internal float[] FixedLayout() {
            float[] columnWidths = new float[numberOfColumns];
            //fill columns from col info
            for (int i = 0; i < numberOfColumns; i++) {
                UnitValue colWidth = GetTable().GetColumnWidth(i);
                if (colWidth == null || colWidth.GetValue() < 0) {
                    columnWidths[i] = -1;
                }
                else {
                    if (colWidth.IsPercentValue()) {
                        columnWidths[i] = colWidth.GetValue() * tableWidth / 100;
                    }
                    else {
                        columnWidths[i] = colWidth.GetValue();
                    }
                }
            }
            //fill columns with -1 from cell info.
            int processedColumns = 0;
            float remainWidth = tableWidth;
            for (int i = 0; i < numberOfColumns; i++) {
                if (columnWidths[i] == -1) {
                    CellRenderer cell = tableRenderer.rows[0][i];
                    if (cell != null) {
                        float? cellWidth = cell.RetrieveUnitValue(tableWidth, Property.WIDTH);
                        if (cellWidth != null && cellWidth >= 0) {
                            int colspan = ((Cell)cell.GetModelElement()).GetColspan();
                            for (int j = 0; j < colspan; j++) {
                                columnWidths[i + j] = cellWidth / colspan;
                            }
                            remainWidth -= columnWidths[i];
                            processedColumns++;
                        }
                    }
                }
                else {
                    remainWidth -= columnWidths[i];
                    processedColumns++;
                }
            }
            if (remainWidth > 0) {
                if (numberOfColumns == processedColumns) {
                    //Set remainWidth to all columns.
                    for (int i = 0; i < numberOfColumns; i++) {
                        columnWidths[i] += remainWidth / numberOfColumns;
                    }
                }
                else {
                    // Set all remain width to the unprocessed columns.
                    for (int i = 0; i < numberOfColumns; i++) {
                        if (columnWidths[i] == -1) {
                            columnWidths[i] = remainWidth / (numberOfColumns - processedColumns);
                        }
                    }
                }
            }
            else {
                if (numberOfColumns != processedColumns) {
                    //TODO shall we add warning?
                    for (int i = 0; i < numberOfColumns; i++) {
                        if (columnWidths[i] == -1) {
                            columnWidths[i] = 0;
                        }
                    }
                }
            }
            return columnWidths;
        }

        //region Common methods
        private void CalculateTableWidth(float availableWidth) {
            float? originalTableWidth = tableRenderer.RetrieveUnitValue(availableWidth, Property.WIDTH);
            if (originalTableWidth == null || float.IsNaN(originalTableWidth) || originalTableWidth <= 0) {
                tableWidth = availableWidth;
                unspecifiedTableWidth = true;
            }
            else {
                tableWidth = originalTableWidth < availableWidth ? originalTableWidth : availableWidth;
                unspecifiedTableWidth = false;
            }
            tableWidth -= GetMaxLeftBorder() / 2 + GetMaxRightBorder() / 2;
        }

        private float GetMaxLeftBorder() {
            return collapsedTableBorders[3];
        }

        private float GetMaxRightBorder() {
            return collapsedTableBorders[1];
        }

        private Table GetTable() {
            return (Table)tableRenderer.GetModelElement();
        }

        //endregion
        //region Auto layout utils
        private void FillWidths(float[] minWidths, float[] maxWidths) {
            widths = new TableWidths.ColumnWidthData[minWidths.Length];
            for (int i = 0; i < widths.Length; i++) {
                widths[i] = new TableWidths.ColumnWidthData(minWidths[i], maxWidths[i]);
            }
        }

        private void FillAndSortCells() {
            cells = new List<TableWidths.CellInfo>();
            if (tableRenderer.headerRenderer != null) {
                FillRendererCells(tableRenderer.headerRenderer, TableWidths.CellInfo.HEADER);
            }
            FillRendererCells(tableRenderer, TableWidths.CellInfo.BODY);
            if (tableRenderer.footerRenderer != null) {
                FillRendererCells(tableRenderer.footerRenderer, TableWidths.CellInfo.FOOTER);
            }
            // Cells are sorted, because we need to process cells without colspan
            // and process from top left to bottom right for other cases.
            JavaCollectionsUtil.Sort(cells);
        }

        private void FillRendererCells(TableRenderer renderer, byte region) {
            for (int row = 0; row < renderer.rows.Count; row++) {
                for (int col = 0; col < numberOfColumns; col++) {
                    CellRenderer cell = renderer.rows[row][col];
                    if (cell != null) {
                        cells.Add(new TableWidths.CellInfo(cell, region));
                    }
                }
            }
        }

        private void Warn100percent() {
            ILogger logger = LoggerFactory.GetLogger(typeof(iText.Layout.Renderer.TableWidths));
            logger.Warn(iText.IO.LogMessageConstant.SUM_OF_TABLE_COLUMNS_IS_GREATER_THAN_100);
        }

        private float[] ExtractWidths() {
            float actualWidth = 0;
            float[] columnWidths = new float[widths.Length];
            for (int i = 0; i < widths.Length; i++) {
                System.Diagnostics.Debug.Assert(widths[i].finalWidth >= 0);
                columnWidths[i] = widths[i].finalWidth;
                actualWidth += widths[i].finalWidth;
            }
            if (actualWidth > tableWidth + MinMaxWidthUtils.GetEps() * widths.Length) {
                ILogger logger = LoggerFactory.GetLogger(typeof(iText.Layout.Renderer.TableWidths));
                logger.Warn(iText.IO.LogMessageConstant.TABLE_WIDTH_IS_MORE_THAN_EXPECTED_DUE_TO_MIN_WIDTH);
            }
            return columnWidths;
        }

        private class ColumnWidthData {
            internal readonly float min;

            internal float max;

            internal float width = 0;

            internal float finalWidth = -1;

            internal bool isPercent = false;

            internal bool isFixed = false;

            internal ColumnWidthData(float min, float max) {
                //endregion
                //region Internal classes
                //true means that this column has cell property based width.
                System.Diagnostics.Debug.Assert(min >= 0);
                System.Diagnostics.Debug.Assert(max >= 0);
                this.min = min > 0 ? min + MinMaxWidthUtils.GetEps() : 0;
                // All browsers implement a size limit on the cell's max width.
                // This limit is based on KHTML's representation that used 16 bits widths.
                this.max = max > 0 ? Math.Min(max + MinMaxWidthUtils.GetEps(), 32760) : 0;
            }

            internal virtual TableWidths.ColumnWidthData SetPoints(float width) {
                System.Diagnostics.Debug.Assert(!isPercent);
                this.width = Math.Max(this.width, width);
                return this;
            }

            internal virtual TableWidths.ColumnWidthData ResetPoints(float width) {
                this.width = width;
                this.isPercent = false;
                return this;
            }

            internal virtual TableWidths.ColumnWidthData AddPoints(float width) {
                System.Diagnostics.Debug.Assert(!isPercent);
                this.width += width;
                return this;
            }

            internal virtual TableWidths.ColumnWidthData SetPercents(float percent) {
                if (isPercent) {
                    width = Math.Max(width, percent);
                }
                else {
                    isPercent = true;
                    width = percent;
                }
                return this;
            }

            internal virtual TableWidths.ColumnWidthData AddPercents(float width) {
                System.Diagnostics.Debug.Assert(isPercent);
                this.width += width;
                return this;
            }

            internal virtual TableWidths.ColumnWidthData SetFixed(bool @fixed) {
                this.isFixed = @fixed;
                return this;
            }

            /// <summary>Check collusion between min value and point width</summary>
            /// <returns>
            /// true, if
            /// <see cref="min"/>
            /// greater than
            /// <see cref="width"/>
            /// .
            /// </returns>
            internal virtual bool HasCollision() {
                System.Diagnostics.Debug.Assert(!isPercent);
                return min > width;
            }

            /// <summary>Check collusion between min value and available point width.</summary>
            /// <param name="availableWidth">additional available point width.</param>
            /// <returns>
            /// true, if
            /// <see cref="min"/>
            /// greater than (
            /// <see cref="width"/>
            /// + additionalWidth).
            /// </returns>
            internal virtual bool CheckCollision(float availableWidth) {
                System.Diagnostics.Debug.Assert(!isPercent);
                return min > width + availableWidth;
            }

            public override String ToString() {
                return "w=" + width + (isPercent ? "%" : "pt") + (isFixed ? " !!" : "") + ", min=" + min + ", max=" + max 
                    + ", finalWidth=" + finalWidth;
            }
        }

        private class CellInfo : IComparable<TableWidths.CellInfo> {
            private const byte HEADER = 1;

            private const byte BODY = 2;

            private const byte FOOTER = 3;

            private CellRenderer cell;

            private byte region;

            internal CellInfo(CellRenderer cell, byte region) {
                this.cell = cell;
                this.region = region;
            }

            internal virtual CellRenderer GetCell() {
                return cell;
            }

            internal virtual int GetCol() {
                return ((Cell)cell.GetModelElement()).GetCol();
            }

            internal virtual int GetColspan() {
                return ((Cell)cell.GetModelElement()).GetColspan();
            }

            internal virtual int GetRow() {
                return ((Cell)cell.GetModelElement()).GetRow();
            }

            internal virtual int GetRowspan() {
                return ((Cell)cell.GetModelElement()).GetRowspan();
            }

            internal virtual UnitValue GetWidth() {
                return cell.GetProperty<UnitValue>(Property.WIDTH);
            }

            public virtual int CompareTo(TableWidths.CellInfo o) {
                if (GetColspan() == 1 ^ o.GetColspan() == 1) {
                    return GetColspan() - o.GetColspan();
                }
                if (region == o.region && GetRow() == o.GetRow()) {
                    return GetCol() + GetColspan() - o.GetCol() - o.GetColspan();
                }
                return region == o.region ? GetRow() - o.GetRow() : region - o.region;
            }
        }

        //endregion
        public override String ToString() {
            return "width=" + tableWidth + (unspecifiedTableWidth ? "" : "!!");
        }
    }
}
