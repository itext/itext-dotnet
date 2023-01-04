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
using System.Collections.Generic;
using System.IO;
using iText.IO.Font.Constants;
using iText.Kernel.Exceptions;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Annot;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Tagging;
using iText.Kernel.Utils;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Kernel.Pdf {
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfStructElemTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/pdf/PdfStructElemTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/kernel/pdf/PdfStructElemTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void StructElemTest01() {
            PdfWriter writer = new PdfWriter(destinationFolder + "structElemTest01.pdf");
            writer.SetCompressionLevel(CompressionConstants.NO_COMPRESSION);
            PdfDocument document = new PdfDocument(writer);
            document.SetTagged();
            PdfStructElem doc = document.GetStructTreeRoot().AddKid(new PdfStructElem(document, PdfName.Document));
            PdfPage page1 = document.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page1);
            canvas.BeginText();
            canvas.SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.COURIER), 24);
            canvas.SetTextMatrix(1, 0, 0, 1, 32, 512);
            PdfStructElem paragraph = doc.AddKid(new PdfStructElem(document, PdfName.P));
            PdfStructElem span1 = paragraph.AddKid(new PdfStructElem(document, PdfName.Span, page1));
            canvas.OpenTag(new CanvasTag(span1.AddKid(new PdfMcrNumber(page1, span1))));
            canvas.ShowText("Hello ");
            canvas.CloseTag();
            canvas.OpenTag(new CanvasTag(span1.AddKid(new PdfMcrDictionary(page1, span1))));
            canvas.ShowText("World");
            canvas.CloseTag();
            canvas.EndText();
            canvas.Release();
            PdfPage page2 = document.AddNewPage();
            canvas = new PdfCanvas(page2);
            canvas.BeginText();
            canvas.SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.HELVETICA), 24);
            canvas.SetTextMatrix(1, 0, 0, 1, 32, 512);
            paragraph = doc.AddKid(new PdfStructElem(document, PdfName.P));
            span1 = paragraph.AddKid(new PdfStructElem(document, PdfName.Span, page2));
            canvas.OpenTag(new CanvasTag(span1.AddKid(new PdfMcrNumber(page2, span1))));
            canvas.ShowText("Hello ");
            canvas.CloseTag();
            PdfStructElem span2 = paragraph.AddKid(new PdfStructElem(document, PdfName.Span, page2));
            canvas.OpenTag(new CanvasTag(span2.AddKid(new PdfMcrNumber(page2, span2))));
            canvas.ShowText("World");
            canvas.CloseTag();
            canvas.EndText();
            canvas.Release();
            page1.Flush();
            page2.Flush();
            document.Close();
            CompareResult("structElemTest01.pdf", "cmp_structElemTest01.pdf");
        }

        [NUnit.Framework.Test]
        public virtual void StructElemTest02() {
            PdfWriter writer = new PdfWriter(destinationFolder + "structElemTest02.pdf");
            writer.SetCompressionLevel(CompressionConstants.NO_COMPRESSION);
            PdfDocument document = new PdfDocument(writer);
            document.SetTagged();
            document.GetStructTreeRoot().GetRoleMap().Put(new PdfName("Chunk"), PdfName.Span);
            PdfStructElem doc = document.GetStructTreeRoot().AddKid(new PdfStructElem(document, PdfName.Document));
            PdfPage page = document.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.BeginText();
            canvas.SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.COURIER), 24);
            canvas.SetTextMatrix(1, 0, 0, 1, 32, 512);
            PdfStructElem paragraph = doc.AddKid(new PdfStructElem(document, PdfName.P));
            PdfStructElem span1 = paragraph.AddKid(new PdfStructElem(document, PdfName.Span, page));
            canvas.OpenTag(new CanvasTag(span1.AddKid(new PdfMcrNumber(page, span1))));
            canvas.ShowText("Hello ");
            canvas.CloseTag();
            PdfStructElem span2 = paragraph.AddKid(new PdfStructElem(document, new PdfName("Chunk"), page));
            canvas.OpenTag(new CanvasTag(span2.AddKid(new PdfMcrNumber(page, span2))));
            canvas.ShowText("World");
            canvas.CloseTag();
            canvas.EndText();
            canvas.Release();
            page.Flush();
            document.Close();
            CompareResult("structElemTest02.pdf", "cmp_structElemTest02.pdf");
        }

        [NUnit.Framework.Test]
        public virtual void StructElemTest03() {
            PdfWriter writer = new PdfWriter(destinationFolder + "structElemTest03.pdf");
            writer.SetCompressionLevel(CompressionConstants.NO_COMPRESSION);
            PdfDocument document = new PdfDocument(writer);
            document.SetTagged();
            document.GetStructTreeRoot().GetRoleMap().Put(new PdfName("Chunk"), PdfName.Span);
            PdfStructElem doc = document.GetStructTreeRoot().AddKid(new PdfStructElem(document, PdfName.Document));
            PdfPage page1 = document.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page1);
            canvas.BeginText();
            canvas.SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.COURIER), 24);
            canvas.SetTextMatrix(1, 0, 0, 1, 32, 512);
            PdfStructElem paragraph = doc.AddKid(new PdfStructElem(document, PdfName.P));
            PdfStructElem span1 = paragraph.AddKid(new PdfStructElem(document, PdfName.Span, page1));
            canvas.OpenTag(new CanvasTag(span1.AddKid(new PdfMcrNumber(page1, span1))));
            canvas.ShowText("Hello ");
            canvas.CloseTag();
            PdfStructElem span2 = paragraph.AddKid(new PdfStructElem(document, new PdfName("Chunk"), page1));
            canvas.OpenTag(new CanvasTag(span2.AddKid(new PdfMcrNumber(page1, span2))));
            canvas.ShowText("World");
            canvas.CloseTag();
            canvas.EndText();
            canvas.Release();
            PdfPage page2 = document.AddNewPage();
            canvas = new PdfCanvas(page2);
            canvas.BeginText();
            canvas.SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.HELVETICA), 24);
            canvas.SetTextMatrix(1, 0, 0, 1, 32, 512);
            paragraph = doc.AddKid(new PdfStructElem(document, PdfName.P));
            span1 = paragraph.AddKid(new PdfStructElem(document, PdfName.Span, page2));
            canvas.OpenTag(new CanvasTag(span1.AddKid(new PdfMcrNumber(page2, span1))));
            canvas.ShowText("Hello ");
            canvas.CloseTag();
            span2 = paragraph.AddKid(new PdfStructElem(document, new PdfName("Chunk"), page2));
            canvas.OpenTag(new CanvasTag(span2.AddKid(new PdfMcrNumber(page2, span2))));
            canvas.ShowText("World");
            canvas.CloseTag();
            canvas.EndText();
            canvas.Release();
            page1.Flush();
            page2.Flush();
            document.Close();
            document = new PdfDocument(new PdfReader(destinationFolder + "structElemTest03.pdf"));
            NUnit.Framework.Assert.AreEqual(2, (int)document.GetNextStructParentIndex());
            PdfPage page = document.GetPage(1);
            NUnit.Framework.Assert.AreEqual(0, page.GetStructParentIndex());
            NUnit.Framework.Assert.AreEqual(2, page.GetNextMcid());
            document.Close();
        }

        [NUnit.Framework.Test]
        public virtual void StructElemTest04() {
            MemoryStream baos = new MemoryStream();
            PdfWriter writer = new PdfWriter(baos);
            writer.SetCompressionLevel(CompressionConstants.NO_COMPRESSION);
            PdfDocument document = new PdfDocument(writer);
            document.SetTagged();
            document.GetStructTreeRoot().GetRoleMap().Put(new PdfName("Chunk"), PdfName.Span);
            PdfStructElem doc = document.GetStructTreeRoot().AddKid(new PdfStructElem(document, PdfName.Document));
            PdfPage page = document.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.BeginText();
            canvas.SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.COURIER), 24);
            canvas.SetTextMatrix(1, 0, 0, 1, 32, 512);
            PdfStructElem paragraph = doc.AddKid(new PdfStructElem(document, PdfName.P));
            PdfStructElem span1 = paragraph.AddKid(new PdfStructElem(document, PdfName.Span, page));
            canvas.OpenTag(new CanvasTag(span1.AddKid(new PdfMcrNumber(page, span1))));
            canvas.ShowText("Hello ");
            canvas.CloseTag();
            PdfStructElem span2 = paragraph.AddKid(new PdfStructElem(document, new PdfName("Chunk"), page));
            canvas.OpenTag(new CanvasTag(span2.AddKid(new PdfMcrNumber(page, span2))));
            canvas.ShowText("World");
            canvas.CloseTag();
            canvas.EndText();
            canvas.Release();
            page.Flush();
            document.Close();
            byte[] bytes = baos.ToArray();
            PdfReader reader = new PdfReader(new MemoryStream(bytes));
            writer = new PdfWriter(destinationFolder + "structElemTest04.pdf");
            writer.SetCompressionLevel(CompressionConstants.NO_COMPRESSION);
            document = new PdfDocument(reader, writer);
            page = document.GetPage(1);
            canvas = new PdfCanvas(page);
            PdfStructElem p = (PdfStructElem)document.GetStructTreeRoot().GetKids()[0].GetKids()[0];
            canvas.BeginText();
            canvas.SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.COURIER), 24);
            canvas.SetTextMatrix(1, 0, 0, 1, 32, 490);
            //Inserting span between of 2 existing ones.
            span1 = p.AddKid(1, new PdfStructElem(document, PdfName.Span, page));
            canvas.OpenTag(new CanvasTag(span1.AddKid(new PdfMcrNumber(page, span1))));
            canvas.ShowText("text1");
            canvas.CloseTag();
            //Inserting span at the end.
            span1 = p.AddKid(new PdfStructElem(document, PdfName.Span, page));
            canvas.OpenTag(new CanvasTag(span1.AddKid(new PdfMcrNumber(page, span1))));
            canvas.ShowText("text2");
            canvas.CloseTag();
            canvas.EndText();
            canvas.Release();
            page.Flush();
            document.Close();
            CompareResult("structElemTest04.pdf", "cmp_structElemTest04.pdf");
        }

        [NUnit.Framework.Test]
        public virtual void StructElemTest05() {
            PdfWriter writer = new PdfWriter(destinationFolder + "structElemTest05.pdf");
            writer.SetCompressionLevel(CompressionConstants.NO_COMPRESSION);
            PdfDocument document = new PdfDocument(writer);
            document.SetTagged();
            PdfStructElem doc = document.GetStructTreeRoot().AddKid(new PdfStructElem(document, PdfName.Document));
            PdfPage page = document.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.BeginText();
            canvas.SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.COURIER), 14);
            canvas.SetTextMatrix(1, 0, 0, 1, 32, 512);
            PdfStructElem paragraph = doc.AddKid(new PdfStructElem(document, PdfName.P));
            PdfStructElem span1 = paragraph.AddKid(new PdfStructElem(document, PdfName.Span, page));
            canvas.OpenTag(new CanvasTag(span1.AddKid(new PdfMcrNumber(page, span1))));
            canvas.ShowText("Click ");
            canvas.CloseTag();
            PdfStructElem link = paragraph.AddKid(new PdfStructElem(document, PdfName.Link, page));
            canvas.OpenTag(new CanvasTag(link.AddKid(new PdfMcrNumber(page, link))));
            canvas.SetFillColorRgb(0, 0, 1).ShowText("here");
            PdfLinkAnnotation linkAnnotation = new PdfLinkAnnotation(new Rectangle(80, 508, 40, 18));
            linkAnnotation.SetColor(new float[] { 0, 0, 1 }).SetBorder(new PdfArray(new float[] { 0, 0, 1 }));
            page.AddAnnotation(-1, linkAnnotation, false);
            link.AddKid(new PdfObjRef(linkAnnotation, link, document.GetNextStructParentIndex()));
            canvas.CloseTag();
            PdfStructElem span2 = paragraph.AddKid(new PdfStructElem(document, PdfName.Span, page));
            canvas.OpenTag(new CanvasTag(span2.AddKid(new PdfMcrNumber(page, span2))));
            canvas.SetFillColorRgb(0, 0, 0);
            canvas.ShowText(" to visit iText site.");
            canvas.CloseTag();
            canvas.EndText();
            canvas.Release();
            document.Close();
            CompareResult("structElemTest05.pdf", "cmp_structElemTest05.pdf");
        }

        [NUnit.Framework.Test]
        public virtual void StructElemTest06() {
            PdfWriter writer = new PdfWriter(destinationFolder + "structElemTest06.pdf");
            writer.SetCompressionLevel(CompressionConstants.NO_COMPRESSION);
            PdfDocument document = new PdfDocument(writer);
            document.SetTagged();
            PdfStructElem doc = document.GetStructTreeRoot().AddKid(new PdfStructElem(document, PdfName.Document));
            PdfPage page = document.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.BeginText();
            canvas.SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.COURIER), 14);
            canvas.SetTextMatrix(1, 0, 0, 1, 32, 512);
            PdfStructElem paragraph = doc.AddKid(new PdfStructElem(document, PdfName.P));
            PdfStructElem span1 = paragraph.AddKid(new PdfStructElem(document, PdfName.Span, page));
            canvas.OpenTag(new CanvasTag(span1.AddKid(new PdfMcrNumber(page, span1))).AddProperty(PdfName.Lang, new PdfString
                ("en-US")).AddProperty(PdfName.ActualText, new PdfString("The actual text is: Text with property list"
                )));
            canvas.ShowText("Text with property list");
            canvas.CloseTag();
            canvas.EndText();
            canvas.Release();
            document.Close();
            CompareResult("structElemTest06.pdf", "cmp_structElemTest06.pdf");
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.VERSION_INCOMPATIBILITY_FOR_DICTIONARY_ENTRY, Count = 5)]
        public virtual void StructElemTest07() {
            PdfWriter writer = new PdfWriter(destinationFolder + "structElemTest07.pdf");
            writer.SetCompressionLevel(CompressionConstants.NO_COMPRESSION);
            PdfDocument document = new PdfDocument(writer);
            document.SetTagged();
            PdfStructElem doc = document.GetStructTreeRoot().AddKid(new PdfStructElem(document, PdfName.Document));
            PdfPage page = document.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page);
            canvas.BeginText();
            canvas.SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.COURIER), 24);
            canvas.SetTextMatrix(1, 0, 0, 1, 32, 512);
            PdfStructElem paragraph = doc.AddKid(new PdfStructElem(document, PdfName.P));
            PdfStructElem span1 = paragraph.AddKid(new PdfStructElem(document, PdfName.Span, page));
            canvas.OpenTag(new CanvasTag(span1.AddKid(new PdfMcrNumber(page, span1))));
            canvas.ShowText("Hello ");
            canvas.CloseTag();
            PdfStructElem span2 = paragraph.AddKid(new PdfStructElem(document, new PdfName("Chunk"), page));
            canvas.OpenTag(new CanvasTag(span2.AddKid(new PdfMcrNumber(page, span2))));
            canvas.ShowText("World");
            canvas.CloseTag();
            canvas.EndText();
            canvas.Release();
            PdfNamespace @namespace = new PdfNamespace("http://www.w3.org/1999/xhtml");
            span1.SetNamespace(@namespace);
            span1.AddRef(span2);
            span1.SetPhoneticAlphabet(PdfName.ipa);
            span1.SetPhoneme(new PdfString("Heeeelllloooooo"));
            @namespace.AddNamespaceRoleMapping(StandardRoles.SPAN, StandardRoles.SPAN);
            document.GetStructTreeRoot().AddNamespace(@namespace);
            page.Flush();
            document.Close();
            CompareResult("structElemTest07.pdf", "cmp_structElemTest07.pdf");
        }

        [NUnit.Framework.Test]
        public virtual void StructElemTest08() {
            PdfWriter writer = new PdfWriter(destinationFolder + "structElemTest08.pdf");
            writer.SetCompressionLevel(CompressionConstants.NO_COMPRESSION);
            PdfDocument document = new PdfDocument(writer);
            document.SetTagged();
            PdfStructTreeRoot doc = document.GetStructTreeRoot();
            PdfPage firstPage = document.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(firstPage);
            canvas.BeginText();
            canvas.SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.COURIER), 24);
            canvas.SetTextMatrix(1, 0, 0, 1, 32, 512);
            PdfStructElem paragraph = doc.AddKid(new PdfStructElem(document, PdfName.P));
            PdfStructElem span1 = paragraph.AddKid(new PdfStructElem(document, PdfName.Span, firstPage));
            canvas.OpenTag(new CanvasTag(span1.AddKid(new PdfMcrNumber(firstPage, span1))));
            canvas.ShowText("Hello ");
            canvas.CloseTag();
            canvas.OpenTag(new CanvasTag(span1.AddKid(new PdfMcrDictionary(firstPage, span1))));
            canvas.ShowText("World");
            canvas.CloseTag();
            canvas.EndText();
            canvas.Release();
            PdfPage secondPage = document.AddNewPage();
            // on flushing, the Document tag is not added
            firstPage.Flush();
            secondPage.Flush();
            document.Close();
            CompareResult("structElemTest08.pdf", "cmp_structElemTest08.pdf");
        }

        [NUnit.Framework.Test]
        public virtual void StructElemTest09() {
            PdfWriter writer = new PdfWriter(destinationFolder + "structElemTest09.pdf");
            writer.SetCompressionLevel(CompressionConstants.NO_COMPRESSION);
            PdfDocument document = new PdfDocument(new PdfReader(sourceFolder + "88th_Academy_Awards_mult_roots.pdf"), 
                writer);
            document.RemovePage(1);
            document.Close();
            CompareResult("structElemTest09.pdf", "cmp_structElemTest09.pdf");
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.SOURCE_DOCUMENT_HAS_ACROFORM_DICTIONARY)]
        public virtual void StructTreeCopyingTest01() {
            PdfDocument source = new PdfDocument(new PdfReader(sourceFolder + "iphone_user_guide.pdf"));
            PdfDocument destination = new PdfDocument(new PdfWriter(destinationFolder + "structTreeCopyingTest01.pdf")
                );
            destination.SetTagged();
            destination.InitializeOutlines();
            List<int> pagesToCopy = new List<int>();
            pagesToCopy.Add(3);
            pagesToCopy.Add(4);
            pagesToCopy.Add(10);
            pagesToCopy.Add(11);
            source.CopyPagesTo(pagesToCopy, destination);
            source.CopyPagesTo(50, 52, destination);
            destination.Close();
            source.Close();
            CompareResult("structTreeCopyingTest01.pdf", "cmp_structTreeCopyingTest01.pdf");
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.SOURCE_DOCUMENT_HAS_ACROFORM_DICTIONARY)]
        public virtual void StructTreeCopyingTest02() {
            PdfDocument source = new PdfDocument(new PdfReader(sourceFolder + "iphone_user_guide.pdf"));
            PdfDocument destination = new PdfDocument(new PdfWriter(destinationFolder + "structTreeCopyingTest02.pdf")
                );
            destination.SetTagged();
            destination.InitializeOutlines();
            source.CopyPagesTo(6, source.GetNumberOfPages(), destination);
            source.CopyPagesTo(1, 5, destination);
            destination.Close();
            source.Close();
            CompareResult("structTreeCopyingTest02.pdf", "cmp_structTreeCopyingTest02.pdf");
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.SOURCE_DOCUMENT_HAS_ACROFORM_DICTIONARY)]
        public virtual void StructTreeCopyingTest03() {
            PdfDocument source = new PdfDocument(new PdfReader(sourceFolder + "iphone_user_guide.pdf"));
            PdfDocument destination = new PdfDocument(new PdfWriter(destinationFolder + "structTreeCopyingTest03.pdf")
                );
            destination.InitializeOutlines();
            source.CopyPagesTo(6, source.GetNumberOfPages(), destination);
            source.CopyPagesTo(1, 5, destination);
            destination.Close();
            source.Close();
            // we don't compare tag structures, because resultant document is not tagged
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + "structTreeCopyingTest03.pdf"
                , sourceFolder + "cmp_structTreeCopyingTest03.pdf", destinationFolder, "diff_copying_03_"));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.SOURCE_DOCUMENT_HAS_ACROFORM_DICTIONARY)]
        public virtual void StructTreeCopyingTest04() {
            PdfDocument source = new PdfDocument(new PdfReader(sourceFolder + "iphone_user_guide.pdf"));
            PdfDocument destination = new PdfDocument(new PdfWriter(destinationFolder + "structTreeCopyingTest04.pdf")
                );
            destination.SetTagged();
            destination.InitializeOutlines();
            for (int i = 1; i <= source.GetNumberOfPages(); i++) {
                source.CopyPagesTo(i, i, destination);
            }
            destination.Close();
            source.Close();
            CompareResult("structTreeCopyingTest04.pdf", "cmp_structTreeCopyingTest04.pdf");
        }

        [NUnit.Framework.Test]
        public virtual void StructTreeCopyingTest05() {
            PdfDocument document = new PdfDocument(new PdfReader(sourceFolder + "iphone_user_guide.pdf"), new PdfWriter
                (destinationFolder + "structTreeCopyingTest05.pdf"));
            PdfDocument document1 = new PdfDocument(new PdfReader(sourceFolder + "quick-brown-fox.pdf"));
            document1.CopyPagesTo(1, 1, document, 2);
            PdfDocument document2 = new PdfDocument(new PdfReader(sourceFolder + "quick-brown-fox-table.pdf"));
            document2.CopyPagesTo(1, 3, document, 4);
            document.Close();
            document1.Close();
            document2.Close();
            CompareResult("structTreeCopyingTest05.pdf", "cmp_structTreeCopyingTest05.pdf");
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.SOURCE_DOCUMENT_HAS_ACROFORM_DICTIONARY)]
        public virtual void StructTreeCopyingTest06() {
            PdfDocument source = new PdfDocument(new PdfReader(sourceFolder + "iphone_user_guide.pdf"));
            PdfDocument destination = new PdfDocument(new PdfWriter(destinationFolder + "structTreeCopyingTest06.pdf")
                );
            destination.SetTagged();
            destination.InitializeOutlines();
            source.CopyPagesTo(1, source.GetNumberOfPages(), destination);
            destination.Close();
            source.Close();
            CompareResult("structTreeCopyingTest06.pdf", "cmp_structTreeCopyingTest06.pdf");
        }

        [NUnit.Framework.Test]
        public virtual void StructTreeCopyingTest07() {
            PdfReader reader = new PdfReader(sourceFolder + "quick-brown-fox.pdf");
            PdfWriter writer = new PdfWriter(destinationFolder + "structTreeCopyingTest07.pdf");
            PdfDocument document = new PdfDocument(writer);
            document.SetTagged();
            PdfStructElem doc = document.GetStructTreeRoot().AddKid(new PdfStructElem(document, PdfName.Document));
            PdfPage page1 = document.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page1);
            canvas.BeginText();
            canvas.SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.COURIER), 24);
            canvas.SetTextMatrix(1, 0, 0, 1, 32, 512);
            PdfStructElem paragraph = doc.AddKid(new PdfStructElem(document, PdfName.P));
            PdfStructElem span1 = paragraph.AddKid(new PdfStructElem(document, PdfName.Span, page1));
            canvas.OpenTag(new CanvasTag(span1.AddKid(new PdfMcrNumber(page1, span1))));
            canvas.ShowText("Hello ");
            canvas.CloseTag();
            canvas.OpenTag(new CanvasTag(span1.AddKid(new PdfMcrDictionary(page1, span1))));
            canvas.ShowText("World");
            canvas.CloseTag();
            canvas.EndText();
            canvas.Release();
            PdfDocument document1 = new PdfDocument(reader);
            document1.InitializeOutlines();
            document1.CopyPagesTo(1, 1, document);
            document.Close();
            document1.Close();
            CompareResult("structTreeCopyingTest07.pdf", "cmp_structTreeCopyingTest07.pdf");
        }

        [NUnit.Framework.Test]
        public virtual void StructTreeCopyingTest08() {
            PdfDocument document = new PdfDocument(new PdfReader(sourceFolder + "quick-brown-fox-table.pdf"), new PdfWriter
                (destinationFolder + "structTreeCopyingTest08.pdf"));
            PdfDocument document1 = new PdfDocument(new PdfReader(sourceFolder + "quick-brown-fox.pdf"));
            document1.InitializeOutlines();
            document1.CopyPagesTo(1, 1, document, 2);
            document.Close();
            document1.Close();
            CompareResult("structTreeCopyingTest08.pdf", "cmp_structTreeCopyingTest08.pdf");
        }

        [NUnit.Framework.Test]
        public virtual void StructTreeCopyingTest09() {
            PdfDocument document = new PdfDocument(new PdfReader(sourceFolder + "quick-brown-fox-table.pdf"), new PdfWriter
                (destinationFolder + "structTreeCopyingTest09.pdf"));
            PdfDocument document1 = new PdfDocument(new PdfReader(sourceFolder + "quick-brown-fox.pdf"));
            document1.InitializeOutlines();
            document1.CopyPagesTo(1, 1, document, 2);
            document1.CopyPagesTo(1, 1, document, 4);
            document.Close();
            document1.Close();
            CompareResult("structTreeCopyingTest09.pdf", "cmp_structTreeCopyingTest09.pdf");
        }

        [NUnit.Framework.Test]
        public virtual void StructTreeCopyingTest10() {
            PdfDocument document = new PdfDocument(new PdfReader(sourceFolder + "88th_Academy_Awards.pdf"), new PdfWriter
                (destinationFolder + "structTreeCopyingTest10.pdf"));
            PdfDocument document1 = new PdfDocument(new PdfReader(sourceFolder + "quick-brown-fox-table.pdf"));
            document1.InitializeOutlines();
            document1.CopyPagesTo(1, 3, document, 2);
            PdfDocument document2 = new PdfDocument(new PdfReader(sourceFolder + "quick-brown-fox.pdf"));
            document2.InitializeOutlines();
            document2.CopyPagesTo(1, 1, document, 4);
            document.Close();
            document1.Close();
            document2.Close();
            CompareResult("structTreeCopyingTest10.pdf", "cmp_structTreeCopyingTest10.pdf");
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.ROLE_MAPPING_FROM_SOURCE_IS_NOT_COPIED_ALREADY_EXIST)]
        public virtual void StructTreeCopyingTest11() {
            PdfDocument document = new PdfDocument(new PdfReader(sourceFolder + "88th_Academy_Awards.pdf"), new PdfWriter
                (destinationFolder + "structTreeCopyingTest11.pdf"));
            PdfDocument document1 = new PdfDocument(new PdfReader(sourceFolder + "quick-brown-fox_mapping_mod.pdf"));
            document1.InitializeOutlines();
            document1.CopyPagesTo(1, 1, document, 2);
            PdfDocument document2 = new PdfDocument(new PdfReader(sourceFolder + "quick-brown-fox.pdf"));
            document2.InitializeOutlines();
            document2.CopyPagesTo(1, 1, document, 4);
            document.Close();
            document1.Close();
            document2.Close();
            CompareResult("structTreeCopyingTest11.pdf", "cmp_structTreeCopyingTest11.pdf");
        }

        [NUnit.Framework.Test]
        public virtual void StructTreeCopyingToPartiallyFlushedDocumentTest() {
            String outFile = "structTreeCopyingToPartiallyFlushedDocumentTest.pdf";
            PdfDocument resultDoc = new PdfDocument(new PdfWriter(destinationFolder + outFile));
            resultDoc.SetTagged();
            PdfDocument document1 = new PdfDocument(new PdfReader(sourceFolder + "quick-brown-fox.pdf"));
            document1.CopyPagesTo(1, 1, resultDoc);
            resultDoc.FlushCopiedObjects(document1);
            document1.Close();
            PdfDocument document2 = new PdfDocument(new PdfReader(sourceFolder + "quick-brown-fox.pdf"));
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => {
                document2.CopyPagesTo(1, 1, resultDoc);
            }
            );
            // TODO DEVSIX-7005 after exception is gone add assertion for the resulting document
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.TAG_STRUCTURE_COPYING_FAILED_IT_MIGHT_BE_CORRUPTED_IN_ONE_OF_THE_DOCUMENTS
                , e.Message);
            document2.Close();
            resultDoc.Close();
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.ENCOUNTERED_INVALID_MCR)]
        public virtual void CorruptedTagStructureTest01() {
            PdfDocument document = new PdfDocument(new PdfReader(sourceFolder + "invalidMcr.pdf"));
            NUnit.Framework.Assert.IsTrue(document.IsTagged());
            document.Close();
        }

        [NUnit.Framework.Test]
        public virtual void CorruptedTagStructureTest02() {
            PdfDocument document = new PdfDocument(new PdfReader(sourceFolder + "directStructElem01.pdf"));
            NUnit.Framework.Assert.IsTrue(document.IsTagged());
            document.Close();
        }

        [NUnit.Framework.Test]
        public virtual void CorruptedTagStructureTest03() {
            PdfReader reader = new PdfReader(sourceFolder + "directStructElem02.pdf");
            MemoryStream baos = new MemoryStream();
            PdfWriter writer = new PdfWriter(baos);
            PdfDocument document = new PdfDocument(reader, writer);
            NUnit.Framework.Assert.IsTrue(document.IsTagged());
            document.Close();
            document = new PdfDocument(new PdfReader(new MemoryStream(baos.ToArray())));
            NUnit.Framework.Assert.IsTrue(document.IsTagged());
            document.Close();
        }

        [NUnit.Framework.Test]
        public virtual void CorruptedTagStructureTest04() {
            PdfDocument document = new PdfDocument(new PdfReader(sourceFolder + "directStructElem03.pdf"));
            NUnit.Framework.Assert.IsTrue(document.IsTagged());
            MemoryStream baos = new MemoryStream();
            PdfDocument docToCopyTo = new PdfDocument(new PdfWriter(baos));
            docToCopyTo.SetTagged();
            document.CopyPagesTo(1, 1, docToCopyTo);
            document.Close();
            docToCopyTo.Close();
            document = new PdfDocument(new PdfReader(new MemoryStream(baos.ToArray())));
            NUnit.Framework.Assert.IsTrue(document.IsTagged());
            document.Close();
        }

        private void CompareResult(String outFileName, String cmpFileName) {
            CompareTool compareTool = new CompareTool();
            String outPdf = destinationFolder + outFileName;
            String cmpPdf = sourceFolder + cmpFileName;
            String contentDifferences = compareTool.CompareByContent(outPdf, cmpPdf, destinationFolder);
            String taggedStructureDifferences = compareTool.CompareTagStructures(outPdf, cmpPdf);
            String errorMessage = "";
            errorMessage += taggedStructureDifferences == null ? "" : taggedStructureDifferences + "\n";
            errorMessage += contentDifferences == null ? "" : contentDifferences;
            if (errorMessage.Length > 0) {
                NUnit.Framework.Assert.Fail(errorMessage);
            }
        }
    }
}
