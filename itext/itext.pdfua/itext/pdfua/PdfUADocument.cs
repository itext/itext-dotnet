using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Pdfua.Checkers;
using iText.Pdfua.Exceptions;

namespace iText.Pdfua {
    /// <summary>Creates a Pdf/UA document.</summary>
    /// <remarks>
    /// Creates a Pdf/UA document.
    /// This class is an extension of PdfDocument and adds the necessary configuration for PDF/UA conformance.
    /// It will add necessary validation to guide the user to create a PDF/UA compliant document.
    /// </remarks>
    public class PdfUADocument : PdfDocument {
        private static readonly IPdfPageFactory pdfPageFactory = new PdfUAPageFactory();

        private static readonly ILogger LOGGER = ITextLogManager.GetLogger(typeof(iText.Pdfua.PdfUADocument));

        private PdfUAConfig config;

        private bool warnedOnPageFlush = false;

        /// <summary>Creates a PdfUADocument instance.</summary>
        /// <param name="writer">The writer to write the PDF document.</param>
        /// <param name="config">The configuration for the PDF/UA document.</param>
        public PdfUADocument(PdfWriter writer, PdfUAConfig config)
            : this(writer, new DocumentProperties(), config) {
        }

        /// <summary>Creates a PdfUADocument instance.</summary>
        /// <param name="writer">The writer to write the PDF document.</param>
        /// <param name="properties">The properties for the PDF document.</param>
        /// <param name="config">The configuration for the PDF/UA document.</param>
        public PdfUADocument(PdfWriter writer, DocumentProperties properties, PdfUAConfig config)
            : base(ConfigureWriterProperties(writer), properties) {
            SetupUAConfiguration(config);
        }

        /// <summary>Creates a PdfUADocument instance.</summary>
        /// <param name="reader">The reader to read the PDF document.</param>
        /// <param name="writer">The writer to write the PDF document.</param>
        /// <param name="config">The configuration for the PDF/UA document.</param>
        public PdfUADocument(PdfReader reader, PdfWriter writer, PdfUAConfig config)
            : base(reader, ConfigureWriterProperties(writer)) {
            SetupUAConfiguration(config);
        }

        /// <summary>Creates a PdfUADocument instance.</summary>
        /// <param name="reader">The reader to read the PDF document.</param>
        /// <param name="writer">The writer to write the PDF document.</param>
        /// <param name="properties">The properties for the PDF document.</param>
        /// <param name="config">The configuration for the PDF/UA document.</param>
        public PdfUADocument(PdfReader reader, PdfWriter writer, StampingProperties properties, PdfUAConfig config
            )
            : base(reader, ConfigureWriterProperties(writer), properties) {
            SetupUAConfiguration(config);
        }

        /// <summary>{inheritDoc}</summary>
        public override IConformanceLevel GetConformanceLevel() {
            return config.GetConformanceLevel();
        }

        /// <returns>The PageFactory for the PDF/UA document.</returns>
        protected override IPdfPageFactory GetPageFactory() {
            return pdfPageFactory;
        }

        /// <summary>Returns if the document is in the closing state.</summary>
        /// <returns>true if the document is closing, false otherwise.</returns>
        internal virtual bool IsClosing() {
            return this.isClosing;
        }

        /// <summary>Warns the user that the page is being flushed.</summary>
        /// <remarks>
        /// Warns the user that the page is being flushed.
        /// Will only warn once.
        /// </remarks>
        internal virtual void WarnOnPageFlush() {
            if (!warnedOnPageFlush) {
                LOGGER.LogWarning(PdfUALogMessageConstants.PAGE_FLUSHING_DISABLED);
                warnedOnPageFlush = true;
            }
        }

        /// <summary>Disables the warning for page flushing.</summary>
        public virtual void DisablePageFlushingWarning() {
            warnedOnPageFlush = true;
        }

        private void SetupUAConfiguration(PdfUAConfig config) {
            //basic configuration
            this.config = config;
            this.SetTagged();
            this.GetCatalog().SetViewerPreferences(new PdfViewerPreferences().SetDisplayDocTitle(true));
            this.GetCatalog().SetLang(new PdfString(config.GetLanguage()));
            PdfDocumentInfo info = this.GetDocumentInfo();
            info.SetTitle(config.GetTitle());
            //validation
            ValidationContainer validationContainer = new ValidationContainer();
            validationContainer.AddChecker(new PdfUA1Checker(this));
            this.GetDiContainer().Register(typeof(ValidationContainer), validationContainer);
        }

        private static PdfWriter ConfigureWriterProperties(PdfWriter writer) {
            writer.GetProperties().AddUAXmpMetadata();
            return writer;
        }
    }
}
