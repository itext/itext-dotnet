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
using iText.Layout.Properties;
using iText.Test;

namespace iText.Layout.Renderer.Objectfit {
    [NUnit.Framework.Category("UnitTest")]
    public class ObjectFitCalculatorUnitTest : ExtendedITextTest {
        private const float SMALL_WIDTH = 200;

        private const float BIG_WIDTH = 500;

        private const float SMALL_HEIGHT = 400;

        private const float BIG_HEIGHT = 700;

        [NUnit.Framework.Test]
        public virtual void FillModeContainerIsGreaterThanImageTest() {
            ObjectFitApplyingResult result = ObjectFitCalculator.CalculateRenderedImageSize(ObjectFit.FILL, SMALL_WIDTH
                , SMALL_HEIGHT, BIG_WIDTH, BIG_HEIGHT);
            NUnit.Framework.Assert.AreEqual(BIG_WIDTH, result.GetRenderedImageWidth(), 0.1);
            NUnit.Framework.Assert.AreEqual(BIG_HEIGHT, result.GetRenderedImageHeight(), 0.1);
            NUnit.Framework.Assert.IsFalse(result.IsImageCuttingRequired());
        }

        [NUnit.Framework.Test]
        public virtual void FillModeContainerIsLessThanImageTest() {
            ObjectFitApplyingResult result = ObjectFitCalculator.CalculateRenderedImageSize(ObjectFit.FILL, BIG_WIDTH, 
                BIG_HEIGHT, SMALL_WIDTH, SMALL_HEIGHT);
            NUnit.Framework.Assert.AreEqual(SMALL_WIDTH, result.GetRenderedImageWidth(), 0.1);
            NUnit.Framework.Assert.AreEqual(SMALL_HEIGHT, result.GetRenderedImageHeight(), 0.1);
            NUnit.Framework.Assert.IsFalse(result.IsImageCuttingRequired());
        }

        [NUnit.Framework.Test]
        public virtual void FillModeContainerIsHorizontalAndImageIsVerticalTest() {
            ObjectFitApplyingResult result = ObjectFitCalculator.CalculateRenderedImageSize(ObjectFit.FILL, BIG_WIDTH, 
                SMALL_HEIGHT, SMALL_WIDTH, BIG_HEIGHT);
            NUnit.Framework.Assert.AreEqual(SMALL_WIDTH, result.GetRenderedImageWidth(), 0.1);
            NUnit.Framework.Assert.AreEqual(BIG_HEIGHT, result.GetRenderedImageHeight(), 0.1);
            NUnit.Framework.Assert.IsFalse(result.IsImageCuttingRequired());
        }

        [NUnit.Framework.Test]
        public virtual void FillModeContainerIsVerticalAndImageIsHorizontalTest() {
            ObjectFitApplyingResult result = ObjectFitCalculator.CalculateRenderedImageSize(ObjectFit.FILL, SMALL_WIDTH
                , BIG_HEIGHT, BIG_WIDTH, SMALL_HEIGHT);
            NUnit.Framework.Assert.AreEqual(BIG_WIDTH, result.GetRenderedImageWidth(), 0.1);
            NUnit.Framework.Assert.AreEqual(SMALL_HEIGHT, result.GetRenderedImageHeight(), 0.1);
            NUnit.Framework.Assert.IsFalse(result.IsImageCuttingRequired());
        }

        [NUnit.Framework.Test]
        public virtual void ContainModeContainerIsGreaterThanImageTest() {
            ObjectFitApplyingResult result = ObjectFitCalculator.CalculateRenderedImageSize(ObjectFit.CONTAIN, SMALL_WIDTH
                , SMALL_HEIGHT, BIG_WIDTH, BIG_HEIGHT);
            float expectedWidth = SMALL_WIDTH / SMALL_HEIGHT * BIG_HEIGHT;
            NUnit.Framework.Assert.AreEqual(expectedWidth, result.GetRenderedImageWidth(), 0.1);
            NUnit.Framework.Assert.AreEqual(BIG_HEIGHT, result.GetRenderedImageHeight(), 0.1);
            NUnit.Framework.Assert.IsFalse(result.IsImageCuttingRequired());
        }

