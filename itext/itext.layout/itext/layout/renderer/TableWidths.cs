using System;
using System.Collections.Generic;
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
                                    percentSum += widths[i].GetPercent();
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
                            if (!widths[i].isFixed) {
                                remainWidth += widths[i].max - widths[i].width;
                                flexibleCols++;
                            }
                        }
                        if (remainWidth > 0) {
                            if (flexibleCols > 0) {
                                for (int i = cell.GetCol(); i < cell.GetCol() + cell.GetColspan(); i++) {
                                    if (!widths[i].isFixed) {
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
                        widths[i].SetPercents(colWidth.GetValue());
                    }
                    else {
                        if (!widths[i].isPercent && colWidth.GetValue() >= widths[i].min) {
                            if (!widths[i].isFixed) {
                                widths[i].ResetPoints(colWidth.GetValue());
                            }
                            else {
                                widths[i].SetPoints(colWidth.GetValue());
                            }
                        }
                    }
                }
            }
            //endregion
            // region recalculate
            if (Math.Abs(tableWidth - minSum) < MinMaxWidthUtils.GetEps()) {
                for (int i = 0; i < numberOfColumns; i++) {
                    widths[i].finalWidth = widths[i].min;
                }
            }
            else {
                float sumOfPercents = 0;
                // minTableWidth included fixed columns.
                float minTableWidth = 0;
                float totalNonPercent = 0;
                for (int i = 0; i < widths.Length; i++) {
                    if (widths[i].isPercent) {
                        //some magic...
                        if (sumOfPercents < 100 && sumOfPercents + widths[i].width >= 100) {
                            widths[i].width -= sumOfPercents + widths[i].width - 100;
                        }
                        else {
                            if (sumOfPercents >= 100) {
                                widths[i].ResetPoints(widths[i].min);
                                minTableWidth += widths[i].width;
                            }
                        }
                        sumOfPercents += widths[i].width;
                    }
                    else {
                        minTableWidth += widths[i].min;
                        totalNonPercent += widths[i].width;
                    }
                }
                if (sumOfPercents >= 100) {
                    sumOfPercents = 100;
                    float remainingWidth = tableWidth - minTableWidth;
                    bool recalculatePercents = false;
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
                    //hasExtraSpace means that we have some extra space and(!) may extend columns.
                    //columns shouldn't be more than its max value or its percentage value.
                    bool hasExtraSpace = true;
                    if (unspecifiedTableWidth) {
                        float tableWidthBasedOnPercents = totalNonPercent * 100 / (100 - sumOfPercents);
                        for (int i = 0; i < numberOfColumns; i++) {
                            if (widths[i].isPercent) {
                                tableWidthBasedOnPercents = Math.Max(widths[i].min * 100 / widths[i].width, tableWidthBasedOnPercents);
                            }
                        }
                        if (tableWidthBasedOnPercents <= tableWidth) {
                            for (int i = 0; i < numberOfColumns; i++) {
                                widths[i].finalWidth = widths[i].isPercent ? tableWidthBasedOnPercents * widths[i].width / 100 : widths[i]
                                    .width;
                            }
                            //we don't need more space, columns are done.
                            hasExtraSpace = false;
                        }
                    }
                    //need to decrease some column.
                    if (hasExtraSpace) {
                        // opposite to sumOfPercents, which is sum of percent values.
                        float totalPercent = 0;
                        //if didn't sum columns with percent in case sumOfPercents > 100, recalculating needed.
                        totalNonPercent = 0;
                        float minTotalNonPercent = 0;
                        //sum of non fixed non percent columns.
                        float totalFlexible = 0;
                        bool recalculatePercents = false;
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
                                    totalNonPercent += widths[i].min;
                                    minTotalNonPercent += widths[i].min;
                                    totalFlexible += widths[i].min;
                                    recalculatePercents = true;
                                }
                            }
                            else {
                                widths[i].finalWidth = widths[i].min;
                                totalNonPercent += widths[i].width;
                                minTotalNonPercent += widths[i].min;
                                if (!widths[i].isFixed) {
                                    totalFlexible += widths[i].width;
                                }
                            }
                        }
                        // collision between minWidth and percent value.
                        if (recalculatePercents) {
                            if (totalPercent + minTotalNonPercent > tableWidth) {
                                float extraWidth = tableWidth - minTotalNonPercent;
                                if (sumOfPercents > 0) {
                                    for (int i = 0; i < numberOfColumns; i++) {
                                        if (widths[i].isPercent) {
                                            widths[i].finalWidth = extraWidth * widths[i].width / sumOfPercents;
                                        }
                                    }
                                }
                                //we already use more than we have.
                                hasExtraSpace = false;
                            }
                        }
                        // still has some free space.
                        if (hasExtraSpace) {
                            float extraWidth = tableWidth - minTotalNonPercent - totalPercent;
                            if (totalNonPercent > extraWidth + MinMaxWidthUtils.GetEps()) {
                                float remainingPercentageWidth = totalNonPercent - minTotalNonPercent;
                                if (remainingPercentageWidth > 0) {
                                    for (int i = 0; i < numberOfColumns; i++) {
                                        if (!widths[i].isPercent) {
                                            float addition = widths[i].width - widths[i].min;
                                            widths[i].finalWidth = widths[i].min + addition * extraWidth / remainingPercentageWidth;
                                        }
                                    }
                                }
                            }
                            else {
                                if (totalNonPercent == 0) {
                                    if (totalPercent > 0) {
                                        for (int i = 0; i < numberOfColumns; i++) {
                                            widths[i].finalWidth += extraWidth * widths[i].finalWidth / totalPercent;
                                        }
                                    }
                                }
                                else {
                                    if (totalFlexible == 0) {
                                        float addition = extraWidth - totalNonPercent;
                                        for (int i = 0; i < numberOfColumns; i++) {
                                            if (!widths[i].isPercent) {
                                                widths[i].finalWidth += widths[i].width + addition * widths[i].width / totalNonPercent;
                                            }
                                        }
                                    }
                                    else {
                                        float addition = extraWidth - totalNonPercent;
                                        for (int i = 0; i < numberOfColumns; i++) {
                                            if (!widths[i].isPercent && !widths[i].isFixed) {
                                                widths[i].finalWidth += widths[i].width + addition * widths[i].width / totalFlexible;
                                            }
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
        //region Auto layout
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

        private float[] ExtractWidths() {
            float[] columnWidths = new float[widths.Length];
            for (int i = 0; i < widths.Length; i++) {
                System.Diagnostics.Debug.Assert(widths[i].finalWidth >= 0);
                columnWidths[i] = widths[i].finalWidth;
            }
            return columnWidths;
        }

        private class ColumnWidthData {
            internal readonly float min;

            internal readonly float max;

            internal float width = 0;

            internal float finalWidth = -1;

            internal bool isPercent = false;

            internal bool isFixed = false;

            internal ColumnWidthData(float min, float max) {
                //endregion
                //region Internal classes
                //true means that this column has cell property based width.
                this.min = min > 0 ? min + MinMaxWidthUtils.GetEps() : 0;
                // All browsers implement a size limit on the cell's max width.
                // This limit is based on KHTML's representation that used 16 bits widths.
                this.max = max > 0 ? Math.Min(max + MinMaxWidthUtils.GetEps(), 32760) : 0;
            }

            /// <summary>Gets percents based on 1.</summary>
            public virtual float GetPercent() {
                return width;
            }

            public virtual TableWidths.ColumnWidthData SetPoints(float width) {
                System.Diagnostics.Debug.Assert(!isPercent);
                this.width = Math.Max(this.width, width);
                return this;
            }

            public virtual TableWidths.ColumnWidthData ResetPoints(float width) {
                this.width = width;
                this.isPercent = false;
                return this;
            }

            public virtual TableWidths.ColumnWidthData AddPoints(float width) {
                System.Diagnostics.Debug.Assert(!isPercent);
                this.width += width;
                return this;
            }

            public virtual TableWidths.ColumnWidthData SetPercents(float percent) {
                if (isPercent) {
                    width = Math.Max(width, percent);
                }
                else {
                    isPercent = true;
                    width = percent;
                }
                return this;
            }

            public virtual TableWidths.ColumnWidthData AddPercents(float width) {
                System.Diagnostics.Debug.Assert(isPercent);
                this.width += width;
                return this;
            }

            public virtual TableWidths.ColumnWidthData SetFixed(bool @fixed) {
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
            public virtual bool HasCollision() {
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
            public virtual bool CheckCollision(float availableWidth) {
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
