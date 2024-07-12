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
using System.IO;
using iText.Bouncycastleconnector;
using iText.Bouncycastlefips;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Crypto;
using iText.Commons.Bouncycastle.Openssl;
using iText.Commons.Utils;
using iText.IO.Font.Constants;
using iText.Kernel.Exceptions;
using iText.Kernel.Font;
using iText.Kernel.Logs;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Utils;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Kernel.Crypto {
    [NUnit.Framework.Category("BouncyCastleIntegrationTest")]
    public class PdfEncryptingTest : ExtendedITextTest {
        private static readonly String CERTS_SRC = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/crypto/PdfEncryptingTest/certs/";

        private static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/kernel/crypto/PdfEncryptingTest/";

        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/crypto/PdfEncryptingTest/";

        private static readonly String PROVIDER_NAME = BouncyCastleFactoryCreator.GetFactory().GetProviderName();

        private static readonly byte[] USER_PASSWORD = "user".GetBytes();

        private static readonly byte[] OWNER_PASSWORD = "owner".GetBytes();

        [NUnit.Framework.OneTimeSetUp]
        public static void SetUpBeforeClass() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void EncryptWithPasswordStandard40() {
            EncryptWithPassword("encryptWithPasswordStandard40.pdf", EncryptionConstants.STANDARD_ENCRYPTION_40, false
                );
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void EncryptWithPasswordStandard128() {
            EncryptWithPassword("encryptWithPasswordStandard128.pdf", EncryptionConstants.STANDARD_ENCRYPTION_128, false
                );
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void EncryptWithPasswordAes128() {
            EncryptWithPassword("encryptWithPasswordAes128.pdf", EncryptionConstants.ENCRYPTION_AES_128, false);
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void EncryptWithPasswordAes256() {
            EncryptWithPassword("encryptWithPasswordAes256.pdf", EncryptionConstants.ENCRYPTION_AES_256, false);
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void EncryptWithPasswordAes256Pdf2() {
            EncryptWithPassword("encryptWithPasswordAes256Pdf2.pdf", EncryptionConstants.ENCRYPTION_AES_256, true);
        }

        [NUnit.Framework.Test]
        public virtual void EncryptWithCertificateAes256Rsa() {
            if ("BCFIPS".Equals(PROVIDER_NAME)) {
                String exceptionTest = NUnit.Framework.Assert.Catch(typeof(UnsupportedEncryptionFeatureException),
                    () => EncryptWithCertificate("encryptWithCertificateAes256Rsa.pdf", "SHA256withRSA.crt")).Message;
                NUnit.Framework.Assert.AreEqual(UnsupportedEncryptionFeatureException.ENCRYPTION_WITH_CERTIFICATE_ISNT_SUPPORTED_IN_FIPS, exceptionTest);
            } else {
                EncryptWithCertificate("encryptWithCertificateAes256Rsa.pdf", "SHA256withRSA.crt");
            }
        }

        [NUnit.Framework.Test]
        public virtual void EncryptWithCertificateAes256EcdsaP256() {
            if ("BCFIPS".Equals(PROVIDER_NAME)) {
                String exceptionTest = NUnit.Framework.Assert.Catch(typeof(UnsupportedEncryptionFeatureException),
                    () => EncryptWithCertificate("encryptWithCertificateAes256EcdsaP256.pdf", "SHA256withECDSA_P256.crt")).Message;
                NUnit.Framework.Assert.AreEqual(UnsupportedEncryptionFeatureException.ENCRYPTION_WITH_CERTIFICATE_ISNT_SUPPORTED_IN_FIPS, exceptionTest);
            }
            else {
                String exceptionTest = NUnit.Framework.Assert.Catch(typeof(PdfException),
                    () => EncryptWithCertificate("encryptWithCertificateAes256EcdsaP256.pdf", "SHA256withECDSA_P256.crt")).Message;
                NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(
                    KernelExceptionMessageConstant.ALGORITHM_IS_NOT_SUPPORTED, "1.2.840.10045.2.1"), exceptionTest);
            }
        }

        [NUnit.Framework.Test]
        public virtual void EncryptWithCertificateAes256EcdsaBrainpoolP256R1() {
            if ("BCFIPS".Equals(PROVIDER_NAME)) {
                String exceptionTest = NUnit.Framework.Assert.Catch(typeof(UnsupportedEncryptionFeatureException),
                    () => EncryptWithCertificate("encryptWithCertificateAes256EcdsaBrainpoolP256R1.pdf", "SHA256withECDSA_brainpoolP256r1.crt")).Message;
                NUnit.Framework.Assert.AreEqual(UnsupportedEncryptionFeatureException.ENCRYPTION_WITH_CERTIFICATE_ISNT_SUPPORTED_IN_FIPS, exceptionTest);
            }
            else {
                String exceptionTest = NUnit.Framework.Assert.Catch(typeof(PdfException),
                    () => EncryptWithCertificate("encryptWithCertificateAes256EcdsaBrainpoolP256R1.pdf", "SHA256withECDSA_brainpoolP256r1.crt")).Message;
                NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(
                    KernelExceptionMessageConstant.ALGORITHM_IS_NOT_SUPPORTED, "1.2.840.10045.2.1"), exceptionTest);
            }
        }

        private void EncryptWithPassword(String fileName, int encryptionType, bool pdf2) {
            WriterProperties writerProperties = new WriterProperties().SetStandardEncryption(USER_PASSWORD, OWNER_PASSWORD
                , -1, encryptionType);
            if (pdf2) {
                writerProperties.SetPdfVersion(PdfVersion.PDF_2_0);
            }
            using (PdfWriter writer = new PdfWriter(DESTINATION_FOLDER + fileName, writerProperties.AddXmpMetadata())) {
                using (PdfDocument document = new PdfDocument(writer)) {
                    WriteTextToDocument(document);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(DESTINATION_FOLDER + fileName, SOURCE_FOLDER
                 + "cmp_" + fileName, DESTINATION_FOLDER, "diff", USER_PASSWORD, USER_PASSWORD));
        }

        private void EncryptWithCertificate(String fileName, String certificatePath) {
            IX509Certificate certificate = CryptoUtil.ReadPublicCertificate( FileUtil.GetInputStreamForFile(CERTS_SRC + certificatePath));
            WriterProperties writerProperties = new WriterProperties().SetPublicKeyEncryption(new IX509Certificate[] { 
                certificate }, new int[] { -1 }, EncryptionConstants.ENCRYPTION_AES_256);
            using (PdfWriter writer = new PdfWriter(DESTINATION_FOLDER + fileName, writerProperties.AddXmpMetadata())) {
                using (PdfDocument document = new PdfDocument(writer)) {
                    WriteTextToDocument(document);
                }
            }
            CompareTool compareTool = new CompareTool();
            IPrivateKey privateKey = ReadPrivateKey("SHA256withRSA.key");
            compareTool.GetCmpReaderProperties().SetPublicKeySecurityParams(certificate, privateKey);
            compareTool.GetOutReaderProperties().SetPublicKeySecurityParams(certificate, privateKey);
            NUnit.Framework.Assert.IsNull(compareTool.CompareByContent(DESTINATION_FOLDER + fileName, SOURCE_FOLDER + 
                "cmp_" + fileName, DESTINATION_FOLDER, "diff"));
        }

        private void WriteTextToDocument(PdfDocument document) {
            PdfCanvas canvas = new PdfCanvas(document.AddNewPage());
            canvas.SaveState()
                .BeginText()
                .MoveText(36, 750)
                .SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.HELVETICA), 16)
                .ShowText("Content encrypted by iText")
                .EndText()
                .RestoreState();
        }

        private IPrivateKey ReadPrivateKey(String privateKeyName) {
            using (Stream pemFile = FileUtil.GetInputStreamForFile(CERTS_SRC + privateKeyName)) {
                using (TextReader file = new StreamReader(pemFile))
                {
                    IPemReader parser = BouncyCastleFactoryCreator.GetFactory().CreatePEMParser(file, "test".ToCharArray());
                    Object readObject = parser.ReadObject();
                    while (!(readObject is IPrivateKey) && readObject != null)
                    {
                        readObject = parser.ReadObject();
                    }

                    return (IPrivateKey)readObject;
                }
            }
        }
    }
}
