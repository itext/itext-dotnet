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
    public class CompareToolResultUnitTest : ExtendedITextTest {
        // Android-Conversion-Skip-File (during Android conversion the class will be replaced by DeferredCompare)
        [NUnit.Framework.Test]
        public virtual void ErrorCountTest() {
            CompareToolResult result = new CompareToolResult(2);
            result.AddError(new ObjectPath(), "error1");
            NUnit.Framework.Assert.AreEqual(2, result.GetMessageLimit());
            NUnit.Framework.Assert.AreEqual(1, result.GetDifferences().Count);
            NUnit.Framework.Assert.AreEqual(1, result.GetErrorCount());
        }

        [NUnit.Framework.Test]
        public virtual void AddErrorTest() {
            PdfDocument document = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
            PdfIndirectReference ref1 = document.GetCatalog().GetDocument().CreateNextIndirectReference();
            PdfIndirectReference ref2 = document.GetCatalog().GetDocument().CreateNextIndirectReference();
            CompareToolResult result = new CompareToolResult(1);
            result.AddError(new ObjectPath(ref1, ref1), "error1");
            result.AddError(new ObjectPath(ref2, ref2), "error2");
            NUnit.Framework.Assert.AreEqual(1, result.GetErrorCount());
            NUnit.Framework.Assert.IsTrue(result.IsMessageLimitReached());
            NUnit.Framework.Assert.IsTrue(result.GetDifferences().Values.Contains("error1"));
            NUnit.Framework.Assert.IsFalse(result.GetDifferences().Values.Contains("error2"));
        }
    }
}
