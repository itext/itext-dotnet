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
