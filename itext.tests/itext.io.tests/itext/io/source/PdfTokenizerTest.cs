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
using iText.Commons.Utils;
using iText.Test;

namespace iText.IO.Source {
    [NUnit.Framework.Category("UnitTest")]
    public class PdfTokenizerTest : ExtendedITextTest {
        private static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/io/util/";

        [NUnit.Framework.Test]
        public virtual void SeekTest() {
            String data = "/Name1 70";
            PdfTokenizer.TokenType[] expectedTypes = new PdfTokenizer.TokenType[] { PdfTokenizer.TokenType.Name, PdfTokenizer.TokenType
                .Number, PdfTokenizer.TokenType.EndOfFile };
            RandomAccessSourceFactory factory = new RandomAccessSourceFactory();
            PdfTokenizer tok = new PdfTokenizer(new RandomAccessFileOrArray(factory.CreateSource(data.GetBytes(iText.Commons.Utils.EncodingUtil.ISO_8859_1
                ))));
            tok.Seek(0);
            tok.NextValidToken();
            NUnit.Framework.Assert.AreEqual(expectedTypes[0], tok.GetTokenType());
            NUnit.Framework.Assert.AreEqual("Name1", tok.GetStringValue());
            tok.Seek(7);
            tok.NextValidToken();
            NUnit.Framework.Assert.AreEqual(expectedTypes[1], tok.GetTokenType());
            NUnit.Framework.Assert.AreEqual("70", tok.GetStringValue());
            tok.Seek(8);
            tok.NextValidToken();
            NUnit.Framework.Assert.AreEqual(expectedTypes[1], tok.GetTokenType());
            NUnit.Framework.Assert.AreEqual("0", tok.GetStringValue());
            tok.Seek(9);
            tok.NextValidToken();
            NUnit.Framework.Assert.AreEqual(expectedTypes[2], tok.GetTokenType());
        }

        [NUnit.Framework.Test]
        public virtual void GetLongValueTest() {
            String data = "21474836470";
            RandomAccessSourceFactory factory = new RandomAccessSourceFactory();
            PdfTokenizer tok = new PdfTokenizer(new RandomAccessFileOrArray(factory.CreateSource(data.GetBytes(iText.Commons.Utils.EncodingUtil.ISO_8859_1
                ))));
            tok.NextValidToken();
            NUnit.Framework.Assert.AreEqual(PdfTokenizer.TokenType.Number, tok.GetTokenType());
            NUnit.Framework.Assert.AreEqual(21474836470L, tok.GetLongValue());
        }

        [NUnit.Framework.Test]
        public virtual void GetIntValueTest() {
            String data = "15";
            RandomAccessSourceFactory factory = new RandomAccessSourceFactory();
            PdfTokenizer tok = new PdfTokenizer(new RandomAccessFileOrArray(factory.CreateSource(data.GetBytes(iText.Commons.Utils.EncodingUtil.ISO_8859_1
                ))));
            tok.NextValidToken();
            NUnit.Framework.Assert.AreEqual(PdfTokenizer.TokenType.Number, tok.GetTokenType());
            NUnit.Framework.Assert.AreEqual(15, tok.GetIntValue());
        }

        [NUnit.Framework.Test]
        public virtual void GetPositionTest() {
            String data = "/Name1 70";
            RandomAccessSourceFactory factory = new RandomAccessSourceFactory();
            PdfTokenizer tok = new PdfTokenizer(new RandomAccessFileOrArray(factory.CreateSource(data.GetBytes(iText.Commons.Utils.EncodingUtil.ISO_8859_1
                ))));
            NUnit.Framework.Assert.AreEqual(0, tok.GetPosition());
            tok.NextValidToken();
            NUnit.Framework.Assert.AreEqual(6, tok.GetPosition());
            tok.NextValidToken();
            NUnit.Framework.Assert.AreEqual(11, tok.GetPosition());
        }

