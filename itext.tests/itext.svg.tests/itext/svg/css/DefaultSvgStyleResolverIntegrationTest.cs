using System;
using System.Collections.Generic;
using iText.StyledXmlParser.Node;
using iText.StyledXmlParser.Node.Impl.Jsoup;
using iText.Svg.Processors;
using iText.Svg.Processors.Impl;
using iText.Svg.Renderers;

namespace iText.Svg.Css {
    public class DefaultSvgStyleResolverIntegrationTest {
        [NUnit.Framework.Test]
        public virtual void RedCirleTest() {
            String svg = "<svg\n" + "   width=\"210mm\"\n" + "   height=\"297mm\"\n" + "   viewBox=\"0 0 210 297\"\n" 
                + "   version=\"1.1\"\n" + "  <title id=\"title4508\">Red Circle</title>\n" + "    <ellipse\n" + "       id=\"path3699\"\n"
                 + "       cx=\"96.005951\"\n" + "       cy=\"110.65774\"\n" + "       rx=\"53.672619\"\n" + "       ry=\"53.294643\"\n"
                 + "       style=\"stroke-width:1.76388889;stroke:#da0000;stroke-opacity:1;fill:none;stroke-miterlimit:4;stroke-dasharray:none\" />\n"
                 + "</svg>\n";
            ISvgProcessor processor = new DefaultSvgProcessor();
            JsoupXmlParser xmlParser = new JsoupXmlParser();
            IDocumentNode root = xmlParser.Parse(svg);
            IBranchSvgNodeRenderer nodeRenderer = (IBranchSvgNodeRenderer)processor.Process(root);
            IDictionary<String, String> actual = new Dictionary<String, String>();
            //Traverse to ellipse
            ISvgNodeRenderer ellipse = nodeRenderer.GetChildren()[0];
            actual.Put("stroke", ellipse.GetAttribute("stroke"));
            actual.Put("stroke-width", ellipse.GetAttribute("stroke-width"));
            actual.Put("stroke-opacity", ellipse.GetAttribute("stroke-opacity"));
            IDictionary<String, String> expected = new Dictionary<String, String>();
            expected.Put("stroke-width", "1.76388889");
            expected.Put("stroke", "#da0000");
            expected.Put("stroke-opacity", "1");
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void StyleTagProcessingTest() {
            String svg = "<svg\n" + "   width=\"210mm\"\n" + "   height=\"297mm\"\n" + "   viewBox=\"0 0 210 297\"\n" 
                + "   version=\"1.1\"\n" + "   id=\"svg8\"\n" + "   >\n" + "  <style>\n" + "\tellipse{\n" + "\t\tstroke-width:1.76388889;\n"
                 + "\t\tstroke:#da0000;\n" + "\t\tstroke-opacity:1;\n" + "\t}\n" + "  </style>\n" + "    <ellipse\n" +
                 "       id=\"path3699\"\n" + "       cx=\"96.005951\"\n" + "       cy=\"110.65774\"\n" + "       rx=\"53.672619\"\n"
                 + "       ry=\"53.294643\"\n" + "       style=\"fill:none;stroke-miterlimit:4;stroke-dasharray:none\" />\n"
                 + "</svg>\n";
            ISvgProcessor processor = new DefaultSvgProcessor();
            JsoupXmlParser xmlParser = new JsoupXmlParser();
            IDocumentNode root = xmlParser.Parse(svg);
            IBranchSvgNodeRenderer nodeRenderer = (IBranchSvgNodeRenderer)processor.Process(root);
            IDictionary<String, String> actual = new Dictionary<String, String>();
            //Traverse to ellipse
            ISvgNodeRenderer ellipse = nodeRenderer.GetChildren()[0];
            actual.Put("stroke", ellipse.GetAttribute("stroke"));
            actual.Put("stroke-width", ellipse.GetAttribute("stroke-width"));
            actual.Put("stroke-opacity", ellipse.GetAttribute("stroke-opacity"));
            IDictionary<String, String> expected = new Dictionary<String, String>();
            expected.Put("stroke-width", "1.76388889");
            expected.Put("stroke", "#da0000");
            expected.Put("stroke-opacity", "1");
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }
    }
}
