/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 Apryse Group NV
Authors: Apryse Software.

This program is offered under a commercial and under the AGPL license.
For commercial licensing, contact us at https://itextpdf.com/sales.  For AGPL licensing, see below.

AGPL licensing:
This program is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/
using System;
using Microsoft.Extensions.Logging;
using iText.Commons;

namespace iText.Kernel.Pdf {
    public class VersionConforming {
        public const String DEPRECATED_AES256_REVISION = "It seems that PDF 1.7 document encrypted with AES256 was updated to PDF 2.0 version and StampingProperties#preserveEncryption flag was set: encryption shall be updated via WriterProperties#setStandardEncryption method. Standard security handler was found with revision 5, which is deprecated and shall not be used in PDF 2.0 documents.";

        public const String DEPRECATED_ENCRYPTION_ALGORITHMS = "Encryption algorithms STANDARD_ENCRYPTION_40, STANDARD_ENCRYPTION_128 and ENCRYPTION_AES_128 (see com.itextpdf.kernel.pdf.EncryptionConstants) are deprecated in PDF 2.0. It is highly recommended not to use it.";

        public const String DEPRECATED_NEED_APPEARANCES_IN_ACROFORM = "NeedAppearances has been deprecated in PDF 2.0. Appearance streams are required in PDF 2.0.";

        public const String DEPRECATED_XFA_FORMS = "XFA is deprecated in PDF 2.0. The XFA form will not be written to the document";

        private static readonly ILogger logger = ITextLogManager.GetLogger(typeof(VersionConforming));

        public static bool ValidatePdfVersionForDictEntry(PdfDocument document, PdfVersion expectedVersion, PdfName
             entryKey, PdfName dictType) {
            if (document != null && document.GetPdfVersion().CompareTo(expectedVersion) < 0) {
                logger.LogWarning(String.Format(iText.IO.Logs.IoLogMessageConstant.VERSION_INCOMPATIBILITY_FOR_DICTIONARY_ENTRY
                    , entryKey, dictType, expectedVersion, document.GetPdfVersion()));
                return true;
            }
            else {
                return false;
            }
        }

        public static bool ValidatePdfVersionForDeprecatedFeatureLogWarn(PdfDocument document, PdfVersion expectedVersion
            , String deprecatedFeatureLogMessage) {
            if (document.GetPdfVersion().CompareTo(expectedVersion) >= 0) {
                logger.LogWarning(deprecatedFeatureLogMessage);
                return true;
            }
            else {
                return false;
            }
        }

        public static bool ValidatePdfVersionForDeprecatedFeatureLogError(PdfDocument document, PdfVersion expectedVersion
            , String deprecatedFeatureLogMessage) {
            if (document.GetPdfVersion().CompareTo(expectedVersion) >= 0) {
                logger.LogError(deprecatedFeatureLogMessage);
                return true;
            }
            else {
                return false;
            }
        }
    }
}
