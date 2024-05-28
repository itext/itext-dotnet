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
using System.Linq;
using iText.Commons.Utils;
using iText.Layout.Exceptions;
using iText.Layout.Properties;

namespace iText.Layout.Renderer {
    /// <summary>This class represents a grid of elements.</summary>
    /// <remarks>
    /// This class represents a grid of elements.
    /// Complex elements (which span over few cells of a grid) are stored as duplicates.
    /// For example if element with width = 2, height = 3 added to a grid, grid will store it as 6 elements each having
    /// width = height = 1.
    /// </remarks>
    internal class Grid {
        private GridCell[][] rows = new GridCell[][] { new GridCell[1] };

        private readonly Grid.CellPlacementHelper cellPlacementHelper;

        private float minHeight = 0.0f;

        private float minWidth = 0.0f;

        /// <summary>Creates a new grid instance.</summary>
        /// <param name="initialRowsCount">initial number of row for the grid</param>
        /// <param name="initialColumnsCount">initial number of columns for the grid</param>
        /// 
        internal Grid(int initialRowsCount, int initialColumnsCount, GridFlow flow) {
            cellPlacementHelper = new Grid.CellPlacementHelper(this, flow);
            EnsureGridSize(initialRowsCount, initialColumnsCount);
        }

        /// <summary>
        /// Get resulting layout height of the grid, if it's less than explicit (minimal) height of the grid
        /// return the explicit one.
        /// </summary>
        /// <returns>resulting layout height of a grid.</returns>
        internal virtual float GetHeight() {
            for (int i = rows.Length - 1; i >= 0; --i) {
                for (int j = 0; j < rows[0].Length; ++j) {
                    if (rows[i][j] != null) {
                        return Math.Max(rows[i][j].GetLayoutArea().GetTop(), minHeight);
                    }
                }
            }
            return minHeight;
        }

        /// <summary>Get min width of the grid, which is size of the grid covered by absolute template values.</summary>
        /// <returns>min width of a grid.</returns>
        internal virtual float GetMinWidth() {
            return minWidth;
        }

        /// <summary>Get internal matrix of cells.</summary>
        /// <returns>matrix of cells.</returns>
        internal virtual GridCell[][] GetRows() {
            return rows;
        }

        /// <summary>Get any cell adjacent to the left of a given cell.</summary>
        /// <remarks>
        /// Get any cell adjacent to the left of a given cell.
        /// If there is no a direct neighbor to the left, and other adjacent cells are wide cells and their column end
        /// is bigger than the column start of a given cell, method will still return such a neighbor, though it's not
        /// actually a neighbor to the left. But if there will be a neighbor before column start of such a cell method will
        /// return such a neighbor.
        /// </remarks>
        /// <param name="value">cell for which to find a neighbor</param>
        /// <returns>adjacent cell to the left if found one, null otherwise</returns>
        internal virtual GridCell GetClosestLeftNeighbor(GridCell value) {
            int x = value.GetColumnStart();
            GridCell bigNeighbor = null;
            for (int i = 1; i <= x; ++i) {
                for (int j = 0; j < rows.Length; ++j) {
                    if (rows[j][x - i] != null) {
                        if (rows[j][x - i].GetColumnEnd() > x) {
                            bigNeighbor = rows[j][x - i];
                            continue;
                        }
                        return rows[j][x - i];
                    }
                }
                if (bigNeighbor != null && bigNeighbor.GetColumnStart() == x - i) {
                    return bigNeighbor;
                }
            }
            return null;
        }

        /// <summary>
        /// Get any cell adjacent to the top of a given cell
        /// If there is no a direct neighbor to the top, and other adjacent cells are tall cells and their row end
        /// is bigger than the row start of a given cell, method will still return such a neighbor, though it's not
        /// actually a neighbor to the top.
        /// </summary>
        /// <remarks>
        /// Get any cell adjacent to the top of a given cell
        /// If there is no a direct neighbor to the top, and other adjacent cells are tall cells and their row end
        /// is bigger than the row start of a given cell, method will still return such a neighbor, though it's not
        /// actually a neighbor to the top. But if there will be a neighbor before row start of such a cell method will
        /// return such a neighbor.
        /// </remarks>
        /// <param name="value">cell for which to find a neighbor</param>
        /// <returns>adjacent cell to the top if found one, null otherwise</returns>
        internal virtual GridCell GetClosestTopNeighbor(GridCell value) {
            int y = value.GetRowStart();
            GridCell bigNeighbor = null;
            for (int i = 1; i <= y; ++i) {
                for (int j = 0; j < rows[0].Length; ++j) {
                    if (rows[y - i][j] != null) {
                        if (rows[y - i][j].GetRowEnd() > y) {
                            bigNeighbor = rows[y - i][j];
                            continue;
                        }
                        return rows[y - i][j];
                    }
                }
                if (bigNeighbor != null && bigNeighbor.GetRowStart() == y - i) {
                    return bigNeighbor;
                }
            }
            return null;
        }

