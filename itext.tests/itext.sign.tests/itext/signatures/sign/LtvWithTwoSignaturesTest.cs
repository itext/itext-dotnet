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
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.X509;
using iText.Kernel.Pdf;
using iText.Signatures;
using iText.Signatures.Testutils.Client;
using iText.Test;
using iText.Test.Signutils;

namespace iText.Signatures.Sign {
    [NUnit.Framework.Category("IntegrationTest")]
    public class LtvWithTwoSignaturesTest : ExtendedITextTest {
        private static readonly String certsSrc = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/certs/";

        private static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/sign/LtvWithTwoSignaturesTest/";

        private static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/signatures/sign/LtvWithTwoSignaturesTest/";

        private static readonly char[] password = "testpass".ToCharArray();

        [NUnit.Framework.OneTimeSetUp]
        public static void Before() {
            CreateOrClearDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void AddLtvInfo() {
            String caCertFileName = certsSrc + "rootRsa.p12";
            String interCertFileName = certsSrc + "intermediateRsa.p12";
            String srcFileName = sourceFolder + "signedTwice.pdf";
            String ltvFileName = destinationFolder + "ltvEnabledTest01.pdf";
            String ltvFileName2 = destinationFolder + "ltvEnabledTest02.pdf";
            X509Certificate caCert = (X509Certificate)Pkcs12FileHelper.ReadFirstChain(caCertFileName, password)[0];
            ICipherParameters caPrivateKey = Pkcs12FileHelper.ReadFirstKey(caCertFileName, password, password);
            X509Certificate interCert = (X509Certificate)Pkcs12FileHelper.ReadFirstChain(interCertFileName, password)[
                0];
            ICipherParameters interPrivateKey = Pkcs12FileHelper.ReadFirstKey(interCertFileName, password, password);
            TestOcspClient testOcspClient = new TestOcspClient().AddBuilderForCertIssuer(interCert, interPrivateKey).AddBuilderForCertIssuer
                (caCert, caPrivateKey);
            TestCrlClient testCrlClient = new TestCrlClient().AddBuilderForCertIssuer(caCert, caPrivateKey);
            AddLtvInfo(srcFileName, ltvFileName, "Signature1", testOcspClient, testCrlClient);
            AddLtvInfo(ltvFileName, ltvFileName2, "Signature2", testOcspClient, testCrlClient);
            PdfReader reader = new PdfReader(ltvFileName2);
            PdfDocument document = new PdfDocument(reader);
            PdfDictionary catalogDictionary = document.GetCatalog().GetPdfObject();
            PdfDictionary dssDictionary = catalogDictionary.GetAsDictionary(PdfName.DSS);
            PdfDictionary vri = dssDictionary.GetAsDictionary(PdfName.VRI);
            NUnit.Framework.Assert.IsNotNull(vri);
            NUnit.Framework.Assert.AreEqual(2, vri.Size());
            PdfArray ocsps = dssDictionary.GetAsArray(PdfName.OCSPs);
            NUnit.Framework.Assert.IsNotNull(ocsps);
            NUnit.Framework.Assert.AreEqual(5, ocsps.Size());
            PdfArray certs = dssDictionary.GetAsArray(PdfName.Certs);
            NUnit.Framework.Assert.IsNotNull(certs);
            NUnit.Framework.Assert.AreEqual(5, certs.Size());
            PdfArray crls = dssDictionary.GetAsArray(PdfName.CRLs);
            NUnit.Framework.Assert.IsNotNull(crls);
            NUnit.Framework.Assert.AreEqual(2, crls.Size());
        }

        private void AddLtvInfo(String src, String dest, String sigName, TestOcspClient testOcspClient, TestCrlClient
             testCrlClient) {
            PdfDocument document = new PdfDocument(new PdfReader(src), new PdfWriter(dest), new StampingProperties().UseAppendMode
                ());
            LtvVerification ltvVerification = new LtvVerification(document);
            ltvVerification.AddVerification(sigName, testOcspClient, testCrlClient, LtvVerification.CertificateOption.
                WHOLE_CHAIN, LtvVerification.Level.OCSP_CRL, LtvVerification.CertificateInclusion.YES);
            ltvVerification.Merge();
            document.Close();
        }
    }
}
