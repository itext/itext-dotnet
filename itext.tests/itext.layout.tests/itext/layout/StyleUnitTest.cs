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

namespace iText.Layout {
    [NUnit.Framework.Category("UnitTest")]
    public class StyleUnitTest : ExtendedITextTest {
        public static float EPS = 0.0001f;

        [NUnit.Framework.Test]
        public virtual void SetAndGetMarginsTest() {
            float expectedMarginTop = 92;
            float expectedMarginRight = 90;
            float expectedMarginBottom = 86;
            float expectedMarginLeft = 88;
            Style style = new Style();
            NUnit.Framework.Assert.IsNull(style.GetMarginTop());
            NUnit.Framework.Assert.IsNull(style.GetMarginRight());
            NUnit.Framework.Assert.IsNull(style.GetMarginBottom());
            NUnit.Framework.Assert.IsNull(style.GetMarginLeft());
            style.SetMargins(expectedMarginTop, expectedMarginRight, expectedMarginBottom, expectedMarginLeft);
            NUnit.Framework.Assert.AreEqual(UnitValue.CreatePointValue(expectedMarginTop), style.GetMarginTop());
            NUnit.Framework.Assert.AreEqual(UnitValue.CreatePointValue(expectedMarginRight), style.GetMarginRight());
            NUnit.Framework.Assert.AreEqual(UnitValue.CreatePointValue(expectedMarginBottom), style.GetMarginBottom());
            NUnit.Framework.Assert.AreEqual(UnitValue.CreatePointValue(expectedMarginLeft), style.GetMarginLeft());
        }

        [NUnit.Framework.Test]
        public virtual void SetMarginTest() {
            float expectedMargin = 90;
            Style style = new Style();
            NUnit.Framework.Assert.IsNull(style.GetMarginTop());
            NUnit.Framework.Assert.IsNull(style.GetMarginRight());
            NUnit.Framework.Assert.IsNull(style.GetMarginBottom());
            NUnit.Framework.Assert.IsNull(style.GetMarginLeft());
            style.SetMargin(expectedMargin);
            NUnit.Framework.Assert.AreEqual(UnitValue.CreatePointValue(expectedMargin), style.GetMarginTop());
            NUnit.Framework.Assert.AreEqual(UnitValue.CreatePointValue(expectedMargin), style.GetMarginRight());
            NUnit.Framework.Assert.AreEqual(UnitValue.CreatePointValue(expectedMargin), style.GetMarginBottom());
            NUnit.Framework.Assert.AreEqual(UnitValue.CreatePointValue(expectedMargin), style.GetMarginLeft());
        }

        [NUnit.Framework.Test]
        public virtual void GetMarginLeftTest() {
            float expLeftMargin = 88;
            Style style = new Style();
            NUnit.Framework.Assert.IsNull(style.GetMarginLeft());
            style.SetMarginLeft(expLeftMargin);
            NUnit.Framework.Assert.AreEqual(UnitValue.CreatePointValue(expLeftMargin), style.GetMarginLeft());
        }

        [NUnit.Framework.Test]
        public virtual void GetMarginRightTest() {
            float expRightMargin = 90;
            Style style = new Style();
            NUnit.Framework.Assert.IsNull(style.GetMarginRight());
            style.SetMarginRight(expRightMargin);
            NUnit.Framework.Assert.AreEqual(UnitValue.CreatePointValue(expRightMargin), style.GetMarginRight());
        }

        [NUnit.Framework.Test]
        public virtual void GetMarginTopTest() {
            float expTopMargin = 92;
            Style style = new Style();
            NUnit.Framework.Assert.IsNull(style.GetMarginTop());
            style.SetMarginTop(expTopMargin);
            NUnit.Framework.Assert.AreEqual(UnitValue.CreatePointValue(expTopMargin), style.GetMarginTop());
        }

        [NUnit.Framework.Test]
        public virtual void GetMarginBottomTest() {
            float expBottomMargin = 86;
            Style style = new Style();
            NUnit.Framework.Assert.IsNull(style.GetMarginTop());
            style.SetMarginBottom(expBottomMargin);
            NUnit.Framework.Assert.AreEqual(UnitValue.CreatePointValue(expBottomMargin), style.GetMarginBottom());
        }

        [NUnit.Framework.Test]
        public virtual void GetPaddingLeftTest() {
            float expLeftPadding = 6;
            Style style = new Style();
            NUnit.Framework.Assert.IsNull(style.GetPaddingLeft());
            style.SetPaddingLeft(expLeftPadding);
            NUnit.Framework.Assert.AreEqual(UnitValue.CreatePointValue(expLeftPadding), style.GetPaddingLeft());
        }

