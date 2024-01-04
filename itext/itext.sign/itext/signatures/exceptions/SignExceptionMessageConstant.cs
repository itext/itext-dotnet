/*
This file is part of the iText (R) project.
Copyright (c) 1998-2024 Apryse Group NV
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

namespace iText.Signatures.Exceptions {
    /// <summary>Class that bundles all the error message templates as constants.</summary>
    public sealed class SignExceptionMessageConstant {
        public const String ALGORITHMS_NOT_SUPPORTED = "Signing algorithms {0} and {1} are not supported.";

        public const String AUTHENTICATED_ATTRIBUTE_IS_MISSING_THE_DIGEST = "Authenticated attribute is missing " 
            + "the digest.";

        public const String AVAILABLE_SPACE_IS_NOT_ENOUGH_FOR_SIGNATURE = "Available space is not enough for " + "signature.";

        public const String TOKEN_ESTIMATION_SIZE_IS_NOT_LARGE_ENOUGH = "Timestamp token estimation size is not " 
            + "large enough to accommodate the entire timestamp token. Timestamp token estimation size is: {0} bytes, "
             + "however real timestamp token size is: {1} bytes.";

        public const String CANNOT_DECODE_PKCS7_SIGNED_DATA_OBJECT = "Cannot decode PKCS#7 SignedData object.";

        public const String CANNOT_FIND_SIGNING_CERTIFICATE_WITH_THIS_SERIAL = "Cannot find signing certificate " 
            + "with serial {0}.";

        public const String CANNOT_BE_VERIFIED_CERTIFICATE_CHAIN = "Cannot be verified against the KeyStore or the "
             + "certificate chain.";

        public const String CERTIFICATION_SIGNATURE_CREATION_FAILED_DOC_SHALL_NOT_CONTAIN_SIGS = "Certification " 
            + "signature creation failed. Document shall not contain any certification or approval signatures before "
             + "signing with certification signature.";

        public const String CERTIFICATE_TEMPLATE_FOR_EXCEPTION_MESSAGE = "Certificate {0} failed: {1}";

        public const String DEFAULT_CLIENTS_CANNOT_BE_CREATED = "Default implementation of OCSP and CRL clients " 
            + "cannot be created, because signing certificate doesn't contain revocation data sources. " + "Please try to explicitly add OCSP or CRL client.";

        public const String DICTIONARY_THIS_KEY_IS_NOT_A_NAME = "Dictionary key {0} is not a name.";

        public const String DIGEST_ALGORITHMS_ARE_NOT_SAME = "Digest algorithm used in the provided " + "IExternalSignature shall be the same as digest algorithm in the provided CMSContainer. "
             + "Digest algorithm in CMS container: \"{0}\". Digest algorithm in IExternalSignature: \"{1}\"";

        public const String DOCUMENT_ALREADY_PRE_CLOSED = "Document has been already pre closed.";

        public const String DOCUMENT_MUST_BE_PRE_CLOSED = "Document must be preClosed.";

        public const String DOCUMENT_MUST_HAVE_READER = "Document must have reader.";

        public const String FAILED_TO_GET_TSA_RESPONSE = "Failed to get TSA response from {0}.";

        public const String FIELD_ALREADY_SIGNED = "Field has been already signed.";

        public const String FIELD_NAMES_CANNOT_CONTAIN_A_DOT = "Field names cannot contain a dot.";

        public const String FIELD_TYPE_IS_NOT_A_SIGNATURE_FIELD_TYPE = "Field type is not a signature field type.";

        public const String INVALID_HTTP_RESPONSE = "Invalid http response {0}.";

        public const String INVALID_STATE_WHILE_CHECKING_CERT_CHAIN = "Invalid state. Possible circular " + "certificate chain.";

        public const String INVALID_TSA_RESPONSE = "Invalid TSA {0} response code {1}.";

        public const String NO_CRYPTO_DICTIONARY_DEFINED = "No crypto dictionary defined.";

        public const String NO_REVOCATION_DATA_FOR_SIGNING_CERTIFICATE = "Neither ocsp nor crl data are available "
             + "for the signing certificate or certificate is revoked.";

        public const String NO_SIGNATURES_TO_PROLONG = "Document doesn't contain any signatures to prolong.";

        public const String NOT_A_VALID_PKCS7_OBJECT_NOT_A_SEQUENCE = "Not a valid PKCS#7 object - not a sequence";

        public const String NOT_A_VALID_PKCS7_OBJECT_NOT_SIGNED_DATA = "Not a valid PKCS#7 object - not signed " +
             "data.";

        public const String NOT_ENOUGH_SPACE = "Not enough space.";

        public const String PATH_IS_NOT_DIRECTORY = "Provided path: {0} is not a directory. " + "Please provide a directory path to store temporary pdf files which are required for signing.";

        public const String PROVIDED_TSA_CLIENT_IS_NULL = "Provided TSA client is null. TSA client is required for timestamp signing.";

        public const String SIGNATURE_WITH_THIS_NAME_IS_NOT_THE_LAST_IT_DOES_NOT_COVER_WHOLE_DOCUMENT = "Signature "
             + "with name {0} is not the last. It doesn't cover the whole document.";

        public const String THE_NAME_OF_THE_DIGEST_ALGORITHM_IS_NULL = "The name of the digest algorithm is null.";

        public const String THERE_IS_NO_FIELD_IN_THE_DOCUMENT_WITH_SUCH_NAME = "There is no field in the document "
             + "with such name: {0}.";

        public const String THIS_PKCS7_OBJECT_HAS_MULTIPLE_SIGNERINFOS_ONLY_ONE_IS_SUPPORTED_AT_THIS_TIME = "This "
             + "PKCS#7 object has multiple SignerInfos. Only one is supported at this time.";

        public const String THIS_INSTANCE_OF_PDF_SIGNER_ALREADY_CLOSED = "This instance of PdfSigner has been " + 
            "already closed.";

        public const String THIS_TSA_FAILED_TO_RETURN_TIME_STAMP_TOKEN = "TSA {0} failed to return time stamp " + 
            "token: {1}.";

        public const String TOO_BIG_KEY = "The key is too big.";

        public const String TSA_CLIENT_IS_MISSING = "ITSAClient must be present to rich this PAdES level. " + "Please use setTSAClient method to provide it.";

        public const String UNEXPECTED_CLOSE_BRACKET = "Unexpected close bracket.";

        public const String UNEXPECTED_GT_GT = "unexpected >>.";

        public const String UNKNOWN_HASH_ALGORITHM = "Unknown hash algorithm: {0}.";

        public const String COULD_NOT_DETERMINE_SIGNATURE_MECHANISM_OID = "Could not determine OID for signature algorithm {0} with digest {1}.";

        public const String VERIFICATION_ALREADY_OUTPUT = "Verification already output.";

        public const String ALGO_REQUIRES_SPECIFIC_HASH = "{0} requires the document to be digested using {1}, not {2}";

        public const String ONLY_MGF1_SUPPORTED_IN_RSASSA_PSS = "Only MGF1 is supported in RSASSA-PSS";

        public const String RSASSA_PSS_DIGESTMISSMATCH = "Digest algorithm in RSASSA-PSS parameters is {0} while" 
            + " ambientdigest algorithm is {1}";

        public const String DISGEST_ALGORITM_MGF_MISMATCH = "Digest algorithm in MGF1 parameters is {0} while" + " ambient digest algorithm is {1}";

        public const String INVALID_ARGUMENTS = "Invalid parameters provided.";

        public const String CMS_SIGNERINFO_READONLY = "Updating the signed attributes of this SignerInfo instance is"
             + " not possible because it has been serialized or been initiated from a serialized version.";

        public const String CMS_SIGNERINFO_NOT_INITIALIZED = "Signer info is not yet initialized";

        public const String CMS_INVALID_CONTAINER_STRUCTURE = "Provided data is not a CMS container";

        public const String CMS_ONLY_ONE_SIGNER_ALLOWED = "Only one signer per CMS container is allowed";

        public const String CMS_CERTIFICATE_NOT_FOUND = "Signer certificate not found in list of certificates";

        public const String CMS_MISSING_CERTIFICATES = "The certificate set must at least contains the signer certificate";

        private SignExceptionMessageConstant() {
        }
        // Private constructor will prevent the instantiation of this class directly
    }
}
