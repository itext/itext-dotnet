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
using System.Collections;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using iText.Bouncycastleconnector;
using iText.Commons;
using iText.Commons.Bouncycastle;
using iText.Commons.Bouncycastle.Asn1.Ocsp;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Cert.Ocsp;
using iText.Commons.Bouncycastle.Operator;
using iText.Commons.Bouncycastle.Security;
using iText.Commons.Utils;

namespace iText.Signatures {
    /// <summary>
    /// Class that allows you to verify a certificate against
    /// one or more OCSP responses.
    /// </summary>
    public class OCSPVerifier : RootStoreVerifier {
        private static readonly IBouncyCastleFactory BOUNCY_CASTLE_FACTORY = BouncyCastleFactoryCreator.GetFactory
            ();

        /// <summary>The Logger instance</summary>
        protected internal static readonly ILogger LOGGER = ITextLogManager.GetLogger(typeof(iText.Signatures.OCSPVerifier
            ));

        protected internal const String id_kp_OCSPSigning = "1.3.6.1.5.5.7.3.9";

        /// <summary>
        /// The list of
        /// <see cref="iText.Commons.Bouncycastle.Asn1.Ocsp.IBasicOcspResponse"/>
        /// OCSP response wrappers.
        /// </summary>
        protected internal IList<IBasicOcspResponse> ocsps;

        /// <summary>Creates an OCSPVerifier instance.</summary>
        /// <param name="verifier">the next verifier in the chain</param>
        /// <param name="ocsps">
        /// a list of
        /// <see cref="iText.Commons.Bouncycastle.Asn1.Ocsp.IBasicOcspResponse"/>
        /// OCSP response wrappers
        /// </param>
        public OCSPVerifier(CertificateVerifier verifier, IList<IBasicOcspResponse> ocsps)
            : base(verifier) {
            this.ocsps = ocsps;
        }

        /// <summary>Verifies if a valid OCSP response is found for the certificate.</summary>
        /// <remarks>
        /// Verifies if a valid OCSP response is found for the certificate.
        /// If this method returns false, it doesn't mean the certificate isn't valid.
        /// It means we couldn't verify it against any OCSP response that was available.
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
            int validOCSPsFound = 0;
            // first check in the list of OCSP responses that was provided
            if (ocsps != null) {
                foreach (IBasicOcspResponse ocspResp in ocsps) {
                    if (Verify(ocspResp, signCert, issuerCert, signDate)) {
                        validOCSPsFound++;
                    }
                }
            }
            // then check online if allowed
            bool online = false;
            if (onlineCheckingAllowed && validOCSPsFound == 0) {
                if (Verify(GetOcspResponse(signCert, issuerCert), signCert, issuerCert, signDate)) {
                    validOCSPsFound++;
                    online = true;
                }
            }
            // show how many valid OCSP responses were found
            LOGGER.LogInformation("Valid OCSPs found: " + validOCSPsFound);
            if (validOCSPsFound > 0) {
                result.Add(new VerificationOK(signCert, this.GetType(), "Valid OCSPs Found: " + validOCSPsFound + (online ? 
                    " (online)" : "")));
            }
            if (verifier != null) {
                result.AddAll(verifier.Verify(signCert, issuerCert, signDate));
            }
            // verify using the previous verifier in the chain (if any)
            return result;
        }