        [NUnit.Framework.Test]
        public virtual void ContainModeContainerIsLessThanImageTest() {
            ObjectFitApplyingResult result = ObjectFitCalculator.CalculateRenderedImageSize(ObjectFit.CONTAIN, BIG_WIDTH
                , BIG_HEIGHT, SMALL_WIDTH, SMALL_HEIGHT);
            float expectedHeight = BIG_HEIGHT / BIG_WIDTH * SMALL_WIDTH;
            NUnit.Framework.Assert.AreEqual(SMALL_WIDTH, result.GetRenderedImageWidth(), 0.1);
            NUnit.Framework.Assert.AreEqual(expectedHeight, result.GetRenderedImageHeight(), 0.1);
            NUnit.Framework.Assert.IsFalse(result.IsImageCuttingRequired());
        }

        [NUnit.Framework.Test]
        public virtual void ContainModeContainerIsHorizontalAndImageIsVerticalTest() {
            ObjectFitApplyingResult result = ObjectFitCalculator.CalculateRenderedImageSize(ObjectFit.CONTAIN, BIG_WIDTH
                , SMALL_HEIGHT, SMALL_WIDTH, BIG_HEIGHT);
            float expectedHeight = SMALL_HEIGHT / BIG_WIDTH * SMALL_WIDTH;
            NUnit.Framework.Assert.AreEqual(SMALL_WIDTH, result.GetRenderedImageWidth(), 0.1);
            NUnit.Framework.Assert.AreEqual(expectedHeight, result.GetRenderedImageHeight(), 0.1);
            NUnit.Framework.Assert.IsFalse(result.IsImageCuttingRequired());
        }

        [NUnit.Framework.Test]
        public virtual void ContainModeContainerIsVerticalAndImageIsHorizontalTest() {
            ObjectFitApplyingResult result = ObjectFitCalculator.CalculateRenderedImageSize(ObjectFit.CONTAIN, SMALL_WIDTH
                , BIG_HEIGHT, BIG_WIDTH, SMALL_HEIGHT);
            float expectedWidth = SMALL_WIDTH / BIG_HEIGHT * SMALL_HEIGHT;
            NUnit.Framework.Assert.AreEqual(expectedWidth, result.GetRenderedImageWidth(), 0.1);
            NUnit.Framework.Assert.AreEqual(SMALL_HEIGHT, result.GetRenderedImageHeight(), 0.1);
            NUnit.Framework.Assert.IsFalse(result.IsImageCuttingRequired());
        }

        [NUnit.Framework.Test]
        public virtual void CoverModeContainerIsGreaterThanImageTest() {
            ObjectFitApplyingResult result = ObjectFitCalculator.CalculateRenderedImageSize(ObjectFit.COVER, SMALL_WIDTH
                , SMALL_HEIGHT, BIG_WIDTH, BIG_HEIGHT);
            float expectedHeight = SMALL_HEIGHT / SMALL_WIDTH * BIG_WIDTH;
            NUnit.Framework.Assert.AreEqual(BIG_WIDTH, result.GetRenderedImageWidth(), 0.1);
            NUnit.Framework.Assert.AreEqual(expectedHeight, result.GetRenderedImageHeight(), 0.1);
            NUnit.Framework.Assert.IsTrue(result.IsImageCuttingRequired());
        }

        [NUnit.Framework.Test]
        public virtual void CoverModeContainerIsLessThanImageTest() {
            ObjectFitApplyingResult result = ObjectFitCalculator.CalculateRenderedImageSize(ObjectFit.COVER, BIG_WIDTH
                , BIG_HEIGHT, SMALL_WIDTH, SMALL_HEIGHT);
            float expectedWidth = BIG_WIDTH / BIG_HEIGHT * SMALL_HEIGHT;
            NUnit.Framework.Assert.AreEqual(expectedWidth, result.GetRenderedImageWidth(), 0.1);
            NUnit.Framework.Assert.AreEqual(SMALL_HEIGHT, result.GetRenderedImageHeight(), 0.1);
            NUnit.Framework.Assert.IsTrue(result.IsImageCuttingRequired());
        }

