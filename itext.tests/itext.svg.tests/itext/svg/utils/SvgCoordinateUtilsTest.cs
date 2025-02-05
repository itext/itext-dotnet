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
using iText.Kernel.Geom;
using iText.Svg;
using iText.Svg.Exceptions;
using iText.Test;

namespace iText.Svg.Utils {
    [NUnit.Framework.Category("UnitTest")]
    public class SvgCoordinateUtilsTest : ExtendedITextTest {
        private const double DELTA = 0.0000001;

        private static readonly Rectangle VIEW_BOX = new Rectangle(20F, 20F, 20F, 20F);

        private static readonly Rectangle VIEW_PORT_HORIZONTAL = new Rectangle(60F, 40F, 100F, 60F);

        private static readonly Rectangle VIEW_PORT_VERTICAL = new Rectangle(60F, 40F, 60F, 100F);

        [NUnit.Framework.Test]
        public virtual void CalculateAngleBetweenTwoVectors45degTest() {
            Vector vectorA = new Vector(1, 0, 0);
            Vector vectorB = new Vector(1, 1, 0);
            double expected = Math.PI / 4;
            double actual = SvgCoordinateUtils.CalculateAngleBetweenTwoVectors(vectorA, vectorB);
            NUnit.Framework.Assert.AreEqual(expected, actual, DELTA);
        }

        [NUnit.Framework.Test]
        public virtual void CalculateAngleBetweenTwoVectors45degInverseTest() {
            Vector vectorA = new Vector(1, 0, 0);
            Vector vectorB = new Vector(1, -1, 0);
            double expected = Math.PI / 4;
            double actual = SvgCoordinateUtils.CalculateAngleBetweenTwoVectors(vectorA, vectorB);
            NUnit.Framework.Assert.AreEqual(expected, actual, DELTA);
        }

        [NUnit.Framework.Test]
        public virtual void CalculateAngleBetweenTwoVectors135degTest() {
            Vector vectorA = new Vector(1, 0, 0);
            Vector vectorB = new Vector(-1, 1, 0);
            double expected = (Math.PI - Math.PI / 4);
            double actual = SvgCoordinateUtils.CalculateAngleBetweenTwoVectors(vectorA, vectorB);
            NUnit.Framework.Assert.AreEqual(expected, actual, DELTA);
        }

        [NUnit.Framework.Test]
        public virtual void CalculateAngleBetweenTwoVectors135degInverseTest() {
            Vector vectorA = new Vector(1, 0, 0);
            Vector vectorB = new Vector(-1, -1, 0);
            double expected = (Math.PI - Math.PI / 4);
            double actual = SvgCoordinateUtils.CalculateAngleBetweenTwoVectors(vectorA, vectorB);
            NUnit.Framework.Assert.AreEqual(expected, actual, DELTA);
        }

        [NUnit.Framework.Test]
        public virtual void CalculateAngleBetweenTwoVectors90degTest() {
            Vector vectorA = new Vector(1, 0, 0);
            Vector vectorB = new Vector(0, 1, 0);
            double expected = Math.PI / 2;
            double actual = SvgCoordinateUtils.CalculateAngleBetweenTwoVectors(vectorA, vectorB);
            NUnit.Framework.Assert.AreEqual(expected, actual, DELTA);
        }

        [NUnit.Framework.Test]
        public virtual void CalculateAngleBetweenTwoVectors180degTest() {
            Vector vectorA = new Vector(1, 0, 0);
            Vector vectorB = new Vector(-1, 0, 0);
            double expected = Math.PI;
            double actual = SvgCoordinateUtils.CalculateAngleBetweenTwoVectors(vectorA, vectorB);
            NUnit.Framework.Assert.AreEqual(expected, actual, DELTA);
        }

        [NUnit.Framework.Test]
        public virtual void GetCoordinateForUserSpaceOnUseDefaultTest() {
            double defaultValue = 244.0;
            double result = SvgCoordinateUtils.GetCoordinateForUserSpaceOnUse("random", defaultValue, 0, 0, 0, 0);
            NUnit.Framework.Assert.AreEqual(defaultValue, result, DELTA);
        }

        [NUnit.Framework.Test]
        public virtual void GetCoordinateForUserSpaceOnUsePercentTest() {
            double result = SvgCoordinateUtils.GetCoordinateForUserSpaceOnUse("20%", 0, 10, 20, 0, 0);
            NUnit.Framework.Assert.AreEqual(14.0, result, DELTA);
        }

        [NUnit.Framework.Test]
        public virtual void GetCoordinateForUserSpaceOnUsePxTest() {
            double result = SvgCoordinateUtils.GetCoordinateForUserSpaceOnUse("20px", 0, 0, 0, 0, 0);
            NUnit.Framework.Assert.AreEqual(15.0, result, DELTA);
        }

