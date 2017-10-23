using System;
using Common.Logging;

namespace iText.Kernel.Pdf {
    public class VersionConforming {
        public const String DEPRECATED_AES256_REVISION = "It seems that PDF 1.7 document encrypted with AES256 was updated to PDF 2.0 version and StampingProperties#preserveEncryption flag was set: encryption shall be updated via WriterProperties#setStandardEncryption method. Standard security handler was found with revision 5, which is deprecated and shall not be used in PDF 2.0 documents.";

        public const String DEPRECATED_ENCRYPTION_ALGORITHMS = "Encryption algorithms STANDARD_ENCRYPTION_40, STANDARD_ENCRYPTION_128 and ENCRYPTION_AES_128 (see com.itextpdf.kernel.pdf.EncryptionConstants) are deprecated in PDF 2.0. It is highly recommended not to use it.";

        public const String DEPRECATED_NEED_APPEARANCES_IN_ACROFORM = "NeedAppearances has been deprecated in PDF 2.0. Appearance streams are required in PDF 2.0.";

        public const String DEPRECATED_XFA_FORMS = "XFA is deprecated in PDF 2.0. The XFA form will not be written to the document";

        private static readonly ILog logger = LogManager.GetLogger(typeof(VersionConforming));

        public static bool ValidatePdfVersionForDictEntry(PdfDocument document, PdfVersion expectedVersion, PdfName
             entryKey, PdfName dictType) {
            if (document != null && document.GetPdfVersion().CompareTo(expectedVersion) < 0) {
                logger.Warn(String.Format(iText.IO.LogMessageConstant.VERSION_INCOMPATIBILITY_FOR_DICTIONARY_ENTRY, entryKey
                    , dictType, expectedVersion, document.GetPdfVersion()));
                return true;
            }
            else {
                return false;
            }
        }

        public static bool ValidatePdfVersionForDeprecatedFeatureLogWarn(PdfDocument document, PdfVersion expectedVersion
            , String deprecatedFeatureLogMessage) {
            if (document.GetPdfVersion().CompareTo(expectedVersion) >= 0) {
                logger.Warn(deprecatedFeatureLogMessage);
                return true;
            }
            else {
                return false;
            }
        }

        public static bool ValidatePdfVersionForDeprecatedFeatureLogError(PdfDocument document, PdfVersion expectedVersion
            , String deprecatedFeatureLogMessage) {
            if (document.GetPdfVersion().CompareTo(expectedVersion) >= 0) {
                logger.Error(deprecatedFeatureLogMessage);
                return true;
            }
            else {
                return false;
            }
        }
    }
}
