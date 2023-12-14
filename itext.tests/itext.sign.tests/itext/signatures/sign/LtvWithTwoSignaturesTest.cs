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
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.X509;
using iText.Kernel.Pdf;
using iText.Signatures;
using iText.Signatures.Testutils.Client;
using iText.Test;
using iText.Test.Signutils;

namespace iText.Signatures.Sign {
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
            TestCrlClient testCrlClient = new TestCrlClient(caCert, caPrivateKey);
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
