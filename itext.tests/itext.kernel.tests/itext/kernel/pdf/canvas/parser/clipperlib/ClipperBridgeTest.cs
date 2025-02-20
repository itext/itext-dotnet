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
using System.Collections.Generic;
using System.Linq;
using iText.Commons.Utils;
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Canvas;
using iText.Test;

namespace iText.Kernel.Pdf.Canvas.Parser.ClipperLib {
    [NUnit.Framework.Category("UnitTest")]
    public class ClipperBridgeTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void SquareClippingTest() {
            Subpath squareSubpath = new Subpath(new Point(10, 10));
            squareSubpath.AddSegment(new Line(10, 10, 10, 30));
            squareSubpath.AddSegment(new Line(10, 30, 30, 30));
            squareSubpath.AddSegment(new Line(30, 30, 30, 10));
            squareSubpath.AddSegment(new Line(30, 10, 10, 10));
            squareSubpath.SetClosed(true);
            Path squarePath = new Path();
            squarePath.AddSubpath(squareSubpath);
            Subpath rectangleSubpath = new Subpath(new Point(20, 20));
            rectangleSubpath.AddSegment(new Line(20, 20, 20, 40));
            rectangleSubpath.AddSegment(new Line(20, 40, 30, 40));
            rectangleSubpath.AddSegment(new Line(30, 40, 30, 20));
            rectangleSubpath.AddSegment(new Line(30, 20, 20, 20));
            rectangleSubpath.SetClosed(true);
            Path rectanglePath = new Path();
            rectanglePath.AddSubpath(rectangleSubpath);
            Clipper clipper = new Clipper();
            ClipperBridge clipperBridge = new ClipperBridge(squarePath, rectanglePath);
            clipperBridge.AddPath(clipper, squarePath, PolyType.SUBJECT);
            clipperBridge.AddPath(clipper, rectanglePath, PolyType.CLIP);
            PolyTree polyTree = new PolyTree();
            clipper.Execute(ClipType.UNION, polyTree);
            Path result = clipperBridge.ConvertToPath(polyTree);
            NUnit.Framework.Assert.AreEqual(new Point(20, 40), result.GetCurrentPoint());
            NUnit.Framework.Assert.AreEqual(2, result.GetSubpaths().Count);
            Subpath closedPath = result.GetSubpaths()[0];
            NUnit.Framework.Assert.AreEqual(new Point(20, 40), closedPath.GetStartPoint());
            IList<IShape> closedPartSegments = closedPath.GetSegments();
            NUnit.Framework.Assert.AreEqual(5, closedPartSegments.Count);
            NUnit.Framework.Assert.IsTrue(AreShapesEqual(new Line(20, 40, 20, 30), closedPartSegments[0]));
            NUnit.Framework.Assert.IsTrue(AreShapesEqual(new Line(20, 30, 10, 30), closedPartSegments[1]));
            NUnit.Framework.Assert.IsTrue(AreShapesEqual(new Line(10, 30, 10, 10), closedPartSegments[2]));
            NUnit.Framework.Assert.IsTrue(AreShapesEqual(new Line(10, 10, 30, 10), closedPartSegments[3]));
            NUnit.Framework.Assert.IsTrue(AreShapesEqual(new Line(30, 10, 30, 40), closedPartSegments[4]));
            NUnit.Framework.Assert.IsTrue(closedPath.IsClosed());
            Subpath openPart = result.GetSubpaths()[1];
            NUnit.Framework.Assert.AreEqual(new Point(20, 40), openPart.GetStartPoint());
            NUnit.Framework.Assert.AreEqual(0, openPart.GetSegments().Count);
            NUnit.Framework.Assert.IsFalse(openPart.IsClosed());
        }

        [NUnit.Framework.Test]
        public virtual void GetJoinTypeTest() {
            NUnit.Framework.Assert.AreEqual(JoinType.BEVEL, ClipperBridge.GetJoinType(PdfCanvasConstants.LineJoinStyle
                .BEVEL));
            NUnit.Framework.Assert.AreEqual(JoinType.MITER, ClipperBridge.GetJoinType(PdfCanvasConstants.LineJoinStyle
                .MITER));
            NUnit.Framework.Assert.AreEqual(JoinType.ROUND, ClipperBridge.GetJoinType(PdfCanvasConstants.LineJoinStyle
                .ROUND));
        }

        [NUnit.Framework.Test]
        public virtual void GetEndTypeTest() {
            NUnit.Framework.Assert.AreEqual(EndType.OPEN_BUTT, ClipperBridge.GetEndType(PdfCanvasConstants.LineCapStyle
                .BUTT));
            NUnit.Framework.Assert.AreEqual(EndType.OPEN_SQUARE, ClipperBridge.GetEndType(PdfCanvasConstants.LineCapStyle
                .PROJECTING_SQUARE));
            NUnit.Framework.Assert.AreEqual(EndType.OPEN_ROUND, ClipperBridge.GetEndType(PdfCanvasConstants.LineCapStyle
                .ROUND));
        }

        [NUnit.Framework.Test]
        public virtual void LongRectWidthTest() {
            IntRect longRect = new IntRect(14900000000000000L, 21275000000000000L, 71065802001953128L, 71075000000000000L
                );
            NUnit.Framework.Assert.AreEqual(561.658, new ClipperBridge().LongRectCalculateWidth(longRect), 0.001f);
        }

        [NUnit.Framework.Test]
        public virtual void LongRectHeightTest() {
            IntRect longRect = new IntRect(14900000000000000L, 21275000000000000L, 71065802001953128L, 71075000000000000L
                );
            NUnit.Framework.Assert.AreEqual(498, new ClipperBridge().LongRectCalculateHeight(longRect), 0.001f);
        }

        [NUnit.Framework.Test]
        public virtual void DynamicFloatMultiplierCalculationsSmallValuesTest() {
            Point[] points = new Point[] { new Point(1e-10, 0), new Point(0, 1e-13) };
            NUnit.Framework.Assert.AreEqual(1.8014398509481984e26, new ClipperBridge(points).GetFloatMultiplier(), 0e+10
                );
        }

        [NUnit.Framework.Test]
        public virtual void DynamicFloatMultiplierCalculationsBigValuesTest() {
            Point[] points = new Point[] { new Point(1e+11, 10), new Point(10, 1e+10) };
            NUnit.Framework.Assert.AreEqual(180143, new ClipperBridge(points).GetFloatMultiplier(), 0.001f);
        }

        [NUnit.Framework.Test]
        public virtual void SmallFloatMultiplierCoefficientTest() {
            Point[] points = new Point[] { new Point(1e-10, 1e+10) };
            NUnit.Framework.Assert.AreEqual(new IntPoint(0, 18014390000000000L), new ClipperBridge(points).ConvertToLongPoints
                (JavaUtil.ArraysAsList(points))[0]);
        }

        private bool AreShapesEqual(IShape expected, IShape actual) {
            if (expected == actual) {
                return true;
            }
            if (actual == null || expected.GetType() != actual.GetType()) {
                return false;
            }
            return Enumerable.SequenceEqual(expected.GetBasePoints(), actual.GetBasePoints());
        }
    }
}
