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
using System.IO;
using Common.Logging;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Ocsp;
using Org.BouncyCastle.X509;
using iText.IO.Util;

namespace iText.Signatures {
    /// <summary>OcspClient implementation using BouncyCastle.</summary>
    /// <author>Paulo Soarees</author>
    public class OcspClientBouncyCastle : IOcspClient {
        /// <summary>The Logger instance.</summary>
        private static readonly ILog LOGGER = LogManager.GetLogger(typeof(iText.Signatures.OcspClientBouncyCastle)
            );

        private readonly OCSPVerifier verifier;

        /// <summary>
        /// Create
        /// <c>OcspClient</c>
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
        /// <returns>OCSP response</returns>
        public virtual BasicOcspResp GetBasicOCSPResp(X509Certificate checkCert, X509Certificate rootCert, String 
            url) {
            try {
                OcspResp ocspResponse = GetOcspResponse(checkCert, rootCert, url);
                if (ocspResponse == null) {
                    return null;
                }
                if (ocspResponse.Status != Org.BouncyCastle.Asn1.Ocsp.OcspResponseStatus.Successful) {
                    return null;
                }
                BasicOcspResp basicResponse = (BasicOcspResp)ocspResponse.GetResponseObject();
                if (verifier != null) {
                    verifier.IsValidResponse(basicResponse, rootCert);
                }
                return basicResponse;
            }
            catch (Exception ex) {
                LOGGER.Error(ex.Message);
            }
            return null;
        }

        /// <summary><inheritDoc/></summary>
        public virtual byte[] GetEncoded(X509Certificate checkCert, X509Certificate rootCert, String url) {
            try {
                BasicOcspResp basicResponse = GetBasicOCSPResp(checkCert, rootCert, url);
                if (basicResponse != null) {
                    SingleResp[] responses = basicResponse.Responses;
                    if (responses.Length == 1) {
                        SingleResp resp = responses[0];
                        Object status = resp.GetCertStatus();
                        if (status == CertificateStatus.Good) {
                            return basicResponse.GetEncoded();
                        }
                        else {
                            if (status is RevokedStatus) {
                                throw new System.IO.IOException(iText.IO.LogMessageConstant.OCSP_STATUS_IS_REVOKED);
                            }
                            else {
                                throw new System.IO.IOException(iText.IO.LogMessageConstant.OCSP_STATUS_IS_UNKNOWN);
                            }
                        }
                    }
                }
            }
            catch (Exception ex) {
                LOGGER.Error(ex.Message);
            }
            return null;
        }

        /// <summary>Generates an OCSP request using BouncyCastle.</summary>
        /// <param name="issuerCert">certificate of the issues</param>
        /// <param name="serialNumber">serial number</param>
        /// <returns>an OCSP request</returns>
        private static OcspReq GenerateOCSPRequest(X509Certificate issuerCert, BigInteger serialNumber) {
            //Add provider BC
            // Generate the id for the certificate we are looking for
            CertificateID id = SignUtils.GenerateCertificateId(issuerCert, serialNumber, Org.BouncyCastle.Ocsp.CertificateID.HashSha1
                );
            // basic request generation with nonce
            return SignUtils.GenerateOcspRequestWithNonce(id);
        }

        /// <summary>Gets an OCSP response object using BouncyCastle.</summary>
        /// <param name="checkCert">to certificate to check</param>
        /// <param name="rootCert">the parent certificate</param>
        /// <param name="url">
        /// to get the verification. It it's null it will be taken
        /// from the check cert or from other implementation specific source
        /// </param>
        /// <returns>an OCSP response</returns>
        internal virtual OcspResp GetOcspResponse(X509Certificate checkCert, X509Certificate rootCert, String url) {
            if (checkCert == null || rootCert == null) {
                return null;
            }
            if (url == null) {
                url = CertificateUtil.GetOCSPURL(checkCert);
            }
            if (url == null) {
                return null;
            }
            LOGGER.Info("Getting OCSP from " + url);
            OcspReq request = GenerateOCSPRequest(rootCert, checkCert.SerialNumber);
            byte[] array = request.GetEncoded();
            Uri urlt = new Uri(url);
            Stream @in = SignUtils.GetHttpResponseForOcspRequest(array, urlt);
            return new OcspResp(StreamUtil.InputStreamToArray(@in));
        }
    }
}
