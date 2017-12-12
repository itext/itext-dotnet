using System;
using System.IO;
using Common.Logging;
using iText.Kernel;
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
        /// <exception cref="System.IO.IOException"/>
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
        /// <exception cref="System.IO.IOException"/>
        public static PdfFileSpec Create(PdfDocument doc, String filePath, PdfEncryptedPayload encryptedPayload, PdfName
             mimeType) {
            return Create(doc, filePath, encryptedPayload, mimeType, null);
        }

        /// <summary>Embed a encrypted payload to a PdfDocument.</summary>
        /// <param name="doc">PdfDocument to add the file to</param>
        /// <param name="filePath">path to the encrypted file</param>
        /// <param name="encryptedPayload">the encrypted payload dictionary</param>
        /// <returns>PdfFileSpec containing the file specification of the encrypted payload</returns>
        /// <exception cref="System.IO.IOException"/>
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
                LogManager.GetLogger(typeof(PdfEncryptedPayloadFileSpecFactory)).Error(iText.IO.LogMessageConstant.ENCRYPTED_PAYLOAD_FILE_SPEC_SHALL_HAVE_AFRELATIONSHIP_FILED_EQUAL_TO_ENCRYPTED_PAYLOAD
                    );
            }
            PdfDictionary ef = dictionary.GetAsDictionary(PdfName.EF);
            if (ef == null || (ef.GetAsStream(PdfName.F) == null) && (ef.GetAsStream(PdfName.UF) == null)) {
                throw new PdfException(PdfException.EncryptedPayloadFileSpecShallHaveEFDictionary);
            }
            if (!PdfName.Filespec.Equals(dictionary.GetAsName(PdfName.Type))) {
                throw new PdfException(PdfException.EncryptedPayloadFileSpecShallHaveTypeEqualToFilespec);
            }
            if (!dictionary.IsIndirect()) {
                throw new PdfException(PdfException.EncryptedPayloadFileSpecShallBeIndirect);
            }
            PdfFileSpec fileSpec = PdfFileSpec.WrapFileSpecObject(dictionary);
            if (PdfEncryptedPayload.ExtractFrom(fileSpec) == null) {
                throw new PdfException(PdfException.EncryptedPayloadFileSpecDoesntHaveEncryptedPayloadDictionary);
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
