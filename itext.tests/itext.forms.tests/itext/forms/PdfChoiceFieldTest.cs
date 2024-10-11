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
using iText.Forms.Fields;
using iText.IO.Font;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Forms {
    [NUnit.Framework.Category("IntegrationTest")]
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
            PdfAcroForm form = PdfFormCreator.GetAcroForm(pdfDoc, true);
            pdfDoc.AddNewPage();
            // 规
            PdfFormField field = new ChoiceFormFieldBuilder(pdfDoc, "combo1").SetWidgetRectangle(new Rectangle(36, 666
                , 40, 80)).SetOptions(new String[] { "\u89c4", "\u89c9" }).SetConformance(null).CreateComboBox().SetValue
                ("\u89c4");
            field.SetFont(font);
            field.GetFirstFormAnnotation().SetBorderColor(ColorConstants.BLACK);
            form.AddField(field);
            // 觉
            field = new ChoiceFormFieldBuilder(pdfDoc, "combo2").SetWidgetRectangle(new Rectangle(136, 666, 40, 80)).SetOptions
                (new String[] { "\u89c4", "\u89c9" }).SetConformance(null).CreateComboBox();
            field.SetValue("\u89c4").SetFont(font);
            field.SetValue("\u89c9");
            field.GetFirstFormAnnotation().SetBorderColor(ColorConstants.BLACK);
            form.AddField(field);
            // 规
            field = new ChoiceFormFieldBuilder(pdfDoc, "list1").SetWidgetRectangle(new Rectangle(236, 666, 50, 80)).SetOptions
                (new String[] { "\u89c4", "\u89c9" }).SetConformance(null).CreateList().SetValue("\u89c4");
            field.SetFont(font);
            field.GetFirstFormAnnotation().SetBorderColor(ColorConstants.BLACK);
            form.AddField(field);
            // 觉
            field = new ChoiceFormFieldBuilder(pdfDoc, "list2").SetWidgetRectangle(new Rectangle(336, 666, 50, 80)).SetOptions
                (new String[] { "\u89c4", "\u89c9" }).SetConformance(null).CreateList();
            field.SetValue("\u89c4").SetFont(font);
            field.SetValue("\u89c9");
            field.GetFirstFormAnnotation().SetBorderColor(ColorConstants.BLACK);
            form.AddField(field);
            pdfDoc.Close();
            CompareTool compareTool = new CompareTool();
            String errorMessage = compareTool.CompareByContent(outPdf, cmpPdf, destinationFolder);
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
            PdfAcroForm form = PdfFormCreator.GetAcroForm(pdfDocument, false);
            PdfFormField firstField = form.GetField("First");
            PdfFormField secondField = form.GetField("Second");
            firstField.SetValue("First");
            secondField.SetValue("Second");
            PdfArray indicesFirst = ((PdfChoiceFormField)firstField).GetIndices();
            PdfArray indicesSecond = ((PdfChoiceFormField)secondField).GetIndices();
            PdfArray expectedIndicesFirst = new PdfArray(new int[] { 1 });
            PdfArray expectedIndicesSecond = new PdfArray(new int[] { 2 });
            CompareTool compareTool = new CompareTool();
            NUnit.Framework.Assert.IsTrue(compareTool.CompareArrays(indicesFirst, expectedIndicesFirst));
            NUnit.Framework.Assert.IsTrue(compareTool.CompareArrays(indicesSecond, expectedIndicesSecond));
            pdfDocument.Close();
            NUnit.Framework.Assert.IsNull(compareTool.CompareByContent(outPdf, cmpPdf, destinationFolder));
        }

        [NUnit.Framework.Test]
        public virtual void ComboNoHighlightCenteredTextOfChosenFirstItemTest() {
            String srcPdf = sourceFolder + "comboNoHighlightCenteredTextOfChosenFirstItemTest.pdf";
            String outPdf = destinationFolder + "comboNoHighlightCenteredTextOfChosenFirstItemTest.pdf";
            String cmpPdf = sourceFolder + "cmp_comboNoHighlightCenteredTextOfChosenFirstItemTest.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(srcPdf), new PdfWriter(outPdf));
            PdfAcroForm form = PdfFormCreator.GetAcroForm(pdfDocument, false);
            form.GetField("First").SetValue("Default");
            // flattening is only used for the sake of ease to see what appearance is generated by iText
            form.FlattenFields();
            pdfDocument.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, destinationFolder));
        }

        [NUnit.Framework.Test]
        public virtual void NoWarningOnValueNotOfOptComboEditTest() {
            String srcPdf = sourceFolder + "noWarningOnValueNotOfOptComboEditTest.pdf";
            String outPdf = destinationFolder + "noWarningOnValueNotOfOptComboEditTest.pdf";
            String cmpPdf = sourceFolder + "cmp_noWarningOnValueNotOfOptComboEditTest.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(srcPdf), new PdfWriter(outPdf));
            PdfAcroForm form = PdfFormCreator.GetAcroForm(pdfDocument, false);
            form.GetField("First").SetValue("Value not of /Opt array");
            pdfDocument.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, destinationFolder));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.FIELD_VALUE_IS_NOT_CONTAINED_IN_OPT_ARRAY, Count = 2)]
        public virtual void MultiSelectByValueTest() {
            String outPdf = destinationFolder + "multiSelectByValueTest.pdf";
            String cmpPdf = sourceFolder + "cmp_multiSelectByValueTest.pdf";
            PdfDocument document = new PdfDocument(new PdfWriter(outPdf));
            document.AddNewPage();
            PdfAcroForm form = PdfFormCreator.GetAcroForm(document, true);
            PdfChoiceFormField choice = (PdfChoiceFormField)new ChoiceFormFieldBuilder(document, "choice").SetWidgetRectangle
                (new Rectangle(336, 666, 50, 80)).SetOptions(new String[] { "one", "two", "three", "four" }).SetConformance
                (null).CreateList().SetValue("two").SetFont(null);
            choice.GetFirstFormAnnotation().SetBorderColor(ColorConstants.BLACK);
            choice.SetMultiSelect(true);
            choice.SetListSelected(new String[] { "one", "three", "eins", "drei" });
            NUnit.Framework.Assert.AreEqual(new int[] { 0, 2 }, choice.GetIndices().ToIntArray());
            PdfArray values = (PdfArray)choice.GetValue();
            String[] valuesAsStrings = new String[values.Size()];
            for (int i = 0; i < values.Size(); i++) {
                valuesAsStrings[i] = values.GetAsString(i).ToUnicodeString();
            }
            NUnit.Framework.Assert.AreEqual(new String[] { "one", "three", "eins", "drei" }, valuesAsStrings);
            form.AddField(choice);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, destinationFolder));
        }

        [NUnit.Framework.Test]
        public virtual void CorruptedOptAndValueSetToNullTest() {
            String srcPdf = sourceFolder + "corruptedOptAndValueSetToNullTest.pdf";
            String outPdf = destinationFolder + "corruptedOptAndValueSetToNullTest.pdf";
            String cmpPdf = sourceFolder + "cmp_corruptedOptAndValueSetToNullTest.pdf";
            PdfDocument document = new PdfDocument(new PdfReader(srcPdf), new PdfWriter(outPdf));
            PdfAcroForm form = PdfFormCreator.GetAcroForm(document, false);
            PdfChoiceFormField choice = (PdfChoiceFormField)form.GetField("choice");
            choice.SetListSelected(new String[] { null, "three" });
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, destinationFolder));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.FIELD_VALUE_IS_NOT_CONTAINED_IN_OPT_ARRAY)]
        public virtual void MultiSelectByValueRemoveIKeyTest() {
            String srcPdf = sourceFolder + "listWithPreselectedValue.pdf";
            String outPdf = destinationFolder + "selectByValueRemoveIKeyTest.pdf";
            String cmpPdf = sourceFolder + "cmp_selectByValueRemoveIKeyTest.pdf";
            String value = "zwei";
            PdfDocument document = new PdfDocument(new PdfReader(srcPdf), new PdfWriter(outPdf));
            document.AddNewPage();
            PdfAcroForm form = PdfFormCreator.GetAcroForm(document, true);
            PdfFormField field = form.GetField("choice");
            field.SetValue(value);
            NUnit.Framework.Assert.IsNull(field.GetPdfObject().Get(PdfName.I));
            CompareTool compareTool = new CompareTool();
            NUnit.Framework.Assert.IsTrue(compareTool.CompareStrings(new PdfString(value), field.GetPdfObject().GetAsString
                (PdfName.V)));
            document.Close();
            NUnit.Framework.Assert.IsNull(compareTool.CompareByContent(outPdf, cmpPdf, destinationFolder));
        }

        [NUnit.Framework.Test]
        public virtual void MultiSelectByIndexOutOfBoundsTest() {
            String srcPdf = sourceFolder + "multiSelectTest.pdf";
            String outPdf = destinationFolder + "multiSelectByIndexOutOfBoundsTest.pdf";
            String cmpPdf = sourceFolder + "cmp_multiSelectByIndexOutOfBoundsTest.pdf";
            PdfDocument document = new PdfDocument(new PdfReader(srcPdf), new PdfWriter(outPdf));
            PdfAcroForm form = PdfFormCreator.GetAcroForm(document, false);
            PdfChoiceFormField field = (PdfChoiceFormField)form.GetField("choice");
            field.SetListSelected(new int[] { 5 });
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, destinationFolder));
        }

        [NUnit.Framework.Test]
        public virtual void NotInstanceOfPdfChoiceFormFieldTest() {
            String outPdf = destinationFolder + "notInstanceOfPdfChoiceFormFieldTest.pdf";
            String cmpPdf = sourceFolder + "cmp_notInstanceOfPdfChoiceFormFieldTest.pdf";
            PdfDocument document = new PdfDocument(new PdfWriter(outPdf));
            PdfPage page = document.AddNewPage();
            PdfAcroForm form = PdfFormCreator.GetAcroForm(document, true);
            PdfDictionary fieldDictionary = new PdfDictionary();
            fieldDictionary.Put(PdfName.FT, PdfName.Ch);
            PdfArray opt = new PdfArray();
            opt.Add(new PdfString("one", PdfEncodings.UNICODE_BIG));
            opt.Add(new PdfString("two", PdfEncodings.UNICODE_BIG));
            opt.Add(new PdfString("three", PdfEncodings.UNICODE_BIG));
            opt.Add(new PdfString("four", PdfEncodings.UNICODE_BIG));
            fieldDictionary.Put(PdfName.Opt, opt);
            fieldDictionary.Put(PdfName.P, page.GetPdfObject().GetIndirectReference());
            fieldDictionary.Put(PdfName.Rect, new PdfArray(new int[] { 330, 660, 380, 740 }));
            fieldDictionary.Put(PdfName.Subtype, PdfName.Widget);
            fieldDictionary.Put(PdfName.T, new PdfString("choice", PdfEncodings.UNICODE_BIG));
            fieldDictionary.MakeIndirect(document);
            PdfFormField field = PdfFormCreator.CreateFormField(fieldDictionary);
            field.SetValue("two");
            form.AddField(field);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, destinationFolder));
        }

        [NUnit.Framework.Test]
        public virtual void TopIndexTest() {
            String srcPdf = sourceFolder + "choiceFieldNotFittingTest.pdf";
            String outPdf = destinationFolder + "topIndexTest.pdf";
            String cmpPdf = sourceFolder + "cmp_topIndexTest.pdf";
            PdfDocument document = new PdfDocument(new PdfReader(srcPdf), new PdfWriter(outPdf));
            PdfAcroForm form = PdfFormCreator.GetAcroForm(document, false);
            PdfChoiceFormField field = (PdfChoiceFormField)form.GetField("choice");
            field.SetListSelected(new String[] { "seven" });
            int topIndex = field.GetIndices().GetAsNumber(0).IntValue();
            field.SetTopIndex(topIndex);
            document.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, destinationFolder));
        }

        [NUnit.Framework.Test]
        public virtual void LongOptionWrappedIntoTwoLinesTest() {
            // TODO DEVSIX-4480 iText wraps the text into more than one line when generating listbox appearance
            String outFileName = destinationFolder + "longOptionWrappedIntoTwoLinesTest.pdf";
            String cmpFileName = sourceFolder + "cmp_longOptionWrappedIntoTwoLinesTest.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFileName));
            PdfAcroForm form = PdfFormCreator.GetAcroForm(pdfDocument, true);
            String shortOption = "Short option";
            String longOption = "Long long long long long long long option";
            String[] options = new String[] { shortOption, longOption };
            Rectangle rect = new Rectangle(50, 650, 100, 100);
            PdfChoiceFormField choice = new ChoiceFormFieldBuilder(pdfDocument, "List").SetWidgetRectangle(rect).SetOptions
                (options).CreateList();
            choice.SetValue("Short option", true);
            form.AddField(choice);
            pdfDocument.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                ));
        }
    }
}
