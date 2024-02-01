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
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Crypto;
using iText.Commons.Bouncycastle.Operator;
using iText.Commons.Utils;
using iText.Signatures;
using iText.Signatures.Testutils;
using iText.Signatures.Testutils.Builder;

namespace iText.Signatures.Testutils.Client {
    public class AdvancedTestCrlClient : CrlClientOnline {
        private readonly IDictionary<String, TestCrlBuilder> crlBuilders = new LinkedDictionary<String, TestCrlBuilder
            >();

        public virtual AdvancedTestCrlClient AddBuilderForCertIssuer(IX509Certificate cert, TestCrlBuilder crlBuilder
            ) {
            crlBuilders.Put(cert.GetSerialNumber().ToString(16), crlBuilder);
            return this;
        }

        public virtual AdvancedTestCrlClient AddBuilderForCertIssuer(IX509Certificate cert, IX509Certificate issuerCert
            , IPrivateKey issuerPrivateKey) {
            DateTime yesterday = TimeTestUtil.TEST_DATE_TIME.AddDays(-1);
            crlBuilders.Put(cert.GetSerialNumber().ToString(16), new TestCrlBuilder(issuerCert, issuerPrivateKey, yesterday
                ));
            return this;
        }

        protected internal override Stream GetCrlResponse(IX509Certificate cert, Uri urlt) {
            TestCrlBuilder builder = crlBuilders.Get(cert.GetSerialNumber().ToString(16));
            try {
                return new MemoryStream(builder.MakeCrl());
            }
            catch (AbstractOperatorCreationException e) {
                throw new Exception(e.Message);
            }
        }
    }
}
