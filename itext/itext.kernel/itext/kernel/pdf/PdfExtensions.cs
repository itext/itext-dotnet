namespace iTextSharp.Kernel.Pdf {
    public static class PdfExtensions {
        public static PdfDictionary MakeIndirect(this PdfDictionary dict, PdfDocument document) {
            return (PdfDictionary)dict.MakeIndirect(document);
        }
    }
}
