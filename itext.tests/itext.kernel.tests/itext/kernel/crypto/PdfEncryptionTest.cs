/*
This file is part of the iText (R) project.
Copyright (c) 1998-2017 iText Group NV
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
using iText.IO.Font;
using iText.Kernel;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Filespec;
using iText.Kernel.Utils;
using iText.Kernel.XMP;
using iText.Kernel.XMP.Properties;
using iText.Test;
using iText.Test.Attributes;

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
    public class PdfEncryptionTest : ExtendedITextTest {
        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/kernel/crypto/PdfEncryptionTest/";

        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/crypto/PdfEncryptionTest/";

        public static readonly char[] PRIVATE_KEY_PASS = "kspass".ToCharArray();

        public static readonly String CERT = sourceFolder + "test.cer";

        public static readonly String PRIVATE_KEY = sourceFolder + "test.p12";

        internal const String author = "Alexander Chingarev";

        internal const String creator = "iText 7";

        internal const String pageTextContent = "Hello world!";

        /// <summary>User password.</summary>
        public static byte[] USER = "Hello".GetBytes(iText.IO.Util.EncodingUtil.ISO_8859_1);

        /// <summary>Owner password.</summary>
        public static byte[] OWNER = "World".GetBytes(iText.IO.Util.EncodingUtil.ISO_8859_1);

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

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="Org.BouncyCastle.Security.GeneralSecurityException"/>
        /// <exception cref="iText.Kernel.XMP.XMPException"/>
        [NUnit.Framework.Test]
        public virtual void MetadataReadingInEncryptedDoc() {
            PdfReader reader = new PdfReader(sourceFolder + "encryptedWithPlainMetadata.pdf", new ReaderProperties().SetPassword
                (OWNER));
            PdfDocument doc = new PdfDocument(reader);
            XMPMeta xmpMeta = XMPMetaFactory.ParseFromBuffer(doc.GetXmpMetadata());
            XMPProperty creatorToolXmp = xmpMeta.GetProperty(XMPConst.NS_XMP, "CreatorTool");
            doc.Close();
            NUnit.Framework.Assert.IsNotNull(creatorToolXmp);
            NUnit.Framework.Assert.AreEqual("iText 7", creatorToolXmp.GetValue());
        }

        /// <exception cref="Org.BouncyCastle.Security.GeneralSecurityException"/>
        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void CopyEncryptedDocument() {
            PdfDocument srcDoc = new PdfDocument(new PdfReader(sourceFolder + "encryptedWithCertificateAes128.pdf", new 
                ReaderProperties().SetPublicKeySecurityParams(GetPublicCertificate(CERT), GetPrivateKey())));
            String fileName = "copiedEncryptedDoc.pdf";
            PdfDocument destDoc = new PdfDocument(new PdfWriter(destinationFolder + fileName));
            srcDoc.CopyPagesTo(1, 1, destDoc);
            PdfDictionary srcInfo = srcDoc.GetDocumentInfo().GetPdfObject();
            PdfDictionary destInfo = destDoc.GetDocumentInfo().GetPdfObject();
            foreach (PdfName srcInfoKey in srcInfo.KeySet()) {
                destInfo.Put(((PdfName)srcInfoKey.CopyTo(destDoc)), srcInfo.Get(srcInfoKey).CopyTo(destDoc));
            }
            srcDoc.Close();
            destDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + fileName, sourceFolder
                 + "cmp_" + fileName, destinationFolder, "diff_"));
        }

        /// <exception cref="System.Exception"/>
        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="iText.Kernel.XMP.XMPException"/>
        [NUnit.Framework.Test]
        public virtual void OpenDocNoUserPassword() {
            String fileName = "noUserPassword.pdf";
            PdfDocument document = new PdfDocument(new PdfReader(sourceFolder + fileName));
            document.Close();
            CheckDecryptedWithPasswordContent(sourceFolder + fileName, null, pageTextContent);
        }

        /// <exception cref="System.Exception"/>
        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="iText.Kernel.XMP.XMPException"/>
        [NUnit.Framework.Test]
        public virtual void StampDocNoUserPassword() {
            NUnit.Framework.Assert.That(() =>  {
                String fileName = "stampedNoPassword.pdf";
                PdfDocument document = new PdfDocument(new PdfReader(sourceFolder + "noUserPassword.pdf"), new PdfWriter(destinationFolder
                     + fileName));
                document.Close();
            }
            , NUnit.Framework.Throws.TypeOf<BadPasswordException>().With.Message.EqualTo(BadPasswordException.PdfReaderNotOpenedWithOwnerPassword));
;
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="Org.BouncyCastle.Security.GeneralSecurityException"/>
        /// <exception cref="iText.Kernel.XMP.XMPException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        [NUnit.Framework.Ignore("Specific crypto filters for EFF StmF and StrF are not supported at the moment.")]
        public virtual void EncryptWithPasswordAes128EmbeddedFilesOnly() {
            String filename = "encryptWithPasswordAes128EmbeddedFilesOnly.pdf";
            int encryptionType = EncryptionConstants.ENCRYPTION_AES_128 | EncryptionConstants.EMBEDDED_FILES_ONLY;
            String outFileName = destinationFolder + filename;
            int permissions = EncryptionConstants.ALLOW_SCREENREADERS;
            PdfWriter writer = new PdfWriter(outFileName, new WriterProperties().SetStandardEncryption(USER, OWNER, permissions
                , encryptionType).AddXmpMetadata());
            PdfDocument document = new PdfDocument(writer);
            document.GetDocumentInfo().SetAuthor(author).SetCreator(creator);
            PdfPage page = document.AddNewPage();
            String textContent = "Hello world!";
            WriteTextBytesOnPageContent(page, textContent);
            String descripton = "encryptedFile";
            String path = sourceFolder + "pageWithContent.pdf";
            document.AddFileAttachment(descripton, PdfFileSpec.CreateEmbeddedFileSpec(document, path, descripton, path
                , null, null, true));
            page.Flush();
            document.Close();
            CheckDecryptedWithPasswordContent(destinationFolder + filename, OWNER, textContent);
            CheckDecryptedWithPasswordContent(destinationFolder + filename, USER, textContent);
            CompareTool compareTool = new CompareTool().EnableEncryptionCompare();
            String compareResult = compareTool.CompareByContent(outFileName, sourceFolder + "cmp_" + filename, destinationFolder
                , "diff_", USER, USER);
            if (compareResult != null) {
                NUnit.Framework.Assert.Fail(compareResult);
            }
            CheckEncryptedWithPasswordDocumentStamping(filename, OWNER);
            CheckEncryptedWithPasswordDocumentAppending(filename, OWNER);
        }

        /// <exception cref="System.Exception"/>
        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="iText.Kernel.XMP.XMPException"/>
        [NUnit.Framework.Test]
        public virtual void EncryptAes256Pdf2NotEncryptMetadata() {
            String filename = "encryptAes256Pdf2NotEncryptMetadata.pdf";
            int encryptionType = EncryptionConstants.ENCRYPTION_AES_256 | EncryptionConstants.DO_NOT_ENCRYPT_METADATA;
            EncryptWithPassword(filename, encryptionType, CompressionConstants.DEFAULT_COMPRESSION);
        }

        /// <exception cref="System.Exception"/>
        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="iText.Kernel.XMP.XMPException"/>
        [NUnit.Framework.Test]
        public virtual void EncryptWithPasswordAes256Pdf2() {
            String filename = "encryptWithPasswordAes256Pdf2.pdf";
            int encryptionType = EncryptionConstants.ENCRYPTION_AES_256;
            EncryptWithPassword(filename, encryptionType, CompressionConstants.DEFAULT_COMPRESSION, true);
        }

        /// <exception cref="System.Exception"/>
        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="iText.Kernel.XMP.XMPException"/>
        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.FEATURE_IS_DEPRECATED)]
        public virtual void EncryptWithPasswordAes128Pdf2() {
            String filename = "encryptWithPasswordAes128Pdf2.pdf";
            int encryptionType = EncryptionConstants.ENCRYPTION_AES_128;
            EncryptWithPassword(filename, encryptionType, CompressionConstants.DEFAULT_COMPRESSION, true);
        }

        /// <exception cref="System.Exception"/>
        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="iText.Kernel.XMP.XMPException"/>
        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.FEATURE_IS_DEPRECATED)]
        public virtual void StampAndUpdateVersionPreserveStandard40() {
            String filename = "stampAndUpdateVersionPreserveStandard40.pdf";
            PdfDocument doc = new PdfDocument(new PdfReader(sourceFolder + "encryptedWithPasswordStandard40.pdf", new 
                ReaderProperties().SetPassword(OWNER)), new PdfWriter(destinationFolder + filename, new WriterProperties
                ().SetPdfVersion(PdfVersion.PDF_2_0)), new StampingProperties().PreserveEncryption());
            doc.Close();
            CompareEncryptedPdf(filename);
        }

        /// <exception cref="System.Exception"/>
        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="iText.Kernel.XMP.XMPException"/>
        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.FEATURE_IS_DEPRECATED)]
        public virtual void StampAndUpdateVersionPreserveAes256() {
            String filename = "stampAndUpdateVersionPreserveAes256.pdf";
            PdfDocument doc = new PdfDocument(new PdfReader(sourceFolder + "encryptedWithPasswordAes256.pdf", new ReaderProperties
                ().SetPassword(OWNER)), new PdfWriter(destinationFolder + filename, new WriterProperties().SetPdfVersion
                (PdfVersion.PDF_2_0)), new StampingProperties().PreserveEncryption());
            doc.Close();
            CompareEncryptedPdf(filename);
        }

        /// <exception cref="System.Exception"/>
        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="iText.Kernel.XMP.XMPException"/>
        [NUnit.Framework.Test]
        public virtual void StampAndUpdateVersionNewAes256() {
            String filename = "stampAndUpdateVersionNewAes256.pdf";
            PdfDocument doc = new PdfDocument(new PdfReader(sourceFolder + "encryptedWithPasswordAes256.pdf", new ReaderProperties
                ().SetPassword(OWNER)), new PdfWriter(destinationFolder + filename, new WriterProperties().SetPdfVersion
                (PdfVersion.PDF_2_0).SetStandardEncryption(USER, OWNER, 0, EncryptionConstants.ENCRYPTION_AES_256)));
            doc.Close();
            CompareEncryptedPdf(filename);
        }

        /// <exception cref="System.Exception"/>
        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="iText.Kernel.XMP.XMPException"/>
        [NUnit.Framework.Test]
        public virtual void EncryptAes256Pdf2Permissions() {
            String filename = "encryptAes256Pdf2Permissions.pdf";
            int permissions = EncryptionConstants.ALLOW_FILL_IN | EncryptionConstants.ALLOW_SCREENREADERS | EncryptionConstants
                .ALLOW_DEGRADED_PRINTING;
            PdfDocument doc = new PdfDocument(new PdfWriter(destinationFolder + filename, new WriterProperties().SetPdfVersion
                (PdfVersion.PDF_2_0).SetStandardEncryption(USER, OWNER, permissions, EncryptionConstants.ENCRYPTION_AES_256
                )));
            doc.GetDocumentInfo().SetAuthor(author).SetCreator(creator);
            WriteTextBytesOnPageContent(doc.AddNewPage(), pageTextContent);
            doc.Close();
            CompareEncryptedPdf(filename);
        }

        /// <exception cref="iText.Kernel.XMP.XMPException"/>
        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        public virtual void EncryptWithPassword(String filename, int encryptionType, int compression) {
            EncryptWithPassword(filename, encryptionType, compression, false);
        }

        /// <exception cref="iText.Kernel.XMP.XMPException"/>
        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        public virtual void EncryptWithPassword(String filename, int encryptionType, int compression, bool isPdf2) {
            int permissions = EncryptionConstants.ALLOW_SCREENREADERS;
            WriterProperties writerProperties = new WriterProperties().SetStandardEncryption(USER, OWNER, permissions, 
                encryptionType);
            if (isPdf2) {
                writerProperties.SetPdfVersion(PdfVersion.PDF_2_0);
            }
            PdfWriter writer = new PdfWriter(destinationFolder + filename, writerProperties.AddXmpMetadata());
            writer.SetCompressionLevel(compression);
            PdfDocument document = new PdfDocument(writer);
            document.GetDocumentInfo().SetAuthor(author).SetCreator(creator);
            PdfPage page = document.AddNewPage();
            WriteTextBytesOnPageContent(page, pageTextContent);
            page.Flush();
            document.Close();
            CompareEncryptedPdf(filename);
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
                NUnit.Framework.Assert.Fail(compareResult);
            }
            CheckEncryptedWithCertificateDocumentStamping(filename, cert);
            CheckEncryptedWithCertificateDocumentAppending(filename, cert);
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
        public virtual void CheckDecryptedWithPasswordContent(String src, byte[] password, String pageContent) {
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

        /// <exception cref="System.IO.IOException"/>
        private void WriteTextBytesOnPageContent(PdfPage page, String text) {
            page.GetFirstContentStream().GetOutputStream().WriteBytes(("q\n" + "BT\n" + "36 706 Td\n" + "0 0 Td\n" + "/F1 24 Tf\n"
                 + "(" + text + ")Tj\n" + "0 0 Td\n" + "ET\n" + "Q ").GetBytes(iText.IO.Util.EncodingUtil.ISO_8859_1));
            page.GetResources().AddFont(page.GetDocument(), PdfFontFactory.CreateFont(FontConstants.HELVETICA));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        private void CompareEncryptedPdf(String filename) {
            CheckDecryptedWithPasswordContent(destinationFolder + filename, OWNER, pageTextContent);
            CheckDecryptedWithPasswordContent(destinationFolder + filename, USER, pageTextContent);
            CompareTool compareTool = new CompareTool().EnableEncryptionCompare();
            String compareResult = compareTool.CompareByContent(destinationFolder + filename, sourceFolder + "cmp_" + 
                filename, destinationFolder, "diff_", USER, USER);
            if (compareResult != null) {
                NUnit.Framework.Assert.Fail(compareResult);
            }
        }
    }
}
