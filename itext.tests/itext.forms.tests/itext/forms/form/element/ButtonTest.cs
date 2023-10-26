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
using System.IO;
using iText.Forms;
using iText.Forms.Fields;
using iText.Forms.Fields.Borders;
using iText.Forms.Form;
using iText.Forms.Form.Renderer;
using iText.IO.Font.Constants;
using iText.IO.Image;
using iText.IO.Util;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Annot;
using iText.Kernel.Pdf.Xobject;
using iText.Kernel.Utils;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Test;

namespace iText.Forms.Form.Element {
    [NUnit.Framework.Category("IntegrationTest")]
    public class ButtonTest : ExtendedITextTest {
        public static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/forms/form/element/ButtonTest/";

        public static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/forms/form/element/ButtonTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void BasicButtonTest() {
            String outPdf = DESTINATION_FOLDER + "basicButton.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_basicButton.pdf";
            using (Document document = new Document(new PdfDocument(new PdfWriter(outPdf)))) {
                Button formButton = new Button("form button");
                formButton.SetProperty(FormProperty.FORM_FIELD_FLATTEN, false);
                formButton.Add(new Paragraph("form button"));
                formButton.Add(new Paragraph("paragraph with yellow border inside button").SetBorder(new SolidBorder(ColorConstants
                    .YELLOW, 1)));
                document.Add(formButton);
                document.Add(new Paragraph(""));
                Button flattenButton = new Button("flatten button");
                flattenButton.SetProperty(FormProperty.FORM_FIELD_FLATTEN, true);
                flattenButton.Add(new Paragraph("flatten button"));
                flattenButton.Add(new Paragraph("paragraph with pink border inside button").SetBorder(new SolidBorder(ColorConstants
                    .PINK, 1)));
                document.Add(flattenButton);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        public virtual void BasicButtonTestWithFontDiffersOnParagraph() {
            String outPdf = DESTINATION_FOLDER + "basicButtonWithFontDiffersOnParagraph.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_basicButtonWithFontDiffersOnParagraph.pdf";
            using (Document document = new Document(new PdfDocument(new PdfWriter(outPdf)))) {
                Button formButton = new Button("form button");
                formButton.SetProperty(FormProperty.FORM_FIELD_FLATTEN, false);
                formButton.SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD));
                formButton.Add(new Paragraph("form button"));
                formButton.Add(new Paragraph("paragraph with yellow border inside button").SetFont(PdfFontFactory.CreateFont
                    (StandardFonts.COURIER)).SetBorder(new SolidBorder(ColorConstants.YELLOW, 1)));
                document.Add(formButton);
                document.Add(new Paragraph(""));
                Button flattenButton = new Button("flatten button");
                flattenButton.SetProperty(FormProperty.FORM_FIELD_FLATTEN, true);
                flattenButton.Add(new Paragraph("flatten button"));
                flattenButton.SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD));
                flattenButton.Add(new Paragraph("paragraph with pink border inside button").SetFont(PdfFontFactory.CreateFont
                    (StandardFonts.COURIER)).SetBorder(new SolidBorder(ColorConstants.PINK, 1)));
                document.Add(flattenButton);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        public virtual void BasicButtonTestWithFont() {
            String outPdf = DESTINATION_FOLDER + "basicButtonWithFont.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_basicButtonWithFon.pdf";
            using (Document document = new Document(new PdfDocument(new PdfWriter(outPdf)))) {
                Button formButton = new Button("form button");
                formButton.SetProperty(FormProperty.FORM_FIELD_FLATTEN, false);
                formButton.SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD));
                formButton.Add(new Paragraph("form button"));
                formButton.Add(new Paragraph("paragraph with yellow border inside button").SetBorder(new SolidBorder(ColorConstants
                    .YELLOW, 1)));
                document.Add(formButton);
                document.Add(new Paragraph(""));
                Button flattenButton = new Button("flatten button");
                flattenButton.SetProperty(FormProperty.FORM_FIELD_FLATTEN, true);
                flattenButton.Add(new Paragraph("flatten button"));
                flattenButton.SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD));
                flattenButton.Add(new Paragraph("paragraph with pink border inside button").SetBorder(new SolidBorder(ColorConstants
                    .PINK, 1)));
                document.Add(flattenButton);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        public virtual void CustomizedButtonTest() {
            String outPdf = DESTINATION_FOLDER + "customizedButton.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_customizedButton.pdf";
            using (Document document = new Document(new PdfDocument(new PdfWriter(outPdf)))) {
                Button formButton = new Button("form button");
                formButton.SetProperty(FormProperty.FORM_FIELD_FLATTEN, false);
                formButton.SetValue("form button");
                formButton.SetFontColor(ColorConstants.BLUE);
                formButton.SetBackgroundColor(ColorConstants.YELLOW);
                formButton.SetBorder(new SolidBorder(ColorConstants.GREEN, 2));
                document.Add(formButton);
                document.Add(new Paragraph(""));
                Button flattenButton = new Button("flatten  button");
                flattenButton.SetProperty(FormProperty.FORM_FIELD_FLATTEN, true);
                flattenButton.SetValue("flatten button");
                flattenButton.SetFontColor(ColorConstants.BLUE);
                flattenButton.SetBackgroundColor(ColorConstants.YELLOW);
                flattenButton.SetBorder(new SolidBorder(ColorConstants.GREEN, 2));
                document.Add(flattenButton);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        public virtual void ButtonVerticalAlignmentTest() {
            String outPdf = DESTINATION_FOLDER + "buttonVerticalAlignment.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_buttonVerticalAlignment.pdf";
            using (Document document = new Document(new PdfDocument(new PdfWriter(outPdf)))) {
                Button formButton = new Button("form button");
                formButton.SetProperty(FormProperty.FORM_FIELD_FLATTEN, false);
                formButton.SetValue("capture on bottom");
                formButton.SetProperty(Property.VERTICAL_ALIGNMENT, VerticalAlignment.BOTTOM);
                formButton.SetProperty(Property.HEIGHT, UnitValue.CreatePointValue(100));
                document.Add(formButton);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        public virtual void AddButtonInTwoWaysTest() {
            String outPdf = DESTINATION_FOLDER + "addButtonInTwoWays.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_addButtonInTwoWays.pdf";
            String imagePath = SOURCE_FOLDER + "Desert.jpg";
            using (Document document = new Document(new PdfDocument(new PdfWriter(outPdf)))) {
                // Create push button using html element
                Button formButton = new Button("button");
                formButton.SetProperty(FormProperty.FORM_FIELD_FLATTEN, false);
                formButton.SetProperty(Property.WIDTH, UnitValue.CreatePointValue(100));
                formButton.SetProperty(Property.HEIGHT, UnitValue.CreatePointValue(100));
                formButton.Add(new Image(new PdfImageXObject(ImageDataFactory.Create(StreamUtil.InputStreamToArray(new FileStream
                    (imagePath, FileMode.Open, FileAccess.Read))))).SetWidth(98).SetHeight(98));
                formButton.SetFontColor(ColorConstants.BLUE);
                formButton.SetBackgroundColor(ColorConstants.YELLOW);
                formButton.SetBorder(new SolidBorder(ColorConstants.GREEN, 1));
                document.Add(formButton);
                // Create push button using form field
                PdfAcroForm form = PdfFormCreator.GetAcroForm(document.GetPdfDocument(), true);
                PdfButtonFormField button = new PushButtonFormFieldBuilder(document.GetPdfDocument(), "push").SetWidgetRectangle
                    (new Rectangle(36, 600, 100, 100)).CreatePushButton();
                button.SetImage(imagePath);
                button.GetFirstFormAnnotation().SetBorderWidth(1).SetBorderColor(ColorConstants.MAGENTA).SetBackgroundColor
                    (ColorConstants.PINK).SetVisibility(PdfFormAnnotation.VISIBLE);
                form.AddField(button);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        public virtual void BorderBoxesTest() {
            String outPdf = DESTINATION_FOLDER + "borderBoxes.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_borderBoxes.pdf";
            using (Document document = new Document(new PdfDocument(new PdfWriter(outPdf)))) {
                // BORDER_BOX
                Button interactiveButton = new Button("interactiveButton").SetBorder(new SolidBorder(ColorConstants.PINK, 
                    10));
                interactiveButton.SetWidth(200);
                interactiveButton.SetInteractive(true);
                interactiveButton.SetValue("interactive border box");
                interactiveButton.SetProperty(Property.BOX_SIZING, BoxSizingPropertyValue.BORDER_BOX);
                document.Add(interactiveButton);
                // CONTENT_BOX
                Button interactiveButton2 = new Button("interactiveButton").SetBorder(new SolidBorder(ColorConstants.YELLOW
                    , 10));
                interactiveButton2.SetWidth(200);
                interactiveButton2.SetInteractive(true);
                interactiveButton2.SetValue("interactive content box");
                interactiveButton2.SetProperty(Property.BOX_SIZING, BoxSizingPropertyValue.CONTENT_BOX);
                document.Add(interactiveButton2);
                // BORDER_BOX
                Button flattenButton = new Button("flattenButton").SetBorder(new SolidBorder(ColorConstants.PINK, 10));
                flattenButton.SetWidth(200);
                flattenButton.SetInteractive(false);
                flattenButton.SetValue("flatten border box");
                flattenButton.SetProperty(Property.BOX_SIZING, BoxSizingPropertyValue.BORDER_BOX);
                document.Add(flattenButton);
                // CONTENT_BOX
                Button flattenButton2 = new Button("flattenButton").SetBorder(new SolidBorder(ColorConstants.YELLOW, 10));
                flattenButton2.SetWidth(200);
                flattenButton2.SetInteractive(false);
                flattenButton2.SetValue("flatten content box");
                flattenButton2.SetProperty(Property.BOX_SIZING, BoxSizingPropertyValue.CONTENT_BOX);
                document.Add(flattenButton2);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        public virtual void BorderTypesTest() {
            String outPdf = DESTINATION_FOLDER + "borderTypes.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_borderTypes.pdf";
            using (Document document = new Document(new PdfDocument(new PdfWriter(outPdf)))) {
                // DASHED
                Button button = new Button("button").SetBorder(new DashedBorder(ColorConstants.PINK, 10)).SetBackgroundColor
                    (ColorConstants.YELLOW);
                button.SetWidth(100);
                button.SetInteractive(true);
                button.SetValue("dashed");
                document.Add(button);
                PdfDictionary bs = new PdfDictionary();
                // UNDERLINE
                bs.Put(PdfName.S, PdfAnnotation.STYLE_UNDERLINE);
                Button button2 = new Button("button2").SetBorder(FormBorderFactory.GetBorder(bs, 10f, ColorConstants.YELLOW
                    , ColorConstants.ORANGE)).SetBackgroundColor(ColorConstants.PINK);
                button2.SetSize(100);
                button2.SetInteractive(true);
                button2.SetValue("underline");
                document.Add(button2);
                // INSET
                bs.Put(PdfName.S, PdfAnnotation.STYLE_INSET);
                Button button3 = new Button("button3").SetBorder(FormBorderFactory.GetBorder(bs, 10f, ColorConstants.PINK, 
                    ColorConstants.RED)).SetBackgroundColor(ColorConstants.YELLOW);
                button3.SetSize(100);
                button3.SetInteractive(true);
                button3.SetValue("inset");
                document.Add(button3);
                // BEVELLED
                bs.Put(PdfName.S, PdfAnnotation.STYLE_BEVELED);
                Button button4 = new Button("button4").SetBorder(FormBorderFactory.GetBorder(bs, 10f, ColorConstants.YELLOW
                    , ColorConstants.ORANGE)).SetBackgroundColor(ColorConstants.PINK);
                button4.SetSize(100);
                button4.SetInteractive(true);
                button4.SetValue("bevelled");
                document.Add(button4);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        public virtual void IsFlattenTest() {
            Button button = new Button("button");
            button.SetProperty(FormProperty.FORM_FIELD_FLATTEN, false);
            ButtonRenderer buttonRenderer = new ButtonRenderer(button);
            NUnit.Framework.Assert.IsFalse(buttonRenderer.IsFlatten());
            button.SetProperty(FormProperty.FORM_FIELD_FLATTEN, true);
            NUnit.Framework.Assert.IsTrue(buttonRenderer.IsFlatten());
            InputField inputField = new InputField("input");
            inputField.SetProperty(FormProperty.FORM_FIELD_FLATTEN, false);
            button.Add(inputField);
            buttonRenderer = (ButtonRenderer)button.CreateRendererSubTree();
            NUnit.Framework.Assert.IsTrue(((InputFieldRenderer)buttonRenderer.GetChildRenderers()[0].SetParent(buttonRenderer
                )).IsFlatten());
        }
    }
}