        /// <summary>Verifies a certificate against a single OCSP response</summary>
        /// <param name="ocspResp">
        /// 
        /// <see cref="iText.Commons.Bouncycastle.Asn1.Ocsp.IBasicOcspResponse"/>
        /// the OCSP response wrapper
        /// </param>
        /// <param name="signCert">the certificate that needs to be checked</param>
        /// <param name="issuerCert">
        /// the certificate of CA (certificate that issued signCert). This certificate is considered trusted
        /// and valid by this method.
        /// </param>
        /// <param name="signDate">sign date</param>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// , in case successful check, otherwise false.
        /// </returns>
        public virtual bool Verify(IBasicOcspResponse ocspResp, IX509Certificate signCert, IX509Certificate issuerCert
            , DateTime signDate) {
            if (ocspResp == null) {
                return false;
            }
            // Getting the responses
            ISingleResponse[] resp = ocspResp.GetResponses();
            foreach (ISingleResponse iSingleResp in resp) {
                // check if the serial number corresponds
                if (!signCert.GetSerialNumber().Equals(iSingleResp.GetCertID().GetSerialNumber())) {
                    continue;
                }
                // check if the issuer matches
                try {
                    if (issuerCert == null) {
                        issuerCert = signCert;
                    }
                    if (!SignUtils.CheckIfIssuersMatch(iSingleResp.GetCertID(), issuerCert)) {
                        LOGGER.LogInformation("OCSP: Issuers doesn't match.");
                        continue;
                    }
                }
                catch (System.IO.IOException e) {
                    throw iText.Bouncycastleconnector.BouncyCastleFactoryCreator.GetFactory().CreateGeneralSecurityException(e
                        .Message);
                }
                catch (AbstractOcspException) {
                    continue;
                }
                catch (AbstractOperatorCreationException) {
                    continue;
                }
                // check if the OCSP response was valid at the time of signing
                if (iSingleResp.GetNextUpdate() == null) {
                    DateTime nextUpdate = SignUtils.Add180Sec(iSingleResp.GetThisUpdate());
                    LOGGER.LogInformation(MessageFormatUtil.Format("No 'next update' for OCSP Response; assuming {0}", nextUpdate
                        ));
                    if (signDate.After(nextUpdate)) {
                        LOGGER.LogInformation(MessageFormatUtil.Format("OCSP no longer valid: {0} after {1}", signDate, nextUpdate
                            ));
                        continue;
                    }
                }
                else {
                    if (signDate.After(iSingleResp.GetNextUpdate())) {
                        LOGGER.LogInformation(MessageFormatUtil.Format("OCSP no longer valid: {0} after {1}", signDate, iSingleResp
                            .GetNextUpdate()));
                        continue;
                    }
                }
                // check the status of the certificate
                Object status = iSingleResp.GetCertStatus();
                if (Object.Equals(status, BOUNCY_CASTLE_FACTORY.CreateCertificateStatus().GetGood())) {
                    // check if the OCSP response was genuine
                    IsValidResponse(ocspResp, issuerCert, signDate);
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
        /// <param name="ocspResp">
        /// 
        /// <see cref="iText.Commons.Bouncycastle.Asn1.Ocsp.IBasicOcspResponse"/>
        /// the OCSP response wrapper
        /// </param>
        /// <param name="issuerCert">the issuer certificate. This certificate is considered trusted and valid by this method.
        ///     </param>
        /// <param name="signDate">sign date</param>
        public virtual void IsValidResponse(IBasicOcspResponse ocspResp, IX509Certificate issuerCert, DateTime signDate
            ) {
            // OCSP response might be signed by the issuer certificate or
            // the Authorized OCSP responder certificate containing the id-kp-OCSPSigning extended key usage extension
            IX509Certificate responderCert = null;
            // first check if the issuer certificate signed the response
            // since it is expected to be the most common case
            if (IsSignatureValid(ocspResp, issuerCert)) {
                responderCert = issuerCert;
            }
            // if the issuer certificate didn't sign the ocsp response, look for authorized ocsp responses
            // from properties or from certificate chain received with response
            if (responderCert == null) {
                if (ocspResp.GetCerts() != null) {
                    //look for existence of Authorized OCSP responder inside the cert chain in ocsp response
                    IEnumerable<IX509Certificate> certs = SignUtils.GetCertsFromOcspResponse(ocspResp);
                    foreach (IX509Certificate cert in certs) {
                        IList keyPurposes = null;
                        try {
                            keyPurposes = cert.GetExtendedKeyUsage();
                            if ((keyPurposes != null) && keyPurposes.Contains(id_kp_OCSPSigning) && IsSignatureValid(ocspResp, cert)) {
                                responderCert = cert;
                                break;
                            }
                        }
                        catch (AbstractCertificateParsingException) {
                        }
                    }
                    // Certificate signing the ocsp response is not found in ocsp response's certificate chain received
                    // and is not signed by the issuer certificate.
                    if (responderCert == null) {
                        throw new VerificationException(issuerCert, "OCSP response could not be verified");
                    }
                    // RFC 6960 4.2.2.2. Authorized Responders:
                    // "Systems relying on OCSP responses MUST recognize a delegation certificate as being issued
                    // by the CA that issued the certificate in question only if the delegation certificate and the
                    // certificate being checked for revocation were signed by the same key."
                    // and
                    // "This certificate MUST be issued directly by the CA that is identified in the request"
                    responderCert.Verify(issuerCert.GetPublicKey());
                    // check if lifetime of certificate is ok
                    responderCert.CheckValidity(signDate);
                    // validating ocsp signers certificate
                    // Check if responders certificate has id-pkix-ocsp-nocheck extension,
                    // in which case we do not validate (perform revocation check on) ocsp certs for lifetime of certificate
                    if (SignUtils.GetExtensionValueByOid(responderCert, BOUNCY_CASTLE_FACTORY.CreateOCSPObjectIdentifiers().GetIdPkixOcspNoCheck
                        ().GetId()) == null) {
                        IX509Crl crl;
                        try {
                            // TODO DEVSIX-5210 Implement a check heck for Authority Information Access according to
                            // RFC6960 4.2.2.2.1. "Revocation Checking of an Authorized Responder"
                            crl = CertificateUtil.GetCRL(responderCert);
                        }
                        catch (Exception) {
                            crl = (IX509Crl)null;
                        }
                        if (crl != null && crl is IX509Crl) {
                            CRLVerifier crlVerifier = new CRLVerifier(null, null);
                            crlVerifier.SetRootStore(rootStore);
                            crlVerifier.SetOnlineCheckingAllowed(onlineCheckingAllowed);
                            if (!crlVerifier.Verify((IX509Crl)crl, responderCert, issuerCert, signDate)) {
                                throw new VerificationException(issuerCert, "Authorized OCSP responder certificate was revoked.");
                            }
                        }
                        else {
                            LOGGER.LogError("Authorized OCSP responder certificate revocation status cannot be checked");
                        }
                    }
                }
                else {
                    // TODO DEVSIX-5207 throw exception starting from iText version 7.2, but only after OCSPVerifier
                    // would allow explicit setting revocation check end points/provide revocation data
                    // certificate chain is not present in response received
                    // try to verify using rootStore according to RFC 6960 2.2. Response:
                    // "The key used to sign the response MUST belong to one of the following:
                    // - ...
                    // - a Trusted Responder whose public key is trusted by the requestor;
                    // - ..."
                    if (rootStore != null) {
                        try {
                            foreach (IX509Certificate anchor in SignUtils.GetCertificates(rootStore)) {
                                if (IsSignatureValid(ocspResp, anchor)) {
                                    // certificate from the root store is considered trusted and valid by this method
                                    responderCert = anchor;
                                    break;
                                }
                            }
                        }
                        catch (Exception) {
                            responderCert = (IX509Certificate)null;
                        }
                    }
                    if (responderCert == null) {
                        throw new VerificationException(issuerCert, "OCSP response could not be verified: it does not contain certificate chain and response is not signed by issuer certificate or any from the root store."
                            );
                    }
                }
            }
        }

        /// <summary>Checks if an OCSP response is genuine</summary>
        /// <param name="ocspResp">
        /// 
        /// <see cref="iText.Commons.Bouncycastle.Asn1.Ocsp.IBasicOcspResponse"/>
        /// the OCSP response wrapper
        /// </param>
        /// <param name="responderCert">the responder certificate</param>
        /// <returns>true if the OCSP response verifies against the responder certificate</returns>
        public virtual bool IsSignatureValid(IBasicOcspResponse ocspResp, IX509Certificate responderCert) {
            try {
                return SignUtils.IsSignatureValid(ocspResp, responderCert);
            }
            catch (Exception) {
                return false;
            }
        }

        /// <summary>
        /// Gets an OCSP response online and returns it if the status is GOOD
        /// (without further checking!).
        /// </summary>
        /// <param name="signCert">the signing certificate</param>
        /// <param name="issuerCert">the issuer certificate</param>
        /// <returns>
        /// 
        /// <see cref="iText.Commons.Bouncycastle.Asn1.Ocsp.IBasicOcspResponse"/>
        /// an OCSP response wrapper
        /// </returns>
        public virtual IBasicOcspResponse GetOcspResponse(IX509Certificate signCert, IX509Certificate issuerCert) {
            if (signCert == null && issuerCert == null) {
                return null;
            }
            OcspClientBouncyCastle ocsp = new OcspClientBouncyCastle(null);
            IBasicOcspResponse ocspResp = ocsp.GetBasicOCSPResp(signCert, issuerCert, null);
            if (ocspResp == null) {
                return null;
            }
            ISingleResponse[] resps = ocspResp.GetResponses();
            foreach (ISingleResponse resp in resps) {
                Object status = resp.GetCertStatus();
                if (Object.Equals(status, BOUNCY_CASTLE_FACTORY.CreateCertificateStatus().GetGood())) {
                    return ocspResp;
                }
            }
            return null;
        }
    }
}
