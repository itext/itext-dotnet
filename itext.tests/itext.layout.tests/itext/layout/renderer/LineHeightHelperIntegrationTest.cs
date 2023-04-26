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
using iText.IO.Font.Constants;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Test;

namespace iText.Layout.Renderer {
    [NUnit.Framework.Category("IntegrationTest")]
    public class LineHeightHelperIntegrationTest : ExtendedITextTest {
        private static readonly String CMP = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/LineHeightHelperIntegrationTest/";

        private static readonly String DESTINATION = NUnit.Framework.TestContext.CurrentContext.TestDirectory + "/test/itext/layout/LineHeightHelperTest/";

        private const String TEXT_SAMPLE = "Effects present letters inquiry no an removed or friends. " + "Desire behind latter me though in. Supposing shameless am he engrossed up additions. "
             + "My possible peculiar together to. Desire so better am cannot he up before points. " + "Remember mistaken opinions it pleasure of debating. "
             + "Court front maids forty if aware their at. Chicken use are pressed removed.";

        [NUnit.Framework.OneTimeSetUp]
        public static void CreateDestFolder() {
            CreateDestinationFolder(DESTINATION);
        }

        [NUnit.Framework.Test]
        public virtual void CourierTest() {
            String name = "courierTest.pdf";
            String cmpPdf = CMP + "cmp_" + name;
            String outPdf = DESTINATION + name;
            TestFont(PdfFontFactory.CreateFont(StandardFonts.COURIER), outPdf);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION));
        }

        [NUnit.Framework.Test]
        public virtual void HelveticaTest() {
            String name = "helveticaTest.pdf";
            String cmpPdf = CMP + "cmp_" + name;
            String outPdf = DESTINATION + name;
            TestFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA), outPdf);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION));
        }

        [NUnit.Framework.Test]
        public virtual void TimesRomanTest() {
            String name = "timesRomanTest.pdf";
            String cmpPdf = CMP + "cmp_" + name;
            String outPdf = DESTINATION + name;
            TestFont(PdfFontFactory.CreateFont(StandardFonts.TIMES_ROMAN), outPdf);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION));
        }

        private void TestFont(PdfFont font, String outPdf) {
            Document document = new Document(new PdfDocument(new PdfWriter(outPdf)));
            document.SetFont(font);
            Paragraph paragraph = new Paragraph(TEXT_SAMPLE);
            paragraph.SetProperty(Property.RENDERING_MODE, RenderingMode.HTML_MODE);
            document.Add(paragraph);
            document.Close();
        }
    }
}
