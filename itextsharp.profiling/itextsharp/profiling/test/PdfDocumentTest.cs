using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using iTextSharp.IO.Font;
using iTextSharp.Kernel.Font;
using iTextSharp.Kernel.Pdf;
using iTextSharp.Kernel.Pdf.Canvas;
using iTextSharp.text;
using iTextSharp.text.pdf;
using NUnit.Framework;
using PdfArray = iTextSharp.Kernel.Pdf.PdfArray;
using PdfDictionary = iTextSharp.text.pdf.PdfDictionary;
using PdfDocument = iTextSharp.Kernel.Pdf.PdfDocument;
using PdfName = iTextSharp.Kernel.Pdf.PdfName;
using PdfPage = iTextSharp.Kernel.Pdf.PdfPage;
using PdfReader = iTextSharp.Kernel.Pdf.PdfReader;
using PdfWriter = iTextSharp.Kernel.Pdf.PdfWriter;
using Rectangle = iTextSharp.Kernel.Geom.Rectangle;

namespace itextsharp.profiling.itextsharp.profiling.test
{
    class PdfDocumenTest
    {
        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext
            .TestDirectory + "/test/itextsharp/profiling/PdfDocumentTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            iTextSharp.text.pdf.PdfDocument.Compress = false;
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

        protected void RemoveThreePages(String filename, bool fullCompression, float coef)
        {
            int runCount = 10;
            long iText5Time = 0;
            long iText7Time = 0;

            IList<int> pagesToKeep = new List<int>();
            for (int i = 1; i <= 100000; i++) {
                pagesToKeep.Add(i);
            }
            pagesToKeep.Remove(10);
            pagesToKeep.Remove(50000);
            pagesToKeep.Remove(99990);

            for (int i = 0; i < runCount; i++)
            {
                {
                    long t1 = DateTime.Now.Ticks;
                    Document.Compress = false;
                    String filename2 = String.Format("{0}removeThreePagesIText5{1}.pdf", destinationFolder, fullCompression ? "WithFC" : "");
                    iTextSharp.text.pdf.PdfReader reader = new iTextSharp.text.pdf.PdfReader(new FileStream(filename, FileMode.Open));

                    reader.SelectPages(pagesToKeep);
                    PdfStamper stamper = new PdfStamper(reader, new FileStream(filename2, FileMode.Create));
                    stamper.Close();
                    long t2 = DateTime.Now.Ticks;
                    iText5Time += (t2 - t1) / 10000;
                }
                {
                    long t1 = DateTime.Now.Ticks;
                    String filename2 = String.Format("{0}removeThreePagesIText7{1}.pdf", destinationFolder, fullCompression ? "WithFC" : "");
                    PdfReader reader = new PdfReader(new FileStream(filename, FileMode.Open));
                    PdfWriter writer = new PdfWriter(new FileStream(filename2, FileMode.Create),
                            new WriterProperties().SetFullCompressionMode(fullCompression));
                    writer.SetCompressionLevel(CompressionConstants.NO_COMPRESSION);
                    PdfDocument document = new PdfDocument(reader, writer);

                    document.RemovePage(10);
                    document.RemovePage(50000);
                    document.RemovePage(99990);
                    document.Close();
                    long t2 = DateTime.Now.Ticks;
                    iText7Time += (t2 - t1) / 10000;
                }
            }

            String compression = "Remove three pages with " + (fullCompression ? "compression" : "no compression");
            Console.Out.WriteLine("iText5 time:{0}ms\t({1}s)", iText5Time, compression);
            Console.Out.WriteLine("iText7 time: {0}ms\t({1}s)", iText7Time, compression);
            String message = String.Format("{0}: {1:0.##} ({2:0.##})", compression, (double) iText5Time / iText7Time, coef);
            Assert.IsTrue(iText5Time >= iText7Time * coef, message);
            Console.Out.WriteLine(message);
            Console.Out.Flush();
        }

