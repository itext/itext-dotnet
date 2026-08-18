/*
This file is part of the iText (R) project.
Copyright (c) 1998-2026 Apryse Group NV
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
using iText.Svg.Exceptions;
using iText.Svg.Logs;
using iText.Svg.Renderers;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Svg.Renderers.Impl {
    [NUnit.Framework.Category("IntegrationTest")]
    public class PathParsingIntegrationTest : SvgIntegrationTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/svg/renderers/impl/PathParsingIntegrationTest/";

        private static readonly String DESTINATION_FOLDER = TestUtil.GetOutputPath() + "/svg/renderers/impl/PathParsingIntegrationTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            ITextTest.CreateDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void NormalTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "normal");
        }

        [NUnit.Framework.Test]
        public virtual void MixTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "mix");
        }

        [NUnit.Framework.Test]
        public virtual void NoWhitespace() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "noWhitespace");
        }

        [NUnit.Framework.Test]
        public virtual void ZOperator() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "zOperator");
        }

        [NUnit.Framework.Test]
        public virtual void MissingOperandArgument() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "missingOperandArgument");
        }

        [NUnit.Framework.Test]
        public virtual void DecimalPointHandlingTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "decimalPointHandling");
        }

        [NUnit.Framework.Test]
        public virtual void InvalidOperatorTest() {
            NUnit.Framework.Assert.Catch(typeof(SvgProcessingException), () => ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER
                , "invalidOperator"));
        }

        [NUnit.Framework.Test]
        public virtual void InvalidOperatorCSensTest() {
            NUnit.Framework.Assert.Catch(typeof(SvgProcessingException), () => ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER
                , "invalidOperatorCSens"));
        }

        [NUnit.Framework.Test]
        public virtual void MoreThanOneHParam() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "moreThanOneHParam");
        }

        [NUnit.Framework.Test]
        public virtual void NegativeAfterPositiveHandlingTest01() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "negativeAfterPositiveHandling");
        }

        [NUnit.Framework.Test]
        public virtual void NegativeAfterPositiveHandlingTest02() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "negativeAfterPositiveHandlingExtendedViewbox");
        }

        [NUnit.Framework.Test]
        public virtual void InsignificantSpacesTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "insignificantSpaces");
        }

        [NUnit.Framework.Test]
        public virtual void PrecedingSpacesTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "precedingSpaces");
        }

        [NUnit.Framework.Test]
        [LogMessage(SvgLogMessageConstant.UNMAPPED_TAG)]
        public virtual void TextPathTest() {
            //TODO: update cmp-file after DEVSIX-2255
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "textpath");
        }

        [NUnit.Framework.Test]
        [LogMessage(SvgLogMessageConstant.UNMAPPED_TAG)]
        public virtual void TextPathExample() {
            //TODO: update when DEVSIX-2255 implemented
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "textPathExample");
        }

        [NUnit.Framework.Test]
        [LogMessage(SvgLogMessageConstant.UNMAPPED_TAG)]
        public virtual void TspanInTextPathTest() {
            //TODO: update when DEVSIX-2255 implemented
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "tspanInTextPath");
        }

        [NUnit.Framework.Test]
        public virtual void PathH() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "pathH");
        }

        [NUnit.Framework.Test]
        public virtual void PathV() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "pathV");
        }

        [NUnit.Framework.Test]
        public virtual void PathHV() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "pathHV");
        }

        [NUnit.Framework.Test]
        public virtual void PathRelativeAbsoluteCombinedTest() {
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "pathRelativeAbsoluteCombined");
        }

        [NUnit.Framework.Test]
        public virtual void PathHVExponential() {
            // TODO DEVSIX-2906 This file has large numbers (2e+10) in it. At the moment we do not post-process such big numbers
            // and simply print them to the output PDF. Not all the viewers are able to process such large numbers
            // and hence different results in different viewers. Acrobat is not able to process the numbers
            // and the result is garbled visual representation. GhostScript, however, renders the PDF just fine
            ConvertAndCompare(SOURCE_FOLDER, DESTINATION_FOLDER, "pathHVExponential");
        }

        [NUnit.Framework.Test]
        public virtual void PathABasic() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "pathABasic");
        }

        [NUnit.Framework.Test]
        public virtual void PathAFlags() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "pathAFlags");
        }

        [NUnit.Framework.Test]
        public virtual void PathAAxisRotation() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "pathAAxisRotation");
        }

        [NUnit.Framework.Test]
        public virtual void PathAOutOfRange() {
            //TODO: update cmp when DEVSIX-3010 and DEVSIX-3011 fixed
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "pathAOutOfRange");
        }

        [NUnit.Framework.Test]
        public virtual void Arcs_end_point() {
            //TODO: update cmp when DEVSIX-3010 fixed
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "arcsEndPoint");
        }

        [NUnit.Framework.Test]
        public virtual void Flags_out_of_range() {
            //TODO: update cmp when DEVSIX-3011 fixed
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "flags_out_of_range");
        }
    }
}
