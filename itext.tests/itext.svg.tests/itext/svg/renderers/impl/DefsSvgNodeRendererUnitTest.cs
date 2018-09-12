using System;
using System.IO;
using iText.StyledXmlParser.Node;
using iText.Svg.Converter;
using iText.Svg.Processors;
using iText.Svg.Processors.Impl;

namespace iText.Svg.Renderers.Impl {
    public class DefsSvgNodeRendererUnitTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/svg/renderers/impl/DefsSvgNodeRendererTest/";

        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void ProcessDefsNoChildrenTest() {
            INode parsedSvg = SvgConverter.Parse(new FileStream(sourceFolder + "onlyDefsWithNoChildren.svg", FileMode.Open
                , FileAccess.Read));
            ISvgProcessorResult result = new DefaultSvgProcessor().Process(parsedSvg);
            NUnit.Framework.Assert.IsTrue(result.GetNamedObjects().IsEmpty());
        }

        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void ProcessDefsOneChildTest() {
            INode parsedSvg = SvgConverter.Parse(new FileStream(sourceFolder + "onlyDefsWithOneChild.svg", FileMode.Open
                , FileAccess.Read));
            ISvgProcessorResult result = new DefaultSvgProcessor().Process(parsedSvg);
            NUnit.Framework.Assert.IsTrue(result.GetNamedObjects().Get("circle1") is CircleSvgNodeRenderer);
        }

        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void ProcessDefsMultipleChildrenTest() {
            INode parsedSvg = SvgConverter.Parse(new FileStream(sourceFolder + "onlyDefsWithMultipleChildren.svg", FileMode.Open
                , FileAccess.Read));
            ISvgProcessorResult result = new DefaultSvgProcessor().Process(parsedSvg);
            NUnit.Framework.Assert.IsTrue(result.GetNamedObjects().Get("circle1") is CircleSvgNodeRenderer);
            NUnit.Framework.Assert.IsTrue(result.GetNamedObjects().Get("line1") is LineSvgNodeRenderer);
            NUnit.Framework.Assert.IsTrue(result.GetNamedObjects().Get("rect1") is RectangleSvgNodeRenderer);
        }

        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void ProcessDefsParentShouldBeNullTest() {
            INode parsedSvg = SvgConverter.Parse(new FileStream(sourceFolder + "onlyDefsWithOneChild.svg", FileMode.Open
                , FileAccess.Read));
            ISvgProcessorResult result = new DefaultSvgProcessor().Process(parsedSvg);
            NUnit.Framework.Assert.IsNull(result.GetNamedObjects().Get("circle1").GetParent());
        }
    }
}
