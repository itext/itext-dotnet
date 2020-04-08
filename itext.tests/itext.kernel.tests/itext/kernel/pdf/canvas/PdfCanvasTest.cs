/*
This file is part of the iText (R) project.
Copyright (c) 1998-2020 iText Group NV
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
using System.Collections.Generic;
using System.IO;
using iText.IO.Font.Constants;
using iText.IO.Image;
using iText.IO.Source;
using iText.IO.Util;
using iText.Kernel;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Wmf;
using iText.Kernel.Pdf.Extgstate;
using iText.Kernel.Utils;
using iText.Test;

namespace iText.Kernel.Pdf.Canvas {
    public class PdfCanvasTest : ExtendedITextTest {
        /// <summary>Paths to images.</summary>
        public static readonly String[] RESOURCES = new String[] { "Desert.jpg", "bulb.gif", "0047478.jpg", "itext.png"
             };

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/kernel/pdf/canvas/PdfCanvasTest/";

        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/pdf/canvas/PdfCanvasTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void CreateSimpleCanvas() {
            String author = "Alexander Chingarev";
            String creator = "iText 6";
            String title = "Empty iText 6 Document";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(destinationFolder + "simpleCanvas.pdf"));
            pdfDoc.GetDocumentInfo().SetAuthor(author).SetCreator(creator).SetTitle(title);
            PdfPage page1 = pdfDoc.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page1);
            canvas.Rectangle(100, 100, 100, 100).Fill();
            canvas.Release();
            pdfDoc.Close();
            PdfReader reader = new PdfReader(destinationFolder + "simpleCanvas.pdf");
            PdfDocument pdfDocument = new PdfDocument(reader);
            NUnit.Framework.Assert.AreEqual(false, reader.HasRebuiltXref(), "Rebuilt");
            PdfDictionary info = pdfDocument.GetTrailer().GetAsDictionary(PdfName.Info);
            NUnit.Framework.Assert.AreEqual(author, info.Get(PdfName.Author).ToString(), "Author");
            NUnit.Framework.Assert.AreEqual(creator, info.Get(PdfName.Creator).ToString(), "Creator");
            NUnit.Framework.Assert.AreEqual(title, info.Get(PdfName.Title).ToString(), "Title");
            NUnit.Framework.Assert.AreEqual(1, pdfDocument.GetNumberOfPages(), "Page count");
            PdfDictionary page = pdfDocument.GetPage(1).GetPdfObject();
            NUnit.Framework.Assert.AreEqual(PdfName.Page, page.Get(PdfName.Type));
            reader.Close();
        }

        [NUnit.Framework.Test]
        public virtual void CreateSimpleCanvasWithDrawing() {
            String fileName = "simpleCanvasWithDrawing.pdf";
            String author = "Alexander Chingarev";
            String creator = "iText 6";
            String title = "Empty iText 6 Document";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(destinationFolder + fileName));
            pdfDoc.GetDocumentInfo().SetAuthor(author).SetCreator(creator).SetTitle(title);
            PdfPage page1 = pdfDoc.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page1);
            canvas.SaveState().SetLineWidth(30).MoveTo(36, 700).LineTo(300, 300).Stroke().RestoreState();
            canvas.SaveState().Rectangle(250, 500, 100, 100).Fill().RestoreState();
            canvas.SaveState().Circle(100, 400, 25).Fill().RestoreState();
            canvas.SaveState().RoundRectangle(100, 650, 100, 100, 10).Fill().RestoreState();
            canvas.SaveState().SetLineWidth(10).RoundRectangle(250, 650, 100, 100, 10).Stroke().RestoreState();
            canvas.SaveState().SetLineWidth(5).Arc(400, 650, 550, 750, 0, 180).Stroke().RestoreState();
            canvas.SaveState().SetLineWidth(5).MoveTo(400, 550).CurveTo(500, 570, 450, 450, 550, 550).Stroke().RestoreState
                ();
            canvas.Release();
            pdfDoc.Close();
            PdfReader reader = new PdfReader(destinationFolder + fileName);
            PdfDocument pdfDocument = new PdfDocument(reader);
            NUnit.Framework.Assert.AreEqual(false, reader.HasRebuiltXref(), "Rebuilt");
            PdfDictionary info = pdfDocument.GetTrailer().GetAsDictionary(PdfName.Info);
            NUnit.Framework.Assert.AreEqual(author, info.Get(PdfName.Author).ToString(), "Author");
            NUnit.Framework.Assert.AreEqual(creator, info.Get(PdfName.Creator).ToString(), "Creator");
            NUnit.Framework.Assert.AreEqual(title, info.Get(PdfName.Title).ToString(), "Title");
            NUnit.Framework.Assert.AreEqual(1, pdfDocument.GetNumberOfPages(), "Page count");
            PdfDictionary page = pdfDocument.GetPage(1).GetPdfObject();
            NUnit.Framework.Assert.AreEqual(PdfName.Page, page.Get(PdfName.Type));
            pdfDocument.Close();
        }

        [NUnit.Framework.Test]
        public virtual void CreateSimpleCanvasWithText() {
            String fileName = "simpleCanvasWithText.pdf";
            String author = "Alexander Chingarev";
            String creator = "iText 6";
            String title = "Empty iText 6 Document";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(destinationFolder + fileName));
            pdfDoc.GetDocumentInfo().SetAuthor(author).SetCreator(creator).SetTitle(title);
            PdfPage page1 = pdfDoc.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page1);
            //Initialize canvas and write text to it
            canvas.SaveState().BeginText().MoveText(36, 750).SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.HELVETICA
                ), 16).ShowText("Hello Helvetica!").EndText().RestoreState();
            canvas.SaveState().BeginText().MoveText(36, 700).SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLDOBLIQUE
                ), 16).ShowText("Hello Helvetica Bold Oblique!").EndText().RestoreState();
            canvas.SaveState().BeginText().MoveText(36, 650).SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.COURIER
                ), 16).ShowText("Hello Courier!").EndText().RestoreState();
            canvas.SaveState().BeginText().MoveText(36, 600).SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.TIMES_ITALIC
                ), 16).ShowText("Hello Times Italic!").EndText().RestoreState();
            canvas.SaveState().BeginText().MoveText(36, 550).SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.SYMBOL
                ), 16).ShowText("Hello Ellada!").EndText().RestoreState();
            canvas.SaveState().BeginText().MoveText(36, 500).SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.ZAPFDINGBATS
                ), 16).ShowText("Hello ZapfDingbats!").EndText().RestoreState();
            canvas.Release();
            pdfDoc.Close();
            PdfReader reader = new PdfReader(destinationFolder + fileName);
            PdfDocument pdfDocument = new PdfDocument(reader);
            NUnit.Framework.Assert.AreEqual(false, reader.HasRebuiltXref(), "Rebuilt");
            PdfDictionary info = pdfDocument.GetTrailer().GetAsDictionary(PdfName.Info);
            NUnit.Framework.Assert.AreEqual(author, info.Get(PdfName.Author).ToString(), "Author");
            NUnit.Framework.Assert.AreEqual(creator, info.Get(PdfName.Creator).ToString(), "Creator");
            NUnit.Framework.Assert.AreEqual(title, info.Get(PdfName.Title).ToString(), "Title");
            NUnit.Framework.Assert.AreEqual(1, pdfDocument.GetNumberOfPages(), "Page count");
            PdfDictionary page = pdfDocument.GetPage(1).GetPdfObject();
            NUnit.Framework.Assert.AreEqual(PdfName.Page, page.Get(PdfName.Type));
            pdfDocument.Close();
        }

        [NUnit.Framework.Test]
        public virtual void CreateSimpleCanvasWithPageFlush() {
            String author = "Alexander Chingarev";
            String creator = "iText 6";
            String title = "Empty iText 6 Document";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(destinationFolder + "simpleCanvasWithPageFlush.pdf"));
            pdfDoc.GetDocumentInfo().SetAuthor(author).SetCreator(creator).SetTitle(title);
            PdfPage page1 = pdfDoc.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page1);
            canvas.Rectangle(100, 100, 100, 100).Fill();
            canvas.Release();
            page1.Flush();
            pdfDoc.Close();
            PdfReader reader = new PdfReader(destinationFolder + "simpleCanvasWithPageFlush.pdf");
            PdfDocument pdfDocument = new PdfDocument(reader);
            NUnit.Framework.Assert.AreEqual(false, reader.HasRebuiltXref(), "Rebuilt");
            PdfDictionary info = pdfDocument.GetTrailer().GetAsDictionary(PdfName.Info);
            NUnit.Framework.Assert.AreEqual(author, info.Get(PdfName.Author).ToString(), "Author");
            NUnit.Framework.Assert.AreEqual(creator, info.Get(PdfName.Creator).ToString(), "Creator");
            NUnit.Framework.Assert.AreEqual(title, info.Get(PdfName.Title).ToString(), "Title");
            NUnit.Framework.Assert.AreEqual(1, pdfDocument.GetNumberOfPages(), "Page count");
            PdfDictionary page = pdfDocument.GetPage(1).GetPdfObject();
            NUnit.Framework.Assert.AreEqual(PdfName.Page, page.Get(PdfName.Type));
            pdfDocument.Close();
        }

        [NUnit.Framework.Test]
        public virtual void CreateSimpleCanvasWithFullCompression() {
            String author = "Alexander Chingarev";
            String creator = "iText 6";
            String title = "Empty iText 6 Document";
            PdfWriter writer = new PdfWriter(destinationFolder + "simpleCanvasWithFullCompression.pdf", new WriterProperties
                ().SetFullCompressionMode(true));
            PdfDocument pdfDoc = new PdfDocument(writer);
            pdfDoc.GetDocumentInfo().SetAuthor(author).SetCreator(creator).SetTitle(title);
            PdfPage page1 = pdfDoc.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page1);
            canvas.Rectangle(100, 100, 100, 100).Fill();
            canvas.Release();
            pdfDoc.Close();
            PdfReader reader = new PdfReader(destinationFolder + "simpleCanvasWithFullCompression.pdf");
            PdfDocument pdfDocument = new PdfDocument(reader);
            NUnit.Framework.Assert.AreEqual(false, reader.HasRebuiltXref(), "Rebuilt");
            PdfDictionary info = pdfDocument.GetTrailer().GetAsDictionary(PdfName.Info);
            NUnit.Framework.Assert.AreEqual(author, info.Get(PdfName.Author).ToString(), "Author");
            NUnit.Framework.Assert.AreEqual(creator, info.Get(PdfName.Creator).ToString(), "Creator");
            NUnit.Framework.Assert.AreEqual(title, info.Get(PdfName.Title).ToString(), "Title");
            NUnit.Framework.Assert.AreEqual(1, pdfDocument.GetNumberOfPages(), "Page count");
            PdfDictionary page = pdfDocument.GetPage(1).GetPdfObject();
            NUnit.Framework.Assert.AreEqual(PdfName.Page, page.Get(PdfName.Type));
            pdfDocument.Close();
        }

        [NUnit.Framework.Test]
        public virtual void CreateSimpleCanvasWithPageFlushAndFullCompression() {
            String author = "Alexander Chingarev";
            String creator = "iText 6";
            String title = "Empty iText 6 Document";
            PdfWriter writer = new PdfWriter(destinationFolder + "simpleCanvasWithPageFlushAndFullCompression.pdf", new 
                WriterProperties().SetFullCompressionMode(true));
            PdfDocument pdfDoc = new PdfDocument(writer);
            pdfDoc.GetDocumentInfo().SetAuthor(author).SetCreator(creator).SetTitle(title);
            PdfPage page1 = pdfDoc.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page1);
            canvas.Rectangle(100, 100, 100, 100).Fill();
            canvas.Release();
            page1.Flush();
            pdfDoc.Close();
            PdfReader reader = new PdfReader(destinationFolder + "simpleCanvasWithPageFlushAndFullCompression.pdf");
            PdfDocument pdfDocument = new PdfDocument(reader);
            NUnit.Framework.Assert.AreEqual(false, reader.HasRebuiltXref(), "Rebuilt");
            PdfDictionary info = pdfDocument.GetTrailer().GetAsDictionary(PdfName.Info);
            NUnit.Framework.Assert.AreEqual(author, info.Get(PdfName.Author).ToString(), "Author");
            NUnit.Framework.Assert.AreEqual(creator, info.Get(PdfName.Creator).ToString(), "Creator");
            NUnit.Framework.Assert.AreEqual(title, info.Get(PdfName.Title).ToString(), "Title");
            NUnit.Framework.Assert.AreEqual(1, pdfDocument.GetNumberOfPages(), "Page count");
            PdfDictionary page = pdfDocument.GetPage(1).GetPdfObject();
            NUnit.Framework.Assert.AreEqual(PdfName.Page, page.Get(PdfName.Type));
            pdfDocument.Close();
        }

        [NUnit.Framework.Test]
        public virtual void Create1000PagesDocument() {
            int pageCount = 1000;
            String filename = destinationFolder + pageCount + "PagesDocument.pdf";
            String author = "Alexander Chingarev";
            String creator = "iText 6";
            String title = "Empty iText 6 Document";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(filename));
            pdfDoc.GetDocumentInfo().SetAuthor(author).SetCreator(creator).SetTitle(title);
            for (int i = 0; i < pageCount; i++) {
                PdfPage page = pdfDoc.AddNewPage();
                PdfCanvas canvas = new PdfCanvas(page);
                canvas.SaveState().BeginText().MoveText(36, 700).SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.HELVETICA
                    ), 72).ShowText(JavaUtil.IntegerToString(i + 1)).EndText().RestoreState();
                canvas.Rectangle(100, 500, 100, 100).Fill();
                canvas.Release();
                page.Flush();
            }
            pdfDoc.Close();
            PdfReader reader = new PdfReader(destinationFolder + "1000PagesDocument.pdf");
            PdfDocument pdfDocument = new PdfDocument(reader);
            NUnit.Framework.Assert.AreEqual(false, reader.HasRebuiltXref(), "Rebuilt");
            PdfDictionary info = pdfDocument.GetTrailer().GetAsDictionary(PdfName.Info);
            NUnit.Framework.Assert.AreEqual(author, info.Get(PdfName.Author).ToString(), "Author");
            NUnit.Framework.Assert.AreEqual(creator, info.Get(PdfName.Creator).ToString(), "Creator");
            NUnit.Framework.Assert.AreEqual(title, info.Get(PdfName.Title).ToString(), "Title");
            NUnit.Framework.Assert.AreEqual(pageCount, pdfDocument.GetNumberOfPages(), "Page count");
            for (int i = 1; i <= pageCount; i++) {
                PdfDictionary page = pdfDocument.GetPage(i).GetPdfObject();
                NUnit.Framework.Assert.AreEqual(PdfName.Page, page.Get(PdfName.Type));
            }
            pdfDocument.Close();
        }

        [NUnit.Framework.Test]
        public virtual void Create100PagesDocument() {
            int pageCount = 100;
            String filename = destinationFolder + pageCount + "PagesDocument.pdf";
            String author = "Alexander Chingarev";
            String creator = "iText 6";
            String title = "Empty iText 6 Document";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(filename));
            pdfDoc.GetDocumentInfo().SetAuthor(author).SetCreator(creator).SetTitle(title);
            for (int i = 0; i < pageCount; i++) {
                PdfPage page = pdfDoc.AddNewPage();
                PdfCanvas canvas = new PdfCanvas(page);
                canvas.SaveState().BeginText().MoveText(36, 700).SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.HELVETICA
                    ), 72).ShowText(JavaUtil.IntegerToString(i + 1)).EndText().RestoreState();
                canvas.Rectangle(100, 500, 100, 100).Fill();
                canvas.Release();
                page.Flush();
            }
            pdfDoc.Close();
            PdfReader reader = new PdfReader(filename);
            PdfDocument pdfDocument = new PdfDocument(reader);
            NUnit.Framework.Assert.AreEqual(false, reader.HasRebuiltXref(), "Rebuilt");
            PdfDictionary info = pdfDocument.GetTrailer().GetAsDictionary(PdfName.Info);
            NUnit.Framework.Assert.AreEqual(author, info.Get(PdfName.Author).ToString(), "Author");
            NUnit.Framework.Assert.AreEqual(creator, info.Get(PdfName.Creator).ToString(), "Creator");
            NUnit.Framework.Assert.AreEqual(title, info.Get(PdfName.Title).ToString(), "Title");
            NUnit.Framework.Assert.AreEqual(pageCount, pdfDocument.GetNumberOfPages(), "Page count");
            for (int i = 1; i <= pageCount; i++) {
                PdfDictionary page = pdfDocument.GetPage(i).GetPdfObject();
                NUnit.Framework.Assert.AreEqual(PdfName.Page, page.Get(PdfName.Type));
            }
            pdfDocument.Close();
        }

        [NUnit.Framework.Test]
        public virtual void Create10PagesDocument() {
            int pageCount = 10;
            String filename = destinationFolder + pageCount + "PagesDocument.pdf";
            String author = "Alexander Chingarev";
            String creator = "iText 6";
            String title = "Empty iText 6 Document";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(filename));
            pdfDoc.GetDocumentInfo().SetAuthor(author).SetCreator(creator).SetTitle(title);
            for (int i = 0; i < pageCount; i++) {
                PdfPage page = pdfDoc.AddNewPage();
                PdfCanvas canvas = new PdfCanvas(page);
                canvas.SaveState().BeginText().MoveText(36, 700).SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.HELVETICA
                    ), 72).ShowText(JavaUtil.IntegerToString(i + 1)).EndText().RestoreState();
                canvas.Rectangle(100, 500, 100, 100).Fill();
                canvas.Release();
                page.Flush();
            }
            pdfDoc.Close();
            PdfReader reader = new PdfReader(filename);
            PdfDocument pdfDocument = new PdfDocument(reader);
            NUnit.Framework.Assert.AreEqual(false, reader.HasRebuiltXref(), "Rebuilt");
            PdfDictionary info = pdfDocument.GetTrailer().GetAsDictionary(PdfName.Info);
            NUnit.Framework.Assert.AreEqual(author, info.Get(PdfName.Author).ToString(), "Author");
            NUnit.Framework.Assert.AreEqual(creator, info.Get(PdfName.Creator).ToString(), "Creator");
            NUnit.Framework.Assert.AreEqual(title, info.Get(PdfName.Title).ToString(), "Title");
            NUnit.Framework.Assert.AreEqual(pageCount, pdfDocument.GetNumberOfPages(), "Page count");
            for (int i = 1; i <= pageCount; i++) {
                PdfDictionary page = pdfDocument.GetPage(i).GetPdfObject();
                NUnit.Framework.Assert.AreEqual(PdfName.Page, page.Get(PdfName.Type));
            }
            pdfDocument.Close();
        }

        [NUnit.Framework.Test]
        public virtual void Create1000PagesDocumentWithText() {
            int pageCount = 1000;
            String filename = destinationFolder + "1000PagesDocumentWithText.pdf";
            String author = "Alexander Chingarev";
            String creator = "iText 6";
            String title = "Empty iText 6 Document";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(filename));
            pdfDoc.GetDocumentInfo().SetAuthor(author).SetCreator(creator).SetTitle(title);
            for (int i = 0; i < pageCount; i++) {
                PdfPage page = pdfDoc.AddNewPage();
                PdfCanvas canvas = new PdfCanvas(page);
                canvas.SaveState().BeginText().MoveText(36, 650).SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.COURIER
                    ), 16).ShowText("Page " + (i + 1)).EndText();
                canvas.Rectangle(100, 100, 100, 100).Fill();
                canvas.Release();
                page.Flush();
            }
            pdfDoc.Close();
            PdfReader reader = new PdfReader(filename);
            PdfDocument pdfDocument = new PdfDocument(reader);
            NUnit.Framework.Assert.AreEqual(false, reader.HasRebuiltXref(), "Rebuilt");
            PdfDictionary info = pdfDocument.GetTrailer().GetAsDictionary(PdfName.Info);
            NUnit.Framework.Assert.AreEqual(author, info.Get(PdfName.Author).ToString(), "Author");
            NUnit.Framework.Assert.AreEqual(creator, info.Get(PdfName.Creator).ToString(), "Creator");
            NUnit.Framework.Assert.AreEqual(title, info.Get(PdfName.Title).ToString(), "Title");
            NUnit.Framework.Assert.AreEqual(pageCount, pdfDocument.GetNumberOfPages(), "Page count");
            for (int i = 1; i <= pageCount; i++) {
                PdfDictionary page = pdfDocument.GetPage(i).GetPdfObject();
                NUnit.Framework.Assert.AreEqual(PdfName.Page, page.Get(PdfName.Type));
            }
            pdfDocument.Close();
        }

        [NUnit.Framework.Test]
        public virtual void Create1000PagesDocumentWithFullCompression() {
            int pageCount = 1000;
            String filename = destinationFolder + "1000PagesDocumentWithFullCompression.pdf";
            String author = "Alexander Chingarev";
            String creator = "iText 6";
            String title = "Empty iText 6 Document";
            PdfWriter writer = new PdfWriter(filename, new WriterProperties().SetFullCompressionMode(true));
            PdfDocument pdfDoc = new PdfDocument(writer);
            pdfDoc.GetDocumentInfo().SetAuthor(author).SetCreator(creator).SetTitle(title);
            for (int i = 0; i < pageCount; i++) {
                PdfPage page = pdfDoc.AddNewPage();
                PdfCanvas canvas = new PdfCanvas(page);
                canvas.SaveState().BeginText().MoveText(36, 700).SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.HELVETICA
                    ), 72).ShowText(JavaUtil.IntegerToString(i + 1)).EndText().RestoreState();
                canvas.Rectangle(100, 500, 100, 100).Fill();
                canvas.Release();
                page.Flush();
            }
            pdfDoc.Close();
            PdfReader reader = new PdfReader(filename);
            PdfDocument pdfDocument = new PdfDocument(reader);
            NUnit.Framework.Assert.AreEqual(false, reader.HasRebuiltXref(), "Rebuilt");
            PdfDictionary info = pdfDocument.GetTrailer().GetAsDictionary(PdfName.Info);
            NUnit.Framework.Assert.AreEqual(author, info.Get(PdfName.Author).ToString(), "Author");
            NUnit.Framework.Assert.AreEqual(creator, info.Get(PdfName.Creator).ToString(), "Creator");
            NUnit.Framework.Assert.AreEqual(title, info.Get(PdfName.Title).ToString(), "Title");
            NUnit.Framework.Assert.AreEqual(pageCount, pdfDocument.GetNumberOfPages(), "Page count");
            for (int i = 1; i <= pageCount; i++) {
                PdfDictionary page = pdfDocument.GetPage(i).GetPdfObject();
                NUnit.Framework.Assert.AreEqual(PdfName.Page, page.Get(PdfName.Type));
            }
            pdfDocument.Close();
        }

        [NUnit.Framework.Test]
        public virtual void SmallDocumentWithFullCompression() {
            String filename = destinationFolder + "smallDocumentWithFullCompression.pdf";
            String author = "Alexander Chingarev";
            String creator = "iText 6";
            String title = "Empty iText 6 Document";
            PdfWriter writer = new PdfWriter(filename, new WriterProperties().SetFullCompressionMode(true));
            PdfDocument pdfDoc = new PdfDocument(writer);
            pdfDoc.GetDocumentInfo().SetAuthor(author).SetCreator(creator).SetTitle(title);
            PdfPage page = pdfDoc.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.SaveState().BeginText().MoveText(36, 700).SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.HELVETICA
                ), 72).ShowText("Hi!").EndText().RestoreState();
            page.Flush();
            pdfDoc.Close();
            PdfReader reader = new PdfReader(filename);
            PdfDocument pdfDocument = new PdfDocument(reader);
            NUnit.Framework.Assert.AreEqual(false, reader.HasRebuiltXref(), "Rebuilt");
            PdfDictionary info = pdfDocument.GetTrailer().GetAsDictionary(PdfName.Info);
            NUnit.Framework.Assert.AreEqual(author, info.Get(PdfName.Author).ToString(), "Author");
            NUnit.Framework.Assert.AreEqual(creator, info.Get(PdfName.Creator).ToString(), "Creator");
            NUnit.Framework.Assert.AreEqual(title, info.Get(PdfName.Title).ToString(), "Title");
            NUnit.Framework.Assert.AreEqual(1, pdfDocument.GetNumberOfPages(), "Page count");
            page = pdfDocument.GetPage(1);
            NUnit.Framework.Assert.AreEqual(PdfName.Page, page.GetPdfObject().Get(PdfName.Type));
            pdfDocument.Close();
        }

        [NUnit.Framework.Test]
        public virtual void Create100PagesDocumentWithFullCompression() {
            int pageCount = 100;
            String filename = destinationFolder + pageCount + "PagesDocumentWithFullCompression.pdf";
            String author = "Alexander Chingarev";
            String creator = "iText 6";
            String title = "Empty iText 6 Document";
            PdfWriter writer = new PdfWriter(filename, new WriterProperties().SetFullCompressionMode(true));
            PdfDocument pdfDoc = new PdfDocument(writer);
            pdfDoc.GetDocumentInfo().SetAuthor(author).SetCreator(creator).SetTitle(title);
            for (int i = 0; i < pageCount; i++) {
                PdfPage page = pdfDoc.AddNewPage();
                PdfCanvas canvas = new PdfCanvas(page);
                canvas.SaveState().BeginText().MoveText(36, 700).SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.HELVETICA
                    ), 72).ShowText(JavaUtil.IntegerToString(i + 1)).EndText().RestoreState();
                canvas.Rectangle(100, 500, 100, 100).Fill();
                canvas.Release();
                page.Flush();
            }
            pdfDoc.Close();
            PdfReader reader = new PdfReader(filename);
            PdfDocument pdfDocument = new PdfDocument(reader);
            NUnit.Framework.Assert.AreEqual(false, reader.HasRebuiltXref(), "Rebuilt");
            PdfDictionary info = pdfDocument.GetTrailer().GetAsDictionary(PdfName.Info);
            NUnit.Framework.Assert.AreEqual(author, info.Get(PdfName.Author).ToString(), "Author");
            NUnit.Framework.Assert.AreEqual(creator, info.Get(PdfName.Creator).ToString(), "Creator");
            NUnit.Framework.Assert.AreEqual(title, info.Get(PdfName.Title).ToString(), "Title");
            NUnit.Framework.Assert.AreEqual(pageCount, pdfDocument.GetNumberOfPages(), "Page count");
            for (int i = 1; i <= pageCount; i++) {
                PdfDictionary page = pdfDocument.GetPage(i).GetPdfObject();
                NUnit.Framework.Assert.AreEqual(PdfName.Page, page.Get(PdfName.Type));
            }
            pdfDocument.Close();
        }

        [NUnit.Framework.Test]
        public virtual void Create197PagesDocumentWithFullCompression() {
            int pageCount = 197;
            String filename = destinationFolder + pageCount + "PagesDocumentWithFullCompression.pdf";
            String author = "Alexander Chingarev";
            String creator = "iText 6";
            String title = "Empty iText 6 Document";
            PdfWriter writer = new PdfWriter(filename, new WriterProperties().SetFullCompressionMode(true));
            PdfDocument pdfDoc = new PdfDocument(writer);
            pdfDoc.GetDocumentInfo().SetAuthor(author).SetCreator(creator).SetTitle(title);
            for (int i = 0; i < pageCount; i++) {
                PdfPage page = pdfDoc.AddNewPage();
                PdfCanvas canvas = new PdfCanvas(page);
                canvas.SaveState().BeginText().MoveText(36, 700).SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.HELVETICA
                    ), 72).ShowText(JavaUtil.IntegerToString(i + 1)).EndText().RestoreState();
                canvas.Rectangle(100, 500, 100, 100).Fill();
                canvas.Release();
                page.Flush();
            }
            pdfDoc.Close();
            PdfReader reader = new PdfReader(filename);
            PdfDocument pdfDocument = new PdfDocument(reader);
            NUnit.Framework.Assert.AreEqual(false, reader.HasRebuiltXref(), "Rebuilt");
            PdfDictionary info = pdfDocument.GetTrailer().GetAsDictionary(PdfName.Info);
            NUnit.Framework.Assert.AreEqual(author, info.Get(PdfName.Author).ToString(), "Author");
            NUnit.Framework.Assert.AreEqual(creator, info.Get(PdfName.Creator).ToString(), "Creator");
            NUnit.Framework.Assert.AreEqual(title, info.Get(PdfName.Title).ToString(), "Title");
            NUnit.Framework.Assert.AreEqual(pageCount, pdfDocument.GetNumberOfPages(), "Page count");
            for (int i = 1; i <= pageCount; i++) {
                PdfDictionary page = pdfDocument.GetPage(i).GetPdfObject();
                NUnit.Framework.Assert.AreEqual(PdfName.Page, page.Get(PdfName.Type));
            }
            pdfDocument.Close();
        }

        [NUnit.Framework.Test]
        public virtual void Create10PagesDocumentWithFullCompression() {
            int pageCount = 10;
            String filename = destinationFolder + pageCount + "PagesDocumentWithFullCompression.pdf";
            String author = "Alexander Chingarev";
            String creator = "iText 6";
            String title = "Empty iText 6 Document";
            PdfWriter writer = new PdfWriter(filename, new WriterProperties().SetFullCompressionMode(true));
            PdfDocument pdfDoc = new PdfDocument(writer);
            pdfDoc.GetDocumentInfo().SetAuthor(author).SetCreator(creator).SetTitle(title);
            for (int i = 0; i < pageCount; i++) {
                PdfPage page = pdfDoc.AddNewPage();
                PdfCanvas canvas = new PdfCanvas(page);
                canvas.SaveState().BeginText().MoveText(36, 700).SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.HELVETICA
                    ), 72).ShowText(JavaUtil.IntegerToString(i + 1)).EndText().RestoreState();
                canvas.Rectangle(100, 500, 100, 100).Fill();
                canvas.Release();
                page.Flush();
            }
            pdfDoc.Close();
            PdfReader reader = new PdfReader(filename);
            PdfDocument pdfDocument = new PdfDocument(reader);
            NUnit.Framework.Assert.AreEqual(false, reader.HasRebuiltXref(), "Rebuilt");
            PdfDictionary info = pdfDocument.GetTrailer().GetAsDictionary(PdfName.Info);
            NUnit.Framework.Assert.AreEqual(author, info.Get(PdfName.Author).ToString(), "Author");
            NUnit.Framework.Assert.AreEqual(creator, info.Get(PdfName.Creator).ToString(), "Creator");
            NUnit.Framework.Assert.AreEqual(title, info.Get(PdfName.Title).ToString(), "Title");
            NUnit.Framework.Assert.AreEqual(pageCount, pdfDocument.GetNumberOfPages(), "Page count");
            for (int i = 1; i <= pageCount; i++) {
                PdfDictionary page = pdfDocument.GetPage(i).GetPdfObject();
                NUnit.Framework.Assert.AreEqual(PdfName.Page, page.Get(PdfName.Type));
            }
            pdfDocument.Close();
        }

        [NUnit.Framework.Test]
        public virtual void CopyPagesTest1() {
            String file1 = destinationFolder + "copyPages1_1.pdf";
            String file2 = destinationFolder + "copyPages1_2.pdf";
            PdfDocument pdfDoc1 = new PdfDocument(new PdfWriter(file1));
            PdfPage page1 = pdfDoc1.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page1);
            canvas.Rectangle(100, 600, 100, 100);
            canvas.Fill();
            canvas.BeginText();
            canvas.SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.COURIER), 12);
            canvas.SetTextMatrix(1, 0, 0, 1, 100, 500);
            canvas.ShowText("Hello World!");
            canvas.EndText();
            canvas.Release();
            page1.Flush();
            pdfDoc1.Close();
            pdfDoc1 = new PdfDocument(new PdfReader(file1));
            page1 = pdfDoc1.GetPage(1);
            PdfDocument pdfDoc2 = new PdfDocument(new PdfWriter(file2));
            PdfPage page2 = page1.CopyTo(pdfDoc2);
            pdfDoc2.AddPage(page2);
            page2.Flush();
            pdfDoc2.Close();
            PdfReader reader = new PdfReader(file2);
            PdfDocument pdfDocument = new PdfDocument(reader);
            for (int i = 1; i <= pdfDocument.GetNumberOfPages(); i++) {
                PdfDictionary page = pdfDocument.GetPage(i).GetPdfObject();
                NUnit.Framework.Assert.AreEqual(PdfName.Page, page.Get(PdfName.Type));
            }
            reader.Close();
            NUnit.Framework.Assert.AreEqual(false, reader.HasRebuiltXref(), "Rebuilt");
            PdfDictionary page_1 = pdfDocument.GetPage(1).GetPdfObject();
            NUnit.Framework.Assert.IsNotNull(page_1.Get(PdfName.Parent));
            pdfDocument.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(file1, file2, destinationFolder, "diff_")
                );
        }

        [NUnit.Framework.Test]
        public virtual void CopyPagesTest2() {
            String file1 = destinationFolder + "copyPages2_1.pdf";
            String file2 = destinationFolder + "copyPages2_2.pdf";
            PdfDocument pdfDoc1 = new PdfDocument(new PdfWriter(file1));
            for (int i = 0; i < 10; i++) {
                PdfPage page1 = pdfDoc1.AddNewPage();
                PdfCanvas canvas = new PdfCanvas(page1);
                canvas.Rectangle(100, 600, 100, 100);
                canvas.Fill();
                canvas.BeginText();
                canvas.SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.COURIER), 12);
                canvas.SetTextMatrix(1, 0, 0, 1, 100, 500);
                canvas.ShowText(MessageFormatUtil.Format("Page_{0}", i + 1));
                canvas.EndText();
                canvas.Release();
                page1.Flush();
            }
            pdfDoc1.Close();
            pdfDoc1 = new PdfDocument(new PdfReader(file1));
            PdfDocument pdfDoc2 = new PdfDocument(new PdfWriter(file2));
            for (int i = 9; i >= 0; i--) {
                PdfPage page2 = pdfDoc1.GetPage(i + 1).CopyTo(pdfDoc2);
                pdfDoc2.AddPage(page2);
            }
            pdfDoc1.Close();
            pdfDoc2.Close();
            PdfReader reader = new PdfReader(file2);
            PdfDocument pdfDocument = new PdfDocument(reader);
            NUnit.Framework.Assert.AreEqual(false, reader.HasRebuiltXref(), "Rebuilt");
            PdfDictionary page = pdfDocument.GetPage(1).GetPdfObject();
            NUnit.Framework.Assert.IsNotNull(page.Get(PdfName.Parent));
            pdfDocument.Close();
            CompareTool cmpTool = new CompareTool();
            PdfDocument doc1 = new PdfDocument(new PdfReader(file1));
            PdfDocument doc2 = new PdfDocument(new PdfReader(file2));
            for (int i = 0; i < 10; i++) {
                PdfDictionary page1 = doc1.GetPage(i + 1).GetPdfObject();
                PdfDictionary page2 = doc2.GetPage(10 - i).GetPdfObject();
                NUnit.Framework.Assert.IsTrue(cmpTool.CompareDictionaries(page1, page2));
            }
            doc1.Close();
            doc2.Close();
        }

        [NUnit.Framework.Test]
        public virtual void CopyPagesTest3() {
            String file1 = destinationFolder + "copyPages3_1.pdf";
            String file2 = destinationFolder + "copyPages3_2.pdf";
            PdfDocument pdfDoc1 = new PdfDocument(new PdfWriter(file1));
            PdfPage page1 = pdfDoc1.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page1);
            canvas.Rectangle(100, 600, 100, 100);
            canvas.Fill();
            canvas.BeginText();
            canvas.SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.COURIER), 12);
            canvas.SetTextMatrix(1, 0, 0, 1, 100, 500);
            canvas.ShowText("Hello World!!!");
            canvas.EndText();
            canvas.Release();
            page1.Flush();
            pdfDoc1.Close();
            pdfDoc1 = new PdfDocument(new PdfReader(file1));
            page1 = pdfDoc1.GetPage(1);
            PdfDocument pdfDoc2 = new PdfDocument(new PdfWriter(file2));
            for (int i = 0; i < 10; i++) {
                PdfPage page2 = page1.CopyTo(pdfDoc2);
                pdfDoc2.AddPage(page2);
                if (i % 2 == 0) {
                    page2.Flush();
                }
            }
            pdfDoc1.Close();
            pdfDoc2.Close();
            CompareTool cmpTool = new CompareTool();
            PdfReader reader1 = new PdfReader(file1);
            PdfDocument doc1 = new PdfDocument(reader1);
            NUnit.Framework.Assert.AreEqual(false, reader1.HasRebuiltXref(), "Rebuilt");
            PdfDictionary p1 = doc1.GetPage(1).GetPdfObject();
            PdfReader reader2 = new PdfReader(file2);
            PdfDocument doc2 = new PdfDocument(reader2);
            NUnit.Framework.Assert.AreEqual(false, reader2.HasRebuiltXref(), "Rebuilt");
            for (int i = 0; i < 10; i++) {
                PdfDictionary p2 = doc2.GetPage(i + 1).GetPdfObject();
                NUnit.Framework.Assert.IsTrue(cmpTool.CompareDictionaries(p1, p2));
            }
            doc1.Close();
            doc2.Close();
        }

        [NUnit.Framework.Test]
        public virtual void CopyPagesTest4() {
            String file1 = destinationFolder + "copyPages4_1.pdf";
            PdfDocument pdfDoc1 = new PdfDocument(new PdfWriter(file1));
            for (int i = 0; i < 5; i++) {
                PdfPage page1 = pdfDoc1.AddNewPage();
                PdfCanvas canvas = new PdfCanvas(page1);
                canvas.Rectangle(100, 600, 100, 100);
                canvas.Fill();
                canvas.BeginText();
                canvas.SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.COURIER), 12);
                canvas.SetTextMatrix(1, 0, 0, 1, 100, 500);
                canvas.ShowText(MessageFormatUtil.Format("Page_{0}", i + 1));
                canvas.EndText();
                canvas.Release();
            }
            pdfDoc1.Close();
            pdfDoc1 = new PdfDocument(new PdfReader(file1));
            for (int i = 0; i < 5; i++) {
                PdfDocument pdfDoc2 = new PdfDocument(new PdfWriter(destinationFolder + MessageFormatUtil.Format("copyPages4_{0}.pdf"
                    , i + 2)));
                PdfPage page2 = pdfDoc1.GetPage(i + 1).CopyTo(pdfDoc2);
                pdfDoc2.AddPage(page2);
                pdfDoc2.Close();
            }
            pdfDoc1.Close();
            CompareTool cmpTool = new CompareTool();
            PdfReader reader1 = new PdfReader(file1);
            PdfDocument doc1 = new PdfDocument(reader1);
            NUnit.Framework.Assert.AreEqual(false, reader1.HasRebuiltXref(), "Rebuilt");
            for (int i = 0; i < 5; i++) {
                PdfDictionary page1 = doc1.GetPage(i + 1).GetPdfObject();
                PdfDocument doc2 = new PdfDocument(new PdfReader(destinationFolder + MessageFormatUtil.Format("copyPages4_{0}.pdf"
                    , i + 2)));
                PdfDictionary page = doc2.GetPage(1).GetPdfObject();
                NUnit.Framework.Assert.IsTrue(cmpTool.CompareDictionaries(page1, page));
                doc2.Close();
            }
            doc1.Close();
        }

        [NUnit.Framework.Test]
        public virtual void CopyPagesTest5() {
            int documentCount = 3;
            for (int i = 0; i < documentCount; i++) {
                PdfDocument pdfDoc1 = new PdfDocument(new PdfWriter(destinationFolder + MessageFormatUtil.Format("copyPages5_{0}.pdf"
                    , i + 1)));
                PdfPage page1 = pdfDoc1.AddNewPage();
                PdfCanvas canvas = new PdfCanvas(page1);
                canvas.Rectangle(100, 600, 100, 100);
                canvas.Fill();
                canvas.BeginText();
                canvas.SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.COURIER), 12);
                canvas.SetTextMatrix(1, 0, 0, 1, 100, 500);
                canvas.ShowText(MessageFormatUtil.Format("Page_{0}", i + 1));
                canvas.EndText();
                canvas.Release();
                pdfDoc1.Close();
            }
            IList<PdfDocument> docs = new List<PdfDocument>();
            for (int i = 0; i < documentCount; i++) {
                PdfDocument pdfDoc1 = new PdfDocument(new PdfReader(destinationFolder + MessageFormatUtil.Format("copyPages5_{0}.pdf"
                    , i + 1)));
                docs.Add(pdfDoc1);
            }
            PdfDocument pdfDoc2 = new PdfDocument(new PdfWriter(destinationFolder + "copyPages5_4.pdf"));
            for (int i = 0; i < 3; i++) {
                pdfDoc2.AddPage(docs[i].GetPage(1).CopyTo(pdfDoc2));
            }
            pdfDoc2.Close();
            foreach (PdfDocument doc in docs) {
                doc.Close();
            }
            CompareTool cmpTool = new CompareTool();
            for (int i = 0; i < 3; i++) {
                PdfReader reader1 = new PdfReader(destinationFolder + MessageFormatUtil.Format("copyPages5_{0}.pdf", i + 1
                    ));
                PdfDocument doc1 = new PdfDocument(reader1);
                NUnit.Framework.Assert.AreEqual(false, reader1.HasRebuiltXref(), "Rebuilt");
                PdfReader reader2 = new PdfReader(destinationFolder + "copyPages5_4.pdf");
                PdfDocument doc2 = new PdfDocument(reader2);
                NUnit.Framework.Assert.AreEqual(false, reader2.HasRebuiltXref(), "Rebuilt");
                PdfDictionary page1 = doc1.GetPage(1).GetPdfObject();
                PdfDictionary page2 = doc2.GetPage(i + 1).GetPdfObject();
                NUnit.Framework.Assert.IsTrue(cmpTool.CompareDictionaries(page1, page2));
                doc1.Close();
                doc2.Close();
            }
        }

        [NUnit.Framework.Test]
        public virtual void CopyPagesTest6() {
            String file1 = destinationFolder + "copyPages6_1.pdf";
            String file2 = destinationFolder + "copyPages6_2.pdf";
            String file3 = destinationFolder + "copyPages6_3.pdf";
            String file1_upd = destinationFolder + "copyPages6_1_upd.pdf";
            PdfDocument pdfDoc1 = new PdfDocument(new PdfWriter(file1));
            PdfPage page1 = pdfDoc1.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page1);
            canvas.Rectangle(100, 600, 100, 100);
            canvas.Fill();
            canvas.BeginText();
            canvas.SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.COURIER), 12);
            canvas.SetTextMatrix(1, 0, 0, 1, 100, 500);
            canvas.ShowText("Hello World!");
            canvas.EndText();
            canvas.Release();
            pdfDoc1.Close();
            pdfDoc1 = new PdfDocument(new PdfReader(file1));
            PdfDocument pdfDoc2 = new PdfDocument(new PdfWriter(file2));
            pdfDoc2.AddPage(pdfDoc1.GetPage(1).CopyTo(pdfDoc2));
            pdfDoc2.Close();
            pdfDoc2 = new PdfDocument(new PdfReader(file2));
            PdfDocument pdfDoc3 = new PdfDocument(new PdfWriter(file3));
            pdfDoc3.AddPage(pdfDoc2.GetPage(1).CopyTo(pdfDoc3));
            pdfDoc3.Close();
            pdfDoc3 = new PdfDocument(new PdfReader(file3));
            pdfDoc1.Close();
            pdfDoc1 = new PdfDocument(new PdfReader(file1), new PdfWriter(file1_upd));
            pdfDoc1.AddPage(pdfDoc3.GetPage(1).CopyTo(pdfDoc1));
            pdfDoc1.Close();
            pdfDoc2.Close();
            pdfDoc3.Close();
            CompareTool cmpTool = new CompareTool();
            for (int i = 0; i < 3; i++) {
                PdfReader reader1 = new PdfReader(file1);
                PdfDocument doc1 = new PdfDocument(reader1);
                NUnit.Framework.Assert.AreEqual(false, reader1.HasRebuiltXref(), "Rebuilt");
                PdfReader reader2 = new PdfReader(file2);
                PdfDocument doc2 = new PdfDocument(reader2);
                NUnit.Framework.Assert.AreEqual(false, reader2.HasRebuiltXref(), "Rebuilt");
                PdfReader reader3 = new PdfReader(file3);
                PdfDocument doc3 = new PdfDocument(reader3);
                NUnit.Framework.Assert.AreEqual(false, reader3.HasRebuiltXref(), "Rebuilt");
                PdfReader reader4 = new PdfReader(file1_upd);
                PdfDocument doc4 = new PdfDocument(reader4);
                NUnit.Framework.Assert.AreEqual(false, reader4.HasRebuiltXref(), "Rebuilt");
                NUnit.Framework.Assert.IsTrue(cmpTool.CompareDictionaries(doc1.GetPage(1).GetPdfObject(), doc4.GetPage(2).
                    GetPdfObject()));
                NUnit.Framework.Assert.IsTrue(cmpTool.CompareDictionaries(doc4.GetPage(2).GetPdfObject(), doc2.GetPage(1).
                    GetPdfObject()));
                NUnit.Framework.Assert.IsTrue(cmpTool.CompareDictionaries(doc2.GetPage(1).GetPdfObject(), doc4.GetPage(1).
                    GetPdfObject()));
                doc1.Close();
                doc2.Close();
                doc3.Close();
                doc4.Close();
            }
        }

        [NUnit.Framework.Test]
        public virtual void MarkedContentTest1() {
            String message = "";
            PdfDocument document = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
            PdfPage page = document.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.BeginMarkedContent(new PdfName("Tag1"));
            canvas.EndMarkedContent();
            try {
                canvas.EndMarkedContent();
            }
            catch (PdfException e) {
                message = e.Message;
            }
            canvas.Release();
            document.Close();
            NUnit.Framework.Assert.AreEqual(PdfException.UnbalancedBeginEndMarkedContentOperators, message);
        }

        [NUnit.Framework.Test]
        public virtual void MarkedContentTest2() {
            PdfDocument document = new PdfDocument(new PdfWriter(destinationFolder + "markedContentTest2.pdf"));
            PdfPage page = document.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            Dictionary<PdfName, PdfObject> tmpMap = new Dictionary<PdfName, PdfObject>();
            tmpMap.Put(new PdfName("Tag"), new PdfNumber(2));
            PdfDictionary tag2 = new PdfDictionary(tmpMap);
            tmpMap = new Dictionary<PdfName, PdfObject>();
            tmpMap.Put(new PdfName("Tag"), new PdfNumber(3).MakeIndirect(document));
            PdfDictionary tag3 = new PdfDictionary(tmpMap);
            canvas.BeginMarkedContent(new PdfName("Tag1")).EndMarkedContent().BeginMarkedContent(new PdfName("Tag2"), 
                tag2).EndMarkedContent().BeginMarkedContent(new PdfName("Tag3"), (PdfDictionary)tag3.MakeIndirect(document
                )).EndMarkedContent();
            canvas.Release();
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + "markedContentTest2.pdf"
                , sourceFolder + "cmp_markedContentTest2.pdf", destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void GraphicsStateTest1() {
            PdfDocument document = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
            PdfPage page = document.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.SetLineWidth(3);
            canvas.SaveState();
            canvas.SetLineWidth(5);
            NUnit.Framework.Assert.AreEqual(5, canvas.GetGraphicsState().GetLineWidth(), 0);
            canvas.RestoreState();
            NUnit.Framework.Assert.AreEqual(3, canvas.GetGraphicsState().GetLineWidth(), 0);
            PdfExtGState egs = new PdfExtGState();
            egs.GetPdfObject().Put(PdfName.LW, new PdfNumber(2));
            canvas.SetExtGState(egs);
            NUnit.Framework.Assert.AreEqual(2, canvas.GetGraphicsState().GetLineWidth(), 0);
            canvas.Release();
            document.Close();
        }

        [NUnit.Framework.Test]
        public virtual void WmfImageTest01() {
            PdfDocument document = new PdfDocument(new PdfWriter(destinationFolder + "wmfImageTest01.pdf"));
            PdfPage page = document.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            ImageData img = new WmfImageData(sourceFolder + "example.wmf");
            canvas.AddImage(img, 0, 0, 0.1f, false);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + "wmfImageTest01.pdf", 
                sourceFolder + "cmp_wmfImageTest01.pdf", destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void WmfImageTest02() {
            PdfDocument document = new PdfDocument(new PdfWriter(destinationFolder + "wmfImageTest02.pdf"));
            PdfPage page = document.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            ImageData img = new WmfImageData(sourceFolder + "butterfly.wmf");
            canvas.AddImage(img, 0, 0, 1, false);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + "wmfImageTest02.pdf", 
                sourceFolder + "cmp_wmfImageTest02.pdf", destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void WmfImageTest03() {
            PdfDocument document = new PdfDocument(new PdfWriter(destinationFolder + "wmfImageTest03.pdf"));
            PdfPage page = document.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            ImageData img = new WmfImageData(sourceFolder + "type1.wmf");
            canvas.AddImage(img, 0, 0, 1, false);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + "wmfImageTest03.pdf", 
                sourceFolder + "cmp_wmfImageTest03.pdf", destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void WmfImageTest04() {
            PdfDocument document = new PdfDocument(new PdfWriter(destinationFolder + "wmfImageTest04.pdf"));
            PdfPage page = document.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            ImageData img = new WmfImageData(sourceFolder + "type0.wmf");
            canvas.AddImage(img, 0, 0, 1, false);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + "wmfImageTest04.pdf", 
                sourceFolder + "cmp_wmfImageTest04.pdf", destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void WmfImageTest05() {
            PdfDocument document = new PdfDocument(new PdfWriter(destinationFolder + "wmfImageTest05.pdf"));
            PdfPage page = document.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            Stream stream = UrlUtil.OpenStream(UrlUtil.ToURL(sourceFolder + "example2.wmf"));
            ByteArrayOutputStream baos = new ByteArrayOutputStream();
            StreamUtil.TransferBytes(stream, baos);
            ImageData img = new WmfImageData(baos.ToArray());
            canvas.AddImage(img, 0, 0, 1, false);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + "wmfImageTest05.pdf", 
                sourceFolder + "cmp_wmfImageTest05.pdf", destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void GifImageTest01() {
            PdfDocument document = new PdfDocument(new PdfWriter(destinationFolder + "gifImageTest01.pdf"));
            PdfPage page = document.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            ImageData img = ImageDataFactory.Create(sourceFolder + "2-frames.gif");
            canvas.AddImage(img, 100, 100, 200, false);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + "gifImageTest01.pdf", 
                sourceFolder + "cmp_gifImageTest01.pdf", destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void GifImageTest02() {
            PdfDocument document = new PdfDocument(new PdfWriter(destinationFolder + "gifImageTest02.pdf"));
            PdfPage page = document.AddNewPage();
            Stream @is = new FileStream(sourceFolder + "2-frames.gif", FileMode.Open, FileAccess.Read);
            ByteArrayOutputStream baos = new ByteArrayOutputStream();
            int reads = @is.Read();
            while (reads != -1) {
                baos.Write(reads);
                reads = @is.Read();
            }
            PdfCanvas canvas = new PdfCanvas(page);
            ImageData img = ImageDataFactory.CreateGifFrame(baos.ToArray(), 1);
            canvas.AddImage(img, 100, 100, 200, false);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + "gifImageTest02.pdf", 
                sourceFolder + "cmp_gifImageTest02.pdf", destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void GifImageTest03() {
            PdfDocument document = new PdfDocument(new PdfWriter(destinationFolder + "gifImageTest03.pdf"));
            PdfPage page = document.AddNewPage();
            Stream @is = new FileStream(sourceFolder + "2-frames.gif", FileMode.Open, FileAccess.Read);
            ByteArrayOutputStream baos = new ByteArrayOutputStream();
            int reads = @is.Read();
            while (reads != -1) {
                baos.Write(reads);
                reads = @is.Read();
            }
            PdfCanvas canvas = new PdfCanvas(page);
            ImageData img = ImageDataFactory.CreateGifFrame(baos.ToArray(), 2);
            canvas.AddImage(img, 100, 100, 200, false);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + "gifImageTest03.pdf", 
                sourceFolder + "cmp_gifImageTest03.pdf", destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void GifImageTest04() {
            PdfDocument document = new PdfDocument(new PdfWriter(destinationFolder + "gifImageTest04.pdf"));
            PdfPage page = document.AddNewPage();
            Stream @is = new FileStream(sourceFolder + "2-frames.gif", FileMode.Open, FileAccess.Read);
            ByteArrayOutputStream baos = new ByteArrayOutputStream();
            int reads = @is.Read();
            while (reads != -1) {
                baos.Write(reads);
                reads = @is.Read();
            }
            PdfCanvas canvas = new PdfCanvas(page);
            try {
                ImageDataFactory.CreateGifFrame(baos.ToArray(), 3);
                NUnit.Framework.Assert.Fail("IOException expected");
            }
            catch (iText.IO.IOException) {
            }
        }

        [NUnit.Framework.Test]
        public virtual void GifImageTest05() {
            PdfDocument document = new PdfDocument(new PdfWriter(destinationFolder + "gifImageTest05.pdf"));
            PdfPage page = document.AddNewPage();
            Stream @is = new FileStream(sourceFolder + "animated_fox_dog.gif", FileMode.Open, FileAccess.Read);
            ByteArrayOutputStream baos = new ByteArrayOutputStream();
            int reads = @is.Read();
            while (reads != -1) {
                baos.Write(reads);
                reads = @is.Read();
            }
            PdfCanvas canvas = new PdfCanvas(page);
            IList<ImageData> frames = ImageDataFactory.CreateGifFrames(baos.ToArray(), new int[] { 1, 2, 5 });
            float y = 600;
            foreach (ImageData img in frames) {
                canvas.AddImage(img, 100, y, 200, false);
                y -= 200;
            }
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + "gifImageTest05.pdf", 
                sourceFolder + "cmp_gifImageTest05.pdf", destinationFolder, "diff_"));
        }

        //    @Test
        //    public void kernedTextTest01() throws IOException, InterruptedException {
        //        FileOutputStream fos = new FileOutputStream(destinationFolder + "kernedTextTest01.pdf");
        //        PdfWriter writer = new PdfWriter(fos);
        //        PdfDocument document = new PdfDocument(writer);
        //        PdfPage page = document.addNewPage();
        //
        //        PdfCanvas canvas = new PdfCanvas(page);
        //        String kernableText = "AVAVAVAVAVAVAVAVAVAVAVAVAVAVAVAVAVAVAVAVAVAVAVAVAVAVAVAVAVAV";
        //        PdfFont font = PdfFont.createFont(document, StandardFonts.HELVETICA);
        //        canvas.beginText().moveText(50, 600).setFontAndSize(font, 12).showText("Kerning:-" + kernableText).endText();
        //        canvas.beginText().moveText(50, 650).setFontAndSize(font, 12).showTextKerned("Kerning:+" + kernableText).endText();
        //
        //        document.close();
        //
        //        Assert.assertNull(new CompareTool().compareByContent(destinationFolder + "kernedTextTest01.pdf", sourceFolder + "cmp_kernedTextTest01.pdf", destinationFolder, "diff_"));
        //    }
        /*@Test
        public void ccittImageTest01() throws IOException, InterruptedException {
        String filename = "ccittImage01.pdf";
        PdfWriter writer = new PdfWriter(destinationFolder + filename);
        PdfDocument document = new PdfDocument(writer);
        
        PdfPage page = document.addNewPage();
        PdfCanvas canvas = new PdfCanvas(page);
        
        String text = "Call me Ishmael. Some years ago--never mind how long "
        + "precisely --having little or no money in my purse, and nothing "
        + "particular to interest me on shore, I thought I would sail about "
        + "a little and see the watery part of the world.";
        
        BarcodePDF417 barcode = new BarcodePDF417();
        barcode.setText(text);
        barcode.paintCode();
        
        byte g4[] = CCITTG4Encoder.compress(barcode.getOutBits(), barcode.getBitColumns(), barcode.getCodeRows());
        RawImage img = (RawImage) ImageDataFactory.create(barcode.getBitColumns(), barcode.getCodeRows(), false, RawImage.CCITTG4, 0, g4, null);
        img.setTypeCcitt(RawImage.CCITTG4);
        canvas.addImage(img, 100, 100, false);
        
        document.close();
        
        Assert.assertNull(new CompareTool().compareByContent(destinationFolder + filename, sourceFolder + "cmp_" + filename, destinationFolder, "diff_"));
        }*/
        [NUnit.Framework.Test]
        public virtual void CanvasInitializationPageNoContentsKey() {
            String srcFile = sourceFolder + "pageNoContents.pdf";
            String cmpFile = sourceFolder + "cmp_pageNoContentsStamp.pdf";
            String destFile = destinationFolder + "pageNoContentsStamp.pdf";
            PdfDocument document = new PdfDocument(new PdfReader(srcFile), new PdfWriter(destFile));
            PdfCanvas canvas = new PdfCanvas(document.GetPage(1));
            canvas.SetLineWidth(5).Rectangle(50, 680, 300, 50).Stroke();
            canvas.Release();
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destFile, cmpFile, destinationFolder, "diff_"
                ));
        }

        [NUnit.Framework.Test]
        public virtual void CanvasInitializationStampingExistingStream() {
            String srcFile = sourceFolder + "pageWithContent.pdf";
            String cmpFile = sourceFolder + "cmp_stampingExistingStream.pdf";
            String destFile = destinationFolder + "stampingExistingStream.pdf";
            PdfDocument document = new PdfDocument(new PdfReader(srcFile), new PdfWriter(destFile));
            PdfPage page = document.GetPage(1);
            PdfCanvas canvas = new PdfCanvas(page.GetLastContentStream(), page.GetResources(), page.GetDocument());
            canvas.SetLineWidth(5).Rectangle(50, 680, 300, 50).Stroke();
            canvas.Release();
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destFile, cmpFile, destinationFolder, "diff_"
                ));
        }

        [NUnit.Framework.Test]
        public virtual void CanvasStampingJustCopiedStreamWithCompression() {
            String srcFile = sourceFolder + "pageWithContent.pdf";
            String cmpFile = sourceFolder + "cmp_stampingJustCopiedStreamWithCompression.pdf";
            String destFile = destinationFolder + "stampingJustCopiedStreamWithCompression.pdf";
            PdfDocument srcDocument = new PdfDocument(new PdfReader(srcFile));
            PdfDocument document = new PdfDocument(new PdfWriter(destFile));
            srcDocument.CopyPagesTo(1, 1, document);
            srcDocument.Close();
            PdfPage page = document.GetPage(1);
            PdfCanvas canvas = new PdfCanvas(page.GetLastContentStream(), page.GetResources(), page.GetDocument());
            canvas.SetLineWidth(5).Rectangle(50, 680, 300, 50).Stroke();
            canvas.Release();
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destFile, cmpFile, destinationFolder, "diff_"
                ));
        }

        [NUnit.Framework.Test]
        public virtual void CanvasSmallFontSize01() {
            String cmpFile = sourceFolder + "cmp_canvasSmallFontSize01.pdf";
            String destFile = destinationFolder + "canvasSmallFontSize01.pdf";
            PdfDocument document = new PdfDocument(new PdfWriter(destFile));
            PdfPage page = document.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.SaveState().BeginText().MoveText(50, 750).SetFontAndSize(PdfFontFactory.CreateFont(), 0).ShowText("simple text"
                ).EndText().RestoreState();
            canvas.SaveState().BeginText().MoveText(50, 700).SetFontAndSize(PdfFontFactory.CreateFont(), -0.00005f).ShowText
                ("simple text").EndText().RestoreState();
            canvas.SaveState().BeginText().MoveText(50, 650).SetFontAndSize(PdfFontFactory.CreateFont(), 0.00005f).ShowText
                ("simple text").EndText().RestoreState();
            canvas.SaveState().BeginText().MoveText(50, 600).SetFontAndSize(PdfFontFactory.CreateFont(), -12).ShowText
                ("simple text").EndText().RestoreState();
            canvas.SaveState().BeginText().MoveText(50, 550).SetFontAndSize(PdfFontFactory.CreateFont(), 12).ShowText(
                "simple text").EndText().RestoreState();
            canvas.Release();
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destFile, cmpFile, destinationFolder, "diff_"
                ));
        }

        [NUnit.Framework.Test]
        public virtual void EndPathNewPathTest() {
            ByteArrayOutputStream boasEndPath = new ByteArrayOutputStream();
            PdfDocument pdfDocEndPath = new PdfDocument(new PdfWriter(boasEndPath));
            pdfDocEndPath.AddNewPage();
            PdfCanvas endPathCanvas = new PdfCanvas(pdfDocEndPath.GetPage(1));
            endPathCanvas.EndPath();
            ByteArrayOutputStream boasNewPath = new ByteArrayOutputStream();
            PdfDocument pdfDocNewPath = new PdfDocument(new PdfWriter(boasNewPath));
            pdfDocNewPath.AddNewPage();
            PdfCanvas newPathCanvas = new PdfCanvas(pdfDocNewPath.GetPage(1));
            newPathCanvas.NewPath();
            NUnit.Framework.Assert.AreEqual(boasNewPath.ToArray(), boasEndPath.ToArray());
        }
    }
}
