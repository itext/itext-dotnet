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
using System.Collections.Generic;
using iText.Commons.Bouncycastle.Cert;
using iText.Signatures;

namespace iText.Signatures.Validation.V1 {
    /// <summary>
    /// A builder class to construct all necessary parts of a validation chain
    /// The builder can be reused to create multiple instances of a validator
    /// </summary>
    public class ValidatorChainBuilder {
        private SignatureValidationProperties properties;

        private IssuingCertificateRetriever certificateRetriever;

        private CertificateChainValidator certificateChainValidator;

        private RevocationDataValidator revocationDataValidator;

        private OCSPValidator ocspValidator;

        private CRLValidator crlValidator;

        private DocumentRevisionsValidator documentRevisionsValidator;

        /// <summary>
        /// Create a new
        /// <see cref="SignatureValidator"/>
        /// instance with the current configuration.
        /// </summary>
        /// <remarks>
        /// Create a new
        /// <see cref="SignatureValidator"/>
        /// instance with the current configuration.
        /// This method can be used to create multiple validators.
        /// </remarks>
        /// <returns>a new instance of a signature validator</returns>
        internal virtual SignatureValidator BuildSignatureValidator() {
            return new SignatureValidator(this);
        }

        /// <summary>
        /// Create a bew
        /// <see cref="DocumentRevisionsValidator"/>
        /// instance with the current configuration.
        /// </summary>
        /// <remarks>
        /// Create a bew
        /// <see cref="DocumentRevisionsValidator"/>
        /// instance with the current configuration.
        /// This method can be used to create multiple validators.
        /// </remarks>
        /// <returns>a new instance of a document revisions validator</returns>
        public virtual DocumentRevisionsValidator BuildDocumentRevisionsValidator() {
            return new DocumentRevisionsValidator(this);
        }

        /// <summary>
        /// Create a new
        /// <see cref="CertificateChainValidator"/>
        /// instance.
        /// </summary>
        /// <remarks>
        /// Create a new
        /// <see cref="CertificateChainValidator"/>
        /// instance.
        /// This method can be used to create multiple validators.
        /// </remarks>
        /// <returns>a new instance of a CertificateChainValidator</returns>
        public virtual CertificateChainValidator BuildCertificateChainValidator() {
            return new CertificateChainValidator(this);
        }

        /// <summary>
        /// Create a new
        /// <see cref="RevocationDataValidator"/>
        /// instance
        /// This method can be used to create multiple validators.
        /// </summary>
        /// <returns>a new instance of a RevocationDataValidator</returns>
        public virtual RevocationDataValidator BuildRevocationDataValidator() {
            return new RevocationDataValidator(this);
        }

        /// <summary>
        /// Create a new
        /// <see cref="OCSPValidator"/>
        /// instance.
        /// </summary>
        /// <remarks>
        /// Create a new
        /// <see cref="OCSPValidator"/>
        /// instance.
        /// This method can be used to create multiple validators.
        /// </remarks>
        /// <returns>a new instance of a OCSPValidator</returns>
        public virtual OCSPValidator BuildOCSPValidator() {
            return new OCSPValidator(this);
        }

        /// <summary>
        /// Create a new
        /// <see cref="CRLValidator"/>
        /// instance.
        /// </summary>
        /// <remarks>
        /// Create a new
        /// <see cref="CRLValidator"/>
        /// instance.
        /// This method can be used to create multiple validators.
        /// </remarks>
        /// <returns>a new instance of a CRLValidator</returns>
        public virtual CRLValidator BuildCRLValidator() {
            return new CRLValidator(this);
        }

        /// <summary>
        /// Use this instance of a
        /// <see cref="DocumentRevisionsValidator"/>
        /// in the validation chain.
        /// </summary>
        /// <param name="documentRevisionsValidator">the document revisions validator instance to use</param>
        /// <returns>the current ValidatorChainBuilder</returns>
        public virtual ValidatorChainBuilder WithDocumentRevisionsValidator(DocumentRevisionsValidator documentRevisionsValidator
            ) {
            this.documentRevisionsValidator = documentRevisionsValidator;
            return this;
        }

