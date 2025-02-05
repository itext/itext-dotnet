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
namespace iText.IO.Font.Otf {
    public class GposValueRecord {
        private int xPlacement;

        private int yPlacement;

        private int xAdvance;

        private int yAdvance;

        /// <summary>Retrieves the X placement of the Gpos value record.</summary>
        /// <returns>X placement</returns>
        public virtual int GetXPlacement() {
            return xPlacement;
        }

        /// <summary>Sets the X placement of the Gpos value record.</summary>
        /// <param name="xPlacement">X placement</param>
        public virtual void SetXPlacement(int xPlacement) {
            this.xPlacement = xPlacement;
        }

        /// <summary>Retrieves the Y placement of the Gpos value record.</summary>
        /// <returns>Y placement</returns>
        public virtual int GetYPlacement() {
            return yPlacement;
        }

        /// <summary>Sets the Y placement of the Gpos value record.</summary>
        /// <param name="yPlacement">Y placement</param>
        public virtual void SetYPlacement(int yPlacement) {
            this.yPlacement = yPlacement;
        }

        /// <summary>Retrieves the X advance of the Gpos value record.</summary>
        /// <returns>x advance</returns>
        public virtual int GetXAdvance() {
            return xAdvance;
        }

        /// <summary>Sets the X advance of the Gpos value record.</summary>
        /// <param name="xAdvance">X advance</param>
        public virtual void SetXAdvance(int xAdvance) {
            this.xAdvance = xAdvance;
        }

        /// <summary>Retrieves the Y advance of the Gpos value record.</summary>
        /// <returns>Y advance</returns>
        public virtual int GetYAdvance() {
            return yAdvance;
        }

        /// <summary>Sets the Y advance of the Gpos value record.</summary>
        /// <param name="yAdvance">Y advance</param>
        public virtual void SetYAdvance(int yAdvance) {
            this.yAdvance = yAdvance;
        }
    }
}
