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
