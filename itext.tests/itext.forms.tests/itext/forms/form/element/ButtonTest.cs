/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
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
using iText.Forms.Form;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Layout;
using iText.Layout.Element;
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
                formButton.SetProperty(FormProperty.FORM_FIELD_VALUE, "form button");
                formButton.Add(new Paragraph("text to display"));
                document.Add(formButton);
                Button flattenButton = new Button("flatten button");
                flattenButton.SetProperty(FormProperty.FORM_FIELD_FLATTEN, true);
                flattenButton.SetProperty(FormProperty.FORM_FIELD_VALUE, "flatten button");
                formButton.Add(new Paragraph("text to display"));
                document.Add(flattenButton);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }
    }
}
