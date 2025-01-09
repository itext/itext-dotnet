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
using iText.Kernel.Geom;
using iText.Kernel.Logs;
using iText.Kernel.Pdf.Annot;
using iText.Kernel.Pdf.Filespec;
using iText.Kernel.Utils;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Kernel.Pdf {
    [NUnit.Framework.Category("BouncyCastleIntegrationTest")]
    public class EncryptedEmbeddedStreamsHandlerTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/pdf/EncryptedEmbeddedStreamsHandlerTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/kernel/pdf/EncryptedEmbeddedStreamsHandlerTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.OneTimeTearDown]
        public static void AfterClass() {
            CompareTool.Cleanup(destinationFolder);
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void NoReaderStandardEncryptionAddFileAttachment() {
            String outFileName = destinationFolder + "noReaderStandardEncryptionAddFileAttachment.pdf";
            String cmpFileName = sourceFolder + "cmp_noReaderStandardEncryptionAddFileAttachment.pdf";
            PdfDocument pdfDocument = CreateEncryptedDocument(EncryptionConstants.STANDARD_ENCRYPTION_128, outFileName
                );
            PdfFileSpec fs = PdfFileSpec.CreateEmbeddedFileSpec(pdfDocument, "file".GetBytes(), "description", "file.txt"
                , null, null, null);
            pdfDocument.AddFileAttachment("file.txt", fs);
            pdfDocument.AddNewPage();
            pdfDocument.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff", "password".GetBytes(), "password".GetBytes()));
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void NoReaderAesEncryptionAddFileAttachment() {
            String outFileName = destinationFolder + "noReaderAesEncryptionAddFileAttachment.pdf";
            String cmpFileName = sourceFolder + "cmp_noReaderAesEncryptionAddFileAttachment.pdf";
            PdfDocument pdfDocument = CreateEncryptedDocument(EncryptionConstants.ENCRYPTION_AES_128, outFileName);
            PdfFileSpec fs = PdfFileSpec.CreateEmbeddedFileSpec(pdfDocument, "file".GetBytes(), "description", "file.txt"
                , null, null, null);
            pdfDocument.AddFileAttachment("file.txt", fs);
            pdfDocument.AddNewPage();
            pdfDocument.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff", "password".GetBytes(), "password".GetBytes()));
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void WithReaderStandardEncryptionAddFileAttachment() {
            String outFileName = destinationFolder + "withReaderStandardEncryptionAddFileAttachment.pdf";
            String cmpFileName = sourceFolder + "cmp_withReaderStandardEncryptionAddFileAttachment.pdf";
            PdfReader reader = new PdfReader(sourceFolder + "pdfWithFileAttachments.pdf", new ReaderProperties().SetPassword
                ("password".GetBytes()));
            // Setting compression level to zero doesn't affect the encryption at any level.
            // We do it to simplify observation of the resultant PDF.
            PdfDocument pdfDocument = new PdfDocument(reader, CompareTool.CreateTestPdfWriter(outFileName).SetCompressionLevel
                (0));
            PdfFileSpec fs = PdfFileSpec.CreateEmbeddedFileSpec(pdfDocument, "file".GetBytes(), "description", "file.txt"
                , null, null, null);
            pdfDocument.AddFileAttachment("file.txt", fs);
            pdfDocument.AddNewPage();
            pdfDocument.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void NoReaderStandardEncryptionAddAnnotation() {
            String outFileName = destinationFolder + "noReaderStandardEncryptionAddAnnotation.pdf";
            String cmpFileName = sourceFolder + "cmp_noReaderStandardEncryptionAddAnnotation.pdf";
            PdfDocument pdfDocument = CreateEncryptedDocument(EncryptionConstants.STANDARD_ENCRYPTION_128, outFileName
                );
            pdfDocument.AddNewPage();
            PdfFileSpec fs = PdfFileSpec.CreateEmbeddedFileSpec(pdfDocument, "file".GetBytes(), "description", "file.txt"
                , null, null, null);
            pdfDocument.GetPage(1).AddAnnotation(new PdfFileAttachmentAnnotation(new Rectangle(100, 100), fs));
            pdfDocument.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff", "password".GetBytes(), "password".GetBytes()));
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void WithReaderStandardEncryptionAddAnnotation() {
            String outFileName = destinationFolder + "withReaderStandardEncryptionAddAnnotation.pdf";
            String cmpFileName = sourceFolder + "cmp_withReaderStandardEncryptionAddAnnotation.pdf";
            PdfReader reader = new PdfReader(sourceFolder + "pdfWithFileAttachmentAnnotations.pdf", new ReaderProperties
                ().SetPassword("password".GetBytes()));
            // Setting compression level to zero doesn't affect the encryption at any level.
            // We do it to simplify observation of the resultant PDF.
            PdfDocument pdfDocument = new PdfDocument(reader, CompareTool.CreateTestPdfWriter(outFileName).SetCompressionLevel
                (0));
            pdfDocument.AddNewPage();
            PdfFileSpec fs = PdfFileSpec.CreateEmbeddedFileSpec(pdfDocument, "file".GetBytes(), "description", "file.txt"
                , null, null, null);
            pdfDocument.GetPage(1).AddAnnotation(new PdfFileAttachmentAnnotation(new Rectangle(100, 100), fs));
            pdfDocument.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.MD5_IS_NOT_FIPS_COMPLIANT, Ignore = true)]
        public virtual void ReaderWithoutEncryptionWriterStandardEncryption() {
            String outFileName = destinationFolder + "readerWithoutEncryptionWriterStandardEncryption.pdf";
            String cmpFileName = sourceFolder + "cmp_readerWithoutEncryptionWriterStandardEncryption.pdf";
            PdfReader reader = new PdfReader(sourceFolder + "pdfWithUnencryptedAttachmentAnnotations.pdf");
            PdfDocument pdfDocument = CreateEncryptedDocument(reader, EncryptionConstants.STANDARD_ENCRYPTION_128, outFileName
                );
            PdfFileSpec fs = PdfFileSpec.CreateEmbeddedFileSpec(pdfDocument, "file".GetBytes(), "description", "file.txt"
                , null, null, null);
            pdfDocument.AddFileAttachment("new attachment", fs);
            pdfDocument.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff", "password".GetBytes(), "password".GetBytes()));
        }

        private PdfDocument CreateEncryptedDocument(int encryptionAlgorithm, String outFileName) {
            PdfWriter writer = CompareTool.CreateTestPdfWriter(outFileName, new WriterProperties().SetStandardEncryption
                ("password".GetBytes(), "password".GetBytes(), 0, encryptionAlgorithm | EncryptionConstants.EMBEDDED_FILES_ONLY
                ));
            // Setting compression level to zero doesn't affect the encryption at any level.
            // We do it to simplify observation of the resultant PDF.
            writer.SetCompressionLevel(0);
            return new PdfDocument(writer);
        }

        private PdfDocument CreateEncryptedDocument(PdfReader reader, int encryptionAlgorithm, String outFileName) {
            PdfWriter writer = CompareTool.CreateTestPdfWriter(outFileName, new WriterProperties().SetStandardEncryption
                ("password".GetBytes(), "password".GetBytes(), 0, encryptionAlgorithm | EncryptionConstants.EMBEDDED_FILES_ONLY
                ));
            // Setting compression level to zero doesn't affect the encryption at any level.
            // We do it to simplify observation of the resultant PDF.
            writer.SetCompressionLevel(0);
            return new PdfDocument(reader, writer);
        }
    }
}
