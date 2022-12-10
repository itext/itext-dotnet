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
using System.IO;
using NUnit.Framework;
using iText.Bouncycastleconnector;
using iText.Commons.Bouncycastle;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Crypto;
using iText.IO.Font.Constants;
using iText.Kernel.Exceptions;
using iText.Kernel.Font;
using iText.Kernel.Logs;
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
    [NUnit.Framework.Category("BouncyCastleIntegrationTest")]
    public class PdfEncryptionTest : ExtendedITextTest {
        private static readonly IBouncyCastleFactory FACTORY = BouncyCastleFactoryCreator.GetFactory();

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/kernel/crypto/PdfEncryptionTest/";

        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/crypto/PdfEncryptionTest/";

        public static readonly char[] PRIVATE_KEY_PASS = "testpassphrase".ToCharArray();

        public static readonly String CERT = sourceFolder + "test.cer";

        public static readonly String PRIVATE_KEY = sourceFolder + "test.pem";

        internal const String pageTextContent = "Hello world!";

        // Custom entry in Info dictionary is used because standard entried are gone into metadata in PDF 2.0
        internal const String customInfoEntryKey = "Custom";

        internal const String customInfoEntryValue = "String";

        /// <summary>User password.</summary>
        public static byte[] USER = "Hello".GetBytes(iText.Commons.Utils.EncodingUtil.ISO_8859_1);

        /// <summary>Owner password.</summary>
        public static byte[] OWNER = "World".GetBytes(iText.Commons.Utils.EncodingUtil.ISO_8859_1);

        private IPrivateKey privateKey;

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void EncryptWithPasswordStandard128() {
            String filename = "encryptWithPasswordStandard128.pdf";
            int encryptionType = EncryptionConstants.STANDARD_ENCRYPTION_128;
            EncryptWithPassword2(filename, encryptionType, CompressionConstants.DEFAULT_COMPRESSION);
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void EncryptWithPasswordStandard40() {
            String filename = "encryptWithPasswordStandard40.pdf";
            int encryptionType = EncryptionConstants.STANDARD_ENCRYPTION_40;
            EncryptWithPassword2(filename, encryptionType, CompressionConstants.DEFAULT_COMPRESSION);
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void EncryptWithPasswordStandard128NoCompression() {
            String filename = "encryptWithPasswordStandard128NoCompression.pdf";
            int encryptionType = EncryptionConstants.STANDARD_ENCRYPTION_128;
            EncryptWithPassword2(filename, encryptionType, CompressionConstants.NO_COMPRESSION);
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void EncryptWithPasswordStandard40NoCompression() {
            String filename = "encryptWithPasswordStandard40NoCompression.pdf";
            int encryptionType = EncryptionConstants.STANDARD_ENCRYPTION_40;
            EncryptWithPassword2(filename, encryptionType, CompressionConstants.NO_COMPRESSION);
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void EncryptWithPasswordAes128() {
            String filename = "encryptWithPasswordAes128.pdf";
            int encryptionType = EncryptionConstants.ENCRYPTION_AES_128;
            EncryptWithPassword2(filename, encryptionType, CompressionConstants.DEFAULT_COMPRESSION);
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void EncryptWithPasswordAes256() {
            String filename = "encryptWithPasswordAes256.pdf";
            int encryptionType = EncryptionConstants.ENCRYPTION_AES_256;
            EncryptWithPassword2(filename, encryptionType, CompressionConstants.DEFAULT_COMPRESSION);
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void EncryptWithPasswordAes128NoCompression() {
            String filename = "encryptWithPasswordAes128NoCompression.pdf";
            int encryptionType = EncryptionConstants.ENCRYPTION_AES_128;
            EncryptWithPassword2(filename, encryptionType, CompressionConstants.NO_COMPRESSION);
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void EncryptWithPasswordAes256NoCompression() {
            String filename = "encryptWithPasswordAes256NoCompression.pdf";
            int encryptionType = EncryptionConstants.ENCRYPTION_AES_256;
            EncryptWithPassword2(filename, encryptionType, CompressionConstants.NO_COMPRESSION);
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void EncryptWithCertificateStandard128() {
            String filename = "encryptWithCertificateStandard128.pdf";
            int encryptionType = EncryptionConstants.STANDARD_ENCRYPTION_128;
            EncryptWithCertificate(filename, encryptionType, CompressionConstants.DEFAULT_COMPRESSION);
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void EncryptWithCertificateStandard40() {
            String filename = "encryptWithCertificateStandard40.pdf";
            int encryptionType = EncryptionConstants.STANDARD_ENCRYPTION_40;
            EncryptWithCertificate(filename, encryptionType, CompressionConstants.DEFAULT_COMPRESSION);
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void EncryptWithCertificateStandard128NoCompression() {
            String filename = "encryptWithCertificateStandard128NoCompression.pdf";
            int encryptionType = EncryptionConstants.STANDARD_ENCRYPTION_128;
            EncryptWithCertificate(filename, encryptionType, CompressionConstants.NO_COMPRESSION);
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void EncryptWithCertificateStandard40NoCompression() {
            String filename = "encryptWithCertificateStandard40NoCompression.pdf";
            int encryptionType = EncryptionConstants.STANDARD_ENCRYPTION_40;
            EncryptWithCertificate(filename, encryptionType, CompressionConstants.NO_COMPRESSION);
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void EncryptWithCertificateAes128() {
            String filename = "encryptWithCertificateAes128.pdf";
            int encryptionType = EncryptionConstants.ENCRYPTION_AES_128;
            EncryptWithCertificate(filename, encryptionType, CompressionConstants.DEFAULT_COMPRESSION);
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void EncryptWithCertificateAes256() {
            String filename = "encryptWithCertificateAes256.pdf";
            int encryptionType = EncryptionConstants.ENCRYPTION_AES_256;
            EncryptWithCertificate(filename, encryptionType, CompressionConstants.DEFAULT_COMPRESSION);
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void EncryptWithCertificateAes128NoCompression() {
            String filename = "encryptWithCertificateAes128NoCompression.pdf";
            int encryptionType = EncryptionConstants.ENCRYPTION_AES_128;
            EncryptWithCertificate(filename, encryptionType, CompressionConstants.NO_COMPRESSION);
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void EncryptWithCertificateAes256NoCompression() {
            String filename = "encryptWithCertificateAes256NoCompression.pdf";
            int encryptionType = EncryptionConstants.ENCRYPTION_AES_256;
            EncryptWithCertificate(filename, encryptionType, CompressionConstants.NO_COMPRESSION);
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void OpenEncryptedDocWithoutPassword() {
            using (PdfReader reader = new PdfReader(sourceFolder + "encryptedWithPasswordStandard40.pdf")) {
                Exception e = NUnit.Framework.Assert.Catch(typeof(BadPasswordException), () => new PdfDocument(reader));
                NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.BAD_USER_PASSWORD, e.Message);
            }
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void OpenEncryptedDocWithWrongPassword() {
            using (PdfReader reader = new PdfReader(sourceFolder + "encryptedWithPasswordStandard40.pdf", new ReaderProperties
                ().SetPassword("wrong_password".GetBytes(iText.Commons.Utils.EncodingUtil.ISO_8859_1)))) {
                Exception e = NUnit.Framework.Assert.Catch(typeof(BadPasswordException), () => new PdfDocument(reader));
                NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.BAD_USER_PASSWORD, e.Message);
            }
        }

        [NUnit.Framework.Test]
        public virtual void OpenEncryptedDocWithoutCertificate() {
            using (PdfReader reader = new PdfReader(sourceFolder + "encryptedWithCertificateAes128.pdf")) {
                Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => new PdfDocument(reader));
                NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.CERTIFICATE_IS_NOT_PROVIDED_DOCUMENT_IS_ENCRYPTED_WITH_PUBLIC_KEY_CERTIFICATE
                    , e.Message);
            }
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void OpenEncryptedDocWithoutPrivateKey() {
            using (PdfReader reader = new PdfReader(sourceFolder + "encryptedWithCertificateAes128.pdf", new ReaderProperties
                ().SetPublicKeySecurityParams(GetPublicCertificate(sourceFolder + "wrong.cer"), null))) {
                Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => new PdfDocument(reader));
                NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.BAD_CERTIFICATE_AND_KEY, e.Message);
            }
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void OpenEncryptedDocWithWrongCertificate() {
            using (PdfReader reader = new PdfReader(sourceFolder + "encryptedWithCertificateAes128.pdf", new ReaderProperties
                ().SetPublicKeySecurityParams(GetPublicCertificate(sourceFolder + "wrong.cer"), GetPrivateKey()))) {
                Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => new PdfDocument(reader));
                NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.BAD_CERTIFICATE_AND_KEY, e.Message);
            }
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void OpenEncryptedDocWithWrongPrivateKey() {
            using (PdfReader reader = new PdfReader(sourceFolder + "encryptedWithCertificateAes128.pdf", new ReaderProperties
                ().SetPublicKeySecurityParams(GetPublicCertificate(CERT), PemFileHelper.ReadPrivateKeyFromPemFile(new 
                FileStream(sourceFolder + "wrong.pem", FileMode.Open, FileAccess.Read), PRIVATE_KEY_PASS)))) {
                Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => new PdfDocument(reader));
                NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.PDF_DECRYPTION, e.Message);
            }
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void OpenEncryptedDocWithWrongCertificateAndPrivateKey() {
            using (PdfReader reader = new PdfReader(sourceFolder + "encryptedWithCertificateAes128.pdf", new ReaderProperties
                ().SetPublicKeySecurityParams(GetPublicCertificate(sourceFolder + "wrong.cer"), PemFileHelper.ReadPrivateKeyFromPemFile
                (new FileStream(sourceFolder + "wrong.pem", FileMode.Open, FileAccess.Read), PRIVATE_KEY_PASS)))) {
                Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => new PdfDocument(reader));
                NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.BAD_CERTIFICATE_AND_KEY, e.Message);
            }
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
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

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void CopyEncryptedDocument() {
            PdfDocument srcDoc = new PdfDocument(new PdfReader(sourceFolder + "encryptedWithCertificateAes128.pdf", new 
                ReaderProperties().SetPublicKeySecurityParams(GetPublicCertificate(CERT), GetPrivateKey())));
            String fileName = "copiedEncryptedDoc.pdf";
            PdfDocument destDoc = new PdfDocument(new PdfWriter(destinationFolder + fileName));
            srcDoc.CopyPagesTo(1, 1, destDoc);
            PdfDictionary srcInfo = srcDoc.GetTrailer().GetAsDictionary(PdfName.Info);
            PdfDictionary destInfo = destDoc.GetTrailer().GetAsDictionary(PdfName.Info);
            foreach (PdfName srcInfoKey in srcInfo.KeySet()) {
                destInfo.Put((PdfName)srcInfoKey.CopyTo(destDoc), srcInfo.Get(srcInfoKey).CopyTo(destDoc));
            }
            srcDoc.Close();
            destDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + fileName, sourceFolder
                 + "cmp_" + fileName, destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void OpenDocNoUserPassword() {
            String fileName = "noUserPassword.pdf";
            PdfDocument document = new PdfDocument(new PdfReader(sourceFolder + fileName));
            document.Close();
            CheckDecryptedWithPasswordContent(sourceFolder + fileName, null, pageTextContent);
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void StampDocNoUserPassword() {
            String fileName = "stampedNoPassword.pdf";
            using (PdfReader reader = new PdfReader(sourceFolder + "noUserPassword.pdf")) {
                using (PdfWriter writer = new PdfWriter(destinationFolder + fileName)) {
                    Exception e = NUnit.Framework.Assert.Catch(typeof(BadPasswordException), () => new PdfDocument(reader, writer
                        ));
                    NUnit.Framework.Assert.AreEqual(BadPasswordException.PdfReaderNotOpenedWithOwnerPassword, e.Message);
                }
            }
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void EncryptWithPasswordAes128EmbeddedFilesOnly() {
            String filename = "encryptWithPasswordAes128EmbeddedFilesOnly.pdf";
            int encryptionType = EncryptionConstants.ENCRYPTION_AES_128 | EncryptionConstants.EMBEDDED_FILES_ONLY;
            String outFileName = destinationFolder + filename;
            int permissions = EncryptionConstants.ALLOW_SCREENREADERS;
            PdfWriter writer = new PdfWriter(outFileName, new WriterProperties().SetStandardEncryption(USER, OWNER, permissions
                , encryptionType).AddXmpMetadata());
            PdfDocument document = new PdfDocument(writer);
            document.GetDocumentInfo().SetMoreInfo(customInfoEntryKey, customInfoEntryValue);
            PdfPage page = document.AddNewPage();
            String textContent = "Hello world!";
            WriteTextBytesOnPageContent(page, textContent);
            String descripton = "encryptedFile";
            String path = sourceFolder + "pageWithContent.pdf";
            document.AddFileAttachment(descripton, PdfFileSpec.CreateEmbeddedFileSpec(document, path, descripton, path
                , null, null));
            page.Flush();
            document.Close();
            //TODO DEVSIX-5355 Specific crypto filters for EFF StmF and StrF are not supported at the moment.
            // However we can read embedded files only mode.
            bool ERROR_IS_EXPECTED = false;
            CheckDecryptedWithPasswordContent(destinationFolder + filename, OWNER, textContent, ERROR_IS_EXPECTED);
            CheckDecryptedWithPasswordContent(destinationFolder + filename, USER, textContent, ERROR_IS_EXPECTED);
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void EncryptAes256Pdf2NotEncryptMetadata() {
            String filename = "encryptAes256Pdf2NotEncryptMetadata.pdf";
            int encryptionType = EncryptionConstants.ENCRYPTION_AES_256 | EncryptionConstants.DO_NOT_ENCRYPT_METADATA;
            EncryptWithPassword2(filename, encryptionType, CompressionConstants.DEFAULT_COMPRESSION);
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void EncryptAes256Pdf2NotEncryptMetadata02() {
            String filename = "encryptAes256Pdf2NotEncryptMetadata02.pdf";
            int encryptionType = EncryptionConstants.ENCRYPTION_AES_256 | EncryptionConstants.DO_NOT_ENCRYPT_METADATA;
            EncryptWithPassword(filename, encryptionType, CompressionConstants.DEFAULT_COMPRESSION, true);
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void EncryptAes256EncryptedStampingPreserve() {
            String filename = "encryptAes256EncryptedStampingPreserve.pdf";
            String src = sourceFolder + "encryptedWithPlainMetadata.pdf";
            String @out = destinationFolder + filename;
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(src, new ReaderProperties().SetPassword(OWNER)), new PdfWriter
                (@out, new WriterProperties()), new StampingProperties().PreserveEncryption());
            pdfDoc.Close();
            CompareTool compareTool = new CompareTool().EnableEncryptionCompare();
            String compareResult = compareTool.CompareByContent(@out, sourceFolder + "cmp_" + filename, destinationFolder
                , "diff_", USER, USER);
            if (compareResult != null) {
                NUnit.Framework.Assert.Fail(compareResult);
            }
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void EncryptAes256EncryptedStampingUpdate() {
            String filename = "encryptAes256EncryptedStampingUpdate.pdf";
            String src = sourceFolder + "encryptedWithPlainMetadata.pdf";
            String @out = destinationFolder + filename;
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(src, new ReaderProperties().SetPassword(OWNER)), new PdfWriter
                (@out, new WriterProperties().SetStandardEncryption(USER, OWNER, EncryptionConstants.ALLOW_PRINTING, EncryptionConstants
                .STANDARD_ENCRYPTION_40)), new StampingProperties());
            pdfDoc.Close();
            CompareTool compareTool = new CompareTool().EnableEncryptionCompare();
            String compareResult = compareTool.CompareByContent(@out, sourceFolder + "cmp_" + filename, destinationFolder
                , "diff_", USER, USER);
            if (compareResult != null) {
                NUnit.Framework.Assert.Fail(compareResult);
            }
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void EncryptAes256FullCompression() {
            String filename = "encryptAes256FullCompression.pdf";
            int encryptionType = EncryptionConstants.ENCRYPTION_AES_256;
            EncryptWithPassword2(filename, encryptionType, CompressionConstants.DEFAULT_COMPRESSION, true);
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void EncryptWithPasswordAes256Pdf2() {
            String filename = "encryptWithPasswordAes256Pdf2.pdf";
            int encryptionType = EncryptionConstants.ENCRYPTION_AES_256;
            EncryptWithPassword2(filename, encryptionType, CompressionConstants.DEFAULT_COMPRESSION, true);
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        [LogMessage(VersionConforming.DEPRECATED_ENCRYPTION_ALGORITHMS)]
        public virtual void EncryptWithPasswordAes128Pdf2() {
            String filename = "encryptWithPasswordAes128Pdf2.pdf";
            int encryptionType = EncryptionConstants.ENCRYPTION_AES_128;
            EncryptWithPassword2(filename, encryptionType, CompressionConstants.DEFAULT_COMPRESSION, true);
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        [LogMessage(VersionConforming.DEPRECATED_ENCRYPTION_ALGORITHMS)]
        public virtual void StampAndUpdateVersionPreserveStandard40() {
            String filename = "stampAndUpdateVersionPreserveStandard40.pdf";
            PdfDocument doc = new PdfDocument(new PdfReader(sourceFolder + "encryptedWithPasswordStandard40.pdf", new 
                ReaderProperties().SetPassword(OWNER)), new PdfWriter(destinationFolder + filename, new WriterProperties
                ().SetPdfVersion(PdfVersion.PDF_2_0)), new StampingProperties().PreserveEncryption());
            doc.Close();
            CompareEncryptedPdf(filename);
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        [LogMessage(VersionConforming.DEPRECATED_AES256_REVISION)]
        public virtual void StampAndUpdateVersionPreserveAes256() {
            String filename = "stampAndUpdateVersionPreserveAes256.pdf";
            PdfDocument doc = new PdfDocument(new PdfReader(sourceFolder + "encryptedWithPasswordAes256.pdf", new ReaderProperties
                ().SetPassword(OWNER)), new PdfWriter(destinationFolder + filename, new WriterProperties().SetPdfVersion
                (PdfVersion.PDF_2_0)), new StampingProperties().PreserveEncryption());
            doc.Close();
            CompareEncryptedPdf(filename);
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void StampAndUpdateVersionNewAes256() {
            String filename = "stampAndUpdateVersionNewAes256.pdf";
            PdfDocument doc = new PdfDocument(new PdfReader(sourceFolder + "encryptedWithPasswordAes256.pdf", new ReaderProperties
                ().SetPassword(OWNER)), new PdfWriter(destinationFolder + filename, new WriterProperties().SetPdfVersion
                (PdfVersion.PDF_2_0).SetStandardEncryption(USER, OWNER, 0, EncryptionConstants.ENCRYPTION_AES_256)));
            doc.Close();
            CompareEncryptedPdf(filename);
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void EncryptAes256Pdf2Permissions() {
            String filename = "encryptAes256Pdf2Permissions.pdf";
            int permissions = EncryptionConstants.ALLOW_FILL_IN | EncryptionConstants.ALLOW_SCREENREADERS | EncryptionConstants
                .ALLOW_DEGRADED_PRINTING;
            PdfDocument doc = new PdfDocument(new PdfWriter(destinationFolder + filename, new WriterProperties().SetPdfVersion
                (PdfVersion.PDF_2_0).SetStandardEncryption(USER, OWNER, permissions, EncryptionConstants.ENCRYPTION_AES_256
                )));
            doc.GetDocumentInfo().SetMoreInfo(customInfoEntryKey, customInfoEntryValue);
            WriteTextBytesOnPageContent(doc.AddNewPage(), pageTextContent);
            doc.Close();
            CompareEncryptedPdf(filename);
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void EncryptWithPasswordAes128NoMetadataCompression() {
            String srcFilename = "srcEncryptWithPasswordAes128NoMetadataCompression.pdf";
            PdfReader reader = new PdfReader(sourceFolder + srcFilename, new ReaderProperties());
            WriterProperties props = new WriterProperties().SetStandardEncryption("superuser".GetBytes(), "superowner"
                .GetBytes(), EncryptionConstants.ALLOW_PRINTING, EncryptionConstants.ENCRYPTION_AES_128 | EncryptionConstants
                .DO_NOT_ENCRYPT_METADATA);
            String outFilename = "encryptWithPasswordAes128NoMetadataCompression.pdf";
            PdfWriter writer = new PdfWriter(destinationFolder + outFilename, props);
            PdfDocument pdfDoc = new PdfDocument(reader, writer);
            pdfDoc.Close();
            CompareTool compareTool = new CompareTool();
            compareTool.EnableEncryptionCompare();
            compareTool.GetOutReaderProperties().SetPassword("superowner".GetBytes());
            compareTool.GetCmpReaderProperties().SetPassword("superowner".GetBytes());
            String outPdf = destinationFolder + outFilename;
            String cmpPdf = sourceFolder + "cmp_" + outFilename;
            NUnit.Framework.Assert.IsNull(compareTool.CompareByContent(outPdf, cmpPdf, destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void CheckMD5LogAbsenceInUnapprovedMode() {
            NUnit.Framework.Assume.That(!FACTORY.IsInApprovedOnlyMode());
            String fileName = "noUserPassword.pdf";
            using (PdfDocument document = new PdfDocument(new PdfReader(sourceFolder + fileName))) {
            }
        }

        // this test checks log message absence
        public virtual void EncryptWithPassword2(String filename, int encryptionType, int compression) {
            EncryptWithPassword2(filename, encryptionType, compression, false);
        }

        public virtual void EncryptWithPassword2(String filename, int encryptionType, int compression, bool isPdf2
            ) {
            int permissions = EncryptionConstants.ALLOW_SCREENREADERS;
            WriterProperties writerProperties = new WriterProperties().SetStandardEncryption(USER, OWNER, permissions, 
                encryptionType);
            if (isPdf2) {
                writerProperties.SetPdfVersion(PdfVersion.PDF_2_0);
            }
            PdfWriter writer = new PdfWriter(destinationFolder + filename, writerProperties.AddXmpMetadata());
            writer.SetCompressionLevel(compression);
            PdfDocument document = new PdfDocument(writer);
            document.GetDocumentInfo().SetMoreInfo(customInfoEntryKey, customInfoEntryValue);
            PdfPage page = document.AddNewPage();
            WriteTextBytesOnPageContent(page, pageTextContent);
            page.Flush();
            document.Close();
            CompareEncryptedPdf(filename);
            CheckEncryptedWithPasswordDocumentStamping(filename, OWNER);
            CheckEncryptedWithPasswordDocumentAppending(filename, OWNER);
        }

        public virtual void EncryptWithPassword(String filename, int encryptionType, int compression) {
            EncryptWithPassword(filename, encryptionType, compression, false);
        }

        public virtual void EncryptWithPassword(String filename, int encryptionType, int compression, bool fullCompression
            ) {
            String outFileName = destinationFolder + filename;
            int permissions = EncryptionConstants.ALLOW_SCREENREADERS;
            PdfWriter writer = new PdfWriter(outFileName, new WriterProperties().SetStandardEncryption(USER, OWNER, permissions
                , encryptionType).AddXmpMetadata().SetFullCompressionMode(fullCompression));
            writer.SetCompressionLevel(compression);
            PdfDocument document = new PdfDocument(writer);
            document.GetDocumentInfo().SetMoreInfo(customInfoEntryKey, customInfoEntryValue);
            PdfPage page = document.AddNewPage();
            WriteTextBytesOnPageContent(page, pageTextContent);
            page.Flush();
            document.Close();
            CompareEncryptedPdf(filename);
            CheckEncryptedWithPasswordDocumentStamping(filename, OWNER);
            CheckEncryptedWithPasswordDocumentAppending(filename, OWNER);
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
                NUnit.Framework.Assert.Fail(compareResult);
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

        public static void CheckDecryptedWithPasswordContent(String src, byte[] password, String pageContent) {
            CheckDecryptedWithPasswordContent(src, password, pageContent, false);
        }

        private static void CheckDecryptedWithPasswordContent(String src, byte[] password, String pageContent, bool
             expectError) {
            PdfReader reader = new PdfReader(src, new ReaderProperties().SetPassword(password));
            PdfDocument document = new PdfDocument(reader);
            PdfPage page = document.GetPage(1);
            bool expectedContentFound = iText.Commons.Utils.JavaUtil.GetStringForBytes(page.GetStreamBytes(0)).Contains
                (pageContent);
            String actualCustomInfoEntry = document.GetTrailer().GetAsDictionary(PdfName.Info).GetAsString(new PdfName
                (customInfoEntryKey)).ToUnicodeString();
            if (!expectError) {
                NUnit.Framework.Assert.IsTrue(expectedContentFound, "Expected content: \n" + pageContent);
                NUnit.Framework.Assert.AreEqual(customInfoEntryValue, actualCustomInfoEntry, "Encrypted custom");
            }
            else {
                NUnit.Framework.Assert.IsFalse(expectedContentFound, "Expected content: \n" + pageContent);
                NUnit.Framework.Assert.AreNotEqual(customInfoEntryValue, actualCustomInfoEntry, "Encrypted custom");
            }
            document.Close();
        }

        public virtual void CheckDecryptedWithCertificateContent(String filename, IX509Certificate certificate, String
             pageContent) {
            String src = destinationFolder + filename;
            PdfReader reader = new PdfReader(src, new ReaderProperties().SetPublicKeySecurityParams(certificate, GetPrivateKey
                ()));
            PdfDocument document = new PdfDocument(reader);
            PdfPage page = document.GetPage(1);
            String s = iText.Commons.Utils.JavaUtil.GetStringForBytes(page.GetStreamBytes(0));
            NUnit.Framework.Assert.IsTrue(s.Contains(pageContent), "Expected content: \n" + pageContent);
            NUnit.Framework.Assert.AreEqual(customInfoEntryValue, document.GetTrailer().GetAsDictionary(PdfName.Info).
                GetAsString(new PdfName(customInfoEntryKey)).ToUnicodeString(), "Encrypted custom");
            document.Close();
        }

        // basically this is comparing content of decrypted by itext document with content of encrypted document
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
                NUnit.Framework.Assert.Fail(compareResult);
            }
        }

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

        internal static void WriteTextBytesOnPageContent(PdfPage page, String text) {
            page.GetFirstContentStream().GetOutputStream().WriteBytes(("q\n" + "BT\n" + "36 706 Td\n" + "0 0 Td\n" + "/F1 24 Tf\n"
                 + "(" + text + ")Tj\n" + "0 0 Td\n" + "ET\n" + "Q ").GetBytes(iText.Commons.Utils.EncodingUtil.ISO_8859_1
                ));
            page.GetResources().AddFont(page.GetDocument(), PdfFontFactory.CreateFont(StandardFonts.HELVETICA));
        }

        internal static void CompareEncryptedPdf(String filename) {
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
