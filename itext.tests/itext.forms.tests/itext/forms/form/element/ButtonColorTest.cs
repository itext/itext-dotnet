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
using iText.Forms.Form;
using iText.Kernel.Colors;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Test;

namespace iText.Forms.Form.Element {
    [NUnit.Framework.Category("IntegrationTest")]
    public class ButtonColorTest : ExtendedITextTest {
        public static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/forms/form/element/ButtonColorTest/";

        public static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/forms/form/element/ButtonColorTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void ButtonsWithColorTest() {
            String outPdf = DESTINATION_FOLDER + "buttonsWithColor.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_buttonsWithColor.pdf";
            DrawButtons(outPdf, cmpPdf, ColorConstants.RED);
        }

        [NUnit.Framework.Test]
        public virtual void ButtonsWithoutColorTest() {
            String outPdf = DESTINATION_FOLDER + "buttonsWithoutColor.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_buttonsWithoutColor.pdf";
            DrawButtons(outPdf, cmpPdf, null);
        }

        private static void DrawButtons(String outPdf, String cmpPdf, Color color) {
            using (PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new FileStream(outPdf, FileMode.Create)))) {
                using (Document document = new Document(pdfDocument)) {
                    Button button = new Button("button");
                    button.Add(new Paragraph("button child"));
                    InputButton inputButton = new InputButton("input button");
                    button.SetProperty(FormProperty.FORM_FIELD_FLATTEN, false);
                    inputButton.SetProperty(FormProperty.FORM_FIELD_FLATTEN, false);
                    button.SetProperty(FormProperty.FORM_FIELD_VALUE, "button value");
                    inputButton.SetProperty(FormProperty.FORM_FIELD_VALUE, "input button value");
                    button.SetProperty(Property.FONT_COLOR, color == null ? null : new TransparentColor(color));
                    inputButton.SetProperty(Property.BACKGROUND, color == null ? null : new Background(color));
                    document.Add(button);
                    document.Add(inputButton);
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }
    }
}
