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
using iText.Kernel.Exceptions;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Kernel.Pdf {
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfReaderDecodeTest : ExtendedITextTest {
        public static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/pdf/PdfReaderDecodeTest/";

        [NUnit.Framework.Test]
        public virtual void NoMemoryHandlerTest() {
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new MemoryStream()))) {
                using (Stream @is = FileUtil.GetInputStreamForFile(SOURCE_FOLDER + "stream")) {
                    byte[] b = new byte[51];
                    @is.Read(b);
                    PdfArray array = new PdfArray();
                    PdfStream stream = new PdfStream(b);
                    stream.Put(PdfName.Filter, array);
                    stream.MakeIndirect(pdfDocument);
                    NUnit.Framework.Assert.AreEqual(51, PdfReader.DecodeBytes(b, stream).Length);
                    array.Add(PdfName.Fl);
                    NUnit.Framework.Assert.AreEqual(40, PdfReader.DecodeBytes(b, stream).Length);
                    array.Add(PdfName.Fl);
                    NUnit.Framework.Assert.AreEqual(992, PdfReader.DecodeBytes(b, stream).Length);
                    array.Add(PdfName.Fl);
                    NUnit.Framework.Assert.AreEqual(1000000, PdfReader.DecodeBytes(b, stream).Length);
                    // needed to close the document
                    pdfDocument.AddNewPage();
                }
            }
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.INVALID_INDIRECT_REFERENCE)]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.XREF_ERROR_WHILE_READING_TABLE_WILL_BE_REBUILT_WITH_CAUSE)]
        public virtual void DefaultMemoryHandlerTest() {
            using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(SOURCE_FOLDER + "timing.pdf"), new PdfWriter
                (new MemoryStream()))) {
                PdfStream stream = pdfDocument.GetFirstPage().GetContentStream(0);
                byte[] b = stream.GetBytes(false);
                PdfArray array = new PdfArray();
                stream.Put(PdfName.Filter, array);
                NUnit.Framework.Assert.AreEqual(51, PdfReader.DecodeBytes(b, stream).Length);
                array.Add(PdfName.Fl);
                NUnit.Framework.Assert.AreEqual(40, PdfReader.DecodeBytes(b, stream).Length);
                array.Add(PdfName.Fl);
                NUnit.Framework.Assert.AreEqual(992, PdfReader.DecodeBytes(b, stream).Length);
                array.Add(PdfName.Fl);
                NUnit.Framework.Assert.AreEqual(1000000, PdfReader.DecodeBytes(b, stream).Length);
            }
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.INVALID_INDIRECT_REFERENCE)]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.XREF_ERROR_WHILE_READING_TABLE_WILL_BE_REBUILT_WITH_CAUSE)]
        public virtual void CustomMemoryHandlerSingleTest() {
            MemoryLimitsAwareHandler handler = new MemoryLimitsAwareHandler();
            handler.SetMaxSizeOfSingleDecompressedPdfStream(1000);
            using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(SOURCE_FOLDER + "timing.pdf", new ReaderProperties
                ().SetMemoryLimitsAwareHandler(handler)), new PdfWriter(new MemoryStream()))) {
                PdfStream stream = pdfDocument.GetFirstPage().GetContentStream(0);
                byte[] b = stream.GetBytes(false);
                PdfArray array = new PdfArray();
                stream.Put(PdfName.Filter, array);
                NUnit.Framework.Assert.AreEqual(51, PdfReader.DecodeBytes(b, stream).Length);
                array.Add(PdfName.Fl);
                NUnit.Framework.Assert.AreEqual(40, PdfReader.DecodeBytes(b, stream).Length);
                array.Add(PdfName.Fl);
                NUnit.Framework.Assert.AreEqual(992, PdfReader.DecodeBytes(b, stream).Length);
                array.Add(PdfName.Fl);
                Exception e = NUnit.Framework.Assert.Catch(typeof(MemoryLimitsAwareException), () => PdfReader.DecodeBytes
                    (b, stream));
                NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.DURING_DECOMPRESSION_SINGLE_STREAM_OCCUPIED_MORE_MEMORY_THAN_ALLOWED
                    , e.Message);
            }
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.INVALID_INDIRECT_REFERENCE)]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.XREF_ERROR_WHILE_READING_TABLE_WILL_BE_REBUILT_WITH_CAUSE)]
        public virtual void OneFilterCustomMemoryHandlerSingleTest() {
            MemoryLimitsAwareHandler handler = new MemoryLimitsAwareHandler();
            handler.SetMaxSizeOfSingleDecompressedPdfStream(20);
            using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(SOURCE_FOLDER + "timing.pdf", new ReaderProperties
                ().SetMemoryLimitsAwareHandler(handler)), new PdfWriter(new MemoryStream()))) {
                PdfStream stream = pdfDocument.GetFirstPage().GetContentStream(0);
                byte[] b = stream.GetBytes(false);
                PdfArray array = new PdfArray();
                stream.Put(PdfName.Filter, array);
                // Limit is reached, but the stream has no filters. Therefore, we don't consider it to be suspicious.
                NUnit.Framework.Assert.AreEqual(51, PdfReader.DecodeBytes(b, stream).Length);
                // Limit is reached, but the stream has only one filter. Therefore, we don't consider it to be suspicious.
                array.Add(PdfName.Fl);
                NUnit.Framework.Assert.AreEqual(40, PdfReader.DecodeBytes(b, stream).Length);
            }
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.INVALID_INDIRECT_REFERENCE)]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.XREF_ERROR_WHILE_READING_TABLE_WILL_BE_REBUILT_WITH_CAUSE)]
        public virtual void OverriddenMemoryHandlerAllStreamsAreSuspiciousTest() {
            MemoryLimitsAwareHandler handler = new _MemoryLimitsAwareHandler_174();
            handler.SetMaxSizeOfSingleDecompressedPdfStream(20);
            using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(SOURCE_FOLDER + "timing.pdf", new ReaderProperties
                ().SetMemoryLimitsAwareHandler(handler)), new PdfWriter(new MemoryStream()))) {
                PdfStream stream = pdfDocument.GetFirstPage().GetContentStream(0);
                byte[] b = stream.GetBytes(false);
                PdfArray array = new PdfArray();
                stream.Put(PdfName.Filter, array);
                array.Add(PdfName.Fl);
                // Limit is reached, and the stream with one filter is considered to be suspicious.
                Exception e = NUnit.Framework.Assert.Catch(typeof(MemoryLimitsAwareException), () => PdfReader.DecodeBytes
                    (b, stream));
                NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.DURING_DECOMPRESSION_SINGLE_STREAM_OCCUPIED_MORE_MEMORY_THAN_ALLOWED
                    , e.Message);
            }
        }

        private sealed class _MemoryLimitsAwareHandler_174 : MemoryLimitsAwareHandler {
            public _MemoryLimitsAwareHandler_174() {
            }

            public override bool IsMemoryLimitsAwarenessRequiredOnDecompression(PdfArray filters) {
                return true;
            }
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.INVALID_INDIRECT_REFERENCE)]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.XREF_ERROR_WHILE_READING_TABLE_WILL_BE_REBUILT_WITH_CAUSE)]
        public virtual void OverriddenMemoryHandlerNoStreamsAreSuspiciousTest() {
            MemoryLimitsAwareHandler handler = new _MemoryLimitsAwareHandler_209();
            handler.SetMaxSizeOfSingleDecompressedPdfStream(20);
            using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(SOURCE_FOLDER + "timing.pdf", new ReaderProperties
                ().SetMemoryLimitsAwareHandler(handler)), new PdfWriter(new MemoryStream()))) {
                PdfStream stream = pdfDocument.GetFirstPage().GetContentStream(0);
                byte[] b = stream.GetBytes(false);
                PdfArray array = new PdfArray();
                stream.Put(PdfName.Filter, array);
                array.Add(PdfName.Fl);
                array.Add(PdfName.Fl);
                // Limit is reached but the stream with several copies of the filter is not considered to be suspicious.
                PdfReader.DecodeBytes(b, stream);
            }
        }

        private sealed class _MemoryLimitsAwareHandler_209 : MemoryLimitsAwareHandler {
            public _MemoryLimitsAwareHandler_209() {
            }

            public override bool IsMemoryLimitsAwarenessRequiredOnDecompression(PdfArray filters) {
                return false;
            }
        }

        [NUnit.Framework.Test]
        public virtual void DifferentFiltersEmptyTest() {
            byte[] b = new byte[1000];
            PdfArray array = new PdfArray();
            array.Add(PdfName.Fl);
            array.Add(PdfName.AHx);
            array.Add(PdfName.A85);
            array.Add(PdfName.RunLengthDecode);
            PdfStream stream = new PdfStream(b);
            stream.Put(PdfName.Filter, array);
            NUnit.Framework.Assert.AreEqual(0, PdfReader.DecodeBytes(b, stream).Length);
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.INVALID_INDIRECT_REFERENCE)]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.XREF_ERROR_WHILE_READING_TABLE_WILL_BE_REBUILT_WITH_CAUSE)]
        public virtual void CustomMemoryHandlerSumTest() {
            MemoryLimitsAwareHandler handler = new MemoryLimitsAwareHandler();
            handler.SetMaxSizeOfDecompressedPdfStreamsSum(100000);
            using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(SOURCE_FOLDER + "timing.pdf", new ReaderProperties
                ().SetMemoryLimitsAwareHandler(handler)), new PdfWriter(new MemoryStream()))) {
                PdfStream stream = pdfDocument.GetFirstPage().GetContentStream(0);
                byte[] b = stream.GetBytes(false);
                Exception e = NUnit.Framework.Assert.Catch(typeof(MemoryLimitsAwareException), () => PdfReader.DecodeBytes
                    (b, stream));
                NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.DURING_DECOMPRESSION_MULTIPLE_STREAMS_IN_SUM_OCCUPIED_MORE_MEMORY_THAN_ALLOWED
                    , e.Message);
            }
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.INVALID_INDIRECT_REFERENCE)]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.XREF_ERROR_WHILE_READING_TABLE_WILL_BE_REBUILT_WITH_CAUSE)]
        public virtual void PageSumTest() {
            MemoryLimitsAwareHandler handler = new MemoryLimitsAwareHandler();
            handler.SetMaxSizeOfDecompressedPdfStreamsSum(1500000);
            using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(SOURCE_FOLDER + "timing.pdf", new ReaderProperties
                ().SetMemoryLimitsAwareHandler(handler)), new PdfWriter(new MemoryStream()))) {
                Exception e = NUnit.Framework.Assert.Catch(typeof(MemoryLimitsAwareException), () => pdfDocument.GetFirstPage
                    ().GetContentBytes());
                NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.DURING_DECOMPRESSION_MULTIPLE_STREAMS_IN_SUM_OCCUPIED_MORE_MEMORY_THAN_ALLOWED
                    , e.Message);
            }
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.INVALID_INDIRECT_REFERENCE)]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.XREF_ERROR_WHILE_READING_TABLE_WILL_BE_REBUILT_WITH_CAUSE)]
        public virtual void PageAsSingleStreamTest() {
            MemoryLimitsAwareHandler handler = new MemoryLimitsAwareHandler();
            handler.SetMaxSizeOfSingleDecompressedPdfStream(1500000);
            using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(SOURCE_FOLDER + "timing.pdf", new ReaderProperties
                ().SetMemoryLimitsAwareHandler(handler)), new PdfWriter(new MemoryStream()))) {
                Exception e = NUnit.Framework.Assert.Catch(typeof(MemoryLimitsAwareException), () => pdfDocument.GetFirstPage
                    ().GetContentBytes());
                NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.DURING_DECOMPRESSION_SINGLE_STREAM_OCCUPIED_MORE_MEMORY_THAN_ALLOWED
                    , e.Message);
            }
        }
    }
}
