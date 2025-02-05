/*
This file is part of the iText (R) project.
Copyright (c) 1998-2025 Apryse Group NV
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
using iText.Kernel.Utils;
using iText.Test;

namespace iText.Kernel.Pdf {
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfDocumentInfoTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/pdf/PdfDocumentInfoTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/kernel/pdf/PdfDocumentInfoTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.OneTimeTearDown]
        public static void AfterClass() {
            CompareTool.Cleanup(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void DocumentInfoCreatePdf20() {
            String outFile = destinationFolder + "test01.pdf";
            String cmpFile = sourceFolder + "cmp_test01.pdf";
            PdfDocument document = new PdfDocument(CompareTool.CreateTestPdfWriter(outFile, new WriterProperties().SetPdfVersion
                (PdfVersion.PDF_2_0)));
            document.AddNewPage();
            document.GetDocumentInfo().SetAuthor("Alexey");
            document.Close();
            CompareTool ct = new CompareTool();
            NUnit.Framework.Assert.IsNull(ct.CompareByContent(outFile, cmpFile, destinationFolder, "diff_"));
            NUnit.Framework.Assert.IsNull(ct.CompareDocumentInfo(outFile, cmpFile));
            NUnit.Framework.Assert.IsNull(ct.CompareXmp(outFile, cmpFile, true));
        }

        [NUnit.Framework.Test]
        public virtual void DocumentInfoTransformPdf17ToPdf20() {
            String inputFile = sourceFolder + "metadata_pdf.pdf";
            String outFile = destinationFolder + "metadata_pdf_20.pdf";
            String cmpFile = sourceFolder + "cmp_metadata_pdf_20.pdf";
            PdfDocument document = new PdfDocument(new PdfReader(inputFile), CompareTool.CreateTestPdfWriter(outFile, 
                new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0)));
            document.Close();
            CompareTool ct = new CompareTool();
            NUnit.Framework.Assert.IsNull(ct.CompareByContent(outFile, cmpFile, destinationFolder, "diff_"));
            NUnit.Framework.Assert.IsNull(ct.CompareDocumentInfo(outFile, cmpFile));
            NUnit.Framework.Assert.IsNull(ct.CompareXmp(outFile, cmpFile, true));
        }

        [NUnit.Framework.Test]
        public virtual void ChangeDocumentVersionAndInfoInAppendMode() {
            String inputFile = sourceFolder + "metadata_pdf.pdf";
            String outFile = destinationFolder + "metadata_pdf_20_append.pdf";
            String cmpFile = sourceFolder + "cmp_metadata_pdf_20_append.pdf";
            PdfDocument document = new PdfDocument(new PdfReader(inputFile), CompareTool.CreateTestPdfWriter(outFile, 
                new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0)), new StampingProperties().UseAppendMode());
            document.GetDocumentInfo().SetAuthor("Alexey Subach");
            document.Close();
            CompareTool ct = new CompareTool();
            NUnit.Framework.Assert.IsNull(ct.CompareByContent(outFile, cmpFile, destinationFolder, "diff_"));
            NUnit.Framework.Assert.IsNull(ct.CompareDocumentInfo(outFile, cmpFile));
            NUnit.Framework.Assert.IsNull(ct.CompareXmp(outFile, cmpFile, true));
        }

        [NUnit.Framework.Test]
        public virtual void ReadInfoFromMetadata() {
            String inputFile = sourceFolder + "cmp_metadata_pdf_20.pdf";
            PdfDocument document = new PdfDocument(new PdfReader(inputFile));
            String author = document.GetDocumentInfo().GetAuthor();
            String subject = document.GetDocumentInfo().GetSubject();
            String title = document.GetDocumentInfo().GetTitle();
            document.Close();
            NUnit.Framework.Assert.AreEqual("Bruno Lowagie", author, "Author");
            NUnit.Framework.Assert.AreEqual("Hello World example", title, "Title");
            NUnit.Framework.Assert.AreEqual("This example shows how to add metadata", subject, "Subject");
        }

        [NUnit.Framework.Test]
        public virtual void ChangeMetadataInAppendMode() {
            String inputFile = sourceFolder + "cmp_metadata_pdf_20.pdf";
            String outFile = destinationFolder + "metadata_pdf_20_changed_append.pdf";
            String cmpFile = sourceFolder + "cmp_metadata_pdf_20_changed_append.pdf";
            PdfDocument document = new PdfDocument(new PdfReader(inputFile), CompareTool.CreateTestPdfWriter(outFile), 
                new StampingProperties().UseAppendMode());
            document.GetDocumentInfo().SetAuthor("Alexey Subach");
            document.Close();
            CompareTool ct = new CompareTool();
            NUnit.Framework.Assert.IsNull(ct.CompareByContent(outFile, cmpFile, destinationFolder, "diff_"));
            NUnit.Framework.Assert.IsNull(ct.CompareDocumentInfo(outFile, cmpFile));
            NUnit.Framework.Assert.IsNull(ct.CompareXmp(outFile, cmpFile, true));
        }

        [NUnit.Framework.Test]
        public virtual void SimpleStampingMetadataLeaveUnchanged() {
            String inputFile = sourceFolder + "cmp_metadata_pdf_20_changed_append.pdf";
            String outFile = destinationFolder + "metadata_pdf_20_unchanged_stamper.pdf";
            String cmpFile = sourceFolder + "cmp_metadata_pdf_20_unchanged_append.pdf";
            PdfDocument document = new PdfDocument(new PdfReader(inputFile), CompareTool.CreateTestPdfWriter(outFile), 
                new StampingProperties());
            String author = document.GetDocumentInfo().GetAuthor();
            document.Close();
            NUnit.Framework.Assert.AreEqual("Bruno Lowagie; Alexey Subach", author, "Author");
            CompareTool ct = new CompareTool();
            NUnit.Framework.Assert.IsNull(ct.CompareByContent(outFile, cmpFile, destinationFolder, "diff_"));
            NUnit.Framework.Assert.IsNull(ct.CompareDocumentInfo(outFile, cmpFile));
            NUnit.Framework.Assert.IsNull(ct.CompareXmp(outFile, cmpFile, true));
        }
    }
}
