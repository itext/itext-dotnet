using System;
using System.Collections.Generic;
using System.IO;
using iText.IO.Font;
using iText.Kernel;
using iText.Kernel.Font;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Tagging;
using iText.Kernel.Pdf.Tagutils;
using iText.Kernel.Utils;
using iText.Test;

namespace iText.Kernel.Pdf {
    public class TagTreePointerTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/pdf/TagTreePointerTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/kernel/pdf/TagTreePointerTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(destinationFolder);
        }

        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TagTreePointerTest01() {
            FileStream fos = new FileStream(destinationFolder + "tagTreePointerTest01.pdf", FileMode.Create);
            PdfWriter writer = new PdfWriter(fos).SetCompressionLevel(CompressionConstants.NO_COMPRESSION);
            PdfDocument document = new PdfDocument(writer);
            document.SetTagged();
            PdfPage page1 = document.AddNewPage();
            TagTreePointer tagPointer = new TagTreePointer(document);
            tagPointer.SetPageForTagging(page1);
            PdfCanvas canvas = new PdfCanvas(page1);
            PdfFont standardFont = PdfFontFactory.CreateFont(FontConstants.COURIER);
            canvas.BeginText().SetFontAndSize(standardFont, 24).SetTextMatrix(1, 0, 0, 1, 32, 512);
            tagPointer.AddTag(PdfName.P).AddTag(PdfName.Span);
            canvas.OpenTag(tagPointer.GetTagReference()).ShowText("Hello ").CloseTag();
            canvas.SetFontAndSize(standardFont, 30).OpenTag(tagPointer.GetTagReference()).ShowText("World").CloseTag();
            tagPointer.MoveToParent().MoveToParent();
            canvas.EndText().Release();
            PdfPage page2 = document.AddNewPage();
            tagPointer.SetPageForTagging(page2);
            canvas = new PdfCanvas(page2);
            canvas.BeginText().SetFontAndSize(PdfFontFactory.CreateFont(FontConstants.HELVETICA), 24).SetTextMatrix(1, 
                0, 0, 1, 32, 512);
            tagPointer.AddTag(PdfName.P).AddTag(PdfName.Span);
            canvas.OpenTag(tagPointer.GetTagReference()).ShowText("Hello ").CloseTag();
            tagPointer.MoveToParent().AddTag(PdfName.Span);
            canvas.OpenTag(tagPointer.GetTagReference()).ShowText("World").CloseTag();
            canvas.EndText().Release();
            page1.Flush();
            page2.Flush();
            document.Close();
            CompareResult("tagTreePointerTest01.pdf", "cmp_tagTreePointerTest01.pdf", "diff01_");
        }

        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TagTreePointerTest02() {
            FileStream fos = new FileStream(destinationFolder + "tagTreePointerTest02.pdf", FileMode.Create);
            PdfWriter writer = new PdfWriter(fos);
            writer.SetCompressionLevel(CompressionConstants.NO_COMPRESSION);
            PdfDocument document = new PdfDocument(writer);
            document.SetTagged();
            PdfPage page = document.AddNewPage();
            TagTreePointer tagPointer = new TagTreePointer(document);
            tagPointer.SetPageForTagging(page);
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.BeginText();
            PdfFont standardFont = PdfFontFactory.CreateFont(FontConstants.COURIER);
            canvas.SetFontAndSize(standardFont, 24).SetTextMatrix(1, 0, 0, 1, 32, 512);
            PdfDictionary attributes = new PdfDictionary();
            attributes.Put(PdfName.O, new PdfString("random attributes"));
            attributes.Put(new PdfName("hello"), new PdfString("world"));
            tagPointer.AddTag(PdfName.P).AddTag(PdfName.Span).GetProperties().SetActualText("Actual text for span is: Hello World"
                ).SetLanguage("en-GB").AddAttributes(attributes);
            canvas.OpenTag(tagPointer.GetTagReference()).ShowText("Hello ").CloseTag();
            canvas.SetFontAndSize(standardFont, 30).OpenTag(tagPointer.GetTagReference()).ShowText("World").CloseTag();
            canvas.EndText().Release();
            page.Flush();
            document.Close();
            CompareResult("tagTreePointerTest02.pdf", "cmp_tagTreePointerTest02.pdf", "diff02_");
        }

        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TagTreePointerTest03() {
            PdfReader reader = new PdfReader(sourceFolder + "taggedDocument.pdf");
            PdfWriter writer = new PdfWriter(destinationFolder + "tagTreePointerTest03.pdf");
            writer.SetCompressionLevel(CompressionConstants.NO_COMPRESSION);
            PdfDocument document = new PdfDocument(reader, writer);
            TagTreePointer tagPointer = new TagTreePointer(document);
            tagPointer.MoveToKid(PdfName.Table).MoveToKid(2, PdfName.TR);
            TagTreePointer tagPointerCopy = new TagTreePointer(tagPointer);
            tagPointer.RemoveTag();
            // tagPointerCopy now points at removed tag
            String exceptionMessage = null;
            try {
                tagPointerCopy.AddTag(PdfName.Span);
            }
            catch (PdfException e) {
                exceptionMessage = e.Message;
            }
            NUnit.Framework.Assert.AreEqual(PdfException.TagTreePointerIsInInvalidStateItPointsAtRemovedElementUseMoveToRoot
                , exceptionMessage);
            tagPointerCopy.MoveToRoot().MoveToKid(PdfName.Table);
            tagPointerCopy.MoveToKid(PdfName.TR);
            TagTreePointer tagPointerCopyCopy = new TagTreePointer(tagPointerCopy);
            tagPointerCopy.FlushTag();
            // tagPointerCopyCopy now points at flushed tag
            try {
                tagPointerCopyCopy.AddTag(PdfName.Span);
            }
            catch (PdfException e) {
                exceptionMessage = e.Message;
            }
            NUnit.Framework.Assert.AreEqual(PdfException.TagTreePointerIsInInvalidStateItPointsAtFlushedElementUseMoveToRoot
                , exceptionMessage);
            try {
                tagPointerCopy.MoveToKid(0);
            }
            catch (PdfException e) {
                exceptionMessage = e.Message;
            }
            NUnit.Framework.Assert.AreEqual(PdfException.CannotMoveToFlushedKid, exceptionMessage);
            document.Close();
        }

        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TagTreePointerTest04() {
            PdfReader reader = new PdfReader(sourceFolder + "taggedDocument.pdf");
            PdfWriter writer = new PdfWriter(destinationFolder + "tagTreePointerTest04.pdf").SetCompressionLevel(CompressionConstants
                .NO_COMPRESSION);
            PdfDocument document = new PdfDocument(reader, writer);
            TagTreePointer tagPointer = new TagTreePointer(document);
            tagPointer.MoveToKid(PdfName.Table).MoveToKid(2, PdfName.TR);
            tagPointer.RemoveTag();
            tagPointer.MoveToKid(PdfName.TR).MoveToKid(PdfName.TD).MoveToKid(PdfName.P).MoveToKid(PdfName.Span);
            tagPointer.RemoveTag().RemoveTag();
            document.Close();
            CompareResult("tagTreePointerTest04.pdf", "cmp_tagTreePointerTest04.pdf", "diff04_");
        }

        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TagTreePointerTest05() {
            PdfReader reader = new PdfReader(sourceFolder + "taggedDocument.pdf");
            PdfWriter writer = new PdfWriter(destinationFolder + "tagTreePointerTest05.pdf");
            writer.SetCompressionLevel(CompressionConstants.NO_COMPRESSION);
            PdfDocument document = new PdfDocument(reader, writer);
            TagTreePointer tagPointer1 = new TagTreePointer(document);
            tagPointer1.MoveToKid(PdfName.Table).MoveToKid(2, PdfName.TR);
            TagTreePointer tagPointer2 = new TagTreePointer(document);
            tagPointer2.MoveToKid(PdfName.Table).MoveToKid(0, PdfName.TR);
            tagPointer1.RelocateKid(0, tagPointer2);
            tagPointer1.MoveToParent().MoveToKid(5, PdfName.TR).MoveToKid(2, PdfName.TD).MoveToKid(PdfName.P).MoveToKid
                (PdfName.Span);
            tagPointer2.MoveToKid(PdfName.TD).MoveToKid(PdfName.P).MoveToKid(PdfName.Span);
            tagPointer2.SetNextNewKidIndex(3);
            tagPointer1.RelocateKid(4, tagPointer2);
            document.Close();
            CompareResult("tagTreePointerTest05.pdf", "cmp_tagTreePointerTest05.pdf", "diff05_");
        }

        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void TagTreePointerTest06() {
            PdfReader reader = new PdfReader(sourceFolder + "taggedDocument.pdf");
            PdfWriter writer = new PdfWriter(destinationFolder + "tagTreePointerTest06.pdf");
            writer.SetCompressionLevel(CompressionConstants.NO_COMPRESSION);
            PdfDocument document = new PdfDocument(reader, writer);
            TagTreePointer tagPointer = new TagTreePointer(document);
            tagPointer.SetRole(PdfName.Part);
            NUnit.Framework.Assert.AreEqual(tagPointer.GetRole().GetValue(), "Part");
            tagPointer.MoveToKid(PdfName.Table).GetProperties().SetLanguage("en-US");
            tagPointer.MoveToKid(PdfName.TR).MoveToKid(PdfName.TD).MoveToKid(PdfName.P);
            String actualText1 = "Some looong latin text";
            tagPointer.GetProperties().SetActualText(actualText1);
            NUnit.Framework.Assert.IsNull(tagPointer.GetConnectedElement(false));
            IAccessibleElement connectedElement = tagPointer.GetConnectedElement(true);
            tagPointer.MoveToRoot().MoveToKid(PdfName.Table).MoveToKid(1, PdfName.TR).GetProperties().SetActualText("More latin text"
                );
            connectedElement.SetRole(PdfName.Div);
            connectedElement.GetAccessibilityProperties().SetLanguage("en-Us");
            NUnit.Framework.Assert.AreEqual(connectedElement.GetAccessibilityProperties().GetActualText(), actualText1
                );
            document.Close();
            CompareResult("tagTreePointerTest06.pdf", "cmp_tagTreePointerTest06.pdf", "diff06_");
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        /// <exception cref="Org.Xml.Sax.SAXException"/>
        /// <exception cref="Javax.Xml.Parsers.ParserConfigurationException"/>
        [NUnit.Framework.Test]
        public virtual void TagStructureFlushingTest01() {
            PdfReader reader = new PdfReader(sourceFolder + "taggedDocument.pdf");
            PdfWriter writer = new PdfWriter(destinationFolder + "tagStructureFlushingTest01.pdf");
            writer.SetCompressionLevel(CompressionConstants.NO_COMPRESSION);
            PdfDocument document = new PdfDocument(reader, writer);
            TagTreePointer tagPointer = new TagTreePointer(document);
            tagPointer.MoveToKid(PdfName.Table).MoveToKid(2, PdfName.TR).FlushTag();
            tagPointer.MoveToKid(3, PdfName.TR).MoveToKid(PdfName.TD).FlushTag();
            tagPointer.MoveToParent().FlushTag();
            String exceptionMessage = null;
            try {
                tagPointer.FlushTag();
            }
            catch (PdfException e) {
                exceptionMessage = e.Message;
            }
            document.Close();
            NUnit.Framework.Assert.AreEqual(PdfException.CannotFlushDocumentRootTagBeforeDocumentIsClosed, exceptionMessage
                );
            CompareResult("tagStructureFlushingTest01.pdf", "taggedDocument.pdf", "diffFlushing01_");
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        /// <exception cref="Org.Xml.Sax.SAXException"/>
        /// <exception cref="Javax.Xml.Parsers.ParserConfigurationException"/>
        [NUnit.Framework.Test]
        public virtual void TagStructureFlushingTest02() {
            PdfReader reader = new PdfReader(sourceFolder + "taggedDocument.pdf");
            PdfWriter writer = new PdfWriter(destinationFolder + "tagStructureFlushingTest02.pdf");
            writer.SetCompressionLevel(CompressionConstants.NO_COMPRESSION);
            PdfDocument document = new PdfDocument(reader, writer);
            TagStructureContext tagStructure = document.GetTagStructureContext();
            tagStructure.FlushPageTags(document.GetPage(1));
            IList<IPdfStructElem> kids = document.GetStructTreeRoot().GetKids();
            NUnit.Framework.Assert.IsTrue(!((PdfStructElem)kids[0]).GetPdfObject().IsFlushed());
            NUnit.Framework.Assert.IsTrue(!((PdfStructElem)kids[0].GetKids()[0]).GetPdfObject().IsFlushed());
            PdfArray rowsTags = (PdfArray)((PdfStructElem)kids[0].GetKids()[0]).GetK();
            NUnit.Framework.Assert.IsTrue(rowsTags.Get(0).IsFlushed());
            document.Close();
            CompareResult("tagStructureFlushingTest02.pdf", "taggedDocument.pdf", "diffFlushing02_");
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        /// <exception cref="Org.Xml.Sax.SAXException"/>
        /// <exception cref="Javax.Xml.Parsers.ParserConfigurationException"/>
        [NUnit.Framework.Test]
        public virtual void TagStructureFlushingTest03() {
            PdfReader reader = new PdfReader(sourceFolder + "taggedDocument.pdf");
            PdfWriter writer = new PdfWriter(destinationFolder + "tagStructureFlushingTest03.pdf");
            writer.SetCompressionLevel(CompressionConstants.NO_COMPRESSION);
            PdfDocument document = new PdfDocument(reader, writer);
            document.GetPage(2).Flush();
            document.GetPage(1).Flush();
            PdfArray kids = document.GetStructTreeRoot().GetKidsObject();
            NUnit.Framework.Assert.IsFalse(kids.Get(0).IsFlushed());
            NUnit.Framework.Assert.IsTrue(kids.GetAsDictionary(0).GetAsDictionary(PdfName.K).IsFlushed());
            document.Close();
            CompareResult("tagStructureFlushingTest03.pdf", "taggedDocument.pdf", "diffFlushing03_");
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        /// <exception cref="Org.Xml.Sax.SAXException"/>
        /// <exception cref="Javax.Xml.Parsers.ParserConfigurationException"/>
        [NUnit.Framework.Test]
        public virtual void TagStructureFlushingTest04() {
            PdfReader reader = new PdfReader(sourceFolder + "taggedDocument.pdf");
            PdfWriter writer = new PdfWriter(destinationFolder + "tagStructureFlushingTest04.pdf");
            writer.SetCompressionLevel(CompressionConstants.NO_COMPRESSION);
            PdfDocument document = new PdfDocument(reader, writer);
            TagTreePointer tagPointer = new TagTreePointer(document);
            tagPointer.MoveToKid(PdfName.Table).MoveToKid(2, PdfName.TR).FlushTag();
            // intended redundant call to flush page tags separately from page. Page tags are flushed when the page is flushed.
            document.GetTagStructureContext().FlushPageTags(document.GetPage(1));
            document.GetPage(1).Flush();
            tagPointer.MoveToKid(5).FlushTag();
            document.GetPage(2).Flush();
            PdfArray kids = document.GetStructTreeRoot().GetKidsObject();
            NUnit.Framework.Assert.IsFalse(kids.Get(0).IsFlushed());
            NUnit.Framework.Assert.IsTrue(kids.GetAsDictionary(0).GetAsDictionary(PdfName.K).IsFlushed());
            document.Close();
            CompareResult("tagStructureFlushingTest04.pdf", "taggedDocument.pdf", "diffFlushing04_");
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        /// <exception cref="Org.Xml.Sax.SAXException"/>
        /// <exception cref="Javax.Xml.Parsers.ParserConfigurationException"/>
        [NUnit.Framework.Test]
        public virtual void TagStructureFlushingTest05() {
            PdfWriter writer = new PdfWriter(destinationFolder + "tagStructureFlushingTest05.pdf");
            writer.SetCompressionLevel(CompressionConstants.NO_COMPRESSION);
            PdfDocument document = new PdfDocument(writer);
            document.SetTagged();
            PdfPage page1 = document.AddNewPage();
            TagTreePointer tagPointer = new TagTreePointer(document);
            tagPointer.SetPageForTagging(page1);
            PdfCanvas canvas = new PdfCanvas(page1);
            tagPointer.AddTag(PdfName.Div);
            tagPointer.AddTag(PdfName.P);
            IAccessibleElement paragraphElement = tagPointer.GetConnectedElement(true);
            canvas.BeginText();
            PdfFont standardFont = PdfFontFactory.CreateFont(FontConstants.COURIER);
            canvas.SetFontAndSize(standardFont, 24).SetTextMatrix(1, 0, 0, 1, 32, 512);
            tagPointer.AddTag(PdfName.Span);
            canvas.OpenTag(tagPointer.GetTagReference()).ShowText("Hello ").CloseTag();
            canvas.SetFontAndSize(standardFont, 30).OpenTag(tagPointer.GetTagReference()).ShowText("World").CloseTag();
            tagPointer.MoveToParent().MoveToParent();
            // Flushing /Div tag and it's children. /P tag shall not be flushed, as it is has connected paragraphElement
            // object. On removing connection between paragraphElement and /P tag, /P tag shall be flushed.
            // When tag is flushed, tagPointer begins to point to tag's parent. If parent is also flushed - to the root.
            tagPointer.FlushTag();
            tagPointer.MoveToTag(paragraphElement);
            tagPointer.AddTag(PdfName.Span);
            canvas.OpenTag(tagPointer.GetTagReference()).ShowText("Hello ").CloseTag();
            canvas.SetFontAndSize(standardFont, 30).OpenTag(tagPointer.GetTagReference()).ShowText("again").CloseTag();
            tagPointer.RemoveElementConnectionToTag(paragraphElement);
            tagPointer.MoveToRoot();
            canvas.EndText().Release();
            PdfPage page2 = document.AddNewPage();
            tagPointer.SetPageForTagging(page2);
            canvas = new PdfCanvas(page2);
            tagPointer.AddTag(PdfName.P);
            canvas.BeginText().SetFontAndSize(PdfFontFactory.CreateFont(FontConstants.HELVETICA), 24).SetTextMatrix(1, 
                0, 0, 1, 32, 512);
            tagPointer.AddTag(PdfName.Span);
            canvas.OpenTag(tagPointer.GetTagReference()).ShowText("Hello ").CloseTag();
            tagPointer.MoveToParent().AddTag(PdfName.Span);
            canvas.OpenTag(tagPointer.GetTagReference()).ShowText("World").CloseTag();
            canvas.EndText().Release();
            page1.Flush();
            page2.Flush();
            document.Close();
            CompareResult("tagStructureFlushingTest05.pdf", "cmp_tagStructureFlushingTest05.pdf", "diffFlushing05_");
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        /// <exception cref="Org.Xml.Sax.SAXException"/>
        /// <exception cref="Javax.Xml.Parsers.ParserConfigurationException"/>
        [NUnit.Framework.Test]
        public virtual void TagStructureRemovingTest01() {
            PdfReader reader = new PdfReader(sourceFolder + "taggedDocument.pdf");
            PdfWriter writer = new PdfWriter(destinationFolder + "tagStructureRemovingTest01.pdf");
            writer.SetCompressionLevel(CompressionConstants.NO_COMPRESSION);
            PdfDocument document = new PdfDocument(reader, writer);
            document.RemovePage(1);
            document.Close();
            CompareResult("tagStructureRemovingTest01.pdf", "cmp_tagStructureRemovingTest01.pdf", "diffRemoving01_");
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        /// <exception cref="Org.Xml.Sax.SAXException"/>
        /// <exception cref="Javax.Xml.Parsers.ParserConfigurationException"/>
        [NUnit.Framework.Test]
        public virtual void TagStructureRemovingTest02() {
            PdfReader reader = new PdfReader(sourceFolder + "taggedDocument.pdf");
            PdfWriter writer = new PdfWriter(destinationFolder + "tagStructureRemovingTest02.pdf");
            writer.SetCompressionLevel(CompressionConstants.NO_COMPRESSION);
            PdfDocument document = new PdfDocument(reader, writer);
            PdfPage firstPage = document.GetPage(1);
            PdfPage secondPage = document.GetPage(2);
            document.RemovePage(firstPage);
            document.RemovePage(secondPage);
            PdfPage page = document.AddNewPage();
            TagTreePointer tagPointer = new TagTreePointer(document);
            tagPointer.SetPageForTagging(page);
            PdfCanvas canvas = new PdfCanvas(page);
            tagPointer.AddTag(PdfName.P);
            PdfFont standardFont = PdfFontFactory.CreateFont(FontConstants.COURIER);
            canvas.BeginText().SetFontAndSize(standardFont, 24).SetTextMatrix(1, 0, 0, 1, 32, 512);
            tagPointer.AddTag(PdfName.Span);
            canvas.OpenTag(tagPointer.GetTagReference()).ShowText("Hello ").CloseTag();
            canvas.SetFontAndSize(standardFont, 30).OpenTag(tagPointer.GetTagReference()).ShowText("World").CloseTag()
                .EndText();
            document.Close();
            CompareResult("tagStructureRemovingTest02.pdf", "cmp_tagStructureRemovingTest02.pdf", "diffRemoving02_");
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        /// <exception cref="Org.Xml.Sax.SAXException"/>
        /// <exception cref="Javax.Xml.Parsers.ParserConfigurationException"/>
        [NUnit.Framework.Test]
        public virtual void TagStructureRemovingTest03() {
            PdfWriter writer = new PdfWriter(destinationFolder + "tagStructureRemovingTest03.pdf");
            writer.SetCompressionLevel(CompressionConstants.NO_COMPRESSION);
            PdfDocument document = new PdfDocument(writer);
            document.SetTagged();
            PdfPage page = document.AddNewPage();
            TagTreePointer tagPointer = new TagTreePointer(document);
            tagPointer.SetPageForTagging(page);
            PdfCanvas canvas = new PdfCanvas(page);
            tagPointer.AddTag(PdfName.P);
            IAccessibleElement paragraphElement = tagPointer.GetConnectedElement(true);
            PdfFont standardFont = PdfFontFactory.CreateFont(FontConstants.COURIER);
            canvas.BeginText().SetFontAndSize(standardFont, 24).SetTextMatrix(1, 0, 0, 1, 32, 512);
            tagPointer.AddTag(PdfName.Span);
            canvas.OpenTag(tagPointer.GetTagReference()).ShowText("Hello ").CloseTag();
            canvas.SetFontAndSize(standardFont, 30).OpenTag(tagPointer.GetTagReference()).ShowText("World").CloseTag()
                .EndText();
            tagPointer.MoveToParent().MoveToParent();
            document.RemovePage(1);
            PdfPage newPage = document.AddNewPage();
            canvas = new PdfCanvas(newPage);
            tagPointer.SetPageForTagging(newPage);
            tagPointer.MoveToTag(paragraphElement).AddTag(PdfName.Span);
            canvas.OpenTag(tagPointer.GetTagReference()).BeginText().SetFontAndSize(standardFont, 24).SetTextMatrix(1, 
                0, 0, 1, 32, 512).ShowText("Hello.").EndText().CloseTag();
            document.Close();
            CompareResult("tagStructureRemovingTest03.pdf", "cmp_tagStructureRemovingTest03.pdf", "diffRemoving03_");
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        /// <exception cref="Org.Xml.Sax.SAXException"/>
        /// <exception cref="Javax.Xml.Parsers.ParserConfigurationException"/>
        [NUnit.Framework.Test]
        public virtual void TagStructureRemovingTest04() {
            PdfReader reader = new PdfReader(sourceFolder + "taggedDocumentWithAnnots.pdf");
            PdfWriter writer = new PdfWriter(destinationFolder + "tagStructureRemovingTest04.pdf").SetCompressionLevel
                (CompressionConstants.NO_COMPRESSION);
            PdfDocument document = new PdfDocument(reader, writer);
            document.RemovePage(1);
            document.Close();
            CompareResult("tagStructureRemovingTest04.pdf", "cmp_tagStructureRemovingTest04.pdf", "diffRemoving04_");
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        /// <exception cref="Javax.Xml.Parsers.ParserConfigurationException"/>
        /// <exception cref="Org.Xml.Sax.SAXException"/>
        private void CompareResult(String outFileName, String cmpFileName, String diffNamePrefix) {
            CompareTool compareTool = new CompareTool();
            String outPdf = destinationFolder + outFileName;
            String cmpPdf = sourceFolder + cmpFileName;
            String contentDifferences = compareTool.CompareByContent(outPdf, cmpPdf, destinationFolder, diffNamePrefix
                );
            String taggedStructureDifferences = compareTool.CompareTagStructures(outPdf, cmpPdf);
            String errorMessage = "";
            errorMessage += taggedStructureDifferences == null ? "" : taggedStructureDifferences + "\n";
            errorMessage += contentDifferences == null ? "" : contentDifferences;
            if (!String.IsNullOrEmpty(errorMessage)) {
                NUnit.Framework.Assert.Fail(errorMessage);
            }
        }
    }
}
