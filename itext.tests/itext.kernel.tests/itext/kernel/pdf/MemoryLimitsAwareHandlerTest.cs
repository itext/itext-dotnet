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
            NUnit.Framework.Assert.AreEqual(1024L * 1024L * 1024L * 3L, handler.GetMaxXObjectsSizePerPage());
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
            MemoryLimitsAwareHandler customHandler = new _MemoryLimitsAwareHandler_59();
            PdfArray filters = new PdfArray();
            filters.Add(PdfName.FlateDecode);
            NUnit.Framework.Assert.IsFalse(defaultHandler.IsMemoryLimitsAwarenessRequiredOnDecompression(filters));
            NUnit.Framework.Assert.IsTrue(customHandler.IsMemoryLimitsAwarenessRequiredOnDecompression(filters));
        }

        private sealed class _MemoryLimitsAwareHandler_59 : MemoryLimitsAwareHandler {
            public _MemoryLimitsAwareHandler_59() {
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
        public virtual void CustomMaxXObjectSizePerPageHandlerTest() {
            MemoryLimitsAwareHandler memoryLimitsAwareHandler = new MemoryLimitsAwareHandler();
            NUnit.Framework.Assert.AreEqual(1024L * 1024L * 1024L * 3L, memoryLimitsAwareHandler.GetMaxXObjectsSizePerPage
                ());
            memoryLimitsAwareHandler.SetMaxXObjectsSizePerPage(1024L);
            NUnit.Framework.Assert.AreEqual(1024L, memoryLimitsAwareHandler.GetMaxXObjectsSizePerPage());
        }

        [NUnit.Framework.Test]
        public virtual void MinSizeBasedXrefCapacityHandlerTest() {
            MemoryLimitsAwareHandler memoryLimitsAwareHandler = new MemoryLimitsAwareHandler(1024 * 1024);
            NUnit.Framework.Assert.AreEqual(500000, memoryLimitsAwareHandler.GetMaxNumberOfElementsInXrefStructure());
        }

        [NUnit.Framework.Test]
        public virtual void SizeBasedXrefCapacityHandlerTest() {
            MemoryLimitsAwareHandler memoryLimitsAwareHandler = new MemoryLimitsAwareHandler(1024 * 1024 * 80);
            NUnit.Framework.Assert.AreEqual(40000000, memoryLimitsAwareHandler.GetMaxNumberOfElementsInXrefStructure()
                );
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
