/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: iText Software.

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
namespace iText.IO.Util {
    /// <summary>
    /// A helper data class, which aggregates true/false result of ImageMagick comparing
    /// as well as the number of different pixels.
    /// </summary>
    public sealed class ImageMagickCompareResult {
        private readonly bool result;

        private readonly long diffPixels;

        /// <summary>Creates an instance that contains ImageMagick comparing result information.</summary>
        /// <param name="result">true, if the compared images are equal.</param>
        /// <param name="diffPixels">number of different pixels.</param>
        public ImageMagickCompareResult(bool result, long diffPixels) {
            this.result = result;
            this.diffPixels = diffPixels;
        }

        /// <summary>Returns image compare boolean value.</summary>
        /// <returns>true if the compared images are equal.</returns>
        public bool IsComparingResultSuccessful() {
            return result;
        }

        /// <summary>Getter for a different pixels count.</summary>
        /// <returns>Returns a a different pixels count.</returns>
        public long GetDiffPixels() {
            return diffPixels;
        }
    }
}
