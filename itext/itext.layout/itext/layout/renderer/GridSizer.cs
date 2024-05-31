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
using System.Collections.Generic;
using iText.Kernel.Geom;
using iText.Layout.Properties;

namespace iText.Layout.Renderer {
    // 12.1. Grid Sizing Algorithm
    internal class GridSizer {
        private readonly Grid grid;

        private readonly IList<GridValue> templateColumns;

        private readonly IList<GridValue> templateRows;

        private readonly GridValue columnAutoWidth;

        private readonly GridValue rowAutoHeight;

        private readonly float columnGap;

        private readonly float rowGap;

        private readonly Rectangle actualBBox;

        internal GridSizer(Grid grid, IList<GridValue> templateColumns, IList<GridValue> templateRows, GridValue columnAutoWidth
            , GridValue rowAutoHeight, float columnGap, float rowGap, Rectangle actualBBox) {
            this.grid = grid;
            this.templateColumns = templateColumns;
            this.templateRows = templateRows;
            this.columnAutoWidth = columnAutoWidth;
            this.rowAutoHeight = rowAutoHeight;
            this.columnGap = columnGap;
            this.rowGap = rowGap;
            this.actualBBox = actualBBox;
        }

        public virtual void SizeGrid() {
            // 1. First, the track sizing algorithm is used to resolve the sizes of the grid columns.
            ResolveGridColumns();
            // 2. Next, the track sizing algorithm resolves the sizes of the grid rows.
            ResolveGridRows();
        }

        private void ResolveGridRows() {
            IList<GridValue> rowsValues = new List<GridValue>();
            for (int i = 0; i < grid.GetNumberOfRows(); i++) {
                if (templateRows != null && i < templateRows.Count) {
                    rowsValues.Add(templateRows[i]);
                }
                else {
                    if (rowAutoHeight != null) {
                        rowsValues.Add(rowAutoHeight);
                    }
                    else {
                        rowsValues.Add(GridValue.CreateAutoValue());
                    }
                }
            }
            // TODO DEVSIX-8384 during grid sizing algorithm take into account grid container constraints
            GridTrackSizer gridTrackSizer = new GridTrackSizer(grid, rowsValues, rowGap, -1, Grid.GridOrder.ROW);
            IList<float> rows = gridTrackSizer.SizeTracks();
            foreach (GridCell cell in grid.GetUniqueGridCells(Grid.GridOrder.ROW)) {
                float y = 0.0f;
                for (int currentRow = 0; currentRow < cell.GetRowStart(); ++currentRow) {
                    y += (float)rows[currentRow];
                    y += rowGap;
                }
                cell.GetLayoutArea().SetY(y);
                float cellHeight = 0.0f;
                for (int i = cell.GetRowStart(); i < cell.GetRowEnd(); ++i) {
                    cellHeight += (float)rows[i];
                }
                cellHeight += (cell.GetGridHeight() - 1) * rowGap;
                cell.GetLayoutArea().SetHeight(cellHeight);
            }
            // calculating explicit height to ensure that even empty rows which covered by template would be considered
            float minHeight = 0.0f;
            foreach (float? row in rows) {
                minHeight += (float)row;
            }
            grid.SetMinHeight(minHeight);
        }

        private void ResolveGridColumns() {
            IList<GridValue> colsValues = new List<GridValue>();
            for (int i = 0; i < grid.GetNumberOfColumns(); i++) {
                if (templateColumns != null && i < templateColumns.Count) {
                    colsValues.Add(templateColumns[i]);
                }
                else {
                    if (columnAutoWidth != null) {
                        colsValues.Add(columnAutoWidth);
                    }
                    else {
                        colsValues.Add(GridValue.CreateFlexValue(1f));
                    }
                }
            }
            GridTrackSizer gridTrackSizer = new GridTrackSizer(grid, colsValues, columnGap, actualBBox.GetWidth(), Grid.GridOrder
                .COLUMN);
            IList<float> columns = gridTrackSizer.SizeTracks();
            foreach (GridCell cell in grid.GetUniqueGridCells(Grid.GridOrder.COLUMN)) {
                float x = 0.0f;
                for (int currentColumn = 0; currentColumn < cell.GetColumnStart(); ++currentColumn) {
                    x += (float)columns[currentColumn];
                    x += columnGap;
                }
                cell.GetLayoutArea().SetX(x);
                float cellWidth = 0.0f;
                for (int i = cell.GetColumnStart(); i < cell.GetColumnEnd(); ++i) {
                    cellWidth += (float)columns[i];
                }
                cellWidth += (cell.GetGridWidth() - 1) * columnGap;
                cell.GetLayoutArea().SetWidth(cellWidth);
            }
        }
    }
}
