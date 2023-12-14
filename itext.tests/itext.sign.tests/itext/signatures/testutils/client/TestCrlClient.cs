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
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.X509;
using iText.IO.Util;
using iText.Kernel;
using iText.Signatures;
using iText.Signatures.Testutils.Builder;

namespace iText.Signatures.Testutils.Client {
    public class TestCrlClient : ICrlClient {
        private readonly TestCrlBuilder crlBuilder;

        private readonly ICipherParameters caPrivateKey;

        public TestCrlClient(TestCrlBuilder crlBuilder, ICipherParameters caPrivateKey) {
            this.crlBuilder = crlBuilder;
            this.caPrivateKey = caPrivateKey;
        }

        public TestCrlClient(X509Certificate caCert, ICipherParameters caPrivateKey) {
            this.crlBuilder = new TestCrlBuilder(caCert, DateTimeUtil.GetCurrentUtcTime().AddDays(-1));
            this.caPrivateKey = caPrivateKey;
        }

        public virtual ICollection<byte[]> GetEncoded(X509Certificate checkCert, String url) {
            ICollection<byte[]> crls = null;
            try {
                byte[] crl = crlBuilder.MakeCrl(caPrivateKey);
                crls = JavaCollectionsUtil.SingletonList(crl);
            }
            catch (Exception ignore) {
                throw new PdfException(ignore);
            }
            return crls;
        }
    }
}
