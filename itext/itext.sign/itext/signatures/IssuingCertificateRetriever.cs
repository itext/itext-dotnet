/*
This file is part of the iText (R) project.
Copyright (c) 1998-2026 Apryse Group NV
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
using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging;
using iText.Bouncycastleconnector;
using iText.Commons;
using iText.Commons.Bouncycastle;
using iText.Commons.Bouncycastle.Asn1.Ocsp;
using iText.Commons.Bouncycastle.Asn1.X500;
using iText.Commons.Bouncycastle.Asn1.X509;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Utils;
using iText.Commons.Utils.Collections;
using iText.IO.Resolver.Resource;
using iText.Kernel.Crypto;
using iText.Signatures.Logs;
using iText.Signatures.Validation;
using iText.Signatures.Validation.Dataorigin;

namespace iText.Signatures {
    /// <summary>
    /// <see cref="IIssuingCertificateRetriever"/>
    /// default implementation.
    /// </summary>
    public class IssuingCertificateRetriever : IIssuingCertificateRetriever {
        private static readonly IBouncyCastleFactory FACTORY = BouncyCastleFactoryCreator.GetFactory();

        private static readonly ILogger LOGGER = ITextLogManager.GetLogger(typeof(iText.Signatures.IssuingCertificateRetriever
            ));

        private readonly TrustedCertificatesStore trustedCertificatesStore = new TrustedCertificatesStore();

        private readonly IDictionary<String, IList<IX509Certificate>> knownCertificates = new Dictionary<String, IList
            <IX509Certificate>>();

        private readonly IDictionary<IX509Certificate, CertificateOrigin?> certificateOrigins = new Dictionary<IX509Certificate
            , CertificateOrigin?>();

        [Obsolete]
        private readonly iText.StyledXmlParser.Resolver.Resource.IResourceRetriever resourceRetriever;

        private IAdvancedResourceRetriever advancedResourceRetriever;

        /// <summary>
        /// Creates
        /// <see cref="IssuingCertificateRetriever"/>
        /// instance.
        /// </summary>
        public IssuingCertificateRetriever() {
            this.resourceRetriever = null;
            this.advancedResourceRetriever = new DefaultResourceRetriever();
        }

        /// <summary>
        /// Creates
        /// <see cref="IssuingCertificateRetriever"/>
        /// instance.
        /// </summary>
        /// <param name="resourceRetriever">
        /// an @{link com.itextpdf.styledxmlparser.resolver.resource.IResourceRetriever}
        /// instance to use for performing http requests.
        /// </param>
        [System.ObsoleteAttribute(@"in favour of WithResourceRetriever(iText.IO.Resolver.Resource.IAdvancedResourceRetriever)"
            )]
        public IssuingCertificateRetriever(iText.StyledXmlParser.Resolver.Resource.IResourceRetriever resourceRetriever
            ) {
            this.resourceRetriever = resourceRetriever;
        }

        /// <summary><inheritDoc/></summary>
        /// <param name="chain">
        /// 
        /// <inheritDoc/>
        /// </param>
        /// <returns>
        /// 
        /// <inheritDoc/>
        /// </returns>
        public virtual IX509Certificate[] RetrieveMissingCertificates(IX509Certificate[] chain) {
            IList<IList<IX509Certificate>> chains = BuildCertificateChainsList(JavaUtil.ArraysToEnumerable(chain).Select
                ((c) => (IX509Certificate)c).ToArray());
            IList<IX509Certificate> result = new List<IX509Certificate>();
            chains.ForEach((subChain) => {
                JavaCollectionsUtil.Reverse(subChain);
                subChain.ForEach((c) => {
                    if (!result.Contains(c)) {
                        result.Add(c);
                    }
                }
                );
            }
            );
            // We want to add all the originally provided certificates, even if there is no path to them
            foreach (IX509Certificate c in chain) {
                if (!result.Contains(c)) {
                    result.Add((IX509Certificate)c);
                }
            }
            return result.ToArray();
        }

        /// <summary>This method tries to rebuild certificate issuer chain.</summary>
        /// <remarks>
        /// This method tries to rebuild certificate issuer chain. The result contains all possible chains
        /// starting with the given certificate based on issuer names and public keys.
        /// </remarks>
        /// <param name="certificate">
        /// 
        /// <see cref="iText.Commons.Bouncycastle.Cert.IX509Certificate"/>
        /// for which issuer chains shall be built
        /// </param>
        /// <returns>all possible issuer chains</returns>
        public virtual IList<IX509Certificate[]> BuildCertificateChains(IX509Certificate certificate) {
            return BuildCertificateChains(new IX509Certificate[] { certificate });
        }

        /// <summary>This method tries to rebuild certificate issuer chain.</summary>
        /// <remarks>
        /// This method tries to rebuild certificate issuer chain. The result contains all possible chains
        /// starting with the given certificate array based on issuer names and public keys.
        /// </remarks>
        /// <param name="certificate">
        /// 
        /// <see cref="iText.Commons.Bouncycastle.Cert.IX509Certificate"/>
        /// array for which issuer chains shall be built
        /// </param>
        /// <returns>all possible issuer chains</returns>
        public virtual IList<IX509Certificate[]> BuildCertificateChains(IX509Certificate[] certificate) {
            IList<IList<IX509Certificate>> allCertificateChains = BuildCertificateChainsList(certificate);
            IList<IX509Certificate[]> result = new List<IX509Certificate[]>(allCertificateChains.Count * 5);
            foreach (IList<IX509Certificate> chain in allCertificateChains) {
                JavaCollectionsUtil.Reverse(chain);
                result.Add(chain.ToArray(new IX509Certificate[0]));
            }
            return result;
        }

        /// <summary>Sets a resource retriever for this CA issuer certificates retriever.</summary>
        /// <remarks>
        /// Sets a resource retriever for this CA issuer certificates retriever.
        /// <para />
        /// This method allows you to provide a custom implementation of
        /// <see cref="iText.IO.Resolver.Resource.IAdvancedResourceRetriever"/>
        /// to be used
        /// for fetching CA issuer certificates. By default,
        /// <see cref="iText.IO.Resolver.Resource.DefaultResourceRetriever"/>
        /// is used.
        /// </remarks>
        /// <param name="resourceRetriever">the custom resource retriever to be used for fetching CA issuer certificates
        ///     </param>
        /// <returns>
        /// the current instance of
        /// <see cref="IssuingCertificateRetriever"/>
        /// </returns>
        public virtual iText.Signatures.IssuingCertificateRetriever WithResourceRetriever(IAdvancedResourceRetriever
             resourceRetriever) {
            advancedResourceRetriever = resourceRetriever;
            return this;
        }

        /// <summary>Gets the resource retriever currently being used in this CA issuer certificates retriever.</summary>
        /// <returns>resource retriever</returns>
        public virtual IAdvancedResourceRetriever GetResourceRetriever() {
            return advancedResourceRetriever;
        }

        private IList<IList<IX509Certificate>> BuildCertificateChainsList(IX509Certificate[] certificates) {
            AddKnownCertificates(JavaUtil.ArraysAsList(certificates));
            IList<IList<IX509Certificate>> allChains = new List<IList<IX509Certificate>>(BuildCertificateChainsList(certificates
                [0], null, null));
            foreach (IList<IX509Certificate> issuerChain in allChains) {
                for (int i = certificates.Length - 2; i >= 0; --i) {
                    issuerChain.Add(certificates[i]);
                }
            }
            return allChains;
        }

        private IList<IList<IX509Certificate>> BuildCertificateChainsList(IX509Certificate certificate, HashSet<IX509Certificate
            > prevProcessed, HashSet<String> prevProcessedUrls) {
            HashSet<IX509Certificate> processed;
            if (prevProcessed == null) {
                processed = new HashSet<IX509Certificate>();
            }
            else {
                processed = prevProcessed;
            }
            HashSet<String> processedUrls;
            if (prevProcessedUrls == null) {
                processedUrls = new HashSet<String>();
            }
            else {
                processedUrls = prevProcessedUrls;
            }
            if (!processed.Add(certificate)) {
                //certificate is already being processed
                return new List<IList<IX509Certificate>>();
            }
            if (CertificateUtil.IsSelfSigned(certificate)) {
                IList<IList<IX509Certificate>> singleChain = new List<IList<IX509Certificate>>();
                IList<IX509Certificate> chain = new List<IX509Certificate>();
                chain.Add(certificate);
                singleChain.Add(chain);
                return singleChain;
            }
            IList<IList<IX509Certificate>> allChains = new List<IList<IX509Certificate>>();
            // Get missing certificates using AIA Extensions
            String url = CertificateUtil.GetIssuerCertURL(certificate);
            if (processedUrls.Add(url)) {
                ICollection<IX509Certificate> certificatesFromAIA = ProcessCertificatesFromAIA(url);
                if (certificatesFromAIA != null) {
                    AddKnownCertificates(certificatesFromAIA, CertificateOrigin.OTHER);
                }
            }
            ICollection<IX509Certificate> possibleIssuers = trustedCertificatesStore.GetKnownCertificates(certificate.
                GetIssuerDN().ToString());
            if (knownCertificates.Get(certificate.GetIssuerDN().ToString()) != null) {
                possibleIssuers.AddAll(knownCertificates.Get(certificate.GetIssuerDN().ToString()));
            }
            if (possibleIssuers.IsEmpty()) {
                IList<IList<IX509Certificate>> singleChain = new List<IList<IX509Certificate>>();
                IList<IX509Certificate> chain = new List<IX509Certificate>();
                chain.Add(certificate);
                singleChain.Add(chain);
                return singleChain;
            }
            foreach (IX509Certificate possibleIssuer in possibleIssuers) {
                IList<IList<IX509Certificate>> issuerChains = BuildCertificateChainsList((IX509Certificate)possibleIssuer, 
                    processed, processedUrls);
                foreach (IList<IX509Certificate> issuerChain in issuerChains) {
                    issuerChain.Add(certificate);
                    allChains.Add(issuerChain);
                }
            }
            return allChains;
        }

        /// <summary>Retrieve issuer certificate for the provided certificate.</summary>
        /// <param name="certificate">
        /// 
        /// <see cref="iText.Commons.Bouncycastle.Cert.IX509Certificate"/>
        /// for which issuer certificate shall be retrieved
        /// </param>
        /// <returns>
        /// issuer certificate.
        /// <see langword="null"/>
        /// if there is no issuer certificate, or it cannot be retrieved.
        /// </returns>
        public virtual IList<IX509Certificate> RetrieveIssuerCertificate(IX509Certificate certificate) {
            IList<IX509Certificate> result = new List<IX509Certificate>();
            foreach (IX509Certificate[] certificateChain in BuildCertificateChains((IX509Certificate)certificate)) {
                if (certificateChain.Length > 1) {
                    result.Add(certificateChain[1]);
                }
            }
            return result;
        }

        /// <summary>
        /// Retrieves OCSP responder certificate candidates either from the response certs or
        /// trusted store in case responder certificate isn't found in /Certs.
        /// </summary>
        /// <param name="ocspResp">basic OCSP response to get responder certificate for</param>
        /// <returns>retrieved OCSP responder candidates or an empty set in case none were found.</returns>
        public virtual ICollection<IX509Certificate> RetrieveOCSPResponderByNameCertificate(IBasicOcspResponse ocspResp
            ) {
            // Look for the existence of an Authorized OCSP responder inside the cert chain in the ocsp response.
            IEnumerable<IX509Certificate> certs = SignUtils.GetCertsFromOcspResponse(ocspResp);
            IList<IX509Certificate> certList = new List<IX509Certificate>();
            foreach (IX509Certificate certificate in certs) {
                certList.Add(certificate);
            }
            AddKnownCertificates(certList, CertificateOrigin.OCSP_RESPONSE);
            IX500Name x500Name = ocspResp.GetResponderId().ToASN1Primitive().GetName();
            ICollection<IX509Certificate> responders = null;
            if (!x500Name.IsNull()) {
                responders = RetrieveOCSPResponderByName(certs, x500Name);
            }
            if (responders == null) {
                byte[] keyHash = ocspResp.GetResponderId().ToASN1Primitive().GetKeyHash();
                responders = RetrieveOCSPResponderByKeyHash(certs, keyHash);
            }
            // Certificate chain is not present in the response, or it does not contain the responder.
            if (responders == null) {
                if (x500Name.IsNull()) {
                    responders = JavaCollectionsUtil.EmptySet<IX509Certificate>();
                }
                else {
                    responders = trustedCertificatesStore.GetKnownCertificates(FACTORY.CreateX500Name(FACTORY.CreateASN1Sequence
                        (x500Name.ToASN1Primitive())).GetName());
                }
            }
            return responders;
        }

        /// <summary><inheritDoc/></summary>
        /// <param name="crl">
        /// 
        /// <inheritDoc/>
        /// </param>
        /// <returns>
        /// 
        /// <inheritDoc/>
        /// </returns>
        public virtual IX509Certificate[] GetCrlIssuerCertificates(IX509Crl crl) {
            IX509Certificate[][] result = GetCrlIssuerCertificatesGeneric(crl, true);
            if (result.Length == 0) {
                return new IX509Certificate[0];
            }
            return result[0];
        }

        /// <summary><inheritDoc/></summary>
        /// <param name="crl">
        /// 
        /// <inheritDoc/>
        /// </param>
        /// <returns>
        /// 
        /// <inheritDoc/>
        /// </returns>
        public virtual IX509Certificate[][] GetCrlIssuerCertificatesByName(IX509Crl crl) {
            return GetCrlIssuerCertificatesGeneric(crl, false);
        }

        private IX509Certificate[][] GetCrlIssuerCertificatesGeneric(IX509Crl crl, bool verify) {
            // Usually CRLs are signed using CA certificate, so we don’t need to do anything extra and the revocation data
            // is already collected. However, it is possible to sign it with any other certificate.
            // IssuingDistributionPoint extension: https://datatracker.ietf.org/doc/html/rfc5280#section-5.2.5
            // Nothing special for the indirect CRLs.
            // AIA Extension
            List<IX509Certificate[]> matches = new List<IX509Certificate[]>();
            String url = CertificateUtil.GetIssuerCertURL(crl);
            IList<IX509Certificate> certificatesFromAIA = (IList<IX509Certificate>)ProcessCertificatesFromAIA(url);
            if (certificatesFromAIA != null) {
                AddKnownCertificates(certificatesFromAIA, CertificateOrigin.OTHER);
            }
            // Retrieve Issuer from the certificate store
            ICollection<IX509Certificate> issuers = trustedCertificatesStore.GetKnownCertificates(((IX509Crl)crl).GetIssuerDN
                ().ToString());
            if (issuers == null) {
                issuers = new HashSet<IX509Certificate>();
            }
            IList<IX509Certificate> localIssuers = GetCrlIssuersFromKnownCertificates((IX509Crl)crl);
            if (localIssuers != null) {
                issuers.AddAll(localIssuers);
            }
            if (issuers.IsEmpty()) {
                // Unable to retrieve CRL issuer
                return new IX509Certificate[0][];
            }
            foreach (IX509Certificate i in issuers) {
                if (!verify || IsSignedBy((IX509Crl)crl, i)) {
                    matches.AddAll(BuildCertificateChains((IX509Certificate)i));
                }
            }
            return matches.ToArray(new IX509Certificate[][] {  });
        }

        /// <summary>Sets trusted certificate list to be used as certificates trusted for any possible usage.</summary>
        /// <remarks>
        /// Sets trusted certificate list to be used as certificates trusted for any possible usage.
        /// In case more specific trusted is desired to be configured
        /// <see cref="GetTrustedCertificatesStore()"/>
        /// method is expected to be used.
        /// </remarks>
        /// <param name="certificates">certificate list to be used as certificates trusted for any possible usage.</param>
        public virtual void SetTrustedCertificates(ICollection<IX509Certificate> certificates) {
            AddTrustedCertificates(certificates);
        }

        /// <summary>Add trusted certificates collection to trusted certificates storage.</summary>
        /// <param name="certificates">
        /// certificates
        /// <see cref="System.Collections.ICollection{E}"/>
        /// to be added
        /// </param>
        public virtual void AddTrustedCertificates(ICollection<IX509Certificate> certificates) {
            trustedCertificatesStore.AddGenerallyTrustedCertificates(certificates);
        }

        /// <summary>Adds certificates collection to known certificates storage, which is used for issuer certificates retrieval.
        ///     </summary>
        /// <param name="certificates">
        /// certificates
        /// <see cref="System.Collections.ICollection{E}"/>
        /// to be added
        /// </param>
        public virtual void AddKnownCertificates(ICollection<IX509Certificate> certificates) {
            AddKnownCertificates(certificates, CertificateOrigin.OTHER);
        }

        /// <summary>Adds certificates collection to known certificates storage, which is used for issuer certificates retrieval.
        ///     </summary>
        /// <remarks>
        /// Adds certificates collection to known certificates storage, which is used for issuer certificates retrieval.
        /// <para />
        /// Additionally, adds stores the provided origin for all these certificates.
        /// </remarks>
        /// <param name="certificates">
        /// certificates
        /// <see cref="System.Collections.ICollection{E}"/>
        /// to be added
        /// </param>
        /// <param name="dataOrigin">
        /// 
        /// <see cref="iText.Signatures.Validation.Dataorigin.RevocationDataOrigin?"/>
        /// from which these certificates come from
        /// </param>
        public virtual void AddKnownCertificates(ICollection<IX509Certificate> certificates, CertificateOrigin? dataOrigin
            ) {
            foreach (IX509Certificate certificate in certificates) {
                String name = ((IX509Certificate)certificate).GetSubjectDN().ToString();
                IList<IX509Certificate> certs = knownCertificates.ComputeIfAbsent(name, (k) => new List<IX509Certificate>(
                    ));
                certs.Add(certificate);
                CertificateOrigin? currentCertOrigin = certificateOrigins.Get(certificate);
                if (currentCertOrigin == null || (int)(currentCertOrigin) > (int)(dataOrigin)) {
                    certificateOrigins.Put(certificate, dataOrigin);
                }
            }
        }

        /// <summary>
        /// Gets certificate origin for provided
        /// <see cref="iText.Commons.Bouncycastle.Cert.IX509Certificate"/>.
        /// </summary>
        /// <param name="certificate">
        /// 
        /// <see cref="iText.Commons.Bouncycastle.Cert.IX509Certificate"/>
        /// for which origin is requested
        /// </param>
        /// <returns>
        /// 
        /// <see cref="iText.Signatures.Validation.Dataorigin.RevocationDataOrigin?"/>
        /// for the certificate
        /// </returns>
        public virtual CertificateOrigin? GetCertificateOrigin(IX509Certificate certificate) {
            CertificateOrigin? dataOrigin = certificateOrigins.Get(certificate);
            return dataOrigin == null ? CertificateOrigin.OTHER : dataOrigin;
        }

        /// <summary>
        /// Gets
        /// <see cref="iText.Signatures.Validation.TrustedCertificatesStore"/>
        /// to be used to provide more complex trusted certificates configuration.
        /// </summary>
        /// <returns>
        /// 
        /// <see cref="iText.Signatures.Validation.TrustedCertificatesStore"/>
        /// storage
        /// </returns>
        public virtual TrustedCertificatesStore GetTrustedCertificatesStore() {
            return trustedCertificatesStore;
        }

        /// <summary>Check if provided certificate is present in trusted certificates storage.</summary>
        /// <param name="certificate">
        /// 
        /// <see cref="iText.Commons.Bouncycastle.Cert.IX509Certificate"/>
        /// to be checked
        /// </param>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if certificate is present in trusted certificates storage,
        /// <see langword="false"/>
        /// otherwise
        /// </returns>
        public virtual bool IsCertificateTrusted(IX509Certificate certificate) {
            return trustedCertificatesStore.IsCertificateGenerallyTrusted(certificate);
        }

        /// <summary>
        /// Get CA issuers certificates represented as
        /// <see cref="System.IO.Stream"/>.
        /// </summary>
        /// <param name="uri">
        /// 
        /// <see cref="System.Uri"/>
        /// URI, which is expected to be used to get issuer certificates from. Usually
        /// CA Issuers value from Authority Information Access (AIA) certificate extension.
        /// </param>
        /// <returns>
        /// CA issuer certificate (or chain) bytes, represented as
        /// <see cref="System.IO.Stream"/>.
        /// </returns>
        protected internal virtual Stream GetIssuerCertByURI(String uri) {
            if (advancedResourceRetriever == null) {
                return resourceRetriever.GetInputStreamByUrl(new Uri(uri));
            }
            else {
                return advancedResourceRetriever.GetInputStreamByUrl(new Uri(uri));
            }
        }

        /// <summary>Parses certificates represented as byte array.</summary>
        /// <param name="certsData">stream which contains one or more X509 certificates.</param>
        /// <returns>a (possibly empty) collection of the certificates read from the given byte array.</returns>
        protected internal virtual ICollection<IX509Certificate> ParseCertificates(Stream certsData) {
            return SignUtils.ReadAllCerts(certsData);
        }

        private static ICollection<IX509Certificate> RetrieveOCSPResponderByKeyHash(IEnumerable<IX509Certificate> 
            certs, byte[] keyHash) {
            foreach (IX509Certificate cert in certs) {
                try {
                    ISubjectPublicKeyInfo publicKeyInfo = FACTORY.CreateSubjectPublicKeyInfo(cert.GetPublicKey());
                    // According to RFC2560 hash algorithm is always SHA-1.
                    byte[] publicKeyHash = DigestAlgorithms.Digest(new MemoryStream(publicKeyInfo.GetPublicKeyData().GetBytes(
                        )), DigestAlgorithms.GetMessageDigest(DigestAlgorithms.SHA1));
                    if (JavaUtil.ArraysEquals(publicKeyHash, keyHash)) {
                        return JavaCollectionsUtil.Singleton(cert);
                    }
                }
                catch (Exception) {
                }
            }
            // Ignore.
            return null;
        }

        private static ICollection<IX509Certificate> RetrieveOCSPResponderByName(IEnumerable<IX509Certificate> certs
            , IX500Name x500Name) {
            String name = FACTORY.CreateX500Name(FACTORY.CreateASN1Sequence(x500Name.ToASN1Primitive())).GetName();
            foreach (IX509Certificate cert in certs) {
                try {
                    if (name.Equals(cert.GetSubjectDN().ToString())) {
                        return JavaCollectionsUtil.Singleton(cert);
                    }
                }
                catch (Exception) {
                }
            }
            // Ignore.
            return null;
        }

        private ICollection<IX509Certificate> ProcessCertificatesFromAIA(String url) {
            if (url == null) {
                // We don't have any URIs to the issuer certificates in AuthorityInfoAccess extension
                return null;
            }
            try {
                using (Stream missingCertsData = GetIssuerCertByURI(url)) {
                    return ParseCertificates(missingCertsData);
                }
            }
            catch (Exception) {
                LOGGER.LogWarning(SignLogMessageConstant.UNABLE_TO_PARSE_AIA_CERT);
                return null;
            }
        }

        private static bool IsSignedBy(IX509Certificate certificate, IX509Certificate issuer) {
            try {
                certificate.Verify(issuer.GetPublicKey());
                return true;
            }
            catch (Exception) {
                return false;
            }
        }

        private static bool IsSignedBy(IX509Crl crl, IX509Certificate issuer) {
            try {
                crl.Verify(issuer.GetPublicKey());
                return true;
            }
            catch (Exception) {
                return false;
            }
        }

        private static IX509Certificate GetIssuerFromCertificateSet(IX509Certificate lastAddedCert, ICollection<IX509Certificate
            > certs) {
            if (certs != null) {
                foreach (IX509Certificate cert in certs) {
                    if (IsSignedBy(lastAddedCert, cert)) {
                        return cert;
                    }
                }
            }
            return null;
        }

        private IList<IX509Certificate> GetCrlIssuersFromKnownCertificates(IX509Crl crl) {
            return knownCertificates.Get(crl.GetIssuerDN().ToString());
        }
    }
}
