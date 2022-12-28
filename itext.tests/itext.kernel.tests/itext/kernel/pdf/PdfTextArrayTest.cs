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
using iText.Kernel.Font;
using iText.Test;

namespace iText.Kernel.Pdf {
    [NUnit.Framework.Category("UnitTest")]
    public class PdfTextArrayTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void AddNZeroTest() {
            PdfTextArray textArray = new PdfTextArray();
            NUnit.Framework.Assert.IsFalse(textArray.Add(0.0f));
            NUnit.Framework.Assert.IsTrue(textArray.IsEmpty());
        }

        [NUnit.Framework.Test]
        public virtual void AdditiveInverseTest() {
            PdfTextArray textArray = new PdfTextArray();
            float number = 10;
            textArray.Add(number);
            textArray.Add(number * -1);
            NUnit.Framework.Assert.IsTrue(textArray.IsEmpty());
        }

        [NUnit.Framework.Test]
        public virtual void AddEmptyStringTest() {
            PdfTextArray textArray = new PdfTextArray();
            NUnit.Framework.Assert.IsFalse(textArray.Add(""));
            NUnit.Framework.Assert.IsTrue(textArray.IsEmpty());
        }

        [NUnit.Framework.Test]
        public virtual void AddNewStringTest() {
            PdfTextArray textArray = new PdfTextArray();
            String content = "content";
            NUnit.Framework.Assert.IsTrue(textArray.Add(content));
            NUnit.Framework.Assert.AreEqual(new PdfString(content), textArray.Get(0));
        }

        [NUnit.Framework.Test]
        public virtual void AppendStringTest() {
            PdfTextArray textArray = new PdfTextArray();
            String[] stringArray = new String[] { "one", "element" };
            foreach (String @string in stringArray) {
                textArray.Add(@string);
            }
            PdfString expected = new PdfString(stringArray[0] + stringArray[1]);
            NUnit.Framework.Assert.AreEqual(expected, textArray.Get(0));
        }

        [NUnit.Framework.Test]
        public virtual void AddStringWithFontTest() {
            PdfTextArray textArray = new PdfTextArray();
            String @string = "font";
            textArray.Add(@string, PdfFontFactory.CreateFont());
            NUnit.Framework.Assert.AreEqual(new PdfString(@string), textArray.Get(0));
        }

        [NUnit.Framework.Test]
        public virtual void AddAllNullTest() {
            PdfTextArray textArray = new PdfTextArray();
            textArray.AddAll((PdfArray)null);
            NUnit.Framework.Assert.IsTrue(textArray.IsEmpty());
        }

        [NUnit.Framework.Test]
        public virtual void AddNumbersTest() {
            PdfTextArray textArray = new PdfTextArray();
            float a = 5f;
            float b = 10f;
            textArray.Add(new PdfNumber(a));
            textArray.Add(new PdfNumber(b));
            NUnit.Framework.Assert.AreEqual(a + b, textArray.GetAsNumber(0).FloatValue(), 0.0001);
        }

        [NUnit.Framework.Test]
        public virtual void AddCollectionTest() {
            PdfArray collection = new PdfArray();
            collection.Add(new PdfString("str"));
            collection.Add(new PdfNumber(11));
            PdfTextArray textArray = new PdfTextArray();
            textArray.AddAll(collection);
            NUnit.Framework.Assert.AreEqual(collection.list, textArray.list);
        }
    }
}
