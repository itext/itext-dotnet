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
using System.Collections.Concurrent;
using System.Collections.Generic;
using iText.Commons.Utils;

namespace iText.Barcodes.Dmcode {
    /// <summary>Utility class that helps to place the data in the barcode.</summary>
    public class Placement {
        private readonly int nrow;

        private readonly int ncol;

        private readonly short[] array;

        private static readonly IDictionary<int, short[]> cache = new ConcurrentDictionary<int, short[]>();

        private Placement(int nrow, int ncol) {
            this.nrow = nrow;
            this.ncol = ncol;
            array = new short[nrow * ncol];
        }

        /// <summary>Execute the placement</summary>
        /// <param name="nrow">number of rows</param>
        /// <param name="ncol">number of columns</param>
        /// <returns>array containing appropriate values for ECC200</returns>
        public static short[] DoPlacement(int nrow, int ncol) {
            int key = nrow * 1000 + ncol;
            short[] pc = cache.Get(key);
            if (pc != null) {
                return pc;
            }
            iText.Barcodes.Dmcode.Placement p = new iText.Barcodes.Dmcode.Placement(nrow, ncol);
            p.Ecc200();
            cache.Put(key, p.array);
            return p.array;
        }

        /* "module" places "chr+bit" with appropriate wrapping within array[] */
        private void Module(int row, int col, int chr, int bit) {
            if (row < 0) {
                row += nrow;
                col += 4 - (nrow + 4) % 8;
            }
            if (col < 0) {
                col += ncol;
                row += 4 - (ncol + 4) % 8;
            }
            array[row * ncol + col] = (short)(8 * chr + bit);
        }

        /* "utah" places the 8 bits of a utah-shaped symbol character in ECC200 */
        private void Utah(int row, int col, int chr) {
            Module(row - 2, col - 2, chr, 0);
            Module(row - 2, col - 1, chr, 1);
            Module(row - 1, col - 2, chr, 2);
            Module(row - 1, col - 1, chr, 3);
            Module(row - 1, col, chr, 4);
            Module(row, col - 2, chr, 5);
            Module(row, col - 1, chr, 6);
            Module(row, col, chr, 7);
        }

        /* "cornerN" places 8 bits of the four special corner cases in ECC200 */
        private void Corner1(int chr) {
            Module(nrow - 1, 0, chr, 0);
            Module(nrow - 1, 1, chr, 1);
            Module(nrow - 1, 2, chr, 2);
            Module(0, ncol - 2, chr, 3);
            Module(0, ncol - 1, chr, 4);
            Module(1, ncol - 1, chr, 5);
            Module(2, ncol - 1, chr, 6);
            Module(3, ncol - 1, chr, 7);
        }

        private void Corner2(int chr) {
            Module(nrow - 3, 0, chr, 0);
            Module(nrow - 2, 0, chr, 1);
            Module(nrow - 1, 0, chr, 2);
            Module(0, ncol - 4, chr, 3);
            Module(0, ncol - 3, chr, 4);
            Module(0, ncol - 2, chr, 5);
            Module(0, ncol - 1, chr, 6);
            Module(1, ncol - 1, chr, 7);
        }

        private void Corner3(int chr) {
            Module(nrow - 3, 0, chr, 0);
            Module(nrow - 2, 0, chr, 1);
            Module(nrow - 1, 0, chr, 2);
            Module(0, ncol - 2, chr, 3);
            Module(0, ncol - 1, chr, 4);
            Module(1, ncol - 1, chr, 5);
            Module(2, ncol - 1, chr, 6);
            Module(3, ncol - 1, chr, 7);
        }

        private void Corner4(int chr) {
            Module(nrow - 1, 0, chr, 0);
            Module(nrow - 1, ncol - 1, chr, 1);
            Module(0, ncol - 3, chr, 2);
            Module(0, ncol - 2, chr, 3);
            Module(0, ncol - 1, chr, 4);
            Module(1, ncol - 3, chr, 5);
            Module(1, ncol - 2, chr, 6);
            Module(1, ncol - 1, chr, 7);
        }

        /* "ECC200" fills an nrow x ncol array with appropriate values for ECC200 */
        private void Ecc200() {
            int row;
            int col;
            int chr;
            /* First, fill the array[] with invalid entries */
            JavaUtil.Fill(array, (short)0);
            /* Starting in the correct location for character #1, bit 8,... */
            chr = 1;
            row = 4;
            col = 0;
            do {
                /* repeatedly first check for one of the special corner cases, then... */
                if (row == nrow && col == 0) {
                    Corner1(chr++);
                }
                if (row == nrow - 2 && col == 0 && ncol % 4 != 0) {
                    Corner2(chr++);
                }
                if (row == nrow - 2 && col == 0 && ncol % 8 == 4) {
                    Corner3(chr++);
                }
                if (row == nrow + 4 && col == 2 && ncol % 8 == 0) {
                    Corner4(chr++);
                }
                /* sweep upward diagonally, inserting successive characters,... */
                do {
                    if (row < nrow && col >= 0 && array[row * ncol + col] == 0) {
                        Utah(row, col, chr++);
                    }
                    row -= 2;
                    col += 2;
                }
                while (row >= 0 && col < ncol);
                row += 1;
                col += 3;
                /* & then sweep downward diagonally, inserting successive characters,... */
                do {
                    if (row >= 0 && col < ncol && array[row * ncol + col] == 0) {
                        Utah(row, col, chr++);
                    }
                    row += 2;
                    col -= 2;
                }
                while (row < nrow && col >= 0);
                row += 3;
                col += 1;
            }
            /* ... until the entire array is scanned */
            while (row < nrow || col < ncol);
            /* Lastly, if the lower righthand corner is untouched, fill in fixed pattern */
            if (array[nrow * ncol - 1] == 0) {
                array[nrow * ncol - 1] = array[nrow * ncol - ncol - 2] = 1;
            }
        }
    }
}
