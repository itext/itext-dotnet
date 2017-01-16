using System;
using iText.IO.Font;
using iText.IO.Source;

namespace iText.Kernel.Pdf {
    public class PdfTokenizerTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/pdf/PdfTokeniserTest/";

        /// <exception cref="System.Exception"/>
        private void CheckTokenTypes(String data, params PdfTokenizer.TokenType[] expectedTypes) {
            RandomAccessSourceFactory factory = new RandomAccessSourceFactory();
            PdfTokenizer tok = new PdfTokenizer(new RandomAccessFileOrArray(factory.CreateSource(data.GetBytes(iText.IO.Util.EncodingUtil.ISO_8859_1
                ))));
            for (int i = 0; i < expectedTypes.Length; i++) {
                tok.NextValidToken();
                //System.out.println(tok.getTokenType() + " -> " + tok.getStringValue());
                NUnit.Framework.Assert.AreEqual(expectedTypes[i], tok.GetTokenType(), "Position " + i);
            }
        }

        /// <exception cref="System.Exception"/>
        private void CheckTokenValues(String data, params byte[][] expectedValues) {
            RandomAccessSourceFactory factory = new RandomAccessSourceFactory();
            PdfTokenizer tok = new PdfTokenizer(new RandomAccessFileOrArray(factory.CreateSource(data.GetBytes(iText.IO.Util.EncodingUtil.ISO_8859_1
                ))));
            for (int i = 0; i < expectedValues.Length; i++) {
                tok.NextValidToken();
                //System.out.println(tok.getTokenType() + " -> " + tok.getStringValue());
                NUnit.Framework.Assert.AreEqual(expectedValues[i], tok.GetByteContent(), "Position " + i);
            }
        }

        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TestOneNumber() {
            CheckTokenTypes("/Name1 70", PdfTokenizer.TokenType.Name, PdfTokenizer.TokenType.Number, PdfTokenizer.TokenType
                .EndOfFile);
        }

        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TestTwoNumbers() {
            CheckTokenTypes("/Name1 70/Name 2", PdfTokenizer.TokenType.Name, PdfTokenizer.TokenType.Number, PdfTokenizer.TokenType
                .Name, PdfTokenizer.TokenType.Number, PdfTokenizer.TokenType.EndOfFile);
        }

        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TokenTypesTest() {
            CheckTokenTypes("<</Size 70/Root 46 0 R/Info 44 0 R/ID[<8C2547D58D4BD2C6F3D32B830BE3259D><8F69587888569A458EB681A4285D5879>]/Prev 116 >>"
                , PdfTokenizer.TokenType.StartDic, PdfTokenizer.TokenType.Name, PdfTokenizer.TokenType.Number, PdfTokenizer.TokenType
                .Name, PdfTokenizer.TokenType.Ref, PdfTokenizer.TokenType.Name, PdfTokenizer.TokenType.Ref, PdfTokenizer.TokenType
                .Name, PdfTokenizer.TokenType.StartArray, PdfTokenizer.TokenType.String, PdfTokenizer.TokenType.String
                , PdfTokenizer.TokenType.EndArray, PdfTokenizer.TokenType.Name, PdfTokenizer.TokenType.Number, PdfTokenizer.TokenType
                .EndDic, PdfTokenizer.TokenType.EndOfFile);
        }

        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void NumberValueInTheEndTest() {
            CheckTokenValues("123", new byte[] { 49, 50, 51 }, new byte[] {  });
        }

