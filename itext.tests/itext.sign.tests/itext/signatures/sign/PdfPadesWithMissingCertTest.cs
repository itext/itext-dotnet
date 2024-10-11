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
using iText.Bouncycastleconnector;
using iText.Commons.Bouncycastle;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Crypto;
using iText.Commons.Utils;
using iText.Forms.Form.Element;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Signatures;
using iText.Signatures.Logs;
using iText.Signatures.Testutils;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Signatures.Sign {
    [NUnit.Framework.Category("BouncyCastleIntegrationTest")]
    public class PdfPadesWithMissingCertTest : ExtendedITextTest {
        private static readonly IBouncyCastleFactory FACTORY = BouncyCastleFactoryCreator.GetFactory();

        private static readonly String certsSrc = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/sign/PdfPadesWithMissingCertTest/certs/";

        private static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/signatures/sign/PdfPadesWithMissingCertTest/";

        private static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/signatures/sign/PdfPadesWithMissingCertTest/";

        private static readonly char[] PASSWORD = "testpassphrase".ToCharArray();

        [NUnit.Framework.OneTimeSetUp]
        public static void Before() {
            CreateOrClearDestinationFolder(destinationFolder);
        }

        public static IEnumerable<Object[]> CreateParameters() {
            return JavaUtil.ArraysAsList(new Object[] { "missing_cert1.cer", "missing_cert2.cer" }, new Object[] { "missing_cert1.crt"
                , "missing_cert2.crt" }, new Object[] { null, "missing_certs.p7b" }, new Object[] { "not_existing_file"
                , "not_existing_file" }, new Object[] { "missing_cert1.der", "missing_cert2.der" });
        }

        [NUnit.Framework.TestCaseSource("CreateParameters")]
        [LogMessage(SignLogMessageConstant.UNABLE_TO_PARSE_AIA_CERT, Ignore = true)]
        public virtual void MissingCertTest(String missingCertName1, String missingCertName2) {
            String srcFileName = sourceFolder + "helloWorldDoc.pdf";
            String signCertFileName = certsSrc + "sign_cert.pem";
            String fistIntermediateCertFileName = certsSrc + "first_intermediate_cert.pem";
            String secondIntermediateCertFileName = certsSrc + "second_intermediate_cert.pem";
            String rootCertFileName = certsSrc + "root_cert.pem";
            String firstMissingCertFileName = certsSrc + missingCertName1;
            String secondMissingCertFileName = certsSrc + missingCertName2;
            IX509Certificate signCert = (IX509Certificate)PemFileHelper.ReadFirstChain(signCertFileName)[0];
            IX509Certificate fistIntermediateCert = (IX509Certificate)PemFileHelper.ReadFirstChain(fistIntermediateCertFileName
                )[0];
            IX509Certificate secondIntermediateCert = (IX509Certificate)PemFileHelper.ReadFirstChain(secondIntermediateCertFileName
                )[0];
            IX509Certificate rootCert = (IX509Certificate)PemFileHelper.ReadFirstChain(rootCertFileName)[0];
            IPrivateKey signPrivateKey = PemFileHelper.ReadFirstKey(signCertFileName, PASSWORD);
            SignerProperties signerProperties = CreateSignerProperties();
            MemoryStream outputStream = new MemoryStream();
            PdfPadesSigner padesSigner = CreatePdfPadesSigner(srcFileName, outputStream);
            IIssuingCertificateRetriever issuingCertificateRetriever = new _IssuingCertificateRetriever_107(firstMissingCertFileName
                , secondMissingCertFileName);
            padesSigner.SetIssuingCertificateRetriever(issuingCertificateRetriever);
            padesSigner.SignWithBaselineBProfile(signerProperties, new IX509Certificate[] { signCert, rootCert }, signPrivateKey
                );
            TestSignUtils.BasicCheckSignedDoc(new MemoryStream(outputStream.ToArray()), "Signature1");
            IList<IX509Certificate> expectedCerts;
            if ("not_existing_file".Equals(missingCertName1)) {
                expectedCerts = JavaUtil.ArraysAsList(signCert, rootCert);
            }
            else {
                expectedCerts = JavaUtil.ArraysAsList(signCert, fistIntermediateCert, secondIntermediateCert, rootCert);
            }
            TestSignUtils.SignedDocumentContainsCerts(new MemoryStream(outputStream.ToArray()), expectedCerts, "Signature1"
                );
        }

        private sealed class _IssuingCertificateRetriever_107 : IssuingCertificateRetriever {
            public _IssuingCertificateRetriever_107(String firstMissingCertFileName, String secondMissingCertFileName) {
                this.firstMissingCertFileName = firstMissingCertFileName;
                this.secondMissingCertFileName = secondMissingCertFileName;
            }

            protected internal override Stream GetIssuerCertByURI(String uri) {
                if (uri.Contains("intermediate")) {
                    return FileUtil.GetInputStreamForFile(firstMissingCertFileName);
                }
                if (uri.Contains("leaf")) {
                    return FileUtil.GetInputStreamForFile(secondMissingCertFileName);
                }
                return null;
            }

            private readonly String firstMissingCertFileName;

            private readonly String secondMissingCertFileName;
        }

        private SignerProperties CreateSignerProperties() {
            SignerProperties signerProperties = new SignerProperties();
            signerProperties.SetFieldName("Signature1");
            SignatureFieldAppearance appearance = new SignatureFieldAppearance(SignerProperties.IGNORED_ID).SetContent
                ("Approval test signature.\nCreated by iText.");
            signerProperties.SetPageRect(new Rectangle(50, 650, 200, 100)).SetSignatureAppearance(appearance);
            return signerProperties;
        }

        private PdfPadesSigner CreatePdfPadesSigner(String srcFileName, Stream outputStream) {
            return new PdfPadesSigner(new PdfReader(FileUtil.GetInputStreamForFile(srcFileName)), outputStream);
        }
    }
}
