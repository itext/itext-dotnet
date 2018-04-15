using System;
using System.Collections.Generic;
using System.IO;
using iText.StyledXmlParser.Node;
using iText.StyledXmlParser.Node.Impl.Jsoup;
using iText.Svg.Processors.Impl;
using iText.Svg.Renderers;
using iText.Svg.Renderers.Impl;

namespace iText.Svg.Processors {
    public class DefaultSvgProcessorIntegrationTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/svg/processors/impl/DefaultSvgProcessorIntegrationTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/svg/processors/impl/DefaultSvgProcessorIntegrationTest/";

        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void DefaultBehaviourTest() {
            String svgFile = sourceFolder + "RedCircle.svg";
            Stream svg = new FileStream(svgFile, FileMode.Open, FileAccess.Read);
            ISvgProcessor processor = new DefaultSvgProcessor();
            JsoupXmlParser xmlParser = new JsoupXmlParser();
            IDocumentNode root = xmlParser.Parse(svg, null);
            IBranchSvgNodeRenderer actual = (IBranchSvgNodeRenderer)processor.Process(root);
            IBranchSvgNodeRenderer expected = new SvgSvgNodeRenderer();
            ISvgNodeRenderer expectedEllipse = new EllipseSvgNodeRenderer();
            IDictionary<String, String> expectedEllipseAttributes = new Dictionary<String, String>();
            expectedEllipse.SetAttributesAndStyles(expectedEllipseAttributes);
            expected.AddChild(expectedEllipse);
            //1 child
            NUnit.Framework.Assert.AreEqual(expected.GetChildren().Count, actual.GetChildren().Count);
        }
        //Attribute comparison
        //TODO(RND-868) : Replace above check with the following
        //Assert.assertEquals(expected,actual);
    }
}
