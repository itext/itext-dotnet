using System;
using System.Text;
using iText.IO.Source;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Kernel.Pdf {
    public class PdfPrimitivesTest : ExtendedITextTest {
        internal static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/kernel/pdf/PdfPrimitivesTest/";

        internal static readonly PdfName TestArray = new PdfName("TestArray");

        internal const int DefaultArraySize = 64;

        internal const int PageCount = 1000;

        public class RandomString {
            private static readonly char[] symbols;

            private readonly Random random = new Random();

            private readonly char[] buf;

            static RandomString() {
                StringBuilder tmp = new StringBuilder();
                for (char ch = 'A'; ch <= 'Z'; ++ch) {
                    tmp.Append(ch);
                }
                for (char ch_1 = 'a'; ch_1 <= 'z'; ++ch_1) {
                    tmp.Append(ch_1);
                }
                for (char ch_2 = '0'; ch_2 <= '9'; ++ch_2) {
                    tmp.Append(ch_2);
                }
                symbols = tmp.ToString().ToCharArray();
            }

            public RandomString(int length) {
                if (length < 1) {
                    throw new ArgumentException("length < 1: " + length);
                }
                buf = new char[length];
            }

            public virtual String NextString() {
                for (int idx = 0; idx < buf.Length; ++idx) {
                    buf[idx] = symbols[random.Next(symbols.Length)];
                }
                return new String(buf);
            }
        }

        [NUnit.Framework.SetUp]
        public virtual void Setup() {
            CreateDestinationFolder(destinationFolder);
        }

        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void PrimitivesFloatNumberTest() {
            String filename = "primitivesFloatNumberTest.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(destinationFolder + filename));
            for (int i = 0; i < PageCount; i++) {
                PdfPage page = pdfDoc.AddNewPage();
                PdfArray array = GeneratePdfArrayWithFloatNumbers(null, false);
                page.GetPdfObject().Put(TestArray, array);
                array.Flush();
                page.Flush();
            }
            pdfDoc.Close();
        }

        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void PrimitivesIntNumberTest() {
            String filename = "primitivesIntNumberTest.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(destinationFolder + filename));
            for (int i = 0; i < PageCount; i++) {
                PdfPage page = pdfDoc.AddNewPage();
                PdfArray array = GeneratePdfArrayWithIntNumbers(null, false);
                page.GetPdfObject().Put(TestArray, array);
                array.Flush();
                page.Flush();
            }
            pdfDoc.Close();
        }

        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void PrimitivesNameTest() {
            String filename = "primitivesNameTest.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(destinationFolder + filename));
            for (int i = 0; i < PageCount; i++) {
                PdfPage page = pdfDoc.AddNewPage();
                PdfArray array = GeneratePdfArrayWithNames(null, false);
                page.GetPdfObject().Put(TestArray, array);
                array.Flush();
                page.Flush();
            }
            pdfDoc.Close();
        }

        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void PrimitivesStringTest() {
            String filename = "primitivesStringTest.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(destinationFolder + filename));
            for (int i = 0; i < PageCount; i++) {
                PdfPage page = pdfDoc.AddNewPage();
                PdfArray array = GeneratePdfArrayWithStrings(null, false);
                page.GetPdfObject().Put(TestArray, array);
                array.Flush();
                page.Flush();
            }
            pdfDoc.Close();
        }

        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void PrimitivesBooleanTest() {
            String filename = "primitivesBooleanTest.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(destinationFolder + filename));
            for (int i = 0; i < PageCount; i++) {
                PdfPage page = pdfDoc.AddNewPage();
                page.GetPdfObject().Put(TestArray, GeneratePdfArrayWithBooleans(null, false));
                page.Flush();
            }
            pdfDoc.Close();
        }

        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void PrimitivesFloatNumberIndirectTest() {
            String filename = "primitivesFloatNumberIndirectTest.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(destinationFolder + filename));
            for (int i = 0; i < PageCount; i++) {
                PdfPage page = pdfDoc.AddNewPage();
                page.GetPdfObject().Put(TestArray, GeneratePdfArrayWithFloatNumbers(pdfDoc, true));
                page.Flush();
            }
            pdfDoc.Close();
        }

        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void PrimitivesIntNumberIndirectTest() {
            String filename = "primitivesIntNumberIndirectTest.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(destinationFolder + filename));
            for (int i = 0; i < PageCount; i++) {
                PdfPage page = pdfDoc.AddNewPage();
                page.GetPdfObject().Put(TestArray, GeneratePdfArrayWithIntNumbers(pdfDoc, true));
                page.Flush();
            }
            pdfDoc.Close();
        }

        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void PrimitivesStringIndirectTest() {
            String filename = "primitivesStringIndirectTest.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(destinationFolder + filename));
            for (int i = 0; i < PageCount; i++) {
                PdfPage page = pdfDoc.AddNewPage();
                page.GetPdfObject().Put(TestArray, GeneratePdfArrayWithStrings(pdfDoc, true));
                page.Flush();
            }
            pdfDoc.Close();
        }

        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void PrimitivesNameIndirectTest() {
            String filename = "primitivesNameIndirectTest.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(destinationFolder + filename));
            for (int i = 0; i < PageCount; i++) {
                PdfPage page = pdfDoc.AddNewPage();
                page.GetPdfObject().Put(TestArray, GeneratePdfArrayWithNames(pdfDoc, true));
                page.Flush();
            }
            pdfDoc.Close();
        }

        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void PrimitivesBooleanIndirectTest() {
            String filename = "primitivesBooleanIndirectTest.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(destinationFolder + filename));
            for (int i = 0; i < PageCount; i++) {
                PdfPage page = pdfDoc.AddNewPage();
                page.GetPdfObject().Put(TestArray, GeneratePdfArrayWithBooleans(pdfDoc, true));
                page.Flush();
            }
            pdfDoc.Close();
        }

        [NUnit.Framework.Test]
        public virtual void PdfNamesTest() {
            PdfPrimitivesTest.RandomString rnd = new PdfPrimitivesTest.RandomString(16);
            for (int i = 0; i < 10000000; i++) {
                new PdfName(rnd.NextString());
            }
        }

        [NUnit.Framework.Test]
        public virtual void EqualStrings() {
            PdfString a = ((PdfString)new PdfString("abcd").MakeIndirect(new PdfDocument(new PdfWriter(new ByteArrayOutputStream
                ()))));
            PdfString b = new PdfString("abcd".GetBytes(Encoding.ASCII));
            NUnit.Framework.Assert.IsTrue(a.Equals(b));
            PdfString c = new PdfString("abcd", "UTF-8");
            NUnit.Framework.Assert.IsFalse(c.Equals(a));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.CALCULATE_HASHCODE_FOR_MODIFIED_PDFNUMBER)]
        public virtual void EqualNumbers() {
            PdfNumber num1 = ((PdfNumber)new PdfNumber(1).MakeIndirect(new PdfDocument(new PdfWriter(new ByteArrayOutputStream
                ()))));
            PdfNumber num2 = new PdfNumber(2);
            NUnit.Framework.Assert.IsFalse(num1.Equals(num2));
            int hashCode = num1.GetHashCode();
            num1.Increment();
            NUnit.Framework.Assert.IsTrue(num1.Equals(num2));
            NUnit.Framework.Assert.AreNotEqual(hashCode, num1.GetHashCode());
        }

        [NUnit.Framework.Test]
        public virtual void EqualNames() {
            PdfName a = ((PdfName)new PdfName("abcd").MakeIndirect(new PdfDocument(new PdfWriter(new ByteArrayOutputStream
                ()))));
            PdfName b = new PdfName("abcd");
            NUnit.Framework.Assert.IsTrue(a.Equals(b));
        }

        [NUnit.Framework.Test]
        public virtual void EqualBoolean() {
            PdfBoolean f = ((PdfBoolean)new PdfBoolean(false).MakeIndirect(new PdfDocument(new PdfWriter(new ByteArrayOutputStream
                ()))));
            PdfBoolean t = new PdfBoolean(true);
            NUnit.Framework.Assert.IsFalse(f.Equals(t));
            NUnit.Framework.Assert.IsTrue(f.Equals(PdfBoolean.FALSE));
            NUnit.Framework.Assert.IsTrue(t.Equals(PdfBoolean.TRUE));
        }

        [NUnit.Framework.Test]
        public virtual void EqualNulls() {
            PdfNull a = ((PdfNull)new PdfNull().MakeIndirect(new PdfDocument(new PdfWriter(new ByteArrayOutputStream()
                ))));
            NUnit.Framework.Assert.IsTrue(a.Equals(PdfNull.PDF_NULL));
        }

        [NUnit.Framework.Test]
        public virtual void EqualLiterals() {
            PdfLiteral a = new PdfLiteral("abcd");
            PdfLiteral b = new PdfLiteral("abcd".GetBytes(Encoding.ASCII));
            NUnit.Framework.Assert.IsTrue(a.Equals(b));
        }

        private PdfArray GeneratePdfArrayWithFloatNumbers(PdfDocument doc, bool indirects) {
            PdfArray array = ((PdfArray)new PdfArray().MakeIndirect(doc));
            Random rnd = new Random();
            for (int i = 0; i < DefaultArraySize; i++) {
                PdfNumber num = new PdfNumber(rnd.NextFloat());
                if (indirects) {
                    num.MakeIndirect(doc);
                }
                array.Add(num);
            }
            return array;
        }

        private PdfArray GeneratePdfArrayWithIntNumbers(PdfDocument doc, bool indirects) {
            PdfArray array = ((PdfArray)new PdfArray().MakeIndirect(doc));
            Random rnd = new Random();
            for (int i = 0; i < DefaultArraySize; i++) {
                array.Add(((PdfNumber)new PdfNumber(rnd.Next()).MakeIndirect(indirects ? doc : null)));
            }
            return array;
        }

        private PdfArray GeneratePdfArrayWithStrings(PdfDocument doc, bool indirects) {
            PdfArray array = ((PdfArray)new PdfArray().MakeIndirect(doc));
            PdfPrimitivesTest.RandomString rnd = new PdfPrimitivesTest.RandomString(16);
            for (int i = 0; i < DefaultArraySize; i++) {
                array.Add(((PdfString)new PdfString(rnd.NextString()).MakeIndirect(indirects ? doc : null)));
            }
            return array;
        }

        private PdfArray GeneratePdfArrayWithNames(PdfDocument doc, bool indirects) {
            PdfArray array = ((PdfArray)new PdfArray().MakeIndirect(doc));
            PdfPrimitivesTest.RandomString rnd = new PdfPrimitivesTest.RandomString(6);
            for (int i = 0; i < DefaultArraySize; i++) {
                array.Add(((PdfName)new PdfName(rnd.NextString()).MakeIndirect(indirects ? doc : null)));
            }
            return array;
        }

        private PdfArray GeneratePdfArrayWithBooleans(PdfDocument doc, bool indirects) {
            PdfArray array = ((PdfArray)new PdfArray().MakeIndirect(doc));
            Random rnd = new Random();
            for (int i = 0; i < DefaultArraySize; i++) {
                array.Add(((PdfBoolean)new PdfBoolean(rnd.NextBoolean()).MakeIndirect(indirects ? doc : null)));
            }
            return array;
        }
    }
}
