using System;
using System.IO;
using iTextSharp.Kernel.Pdf;
using iTextSharp.Kernel.Pdf.Canvas;
using iTextSharp.text;
using iTextSharp.text.pdf;
using NUnit.Framework;
using PdfDocument = iTextSharp.Kernel.Pdf.PdfDocument;
using PdfPage = iTextSharp.Kernel.Pdf.PdfPage;
using PdfWriter = iTextSharp.Kernel.Pdf.PdfWriter;

namespace iTextSharp.Profiling
{
    class PdfCanvasTest
    {
        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext
            .TestDirectory + "/test/itextsharp/profiling/PdfCanvasTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(destinationFolder);
        }

        public static void CreateOrClearDestinationFolder(String path)
        {
            Directory.CreateDirectory(path);
            foreach (String f in Directory.GetFiles(path))
            {
                File.Delete(f);
            }
        }

        protected void ComparePerformance(bool fullCompression, float coef) {
            int pageCount = 100000;
            int runCount = 10;

            String author = "Alexander Chingarev";
            String creator = "iText";
            String title = "Empty iText Document";

            long iText5Time = 0;
            long iText7Time = 0;

            for (int i = 0; i < runCount; i++)
            {
                {
                    long t1 = DateTime.Now.Ticks;
                    Document.Compress = false;
                    Document document = new Document();
                    FileStream fos = new FileStream(destinationFolder + "comparePerformanceTest_iText5.pdf", FileMode.Create);
                    iTextSharp.text.pdf.PdfWriter writer = iTextSharp.text.pdf.PdfWriter.GetInstance(document, fos);
                    if (fullCompression)
                        writer.SetFullCompression();
                    document.AddAuthor(author);
                    document.AddCreator(creator);
                    document.AddTitle(title);
                    document.Open();
                    for (int k = 0; k < pageCount; k++)
                    {
                        document.NewPage();
                        PdfContentByte cb = writer.DirectContent;
                        cb.Rectangle(100, 100, 100, 100);
                        cb.Fill();
                    }
                    document.Close();
                    long t2 = DateTime.Now.Ticks;
                    iText5Time += (t2 - t1) / 10000;
                }

                {
                    long t1 = DateTime.Now.Ticks;
                    FileStream fos = new FileStream(destinationFolder + "comparePerformanceTest_iText6.pdf", FileMode.Create);
                    PdfWriter writer = new iTextSharp.Kernel.Pdf.PdfWriter(fos, new WriterProperties()
                            .SetFullCompressionMode(fullCompression)
                            .SetCompressionLevel(CompressionConstants.NO_COMPRESSION));
                    iTextSharp.Kernel.Pdf.PdfDocument pdfDoc = new iTextSharp.Kernel.Pdf.PdfDocument(writer);
                    pdfDoc.GetDocumentInfo().SetAuthor(author).
                            SetCreator(creator).
                            SetTitle(title);
                    for (int k = 0; k < pageCount; k++)
                    {
                        PdfPage page = pdfDoc.AddNewPage();
                        PdfCanvas canvas = new PdfCanvas(page);
                        canvas.Rectangle(100, 100, 100, 100).Fill();
                        canvas.Release();
                        page.Flush();
                    }
                    pdfDoc.Close();
                    long t2 = DateTime.Now.Ticks;
                    iText7Time += (t2 - t1) / 10000;
                }
            }
            String compression = fullCompression ? "compression" : "no compression";
            Console.Out.WriteLine("iText5 time:{0}ms\t({1}s)", iText5Time, compression);
            Console.Out.WriteLine("iText7 time: {0}ms\t({1}s)", iText7Time, compression);
            String message = String.Format("{0}: {1:0.##} ({2:0.##})", compression, (double) iText5Time / iText7Time, coef);
            Assert.IsTrue(iText5Time >= iText7Time * coef, message);
            Console.Out.WriteLine(message);
            Console.Out.Flush();
        }

        protected void ComparePerformanceWithFlateFilter(bool fullCompression, float coef) {
            int pageCount = 100000;
            int runCount = 10;

            String author = "Alexander Chingarev";
            String creator = "iText";
            String title = "Empty iText Document";

            long iText5Time = 0;
            long iText7Time = 0;

            for (int i = 0; i < runCount; i++) {
                {
                    long t1 = DateTime.Now.Ticks;
                    Document.Compress = true;
                    Document document = new Document();
                    FileStream fos = new FileStream(destinationFolder + "comparePerformanceWithFlateTest_iText5.pdf", FileMode.Create);
                    iTextSharp.text.pdf.PdfWriter writer = iTextSharp.text.pdf.PdfWriter.GetInstance(document, fos);
                    if (fullCompression)
                        writer.SetFullCompression();
                    document.AddAuthor(author);
                    document.AddCreator(creator);
                    document.AddTitle(title);
                    document.Open();
                    for (int k = 0; k < pageCount; k++) {
                        document.NewPage();
                        PdfContentByte cb = writer.DirectContent;
                        cb.Rectangle(100, 100, 100, 100);
                        cb.Fill();
                    }
                    document.Close();
                    long t2 = DateTime.Now.Ticks;
                    iText5Time += (t2 - t1) / 10000;
                }

                {
                    long t1 = DateTime.Now.Ticks;
                    FileStream fos = new FileStream(destinationFolder + "comparePerformanceWithFlateTest_iText6.pdf", FileMode.Create);
                    PdfWriter writer = new PdfWriter(fos,
                            new WriterProperties()
                                    .SetFullCompressionMode(fullCompression)
                                    .SetCompressionLevel(CompressionConstants.NO_COMPRESSION));
                    PdfDocument pdfDoc = new PdfDocument(writer);
                    pdfDoc.GetDocumentInfo().SetAuthor(author).
                            SetCreator(creator).
                            SetTitle(title);
                    for (int k = 0; k < pageCount; k++) {
                        PdfPage page = pdfDoc.AddNewPage();
                        PdfCanvas canvas = new PdfCanvas(page);
                        canvas.Rectangle(100, 100, 100, 100).Fill();
                        canvas.Release();
                        page.Flush();
                    }
                    pdfDoc.Close();
                    long t2 = DateTime.Now.Ticks;
                    iText7Time += (t2 - t1) / 10000;
                }
            }

            String compression = fullCompression ? "compression, flate" : "no compression, flate";
            Console.Out.WriteLine("iText5 time:{0}dms\t({1}s)", iText5Time, compression);
            Console.Out.WriteLine("iText7 time: {0}ms\t({1}s)", iText7Time, compression);
            String message = String.Format("{0}: {1:0.##} ({2:0.##})", compression, (double) iText5Time / iText7Time, coef);
            Assert.IsTrue(iText5Time >= iText7Time * coef, message);
            Console.Out.WriteLine(message);
            Console.Out.Flush();
        }
    }
}
