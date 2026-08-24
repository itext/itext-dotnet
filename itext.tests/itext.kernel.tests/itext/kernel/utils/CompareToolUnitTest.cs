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
using iText.Commons.Internal.Runtime;
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

        [NUnit.Framework.Test]
        public virtual void EqualsWithFloatToleranceTest() {
            NUnit.Framework.Assert.IsTrue(CompareTool.EqualsWithFloatTolerance(new byte[0], new byte[0], 0.1f));
            byte[] a = "asd 143.6 asd".GetBytes(iText.Commons.Utils.EncodingUtil.ISO_8859_1);
            NUnit.Framework.Assert.IsTrue(CompareTool.EqualsWithFloatTolerance(a, a, 0.02f));
            a = "asd 143.6 asd".GetBytes(iText.Commons.Utils.EncodingUtil.ISO_8859_1);
            byte[] b = "asd 143.59 asd".GetBytes(iText.Commons.Utils.EncodingUtil.ISO_8859_1);
            NUnit.Framework.Assert.IsTrue(CompareTool.EqualsWithFloatTolerance(a, b, 0.02f));
            a = "val -10.0 end".GetBytes(iText.Commons.Utils.EncodingUtil.ISO_8859_1);
            b = "val -10.1 end".GetBytes(iText.Commons.Utils.EncodingUtil.ISO_8859_1);
            NUnit.Framework.Assert.IsTrue(CompareTool.EqualsWithFloatTolerance(a, b, 0.11f));
            a = "1.0 Tf 100 200.0 Td".GetBytes(iText.Commons.Utils.EncodingUtil.ISO_8859_1);
            b = "1.0 Tf 100.05 199.95 Td".GetBytes(iText.Commons.Utils.EncodingUtil.ISO_8859_1);
            NUnit.Framework.Assert.IsTrue(CompareTool.EqualsWithFloatTolerance(a, b, 0.1f));
            a = "1.0 Tf 100 200.0 Td".GetBytes(iText.Commons.Utils.EncodingUtil.ISO_8859_1);
            b = "1.0 Tf 100.05 200.11 Td".GetBytes(iText.Commons.Utils.EncodingUtil.ISO_8859_1);
            NUnit.Framework.Assert.IsFalse(CompareTool.EqualsWithFloatTolerance(a, b, 0.1f));
            a = "1.0 2.0 3.0".GetBytes(iText.Commons.Utils.EncodingUtil.ISO_8859_1);
            b = "1.0 2.0".GetBytes(iText.Commons.Utils.EncodingUtil.ISO_8859_1);
            NUnit.Framework.Assert.IsFalse(CompareTool.EqualsWithFloatTolerance(a, b, 1.0f));
            a = "pos -5 end".GetBytes(iText.Commons.Utils.EncodingUtil.ISO_8859_1);
            b = "pos -5.02 end differ".GetBytes(iText.Commons.Utils.EncodingUtil.ISO_8859_1);
            NUnit.Framework.Assert.IsFalse(CompareTool.EqualsWithFloatTolerance(a, b, 0.1f));
            a = "pos -5.000001 end".GetBytes(iText.Commons.Utils.EncodingUtil.ISO_8859_1);
            b = "pos -5.000002 end".GetBytes(iText.Commons.Utils.EncodingUtil.ISO_8859_1);
            NUnit.Framework.Assert.IsFalse(CompareTool.EqualsWithFloatTolerance(a, b, 0.1f));
            a = "asd 1.5 asd".GetBytes(iText.Commons.Utils.EncodingUtil.ISO_8859_1);
            b = "asd 1.50001 asd".GetBytes(iText.Commons.Utils.EncodingUtil.ISO_8859_1);
            NUnit.Framework.Assert.IsFalse(CompareTool.EqualsWithFloatTolerance(a, b, 0f));
            a = "BT ET q Q".GetBytes(iText.Commons.Utils.EncodingUtil.ISO_8859_1);
            b = "BT ET Q q".GetBytes(iText.Commons.Utils.EncodingUtil.ISO_8859_1);
            NUnit.Framework.Assert.IsFalse(CompareTool.EqualsWithFloatTolerance(a, b, 0.5f));
            // A 9-digit integer exceeds the 8-digit cap and must be compared as raw text,
            // so even a "small" difference causes the arrays to be considered different.
            a = "id 123456789 end".GetBytes(iText.Commons.Utils.EncodingUtil.ISO_8859_1);
            b = "id 123456788 end".GetBytes(iText.Commons.Utils.EncodingUtil.ISO_8859_1);
            NUnit.Framework.Assert.IsFalse(CompareTool.EqualsWithFloatTolerance(a, b, 10f));
        }
    }
}
