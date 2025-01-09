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
using System.Collections.Generic;
using System.IO;
using iText.Commons.Utils;
using iText.Forms;
using iText.Forms.Fields;
using iText.Forms.Fields.Properties;
using iText.Forms.Form.Element;
using iText.IO.Font;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Annot;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Xobject;
using iText.Kernel.Utils;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Layout.Renderer;
using iText.Test;
using iText.Test.Attributes;
using iText.Test.Pdfa;

namespace iText.Pdfa {
    // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf\a validation on Android)
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfAFormFieldTest : ExtendedITextTest {
        public static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfa/";

        public static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/pdfa/PdfAFormFieldTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void PdfAButtonFieldTest() {
            PdfDocument pdf;
            Stream @is = FileUtil.GetInputStreamForFile(SOURCE_FOLDER + "sRGB Color Space Profile.icm");
            String file = "pdfAButtonField.pdf";
            String filename = DESTINATION_FOLDER + file;
            pdf = new PdfADocument(new PdfWriter(FileUtil.GetFileOutputStream(filename)), PdfAConformance.PDF_A_1B, new 
                PdfOutputIntent("Custom", "", "http://www.color.org", "sRGB ICC preference", @is));
            PageSize pageSize = PageSize.LETTER;
            Document doc = new Document(pdf, pageSize);
            PdfFontFactory.Register(SOURCE_FOLDER + "FreeSans.ttf", SOURCE_FOLDER + "FreeSans.ttf");
            PdfFont font = PdfFontFactory.CreateFont(SOURCE_FOLDER + "FreeSans.ttf", PdfFontFactory.EmbeddingStrategy.
                PREFER_EMBEDDED);
            PdfButtonFormField group = new RadioFormFieldBuilder(pdf, "group").SetConformance(PdfConformance.PDF_A_1B)
                .CreateRadioGroup();
            group.SetValue("");
            group.SetReadOnly(true);
            Paragraph p = new Paragraph();
            Text t = new Text("supported");
            t.SetFont(font);
            p.Add(t);
            Image ph = new Image(new PdfFormXObject(new Rectangle(10, 10)));
            Paragraph pc = new Paragraph().Add(ph);
            PdfAFormFieldTest.PdfAButtonFieldTestRenderer r = new PdfAFormFieldTest.PdfAButtonFieldTestRenderer(pc, group
                , "v1");
            pc.SetNextRenderer(r);
            p.Add(pc);
            Paragraph pc1 = new Paragraph().Add(ph);
            PdfAFormFieldTest.PdfAButtonFieldTestRenderer r1 = new PdfAFormFieldTest.PdfAButtonFieldTestRenderer(pc, group
                , "v2");
            pc1.SetNextRenderer(r1);
            Paragraph p2 = new Paragraph();
            Text t2 = new Text("supported 2");
            t2.SetFont(font);
            p2.Add(t2).Add(pc1);
            doc.Add(p);
            doc.Add(p2);
            //set generateAppearance param to false to retain custom appearance
            group.SetValue("v1", false);
            PdfFormCreator.GetAcroForm(pdf, true).AddField(group);
            pdf.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(filename, SOURCE_FOLDER + "cmp/PdfAFormFieldTest/cmp_"
                 + file, DESTINATION_FOLDER, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void PdfA1DocWithPdfA1ButtonFieldTest() {
            String name = "pdfA1DocWithPdfA1ButtonField";
            String fileName = DESTINATION_FOLDER + name + ".pdf";
            String cmp = SOURCE_FOLDER + "cmp/PdfAFormFieldTest/cmp_pdfA1DocWithPdfA1ButtonField.pdf";
            Stream @is = FileUtil.GetInputStreamForFile(SOURCE_FOLDER + "sRGB Color Space Profile.icm");
            PdfConformance conformance = PdfConformance.PDF_A_1B;
            PdfADocument pdfDoc = new PdfADocument(new PdfWriter(fileName), conformance.GetAConformance(), new PdfOutputIntent
                ("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1", @is));
            PdfAcroForm form = PdfFormCreator.GetAcroForm(pdfDoc, true);
            PdfFormField emptyField = new NonTerminalFormFieldBuilder(pdfDoc, "empty").SetConformance(conformance).CreateNonTerminalFormField
                ();
            emptyField.AddKid(new PushButtonFormFieldBuilder(pdfDoc, "button").SetWidgetRectangle(new Rectangle(36, 756
                , 20, 20)).SetConformance(conformance).CreatePushButton().SetFieldFlags(PdfAnnotation.PRINT).SetFieldName
                ("button").SetValue("hello"));
            form.AddField(emptyField);
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(fileName, cmp, DESTINATION_FOLDER));
            NUnit.Framework.Assert.IsNull(new VeraPdfValidator().Validate(fileName));
        }

        // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf\a validation on Android)
        [NUnit.Framework.Test]
        public virtual void PdfA1DocWithPdfA1CheckBoxFieldTest() {
            String name = "pdfA1DocWithPdfA1CheckBoxField";
            String fileName = DESTINATION_FOLDER + name + ".pdf";
            String cmp = SOURCE_FOLDER + "cmp/PdfAFormFieldTest/cmp_pdfA1DocWithPdfA1CheckBoxField.pdf";
            Stream @is = FileUtil.GetInputStreamForFile(SOURCE_FOLDER + "sRGB Color Space Profile.icm");
            PdfConformance conformance = PdfConformance.PDF_A_1B;
            PdfADocument pdfDoc = new PdfADocument(new PdfWriter(fileName), conformance.GetAConformance(), new PdfOutputIntent
                ("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1", @is));
            PdfAcroForm form = PdfFormCreator.GetAcroForm(pdfDoc, true);
            form.AddField(new CheckBoxFormFieldBuilder(pdfDoc, "checkBox").SetWidgetRectangle(new Rectangle(36, 726, 20
                , 20)).SetCheckType(CheckBoxType.STAR).SetConformance(conformance).CreateCheckBox().SetValue("1"));
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(fileName, cmp, DESTINATION_FOLDER));
            NUnit.Framework.Assert.IsNull(new VeraPdfValidator().Validate(fileName));
        }

        // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf\a validation on Android)
        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.FIELD_VALUE_IS_NOT_CONTAINED_IN_OPT_ARRAY)]
        public virtual void PdfA1DocWithPdfA1ChoiceFieldTest() {
            String name = "pdfA1DocWithPdfA1ChoiceField";
            String fileName = DESTINATION_FOLDER + name + ".pdf";
            String cmp = SOURCE_FOLDER + "cmp/PdfAFormFieldTest/cmp_pdfA1DocWithPdfA1ChoiceField.pdf";
            PdfFont fontFreeSans = PdfFontFactory.CreateFont(SOURCE_FOLDER + "FreeSans.ttf", "WinAnsi", PdfFontFactory.EmbeddingStrategy
                .FORCE_EMBEDDED);
            Stream @is = FileUtil.GetInputStreamForFile(SOURCE_FOLDER + "sRGB Color Space Profile.icm");
            PdfConformance conformance = PdfConformance.PDF_A_1B;
            PdfADocument pdfDoc = new PdfADocument(new PdfWriter(fileName), conformance.GetAConformance(), new PdfOutputIntent
                ("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1", @is));
            PdfAcroForm form = PdfFormCreator.GetAcroForm(pdfDoc, true);
            PdfArray options = new PdfArray();
            options.Add(new PdfString("Name"));
            options.Add(new PdfString("Surname"));
            PdfFormField choiceFormField = new ChoiceFormFieldBuilder(pdfDoc, "choice").SetWidgetRectangle(new Rectangle
                (36, 696, 100, 70)).SetOptions(options).SetConformance(conformance).CreateList().SetValue("1", true);
            choiceFormField.SetFont(fontFreeSans);
            form.AddField(choiceFormField);
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(fileName, cmp, DESTINATION_FOLDER));
            NUnit.Framework.Assert.IsNull(new VeraPdfValidator().Validate(fileName));
        }

        // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf\a validation on Android)
        [NUnit.Framework.Test]
        public virtual void PdfA1DocWithPdfA1ComboBoxFieldTest() {
            String name = "pdfA1DocWithPdfA1ComboBoxField";
            String fileName = DESTINATION_FOLDER + name + ".pdf";
            String cmp = SOURCE_FOLDER + "cmp/PdfAFormFieldTest/cmp_pdfA1DocWithPdfA1ComboBoxField.pdf";
            PdfFont fontCJK = PdfFontFactory.CreateFont(SOURCE_FOLDER + "NotoSansCJKtc-Light.otf", PdfEncodings.IDENTITY_H
                , PdfFontFactory.EmbeddingStrategy.FORCE_EMBEDDED);
            Stream @is = FileUtil.GetInputStreamForFile(SOURCE_FOLDER + "sRGB Color Space Profile.icm");
            PdfConformance conformance = PdfConformance.PDF_A_1B;
            PdfADocument pdfDoc = new PdfADocument(new PdfWriter(fileName), conformance.GetAConformance(), new PdfOutputIntent
                ("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1", @is));
            PdfAcroForm form = PdfFormCreator.GetAcroForm(pdfDoc, true);
            PdfFormField choiceFormField = new ChoiceFormFieldBuilder(pdfDoc, "combo").SetWidgetRectangle(new Rectangle
                (156, 616, 70, 70)).SetOptions(new String[] { "用", "规", "表" }).SetConformance(conformance).CreateComboBox
                ().SetValue("用");
            choiceFormField.SetFont(fontCJK);
            form.AddField(choiceFormField);
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(fileName, cmp, DESTINATION_FOLDER));
            NUnit.Framework.Assert.IsNull(new VeraPdfValidator().Validate(fileName));
        }

        // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf\a validation on Android)
        [NUnit.Framework.Test]
        public virtual void PdfA1DocWithPdfA1ListFieldTest() {
            String name = "pdfA1DocWithPdfA1ListField";
            String fileName = DESTINATION_FOLDER + name + ".pdf";
            String cmp = SOURCE_FOLDER + "cmp/PdfAFormFieldTest/cmp_pdfA1DocWithPdfA1ListField.pdf";
            PdfFont fontFreeSans = PdfFontFactory.CreateFont(SOURCE_FOLDER + "FreeSans.ttf", "WinAnsi", PdfFontFactory.EmbeddingStrategy
                .FORCE_EMBEDDED);
            Stream @is = FileUtil.GetInputStreamForFile(SOURCE_FOLDER + "sRGB Color Space Profile.icm");
            PdfConformance conformance = PdfConformance.PDF_A_1B;
            PdfADocument pdfDoc = new PdfADocument(new PdfWriter(fileName), conformance.GetAConformance(), new PdfOutputIntent
                ("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1", @is));
            PdfAcroForm form = PdfFormCreator.GetAcroForm(pdfDoc, true);
            PdfChoiceFormField f = new ChoiceFormFieldBuilder(pdfDoc, "list").SetWidgetRectangle(new Rectangle(86, 556
                , 50, 200)).SetOptions(new String[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" }).SetConformance
                (conformance).CreateList();
            f.SetValue("9").SetFont(fontFreeSans);
            f.SetValue("4");
            f.SetTopIndex(2);
            f.SetMultiSelect(true);
            f.SetListSelected(new String[] { "3", "5" });
            form.AddField(f);
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(fileName, cmp, DESTINATION_FOLDER));
            NUnit.Framework.Assert.IsNull(new VeraPdfValidator().Validate(fileName));
        }

        // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf\a validation on Android)
        [NUnit.Framework.Test]
        public virtual void PdfA1DocWithPdfA1PushButtonFieldTest() {
            String name = "pdfA1DocWithPdfA1PushButtonField";
            String fileName = DESTINATION_FOLDER + name + ".pdf";
            String cmp = SOURCE_FOLDER + "cmp/PdfAFormFieldTest/cmp_pdfA1DocWithPdfA1PushButtonField.pdf";
            PdfFont fontFreeSans = PdfFontFactory.CreateFont(SOURCE_FOLDER + "FreeSans.ttf", "WinAnsi", PdfFontFactory.EmbeddingStrategy
                .FORCE_EMBEDDED);
            Stream @is = FileUtil.GetInputStreamForFile(SOURCE_FOLDER + "sRGB Color Space Profile.icm");
            PdfConformance conformance = PdfConformance.PDF_A_1B;
            PdfADocument pdfDoc = new PdfADocument(new PdfWriter(fileName), conformance.GetAConformance(), new PdfOutputIntent
                ("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1", @is));
            PdfAcroForm form = PdfFormCreator.GetAcroForm(pdfDoc, true);
            PdfFormField pushButtonFormField = new PushButtonFormFieldBuilder(pdfDoc, "push button").SetWidgetRectangle
                (new Rectangle(36, 526, 100, 20)).SetCaption("Push").SetConformance(conformance).CreatePushButton();
            pushButtonFormField.SetFont(fontFreeSans).SetFontSize(12);
            form.AddField(pushButtonFormField);
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(fileName, cmp, DESTINATION_FOLDER));
            NUnit.Framework.Assert.IsNull(new VeraPdfValidator().Validate(fileName));
        }

        // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf\a validation on Android)
        [NUnit.Framework.Test]
        public virtual void PdfA1DocWithPdfA1RadioButtonFieldTest() {
            String name = "pdfA1DocWithPdfA1RadioButtonField";
            String fileName = DESTINATION_FOLDER + name + ".pdf";
            String cmp = SOURCE_FOLDER + "cmp/PdfAFormFieldTest/cmp_pdfA1DocWithPdfA1RadioButtonField.pdf";
            Stream @is = FileUtil.GetInputStreamForFile(SOURCE_FOLDER + "sRGB Color Space Profile.icm");
            PdfConformance conformance = PdfConformance.PDF_A_1B;
            PdfADocument pdfDoc = new PdfADocument(new PdfWriter(fileName), conformance.GetAConformance(), new PdfOutputIntent
                ("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1", @is));
            PdfAcroForm form = PdfFormCreator.GetAcroForm(pdfDoc, true);
            String pdfFormFieldName = "radio group";
            RadioFormFieldBuilder builder = new RadioFormFieldBuilder(pdfDoc, pdfFormFieldName).SetConformance(conformance
                );
            PdfButtonFormField radioGroup = builder.SetConformance(conformance).CreateRadioGroup();
            radioGroup.SetValue("");
            PdfFormAnnotation radio1 = builder.CreateRadioButton("1", new Rectangle(36, 496, 20, 20)).SetBorderWidth(2
                ).SetBorderColor(ColorConstants.ORANGE);
            PdfFormAnnotation radio2 = builder.CreateRadioButton("2", new Rectangle(66, 496, 20, 20)).SetBorderWidth(2
                ).SetBorderColor(ColorConstants.ORANGE);
            radioGroup.AddKid(radio1);
            radioGroup.AddKid(radio2);
            form.AddField(radioGroup);
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(fileName, cmp, DESTINATION_FOLDER));
            NUnit.Framework.Assert.IsNull(new VeraPdfValidator().Validate(fileName));
        }

        // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf\a validation on Android)
        [NUnit.Framework.Test]
        public virtual void PdfA1DocWithPdfA1TextFieldTest() {
            String name = "pdfA1DocWithPdfA1TextField";
            String fileName = DESTINATION_FOLDER + name + ".pdf";
            String cmp = SOURCE_FOLDER + "cmp/PdfAFormFieldTest/cmp_pdfA1DocWithPdfA1TextField.pdf";
            PdfFont fontFreeSans = PdfFontFactory.CreateFont(SOURCE_FOLDER + "FreeSans.ttf", "WinAnsi", PdfFontFactory.EmbeddingStrategy
                .FORCE_EMBEDDED);
            fontFreeSans.SetSubset(false);
            Stream @is = FileUtil.GetInputStreamForFile(SOURCE_FOLDER + "sRGB Color Space Profile.icm");
            PdfConformance conformance = PdfConformance.PDF_A_1B;
            PdfADocument pdfDoc = new PdfADocument(new PdfWriter(fileName), conformance.GetAConformance(), new PdfOutputIntent
                ("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1", @is));
            PdfAcroForm form = PdfFormCreator.GetAcroForm(pdfDoc, true);
            PdfFormField textFormField = new TextFormFieldBuilder(pdfDoc, "text").SetWidgetRectangle(new Rectangle(36, 
                466, 90, 20)).SetConformance(conformance).CreateText().SetValue("textField").SetValue("iText");
            textFormField.SetFont(fontFreeSans).SetFontSize(12);
            form.AddField(textFormField);
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(fileName, cmp, DESTINATION_FOLDER));
            NUnit.Framework.Assert.IsNull(new VeraPdfValidator().Validate(fileName));
        }

        // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf\a validation on Android)
        [NUnit.Framework.Test]
        public virtual void PdfA1DocWithPdfA1SignatureFieldTest() {
            String name = "pdfA1DocWithPdfA1SignatureField";
            String fileName = DESTINATION_FOLDER + name + ".pdf";
            String cmp = SOURCE_FOLDER + "cmp/PdfAFormFieldTest/cmp_pdfA1DocWithPdfA1SignatureField.pdf";
            PdfFont fontFreeSans = PdfFontFactory.CreateFont(SOURCE_FOLDER + "FreeSans.ttf", "WinAnsi", PdfFontFactory.EmbeddingStrategy
                .FORCE_EMBEDDED);
            fontFreeSans.SetSubset(false);
            Stream @is = FileUtil.GetInputStreamForFile(SOURCE_FOLDER + "sRGB Color Space Profile.icm");
            PdfConformance conformance = PdfConformance.PDF_A_1B;
            PdfADocument pdfDoc = new PdfADocument(new PdfWriter(fileName), conformance.GetAConformance(), new PdfOutputIntent
                ("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1", @is));
            PdfAcroForm form = PdfFormCreator.GetAcroForm(pdfDoc, true);
            PdfFormField signFormField = new SignatureFormFieldBuilder(pdfDoc, "signature").SetConformance(conformance
                ).CreateSignature();
            signFormField.SetFont(fontFreeSans).SetFontSize(20);
            form.AddField(signFormField);
            pdfDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(fileName, cmp, DESTINATION_FOLDER));
            NUnit.Framework.Assert.IsNull(new VeraPdfValidator().Validate(fileName));
        }

        // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf\a validation on Android)
        [NUnit.Framework.Test]
        public virtual void MergePdfADocWithFormTest() {
            String fileName = DESTINATION_FOLDER + "pdfADocWithTextFormField.pdf";
            String mergedDocFileName = DESTINATION_FOLDER + "mergedPdfADoc.pdf";
            using (Stream @is = FileUtil.GetInputStreamForFile(SOURCE_FOLDER + "sRGB Color Space Profile.icm")) {
                using (PdfADocument pdfDoc = new PdfADocument(new PdfWriter(fileName), PdfAConformance.PDF_A_1B, new PdfOutputIntent
                    ("Custom", "", "http://www.color.org", "sRGB ICC preference", @is))) {
                    using (Document doc = new Document(pdfDoc)) {
                        PdfFont font = PdfFontFactory.CreateFont(SOURCE_FOLDER + "FreeSans.ttf", PdfEncodings.WINANSI);
                        doc.Add(new Paragraph(new Text("Some text").SetFont(font).SetFontSize(10)));
                        PdfAcroForm form = PdfFormCreator.GetAcroForm(pdfDoc, true);
                        PdfFormField field = new TextFormFieldBuilder(pdfDoc, "text").SetWidgetRectangle(new Rectangle(150, 100, 100
                            , 20)).SetConformance(PdfConformance.PDF_A_1B).CreateText().SetValue("textField").SetFieldName("text");
                        field.SetFont(font).SetFontSize(10);
                        field.GetFirstFormAnnotation().SetPage(1);
                        form.AddField(field, pdfDoc.GetPage(1));
                    }
                }
            }
            NUnit.Framework.Assert.IsNull(new VeraPdfValidator().Validate(fileName));
            // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf\a validation on Android)
            PdfADocument pdfDocToMerge;
            using (Stream is_1 = FileUtil.GetInputStreamForFile(SOURCE_FOLDER + "sRGB Color Space Profile.icm")) {
                using (PdfDocument newDoc = new PdfDocument(new PdfReader(fileName))) {
                    pdfDocToMerge = new PdfADocument(new PdfWriter(mergedDocFileName).SetSmartMode(true), PdfAConformance.PDF_A_1B
                        , new PdfOutputIntent("Custom", "", "http://www.color.org", "sRGB ICC preference", is_1));
                    newDoc.CopyPagesTo(1, newDoc.GetNumberOfPages(), pdfDocToMerge, new PdfPageFormCopier());
                }
            }
            pdfDocToMerge.Close();
            String cmp = SOURCE_FOLDER + "cmp/PdfAFormFieldTest/cmp_mergePdfADocWithForm.pdf";
            NUnit.Framework.Assert.IsNull(new VeraPdfValidator().Validate(mergedDocFileName));
            // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf\a validation on Android)
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(mergedDocFileName, cmp, DESTINATION_FOLDER
                , "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void TestComboBoxNoFont() {
            String outPdf = DESTINATION_FOLDER + "testComboBoxNoFont.pdf";
            MakePdfDocument(outPdf, null, ((document) => {
                ComboBoxField comboBoxField = new ComboBoxField("combobox");
                comboBoxField.SetWidth(200);
                comboBoxField.SetInteractive(true);
                comboBoxField.AddOption(new SelectFieldItem("item1"));
                comboBoxField.AddOption(new SelectFieldItem("item2"));
                comboBoxField.AddOption(new SelectFieldItem("item3"));
                NUnit.Framework.Assert.Catch(typeof(InvalidOperationException), () => {
                    document.Add(comboBoxField);
                }
                );
            }
            ));
        }

        [NUnit.Framework.Test]
        public virtual void TestButtonNoFont() {
            String outPdf = DESTINATION_FOLDER + "testButtonNoFont.pdf";
            MakePdfDocument(outPdf, null, ((document) => {
                Button button = new Button("button");
                button.SetValue("Hello there");
                button.SetInteractive(true);
                NUnit.Framework.Assert.Catch(typeof(InvalidOperationException), () => {
                    document.Add(button);
                }
                );
            }
            ));
        }

        [NUnit.Framework.Test]
        public virtual void TestTextFieldNoFont() {
            String outPdf = DESTINATION_FOLDER + "testTextFieldNoFont.pdf";
            MakePdfDocument(outPdf, null, ((document) => {
                InputField inputField = new InputField("inputfield");
                inputField.SetValue("Hello there");
                inputField.SetInteractive(true);
                NUnit.Framework.Assert.Catch(typeof(InvalidOperationException), () => {
                    document.Add(inputField);
                }
                );
            }
            ));
        }

        [NUnit.Framework.Test]
        public virtual void TestCheckboxWithPDFA() {
            String outPdf = DESTINATION_FOLDER + "testCheckboxNonPdfa.pdf";
            String cmp = SOURCE_FOLDER + "cmp/PdfAFormFieldTest/cmp_testCheckboxNonPdfa.pdf";
            MakePdfDocument(outPdf, cmp, (doc) => {
                CheckBox checkBox = new CheckBox("CheckBox");
                checkBox.SetChecked(true);
                checkBox.SetInteractive(true);
                checkBox.SetPdfConformance(PdfConformance.PDF_A_1A);
                doc.Add(checkBox);
            }
            );
        }

        [NUnit.Framework.Test]
        public virtual void TestMultipleCombinationsFontOnFieldSeparate() {
            String outPdf = DESTINATION_FOLDER + "testMultipleCombinations.pdf";
            String cmp = SOURCE_FOLDER + "cmp/PdfAFormFieldTest/cmp_testMultipleCombinations.pdf";
            PdfFont font = PdfFontFactory.CreateFont(SOURCE_FOLDER + "FreeSans.ttf", "WinAnsi", PdfFontFactory.EmbeddingStrategy
                .FORCE_EMBEDDED);
            MakePdfDocument(outPdf, cmp, (document) => {
                foreach (Func<IFormField> formFieldSupplier in GenerateFormFields()) {
                    IFormField formField = formFieldSupplier();
                    formField.SetProperty(Property.FONT, font);
                    SolidBorder border = new SolidBorder(ColorConstants.BLACK, 1);
                    formField.SetProperty(Property.BORDER_TOP, border);
                    formField.SetProperty(Property.BORDER_RIGHT, border);
                    formField.SetProperty(Property.BORDER_BOTTOM, border);
                    formField.SetProperty(Property.BORDER_LEFT, border);
                    formField.SetInteractive(true);
                    document.Add(formField);
                }
            }
            );
        }

        [NUnit.Framework.Test]
        public virtual void TestMultipleCombinationsWriteAndReload() {
            String outPdf = DESTINATION_FOLDER + "testMultipleCombinationsWriteAndLoad1.pdf";
            PdfFont font = PdfFontFactory.CreateFont(SOURCE_FOLDER + "FreeSans.ttf", "WinAnsi", PdfFontFactory.EmbeddingStrategy
                .FORCE_EMBEDDED);
            MakePdfDocument(outPdf, null, ((document) => {
                foreach (Func<IFormField> formFieldSupplier in GenerateFormFields()) {
                    IFormField formField = formFieldSupplier();
                    formField.SetProperty(Property.FONT, font);
                    SolidBorder border = new SolidBorder(ColorConstants.BLACK, 1);
                    formField.SetProperty(Property.BORDER_TOP, border);
                    formField.SetProperty(Property.BORDER_RIGHT, border);
                    formField.SetProperty(Property.BORDER_BOTTOM, border);
                    formField.SetProperty(Property.BORDER_LEFT, border);
                    formField.SetInteractive(true);
                    document.Add(formField);
                }
            }
            ));
            String cmp = SOURCE_FOLDER + "cmp/PdfAFormFieldTest/cmp_testMultipleCombinationsWriteAndLoad.pdf";
            String outPdf2 = DESTINATION_FOLDER + "testMultipleCombinationsWriteAndLoad2.pdf";
            PdfADocument newDoc = new PdfADocument(new PdfReader(outPdf), new PdfWriter(outPdf2));
            PdfAcroForm acroForm = PdfAcroForm.GetAcroForm(newDoc, false);
            foreach (KeyValuePair<String, PdfFormField> stringPdfFormFieldEntry in acroForm.GetAllFormFields()) {
                stringPdfFormFieldEntry.Value.SetValue("item1");
            }
            newDoc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf2, cmp, DESTINATION_FOLDER));
            NUnit.Framework.Assert.IsNull(new VeraPdfValidator().Validate(outPdf2));
        }

        // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf\a validation on Android)
        [NUnit.Framework.Test]
        public virtual void TestMultipleCombinationsOnDocument() {
            String outPdf = DESTINATION_FOLDER + "testMultipleCombinationsOnDocument.pdf";
            String cmp = SOURCE_FOLDER + "cmp/PdfAFormFieldTest/cmp_testMultipleCombinationsOnDocument.pdf";
            PdfFont font = PdfFontFactory.CreateFont(SOURCE_FOLDER + "FreeSans.ttf", "WinAnsi", PdfFontFactory.EmbeddingStrategy
                .FORCE_EMBEDDED);
            MakePdfDocument(outPdf, cmp, ((document) => {
                document.SetFont(font);
                foreach (Func<IFormField> formFieldSupplier in GenerateFormFields()) {
                    IFormField formField = formFieldSupplier();
                    SolidBorder border = new SolidBorder(ColorConstants.BLACK, 1);
                    formField.SetProperty(Property.BORDER_TOP, border);
                    formField.SetProperty(Property.BORDER_RIGHT, border);
                    formField.SetProperty(Property.BORDER_BOTTOM, border);
                    formField.SetProperty(Property.BORDER_LEFT, border);
                    formField.SetProperty(Property.FONT, font);
                    formField.SetInteractive(true);
                    document.Add(formField);
                }
            }
            ));
        }

        [NUnit.Framework.Test]
        public virtual void TestMultipleCombinationsFontOnFieldSeparateNonInteractive() {
            String outPdf = DESTINATION_FOLDER + "testMultipleCombinationsNonInteractive.pdf";
            String cmp = SOURCE_FOLDER + "cmp/PdfAFormFieldTest/cmp_testMultipleCombinationsNonInteractive.pdf";
            PdfFont font = PdfFontFactory.CreateFont(SOURCE_FOLDER + "FreeSans.ttf", "WinAnsi", PdfFontFactory.EmbeddingStrategy
                .FORCE_EMBEDDED);
            MakePdfDocument(outPdf, cmp, ((document) => {
                foreach (Func<IFormField> formFieldSupplier in GenerateFormFields()) {
                    IFormField formField = formFieldSupplier();
                    formField.SetProperty(Property.FONT, font);
                    SolidBorder border = new SolidBorder(ColorConstants.BLACK, 1);
                    formField.SetProperty(Property.BORDER_TOP, border);
                    formField.SetProperty(Property.BORDER_RIGHT, border);
                    formField.SetProperty(Property.BORDER_BOTTOM, border);
                    formField.SetProperty(Property.BORDER_LEFT, border);
                    formField.SetInteractive(false);
                    document.Add(formField);
                }
            }
            ));
        }

        [NUnit.Framework.Test]
        public virtual void TestMultipleCombinationsOnDocumentNonInteractive() {
            String outPdf = DESTINATION_FOLDER + "testMultipleCombinationsOnDocumentNonInteractive.pdf";
            String cmp = SOURCE_FOLDER + "cmp/PdfAFormFieldTest/cmp_testMultipleCombinationsOnDocumentNonInteractive.pdf";
            PdfFont font = PdfFontFactory.CreateFont(SOURCE_FOLDER + "FreeSans.ttf", "WinAnsi", PdfFontFactory.EmbeddingStrategy
                .FORCE_EMBEDDED);
            MakePdfDocument(outPdf, cmp, ((document) => {
                document.SetFont(font);
                foreach (Func<IFormField> formFieldSupplier in GenerateFormFields()) {
                    IFormField formField = formFieldSupplier();
                    SolidBorder border = new SolidBorder(ColorConstants.BLACK, 1);
                    formField.SetProperty(Property.BORDER_TOP, border);
                    formField.SetProperty(Property.BORDER_RIGHT, border);
                    formField.SetProperty(Property.BORDER_BOTTOM, border);
                    formField.SetProperty(Property.BORDER_LEFT, border);
                    formField.SetProperty(Property.FONT, font);
                    formField.SetInteractive(false);
                    document.Add(formField);
                }
            }
            ));
        }

        [NUnit.Framework.Test]
        public virtual void TestCopyPagesDoesntEmbedHelveticaFont() {
            String simplePdf = DESTINATION_FOLDER + "simplePdfAWithFormfield.pdf";
            String outPdf = DESTINATION_FOLDER + "testCopyPagesDoesntEmbedHelveticaFont.pdf";
            String cmp = SOURCE_FOLDER + "cmp/PdfAFormFieldTest/cmp_testCopyPagesDoesntEmbedHelveticaFont.pdf";
            PdfFont font = PdfFontFactory.CreateFont(SOURCE_FOLDER + "FreeSans.ttf", "WinAnsi", PdfFontFactory.EmbeddingStrategy
                .FORCE_EMBEDDED);
            PdfWriter writer = new PdfWriter(simplePdf, new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0));
            PdfADocument doc = new PdfADocument(writer, PdfAConformance.PDF_A_4E, new PdfOutputIntent("Custom", "", "http://www.color.org"
                , "sRGB IEC61966-2.1", FileUtil.GetInputStreamForFile(SOURCE_FOLDER + "sRGB Color Space Profile.icm"))
                );
            Document document = new Document(doc);
            document.Add(new InputField("inputfield1").SetFont(font).SetInteractive(true).SetValue("Hello there"));
            document.Add(new Paragraph("Hello there paragraph").SetFont(font));
            doc.Close();
            PdfWriter writer2 = new PdfWriter(outPdf, new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0));
            PdfADocument doc2 = new PdfADocument(writer2, PdfAConformance.PDF_A_4, new PdfOutputIntent("Custom", "", "http://www.color.org"
                , "sRGB IEC61966-2.1", FileUtil.GetInputStreamForFile(SOURCE_FOLDER + "sRGB Color Space Profile.icm"))
                );
            PdfDocument docToCopy = new PdfDocument(new PdfReader(simplePdf));
            docToCopy.CopyPagesTo(1, 1, doc2, new PdfPageFormCopier());
            docToCopy.Close();
            doc2.Close();
            NUnit.Framework.Assert.IsNull(new VeraPdfValidator().Validate(outPdf));
            // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf\a validation on Android)
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmp, DESTINATION_FOLDER, "diff_")
                );
        }

        [NUnit.Framework.Test]
        public virtual void PdfASignatureFieldWithTextAndFontTest() {
            String name = "pdfASignatureFieldTestWithText";
            String fileName = DESTINATION_FOLDER + name + ".pdf";
            String cmp = SOURCE_FOLDER + "cmp/PdfAFormFieldTest/cmp_" + name + ".pdf";
            PdfFont fontFreeSans = PdfFontFactory.CreateFont(SOURCE_FOLDER + "FreeSans.ttf", "WinAnsi", PdfFontFactory.EmbeddingStrategy
                .FORCE_EMBEDDED);
            MakePdfDocument(fileName, cmp, (pdfDoc) => {
                SignatureFieldAppearance signatureFieldAppearance = new SignatureFieldAppearance("Signature1");
                signatureFieldAppearance.SetContent(new SignedAppearanceText().SetLocationLine("HEEELLLLLO"));
                signatureFieldAppearance.SetInteractive(true);
                signatureFieldAppearance.SetFont(fontFreeSans);
                pdfDoc.Add(signatureFieldAppearance);
                PdfAcroForm form = PdfFormCreator.GetAcroForm(pdfDoc.GetPdfDocument(), true);
                SignatureFormFieldBuilder signatureFormFieldBuilder = new SignatureFormFieldBuilder(pdfDoc.GetPdfDocument(
                    ), "Signature2");
                SignatureFieldAppearance signatureFieldAppearance2 = new SignatureFieldAppearance("Signature2");
                signatureFieldAppearance2.SetContent(new SignedAppearanceText().SetLocationLine("Byeeee"));
                signatureFieldAppearance2.SetInteractive(true);
                PdfSignatureFormField signatureFormField = signatureFormFieldBuilder.SetWidgetRectangle(new Rectangle(200, 
                    200, 40, 40)).SetFont(fontFreeSans).SetConformance(PdfConformance.PDF_A_4).CreateSignature();
                signatureFormField.GetFirstFormAnnotation().SetFormFieldElement(signatureFieldAppearance2);
                form.AddField(signatureFormField);
            }
            );
        }

        private void MakePdfDocument(String outPdf, String cmp, Action<Document> consumer) {
            PdfWriter writer = new PdfWriter(outPdf, new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0));
            PdfADocument doc = new PdfADocument(writer, PdfAConformance.PDF_A_4E, new PdfOutputIntent("Custom", "", "http://www.color.org"
                , "sRGB IEC61966-2.1", FileUtil.GetInputStreamForFile(SOURCE_FOLDER + "sRGB Color Space Profile.icm"))
                );
            Document document = new Document(doc);
            consumer(document);
            doc.Close();
            if (cmp == null) {
                return;
            }
            NUnit.Framework.Assert.IsNull(new VeraPdfValidator().Validate(outPdf));
            // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf\a validation on Android)
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmp, DESTINATION_FOLDER));
        }

        private IList<Func<IFormField>> GenerateFormFields() {
            IList<Func<IFormField>> inputs = new List<Func<IFormField>>();
            inputs.Add(() => {
                CheckBox checkBox = new CheckBox("CheckBox");
                checkBox.SetChecked(true);
                checkBox.SetPdfConformance(PdfConformance.PDF_A_4);
                return checkBox;
            }
            );
            inputs.Add(() => {
                CheckBox checkBox = new CheckBox("CheckBox1");
                checkBox.SetChecked(false);
                checkBox.SetPdfConformance(PdfConformance.PDF_A_4);
                return checkBox;
            }
            );
            inputs.Add(() => {
                InputField inputField = new InputField("inputfield1");
                return inputField;
            }
            );
            inputs.Add(() => {
                InputField inputField = new InputField("inputfield2");
                inputField.SetValue("Hello there");
                return inputField;
            }
            );
            inputs.Add(() => {
                Radio radio = new Radio("Radio1", "group1");
                radio.SetChecked(true);
                return radio;
            }
            );
            inputs.Add(() => {
                Radio radio = new Radio("Radio2", "group1");
                radio.SetChecked(false);
                return radio;
            }
            );
            inputs.Add(() => {
                ComboBoxField comboBoxField = new ComboBoxField("combobox1");
                comboBoxField.SetWidth(200);
                comboBoxField.AddOption(new SelectFieldItem("item1"));
                comboBoxField.AddOption(new SelectFieldItem("item2"));
                comboBoxField.AddOption(new SelectFieldItem("item3"));
                return comboBoxField;
            }
            );
            inputs.Add(() => {
                ComboBoxField comboBoxField = new ComboBoxField("combobox2");
                comboBoxField.SetWidth(200);
                comboBoxField.AddOption(new SelectFieldItem("item1"));
                comboBoxField.AddOption(new SelectFieldItem("item2"));
                comboBoxField.AddOption(new SelectFieldItem("item3"));
                comboBoxField.SetSelected(0);
                return comboBoxField;
            }
            );
            inputs.Add(() => {
                TextArea textArea = new TextArea("textarea1");
                textArea.SetValue("Hello there");
                textArea.SetHeight(100);
                textArea.SetWidth(300);
                return textArea;
            }
            );
            inputs.Add(() => {
                TextArea textArea = new TextArea("textarea2");
                textArea.SetHeight(100);
                textArea.SetWidth(300);
                return textArea;
            }
            );
            inputs.Add(() => {
                Button btn = new Button("button1");
                btn.SetValue("Hello button");
                return btn;
            }
            );
            return inputs;
        }

