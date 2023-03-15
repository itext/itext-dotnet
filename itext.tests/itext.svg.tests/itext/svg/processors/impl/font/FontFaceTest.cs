/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 Apryse Group NV
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
using System.IO;
using iText.Commons.Utils;
using iText.Kernel.Pdf;
using iText.StyledXmlParser.Css.Media;
using iText.StyledXmlParser.Resolver.Font;
using iText.Svg.Logs;
using iText.Svg.Processors;
using iText.Svg.Processors.Impl;
using iText.Svg.Renderers;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Svg.Processors.Impl.Font {
    [NUnit.Framework.Category("IntegrationTest")]
    public class FontFaceTest : SvgIntegrationTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/svg/processors/impl/font/FontFaceTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/svg/processors/impl/font/FontFaceTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            ITextTest.CreateDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void UnicodeRangeTest() {
            // TODO fix cmp file after DEVSIX-2256 is finished. Right now unicode range is not processed correctly
            ConvertAndCompare(sourceFolder, destinationFolder, "unicodeRangeTest");
        }

        [NUnit.Framework.Test]
        public virtual void DroidSerifSingleQuotesTest() {
            // TODO fix cmp file after DEVSIX-2534 is finished. Right now droid fonts are not applied if
            //  their aliases are inside single quotes and contain spaces
            ConvertAndCompare(sourceFolder, destinationFolder, "droidSerifSingleQuotesTest");
        }

        [NUnit.Framework.Test]
        public virtual void DroidSerifWebFontTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "droidSerifWebFontTest");
        }

        [NUnit.Framework.Test]
        public virtual void DroidSerifLocalFontTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "droidSerifLocalFontTest");
        }

        [NUnit.Framework.Test]
        public virtual void DroidSerifLocalLocalFontTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "droidSerifLocalLocalFontTest");
        }

        [NUnit.Framework.Test]
        public virtual void DroidSerifLocalWithMediaFontTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "droidSerifLocalWithMediaFontTest");
        }

        [NUnit.Framework.Test]
        public virtual void DroidSerifLocalWithMediaRuleFontTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "droidSerifLocalWithMediaRuleFontTest");
        }

        [NUnit.Framework.Test]
        public virtual void FontSelectorTest01() {
            ConvertAndCompare(sourceFolder, destinationFolder, "fontSelectorTest01");
        }

        [NUnit.Framework.Test]
        public virtual void FontFaceGrammarTest() {
            ConvertAndCompare(sourceFolder, destinationFolder, "fontFaceGrammarTest");
        }

        [NUnit.Framework.Test]
        public virtual void FontFaceWoffTest01() {
            RunTest("fontFaceWoffTest01");
        }

        [NUnit.Framework.Test]
        public virtual void FontFaceWoffTest02() {
            RunTest("fontFaceWoffTest02");
        }

        [NUnit.Framework.Test]
        [LogMessage(SvgLogMessageConstant.UNABLE_TO_RETRIEVE_FONT)]
        public virtual void FontFaceTtcTest() {
            //TODO (DEVSIX-2064) Cannot retrieve NotoSansCJK-Regular
            RunTest("fontFaceTtcTest");
        }

        [NUnit.Framework.Test]
        public virtual void FontFaceWoff2SimpleTest() {
            RunTest("fontFaceWoff2SimpleTest");
        }

        [NUnit.Framework.Test]
        [LogMessage(SvgLogMessageConstant.UNABLE_TO_RETRIEVE_FONT)]
        public virtual void FontFaceWoff2TtcTest() {
            //TODO (DEVSIX-2064) Cannot retrieve NotoSansCJK-Regular
            RunTest("fontFaceWoff2TtcTest");
        }

        [NUnit.Framework.Test]
        public virtual void W3cProblemTest01() {
            //TODO(DEVSIX-5755): In w3c test suite this font is labeled as invalid though it correctly parsers both in browser and iText
            //See BlocksMetadataPadding001Test in io for decompression details
            RunTest("w3cProblemTest01");
        }

        [NUnit.Framework.Test]
        public virtual void W3cProblemTest02() {
            try {
                RunTest("w3cProblemTest02");
            }
            catch (OverflowException) {
                return;
            }
            NUnit.Framework.Assert.Fail("In w3c test suite this font is labeled as invalid, " + "so the invalid negative value is expected while creating a glyph."
                );
        }

        [NUnit.Framework.Test]
        public virtual void W3cProblemTest03() {
            //TODO(DEVSIX-5756): silently omitted, decompression should fail.
            //See HeaderFlavor001Test in io for decompression details
            RunTest("w3cProblemTest03");
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.FONT_SUBSET_ISSUE)]
        public virtual void W3cProblemTest04() {
            //TODO(DEVSIX-5756): silently omitted, decompression should fail. Browser loads font but don't draw glyph.
            //See HeaderFlavor002Test in io for decompression details
            //NOTE, iText fails on subsetting as expected.
            RunTest("w3cProblemTest04");
        }

        [NUnit.Framework.Test]
        public virtual void W3cProblemTest05() {
            //TODO(DEVSIX-5755): In w3c test suite this font is labeled as invalid though it correctly parsers both in browser and iText
            //See HeaderReserved001Test in io for decompression details
            RunTest("w3cProblemTest05");
        }

        [NUnit.Framework.Test]
        public virtual void W3cProblemTest06() {
            //TODO(DEVSIX-5755): In w3c test suite this font is labeled as invalid though it correctly parsers both in browser and iText
            //See TabledataHmtxTransform003Test in io for decompression details
            RunTest("w3cProblemTest06");
        }

        [NUnit.Framework.Test]
        public virtual void W3cProblemTest07() {
            try {
                RunTest("w3cProblemTest07");
            }
            catch (OverflowException) {
                return;
            }
            NUnit.Framework.Assert.Fail("In w3c test suite this font is labeled as invalid, " + "so the invalid negative value is expected while creating a glyph."
                );
        }

        [NUnit.Framework.Test]
        public virtual void IncorrectFontNameTest01() {
            RunTest("incorrectFontNameTest01");
        }

        [NUnit.Framework.Test]
        public virtual void IncorrectFontNameTest02() {
            // The result of te test is FAIL. However we consider it to be correct.
            // Although the font-family specified by the paragraph's class doesn't match the one of fontface,
            // font's full name contains specified font-family and iText takes it into account.
            RunTest("incorrectFontNameTest02");
        }

        [NUnit.Framework.Test]
        public virtual void IncorrectFontNameTest03() {
            //Checks that font used in previous two files is correct
            RunTest("incorrectFontNameTest03");
        }

        [NUnit.Framework.Test]
        public virtual void IncorrectFontNameTest04() {
            RunTest("incorrectFontNameTest04");
        }

        [NUnit.Framework.Test]
        [NUnit.Framework.Ignore("DEVSIX-1759 - unicode in font family and different result in dotnet")]
        public virtual void FontFamilyTest01() {
            RunTest("fontFamilyTest01");
        }

        [NUnit.Framework.Test]
        public virtual void ResolveFontsWithoutWriterProperties() {
            String fileName = "fontSelectorTest";
            ISvgConverterProperties properties = new SvgConverterProperties().SetFontProvider(new BasicFontProvider())
                .SetMediaDeviceDescription(new MediaDeviceDescription(MediaType.ALL));
            ConvertToSinglePage(new FileInfo(sourceFolder + fileName + ".svg"), new FileInfo(destinationFolder + fileName
                 + ".pdf"), properties);
            Compare(fileName, sourceFolder, destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void ResolveFontsWithoutConverterPropertiesAndWriterProperties() {
            String fileName = "resolveFonts_WithoutConverterPropertiesAndWriterProperties";
            String svgFile = "fontSelectorTest";
            ConvertToSinglePage(new FileInfo(sourceFolder + svgFile + ".svg"), new FileInfo(destinationFolder + fileName
                 + ".pdf"));
            Compare(fileName, sourceFolder, destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void ResolveFontsWithAllProperties() {
            String fileName = "resolveFonts_WithAllProperties";
            String svgFile = "fontSelectorTest";
            WriterProperties writerprops = new WriterProperties().SetCompressionLevel(0);
            String baseUri = FileUtil.GetParentDirectoryUri(new FileInfo(sourceFolder + svgFile + ".svg"));
            ISvgConverterProperties properties = new SvgConverterProperties().SetBaseUri(baseUri).SetFontProvider(new 
                BasicFontProvider()).SetMediaDeviceDescription(new MediaDeviceDescription(MediaType.ALL));
            ConvertToSinglePage(new FileInfo(sourceFolder + svgFile + ".svg"), new FileInfo(destinationFolder + fileName
                 + ".pdf"), properties, writerprops);
            Compare(fileName, sourceFolder, destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void ResolveFontsWithWriterProperties() {
            String fileName = "resolveFonts_WithWriterProperties";
            String svgFile = "fontSelectorTest";
            WriterProperties writerprops = new WriterProperties().SetCompressionLevel(0);
            ConvertToSinglePage(new FileInfo(sourceFolder + svgFile + ".svg"), new FileInfo(destinationFolder + fileName
                 + ".pdf"), writerprops);
            Compare(fileName, sourceFolder, destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void ResolveFontsWithConverterPropsAndWriterProps() {
            String fileName = "resolveFonts_WithConverterPropsAndWriterProps";
            String svgFile = "fontSelectorTest";
            WriterProperties writerprops = new WriterProperties().SetCompressionLevel(0);
            String baseUri = FileUtil.GetParentDirectoryUri(new FileInfo(sourceFolder + svgFile + ".svg"));
            ISvgConverterProperties properties = new SvgConverterProperties().SetBaseUri(baseUri).SetFontProvider(new 
                BasicFontProvider()).SetMediaDeviceDescription(new MediaDeviceDescription(MediaType.ALL));
            ConvertToSinglePage(new FileStream(sourceFolder + svgFile + ".svg", FileMode.Open, FileAccess.Read), new FileStream
                (destinationFolder + fileName + ".pdf", FileMode.Create), properties, writerprops);
            Compare(fileName, sourceFolder, destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void ResolveFontsWithConverterPropertiesAndEmptyUri() {
            String fileName = "resolveFonts_WithConverterPropertiesAndEmptyUri";
            String svgFile = "fontSelectorTest";
            ISvgConverterProperties properties = new SvgConverterProperties().SetBaseUri("").SetFontProvider(new BasicFontProvider
                ()).SetMediaDeviceDescription(new MediaDeviceDescription(MediaType.ALL));
            ConvertToSinglePage(new FileInfo(sourceFolder + svgFile + ".svg"), new FileInfo(destinationFolder + fileName
                 + ".pdf"), properties);
            Compare(fileName, sourceFolder, destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void ResolveFontsWithConverterPropertiesAndNullUri() {
            String fileName = "resolveFonts_WithConverterPropertiesAndNullUri";
            String svgFile = "fontSelectorTest";
            ISvgConverterProperties properties = new SvgConverterProperties().SetBaseUri(null).SetFontProvider(new BasicFontProvider
                ()).SetMediaDeviceDescription(new MediaDeviceDescription(MediaType.ALL));
            ConvertToSinglePage(new FileInfo(sourceFolder + svgFile + ".svg"), new FileInfo(destinationFolder + fileName
                 + ".pdf"), properties);
            Compare(fileName, sourceFolder, destinationFolder);
        }

        [NUnit.Framework.Test]
        public virtual void ResolveFontsDefaultUri() {
            String fileName = "fontSelectorTest02";
            ConvertToSinglePage(new FileInfo(sourceFolder + fileName + ".svg"), new FileInfo(destinationFolder + fileName
                 + ".pdf"));
            Compare(fileName, sourceFolder, destinationFolder);
        }

        private void RunTest(String fileName) {
            Convert(sourceFolder + fileName + ".svg", destinationFolder + fileName + ".pdf");
            Compare(fileName, sourceFolder, destinationFolder);
        }
    }
}