        [NUnit.Framework.Test]
        public virtual void LengthTest() {
            String data = "/Name1";
            RandomAccessSourceFactory factory = new RandomAccessSourceFactory();
            PdfTokenizer tok = new PdfTokenizer(new RandomAccessFileOrArray(factory.CreateSource(data.GetBytes(iText.Commons.Utils.EncodingUtil.ISO_8859_1
                ))));
            NUnit.Framework.Assert.AreEqual(6, tok.Length());
        }

        [NUnit.Framework.Test]
        public virtual void LengthTwoTokenTest() {
            String data = "/Name1 15";
            RandomAccessSourceFactory factory = new RandomAccessSourceFactory();
            PdfTokenizer tok = new PdfTokenizer(new RandomAccessFileOrArray(factory.CreateSource(data.GetBytes(iText.Commons.Utils.EncodingUtil.ISO_8859_1
                ))));
            NUnit.Framework.Assert.AreEqual(9, tok.Length());
        }

        [NUnit.Framework.Test]
        public virtual void ReadTest() {
            String data = "/Name1 15";
            RandomAccessSourceFactory factory = new RandomAccessSourceFactory();
            PdfTokenizer tok = new PdfTokenizer(new RandomAccessFileOrArray(factory.CreateSource(data.GetBytes(iText.Commons.Utils.EncodingUtil.ISO_8859_1
                ))));
            byte[] read = new byte[] { (byte)tok.Read(), (byte)tok.Read(), (byte)tok.Read(), (byte)tok.Read(), (byte)tok
                .Read(), (byte)tok.Read(), (byte)tok.Read() };
            NUnit.Framework.Assert.AreEqual("/Name1 ", iText.Commons.Utils.JavaUtil.GetStringForBytes(read));
        }

        [NUnit.Framework.Test]
        public virtual void ReadStringFullTest() {
            String data = "/Name1 15";
            RandomAccessSourceFactory factory = new RandomAccessSourceFactory();
            PdfTokenizer tok = new PdfTokenizer(new RandomAccessFileOrArray(factory.CreateSource(data.GetBytes(iText.Commons.Utils.EncodingUtil.ISO_8859_1
                ))));
            NUnit.Framework.Assert.AreEqual(data, tok.ReadString(data.Length));
        }

        [NUnit.Framework.Test]
        public virtual void ReadStringShortTest() {
            String data = "/Name1 15";
            RandomAccessSourceFactory factory = new RandomAccessSourceFactory();
            PdfTokenizer tok = new PdfTokenizer(new RandomAccessFileOrArray(factory.CreateSource(data.GetBytes(iText.Commons.Utils.EncodingUtil.ISO_8859_1
                ))));
            NUnit.Framework.Assert.AreEqual("/Name", tok.ReadString(5));
        }

        [NUnit.Framework.Test]
        public virtual void ReadStringLongerThenDataTest() {
            String data = "/Name1 15";
            RandomAccessSourceFactory factory = new RandomAccessSourceFactory();
            PdfTokenizer tok = new PdfTokenizer(new RandomAccessFileOrArray(factory.CreateSource(data.GetBytes(iText.Commons.Utils.EncodingUtil.ISO_8859_1
                ))));
            NUnit.Framework.Assert.AreEqual(data, tok.ReadString(data.Length + 10));
        }

        [NUnit.Framework.Test]
        public virtual void ReadFullyPartThenReadStringTest() {
            String data = "/Name1 15";
            RandomAccessSourceFactory factory = new RandomAccessSourceFactory();
            PdfTokenizer tok = new PdfTokenizer(new RandomAccessFileOrArray(factory.CreateSource(data.GetBytes(iText.Commons.Utils.EncodingUtil.ISO_8859_1
                ))));
            tok.ReadFully(new byte[6]);
            NUnit.Framework.Assert.AreEqual(" 15", tok.ReadString(data.Length));
        }

        [NUnit.Framework.Test]
        public virtual void ReadFullyThenReadStringTest() {
            String data = "/Name1 15";
            RandomAccessSourceFactory factory = new RandomAccessSourceFactory();
            PdfTokenizer tok = new PdfTokenizer(new RandomAccessFileOrArray(factory.CreateSource(data.GetBytes(iText.Commons.Utils.EncodingUtil.ISO_8859_1
                ))));
            tok.ReadFully(new byte[7]);
            NUnit.Framework.Assert.AreEqual("15", tok.ReadString(data.Length));
        }

