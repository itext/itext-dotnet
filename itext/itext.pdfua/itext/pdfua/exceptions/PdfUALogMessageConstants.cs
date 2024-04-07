using System;

namespace iText.Pdfua.Exceptions {
    /// <summary>Class containing the log message constants.</summary>
    public sealed class PdfUALogMessageConstants {
        public const String PAGE_FLUSHING_DISABLED = "Page flushing is disabled in PDF/UA mode to allow UA checks "
             + "to be applied. Page will only be flushed on closing.";

        private PdfUALogMessageConstants() {
        }
        // empty constructor
    }
}
