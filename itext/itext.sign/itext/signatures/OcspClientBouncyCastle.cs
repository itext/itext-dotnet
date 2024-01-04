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
using System.IO;
using Microsoft.Extensions.Logging;
using iText.Bouncycastleconnector;
using iText.Commons;
using iText.Commons.Bouncycastle;
using iText.Commons.Bouncycastle.Asn1.Ocsp;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Cert.Ocsp;
using iText.Commons.Bouncycastle.Math;
using iText.Commons.Utils;
using iText.IO.Util;

namespace iText.Signatures {
    /// <summary>OcspClient implementation using BouncyCastle.</summary>
    public class OcspClientBouncyCastle : IOcspClient {
        private static readonly IBouncyCastleFactory BOUNCY_CASTLE_FACTORY = BouncyCastleFactoryCreator.GetFactory
            ();

        /// <summary>The Logger instance.</summary>
        private static readonly ILogger LOGGER = ITextLogManager.GetLogger(typeof(iText.Signatures.OcspClientBouncyCastle
            ));

        private readonly OCSPVerifier verifier;

        /// <summary>
        /// Creates
        /// <c>OcspClient</c>.
        /// </summary>
        /// <param name="verifier">will be used for response verification.</param>
        /// <seealso cref="OCSPVerifier"/>
        public OcspClientBouncyCastle(OCSPVerifier verifier) {
            this.verifier = verifier;
        }

        /// <summary>Gets OCSP response.</summary>
        /// <remarks>
        /// Gets OCSP response. If
        /// <see cref="OCSPVerifier"/>
        /// was set, the response will be checked.
        /// </remarks>
        /// <param name="checkCert">to certificate to check</param>
        /// <param name="rootCert">the parent certificate</param>
        /// <param name="url">to get the verification</param>
        /// <returns>
        /// 
        /// <see cref="iText.Commons.Bouncycastle.Asn1.Ocsp.IBasicOcspResponse"/>
        /// an OCSP response wrapper
        /// </returns>
        public virtual IBasicOcspResponse GetBasicOCSPResp(IX509Certificate checkCert, IX509Certificate rootCert, 
            String url) {
            try {
                IOcspResponse ocspResponse = GetOcspResponse(checkCert, rootCert, url);
                if (ocspResponse == null) {
                    return null;
                }
                if (ocspResponse.GetStatus() != BOUNCY_CASTLE_FACTORY.CreateOCSPResponseStatus().GetSuccessful()) {
                    return null;
                }
                IBasicOcspResponse basicResponse = BOUNCY_CASTLE_FACTORY.CreateBasicOCSPResponse(ocspResponse.GetResponseObject
                    ());
                if (verifier != null) {
                    verifier.IsValidResponse(basicResponse, rootCert, DateTimeUtil.GetCurrentUtcTime());
                }
                return basicResponse;
            }
            catch (Exception ex) {
                LOGGER.LogError(ex.Message);
            }
            return null;
        }

        /// <summary><inheritDoc/></summary>
        public virtual byte[] GetEncoded(IX509Certificate checkCert, IX509Certificate rootCert, String url) {
            try {
                IBasicOcspResponse basicResponse = GetBasicOCSPResp(checkCert, rootCert, url);
                if (basicResponse != null) {
                    ISingleResponse[] responses = basicResponse.GetResponses();
                    if (responses.Length == 1) {
                        ISingleResponse resp = responses[0];
                        ICertStatus status = resp.GetCertStatus();
                        if (Object.Equals(status, BOUNCY_CASTLE_FACTORY.CreateCertificateStatus().GetGood())) {
                            return basicResponse.GetEncoded();
                        }
                        else {
                            if (BOUNCY_CASTLE_FACTORY.CreateRevokedStatus(status) == null) {
                                throw new System.IO.IOException(iText.IO.Logs.IoLogMessageConstant.OCSP_STATUS_IS_UNKNOWN);
                            }
                            else {
                                throw new System.IO.IOException(iText.IO.Logs.IoLogMessageConstant.OCSP_STATUS_IS_REVOKED);
                            }
                        }
                    }
                }
            }
            catch (Exception ex) {
                LOGGER.LogError(ex.Message);
            }
            return null;
        }

        /// <summary>Generates an OCSP request using BouncyCastle.</summary>
        /// <param name="issuerCert">certificate of the issues</param>
        /// <param name="serialNumber">serial number</param>
        /// <returns>
        /// 
        /// <see cref="iText.Commons.Bouncycastle.Cert.Ocsp.IOcspRequest"/>
        /// an OCSP request wrapper
        /// </returns>
        protected internal static IOcspRequest GenerateOCSPRequest(IX509Certificate issuerCert, IBigInteger serialNumber
            ) {
            //Add provider BC
            // Generate the id for the certificate we are looking for
            ICertID id = SignUtils.GenerateCertificateId(issuerCert, serialNumber, BOUNCY_CASTLE_FACTORY.CreateCertificateID
                ().GetHashSha1());
            // basic request generation with nonce
            return SignUtils.GenerateOcspRequestWithNonce(id);
        }

        /// <summary>Gets an OCSP response object using BouncyCastle.</summary>
        /// <param name="checkCert">to certificate to check</param>
        /// <param name="rootCert">the parent certificate</param>
        /// <param name="url">
        /// to get the verification. If it's null it will be taken
        /// from the check cert or from other implementation specific source
        /// </param>
        /// <returns>
        /// 
        /// <see cref="iText.Commons.Bouncycastle.Asn1.Ocsp.IOcspResponse"/>
        /// an OCSP response wrapper
        /// </returns>
        internal virtual IOcspResponse GetOcspResponse(IX509Certificate checkCert, IX509Certificate rootCert, String
             url) {
            if (checkCert == null || rootCert == null) {
                return null;
            }
            if (url == null) {
                url = CertificateUtil.GetOCSPURL(checkCert);
            }
            if (url == null) {
                return null;
            }
            Stream @in = CreateRequestAndResponse(checkCert, rootCert, url);
            return @in == null ? null : BOUNCY_CASTLE_FACTORY.CreateOCSPResponse(StreamUtil.InputStreamToArray(@in));
        }

        /// <summary>
        /// Create OCSP request and get the response for this request, represented as
        /// <see cref="System.IO.Stream"/>.
        /// </summary>
        /// <param name="checkCert">
        /// 
        /// <see cref="iText.Commons.Bouncycastle.Cert.IX509Certificate"/>
        /// certificate to get OCSP response for
        /// </param>
        /// <param name="rootCert">
        /// 
        /// <see cref="iText.Commons.Bouncycastle.Cert.IX509Certificate"/>
        /// root certificate from which OCSP request will be built
        /// </param>
        /// <param name="url">
        /// 
        /// <see cref="System.Uri"/>
        /// link, which is expected to be used to get OCSP response from
        /// </param>
        /// <returns>
        /// OCSP response bytes, represented as
        /// <see cref="System.IO.Stream"/>
        /// </returns>
        protected internal virtual Stream CreateRequestAndResponse(IX509Certificate checkCert, IX509Certificate rootCert
            , String url) {
            LOGGER.LogInformation("Getting OCSP from " + url);
            IOcspRequest request = GenerateOCSPRequest(rootCert, checkCert.GetSerialNumber());
            byte[] array = request.GetEncoded();
            Uri urlt = new Uri(url);
            return SignUtils.GetHttpResponseForOcspRequest(array, urlt);
        }
    }
}
