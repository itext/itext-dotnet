/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: iText Software.

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License version 3
as published by the Free Software Foundation with the addition of the
following permission added to Section 15 as permitted in Section 7(a):
FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
OF THIRD PARTY RIGHTS

This program is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU Affero General Public License for more details.
You should have received a copy of the GNU Affero General Public License
along with this program; if not, see http://www.gnu.org/licenses or write to
the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
Boston, MA, 02110-1301 USA, or download the license from the following URL:
http://itextpdf.com/terms-of-use/

The interactive user interfaces in modified source and object code versions
of this program must display Appropriate Legal Notices, as required under
Section 5 of the GNU Affero General Public License.

In accordance with Section 7(b) of the GNU Affero General Public License,
a covered work must retain the producer line in every PDF that is created
or manipulated using iText.

You can be released from the requirements of the license by purchasing
a commercial license. Buying such a license is mandatory as soon as you
develop commercial activities involving the iText software without
disclosing the source code of your own applications.
These activities include: offering paid services to customers as an ASP,
serving PDFs on the fly in a web application, shipping iText with a closed
source product.

For more information, please contact iText Software Corp. at this
address: sales@itextpdf.com
*/
using System;
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
            properties = new SvgConverterProperties().SetBaseUri(sourceFolder);
            ConvertAndCompareSinglePage(sourceFolder, destinationFolder, "clipPathCssProperty", properties);
        }

        [NUnit.Framework.Test]
        public virtual void ClipPathRulesCombined() {
            //TODO: update after DEVSIX-2377
            ConvertAndCompareSinglePage(sourceFolder, destinationFolder, "clipPathRulesCombined");
        }

        [NUnit.Framework.Test]
        public virtual void InvalidClipPathTagTest() {
            // TODO: DEVSIX-3923 update cmp_ after fix
            ConvertAndCompareSinglePage(sourceFolder, destinationFolder, "clippath_invalid_tag");
        }
    }
}
