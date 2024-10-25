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
using iText.IO.Font;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Hyphenation;
using iText.Layout.Properties;
using iText.Test;

namespace iText.Layout {
    [NUnit.Framework.Category("IntegrationTest")]
    public class HyphenateLayoutTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/HyphenateLayoutTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/layout/HyphenateLayoutTest/";

        public static readonly String fontsFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/fonts/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void ParenthesisTest01() {
            //TODO DEVSIX-3148
            String outFileName = destinationFolder + "parenthesisTest01.pdf";
            String cmpFileName = sourceFolder + "cmp_parenthesisTest01.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDoc, new PageSize(300, 500));
            Hyphenator hyphenator = new Hyphenator("de", "de", 3, 3);
            HyphenationConfig hyphenationConfig = new HyphenationConfig(hyphenator);
            document.SetHyphenation(hyphenationConfig);
            document.Add(new Paragraph("1                             (((\"|Annuitätendarlehen|\")))"));
            document.Add(new Paragraph("2                              ((\"|Annuitätendarlehen|\"))"));
            document.Add(new Paragraph("3                               (\"|Annuitätendarlehen|\")"));
            document.Add(new Paragraph("4                                \"|Annuitätendarlehen|\""));
            document.Add(new Paragraph("5                                 \"Annuitätendarlehen\""));
            document.Add(new Paragraph("6                                      Annuitätendarlehen"));
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void UriTest01() {
            String outFileName = destinationFolder + "uriTest01.pdf";
            String cmpFileName = sourceFolder + "cmp_uriTest01.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document document = new Document(pdfDoc, new PageSize(140, 500));
            Hyphenator hyphenator = new Hyphenator("en", "en", 3, 3);
            HyphenationConfig hyphenationConfig = new HyphenationConfig(hyphenator);
            document.SetHyphenation(hyphenationConfig);
            Paragraph p = new Paragraph("https://stackoverflow.com/");
            document.Add(p);
            p = new Paragraph("http://stackoverflow.com/");
            document.Add(p);
            p = new Paragraph("m://iiiiiiii.com/");
            document.Add(p);
            document.Add(new AreaBreak());
            p = new Paragraph("https://stackoverflow.com/");
            p.SetHyphenation(null);
            document.Add(p);
            p = new Paragraph("http://stackoverflow.com/");
            p.SetHyphenation(null);
            document.Add(p);
            p = new Paragraph("m://iiiiiiii.com/");
            p.SetHyphenation(null);
            document.Add(p);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void WidthTest01() {
            String outFileName = destinationFolder + "widthTest01.pdf";
            String cmpFileName = sourceFolder + "cmp_widthTest01.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc);
            Text text = new Text("Hier ein link https://stackoverflow " + "\n" + " (Sperrvermerk) (Sperrvermerk)" + "\n"
                 + "„Sperrvermerk“ „Sperrvermerk“" + "\n" + "Der Sperrvermerk Sperrvermerk" + "\n" + "correct Sperr|ver|merk"
                );
            Paragraph paragraph = new Paragraph(text);
            paragraph.SetWidth(150);
            paragraph.SetTextAlignment(TextAlignment.JUSTIFIED);
            paragraph.SetHyphenation(new HyphenationConfig("de", "DE", 2, 2));
            doc.Add(paragraph);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void WidthTest02() {
            String outFileName = destinationFolder + "widthTest02.pdf";
            String cmpFileName = sourceFolder + "cmp_widthTest02.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc);
            Text text = new Text("Der/Die Depot-/Kontoinhaber muss/m\u00FCssen sich im Klaren dar\u00FCber sein.");
            Paragraph paragraph = new Paragraph(text);
            paragraph.SetWidth(210);
            paragraph.SetBorder(new SolidBorder(ColorConstants.BLACK, 1));
            paragraph.SetHyphenation(new HyphenationConfig("de", "DE", 2, 2));
            doc.Add(paragraph);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void WidthTest03() {
            String outFileName = destinationFolder + "widthTest03.pdf";
            String cmpFileName = sourceFolder + "cmp_widthTest03.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDocument);
            String s = "";
            s = s + "Hier ein Link: https://stackoverflow" + "\n";
            s = s + "(Sperrvermerk) (Sperrvermerk)" + "\n";
            s = s + "„Sperrvermerk“ „Sperrvermerk“" + "\n";
            s = s + "\"Sperrvermerk\" \"Sperrvermerk\"" + "\n";
            s = s + "'Sperrvermerk' 'Sperrvermerk'" + "\n";
            s = s + "Der Sperrvermerk Sperrvermerk" + "\n";
            s = s + "correct Sperr|ver|merk" + "\n";
            s = s + "Leistung Leistungen Leistung leisten" + "\n";
            s = s + "correct Leis|tung" + "\n";
            s = s + "Einmalig Einmalig Einmalig Einmalig" + "\n";
            s = s + "(Einmalig) (Einmalig) (Einmalig)" + "\n";
            s = s + "muss/müssen muss/müssen muss/müssen" + "\n";
            Paragraph p = new Paragraph(s).SetWidth(150).SetTextAlignment(TextAlignment.JUSTIFIED).SetBorderRight(new 
                SolidBorder(1)).SetHyphenation(new HyphenationConfig("de", "DE", 2, 2));
            doc.Add(p);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void NonBreakingHyphenTest01() {
            String outFileName = destinationFolder + "nonBreakingHyphenTest01.pdf";
            String cmpFileName = sourceFolder + "cmp_nonBreakingHyphenTest01.pdf";
            PdfWriter writer = new PdfWriter(outFileName);
            PdfDocument pdf = new PdfDocument(writer);
            Document document = new Document(pdf);
            Text text = new Text("Dies ist ein Satz in deutscher Sprache. An hm kann man sehen, ob alle Buchstaben da sind. Und der Umbruch? 99\u2011Tage-Kaiser.\n"
                 + "Dies ist ein Satz in deutscher Sprache. An hm kann man sehen, ob alle Buchstaben da sind. Und der Umbruch? 99\u2011Days-Kaiser.\n"
                 + "Dies ist ein Satz in deutscher Sprache. An hm kann man sehen, ob alle Buchstaben da sind. Und der Umbruch? 99\u2011Frage-Kaiser.\n"
                );
            PdfFont font = PdfFontFactory.CreateFont(fontsFolder + "FreeSans.ttf", PdfEncodings.IDENTITY_H);
            text.SetFont(font);
            text.SetFontSize(10);
            Paragraph paragraph = new Paragraph(text);
            paragraph.SetHyphenation(new HyphenationConfig("de", "DE", 2, 2));
            document.Add(paragraph);
            document.Close();
            pdf.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void NonBreakingHyphenTest02() {
            String outFileName = destinationFolder + "nonBreakingHyphenTest02.pdf";
            String cmpFileName = sourceFolder + "cmp_nonBreakingHyphenTest02.pdf";
            PdfWriter writer = new PdfWriter(outFileName);
            PdfDocument pdf = new PdfDocument(writer);
            Document document = new Document(pdf);
            Div div = new Div();
            div.SetHyphenation(new HyphenationConfig("en", "EN", 2, 2));
            PdfFont font = PdfFontFactory.CreateFont(fontsFolder + "FreeSans.ttf", PdfEncodings.IDENTITY_H);
            div.SetFont(font);
            div.SetFontSize(12);
            Text text = new Text("Hyphen hyphen hyphen hyphen hyphen hyphen hyphen hyphen hyphen hyphen hyphen ");
            Paragraph paragraph1 = new Paragraph().Add(text).Add("non\u2011breaking");
            div.Add(paragraph1);
            Paragraph paragraph2 = new Paragraph().Add(text).Add("non\u2010breaking");
            div.Add(paragraph2);
            document.Add(div);
            document.Close();
            pdf.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void HyphenSymbolTest01() {
            String outFileName = destinationFolder + "hyphenSymbolTest01.pdf";
            String cmpFileName = sourceFolder + "cmp_hyphenSymbolTest01.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDoc);
            PdfFont font = PdfFontFactory.CreateFont(fontsFolder + "FreeSans.ttf", PdfEncodings.IDENTITY_H);
            Style style = new Style();
            style.SetBorder(new SolidBorder(ColorConstants.BLACK, 1));
            style.SetHyphenation(new HyphenationConfig("en", "EN", 2, 2));
            style.SetFont(font);
            style.SetBackgroundColor(ColorConstants.RED);
            doc.Add(new Paragraph("tre\u2011").SetWidth(19).AddStyle(style));
            doc.Add(new Paragraph("tre\u2011\u2011").SetWidth(19).AddStyle(style));
            doc.Add(new Paragraph("r\u2011\u2011m").SetWidth(19).AddStyle(style));
            doc.Add(new Paragraph("r\u2011\u2011\u2011\u2011\u2011\u2011mmma").SetWidth(19).AddStyle(style));
            style.SetBackgroundColor(ColorConstants.BLUE);
            doc.Add(new Paragraph("tre\u2011\u2011").SetWidth(22).AddStyle(style));
            doc.Add(new Paragraph("tre\u2011\u2011m").SetWidth(22).AddStyle(style));
            doc.Add(new Paragraph("\n\n\n"));
            style.SetBackgroundColor(ColorConstants.GREEN);
            doc.Add(new Paragraph("e\u2011\u2011m\u2011ma").SetWidth(20).AddStyle(style));
            doc.Add(new Paragraph("tre\u2011\u2011m\u2011ma").SetWidth(20).AddStyle(style));
            doc.Add(new Paragraph("tre\u2011\u2011m\u2011ma").SetWidth(35).AddStyle(style));
            doc.Add(new Paragraph("tre\u2011\u2011m\u2011ma").SetWidth(40).AddStyle(style));
            style.SetBackgroundColor(ColorConstants.YELLOW);
            doc.Add(new Paragraph("ar\u2011ma").SetWidth(22).AddStyle(style));
            doc.Add(new Paragraph("ar\u2011ma").SetWidth(15).AddStyle(style));
            doc.Add(new Paragraph("ar\u2011").SetWidth(14).AddStyle(style));
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void WordsBreakingWordSoftHyphenTest() {
            String outFileName = destinationFolder + "wordsBreakingWordSoftHyphenTest.pdf";
            String cmpFileName = sourceFolder + "cmp_wordsBreakingWordSoftHyphenTest.pdf";
            String SOFT_HYPHEN = "\u00AD";
            String text = "Soft hyphen at the mid" + SOFT_HYPHEN + "dle,\nhyphen at the end: abcdef" + SOFT_HYPHEN + "ghijklmnopqrst\n"
                 + SOFT_HYPHEN + "hyphen at the beginning.";
            using (Document document = new Document(new PdfDocument(new PdfWriter(outFileName)))) {
                document.Add(new Paragraph(text).SetWidth(150).SetBorder(new SolidBorder(1)).SetHyphenation(new HyphenationConfig
                    (1, 1)));
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                ));
        }
    }
}
