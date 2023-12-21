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
using System.Linq;
using System.Net;
using System.Text;
using iText.Bouncycastleconnector;
using iText.Commons.Bouncycastle;
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.Ocsp;
using iText.Commons.Bouncycastle.Asn1.Tsp;
using iText.Commons.Bouncycastle.Asn1.X500;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Cert.Ocsp;
using iText.Commons.Bouncycastle.Crypto;
using iText.Commons.Bouncycastle.Math;
using iText.Commons.Bouncycastle.Tsp;
using iText.Kernel.Exceptions;
using iText.Kernel.Pdf;
using iText.Signatures.Exceptions;

namespace iText.Signatures {
    internal sealed class SignUtils {
        private static readonly IBouncyCastleFactory FACTORY = BouncyCastleFactoryCreator.GetFactory();

        internal static String GetPrivateKeyAlgorithm(IPrivateKey privateKey) {
            String algorithm = privateKey.GetAlgorithm();
            if (algorithm == null) {
                throw new PdfException(SignExceptionMessageConstant.COULD_NOT_DETERMINE_SIGNATURE_MECHANISM_OID).SetMessageParams(algorithm, privateKey.GetHashCode());
            }

            return algorithm;
        }

        /// <summary>
        /// Parses a CRL from an input Stream.
        /// </summary>
        /// <param name="input">The input Stream holding the unparsed CRL.</param>
        /// <returns>The parsed CRL object.</returns>
        internal static IX509Crl ParseCrlFromStream(Stream input) {
            return FACTORY.CreateX509Crl(input);
        }

        internal static IX509Crl ParseCrlFromUrl(String crlurl) {
            // Creates the CRL
            Stream url = WebRequest.Create(crlurl).GetResponse().GetResponseStream();
            return ParseCrlFromStream(url);
        }

        internal static byte[] GetExtensionValueByOid(IX509Certificate certificate, String oid) {
            IAsn1OctetString extensionValue = certificate.GetExtensionValue(oid);
            return extensionValue.IsNull() ? null : extensionValue.GetDerEncoded();
        }

        internal static byte[] GetExtensionValueByOid(IX509Crl crl, String oid) {
            IAsn1OctetString extensionValue = crl.GetExtensionValue(oid);;
            return extensionValue.IsNull() ? null : extensionValue.GetDerEncoded();
        }

        internal static IDigest GetMessageDigest(String hashAlgorithm) {
            return FACTORY.CreateIDigest(hashAlgorithm);
        }

        internal static Stream GetHttpResponse(Uri urlt) {
            HttpWebRequest con = (HttpWebRequest) WebRequest.Create(urlt);
            HttpWebResponse response = (HttpWebResponse) con.GetResponse();
            if (response.StatusCode != HttpStatusCode.OK)
                throw new PdfException(
                    SignExceptionMessageConstant.INVALID_HTTP_RESPONSE).SetMessageParams(response.StatusCode);
            return response.GetResponseStream();
        }

        internal static ICertID GenerateCertificateId(IX509Certificate issuerCert, IBigInteger serialNumber, String hashAlgorithm) {
            return FACTORY.CreateCertificateID(hashAlgorithm, issuerCert, serialNumber);
        }

        internal static Stream GetHttpResponseForOcspRequest(byte[] request, Uri urlt) {
            HttpWebRequest con = (HttpWebRequest) WebRequest.Create(urlt);
            con.ContentLength = request.Length;
            con.ContentType = "application/ocsp-request";
            con.Accept = "application/ocsp-response";
            con.Method = "POST";
            Stream outp = con.GetRequestStream();
            outp.Write(request, 0, request.Length);
            outp.Dispose();
            HttpWebResponse response = (HttpWebResponse) con.GetResponse();
            if (response.StatusCode != HttpStatusCode.OK)
                throw new PdfException(
                    SignExceptionMessageConstant.INVALID_HTTP_RESPONSE).SetMessageParams(response.StatusCode);

            return response.GetResponseStream();
        }

        internal static IOcspRequest GenerateOcspRequestWithNonce(ICertID id) {
            return FACTORY.CreateOCSPReq(id, PdfEncryption.GenerateNewDocumentId());
        }

        internal static bool IsSignatureValid(IBasicOcspResponse validator, IX509Certificate certStoreX509) {
            return validator.Verify(certStoreX509);
        }

        internal static void IsSignatureValid(ITimeStampToken validator, IX509Certificate certStoreX509) {
            validator.Validate(certStoreX509);
        }

        internal static bool CheckIfIssuersMatch(ICertID certificateID, IX509Certificate issuerCert) {
            return certificateID.MatchesIssuer(issuerCert);
        }

        internal static DateTime Add180Sec(DateTime date) {
            return date.AddSeconds(180);
        }

        internal static IEnumerable<IX509Certificate> GetCertsFromOcspResponse(IBasicOcspResponse ocspResp) {
            return ocspResp.GetCerts();
        }

        internal static List<IX509Certificate> ReadAllCerts(byte[] contentsKey) {
            List<IX509Certificate> certificates = FACTORY.CreateX509CertificateParser().ReadAllCerts(contentsKey);
            if (certificates.IsEmpty()) {
                using (MemoryStream data = new MemoryStream(contentsKey)) {
                    certificates = FACTORY.GetBouncyCastleUtil().ReadPkcs7Certs(data);
                }
            }

            return certificates;
        }
        
