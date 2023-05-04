namespace iText.Kernel.Pdf {
    /// <summary>
    /// This interface extends the logic of the {#link IPdfPageExtraCopier} interface to
    /// copy AcroForm fields to a new page.
    /// </summary>
    public interface IPdfPageFormCopier : IPdfPageExtraCopier {
        /// <summary>Create Acroform from its PDF object to process form field objects added to the Acroform during copying.
        ///     </summary>
        /// <remarks>
        /// Create Acroform from its PDF object to process form field objects added to the Acroform during copying.
        /// <para />
        /// All pages must already be copied to the target document before calling this. So fields with the same names will
        /// be merged and target document tag structure will be correct.
        /// </remarks>
        /// <param name="documentTo">the target document.</param>
        void RecreateAcroformToProcessCopiedFields(PdfDocument documentTo);
    }
}
