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
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Security;
using iText.IO.Util;

namespace iText.Signatures {
    /// <summary>
    /// Class that allows you to verify a certificate against
    /// one or more Certificate Revocation Lists.
    /// </summary>
    [System.ObsoleteAttribute(@"starting from 8.0.5.iText.Signatures.Validation.V1.CRLValidator should be used instead."
        )]
    public class CRLVerifier : RootStoreVerifier {
        /// <summary>The Logger instance</summary>
        protected internal static readonly ILogger LOGGER = ITextLogManager.GetLogger(typeof(iText.Signatures.CRLVerifier
            ));

//\cond DO_NOT_DOCUMENT
        /// <summary>The list of CRLs to check for revocation date.</summary>
        internal IList<IX509Crl> crls;
//\endcond

        /// <summary>Creates a CRLVerifier instance.</summary>
        /// <param name="verifier">the next verifier in the chain</param>
        /// <param name="crls">a list of CRLs</param>
        public CRLVerifier(CertificateVerifier verifier, IList<IX509Crl> crls)
            : base(verifier) {
            this.crls = crls;
        }

        /// <summary>Verifies whether a valid CRL is found for the certificate.</summary>
        /// <remarks>
        /// Verifies whether a valid CRL is found for the certificate.
        /// If this method returns false, it doesn't mean the certificate isn't valid.
        /// It means we couldn't verify it against any CRL that was available.
        /// </remarks>
        /// <param name="signCert">the certificate that needs to be checked</param>
        /// <param name="issuerCert">its issuer</param>
        /// <returns>
        /// a list of <c>VerificationOK</c> objects.
        /// The list will be empty if the certificate couldn't be verified.
        /// </returns>
        /// <seealso cref="RootStoreVerifier.Verify(iText.Commons.Bouncycastle.Cert.IX509Certificate, iText.Commons.Bouncycastle.Cert.IX509Certificate, System.DateTime)
        ///     "/>
        public override IList<VerificationOK> Verify(IX509Certificate signCert, IX509Certificate issuerCert, DateTime
             signDate) {
            IList<VerificationOK> result = new List<VerificationOK>();
            int validCrlsFound = 0;
            // first check the list of CRLs that is provided
            if (crls != null) {
                foreach (IX509Crl crl in crls) {
                    if (Verify(crl, signCert, issuerCert, signDate)) {
                        validCrlsFound++;
                    }
                }
            }
            // then check online if allowed
            bool online = false;
            if (onlineCheckingAllowed && validCrlsFound == 0) {
                if (Verify(GetCRL(signCert, issuerCert), signCert, issuerCert, signDate)) {
                    validCrlsFound++;
                    online = true;
                }
            }
            // show how many valid CRLs were found
            LOGGER.LogInformation("Valid CRLs found: " + validCrlsFound);
            if (validCrlsFound > 0) {
                result.Add(new VerificationOK(signCert, this.GetType(), "Valid CRLs found: " + validCrlsFound + (online ? 
                    " (online)" : "")));
            }
            if (verifier != null) {
                result.AddAll(verifier.Verify(signCert, issuerCert, signDate));
            }
            // verify using the previous verifier in the chain (if any)
            return result;
        }

        /// <summary>Verifies a certificate against a single CRL.</summary>
        /// <param name="crl">the Certificate Revocation List</param>
        /// <param name="signCert">a certificate that needs to be verified</param>
        /// <param name="issuerCert">its issuer</param>
        /// <param name="signDate">the sign date</param>
        /// <returns>true if the verification succeeded</returns>
        public virtual bool Verify(IX509Crl crl, IX509Certificate signCert, IX509Certificate issuerCert, DateTime 
            signDate) {
            if (crl == null || signDate == TimestampConstants.UNDEFINED_TIMESTAMP_DATE) {
                return false;
            }
            // We only check CRLs valid on the signing date for which the issuer matches
            if (crl.GetIssuerDN().Equals(signCert.GetIssuerDN()) && signDate.Before(crl.GetNextUpdate())) {
                // the signing certificate may not be revoked
                if (IsSignatureValid(crl, issuerCert) && crl.IsRevoked(signCert)) {
                    throw new VerificationException(signCert, "The certificate has been revoked.");
                }
                return true;
            }
            return false;
        }

        /// <summary>Fetches a CRL for a specific certificate online (without further checking).</summary>
        /// <param name="signCert">the certificate</param>
        /// <param name="issuerCert">its issuer left for backwards compatibility</param>
        /// <returns>an X509CRL object.</returns>
        public virtual IX509Crl GetCRL(IX509Certificate signCert, IX509Certificate issuerCert) {
            try {
                // gets the URL from the certificate
                String crlurl = CertificateUtil.GetCRLURL(signCert);
                if (crlurl == null) {
                    return null;
                }
                LOGGER.LogInformation("Getting CRL from " + crlurl);
                return (IX509Crl)SignUtils.ParseCrlFromStream(UrlUtil.OpenStream(new Uri(crlurl)));
            }
            catch (System.IO.IOException) {
                return null;
            }
            catch (AbstractGeneralSecurityException) {
                return null;
            }
        }

        /// <summary>Checks if a CRL verifies against the issuer certificate or a trusted anchor.</summary>
        /// <param name="crl">the CRL</param>
        /// <param name="crlIssuer">the trusted anchor</param>
        /// <returns>true if the CRL can be trusted</returns>
        public virtual bool IsSignatureValid(IX509Crl crl, IX509Certificate crlIssuer) {
            // check if the CRL was issued by the issuer
            if (crlIssuer != null) {
                try {
                    crl.Verify(crlIssuer.GetPublicKey());
                    return true;
                }
                catch (AbstractGeneralSecurityException) {
                    LOGGER.LogWarning("CRL not issued by the same authority as the certificate that is being checked");
                }
            }
            // check the CRL against trusted anchors
            if (rootStore == null) {
                return false;
            }
            try {
                // loop over the certificate in the key store
                foreach (IX509Certificate anchor in SignUtils.GetCertificates(rootStore)) {
                    try {
                        // check if the crl was signed by a trusted party (indirect CRLs)
                        crl.Verify(anchor.GetPublicKey());
                        return true;
                    }
                    catch (AbstractGeneralSecurityException) {
                    }
                }
            }
            catch (AbstractGeneralSecurityException) {
            }
            // do nothing and continue
            // do nothing and return false at the end
            return false;
        }
    }
}
