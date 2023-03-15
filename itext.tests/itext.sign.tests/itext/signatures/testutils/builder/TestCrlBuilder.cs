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
using iText.Bouncycastleconnector;
using iText.Commons.Bouncycastle;
using iText.Commons.Bouncycastle.Asn1.X500;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Crypto;
using iText.Commons.Utils;

namespace iText.Signatures.Testutils.Builder {
    public class TestCrlBuilder {
        private static readonly IBouncyCastleFactory FACTORY = BouncyCastleFactoryCreator.GetFactory();
        private const String SIGN_ALG = "SHA256withRSA";

        private readonly IPrivateKey issuerPrivateKey;
        private readonly IX509v2CRLBuilder crlBuilder;

        private DateTime nextUpdate = DateTimeUtil.GetCurrentUtcTime().AddDays(30);

        public TestCrlBuilder(IX509Certificate issuerCert, IPrivateKey issuerPrivateKey, DateTime thisUpdate) {
            IX500Name issuerCertSubjectDn = issuerCert.GetSubjectDN();
            this.crlBuilder = FACTORY.CreateX509v2CRLBuilder(issuerCertSubjectDn, thisUpdate);
            this.issuerPrivateKey = issuerPrivateKey;
        }

        public virtual void SetNextUpdate(DateTime nextUpdate) {
            this.nextUpdate = nextUpdate;
        }

        /// <summary>See CRLReason</summary>
        public virtual void AddCrlEntry(IX509Certificate certificate, DateTime revocationDate, int reason) {
            crlBuilder.AddCRLEntry(certificate.GetSerialNumber(), revocationDate, reason);
        }

        public virtual byte[] MakeCrl() {
            crlBuilder.SetNextUpdate(nextUpdate);
            IX509Crl crl = crlBuilder.Build(FACTORY.CreateContentSigner(SIGN_ALG, issuerPrivateKey));
            return crl.GetEncoded();
        }
    }
}
