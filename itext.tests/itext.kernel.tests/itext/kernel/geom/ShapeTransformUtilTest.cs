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
using iText.Commons.Utils;
using iText.Test;

namespace iText.Kernel.Geom {
    [NUnit.Framework.Category("UnitTest")]
    public class ShapeTransformUtilTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void TransformBezierCurveTest() {
            BezierCurve inBezierCurve = new BezierCurve(JavaUtil.ArraysAsList(new Point(0, 0), new Point(0, 5), new Point
                (5, 5), new Point(5, 0)));
            Matrix ctm = new Matrix(1, 0, 0, 1, 5, 5);
            BezierCurve outBezierCurve = ShapeTransformUtil.TransformBezierCurve(inBezierCurve, ctm);
            BezierCurve cmpBezierCurve = new BezierCurve(JavaUtil.ArraysAsList(new Point(-5, -5), new Point(-5, 0), new 
                Point(0, 0), new Point(0, -5)));
            NUnit.Framework.Assert.AreEqual(cmpBezierCurve.GetBasePoints().ToArray(), outBezierCurve.GetBasePoints().ToArray
                ());
        }

        [NUnit.Framework.Test]
        public virtual void TransformLineTest() {
            Line inLine = new Line(new Point(0, 0), new Point(10, 10));
            Matrix ctm = new Matrix(2, 0, 0, 1, 5, 5);
            Line outLine = ShapeTransformUtil.TransformLine(inLine, ctm);
            Line cmpLine = new Line(new Point(-2.5, -5), new Point(2.5, 5));
            NUnit.Framework.Assert.AreEqual(cmpLine.GetBasePoints().ToArray(), outLine.GetBasePoints().ToArray());
        }

        [NUnit.Framework.Test]
        public virtual void TransformPathTest() {
            Line inLine = new Line(new Point(0, 0), new Point(10, 10));
            BezierCurve inBezierCurve = new BezierCurve(JavaUtil.ArraysAsList(new Point(0, 0), new Point(0, 5), new Point
                (5, 5), new Point(5, 0)));
            Subpath inSubpath = new Subpath();
            inSubpath.AddSegment(inLine);
            inSubpath.AddSegment(inBezierCurve);
            Path inPath = new Path(JavaUtil.ArraysAsList(inSubpath));
            Matrix ctm = new Matrix(1, 0, 0, 1, 5, 5);
            Path outPath = ShapeTransformUtil.TransformPath(inPath, ctm);
            Line cmpLine = new Line(new Point(-5, -5), new Point(5, 5));
            BezierCurve cmpBezierCurve = new BezierCurve(JavaUtil.ArraysAsList(new Point(-5, -5), new Point(-5, 0), new 
                Point(0, 0), new Point(0, -5)));
            Subpath cmpSubpath = new Subpath();
            inSubpath.AddSegment(cmpLine);
            inSubpath.AddSegment(cmpBezierCurve);
            Path cmpPath = new Path(JavaUtil.ArraysAsList(cmpSubpath));
            for (int i = 0; i < cmpPath.GetSubpaths().Count; i++) {
                Subpath subpath = cmpPath.GetSubpaths()[i];
                for (int j = 0; j < subpath.GetSegments().Count; j++) {
                    IShape cmpShape = subpath.GetSegments()[j];
                    IShape outShape = outPath.GetSubpaths()[i].GetSegments()[j];
                    NUnit.Framework.Assert.AreEqual(cmpShape.GetBasePoints().ToArray(), outShape.GetBasePoints().ToArray());
                }
            }
        }
    }
}
