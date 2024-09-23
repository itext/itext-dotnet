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
using iText.Bouncycastleconnector;
using iText.Bouncycastlefips;
using iText.Commons.Bouncycastle;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Crypto;
using iText.Commons.Utils;
using iText.Kernel.Exceptions;
using iText.Kernel.Logs;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Kernel.Crypto.Securityhandler {
    [NUnit.Framework.Category("BouncyCastleIntegrationTest")]
    public class PubSecHandlerUsingAesGcmTest : ExtendedITextTest {
        public static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/crypto/securityhandler/PubSecHandlerUsingAesGcmTest/";

        public static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/kernel/crypto/securityhandler/PubSecHandlerUsingAesGcmTest/";

        private static readonly IBouncyCastleFactory FACTORY = BouncyCastleFactoryCreator.GetFactory();

        private static readonly char[] PASSWORD = "testpassphrase".ToCharArray();

        [NUnit.Framework.OneTimeSetUp]
        public static void SetUp() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        [NUnit.Framework.Test]
        public virtual void TestSimpleEncryptDecryptTest() {
            String fileName = "simpleEncryptDecrypt.pdf";
            String srcFile = SOURCE_FOLDER + fileName;
            String outFile = DESTINATION_FOLDER + fileName;
            if ("BCFIPS".Equals(FACTORY.GetProviderName())) {
                Exception e = NUnit.Framework.Assert.Catch(typeof(UnsupportedEncryptionFeatureException), () =>
                    DoEncrypt(srcFile, outFile, true));
                NUnit.Framework.Assert.AreEqual(UnsupportedEncryptionFeatureException.ENCRYPTION_WITH_CERTIFICATE_ISNT_SUPPORTED_IN_FIPS, e.Message);
            } else {
                DoEncrypt(srcFile, outFile, true);
                DecryptWithCertificate(fileName, DESTINATION_FOLDER, "test.cer", "test.pem");
            }
        }

        [LogMessage(VersionConforming.NOT_SUPPORTED_AES_GCM, Ignore = true)]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        [NUnit.Framework.Test]
        public virtual void TestSimpleEncryptDecryptPdf15Test() {
            String fileName = "simpleEncryptDecrypt.pdf";
            String srcFile = SOURCE_FOLDER + fileName;
            String outFile = DESTINATION_FOLDER + fileName;
            if ("BCFIPS".Equals(FACTORY.GetProviderName())) {
                Exception e = NUnit.Framework.Assert.Catch(typeof(UnsupportedEncryptionFeatureException), () =>
                    DoEncrypt(srcFile, outFile, false));
                NUnit.Framework.Assert.AreEqual(UnsupportedEncryptionFeatureException.ENCRYPTION_WITH_CERTIFICATE_ISNT_SUPPORTED_IN_FIPS, e.Message);
            } else {
                DoEncrypt(srcFile, outFile, false);
                DecryptWithCertificate(fileName, DESTINATION_FOLDER, "test.cer", "test.pem");
            }
        }

        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        [NUnit.Framework.Test]
        public virtual void DecryptExternalFileTest() {
            if ("BCFIPS".Equals(FACTORY.GetProviderName())) {
                Exception e = NUnit.Framework.Assert.Catch(typeof(UnsupportedEncryptionFeatureException), () =>
                    DecryptWithCertificate("externalFile.pdf", SOURCE_FOLDER, "decrypter.cert.pem", "signerkey.pem"));
                NUnit.Framework.Assert.AreEqual(UnsupportedEncryptionFeatureException.ENCRYPTION_WITH_CERTIFICATE_ISNT_SUPPORTED_IN_FIPS, e.Message);
            } else {
                DecryptWithCertificate("externalFile.pdf", SOURCE_FOLDER, "decrypter.cert.pem", "signerkey.pem");
            }
        }

        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        [NUnit.Framework.Test]
        public virtual void InvalidCryptFilterTest() {
            String fileName = "invalidCryptFilter.pdf";
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => DecryptWithCertificate(fileName, SOURCE_FOLDER
                , "test.cer", "test.pem"));
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.NO_COMPATIBLE_ENCRYPTION_FOUND, e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void EncryptPdfWithMissingCFTest() {
            IPrivateKey certificateKey = PemFileHelper.ReadPrivateKeyFromPemFile(FileUtil.GetInputStreamForFile(SOURCE_FOLDER
                 + "signerkey.pem"), PASSWORD);
            IX509Certificate certificate = CryptoUtil.ReadPublicCertificate(FileUtil.GetInputStreamForFile(SOURCE_FOLDER
                 + "decrypter.cert.pem"));
            Dictionary<PdfName, PdfObject> encMap = new Dictionary<PdfName, PdfObject>();
            encMap.Put(PdfName.V, new PdfNumber(6));
            encMap.Put(PdfName.EncryptMetadata, PdfBoolean.TRUE);
            PdfDictionary dictionary = new PdfDictionary(encMap);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => new PdfEncryption(dictionary, certificateKey
                , certificate));
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.CF_NOT_FOUND_ENCRYPTION, e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void EncryptPdfWithMissingDefaultCryptFilterTest() {
            IPrivateKey certificateKey = PemFileHelper.ReadPrivateKeyFromPemFile(FileUtil.GetInputStreamForFile(SOURCE_FOLDER
                 + "signerkey.pem"), PASSWORD);
            IX509Certificate certificate = CryptoUtil.ReadPublicCertificate(FileUtil.GetInputStreamForFile(SOURCE_FOLDER
                 + "decrypter.cert.pem"));
            Dictionary<PdfName, PdfObject> encMap = new Dictionary<PdfName, PdfObject>();
            encMap.Put(PdfName.V, new PdfNumber(6));
            PdfDictionary embeddedFilesDict = new PdfDictionary();
            embeddedFilesDict.Put(PdfName.FlateDecode, new PdfDictionary());
            encMap.Put(PdfName.CF, embeddedFilesDict);
            PdfDictionary dictionary = new PdfDictionary(encMap);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => new PdfEncryption(dictionary, certificateKey
                , certificate));
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.DEFAULT_CRYPT_FILTER_NOT_FOUND_ENCRYPTION, 
                e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void EncryptPdfWithMissingCFMTest() {
            IPrivateKey certificateKey = PemFileHelper.ReadPrivateKeyFromPemFile(FileUtil.GetInputStreamForFile(SOURCE_FOLDER
                 + "signerkey.pem"), PASSWORD);
            IX509Certificate certificate = CryptoUtil.ReadPublicCertificate(FileUtil.GetInputStreamForFile(SOURCE_FOLDER
                 + "decrypter.cert.pem"));
            Dictionary<PdfName, PdfObject> encMap = new Dictionary<PdfName, PdfObject>();
            encMap.Put(PdfName.V, new PdfNumber(6));
            PdfDictionary embeddedFilesDict = new PdfDictionary();
            embeddedFilesDict.Put(PdfName.DefaultCryptFilter, new PdfDictionary());
            encMap.Put(PdfName.CF, embeddedFilesDict);
            PdfDictionary dictionary = new PdfDictionary(encMap);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => new PdfEncryption(dictionary, certificateKey
                , certificate));
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.NO_COMPATIBLE_ENCRYPTION_FOUND, e.Message);
        }

        private void DoEncrypt(String input, String output, bool isPdf20) {
            IX509Certificate certificate = CryptoUtil.ReadPublicCertificate(FileUtil.GetInputStreamForFile(SOURCE_FOLDER
                 + "test.cer"));
            WriterProperties writerProperties = new WriterProperties().SetPublicKeyEncryption(new IX509Certificate[] { 
                certificate }, new int[] { EncryptionConstants.ALLOW_PRINTING }, EncryptionConstants.ENCRYPTION_AES_GCM
                );
            if (isPdf20) {
                writerProperties.SetPdfVersion(PdfVersion.PDF_2_0);
            }
            // Instantiate input/output document.
            using (PdfDocument docIn = new PdfDocument(new PdfReader(input))) {
                using (PdfDocument docOut = new PdfDocument(new PdfWriter(output, writerProperties))) {
                    // Copy one page from input to output.
                    docIn.CopyPagesTo(1, 1, docOut);
                }
            }
        }

        private void DecryptWithCertificate(String fileName, String srcFileFolder, String certificateName, String 
            privateKeyName) {
            String srcFile = srcFileFolder + fileName;
            String cmpFile = SOURCE_FOLDER + "cmp_" + fileName;
            String outFile = DESTINATION_FOLDER + "decrypted_" + fileName;
            IX509Certificate certificate = CryptoUtil.ReadPublicCertificate(FileUtil.GetInputStreamForFile(SOURCE_FOLDER
                 + certificateName));
            IPrivateKey privateKey = PemFileHelper.ReadPrivateKeyFromPemFile(FileUtil.GetInputStreamForFile(SOURCE_FOLDER
                 + privateKeyName), PASSWORD);
            ReaderProperties readerProperties = new ReaderProperties().SetPublicKeySecurityParams(certificate, privateKey
                );
            PdfDocument ignored = new PdfDocument(new PdfReader(srcFile, readerProperties), new PdfWriter(outFile));
            ignored.Close();
            String errorMessage = new CompareTool().CompareByContent(outFile, cmpFile, DESTINATION_FOLDER, "diff_");
            if (errorMessage != null) {
                NUnit.Framework.Assert.Fail(errorMessage);
            }
        }
    }
}
