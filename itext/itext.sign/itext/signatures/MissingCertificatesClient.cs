/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 Apryse Group NV
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
    /// <see cref="IMissingCertificatesClient"/>
    /// default implementation.
    /// </summary>
    public class MissingCertificatesClient : IMissingCertificatesClient {
        private static readonly ILogger LOGGER = ITextLogManager.GetLogger(typeof(iText.Signatures.MissingCertificatesClient
            ));

        /// <summary>
        /// Creates
        /// <see cref="MissingCertificatesClient"/>
        /// instance.
        /// </summary>
        public MissingCertificatesClient() {
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
            while (!CertificateUtil.IsSelfSigned((IX509Certificate)lastAddedCert)) {
                //  Check if there are any missing certificates with isSignedByNext
                if (i < chain.Length && CertificateUtil.IsIssuerCertificate((IX509Certificate)lastAddedCert, (IX509Certificate
                    )chain[i])) {
                    fullChain.Add(chain[i]);
                    i++;
                }
                else {
                    // Get missing certificates using AIA Extensions
                    ICollection<IX509Certificate> certificatesFromAIA = ProcessCertificatesFromAIA(lastAddedCert);
                    if (certificatesFromAIA == null || certificatesFromAIA.IsEmpty()) {
                        // Unable to retrieve missing certificates
                        while (i < chain.Length) {
                            fullChain.Add(chain[i]);
                            i++;
                        }
                        return fullChain.ToArray(new IX509Certificate[0]);
                    }
                    fullChain.AddAll(certificatesFromAIA);
                }
                lastAddedCert = fullChain[fullChain.Count - 1];
            }
            return fullChain.ToArray(new IX509Certificate[0]);
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

        private ICollection<IX509Certificate> ProcessCertificatesFromAIA(IX509Certificate certificate) {
            String url = CertificateUtil.GetIssuerCertURL((IX509Certificate)certificate);
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