        [NUnit.Framework.Test]
        public virtual void GetDecodedStringContentTest() {
            String data = "/Name1 15";
            RandomAccessSourceFactory factory = new RandomAccessSourceFactory();
            PdfTokenizer tok = new PdfTokenizer(new RandomAccessFileOrArray(factory.CreateSource(data.GetBytes(iText.Commons.Utils.EncodingUtil.ISO_8859_1
                ))));
            tok.NextToken();
            NUnit.Framework.Assert.AreEqual("Name1", iText.Commons.Utils.JavaUtil.GetStringForBytes(tok.GetDecodedStringContent
                ()));
            tok.NextToken();
            NUnit.Framework.Assert.AreEqual("15", iText.Commons.Utils.JavaUtil.GetStringForBytes(tok.GetDecodedStringContent
                ()));
            tok.NextToken();
            NUnit.Framework.Assert.AreEqual("", iText.Commons.Utils.JavaUtil.GetStringForBytes(tok.GetDecodedStringContent
                ()));
        }

        [NUnit.Framework.Test]
        public virtual void GetDecodedStringContentHexTest() {
            String data = "<736f6d652068657820737472696e67>";
            RandomAccessSourceFactory factory = new RandomAccessSourceFactory();
            PdfTokenizer tok = new PdfTokenizer(new RandomAccessFileOrArray(factory.CreateSource(data.GetBytes(iText.Commons.Utils.EncodingUtil.ISO_8859_1
                ))));
            tok.NextToken();
            NUnit.Framework.Assert.IsTrue(tok.IsHexString());
            NUnit.Framework.Assert.AreEqual("some hex string", iText.Commons.Utils.JavaUtil.GetStringForBytes(tok.GetDecodedStringContent
                ()));
        }

