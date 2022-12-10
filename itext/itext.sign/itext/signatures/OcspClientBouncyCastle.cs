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
    /// <author>Paulo Soarees</author>
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
        /// <see cref="iText.Commons.Bouncycastle.Asn1.Ocsp.IBasicOCSPResponse"/>
        /// an OCSP response wrapper
        /// </returns>
        public virtual IBasicOCSPResponse GetBasicOCSPResp(IX509Certificate checkCert, IX509Certificate rootCert, 
            String url) {
            try {
                IOCSPResponse ocspResponse = GetOcspResponse(checkCert, rootCert, url);
                if (ocspResponse == null) {
                    return null;
                }
                if (ocspResponse.GetStatus() != BOUNCY_CASTLE_FACTORY.CreateOCSPResponseStatus().GetSuccessful()) {
                    return null;
                }
                IBasicOCSPResponse basicResponse = BOUNCY_CASTLE_FACTORY.CreateBasicOCSPResponse(ocspResponse.GetResponseObject
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
                IBasicOCSPResponse basicResponse = GetBasicOCSPResp(checkCert, rootCert, url);
                if (basicResponse != null) {
                    ISingleResp[] responses = basicResponse.GetResponses();
                    if (responses.Length == 1) {
                        ISingleResp resp = responses[0];
                        ICertificateStatus status = resp.GetCertStatus();
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
        /// <see cref="iText.Commons.Bouncycastle.Cert.Ocsp.IOCSPReq"/>
        /// an OCSP request wrapper
        /// </returns>
        private static IOCSPReq GenerateOCSPRequest(IX509Certificate issuerCert, IBigInteger serialNumber) {
            //Add provider BC
            // Generate the id for the certificate we are looking for
            ICertificateID id = SignUtils.GenerateCertificateId(issuerCert, serialNumber, BOUNCY_CASTLE_FACTORY.CreateCertificateID
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
        /// <see cref="iText.Commons.Bouncycastle.Asn1.Ocsp.IOCSPResponse"/>
        /// an OCSP response wrapper
        /// </returns>
        internal virtual IOCSPResponse GetOcspResponse(IX509Certificate checkCert, IX509Certificate rootCert, String
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
            LOGGER.LogInformation("Getting OCSP from " + url);
            IOCSPReq request = GenerateOCSPRequest(rootCert, checkCert.GetSerialNumber());
            byte[] array = request.GetEncoded();
            Uri urlt = new Uri(url);
            Stream @in = SignUtils.GetHttpResponseForOcspRequest(array, urlt);
            return BOUNCY_CASTLE_FACTORY.CreateOCSPResponse(StreamUtil.InputStreamToArray(@in));
        }
    }
}
