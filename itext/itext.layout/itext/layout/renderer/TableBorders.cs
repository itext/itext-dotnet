using System;
using System.Collections.Generic;
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Canvas;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;

namespace iText.Layout.Renderer {
    public abstract class TableBorders {
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

        public TableBorders(IList<CellRenderer[]> rows, int numberOfColumns) {
            this.rows = rows;
            this.numberOfColumns = numberOfColumns;
        }

        public TableBorders(IList<CellRenderer[]> rows, int numberOfColumns, Border[] tableBoundingBorders)
            : this(rows, numberOfColumns) {
            SetTableBoundingBorders(tableBoundingBorders);
        }

        public TableBorders(IList<CellRenderer[]> rows, int numberOfColumns, int largeTableIndexOffset)
            : this(rows, numberOfColumns) {
            this.largeTableIndexOffset = largeTableIndexOffset;
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
        protected internal abstract iText.Layout.Renderer.TableBorders ApplyTopBorder(Rectangle occupiedBox, Rectangle
             layoutBox, bool isEmpty, bool force, bool reverse);

        protected internal abstract iText.Layout.Renderer.TableBorders ApplyTopBorder(Rectangle occupiedBox, Rectangle
             layoutBox, bool reverse);

        protected internal abstract iText.Layout.Renderer.TableBorders ApplyBottomBorder(Rectangle occupiedBox, Rectangle
             layoutBox, bool isEmpty, bool force, bool reverse);

        protected internal abstract iText.Layout.Renderer.TableBorders ApplyBottomBorder(Rectangle occupiedBox, Rectangle
             layoutBox, bool reverse);

        protected internal abstract iText.Layout.Renderer.TableBorders ApplyLeftAndRightBorder(Rectangle layoutBox
            , bool reverse);

        protected internal abstract iText.Layout.Renderer.TableBorders SkipFooter(Border[] borders);

        protected internal abstract iText.Layout.Renderer.TableBorders ConsiderFooter(iText.Layout.Renderer.TableBorders
             footerBordersHandler, bool hasContent);

        protected internal abstract iText.Layout.Renderer.TableBorders ConsiderHeader(iText.Layout.Renderer.TableBorders
             headerBordersHandler, bool changeThis);

        protected internal abstract iText.Layout.Renderer.TableBorders ConsiderHeaderOccupiedArea(Rectangle occupiedBox
            , Rectangle layoutBox);

        protected internal abstract iText.Layout.Renderer.TableBorders ApplyCellIndents(Rectangle box, float topIndent
            , float rightIndent, float bottomIndent, float leftIndent, bool reverse);

        // endregion
        // region getters
        public abstract IList<Border> GetVerticalBorder(int index);

        public abstract IList<Border> GetHorizontalBorder(int index);

        protected internal abstract float GetCellVerticalAddition(float[] indents);

        // endregion
        protected internal abstract iText.Layout.Renderer.TableBorders UpdateOnNewPage(bool isOriginalNonSplitRenderer
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
            return GetWidestBorder(GetVerticalBorder(col));
        }

        public virtual Border GetWidestVerticalBorder(int col, int start, int end) {
            return GetWidestBorder(GetVerticalBorder(col), start, end);
        }

        public virtual Border GetWidestHorizontalBorder(int row) {
            return GetWidestBorder(GetHorizontalBorder(row));
        }

        public virtual Border GetWidestHorizontalBorder(int row, int start, int end) {
            return GetWidestBorder(GetHorizontalBorder(row), start, end);
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
        //region static
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
    }
}