        [NUnit.Framework.Test]
        public virtual void ThrowErrorTest() {
            RandomAccessSourceFactory factory = new RandomAccessSourceFactory();
            PdfTokenizer tok = new PdfTokenizer(new RandomAccessFileOrArray(factory.CreateSource("/Name1".GetBytes(iText.Commons.Utils.EncodingUtil.ISO_8859_1
                ))));
            Exception e = NUnit.Framework.Assert.Catch(typeof(iText.IO.Exceptions.IOException), () => tok.ThrowError(iText.IO.Exceptions.IOException
                .ErrorAtFilePointer1, 0));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(iText.IO.Exceptions.IOException.ErrorAtFilePointer1
                , 0), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void TestOneNumber() {
            CheckTokenTypes("/Name1 70", PdfTokenizer.TokenType.Name, PdfTokenizer.TokenType.Number, PdfTokenizer.TokenType
                .EndOfFile);
        }

        [NUnit.Framework.Test]
        public virtual void TestTwoNumbers() {
            CheckTokenTypes("/Name1 70/Name 2", PdfTokenizer.TokenType.Name, PdfTokenizer.TokenType.Number, PdfTokenizer.TokenType
                .Name, PdfTokenizer.TokenType.Number, PdfTokenizer.TokenType.EndOfFile);
        }

        [NUnit.Framework.Test]
        public virtual void TokenTypesTest() {
            CheckTokenTypes("<</Size 70/Root 46 0 R/Info 44 0 R/ID[<8C2547D58D4BD2C6F3D32B830BE3259D><8F69587888569A458EB681A4285D5879>]/Prev 116 >>"
                , PdfTokenizer.TokenType.StartDic, PdfTokenizer.TokenType.Name, PdfTokenizer.TokenType.Number, PdfTokenizer.TokenType
                .Name, PdfTokenizer.TokenType.Ref, PdfTokenizer.TokenType.Name, PdfTokenizer.TokenType.Ref, PdfTokenizer.TokenType
                .Name, PdfTokenizer.TokenType.StartArray, PdfTokenizer.TokenType.String, PdfTokenizer.TokenType.String
                , PdfTokenizer.TokenType.EndArray, PdfTokenizer.TokenType.Name, PdfTokenizer.TokenType.Number, PdfTokenizer.TokenType
                .EndDic, PdfTokenizer.TokenType.EndOfFile);
        }

        [NUnit.Framework.Test]
        public virtual void NumberValueInTheEndTest() {
            CheckTokenValues("123", new byte[] { 49, 50, 51 }, new byte[] {  });
        }

        //EndOfFile buffer
        [NUnit.Framework.Test]
        public virtual void TokenValueEqualsToTest() {
            String data = "SomeString";
            RandomAccessSourceFactory factory = new RandomAccessSourceFactory();
            PdfTokenizer tok = new PdfTokenizer(new RandomAccessFileOrArray(factory.CreateSource(data.GetBytes(iText.Commons.Utils.EncodingUtil.ISO_8859_1
                ))));
            tok.NextToken();
            NUnit.Framework.Assert.IsTrue(tok.TokenValueEqualsTo(data.GetBytes(iText.Commons.Utils.EncodingUtil.ISO_8859_1
                )));
        }

        [NUnit.Framework.Test]
        public virtual void TokenValueEqualsToNullTest() {
            String data = "SomeString";
            RandomAccessSourceFactory factory = new RandomAccessSourceFactory();
            PdfTokenizer tok = new PdfTokenizer(new RandomAccessFileOrArray(factory.CreateSource(data.GetBytes(iText.Commons.Utils.EncodingUtil.ISO_8859_1
                ))));
            tok.NextToken();
            NUnit.Framework.Assert.IsFalse(tok.TokenValueEqualsTo(null));
        }

        [NUnit.Framework.Test]
        public virtual void TokenValueEqualsToNotSameStringTest() {
            String data = "SomeString";
            RandomAccessSourceFactory factory = new RandomAccessSourceFactory();
            PdfTokenizer tok = new PdfTokenizer(new RandomAccessFileOrArray(factory.CreateSource(data.GetBytes(iText.Commons.Utils.EncodingUtil.ISO_8859_1
                ))));
            tok.NextToken();
            NUnit.Framework.Assert.IsFalse(tok.TokenValueEqualsTo((data + "s").GetBytes(iText.Commons.Utils.EncodingUtil.ISO_8859_1
                )));
        }

        [NUnit.Framework.Test]
        public virtual void TokenValueEqualsToNotCaseSensitiveStringTest() {
            String data = "SomeString";
            RandomAccessSourceFactory factory = new RandomAccessSourceFactory();
            PdfTokenizer tok = new PdfTokenizer(new RandomAccessFileOrArray(factory.CreateSource(data.GetBytes(iText.Commons.Utils.EncodingUtil.ISO_8859_1
                ))));
            tok.NextToken();
            NUnit.Framework.Assert.IsFalse(tok.TokenValueEqualsTo("Somestring".GetBytes(iText.Commons.Utils.EncodingUtil.ISO_8859_1
                )));
        }

        [NUnit.Framework.Test]
        public virtual void CheckPdfHeaderTest() {
            RandomAccessSourceFactory factory = new RandomAccessSourceFactory();
            PdfTokenizer tok = new PdfTokenizer(new RandomAccessFileOrArray(factory.CreateBestSource(sourceFolder + "test.pdf"
                )));
            NUnit.Framework.Assert.AreEqual("PDF-1.7", tok.CheckPdfHeader());
        }

        [NUnit.Framework.Test]
        public virtual void GetHeaderOffsetTest() {
            RandomAccessSourceFactory factory = new RandomAccessSourceFactory();
            PdfTokenizer tok = new PdfTokenizer(new RandomAccessFileOrArray(factory.CreateBestSource(sourceFolder + "test.pdf"
                )));
            NUnit.Framework.Assert.AreEqual(0, tok.GetHeaderOffset());
        }

        [NUnit.Framework.Test]
        public virtual void PrimitivesTest() {
            String data = "<</Size 70.%comment\n" + "/Value#20 .1" + "/Root 46 0 R" + "/Info 44 0 R" + "/ID[<736f6d652068657820737472696e672>(some simple string )<8C2547D58D4BD2C6F3D32B830BE3259D2>-70.1--0.2]"
                 + "/Name1 --15" + "/Prev ---116.23 >>";
            RandomAccessSourceFactory factory = new RandomAccessSourceFactory();
            PdfTokenizer tok = new PdfTokenizer(new RandomAccessFileOrArray(factory.CreateSource(data.GetBytes(iText.Commons.Utils.EncodingUtil.ISO_8859_1
                ))));
            tok.NextValidToken();
            NUnit.Framework.Assert.AreEqual(PdfTokenizer.TokenType.StartDic, tok.GetTokenType());
            tok.NextValidToken();
            NUnit.Framework.Assert.AreEqual(PdfTokenizer.TokenType.Name, tok.GetTokenType());
            NUnit.Framework.Assert.AreEqual("Size", iText.Commons.Utils.JavaUtil.GetStringForBytes(tok.GetByteContent(
                )));
            tok.NextValidToken();
            NUnit.Framework.Assert.AreEqual(PdfTokenizer.TokenType.Number, tok.GetTokenType());
            NUnit.Framework.Assert.AreEqual("70.", iText.Commons.Utils.JavaUtil.GetStringForBytes(tok.GetByteContent()
                ));
            tok.NextValidToken();
            NUnit.Framework.Assert.AreEqual(PdfTokenizer.TokenType.Name, tok.GetTokenType());
            NUnit.Framework.Assert.AreEqual("Value#20", iText.Commons.Utils.JavaUtil.GetStringForBytes(tok.GetByteContent
                ()));
            tok.NextValidToken();
            NUnit.Framework.Assert.AreEqual(PdfTokenizer.TokenType.Number, tok.GetTokenType());
            NUnit.Framework.Assert.AreEqual(".1", iText.Commons.Utils.JavaUtil.GetStringForBytes(tok.GetByteContent())
                );
            tok.NextValidToken();
            NUnit.Framework.Assert.AreEqual(PdfTokenizer.TokenType.Name, tok.GetTokenType());
            NUnit.Framework.Assert.AreEqual("Root", iText.Commons.Utils.JavaUtil.GetStringForBytes(tok.GetByteContent(
                )));
            tok.NextValidToken();
            NUnit.Framework.Assert.AreEqual(PdfTokenizer.TokenType.Ref, tok.GetTokenType());
            NUnit.Framework.Assert.AreEqual("46 0 R", "" + tok.GetObjNr() + " " + tok.GetGenNr() + " " + iText.Commons.Utils.JavaUtil.GetStringForBytes
                (tok.GetByteContent()));
            tok.NextValidToken();
            NUnit.Framework.Assert.AreEqual(PdfTokenizer.TokenType.Name, tok.GetTokenType());
            NUnit.Framework.Assert.AreEqual("Info", iText.Commons.Utils.JavaUtil.GetStringForBytes(tok.GetByteContent(
                )));
            tok.NextValidToken();
            NUnit.Framework.Assert.AreEqual(PdfTokenizer.TokenType.Ref, tok.GetTokenType());
            NUnit.Framework.Assert.AreEqual("44 0 R", "" + tok.GetObjNr() + " " + tok.GetGenNr() + " " + iText.Commons.Utils.JavaUtil.GetStringForBytes
                (tok.GetByteContent()));
            tok.NextValidToken();
            NUnit.Framework.Assert.AreEqual(PdfTokenizer.TokenType.Name, tok.GetTokenType());
            NUnit.Framework.Assert.AreEqual("ID", iText.Commons.Utils.JavaUtil.GetStringForBytes(tok.GetByteContent())
                );
            tok.NextValidToken();
            NUnit.Framework.Assert.AreEqual(PdfTokenizer.TokenType.StartArray, tok.GetTokenType());
            tok.NextValidToken();
            NUnit.Framework.Assert.AreEqual(PdfTokenizer.TokenType.String, tok.GetTokenType());
            NUnit.Framework.Assert.IsTrue(tok.IsHexString());
            NUnit.Framework.Assert.AreEqual("736f6d652068657820737472696e672", iText.Commons.Utils.JavaUtil.GetStringForBytes
                (tok.GetByteContent()));
            tok.NextValidToken();
            NUnit.Framework.Assert.AreEqual(PdfTokenizer.TokenType.String, tok.GetTokenType());
            NUnit.Framework.Assert.IsFalse(tok.IsHexString());
            NUnit.Framework.Assert.AreEqual("some simple string ", iText.Commons.Utils.JavaUtil.GetStringForBytes(tok.
                GetByteContent()));
            tok.NextValidToken();
            NUnit.Framework.Assert.AreEqual(PdfTokenizer.TokenType.String, tok.GetTokenType());
            NUnit.Framework.Assert.IsTrue(tok.IsHexString());
            NUnit.Framework.Assert.AreEqual("8C2547D58D4BD2C6F3D32B830BE3259D2", iText.Commons.Utils.JavaUtil.GetStringForBytes
                (tok.GetByteContent()));
            tok.NextValidToken();
            NUnit.Framework.Assert.AreEqual(PdfTokenizer.TokenType.Number, tok.GetTokenType());
            NUnit.Framework.Assert.AreEqual("-70.1", iText.Commons.Utils.JavaUtil.GetStringForBytes(tok.GetByteContent
                ()));
            tok.NextValidToken();
            NUnit.Framework.Assert.AreEqual(PdfTokenizer.TokenType.Number, tok.GetTokenType());
            NUnit.Framework.Assert.AreEqual("-0.2", iText.Commons.Utils.JavaUtil.GetStringForBytes(tok.GetByteContent(
                )));
            tok.NextValidToken();
            NUnit.Framework.Assert.AreEqual(PdfTokenizer.TokenType.EndArray, tok.GetTokenType());
            tok.NextValidToken();
            NUnit.Framework.Assert.AreEqual(PdfTokenizer.TokenType.Name, tok.GetTokenType());
            NUnit.Framework.Assert.AreEqual("Name1", iText.Commons.Utils.JavaUtil.GetStringForBytes(tok.GetByteContent
                ()));
            tok.NextValidToken();
            NUnit.Framework.Assert.AreEqual(PdfTokenizer.TokenType.Number, tok.GetTokenType());
            NUnit.Framework.Assert.AreEqual("0", iText.Commons.Utils.JavaUtil.GetStringForBytes(tok.GetByteContent()));
            tok.NextValidToken();
            NUnit.Framework.Assert.AreEqual(PdfTokenizer.TokenType.Name, tok.GetTokenType());
            NUnit.Framework.Assert.AreEqual("Prev", iText.Commons.Utils.JavaUtil.GetStringForBytes(tok.GetByteContent(
                )));
            tok.NextValidToken();
            NUnit.Framework.Assert.AreEqual(PdfTokenizer.TokenType.Number, tok.GetTokenType());
            NUnit.Framework.Assert.AreEqual("-116.23", iText.Commons.Utils.JavaUtil.GetStringForBytes(tok.GetByteContent
                ()));
        }

        private void CheckTokenTypes(String data, params PdfTokenizer.TokenType[] expectedTypes) {
            RandomAccessSourceFactory factory = new RandomAccessSourceFactory();
            PdfTokenizer tok = new PdfTokenizer(new RandomAccessFileOrArray(factory.CreateSource(data.GetBytes(iText.Commons.Utils.EncodingUtil.ISO_8859_1
                ))));
            for (int i = 0; i < expectedTypes.Length; i++) {
                tok.NextValidToken();
                NUnit.Framework.Assert.AreEqual(expectedTypes[i], tok.GetTokenType(), "Position " + i);
            }
        }

        private void CheckTokenValues(String data, params byte[][] expectedValues) {
            RandomAccessSourceFactory factory = new RandomAccessSourceFactory();
            PdfTokenizer tok = new PdfTokenizer(new RandomAccessFileOrArray(factory.CreateSource(data.GetBytes(iText.Commons.Utils.EncodingUtil.ISO_8859_1
                ))));
            for (int i = 0; i < expectedValues.Length; i++) {
                tok.NextValidToken();
                NUnit.Framework.Assert.AreEqual(expectedValues[i], tok.GetByteContent(), "Position " + i);
            }
        }
    }
}
