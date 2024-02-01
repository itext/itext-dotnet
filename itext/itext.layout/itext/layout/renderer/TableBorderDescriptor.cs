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
namespace iText.Layout.Renderer {
    internal class TableBorderDescriptor {
        private int borderIndex;

        private float mainCoordinateStart;

        private float crossCoordinate;

        private float[] mainCoordinateWidths;

        /// <summary>Creates a table border descriptor which will be used while drawing the described border.</summary>
        /// <param name="borderIndex">the index of the described border</param>
        /// <param name="mainCoordinateStart">the border's start main-axis coordinate</param>
        /// <param name="crossCoordinate">fixed cross-axis coordinate of the whole border</param>
        /// <param name="mainCoordinateWidths">the sizes (widths or heights) of rows or columns depending on the type of main axis
        ///     </param>
        public TableBorderDescriptor(int borderIndex, float mainCoordinateStart, float crossCoordinate, float[] mainCoordinateWidths
            ) {
            this.borderIndex = borderIndex;
            this.mainCoordinateStart = mainCoordinateStart;
            this.crossCoordinate = crossCoordinate;
            this.mainCoordinateWidths = mainCoordinateWidths;
        }

        public virtual int GetBorderIndex() {
            return borderIndex;
        }

        public virtual float GetMainCoordinateStart() {
            return mainCoordinateStart;
        }

        public virtual float GetCrossCoordinate() {
            return crossCoordinate;
        }

        public virtual float[] GetMainCoordinateWidths() {
            return mainCoordinateWidths;
        }
    }
}
