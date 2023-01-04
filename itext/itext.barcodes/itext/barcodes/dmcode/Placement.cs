/*

This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: Bruno Lowagie, Paulo Soares, et al.

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License version 3
as published by the Free Software Foundation with the addition of the
following permission added to Section 15 as permitted in Section 7(a):
FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
OF THIRD PARTY RIGHTS

This program is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU Affero General Public License for more details.
You should have received a copy of the GNU Affero General Public License
along with this program; if not, see http://www.gnu.org/licenses or write to
the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
Boston, MA, 02110-1301 USA, or download the license from the following URL:
http://itextpdf.com/terms-of-use/

The interactive user interfaces in modified source and object code versions
of this program must display Appropriate Legal Notices, as required under
Section 5 of the GNU Affero General Public License.

In accordance with Section 7(b) of the GNU Affero General Public License,
a covered work must retain the producer line in every PDF that is created
or manipulated using iText.

You can be released from the requirements of the license by purchasing
a commercial license. Buying such a license is mandatory as soon as you
develop commercial activities involving the iText software without
disclosing the source code of your own applications.
These activities include: offering paid services to customers as an ASP,
serving PDFs on the fly in a web application, shipping iText with a closed
source product.

For more information, please contact iText Software Corp. at this
address: sales@itextpdf.com
*/
using System.Collections.Concurrent;
using System.Collections.Generic;
using iText.Commons.Utils;

namespace iText.Barcodes.Dmcode {
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
