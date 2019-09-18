using System;
using iText.Kernel.Font;
using iText.Test;

namespace iText.Kernel.Pdf {
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

        /// <exception cref="System.IO.IOException"/>
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
