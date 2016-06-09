
using System;
using iTextSharp.Forms;
using iTextSharp.IO.Source;
using iTextSharp.Kernel.Pdf;
using iTextSharp.text;
using iTextSharp.text.pdf;
using NUnit.Framework;
using PdfDocument = iTextSharp.Kernel.Pdf.PdfDocument;
using PdfReader = iTextSharp.Kernel.Pdf.PdfReader;
using PdfWriter = iTextSharp.Kernel.Pdf.PdfWriter;

namespace iTextSharp.Profiling
{
    class PdfCopyTest
    {
        public static readonly String sourceFolder = NUnit.Framework.TestContext.CurrentContext
            .TestDirectory + "/../../resources/itextsharp/profiling/PdfCopyTest/";

        [Test]
        [Timeout(300000)]
        public void CopyPagesTest() {
            int runCount = 10;
            long iText5Time = 0;
            long iText7Time = 0;
            float coef = 5.0f;
            String fileName = sourceFolder + "largeFile.pdf";

            for (int i = 0; i < runCount; i++) {
                {
                    long t1 = DateTime.Now.Ticks;
                    Document.Compress = false;

                    Document document = new Document();
                    iTextSharp.text.pdf.PdfReader reader = new iTextSharp.text.pdf.PdfReader(fileName);
                    PdfSmartCopy copy = new PdfSmartCopy(document, new ByteArrayOutputStream());
                    document.Open();
                    copy.AddDocument(reader);
                    document.Close();

                    long t2 = DateTime.Now.Ticks;
                    iText5Time += (t2 - t1) / 10000;
                }
                {
                    long t1 = DateTime.Now.Ticks;

                    PdfReader reader = new PdfReader(fileName);
                    PdfDocument fromDocument = new PdfDocument(reader);
                    PdfWriter writer = new PdfWriter(new ByteArrayOutputStream());
                    writer.SetCompressionLevel(CompressionConstants.NO_COMPRESSION);
                    PdfDocument toDocument = new PdfDocument(writer);
                    fromDocument.CopyPagesTo(1, fromDocument.GetNumberOfPages(), toDocument);

                    fromDocument.Close();
                    toDocument.Close();
                    long t2 = DateTime.Now.Ticks;
                    iText7Time += (t2 - t1) / 10000;
                }
            }
            String compression = "no compression";
            Console.Out.WriteLine("iText5 time:{0}ms\t({1}s)", iText5Time, compression);
            Console.Out.WriteLine("iText7 time: {0}ms\t({1}s)", iText7Time, compression);

            String message = String.Format("{0}: {1:0.##} ({2:0.##})", compression, (double)iText5Time / iText7Time, coef);
            Assert.IsTrue(iText5Time >= iText7Time * coef, message);
            Console.Out.WriteLine(message);
            Console.Out.Flush();
        }

        [Test]
        [Timeout(300000)]
        public void CopyPagesWithFieldsTest()
        {
            int runCount = 10;
            long iText5Time = 0;
            long iText7Time = 0;
            float coef = 5.0f;
            String fileName = sourceFolder + "largeFile.pdf";

            for (int i = 0; i < runCount; i++)
            {
                {
                    long t1 = DateTime.Now.Ticks;
                    Document.Compress = false;

                    Document document = new Document();
                    iTextSharp.text.pdf.PdfReader reader = new iTextSharp.text.pdf.PdfReader(fileName);
                    PdfSmartCopy copy = new PdfSmartCopy(document, new ByteArrayOutputStream());
                    document.Open();
                    copy.SetMergeFields();
                    copy.AddDocument(reader);
                    document.Close();

                    long t2 = DateTime.Now.Ticks;
                    iText5Time += (t2 - t1) / TimeSpan.TicksPerMillisecond;
                }
                {
                    long t1 = DateTime.Now.Ticks;

                    PdfReader reader = new PdfReader(fileName);
                    PdfDocument fromDocument = new PdfDocument(reader);
                    PdfWriter writer = new PdfWriter(new ByteArrayOutputStream());
                    writer.SetCompressionLevel(CompressionConstants.NO_COMPRESSION);
                    PdfDocument toDocument = new PdfDocument(writer);
                    fromDocument.CopyPagesTo(1, fromDocument.GetNumberOfPages(), toDocument, new PdfPageFormCopier());

                    fromDocument.Close();
                    toDocument.Close();
                    long t2 = DateTime.Now.Ticks;
                    iText7Time += (t2 - t1) / TimeSpan.TicksPerMillisecond;
                }
            }
            String compression = "no compression";
            Console.Out.WriteLine("iText5 time:{0}ms\t({1}s)", iText5Time, compression);
            Console.Out.WriteLine("iText7 time: {0}ms\t({1}s)", iText7Time, compression);

            String message = String.Format("{0}: {1:0.##} ({2:0.##})", compression, (double)iText5Time / iText7Time, coef);
            Assert.IsTrue(iText5Time >= iText7Time * coef, message);
            Console.Out.WriteLine(message);
            Console.Out.Flush();
        }


    }
}
