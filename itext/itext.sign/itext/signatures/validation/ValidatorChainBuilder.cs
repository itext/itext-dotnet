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
using iText.StyledXmlParser.Resolver.Resource;

namespace iText.Signatures.Validation {
    /// <summary>A builder class to construct all necessary parts of a validation chain.</summary>
    /// <remarks>
    /// A builder class to construct all necessary parts of a validation chain.
    /// The builder can be reused to create multiple instances of a validator.
    /// </remarks>
    public class ValidatorChainBuilder {
        private SignatureValidationProperties properties = new SignatureValidationProperties();

        private Func<IssuingCertificateRetriever> certificateRetrieverFactory;

        private Func<CertificateChainValidator> certificateChainValidatorFactory;

        private Func<RevocationDataValidator> revocationDataValidatorFactory;

        private Func<OCSPValidator> ocspValidatorFactory;

        private Func<CRLValidator> crlValidatorFactory;

        private Func<IResourceRetriever> resourceRetrieverFactory;

        private Func<DocumentRevisionsValidator> documentRevisionsValidatorFactory;

        private Func<IOcspClientBouncyCastle> ocspClientFactory;

        private Func<ICrlClient> crlClientFactory;

        private ICollection<IX509Certificate> trustedCertificates;

        private ICollection<IX509Certificate> knownCertificates;

        private AdESReportAggregator adESReportAggregator = new NullAdESReportAggregator();