        /// <summary>Get all unique cells in the grid.</summary>
        /// <remarks>
        /// Get all unique cells in the grid.
        /// Internally big cells (height * width &gt; 1) are stored in multiple quantities
        /// For example, cell with height = 2 and width = 2 will have 4 instances on a grid (width * height) to simplify
        /// internal grid processing. This method counts such cells as one and returns a list of unique cells.
        /// </remarks>
        /// <param name="iterationOrder">
        /// if {GridOrder.ROW} the order of cells is from left to right, top to bottom
        /// if {GridOrder.COLUMN} the order of cells is from top to bottom, left to right
        /// </param>
        /// <returns>collection of unique grid cells.</returns>
        internal virtual ICollection<GridCell> GetUniqueGridCells(Grid.GridOrder iterationOrder) {
            ICollection<GridCell> result = new LinkedHashSet<GridCell>();
            if (Grid.GridOrder.COLUMN.Equals(iterationOrder)) {
                for (int j = 0; j < rows[0].Length; ++j) {
                    for (int i = 0; i < rows.Length; ++i) {
                        if (rows[i][j] != null) {
                            result.Add(rows[i][j]);
                        }
                    }
                }
                return result;
            }
            foreach (GridCell[] cellsRow in rows) {
                foreach (GridCell cell in cellsRow) {
                    if (cell != null) {
                        result.Add(cell);
                    }
                }
            }
            return result;
        }

        /// <summary>align all cells in the specified row.</summary>
        /// <param name="row">row to iterate</param>
        /// <param name="value">new pos on a grid at which row should end</param>
        internal virtual void AlignRow(int row, float value) {
            GridCell previousCell = null;
            foreach (GridCell cell in rows[row]) {
                if (cell == null) {
                    continue;
                }
                // previousCell is used to avoid multiple area updating for items which spread through few cells
                if (previousCell != cell && cell.GetLayoutArea().GetTop() < value) {
                    cell.GetLayoutArea().SetHeight(value - cell.GetLayoutArea().GetY());
                }
                previousCell = cell;
            }
        }

        /// <summary>align all cells in the specified column.</summary>
        /// <param name="column">column to iterate</param>
        /// <param name="value">new pos on a grid at which column should end</param>
        internal virtual void AlignColumn(int column, float value) {
            GridCell previousCell = null;
            for (int i = 0; i < rows.Length; ++i) {
                GridCell cell = rows[i][column];
                if (cell == null) {
                    continue;
                }
                // previousCell is used to avoid multiple area updating for items which spread through few cells
                if (previousCell != cell && cell.GetLayoutArea().GetRight() < value) {
                    cell.GetLayoutArea().SetWidth(value - cell.GetLayoutArea().GetX());
                }
                previousCell = cell;
            }
        }

        /// <summary>Get max top (layout area y + height of a cell) in a row with index = y, for all elements in a row before given x.
        ///     </summary>
        /// <param name="y">index of a row to find max top value</param>
        /// <param name="x">index of element in a row before which to search for max top value</param>
        /// <returns>max top value, all cells which do not end in the given row are not counted.</returns>
        internal virtual float GetMaxRowTop(int y, int x) {
            GridCell[] row = rows[y];
            float maxTop = 0.0f;
            for (int i = 0; i < x; ++i) {
                if (row[i] == null || row[i].GetLayoutArea() == null) {
                    continue;
                }
                //process cells which end at the same row
                if (row[i].GetLayoutArea().GetTop() > maxTop && row[i].GetRowEnd() == y + 1) {
                    maxTop = row[i].GetLayoutArea().GetTop();
                }
            }
            return maxTop;
        }

        /// <summary>
        /// Get max right (layout area x + width of a cell) in a column with index = x,
        /// for all elements in a column before given y.
        /// </summary>
        /// <param name="y">index of element in a column before which to search for max right value</param>
        /// <param name="x">index of a column to find max right value</param>
        /// <returns>max right value, all cells which do not end in the given column are not counted.</returns>
        internal virtual float GetMaxColumnRight(int y, int x) {
            float maxRight = 0.0f;
            for (int i = 0; i < y; ++i) {
                GridCell cell = rows[i][x];
                if (cell == null || cell.GetLayoutArea() == null) {
                    continue;
                }
                //process cells which ends in the same column
                if (cell.GetLayoutArea().GetRight() > maxRight && cell.GetColumnEnd() == x + 1) {
                    maxRight = cell.GetLayoutArea().GetRight();
                }
            }
            return maxRight;
        }