        [NUnit.Framework.Test]
        public virtual void CoverModeContainerIsHorizontalAndImageIsVerticalTest() {
            ObjectFitApplyingResult result = ObjectFitCalculator.CalculateRenderedImageSize(ObjectFit.COVER, BIG_WIDTH
                , SMALL_HEIGHT, SMALL_WIDTH, BIG_HEIGHT);
            float expectedWidth = BIG_WIDTH / SMALL_HEIGHT * BIG_HEIGHT;
            NUnit.Framework.Assert.AreEqual(expectedWidth, result.GetRenderedImageWidth(), 0.1);
            NUnit.Framework.Assert.AreEqual(BIG_HEIGHT, result.GetRenderedImageHeight(), 0.1);
            NUnit.Framework.Assert.IsTrue(result.IsImageCuttingRequired());
        }

        [NUnit.Framework.Test]
        public virtual void CoverModeContainerIsVerticalAndImageIsHorizontalTest() {
            ObjectFitApplyingResult result = ObjectFitCalculator.CalculateRenderedImageSize(ObjectFit.COVER, SMALL_WIDTH
                , BIG_HEIGHT, BIG_WIDTH, SMALL_HEIGHT);
            float expectedHeight = BIG_HEIGHT / SMALL_WIDTH * BIG_WIDTH;
            NUnit.Framework.Assert.AreEqual(BIG_WIDTH, result.GetRenderedImageWidth(), 0.1);
            NUnit.Framework.Assert.AreEqual(expectedHeight, result.GetRenderedImageHeight(), 0.1);
            NUnit.Framework.Assert.IsTrue(result.IsImageCuttingRequired());
        }

        [NUnit.Framework.Test]
        public virtual void ScaleDownModeContainerIsGreaterThanImageTest() {
            ObjectFitApplyingResult result = ObjectFitCalculator.CalculateRenderedImageSize(ObjectFit.SCALE_DOWN, SMALL_WIDTH
                , SMALL_HEIGHT, BIG_WIDTH, BIG_HEIGHT);
            NUnit.Framework.Assert.AreEqual(SMALL_WIDTH, result.GetRenderedImageWidth(), 0.1);
            NUnit.Framework.Assert.AreEqual(SMALL_HEIGHT, result.GetRenderedImageHeight(), 0.1);
            NUnit.Framework.Assert.IsFalse(result.IsImageCuttingRequired());
        }

        [NUnit.Framework.Test]
        public virtual void ScaleDownModeContainerIsLessThanImageTest() {
            ObjectFitApplyingResult result = ObjectFitCalculator.CalculateRenderedImageSize(ObjectFit.SCALE_DOWN, BIG_WIDTH
                , BIG_HEIGHT, SMALL_WIDTH, SMALL_HEIGHT);
            float expectedHeight = BIG_HEIGHT / BIG_WIDTH * SMALL_WIDTH;
            NUnit.Framework.Assert.AreEqual(SMALL_WIDTH, result.GetRenderedImageWidth(), 0.1);
            NUnit.Framework.Assert.AreEqual(expectedHeight, result.GetRenderedImageHeight(), 0.1);
            NUnit.Framework.Assert.IsFalse(result.IsImageCuttingRequired());
        }

        [NUnit.Framework.Test]
        public virtual void ScaleDownModeContainerIsHorizontalAndImageIsVerticalTest() {
            ObjectFitApplyingResult result = ObjectFitCalculator.CalculateRenderedImageSize(ObjectFit.SCALE_DOWN, BIG_WIDTH
                , SMALL_HEIGHT, SMALL_WIDTH, BIG_HEIGHT);
            float expectedHeight = SMALL_HEIGHT / BIG_WIDTH * SMALL_WIDTH;
            NUnit.Framework.Assert.AreEqual(SMALL_WIDTH, result.GetRenderedImageWidth(), 0.1);
            NUnit.Framework.Assert.AreEqual(expectedHeight, result.GetRenderedImageHeight(), 0.1);
            NUnit.Framework.Assert.IsFalse(result.IsImageCuttingRequired());
        }

