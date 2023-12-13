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
using iText.Test;

namespace iText.Kernel.Pdf {
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
    }
}