        /// <summary>
        /// Use this instance of a
        /// <see cref="CRLValidator"/>
        /// in the validation chain.
        /// </summary>
        /// <param name="crlValidator">the CRLValidator instance to use</param>
        /// <returns>the current ValidatorChainBuilder</returns>
        public virtual ValidatorChainBuilder WithCRLValidator(CRLValidator crlValidator) {
            this.crlValidator = crlValidator;
            return this;
        }

        /// <summary>
        /// Use this instance of a
        /// <see cref="OCSPValidator"/>
        /// in the validation chain.
        /// </summary>
        /// <param name="ocspValidator">the OCSPValidator instance to use</param>
        /// <returns>the current ValidatorChainBuilder</returns>
        public virtual ValidatorChainBuilder WithOCSPValidator(OCSPValidator ocspValidator) {
            this.ocspValidator = ocspValidator;
            return this;
        }

        /// <summary>
        /// Use this instance of a
        /// <see cref="RevocationDataValidator"/>
        /// in the validation chain.
        /// </summary>
        /// <param name="revocationDataValidator">the RevocationDataValidator instance to use</param>
        /// <returns>the current ValidatorChainBuilder</returns>
        public virtual ValidatorChainBuilder WithRevocationDataValidator(RevocationDataValidator revocationDataValidator
            ) {
            this.revocationDataValidator = revocationDataValidator;
            return this;
        }

        /// <summary>
        /// Use this instance of a
        /// <see cref="CertificateChainValidator"/>
        /// in the validation chain.
        /// </summary>
        /// <param name="certificateChainValidator">the CertificateChainValidator instance to use</param>
        /// <returns>the current ValidatorChainBuilder</returns>
        public virtual ValidatorChainBuilder WithCertificateChainValidator(CertificateChainValidator certificateChainValidator
            ) {
            this.certificateChainValidator = certificateChainValidator;
            return this;
        }

        /// <summary>
        /// Use this instance of a
        /// <see cref="SignatureValidationProperties"/>
        /// in the validation chain.
        /// </summary>
        /// <param name="properties">the SignatureValidationProperties instance to use</param>
        /// <returns>the current ValidatorChainBuilder</returns>
        public virtual ValidatorChainBuilder WithSignatureValidationProperties(SignatureValidationProperties properties
            ) {
            this.properties = properties;
            return this;
        }

        /// <summary>
        /// Use this instance of a
        /// <see cref="iText.Signatures.IssuingCertificateRetriever"/>
        /// in the validation chain.
        /// </summary>
        /// <param name="certificateRetriever">the IssuingCertificateRetriever instance to use</param>
        /// <returns>the current ValidatorChainBuilder</returns>
        public virtual ValidatorChainBuilder WithIssuingCertificateRetriever(IssuingCertificateRetriever certificateRetriever
            ) {
            this.certificateRetriever = certificateRetriever;
            return this;
        }

        /// <summary>
        /// Adds known certificates to the
        /// <see cref="iText.Signatures.IssuingCertificateRetriever"/>.
        /// </summary>
        /// <param name="knownCertificates">the list of known certificates to add</param>
        /// <returns>the current ValidatorChainBuilder</returns>
        public virtual ValidatorChainBuilder WithKnownCertificates(ICollection<IX509Certificate> knownCertificates
            ) {
            GetCertificateRetriever().AddKnownCertificates(knownCertificates);
            return this;
        }

        /// <summary>
        /// Sets the trusted certificates to the
        /// <see cref="iText.Signatures.IssuingCertificateRetriever"/>.
        /// </summary>
        /// <param name="trustedCertificates">the list of trusted certificates to set</param>
        /// <returns>the current ValidatorChainBuilder</returns>
        public virtual ValidatorChainBuilder WithTrustedCertificates(ICollection<IX509Certificate> trustedCertificates
            ) {
            GetCertificateRetriever().SetTrustedCertificates(trustedCertificates);
            return this;
        }