        [NUnit.Framework.Test]
        public virtual void ScaleDownModeContainerIsVerticalAndImageIsHorizontalTest() {
            ObjectFitApplyingResult result = ObjectFitCalculator.CalculateRenderedImageSize(ObjectFit.SCALE_DOWN, SMALL_WIDTH
                , BIG_HEIGHT, BIG_WIDTH, SMALL_HEIGHT);
            float expectedWidth = SMALL_WIDTH / BIG_HEIGHT * SMALL_HEIGHT;
            NUnit.Framework.Assert.AreEqual(expectedWidth, result.GetRenderedImageWidth(), 0.1);
            NUnit.Framework.Assert.AreEqual(SMALL_HEIGHT, result.GetRenderedImageHeight(), 0.1);
            NUnit.Framework.Assert.IsFalse(result.IsImageCuttingRequired());
        }

        [NUnit.Framework.Test]
        public virtual void NoneModeContainerIsGreaterThanImageTest() {
            ObjectFitApplyingResult result = ObjectFitCalculator.CalculateRenderedImageSize(ObjectFit.NONE, SMALL_WIDTH
                , SMALL_HEIGHT, BIG_WIDTH, BIG_HEIGHT);
            NUnit.Framework.Assert.AreEqual(SMALL_WIDTH, result.GetRenderedImageWidth(), 0.1);
            NUnit.Framework.Assert.AreEqual(SMALL_HEIGHT, result.GetRenderedImageHeight(), 0.1);
            NUnit.Framework.Assert.IsFalse(result.IsImageCuttingRequired());
        }

        [NUnit.Framework.Test]
        public virtual void NoneModeContainerIsLessThanImageTest() {
            ObjectFitApplyingResult result = ObjectFitCalculator.CalculateRenderedImageSize(ObjectFit.NONE, BIG_WIDTH, 
                BIG_HEIGHT, SMALL_WIDTH, SMALL_HEIGHT);
            NUnit.Framework.Assert.AreEqual(BIG_WIDTH, result.GetRenderedImageWidth(), 0.1);
            NUnit.Framework.Assert.AreEqual(BIG_HEIGHT, result.GetRenderedImageHeight(), 0.1);
            NUnit.Framework.Assert.IsTrue(result.IsImageCuttingRequired());
        }

        [NUnit.Framework.Test]
        public virtual void NoneModeContainerIsHorizontalAndImageIsVerticalTest() {
            ObjectFitApplyingResult result = ObjectFitCalculator.CalculateRenderedImageSize(ObjectFit.NONE, BIG_WIDTH, 
                SMALL_HEIGHT, SMALL_WIDTH, BIG_HEIGHT);
            NUnit.Framework.Assert.AreEqual(BIG_WIDTH, result.GetRenderedImageWidth(), 0.1);
            NUnit.Framework.Assert.AreEqual(SMALL_HEIGHT, result.GetRenderedImageHeight(), 0.1);
            NUnit.Framework.Assert.IsTrue(result.IsImageCuttingRequired());
        }

        [NUnit.Framework.Test]
        public virtual void NoneModeContainerIsVerticalAndImageIsHorizontalTest() {
            ObjectFitApplyingResult result = ObjectFitCalculator.CalculateRenderedImageSize(ObjectFit.NONE, SMALL_WIDTH
                , BIG_HEIGHT, BIG_WIDTH, SMALL_HEIGHT);
            NUnit.Framework.Assert.AreEqual(SMALL_WIDTH, result.GetRenderedImageWidth(), 0.1);
            NUnit.Framework.Assert.AreEqual(BIG_HEIGHT, result.GetRenderedImageHeight(), 0.1);
            NUnit.Framework.Assert.IsTrue(result.IsImageCuttingRequired());
        }
    }
}
