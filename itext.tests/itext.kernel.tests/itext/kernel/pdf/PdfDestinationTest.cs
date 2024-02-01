/*
This file is part of the iText (R) project.
Copyright (c) 1998-2024 Apryse Group NV
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
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Action;
using iText.Kernel.Pdf.Annot;
using iText.Kernel.Pdf.Filespec;
using iText.Kernel.Pdf.Navigation;
using iText.Kernel.Pdf.Tagging;
using iText.Kernel.Utils;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Kernel.Pdf {
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfDestinationTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/pdf/PdfDestinationTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/kernel/pdf/PdfDestinationTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.OneTimeTearDown]
        public static void AfterClass() {
            CompareTool.Cleanup(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void DestTest01() {
            String srcFile = sourceFolder + "simpleNoLinks.pdf";
            String outFile = destinationFolder + "destTest01.pdf";
            String cmpFile = sourceFolder + "cmp_destTest01.pdf";
            PdfDocument document = new PdfDocument(new PdfReader(srcFile), CompareTool.CreateTestPdfWriter(outFile));
            PdfPage firstPage = document.GetPage(1);
            PdfLinkAnnotation linkExplicitDest = new PdfLinkAnnotation(new Rectangle(35, 785, 160, 15));
            linkExplicitDest.SetAction(PdfAction.CreateGoTo(PdfExplicitDestination.CreateFit(document.GetPage(2))));
            firstPage.AddAnnotation(linkExplicitDest);
            PdfLinkAnnotation linkStringDest = new PdfLinkAnnotation(new Rectangle(35, 760, 160, 15));
            PdfExplicitDestination destToPage3 = PdfExplicitDestination.CreateFit(document.GetPage(3));
            String stringDest = "thirdPageDest";
            document.AddNamedDestination(stringDest, destToPage3.GetPdfObject());
            linkStringDest.SetAction(PdfAction.CreateGoTo(new PdfStringDestination(stringDest)));
            firstPage.AddAnnotation(linkStringDest);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFile, destinationFolder, "diff_"
                ));
        }

        [NUnit.Framework.Test]
        public virtual void DestCopyingTest01() {
            String srcFile = sourceFolder + "simpleWithLinks.pdf";
            String outFile = destinationFolder + "destCopyingTest01.pdf";
            String cmpFile = sourceFolder + "cmp_destCopyingTest01.pdf";
            PdfDocument srcDoc = new PdfDocument(new PdfReader(srcFile));
            PdfDocument destDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(outFile));
            srcDoc.CopyPagesTo(JavaUtil.ArraysAsList(1, 2, 3), destDoc);
            destDoc.Close();
            srcDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFile, destinationFolder, "diff_"
                ));
        }

        [NUnit.Framework.Test]
        public virtual void DestCopyingTest02() {
            String srcFile = sourceFolder + "simpleWithLinks.pdf";
            String outFile = destinationFolder + "destCopyingTest02.pdf";
            String cmpFile = sourceFolder + "cmp_destCopyingTest02.pdf";
            PdfDocument srcDoc = new PdfDocument(new PdfReader(srcFile));
            PdfDocument destDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(outFile));
            srcDoc.CopyPagesTo(JavaUtil.ArraysAsList(1), destDoc);
            destDoc.Close();
            srcDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFile, destinationFolder, "diff_"
                ));
        }

        [NUnit.Framework.Test]
        public virtual void DestCopyingTest03() {
            String srcFile = sourceFolder + "simpleWithLinks.pdf";
            String outFile = destinationFolder + "destCopyingTest03.pdf";
            String cmpFile = sourceFolder + "cmp_destCopyingTest03.pdf";
            PdfDocument srcDoc = new PdfDocument(new PdfReader(srcFile));
            PdfDocument destDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(outFile));
            srcDoc.CopyPagesTo(JavaUtil.ArraysAsList(1, 2), destDoc);
            destDoc.Close();
            srcDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFile, destinationFolder, "diff_"
                ));
        }

        [NUnit.Framework.Test]
        public virtual void DestCopyingTest04() {
            String srcFile = sourceFolder + "simpleWithLinks.pdf";
            String outFile = destinationFolder + "destCopyingTest04.pdf";
            String cmpFile = sourceFolder + "cmp_destCopyingTest04.pdf";
            PdfDocument srcDoc = new PdfDocument(new PdfReader(srcFile));
            PdfDocument destDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(outFile));
            srcDoc.CopyPagesTo(JavaUtil.ArraysAsList(1, 3), destDoc);
            destDoc.Close();
            srcDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFile, destinationFolder, "diff_"
                ));
        }

        [NUnit.Framework.Test]
        public virtual void DestCopyingTest05() {
            String srcFile = sourceFolder + "simpleWithLinks.pdf";
            String outFile = destinationFolder + "destCopyingTest05.pdf";
            String cmpFile = sourceFolder + "cmp_destCopyingTest05.pdf";
            PdfDocument srcDoc = new PdfDocument(new PdfReader(srcFile));
            PdfDocument destDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(outFile));
            srcDoc.CopyPagesTo(JavaUtil.ArraysAsList(1, 2, 3, 1), destDoc);
            destDoc.Close();
            srcDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFile, destinationFolder, "diff_"
                ));
        }

        [NUnit.Framework.Test]
        public virtual void DestCopyingTest06() {
            String srcFile = sourceFolder + "sourceWithNamedDestination.pdf";
            String outFile = destinationFolder + "destCopyingTest06.pdf";
            String cmpFile = sourceFolder + "cmp_destCopyingTest06.pdf";
            PdfDocument srcDoc = new PdfDocument(new PdfReader(srcFile));
            PdfDocument destDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(outFile));
            srcDoc.CopyPagesTo(JavaUtil.ArraysAsList(1, 2, 1), destDoc);
            destDoc.Close();
            srcDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFile, destinationFolder, "diff_"
                ));
        }

        [NUnit.Framework.Test]
        public virtual void DestCopyingTest07() {
            String srcFile = sourceFolder + "sourceStringDestWithPageNumber.pdf";
            String outFile = destinationFolder + "destCopyingTest07.pdf";
            String cmpFile = sourceFolder + "cmp_destCopyingTest07.pdf";
            PdfDocument srcDoc = new PdfDocument(new PdfReader(srcFile));
            PdfDocument destDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(outFile));
            srcDoc.CopyPagesTo(JavaUtil.ArraysAsList(1, 2, 1), destDoc);
            destDoc.Close();
            srcDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFile, destinationFolder, "diff_"
                ));
        }

        [NUnit.Framework.Test]
        public virtual void StructureDestinationWithoutRemoteIdTest() {
            String srcFile = sourceFolder + "customRolesMappingPdf2.pdf";
            PdfDocument document = new PdfDocument(new PdfReader(srcFile), new PdfWriter(new MemoryStream()));
            PdfStructElem imgElement = new PdfStructElem((PdfDictionary)document.GetPdfObject(13));
            try {
                PdfAction.CreateGoToR(new PdfStringFS("Some fake destination"), PdfStructureDestination.CreateFit(imgElement
                    ));
                NUnit.Framework.Assert.Fail("Exception not thrown");
            }
            catch (ArgumentException e) {
                NUnit.Framework.Assert.AreEqual("Structure destinations shall specify structure element ID in remote go-to actions. Structure element that has no ID is specified instead"
                    , e.Message);
            }
            document.Close();
        }

        [NUnit.Framework.Test]
        public virtual void StructureDestination01Test() {
            String srcFile = sourceFolder + "customRolesMappingPdf2.pdf";
            String outFile = destinationFolder + "structureDestination01Test.pdf";
            String cmpFile = sourceFolder + "cmp_structureDestination01Test.pdf";
            PdfDocument document = new PdfDocument(new PdfReader(srcFile), CompareTool.CreateTestPdfWriter(outFile));
            PdfStructElem imgElement = new PdfStructElem((PdfDictionary)document.GetPdfObject(13));
            PdfStructureDestination dest = PdfStructureDestination.CreateFit(imgElement);
            PdfPage secondPage = document.AddNewPage();
            PdfLinkAnnotation linkExplicitDest = new PdfLinkAnnotation(new Rectangle(35, 785, 160, 15));
            linkExplicitDest.SetAction(PdfAction.CreateGoTo(dest));
            secondPage.AddAnnotation(linkExplicitDest);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFile, destinationFolder, "diff_"
                ));
        }

        [NUnit.Framework.Test]
        public virtual void StructureDestination02Test() {
            String srcFile = sourceFolder + "customRolesMappingPdf2.pdf";
            String outFile = destinationFolder + "structureDestination02Test.pdf";
            String cmpFile = sourceFolder + "cmp_structureDestination02Test.pdf";
            PdfDocument document = new PdfDocument(new PdfReader(srcFile), CompareTool.CreateTestPdfWriter(outFile));
            PdfStructElem imgElement = new PdfStructElem((PdfDictionary)document.GetPdfObject(13));
            PdfStructureDestination dest = PdfStructureDestination.CreateFit(imgElement);
            PdfPage secondPage = document.AddNewPage();
            PdfPage thirdPage = document.AddNewPage();
            PdfLinkAnnotation linkExplicitDest = new PdfLinkAnnotation(new Rectangle(35, 785, 160, 15));
            PdfAction gotoStructAction = PdfAction.CreateGoTo(PdfExplicitDestination.CreateFit(thirdPage));
            gotoStructAction.Put(PdfName.SD, dest.GetPdfObject());
            linkExplicitDest.SetAction(gotoStructAction);
            secondPage.AddAnnotation(linkExplicitDest);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFile, destinationFolder, "diff_"
                ));
        }

        [NUnit.Framework.Test]
        public virtual void MakeDestination01Test() {
            String srcFile = sourceFolder + "cmp_structureDestination01Test.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(srcFile));
            PdfObject destObj = ((PdfLinkAnnotation)pdfDocument.GetPage(2).GetAnnotations()[0]).GetAction().Get(PdfName
                .D);
            PdfDestination destWrapper = PdfDestination.MakeDestination(destObj);
            NUnit.Framework.Assert.AreEqual(typeof(PdfStructureDestination), destWrapper.GetType());
        }

        [NUnit.Framework.Test]
        public virtual void RemoteGoToDestinationTest01() {
            String cmpFile = sourceFolder + "cmp_remoteGoToDestinationTest01.pdf";
            String outFile = destinationFolder + "remoteGoToDestinationTest01.pdf";
            PdfDocument @out = new PdfDocument(CompareTool.CreateTestPdfWriter(outFile));
            @out.AddNewPage();
            IList<PdfDestination> destinations = new List<PdfDestination>(7);
            destinations.Add(PdfExplicitRemoteGoToDestination.CreateFit(1));
            destinations.Add(PdfExplicitRemoteGoToDestination.CreateFitH(1, 10));
            destinations.Add(PdfExplicitRemoteGoToDestination.CreateFitV(1, 10));
            destinations.Add(PdfExplicitRemoteGoToDestination.CreateFitR(1, 10, 10, 10, 10));
            destinations.Add(PdfExplicitRemoteGoToDestination.CreateFitB(1));
            destinations.Add(PdfExplicitRemoteGoToDestination.CreateFitBH(1, 10));
            destinations.Add(PdfExplicitRemoteGoToDestination.CreateFitBV(1, 10));
            int y = 785;
            foreach (PdfDestination destination in destinations) {
                PdfLinkAnnotation linkExplicitDest = new PdfLinkAnnotation(new Rectangle(35, y, 160, 15));
                PdfAction action = PdfAction.CreateGoToR(new PdfStringFS("Some fake destination"), destination);
                linkExplicitDest.SetAction(action);
                @out.GetFirstPage().AddAnnotation(linkExplicitDest);
                y -= 20;
            }
            @out.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFile, destinationFolder, "diff_"
                ));
        }

        [NUnit.Framework.Test]
        public virtual void RemoteGoToDestinationTest02() {
            String cmpFile = sourceFolder + "cmp_remoteGoToDestinationTest02.pdf";
            String outFile = destinationFolder + "remoteGoToDestinationTest02.pdf";
            PdfDocument @out = new PdfDocument(CompareTool.CreateTestPdfWriter(outFile));
            @out.AddNewPage();
            @out.AddNewPage();
            PdfLinkAnnotation linkExplicitDest = new PdfLinkAnnotation(new Rectangle(35, 785, 160, 15));
            PdfAction action = PdfAction.CreateGoToR(new PdfStringFS("Some fake destination"), PdfExplicitRemoteGoToDestination
                .CreateFitR(2, 10, 10, 10, 10), true);
            linkExplicitDest.SetAction(action);
            @out.GetFirstPage().AddAnnotation(linkExplicitDest);
            @out.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFile, destinationFolder, "diff_"
                ));
        }

        [NUnit.Framework.Test]
        public virtual void RemoteGoToRIllegalDestinationTest() {
            String outFile = destinationFolder + "remoteGoToDestinationTest01.pdf";
            PdfDocument document = new PdfDocument(CompareTool.CreateTestPdfWriter(outFile));
            document.AddNewPage();
            document.AddNewPage();
            try {
                PdfAction.CreateGoToR(new PdfStringFS("Some fake destination"), PdfExplicitDestination.CreateFitB(document
                    .GetPage(1)));
                NUnit.Framework.Assert.Fail("Exception not thrown");
            }
            catch (ArgumentException e) {
                NUnit.Framework.Assert.AreEqual("Explicit destinations shall specify page number in remote go-to actions instead of page dictionary"
                    , e.Message);
            }
            document.Close();
        }

        [NUnit.Framework.Test]
        public virtual void RemoteGoToRByIntDestinationTest() {
            String cmpFile = sourceFolder + "cmp_remoteGoToRByIntDestinationTest.pdf";
            String outFile = destinationFolder + "remoteGoToRByIntDestinationTest.pdf";
            PdfDocument @out = new PdfDocument(CompareTool.CreateTestPdfWriter(outFile));
            @out.AddNewPage();
            @out.AddNewPage();
            PdfLinkAnnotation linkExplicitDest = new PdfLinkAnnotation(new Rectangle(35, 785, 160, 15));
            PdfAction action = PdfAction.CreateGoToR("Some fake destination", 2);
            linkExplicitDest.SetAction(action);
            @out.GetFirstPage().AddAnnotation(linkExplicitDest);
            @out.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFile, destinationFolder, "diff_"
                ));
        }

        [NUnit.Framework.Test]
        public virtual void RemoteGoToRByStringDestinationTest() {
            String cmpFile = sourceFolder + "cmp_remoteGoToRByStringDestinationTest.pdf";
            String outFile = destinationFolder + "remoteGoToRByStringDestinationTest.pdf";
            PdfDocument @out = new PdfDocument(CompareTool.CreateTestPdfWriter(outFile));
            @out.AddNewPage();
            PdfLinkAnnotation linkExplicitDest = new PdfLinkAnnotation(new Rectangle(35, 785, 160, 15));
            PdfAction action = PdfAction.CreateGoToR("Some fake destination", "1");
            linkExplicitDest.SetAction(action);
            @out.GetFirstPage().AddAnnotation(linkExplicitDest);
            @out.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFile, destinationFolder, "diff_"
                ));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.INVALID_DESTINATION_TYPE)]
        public virtual void RemoteGoToNotValidExplicitDestinationTest() {
            String cmpFile = sourceFolder + "cmp_remoteGoToNotValidExplicitDestinationTest.pdf";
            String outFile = destinationFolder + "remoteGoToNotValidExplicitDestinationTest.pdf";
            PdfDocument document = new PdfDocument(CompareTool.CreateTestPdfWriter(outFile));
            document.AddNewPage();
            PdfLinkAnnotation linkExplicitDest = new PdfLinkAnnotation(new Rectangle(35, 785, 160, 15));
            linkExplicitDest.SetAction(PdfAction.CreateGoTo(PdfExplicitRemoteGoToDestination.CreateFit(1)));
            document.GetFirstPage().AddAnnotation(linkExplicitDest);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFile, cmpFile, destinationFolder, "diff_"
                ));
        }

        [NUnit.Framework.Test]
        public virtual void CopyNullDestination() {
            using (MemoryStream baos = new MemoryStream()) {
                using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(baos))) {
                    pdfDocument.AddNewPage();
                    PdfDestination copiedDestination = pdfDocument.GetCatalog().CopyDestination(null, new Dictionary<PdfPage, 
                        PdfPage>(), pdfDocument);
                    // We expect null to be returned if the destination to be copied is null
                    NUnit.Framework.Assert.IsNull(copiedDestination);
                }
            }
        }
    }
}