        /// <summary>
        /// Retrieves the explicitly added or automatically created
        /// <see cref="DocumentRevisionsValidator"/>
        /// instance.
        /// </summary>
        /// <returns>
        /// the explicitly added or automatically created
        /// <see cref="DocumentRevisionsValidator"/>
        /// instance
        /// </returns>
        internal virtual DocumentRevisionsValidator GetDocumentRevisionsValidator() {
            if (documentRevisionsValidator == null) {
                documentRevisionsValidator = BuildDocumentRevisionsValidator();
            }
            return documentRevisionsValidator;
        }

        /// <summary>
        /// Retrieves the explicitly added or automatically created
        /// <see cref="CertificateChainValidator"/>
        /// instance.
        /// </summary>
        /// <returns>
        /// the explicitly added or automatically created
        /// <see cref="CertificateChainValidator"/>
        /// instance
        /// </returns>
        internal virtual CertificateChainValidator GetCertificateChainValidator() {
            if (certificateChainValidator == null) {
                certificateChainValidator = BuildCertificateChainValidator();
            }
            return certificateChainValidator;
        }

        /// <summary>
        /// Retrieves the explicitly added or automatically created
        /// <see cref="RevocationDataValidator"/>
        /// instance.
        /// </summary>
        /// <returns>
        /// the explicitly added or automatically created
        /// <see cref="RevocationDataValidator"/>
        /// instance
        /// </returns>
        internal virtual RevocationDataValidator GetRevocationDataValidator() {
            if (revocationDataValidator == null) {
                revocationDataValidator = BuildRevocationDataValidator();
            }
            return revocationDataValidator;
        }

        /// <summary>
        /// Retrieves the explicitly added or automatically created
        /// <see cref="CRLValidator"/>
        /// instance.
        /// </summary>
        /// <returns>
        /// the explicitly added or automatically created
        /// <see cref="CRLValidator"/>
        /// instance
        /// </returns>
        internal virtual CRLValidator GetCRLValidator() {
            if (crlValidator == null) {
                crlValidator = BuildCRLValidator();
            }
            return crlValidator;
        }

        /// <summary>
        /// Retrieves the explicitly added or automatically created
        /// <see cref="OCSPValidator"/>
        /// instance.
        /// </summary>
        /// <returns>
        /// the explicitly added or automatically created
        /// <see cref="OCSPValidator"/>
        /// instance
        /// </returns>
        internal virtual OCSPValidator GetOCSPValidator() {
            if (ocspValidator == null) {
                ocspValidator = BuildOCSPValidator();
            }
            return ocspValidator;
        }

        /// <summary>
        /// Retrieves the explicitly added or automatically created
        /// <see cref="iText.Signatures.IssuingCertificateRetriever"/>
        /// instance.
        /// </summary>
        /// <returns>
        /// the explicitly added or automatically created
        /// <see cref="iText.Signatures.IssuingCertificateRetriever"/>
        /// instance
        /// </returns>
        public virtual IssuingCertificateRetriever GetCertificateRetriever() {
            if (certificateRetriever == null) {
                certificateRetriever = new IssuingCertificateRetriever();
            }
            return certificateRetriever;
        }

        /// <summary>
        /// Retrieves the explicitly added or automatically created
        /// <see cref="SignatureValidationProperties"/>
        /// instance.
        /// </summary>
        /// <returns>
        /// the explicitly added or automatically created
        /// <see cref="SignatureValidationProperties"/>
        /// instance
        /// </returns>
        public virtual SignatureValidationProperties GetProperties() {
            if (properties == null) {
                properties = new SignatureValidationProperties();
            }
            return properties;
        }
    }
}
