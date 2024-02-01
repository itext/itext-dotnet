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
using System.IO;
using iText.Kernel.Pdf;
using iText.Test;

namespace iText.Kernel.Pdf.Layer {
    [NUnit.Framework.Category("UnitTest")]
    public class PdfVisibilityExpressionTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void ExpressionByArrayTest() {
            PdfDocument tempDoc = new PdfDocument(new PdfWriter(new MemoryStream()));
            PdfArray array = new PdfArray();
            // add the AND operator as the first parameter of the expression
            array.Add(PdfName.And);
            // add two empty dictionaries as the other parameters
            array.Add(new PdfLayer((PdfDictionary)new PdfDictionary().MakeIndirect(tempDoc)).GetPdfObject());
            array.Add(new PdfLayer((PdfDictionary)new PdfDictionary().MakeIndirect(tempDoc)).GetPdfObject());
            // create visibility expression
            PdfVisibilityExpression expression = new PdfVisibilityExpression(array);
            PdfObject expressionObject = expression.GetPdfObject();
            NUnit.Framework.Assert.IsTrue(expressionObject is PdfArray);
            NUnit.Framework.Assert.AreEqual(3, ((PdfArray)expressionObject).Size());
            NUnit.Framework.Assert.AreEqual(PdfName.And, ((PdfArray)expressionObject).GetAsName(0));
        }

        [NUnit.Framework.Test]
        public virtual void AndExpressionTest() {
            PdfDocument tempDoc = new PdfDocument(new PdfWriter(new MemoryStream()));
            // create expression with the AND operator as the first parameter
            PdfVisibilityExpression expression = new PdfVisibilityExpression(PdfName.And);
            // add two empty dictionaries as the other parameters
            expression.AddOperand(new PdfLayer((PdfDictionary)new PdfDictionary().MakeIndirect(tempDoc)));
            expression.AddOperand(new PdfLayer((PdfDictionary)new PdfDictionary().MakeIndirect(tempDoc)));
            PdfObject expressionObject = expression.GetPdfObject();
            NUnit.Framework.Assert.IsTrue(expressionObject is PdfArray);
            NUnit.Framework.Assert.AreEqual(3, ((PdfArray)expressionObject).Size());
            NUnit.Framework.Assert.AreEqual(PdfName.And, ((PdfArray)expressionObject).GetAsName(0));
        }

        [NUnit.Framework.Test]
        public virtual void NestedExpressionTest() {
            PdfDocument tempDoc = new PdfDocument(new PdfWriter(new MemoryStream()));
            // create expression with the OR operator as the first parameter
            PdfVisibilityExpression expression = new PdfVisibilityExpression(PdfName.Or);
            // add an empty dictionary as the second parameter
            expression.AddOperand(new PdfLayer((PdfDictionary)new PdfDictionary().MakeIndirect(tempDoc)));
            // create a nested expression with the AND operator and two empty dictionaries as parameters
            PdfVisibilityExpression nestedExpression = new PdfVisibilityExpression(PdfName.And);
            nestedExpression.AddOperand(new PdfLayer((PdfDictionary)new PdfDictionary().MakeIndirect(tempDoc)));
            nestedExpression.AddOperand(new PdfLayer((PdfDictionary)new PdfDictionary().MakeIndirect(tempDoc)));
            // add another expression as the third parameter
            expression.AddOperand(nestedExpression);
            PdfObject expressionObject = expression.GetPdfObject();
            NUnit.Framework.Assert.IsTrue(expressionObject is PdfArray);
            NUnit.Framework.Assert.AreEqual(3, ((PdfArray)expressionObject).Size());
            NUnit.Framework.Assert.AreEqual(PdfName.Or, ((PdfArray)expressionObject).GetAsName(0));
            PdfObject child = ((PdfArray)expressionObject).Get(2);
            NUnit.Framework.Assert.IsTrue(child is PdfArray);
            NUnit.Framework.Assert.AreEqual(3, ((PdfArray)child).Size());
            NUnit.Framework.Assert.AreEqual(PdfName.And, ((PdfArray)child).Get(0));
        }
    }
}
