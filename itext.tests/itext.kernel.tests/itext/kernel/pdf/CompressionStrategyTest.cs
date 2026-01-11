/*
This file is part of the iText (R) project.
Copyright (c) 1998-2026 Apryse Group NV
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
using iText.IO.Source;
using iText.Kernel.Font;
using iText.Kernel.Pdf.Canvas;
using iText.Test;

namespace iText.Kernel.Pdf {
    [NUnit.Framework.Category("IntegrationTest")]
    public class CompressionStrategyTest : ExtendedITextTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/pdf/CompressionStrategyTest/";

        private static readonly String DESTINATION_FOLDER = TestUtil.GetOutputPath() + "/kernel/pdf/CompressionStrategyTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(DESTINATION_FOLDER);
        }

        private const String testString = "Some test string for testing compression strategy should be big enough";

        private static readonly byte[] TEST_CONTENT_STREAM_DATA = ("q\n" + "0 0 0 RG\n" + "10 w\n" + "1 j\n" + "2 J\n"
             + "100 100 m\n" + "300 100 l\n" + "200 300 l\n" + "s\n" + "1 0 0 RG\n" + "200 50 m\n" + "200 350 l\n"
             + "S\n" + "Q\n").GetBytes(System.Text.Encoding.ASCII);

        public static IEnumerable<Object[]> CompressionStrategiesArguments() {
            return JavaUtil.ArraysAsList(new Object[] { new ASCII85CompressionStrategy(), "ASCII85" }, new Object[] { 
                new ASCIIHexCompressionStrategy(), "ASCIIHex" }, new Object[] { new RunLengthCompressionStrategy(), "RunLength"
                 });
        }

        public static IEnumerable<Object[]> TwoCompressionStrategies() {
            return JavaUtil.ArraysAsList(new Object[] { new RunLengthCompressionStrategy(), new ASCII85CompressionStrategy
                (), "Run length on ASCII85" }, new Object[] { new ASCII85CompressionStrategy(), new ASCIIHexCompressionStrategy
                (), "ASCII85 on ASCIIHex" }, new Object[] { new ASCIIHexCompressionStrategy(), new RunLengthCompressionStrategy
                (), "ASCIIHex on RunLength" });
        }

        [NUnit.Framework.Test]
        public virtual void Ascii85DecodeTest() {
            DoStrategyTest(new ASCII85CompressionStrategy());
        }

        [NUnit.Framework.Test]
        public virtual void AsciiHexDecodeTest() {
            DoStrategyTest(new ASCIIHexCompressionStrategy());
        }

        [NUnit.Framework.Test]
        public virtual void FlateDecodeTest() {
            DoStrategyTest(new FlateCompressionStrategy());
        }

        [NUnit.Framework.Test]
        public virtual void RunLengthDecodeTest() {
            DoStrategyTest(new RunLengthCompressionStrategy());
        }

        [NUnit.Framework.TestCaseSource("CompressionStrategiesArguments")]
        public virtual void AddStreamCompressionStampingModeTest(IStreamCompressionStrategy strategy, String compressionName
            ) {
            String resultPath = DESTINATION_FOLDER + "stamped" + compressionName + "Streams.pdf";
            StampingProperties props = new StampingProperties();
            props.RegisterDependency(typeof(IStreamCompressionStrategy), strategy);
            int streamCount = 3;
            using (PdfDocument pdfDoc = new PdfDocument(new PdfReader(SOURCE_FOLDER + compressionName + "ContentStream.pdf"
                ), new PdfWriter(resultPath), props)) {
                PdfPage page = pdfDoc.AddNewPage();
                PdfStream stream = new PdfStream(testString.GetBytes(System.Text.Encoding.UTF8), CompressionConstants.BEST_COMPRESSION
                    );
                stream.MakeIndirect(pdfDoc);
                PdfArray contents = new PdfArray();
                contents.Add(page.GetPdfObject().Get(PdfName.Contents));
                for (int i = 0; i < streamCount; i++) {
                    contents.Add(stream.GetIndirectReference());
                }
                page.GetPdfObject().Put(PdfName.Contents, contents);
            }
            using (PdfDocument pdfDoc_1 = new PdfDocument(new PdfReader(resultPath), props)) {
                PdfPage page = pdfDoc_1.GetPage(2);
                NUnit.Framework.Assert.AreEqual(streamCount + 1, page.GetContentStreamCount());
                for (int i = 1; i < page.GetContentStreamCount(); i++) {
                    PdfObject filterObject = page.GetContentStream(i).Get(PdfName.Filter);
                    NUnit.Framework.Assert.AreEqual(strategy.GetFilterName(), filterObject);
                    NUnit.Framework.Assert.AreEqual(page.GetContentStream(i).GetBytes(), testString.GetBytes(System.Text.Encoding
                        .UTF8));
                }
            }
        }

        [NUnit.Framework.TestCaseSource("TwoCompressionStrategies")]
        public virtual void TwoFiltersInSingleStreamTest(IStreamCompressionStrategy firstStrategy, IStreamCompressionStrategy
             secondStrategy, String testName) {
            ByteArrayOutputStream baos = new ByteArrayOutputStream();
            StampingProperties props = new StampingProperties();
            props.RegisterDependency(typeof(IStreamCompressionStrategy), firstStrategy);
            using (PdfDocument pdfDoc = new PdfDocument(new PdfWriter(baos), props)) {
                PdfPage page = pdfDoc.AddNewPage();
                PdfCanvas canvas = new PdfCanvas(page);
                canvas.BeginText();
                canvas.SetFontAndSize(PdfFontFactory.CreateFont(), 12);
                canvas.MoveText(50, 700);
                canvas.ShowText(testString);
                canvas.EndText();
                canvas.Release();
                PdfStream contentStream = page.GetContentStream(0);
                contentStream.SetCompressionLevel(CompressionConstants.DEFAULT_COMPRESSION);
                ByteArrayOutputStream byteArrayStream = new ByteArrayOutputStream();
                Stream zip = secondStrategy.CreateNewOutputStream(byteArrayStream, contentStream);
                ((ByteArrayOutputStream)contentStream.GetOutputStream().GetOutputStream()).WriteTo(zip);
                ((IFinishable)zip).Finish();
                contentStream.SetData(byteArrayStream.ToArray());
                PdfArray filters = new PdfArray();
                filters.Add(secondStrategy.GetFilterName());
                contentStream.Put(PdfName.Filter, filters);
            }
            using (PdfDocument readDoc = new PdfDocument(new PdfReader(new MemoryStream(baos.ToArray())))) {
                PdfStream stream = readDoc.GetFirstPage().GetContentStream(0);
                PdfArray filters = stream.GetAsArray(PdfName.Filter);
                NUnit.Framework.Assert.AreEqual(firstStrategy.GetFilterName(), filters.GetAsName(0));
                NUnit.Framework.Assert.AreEqual(secondStrategy.GetFilterName(), filters.GetAsName(1));
                byte[] decoded = stream.GetBytes();
                String decodedText = iText.Commons.Utils.JavaUtil.GetStringForBytes(decoded, System.Text.Encoding.UTF8);
                NUnit.Framework.Assert.IsTrue(decodedText.Contains(testString));
            }
        }

        private static void DoStrategyTest(IStreamCompressionStrategy strategy) {
            long writePlainTime = SystemUtil.CurrentTimeMillis();
            ByteArrayOutputStream plainPdfBytes = new ByteArrayOutputStream();
            WriteTestDocument(plainPdfBytes, null, CompressionConstants.NO_COMPRESSION);
            byte[] bytes = plainPdfBytes.ToArray();
            long plainSize = bytes.Length;
            writePlainTime = SystemUtil.CurrentTimeMillis() - writePlainTime;
            System.Console.Out.WriteLine("Generated PDF size without compression: " + plainSize + " bytes in " + writePlainTime
                 + "ms");
            long writeCompressedTime = SystemUtil.CurrentTimeMillis();
            ByteArrayOutputStream compressedPdfBytes = new ByteArrayOutputStream();
            WriteTestDocument(compressedPdfBytes, strategy, CompressionConstants.DEFAULT_COMPRESSION);
            byte[] compressedBytes = compressedPdfBytes.ToArray();
            long compressedSize = compressedBytes.Length;
            writeCompressedTime = SystemUtil.CurrentTimeMillis() - writeCompressedTime;
            System.Console.Out.WriteLine("Generated PDF with `" + strategy.GetFilterName() + "` compression: " + compressedSize
                 + " bytes in " + writeCompressedTime + "ms");
            System.Console.Out.WriteLine("Compression ratio: " + ((double)compressedSize / plainSize));
            PdfDocument plainDoc = new PdfDocument(new PdfReader(new MemoryStream(compressedBytes)));
            PdfDocument compressedDoc = new PdfDocument(new PdfReader(new MemoryStream(bytes)));
            int numberOfPdfObjects = plainDoc.GetNumberOfPdfObjects();
            NUnit.Framework.Assert.AreEqual(numberOfPdfObjects, compressedDoc.GetNumberOfPdfObjects(), "Number of PDF objects should be the same in both documents"
                );
            for (int objNum = 1; objNum <= numberOfPdfObjects; ++objNum) {
                PdfObject plainObj = plainDoc.GetPdfObject(objNum);
                PdfObject compressedObj = compressedDoc.GetPdfObject(objNum);
                NUnit.Framework.Assert.AreEqual(GetType(plainObj), GetType(compressedObj), "PDF object type should be identical for object number "
                     + objNum);
                if ((plainObj is PdfStream) && (compressedObj is PdfStream)) {
                    byte[] plainStreamBytes = ((PdfStream)plainObj).GetBytes();
                    byte[] compressedStreamBytes = ((PdfStream)compressedObj).GetBytes();
                    NUnit.Framework.Assert.AreEqual(plainStreamBytes, compressedStreamBytes, "PDF stream bytes should be identical for object number "
                         + objNum);
                }
            }
        }

        private static void WriteTestDocument(ByteArrayOutputStream os, IStreamCompressionStrategy strategy, int compressionLevel
            ) {
            DocumentProperties docProps = new DocumentProperties();
            if (strategy != null) {
                docProps.RegisterDependency(typeof(IStreamCompressionStrategy), strategy);
            }
            WriterProperties writerProps = new WriterProperties().SetCompressionLevel(compressionLevel);
            using (PdfDocument doc = new PdfDocument(new PdfWriter(os, writerProps), docProps)) {
                PdfPage page = doc.AddNewPage();
                page.GetFirstContentStream().SetData(TEST_CONTENT_STREAM_DATA);
            }
        }

        private static byte GetType(PdfObject obj) {
            if (obj == null) {
                return (byte)0;
            }
            return obj.GetObjectType();
        }
    }
}