        protected void LeaveThreePages(String filename, bool fullCompression, float coef)
        {
            int runCount = 10;
            long iText5Time = 0;
            long iText7Time = 0;


            IList<int> pagesToKeep = new int[] { 10, 50000, 99990 }.ToList();

            for (int i = 0; i < runCount; i++)
            {
                {
                    long t1 = DateTime.Now.Ticks;
                    Document.Compress = false;
                    String filename2 = String.Format("{0}leaveThreePagesIText5{1}.pdf", destinationFolder, fullCompression ? "WithFC" : "");
                    iTextSharp.text.pdf.PdfReader reader = new iTextSharp.text.pdf.PdfReader(new FileStream(filename, FileMode.Open));

                    reader.SelectPages(pagesToKeep);
                    PdfStamper stamper = new PdfStamper(reader, new FileStream(filename2, FileMode.Create));
                    stamper.Close();
                    long t2 = DateTime.Now.Ticks;
                    iText5Time += (t2 - t1) / 10000;
                }
                {
                    long t1 = DateTime.Now.Ticks;
                    String filename2 = String.Format("{0}leaveThreePagesIText7{1}.pdf", destinationFolder, fullCompression ? "WithFC" : "");
                    PdfReader reader = new PdfReader(new FileStream(filename, FileMode.Open));
                    PdfWriter writer = new PdfWriter(new FileStream(filename2, FileMode.Create),
                            new WriterProperties().SetFullCompressionMode(fullCompression));
                    writer.SetCompressionLevel(CompressionConstants.NO_COMPRESSION);
                    PdfDocument document = new PdfDocument(reader, writer);

                    for (int p = 100000; p > 0; p--)
                    {
                        if (p == 10 || p == 50000 || p == 99990) continue;
                        document.RemovePage(p);
                    }
                    document.Close();
                    long t2 = DateTime.Now.Ticks;
                    iText7Time += (t2 - t1) / 10000;
                }
            }

            String compression = "Leave three pages with " + (fullCompression ? "compression" : "no compression");
            Console.Out.WriteLine("iText5 time:{0}ms\t({1}s)", iText5Time, compression);
            Console.Out.WriteLine("iText7 time: {0}ms\t({1}s)", iText7Time, compression);
            String message = String.Format("{0}: {1:0.##} ({2:0.##})", compression, (double)iText5Time / iText7Time, coef);
            Assert.IsTrue(iText5Time >= iText7Time * coef, message);
            Console.Out.WriteLine(message);
            Console.Out.Flush();
        }

        protected void ChangeMediaBox(String filename, bool fullCompression, float coef)
        {
            int runCount = 10;
            long iText5Time = 0;
            long iText7Time = 0;

            for (int i = 0; i < runCount; i++) {
                {
                    long t1 = DateTime.Now.Ticks;
                    Document.Compress = false;
                    String filename2 = String.Format("{0}changeMediaBoxIText5{1}.pdf", destinationFolder, fullCompression ? "WithFC":"");

                    iTextSharp.text.pdf.PdfReader reader = new iTextSharp.text.pdf.PdfReader(new FileStream(filename, FileMode.Open));
                    PdfStamper stamper = new PdfStamper(reader, new FileStream(filename2, FileMode.Create));
                    if (fullCompression) stamper.SetFullCompression();
                    for (int p = 1; p <= reader.NumberOfPages; p++){
                        PdfDictionary page = reader.GetPageN(p);
                        page.Put(iTextSharp.text.pdf.PdfName.MEDIABOX, new PdfRectangle(0, 0, 610, 790));
                    }

                    stamper.Close();
                    long t2 = DateTime.Now.Ticks;
                    iText5Time += (t2 - t1) / 10000;
                }
                {
                    long t1 = DateTime.Now.Ticks;
                    String filename2 = String.Format("{0}changeMediaBoxIText7{1}.pdf", destinationFolder, fullCompression ? "WithFC":"");

                    PdfReader reader = new PdfReader(new FileStream(filename, FileMode.Open));
                    PdfWriter writer = new PdfWriter(new FileStream(filename2, FileMode.Create),
                            new WriterProperties().SetFullCompressionMode(fullCompression));
                    writer.SetCompressionLevel(CompressionConstants.NO_COMPRESSION);
                    PdfDocument document = new PdfDocument(reader, writer);
                    for (int p = 1; p <= document.GetNumberOfPages(); p++){
                        PdfPage page = document.GetPage(p);
                        page.GetPdfObject().Put(PdfName.MediaBox, new PdfArray(new Rectangle(0, 0, 610, 790)));
                    }

                    document.Close();
                    long t2 = DateTime.Now.Ticks;
                    iText7Time += (t2 - t1) / 10000;
                }
            }
            String compression = "Change media box with " + (fullCompression ? "compression" : "no compression");
            Console.Out.WriteLine("iText5 time:{0}ms\t({1}s)", iText5Time, compression);
            Console.Out.WriteLine("iText7 time: {0}ms\t({1}s)", iText7Time, compression);
            String message = String.Format("{0}: {1:0.##} ({2:0.##})", compression, (double)iText5Time / iText7Time, coef);
            Assert.IsTrue(iText5Time >= iText7Time * coef, message);
            Console.Out.WriteLine(message);
            Console.Out.Flush();
        }

