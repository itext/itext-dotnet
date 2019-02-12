using System;
using System.Collections.Generic;
using iText.Kernel.Geom;
using iText.Svg;
using iText.Svg.Exceptions;
using iText.Svg.Renderers.Path;
using iText.Svg.Renderers.Path.Impl;

namespace iText.Svg.Renderers.Impl {
    public class PathSvgNodeRendererLowLevelIntegrationTest {
        [NUnit.Framework.Test]
        public virtual void TestRelativeArcOperatorShapes() {
            PathSvgNodeRenderer path = new PathSvgNodeRenderer();
            String instructions = "M 200,300 a 10 10 0 0 0 10 10";
            path.SetAttribute(SvgConstants.Attributes.D, instructions);
            IList<IPathShape> segments = (IList<IPathShape>)path.GetShapes();
            NUnit.Framework.Assert.AreEqual(2, segments.Count);
            NUnit.Framework.Assert.IsTrue(segments[0] is MoveTo);
            NUnit.Framework.Assert.IsTrue(segments[1] is EllipticalCurveTo);
        }

        [NUnit.Framework.Test]
        public virtual void TestRelativeArcOperatorCoordinates() {
            PathSvgNodeRenderer path = new PathSvgNodeRenderer();
            String instructions = "M 200,300 a 10 10 0 0 0 10 10";
            path.SetAttribute(SvgConstants.Attributes.D, instructions);
            IPathShape arc = ((IList<IPathShape>)path.GetShapes())[1];
            Point end = arc.GetEndingPoint();
            NUnit.Framework.Assert.AreEqual(new Point(210, 310), end);
        }

        [NUnit.Framework.Test]
        public virtual void TestMultipleRelativeArcOperatorCoordinates() {
            PathSvgNodeRenderer path = new PathSvgNodeRenderer();
            String instructions = "M 200,300 a 10 10 0 0 0 10 10 a 10 10 0 0 0 10 10";
            path.SetAttribute(SvgConstants.Attributes.D, instructions);
            IPathShape arc = ((IList<IPathShape>)path.GetShapes())[2];
            Point end = arc.GetEndingPoint();
            NUnit.Framework.Assert.AreEqual(new Point(220, 320), end);
        }

        [NUnit.Framework.Test]
        public virtual void TestAbsoluteArcOperatorCoordinates() {
            PathSvgNodeRenderer path = new PathSvgNodeRenderer();
            String instructions = "M 200,300 A 10 10 0 0 0 210 310";
            path.SetAttribute(SvgConstants.Attributes.D, instructions);
            IPathShape arc = ((IList<IPathShape>)path.GetShapes())[1];
            Point end = arc.GetEndingPoint();
            NUnit.Framework.Assert.AreEqual(new Point(210, 310), end);
        }

        [NUnit.Framework.Test]
        public virtual void TestMultipleAbsoluteArcOperatorCoordinates() {
            PathSvgNodeRenderer path = new PathSvgNodeRenderer();
            String instructions = "M 200,300 A 10 10 0 0 0 210 310 A 10 10 0 0 0 220 320";
            path.SetAttribute(SvgConstants.Attributes.D, instructions);
            IPathShape arc = ((IList<IPathShape>)path.GetShapes())[2];
            Point end = arc.GetEndingPoint();
            NUnit.Framework.Assert.AreEqual(new Point(220, 320), end);
        }

        // tests resulting in empty path
        [NUnit.Framework.Test]
        public virtual void TestEmptyPath() {
            PathSvgNodeRenderer path = new PathSvgNodeRenderer();
            String instructions = "";
            path.SetAttribute(SvgConstants.Attributes.D, instructions);
            NUnit.Framework.Assert.IsTrue(path.GetShapes().IsEmpty());
        }

        [NUnit.Framework.Test]
        public virtual void TestNonsensePathNoOperators() {
            PathSvgNodeRenderer path = new PathSvgNodeRenderer();
            String instructions = "200";
            path.SetAttribute(SvgConstants.Attributes.D, instructions);
            NUnit.Framework.Assert.IsTrue(path.GetShapes().IsEmpty());
        }

        [NUnit.Framework.Test]
        public virtual void TestNonsensePathNotExistingOperator() {
            NUnit.Framework.Assert.That(() =>  {
                PathSvgNodeRenderer path = new PathSvgNodeRenderer();
                String instructions = "F";
                path.SetAttribute(SvgConstants.Attributes.D, instructions);
                NUnit.Framework.Assert.IsTrue(path.GetShapes().IsEmpty());
            }
            , NUnit.Framework.Throws.InstanceOf<SvgProcessingException>())
;
        }

        [NUnit.Framework.Test]
        public virtual void TestClosePathNoPrecedingPathsOperator() {
            NUnit.Framework.Assert.That(() =>  {
                PathSvgNodeRenderer path = new PathSvgNodeRenderer();
                String instructions = "z";
                path.SetAttribute(SvgConstants.Attributes.D, instructions);
                NUnit.Framework.Assert.IsTrue(path.GetShapes().IsEmpty());
            }
            , NUnit.Framework.Throws.InstanceOf<SvgProcessingException>())
;
        }

        [NUnit.Framework.Test]
        public virtual void TestMoveNoArgsOperator() {
            NUnit.Framework.Assert.That(() =>  {
                PathSvgNodeRenderer path = new PathSvgNodeRenderer();
                String instructions = "M";
                path.SetAttribute(SvgConstants.Attributes.D, instructions);
                NUnit.Framework.Assert.IsTrue(path.GetShapes().IsEmpty());
            }
            , NUnit.Framework.Throws.InstanceOf<ArgumentException>())
;
        }

        [NUnit.Framework.Test]
        public virtual void TestMoveOddArgsOperator() {
            NUnit.Framework.Assert.That(() =>  {
                PathSvgNodeRenderer path = new PathSvgNodeRenderer();
                String instructions = "M 500";
                path.SetAttribute(SvgConstants.Attributes.D, instructions);
                NUnit.Framework.Assert.IsTrue(path.GetShapes().IsEmpty());
            }
            , NUnit.Framework.Throws.InstanceOf<ArgumentException>())
;
        }
    }
}
