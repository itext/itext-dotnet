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
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Utils;
using iText.Test;

namespace iText.Kernel.Colors.Gradients {
    /// <summary>This test class focuses on different types of linear gradient coordinates vector strategies.</summary>
    /// <remarks>
    /// This test class focuses on different types of linear gradient coordinates vector strategies.
    /// Tests related to stop colors work omitted here as they would be equivalent to tests in
    /// <see cref="LinearGradientBuilderTest"/>
    /// </remarks>
    [NUnit.Framework.Category("IntegrationTest")]
    public class StrategyBasedLinearGradientBuilderTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/colors/gradients/StrategyBasedLinearGradientBuilderTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/kernel/colors/gradients/StrategyBasedLinearGradientBuilderTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.OneTimeTearDown]
        public static void AfterClass() {
            CompareTool.Cleanup(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void NoSettersTest() {
            NUnit.Framework.Assert.IsNull(new StrategyBasedLinearGradientBuilder().BuildColor(new Rectangle(50f, 450f, 
                500f, 300f), null, null));
        }

        [NUnit.Framework.Test]
        public virtual void NoRectangleTest() {
            NUnit.Framework.Assert.IsNull(new StrategyBasedLinearGradientBuilder().AddColorStop(new GradientColorStop(
                ColorConstants.RED.GetColorValue(), 0d, GradientColorStop.OffsetType.RELATIVE)).AddColorStop(new GradientColorStop
                (ColorConstants.GREEN.GetColorValue(), 0.5, GradientColorStop.OffsetType.RELATIVE)).AddColorStop(new GradientColorStop
                (ColorConstants.BLUE.GetColorValue(), 1d, GradientColorStop.OffsetType.RELATIVE)).BuildColor(null, null
                , null));
        }

        [NUnit.Framework.Test]
        public virtual void NoStrategyProvidedTest() {
            AbstractLinearGradientBuilder gradientBuilder = new StrategyBasedLinearGradientBuilder().AddColorStop(new 
                GradientColorStop(ColorConstants.RED.GetColorValue(), 0d, GradientColorStop.OffsetType.RELATIVE)).AddColorStop
                (new GradientColorStop(ColorConstants.GREEN.GetColorValue(), 0.5, GradientColorStop.OffsetType.RELATIVE
                )).AddColorStop(new GradientColorStop(ColorConstants.BLUE.GetColorValue(), 1d, GradientColorStop.OffsetType
                .RELATIVE));
            GenerateAndComparePdfs("noStrategyProvidedTest.pdf", null, gradientBuilder);
        }

        [NUnit.Framework.Test]
        public virtual void BuilderToRightTest() {
            AbstractLinearGradientBuilder gradientBuilder = new StrategyBasedLinearGradientBuilder().SetGradientDirectionAsStrategy
                (StrategyBasedLinearGradientBuilder.GradientStrategy.TO_RIGHT).AddColorStop(new GradientColorStop(ColorConstants
                .RED.GetColorValue(), 0d, GradientColorStop.OffsetType.RELATIVE)).AddColorStop(new GradientColorStop(ColorConstants
                .GREEN.GetColorValue(), 0.5, GradientColorStop.OffsetType.RELATIVE)).AddColorStop(new GradientColorStop
                (ColorConstants.BLUE.GetColorValue(), 1d, GradientColorStop.OffsetType.RELATIVE));
            GenerateAndComparePdfs("builderToRightTest.pdf", null, gradientBuilder);
        }

        [NUnit.Framework.Test]
        public virtual void BuilderToLeftTest() {
            AbstractLinearGradientBuilder gradientBuilder = new StrategyBasedLinearGradientBuilder().SetGradientDirectionAsStrategy
                (StrategyBasedLinearGradientBuilder.GradientStrategy.TO_LEFT).AddColorStop(new GradientColorStop(ColorConstants
                .RED.GetColorValue(), 0d, GradientColorStop.OffsetType.RELATIVE)).AddColorStop(new GradientColorStop(ColorConstants
                .GREEN.GetColorValue(), 0.5, GradientColorStop.OffsetType.RELATIVE)).AddColorStop(new GradientColorStop
                (ColorConstants.BLUE.GetColorValue(), 1d, GradientColorStop.OffsetType.RELATIVE));
            GenerateAndComparePdfs("builderToLeftTest.pdf", null, gradientBuilder);
        }

        [NUnit.Framework.Test]
        public virtual void BuilderToBottomTest() {
            AbstractLinearGradientBuilder gradientBuilder = new StrategyBasedLinearGradientBuilder().SetGradientDirectionAsStrategy
                (StrategyBasedLinearGradientBuilder.GradientStrategy.TO_BOTTOM).AddColorStop(new GradientColorStop(ColorConstants
                .RED.GetColorValue(), 0d, GradientColorStop.OffsetType.RELATIVE)).AddColorStop(new GradientColorStop(ColorConstants
                .GREEN.GetColorValue(), 0.5, GradientColorStop.OffsetType.RELATIVE)).AddColorStop(new GradientColorStop
                (ColorConstants.BLUE.GetColorValue(), 1d, GradientColorStop.OffsetType.RELATIVE));
            GenerateAndComparePdfs("builderToBottomTest.pdf", null, gradientBuilder);
        }

        [NUnit.Framework.Test]
        public virtual void BuilderToBottomRightTest() {
            AbstractLinearGradientBuilder gradientBuilder = new StrategyBasedLinearGradientBuilder().SetGradientDirectionAsStrategy
                (StrategyBasedLinearGradientBuilder.GradientStrategy.TO_BOTTOM_RIGHT).AddColorStop(new GradientColorStop
                (ColorConstants.RED.GetColorValue(), 0d, GradientColorStop.OffsetType.RELATIVE)).AddColorStop(new GradientColorStop
                (ColorConstants.GREEN.GetColorValue(), 0.5, GradientColorStop.OffsetType.RELATIVE)).AddColorStop(new GradientColorStop
                (ColorConstants.BLUE.GetColorValue(), 1d, GradientColorStop.OffsetType.RELATIVE));
            GenerateAndComparePdfs("builderToBottomRightTest.pdf", null, gradientBuilder);
        }

        [NUnit.Framework.Test]
        public virtual void BuilderToBottomLeftTest() {
            AbstractLinearGradientBuilder gradientBuilder = new StrategyBasedLinearGradientBuilder().SetGradientDirectionAsStrategy
                (StrategyBasedLinearGradientBuilder.GradientStrategy.TO_BOTTOM_LEFT).AddColorStop(new GradientColorStop
                (ColorConstants.RED.GetColorValue(), 0d, GradientColorStop.OffsetType.RELATIVE)).AddColorStop(new GradientColorStop
                (ColorConstants.GREEN.GetColorValue(), 0.5, GradientColorStop.OffsetType.RELATIVE)).AddColorStop(new GradientColorStop
                (ColorConstants.BLUE.GetColorValue(), 1d, GradientColorStop.OffsetType.RELATIVE));
            GenerateAndComparePdfs("builderToBottomLeftTest.pdf", null, gradientBuilder);
        }

        [NUnit.Framework.Test]
        public virtual void BuilderToTopTest() {
            AbstractLinearGradientBuilder gradientBuilder = new StrategyBasedLinearGradientBuilder().SetGradientDirectionAsStrategy
                (StrategyBasedLinearGradientBuilder.GradientStrategy.TO_TOP).AddColorStop(new GradientColorStop(ColorConstants
                .RED.GetColorValue(), 0d, GradientColorStop.OffsetType.RELATIVE)).AddColorStop(new GradientColorStop(ColorConstants
                .GREEN.GetColorValue(), 0.5, GradientColorStop.OffsetType.RELATIVE)).AddColorStop(new GradientColorStop
                (ColorConstants.BLUE.GetColorValue(), 1d, GradientColorStop.OffsetType.RELATIVE));
            GenerateAndComparePdfs("builderToTopTest.pdf", null, gradientBuilder);
        }

        [NUnit.Framework.Test]
        public virtual void BuilderToTopRightTest() {
            AbstractLinearGradientBuilder gradientBuilder = new StrategyBasedLinearGradientBuilder().SetGradientDirectionAsStrategy
                (StrategyBasedLinearGradientBuilder.GradientStrategy.TO_TOP_RIGHT).AddColorStop(new GradientColorStop(
                ColorConstants.RED.GetColorValue(), 0d, GradientColorStop.OffsetType.RELATIVE)).AddColorStop(new GradientColorStop
                (ColorConstants.GREEN.GetColorValue(), 0.5, GradientColorStop.OffsetType.RELATIVE)).AddColorStop(new GradientColorStop
                (ColorConstants.BLUE.GetColorValue(), 1d, GradientColorStop.OffsetType.RELATIVE));
            GenerateAndComparePdfs("builderToTopRightTest.pdf", null, gradientBuilder);
        }

        [NUnit.Framework.Test]
        public virtual void BuilderToTopLeftTest() {
            AbstractLinearGradientBuilder gradientBuilder = new StrategyBasedLinearGradientBuilder().SetGradientDirectionAsStrategy
                (StrategyBasedLinearGradientBuilder.GradientStrategy.TO_TOP_LEFT).AddColorStop(new GradientColorStop(ColorConstants
                .RED.GetColorValue(), 0d, GradientColorStop.OffsetType.RELATIVE)).AddColorStop(new GradientColorStop(ColorConstants
                .GREEN.GetColorValue(), 0.5, GradientColorStop.OffsetType.RELATIVE)).AddColorStop(new GradientColorStop
                (ColorConstants.BLUE.GetColorValue(), 1d, GradientColorStop.OffsetType.RELATIVE));
            GenerateAndComparePdfs("builderToTopLeftTest.pdf", null, gradientBuilder);
        }

        [NUnit.Framework.Test]
        public virtual void BuilderZeroAngleTest() {
            AbstractLinearGradientBuilder gradientBuilder = new StrategyBasedLinearGradientBuilder().SetGradientDirectionAsCentralRotationAngle
                (0d).AddColorStop(new GradientColorStop(ColorConstants.RED.GetColorValue(), 0d, GradientColorStop.OffsetType
                .RELATIVE)).AddColorStop(new GradientColorStop(ColorConstants.GREEN.GetColorValue(), 0.5, GradientColorStop.OffsetType
                .RELATIVE)).AddColorStop(new GradientColorStop(ColorConstants.BLUE.GetColorValue(), 1d, GradientColorStop.OffsetType
                .RELATIVE));
            GenerateAndComparePdfs("builderZeroAngleTest.pdf", null, gradientBuilder);
        }

        [NUnit.Framework.Test]
        public virtual void BuilderPositiveAngleTest() {
            AbstractLinearGradientBuilder gradientBuilder = new StrategyBasedLinearGradientBuilder().SetGradientDirectionAsCentralRotationAngle
                (Math.PI / 3).AddColorStop(new GradientColorStop(ColorConstants.RED.GetColorValue(), 0d, GradientColorStop.OffsetType
                .RELATIVE)).AddColorStop(new GradientColorStop(ColorConstants.GREEN.GetColorValue(), 0.5, GradientColorStop.OffsetType
                .RELATIVE)).AddColorStop(new GradientColorStop(ColorConstants.BLUE.GetColorValue(), 1d, GradientColorStop.OffsetType
                .RELATIVE));
            GenerateAndComparePdfs("builderPositiveAngleTest.pdf", null, gradientBuilder);
        }

        [NUnit.Framework.Test]
        public virtual void BuilderNegativeAngleTest() {
            AbstractLinearGradientBuilder gradientBuilder = new StrategyBasedLinearGradientBuilder().SetGradientDirectionAsCentralRotationAngle
                (-Math.PI / 3).AddColorStop(new GradientColorStop(ColorConstants.RED.GetColorValue(), 0d, GradientColorStop.OffsetType
                .RELATIVE)).AddColorStop(new GradientColorStop(ColorConstants.GREEN.GetColorValue(), 0.5, GradientColorStop.OffsetType
                .RELATIVE)).AddColorStop(new GradientColorStop(ColorConstants.BLUE.GetColorValue(), 1d, GradientColorStop.OffsetType
                .RELATIVE));
            GenerateAndComparePdfs("builderNegativeAngleTest.pdf", null, gradientBuilder);
        }

        [NUnit.Framework.Test]
        public virtual void BuilderWithNoneSpreadingAndCanvasTransformTest() {
            AbstractLinearGradientBuilder gradientBuilder = new StrategyBasedLinearGradientBuilder().SetGradientDirectionAsStrategy
                (StrategyBasedLinearGradientBuilder.GradientStrategy.TO_RIGHT).SetSpreadMethod(GradientSpreadMethod.NONE
                ).AddColorStop(new GradientColorStop(ColorConstants.RED.GetColorValue(), 0d, GradientColorStop.OffsetType
                .RELATIVE)).AddColorStop(new GradientColorStop(ColorConstants.GREEN.GetColorValue(), 0.5, GradientColorStop.OffsetType
                .RELATIVE)).AddColorStop(new GradientColorStop(ColorConstants.BLUE.GetColorValue(), 1d, GradientColorStop.OffsetType
                .RELATIVE));
            AffineTransform canvasTransform = AffineTransform.GetTranslateInstance(50, -50);
            canvasTransform.Scale(0.8, 1.1);
            canvasTransform.Rotate(Math.PI / 3, 400f, 550f);
            GenerateAndComparePdfs("builderWithNoneSpreadingAndCanvasTransformTest.pdf", canvasTransform, gradientBuilder
                );
        }

        [NUnit.Framework.Test]
        public virtual void BuilderWithToCornerAndInnerStopsAndNoneSpreadingTest() {
            AbstractLinearGradientBuilder gradientBuilder = new StrategyBasedLinearGradientBuilder().SetGradientDirectionAsStrategy
                (StrategyBasedLinearGradientBuilder.GradientStrategy.TO_TOP_RIGHT).SetSpreadMethod(GradientSpreadMethod
                .NONE).AddColorStop(new GradientColorStop(ColorConstants.RED.GetColorValue(), 0.3d, GradientColorStop.OffsetType
                .RELATIVE)).AddColorStop(new GradientColorStop(ColorConstants.GREEN.GetColorValue(), 0.4, GradientColorStop.OffsetType
                .RELATIVE)).AddColorStop(new GradientColorStop(ColorConstants.BLUE.GetColorValue(), 0.5d, GradientColorStop.OffsetType
                .RELATIVE));
            GenerateAndComparePdfs("builderWithToCornerAndInnerStopsAndNoneSpreadingTest.pdf", null, gradientBuilder);
        }

        [NUnit.Framework.Test]
        public virtual void BuilderWithToCornerAndInnerStopsAndPadSpreadingTest() {
            AbstractLinearGradientBuilder gradientBuilder = new StrategyBasedLinearGradientBuilder().SetGradientDirectionAsStrategy
                (StrategyBasedLinearGradientBuilder.GradientStrategy.TO_TOP_RIGHT).SetSpreadMethod(GradientSpreadMethod
                .PAD).AddColorStop(new GradientColorStop(ColorConstants.RED.GetColorValue(), 0.3d, GradientColorStop.OffsetType
                .RELATIVE)).AddColorStop(new GradientColorStop(ColorConstants.GREEN.GetColorValue(), 0.4, GradientColorStop.OffsetType
                .RELATIVE)).AddColorStop(new GradientColorStop(ColorConstants.BLUE.GetColorValue(), 0.5d, GradientColorStop.OffsetType
                .RELATIVE));
            GenerateAndComparePdfs("builderWithToCornerAndInnerStopsAndPadSpreadingTest.pdf", null, gradientBuilder);
        }

        [NUnit.Framework.Test]
        public virtual void BuilderWithToCornerAndInnerStopsAndReflectSpreadingTest() {
            AbstractLinearGradientBuilder gradientBuilder = new StrategyBasedLinearGradientBuilder().SetGradientDirectionAsStrategy
                (StrategyBasedLinearGradientBuilder.GradientStrategy.TO_TOP_RIGHT).SetSpreadMethod(GradientSpreadMethod
                .REFLECT).AddColorStop(new GradientColorStop(ColorConstants.RED.GetColorValue(), 0.3d, GradientColorStop.OffsetType
                .RELATIVE)).AddColorStop(new GradientColorStop(ColorConstants.GREEN.GetColorValue(), 0.4, GradientColorStop.OffsetType
                .RELATIVE)).AddColorStop(new GradientColorStop(ColorConstants.BLUE.GetColorValue(), 0.5d, GradientColorStop.OffsetType
                .RELATIVE));
            GenerateAndComparePdfs("builderWithToCornerAndInnerStopsAndReflectSpreadingTest.pdf", null, gradientBuilder
                );
        }

        [NUnit.Framework.Test]
        public virtual void BuilderWithToCornerAndInnerStopsAndRepeatSpreadingTest() {
            AbstractLinearGradientBuilder gradientBuilder = new StrategyBasedLinearGradientBuilder().SetGradientDirectionAsStrategy
                (StrategyBasedLinearGradientBuilder.GradientStrategy.TO_TOP_RIGHT).SetSpreadMethod(GradientSpreadMethod
                .REPEAT).AddColorStop(new GradientColorStop(ColorConstants.RED.GetColorValue(), 0.3d, GradientColorStop.OffsetType
                .RELATIVE)).AddColorStop(new GradientColorStop(ColorConstants.GREEN.GetColorValue(), 0.4, GradientColorStop.OffsetType
                .RELATIVE)).AddColorStop(new GradientColorStop(ColorConstants.BLUE.GetColorValue(), 0.5d, GradientColorStop.OffsetType
                .RELATIVE));
            GenerateAndComparePdfs("builderWithToCornerAndInnerStopsAndRepeatSpreadingTest.pdf", null, gradientBuilder
                );
        }

        private void GenerateAndComparePdfs(String fileName, AffineTransform transform, AbstractLinearGradientBuilder
             gradientBuilder) {
            String outPdfPath = destinationFolder + fileName;
            using (PdfDocument pdfDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(outPdfPath))) {
                PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage());
                if (transform != null) {
                    canvas.ConcatMatrix(transform);
                }
                Rectangle toDraw = new Rectangle(50f, 450f, 500f, 300f);
                canvas.SetFillColor(gradientBuilder.BuildColor(toDraw, transform, pdfDoc)).SetStrokeColor(ColorConstants.BLACK
                    ).Rectangle(toDraw).FillStroke();
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdfPath, sourceFolder + "cmp_" + fileName
                , destinationFolder, "diff"));
        }
    }
}
