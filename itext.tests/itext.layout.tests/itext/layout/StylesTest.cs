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
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Test;

namespace iText.Layout {
    [NUnit.Framework.Category("IntegrationTest")]
    public class StylesTest : ExtendedITextTest {
        public static float EPS = 0.0001f;

        [NUnit.Framework.Test]
        public virtual void CopyConstructorTest() {
            Style myStyle = new Style();
            myStyle.SetFontColor(ColorConstants.RED);
            Style copiedStyle = new Style(myStyle);
            NUnit.Framework.Assert.AreEqual(ColorConstants.RED, copiedStyle.GetProperty<TransparentColor>(Property.FONT_COLOR
                ).GetColor());
        }

        [NUnit.Framework.Test]
        public virtual void AddingStyleBeforeSettingPropertyTest() {
            Style myStyle = new Style();
            myStyle.SetFontColor(ColorConstants.RED);
            Paragraph p = new Paragraph("text").AddStyle(myStyle).SetFontColor(ColorConstants.GREEN);
            NUnit.Framework.Assert.AreEqual(ColorConstants.GREEN, p.GetRenderer().GetProperty<TransparentColor>(Property
                .FONT_COLOR).GetColor());
        }

        [NUnit.Framework.Test]
        public virtual void AddingStyleAfterSettingPropertyTest() {
            Style myStyle = new Style();
            myStyle.SetFontColor(ColorConstants.RED);
            Paragraph p = new Paragraph("text").SetFontColor(ColorConstants.GREEN).AddStyle(myStyle);
            NUnit.Framework.Assert.AreEqual(ColorConstants.GREEN, p.GetRenderer().GetProperty<TransparentColor>(Property
                .FONT_COLOR).GetColor());
        }

        [NUnit.Framework.Test]
        public virtual void AddingStyleTest() {
            Style myStyle = new Style();
            myStyle.SetFontColor(ColorConstants.RED);
            Paragraph p = new Paragraph("text").AddStyle(myStyle);
            NUnit.Framework.Assert.AreEqual(ColorConstants.RED, p.GetRenderer().GetProperty<TransparentColor>(Property
                .FONT_COLOR).GetColor());
        }

        [NUnit.Framework.Test]
        public virtual void AddingSeveralStyleTest() {
            Style myStyle = new Style();
            myStyle.SetFontColor(ColorConstants.RED);
            Paragraph p = new Paragraph("text").AddStyle(myStyle);
            NUnit.Framework.Assert.AreEqual(ColorConstants.RED, p.GetRenderer().GetProperty<TransparentColor>(Property
                .FONT_COLOR).GetColor());
            Style myStyle2 = new Style();
            myStyle2.SetFontColor(ColorConstants.GREEN);
            p.AddStyle(myStyle2);
            NUnit.Framework.Assert.AreEqual(ColorConstants.GREEN, p.GetRenderer().GetProperty<TransparentColor>(Property
                .FONT_COLOR).GetColor());
        }

        [NUnit.Framework.Test]
        public virtual void AddNullAsStyleTest() {
            Paragraph p = new Paragraph("text");
            NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => p.AddStyle(null));
        }

        [NUnit.Framework.Test]
        public virtual void SetMarginsViaStyleTest() {
            float expectedMarginTop = 92;
            float expectedMarginRight = 90;
            float expectedMarginBottom = 86;
            float expectedMarginLeft = 88;
            Style style = new Style();
            style.SetMargins(expectedMarginTop, expectedMarginRight, expectedMarginBottom, expectedMarginLeft);
            Paragraph p = new Paragraph("Hello, iText!");
            p.AddStyle(style);
            NUnit.Framework.Assert.AreEqual(UnitValue.CreatePointValue(expectedMarginTop), p.GetProperty<UnitValue>(Property
                .MARGIN_TOP));
            NUnit.Framework.Assert.AreEqual(UnitValue.CreatePointValue(expectedMarginRight), p.GetProperty<UnitValue>(
                Property.MARGIN_RIGHT));
            NUnit.Framework.Assert.AreEqual(UnitValue.CreatePointValue(expectedMarginBottom), p.GetProperty<UnitValue>
                (Property.MARGIN_BOTTOM));
            NUnit.Framework.Assert.AreEqual(UnitValue.CreatePointValue(expectedMarginLeft), p.GetProperty<UnitValue>(Property
                .MARGIN_LEFT));
        }

        [NUnit.Framework.Test]
        public virtual void SetMarginTopViaStyleTest() {
            float expectedMarginTop = 92;
            Style style = new Style();
            style.SetMarginTop(expectedMarginTop);
            Paragraph p = new Paragraph("Hello, iText!");
            p.AddStyle(style);
            NUnit.Framework.Assert.AreEqual(UnitValue.CreatePointValue(expectedMarginTop), p.GetProperty<UnitValue>(Property
                .MARGIN_TOP));
        }

        [NUnit.Framework.Test]
        public virtual void SetVerticalAlignmentViaStyleTest() {
            VerticalAlignment? expectedAlignment = VerticalAlignment.MIDDLE;
            Style style = new Style();
            style.SetVerticalAlignment(expectedAlignment);
            Paragraph p = new Paragraph();
            p.AddStyle(style);
            NUnit.Framework.Assert.AreEqual(expectedAlignment, p.GetProperty<VerticalAlignment?>(Property.VERTICAL_ALIGNMENT
                ));
        }

