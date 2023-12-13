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
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.X509;
using iText.IO.Util;
using Org.BouncyCastle.Crypto.Operators;

namespace iText.Signatures.Testutils.Builder {
    public class TestCrlBuilder {
        private const String SIGN_ALG = "SHA256withRSA";

        private X509V2CrlGenerator crlBuilder;

        private DateTime nextUpdate = DateTimeUtil.GetCurrentUtcTime().AddDays(30);

        public TestCrlBuilder(X509Certificate caCert, DateTime thisUpdate) {
            X509Name issuerDN = caCert.IssuerDN;
            crlBuilder = new X509V2CrlGenerator();
            crlBuilder.SetIssuerDN(issuerDN);
            crlBuilder.SetThisUpdate(thisUpdate);
        }

        public virtual void SetNextUpdate(DateTime nextUpdate) {
            this.nextUpdate = nextUpdate;
        }

        /// <summary>See CRLReason</summary>
        public virtual void AddCrlEntry(X509Certificate certificate, DateTime revocationDate, int reason) {
            crlBuilder.AddCrlEntry(certificate.SerialNumber, revocationDate, reason);
        }

        public virtual byte[] MakeCrl(ICipherParameters caPrivateKey) {
            crlBuilder.SetNextUpdate(nextUpdate);
            X509Crl crl = crlBuilder.Generate(new Asn1SignatureFactory(SIGN_ALG, (AsymmetricKeyParameter) caPrivateKey));
            return crl.GetEncoded();
        }
    }
}
