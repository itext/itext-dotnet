using System;
using System.Collections.Generic;
using System.IO;
using iTextSharp.IO;
using iTextSharp.Kernel.Pdf.Navigation;
using iTextSharp.Kernel.Utils;
using iTextSharp.Test;
using iTextSharp.Test.Attributes;

namespace iTextSharp.Kernel.Pdf
{
    public class PdfOutlineTest : ExtendedITextTest
    {
        public static readonly String sourceFolder = NUnit.Framework.TestContext.CurrentContext
            .TestDirectory + "/../../resources/itextsharp/kernel/pdf/PdfOutlineTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext
            .TestDirectory + "/test/itextsharp/kernel/pdf/PdfOutlineTest/";

        /// <exception cref="System.IO.FileNotFoundException"/>
        [NUnit.Framework.TestFixtureSetUp]
        public static void BeforeClass()
        {
            CreateDestinationFolder(destinationFolder);
            FileStream fos = new FileStream(destinationFolder + "documentWithOutlines.pdf", FileMode.Create
                );
            PdfWriter writer = new PdfWriter(fos);
            PdfDocument pdfDoc = new PdfDocument(writer);
            pdfDoc.GetCatalog().SetPageMode(PdfName.UseOutlines);
            PdfPage firstPage = pdfDoc.AddNewPage();
            PdfPage secondPage = pdfDoc.AddNewPage();
            PdfOutline rootOutline = pdfDoc.GetOutlines(false);
            PdfOutline firstOutline = rootOutline.AddOutline("First Page");
            PdfOutline secondOutline = rootOutline.AddOutline("Second Page");
            firstOutline.AddDestination(PdfExplicitDestination.CreateFit(firstPage));
            secondOutline.AddDestination(PdfExplicitDestination.CreateFit(secondPage));
            pdfDoc.Close();
        }

        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void OutlinesTest()
        {
            PdfReader reader = new PdfReader(new FileStream(sourceFolder + "iphone_user_guide.pdf"
                , FileMode.Open, FileAccess.Read));
            PdfDocument pdfDoc = new PdfDocument(reader);
            PdfOutline outlines = pdfDoc.GetOutlines(false);
            IList<PdfOutline> children = outlines.GetAllChildren()[0].GetAllChildren();
            NUnit.Framework.Assert.AreEqual(outlines.GetTitle(), "Outlines");
            NUnit.Framework.Assert.AreEqual(children.Count, 13);
            NUnit.Framework.Assert.IsTrue(children[0].GetDestination() is PdfStringDestination
                );
        }

        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void OutlinesWithPagesTest()
        {
            PdfReader reader = new PdfReader(new FileStream(sourceFolder + "iphone_user_guide.pdf"
                , FileMode.Open, FileAccess.Read));
            PdfDocument pdfDoc = new PdfDocument(reader);
            PdfPage page = pdfDoc.GetPage(52);
            IList<PdfOutline> pageOutlines = page.GetOutlines(true);
            try
            {
                NUnit.Framework.Assert.AreEqual(3, pageOutlines.Count);
                NUnit.Framework.Assert.IsTrue(pageOutlines[0].GetTitle().Equals("Safari"));
                NUnit.Framework.Assert.AreEqual(pageOutlines[0].GetAllChildren().Count, 4);
            }
            finally
            {
                pdfDoc.Close();
            }
        }

        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.SetUp]
        public virtual void SetupAddOutlinesToDocumentTest()
        {
            String filename = sourceFolder + "iphone_user_guide.pdf";
            PdfReader reader = new PdfReader(new FileStream(filename, FileMode.Open, FileAccess.Read
                ));
            FileStream fos = new FileStream(destinationFolder + "addOutlinesResult.pdf", FileMode.Create
                );
            PdfWriter writer = new PdfWriter(fos);
            PdfDocument pdfDoc = new PdfDocument(reader, writer);
            pdfDoc.SetTagged();
            PdfOutline outlines = pdfDoc.GetOutlines(false);
            PdfOutline firstPage = outlines.AddOutline("firstPage");
            PdfOutline firstPageChild = firstPage.AddOutline("firstPageChild");
            PdfOutline secondPage = outlines.AddOutline("secondPage");
            PdfOutline secondPageChild = secondPage.AddOutline("secondPageChild");
            firstPage.AddDestination(PdfExplicitDestination.CreateFit(pdfDoc.GetPage(1)));
            firstPageChild.AddDestination(PdfExplicitDestination.CreateFit(pdfDoc.GetPage(1))
                );
            secondPage.AddDestination(PdfExplicitDestination.CreateFit(pdfDoc.GetPage(2)));
            secondPageChild.AddDestination(PdfExplicitDestination.CreateFit(pdfDoc.GetPage(2)
                ));
            outlines.GetAllChildren()[0].GetAllChildren()[1].AddOutline("testOutline", 1).AddDestination
                (PdfExplicitDestination.CreateFit(pdfDoc.GetPage(102)));
            pdfDoc.Close();
            fos.Close();
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void AddOutlinesToDocumentTest()
        {
            String filename = destinationFolder + "addOutlinesResult.pdf";
            PdfReader reader = new PdfReader(new FileStream(filename, FileMode.Open, FileAccess.Read
                ));
            PdfDocument pdfDoc = new PdfDocument(reader);
            PdfOutline outlines = pdfDoc.GetOutlines(false);
            try
            {
                NUnit.Framework.Assert.AreEqual(3, outlines.GetAllChildren().Count);
                NUnit.Framework.Assert.AreEqual("firstPageChild", outlines.GetAllChildren()[1].GetAllChildren
                    ()[0].GetTitle());
            }
            finally
            {
                pdfDoc.Close();
            }
        }

        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.SetUp]
        public virtual void SetupRemovePageWithOutlinesTest()
        {
            String filename = sourceFolder + "iphone_user_guide.pdf";
            PdfReader reader = new PdfReader(new FileStream(filename, FileMode.Open, FileAccess.Read
                ));
            FileStream fos = new FileStream(destinationFolder + "removePagesWithOutlinesResult.pdf"
                , FileMode.Create);
            PdfWriter writer = new PdfWriter(fos);
            PdfDocument pdfDoc = new PdfDocument(reader, writer);
            pdfDoc.RemovePage(102);
            pdfDoc.Close();
        }

        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void RemovePageWithOutlinesTest()
        {
            String filename = destinationFolder + "removePagesWithOutlinesResult.pdf";
            PdfReader reader = new PdfReader(new FileStream(filename, FileMode.Open, FileAccess.Read
                ));
            PdfDocument pdfDoc = new PdfDocument(reader);
            PdfPage page = pdfDoc.GetPage(102);
            IList<PdfOutline> pageOutlines = page.GetOutlines(false);
            try
            {
                NUnit.Framework.Assert.AreEqual(4, pageOutlines.Count);
            }
            finally
            {
                pdfDoc.Close();
            }
        }

        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.SetUp]
        public virtual void SetupUpdateOutlineTitle()
        {
            String filename = sourceFolder + "iphone_user_guide.pdf";
            PdfReader reader = new PdfReader(new FileStream(filename, FileMode.Open, FileAccess.Read
                ));
            FileStream fos = new FileStream(destinationFolder + "updateOutlineTitleResult.pdf"
                , FileMode.Create);
            PdfWriter writer = new PdfWriter(fos);
            PdfDocument pdfDoc = new PdfDocument(reader, writer);
            PdfOutline outlines = pdfDoc.GetOutlines(false);
            outlines.GetAllChildren()[0].GetAllChildren()[1].SetTitle("New Title");
            pdfDoc.Close();
        }

        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void UpdateOutlineTitle()
        {
            String filename = destinationFolder + "updateOutlineTitleResult.pdf";
            PdfReader reader = new PdfReader(new FileStream(filename, FileMode.Open, FileAccess.Read
                ));
            PdfDocument pdfDoc = new PdfDocument(reader);
            PdfOutline outlines = pdfDoc.GetOutlines(false);
            PdfOutline outline = outlines.GetAllChildren()[0].GetAllChildren()[1];
            try
            {
                NUnit.Framework.Assert.AreEqual("New Title", outline.GetTitle());
            }
            finally
            {
                pdfDoc.Close();
            }
        }

        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.SetUp]
        public virtual void SetupAddOutlineInNotOutlineMode()
        {
            String filename = sourceFolder + "iphone_user_guide.pdf";
            PdfReader reader = new PdfReader(new FileStream(filename, FileMode.Open, FileAccess.Read
                ));
            FileStream fos = new FileStream(destinationFolder + "addOutlinesWithoutOutlineModeResult.pdf"
                , FileMode.Create);
            PdfWriter writer = new PdfWriter(fos);
            PdfDocument pdfDoc = new PdfDocument(reader, writer);
            PdfOutline outlines = new PdfOutline(pdfDoc);
            PdfOutline firstPage = outlines.AddOutline("firstPage");
            PdfOutline firstPageChild = firstPage.AddOutline("firstPageChild");
            PdfOutline secondPage = outlines.AddOutline("secondPage");
            PdfOutline secondPageChild = secondPage.AddOutline("secondPageChild");
            firstPage.AddDestination(PdfExplicitDestination.CreateFit(pdfDoc.GetPage(1)));
            firstPageChild.AddDestination(PdfExplicitDestination.CreateFit(pdfDoc.GetPage(1))
                );
            secondPage.AddDestination(PdfExplicitDestination.CreateFit(pdfDoc.GetPage(2)));
            secondPageChild.AddDestination(PdfExplicitDestination.CreateFit(pdfDoc.GetPage(2)
                ));
            pdfDoc.Close();
        }

        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void AddOutlineInNotOutlineMode()
        {
            String filename = destinationFolder + "addOutlinesWithoutOutlineModeResult.pdf";
            PdfReader reader = new PdfReader(new FileStream(filename, FileMode.Open, FileAccess.Read
                ));
            PdfDocument pdfDoc = new PdfDocument(reader);
            IList<PdfOutline> pageOutlines = pdfDoc.GetPage(102).GetOutlines(true);
            try
            {
                NUnit.Framework.Assert.AreEqual(5, pageOutlines.Count);
            }
            finally
            {
                pdfDoc.Close();
            }
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void CreateDocWithOutlines()
        {
            String filename = destinationFolder + "documentWithOutlines.pdf";
            PdfReader reader = new PdfReader(new FileStream(filename, FileMode.Open, FileAccess.Read
                ));
            PdfDocument pdfDoc = new PdfDocument(reader);
            PdfOutline outlines = pdfDoc.GetOutlines(false);
            try
            {
                NUnit.Framework.Assert.AreEqual(2, outlines.GetAllChildren().Count);
                NUnit.Framework.Assert.AreEqual("First Page", outlines.GetAllChildren()[0].GetTitle
                    ());
            }
            finally
            {
                pdfDoc.Close();
            }
        }

        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        [LogMessage(LogMessageConstant.SOURCE_DOCUMENT_HAS_ACROFORM_DICTIONARY)]
        public virtual void CopyPagesWithOutlines()
        {
            PdfReader reader = new PdfReader(new FileStream(sourceFolder + "iphone_user_guide.pdf"
                , FileMode.Open, FileAccess.Read));
            PdfWriter writer = new PdfWriter(new FileStream(destinationFolder + "copyPagesWithOutlines01.pdf"
                , FileMode.Create));
            PdfDocument pdfDoc = new PdfDocument(reader);
            PdfDocument pdfDoc1 = new PdfDocument(writer);
            IList<int> pages = new List<int>();
            pages.Add(1);
            pages.Add(2);
            pages.Add(3);
            pages.Add(5);
            pages.Add(52);
            pages.Add(102);
            pdfDoc1.InitializeOutlines();
            pdfDoc.CopyPagesTo(pages, pdfDoc1);
            pdfDoc.Close();
            NUnit.Framework.Assert.AreEqual(6, pdfDoc1.GetNumberOfPages());
            NUnit.Framework.Assert.AreEqual(4, pdfDoc1.GetOutlines(false).GetAllChildren()[0]
                .GetAllChildren().Count);
            pdfDoc1.Close();
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void AddOutlinesWithNamedDestinations01()
        {
            PdfReader reader = new PdfReader(new FileStream(sourceFolder + "iphone_user_guide.pdf"
                , FileMode.Open, FileAccess.Read));
            String filename = destinationFolder + "outlinesWithNamedDestinations01.pdf";
            FileStream fos = new FileStream(filename, FileMode.Create);
            PdfWriter writer = new PdfWriter(fos);
            PdfDocument pdfDoc = new PdfDocument(reader, writer);
            PdfArray array1 = new PdfArray();
            array1.Add(pdfDoc.GetPage(2).GetPdfObject());
            array1.Add(PdfName.XYZ);
            array1.Add(new PdfNumber(36));
            array1.Add(new PdfNumber(806));
            array1.Add(new PdfNumber(0));
            PdfArray array2 = new PdfArray();
            array2.Add(pdfDoc.GetPage(3).GetPdfObject());
            array2.Add(PdfName.XYZ);
            array2.Add(new PdfNumber(36));
            array2.Add(new PdfNumber(806));
            array2.Add(new PdfNumber(1.25));
            PdfArray array3 = new PdfArray();
            array3.Add(pdfDoc.GetPage(4).GetPdfObject());
            array3.Add(PdfName.XYZ);
            array3.Add(new PdfNumber(36));
            array3.Add(new PdfNumber(806));
            array3.Add(new PdfNumber(1));
            pdfDoc.AddNamedDestination("test1", array2);
            pdfDoc.AddNamedDestination("test2", array3);
            pdfDoc.AddNamedDestination("test3", array1);
            PdfOutline root = pdfDoc.GetOutlines(false);
            PdfOutline firstOutline = root.AddOutline("Test1");
            firstOutline.AddDestination(PdfDestination.MakeDestination(new PdfString("test1")
                ));
            PdfOutline secondOutline = root.AddOutline("Test2");
            secondOutline.AddDestination(PdfDestination.MakeDestination(new PdfString("test2"
                )));
            PdfOutline thirdOutline = root.AddOutline("Test3");
            thirdOutline.AddDestination(PdfDestination.MakeDestination(new PdfString("test3")
                ));
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(filename, sourceFolder
                 + "cmp_outlinesWithNamedDestinations01.pdf", destinationFolder, "diff_"));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void AddOutlinesWithNamedDestinations02()
        {
            String filename = destinationFolder + "outlinesWithNamedDestinations02.pdf";
            FileStream fos = new FileStream(filename, FileMode.Create);
            PdfWriter writer = new PdfWriter(fos);
            PdfDocument pdfDoc = new PdfDocument(writer);
            PdfArray array1 = new PdfArray();
            array1.Add(pdfDoc.AddNewPage().GetPdfObject());
            array1.Add(PdfName.XYZ);
            array1.Add(new PdfNumber(36));
            array1.Add(new PdfNumber(806));
            array1.Add(new PdfNumber(0));
            PdfArray array2 = new PdfArray();
            array2.Add(pdfDoc.AddNewPage().GetPdfObject());
            array2.Add(PdfName.XYZ);
            array2.Add(new PdfNumber(36));
            array2.Add(new PdfNumber(806));
            array2.Add(new PdfNumber(1.25));
            PdfArray array3 = new PdfArray();
            array3.Add(pdfDoc.AddNewPage().GetPdfObject());
            array3.Add(PdfName.XYZ);
            array3.Add(new PdfNumber(36));
            array3.Add(new PdfNumber(806));
            array3.Add(new PdfNumber(1));
            pdfDoc.AddNamedDestination("page1", array2);
            pdfDoc.AddNamedDestination("page2", array3);
            pdfDoc.AddNamedDestination("page3", array1);
            PdfOutline root = pdfDoc.GetOutlines(false);
            PdfOutline firstOutline = root.AddOutline("Test1");
            firstOutline.AddDestination(PdfDestination.MakeDestination(new PdfString("page1")
                ));
            PdfOutline secondOutline = root.AddOutline("Test2");
            secondOutline.AddDestination(PdfDestination.MakeDestination(new PdfString("page2"
                )));
            PdfOutline thirdOutline = root.AddOutline("Test3");
            thirdOutline.AddDestination(PdfDestination.MakeDestination(new PdfString("page3")
                ));
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(filename, sourceFolder
                 + "cmp_outlinesWithNamedDestinations02.pdf", destinationFolder, "diff_"));
        }
    }
}
