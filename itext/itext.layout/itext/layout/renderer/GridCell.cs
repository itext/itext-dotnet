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
using iText.Kernel.Geom;
using iText.Layout.Properties;

namespace iText.Layout.Renderer {
//\cond DO_NOT_DOCUMENT
    /// <summary>This class represents a cell in a grid.</summary>
    internal class GridCell {
        private readonly IRenderer value;

        private int gridX;

        private int gridY;

        private readonly int spanColumn;

        private readonly int spanRow;

        private readonly Rectangle layoutArea = new Rectangle(0.0f, 0.0f, 0.0f, 0.0f);

        /// <summary>Cached track sizes for rows to use them during split.</summary>
        private float[] rowSizes;

//\cond DO_NOT_DOCUMENT
        /// <summary>Create a grid cell and init value renderer position on a grid based on its properties.</summary>
        /// <param name="value">item renderer</param>
        internal GridCell(IRenderer value) {
            this.value = value;
            int[] rowPlacement = InitAxisPlacement(value.GetProperty<int?>(Property.GRID_ROW_START), value.GetProperty
                <int?>(Property.GRID_ROW_END), value.GetProperty<int?>(Property.GRID_ROW_SPAN));
            gridY = rowPlacement[0];
            spanRow = rowPlacement[1];
            int[] columnPlacement = InitAxisPlacement(value.GetProperty<int?>(Property.GRID_COLUMN_START), value.GetProperty
                <int?>(Property.GRID_COLUMN_END), value.GetProperty<int?>(Property.GRID_COLUMN_SPAN));
            gridX = columnPlacement[0];
            spanColumn = columnPlacement[1];
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual int GetColumnStart() {
            return gridX;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual int GetColumnEnd() {
            return gridX + spanColumn;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual int GetRowStart() {
            return gridY;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual int GetRowEnd() {
            return gridY + spanRow;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual int GetStart(Grid.GridOrder order) {
            if (Grid.GridOrder.COLUMN == order) {
                return GetColumnStart();
            }
            else {
                return GetRowStart();
            }
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual int GetEnd(Grid.GridOrder order) {
            if (Grid.GridOrder.COLUMN == order) {
                return GetColumnEnd();
            }
            else {
                return GetRowEnd();
            }
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual int GetGridHeight() {
            return spanRow;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual int GetGridWidth() {
            return spanColumn;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual int GetGridSpan(Grid.GridOrder order) {
            if (Grid.GridOrder.COLUMN == order) {
                return GetGridWidth();
            }
            else {
                return GetGridHeight();
            }
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual IRenderer GetValue() {
            return value;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual Rectangle GetLayoutArea() {
            return layoutArea;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual void SetPos(int y, int x) {
            this.gridY = y;
            this.gridX = x;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual float[] GetRowSizes() {
            return this.rowSizes;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual void SetRowSizes(float[] rowSizes) {
            this.rowSizes = rowSizes;
        }
//\endcond

        /// <summary>
        /// Init axis placement values
        /// if start &gt; end values are swapped
        /// </summary>
        /// <param name="start">x/y pos of cell on a grid</param>
        /// <param name="end">x/y + width/height pos of cell on a grid</param>
        /// <param name="span">vertical or horizontal span of the cell on a grid</param>
        /// <returns>row/column start + vertical/horizontal span values as a pair, where first value is start, second is span
        ///     </returns>
        private int[] InitAxisPlacement(int? start, int? end, int? span) {
            int[] result = new int[] { 0, 1 };
            if (start != null && end != null) {
                int intStart = (int)start;
                int intEnd = (int)end;
                if (intStart < intEnd) {
                    result[0] = intStart;
                    result[1] = intEnd - intStart;
                }
                else {
                    if (intStart == intEnd) {
                        result[0] = intStart;
                    }
                    else {
                        result[0] = intEnd;
                        result[1] = intStart - intEnd;
                    }
                }
            }
            else {
                if (start != null) {
                    result[0] = (int)start;
                    if (span != null) {
                        result[1] = (int)span;
                    }
                }
                else {
                    // span default value 1 was set up on the result array initialization
                    if (end != null) {
                        int intEnd = (int)end;
                        if (span == null) {
                            result[0] = end <= 1 ? 1 : ((int)end) - 1;
                        }
                        else {
                            // span default value 1 was set up on the result array initialization
                            int intSpan = (int)span;
                            result[1] = intSpan;
                            result[0] = Math.Max(intEnd - intSpan, 1);
                        }
                    }
                    else {
                        if (span != null) {
                            result[1] = (int)span;
                        }
                    }
                }
            }
            result[0] -= 1;
            return result;
        }
    }
//\endcond
}
