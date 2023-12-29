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
using System.Collections.Generic;
using iText.IO.Font;
using iText.IO.Font.Constants;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using iText.Kernel.Pdf.Tagging;
using iText.Kernel.Pdf.Tagutils;
using iText.Kernel.Utils;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Kernel.Pdf {
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfStringTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/pdf/PdfStringTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/kernel/pdf/PdfStringTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.OneTimeTearDown]
        public static void AfterClass() {
            CompareTool.Cleanup(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void TestPdfDocumentInfoStringEncoding01() {
            String fileName = "testPdfDocumentInfoStringEncoding01.pdf";
            PdfDocument pdfDocument = new PdfDocument(CompareTool.CreateTestPdfWriter(destinationFolder + fileName, new 
                WriterProperties().SetCompressionLevel(CompressionConstants.NO_COMPRESSION)));
            pdfDocument.AddNewPage();
            String author = "Алексей";
            String title = "Заголовок";
            String subject = "Тема";
            String keywords = "Ключевые слова";
            String creator = "English text";
            pdfDocument.GetDocumentInfo().SetAuthor(author);
            pdfDocument.GetDocumentInfo().SetTitle(title);
            pdfDocument.GetDocumentInfo().SetSubject(subject);
            pdfDocument.GetDocumentInfo().SetKeywords(keywords);
            pdfDocument.GetDocumentInfo().SetCreator(creator);
            pdfDocument.Close();
            PdfDocument readDoc = new PdfDocument(CompareTool.CreateOutputReader(destinationFolder + fileName));
            NUnit.Framework.Assert.AreEqual(author, readDoc.GetDocumentInfo().GetAuthor());
            NUnit.Framework.Assert.AreEqual(title, readDoc.GetDocumentInfo().GetTitle());
            NUnit.Framework.Assert.AreEqual(subject, readDoc.GetDocumentInfo().GetSubject());
            NUnit.Framework.Assert.AreEqual(keywords, readDoc.GetDocumentInfo().GetKeywords());
            NUnit.Framework.Assert.AreEqual(creator, readDoc.GetDocumentInfo().GetCreator());
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + fileName, sourceFolder
                 + "cmp_" + fileName, destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void TestUnicodeString() {
            String unicode = "Привет!";
            PdfString @string = new PdfString(unicode);
            NUnit.Framework.Assert.AreNotEqual(unicode, @string.ToUnicodeString());
        }

        [NUnit.Framework.Test]
        public virtual void ReadUtf8ActualText() {
            String filename = sourceFolder + "utf-8-actual-text.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(filename));
            String text = PdfTextExtractor.GetTextFromPage(pdfDoc.GetPage(1), new LocationTextExtractionStrategy().SetUseActualText
                (true));
            pdfDoc.Close();
            //  शांति देवनागरी
            NUnit.Framework.Assert.AreEqual("\u0936\u093e\u0902\u0924\u093f \u0926\u0947\u0935\u0928\u093E\u0917\u0930\u0940"
                , text);
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.EXISTING_TAG_STRUCTURE_ROOT_IS_NOT_STANDARD)]
        public virtual void ReadUtf8AltText() {
            String filename = sourceFolder + "utf-8-alt-text.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(filename), CompareTool.CreateTestPdfWriter(destinationFolder
                 + "whatever"));
            TagTreePointer tagTreePointer = new TagTreePointer(pdfDoc);
            String alternateDescription = tagTreePointer.MoveToKid(0).MoveToKid(0).MoveToKid(0).GetProperties().GetAlternateDescription
                ();
            pdfDoc.Close();
            //  2001: A Space Odyssey (Космическая одиссея)
            NUnit.Framework.Assert.AreEqual("2001: A Space Odyssey (\u041A\u043E\u0441\u043C\u0438\u0447\u0435\u0441\u043A\u0430\u044F "
                 + "\u043E\u0434\u0438\u0441\u0441\u0435\u044F)", alternateDescription);
        }

        [NUnit.Framework.Test]
        public virtual void ReadUtf8Bookmarks() {
            String filename = sourceFolder + "utf-8-bookmarks.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(filename));
            PdfOutline outline = pdfDoc.GetOutlines(true);
            IList<String> children = new List<String>(6);
            foreach (PdfOutline child in outline.GetAllChildren()) {
                children.Add(child.GetTitle());
                foreach (PdfOutline childOfChild in child.GetAllChildren()) {
                    children.Add(childOfChild.GetTitle());
                }
            }
            pdfDoc.Close();
            IList<String> expected = new List<String>(6);
            //  福昕
            expected.Add("\u798F\u6615 bookmark 1");
            expected.Add("\u798F\u6615  bookmark 1-1");
            expected.Add("\u798F\u6615  bookmark 1-2");
            //  中国
            expected.Add("\u4E2D\u56FD bookmark 2");
            expected.Add("\u4E2D\u56FD  bookmark 2-1");
            expected.Add("\u4E2D\u56FD  bookmark 2-2");
            for (int i = 0; i < 6; i++) {
                NUnit.Framework.Assert.AreEqual(expected[i], children[i]);
            }
        }

        [NUnit.Framework.Test]
        public virtual void ReadUtf8PageLabelPrefix() {
            String filename = sourceFolder + "utf-8-page-label-prefix.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(filename));
            String[] labels = pdfDoc.GetPageLabels();
            String[] expected = new String[] { "A", "B", "1", "2", "3", "4", "Movies-5", "Movies-6", "Movies-7", "Movies-8"
                , "Movies-9", "Movies-10", "Movies-11", "Movies-12" };
            pdfDoc.Close();
            for (int i = 0; i < labels.Length; i++) {
                NUnit.Framework.Assert.AreEqual(expected[i], labels[i]);
            }
        }

        [NUnit.Framework.Test]
        public virtual void WriteUtf8AltText() {
            String RESOURCE = sourceFolder + "Space Odyssey.jpg";
            PdfDocument pdfDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(destinationFolder + "writeUtf8AltText.pdf"
                ));
            pdfDoc.SetTagged();
            PdfPage page = pdfDoc.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            TagTreePointer tagPointer = new TagTreePointer(pdfDoc);
            tagPointer.SetPageForTagging(page);
            tagPointer.AddTag(StandardRoles.DIV);
            tagPointer.AddTag(StandardRoles.SPAN);
            //  2001: A Space Odyssey (Космическая одиссея)
            tagPointer.GetContext().GetPointerStructElem(tagPointer).SetAlt(new PdfString("2001: A Space Odyssey (\u041A\u043E\u0441\u043C\u0438\u0447\u0435\u0441\u043A\u0430\u044F "
                 + "\u043E\u0434\u0438\u0441\u0441\u0435\u044F)", PdfEncodings.UTF8));
            ImageData img = ImageDataFactory.Create(RESOURCE);
            canvas.OpenTag(tagPointer.GetTagReference());
            canvas.AddImageFittedIntoRectangle(img, new Rectangle(36, 700, 65, 100), false);
            canvas.CloseTag();
            canvas.EndText();
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + "writeUtf8AltText.pdf"
                , sourceFolder + "cmp_writeUtf8AltText.pdf", destinationFolder, "diffAltText_"));
        }

        [NUnit.Framework.Test]
        public virtual void WriteUtf8Bookmarks() {
            PdfDocument pdfDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(destinationFolder + "writeUtf8Bookmarks.pdf"
                ));
            PdfPage page = pdfDoc.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.SetFillColor(ColorConstants.MAGENTA);
            canvas.BeginText();
            canvas.SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.TIMES_ROMAN), 30);
            canvas.SetTextMatrix(25, 500);
            canvas.ShowText("This file has bookmarks encoded with utf-8");
            canvas.EndText();
            PdfOutline root = pdfDoc.GetOutlines(false);
            PdfOutline first = root.AddOutline("");
            //  福昕
            first.GetContent().Put(PdfName.Title, new PdfString("\u798F\u6615 bookmark 1", PdfEncodings.UTF8));
            first.AddOutline("").GetContent().Put(PdfName.Title, new PdfString("\u798F\u6615  bookmark 1-1", PdfEncodings
                .UTF8));
            first.AddOutline("").GetContent().Put(PdfName.Title, new PdfString("\u798F\u6615  bookmark 1-2", PdfEncodings
                .UTF8));
            PdfOutline second = root.AddOutline("");
            //  中国
            second.GetContent().Put(PdfName.Title, new PdfString("\u4E2D\u56FD bookmark 2", PdfEncodings.UTF8));
            second.AddOutline("").GetContent().Put(PdfName.Title, new PdfString("\u4E2D\u56FD  bookmark 2-1", PdfEncodings
                .UTF8));
            second.AddOutline("").GetContent().Put(PdfName.Title, new PdfString("\u4E2D\u56FD  bookmark 2-2", PdfEncodings
                .UTF8));
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + "writeUtf8Bookmarks.pdf"
                , sourceFolder + "cmp_writeUtf8Bookmarks.pdf", destinationFolder, "diffBookmarks_"));
        }

        [NUnit.Framework.Test]
        public virtual void WriteUtf8PageLabelPrefix() {
            PdfDocument pdfDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(destinationFolder + "writeUtf8PageLabelPrefix.pdf"
                ));
            PdfPage page = pdfDoc.AddNewPage();
            PdfDictionary pageLabel = new PdfDictionary();
            pageLabel.Put(PdfName.S, PdfName.D);
            pageLabel.Put(PdfName.P, new PdfString("PREFIX-", PdfEncodings.UTF8));
            pageLabel.Put(PdfName.St, new PdfNumber(1));
            pdfDoc.GetCatalog().GetPageLabelsTree(true).AddEntry(pdfDoc.GetPageNumber(page) - 1, pageLabel);
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.SetFillColor(ColorConstants.MAGENTA);
            canvas.BeginText();
            canvas.SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.TIMES_ROMAN), 30);
            canvas.SetTextMatrix(25, 500);
            String text = "This page has pageLabel prefix " + "PREFIX-";
            canvas.ShowText(text);
            canvas.EndText();
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + "writeUtf8PageLabelPrefix.pdf"
                , sourceFolder + "cmp_writeUtf8PageLabelPrefix.pdf", destinationFolder, "diffPageLabelPrefix_"));
        }

        [NUnit.Framework.Test]
        public virtual void WriteUtf8ActualText() {
            PdfDocument pdfDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(destinationFolder + "writeUtf8ActualText.pdf"
                ));
            pdfDoc.SetTagged();
            PdfPage page = pdfDoc.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            TagTreePointer tagPointer = new TagTreePointer(pdfDoc);
            tagPointer.SetPageForTagging(page);
            tagPointer.AddTag(StandardRoles.DIV);
            tagPointer.AddTag(StandardRoles.SPAN);
            tagPointer.GetContext().GetPointerStructElem(tagPointer).SetActualText(new PdfString("actual", PdfEncodings
                .UTF8));
            canvas.BeginText();
            canvas.MoveText(36, 788);
            canvas.SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.TIMES_ROMAN), 12);
            canvas.OpenTag(tagPointer.GetTagReference());
            canvas.ShowText("These piece of text has an actual text property. Can be viewed via properties of span in the tag tree."
                );
            canvas.CloseTag();
            canvas.EndText();
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + "writeUtf8ActualText.pdf"
                , sourceFolder + "cmp_writeUtf8ActualText.pdf", destinationFolder, "diffActualText_"));
        }
    }
}
