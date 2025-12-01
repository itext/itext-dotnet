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

        [NUnit.Framework.Test]
        public virtual void SingleFilterNoDecodeChangesNothing() {
            PdfOutputStreamTest.CustomPdfStream stream = new PdfOutputStreamTest.CustomPdfStream(new MemoryStream(10));
            PdfStream pdfStream = new PdfStream();
            stream.UpdateCompressionFilter(pdfStream);
            NUnit.Framework.Assert.AreEqual(PdfName.FlateDecode, pdfStream.GetAsName(PdfName.Filter));
            NUnit.Framework.Assert.IsNull(pdfStream.Get(PdfName.DecodeParms));
        }

        [NUnit.Framework.Test]
        public virtual void WithoutFilterAndWithDecodeParamsRemovesDecodeParamsAndAddsFilter() {
            PdfOutputStreamTest.CustomPdfStream stream = new PdfOutputStreamTest.CustomPdfStream(new MemoryStream(10));
            PdfStream pdfStream = new PdfStream();
            PdfDictionary decodeParms = new PdfDictionary();
            decodeParms.Put(PdfName.Predictor, new PdfNumber(12));
            pdfStream.Put(PdfName.DecodeParms, decodeParms);
            stream.UpdateCompressionFilter(pdfStream);
            NUnit.Framework.Assert.AreEqual(PdfName.FlateDecode, pdfStream.GetAsName(PdfName.Filter));
            NUnit.Framework.Assert.IsNull(pdfStream.Get(PdfName.DecodeParms));
        }

        [NUnit.Framework.Test]
        public virtual void FilterAlreadyExistsAddItConvertsItToArray() {
            PdfOutputStreamTest.CustomPdfStream stream = new PdfOutputStreamTest.CustomPdfStream(new MemoryStream(10));
            PdfStream pdfStream = new PdfStream();
            pdfStream.Put(PdfName.Filter, PdfName.FlateDecode);
            stream.UpdateCompressionFilter(pdfStream);
            PdfArray filterArray = pdfStream.GetAsArray(PdfName.Filter);
            NUnit.Framework.Assert.AreEqual(2, filterArray.Size());
            NUnit.Framework.Assert.AreEqual(PdfName.FlateDecode, filterArray.GetAsName(0));
            NUnit.Framework.Assert.AreEqual(PdfName.FlateDecode, filterArray.GetAsName(1));
            PdfArray decodeParmsArray = pdfStream.GetAsArray(PdfName.DecodeParms);
            NUnit.Framework.Assert.IsNull(decodeParmsArray);
        }

        [NUnit.Framework.Test]
        public virtual void FilterArrayExistsAddsNewFilterAtTheEnd() {
            PdfOutputStreamTest.CustomPdfStream stream = new PdfOutputStreamTest.CustomPdfStream(new MemoryStream(10));
            PdfStream pdfStream = new PdfStream();
            PdfArray filterArray = new PdfArray();
            filterArray.Add(PdfName.LZWDecode);
            pdfStream.Put(PdfName.Filter, filterArray);
            PdfArray decodeParmsArray = new PdfArray();
            decodeParmsArray.Add(new PdfNumber(20));
            pdfStream.Put(PdfName.DecodeParms, decodeParmsArray);
            stream.UpdateCompressionFilter(pdfStream);
            PdfArray updatedFilterArray = pdfStream.GetAsArray(PdfName.Filter);
            NUnit.Framework.Assert.AreEqual(2, updatedFilterArray.Size());
            //new filter should be added at the beginning
            NUnit.Framework.Assert.AreEqual(PdfName.FlateDecode, updatedFilterArray.GetAsName(0));
            NUnit.Framework.Assert.AreEqual(PdfName.LZWDecode, updatedFilterArray.GetAsName(1));
            PdfArray updatedDecodeParmsArray = pdfStream.GetAsArray(PdfName.DecodeParms);
            NUnit.Framework.Assert.AreEqual(2, updatedDecodeParmsArray.Size());
            //new decode parms should be added at the beginning
            NUnit.Framework.Assert.IsTrue(updatedDecodeParmsArray.Get(0) is PdfNull);
            NUnit.Framework.Assert.AreEqual(new PdfNumber(20), updatedDecodeParmsArray.GetAsNumber(1));
        }

        [NUnit.Framework.Test]
        public virtual void FilterWith3AlreadyExistingFiltersButNoDecodeBackFillsDecodeParams() {
            PdfOutputStreamTest.CustomPdfStream stream = new PdfOutputStreamTest.CustomPdfStream(new MemoryStream(10));
            PdfStream pdfStream = new PdfStream();
            PdfArray filterArray = new PdfArray();
            filterArray.Add(PdfName.LZWDecode);
            filterArray.Add(PdfName.FlateDecode);
            filterArray.Add(PdfName.ASCII85Decode);
            pdfStream.Put(PdfName.Filter, filterArray);
            stream.UpdateCompressionFilter(pdfStream);
            PdfArray updatedFilterArray = pdfStream.GetAsArray(PdfName.Filter);
            NUnit.Framework.Assert.AreEqual(4, updatedFilterArray.Size());
            //new filter should be added at the beginning
            NUnit.Framework.Assert.AreEqual(PdfName.FlateDecode, updatedFilterArray.GetAsName(0));
            NUnit.Framework.Assert.AreEqual(PdfName.LZWDecode, updatedFilterArray.GetAsName(1));
            NUnit.Framework.Assert.AreEqual(PdfName.FlateDecode, updatedFilterArray.GetAsName(2));
            NUnit.Framework.Assert.AreEqual(PdfName.ASCII85Decode, updatedFilterArray.GetAsName(3));
            PdfArray updatedDecodeParmsArray = pdfStream.GetAsArray(PdfName.DecodeParms);
            NUnit.Framework.Assert.IsNull(updatedDecodeParmsArray);
        }

        private sealed class CustomPdfStream : PdfOutputStream {
            public CustomPdfStream(Stream outputStream)
                : base(outputStream) {
            }
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
