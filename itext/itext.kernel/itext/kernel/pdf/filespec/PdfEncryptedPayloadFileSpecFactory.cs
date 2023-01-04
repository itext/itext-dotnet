/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: iText Software.

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License version 3
as published by the Free Software Foundation with the addition of the
following permission added to Section 15 as permitted in Section 7(a):
FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
OF THIRD PARTY RIGHTS

This program is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU Affero General Public License for more details.
You should have received a copy of the GNU Affero General Public License
along with this program; if not, see http://www.gnu.org/licenses or write to
the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
Boston, MA, 02110-1301 USA, or download the license from the following URL:
http://itextpdf.com/terms-of-use/

The interactive user interfaces in modified source and object code versions
of this program must display Appropriate Legal Notices, as required under
Section 5 of the GNU Affero General Public License.

In accordance with Section 7(b) of the GNU Affero General Public License,
a covered work must retain the producer line in every PDF that is created
or manipulated using iText.

You can be released from the requirements of the license by purchasing
a commercial license. Buying such a license is mandatory as soon as you
develop commercial activities involving the iText software without
disclosing the source code of your own applications.
These activities include: offering paid services to customers as an ASP,
serving PDFs on the fly in a web application, shipping iText with a closed
source product.

For more information, please contact iText Software Corp. at this
address: sales@itextpdf.com
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
