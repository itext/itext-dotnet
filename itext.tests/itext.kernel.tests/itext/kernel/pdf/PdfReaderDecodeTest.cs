/*
This file is part of the iText (R) project.
Copyright (c) 1998-2020 iText Group NV
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
using iText.Kernel;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Kernel.Pdf {
    public class PdfReaderDecodeTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/pdf/PdfReaderDecodeTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/kernel/pdf/PdfReaderDecodeTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void NoMemoryHandlerTest() {
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new MemoryStream()));
            FileStream @is = new FileStream(sourceFolder + "stream", FileMode.Open, FileAccess.Read);
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
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.INVALID_INDIRECT_REFERENCE)]
        [LogMessage(iText.IO.LogMessageConstant.XREF_ERROR_WHILE_READING_TABLE_WILL_BE_REBUILT)]
        public virtual void DefaultMemoryHandlerTest() {
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(sourceFolder + "timing.pdf"), new PdfWriter(new MemoryStream
                ()));
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

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.INVALID_INDIRECT_REFERENCE)]
        [LogMessage(iText.IO.LogMessageConstant.XREF_ERROR_WHILE_READING_TABLE_WILL_BE_REBUILT)]
        public virtual void CustomMemoryHandlerSingleTest() {
            MemoryLimitsAwareHandler handler = new MemoryLimitsAwareHandler();
            handler.SetMaxSizeOfSingleDecompressedPdfStream(1000);
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(sourceFolder + "timing.pdf", new ReaderProperties(
                ).SetMemoryLimitsAwareHandler(handler)), new PdfWriter(new MemoryStream()));
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
            String expectedExceptionMessage = PdfException.DuringDecompressionSingleStreamOccupiedMoreMemoryThanAllowed;
            String thrownExceptionMessage = null;
            try {
                PdfReader.DecodeBytes(b, stream);
            }
            catch (MemoryLimitsAwareException e) {
                thrownExceptionMessage = e.Message;
            }
            NUnit.Framework.Assert.AreEqual(expectedExceptionMessage, thrownExceptionMessage);
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.INVALID_INDIRECT_REFERENCE)]
        [LogMessage(iText.IO.LogMessageConstant.XREF_ERROR_WHILE_READING_TABLE_WILL_BE_REBUILT)]
        public virtual void OneFilterCustomMemoryHandlerSingleTest() {
            MemoryLimitsAwareHandler handler = new MemoryLimitsAwareHandler();
            handler.SetMaxSizeOfSingleDecompressedPdfStream(20);
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(sourceFolder + "timing.pdf", new ReaderProperties(
                ).SetMemoryLimitsAwareHandler(handler)), new PdfWriter(new MemoryStream()));
            PdfStream stream = pdfDocument.GetFirstPage().GetContentStream(0);
            byte[] b = stream.GetBytes(false);
            PdfArray array = new PdfArray();
            stream.Put(PdfName.Filter, array);
            // Limit is reached, but the stream has no filters. Therefore we don't consider ot to be suspicious
            NUnit.Framework.Assert.AreEqual(51, PdfReader.DecodeBytes(b, stream).Length);
            // Limit is reached, but the stream has only one filter. Therefore we don't consider ot to be suspicious
            array.Add(PdfName.Fl);
            NUnit.Framework.Assert.AreEqual(40, PdfReader.DecodeBytes(b, stream).Length);
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
        [LogMessage(iText.IO.LogMessageConstant.INVALID_INDIRECT_REFERENCE)]
        [LogMessage(iText.IO.LogMessageConstant.XREF_ERROR_WHILE_READING_TABLE_WILL_BE_REBUILT)]
        public virtual void CustomMemoryHandlerSumTest() {
            MemoryLimitsAwareHandler handler = new MemoryLimitsAwareHandler();
            handler.SetMaxSizeOfDecompressedPdfStreamsSum(100000);
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(sourceFolder + "timing.pdf", new ReaderProperties(
                ).SetMemoryLimitsAwareHandler(handler)), new PdfWriter(new MemoryStream()));
            PdfStream stream = pdfDocument.GetFirstPage().GetContentStream(0);
            byte[] b = stream.GetBytes(false);
            String expectedExceptionMessage = PdfException.DuringDecompressionMultipleStreamsInSumOccupiedMoreMemoryThanAllowed;
            String thrownExceptionMessage = null;
            try {
                PdfReader.DecodeBytes(b, stream);
            }
            catch (MemoryLimitsAwareException e) {
                thrownExceptionMessage = e.Message;
            }
            NUnit.Framework.Assert.AreEqual(expectedExceptionMessage, thrownExceptionMessage);
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.INVALID_INDIRECT_REFERENCE)]
        [LogMessage(iText.IO.LogMessageConstant.XREF_ERROR_WHILE_READING_TABLE_WILL_BE_REBUILT)]
        public virtual void PageSumTest() {
            MemoryLimitsAwareHandler handler = new MemoryLimitsAwareHandler();
            handler.SetMaxSizeOfDecompressedPdfStreamsSum(1500000);
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(sourceFolder + "timing.pdf", new ReaderProperties(
                ).SetMemoryLimitsAwareHandler(handler)), new PdfWriter(new MemoryStream()));
            String expectedExceptionMessage = PdfException.DuringDecompressionMultipleStreamsInSumOccupiedMoreMemoryThanAllowed;
            String thrownExceptionMessage = null;
            try {
                pdfDocument.GetFirstPage().GetContentBytes();
            }
            catch (MemoryLimitsAwareException e) {
                thrownExceptionMessage = e.Message;
            }
            NUnit.Framework.Assert.AreEqual(expectedExceptionMessage, thrownExceptionMessage);
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.INVALID_INDIRECT_REFERENCE)]
        [LogMessage(iText.IO.LogMessageConstant.XREF_ERROR_WHILE_READING_TABLE_WILL_BE_REBUILT)]
        public virtual void PageAsSingleStreamTest() {
            MemoryLimitsAwareHandler handler = new MemoryLimitsAwareHandler();
            handler.SetMaxSizeOfSingleDecompressedPdfStream(1500000);
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(sourceFolder + "timing.pdf", new ReaderProperties(
                ).SetMemoryLimitsAwareHandler(handler)), new PdfWriter(new MemoryStream()));
            String expectedExceptionMessage = PdfException.DuringDecompressionSingleStreamOccupiedMoreMemoryThanAllowed;
            String thrownExceptionMessage = null;
            try {
                pdfDocument.GetFirstPage().GetContentBytes();
            }
            catch (MemoryLimitsAwareException e) {
                thrownExceptionMessage = e.Message;
            }
            NUnit.Framework.Assert.AreEqual(expectedExceptionMessage, thrownExceptionMessage);
        }
    }
}
