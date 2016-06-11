/*

This file is part of the iText (R) project.
Copyright (c) 1998-2016 iText Group NV
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
using System.Collections;
using System.Collections.Generic;
using Org.BouncyCastle.Asn1.Ocsp;
using Org.BouncyCastle.Ocsp;
using Org.BouncyCastle.Security.Certificates;
using Org.BouncyCastle.X509;
using iTextSharp.IO.Log;
using iTextSharp.IO.Util;

namespace iTextSharp.Signatures
{
    /// <summary>
    /// Class that allows you to verify a certificate against
    /// one or more OCSP responses.
    /// </summary>
    public class OCSPVerifier : RootStoreVerifier
    {
        /// <summary>The Logger instance</summary>
        protected internal static readonly ILogger LOGGER = LoggerFactory.GetLogger(typeof(iTextSharp.Signatures.OCSPVerifier
            ));

        protected internal const String id_kp_OCSPSigning = "1.3.6.1.5.5.7.3.9";

        /// <summary>The list of OCSP responses.</summary>
        protected internal IList<BasicOcspResp> ocsps;

        /// <summary>Creates an OCSPVerifier instance.</summary>
        /// <param name="verifier">the next verifier in the chain</param>
        /// <param name="ocsps">a list of OCSP responses</param>
        public OCSPVerifier(CertificateVerifier verifier, IList<BasicOcspResp> ocsps)
            : base(verifier)
        {
            this.ocsps = ocsps;
        }

        /// <summary>Verifies if a a valid OCSP response is found for the certificate.</summary>
        /// <remarks>
        /// Verifies if a a valid OCSP response is found for the certificate.
        /// If this method returns false, it doesn't mean the certificate isn't valid.
        /// It means we couldn't verify it against any OCSP response that was available.
        /// </remarks>
        /// <param name="signCert">the certificate that needs to be checked</param>
        /// <param name="issuerCert">its issuer</param>
        /// <returns>
        /// a list of <code>VerificationOK</code> objects.
        /// The list will be empty if the certificate couldn't be verified.
        /// </returns>
        /// <seealso cref="RootStoreVerifier.Verify(Org.BouncyCastle.X509.X509Certificate, Org.BouncyCastle.X509.X509Certificate, System.DateTime)
        ///     "/>
        /// <exception cref="Org.BouncyCastle.Security.GeneralSecurityException"/>
        /// <exception cref="System.IO.IOException"/>
        public override IList<VerificationOK> Verify(X509Certificate signCert, X509Certificate issuerCert, DateTime
             signDate)
        {
            IList<VerificationOK> result = new List<VerificationOK>();
            int validOCSPsFound = 0;
            // first check in the list of OCSP responses that was provided
            if (ocsps != null)
            {
                foreach (BasicOcspResp ocspResp in ocsps)
                {
                    if (Verify(ocspResp, signCert, issuerCert, signDate))
                    {
                        validOCSPsFound++;
                    }
                }
            }
            // then check online if allowed
            bool online = false;
            if (onlineCheckingAllowed && validOCSPsFound == 0)
            {
                if (Verify(GetOcspResponse(signCert, issuerCert), signCert, issuerCert, signDate))
                {
                    validOCSPsFound++;
                    online = true;
                }
            }
            // show how many valid OCSP responses were found
            LOGGER.Info("Valid OCSPs found: " + validOCSPsFound);
            if (validOCSPsFound > 0)
            {
                result.Add(new VerificationOK(signCert, this.GetType(), "Valid OCSPs Found: " + validOCSPsFound + (online ? 
                    " (online)" : "")));
            }
            if (verifier != null)
            {
                result.AddAll(verifier.Verify(signCert, issuerCert, signDate));
            }
            // verify using the previous verifier in the chain (if any)
            return result;
        }

        /// <summary>Verifies a certificate against a single OCSP response</summary>
        /// <param name="ocspResp">the OCSP response</param>
        /// <param name="signCert">the certificate that needs to be checked</param>
        /// <param name="issuerCert">the certificate of CA</param>
        /// <param name="signDate">sign date</param>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// , in case successful check, otherwise false.
        /// </returns>
        /// <exception cref="Org.BouncyCastle.Security.GeneralSecurityException"/>
        /// <exception cref="System.IO.IOException"/>
        public virtual bool Verify(BasicOcspResp ocspResp, X509Certificate signCert, X509Certificate issuerCert, DateTime
             signDate)
        {
            if (ocspResp == null)
            {
                return false;
            }
            // Getting the responses
            SingleResp[] resp = ocspResp.Responses;
            for (int i = 0; i < resp.Length; i++)
            {
                // check if the serial number corresponds
                if (!signCert.SerialNumber.Equals(resp[i].GetCertID().SerialNumber))
                {
                    continue;
                }
                // check if the issuer matches
                try
                {
                    if (issuerCert == null)
                    {
                        issuerCert = signCert;
                    }
                    if (!SignUtils.CheckIfIssuersMatch(resp[i].GetCertID(), issuerCert))
                    {
                        LOGGER.Info("OCSP: Issuers doesn't match.");
                        continue;
                    }
                }
                catch (OcspException)
                {
                    continue;
                }
                // check if the OCSP response was valid at the time of signing
                if (resp[i].NextUpdate == null)
                {
                    DateTime nextUpdate = SignUtils.Add180Sec(resp[i].ThisUpdate);
                    LOGGER.Info(String.Format("No 'next update' for OCSP Response; assuming {0}", nextUpdate));
                    if (signDate.After(nextUpdate))
                    {
                        LOGGER.Info(String.Format("OCSP no longer valid: {0} after {1}", signDate, nextUpdate));
                        continue;
                    }
                }
                else
                {
                    if (signDate.After(resp[i].NextUpdate))
                    {
                        LOGGER.Info(String.Format("OCSP no longer valid: {0} after {1}", signDate, resp[i].NextUpdate));
                        continue;
                    }
                }
                // check the status of the certificate
                Object status = resp[i].GetCertStatus();
                if (status == CertificateStatus.Good)
                {
                    // check if the OCSP response was genuine
                    IsValidResponse(ocspResp, issuerCert);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Verifies if an OCSP response is genuine
        /// If it doesn't verify against the issuer certificate and response's certificates, it may verify
        /// using a trusted anchor or cert.
        /// </summary>
        /// <param name="ocspResp">the OCSP response</param>
        /// <param name="issuerCert">the issuer certificate</param>
        /// <exception cref="Org.BouncyCastle.Security.GeneralSecurityException"/>
        /// <exception cref="System.IO.IOException"/>
        public virtual void IsValidResponse(BasicOcspResp ocspResp, X509Certificate issuerCert)
        {
            //OCSP response might be signed by the issuer certificate or
            //the Authorized OCSP responder certificate containing the id-kp-OCSPSigning extended key usage extension
            X509Certificate responderCert = null;
            //first check if the issuer certificate signed the response
            //since it is expected to be the most common case
            if (IsSignatureValid(ocspResp, issuerCert))
            {
                responderCert = issuerCert;
            }
            //if the issuer certificate didn't sign the ocsp response, look for authorized ocsp responses
            // from properties or from certificate chain received with response
            if (responderCert == null)
            {
                if (ocspResp.GetCerts() != null)
                {
                    //look for existence of Authorized OCSP responder inside the cert chain in ocsp response
                    IEnumerable<X509Certificate> certs = SignUtils.GetCertsFromOcspResponse(ocspResp);
                    foreach (X509Certificate cert in certs)
                    {
                        IList keyPurposes = null;
                        try
                        {
                            keyPurposes = cert.GetExtendedKeyUsage();
                            if ((keyPurposes != null) && keyPurposes.Contains(id_kp_OCSPSigning) && IsSignatureValid(ocspResp, cert))
                            {
                                responderCert = cert;
                                break;
                            }
                        }
                        catch (CertificateParsingException)
                        {
                        }
                    }
                    // Certificate signing the ocsp response is not found in ocsp response's certificate chain received
                    // and is not signed by the issuer certificate.
                    if (responderCert == null)
                    {
                        throw new VerificationException(issuerCert, "OCSP response could not be verified");
                    }
                }
                else
                {
                    //certificate chain is not present in response received
                    //try to verify using rootStore
                    if (rootStore != null)
                    {
                        try
                        {
                            foreach (X509Certificate anchor in SignUtils.GetCertificates(rootStore))
                            {
                                if (IsSignatureValid(ocspResp, anchor))
                                {
                                    responderCert = anchor;
                                    break;
                                }
                            }
                        }
                        catch (Exception)
                        {
                            responderCert = (X509Certificate)null;
                        }
                    }
                    // OCSP Response does not contain certificate chain, and response is not signed by any
                    // of the rootStore or the issuer certificate.
                    if (responderCert == null)
                    {
                        throw new VerificationException(issuerCert, "OCSP response could not be verified");
                    }
                }
            }
            //check "This certificate MUST be issued directly by the CA that issued the certificate in question".
            responderCert.Verify(issuerCert.GetPublicKey());
            // validating ocsp signers certificate
            // Check if responders certificate has id-pkix-ocsp-nocheck extension,
            // in which case we do not validate (perform revocation check on) ocsp certs for lifetime of certificate
            if (responderCert.GetExtensionValue(OcspObjectIdentifiers.PkixOcspNocheck.Id) == null)
            {
                X509Crl crl;
                try
                {
                    crl = CertificateUtil.GetCRL(responderCert);
                }
                catch (Exception)
                {
                    crl = (X509Crl)null;
                }
                if (crl != null && crl is X509Crl)
                {
                    CRLVerifier crlVerifier = new CRLVerifier(null, null);
                    crlVerifier.SetRootStore(rootStore);
                    crlVerifier.SetOnlineCheckingAllowed(onlineCheckingAllowed);
                    crlVerifier.Verify((X509Crl)crl, responderCert, issuerCert, DateTimeUtil.GetCurrentUtcTime());
                    return;
                }
            }
            //check if lifetime of certificate is ok
            responderCert.CheckValidity();
        }

        /// <summary>Verifies if the response is valid.</summary>
        /// <remarks>
        /// Verifies if the response is valid.
        /// If it doesn't verify against the issuer certificate and response's certificates, it may verify
        /// using a trusted anchor or cert.
        /// NOTE. Use
        /// <c>isValidResponse()</c>
        /// instead.
        /// </remarks>
        /// <param name="ocspResp">the response object</param>
        /// <param name="issuerCert">the issuer certificate</param>
        /// <returns>true if the response can be trusted</returns>
        [Obsolete]
        public virtual bool VerifyResponse(BasicOcspResp ocspResp, X509Certificate issuerCert)
        {
            try
            {
                IsValidResponse(ocspResp, issuerCert);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>Checks if an OCSP response is genuine</summary>
        /// <param name="ocspResp">the OCSP response</param>
        /// <param name="responderCert">the responder certificate</param>
        /// <returns>true if the OCSP response verifies against the responder certificate</returns>
        public virtual bool IsSignatureValid(BasicOcspResp ocspResp, X509Certificate responderCert)
        {
            try
            {
                return SignUtils.IsSignatureValid(ocspResp, responderCert);
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Gets an OCSP response online and returns it if the status is GOOD
        /// (without further checking!).
        /// </summary>
        /// <param name="signCert">the signing certificate</param>
        /// <param name="issuerCert">the issuer certificate</param>
        /// <returns>an OCSP response</returns>
        public virtual BasicOcspResp GetOcspResponse(X509Certificate signCert, X509Certificate issuerCert)
        {
            if (signCert == null && issuerCert == null)
            {
                return null;
            }
            OcspClientBouncyCastle ocsp = new OcspClientBouncyCastle(null);
            BasicOcspResp ocspResp = ocsp.GetBasicOCSPResp(signCert, issuerCert, null);
            if (ocspResp == null)
            {
                return null;
            }
            SingleResp[] resps = ocspResp.Responses;
            foreach (SingleResp resp in resps)
            {
                Object status = resp.GetCertStatus();
                if (status == CertificateStatus.Good)
                {
                    return ocspResp;
                }
            }
            return null;
        }
    }
}
