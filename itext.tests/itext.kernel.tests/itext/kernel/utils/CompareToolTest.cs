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
using System.IO;
using iText.Commons.Utils;
using iText.IO.Exceptions;
using iText.IO.Font.Constants;
using iText.IO.Util;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Annot;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Navigation;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Kernel.Utils {
    [NUnit.Framework.Category("IntegrationTest")]
    public class CompareToolTest : ExtendedITextTest {
        // Android-Conversion-Skip-File (during Android conversion the class will be replaced by DeferredCompareTool)
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/utils/CompareToolTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/kernel/utils/CompareToolTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void SetUp() {
            CreateOrClearDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.OneTimeTearDown]
        public static void AfterClass() {
            CompareTool.Cleanup(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void CompareToolErrorReportTest01() {
            CompareTool compareTool = new CompareTool();
            compareTool.SetCompareByContentErrorsLimit(10);
            compareTool.SetGenerateCompareByContentXmlReport(true);
            String outPdf = sourceFolder + "simple_pdf.pdf";
            String cmpPdf = sourceFolder + "cmp_simple_pdf.pdf";
            String result = compareTool.CompareByContent(outPdf, cmpPdf, destinationFolder);
            System.Console.Out.WriteLine("\nRESULT:\n" + result);
            NUnit.Framework.Assert.IsNotNull("CompareTool must return differences found between the files", result);
            NUnit.Framework.Assert.IsTrue(result.Contains("differs on page [1, 2]."));
            // Comparing the report to the reference one.
            NUnit.Framework.Assert.IsTrue(compareTool.CompareXmls(destinationFolder + "simple_pdf.report.xml", sourceFolder
                 + "cmp_report01.xml"), "CompareTool report differs from the reference one");
        }

        [NUnit.Framework.Test]
        public virtual void CompareToolErrorReportTest02() {
            CompareTool compareTool = new CompareTool();
            compareTool.SetCompareByContentErrorsLimit(10);
            compareTool.SetGenerateCompareByContentXmlReport(true);
            String outPdf = sourceFolder + "tagged_pdf.pdf";
            String cmpPdf = sourceFolder + "cmp_tagged_pdf.pdf";
            String result = compareTool.CompareByContent(outPdf, cmpPdf, destinationFolder);
            System.Console.Out.WriteLine("\nRESULT:\n" + result);
            NUnit.Framework.Assert.IsNotNull("CompareTool must return differences found between the files", result);
            NUnit.Framework.Assert.IsTrue(result.Contains("Compare by content fails. No visual differences"));
            // Comparing the report to the reference one.
            NUnit.Framework.Assert.IsTrue(compareTool.CompareXmls(destinationFolder + "tagged_pdf.report.xml", sourceFolder
                 + "cmp_report02.xml"), "CompareTool report differs from the reference one");
        }

        [NUnit.Framework.Test]
        public virtual void CompareToolErrorReportTest03() {
            CompareTool compareTool = new CompareTool();
            compareTool.SetCompareByContentErrorsLimit(10);
            compareTool.SetGenerateCompareByContentXmlReport(true);
            String outPdf = sourceFolder + "screenAnnotation.pdf";
            String cmpPdf = sourceFolder + "cmp_screenAnnotation.pdf";
            String result = compareTool.CompareByContent(outPdf, cmpPdf, destinationFolder);
            System.Console.Out.WriteLine("\nRESULT:\n" + result);
            NUnit.Framework.Assert.IsNotNull("CompareTool must return differences found between the files", result);
            NUnit.Framework.Assert.IsTrue(result.Contains("Compare by content fails. No visual differences"));
            // Comparing the report to the reference one.
            NUnit.Framework.Assert.IsTrue(compareTool.CompareXmls(destinationFolder + "screenAnnotation.report.xml", sourceFolder
                 + "cmp_report03.xml"), "CompareTool report differs from the reference one");
        }

        [NUnit.Framework.Test]
        public virtual void CompareToolErrorReportTest04() {
            // Test space in name
            CompareTool compareTool = new CompareTool();
            compareTool.SetCompareByContentErrorsLimit(10);
            compareTool.SetGenerateCompareByContentXmlReport(true);
            String outPdf = sourceFolder + "simple_pdf.pdf";
            String cmpPdf = sourceFolder + "cmp_simple_pdf_with_space .pdf";
            String result = compareTool.CompareByContent(outPdf, cmpPdf, destinationFolder);
            System.Console.Out.WriteLine("\nRESULT:\n" + result);
            NUnit.Framework.Assert.IsNotNull("CompareTool must return differences found between the files", result);
            NUnit.Framework.Assert.IsTrue(result.Contains("differs on page [1, 2]."));
            // Comparing the report to the reference one.
            NUnit.Framework.Assert.IsTrue(compareTool.CompareXmls(destinationFolder + "simple_pdf.report.xml", sourceFolder
                 + "cmp_report01.xml"), "CompareTool report differs from the reference one");
        }

        [NUnit.Framework.Test]
        public virtual void DifferentProducerTest() {
            String expectedMessage = "Document info fail. Expected: \"iText\u00ae <version> \u00a9<copyright years> Apryse Group NV (iText Software; licensed version)\", actual: \"iText\u00ae <version> \u00a9<copyright years> Apryse Group NV (AGPL-version)\"";
            String licensed = sourceFolder + "producerLicensed.pdf";
            String agpl = sourceFolder + "producerAGPL.pdf";
            NUnit.Framework.Assert.AreEqual(expectedMessage, new CompareTool().CompareDocumentInfo(agpl, licensed));
        }

        [NUnit.Framework.Test]
        public virtual void VersionReplaceTest() {
            String initial = "iText® 1.10.10-SNAPSHOT (licensed to iText) ©2000-2018 Apryse Group NV";
            String replacedExpected = "iText® <version> (licensed to iText) ©<copyright years> Apryse Group NV";
            NUnit.Framework.Assert.AreEqual(replacedExpected, new CompareTool().ConvertProducerLine(initial));
        }

        [NUnit.Framework.Test]
        public virtual void GsEnvironmentVariableIsNotSpecifiedExceptionTest() {
            String outPdf = sourceFolder + "simple_pdf.pdf";
            String cmpPdf = sourceFolder + "cmp_simple_pdf.pdf";
            new CompareTool(null, null).CompareVisually(outPdf, cmpPdf, destinationFolder, "diff_");
            NUnit.Framework.Assert.IsTrue(new FileInfo(destinationFolder + "diff_1.png").Exists);
        }

        [NUnit.Framework.Test]
        public virtual void CompareXmpThrows() {
            CompareTool compareTool = new CompareTool();
            String outPdf = sourceFolder + "simple_pdf.pdf";
            String cmpPdf = sourceFolder + "cmp_simple_pdf.pdf";
            NUnit.Framework.Assert.AreEqual("XMP parsing failure!", compareTool.CompareXmp(outPdf, cmpPdf));
        }

        [NUnit.Framework.Test]
        public virtual void GsEnvironmentVariableSpecifiedIncorrectlyTest() {
            String outPdf = sourceFolder + "simple_pdf.pdf";
            String cmpPdf = sourceFolder + "cmp_simple_pdf.pdf";
            Exception e = NUnit.Framework.Assert.Catch(typeof(CompareTool.CompareToolExecutionException), () => new CompareTool
                ("unspecified", null).CompareVisually(outPdf, cmpPdf, destinationFolder, "diff_"));
            NUnit.Framework.Assert.AreEqual(IoExceptionMessageConstant.GS_ENVIRONMENT_VARIABLE_IS_NOT_SPECIFIED, e.Message
                );
        }

        [NUnit.Framework.Test]
        public virtual void CompareCommandIsNotSpecifiedTest() {
            String outPdf = sourceFolder + "simple_pdf.pdf";
            String cmpPdf = sourceFolder + "cmp_simple_pdf.pdf";
            String gsExec = SystemUtil.GetEnvironmentVariable(GhostscriptHelper.GHOSTSCRIPT_ENVIRONMENT_VARIABLE);
            if (gsExec == null) {
                gsExec = SystemUtil.GetEnvironmentVariable("gsExec");
            }
            String result = new CompareTool(gsExec, null).CompareVisually(outPdf, cmpPdf, destinationFolder, "diff_");
            NUnit.Framework.Assert.IsFalse(result.Contains(IoExceptionMessageConstant.COMPARE_COMMAND_IS_NOT_SPECIFIED
                ));
            NUnit.Framework.Assert.IsTrue(new FileInfo(destinationFolder + "diff_1.png").Exists);
        }

        [NUnit.Framework.Test]
        [LogMessage(IoExceptionMessageConstant.COMPARE_COMMAND_SPECIFIED_INCORRECTLY)]
        public virtual void CompareCommandSpecifiedIncorrectlyTest() {
            String outPdf = sourceFolder + "simple_pdf.pdf";
            String cmpPdf = sourceFolder + "cmp_simple_pdf.pdf";
            String gsExec = SystemUtil.GetEnvironmentVariable(GhostscriptHelper.GHOSTSCRIPT_ENVIRONMENT_VARIABLE);
            if (gsExec == null) {
                gsExec = SystemUtil.GetEnvironmentVariable("gsExec");
            }
            String result = new CompareTool(gsExec, "unspecified").CompareVisually(outPdf, cmpPdf, destinationFolder, 
                "diff_");
            NUnit.Framework.Assert.IsTrue(result.Contains(IoExceptionMessageConstant.COMPARE_COMMAND_SPECIFIED_INCORRECTLY
                ));
        }

        [NUnit.Framework.Test]
        public virtual void CompareVisuallyDiffTestTest() {
            String outPdf = sourceFolder + "compareVisuallyDiffTestTest1.pdf";
            String cmpPdf = sourceFolder + "compareVisuallyDiffTestTest2.pdf";
            String result = new CompareTool().CompareVisually(outPdf, cmpPdf, destinationFolder, "diff_");
            System.Console.Out.WriteLine("\nRESULT:\n" + result);
            NUnit.Framework.Assert.IsTrue(result.Contains("differs on page [1, 2]."));
            NUnit.Framework.Assert.IsTrue(new FileInfo(destinationFolder + "diff_1.png").Exists);
            NUnit.Framework.Assert.IsTrue(new FileInfo(destinationFolder + "diff_2.png").Exists);
        }

        [NUnit.Framework.Test]
        public virtual void CompareDiffFilesWithSameLinkAnnotationTest() {
            String firstPdf = destinationFolder + "firstPdf.pdf";
            String secondPdf = destinationFolder + "secondPdf.pdf";
            PdfDocument firstDocument = new PdfDocument(CompareTool.CreateTestPdfWriter(firstPdf));
            PdfPage page1FirstDocument = firstDocument.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page1FirstDocument);
            canvas.BeginText();
            canvas.SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.COURIER_BOLD), 14);
            canvas.MoveText(100, 600);
            canvas.ShowText("Page 1");
            canvas.MoveText(0, -30);
            canvas.ShowText("Link to page 1. Click here!");
            canvas.EndText();
            canvas.Release();
            page1FirstDocument.AddAnnotation(new PdfLinkAnnotation(new Rectangle(100, 560, 260, 25)).SetDestination(PdfExplicitDestination
                .CreateFit(page1FirstDocument)).SetBorder(new PdfArray(new float[] { 0, 0, 1 })));
            page1FirstDocument.Flush();
            firstDocument.Close();
            PdfDocument secondDocument = new PdfDocument(CompareTool.CreateTestPdfWriter(secondPdf));
            PdfPage page1secondDocument = secondDocument.AddNewPage();
            canvas = new PdfCanvas(page1secondDocument);
            canvas.BeginText();
            canvas.SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.COURIER_BOLD), 14);
            canvas.MoveText(100, 600);
            canvas.ShowText("Page 1 wit different Text");
            canvas.MoveText(0, -30);
            canvas.ShowText("Link to page 1. Click here!");
            canvas.EndText();
            canvas.Release();
            page1secondDocument.AddAnnotation(new PdfLinkAnnotation(new Rectangle(100, 560, 260, 25)).SetDestination(PdfExplicitDestination
                .CreateFit(page1secondDocument)).SetBorder(new PdfArray(new float[] { 0, 0, 1 })));
            page1secondDocument.Flush();
            secondDocument.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareLinkAnnotations(firstPdf, secondPdf));
        }

        [NUnit.Framework.Test]
        public virtual void CompareFilesWithDiffLinkAnnotationTest() {
            String firstPdf = destinationFolder + "outPdf.pdf";
            String secondPdf = destinationFolder + "secondPdf.pdf";
            PdfDocument firstDocument = new PdfDocument(CompareTool.CreateTestPdfWriter(firstPdf));
            PdfDocument secondDocument = new PdfDocument(CompareTool.CreateTestPdfWriter(secondPdf));
            PdfPage page1FirstDocument = firstDocument.AddNewPage();
            page1FirstDocument.AddAnnotation(new PdfLinkAnnotation(new Rectangle(100, 560, 400, 50)).SetDestination(PdfExplicitDestination
                .CreateFit(page1FirstDocument)).SetBorder(new PdfArray(new float[] { 0, 0, 1 })));
            page1FirstDocument.Flush();
            firstDocument.Close();
            PdfPage page1SecondDocument = secondDocument.AddNewPage();
            page1SecondDocument.AddAnnotation(new PdfLinkAnnotation(new Rectangle(100, 560, 260, 25)).SetDestination(PdfExplicitDestination
                .CreateFit(page1SecondDocument)).SetBorder(new PdfArray(new float[] { 0, 0, 1 })));
            page1SecondDocument.Flush();
            secondDocument.Close();
            NUnit.Framework.Assert.IsNotNull(new CompareTool().CompareLinkAnnotations(firstPdf, secondPdf));
        }

        [NUnit.Framework.Test]
        public virtual void ConvertDocInfoToStringsTest() {
            String inPdf = sourceFolder + "test.pdf";
            CompareTool compareTool = new _T798358249(this);
            using (PdfReader reader = new PdfReader(inPdf, compareTool.GetOutReaderProperties())) {
                using (PdfDocument doc = new PdfDocument(reader)) {
                    String[] docInfo = compareTool.ConvertDocInfoToStrings(doc.GetDocumentInfo());
                    NUnit.Framework.Assert.AreEqual("very long title to compare later on", docInfo[0]);
                    NUnit.Framework.Assert.AreEqual("itextcore", docInfo[1]);
                    NUnit.Framework.Assert.AreEqual("test file", docInfo[2]);
                    NUnit.Framework.Assert.AreEqual("new job", docInfo[3]);
                    NUnit.Framework.Assert.AreEqual("Adobe Acrobat Pro DC (64-bit) <version>", docInfo[4]);
                }
            }
        }

