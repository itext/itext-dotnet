/*
This file is part of the iText (R) project.
Copyright (c) 1998-2019 iText Group NV
Authors: iText Software.

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
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Test;

namespace iText.Forms {
    public class PdfChoiceFieldTest {
        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/forms/PdfChoiceFieldTest/";

        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/forms/PdfChoiceFieldTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            ITextTest.CreateOrClearDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void ChoiceFieldsWithUnicodeTest() {
            String outPdf = destinationFolder + "choiceFieldsWithUnicodeTest.pdf";
            String cmpPdf = sourceFolder + "cmp_choiceFieldsWithUnicodeTest.pdf";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(outPdf));
            PdfFont font = PdfFontFactory.CreateFont(sourceFolder + "NotoSansCJKjp-Bold.otf", "Identity-H");
            font.SetSubset(false);
            PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
            pdfDoc.AddNewPage();
            // 规
            form.AddField(PdfFormField.CreateComboBox(pdfDoc, new Rectangle(36, 666, 40, 80), "combo1", "\u89c4", new 
                String[] { "\u89c4", "\u89c9" }, font, null).SetBorderColor(ColorConstants.BLACK));
            // 觉
            form.AddField(PdfFormField.CreateComboBox(pdfDoc, new Rectangle(136, 666, 40, 80), "combo2", "\u89c4", new 
                String[] { "\u89c4", "\u89c9" }, font, null).SetValue("\u89c9").SetBorderColor(ColorConstants.BLACK));
            // 规
            form.AddField(PdfFormField.CreateList(pdfDoc, new Rectangle(236, 666, 50, 80), "list1", "\u89c4", new String
                [] { "\u89c4", "\u89c9" }, font, null).SetBorderColor(ColorConstants.BLACK));
            // 觉
            form.AddField(PdfFormField.CreateList(pdfDoc, new Rectangle(336, 666, 50, 80), "list2", "\u89c4", new String
                [] { "\u89c4", "\u89c9" }, font, null).SetValue("\u89c9").SetBorderColor(ColorConstants.BLACK));
            pdfDoc.Close();
            CompareTool compareTool = new CompareTool();
            String errorMessage = compareTool.CompareByContent(outPdf, cmpPdf, destinationFolder, "diff_");
            if (errorMessage != null) {
                NUnit.Framework.Assert.Fail(errorMessage);
            }
        }
    }
}
