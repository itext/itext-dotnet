/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 Apryse Group NV
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

        public int height;

        public int width;

        public int heightSection;

        public int widthSection;

        public int dataSize;

        public int dataBlock;

        public int errorBlock;
    }
}
