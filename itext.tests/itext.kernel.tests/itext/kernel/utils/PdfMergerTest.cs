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
using iText.Kernel.Logs;
using iText.Kernel.Pdf;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Kernel.Utils {
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfMergerTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/utils/PdfMergerTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/kernel/utils/PdfMergerTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.OneTimeTearDown]
        public static void AfterClass() {
            CompareTool.Cleanup(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void MergeDocumentTest01() {
            String filename = sourceFolder + "courierTest.pdf";
            String filename1 = sourceFolder + "helveticaTest.pdf";
            String filename2 = sourceFolder + "timesRomanTest.pdf";
            String resultFile = destinationFolder + "mergedResult01.pdf";
            PdfReader reader = new PdfReader(filename);
            PdfReader reader1 = new PdfReader(filename1);
            PdfReader reader2 = new PdfReader(filename2);
            PdfWriter writer1 = CompareTool.CreateTestPdfWriter(resultFile);
            PdfDocument pdfDoc = new PdfDocument(reader);
            PdfDocument pdfDoc1 = new PdfDocument(reader1);
            PdfDocument pdfDoc2 = new PdfDocument(reader2);
            PdfDocument pdfDoc3 = new PdfDocument(writer1);
            PdfMerger merger = new PdfMerger(pdfDoc3).SetCloseSourceDocuments(true);
            merger.Merge(pdfDoc, 1, 1);
            merger.Merge(pdfDoc1, 1, 1);
            merger.Merge(pdfDoc2, 1, 1);
            pdfDoc3.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(resultFile, sourceFolder + "cmp_mergedResult01.pdf"
                , destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.SOURCE_DOCUMENT_HAS_ACROFORM_DICTIONARY)]
        public virtual void MergeDocumentOutlinesWithNullDestinationTest01() {
            String resultFile = destinationFolder + "mergeDocumentOutlinesWithNullDestinationTest01.pdf";
            String filename = sourceFolder + "null_dest_outline.pdf";
            PdfDocument sourceDocument = new PdfDocument(new PdfReader(filename));
            PdfMerger resultDocument = new PdfMerger(new PdfDocument(CompareTool.CreateTestPdfWriter(resultFile)));
            resultDocument.Merge(sourceDocument, 1, 1);
            resultDocument.Close();
            sourceDocument.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(resultFile, sourceFolder + "cmp_mergeDocumentOutlinesWithNullDestinationTest01.pdf"
                , destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void MergeDocumentWithCycleRefInAcroFormTest() {
            String filename1 = sourceFolder + "doc1.pdf";
            String filename2 = sourceFolder + "pdfWithCycleRefInAnnotationParent.pdf";
            String resultFile = destinationFolder + "resultFileWithoutStackOverflow.pdf";
            using (PdfDocument pdfDocument1 = new PdfDocument(new PdfReader(filename2))) {
                using (PdfDocument pdfDocument2 = new PdfDocument(new PdfReader(filename1), CompareTool.CreateTestPdfWriter
                    (resultFile).SetSmartMode(true))) {
                    PdfMerger merger = new PdfMerger(pdfDocument2);
                    merger.Merge(pdfDocument1, 1, pdfDocument1.GetNumberOfPages());
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(resultFile, sourceFolder + "cmp_resultFileWithoutStackOverflow.pdf"
                , destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void MergeDocumentWithLinkAnnotationTest() {
            String filename = sourceFolder + "documentWithLinkAnnotation.pdf";
            String resultFile = destinationFolder + "mergedDocumentWithLinkAnnotation.pdf";
            PdfReader reader = new PdfReader(filename);
            PdfWriter writer1 = CompareTool.CreateTestPdfWriter(resultFile);
            PdfDocument pdfDoc = new PdfDocument(reader);
            PdfDocument result = new PdfDocument(writer1);
            PdfMerger merger = new PdfMerger(result).SetCloseSourceDocuments(true);
            merger.Merge(pdfDoc, 1, 1).Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(resultFile, sourceFolder + "cmp_mergedDocumentWithLinkAnnotation.pdf"
                , destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void MergeDocumentTest02() {
            String filename = sourceFolder + "doc1.pdf";
            String filename1 = sourceFolder + "doc2.pdf";
            String filename2 = sourceFolder + "doc3.pdf";
            String resultFile = destinationFolder + "mergedResult02.pdf";
            PdfReader reader = new PdfReader(filename);
            PdfReader reader1 = new PdfReader(filename1);
            PdfReader reader2 = new PdfReader(filename2);
            PdfWriter writer1 = CompareTool.CreateTestPdfWriter(resultFile);
            PdfDocument pdfDoc = new PdfDocument(reader);
            PdfDocument pdfDoc1 = new PdfDocument(reader1);
            PdfDocument pdfDoc2 = new PdfDocument(reader2);
            PdfDocument pdfDoc3 = new PdfDocument(writer1);
            PdfMerger merger = new PdfMerger(pdfDoc3).SetCloseSourceDocuments(true);
            merger.Merge(pdfDoc, 1, 1).Merge(pdfDoc1, 1, 1).Merge(pdfDoc2, 1, 1).Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(resultFile, sourceFolder + "cmp_mergedResult02.pdf"
                , destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void MergeDocumentWithCycleTagReferenceTest() {
            String filename1 = sourceFolder + "doc1.pdf";
            String filename2 = sourceFolder + "pdfWithCycleRefInParentTag.pdf";
            String resultFile = destinationFolder + "pdfWithCycleRefInParentTag.pdf";
            using (PdfDocument pdfDocument1 = new PdfDocument(new PdfReader(filename2))) {
                using (PdfDocument pdfDocument2 = new PdfDocument(new PdfReader(filename1), CompareTool.CreateTestPdfWriter
                    (resultFile).SetSmartMode(true))) {
                    PdfMerger merger = new PdfMerger(pdfDocument2);
                    merger.Merge(pdfDocument1, 1, pdfDocument1.GetNumberOfPages());
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(resultFile, sourceFolder + "cmp_pdfWithCycleRefInParentTag.pdf"
                , destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void MergeDocumentWithCycleReferenceInFormFieldTest() {
            String filename1 = sourceFolder + "doc1.pdf";
            String filename2 = sourceFolder + "pdfWithCycleRefInFormField.pdf";
            String resultFile = destinationFolder + "pdfWithCycleRefInFormField.pdf";
            using (PdfDocument pdfDocument1 = new PdfDocument(new PdfReader(filename2))) {
                using (PdfDocument pdfDocument2 = new PdfDocument(new PdfReader(filename1), CompareTool.CreateTestPdfWriter
                    (resultFile).SetSmartMode(true))) {
                    PdfMerger merger = new PdfMerger(pdfDocument2);
                    merger.Merge(pdfDocument1, 1, pdfDocument1.GetNumberOfPages());
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(resultFile, sourceFolder + "cmp_pdfWithCycleRefInFormField.pdf"
                , destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.SOURCE_DOCUMENT_HAS_ACROFORM_DICTIONARY)]
        public virtual void MergeDocumentTest03() {
            String filename = sourceFolder + "pdf_open_parameters.pdf";
            String filename1 = sourceFolder + "iphone_user_guide.pdf";
            String resultFile = destinationFolder + "mergedResult03.pdf";
            PdfReader reader = new PdfReader(filename);
            PdfReader reader1 = new PdfReader(filename1);
            PdfWriter writer1 = CompareTool.CreateTestPdfWriter(resultFile);
            PdfDocument pdfDoc = new PdfDocument(reader);
            PdfDocument pdfDoc1 = new PdfDocument(reader1);
            PdfDocument pdfDoc3 = new PdfDocument(writer1);
            pdfDoc3.SetTagged();
            new PdfMerger(pdfDoc3).Merge(pdfDoc, 2, 2).Merge(pdfDoc1, 7, 8).Close();
            pdfDoc.Close();
            pdfDoc1.Close();
            CompareTool compareTool = new CompareTool();
            String errorMessage = "";
            String contentErrorMessage = compareTool.CompareByContent(resultFile, sourceFolder + "cmp_mergedResult03.pdf"
                , destinationFolder, "diff_");
            String tagStructErrorMessage = compareTool.CompareTagStructures(resultFile, sourceFolder + "cmp_mergedResult03.pdf"
                );
            errorMessage += tagStructErrorMessage == null ? "" : tagStructErrorMessage + "\n";
            errorMessage += contentErrorMessage == null ? "" : contentErrorMessage;
            if (!String.IsNullOrEmpty(errorMessage)) {
                NUnit.Framework.Assert.Fail(errorMessage);
            }
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.SOURCE_DOCUMENT_HAS_ACROFORM_DICTIONARY)]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.CREATED_ROOT_TAG_HAS_MAPPING, Count = 2)]
        public virtual void MergeDocumentTest04() {
            String filename = sourceFolder + "pdf_open_parameters.pdf";
            String filename1 = sourceFolder + "iphone_user_guide.pdf";
            String resultFile = destinationFolder + "mergedResult04.pdf";
            PdfReader reader = new PdfReader(filename);
            PdfReader reader1 = new PdfReader(filename1);
            PdfWriter writer1 = CompareTool.CreateTestPdfWriter(resultFile);
            PdfDocument pdfDoc = new PdfDocument(reader);
            PdfDocument pdfDoc1 = new PdfDocument(reader1);
            PdfDocument pdfDoc3 = new PdfDocument(writer1);
            pdfDoc3.SetTagged();
            PdfMerger merger = new PdfMerger(pdfDoc3).SetCloseSourceDocuments(true);
            IList<int> pages = new List<int>();
            pages.Add(3);
            pages.Add(2);
            pages.Add(1);
            merger.Merge(pdfDoc, pages);
            IList<int> pages1 = new List<int>();
            pages1.Add(5);
            pages1.Add(9);
            pages1.Add(4);
            pages1.Add(3);
            merger.Merge(pdfDoc1, pages1);
            merger.Close();
            CompareTool compareTool = new CompareTool();
            String errorMessage = "";
            String contentErrorMessage = compareTool.CompareByContent(resultFile, sourceFolder + "cmp_mergedResult04.pdf"
                , destinationFolder, "diff_");
            String tagStructErrorMessage = compareTool.CompareTagStructures(resultFile, sourceFolder + "cmp_mergedResult04.pdf"
                );
            errorMessage += tagStructErrorMessage == null ? "" : tagStructErrorMessage + "\n";
            errorMessage += contentErrorMessage == null ? "" : contentErrorMessage;
            if (!String.IsNullOrEmpty(errorMessage)) {
                NUnit.Framework.Assert.Fail(errorMessage);
            }
        }

        [NUnit.Framework.Test]
        public virtual void MergeTableWithEmptyTdTest() {
            MergeAndCompareTagStructures("tableWithEmptyTd.pdf", 1, 1);
        }

        [NUnit.Framework.Test]
        public virtual void MergeSplitTableWithEmptyTdTest() {
            MergeAndCompareTagStructures("splitTableWithEmptyTd.pdf", 2, 2);
        }

        [NUnit.Framework.Test]
        public virtual void MergeEmptyRowWithTagsTest() {
            MergeAndCompareTagStructures("emptyRowWithTags.pdf", 1, 1);
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.SOURCE_DOCUMENT_HAS_ACROFORM_DICTIONARY)]
        public virtual void TrInsideTdTableTest() {
            MergeAndCompareTagStructures("trInsideTdTable.pdf", 1, 1);
        }

        [NUnit.Framework.Test]
        public virtual void TdInsideTdTableTest() {
            MergeAndCompareTagStructures("tdInsideTdTable.pdf", 1, 1);
        }

        [NUnit.Framework.Test]
        public virtual void EmptyTrTableTest() {
            MergeAndCompareTagStructures("emptyTrTable.pdf", 1, 1);
        }

        [NUnit.Framework.Test]
        public virtual void SplitEmptyTrTableFirstPageTest() {
            MergeAndCompareTagStructures("splitTableWithEmptyTrFirstPage.pdf", 1, 1);
        }

        [NUnit.Framework.Test]
        public virtual void SplitEmptyTrTableSecondPageTest() {
            MergeAndCompareTagStructures("splitTableWithEmptyTrSecondPage.pdf", 2, 2);
        }

        [NUnit.Framework.Test]
        public virtual void SplitEmptyTrTableFullTest() {
            MergeAndCompareTagStructures("splitTableWithEmptyTrFull.pdf", 1, 2);
        }

        [NUnit.Framework.Test]
        public virtual void EmptyFirstTrTableTest() {
            MergeAndCompareTagStructures("emptyFirstTrTable.pdf", 1, 1);
        }

        [NUnit.Framework.Test]
        public virtual void EmptyLastTrTableTest() {
            MergeAndCompareTagStructures("emptyLastTrTable.pdf", 1, 1);
        }

        [NUnit.Framework.Test]
        public virtual void EmptyTwoAdjacentTrTableTest() {
            MergeAndCompareTagStructures("emptyTwoAdjacentTrTable.pdf", 1, 1);
        }

        [NUnit.Framework.Test]
        public virtual void EmptyAllTrTableTest() {
            MergeAndCompareTagStructures("emptyAllTrTable.pdf", 1, 1);
        }

        [NUnit.Framework.Test]
        public virtual void EmptySingleTrTableTest() {
            MergeAndCompareTagStructures("emptySingleTrTable.pdf", 1, 1);
        }

        [NUnit.Framework.Test]
        public virtual void SplitAndMergeEmptyTrTableTest() {
            String sourceFilename = sourceFolder + "splitTableWithEmptyTrFull.pdf";
            String firstPageFilename = destinationFolder + "firstPageDoc.pdf";
            String secondPageFilename = destinationFolder + "secondPageDoc.pdf";
            String resultFilename = destinationFolder + "splitAndMergeEmptyTrTable.pdf";
            String cmpFilename = sourceFolder + "cmp_splitAndMergeEmptyTrTable.pdf";
            PdfDocument sourceDoc = new PdfDocument(new PdfReader(sourceFilename));
            PdfDocument firstPageDoc = new PdfDocument(new PdfWriter(firstPageFilename));
            PdfMerger mergerFirstPage = new PdfMerger(firstPageDoc);
            mergerFirstPage.Merge(sourceDoc, 1, 1);
            mergerFirstPage.Close();
            PdfDocument secondPageDoc = new PdfDocument(new PdfWriter(secondPageFilename));
            PdfMerger mergerSecondPage = new PdfMerger(secondPageDoc);
            mergerSecondPage.Merge(sourceDoc, 2, 2);
            mergerSecondPage.Close();
            IList<FileInfo> sources = new List<FileInfo>();
            sources.Add(new FileInfo(firstPageFilename));
            sources.Add(new FileInfo(secondPageFilename));
            MergePdfs(sources, resultFilename, new PdfMergerProperties(), false);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareTagStructures(resultFilename, cmpFilename));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.NAME_ALREADY_EXISTS_IN_THE_NAME_TREE, Count = 2)]
        public virtual void MergeOutlinesNamedDestinations() {
            String filename = sourceFolder + "outlinesNamedDestinations.pdf";
            String resultFile = destinationFolder + "mergeOutlinesNamedDestinations.pdf";
            PdfReader reader = new PdfReader(filename);
            PdfDocument sourceDoc = new PdfDocument(reader);
            PdfDocument output = new PdfDocument(CompareTool.CreateTestPdfWriter(resultFile));
            PdfMerger merger = new PdfMerger(output).SetCloseSourceDocuments(false);
            merger.Merge(sourceDoc, 2, 3);
            merger.Merge(sourceDoc, 2, 3);
            sourceDoc.Close();
            reader.Close();
            merger.Close();
            output.Close();
            CompareTool compareTool = new CompareTool();
            String errorMessage = compareTool.CompareByContent(resultFile, sourceFolder + "cmp_mergeOutlinesNamedDestinations.pdf"
                , destinationFolder, "diff_");
            if (errorMessage != null) {
                NUnit.Framework.Assert.Fail(errorMessage);
            }
        }

        [NUnit.Framework.Test]
        // TODO DEVSIX-1743. Update cmp file after fix
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.SOURCE_DOCUMENT_HAS_ACROFORM_DICTIONARY)]
        public virtual void MergeWithAcroFormsTest() {
            String pdfAcro1 = sourceFolder + "pdfSource1.pdf";
            String pdfAcro2 = sourceFolder + "pdfSource2.pdf";
            String outFileName = destinationFolder + "mergeWithAcroFormsTest.pdf";
            String cmpFileName = sourceFolder + "cmp_mergeWithAcroFormsTest.pdf";
            IList<FileInfo> sources = new List<FileInfo>();
            sources.Add(new FileInfo(pdfAcro1));
            sources.Add(new FileInfo(pdfAcro2));
            MergePdfs(sources, outFileName, false);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                ));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.DOCUMENT_HAS_CONFLICTING_OCG_NAMES, Count = 3)]
        public virtual void MergePdfWithOCGTest() {
            String pdfWithOCG1 = sourceFolder + "sourceOCG1.pdf";
            String pdfWithOCG2 = sourceFolder + "sourceOCG2.pdf";
            String outPdf = destinationFolder + "mergePdfWithOCGTest.pdf";
            String cmpPdf = sourceFolder + "cmp_mergePdfWithOCGTest.pdf";
            IList<FileInfo> sources = new List<FileInfo>();
            sources.Add(new FileInfo(pdfWithOCG1));
            sources.Add(new FileInfo(pdfWithOCG2));
            sources.Add(new FileInfo(pdfWithOCG2));
            sources.Add(new FileInfo(pdfWithOCG2));
            MergePdfs(sources, outPdf, false);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, destinationFolder));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.DOCUMENT_HAS_CONFLICTING_OCG_NAMES)]
        public virtual void MergePdfWithComplexOCGTest() {
            String pdfWithOCG1 = sourceFolder + "sourceOCG1.pdf";
            String pdfWithOCG2 = sourceFolder + "pdfWithComplexOCG.pdf";
            String outPdf = destinationFolder + "mergePdfWithComplexOCGTest.pdf";
            String cmpPdf = sourceFolder + "cmp_mergePdfWithComplexOCGTest.pdf";
            IList<FileInfo> sources = new List<FileInfo>();
            sources.Add(new FileInfo(pdfWithOCG1));
            sources.Add(new FileInfo(pdfWithOCG2));
            MergePdfs(sources, outPdf, false);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, destinationFolder));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.DOCUMENT_HAS_CONFLICTING_OCG_NAMES)]
        public virtual void MergeTwoPagePdfWithComplexOCGTest() {
            String pdfWithOCG1 = sourceFolder + "sourceOCG1.pdf";
            String pdfWithOCG2 = sourceFolder + "twoPagePdfWithComplexOCGTest.pdf";
            String outPdf = destinationFolder + "mergeTwoPagePdfWithComplexOCGTest.pdf";
            String cmpPdf = sourceFolder + "cmp_mergeTwoPagePdfWithComplexOCGTest.pdf";
            PdfDocument mergedDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(outPdf));
            PdfMerger merger = new PdfMerger(mergedDoc);
            IList<FileInfo> sources = new List<FileInfo>();
            sources.Add(new FileInfo(pdfWithOCG1));
            sources.Add(new FileInfo(pdfWithOCG2));
            // The test verifies that are copying only those OCGs and properties that are used on the copied pages
            foreach (FileInfo source in sources) {
                PdfDocument sourcePdf = new PdfDocument(new PdfReader(source));
                merger.Merge(sourcePdf, 1, 1).SetCloseSourceDocuments(true);
                sourcePdf.Close();
            }
            merger.Close();
            mergedDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, destinationFolder));
        }

        [NUnit.Framework.Test]
        public virtual void MergePdfWithComplexOCGTwiceTest() {
            String pdfWithOCG = sourceFolder + "pdfWithComplexOCG.pdf";
            String outPdf = destinationFolder + "mergePdfWithComplexOCGTwiceTest.pdf";
            String cmpPdf = sourceFolder + "cmp_mergePdfWithComplexOCGTwiceTest.pdf";
            PdfDocument mergedDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(outPdf));
            PdfMerger merger = new PdfMerger(mergedDoc);
            PdfDocument sourcePdf = new PdfDocument(new PdfReader(new FileInfo(pdfWithOCG)));
            // The test verifies that identical layers from the same document are not copied
            merger.Merge(sourcePdf, 1, sourcePdf.GetNumberOfPages());
            merger.Merge(sourcePdf, 1, sourcePdf.GetNumberOfPages());
            sourcePdf.Close();
            merger.Close();
            mergedDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, destinationFolder));
        }

        [NUnit.Framework.Test]
        public virtual void StackOverflowErrorCycleReferenceOcgMergeTest() {
            String outPdf = destinationFolder + "cycleReferenceMerged.pdf";
            String cmpPdf = sourceFolder + "cmp_stackOverflowErrorCycleReferenceOcrMerge.pdf";
            PdfDocument pdfWithOCG = new PdfDocument(new PdfReader(sourceFolder + "sourceOCG1.pdf"), CompareTool.CreateTestPdfWriter
                (outPdf));
            PdfDocument pdfWithOCGToMerge = new PdfDocument(new PdfReader(sourceFolder + "stackOverflowErrorCycleReferenceOcgMerge.pdf"
                ));
            // problem file
            PdfMerger merger = new PdfMerger(pdfWithOCG);
            merger.Merge(pdfWithOCGToMerge, 1, pdfWithOCGToMerge.GetNumberOfPages());
            pdfWithOCGToMerge.Close();
            pdfWithOCG.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, destinationFolder));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.SOURCE_DOCUMENT_HAS_ACROFORM_DICTIONARY)]
        public virtual void MergeOutlinesWithWrongStructureTest() {
            PdfDocument inputDoc = new PdfDocument(new PdfReader(sourceFolder + "infiniteLoopInOutlineStructure.pdf"));
            PdfDocument outputDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(destinationFolder + "infiniteLoopInOutlineStructure.pdf"
                ));
            PdfMerger merger = new PdfMerger(outputDoc, new PdfMergerProperties().SetMergeTags(false).SetMergeOutlines
                (true));
            System.Console.Out.WriteLine("Doing merge");
            merger.Merge(inputDoc, 1, 2);
            merger.Close();
            System.Console.Out.WriteLine("Merge done");
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + "infiniteLoopInOutlineStructure.pdf"
                , sourceFolder + "cmp_infiniteLoopInOutlineStructure.pdf", destinationFolder));
        }

        private static void MergeAndCompareTagStructures(String testName, int fromPage, int toPage) {
            String src = sourceFolder + testName;
            String dest = destinationFolder + testName;
            String cmp = sourceFolder + "cmp_" + testName;
            PdfReader reader = new PdfReader(src);
            PdfDocument sourceDoc = new PdfDocument(reader);
            PdfDocument output = new PdfDocument(CompareTool.CreateTestPdfWriter(dest));
            output.SetTagged();
            PdfMerger merger = new PdfMerger(output).SetCloseSourceDocuments(true);
            merger.Merge(sourceDoc, fromPage, toPage);
            sourceDoc.Close();
            reader.Close();
            merger.Close();
            output.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareTagStructures(dest, cmp));
        }

        [NUnit.Framework.Test]
        public virtual void MergeDocumentWithColorPropertyInOutlineTest() {
            String firstDocument = sourceFolder + "firstDocumentWithColorPropertyInOutline.pdf";
            String secondDocument = sourceFolder + "SecondDocumentWithColorPropertyInOutline.pdf";
            String cmpDocument = sourceFolder + "cmp_mergeOutlinesWithColorProperty.pdf";
            String mergedPdf = destinationFolder + "mergeOutlinesWithColorProperty.pdf";
            using (PdfDocument merged = new PdfDocument(CompareTool.CreateTestPdfWriter(mergedPdf))) {
                using (PdfDocument fileA = new PdfDocument(new PdfReader(firstDocument))) {
                    using (PdfDocument fileB = new PdfDocument(new PdfReader(secondDocument))) {
                        PdfMerger merger = new PdfMerger(merged, new PdfMergerProperties().SetMergeTags(false).SetMergeOutlines(true
                            ));
                        merger.Merge(fileA, 1, fileA.GetNumberOfPages());
                        merger.Merge(fileB, 1, fileB.GetNumberOfPages());
                        merger.Close();
                    }
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(mergedPdf, cmpDocument, destinationFolder
                ));
        }

        [NUnit.Framework.Test]
        public virtual void MergeDocumentWithStylePropertyInOutlineTest() {
            String firstDocument = sourceFolder + "firstDocumentWithStylePropertyInOutline.pdf";
            String secondDocument = sourceFolder + "secondDocumentWithStylePropertyInOutline.pdf";
            String cmpPdf = sourceFolder + "cmp_mergeOutlineWithStyleProperty.pdf";
            String mergedPdf = destinationFolder + "mergeOutlineWithStyleProperty.pdf";
            using (PdfDocument documentA = new PdfDocument(new PdfReader(firstDocument))) {
                using (PdfDocument documentB = new PdfDocument(new PdfReader(secondDocument))) {
                    using (PdfDocument merged = new PdfDocument(CompareTool.CreateTestPdfWriter(mergedPdf))) {
                        PdfMerger merger = new PdfMerger(merged, new PdfMergerProperties().SetMergeTags(false).SetMergeOutlines(true
                            ));
                        merger.Merge(documentA, 1, documentA.GetNumberOfPages());
                        merger.Merge(documentB, 1, documentB.GetNumberOfPages());
                        merger.Close();
                    }
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(mergedPdf, cmpPdf, destinationFolder));
        }

        [NUnit.Framework.Test]
        public virtual void MergePdfDocumentsWithCopingOutlinesTest() {
            String firstPdfDocument = sourceFolder + "firstDocumentWithOutlines.pdf";
            String secondPdfDocument = sourceFolder + "secondDocumentWithOutlines.pdf";
            String cmpDocument = sourceFolder + "cmp_mergeDocumentsWithOutlines.pdf";
            String mergedDocument = destinationFolder + "mergeDocumentsWithOutlines.pdf";
            using (PdfDocument documentA = new PdfDocument(new PdfReader(firstPdfDocument))) {
                using (PdfDocument documentB = new PdfDocument(new PdfReader(secondPdfDocument))) {
                    using (PdfDocument mergedPdf = new PdfDocument(CompareTool.CreateTestPdfWriter(mergedDocument))) {
                        PdfMerger merger = new PdfMerger(mergedPdf, new PdfMergerProperties().SetMergeTags(false).SetMergeOutlines
                            (true));
                        merger.Merge(documentA, 1, documentA.GetNumberOfPages());
                        merger.Merge(documentB, 1, documentB.GetNumberOfPages());
                        merger.Close();
                    }
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(mergedDocument, cmpDocument, destinationFolder
                ));
        }

        [NUnit.Framework.Test]
        public virtual void MergeWithSameNamedOcgTest() {
            String firstPdfDocument = sourceFolder + "sameNamdOCGSource.pdf";
            String secondPdfDocument = sourceFolder + "doc2.pdf";
            String cmpDocument = sourceFolder + "cmp_MergeWithSameNamedOCG.pdf";
            String mergedDocument = destinationFolder + "mergeWithSameNamedOCG.pdf";
            IList<FileInfo> sources = new List<FileInfo>();
            sources.Add(new FileInfo(firstPdfDocument));
            sources.Add(new FileInfo(secondPdfDocument));
            MergePdfs(sources, mergedDocument, true);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(mergedDocument, cmpDocument, destinationFolder
                ));
            // We have to compare visually also because compareByContent doesn't catch the differences in OCGs with the same names
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareVisually(mergedDocument, cmpDocument, destinationFolder
                , "diff_"));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.SOURCE_DOCUMENT_HAS_ACROFORM_DICTIONARY)]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.DOCUMENT_HAS_CONFLICTING_OCG_NAMES)]
        public virtual void MergeWithSameNamedOcgOcmdDTest() {
            String firstPdfDocument = sourceFolder + "Layer doc1.pdf";
            String secondPdfDocument = sourceFolder + "Layer doc2.pdf";
            String cmpDocument = sourceFolder + "cmp_mergeWithSameNamedOCMD.pdf";
            String mergedDocument = destinationFolder + "mergeWithSameNamedOCMD.pdf";
            IList<FileInfo> sources = new List<FileInfo>();
            sources.Add(new FileInfo(firstPdfDocument));
            sources.Add(new FileInfo(secondPdfDocument));
            MergePdfs(sources, mergedDocument, true);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(mergedDocument, cmpDocument, destinationFolder
                ));
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.STRUCT_PARENT_INDEX_MISSED_AND_RECREATED)]
        public virtual void MergePdfWithMissingStructElemBeginningOfTreeTest() {
            String name = "structParentMissingFirstElement.pdf";
            NUnit.Framework.Assert.IsNotNull(MergeSinglePdfAndGetResultingStructTreeRoot(name));
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + name, sourceFolder + 
                "cmp_" + name, destinationFolder));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.SOURCE_DOCUMENT_HAS_ACROFORM_DICTIONARY)]
        [LogMessage(KernelLogMessageConstant.STRUCT_PARENT_INDEX_MISSED_AND_RECREATED)]
        public virtual void MergePdfWithMissingStructElemEndOfTreeTest() {
            String name = "structParentMissingLastElement.pdf";
            NUnit.Framework.Assert.IsNotNull(MergeSinglePdfAndGetResultingStructTreeRoot(name));
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + name, sourceFolder + 
                "cmp_" + name, destinationFolder));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.SOURCE_DOCUMENT_HAS_ACROFORM_DICTIONARY)]
        [LogMessage(KernelLogMessageConstant.STRUCT_PARENT_INDEX_MISSED_AND_RECREATED, Count = 4)]
        public virtual void MergePdfAllObjectsMissingStructParentTest() {
            String name = "allObjectsHaveStructParent.pdf";
            NUnit.Framework.Assert.IsNotNull(MergeSinglePdfAndGetResultingStructTreeRoot(name));
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + name, sourceFolder + 
                "cmp_" + name, destinationFolder));
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.STRUCT_PARENT_INDEX_MISSED_AND_RECREATED, Count = 2)]
        public virtual void MergePdfChildObjectsOfSameStructElemMissingStructParentTest() {
            String name = "SameStructElemNoParent.pdf";
            NUnit.Framework.Assert.IsNotNull(MergeSinglePdfAndGetResultingStructTreeRoot(name));
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + name, sourceFolder + 
                "cmp_" + name, destinationFolder));
        }

        [NUnit.Framework.Test]
        public virtual void MergeDocumentsWithStringAdditionalActions() {
            String firstPdfDocument = sourceFolder + "doc1.pdf";
            String secondPdfDocument = sourceFolder + "docAAString.pdf";
            String cmpDocument = sourceFolder + "cmp_mergeAAString.pdf";
            String mergedDocument = destinationFolder + "mergedAAString.pdf";
            IList<FileInfo> sources = new List<FileInfo>();
            sources.Add(new FileInfo(firstPdfDocument));
            sources.Add(new FileInfo(secondPdfDocument));
            MergePdfs(sources, mergedDocument, new PdfMergerProperties().SetMergeScripts(true), true);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(mergedDocument, cmpDocument, destinationFolder
                ));
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.CANNOT_MERGE_ENTRY)]
        public virtual void MergeDocumentsWithAdditionalActionsInDestination() {
            String firstPdfDocument = sourceFolder + "docAAStream.pdf";
            String secondPdfDocument = sourceFolder + "docAAString.pdf";
            String cmpDocument = sourceFolder + "cmp_mergeAA2.pdf";
            String mergedDocument = destinationFolder + "mergedAAInDest.pdf";
            IList<FileInfo> sources = new List<FileInfo>();
            sources.Add(new FileInfo(firstPdfDocument));
            sources.Add(new FileInfo(secondPdfDocument));
            MergePdfs(sources, mergedDocument, new PdfMergerProperties().SetMergeScripts(true), true);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(mergedDocument, cmpDocument, destinationFolder
                ));
        }

        [NUnit.Framework.Test]
        public virtual void MergeDocumentsWithUnexpectedKeyAdditionalActions() {
            String firstPdfDocument = sourceFolder + "doc1.pdf";
            String secondPdfDocument = sourceFolder + "docAAStringWithUnexpectedKey.pdf";
            String cmpDocument = sourceFolder + "cmp_mergeAAUnexpectedKey.pdf";
            String mergedDocument = destinationFolder + "mergedAAUnexpectedKey.pdf";
            IList<FileInfo> sources = new List<FileInfo>();
            sources.Add(new FileInfo(firstPdfDocument));
            sources.Add(new FileInfo(secondPdfDocument));
            MergePdfs(sources, mergedDocument, new PdfMergerProperties().SetMergeScripts(true), true);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(mergedDocument, cmpDocument, destinationFolder
                ));
        }

        [NUnit.Framework.Test]
        public virtual void MergeDocumentsWithStreamAdditionalActions() {
            String firstPdfDocument = sourceFolder + "doc1.pdf";
            String secondPdfDocument = sourceFolder + "docAAStream.pdf";
            String cmpDocument = sourceFolder + "cmp_mergeAAStream.pdf";
            String mergedDocument = destinationFolder + "mergedAAStream.pdf";
            IList<FileInfo> sources = new List<FileInfo>();
            sources.Add(new FileInfo(firstPdfDocument));
            sources.Add(new FileInfo(secondPdfDocument));
            MergePdfs(sources, mergedDocument, new PdfMergerProperties().SetMergeScripts(true), true);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(mergedDocument, cmpDocument, destinationFolder
                ));
        }

        [NUnit.Framework.Test]
        public virtual void MergeDocumentsWithStringOpenActions() {
            String firstPdfDocument = sourceFolder + "doc1.pdf";
            String secondPdfDocument = sourceFolder + "docOAString.pdf";
            String cmpDocument = sourceFolder + "cmp_mergeOAString.pdf";
            String mergedDocument = destinationFolder + "mergedOAString.pdf";
            IList<FileInfo> sources = new List<FileInfo>();
            sources.Add(new FileInfo(firstPdfDocument));
            sources.Add(new FileInfo(secondPdfDocument));
            MergePdfs(sources, mergedDocument, new PdfMergerProperties().SetMergeScripts(true), true);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(mergedDocument, cmpDocument, destinationFolder
                ));
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.CANNOT_MERGE_ENTRY)]
        public virtual void MergeDocumentsWithOpenActionInDestination() {
            String firstPdfDocument = sourceFolder + "docOAStream.pdf";
            String secondPdfDocument = sourceFolder + "docOAString.pdf";
            String cmpDocument = sourceFolder + "cmp_mergeOA2.pdf";
            String mergedDocument = destinationFolder + "mergedOAInDest.pdf";
            IList<FileInfo> sources = new List<FileInfo>();
            sources.Add(new FileInfo(firstPdfDocument));
            sources.Add(new FileInfo(secondPdfDocument));
            MergePdfs(sources, mergedDocument, new PdfMergerProperties().SetMergeScripts(true), true);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(mergedDocument, cmpDocument, destinationFolder
                ));
        }

        [NUnit.Framework.Test]
        public virtual void MergeDocumentsWithStreamOpenActions() {
            String firstPdfDocument = sourceFolder + "doc1.pdf";
            String secondPdfDocument = sourceFolder + "docOAStream.pdf";
            String cmpDocument = sourceFolder + "cmp_mergeOAStream.pdf";
            String mergedDocument = destinationFolder + "mergedOAStream.pdf";
            IList<FileInfo> sources = new List<FileInfo>();
            sources.Add(new FileInfo(firstPdfDocument));
            sources.Add(new FileInfo(secondPdfDocument));
            MergePdfs(sources, mergedDocument, new PdfMergerProperties().SetMergeScripts(true), true);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(mergedDocument, cmpDocument, destinationFolder
                ));
        }

        [NUnit.Framework.Test]
        public virtual void MergeDocumentsWithJSInTree() {
            String firstPdfDocument = sourceFolder + "doc1.pdf";
            String secondPdfDocument = sourceFolder + "docJS.pdf";
            String cmpDocument = sourceFolder + "cmp_mergeJS.pdf";
            String mergedDocument = destinationFolder + "mergedJS.pdf";
            IList<FileInfo> sources = new List<FileInfo>();
            sources.Add(new FileInfo(firstPdfDocument));
            sources.Add(new FileInfo(secondPdfDocument));
            MergePdfs(sources, mergedDocument, new PdfMergerProperties().SetMergeScripts(true), true);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(mergedDocument, cmpDocument, destinationFolder
                ));
        }

        [NUnit.Framework.Test]
        public virtual void MergeDocumentsWithNullDestination() {
            String firstPdfDocument = sourceFolder + "doc1.pdf";
            String secondPdfDocument = sourceFolder + "linkAnnotationWithNullDestinationTest.pdf";
            String cmpDocument = sourceFolder + "cmp_linkAnnotationWithNullDestinationTest.pdf";
            String mergedDocument = destinationFolder + "mergedLinkAnnotationWithNullDestinationTest.pdf";
            IList<FileInfo> sources = new List<FileInfo>();
            sources.Add(new FileInfo(firstPdfDocument));
            sources.Add(new FileInfo(secondPdfDocument));
            MergePdfs(sources, mergedDocument, new PdfMergerProperties().SetMergeScripts(true), true);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(mergedDocument, cmpDocument, destinationFolder
                ));
        }

        [NUnit.Framework.Test]
        public virtual void MergeDocumentsWithNullDestinationInGoTo() {
            String firstPdfDocument = sourceFolder + "doc1.pdf";
            String secondPdfDocument = sourceFolder + "linkAnnotationWithNullDestinationInGoToTest.pdf";
            String cmpDocument = sourceFolder + "cmp_linkAnnotationWithNullDestinationInGoToTest.pdf";
            String mergedDocument = destinationFolder + "mergedLinkAnnotationWithNullDestinationInGoToTest.pdf";
            IList<FileInfo> sources = new List<FileInfo>();
            sources.Add(new FileInfo(firstPdfDocument));
            sources.Add(new FileInfo(secondPdfDocument));
            MergePdfs(sources, mergedDocument, new PdfMergerProperties().SetMergeScripts(true), true);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(mergedDocument, cmpDocument, destinationFolder
                ));
        }

        [NUnit.Framework.Test]
        public virtual void MergeDocumentsWithPdfNullDestinationInGoTo() {
            String firstPdfDocument = sourceFolder + "doc1.pdf";
            String secondPdfDocument = sourceFolder + "linkAnnotationWithPdfNullDestinationInGoToTest.pdf";
            String cmpDocument = sourceFolder + "cmp_linkAnnotationWithPdfNullDestinationInGoToTest.pdf";
            String mergedDocument = destinationFolder + "mergedLinkAnnotationWithPdfNullDestinationInGoToTest.pdf";
            IList<FileInfo> sources = new List<FileInfo>();
            sources.Add(new FileInfo(firstPdfDocument));
            sources.Add(new FileInfo(secondPdfDocument));
            MergePdfs(sources, mergedDocument, new PdfMergerProperties().SetMergeScripts(true), true);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(mergedDocument, cmpDocument, destinationFolder
                ));
        }

        [NUnit.Framework.Test]
        [LogMessage(KernelLogMessageConstant.CANNOT_MERGE_ENTRY)]
        public virtual void MergeDocumentsWithNamesJSInDestination() {
            String firstPdfDocument = sourceFolder + "cmp_mergeJS.pdf";
            String secondPdfDocument = sourceFolder + "docJS.pdf";
            String cmpDocument = sourceFolder + "cmp_mergeJS2.pdf";
            String mergedDocument = destinationFolder + "mergedJSInDest.pdf";
            IList<FileInfo> sources = new List<FileInfo>();
            sources.Add(new FileInfo(firstPdfDocument));
            sources.Add(new FileInfo(secondPdfDocument));
            MergePdfs(sources, mergedDocument, new PdfMergerProperties().SetMergeScripts(true), true);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(mergedDocument, cmpDocument, destinationFolder
                ));
        }

        [NUnit.Framework.Test]
        public virtual void CopyEmptyOcPropertiesTest() {
            String filename = sourceFolder + "emptyOcPropertiesDoc.pdf";
            String resultFile = destinationFolder + "mergedEmptyOcPropertiesDoc.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(filename));
            PdfDocument result = new PdfDocument(CompareTool.CreateTestPdfWriter(resultFile));
            PdfMerger merger = new PdfMerger(result).SetCloseSourceDocuments(true);
            merger.Merge(pdfDoc, 1, 1).Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(resultFile, sourceFolder + "cmp_mergedEmptyOcPropertiesDoc.pdf"
                , destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void CopyOnlyEmptyOcPropertiesTest() {
            String filename = sourceFolder + "ocPropertiesDoc.pdf";
            String resultFile = destinationFolder + "mergedOcPropertiesDoc.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(filename));
            PdfDocument result = new PdfDocument(CompareTool.CreateTestPdfWriter(resultFile));
            PdfMerger merger = new PdfMerger(result).SetCloseSourceDocuments(true);
            merger.Merge(pdfDoc, 1, 1).Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(resultFile, sourceFolder + "cmp_mergedOcPropertiesDoc.pdf"
                , destinationFolder, "diff_"));
        }

        [LogMessage(iText.IO.Logs.IoLogMessageConstant.SOURCE_DOCUMENT_HAS_ACROFORM_DICTIONARY)]
        [NUnit.Framework.Test]
        public virtual void CombineTagRootKidsTest() {
            String filename1 = sourceFolder + "tagRootKidsDoc1.pdf";
            String filename2 = sourceFolder + "tagRootKidsDoc2.pdf";
            String resultFile = destinationFolder + "mergedTags.pdf";
            PdfDocument result = new PdfDocument(CompareTool.CreateTestPdfWriter(resultFile));
            PdfMerger merger = new PdfMerger(result, new PdfMergerProperties().SetMergeTags(true).SetMergeOutlines(true
                )).SetCloseSourceDocuments(true);
            PdfDocument input1 = new PdfDocument(new PdfReader(filename1));
            merger.Merge(input1, 1, 1);
            input1.Close();
            PdfDocument input2 = new PdfDocument(new PdfReader(filename2));
            merger.Merge(input2, 1, 1);
            input2.Close();
            merger.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(resultFile, sourceFolder + "cmp_mergedTags.pdf"
                , destinationFolder, "diff_"));
        }

        private PdfDictionary MergeSinglePdfAndGetResultingStructTreeRoot(String pathToMerge) {
            IList<FileInfo> sources = new List<FileInfo>();
            sources.Add(new FileInfo(sourceFolder + pathToMerge));
            String mergedDoc = destinationFolder + pathToMerge;
            MergePdfs(sources, mergedDoc, true);
            return GetStructTreeRootOfDocument(mergedDoc);
        }

        private PdfDictionary GetStructTreeRootOfDocument(String pathToFile) {
            PdfDocument mergedDocument = new PdfDocument(new PdfReader(pathToFile));
            return mergedDocument.GetCatalog().GetPdfObject().GetAsDictionary(PdfName.StructTreeRoot);
        }

        private void MergePdfs(IList<FileInfo> sources, String destination, bool smartMode) {
            PdfDocument mergedDoc = new PdfDocument(new PdfWriter(destination));
            mergedDoc.GetWriter().SetSmartMode(smartMode);
            PdfMerger merger = new PdfMerger(mergedDoc);
            foreach (FileInfo source in sources) {
                PdfDocument sourcePdf = new PdfDocument(new PdfReader(source));
                merger.Merge(sourcePdf, 1, sourcePdf.GetNumberOfPages()).SetCloseSourceDocuments(true);
                sourcePdf.Close();
            }
            merger.Close();
            mergedDoc.Close();
        }

        private void MergePdfs(IList<FileInfo> sources, String destination, PdfMergerProperties properties, bool smartMode
            ) {
            PdfDocument mergedDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(destination));
            mergedDoc.GetWriter().SetSmartMode(smartMode);
            PdfMerger merger = new PdfMerger(mergedDoc, properties);
            foreach (FileInfo source in sources) {
                PdfDocument sourcePdf = new PdfDocument(new PdfReader(source));
                merger.Merge(sourcePdf, 1, sourcePdf.GetNumberOfPages()).SetCloseSourceDocuments(true);
                sourcePdf.Close();
            }
            merger.Close();
            mergedDoc.Close();
        }
    }
}
