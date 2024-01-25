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
using iText.IO.Source;
using iText.Kernel.Exceptions;
using iText.Test;

namespace iText.Kernel.Pdf {
    [NUnit.Framework.Category("UnitTest")]
    public class PdfXrefTableUnitTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void CheckNumberOfIndirectObjectsTest() {
            PdfXrefTable table = new PdfXrefTable();
            NUnit.Framework.Assert.AreEqual(0, table.GetCountOfIndirectObjects());
            int numberOfReferences = 10;
            for (int i = 0; i < numberOfReferences; i++) {
                table.Add(new PdfIndirectReference(null, i + 1));
            }
            NUnit.Framework.Assert.AreEqual(numberOfReferences, table.GetCountOfIndirectObjects());
        }

        [NUnit.Framework.Test]
        public virtual void CheckNumberOfIndirectObjectsWithFreeReferencesTest() {
            PdfXrefTable table = new PdfXrefTable();
            int numberOfReferences = 10;
            for (int i = 0; i < numberOfReferences; i++) {
                table.Add(new PdfIndirectReference(null, i + 1));
            }
            table.InitFreeReferencesList(null);
            int freeReferenceNumber = 5;
            table.FreeReference(table.Get(freeReferenceNumber));
            NUnit.Framework.Assert.AreEqual(numberOfReferences - 1, table.GetCountOfIndirectObjects());
            NUnit.Framework.Assert.IsTrue(table.Get(freeReferenceNumber).IsFree());
        }

        [NUnit.Framework.Test]
        public virtual void CheckNumberOfIndirectObjectsWithRandomNumbersTest() {
            PdfXrefTable table = new PdfXrefTable();
            int numberOfReferences = 10;
            for (int i = 0; i < numberOfReferences; i++) {
                table.Add(new PdfIndirectReference(null, i * 25));
            }
            NUnit.Framework.Assert.AreEqual(numberOfReferences, table.GetCountOfIndirectObjects());
            NUnit.Framework.Assert.AreEqual(226, table.Size());
        }

        [NUnit.Framework.Test]
        public virtual void CheckExceedTheNumberOfElementsInXrefTest() {
            MemoryLimitsAwareHandler memoryLimitsAwareHandler = new MemoryLimitsAwareHandler();
            memoryLimitsAwareHandler.SetMaxNumberOfElementsInXrefStructure(5);
            PdfXrefTable xrefTable = new PdfXrefTable(5, memoryLimitsAwareHandler);
            int numberOfReferences = 5;
            for (int i = 1; i < numberOfReferences; i++) {
                xrefTable.Add(new PdfIndirectReference(null, i));
            }
            Exception exception = NUnit.Framework.Assert.Catch(typeof(MemoryLimitsAwareException), () => xrefTable.Add
                (new PdfIndirectReference(null, numberOfReferences)));
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.XREF_STRUCTURE_SIZE_EXCEEDED_THE_LIMIT, exception
                .Message);
        }

        [NUnit.Framework.Test]
        public virtual void EnsureCapacityExceedTheLimitTest() {
            MemoryLimitsAwareHandler memoryLimitsAwareHandler = new MemoryLimitsAwareHandler();
            PdfXrefTable xrefTable = new PdfXrefTable(memoryLimitsAwareHandler);
            int newCapacityExceededTheLimit = memoryLimitsAwareHandler.GetMaxNumberOfElementsInXrefStructure() + 2;
            // There we add 2 instead of 1 since xref structures used 1-based indexes, so we decrement the capacity
            // before check.
            Exception ex = NUnit.Framework.Assert.Catch(typeof(MemoryLimitsAwareException), () => xrefTable.SetCapacity
                (newCapacityExceededTheLimit));
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.XREF_STRUCTURE_SIZE_EXCEEDED_THE_LIMIT, ex.
                Message);
        }

        [NUnit.Framework.Test]
        public virtual void PassCapacityGreaterThanLimitInConstructorTest() {
            MemoryLimitsAwareHandler memoryLimitsAwareHandler = new MemoryLimitsAwareHandler();
            memoryLimitsAwareHandler.SetMaxNumberOfElementsInXrefStructure(20);
            Exception ex = NUnit.Framework.Assert.Catch(typeof(MemoryLimitsAwareException), () => new PdfXrefTable(30, 
                memoryLimitsAwareHandler));
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.XREF_STRUCTURE_SIZE_EXCEEDED_THE_LIMIT, ex.
                Message);
        }

        [NUnit.Framework.Test]
        public virtual void ZeroCapacityInConstructorWithHandlerTest() {
            MemoryLimitsAwareHandler memoryLimitsAwareHandler = new MemoryLimitsAwareHandler();
            memoryLimitsAwareHandler.SetMaxNumberOfElementsInXrefStructure(20);
            PdfXrefTable xrefTable = new PdfXrefTable(0, memoryLimitsAwareHandler);
            NUnit.Framework.Assert.AreEqual(20, xrefTable.GetCapacity());
        }

        [NUnit.Framework.Test]
        public virtual void XRefMaxValueLong() {
            PdfDocument document = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
            document.xref.Add(new PdfIndirectReferenceProxy(document, 11, long.MaxValue));
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => {
                document.Close();
            }
            );
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.XREF_HAS_AN_ENTRY_WITH_TOO_BIG_OFFSET, e.Message
                );
        }

        [NUnit.Framework.Test]
        public virtual void MaxCrossReferenceOffSetReached() {
            long justOver10gbLogical = 10_000_000_001L;
            PdfDocument document = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
            document.xref.Add(new PdfIndirectReferenceProxy(document, 11, justOver10gbLogical));
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => {
                document.Close();
            }
            );
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.XREF_HAS_AN_ENTRY_WITH_TOO_BIG_OFFSET, e.Message
                );
        }

        [NUnit.Framework.Test]
        public virtual void MaxCrossReference() {
            long justOver10gbLogical = 10_000_000_000L;
            PdfDocument document = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
            document.xref.Add(new PdfIndirectReferenceProxy(document, 11, justOver10gbLogical));
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => {
                document.Close();
            }
            );
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.XREF_HAS_AN_ENTRY_WITH_TOO_BIG_OFFSET, e.Message
                );
        }

        [NUnit.Framework.Test]
        public virtual void JustBelowXrefThreshold() {
            long maxAllowedOffset = 10_000_000_000L - 1L;
            PdfDocument document = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
            document.xref.Add(new PdfIndirectReferenceProxy(document, 11, maxAllowedOffset));
            NUnit.Framework.Assert.DoesNotThrow(() => document.Close());
        }

        [NUnit.Framework.Test]
        public virtual void XRefIntMax() {
            PdfDocument document = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
            document.xref.Add(new PdfIndirectReferenceProxy(document, 11, int.MaxValue));
            NUnit.Framework.Assert.DoesNotThrow(() => document.Close());
        }
    }

    internal class PdfIndirectReferenceProxy : PdfIndirectReference {
        private readonly long offset;

        public PdfIndirectReferenceProxy(PdfDocument document, int objNumber, long offset)
            : base(document, objNumber) {
            this.offset = offset;
        }

        public override long GetOffset() {
            return offset;
        }
    }
}
