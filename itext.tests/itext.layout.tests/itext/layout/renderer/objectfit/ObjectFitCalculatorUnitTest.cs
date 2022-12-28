/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: iText Software.

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License version 3
as published by the Free Software Foundation with the addition of the
following permission added to Section 15 as permitted in Section 7(a):
FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
OF THIRD PARTY RIGHTS

This program is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU Affero General Public License for more details.
You should have received a copy of the GNU Affero General Public License
along with this program; if not, see http://www.gnu.org/licenses or write to
the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
Boston, MA, 02110-1301 USA, or download the license from the following URL:
http://itextpdf.com/terms-of-use/

The interactive user interfaces in modified source and object code versions
of this program must display Appropriate Legal Notices, as required under
Section 5 of the GNU Affero General Public License.

In accordance with Section 7(b) of the GNU Affero General Public License,
a covered work must retain the producer line in every PDF that is created
or manipulated using iText.

You can be released from the requirements of the license by purchasing
a commercial license. Buying such a license is mandatory as soon as you
develop commercial activities involving the iText software without
disclosing the source code of your own applications.
These activities include: offering paid services to customers as an ASP,
serving PDFs on the fly in a web application, shipping iText with a closed
source product.

For more information, please contact iText Software Corp. at this
address: sales@itextpdf.com
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
