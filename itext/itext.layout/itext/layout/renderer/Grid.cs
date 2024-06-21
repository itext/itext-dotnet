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
using iText.Layout.Properties.Grid;

namespace iText.Layout.Renderer {
//\cond DO_NOT_DOCUMENT
    /// <summary>This class represents a grid of elements.</summary>
    /// <remarks>
    /// This class represents a grid of elements.
    /// Complex elements (which span over few cells of a grid) are stored as duplicates.
    /// For example if element with width = 2, height = 3 added to a grid, grid will store it as 6 elements each having
    /// width = height = 1.
    /// </remarks>
    internal class Grid {
        private GridCell[][] rows = new GridCell[][] { new GridCell[1] };

        //Using array list instead of array for .NET portability
        private readonly IList<ICollection<GridCell>> uniqueCells = new List<ICollection<GridCell>>(2);

//\cond DO_NOT_DOCUMENT
        /// <summary>Creates a new grid instance.</summary>
        /// <param name="initialRowsCount">initial number of row for the grid</param>
        /// <param name="initialColumnsCount">initial number of columns for the grid</param>
        internal Grid(int initialRowsCount, int initialColumnsCount) {
            EnsureGridSize(initialRowsCount, initialColumnsCount);
            //Add GridOrder.ROW and GridOrder.COLUMN cache for unique cells, which is null initially
            uniqueCells.Add(null);
            uniqueCells.Add(null);
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Get internal matrix of cells.</summary>
        /// <returns>matrix of cells.</returns>
        internal GridCell[][] GetRows() {
            return rows;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Gets the current number of rows of grid.</summary>
        /// <returns>the number of rows</returns>
        internal int GetNumberOfRows() {
            return rows.Length;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Gets the current number of rows of grid.</summary>
        /// <returns>the number of columns</returns>
        internal int GetNumberOfColumns() {
            return rows.Length > 0 ? rows[0].Length : 0;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>
        /// Gets unique cells in the specified row or column depends on passed
        /// <see cref="GridOrder"/>.
        /// </summary>
        /// <param name="order">the order which will be used to extract cells</param>
        /// <param name="trackIndex">the track index from which cells will be extracted</param>
        /// <returns>collection of unique cells in a row or column</returns>
        internal virtual ICollection<GridCell> GetUniqueCellsInTrack(Grid.GridOrder order, int trackIndex) {
            IList<GridCell> result = new List<GridCell>(order == Grid.GridOrder.ROW ? GetNumberOfRows() : GetNumberOfColumns
                ());
            GridCell previous = null;
            if (Grid.GridOrder.COLUMN == order) {
                foreach (GridCell[] row in rows) {
                    GridCell current = row[trackIndex];
                    if (current != null && current != previous) {
                        previous = current;
                        result.Add(current);
                    }
                }
            }
            else {
                foreach (GridCell current in rows[trackIndex]) {
                    if (current != null && current != previous) {
                        previous = current;
                        result.Add(current);
                    }
                }
            }
            return result;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Get all unique cells in the grid.</summary>
        /// <remarks>
        /// Get all unique cells in the grid.
        /// Internally big cells (height * width &gt; 1) are stored in multiple quantities
        /// For example, cell with height = 2 and width = 2 will have 4 instances on a grid (width * height) to simplify
        /// internal grid processing. This method counts such cells as one and returns a list of unique cells.
        /// The result is cached since grid can't be changed after creation.
        /// </remarks>
        /// <param name="iterationOrder">
        /// if {GridOrder.ROW} the order of cells is from left to right, top to bottom
        /// if {GridOrder.COLUMN} the order of cells is from top to bottom, left to right
        /// </param>
        /// <returns>collection of unique grid cells.</returns>
        internal virtual ICollection<GridCell> GetUniqueGridCells(Grid.GridOrder iterationOrder) {
            ICollection<GridCell> result = new LinkedHashSet<GridCell>();
            if (uniqueCells[(int)(iterationOrder)] != null) {
                return uniqueCells[(int)(iterationOrder)];
            }
            if (Grid.GridOrder.COLUMN.Equals(iterationOrder)) {
                for (int j = 0; j < GetNumberOfColumns(); ++j) {
                    for (int i = 0; i < GetNumberOfRows(); ++i) {
                        if (rows[i][j] != null) {
                            result.Add(rows[i][j]);
                        }
                    }
                }
                uniqueCells[(int)(iterationOrder)] = result;
                return result;
            }
            // GridOrder.ROW
            foreach (GridCell[] cellsRow in rows) {
                foreach (GridCell cell in cellsRow) {
                    if (cell != null) {
                        result.Add(cell);
                    }
                }
            }
            uniqueCells[(int)(iterationOrder)] = result;
            return result;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Resize grid if needed, so it would have given number of rows/columns.</summary>
        /// <param name="height">new grid height</param>
        /// <param name="width">new grid width</param>
        internal virtual void EnsureGridSize(int height, int width) {
            if (height <= GetNumberOfRows() && width <= GetNumberOfColumns()) {
                return;
            }
            GridCell[][] resizedRows = height > GetNumberOfRows() ? new GridCell[height][] : rows;
            int gridWidth = Math.Max(width, GetNumberOfColumns());
            for (int i = 0; i < resizedRows.Length; ++i) {
                if (i < GetNumberOfRows()) {
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
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Deletes all null rows/columns depending on the given values.</summary>
        /// <remarks>
        /// Deletes all null rows/columns depending on the given values.
        /// If resulting grid size is less than provided minSize than some null lines will be preserved.
        /// </remarks>
        /// <param name="order">
        /// which null lines to remove -
        /// <see cref="GridOrder.ROW"/>
        /// to remove rows
        /// <see cref="GridOrder.COLUMN"/>
        /// to remove columns
        /// </param>
        /// <param name="minSize">minimal size of the resulting grid</param>
        /// <returns>the number of left lines in given order</returns>
        internal virtual int CollapseNullLines(Grid.GridOrder order, int minSize) {
            int nullLinesStart = DetermineNullLinesStart(order);
            if (nullLinesStart == -1) {
                return Grid.GridOrder.ROW.Equals(order) ? GetNumberOfRows() : GetNumberOfColumns();
            }
            else {
                nullLinesStart = Math.Max(minSize, nullLinesStart);
            }
            int rowsNumber = Grid.GridOrder.ROW.Equals(order) ? nullLinesStart : GetNumberOfRows();
            int colsNumber = Grid.GridOrder.COLUMN.Equals(order) ? nullLinesStart : GetNumberOfColumns();
            GridCell[][] shrankGrid = new GridCell[rowsNumber][];
            for (int i = 0; i < shrankGrid.Length; ++i) {
                shrankGrid[i] = new GridCell[colsNumber];
            }
            for (int i = 0; i < shrankGrid.Length; ++i) {
                Array.Copy(rows[i], 0, shrankGrid[i], 0, shrankGrid[0].Length);
            }
            rows = shrankGrid;
            return Grid.GridOrder.ROW.Equals(order) ? GetNumberOfRows() : GetNumberOfColumns();
        }
//\endcond

        /// <summary>Add cell in the grid, checking that it would fit and initializing it bottom left corner (x, y).</summary>
        /// <param name="cell">cell to and in the grid</param>
        private void AddCell(GridCell cell) {
            for (int i = cell.GetRowStart(); i < cell.GetRowEnd(); ++i) {
                for (int j = cell.GetColumnStart(); j < cell.GetColumnEnd(); ++j) {
                    rows[i][j] = cell;
                }
            }
        }

        private int DetermineNullLinesStart(Grid.GridOrder order) {
            if (Grid.GridOrder.ROW.Equals(order)) {
                for (int i = 0; i < GetNumberOfRows(); ++i) {
                    bool isNull = true;
                    for (int j = 0; j < GetNumberOfColumns(); ++j) {
                        if (GetRows()[i][j] != null) {
                            isNull = false;
                            break;
                        }
                    }
                    if (isNull) {
                        return i;
                    }
                }
                return -1;
            }
            else {
                if (Grid.GridOrder.COLUMN.Equals(order)) {
                    for (int j = 0; j < GetNumberOfColumns(); ++j) {
                        bool isNull = true;
                        for (int i = 0; i < GetNumberOfRows(); ++i) {
                            if (GetRows()[i][j] != null) {
                                isNull = false;
                                break;
                            }
                        }
                        if (isNull) {
                            return j;
                        }
                    }
                    return -1;
                }
            }
            return -1;
        }

        internal enum GridOrder {
            ROW,
            COLUMN
        }

//\cond DO_NOT_DOCUMENT
        /// <summary>This class is used to properly initialize starting values for grid.</summary>
        internal sealed class Builder {
            private int columnCount;

            private int rowCount;

            private GridFlow flow;

            private IList<GridCell> cells;

            private Builder() {
            }

//\cond DO_NOT_DOCUMENT
            /// <summary>Get grid builder for list of values.</summary>
            /// <param name="values">values to layout on grid</param>
            /// <returns>new grid builder instance</returns>
            internal static Grid.Builder ForItems(IList<IRenderer> values) {
                Grid.Builder builder = new Grid.Builder();
                builder.cells = values.Select((val) => new GridCell(val)).ToList();
                return builder;
            }
//\endcond

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
            public iText.Layout.Renderer.Grid Build() {
                iText.Layout.Renderer.Grid grid = new iText.Layout.Renderer.Grid(rowCount, columnCount);
                Grid.CellPlacementHelper cellPlacementHelper = new Grid.CellPlacementHelper(grid, flow);
                foreach (GridCell cell in cells) {
                    cellPlacementHelper.Fit(cell);
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

//\cond DO_NOT_DOCUMENT
            internal static IComparer<GridCell> GetOrderingFunctionForFlow(GridFlow flow) {
                if (GridFlow.COLUMN.Equals(flow) || GridFlow.COLUMN_DENSE.Equals(flow)) {
                    return new Grid.ColumnCellComparator();
                }
                return new Grid.RowCellComparator();
            }
//\endcond
        }
//\endcond

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

            private readonly iText.Layout.Renderer.Grid grid;

//\cond DO_NOT_DOCUMENT
            internal CellPlacementHelper(iText.Layout.Renderer.Grid grid, GridFlow flow) {
                this.view = new GridView(grid, flow);
                this.grid = grid;
            }
//\endcond

//\cond DO_NOT_DOCUMENT
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
//\endcond
        }
    }
//\endcond
}
