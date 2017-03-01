using System;
using System.Collections.Generic;
using iText.Kernel.Pdf.Canvas;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;

namespace iText.Layout.Renderer {
    public abstract class TableBorders {
        protected internal IList<IList<Border>> horizontalBorders;

        protected internal IList<IList<Border>> verticalBorders;

        protected internal readonly int numberOfColumns;

        protected internal Border[] tableBoundingBorders;

        protected internal IList<CellRenderer[]> rows;

        protected internal Table.RowRange rowRange;

        public TableBorders(IList<CellRenderer[]> rows, int numberOfColumns) {
            // TODO Maybe Table ?
            this.rows = new List<CellRenderer[]>(rows);
            this.numberOfColumns = numberOfColumns;
            verticalBorders = new List<IList<Border>>();
            horizontalBorders = new List<IList<Border>>();
            tableBoundingBorders = new Border[4];
        }

        public TableBorders(IList<CellRenderer[]> rows, int numberOfColumns, Border[] tableBoundingBorders)
            : this(rows, numberOfColumns) {
            SetTableBoundingBorders(tableBoundingBorders);
        }

        protected internal virtual void InitializeBorders(IList<Border> lastFlushedRowBottomBorder, bool isFirstOnPage
            ) {
            IList<Border> tempBorders;
            // initialize vertical borders
            while (numberOfColumns + 1 > verticalBorders.Count) {
                tempBorders = new List<Border>();
                while (Math.Max(rows.Count, 1) > tempBorders.Count) {
                    tempBorders.Add(null);
                }
                verticalBorders.Add(tempBorders);
            }
            // initialize horizontal borders
            while (Math.Max(rows.Count, 1) + 1 > horizontalBorders.Count) {
                tempBorders = new List<Border>();
                while (numberOfColumns > tempBorders.Count) {
                    tempBorders.Add(null);
                }
                horizontalBorders.Add(tempBorders);
            }
        }

        // Notice that the first row on the page shouldn't collapse with the last on the previous one
        //        if (null != lastFlushedRowBottomBorder && 0 < lastFlushedRowBottomBorder.size() && !isFirstOnPage) { // TODO
        //            tempBorders = new ArrayList<Border>();
        //            for (Border border : lastFlushedRowBottomBorder) {
        //                tempBorders.add(border);
        //            }
        //            horizontalBorders.set(horizontalBorders.size()-1, tempBorders);
        //        }
        protected internal virtual iText.Layout.Renderer.TableBorders SetTableBoundingBorders(Border[] borders) {
            tableBoundingBorders = new Border[4];
            if (null != borders) {
                for (int i = 0; i < borders.Length; i++) {
                    tableBoundingBorders[i] = borders[i];
                }
            }
            return this;
        }

        protected internal virtual iText.Layout.Renderer.TableBorders SetRowRange(Table.RowRange rowRange) {
            this.rowRange = rowRange;
            return this;
        }

        protected internal virtual iText.Layout.Renderer.TableBorders SetStartRow(int row) {
            return SetRowRange(new Table.RowRange(row, rowRange.GetFinishRow()));
        }

        protected internal virtual iText.Layout.Renderer.TableBorders SetFinishRow(int row) {
            return SetRowRange(new Table.RowRange(rowRange.GetStartRow(), row));
        }

        // region getters
        public abstract IList<Border> GetVerticalBorder(int index);

        public abstract IList<Border> GetHorizontalBorder(int index);

        public virtual float GetMaxTopWidth() {
            float width = 0;
            Border widestBorder = GetWidestHorizontalBorder(rowRange.GetStartRow());
            if (null != widestBorder && widestBorder.GetWidth() >= width) {
                width = widestBorder.GetWidth();
            }
            return width;
        }

        public virtual float GetMaxBottomWidth() {
            float width = 0;
            Border widestBorder = GetWidestHorizontalBorder(rowRange.GetFinishRow() + 1);
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
            Border theWidestBorder = null;
            if (col >= 0 && col < verticalBorders.Count) {
                theWidestBorder = GetWidestBorder(GetVerticalBorder(col));
            }
            return theWidestBorder;
        }

        public virtual Border GetWidestVerticalBorder(int col, int start, int end) {
            Border theWidestBorder = null;
            if (col >= 0 && col < verticalBorders.Count) {
                theWidestBorder = GetWidestBorder(GetVerticalBorder(col), start, end);
            }
            return theWidestBorder;
        }

        public virtual Border GetWidestHorizontalBorder(int row) {
            Border theWidestBorder = null;
            //        if (row >= 0 && row < horizontalBorders.size()) {
            theWidestBorder = GetWidestBorder(GetHorizontalBorder(row));
            //        }
            return theWidestBorder;
        }

        public virtual Border GetWidestHorizontalBorder(int row, int start, int end) {
            Border theWidestBorder = null;
            //        if (row >= 0 && row < horizontalBorders.size()) {
            theWidestBorder = GetWidestBorder(GetHorizontalBorder(row), start, end);
            //        }
            return theWidestBorder;
        }

        public virtual IList<Border> GetFirstHorizontalBorder() {
            return GetHorizontalBorder(rowRange.GetStartRow());
        }