        /// <summary>Add cell in the grid, checking that it would fit and initializing it bottom left corner (x, y).</summary>
        /// <param name="cell">cell to and in the grid</param>
        internal virtual void AddCell(GridCell cell) {
            cellPlacementHelper.Fit(cell);
            for (int i = cell.GetRowStart(); i < cell.GetRowEnd(); ++i) {
                for (int j = cell.GetColumnStart(); j < cell.GetColumnEnd(); ++j) {
                    rows[i][j] = cell;
                }
            }
        }

        internal virtual void SetMinHeight(float minHeight) {
            this.minHeight = minHeight;
        }

        internal virtual void SetMinWidth(float minWidth) {
            this.minWidth = minWidth;
        }

        /// <summary>Resize grid if needed, so it would have given number of rows/columns.</summary>
        /// <param name="height">new grid height</param>
        /// <param name="width">new grid width</param>
        internal virtual void EnsureGridSize(int height, int width) {
            if (height <= rows.Length && width <= rows[0].Length) {
                return;
            }
            GridCell[][] resizedRows = height > rows.Length ? new GridCell[height][] : rows;
            int gridWidth = Math.Max(width, rows[0].Length);
            for (int i = 0; i < resizedRows.Length; ++i) {
                if (i < rows.Length) {
                    if (width <= rows[i].Length) {
                        resizedRows[i] = rows[i];
                    }
                    else {
                        GridCell[] row = new GridCell[width];
                        Array.Copy(rows[i], 0, row, 0, rows[i].Length);
                        resizedRows[i] = row;
                    }
                }
                else {
                    GridCell[] row = new GridCell[gridWidth];
                    resizedRows[i] = row;
                }
            }
            rows = resizedRows;
        }

        internal enum GridOrder {
            ROW,
            COLUMN
        }

        /// <summary>This class is used to properly initialize starting values for grid.</summary>
        internal sealed class Builder {
            private int columnCount;

            private int rowCount;

            private GridFlow flow;

            private IList<GridCell> cells;

            private Builder() {
            }

            /// <summary>Get grid builder for list of values.</summary>
            /// <param name="values">values to layout on grid</param>
            /// <returns>new grid builder instance</returns>
            internal static Grid.Builder ForItems(IList<IRenderer> values) {
                Grid.Builder builder = new Grid.Builder();
                builder.cells = values.Select((val) => new GridCell(val)).ToList();
                return builder;
            }

            /// <summary>
            /// Set number of columns for a grid, the result will be either a provided one or if some elements
            /// have a property defining more columns on a grid than provided value it will be set instead.
            /// </summary>
            /// <param name="minColumnCount">min column count of a grid</param>
            /// <returns>current builder instance</returns>
            public Grid.Builder Columns(int minColumnCount) {
                columnCount = Math.Max(minColumnCount, CalculateInitialColumnsCount(cells));
                return this;
            }

            /// <summary>
            /// Set number of rows for a grid, the result will be either a provided one or if some elements
            /// have a property defining more rows on a grid than provided value it will be set instead.
            /// </summary>
            /// <param name="minRowCount">min height of a grid</param>
            /// <returns>current builder instance</returns>
            public Grid.Builder Rows(int minRowCount) {
                rowCount = Math.Max(minRowCount, CalculateInitialRowsCount(cells));
                return this;
            }

            /// <summary>Set iteration flow for a grid.</summary>
            /// <param name="flow">iteration flow</param>
            /// <returns>current builder instance</returns>
            public Grid.Builder Flow(GridFlow flow) {
                this.flow = flow;
                JavaCollectionsUtil.Sort(cells, GetOrderingFunctionForFlow(flow));
                return this;
            }

            /// <summary>Build a grid with provided properties.</summary>
            /// <returns>
            /// new
            /// <c>Grid</c>
            /// instance.
            /// </returns>
            public Grid Build() {
                Grid grid = new Grid(rowCount, columnCount, flow);
                foreach (GridCell cell in cells) {
                    grid.AddCell(cell);
                }
                return grid;
            }

            private static int CalculateInitialColumnsCount(ICollection<GridCell> cells) {
                int initialColumnsCount = 1;
                foreach (GridCell cell in cells) {
                    if (cell != null) {
                        initialColumnsCount = Math.Max(cell.GetGridWidth(), Math.Max(initialColumnsCount, cell.GetColumnEnd()));
                    }
                }
                return initialColumnsCount;
            }

