/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
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
using iText.Kernel.Exceptions;
using iText.Test;

namespace iText.Kernel.Pdf {
    [NUnit.Framework.Category("UnitTest")]
    public class MemoryLimitsAwareHandlerTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void DefaultMemoryHandler() {
            MemoryLimitsAwareHandler handler = new MemoryLimitsAwareHandler();
            NUnit.Framework.Assert.AreEqual(int.MaxValue / 100, handler.GetMaxSizeOfSingleDecompressedPdfStream());
            NUnit.Framework.Assert.AreEqual(int.MaxValue / 20, handler.GetMaxSizeOfDecompressedPdfStreamsSum());
            NUnit.Framework.Assert.AreEqual(50000000, handler.GetMaxNumberOfElementsInXrefStructure());
        }

        [NUnit.Framework.Test]
        public virtual void CustomMemoryHandler() {
            MemoryLimitsAwareHandler handler = new MemoryLimitsAwareHandler(1000000);
            NUnit.Framework.Assert.AreEqual(100000000, handler.GetMaxSizeOfSingleDecompressedPdfStream());
            NUnit.Framework.Assert.AreEqual(500000000, handler.GetMaxSizeOfDecompressedPdfStreamsSum());
        }

        [NUnit.Framework.Test]
        public virtual void OverridenMemoryHandler() {
            MemoryLimitsAwareHandler defaultHandler = new MemoryLimitsAwareHandler();
            MemoryLimitsAwareHandler customHandler = new _MemoryLimitsAwareHandler_78();
            PdfArray filters = new PdfArray();
            filters.Add(PdfName.FlateDecode);
            NUnit.Framework.Assert.IsFalse(defaultHandler.IsMemoryLimitsAwarenessRequiredOnDecompression(filters));
            NUnit.Framework.Assert.IsTrue(customHandler.IsMemoryLimitsAwarenessRequiredOnDecompression(filters));
        }

        private sealed class _MemoryLimitsAwareHandler_78 : MemoryLimitsAwareHandler {
            public _MemoryLimitsAwareHandler_78() {
            }

            public override bool IsMemoryLimitsAwarenessRequiredOnDecompression(PdfArray filters) {
                return true;
            }
        }

        [NUnit.Framework.Test]
        public virtual void DefaultSingleMemoryHandler() {
            MemoryLimitsAwareHandler handler = new MemoryLimitsAwareHandler();
            TestSingleStream(handler);
        }

        [NUnit.Framework.Test]
        public virtual void DefaultMultipleMemoryHandler() {
            MemoryLimitsAwareHandler handler = new MemoryLimitsAwareHandler();
            TestMultipleStreams(handler);
        }

        [NUnit.Framework.Test]
        public virtual void ConsiderBytesTest() {
            MemoryLimitsAwareHandler handler = new MemoryLimitsAwareHandler();
            long state1 = handler.GetAllMemoryUsedForDecompression();
            handler.ConsiderBytesOccupiedByDecompressedPdfStream(100);
            long state2 = handler.GetAllMemoryUsedForDecompression();
            NUnit.Framework.Assert.AreEqual(state1, state2);
            handler.BeginDecompressedPdfStreamProcessing();
            handler.ConsiderBytesOccupiedByDecompressedPdfStream(100);
            long state3 = handler.GetAllMemoryUsedForDecompression();
            NUnit.Framework.Assert.AreEqual(state1, state3);
            handler.ConsiderBytesOccupiedByDecompressedPdfStream(80);
            long state4 = handler.GetAllMemoryUsedForDecompression();
            NUnit.Framework.Assert.AreEqual(state1, state4);
            handler.EndDecompressedPdfStreamProcessing();
            long state5 = handler.GetAllMemoryUsedForDecompression();
            NUnit.Framework.Assert.AreEqual(state1 + 100, state5);
        }