        [NUnit.Framework.Test]
        public virtual void GetPaddingRightTest() {
            float expRightPadding = 8;
            Style style = new Style();
            NUnit.Framework.Assert.IsNull(style.GetPaddingRight());
            style.SetPaddingRight(expRightPadding);
            NUnit.Framework.Assert.AreEqual(UnitValue.CreatePointValue(expRightPadding), style.GetPaddingRight());
        }

        [NUnit.Framework.Test]
        public virtual void GetPaddingTopTest() {
            float expTopPadding = 10;
            Style style = new Style();
            NUnit.Framework.Assert.IsNull(style.GetPaddingTop());
            style.SetPaddingTop(expTopPadding);
            NUnit.Framework.Assert.AreEqual(UnitValue.CreatePointValue(expTopPadding), style.GetPaddingTop());
        }

        [NUnit.Framework.Test]
        public virtual void GetPaddingBottomTest() {
            float expBottomPadding = 5;
            Style style = new Style();
            NUnit.Framework.Assert.IsNull(style.GetPaddingBottom());
            style.SetPaddingBottom(expBottomPadding);
            NUnit.Framework.Assert.AreEqual(UnitValue.CreatePointValue(expBottomPadding), style.GetPaddingBottom());
        }

        [NUnit.Framework.Test]
        public virtual void SetPaddingTest() {
            float expPadding = 10;
            Style style = new Style();
            NUnit.Framework.Assert.IsNull(style.GetPaddingTop());
            NUnit.Framework.Assert.IsNull(style.GetPaddingRight());
            NUnit.Framework.Assert.IsNull(style.GetPaddingBottom());
            NUnit.Framework.Assert.IsNull(style.GetPaddingLeft());
            style.SetPadding(expPadding);
            NUnit.Framework.Assert.AreEqual(UnitValue.CreatePointValue(expPadding), style.GetPaddingTop());
            NUnit.Framework.Assert.AreEqual(UnitValue.CreatePointValue(expPadding), style.GetPaddingRight());
            NUnit.Framework.Assert.AreEqual(UnitValue.CreatePointValue(expPadding), style.GetPaddingBottom());
            NUnit.Framework.Assert.AreEqual(UnitValue.CreatePointValue(expPadding), style.GetPaddingLeft());
        }

        [NUnit.Framework.Test]
        public virtual void SetPaddingsTest() {
            float expPaddingTop = 10;
            float expPaddingRight = 8;
            float expPaddingBottom = 5;
            float expPaddingLeft = 6;
            Style style = new Style();
            NUnit.Framework.Assert.IsNull(style.GetPaddingTop());
            NUnit.Framework.Assert.IsNull(style.GetPaddingRight());
            NUnit.Framework.Assert.IsNull(style.GetPaddingBottom());
            NUnit.Framework.Assert.IsNull(style.GetPaddingLeft());
            style.SetPaddings(expPaddingTop, expPaddingRight, expPaddingBottom, expPaddingLeft);
            NUnit.Framework.Assert.AreEqual(UnitValue.CreatePointValue(expPaddingLeft), style.GetPaddingLeft());
            NUnit.Framework.Assert.AreEqual(UnitValue.CreatePointValue(expPaddingBottom), style.GetPaddingBottom());
            NUnit.Framework.Assert.AreEqual(UnitValue.CreatePointValue(expPaddingTop), style.GetPaddingTop());
            NUnit.Framework.Assert.AreEqual(UnitValue.CreatePointValue(expPaddingRight), style.GetPaddingRight());
        }

        [NUnit.Framework.Test]
        public virtual void SetVerticalAlignmentMiddleTest() {
            VerticalAlignment? expectedAlignment = VerticalAlignment.MIDDLE;
            Style style = new Style();
            NUnit.Framework.Assert.IsNull(style.GetProperty<VerticalAlignment?>(Property.VERTICAL_ALIGNMENT));
            style.SetVerticalAlignment(expectedAlignment);
            NUnit.Framework.Assert.AreEqual(expectedAlignment, style.GetProperty<VerticalAlignment?>(Property.VERTICAL_ALIGNMENT
                ));
        }

        [NUnit.Framework.Test]
        public virtual void SetVerticalAlignmentTopTest() {
            VerticalAlignment? expectedAlignment = VerticalAlignment.TOP;
            Style style = new Style();
            NUnit.Framework.Assert.IsNull(style.GetProperty<VerticalAlignment?>(Property.VERTICAL_ALIGNMENT));
            style.SetVerticalAlignment(expectedAlignment);
            NUnit.Framework.Assert.AreEqual(expectedAlignment, style.GetProperty<VerticalAlignment?>(Property.VERTICAL_ALIGNMENT
                ));
        }