            private static int CalculateInitialRowsCount(ICollection<GridCell> cells) {
                int initialRowsCount = 1;
                foreach (GridCell cell in cells) {
                    if (cell != null) {
                        initialRowsCount = Math.Max(cell.GetGridHeight(), Math.Max(initialRowsCount, cell.GetRowEnd()));
                    }
                }
                return initialRowsCount;
            }

            internal static IComparer<GridCell> GetOrderingFunctionForFlow(GridFlow flow) {
                if (GridFlow.COLUMN.Equals(flow) || GridFlow.COLUMN_DENSE.Equals(flow)) {
                    return new Grid.ColumnCellComparator();
                }
                return new Grid.RowCellComparator();
            }
        }

        /// <summary>
        /// This comparator sorts cells so ones with both fixed row and column positions would go first,
        /// then cells with fixed row and then all other cells.
        /// </summary>
        private sealed class RowCellComparator : IComparer<GridCell> {
            public int Compare(GridCell lhs, GridCell rhs) {
                int lhsModifiers = 0;
                if (lhs.GetColumnStart() != -1 && lhs.GetRowStart() != -1) {
                    lhsModifiers = 2;
                }
                else {
                    if (lhs.GetRowStart() != -1) {
                        lhsModifiers = 1;
                    }
                }
                int rhsModifiers = 0;
                if (rhs.GetColumnStart() != -1 && rhs.GetRowStart() != -1) {
                    rhsModifiers = 2;
                }
                else {
                    if (rhs.GetRowStart() != -1) {
                        rhsModifiers = 1;
                    }
                }
                //passing parameters in reversed order so ones with properties would come first
                return JavaUtil.IntegerCompare(rhsModifiers, lhsModifiers);
            }
        }

        /// <summary>
        /// This comparator sorts cells so ones with both fixed row and column positions would go first,
        /// then cells with fixed column and then all other cells.
        /// </summary>
        private sealed class ColumnCellComparator : IComparer<GridCell> {
            public int Compare(GridCell lhs, GridCell rhs) {
                int lhsModifiers = 0;
                if (lhs.GetColumnStart() != -1 && lhs.GetRowStart() != -1) {
                    lhsModifiers = 2;
                }
                else {
                    if (lhs.GetColumnStart() != -1) {
                        lhsModifiers = 1;
                    }
                }
                int rhsModifiers = 0;
                if (rhs.GetColumnStart() != -1 && rhs.GetRowStart() != -1) {
                    rhsModifiers = 2;
                }
                else {
                    if (rhs.GetColumnStart() != -1) {
                        rhsModifiers = 1;
                    }
                }
                //passing parameters in reversed order so ones with properties would come first
                return JavaUtil.IntegerCompare(rhsModifiers, lhsModifiers);
            }
        }

        /// <summary>This class is used to place cells on grid.</summary>
        private class CellPlacementHelper {
            private readonly GridView view;

            private readonly Grid grid;

            internal CellPlacementHelper(Grid grid, GridFlow flow) {
                this.view = new GridView(grid, flow);
                this.grid = grid;
            }

            /// <summary>Place cell on grid and resize grid if needed.</summary>
            /// <param name="cell">cell to place on a grid.</param>
            internal virtual void Fit(GridCell cell) {
                //resize the grid if needed to fit a cell into it
                grid.EnsureGridSize(cell.GetRowEnd(), cell.GetColumnEnd());
                bool result;
                //reset grid view to process new cell
                GridView.Pos pos = view.Reset(cell.GetRowStart(), cell.GetColumnStart(), cell.GetGridWidth(), cell.GetGridHeight
                    ());
                //Can use while(true) here, but since it's not expected to do more placement iteration as described with
                //max statement, to be on a safe side and prevent algorithm from hanging in unexpected situations doing
                //a finite number of iterations here.
                for (int i = 0; i < Math.Max(cell.GetGridHeight(), cell.GetGridWidth()) + 1; ++i) {
                    while (view.HasNext()) {
                        //Try to place the cell
                        result = view.Fit(cell.GetGridWidth(), cell.GetGridHeight());
                        //If fit, init cell's left corner position
                        if (result) {
                            cell.SetPos(pos.GetY(), pos.GetX());
                            return;
                        }
                        //Move grid view cursor
                        pos = view.Next();
                    }
                    //If cell restricts both x and y position grow and can't be fitted on a grid, throw an excpetion
                    if (view.IsFixed()) {
                        throw new ArgumentException(LayoutExceptionMessageConstant.INVALID_CELL_INDEXES);
                    }
                    //If cell was not fitted while iterating grid, then there is not enough space to fit it, and grid
                    //has to be resized
                    view.IncreaseDefaultAxis();
                }
            }
        }
    }
}
