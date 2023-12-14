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
using System.Linq;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Cms;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Tsp;
using Org.BouncyCastle.X509;
using iText.IO.Util;
using iText.Signatures;
using Org.BouncyCastle.X509.Store;

namespace iText.Signatures.Testutils.Builder {
    public class TestTimestampTokenBuilder {
        private IList<X509Certificate> tsaCertificateChain;

        private ICipherParameters tsaPrivateKey;

        public TestTimestampTokenBuilder(IList<X509Certificate> tsaCertificateChain, ICipherParameters tsaPrivateKey
            ) {
            if (tsaCertificateChain.Count == 0) {
                throw new ArgumentException("tsaCertificateChain shall not be empty");
            }
            this.tsaCertificateChain = tsaCertificateChain;
            this.tsaPrivateKey = tsaPrivateKey;
        }

        public virtual byte[] CreateTimeStampToken(TimeStampRequest request) {
            // just a more or less random oid of timestamp policy
            String policy = "1.3.6.1.4.1.45794.1.1";
            TimeStampTokenGenerator tsTokGen = new TimeStampTokenGenerator((AsymmetricKeyParameter) tsaPrivateKey, tsaCertificateChain[0], DigestAlgorithms.GetAllowedDigest("SHA1"), policy);
            tsTokGen.SetAccuracySeconds(1);

            // TODO setting this is somewhat wrong. Acrobat and openssl recognize timestamp tokens generated with this line as corrupted
            // openssl error message: 2304:error:2F09506F:time stamp routines:INT_TS_RESP_VERIFY_TOKEN:tsa name mismatch:ts_rsp_verify.c:476:
            // tsTokGen.setTSA(new GeneralName(new X500Name(PrincipalUtil.getIssuerX509Principal(tsCertificate).getName())));

            tsTokGen.SetCertificates(X509StoreFactory.Create("Certificate/Collection", new X509CollectionStoreParameters(tsaCertificateChain.ToList())));
            // should be unique for every timestamp
            BigInteger serialNumber = new BigInteger(SystemUtil.GetTimeBasedSeed().ToString());
            DateTime genTime = DateTimeUtil.GetCurrentUtcTime();
            TimeStampToken tsToken = tsTokGen.Generate(request, serialNumber, genTime);
            return tsToken.GetEncoded();
        }
    }
}
