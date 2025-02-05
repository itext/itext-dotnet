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
using System;
using iText.IO.Source;
using iText.Kernel.Pdf;

namespace iText.Pdfa.Checker {
    public class PdfACheckerTestUtils {
        private PdfACheckerTestUtils() {
        }

//\cond DO_NOT_DOCUMENT
        internal static PdfString GetLongString(int length) {
            return new PdfString(GetLongPlainString(length));
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static PdfName GetLongName(int length) {
            return new PdfName(GetLongPlainString(length));
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static PdfArray GetLongArray(int length) {
            PdfArray array = new PdfArray();
            for (int i = 0; i < length; i++) {
                array.Add(new PdfNumber(i));
            }
            return array;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static PdfDictionary GetLongDictionary(int length) {
            PdfDictionary dict = new PdfDictionary();
            for (int i = 0; i < length; i++) {
                dict.Put(new PdfName("value #" + i), new PdfNumber(i));
            }
            return dict;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static PdfStream GetStreamWithLongDictionary(int length) {
            PdfStream stream = new PdfStream("Hello, world!".GetBytes());
            for (int i = 0; i < length; i++) {
                stream.Put(new PdfName("value #" + i), new PdfNumber(i));
            }
            return stream;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static String GetStreamWithValue(PdfObject @object) {
            ByteArrayOutputStream baos = new ByteArrayOutputStream();
            PdfOutputStream stream = new PdfOutputStream(baos);
            stream.Write(@object);
            return "q\n" + "BT\n" + "/F1 12 Tf\n" + "36 787.96 Td\n" + iText.Commons.Utils.JavaUtil.GetStringForBytes(
                baos.ToArray()) + " Tj\n" + "ET\n" + "Q";
        }
//\endcond

        private static String GetLongPlainString(int length) {
            char charToFill = 'A';
            char[] array = new char[length];
            for (int i = 0; i < array.Length; i++) {
                array[i] = charToFill;
            }
            return new String(array);
        }
    }
}
