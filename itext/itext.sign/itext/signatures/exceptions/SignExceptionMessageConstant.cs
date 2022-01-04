/*
This file is part of the iText (R) project.
Copyright (c) 1998-2022 iText Group NV
Authors: iText Software.

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
        public const String AUTHENTICATED_ATTRIBUTE_IS_MISSING_THE_DIGEST = "Authenticated attribute is missing " 
            + "the digest.";

        public const String AVAILABLE_SPACE_IS_NOT_ENOUGH_FOR_SIGNATURE = "Available space is not enough for " + "signature.";

        public const String CANNOT_DECODE_PKCS7_SIGNED_DATA_OBJECT = "Cannot decode PKCS#7 SignedData object.";

        public const String CANNOT_FIND_SIGNING_CERTIFICATE_WITH_THIS_SERIAL = "Cannot find signing certificate " 
            + "with serial {0}.";

        public const String CANNOT_BE_VERIFIED_CERTIFICATE_CHAIN = "Cannot be verified against the KeyStore or the "
             + "certificate chain.";

        public const String CERTIFICATION_SIGNATURE_CREATION_FAILED_DOC_SHALL_NOT_CONTAIN_SIGS = "Certification " 
            + "signature creation failed. Document shall not contain any certification or approval signatures before "
             + "signing with certification signature.";

        public const String CERTIFICATE_TEMPLATE_FOR_EXCEPTION_MESSAGE = "Certificate {0} failed: {1}";

        public const String DICTIONARY_THIS_KEY_IS_NOT_A_NAME = "Dictionary key {0} is not a name.";

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

        public const String NOT_A_VALID_PKCS7_OBJECT_NOT_A_SEQUENCE = "Not a valid PKCS#7 object - not a sequence";

        public const String NOT_A_VALID_PKCS7_OBJECT_NOT_SIGNED_DATA = "Not a valid PKCS#7 object - not signed " +
             "data.";

        public const String NOT_ENOUGH_SPACE = "Not enough space.";

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

        public const String UNEXPECTED_CLOSE_BRACKET = "Unexpected close bracket.";

        public const String UNEXPECTED_GT_GT = "unexpected >>.";

        public const String UNKNOWN_HASH_ALGORITHM = "Unknown hash algorithm: {0}.";

        public const String UNKNOWN_KEY_ALGORITHM = "Unknown key algorithm: {0}.";

        public const String VERIFICATION_ALREADY_OUTPUT = "Verification already output.";

        private SignExceptionMessageConstant() {
        }
        // Private constructor will prevent the instantiation of this class directly
    }
}
