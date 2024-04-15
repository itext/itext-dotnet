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
using System.Collections.Generic;
using iText.Forms.Fields.Properties;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Forms.Form.Element {
    [NUnit.Framework.Category("IntegrationTest")]
    public class FixedPositionTest : ExtendedITextTest {
        public static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/forms/form/element/FixedPositionTest/";

        public static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/forms/form/element/FixedPositionTest/";

        public static readonly String IMG_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/forms/form/element/SignatureFieldAppearanceTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void SetUp() {
            CreateDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void NonInteractive() {
            String outputFileName = DESTINATION_FOLDER + "ni_setFixedPosition.pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_ni_setFixedPosition.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outputFileName))) {
                Document document = new Document(pdfDocument);
                int left = 100;
                int bottom = 700;
                int width = 150;
                foreach (Func<IFormField> iFormFieldSupplier in GetDataToTest()) {
                    IFormField field = iFormFieldSupplier();
                    new FixedPositionTest.DummyContainer().ApplyFixedPosition(left, bottom, width).ApplyToElement(field);
                    bottom -= 75;
                    document.Add(field);
                }
                document.Close();
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outputFileName, cmpFileName, DESTINATION_FOLDER
                , "diff"));
        }

        [NUnit.Framework.Test]
        public virtual void NonInteractiveOnSpecificPage() {
            String outputFileName = DESTINATION_FOLDER + "ni_setFixedPositionOnPage.pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_ni_setFixedPositionOnPage.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outputFileName))) {
                Document document = new Document(pdfDocument);
                int left = 100;
                int bottom = 700;
                int width = 150;
                int page = 1;
                foreach (Func<IFormField> iFormFieldSupplier in GetDataToTest()) {
                    IFormField field = iFormFieldSupplier();
                    new FixedPositionTest.DummyContainer().ApplyFixedPosition(++page, left, bottom, width).ApplyToElement(field
                        );
                    document.Add(field);
                }
                document.Close();
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outputFileName, cmpFileName, DESTINATION_FOLDER
                , "diff"));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.CLIP_ELEMENT)]
        public virtual void Interactive() {
            String outputFileName = DESTINATION_FOLDER + "interactive_fixed_pos.pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_interactive_fixed_pos.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outputFileName))) {
                Document document = new Document(pdfDocument);
                int left = 100;
                int bottom = 700;
                int width = 150;
                foreach (Func<IFormField> iFormFieldSupplier in GetDataToTest()) {
                    IFormField field = iFormFieldSupplier();
                    field.SetInteractive(true);
                    new FixedPositionTest.DummyContainer().ApplyFixedPosition(left, bottom, width).ApplyToElement(field);
                    document.Add(field);
                    bottom -= 75;
                }
                document.Close();
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outputFileName, cmpFileName, DESTINATION_FOLDER
                , "diff"));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.CLIP_ELEMENT)]
        public virtual void InteractiveOnPage() {
            String outputFileName = DESTINATION_FOLDER + "interactive_fixed_pos_on_page.pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_interactive_fixed_pos_on_page.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outputFileName))) {
                Document document = new Document(pdfDocument);
                int left = 100;
                int bottom = 700;
                int width = 150;
                int page = 1;
                foreach (Func<IFormField> iFormFieldSupplier in GetDataToTest()) {
                    IFormField field = iFormFieldSupplier();
                    field.SetInteractive(true);
                    new FixedPositionTest.DummyContainer().ApplyFixedPosition(++page, left, bottom, width).ApplyToElement(field
                        );
                    document.Add(field);
                }
                document.Close();
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outputFileName, cmpFileName, DESTINATION_FOLDER
                , "diff"));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.CLIP_ELEMENT)]
        public virtual void InteractiveWidthOutOfBounds() {
            String outputFileName = DESTINATION_FOLDER + "interactiveOutOfBounds.pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_interactiveOutOfBounds.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outputFileName))) {
                Document document = new Document(pdfDocument);
                int left = 100;
                int bottom = 10000;
                int width = 100;
                foreach (Func<IFormField> iFormFieldSupplier in GetDataToTest()) {
                    IFormField field = iFormFieldSupplier();
                    field.SetInteractive(true);
                    new FixedPositionTest.DummyContainer().ApplyFixedPosition(left, bottom, width).ApplyToElement(field);
                    document.Add(field);
                    bottom -= 75;
                }
                document.Close();
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outputFileName, cmpFileName, DESTINATION_FOLDER
                , "diff"));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.CLIP_ELEMENT, Count = 2)]
        public virtual void InteractiveMarginLeft() {
            String outputFileName = DESTINATION_FOLDER + "interactiveMarginLeft.pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_interactiveMarginLeft.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outputFileName))) {
                Document document = new Document(pdfDocument);
                foreach (Func<IFormField> iFormFieldSupplier in GetDataToTest()) {
                    IFormField field = iFormFieldSupplier();
                    new FixedPositionTest.DummyContainer().ApplyToElement(field);
                    UnitValue marginUV = UnitValue.CreatePointValue(200);
                    field.SetProperty(Property.MARGIN_LEFT, marginUV);
                    document.Add(field);
                }
                document.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));
                foreach (Func<IFormField> iFormFieldSupplier in GetDataToTest()) {
                    IFormField field = iFormFieldSupplier();
                    field.SetInteractive(true);
                    new FixedPositionTest.DummyContainer().ApplyToElement(field);
                    UnitValue marginUV = UnitValue.CreatePointValue(200);
                    field.SetProperty(Property.MARGIN_LEFT, marginUV);
                    document.Add(field);
                }
                document.Close();
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outputFileName, cmpFileName, DESTINATION_FOLDER
                , "diff"));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.CLIP_ELEMENT, Count = 2)]
        public virtual void InteractiveMarginTop() {
            String outputFileName = DESTINATION_FOLDER + "marginTop.pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_marginTop.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outputFileName))) {
                Document document = new Document(pdfDocument, PageSize.A4, false);
                foreach (Func<IFormField> iFormFieldSupplier in GetDataToTest()) {
                    IFormField field = iFormFieldSupplier();
                    new FixedPositionTest.DummyContainer().ApplyToElement(field);
                    UnitValue marginUV = UnitValue.CreatePointValue(50);
                    field.SetProperty(Property.MARGIN_TOP, marginUV);
                    document.Add(field);
                }
                document.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));
                foreach (Func<IFormField> iFormFieldSupplier in GetDataToTest()) {
                    IFormField field = iFormFieldSupplier();
                    field.SetInteractive(true);
                    new FixedPositionTest.DummyContainer().ApplyToElement(field);
                    UnitValue marginUV = UnitValue.CreatePointValue(50);
                    field.SetProperty(Property.MARGIN_TOP, marginUV);
                    document.Add(field);
                }
                document.Close();
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outputFileName, cmpFileName, DESTINATION_FOLDER
                , "diff"));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.CLIP_ELEMENT, Count = 2)]
        public virtual void Width() {
            String outputFileName = DESTINATION_FOLDER + "width.pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_width.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outputFileName))) {
                Document document = new Document(pdfDocument, PageSize.A4, false);
                foreach (Func<IFormField> iFormFieldSupplier in GetDataToTest()) {
                    IFormField field = iFormFieldSupplier();
                    new FixedPositionTest.DummyContainer().ApplyToElement(field);
                    field.SetProperty(Property.WIDTH, UnitValue.CreatePointValue(400));
                    document.Add(field);
                }
                document.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));
                foreach (Func<IFormField> iFormFieldSupplier in GetDataToTest()) {
                    IFormField field = iFormFieldSupplier();
                    field.SetInteractive(true);
                    new FixedPositionTest.DummyContainer().ApplyToElement(field);
                    field.SetProperty(Property.WIDTH, UnitValue.CreatePointValue(400));
                    document.Add(field);
                }
                document.Close();
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outputFileName, cmpFileName, DESTINATION_FOLDER
                , "diff"));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.CLIP_ELEMENT, Count = 2)]
        public virtual void Padding() {
            String outputFileName = DESTINATION_FOLDER + "padding.pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_padding.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outputFileName))) {
                Document document = new Document(pdfDocument, PageSize.A4, false);
                foreach (Func<IFormField> iFormFieldSupplier in GetDataToTest()) {
                    IFormField field = iFormFieldSupplier();
                    new FixedPositionTest.DummyContainer().ApplyToElement(field);
                    field.SetProperty(Property.PADDING_LEFT, UnitValue.CreatePointValue(50));
                    document.Add(field);
                }
                document.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));
                foreach (Func<IFormField> iFormFieldSupplier in GetDataToTest()) {
                    IFormField field = iFormFieldSupplier();
                    field.SetInteractive(true);
                    new FixedPositionTest.DummyContainer().ApplyToElement(field);
                    field.SetProperty(Property.LEFT, UnitValue.CreatePointValue(20));
                    document.Add(field);
                }
                document.Close();
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outputFileName, cmpFileName, DESTINATION_FOLDER
                , "diff"));
        }

        private static IList<Func<IFormField>> GetDataToTest() {
            IList<Func<IFormField>> data = new List<Func<IFormField>>();
            data.Add(() => {
                InputField inputField = new InputField("inputField");
                inputField.SetValue("value some text");
                return inputField;
            }
            );
            data.Add(() => {
                TextArea textArea = new TextArea("textArea");
                textArea.SetValue("value some text\nsome more text");
                return textArea;
            }
            );
            data.Add(() => {
                Radio radio = new Radio("radioButton", "group");
                radio.SetChecked(true);
                return radio;
            }
            );
            data.Add(() => {
                ComboBoxField field = new ComboBoxField("comboBox");
                field.AddOption(new SelectFieldItem("option1"));
                field.AddOption(new SelectFieldItem("option2"));
                field.SetSelected("option1");
                return field;
            }
            );
            data.Add(() => {
                ListBoxField field2 = new ListBoxField("listBox", 4, false);
                field2.AddOption(new SelectFieldItem("option1"));
                field2.AddOption(new SelectFieldItem("option2"));
                field2.SetValue("option1");
                return field2;
            }
            );
            data.Add(() => {
                SignatureFieldAppearance app = new SignatureFieldAppearance("signatureField1");
                app.SetContent(new SignedAppearanceText().SetSignedBy("signer\nname").SetLocationLine("location").SetReasonLine
                    ("reason"));
                return app;
            }
            );
            ImageData image = ImageDataFactory.Create(IMG_FOLDER + "1.png");
            data.Add(() => {
                SignatureFieldAppearance app = new SignatureFieldAppearance("signatureField2");
                app.SetContent(image);
                return app;
            }
            );
            data.Add(() => {
                SignatureFieldAppearance app = new SignatureFieldAppearance("signatureField3");
                app.SetContent("signature with image and description test\n" + "signature with image and description test\nsignature with image and description test"
                    , image);
                return app;
            }
            );
            data.Add(() => {
                SignatureFieldAppearance app = new SignatureFieldAppearance("signatureField4");
                app.SetContent("signer", new SignedAppearanceText().SetSignedBy("signer").SetLocationLine("location").SetReasonLine
                    ("reason"));
                return app;
            }
            );
            data.Add(() => {
                SignatureFieldAppearance app = new SignatureFieldAppearance("signatureField5");
                app.SetContent(new Div().Add(new Paragraph("signature with div element test\n" + "signature with div element test\nsignature with div element test"
                    )));
                return app;
            }
            );
            data.Add(() => {
                Button button = new Button("button");
                button.SetValue("Click me");
                return button;
            }
            );
            data.Add(() => {
                CheckBox cb = new CheckBox("checkBox");
                cb.SetSize(20);
                cb.SetChecked(true);
                return cb;
            }
            );
            return data;
        }

        public class DummyContainer : ElementPropertyContainer<IPropertyContainer> {
            public DummyContainer() {
                SetBackgroundColor(ColorConstants.RED);
                SetBorder(new SolidBorder(ColorConstants.BLUE, 2));
            }

            public virtual FixedPositionTest.DummyContainer ApplyFixedPosition(int left, int bottom, int width) {
                SetFixedPosition(left, bottom, width);
                return this;
            }

            public virtual FixedPositionTest.DummyContainer ApplyFixedPosition(int pageNumber, int left, int bottom, int
                 width) {
                SetFixedPosition(pageNumber, left, bottom, width);
                return this;
            }

            public virtual void ApplyToElement(IPropertyContainer propertyContainer) {
                foreach (int? i in this.properties.Keys) {
                    Object value = this.properties.Get((int)i);
                    propertyContainer.SetProperty((int)i, value);
                }
            }
        }
    }
}
