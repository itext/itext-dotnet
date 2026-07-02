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
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Utils;
using iText.Test;

namespace iText.Kernel.Colors.Gradients {
    /// <summary>This test class focuses on different types of radial gradient strategies.</summary>
    /// <remarks>
    /// This test class focuses on different types of radial gradient strategies.
    /// Tests related to stop colors work omitted here as they would be equivalent to tests in
    /// <see cref="RadialGradientBuilderTest"/>.
    /// </remarks>
    [NUnit.Framework.Category("IntegrationTest")]
    public class StrategyBasedRadialGradientBuilderTest : ExtendedITextTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/colors/gradients/StrategyBasedRadialGradientBuilderTest/";

        private static readonly String DESTINATION_FOLDER = TestUtil.GetOutputPath() + "/kernel/colors/gradients/StrategyBasedRadialGradientBuilderTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.OneTimeTearDown]
        public static void AfterClass() {
            CompareTool.Cleanup(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void NoSettersTest() {
            NUnit.Framework.Assert.IsNull(new StrategyBasedRadialGradientBuilder().BuildColor(new Rectangle(50f, 450f, 
                500f, 300f), null, null));
        }

        [NUnit.Framework.Test]
        public virtual void NoRectangleTest() {
            NUnit.Framework.Assert.IsNull(new StrategyBasedRadialGradientBuilder().AddStopColor(new GradientColorStop(
                ColorConstants.RED.GetColorValue(), 0d, GradientColorStop.OffsetType.RELATIVE)).AddStopColor(new GradientColorStop
                (ColorConstants.GREEN.GetColorValue(), 0.5, GradientColorStop.OffsetType.RELATIVE)).AddStopColor(new GradientColorStop
                (ColorConstants.BLUE.GetColorValue(), 1d, GradientColorStop.OffsetType.RELATIVE)).BuildColor(null, null
                , null));
        }

        [NUnit.Framework.Test]
        public virtual void NoStrategyProvidedTest() {
            AbstractGradientBuilder<RadialGradientPoint> gradientBuilder = new StrategyBasedRadialGradientBuilder().AddStopColor
                (new GradientColorStop(ColorConstants.RED.GetColorValue(), 0d, GradientColorStop.OffsetType.RELATIVE))
                .AddStopColor(new GradientColorStop(ColorConstants.GREEN.GetColorValue(), 0.5, GradientColorStop.OffsetType
                .RELATIVE)).AddStopColor(new GradientColorStop(ColorConstants.BLUE.GetColorValue(), 1d, GradientColorStop.OffsetType
                .RELATIVE));
            GenerateAndComparePdfs("noStrategyProvided.pdf", null, gradientBuilder);
        }

        [NUnit.Framework.Test]
        public virtual void AbsoluteCenterFromLeftBottomTest() {
            AbstractGradientBuilder<RadialGradientPoint> gradientBuilder = new StrategyBasedRadialGradientBuilder().SetCenterStrategy
                (true, 120, false, true, 50, false).SetRadiusRelativeToBoundingBoxSize(100, false, 100, false).AddStopColor
                (new GradientColorStop(ColorConstants.RED.GetColorValue(), 0d, GradientColorStop.OffsetType.RELATIVE))
                .AddStopColor(new GradientColorStop(ColorConstants.GREEN.GetColorValue(), 0.5, GradientColorStop.OffsetType
                .RELATIVE)).AddStopColor(new GradientColorStop(ColorConstants.BLUE.GetColorValue(), 1d, GradientColorStop.OffsetType
                .RELATIVE));
            GenerateAndComparePdfs("absoluteCenterFromLeftBottom.pdf", null, gradientBuilder);
        }

        [NUnit.Framework.Test]
        public virtual void AbsoluteCenterFromRightTopTest() {
            AbstractGradientBuilder<RadialGradientPoint> gradientBuilder = new StrategyBasedRadialGradientBuilder().SetCenterStrategy
                (false, 120, false, false, 50, false).SetRadiusRelativeToBoundingBoxSize(100, false, 100, false).AddStopColor
                (new GradientColorStop(ColorConstants.RED.GetColorValue(), 0d, GradientColorStop.OffsetType.RELATIVE))
                .AddStopColor(new GradientColorStop(ColorConstants.GREEN.GetColorValue(), 0.5, GradientColorStop.OffsetType
                .RELATIVE)).AddStopColor(new GradientColorStop(ColorConstants.BLUE.GetColorValue(), 1d, GradientColorStop.OffsetType
                .RELATIVE));
            GenerateAndComparePdfs("absoluteCenterFromRightTop.pdf", null, gradientBuilder);
        }

        [NUnit.Framework.Test]
        public virtual void AbsoluteNegativeCenterFromLeftBottomTest() {
            AbstractGradientBuilder<RadialGradientPoint> gradientBuilder = new StrategyBasedRadialGradientBuilder().SetCenterStrategy
                (true, -120, false, true, -50, false).SetRadiusRelativeToBoundingBoxSize(200, false, 200, false).AddStopColor
                (new GradientColorStop(ColorConstants.RED.GetColorValue(), 0d, GradientColorStop.OffsetType.RELATIVE))
                .AddStopColor(new GradientColorStop(ColorConstants.GREEN.GetColorValue(), 0.5, GradientColorStop.OffsetType
                .RELATIVE)).AddStopColor(new GradientColorStop(ColorConstants.BLUE.GetColorValue(), 1d, GradientColorStop.OffsetType
                .RELATIVE));
            GenerateAndComparePdfs("absoluteNegativeCenterFromLeftBottom.pdf", null, gradientBuilder);
        }

        [NUnit.Framework.Test]
        public virtual void AbsoluteNegativeCenterFromRightTopTest() {
            AbstractGradientBuilder<RadialGradientPoint> gradientBuilder = new StrategyBasedRadialGradientBuilder().SetCenterStrategy
                (false, -120, false, false, -50, false).SetRadiusRelativeToBoundingBoxSize(200, false, 200, false).AddStopColor
                (new GradientColorStop(ColorConstants.RED.GetColorValue(), 0d, GradientColorStop.OffsetType.RELATIVE))
                .AddStopColor(new GradientColorStop(ColorConstants.GREEN.GetColorValue(), 0.5, GradientColorStop.OffsetType
                .RELATIVE)).AddStopColor(new GradientColorStop(ColorConstants.BLUE.GetColorValue(), 1d, GradientColorStop.OffsetType
                .RELATIVE));
            GenerateAndComparePdfs("absoluteNegativeCenterFromRightTop.pdf", null, gradientBuilder);
        }

        [NUnit.Framework.Test]
        public virtual void RelativeCenterFromLeftBottomTest() {
            AbstractGradientBuilder<RadialGradientPoint> gradientBuilder = new StrategyBasedRadialGradientBuilder().SetCenterStrategy
                (true, 0.1d, true, true, 0.1d, true).SetRadiusRelativeToBoundingBoxSize(100, false, 100, false).AddStopColor
                (new GradientColorStop(ColorConstants.RED.GetColorValue(), 0d, GradientColorStop.OffsetType.RELATIVE))
                .AddStopColor(new GradientColorStop(ColorConstants.GREEN.GetColorValue(), 0.5, GradientColorStop.OffsetType
                .RELATIVE)).AddStopColor(new GradientColorStop(ColorConstants.BLUE.GetColorValue(), 1d, GradientColorStop.OffsetType
                .RELATIVE));
            GenerateAndComparePdfs("relativeCenterFromLeftBottom.pdf", null, gradientBuilder);
        }

        [NUnit.Framework.Test]
        public virtual void RelativeCenterFromRightTopTest() {
            AbstractGradientBuilder<RadialGradientPoint> gradientBuilder = new StrategyBasedRadialGradientBuilder().SetCenterStrategy
                (false, 0.1d, true, false, 0.1d, true).SetRadiusRelativeToBoundingBoxSize(100, false, 100, false).AddStopColor
                (new GradientColorStop(ColorConstants.RED.GetColorValue(), 0d, GradientColorStop.OffsetType.RELATIVE))
                .AddStopColor(new GradientColorStop(ColorConstants.GREEN.GetColorValue(), 0.5, GradientColorStop.OffsetType
                .RELATIVE)).AddStopColor(new GradientColorStop(ColorConstants.BLUE.GetColorValue(), 1d, GradientColorStop.OffsetType
                .RELATIVE));
            GenerateAndComparePdfs("relativeCenterFromRightTop.pdf", null, gradientBuilder);
        }

        [NUnit.Framework.Test]
        public virtual void BuilderClosestSideEllipseTest() {
            AbstractGradientBuilder<RadialGradientPoint> gradientBuilder = new StrategyBasedRadialGradientBuilder().SetCenterStrategy
                (true, 0.3, true, true, 0.2, true).SetRadiusFromCenterStrategy(false, StrategyBasedRadialGradientBuilder.GradientStrategy
                .CLOSEST_SIDE).AddStopColor(new GradientColorStop(ColorConstants.RED.GetColorValue(), 0d, GradientColorStop.OffsetType
                .RELATIVE)).AddStopColor(new GradientColorStop(ColorConstants.GREEN.GetColorValue(), 0.5, GradientColorStop.OffsetType
                .RELATIVE)).AddStopColor(new GradientColorStop(ColorConstants.BLUE.GetColorValue(), 1d, GradientColorStop.OffsetType
                .RELATIVE));
            GenerateAndComparePdfs("builderClosestSideEllipse.pdf", null, gradientBuilder);
        }

        [NUnit.Framework.Test]
        public virtual void BuilderClosestCornerEllipseTest() {
            AbstractGradientBuilder<RadialGradientPoint> gradientBuilder = new StrategyBasedRadialGradientBuilder().SetCenterStrategy
                (true, 0.3, true, true, 0.2, true).SetRadiusFromCenterStrategy(false, StrategyBasedRadialGradientBuilder.GradientStrategy
                .CLOSEST_CORNER).AddStopColor(new GradientColorStop(ColorConstants.RED.GetColorValue(), 0d, GradientColorStop.OffsetType
                .RELATIVE)).AddStopColor(new GradientColorStop(ColorConstants.GREEN.GetColorValue(), 0.5, GradientColorStop.OffsetType
                .RELATIVE)).AddStopColor(new GradientColorStop(ColorConstants.BLUE.GetColorValue(), 1d, GradientColorStop.OffsetType
                .RELATIVE));
            GenerateAndComparePdfs("builderClosestCornerEllipse.pdf", null, gradientBuilder);
        }

        [NUnit.Framework.Test]
        public virtual void BuilderFarthestSideEllipseTest() {
            AbstractGradientBuilder<RadialGradientPoint> gradientBuilder = new StrategyBasedRadialGradientBuilder().SetCenterStrategy
                (true, 0.3, true, true, 0.2, true).SetRadiusFromCenterStrategy(false, StrategyBasedRadialGradientBuilder.GradientStrategy
                .FARTHEST_SIDE).AddStopColor(new GradientColorStop(ColorConstants.RED.GetColorValue(), 0d, GradientColorStop.OffsetType
                .RELATIVE)).AddStopColor(new GradientColorStop(ColorConstants.GREEN.GetColorValue(), 0.5, GradientColorStop.OffsetType
                .RELATIVE)).AddStopColor(new GradientColorStop(ColorConstants.BLUE.GetColorValue(), 1d, GradientColorStop.OffsetType
                .RELATIVE));
            GenerateAndComparePdfs("builderFarthestSideEllipse.pdf", null, gradientBuilder);
        }

        [NUnit.Framework.Test]
        public virtual void BuilderFarthestCornerEllipseTest() {
            AbstractGradientBuilder<RadialGradientPoint> gradientBuilder = new StrategyBasedRadialGradientBuilder().SetCenterStrategy
                (true, 0.3, true, true, 0.2, true).SetRadiusFromCenterStrategy(false, StrategyBasedRadialGradientBuilder.GradientStrategy
                .FARTHEST_CORNER).AddStopColor(new GradientColorStop(ColorConstants.RED.GetColorValue(), 0d, GradientColorStop.OffsetType
                .RELATIVE)).AddStopColor(new GradientColorStop(ColorConstants.GREEN.GetColorValue(), 0.5, GradientColorStop.OffsetType
                .RELATIVE)).AddStopColor(new GradientColorStop(ColorConstants.BLUE.GetColorValue(), 1d, GradientColorStop.OffsetType
                .RELATIVE));
            GenerateAndComparePdfs("builderFarthestCornerEllipse.pdf", null, gradientBuilder);
        }

        [NUnit.Framework.Test]
        public virtual void BuilderClosestSideCircleTest() {
            AbstractGradientBuilder<RadialGradientPoint> gradientBuilder = new StrategyBasedRadialGradientBuilder().SetCenterStrategy
                (true, 0.3, true, true, 0.2, true).SetRadiusFromCenterStrategy(true, StrategyBasedRadialGradientBuilder.GradientStrategy
                .CLOSEST_SIDE).AddStopColor(new GradientColorStop(ColorConstants.RED.GetColorValue(), 0d, GradientColorStop.OffsetType
                .RELATIVE)).AddStopColor(new GradientColorStop(ColorConstants.GREEN.GetColorValue(), 0.5, GradientColorStop.OffsetType
                .RELATIVE)).AddStopColor(new GradientColorStop(ColorConstants.BLUE.GetColorValue(), 1d, GradientColorStop.OffsetType
                .RELATIVE));
            GenerateAndComparePdfs("builderClosestSideCircle.pdf", null, gradientBuilder);
        }

        [NUnit.Framework.Test]
        public virtual void BuilderClosestCornerCircleTest() {
            AbstractGradientBuilder<RadialGradientPoint> gradientBuilder = new StrategyBasedRadialGradientBuilder().SetCenterStrategy
                (true, 0.3, true, true, 0.2, true).SetRadiusFromCenterStrategy(true, StrategyBasedRadialGradientBuilder.GradientStrategy
                .CLOSEST_CORNER).AddStopColor(new GradientColorStop(ColorConstants.RED.GetColorValue(), 0d, GradientColorStop.OffsetType
                .RELATIVE)).AddStopColor(new GradientColorStop(ColorConstants.GREEN.GetColorValue(), 0.5, GradientColorStop.OffsetType
                .RELATIVE)).AddStopColor(new GradientColorStop(ColorConstants.BLUE.GetColorValue(), 1d, GradientColorStop.OffsetType
                .RELATIVE));
            GenerateAndComparePdfs("builderClosestCornerCircle.pdf", null, gradientBuilder);
        }

        [NUnit.Framework.Test]
        public virtual void BuilderFarthestSideCircleTest() {
            AbstractGradientBuilder<RadialGradientPoint> gradientBuilder = new StrategyBasedRadialGradientBuilder().SetCenterStrategy
                (true, 0.3, true, true, 0.2, true).SetRadiusFromCenterStrategy(true, StrategyBasedRadialGradientBuilder.GradientStrategy
                .FARTHEST_SIDE).AddStopColor(new GradientColorStop(ColorConstants.RED.GetColorValue(), 0d, GradientColorStop.OffsetType
                .RELATIVE)).AddStopColor(new GradientColorStop(ColorConstants.GREEN.GetColorValue(), 0.5, GradientColorStop.OffsetType
                .RELATIVE)).AddStopColor(new GradientColorStop(ColorConstants.BLUE.GetColorValue(), 1d, GradientColorStop.OffsetType
                .RELATIVE));
            GenerateAndComparePdfs("builderFarthestSideCircle.pdf", null, gradientBuilder);
        }

        [NUnit.Framework.Test]
        public virtual void BuilderFarthestCornerCircleTest() {
            AbstractGradientBuilder<RadialGradientPoint> gradientBuilder = new StrategyBasedRadialGradientBuilder().SetCenterStrategy
                (true, 0.3, true, true, 0.2, true).SetRadiusFromCenterStrategy(true, StrategyBasedRadialGradientBuilder.GradientStrategy
                .FARTHEST_CORNER).AddStopColor(new GradientColorStop(ColorConstants.RED.GetColorValue(), 0d, GradientColorStop.OffsetType
                .RELATIVE)).AddStopColor(new GradientColorStop(ColorConstants.GREEN.GetColorValue(), 0.5, GradientColorStop.OffsetType
                .RELATIVE)).AddStopColor(new GradientColorStop(ColorConstants.BLUE.GetColorValue(), 1d, GradientColorStop.OffsetType
                .RELATIVE));
            GenerateAndComparePdfs("builderFarthestCornerCircle.pdf", null, gradientBuilder);
        }

        [NUnit.Framework.Test]
        public virtual void BuilderZeroManualRadiusWithPadTest() {
            AbstractGradientBuilder<RadialGradientPoint> gradientBuilder = new StrategyBasedRadialGradientBuilder().SetRadiusRelativeToBoundingBoxSize
                (0, false, 0, false).AddStopColor(new GradientColorStop(ColorConstants.RED.GetColorValue(), 0d, GradientColorStop.OffsetType
                .RELATIVE)).AddStopColor(new GradientColorStop(ColorConstants.GREEN.GetColorValue(), 0.5, GradientColorStop.OffsetType
                .RELATIVE)).AddStopColor(new GradientColorStop(ColorConstants.BLUE.GetColorValue(), 1d, GradientColorStop.OffsetType
                .RELATIVE)).SetSpread(GradientSpreadMethod.PAD);
            GenerateAndComparePdfs("builderZeroManualRadiusWithPad.pdf", null, gradientBuilder);
        }

        [NUnit.Framework.Test]
        public virtual void BuilderManualRadiusTest() {
            AbstractGradientBuilder<RadialGradientPoint> gradientBuilder = new StrategyBasedRadialGradientBuilder().SetRadiusRelativeToBoundingBoxSize
                (0.3d, true, 20, false).AddStopColor(new GradientColorStop(ColorConstants.RED.GetColorValue(), 0d, GradientColorStop.OffsetType
                .RELATIVE)).AddStopColor(new GradientColorStop(ColorConstants.GREEN.GetColorValue(), 0.5, GradientColorStop.OffsetType
                .RELATIVE)).AddStopColor(new GradientColorStop(ColorConstants.BLUE.GetColorValue(), 1d, GradientColorStop.OffsetType
                .RELATIVE));
            GenerateAndComparePdfs("builderManualRadius.pdf", null, gradientBuilder);
        }

        [NUnit.Framework.Test]
        public virtual void BuilderNegativeRadiusTest() {
            AbstractGradientBuilder<RadialGradientPoint> gradientBuilder = new StrategyBasedRadialGradientBuilder().SetRadiusRelativeToBoundingBoxSize
                (-0.3d, true, -20, false).AddStopColor(new GradientColorStop(ColorConstants.RED.GetColorValue(), 0d, GradientColorStop.OffsetType
                .RELATIVE)).AddStopColor(new GradientColorStop(ColorConstants.GREEN.GetColorValue(), 0.5, GradientColorStop.OffsetType
                .RELATIVE)).AddStopColor(new GradientColorStop(ColorConstants.BLUE.GetColorValue(), 1d, GradientColorStop.OffsetType
                .RELATIVE));
            NUnit.Framework.Assert.IsNull(gradientBuilder.BuildColor(new Rectangle(50f, 450f, 500f, 300f), null, null)
                );
        }

        [NUnit.Framework.Test]
        public virtual void BuilderWithNoneSpreadingAndCanvasTransformTest() {
            AbstractGradientBuilder<RadialGradientPoint> gradientBuilder = new StrategyBasedRadialGradientBuilder().AddStopColor
                (new GradientColorStop(ColorConstants.RED.GetColorValue(), 0d, GradientColorStop.OffsetType.RELATIVE))
                .AddStopColor(new GradientColorStop(ColorConstants.GREEN.GetColorValue(), 0.5, GradientColorStop.OffsetType
                .RELATIVE)).AddStopColor(new GradientColorStop(ColorConstants.BLUE.GetColorValue(), 1d, GradientColorStop.OffsetType
                .RELATIVE)).SetSpread(GradientSpreadMethod.NONE);
            AffineTransform canvasTransform = AffineTransform.GetTranslateInstance(50, -50);
            canvasTransform.Scale(0.8, 1.1);
            canvasTransform.Rotate(Math.PI / 3, 400f, 550f);
            GenerateAndComparePdfs("noneSpreadingCanvasTransform.pdf", canvasTransform, gradientBuilder);
        }

        private void GenerateAndComparePdfs(String fileName, AffineTransform transform, AbstractGradientBuilder<RadialGradientPoint
            > gradientBuilder) {
            String outPdfPath = DESTINATION_FOLDER + fileName;
            using (PdfDocument pdfDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(outPdfPath))) {
                PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage());
                if (transform != null) {
                    canvas.ConcatMatrix(transform);
                }
                Rectangle toDraw = new Rectangle(50f, 450f, 500f, 300f);
                canvas.SetFillColor(gradientBuilder.BuildColor(toDraw, transform, pdfDoc)).SetStrokeColor(ColorConstants.BLACK
                    ).Rectangle(toDraw).FillStroke();
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdfPath, SOURCE_FOLDER + "cmp_" + fileName
                , DESTINATION_FOLDER, "diff"));
        }
    }
}