//\cond DO_NOT_DOCUMENT
        internal class _T798358249 : CompareTool {
            protected internal override String[] ConvertDocInfoToStrings(PdfDocumentInfo info) {
                return base.ConvertDocInfoToStrings(info);
            }

            internal _T798358249(CompareToolTest _enclosing) {
                this._enclosing = _enclosing;
            }

            private readonly CompareToolTest _enclosing;
        }
//\endcond

        [NUnit.Framework.Test]
        public virtual void MemoryFirstWriterNoFileTest() {
            String firstPdf = destinationFolder + "memoryFirstWriterNoFileTest.pdf";
            String secondPdf = destinationFolder + "memoryFirstWriterNoFileTest2.pdf";
            PdfDocument firstDocument = new PdfDocument(CompareTool.CreateTestPdfWriter(firstPdf));
            PdfDocument secondDocument = new PdfDocument(CompareTool.CreateTestPdfWriter(secondPdf));
            PdfPage page1FirstDocument = firstDocument.AddNewPage();
            page1FirstDocument.AddAnnotation(new PdfLinkAnnotation(new Rectangle(100, 560, 400, 50)).SetDestination(PdfExplicitDestination
                .CreateFit(page1FirstDocument)).SetBorder(new PdfArray(new float[] { 0, 0, 1 })));
            page1FirstDocument.Flush();
            firstDocument.Close();
            PdfPage page1SecondDocument = secondDocument.AddNewPage();
            page1SecondDocument.AddAnnotation(new PdfLinkAnnotation(new Rectangle(100, 560, 400, 50)).SetDestination(PdfExplicitDestination
                .CreateFit(page1SecondDocument)).SetBorder(new PdfArray(new float[] { 0, 0, 1 })));
            page1SecondDocument.Flush();
            secondDocument.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(firstPdf, secondPdf, destinationFolder));
            NUnit.Framework.Assert.IsFalse(new FileInfo(firstPdf).Exists);
            NUnit.Framework.Assert.IsFalse(new FileInfo(secondPdf).Exists);
        }

        [NUnit.Framework.Test]
        public virtual void DumpMemoryFirstWriterOnDiskTest() {
            String firstPdf = destinationFolder + "dumpMemoryFirstWriterOnDiskTest.pdf";
            String secondPdf = destinationFolder + "dumpMemoryFirstWriterOnDiskTest2.pdf";
            PdfDocument firstDocument = new PdfDocument(CompareTool.CreateTestPdfWriter(firstPdf));
            PdfDocument secondDocument = new PdfDocument(CompareTool.CreateTestPdfWriter(secondPdf));
            PdfPage page1FirstDocument = firstDocument.AddNewPage();
            page1FirstDocument.AddAnnotation(new PdfLinkAnnotation(new Rectangle(100, 560, 400, 50)).SetDestination(PdfExplicitDestination
                .CreateFit(page1FirstDocument)).SetBorder(new PdfArray(new float[] { 0, 0, 1 })));
            page1FirstDocument.Flush();
            firstDocument.Close();
            PdfPage page1SecondDocument = secondDocument.AddNewPage();
            page1SecondDocument.AddAnnotation(new PdfLinkAnnotation(new Rectangle(100, 560, 260, 25)).SetDestination(PdfExplicitDestination
                .CreateFit(page1SecondDocument)).SetBorder(new PdfArray(new float[] { 0, 0, 1 })));
            page1SecondDocument.Flush();
            secondDocument.Close();
            NUnit.Framework.Assert.IsNotNull(new CompareTool().CompareByContent(firstPdf, secondPdf, destinationFolder
                ));
            NUnit.Framework.Assert.IsTrue(new FileInfo(firstPdf).Exists);
            NUnit.Framework.Assert.IsTrue(new FileInfo(secondPdf).Exists);
        }

        [NUnit.Framework.Test]
        public virtual void CleanupTest() {
            CompareTool.CreateTestPdfWriter(destinationFolder + "cleanupTest/cleanupTest.pdf");
            NUnit.Framework.Assert.IsNotNull(MemoryFirstPdfWriter.Get(destinationFolder + "cleanupTest/cleanupTest.pdf"
                ));
            NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => CompareTool.Cleanup(null));
            CompareTool.Cleanup(destinationFolder + "cleanupTest");
            NUnit.Framework.Assert.IsNull(MemoryFirstPdfWriter.Get(destinationFolder + "cleanupTest/cleanupTest.pdf"));
        }
    }
}
