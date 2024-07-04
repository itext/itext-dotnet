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
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Annot;
using iText.Kernel.Utils;
using iText.Test;

namespace iText.Kernel.Geom {
    [NUnit.Framework.Category("IntegrationTest")]
    public class PageSizeTest : ExtendedITextTest {
        public static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/kernel/geom/PageSizeTest/";

        public static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/geom/PageSizeTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.OneTimeTearDown]
        public static void AfterClass() {
            CompareTool.Cleanup(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void EmptyA9PageTest() {
            String outPdf = DESTINATION_FOLDER + "emptyA9Page.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_emptyA9Page.pdf";
            PdfDocument doc = new PdfDocument(CompareTool.CreateTestPdfWriter(outPdf));
            doc.AddNewPage(PageSize.A9);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER, "diff"
                ));
        }

        [NUnit.Framework.Test]
        public virtual void NotEmptyA9PageTest() {
            String outPdf = DESTINATION_FOLDER + "notEmptyA9Page.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_notEmptyA9Page.pdf";
            PdfDocument doc = new PdfDocument(CompareTool.CreateTestPdfWriter(outPdf));
            PdfPage page = doc.AddNewPage(PageSize.A9);
            PdfAnnotation annotation = new PdfFreeTextAnnotation(new Rectangle(50, 10, 50, 50), new PdfString("some content"
                ));
            page.AddAnnotation(annotation);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER, "diff"
                ));
        }

        [NUnit.Framework.Test]
        public virtual void AllATypePageSizesTest() {
            String outPdf = DESTINATION_FOLDER + "allATypePageSizes.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_allATypePageSizes.pdf";
            PageSize[] pageSizes = new PageSize[] { PageSize.A0, PageSize.A1, PageSize.A2, PageSize.A3, PageSize.A4, PageSize
                .A5, PageSize.A6, PageSize.A7, PageSize.A8, PageSize.A9, PageSize.A10 };
            PdfDocument doc = new PdfDocument(CompareTool.CreateTestPdfWriter(outPdf));
            foreach (PageSize pageSize in pageSizes) {
                doc.AddNewPage(pageSize);
            }
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER, "diff"
                ));
        }
    }
}
