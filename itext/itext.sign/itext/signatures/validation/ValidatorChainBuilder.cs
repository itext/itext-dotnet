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
using System;
using System.Collections.Generic;
using iText.Commons.Actions;
using iText.Commons.Bouncycastle.Cert;
using iText.Kernel.Pdf;
using iText.Signatures;
using iText.Signatures.Validation.Dataorigin;
using iText.Signatures.Validation.Lotl;
using iText.Signatures.Validation.Report.Pades;
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

        private Func<LotlTrustedStore> lotlTrustedStoreFactory;

        private Func<LotlService> lotlServiceFactory;

        private QualifiedValidator qualifiedValidator;

        /// <summary>This set is used to catch recursions while CRL/OCSP responses validation.</summary>
        /// <remarks>
        /// This set is used to catch recursions while CRL/OCSP responses validation.
        /// There might be loops when
        /// Revocation data for cert 0 is signed by cert 0. Or
        /// Revocation data for cert 0 is signed by cert 1 and revocation data for cert 1 is signed by cert 0.
        /// Some more complex loops are possible, and they all are supposed to be caught by this set
        /// and the methods to manipulate this set.
        /// </remarks>
        private ICollection<IX509Certificate> certificatesChainBeingValidated = new HashSet<IX509Certificate>();

        private ICollection<IX509Certificate> trustedCertificates;

        private ICollection<IX509Certificate> knownCertificates;

        private bool trustEuropeanLotl = false;

        private readonly EventManager eventManager;

        private AdESReportAggregator adESReportAggregator = new NullAdESReportAggregator();

        private bool padesValidationRequested = false;

        /// <summary>Creates a ValidatorChainBuilder using default implementations</summary>
        public ValidatorChainBuilder() {
            lotlTrustedStoreFactory = () => BuildLotlTrustedStore();
            certificateRetrieverFactory = () => BuildIssuingCertificateRetriever();
            certificateChainValidatorFactory = () => BuildCertificateChainValidator();
            revocationDataValidatorFactory = () => BuildRevocationDataValidator();
            ocspValidatorFactory = () => BuildOCSPValidator();
            crlValidatorFactory = () => BuildCRLValidator();
            resourceRetrieverFactory = () => new DefaultResourceRetriever();
            documentRevisionsValidatorFactory = () => BuildDocumentRevisionsValidator();
            ocspClientFactory = () => new OcspClientBouncyCastle();
            crlClientFactory = () => new CrlClientOnline();
            lotlServiceFactory = () => BuildLotlService();
            qualifiedValidator = new NullQualifiedValidator();
            eventManager = EventManager.CreateNewInstance();
        }

        /// <summary>Establishes trust in European Union List of Trusted Lists.</summary>
        /// <remarks>
        /// Establishes trust in European Union List of Trusted Lists.
        /// <para />
        /// This feature by default relies on remote resource fetching and third-party EU trusted lists posted online.
        /// iText has no influence over these resources maintained by third-party authorities.
        /// <para />
        /// If this feature is enabled,
        /// <see cref="iText.Signatures.Validation.Lotl.LotlService"/>
        /// is created and used to retrieve,
        /// validate and establish trust in EU List of Trusted Lists.
        /// <para />
        /// In order to properly work, apart from enabling it, user needs to call
        /// <see cref="iText.Signatures.Validation.Lotl.LotlService.InitializeGlobalCache(iText.Signatures.Validation.Lotl.LotlFetchingProperties)
        ///     "/>
        /// method, which performs initial initialization.
        /// <para />
        /// Additionally, in order to successfully use this feature, a user needs to provide a source for trusted
        /// certificates which will be used for LOTL files validation.
        /// One can either add an explicit dependency to "eu-trusted-lists-resources" iText module or configure own source of
        /// trusted certificates. When iText dependency is used it is required to make sure that the newest version of the
        /// dependency is selected, otherwise LOTL validation will fail.
        /// <para />
        /// The required certificates for LOTL files validations are published in the Official Journal of the European Union.
        /// Your own source of trusted certificates can be configured by using
        /// <see cref="EuropeanTrustedListConfigurationFactory.SetFactory(System.Func{T})"/>.
        /// </remarks>
        /// <param name="trustEuropeanLotl">
        /// 
        /// <see langword="true"/>
        /// if European Union LOTLs are expected to be trusted,
        /// <see langword="false"/>
        /// otherwise
        /// </param>
        /// <returns>current ValidatorChainBuilder.</returns>
        public virtual iText.Signatures.Validation.ValidatorChainBuilder TrustEuropeanLotl(bool trustEuropeanLotl) {
            this.trustEuropeanLotl = trustEuropeanLotl;
            return this;
        }

        /// <summary>Checks if European Union List of Trusted Lists is supposed to be trusted.</summary>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if European Union LOTLs are expected to be trusted,
        /// <see langword="false"/>
        /// otherwise
        /// </returns>
        public virtual bool IsEuropeanLotlTrusted() {
            return this.trustEuropeanLotl;
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
        [System.ObsoleteAttribute(@"This method will be removed in a later version, use WithAdESLevelReportGenerator(iText.Signatures.Validation.Report.Xml.XmlReportAggregator) instead."
            )]
        public virtual iText.Signatures.Validation.ValidatorChainBuilder WithAdESReportAggregator(AdESReportAggregator
             adESReportAggregator) {
            this.adESReportAggregator = adESReportAggregator;
            eventManager.Register(new EventsToAdESReportAggratorConvertor(adESReportAggregator));
            return this;
        }

        /// <summary>Use this reportEventListener to generate an AdES xml report.</summary>
        /// <remarks>
        /// Use this reportEventListener to generate an AdES xml report.
        /// <para />
        /// Generated
        /// <see cref="iText.Signatures.Validation.Report.Xml.PadesValidationReport"/>
        /// report could be provided to
        /// <see cref="iText.Signatures.Validation.Report.Xml.XmlReportGenerator.Generate(iText.Signatures.Validation.Report.Xml.PadesValidationReport, System.IO.TextWriter)
        ///     "/>.
        /// </remarks>
        /// <param name="reportEventListener">the AdESReportEventListener to use</param>
        /// <returns>the current ValidatorChainBuilder</returns>
        public virtual iText.Signatures.Validation.ValidatorChainBuilder WithAdESLevelReportGenerator(XmlReportAggregator
             reportEventListener) {
            eventManager.Register(reportEventListener);
            return this;
        }

        /// <summary>Use this PAdES level report generator to generate PAdES report.</summary>
        /// <remarks>
        /// Use this PAdES level report generator to generate PAdES report.
        /// <para />
        /// If called multiple times, multiple
        /// <see cref="iText.Signatures.Validation.Report.Pades.PAdESLevelReportGenerator"/>
        /// objects will be registered.
        /// </remarks>
        /// <param name="reportGenerator">the PAdESLevelReportGenerator to use</param>
        /// <returns>current ValidatorChainBuilder</returns>
        public virtual iText.Signatures.Validation.ValidatorChainBuilder WithPAdESLevelReportGenerator(PAdESLevelReportGenerator
             reportGenerator) {
            padesValidationRequested = true;
            eventManager.Register(reportGenerator);
            return this;
        }

        /// <summary>Checks whether PAdES compliance validation was requested.</summary>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if PAdES compliance validation was requested,
        /// <see langword="false"/>
        /// otherwise
        /// </returns>
        public virtual bool PadesValidationRequested() {
            return padesValidationRequested;
        }

        /// <summary>
        /// Sets
        /// <see cref="iText.Signatures.Validation.Lotl.QualifiedValidator"/>
        /// instance to be used during signature qualification validation.
        /// </summary>
        /// <remarks>
        /// Sets
        /// <see cref="iText.Signatures.Validation.Lotl.QualifiedValidator"/>
        /// instance to be used during signature qualification validation.
        /// The results of this validation can be obtained from this same instance.
        /// The feature is only executed if European LOTL is used. See
        /// <see cref="TrustEuropeanLotl(bool)"/>.
        /// <para />
        /// This validator needs to be updated per each document validation, or the results need to be obtained.
        /// Otherwise, the exception will be thrown.
        /// <para />
        /// If no instance is provided, the qualification validation is not executed.
        /// </remarks>
        /// <param name="qualifiedValidator">
        /// 
        /// <see cref="iText.Signatures.Validation.Lotl.QualifiedValidator"/>
        /// instance which performs the validation.
        /// </param>
        /// <returns>current ValidatorChainBuilder</returns>
        public virtual iText.Signatures.Validation.ValidatorChainBuilder WithQualifiedValidator(QualifiedValidator
             qualifiedValidator) {
            this.qualifiedValidator = qualifiedValidator;
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

        /// <summary>Returns the EventManager to be used for all events fired during validation.</summary>
        /// <returns>the EventManager to be used for all events fired during validation</returns>
        public virtual EventManager GetEventManager() {
            return eventManager;
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
        [System.ObsoleteAttribute(@"The AdESReportAggregator system is replaced by the iText.Signatures.Validation.Report.Xml.XmlReportAggregator system."
            )]
        public virtual AdESReportAggregator GetAdESReportAggregator() {
            return adESReportAggregator;
        }

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

        /// <summary>
        /// Gets
        /// <see cref="iText.Signatures.Validation.Lotl.QualifiedValidator"/>
        /// instance.
        /// </summary>
        /// <returns>
        /// 
        /// <see cref="iText.Signatures.Validation.Lotl.QualifiedValidator"/>
        /// instance
        /// </returns>
        public virtual QualifiedValidator GetQualifiedValidator() {
            return qualifiedValidator;
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

        /// <summary>
        /// Sets up factory which is responsible for
        /// <see cref="iText.Signatures.Validation.Lotl.LotlTrustedStore"/>
        /// creation.
        /// </summary>
        /// <param name="lotlTrustedStoreFactory">
        /// factory responsible for
        /// <see cref="iText.Signatures.Validation.Lotl.LotlTrustedStore"/>
        /// creation
        /// </param>
        /// <returns>
        /// this same instance of
        /// <see cref="ValidatorChainBuilder"/>
        /// </returns>
        public virtual iText.Signatures.Validation.ValidatorChainBuilder WithLotlTrustedStoreFactory(Func<LotlTrustedStore
            > lotlTrustedStoreFactory) {
            this.lotlTrustedStoreFactory = lotlTrustedStoreFactory;
            return this;
        }

        /// <summary>
        /// Retrieves explicitly added or automatically created
        /// <see cref="iText.Signatures.Validation.Lotl.LotlTrustedStore"/>
        /// instance.
        /// </summary>
        /// <returns>
        /// explicitly added or automatically created
        /// <see cref="iText.Signatures.Validation.Lotl.LotlTrustedStore"/>
        /// instance
        /// </returns>
        public virtual LotlTrustedStore GetLotlTrustedStore() {
            return this.lotlTrustedStoreFactory();
        }

        /// <summary>
        /// Sets up factory which is responsible for
        /// <see cref="iText.Signatures.Validation.Lotl.LotlService"/>
        /// creation.
        /// </summary>
        /// <param name="lotlServiceFactory">
        /// factory responsible for
        /// <see cref="iText.Signatures.Validation.Lotl.LotlService"/>
        /// creation
        /// </param>
        /// <returns>
        /// this same instance of
        /// <see cref="ValidatorChainBuilder"/>
        /// </returns>
        public virtual iText.Signatures.Validation.ValidatorChainBuilder WithLotlService(Func<LotlService> lotlServiceFactory
            ) {
            this.lotlServiceFactory = lotlServiceFactory;
            return this;
        }

        /// <summary>
        /// Retrieves explicitly added or automatically created
        /// <see cref="iText.Signatures.Validation.Lotl.LotlService"/>
        /// instance.
        /// </summary>
        /// <returns>
        /// explicitly added or automatically created
        /// <see cref="iText.Signatures.Validation.Lotl.LotlService"/>
        /// instance
        /// </returns>
        public virtual LotlService GetLotlService() {
            return this.lotlServiceFactory();
        }

//\cond DO_NOT_DOCUMENT
        internal virtual void AddCertificateBeingValidated(IX509Certificate certificate) {
            certificatesChainBeingValidated.Add(certificate);
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual void RemoveCertificateBeingValidated(IX509Certificate certificate) {
            certificatesChainBeingValidated.Remove(certificate);
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual bool IsCertificateBeingValidated(IX509Certificate certificate) {
            return certificatesChainBeingValidated.Contains(certificate);
        }
//\endcond

        private static LotlService BuildLotlService() {
            return LotlService.GetGlobalService();
        }

        private IssuingCertificateRetriever BuildIssuingCertificateRetriever() {
            IssuingCertificateRetriever result = new IssuingCertificateRetriever(this.resourceRetrieverFactory());
            if (trustedCertificates != null) {
                result.SetTrustedCertificates(trustedCertificates);
            }
            if (knownCertificates != null) {
                result.AddKnownCertificates(knownCertificates, CertificateOrigin.OTHER);
            }
            result.AddKnownCertificates(lotlTrustedStoreFactory().GetCertificates(), CertificateOrigin.OTHER);
            return result;
        }

        private LotlTrustedStore BuildLotlTrustedStore() {
            return new LotlTrustedStore(this);
        }
    }
}
