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
using iText.Kernel.Exceptions;
using iText.Kernel.Logs;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Kernel.Crypto {
    [NUnit.Framework.Category("BouncyCastleIntegrationTest")]
    public class PdfDecryptingTest : ExtendedITextTest {
        private static readonly String CERTS_SRC = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/crypto/PdfDecryptingTest/certs/";

        private static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/kernel/crypto/PdfDecryptingTest/";

        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/crypto/PdfDecryptingTest/";
        
        private static readonly String PROVIDER_NAME = BouncyCastleFactoryCreator.GetFactory().GetProviderName();

        [NUnit.Framework.OneTimeSetUp]
        public static void SetUpBeforeClass() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        //
        // .NET with regular BC
        //
        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void DecryptDotNetRegularWithPasswordStandard40() {
            DecryptWithPassword("dotnet_regular/withPassword/standard40.pdf");
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void DecryptDotNetRegularWithPasswordStandard128() {
            DecryptWithPassword("dotnet_regular/withPassword/standard128.pdf");
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void DecryptDotNetRegularWithPasswordAes128() {
            DecryptWithPassword("dotnet_regular/withPassword/aes128.pdf");
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void DecryptDotNetRegularWithPasswordAes256() {
            DecryptWithPassword("dotnet_regular/withPassword/aes256.pdf");
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void DecryptDotNetRegularWithPasswordAes256Pdf2() {
            DecryptWithPassword("dotnet_regular/withPassword/aes256Pdf2.pdf");
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void DecryptDotNetRegularWithCertificateAes256Rsa() {
            IPrivateKey privateKey = ReadPrivateKey("SHA256withRSA.key");
            if ("BCFIPS".Equals(PROVIDER_NAME)) {
                Exception e = NUnit.Framework.Assert.Catch(typeof(UnsupportedEncryptionFeatureException), () =>
                    DecryptWithCertificate("dotnet_regular/withCertificate/aes256Rsa.pdf",
                        "SHA256withRSA.crt", privateKey));
                NUnit.Framework.Assert.AreEqual(UnsupportedEncryptionFeatureException.ENCRYPTION_WITH_CERTIFICATE_ISNT_SUPPORTED_IN_FIPS, e.Message);
            } else {
                DecryptWithCertificate("dotnet_regular/withCertificate/aes256Rsa.pdf", "SHA256withRSA.crt", privateKey);
            }
        }

        //
        // .NET with FIPS BC
        //
        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void DecryptDotNetWithFipsWithPasswordStandard40() {
            DecryptWithPassword("dotnet_with_fips/withPassword/standard40.pdf");
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void DecryptDotNetWithFipsWithPasswordStandard128() {
            DecryptWithPassword("dotnet_with_fips/withPassword/standard128.pdf");
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void DecryptDotNetWithFipsWithPasswordAes128() {
            DecryptWithPassword("dotnet_with_fips/withPassword/aes128.pdf");
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void DecryptDotNetWithFipsWithPasswordAes256() {
            DecryptWithPassword("dotnet_with_fips/withPassword/aes256.pdf");
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void DecryptDotNetWithFipsWithPasswordAes256Pdf2() {
            DecryptWithPassword("dotnet_with_fips/withPassword/aes256Pdf2.pdf");
        }

        //
        // Java with regular BC
        //
        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void DecryptJavaRegularWithPasswordStandard40() {
            DecryptWithPassword("java_regular/withPassword/standard40.pdf");
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void DecryptJavaRegularWithPasswordStandard128() {
            DecryptWithPassword("java_regular/withPassword/standard128.pdf");
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void DecryptJavaRegularWithPasswordAes128() {
            DecryptWithPassword("java_regular/withPassword/aes128.pdf");
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void DecryptJavaRegularWithPasswordAes256() {
            DecryptWithPassword("java_regular/withPassword/aes256.pdf");
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void DecryptJavaRegularWithPasswordAes256Pdf2() {
            DecryptWithPassword("java_regular/withPassword/aes256Pdf2.pdf");
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void DecryptJavaRegularWithCertificateAes256Rsa() {
            IPrivateKey privateKey = ReadPrivateKey("SHA256withRSA.key");
            if ("BCFIPS".Equals(PROVIDER_NAME)) {
                Exception e = NUnit.Framework.Assert.Catch(typeof(UnsupportedEncryptionFeatureException), () =>
                    DecryptWithCertificate("java_regular/withCertificate/aes256Rsa.pdf",
                        "SHA256withRSA.crt", privateKey));
                NUnit.Framework.Assert.AreEqual(UnsupportedEncryptionFeatureException.ENCRYPTION_WITH_CERTIFICATE_ISNT_SUPPORTED_IN_FIPS, e.Message);
            } else {
                DecryptWithCertificate("java_regular/withCertificate/aes256Rsa.pdf", "SHA256withRSA.crt", privateKey);
            }
        }

        //
        // Java with FIPS BC
        //
        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void DecryptJavaWithFipsWithPasswordStandard40() {
            DecryptWithPassword("java_with_fips/withPassword/standard40.pdf");
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void DecryptJavaWithFipsWithPasswordStandard128() {
            DecryptWithPassword("java_with_fips/withPassword/standard128.pdf");
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void DecryptJavaWithFipsWithPasswordAes128() {
            DecryptWithPassword("java_with_fips/withPassword/aes128.pdf");
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void DecryptJavaWithFIPSWithPasswordAes256() {
            DecryptWithPassword("java_with_fips/withPassword/aes256.pdf");
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void DecryptJavaWithFipsWithPasswordAes256Pdf2() {
            DecryptWithPassword("java_with_fips/withPassword/aes256Pdf2.pdf");
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void DecryptJavaWithFipsWithCertificateAes256Rsa() {
            IPrivateKey privateKey = ReadPrivateKey("SHA256withRSA.key");
            if ("BCFIPS".Equals(PROVIDER_NAME)) {
                Exception e = NUnit.Framework.Assert.Catch(typeof(UnsupportedEncryptionFeatureException), () =>
                    DecryptWithCertificate("java_with_fips/withCertificate/aes256Rsa.pdf",
                        "SHA256withRSA.crt", privateKey));
                NUnit.Framework.Assert.AreEqual(UnsupportedEncryptionFeatureException.ENCRYPTION_WITH_CERTIFICATE_ISNT_SUPPORTED_IN_FIPS, e.Message);
            } else {
                DecryptWithCertificate("java_with_fips/withCertificate/aes256Rsa.pdf", "SHA256withRSA.crt", privateKey);
            }
        }

        //
        // Adobe Acrobat
        //
        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void DecryptAdobeWithPasswordStandard40() {
            DecryptWithPassword("adobe/withPassword/standard40.pdf");
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void DecryptAdobeWithPasswordStandard128() {
            DecryptWithPassword("adobe/withPassword/standard128.pdf");
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void DecryptAdobeWithPasswordAes128() {
            DecryptWithPassword("adobe/withPassword/aes128.pdf");
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void DecryptAdobeWithPasswordAes256() {
            DecryptWithPassword("adobe/withPassword/aes256.pdf");
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void DecryptAdobeWithCertificateAes256Rsa() {
            IPrivateKey privateKey = ReadPrivateKey("SHA256withRSA.key");
            if ("BCFIPS".Equals(PROVIDER_NAME)) {
                Exception e = NUnit.Framework.Assert.Catch(typeof(UnsupportedEncryptionFeatureException), () =>
                    DecryptWithCertificate("adobe/withCertificate/aes256Rsa.pdf", "SHA256withRSA.crt",
                        privateKey
                    ));
                NUnit.Framework.Assert.AreEqual(UnsupportedEncryptionFeatureException.ENCRYPTION_WITH_CERTIFICATE_ISNT_SUPPORTED_IN_FIPS, e.Message);
            } else {
                DecryptWithCertificate("adobe/withCertificate/aes256Rsa.pdf", "SHA256withRSA.crt", privateKey);
            }
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void DecryptAdobeWithCertificateAes256EcdsaP256() {
            IPrivateKey privateKey = ReadPrivateKey("SHA256withECDSA_P256.key");
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () =>
                DecryptWithCertificate("adobe/withCertificate/aes256EcdsaP256.pdf", "SHA256withECDSA_P256.crt",
                    privateKey
                ));
            NUnit.Framework.Assert.AreEqual(
                MessageFormatUtil.Format(KernelExceptionMessageConstant.ALGORITHM_IS_NOT_SUPPORTED, "ECDSA"),
                e.Message);
        }

        private void DecryptWithPassword(String fileName) {
            byte[] user = "user".GetBytes();
            byte[] owner = "owner".GetBytes();
            DecryptWithPassword(fileName, user);
            DecryptWithPassword(fileName, owner);
        }

        private void DecryptWithPassword(String fileName, byte[] password) {
            ReaderProperties readerProperties = new ReaderProperties().SetPassword(password);
            using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(SOURCE_FOLDER + fileName, readerProperties)
                )) {
                NUnit.Framework.Assert.IsTrue(PdfTextExtractor.GetTextFromPage(pdfDocument.GetFirstPage()).StartsWith("Content encrypted by "
                    ));
            }
        }

        private void DecryptWithCertificate(String fileName, String certificateName, IPrivateKey certificateKey) {
            IX509Certificate certificate = CryptoUtil.ReadPublicCertificate(FileUtil.GetInputStreamForFile(CERTS_SRC + certificateName));
            ReaderProperties readerProperties = new ReaderProperties().SetPublicKeySecurityParams(certificate, certificateKey
                );
            using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(SOURCE_FOLDER + fileName, readerProperties)
                )) {
                NUnit.Framework.Assert.IsTrue(PdfTextExtractor.GetTextFromPage(pdfDocument.GetFirstPage()).StartsWith("Content encrypted by "
                    ));
            }
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
