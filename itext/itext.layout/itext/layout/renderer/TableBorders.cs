/*
This file is part of the iText (R) project.
Copyright (c) 1998-2018 iText Group NV
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
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Canvas;
using iText.Layout.Borders;

namespace iText.Layout.Renderer {
    internal abstract class TableBorders {
        protected internal IList<IList<Border>> horizontalBorders = new List<IList<Border>>();

        protected internal IList<IList<Border>> verticalBorders = new List<IList<Border>>();

        protected internal readonly int numberOfColumns;

        protected internal Border[] tableBoundingBorders = new Border[4];

        protected internal IList<CellRenderer[]> rows;

        protected internal int startRow;

        protected internal int finishRow;

        protected internal float leftBorderMaxWidth;

        protected internal float rightBorderMaxWidth;

        protected internal int largeTableIndexOffset = 0;

        public TableBorders(IList<CellRenderer[]> rows, int numberOfColumns, Border[] tableBoundingBorders) {
            // Zero-based, inclusive
            // Zero-based, inclusive. The last border will have index (finishRow+1) because the number of borders is greater
            // by one than the number of rows
            this.rows = rows;
            this.numberOfColumns = numberOfColumns;
            SetTableBoundingBorders(tableBoundingBorders);
        }

        public TableBorders(IList<CellRenderer[]> rows, int numberOfColumns, Border[] tableBoundingBorders, int largeTableIndexOffset
            )
            : this(rows, numberOfColumns, tableBoundingBorders) {
            this.largeTableIndexOffset = largeTableIndexOffset;
        }

        // region abstract
        // region draw
        protected internal abstract iText.Layout.Renderer.TableBorders DrawHorizontalBorder(int i, float startX, float
             y1, PdfCanvas canvas, float[] countedColumnWidth);

        protected internal abstract iText.Layout.Renderer.TableBorders DrawVerticalBorder(int i, float startY, float
             x1, PdfCanvas canvas, IList<float> heights);

        // endregion
        // region area occupation
        protected internal abstract iText.Layout.Renderer.TableBorders ApplyTopTableBorder(Rectangle occupiedBox, 
            Rectangle layoutBox, bool isEmpty, bool force, bool reverse);

        protected internal abstract iText.Layout.Renderer.TableBorders ApplyTopTableBorder(Rectangle occupiedBox, 
            Rectangle layoutBox, bool reverse);

        protected internal abstract iText.Layout.Renderer.TableBorders ApplyBottomTableBorder(Rectangle occupiedBox
            , Rectangle layoutBox, bool isEmpty, bool force, bool reverse);

        protected internal abstract iText.Layout.Renderer.TableBorders ApplyBottomTableBorder(Rectangle occupiedBox
            , Rectangle layoutBox, bool reverse);

        protected internal abstract iText.Layout.Renderer.TableBorders ApplyLeftAndRightTableBorder(Rectangle layoutBox
            , bool reverse);

        protected internal abstract iText.Layout.Renderer.TableBorders SkipFooter(Border[] borders);

        protected internal abstract iText.Layout.Renderer.TableBorders SkipHeader(Border[] borders);

        protected internal abstract iText.Layout.Renderer.TableBorders CollapseTableWithFooter(iText.Layout.Renderer.TableBorders
             footerBordersHandler, bool hasContent);

        protected internal abstract iText.Layout.Renderer.TableBorders CollapseTableWithHeader(iText.Layout.Renderer.TableBorders
             headerBordersHandler, bool changeThis);

        protected internal abstract iText.Layout.Renderer.TableBorders FixHeaderOccupiedArea(Rectangle occupiedBox
            , Rectangle layoutBox);

        protected internal abstract iText.Layout.Renderer.TableBorders ApplyCellIndents(Rectangle box, float topIndent
            , float rightIndent, float bottomIndent, float leftIndent, bool reverse);

        // endregion
        // region getters
        public abstract IList<Border> GetVerticalBorder(int index);

        public abstract IList<Border> GetHorizontalBorder(int index);

        protected internal abstract float GetCellVerticalAddition(float[] indents);

        // endregion
        protected internal abstract iText.Layout.Renderer.TableBorders UpdateBordersOnNewPage(bool isOriginalNonSplitRenderer
            , bool isFooterOrHeader, TableRenderer currentRenderer, TableRenderer headerRenderer, TableRenderer footerRenderer
            );

        // endregion
        // region init
        protected internal virtual iText.Layout.Renderer.TableBorders InitializeBorders() {
            IList<Border> tempBorders;
            // initialize vertical borders
            while (numberOfColumns + 1 > verticalBorders.Count) {
                tempBorders = new List<Border>();
                while ((int)Math.Max(rows.Count, 1) > tempBorders.Count) {
                    tempBorders.Add(null);
                }
                verticalBorders.Add(tempBorders);
            }
            // initialize horizontal borders
            while ((int)Math.Max(rows.Count, 1) + 1 > horizontalBorders.Count) {
                tempBorders = new List<Border>();
                while (numberOfColumns > tempBorders.Count) {
                    tempBorders.Add(null);
                }
                horizontalBorders.Add(tempBorders);
            }
            return this;
        }

        // endregion
        // region setters
        protected internal virtual iText.Layout.Renderer.TableBorders SetTableBoundingBorders(Border[] borders) {
            tableBoundingBorders = new Border[4];
            if (null != borders) {
                for (int i = 0; i < borders.Length; i++) {
                    tableBoundingBorders[i] = borders[i];
                }
            }
            return this;
        }

        protected internal virtual iText.Layout.Renderer.TableBorders SetRowRange(int startRow, int finishRow) {
            this.startRow = startRow;
            this.finishRow = finishRow;
            return this;
        }

        protected internal virtual iText.Layout.Renderer.TableBorders SetStartRow(int row) {
            this.startRow = row;
            return this;
        }

        protected internal virtual iText.Layout.Renderer.TableBorders SetFinishRow(int row) {
            this.finishRow = row;
            return this;
        }

        // endregion
        // region getters
        public virtual float GetLeftBorderMaxWidth() {
            return leftBorderMaxWidth;
        }

        public virtual float GetRightBorderMaxWidth() {
            return rightBorderMaxWidth;
        }

        public virtual float GetMaxTopWidth() {
            float width = 0;
            Border widestBorder = GetWidestHorizontalBorder(startRow);
            if (null != widestBorder && widestBorder.GetWidth() >= width) {
                width = widestBorder.GetWidth();
            }
            return width;
        }

        public virtual float GetMaxBottomWidth() {
            float width = 0;
            Border widestBorder = GetWidestHorizontalBorder(finishRow + 1);
            if (null != widestBorder && widestBorder.GetWidth() >= width) {
                width = widestBorder.GetWidth();
            }
            return width;
        }

        public virtual float GetMaxRightWidth() {
            float width = 0;
            Border widestBorder = GetWidestVerticalBorder(verticalBorders.Count - 1);
            if (null != widestBorder && widestBorder.GetWidth() >= width) {
                width = widestBorder.GetWidth();
            }
            return width;
        }

        public virtual float GetMaxLeftWidth() {
            float width = 0;
            Border widestBorder = GetWidestVerticalBorder(0);
            if (null != widestBorder && widestBorder.GetWidth() >= width) {
                width = widestBorder.GetWidth();
            }
            return width;
        }

        public virtual Border GetWidestVerticalBorder(int col) {
            return TableBorderUtil.GetWidestBorder(GetVerticalBorder(col));
        }

        public virtual Border GetWidestVerticalBorder(int col, int start, int end) {
            return TableBorderUtil.GetWidestBorder(GetVerticalBorder(col), start, end);
        }

        public virtual Border GetWidestHorizontalBorder(int row) {
            return TableBorderUtil.GetWidestBorder(GetHorizontalBorder(row));
        }

        public virtual Border GetWidestHorizontalBorder(int row, int start, int end) {
            return TableBorderUtil.GetWidestBorder(GetHorizontalBorder(row), start, end);
        }

        public virtual IList<Border> GetFirstHorizontalBorder() {
            return GetHorizontalBorder(startRow);
        }

        public virtual IList<Border> GetLastHorizontalBorder() {
            return GetHorizontalBorder(finishRow + 1);
        }

        public virtual IList<Border> GetFirstVerticalBorder() {
            return GetVerticalBorder(0);
        }

        public virtual IList<Border> GetLastVerticalBorder() {
            return GetVerticalBorder(verticalBorders.Count - 1);
        }

        public virtual int GetNumberOfColumns() {
            return numberOfColumns;
        }

        public virtual int GetStartRow() {
            return startRow;
        }

        public virtual int GetFinishRow() {
            return finishRow;
        }

        public virtual Border[] GetTableBoundingBorders() {
            return tableBoundingBorders;
        }

        public virtual float[] GetCellBorderIndents(int row, int col, int rowspan, int colspan) {
            float[] indents = new float[4];
            IList<Border> borderList;
            Border border;
            // process top border
            borderList = GetHorizontalBorder(startRow + row - rowspan + 1);
            for (int i = col; i < col + colspan; i++) {
                border = borderList[i];
                if (null != border && border.GetWidth() > indents[0]) {
                    indents[0] = border.GetWidth();
                }
            }
            // process right border
            borderList = GetVerticalBorder(col + colspan);
            for (int i = startRow + row - rowspan + 1; i < startRow + row + 1; i++) {
                border = borderList[i];
                if (null != border && border.GetWidth() > indents[1]) {
                    indents[1] = border.GetWidth();
                }
            }
            // process bottom border
            borderList = GetHorizontalBorder(startRow + row + 1);
            for (int i = col; i < col + colspan; i++) {
                border = borderList[i];
                if (null != border && border.GetWidth() > indents[2]) {
                    indents[2] = border.GetWidth();
                }
            }
            // process left border
            borderList = GetVerticalBorder(col);
            for (int i = startRow + row - rowspan + 1; i < startRow + row + 1; i++) {
                border = borderList[i];
                if (null != border && border.GetWidth() > indents[3]) {
                    indents[3] = border.GetWidth();
                }
            }
            return indents;
        }
        // endregion
    }
}
