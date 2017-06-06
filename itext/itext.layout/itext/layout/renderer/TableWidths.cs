/*
This file is part of the iText (R) project.
Copyright (c) 1998-2017 iText Group NV
Authors: iText Software.

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
using iText.IO.Util;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Minmaxwidth;
using iText.Layout.Properties;

namespace iText.Layout.Renderer {
    internal sealed class TableWidths {
        private readonly TableRenderer tableRenderer;

        private readonly int numberOfColumns;

        private readonly float rightBorderMaxWidth;

        private readonly float leftBorderMaxWidth;

        private readonly TableWidths.ColumnWidthData[] widths;

        private IList<TableWidths.CellInfo> cells;

        private float tableWidth;

        private bool fixedTableWidth;

        private bool fixedTableLayout = false;

        private float tableMinWidth;

        internal TableWidths(TableRenderer tableRenderer, float availableWidth, bool calculateTableMaxWidth, float
             rightBorderMaxWidth, float leftBorderMaxWidth) {
            this.tableRenderer = tableRenderer;
            this.numberOfColumns = ((Table)tableRenderer.GetModelElement()).GetNumberOfColumns();
            this.widths = new TableWidths.ColumnWidthData[numberOfColumns];
            this.rightBorderMaxWidth = rightBorderMaxWidth;
            this.leftBorderMaxWidth = leftBorderMaxWidth;
            CalculateTableWidth(availableWidth, calculateTableMaxWidth);
        }

        internal bool HasFixedLayout() {
            return fixedTableLayout;
        }

        internal float[] Layout() {
            if (HasFixedLayout()) {
                return FixedLayout();
            }
            else {
                return AutoLayout();
            }
        }

        internal float GetMinWidth() {
            return tableMinWidth;
        }

        internal float[] AutoLayout() {
            System.Diagnostics.Debug.Assert(tableRenderer.GetTable().IsComplete());
            FillAndSortCells();
            CalculateMinMaxWidths();
            float minSum = 0;
            foreach (TableWidths.ColumnWidthData width in widths) {
                minSum += width.min;
            }
            //region Process cells
            foreach (TableWidths.CellInfo cell in cells) {
                // For automatic layout algorithm percents have higher priority
                // value must be > 0, while for fixed layout >= 0
                UnitValue cellWidth = GetCellWidth(cell.GetCell(), false);
                if (cellWidth != null) {
                    System.Diagnostics.Debug.Assert(cellWidth.GetValue() > 0);
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
                                            widths[i].SetPercents(percentAddition / pointColumns);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else {
                        //cellWidth has point value
                        if (cell.GetColspan() == 1) {
                            if (!widths[cell.GetCol()].isPercent) {
                                if (widths[cell.GetCol()].min <= cellWidth.GetValue()) {
                                    widths[cell.GetCol()].SetPoints(cellWidth.GetValue()).SetFixed(true);
                                }
                                else {
                                    widths[cell.GetCol()].SetPoints(widths[cell.GetCol()].min);
                                }
                            }
                        }
                        else {
                            int flexibleCols = 0;
                            float remainWidth = cellWidth.GetValue();
                            for (int i = cell.GetCol(); i < cell.GetCol() + cell.GetColspan(); i++) {
                                if (!widths[i].isPercent) {
                                    remainWidth -= widths[i].width;
                                    if (!widths[i].isFixed) {
                                        flexibleCols++;
                                    }
                                }
                                else {
                                    // if any col has percent value, we cannot predict remaining width.
                                    remainWidth = 0;
                                    break;
                                }
                            }
                            if (remainWidth > 0) {
                                int[] flexibleColIndexes = ArrayUtil.FillWithValue(new int[cell.GetColspan()], -1);
                                if (flexibleCols > 0) {
                                    // check min width in columns
                                    for (int i = cell.GetCol(); i < cell.GetCol() + cell.GetColspan(); i++) {
                                        if (!widths[i].IsFlexible()) {
                                            continue;
                                        }
                                        if (widths[i].min > widths[i].width + remainWidth / flexibleCols) {
                                            widths[i].ResetPoints(widths[i].min);
                                            remainWidth -= widths[i].min - widths[i].width;
                                            flexibleCols--;
                                            if (flexibleCols == 0 || remainWidth <= 0) {
                                                break;
                                            }
                                        }
                                        else {
                                            flexibleColIndexes[i - cell.GetCol()] = i;
                                        }
                                    }
                                    if (flexibleCols > 0 && remainWidth > 0) {
                                        for (int i = 0; i < flexibleColIndexes.Length; i++) {
                                            if (flexibleColIndexes[i] >= 0) {
                                                widths[flexibleColIndexes[i]].AddPoints(remainWidth / flexibleCols).SetFixed(true);
                                            }
                                        }
                                    }
                                }
                                else {
                                    for (int i = cell.GetCol(); i < cell.GetCol() + cell.GetColspan(); i++) {
                                        widths[i].AddPoints(remainWidth / cell.GetColspan());
                                    }
                                }
                            }
                        }
                    }
                }
                else {
                    if (widths[cell.GetCol()].IsFlexible()) {
                        //if there is no information, try to set max width
                        int flexibleCols = 0;
                        float remainWidth = 0;
                        for (int i = cell.GetCol(); i < cell.GetCol() + cell.GetColspan(); i++) {
                            if (widths[i].IsFlexible()) {
                                remainWidth += widths[i].max - widths[i].width;
                                flexibleCols++;
                            }
                        }
                        if (remainWidth > 0) {
                            // flexibleCols > 0 too
                            for (int i = cell.GetCol(); i < cell.GetCol() + cell.GetColspan(); i++) {
                                if (widths[i].IsFlexible()) {
                                    widths[i].AddPoints(remainWidth / flexibleCols);
                                }
                            }
                        }
                    }
                }
            }
            //endregion
            //region Process columns
            //TODO add colgroup information.
            for (int i = 0; i < numberOfColumns; i++) {
                UnitValue colWidth = GetTable().GetColumnWidth(i);
                if (colWidth != null && colWidth.GetValue() > 0) {
                    if (colWidth.IsPercentValue()) {
                        if (!widths[i].isPercent) {
                            if (widths[i].isFixed && widths[i].width > widths[i].min) {
                                widths[i].max = widths[i].width;
                            }
                            widths[i].SetPercents(colWidth.GetValue());
                        }
                    }
                    else {
                        if (!widths[i].isPercent && colWidth.GetValue() >= widths[i].min) {
                            if (widths[i].isFixed) {
                                widths[i].SetPoints(colWidth.GetValue());
                            }
                            else {
                                widths[i].ResetPoints(colWidth.GetValue()).SetFixed(true);
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
                                minTableWidth += widths[i].min;
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
                if (!fixedTableWidth) {
                    float tableWidthBasedOnPercents = sumOfPercents < 100 ? totalNonPercent * 100 / (100 - sumOfPercents) : 0;
                    for (int i = 0; i < numberOfColumns; i++) {
                        if (widths[i].isPercent) {
                            tableWidthBasedOnPercents = Math.Max(widths[i].max * 100 / widths[i].width, tableWidthBasedOnPercents);
                        }
                    }
                    if (tableWidthBasedOnPercents <= tableWidth) {
                        tableWidth = tableWidthBasedOnPercents;
                        //we don't need more space, columns are done based on column's max width.
                        toBalance = false;
                    }
                }
                if (sumOfPercents > 0 && sumOfPercents < 100 && totalNonPercent == 0) {
                    // each column has percent value but sum < 100%
                    // upscale percents
                    for (int i = 0; i < widths.Length; i++) {
                        widths[i].width = 100 * widths[i].width / sumOfPercents;
                    }
                    sumOfPercents = 100;
                }
                if (!toBalance) {
                    //column width based on max width, no need to check min width.
                    for (int i = 0; i < numberOfColumns; i++) {
                        widths[i].finalWidth = widths[i].isPercent ? tableWidth * widths[i].width / 100 : widths[i].width;
                    }
                }
                else {
                    if (sumOfPercents >= 100) {
                        sumOfPercents = 100;
                        bool recalculatePercents = false;
                        float remainWidth = tableWidth - minTableWidth;
                        for (int i = 0; i < numberOfColumns; i++) {
                            if (widths[i].isPercent) {
                                if (remainWidth * widths[i].width / 100 >= widths[i].min) {
                                    widths[i].finalWidth = remainWidth * widths[i].width / 100;
                                }
                                else {
                                    widths[i].finalWidth = widths[i].min;
                                    widths[i].isPercent = false;
                                    remainWidth -= widths[i].min;
                                    sumOfPercents -= widths[i].width;
                                    recalculatePercents = true;
                                }
                            }
                            else {
                                widths[i].finalWidth = widths[i].min;
                            }
                        }
                        if (recalculatePercents) {
                            for (int i = 0; i < numberOfColumns; i++) {
                                if (widths[i].isPercent) {
                                    widths[i].finalWidth = remainWidth * widths[i].width / sumOfPercents;
                                }
                            }
                        }
                    }
                    else {
                        // We either have some extra space and may extend columns in case fixed table width,
                        // or have to decrease columns to fit table width.
                        //
                        // columns shouldn't be more than its max value in case unspecified table width.
                        // columns shouldn't be more than its percentage value.
                        // opposite to sumOfPercents, which is sum of percent values in points.
                        float totalPercent = 0;
                        float minTotalNonPercent = 0;
                        float fixedAddition = 0;
                        float flexibleAddition = 0;
                        bool hasFlexibleCell = false;
                        //sum of non fixed non percent columns.
                        for (int i = 0; i < numberOfColumns; i++) {
                            if (widths[i].isPercent) {
                                if (tableWidth * widths[i].width / 100 >= widths[i].min) {
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
                                    hasFlexibleCell = true;
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
                            if (fixedAddition > 0 && (extraWidth < fixedAddition || !hasFlexibleCell)) {
                                for (int i = 0; i < numberOfColumns; i++) {
                                    //only points could be fixed
                                    if (widths[i].isFixed) {
                                        widths[i].finalWidth += (widths[i].width - widths[i].min) * extraWidth / fixedAddition;
                                    }
                                }
                            }
                            else {
                                extraWidth -= fixedAddition;
                                if (extraWidth < flexibleAddition) {
                                    for (int i = 0; i < numberOfColumns; i++) {
                                        if (widths[i].isFixed) {
                                            widths[i].finalWidth = widths[i].width;
                                        }
                                        else {
                                            if (!widths[i].isPercent) {
                                                widths[i].finalWidth += (widths[i].width - widths[i].min) * extraWidth / flexibleAddition;
                                            }
                                        }
                                    }
                                }
                                else {
                                    float totalFixed = 0;
                                    float totalFlexible = 0;
                                    for (int i = 0; i < numberOfColumns; i++) {
                                        if (widths[i].isFixed) {
                                            widths[i].finalWidth = widths[i].width;
                                            totalFixed += widths[i].width;
                                        }
                                        else {
                                            if (!widths[i].isPercent) {
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
            CellRenderer[] firtsRow;
            if (tableRenderer.headerRenderer != null && tableRenderer.headerRenderer.rows.Count > 0) {
                firtsRow = tableRenderer.headerRenderer.rows[0];
            }
            else {
                if (tableRenderer.rows.Count > 0) {
                    firtsRow = tableRenderer.rows[0];
                }
                else {
                    //most likely it is large table
                    firtsRow = null;
                }
            }
            if (firtsRow != null) {
                for (int i = 0; i < numberOfColumns; i++) {
                    if (columnWidths[i] == -1) {
                        CellRenderer cell = firtsRow[i];
                        if (cell != null) {
                            UnitValue cellWidth = GetCellWidth(cell, true);
                            if (cellWidth != null) {
                                System.Diagnostics.Debug.Assert(cellWidth.GetValue() >= 0);
                                float width = cellWidth.IsPercentValue() ? tableWidth * cellWidth.GetValue() / 100 : cellWidth.GetValue();
                                int colspan = ((Cell)cell.GetModelElement()).GetColspan();
                                for (int j = 0; j < colspan; j++) {
                                    columnWidths[i + j] = width / colspan;
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
            }
            if (remainWidth > 0) {
                if (numberOfColumns == processedColumns) {
                    //Set remaining width to all columns.
                    for (int i = 0; i < numberOfColumns; i++) {
                        columnWidths[i] = tableWidth * columnWidths[i] / (tableWidth - remainWidth);
                    }
                }
                else {
                    // Set remaining width to the unprocessed columns.
                    for (int i = 0; i < numberOfColumns; i++) {
                        if (columnWidths[i] == -1) {
                            columnWidths[i] = remainWidth / (numberOfColumns - processedColumns);
                        }
                    }
                }
            }
            else {
                if (numberOfColumns != processedColumns) {
                    //            Logger logger = LoggerFactory.getLogger(TableWidths.class);
                    //            logger.warn(LogMessageConstant.SUM_OF_TABLE_COLUMNS_IS_GREATER_THAN_TABLE_WIDTH);
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
        private void CalculateTableWidth(float availableWidth, bool calculateTableMaxWidth) {
            fixedTableLayout = "fixed".Equals(tableRenderer.GetProperty<String>(Property.TABLE_LAYOUT, "auto").ToLowerInvariant
                ());
            UnitValue width = tableRenderer.GetProperty<UnitValue>(Property.WIDTH);
            if (fixedTableLayout && width != null && width.GetValue() >= 0) {
                fixedTableWidth = true;
                tableWidth = RetrieveTableWidth(width, availableWidth);
                tableMinWidth = width.IsPercentValue() ? 0 : tableWidth;
            }
            else {
                fixedTableLayout = false;
                //min width will initialize later
                tableMinWidth = -1;
                if (calculateTableMaxWidth) {
                    fixedTableWidth = false;
                    tableWidth = RetrieveTableWidth(availableWidth);
                }
                else {
                    if (width != null && width.GetValue() >= 0) {
                        fixedTableWidth = true;
                        tableWidth = RetrieveTableWidth(width, availableWidth);
                    }
                    else {
                        fixedTableWidth = false;
                        tableWidth = RetrieveTableWidth(availableWidth);
                    }
                }
            }
        }

        private float RetrieveTableWidth(UnitValue width, float availableWidth) {
            return RetrieveTableWidth(width.IsPercentValue() ? width.GetValue() * availableWidth / 100 : width.GetValue
                ());
        }

        private float RetrieveTableWidth(float width) {
            float result = width - rightBorderMaxWidth / 2 - leftBorderMaxWidth / 2;
            return result > 0 ? result : 0;
        }

        private Table GetTable() {
            return (Table)tableRenderer.GetModelElement();
        }

        //endregion
        //region Auto layout utils
        private void CalculateMinMaxWidths() {
            float[] minWidths = new float[numberOfColumns];
            float[] maxWidths = new float[numberOfColumns];
            foreach (TableWidths.CellInfo cell in cells) {
                // Why we need it? Header/Footer?
                cell.GetCell().SetParent(tableRenderer);
                MinMaxWidth minMax = cell.GetCell().GetMinMaxWidth(MinMaxWidthUtils.GetMax());
                float[] indents = GetCellBorderIndents(cell);
                minMax.SetAdditionalWidth(minMax.GetAdditionalWidth() + indents[1] / 2 + indents[3] / 2);
                if (cell.GetColspan() == 1) {
                    minWidths[cell.GetCol()] = Math.Max(minMax.GetMinWidth(), minWidths[cell.GetCol()]);
                    maxWidths[cell.GetCol()] = Math.Max(minMax.GetMaxWidth(), maxWidths[cell.GetCol()]);
                }
                else {
                    float remainMin = minMax.GetMinWidth();
                    float remainMax = minMax.GetMaxWidth();
                    for (int i = cell.GetCol(); i < cell.GetCol() + cell.GetColspan(); i++) {
                        remainMin -= minWidths[i];
                        remainMax -= maxWidths[i];
                    }
                    if (remainMin > 0) {
                        for (int i = cell.GetCol(); i < cell.GetCol() + cell.GetColspan(); i++) {
                            minWidths[i] += remainMin / cell.GetColspan();
                        }
                    }
                    if (remainMax > 0) {
                        for (int i = cell.GetCol(); i < cell.GetCol() + cell.GetColspan(); i++) {
                            maxWidths[i] += remainMax / cell.GetColspan();
                        }
                    }
                }
            }
            for (int i = 0; i < widths.Length; i++) {
                widths[i] = new TableWidths.ColumnWidthData(minWidths[i], maxWidths[i]);
            }
        }

        private float[] GetCellBorderIndents(TableWidths.CellInfo cell) {
            TableRenderer renderer;
            if (cell.region == TableWidths.CellInfo.HEADER) {
                renderer = tableRenderer.headerRenderer;
            }
            else {
                if (cell.region == TableWidths.CellInfo.FOOTER) {
                    renderer = tableRenderer.footerRenderer;
                }
                else {
                    renderer = tableRenderer;
                }
            }
            return renderer.bordersHandler.GetCellBorderIndents(cell.GetRow(), cell.GetCol(), cell.GetRowspan(), cell.
                GetColspan());
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
                        cells.Add(new TableWidths.CellInfo(cell, row, col, region));
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
            tableMinWidth = 0;
            float[] columnWidths = new float[widths.Length];
            for (int i = 0; i < widths.Length; i++) {
                System.Diagnostics.Debug.Assert(widths[i].finalWidth >= 0);
                columnWidths[i] = widths[i].finalWidth;
                actualWidth += widths[i].finalWidth;
                tableMinWidth += widths[i].min;
            }
            if (actualWidth > tableWidth + MinMaxWidthUtils.GetEps() * widths.Length) {
                ILogger logger = LoggerFactory.GetLogger(typeof(iText.Layout.Renderer.TableWidths));
                logger.Warn(iText.IO.LogMessageConstant.TABLE_WIDTH_IS_MORE_THAN_EXPECTED_DUE_TO_MIN_WIDTH);
            }
            return columnWidths;
        }

        //endregion
        //region Internal classes
        public override String ToString() {
            return "width=" + tableWidth + (fixedTableWidth ? "!!" : "");
        }

        private class ColumnWidthData {
            internal readonly float min;

            internal float max;

            internal float width = 0;

            internal float finalWidth = -1;

            internal bool isPercent = false;

            internal bool isFixed = false;

            internal ColumnWidthData(float min, float max) {
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
                System.Diagnostics.Debug.Assert(this.min <= width);
                this.width = Math.Max(this.width, width);
                return this;
            }

            internal virtual TableWidths.ColumnWidthData ResetPoints(float width) {
                System.Diagnostics.Debug.Assert(this.min <= width);
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
                isFixed = false;
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

            internal virtual bool IsFlexible() {
                return !this.isFixed && !this.isPercent;
            }

            public override String ToString() {
                return "w=" + width + (isPercent ? "%" : "pt") + (isFixed ? " !!" : "") + ", min=" + min + ", max=" + max 
                    + ", finalWidth=" + finalWidth;
            }
        }

        private static readonly UnitValue ZeroWidth = UnitValue.CreatePointValue(0);

        //TODO DEVSIX-1174, box-sizing property
        internal UnitValue GetCellWidth(CellRenderer cell, bool zeroIsValid) {
            UnitValue widthValue = cell.GetProperty<UnitValue>(Property.WIDTH);
            //zero has special meaning in fixed layout, we shall not add padding to zero value
            if (widthValue == null || widthValue.GetValue() < 0) {
                return null;
            }
            else {
                if (widthValue.GetValue() == 0) {
                    return zeroIsValid ? ZeroWidth : null;
                }
                else {
                    if (widthValue.IsPercentValue()) {
                        return widthValue;
                    }
                    else {
                        Border[] borders = cell.GetBorders();
                        if (borders[1] != null) {
                            widthValue.SetValue(widthValue.GetValue() + borders[1].GetWidth() / 2);
                        }
                        if (borders[3] != null) {
                            widthValue.SetValue(widthValue.GetValue() + borders[3].GetWidth() / 2);
                        }
                        float[] paddings = cell.GetPaddings();
                        widthValue.SetValue(widthValue.GetValue() + paddings[1] + paddings[3]);
                        return widthValue;
                    }
                }
            }
        }

        private class CellInfo : IComparable<TableWidths.CellInfo> {
            internal const byte HEADER = 1;

            internal const byte BODY = 2;

            internal const byte FOOTER = 3;

            private readonly CellRenderer cell;

            private readonly int row;

            private readonly int col;

            internal readonly byte region;

            internal CellInfo(CellRenderer cell, int row, int col, byte region) {
                this.cell = cell;
                this.region = region;
                //we cannot use getModelElement().getCol() or getRow(), because its may be changed during layout.
                this.row = row;
                this.col = col;
            }

            internal virtual CellRenderer GetCell() {
                return cell;
            }

            internal virtual int GetCol() {
                return col;
            }

            internal virtual int GetColspan() {
                //we cannot use getModelElement().getColspan(), because it may be changed during layout.
                return (int)cell.GetPropertyAsInteger(Property.COLSPAN);
            }

            internal virtual int GetRow() {
                return row;
            }

            internal virtual int GetRowspan() {
                //we cannot use getModelElement().getRowspan(), because it may be changed during layout.
                return (int)cell.GetPropertyAsInteger(Property.ROWSPAN);
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

            public override String ToString() {
                String str = String.Format("row={0}, col={1}, rowspan={2}, colspan={3}, ", GetRow(), GetCol(), GetRowspan(
                    ), GetColspan());
                if (region == HEADER) {
                    str += "header";
                }
                else {
                    if (region == BODY) {
                        str += "body";
                    }
                    else {
                        if (region == FOOTER) {
                            str += "footer";
                        }
                    }
                }
                return str;
            }
        }
        //endregion
    }
}