        protected void AppendContentStream(String filename, bool fullCompression, float coef)
        {
            int runCount = 10;
            long iText5Time = 0;
            long iText7Time = 0;

            int x = 10;
            int y = 760;
            int fontSize = 36;

            for (int i = 0; i < runCount; i++)
            {
                {
                    long t1 = DateTime.Now.Ticks;
                    Document.Compress = false;
                    String filename2 = String.Format("{0}appendContentStreamIText5{1}.pdf", destinationFolder, fullCompression ? "WithFC" : "");

                    iTextSharp.text.pdf.PdfReader reader = new iTextSharp.text.pdf.PdfReader(new FileStream(filename, FileMode.Open));
                    PdfStamper stamper = new PdfStamper(reader, new FileStream(filename2, FileMode.Create));
                    if (fullCompression) stamper.SetFullCompression();
                    for (int p = 1; p <= reader.NumberOfPages; p++)
                    {
                        PdfContentByte contentByte = stamper.GetOverContent(p);
                        contentByte.SaveState();
                        contentByte.BeginText();
                        contentByte.MoveText(x, y);
                        contentByte.SetFontAndSize(FontFactory.GetFont(FontFactory.HELVETICA_OBLIQUE).BaseFont, fontSize);
                        contentByte.ShowText("Updated with iText 5");
                        contentByte.EndText();
                        contentByte.RestoreState();
                    }

                    stamper.Close();
                    long t2 = DateTime.Now.Ticks;
                    iText5Time += (t2 - t1) / 10000;
                }
                {
                    long t1 = DateTime.Now.Ticks;
                    String filename2 = String.Format("{0}changeMediaBoxIText7{1}.pdf", destinationFolder, fullCompression ? "WithFC" : "");

                    PdfReader reader = new PdfReader(new FileStream(filename, FileMode.Open));
                    PdfWriter writer = new PdfWriter(new FileStream(filename2, FileMode.Create),
                            new WriterProperties().SetFullCompressionMode(fullCompression));
                    writer.SetCompressionLevel(CompressionConstants.NO_COMPRESSION);
                    PdfDocument document = new PdfDocument(reader, writer);
                    for (int p = 1; p <= document.GetNumberOfPages(); p++)
                    {
                        PdfPage page = document.GetPage(p);
                        page.NewContentStreamAfter();
                        PdfCanvas canvas = new PdfCanvas(page);
                        canvas
                                .SaveState()
                                .BeginText()
                                .MoveText(x, y)
                                .SetFontAndSize(PdfFontFactory.CreateFont(FontConstants.HELVETICA_OBLIQUE), fontSize)
                                .ShowText("Updated with iText 7")
                                .EndText()
                                .RestoreState();

                        canvas.Release();
                        page.Flush();
                    }

                    document.Close();
                    long t2 = DateTime.Now.Ticks;
                    iText7Time += (t2 - t1) / 10000;
                }
            }
            String compression = "Append content stream with " + (fullCompression ? "compression" : "no compression");
            Console.Out.WriteLine("iText5 time:{0}ms\t({1}s)", iText5Time, compression);
            Console.Out.WriteLine("iText7 time: {0}ms\t({1}s)", iText7Time, compression);
            String message = String.Format("{0}: {1:0.##} ({2:0.##})", compression, (double)iText5Time / iText7Time, coef);
            Assert.IsTrue(iText5Time >= iText7Time * coef, message);
            Console.Out.WriteLine(message);
            Console.Out.Flush();
        }
    }
}
