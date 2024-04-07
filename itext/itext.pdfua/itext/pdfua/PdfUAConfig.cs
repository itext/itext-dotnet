using System;
using iText.Kernel.Pdf;

namespace iText.Pdfua {
    /// <summary>Class that holds the configuration for the PDF/UA document.</summary>
    public class PdfUAConfig {
        private readonly PdfUAConformanceLevel conformanceLevel;

        private readonly String title;

        private readonly String language;

        /// <summary>Creates a new PdfUAConfig instance.</summary>
        /// <param name="conformanceLevel">The conformance level of the PDF/UA document.</param>
        /// <param name="title">The title of the PDF/UA document.</param>
        /// <param name="language">The language of the PDF/UA document.</param>
        public PdfUAConfig(PdfUAConformanceLevel conformanceLevel, String title, String language) {
            this.conformanceLevel = conformanceLevel;
            this.title = title;
            this.language = language;
        }

        /// <summary>Gets the conformance level.</summary>
        /// <returns>
        /// The
        /// <see cref="iText.Kernel.Pdf.PdfUAConformanceLevel"/>.
        /// </returns>
        public virtual PdfUAConformanceLevel GetConformanceLevel() {
            return conformanceLevel;
        }

        /// <summary>Gets the title.</summary>
        /// <returns>The title.</returns>
        public virtual String GetTitle() {
            return title;
        }

        /// <summary>Gets the language.</summary>
        /// <returns>The language.</returns>
        public virtual String GetLanguage() {
            return language;
        }
    }
}