        [NUnit.Framework.Test]
        public virtual void SetVerticalAlignmentBottomTest() {
            VerticalAlignment? expectedAlignment = VerticalAlignment.BOTTOM;
            Style style = new Style();
            NUnit.Framework.Assert.IsNull(style.GetProperty<VerticalAlignment?>(Property.VERTICAL_ALIGNMENT));
            style.SetVerticalAlignment(expectedAlignment);
            NUnit.Framework.Assert.AreEqual(expectedAlignment, style.GetProperty<VerticalAlignment?>(Property.VERTICAL_ALIGNMENT
                ));
        }

        [NUnit.Framework.Test]
        public virtual void SetSpacingRatioTest() {
            float expectedSpacingRatio = 0.5f;
            Style style = new Style();
            NUnit.Framework.Assert.IsNull(style.GetProperty<float?>(Property.SPACING_RATIO));
            style.SetSpacingRatio(expectedSpacingRatio);
            NUnit.Framework.Assert.AreEqual(expectedSpacingRatio, (float)style.GetProperty<float?>(Property.SPACING_RATIO
                ), EPS);
        }

        [NUnit.Framework.Test]
        public virtual void SetKeepTogetherTrueTest() {
            Style style = new Style();
            NUnit.Framework.Assert.IsNull(style.GetProperty<bool?>(Property.KEEP_TOGETHER));
            style.SetKeepTogether(true);
            NUnit.Framework.Assert.AreEqual(true, style.GetProperty<bool?>(Property.KEEP_TOGETHER));
        }

        [NUnit.Framework.Test]
        public virtual void SetKeepTogetherFalseTest() {
            Style style = new Style();
            NUnit.Framework.Assert.IsNull(style.GetProperty<bool?>(Property.KEEP_TOGETHER));
            style.SetKeepTogether(false);
            NUnit.Framework.Assert.AreEqual(false, style.GetProperty<bool?>(Property.KEEP_TOGETHER));
        }

        [NUnit.Framework.Test]
        public virtual void IsKeepTogetherTest() {
            Style style = new Style();
            NUnit.Framework.Assert.IsNull(style.GetProperty<bool?>(Property.KEEP_TOGETHER));
            style.SetKeepTogether(true);
            NUnit.Framework.Assert.AreEqual(true, style.IsKeepTogether());
        }

        [NUnit.Framework.Test]
        public virtual void SetRotationAngleFloatTest() {
            float expectedRotationAngle = 20f;
            Style style = new Style();
            NUnit.Framework.Assert.IsNull(style.GetProperty<float?>(Property.ROTATION_ANGLE));
            style.SetRotationAngle(expectedRotationAngle);
            NUnit.Framework.Assert.AreEqual(expectedRotationAngle, (float)style.GetProperty<float?>(Property.ROTATION_ANGLE
                ), EPS);
        }

        [NUnit.Framework.Test]
        public virtual void SetRotationAngleDoubleTest() {
            double expectedRotationAngle = 20.0;
            Style style = new Style();
            NUnit.Framework.Assert.IsNull(style.GetProperty<float?>(Property.ROTATION_ANGLE));
            style.SetRotationAngle(expectedRotationAngle);
            NUnit.Framework.Assert.AreEqual(expectedRotationAngle, (float)style.GetProperty<float?>(Property.ROTATION_ANGLE
                ), EPS);
        }

        [NUnit.Framework.Test]
        public virtual void SetAndGetWidthTest() {
            float expectedWidth = 100;
            Style style = new Style();
            NUnit.Framework.Assert.IsNull(style.GetWidth());
            style.SetWidth(expectedWidth);
            NUnit.Framework.Assert.AreEqual(UnitValue.CreatePointValue(expectedWidth), style.GetWidth());
        }

        [NUnit.Framework.Test]
        public virtual void SetAndGetWidthUnitValueTest() {
            float expectedWidth = 50;
            Style style = new Style();
            NUnit.Framework.Assert.IsNull(style.GetWidth());
            style.SetWidth(UnitValue.CreatePointValue(expectedWidth));
            NUnit.Framework.Assert.AreEqual(UnitValue.CreatePointValue(expectedWidth), style.GetWidth());
        }

        [NUnit.Framework.Test]
        public virtual void SetAndGetHeightTest() {
            float expectedHeight = 100;
            Style style = new Style();
            NUnit.Framework.Assert.IsNull(style.GetHeight());
            style.SetHeight(expectedHeight);
            NUnit.Framework.Assert.AreEqual(UnitValue.CreatePointValue(expectedHeight), style.GetHeight());
        }

