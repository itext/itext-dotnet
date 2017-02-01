using System;
using System.IO;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.X509;
using iText.IO.Font;
using iText.Kernel;
using iText.Kernel.Crypto;
using iText.Kernel.Font;
using iText.Kernel.Utils;
using iText.Test;

namespace iText.Kernel.Pdf {
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
    public class PdfEncryptionTest : ExtendedITextTest {
        /// <summary>User password.</summary>
        public static byte[] USER = "Hello".GetBytes(iText.IO.Util.EncodingUtil.ISO_8859_1);

        /// <summary>Owner password.</summary>
        public static byte[] OWNER = "World".GetBytes(iText.IO.Util.EncodingUtil.ISO_8859_1);

        internal const String author = "Alexander Chingarev";

        internal const String creator = "iText 7";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/kernel/pdf/PdfEncryptionTest/";

        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/pdf/PdfEncryptionTest/";

        public static readonly String CERT = sourceFolder + "test.cer";

        public static readonly String PRIVATE_KEY = sourceFolder + "test.p12";

        public static readonly char[] PRIVATE_KEY_PASS = "kspass".ToCharArray();

        private ICipherParameters privateKey;

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(destinationFolder);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="iText.Kernel.XMP.XMPException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void EncryptWithPasswordStandard128() {
            String filename = "encryptWithPasswordStandard128.pdf";
            int encryptionType = EncryptionConstants.STANDARD_ENCRYPTION_128;
            EncryptWithPassword(filename, encryptionType, CompressionConstants.DEFAULT_COMPRESSION);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="iText.Kernel.XMP.XMPException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void EncryptWithPasswordStandard40() {
            String filename = "encryptWithPasswordStandard40.pdf";
            int encryptionType = EncryptionConstants.STANDARD_ENCRYPTION_40;
            EncryptWithPassword(filename, encryptionType, CompressionConstants.DEFAULT_COMPRESSION);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="iText.Kernel.XMP.XMPException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void EncryptWithPasswordStandard128NoCompression() {
            String filename = "encryptWithPasswordStandard128NoCompression.pdf";
            int encryptionType = EncryptionConstants.STANDARD_ENCRYPTION_128;
            EncryptWithPassword(filename, encryptionType, CompressionConstants.NO_COMPRESSION);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="iText.Kernel.XMP.XMPException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void EncryptWithPasswordStandard40NoCompression() {
            String filename = "encryptWithPasswordStandard40NoCompression.pdf";
            int encryptionType = EncryptionConstants.STANDARD_ENCRYPTION_40;
            EncryptWithPassword(filename, encryptionType, CompressionConstants.NO_COMPRESSION);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="iText.Kernel.XMP.XMPException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void EncryptWithPasswordAes128() {
            String filename = "encryptWithPasswordAes128.pdf";
            int encryptionType = EncryptionConstants.ENCRYPTION_AES_128;
            EncryptWithPassword(filename, encryptionType, CompressionConstants.DEFAULT_COMPRESSION);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="iText.Kernel.XMP.XMPException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void EncryptWithPasswordAes256() {
            String filename = "encryptWithPasswordAes256.pdf";
            int encryptionType = EncryptionConstants.ENCRYPTION_AES_256;
            EncryptWithPassword(filename, encryptionType, CompressionConstants.DEFAULT_COMPRESSION);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="iText.Kernel.XMP.XMPException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void EncryptWithPasswordAes128NoCompression() {
            String filename = "encryptWithPasswordAes128NoCompression.pdf";
            int encryptionType = EncryptionConstants.ENCRYPTION_AES_128;
            EncryptWithPassword(filename, encryptionType, CompressionConstants.NO_COMPRESSION);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="iText.Kernel.XMP.XMPException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void EncryptWithPasswordAes256NoCompression() {
            String filename = "encryptWithPasswordAes256NoCompression.pdf";
            int encryptionType = EncryptionConstants.ENCRYPTION_AES_256;
            EncryptWithPassword(filename, encryptionType, CompressionConstants.NO_COMPRESSION);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="iText.Kernel.XMP.XMPException"/>
        /// <exception cref="System.Exception"/>
        /// <exception cref="Org.BouncyCastle.Security.GeneralSecurityException"/>
        [NUnit.Framework.Test]
        public virtual void EncryptWithCertificateStandard128() {
            String filename = "encryptWithCertificateStandard128.pdf";
            int encryptionType = EncryptionConstants.STANDARD_ENCRYPTION_128;
            EncryptWithCertificate(filename, encryptionType, CompressionConstants.DEFAULT_COMPRESSION);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="iText.Kernel.XMP.XMPException"/>
        /// <exception cref="System.Exception"/>
        /// <exception cref="Org.BouncyCastle.Security.GeneralSecurityException"/>
        [NUnit.Framework.Test]
        public virtual void EncryptWithCertificateStandard40() {
            String filename = "encryptWithCertificateStandard40.pdf";
            int encryptionType = EncryptionConstants.STANDARD_ENCRYPTION_40;
            EncryptWithCertificate(filename, encryptionType, CompressionConstants.DEFAULT_COMPRESSION);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="iText.Kernel.XMP.XMPException"/>
        /// <exception cref="System.Exception"/>
        /// <exception cref="Org.BouncyCastle.Security.GeneralSecurityException"/>
        [NUnit.Framework.Test]
        public virtual void EncryptWithCertificateStandard128NoCompression() {
            String filename = "encryptWithCertificateStandard128NoCompression.pdf";
            int encryptionType = EncryptionConstants.STANDARD_ENCRYPTION_128;
            EncryptWithCertificate(filename, encryptionType, CompressionConstants.NO_COMPRESSION);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="iText.Kernel.XMP.XMPException"/>
        /// <exception cref="System.Exception"/>
        /// <exception cref="Org.BouncyCastle.Security.GeneralSecurityException"/>
        [NUnit.Framework.Test]
        public virtual void EncryptWithCertificateStandard40NoCompression() {
            String filename = "encryptWithCertificateStandard40NoCompression.pdf";
            int encryptionType = EncryptionConstants.STANDARD_ENCRYPTION_40;
            EncryptWithCertificate(filename, encryptionType, CompressionConstants.NO_COMPRESSION);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="iText.Kernel.XMP.XMPException"/>
        /// <exception cref="System.Exception"/>
        /// <exception cref="Org.BouncyCastle.Security.GeneralSecurityException"/>
        [NUnit.Framework.Test]
        public virtual void EncryptWithCertificateAes128() {
            String filename = "encryptWithCertificateAes128.pdf";
            int encryptionType = EncryptionConstants.ENCRYPTION_AES_128;
            EncryptWithCertificate(filename, encryptionType, CompressionConstants.DEFAULT_COMPRESSION);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="iText.Kernel.XMP.XMPException"/>
        /// <exception cref="System.Exception"/>
        /// <exception cref="Org.BouncyCastle.Security.GeneralSecurityException"/>
        [NUnit.Framework.Test]
        public virtual void EncryptWithCertificateAes256() {
            String filename = "encryptWithCertificateAes256.pdf";
            int encryptionType = EncryptionConstants.ENCRYPTION_AES_256;
            EncryptWithCertificate(filename, encryptionType, CompressionConstants.DEFAULT_COMPRESSION);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="iText.Kernel.XMP.XMPException"/>
        /// <exception cref="System.Exception"/>
        /// <exception cref="Org.BouncyCastle.Security.GeneralSecurityException"/>
        [NUnit.Framework.Test]
        public virtual void EncryptWithCertificateAes128NoCompression() {
            String filename = "encryptWithCertificateAes128NoCompression.pdf";
            int encryptionType = EncryptionConstants.ENCRYPTION_AES_128;
            EncryptWithCertificate(filename, encryptionType, CompressionConstants.NO_COMPRESSION);
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="iText.Kernel.XMP.XMPException"/>
        /// <exception cref="System.Exception"/>
        /// <exception cref="Org.BouncyCastle.Security.GeneralSecurityException"/>
        [NUnit.Framework.Test]
        public virtual void EncryptWithCertificateAes256NoCompression() {
            String filename = "encryptWithCertificateAes256NoCompression.pdf";
            int encryptionType = EncryptionConstants.ENCRYPTION_AES_256;
            EncryptWithCertificate(filename, encryptionType, CompressionConstants.NO_COMPRESSION);
        }

        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void OpenEncryptedDocWithoutPassword() {
            NUnit.Framework.Assert.That(() =>  {
                PdfDocument doc = new PdfDocument(new PdfReader(sourceFolder + "encryptedWithPasswordStandard40.pdf"));
                doc.Close();
            }
            , NUnit.Framework.Throws.TypeOf<BadPasswordException>().With.Message.EqualTo(BadPasswordException.BadUserPassword));
;
        }

        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void OpenEncryptedDocWithWrongPassword() {
            NUnit.Framework.Assert.That(() =>  {
                PdfReader reader = new PdfReader(sourceFolder + "encryptedWithPasswordStandard40.pdf", new ReaderProperties
                    ().SetPassword("wrong_password".GetBytes(iText.IO.Util.EncodingUtil.ISO_8859_1)));
                PdfDocument doc = new PdfDocument(reader);
                doc.Close();
            }
            , NUnit.Framework.Throws.TypeOf<BadPasswordException>().With.Message.EqualTo(BadPasswordException.BadUserPassword));
;
        }

        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void OpenEncryptedDocWithoutCertificate() {
            NUnit.Framework.Assert.That(() =>  {
                PdfDocument doc = new PdfDocument(new PdfReader(sourceFolder + "encryptedWithCertificateAes128.pdf"));
                doc.Close();
            }
            , NUnit.Framework.Throws.TypeOf<PdfException>().With.Message.EqualTo(PdfException.CertificateIsNotProvidedDocumentIsEncryptedWithPublicKeyCertificate));
;
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="Java.Security.Cert.CertificateException"/>
        [NUnit.Framework.Test]
        public virtual void OpenEncryptedDocWithoutPrivateKey() {
            NUnit.Framework.Assert.That(() =>  {
                PdfReader reader = new PdfReader(sourceFolder + "encryptedWithCertificateAes128.pdf", new ReaderProperties
                    ().SetPublicKeySecurityParams(GetPublicCertificate(sourceFolder + "wrong.cer"), null));
                PdfDocument doc = new PdfDocument(reader);
                doc.Close();
            }
            , NUnit.Framework.Throws.TypeOf<PdfException>().With.Message.EqualTo(PdfException.BadCertificateAndKey));
;
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="Org.BouncyCastle.Security.GeneralSecurityException"/>
        [NUnit.Framework.Test]
        public virtual void OpenEncryptedDocWithWrongCertificate() {
            NUnit.Framework.Assert.That(() =>  {
                PdfReader reader = new PdfReader(sourceFolder + "encryptedWithCertificateAes128.pdf", new ReaderProperties
                    ().SetPublicKeySecurityParams(GetPublicCertificate(sourceFolder + "wrong.cer"), GetPrivateKey()));
                PdfDocument doc = new PdfDocument(reader);
                doc.Close();
            }
            , NUnit.Framework.Throws.TypeOf<PdfException>().With.Message.EqualTo(PdfException.BadCertificateAndKey));
;
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="Org.BouncyCastle.Security.GeneralSecurityException"/>
        [NUnit.Framework.Test]
        public virtual void OpenEncryptedDocWithWrongPrivateKey() {
            NUnit.Framework.Assert.That(() =>  {
                PdfReader reader = new PdfReader(sourceFolder + "encryptedWithCertificateAes128.pdf", new ReaderProperties
                    ().SetPublicKeySecurityParams(GetPublicCertificate(CERT), iText.Kernel.Crypto.CryptoUtil.ReadPrivateKeyFromPkcs12KeyStore
                    (new FileStream(sourceFolder + "wrong.p12", FileMode.Open, FileAccess.Read), "demo", "password".ToCharArray
                    ())));
                PdfDocument doc = new PdfDocument(reader);
                doc.Close();
            }
            , NUnit.Framework.Throws.TypeOf<PdfException>().With.Message.EqualTo(PdfException.PdfDecryption));
;
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="Org.BouncyCastle.Security.GeneralSecurityException"/>
        [NUnit.Framework.Test]
        public virtual void OpenEncryptedDocWithWrongCertificateAndPrivateKey() {
            NUnit.Framework.Assert.That(() =>  {
                PdfReader reader = new PdfReader(sourceFolder + "encryptedWithCertificateAes128.pdf", new ReaderProperties
                    ().SetPublicKeySecurityParams(GetPublicCertificate(sourceFolder + "wrong.cer"), iText.Kernel.Crypto.CryptoUtil.ReadPrivateKeyFromPkcs12KeyStore
                    (new FileStream(sourceFolder + "wrong.p12", FileMode.Open, FileAccess.Read), "demo", "password".ToCharArray
                    ())));
                PdfDocument doc = new PdfDocument(reader);
                doc.Close();
            }
            , NUnit.Framework.Throws.TypeOf<PdfException>().With.Message.EqualTo(PdfException.BadCertificateAndKey));
;
        }

        /// <exception cref="iText.Kernel.XMP.XMPException"/>
        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        public virtual void EncryptWithPassword(String filename, int encryptionType, int compression) {
            String outFileName = destinationFolder + filename;
            int permissions = EncryptionConstants.ALLOW_SCREENREADERS;
            PdfWriter writer = new PdfWriter(outFileName, new WriterProperties().SetStandardEncryption(USER, OWNER, permissions
                , encryptionType).AddXmpMetadata());
            writer.SetCompressionLevel(compression);
            PdfDocument document = new PdfDocument(writer);
            document.GetDocumentInfo().SetAuthor(author).SetCreator(creator);
            PdfPage page = document.AddNewPage();
            String textContent = "Hello world!";
            WriteTextBytesOnPageContent(page, textContent);
            page.Flush();
            document.Close();
            CheckDecryptedWithPasswordContent(filename, OWNER, textContent);
            CheckDecryptedWithPasswordContent(filename, USER, textContent);
            CompareTool compareTool = new CompareTool().EnableEncryptionCompare();
            String compareResult = compareTool.CompareByContent(outFileName, sourceFolder + "cmp_" + filename, destinationFolder
                , "diff_", USER, USER);
            if (compareResult != null) {
                NUnit.Framework.Assert.Fail(compareResult);
            }
            CheckEncryptedWithPasswordDocumentStamping(filename, OWNER);
            CheckEncryptedWithPasswordDocumentAppending(filename, OWNER);
        }

        /// <exception cref="iText.Kernel.XMP.XMPException"/>
        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        /// <exception cref="Org.BouncyCastle.Security.GeneralSecurityException"/>
        public virtual void EncryptWithCertificate(String filename, int encryptionType, int compression) {
            String outFileName = destinationFolder + filename;
            int permissions = EncryptionConstants.ALLOW_SCREENREADERS;
            X509Certificate cert = GetPublicCertificate(CERT);
            PdfWriter writer = new PdfWriter(outFileName, new WriterProperties().SetPublicKeyEncryption(new X509Certificate
                [] { cert }, new int[] { permissions }, encryptionType).AddXmpMetadata());
            writer.SetCompressionLevel(compression);
            PdfDocument document = new PdfDocument(writer);
            document.GetDocumentInfo().SetAuthor(author).SetCreator(creator);
            PdfPage page = document.AddNewPage();
            String textContent = "Hello world!";
            WriteTextBytesOnPageContent(page, textContent);
            page.Flush();
            document.Close();
            CheckDecryptedWithCertificateContent(filename, cert, textContent);
            CompareTool compareTool = new CompareTool().EnableEncryptionCompare();
            compareTool.GetOutReaderProperties().SetPublicKeySecurityParams(cert, GetPrivateKey());
            compareTool.GetCmpReaderProperties().SetPublicKeySecurityParams(cert, GetPrivateKey());
            String compareResult = compareTool.CompareByContent(outFileName, sourceFolder + "cmp_" + filename, destinationFolder
                , "diff_");
            if (compareResult != null) {
                NUnit.Framework.Assert.Fail(compareResult);
            }
            CheckEncryptedWithCertificateDocumentStamping(filename, cert);
            CheckEncryptedWithCertificateDocumentAppending(filename, cert);
        }

        /// <exception cref="System.IO.IOException"/>
        private void WriteTextBytesOnPageContent(PdfPage page, String text) {
            page.GetFirstContentStream().GetOutputStream().WriteBytes(("q\n" + "BT\n" + "36 706 Td\n" + "0 0 Td\n" + "/F1 24 Tf\n"
                 + "(" + text + ")Tj\n" + "0 0 Td\n" + "ET\n" + "Q ").GetBytes(iText.IO.Util.EncodingUtil.ISO_8859_1));
            page.GetResources().AddFont(page.GetDocument(), PdfFontFactory.CreateFont(FontConstants.HELVETICA));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="Java.Security.Cert.CertificateException"/>
        public virtual X509Certificate GetPublicCertificate(String path) {
            FileStream @is = new FileStream(path, FileMode.Open, FileAccess.Read);
            return iText.Kernel.Crypto.CryptoUtil.ReadPublicCertificate(@is);
        }

        /// <exception cref="Org.BouncyCastle.Security.GeneralSecurityException"/>
        /// <exception cref="System.IO.IOException"/>
        public virtual ICipherParameters GetPrivateKey() {
            if (privateKey == null) {
                privateKey = iText.Kernel.Crypto.CryptoUtil.ReadPrivateKeyFromPkcs12KeyStore(new FileStream(PRIVATE_KEY, FileMode.Open
                    , FileAccess.Read), "sandbox", PRIVATE_KEY_PASS);
            }
            return privateKey;
        }

        /// <exception cref="System.IO.IOException"/>
        public virtual void CheckDecryptedWithPasswordContent(String filename, byte[] password, String pageContent
            ) {
            String src = destinationFolder + filename;
            PdfReader reader = new PdfReader(src, new ReaderProperties().SetPassword(password));
            PdfDocument document = new PdfDocument(reader);
            PdfPage page = document.GetPage(1);
            NUnit.Framework.Assert.IsTrue(iText.IO.Util.JavaUtil.GetStringForBytes(page.GetStreamBytes(0)).Contains(pageContent
                ), "Expected content: \n" + pageContent);
            NUnit.Framework.Assert.AreEqual(author, document.GetDocumentInfo().GetAuthor(), "Encrypted author");
            NUnit.Framework.Assert.AreEqual(creator, document.GetDocumentInfo().GetCreator(), "Encrypted creator");
            document.Close();
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="Org.BouncyCastle.Security.GeneralSecurityException"/>
        public virtual void CheckDecryptedWithCertificateContent(String filename, X509Certificate certificate, String
             pageContent) {
            String src = destinationFolder + filename;
            PdfReader reader = new PdfReader(src, new ReaderProperties().SetPublicKeySecurityParams(certificate, GetPrivateKey
                ()));
            PdfDocument document = new PdfDocument(reader);
            PdfPage page = document.GetPage(1);
            String s = iText.IO.Util.JavaUtil.GetStringForBytes(page.GetStreamBytes(0));
            NUnit.Framework.Assert.IsTrue(s.Contains(pageContent), "Expected content: \n" + pageContent);
            NUnit.Framework.Assert.AreEqual(author, document.GetDocumentInfo().GetAuthor(), "Encrypted author");
            NUnit.Framework.Assert.AreEqual(creator, document.GetDocumentInfo().GetCreator(), "Encrypted creator");
            document.Close();
        }

        // basically this is comparing content of decrypted by itext document with content of encrypted document
        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        public virtual void CheckEncryptedWithPasswordDocumentStamping(String filename, byte[] password) {
            String srcFileName = destinationFolder + filename;
            String outFileName = destinationFolder + "stamped_" + filename;
            PdfReader reader = new PdfReader(srcFileName, new ReaderProperties().SetPassword(password));
            PdfDocument document = new PdfDocument(reader, new PdfWriter(outFileName));
            document.Close();
            CompareTool compareTool = new CompareTool();
            String compareResult = compareTool.CompareByContent(outFileName, sourceFolder + "cmp_" + filename, destinationFolder
                , "diff_", USER, USER);
            if (compareResult != null) {
                NUnit.Framework.Assert.Fail(compareResult);
            }
        }

        // basically this is comparing content of decrypted by itext document with content of encrypted document
        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        /// <exception cref="Org.BouncyCastle.Security.GeneralSecurityException"/>
        public virtual void CheckEncryptedWithCertificateDocumentStamping(String filename, X509Certificate certificate
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
                NUnit.Framework.Assert.Fail(compareResult);
            }
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        public virtual void CheckEncryptedWithPasswordDocumentAppending(String filename, byte[] password) {
            String srcFileName = destinationFolder + filename;
            String outFileName = destinationFolder + "appended_" + filename;
            PdfReader reader = new PdfReader(srcFileName, new ReaderProperties().SetPassword(password));
            PdfDocument document = new PdfDocument(reader, new PdfWriter(outFileName), new StampingProperties().UseAppendMode
                ());
            PdfPage newPage = document.AddNewPage();
            newPage.Put(PdfName.Default, new PdfString("Hello world string"));
            WriteTextBytesOnPageContent(newPage, "Hello world page_2!");
            document.Close();
            CompareTool compareTool = new CompareTool().EnableEncryptionCompare();
            String compareResult = compareTool.CompareByContent(outFileName, sourceFolder + "cmp_appended_" + filename
                , destinationFolder, "diff_", USER, USER);
            if (compareResult != null) {
                NUnit.Framework.Assert.Fail(compareResult);
            }
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        /// <exception cref="Org.BouncyCastle.Security.GeneralSecurityException"/>
        public virtual void CheckEncryptedWithCertificateDocumentAppending(String filename, X509Certificate certificate
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
            NUnit.Framework.Assert.AreEqual(actualHelloWorldStringValue, helloWorldStringValue);
            appendedDoc.Close();
            CompareTool compareTool = new CompareTool().EnableEncryptionCompare();
            compareTool.GetOutReaderProperties().SetPublicKeySecurityParams(certificate, GetPrivateKey());
            compareTool.GetCmpReaderProperties().SetPublicKeySecurityParams(certificate, GetPrivateKey());
            String compareResult = compareTool.CompareByContent(outFileName, sourceFolder + "cmp_appended_" + filename
                , destinationFolder, "diff_");
            if (compareResult != null) {
                NUnit.Framework.Assert.Fail(compareResult);
            }
        }
    }
}
