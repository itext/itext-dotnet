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
using iText.Commons.Utils;
using iText.IO.Font;
using iText.Kernel.Exceptions;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Tagging;
using iText.Kernel.Pdf.Tagutils;
using iText.Kernel.Utils;
using iText.Pdfua.Exceptions;
using iText.Test;
using iText.Test.Attributes;
using iText.Test.Pdfa;

namespace iText.Pdfua {
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfUARoleMappingTest : ExtendedITextTest {
        private static readonly String FONT = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfua/font/FreeSans.ttf";

        private static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/pdfua/PdfUARoleMappingTest/";

        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfua/PdfUARoleMappingTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void Before() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void NonStandardMappingViaTagTreePointer1_02_001_Test() {
            String outPdf = DESTINATION_FOLDER + "nonStandardMappingViaTagTreePointer1Test.pdf";
            PdfUATestPdfDocument pdfDoc = new PdfUATestPdfDocument(new PdfWriter(outPdf, PdfUATestPdfDocument.CreateWriterProperties
                ()));
            PdfPage page1 = pdfDoc.AddNewPage();
            TagTreePointer tagPointer = new TagTreePointer(pdfDoc).SetPageForTagging(page1);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => tagPointer.AddTag("chapter"));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(KernelExceptionMessageConstant.ROLE_IS_NOT_MAPPED_TO_ANY_STANDARD_ROLE
                , "chapter"), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void NonStandardMappingViaTagTreePointer2_02_001_Test() {
            String outPdf = DESTINATION_FOLDER + "nonStandardMappingViaTagTreePointer2Test.pdf";
            PdfUATestPdfDocument pdfDoc = new PdfUATestPdfDocument(new PdfWriter(outPdf, PdfUATestPdfDocument.CreateWriterProperties
                ()));
            PdfPage page1 = pdfDoc.AddNewPage();
            PdfStructTreeRoot root = pdfDoc.GetStructTreeRoot();
            root.AddRoleMapping("chapter", "chapterChild");
            root.AddRoleMapping("chapterChild", "chapterGrandchild");
            TagTreePointer tagPointer = new TagTreePointer(pdfDoc).SetPageForTagging(page1);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => tagPointer.AddTag("chapter"));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(KernelExceptionMessageConstant.ROLE_IS_NOT_MAPPED_TO_ANY_STANDARD_ROLE
                , "chapter"), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void NonStandardMappingViaTagTreePointer3_02_001_Test() {
            String outPdf = DESTINATION_FOLDER + "nonStandardMappingViaTagTreePointer3Test.pdf";
            PdfUATestPdfDocument pdfDoc = new PdfUATestPdfDocument(new PdfWriter(outPdf, PdfUATestPdfDocument.CreateWriterProperties
                ()));
            PdfPage page1 = pdfDoc.AddNewPage();
            TagTreePointer tagPointer = new TagTreePointer(pdfDoc).SetPageForTagging(page1);
            // Although PDF/UA defines the nomenclature for heading levels above <H6> (<Hn>), these are not standard
            // structure types and therefore <Hn> tags must be role-mapped to a standard structure type.
            // According to PDF/UA-1, PDF/UA-conforming processors are expected to ignore such mappings and respect the heading level.
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => tagPointer.AddTag("H7"));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(KernelExceptionMessageConstant.ROLE_IS_NOT_MAPPED_TO_ANY_STANDARD_ROLE
                , "H7"), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void NonStandardMappingViaPdfName_02_001_Test() {
            String outPdf = DESTINATION_FOLDER + "nonStandardMappingViaPdfNameTest.pdf";
            PdfUATestPdfDocument pdfDoc = new PdfUATestPdfDocument(new PdfWriter(outPdf, PdfUATestPdfDocument.CreateWriterProperties
                ()));
            PdfFont font = PdfFontFactory.CreateFont(FONT, PdfEncodings.WINANSI, PdfFontFactory.EmbeddingStrategy.FORCE_EMBEDDED
                );
            PdfPage page1 = pdfDoc.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page1);
            // Another attempts of PDF/UA document creation with non-standard tags see in PdfUACanvasTest class
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfUAConformanceException), () => canvas.OpenTag(new CanvasTag
                (new PdfName("chapter"))));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(PdfUAExceptionMessageConstants.TAG_MAPPING_DOESNT_TERMINATE_WITH_STANDARD_TYPE
                , "chapter"), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void NonStandardMappingViaPdfMcr_02_001_Test() {
            String outPdf = DESTINATION_FOLDER + "nonStandardMappingViaPdfMcrTest.pdf";
            PdfUATestPdfDocument pdfDoc = new PdfUATestPdfDocument(new PdfWriter(outPdf, PdfUATestPdfDocument.CreateWriterProperties
                ()));
            PdfPage page1 = pdfDoc.AddNewPage();
            PdfStructElem doc = pdfDoc.GetStructTreeRoot().AddKid(new PdfStructElem(pdfDoc, PdfName.Document));
            PdfStructElem paragraph = doc.AddKid(new PdfStructElem(pdfDoc, new PdfName("chapter"), page1));
            PdfMcr mcr = paragraph.AddKid(new PdfMcrNumber(page1, paragraph));
            PdfCanvas canvas = new PdfCanvas(page1);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfUAConformanceException), () => canvas.OpenTag(new CanvasTag
                (mcr)));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(PdfUAExceptionMessageConstants.TAG_MAPPING_DOESNT_TERMINATE_WITH_STANDARD_TYPE
                , "chapter"), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void StandardMappingViaTagTreePointer_02_001_Test() {
            String outPdf = DESTINATION_FOLDER + "standardMappingViaTagTreePointerTest.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_standardMappingViaTagTreePointerTest.pdf";
            PdfUATestPdfDocument pdfDoc = new PdfUATestPdfDocument(new PdfWriter(outPdf, PdfUATestPdfDocument.CreateWriterProperties
                ()));
            PdfFont font = PdfFontFactory.CreateFont(FONT, PdfEncodings.WINANSI, PdfFontFactory.EmbeddingStrategy.FORCE_EMBEDDED
                );
            PdfStructTreeRoot root = pdfDoc.GetStructTreeRoot();
            root.AddRoleMapping("chapter", "chapterChild");
            root.AddRoleMapping("chapterChild", StandardRoles.SECT);
            PdfPage page1 = pdfDoc.AddNewPage();
            PdfCanvas canvas = new PdfCanvas(page1);
            TagTreePointer tagPointer = new TagTreePointer(pdfDoc).SetPageForTagging(page1).AddTag("chapter");
            canvas.OpenTag(tagPointer.GetTagReference()).BeginText().SetFontAndSize(font, 12).MoveText(200, 200).ShowText
                ("Hello World!").EndText().CloseTag();
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER, "diff_"
                ));
            NUnit.Framework.Assert.IsNull(new UAVeraPdfValidator().Validate(outPdf));
        }

        // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf/ua validation on Android)
        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.VERSION_INCOMPATIBILITY_FOR_DICTIONARY_ENTRY, Count = 2, LogLevel
             = LogLevelConstants.WARN)]
        public virtual void StandardMappingViaNamespace_02_001_Test() {
            String outPdf = DESTINATION_FOLDER + "standardMappingViaNamespaceTest.pdf";
            PdfUATestPdfDocument pdfDoc = new PdfUATestPdfDocument(new PdfWriter(outPdf, PdfUATestPdfDocument.CreateWriterProperties
                ()));
            PdfPage page1 = pdfDoc.AddNewPage();
            PdfStructElem doc = pdfDoc.GetStructTreeRoot().AddKid(new PdfStructElem(pdfDoc, PdfName.Document));
            PdfStructElem paragraph = doc.AddKid(new PdfStructElem(pdfDoc, PdfName.P));
            PdfStructElem chapter = paragraph.AddKid(new PdfStructElem(pdfDoc, new PdfName("chapter"), page1));
            // Napespaces are actual only for PDF-2.0, which is actual only for PDF/UA-2
            PdfNamespace @namespace = new PdfNamespace("http://www.w3.org/1999/xhtml");
            chapter.SetNamespace(@namespace);
            @namespace.AddNamespaceRoleMapping("chapter", StandardRoles.SPAN);
            pdfDoc.GetStructTreeRoot().AddNamespace(@namespace);
            PdfMcr mcr = chapter.AddKid(new PdfMcrNumber(page1, chapter));
            PdfCanvas canvas = new PdfCanvas(page1);
            // VeraPdf also complains about non-standard mapping
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfUAConformanceException), () => canvas.OpenTag(new CanvasTag
                (mcr)));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(PdfUAExceptionMessageConstants.TAG_MAPPING_DOESNT_TERMINATE_WITH_STANDARD_TYPE
                , "chapter"), e.Message);
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.CANNOT_RESOLVE_ROLE_TOO_MUCH_TRANSITIVE_MAPPINGS, LogLevel = 
            LogLevelConstants.ERROR)]
        public virtual void CycleMappingViaTagTreePointer1_02_003_Test() {
            String outPdf = DESTINATION_FOLDER + "cycleMappingViaTagTreePointer1Test.pdf";
            PdfUATestPdfDocument pdfDoc = new PdfUATestPdfDocument(new PdfWriter(outPdf, PdfUATestPdfDocument.CreateWriterProperties
                ()));
            PdfPage page1 = pdfDoc.AddNewPage();
            PdfStructTreeRoot root = pdfDoc.GetStructTreeRoot();
            root.AddRoleMapping("chapter", "chapterChild");
            root.AddRoleMapping("chapterChild", "chapter");
            TagTreePointer tagPointer = new TagTreePointer(pdfDoc).SetPageForTagging(page1);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => tagPointer.AddTag("chapter"));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(KernelExceptionMessageConstant.ROLE_IS_NOT_MAPPED_TO_ANY_STANDARD_ROLE
                , "chapter"), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void CycleMappingViaTagTreePointer2_02_003_Test() {
            String outPdf = DESTINATION_FOLDER + "cycleMappingViaTagTreePointer2Test.pdf";
            PdfUATestPdfDocument pdfDoc = new PdfUATestPdfDocument(new PdfWriter(outPdf, PdfUATestPdfDocument.CreateWriterProperties
                ()));
            PdfPage page1 = pdfDoc.AddNewPage();
            PdfFont font = PdfFontFactory.CreateFont(FONT, PdfEncodings.WINANSI, PdfFontFactory.EmbeddingStrategy.FORCE_EMBEDDED
                );
            PdfStructTreeRoot root = pdfDoc.GetStructTreeRoot();
            root.AddRoleMapping("chapter", "chapterChild");
            root.AddRoleMapping("chapterChild", StandardRoles.SPAN);
            root.AddRoleMapping(StandardRoles.SPAN, "chapter");
            TagTreePointer tagPointer = new TagTreePointer(pdfDoc).SetPageForTagging(page1).AddTag("chapter");
            PdfCanvas canvas = new PdfCanvas(page1);
            canvas.OpenTag(tagPointer.GetTagReference()).BeginText().SetFontAndSize(font, 12).MoveText(200, 200).ShowText
                ("Hello World!").EndText().CloseTag();
            // VeraPdf complains about circular mapping
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => pdfDoc.Close());
            NUnit.Framework.Assert.AreEqual(PdfUAExceptionMessageConstants.ONE_OR_MORE_STANDARD_ROLE_REMAPPED, e.Message
                );
        }

        [NUnit.Framework.Test]
        public virtual void MappingStandardRoles_02_004_Test() {
            String outPdf = DESTINATION_FOLDER + "mappingStandardRolesTest.pdf";
            PdfUATestPdfDocument pdfDoc = new PdfUATestPdfDocument(new PdfWriter(outPdf, PdfUATestPdfDocument.CreateWriterProperties
                ()));
            PdfPage page1 = pdfDoc.AddNewPage();
            PdfFont font = PdfFontFactory.CreateFont(FONT, PdfEncodings.WINANSI, PdfFontFactory.EmbeddingStrategy.FORCE_EMBEDDED
                );
            PdfStructTreeRoot root = pdfDoc.GetStructTreeRoot();
            root.AddRoleMapping("chapter", "chapterChild");
            root.AddRoleMapping("chapterChild", StandardRoles.SPAN);
            root.AddRoleMapping(StandardRoles.SPAN, StandardRoles.SECT);
            TagTreePointer tagPointer = new TagTreePointer(pdfDoc).SetPageForTagging(page1).AddTag("chapter");
            PdfCanvas canvas = new PdfCanvas(page1);
            canvas.OpenTag(tagPointer.GetTagReference()).BeginText().SetFontAndSize(font, 12).MoveText(200, 200).ShowText
                ("Hello World!").EndText().CloseTag();
            // VeraPdf doesn't complain
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => pdfDoc.Close());
            NUnit.Framework.Assert.AreEqual(PdfUAExceptionMessageConstants.ONE_OR_MORE_STANDARD_ROLE_REMAPPED, e.Message
                );
        }
    }
}
