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
using System.IO;
using iText.StyledXmlParser.Node;
using iText.Svg.Converter;
using iText.Svg.Processors;
using iText.Svg.Processors.Impl;
using iText.Svg.Renderers;
using iText.Test.Attributes;

namespace iText.Svg.Renderers.Impl {
    [NUnit.Framework.Category("UnitTest")]
    public class NamedObjectsTest : SvgIntegrationTest {
        [NUnit.Framework.Test]
        [LogMessage(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.RULE_IS_NOT_SUPPORTED)]
        public virtual void AddNamedObject() {
            INode parsedSvg = SvgConverter.Parse(new FileStream(iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
                .CurrentContext.TestDirectory) + "/resources/itext/svg/renderers/impl/NamedObjectsTest/names.svg", FileMode.Open
                , FileAccess.Read));
            ISvgProcessorResult result = new DefaultSvgProcessor().Process(parsedSvg, null);
            NUnit.Framework.Assert.IsTrue(result.GetNamedObjects().Get("name_svg") is SvgTagSvgNodeRenderer);
            NUnit.Framework.Assert.IsTrue(result.GetNamedObjects().Get("name_rect") is RectangleSvgNodeRenderer);
        }
    }
}
