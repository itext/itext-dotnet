using System;
using iText.IO.Log;

namespace iText.Kernel.Pdf {
    public class VersionConforming {
        internal const String DEPRECATED_AES256_REVISION = "It seems that PDF 1.7 document encrypted with AES256 was updated to PDF 2.0 version and StampingProperties#preserveEncryption flag was set: encryption shall be updated via WriterProperties#setStandardEncryption method. Standard security handler with revision 5";

        internal const String DEPRECATED_ENCRYPTION_ALGORITHMS = "Encryption algorithms STANDARD_ENCRYPTION_40, STANDARD_ENCRYPTION_128 and ENCRYPTION_AES_128 (see com.itextpdf.kernel.pdf.EncryptionConstants) usage";

        private static readonly ILogger logger = LoggerFactory.GetLogger(typeof(VersionConforming));

        // TODO consider naming?
        // TODO consider this class access level and package
        // TODO consider constants location
        public static void ValidatePdfVersionForDictEntry(PdfDocument document, PdfVersion expectedVersion, PdfName
             entryKey, PdfName dictType) {
            if (document.GetPdfVersion().CompareTo(expectedVersion) < 0) {
                logger.Warn(String.Format(iText.IO.LogMessageConstant.VERSION_INCOMPATIBILITY_FOR_DICTIONARY_ENTRY, entryKey
                    , dictType, expectedVersion, document.GetPdfVersion()));
            }
        }

        public static void ValidatePdfVersionForDeprecatedFeature(PdfDocument document, PdfVersion expectedVersion
            , String deprecatedFeatureDescription) {
            if (document.GetPdfVersion().CompareTo(expectedVersion) >= 0) {
                logger.Warn(String.Format(iText.IO.LogMessageConstant.FEATURE_IS_DEPRECATED, deprecatedFeatureDescription, 
                    expectedVersion));
            }
        }
    }
}
