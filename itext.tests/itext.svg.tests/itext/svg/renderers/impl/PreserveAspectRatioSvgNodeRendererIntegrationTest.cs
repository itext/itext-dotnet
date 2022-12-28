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
using iText.Svg.Renderers;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Svg.Renderers.Impl {
    [NUnit.Framework.Category("IntegrationTest")]
    public class PreserveAspectRatioSvgNodeRendererIntegrationTest : SvgIntegrationTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/svg/renderers/impl/PreserveAspectRatioSvgNodeRendererIntegrationTest/";

        private static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/svg/renderers/impl/PreserveAspectRatioSvgNodeRendererIntegrationTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            ITextTest.CreateDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void AspectRatioPreservationMidXMidYMeetMinimalTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "MidXMidYMeetMinimalTest");
        }

        [NUnit.Framework.Test]
        public virtual void ViewBoxScalingTestPreserveAspectDefaultAll() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "PreserveAspectDefaultAll");
        }

        [NUnit.Framework.Test]
        public virtual void ViewBoxWithoutSetPreserveAspectRatioTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "PreserveAspectRatio");
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.INVALID_CSS_PROPERTY_DECLARATION, 
            Count = 19)]
        public virtual void DifferentAspectRatiosTest() {
            //TODO: update cmp_ when DEVSIX-2250 fixed
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "differentAspectRatios");
        }

        [NUnit.Framework.Test]
        public virtual void ViewBoxScalingTestPreserveAspectDefaultAllGroup() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "PreserveAspectDefaultAllGroup");
        }

        [NUnit.Framework.Test]
        public virtual void ViewBoxScalingTestDoNotPreserveAspectMin() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "DoNotPreserveAspectMin");
        }

        [NUnit.Framework.Test]
        public virtual void ViewBoxScalingTestDoNotPreserveAspectAll() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "DoNotPreserveAspectAll");
        }

        [NUnit.Framework.Test]
        public virtual void ViewBoxScalingTestDoNotPreserveAspectMetricDimensionsMin() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "DoNotPreserveAspectMetricDimensionsMin");
        }

        [NUnit.Framework.Test]
        public virtual void ViewBoxScalingTestDoNotPreserveAspectMetricDimensionsAll() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "DoNotPreserveAspectMetricDimensionsAll");
        }

        [NUnit.Framework.Test]
        public virtual void ViewBoxScalingTestPreserveAspectRatioXMinYMinMeetScaling() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "PreserveAspectRatioXMinYMinMeetScaling");
        }

        [NUnit.Framework.Test]
        public virtual void ViewBoxScalingTestPreserveAspectRatioXMinYMidMeetScaling() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "PreserveAspectRatioXMinYMidMeetScaling");
        }

        [NUnit.Framework.Test]
        public virtual void ViewBoxScalingTestPreserveAspectRatioXMinYMaxMeetScaling() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "PreserveAspectRatioXMinYMaxMeetScaling");
        }

        [NUnit.Framework.Test]
        public virtual void ViewBoxScalingTestPreserveAspectRatioXMidYMinMeetScaling() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "PreserveAspectRatioXMidYMinMeetScaling");
        }

        [NUnit.Framework.Test]
        public virtual void ViewBoxScalingTestPreserveAspectRatioXMidYMaxMeetScaling() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "PreserveAspectRatioXMidYMaxMeetScaling");
        }

        [NUnit.Framework.Test]
        public virtual void ViewBoxScalingTestPreserveAspectRatioXMaxYMinMeetScaling() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "PreserveAspectRatioXMaxYMinMeetScaling");
        }

        [NUnit.Framework.Test]
        public virtual void ViewBoxScalingTestPreserveAspectRatioXMaxYMidMeetScaling() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "PreserveAspectRatioXMaxYMidMeetScaling");
        }

        [NUnit.Framework.Test]
        public virtual void ViewBoxTranslationTestInnerZeroCoordinatesViewBox() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "InnerZeroCoordinatesViewBox");
        }

        [NUnit.Framework.Test]
        public virtual void ViewBoxTranslationTestOuterZeroCoordinatesViewBox() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "OuterZeroCoordinatesViewBox");
        }

        [NUnit.Framework.Test]
        public virtual void ViewBoxTranslationTestMultipleViewBoxes() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "MultipleViewBoxes");
        }

        [NUnit.Framework.Test]
        public virtual void SvgTranslationYMinMeetTest() {
            //TODO (DEVSIX-3537) change cmp files after the ticket will be fixed 
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "svgTranslationYMinMeet");
        }

        [NUnit.Framework.Test]
        public virtual void SvgTranslationYMidMeetTest() {
            //TODO (DEVSIX-3537) change cmp files after the ticket will be fixed
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "svgTranslationYMidMeet");
        }

        [NUnit.Framework.Test]
        public virtual void SvgTranslationYMaxMeetTest() {
            //TODO (DEVSIX-3537) change cmp files after the ticket will be fixed
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "svgTranslationYMaxMeet");
        }

        [NUnit.Framework.Test]
        public virtual void SvgTranslationXMinMeetTest() {
            //TODO (DEVSIX-3537) change cmp files after the ticket will be fixed
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "svgTranslationXMinMeet");
        }

        [NUnit.Framework.Test]
        public virtual void SvgTranslationXMidMeetTest() {
            //TODO (DEVSIX-3537) change cmp files after the ticket will be fixed
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "svgTranslationXMidMeet");
        }

        [NUnit.Framework.Test]
        public virtual void SvgTranslationXMaxMeetTest() {
            //TODO (DEVSIX-3537) change cmp files after the ticket will be fixed
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "svgTranslationXMaxMeet");
        }
    }
}
