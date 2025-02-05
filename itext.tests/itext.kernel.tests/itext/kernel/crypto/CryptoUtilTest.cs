/*
This file is part of the iText (R) project.
Copyright (c) 1998-2025 Apryse Group NV
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
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Utils;
using iText.IO.Source;
using iText.Kernel.Exceptions;
using iText.Test;

namespace iText.Kernel.Crypto {
    [NUnit.Framework.Category("BouncyCastleUnitTest")]
    public class CryptoUtilTest : ExtendedITextTest {
        private static readonly IBouncyCastleFactory FACTORY = BouncyCastleFactoryCreator.GetFactory();

        [NUnit.Framework.Test]
        public virtual void CreateBerStreamTest() {
            ByteArrayOutputStream baos = new ByteArrayOutputStream();
            IDerOutputStream stream = CryptoUtil.CreateAsn1OutputStream(baos, FACTORY.CreateASN1Encoding().GetBer());
            NUnit.Framework.Assert.IsNotNull(stream);
        }

        [NUnit.Framework.Test]
        public virtual void CreateDerStreamTest() {
            ByteArrayOutputStream baos = new ByteArrayOutputStream();
            IDerOutputStream stream = CryptoUtil.CreateAsn1OutputStream(baos, FACTORY.CreateASN1Encoding().GetDer());
            NUnit.Framework.Assert.IsNotNull(stream);
        }

        [NUnit.Framework.Test]
        public virtual void CreateUnsupportedEncodingStreamTest() {
            ByteArrayOutputStream baos = new ByteArrayOutputStream();
            Exception e = NUnit.Framework.Assert.Catch(typeof(NotSupportedException), () => CryptoUtil.CreateAsn1OutputStream
                (baos, "DL"));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(KernelExceptionMessageConstant.UNSUPPORTED_ASN1_ENCODING
                , "DL"), e.Message);
        }
    }
}
