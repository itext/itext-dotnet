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
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Canvas;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;

namespace iText.Layout.Renderer {
//\cond DO_NOT_DOCUMENT
    internal class SeparatedTableBorders : TableBorders {
        public SeparatedTableBorders(IList<CellRenderer[]> rows, int numberOfColumns, Border[] tableBoundingBorders
            )
            : base(rows, numberOfColumns, tableBoundingBorders) {
        }

        public SeparatedTableBorders(IList<CellRenderer[]> rows, int numberOfColumns, Border[] tableBoundingBorders
            , int largeTableIndexOffset)
            : base(rows, numberOfColumns, tableBoundingBorders, largeTableIndexOffset) {
        }

        protected internal override TableBorders DrawHorizontalBorder(PdfCanvas canvas, TableBorderDescriptor borderDescriptor
            ) {
            return this;
        }

        protected internal override TableBorders DrawVerticalBorder(PdfCanvas canvas, TableBorderDescriptor borderDescriptor
            ) {
            return this;
        }

        protected internal override TableBorders ApplyTopTableBorder(Rectangle occupiedBox, Rectangle layoutBox, bool
             isEmpty, bool force, bool reverse) {
            return ApplyTopTableBorder(occupiedBox, layoutBox, reverse);
        }

        protected internal override TableBorders ApplyTopTableBorder(Rectangle occupiedBox, Rectangle layoutBox, bool
             reverse) {
            float topIndent = (reverse ? -1 : 1) * GetMaxTopWidth();
            layoutBox.DecreaseHeight(topIndent);
            occupiedBox.MoveDown(topIndent).IncreaseHeight(topIndent);
            return this;
        }

        protected internal override TableBorders ApplyBottomTableBorder(Rectangle occupiedBox, Rectangle layoutBox
            , bool isEmpty, bool force, bool reverse) {
            return ApplyBottomTableBorder(occupiedBox, layoutBox, reverse);
        }

        protected internal override TableBorders ApplyBottomTableBorder(Rectangle occupiedBox, Rectangle layoutBox
            , bool reverse) {
            float bottomTableBorderWidth = (reverse ? -1 : 1) * GetMaxBottomWidth();
            layoutBox.DecreaseHeight(bottomTableBorderWidth);
            occupiedBox.MoveDown(bottomTableBorderWidth).IncreaseHeight(bottomTableBorderWidth);
            return this;
        }

        protected internal override TableBorders ApplyLeftAndRightTableBorder(Rectangle layoutBox, bool reverse) {
            if (null != layoutBox) {
                layoutBox.ApplyMargins(0, rightBorderMaxWidth, 0, leftBorderMaxWidth, reverse);
            }
            return this;
        }

        protected internal override TableBorders SkipFooter(Border[] borders) {
            SetTableBoundingBorders(borders);
            return this;
        }

        protected internal override TableBorders SkipHeader(Border[] borders) {
            return this;
        }

        protected internal override TableBorders CollapseTableWithFooter(TableBorders footerBordersHandler, bool hasContent
            ) {
            return this;
        }

        protected internal override TableBorders CollapseTableWithHeader(TableBorders headerBordersHandler, bool updateBordersHandler
            ) {
            return this;
        }

        protected internal override TableBorders FixHeaderOccupiedArea(Rectangle occupiedBox, Rectangle layoutBox) {
            return this;
        }

        protected internal override TableBorders ApplyCellIndents(Rectangle box, float topIndent, float rightIndent
            , float bottomIndent, float leftIndent, bool reverse) {
            box.ApplyMargins(topIndent, rightIndent, bottomIndent, leftIndent, false);
            return this;
        }

        public override IList<Border> GetVerticalBorder(int index) {
            return verticalBorders[index];
        }

        public override IList<Border> GetHorizontalBorder(int index) {
            return horizontalBorders[index - largeTableIndexOffset];
        }

        protected internal override float GetCellVerticalAddition(float[] indents) {
            return 0;
        }

