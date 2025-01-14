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
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/svg/renderers/impl/ClipPathTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/svg/renderers/impl/ClipPathTest/";

        private SvgConverterProperties properties;

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            ITextTest.CreateDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void RectClipPathComplexTest() {
            ConvertAndCompareSinglePage(sourceFolder, destinationFolder, "clippath_rect_complex");
        }

        [NUnit.Framework.Test]
        public virtual void RectClipPathSimpleTest() {
            ConvertAndCompareSinglePage(sourceFolder, destinationFolder, "clippath_rect_simple");
        }

        [NUnit.Framework.Test]
        public virtual void CircleClipPathComplexTest() {
            ConvertAndCompareSinglePage(sourceFolder, destinationFolder, "clippath_circle_complex");
        }

        [NUnit.Framework.Test]
        public virtual void CircleClipPathSimpleTest() {
            ConvertAndCompareSinglePage(sourceFolder, destinationFolder, "clippath_circle_simple");
        }

        [NUnit.Framework.Test]
        public virtual void MultiClipPathComplexTest() {
            //TODO: update cmp file after DEVSIX-4044 will be fixed
            ConvertAndCompareSinglePage(sourceFolder, destinationFolder, "clippath_multi_complex");
        }

        [NUnit.Framework.Test]
        public virtual void MoveClipPathTest() {
            ConvertAndCompareSinglePage(sourceFolder, destinationFolder, "clippath_move");
        }

        [NUnit.Framework.Test]
        public virtual void MoveClipPathRuleMultipleTest() {
            ConvertAndCompareSinglePage(sourceFolder, destinationFolder, "clippath_rule_multiple");
        }

        [NUnit.Framework.Test]
        public virtual void ClipRule() {
            ConvertAndCompareSinglePage(sourceFolder, destinationFolder, "clipRule");
        }

        [NUnit.Framework.Test]
        public virtual void ClipPathRuleParameterVsFillRule() {
            ConvertAndCompareSinglePage(sourceFolder, destinationFolder, "clipPathRuleParameterVsFillRule");
        }

        [NUnit.Framework.Test]
        public virtual void ClipPathRuleEvenoddNonzero() {
            ConvertAndCompareSinglePage(sourceFolder, destinationFolder, "clipPathRuleEvenoddNonzero");
        }

        [NUnit.Framework.Test]
        public virtual void ClipPathCss() {
            //TODO: update after DEVSIX-2827
            properties = new SvgConverterProperties().SetBaseUri(sourceFolder);
            ConvertAndCompareSinglePage(sourceFolder, destinationFolder, "clipPathCss", properties);
        }

        [NUnit.Framework.Test]
        public virtual void ClipPathCssProperty() {
            //TODO: update after DEVSIX-2828
            properties = new SvgConverterProperties().SetBaseUri(sourceFolder);
            ConvertAndCompareSinglePage(sourceFolder, destinationFolder, "clipPathCssProperty", properties);
        }

        [NUnit.Framework.Test]
        public virtual void ClipPathRulesCombined() {
            // TODO DEVSIX-2589 Support overflow attribute for symbol
            ConvertAndCompareSinglePage(sourceFolder, destinationFolder, "clipPathRulesCombined");
        }

        [NUnit.Framework.Test]
        public virtual void InvalidClipPathTagTest() {
            // TODO: DEVSIX-3923 update cmp_ after fix
            ConvertAndCompareSinglePage(sourceFolder, destinationFolder, "clippath_invalid_tag");
        }

        [NUnit.Framework.Test]
        public virtual void ClipPathUnitsTest() {
            ConvertAndCompareSinglePage(sourceFolder, destinationFolder, "clipPathUnits");
        }

        [NUnit.Framework.Test]
        public virtual void ClipPathUrlTopLevelTest() {
            ConvertAndCompareSinglePage(sourceFolder, destinationFolder, "clipPathUrlTopLevel");
        }

        [NUnit.Framework.Test]
        public virtual void ClipPathUrl2ndLevelTest() {
            ConvertAndCompareSinglePage(sourceFolder, destinationFolder, "clipPathUrl2ndLevel");
        }

        [NUnit.Framework.Test]
        public virtual void ClipPathTextSimpleTest() {
            //TODO DEVSIX-2588: Update cmp files
            ConvertAndCompareSinglePage(sourceFolder, destinationFolder, "clipPathText");
        }

        [NUnit.Framework.Test]
        public virtual void ClipPathTextBoldTest() {
            //TODO DEVSIX-2588: Update cmp files
            ConvertAndCompareSinglePage(sourceFolder, destinationFolder, "clipPathTextBold");
        }

        [NUnit.Framework.Test]
        public virtual void ClipPathTextMultiObjectsTest() {
            //TODO DEVSIX-2588: Update cmp files
            ConvertAndCompareSinglePage(sourceFolder, destinationFolder, "clipPathTextMultiObjects");
        }

        [NUnit.Framework.Test]
        public virtual void ClipPathTextMultiObjects2Test() {
            //TODO DEVSIX-2588: Update cmp files
            ConvertAndCompareSinglePage(sourceFolder, destinationFolder, "clipPathTextMultiObjects2");
        }

        [NUnit.Framework.Test]
        public virtual void ClipPathTextMultiObjects3Test() {
            //TODO DEVSIX-2588: Update cmp files
            ConvertAndCompareSinglePage(sourceFolder, destinationFolder, "clipPathTextMultiObjects3");
        }

        [NUnit.Framework.Test]
        public virtual void ClipPathTextLinearGradientTest() {
            //TODO DEVSIX-2588: Update cmp files
            ConvertAndCompareSinglePage(sourceFolder, destinationFolder, "clipPathTextLinearGrad");
        }

        [NUnit.Framework.Test]
        public virtual void ClipPathTextPatternTest() {
            //TODO DEVSIX-2588: Update cmp files
            ConvertAndCompareSinglePage(sourceFolder, destinationFolder, "clipPathTextPattern");
        }

        [NUnit.Framework.Test]
        public virtual void ClipPathTextImageTest() {
            //TODO DEVSIX-2588: Update cmp files
            ISvgConverterProperties properties = new SvgConverterProperties().SetBaseUri(sourceFolder);
            ConvertAndCompareSinglePage(sourceFolder, destinationFolder, "clipPathTextImage", properties);
        }
    }
}
