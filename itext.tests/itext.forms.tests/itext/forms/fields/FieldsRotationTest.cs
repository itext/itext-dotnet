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
using System.Linq;
using iText.Commons.Utils;
using iText.Forms;
using iText.Forms.Form.Element;
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Layout;
using iText.Test;

namespace iText.Forms.Fields {
    [NUnit.Framework.Category("IntegrationTest")]
    [NUnit.Framework.TestFixtureSource("RotationRelatedPropertiesTestFixtureData")]
    public class FieldsRotationTest : ExtendedITextTest {
        public static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/forms/fields/FieldsRotationTest/";

        public static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/forms/fields/FieldsRotationTest/";

        private readonly int[] pageRotation;

        private readonly int[] fieldRotation;

        private readonly bool ignorePageRotation;

        private readonly String testName;

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(DESTINATION_FOLDER);
        }

        public FieldsRotationTest(Object pageRotation, Object fieldRotation, Object ignorePageRotation, Object testName
            ) {
            this.pageRotation = (int[])pageRotation;
            this.fieldRotation = (int[])fieldRotation;
            this.ignorePageRotation = (bool)ignorePageRotation;
            this.testName = (String)testName;
        }

        public FieldsRotationTest(Object[] array)
            : this(array[0], array[1], array[2], array[3]) {
        }

        public static IEnumerable<Object[]> RotationRelatedProperties() {
            return JavaUtil.ArraysAsList(new Object[][] { new Object[] { new int[] { 360, 90, 180, 270 }, new int[] { 
                0, 0, 0, 0 }, true, "fieldsOnRotatedPagesDefault" }, new Object[] { new int[] { 360, 90, 180, 270 }, new 
                int[] { 0, 0, 0, 0 }, false, "fieldsOnRotatedPages" }, new Object[] { new int[] { 0, 0, 0, 0 }, new int
                [] { 360, 90, 180, 270 }, true, "rotatedFieldsDefault" }, new Object[] { new int[] { 90, 90, 90, 90 }, 
                new int[] { 720, 90, 180, 270 }, true, "rotatedFieldsPage90Default" }, new Object[] { new int[] { 90, 
                90, 90, 90 }, new int[] { 0, -270, 180, -90 }, false, "rotatedFieldsPage90" }, new Object[] { new int[
                ] { 0, 90, 180, 270 }, new int[] { 0, 90, 180, 270 }, true, "rotatedFieldsOnRotatedPagesDefault" }, new 
                Object[] { new int[] { 0, 90, 180, 270 }, new int[] { 0, 90, 180, 270 }, false, "rotatedFieldsOnRotatedPages"
                 } });
        }

        public static ICollection<NUnit.Framework.TestFixtureData> RotationRelatedPropertiesTestFixtureData() {
            return RotationRelatedProperties().Select(array => new NUnit.Framework.TestFixtureData(array)).ToList();
        }

        [NUnit.Framework.Test]
        public virtual void FieldRotationTest() {
            String outFileName = DESTINATION_FOLDER + testName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + testName + ".pdf";
            FillForm(outFileName);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                , "diff"));
        }

        private void FillForm(String outPdf) {
            using (Document document = new Document(new PdfDocument(new PdfWriter(outPdf)))) {
                PdfAcroForm form = PdfFormCreator.GetAcroForm(document.GetPdfDocument(), true);
                for (int i = 1; i < 5; ++i) {
                    String caption = GenerateCaption(pageRotation[i - 1], fieldRotation[i - 1]);
                    document.GetPdfDocument().AddNewPage().SetRotation(pageRotation[i - 1]);
                    String buttonName = "button" + i;
                    PdfButtonFormField button = new PushButtonFormFieldBuilder(document.GetPdfDocument(), buttonName).SetWidgetRectangle
                        (new Rectangle(50, 570, 400, 200)).SetPage(i).CreatePushButton();
                    Button buttonField = new Button(buttonName);
                    buttonField.SetValue("button" + caption);
                    button.GetFirstFormAnnotation().SetFormFieldElement(buttonField).SetBorderColor(ColorConstants.GREEN).SetRotation
                        (fieldRotation[i - 1]);
                    form.AddField(button);
                    String textName = "text" + i;
                    PdfTextFormField text = new TextFormFieldBuilder(document.GetPdfDocument(), textName).SetWidgetRectangle(new 
                        Rectangle(50, 320, 400, 200)).SetPage(i).CreateText();
                    text.GetFirstFormAnnotation().SetBorderColor(ColorConstants.GREEN).SetRotation(fieldRotation[i - 1]);
                    form.AddField(text);
                    String signatureName = "signature" + i;
                    PdfSignatureFormField signature = new SignatureFormFieldBuilder(document.GetPdfDocument(), signatureName).
                        SetWidgetRectangle(new Rectangle(50, 70, 400, 200)).SetPage(i).CreateSignature();
                    SignatureFieldAppearance sigField = new SignatureFieldAppearance(signatureName).SetContent("signature" + caption
                        );
                    signature.SetIgnorePageRotation(ignorePageRotation).GetFirstFormAnnotation().SetFormFieldElement(sigField)
                        .SetBorderColor(ColorConstants.GREEN).SetRotation(fieldRotation[i - 1]);
                    form.AddField(signature);
                }
            }
        }

        private String GenerateCaption(int pageRotation, int fieldRotation) {
            String caption = ", page rotation: " + pageRotation + ", field rotation: " + fieldRotation;
            for (int i = 0; i < 3; ++i) {
                caption += caption;
            }
            return caption;
        }
    }
}
