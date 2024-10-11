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
using iText.Bouncycastleconnector;
using iText.Commons.Bouncycastle;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Crypto;
using iText.Commons.Bouncycastle.Math;
using iText.Commons.Bouncycastle.Tsp;
using iText.Commons.Utils;
using iText.Kernel.Crypto;

namespace iText.Signatures.Testutils.Builder {
    public class TestTimestampTokenBuilder {
        private static readonly IBouncyCastleFactory FACTORY = BouncyCastleFactoryCreator.GetFactory();
        private IList<IX509Certificate> tsaCertificateChain;
        
        // just a more or less random oid of timestamp policy
        private static readonly String POLICY_OID = "1.3.6.1.4.1.45794.1.1";

        private IPrivateKey tsaPrivateKey;

        public TestTimestampTokenBuilder(IList<IX509Certificate> tsaCertificateChain, IPrivateKey tsaPrivateKey
            ) {
            if (tsaCertificateChain.Count == 0) {
                throw new ArgumentException("tsaCertificateChain shall not be empty");
            }
            this.tsaCertificateChain = tsaCertificateChain;
            this.tsaPrivateKey = tsaPrivateKey;
        }

        public virtual byte[] CreateTimeStampToken(ITimeStampRequest request) {
            ITimeStampTokenGenerator tsTokGen = CreateTimeStampTokenGenerator(tsaPrivateKey, tsaCertificateChain[0], 
                "SHA1", POLICY_OID);
            tsTokGen.SetAccuracySeconds(1);

            tsTokGen.SetCertificates(tsaCertificateChain);
            // should be unique for every timestamp
            IBigInteger serialNumber = FACTORY.CreateBigInteger(SystemUtil.GetTimeBasedSeed().ToString());
            DateTime genTime = DateTimeUtil.GetCurrentUtcTime();
            ITimeStampToken tsToken = tsTokGen.Generate(request, serialNumber, genTime);
            return tsToken.GetEncoded();
        }
        
        public virtual byte[] CreateTSAResponse(byte[] requestBytes, String signatureAlgorithm, String allowedDigest) {
            try {
                String digestForTsSigningCert = DigestAlgorithms.GetAllowedDigest(allowedDigest);
                ITimeStampTokenGenerator tokenGenerator = CreateTimeStampTokenGenerator(tsaPrivateKey,
                    tsaCertificateChain[0], allowedDigest, POLICY_OID);

                IList<String> algorithms = new List<string>();
                algorithms.Add(digestForTsSigningCert);
                ITimeStampResponseGenerator generator = FACTORY.CreateTimeStampResponseGenerator(tokenGenerator, (IList)algorithms);
                ITimeStampRequest request = FACTORY.CreateTimeStampRequest(requestBytes);
                return generator.Generate(request, request.GetNonce(), new DateTime()).GetEncoded();
            } catch (Exception e) {
                return null;
            }
        }

        private static ITimeStampTokenGenerator CreateTimeStampTokenGenerator(IPrivateKey pk, IX509Certificate cert,
            String allowedDigest, String policyOid) {
            return FACTORY.CreateTimeStampTokenGenerator(pk, cert,
                DigestAlgorithms.GetDigest(DigestAlgorithms.GetAllowedDigest(allowedDigest)), policyOid);
        }

    }
}
