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
using iText.Forms.Fields;
using iText.Forms.Form;
using iText.Forms.Form.Element;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Tagging;
using iText.Kernel.Pdf.Tagutils;
using iText.Kernel.Utils;
using iText.Kernel.XMP;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Test;
using iText.Test.Pdfa;

namespace iText.Forms {
    // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf\a validation on Android)
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfUA2FormTest : ExtendedITextTest {
        public static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/forms/PdfUA2FormTest/";

        public static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/forms//PdfUA2FormTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void CheckFormFieldTest() {
            String outFile = DESTINATION_FOLDER + "formFieldTest.pdf";
            String cmpFile = SOURCE_FOLDER + "cmp_formFieldTest.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFile, new WriterProperties().SetPdfVersion
                (PdfVersion.PDF_2_0)))) {
                Document document = new Document(pdfDocument);
                PdfFont font = PdfFontFactory.CreateFont(SOURCE_FOLDER + "FreeSans.ttf", "WinAnsi", PdfFontFactory.EmbeddingStrategy
                    .FORCE_EMBEDDED);
                document.SetFont(font);
                CreateSimplePdfUA2Document(pdfDocument);
                PdfAcroForm form = PdfFormCreator.GetAcroForm(pdfDocument, true);
                Rectangle rect = new Rectangle(210, 490, 150, 22);
                PdfTextFormField field = new TextFormFieldBuilder(pdfDocument, "fieldName").SetWidgetRectangle(rect).CreateText
                    ();
                field.Put(PdfName.Contents, new PdfString("Description"));
                field.SetValue("some value");
                field.SetFont(font);
                form.AddField(field);
            }
            CompareAndValidate(outFile, cmpFile);
        }

        [NUnit.Framework.Test]
        public virtual void CheckTextAreaWithLabelTest() {
            String outFile = DESTINATION_FOLDER + "textAreaTest.pdf";
            String cmpFile = SOURCE_FOLDER + "cmp_textAreaTest.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFile, new WriterProperties().SetPdfVersion
                (PdfVersion.PDF_2_0)))) {
                Document document = new Document(pdfDocument);
                PdfFont font = PdfFontFactory.CreateFont(SOURCE_FOLDER + "FreeSans.ttf", "WinAnsi", PdfFontFactory.EmbeddingStrategy
                    .FORCE_EMBEDDED);
                document.SetFont(font);
                CreateSimplePdfUA2Document(pdfDocument);
                Paragraph paragraph = new Paragraph("Widget label").SetFont(font);
                paragraph.GetAccessibilityProperties().SetRole(StandardRoles.LBL);
                TextArea formTextArea = new TextArea("form text1");
                formTextArea.SetProperty(FormProperty.FORM_FIELD_FLATTEN, false);
                formTextArea.SetProperty(FormProperty.FORM_FIELD_VALUE, "form\ntext\narea");
                Div div = new Div();
                div.GetAccessibilityProperties().SetRole(StandardRoles.FORM);
                div.Add(paragraph);
                div.Add(formTextArea);
                document.Add(div);
            }
            CompareAndValidate(outFile, cmpFile);
        }

        [NUnit.Framework.Test]
        public virtual void CheckInputFieldTest() {
            String outFile = DESTINATION_FOLDER + "inputFieldTest.pdf";
            String cmpFile = SOURCE_FOLDER + "cmp_inputFieldTest.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFile, new WriterProperties().SetPdfVersion
                (PdfVersion.PDF_2_0)))) {
                Document document = new Document(pdfDocument);
                PdfFont font = PdfFontFactory.CreateFont(SOURCE_FOLDER + "FreeSans.ttf", "WinAnsi", PdfFontFactory.EmbeddingStrategy
                    .FORCE_EMBEDDED);
                document.SetFont(font);
                CreateSimplePdfUA2Document(pdfDocument);
                InputField formInputField = new InputField("form input field");
                formInputField.SetProperty(FormProperty.FORM_FIELD_FLATTEN, false);
                formInputField.SetProperty(FormProperty.FORM_FIELD_VALUE, "form input field");
                formInputField.SetProperty(FormProperty.FORM_FIELD_LABEL, "label form field");
                document.Add(formInputField);
                PdfAcroForm form = PdfFormCreator.GetAcroForm(pdfDocument, true);
                form.GetField("form input field").GetPdfObject().Put(PdfName.Contents, new PdfString("Description"));
            }
            CompareAndValidate(outFile, cmpFile);
        }

        [NUnit.Framework.Test]
        public virtual void CheckSignatureFormTest() {
            String outFile = DESTINATION_FOLDER + "signatureFormTest.pdf";
            String cmpFile = SOURCE_FOLDER + "cmp_signatureFormTest.pdf";
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(outFile, new WriterProperties().SetPdfVersion
                (PdfVersion.PDF_2_0)))) {
                Document document = new Document(pdfDocument);
                PdfFont font = PdfFontFactory.CreateFont(SOURCE_FOLDER + "FreeSans.ttf", "WinAnsi", PdfFontFactory.EmbeddingStrategy
                    .FORCE_EMBEDDED);
                document.SetFont(font);
                CreateSimplePdfUA2Document(pdfDocument);
                TagTreePointer tagPointer = pdfDocument.GetTagStructureContext().GetAutoTaggingPointer();
                tagPointer.AddTag(StandardRoles.FIGURE);
                tagPointer.GetProperties().SetAlternateDescription("Alt Description");
                SignatureFieldAppearance formSigField = new SignatureFieldAppearance("form SigField");
                formSigField.SetProperty(FormProperty.FORM_FIELD_FLATTEN, false);
                formSigField.SetContent("form SigField");
                formSigField.SetBorder(new SolidBorder(ColorConstants.YELLOW, 1));
                formSigField.SetFont(font);
                document.Add(formSigField);
                PdfAcroForm form = PdfFormCreator.GetAcroForm(pdfDocument, true);
                form.GetField("form SigField").GetPdfObject().Put(PdfName.Contents, new PdfString("Description"));
            }
            CompareAndValidate(outFile, cmpFile);
        }

        private void CreateSimplePdfUA2Document(PdfDocument pdfDocument) {
            byte[] bytes = File.ReadAllBytes(System.IO.Path.Combine(SOURCE_FOLDER + "simplePdfUA2.xmp"));
            XMPMeta xmpMeta = XMPMetaFactory.Parse(new MemoryStream(bytes));
            pdfDocument.SetXmpMetadata(xmpMeta);
            pdfDocument.SetTagged();
            pdfDocument.GetCatalog().SetViewerPreferences(new PdfViewerPreferences().SetDisplayDocTitle(true));
            pdfDocument.GetCatalog().SetLang(new PdfString("en-US"));
            PdfDocumentInfo info = pdfDocument.GetDocumentInfo();
            info.SetTitle("PdfUA2 Title");
        }

        private void CompareAndValidate(String outPdf, String cmpPdf) {
            NUnit.Framework.Assert.IsNull(new VeraPdfValidator().Validate(outPdf));
            // Android-Conversion-Skip-Line (TODO DEVSIX-7377 introduce pdf\a validation on Android)
            String result = new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER, "diff_");
            if (result != null) {
                NUnit.Framework.Assert.Fail(result);
            }
        }
    }
}
