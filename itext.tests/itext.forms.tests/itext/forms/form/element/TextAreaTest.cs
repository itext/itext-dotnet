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
using iText.Layout.Borders;
using iText.Layout.Properties;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Forms.Form.Element {
    [NUnit.Framework.Category("IntegrationTest")]
    public class TextAreaTest : ExtendedITextTest {
        public static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/forms/form/element/TextAreaTest/";

        public static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/forms/form/element/TextAreaTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void BasicTextAreaTest() {
            String outPdf = DESTINATION_FOLDER + "basicTextArea.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_basicTextArea.pdf";
            using (Document document = new Document(new PdfDocument(new PdfWriter(outPdf)))) {
                TextArea formTextArea = new TextArea("form text area");
                formTextArea.SetProperty(FormProperty.FORM_FIELD_FLATTEN, false);
                formTextArea.SetProperty(FormProperty.FORM_FIELD_VALUE, "form\ntext\narea");
                document.Add(formTextArea);
                TextArea flattenTextArea = new TextArea("flatten text area");
                flattenTextArea.SetProperty(FormProperty.FORM_FIELD_FLATTEN, true);
                flattenTextArea.SetProperty(FormProperty.FORM_FIELD_VALUE, "flatten\ntext\narea");
                document.Add(flattenTextArea);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.PROPERTY_IN_PERCENTS_NOT_SUPPORTED, Count = 16)]
        public virtual void PercentFontTextAreaTest() {
            String outPdf = DESTINATION_FOLDER + "percentFontTextArea.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_percentFontTextArea.pdf";
            using (Document document = new Document(new PdfDocument(new PdfWriter(outPdf)))) {
                TextArea formTextArea = new TextArea("form text area");
                formTextArea.SetProperty(FormProperty.FORM_FIELD_FLATTEN, false);
                formTextArea.SetProperty(FormProperty.FORM_FIELD_VALUE, "form\ntext\narea");
                formTextArea.SetProperty(Property.FONT_SIZE, UnitValue.CreatePercentValue(10));
                document.Add(formTextArea);
                TextArea flattenTextArea = new TextArea("flatten text area");
                flattenTextArea.SetProperty(FormProperty.FORM_FIELD_FLATTEN, true);
                flattenTextArea.SetProperty(FormProperty.FORM_FIELD_VALUE, "flatten\ntext\narea");
                formTextArea.SetProperty(Property.FONT_SIZE, UnitValue.CreatePercentValue(10));
                document.Add(flattenTextArea);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        public virtual void HeightTextAreaTest() {
            String outPdf = DESTINATION_FOLDER + "heightTextArea.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_heightTextArea.pdf";
            using (Document document = new Document(new PdfDocument(new PdfWriter(outPdf)))) {
                TextArea flattenTextArea = new TextArea("flatten text area with height");
                flattenTextArea.SetProperty(FormProperty.FORM_FIELD_FLATTEN, true);
                flattenTextArea.SetProperty(FormProperty.FORM_FIELD_VALUE, "flatten\ntext area\nwith height");
                flattenTextArea.SetProperty(Property.HEIGHT, new UnitValue(UnitValue.POINT, 100));
                flattenTextArea.SetProperty(Property.BORDER, new SolidBorder(2f));
                document.Add(flattenTextArea);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        public virtual void MinHeightTextAreaTest() {
            String outPdf = DESTINATION_FOLDER + "minHeightTextArea.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_minHeightTextArea.pdf";
            using (Document document = new Document(new PdfDocument(new PdfWriter(outPdf)))) {
                TextArea flattenTextArea = new TextArea("flatten text area with height");
                flattenTextArea.SetProperty(FormProperty.FORM_FIELD_FLATTEN, true);
                flattenTextArea.SetProperty(FormProperty.FORM_FIELD_VALUE, "flatten\ntext area\nwith height");
                flattenTextArea.SetProperty(Property.MIN_HEIGHT, new UnitValue(UnitValue.POINT, 100));
                flattenTextArea.SetProperty(Property.BORDER, new SolidBorder(2f));
                document.Add(flattenTextArea);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }

        [NUnit.Framework.Test]
        public virtual void MaxHeightTextAreaTest() {
            String outPdf = DESTINATION_FOLDER + "maxHeightTextArea.pdf";
            String cmpPdf = SOURCE_FOLDER + "cmp_maxHeightTextArea.pdf";
            using (Document document = new Document(new PdfDocument(new PdfWriter(outPdf)))) {
                TextArea flattenTextArea = new TextArea("flatten text area with height");
                flattenTextArea.SetProperty(FormProperty.FORM_FIELD_FLATTEN, true);
                flattenTextArea.SetProperty(FormProperty.FORM_FIELD_VALUE, "flatten\ntext area\nwith height");
                flattenTextArea.SetProperty(Property.MAX_HEIGHT, new UnitValue(UnitValue.POINT, 28));
                flattenTextArea.SetProperty(Property.BORDER, new SolidBorder(2f));
                document.Add(flattenTextArea);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdf, cmpPdf, DESTINATION_FOLDER));
        }
    }
}
