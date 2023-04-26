/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 Apryse Group NV
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
using iText.IO.Font;
using iText.Kernel.Colors;
using iText.Kernel.Exceptions;
using iText.Kernel.Logs;
using iText.Kernel.Pdf.Navigation;
using iText.Kernel.Utils;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Kernel.Pdf {
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfOutlineTest : ExtendedITextTest {
        public static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/pdf/PdfOutlineTest/";

        public static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/kernel/pdf/PdfOutlineTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void Before() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void CreateSimpleDocWithOutlines() {
            String filename = "simpleDocWithOutlines.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(DESTINATION_FOLDER + filename));
            pdfDoc.GetCatalog().SetPageMode(PdfName.UseOutlines);
            PdfPage firstPage = pdfDoc.AddNewPage();
            PdfPage secondPage = pdfDoc.AddNewPage();
            PdfOutline rootOutline = pdfDoc.GetOutlines(false);
            PdfOutline firstOutline = rootOutline.AddOutline("First Page");
            PdfOutline secondOutline = rootOutline.AddOutline("Second Page");
            firstOutline.AddDestination(PdfExplicitDestination.CreateFit(firstPage));
            secondOutline.AddDestination(PdfExplicitDestination.CreateFit(secondPage));
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(DESTINATION_FOLDER + filename, SOURCE_FOLDER
                 + "cmp_" + filename, DESTINATION_FOLDER, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void OutlinesTest() {
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(SOURCE_FOLDER + "iphone_user_guide.pdf"));
            PdfOutline outlines = pdfDoc.GetOutlines(false);
            IList<PdfOutline> children = outlines.GetAllChildren()[0].GetAllChildren();
            NUnit.Framework.Assert.AreEqual(outlines.GetTitle(), "Outlines");
            NUnit.Framework.Assert.AreEqual(children.Count, 13);
            NUnit.Framework.Assert.IsTrue(children[0].GetDestination() is PdfStringDestination);
        }

        [NUnit.Framework.Test]
        public virtual void OutlinesWithPagesTest() {
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(SOURCE_FOLDER + "iphone_user_guide.pdf"));
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
            PdfReader reader = new PdfReader(SOURCE_FOLDER + "iphone_user_guide.pdf");
            String filename = "addOutlinesToDocumentTest.pdf";
            PdfWriter writer = new PdfWriter(DESTINATION_FOLDER + filename);
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
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(DESTINATION_FOLDER + filename, SOURCE_FOLDER
                 + "cmp_" + filename, DESTINATION_FOLDER, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void ReadOutlinesFromDocumentTest() {
            String filename = SOURCE_FOLDER + "addOutlinesResult.pdf";
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
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.FLUSHED_OBJECT_CONTAINS_FREE_REFERENCE, Count = 36)]
        public virtual void RemovePageWithOutlinesTest() {
            // TODO DEVSIX-1643: destinations are not removed along with page
            String filename = "removePageWithOutlinesTest.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(SOURCE_FOLDER + "iphone_user_guide.pdf"), new PdfWriter
                (DESTINATION_FOLDER + filename));
            // TODO DEVSIX-1643 (this causes log message errors. It's because of destinations pointing to removed page (freed reference, replaced by PdfNull))
            pdfDoc.RemovePage(102);
            pdfDoc.Close();
            CompareTool compareTool = new CompareTool();
            String diffContent = compareTool.CompareByContent(DESTINATION_FOLDER + filename, SOURCE_FOLDER + "cmp_" + 
                filename, DESTINATION_FOLDER, "diff_");
            String diffTags = compareTool.CompareTagStructures(DESTINATION_FOLDER + filename, SOURCE_FOLDER + "cmp_" +
                 filename);
            if (diffContent != null || diffTags != null) {
                diffContent = diffContent != null ? diffContent : "";
                diffTags = diffTags != null ? diffTags : "";
                NUnit.Framework.Assert.Fail(diffContent + diffTags);
            }
        }

        [NUnit.Framework.Test]
        public virtual void ReadRemovedPageWithOutlinesTest() {
            // TODO DEVSIX-1643: src document is taken from the previous removePageWithOutlinesTest test, however it contains numerous destination objects which contain PdfNull instead of page reference
            String filename = SOURCE_FOLDER + "removePagesWithOutlinesResult.pdf";
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
            PdfReader reader = new PdfReader(SOURCE_FOLDER + "iphone_user_guide.pdf");
            String filename = "updateOutlineTitle.pdf";
            PdfWriter writer = new PdfWriter(DESTINATION_FOLDER + filename);
            PdfDocument pdfDoc = new PdfDocument(reader, writer);
            PdfOutline outlines = pdfDoc.GetOutlines(false);
            outlines.GetAllChildren()[0].GetAllChildren()[1].SetTitle("New Title");
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(DESTINATION_FOLDER + filename, SOURCE_FOLDER
                 + "cmp_" + filename, DESTINATION_FOLDER, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void GetOutlinesInvalidParentLink() {
            PdfReader reader = new PdfReader(SOURCE_FOLDER + "outlinesInvalidParentLink.pdf");
            String filename = "updateOutlineTitleInvalidParentLink.pdf";
            PdfWriter writer = new PdfWriter(DESTINATION_FOLDER + filename);
            PdfDocument pdfDoc = new PdfDocument(reader, writer);
            PdfOutline outlines = pdfDoc.GetOutlines(true);
            PdfOutline firstOutline = outlines.GetAllChildren()[0];
            PdfOutline secondOutline = outlines.GetAllChildren()[1];
            try {
                NUnit.Framework.Assert.AreEqual(2, outlines.GetAllChildren().Count);
                NUnit.Framework.Assert.AreEqual("First Page", firstOutline.GetTitle());
                NUnit.Framework.Assert.AreEqual(outlines, firstOutline.GetParent());
                NUnit.Framework.Assert.AreEqual("Second Page", secondOutline.GetTitle());
                NUnit.Framework.Assert.AreEqual(outlines, secondOutline.GetParent());
            }
            finally {
                pdfDoc.Close();
            }
        }

        [NUnit.Framework.Test]
        public virtual void ReadOutlineTitle() {
            String filename = SOURCE_FOLDER + "updateOutlineTitleResult.pdf";
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
            PdfReader reader = new PdfReader(SOURCE_FOLDER + "iphone_user_guide.pdf");
            PdfWriter writer = new PdfWriter(DESTINATION_FOLDER + filename);
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
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(DESTINATION_FOLDER + filename, SOURCE_FOLDER
                 + "cmp_" + filename, DESTINATION_FOLDER, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void ReadOutlineAddedInNotOutlineMode() {
            String filename = SOURCE_FOLDER + "addOutlinesWithoutOutlineModeResult.pdf";
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
            String filename = SOURCE_FOLDER + "documentWithOutlines.pdf";
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
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.SOURCE_DOCUMENT_HAS_ACROFORM_DICTIONARY)]
        public virtual void CopyPagesWithOutlines() {
            PdfReader reader = new PdfReader(SOURCE_FOLDER + "iphone_user_guide.pdf");
            PdfWriter writer = new PdfWriter(DESTINATION_FOLDER + "copyPagesWithOutlines01.pdf");
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
            String filename = DESTINATION_FOLDER + "outlinesWithNamedDestinations01.pdf";
            PdfReader reader = new PdfReader(SOURCE_FOLDER + "iphone_user_guide.pdf");
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
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(filename, SOURCE_FOLDER + "cmp_outlinesWithNamedDestinations01.pdf"
                , DESTINATION_FOLDER, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void AddOutlinesWithNamedDestinations02() {
            String filename = DESTINATION_FOLDER + "outlinesWithNamedDestinations02.pdf";
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
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(filename, SOURCE_FOLDER + "cmp_outlinesWithNamedDestinations02.pdf"
                , DESTINATION_FOLDER, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void OutlineStackOverflowTest01() {
            PdfReader reader = new PdfReader(SOURCE_FOLDER + "outlineStackOverflowTest01.pdf");
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
            String outputFile = DESTINATION_FOLDER + filename + ".pdf";
            PdfReader reader = new PdfReader(SOURCE_FOLDER + filename + ".pdf");
            PdfWriter writer = new PdfWriter(new FileStream(outputFile, FileMode.Create));
            PdfDocument pdfDoc = new PdfDocument(reader, writer);
            pdfDoc.RemovePage(3);
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outputFile, SOURCE_FOLDER + "cmp_" + filename
                 + ".pdf", DESTINATION_FOLDER, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void RemoveAllOutlinesTest() {
            String filename = "iphone_user_guide_removeAllOutlinesTest.pdf";
            String input = SOURCE_FOLDER + "iphone_user_guide.pdf";
            String output = DESTINATION_FOLDER + "cmp_" + filename;
            String cmp = SOURCE_FOLDER + "cmp_" + filename;
            PdfReader reader = new PdfReader(input);
            PdfWriter writer = new PdfWriter(output);
            PdfDocument pdfDocument = new PdfDocument(reader, writer);
            pdfDocument.GetOutlines(true).RemoveOutline();
            pdfDocument.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(output, cmp, DESTINATION_FOLDER, "diff_")
                );
        }

        [NUnit.Framework.Test]
        public virtual void RemoveOneOutlineTest() {
            String filename = "removeOneOutline.pdf";
            String input = SOURCE_FOLDER + "outlineTree.pdf";
            String output = DESTINATION_FOLDER + "cmp_" + filename;
            String cmp = SOURCE_FOLDER + "cmp_" + filename;
            PdfReader reader = new PdfReader(input);
            PdfWriter writer = new PdfWriter(output);
            PdfDocument pdfDocument = new PdfDocument(reader, writer);
            PdfOutline root = pdfDocument.GetOutlines(true);
            PdfOutline toRemove = root.GetAllChildren()[2];
            toRemove.RemoveOutline();
            pdfDocument.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(output, cmp, DESTINATION_FOLDER, "diff_")
                );
        }

        [NUnit.Framework.Test]
        public virtual void TestReinitializingOutlines() {
            String input = SOURCE_FOLDER + "outlineTree.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(input));
            PdfOutline root = pdfDocument.GetOutlines(false);
            NUnit.Framework.Assert.AreEqual(4, root.GetAllChildren().Count);
            pdfDocument.GetCatalog().GetPdfObject().Remove(PdfName.Outlines);
            root = pdfDocument.GetOutlines(true);
            NUnit.Framework.Assert.IsNull(root);
            pdfDocument.Close();
        }

        [NUnit.Framework.Test]
        public virtual void RemovePageInDocWithSimpleOutlineTreeStructTest() {
            String input = SOURCE_FOLDER + "simpleOutlineTreeStructure.pdf";
            String output = DESTINATION_FOLDER + "simpleOutlineTreeStructure.pdf";
            String cmp = SOURCE_FOLDER + "cmp_simpleOutlineTreeStructure.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(input), new PdfWriter(output));
            pdfDocument.RemovePage(2);
            NUnit.Framework.Assert.AreEqual(2, pdfDocument.GetNumberOfPages());
            pdfDocument.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(output, cmp, DESTINATION_FOLDER, "diff_")
                );
        }

        [NUnit.Framework.Test]
        public virtual void RemovePageInDocWithComplexOutlineTreeStructTest() {
            String input = SOURCE_FOLDER + "complexOutlineTreeStructure.pdf";
            String output = DESTINATION_FOLDER + "complexOutlineTreeStructure.pdf";
            String cmp = SOURCE_FOLDER + "cmp_complexOutlineTreeStructure.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(input), new PdfWriter(output));
            pdfDocument.RemovePage(2);
            NUnit.Framework.Assert.AreEqual(2, pdfDocument.GetNumberOfPages());
            pdfDocument.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(output, cmp, DESTINATION_FOLDER, "diff_")
                );
        }

        [NUnit.Framework.Test]
        public virtual void ConstructOutlinesNoParentTest() {
            using (MemoryStream baos = new MemoryStream()) {
                using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(baos))) {
                    pdfDocument.AddNewPage();
                    PdfDictionary first = new PdfDictionary();
                    first.MakeIndirect(pdfDocument);
                    PdfDictionary outlineDictionary = new PdfDictionary();
                    outlineDictionary.Put(PdfName.First, first);
                    outlineDictionary.Put(PdfName.Title, new PdfString("title", PdfEncodings.UNICODE_BIG));
                    first.Put(PdfName.Title, new PdfString("title", PdfEncodings.UNICODE_BIG));
                    NUnit.Framework.Assert.DoesNotThrow(() => pdfDocument.GetCatalog().ConstructOutlines(outlineDictionary, new 
                        PdfOutlineTest.EmptyNameTree()));
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void ConstructOutlinesNoTitleTest() {
            using (MemoryStream baos = new MemoryStream()) {
                using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(baos))) {
                    pdfDocument.AddNewPage();
                    PdfDictionary first = new PdfDictionary();
                    first.MakeIndirect(pdfDocument);
                    PdfDictionary outlineDictionary = new PdfDictionary();
                    outlineDictionary.MakeIndirect(pdfDocument);
                    outlineDictionary.Put(PdfName.First, first);
                    first.Put(PdfName.Parent, outlineDictionary);
                    Exception exception = NUnit.Framework.Assert.Catch(typeof(PdfException), () => pdfDocument.GetCatalog().ConstructOutlines
                        (outlineDictionary, new PdfOutlineTest.EmptyNameTree()));
                    NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(KernelExceptionMessageConstant.CORRUPTED_OUTLINE_NO_TITLE_ENTRY
                        , first.indirectReference), exception.Message);
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void CheckParentOfOutlinesTest() {
            using (MemoryStream baos = new MemoryStream()) {
                using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(baos))) {
                    pdfDocument.GetCatalog().SetPageMode(PdfName.UseOutlines);
                    PdfPage firstPage = pdfDocument.AddNewPage();
                    PdfOutline rootOutline = pdfDocument.GetOutlines(false);
                    PdfOutline firstOutline = rootOutline.AddOutline("First outline");
                    PdfOutline firstSubOutline = firstOutline.AddOutline("First suboutline");
                    PdfOutline secondSubOutline = firstOutline.AddOutline("Second suboutline");
                    PdfOutline secondOutline = rootOutline.AddOutline("SecondOutline");
                    firstOutline.AddDestination(PdfExplicitDestination.CreateFit(firstPage));
                    PdfOutline resultedRoot = pdfDocument.GetOutlines(true);
                    NUnit.Framework.Assert.AreEqual(2, resultedRoot.GetAllChildren().Count);
                    NUnit.Framework.Assert.AreEqual(resultedRoot, resultedRoot.GetAllChildren()[0].GetParent());
                    NUnit.Framework.Assert.AreEqual(resultedRoot, resultedRoot.GetAllChildren()[1].GetParent());
                    PdfOutline resultedFirstOutline = resultedRoot.GetAllChildren()[0];
                    NUnit.Framework.Assert.AreEqual(2, resultedFirstOutline.GetAllChildren().Count);
                    NUnit.Framework.Assert.AreEqual(resultedFirstOutline, resultedFirstOutline.GetAllChildren()[0].GetParent()
                        );
                    NUnit.Framework.Assert.AreEqual(resultedFirstOutline, resultedFirstOutline.GetAllChildren()[1].GetParent()
                        );
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void CheckNestedOutlinesParentTest() {
            using (MemoryStream baos = new MemoryStream()) {
                using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(baos))) {
                    pdfDocument.GetCatalog().SetPageMode(PdfName.UseOutlines);
                    PdfPage firstPage = pdfDocument.AddNewPage();
                    PdfOutline rootOutline = pdfDocument.GetOutlines(false);
                    PdfOutline firstOutline = rootOutline.AddOutline("First outline");
                    PdfOutline secondOutline = firstOutline.AddOutline("Second outline");
                    PdfOutline thirdOutline = secondOutline.AddOutline("Third outline");
                    firstOutline.AddDestination(PdfExplicitDestination.CreateFit(firstPage));
                    PdfOutline resultedRoot = pdfDocument.GetOutlines(true);
                    NUnit.Framework.Assert.AreEqual(1, resultedRoot.GetAllChildren().Count);
                    NUnit.Framework.Assert.AreEqual(resultedRoot, resultedRoot.GetAllChildren()[0].GetParent());
                    PdfOutline resultedFirstOutline = resultedRoot.GetAllChildren()[0];
                    NUnit.Framework.Assert.AreEqual(1, resultedFirstOutline.GetAllChildren().Count);
                    NUnit.Framework.Assert.AreEqual(resultedFirstOutline, resultedFirstOutline.GetAllChildren()[0].GetParent()
                        );
                    PdfOutline resultedSecondOutline = resultedFirstOutline.GetAllChildren()[0];
                    NUnit.Framework.Assert.AreEqual(1, resultedSecondOutline.GetAllChildren().Count);
                    NUnit.Framework.Assert.AreEqual(resultedSecondOutline, resultedSecondOutline.GetAllChildren()[0].GetParent
                        ());
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void SetOutlinePropertiesTest() {
            using (MemoryStream baos = new MemoryStream()) {
                using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(baos))) {
                    PdfPage firstPage = pdfDocument.AddNewPage();
                    PdfOutline rootOutline = pdfDocument.GetOutlines(true);
                    PdfOutline outline = rootOutline.AddOutline("Outline");
                    NUnit.Framework.Assert.IsTrue(outline.IsOpen());
                    NUnit.Framework.Assert.IsNull(outline.GetStyle());
                    NUnit.Framework.Assert.IsNull(outline.GetColor());
                    outline.GetContent().Put(PdfName.C, new PdfArray(ColorConstants.BLACK.GetColorValue()));
                    outline.GetContent().Put(PdfName.F, new PdfNumber(2));
                    outline.GetContent().Put(PdfName.Count, new PdfNumber(4));
                    NUnit.Framework.Assert.IsTrue(outline.IsOpen());
                    NUnit.Framework.Assert.AreEqual(2, outline.GetStyle());
                    NUnit.Framework.Assert.AreEqual(ColorConstants.BLACK, outline.GetColor());
                    outline.GetContent().Put(PdfName.Count, new PdfNumber(0));
                    NUnit.Framework.Assert.IsTrue(outline.IsOpen());
                    outline.GetContent().Put(PdfName.Count, new PdfNumber(-5));
                    NUnit.Framework.Assert.IsFalse(outline.IsOpen());
                }
            }
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.CORRUPTED_OUTLINE_DICTIONARY_HAS_INFINITE_LOOP)]
        public virtual void CheckPossibleInfiniteLoopWithSameNextAndPrevLinkTest() {
            using (MemoryStream baos = new MemoryStream()) {
                using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(baos))) {
                    pdfDocument.AddNewPage();
                    PdfDictionary first = new PdfDictionary();
                    first.MakeIndirect(pdfDocument);
                    PdfDictionary second = new PdfDictionary();
                    second.MakeIndirect(pdfDocument);
                    PdfDictionary outlineDictionary = new PdfDictionary();
                    outlineDictionary.MakeIndirect(pdfDocument);
                    outlineDictionary.Put(PdfName.First, first);
                    outlineDictionary.Put(PdfName.Last, second);
                    first.Put(PdfName.Parent, outlineDictionary);
                    second.Put(PdfName.Parent, outlineDictionary);
                    first.Put(PdfName.Next, second);
                    first.Put(PdfName.Prev, second);
                    second.Put(PdfName.Next, first);
                    second.Put(PdfName.Prev, first);
                    outlineDictionary.Put(PdfName.Title, new PdfString("title", PdfEncodings.UNICODE_BIG));
                    first.Put(PdfName.Title, new PdfString("title", PdfEncodings.UNICODE_BIG));
                    second.Put(PdfName.Title, new PdfString("title", PdfEncodings.UNICODE_BIG));
                    NUnit.Framework.Assert.DoesNotThrow(() => pdfDocument.GetCatalog().ConstructOutlines(outlineDictionary, new 
                        PdfOutlineTest.EmptyNameTree()));
                    PdfOutline resultedOutline = pdfDocument.GetOutlines(false);
                    NUnit.Framework.Assert.AreEqual(2, resultedOutline.GetAllChildren().Count);
                    NUnit.Framework.Assert.AreEqual(resultedOutline.GetAllChildren()[1].GetParent(), resultedOutline.GetAllChildren
                        ()[0].GetParent());
                }
            }
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.CORRUPTED_OUTLINE_DICTIONARY_HAS_INFINITE_LOOP)]
        public virtual void CheckPossibleInfiniteLoopWithSameFirstAndLastLinkTest() {
            using (MemoryStream baos = new MemoryStream()) {
                using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(baos))) {
                    pdfDocument.AddNewPage();
                    PdfDictionary first = new PdfDictionary();
                    first.MakeIndirect(pdfDocument);
                    PdfDictionary outlineDictionary = new PdfDictionary();
                    outlineDictionary.MakeIndirect(pdfDocument);
                    outlineDictionary.Put(PdfName.First, first);
                    first.Put(PdfName.Parent, outlineDictionary);
                    first.Put(PdfName.First, outlineDictionary);
                    first.Put(PdfName.Last, outlineDictionary);
                    outlineDictionary.Put(PdfName.Title, new PdfString("title", PdfEncodings.UNICODE_BIG));
                    first.Put(PdfName.Title, new PdfString("title", PdfEncodings.UNICODE_BIG));
                    NUnit.Framework.Assert.DoesNotThrow(() => pdfDocument.GetCatalog().ConstructOutlines(outlineDictionary, new 
                        PdfOutlineTest.EmptyNameTree()));
                    PdfOutline resultedOutline = pdfDocument.GetOutlines(false);
                    NUnit.Framework.Assert.AreEqual(1, resultedOutline.GetAllChildren().Count);
                    NUnit.Framework.Assert.AreEqual(resultedOutline, resultedOutline.GetAllChildren()[0].GetParent());
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void OutlineNoParentLinkInConservativeModeTest() {
            using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(SOURCE_FOLDER + "outlinesNoParentLink.pdf")
                )) {
                pdfDocument.GetReader().SetStrictnessLevel(PdfReader.StrictnessLevel.CONSERVATIVE);
                Exception exception = NUnit.Framework.Assert.Catch(typeof(PdfException), () => pdfDocument.GetOutlines(true
                    ));
                //Hardcode indirectReference, cause there is no option to get this outline due to #getOutlines method
                // will be thrown an exception.
                NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(KernelExceptionMessageConstant.CORRUPTED_OUTLINE_NO_PARENT_ENTRY
                    , "9 0 R"), exception.Message);
            }
        }

        [NUnit.Framework.Test]
        public virtual void OutlineHasInfiniteLoopInConservativeModeTest() {
            using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(SOURCE_FOLDER + "outlinesHaveInfiniteLoop.pdf"
                ))) {
                pdfDocument.GetReader().SetStrictnessLevel(PdfReader.StrictnessLevel.CONSERVATIVE);
                Exception exception = NUnit.Framework.Assert.Catch(typeof(PdfException), () => pdfDocument.GetOutlines(true
                    ));
                //Hardcode indirectReference, cause there is no option to get this outline due to #getOutlines method
                // will be thrown an exception.
                NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(KernelExceptionMessageConstant.CORRUPTED_OUTLINE_DICTIONARY_HAS_INFINITE_LOOP
                    , "<</Dest [4 0 R /Fit ] /Next 10 0 R /Parent <<>> /Prev 10 0 R /Title First Page >>"), exception.Message
                    );
            }
        }

        [NUnit.Framework.Test]
        public virtual void CreateOutlinesWithDifferentVariantsOfChildrenTest() {
            using (MemoryStream baos = new MemoryStream()) {
                using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(baos))) {
                    pdfDocument.GetCatalog().SetPageMode(PdfName.UseOutlines);
                    PdfPage firstPage = pdfDocument.AddNewPage();
                    PdfOutline a = pdfDocument.GetOutlines(false);
                    PdfOutline b = a.AddOutline("B");
                    PdfOutline e = b.AddOutline("E");
                    PdfOutline f = e.AddOutline("F");
                    PdfOutline d = b.AddOutline("D");
                    PdfOutline c = a.AddOutline("C");
                    PdfOutline g = f.AddOutline("G");
                    PdfOutline h = f.AddOutline("H");
                    a.AddDestination(PdfExplicitDestination.CreateFit(firstPage));
                    PdfOutline resultedA = pdfDocument.GetOutlines(true);
                    // Asserting children of root outline.
                    NUnit.Framework.Assert.AreEqual(2, resultedA.GetAllChildren().Count);
                    NUnit.Framework.Assert.AreEqual(resultedA, resultedA.GetAllChildren()[0].GetParent());
                    NUnit.Framework.Assert.AreEqual(resultedA, resultedA.GetAllChildren()[1].GetParent());
                    NUnit.Framework.Assert.IsTrue(resultedA.GetAllChildren()[1].GetAllChildren().IsEmpty());
                    NUnit.Framework.Assert.AreEqual(2, resultedA.GetAllChildren()[0].GetAllChildren().Count);
                    //Asserting children of B outline after reconstructing.
                    PdfOutline resultedB = resultedA.GetAllChildren()[0];
                    NUnit.Framework.Assert.AreEqual(resultedB, resultedB.GetAllChildren()[0].GetParent());
                    NUnit.Framework.Assert.AreEqual(resultedB, resultedB.GetAllChildren()[1].GetParent());
                    NUnit.Framework.Assert.IsTrue(resultedB.GetAllChildren()[1].GetAllChildren().IsEmpty());
                    NUnit.Framework.Assert.AreEqual(1, resultedB.GetAllChildren()[0].GetAllChildren().Count);
                    //Asserting children of E outline after reconstructing.
                    PdfOutline resultedE = resultedB.GetAllChildren()[0];
                    NUnit.Framework.Assert.AreEqual(resultedE, resultedE.GetAllChildren()[0].GetParent());
                    NUnit.Framework.Assert.AreEqual(2, resultedE.GetAllChildren()[0].GetAllChildren().Count);
                    //Asserting children of F outline after reconstructing.
                    PdfOutline resultedF = resultedE.GetAllChildren()[0];
                    NUnit.Framework.Assert.AreEqual(resultedF, resultedF.GetAllChildren()[0].GetParent());
                    NUnit.Framework.Assert.AreEqual(resultedF, resultedF.GetAllChildren()[1].GetParent());
                    NUnit.Framework.Assert.IsTrue(resultedF.GetAllChildren()[0].GetAllChildren().IsEmpty());
                    NUnit.Framework.Assert.IsTrue(resultedF.GetAllChildren()[1].GetAllChildren().IsEmpty());
                }
            }
        }

        private sealed class EmptyNameTree : IPdfNameTreeAccess {
            public PdfObject GetEntry(PdfString key) {
                return null;
            }

            public PdfObject GetEntry(String key) {
                return null;
            }

            public ICollection<PdfString> GetKeys() {
                return JavaCollectionsUtil.EmptySet<PdfString>();
            }
        }
    }
}
