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
using System.Collections.Generic;
using iText.Kernel.Geom;
using iText.Svg;
using iText.Svg.Exceptions;
using iText.Svg.Renderers;
using iText.Svg.Renderers.Path;
using iText.Svg.Renderers.Path.Impl;

namespace iText.Svg.Renderers.Impl {
    [NUnit.Framework.Category("IntegrationTest")]
    public class PathSvgNodeRendererLowLevelIntegrationTest : SvgIntegrationTest {
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
            PathSvgNodeRenderer path = new PathSvgNodeRenderer();
            String instructions = "F";
            path.SetAttribute(SvgConstants.Attributes.D, instructions);
            NUnit.Framework.Assert.Catch(typeof(SvgProcessingException), () => path.GetShapes());
        }

        [NUnit.Framework.Test]
        public virtual void TestClosePathNoPrecedingPathsOperator() {
            PathSvgNodeRenderer path = new PathSvgNodeRenderer();
            String instructions = "z";
            path.SetAttribute(SvgConstants.Attributes.D, instructions);
            NUnit.Framework.Assert.Catch(typeof(SvgProcessingException), () => path.GetShapes());
        }

        [NUnit.Framework.Test]
        public virtual void TestMoveNoArgsOperator() {
            PathSvgNodeRenderer path = new PathSvgNodeRenderer();
            String instructions = "M";
            path.SetAttribute(SvgConstants.Attributes.D, instructions);
            NUnit.Framework.Assert.IsTrue(path.GetShapes().IsEmpty());
        }

        [NUnit.Framework.Test]
        public virtual void TestMoveOddArgsOperator() {
            PathSvgNodeRenderer path = new PathSvgNodeRenderer();
            String instructions = "M 500";
            path.SetAttribute(SvgConstants.Attributes.D, instructions);
            NUnit.Framework.Assert.IsTrue(path.GetShapes().IsEmpty());
        }

        [NUnit.Framework.Test]
        public virtual void TestAddMultipleArgsOperator() {
            PathSvgNodeRenderer path = new PathSvgNodeRenderer();
            String instructions = "M 500 500 200 200 300 300";
            path.SetAttribute(SvgConstants.Attributes.D, instructions);
            NUnit.Framework.Assert.AreEqual(3, path.GetShapes().Count);
        }

        [NUnit.Framework.Test]
        public virtual void TestAddMultipleOddArgsOperator() {
            PathSvgNodeRenderer path = new PathSvgNodeRenderer();
            String instructions = "L 500 500 200 200 300";
            path.SetAttribute(SvgConstants.Attributes.D, instructions);
            NUnit.Framework.Assert.AreEqual(2, path.GetShapes().Count);
        }

        [NUnit.Framework.Test]
        public virtual void TestAddMultipleOddArgsOperatorThenOtherStuff() {
            PathSvgNodeRenderer path = new PathSvgNodeRenderer();
            String instructions = "M 500 500 200 200 300 z";
            path.SetAttribute(SvgConstants.Attributes.D, instructions);
            NUnit.Framework.Assert.AreEqual(3, path.GetShapes().Count);
            NUnit.Framework.Assert.IsTrue(((IList<IPathShape>)path.GetShapes())[2] is ClosePath);
        }

        [NUnit.Framework.Test]
        public virtual void TestAddDoubleArgsOperator() {
            PathSvgNodeRenderer path = new PathSvgNodeRenderer();
            String instructions = "M 500 500 S 200 100 100 200 300 300 400 400";
            path.SetAttribute(SvgConstants.Attributes.D, instructions);
            NUnit.Framework.Assert.AreEqual(3, path.GetShapes().Count);
            NUnit.Framework.Assert.IsTrue(((IList<IPathShape>)path.GetShapes())[2] is SmoothSCurveTo);
        }

        [NUnit.Framework.Test]
        public virtual void SmoothCurveAsFirstShapeTest1() {
            String instructions = "S 100 200 300 400";
            PathSvgNodeRenderer path = new PathSvgNodeRenderer();
            path.SetAttribute(SvgConstants.Attributes.D, instructions);
            Exception e = NUnit.Framework.Assert.Catch(typeof(SvgProcessingException), () => path.GetShapes());
            NUnit.Framework.Assert.AreEqual(SvgExceptionMessageConstant.INVALID_SMOOTH_CURVE_USE, e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void SmoothCurveAsFirstShapeTest2() {
            String instructions = "T 100,200";
            PathSvgNodeRenderer path = new PathSvgNodeRenderer();
            path.SetAttribute(SvgConstants.Attributes.D, instructions);
            Exception e = NUnit.Framework.Assert.Catch(typeof(SvgProcessingException), () => path.GetShapes());
            NUnit.Framework.Assert.AreEqual(SvgExceptionMessageConstant.INVALID_SMOOTH_CURVE_USE, e.Message);
        }
    }
}
