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
using System.IO;
using iText.Commons.Utils;
using iText.StyledXmlParser.Node;
using iText.StyledXmlParser.Node.Impl.Jsoup;
using iText.Svg.Processors.Impl;
using iText.Svg.Renderers;

namespace iText.Svg.Processors {
    [NUnit.Framework.Category("IntegrationTest")]
    public class DefaultSvgProcessorIntegrationTest : SvgIntegrationTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/svg/processors/impl/DefaultSvgProcessorIntegrationTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/svg/processors/impl/DefaultSvgProcessorIntegrationTest/";

        [NUnit.Framework.Test]
        public virtual void DefaultBehaviourTest() {
            String svgFile = sourceFolder + "RedCircle.svg";
            Stream svg = FileUtil.GetInputStreamForFile(svgFile);
            JsoupXmlParser xmlParser = new JsoupXmlParser();
            IDocumentNode root = xmlParser.Parse(svg, null);
            IBranchSvgNodeRenderer actual = (IBranchSvgNodeRenderer)new DefaultSvgProcessor().Process(root, null).GetRootRenderer
                ();
            //Attribute comparison from the known RedCircle.svg
            IDictionary<String, String> attrs = actual.GetChildren()[0].GetAttributeMapCopy();
            NUnit.Framework.Assert.AreEqual(12, attrs.Keys.Count, "Number of parsed attributes is wrong");
            NUnit.Framework.Assert.AreEqual("1", attrs.Get("stroke-opacity"), "The stroke-opacity attribute doesn't correspond it's value"
                );
            NUnit.Framework.Assert.AreEqual("1.76388889", attrs.Get("stroke-width"), "The stroke-width attribute doesn't correspond it's value"
                );
            NUnit.Framework.Assert.AreEqual("path3699", attrs.Get("id"), "The id attribute doesn't correspond it's value"
                );
            NUnit.Framework.Assert.AreEqual("none", attrs.Get("stroke-dasharray"), "The stroke-dasharray attribute doesn't correspond it's value"
                );
        }

        [NUnit.Framework.Test]
        public virtual void NamedObjectRectangleTest() {
            String svgFile = sourceFolder + "namedObjectRectangleTest.svg";
            Stream svg = FileUtil.GetInputStreamForFile(svgFile);
            JsoupXmlParser xmlParser = new JsoupXmlParser();
            IDocumentNode root = xmlParser.Parse(svg, null);
            ISvgProcessorResult processorResult = new DefaultSvgProcessor().Process(root, null);
            IDictionary<String, ISvgNodeRenderer> actual = processorResult.GetNamedObjects();
            NUnit.Framework.Assert.AreEqual(1, actual.Count);
            NUnit.Framework.Assert.IsTrue(actual.ContainsKey("MyRect"));
        }
    }
}
