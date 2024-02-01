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
using iText.IO.Font;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Layout;
using iText.Layout.Element;
using iText.Test;

namespace iText.Pdfa {
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfA1LayoutListTest : ExtendedITextTest {
        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/pdfa/PdfA1LayoutListTest/";

        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfa/";

        public static readonly String cmpFolder = sourceFolder + "cmp/PdfA1LayoutListTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void ListTest01() {
            String outPdf = destinationFolder + "pdfA1b_listTest01.pdf";
            String cmpPdf = cmpFolder + "cmp_pdfA1b_listTest01.pdf";
            Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode.Open, FileAccess.Read);
            PdfOutputIntent outputIntent = new PdfOutputIntent("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1"
                , @is);
            PdfADocument pdfDocument = new PdfADocument(new PdfWriter(outPdf), PdfAConformanceLevel.PDF_A_1B, outputIntent
                );
            Document doc = new Document(pdfDocument);
            pdfDocument.SetTagged();
            PdfFont textfont = PdfFontFactory.CreateFont(sourceFolder + "FreeSans.ttf", PdfEncodings.WINANSI, PdfFontFactory.EmbeddingStrategy
                .FORCE_EMBEDDED);
            textfont.SetSubset(true);
            List list = new List();
            ListItem listItem = new ListItem();
            listItem.Add(new Paragraph().Add("foobar"));
            list.Add(listItem);
            listItem.SetFont(textfont);
            doc.Add(list);
            doc.Close();
            String result = new CompareTool().CompareByContent(outPdf, cmpPdf, destinationFolder, "diff_");
            if (result != null) {
                NUnit.Framework.Assert.Fail(result);
            }
        }
    }
}