        public virtual IList<Border> GetLastHorizontalBorder() {
            return GetHorizontalBorder(rowRange.GetFinishRow() + 1);
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

        public virtual Border[] GetTableBoundingBorders() {
            return tableBoundingBorders;
        }

        public virtual int GetVerticalBordersSize() {
            return verticalBorders.Count;
        }

        public virtual int GetHorizontalBordersSize() {
            return verticalBorders.Count;
        }

        public virtual float[] GetCellBorderIndents(int row, int col, int rowspan, int colspan) {
            float[] indents = new float[4];
            IList<Border> borderList;
            Border border;
            // process top border
            borderList = GetHorizontalBorder(rowRange.GetStartRow() + row - rowspan + 1);
            for (int i = col; i < col + colspan; i++) {
                border = borderList[i];
                if (null != border && border.GetWidth() > indents[0]) {
                    indents[0] = border.GetWidth();
                }
            }
            // process right border
            borderList = GetVerticalBorder(col + colspan);
            for (int i = rowRange.GetStartRow() + row - rowspan + 1; i < rowRange.GetStartRow() + row + 1; i++) {
                border = borderList[i];
                if (null != border && border.GetWidth() > indents[1]) {
                    indents[1] = border.GetWidth();
                }
            }
            // process bottom border
            borderList = GetHorizontalBorder(rowRange.GetStartRow() + row + 1);
            for (int i = col; i < col + colspan; i++) {
                border = borderList[i];
                if (null != border && border.GetWidth() > indents[2]) {
                    indents[2] = border.GetWidth();
                }
            }
            // process left border
            borderList = GetVerticalBorder(col);
            for (int i = rowRange.GetStartRow() + row - rowspan + 1; i < rowRange.GetStartRow() + row + 1; i++) {
                border = borderList[i];
                if (null != border && border.GetWidth() > indents[3]) {
                    indents[3] = border.GetWidth();
                }
            }
            return indents;
        }

        // endregion
        //region static
        /// <summary>Returns the collapsed border.</summary>
        /// <remarks>
        /// Returns the collapsed border. We process collapse
        /// if the table border width is strictly greater than cell border width.
        /// </remarks>
        /// <param name="cellBorder">cell border</param>
        /// <param name="tableBorder">table border</param>
        /// <returns>the collapsed border</returns>
        public static Border GetCollapsedBorder(Border cellBorder, Border tableBorder) {
            if (null != tableBorder) {
                if (null == cellBorder || cellBorder.GetWidth() < tableBorder.GetWidth()) {
                    return tableBorder;
                }
            }
            if (null != cellBorder) {
                return cellBorder;
            }
            else {
                return Border.NO_BORDER;
            }
        }

        public static Border GetCellSideBorder(Cell cellModel, int borderType) {
            Border cellModelSideBorder = cellModel.GetProperty(borderType);
            if (null == cellModelSideBorder && !cellModel.HasProperty(borderType)) {
                cellModelSideBorder = cellModel.GetProperty(Property.BORDER);
                if (null == cellModelSideBorder && !cellModel.HasProperty(Property.BORDER)) {
                    cellModelSideBorder = cellModel.GetDefaultProperty(Property.BORDER);
                }
            }
            // TODO Maybe we need to foresee the possibility of default side border property
            return cellModelSideBorder;
        }

        public static Border GetWidestBorder(IList<Border> borderList) {
            Border theWidestBorder = null;
            if (0 != borderList.Count) {
                foreach (Border border in borderList) {
                    if (null != border && (null == theWidestBorder || border.GetWidth() > theWidestBorder.GetWidth())) {
                        theWidestBorder = border;
                    }
                }
            }
            return theWidestBorder;
        }

        public static Border GetWidestBorder(IList<Border> borderList, int start, int end) {
            Border theWidestBorder = null;
            if (0 != borderList.Count) {
                foreach (Border border in borderList.SubList(start, end)) {
                    if (null != border && (null == theWidestBorder || border.GetWidth() > theWidestBorder.GetWidth())) {
                        theWidestBorder = border;
                    }
                }
            }
            return theWidestBorder;
        }

        public static IList<Border> GetBorderList(Border border, int size) {
            IList<Border> borderList = new List<Border>();
            for (int i = 0; i < size; i++) {
                borderList.Add(border);
            }
            return borderList;
        }

        public static IList<Border> GetBorderList(IList<Border> originalList, Border borderToCollapse, int size) {
            IList<Border> borderList = new List<Border>();
            if (null != originalList) {
                borderList.AddAll(originalList);
            }
            while (borderList.Count < size) {
                borderList.Add(borderToCollapse);
            }
            int end = null == originalList ? size : Math.Min(originalList.Count, size);
            for (int i = 0; i < end; i++) {
                if (null == borderList[i] || (null != borderToCollapse && borderList[i].GetWidth() <= borderToCollapse.GetWidth
                    ())) {
                    borderList[i] = borderToCollapse;
                }
            }
            return borderList;
        }

        // endregion
        // region draw
        protected internal abstract iText.Layout.Renderer.TableBorders DrawHorizontalBorder(int i, float startX, float
             y1, PdfCanvas canvas, float[] countedColumnWidth);

        protected internal abstract iText.Layout.Renderer.TableBorders DrawVerticalBorder(int i, float startY, float
             x1, PdfCanvas canvas, IList<float> heights);
        // endregion
    }
}
