/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 Apryse Group NV
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
using iText.Kernel;
using iText.Test;

namespace iText.Kernel.Pdf {
    public class MemoryLimitsAwareHandlerTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void DefaultMemoryHandler() {
            MemoryLimitsAwareHandler handler = new MemoryLimitsAwareHandler();
            NUnit.Framework.Assert.AreEqual(int.MaxValue / 100, handler.GetMaxSizeOfSingleDecompressedPdfStream());
            NUnit.Framework.Assert.AreEqual(int.MaxValue / 20, handler.GetMaxSizeOfDecompressedPdfStreamsSum());
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
            MemoryLimitsAwareHandler customHandler = new _MemoryLimitsAwareHandler_75();
            PdfArray filters = new PdfArray();
            filters.Add(PdfName.FlateDecode);
            NUnit.Framework.Assert.IsFalse(defaultHandler.IsMemoryLimitsAwarenessRequiredOnDecompression(filters));
            NUnit.Framework.Assert.IsTrue(customHandler.IsMemoryLimitsAwarenessRequiredOnDecompression(filters));
        }

        private sealed class _MemoryLimitsAwareHandler_75 : MemoryLimitsAwareHandler {
            public _MemoryLimitsAwareHandler_75() {
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

        private static void TestSingleStream(MemoryLimitsAwareHandler handler) {
            String expectedExceptionMessage = PdfException.DuringDecompressionSingleStreamOccupiedMoreMemoryThanAllowed;
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
            String expectedExceptionMessage = PdfException.DuringDecompressionMultipleStreamsInSumOccupiedMoreMemoryThanAllowed;
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
