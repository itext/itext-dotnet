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
using iText.Commons.Bouncycastle;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Crypto;
using iText.IO.Font.Constants;
using iText.Kernel.Exceptions;
using iText.Kernel.Font;
using iText.Kernel.Logs;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Test;
using iText.Test.Attributes;
using NUnit.Framework;

namespace iText.Kernel.Crypto {
    /// <summary>
    /// Due to import control restrictions by the governments of a few countries,
    /// the encryption libraries shipped by default with the Java SDK restrict the
    /// length, and as a result the strength, of encryption keys.
    /// </summary>
    /// <remarks>
    /// Due to import control restrictions by the governments of a few countries,
    /// the encryption libraries shipped by default with the Java SDK restrict the
    /// length, and as a result the strength, of encryption keys. Be aware that in
    /// this test by using
    /// <see cref="iText.Test.ITextTest.RemoveCryptographyRestrictions()"/>
    /// we
    /// remove cryptography restrictions via reflection for testing purposes.
    /// <br/>
    /// For more conventional way of solving this problem you need to replace the
    /// default security JARs in your Java installation with the Java Cryptography
    /// Extension (JCE) Unlimited Strength Jurisdiction Policy Files. These JARs
    /// are available for download from http://java.oracle.com/ in eligible countries.
    /// </remarks>
    [Category("BouncyCastleIntegrationTest")]
    public class PdfEncryptionManuallyPortedTest : ExtendedITextTest {
        private static readonly IBouncyCastleFactory FACTORY = BouncyCastleFactoryCreator.GetFactory();

        public static readonly String destinationFolder = TestContext.CurrentContext.TestDirectory
             + "/test/itext/kernel/crypto/PdfEncryptionManuallyPortedTest/";

        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/crypto/PdfEncryptionManuallyPortedTest/";

        public static readonly char[] PRIVATE_KEY_PASS = "testpassphrase".ToCharArray();

        // There is also test.pfx to add to Acrobat to be able to open result pdf files
        public static readonly String CERT = sourceFolder + "test.cer";

        public static readonly String PRIVATE_KEY = sourceFolder + "test.pem";

        internal const String pageTextContent = "Hello world!";

        // Custom entry in Info dictionary is used because standard entried are gone into metadata in PDF 2.0
        internal const String customInfoEntryKey = "Custom";

        internal const String customInfoEntryValue = "String";

        private IPrivateKey privateKey;

        [OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(destinationFolder);
        }

