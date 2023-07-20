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
using iText.Forms.Fields;
using iText.IO.Source;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Forms {
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfFormCopyTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/forms/PdfFormCopyTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/forms/PdfFormCopyTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.DOCUMENT_ALREADY_HAS_FIELD, Count = 14)]
        public virtual void CopyFieldsTest01() {
            String srcFilename1 = sourceFolder + "appearances1.pdf";
            String srcFilename2 = sourceFolder + "fieldsOn2-sPage.pdf";
            String srcFilename3 = sourceFolder + "fieldsOn3-sPage.pdf";
            String filename = destinationFolder + "copyFields01.pdf";
            PdfDocument doc1 = new PdfDocument(new PdfReader(srcFilename1));
            PdfDocument doc2 = new PdfDocument(new PdfReader(srcFilename2));
            PdfDocument doc3 = new PdfDocument(new PdfReader(srcFilename3));
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(filename));
            pdfDoc.InitializeOutlines();
            doc3.CopyPagesTo(1, doc3.GetNumberOfPages(), pdfDoc, new PdfPageFormCopier());
            doc2.CopyPagesTo(1, doc2.GetNumberOfPages(), pdfDoc, new PdfPageFormCopier());
            doc1.CopyPagesTo(1, doc1.GetNumberOfPages(), pdfDoc, new PdfPageFormCopier());
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(filename, sourceFolder + "cmp_copyFields01.pdf"
                , destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void CopyFieldsTest02() {
            String srcFilename = sourceFolder + "hello_with_comments.pdf";
            String filename = destinationFolder + "copyFields02.pdf";
            PdfDocument doc1 = new PdfDocument(new PdfReader(srcFilename));
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(filename));
            pdfDoc.InitializeOutlines();
            doc1.CopyPagesTo(1, doc1.GetNumberOfPages(), pdfDoc, new PdfPageFormCopier());
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(filename, sourceFolder + "cmp_copyFields02.pdf"
                , destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void CopyFieldsTest03() {
            String srcFilename = sourceFolder + "hello2_with_comments.pdf";
            String filename = destinationFolder + "copyFields03.pdf";
            PdfDocument doc1 = new PdfDocument(new PdfReader(srcFilename));
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(filename));
            pdfDoc.InitializeOutlines();
            doc1.CopyPagesTo(1, doc1.GetNumberOfPages(), pdfDoc, new PdfPageFormCopier());
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(filename, sourceFolder + "cmp_copyFields03.pdf"
                , destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void LargeFileTest() {
            String srcFilename1 = sourceFolder + "frontpage.pdf";
            String srcFilename2 = sourceFolder + "largeFile.pdf";
            String filename = destinationFolder + "copyLargeFile.pdf";
            PdfDocument doc1 = new PdfDocument(new PdfReader(srcFilename1));
            PdfDocument doc2 = new PdfDocument(new PdfReader(srcFilename2));
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(filename));
            pdfDoc.InitializeOutlines();
            PdfPageFormCopier formCopier = new PdfPageFormCopier();
            doc1.CopyPagesTo(1, doc1.GetNumberOfPages(), pdfDoc, formCopier);
            doc2.CopyPagesTo(1, 10, pdfDoc, formCopier);
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(filename, sourceFolder + "cmp_copyLargeFile.pdf"
                , destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.DOCUMENT_ALREADY_HAS_FIELD)]
        public virtual void CopyFieldsTest04() {
            String srcFilename = sourceFolder + "srcFile1.pdf";
            PdfDocument srcDoc = new PdfDocument(new PdfReader(srcFilename));
            PdfDocument destDoc = new PdfDocument(new PdfWriter(new ByteArrayOutputStream()));
            PdfPageFormCopier formCopier = new PdfPageFormCopier();
            srcDoc.CopyPagesTo(1, srcDoc.GetNumberOfPages(), destDoc, formCopier);
            srcDoc.CopyPagesTo(1, srcDoc.GetNumberOfPages(), destDoc, formCopier);
            PdfAcroForm form = PdfFormCreator.GetAcroForm(destDoc, false);
            NUnit.Framework.Assert.AreEqual(1, form.GetFields().Size());
            NUnit.Framework.Assert.IsNotNull(form.GetField("Name1"));
            destDoc.Close();
        }

        [NUnit.Framework.Test]
        public virtual void CopyFieldsTest05() {
            String srcFilename = sourceFolder + "srcFile1.pdf";
            String destFilename = destinationFolder + "copyFields05.pdf";
            PdfDocument srcDoc = new PdfDocument(new PdfReader(srcFilename));
            PdfDocument destDoc = new PdfDocument(new PdfWriter(destFilename));
            destDoc.AddPage(srcDoc.GetFirstPage().CopyTo(destDoc, new PdfPageFormCopier()));
            destDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destFilename, sourceFolder + "cmp_copyFields05.pdf"
                , destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.DOCUMENT_ALREADY_HAS_FIELD, Count = 9)]
        public virtual void CopyMultipleSubfieldsTest01() {
            String srcFilename = sourceFolder + "copyMultipleSubfieldsTest01.pdf";
            String destFilename = destinationFolder + "copyMultipleSubfieldsTest01.pdf";
            PdfDocument srcDoc = new PdfDocument(new PdfReader(srcFilename));
            PdfDocument destDoc = new PdfDocument(new PdfWriter(destFilename));
            PdfPageFormCopier pdfPageFormCopier = new PdfPageFormCopier();
            // copying the same page from the same document twice
            for (int i = 0; i < 4; ++i) {
                srcDoc.CopyPagesTo(1, 1, destDoc, pdfPageFormCopier);
            }
            PdfAcroForm acroForm = PdfFormCreator.GetAcroForm(destDoc, false);
            acroForm.GetField("text_1").SetValue("Text 1!");
            acroForm.GetField("text_2").SetValue("Text 2!");
            acroForm.GetField("text.3").SetValue("Text 3!");
            acroForm.GetField("text.4").SetValue("Text 4!");
            destDoc.Close();
            srcDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destFilename, sourceFolder + "cmp_copyMultipleSubfieldsTest01.pdf"
                , destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.DOCUMENT_ALREADY_HAS_FIELD, Count = 2)]
        public virtual void CopyMultipleSubfieldsTest02() {
            String srcFilename = sourceFolder + "copyMultipleSubfieldsTest02.pdf";
            String destFilename = destinationFolder + "copyMultipleSubfieldsTest02.pdf";
            PdfDocument srcDoc = new PdfDocument(new PdfReader(srcFilename));
            PdfDocument destDoc = new PdfDocument(new PdfWriter(destFilename));
            PdfPageFormCopier pdfPageFormCopier = new PdfPageFormCopier();
            // copying the same page from the same document twice
            for (int i = 0; i < 3; ++i) {
                srcDoc.CopyPagesTo(1, 1, destDoc, pdfPageFormCopier);
            }
            PdfAcroForm acroForm = PdfFormCreator.GetAcroForm(destDoc, false);
            acroForm.GetField("text.3").SetValue("Text 3!");
            destDoc.Close();
            srcDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destFilename, sourceFolder + "cmp_copyMultipleSubfieldsTest02.pdf"
                , destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.DOCUMENT_ALREADY_HAS_FIELD, Count = 2)]
        public virtual void CopyMultipleSubfieldsTest03() {
            String srcFilename = sourceFolder + "copyMultipleSubfieldsTest03.pdf";
            String destFilename = destinationFolder + "copyMultipleSubfieldsTest03.pdf";
            PdfDocument srcDoc = new PdfDocument(new PdfReader(srcFilename));
            PdfDocument destDoc = new PdfDocument(new PdfWriter(destFilename));
            PdfPageFormCopier pdfPageFormCopier = new PdfPageFormCopier();
            // copying the same page from the same document twice
            for (int i = 0; i < 3; ++i) {
                srcDoc.CopyPagesTo(1, 1, destDoc, pdfPageFormCopier);
            }
            PdfAcroForm acroForm = PdfFormCreator.GetAcroForm(destDoc, false);
            acroForm.GetField("text_1").SetValue("Text 1!");
            destDoc.Close();
            srcDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destFilename, sourceFolder + "cmp_copyMultipleSubfieldsTest03.pdf"
                , destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.DOCUMENT_ALREADY_HAS_FIELD, Count = 9)]
        public virtual void CopyMultipleSubfieldsSmartModeTest01() {
            String srcFilename = sourceFolder + "copyMultipleSubfieldsSmartModeTest01.pdf";
            String destFilename = destinationFolder + "copyMultipleSubfieldsSmartModeTest01.pdf";
            PdfDocument srcDoc = new PdfDocument(new PdfReader(srcFilename));
            PdfDocument destDoc = new PdfDocument(new PdfWriter(destFilename).SetSmartMode(true));
            PdfPageFormCopier pdfPageFormCopier = new PdfPageFormCopier();
            // copying the same page from the same document twice
            for (int i = 0; i < 4; ++i) {
                srcDoc.CopyPagesTo(1, 1, destDoc, pdfPageFormCopier);
            }
            PdfAcroForm acroForm = PdfFormCreator.GetAcroForm(destDoc, false);
            acroForm.GetField("text_1").SetValue("Text 1!");
            acroForm.GetField("text_2").SetValue("Text 2!");
            acroForm.GetField("text.3").SetValue("Text 3!");
            acroForm.GetField("text.4").SetValue("Text 4!");
            destDoc.Close();
            srcDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destFilename, sourceFolder + "cmp_copyMultipleSubfieldsSmartModeTest01.pdf"
                , destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.DOCUMENT_ALREADY_HAS_FIELD, Count = 14)]
        public virtual void CopyFieldsTest06() {
            String srcFilename = sourceFolder + "datasheet.pdf";
            String destFilename = destinationFolder + "copyFields06.pdf";
            PdfDocument srcDoc = new PdfDocument(new PdfReader(srcFilename));
            PdfDocument destDoc = new PdfDocument(new PdfWriter(destFilename));
            PdfPageFormCopier pdfPageFormCopier = new PdfPageFormCopier();
            // copying the same page from the same document twice
            for (int i = 0; i < 2; ++i) {
                srcDoc.CopyPagesTo(1, 1, destDoc, pdfPageFormCopier);
            }
            destDoc.Close();
            srcDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destFilename, sourceFolder + "cmp_copyFields06.pdf"
                , destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.DOCUMENT_ALREADY_HAS_FIELD, Count = 14)]
        public virtual void CopyFieldsTest07() {
            String srcFilename = sourceFolder + "datasheet.pdf";
            String destFilename = destinationFolder + "copyFields07.pdf";
            PdfDocument destDoc = new PdfDocument(new PdfWriter(destFilename));
            PdfPageFormCopier pdfPageFormCopier = new PdfPageFormCopier();
            // copying the same page from reopened document twice
            for (int i = 0; i < 2; ++i) {
                PdfDocument srcDoc = new PdfDocument(new PdfReader(srcFilename));
                srcDoc.CopyPagesTo(1, 1, destDoc, pdfPageFormCopier);
                srcDoc.Close();
            }
            destDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destFilename, sourceFolder + "cmp_copyFields07.pdf"
                , destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.DOCUMENT_ALREADY_HAS_FIELD, Count = 14)]
        public virtual void CopyFieldsTest08() {
            String srcFilename1 = sourceFolder + "appearances1.pdf";
            String srcFilename2 = sourceFolder + "fieldsOn2-sPage.pdf";
            String srcFilename3 = sourceFolder + "fieldsOn3-sPage.pdf";
            String filename = destinationFolder + "copyFields08.pdf";
            PdfDocument doc1 = new PdfDocument(new PdfReader(srcFilename1));
            PdfDocument doc2 = new PdfDocument(new PdfReader(srcFilename2));
            PdfDocument doc3 = new PdfDocument(new PdfReader(srcFilename3));
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(filename));
            pdfDoc.InitializeOutlines();
            PdfPageFormCopier formCopier = new PdfPageFormCopier();
            doc3.CopyPagesTo(1, doc3.GetNumberOfPages(), pdfDoc, formCopier);
            doc2.CopyPagesTo(1, doc2.GetNumberOfPages(), pdfDoc, formCopier);
            doc1.CopyPagesTo(1, doc1.GetNumberOfPages(), pdfDoc, formCopier);
            pdfDoc.Close();
            // comparing with cmp_copyFields01.pdf on purpose: result should be the same as in the first test
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(filename, sourceFolder + "cmp_copyFields01.pdf"
                , destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void CopyFieldsTest09() {
            String srcFilename = sourceFolder + "datasheet.pdf";
            String destFilename = destinationFolder + "copyFields09.pdf";
            PdfDocument destDoc = new PdfDocument(new PdfWriter(destFilename, new WriterProperties().UseSmartMode()));
            // copying the same page from the same document twice
            PdfPageFormCopier copier = new PdfPageFormCopier();
            for (int i = 0; i < 3; ++i) {
                PdfDocument srcDoc = new PdfDocument(new PdfReader(srcFilename));
                srcDoc.CopyPagesTo(1, 1, destDoc, copier);
                destDoc.FlushCopiedObjects(srcDoc);
                srcDoc.Close();
            }
            destDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destFilename, sourceFolder + "cmp_copyFields09.pdf"
                , destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void CopyFieldsTest10() {
            String srcFilename = sourceFolder + "datasheet.pdf";
            String destFilename = destinationFolder + "copyFields10.pdf";
            PdfDocument destDoc = new PdfDocument(new PdfWriter(destFilename, new WriterProperties().UseSmartMode()));
            // copying the same page from the same document twice
            for (int i = 0; i < 3; ++i) {
                PdfDocument srcDoc = new PdfDocument(new PdfReader(srcFilename));
                srcDoc.CopyPagesTo(1, 1, destDoc, new PdfPageFormCopier());
                destDoc.FlushCopiedObjects(srcDoc);
                srcDoc.Close();
            }
            destDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destFilename, sourceFolder + "cmp_copyFields10.pdf"
                , destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void CopyFieldsTest11() {
            String srcFilename1 = sourceFolder + "datasheet.pdf";
            String srcFilename2 = sourceFolder + "datasheet2.pdf";
            String destFilename = destinationFolder + "copyFields11.pdf";
            PdfDocument destDoc = new PdfDocument(new PdfWriter(destFilename, new WriterProperties()));
            PdfDocument srcDoc1 = new PdfDocument(new PdfReader(srcFilename1));
            srcDoc1.CopyPagesTo(1, 1, destDoc, new PdfPageFormCopier());
            destDoc.FlushCopiedObjects(srcDoc1);
            srcDoc1.Close();
            PdfDocument srcDoc2 = new PdfDocument(new PdfReader(srcFilename2));
            srcDoc2.CopyPagesTo(1, 1, destDoc, new PdfPageFormCopier());
            destDoc.FlushCopiedObjects(srcDoc2);
            srcDoc2.Close();
            destDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destFilename, sourceFolder + "cmp_copyFields11.pdf"
                , destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void CopyFieldsTest12() {
            String srcFilename1 = sourceFolder + "datasheet.pdf";
            String srcFilename2 = sourceFolder + "datasheet2.pdf";
            String destFilename = destinationFolder + "copyFields12.pdf";
            PdfDocument destDoc = new PdfDocument(new PdfWriter(destFilename, new WriterProperties().UseSmartMode()));
            PdfDocument srcDoc2 = new PdfDocument(new PdfReader(srcFilename2));
            srcDoc2.CopyPagesTo(1, 1, destDoc, new PdfPageFormCopier());
            destDoc.FlushCopiedObjects(srcDoc2);
            srcDoc2.Close();
            PdfDocument srcDoc1 = new PdfDocument(new PdfReader(srcFilename1));
            srcDoc1.CopyPagesTo(1, 1, destDoc, new PdfPageFormCopier());
            destDoc.FlushCopiedObjects(srcDoc1);
            srcDoc1.Close();
            destDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destFilename, sourceFolder + "cmp_copyFields12.pdf"
                , destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void CopyFieldsTest13() {
            String srcFilename = sourceFolder + "copyFields13.pdf";
            String destFilename = destinationFolder + "copyFields13.pdf";
            PdfDocument srcDoc = new PdfDocument(new PdfReader(srcFilename));
            PdfDocument destDoc = new PdfDocument(new PdfWriter(destFilename));
            PdfPageFormCopier pdfPageFormCopier = new PdfPageFormCopier();
            for (int i = 0; i < 1; ++i) {
                srcDoc.CopyPagesTo(1, 1, destDoc, pdfPageFormCopier);
            }
            PdfAcroForm acroForm = PdfFormCreator.GetAcroForm(destDoc, false);
            acroForm.GetField("text").SetValue("Text!");
            destDoc.Close();
            srcDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destFilename, sourceFolder + "cmp_copyFields13.pdf"
                , destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void CopyPagesWithInheritedResources() {
            String sourceFile = sourceFolder + "AnnotationSampleStandard.pdf";
            String destFile = destinationFolder + "AnnotationSampleStandard_copy.pdf";
            PdfDocument source = new PdfDocument(new PdfReader(sourceFile));
            PdfDocument target = new PdfDocument(new PdfWriter(destFile));
            target.InitializeOutlines();
            source.CopyPagesTo(1, source.GetNumberOfPages(), target, new PdfPageFormCopier());
            target.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destFile, sourceFolder + "cmp_AnnotationSampleStandard_copy.pdf"
                , destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void UnnamedFieldsHierarchyTest() {
            String srcFilename = sourceFolder + "unnamedFields.pdf";
            String destFilename = destinationFolder + "hierarchyTest.pdf";
            PdfDocument src = new PdfDocument(new PdfReader(srcFilename));
            PdfDocument merged = new PdfDocument(new PdfWriter(destFilename));
            src.CopyPagesTo(1, 1, merged, new PdfPageFormCopier());
            merged.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destFilename, sourceFolder + "cmp_unnamedFieldsHierarchyTest.pdf"
                , destinationFolder, "diff_"));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.DOCUMENT_ALREADY_HAS_FIELD, Count = 45)]
        public virtual void CopyAndEditTextFields() {
            String srcFileName = sourceFolder + "checkPdfFormCopy_Source.pdf";
            String destFilename = destinationFolder + "copyAndEditTextFields.pdf";
            String cmpFileName = sourceFolder + "cmp_copyAndEditTextFields.pdf";
            PdfDocument srcDoc = new PdfDocument(new PdfReader(srcFileName));
            PdfDocument destDoc = new PdfDocument(new PdfWriter(destFilename));
            PdfPageFormCopier pdfPageFormCopier = new PdfPageFormCopier();
            for (int i = 0; i < 4; i++) {
                srcDoc.CopyPagesTo(1, 1, destDoc, pdfPageFormCopier);
            }
            PdfAcroForm acroForm = PdfFormCreator.GetAcroForm(destDoc, false);
            acroForm.GetField("text_1").SetValue("text_1");
            acroForm.GetField("NumberField_text.2").SetValue("-100.00");
            acroForm.GetField("NumberField_text.2_1").SetValue("3.00");
            acroForm.GetField("text.3_1<!").SetValue("text.3_1<!");
            acroForm.GetField("text.4___#1+1").SetValue("CHANGEDtext");
            destDoc.Close();
            srcDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destFilename, cmpFileName, destinationFolder
                , "diff_"));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.DOCUMENT_ALREADY_HAS_FIELD, Count = 45)]
        public virtual void CopyAndEditCheckboxes() {
            String srcFileName = sourceFolder + "checkPdfFormCopy_Source.pdf";
            String destFilename = destinationFolder + "copyAndEditCheckboxes.pdf";
            String cmpFileName = sourceFolder + "cmp_copyAndEditCheckboxes.pdf";
            PdfDocument srcDoc = new PdfDocument(new PdfReader(srcFileName));
            PdfDocument destDoc = new PdfDocument(new PdfWriter(destFilename));
            PdfPageFormCopier pdfPageFormCopier = new PdfPageFormCopier();
            for (int i = 0; i < 4; i++) {
                srcDoc.CopyPagesTo(1, 1, destDoc, pdfPageFormCopier);
            }
            PdfAcroForm acroForm = PdfFormCreator.GetAcroForm(destDoc, false);
            acroForm.GetField("CheckBox_1").SetValue("On");
            acroForm.GetField("Check Box.2").SetValue("Off");
            acroForm.GetField("CheckBox4.1#1").SetValue("Off");
            destDoc.Close();
            srcDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destFilename, cmpFileName, destinationFolder
                , "diff_"));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.DOCUMENT_ALREADY_HAS_FIELD, Count = 45)]
        public virtual void CopyAndEditRadioButtons() {
            String srcFileName = sourceFolder + "checkPdfFormCopy_Source.pdf";
            String destFilename = destinationFolder + "copyAndEditRadioButtons.pdf";
            String cmpFileName = sourceFolder + "cmp_copyAndEditRadioButtons.pdf";
            PdfDocument srcDoc = new PdfDocument(new PdfReader(srcFileName));
            PdfDocument destDoc = new PdfDocument(new PdfWriter(destFilename));
            PdfPageFormCopier pdfPageFormCopier = new PdfPageFormCopier();
            for (int i = 0; i < 4; i++) {
                srcDoc.CopyPagesTo(1, 1, destDoc, pdfPageFormCopier);
            }
            PdfAcroForm acroForm = PdfFormCreator.GetAcroForm(destDoc, false);
            acroForm.GetField("Group.4").SetValue("Choice_3!<>3.3.3");
            destDoc.Close();
            srcDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destFilename, cmpFileName, destinationFolder
                , "diff_"));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.DOCUMENT_ALREADY_HAS_FIELD)]
        public virtual void MergeMergedFieldAndMergedFieldTest() {
            String srcFileName1 = sourceFolder + "fieldMergedWithWidget.pdf";
            String destFilename = destinationFolder + "mergeMergedFieldAndMergedFieldTest.pdf";
            String cmpFileName = sourceFolder + "cmp_mergeMergedFieldAndMergedFieldTest.pdf";
            using (PdfWriter writer = new PdfWriter(destFilename)) {
                using (PdfDocument resultPdfDocument = new PdfDocument(writer)) {
                    using (PdfReader reader1 = new PdfReader(srcFileName1)) {
                        using (PdfDocument sourceDoc1 = new PdfDocument(reader1)) {
                            PdfFormCreator.GetAcroForm(resultPdfDocument, true);
                            PdfPageFormCopier formCopier = new PdfPageFormCopier();
                            sourceDoc1.CopyPagesTo(1, sourceDoc1.GetNumberOfPages(), resultPdfDocument, formCopier);
                            sourceDoc1.CopyPagesTo(1, sourceDoc1.GetNumberOfPages(), resultPdfDocument, formCopier);
                        }
                    }
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destFilename, cmpFileName, destinationFolder
                , "diff_"));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.DOCUMENT_ALREADY_HAS_FIELD, Count = 1)]
        public virtual void MergeMergedFieldAndTwoWidgetsTest() {
            String srcFileName1 = sourceFolder + "fieldMergedWithWidget.pdf";
            String srcFileName2 = sourceFolder + "fieldTwoWidgets.pdf";
            String destFilename = destinationFolder + "mergeMergedFieldAndTwoWidgetsTest.pdf";
            String cmpFileName = sourceFolder + "cmp_mergeMergedFieldAndTwoWidgetsTest.pdf";
            using (PdfWriter writer = new PdfWriter(destFilename)) {
                using (PdfDocument resultPdfDocument = new PdfDocument(writer)) {
                    using (PdfReader reader1 = new PdfReader(srcFileName1)) {
                        using (PdfDocument sourceDoc1 = new PdfDocument(reader1)) {
                            using (PdfReader reader2 = new PdfReader(srcFileName2)) {
                                using (PdfDocument sourceDoc2 = new PdfDocument(reader2)) {
                                    PdfFormCreator.GetAcroForm(resultPdfDocument, true);
                                    PdfPageFormCopier formCopier = new PdfPageFormCopier();
                                    sourceDoc1.CopyPagesTo(1, sourceDoc1.GetNumberOfPages(), resultPdfDocument, formCopier);
                                    sourceDoc2.CopyPagesTo(1, sourceDoc2.GetNumberOfPages(), resultPdfDocument, formCopier);
                                }
                            }
                        }
                    }
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destFilename, cmpFileName, destinationFolder
                , "diff_"));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.DOCUMENT_ALREADY_HAS_FIELD)]
        public virtual void MergeTwoWidgetsAndMergedFieldTest() {
            String srcFileName1 = sourceFolder + "fieldMergedWithWidget.pdf";
            String srcFileName2 = sourceFolder + "fieldTwoWidgets.pdf";
            String destFilename = destinationFolder + "mergeTwoWidgetsAndMergedFieldTest.pdf";
            String cmpFileName = sourceFolder + "cmp_mergeTwoWidgetsAndMergedFieldTest.pdf";
            using (PdfWriter writer = new PdfWriter(destFilename)) {
                using (PdfDocument resultPdfDocument = new PdfDocument(writer)) {
                    using (PdfReader reader1 = new PdfReader(srcFileName1)) {
                        using (PdfDocument sourceDoc1 = new PdfDocument(reader1)) {
                            using (PdfReader reader2 = new PdfReader(srcFileName2)) {
                                using (PdfDocument sourceDoc2 = new PdfDocument(reader2)) {
                                    PdfFormCreator.GetAcroForm(resultPdfDocument, true);
                                    PdfPageFormCopier formCopier = new PdfPageFormCopier();
                                    sourceDoc2.CopyPagesTo(1, sourceDoc2.GetNumberOfPages(), resultPdfDocument, formCopier);
                                    sourceDoc1.CopyPagesTo(1, sourceDoc1.GetNumberOfPages(), resultPdfDocument, formCopier);
                                }
                            }
                        }
                    }
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destFilename, cmpFileName, destinationFolder
                , "diff_"));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.DOCUMENT_ALREADY_HAS_FIELD)]
        public virtual void MergeTwoWidgetsAndTwoWidgetsTest() {
            String srcFileName2 = sourceFolder + "fieldTwoWidgets.pdf";
            String destFilename = destinationFolder + "mergeTwoWidgetsAndTwoWidgetsTest.pdf";
            String cmpFileName = sourceFolder + "cmp_mergeTwoWidgetsAndTwoWidgetsTest.pdf";
            using (PdfWriter writer = new PdfWriter(destFilename)) {
                using (PdfDocument resultPdfDocument = new PdfDocument(writer)) {
                    using (PdfReader reader2 = new PdfReader(srcFileName2)) {
                        using (PdfDocument sourceDoc2 = new PdfDocument(reader2)) {
                            PdfFormCreator.GetAcroForm(resultPdfDocument, true);
                            PdfPageFormCopier formCopier = new PdfPageFormCopier();
                            sourceDoc2.CopyPagesTo(1, sourceDoc2.GetNumberOfPages(), resultPdfDocument, formCopier);
                            sourceDoc2.CopyPagesTo(1, sourceDoc2.GetNumberOfPages(), resultPdfDocument, formCopier);
                        }
                    }
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destFilename, cmpFileName, destinationFolder
                , "diff_"));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.DOCUMENT_ALREADY_HAS_FIELD, Count = 2)]
        public virtual void ComplexFieldsHierarchyTest() {
            String srcFileName = sourceFolder + "complexFieldsHierarchyTest.pdf";
            String destFilename = destinationFolder + "complexFieldsHierarchyTest.pdf";
            String cmpFileName = sourceFolder + "cmp_complexFieldsHierarchyTest.pdf";
            using (PdfDocument pdfDocMerged = new PdfDocument(new PdfReader(srcFileName), new PdfWriter(destFilename))
                ) {
                using (PdfDocument pdfDoc = new PdfDocument(new PdfReader(srcFileName))) {
                    pdfDoc.CopyPagesTo(1, pdfDoc.GetNumberOfPages(), pdfDocMerged, new PdfPageFormCopier());
                    pdfDoc.CopyPagesTo(1, pdfDoc.GetNumberOfPages(), pdfDocMerged, new PdfPageFormCopier());
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destFilename, cmpFileName, destinationFolder
                , "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void WidgetContainsNoTEntryTest() {
            String sourceFileName = sourceFolder + "fieldThreeWidgets.pdf";
            String destFileName = destinationFolder + "widgetContainsNoTEntryTest.pdf";
            String cmpFileName = sourceFolder + "cmp_widgetContainsNoTEntryTest.pdf";
            PdfDocument sourcePdfDocument = new PdfDocument(new PdfReader(sourceFileName));
            PdfDocument resultPdfDocument = new PdfDocument(new PdfWriter(destFileName));
            sourcePdfDocument.CopyPagesTo(1, sourcePdfDocument.GetNumberOfPages(), resultPdfDocument, new PdfPageFormCopier
                ());
            resultPdfDocument.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destFileName, cmpFileName, destinationFolder
                , "diff_"));
        }
    }
}
