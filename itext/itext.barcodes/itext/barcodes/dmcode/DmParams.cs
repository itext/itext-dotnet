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
namespace iText.Barcodes.Dmcode {
    /// <summary>Class that contains the parameters for a DM code.</summary>
    /// <remarks>
    /// Class that contains the parameters for a DM code.
    /// It contains all the information needed to create one data matrix entry
    /// </remarks>
    public class DmParams {
        /// <summary>Creates a DM code parameter block</summary>
        /// <param name="height">total height</param>
        /// <param name="width">total width</param>
        /// <param name="heightSection">height of a single section</param>
        /// <param name="widthSection">width of a single section</param>
        /// <param name="dataSize">size of the data</param>
        /// <param name="dataBlock">size of a data-block</param>
        /// <param name="errorBlock">size of a error-correction block</param>
        public DmParams(int height, int width, int heightSection, int widthSection, int dataSize, int dataBlock, int
             errorBlock) {
            this.height = height;
            this.width = width;
            this.heightSection = heightSection;
            this.widthSection = widthSection;
            this.dataSize = dataSize;
            this.dataBlock = dataBlock;
            this.errorBlock = errorBlock;
        }

        private readonly int height;

        private readonly int width;

        private readonly int heightSection;

        private readonly int widthSection;

        private readonly int dataSize;

        private readonly int dataBlock;

        private readonly int errorBlock;

        /// <summary>Retrieves the height of DmParams object.</summary>
        /// <returns>total height value</returns>
        public virtual int GetHeight() {
            return height;
        }

        /// <summary>Retrieves the width of DmParams object.</summary>
        /// <returns>total width value</returns>
        public virtual int GetWidth() {
            return width;
        }

        /// <summary>Retrieves the height of a single section.</summary>
        /// <returns>total height value</returns>
        public virtual int GetHeightSection() {
            return heightSection;
        }

        /// <summary>Retrieves the width of a single section.</summary>
        /// <returns>total width value</returns>
        public virtual int GetWidthSection() {
            return widthSection;
        }

        /// <summary>Retrieves the size of the data.</summary>
        /// <returns>data size value</returns>
        public virtual int GetDataSize() {
            return dataSize;
        }

        /// <summary>Retrieves the size of the data block.</summary>
        /// <returns>data block size value</returns>
        public virtual int GetDataBlock() {
            return dataBlock;
        }

        /// <summary>Retrieves the size of the error block.</summary>
        /// <returns>error block size value</returns>
        public virtual int GetErrorBlock() {
            return errorBlock;
        }
    }
}