        [NUnit.Framework.Test]
        public virtual void SetSpacingRatioViaStyleTest() {
            float expectedSpacingRatio = 0.5f;
            Style style = new Style();
            style.SetSpacingRatio(expectedSpacingRatio);
            Paragraph p = new Paragraph();
            p.AddStyle(style);
            NUnit.Framework.Assert.AreEqual(expectedSpacingRatio, (float)p.GetProperty<float?>(Property.SPACING_RATIO)
                , EPS);
        }

        [NUnit.Framework.Test]
        public virtual void SetKeepTogetherTrueViaStyleTest() {
            Style trueStyle = new Style();
            trueStyle.SetKeepTogether(true);
            Paragraph p1 = new Paragraph();
            p1.AddStyle(trueStyle);
            NUnit.Framework.Assert.AreEqual(true, p1.IsKeepTogether());
        }

        [NUnit.Framework.Test]
        public virtual void SetKeepTogetherFalseViaStyleTest() {
            Style falseStyle = new Style();
            falseStyle.SetKeepTogether(false);
            Paragraph p = new Paragraph();
            p.AddStyle(falseStyle);
            NUnit.Framework.Assert.AreEqual(false, p.IsKeepTogether());
        }

        [NUnit.Framework.Test]
        public virtual void SetRotationAngleViaStyleTest() {
            float expectedRotationAngle = 20f;
            Style style = new Style();
            style.SetRotationAngle(expectedRotationAngle);
            Paragraph p = new Paragraph();
            p.AddStyle(style);
            NUnit.Framework.Assert.AreEqual(expectedRotationAngle, (float)p.GetProperty<float?>(Property.ROTATION_ANGLE
                ), EPS);
        }

        [NUnit.Framework.Test]
        public virtual void SetWidthViaStyleTest() {
            float expectedWidth = 100;
            Style style = new Style();
            style.SetWidth(expectedWidth);
            Paragraph p = new Paragraph();
            p.AddStyle(style);
            NUnit.Framework.Assert.AreEqual(UnitValue.CreatePointValue(expectedWidth), p.GetProperty<UnitValue>(Property
                .WIDTH));
        }

        [NUnit.Framework.Test]
        public virtual void SetWidthInUnitValueViaStyleTest() {
            float expectedWidth = 100;
            Style style = new Style();
            style.SetWidth(UnitValue.CreatePointValue(expectedWidth));
            Paragraph p = new Paragraph();
            p.AddStyle(style);
            NUnit.Framework.Assert.AreEqual(UnitValue.CreatePointValue(expectedWidth), p.GetProperty<UnitValue>(Property
                .WIDTH));
        }

        [NUnit.Framework.Test]
        public virtual void SetHeightViaStyleTest() {
            float expectedHeight = 100;
            Style style = new Style();
            style.SetHeight(expectedHeight);
            Paragraph p = new Paragraph();
            p.AddStyle(style);
            NUnit.Framework.Assert.AreEqual(UnitValue.CreatePointValue(expectedHeight), p.GetProperty<UnitValue>(Property
                .HEIGHT));
        }

        [NUnit.Framework.Test]
        public virtual void SetHeightInUnitValueViaStyleTest() {
            float expectedHeight = 100;
            Style style = new Style();
            style.SetHeight(UnitValue.CreatePointValue(expectedHeight));
            Paragraph p = new Paragraph();
            p.AddStyle(style);
            NUnit.Framework.Assert.AreEqual(UnitValue.CreatePointValue(expectedHeight), p.GetProperty<UnitValue>(Property
                .HEIGHT));
        }

        [NUnit.Framework.Test]
        public virtual void SetMaxHeightViaStyleTest() {
            float expectedMaxHeight = 80;
            Style style = new Style();
            style.SetMaxHeight(UnitValue.CreatePointValue(expectedMaxHeight));
            Paragraph p = new Paragraph();
            p.AddStyle(style);
            NUnit.Framework.Assert.AreEqual(UnitValue.CreatePointValue(expectedMaxHeight), p.GetProperty<UnitValue>(Property
                .MAX_HEIGHT));
        }

        [NUnit.Framework.Test]
        public virtual void SetMinHeightViaStyleTest() {
            float expectedMinHeight = 20;
            Style style = new Style();
            style.SetMinHeight(expectedMinHeight);
            Paragraph p = new Paragraph();
            p.AddStyle(style);
            NUnit.Framework.Assert.AreEqual(UnitValue.CreatePointValue(expectedMinHeight), p.GetProperty<UnitValue>(Property
                .MIN_HEIGHT));
        }

        [NUnit.Framework.Test]
        public virtual void SetMaxWidthViaStyleTest() {
            float expectedMaxWidth = 200;
            Style style = new Style();
            style.SetMaxWidth(expectedMaxWidth);
            Paragraph p = new Paragraph();
            p.AddStyle(style);
            NUnit.Framework.Assert.AreEqual(UnitValue.CreatePointValue(expectedMaxWidth), p.GetProperty<UnitValue>(Property
                .MAX_WIDTH));
        }

        [NUnit.Framework.Test]
        public virtual void SetMinWidthViaStyleTest() {
            float expectedMinWidth = 20;
            Style style = new Style();
            style.SetMinWidth(expectedMinWidth);
            Paragraph p = new Paragraph();
            p.AddStyle(style);
            NUnit.Framework.Assert.AreEqual(UnitValue.CreatePointValue(expectedMinWidth), p.GetProperty<UnitValue>(Property
                .MIN_WIDTH));
        }
    }
}
