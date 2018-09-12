using System;
using iText.Svg;
using iText.Svg.Renderers.Impl;

namespace iText.Svg.Css.Impl {
    public class SvgNodeRendererInheritanceResolverUnitTest {
        [NUnit.Framework.Test]
        public virtual void ApplyInheritanceToSubTreeFillTest() {
            String expectedFillAttribute = "blue";
            UseSvgNodeRenderer newRoot = new UseSvgNodeRenderer();
            newRoot.SetAttribute(SvgConstants.Attributes.FILL, expectedFillAttribute);
            GroupSvgNodeRenderer subTree = new GroupSvgNodeRenderer();
            RectangleSvgNodeRenderer rect = new RectangleSvgNodeRenderer();
            CircleSvgNodeRenderer circle = new CircleSvgNodeRenderer();
            subTree.AddChild(rect);
            subTree.AddChild(circle);
            SvgNodeRendererInheritanceResolver sru = new SvgNodeRendererInheritanceResolver();
            sru.ApplyInheritanceToSubTree(newRoot, subTree);
            NUnit.Framework.Assert.AreEqual(expectedFillAttribute, subTree.GetAttribute(SvgConstants.Attributes.FILL));
            NUnit.Framework.Assert.AreEqual(expectedFillAttribute, rect.GetAttribute(SvgConstants.Attributes.FILL));
            NUnit.Framework.Assert.AreEqual(expectedFillAttribute, circle.GetAttribute(SvgConstants.Attributes.FILL));
        }

        [NUnit.Framework.Test]
        public virtual void ApplyInheritanceToSubTreeFillDoNotOverwriteTest() {
        }
    }
}
