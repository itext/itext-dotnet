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
