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
using iText.Commons.Utils;
using iText.Kernel.Utils;
using iText.Test;

namespace iText.Kernel.Pdf {
    [NUnit.Framework.Category("IntegrationTest")]
    public class ReorderPagesTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/pdf/ReorderPagesTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/kernel/pdf/ReorderPagesTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.OneTimeTearDown]
        public static void AfterClass() {
            CompareTool.Cleanup(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void ReorderTaggedHasCommonStructElem01() {
            String inPath = sourceFolder + "taggedHasCommonStructElem.pdf";
            String outPath = destinationFolder + "reorderTaggedHasCommonStructElem01.pdf";
            String cmpPath = sourceFolder + "cmp_reorderTaggedHasCommonStructElem.pdf";
            PdfDocument pdf = new PdfDocument(new PdfReader(inPath), CompareTool.CreateTestPdfWriter(outPath));
            pdf.SetTagged();
            pdf.MovePage(2, 1);
            pdf.Close();
            Compare(outPath, cmpPath, destinationFolder, "diff_01");
        }

        [NUnit.Framework.Test]
        public virtual void ReorderTaggedHasCommonStructElem02() {
            String inPath = sourceFolder + "taggedHasCommonStructElem.pdf";
            String outPath = destinationFolder + "reorderTaggedHasCommonStructElem02.pdf";
            String cmpPath = sourceFolder + "cmp_reorderTaggedHasCommonStructElem.pdf";
            PdfDocument pdf = new PdfDocument(new PdfReader(inPath), CompareTool.CreateTestPdfWriter(outPath));
            pdf.MovePage(1, 3);
            pdf.Close();
            Compare(outPath, cmpPath, destinationFolder, "diff_02");
        }

        [NUnit.Framework.Test]
        public virtual void ReorderTaggedHasCommonStructElemBigger() {
            String inPath = sourceFolder + "taggedHasCommonStructElemBigger.pdf";
            String outPath = destinationFolder + "reorderTaggedHasCommonStructElemBigger.pdf";
            String cmpPath = sourceFolder + "cmp_reorderTaggedHasCommonStructElemBigger.pdf";
            PdfDocument pdf = new PdfDocument(new PdfReader(inPath), CompareTool.CreateTestPdfWriter(outPath));
            pdf.MovePage(2, 5);
            pdf.Close();
            Compare(outPath, cmpPath, destinationFolder, "diff_03");
        }

        [NUnit.Framework.Test]
        public virtual void CopyReorderTaggedHasCommonStructElem() {
            String inPath = sourceFolder + "taggedHasCommonStructElem.pdf";
            String outPath = destinationFolder + "copyReorderTaggedHasCommonStructElem.pdf";
            String cmpPath = sourceFolder + "cmp_copyReorderTaggedHasCommonStructElem.pdf";
            PdfDocument sourceDoc = new PdfDocument(new PdfReader(inPath));
            PdfDocument pdfDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(outPath));
            pdfDoc.SetTagged();
            sourceDoc.CopyPagesTo(JavaUtil.ArraysAsList(2, 1, 3), pdfDoc);
            sourceDoc.Close();
            pdfDoc.Close();
            Compare(outPath, cmpPath, destinationFolder, "diff_04");
        }

        private void Compare(String outPath, String cmpPath, String destinationFolder, String diffPrefix) {
            CompareTool compareTool = new CompareTool();
            String tagStructureErrors = compareTool.CompareTagStructures(outPath, cmpPath);
            String contentErrors = compareTool.CompareByContent(outPath, cmpPath, destinationFolder, diffPrefix);
            String resultMessage = "";
            if (tagStructureErrors != null) {
                resultMessage += tagStructureErrors + "\n";
            }
            if (contentErrors != null) {
                resultMessage += contentErrors + "\n";
            }
            NUnit.Framework.Assert.IsTrue(tagStructureErrors == null && contentErrors == null, resultMessage);
        }
    }
}
