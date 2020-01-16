/*
This file is part of the iText (R) project.
Copyright (c) 1998-2020 iText Group NV
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
using System.IO;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.X509;
using iText.IO.Util;
using iText.Kernel.Pdf;
using iText.Signatures;
using iText.Signatures.Testutils.Client;
using iText.Test;
using iText.Test.Signutils;

namespace iText.Signatures.Sign {
    public class LtvSigTest : ExtendedITextTest {
        private static readonly String certsSrc = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/certs/";

        private static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/sign/LtvSigTest/";

        private static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/signatures/sign/LtvSigTest/";

        private static readonly char[] password = "testpass".ToCharArray();

        [NUnit.Framework.OneTimeSetUp]
        public static void Before() {
            CreateOrClearDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void LtvEnabledTest01() {
            String tsaCertFileName = certsSrc + "tsCertRsa.p12";
            String caCertFileName = certsSrc + "rootRsa.p12";
            String srcFileName = sourceFolder + "signedDoc.pdf";
            String ltvFileName = destinationFolder + "ltvEnabledTest01.pdf";
            String ltvTsFileName = destinationFolder + "ltvEnabledTsTest01.pdf";
            X509Certificate[] tsaChain = Pkcs12FileHelper.ReadFirstChain(tsaCertFileName, password);
            ICipherParameters tsaPrivateKey = Pkcs12FileHelper.ReadFirstKey(tsaCertFileName, password, password);
            X509Certificate caCert = (X509Certificate)Pkcs12FileHelper.ReadFirstChain(caCertFileName, password)[0];
            ICipherParameters caPrivateKey = Pkcs12FileHelper.ReadFirstKey(caCertFileName, password, password);
            TestTsaClient testTsa = new TestTsaClient(JavaUtil.ArraysAsList(tsaChain), tsaPrivateKey);
            TestOcspClient testOcspClient = new TestOcspClient().AddBuilderForCertIssuer(caCert, caPrivateKey);
            TestCrlClient testCrlClient = new TestCrlClient(caCert, caPrivateKey);
            PdfDocument document = new PdfDocument(new PdfReader(srcFileName), new PdfWriter(ltvFileName), new StampingProperties
                ().UseAppendMode());
            LtvVerification ltvVerification = new LtvVerification(document);
            ltvVerification.AddVerification("Signature1", testOcspClient, testCrlClient, LtvVerification.CertificateOption
                .SIGNING_CERTIFICATE, LtvVerification.Level.OCSP_CRL, LtvVerification.CertificateInclusion.YES);
            ltvVerification.Merge();
            document.Close();
            PdfSigner signer = new PdfSigner(new PdfReader(ltvFileName), new FileStream(ltvTsFileName, FileMode.Create
                ), new StampingProperties().UseAppendMode());
            signer.Timestamp(testTsa, "timestampSig1");
            BasicCheckLtvDoc("ltvEnabledTsTest01.pdf", "timestampSig1");
        }

        [NUnit.Framework.Test]
        public virtual void LtvEnabledSingleSignatureTest01() {
            String signCertFileName = certsSrc + "signCertRsaWithChain.p12";
            String tsaCertFileName = certsSrc + "tsCertRsa.p12";
            String intermediateCertFileName = certsSrc + "intermediateRsa.p12";
            String caCertFileName = certsSrc + "rootRsa.p12";
            String srcFileName = sourceFolder + "helloWorldDoc.pdf";
            String ltvFileName = destinationFolder + "ltvEnabledSingleSignatureTest01.pdf";
            X509Certificate[] tsaChain = Pkcs12FileHelper.ReadFirstChain(tsaCertFileName, password);
            ICipherParameters tsaPrivateKey = Pkcs12FileHelper.ReadFirstKey(tsaCertFileName, password, password);
            X509Certificate intermediateCert = (X509Certificate)Pkcs12FileHelper.ReadFirstChain(intermediateCertFileName
                , password)[0];
            ICipherParameters intermediatePrivateKey = Pkcs12FileHelper.ReadFirstKey(intermediateCertFileName, password
                , password);
            X509Certificate caCert = (X509Certificate)Pkcs12FileHelper.ReadFirstChain(caCertFileName, password)[0];
            ICipherParameters caPrivateKey = Pkcs12FileHelper.ReadFirstKey(caCertFileName, password, password);
            TestTsaClient testTsa = new TestTsaClient(JavaUtil.ArraysAsList(tsaChain), tsaPrivateKey);
            TestOcspClient testOcspClient = new TestOcspClient().AddBuilderForCertIssuer(intermediateCert, intermediatePrivateKey
                ).AddBuilderForCertIssuer(caCert, caPrivateKey);
            X509Certificate[] signChain = Pkcs12FileHelper.ReadFirstChain(signCertFileName, password);
            ICipherParameters signPrivateKey = Pkcs12FileHelper.ReadFirstKey(signCertFileName, password, password);
            IExternalSignature pks = new PrivateKeySignature(signPrivateKey, DigestAlgorithms.SHA256);
            PdfSigner signer = new PdfSigner(new PdfReader(srcFileName), new FileStream(ltvFileName, FileMode.Create), 
                new StampingProperties());
            signer.SetFieldName("Signature1");
            signer.SignDetached(pks, signChain, null, testOcspClient, testTsa, 0, PdfSigner.CryptoStandard.CADES);
            PadesSigTest.BasicCheckSignedDoc(destinationFolder + "ltvEnabledSingleSignatureTest01.pdf", "Signature1");
        }

        [NUnit.Framework.Test]
        public virtual void SecondLtvOriginalHasNoVri01() {
            String tsaCertFileName = certsSrc + "tsCertRsa.p12";
            String caCertFileName = certsSrc + "rootRsa.p12";
            String srcFileName = sourceFolder + "ltvEnabledNoVriEntry.pdf";
            String ltvFileName = destinationFolder + "secondLtvOriginalHasNoVri01.pdf";
            String ltvTsFileName = destinationFolder + "secondLtvOriginalHasNoVriTs01.pdf";
            X509Certificate[] tsaChain = Pkcs12FileHelper.ReadFirstChain(tsaCertFileName, password);
            ICipherParameters tsaPrivateKey = Pkcs12FileHelper.ReadFirstKey(tsaCertFileName, password, password);
            X509Certificate caCert = (X509Certificate)Pkcs12FileHelper.ReadFirstChain(caCertFileName, password)[0];
            ICipherParameters caPrivateKey = Pkcs12FileHelper.ReadFirstKey(caCertFileName, password, password);
            TestTsaClient testTsa = new TestTsaClient(JavaUtil.ArraysAsList(tsaChain), tsaPrivateKey);
            TestOcspClient testOcspClient = new TestOcspClient().AddBuilderForCertIssuer(caCert, caPrivateKey);
            TestCrlClient testCrlClient = new TestCrlClient(caCert, caPrivateKey);
            PdfDocument document = new PdfDocument(new PdfReader(srcFileName), new PdfWriter(ltvFileName), new StampingProperties
                ().UseAppendMode());
            LtvVerification ltvVerification = new LtvVerification(document);
            ltvVerification.AddVerification("timestampSig1", testOcspClient, testCrlClient, LtvVerification.CertificateOption
                .SIGNING_CERTIFICATE, LtvVerification.Level.OCSP_CRL, LtvVerification.CertificateInclusion.YES);
            ltvVerification.Merge();
            document.Close();
            PdfSigner signer = new PdfSigner(new PdfReader(ltvFileName), new FileStream(ltvTsFileName, FileMode.Create
                ), new StampingProperties().UseAppendMode());
            signer.Timestamp(testTsa, "timestampSig2");
            BasicCheckLtvDoc("secondLtvOriginalHasNoVriTs01.pdf", "timestampSig2");
        }

        private void BasicCheckLtvDoc(String outFileName, String tsSigName) {
            PdfDocument outDocument = new PdfDocument(new PdfReader(destinationFolder + outFileName));
            PdfDictionary dssDict = outDocument.GetCatalog().GetPdfObject().GetAsDictionary(PdfName.DSS);
            NUnit.Framework.Assert.IsNotNull(dssDict);
            NUnit.Framework.Assert.AreEqual(4, dssDict.Size());
            outDocument.Close();
            PadesSigTest.BasicCheckSignedDoc(destinationFolder + outFileName, tsSigName);
        }
    }
}
