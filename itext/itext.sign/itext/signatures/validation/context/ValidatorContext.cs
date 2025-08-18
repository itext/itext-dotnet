/*
This file is part of the iText (R) project.
Copyright (c) 1998-2025 Apryse Group NV
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
using iText.Signatures.Validation;

namespace iText.Signatures.Validation.Context {
    /// <summary>This enum lists all possible contexts related to the validator in which the validation is taking place.
    ///     </summary>
    public enum ValidatorContext {
        /// <summary>
        /// This value is expected to be used in
        /// <see cref="OCSPValidator"/>
        /// context.
        /// </summary>
        OCSP_VALIDATOR,
        /// <summary>
        /// This value is expected to be used in
        /// <see cref="CRLValidator"/>
        /// context.
        /// </summary>
        CRL_VALIDATOR,
        /// <summary>
        /// This value is expected to be used in
        /// <see cref="RevocationDataValidator"/>
        /// context.
        /// </summary>
        REVOCATION_DATA_VALIDATOR,
        /// <summary>
        /// This value is expected to be used in
        /// <see cref="CertificateChainValidator"/>
        /// context.
        /// </summary>
        CERTIFICATE_CHAIN_VALIDATOR,
        /// <summary>
        /// This value is expected to be used in
        /// <see cref="SignatureValidator"/>
        /// context.
        /// </summary>
        SIGNATURE_VALIDATOR,
        /// <summary>
        /// This value is expected to be used in
        /// <see cref="DocumentRevisionsValidator"/>
        /// context.
        /// </summary>
        DOCUMENT_REVISIONS_VALIDATOR,
        /// <summary>This value is expected to be used in XmlSignatureValidator context.</summary>
        XML_SIGNATURE_VALIDATOR
    }
}
