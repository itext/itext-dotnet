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
using iText.Commons.Utils;
using iText.Layout.Exceptions;

namespace iText.Layout.Renderer {
    /// <summary>This class represents a grid of elements.</summary>
    internal class Grid {
        private readonly IList<IList<GridCell>> rows = new List<IList<GridCell>>();

        private readonly Grid.CellPacker cellPacker;

        private float minHeight = 0.0f;

        private float minWidth = 0.0f;

        internal const int ROW_ORDER = 1;

        internal const int COLUMN_ORDER = 2;

        /// <summary>Creates a new grid instance.</summary>
        /// <param name="initialRowsCount">initial number of row for the grid</param>
        /// <param name="initialColumnsCount">initial number of columns for the grid</param>
        /// <param name="densePacking">if true, dense packing will be used, otherwise sparse packing will be used</param>
        internal Grid(int initialRowsCount, int initialColumnsCount, bool densePacking) {
            EnsureGridSize(initialRowsCount, initialColumnsCount);
            cellPacker = new Grid.CellPacker(this, densePacking);
        }

        /// <summary>
        /// Get resulting layout height of the grid, if it's less than explicit (minimal) height of the grid
        /// return the explicit one.
        /// </summary>
        /// <returns>resulting layout height of a grid.</returns>
        internal virtual float GetHeight() {
            for (int i = rows.Count - 1; i >= 0; --i) {
                for (int j = 0; j < rows[0].Count; ++j) {
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
        internal virtual IList<IList<GridCell>> GetRows() {
            return rows;
        }

        /// <summary>Get any cell adjacent to the left of a given cell.</summary>
        /// <remarks>
        /// Get any cell adjacent to the left of a given cell.
        /// If there is no a direct neighbor to the left, and other adjacent cells are big cells and their column end
        /// is bigger than the column start of a given cell, method will still return such a neighbor, though it's not
        /// actually a neighbor to the left.
        /// </remarks>
        /// <param name="value">cell for which to find the neighbor</param>
        /// <returns>adjacent cell to the left if found one, null otherwise</returns>
        internal virtual GridCell GetClosestLeftNeighbor(GridCell value) {
            int x = value.GetColumnStart();
            GridCell bigNeighbor = null;
            for (int i = 1; i <= x; ++i) {
                for (int j = 0; j < rows.Count; ++j) {
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
        /// If there is no a direct neighbor to the top, and other adjacent cells are big cells and their row end
        /// is bigger than the row start of a given cell, method will still return such a neighbor, though it's not
        /// actually a neighbor to the top.
        /// </summary>
        /// <param name="value">cell for which to find the neighbor</param>
        /// <returns>adjacent cell to the top if found one, null otherwise</returns>
        internal virtual GridCell GetClosestTopNeighbor(GridCell value) {
            int y = value.GetRowStart();
            GridCell bigNeighbor = null;
            for (int i = 1; i <= y; ++i) {
                for (int j = 0; j < rows[0].Count; ++j) {
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
        /// if <c>Grid.ROW</c> the order of cells is from left to right, top to bottom
        /// if <c>Grid.COLUMN</c> the order of cells is from top to bottom, left to right
        /// </param>
        /// <returns>collection of unique grid cells.</returns>
        internal virtual ICollection<GridCell> GetUniqueGridCells(int iterationOrder) {
            ICollection<GridCell> result = new LinkedHashSet<GridCell>();
            if (iterationOrder == ROW_ORDER) {
                foreach (IList<GridCell> cellsRow in rows) {
                    foreach (GridCell cell in cellsRow) {
                        if (cell != null) {
                            result.Add(cell);
                        }
                    }
                }
            }
            if (iterationOrder == COLUMN_ORDER) {
                for (int j = 0; j < rows[0].Count; ++j) {
                    for (int i = 0; i < rows.Count; ++i) {
                        if (rows[i][j] != null) {
                            result.Add(rows[i][j]);
                        }
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
            for (int i = 0; i < rows.Count; ++i) {
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

        internal virtual float GetMaxRowTop(int y, int x) {
            IList<GridCell> row = rows[y];
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

        /// <summary>Add cell in the grid, checking that it would fit and initializing it upper left corner (x, y).</summary>
        /// <param name="cell">cell to and in the grid</param>
        internal virtual void AddCell(GridCell cell) {
            EnsureGridSize(cell.GetRowEnd(), cell.GetColumnEnd());
            SetStartingRowAndColumn(cell);
            for (int i = cell.GetRowStart(); i < cell.GetRowEnd(); ++i) {
                for (int j = cell.GetColumnStart(); j < cell.GetColumnEnd(); ++j) {
                    rows[i][j] = cell;
                }
            }
        }

        public virtual void SetMinHeight(float minHeight) {
            this.minHeight = minHeight;
        }

        public virtual void SetMinWidth(float minWidth) {
            this.minWidth = minWidth;
        }

        private void SetStartingRowAndColumn(GridCell cell) {
            if (cell.GetColumnStart() != -1 && cell.GetRowStart() != -1) {
                // cells which take more than 1 grid cells vertically and horizontally can't be moved
                for (int i = cell.GetRowStart(); i < cell.GetRowEnd(); ++i) {
                    for (int j = cell.GetColumnStart(); j < cell.GetColumnEnd(); ++j) {
                        if (rows[i][j] != null) {
                            throw new ArgumentException(LayoutExceptionMessageConstant.INVALID_CELL_INDEXES);
                        }
                    }
                }
            }
            else {
                if (cell.GetColumnStart() != -1) {
                    cellPacker.FitHorizontalCell(cell);
                }
                else {
                    if (cell.GetRowStart() != -1) {
                        cellPacker.FitVerticalCell(cell);
                    }
                    else {
                        cellPacker.FitSimpleCell(cell);
                    }
                }
            }
        }

        /// <summary>Resize grid to ensure that right bottom corner of a cell will fit into the grid.</summary>
        /// <param name="rowEnd">end row pos of a cell on a grid</param>
        /// <param name="columnEnd">end column pos of a cell on a grid</param>
        private void EnsureGridSize(int rowEnd, int columnEnd) {
            int maxRowSize = -1;
            for (int i = 0; i < Math.Max(rowEnd, rows.Count); i++) {
                IList<GridCell> row;
                if (i >= rows.Count) {
                    row = new List<GridCell>();
                    rows.Add(row);
                }
                else {
                    row = rows[i];
                }
                maxRowSize = Math.Max(maxRowSize, row.Count);
                for (int j = row.Count; j < Math.Max(columnEnd, maxRowSize); j++) {
                    row.Add(null);
                }
            }
        }

        //TODO DEVSIX-8323 Right now "row sparse" and "row dense" algorithms are implemented
        // implement "column sparse" and "column dense" the only thing which changes is winding order of a grid.
        // One will need to create a "view" on cellRows which is rotated 90 degrees to the right and also swap parameters
        // for the ensureGridSize in such a case
        private class CellPacker {
            //Determines whether to use "dense" or "sparse" packing algorithm
            private readonly bool densePacking;

            private int placementCursorX = 0;

            private int placementCursorY = 0;

            internal CellPacker(Grid _enclosing, bool densePacking) {
                this._enclosing = _enclosing;
                this.densePacking = densePacking;
            }

            //TODO DEVSIX-8323 double check with https://drafts.csswg.org/css-grid/#grid-item-placement-algorithm
            // they have 2 cases for such cells however I could not find a case for “sparse” and “dense” packing to
            // be different
            /// <summary>
            /// init vertical (<c>GridCell#getGridHeight() &gt; 1</c>)
            /// <c>GridCell</c> upper left corner to fit it in the grid
            /// </summary>
            /// <param name="cell">cell to fit</param>
            internal virtual void FitVerticalCell(GridCell cell) {
                for (int j = 0; j < this._enclosing.rows[0].Count; ++j) {
                    bool found = true;
                    for (int i = cell.GetRowStart(); i < cell.GetRowEnd(); ++i) {
                        if (this._enclosing.rows[i][j] != null) {
                            found = false;
                            break;
                        }
                    }
                    if (found) {
                        cell.SetStartingRowAndColumn(cell.GetRowStart(), j);
                        return;
                    }
                }
                cell.SetStartingRowAndColumn(cell.GetRowStart(), this._enclosing.rows[0].Count);
                this._enclosing.EnsureGridSize(-1, this._enclosing.rows[0].Count + 1);
            }

            /// <summary>
            /// init horizontal (<c>GridCell#getColumnEnd() - GridCell#getColumnStart() &gt; 1</c>)
            /// <c>GridCell</c> upper left corner to fit it in the grid
            /// </summary>
            /// <param name="cell">cell to fit</param>
            internal virtual void FitHorizontalCell(GridCell cell) {
                // All comments bellow are for the "sparse" packing, dense packing is much simpler and is achieved by
                // disabling placement cursor
                //Increment the cursor’s row position until a value is found where the grid item
                // does not overlap any occupied grid cells (creating new rows in the implicit grid as necessary).
                for (int i = this.GetPlacementCursorY(); i < this._enclosing.rows.Count; ++i, ++this.placementCursorY) {
                    //Set the column position of the cursor to the grid item’s column-start line.
                    // If this is less than the previous column position of the cursor, increment the row position by 1.
                    if (cell.GetColumnStart() < this.GetPlacementCursorX()) {
                        this.placementCursorX = cell.GetColumnStart();
                        continue;
                    }
                    this.placementCursorX = cell.GetColumnStart();
                    bool found = true;
                    for (int j = cell.GetColumnStart(); j < cell.GetColumnEnd(); ++j, ++this.placementCursorX) {
                        if (this._enclosing.rows[i][j] != null) {
                            found = false;
                            break;
                        }
                    }
                    if (found) {
                        //Set the item’s row-start line to the cursor’s row position
                        cell.SetStartingRowAndColumn(i, cell.GetColumnStart());
                        return;
                    }
                }
                cell.SetStartingRowAndColumn(this._enclosing.rows.Count, cell.GetColumnStart());
                this._enclosing.EnsureGridSize(this._enclosing.rows.Count + 1, -1);
            }

            /// <summary>
            /// init simple (cell height = width = 1)
            /// <c>GridCell</c> upper left corner to fit it in the grid
            /// </summary>
            /// <param name="cell">cell to fit</param>
            internal virtual void FitSimpleCell(GridCell cell) {
                //Algorithm the same as for horizontal cells except we're not checking for overlapping
                //and just placing the cell to the first space place
                for (int i = this.GetPlacementCursorY(); i < this._enclosing.rows.Count; ++i, ++this.placementCursorY) {
                    for (int j = this.GetPlacementCursorX(); j < this._enclosing.rows[i].Count; ++j, ++this.placementCursorX) {
                        if (this._enclosing.rows[i][j] == null) {
                            cell.SetStartingRowAndColumn(i, j);
                            return;
                        }
                    }
                    this.placementCursorX = 0;
                }
                cell.SetStartingRowAndColumn(this._enclosing.rows.Count, 0);
                this._enclosing.EnsureGridSize(this._enclosing.rows.Count + 1, -1);
            }

            //If it's dense packing, then it's enough to just disable placement cursors
            // and search for a spare place for the cell from the start of the grid.
            internal virtual int GetPlacementCursorX() {
                return this.densePacking ? 0 : this.placementCursorX;
            }

            internal virtual int GetPlacementCursorY() {
                return this.densePacking ? 0 : this.placementCursorY;
            }

            private readonly Grid _enclosing;
        }
    }
}
