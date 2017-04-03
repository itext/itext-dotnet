using System;
using iText.IO.Log;

namespace iText.Kernel.Pdf {
    public class VersionConforming {
        private static readonly ILogger logger = LoggerFactory.GetLogger(typeof(VersionConforming));

        // TODO consider naming?
        public static void EnsurePdfVersionForDictEntry(PdfDocument document, PdfVersion expectedVersion, PdfName 
            entryKey, PdfName dictType) {
            if (document.GetPdfVersion().CompareTo(expectedVersion) < 0) {
                logger.Warn(String.Format(iText.IO.LogMessageConstant.VERSION_INCOMPATIBILITY_FOR_DICTIONARY_ENTRY, entryKey
                    , dictType, expectedVersion, document.GetPdfVersion()));
            }
        }
    }
}
