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
    internal class GridCell {
        private readonly IRenderer value;

        private readonly GridCell.IntRectangle gridArea;

        private Rectangle layoutArea = new Rectangle(0.0f, 0.0f, 0.0f, 0.0f);

        private bool isValueFitOnCellArea = true;

        /// <summary>Create a grid cell and init value renderer position on a grid based on its properties.</summary>
        /// <param name="value">item renderer</param>
        internal GridCell(IRenderer value) {
            this.value = value;
            int[] rowValues = InitRowColumnsValues(value.GetProperty<int?>(Property.GRID_ROW_START), value.GetProperty
                <int?>(Property.GRID_ROW_END));
            int height = rowValues[0] == 0 ? 1 : rowValues[1] - rowValues[0];
            int[] columnValues = InitRowColumnsValues(value.GetProperty<int?>(Property.GRID_COLUMN_START), value.GetProperty
                <int?>(Property.GRID_COLUMN_END));
            int width = columnValues[0] == 0 ? 1 : columnValues[1] - columnValues[0];
            gridArea = new GridCell.IntRectangle(columnValues[0] - 1, rowValues[0] - 1, width, height);
        }

        internal virtual int GetColumnStart() {
            return gridArea.GetLeft();
        }

        internal virtual int GetColumnEnd() {
            return gridArea.GetRight();
        }

        internal virtual int GetRowStart() {
            return gridArea.GetBottom();
        }

        internal virtual int GetRowEnd() {
            return gridArea.GetTop();
        }

        internal virtual int GetGridHeight() {
            return gridArea.GetHeight();
        }

        internal virtual int GetGridWidth() {
            return gridArea.GetWidth();
        }

        internal virtual IRenderer GetValue() {
            return value;
        }

        internal virtual bool IsValueFitOnCellArea() {
            return isValueFitOnCellArea;
        }

        internal virtual Rectangle GetLayoutArea() {
            return layoutArea;
        }

        internal virtual void SetLayoutArea(Rectangle layoutArea) {
            this.layoutArea = layoutArea;
        }

        internal virtual void SetValueFitOnCellArea(bool valueFitOnCellArea) {
            isValueFitOnCellArea = valueFitOnCellArea;
        }

        internal virtual void SetStartingRowAndColumn(int row, int column) {
            this.gridArea.SetY(row);
            this.gridArea.SetX(column);
        }

        /// <summary>
        /// init row/column start/end value
        /// if start &gt; end values are swapped
        /// if only start or end are specified - other value is initialized so cell would have height/width = 1
        /// </summary>
        /// <param name="start">x/y pos of cell on a grid</param>
        /// <param name="end">x/y + width/height pos of cell on a grid</param>
        /// <returns/>
        private int[] InitRowColumnsValues(int? start, int? end) {
            int[] result = new int[] { 0, 0 };
            if (start != null && end != null) {
                result[0] = (int)start;
                result[1] = (int)end;
                if (start > end) {
                    result[0] = (int)end;
                    result[1] = (int)start;
                }
            }
            else {
                if (start != null) {
                    result[0] = (int)start;
                    result[1] = (int)start + 1;
                }
                else {
                    if (end != null) {
                        result[0] = end <= 1 ? 1 : ((int)end) - 1;
                        result[1] = end <= 1 ? 2 : (int)end;
                    }
                }
            }
            return result;
        }

        private class IntRectangle {
            private int x;

            private int y;

            private int width;

            private int height;

            public IntRectangle(int x, int y, int width, int height) {
                this.x = x;
                this.y = y;
                this.width = width;
                this.height = height;
            }

            public virtual int GetLeft() {
                return x;
            }

            public virtual int GetRight() {
                return x + width;
            }

            public virtual int GetTop() {
                return y + height;
            }

            public virtual int GetBottom() {
                return y;
            }

            public virtual int GetWidth() {
                return width;
            }

            public virtual int GetHeight() {
                return height;
            }

            public virtual void SetX(int x) {
                this.x = x;
            }

            public virtual void SetY(int y) {
                this.y = y;
            }

            public override String ToString() {
                return "Rectangle: start(" + x + ',' + y + ") ," + width + 'x' + height;
            }
        }
    }
}