        /// <summary>Creates a ValidatorChainBuilder using default implementations</summary>
        public ValidatorChainBuilder() {
            certificateRetrieverFactory = () => BuildIssuingCertificateRetriever();
            certificateChainValidatorFactory = () => BuildCertificateChainValidator();
            revocationDataValidatorFactory = () => BuildRevocationDataValidator();
            ocspValidatorFactory = () => BuildOCSPValidator();
            crlValidatorFactory = () => BuildCRLValidator();
            resourceRetrieverFactory = () => new DefaultResourceRetriever();
            documentRevisionsValidatorFactory = () => BuildDocumentRevisionsValidator();
            ocspClientFactory = () => new OcspClientBouncyCastle();
            crlClientFactory = () => new CrlClientOnline();
        }

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
        public virtual iText.Signatures.Validation.ValidatorChainBuilder WithDocumentRevisionsValidatorFactory(Func
            <DocumentRevisionsValidator> documentRevisionsValidatorFactory) {
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
        public virtual iText.Signatures.Validation.ValidatorChainBuilder WithCRLValidatorFactory(Func<CRLValidator
            > crlValidatorFactory) {
            this.crlValidatorFactory = crlValidatorFactory;
            return this;
        }

        /// <summary>
        /// Use this factory method to create instances of
        /// <see cref="iText.StyledXmlParser.Resolver.Resource.IResourceRetriever"/>
        /// for use in the validation chain.
        /// </summary>
        /// <param name="resourceRetrieverFactory">the ResourceRetrieverFactory method to use.</param>
        /// <returns>the current ValidatorChainBuilder.</returns>
        public virtual iText.Signatures.Validation.ValidatorChainBuilder WithResourceRetriever(Func<IResourceRetriever
            > resourceRetrieverFactory) {
            this.resourceRetrieverFactory = resourceRetrieverFactory;
            return this;
        }

        /// <summary>
        /// Use this factory method to create instances of
        /// <see cref="OCSPValidator"/>
        /// for use in the validation chain.
        /// </summary>
        /// <param name="ocspValidatorFactory">the OCSPValidatorFactory method to use</param>
        /// <returns>the current ValidatorChainBuilder.</returns>
        public virtual iText.Signatures.Validation.ValidatorChainBuilder WithOCSPValidatorFactory(Func<OCSPValidator
            > ocspValidatorFactory) {
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
        public virtual iText.Signatures.Validation.ValidatorChainBuilder WithRevocationDataValidatorFactory(Func<RevocationDataValidator
            > revocationDataValidatorFactory) {
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
        public virtual iText.Signatures.Validation.ValidatorChainBuilder WithCertificateChainValidatorFactory(Func
            <CertificateChainValidator> certificateChainValidatorFactory) {
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
        public virtual iText.Signatures.Validation.ValidatorChainBuilder WithSignatureValidationProperties(SignatureValidationProperties
             properties) {
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
        public virtual iText.Signatures.Validation.ValidatorChainBuilder WithIssuingCertificateRetrieverFactory(Func
            <IssuingCertificateRetriever> certificateRetrieverFactory) {
            this.certificateRetrieverFactory = certificateRetrieverFactory;
            return this;
        }

        /// <summary>
        /// Use this factory to create instances of
        /// <see cref="iText.Signatures.IOcspClientBouncyCastle"/>
        /// for use in the validation chain.
        /// </summary>
        /// <param name="ocspClientFactory">the IOcspClient factory method to use</param>
        /// <returns>the current ValidatorChainBuilder.</returns>
        public virtual iText.Signatures.Validation.ValidatorChainBuilder WithOcspClient(Func<IOcspClientBouncyCastle
            > ocspClientFactory) {
            this.ocspClientFactory = ocspClientFactory;
            return this;
        }

        /// <summary>
        /// Use this factory to create instances of
        /// <see cref="iText.Signatures.ICrlClient"/>
        /// for use in the validation chain.
        /// </summary>
        /// <param name="crlClientFactory">the ICrlClient factory method to use</param>
        /// <returns>the current ValidatorChainBuilder.</returns>
        public virtual iText.Signatures.Validation.ValidatorChainBuilder WithCrlClient(Func<ICrlClient> crlClientFactory
            ) {
            this.crlClientFactory = crlClientFactory;
            return this;
        }

        /// <summary>
        /// Adds known certificates to the
        /// <see cref="iText.Signatures.IssuingCertificateRetriever"/>.
        /// </summary>
        /// <param name="knownCertificates">the list of known certificates to add</param>
        /// <returns>the current ValidatorChainBuilder.</returns>
        public virtual iText.Signatures.Validation.ValidatorChainBuilder WithKnownCertificates(ICollection<IX509Certificate
            > knownCertificates) {
            this.knownCertificates = new List<IX509Certificate>(knownCertificates);
            return this;
        }

        /// <summary>
        /// Sets the trusted certificates to the
        /// <see cref="iText.Signatures.IssuingCertificateRetriever"/>.
        /// </summary>
        /// <param name="trustedCertificates">the list of trusted certificates to set</param>
        /// <returns>the current ValidatorChainBuilder.</returns>
        public virtual iText.Signatures.Validation.ValidatorChainBuilder WithTrustedCertificates(ICollection<IX509Certificate
            > trustedCertificates) {
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
        public virtual iText.Signatures.Validation.ValidatorChainBuilder WithAdESReportAggregator(AdESReportAggregator
             adESReportAggregator) {
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
            return revocationDataValidatorFactory();
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>
        /// Retrieves the explicitly added or automatically created
        /// <see cref="iText.Signatures.ICrlClient"/>
        /// instance.
        /// </summary>
        /// <returns>
        /// the explicitly added or automatically created
        /// <see cref="iText.Signatures.ICrlClient"/>
        /// instance.
        /// </returns>
        internal virtual ICrlClient GetCrlClient() {
            return crlClientFactory();
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>
        /// Retrieves the explicitly added or automatically created
        /// <see cref="iText.Signatures.IOcspClientBouncyCastle"/>
        /// instance.
        /// </summary>
        /// <returns>
        /// the explicitly added or automatically created
        /// <see cref="iText.Signatures.IOcspClientBouncyCastle"/>
        /// instance.
        /// </returns>
        internal virtual IOcspClientBouncyCastle GetOcspClient() {
            return ocspClientFactory();
        }
//\endcond

        /// <summary>
        /// Retrieves the explicitly added or automatically created
        /// <see cref="iText.StyledXmlParser.Resolver.Resource.IResourceRetriever"/>
        /// instance.
        /// </summary>
        /// <returns>
        /// the explicitly added or automatically created
        /// <see cref="iText.StyledXmlParser.Resolver.Resource.IResourceRetriever"/>
        /// instance.
        /// </returns>
        public virtual IResourceRetriever GetResourceRetriever() {
            return resourceRetrieverFactory();
        }

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
            return ocspValidatorFactory();
        }
//\endcond

        private IssuingCertificateRetriever BuildIssuingCertificateRetriever() {
            IssuingCertificateRetriever result = new IssuingCertificateRetriever(this.resourceRetrieverFactory());
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