        [NUnit.Framework.Test]
        public virtual void SetAndGetHeightUnitValueTest() {
            float expectedHeight = 50;
            Style style = new Style();
            NUnit.Framework.Assert.IsNull(style.GetHeight());
            style.SetHeight(UnitValue.CreatePointValue(expectedHeight));
            NUnit.Framework.Assert.AreEqual(UnitValue.CreatePointValue(expectedHeight), style.GetHeight());
        }

        [NUnit.Framework.Test]
        public virtual void SetMaxHeightTest() {
            float expectedMaxHeight = 80;
            Style style = new Style();
            NUnit.Framework.Assert.IsNull(style.GetProperty<UnitValue>(Property.MAX_HEIGHT));
            style.SetMaxHeight(expectedMaxHeight);
            NUnit.Framework.Assert.AreEqual(UnitValue.CreatePointValue(expectedMaxHeight), style.GetProperty<UnitValue
                >(Property.MAX_HEIGHT));
        }

        [NUnit.Framework.Test]
        public virtual void SetMaxHeightUnitValueTest() {
            float expectedMaxHeight = 50;
            Style style = new Style();
            NUnit.Framework.Assert.IsNull(style.GetProperty<UnitValue>(Property.MAX_HEIGHT));
            style.SetMaxHeight(UnitValue.CreatePointValue(expectedMaxHeight));
            NUnit.Framework.Assert.AreEqual(UnitValue.CreatePointValue(expectedMaxHeight), style.GetProperty<UnitValue
                >(Property.MAX_HEIGHT));
        }

        [NUnit.Framework.Test]
        public virtual void SetMinHeightTest() {
            float expectedMinHeight = 50;
            Style style = new Style();
            NUnit.Framework.Assert.IsNull(style.GetProperty<UnitValue>(Property.MIN_HEIGHT));
            style.SetMinHeight(expectedMinHeight);
            NUnit.Framework.Assert.AreEqual(UnitValue.CreatePointValue(expectedMinHeight), style.GetProperty<UnitValue
                >(Property.MIN_HEIGHT));
        }

        [NUnit.Framework.Test]
        public virtual void SetMinHeightUnitValueTest() {
            float expectedMinHeight = 30;
            Style style = new Style();
            NUnit.Framework.Assert.IsNull(style.GetProperty<UnitValue>(Property.MIN_HEIGHT));
            style.SetMinHeight(UnitValue.CreatePointValue(expectedMinHeight));
            NUnit.Framework.Assert.AreEqual(UnitValue.CreatePointValue(expectedMinHeight), style.GetProperty<UnitValue
                >(Property.MIN_HEIGHT));
        }

        [NUnit.Framework.Test]
        public virtual void SetMaxWidthTest() {
            float expectedMaxWidth = 200;
            Style style = new Style();
            NUnit.Framework.Assert.IsNull(style.GetProperty<UnitValue>(Property.MAX_WIDTH));
            style.SetMaxWidth(expectedMaxWidth);
            NUnit.Framework.Assert.AreEqual(UnitValue.CreatePointValue(expectedMaxWidth), style.GetProperty<UnitValue>
                (Property.MAX_WIDTH));
        }

        [NUnit.Framework.Test]
        public virtual void SetMaxWidthUnitValueTest() {
            float expectedMaxWidth = 150;
            Style style = new Style();
            NUnit.Framework.Assert.IsNull(style.GetProperty<UnitValue>(Property.MAX_WIDTH));
            style.SetMaxWidth(UnitValue.CreatePointValue(expectedMaxWidth));
            NUnit.Framework.Assert.AreEqual(UnitValue.CreatePointValue(expectedMaxWidth), style.GetProperty<UnitValue>
                (Property.MAX_WIDTH));
        }

        [NUnit.Framework.Test]
        public virtual void SetMinWidthTest() {
            float expectedMinWidth = 50;
            Style style = new Style();
            NUnit.Framework.Assert.IsNull(style.GetProperty<UnitValue>(Property.MIN_WIDTH));
            style.SetMinWidth(expectedMinWidth);
            NUnit.Framework.Assert.AreEqual(UnitValue.CreatePointValue(expectedMinWidth), style.GetProperty<UnitValue>
                (Property.MIN_WIDTH));
        }

        [NUnit.Framework.Test]
        public virtual void SetMinWidthUnitValueTest() {
            float expectedMinWidth = 30;
            Style style = new Style();
            NUnit.Framework.Assert.IsNull(style.GetProperty<UnitValue>(Property.MIN_WIDTH));
            style.SetMinWidth(UnitValue.CreatePointValue(expectedMinWidth));
            NUnit.Framework.Assert.AreEqual(UnitValue.CreatePointValue(expectedMinWidth), style.GetProperty<UnitValue>
                (Property.MIN_WIDTH));
        }
    }
}
