/*
This file is part of the iText (R) project.
Copyright (c) 1998-2025 Apryse Group NV
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
using System.IO;
using iText.Commons.Utils;
using iText.IO.Image;
using iText.IO.Source;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Filespec;
using iText.Kernel.Pdf.Filters;
using iText.Kernel.Utils;
using iText.Layout;
using iText.Layout.Element;
using iText.Test;

namespace iText.Brotlicompressor {
    [NUnit.Framework.Category("IntegrationTest")]
    public class BrotliStreamCompressionStrategyTest : ExtendedITextTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/brotli-compressor" + "/BrotliStreamCompressionStrategyTest/";

        private static readonly String DESTINATION_FOLDER = TestUtil.GetOutputPath() + "/brotli-compressor/BrotliStreamCompressionStrategyTest/";

        private const String testString = "The quick brown fox jumps over the lazy dog. " + "And accented words like naïve, façade, and piñata help validate Unicode handling";

        private const int streamCount = 10;

        [NUnit.Framework.OneTimeSetUp]
        public static void SetUp() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void GenerateSimplePdfTest() {
            String fileName = "simpleBrotli.pdf";
            RunTest(((pdfDocument) => {
                Document layoutDoc = new Document(pdfDocument);
                Table table = new Table(3);
                for (int i = 0; i < 3000; i++) {
                    table.AddCell("Cell " + (i + 1) + ", 1");
                    table.AddCell("Cell " + (i + 1) + ", 2");
                    table.AddCell("Cell " + (i + 1) + ", 3");
                }
                layoutDoc.Add(table);
                layoutDoc.Close();
            }
            ), fileName);
        }

        [NUnit.Framework.Test]
        public virtual void BasicBrotliContentStreamTest() {
            String fileName = "simpleBrotliContentStream.pdf";
            RunTest(((pdfDocument) => {
                PdfPage page = pdfDocument.AddNewPage();
                PdfStream pdfStream = new PdfStream(testString.GetBytes(System.Text.Encoding.UTF8), CompressionConstants.BEST_COMPRESSION
                    );
                pdfStream.MakeIndirect(pdfDocument);
                PdfArray contents = new PdfArray();
                contents.Add(pdfStream.GetIndirectReference());
                page.GetPdfObject().Put(PdfName.Contents, contents);
            }
            ), fileName);
        }

        [NUnit.Framework.Test]
        public virtual void AddBrotliStreamsToFlateStampingModeTest() {
            String resultPath = DESTINATION_FOLDER + "stampedBrotliStreams.pdf";
            StampingProperties props = new StampingProperties();
            props.RegisterDependency(typeof(IStreamCompressionStrategy), new BrotliStreamCompressionStrategy());
            using (PdfDocument pdfDoc = new PdfDocument(new PdfReader(SOURCE_FOLDER + "flateBase.pdf"), new PdfWriter(
                resultPath), props)) {
                PdfPage page = pdfDoc.AddNewPage();
                PdfStream brotliStream = CreateBrotliContentStream(pdfDoc, testString.GetBytes(System.Text.Encoding.UTF8));
                PdfArray contents = CreateContentArrayIfNotFound(page);
                for (int i = 0; i < streamCount; i++) {
                    contents.Add(brotliStream.GetIndirectReference());
                }
                page.GetPdfObject().Put(PdfName.Contents, contents);
            }
            using (PdfDocument pdfDoc_1 = new PdfDocument(new PdfReader(resultPath), props)) {
                PdfPage page = pdfDoc_1.GetFirstPage();
                NUnit.Framework.Assert.AreEqual(1, page.GetContentStreamCount());
                PdfObject filterObject = page.GetFirstContentStream().Get(PdfName.Filter);
                NUnit.Framework.Assert.AreEqual(PdfName.FlateDecode, filterObject);
                page = pdfDoc_1.GetPage(2);
                NUnit.Framework.Assert.AreEqual(streamCount + 1, page.GetContentStreamCount());
                for (int i = 1; i < page.GetContentStreamCount(); i++) {
                    filterObject = page.GetContentStream(i).Get(PdfName.Filter);
                    NUnit.Framework.Assert.AreEqual(PdfName.BrotliDecode, filterObject);
                    NUnit.Framework.Assert.AreEqual(page.GetContentStream(i).GetBytes(), testString.GetBytes(System.Text.Encoding
                        .UTF8));
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void AddBrotliStreamsStampingModeTest() {
            String resultPath = DESTINATION_FOLDER + "stampedBrotliStreams2.pdf";
            StampingProperties props = new StampingProperties();
            props.RegisterDependency(typeof(IStreamCompressionStrategy), new BrotliStreamCompressionStrategy());
            using (PdfDocument pdfDoc = new PdfDocument(new PdfReader(SOURCE_FOLDER + "brotliBase.pdf"), new PdfWriter
                (resultPath), props)) {
                PdfPage page = pdfDoc.AddNewPage();
                PdfStream brotliStream = CreateBrotliContentStream(pdfDoc, testString.GetBytes(System.Text.Encoding.UTF8));
                PdfArray contents = CreateContentArrayIfNotFound(page);
                for (int i = 0; i < streamCount; i++) {
                    contents.Add(brotliStream.GetIndirectReference());
                }
                page.GetPdfObject().Put(PdfName.Contents, contents);
            }
            using (PdfDocument pdfDoc_1 = new PdfDocument(new PdfReader(resultPath), props)) {
                PdfPage page = pdfDoc_1.GetPage(2);
                NUnit.Framework.Assert.AreEqual(streamCount + 1, page.GetContentStreamCount());
                for (int i = 1; i < page.GetContentStreamCount(); i++) {
                    PdfObject filterObject = page.GetContentStream(i).Get(PdfName.Filter);
                    NUnit.Framework.Assert.AreEqual(PdfName.BrotliDecode, filterObject);
                    NUnit.Framework.Assert.AreEqual(page.GetContentStream(i).GetBytes(), testString.GetBytes(System.Text.Encoding
                        .UTF8));
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void ReplaceFlateContentStreamWithBrotliTest() {
            String resultPath = DESTINATION_FOLDER + "replacedContentStream.pdf";
            StampingProperties props = new StampingProperties();
            props.RegisterDependency(typeof(IStreamCompressionStrategy), new BrotliStreamCompressionStrategy());
            using (PdfDocument pdfDoc = new PdfDocument(new PdfReader(SOURCE_FOLDER + "flateBase.pdf"), new PdfWriter(
                resultPath), props)) {
                PdfPage page = pdfDoc.GetFirstPage();
                PdfStream brotliContent = CreateBrotliContentStream(pdfDoc, "Overwritten content".GetBytes(System.Text.Encoding
                    .UTF8));
                page.GetPdfObject().Put(PdfName.Contents, brotliContent.GetIndirectReference());
                page.SetModified();
            }
            using (PdfDocument pdfDoc_1 = new PdfDocument(new PdfReader(resultPath), props)) {
                PdfPage page = pdfDoc_1.GetFirstPage();
                NUnit.Framework.Assert.AreEqual(1, page.GetContentStreamCount());
                PdfObject filterObject = page.GetFirstContentStream().Get(PdfName.Filter);
                NUnit.Framework.Assert.AreEqual(PdfName.BrotliDecode, filterObject);
            }
        }

        [NUnit.Framework.Test]
        public virtual void ReadAndDecodeStreamsTest() {
            String sourcePdf = SOURCE_FOLDER + "mixedStreamFiltersDocument.pdf";
            StampingProperties props = new StampingProperties();
            props.RegisterDependency(typeof(IStreamCompressionStrategy), new BrotliStreamCompressionStrategy());
            using (PdfDocument pdfDoc = new PdfDocument(new PdfReader(sourcePdf))) {
                int numberOfPages = pdfDoc.GetNumberOfPages();
                for (int i = 2; i <= numberOfPages; i++) {
                    PdfStream contentStream = pdfDoc.GetPage(i).GetFirstContentStream();
                    NUnit.Framework.Assert.AreEqual(contentStream.GetBytes(), testString.GetBytes(System.Text.Encoding.UTF8));
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void SingleContentStreamWithBrotliAndFlateTest() {
            ByteArrayOutputStream baos = new ByteArrayOutputStream();
            DocumentProperties props = CreateBrotliProperties();
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(baos), props)) {
                PdfPage page = pdfDoc.AddNewPage();
                PdfCanvas canvas = new PdfCanvas(page);
                canvas.BeginText();
                canvas.SetFontAndSize(PdfFontFactory.CreateFont(), 12);
                canvas.MoveText(50, 700);
                canvas.ShowText(testString);
                canvas.EndText();
                canvas.Release();
                // Flate encode upfront
                PdfStream contentStream = page.GetContentStream(0);
                contentStream.SetCompressionLevel(CompressionConstants.DEFAULT_COMPRESSION);
                ByteArrayOutputStream byteArrayStream = new ByteArrayOutputStream();
                Stream zip = new FlateCompressionStrategy().CreateNewOutputStream(byteArrayStream, contentStream);
                ((ByteArrayOutputStream)contentStream.GetOutputStream().GetOutputStream()).WriteTo(zip);
                ((IFinishable)zip).Finish();
                contentStream.SetData(byteArrayStream.ToArray());
                PdfArray filters = new PdfArray();
                filters.Add(PdfName.FlateDecode);
                contentStream.Put(PdfName.Filter, filters);
            }
            using (PdfDocument readDoc = new PdfDocument(new PdfReader(new MemoryStream(baos.ToArray())))) {
                PdfStream stream = readDoc.GetFirstPage().GetContentStream(0);
                PdfArray filters = stream.GetAsArray(PdfName.Filter);
                NUnit.Framework.Assert.AreEqual(PdfName.BrotliDecode, filters.GetAsName(0));
                NUnit.Framework.Assert.AreEqual(PdfName.FlateDecode, filters.GetAsName(1));
                byte[] decoded = stream.GetBytes();
                String decodedText = iText.Commons.Utils.JavaUtil.GetStringForBytes(decoded, System.Text.Encoding.UTF8);
                NUnit.Framework.Assert.IsTrue(decodedText.Contains("The quick brown fox jumps over the lazy dog. "));
            }
        }

        [NUnit.Framework.Test]
        [NUnit.Framework.Ignore("DEVSIX-9595: This is a starting point test with brotli dictionaries." + " Dictionary is used for decompression but not for compression"
            )]
        public virtual void BrotliStreamWithDDictionaryTest() {
            String resultPath = DESTINATION_FOLDER + "brotli_with_decodeParams_full.pdf";
            byte[] dictionaryBytes = "custom dictionary for Brotli".GetBytes(System.Text.Encoding.UTF8);
            byte[] contentBytes = "The quick brown fox jumps over the lazy dog.".GetBytes(System.Text.Encoding.UTF8);
            DocumentProperties props = new DocumentProperties();
            props.RegisterDependency(typeof(IStreamCompressionStrategy), new BrotliStreamCompressionStrategy());
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(resultPath), props)) {
                PdfPage page = pdfDoc.AddNewPage();
                PdfStream dictStream = new PdfStream(dictionaryBytes);
                dictStream.MakeIndirect(pdfDoc);
                PdfDictionary decodeParms = new PdfDictionary();
                decodeParms.Put(PdfName.D, dictStream.GetIndirectReference());
                PdfStream brotliStream = new PdfStream(contentBytes, CompressionConstants.BEST_COMPRESSION);
                brotliStream.MakeIndirect(pdfDoc);
                // Brotli encode upfront
                brotliStream.SetCompressionLevel(CompressionConstants.DEFAULT_COMPRESSION);
                ByteArrayOutputStream byteArrayStream = new ByteArrayOutputStream();
                Stream zip = new FlateCompressionStrategy().CreateNewOutputStream(byteArrayStream, brotliStream);
                ((ByteArrayOutputStream)brotliStream.GetOutputStream().GetOutputStream()).WriteTo(zip);
                ((IFinishable)zip).Finish();
                brotliStream.SetData(byteArrayStream.ToArray());
                brotliStream.Put(PdfName.Filter, PdfName.BrotliDecode);
                brotliStream.Put(PdfName.DecodeParms, decodeParms);
                page.GetPdfObject().Put(PdfName.Contents, brotliStream.GetIndirectReference());
                page.SetModified();
            }
            using (PdfDocument pdfDoc_1 = new PdfDocument(new PdfReader(resultPath))) {
                PdfPage page = pdfDoc_1.GetFirstPage();
                PdfStream stream = page.GetFirstContentStream();
                PdfDictionary decodeParams = stream.GetAsDictionary(PdfName.DecodeParms);
                BrotliFilter filter = new BrotliFilter();
                byte[] decoded = filter.Decode(stream.GetBytes(false), PdfName.BrotliDecode, decodeParams, stream);
                NUnit.Framework.Assert.AreEqual(contentBytes, decoded, "Decoded content should match original bytes");
            }
        }

        [NUnit.Framework.Test]
        public virtual void AddEmbeddedFileAndCompareTest() {
            String fileName = "simpleEmbeddedFile.pdf";
            RunTest((pdfDoc) => AddEmbeddedFile(pdfDoc, "example.txt", "Hello from Brotli embedded file".GetBytes(System.Text.Encoding
                .UTF8)), fileName);
        }

        [NUnit.Framework.Test]
        public virtual void AddEmbeddedJsonFileWithBrotliTest() {
            String fileName = "simpleJsonEmbedded.pdf";
            RunTest((pdfDoc) => {
                String json = "{\"message\": \"Hello Brotli JSON\", \"id\": 42}";
                byte[] fileBytes = json.GetBytes(System.Text.Encoding.UTF8);
                AddEmbeddedFile(pdfDoc, "JSON File", "data.json", fileBytes);
            }
            , fileName);
        }

        [NUnit.Framework.Test]
        public virtual void AddMultipleEmbeddedFilesWithBrotliTest() {
            String fileName = "simpleBrotliEmbedded.pdf";
            RunTest((pdfDoc) => {
                for (int i = 1; i <= 3; i++) {
                    AddEmbeddedFile(pdfDoc, "file" + i + ".txt", ("File " + i).GetBytes(System.Text.Encoding.UTF8));
                }
            }
            , fileName);
        }

        [NUnit.Framework.Test]
        public virtual void AddEmbeddedUnicodeFileWithBrotliTest() {
            String fileName = "simpleEmbeddedUnicode.pdf";
            RunTest((pdfDoc) => {
                String unicode = "こんにちは世界 — Hello World 🌍";
                byte[] bytes = unicode.GetBytes(System.Text.Encoding.Unicode);
                AddEmbeddedFile(pdfDoc, "Unicode Text", "unicode.txt", bytes);
            }
            , fileName);
        }

        [NUnit.Framework.Test]
        public virtual void AddRealEmbeddedFileAndCompareTest() {
            String fileName = "realEmbeddedFile.pdf";
            byte[] fileBytes = File.ReadAllBytes(System.IO.Path.Combine(SOURCE_FOLDER + "hello.txt"));
            RunTest((pdfDocument) => {
                AddEmbeddedFile(pdfDocument, "Brotli Embedded File", "hello.txt", fileBytes);
            }
            , fileName);
        }

        [NUnit.Framework.Test]
        public virtual void AddEmbeddedFileAndBrotliContentStreamTest() {
            String fileName = "embeddedFileAndContentStream.pdf";
            RunTest((pdfDoc) => {
                byte[] fileBytes = "Embedded + Brotli content".GetBytes(System.Text.Encoding.UTF8);
                AddEmbeddedFile(pdfDoc, "Brotli File", "embedded.txt", fileBytes);
                PdfPage page = pdfDoc.AddNewPage();
                PdfStream brotliContent = CreateBrotliContentStream(pdfDoc, "Hello from Brotli page stream".GetBytes(System.Text.Encoding
                    .UTF8));
                PdfArray arr = new PdfArray();
                arr.Add(brotliContent.GetIndirectReference());
                page.GetPdfObject().Put(PdfName.Contents, arr);
            }
            , fileName);
        }

        [NUnit.Framework.Test]
        public virtual void AddEmbeddedBrotliFileTest() {
            String dest = DESTINATION_FOLDER + "embedded_brotli.pdf";
            DocumentProperties props = CreateBrotliProperties();
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(dest), props)) {
                byte[] fileBytes = "Hello from Brotli embedded file".GetBytes(System.Text.Encoding.UTF8);
                AddEmbeddedFile(pdfDoc, "Brotli Embedded File", "example.txt", fileBytes);
            }
            using (PdfDocument pdfDoc_1 = new PdfDocument(new PdfReader(dest))) {
                IDictionary<String, PdfStream> embedded = GetEmbeddedFiles(pdfDoc_1);
                foreach (KeyValuePair<String, PdfStream> e in embedded) {
                    String name = e.Key;
                    PdfStream stream = e.Value;
                    byte[] decoded = stream.GetBytes();
                    NUnit.Framework.Assert.AreEqual("example.txt", name);
                    NUnit.Framework.Assert.AreEqual(PdfName.BrotliDecode, stream.GetAsName(PdfName.Filter));
                    NUnit.Framework.Assert.AreEqual(31, decoded.Length);
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void AddFlateImageToBrotliPdfTest() {
            String fileName = "imgBrotli.pdf";
            byte[] imageBytes = File.ReadAllBytes(System.IO.Path.Combine(SOURCE_FOLDER + "apryse.png"));
            RunTest(((pdfDocument) => {
                ImageData imageData = ImageDataFactory.Create(imageBytes);
                PdfPage page = pdfDocument.AddNewPage();
                PdfCanvas canvas = new PdfCanvas(page);
                canvas.AddImageAt(imageData, 50, 700, false);
                canvas.Release();
            }
            ), fileName);
        }

        [NUnit.Framework.Test]
        public virtual void AddMultipleBrotliCompressedImagesAndDecodeTest() {
            String[] images = new String[] { "apryse.png", "itext.png", "bulb.gif", "bee.jp2", "simple.bmp" };
            ByteArrayOutputStream baos = new ByteArrayOutputStream();
            DocumentProperties props = CreateBrotliProperties();
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(baos), props)) {
                for (int i = 0; i < images.Length; i++) {
                    PdfPage page = pdfDoc.AddNewPage();
                    byte[] imgBytes = File.ReadAllBytes(System.IO.Path.Combine(SOURCE_FOLDER + images[i]));
                    AddBrotliImageXObject(pdfDoc, page, "Im1", imgBytes);
                }
            }
            using (PdfDocument pdfDoc_1 = new PdfDocument(new PdfReader(new MemoryStream(baos.ToArray())))) {
                for (int j = 1; j <= pdfDoc_1.GetNumberOfPages(); j++) {
                    PdfPage page = pdfDoc_1.GetPage(j);
                    byte[] original = File.ReadAllBytes(System.IO.Path.Combine(SOURCE_FOLDER + images[j - 1]));
                    byte[] decoded = ReadAndDecodeBrotliImage(page, new PdfName("Im1"));
                    NUnit.Framework.Assert.AreEqual(original, decoded);
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void BrotliInlineImagesProhibitedTest() {
            String fileName = "inlineImg.pdf";
            ImageData imageData = ImageDataFactory.Create(SOURCE_FOLDER + "bulb.gif");
            RunTest((pdfDoc) => {
                PdfPage page = pdfDoc.AddNewPage();
                PdfCanvas canvas = new PdfCanvas(page);
                canvas.AddImageFittedIntoRectangle(imageData, new Rectangle(36, 460, 100, 14.16f), true);
            }
            , fileName);
        }

        [NUnit.Framework.Test]
        public virtual void ComparePdfStreamsTest() {
            // Create PDF with Brotli compression
            ByteArrayOutputStream brotliBaos = new ByteArrayOutputStream();
            DocumentProperties props = CreateBrotliProperties();
            PdfDocument brotliPdfDoc = new PdfDocument(new PdfWriter(brotliBaos), props);
            Document brotliLayoutDoc = new Document(brotliPdfDoc);
            Table brotliTable = new Table(3);
            for (int i = 0; i < 1000; i++) {
                brotliTable.AddCell("Cell " + (i + 1) + ", Column 1");
                brotliTable.AddCell("Cell " + (i + 1) + ", Column 2");
                brotliTable.AddCell("Cell " + (i + 1) + ", Column 3");
            }
            brotliLayoutDoc.Add(brotliTable);
            brotliLayoutDoc.Close();
            long brotliSize = brotliBaos.ToArray().Length;
            // Create PDF with Flate compression
            ByteArrayOutputStream flateBaos = new ByteArrayOutputStream();
            PdfDocument flatePdfDoc = new PdfDocument(new PdfWriter(flateBaos));
            Document flateLayoutDoc = new Document(flatePdfDoc);
            Table flateTable = new Table(3);
            for (int i = 0; i < 1000; i++) {
                flateTable.AddCell("Cell " + (i + 1) + ", Column 1");
                flateTable.AddCell("Cell " + (i + 1) + ", Column 2");
                flateTable.AddCell("Cell " + (i + 1) + ", Column 3");
            }
            flateLayoutDoc.Add(flateTable);
            flateLayoutDoc.Close();
            long flateSize = flateBaos.ToArray().Length;
            // Verify both PDFs were created successfully
            NUnit.Framework.Assert.IsTrue(brotliSize > 0, "Brotli compressed PDF should not be empty");
            NUnit.Framework.Assert.IsTrue(flateSize > 0, "Flate compressed PDF should not be empty");
            // Verify both PDFs can be read back
            PdfDocument brotliReadDoc = new PdfDocument(new PdfReader(new MemoryStream(brotliBaos.ToArray())));
            NUnit.Framework.Assert.AreEqual(30, brotliReadDoc.GetNumberOfPages(), "Brotli PDF should have 30 pages");
            PdfDocument flateReadDoc = new PdfDocument(new PdfReader(new MemoryStream(flateBaos.ToArray())));
            NUnit.Framework.Assert.AreEqual(30, flateReadDoc.GetNumberOfPages(), "Flate PDF should have 30 pages");
            //loop over each page and compare the content streams
            for (int i = 1; i <= brotliReadDoc.GetNumberOfPages(); i++) {
                PdfStream brotliContentStream = brotliReadDoc.GetPage(i).GetContentStream(0);
                PdfStream flateContentStream = flateReadDoc.GetPage(i).GetContentStream(0);
                byte[] brotliBytes = brotliContentStream.GetBytes();
                byte[] flateBytes = flateContentStream.GetBytes();
                NUnit.Framework.Assert.AreEqual(flateBytes, brotliBytes, "Content streams of page " + i + " should be identical between Brotli and Flate PDFs"
                    );
            }
            brotliReadDoc.Close();
            flateReadDoc.Close();
        }

        private void RunTest(Action<PdfDocument> testRunner, String brotliPdfPath) {
            long startTime = SystemUtil.CurrentTimeMillis();
            ByteArrayOutputStream baos = new ByteArrayOutputStream();
            DocumentProperties props = CreateBrotliProperties();
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(baos), props);
            testRunner(pdfDoc);
            long length = baos.ToArray().Length;
            pdfDoc.Close();
            long endTime = SystemUtil.CurrentTimeMillis();
            long startFlateTime = SystemUtil.CurrentTimeMillis();
            ByteArrayOutputStream flateBaos = new ByteArrayOutputStream();
            PdfDocument flatePdfDoc = new PdfDocument(new PdfWriter(flateBaos));
            testRunner(flatePdfDoc);
            long flateLength = flateBaos.ToArray().Length;
            flatePdfDoc.Close();
            System.Console.Out.WriteLine("Generated PDF size with Brotli compression: " + length + " bytes" + " in " +
                 (endTime - startTime) + " ms");
            System.Console.Out.WriteLine("Generated PDF size with Flate  compression: " + flateLength + " bytes" + " in "
                 + (SystemUtil.CurrentTimeMillis() - startFlateTime) + " ms");
            double ratio = (double)flateLength / length;
            System.Console.Out.WriteLine("Compression ratio (Flate / Brotli): " + ratio);
            PdfDocument readDoc = new PdfDocument(new PdfReader(new MemoryStream(baos.ToArray())));
            PdfDocument flateReadDoc = new PdfDocument(new PdfReader(new MemoryStream(flateBaos.ToArray())));
            int numberOfPdfObjects = readDoc.GetNumberOfPdfObjects();
            int numberOfFlatePdfObjects = flateReadDoc.GetNumberOfPdfObjects();
            NUnit.Framework.Assert.AreEqual(numberOfFlatePdfObjects, numberOfPdfObjects, "Number of PDF objects should be the same in both documents"
                );
            for (int i = 1; i <= numberOfPdfObjects; i++) {
                PdfObject obj = readDoc.GetPdfObject(i);
                PdfObject flateObj = flateReadDoc.GetPdfObject(i);
                if (obj is PdfStream && flateObj is PdfStream) {
                    byte[] brotliBytes = ((PdfStream)obj).GetBytes();
                    byte[] flateBytes = ((PdfStream)flateObj).GetBytes();
                    NUnit.Framework.Assert.AreEqual(flateBytes, brotliBytes, "PDF stream bytes should be identical for object number "
                         + i);
                }
            }
            //compare PDF
            if (brotliPdfPath != null) {
                String destPath = DESTINATION_FOLDER + brotliPdfPath;
                String cmpPath = SOURCE_FOLDER + "cmp_" + brotliPdfPath;
                System.IO.File.WriteAllBytes(System.IO.Path.Combine(destPath), baos.ToArray());
                CompareTool compareTool = new CompareTool();
                String compareResult = compareTool.CompareByContent(destPath, cmpPath, DESTINATION_FOLDER, "diff_");
                NUnit.Framework.Assert.IsNull(compareResult, "Brotli PDF does not match reference PDF: " + compareResult);
            }
        }

        private PdfStream CreateBrotliContentStream(PdfDocument pdfDoc, byte[] data) {
            PdfStream stream = new PdfStream(data, CompressionConstants.BEST_COMPRESSION);
            stream.MakeIndirect(pdfDoc);
            return stream;
        }

        private void AddEmbeddedFile(PdfDocument pdfDoc, String displayName, String fileName, byte[] bytes) {
            PdfFileSpec spec = PdfFileSpec.CreateEmbeddedFileSpec(pdfDoc, bytes, displayName, fileName, null, null);
            pdfDoc.AddFileAttachment(fileName, spec);
        }

        private void AddBrotliImageXObject(PdfDocument pdfDoc, PdfPage page, String name, byte[] imgBytes) {
            PdfStream imgStream = new PdfStream(imgBytes, CompressionConstants.BEST_COMPRESSION);
            imgStream.MakeIndirect(pdfDoc);
            PdfDictionary resources = page.GetResources().GetPdfObject();
            PdfDictionary xObjects = resources.GetAsDictionary(PdfName.XObject);
            if (xObjects == null) {
                xObjects = new PdfDictionary();
                resources.Put(PdfName.XObject, xObjects);
            }
            xObjects.Put(new PdfName(name), imgStream);
        }

        private byte[] ReadAndDecodeBrotliImage(PdfPage page, PdfName imageName) {
            PdfDictionary xObjects = page.GetResources().GetPdfObject().GetAsDictionary(PdfName.XObject);
            PdfStream stream = xObjects.GetAsStream(imageName);
            return stream.GetBytes();
        }

        private void AddEmbeddedFile(PdfDocument pdfDoc, String name, byte[] bytes) {
            PdfFileSpec fs = PdfFileSpec.CreateEmbeddedFileSpec(pdfDoc, bytes, name, name, null, null);
            pdfDoc.AddFileAttachment(name, fs);
        }

        private DocumentProperties CreateBrotliProperties() {
            DocumentProperties props = new DocumentProperties();
            props.RegisterDependency(typeof(IStreamCompressionStrategy), new BrotliStreamCompressionStrategy());
            return props;
        }

        private PdfArray CreateContentArrayIfNotFound(PdfPage page) {
            PdfArray contents;
            PdfObject existingContents = page.GetPdfObject().Get(PdfName.Contents);
            if (existingContents is PdfArray) {
                contents = (PdfArray)existingContents;
            }
            else {
                if (existingContents != null) {
                    contents = new PdfArray();
                    contents.Add(existingContents);
                }
                else {
                    contents = new PdfArray();
                }
            }
            return contents;
        }

        private static IDictionary<String, PdfStream> GetEmbeddedFiles(PdfDocument pdfDoc) {
            IDictionary<String, PdfStream> result = new Dictionary<String, PdfStream>();
            PdfDictionary catalog = pdfDoc.GetCatalog().GetPdfObject();
            PdfDictionary names = catalog.GetAsDictionary(PdfName.Names);
            if (names == null) {
                return result;
            }
            PdfDictionary embeddedFiles = names.GetAsDictionary(PdfName.EmbeddedFiles);
            if (embeddedFiles == null) {
                return result;
            }
            PdfArray nameArray = embeddedFiles.GetAsArray(PdfName.Names);
            if (nameArray == null || nameArray.IsEmpty()) {
                return result;
            }
            for (int i = 0; i < nameArray.Size(); i += 2) {
                PdfString fileName = nameArray.GetAsString(i);
                PdfDictionary fileSpec = nameArray.GetAsDictionary(i + 1);
                if (fileName == null || fileSpec == null) {
                    continue;
                }
                PdfDictionary efDict = fileSpec.GetAsDictionary(PdfName.EF);
                if (efDict == null) {
                    continue;
                }
                PdfStream stream = efDict.GetAsStream(PdfName.UF);
                if (stream == null) {
                    stream = efDict.GetAsStream(PdfName.F);
                }
                if (stream == null) {
                    continue;
                }
                result.Put(fileName.ToString(), stream);
            }
            return result;
        }
    }
}
