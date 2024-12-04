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
using System;
using iText.Svg.Exceptions;
using iText.Svg.Logs;
using iText.Svg.Renderers;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Svg.Renderers.Impl {
    [NUnit.Framework.Category("IntegrationTest")]
    public class PathParsingIntegrationTest : SvgIntegrationTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/svg/renderers/impl/PathParsingIntegrationTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/svg/renderers/impl/PathParsingIntegrationTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            ITextTest.CreateDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void NormalTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "normal");
        }

        [NUnit.Framework.Test]
        public virtual void MixTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "mix");
        }

        [NUnit.Framework.Test]
        public virtual void NoWhitespace() {
            ConvertAndCompare(sourceFolder, destinationFolder, "noWhitespace");
        }

        [NUnit.Framework.Test]
        public virtual void ZOperator() {
            ConvertAndCompare(sourceFolder, destinationFolder, "zOperator");
        }

        [NUnit.Framework.Test]
        public virtual void MissingOperandArgument() {
            ConvertAndCompare(sourceFolder, destinationFolder, "missingOperandArgument");
        }

        [NUnit.Framework.Test]
        public virtual void DecimalPointHandlingTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "decimalPointHandling");
        }

        [NUnit.Framework.Test]
        public virtual void InvalidOperatorTest() {
            NUnit.Framework.Assert.Catch(typeof(SvgProcessingException), () => ConvertAndCompare(sourceFolder, destinationFolder
                , "invalidOperator"));
        }

        [NUnit.Framework.Test]
        public virtual void InvalidOperatorCSensTest() {
            NUnit.Framework.Assert.Catch(typeof(SvgProcessingException), () => ConvertAndCompare(sourceFolder, destinationFolder
                , "invalidOperatorCSens"));
        }

        [NUnit.Framework.Test]
        public virtual void MoreThanOneHParam() {
            ConvertAndCompare(sourceFolder, destinationFolder, "moreThanOneHParam");
        }

        [NUnit.Framework.Test]
        public virtual void NegativeAfterPositiveHandlingTest01() {
            ConvertAndCompare(sourceFolder, destinationFolder, "negativeAfterPositiveHandling");
        }

        [NUnit.Framework.Test]
        public virtual void NegativeAfterPositiveHandlingTest02() {
            ConvertAndCompare(sourceFolder, destinationFolder, "negativeAfterPositiveHandlingExtendedViewbox");
        }

        [NUnit.Framework.Test]
        public virtual void InsignificantSpacesTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "insignificantSpaces");
        }

        [NUnit.Framework.Test]
        public virtual void PrecedingSpacesTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "precedingSpaces");
        }

        [NUnit.Framework.Test]
        [LogMessage(SvgLogMessageConstant.UNMAPPED_TAG)]
        public virtual void TextPathTest() {
            //TODO: update cmp-file after DEVSIX-2255
            ConvertAndCompare(sourceFolder, destinationFolder, "textpath");
        }

        [NUnit.Framework.Test]
        [LogMessage(SvgLogMessageConstant.UNMAPPED_TAG)]
        public virtual void TextPathExample() {
            //TODO: update when DEVSIX-2255 implemented
            ConvertAndCompare(sourceFolder, destinationFolder, "textPathExample");
        }

        [NUnit.Framework.Test]
        [LogMessage(SvgLogMessageConstant.UNMAPPED_TAG)]
        public virtual void TspanInTextPathTest() {
            //TODO: update when DEVSIX-2255 implemented
            ConvertAndCompare(sourceFolder, destinationFolder, "tspanInTextPath");
        }

        [NUnit.Framework.Test]
        public virtual void PathH() {
            ConvertAndCompare(sourceFolder, destinationFolder, "pathH");
        }

        [NUnit.Framework.Test]
        public virtual void PathV() {
            ConvertAndCompare(sourceFolder, destinationFolder, "pathV");
        }

        [NUnit.Framework.Test]
        public virtual void PathHV() {
            ConvertAndCompare(sourceFolder, destinationFolder, "pathHV");
        }

        [NUnit.Framework.Test]
        public virtual void PathRelativeAbsoluteCombinedTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "pathRelativeAbsoluteCombined");
        }

        [NUnit.Framework.Test]
        public virtual void PathHVExponential() {
            // TODO DEVSIX-2906 This file has large numbers (2e+10) in it. At the moment we do not post-process such big numbers
            // and simply print them to the output PDF. Not all the viewers are able to process such large numbers
            // and hence different results in different viewers. Acrobat is not able to process the numbers
            // and the result is garbled visual representation. GhostScript, however, renders the PDF just fine
            ConvertAndCompare(sourceFolder, destinationFolder, "pathHVExponential");
        }

        [NUnit.Framework.Test]
        public virtual void PathABasic() {
            ConvertAndCompareSinglePage(sourceFolder, destinationFolder, "pathABasic");
        }

        [NUnit.Framework.Test]
        public virtual void PathAFlags() {
            ConvertAndCompareSinglePage(sourceFolder, destinationFolder, "pathAFlags");
        }

        [NUnit.Framework.Test]
        public virtual void PathAAxisRotation() {
            ConvertAndCompareSinglePage(sourceFolder, destinationFolder, "pathAAxisRotation");
        }

        [NUnit.Framework.Test]
        public virtual void PathAOutOfRange() {
            //TODO: update cmp when DEVSIX-3010 and DEVSIX-3011 fixed
            ConvertAndCompareSinglePage(sourceFolder, destinationFolder, "pathAOutOfRange");
        }

        [NUnit.Framework.Test]
        public virtual void Arcs_end_point() {
            //TODO: update cmp when DEVSIX-3010 fixed
            ConvertAndCompareSinglePage(sourceFolder, destinationFolder, "arcsEndPoint");
        }

        [NUnit.Framework.Test]
        public virtual void Flags_out_of_range() {
            //TODO: update cmp when DEVSIX-3011 fixed
            ConvertAndCompareSinglePage(sourceFolder, destinationFolder, "flags_out_of_range");
        }
    }
}
