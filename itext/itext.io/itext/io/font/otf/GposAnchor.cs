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
namespace iText.IO.Font.Otf {
    public class GposAnchor {
        private int xCoordinate;

        private int yCoordinate;

        public GposAnchor() {
        }

        /// <summary>Creates a Gpos Anchor object based on a given Gpos Anchor object.</summary>
        /// <param name="other">other Gpos Anchor object</param>
        public GposAnchor(iText.IO.Font.Otf.GposAnchor other) {
            this.xCoordinate = other.xCoordinate;
            this.yCoordinate = other.yCoordinate;
        }

        /// <summary>Retrieves the X coordinate of the Gpos Anchor.</summary>
        /// <returns>X coordinate</returns>
        public virtual int GetXCoordinate() {
            return xCoordinate;
        }

        /// <summary>Sets the x coordinate of the Gpos Anchor.</summary>
        /// <param name="xCoordinate">X coordinate</param>
        public virtual void SetXCoordinate(int xCoordinate) {
            this.xCoordinate = xCoordinate;
        }

        /// <summary>Retrieves the Y coordinate of the Gpos Anchor.</summary>
        /// <returns>Y coordinate</returns>
        public virtual int GetYCoordinate() {
            return yCoordinate;
        }

        /// <summary>Sets the Y coordinate of the Gpos Anchor.</summary>
        /// <param name="yCoordinate">Y coordinate</param>
        public virtual void SetYCoordinate(int yCoordinate) {
            this.yCoordinate = yCoordinate;
        }
    }
}
