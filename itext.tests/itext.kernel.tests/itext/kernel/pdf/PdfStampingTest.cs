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
using System.IO;
using iText.Commons.Utils;
using iText.IO.Source;
using iText.Kernel.Events;
using iText.Kernel.Font;
using iText.Kernel.Logs;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using iText.Kernel.Utils;
using iText.Kernel.XMP;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Kernel.Pdf {
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfStampingTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/pdf/PdfStampingTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/kernel/pdf/PdfStampingTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.OneTimeTearDown]
        public static void AfterClass() {
            CompareTool.Cleanup(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void Stamping1() {
            String filename1 = destinationFolder + "stamping1_1.pdf";
            String filename2 = destinationFolder + "stamping1_2.pdf";
            PdfDocument pdfDoc1 = new PdfDocument(CompareTool.CreateTestPdfWriter(filename1));
            pdfDoc1.GetDocumentInfo().SetAuthor("Alexander Chingarev").SetCreator("iText 6").SetTitle("Empty iText 6 Document"
                );
            PdfPage page1 = pdfDoc1.AddNewPage();
            page1.GetContentStream(0).GetOutputStream().Write(ByteUtils.GetIsoBytes("%Hello World\n"));
            page1.Flush();
            pdfDoc1.Close();
            PdfReader reader2 = CompareTool.CreateOutputReader(filename1);
            PdfWriter writer2 = CompareTool.CreateTestPdfWriter(filename2);
            PdfDocument pdfDoc2 = new PdfDocument(reader2, writer2);
            pdfDoc2.GetDocumentInfo().SetCreator("iText").SetTitle("Empty iText Document");
            pdfDoc2.Close();
            PdfReader reader3 = CompareTool.CreateOutputReader(filename2);
            PdfDocument pdfDoc3 = new PdfDocument(reader3);
            for (int i = 0; i < pdfDoc3.GetNumberOfPages(); i++) {
                pdfDoc3.GetPage(i + 1);
            }
            NUnit.Framework.Assert.AreEqual(false, reader3.HasRebuiltXref(), "Rebuilt");
            NUnit.Framework.Assert.AreEqual(false, reader3.HasFixedXref(), "Fixed");
            VerifyPdfPagesCount(pdfDoc3.GetCatalog().GetPageTree().GetRoot().GetPdfObject());
            pdfDoc3.Close();
            PdfReader reader = CompareTool.CreateOutputReader(destinationFolder + "stamping1_2.pdf");
            PdfDocument document = new PdfDocument(reader);
            NUnit.Framework.Assert.AreEqual(false, reader.HasRebuiltXref(), "Rebuilt");
            PdfDictionary trailer = document.GetTrailer();
            PdfDictionary info = trailer.GetAsDictionary(PdfName.Info);
            PdfString creator = info.GetAsString(PdfName.Creator);
            NUnit.Framework.Assert.AreEqual("iText", creator.ToString());
            byte[] bytes = document.GetPage(1).GetContentBytes();
            NUnit.Framework.Assert.AreEqual("%Hello World\n", iText.Commons.Utils.JavaUtil.GetStringForBytes(bytes));
            String date = document.GetDocumentInfo().GetPdfObject().GetAsString(PdfName.ModDate).GetValue();
            DateTime cl = PdfDate.Decode(date);
            double diff = DateTimeUtil.GetUtcMillisFromEpoch(null) - DateTimeUtil.GetUtcMillisFromEpoch(cl);
            String message = "Unexpected creation date. Different from now is " + (float)diff / 1000 + "s";
            NUnit.Framework.Assert.IsTrue(diff < 5000, message);
            document.Close();
        }

        [NUnit.Framework.Test]
        public virtual void Stamping2() {
            String filename1 = destinationFolder + "stamping2_1.pdf";
            String filename2 = destinationFolder + "stamping2_2.pdf";
            PdfDocument pdfDoc1 = new PdfDocument(CompareTool.CreateTestPdfWriter(filename1));
            PdfPage page1 = pdfDoc1.AddNewPage();
            page1.GetContentStream(0).GetOutputStream().Write(ByteUtils.GetIsoBytes("%page 1\n"));
            page1.Flush();
            pdfDoc1.Close();
            PdfReader reader2 = CompareTool.CreateOutputReader(filename1);
            PdfWriter writer2 = CompareTool.CreateTestPdfWriter(filename2);
            PdfDocument pdfDoc2 = new PdfDocument(reader2, writer2);
            PdfPage page2 = pdfDoc2.AddNewPage();
            page2.GetContentStream(0).GetOutputStream().Write(ByteUtils.GetIsoBytes("%page 2\n"));
            page2.Flush();
            pdfDoc2.Close();
            PdfReader reader3 = CompareTool.CreateOutputReader(filename2);
            PdfDocument pdfDoc3 = new PdfDocument(reader3);
            for (int i = 0; i < pdfDoc3.GetNumberOfPages(); i++) {
                pdfDoc3.GetPage(i + 1);
            }
            NUnit.Framework.Assert.AreEqual(false, reader3.HasRebuiltXref(), "Rebuilt");
            NUnit.Framework.Assert.AreEqual(false, reader3.HasFixedXref(), "Fixed");
            VerifyPdfPagesCount(pdfDoc3.GetCatalog().GetPageTree().GetRoot().GetPdfObject());
            pdfDoc3.Close();
            PdfReader reader = CompareTool.CreateOutputReader(destinationFolder + "stamping2_2.pdf");
            PdfDocument pdfDocument = new PdfDocument(reader);
            NUnit.Framework.Assert.AreEqual(false, reader.HasRebuiltXref(), "Rebuilt");
            byte[] bytes = pdfDocument.GetPage(1).GetContentBytes();
            NUnit.Framework.Assert.AreEqual("%page 1\n", iText.Commons.Utils.JavaUtil.GetStringForBytes(bytes));
            bytes = pdfDocument.GetPage(2).GetContentBytes();
            NUnit.Framework.Assert.AreEqual("%page 2\n", iText.Commons.Utils.JavaUtil.GetStringForBytes(bytes));
            reader.Close();
        }

        [NUnit.Framework.Test]
        public virtual void Stamping3() {
            String filename1 = destinationFolder + "stamping3_1.pdf";
            String filename2 = destinationFolder + "stamping3_2.pdf";
            PdfWriter writer1 = CompareTool.CreateTestPdfWriter(filename1, new WriterProperties().SetFullCompressionMode
                (true));
            PdfDocument pdfDoc1 = new PdfDocument(writer1);
            PdfPage page1 = pdfDoc1.AddNewPage();
            page1.GetContentStream(0).GetOutputStream().Write(ByteUtils.GetIsoBytes("%page 1\n"));
            page1.Flush();
            pdfDoc1.Close();
            PdfReader reader2 = CompareTool.CreateOutputReader(filename1);
            PdfWriter writer2 = CompareTool.CreateTestPdfWriter(filename2, new WriterProperties().SetFullCompressionMode
                (true));
            PdfDocument pdfDoc2 = new PdfDocument(reader2, writer2);
            PdfPage page2 = pdfDoc2.AddNewPage();
            page2.GetContentStream(0).GetOutputStream().Write(ByteUtils.GetIsoBytes("%page 2\n"));
            page2.Flush();
            pdfDoc2.Close();
            PdfReader reader3 = CompareTool.CreateOutputReader(filename2);
            PdfDocument pdfDoc3 = new PdfDocument(reader3);
            for (int i = 0; i < pdfDoc3.GetNumberOfPages(); i++) {
                pdfDoc3.GetPage(i + 1);
            }
            NUnit.Framework.Assert.AreEqual(false, reader3.HasRebuiltXref(), "Rebuilt");
            NUnit.Framework.Assert.AreEqual(false, reader3.HasFixedXref(), "Fixed");
            VerifyPdfPagesCount(pdfDoc3.GetCatalog().GetPageTree().GetRoot().GetPdfObject());
            pdfDoc3.Close();
            PdfReader reader = CompareTool.CreateOutputReader(filename2);
            PdfDocument pdfDocument = new PdfDocument(reader);
            NUnit.Framework.Assert.AreEqual(false, reader.HasRebuiltXref(), "Rebuilt");
            byte[] bytes = pdfDocument.GetPage(1).GetContentBytes();
            NUnit.Framework.Assert.AreEqual("%page 1\n", iText.Commons.Utils.JavaUtil.GetStringForBytes(bytes));
            bytes = pdfDocument.GetPage(2).GetContentBytes();
            NUnit.Framework.Assert.AreEqual("%page 2\n", iText.Commons.Utils.JavaUtil.GetStringForBytes(bytes));
            pdfDocument.Close();
        }

        [NUnit.Framework.Test]
        public virtual void Stamping4() {
            String filename1 = destinationFolder + "stamping4_1.pdf";
            String filename2 = destinationFolder + "stamping4_2.pdf";
            PdfDocument pdfDoc1 = new PdfDocument(CompareTool.CreateTestPdfWriter(filename1));
            PdfPage page1 = pdfDoc1.AddNewPage();
            page1.GetContentStream(0).GetOutputStream().Write(ByteUtils.GetIsoBytes("%page 1\n"));
            page1.Flush();
            pdfDoc1.Close();
            int pageCount = 15;
            PdfDocument pdfDoc2 = new PdfDocument(CompareTool.CreateOutputReader(filename1), CompareTool.CreateTestPdfWriter
                (filename2));
            for (int i = 2; i <= pageCount; i++) {
                PdfPage page2 = pdfDoc2.AddNewPage();
                page2.GetContentStream(0).GetOutputStream().Write(ByteUtils.GetIsoBytes("%page " + i + "\n"));
                page2.Flush();
            }
            pdfDoc2.Close();
            PdfReader reader3 = CompareTool.CreateOutputReader(filename2);
            PdfDocument pdfDoc3 = new PdfDocument(reader3);
            for (int i = 0; i < pdfDoc3.GetNumberOfPages(); i++) {
                pdfDoc3.GetPage(i + 1);
            }
            NUnit.Framework.Assert.AreEqual(false, reader3.HasRebuiltXref(), "Rebuilt");
            NUnit.Framework.Assert.AreEqual(false, reader3.HasFixedXref(), "Fixed");
            VerifyPdfPagesCount(pdfDoc3.GetCatalog().GetPageTree().GetRoot().GetPdfObject());
            pdfDoc3.Close();
            PdfReader reader = CompareTool.CreateOutputReader(filename2);
            PdfDocument pdfDocument = new PdfDocument(reader);
            NUnit.Framework.Assert.AreEqual(false, reader.HasRebuiltXref(), "Rebuilt");
            NUnit.Framework.Assert.AreEqual(pageCount, pdfDocument.GetNumberOfPages(), "Page count");
            for (int i = 1; i < pdfDocument.GetNumberOfPages(); i++) {
                byte[] bytes = pdfDocument.GetPage(i).GetContentBytes();
                NUnit.Framework.Assert.AreEqual("%page " + i + "\n", iText.Commons.Utils.JavaUtil.GetStringForBytes(bytes)
                    );
            }
            pdfDocument.Close();
        }

        [NUnit.Framework.Test]
        public virtual void Stamping5() {
            String filename1 = destinationFolder + "stamping5_1.pdf";
            String filename2 = destinationFolder + "stamping5_2.pdf";
            PdfDocument pdfDoc1 = new PdfDocument(CompareTool.CreateTestPdfWriter(filename1));
            PdfPage page1 = pdfDoc1.AddNewPage();
            page1.GetContentStream(0).GetOutputStream().Write(ByteUtils.GetIsoBytes("%page 1\n"));
            page1.Flush();
            pdfDoc1.Close();
            int pageCount = 15;
            PdfReader reader2 = CompareTool.CreateOutputReader(filename1);
            PdfWriter writer2 = CompareTool.CreateTestPdfWriter(filename2, new WriterProperties().SetFullCompressionMode
                (true));
            PdfDocument pdfDoc2 = new PdfDocument(reader2, writer2);
            for (int i = 2; i <= pageCount; i++) {
                PdfPage page2 = pdfDoc2.AddNewPage();
                page2.GetContentStream(0).GetOutputStream().Write(ByteUtils.GetIsoBytes("%page " + i + "\n"));
                page2.Flush();
            }
            pdfDoc2.Close();
            PdfReader reader3 = CompareTool.CreateOutputReader(filename2);
            PdfDocument pdfDoc3 = new PdfDocument(reader3);
            for (int i = 0; i < pdfDoc3.GetNumberOfPages(); i++) {
                pdfDoc3.GetPage(i + 1);
            }
            NUnit.Framework.Assert.AreEqual(false, reader3.HasRebuiltXref(), "Rebuilt");
            NUnit.Framework.Assert.AreEqual(false, reader3.HasFixedXref(), "Fixed");
            VerifyPdfPagesCount(pdfDoc3.GetCatalog().GetPageTree().GetRoot().GetPdfObject());
            pdfDoc3.Close();
            PdfReader reader = CompareTool.CreateOutputReader(filename2);
            PdfDocument pdfDocument = new PdfDocument(reader);
            NUnit.Framework.Assert.AreEqual(false, reader.HasRebuiltXref(), "Rebuilt");
            NUnit.Framework.Assert.AreEqual(pageCount, pdfDocument.GetNumberOfPages(), "Page count");
            for (int i = 1; i < pdfDocument.GetNumberOfPages(); i++) {
                byte[] bytes = pdfDocument.GetPage(i).GetContentBytes();
                NUnit.Framework.Assert.AreEqual("%page " + i + "\n", iText.Commons.Utils.JavaUtil.GetStringForBytes(bytes)
                    );
            }
            reader.Close();
        }

        [NUnit.Framework.Test]
        public virtual void Stamping6() {
            String filename1 = destinationFolder + "stamping6_1.pdf";
            String filename2 = destinationFolder + "stamping6_2.pdf";
            PdfDocument pdfDoc1 = new PdfDocument(CompareTool.CreateTestPdfWriter(filename1, new WriterProperties().SetFullCompressionMode
                (true)));
            PdfPage page1 = pdfDoc1.AddNewPage();
            page1.GetContentStream(0).GetOutputStream().Write(ByteUtils.GetIsoBytes("%page 1\n"));
            page1.Flush();
            pdfDoc1.Close();
            PdfDocument pdfDoc2 = new PdfDocument(CompareTool.CreateOutputReader(filename1), CompareTool.CreateTestPdfWriter
                (filename2));
            PdfPage page2 = pdfDoc2.AddNewPage();
            page2.GetContentStream(0).GetOutputStream().Write(ByteUtils.GetIsoBytes("%page 2\n"));
            page2.Flush();
            pdfDoc2.Close();
            PdfReader reader3 = CompareTool.CreateOutputReader(filename2);
            PdfDocument pdfDoc3 = new PdfDocument(reader3);
            for (int i = 0; i < pdfDoc3.GetNumberOfPages(); i++) {
                pdfDoc3.GetPage(i + 1);
            }
            NUnit.Framework.Assert.AreEqual(false, reader3.HasRebuiltXref(), "Rebuilt");
            NUnit.Framework.Assert.AreEqual(false, reader3.HasFixedXref(), "Fixed");
            VerifyPdfPagesCount(pdfDoc3.GetCatalog().GetPageTree().GetRoot().GetPdfObject());
            pdfDoc3.Close();
            PdfReader reader = CompareTool.CreateOutputReader(filename2);
            PdfDocument pdfDocument = new PdfDocument(reader);
            NUnit.Framework.Assert.AreEqual(false, reader.HasRebuiltXref(), "Rebuilt");
            byte[] bytes = pdfDocument.GetPage(1).GetContentBytes();
            NUnit.Framework.Assert.AreEqual("%page 1\n", iText.Commons.Utils.JavaUtil.GetStringForBytes(bytes));
            bytes = pdfDocument.GetPage(2).GetContentBytes();
            NUnit.Framework.Assert.AreEqual("%page 2\n", iText.Commons.Utils.JavaUtil.GetStringForBytes(bytes));
            pdfDocument.Close();
        }

        [NUnit.Framework.Test]
        public virtual void Stamping7() {
            String filename1 = destinationFolder + "stamping7_1.pdf";
            String filename2 = destinationFolder + "stamping7_2.pdf";
            PdfDocument pdfDoc1 = new PdfDocument(CompareTool.CreateTestPdfWriter(filename1));
            PdfPage page1 = pdfDoc1.AddNewPage();
            page1.GetContentStream(0).GetOutputStream().Write(ByteUtils.GetIsoBytes("%page 1\n"));
            page1.Flush();
            pdfDoc1.Close();
            PdfReader reader2 = CompareTool.CreateOutputReader(filename1);
            PdfWriter writer2 = CompareTool.CreateTestPdfWriter(filename2, new WriterProperties().SetFullCompressionMode
                (true));
            PdfDocument pdfDoc2 = new PdfDocument(reader2, writer2);
            PdfPage page2 = pdfDoc2.AddNewPage();
            page2.GetContentStream(0).GetOutputStream().Write(ByteUtils.GetIsoBytes("%page 2\n"));
            page2.Flush();
            pdfDoc2.Close();
            PdfReader reader3 = CompareTool.CreateOutputReader(filename2);
            PdfDocument pdfDoc3 = new PdfDocument(reader3);
            for (int i = 0; i < pdfDoc3.GetNumberOfPages(); i++) {
                pdfDoc3.GetPage(i + 1);
            }
            NUnit.Framework.Assert.AreEqual(false, reader3.HasRebuiltXref(), "Rebuilt");
            NUnit.Framework.Assert.AreEqual(false, reader3.HasFixedXref(), "Fixed");
            VerifyPdfPagesCount(pdfDoc3.GetCatalog().GetPageTree().GetRoot().GetPdfObject());
            pdfDoc3.Close();
            PdfReader reader = CompareTool.CreateOutputReader(filename2);
            PdfDocument pdfDocument = new PdfDocument(reader);
            NUnit.Framework.Assert.AreEqual(false, reader.HasRebuiltXref(), "Rebuilt");
            byte[] bytes = pdfDocument.GetPage(1).GetContentBytes();
            NUnit.Framework.Assert.AreEqual("%page 1\n", iText.Commons.Utils.JavaUtil.GetStringForBytes(bytes));
            bytes = pdfDocument.GetPage(2).GetContentBytes();
            NUnit.Framework.Assert.AreEqual("%page 2\n", iText.Commons.Utils.JavaUtil.GetStringForBytes(bytes));
            pdfDocument.Close();
        }

        [NUnit.Framework.Test]
        public virtual void Stamping8() {
            String filename1 = destinationFolder + "stamping8_1.pdf";
            String filename2 = destinationFolder + "stamping8_2.pdf";
            int pageCount = 10;
            PdfWriter writer1 = CompareTool.CreateTestPdfWriter(filename1, new WriterProperties().SetFullCompressionMode
                (true));
            PdfDocument pdfDoc1 = new PdfDocument(writer1);
            for (int i = 1; i <= pageCount; i++) {
                PdfPage page = pdfDoc1.AddNewPage();
                page.GetContentStream(0).GetOutputStream().Write(ByteUtils.GetIsoBytes("%page " + i + "\n"));
                page.Flush();
            }
            pdfDoc1.Close();
            PdfReader reader2 = CompareTool.CreateOutputReader(filename1);
            PdfWriter writer2 = CompareTool.CreateTestPdfWriter(filename2, new WriterProperties().SetFullCompressionMode
                (true));
            PdfDocument pdfDoc2 = new PdfDocument(reader2, writer2);
            pdfDoc2.Close();
            PdfReader reader3 = CompareTool.CreateOutputReader(filename2);
            PdfDocument pdfDoc3 = new PdfDocument(reader3);
            for (int i = 0; i < pdfDoc3.GetNumberOfPages(); i++) {
                pdfDoc3.GetPage(i + 1);
            }
            NUnit.Framework.Assert.AreEqual(pageCount, pdfDoc3.GetNumberOfPages(), "Number of pages");
            NUnit.Framework.Assert.AreEqual(false, reader3.HasRebuiltXref(), "Rebuilt");
            NUnit.Framework.Assert.AreEqual(false, reader3.HasFixedXref(), "Fixed");
            VerifyPdfPagesCount(pdfDoc3.GetCatalog().GetPageTree().GetRoot().GetPdfObject());
            pdfDoc3.Close();
            PdfReader reader = CompareTool.CreateOutputReader(filename2);
            PdfDocument pdfDocument = new PdfDocument(reader);
            NUnit.Framework.Assert.AreEqual(false, reader.HasRebuiltXref(), "Rebuilt");
            for (int i = 1; i <= pageCount; i++) {
                byte[] bytes = pdfDocument.GetPage(i).GetContentBytes();
                NUnit.Framework.Assert.AreEqual("%page " + i + "\n", iText.Commons.Utils.JavaUtil.GetStringForBytes(bytes)
                    , "Page content at page " + i);
            }
            pdfDocument.Close();
        }

        [NUnit.Framework.Test]
        public virtual void Stamping9() {
            String filename1 = destinationFolder + "stamping9_1.pdf";
            String filename2 = destinationFolder + "stamping9_2.pdf";
            int pageCount = 10;
            PdfWriter writer1 = CompareTool.CreateTestPdfWriter(filename1, new WriterProperties().SetFullCompressionMode
                (false));
            PdfDocument pdfDoc1 = new PdfDocument(writer1);
            for (int i = 1; i <= pageCount; i++) {
                PdfPage page = pdfDoc1.AddNewPage();
                page.GetContentStream(0).GetOutputStream().Write(ByteUtils.GetIsoBytes("%page " + i + "\n"));
                page.Flush();
            }
            pdfDoc1.Close();
            PdfReader reader2 = CompareTool.CreateOutputReader(filename1);
            PdfWriter writer2 = CompareTool.CreateTestPdfWriter(filename2, new WriterProperties().SetFullCompressionMode
                (true));
            PdfDocument pdfDoc2 = new PdfDocument(reader2, writer2);
            pdfDoc2.Close();
            PdfReader reader3 = CompareTool.CreateOutputReader(filename2);
            PdfDocument pdfDoc3 = new PdfDocument(reader3);
            for (int i = 0; i < pdfDoc3.GetNumberOfPages(); i++) {
                pdfDoc3.GetPage(i + 1);
            }
            NUnit.Framework.Assert.AreEqual(pageCount, pdfDoc3.GetNumberOfPages(), "Number of pages");
            NUnit.Framework.Assert.AreEqual(false, reader3.HasRebuiltXref(), "Rebuilt");
            NUnit.Framework.Assert.AreEqual(false, reader3.HasFixedXref(), "Fixed");
            VerifyPdfPagesCount(pdfDoc3.GetCatalog().GetPageTree().GetRoot().GetPdfObject());
            pdfDoc3.Close();
            PdfReader reader = CompareTool.CreateOutputReader(filename2);
            PdfDocument pdfDocument = new PdfDocument(reader);
            NUnit.Framework.Assert.AreEqual(false, reader.HasRebuiltXref(), "Rebuilt");
            for (int i = 1; i <= pageCount; i++) {
                byte[] bytes = pdfDocument.GetPage(i).GetContentBytes();
                NUnit.Framework.Assert.AreEqual("%page " + i + "\n", iText.Commons.Utils.JavaUtil.GetStringForBytes(bytes)
                    , "Page content at page " + i);
            }
            pdfDocument.Close();
        }

        [NUnit.Framework.Test]
        public virtual void Stamping10() {
            String filename1 = destinationFolder + "stamping10_1.pdf";
            String filename2 = destinationFolder + "stamping10_2.pdf";
            int pageCount = 10;
            PdfWriter writer1 = CompareTool.CreateTestPdfWriter(filename1, new WriterProperties().SetFullCompressionMode
                (true));
            PdfDocument pdfDoc1 = new PdfDocument(writer1);
            for (int i = 1; i <= pageCount; i++) {
                PdfPage page = pdfDoc1.AddNewPage();
                page.GetContentStream(0).GetOutputStream().Write(ByteUtils.GetIsoBytes("%page " + i + "\n"));
                page.Flush();
            }
            pdfDoc1.Close();
            PdfReader reader2 = CompareTool.CreateOutputReader(filename1);
            PdfWriter writer2 = CompareTool.CreateTestPdfWriter(filename2, new WriterProperties().SetFullCompressionMode
                (false));
            PdfDocument pdfDoc2 = new PdfDocument(reader2, writer2);
            pdfDoc2.Close();
            PdfReader reader3 = CompareTool.CreateOutputReader(filename2);
            PdfDocument pdfDoc3 = new PdfDocument(reader3);
            for (int i = 0; i < pdfDoc3.GetNumberOfPages(); i++) {
                pdfDoc3.GetPage(i + 1);
            }
            NUnit.Framework.Assert.AreEqual(pageCount, pdfDoc3.GetNumberOfPages(), "Number of pages");
            NUnit.Framework.Assert.AreEqual(false, reader3.HasRebuiltXref(), "Rebuilt");
            NUnit.Framework.Assert.AreEqual(false, reader3.HasFixedXref(), "Fixed");
            VerifyPdfPagesCount(pdfDoc3.GetCatalog().GetPageTree().GetRoot().GetPdfObject());
            pdfDoc3.Close();
            PdfReader reader = CompareTool.CreateOutputReader(filename2);
            PdfDocument pdfDocument = new PdfDocument(reader);
            NUnit.Framework.Assert.AreEqual(false, reader.HasRebuiltXref(), "Rebuilt");
            for (int i = 1; i <= pageCount; i++) {
                byte[] bytes = pdfDocument.GetPage(i).GetContentBytes();
                NUnit.Framework.Assert.AreEqual("%page " + i + "\n", iText.Commons.Utils.JavaUtil.GetStringForBytes(bytes)
                    , "Page content at page " + i);
            }
            pdfDocument.Close();
        }

        [NUnit.Framework.Test]
        public virtual void Stamping11() {
            String filename1 = destinationFolder + "stamping11_1.pdf";
            String filename2 = destinationFolder + "stamping11_2.pdf";
            int pageCount = 10;
            PdfWriter writer1 = CompareTool.CreateTestPdfWriter(filename1, new WriterProperties().SetFullCompressionMode
                (false));
            PdfDocument pdfDoc1 = new PdfDocument(writer1);
            for (int i = 1; i <= pageCount; i++) {
                PdfPage page = pdfDoc1.AddNewPage();
                page.GetContentStream(0).GetOutputStream().Write(ByteUtils.GetIsoBytes("%page " + i + "\n"));
                page.Flush();
            }
            pdfDoc1.Close();
            PdfReader reader2 = CompareTool.CreateOutputReader(filename1);
            PdfWriter writer2 = CompareTool.CreateTestPdfWriter(filename2, new WriterProperties().SetFullCompressionMode
                (false));
            PdfDocument pdfDoc2 = new PdfDocument(reader2, writer2);
            pdfDoc2.Close();
            PdfReader reader3 = CompareTool.CreateOutputReader(filename2);
            PdfDocument pdfDoc3 = new PdfDocument(reader3);
            for (int i = 0; i < pdfDoc3.GetNumberOfPages(); i++) {
                pdfDoc3.GetPage(i + 1);
            }
            NUnit.Framework.Assert.AreEqual(pageCount, pdfDoc3.GetNumberOfPages(), "Number of pages");
            NUnit.Framework.Assert.AreEqual(false, reader3.HasRebuiltXref(), "Rebuilt");
            NUnit.Framework.Assert.AreEqual(false, reader3.HasFixedXref(), "Fixed");
            VerifyPdfPagesCount(pdfDoc3.GetCatalog().GetPageTree().GetRoot().GetPdfObject());
            pdfDoc3.Close();
            PdfReader reader = CompareTool.CreateOutputReader(filename2);
            PdfDocument pdfDocument = new PdfDocument(reader);
            NUnit.Framework.Assert.AreEqual(false, reader.HasRebuiltXref(), "Rebuilt");
            for (int i = 1; i <= pageCount; i++) {
                byte[] bytes = pdfDocument.GetPage(i).GetContentBytes();
                NUnit.Framework.Assert.AreEqual("%page " + i + "\n", iText.Commons.Utils.JavaUtil.GetStringForBytes(bytes)
                    , "Page content at page " + i);
            }
            pdfDocument.Close();
        }

        [NUnit.Framework.Test]
        public virtual void Stamping12() {
            String filename1 = destinationFolder + "stamping12_1.pdf";
            String filename2 = destinationFolder + "stamping12_2.pdf";
            int pageCount = 1010;
            PdfDocument pdfDoc1 = new PdfDocument(CompareTool.CreateTestPdfWriter(filename1));
            for (int i = 1; i <= pageCount; i++) {
                PdfPage page = pdfDoc1.AddNewPage();
                page.GetContentStream(0).GetOutputStream().Write(ByteUtils.GetIsoBytes("%page " + i + "\n"));
                page.Flush();
            }
            pdfDoc1.Close();
            PdfDocument pdfDoc2 = new PdfDocument(CompareTool.CreateOutputReader(filename1), CompareTool.CreateTestPdfWriter
                (filename2));
            int newPageCount = 10;
            for (int i = pageCount; i > newPageCount; i--) {
                pdfDoc2.RemovePage(i);
            }
            pdfDoc2.Close();
            PdfReader reader3 = CompareTool.CreateOutputReader(filename2);
            PdfDocument pdfDoc3 = new PdfDocument(reader3);
            for (int i = 1; i <= pdfDoc3.GetNumberOfPages(); i++) {
                pdfDoc3.GetPage(i);
            }
            PdfPage pdfPage = pdfDoc3.GetPage(1);
            PdfDictionary root = pdfPage.GetPdfObject().GetAsDictionary(PdfName.Parent);
            NUnit.Framework.Assert.AreEqual(newPageCount, root.GetAsArray(PdfName.Kids).Size(), "PdfPages kids count");
            NUnit.Framework.Assert.AreEqual(newPageCount, pdfDoc3.GetNumberOfPages(), "Number of pages");
            NUnit.Framework.Assert.AreEqual(false, reader3.HasRebuiltXref(), "Rebuilt");
            NUnit.Framework.Assert.AreEqual(false, reader3.HasFixedXref(), "Fixed");
            VerifyPdfPagesCount(pdfDoc3.GetCatalog().GetPageTree().GetRoot().GetPdfObject());
            pdfDoc3.Close();
            PdfReader reader = CompareTool.CreateOutputReader(filename2);
            PdfDocument pdfDocument = new PdfDocument(reader);
            NUnit.Framework.Assert.AreEqual(false, reader.HasRebuiltXref(), "Rebuilt");
            for (int i = 1; i <= pdfDocument.GetNumberOfPages(); i++) {
                byte[] bytes = pdfDocument.GetPage(i).GetContentBytes();
                NUnit.Framework.Assert.AreEqual("%page " + i + "\n", iText.Commons.Utils.JavaUtil.GetStringForBytes(bytes)
                    , "Page content at page " + i);
            }
            pdfDocument.Close();
        }

        [NUnit.Framework.Test]
        public virtual void Stamping13() {
            String filename1 = destinationFolder + "stamping13_1.pdf";
            String filename2 = destinationFolder + "stamping13_2.pdf";
            int pageCount = 1010;
            PdfDocument pdfDoc1 = new PdfDocument(CompareTool.CreateTestPdfWriter(filename1));
            for (int i = 1; i <= pageCount; i++) {
                PdfPage page = pdfDoc1.AddNewPage();
                page.GetContentStream(0).GetOutputStream().Write(ByteUtils.GetIsoBytes("%page " + i + "\n"));
                page.Flush();
            }
            pdfDoc1.Close();
            PdfDocument pdfDoc2 = new PdfDocument(CompareTool.CreateOutputReader(filename1), CompareTool.CreateTestPdfWriter
                (filename2));
            for (int i = pageCount; i > 1; i--) {
                pdfDoc2.RemovePage(i);
            }
            pdfDoc2.RemovePage(1);
            for (int i = 1; i <= pageCount; i++) {
                PdfPage page = pdfDoc2.AddNewPage();
                page.GetContentStream(0).GetOutputStream().Write(ByteUtils.GetIsoBytes("%page " + i + "\n"));
                page.Flush();
            }
            pdfDoc2.Close();
            PdfReader reader3 = CompareTool.CreateOutputReader(filename2);
            PdfDocument pdfDoc3 = new PdfDocument(reader3);
            for (int i = 1; i <= pdfDoc3.GetNumberOfPages(); i++) {
                pdfDoc3.GetPage(i);
            }
            PdfArray rootKids = pdfDoc3.GetCatalog().GetPageTree().GetRoot().GetPdfObject().GetAsArray(PdfName.Kids);
            NUnit.Framework.Assert.AreEqual(2, rootKids.Size(), "Page root kids count");
            NUnit.Framework.Assert.AreEqual(pageCount, pdfDoc3.GetNumberOfPages(), "Number of pages");
            NUnit.Framework.Assert.AreEqual(false, reader3.HasRebuiltXref(), "Rebuilt");
            NUnit.Framework.Assert.AreEqual(false, reader3.HasFixedXref(), "Fixed");
            VerifyPdfPagesCount(pdfDoc3.GetCatalog().GetPageTree().GetRoot().GetPdfObject());
            pdfDoc3.Close();
            PdfReader reader = CompareTool.CreateOutputReader(filename2);
            PdfDocument pdfDocument = new PdfDocument(reader);
            NUnit.Framework.Assert.AreEqual(false, reader.HasRebuiltXref(), "Rebuilt");
            for (int i = 1; i <= pageCount; i++) {
                byte[] bytes = pdfDocument.GetPage(i).GetContentBytes();
                NUnit.Framework.Assert.AreEqual("%page " + i + "\n", iText.Commons.Utils.JavaUtil.GetStringForBytes(bytes)
                    , "Page content at page " + i);
            }
            pdfDocument.Close();
        }

        [NUnit.Framework.Test]
        public virtual void Stamping14() {
            String filename1 = sourceFolder + "20000PagesDocument.pdf";
            String filename2 = destinationFolder + "stamping14.pdf";
            PdfDocument pdfDoc2 = new PdfDocument(new PdfReader(filename1), CompareTool.CreateTestPdfWriter(filename2)
                );
            for (int i = pdfDoc2.GetNumberOfPages(); i > 3; i--) {
                pdfDoc2.RemovePage(i);
            }
            pdfDoc2.Close();
            PdfReader reader3 = CompareTool.CreateOutputReader(filename2);
            PdfDocument pdfDoc3 = new PdfDocument(reader3);
            for (int i = 1; i <= pdfDoc3.GetNumberOfPages(); i++) {
                pdfDoc3.GetPage(i);
            }
            //NOTE: during page removing iText don't flatten page structure (we can end up with a lot of embedded pages dictionaries)
            NUnit.Framework.Assert.AreEqual(42226, pdfDoc3.GetXref().Size(), "Xref size");
            NUnit.Framework.Assert.AreEqual(3, pdfDoc3.GetNumberOfPages(), "Number of pages");
            NUnit.Framework.Assert.IsFalse(reader3.HasRebuiltXref(), "Rebuilt");
            NUnit.Framework.Assert.IsFalse(reader3.HasFixedXref(), "Fixed");
            VerifyPdfPagesCount(pdfDoc3.GetCatalog().GetPageTree().GetRoot().GetPdfObject());
            pdfDoc3.Close();
            PdfReader reader = CompareTool.CreateOutputReader(filename2);
            PdfDocument pdfDocument = new PdfDocument(reader);
            NUnit.Framework.Assert.AreEqual(false, reader.HasRebuiltXref(), "Rebuilt");
            for (int i = 1; i <= pdfDocument.GetNumberOfPages(); i++) {
                byte[] bytes = pdfDocument.GetPage(i).GetContentBytes();
                NUnit.Framework.Assert.AreEqual("%page " + i + "\n", iText.Commons.Utils.JavaUtil.GetStringForBytes(bytes)
                    , "Page content at page " + i);
            }
            pdfDocument.Close();
        }

        [NUnit.Framework.Test]
        public virtual void StampingStreamsCompression01() {
            // by default, old streams should not be recompressed
            String filenameIn = sourceFolder + "stampingStreamsCompression.pdf";
            String filenameOut = destinationFolder + "stampingStreamsCompression01.pdf";
            PdfReader reader = new PdfReader(filenameIn);
            PdfWriter writer = CompareTool.CreateTestPdfWriter(filenameOut);
            writer.SetCompressionLevel(CompressionConstants.BEST_COMPRESSION);
            PdfDocument doc = new PdfDocument(reader, writer);
            PdfStream stream = (PdfStream)doc.GetPdfObject(6);
            int lengthBefore = stream.GetLength();
            doc.Close();
            doc = new PdfDocument(CompareTool.CreateOutputReader(filenameOut));
            stream = (PdfStream)doc.GetPdfObject(6);
            int lengthAfter = stream.GetLength();
            NUnit.Framework.Assert.AreEqual(5731884, lengthBefore);
            float expected = 5731884;
            float coef = Math.Abs((expected - lengthAfter) / expected);
            NUnit.Framework.Assert.IsTrue(coef < 0.01);
        }

        [NUnit.Framework.Test]
        public virtual void StampingStreamsCompression02() {
            // if user specified, stream may be uncompressed
            String filenameIn = sourceFolder + "stampingStreamsCompression.pdf";
            String filenameOut = destinationFolder + "stampingStreamsCompression02.pdf";
            PdfReader reader = new PdfReader(filenameIn);
            PdfWriter writer = CompareTool.CreateTestPdfWriter(filenameOut);
            PdfDocument doc = new PdfDocument(reader, writer);
            PdfStream stream = (PdfStream)doc.GetPdfObject(6);
            int lengthBefore = stream.GetLength();
            stream.SetCompressionLevel(CompressionConstants.NO_COMPRESSION);
            doc.Close();
            doc = new PdfDocument(CompareTool.CreateOutputReader(filenameOut));
            stream = (PdfStream)doc.GetPdfObject(6);
            int lengthAfter = stream.GetLength();
            NUnit.Framework.Assert.AreEqual(5731884, lengthBefore);
            float expected = 11321910;
            float coef = Math.Abs((expected - lengthAfter) / expected);
            NUnit.Framework.Assert.IsTrue(coef < 0.01);
        }

        [NUnit.Framework.Test]
        public virtual void StampingStreamsCompression03() {
            // if user specified, stream may be recompressed
            String filenameIn = sourceFolder + "stampingStreamsCompression.pdf";
            String filenameOut = destinationFolder + "stampingStreamsCompression03.pdf";
            PdfDocument doc = new PdfDocument(new PdfReader(filenameIn), CompareTool.CreateTestPdfWriter(filenameOut));
            PdfStream stream = (PdfStream)doc.GetPdfObject(6);
            int lengthBefore = stream.GetLength();
            stream.SetCompressionLevel(CompressionConstants.BEST_COMPRESSION);
            doc.Close();
            doc = new PdfDocument(CompareTool.CreateOutputReader(filenameOut));
            stream = (PdfStream)doc.GetPdfObject(6);
            int lengthAfter = stream.GetLength();
            NUnit.Framework.Assert.AreEqual(5731884, lengthBefore);
            float expected = 5729270;
            float coef = Math.Abs((expected - lengthAfter) / expected);
            NUnit.Framework.Assert.IsTrue(coef < 0.01);
        }

        [NUnit.Framework.Test]
        public virtual void StampingXmp1() {
            String filename1 = destinationFolder + "stampingXmp1_1.pdf";
            String filename2 = destinationFolder + "stampingXmp1_2.pdf";
            int pageCount = 10;
            PdfWriter writer1 = CompareTool.CreateTestPdfWriter(filename1, new WriterProperties().SetFullCompressionMode
                (true));
            PdfDocument pdfDoc1 = new PdfDocument(writer1);
            for (int i = 1; i <= pageCount; i++) {
                PdfPage page = pdfDoc1.AddNewPage();
                page.GetContentStream(0).GetOutputStream().Write(ByteUtils.GetIsoBytes("%page " + i + "\n"));
                page.Flush();
            }
            pdfDoc1.Close();
            PdfReader reader2 = CompareTool.CreateOutputReader(filename1);
            PdfWriter writer2 = CompareTool.CreateTestPdfWriter(filename2, new WriterProperties().SetFullCompressionMode
                (false).AddXmpMetadata());
            PdfDocument pdfDoc2 = new PdfDocument(reader2, writer2);
            pdfDoc2.GetDocumentInfo().SetAuthor("Alexander Chingarev");
            pdfDoc2.Close();
            PdfReader reader3 = CompareTool.CreateOutputReader(filename2);
            PdfDocument pdfDoc3 = new PdfDocument(reader3);
            for (int i = 0; i < pdfDoc3.GetNumberOfPages(); i++) {
                pdfDoc3.GetPage(i + 1);
            }
            NUnit.Framework.Assert.IsNotNull(XMPMetaFactory.ParseFromBuffer(pdfDoc3.GetXmpMetadata()), "XmpMetadata not found"
                );
            NUnit.Framework.Assert.AreEqual(pageCount, pdfDoc3.GetNumberOfPages(), "Number of pages");
            NUnit.Framework.Assert.AreEqual(false, reader3.HasRebuiltXref(), "Rebuilt");
            NUnit.Framework.Assert.AreEqual(false, reader3.HasFixedXref(), "Fixed");
            VerifyPdfPagesCount(pdfDoc3.GetCatalog().GetPageTree().GetRoot().GetPdfObject());
            pdfDoc3.Close();
            PdfReader reader = CompareTool.CreateOutputReader(filename2);
            PdfDocument pdfDocument = new PdfDocument(reader);
            NUnit.Framework.Assert.AreEqual(false, reader.HasRebuiltXref(), "Rebuilt");
            for (int i = 1; i <= pageCount; i++) {
                byte[] bytes = pdfDocument.GetPage(i).GetContentBytes();
                NUnit.Framework.Assert.AreEqual("%page " + i + "\n", iText.Commons.Utils.JavaUtil.GetStringForBytes(bytes)
                    , "Page content at page " + i);
            }
            pdfDocument.Close();
        }

        [NUnit.Framework.Test]
        public virtual void StampingXmp2() {
            String filename1 = destinationFolder + "stampingXmp2_1.pdf";
            String filename2 = destinationFolder + "stampingXmp2_2.pdf";
            int pageCount = 10;
            PdfWriter writer1 = CompareTool.CreateTestPdfWriter(filename1, new WriterProperties().SetFullCompressionMode
                (false));
            PdfDocument pdfDoc1 = new PdfDocument(writer1);
            for (int i = 1; i <= pageCount; i++) {
                PdfPage page = pdfDoc1.AddNewPage();
                page.GetContentStream(0).GetOutputStream().Write(ByteUtils.GetIsoBytes("%page " + i + "\n"));
                page.Flush();
            }
            pdfDoc1.Close();
            PdfReader reader2 = CompareTool.CreateOutputReader(filename1);
            PdfWriter writer2 = CompareTool.CreateTestPdfWriter(filename2, new WriterProperties().SetFullCompressionMode
                (true).AddXmpMetadata());
            PdfDocument pdfDoc2 = new PdfDocument(reader2, writer2);
            pdfDoc2.GetDocumentInfo().SetAuthor("Alexander Chingarev");
            pdfDoc2.Close();
            PdfReader reader3 = CompareTool.CreateOutputReader(filename2);
            PdfDocument pdfDoc3 = new PdfDocument(reader3);
            for (int i = 0; i < pdfDoc3.GetNumberOfPages(); i++) {
                pdfDoc3.GetPage(i + 1);
            }
            NUnit.Framework.Assert.IsNotNull(XMPMetaFactory.ParseFromBuffer(pdfDoc3.GetXmpMetadata()), "XmpMetadata not found"
                );
            NUnit.Framework.Assert.AreEqual(pageCount, pdfDoc3.GetNumberOfPages(), "Number of pages");
            NUnit.Framework.Assert.AreEqual(false, reader3.HasRebuiltXref(), "Rebuilt");
            NUnit.Framework.Assert.AreEqual(false, reader3.HasFixedXref(), "Fixed");
            VerifyPdfPagesCount(pdfDoc3.GetCatalog().GetPageTree().GetRoot().GetPdfObject());
            pdfDoc3.Close();
            PdfReader reader = CompareTool.CreateOutputReader(filename2);
            PdfDocument pdfDocument = new PdfDocument(reader);
            NUnit.Framework.Assert.AreEqual(false, reader.HasRebuiltXref(), "Rebuilt");
            for (int i = 1; i <= pageCount; i++) {
                byte[] bytes = pdfDocument.GetPage(i).GetContentBytes();
                NUnit.Framework.Assert.AreEqual("%page " + i + "\n", iText.Commons.Utils.JavaUtil.GetStringForBytes(bytes)
                    , "Page content at page " + i);
            }
            pdfDocument.Close();
        }

        [NUnit.Framework.Test]
        public virtual void StampingAppend1() {
            String filename1 = destinationFolder + "stampingAppend1_1.pdf";
            String filename2 = destinationFolder + "stampingAppend1_2.pdf";
            PdfDocument pdfDoc1 = new PdfDocument(CompareTool.CreateTestPdfWriter(filename1));
            pdfDoc1.GetDocumentInfo().SetAuthor("Alexander Chingarev").SetCreator("iText 6").SetTitle("Empty iText 6 Document"
                );
            PdfPage page1 = pdfDoc1.AddNewPage();
            page1.GetContentStream(0).GetOutputStream().Write(ByteUtils.GetIsoBytes("%Hello World\n"));
            page1.Flush();
            pdfDoc1.Close();
            PdfDocument pdfDoc2 = new PdfDocument(CompareTool.CreateOutputReader(filename1), CompareTool.CreateTestPdfWriter
                (filename2), new StampingProperties().UseAppendMode());
            pdfDoc2.GetDocumentInfo().SetCreator("iText").SetTitle("Empty iText Document");
            pdfDoc2.Close();
            PdfReader reader3 = CompareTool.CreateOutputReader(filename2);
            PdfDocument pdfDoc3 = new PdfDocument(reader3);
            for (int i = 0; i < pdfDoc3.GetNumberOfPages(); i++) {
                pdfDoc3.GetPage(i + 1);
            }
            NUnit.Framework.Assert.AreEqual(false, reader3.HasRebuiltXref(), "Rebuilt");
            NUnit.Framework.Assert.AreEqual(false, reader3.HasFixedXref(), "Fixed");
            VerifyPdfPagesCount(pdfDoc3.GetCatalog().GetPageTree().GetRoot().GetPdfObject());
            pdfDoc3.Close();
            PdfReader reader = CompareTool.CreateOutputReader(filename2);
            PdfDocument pdfDocument = new PdfDocument(reader);
            NUnit.Framework.Assert.AreEqual(false, reader.HasRebuiltXref(), "Rebuilt");
            PdfDictionary trailer = pdfDocument.GetTrailer();
            PdfDictionary info = trailer.GetAsDictionary(PdfName.Info);
            PdfString creator = info.GetAsString(PdfName.Creator);
            NUnit.Framework.Assert.AreEqual("iText", creator.ToString());
            byte[] bytes = pdfDocument.GetPage(1).GetContentBytes();
            NUnit.Framework.Assert.AreEqual("%Hello World\n", iText.Commons.Utils.JavaUtil.GetStringForBytes(bytes));
            String date = pdfDocument.GetDocumentInfo().GetPdfObject().GetAsString(PdfName.ModDate).GetValue();
            DateTime cl = PdfDate.Decode(date);
            double diff = DateTimeUtil.GetUtcMillisFromEpoch(null) - DateTimeUtil.GetUtcMillisFromEpoch(cl);
            String message = "Unexpected creation date. Different from now is " + (float)diff / 1000 + "s";
            NUnit.Framework.Assert.IsTrue(diff < 5000, message);
            pdfDocument.Close();
        }

        [NUnit.Framework.Test]
        public virtual void StampingAppend2() {
            String filename1 = destinationFolder + "stampingAppend2_1.pdf";
            String filename2 = destinationFolder + "stampingAppend2_2.pdf";
            PdfDocument pdfDoc1 = new PdfDocument(CompareTool.CreateTestPdfWriter(filename1));
            PdfPage page1 = pdfDoc1.AddNewPage();
            page1.GetContentStream(0).GetOutputStream().Write(ByteUtils.GetIsoBytes("%page 1\n"));
            page1.Flush();
            pdfDoc1.Close();
            PdfDocument pdfDoc2 = new PdfDocument(CompareTool.CreateOutputReader(filename1), CompareTool.CreateTestPdfWriter
                (filename2), new StampingProperties().UseAppendMode());
            PdfPage page2 = pdfDoc2.AddNewPage();
            page2.GetContentStream(0).GetOutputStream().Write(ByteUtils.GetIsoBytes("%page 2\n"));
            page2.SetModified();
            page2.Flush();
            pdfDoc2.Close();
            PdfReader reader3 = CompareTool.CreateOutputReader(filename2);
            PdfDocument pdfDoc3 = new PdfDocument(reader3);
            for (int i = 0; i < pdfDoc3.GetNumberOfPages(); i++) {
                pdfDoc3.GetPage(i + 1);
            }
            NUnit.Framework.Assert.AreEqual(false, reader3.HasRebuiltXref(), "Rebuilt");
            NUnit.Framework.Assert.AreEqual(false, reader3.HasFixedXref(), "Fixed");
            VerifyPdfPagesCount(pdfDoc3.GetCatalog().GetPageTree().GetRoot().GetPdfObject());
            pdfDoc3.Close();
            PdfReader reader = CompareTool.CreateOutputReader(filename2);
            PdfDocument pdfDocument = new PdfDocument(reader);
            NUnit.Framework.Assert.AreEqual(false, reader.HasRebuiltXref(), "Rebuilt");
            byte[] bytes = pdfDocument.GetPage(1).GetContentBytes();
            NUnit.Framework.Assert.AreEqual("%page 1\n", iText.Commons.Utils.JavaUtil.GetStringForBytes(bytes));
            bytes = pdfDocument.GetPage(2).GetContentBytes();
            NUnit.Framework.Assert.AreEqual("%page 2\n", iText.Commons.Utils.JavaUtil.GetStringForBytes(bytes));
            pdfDocument.Close();
        }

        [NUnit.Framework.Test]
        public virtual void StampingAppend3() {
            String filename1 = destinationFolder + "stampingAppend3_1.pdf";
            String filename2 = destinationFolder + "stampingAppend3_2.pdf";
            PdfWriter writer1 = CompareTool.CreateTestPdfWriter(filename1, new WriterProperties().SetFullCompressionMode
                (true));
            PdfDocument pdfDoc1 = new PdfDocument(writer1);
            PdfPage page1 = pdfDoc1.AddNewPage();
            page1.GetContentStream(0).GetOutputStream().Write(ByteUtils.GetIsoBytes("%page 1\n"));
            page1.Flush();
            pdfDoc1.Close();
            PdfDocument pdfDoc2 = new PdfDocument(CompareTool.CreateOutputReader(filename1), CompareTool.CreateTestPdfWriter
                (filename2), new StampingProperties().UseAppendMode());
            PdfPage page2 = pdfDoc2.AddNewPage();
            page2.GetContentStream(0).GetOutputStream().Write(ByteUtils.GetIsoBytes("%page 2\n"));
            page2.Flush();
            pdfDoc2.Close();
            PdfReader reader3 = CompareTool.CreateOutputReader(filename2);
            PdfDocument pdfDoc3 = new PdfDocument(reader3);
            for (int i = 0; i < pdfDoc3.GetNumberOfPages(); i++) {
                pdfDoc3.GetPage(i + 1);
            }
            NUnit.Framework.Assert.AreEqual(false, reader3.HasRebuiltXref(), "Rebuilt");
            NUnit.Framework.Assert.AreEqual(false, reader3.HasFixedXref(), "Fixed");
            VerifyPdfPagesCount(pdfDoc3.GetCatalog().GetPageTree().GetRoot().GetPdfObject());
            pdfDoc3.Close();
            PdfReader reader = CompareTool.CreateOutputReader(filename2);
            PdfDocument pdfDocument = new PdfDocument(reader);
            NUnit.Framework.Assert.AreEqual(false, reader.HasRebuiltXref(), "Rebuilt");
            byte[] bytes = pdfDocument.GetPage(1).GetContentBytes();
            NUnit.Framework.Assert.AreEqual("%page 1\n", iText.Commons.Utils.JavaUtil.GetStringForBytes(bytes));
            bytes = pdfDocument.GetPage(2).GetContentBytes();
            NUnit.Framework.Assert.AreEqual("%page 2\n", iText.Commons.Utils.JavaUtil.GetStringForBytes(bytes));
            pdfDocument.Close();
        }

        [NUnit.Framework.Test]
        public virtual void StampingAppend4() {
            String filename1 = destinationFolder + "stampingAppend4_1.pdf";
            String filename2 = destinationFolder + "stampingAppend4_2.pdf";
            PdfDocument pdfDoc1 = new PdfDocument(CompareTool.CreateTestPdfWriter(filename1));
            PdfPage page1 = pdfDoc1.AddNewPage();
            page1.GetContentStream(0).GetOutputStream().Write(ByteUtils.GetIsoBytes("%page 1\n"));
            page1.Flush();
            pdfDoc1.Close();
            int pageCount = 15;
            PdfDocument pdfDoc2 = new PdfDocument(CompareTool.CreateOutputReader(filename1), CompareTool.CreateTestPdfWriter
                (filename2), new StampingProperties().UseAppendMode());
            for (int i = 2; i <= pageCount; i++) {
                PdfPage page2 = pdfDoc2.AddNewPage();
                page2.GetContentStream(0).GetOutputStream().Write(ByteUtils.GetIsoBytes("%page " + i + "\n"));
                page2.Flush();
            }
            pdfDoc2.Close();
            PdfReader reader3 = CompareTool.CreateOutputReader(filename2);
            PdfDocument pdfDoc3 = new PdfDocument(reader3);
            for (int i = 0; i < pdfDoc3.GetNumberOfPages(); i++) {
                pdfDoc3.GetPage(i + 1);
            }
            NUnit.Framework.Assert.AreEqual(false, reader3.HasRebuiltXref(), "Rebuilt");
            NUnit.Framework.Assert.AreEqual(false, reader3.HasFixedXref(), "Fixed");
            VerifyPdfPagesCount(pdfDoc3.GetCatalog().GetPageTree().GetRoot().GetPdfObject());
            pdfDoc3.Close();
            PdfReader reader = CompareTool.CreateOutputReader(filename2);
            PdfDocument pdfDocument = new PdfDocument(reader);
            NUnit.Framework.Assert.AreEqual(false, reader.HasRebuiltXref(), "Rebuilt");
            NUnit.Framework.Assert.AreEqual(pageCount, pdfDocument.GetNumberOfPages(), "Page count");
            for (int i = 1; i < pdfDocument.GetNumberOfPages(); i++) {
                byte[] bytes = pdfDocument.GetPage(i).GetContentBytes();
                NUnit.Framework.Assert.AreEqual("%page " + i + "\n", iText.Commons.Utils.JavaUtil.GetStringForBytes(bytes)
                    );
            }
            pdfDocument.Close();
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.FULL_COMPRESSION_APPEND_MODE_XREF_TABLE_INCONSISTENCY)]
        public virtual void StampingAppend5() {
            String filename1 = destinationFolder + "stampingAppend5_1.pdf";
            String filename2 = destinationFolder + "stampingAppend5_2.pdf";
            PdfDocument pdfDoc1 = new PdfDocument(CompareTool.CreateTestPdfWriter(filename1));
            PdfPage page1 = pdfDoc1.AddNewPage();
            page1.GetContentStream(0).GetOutputStream().Write(ByteUtils.GetIsoBytes("%page 1\n"));
            page1.Flush();
            pdfDoc1.Close();
            int pageCount = 15;
            PdfReader reader2 = CompareTool.CreateOutputReader(filename1);
            PdfWriter writer2 = CompareTool.CreateTestPdfWriter(filename2, new WriterProperties().SetFullCompressionMode
                (true));
            PdfDocument pdfDoc2 = new PdfDocument(reader2, writer2, new StampingProperties().UseAppendMode());
            for (int i = 2; i <= pageCount; i++) {
                PdfPage page2 = pdfDoc2.AddNewPage();
                page2.GetContentStream(0).GetOutputStream().Write(ByteUtils.GetIsoBytes("%page " + i + "\n"));
                page2.Flush();
            }
            pdfDoc2.Close();
            PdfReader reader3 = CompareTool.CreateOutputReader(filename2);
            PdfDocument pdfDoc3 = new PdfDocument(reader3);
            for (int i = 0; i < pdfDoc3.GetNumberOfPages(); i++) {
                pdfDoc3.GetPage(i + 1);
            }
            NUnit.Framework.Assert.AreEqual(false, reader3.HasRebuiltXref(), "Rebuilt");
            NUnit.Framework.Assert.AreEqual(false, reader3.HasFixedXref(), "Fixed");
            VerifyPdfPagesCount(pdfDoc3.GetCatalog().GetPageTree().GetRoot().GetPdfObject());
            pdfDoc3.Close();
            PdfReader reader = CompareTool.CreateOutputReader(filename2);
            PdfDocument pdfDocument = new PdfDocument(reader);
            NUnit.Framework.Assert.AreEqual(false, reader.HasRebuiltXref(), "Rebuilt");
            NUnit.Framework.Assert.AreEqual(pageCount, pdfDocument.GetNumberOfPages(), "Page count");
            for (int i = 1; i < pdfDocument.GetNumberOfPages(); i++) {
                byte[] bytes = pdfDocument.GetPage(i).GetContentBytes();
                NUnit.Framework.Assert.AreEqual("%page " + i + "\n", iText.Commons.Utils.JavaUtil.GetStringForBytes(bytes)
                    );
            }
            pdfDocument.Close();
        }

        [NUnit.Framework.Test]
        public virtual void StampingAppend8() {
            String filename1 = destinationFolder + "stampingAppend8_1.pdf";
            String filename2 = destinationFolder + "stampingAppend8_2.pdf";
            int pageCount = 10;
            PdfWriter writer1 = CompareTool.CreateTestPdfWriter(filename1, new WriterProperties().SetFullCompressionMode
                (true));
            PdfDocument pdfDoc1 = new PdfDocument(writer1);
            for (int i = 1; i <= pageCount; i++) {
                PdfPage page = pdfDoc1.AddNewPage();
                page.GetContentStream(0).GetOutputStream().Write(ByteUtils.GetIsoBytes("%page " + i + "\n"));
                page.Flush();
            }
            pdfDoc1.Close();
            PdfDocument pdfDoc2 = new PdfDocument(CompareTool.CreateOutputReader(filename1), CompareTool.CreateTestPdfWriter
                (filename2), new StampingProperties().UseAppendMode());
            pdfDoc2.Close();
            PdfReader reader3 = CompareTool.CreateOutputReader(filename2);
            PdfDocument pdfDoc3 = new PdfDocument(reader3);
            for (int i = 0; i < pdfDoc3.GetNumberOfPages(); i++) {
                pdfDoc3.GetPage(i + 1);
            }
            NUnit.Framework.Assert.AreEqual(pageCount, pdfDoc3.GetNumberOfPages(), "Number of pages");
            NUnit.Framework.Assert.AreEqual(false, reader3.HasRebuiltXref(), "Rebuilt");
            NUnit.Framework.Assert.AreEqual(false, reader3.HasFixedXref(), "Fixed");
            VerifyPdfPagesCount(pdfDoc3.GetCatalog().GetPageTree().GetRoot().GetPdfObject());
            pdfDoc3.Close();
            PdfReader reader = CompareTool.CreateOutputReader(filename2);
            PdfDocument pdfDocument = new PdfDocument(reader);
            NUnit.Framework.Assert.AreEqual(false, reader.HasRebuiltXref(), "Rebuilt");
            for (int i = 1; i <= pageCount; i++) {
                byte[] bytes = pdfDocument.GetPage(i).GetContentBytes();
                NUnit.Framework.Assert.AreEqual("%page " + i + "\n", iText.Commons.Utils.JavaUtil.GetStringForBytes(bytes)
                    , "Page content at page " + i);
            }
            pdfDocument.Close();
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.FULL_COMPRESSION_APPEND_MODE_XREF_TABLE_INCONSISTENCY)]
        public virtual void StampingAppend9() {
            String filename1 = destinationFolder + "stampingAppend9_1.pdf";
            String filename2 = destinationFolder + "stampingAppend9_2.pdf";
            int pageCount = 10;
            PdfWriter writer1 = CompareTool.CreateTestPdfWriter(filename1, new WriterProperties().SetFullCompressionMode
                (false));
            PdfDocument pdfDoc1 = new PdfDocument(writer1);
            for (int i = 1; i <= pageCount; i++) {
                PdfPage page = pdfDoc1.AddNewPage();
                page.GetContentStream(0).GetOutputStream().Write(ByteUtils.GetIsoBytes("%page " + i + "\n"));
                page.Flush();
            }
            pdfDoc1.Close();
            PdfReader reader2 = CompareTool.CreateOutputReader(filename1);
            PdfWriter writer2 = CompareTool.CreateTestPdfWriter(filename2, new WriterProperties().SetFullCompressionMode
                (true));
            PdfDocument pdfDoc2 = new PdfDocument(reader2, writer2, new StampingProperties().UseAppendMode());
            pdfDoc2.Close();
            PdfReader reader3 = CompareTool.CreateOutputReader(filename2);
            PdfDocument pdfDoc3 = new PdfDocument(reader3);
            for (int i = 0; i < pdfDoc3.GetNumberOfPages(); i++) {
                pdfDoc3.GetPage(i + 1);
            }
            NUnit.Framework.Assert.AreEqual(pageCount, pdfDoc3.GetNumberOfPages(), "Number of pages");
            NUnit.Framework.Assert.AreEqual(false, reader3.HasRebuiltXref(), "Rebuilt");
            NUnit.Framework.Assert.AreEqual(false, reader3.HasFixedXref(), "Fixed");
            VerifyPdfPagesCount(pdfDoc3.GetCatalog().GetPageTree().GetRoot().GetPdfObject());
            pdfDoc3.Close();
            PdfReader reader = CompareTool.CreateOutputReader(filename2);
            PdfDocument pdfDocument = new PdfDocument(reader);
            NUnit.Framework.Assert.AreEqual(false, reader.HasRebuiltXref(), "Rebuilt");
            for (int i = 1; i <= pageCount; i++) {
                byte[] bytes = pdfDocument.GetPage(i).GetContentBytes();
                NUnit.Framework.Assert.AreEqual("%page " + i + "\n", iText.Commons.Utils.JavaUtil.GetStringForBytes(bytes)
                    , "Page content at page " + i);
            }
            pdfDocument.Close();
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.FULL_COMPRESSION_APPEND_MODE_XREF_STREAM_INCONSISTENCY)]
        public virtual void StampingAppend10() {
            String filename1 = destinationFolder + "stampingAppend10_1.pdf";
            String filename2 = destinationFolder + "stampingAppend10_2.pdf";
            int pageCount = 10;
            PdfWriter writer1 = CompareTool.CreateTestPdfWriter(filename1, new WriterProperties().SetFullCompressionMode
                (true));
            PdfDocument pdfDoc1 = new PdfDocument(writer1);
            for (int i = 1; i <= pageCount; i++) {
                PdfPage page = pdfDoc1.AddNewPage();
                page.GetContentStream(0).GetOutputStream().Write(ByteUtils.GetIsoBytes("%page " + i + "\n"));
                page.Flush();
            }
            pdfDoc1.Close();
            PdfReader reader2 = CompareTool.CreateOutputReader(filename1);
            PdfWriter writer2 = CompareTool.CreateTestPdfWriter(filename2, new WriterProperties().SetFullCompressionMode
                (false));
            PdfDocument pdfDoc2 = new PdfDocument(reader2, writer2, new StampingProperties().UseAppendMode());
            pdfDoc2.Close();
            PdfReader reader3 = CompareTool.CreateOutputReader(filename2);
            PdfDocument pdfDoc3 = new PdfDocument(reader3);
            for (int i = 0; i < pdfDoc3.GetNumberOfPages(); i++) {
                pdfDoc3.GetPage(i + 1);
            }
            NUnit.Framework.Assert.AreEqual(pageCount, pdfDoc3.GetNumberOfPages(), "Number of pages");
            NUnit.Framework.Assert.AreEqual(false, reader3.HasRebuiltXref(), "Rebuilt");
            NUnit.Framework.Assert.AreEqual(false, reader3.HasFixedXref(), "Fixed");
            VerifyPdfPagesCount(pdfDoc3.GetCatalog().GetPageTree().GetRoot().GetPdfObject());
            pdfDoc3.Close();
            PdfReader reader = CompareTool.CreateOutputReader(filename2);
            PdfDocument pdfDocument = new PdfDocument(reader);
            NUnit.Framework.Assert.AreEqual(false, reader.HasRebuiltXref(), "Rebuilt");
            for (int i = 1; i <= pageCount; i++) {
                byte[] bytes = pdfDocument.GetPage(i).GetContentBytes();
                NUnit.Framework.Assert.AreEqual("%page " + i + "\n", iText.Commons.Utils.JavaUtil.GetStringForBytes(bytes)
                    , "Page content at page " + i);
            }
            pdfDocument.Close();
        }

        [NUnit.Framework.Test]
        public virtual void StampingAppend11() {
            String filename1 = destinationFolder + "stampingAppend11_1.pdf";
            String filename2 = destinationFolder + "stampingAppend11_2.pdf";
            int pageCount = 10;
            PdfWriter writer1 = CompareTool.CreateTestPdfWriter(filename1, new WriterProperties().SetFullCompressionMode
                (false));
            PdfDocument pdfDoc1 = new PdfDocument(writer1);
            for (int i = 1; i <= pageCount; i++) {
                PdfPage page = pdfDoc1.AddNewPage();
                page.GetContentStream(0).GetOutputStream().Write(ByteUtils.GetIsoBytes("%page " + i + "\n"));
                page.Flush();
            }
            pdfDoc1.Close();
            PdfReader reader2 = CompareTool.CreateOutputReader(filename1);
            PdfWriter writer2 = CompareTool.CreateTestPdfWriter(filename2, new WriterProperties().SetFullCompressionMode
                (false));
            PdfDocument pdfDoc2 = new PdfDocument(reader2, writer2, new StampingProperties().UseAppendMode());
            pdfDoc2.Close();
            PdfReader reader3 = CompareTool.CreateOutputReader(filename2);
            PdfDocument pdfDoc3 = new PdfDocument(reader3);
            for (int i = 0; i < pdfDoc3.GetNumberOfPages(); i++) {
                pdfDoc3.GetPage(i + 1);
            }
            NUnit.Framework.Assert.AreEqual(pageCount, pdfDoc3.GetNumberOfPages(), "Number of pages");
            NUnit.Framework.Assert.AreEqual(false, reader3.HasRebuiltXref(), "Rebuilt");
            NUnit.Framework.Assert.AreEqual(false, reader3.HasFixedXref(), "Fixed");
            VerifyPdfPagesCount(pdfDoc3.GetCatalog().GetPageTree().GetRoot().GetPdfObject());
            pdfDoc3.Close();
            PdfReader reader = CompareTool.CreateOutputReader(filename2);
            PdfDocument pdfDocument = new PdfDocument(reader);
            NUnit.Framework.Assert.AreEqual(false, reader.HasRebuiltXref(), "Rebuilt");
            for (int i = 1; i <= pageCount; i++) {
                byte[] bytes = pdfDocument.GetPage(i).GetContentBytes();
                NUnit.Framework.Assert.AreEqual("%page " + i + "\n", iText.Commons.Utils.JavaUtil.GetStringForBytes(bytes)
                    , "Page content at page " + i);
            }
            pdfDocument.Close();
        }

        [NUnit.Framework.Test]
        public virtual void StampingVersionTest01() {
            // By default the version of the output file should be the same as the original one
            String @in = sourceFolder + "hello.pdf";
            String @out = destinationFolder + "hello_stamped01.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(@in), CompareTool.CreateTestPdfWriter(@out));
            NUnit.Framework.Assert.AreEqual(PdfVersion.PDF_1_4, pdfDoc.GetPdfVersion());
            pdfDoc.Close();
            PdfDocument assertPdfDoc = new PdfDocument(CompareTool.CreateOutputReader(@out));
            NUnit.Framework.Assert.AreEqual(PdfVersion.PDF_1_4, assertPdfDoc.GetPdfVersion());
            assertPdfDoc.Close();
        }

        [NUnit.Framework.Test]
        public virtual void StampingVersionTest02() {
            // There is a possibility to override version in stamping mode
            String @in = sourceFolder + "hello.pdf";
            String @out = destinationFolder + "hello_stamped02.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(@in), CompareTool.CreateTestPdfWriter(@out, new WriterProperties
                ().SetPdfVersion(PdfVersion.PDF_2_0)));
            NUnit.Framework.Assert.AreEqual(PdfVersion.PDF_2_0, pdfDoc.GetPdfVersion());
            pdfDoc.Close();
            PdfDocument assertPdfDoc = new PdfDocument(CompareTool.CreateOutputReader(@out));
            NUnit.Framework.Assert.AreEqual(PdfVersion.PDF_2_0, assertPdfDoc.GetPdfVersion());
            assertPdfDoc.Close();
        }

        [NUnit.Framework.Test]
        public virtual void StampingAppendVersionTest01() {
            // There is a possibility to override version in stamping mode
            String @in = sourceFolder + "hello.pdf";
            String @out = destinationFolder + "stampingAppendVersionTest01.pdf";
            PdfReader reader = new PdfReader(@in);
            PdfWriter writer = CompareTool.CreateTestPdfWriter(@out, new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0
                ));
            PdfDocument pdfDoc = new PdfDocument(reader, writer, new StampingProperties().UseAppendMode());
            NUnit.Framework.Assert.AreEqual(PdfVersion.PDF_2_0, pdfDoc.GetPdfVersion());
            pdfDoc.Close();
            PdfDocument assertPdfDoc = new PdfDocument(CompareTool.CreateOutputReader(@out));
            NUnit.Framework.Assert.AreEqual(PdfVersion.PDF_2_0, assertPdfDoc.GetPdfVersion());
            assertPdfDoc.Close();
        }

        [NUnit.Framework.Test]
        public virtual void StampingTestWithTaggedStructure() {
            String filename = sourceFolder + "iphone_user_guide.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(filename), CompareTool.CreateTestPdfWriter(destinationFolder
                 + "stampingDocWithTaggedStructure.pdf"));
            pdfDoc.Close();
        }

        [NUnit.Framework.Test]
        public virtual void StampingTestWithFullCompression01() {
            String compressedOutPdf = destinationFolder + "stampingTestWithFullCompression01Compressed.pdf";
            String decompressedOutPdf = destinationFolder + "stampingTestWithFullCompression01Decompressed.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(sourceFolder + "fullCompressedDocument.pdf"), new PdfWriter
                (compressedOutPdf));
            pdfDoc.Close();
            float compressedLength = new FileInfo(compressedOutPdf).Length;
            pdfDoc = new PdfDocument(new PdfReader(sourceFolder + "fullCompressedDocument.pdf"), new PdfWriter(decompressedOutPdf
                , new WriterProperties().SetFullCompressionMode(false)));
            pdfDoc.Close();
            float decompressedLength = new FileInfo(decompressedOutPdf).Length;
            float coef = compressedLength / decompressedLength;
            String compareRes = new CompareTool().CompareByContent(compressedOutPdf, decompressedOutPdf, destinationFolder
                );
            NUnit.Framework.Assert.IsTrue(coef < 0.7);
            NUnit.Framework.Assert.IsNull(compareRes);
        }

        [NUnit.Framework.Test]
        public virtual void StampingStreamNoEndingWhitespace01() {
            //TODO: DEVSIX-2007
            PdfDocument pdfDocInput = new PdfDocument(new PdfReader(sourceFolder + "stampingStreamNoEndingWhitespace01.pdf"
                ));
            PdfDocument pdfDocOutput = new PdfDocument(CompareTool.CreateTestPdfWriter(destinationFolder + "stampingStreamNoEndingWhitespace01.pdf"
                , new WriterProperties().SetCompressionLevel(0)));
            pdfDocOutput.AddEventHandler(PdfDocumentEvent.END_PAGE, new PdfStampingTest.WatermarkEventHandler());
            pdfDocInput.CopyPagesTo(1, pdfDocInput.GetNumberOfPages(), pdfDocOutput);
            pdfDocInput.Close();
            pdfDocOutput.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + "stampingStreamNoEndingWhitespace01.pdf"
                , sourceFolder + "cmp_stampingStreamNoEndingWhitespace01.pdf", destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void StampingInAppendModeCreatesNewResourceDictionary() {
            // with some PDFs, when adding content to an existing PDF in append mode, the resource dictionary didn't get written as a new version
            StampingProperties stampProps = new StampingProperties();
            stampProps.UseAppendMode();
            stampProps.PreserveEncryption();
            PdfFont font = PdfFontFactory.CreateFont();
            ByteArrayOutputStream resultStream = new ByteArrayOutputStream();
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(sourceFolder + "hello-d.pdf"), new PdfWriter(resultStream
                ), stampProps);
            PdfPage page = pdfDoc.GetPage(1);
            PdfCanvas canvas = new PdfCanvas(page.NewContentStreamAfter(), page.GetResources(), pdfDoc);
            canvas.BeginText();
            canvas.SetTextRenderingMode(2);
            canvas.SetFontAndSize(font, 42);
            canvas.SetTextMatrix(1, 0, 0, -1, 100, 100);
            canvas.ShowText("TEXT TO STAMP");
            canvas.EndText();
            pdfDoc.Close();
            // parse text
            pdfDoc = new PdfDocument(new PdfReader(new MemoryStream(resultStream.ToArray())));
            LocationTextExtractionStrategy strat = new LocationTextExtractionStrategy();
            PdfCanvasProcessor processor = new PdfCanvasProcessor(strat);
            // this fails with an NPE b/c the /F1 font isn't in the fonts dictionary
            processor.ProcessPageContent(pdfDoc.GetPage(1));
            NUnit.Framework.Assert.IsTrue(strat.GetResultantText().Contains("TEXT TO STAMP"));
        }

        internal static void VerifyPdfPagesCount(PdfObject root) {
            if (root.GetObjectType() == PdfObject.INDIRECT_REFERENCE) {
                root = ((PdfIndirectReference)root).GetRefersTo();
            }
            PdfDictionary pages = (PdfDictionary)root;
            if (!pages.ContainsKey(PdfName.Kids)) {
                return;
            }
            PdfNumber count = pages.GetAsNumber(PdfName.Count);
            if (count != null) {
                NUnit.Framework.Assert.IsTrue(count.IntValue() > 0, "PdfPages with zero count");
            }
            PdfObject kids = pages.Get(PdfName.Kids);
            if (kids.GetObjectType() == PdfObject.ARRAY) {
                foreach (PdfObject kid in (PdfArray)kids) {
                    VerifyPdfPagesCount(kid);
                }
            }
            else {
                VerifyPdfPagesCount(kids);
            }
        }

        internal class WatermarkEventHandler : iText.Kernel.Events.IEventHandler {
            public virtual void HandleEvent(Event @event) {
                PdfDocumentEvent pdfEvent = (PdfDocumentEvent)@event;
                PdfPage page = pdfEvent.GetPage();
                PdfCanvas pdfCanvas = new PdfCanvas(page);
                try {
                    pdfCanvas.BeginText().SetFontAndSize(PdfFontFactory.CreateFont(), 12.0f).ShowText("Text").EndText();
                }
                catch (System.IO.IOException) {
                }
            }
        }
    }
}
