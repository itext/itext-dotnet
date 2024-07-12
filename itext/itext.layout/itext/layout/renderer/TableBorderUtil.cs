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
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;

namespace iText.Layout.Renderer {
//\cond DO_NOT_DOCUMENT
    internal sealed class TableBorderUtil {
        private TableBorderUtil() {
        }

        public static Border GetCellSideBorder(Cell cellModel, int borderType) {
            Border cellModelSideBorder = cellModel.GetProperty<Border>(borderType);
            if (null == cellModelSideBorder && !cellModel.HasProperty(borderType)) {
                cellModelSideBorder = cellModel.GetProperty<Border>(Property.BORDER);
                if (null == cellModelSideBorder && !cellModel.HasProperty(Property.BORDER)) {
                    cellModelSideBorder = cellModel.GetDefaultProperty<Border>(Property.BORDER);
                }
            }
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

        public static IList<Border> CreateAndFillBorderList(Border border, int size) {
            IList<Border> borderList = new List<Border>();
            for (int i = 0; i < size; i++) {
                borderList.Add(border);
            }
            return borderList;
        }

        public static IList<Border> CreateAndFillBorderList(IList<Border> originalList, Border borderToCollapse, int
             size) {
            IList<Border> borderList;
            if (null != originalList) {
                borderList = new List<Border>(originalList.Count + size);
                borderList.AddAll(originalList);
            }
            else {
                borderList = new List<Border>(size);
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
    }
//\endcond
}