        internal static List<IX509Certificate> ReadAllCerts(Stream data) {
            using (MemoryStream bout = new MemoryStream()) {
                byte[] buf = new byte[1024];
                while (true) {
                    int n = data.JRead(buf, 0, buf.Length);
                    if (n <= 0) {
                        break;
                    }
                    bout.Write(buf, 0, n);
                }
                byte[] certsData = bout.ToArray();
                return ReadAllCerts(certsData);
            }
        }

        internal static T GetFirstElement<T>(IEnumerable<T> enumerable) {
            return enumerable.First();
        }

        internal static IX500Name GetIssuerX500Principal(IAsn1Sequence issuerAndSerialNumber) {
            return FACTORY.CreateX500NameInstance(issuerAndSerialNumber.GetObjectAt(0));
        }

        internal class TsaResponse {
            internal String encoding;
            internal Stream tsaResponseStream;
        }

        internal static TsaResponse GetTsaResponseForUserRequest(String tsaUrl, byte[] requestBytes, String tsaUsername, String tsaPassword) {
            HttpWebRequest con;
            try {
                con = (HttpWebRequest) WebRequest.Create(tsaUrl);
            } catch (Exception e) {
                throw new PdfException(SignExceptionMessageConstant.FAILED_TO_GET_TSA_RESPONSE).SetMessageParams(tsaUrl);
            }
            con.ContentLength = requestBytes.Length;
            con.ContentType = "application/timestamp-query";
            con.Method = "POST";
            if ((tsaUsername != null) && !tsaUsername.Equals("")) {
                string authInfo = tsaUsername + ":" + tsaPassword;
                authInfo = Convert.ToBase64String(Encoding.UTF8.GetBytes(authInfo));
                con.Headers["Authorization"] = "Basic " + authInfo;
            }
            Stream outp = con.GetRequestStream();
            outp.Write(requestBytes, 0, requestBytes.Length);
            outp.Dispose();
            HttpWebResponse httpWebResponse = (HttpWebResponse) con.GetResponse();

            TsaResponse response = new TsaResponse();
            response.tsaResponseStream = httpWebResponse.GetResponseStream();
            response.encoding = httpWebResponse.Headers[HttpResponseHeader.ContentEncoding];
            return response;
        }
        
        /// <summary>
        /// This behavior is different in Java and .NET, because in Java we use this two-step check:
        /// first via #hasUnsupportedCriticalExtension method, and then additionally allowing standard critical extensions;
        /// in .NET there's only second step. However, removing first step in Java can be a breaking change for some users
        /// and moreover we don't have any means of providing customization for unsupported extensions check as of right now.
        ///
        /// During major release I'd suggest changing java unsupported extensions check logic to the same as in .NET,
        /// but only if it is possible to customize this logic.
        /// </summary>
        /// <param name="cert"></param>
        /// <returns></returns>
        /// TODO DEVSIX-2634
        [Obsolete]
        internal static bool HasUnsupportedCriticalExtension(IX509Certificate cert) {
            if ( cert == null ) {
                throw new ArgumentException("X509Certificate can't be null.");
            }

            ISet<string> criticalExtensionsSet = cert.GetCriticalExtensionOids();
            if (criticalExtensionsSet != null) {
                foreach (String oid in criticalExtensionsSet) {
                    if (OID.X509Extensions.SUPPORTED_CRITICAL_EXTENSIONS.Contains(oid)) {
                        continue;
                        
                    }
                    return true;
                }
            }
            return false;
        }

        internal static DateTime GetTimeStampDate(ITstInfo timeStampTokenInfo) {
            return timeStampTokenInfo.GetGenTime();
        }

        internal static ISigner GetSignatureHelper(String algorithm) {
            ISigner signer = FACTORY.CreateISigner();
            signer.SetDigestAlgorithm(algorithm);
            return signer;
        }

        internal static bool VerifyCertificateSignature(IX509Certificate certificate, IPublicKey issuerPublicKey) {
            bool res = false;
            try {
                certificate.Verify(issuerPublicKey);
                res = true;
            } catch (Exception ignored) {
            }
            return res;
        }

        internal static IEnumerable<IX509Certificate> GetCertificates(List<IX509Certificate> rootStore) {
            return rootStore;
        }

        internal static void SetRSASSAPSSParamsWithMGF1(ISigner signature, String digestAlgoName, int saltLen, int trailerField)
        {
         //     var mgf1Spec = new MgfParameters() MGF1ParameterSpec(digestAlgoName);
         //    PSSParameterSpec spec = new Pss  PSSParameterSpec(digestAlgoName, "MGF1", mgf1Spec, saltLen, trailerField);
         // signature.  setParameter(spec);
         }
        
        internal static void UpdateVerifier(ISigner sig, byte[] digest) {
            sig.UpdateVerifier(digest);
        }

        public static ICertID GenerateCertificateId(IX509Certificate issuerCert, IBigInteger serialNumber, 
            IDerObjectIdentifier hashAlgOid) {
            return GenerateCertificateId(issuerCert, serialNumber, hashAlgOid.GetId());
        }

        public static IX509Certificate GenerateCertificate(Stream data) {
            return FACTORY.CreateX509Certificate(data);
        }
    }
}
