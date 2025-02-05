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
using iText.Kernel.Pdf;

namespace iText.Kernel.Pdf.Function {
    public sealed class PdfFunctionUtil {
        private PdfFunctionUtil() {
        }

        // do nothing
        public static PdfDictionary CreateMinimalPdfType0FunctionDict() {
            PdfDictionary type0Func = new PdfDictionary();
            type0Func.Put(PdfName.FunctionType, new PdfNumber(0));
            PdfArray domain = new PdfArray(new int[] { 0, 1, 0, 1 });
            type0Func.Put(PdfName.Domain, domain);
            type0Func.Put(PdfName.Size, new PdfArray(new double[] { 2, 2 }));
            return type0Func;
        }

        public static PdfDictionary CreateMinimalPdfType2FunctionDict() {
            PdfDictionary type2Func = new PdfDictionary();
            type2Func.Put(PdfName.FunctionType, new PdfNumber(2));
            PdfArray domain = new PdfArray(new int[] { 0, 1 });
            type2Func.Put(PdfName.Domain, domain);
            type2Func.Put(PdfName.N, new PdfNumber(2));
            return type2Func;
        }
    }
}
