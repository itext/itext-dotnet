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
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Svg;
using iText.Svg.Renderers;
using iText.Test;

namespace iText.Svg.Renderers.Impl {
    [NUnit.Framework.Category("IntegrationTest")]
    public class EllipseSvgNodeRendererIntegrationTest : SvgIntegrationTest {
        public static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/svg/renderers/impl/EllipseSvgNodeRendererIntegrationTest/";

        public static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/svg/renderers/impl/EllipseSvgNodeRendererIntegrationTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            ITextTest.CreateDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void BasicEllipseTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "basicEllipse");
        }

        [NUnit.Framework.Test]
        public virtual void EllipseCxCyAbsentTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "ellipseCxCyAbsent");
        }

        [NUnit.Framework.Test]
        public virtual void EllipseCxAbsentTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "ellipseCxAbsent");
        }

        [NUnit.Framework.Test]
        public virtual void EllipseCxNegativeTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "ellipseCxNegative");
        }

        [NUnit.Framework.Test]
        public virtual void EllipseCyNegativeTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "ellipseCyNegative");
        }

        [NUnit.Framework.Test]
        public virtual void EllipseCyAbsentTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "ellipseCyAbsent");
        }

        [NUnit.Framework.Test]
        public virtual void EllipseRxAbsentTest() {
            //TODO: update cmp_ when DEVSIX-3119
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "ellipseRxAbsent");
        }

        [NUnit.Framework.Test]
        public virtual void EllipseRyAbsentTest() {
            //TODO: update cmp_ when DEVSIX-3119
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "ellipseRyAbsent");
        }

        [NUnit.Framework.Test]
        public virtual void EllipseRxNegativeTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "ellipseRxNegative");
        }

        [NUnit.Framework.Test]
        public virtual void EllipseRyNegativeTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "ellipseRyNegative");
        }

        [NUnit.Framework.Test]
        public virtual void EllipseTranslatedTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "ellipseTranslated");
        }

        [NUnit.Framework.Test]
        public virtual void EllipseRotatedTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "ellipseRotated");
        }

        [NUnit.Framework.Test]
        public virtual void EllipseScaledUpTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "ellipseScaledUp");
        }

        [NUnit.Framework.Test]
        public virtual void EllipseScaledDownTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "ellipseScaledDown");
        }

        [NUnit.Framework.Test]
        public virtual void EllipseScaledXYTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "ellipseScaledXY");
        }

        [NUnit.Framework.Test]
        public virtual void EllipseSkewXTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "ellipseSkewX");
        }

        [NUnit.Framework.Test]
        public virtual void EllipseSkewYTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "ellipseSkewY");
        }

        [NUnit.Framework.Test]
        public virtual void ParseParametersAndCalculateCoordinatesWithBetterPrecisionEllipseTest() {
            String filename = "calculateCoordinatesWithBetterPrecision.pdf";
            PdfDocument doc = new PdfDocument(new PdfWriter(DESTINATION_FOLDER + filename));
            doc.AddNewPage();
            EllipseSvgNodeRenderer ellipseRenderer = new EllipseSvgNodeRenderer();
            ellipseRenderer.SetAttribute(SvgConstants.Attributes.CX, "170.3");
            ellipseRenderer.SetAttribute(SvgConstants.Attributes.CY, "339.5");
            ellipseRenderer.SetAttribute(SvgConstants.Attributes.RX, "6");
            ellipseRenderer.SetAttribute(SvgConstants.Attributes.RY, "6");
            // Parse parameters with better precision (in double type) in the method CssUtils#parseAbsoluteLength
            ellipseRenderer.SetParameters();
            SvgDrawContext context = new SvgDrawContext(null, null);
            PdfCanvas cv = new PdfCanvas(doc, 1);
            context.PushCanvas(cv);
            // Calculate coordinates with better precision (in double type) in the method EllipseSvgNodeRenderer#doDraw
            ellipseRenderer.Draw(context);
            String pageContentBytes = iText.Commons.Utils.JavaUtil.GetStringForBytes(doc.GetPage(1).GetContentBytes(), 
                System.Text.Encoding.UTF8);
            doc.Close();
            String expectedResult = "132.22 254.63 m\n" + "132.22 257.11 130.21 259.13 127.72 259.13 c\n" + "125.24 259.13 123.22 257.11 123.22 254.63 c\n"
                 + "123.22 252.14 125.24 250.13 127.72 250.13 c\n" + "130.21 250.13 132.22 252.14 132.22 254.63 c";
            NUnit.Framework.Assert.IsTrue(pageContentBytes.Contains(expectedResult));
        }

        [NUnit.Framework.Test]
        public virtual void EllipseWithBigStrokeWidthTest() {
            // TODO: DEVSIX-3932 update cmp_ after fix
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "ellipseWithBigStrokeWidth");
        }
    }
}
