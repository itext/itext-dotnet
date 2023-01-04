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
using System.Text;
using iText.IO.Source;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Kernel.Pdf {
    [NUnit.Framework.Category("IntegrationTest")]
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

        [NUnit.Framework.SetUp]
        public virtual void Setup() {
            CreateDestinationFolder(destinationFolder);
        }

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
