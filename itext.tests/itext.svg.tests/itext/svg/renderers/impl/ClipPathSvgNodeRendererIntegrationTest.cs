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
using iText.Svg.Processors;
using iText.Svg.Processors.Impl;
using iText.Svg.Renderers;
using iText.Test;

namespace iText.Svg.Renderers.Impl {
    [NUnit.Framework.Category("IntegrationTest")]
    public class ClipPathSvgNodeRendererIntegrationTest : SvgIntegrationTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/svg/renderers/impl/ClipPathTest/";

        private static readonly String DESTINATION_FOLDER = TestUtil.GetOutputPath() + "/svg/renderers/impl/ClipPathTest/";

        private SvgConverterProperties properties;

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            ITextTest.CreateDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void EmptyClipPathTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "emptyClipPath");
        }

        [NUnit.Framework.Test]
        public virtual void InvalidClipPathTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "invalidClipPath");
        }

        [NUnit.Framework.Test]
        public virtual void RectClipPathComplexTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "clippath_rect_complex");
        }

        [NUnit.Framework.Test]
        public virtual void RectClipPathSimpleTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "clippath_rect_simple");
        }

        [NUnit.Framework.Test]
        public virtual void CircleClipPathComplexTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "clippath_circle_complex");
        }

        [NUnit.Framework.Test]
        public virtual void CircleClipPathSimpleTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "clippath_circle_simple");
        }

        [NUnit.Framework.Test]
        public virtual void MultiClipPathComplexTest() {
            //TODO DEVSIX-4044 SVG: support outline CSS property
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "clippath_multi_complex");
        }

        [NUnit.Framework.Test]
        public virtual void MoveClipPathTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "clippath_move");
        }

        [NUnit.Framework.Test]
        public virtual void MoveClipPathRuleMultipleTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "clippath_rule_multiple");
        }

        [NUnit.Framework.Test]
        public virtual void SimpleTranslateTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "simpleTranslate");
        }

        [NUnit.Framework.Test]
        public virtual void ClipRule() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "clipRule");
        }

        [NUnit.Framework.Test]
        public virtual void ClipPathRuleParameterVsFillRule() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "clipPathRuleParameterVsFillRule");
        }

        [NUnit.Framework.Test]
        public virtual void ClipPathRuleEvenoddNonzero() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "clipPathRuleEvenoddNonzero");
        }

        [NUnit.Framework.Test]
        public virtual void ClipPathCss() {
            properties = new SvgConverterProperties().SetBaseUri(SOURCE_FOLDER);
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "clipPathCss", properties);
        }

        [NUnit.Framework.Test]
        public virtual void ClipPathCssProperty() {
            //TODO DEVSIX-2946 Support clip-path CSS property
            properties = new SvgConverterProperties().SetBaseUri(SOURCE_FOLDER);
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "clipPathCssProperty", properties);
        }

        [NUnit.Framework.Test]
        public virtual void ClipPathRulesCombined() {
            // TODO DEVSIX-2589 Support clip and overflow attribute for symbol
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "clipPathRulesCombined");
        }

        [NUnit.Framework.Test]
        public virtual void InvalidClipPathTagTest() {
            // TODO: DEVSIX-3923 SVG: tags are processed in сase-insensitive way
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "clippath_invalid_tag");
        }

        [NUnit.Framework.Test]
        public virtual void ClipPathUnitsTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "clipPathUnits");
        }

        [NUnit.Framework.Test]
        public virtual void ClipPathUrlTopLevelTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "clipPathUrlTopLevel");
        }

        [NUnit.Framework.Test]
        public virtual void ClipPathUrl2ndLevelTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "clipPathUrl2ndLevel");
        }

        [NUnit.Framework.Test]
        public virtual void ClipPathUseTest() {
            // doesn't work in Chrome and Firefox too
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "clipPathUse");
        }

        [NUnit.Framework.Test]
        public virtual void ClipPathTextSimpleTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "clipPathText");
        }

        [NUnit.Framework.Test]
        public virtual void ClipPathSimpleNestedTextTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "clipPathSimpleNestedTextTest");
        }

        [NUnit.Framework.Test]
        public virtual void ClipPathNestedTextTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "clipPathNestedTextTest");
        }

        [NUnit.Framework.Test]
        public virtual void ClipPathNestedTextImageTest() {
            ISvgConverterProperties properties = new SvgConverterProperties().SetBaseUri(SOURCE_FOLDER);
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "clipPathNestedTextImage", properties);
        }

        [NUnit.Framework.Test]
        public virtual void ClipPathComplexTest() {
            ISvgConverterProperties properties = new SvgConverterProperties().SetBaseUri(SOURCE_FOLDER);
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "clipPathComplex", properties);
        }

        [NUnit.Framework.Test]
        public virtual void ClipPathTranslateText() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "clipPathTranslateText");
        }

        [NUnit.Framework.Test]
        public virtual void ClipPathNegativeXTranslateText() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "clipPathNegativeXTranslateText");
        }

        [NUnit.Framework.Test]
        public virtual void ClipPathNegativeDxText() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "clipPathNegativeDxText");
        }

        [NUnit.Framework.Test]
        public virtual void ClipPathTextBoldTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "clipPathTextBold");
        }

        [NUnit.Framework.Test]
        public virtual void ClipPathTextItalicTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "clipPathTextItalic");
        }

        [NUnit.Framework.Test]
        public virtual void ClipPathTextItalicBoldTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "clipPathTextItalicBold");
        }

        [NUnit.Framework.Test]
        public virtual void ClipPathTextMultiObjectsTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "clipPathTextMultiObjects");
        }

        [NUnit.Framework.Test]
        public virtual void ClipPathTextMultiObjects2Test() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "clipPathTextMultiObjects2");
        }

        [NUnit.Framework.Test]
        public virtual void ClipPathTextMultiObjects3Test() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "clipPathTextMultiObjects3");
        }

        [NUnit.Framework.Test]
        public virtual void ClipPathTextLinearGradientTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "clipPathTextLinearGrad");
        }

        [NUnit.Framework.Test]
        public virtual void ClipPathTextPatternTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "clipPathTextPattern");
        }

        [NUnit.Framework.Test]
        public virtual void ClipPathTextImageTest() {
            ISvgConverterProperties properties = new SvgConverterProperties().SetBaseUri(SOURCE_FOLDER);
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "clipPathTextImage", properties);
        }

        [NUnit.Framework.Test]
        public virtual void ClipPathStrokeText() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "clipPathStrokeText");
        }

        [NUnit.Framework.Test]
        public virtual void ClipPathOnlyStrokeText() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "clipPathOnlyStrokeText");
        }

        [NUnit.Framework.Test]
        public virtual void ClipPathFillText() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "clipPathFillText");
        }

        [NUnit.Framework.Test]
        public virtual void ClipPathUnderlineText() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "clipPathUnderlineText");
        }

        [NUnit.Framework.Test]
        public virtual void NotUsedClipPathOutsideDefsTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "notUsedClipPathOutsideDefs");
        }
    }
}
