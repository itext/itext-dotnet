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
using System;
using System.Text;
using iText.IO.Source;
using iText.Kernel.Utils;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Kernel.Pdf {
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfPrimitivesTest : ExtendedITextTest {
//\cond DO_NOT_DOCUMENT
        internal static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/kernel/pdf/PdfPrimitivesTest/";
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static readonly PdfName TestArray = new PdfName("TestArray");
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const int DefaultArraySize = 64;
//\endcond

//\cond DO_NOT_DOCUMENT
        internal const int PageCount = 1000;
//\endcond

        public class RandomString {
            private static readonly char[] symbols;

            private readonly Random random = new Random();

            private readonly char[] buf;

            static RandomString() {
                StringBuilder tmp = new StringBuilder();
                for (char ch = 'A'; ch <= 'Z'; ++ch) {
                    tmp.Append(ch);
                }
                for (char ch = 'a'; ch <= 'z'; ++ch) {
                    tmp.Append(ch);
                }
                for (char ch = '0'; ch <= '9'; ++ch) {
                    tmp.Append(ch);
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

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.OneTimeTearDown]
        public static void AfterClass() {
            CompareTool.Cleanup(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void PrimitivesFloatNumberTest() {
            String filename = "primitivesFloatNumberTest.pdf";
            PdfDocument pdfDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(destinationFolder + filename));
            for (int i = 0; i < PageCount; i++) {
                PdfPage page = pdfDoc.AddNewPage();
                PdfArray array = GeneratePdfArrayWithFloatNumbers(null, false);
                page.GetPdfObject().Put(TestArray, array);
                array.Flush();
                page.Flush();
            }
            pdfDoc.Close();
        }

        [NUnit.Framework.Test]
        public virtual void PrimitivesIntNumberTest() {
            String filename = "primitivesIntNumberTest.pdf";
            PdfDocument pdfDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(destinationFolder + filename));
            for (int i = 0; i < PageCount; i++) {
                PdfPage page = pdfDoc.AddNewPage();
                PdfArray array = GeneratePdfArrayWithIntNumbers(null, false);
                page.GetPdfObject().Put(TestArray, array);
                array.Flush();
                page.Flush();
            }
            pdfDoc.Close();
        }

        [NUnit.Framework.Test]
        public virtual void PrimitivesNameTest() {
            String filename = "primitivesNameTest.pdf";
            PdfDocument pdfDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(destinationFolder + filename));
            for (int i = 0; i < PageCount; i++) {
                PdfPage page = pdfDoc.AddNewPage();
                PdfArray array = GeneratePdfArrayWithNames(null, false);
                page.GetPdfObject().Put(TestArray, array);
                array.Flush();
                page.Flush();
            }
            pdfDoc.Close();
        }

        [NUnit.Framework.Test]
        public virtual void PrimitivesStringTest() {
            String filename = "primitivesStringTest.pdf";
            PdfDocument pdfDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(destinationFolder + filename));
            for (int i = 0; i < PageCount; i++) {
                PdfPage page = pdfDoc.AddNewPage();
                PdfArray array = GeneratePdfArrayWithStrings(null, false);
                page.GetPdfObject().Put(TestArray, array);
                array.Flush();
                page.Flush();
            }
            pdfDoc.Close();
        }

        [NUnit.Framework.Test]
        public virtual void PrimitivesBooleanTest() {
            String filename = "primitivesBooleanTest.pdf";
            PdfDocument pdfDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(destinationFolder + filename));
            for (int i = 0; i < PageCount; i++) {
                PdfPage page = pdfDoc.AddNewPage();
                page.GetPdfObject().Put(TestArray, GeneratePdfArrayWithBooleans(null, false));
                page.Flush();
            }
            pdfDoc.Close();
        }

        [NUnit.Framework.Test]
        public virtual void PrimitivesFloatNumberIndirectTest() {
            String filename = "primitivesFloatNumberIndirectTest.pdf";
            PdfDocument pdfDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(destinationFolder + filename));
            for (int i = 0; i < PageCount; i++) {
                PdfPage page = pdfDoc.AddNewPage();
                page.GetPdfObject().Put(TestArray, GeneratePdfArrayWithFloatNumbers(pdfDoc, true));
                page.Flush();
            }
            pdfDoc.Close();
        }

        [NUnit.Framework.Test]
        public virtual void PrimitivesIntNumberIndirectTest() {
            String filename = "primitivesIntNumberIndirectTest.pdf";
            PdfDocument pdfDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(destinationFolder + filename));
            for (int i = 0; i < PageCount; i++) {
                PdfPage page = pdfDoc.AddNewPage();
                page.GetPdfObject().Put(TestArray, GeneratePdfArrayWithIntNumbers(pdfDoc, true));
                page.Flush();
            }
            pdfDoc.Close();
        }

        [NUnit.Framework.Test]
        public virtual void PrimitivesStringIndirectTest() {
            String filename = "primitivesStringIndirectTest.pdf";
            PdfDocument pdfDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(destinationFolder + filename));
            for (int i = 0; i < PageCount; i++) {
                PdfPage page = pdfDoc.AddNewPage();
                page.GetPdfObject().Put(TestArray, GeneratePdfArrayWithStrings(pdfDoc, true));
                page.Flush();
            }
            pdfDoc.Close();
        }

        [NUnit.Framework.Test]
        public virtual void PrimitivesNameIndirectTest() {
            String filename = "primitivesNameIndirectTest.pdf";
            PdfDocument pdfDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(destinationFolder + filename));
            for (int i = 0; i < PageCount; i++) {
                PdfPage page = pdfDoc.AddNewPage();
                page.GetPdfObject().Put(TestArray, GeneratePdfArrayWithNames(pdfDoc, true));
                page.Flush();
            }
            pdfDoc.Close();
        }

        [NUnit.Framework.Test]
        public virtual void PrimitivesBooleanIndirectTest() {
            String filename = "primitivesBooleanIndirectTest.pdf";
            PdfDocument pdfDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(destinationFolder + filename));
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
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.DIRECTONLY_OBJECT_CANNOT_BE_INDIRECT)]
        public virtual void MakeIndirectDirectOnlyPdfBoolean() {
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
            PdfBoolean t = PdfBoolean.ValueOf(true);
            t.MakeIndirect(pdfDoc);
        }

        [NUnit.Framework.Test]
        public virtual void EqualStrings() {
            PdfString a = (PdfString)new PdfString("abcd").MakeIndirect(new PdfDocument(new PdfWriter(new ByteArrayOutputStream
                ())));
            PdfString b = new PdfString("abcd".GetBytes(System.Text.Encoding.ASCII));
            NUnit.Framework.Assert.IsTrue(a.Equals(b));
            PdfString c = new PdfString("abcd", "UTF-8");
            NUnit.Framework.Assert.IsFalse(c.Equals(a));
        }

        [NUnit.Framework.Test]
        public virtual void EqualNumbers() {
            using (PdfDocument document = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()))) {
                // add a page to avoid exception throwing on close
                document.AddNewPage();
                PdfNumber num1 = (PdfNumber)new PdfNumber(1).MakeIndirect(document);
                PdfNumber num2 = new PdfNumber(2);
                NUnit.Framework.Assert.IsFalse(num1.Equals(num2));
                int hashCode = num1.GetHashCode();
                num1.Increment();
                NUnit.Framework.Assert.IsTrue(num1.Equals(num2));
                NUnit.Framework.Assert.AreNotEqual(hashCode, num1.GetHashCode());
            }
            PdfNumber a = new PdfNumber(1);
            PdfNumber aContent = new PdfNumber(a.GetInternalContent());
            PdfNumber b = new PdfNumber(2);
            PdfNumber bContent = new PdfNumber(b.GetInternalContent());
            NUnit.Framework.Assert.IsTrue(a.Equals(aContent));
            NUnit.Framework.Assert.AreEqual(a.GetHashCode(), aContent.GetHashCode());
            NUnit.Framework.Assert.IsTrue(b.Equals(bContent));
            NUnit.Framework.Assert.AreEqual(b.GetHashCode(), bContent.GetHashCode());
            NUnit.Framework.Assert.IsFalse(aContent.Equals(bContent));
            NUnit.Framework.Assert.AreNotEqual(aContent.GetHashCode(), bContent.GetHashCode());
            aContent.Increment();
            NUnit.Framework.Assert.IsFalse(a.Equals(aContent));
            NUnit.Framework.Assert.AreNotEqual(a.GetHashCode(), aContent.GetHashCode());
            NUnit.Framework.Assert.IsTrue(aContent.Equals(bContent));
            NUnit.Framework.Assert.AreEqual(aContent.GetHashCode(), bContent.GetHashCode());
        }

        [NUnit.Framework.Test]
        public virtual void EqualNames() {
            PdfName a = (PdfName)new PdfName("abcd").MakeIndirect(new PdfDocument(new PdfWriter(new ByteArrayOutputStream
                ())));
            PdfName b = new PdfName("abcd");
            NUnit.Framework.Assert.IsTrue(a.Equals(b));
        }

        [NUnit.Framework.Test]
        public virtual void EqualBoolean() {
            PdfBoolean f = (PdfBoolean)new PdfBoolean(false).MakeIndirect(new PdfDocument(new PdfWriter(new ByteArrayOutputStream
                ())));
            PdfBoolean t = new PdfBoolean(true);
            NUnit.Framework.Assert.IsFalse(f.Equals(t));
            NUnit.Framework.Assert.IsTrue(f.Equals(PdfBoolean.FALSE));
            NUnit.Framework.Assert.IsTrue(t.Equals(PdfBoolean.TRUE));
        }

        [NUnit.Framework.Test]
        public virtual void EqualNulls() {
            PdfNull a = (PdfNull)new PdfNull().MakeIndirect(new PdfDocument(new PdfWriter(new ByteArrayOutputStream())
                ));
            NUnit.Framework.Assert.IsTrue(a.Equals(PdfNull.PDF_NULL));
        }

        [NUnit.Framework.Test]
        public virtual void EqualLiterals() {
            PdfLiteral a = new PdfLiteral("abcd");
            PdfLiteral b = new PdfLiteral("abcd".GetBytes(System.Text.Encoding.ASCII));
            NUnit.Framework.Assert.IsTrue(a.Equals(b));
        }

        private PdfArray GeneratePdfArrayWithFloatNumbers(PdfDocument doc, bool indirects) {
            PdfArray array = (PdfArray)new PdfArray().MakeIndirect(doc);
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
            PdfArray array = (PdfArray)new PdfArray().MakeIndirect(doc);
            Random rnd = new Random();
            for (int i = 0; i < DefaultArraySize; i++) {
                array.Add(new PdfNumber(rnd.Next()).MakeIndirect(indirects ? doc : null));
            }
            return array;
        }

        private PdfArray GeneratePdfArrayWithStrings(PdfDocument doc, bool indirects) {
            PdfArray array = (PdfArray)new PdfArray().MakeIndirect(doc);
            PdfPrimitivesTest.RandomString rnd = new PdfPrimitivesTest.RandomString(16);
            for (int i = 0; i < DefaultArraySize; i++) {
                array.Add(new PdfString(rnd.NextString()).MakeIndirect(indirects ? doc : null));
            }
            return array;
        }

        private PdfArray GeneratePdfArrayWithNames(PdfDocument doc, bool indirects) {
            PdfArray array = (PdfArray)new PdfArray().MakeIndirect(doc);
            PdfPrimitivesTest.RandomString rnd = new PdfPrimitivesTest.RandomString(6);
            for (int i = 0; i < DefaultArraySize; i++) {
                array.Add(new PdfName(rnd.NextString()).MakeIndirect(indirects ? doc : null));
            }
            return array;
        }

        private PdfArray GeneratePdfArrayWithBooleans(PdfDocument doc, bool indirects) {
            PdfArray array = (PdfArray)new PdfArray().MakeIndirect(doc);
            Random rnd = new Random();
            for (int i = 0; i < DefaultArraySize; i++) {
                array.Add(new PdfBoolean(rnd.NextBoolean()).MakeIndirect(indirects ? doc : null));
            }
            return array;
        }
    }
}