//\cond DO_NOT_DOCUMENT
        internal class PdfAButtonFieldTestRenderer : ParagraphRenderer {
            private PdfButtonFormField _group;

            private String _value;

            public PdfAButtonFieldTestRenderer(Paragraph para, PdfButtonFormField group, String value)
                : base(para) {
                _group = group;
                _value = value;
            }

            public override void Draw(DrawContext context) {
                int pageNumber = GetOccupiedArea().GetPageNumber();
                Rectangle bbox = GetInnerAreaBBox();
                PdfDocument pdf = context.GetDocument();
                PdfAcroForm form = PdfFormCreator.GetAcroForm(pdf, true);
                PdfFormAnnotation chk = new RadioFormFieldBuilder(pdf, "").SetConformance(PdfConformance.PDF_A_1B).CreateRadioButton
                    (_value, bbox);
                _group.AddKid(chk);
                chk.SetPage(pageNumber);
                chk.SetVisibility(PdfFormAnnotation.VISIBLE);
                chk.SetBorderColor(ColorConstants.BLACK);
                chk.SetBackgroundColor(ColorConstants.WHITE);
                _group.SetReadOnly(true);
                PdfFormXObject appearance = new PdfFormXObject(bbox);
                PdfCanvas canvas = new PdfCanvas(appearance, pdf);
                canvas.SaveState().MoveTo(bbox.GetLeft(), bbox.GetBottom()).LineTo(bbox.GetRight(), bbox.GetBottom()).LineTo
                    (bbox.GetRight(), bbox.GetTop()).LineTo(bbox.GetLeft(), bbox.GetTop()).LineTo(bbox.GetLeft(), bbox.GetBottom
                    ()).SetLineWidth(1f).Stroke().RestoreState();
                //form.addFieldAppearanceToPage(chk, pdf.getPage(pageNumber));
                //appearance stream was set, while AS has kept as is, i.e. in Off state.
                chk.SetAppearance(PdfName.N, "v1".Equals(_value) ? _value : "Off", appearance.GetPdfObject());
            }

            public override IRenderer GetNextRenderer() {
                return new PdfAFormFieldTest.PdfAButtonFieldTestRenderer((Paragraph)modelElement, _group, _value);
            }
        }
//\endcond
    }
}
