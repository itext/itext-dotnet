/*
This file is part of the iText (R) project.
Copyright (c) 1998-2020 iText Group NV
Authors: iText Software.

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
using iText.StyledXmlParser.Node;
using iText.StyledXmlParser.Node.Impl.Jsoup.Node;
using iText.Svg.Converter;
using iText.Svg.Exceptions;
using iText.Svg.Renderers;
using iText.Svg.Renderers.Impl;
using iText.Test;

namespace iText.Svg {
    public class DeprecatedApiTest : ExtendedITextTest {
        //This test class can safely be removed in 7.2
        [NUnit.Framework.Test]
        public virtual void ProcessNullTest() {
            NUnit.Framework.Assert.That(() =>  {
                SvgConverter.Process(null);
            }
            , NUnit.Framework.Throws.InstanceOf<SvgProcessingException>())
;
        }

        [NUnit.Framework.Test]
        public virtual void ProcessNode() {
            INode svg = new JsoupElementNode(new iText.StyledXmlParser.Jsoup.Nodes.Element(iText.StyledXmlParser.Jsoup.Parser.Tag
                .ValueOf("svg"), ""));
            IBranchSvgNodeRenderer node = (IBranchSvgNodeRenderer)SvgConverter.Process(svg).GetRootRenderer();
            NUnit.Framework.Assert.IsTrue(node is SvgTagSvgNodeRenderer);
            NUnit.Framework.Assert.AreEqual(0, node.GetChildren().Count);
            NUnit.Framework.Assert.IsNull(node.GetParent());
        }
    }
}