        //EndOfFile buffer
        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void EncodingTest() {
            RandomAccessSourceFactory factory;
            PdfTokenizer tok;
            PdfString pdfString;
            // hex string parse and check
            String testHexString = "<0D0A09557365729073204775696465>";
            factory = new RandomAccessSourceFactory();
            tok = new PdfTokenizer(new RandomAccessFileOrArray(factory.CreateSource(testHexString.GetBytes(iText.IO.Util.EncodingUtil.ISO_8859_1
                ))));
            tok.NextToken();
            pdfString = new PdfString(tok.GetByteContent(), tok.IsHexString());
            NUnit.Framework.Assert.AreEqual("\r\n\tUser\u0090s Guide", pdfString.GetValue());
            String testUnicodeString = "ΑΒΓΗ€•♣⋅";
            pdfString = new PdfString(PdfEncodings.ConvertToBytes(testUnicodeString, PdfEncodings.UNICODE_BIG), false);
            NUnit.Framework.Assert.AreEqual(testUnicodeString, pdfString.ToUnicodeString());
            pdfString = new PdfString("FEFF041F04400438043204350442".GetBytes(iText.IO.Util.EncodingUtil.ISO_8859_1), 
                true);
            NUnit.Framework.Assert.AreEqual("\u041F\u0440\u0438\u0432\u0435\u0442", pdfString.ToUnicodeString());
            pdfString = new PdfString("FEFF041F04400438043204350442".GetBytes(iText.IO.Util.EncodingUtil.ISO_8859_1), 
                false);
            NUnit.Framework.Assert.AreEqual("FEFF041F04400438043204350442", pdfString.ToUnicodeString());
            String specialCharacter = "\r\n\t\\n\\r\\t\\f";
            pdfString = new PdfString(specialCharacter.GetBytes(iText.IO.Util.EncodingUtil.ISO_8859_1), false);
            NUnit.Framework.Assert.AreEqual("\n\t\n\r\t\f", pdfString.ToUnicodeString());
            String symbol = "\u0001\u0004\u0006\u000E\u001F";
            pdfString = new PdfString(symbol.GetBytes(iText.IO.Util.EncodingUtil.ISO_8859_1), false);
            NUnit.Framework.Assert.AreEqual(symbol, pdfString.ToUnicodeString());
            String testString1 = "These\\\n two\\\r strings\\\n are the same";
            pdfString = new PdfString(testString1.GetBytes(iText.IO.Util.EncodingUtil.ISO_8859_1), false);
            NUnit.Framework.Assert.AreEqual("These two strings are the same", pdfString.GetValue());
            String testString2 = "This string contains \\245two octal characters\\307";
            pdfString = new PdfString(testString2.GetBytes(iText.IO.Util.EncodingUtil.ISO_8859_1), false);
            NUnit.Framework.Assert.AreEqual("This string contains \u00A5two octal characters\u00C7", pdfString.GetValue
                ());
            String testString3 = "\\0053";
            pdfString = new PdfString(testString3.GetBytes(iText.IO.Util.EncodingUtil.ISO_8859_1), false);
            NUnit.Framework.Assert.AreEqual("\u00053", pdfString.GetValue());
            String testString4 = "\\053";
            pdfString = new PdfString(testString4.GetBytes(iText.IO.Util.EncodingUtil.ISO_8859_1), false);
            NUnit.Framework.Assert.AreEqual("+", pdfString.GetValue());
            byte[] b = new byte[] { (byte)46, (byte)56, (byte)40 };
            pdfString = new PdfString(b, false);
            NUnit.Framework.Assert.AreEqual(iText.IO.Util.JavaUtil.GetStringForBytes(b), pdfString.GetValue());
        }

        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void ReadPdfStringTest() {
            String author = "This string9078 contains \u00A5two octal characters\u00C7";
            String creator = "iText\r 6\n";
            String title = "\u00DF\u00E3\u00EB\u00F0";
            String subject = "+";
            String filename = sourceFolder + "writePdfString.pdf";
            PdfReader reader = new PdfReader(filename);
            PdfDocument d = new PdfDocument(reader);
            // text in pdf: int array ( 223,227, 235,240)
            NUnit.Framework.Assert.AreEqual(d.GetDocumentInfo().GetTitle(), title);
            // text in pdf: This string\9078 contains \245two octal characters\307
            NUnit.Framework.Assert.AreEqual(d.GetDocumentInfo().GetAuthor(), author);
            // text in pdf: iText\r 6\n
            NUnit.Framework.Assert.AreEqual(d.GetDocumentInfo().GetCreator(), creator);
            // text in pdf: \053
            NUnit.Framework.Assert.AreEqual(d.GetDocumentInfo().GetSubject(), subject);
        }

        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void PrimitivesTest() {
            String data = "<</Size 70." + "/Value#20 .1" + "/Root 46 0 R" + "/Info 44 0 R" + "/ID[<736f6d652068657820737472696e672>(some simple string )<8C2547D58D4BD2C6F3D32B830BE3259D2>-70.1--0.2]"
                 + "/Name1 --15" + "/Prev ---116.23 >>";
            RandomAccessSourceFactory factory = new RandomAccessSourceFactory();
            PdfTokenizer tok = new PdfTokenizer(new RandomAccessFileOrArray(factory.CreateSource(data.GetBytes(iText.IO.Util.EncodingUtil.ISO_8859_1
                ))));
            tok.NextValidToken();
            NUnit.Framework.Assert.AreEqual(tok.GetTokenType(), PdfTokenizer.TokenType.StartDic);
            tok.NextValidToken();
            NUnit.Framework.Assert.AreEqual(tok.GetTokenType(), PdfTokenizer.TokenType.Name);
            PdfName name = new PdfName(tok.GetByteContent());
            NUnit.Framework.Assert.AreEqual("Size", name.GetValue());
            tok.NextValidToken();
            NUnit.Framework.Assert.AreEqual(tok.GetTokenType(), PdfTokenizer.TokenType.Number);
            PdfNumber num = new PdfNumber(tok.GetByteContent());
            NUnit.Framework.Assert.AreEqual("70.", num.ToString());
            tok.NextValidToken();
            NUnit.Framework.Assert.AreEqual(tok.GetTokenType(), PdfTokenizer.TokenType.Name);
            name = new PdfName(tok.GetByteContent());
            NUnit.Framework.Assert.AreEqual("Value ", name.GetValue());
            tok.NextValidToken();
            NUnit.Framework.Assert.AreEqual(tok.GetTokenType(), PdfTokenizer.TokenType.Number);
            num = new PdfNumber(tok.GetByteContent());
            NUnit.Framework.Assert.AreNotSame("0.1", num.ToString());
            tok.NextValidToken();
            NUnit.Framework.Assert.AreEqual(tok.GetTokenType(), PdfTokenizer.TokenType.Name);
            name = new PdfName(tok.GetByteContent());
            NUnit.Framework.Assert.AreEqual("Root", name.GetValue());
            tok.NextValidToken();
            NUnit.Framework.Assert.AreEqual(tok.GetTokenType(), PdfTokenizer.TokenType.Ref);
            PdfIndirectReference @ref = new PdfIndirectReference(null, tok.GetObjNr(), tok.GetGenNr());
            NUnit.Framework.Assert.AreEqual("46 0 R", @ref.ToString());
            tok.NextValidToken();
            NUnit.Framework.Assert.AreEqual(tok.GetTokenType(), PdfTokenizer.TokenType.Name);
            name = new PdfName(tok.GetByteContent());
            NUnit.Framework.Assert.AreEqual("Info", name.GetValue());
            tok.NextValidToken();
            NUnit.Framework.Assert.AreEqual(tok.GetTokenType(), PdfTokenizer.TokenType.Ref);
            @ref = new PdfIndirectReference(null, tok.GetObjNr(), tok.GetGenNr());
            NUnit.Framework.Assert.AreEqual("44 0 R", @ref.ToString());
            tok.NextValidToken();
            NUnit.Framework.Assert.AreEqual(tok.GetTokenType(), PdfTokenizer.TokenType.Name);
            name = new PdfName(tok.GetByteContent());
            NUnit.Framework.Assert.AreEqual("ID", name.GetValue());
            tok.NextValidToken();
            NUnit.Framework.Assert.AreEqual(tok.GetTokenType(), PdfTokenizer.TokenType.StartArray);
            tok.NextValidToken();
            NUnit.Framework.Assert.AreEqual(tok.GetTokenType(), PdfTokenizer.TokenType.String);
            NUnit.Framework.Assert.AreEqual(tok.IsHexString(), true);
            PdfString str = new PdfString(tok.GetByteContent(), tok.IsHexString());
            NUnit.Framework.Assert.AreEqual("some hex string ", str.GetValue());
            tok.NextValidToken();
            NUnit.Framework.Assert.AreEqual(tok.GetTokenType(), PdfTokenizer.TokenType.String);
            NUnit.Framework.Assert.AreEqual(tok.IsHexString(), false);
            str = new PdfString(tok.GetByteContent(), tok.IsHexString());
            NUnit.Framework.Assert.AreEqual("some simple string ", str.GetValue());
            tok.NextValidToken();
            NUnit.Framework.Assert.AreEqual(tok.GetTokenType(), PdfTokenizer.TokenType.String);
            NUnit.Framework.Assert.AreEqual(tok.IsHexString(), true);
            str = new PdfString(tok.GetByteContent(), tok.IsHexString());
            NUnit.Framework.Assert.AreEqual("\u008C%G\u00D5\u008DK\u00D2\u00C6\u00F3\u00D3+\u0083\u000B\u00E3%\u009D "
                , str.GetValue());
            tok.NextValidToken();
            NUnit.Framework.Assert.AreEqual(tok.GetTokenType(), PdfTokenizer.TokenType.Number);
            num = new PdfNumber(tok.GetByteContent());
            NUnit.Framework.Assert.AreEqual("-70.1", num.ToString());
            tok.NextValidToken();
            NUnit.Framework.Assert.AreEqual(tok.GetTokenType(), PdfTokenizer.TokenType.Number);
            num = new PdfNumber(tok.GetByteContent());
            NUnit.Framework.Assert.AreEqual("-0.2", num.ToString());
            tok.NextValidToken();
            NUnit.Framework.Assert.AreEqual(tok.GetTokenType(), PdfTokenizer.TokenType.EndArray);
            tok.NextValidToken();
            NUnit.Framework.Assert.AreEqual(tok.GetTokenType(), PdfTokenizer.TokenType.Name);
            name = new PdfName(tok.GetByteContent());
            NUnit.Framework.Assert.AreEqual("Name1", name.GetValue());
            tok.NextValidToken();
            NUnit.Framework.Assert.AreEqual(tok.GetTokenType(), PdfTokenizer.TokenType.Number);
            num = new PdfNumber(tok.GetByteContent());
            NUnit.Framework.Assert.AreEqual("0", num.ToString());
            tok.NextValidToken();
            NUnit.Framework.Assert.AreEqual(tok.GetTokenType(), PdfTokenizer.TokenType.Name);
            name = new PdfName(tok.GetByteContent());
            NUnit.Framework.Assert.AreEqual("Prev", name.GetValue());
            tok.NextValidToken();
            NUnit.Framework.Assert.AreEqual(tok.GetTokenType(), PdfTokenizer.TokenType.Number);
            num = new PdfNumber(tok.GetByteContent());
            NUnit.Framework.Assert.AreEqual("-116.23", num.ToString());
        }

        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void TokenValueEqualsToTest() {
            String data = "SomeString";
            RandomAccessSourceFactory factory = new RandomAccessSourceFactory();
            PdfTokenizer tok = new PdfTokenizer(new RandomAccessFileOrArray(factory.CreateSource(data.GetBytes(iText.IO.Util.EncodingUtil.ISO_8859_1
                ))));
            tok.NextToken();
            NUnit.Framework.Assert.IsTrue(tok.TokenValueEqualsTo(data.GetBytes(iText.IO.Util.EncodingUtil.ISO_8859_1))
                );
        }
    }
}
