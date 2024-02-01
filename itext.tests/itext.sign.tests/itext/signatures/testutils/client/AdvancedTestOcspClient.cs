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
using System.IO;
using iText.Bouncycastleconnector;
using iText.Commons.Bouncycastle;
using iText.Commons.Bouncycastle.Asn1.Ocsp;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Cert.Ocsp;
using iText.Commons.Bouncycastle.Crypto;
using iText.Commons.Bouncycastle.Security;
using iText.Commons.Utils;
using iText.Signatures;
using iText.Signatures.Testutils.Builder;

namespace iText.Signatures.Testutils.Client {
    public class AdvancedTestOcspClient : OcspClientBouncyCastle {
        private static readonly IBouncyCastleFactory BOUNCY_CASTLE_FACTORY = BouncyCastleFactoryCreator.GetFactory
            ();

        private readonly IDictionary<String, TestOcspResponseBuilder> subjectNameToResponseBuilder = new LinkedDictionary
            <String, TestOcspResponseBuilder>();

        public AdvancedTestOcspClient(OCSPVerifier verifier)
            : base(verifier) {
        }

        protected internal override Stream CreateRequestAndResponse(IX509Certificate checkCert, IX509Certificate rootCert
            , String url) {
            IOcspRequest request = GenerateOCSPRequest(rootCert, checkCert.GetSerialNumber());
            byte[] array = request.GetEncoded();
            TestOcspResponseBuilder builder = subjectNameToResponseBuilder.Get(checkCert.GetSubjectDN().ToString());
            if (builder == null) {
                return null;
            }
            try {
                IOcspResponse resp = BOUNCY_CASTLE_FACTORY.CreateOCSPResponse(BOUNCY_CASTLE_FACTORY.CreateOCSPResponseStatus
                    ().GetSuccessful(), builder.MakeOcspResponseObject(array));
                return new MemoryStream(resp.GetEncoded());
            }
            catch (AbstractGeneralSecurityException e) {
                throw new Exception(e.Message);
            }
        }

        public virtual iText.Signatures.Testutils.Client.AdvancedTestOcspClient AddBuilderForCertIssuer(IX509Certificate
             cert, IX509Certificate signingCert, IPrivateKey privateKey) {
            subjectNameToResponseBuilder.Put(cert.GetSubjectDN().ToString(), new TestOcspResponseBuilder(signingCert, 
                privateKey));
            return this;
        }

        public virtual iText.Signatures.Testutils.Client.AdvancedTestOcspClient AddBuilderForCertIssuer(IX509Certificate
             cert, TestOcspResponseBuilder builder) {
            subjectNameToResponseBuilder.Put(cert.GetSubjectDN().ToString(), builder);
            return this;
        }
    }
}
