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
using System.IO;
using iText.Commons.Utils;
using iText.Kernel.Crypto.Pdfencryption;
using iText.Kernel.Exceptions;
using iText.Kernel.Pdf.Filespec;
using iText.Kernel.Utils;
using iText.Test;

namespace iText.Kernel.Pdf {
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfOutputStreamTest : ExtendedITextTest {
        public static readonly String DESTINATION_FOLDER = TestUtil.GetOutputPath() + "/kernel/pdf/PdfOutputStreamTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void InvalidDecodeParamsTest() {
            PdfWriter writer = new PdfWriter(new MemoryStream(), new WriterProperties().SetStandardEncryption(PdfEncryptionTestUtils
                .USER, PdfEncryptionTestUtils.OWNER, 0, EncryptionConstants.ENCRYPTION_AES_256 | EncryptionConstants.EMBEDDED_FILES_ONLY
                ));
            PdfDocument document = new PdfOutputStreamTest.CustomPdfDocument1(writer);
            document.AddFileAttachment("descripton", PdfFileSpec.CreateEmbeddedFileSpec(document, "TEST".GetBytes(System.Text.Encoding
                .UTF8), "descripton", "test.txt", null, null));
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => document.Close());
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(KernelExceptionMessageConstant.THIS_DECODE_PARAMETER_TYPE_IS_NOT_SUPPORTED
                , typeof(PdfName)), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void ArrayDecodeParamsTest() {
            String fileName = "arrayDecodeParamsTest.pdf";
            PdfWriter writer = CompareTool.CreateTestPdfWriter(DESTINATION_FOLDER + fileName, new WriterProperties().SetStandardEncryption
                (PdfEncryptionTestUtils.USER, PdfEncryptionTestUtils.OWNER, 0, EncryptionConstants.ENCRYPTION_AES_256 
                | EncryptionConstants.EMBEDDED_FILES_ONLY));
            PdfDocument document = new PdfOutputStreamTest.CustomPdfDocument2(writer);
            document.AddFileAttachment("descripton", PdfFileSpec.CreateEmbeddedFileSpec(document, "TEST".GetBytes(System.Text.Encoding
                .UTF8), "descripton", "test.txt", null, null));
            NUnit.Framework.Assert.DoesNotThrow(() => document.Close());
        }

        [NUnit.Framework.Test]
        public virtual void DictDecodeParamsTest() {
            String fileName = "dictDecodeParamsTest.pdf";
            PdfWriter writer = CompareTool.CreateTestPdfWriter(DESTINATION_FOLDER + fileName, new WriterProperties().SetStandardEncryption
                (PdfEncryptionTestUtils.USER, PdfEncryptionTestUtils.OWNER, 0, EncryptionConstants.ENCRYPTION_AES_256 
                | EncryptionConstants.EMBEDDED_FILES_ONLY));
            PdfDocument document = new PdfOutputStreamTest.CustomPdfDocument3(writer);
            document.AddFileAttachment("descripton", PdfFileSpec.CreateEmbeddedFileSpec(document, "TEST".GetBytes(System.Text.Encoding
                .UTF8), "descripton", "test.txt", null, null));
            NUnit.Framework.Assert.DoesNotThrow(() => document.Close());
        }

        private sealed class CustomPdfDocument1 : PdfDocument {
//\cond DO_NOT_DOCUMENT
            internal CustomPdfDocument1(PdfWriter writer)
                : base(writer) {
            }
//\endcond

            public override void MarkStreamAsEmbeddedFile(PdfStream stream) {
                stream.Put(PdfName.DecodeParms, PdfName.Crypt);
                stream.SetCompressionLevel(CompressionConstants.NO_COMPRESSION);
                base.MarkStreamAsEmbeddedFile(stream);
            }
        }

        private sealed class CustomPdfDocument2 : PdfDocument {
//\cond DO_NOT_DOCUMENT
            internal CustomPdfDocument2(PdfWriter writer)
                : base(writer) {
            }
//\endcond

            public override void MarkStreamAsEmbeddedFile(PdfStream stream) {
                PdfArray decodeParmsValue = new PdfArray();
                decodeParmsValue.Add(new PdfNull());
                stream.Put(PdfName.DecodeParms, decodeParmsValue);
                stream.SetCompressionLevel(CompressionConstants.NO_COMPRESSION);
                base.MarkStreamAsEmbeddedFile(stream);
            }
        }

        private sealed class CustomPdfDocument3 : PdfDocument {
//\cond DO_NOT_DOCUMENT
            internal CustomPdfDocument3(PdfWriter writer)
                : base(writer) {
            }
//\endcond

            public override void MarkStreamAsEmbeddedFile(PdfStream stream) {
                PdfDictionary decodeParmsValue = new PdfDictionary();
                decodeParmsValue.Put(PdfName.Name, new PdfNull());
                stream.Put(PdfName.DecodeParms, decodeParmsValue);
                stream.SetCompressionLevel(CompressionConstants.NO_COMPRESSION);
                base.MarkStreamAsEmbeddedFile(stream);
            }
        }
    }
}