        [NUnit.Framework.Test]
        public virtual void CustomXrefCapacityHandlerTest() {
            MemoryLimitsAwareHandler memoryLimitsAwareHandler = new MemoryLimitsAwareHandler();
            NUnit.Framework.Assert.AreEqual(50000000, memoryLimitsAwareHandler.GetMaxNumberOfElementsInXrefStructure()
                );
            memoryLimitsAwareHandler.SetMaxNumberOfElementsInXrefStructure(20);
            NUnit.Framework.Assert.AreEqual(20, memoryLimitsAwareHandler.GetMaxNumberOfElementsInXrefStructure());
        }

        [NUnit.Framework.Test]
        public virtual void CheckCapacityExceedsLimitTest() {
            MemoryLimitsAwareHandler memoryLimitsAwareHandler = new MemoryLimitsAwareHandler();
            // There we add 2 instead of 1 since xref structures used 1-based indexes, so we decrement the capacity
            // before check.
            int capacityExceededTheLimit = memoryLimitsAwareHandler.GetMaxNumberOfElementsInXrefStructure() + 2;
            Exception ex = NUnit.Framework.Assert.Catch(typeof(MemoryLimitsAwareException), () => memoryLimitsAwareHandler
                .CheckIfXrefStructureExceedsTheLimit(capacityExceededTheLimit));
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.XREF_STRUCTURE_SIZE_EXCEEDED_THE_LIMIT, ex.
                Message);
        }

        [NUnit.Framework.Test]
        public virtual void CheckCapacityTest() {
            MemoryLimitsAwareHandler memoryLimitsAwareHandler = new MemoryLimitsAwareHandler();
            int capacityToSet = 2;
            NUnit.Framework.Assert.DoesNotThrow(() => memoryLimitsAwareHandler.CheckIfXrefStructureExceedsTheLimit(capacityToSet
                ));
        }

        private static void TestSingleStream(MemoryLimitsAwareHandler handler) {
            String expectedExceptionMessage = KernelExceptionMessageConstant.DURING_DECOMPRESSION_SINGLE_STREAM_OCCUPIED_MORE_MEMORY_THAN_ALLOWED;
            int expectedFailureIndex = 10;
            String occuredExceptionMessage = null;
            int limit = handler.GetMaxSizeOfSingleDecompressedPdfStream();
            long step = limit / 10;
            int i = 0;
            try {
                handler.BeginDecompressedPdfStreamProcessing();
                for (i = 0; i < 11; i++) {
                    handler.ConsiderBytesOccupiedByDecompressedPdfStream(step * (1 + i));
                }
                handler.EndDecompressedPdfStreamProcessing();
            }
            catch (MemoryLimitsAwareException e) {
                occuredExceptionMessage = e.Message;
            }
            NUnit.Framework.Assert.AreEqual(expectedFailureIndex, i);
            NUnit.Framework.Assert.AreEqual(expectedExceptionMessage, occuredExceptionMessage);
        }

        private static void TestMultipleStreams(MemoryLimitsAwareHandler handler) {
            String expectedExceptionMessage = KernelExceptionMessageConstant.DURING_DECOMPRESSION_MULTIPLE_STREAMS_IN_SUM_OCCUPIED_MORE_MEMORY_THAN_ALLOWED;
            int expectedFailureIndex = 10;
            String occuredExceptionMessage = null;
            int i = 0;
            try {
                long limit = handler.GetMaxSizeOfDecompressedPdfStreamsSum();
                long step = limit / 10;
                for (i = 0; i < 11; i++) {
                    handler.BeginDecompressedPdfStreamProcessing();
                    handler.ConsiderBytesOccupiedByDecompressedPdfStream(step);
                    handler.EndDecompressedPdfStreamProcessing();
                }
            }
            catch (MemoryLimitsAwareException e) {
                occuredExceptionMessage = e.Message;
            }
            NUnit.Framework.Assert.AreEqual(expectedFailureIndex, i);
            NUnit.Framework.Assert.AreEqual(expectedExceptionMessage, occuredExceptionMessage);
        }
    }
}
