using System;
using System.IO;
using System.Threading;
using iTextSharp.IO.Font;
using iTextSharp.IO.Util;
using iTextSharp.Kernel.Font;
using iTextSharp.Kernel.Pdf;
using iTextSharp.Layout;
using iTextSharp.Layout.Element;
using NUnit.Framework;
using PdfDocument = iTextSharp.Kernel.Pdf.PdfDocument;

namespace iTextSharp.Profiling
{
    class PdfFontTest : PdfCanvasTest
    {
        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext
            .TestDirectory + "/test/itextsharp/profiling/PdfFontTest/";
        public static readonly String sourceFolder = NUnit.Framework.TestContext.CurrentContext
            .TestDirectory + "/../../resources/itextsharp/profiling/PdfFontTest/";
        int itersPerThread = 10;

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass()
        {
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

        [Test]
        public void TestCache()
        {
            int threadNum = 1000;

            Thread[] threads = new Thread[threadNum];

            for (int i = 0; i < threads.Length; ++i) {
                threads[i] = new Thread(Run); 

                threads[i].Start();
                Thread.Sleep(50);
            }
        }

        public void Run() {
            for (int i = 0; i < itersPerThread; ++i)
            {
                try
                {
                    FileStream fos = new FileStream(destinationFolder + Thread.CurrentThread.Name + "_" + i + ".pdf", FileMode.Create);
                    PdfDocument pdfDocument = new PdfDocument(new PdfWriter(fos));
                    Document document = new Document(pdfDocument);
                    PdfFont helvetica = PdfFontFactory.CreateFont(FontProgramFactory.CreateFont());
                    PdfFont courier = PdfFontFactory.CreateFont(FontProgramFactory.CreateFont(FontConstants.COURIER));
                    PdfFont times = PdfFontFactory.CreateFont(FontProgramFactory.CreateFont(FontConstants.TIMES_ROMAN));
                    PdfFont kozmin = PdfFontFactory.CreateFont(FontProgramFactory.CreateFont("KozMinPro-Regular"), "78-EUC-H");

                    byte[] ttf = StreamUtil.InputStreamToArray(new FileStream(sourceFolder + "abserif4_5.ttf", FileMode.Open));
                    PdfFont serif = PdfFontFactory.CreateFont(FontProgramFactory.CreateFont("Aboriginal Serif.ttf", ttf, false));

                    document.Add(new Paragraph("Hello, world-helvetica!").SetFont(helvetica));
                    document.Add(new Paragraph("Hello, world-courier!").SetFont(courier));
                    document.Add(new Paragraph("Hello, world-times-roman!").SetFont(times));
                    document.Add(new Paragraph("Hello, world-KozMinPro-Regular!").SetFont(kozmin));
                    document.Add(new Paragraph("Hello, world-serif!").SetFont(serif));
                    document.Close();
                }
                catch (IOException e) {
                    Console.WriteLine(e.Message);
                }
            }
        }

        [NUnit.Framework.OneTimeTearDownAttribute]
        public static void AfterClass() {
            DeleteFolder(destinationFolder);
        }

        private static void DeleteFolder(String folder) {
            String[] files = Directory.GetFiles(folder);

            foreach (String f in files)
            {
                if (Directory.Exists(f))
                {
                    DeleteFolder(f);
                }
                else
                {
                    File.Delete(f);
                }
            }

            

        }
    }
}
