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
using iText.Commons.Utils;
using iText.IO.Exceptions;
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
        public virtual void GetNextEofShortTextTest() {
            String data = "some text to test \ngetting end of\n file logic%%EOF";
            RandomAccessSourceFactory factory = new RandomAccessSourceFactory();
            using (PdfTokenizer tok = new PdfTokenizer(new RandomAccessFileOrArray(factory.CreateSource(data.GetBytes(
                iText.Commons.Utils.EncodingUtil.ISO_8859_1))))) {
                long eofPosition = tok.GetNextEof();
                NUnit.Framework.Assert.AreEqual(data.Length + 1, eofPosition);
            }
        }

        [NUnit.Framework.Test]
        public virtual void GetNextEofLongTextTest() {
            String data = "some text to test \ngetting end of\n file logic";
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < 20; ++i) {
                stringBuilder.Append(data);
            }
            stringBuilder.Append("%%EOF");
            RandomAccessSourceFactory factory = new RandomAccessSourceFactory();
            using (PdfTokenizer tok = new PdfTokenizer(new RandomAccessFileOrArray(factory.CreateSource(stringBuilder.
                ToString().GetBytes(iText.Commons.Utils.EncodingUtil.ISO_8859_1))))) {
                long eofPosition = tok.GetNextEof();
                NUnit.Framework.Assert.AreEqual(data.Length * 20 + 6, eofPosition);
            }
        }

        [NUnit.Framework.Test]
        public virtual void GetNextEofWhichIsCutTest() {
            StringBuilder stringBuilder = new StringBuilder();
            // We append 'a' 124 times because buffer has 128 bytes length.
            // This way '%%EOF' is cut and first string only contains '%%EO'
            for (int i = 0; i < 124; ++i) {
                stringBuilder.Append("a");
            }
            stringBuilder.Append("%%EOF");
            RandomAccessSourceFactory factory = new RandomAccessSourceFactory();
            using (PdfTokenizer tok = new PdfTokenizer(new RandomAccessFileOrArray(factory.CreateSource(stringBuilder.
                ToString().GetBytes(iText.Commons.Utils.EncodingUtil.ISO_8859_1))))) {
                long eofPosition = tok.GetNextEof();
                NUnit.Framework.Assert.AreEqual(124 + 6, eofPosition);
            }
        }

        [NUnit.Framework.Test]
        public virtual void GetNextEofSeveralEofTest() {
            String data = "some text %%EOFto test \nget%%EOFting end of\n fil%%EOFe logic%%EOF";
            RandomAccessSourceFactory factory = new RandomAccessSourceFactory();
            using (PdfTokenizer tok = new PdfTokenizer(new RandomAccessFileOrArray(factory.CreateSource(data.GetBytes(
                iText.Commons.Utils.EncodingUtil.ISO_8859_1))))) {
                long eofPosition = tok.GetNextEof();
                NUnit.Framework.Assert.AreEqual(data.IndexOf("%%EOF", StringComparison.Ordinal) + 6, eofPosition);
            }
        }

        [NUnit.Framework.Test]
        public virtual void GetNextEofNoEofTest() {
            String data = "some text to test \ngetting end of\n file logic";
            RandomAccessSourceFactory factory = new RandomAccessSourceFactory();
            using (PdfTokenizer tok = new PdfTokenizer(new RandomAccessFileOrArray(factory.CreateSource(data.GetBytes(
                iText.Commons.Utils.EncodingUtil.ISO_8859_1))))) {
                NUnit.Framework.Assert.Catch(typeof(iText.IO.Exceptions.IOException), () => tok.GetNextEof());
            }
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
            Exception e = NUnit.Framework.Assert.Catch(typeof(iText.IO.Exceptions.IOException), () => tok.ThrowError(IoExceptionMessageConstant
                .ERROR_AT_FILE_POINTER, 0));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(IoExceptionMessageConstant.ERROR_AT_FILE_POINTER, 
                0), e.Message);
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

        [NUnit.Framework.Test]
        public virtual void OctalNumberLong1Test() {
            // 49 equal to string "1", octal 1 equals to 1 in decimal
            byte[] bytes = new byte[] { 92, 49 };
            byte[] result = PdfTokenizer.DecodeStringContent(bytes, false);
            NUnit.Framework.Assert.AreEqual(new byte[] { 1 }, result);
        }

        [NUnit.Framework.Test]
        public virtual void OctalNumberLong2Test() {
            // 49 50 equal to string "12", octal 12 equals to 10 in decimal
            byte[] bytes = new byte[] { 92, 49, 50 };
            byte[] result = PdfTokenizer.DecodeStringContent(bytes, false);
            NUnit.Framework.Assert.AreEqual(new byte[] { 10 }, result);
        }

        [NUnit.Framework.Test]
        public virtual void OctalNumberLong3Test() {
            // 49 50 51 equal to string "123", octal 123 equals to 83 in decimal
            byte[] bytes = new byte[] { 92, 49, 50, 51 };
            byte[] result = PdfTokenizer.DecodeStringContent(bytes, false);
            NUnit.Framework.Assert.AreEqual(new byte[] { 83 }, result);
        }

        [NUnit.Framework.Test]
        public virtual void SlashAfterShortOctalTest() {
            // \0\(
            byte[] bytes = new byte[] { 92, 48, 92, 40 };
            byte[] result = PdfTokenizer.DecodeStringContent(bytes, false);
            NUnit.Framework.Assert.AreEqual(new byte[] { 0, 40 }, result);
        }

        [NUnit.Framework.Test]
        public virtual void NotOctalAfterShortOctalTest() {
            // \0&
            byte[] bytes = new byte[] { 92, 48, 26 };
            byte[] result = PdfTokenizer.DecodeStringContent(bytes, false);
            NUnit.Framework.Assert.AreEqual(new byte[] { 0, 26 }, result);
        }

        [NUnit.Framework.Test]
        public virtual void NotOctalAfterShortOctalTest2() {
            // \12&
            byte[] bytes = new byte[] { 92, 49, 50, 26 };
            byte[] result = PdfTokenizer.DecodeStringContent(bytes, false);
            NUnit.Framework.Assert.AreEqual(new byte[] { 10, 26 }, result);
        }

        [NUnit.Framework.Test]
        public virtual void TwoShortOctalsWithGarbageTest() {
            // \0\23 + 4 which should not be taken into account
            byte[] bytes = new byte[] { 92, 48, 92, 50, 51, 52 };
            byte[] result = PdfTokenizer.DecodeStringContent(bytes, 0, 4, false);
            NUnit.Framework.Assert.AreEqual(new byte[] { 0, 19 }, result);
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