        [NUnit.Framework.Test]
        public virtual void GetCoordinateForUserSpaceOnUseEmTest() {
            double result = SvgCoordinateUtils.GetCoordinateForUserSpaceOnUse("14em", 0, 0, 0, 10, 18);
            NUnit.Framework.Assert.AreEqual(140.0, result, DELTA);
        }

        [NUnit.Framework.Test]
        public virtual void GetCoordinateForUserSpaceOnUseRemTest() {
            double result = SvgCoordinateUtils.GetCoordinateForUserSpaceOnUse("14rem", 0, 0, 0, 10, 18);
            NUnit.Framework.Assert.AreEqual(252.0, result, DELTA);
        }

        [NUnit.Framework.Test]
        public virtual void GetCoordinateForObjectBoundingBoxPercentTest() {
            double result = SvgCoordinateUtils.GetCoordinateForObjectBoundingBox("20%", 0);
            NUnit.Framework.Assert.AreEqual(0.2, result, DELTA);
        }

        [NUnit.Framework.Test]
        public virtual void GetCoordinateForObjectBoundingBoxNumericFloatingValueTest() {
            double result = SvgCoordinateUtils.GetCoordinateForObjectBoundingBox("1234.3", 0);
            NUnit.Framework.Assert.AreEqual(1234.3, result, DELTA);
        }

        [NUnit.Framework.Test]
        public virtual void GetCoordinateForObjectBoundingBoxNumericIntegerValueTest() {
            double result = SvgCoordinateUtils.GetCoordinateForObjectBoundingBox("1234", 0);
            NUnit.Framework.Assert.AreEqual(1234.0, result, DELTA);
        }

        [NUnit.Framework.Test]
        public virtual void GetCoordinateForObjectBoundingBoxMetricFloatingValueTest() {
            double result = SvgCoordinateUtils.GetCoordinateForObjectBoundingBox("12.3px", 0);
            NUnit.Framework.Assert.AreEqual(12.3, result, DELTA);
        }

        [NUnit.Framework.Test]
        public virtual void GetCoordinateForObjectBoundingBoxMetricIntegerValueTest() {
            double result = SvgCoordinateUtils.GetCoordinateForObjectBoundingBox("12px", 0);
            NUnit.Framework.Assert.AreEqual(12.0, result, DELTA);
        }

        [NUnit.Framework.Test]
        public virtual void GetCoordinateForObjectBoundingBoxRelativeValueTest() {
            double result = SvgCoordinateUtils.GetCoordinateForObjectBoundingBox("12.3em", 0);
            NUnit.Framework.Assert.AreEqual(12.3, result, DELTA);
        }

        [NUnit.Framework.Test]
        public virtual void GetCoordinateForObjectBoundingBoxDefaultTest() {
            double defaultValue = 20.0;
            double result = SvgCoordinateUtils.GetCoordinateForObjectBoundingBox("random", defaultValue);
            NUnit.Framework.Assert.AreEqual(defaultValue, result, DELTA);
        }

