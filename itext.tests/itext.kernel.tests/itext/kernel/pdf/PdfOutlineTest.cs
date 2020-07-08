/*
This file is part of the iText (R) project.
Copyright (c) 1998-2020 iText Group NV
Authors: iText Software.

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License version 3
as published by the Free Software Foundation with the addition of the
following permission added to Section 15 as permitted in Section 7(a):
FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
OF THIRD PARTY RIGHTS

This program is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU Affero General Public License for more details.
You should have received a copy of the GNU Affero General Public License
along with this program; if not, see http://www.gnu.org/licenses or write to
the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
Boston, MA, 02110-1301 USA, or download the license from the following URL:
http://itextpdf.com/terms-of-use/

The interactive user interfaces in modified source and object code versions
of this program must display Appropriate Legal Notices, as required under
Section 5 of the GNU Affero General Public License.

In accordance with Section 7(b) of the GNU Affero General Public License,
a covered work must retain the producer line in every PDF that is created
or manipulated using iText.

You can be released from the requirements of the license by purchasing
a commercial license. Buying such a license is mandatory as soon as you
develop commercial activities involving the iText software without
disclosing the source code of your own applications.
These activities include: offering paid services to customers as an ASP,
serving PDFs on the fly in a web application, shipping iText with a closed
source product.

For more information, please contact iText Software Corp. at this
address: sales@itextpdf.com
*/
using System;
using System.Collections.Generic;
using System.IO;
using iText.Kernel.Pdf.Navigation;
using iText.Kernel.Utils;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Kernel.Pdf {
    public class PdfOutlineTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/pdf/PdfOutlineTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/kernel/pdf/PdfOutlineTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void Before() {
            CreateOrClearDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void CreateSimpleDocWithOutlines() {
            String filename = "simpleDocWithOutlines.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(destinationFolder + filename));
            pdfDoc.GetCatalog().SetPageMode(PdfName.UseOutlines);
            PdfPage firstPage = pdfDoc.AddNewPage();
            PdfPage secondPage = pdfDoc.AddNewPage();
            PdfOutline rootOutline = pdfDoc.GetOutlines(false);
            PdfOutline firstOutline = rootOutline.AddOutline("First Page");
            PdfOutline secondOutline = rootOutline.AddOutline("Second Page");
            firstOutline.AddDestination(PdfExplicitDestination.CreateFit(firstPage));
            secondOutline.AddDestination(PdfExplicitDestination.CreateFit(secondPage));
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + filename, sourceFolder
                 + "cmp_" + filename, destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void OutlinesTest() {
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(sourceFolder + "iphone_user_guide.pdf"));
            PdfOutline outlines = pdfDoc.GetOutlines(false);
            IList<PdfOutline> children = outlines.GetAllChildren()[0].GetAllChildren();
            NUnit.Framework.Assert.AreEqual(outlines.GetTitle(), "Outlines");
            NUnit.Framework.Assert.AreEqual(children.Count, 13);
            NUnit.Framework.Assert.IsTrue(children[0].GetDestination() is PdfStringDestination);
        }

        [NUnit.Framework.Test]
        public virtual void OutlinesWithPagesTest() {
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(sourceFolder + "iphone_user_guide.pdf"));
            PdfPage page = pdfDoc.GetPage(52);
            IList<PdfOutline> pageOutlines = page.GetOutlines(true);
            try {
                NUnit.Framework.Assert.AreEqual(3, pageOutlines.Count);
                NUnit.Framework.Assert.IsTrue(pageOutlines[0].GetTitle().Equals("Safari"));
                NUnit.Framework.Assert.AreEqual(pageOutlines[0].GetAllChildren().Count, 4);
            }
            finally {
                pdfDoc.Close();
            }
        }

        [NUnit.Framework.Test]
        public virtual void AddOutlinesToDocumentTest() {
            PdfReader reader = new PdfReader(sourceFolder + "iphone_user_guide.pdf");
            String filename = "addOutlinesToDocumentTest.pdf";
            PdfWriter writer = new PdfWriter(destinationFolder + filename);
            PdfDocument pdfDoc = new PdfDocument(reader, writer);
            pdfDoc.SetTagged();
            PdfOutline outlines = pdfDoc.GetOutlines(false);
            PdfOutline firstPage = outlines.AddOutline("firstPage");
            PdfOutline firstPageChild = firstPage.AddOutline("firstPageChild");
            PdfOutline secondPage = outlines.AddOutline("secondPage");
            PdfOutline secondPageChild = secondPage.AddOutline("secondPageChild");
            firstPage.AddDestination(PdfExplicitDestination.CreateFit(pdfDoc.GetPage(1)));
            firstPageChild.AddDestination(PdfExplicitDestination.CreateFit(pdfDoc.GetPage(1)));
            secondPage.AddDestination(PdfExplicitDestination.CreateFit(pdfDoc.GetPage(2)));
            secondPageChild.AddDestination(PdfExplicitDestination.CreateFit(pdfDoc.GetPage(2)));
            outlines.GetAllChildren()[0].GetAllChildren()[1].AddOutline("testOutline", 1).AddDestination(PdfExplicitDestination
                .CreateFit(pdfDoc.GetPage(102)));
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + filename, sourceFolder
                 + "cmp_" + filename, destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void ReadOutlinesFromDocumentTest() {
            String filename = sourceFolder + "addOutlinesResult.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(filename));
            PdfOutline outlines = pdfDoc.GetOutlines(false);
            try {
                NUnit.Framework.Assert.AreEqual(3, outlines.GetAllChildren().Count);
                NUnit.Framework.Assert.AreEqual("firstPageChild", outlines.GetAllChildren()[1].GetAllChildren()[0].GetTitle
                    ());
            }
            finally {
                pdfDoc.Close();
            }
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.FLUSHED_OBJECT_CONTAINS_FREE_REFERENCE, Count = 36)]
        public virtual void RemovePageWithOutlinesTest() {
            // TODO DEVSIX-1643: destinations are not removed along with page
            String filename = "removePageWithOutlinesTest.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(sourceFolder + "iphone_user_guide.pdf"), new PdfWriter(
                destinationFolder + filename));
            // TODO DEVSIX-1643 (this causes log message errors. It's because of destinations pointing to removed page (freed reference, replaced by PdfNull))
            pdfDoc.RemovePage(102);
            pdfDoc.Close();
            CompareTool compareTool = new CompareTool();
            String diffContent = compareTool.CompareByContent(destinationFolder + filename, sourceFolder + "cmp_" + filename
                , destinationFolder, "diff_");
            String diffTags = compareTool.CompareTagStructures(destinationFolder + filename, sourceFolder + "cmp_" + filename
                );
            if (diffContent != null || diffTags != null) {
                diffContent = diffContent != null ? diffContent : "";
                diffTags = diffTags != null ? diffTags : "";
                NUnit.Framework.Assert.Fail(diffContent + diffTags);
            }
        }

        [NUnit.Framework.Test]
        public virtual void ReadRemovedPageWithOutlinesTest() {
            // TODO DEVSIX-1643: src document is taken from the previous removePageWithOutlinesTest test, however it contains numerous destination objects which contain PdfNull instead of page reference
            String filename = sourceFolder + "removePagesWithOutlinesResult.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(filename));
            PdfPage page = pdfDoc.GetPage(102);
            IList<PdfOutline> pageOutlines = page.GetOutlines(false);
            try {
                NUnit.Framework.Assert.AreEqual(4, pageOutlines.Count);
            }
            finally {
                pdfDoc.Close();
            }
        }

        [NUnit.Framework.Test]
        public virtual void UpdateOutlineTitle() {
            PdfReader reader = new PdfReader(sourceFolder + "iphone_user_guide.pdf");
            String filename = "updateOutlineTitle.pdf";
            PdfWriter writer = new PdfWriter(destinationFolder + filename);
            PdfDocument pdfDoc = new PdfDocument(reader, writer);
            PdfOutline outlines = pdfDoc.GetOutlines(false);
            outlines.GetAllChildren()[0].GetAllChildren()[1].SetTitle("New Title");
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + filename, sourceFolder
                 + "cmp_" + filename, destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void ReadOutlineTitle() {
            String filename = sourceFolder + "updateOutlineTitleResult.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(filename));
            PdfOutline outlines = pdfDoc.GetOutlines(false);
            PdfOutline outline = outlines.GetAllChildren()[0].GetAllChildren()[1];
            try {
                NUnit.Framework.Assert.AreEqual("New Title", outline.GetTitle());
            }
            finally {
                pdfDoc.Close();
            }
        }

        [NUnit.Framework.Test]
        public virtual void AddOutlineInNotOutlineMode() {
            String filename = "addOutlineInNotOutlineMode.pdf";
            PdfReader reader = new PdfReader(sourceFolder + "iphone_user_guide.pdf");
            PdfWriter writer = new PdfWriter(destinationFolder + filename);
            PdfDocument pdfDoc = new PdfDocument(reader, writer);
            PdfOutline outlines = new PdfOutline(pdfDoc);
            PdfOutline firstPage = outlines.AddOutline("firstPage");
            PdfOutline firstPageChild = firstPage.AddOutline("firstPageChild");
            PdfOutline secondPage = outlines.AddOutline("secondPage");
            PdfOutline secondPageChild = secondPage.AddOutline("secondPageChild");
            firstPage.AddDestination(PdfExplicitDestination.CreateFit(pdfDoc.GetPage(1)));
            firstPageChild.AddDestination(PdfExplicitDestination.CreateFit(pdfDoc.GetPage(1)));
            secondPage.AddDestination(PdfExplicitDestination.CreateFit(pdfDoc.GetPage(2)));
            secondPageChild.AddDestination(PdfExplicitDestination.CreateFit(pdfDoc.GetPage(2)));
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + filename, sourceFolder
                 + "cmp_" + filename, destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void ReadOutlineAddedInNotOutlineMode() {
            String filename = sourceFolder + "addOutlinesWithoutOutlineModeResult.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(filename));
            IList<PdfOutline> pageOutlines = pdfDoc.GetPage(102).GetOutlines(true);
            try {
                NUnit.Framework.Assert.AreEqual(5, pageOutlines.Count);
            }
            finally {
                pdfDoc.Close();
            }
        }

        [NUnit.Framework.Test]
        public virtual void CreateDocWithOutlines() {
            String filename = sourceFolder + "documentWithOutlines.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(filename));
            PdfOutline outlines = pdfDoc.GetOutlines(false);
            try {
                NUnit.Framework.Assert.AreEqual(2, outlines.GetAllChildren().Count);
                NUnit.Framework.Assert.AreEqual("First Page", outlines.GetAllChildren()[0].GetTitle());
            }
            finally {
                pdfDoc.Close();
            }
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.SOURCE_DOCUMENT_HAS_ACROFORM_DICTIONARY)]
        public virtual void CopyPagesWithOutlines() {
            PdfReader reader = new PdfReader(sourceFolder + "iphone_user_guide.pdf");
            PdfWriter writer = new PdfWriter(destinationFolder + "copyPagesWithOutlines01.pdf");
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
            NUnit.Framework.Assert.AreEqual(4, pdfDoc1.GetOutlines(false).GetAllChildren()[0].GetAllChildren().Count);
            pdfDoc1.Close();
        }

        [NUnit.Framework.Test]
        public virtual void AddOutlinesWithNamedDestinations01() {
            String filename = destinationFolder + "outlinesWithNamedDestinations01.pdf";
            PdfReader reader = new PdfReader(sourceFolder + "iphone_user_guide.pdf");
            PdfWriter writer = new PdfWriter(filename);
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
            firstOutline.AddDestination(PdfDestination.MakeDestination(new PdfString("test1")));
            PdfOutline secondOutline = root.AddOutline("Test2");
            secondOutline.AddDestination(PdfDestination.MakeDestination(new PdfString("test2")));
            PdfOutline thirdOutline = root.AddOutline("Test3");
            thirdOutline.AddDestination(PdfDestination.MakeDestination(new PdfString("test3")));
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(filename, sourceFolder + "cmp_outlinesWithNamedDestinations01.pdf"
                , destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void AddOutlinesWithNamedDestinations02() {
            String filename = destinationFolder + "outlinesWithNamedDestinations02.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(filename));
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
            firstOutline.AddDestination(PdfDestination.MakeDestination(new PdfString("page1")));
            PdfOutline secondOutline = root.AddOutline("Test2");
            secondOutline.AddDestination(PdfDestination.MakeDestination(new PdfString("page2")));
            PdfOutline thirdOutline = root.AddOutline("Test3");
            thirdOutline.AddDestination(PdfDestination.MakeDestination(new PdfString("page3")));
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(filename, sourceFolder + "cmp_outlinesWithNamedDestinations02.pdf"
                , destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void OutlineStackOverflowTest01() {
            PdfReader reader = new PdfReader(sourceFolder + "outlineStackOverflowTest01.pdf");
            PdfDocument pdfDoc = new PdfDocument(reader);
            try {
                pdfDoc.GetOutlines(true);
            }
            catch (OutOfMemoryException) {
                NUnit.Framework.Assert.Fail("StackOverflow thrown when reading document with a large number of outlines.");
            }
        }

        [NUnit.Framework.Test]
        public virtual void OutlineTypeNull() {
            String filename = "outlineTypeNull";
            String outputFile = destinationFolder + filename + ".pdf";
            PdfReader reader = new PdfReader(sourceFolder + filename + ".pdf");
            PdfWriter writer = new PdfWriter(new FileStream(outputFile, FileMode.Create));
            PdfDocument pdfDoc = new PdfDocument(reader, writer);
            pdfDoc.RemovePage(3);
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outputFile, sourceFolder + "cmp_" + filename
                 + ".pdf", destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void RemoveAllOutlinesTest() {
            String filename = "iphone_user_guide_removeAllOutlinesTest.pdf";
            String input = sourceFolder + "iphone_user_guide.pdf";
            String output = destinationFolder + "cmp_" + filename;
            String cmp = sourceFolder + "cmp_" + filename;
            PdfReader reader = new PdfReader(input);
            PdfWriter writer = new PdfWriter(output);
            PdfDocument pdfDocument = new PdfDocument(reader, writer);
            pdfDocument.GetOutlines(true).RemoveOutline();
            pdfDocument.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(output, cmp, destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void RemoveOneOutlineTest() {
            String filename = "removeOneOutline.pdf";
            String input = sourceFolder + "outlineTree.pdf";
            String output = destinationFolder + "cmp_" + filename;
            String cmp = sourceFolder + "cmp_" + filename;
            PdfReader reader = new PdfReader(input);
            PdfWriter writer = new PdfWriter(output);
            PdfDocument pdfDocument = new PdfDocument(reader, writer);
            PdfOutline root = pdfDocument.GetOutlines(true);
            PdfOutline toRemove = root.GetAllChildren()[2];
            toRemove.RemoveOutline();
            pdfDocument.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(output, cmp, destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void TestReinitializingOutlines() {
            String input = sourceFolder + "outlineTree.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(input));
            PdfOutline root = pdfDocument.GetOutlines(false);
            NUnit.Framework.Assert.AreEqual(4, root.GetAllChildren().Count);
            pdfDocument.GetCatalog().GetPdfObject().Remove(PdfName.Outlines);
            root = pdfDocument.GetOutlines(true);
            NUnit.Framework.Assert.IsNull(root);
            pdfDocument.Close();
        }
    }
}
