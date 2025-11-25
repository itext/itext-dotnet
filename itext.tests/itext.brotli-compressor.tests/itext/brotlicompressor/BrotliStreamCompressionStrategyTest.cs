using System;
using System.IO;
using iText.IO.Source;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Test;

namespace iText.Brotlicompressor {
    [NUnit.Framework.Category("IntegrationTest")]
    public class BrotliStreamCompressionStrategyTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void GenerateSimplePdf() {
            RunTest(((pdfDocument) => {
                Document layoutDoc = new Document(pdfDocument);
                Table table = new Table(3);
                for (int i = 0; i < 30000; i++) {
                    table.AddCell("Cell " + (i + 1) + ", 1");
                    table.AddCell("Cell " + (i + 1) + ", 2");
                    table.AddCell("Cell " + (i + 1) + ", 3");
                }
                layoutDoc.Add(table);
                layoutDoc.Close();
            }
            ));
        }

        [NUnit.Framework.Test]
        public virtual void GenerateSimplePdfHighCompression() {
            RunTest(((pdfDocument) => {
                pdfDocument.GetWriter().SetCompressionLevel(9);
                Document layoutDoc = new Document(pdfDocument);
                Table table = new Table(3);
                for (int i = 0; i < 30000; i++) {
                    table.AddCell("Cell " + (i + 1) + ", 1");
                    table.AddCell("Cell " + (i + 1) + ", 2");
                    table.AddCell("Cell " + (i + 1) + ", 3");
                }
                layoutDoc.Add(table);
                layoutDoc.Close();
            }
            ));
        }

        [NUnit.Framework.Test]
        public virtual void GenerateSimplePdfLowCompression() {
            RunTest(((pdfDocument) => {
                pdfDocument.GetWriter().SetCompressionLevel(1);
                Document layoutDoc = new Document(pdfDocument);
                Table table = new Table(3);
                for (int i = 0; i < 30000; i++) {
                    table.AddCell("Cell " + (i + 1) + ", 1");
                    table.AddCell("Cell " + (i + 1) + ", 2");
                    table.AddCell("Cell " + (i + 1) + ", 3");
                }
                layoutDoc.Add(table);
                layoutDoc.Close();
            }
            ));
        }

        [NUnit.Framework.Test]
        public virtual void ComparePdfStreamsTest() {
            // Create PDF with Brotli compression
            ByteArrayOutputStream brotliBaos = new ByteArrayOutputStream();
            PdfDocument brotliPdfDoc = new PdfDocument(new PdfWriter(brotliBaos));
            brotliPdfDoc.GetDiContainer().Register(typeof(IStreamCompressionStrategy), new BrotliStreamCompressionStrategy
                ());
            Document brotliLayoutDoc = new Document(brotliPdfDoc);
            Table brotliTable = new Table(3);
            for (int i = 0; i < 1000; i++) {
                brotliTable.AddCell("Cell " + (i + 1) + ", Column 1");
                brotliTable.AddCell("Cell " + (i + 1) + ", Column 2");
                brotliTable.AddCell("Cell " + (i + 1) + ", Column 3");
            }
            brotliLayoutDoc.Add(brotliTable);
            brotliLayoutDoc.Close();
            int brotliSize = brotliBaos.Length;
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
            int flateSize = flateBaos.Length;
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

        private void RunTest(Action<PdfDocument> testRunner) {
            long startTime = iText.CurrentTimeMillis();
            ByteArrayOutputStream baos = new ByteArrayOutputStream();
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(baos));
            pdfDoc.GetDiContainer().Register(typeof(IStreamCompressionStrategy), new BrotliStreamCompressionStrategy()
                );
            testRunner(pdfDoc);
            int length = baos.Length;
            long endTime = iText.CurrentTimeMillis();
            long startFlateTime = iText.CurrentTimeMillis();
            ByteArrayOutputStream flateBaos = new ByteArrayOutputStream();
            PdfDocument flatePdfDoc = new PdfDocument(new PdfWriter(flateBaos));
            testRunner(flatePdfDoc);
            int flateLength = flateBaos.Length;
            System.Console.Out.WriteLine("Generated PDF size with Brotli compression: " + length + " bytes" + " in " +
                 (endTime - startTime) + " ms");
            System.Console.Out.WriteLine("Generated PDF size with Flate  compression: " + flateLength + " bytes" + " in "
                 + (iText.CurrentTimeMillis() - startFlateTime) + " ms");
            double ratio = (double)flateLength / length;
            System.Console.Out.Printf("Compression ratio (Flate / Brotli): %.2f%n", ratio);
        }
    }
}
