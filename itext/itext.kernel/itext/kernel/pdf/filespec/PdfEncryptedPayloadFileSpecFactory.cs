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
using System.IO;
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Kernel.Exceptions;
using iText.Kernel.Pdf;

namespace iText.Kernel.Pdf.Filespec {
    public class PdfEncryptedPayloadFileSpecFactory {
        /// <summary>Embed a encrypted payload to a PdfDocument.</summary>
        /// <param name="doc">PdfDocument to add the file to</param>
        /// <param name="fileStore">byte[] containing encrypted file</param>
        /// <param name="encryptedPayload">the encrypted payload dictionary</param>
        /// <param name="mimeType">mime-type of the file</param>
        /// <param name="fileParameter">Pdfdictionary containing file parameters</param>
        /// <returns>PdfFileSpec containing the file specification of the encrypted payload</returns>
        public static PdfFileSpec Create(PdfDocument doc, byte[] fileStore, PdfEncryptedPayload encryptedPayload, 
            PdfName mimeType, PdfDictionary fileParameter) {
            return AddEncryptedPayloadDictionary(PdfFileSpec.CreateEmbeddedFileSpec(doc, fileStore, GenerateDescription
                (encryptedPayload), GenerateFileDisplay(encryptedPayload), mimeType, fileParameter, PdfName.EncryptedPayload
                ), encryptedPayload);
        }

        /// <summary>Embed a encrypted payload to a PdfDocument.</summary>
        /// <param name="doc">PdfDocument to add the file to</param>
        /// <param name="fileStore">byte[] containing the file</param>
        /// <param name="encryptedPayload">the encrypted payload dictionary</param>
        /// <param name="fileParameter">Pdfdictionary containing file parameters</param>
        /// <returns>PdfFileSpec containing the file specification of the encrypted payload</returns>
        public static PdfFileSpec Create(PdfDocument doc, byte[] fileStore, PdfEncryptedPayload encryptedPayload, 
            PdfDictionary fileParameter) {
            return Create(doc, fileStore, encryptedPayload, null, fileParameter);
        }

        /// <summary>Embed a encrypted payload to a PdfDocument.</summary>
        /// <param name="doc">PdfDocument to add the file to</param>
        /// <param name="fileStore">byte[] containing the file</param>
        /// <param name="encryptedPayload">the encrypted payload dictionary</param>
        /// <returns>PdfFileSpec containing the file specification of the encrypted payload</returns>
        public static PdfFileSpec Create(PdfDocument doc, byte[] fileStore, PdfEncryptedPayload encryptedPayload) {
            return Create(doc, fileStore, encryptedPayload, null, null);
        }

        /// <summary>Embed a encrypted payload to a PdfDocument.</summary>
        /// <param name="doc">PdfDocument to add the file to</param>
        /// <param name="filePath">path to the encrypted file</param>
        /// <param name="encryptedPayload">the encrypted payload dictionary</param>
        /// <param name="mimeType">mime-type of the file</param>
        /// <param name="fileParameter">Pdfdictionary containing file parameters</param>
        /// <returns>PdfFileSpec containing the file specification of the encrypted payload</returns>
        public static PdfFileSpec Create(PdfDocument doc, String filePath, PdfEncryptedPayload encryptedPayload, PdfName
             mimeType, PdfDictionary fileParameter) {
            return AddEncryptedPayloadDictionary(PdfFileSpec.CreateEmbeddedFileSpec(doc, filePath, GenerateDescription
                (encryptedPayload), GenerateFileDisplay(encryptedPayload), mimeType, fileParameter, PdfName.EncryptedPayload
                ), encryptedPayload);
        }

        /// <summary>Embed a encrypted payload to a PdfDocument.</summary>
        /// <param name="doc">PdfDocument to add the file to</param>
        /// <param name="filePath">path to the encrypted file</param>
        /// <param name="encryptedPayload">the encrypted payload dictionary</param>
        /// <param name="mimeType">mime-type of the file</param>
        /// <returns>PdfFileSpec containing the file specification of the encrypted payload</returns>
        public static PdfFileSpec Create(PdfDocument doc, String filePath, PdfEncryptedPayload encryptedPayload, PdfName
             mimeType) {
            return Create(doc, filePath, encryptedPayload, mimeType, null);
        }

        /// <summary>Embed a encrypted payload to a PdfDocument.</summary>
        /// <param name="doc">PdfDocument to add the file to</param>
        /// <param name="filePath">path to the encrypted file</param>
        /// <param name="encryptedPayload">the encrypted payload dictionary</param>
        /// <returns>PdfFileSpec containing the file specification of the encrypted payload</returns>
        public static PdfFileSpec Create(PdfDocument doc, String filePath, PdfEncryptedPayload encryptedPayload) {
            return Create(doc, filePath, encryptedPayload, null, null);
        }

