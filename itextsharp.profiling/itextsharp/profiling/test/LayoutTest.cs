
using System;
using System.IO;
using System.Text;
using iTextSharp.IO.Font;
using iTextSharp.Kernel.Font;
using iTextSharp.Layout;
using iTextSharp.Layout.Element;
using iTextSharp.text.pdf;
using NUnit.Framework;
using List = iTextSharp.Layout.Element.List;
using ListItem = iTextSharp.text.ListItem;
using PdfDocument = iTextSharp.Kernel.Pdf.PdfDocument;
using PdfFont = iTextSharp.Kernel.Font.PdfFont;
using PdfWriter = iTextSharp.Kernel.Pdf.PdfWriter;

namespace itextsharp.profiling.itextsharp.profiling.test
{
    class LayoutTest
    {
        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext
            .TestDirectory + "/test/itextsharp/profiling/LayoutTest/";

        [OneTimeSetUp]
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
        [Timeout(300000)]
        public void ComparePerformanceWithText01() {
            ComparePerformanceWithLongText(200000, 5, 7.5);
        }

        [Test]
        [Timeout(300000)]
        public void ComparePerformanceWithText02() {
            ComparePerformanceWithLongText(20000, 50, 1.45);
        }

        [Test]
        [Timeout(300000)]
        public void ComparePerformanceWithText03() {
            ComparePerformanceWithLongText(10, 1000, 1.15);
        }

        [Test]
        [Timeout(300000)]
        public void ComparePerformanceWithLongTables01() {
            ComparePerformanceWithLongTables(200000, 1, 0.9);
        }

        [Test]
        [Timeout(300000)]
        public void ComparePerformanceWithLists01() {
            ComparePerformanceWithLists(50000, 3, 0.9);
        }

        
        protected void ComparePerformanceWithLongText(int textRepeatCount, int runCount, double coef)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < textRepeatCount; i++)
            {
                sb.Append("iText");
            }
            String longStr = sb.ToString();

            long iText5Time = 0;
            long iText7Time = 0;

            for (int i = 0; i < runCount; i++) {
                {
                    long t1 = DateTime.Now.Ticks;

                    iTextSharp.text.Document document = new iTextSharp.text.Document();
                    FileStream fos = new FileStream(destinationFolder + "comparePerformanceWithLongText_iText5.pdf", FileMode.Create);
                    iTextSharp.text.pdf.PdfWriter writer = iTextSharp.text.pdf.PdfWriter.GetInstance(document, fos);
                    document.Open();
                    document.Add(new iTextSharp.text.Paragraph(longStr));
                    document.Close();

                    long t2 = DateTime.Now.Ticks;
                    iText5Time += (t2 - t1) / 10000;
                }
                {
                    long t1 = DateTime.Now.Ticks;

                    FileStream fos = new FileStream(destinationFolder + "comparePerformanceWithLongText_iText7.pdf", FileMode.Create);
                    PdfDocument pdfDocument = new PdfDocument(new PdfWriter(fos));
                    Document document = new Document(pdfDocument);
                    PdfFont font = PdfFontFactory.CreateFont(FontProgramFactory.CreateFont());
                    document.Add(new Paragraph(longStr).SetFont(font));
                    document.Close();

                    long t2 = DateTime.Now.Ticks;
                    iText7Time += (t2 - t1) / 10000;
                }
            }

