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

        private X509Certificate issuerCert;
        private ICipherParameters issuerPrivateKey;

        private CertificateStatus certificateStatus = CertificateStatus.Good;

        private DateTime thisUpdate = DateTimeUtil.GetCurrentTime();

        private DateTime nextUpdate = DateTimeUtil.GetCurrentTime();

        public TestOcspResponseBuilder(X509Certificate issuerCert, ICipherParameters issuerPrivateKey) {
            this.issuerCert = issuerCert;
            this.issuerPrivateKey = issuerPrivateKey;
            X509Name subjectDN = issuerCert.SubjectDN;
            thisUpdate = thisUpdate.AddDays(-1);
            nextUpdate = nextUpdate.AddDays(30);
            responseBuilder = new BasicOcspRespGenerator(new RespID(subjectDN));
        }

        public X509Certificate GetIssuerCert() {
            return issuerCert;
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

        public virtual byte[] MakeOcspResponse(byte[] requestBytes) {
            BasicOcspResp ocspResponse = MakeOcspResponseObject(requestBytes);
            return ocspResponse.GetEncoded();
        }
        
        public virtual BasicOcspResp MakeOcspResponseObject(byte[] requestBytes) {
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
            return responseBuilder.Generate(new Asn1SignatureFactory(SIGN_ALG, (AsymmetricKeyParameter)issuerPrivateKey), new X509Certificate[] { issuerCert }, time);
        }
    }
}