        /// <summary>Embed a encrypted payload to a PdfDocument.</summary>
        /// <param name="doc">PdfDocument to add the file to</param>
        /// <param name="is">stream containing encrypted file</param>
        /// <param name="encryptedPayload">the encrypted payload dictionary</param>
        /// <param name="mimeType">mime-type of the file</param>
        /// <param name="fileParameter">Pdfdictionary containing file parameters</param>
        /// <returns>PdfFileSpec containing the file specification of the encrypted payload</returns>
        public static PdfFileSpec Create(PdfDocument doc, Stream @is, PdfEncryptedPayload encryptedPayload, PdfName
             mimeType, PdfDictionary fileParameter) {
            return AddEncryptedPayloadDictionary(PdfFileSpec.CreateEmbeddedFileSpec(doc, @is, GenerateDescription(encryptedPayload
                ), GenerateFileDisplay(encryptedPayload), mimeType, fileParameter, PdfName.EncryptedPayload), encryptedPayload
                );
        }

        /// <summary>Embed a encrypted payload to a PdfDocument.</summary>
        /// <param name="doc">PdfDocument to add the file to</param>
        /// <param name="is">stream containing encrypted file</param>
        /// <param name="encryptedPayload">the encrypted payload dictionary</param>
        /// <param name="mimeType">mime-type of the file</param>
        /// <returns>PdfFileSpec containing the file specification of the encrypted payload</returns>
        public static PdfFileSpec Create(PdfDocument doc, Stream @is, PdfEncryptedPayload encryptedPayload, PdfName
             mimeType) {
            return Create(doc, @is, encryptedPayload, mimeType, null);
        }

        /// <summary>Embed a encrypted payload to a PdfDocument.</summary>
        /// <param name="doc">PdfDocument to add the file to</param>
        /// <param name="is">stream containing encrypted file</param>
        /// <param name="encryptedPayload">the encrypted payload dictionary</param>
        /// <returns>PdfFileSpec containing the file specification of the encrypted payload</returns>
        public static PdfFileSpec Create(PdfDocument doc, Stream @is, PdfEncryptedPayload encryptedPayload) {
            return Create(doc, @is, encryptedPayload, null, null);
        }

        public static PdfFileSpec Wrap(PdfDictionary dictionary) {
            if (!PdfName.EncryptedPayload.Equals(dictionary.GetAsName(PdfName.AFRelationship))) {
                ITextLogManager.GetLogger(typeof(PdfEncryptedPayloadFileSpecFactory)).LogError(iText.IO.Logs.IoLogMessageConstant
                    .ENCRYPTED_PAYLOAD_FILE_SPEC_SHALL_HAVE_AFRELATIONSHIP_FILED_EQUAL_TO_ENCRYPTED_PAYLOAD);
            }
            PdfDictionary ef = dictionary.GetAsDictionary(PdfName.EF);
            if (ef == null || (ef.GetAsStream(PdfName.F) == null) && (ef.GetAsStream(PdfName.UF) == null)) {
                throw new PdfException(KernelExceptionMessageConstant.ENCRYPTED_PAYLOAD_FILE_SPEC_SHALL_HAVE_EF_DICTIONARY
                    );
            }
            if (!PdfName.Filespec.Equals(dictionary.GetAsName(PdfName.Type))) {
                throw new PdfException(KernelExceptionMessageConstant.ENCRYPTED_PAYLOAD_FILE_SPEC_SHALL_HAVE_TYPE_EQUAL_TO_FILESPEC
                    );
            }
            if (!dictionary.IsIndirect()) {
                throw new PdfException(KernelExceptionMessageConstant.ENCRYPTED_PAYLOAD_FILE_SPEC_SHALL_BE_INDIRECT);
            }
            PdfFileSpec fileSpec = PdfFileSpec.WrapFileSpecObject(dictionary);
            if (PdfEncryptedPayload.ExtractFrom(fileSpec) == null) {
                throw new PdfException(KernelExceptionMessageConstant.ENCRYPTED_PAYLOAD_FILE_SPEC_DOES_NOT_HAVE_ENCRYPTED_PAYLOAD_DICTIONARY
                    );
            }
            return fileSpec;
        }

        // Note as stated by spec the desscription and file display
        // shall not be derived from the encrypted payload's actual file name
        // to avoid potential disclosure of sensitive information
        public static String GenerateDescription(PdfEncryptedPayload ep) {
            String result = "This embedded file is encrypted using " + ep.GetSubtype().GetValue();
            PdfName version = ep.GetVersion();
            if (version != null) {
                result += " , version: " + version.GetValue();
            }
            return result;
        }

        public static String GenerateFileDisplay(PdfEncryptedPayload ep) {
            return ep.GetSubtype().GetValue() + "Protected.pdf";
        }

        private static PdfFileSpec AddEncryptedPayloadDictionary(PdfFileSpec fs, PdfEncryptedPayload ep) {
            ((PdfDictionary)fs.GetPdfObject()).Put(PdfName.EP, ep.GetPdfObject());
            return fs;
        }
    }
}
