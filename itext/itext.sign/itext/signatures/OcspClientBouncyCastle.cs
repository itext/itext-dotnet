/*

This file is part of the iText (R) project.
Copyright (c) 1998-2022 iText Group NV
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
using System.IO;
using Microsoft.Extensions.Logging;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Ocsp;
using Org.BouncyCastle.X509;
using iText.Commons;
using iText.Commons.Utils;
using iText.IO.Util;

namespace iText.Signatures {
    /// <summary>OcspClient implementation using BouncyCastle.</summary>
    /// <author>Paulo Soarees</author>
    public class OcspClientBouncyCastle : IOcspClient {
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
                                throw new System.IO.IOException(iText.IO.Logs.IoLogMessageConstant.OCSP_STATUS_IS_REVOKED);
                            }
                            else {
                                throw new System.IO.IOException(iText.IO.Logs.IoLogMessageConstant.OCSP_STATUS_IS_UNKNOWN);
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
        /// to get the verification. If it's null it will be taken
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
            LOGGER.LogInformation("Getting OCSP from " + url);
            OcspReq request = GenerateOCSPRequest(rootCert, checkCert.SerialNumber);
            byte[] array = request.GetEncoded();
            Uri urlt = new Uri(url);
            Stream @in = SignUtils.GetHttpResponseForOcspRequest(array, urlt);
            return new OcspResp(StreamUtil.InputStreamToArray(@in));
        }
    }
}
