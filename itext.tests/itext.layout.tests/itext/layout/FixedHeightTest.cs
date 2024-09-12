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
using iText.Kernel.Colors;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Layout {
    [NUnit.Framework.Category("IntegrationTest")]
    public class FixedHeightTest : ExtendedITextTest {
        private static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/FixedHeightTest/";

        private static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/layout/FixedHeightTest/";

        private const String textByron = "When a man hath no freedom to fight for at home,\n" + "    Let him combat for that of his neighbours;\n"
             + "Let him think of the glories of Greece and of Rome,\n" + "    And get knocked on the head for his labours.\n"
             + "\n" + "To do good to Mankind is the chivalrous plan,\n" + "    And is always as nobly requited;\n"
             + "Then battle for Freedom wherever you can,\n" + "    And, if not shot or hanged, you'll get knighted.";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(destinationFolder);
        }

        [LogMessage(iText.IO.Logs.IoLogMessageConstant.CLIP_ELEMENT, Count = 1)]
        // TODO DEVSIX-1977 partial layout result due to fixed height should not contain not layouted kids
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.OCCUPIED_AREA_HAS_NOT_BEEN_INITIALIZED, Count = 6)]
        [NUnit.Framework.Test]
        public virtual void DivWithParagraphsAndFixedPositionTest() {
            String outFileName = destinationFolder + "blockWithLimitedHeightAndFixedPositionTest.pdf";
            String cmpFileName = sourceFolder + "cmp_blockWithLimitedHeightAndFixedPositionTest.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDocument);
            Div block = new Div();
            block.SetBorder(new SolidBorder(ColorConstants.BLUE, 1));
            block.SetHeight(120);
            foreach (String line in iText.Commons.Utils.StringUtil.Split(textByron, "\n")) {
                Paragraph p = new Paragraph();
                p.Add(new Text(line));
                p.SetBorder(new SolidBorder(0.5f));
                block.Add(p);
            }
            block.SetFixedPosition(100, 600, 300);
            doc.Add(block);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                ));
        }

        [LogMessage(iText.IO.Logs.IoLogMessageConstant.CLIP_ELEMENT, Count = 1)]
        // TODO DEVSIX-1977 partial layout result due to fixed height should not contain not layouted kids
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.OCCUPIED_AREA_HAS_NOT_BEEN_INITIALIZED, Count = 2)]
        [NUnit.Framework.Test]
        public virtual void ListWithFixedPositionTest() {
            String outFileName = destinationFolder + "listWithFixedPositionTest.pdf";
            String cmpFileName = sourceFolder + "cmp_listWithFixedPositionTest.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            Document doc = new Document(pdfDocument);
            List list = new List();
            list.SetBorder(new SolidBorder(ColorConstants.BLUE, 1));
            list.SetHeight(120);
            foreach (String line in iText.Commons.Utils.StringUtil.Split(textByron, "\n")) {
                list.Add(line);
            }
            list.SetFixedPosition(100, 600, 300);
            doc.Add(list);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                ));
        }
    }
}