        [NUnit.Framework.Test]
        public virtual void ApplyViewBoxViewBoxIsNullTest() {
            Exception e = NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => SvgCoordinateUtils.ApplyViewBox
                (null, new Rectangle(10F, 10F), null, null));
            NUnit.Framework.Assert.AreEqual(SvgExceptionMessageConstant.VIEWBOX_IS_INCORRECT, e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void ApplyViewBoxViewBoxWidthIsZeroTest() {
            Exception e = NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => SvgCoordinateUtils.ApplyViewBox
                (new Rectangle(0F, 10F), new Rectangle(10F, 10F), null, null));
            NUnit.Framework.Assert.AreEqual(SvgExceptionMessageConstant.VIEWBOX_IS_INCORRECT, e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void ApplyViewBoxViewBoxHeightIsZeroTest() {
            Exception e = NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => SvgCoordinateUtils.ApplyViewBox
                (new Rectangle(10F, 0F), new Rectangle(10F, 10F), null, null));
            NUnit.Framework.Assert.AreEqual(SvgExceptionMessageConstant.VIEWBOX_IS_INCORRECT, e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void ApplyViewBoxViewBoxWidthIsNegativeTest() {
            Exception e = NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => SvgCoordinateUtils.ApplyViewBox
                (new Rectangle(-10F, 10F), new Rectangle(10F, 10F), null, null));
            NUnit.Framework.Assert.AreEqual(SvgExceptionMessageConstant.VIEWBOX_IS_INCORRECT, e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void ApplyViewBoxViewBoxHeightIsNegativeTest() {
            Exception e = NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => SvgCoordinateUtils.ApplyViewBox
                (new Rectangle(10F, -10F), new Rectangle(10F, 10F), null, null));
            NUnit.Framework.Assert.AreEqual(SvgExceptionMessageConstant.VIEWBOX_IS_INCORRECT, e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void ApplyViewBoxCurrentViewPortIsNullTest() {
            Exception e = NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => SvgCoordinateUtils.ApplyViewBox
                (new Rectangle(10F, 10F), null, null, null));
            NUnit.Framework.Assert.AreEqual(SvgExceptionMessageConstant.CURRENT_VIEWPORT_IS_NULL, e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void ApplyViewBoxAllNullTest() {
            Exception e = NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => SvgCoordinateUtils.ApplyViewBox
                (null, null, null, null));
            NUnit.Framework.Assert.AreEqual(SvgExceptionMessageConstant.CURRENT_VIEWPORT_IS_NULL, e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void ApplyViewBoxCurrentViewPortZeroWidthHeightTest() {
            Rectangle currentViewPort = new Rectangle(0F, 0F, 0F, 0F);
            Rectangle appliedViewBox = SvgCoordinateUtils.ApplyViewBox(VIEW_BOX, currentViewPort, null, null);
            NUnit.Framework.Assert.IsTrue(currentViewPort.EqualsWithEpsilon(appliedViewBox));
        }

        [NUnit.Framework.Test]
        public virtual void ApplyViewBoxCurrentViewPortNegativeWidthHeightTest() {
            Rectangle currentViewPort = new Rectangle(50F, 50F, -100F, -60F);
            Rectangle appliedViewBox = SvgCoordinateUtils.ApplyViewBox(VIEW_BOX, currentViewPort, null, null);
            NUnit.Framework.Assert.IsTrue(new Rectangle(-850F, -950F, -100F, -100F).EqualsWithEpsilon(appliedViewBox));
        }

        [NUnit.Framework.Test]
        public virtual void ApplyViewBoxAlignIsNullSliceTest() {
            Rectangle assertRect = new Rectangle(120F, 0F, 60F, 60F);
            Rectangle appliedViewBox = SvgCoordinateUtils.ApplyViewBox(VIEW_BOX, VIEW_PORT_HORIZONTAL, null, SvgConstants.Values
                .SLICE);
            NUnit.Framework.Assert.IsTrue(assertRect.EqualsWithEpsilon(appliedViewBox));
        }

        [NUnit.Framework.Test]
        public virtual void ApplyViewBoxAlignIsNullMeetTest() {
            Rectangle assertRect = new Rectangle(120F, 0F, 60F, 60F);
            Rectangle appliedViewBox = SvgCoordinateUtils.ApplyViewBox(VIEW_BOX, VIEW_PORT_HORIZONTAL, null, SvgConstants.Values
                .MEET);
            NUnit.Framework.Assert.IsTrue(assertRect.EqualsWithEpsilon(appliedViewBox));
        }

        [NUnit.Framework.Test]
        public virtual void ApplyViewBoxAlignIsNullIncorrectMeetOrSliceTest() {
            Rectangle assertRect = new Rectangle(120F, 0F, 60F, 60F);
            Rectangle appliedViewBox = SvgCoordinateUtils.ApplyViewBox(VIEW_BOX, VIEW_PORT_HORIZONTAL, null, "jklsdj");
            NUnit.Framework.Assert.IsTrue(assertRect.EqualsWithEpsilon(appliedViewBox));
        }

        [NUnit.Framework.Test]
        public virtual void ApplyViewBoxMeetOrSliceIsNullXMaxYMaxTest() {
            Rectangle assertRect = new Rectangle(180F, 0F, 60F, 60F);
            Rectangle appliedViewBox = SvgCoordinateUtils.ApplyViewBox(VIEW_BOX, VIEW_PORT_HORIZONTAL, SvgConstants.Values
                .XMAX_YMAX, null);
            NUnit.Framework.Assert.IsTrue(assertRect.EqualsWithEpsilon(appliedViewBox));
        }

        [NUnit.Framework.Test]
        public virtual void ApplyViewBoxMeetOrSliceIsNullXMinYMinTest() {
            Rectangle assertRect = new Rectangle(60F, 0F, 60F, 60F);
            Rectangle appliedViewBox = SvgCoordinateUtils.ApplyViewBox(VIEW_BOX, VIEW_PORT_HORIZONTAL, SvgConstants.Values
                .XMIN_YMIN, null);
            NUnit.Framework.Assert.IsTrue(assertRect.EqualsWithEpsilon(appliedViewBox));
        }

        [NUnit.Framework.Test]
        public virtual void ApplyViewBoxMeetOrSliceIsNullIncorrectAlignTest() {
            Rectangle assertRect = new Rectangle(120F, 0F, 60F, 60F);
            Rectangle appliedViewBox = SvgCoordinateUtils.ApplyViewBox(VIEW_BOX, VIEW_PORT_HORIZONTAL, "ahfdfs", null);
            NUnit.Framework.Assert.IsTrue(assertRect.EqualsWithEpsilon(appliedViewBox));
        }

        [NUnit.Framework.Test]
        public virtual void ApplyViewBoxIncorrectAlignMeetTest() {
            Rectangle assertRect = new Rectangle(120F, 0F, 60F, 60F);
            Rectangle appliedViewBox = SvgCoordinateUtils.ApplyViewBox(VIEW_BOX, VIEW_PORT_HORIZONTAL, "ahfdfs", SvgConstants.Values
                .MEET);
            NUnit.Framework.Assert.IsTrue(assertRect.EqualsWithEpsilon(appliedViewBox));
        }

        [NUnit.Framework.Test]
        public virtual void ApplyViewBoxIncorrectAlignSliceTest() {
            Rectangle assertRect = new Rectangle(120F, 0F, 60F, 60F);
            Rectangle appliedViewBox = SvgCoordinateUtils.ApplyViewBox(VIEW_BOX, VIEW_PORT_HORIZONTAL, "ahfdfs", SvgConstants.Values
                .SLICE);
            NUnit.Framework.Assert.IsTrue(assertRect.EqualsWithEpsilon(appliedViewBox));
        }

        [NUnit.Framework.Test]
        public virtual void ApplyViewBoxNoneNullTest() {
            Rectangle appliedViewBox = SvgCoordinateUtils.ApplyViewBox(VIEW_BOX, VIEW_PORT_HORIZONTAL, SvgConstants.Values
                .NONE, null);
            NUnit.Framework.Assert.AreNotSame(VIEW_PORT_HORIZONTAL, appliedViewBox);
            NUnit.Framework.Assert.IsTrue(new Rectangle(-100F, 0F, 100F, 60F).EqualsWithEpsilon(appliedViewBox));
        }

        [NUnit.Framework.Test]
        public virtual void ApplyViewBoxNoneMeetTest() {
            Rectangle appliedViewBox = SvgCoordinateUtils.ApplyViewBox(VIEW_BOX, VIEW_PORT_HORIZONTAL, SvgConstants.Values
                .NONE, SvgConstants.Values.MEET);
            NUnit.Framework.Assert.AreNotSame(VIEW_PORT_HORIZONTAL, appliedViewBox);
            NUnit.Framework.Assert.IsTrue(new Rectangle(-100F, 0F, 100F, 60F).EqualsWithEpsilon(appliedViewBox));
        }

        [NUnit.Framework.Test]
        public virtual void ApplyViewBoxNoneSliceTest() {
            Rectangle appliedViewBox = SvgCoordinateUtils.ApplyViewBox(VIEW_BOX, VIEW_PORT_HORIZONTAL, SvgConstants.Values
                .NONE, SvgConstants.Values.SLICE);
            NUnit.Framework.Assert.AreNotSame(VIEW_PORT_HORIZONTAL, appliedViewBox);
            NUnit.Framework.Assert.IsTrue(new Rectangle(-100F, 0F, 100F, 60F).EqualsWithEpsilon(appliedViewBox));
        }

        [NUnit.Framework.Test]
        public virtual void ApplyViewBoxNoneMeetOrSliceIsIncorrectTest() {
            //xMidYMid will be processed  cause meetOrSlice is incorrect
            Rectangle assertRect = new Rectangle(120F, 0F, 60F, 60F);
            Rectangle appliedViewBox = SvgCoordinateUtils.ApplyViewBox(VIEW_BOX, VIEW_PORT_HORIZONTAL, SvgConstants.Values
                .NONE, "fhakljs");
            NUnit.Framework.Assert.IsTrue(assertRect.EqualsWithEpsilon(appliedViewBox));
        }

        [NUnit.Framework.Test]
        public virtual void ApplyViewBoxXMinYMinMeetHorizontalViewPortTest() {
            Rectangle appliedViewBox = SvgCoordinateUtils.ApplyViewBox(VIEW_BOX, VIEW_PORT_HORIZONTAL, SvgConstants.Values
                .XMIN_YMIN, SvgConstants.Values.MEET);
            NUnit.Framework.Assert.IsTrue(new Rectangle(60F, 0F, 60F, 60F).EqualsWithEpsilon(appliedViewBox));
        }

        [NUnit.Framework.Test]
        public virtual void ApplyViewBoxXMinYMinSliceHorizontalViewPortTest() {
            Rectangle appliedViewBox = SvgCoordinateUtils.ApplyViewBox(VIEW_BOX, VIEW_PORT_HORIZONTAL, SvgConstants.Values
                .XMIN_YMIN, SvgConstants.Values.SLICE);
            NUnit.Framework.Assert.IsTrue(new Rectangle(-100F, -200F, 100F, 100F).EqualsWithEpsilon(appliedViewBox));
        }

        [NUnit.Framework.Test]
        public virtual void ApplyViewBoxXMinYMinMeetVerticalViewPortTest() {
            Rectangle appliedViewBox = SvgCoordinateUtils.ApplyViewBox(VIEW_BOX, VIEW_PORT_VERTICAL, SvgConstants.Values
                .XMIN_YMIN, SvgConstants.Values.MEET);
            NUnit.Framework.Assert.IsTrue(new Rectangle(60F, 0F, 60F, 60F).EqualsWithEpsilon(appliedViewBox));
        }

        [NUnit.Framework.Test]
        public virtual void ApplyViewBoxXMinYMinSliceVerticalViewPortTest() {
            Rectangle appliedViewBox = SvgCoordinateUtils.ApplyViewBox(VIEW_BOX, VIEW_PORT_VERTICAL, SvgConstants.Values
                .XMIN_YMIN, SvgConstants.Values.SLICE);
            NUnit.Framework.Assert.IsTrue(new Rectangle(-100F, -200F, 100F, 100F).EqualsWithEpsilon(appliedViewBox));
        }

        [NUnit.Framework.Test]
        public virtual void ApplyViewBoxXMinYMidMeetHorizontalTest() {
            Rectangle appliedViewBox = SvgCoordinateUtils.ApplyViewBox(VIEW_BOX, VIEW_PORT_HORIZONTAL, SvgConstants.Values
                .XMIN_YMID, SvgConstants.Values.MEET);
            NUnit.Framework.Assert.IsTrue(new Rectangle(60F, 0F, 60F, 60F).EqualsWithEpsilon(appliedViewBox));
        }

        [NUnit.Framework.Test]
        public virtual void ApplyViewBoxXMinYMidSliceHorizontalTest() {
            Rectangle appliedViewBox = SvgCoordinateUtils.ApplyViewBox(VIEW_BOX, VIEW_PORT_HORIZONTAL, SvgConstants.Values
                .XMIN_YMID, SvgConstants.Values.SLICE);
            NUnit.Framework.Assert.IsTrue(new Rectangle(-100F, -300F, 100F, 100F).EqualsWithEpsilon(appliedViewBox));
        }

        [NUnit.Framework.Test]
        public virtual void ApplyViewBoxXMinYMidMeetVerticalTest() {
            Rectangle appliedViewBox = SvgCoordinateUtils.ApplyViewBox(VIEW_BOX, VIEW_PORT_VERTICAL, SvgConstants.Values
                .XMIN_YMID, SvgConstants.Values.MEET);
            NUnit.Framework.Assert.IsTrue(new Rectangle(60F, 60F, 60F, 60F).EqualsWithEpsilon(appliedViewBox));
        }

        [NUnit.Framework.Test]
        public virtual void ApplyViewBoxXMinYMidSliceVerticalTest() {
            Rectangle appliedViewBox = SvgCoordinateUtils.ApplyViewBox(VIEW_BOX, VIEW_PORT_VERTICAL, SvgConstants.Values
                .XMIN_YMID, SvgConstants.Values.SLICE);
            NUnit.Framework.Assert.IsTrue(new Rectangle(-100F, -200F, 100F, 100F).EqualsWithEpsilon(appliedViewBox));
        }

        [NUnit.Framework.Test]
        public virtual void ApplyViewBoxXMinYMaxMeetHorizontalTest() {
            Rectangle appliedViewBox = SvgCoordinateUtils.ApplyViewBox(VIEW_BOX, VIEW_PORT_HORIZONTAL, SvgConstants.Values
                .XMIN_YMAX, SvgConstants.Values.MEET);
            NUnit.Framework.Assert.IsTrue(new Rectangle(60F, 0F, 60F, 60F).EqualsWithEpsilon(appliedViewBox));
        }

        [NUnit.Framework.Test]
        public virtual void ApplyViewBoxXMinYMaxSliceHorizontalTest() {
            Rectangle appliedViewBox = SvgCoordinateUtils.ApplyViewBox(VIEW_BOX, VIEW_PORT_HORIZONTAL, SvgConstants.Values
                .XMIN_YMAX, SvgConstants.Values.SLICE);
            NUnit.Framework.Assert.IsTrue(new Rectangle(-100F, -400F, 100F, 100F).EqualsWithEpsilon(appliedViewBox));
        }

        [NUnit.Framework.Test]
        public virtual void ApplyViewBoxXMinYMaxMeetVerticalTest() {
            Rectangle appliedViewBox = SvgCoordinateUtils.ApplyViewBox(VIEW_BOX, VIEW_PORT_VERTICAL, SvgConstants.Values
                .XMIN_YMAX, SvgConstants.Values.MEET);
            NUnit.Framework.Assert.IsTrue(new Rectangle(60F, 120F, 60F, 60F).EqualsWithEpsilon(appliedViewBox));
        }

        [NUnit.Framework.Test]
        public virtual void ApplyViewBoxXMinYMaxSliceVerticalTest() {
            Rectangle appliedViewBox = SvgCoordinateUtils.ApplyViewBox(VIEW_BOX, VIEW_PORT_VERTICAL, SvgConstants.Values
                .XMIN_YMAX, SvgConstants.Values.SLICE);
            NUnit.Framework.Assert.IsTrue(new Rectangle(-100F, -200F, 100F, 100F).EqualsWithEpsilon(appliedViewBox));
        }

        [NUnit.Framework.Test]
        public virtual void ApplyViewBoxXMidYMinMeetHorizontalTest() {
            Rectangle appliedViewBox = SvgCoordinateUtils.ApplyViewBox(VIEW_BOX, VIEW_PORT_HORIZONTAL, SvgConstants.Values
                .XMID_YMIN, SvgConstants.Values.MEET);
            NUnit.Framework.Assert.IsTrue(new Rectangle(120F, 0F, 60F, 60F).EqualsWithEpsilon(appliedViewBox));
        }

        [NUnit.Framework.Test]
        public virtual void ApplyViewBoxXMidYMinSliceHorizontalTest() {
            Rectangle appliedViewBox = SvgCoordinateUtils.ApplyViewBox(VIEW_BOX, VIEW_PORT_HORIZONTAL, SvgConstants.Values
                .XMID_YMIN, SvgConstants.Values.SLICE);
            NUnit.Framework.Assert.IsTrue(new Rectangle(-100F, -200F, 100F, 100F).EqualsWithEpsilon(appliedViewBox));
        }

        [NUnit.Framework.Test]
        public virtual void ApplyViewBoxXMidYMinMeetVerticalTest() {
            Rectangle appliedViewBox = SvgCoordinateUtils.ApplyViewBox(VIEW_BOX, VIEW_PORT_VERTICAL, SvgConstants.Values
                .XMID_YMIN, SvgConstants.Values.MEET);
            NUnit.Framework.Assert.IsTrue(new Rectangle(60F, 0F, 60F, 60F).EqualsWithEpsilon(appliedViewBox));
        }

        [NUnit.Framework.Test]
        public virtual void ApplyViewBoxXMidYMinSliceVerticalTest() {
            Rectangle appliedViewBox = SvgCoordinateUtils.ApplyViewBox(VIEW_BOX, VIEW_PORT_VERTICAL, SvgConstants.Values
                .XMID_YMIN, SvgConstants.Values.SLICE);
            NUnit.Framework.Assert.IsTrue(new Rectangle(-200F, -200F, 100F, 100F).EqualsWithEpsilon(appliedViewBox));
        }

        [NUnit.Framework.Test]
        public virtual void ApplyViewBoxXMidYMidMeetHorizontalTest() {
            Rectangle appliedViewBox = SvgCoordinateUtils.ApplyViewBox(VIEW_BOX, VIEW_PORT_HORIZONTAL, SvgConstants.Values
                .XMID_YMID, SvgConstants.Values.MEET);
            NUnit.Framework.Assert.IsTrue(new Rectangle(120F, 0F, 60F, 60F).EqualsWithEpsilon(appliedViewBox));
        }

        [NUnit.Framework.Test]
        public virtual void ApplyViewBoxXMidYMidSliceHorizontalTest() {
            Rectangle appliedViewBox = SvgCoordinateUtils.ApplyViewBox(VIEW_BOX, VIEW_PORT_HORIZONTAL, SvgConstants.Values
                .XMID_YMID, SvgConstants.Values.SLICE);
            NUnit.Framework.Assert.IsTrue(new Rectangle(-100F, -300F, 100F, 100F).EqualsWithEpsilon(appliedViewBox));
        }

        [NUnit.Framework.Test]
        public virtual void ApplyViewBoxXMidYMidMeetVerticalTest() {
            Rectangle appliedViewBox = SvgCoordinateUtils.ApplyViewBox(VIEW_BOX, VIEW_PORT_VERTICAL, SvgConstants.Values
                .XMID_YMID, SvgConstants.Values.MEET);
            NUnit.Framework.Assert.IsTrue(new Rectangle(60F, 60F, 60F, 60F).EqualsWithEpsilon(appliedViewBox));
        }

        [NUnit.Framework.Test]
        public virtual void ApplyViewBoxXMidYMidSliceVerticalTest() {
            Rectangle appliedViewBox = SvgCoordinateUtils.ApplyViewBox(VIEW_BOX, VIEW_PORT_VERTICAL, SvgConstants.Values
                .XMID_YMID, SvgConstants.Values.SLICE);
            NUnit.Framework.Assert.IsTrue(new Rectangle(-200F, -200F, 100F, 100F).EqualsWithEpsilon(appliedViewBox));
        }

        [NUnit.Framework.Test]
        public virtual void ApplyViewBoxXMidYMaxMeetHorizontalTest() {
            Rectangle appliedViewBox = SvgCoordinateUtils.ApplyViewBox(VIEW_BOX, VIEW_PORT_HORIZONTAL, SvgConstants.Values
                .XMID_YMAX, SvgConstants.Values.MEET);
            NUnit.Framework.Assert.IsTrue(new Rectangle(120F, 0F, 60F, 60F).EqualsWithEpsilon(appliedViewBox));
        }

        [NUnit.Framework.Test]
        public virtual void ApplyViewBoxXMidYMaxSliceHorizontalTest() {
            Rectangle appliedViewBox = SvgCoordinateUtils.ApplyViewBox(VIEW_BOX, VIEW_PORT_HORIZONTAL, SvgConstants.Values
                .XMID_YMAX, SvgConstants.Values.SLICE);
            NUnit.Framework.Assert.IsTrue(new Rectangle(-100F, -400F, 100F, 100F).EqualsWithEpsilon(appliedViewBox));
        }

        [NUnit.Framework.Test]
        public virtual void ApplyViewBoxXMidYMaxMeetVerticalTest() {
            Rectangle appliedViewBox = SvgCoordinateUtils.ApplyViewBox(VIEW_BOX, VIEW_PORT_VERTICAL, SvgConstants.Values
                .XMID_YMAX, SvgConstants.Values.MEET);
            NUnit.Framework.Assert.IsTrue(new Rectangle(60F, 120F, 60F, 60F).EqualsWithEpsilon(appliedViewBox));
        }

        [NUnit.Framework.Test]
        public virtual void ApplyViewBoxXMidYMaxSliceVerticalTest() {
            Rectangle appliedViewBox = SvgCoordinateUtils.ApplyViewBox(VIEW_BOX, VIEW_PORT_VERTICAL, SvgConstants.Values
                .XMID_YMAX, SvgConstants.Values.SLICE);
            NUnit.Framework.Assert.IsTrue(new Rectangle(-200F, -200F, 100F, 100F).EqualsWithEpsilon(appliedViewBox));
        }

        [NUnit.Framework.Test]
        public virtual void ApplyViewBoxXMaxYMinMeetHorizontalTest() {
            Rectangle appliedViewBox = SvgCoordinateUtils.ApplyViewBox(VIEW_BOX, VIEW_PORT_HORIZONTAL, SvgConstants.Values
                .XMAX_YMIN, SvgConstants.Values.MEET);
            NUnit.Framework.Assert.IsTrue(new Rectangle(180F, 0F, 60F, 60F).EqualsWithEpsilon(appliedViewBox));
        }

        [NUnit.Framework.Test]
        public virtual void ApplyViewBoxXMaxYMinSliceHorizontalTest() {
            Rectangle appliedViewBox = SvgCoordinateUtils.ApplyViewBox(VIEW_BOX, VIEW_PORT_HORIZONTAL, SvgConstants.Values
                .XMAX_YMIN, SvgConstants.Values.SLICE);
            NUnit.Framework.Assert.IsTrue(new Rectangle(-100F, -200F, 100F, 100F).EqualsWithEpsilon(appliedViewBox));
        }

        [NUnit.Framework.Test]
        public virtual void ApplyViewBoxXMaxYMinMeetVerticalTest() {
            Rectangle appliedViewBox = SvgCoordinateUtils.ApplyViewBox(VIEW_BOX, VIEW_PORT_VERTICAL, SvgConstants.Values
                .XMAX_YMIN, SvgConstants.Values.MEET);
            NUnit.Framework.Assert.IsTrue(new Rectangle(60F, 0F, 60F, 60F).EqualsWithEpsilon(appliedViewBox));
        }

        [NUnit.Framework.Test]
        public virtual void ApplyViewBoxXMaxYMinSliceVerticalTest() {
            Rectangle appliedViewBox = SvgCoordinateUtils.ApplyViewBox(VIEW_BOX, VIEW_PORT_VERTICAL, SvgConstants.Values
                .XMAX_YMIN, SvgConstants.Values.SLICE);
            NUnit.Framework.Assert.IsTrue(new Rectangle(-300F, -200F, 100F, 100F).EqualsWithEpsilon(appliedViewBox));
        }

        [NUnit.Framework.Test]
        public virtual void ApplyViewBoxXMaxYMidMeetHorizontalTest() {
            Rectangle appliedViewBox = SvgCoordinateUtils.ApplyViewBox(VIEW_BOX, VIEW_PORT_HORIZONTAL, SvgConstants.Values
                .XMAX_YMID, SvgConstants.Values.MEET);
            NUnit.Framework.Assert.IsTrue(new Rectangle(180F, 0F, 60F, 60F).EqualsWithEpsilon(appliedViewBox));
        }

        [NUnit.Framework.Test]
        public virtual void ApplyViewBoxXMaxYMidSliceHorizontalTest() {
            Rectangle appliedViewBox = SvgCoordinateUtils.ApplyViewBox(VIEW_BOX, VIEW_PORT_HORIZONTAL, SvgConstants.Values
                .XMAX_YMID, SvgConstants.Values.SLICE);
            NUnit.Framework.Assert.IsTrue(new Rectangle(-100F, -300F, 100F, 100F).EqualsWithEpsilon(appliedViewBox));
        }

        [NUnit.Framework.Test]
        public virtual void ApplyViewBoxXMaxYMidMeetVerticalTest() {
            Rectangle appliedViewBox = SvgCoordinateUtils.ApplyViewBox(VIEW_BOX, VIEW_PORT_VERTICAL, SvgConstants.Values
                .XMAX_YMID, SvgConstants.Values.MEET);
            NUnit.Framework.Assert.IsTrue(new Rectangle(60F, 60F, 60F, 60F).EqualsWithEpsilon(appliedViewBox));
        }

        [NUnit.Framework.Test]
        public virtual void ApplyViewBoxXMaxYMidSliceVerticalTest() {
            Rectangle appliedViewBox = SvgCoordinateUtils.ApplyViewBox(VIEW_BOX, VIEW_PORT_VERTICAL, SvgConstants.Values
                .XMAX_YMID, SvgConstants.Values.SLICE);
            NUnit.Framework.Assert.IsTrue(new Rectangle(-300F, -200F, 100F, 100F).EqualsWithEpsilon(appliedViewBox));
        }

        [NUnit.Framework.Test]
        public virtual void ApplyViewBoxXMaxYMaxMeetHorizontalTest() {
            Rectangle appliedViewBox = SvgCoordinateUtils.ApplyViewBox(VIEW_BOX, VIEW_PORT_HORIZONTAL, SvgConstants.Values
                .XMAX_YMAX, SvgConstants.Values.MEET);
            NUnit.Framework.Assert.IsTrue(new Rectangle(180F, 0F, 60F, 60F).EqualsWithEpsilon(appliedViewBox));
        }

        [NUnit.Framework.Test]
        public virtual void ApplyViewBoxXMaxYMaxSliceHorizontalTest() {
            Rectangle appliedViewBox = SvgCoordinateUtils.ApplyViewBox(VIEW_BOX, VIEW_PORT_HORIZONTAL, SvgConstants.Values
                .XMAX_YMAX, SvgConstants.Values.SLICE);
            NUnit.Framework.Assert.IsTrue(new Rectangle(-100F, -400F, 100F, 100F).EqualsWithEpsilon(appliedViewBox));
        }

        [NUnit.Framework.Test]
        public virtual void ApplyViewBoxXMaxYMaxMeetVerticalTest() {
            Rectangle appliedViewBox = SvgCoordinateUtils.ApplyViewBox(VIEW_BOX, VIEW_PORT_VERTICAL, SvgConstants.Values
                .XMAX_YMAX, SvgConstants.Values.MEET);
            NUnit.Framework.Assert.IsTrue(new Rectangle(60F, 120F, 60F, 60F).EqualsWithEpsilon(appliedViewBox));
        }

        [NUnit.Framework.Test]
        public virtual void ApplyViewBoxXMaxYMaxSliceVerticalTest() {
            Rectangle appliedViewBox = SvgCoordinateUtils.ApplyViewBox(VIEW_BOX, VIEW_PORT_VERTICAL, SvgConstants.Values
                .XMAX_YMAX, SvgConstants.Values.SLICE);
            NUnit.Framework.Assert.IsTrue(new Rectangle(-300F, -200F, 100F, 100F).EqualsWithEpsilon(appliedViewBox));
        }
    }
}
