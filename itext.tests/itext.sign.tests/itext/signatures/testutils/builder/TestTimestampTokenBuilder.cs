/*
This file is part of the iText (R) project.
Copyright (c) 1998-2022 iText Group NV
Authors: iText Software.

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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using iText.Commons.Utils;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Cms;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Tsp;
using Org.BouncyCastle.X509;
using iText.IO.Util;
using iText.Signatures;
using Org.BouncyCastle.Crypto.Tls;
using Org.BouncyCastle.X509.Store;

namespace iText.Signatures.Testutils.Builder {
    public class TestTimestampTokenBuilder {
        private IList<X509Certificate> tsaCertificateChain;
        
        // just a more or less random oid of timestamp policy
        private static readonly String POLICY_OID = "1.3.6.1.4.1.45794.1.1";

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
            TimeStampTokenGenerator tsTokGen = CreateTimeStampTokenGenerator(tsaPrivateKey, tsaCertificateChain[0], 
                "SHA1", POLICY_OID);
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
        
        public virtual byte[] CreateTSAResponse(byte[] requestBytes, String signatureAlgorithm, String allowedDigest) {
            try {
                String digestForTsSigningCert = DigestAlgorithms.GetAllowedDigest(allowedDigest);
                TimeStampTokenGenerator tokenGenerator = CreateTimeStampTokenGenerator(tsaPrivateKey,
                    tsaCertificateChain[0], allowedDigest, POLICY_OID);

                IList<String> algorithms = new List<string>();
                algorithms.Add(digestForTsSigningCert);
                TimeStampResponseGenerator generator = new TimeStampResponseGenerator(tokenGenerator, (IList)algorithms);
                TimeStampRequest request = new TimeStampRequest(requestBytes);
                return generator.Generate(request, request.Nonce, new DateTime()).GetEncoded();
            } catch (Exception e) {
                return null;
            }
        }

        private static TimeStampTokenGenerator CreateTimeStampTokenGenerator(ICipherParameters pk, X509Certificate cert,
            String allowedDigest, String policyOid) {
            return new TimeStampTokenGenerator((AsymmetricKeyParameter) pk, cert,
                DigestAlgorithms.GetAllowedDigest(allowedDigest), policyOid);
        }

    }
}
