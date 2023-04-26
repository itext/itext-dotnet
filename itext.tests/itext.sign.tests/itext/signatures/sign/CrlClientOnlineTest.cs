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
using System.IO;
using iText.Commons.Bouncycastle.Cert;
using iText.Kernel.Crypto;
using iText.Signatures;
using iText.Signatures.Testutils;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Signatures.Sign {
    [NUnit.Framework.Category("BouncyCastleUnitTest")]
    public class CrlClientOnlineTest : ExtendedITextTest {
        private static readonly String certSrc = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/sign/CrlClientOnlineTest/";

        private static readonly String certWithMalformedUrl = certSrc + "certWithMalformedUrl.crt";

        private static readonly String certWithCorrectUrl = certSrc + "certWithCorrectUrl.crt";

        private static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/signatures/sign/";

        [NUnit.Framework.Test]
        public virtual void CrlClientOnlineURLConstructorTest() {
            String PROTOCOL = "file://";
            Uri[] urls = new Uri[] { new Uri(PROTOCOL + destinationFolder + "duplicateFolder"), new Uri(PROTOCOL + destinationFolder
                 + "duplicateFolder"), new Uri(PROTOCOL + destinationFolder + "uniqueFolder") };
            CrlClientOnline crlClientOnline = new CrlClientOnline(urls);
            NUnit.Framework.Assert.AreEqual(2, crlClientOnline.GetUrlsSize());
        }

        [NUnit.Framework.Test]
        [LogMessage("Added CRL url: https://examples.com", LogLevel = LogLevelConstants.INFO)]
        public virtual void AddCrlUrlTest() {
            CrlClientOnline crlClientOnline = new CrlClientOnline("https://examples.com");
            NUnit.Framework.Assert.AreEqual(1, crlClientOnline.GetUrlsSize());
        }

        [NUnit.Framework.Test]
        [LogMessage("Skipped CRL url (malformed):", LogLevel = LogLevelConstants.INFO)]
        public virtual void AddEmptyCrlUrlTest() {
            CrlClientOnline crlClientOnline = new CrlClientOnline("");
            NUnit.Framework.Assert.AreEqual(0, crlClientOnline.GetUrlsSize());
        }

        [NUnit.Framework.Test]
        [LogMessage("Skipped CRL url (malformed):", LogLevel = LogLevelConstants.INFO)]
        public virtual void AddWrongCrlUrlTest() {
            CrlClientOnline crlClientOnline = new CrlClientOnline("test");
            NUnit.Framework.Assert.AreEqual(0, crlClientOnline.GetUrlsSize());
        }

        [NUnit.Framework.Test]
        [LogMessage("Checking certificate: ", LogLevel = LogLevelConstants.INFO)]
        [LogMessage("Skipped CRL url (malformed): test", LogLevel = LogLevelConstants.INFO)]
        public virtual void CheckCrlCertWithMalformedUrlTest() {
            IX509Certificate chain = CryptoUtil.ReadPublicCertificate(new FileStream(certWithMalformedUrl, FileMode.Open
                , FileAccess.Read));
            CrlClientOnline crlClientOnline = new CrlClientOnline(new IX509Certificate[] { chain });
            NUnit.Framework.Assert.AreEqual(0, crlClientOnline.GetUrlsSize());
        }

        [NUnit.Framework.Test]
        [LogMessage("Checking certificate: ", LogLevel = LogLevelConstants.INFO)]
        [LogMessage("Added CRL url: http://www.example.com/crl/test.crl", LogLevel = LogLevelConstants.INFO)]
        public virtual void CheckCrlCertWithCorrectUrlTest() {
            IX509Certificate chain = CryptoUtil.ReadPublicCertificate(new FileStream(certWithCorrectUrl, FileMode.Open
                , FileAccess.Read));
            CrlClientOnline crlClientOnline = new CrlClientOnline(new IX509Certificate[] { chain });
            NUnit.Framework.Assert.AreEqual(1, crlClientOnline.GetUrlsSize());
        }

        [NUnit.Framework.Test]
        public virtual void CannotGetEncodedWhenCertIsNullTest() {
            CrlClientOnline crlClientOnline = new CrlClientOnline();
            NUnit.Framework.Assert.IsNull(crlClientOnline.GetEncoded(null, ""));
            NUnit.Framework.Assert.AreEqual(0, crlClientOnline.GetUrlsSize());
        }

        [NUnit.Framework.Test]
        [LogMessage("Added CRL url: http://www.example.com/crl/test.crl", LogLevel = LogLevelConstants.INFO)]
        [LogMessage("Checking CRL: http://www.example.com/crl/test.crl", LogLevel = LogLevelConstants.INFO)]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.INVALID_DISTRIBUTION_POINT, LogLevel = LogLevelConstants.INFO
            )]
        public virtual void UnreachableCrlDistributionPointTest() {
            CrlClientOnline crlClientOnline = new CrlClientOnline("http://www.example.com/crl/test.crl");
            IX509Certificate checkCert = new X509MockCertificate();
            ICollection<byte[]> bytes = crlClientOnline.GetEncoded(checkCert, "http://www.example.com/crl/test.crl");
            NUnit.Framework.Assert.IsTrue(bytes.IsEmpty());
            NUnit.Framework.Assert.AreEqual(1, crlClientOnline.GetUrlsSize());
        }

        [NUnit.Framework.Test]
        [LogMessage("Looking for CRL for certificate ", LogLevel = LogLevelConstants.INFO)]
        [LogMessage("Found CRL url: http://www.example.com/crl/test.crl", LogLevel = LogLevelConstants.INFO)]
        [LogMessage("Checking CRL: http://www.example.com/crl/test.crl", LogLevel = LogLevelConstants.INFO)]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.INVALID_DISTRIBUTION_POINT, LogLevel = LogLevelConstants.INFO
            )]
        public virtual void UnreachableCrlDistributionPointFromCertChainTest() {
            CrlClientOnline crlClientOnline = new CrlClientOnline();
            IX509Certificate checkCert = new X509MockCertificate();
            ICollection<byte[]> bytes = crlClientOnline.GetEncoded(checkCert, "http://www.example.com/crl/test.crl");
            NUnit.Framework.Assert.IsTrue(bytes.IsEmpty());
            NUnit.Framework.Assert.AreEqual(0, crlClientOnline.GetUrlsSize());
        }
    }
}
