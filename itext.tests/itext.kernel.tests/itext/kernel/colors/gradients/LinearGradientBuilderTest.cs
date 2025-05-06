/*
This file is part of the iText (R) project.
Copyright (c) 1998-2025 Apryse Group NV
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
    [NUnit.Framework.Category("IntegrationTest")]
    public class LinearGradientBuilderTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/colors/gradients/LinearGradientBuilderTest/";

        public static readonly String destinationFolder = TestUtil.GetOutputPath() + "/kernel/colors/gradients/LinearGradientBuilderTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.OneTimeTearDown]
        public static void AfterClass() {
            CompareTool.Cleanup(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void BuildWithNullArgumentsAndWithoutSettersTest() {
            Rectangle targetBoundingBox = new Rectangle(50f, 450f, 300f, 300f);
            AbstractLinearGradientBuilder gradientBuilder = new LinearGradientBuilder();
            NUnit.Framework.Assert.IsNull(gradientBuilder.BuildColor(targetBoundingBox, null, null));
        }

        [NUnit.Framework.Test]
        public virtual void BuildWithOneStopTest() {
            Rectangle targetBoundingBox = new Rectangle(50f, 450f, 300f, 300f);
            AbstractLinearGradientBuilder gradientBuilder = new LinearGradientBuilder().SetGradientVector(targetBoundingBox
                .GetLeft() + 100f, targetBoundingBox.GetBottom() + 100f, targetBoundingBox.GetRight() - 100f, targetBoundingBox
                .GetTop() - 100f).SetSpreadMethod(GradientSpreadMethod.PAD).AddColorStop(new GradientColorStop(ColorConstants
                .BLUE.GetColorValue(), 1d, GradientColorStop.OffsetType.RELATIVE));
            GenerateAndComparePdfs("oneStop.pdf", targetBoundingBox, null, gradientBuilder);
        }

        [NUnit.Framework.Test]
        public virtual void BuildWithTwoStopsTest() {
            Rectangle targetBoundingBox = new Rectangle(50f, 450f, 300f, 300f);
            AbstractLinearGradientBuilder gradientBuilder = new LinearGradientBuilder().SetGradientVector(targetBoundingBox
                .GetLeft() + 100f, targetBoundingBox.GetBottom() + 100f, targetBoundingBox.GetRight() - 100f, targetBoundingBox
                .GetTop() - 100f).SetSpreadMethod(GradientSpreadMethod.PAD).AddColorStop(new GradientColorStop(ColorConstants
                .RED.GetColorValue(), 0d, GradientColorStop.OffsetType.RELATIVE)).AddColorStop(new GradientColorStop(ColorConstants
                .BLUE.GetColorValue(), 1d, GradientColorStop.OffsetType.RELATIVE));
            GenerateAndComparePdfs("twoStops.pdf", targetBoundingBox, null, gradientBuilder);
        }

        [NUnit.Framework.Test]
        public virtual void BuildWithTwoStopsAtTheBeginningTest() {
            Rectangle targetBoundingBox = new Rectangle(50f, 450f, 300f, 300f);
            AbstractLinearGradientBuilder gradientBuilder = new LinearGradientBuilder().SetGradientVector(targetBoundingBox
                .GetLeft() + 100f, targetBoundingBox.GetBottom() + 100f, targetBoundingBox.GetRight() - 100f, targetBoundingBox
                .GetTop() - 100f).SetSpreadMethod(GradientSpreadMethod.PAD).AddColorStop(new GradientColorStop(ColorConstants
                .RED.GetColorValue(), 0d, GradientColorStop.OffsetType.RELATIVE)).AddColorStop(new GradientColorStop(ColorConstants
                .BLUE.GetColorValue(), 0d, GradientColorStop.OffsetType.RELATIVE));
            GenerateAndComparePdfs("twoStopsAtTheBeginning.pdf", targetBoundingBox, null, gradientBuilder);
        }

        [NUnit.Framework.Test]
        public virtual void BuildWithTwoStopsAtTheEndTest() {
            Rectangle targetBoundingBox = new Rectangle(50f, 450f, 300f, 300f);
            AbstractLinearGradientBuilder gradientBuilder = new LinearGradientBuilder().SetGradientVector(targetBoundingBox
                .GetLeft() + 100f, targetBoundingBox.GetBottom() + 100f, targetBoundingBox.GetRight() - 100f, targetBoundingBox
                .GetTop() - 100f).SetSpreadMethod(GradientSpreadMethod.PAD).AddColorStop(new GradientColorStop(ColorConstants
                .RED.GetColorValue(), 1d, GradientColorStop.OffsetType.RELATIVE)).AddColorStop(new GradientColorStop(ColorConstants
                .BLUE.GetColorValue(), 0d, GradientColorStop.OffsetType.RELATIVE));
            GenerateAndComparePdfs("twoStopsAtTheEnd.pdf", targetBoundingBox, null, gradientBuilder);
        }

        [NUnit.Framework.Test]
        public virtual void BuildWithTwoStopsInTheMiddleTest() {
            Rectangle targetBoundingBox = new Rectangle(50f, 450f, 300f, 300f);
            AbstractLinearGradientBuilder gradientBuilder = new LinearGradientBuilder().SetGradientVector(targetBoundingBox
                .GetLeft() + 100f, targetBoundingBox.GetBottom() + 100f, targetBoundingBox.GetRight() - 100f, targetBoundingBox
                .GetTop() - 100f).SetSpreadMethod(GradientSpreadMethod.PAD).AddColorStop(new GradientColorStop(ColorConstants
                .RED.GetColorValue(), 0.5d, GradientColorStop.OffsetType.RELATIVE)).AddColorStop(new GradientColorStop
                (ColorConstants.BLUE.GetColorValue(), 0.5d, GradientColorStop.OffsetType.RELATIVE));
            GenerateAndComparePdfs("twoStopsInTheMiddle.pdf", targetBoundingBox, null, gradientBuilder);
        }

        [NUnit.Framework.Test]
        public virtual void BuildWithTwoStopsBeforeTheBeginningTest() {
            Rectangle targetBoundingBox = new Rectangle(50f, 450f, 300f, 300f);
            AbstractLinearGradientBuilder gradientBuilder = new LinearGradientBuilder().SetGradientVector(targetBoundingBox
                .GetLeft() + 100f, targetBoundingBox.GetBottom() + 100f, targetBoundingBox.GetRight() - 100f, targetBoundingBox
                .GetTop() - 100f).SetSpreadMethod(GradientSpreadMethod.PAD).AddColorStop(new GradientColorStop(ColorConstants
                .RED.GetColorValue(), -0.1d, GradientColorStop.OffsetType.RELATIVE)).AddColorStop(new GradientColorStop
                (ColorConstants.BLUE.GetColorValue(), -0.2d, GradientColorStop.OffsetType.RELATIVE));
            GenerateAndComparePdfs("twoStopsBeforeTheBeginning.pdf", targetBoundingBox, null, gradientBuilder);
        }

        [NUnit.Framework.Test]
        public virtual void BuildWithTwoStopsAfterTheEndTest() {
            Rectangle targetBoundingBox = new Rectangle(50f, 450f, 300f, 300f);
            AbstractLinearGradientBuilder gradientBuilder = new LinearGradientBuilder().SetGradientVector(targetBoundingBox
                .GetLeft() + 100f, targetBoundingBox.GetBottom() + 100f, targetBoundingBox.GetRight() - 100f, targetBoundingBox
                .GetTop() - 100f).SetSpreadMethod(GradientSpreadMethod.PAD).AddColorStop(new GradientColorStop(ColorConstants
                .RED.GetColorValue(), 1.2d, GradientColorStop.OffsetType.RELATIVE)).AddColorStop(new GradientColorStop
                (ColorConstants.BLUE.GetColorValue(), 0d, GradientColorStop.OffsetType.RELATIVE));
            GenerateAndComparePdfs("twoStopsAfterTheEnd.pdf", targetBoundingBox, null, gradientBuilder);
        }

        [NUnit.Framework.Test]
        public virtual void PadCaseWithVeryCloseCornerStopsTest() {
            Rectangle targetBoundingBox = new Rectangle(50f, 450f, 300f, 300f);
            AbstractLinearGradientBuilder gradientBuilder = new LinearGradientBuilder().SetGradientVector(targetBoundingBox
                .GetLeft() + 100f, targetBoundingBox.GetBottom() + 100f, targetBoundingBox.GetRight() - 100f, targetBoundingBox
                .GetTop() - 100f).SetSpreadMethod(GradientSpreadMethod.PAD).AddColorStop(new GradientColorStop(ColorConstants
                .RED.GetColorValue(), 0d, GradientColorStop.OffsetType.RELATIVE)).AddColorStop(new GradientColorStop(ColorConstants
                .GREEN.GetColorValue(), 0.01d, GradientColorStop.OffsetType.RELATIVE)).AddColorStop(new GradientColorStop
                (ColorConstants.GREEN.GetColorValue(), 0.99d, GradientColorStop.OffsetType.RELATIVE)).AddColorStop(new 
                GradientColorStop(ColorConstants.BLUE.GetColorValue(), 1d, GradientColorStop.OffsetType.RELATIVE));
            GenerateAndComparePdfs("padCaseVeryCloseCornerStops.pdf", targetBoundingBox, null, gradientBuilder);
        }

        [NUnit.Framework.Test]
        public virtual void BuildWithDoublingStopsAtEndsAndPadTest() {
            Rectangle targetBoundingBox = new Rectangle(50f, 450f, 300f, 300f);
            AbstractLinearGradientBuilder gradientBuilder = new LinearGradientBuilder().SetGradientVector(targetBoundingBox
                .GetLeft() + 100f, targetBoundingBox.GetBottom() + 100f, targetBoundingBox.GetRight() - 100f, targetBoundingBox
                .GetTop() - 100f).SetSpreadMethod(GradientSpreadMethod.PAD).AddColorStop(new GradientColorStop(ColorConstants
                .MAGENTA.GetColorValue(), -0.2, GradientColorStop.OffsetType.RELATIVE)).AddColorStop(new GradientColorStop
                (ColorConstants.ORANGE.GetColorValue(), -0.2, GradientColorStop.OffsetType.RELATIVE)).AddColorStop(new 
                GradientColorStop(ColorConstants.RED.GetColorValue(), 0d, GradientColorStop.OffsetType.RELATIVE)).AddColorStop
                (new GradientColorStop(ColorConstants.BLUE.GetColorValue(), 1d, GradientColorStop.OffsetType.RELATIVE)
                ).AddColorStop(new GradientColorStop(ColorConstants.ORANGE.GetColorValue(), 1.2, GradientColorStop.OffsetType
                .RELATIVE)).AddColorStop(new GradientColorStop(ColorConstants.MAGENTA.GetColorValue(), 1.2, GradientColorStop.OffsetType
                .RELATIVE));
            GenerateAndComparePdfs("doublingStopsAtEndsPad.pdf", targetBoundingBox, null, gradientBuilder);
        }

        [NUnit.Framework.Test]
        public virtual void BuildWithDoublingStopsAtEndsAndEndsOfCoordinatesAndPadTest() {
            Rectangle targetBoundingBox = new Rectangle(50f, 450f, 300f, 300f);
            AbstractLinearGradientBuilder gradientBuilder = new LinearGradientBuilder().SetGradientVector(targetBoundingBox
                .GetLeft() + 100f, targetBoundingBox.GetBottom() + 100f, targetBoundingBox.GetRight() - 100f, targetBoundingBox
                .GetTop() - 100f).SetSpreadMethod(GradientSpreadMethod.PAD).AddColorStop(new GradientColorStop(ColorConstants
                .MAGENTA.GetColorValue(), -0.2, GradientColorStop.OffsetType.RELATIVE)).AddColorStop(new GradientColorStop
                (ColorConstants.ORANGE.GetColorValue(), -0.2, GradientColorStop.OffsetType.RELATIVE)).AddColorStop(new 
                GradientColorStop(ColorConstants.RED.GetColorValue(), 0d, GradientColorStop.OffsetType.RELATIVE)).AddColorStop
                (new GradientColorStop(ColorConstants.MAGENTA.GetColorValue(), 0d, GradientColorStop.OffsetType.RELATIVE
                )).AddColorStop(new GradientColorStop(ColorConstants.ORANGE.GetColorValue(), 1d, GradientColorStop.OffsetType
                .RELATIVE)).AddColorStop(new GradientColorStop(ColorConstants.BLUE.GetColorValue(), 1d, GradientColorStop.OffsetType
                .RELATIVE)).AddColorStop(new GradientColorStop(ColorConstants.ORANGE.GetColorValue(), 1.2, GradientColorStop.OffsetType
                .RELATIVE)).AddColorStop(new GradientColorStop(ColorConstants.MAGENTA.GetColorValue(), 1.2, GradientColorStop.OffsetType
                .RELATIVE));
            GenerateAndComparePdfs("doublingStopsAtEndsEndsOfCoordinatesPad.pdf", targetBoundingBox, null, gradientBuilder
                );
        }

        [NUnit.Framework.Test]
        public virtual void BuildWithoutCoordinatesTest() {
            Rectangle targetBoundingBox = new Rectangle(50f, 450f, 300f, 300f);
            AbstractLinearGradientBuilder gradientBuilder = new LinearGradientBuilder().SetSpreadMethod(GradientSpreadMethod
                .PAD).AddColorStop(new GradientColorStop(ColorConstants.RED.GetColorValue(), 0d, GradientColorStop.OffsetType
                .RELATIVE)).AddColorStop(new GradientColorStop(ColorConstants.GREEN.GetColorValue(), 0.5, GradientColorStop.OffsetType
                .RELATIVE)).AddColorStop(new GradientColorStop(ColorConstants.BLUE.GetColorValue(), 1d, GradientColorStop.OffsetType
                .RELATIVE));
            GenerateAndComparePdfs("withoutCoordinates.pdf", targetBoundingBox, null, gradientBuilder);
        }

        [NUnit.Framework.Test]
        public virtual void BuildWithZeroVectorTest() {
            Rectangle targetBoundingBox = new Rectangle(50f, 450f, 300f, 300f);
            AbstractLinearGradientBuilder gradientBuilder = new LinearGradientBuilder().SetGradientVector(targetBoundingBox
                .GetLeft() + 100f, targetBoundingBox.GetBottom() + 100f, targetBoundingBox.GetLeft() + 100f, targetBoundingBox
                .GetBottom() + 100f).SetSpreadMethod(GradientSpreadMethod.PAD).AddColorStop(new GradientColorStop(ColorConstants
                .RED.GetColorValue(), 0d, GradientColorStop.OffsetType.RELATIVE)).AddColorStop(new GradientColorStop(ColorConstants
                .GREEN.GetColorValue(), 0.5, GradientColorStop.OffsetType.RELATIVE)).AddColorStop(new GradientColorStop
                (ColorConstants.BLUE.GetColorValue(), 1d, GradientColorStop.OffsetType.RELATIVE));
            GenerateAndComparePdfs("zeroVector.pdf", targetBoundingBox, null, gradientBuilder);
        }

        [NUnit.Framework.Test]
        public virtual void BuildWithNullArgumentsAndWithoutStopsTest() {
            Rectangle targetBoundingBox = new Rectangle(50f, 450f, 300f, 300f);
            AbstractLinearGradientBuilder gradientBuilder = new LinearGradientBuilder().SetGradientVector(targetBoundingBox
                .GetLeft() + 100f, targetBoundingBox.GetBottom() + 100f, targetBoundingBox.GetRight() - 100f, targetBoundingBox
                .GetTop() - 100f).SetSpreadMethod(GradientSpreadMethod.PAD);
            NUnit.Framework.Assert.IsNull(gradientBuilder.BuildColor(null, null, null));
        }

        [NUnit.Framework.Test]
        public virtual void BuildWithNullArgumentsAndNoneSpreadingTest() {
            Rectangle targetBoundingBox = new Rectangle(50f, 450f, 300f, 300f);
            AbstractLinearGradientBuilder gradientBuilder = new LinearGradientBuilder().SetGradientVector(targetBoundingBox
                .GetLeft() + 100f, targetBoundingBox.GetBottom() + 100f, targetBoundingBox.GetRight() - 100f, targetBoundingBox
                .GetTop() - 100f).SetSpreadMethod(GradientSpreadMethod.NONE).AddColorStop(new GradientColorStop(ColorConstants
                .RED.GetColorValue(), 0d, GradientColorStop.OffsetType.RELATIVE)).AddColorStop(new GradientColorStop(ColorConstants
                .GREEN.GetColorValue(), 0.5, GradientColorStop.OffsetType.RELATIVE)).AddColorStop(new GradientColorStop
                (ColorConstants.BLUE.GetColorValue(), 1d, GradientColorStop.OffsetType.RELATIVE));
            GenerateAndComparePdfsWithoutArgumentToBuild("nullArgumentsNoneSpreading.pdf", targetBoundingBox, gradientBuilder
                );
        }

        [NUnit.Framework.Test]
        public virtual void BuildWithNullArgumentsAndPadSpreadingTest() {
            Rectangle targetBoundingBox = new Rectangle(50f, 450f, 300f, 300f);
            AbstractLinearGradientBuilder gradientBuilder = new LinearGradientBuilder().SetGradientVector(targetBoundingBox
                .GetLeft() + 100f, targetBoundingBox.GetBottom() + 100f, targetBoundingBox.GetRight() - 100f, targetBoundingBox
                .GetTop() - 100f).SetSpreadMethod(GradientSpreadMethod.PAD).AddColorStop(new GradientColorStop(ColorConstants
                .RED.GetColorValue(), 0d, GradientColorStop.OffsetType.RELATIVE)).AddColorStop(new GradientColorStop(ColorConstants
                .GREEN.GetColorValue(), 0.5, GradientColorStop.OffsetType.RELATIVE)).AddColorStop(new GradientColorStop
                (ColorConstants.BLUE.GetColorValue(), 1d, GradientColorStop.OffsetType.RELATIVE));
            GenerateAndComparePdfsWithoutArgumentToBuild("nullArgumentsPadSpreading.pdf", targetBoundingBox, gradientBuilder
                );
        }

        [NUnit.Framework.Test]
        public virtual void BuildWithNullArgumentsAndReflectSpreadingTest() {
            Rectangle targetBoundingBox = new Rectangle(50f, 450f, 300f, 300f);
            AbstractLinearGradientBuilder gradientBuilder = new LinearGradientBuilder().SetGradientVector(targetBoundingBox
                .GetLeft() + 100f, targetBoundingBox.GetBottom() + 100f, targetBoundingBox.GetRight() - 100f, targetBoundingBox
                .GetTop() - 100f).SetSpreadMethod(GradientSpreadMethod.REFLECT).AddColorStop(new GradientColorStop(ColorConstants
                .RED.GetColorValue(), 0d, GradientColorStop.OffsetType.RELATIVE)).AddColorStop(new GradientColorStop(ColorConstants
                .GREEN.GetColorValue(), 0.5, GradientColorStop.OffsetType.RELATIVE)).AddColorStop(new GradientColorStop
                (ColorConstants.BLUE.GetColorValue(), 1d, GradientColorStop.OffsetType.RELATIVE));
            GenerateAndComparePdfsWithoutArgumentToBuild("nullArgumentsReflectSpreading.pdf", targetBoundingBox, gradientBuilder
                );
        }

        [NUnit.Framework.Test]
        public virtual void BuildWithNullArgumentsAndRepeatSpreadingTest() {
            Rectangle targetBoundingBox = new Rectangle(50f, 450f, 300f, 300f);
            AbstractLinearGradientBuilder gradientBuilder = new LinearGradientBuilder().SetGradientVector(targetBoundingBox
                .GetLeft() + 100f, targetBoundingBox.GetBottom() + 100f, targetBoundingBox.GetRight() - 100f, targetBoundingBox
                .GetTop() - 100f).SetSpreadMethod(GradientSpreadMethod.REPEAT).AddColorStop(new GradientColorStop(ColorConstants
                .RED.GetColorValue(), 0d, GradientColorStop.OffsetType.RELATIVE)).AddColorStop(new GradientColorStop(ColorConstants
                .GREEN.GetColorValue(), 0.5, GradientColorStop.OffsetType.RELATIVE)).AddColorStop(new GradientColorStop
                (ColorConstants.BLUE.GetColorValue(), 1d, GradientColorStop.OffsetType.RELATIVE));
            GenerateAndComparePdfsWithoutArgumentToBuild("nullArgumentsRepeatSpreading.pdf", targetBoundingBox, gradientBuilder
                );
        }

        [NUnit.Framework.Test]
        public virtual void BuilderWithNoneSpreadingTest() {
            Rectangle targetBoundingBox = new Rectangle(50f, 450f, 300f, 300f);
            AbstractLinearGradientBuilder gradientBuilder = new LinearGradientBuilder().SetGradientVector(targetBoundingBox
                .GetLeft() + 100f, targetBoundingBox.GetBottom() + 100f, targetBoundingBox.GetRight() - 100f, targetBoundingBox
                .GetTop() - 100f).SetSpreadMethod(GradientSpreadMethod.NONE).AddColorStop(new GradientColorStop(ColorConstants
                .RED.GetColorValue(), 0d, GradientColorStop.OffsetType.RELATIVE)).AddColorStop(new GradientColorStop(ColorConstants
                .GREEN.GetColorValue(), 0.5, GradientColorStop.OffsetType.RELATIVE)).AddColorStop(new GradientColorStop
                (ColorConstants.BLUE.GetColorValue(), 1d, GradientColorStop.OffsetType.RELATIVE));
            GenerateAndComparePdfs("noneSpreading.pdf", targetBoundingBox, null, gradientBuilder);
        }

        [NUnit.Framework.Test]
        public virtual void BuilderWithNoneSpreadingAndCanvasTransformTest() {
            Rectangle targetBoundingBox = new Rectangle(50f, 450f, 300f, 300f);
            AbstractLinearGradientBuilder gradientBuilder = new LinearGradientBuilder().SetGradientVector(targetBoundingBox
                .GetLeft() + 100f, targetBoundingBox.GetBottom() + 100f, targetBoundingBox.GetRight() - 100f, targetBoundingBox
                .GetTop() - 100f).SetSpreadMethod(GradientSpreadMethod.NONE).AddColorStop(new GradientColorStop(ColorConstants
                .RED.GetColorValue(), 0d, GradientColorStop.OffsetType.RELATIVE)).AddColorStop(new GradientColorStop(ColorConstants
                .GREEN.GetColorValue(), 0.5, GradientColorStop.OffsetType.RELATIVE)).AddColorStop(new GradientColorStop
                (ColorConstants.BLUE.GetColorValue(), 1d, GradientColorStop.OffsetType.RELATIVE));
            AffineTransform canvasTransform = AffineTransform.GetTranslateInstance(50, -50);
            canvasTransform.Scale(0.8, 1.1);
            canvasTransform.Rotate(Math.PI / 3, 400f, 550f);
            GenerateAndComparePdfs("noneSpreadingCanvasTransform.pdf", targetBoundingBox, canvasTransform, gradientBuilder
                );
        }

        [NUnit.Framework.Test]
        public virtual void BuilderWithNoneSpreadingAndAllTransformsTest() {
            Rectangle targetBoundingBox = new Rectangle(50f, 450f, 300f, 300f);
            AffineTransform gradientTransform = AffineTransform.GetTranslateInstance(150, -50);
            gradientTransform.Scale(0.5, 1.5);
            gradientTransform.Rotate(Math.PI / 3, 400f, 550f);
            AbstractLinearGradientBuilder gradientBuilder = new LinearGradientBuilder().SetGradientVector(targetBoundingBox
                .GetLeft() + 100f, targetBoundingBox.GetBottom() + 100f, targetBoundingBox.GetRight() - 100f, targetBoundingBox
                .GetTop() - 100f).SetCurrentSpaceToGradientVectorSpaceTransformation(gradientTransform).SetSpreadMethod
                (GradientSpreadMethod.NONE).AddColorStop(new GradientColorStop(ColorConstants.RED.GetColorValue(), 0d, 
                GradientColorStop.OffsetType.RELATIVE)).AddColorStop(new GradientColorStop(ColorConstants.GREEN.GetColorValue
                (), 0.5, GradientColorStop.OffsetType.RELATIVE)).AddColorStop(new GradientColorStop(ColorConstants.BLUE
                .GetColorValue(), 1d, GradientColorStop.OffsetType.RELATIVE));
            AffineTransform canvasTransform = AffineTransform.GetTranslateInstance(50, -50);
            canvasTransform.Scale(0.8, 1.1);
            canvasTransform.Rotate(Math.PI / 3, 400f, 550f);
            GenerateAndComparePdfs("noneSpreadingAllTransforms.pdf", targetBoundingBox, canvasTransform, gradientBuilder
                );
        }

        [NUnit.Framework.Test]
        public virtual void BuilderWithPadSpreadingTest() {
            Rectangle targetBoundingBox = new Rectangle(50f, 450f, 300f, 300f);
            AbstractLinearGradientBuilder gradientBuilder = new LinearGradientBuilder().SetGradientVector(targetBoundingBox
                .GetLeft() + 100f, targetBoundingBox.GetBottom() + 100f, targetBoundingBox.GetRight() - 100f, targetBoundingBox
                .GetTop() - 100f).SetSpreadMethod(GradientSpreadMethod.PAD).AddColorStop(new GradientColorStop(ColorConstants
                .RED.GetColorValue(), 0d, GradientColorStop.OffsetType.RELATIVE)).AddColorStop(new GradientColorStop(ColorConstants
                .GREEN.GetColorValue(), 0.5, GradientColorStop.OffsetType.RELATIVE)).AddColorStop(new GradientColorStop
                (ColorConstants.BLUE.GetColorValue(), 1d, GradientColorStop.OffsetType.RELATIVE));
            GenerateAndComparePdfs("padSpreading.pdf", targetBoundingBox, null, gradientBuilder);
        }

        [NUnit.Framework.Test]
        public virtual void BuilderWithReflectSpreadingTest() {
            Rectangle targetBoundingBox = new Rectangle(50f, 450f, 300f, 300f);
            AbstractLinearGradientBuilder gradientBuilder = new LinearGradientBuilder().SetGradientVector(targetBoundingBox
                .GetLeft() + 100f, targetBoundingBox.GetBottom() + 100f, targetBoundingBox.GetRight() - 100f, targetBoundingBox
                .GetTop() - 100f).SetSpreadMethod(GradientSpreadMethod.REFLECT).AddColorStop(new GradientColorStop(ColorConstants
                .RED.GetColorValue(), 0d, GradientColorStop.OffsetType.RELATIVE)).AddColorStop(new GradientColorStop(ColorConstants
                .GREEN.GetColorValue(), 0.5, GradientColorStop.OffsetType.RELATIVE)).AddColorStop(new GradientColorStop
                (ColorConstants.BLUE.GetColorValue(), 1d, GradientColorStop.OffsetType.RELATIVE));
            GenerateAndComparePdfs("reflectSpreading.pdf", targetBoundingBox, null, gradientBuilder);
        }

        [NUnit.Framework.Test]
        public virtual void BuilderWithRepeatSpreadingTest() {
            Rectangle targetBoundingBox = new Rectangle(50f, 450f, 300f, 300f);
            AbstractLinearGradientBuilder gradientBuilder = new LinearGradientBuilder().SetGradientVector(targetBoundingBox
                .GetLeft() + 100f, targetBoundingBox.GetBottom() + 100f, targetBoundingBox.GetRight() - 100f, targetBoundingBox
                .GetTop() - 100f).SetSpreadMethod(GradientSpreadMethod.REPEAT).AddColorStop(new GradientColorStop(ColorConstants
                .RED.GetColorValue(), 0d, GradientColorStop.OffsetType.RELATIVE)).AddColorStop(new GradientColorStop(ColorConstants
                .GREEN.GetColorValue(), 0.5, GradientColorStop.OffsetType.RELATIVE)).AddColorStop(new GradientColorStop
                (ColorConstants.BLUE.GetColorValue(), 1d, GradientColorStop.OffsetType.RELATIVE));
            GenerateAndComparePdfs("repeatSpreading.pdf", targetBoundingBox, null, gradientBuilder);
        }

        [NUnit.Framework.Test]
        public virtual void BuilderWithRepeatSpreadingAndAllTransformsTest() {
            Rectangle targetBoundingBox = new Rectangle(50f, 450f, 300f, 300f);
            AffineTransform gradientTransform = AffineTransform.GetTranslateInstance(150, -50);
            gradientTransform.Scale(0.5, 1.5);
            gradientTransform.Rotate(Math.PI / 3, 400f, 550f);
            AbstractLinearGradientBuilder gradientBuilder = new LinearGradientBuilder().SetGradientVector(targetBoundingBox
                .GetLeft() + 100f, targetBoundingBox.GetBottom() + 100f, targetBoundingBox.GetRight() - 100f, targetBoundingBox
                .GetTop() - 100f).SetCurrentSpaceToGradientVectorSpaceTransformation(gradientTransform).SetSpreadMethod
                (GradientSpreadMethod.REPEAT).AddColorStop(new GradientColorStop(ColorConstants.RED.GetColorValue(), 0d
                , GradientColorStop.OffsetType.RELATIVE)).AddColorStop(new GradientColorStop(ColorConstants.GREEN.GetColorValue
                (), 0.5, GradientColorStop.OffsetType.RELATIVE)).AddColorStop(new GradientColorStop(ColorConstants.BLUE
                .GetColorValue(), 1d, GradientColorStop.OffsetType.RELATIVE));
            AffineTransform canvasTransform = AffineTransform.GetTranslateInstance(50, -50);
            canvasTransform.Scale(0.8, 1.1);
            canvasTransform.Rotate(Math.PI / 3, 400f, 550f);
            GenerateAndComparePdfs("repeatSpreadingAllTransforms.pdf", targetBoundingBox, canvasTransform, gradientBuilder
                );
        }

        [NUnit.Framework.Test]
        public virtual void BuilderWithRepeatSpreadingAndToRightVectorTest() {
            Rectangle targetBoundingBox = new Rectangle(50f, 450f, 300f, 300f);
            AbstractLinearGradientBuilder gradientBuilder = new LinearGradientBuilder().SetGradientVector(targetBoundingBox
                .GetRight() + 100f, 0f, targetBoundingBox.GetRight() + 300f, 0f).SetSpreadMethod(GradientSpreadMethod.
                REPEAT).AddColorStop(new GradientColorStop(ColorConstants.RED.GetColorValue())).AddColorStop(new GradientColorStop
                (ColorConstants.GREEN.GetColorValue())).AddColorStop(new GradientColorStop(ColorConstants.BLUE.GetColorValue
                ()));
            GenerateAndComparePdfs("repeatSpreadingToRightVector.pdf", targetBoundingBox, null, gradientBuilder);
        }

        [NUnit.Framework.Test]
        public virtual void BuilderWithRepeatSpreadingAndToLeftVectorTest() {
            Rectangle targetBoundingBox = new Rectangle(50f, 450f, 300f, 300f);
            AbstractLinearGradientBuilder gradientBuilder = new LinearGradientBuilder().SetGradientVector(targetBoundingBox
                .GetRight() + 300f, 0f, targetBoundingBox.GetRight() + 100f, 0f).SetSpreadMethod(GradientSpreadMethod.
                REPEAT).AddColorStop(new GradientColorStop(ColorConstants.RED.GetColorValue())).AddColorStop(new GradientColorStop
                (ColorConstants.GREEN.GetColorValue())).AddColorStop(new GradientColorStop(ColorConstants.BLUE.GetColorValue
                ()));
            GenerateAndComparePdfs("repeatSpreadingToLeftVector.pdf", targetBoundingBox, null, gradientBuilder);
        }

        [NUnit.Framework.Test]
        public virtual void BuilderWithRepeatSpreadingAndToTopVectorTest() {
            Rectangle targetBoundingBox = new Rectangle(50f, 450f, 300f, 300f);
            AbstractLinearGradientBuilder gradientBuilder = new LinearGradientBuilder().SetGradientVector(0f, targetBoundingBox
                .GetBottom() - 300f, 0f, targetBoundingBox.GetBottom() - 100f).SetSpreadMethod(GradientSpreadMethod.REPEAT
                ).AddColorStop(new GradientColorStop(ColorConstants.RED.GetColorValue())).AddColorStop(new GradientColorStop
                (ColorConstants.GREEN.GetColorValue())).AddColorStop(new GradientColorStop(ColorConstants.BLUE.GetColorValue
                ()));
            GenerateAndComparePdfs("repeatSpreadingToTopVector.pdf", targetBoundingBox, null, gradientBuilder);
        }

        [NUnit.Framework.Test]
        public virtual void BuilderWithRepeatSpreadingAndToBottomVectorTest() {
            Rectangle targetBoundingBox = new Rectangle(50f, 450f, 300f, 300f);
            AbstractLinearGradientBuilder gradientBuilder = new LinearGradientBuilder().SetGradientVector(0f, targetBoundingBox
                .GetBottom() - 100f, 0f, targetBoundingBox.GetBottom() - 300f).SetSpreadMethod(GradientSpreadMethod.REPEAT
                ).AddColorStop(new GradientColorStop(ColorConstants.RED.GetColorValue())).AddColorStop(new GradientColorStop
                (ColorConstants.GREEN.GetColorValue())).AddColorStop(new GradientColorStop(ColorConstants.BLUE.GetColorValue
                ()));
            GenerateAndComparePdfs("repeatSpreadingToBottomVector.pdf", targetBoundingBox, null, gradientBuilder);
        }

        [NUnit.Framework.Test]
        public virtual void BuildWithAutoStopAndAbsoluteOnCoordinatesHintTest() {
            Rectangle targetBoundingBox = new Rectangle(50f, 450f, 300f, 300f);
            AbstractLinearGradientBuilder gradientBuilder = new LinearGradientBuilder().SetGradientVector(targetBoundingBox
                .GetLeft(), targetBoundingBox.GetBottom() + 100f, targetBoundingBox.GetRight(), targetBoundingBox.GetBottom
                () + 100f).SetSpreadMethod(GradientSpreadMethod.NONE).AddColorStop(new GradientColorStop(ColorConstants
                .RED.GetColorValue(), 0.1d, GradientColorStop.OffsetType.RELATIVE)).AddColorStop(new GradientColorStop
                (ColorConstants.GREEN.GetColorValue()).SetHint(100f, GradientColorStop.HintOffsetType.ABSOLUTE_ON_GRADIENT
                )).AddColorStop(new GradientColorStop(ColorConstants.BLUE.GetColorValue(), 0.9d, GradientColorStop.OffsetType
                .RELATIVE));
            GenerateAndComparePdfs("autoStopAbsoluteOnCoordinatesHint.pdf", targetBoundingBox, null, gradientBuilder);
        }

        [NUnit.Framework.Test]
        public virtual void BuildWithAutoStopAndRelativeOnCoordinatesHintTest() {
            Rectangle targetBoundingBox = new Rectangle(50f, 450f, 300f, 300f);
            AbstractLinearGradientBuilder gradientBuilder = new LinearGradientBuilder().SetGradientVector(targetBoundingBox
                .GetLeft(), targetBoundingBox.GetBottom() + 100f, targetBoundingBox.GetRight(), targetBoundingBox.GetBottom
                () + 100f).SetSpreadMethod(GradientSpreadMethod.NONE).AddColorStop(new GradientColorStop(ColorConstants
                .RED.GetColorValue(), 0.1d, GradientColorStop.OffsetType.RELATIVE)).AddColorStop(new GradientColorStop
                (ColorConstants.GREEN.GetColorValue()).SetHint(0.2f, GradientColorStop.HintOffsetType.RELATIVE_ON_GRADIENT
                )).AddColorStop(new GradientColorStop(ColorConstants.BLUE.GetColorValue(), 0.9d, GradientColorStop.OffsetType
                .RELATIVE));
            GenerateAndComparePdfs("autoStopRelativeOnCoordinatesHint.pdf", targetBoundingBox, null, gradientBuilder);
        }

        [NUnit.Framework.Test]
        public virtual void BuildWithAutoStopAndRelativeBetweenColorsHintTest() {
            Rectangle targetBoundingBox = new Rectangle(50f, 450f, 300f, 300f);
            AbstractLinearGradientBuilder gradientBuilder = new LinearGradientBuilder().SetGradientVector(targetBoundingBox
                .GetLeft(), targetBoundingBox.GetBottom() + 100f, targetBoundingBox.GetRight(), targetBoundingBox.GetBottom
                () + 100f).SetSpreadMethod(GradientSpreadMethod.NONE).AddColorStop(new GradientColorStop(ColorConstants
                .RED.GetColorValue(), 0.1d, GradientColorStop.OffsetType.RELATIVE)).AddColorStop(new GradientColorStop
                (ColorConstants.GREEN.GetColorValue()).SetHint(0.2f, GradientColorStop.HintOffsetType.RELATIVE_BETWEEN_COLORS
                )).AddColorStop(new GradientColorStop(ColorConstants.BLUE.GetColorValue(), 0.9d, GradientColorStop.OffsetType
                .RELATIVE));
            GenerateAndComparePdfs("autoStopRelativeBetweenColorsHint.pdf", targetBoundingBox, null, gradientBuilder);
        }

        [NUnit.Framework.Test]
        public virtual void BuildWithAutoStopAndRelativeBetweenColorsZeroHintTest() {
            Rectangle targetBoundingBox = new Rectangle(50f, 450f, 300f, 300f);
            AbstractLinearGradientBuilder gradientBuilder = new LinearGradientBuilder().SetGradientVector(targetBoundingBox
                .GetLeft(), targetBoundingBox.GetBottom() + 100f, targetBoundingBox.GetRight(), targetBoundingBox.GetBottom
                () + 100f).SetSpreadMethod(GradientSpreadMethod.NONE).AddColorStop(new GradientColorStop(ColorConstants
                .RED.GetColorValue(), 0.1d, GradientColorStop.OffsetType.RELATIVE)).AddColorStop(new GradientColorStop
                (ColorConstants.GREEN.GetColorValue()).SetHint(0f, GradientColorStop.HintOffsetType.RELATIVE_BETWEEN_COLORS
                )).AddColorStop(new GradientColorStop(ColorConstants.BLUE.GetColorValue(), 0.9d, GradientColorStop.OffsetType
                .RELATIVE));
            GenerateAndComparePdfs("autoStopRelativeBetweenColorsZeroHint.pdf", targetBoundingBox, null, gradientBuilder
                );
        }

        [NUnit.Framework.Test]
        public virtual void BuildWithAutoStopAndRelativeBetweenColorsOneHintTest() {
            Rectangle targetBoundingBox = new Rectangle(50f, 450f, 300f, 300f);
            AbstractLinearGradientBuilder gradientBuilder = new LinearGradientBuilder().SetGradientVector(targetBoundingBox
                .GetLeft(), targetBoundingBox.GetBottom() + 100f, targetBoundingBox.GetRight(), targetBoundingBox.GetBottom
                () + 100f).SetSpreadMethod(GradientSpreadMethod.NONE).AddColorStop(new GradientColorStop(ColorConstants
                .RED.GetColorValue(), 0.1d, GradientColorStop.OffsetType.RELATIVE)).AddColorStop(new GradientColorStop
                (ColorConstants.GREEN.GetColorValue()).SetHint(1f, GradientColorStop.HintOffsetType.RELATIVE_BETWEEN_COLORS
                )).AddColorStop(new GradientColorStop(ColorConstants.BLUE.GetColorValue(), 0.9d, GradientColorStop.OffsetType
                .RELATIVE));
            GenerateAndComparePdfs("autoStopRelativeBetweenColorsOneHint.pdf", targetBoundingBox, null, gradientBuilder
                );
        }

        [NUnit.Framework.Test]
        public virtual void BuildWithPadSpreadingAndRelativeBetweenColorsZeroHintTest() {
            Rectangle targetBoundingBox = new Rectangle(50f, 450f, 300f, 300f);
            AbstractLinearGradientBuilder gradientBuilder = new LinearGradientBuilder().SetGradientVector(targetBoundingBox
                .GetLeft(), targetBoundingBox.GetBottom() + 100f, targetBoundingBox.GetRight(), targetBoundingBox.GetBottom
                () + 100f).SetSpreadMethod(GradientSpreadMethod.PAD).AddColorStop(new GradientColorStop(ColorConstants
                .RED.GetColorValue(), 0.1d, GradientColorStop.OffsetType.RELATIVE).SetHint(0f, GradientColorStop.HintOffsetType
                .RELATIVE_BETWEEN_COLORS)).AddColorStop(new GradientColorStop(ColorConstants.BLUE.GetColorValue(), 0.9d
                , GradientColorStop.OffsetType.RELATIVE));
            GenerateAndComparePdfs("padSpreadingRelativeBetweenColorsZeroHint.pdf", targetBoundingBox, null, gradientBuilder
                );
        }

        [NUnit.Framework.Test]
        public virtual void BuildWithPadSpreadingAndRelativeBetweenColorsOneHintTest() {
            Rectangle targetBoundingBox = new Rectangle(50f, 450f, 300f, 300f);
            AbstractLinearGradientBuilder gradientBuilder = new LinearGradientBuilder().SetGradientVector(targetBoundingBox
                .GetLeft(), targetBoundingBox.GetBottom() + 100f, targetBoundingBox.GetRight(), targetBoundingBox.GetBottom
                () + 100f).SetSpreadMethod(GradientSpreadMethod.PAD).AddColorStop(new GradientColorStop(ColorConstants
                .RED.GetColorValue(), 0.1d, GradientColorStop.OffsetType.RELATIVE).SetHint(1f, GradientColorStop.HintOffsetType
                .RELATIVE_BETWEEN_COLORS)).AddColorStop(new GradientColorStop(ColorConstants.BLUE.GetColorValue(), 0.9d
                , GradientColorStop.OffsetType.RELATIVE));
            GenerateAndComparePdfs("padSpreadingRelativeBetweenColorsOneHint.pdf", targetBoundingBox, null, gradientBuilder
                );
        }

        [NUnit.Framework.Test]
        public virtual void BuildWithAutoStopAndNoneHintTypeTest() {
            Rectangle targetBoundingBox = new Rectangle(50f, 450f, 300f, 300f);
            AbstractLinearGradientBuilder gradientBuilder = new LinearGradientBuilder().SetGradientVector(targetBoundingBox
                .GetLeft(), targetBoundingBox.GetBottom() + 100f, targetBoundingBox.GetRight(), targetBoundingBox.GetBottom
                () + 100f).SetSpreadMethod(GradientSpreadMethod.NONE).AddColorStop(new GradientColorStop(ColorConstants
                .RED.GetColorValue(), 0.1d, GradientColorStop.OffsetType.RELATIVE)).AddColorStop(new GradientColorStop
                (ColorConstants.GREEN.GetColorValue()).SetHint(0.2f, GradientColorStop.HintOffsetType.NONE)).AddColorStop
                (new GradientColorStop(ColorConstants.BLUE.GetColorValue(), 0.9d, GradientColorStop.OffsetType.RELATIVE
                ));
            GenerateAndComparePdfs("autoStopNoneHintType.pdf", targetBoundingBox, null, gradientBuilder);
        }

        [NUnit.Framework.Test]
        public virtual void BuildWithReflectSpreadingAndStopsOutsideCoordinatesTest() {
            Rectangle targetBoundingBox = new Rectangle(50f, 450f, 300f, 300f);
            AbstractLinearGradientBuilder gradientBuilder = new LinearGradientBuilder().SetGradientVector(targetBoundingBox
                .GetLeft() + 100f, targetBoundingBox.GetBottom() + 100f, targetBoundingBox.GetRight() - 100f, targetBoundingBox
                .GetBottom() + 100f).SetSpreadMethod(GradientSpreadMethod.REFLECT).AddColorStop(new GradientColorStop(
                ColorConstants.RED.GetColorValue(), -0.5d, GradientColorStop.OffsetType.RELATIVE)).AddColorStop(new GradientColorStop
                (ColorConstants.BLUE.GetColorValue(), 1.5d, GradientColorStop.OffsetType.RELATIVE));
            GenerateAndComparePdfs("reflectSpreadingStopsOutsideCoordinates.pdf", targetBoundingBox, null, gradientBuilder
                );
        }

        [NUnit.Framework.Test]
        public virtual void BuildWithSingleAutoStopsAtStartAndEndTest() {
            Rectangle targetBoundingBox = new Rectangle(50f, 450f, 300f, 300f);
            AbstractLinearGradientBuilder gradientBuilder = new LinearGradientBuilder().SetGradientVector(targetBoundingBox
                .GetLeft() + 10f, targetBoundingBox.GetBottom() + 100f, targetBoundingBox.GetRight() - 10f, targetBoundingBox
                .GetBottom() + 100f).SetSpreadMethod(GradientSpreadMethod.NONE).AddColorStop(new GradientColorStop(ColorConstants
                .RED.GetColorValue()).SetHint(0.1, GradientColorStop.HintOffsetType.RELATIVE_BETWEEN_COLORS)).AddColorStop
                (new GradientColorStop(ColorConstants.BLUE.GetColorValue(), 0.5d, GradientColorStop.OffsetType.RELATIVE
                )).AddColorStop(new GradientColorStop(ColorConstants.GREEN.GetColorValue(), 200d, GradientColorStop.OffsetType
                .ABSOLUTE)).AddColorStop(new GradientColorStop(ColorConstants.RED.GetColorValue()).SetHint(0.1, GradientColorStop.HintOffsetType
                .RELATIVE_BETWEEN_COLORS));
            GenerateAndComparePdfs("singleAutoStopsAtStartAndEnd.pdf", targetBoundingBox, null, gradientBuilder);
        }

        [NUnit.Framework.Test]
        public virtual void BuildWithSingleAutoStopsAtStartAndEndWithHintsTest() {
            Rectangle targetBoundingBox = new Rectangle(50f, 450f, 300f, 300f);
            AbstractLinearGradientBuilder gradientBuilder = new LinearGradientBuilder().SetGradientVector(targetBoundingBox
                .GetLeft() + 10f, targetBoundingBox.GetBottom() + 100f, targetBoundingBox.GetRight() - 10f, targetBoundingBox
                .GetBottom() + 100f).SetSpreadMethod(GradientSpreadMethod.NONE).AddColorStop(new GradientColorStop(ColorConstants
                .RED.GetColorValue()).SetHint(0.1, GradientColorStop.HintOffsetType.RELATIVE_ON_GRADIENT)).AddColorStop
                (new GradientColorStop(ColorConstants.BLUE.GetColorValue(), 0.5d, GradientColorStop.OffsetType.RELATIVE
                )).AddColorStop(new GradientColorStop(ColorConstants.GREEN.GetColorValue(), 200d, GradientColorStop.OffsetType
                .ABSOLUTE)).AddColorStop(new GradientColorStop(ColorConstants.RED.GetColorValue()).SetHint(0.9, GradientColorStop.HintOffsetType
                .RELATIVE_ON_GRADIENT));
            GenerateAndComparePdfs("singleAutoStopsAtStartAndEndWithHints.pdf", targetBoundingBox, null, gradientBuilder
                );
        }

        [NUnit.Framework.Test]
        public virtual void BuildWithMultipleAutoStopsAtStartAndEndWithHintsTest() {
            Rectangle targetBoundingBox = new Rectangle(50f, 450f, 300f, 300f);
            AbstractLinearGradientBuilder gradientBuilder = new LinearGradientBuilder().SetGradientVector(targetBoundingBox
                .GetLeft() + 10f, targetBoundingBox.GetBottom() + 100f, targetBoundingBox.GetRight() - 10f, targetBoundingBox
                .GetBottom() + 100f).SetSpreadMethod(GradientSpreadMethod.NONE).AddColorStop(new GradientColorStop(ColorConstants
                .RED.GetColorValue())).AddColorStop(new GradientColorStop(ColorConstants.GREEN.GetColorValue())).AddColorStop
                (new GradientColorStop(ColorConstants.BLUE.GetColorValue(), 0.5d, GradientColorStop.OffsetType.RELATIVE
                )).AddColorStop(new GradientColorStop(ColorConstants.GREEN.GetColorValue(), 200d, GradientColorStop.OffsetType
                .ABSOLUTE)).AddColorStop(new GradientColorStop(ColorConstants.RED.GetColorValue())).AddColorStop(new GradientColorStop
                (ColorConstants.GREEN.GetColorValue()));
            GenerateAndComparePdfs("multipleAutoStopsAtStartAndEndWithHints.pdf", targetBoundingBox, null, gradientBuilder
                );
        }

        [NUnit.Framework.Test]
        public virtual void BuildWithAutoStopsInTheMiddleTest() {
            Rectangle targetBoundingBox = new Rectangle(50f, 450f, 300f, 300f);
            AbstractLinearGradientBuilder gradientBuilder = new LinearGradientBuilder().SetGradientVector(targetBoundingBox
                .GetLeft() + 10f, targetBoundingBox.GetBottom() + 100f, targetBoundingBox.GetRight() - 10f, targetBoundingBox
                .GetBottom() + 100f).SetSpreadMethod(GradientSpreadMethod.NONE).AddColorStop(new GradientColorStop(ColorConstants
                .RED.GetColorValue(), 0d, GradientColorStop.OffsetType.RELATIVE).SetHint(0.3d, GradientColorStop.HintOffsetType
                .RELATIVE_BETWEEN_COLORS)).AddColorStop(new GradientColorStop(ColorConstants.BLUE.GetColorValue()).SetHint
                (0.3d, GradientColorStop.HintOffsetType.RELATIVE_BETWEEN_COLORS)).AddColorStop(new GradientColorStop(ColorConstants
                .GREEN.GetColorValue()).SetHint(0.3d, GradientColorStop.HintOffsetType.RELATIVE_BETWEEN_COLORS)).AddColorStop
                (new GradientColorStop(ColorConstants.RED.GetColorValue(), 1d, GradientColorStop.OffsetType.RELATIVE));
            GenerateAndComparePdfs("autoStopsInTheMiddle.pdf", targetBoundingBox, null, gradientBuilder);
        }

        [NUnit.Framework.Test]
        public virtual void BuildWithAutoStopsInTheMiddleWithHintsTest() {
            Rectangle targetBoundingBox = new Rectangle(50f, 450f, 300f, 300f);
            AbstractLinearGradientBuilder gradientBuilder = new LinearGradientBuilder().SetGradientVector(targetBoundingBox
                .GetLeft() + 10f, targetBoundingBox.GetBottom() + 100f, targetBoundingBox.GetRight() - 10f, targetBoundingBox
                .GetBottom() + 100f).SetSpreadMethod(GradientSpreadMethod.NONE).AddColorStop(new GradientColorStop(ColorConstants
                .RED.GetColorValue(), 0d, GradientColorStop.OffsetType.RELATIVE).SetHint(0.2d, GradientColorStop.HintOffsetType
                .RELATIVE_ON_GRADIENT)).AddColorStop(new GradientColorStop(ColorConstants.BLUE.GetColorValue())).AddColorStop
                (new GradientColorStop(ColorConstants.GREEN.GetColorValue()).SetHint(0.7d, GradientColorStop.HintOffsetType
                .RELATIVE_ON_GRADIENT)).AddColorStop(new GradientColorStop(ColorConstants.RED.GetColorValue(), 1d, GradientColorStop.OffsetType
                .RELATIVE));
            GenerateAndComparePdfs("autoStopsInTheMiddleWithHints.pdf", targetBoundingBox, null, gradientBuilder);
        }

        [NUnit.Framework.Test]
        public virtual void BuildWithDecreasingOffsetsTest() {
            Rectangle targetBoundingBox = new Rectangle(50f, 450f, 300f, 300f);
            AbstractLinearGradientBuilder gradientBuilder = new LinearGradientBuilder().SetGradientVector(targetBoundingBox
                .GetLeft() + 10f, targetBoundingBox.GetBottom() + 100f, targetBoundingBox.GetRight() - 10f, targetBoundingBox
                .GetBottom() + 100f).SetSpreadMethod(GradientSpreadMethod.PAD).AddColorStop(new GradientColorStop(ColorConstants
                .RED.GetColorValue(), 0.5d, GradientColorStop.OffsetType.RELATIVE).SetHint(0.4d, GradientColorStop.HintOffsetType
                .RELATIVE_ON_GRADIENT)).AddColorStop(new GradientColorStop(ColorConstants.BLUE.GetColorValue(), 0.6d, 
                GradientColorStop.OffsetType.RELATIVE)).AddColorStop(new GradientColorStop(ColorConstants.GREEN.GetColorValue
                (), 100d, GradientColorStop.OffsetType.ABSOLUTE).SetHint(0.3d, GradientColorStop.HintOffsetType.RELATIVE_BETWEEN_COLORS
                )).AddColorStop(new GradientColorStop(ColorConstants.RED.GetColorValue(), 0.9d, GradientColorStop.OffsetType
                .RELATIVE).SetHint(120d, GradientColorStop.HintOffsetType.ABSOLUTE_ON_GRADIENT)).AddColorStop(new GradientColorStop
                (ColorConstants.BLUE.GetColorValue(), 1d, GradientColorStop.OffsetType.RELATIVE));
            GenerateAndComparePdfs("decreasingOffsets.pdf", targetBoundingBox, null, gradientBuilder);
        }

        [NUnit.Framework.Test]
        public virtual void BuilderWithZeroColorsLengthAndReflect() {
            Rectangle targetBoundingBox = new Rectangle(50f, 450f, 300f, 300f);
            AbstractLinearGradientBuilder gradientBuilder = new LinearGradientBuilder().SetGradientVector(targetBoundingBox
                .GetLeft() + 10f, targetBoundingBox.GetBottom() + 100f, targetBoundingBox.GetRight() - 10f, targetBoundingBox
                .GetBottom() + 100f).SetSpreadMethod(GradientSpreadMethod.REFLECT).AddColorStop(new GradientColorStop(
                ColorConstants.RED.GetColorValue(), 0.8d, GradientColorStop.OffsetType.RELATIVE)).AddColorStop(new GradientColorStop
                (ColorConstants.BLUE.GetColorValue(), 0.5d, GradientColorStop.OffsetType.RELATIVE)).AddColorStop(new GradientColorStop
                (ColorConstants.GREEN.GetColorValue(), 0.2d, GradientColorStop.OffsetType.RELATIVE));
            GenerateAndComparePdfs("zeroColorsLengthAndReflect.pdf", targetBoundingBox, null, gradientBuilder);
        }

        [NUnit.Framework.Test]
        public virtual void BuildWithTwoStopsBeforeTheBeginningAndNoneTest() {
            Rectangle targetBoundingBox = new Rectangle(50f, 450f, 300f, 300f);
            AbstractLinearGradientBuilder gradientBuilder = new LinearGradientBuilder().SetGradientVector(targetBoundingBox
                .GetLeft() + 100f, targetBoundingBox.GetBottom() + 100f, targetBoundingBox.GetRight() - 100f, targetBoundingBox
                .GetTop() - 100f).SetSpreadMethod(GradientSpreadMethod.NONE).AddColorStop(new GradientColorStop(ColorConstants
                .RED.GetColorValue(), -10d, GradientColorStop.OffsetType.RELATIVE)).AddColorStop(new GradientColorStop
                (ColorConstants.BLUE.GetColorValue(), -5d, GradientColorStop.OffsetType.RELATIVE));
            NUnit.Framework.Assert.IsNull(gradientBuilder.BuildColor(targetBoundingBox, null, null));
        }

        [NUnit.Framework.Test]
        public virtual void BuildWithTwoStopsAfterEndAndNoneTest() {
            Rectangle targetBoundingBox = new Rectangle(50f, 450f, 300f, 300f);
            AbstractLinearGradientBuilder gradientBuilder = new LinearGradientBuilder().SetGradientVector(targetBoundingBox
                .GetLeft() + 100f, targetBoundingBox.GetBottom() + 100f, targetBoundingBox.GetRight() - 100f, targetBoundingBox
                .GetTop() - 100f).SetSpreadMethod(GradientSpreadMethod.NONE).AddColorStop(new GradientColorStop(ColorConstants
                .RED.GetColorValue(), 5d, GradientColorStop.OffsetType.RELATIVE)).AddColorStop(new GradientColorStop(ColorConstants
                .BLUE.GetColorValue(), 10d, GradientColorStop.OffsetType.RELATIVE));
            NUnit.Framework.Assert.IsNull(gradientBuilder.BuildColor(targetBoundingBox, null, null));
        }

        [NUnit.Framework.Test]
        public virtual void BuildWithTwoEqualOffsetsStopsAndNoneTest() {
            Rectangle targetBoundingBox = new Rectangle(50f, 450f, 300f, 300f);
            AbstractLinearGradientBuilder gradientBuilder = new LinearGradientBuilder().SetGradientVector(targetBoundingBox
                .GetLeft() + 100f, targetBoundingBox.GetBottom() + 100f, targetBoundingBox.GetRight() - 100f, targetBoundingBox
                .GetTop() - 100f).SetSpreadMethod(GradientSpreadMethod.NONE).AddColorStop(new GradientColorStop(ColorConstants
                .RED.GetColorValue(), 0.5d, GradientColorStop.OffsetType.RELATIVE)).AddColorStop(new GradientColorStop
                (ColorConstants.BLUE.GetColorValue(), 0.5d, GradientColorStop.OffsetType.RELATIVE));
            NUnit.Framework.Assert.IsNull(gradientBuilder.BuildColor(targetBoundingBox, null, null));
        }

        [NUnit.Framework.Test]
        public virtual void BuildWithTwoStopsInCenterAndNoneTest() {
            Rectangle targetBoundingBox = new Rectangle(50f, 450f, 300f, 300f);
            AbstractLinearGradientBuilder gradientBuilder = new LinearGradientBuilder().SetGradientVector(targetBoundingBox
                .GetLeft() + 100f, targetBoundingBox.GetBottom() + 100f, targetBoundingBox.GetRight() - 100f, targetBoundingBox
                .GetTop() - 100f).SetSpreadMethod(GradientSpreadMethod.NONE).AddColorStop(new GradientColorStop(ColorConstants
                .RED.GetColorValue(), 0.2d, GradientColorStop.OffsetType.RELATIVE)).AddColorStop(new GradientColorStop
                (ColorConstants.BLUE.GetColorValue(), 0.8d, GradientColorStop.OffsetType.RELATIVE));
            GenerateAndComparePdfs("twoEqualOffsetsStops.pdf", targetBoundingBox, null, gradientBuilder);
        }

        [NUnit.Framework.Test]
        public virtual void BuildWithTwoStopsOutsideAndNoneTest() {
            Rectangle targetBoundingBox = new Rectangle(50f, 450f, 300f, 300f);
            AbstractLinearGradientBuilder gradientBuilder = new LinearGradientBuilder().SetGradientVector(targetBoundingBox
                .GetLeft() + 100f, targetBoundingBox.GetBottom() + 100f, targetBoundingBox.GetRight() - 100f, targetBoundingBox
                .GetTop() - 100f).SetSpreadMethod(GradientSpreadMethod.NONE).AddColorStop(new GradientColorStop(ColorConstants
                .RED.GetColorValue(), -1.5d, GradientColorStop.OffsetType.RELATIVE)).AddColorStop(new GradientColorStop
                (ColorConstants.BLUE.GetColorValue(), 2.5d, GradientColorStop.OffsetType.RELATIVE));
            GenerateAndComparePdfs("twoStopsOutsideAndNone.pdf", targetBoundingBox, null, gradientBuilder);
        }

        private void GenerateAndComparePdfs(String fileName, Rectangle toDraw, AffineTransform transform, AbstractLinearGradientBuilder
             gradientBuilder) {
            String outPdfPath = destinationFolder + fileName;
            using (PdfDocument pdfDoc = new PdfDocument(CompareTool.CreateTestPdfWriter(outPdfPath))) {
                PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage());
                if (transform != null) {
                    canvas.ConcatMatrix(transform);
                }
                canvas.SetFillColor(gradientBuilder.BuildColor(toDraw, transform, pdfDoc)).SetStrokeColor(ColorConstants.BLACK
                    ).Rectangle(toDraw).FillStroke();
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdfPath, sourceFolder + "cmp_" + fileName
                , destinationFolder, "diff"));
        }

        private void GenerateAndComparePdfsWithoutArgumentToBuild(String fileName, Rectangle toDraw, AbstractLinearGradientBuilder
             gradientBuilder) {
            String outPdfPath = destinationFolder + fileName;
            PdfWriter writer = CompareTool.CreateTestPdfWriter(outPdfPath);
            using (PdfDocument pdfDoc = new PdfDocument(writer)) {
                PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage());
                canvas.SetFillColor(gradientBuilder.BuildColor(null, null, pdfDoc)).SetStrokeColor(ColorConstants.BLACK).Rectangle
                    (toDraw).FillStroke();
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPdfPath, sourceFolder + "cmp_" + fileName
                , destinationFolder, "diff"));
        }
    }
}
