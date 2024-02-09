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
using System.Collections;
using System.Collections.Generic;
using System.IO;
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

        /// <summary>Ocsp client to check OCSP Authorized Responder's revocation data.</summary>
        private IOcspClient ocspClient;

        /// <summary>Ocsp client to check OCSP Authorized Responder's revocation data.</summary>
        private ICrlClient crlClient;

        /// <summary>Creates an OCSPVerifier instance.</summary>
        /// <param name="verifier">the next verifier in the chain</param>
        /// <param name="ocsps">
        /// a list of
        /// <see cref="iText.Commons.Bouncycastle.Asn1.Ocsp.IBasicOcspResponse"/>
        /// OCSP response wrappers for the certificate verification
        /// </param>
        public OCSPVerifier(CertificateVerifier verifier, IList<IBasicOcspResponse> ocsps)
            : base(verifier) {
            this.ocsps = ocsps;
        }

        /// <summary>
        /// Sets OCSP client to provide OCSP responses for verifying of the OCSP signer's certificate (an Authorized
        /// Responder).
        /// </summary>
        /// <remarks>
        /// Sets OCSP client to provide OCSP responses for verifying of the OCSP signer's certificate (an Authorized
        /// Responder). Also, should be used in case responder's certificate doesn't have any method of revocation checking.
        /// <para />
        /// See RFC6960 4.2.2.2.1. Revocation Checking of an Authorized Responder.
        /// <para />
        /// Optional. Default one is
        /// <see cref="OcspClientBouncyCastle"/>.
        /// </remarks>
        /// <param name="ocspClient">
        /// 
        /// <see cref="IOcspClient"/>
        /// to provide an Authorized Responder revocation data.
        /// </param>
        public virtual void SetOcspClient(IOcspClient ocspClient) {
            this.ocspClient = ocspClient;
        }

        /// <summary>
        /// Sets CRL client to provide CRL responses for verifying of the OCSP signer's certificate (an Authorized Responder)
        /// that also should be used in case responder's certificate doesn't have any method of revocation checking.
        /// </summary>
        /// <remarks>
        /// Sets CRL client to provide CRL responses for verifying of the OCSP signer's certificate (an Authorized Responder)
        /// that also should be used in case responder's certificate doesn't have any method of revocation checking.
        /// <para />
        /// See RFC6960 4.2.2.2.1. Revocation Checking of an Authorized Responder.
        /// <para />
        /// Optional. Default one is
        /// <see cref="CrlClientOnline"/>.
        /// </remarks>
        /// <param name="crlClient">
        /// 
        /// <see cref="ICrlClient"/>
        /// to provide an Authorized Responder revocation data.
        /// </param>
        public virtual void SetCrlClient(ICrlClient crlClient) {
            this.crlClient = crlClient;
        }

        /// <summary>Verifies if a valid OCSP response is found for the certificate.</summary>
        /// <remarks>
        /// Verifies if a valid OCSP response is found for the certificate.
        /// If this method returns false, it doesn't mean the certificate isn't valid.
        /// It means we couldn't verify it against any OCSP response that was available.
        /// </remarks>
        /// <param name="signCert">the certificate that needs to be checked</param>
        /// <param name="issuerCert">issuer of the certificate to be checked</param>
        /// <param name="signDate">the date the certificate needs to be valid</param>
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
            // First check in the list of OCSP responses that was provided.
            if (ocsps != null) {
                foreach (IBasicOcspResponse ocspResp in ocsps) {
                    if (Verify(ocspResp, signCert, issuerCert, signDate)) {
                        validOCSPsFound++;
                    }
                }
            }
            // Then check online if allowed.
            bool online = false;
            if (onlineCheckingAllowed && validOCSPsFound == 0) {
                if (Verify(GetOcspResponse(signCert, issuerCert), signCert, issuerCert, signDate)) {
                    validOCSPsFound++;
                    online = true;
                }
            }
            // Show how many valid OCSP responses were found.
            LOGGER.LogInformation("Valid OCSPs found: " + validOCSPsFound);
            if (validOCSPsFound > 0) {
                result.Add(new VerificationOK(signCert, this.GetType(), "Valid OCSPs Found: " + validOCSPsFound + (online ? 
                    " (online)" : "")));
            }
            // Verify using the previous verifier in the chain (if any).
            if (verifier != null) {
                result.AddAll(verifier.Verify(signCert, issuerCert, signDate));
            }
            return result;
        }

        /// <summary>Verifies a certificate against a single OCSP response.</summary>
        /// <param name="ocspResp">
        /// 
        /// <see cref="iText.Commons.Bouncycastle.Asn1.Ocsp.IBasicOcspResponse"/>
        /// the OCSP response wrapper for a certificate verification
        /// </param>
        /// <param name="signCert">the certificate that needs to be checked</param>
        /// <param name="issuerCert">
        /// the certificate that issued signCert – immediate parent. This certificate is considered
        /// trusted and valid by this method.
        /// </param>
        /// <param name="signDate">sign date (or the date the certificate needs to be valid)</param>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// in case check is successful, false otherwise.
        /// </returns>
        public virtual bool Verify(IBasicOcspResponse ocspResp, IX509Certificate signCert, IX509Certificate issuerCert
            , DateTime signDate) {
            if (ocspResp == null) {
                return false;
            }
            // Getting the responses.
            ISingleResponse[] resp = ocspResp.GetResponses();
            foreach (ISingleResponse iSingleResp in resp) {
                // SingleResp contains the basic information of the status of the certificate identified by the certID.
                // Check if the serial numbers of the signCert and certID corresponds:
                if (!signCert.GetSerialNumber().Equals(iSingleResp.GetCertID().GetSerialNumber())) {
                    continue;
                }
                // Check if the issuer of the certID and signCert matches, i.e. check that issuerNameHash and issuerKeyHash
                // fields of the certID is the hash of the issuer's name and public key:
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
                // So, since the issuer name and serial number identify a unique certificate, we found the single response
                // for the signCert.
                // Check if the OCSP response was valid at the time of signing:
                DateTime thisUpdate = iSingleResp.GetThisUpdate();
                if (signDate.Before(thisUpdate)) {
                    LOGGER.LogInformation(MessageFormatUtil.Format("OCSP is not valid yet: {0} before {1}", signDate, thisUpdate
                        ));
                    continue;
                }
                // If nextUpdate is not set, the responder is indicating that newer revocation information
                // is available all the time.
                if (iSingleResp.GetNextUpdate() != null && signDate.After(iSingleResp.GetNextUpdate())) {
                    LOGGER.LogInformation(MessageFormatUtil.Format("OCSP is no longer valid: {0} after {1}", signDate, iSingleResp
                        .GetNextUpdate()));
                    continue;
                }
                // Check the status of the certificate:
                Object status = iSingleResp.GetCertStatus();
                if (Object.Equals(status, BOUNCY_CASTLE_FACTORY.CreateCertificateStatus().GetGood())) {
                    // Check if the OCSP response was genuine.
                    IsValidResponse(ocspResp, issuerCert, signDate);
                    return true;
                }
            }
            return false;
        }

        /// <summary>Verifies if an OCSP response is genuine.</summary>
        /// <remarks>
        /// Verifies if an OCSP response is genuine.
        /// If it doesn't verify against the issuer certificate and response's certificates, it may verify
        /// using a trusted anchor or cert.
        /// </remarks>
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
            // the Authorized OCSP responder certificate containing the id-kp-OCSPSigning extended key usage extension.
            IX509Certificate responderCert = null;
            // First check if the issuer certificate signed the response since it is expected to be the most common case:
            if (IsSignatureValid(ocspResp, issuerCert)) {
                responderCert = issuerCert;
            }
            // If the issuer certificate didn't sign the ocsp response, look for authorized ocsp responses
            // from the properties or from the certificate chain received with response.
            if (responderCert == null) {
                if (ocspResp.GetOcspCerts().Length > 0) {
                    // Look for the existence of an Authorized OCSP responder inside the cert chain in the ocsp response.
                    IEnumerable<IX509Certificate> certs = SignUtils.GetCertsFromOcspResponse(ocspResp);
                    foreach (IX509Certificate cert in certs) {
                        try {
                            IList keyPurposes = cert.GetExtendedKeyUsage();
                            if (keyPurposes != null && keyPurposes.Contains(id_kp_OCSPSigning) && IsSignatureValid(ocspResp, cert)) {
                                responderCert = cert;
                                break;
                            }
                        }
                        catch (AbstractCertificateParsingException) {
                        }
                    }
                    // Certificate signing the ocsp response is not found in ocsp response's certificate chain received
                    // and is not signed by the issuer certificate.
                    // RFC 6960 4.2.1. ASN.1 Specification of the OCSP Response: "The responder MAY include certificates in
                    // the certs field of BasicOCSPResponse that help the OCSP client verify the responder's signature.
                    // If no certificates are included, then certs SHOULD be absent".
                    if (responderCert == null) {
                        throw new VerificationException(issuerCert, "OCSP response could not be verified");
                    }
                    // RFC 6960 4.2.2.2. Authorized Responders:
                    // "Systems relying on OCSP responses MUST recognize a delegation certificate as being issued
                    // by the CA that issued the certificate in question only if the delegation certificate and the
                    // certificate being checked for revocation were signed by the same key."
                    // and "This certificate MUST be issued directly by the CA that is identified in the request".
                    responderCert.Verify(issuerCert.GetPublicKey());
                    // Check if the lifetime of the certificate is valid.
                    responderCert.CheckValidity(signDate);
                    // Validating ocsp signer's certificate (responderCert).
                    // See RFC6960 4.2.2.2.1. Revocation Checking of an Authorized Responder.
                    // 1. Check if responders certificate has id-pkix-ocsp-nocheck extension, in which case we do not
                    // validate (perform revocation check on) ocsp certs for the lifetime of the responder certificate.
                    if (SignUtils.GetExtensionValueByOid(responderCert, BOUNCY_CASTLE_FACTORY.CreateOCSPObjectIdentifiers().GetIdPkixOcspNoCheck
                        ().GetId()) != null) {
                        return;
                    }
                    // 2.1. Try to check responderCert for revocation using provided responder OCSP/CRL clients.
                    if (ocspClient != null) {
                        IBasicOcspResponse responderOcspResp = null;
                        byte[] basicOcspRespBytes = ocspClient.GetEncoded(responderCert, issuerCert, null);
                        if (basicOcspRespBytes != null) {
                            try {
                                responderOcspResp = BOUNCY_CASTLE_FACTORY.CreateBasicOCSPResponse(BOUNCY_CASTLE_FACTORY.CreateASN1Primitive
                                    (basicOcspRespBytes));
                            }
                            catch (System.IO.IOException) {
                            }
                        }
                        if (VerifyOcsp(responderOcspResp, responderCert, issuerCert, signDate)) {
                            return;
                        }
                    }
                    if (crlClient != null && CheckCrlResponses(crlClient, responderCert, issuerCert, signDate)) {
                        return;
                    }
                    // 2.2. Try to check responderCert for revocation using Authority Information Access for OCSP responses
                    // or CRL Distribution Points for CRL responses using default clients.
                    IBasicOcspResponse responderOcspResp_1 = new OcspClientBouncyCastle(null).GetBasicOCSPResp(responderCert, 
                        issuerCert, null);
                    if (VerifyOcsp(responderOcspResp_1, responderCert, issuerCert, signDate)) {
                        return;
                    }
                    if (CheckCrlResponses(new CrlClientOnline(), responderCert, issuerCert, signDate)) {
                        return;
                    }
                    // 3. "A CA may choose not to specify any method of revocation checking for the responder's
                    // certificate, in which case it would be up to the OCSP client's local security policy
                    // to decide whether that certificate should be checked for revocation or not".
                    throw new VerificationException(responderCert, "Authorized OCSP responder certificate revocation status cannot be checked"
                        );
                }
                else {
                    // Certificate chain is not present in the response received.
                    // Try to verify using rootStore according to RFC 6960 2.2. Response:
                    // "The key used to sign the response MUST belong to one of the following:
                    // - ...
                    // - a Trusted Responder whose public key is trusted by the requester;
                    // - ..."
                    if (rootStore != null) {
                        try {
                            foreach (IX509Certificate anchor in SignUtils.GetCertificates(rootStore)) {
                                if (IsSignatureValid(ocspResp, anchor)) {
                                    // Certificate from the root store is considered trusted and valid by this method.
                                    responderCert = anchor;
                                    break;
                                }
                            }
                        }
                        catch (Exception) {
                        }
                    }
                    // Ignore.
                    if (responderCert == null) {
                        throw new VerificationException(issuerCert, "OCSP response could not be verified: it does not contain certificate chain and response is not signed by issuer certificate or any from the root store."
                            );
                    }
                }
            }
            // Check if the lifetime of the certificate is valid.
            responderCert.CheckValidity(signDate);
        }

        /// <summary>Checks if an OCSP response is genuine.</summary>
        /// <param name="ocspResp">
        /// 
        /// <see cref="iText.Commons.Bouncycastle.Asn1.Ocsp.IBasicOcspResponse"/>
        /// the OCSP response wrapper
        /// </param>
        /// <param name="responderCert">the responder certificate</param>
        /// <returns>true if the OCSP response verifies against the responder certificate.</returns>
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
        /// an OCSP response wrapper.
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

        private bool VerifyOcsp(IBasicOcspResponse ocspResp, IX509Certificate certificate, IX509Certificate issuerCert
            , DateTime signDate) {
            if (ocspResp == null) {
                // Unable to verify.
                return false;
            }
            return this.Verify(ocspResp, certificate, issuerCert, signDate);
        }

        private bool CheckCrlResponses(ICrlClient client, IX509Certificate responderCert, IX509Certificate issuerCert
            , DateTime signDate) {
            ICollection<byte[]> crlBytesCollection = client.GetEncoded(responderCert, null);
            foreach (byte[] crlBytes in crlBytesCollection) {
                IX509Crl crl = SignUtils.ParseCrlFromStream(new MemoryStream(crlBytes));
                if (VerifyCrl(crl, responderCert, issuerCert, signDate)) {
                    return true;
                }
            }
            return false;
        }

        private bool VerifyCrl(IX509Crl crl, IX509Certificate certificate, IX509Certificate issuerCert, DateTime signDate
            ) {
            if (crl is IX509Crl) {
                CRLVerifier crlVerifier = new CRLVerifier(null, null);
                crlVerifier.SetRootStore(rootStore);
                crlVerifier.SetOnlineCheckingAllowed(onlineCheckingAllowed);
                return crlVerifier.Verify((IX509Crl)crl, certificate, issuerCert, signDate);
            }
            // Unable to verify.
            return false;
        }
    }
}
