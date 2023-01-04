/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
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

        [NUnit.Framework.Test]
        public virtual void DocumentInfoCreatePdf20() {
            String outFile = destinationFolder + "test01.pdf";
            String cmpFile = sourceFolder + "cmp_test01.pdf";
            PdfDocument document = new PdfDocument(new PdfWriter(outFile, new WriterProperties().SetPdfVersion(PdfVersion
                .PDF_2_0)));
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
            PdfDocument document = new PdfDocument(new PdfReader(inputFile), new PdfWriter(outFile, new WriterProperties
                ().SetPdfVersion(PdfVersion.PDF_2_0)));
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
            PdfDocument document = new PdfDocument(new PdfReader(inputFile), new PdfWriter(outFile, new WriterProperties
                ().SetPdfVersion(PdfVersion.PDF_2_0)), new StampingProperties().UseAppendMode());
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
            PdfDocument document = new PdfDocument(new PdfReader(inputFile), new PdfWriter(outFile), new StampingProperties
                ().UseAppendMode());
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
            PdfDocument document = new PdfDocument(new PdfReader(inputFile), new PdfWriter(outFile), new StampingProperties
                ());
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
