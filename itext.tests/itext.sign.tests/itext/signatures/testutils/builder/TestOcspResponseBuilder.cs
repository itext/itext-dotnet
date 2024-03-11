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
using System.Collections.Generic;
using iText.Bouncycastleconnector;
using iText.Commons.Bouncycastle;
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle.Asn1.Ocsp;
using iText.Commons.Bouncycastle.Asn1.X500;
using iText.Commons.Bouncycastle.Asn1.X509;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Cert.Ocsp;
using iText.Commons.Bouncycastle.Crypto;
using iText.Commons.Utils;

namespace iText.Signatures.Testutils.Builder {
    public class TestOcspResponseBuilder {
        private static readonly IBouncyCastleFactory FACTORY = BouncyCastleFactoryCreator.GetFactory();
        private const String SIGN_ALG = "SHA256withRSA";

        private IBasicOcspRespGenerator responseBuilder;

        private IX509Certificate issuerCert;
        private IPrivateKey issuerPrivateKey;

        private ICertStatus certificateStatus;

        private DateTime thisUpdate = TimeTestUtil.TEST_DATE_TIME.AddDays(-1);

        private DateTime nextUpdate = TimeTestUtil.TEST_DATE_TIME.AddDays(30);

        private DateTime producedAt = TimeTestUtil.TEST_DATE_TIME;

        private IX509Certificate[] chain;

        private bool chainSet = false;

        public TestOcspResponseBuilder(IX509Certificate issuerCert, IPrivateKey issuerPrivateKey,
            ICertStatus certificateStatus)
        {
            this.issuerCert = issuerCert;
            this.issuerPrivateKey = issuerPrivateKey;
            this.certificateStatus = certificateStatus;
            IX500Name subjectDN = issuerCert.GetSubjectDN();
            responseBuilder = FACTORY.CreateBasicOCSPRespBuilder(FACTORY.CreateRespID(subjectDN));
        }
        
        public TestOcspResponseBuilder(IX509Certificate issuerCert, IPrivateKey issuerPrivateKey)
            : this(issuerCert, issuerPrivateKey, FACTORY.CreateCertificateStatus().GetGood())
        {
        }

        public IX509Certificate GetIssuerCert() {
            return issuerCert;
        }

        public virtual void SetCertificateStatus(ICertStatus certificateStatus) {
            this.certificateStatus = certificateStatus;
        }

        public virtual void SetThisUpdate(DateTime thisUpdate) {
            this.thisUpdate = thisUpdate;
        }

        public virtual void SetNextUpdate(DateTime nextUpdate) {
            this.nextUpdate = nextUpdate;
        }

        public virtual void SetProducedAt(DateTime producedAt) {
            this.producedAt = producedAt;
        }

        public virtual byte[] MakeOcspResponse(byte[] requestBytes) {
            IBasicOcspResponse ocspResponse = MakeOcspResponseObject(requestBytes);
            return ocspResponse.GetEncoded();
        }
        
        public virtual IBasicOcspResponse MakeOcspResponseObject(byte[] requestBytes) {
            IOcspRequest ocspRequest = FACTORY.CreateOCSPReq(requestBytes);
            IReq[] requestList = ocspRequest.GetRequestList();

            IX509Extension extNonce = ocspRequest.GetExtension(FACTORY.CreateOCSPObjectIdentifiers()
                .GetIdPkixOcspNonce());
            if (!FACTORY.IsNullExtension(extNonce)) {
                // TODO ensure
                IX509Extensions responseExtensions = FACTORY.CreateExtensions(new Dictionary<IDerObjectIdentifier, IX509Extension>() {
                {
                    FACTORY.CreateOCSPObjectIdentifiers().GetIdPkixOcspNonce(), extNonce
                }});
                responseBuilder.SetResponseExtensions(responseExtensions);
            }

            foreach (IReq req in requestList) {
                responseBuilder.AddResponse(req.GetCertID(), certificateStatus, thisUpdate.ToUniversalTime(), nextUpdate.ToUniversalTime(), 
                    FACTORY.CreateExtensions());
            }

            if (!chainSet) {
                chain = new IX509Certificate[] { issuerCert };
            }
            return responseBuilder.Build(FACTORY.CreateContentSigner(SIGN_ALG, issuerPrivateKey), chain, producedAt);
        }

        public void SetOcspCertsChain(IX509Certificate[] ocspCertsChain) {
            chain = ocspCertsChain;
            chainSet = true;
        }
    }
}
