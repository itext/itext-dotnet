/*
This file is part of the iText (R) project.
Copyright (c) 1998-2024 Apryse Group NV
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
using System.IO;
using iText.StyledXmlParser.Node;
using iText.Svg.Converter;
using iText.Svg.Processors;
using iText.Svg.Processors.Impl;
using iText.Test;

namespace iText.Svg.Renderers.Impl {
    [NUnit.Framework.Category("UnitTest")]
    public class DefsSvgNodeRendererUnitTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/svg/renderers/impl/DefsSvgNodeRendererTest/";

        [NUnit.Framework.Test]
        public virtual void ProcessDefsNoChildrenTest() {
            INode parsedSvg = SvgConverter.Parse(new FileStream(sourceFolder + "onlyDefsWithNoChildren.svg", FileMode.Open
                , FileAccess.Read));
            ISvgProcessorResult result = new DefaultSvgProcessor().Process(parsedSvg, null);
            NUnit.Framework.Assert.IsTrue(result.GetNamedObjects().IsEmpty());
        }

        [NUnit.Framework.Test]
        public virtual void ProcessDefsOneChildTest() {
            INode parsedSvg = SvgConverter.Parse(new FileStream(sourceFolder + "onlyDefsWithOneChild.svg", FileMode.Open
                , FileAccess.Read));
            ISvgProcessorResult result = new DefaultSvgProcessor().Process(parsedSvg, null);
            NUnit.Framework.Assert.IsTrue(result.GetNamedObjects().Get("circle1") is CircleSvgNodeRenderer);
        }

        [NUnit.Framework.Test]
        public virtual void ProcessDefsMultipleChildrenTest() {
            INode parsedSvg = SvgConverter.Parse(new FileStream(sourceFolder + "onlyDefsWithMultipleChildren.svg", FileMode.Open
                , FileAccess.Read));
            ISvgProcessorResult result = new DefaultSvgProcessor().Process(parsedSvg, null);
            NUnit.Framework.Assert.IsTrue(result.GetNamedObjects().Get("circle1") is CircleSvgNodeRenderer);
            NUnit.Framework.Assert.IsTrue(result.GetNamedObjects().Get("line1") is LineSvgNodeRenderer);
            NUnit.Framework.Assert.IsTrue(result.GetNamedObjects().Get("rect1") is RectangleSvgNodeRenderer);
        }

        [NUnit.Framework.Test]
        public virtual void ProcessDefsParentShouldBeNullTest() {
            INode parsedSvg = SvgConverter.Parse(new FileStream(sourceFolder + "onlyDefsWithOneChild.svg", FileMode.Open
                , FileAccess.Read));
            ISvgProcessorResult result = new DefaultSvgProcessor().Process(parsedSvg, null);
            NUnit.Framework.Assert.IsNull(result.GetNamedObjects().Get("circle1").GetParent());
        }

        [NUnit.Framework.Test]
        public virtual void NoObjectBoundingBoxTest() {
            DefsSvgNodeRenderer renderer = new DefsSvgNodeRenderer();
            NUnit.Framework.Assert.IsNull(renderer.GetObjectBoundingBox(null));
        }
    }
}
