using iText.Svg;
using iText.Svg.Renderers.Impl;

namespace iText.Svg.Renderers {
    public class DeepCopyIntegrationTest {
        [NUnit.Framework.Test]
        public virtual void DeepCopyTest() {
            //Deep copy of tree with nested svg and group and set attributes
            UseSvgNodeRenderer nestedUse = new UseSvgNodeRenderer();
            nestedUse.SetAttribute(SvgConstants.Attributes.HREF, "#c1");
            GroupSvgNodeRenderer nestedGroup = new GroupSvgNodeRenderer();
            nestedGroup.SetAttribute(SvgConstants.Attributes.FILL, "blue");
            nestedGroup.AddChild(nestedUse);
            CircleSvgNodeRenderer nestedCircle = new CircleSvgNodeRenderer();
            nestedCircle.SetAttribute(SvgConstants.Attributes.R, "100");
            SvgTagSvgNodeRenderer nestedSvg = new SvgTagSvgNodeRenderer();
            nestedSvg.SetAttribute(SvgConstants.Attributes.X, "200");
            nestedSvg.SetAttribute(SvgConstants.Attributes.Y, "200");
            nestedSvg.SetAttribute(SvgConstants.Attributes.XMLNS, SvgConstants.Values.SVGNAMESPACEURL);
            nestedSvg.SetAttribute(SvgConstants.Attributes.VERSION, SvgConstants.Values.VERSION1_1);
            nestedSvg.AddChild(nestedCircle);
            nestedSvg.AddChild(nestedGroup);
            RectangleSvgNodeRenderer nestedRectangle = new RectangleSvgNodeRenderer();
            nestedRectangle.SetAttribute(SvgConstants.Attributes.WIDTH, "100");
            nestedRectangle.SetAttribute(SvgConstants.Attributes.HEIGHT, "50");
            GroupSvgNodeRenderer topGroup = new GroupSvgNodeRenderer();
            topGroup.SetAttribute(SvgConstants.Attributes.FILL, "red");
            topGroup.AddChild(nestedRectangle);
            CircleSvgNodeRenderer topCircle = new CircleSvgNodeRenderer();
            topCircle.SetAttribute(SvgConstants.Attributes.R, "80");
            topCircle.SetAttribute(SvgConstants.Attributes.X, "100");
            topCircle.SetAttribute(SvgConstants.Attributes.Y, "100");
            topCircle.SetAttribute(SvgConstants.Attributes.STROKE, "red");
            topCircle.SetAttribute(SvgConstants.Attributes.FILL, "green");
            SvgTagSvgNodeRenderer topSvg = new SvgTagSvgNodeRenderer();
            topSvg.SetAttribute(SvgConstants.Attributes.WIDTH, "800");
            topSvg.SetAttribute(SvgConstants.Attributes.HEIGHT, "800");
            topSvg.SetAttribute(SvgConstants.Attributes.XMLNS, SvgConstants.Values.SVGNAMESPACEURL);
            topSvg.SetAttribute(SvgConstants.Attributes.VERSION, SvgConstants.Values.VERSION1_1);
            topSvg.AddChild(topCircle);
            topSvg.AddChild(topGroup);
            EllipseSvgNodeRenderer ellipse = new EllipseSvgNodeRenderer();
            ellipse.SetAttribute(SvgConstants.Attributes.CX, "10");
            ellipse.SetAttribute(SvgConstants.Attributes.CY, "20");
            ellipse.SetAttribute(SvgConstants.Attributes.RX, "30");
            ellipse.SetAttribute(SvgConstants.Attributes.RX, "40");
            topSvg.AddChild(ellipse);
            ISvgNodeRenderer copy = topSvg.CreateDeepCopy();
            NUnit.Framework.Assert.AreEqual(topSvg, copy);
        }
    }
}
