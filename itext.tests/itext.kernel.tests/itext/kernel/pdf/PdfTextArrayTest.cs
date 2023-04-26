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

        [NUnit.Framework.Test]
        public virtual void AddZeroSumTest() {
            PdfTextArray textArray = new PdfTextArray();
            textArray.Add(new PdfString("test"));
            textArray.Add(new PdfNumber(11));
            textArray.Add(new PdfNumber(12));
            textArray.Add(new PdfNumber(-13));
            textArray.Add(new PdfNumber(8));
            textArray.Add(new PdfNumber(-18));
            textArray.Add(new PdfString("test"));
            PdfArray expected = new PdfArray();
            expected.Add(new PdfString("test"));
            expected.Add(new PdfString("test"));
            NUnit.Framework.Assert.AreEqual(expected.list, textArray.list);
        }

        [NUnit.Framework.Test]
        public virtual void AddZeroSumAtTheBeginningTest() {
            PdfTextArray textArray = new PdfTextArray();
            textArray.Add(new PdfNumber(11));
            textArray.Add(new PdfNumber(-11));
            textArray.Add(new PdfNumber(13));
            textArray.Add(new PdfString("test"));
            PdfArray expected = new PdfArray();
            expected.Add(new PdfNumber(13));
            expected.Add(new PdfString("test"));
            NUnit.Framework.Assert.AreEqual(expected.list, textArray.list);
        }
    }
}
