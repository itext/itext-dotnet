/*

This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: Bruno Lowagie, Paulo Soares, et al.

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License version 3
as published by the Free Software Foundation with the addition of the
following permission added to Section 15 as permitted in Section 7(a):
FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
OF THIRD PARTY RIGHTS

This program is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU Affero General Public License for more details.
You should have received a copy of the GNU Affero General Public License
along with this program; if not, see http://www.gnu.org/licenses or write to
the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
Boston, MA, 02110-1301 USA, or download the license from the following URL:
http://itextpdf.com/terms-of-use/

The interactive user interfaces in modified source and object code versions
of this program must display Appropriate Legal Notices, as required under
Section 5 of the GNU Affero General Public License.

In accordance with Section 7(b) of the GNU Affero General Public License,
a covered work must retain the producer line in every PDF that is created
or manipulated using iText.

You can be released from the requirements of the license by purchasing
a commercial license. Buying such a license is mandatory as soon as you
develop commercial activities involving the iText software without
disclosing the source code of your own applications.
These activities include: offering paid services to customers as an ASP,
serving PDFs on the fly in a web application, shipping iText with a closed
source product.

For more information, please contact iText Software Corp. at this
address: sales@itextpdf.com
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
    public class CRLVerifier : RootStoreVerifier {
        /// <summary>The Logger instance</summary>
        protected internal static readonly ILogger LOGGER = ITextLogManager.GetLogger(typeof(iText.Signatures.CRLVerifier
            ));

        /// <summary>The list of CRLs to check for revocation date.</summary>
        internal IList<IX509Crl> crls;

        /// <summary>Creates a CRLVerifier instance.</summary>
        /// <param name="verifier">the next verifier in the chain</param>
        /// <param name="crls">a list of CRLs</param>
        public CRLVerifier(CertificateVerifier verifier, IList<IX509Crl> crls)
            : base(verifier) {
            this.crls = crls;
        }

        /// <summary>Verifies if a a valid CRL is found for the certificate.</summary>
        /// <remarks>
        /// Verifies if a a valid CRL is found for the certificate.
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
        /// <param name="issuerCert">its issuer</param>
        /// <returns>an X509CRL object</returns>
        public virtual IX509Crl GetCRL(IX509Certificate signCert, IX509Certificate issuerCert) {
            if (issuerCert == null) {
                issuerCert = signCert;
            }
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
