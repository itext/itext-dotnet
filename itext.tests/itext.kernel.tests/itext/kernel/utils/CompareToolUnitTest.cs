/*
This file is part of the iText (R) project.
Copyright (c) 1998-2026 Apryse Group NV
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
using iText.IO.Source;
using iText.Kernel.Pdf;
using iText.Kernel.Utils.Objectpathitems;
using iText.Test;

namespace iText.Kernel.Utils {
    [NUnit.Framework.Category("UnitTest")]
    public class CompareToolUnitTest : ExtendedITextTest {
        // Android-Conversion-Skip-File (during Android conversion the class will be replaced by DeferredCompareTool)
        [NUnit.Framework.Test]
        public virtual void CompareStreamsSizeTest() {
            byte[] bytes1 = new byte[] { 0, 1 };
            byte[] bytes2 = new byte[] { 0, 1, 3 };
            CompareTool compareTool = new CompareTool();
            NUnit.Framework.Assert.IsFalse(compareTool.CompareStreams(new PdfStream(bytes1), new PdfStream(bytes2)));
        }

        [NUnit.Framework.Test]
        public virtual void CompareArraysNullTest() {
            int[] array1 = new int[] { 0, 1 };
            PdfArray pdfArray1 = new PdfArray(array1);
            CompareTool compareTool = new CompareTool();
            NUnit.Framework.Assert.IsFalse(compareTool.CompareArrays(null, pdfArray1));
        }

        [NUnit.Framework.Test]
        public virtual void CompareArraysSizeTest() {
            int[] array1 = new int[] { 0, 1 };
            PdfArray pdfArray1 = new PdfArray(array1);
            int[] array2 = new int[] { 0, 1, 3, 4 };
            PdfArray pdfArray2 = new PdfArray(array2);
            CompareTool compareTool = new CompareTool();
            NUnit.Framework.Assert.IsFalse(compareTool.CompareArrays(pdfArray1, pdfArray2));
        }

        [NUnit.Framework.Test]
        public virtual void CompareObjectsNullTest() {
            PdfDocument document = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
            PdfIndirectReference ref1 = document.GetCatalog().GetDocument().CreateNextIndirectReference();
            CompareTool compareTool = new CompareTool();
            NUnit.Framework.Assert.IsTrue(compareTool.CompareObjects(null, null, new ObjectPath(ref1, ref1), new CompareToolResult
                (3)));
        }

        [NUnit.Framework.Test]
        public virtual void CompareObjectsDirectTest() {
            PdfDocument document = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
            PdfIndirectReference ref1 = document.GetCatalog().GetPdfObject().GetIndirectReference();
            CompareToolResult result = new CompareToolResult(3);
            CompareTool compareTool = new CompareTool();
            NUnit.Framework.Assert.IsFalse(compareTool.CompareObjects(ref1, document.GetCatalog().GetPdfObject(), new 
                ObjectPath(ref1, ref1), result));
            NUnit.Framework.Assert.IsTrue(result.GetDifferences().Values.Contains("Expected direct object."));
        }

        [NUnit.Framework.Test]
        public virtual void CompareObjectsIndirectTest() {
            PdfDocument document = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
            PdfIndirectReference ref1 = document.GetCatalog().GetPdfObject().GetIndirectReference();
            CompareToolResult result = new CompareToolResult(3);
            CompareTool compareTool = new CompareTool();
            NUnit.Framework.Assert.IsFalse(compareTool.CompareObjects(document.GetCatalog().GetPdfObject(), ref1, new 
                ObjectPath(ref1, ref1), result));
            NUnit.Framework.Assert.IsTrue(result.GetDifferences().Values.Contains("Expected indirect object."));
        }

        [NUnit.Framework.Test]
        public virtual void CompareObjectsDifferentTypeTest() {
            PdfDocument document = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
            PdfIndirectReference ref1 = document.GetCatalog().GetPdfObject().GetIndirectReference();
            PdfStream stream = new PdfStream();
            CompareToolResult result = new CompareToolResult(3);
            CompareTool compareTool = new CompareTool();
            NUnit.Framework.Assert.IsFalse(compareTool.CompareObjects(ref1, stream, new ObjectPath(ref1, ref1), result
                ));
            NUnit.Framework.Assert.IsTrue(result.GetDifferences().Values.Contains("Types do not match. Expected: PdfStream. Found: PdfDictionary."
                ));
        }

        [NUnit.Framework.Test]
        public virtual void CompareObjectsPageTest() {
            PdfDocument document = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
            PdfIndirectReference ref1 = document.AddNewPage().GetPdfObject().GetIndirectReference();
            PdfIndirectReference ref2 = document.GetCatalog().GetPdfObject().GetIndirectReference();
            CompareToolResult result = new CompareToolResult(3);
            CompareTool compareTool = new CompareTool();
            NUnit.Framework.Assert.IsFalse(compareTool.CompareObjects(ref2, ref1, new ObjectPath(ref1, ref1), result));
            NUnit.Framework.Assert.IsTrue(result.GetDifferences().Values.Contains("Expected a page. Found not a page."
                ));
        }
    }
}
