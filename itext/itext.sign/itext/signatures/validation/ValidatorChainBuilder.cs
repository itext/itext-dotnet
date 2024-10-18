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
using System.Collections.Generic;
using iText.Commons.Bouncycastle.Cert;
using iText.Kernel.Pdf;
using iText.Signatures;
using iText.Signatures.Validation.Report.Xml;

namespace iText.Signatures.Validation {
    /// <summary>A builder class to construct all necessary parts of a validation chain.</summary>
    /// <remarks>
    /// A builder class to construct all necessary parts of a validation chain.
    /// The builder can be reused to create multiple instances of a validator.
    /// </remarks>
    public class ValidatorChainBuilder {
        private SignatureValidationProperties properties;

        private Func<IssuingCertificateRetriever> certificateRetrieverFactory;

        private Func<CertificateChainValidator> certificateChainValidatorFactory;

        private Func<RevocationDataValidator> revocationDataValidatorFactory;

        private Func<OCSPValidator> ocspValidatorFactory;

        private Func<CRLValidator> crlValidatorFactory;

        private Func<DocumentRevisionsValidator> documentRevisionsValidatorFactory;

        private ICollection<IX509Certificate> trustedCertificates;

        private ICollection<IX509Certificate> knownCertificates;

        private AdESReportAggregator adESReportAggregator = new NullAdESReportAggregator();

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
        /// <param name="document">
        /// 
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// instance which will be validated
        /// </param>
        /// <returns>a new instance of a signature validator.</returns>
        public virtual SignatureValidator BuildSignatureValidator(PdfDocument document) {
            return new SignatureValidator(document, this);
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
        /// <returns>a new instance of a document revisions validator.</returns>
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
        /// <returns>a new instance of a CertificateChainValidator.</returns>
        public virtual CertificateChainValidator BuildCertificateChainValidator() {
            return new CertificateChainValidator(this);
        }

        /// <summary>
        /// Create a new
        /// <see cref="RevocationDataValidator"/>
        /// instance
        /// This method can be used to create multiple validators.
        /// </summary>
        /// <returns>a new instance of a RevocationDataValidator.</returns>
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
        /// <returns>a new instance of a OCSPValidator.</returns>
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
        /// <returns>a new instance of a CRLValidator.</returns>
        public virtual CRLValidator BuildCRLValidator() {
            return new CRLValidator(this);
        }

        /// <summary>
        /// Use this factory method to create instances of
        /// <see cref="DocumentRevisionsValidator"/>
        /// for use in the validation chain.
        /// </summary>
        /// <param name="documentRevisionsValidatorFactory">the document revisions validator factory method to use</param>
        /// <returns>the current ValidatorChainBuilder.</returns>
        public virtual ValidatorChainBuilder WithDocumentRevisionsValidatorFactory(Func<DocumentRevisionsValidator
            > documentRevisionsValidatorFactory) {
            this.documentRevisionsValidatorFactory = documentRevisionsValidatorFactory;
            return this;
        }

        /// <summary>
        /// Use this factory method to create instances of
        /// <see cref="CRLValidator"/>
        /// for use in the validation chain.
        /// </summary>
        /// <param name="crlValidatorFactory">the CRLValidatorFactory method to use</param>
        /// <returns>the current ValidatorChainBuilder.</returns>
        public virtual ValidatorChainBuilder WithCRLValidatorFactory(Func<CRLValidator> crlValidatorFactory) {
            this.crlValidatorFactory = crlValidatorFactory;
            return this;
        }

        /// <summary>
        /// Use this factory method to create instances of
        /// <see cref="OCSPValidator"/>
        /// for use in the validation chain.
        /// </summary>
        /// <param name="ocspValidatorFactory">the OCSPValidatorFactory method to use</param>
        /// <returns>the current ValidatorChainBuilder.</returns>
        public virtual ValidatorChainBuilder WithOCSPValidatorFactory(Func<OCSPValidator> ocspValidatorFactory) {
            this.ocspValidatorFactory = ocspValidatorFactory;
            return this;
        }

        /// <summary>
        /// Use this factory method to create instances of
        /// <see cref="RevocationDataValidator"/>
        /// for use in the validation chain.
        /// </summary>
        /// <param name="revocationDataValidatorFactory">the RevocationDataValidator factory method to use</param>
        /// <returns>the current ValidatorChainBuilder.</returns>
        public virtual ValidatorChainBuilder WithRevocationDataValidatorFactory(Func<RevocationDataValidator> revocationDataValidatorFactory
            ) {
            this.revocationDataValidatorFactory = revocationDataValidatorFactory;
            return this;
        }

        /// <summary>
        /// Use this factory method to create instances of
        /// <see cref="CertificateChainValidator"/>
        /// for use in the validation chain.
        /// </summary>
        /// <param name="certificateChainValidatorFactory">the CertificateChainValidator factory method to use</param>
        /// <returns>the current ValidatorChainBuilder.</returns>
        public virtual ValidatorChainBuilder WithCertificateChainValidatorFactory(Func<CertificateChainValidator> 
            certificateChainValidatorFactory) {
            this.certificateChainValidatorFactory = certificateChainValidatorFactory;
            return this;
        }

        /// <summary>
        /// Use this instance of a
        /// <see cref="SignatureValidationProperties"/>
        /// in the validation chain.
        /// </summary>
        /// <param name="properties">the SignatureValidationProperties instance to use</param>
        /// <returns>the current ValidatorChainBuilder.</returns>
        public virtual ValidatorChainBuilder WithSignatureValidationProperties(SignatureValidationProperties properties
            ) {
            this.properties = properties;
            return this;
        }

        /// <summary>
        /// Use this factory method to create instances of
        /// <see cref="iText.Signatures.IssuingCertificateRetriever"/>
        /// for use in the validation chain.
        /// </summary>
        /// <param name="certificateRetrieverFactory">the IssuingCertificateRetriever factory method to use</param>
        /// <returns>the current ValidatorChainBuilder.</returns>
        public virtual ValidatorChainBuilder WithIssuingCertificateRetrieverFactory(Func<IssuingCertificateRetriever
            > certificateRetrieverFactory) {
            this.certificateRetrieverFactory = certificateRetrieverFactory;
            return this;
        }

        /// <summary>
        /// Adds known certificates to the
        /// <see cref="iText.Signatures.IssuingCertificateRetriever"/>.
        /// </summary>
        /// <param name="knownCertificates">the list of known certificates to add</param>
        /// <returns>the current ValidatorChainBuilder.</returns>
        public virtual ValidatorChainBuilder WithKnownCertificates(ICollection<IX509Certificate> knownCertificates
            ) {
            this.knownCertificates = new List<IX509Certificate>(knownCertificates);
            return this;
        }

        /// <summary>
        /// Sets the trusted certificates to the
        /// <see cref="iText.Signatures.IssuingCertificateRetriever"/>.
        /// </summary>
        /// <param name="trustedCertificates">the list of trusted certificates to set</param>
        /// <returns>the current ValidatorChainBuilder.</returns>
        public virtual ValidatorChainBuilder WithTrustedCertificates(ICollection<IX509Certificate> trustedCertificates
            ) {
            this.trustedCertificates = new List<IX509Certificate>(trustedCertificates);
            return this;
        }

        /// <summary>Use this AdES report aggregator to enable AdES compliant report generation.</summary>
        /// <remarks>
        /// Use this AdES report aggregator to enable AdES compliant report generation.
        /// <para />
        /// Generated
        /// <see cref="iText.Signatures.Validation.Report.Xml.PadesValidationReport"/>
        /// report could be provided to
        /// <see cref="iText.Signatures.Validation.Report.Xml.XmlReportGenerator.Generate(iText.Signatures.Validation.Report.Xml.PadesValidationReport, System.IO.TextWriter)
        ///     "/>.
        /// </remarks>
        /// <param name="adESReportAggregator">the report aggregator to use</param>
        /// <returns>the current ValidatorChainBuilder</returns>
        public virtual ValidatorChainBuilder WithAdESReportAggregator(AdESReportAggregator adESReportAggregator) {
            this.adESReportAggregator = adESReportAggregator;
            return this;
        }

        /// <summary>
        /// Retrieves the explicitly added or automatically created
        /// <see cref="iText.Signatures.IssuingCertificateRetriever"/>
        /// instance.
        /// </summary>
        /// <returns>
        /// the explicitly added or automatically created
        /// <see cref="iText.Signatures.IssuingCertificateRetriever"/>
        /// instance.
        /// </returns>
        public virtual IssuingCertificateRetriever GetCertificateRetriever() {
            if (certificateRetrieverFactory == null) {
                return BuildIssuingCertificateRetriever();
            }
            return certificateRetrieverFactory();
        }

        /// <summary>
        /// Retrieves the explicitly added or automatically created
        /// <see cref="SignatureValidationProperties"/>
        /// instance.
        /// </summary>
        /// <returns>
        /// the explicitly added or automatically created
        /// <see cref="SignatureValidationProperties"/>
        /// instance.
        /// </returns>
        public virtual SignatureValidationProperties GetProperties() {
            if (properties == null) {
                properties = new SignatureValidationProperties();
            }
            return properties;
        }

        /// <summary>
        /// Retrieves the explicitly added or automatically created
        /// <see cref="iText.Signatures.Validation.Report.Xml.AdESReportAggregator"/>
        /// instance.
        /// </summary>
        /// <remarks>
        /// Retrieves the explicitly added or automatically created
        /// <see cref="iText.Signatures.Validation.Report.Xml.AdESReportAggregator"/>
        /// instance.
        /// Default is the
        /// <see cref="iText.Signatures.Validation.Report.Xml.NullAdESReportAggregator"/>.
        /// </remarks>
        /// <returns>
        /// the explicitly added or automatically created
        /// <see cref="iText.Signatures.Validation.Report.Xml.AdESReportAggregator"/>
        /// instance.
        /// </returns>
        public virtual AdESReportAggregator GetAdESReportAggregator() {
            return adESReportAggregator;
        }

//\cond DO_NOT_DOCUMENT
        /// <summary>
        /// Retrieves the explicitly added or automatically created
        /// <see cref="DocumentRevisionsValidator"/>
        /// instance.
        /// </summary>
        /// <returns>
        /// the explicitly added or automatically created
        /// <see cref="DocumentRevisionsValidator"/>
        /// instance.
        /// </returns>
        internal virtual DocumentRevisionsValidator GetDocumentRevisionsValidator() {
            if (documentRevisionsValidatorFactory == null) {
                return BuildDocumentRevisionsValidator();
            }
            return documentRevisionsValidatorFactory();
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>
        /// Retrieves the explicitly added or automatically created
        /// <see cref="CertificateChainValidator"/>
        /// instance.
        /// </summary>
        /// <returns>
        /// the explicitly added or automatically created
        /// <see cref="CertificateChainValidator"/>
        /// instance.
        /// </returns>
        internal virtual CertificateChainValidator GetCertificateChainValidator() {
            if (certificateChainValidatorFactory == null) {
                return BuildCertificateChainValidator();
            }
            return certificateChainValidatorFactory();
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>
        /// Retrieves the explicitly added or automatically created
        /// <see cref="RevocationDataValidator"/>
        /// instance.
        /// </summary>
        /// <returns>
        /// the explicitly added or automatically created
        /// <see cref="RevocationDataValidator"/>
        /// instance.
        /// </returns>
        internal virtual RevocationDataValidator GetRevocationDataValidator() {
            if (revocationDataValidatorFactory == null) {
                return BuildRevocationDataValidator();
            }
            return revocationDataValidatorFactory();
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>
        /// Retrieves the explicitly added or automatically created
        /// <see cref="CRLValidator"/>
        /// instance.
        /// </summary>
        /// <returns>
        /// the explicitly added or automatically created
        /// <see cref="CRLValidator"/>
        /// instance.
        /// </returns>
        internal virtual CRLValidator GetCRLValidator() {
            if (crlValidatorFactory == null) {
                return BuildCRLValidator();
            }
            return crlValidatorFactory();
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>
        /// Retrieves the explicitly added or automatically created
        /// <see cref="OCSPValidator"/>
        /// instance.
        /// </summary>
        /// <returns>
        /// the explicitly added or automatically created
        /// <see cref="OCSPValidator"/>
        /// instance.
        /// </returns>
        internal virtual OCSPValidator GetOCSPValidator() {
            if (ocspValidatorFactory == null) {
                return BuildOCSPValidator();
            }
            return ocspValidatorFactory();
        }
//\endcond

        private IssuingCertificateRetriever BuildIssuingCertificateRetriever() {
            IssuingCertificateRetriever result = new IssuingCertificateRetriever();
            if (trustedCertificates != null) {
                result.SetTrustedCertificates(trustedCertificates);
            }
            if (knownCertificates != null) {
                result.AddKnownCertificates(knownCertificates);
            }
            return result;
        }
    }
}
