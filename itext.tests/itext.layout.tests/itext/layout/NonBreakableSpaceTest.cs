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
using iText.Kernel.Colors;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Test;

namespace iText.Layout {
    [NUnit.Framework.Category("IntegrationTest")]
    public class NonBreakableSpaceTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/NonBreakableSpaceTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/layout/NonBreakableSpaceTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void SimpleParagraphTest() {
            String outFileName = destinationFolder + "simpleParagraphTest.pdf";
            String cmpFileName = sourceFolder + "cmp_simpleParagraphTest.pdf";
            String diffPrefix = "diff_simpleParagraphTest_";
            Document document = new Document(new PdfDocument(new PdfWriter(outFileName)));
            document.Add(new Paragraph("aaa bbb\u00a0ccccccccccc").SetWidth(100).SetBorder(new SolidBorder(ColorConstants
                .RED, 10)));
            document.Add(new Paragraph("aaa bbb ccccccccccc").SetWidth(100).SetBorder(new SolidBorder(ColorConstants.GREEN
                , 10)));
            document.Add(new Paragraph("aaaaaaa\u00a0bbbbbbbbbbb").SetWidth(100).SetBorder(new SolidBorder(ColorConstants
                .BLUE, 10)));
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , diffPrefix));
        }

        [NUnit.Framework.Test]
        public virtual void ConsecutiveSpacesTest() {
            String outFileName = destinationFolder + "consecutiveSpacesTest.pdf";
            String cmpFileName = sourceFolder + "cmp_consecutiveSpacesTest.pdf";
            String diffPrefix = "diff_consecutiveSpacesTest_";
            Document document = new Document(new PdfDocument(new PdfWriter(outFileName)));
            document.Add(new Paragraph("aaa\u00a0\u00a0\u00a0bbb").SetWidth(100).SetBorder(new SolidBorder(ColorConstants
                .RED, 10)));
            document.Add(new Paragraph("aaa\u00a0bbb").SetWidth(100).SetBorder(new SolidBorder(ColorConstants.GREEN, 10
                )));
            document.Add(new Paragraph("aaa   bbb").SetWidth(100).SetBorder(new SolidBorder(ColorConstants.BLUE, 10)));
            document.Add(new Paragraph("aaa bbb").SetWidth(100).SetBorder(new SolidBorder(ColorConstants.BLACK, 10)));
            Paragraph p = new Paragraph();
            p.Add("aaa\u00a0\u00a0\u00a0bbb").Add("ccc   ddd");
            document.Add(p);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , diffPrefix));
        }
    }
}
