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
using iText.Forms;
using iText.Forms.Fields;
using iText.Forms.Fields.Borders;
using iText.Forms.Fields.Properties;
using iText.Forms.Form;
using iText.Forms.Logs;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Annot;
using iText.Kernel.Utils;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Exceptions;
using iText.Layout.Properties;
using iText.Layout.Renderer;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Forms.Form.Element {
    [NUnit.Framework.Category("IntegrationTest")]
    public class SignatureFieldAppearanceTest : ExtendedITextTest {
        public static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/forms/form/element/SignatureFieldAppearanceTest/";

        public static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/forms/form/element/SignatureFieldAppearanceTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void BasicSigFieldTest() {
            String outPdf = DESTINATION_FOLDER + "basicSigField.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_basicSigField.pdf";
            using (Document document = new Document(new PdfDocument(new PdfWriter(outPdf)))) {
                SignatureFieldAppearance formSigField = new SignatureFieldAppearance("form SigField");
                formSigField.SetProperty(FormProperty.FORM_FIELD_FLATTEN, false);
                formSigField.SetContent("form SigField");
                formSigField.SetBorder(new SolidBorder(ColorConstants.YELLOW, 1));
                document.Add(formSigField);
                document.Add(new Paragraph(""));
                SignatureFieldAppearance flattenSigField = new SignatureFieldAppearance("flatten SigField");
                flattenSigField.SetProperty(FormProperty.FORM_FIELD_FLATTEN, true);
                flattenSigField.SetContent("flatten SigField");
                flattenSigField.SetBorder(new SolidBorder(ColorConstants.PINK, 1));
                document.Add(flattenSigField);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        [LogMessage(FormsLogMessageConstants.INPUT_FIELD_DOES_NOT_FIT)]
        public virtual void InvisibleSigFieldTest() {
            String outPdf = DESTINATION_FOLDER + "invisibleSigField.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_invisibleSigField.pdf";
            using (Document document = new Document(new PdfDocument(new PdfWriter(outPdf)))) {
                SignatureFieldAppearance formSigField = new SignatureFieldAppearance("form SigField");
                formSigField.SetProperty(FormProperty.FORM_FIELD_FLATTEN, false);
                formSigField.SetContent("form SigField");
                formSigField.SetProperty(Property.PADDING_LEFT, UnitValue.CreatePointValue(0));
                formSigField.SetProperty(Property.PADDING_BOTTOM, UnitValue.CreatePointValue(0));
                formSigField.SetProperty(Property.PADDING_RIGHT, UnitValue.CreatePointValue(0));
                formSigField.SetProperty(Property.PADDING_TOP, UnitValue.CreatePointValue(0));
                formSigField.SetWidth(0);
                document.Add(formSigField);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        public virtual void CustomizedSigFieldTest() {
            String outPdf = DESTINATION_FOLDER + "customizedSigField.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_customizedSigField.pdf";
            using (Document document = new Document(new PdfDocument(new PdfWriter(outPdf)))) {
                SignedAppearanceText description = new SignedAppearanceText().SetSignedBy("signer").SetLocationLine("Location capt: location"
                    ).SetReasonLine("Reason capt: reason");
                SignatureFieldAppearance formSigField = new SignatureFieldAppearance("form SigField");
                formSigField.SetProperty(FormProperty.FORM_FIELD_FLATTEN, false);
                formSigField.SetContent(description);
                formSigField.SetFontColor(ColorConstants.BLUE);
                formSigField.SetBackgroundColor(ColorConstants.YELLOW);
                formSigField.SetBorder(new SolidBorder(ColorConstants.GREEN, 2));
                formSigField.SetHeight(100);
                document.Add(formSigField);
                document.Add(new Paragraph(""));
                SignatureFieldAppearance flattenSigField = new SignatureFieldAppearance("flatten SigField");
                flattenSigField.SetProperty(FormProperty.FORM_FIELD_FLATTEN, true);
                flattenSigField.SetContent(description);
                flattenSigField.SetFontColor(ColorConstants.BLUE);
                flattenSigField.SetBackgroundColor(ColorConstants.YELLOW);
                flattenSigField.SetBorder(new SolidBorder(ColorConstants.GREEN, 2));
                flattenSigField.SetHeight(100);
                document.Add(flattenSigField);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.CLIP_ELEMENT)]
        public virtual void SignatureFieldVerticalAlignmentTest() {
            String outPdf = DESTINATION_FOLDER + "signatureFieldVerticalAlignment.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_signatureFieldVerticalAlignment.pdf";
            using (Document document = new Document(new PdfDocument(new PdfWriter(outPdf)))) {
                SignatureFieldAppearance bottomSigField = new SignatureFieldAppearance("bottomSigField");
                bottomSigField.SetProperty(FormProperty.FORM_FIELD_FLATTEN, true);
                bottomSigField.SetContent("description on bottom");
                bottomSigField.SetProperty(Property.VERTICAL_ALIGNMENT, VerticalAlignment.BOTTOM);
                bottomSigField.SetProperty(Property.TEXT_ALIGNMENT, TextAlignment.CENTER);
                bottomSigField.SetProperty(Property.HEIGHT, UnitValue.CreatePointValue(100));
                bottomSigField.SetBorder(new SolidBorder(ColorConstants.YELLOW, 3));
                bottomSigField.SetFontSize(15);
                document.Add(bottomSigField);
                SignatureFieldAppearance middleSigField = new SignatureFieldAppearance("middleSigField");
                middleSigField.SetProperty(FormProperty.FORM_FIELD_FLATTEN, true);
                middleSigField.SetContent("Name", "description on the middle");
                middleSigField.SetProperty(Property.VERTICAL_ALIGNMENT, VerticalAlignment.MIDDLE);
                middleSigField.SetProperty(Property.TEXT_ALIGNMENT, TextAlignment.CENTER);
                middleSigField.SetProperty(Property.HEIGHT, UnitValue.CreatePointValue(100));
                middleSigField.SetBorder(new SolidBorder(ColorConstants.BLUE, 3));
                middleSigField.SetFontSize(15);
                document.Add(middleSigField);
                SignatureFieldAppearance topSigField = new SignatureFieldAppearance("topSigField");
                topSigField.SetProperty(FormProperty.FORM_FIELD_FLATTEN, true);
                topSigField.SetContent("description on top", ImageDataFactory.Create(SOURCE_FOLDER + "1.png"));
                topSigField.SetProperty(Property.VERTICAL_ALIGNMENT, VerticalAlignment.TOP);
                topSigField.SetProperty(Property.TEXT_ALIGNMENT, TextAlignment.CENTER);
                topSigField.SetProperty(Property.HEIGHT, UnitValue.CreatePointValue(100));
                topSigField.SetBorder(new SolidBorder(ColorConstants.PINK, 3));
                topSigField.SetFontSize(15);
                document.Add(topSigField);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        public virtual void SigFieldWithGraphicAndDescriptionModeTest() {
            String outPdf = DESTINATION_FOLDER + "sigFieldWithGraphicAndDescriptionMode.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_sigFieldWithGraphicAndDescriptionMode.pdf";
            String imagePath = SOURCE_FOLDER + "1.png";
            using (Document document = new Document(new PdfDocument(new PdfWriter(outPdf)))) {
                SignatureFieldAppearance formSigField = new SignatureFieldAppearance("SigField");
                ImageData image = ImageDataFactory.Create(imagePath);
                formSigField.SetContent("description", image);
                formSigField.SetFontColor(ColorConstants.BLUE);
                formSigField.SetFontSize(20);
                formSigField.SetBackgroundColor(ColorConstants.YELLOW);
                formSigField.SetBorder(new SolidBorder(ColorConstants.GREEN, 1));
                formSigField.SetProperty(FormProperty.FORM_FIELD_FLATTEN, true);
                document.Add(formSigField);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        public virtual void SigFieldWithGraphicModeTest() {
            String outPdf = DESTINATION_FOLDER + "sigFieldWithGraphicMode.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_sigFieldWithGraphicMode.pdf";
            String imagePath = SOURCE_FOLDER + "1.png";
            using (Document document = new Document(new PdfDocument(new PdfWriter(outPdf)))) {
                SignatureFieldAppearance formSigField = new SignatureFieldAppearance("SigField");
                ImageData image = ImageDataFactory.Create(imagePath);
                formSigField.SetContent(image);
                formSigField.SetBorder(new SolidBorder(ColorConstants.GREEN, 1));
                formSigField.SetProperty(FormProperty.FORM_FIELD_FLATTEN, true);
                document.Add(formSigField);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        public virtual void SigFieldWithNameAndDescriptionModeHorizontalTest() {
            String outPdf = DESTINATION_FOLDER + "sigFieldWithNameAndDescriptionModeHorizontal.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_sigFieldWithNameAndDescriptionModeHorizontal.pdf";
            using (Document document = new Document(new PdfDocument(new PdfWriter(outPdf)))) {
                SignatureFieldAppearance formSigField = new SignatureFieldAppearance("SigField");
                formSigField.SetProperty(Property.WIDTH, UnitValue.CreatePointValue(250));
                formSigField.SetProperty(Property.HEIGHT, UnitValue.CreatePointValue(150));
                formSigField.SetContent("name", "description");
                formSigField.SetFontSize(20);
                formSigField.SetFontColor(ColorConstants.BLUE);
                formSigField.SetBackgroundColor(ColorConstants.YELLOW);
                formSigField.SetBorder(new SolidBorder(ColorConstants.GREEN, 1));
                formSigField.SetProperty(FormProperty.FORM_FIELD_FLATTEN, true);
                document.Add(formSigField);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        public virtual void SigFieldWithNameAndDescriptionModeVerticalTest() {
            String outPdf = DESTINATION_FOLDER + "sigFieldWithNameAndDescriptionModeVertical.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_sigFieldWithNameAndDescriptionModeVertical.pdf";
            using (Document document = new Document(new PdfDocument(new PdfWriter(outPdf)))) {
                SignatureFieldAppearance formSigField = new SignatureFieldAppearance("SigField");
                formSigField.SetProperty(Property.WIDTH, UnitValue.CreatePointValue(150));
                formSigField.SetProperty(Property.HEIGHT, UnitValue.CreatePointValue(250));
                formSigField.SetContent("name", "description");
                formSigField.SetFontSize(20);
                formSigField.SetFontColor(ColorConstants.BLUE);
                formSigField.SetBackgroundColor(ColorConstants.YELLOW);
                formSigField.SetBorder(new SolidBorder(ColorConstants.GREEN, 1));
                formSigField.SetProperty(FormProperty.FORM_FIELD_FLATTEN, true);
                document.Add(formSigField);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        public virtual void BorderBoxesTest() {
            String outPdf = DESTINATION_FOLDER + "borderBoxes.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_borderBoxes.pdf";
            using (Document document = new Document(new PdfDocument(new PdfWriter(outPdf)))) {
                // BORDER_BOX
                SignatureFieldAppearance interactiveSigField = new SignatureFieldAppearance("interactiveSigField").SetBorder
                    (new SolidBorder(ColorConstants.PINK, 10));
                interactiveSigField.SetWidth(200).SetHeight(100);
                interactiveSigField.SetInteractive(true);
                interactiveSigField.SetContent("interactive border box");
                interactiveSigField.SetProperty(Property.BOX_SIZING, BoxSizingPropertyValue.BORDER_BOX);
                document.Add(interactiveSigField);
                // CONTENT_BOX
                SignatureFieldAppearance interactiveSigField2 = new SignatureFieldAppearance("interactiveSigField2").SetBorder
                    (new SolidBorder(ColorConstants.YELLOW, 10));
                interactiveSigField2.SetWidth(200).SetHeight(100);
                interactiveSigField2.SetInteractive(true);
                interactiveSigField2.SetContent("interactive content box");
                interactiveSigField2.SetProperty(Property.BOX_SIZING, BoxSizingPropertyValue.CONTENT_BOX);
                document.Add(interactiveSigField2);
                // BORDER_BOX
                SignatureFieldAppearance flattenSigField = new SignatureFieldAppearance("flattenSigField").SetBorder(new SolidBorder
                    (ColorConstants.PINK, 10));
                flattenSigField.SetWidth(200).SetHeight(100);
                flattenSigField.SetInteractive(false);
                flattenSigField.SetContent("flatten border box");
                flattenSigField.SetProperty(Property.BOX_SIZING, BoxSizingPropertyValue.BORDER_BOX);
                document.Add(flattenSigField);
                // CONTENT_BOX
                SignatureFieldAppearance flattenSigField2 = new SignatureFieldAppearance("flattenSigField2").SetBorder(new 
                    SolidBorder(ColorConstants.YELLOW, 10));
                flattenSigField2.SetWidth(200).SetHeight(100);
                flattenSigField2.SetInteractive(false);
                flattenSigField2.SetContent("flatten content box");
                flattenSigField2.SetProperty(Property.BOX_SIZING, BoxSizingPropertyValue.CONTENT_BOX);
                document.Add(flattenSigField2);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        public virtual void BorderTypesTest() {
            String outPdf = DESTINATION_FOLDER + "borderTypes.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_borderTypes.pdf";
            using (Document document = new Document(new PdfDocument(new PdfWriter(outPdf)))) {
                // DASHED
                SignatureFieldAppearance sigField = new SignatureFieldAppearance("SigField").SetBorder(new DashedBorder(ColorConstants
                    .PINK, 10)).SetBackgroundColor(ColorConstants.YELLOW);
                sigField.SetSize(100);
                sigField.SetInteractive(true);
                sigField.SetContent("dashed");
                sigField.SetProperty(Property.MARGIN_BOTTOM, UnitValue.CreatePointValue(30));
                document.Add(sigField);
                PdfDictionary bs = new PdfDictionary();
                // UNDERLINE
                bs.Put(PdfName.S, PdfAnnotation.STYLE_UNDERLINE);
                SignatureFieldAppearance sigField2 = new SignatureFieldAppearance("SigField2").SetBorder(FormBorderFactory
                    .GetBorder(bs, 10f, ColorConstants.YELLOW, ColorConstants.ORANGE)).SetBackgroundColor(ColorConstants.PINK
                    );
                sigField2.SetSize(100);
                sigField2.SetInteractive(true);
                sigField2.SetContent("underline");
                sigField2.SetFontSize(18);
                sigField2.SetProperty(Property.MARGIN_BOTTOM, UnitValue.CreatePointValue(30));
                document.Add(sigField2);
                // INSET
                bs.Put(PdfName.S, PdfAnnotation.STYLE_INSET);
                SignatureFieldAppearance sigField3 = new SignatureFieldAppearance("SigField3").SetBorder(FormBorderFactory
                    .GetBorder(bs, 10f, ColorConstants.PINK, ColorConstants.RED)).SetBackgroundColor(ColorConstants.YELLOW
                    );
                sigField3.SetSize(100);
                sigField3.SetInteractive(true);
                sigField3.SetContent("inset");
                sigField3.SetProperty(Property.MARGIN_BOTTOM, UnitValue.CreatePointValue(30));
                document.Add(sigField3);
                // BEVELLED
                bs.Put(PdfName.S, PdfAnnotation.STYLE_BEVELED);
                SignatureFieldAppearance sigField4 = new SignatureFieldAppearance("SigField4").SetBorder(FormBorderFactory
                    .GetBorder(bs, 10f, ColorConstants.YELLOW, ColorConstants.ORANGE)).SetBackgroundColor(ColorConstants.PINK
                    );
                sigField4.SetSize(100);
                sigField4.SetInteractive(true);
                sigField4.SetContent("bevelled");
                sigField4.SetFontSize(18);
                document.Add(sigField4);
                PdfFormCreator.GetAcroForm(document.GetPdfDocument(), false).FlattenFields();
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        public virtual void FontSizeTest() {
            String outPdf = DESTINATION_FOLDER + "fontSizeTest.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_fontSizeTest.pdf";
            using (Document document = new Document(new PdfDocument(new PdfWriter(outPdf)))) {
                SignatureFieldAppearance sigField = new SignatureFieldAppearance("SigField");
                sigField.SetFontSize(20);
                sigField.SetContent("test");
                document.Add(sigField);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        public virtual void FontNullCustomCheck() {
            String outPdf = DESTINATION_FOLDER + "fontNullCustomCheck.pdf";
            PdfDocument pdfDoc = new _PdfDocument_412(new PdfWriter(outPdf));
            Document document = new Document(pdfDoc);
            SignatureFieldAppearance sigField = new SignatureFieldAppearance("SigField");
            sigField.SetContent("test");
            sigField.SetInteractive(true);
            sigField.SetBorder(new SolidBorder(ColorConstants.GREEN, 1));
            Exception e = NUnit.Framework.Assert.Catch(typeof(InvalidOperationException), () => document.Add(sigField)
                );
            NUnit.Framework.Assert.AreEqual(LayoutExceptionMessageConstant.INVALID_FONT_PROPERTY_VALUE, e.Message);
        }

        private sealed class _PdfDocument_412 : PdfDocument {
            public _PdfDocument_412(PdfWriter baseArg1)
                : base(baseArg1) {
            }

            public override PdfFont GetDefaultFont() {
                return null;
            }
        }

        [NUnit.Framework.Test]
        public virtual void SignatureOnRotatedPagesTest() {
            String outPdf = DESTINATION_FOLDER + "signatureOnRotatedPages.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_signatureOnRotatedPages.pdf";
            using (Document document = new Document(new PdfDocument(new PdfWriter(outPdf)))) {
                document.GetPdfDocument().AddNewPage().SetRotation(90);
                document.GetPdfDocument().AddNewPage().SetRotation(180);
                document.GetPdfDocument().AddNewPage().SetRotation(270);
                PdfAcroForm form = PdfFormCreator.GetAcroForm(document.GetPdfDocument(), true);
                PdfSignatureFormField field1 = new SignatureFormFieldBuilder(document.GetPdfDocument(), "sigField1").SetWidgetRectangle
                    (new Rectangle(50, 50, 400, 200)).SetPage(1).CreateSignature();
                PdfSignatureFormField field2 = new SignatureFormFieldBuilder(document.GetPdfDocument(), "sigField2").SetWidgetRectangle
                    (new Rectangle(50, 50, 400, 200)).SetPage(2).CreateSignature();
                PdfSignatureFormField field3 = new SignatureFormFieldBuilder(document.GetPdfDocument(), "sigField3").SetWidgetRectangle
                    (new Rectangle(50, 50, 400, 200)).SetPage(3).CreateSignature();
                SignatureFieldAppearance sigField1 = new SignatureFieldAppearance("sigField1");
                sigField1.SetContent("rotation 90 rotation 90 rotation 90 rotation 90 rotation 90");
                sigField1.SetFontSize(25);
                field1.GetFirstFormAnnotation().SetFormFieldElement(sigField1).SetBorderColor(ColorConstants.GREEN);
                SignatureFieldAppearance sigField2 = new SignatureFieldAppearance("sigField2");
                sigField2.SetContent("rotation 180 rotation 180 rotation 180 rotation 180 rotation 180");
                sigField2.SetFontSize(25);
                field2.GetFirstFormAnnotation().SetFormFieldElement(sigField2).SetBorderColor(ColorConstants.GREEN);
                SignatureFieldAppearance sigField3 = new SignatureFieldAppearance("sigField3");
                sigField3.SetContent("rotation 270 rotation 270 rotation 270 rotation 270 rotation 270");
                sigField3.SetFontSize(25);
                field3.GetFirstFormAnnotation().SetFormFieldElement(sigField3).SetBorderColor(ColorConstants.GREEN);
                form.AddField(field1);
                form.AddField(field2);
                form.AddField(field3);
                form.FlattenFields();
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        public virtual void CustomModeTest() {
            String outPdf = DESTINATION_FOLDER + "customModeTest.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_customModeTest.pdf";
            using (Document document = new Document(new PdfDocument(new PdfWriter(outPdf)))) {
                Div div = new Div();
                div.Add(new Paragraph("Paragraph inside div with red dashed border and pink background").SetBorder(new DashedBorder
                    (ColorConstants.RED, 1)).SetBackgroundColor(ColorConstants.PINK));
                Div flexContainer = new Div();
                flexContainer.SetProperty(Property.FLEX_WRAP, FlexWrapPropertyValue.WRAP);
                flexContainer.SetProperty(Property.FLEX_DIRECTION, FlexDirectionPropertyValue.ROW_REVERSE);
                flexContainer.SetProperty(Property.ALIGN_ITEMS, AlignmentPropertyValue.CENTER);
                flexContainer.Add(new iText.Layout.Element.Image(ImageDataFactory.Create(SOURCE_FOLDER + "1.png")).Scale(0.1f
                    , 0.3f).SetPadding(10)).Add(new List().Add(new ListItem("Flex container with").SetListSymbol(ListNumberingType
                    .ZAPF_DINGBATS_1)).Add(new ListItem("image and list,").SetListSymbol(ListNumberingType.ZAPF_DINGBATS_2
                    )).Add(new ListItem("wrap, row-reverse,").SetListSymbol(ListNumberingType.ZAPF_DINGBATS_3)).Add(new ListItem
                    ("green dots border").SetListSymbol(ListNumberingType.ZAPF_DINGBATS_4)).SetPadding(10)).SetBorder(new 
                    RoundDotsBorder(ColorConstants.GREEN, 10));
                flexContainer.SetNextRenderer(new FlexContainerRenderer(flexContainer));
                div.Add(flexContainer);
                SignatureFieldAppearance appearance = new SignatureFieldAppearance("form SigField");
                appearance.SetContent(div).SetFontColor(ColorConstants.WHITE).SetFontSize(10).SetBackgroundColor(ColorConstants
                    .DARK_GRAY).SetBorder(new SolidBorder(ColorConstants.MAGENTA, 2)).SetInteractive(true);
                document.Add(appearance);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        public virtual void FlattenEmptySignatureTest() {
            String srcPdf = SOURCE_FOLDER + "emptySignature.pdf";
            String outPdf = DESTINATION_FOLDER + "flattenEmptySignature.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_flattenEmptySignature.pdf";
            using (PdfDocument document = new PdfDocument(new PdfReader(srcPdf), new PdfWriter(outPdf))) {
                PdfAcroForm acroForm = PdfFormCreator.GetAcroForm(document, false);
                acroForm.FlattenFields();
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareVisually(outPdf, cmpPdf, DESTINATION_FOLDER, "diff_"
                ));
        }
    }
}
