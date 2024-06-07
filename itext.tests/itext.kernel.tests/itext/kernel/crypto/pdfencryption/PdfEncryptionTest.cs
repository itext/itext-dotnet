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
using NUnit.Framework;
using iText.Bouncycastleconnector;
using iText.Commons.Bouncycastle;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Crypto;
using iText.Commons.Utils;
using iText.Kernel.Crypto;
using iText.Kernel.Crypto.Securityhandler;
using iText.Kernel.Exceptions;
using iText.Kernel.Logs;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Filespec;
using iText.Kernel.Utils;
using iText.Kernel.XMP;
using iText.Kernel.XMP.Properties;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Kernel.Crypto.Pdfencryption {
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
             + "/test/itext/kernel/crypto/pdfencryption/PdfEncryptionTest/";

        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/crypto/pdfencryption/PdfEncryptionTest/";

        public static readonly char[] PRIVATE_KEY_PASS = "testpassphrase".ToCharArray();

        public static readonly String CERT = sourceFolder + "test.cer";

        public static readonly String PRIVATE_KEY = sourceFolder + "test.pem";

        private IPrivateKey privateKey;

        internal PdfEncryptionTestUtils encryptionUtil = new PdfEncryptionTestUtils(destinationFolder, sourceFolder
            );

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.OneTimeTearDown]
        public static void AfterClass() {
            CompareTool.Cleanup(destinationFolder);
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
        public virtual void OpenEncryptedDocWithWrongCertificateAndPrivateKey() {
            using (PdfReader reader = new PdfReader(sourceFolder + "encryptedWithCertificateAes128.pdf", new ReaderProperties
                ().SetPublicKeySecurityParams(GetPublicCertificate(sourceFolder + "wrong.cer"), PemFileHelper.ReadPrivateKeyFromPemFile
                (FileUtil.GetInputStreamForFile(sourceFolder + "wrong.pem"), PRIVATE_KEY_PASS)))) {
                Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => new PdfDocument(reader));
                NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.BAD_CERTIFICATE_AND_KEY, e.Message);
            }
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void MetadataReadingInEncryptedDoc() {
            PdfReader reader = new PdfReader(sourceFolder + "encryptedWithPlainMetadata.pdf", new ReaderProperties().SetPassword
                (PdfEncryptionTestUtils.OWNER));
            PdfDocument doc = new PdfDocument(reader);
            XMPMeta xmpMeta = XMPMetaFactory.ParseFromBuffer(doc.GetXmpMetadata());
            XMPProperty creatorToolXmp = xmpMeta.GetProperty(XMPConst.NS_XMP, "CreatorTool");
            doc.Close();
            NUnit.Framework.Assert.IsNotNull(creatorToolXmp);
            NUnit.Framework.Assert.AreEqual("iText", creatorToolXmp.GetValue());
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void CopyEncryptedDocument() {
            // I don't know how this source doc was created. Currently it's not opening by Acrobat and Foxit.
            // If I recreate it using iText, decrypting it in bc-fips on dotnet will start failing. But we probably still
            // want this test.
            PdfDocument srcDoc = new PdfDocument(new PdfReader(sourceFolder + "encryptedWithCertificateAes128.pdf", new 
                ReaderProperties().SetPublicKeySecurityParams(GetPublicCertificate(CERT), GetPrivateKey())));
            String fileName = "copiedEncryptedDoc.pdf";
            PdfDocument destDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(destinationFolder + fileName));
            srcDoc.CopyPagesTo(1, 1, destDoc);
            PdfDictionary srcInfo = srcDoc.GetTrailer().GetAsDictionary(PdfName.Info);
            PdfDictionary destInfo = destDoc.GetTrailer().GetAsDictionary(PdfName.Info);
            if (destInfo == null) {
                destInfo = new PdfDictionary();
                destDoc.GetTrailer().Put(PdfName.Info, destInfo);
            }
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
            encryptionUtil.CheckDecryptedWithPasswordContent(sourceFolder + fileName, null, PdfEncryptionTestUtils.PAGE_TEXT_CONTENT
                );
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void StampDocNoUserPassword() {
            String fileName = "stampedNoPassword.pdf";
            using (PdfReader reader = new PdfReader(sourceFolder + "noUserPassword.pdf")) {
                using (PdfWriter writer = CompareTool.CreateTestPdfWriter(destinationFolder + fileName)) {
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
            PdfWriter writer = CompareTool.CreateTestPdfWriter(outFileName, new WriterProperties().SetStandardEncryption
                (PdfEncryptionTestUtils.USER, PdfEncryptionTestUtils.OWNER, permissions, encryptionType).AddXmpMetadata
                ());
            PdfDocument document = new PdfDocument(writer);
            document.GetDocumentInfo().SetMoreInfo(PdfEncryptionTestUtils.CUSTOM_INFO_ENTRY_KEY, PdfEncryptionTestUtils
                .CUSTOM_INFO_ENTRY_VALUE);
            PdfPage page = document.AddNewPage();
            String textContent = "Hello world!";
            PdfEncryptionTestUtils.WriteTextBytesOnPageContent(page, textContent);
            String descripton = "encryptedFile";
            String path = sourceFolder + "pageWithContent.pdf";
            document.AddFileAttachment(descripton, PdfFileSpec.CreateEmbeddedFileSpec(document, path, descripton, path
                , null, null));
            page.Flush();
            document.Close();
            //TODO DEVSIX-5355 Specific crypto filters for EFF StmF and StrF are not supported at the moment.
            // However we can read embedded files only mode.
            bool ERROR_IS_EXPECTED = false;
            encryptionUtil.CheckDecryptedWithPasswordContent(destinationFolder + filename, PdfEncryptionTestUtils.OWNER
                , textContent, ERROR_IS_EXPECTED);
            encryptionUtil.CheckDecryptedWithPasswordContent(destinationFolder + filename, PdfEncryptionTestUtils.USER
                , textContent, ERROR_IS_EXPECTED);
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
        public virtual void EncryptAes256EncryptedStampingUpdate() {
            String filename = "encryptAes256EncryptedStampingUpdate.pdf";
            String src = sourceFolder + "encryptedWithPlainMetadata.pdf";
            String @out = destinationFolder + filename;
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(src, new ReaderProperties().SetPassword(PdfEncryptionTestUtils
                .OWNER)), CompareTool.CreateTestPdfWriter(@out, new WriterProperties().SetStandardEncryption(PdfEncryptionTestUtils
                .USER, PdfEncryptionTestUtils.OWNER, EncryptionConstants.ALLOW_PRINTING, EncryptionConstants.STANDARD_ENCRYPTION_40
                )), new StampingProperties());
            pdfDoc.Close();
            CompareTool compareTool = new CompareTool().EnableEncryptionCompare();
            String compareResult = compareTool.CompareByContent(@out, sourceFolder + "cmp_" + filename, destinationFolder
                , "diff_", PdfEncryptionTestUtils.USER, PdfEncryptionTestUtils.USER);
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
        public virtual void StampAndUpdateVersionNewAes256() {
            String filename = "stampAndUpdateVersionNewAes256.pdf";
            PdfDocument doc = new PdfDocument(new PdfReader(sourceFolder + "encryptedWithPasswordAes256.pdf", new ReaderProperties
                ().SetPassword(PdfEncryptionTestUtils.OWNER)), CompareTool.CreateTestPdfWriter(destinationFolder + filename
                , new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0).SetStandardEncryption(PdfEncryptionTestUtils
                .USER, PdfEncryptionTestUtils.OWNER, 0, EncryptionConstants.ENCRYPTION_AES_256)));
            doc.Close();
            encryptionUtil.CompareEncryptedPdf(filename);
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void EncryptAes256Pdf2Permissions() {
            String filename = "encryptAes256Pdf2Permissions.pdf";
            int permissions = EncryptionConstants.ALLOW_FILL_IN | EncryptionConstants.ALLOW_SCREENREADERS | EncryptionConstants
                .ALLOW_DEGRADED_PRINTING;
            PdfDocument doc = new PdfDocument(CompareTool.CreateTestPdfWriter(destinationFolder + filename, new WriterProperties
                ().SetPdfVersion(PdfVersion.PDF_2_0).SetStandardEncryption(PdfEncryptionTestUtils.USER, PdfEncryptionTestUtils
                .OWNER, permissions, EncryptionConstants.ENCRYPTION_AES_256)));
            doc.GetDocumentInfo().SetMoreInfo(PdfEncryptionTestUtils.CUSTOM_INFO_ENTRY_KEY, PdfEncryptionTestUtils.CUSTOM_INFO_ENTRY_VALUE
                );
            PdfEncryptionTestUtils.WriteTextBytesOnPageContent(doc.AddNewPage(), PdfEncryptionTestUtils.PAGE_TEXT_CONTENT
                );
            doc.Close();
            encryptionUtil.CompareEncryptedPdf(filename);
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
            PdfWriter writer = CompareTool.CreateTestPdfWriter(destinationFolder + outFilename, props);
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
        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true, Count = 2)]
        public virtual void DecryptAdobeWithPasswordAes256() {
            String filename = System.IO.Path.Combine(sourceFolder + "AdobeAes256.pdf").ToString();
            DecryptWithPassword(filename, "user".GetBytes());
            DecryptWithPassword(filename, "owner".GetBytes());
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void DecodeDictionaryWithInvalidOwnerHashAes256() {
            PdfDictionary dictionary = new PdfDictionary();
            dictionary.Put(PdfName.R, new PdfNumber(0));
            //Setting password hash which exceeds 48 bytes and contains non 0 elements after first 48 bytes
            dictionary.Put(PdfName.O, new PdfString("Ä\u0010\u001D`¶\u0084nË»j{\fßò\u0089JàN*\u0090ø>No\u0099" + "\u0087J \u0013\"V\u008E\fT!\u0082\u0003\u009E£\u008Fc\u0004 ].\u008C\u009C\u009C\u0000"
                 + "\u0000\u0000\u0000\u0013\u0000\u0013\u0013\u0000\u0000\u0000\u0000\u0000\u0000\u0000\u0000" + "\u0000\u0000\u0000\u0000\u0000\u0000\u0000\u0000\u0000\u0000\u0000\u0000\u0000\u0000\u0013"
                ));
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => new StandardHandlerUsingAes256(dictionary
                , "owner".GetBytes()));
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.BAD_PASSWORD_HASH, e.InnerException.Message
                );
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void OpenEncryptedWithPasswordDocWithDefaultKeyLength() {
            using (PdfReader reader = new PdfReader(sourceFolder + "encryptedWithPasswordWithDefaultKeyLength.pdf", new 
                ReaderProperties().SetPassword("user".GetBytes(System.Text.Encoding.UTF8)))) {
                using (PdfDocument document = new PdfDocument(reader)) {
                    NUnit.Framework.Assert.IsFalse(document.GetTrailer().GetAsDictionary(PdfName.Encrypt).ContainsKey(PdfName.
                        Length));
                }
            }
        }

        public virtual void EncryptWithPassword2(String filename, int encryptionType, int compression) {
            EncryptWithPassword2(filename, encryptionType, compression, false);
        }

        public virtual void EncryptWithPassword2(String filename, int encryptionType, int compression, bool isPdf2
            ) {
            int permissions = EncryptionConstants.ALLOW_SCREENREADERS;
            WriterProperties writerProperties = new WriterProperties().SetStandardEncryption(PdfEncryptionTestUtils.USER
                , PdfEncryptionTestUtils.OWNER, permissions, encryptionType);
            if (isPdf2) {
                writerProperties.SetPdfVersion(PdfVersion.PDF_2_0);
            }
            PdfWriter writer = CompareTool.CreateTestPdfWriter(destinationFolder + filename, writerProperties.AddXmpMetadata
                ());
            writer.SetCompressionLevel(compression);
            PdfDocument document = new PdfDocument(writer);
            document.GetDocumentInfo().SetMoreInfo(PdfEncryptionTestUtils.CUSTOM_INFO_ENTRY_KEY, PdfEncryptionTestUtils
                .CUSTOM_INFO_ENTRY_VALUE);
            PdfPage page = document.AddNewPage();
            PdfEncryptionTestUtils.WriteTextBytesOnPageContent(page, PdfEncryptionTestUtils.PAGE_TEXT_CONTENT);
            page.Flush();
            document.Close();
            encryptionUtil.CompareEncryptedPdf(filename);
            CheckEncryptedWithPasswordDocumentStamping(filename, PdfEncryptionTestUtils.OWNER);
            CheckEncryptedWithPasswordDocumentAppending(filename, PdfEncryptionTestUtils.OWNER);
        }

        public virtual void EncryptWithPassword(String filename, int encryptionType, int compression, bool fullCompression
            ) {
            String outFileName = destinationFolder + filename;
            int permissions = EncryptionConstants.ALLOW_SCREENREADERS;
            PdfWriter writer = CompareTool.CreateTestPdfWriter(outFileName, new WriterProperties().SetStandardEncryption
                (PdfEncryptionTestUtils.USER, PdfEncryptionTestUtils.OWNER, permissions, encryptionType).AddXmpMetadata
                ().SetFullCompressionMode(fullCompression));
            writer.SetCompressionLevel(compression);
            PdfDocument document = new PdfDocument(writer);
            document.GetDocumentInfo().SetMoreInfo(PdfEncryptionTestUtils.CUSTOM_INFO_ENTRY_KEY, PdfEncryptionTestUtils
                .CUSTOM_INFO_ENTRY_VALUE);
            PdfPage page = document.AddNewPage();
            PdfEncryptionTestUtils.WriteTextBytesOnPageContent(page, PdfEncryptionTestUtils.PAGE_TEXT_CONTENT);
            page.Flush();
            document.Close();
            encryptionUtil.CompareEncryptedPdf(filename);
            CheckEncryptedWithPasswordDocumentStamping(filename, PdfEncryptionTestUtils.OWNER);
            CheckEncryptedWithPasswordDocumentAppending(filename, PdfEncryptionTestUtils.OWNER);
        }

        public virtual IX509Certificate GetPublicCertificate(String path) {
            Stream @is = FileUtil.GetInputStreamForFile(path);
            return CryptoUtil.ReadPublicCertificate(@is);
        }

        public virtual IPrivateKey GetPrivateKey() {
            if (privateKey == null) {
                privateKey = PemFileHelper.ReadPrivateKeyFromPemFile(FileUtil.GetInputStreamForFile(PRIVATE_KEY), PRIVATE_KEY_PASS
                    );
            }
            return privateKey;
        }

        // basically this is comparing content of decrypted by itext document with content of encrypted document
        public virtual void CheckEncryptedWithPasswordDocumentStamping(String filename, byte[] password) {
            String srcFileName = destinationFolder + filename;
            String outFileName = destinationFolder + "stamped_" + filename;
            PdfReader reader = CompareTool.CreateOutputReader(srcFileName, new ReaderProperties().SetPassword(password
                ));
            PdfDocument document = new PdfDocument(reader, CompareTool.CreateTestPdfWriter(outFileName));
            document.Close();
            CompareTool compareTool = new CompareTool();
            String compareResult = compareTool.CompareByContent(outFileName, sourceFolder + "cmp_" + filename, destinationFolder
                , "diff_", PdfEncryptionTestUtils.USER, PdfEncryptionTestUtils.USER);
            if (compareResult != null) {
                NUnit.Framework.Assert.Fail(compareResult);
            }
        }

        public virtual void CheckEncryptedWithPasswordDocumentAppending(String filename, byte[] password) {
            String srcFileName = destinationFolder + filename;
            String outFileName = destinationFolder + "appended_" + filename;
            PdfReader reader = CompareTool.CreateOutputReader(srcFileName, new ReaderProperties().SetPassword(password
                ));
            PdfDocument document = new PdfDocument(reader, CompareTool.CreateTestPdfWriter(outFileName), new StampingProperties
                ().UseAppendMode());
            PdfPage newPage = document.AddNewPage();
            newPage.Put(PdfName.Default, new PdfString("Hello world string"));
            PdfEncryptionTestUtils.WriteTextBytesOnPageContent(newPage, "Hello world page_2!");
            document.Close();
            CompareTool compareTool = new CompareTool().EnableEncryptionCompare();
            String compareResult = compareTool.CompareByContent(outFileName, sourceFolder + "cmp_appended_" + filename
                , destinationFolder, "diff_", PdfEncryptionTestUtils.USER, PdfEncryptionTestUtils.USER);
            if (compareResult != null) {
                NUnit.Framework.Assert.Fail(compareResult);
            }
        }

        private void DecryptWithPassword(String fileName, byte[] password) {
            ReaderProperties readerProperties = new ReaderProperties().SetPassword(password);
            using (PdfReader reader = new PdfReader(fileName, readerProperties)) {
                using (PdfDocument pdfDocument = new PdfDocument(reader)) {
                    NUnit.Framework.Assert.IsTrue(PdfTextExtractor.GetTextFromPage(pdfDocument.GetFirstPage()).StartsWith("Content encrypted by "
                        ));
                }
            }
        }
    }
}
