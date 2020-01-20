/*
This file is part of the iText (R) project.
Copyright (c) 1998-2020 iText Group NV
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
using iText.Test.Attributes;

namespace iText.Forms {
    public class PdfChoiceFieldTest : ExtendedITextTest {
        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/forms/PdfChoiceFieldTest/";

        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/forms/PdfChoiceFieldTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(destinationFolder);
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

        [NUnit.Framework.Test]
        public virtual void ChoiceFieldsSetValueTest() {
            String srcPdf = sourceFolder + "choiceFieldsWithUnnecessaryIEntries.pdf";
            String outPdf = destinationFolder + "choiceFieldsSetValueTest.pdf";
            String cmpPdf = sourceFolder + "cmp_choiceFieldsSetValueTest.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(srcPdf), new PdfWriter(outPdf));
            PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDocument, false);
            form.GetField("First").SetValue("First");
            form.GetField("Second").SetValue("Second");
            pdfDocument.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, destinationFolder, "diff01_"
                ));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.LogMessageConstant.FIELD_VALUE_IS_NOT_CONTAINED_IN_OPT_ARRAY, Count = 2)]
        public virtual void MultiSelectByValueTest() {
            PdfDocument document = new PdfDocument(new PdfWriter(destinationFolder + "multiSelectByValueTest.pdf"));
            document.AddNewPage();
            PdfAcroForm form = PdfAcroForm.GetAcroForm(document, true);
            PdfFormField choice = PdfFormField.CreateList(document, new Rectangle(336, 666, 50, 80), "choice", "two", 
                new String[] { "one", "two", "three", "four" }, null, null).SetBorderColor(ColorConstants.BLACK);
            ((PdfChoiceFormField)choice).SetMultiSelect(true);
            ((PdfChoiceFormField)choice).SetListSelected(new String[] { "one", "three", "eins", "drei", null });
            form.AddField(choice);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + "multiSelectByValueTest.pdf"
                , sourceFolder + "cmp_multiSelectByValueTest.pdf", destinationFolder, "diff01_"));
        }

        [NUnit.Framework.Test]
        public virtual void MultiSelectByIndexOutOfBoundsTest() {
            PdfDocument document = new PdfDocument(new PdfReader(sourceFolder + "multiSelectTest.pdf"), new PdfWriter(
                destinationFolder + "multiSelectByIndexOutOfBoundsTest.pdf"));
            PdfAcroForm form = PdfAcroForm.GetAcroForm(document, false);
            PdfChoiceFormField field = (PdfChoiceFormField)form.GetField("choice");
            field.SetListSelected(new int[] { 5 });
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(destinationFolder + "multiSelectByIndexOutOfBoundsTest.pdf"
                , sourceFolder + "cmp_multiSelectByIndexOutOfBoundsTest.pdf", destinationFolder, "diff01_"));
        }
    }
}
