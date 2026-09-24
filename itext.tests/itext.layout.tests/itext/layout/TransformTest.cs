/*
This file is part of the iText (R) project.
Copyright (c) 1998-2026 Apryse Group NV
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
using iText.Commons.Utils;
using iText.IO.Image;
using iText.IO.Util;
using iText.Kernel.Colors;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Xobject;
using iText.Kernel.Utils;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Layout.Testutil;
using iText.Test;

namespace iText.Layout {
    [NUnit.Framework.Category("IntegrationTest")]
    public class TransformTest : ExtendedITextTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/TransformTest/";

        private static readonly String DESTINATION_FOLDER = TestUtil.GetOutputPath() + "/layout/TransformTest/";

        private static IEnumerable<Object[]> Transforms() {
            return JavaUtil.ArraysAsList(new Object[][] { new Object[] { new Transform(), "noOp" }, new Object[] { new 
                Transform().AddTransform(new Transform.SingleTransform()), "noOp2" }, new Object[] { new Transform(1, 
                0, 0, (float)Math.Cos(MathUtil.ToRadians(60)), UnitValue.CreatePointValue(0), UnitValue.CreatePointValue
                (0)), "manualRotateX" }, new Object[] { new Transform().Translate(30, 0).Translate(UnitValue.CreatePercentValue
                (0), UnitValue.CreatePercentValue(30)), "translate" }, new Object[] { new Transform().Rotate((float)Math
                .PI / 4), "rotateCenter" }, new Object[] { new Transform().Rotate((float)-Math.PI / 4), "rotateClockWiseCenter"
                 }, new Object[] { new Transform().Rotate((float)Math.PI / 4, UnitValue.CreatePercentValue(-50), UnitValue
                .CreatePercentValue(-50)), "rotateBottomLeft" }, new Object[] { new Transform().Rotate((float)Math.PI 
                / 4, UnitValue.CreatePercentValue(50), UnitValue.CreatePercentValue(50)), "rotateTopRight" }, new Object
                [] { new Transform().Rotate((float)Math.PI / 4, -100, 56), "rotateImageTopLeft" }, new Object[] { new 
                Transform().ScaleX(2), "scaleX" }, new Object[] { new Transform().ScaleX(0), "scaleX0" }, new Object[]
                 { new Transform().ScaleX(0.5f).ScaleY(0.5f), "scaleY" }, new Object[] { new Transform().ScaleX(-0.5f)
                .ScaleY(0.5f), "scaleYNegative" }, new Object[] { new Transform().SkewX((float)Math.PI / 4), "skewX" }
                , new Object[] { new Transform().SkewY((float)-Math.PI / 4), "skewY" }, new Object[] { new Transform()
                .SimulateRotateX((float)Math.PI * 2 / 3), "rotateX" }, new Object[] { new Transform().SimulateRotateY(
                (float)Math.PI / 3), "rotateY" }, new Object[] { new Transform().SimulateRotateX((float)Math.PI), "rotateX180"
                 }, new Object[] { new Transform().SimulateRotateY((float)Math.PI), "rotateY180" }, new Object[] { new 
                Transform().SimulateRotateX((float)Math.PI * 2), "rotateX360" }, new Object[] { new Transform().SimulateRotateY
                ((float)Math.PI * 2), "rotateY360" }, new Object[] { new Transform().SimulateRotateX((float)Math.PI / 
                2), "rotateX90" }, new Object[] { new Transform().SimulateRotateY((float)Math.PI / 2), "rotateY90" } }
                );
        }

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.OneTimeTearDown]
        public static void AfterClass() {
            CompareTool.Cleanup(DESTINATION_FOLDER);
        }

        [NUnit.Framework.TestCaseSource("Transforms")]
        public virtual void CommonTransformTest(Transform transform, String fileName) {
            String outFileName = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            using (PdfDocument pdfDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                using (Document doc = new Document(pdfDoc)) {
                    doc.Add(new Paragraph(TestResourceUtil.GetByronStanza()));
                    doc.Add(new Paragraph(TestResourceUtil.GetByronStanza()).SetBackgroundColor(ColorConstants.GREEN).SetTransform
                        (transform));
                    PdfImageXObject xObject = new PdfImageXObject(ImageDataFactory.Create(SOURCE_FOLDER + "itis.jpg"));
                    iText.Layout.Element.Image image = new iText.Layout.Element.Image(xObject, 200);
                    image.SetTransform(transform);
                    doc.Add(image);
                    doc.Add(new Paragraph(TestResourceUtil.GetByronStanza()));
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                ));
        }

        [NUnit.Framework.Test]
        public virtual void TransformStyleTest() {
            String outFileName = DESTINATION_FOLDER + "transformStyle.pdf";
            String cmpFileName = SOURCE_FOLDER + "cmp_transformStyle.pdf";
            using (PdfDocument pdfDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(outFileName))) {
                using (Document doc = new Document(pdfDoc)) {
                    doc.Add(new Paragraph(TestResourceUtil.GetByronStanza()));
                    Table table = new Table(3);
                    for (int i = 1; i <= 12; i++) {
                        table.AddCell(JavaUtil.IntegerToString(i));
                    }
                    Style style = new Style().SetTransform(new Transform().Rotate(0.7f));
                    table.AddStyle(style);
                    doc.Add(table);
                    doc.Add(new Paragraph(TestResourceUtil.GetByronStanza()));
                }
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, DESTINATION_FOLDER
                ));
        }
    }
}
