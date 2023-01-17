/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
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
