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
using System.IO;
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Commons.Bouncycastle.Cert;
using iText.Signatures.Logs;

namespace iText.Signatures {
    /// <summary>
    /// <see cref="IIssuingCertificateRetriever"/>
    /// default implementation.
    /// </summary>
    public class IssuingCertificateRetriever : IIssuingCertificateRetriever {
        private static readonly ILogger LOGGER = ITextLogManager.GetLogger(typeof(iText.Signatures.IssuingCertificateRetriever
            ));

        private readonly IDictionary<String, IX509Certificate> trustedCertificates = new Dictionary<String, IX509Certificate
            >();

        private readonly IDictionary<String, IX509Certificate> knownCertificates = new Dictionary<String, IX509Certificate
            >();

        /// <summary>
        /// Creates
        /// <see cref="IssuingCertificateRetriever"/>
        /// instance.
        /// </summary>
        public IssuingCertificateRetriever() {
        }

        // Empty constructor.
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
            IList<IX509Certificate> fullChain = new List<IX509Certificate>();
            IX509Certificate signingCertificate = (IX509Certificate)chain[0];
            fullChain.Add(signingCertificate);
            int i = 1;
            IX509Certificate lastAddedCert = signingCertificate;
            while (!CertificateUtil.IsSelfSigned(lastAddedCert)) {
                // Check if there are any missing certificates with isSignedByNext
                if (i < chain.Length && CertificateUtil.IsIssuerCertificate(lastAddedCert, (IX509Certificate)chain[i])) {
                    fullChain.Add(chain[i]);
                    i++;
                }
                else {
                    // Get missing certificates using AIA Extensions
                    String url = CertificateUtil.GetIssuerCertURL(lastAddedCert);
                    ICollection<IX509Certificate> certificatesFromAIA = ProcessCertificatesFromAIA(url);
                    if (certificatesFromAIA == null || certificatesFromAIA.IsEmpty()) {
                        // Retrieve Issuer from the certificate store
                        IX509Certificate issuer = trustedCertificates.Get(lastAddedCert.GetIssuerDN().ToString());
                        if (issuer == null) {
                            issuer = knownCertificates.Get(lastAddedCert.GetIssuerDN().ToString());
                            if (issuer == null) {
                                // Unable to retrieve missing certificates
                                while (i < chain.Length) {
                                    fullChain.Add(chain[i]);
                                    i++;
                                }
                                return fullChain.ToArray(new IX509Certificate[0]);
                            }
                        }
                        fullChain.Add(issuer);
                    }
                    else {
                        fullChain.AddAll(certificatesFromAIA);
                    }
                }
                lastAddedCert = (IX509Certificate)fullChain[fullChain.Count - 1];
            }
            return fullChain.ToArray(new IX509Certificate[0]);
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
        public virtual IX509Certificate RetrieveIssuerCertificate(IX509Certificate certificate) {
            IX509Certificate[] certificateChain = RetrieveMissingCertificates(new IX509Certificate[] { certificate });
            if (certificateChain.Length > 1) {
                return certificateChain[1];
            }
            return null;
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
            // Usually CRLs are signed using CA certificate, so we don’t need to do anything extra and the revocation data
            // is already collected. However, it is possible to sign it with any other certificate.
            // IssuingDistributionPoint extension: https://datatracker.ietf.org/doc/html/rfc5280#section-5.2.5
            // Nothing special for the indirect CRLs.
            // AIA Extension
            String url = CertificateUtil.GetIssuerCertURL(crl);
            IList<IX509Certificate> certificatesFromAIA = (IList<IX509Certificate>)ProcessCertificatesFromAIA(url);
            if (certificatesFromAIA == null) {
                // Retrieve Issuer from the certificate store
                IX509Certificate issuer = trustedCertificates.Get(((IX509Crl)crl).GetIssuerDN().ToString());
                if (issuer == null) {
                    issuer = knownCertificates.Get(((IX509Crl)crl).GetIssuerDN().ToString());
                    if (issuer == null) {
                        // Unable to retrieve CRL issuer
                        return new IX509Certificate[0];
                    }
                }
                return RetrieveMissingCertificates(new IX509Certificate[] { issuer });
            }
            return RetrieveMissingCertificates(certificatesFromAIA.ToArray(new IX509Certificate[0]));
        }

        /// <summary><inheritDoc/></summary>
        /// <param name="certificates">
        /// 
        /// <inheritDoc/>
        /// </param>
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
            foreach (IX509Certificate certificate in certificates) {
                trustedCertificates.Put(((IX509Certificate)certificate).GetSubjectDN().ToString(), certificate);
            }
        }

        /// <summary>Add certificates collection to known certificates storage, which is used for issuer certificates retrieval.
        ///     </summary>
        /// <param name="certificates">
        /// certificates
        /// <see cref="System.Collections.ICollection{E}"/>
        /// to be added
        /// </param>
        public virtual void AddKnownCertificates(ICollection<IX509Certificate> certificates) {
            foreach (IX509Certificate certificate in certificates) {
                knownCertificates.Put(((IX509Certificate)certificate).GetSubjectDN().ToString(), certificate);
            }
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
            return trustedCertificates.ContainsKey(((IX509Certificate)certificate).GetSubjectDN().ToString());
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
            return SignUtils.GetHttpResponse(new Uri(uri));
        }

        /// <summary>Parses certificates represented as byte array.</summary>
        /// <param name="certsData">stream which contains one or more X509 certificates.</param>
        /// <returns>a (possibly empty) collection of the certificates read from the given byte array.</returns>
        protected internal virtual ICollection<IX509Certificate> ParseCertificates(Stream certsData) {
            return SignUtils.ReadAllCerts(certsData);
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
    }
}
