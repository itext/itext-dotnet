using System;
using System.Collections.Generic;
using Org.BouncyCastle.Asn1.Ocsp;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Ocsp;
using Org.BouncyCastle.X509;
using iText.IO.Util;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Crypto.Operators;
using Org.BouncyCastle.Security.Certificates;

namespace iText.Signatures.Testutils.Builder {
    public class TestOcspResponseBuilder {
        private const String SIGN_ALG = "SHA256withRSA";

        private BasicOcspRespGenerator responseBuilder;

        private X509Certificate caCert;

        private CertificateStatus certificateStatus = CertificateStatus.Good;

        private DateTime thisUpdate = DateTimeUtil.GetCurrentTime();

        private DateTime nextUpdate = DateTimeUtil.GetCurrentTime();

        /// <exception cref="Org.BouncyCastle.Security.Certificates.CertificateEncodingException"/>
        public TestOcspResponseBuilder(X509Certificate caCert) {
            this.caCert = caCert;
            X509Name issuerDN = caCert.IssuerDN;
            thisUpdate = thisUpdate.AddDays(-1);
            nextUpdate = nextUpdate.AddDays(30);
            responseBuilder = new BasicOcspRespGenerator(new RespID(issuerDN));
        }

        public virtual void SetCertificateStatus(CertificateStatus certificateStatus) {
            this.certificateStatus = certificateStatus;
        }

        public virtual void SetThisUpdate(DateTime thisUpdate) {
            this.thisUpdate = thisUpdate;
        }

        public virtual void SetNextUpdate(DateTime nextUpdate) {
            this.nextUpdate = nextUpdate;
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="CertificateException"/>
        /// <exception cref="Org.BouncyCastle.Operator.OperatorCreationException"/>
        /// <exception cref="Org.BouncyCastle.Ocsp.OcspException"/>
        public virtual byte[] MakeOcspResponse(byte[] requestBytes, ICipherParameters caPrivateKey) {
            OcspReq ocspRequest = new OcspReq(requestBytes);
            Req[] requestList = ocspRequest.GetRequestList();

            X509Extension extNonce = ocspRequest.RequestExtensions.GetExtension(OcspObjectIdentifiers.PkixOcspNonce);
            if (extNonce != null) {
                // TODO ensure
                X509Extensions responseExtensions = new X509Extensions(new Dictionary<DerObjectIdentifier, X509Extension>() { { OcspObjectIdentifiers.PkixOcspNonce, extNonce }});
                responseBuilder.SetResponseExtensions(responseExtensions);
            }

            foreach (Req req in requestList) {
                responseBuilder.AddResponse(req.GetCertID(), certificateStatus, thisUpdate.ToUniversalTime(), nextUpdate.ToUniversalTime(), null);
            }
            DateTime time = DateTimeUtil.GetCurrentUtcTime();
            BasicOcspResp ocspResponse = responseBuilder.Generate(new Asn1SignatureFactory(SIGN_ALG, (AsymmetricKeyParameter)caPrivateKey), new X509Certificate[] { caCert }, time);
            // return new OCSPRespBuilder().build(ocspResult, ocspResponse).getEncoded();
            return ocspResponse.GetEncoded();
        }
    }
}
