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
using iText.Layout.Properties.Grid;

namespace iText.Layout.Renderer {
//\cond DO_NOT_DOCUMENT
    /// <summary>This class represents a view on a grid, which returns cells one by one in a specified order.</summary>
    /// <remarks>
    /// This class represents a view on a grid, which returns cells one by one in a specified order.
    /// Also it allows to place a cell on a grid in this specific order and resizes the grid if needed.
    /// </remarks>
    internal class GridView {
        private readonly Grid grid;

        private readonly Grid.GridOrder iterationOrder;

        private readonly GridView.Cursor cursor;

        private bool restrictYGrow = false;

        private bool restrictXGrow = false;

        private bool hasNext = true;

        private int rightMargin;

        private int bottomMargin;

//\cond DO_NOT_DOCUMENT
        internal GridView(Grid grid, GridFlow iterationOrder) {
            this.iterationOrder = (GridFlow.COLUMN.Equals(iterationOrder) || GridFlow.COLUMN_DENSE.Equals(iterationOrder
                )) ? Grid.GridOrder.COLUMN : Grid.GridOrder.ROW;
            this.cursor = new GridView.Cursor(GridFlow.ROW_DENSE.Equals(iterationOrder) || GridFlow.COLUMN_DENSE.Equals
                (iterationOrder));
            this.grid = grid;
        }
//\endcond

        public virtual bool HasNext() {
            return cursor.y < grid.GetRows().Length - bottomMargin && cursor.x < grid.GetRows()[0].Length - rightMargin
                 && hasNext;
        }

