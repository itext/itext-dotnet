using iText.IO.Util;
using iText.Test;

namespace iText.Kernel.Geom {
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
