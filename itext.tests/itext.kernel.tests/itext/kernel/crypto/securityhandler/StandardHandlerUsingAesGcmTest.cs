/*
This file is part of the iText (R) project.
Copyright (c) 1998-2025 Apryse Group NV
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
using iText.Commons.Bouncycastle;
using iText.Kernel.Exceptions;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Kernel.Utils.Objectpathitems;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Kernel.Crypto.Securityhandler {
    [NUnit.Framework.Category("BouncyCastleIntegrationTest")]
    public class StandardHandlerUsingAesGcmTest : ExtendedITextTest {
        public static readonly String SRC = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/crypto/securityhandler/StandardHandlerUsingAesGcmTest/";

        public static readonly String DEST = NUnit.Framework.TestContext.CurrentContext.TestDirectory + "/test/itext/kernel/crypto/securityhandler/StandardHandlerUsingAesGcmTest/";

        private static readonly IBouncyCastleFactory FACTORY = BouncyCastleFactoryCreator.GetFactory();

        private static readonly byte[] OWNER_PASSWORD = "supersecret".GetBytes(System.Text.Encoding.UTF8);

        private static readonly byte[] USER_PASSWORD = "secret".GetBytes(System.Text.Encoding.UTF8);

        [NUnit.Framework.OneTimeSetUp]
        public static void SetUp() {
            CreateOrClearDestinationFolder(DEST);
        }

        [NUnit.Framework.Test]
        public virtual void SimpleEncryptDecryptTest() {
            String srcFile = SRC + "simpleDocument.pdf";
            String encryptedCmpFile = SRC + "cmp_encryptedSimpleDocument.pdf";
            String outFile = DEST + "simpleEncryptDecrypt.pdf";
            // Set usage permissions.
            int perms = EncryptionConstants.ALLOW_PRINTING | EncryptionConstants.ALLOW_DEGRADED_PRINTING;
            WriterProperties wProps = new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0).SetStandardEncryption(USER_PASSWORD
                , OWNER_PASSWORD, perms, EncryptionConstants.ENCRYPTION_AES_GCM);
            // Instantiate input/output document.
            using (PdfDocument docIn = new PdfDocument(new PdfReader(srcFile))) {
                using (PdfDocument docOut = new PdfDocument(new PdfWriter(outFile, wProps))) {
                    // Copy one page from input to output.
                    docIn.CopyPagesTo(1, 1, docOut);
                }
            }
            new CToolNoDeveloperExtension().CompareByContent(outFile, srcFile, DEST, "diff", USER_PASSWORD, null);
            new CompareTool().CompareByContent(outFile, encryptedCmpFile, DEST, "diff", USER_PASSWORD, USER_PASSWORD);
        }

        [NUnit.Framework.Test]
        [LogMessage(VersionConforming.NOT_SUPPORTED_AES_GCM)]
        public virtual void SimpleEncryptDecryptPdf15Test() {
            String srcFile = SRC + "simpleDocument.pdf";
            String outFile = DEST + "notSupportedVersionDocument.pdf";
            int perms = EncryptionConstants.ALLOW_PRINTING | EncryptionConstants.ALLOW_DEGRADED_PRINTING;
            WriterProperties wProps = new WriterProperties().SetStandardEncryption(USER_PASSWORD, OWNER_PASSWORD, perms
                , EncryptionConstants.ENCRYPTION_AES_GCM);
            PdfDocument ignored = new PdfDocument(new PdfReader(srcFile), new PdfWriter(outFile, wProps));
            ignored.Close();
            new CToolNoDeveloperExtension().CompareByContent(outFile, srcFile, DEST, "diff", USER_PASSWORD, null);
        }

        [NUnit.Framework.Test]
        public virtual void KnownOutputTest() {
            String srcFile = SRC + "encryptedDocument.pdf";
            String outFile = DEST + "encryptedDocument.pdf";
            String cmpFile = SRC + "simpleDocument.pdf";
            using (PdfDocument ignored = new PdfDocument(new PdfReader(srcFile, new ReaderProperties().SetPassword(OWNER_PASSWORD
                )), new PdfWriter(outFile))) {
            }
            // We need to copy the source file to the destination folder to be able to compare pdf files in android.
            new CompareTool().CompareByContent(outFile, cmpFile, DEST, "diff", USER_PASSWORD, null);
        }

        // In all these tampered files, the stream content of object 14 has been modified.
        [NUnit.Framework.Test]
        public virtual void MacTamperedTest() {
            String srcFile = SRC + "encryptedDocumentTamperedMac.pdf";
            AssertTampered(srcFile);
        }

        [NUnit.Framework.Test]
        public virtual void InitVectorTamperedTest() {
            String srcFile = SRC + "encryptedDocumentTamperedIv.pdf";
            AssertTampered(srcFile);
        }

        [NUnit.Framework.Test]
        public virtual void CiphertextTamperedTest() {
            String srcFile = SRC + "encryptedDocumentTamperedCiphertext.pdf";
            AssertTampered(srcFile);
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.ENCRYPTION_ENTRIES_P_AND_ENCRYPT_METADATA_NOT_CORRESPOND_PERMS_ENTRY
            )]
        public virtual void PdfEncryptionWithEmbeddedFilesTest() {
            byte[] documentId = new byte[] { (byte)88, (byte)189, (byte)192, (byte)48, (byte)240, (byte)200, (byte)87, 
                (byte)183, (byte)244, (byte)119, (byte)224, (byte)109, (byte)226, (byte)173, (byte)32, (byte)90 };
            byte[] password = new byte[] { (byte)115, (byte)101, (byte)99, (byte)114, (byte)101, (byte)116 };
            Dictionary<PdfName, PdfObject> encMap = new Dictionary<PdfName, PdfObject>();
            encMap.Put(PdfName.R, new PdfNumber(7));
            encMap.Put(PdfName.V, new PdfNumber(6));
            encMap.Put(PdfName.P, new PdfNumber(-1852));
            encMap.Put(PdfName.EFF, PdfName.FlateDecode);
            encMap.Put(PdfName.StmF, PdfName.Identity);
            encMap.Put(PdfName.StrF, PdfName.Identity);
            PdfDictionary embeddedFilesDict = new PdfDictionary();
            embeddedFilesDict.Put(PdfName.FlateDecode, new PdfDictionary());
            PdfDictionary cfmDict = new PdfDictionary();
            cfmDict.Put(PdfName.CFM, PdfName.AESV4);
            embeddedFilesDict.Put(PdfName.StdCF, cfmDict);
            encMap.Put(PdfName.CF, embeddedFilesDict);
            encMap.Put(PdfName.EncryptMetadata, PdfBoolean.FALSE);
            encMap.Put(PdfName.O, new PdfString("\u0006¡Ê\u009A<@\u009DÔG\u0013&\u008C5r\u0096\u0081i!\u0091\u000Fªìh=±\u0091\u0006Að¨\u008D\"¼\u0018?õ\u001DNó»{y\u0091)\u0090vâý"
                ));
            encMap.Put(PdfName.U, new PdfString("ôY\u009DÃ\u0017Ý·Ü\u0097vØ\fJ\u0099c\u0004áÝ¹ÔB\u0084·9÷\u008F\u009D-¿xnkþ\u0086Æ\u0088º\u0086ÜTÿëÕï\u0018\u009D\u0016-"
                ));
            encMap.Put(PdfName.OE, new PdfString("5Ë\u009EUÔº\u0007 Nøß\u0094ä\u001DÄ_wnù\u001AKò-\u007F\u00ADQ²Ø \u001FSJ"
                ));
            encMap.Put(PdfName.UE, new PdfString("\u000B:\rÆ\u0004\u0094Ûìkþ,ôBS9ü\u001E³\u0088\u001D(\u0098ºÀ\u0010½\u0082.'`kñ"
                ));
            encMap.Put(PdfName.Perms, new PdfString("\u008F»\u0080.òç\u0011\u001Et\u0012\u00905\u001B\u0019\u0014«"));
            PdfDictionary dictionary = new PdfDictionary(encMap);
            PdfEncryption encryption = new PdfEncryption(dictionary, password, documentId);
            NUnit.Framework.Assert.IsTrue(encryption.IsEmbeddedFilesOnly());
        }

        [NUnit.Framework.Test]
        public virtual void PdfEncryptionWithMetadataTest() {
            byte[] documentId = new byte[] { (byte)88, (byte)189, (byte)192, (byte)48, (byte)240, (byte)200, (byte)87, 
                (byte)183, (byte)244, (byte)119, (byte)224, (byte)109, (byte)226, (byte)173, (byte)32, (byte)90 };
            byte[] password = new byte[] { (byte)115, (byte)101, (byte)99, (byte)114, (byte)101, (byte)116 };
            Dictionary<PdfName, PdfObject> encMap = new Dictionary<PdfName, PdfObject>();
            encMap.Put(PdfName.R, new PdfNumber(7));
            encMap.Put(PdfName.V, new PdfNumber(6));
            encMap.Put(PdfName.P, new PdfNumber(-1852));
            encMap.Put(PdfName.StmF, PdfName.StdCF);
            encMap.Put(PdfName.StrF, PdfName.StdCF);
            PdfDictionary embeddedFilesDict = new PdfDictionary();
            embeddedFilesDict.Put(PdfName.FlateDecode, new PdfDictionary());
            PdfDictionary cfmDict = new PdfDictionary();
            cfmDict.Put(PdfName.CFM, PdfName.AESV4);
            embeddedFilesDict.Put(PdfName.StdCF, cfmDict);
            encMap.Put(PdfName.CF, embeddedFilesDict);
            encMap.Put(PdfName.EncryptMetadata, PdfBoolean.TRUE);
            encMap.Put(PdfName.O, new PdfString("\u0006¡Ê\u009A<@\u009DÔG\u0013&\u008C5r\u0096\u0081i!\u0091\u000Fªìh=±\u0091\u0006Að¨\u008D\"¼\u0018?õ\u001DNó»{y\u0091)\u0090vâý"
                ));
            encMap.Put(PdfName.U, new PdfString("ôY\u009DÃ\u0017Ý·Ü\u0097vØ\fJ\u0099c\u0004áÝ¹ÔB\u0084·9÷\u008F\u009D-¿xnkþ\u0086Æ\u0088º\u0086ÜTÿëÕï\u0018\u009D\u0016-"
                ));
            encMap.Put(PdfName.OE, new PdfString("5Ë\u009EUÔº\u0007 Nøß\u0094ä\u001DÄ_wnù\u001AKò-\u007F\u00ADQ²Ø \u001FSJ"
                ));
            encMap.Put(PdfName.UE, new PdfString("\u000B:\rÆ\u0004\u0094Ûìkþ,ôBS9ü\u001E³\u0088\u001D(\u0098ºÀ\u0010½\u0082.'`kñ"
                ));
            encMap.Put(PdfName.Perms, new PdfString("\u008F»\u0080.òç\u0011\u001Et\u0012\u00905\u001B\u0019\u0014«"));
            PdfDictionary dictionary = new PdfDictionary(encMap);
            PdfEncryption encryption = new PdfEncryption(dictionary, password, documentId);
            NUnit.Framework.Assert.IsTrue(encryption.IsMetadataEncrypted());
        }

        [NUnit.Framework.Test]
        public virtual void EncryptPdfWithMissingCFTest() {
            byte[] documentId = new byte[] { (byte)88, (byte)189, (byte)192, (byte)48, (byte)240, (byte)200, (byte)87, 
                (byte)183, (byte)244, (byte)119, (byte)224, (byte)109, (byte)226, (byte)173, (byte)32, (byte)90 };
            byte[] password = new byte[] { (byte)115, (byte)101, (byte)99, (byte)114, (byte)101, (byte)116 };
            Dictionary<PdfName, PdfObject> encMap = new Dictionary<PdfName, PdfObject>();
            encMap.Put(PdfName.R, new PdfNumber(7));
            encMap.Put(PdfName.V, new PdfNumber(6));
            PdfDictionary dictionary = new PdfDictionary(encMap);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => new PdfEncryption(dictionary, password
                , documentId));
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.CF_NOT_FOUND_ENCRYPTION, e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void EncryptPdfWithMissingStdCFTest() {
            byte[] documentId = new byte[] { (byte)88, (byte)189, (byte)192, (byte)48, (byte)240, (byte)200, (byte)87, 
                (byte)183, (byte)244, (byte)119, (byte)224, (byte)109, (byte)226, (byte)173, (byte)32, (byte)90 };
            byte[] password = new byte[] { (byte)115, (byte)101, (byte)99, (byte)114, (byte)101, (byte)116 };
            Dictionary<PdfName, PdfObject> encMap = new Dictionary<PdfName, PdfObject>();
            encMap.Put(PdfName.R, new PdfNumber(7));
            encMap.Put(PdfName.V, new PdfNumber(6));
            PdfDictionary embeddedFilesDict = new PdfDictionary();
            embeddedFilesDict.Put(PdfName.FlateDecode, new PdfDictionary());
            PdfDictionary cfmDict = new PdfDictionary();
            cfmDict.Put(PdfName.CFM, PdfName.AESV4);
            encMap.Put(PdfName.CF, embeddedFilesDict);
            PdfDictionary dictionary = new PdfDictionary(encMap);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => new PdfEncryption(dictionary, password
                , documentId));
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.STDCF_NOT_FOUND_ENCRYPTION, e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void EncryptPdfWithMissingCFMTest() {
            byte[] documentId = new byte[] { (byte)88, (byte)189, (byte)192, (byte)48, (byte)240, (byte)200, (byte)87, 
                (byte)183, (byte)244, (byte)119, (byte)224, (byte)109, (byte)226, (byte)173, (byte)32, (byte)90 };
            byte[] password = new byte[] { (byte)115, (byte)101, (byte)99, (byte)114, (byte)101, (byte)116 };
            Dictionary<PdfName, PdfObject> encMap = new Dictionary<PdfName, PdfObject>();
            encMap.Put(PdfName.R, new PdfNumber(7));
            encMap.Put(PdfName.V, new PdfNumber(6));
            encMap.Put(PdfName.P, new PdfNumber(-1852));
            encMap.Put(PdfName.StmF, PdfName.StdCF);
            encMap.Put(PdfName.StrF, PdfName.StdCF);
            PdfDictionary embeddedFilesDict = new PdfDictionary();
            embeddedFilesDict.Put(PdfName.FlateDecode, new PdfDictionary());
            PdfDictionary cfmDict = new PdfDictionary();
            embeddedFilesDict.Put(PdfName.StdCF, cfmDict);
            encMap.Put(PdfName.CF, embeddedFilesDict);
            PdfDictionary dictionary = new PdfDictionary(encMap);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => new PdfEncryption(dictionary, password
                , documentId));
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.NO_COMPATIBLE_ENCRYPTION_FOUND, e.Message);
        }

        private void AssertTampered(String outFile) {
            using (PdfDocument pdfDoc = new PdfDocument(new PdfReader(outFile, new ReaderProperties().SetPassword(USER_PASSWORD
                )))) {
                PdfObject obj = pdfDoc.GetPdfObject(14);
                if (obj != null && obj.IsStream()) {
                    // Get decoded stream bytes.
                    NUnit.Framework.Assert.Catch(typeof(Exception), () => ((PdfStream)obj).GetBytes());
                }
            }
        }
    }

//\cond DO_NOT_DOCUMENT
    // Outside test class for porting
    internal class CToolNoDeveloperExtension : CompareTool {
        protected internal override bool CompareObjects(PdfObject outObj, PdfObject cmpObj, ObjectPath currentPath
            , CompareTool.CompareResult compareResult) {
            if (outObj != null && outObj.IsDictionary()) {
                if (((PdfDictionary)outObj).Get(PdfName.ISO_) != null) {
                    return true;
                }
            }
            if (cmpObj != null && cmpObj.IsDictionary()) {
                if (((PdfDictionary)cmpObj).Get(PdfName.ISO_) != null) {
                    return true;
                }
            }
            return base.CompareObjects(outObj, cmpObj, currentPath, compareResult);
        }
    }
//\endcond
}
