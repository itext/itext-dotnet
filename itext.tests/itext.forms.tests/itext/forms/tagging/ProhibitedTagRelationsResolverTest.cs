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
using System.IO;
using iText.Forms;
using iText.Forms.Form;
using iText.Forms.Form.Element;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Tagging;
using iText.Kernel.XMP;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Layout.Tagging;
using iText.Test;
using iText.Test.Pdfa;

namespace iText.Forms.Tagging {
    // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf/ua validation on Android)
    [NUnit.Framework.Category("IntegrationTest")]
    public class ProhibitedTagRelationsResolverTest : ExtendedITextTest {
        public static readonly String DESTINATION_FOLDER = TestUtil.GetOutputPath() + "/forms" + "/ResolveProhibitedRelationsRuleTest/";

        public static readonly String FONT_LOCATION = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/forms/fonts/NotoSans-Regular.ttf";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void Forms001() {
            String dest = DESTINATION_FOLDER + "testf001.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(dest, new WriterProperties().SetPdfVersion(PdfVersion
                .PDF_2_0)));
            ConvertToUa2(pdfDocument);
            PdfFont font = PdfFontFactory.CreateFont(FONT_LOCATION);
            Document document = new Document(pdfDocument);
            document.SetFont(font);
            InputField inputField = new InputField("name");
            inputField.SetValue("Hello world!");
            document.Add(inputField);
            pdfDocument.Close();
            NUnit.Framework.Assert.IsNull(new VeraPdfValidator().Validate(dest));
        }

        // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf\a validation on Android)
        [NUnit.Framework.Test]
        public virtual void Forms001Interactive() {
            String dest = DESTINATION_FOLDER + "testf001Interactive.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(dest, new WriterProperties().SetPdfVersion(PdfVersion
                .PDF_2_0)));
            ConvertToUa2(pdfDocument);
            PdfFont font = PdfFontFactory.CreateFont(FONT_LOCATION);
            Document document = new Document(pdfDocument);
            document.SetFont(font);
            InputField inputField = new InputField("name");
            inputField.SetValue("Hello world!");
            inputField.SetInteractive(true);
            inputField.GetAccessibilityProperties().SetAlternateDescription("This is an input field");
            document.Add(inputField);
            PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDocument, true);
            PdfDictionary f = form.GetField("name").GetPdfObject();
            f.Put(PdfName.Contents, new PdfString("Hello world!"));
            pdfDocument.Close();
            NUnit.Framework.Assert.IsNull(new VeraPdfValidator().Validate(dest));
        }

        // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf\a validation on Android)
        [NUnit.Framework.Test]
        public virtual void Forms002() {
            String dest = DESTINATION_FOLDER + "testf002.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(dest, new WriterProperties().SetPdfVersion(PdfVersion
                .PDF_2_0)));
            ConvertToUa2(pdfDocument);
            PdfFont font = PdfFontFactory.CreateFont(FONT_LOCATION);
            Document document = new Document(pdfDocument);
            document.SetFont(font);
            ComboBoxField comboBoxField = new ComboBoxField("name");
            comboBoxField.AddOption(new SelectFieldItem("Hello world!"));
            comboBoxField.AddOption(new SelectFieldItem("Hello world1!"));
            comboBoxField.AddOption(new SelectFieldItem("Hello world2!"));
            document.Add(comboBoxField);
            ListBoxField listBoxField = new ListBoxField("name1", 4, true);
            listBoxField.AddOption(new SelectFieldItem("Hello world!"));
            listBoxField.AddOption(new SelectFieldItem("Hello world1!"));
            listBoxField.AddOption(new SelectFieldItem("Hello world2!"));
            document.Add(listBoxField);
            ListBoxField listBoxField1 = new ListBoxField("name2", 4, false);
            listBoxField1.AddOption(new SelectFieldItem("Hello world!"));
            listBoxField1.AddOption(new SelectFieldItem("Hello world1!"));
            listBoxField1.AddOption(new SelectFieldItem("Hello world2!"));
            document.Add(listBoxField1);
            pdfDocument.Close();
            NUnit.Framework.Assert.IsNull(new VeraPdfValidator().Validate(dest));
        }

        // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf\a validation on Android)
        [NUnit.Framework.Test]
        public virtual void Forms003() {
            String dest = DESTINATION_FOLDER + "testf003.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(dest, new WriterProperties().SetPdfVersion(PdfVersion
                .PDF_2_0)));
            ConvertToUa2(pdfDocument);
            PdfFont font = PdfFontFactory.CreateFont(FONT_LOCATION);
            Document document = new Document(pdfDocument);
            document.SetFont(font);
            Generate0(document);
            pdfDocument.Close();
            NUnit.Framework.Assert.IsNull(new VeraPdfValidator().Validate(dest));
        }

        // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf\a validation on Android)
        [NUnit.Framework.Test]
        public virtual void Forms005() {
            String dest = DESTINATION_FOLDER + "testf005.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(dest, new WriterProperties().SetPdfVersion(PdfVersion
                .PDF_2_0)));
            ConvertToUa2(pdfDocument);
            PdfFont font = PdfFontFactory.CreateFont(FONT_LOCATION);
            Document document = new Document(pdfDocument);
            document.SetFont(font);
            TextArea textArea = new TextArea("name");
            textArea.SetValue("Hello world!");
            document.Add(textArea);
            TextArea textArea1 = new TextArea("name1");
            textArea1.SetPlaceholder(new Paragraph("Hello world!"));
            document.Add(textArea1);
            pdfDocument.Close();
            NUnit.Framework.Assert.IsNull(new VeraPdfValidator().Validate(dest));
        }

        // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf\a validation on Android)
        [NUnit.Framework.Test]
        public virtual void Forms004() {
            String dest = DESTINATION_FOLDER + "testf004.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(dest, new WriterProperties().SetPdfVersion(PdfVersion
                .PDF_2_0)));
            ConvertToUa2(pdfDocument);
            PdfFont font = PdfFontFactory.CreateFont(FONT_LOCATION);
            Document document = new Document(pdfDocument);
            document.SetFont(font);
            CheckBox checkBox = new CheckBox("name");
            checkBox.SetChecked(true);
            document.Add(checkBox);
            pdfDocument.Close();
            NUnit.Framework.Assert.IsNull(new VeraPdfValidator().Validate(dest));
        }

        // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf\a validation on Android)
        [NUnit.Framework.Test]
        public virtual void FormsButton() {
            String dest = DESTINATION_FOLDER + "testButton.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(dest, new WriterProperties().SetPdfVersion(PdfVersion
                .PDF_2_0)));
            ConvertToUa2(pdfDocument);
            PdfFont font = PdfFontFactory.CreateFont(FONT_LOCATION);
            Document document = new Document(pdfDocument);
            document.SetFont(font);
            Button button = new Button("name");
            Paragraph h1 = new Paragraph("Header 1");
            h1.GetAccessibilityProperties().SetRole(StandardRoles.H1);
            Paragraph paragraph = new Paragraph("Hello World");
            h1.Add(paragraph);
            button.Add(h1);
            document.Add(button);
            pdfDocument.Close();
            NUnit.Framework.Assert.IsNull(new VeraPdfValidator().Validate(dest));
        }

        // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf\a validation on Android)
        public virtual void Generate0(Document parent) {
            Paragraph paragraph1 = new Paragraph();
            paragraph1.SetMargins(0.000000F, 0.000000F, 0.000000F, 0.000000F);
            paragraph1.SetProperty(Property.COLLAPSING_MARGINS, true);
            paragraph1.SetProperty(Property.OVERFLOW_X, OverflowPropertyValue.VISIBLE);
            paragraph1.SetProperty(Property.OVERFLOW_Y, OverflowPropertyValue.VISIBLE);
            paragraph1.SetProperty(Property.RENDERING_MODE, RenderingMode.HTML_MODE);
            this.Generate1(paragraph1);
            parent.Add(paragraph1);
        }

        public virtual void Generate1(Paragraph parent) {
            Radio radio1 = new Radio("A");
            radio1.SetFontSize(9.999975F);
            radio1.SetProperty(Property.BACKGROUND, new Background(new DeviceRgb(1.000000F, 1.000000F, 1.000000F), 1.000000F
                , BackgroundBox.BORDER_BOX));
            radio1.SetProperty(Property.BORDER_BOTTOM, new SolidBorder(new DeviceRgb(0.752941F, 0.752941F, 0.752941F), 
                1.000000F, 1.000000F));
            radio1.SetProperty(Property.BORDER_LEFT, new SolidBorder(new DeviceRgb(0.752941F, 0.752941F, 0.752941F), 1.000000F
                , 1.000000F));
            radio1.SetProperty(Property.BORDER_RIGHT, new SolidBorder(new DeviceRgb(0.752941F, 0.752941F, 0.752941F), 
                1.000000F, 1.000000F));
            radio1.SetProperty(Property.BORDER_TOP, new SolidBorder(new DeviceRgb(0.752941F, 0.752941F, 0.752941F), 1.000000F
                , 1.000000F));
            radio1.SetProperty(Property.FIRST_LINE_INDENT, 0.000000F);
            radio1.SetProperty(Property.HYPHENATION, null);
            radio1.SetProperty(Property.LEADING, null);
            radio1.SetProperty(FormProperty.FORM_FIELD_FLATTEN, true);
            radio1.SetProperty(Property.MARGIN_BOTTOM, new UnitValue(UnitValue.POINT, 2.250000F));
            radio1.SetProperty(Property.MARGIN_LEFT, new UnitValue(UnitValue.POINT, 2.499994F));
            radio1.SetProperty(FormProperty.FORM_FIELD_RADIO_GROUP_NAME, "A");
            radio1.SetProperty(Property.MARGIN_RIGHT, new UnitValue(UnitValue.POINT, 2.499994F));
            radio1.SetProperty(FormProperty.FORM_FIELD_RADIO_BORDER_CIRCLE, true);
            radio1.SetProperty(Property.MARGIN_TOP, new UnitValue(UnitValue.POINT, 2.250000F));
            radio1.SetProperty(Property.PADDING_BOTTOM, new UnitValue(UnitValue.POINT, 0.000000F));
            radio1.SetProperty(FormProperty.FORM_CONFORMANCE_LEVEL, null);
            radio1.SetProperty(Property.PADDING_LEFT, new UnitValue(UnitValue.POINT, 0.000000F));
            radio1.SetProperty(Property.PADDING_RIGHT, new UnitValue(UnitValue.POINT, 0.000000F));
            radio1.SetProperty(Property.PADDING_TOP, new UnitValue(UnitValue.POINT, 0.000000F));
            radio1.SetProperty(Property.OVERFLOW_X, OverflowPropertyValue.VISIBLE);
            radio1.SetProperty(Property.OVERFLOW_Y, OverflowPropertyValue.VISIBLE);
            radio1.SetProperty(Property.BORDER_TOP_LEFT_RADIUS, null);
            radio1.SetProperty(Property.BORDER_TOP_RIGHT_RADIUS, null);
            radio1.SetProperty(Property.BORDER_BOTTOM_RIGHT_RADIUS, null);
            radio1.SetProperty(Property.BORDER_BOTTOM_LEFT_RADIUS, null);
            radio1.SetProperty(Property.NO_SOFT_WRAP_INLINE, false);
            parent.Add(radio1);
        }

        private static void ConvertToUa2(PdfDocument pdfDocument) {
            // We can't depend on ua module in layout module so we need to do some low level operations
            // to convert the to ua2
            pdfDocument.GetDiContainer().Register(typeof(ProhibitedTagRelationsResolver), new ProhibitedTagRelationsResolver
                (pdfDocument));
            byte[] bytes = File.ReadAllBytes(System.IO.Path.Combine(iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
                .CurrentContext.TestDirectory) + "/resources/itext/forms/ua/simplePdfUA2.xmp"));
            XMPMeta xmpMeta = XMPMetaFactory.Parse(new MemoryStream(bytes));
            pdfDocument.SetXmpMetadata(xmpMeta);
            pdfDocument.SetTagged();
            pdfDocument.GetCatalog().SetViewerPreferences(new PdfViewerPreferences().SetDisplayDocTitle(true));
            pdfDocument.GetCatalog().SetLang(new PdfString("en-US"));
            PdfDocumentInfo info = pdfDocument.GetDocumentInfo();
            info.SetTitle("PdfUA2 Title");
        }
    }
}
