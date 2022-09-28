using iText.Kernel.Pdf;

namespace iText.Pdfa {
    /// <summary>
    /// This class extends
    /// <see cref="PdfADocument"/>
    /// and serves as
    /// <see cref="PdfADocument"/>
    /// for
    /// PDF/A compliant documents and as
    /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
    /// for non PDF/A documents.
    /// </summary>
    /// <remarks>
    /// This class extends
    /// <see cref="PdfADocument"/>
    /// and serves as
    /// <see cref="PdfADocument"/>
    /// for
    /// PDF/A compliant documents and as
    /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
    /// for non PDF/A documents.
    /// <para />
    /// This class can throw various exceptions like
    /// <see cref="iText.Kernel.Exceptions.PdfException"/>
    /// as well as
    /// <see cref="iText.Pdfa.Exceptions.PdfAConformanceException"/>
    /// for PDF/A documents.
    /// </remarks>
    public class PdfAAgnosticPdfDocument : PdfADocument {
        /// <summary>Opens a PDF/A document in stamping mode.</summary>
        /// <param name="reader">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfReader"/>
        /// </param>
        /// <param name="writer">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfWriter"/>
        /// object to write to
        /// </param>
        public PdfAAgnosticPdfDocument(PdfReader reader, PdfWriter writer)
            : this(reader, writer, new StampingProperties()) {
        }

        /// <summary>Opens a PDF/A document in stamping mode.</summary>
        /// <param name="reader">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfReader"/>
        /// </param>
        /// <param name="writer">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfWriter"/>
        /// object to write to
        /// </param>
        /// <param name="properties">
        /// 
        /// <see cref="iText.Kernel.Pdf.StampingProperties"/>
        /// of the stamping process
        /// </param>
        public PdfAAgnosticPdfDocument(PdfReader reader, PdfWriter writer, StampingProperties properties)
            : base(reader, writer, properties, true) {
        }
    }
}