        [Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void EncryptWithCertificateStandard128() {
            String filename = "encryptWithCertificateStandard128.pdf";
            int encryptionType = EncryptionConstants.STANDARD_ENCRYPTION_128;
            if ("BCFIPS".Equals(FACTORY.GetProviderName()))
            {
                Exception e = Assert.Catch(typeof(UnsupportedEncryptionFeatureException), () =>
                    EncryptWithCertificate(filename, encryptionType, CompressionConstants.DEFAULT_COMPRESSION));
                Assert.AreEqual(UnsupportedEncryptionFeatureException.ENCRYPTION_WITH_CERTIFICATE_ISNT_SUPPORTED_IN_FIPS, 
                    e.Message);
            }
            else
            {
                EncryptWithCertificate(filename, encryptionType, CompressionConstants.DEFAULT_COMPRESSION);
            }
        }

        [Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void EncryptWithCertificateStandard40() {
            String filename = "encryptWithCertificateStandard40.pdf";
            int encryptionType = EncryptionConstants.STANDARD_ENCRYPTION_40;
            if ("BCFIPS".Equals(FACTORY.GetProviderName()))
            {
                Exception e = Assert.Catch(typeof(UnsupportedEncryptionFeatureException), () =>
                    EncryptWithCertificate(filename, encryptionType, CompressionConstants.DEFAULT_COMPRESSION));
                Assert.AreEqual(UnsupportedEncryptionFeatureException.ENCRYPTION_WITH_CERTIFICATE_ISNT_SUPPORTED_IN_FIPS, 
                    e.Message);
            }
            else
            {
                EncryptWithCertificate(filename, encryptionType, CompressionConstants.DEFAULT_COMPRESSION);
            }
        }

        [Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void EncryptWithCertificateStandard128NoCompression() {
            String filename = "encryptWithCertificateStandard128NoCompression.pdf";
            int encryptionType = EncryptionConstants.STANDARD_ENCRYPTION_128;
            if ("BCFIPS".Equals(FACTORY.GetProviderName()))
            {
                Exception e = Assert.Catch(typeof(UnsupportedEncryptionFeatureException), () =>
                    EncryptWithCertificate(filename, encryptionType, CompressionConstants.NO_COMPRESSION));
                Assert.AreEqual(UnsupportedEncryptionFeatureException.ENCRYPTION_WITH_CERTIFICATE_ISNT_SUPPORTED_IN_FIPS, 
                    e.Message);
            }
            else
            {
                EncryptWithCertificate(filename, encryptionType, CompressionConstants.NO_COMPRESSION);
            }
        }

        [Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void EncryptWithCertificateStandard40NoCompression() {
            String filename = "encryptWithCertificateStandard40NoCompression.pdf";
            int encryptionType = EncryptionConstants.STANDARD_ENCRYPTION_40;
            if ("BCFIPS".Equals(FACTORY.GetProviderName()))
            {
                Exception e = Assert.Catch(typeof(UnsupportedEncryptionFeatureException), () =>
                    EncryptWithCertificate(filename, encryptionType, CompressionConstants.NO_COMPRESSION));
                Assert.AreEqual(UnsupportedEncryptionFeatureException.ENCRYPTION_WITH_CERTIFICATE_ISNT_SUPPORTED_IN_FIPS, 
                    e.Message);
            }
            else
            {
                EncryptWithCertificate(filename, encryptionType, CompressionConstants.NO_COMPRESSION);
            }
        }

        [Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void EncryptWithCertificateAes128() {
            String filename = "encryptWithCertificateAes128.pdf";
            int encryptionType = EncryptionConstants.ENCRYPTION_AES_128;
            if ("BCFIPS".Equals(FACTORY.GetProviderName()))
            {
                Exception e = Assert.Catch(typeof(UnsupportedEncryptionFeatureException) , () =>
                    EncryptWithCertificate(filename, encryptionType, CompressionConstants.DEFAULT_COMPRESSION));
                Assert.AreEqual(UnsupportedEncryptionFeatureException.ENCRYPTION_WITH_CERTIFICATE_ISNT_SUPPORTED_IN_FIPS, 
                    e.Message);
            }
            else
            {
                EncryptWithCertificate(filename, encryptionType, CompressionConstants.DEFAULT_COMPRESSION);
            }
        }

        [Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void EncryptWithCertificateAes256() {
            String filename = "encryptWithCertificateAes256.pdf";
            int encryptionType = EncryptionConstants.ENCRYPTION_AES_256;
            if ("BCFIPS".Equals(FACTORY.GetProviderName()))
            {
                Exception e = Assert.Catch(typeof(UnsupportedEncryptionFeatureException), () =>
                    EncryptWithCertificate(filename, encryptionType, CompressionConstants.DEFAULT_COMPRESSION));
                Assert.AreEqual(UnsupportedEncryptionFeatureException.ENCRYPTION_WITH_CERTIFICATE_ISNT_SUPPORTED_IN_FIPS, 
                    e.Message);
            }
            else
            {
                EncryptWithCertificate(filename, encryptionType, CompressionConstants.DEFAULT_COMPRESSION);
            }
        }

        [Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void EncryptWithCertificateAes128NoCompression() {
            String filename = "encryptWithCertificateAes128NoCompression.pdf";
            int encryptionType = EncryptionConstants.ENCRYPTION_AES_128;
            if ("BCFIPS".Equals(FACTORY.GetProviderName()))
            {
                Exception e = Assert.Catch(typeof(UnsupportedEncryptionFeatureException), () =>
                    EncryptWithCertificate(filename, encryptionType, CompressionConstants.NO_COMPRESSION));
                Assert.AreEqual(UnsupportedEncryptionFeatureException.ENCRYPTION_WITH_CERTIFICATE_ISNT_SUPPORTED_IN_FIPS, 
                    e.Message);
            }
            else
            {
                EncryptWithCertificate(filename, encryptionType, CompressionConstants.NO_COMPRESSION);
            }
        }

        [Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void EncryptWithCertificateAes256NoCompression() {
            String filename = "encryptWithCertificateAes256NoCompression.pdf";
            int encryptionType = EncryptionConstants.ENCRYPTION_AES_256;
            if ("BCFIPS".Equals(FACTORY.GetProviderName()))
            {
                Exception e = Assert.Catch(typeof(UnsupportedEncryptionFeatureException), () =>
                    EncryptWithCertificate(filename, encryptionType, CompressionConstants.NO_COMPRESSION));
                Assert.AreEqual(UnsupportedEncryptionFeatureException.ENCRYPTION_WITH_CERTIFICATE_ISNT_SUPPORTED_IN_FIPS, 
                    e.Message);
            }
            else
            {
                EncryptWithCertificate(filename, encryptionType, CompressionConstants.NO_COMPRESSION);
            }
        }

        [Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void OpenEncryptedDocWithWrongPrivateKey() {
            using (PdfReader reader = new PdfReader(sourceFolder + "encryptedWithCertificateAes128.pdf", new ReaderProperties
                ().SetPublicKeySecurityParams(GetPublicCertificate(CERT), PemFileHelper.ReadPrivateKeyFromPemFile(new 
                FileStream(sourceFolder + "wrong.pem", FileMode.Open, FileAccess.Read), PRIVATE_KEY_PASS)))) {
                if ("BCFIPS".Equals(FACTORY.GetProviderName()))
                {
                    Exception e = NUnit.Framework.Assert.Catch(typeof(UnsupportedEncryptionFeatureException), () => new PdfDocument(reader));
                    NUnit.Framework.Assert.AreEqual(UnsupportedEncryptionFeatureException.ENCRYPTION_WITH_CERTIFICATE_ISNT_SUPPORTED_IN_FIPS, e.Message);
                }
                else
                {
                    Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => new PdfDocument(reader));
                    NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.PDF_DECRYPTION, e.Message);
                }
            }
        }

        [Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void OpenEncryptedWithCertificateDocWithDefaultKeyLength() {
            IX509Certificate cert = GetPublicCertificate(CERT);
            using (PdfReader reader = new PdfReader(sourceFolder + "encryptedWithCertificateWithDefaultKeyLength.pdf",
                       new ReaderProperties().SetPublicKeySecurityParams(cert, GetPrivateKey()))) {
                if ("BCFIPS".Equals(FACTORY.GetProviderName())) {
                    Exception e = NUnit.Framework.Assert.Catch(typeof(UnsupportedEncryptionFeatureException),
                        () => new PdfDocument(reader));
                    NUnit.Framework.Assert.AreEqual(
                        UnsupportedEncryptionFeatureException.ENCRYPTION_WITH_CERTIFICATE_ISNT_SUPPORTED_IN_FIPS,
                        e.Message);
                } else {
                    using (PdfDocument document = new PdfDocument(reader)) {
                        NUnit.Framework.Assert.IsFalse(document.GetTrailer().GetAsDictionary(PdfName.Encrypt)
                            .ContainsKey(PdfName.Length));
                    }
                }
            }
        }

        public virtual void EncryptWithCertificate(String filename, int encryptionType, int compression) {
            String outFileName = destinationFolder + filename;
            int permissions = EncryptionConstants.ALLOW_SCREENREADERS;
            IX509Certificate cert = GetPublicCertificate(CERT);
            PdfWriter writer = new PdfWriter(outFileName, new WriterProperties().SetPublicKeyEncryption(new IX509Certificate
                [] { cert }, new int[] { permissions }, encryptionType).AddXmpMetadata());
            writer.SetCompressionLevel(compression);
            PdfDocument document = new PdfDocument(writer);
            document.GetDocumentInfo().SetMoreInfo(customInfoEntryKey, customInfoEntryValue);
            PdfPage page = document.AddNewPage();
            WriteTextBytesOnPageContent(page, pageTextContent);
            page.Flush();
            document.Close();
            CheckDecryptedWithCertificateContent(filename, cert, pageTextContent);
            CompareTool compareTool = new CompareTool().EnableEncryptionCompare();
            compareTool.GetOutReaderProperties().SetPublicKeySecurityParams(cert, GetPrivateKey());
            compareTool.GetCmpReaderProperties().SetPublicKeySecurityParams(cert, GetPrivateKey());
            String compareResult = compareTool.CompareByContent(outFileName, sourceFolder + "cmp_" + filename, destinationFolder
                , "diff_");
            if (compareResult != null) {
                Assert.Fail(compareResult);
            }
            CheckEncryptedWithCertificateDocumentStamping(filename, cert);
            CheckEncryptedWithCertificateDocumentAppending(filename, cert);
        }

        public virtual IX509Certificate GetPublicCertificate(String path) {
            FileStream @is = new FileStream(path, FileMode.Open, FileAccess.Read);
            return CryptoUtil.ReadPublicCertificate(@is);
        }

        public virtual IPrivateKey GetPrivateKey() {
            if (privateKey == null) {
                privateKey = PemFileHelper.ReadPrivateKeyFromPemFile(new FileStream(PRIVATE_KEY, FileMode.Open, FileAccess.Read
                    ), PRIVATE_KEY_PASS);
            }
            return privateKey;
        }

        public virtual void CheckDecryptedWithCertificateContent(String filename, IX509Certificate certificate, String
             pageContent) {
            String src = destinationFolder + filename;
            PdfReader reader = new PdfReader(src, new ReaderProperties().SetPublicKeySecurityParams(certificate, GetPrivateKey
                ()));
            PdfDocument document = new PdfDocument(reader);
            PdfPage page = document.GetPage(1);
            String s = iText.Commons.Utils.JavaUtil.GetStringForBytes(page.GetStreamBytes(0));
            Assert.IsTrue(s.Contains(pageContent), "Expected content: \n" + pageContent);
            Assert.AreEqual(customInfoEntryValue, document.GetTrailer().GetAsDictionary(PdfName.Info).
                GetAsString(new PdfName(customInfoEntryKey)).ToUnicodeString(), "Encrypted custom");
            document.Close();
        }

        // basically this is comparing content of decrypted by itext document with content of encrypted document
        public virtual void CheckEncryptedWithCertificateDocumentStamping(String filename, IX509Certificate certificate
            ) {
            String srcFileName = destinationFolder + filename;
            String outFileName = destinationFolder + "stamped_" + filename;
            PdfReader reader = new PdfReader(srcFileName, new ReaderProperties().SetPublicKeySecurityParams(certificate
                , GetPrivateKey()));
            PdfDocument document = new PdfDocument(reader, new PdfWriter(outFileName));
            document.Close();
            CompareTool compareTool = new CompareTool();
            compareTool.GetCmpReaderProperties().SetPublicKeySecurityParams(certificate, GetPrivateKey());
            String compareResult = compareTool.CompareByContent(outFileName, sourceFolder + "cmp_" + filename, destinationFolder
                , "diff_");
            if (compareResult != null) {
                Assert.Fail(compareResult);
            }
        }

        public virtual void CheckEncryptedWithCertificateDocumentAppending(String filename, IX509Certificate certificate
            ) {
            String srcFileName = destinationFolder + filename;
            String outFileName = destinationFolder + "appended_" + filename;
            PdfReader reader = new PdfReader(srcFileName, new ReaderProperties().SetPublicKeySecurityParams(certificate
                , GetPrivateKey()));
            PdfDocument document = new PdfDocument(reader, new PdfWriter(outFileName), new StampingProperties().UseAppendMode
                ());
            PdfPage newPage = document.AddNewPage();
            String helloWorldStringValue = "Hello world string";
            newPage.Put(PdfName.Default, new PdfString(helloWorldStringValue));
            WriteTextBytesOnPageContent(newPage, "Hello world page_2!");
            document.Close();
            PdfReader appendedDocReader = new PdfReader(outFileName, new ReaderProperties().SetPublicKeySecurityParams
                (certificate, GetPrivateKey()));
            PdfDocument appendedDoc = new PdfDocument(appendedDocReader);
            PdfPage secondPage = appendedDoc.GetPage(2);
            PdfString helloWorldPdfString = secondPage.GetPdfObject().GetAsString(PdfName.Default);
            String actualHelloWorldStringValue = helloWorldPdfString != null ? helloWorldPdfString.GetValue() : null;
            Assert.AreEqual(actualHelloWorldStringValue, helloWorldStringValue);
            appendedDoc.Close();
            CompareTool compareTool = new CompareTool().EnableEncryptionCompare();
            compareTool.GetOutReaderProperties().SetPublicKeySecurityParams(certificate, GetPrivateKey());
            compareTool.GetCmpReaderProperties().SetPublicKeySecurityParams(certificate, GetPrivateKey());
            String compareResult = compareTool.CompareByContent(outFileName, sourceFolder + "cmp_appended_" + filename
                , destinationFolder, "diff_");
            if (compareResult != null) {
                Assert.Fail(compareResult);
            }
        }

        internal static void WriteTextBytesOnPageContent(PdfPage page, String text) {
            page.GetFirstContentStream().GetOutputStream().WriteBytes(("q\n" + "BT\n" + "36 706 Td\n" + "0 0 Td\n" + "/F1 24 Tf\n"
                 + "(" + text + ")Tj\n" + "0 0 Td\n" + "ET\n" + "Q ").GetBytes(iText.Commons.Utils.EncodingUtil.ISO_8859_1
                ));
            page.GetResources().AddFont(page.GetDocument(), PdfFontFactory.CreateFont(StandardFonts.HELVETICA));
        }
    }
}
