/*
This file is part of the iText (R) project.
Copyright (c) 1998-2025 Apryse Group NV
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
using iText.Kernel.Geom;

namespace iText.Layout.Renderer {
//\cond DO_NOT_DOCUMENT
    /// <summary>This class represents a cell in a grid.</summary>
    internal class GridCell {
        private readonly IRenderer value;

        private int columnStart;

        private int rowStart;

        private readonly int columnSpan;

        private readonly int rowSpan;

        private readonly Rectangle layoutArea = new Rectangle(0.0f, 0.0f, 0.0f, 0.0f);

        /// <summary>Cached track sizes for rows to use them during split.</summary>
        private float[] rowSizes;

//\cond DO_NOT_DOCUMENT
        /// <summary>Create a grid cell and init value renderer position on a grid based on its properties.</summary>
        /// <param name="value">item renderer</param>
        /// <param name="x">column number at which this cell starts (column numbers start from 0)</param>
        /// <param name="y">row number at which this cell starts (row numbers from 0)</param>
        /// <param name="width">number of columns spanned by this cell.</param>
        /// <param name="height">number of rows spanned by this cell.</param>
        internal GridCell(IRenderer value, int x, int y, int width, int height) {
            this.value = value;
            this.columnStart = x;
            this.rowStart = y;
            this.columnSpan = width;
            this.rowSpan = height;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual int GetColumnStart() {
            return columnStart;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual int GetColumnEnd() {
            return columnStart + columnSpan;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual int GetRowStart() {
            return rowStart;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual int GetRowEnd() {
            return rowStart + rowSpan;
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
            return rowSpan;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual int GetGridWidth() {
            return columnSpan;
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
            this.rowStart = y;
            this.columnStart = x;
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
    }
//\endcond
}