        protected internal override TableBorders UpdateBordersOnNewPage(bool isOriginalNonSplitRenderer, bool isFooterOrHeader
            , TableRenderer currentRenderer, TableRenderer headerRenderer, TableRenderer footerRenderer) {
            if (!isFooterOrHeader) {
                // collapse all cell borders
                if (isOriginalNonSplitRenderer) {
                    if (null != rows) {
                        ProcessAllBordersAndEmptyRows();
                        rightBorderMaxWidth = GetMaxRightWidth();
                        leftBorderMaxWidth = GetMaxLeftWidth();
                    }
                }
            }
            if (null != footerRenderer) {
                float rightFooterBorderWidth = footerRenderer.bordersHandler.GetMaxRightWidth();
                float leftFooterBorderWidth = footerRenderer.bordersHandler.GetMaxLeftWidth();
                leftBorderMaxWidth = Math.Max(leftBorderMaxWidth, leftFooterBorderWidth);
                rightBorderMaxWidth = Math.Max(rightBorderMaxWidth, rightFooterBorderWidth);
            }
            if (null != headerRenderer) {
                float rightHeaderBorderWidth = headerRenderer.bordersHandler.GetMaxRightWidth();
                float leftHeaderBorderWidth = headerRenderer.bordersHandler.GetMaxLeftWidth();
                leftBorderMaxWidth = Math.Max(leftBorderMaxWidth, leftHeaderBorderWidth);
                rightBorderMaxWidth = Math.Max(rightBorderMaxWidth, rightHeaderBorderWidth);
            }
            return this;
        }

        public override float[] GetCellBorderIndents(int row, int col, int rowspan, int colspan) {
            return new float[] { 0, 0, 0, 0 };
        }

        protected internal override void BuildBordersArrays(CellRenderer cell, int row, int col, int[] rowspansToDeduct
            ) {
            int colspan = (int)cell.GetPropertyAsInteger(Property.COLSPAN);
            int rowspan = (int)cell.GetPropertyAsInteger(Property.ROWSPAN);
            int colN = ((Cell)cell.GetModelElement()).GetCol();
            Border[] cellBorders = cell.GetBorders();
            // cell with big rowspan was splitted
            if (row + 1 - rowspan < 0) {
                rowspan = row + 1;
            }
            // consider top border
            for (int i = 0; i < colspan; i++) {
                CheckAndReplaceBorderInArray(horizontalBorders, 2 * (row + 1 - rowspan), colN + i, cellBorders[0], false);
            }
            // consider bottom border
            for (int i = 0; i < colspan; i++) {
                CheckAndReplaceBorderInArray(horizontalBorders, 2 * row + 1, colN + i, cellBorders[2], true);
            }
            // consider left border
            for (int j = row - rowspan + 1; j <= row; j++) {
                CheckAndReplaceBorderInArray(verticalBorders, 2 * colN, j, cellBorders[3], false);
            }
            // consider right border
            for (int i = row - rowspan + 1; i <= row; i++) {
                CheckAndReplaceBorderInArray(verticalBorders, 2 * (colN + colspan) - 1, i, cellBorders[1], true);
            }
        }

        protected internal virtual bool CheckAndReplaceBorderInArray(IList<IList<Border>> borderArray, int i, int 
            j, Border borderToAdd, bool hasPriority) {
            IList<Border> borders = borderArray[i];
            Border neighbour = borders[j];
            if (neighbour == null) {
                borders[j] = borderToAdd;
            }
            else {
                ILogger logger = ITextLogManager.GetLogger(typeof(TableRenderer));
                logger.LogWarning(iText.IO.Logs.IoLogMessageConstant.UNEXPECTED_BEHAVIOUR_DURING_TABLE_ROW_COLLAPSING);
            }
            return true;
        }

        protected internal override TableBorders InitializeBorders() {
            IList<Border> tempBorders;
            // initialize vertical borders
            while (2 * Math.Max(numberOfColumns, 1) > verticalBorders.Count) {
                tempBorders = new List<Border>();
                while ((int)2 * Math.Max(rows.Count, 1) > tempBorders.Count) {
                    tempBorders.Add(null);
                }
                verticalBorders.Add(tempBorders);
            }
            // initialize horizontal borders
            while ((int)2 * Math.Max(rows.Count, 1) > horizontalBorders.Count) {
                tempBorders = new List<Border>();
                while (numberOfColumns > tempBorders.Count) {
                    tempBorders.Add(null);
                }
                horizontalBorders.Add(tempBorders);
            }
            return this;
        }

        public override IList<Border> GetFirstHorizontalBorder() {
            return GetHorizontalBorder(2 * startRow);
        }

        public override IList<Border> GetLastHorizontalBorder() {
            return GetHorizontalBorder(2 * finishRow + 1);
        }

        public override float GetMaxTopWidth() {
            return null == tableBoundingBorders[0] ? 0 : tableBoundingBorders[0].GetWidth();
        }

        public override float GetMaxBottomWidth() {
            return null == tableBoundingBorders[2] ? 0 : tableBoundingBorders[2].GetWidth();
        }

        public override float GetMaxRightWidth() {
            return null == tableBoundingBorders[1] ? 0 : tableBoundingBorders[1].GetWidth();
        }

        public override float GetMaxLeftWidth() {
            return null == tableBoundingBorders[3] ? 0 : tableBoundingBorders[3].GetWidth();
        }
    }
//\endcond
}
