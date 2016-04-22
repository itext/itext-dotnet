namespace com.itextpdf.kernel.pdf
{
    public static class PdfExtensions
    {
        public static PdfDictionary MakeIndirect(this PdfDictionary dict, PdfDocument document)
        {
            return (PdfDictionary) dict.makeIndirect(document);
        } 
    }
}