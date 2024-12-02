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
using iText.IO.Font;
using iText.Layout.Font;
using iText.StyledXmlParser.Css.Media;
using iText.StyledXmlParser.Resolver.Font;
using iText.Svg.Processors.Impl;
using iText.Svg.Renderers;
using iText.Test;

namespace iText.Svg.Css {
    [NUnit.Framework.Category("IntegrationTest")]
    public class FontAttributesTest : SvgIntegrationTest {
        public static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/svg/css/FontAttributesTest/";

        public static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/svg/css/FontAttributesTest/";

        public static readonly String FONTS_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/svg/css/fonts/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            ITextTest.CreateDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void LighterBolderFontWeightTest() {
            //TODO DEVSIX-8764: update cmp file after supporting
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "font-weight-lighter-bolder");
        }

        [NUnit.Framework.Test]
        public virtual void FontSizeAdjustTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "font-size-adjust");
        }

        [NUnit.Framework.Test]
        public virtual void FontStretchTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "font-stretch");
        }

        [NUnit.Framework.Test]
        public virtual void FontVariantTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "font-variant");
        }

        [NUnit.Framework.Test]
        public virtual void LengthAdjustTest() {
            //TODO DEVSIX-2507: update cmp after supporting
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "length-adjust");
        }

        [NUnit.Framework.Test]
        public virtual void LetterWordSpacingTest() {
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "letter-word-spacing");
        }

        [NUnit.Framework.Test]
        public virtual void UnicodeBidiTest() {
            // Set up font provider
            String fontPath = FONTS_FOLDER + "NotoSansArabic-Regular.ttf";
            FontProvider fontProvider = new FontProvider();
            fontProvider.AddFont(FontProgramFactory.CreateFont(fontPath));
            SvgConverterProperties properties = new SvgConverterProperties().SetBaseUri(null).SetFontProvider(new BasicFontProvider
                ()).SetMediaDeviceDescription(new MediaDeviceDescription(MediaType.ALL));
            properties.SetFontProvider(fontProvider);
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "unicode-bidi", properties);
        }

        [NUnit.Framework.Test]
        public virtual void WritingModeTest() {
            //TODO DEVSIX-4114: update cmp file after supporting
            ConvertAndCompareSinglePage(SOURCE_FOLDER, DESTINATION_FOLDER, "writing-mode");
        }
    }
}
