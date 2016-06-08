using System;
using System.IO;
using iTextSharp.text.pdf;
using NUnit.Framework;
using PdfDictionary = iTextSharp.text.pdf.PdfDictionary;
using PdfDocument = iTextSharp.Kernel.Pdf.PdfDocument;
using PdfName = iTextSharp.Kernel.Pdf.PdfName;
using PdfPage = iTextSharp.Kernel.Pdf.PdfPage;
using PdfReader = iTextSharp.Kernel.Pdf.PdfReader;

namespace iTextSharp.Profiling
{
    class PdfReaderTest
    {
        public static readonly String sourceFolder = NUnit.Framework.TestContext.CurrentContext
            .TestDirectory + "/../../resources/itextsharp/profiling/PdfReaderTest/";
        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext
            .TestDirectory + "/test/itextsharp/profiling/PdfReaderTest/";

        protected void ComparePerformance(String filename, String message, float coef)
        {
            int runCount = 10;
            long iText5Time = 0;
            long iText7Time = 0;

            for (int i = 0; i < runCount; i++)
            {
                {
                    long t1 = DateTime.Now.Ticks;
                    iTextSharp.text.pdf.PdfReader reader = new iTextSharp.text.pdf.PdfReader(new FileStream(filename, FileMode.Open));
                    int pageCount = reader.NumberOfPages;
                    for (int k = 1; k < pageCount + 1; k++)
                    {
                        PdfDictionary page = reader.GetPageN(k);
                        page.Get(iTextSharp.text.pdf.PdfName.MEDIABOX);
                        iTextSharp.text.pdf.PdfReader.GetStreamBytes((PRStream)page.GetAsStream(iTextSharp.text.pdf.PdfName.CONTENTS));
                    }
                    reader.Close();
                    long t2 = DateTime.Now.Ticks;
                    iText5Time += (t2 - t1) / 10000;
                }
                {
                    long t1 = DateTime.Now.Ticks;
                    PdfReader reader = new PdfReader(new FileStream(filename, FileMode.Open));

                    PdfDocument pdfDoc = new PdfDocument(reader);
                    int pageCount = pdfDoc.GetNumberOfPages();
                    for (int k = 1; k < pageCount + 1; k++)
                    {
                        PdfPage page = pdfDoc.GetPage(k);
                        page.GetPdfObject().Get(PdfName.MediaBox);
                        page.GetContentStream(0).GetBytes();
                    }
                    pdfDoc.Close();
                    long t2 = DateTime.Now.Ticks;
                    iText7Time += (t2 - t1) / 10000;
                }
            }

            Console.Out.WriteLine("iText5 time:{0}ms\t({1}s)", iText5Time, message);
            Console.Out.WriteLine("iText7 time: {0}ms\t({1}s)", iText7Time, message);
            message = String.Format("{0}: {1:0.##} ({2:0.##})", message, (double)iText5Time / iText7Time, coef);
            Console.Out.WriteLine(message);
            Console.Out.Flush();
            Assert.IsTrue(iText5Time >= iText7Time * coef, message);
        }

        protected void ComparePerformancePartial(String filename, String message, float coef)
        {
            int runCount = 10;
            long iText5Time = 0;
            long iText7Time = 0;

            for (int i = 0; i < runCount; i++)
            {
                {
                    long t1 = DateTime.Now.Ticks;
                    iTextSharp.text.pdf.PdfReader reader = new iTextSharp.text.pdf.PdfReader(new FileStream(filename, FileMode.Open));
                    int pageCount = reader.NumberOfPages;
                    for (int k = 1; k < pageCount + 1; k += 10)
                    {
                        if (k > pageCount) 
                            break;
                        PdfDictionary page = reader.GetPageN(k + 1);
                        page.Get(iTextSharp.text.pdf.PdfName.MEDIABOX);
                        iTextSharp.text.pdf.PdfReader.GetStreamBytes((PRStream)page.GetAsStream(iTextSharp.text.pdf.PdfName.CONTENTS));
                    }
                    reader.Close();
                    long t2 = DateTime.Now.Ticks;
                    iText5Time += (t2 - t1) / 10000;
                }
                {
                    long t1 = DateTime.Now.Ticks;
                    PdfReader reader = new PdfReader(new FileStream(filename, FileMode.Open));

                    PdfDocument pdfDoc = new PdfDocument(reader);
                    int pageCount = pdfDoc.GetNumberOfPages();
                    for (int k = 1; k < pageCount + 1; k += 10)
                    {
                        if (k > pageCount)
                            break;
                        PdfPage page = pdfDoc.GetPage(k + 1);
                        page.GetPdfObject().Get(PdfName.MediaBox);
                        page.GetContentStream(0).GetBytes();
                    }
                    pdfDoc.Close();
                    long t2 = DateTime.Now.Ticks;
                    iText7Time += (t2 - t1) / 10000;
                }
            }

            Console.Out.WriteLine("iText5 time:{0}ms\t({1}s)", iText5Time, message);
            Console.Out.WriteLine("iText7 time: {0}ms\t({1}s)", iText7Time, message);
            message = String.Format("{0}: {1:0.##} ({2:0.##})", message, (double)iText5Time / iText7Time, coef);
            Console.Out.WriteLine(message);
            Console.Out.Flush();
            Assert.IsTrue(iText5Time >= iText7Time * coef, message);
        }
    }
}
