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
using System.Collections.Generic;
using iText.IO.Font.Constants;
using iText.Layout.Font;

namespace iText.Layout.Font.Selectorstrategy {
    public class FontSelectorTestsUtil {
        private static readonly String FONTS_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/fonts/";

        public static IFontSelectorStrategy CreateStrategyWithFreeSansAndTNR(IFontSelectorStrategyFactory factory) {
            FontSet fs = new FontSet();
            fs.AddFont(StandardFonts.TIMES_ROMAN);
            fs.AddFont(FONTS_FOLDER + "FreeSans.ttf");
            FontProvider fontProvider = new FontProvider(fs);
            fontProvider.SetFontSelectorStrategyFactory(factory);
            IList<String> fontFamilies = new List<String>();
            fontFamilies.Add("random");
            return fontProvider.CreateFontSelectorStrategy(fontFamilies, new FontCharacteristics(), null);
        }

        public static IFontSelectorStrategy CreateStrategyWithNotoSans(IFontSelectorStrategyFactory factory) {
            FontSet fs = new FontSet();
            fs.AddFont(FONTS_FOLDER + "NotoKufiArabic-Regular.ttf");
            fs.AddFont(FONTS_FOLDER + "NotoSansCJKjp-Regular.otf");
            fs.AddFont(FONTS_FOLDER + "NotoSansCherokee-Regular.ttf");
            FontProvider fontProvider = new FontProvider(fs, StandardFontFamilies.TIMES);
            fontProvider.SetFontSelectorStrategyFactory(factory);
            IList<String> fontFamilies = new List<String>();
            fontFamilies.Add("random");
            return fontProvider.CreateFontSelectorStrategy(fontFamilies, new FontCharacteristics(), null);
        }

        public static IFontSelectorStrategy CreateStrategyWithTNR(IFontSelectorStrategyFactory factory) {
            FontSet fs = new FontSet();
            fs.AddFont(StandardFonts.TIMES_ROMAN);
            FontProvider fontProvider = new FontProvider(fs);
            fontProvider.SetFontSelectorStrategyFactory(factory);
            IList<String> fontFamilies = new List<String>();
            fontFamilies.Add("random");
            return fontProvider.CreateFontSelectorStrategy(fontFamilies, new FontCharacteristics(), null);
        }

        public static IFontSelectorStrategy CreateStrategyWithFreeSans(IFontSelectorStrategyFactory factory) {
            FontSet fs = new FontSet();
            fs.AddFont(FONTS_FOLDER + "FreeSans.ttf");
            FontProvider fontProvider = new FontProvider(fs);
            fontProvider.SetFontSelectorStrategyFactory(factory);
            IList<String> fontFamilies = new List<String>();
            fontFamilies.Add("random");
            return fontProvider.CreateFontSelectorStrategy(fontFamilies, new FontCharacteristics(), null);
        }

        public static IFontSelectorStrategy CreateStrategyWithLimitedThreeFonts(IFontSelectorStrategyFactory factory
            ) {
            FontProvider fontProvider = new FontProvider();
            // 'a', 'b' and 'c' are in that interval
            fontProvider.GetFontSet().AddFont(FONTS_FOLDER + "NotoSansCJKjp-Bold.otf", null, "FontAlias", new RangeBuilder
                (97, 99).Create());
            // 'd', 'e' and 'f' are in that interval
            fontProvider.GetFontSet().AddFont(FONTS_FOLDER + "FreeSans.ttf", null, "FontAlias", new RangeBuilder(100, 
                102).Create());
            // 'x', 'y' and 'z' are in that interval
            fontProvider.GetFontSet().AddFont(FONTS_FOLDER + "Puritan2.otf", null, "FontAlias", new RangeBuilder(120, 
                122).Create());
            fontProvider.SetFontSelectorStrategyFactory(factory);
            IList<String> fontFamilies = new List<String>();
            fontFamilies.Add("random");
            return fontProvider.CreateFontSelectorStrategy(fontFamilies, new FontCharacteristics(), null);
        }

        public static IFontSelectorStrategy CreateStrategyWithOldItalic(IFontSelectorStrategyFactory factory) {
            FontProvider fontProvider = new FontProvider();
            fontProvider.GetFontSet().AddFont(FONTS_FOLDER + "NotoSansOldItalic-Regular.ttf", null, "FontAlias");
            fontProvider.GetFontSet().AddFont(FONTS_FOLDER + "FreeSans.ttf", null, "FontAlias");
            fontProvider.SetFontSelectorStrategyFactory(factory);
            IList<String> fontFamilies = new List<String>();
            fontFamilies.Add("random");
            return fontProvider.CreateFontSelectorStrategy(fontFamilies, new FontCharacteristics(), null);
        }
    }
}