        public virtual GridView.Pos Next() {
            //If cell has fixed both x and y, then no need to iterate over the grid.
            if (IsFixed()) {
                hasNext = false;
            }
            else {
                if (restrictXGrow) {
                    //If cell has fixed x position, then only view's 'y' can be moved.
                    ++cursor.y;
                    return new GridView.Pos(cursor);
                }
                else {
                    if (restrictYGrow) {
                        //If cell has fixed y position, then only view's 'x' can be moved.
                        ++cursor.x;
                        return new GridView.Pos(cursor);
                    }
                }
            }
            //If current flow is row, then grid should be iterated row by row, so iterate 'x' first, then 'y'
            //If current flow is column, then grid should be iterated column by column, so iterate 'y' first, then 'x'
            GridView.Pos boundaries = new GridView.Pos(grid.GetRows().Length - bottomMargin, grid.GetRows()[0].Length 
                - rightMargin);
            cursor.Increment(iterationOrder, boundaries);
            return new GridView.Pos(cursor);
        }

//\cond DO_NOT_DOCUMENT
        /// <summary>
        /// Resets grid view and sets it up for processing new element
        /// If sparse algorithm is used then x and y positions on a grid are not reset.
        /// </summary>
        /// <param name="y">left upper corner y position of an element on the grid</param>
        /// <param name="x">left upper corner x position of an element on the grid</param>
        /// <param name="rightMargin">specifies how many cells not to process at the right end of a grid</param>
        /// <param name="bottomMargin">specifies how many cells not to process at the bottom end of a grid</param>
        /// <returns>first element position</returns>
        internal virtual GridView.Pos Reset(int y, int x, int rightMargin, int bottomMargin) {
            this.cursor.SetY(y);
            this.cursor.SetX(x);
            if (x == -1 && y == -1) {
                if (rightMargin > grid.GetNumberOfColumns() - this.cursor.x + (Grid.GridOrder.COLUMN.Equals(iterationOrder
                    ) ? 1 : 0)) {
                    this.cursor.SetX(0);
                }
                if (bottomMargin > grid.GetNumberOfRows() - this.cursor.y + (Grid.GridOrder.ROW.Equals(iterationOrder) ? 1
                     : 0)) {
                    this.cursor.SetY(0);
                }
            }
            this.rightMargin = rightMargin - 1;
            this.bottomMargin = bottomMargin - 1;
            this.restrictXGrow = x != -1;
            this.restrictYGrow = y != -1;
            this.hasNext = true;
            return new GridView.Pos(cursor);
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Try to fit cell at the current position.</summary>
        /// <remarks>
        /// Try to fit cell at the current position.
        /// If cell is fit, then update current flow cursor axis by width/height of a laid out cell.
        /// </remarks>
        /// <param name="width">width of the cell</param>
        /// <param name="height">height of the cell</param>
        /// <returns>true if cell is fit, false otherwise</returns>
        internal virtual bool Fit(int width, int height) {
            GridCell[][] rows = grid.GetRows();
            for (int i = cursor.x; i < cursor.x + width; ++i) {
                for (int j = cursor.y; j < cursor.y + height; ++j) {
                    if (rows[j][i] != null) {
                        return false;
                    }
                }
            }
            IncreaseDefaultCursor(new GridView.Pos(height, width));
            ResetCursorIfIntersectingCellIsPlaced();
            return true;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Reset cursor to the start of a grid if placed a cell, which intersects current flow axis.</summary>
        /// <remarks>
        /// Reset cursor to the start of a grid if placed a cell, which intersects current flow axis.
        /// This is needed because such cells should be placed out of order and it's expected that
        /// they are go first while constructing the grid.
        /// </remarks>
        internal virtual void ResetCursorIfIntersectingCellIsPlaced() {
            if ((Grid.GridOrder.ROW.Equals(iterationOrder) && restrictYGrow) || (Grid.GridOrder.COLUMN.Equals(iterationOrder
                ) && restrictXGrow)) {
                this.cursor.Reset();
            }
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Increase cursor in current flow axis</summary>
        /// <param name="cellSizes">cell width and height values</param>
        internal virtual void IncreaseDefaultCursor(GridView.Pos cellSizes) {
            if (Grid.GridOrder.ROW.Equals(iterationOrder)) {
                cursor.x += cellSizes.x - 1;
            }
            else {
                if (Grid.GridOrder.COLUMN.Equals(iterationOrder)) {
                    cursor.y += cellSizes.y - 1;
                }
            }
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual void IncreaseDefaultAxis() {
            if (restrictYGrow) {
                grid.Resize(-1, grid.GetRows()[0].Length + 1);
            }
            else {
                if (restrictXGrow) {
                    grid.Resize(grid.GetRows().Length + 1, -1);
                }
                else {
                    if (Grid.GridOrder.ROW.Equals(iterationOrder)) {
                        grid.Resize(grid.GetRows().Length + 1, -1);
                    }
                    else {
                        if (Grid.GridOrder.COLUMN.Equals(iterationOrder)) {
                            grid.Resize(-1, grid.GetRows()[0].Length + 1);
                        }
                    }
                }
            }
            hasNext = true;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Determines if current grid view can be iterated.</summary>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if fixed
        /// <see langword="false"/>
        /// otherwise
        /// </returns>
        internal virtual bool IsFixed() {
            return restrictXGrow && restrictYGrow;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Represents position on a grid.</summary>
        internal class Pos {
            /// <summary>column index.</summary>
            protected internal int x;

            /// <summary>row index.</summary>
            protected internal int y;

            /// <summary>Creates a position from two integers.</summary>
            /// <param name="y">row index.</param>
            /// <param name="x">column index.</param>
            public Pos(int y, int x) {
                this.y = y;
                this.x = x;
            }

            /// <summary>Creates a position from other position instance.</summary>
            /// <param name="other">other position</param>
            public Pos(GridView.Pos other) {
                this.y = other.y;
                this.x = other.x;
            }

            public virtual int GetX() {
                return x;
            }

            public virtual int GetY() {
                return y;
            }
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Represents a placement cursor.</summary>
        internal class Cursor : GridView.Pos {
            //Determines whether to use "dense" or "sparse" packing algorithm
            private readonly bool densePacking;

            /// <summary>Create new placement cursor with either sparse or dense placement algorithm.</summary>
            /// <param name="densePacking">true to use "dense", false to use "sparse" placement algorithm</param>
            public Cursor(bool densePacking)
                : base(0, 0) {
                this.densePacking = densePacking;
            }

            public virtual void SetX(int x) {
                if (densePacking) {
                    this.x = Math.Max(x, 0);
                }
                else {
                    if (this.x > x && x != -1) {
                        //Special case for sparse algorithm
                        //if provided 'x' less than cursor 'x' then increase cursor's 'y'
                        this.x = x;
                        ++this.y;
                    }
                    else {
                        this.x = Math.Max(x, this.x);
                    }
                }
            }

            public virtual void SetY(int y) {
                if (densePacking) {
                    this.y = Math.Max(y, 0);
                }
                else {
                    if (this.y > y && y != -1) {
                        //Special case for sparse algorithm
                        //if provided 'y' less than cursor 'y' then increase cursor's 'x'
                        this.y = y;
                        ++this.x;
                    }
                    else {
                        this.y = Math.Max(y, this.y);
                    }
                }
            }

            /// <summary>
            /// Increment cursor in specified flow axis and if it exceeds the boundary in that axis
            /// make a carriage return.
            /// </summary>
            /// <param name="flow">flow which determines in which axis cursor will be increased</param>
            /// <param name="boundaries">grid view boundaries</param>
            public virtual void Increment(Grid.GridOrder flow, GridView.Pos boundaries) {
                if (Grid.GridOrder.ROW.Equals(flow)) {
                    ++x;
                    if (x == boundaries.x) {
                        x = 0;
                        ++y;
                    }
                }
                else {
                    if (Grid.GridOrder.COLUMN.Equals(flow)) {
                        ++y;
                        if (y == boundaries.y) {
                            y = 0;
                            ++x;
                        }
                    }
                }
            }

            public virtual void Reset() {
                this.x = 0;
                this.y = 0;
            }
        }
//\endcond
    }
//\endcond
}