            Console.Out.WriteLine("iText5 time: {0}ms\t", iText5Time);
            Console.Out.WriteLine("iText7 time: {0}ms\t", iText7Time);
            String message = String.Format("{0:0.##} ({1:0.##})", (double)iText5Time / iText7Time, coef);
            Assert.IsTrue(iText5Time >= iText7Time * coef, message);
            Console.Out.WriteLine(message);
            Console.Out.Flush();
        }
        
        protected void ComparePerformanceWithLongTables(int cellCount, int runCount, double coef)
        {
            long iText5Time = 0;
            long iText7Time = 0;

            for (int i = 0; i < runCount; i++)
            {
                {
                    long t1 = DateTime.Now.Ticks;

                    iTextSharp.text.Document document = new iTextSharp.text.Document();
                    FileStream fos = new FileStream(destinationFolder + "comparePerformanceWithLongTables_iText5.pdf", FileMode.Create);
                    iTextSharp.text.pdf.PdfWriter.GetInstance(document, fos);
                    document.Open();
                    PdfPTable table = new PdfPTable(5);
                    table.SplitRows = false;
                    table.Complete = false;

                    for (int j = 0; j < cellCount; j++)
                    {
                        if (j % 5 == 1)
                        {
                            document.Add(table);
                        }
                        table.AddCell(String.Format("This is a cell which might in theory wrap for the sake of the good testing. The magic number of this cell is {0}", j + 1));
                    }

                    table.Complete = true;
                    document.Add(table);

                    document.Close();

                    long t2 = DateTime.Now.Ticks;
                    iText5Time += (t2 - t1) / 10000;
                }
                {
                    long t1 = DateTime.Now.Ticks;

                    FileStream fos = new FileStream(destinationFolder + "comparePerformanceWithLongTables_iText7.pdf", FileMode.Create);
                    PdfDocument pdfDocument = new PdfDocument(new PdfWriter(fos));
                    Document document = new Document(pdfDocument);
                    Table table = new Table(5, true);
                    table.SetKeepTogether(true);
                    document.Add(table);

                    for (int j = 0; j < cellCount; j++)
                    {
                        if (j % 5 == 1)
                        {
                            table.Flush();
                        }
                        table.AddCell(new Cell().SetKeepTogether(true).
                                Add(new Paragraph(String.Format("This is a cell which might in theory wrap for the sake of the good testing. The magic number of this cell is {0}", j + 1))));
                    }

                    table.Complete();
                    document.Close();

                    long t2 = DateTime.Now.Ticks;
                    iText7Time += (t2 - t1) / 10000;
                }
            }

            Console.Out.WriteLine("iText5 time: {0}ms\t", iText5Time);
            Console.Out.WriteLine("iText7 time: {0}ms\t", iText7Time);
            String message = String.Format("{0:0.##} ({1:0.##})", (double)iText5Time / iText7Time, coef);
            Assert.IsTrue(iText5Time >= iText7Time * coef, message);
            Console.Out.WriteLine(message);
            Console.Out.Flush();
        }

        protected void ComparePerformanceWithLists(int itemCount, int runCount, double coef)
        {
            long iText5Time = 0;
            long iText7Time = 0;

            for (int i = 0; i < runCount; i++)
            {
                {
                    long t1 = DateTime.Now.Ticks;

                    iTextSharp.text.Document document = new iTextSharp.text.Document();
                    FileStream fos = new FileStream(destinationFolder + "comparePerformanceWithLongTables_iText5.pdf", FileMode.Create);
                    iTextSharp.text.pdf.PdfWriter.GetInstance(document, fos);
                    document.Open();
                    iTextSharp.text.List list = new iTextSharp.text.List();
                    for (int j = 0; j < itemCount; j++)
                    {
                        list.Add(new ListItem("Hello " + j));
                    }

                    document.Add(list);

                    document.Close();

                    long t2 = DateTime.Now.Ticks;
                    iText5Time += (t2 - t1) / 10000;
                }
                {
                    long t1 = DateTime.Now.Ticks;

                    FileStream fos = new FileStream(destinationFolder + "comparePerformanceWithLongTables_iText7.pdf", FileMode.Create);
                    PdfDocument pdfDocument = new PdfDocument(new PdfWriter(fos));
                    Document document = new Document(pdfDocument);
                    List list = new List();
                    for (int j = 0; j < itemCount; j++)
                    {
                        list.Add("Hello " + j);
                    }

                    document.Add(list);
                    document.Close();

                    long t2 = DateTime.Now.Ticks;
                    iText7Time += (t2 - t1) / 10000;
                }
            }

            Console.Out.WriteLine("iText5 time: {0}ms\t", iText5Time);
            Console.Out.WriteLine("iText7 time: {0}ms\t", iText7Time);
            String message = String.Format("{0:0.##} ({1:0.##})", (double)iText5Time / iText7Time, coef);
            Assert.IsTrue(iText5Time >= iText7Time * coef, message);
            Console.Out.WriteLine(message);
            Console.Out.Flush();
        }
    }
}
