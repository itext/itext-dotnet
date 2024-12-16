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
using System.Collections.Generic;
using iText.Commons.Utils;
using iText.IO.Font.Constants;
using iText.Layout.Font;
using iText.Test.Attributes;

namespace iText.StyledXmlParser.Resolver.Font {
    [NUnit.Framework.Category("UnitTest")]
    public class BasicFontProviderTest {
        [NUnit.Framework.Test]
        public virtual void BasicTest() {
            FontProvider fontProvider = new BasicFontProvider();
            FontSelector fontSelector = fontProvider.GetFontSelector(JavaCollectionsUtil.SingletonList("Helvetica"), new 
                FontCharacteristics(), null);
            FontInfo selectedFont = fontSelector.BestMatch();
            NUnit.Framework.Assert.AreEqual("Helvetica", selectedFont.GetFontName());
        }

        [NUnit.Framework.Test]
        public virtual void BasicTest2() {
            FontProvider fontProvider = new BasicFontProvider(true, true, false);
            FontSelector fontSelector = fontProvider.GetFontSelector(JavaCollectionsUtil.SingletonList("Symbol"), new 
                FontCharacteristics(), null);
            FontInfo selectedFont = fontSelector.BestMatch();
            NUnit.Framework.Assert.AreEqual("Symbol", selectedFont.GetFontName());
        }

        [NUnit.Framework.Test]
        public virtual void BasicTest3() {
            FontProvider fontProvider = new BasicFontProvider(new FontSet(), StandardFontFamilies.TIMES);
            FontSelector fontSelector = fontProvider.GetFontSelector(JavaCollectionsUtil.SingletonList("Times"), new FontCharacteristics
                (), null);
            NUnit.Framework.Assert.Catch(typeof(Exception), () => fontSelector.BestMatch());
        }

        [NUnit.Framework.Test]
        public virtual void ShippedFontTest() {
            FontProvider fontProvider = new BasicFontProviderTest.TestFontProvider();
            //Not checking shipped fonts since that's an empty file in this test,
            //so instead just trying to get "Courier" font to check that nothing breaks
            FontSelector fontSelector = fontProvider.GetFontSelector(JavaCollectionsUtil.SingletonList("Courier"), new 
                FontCharacteristics(), null);
            FontInfo selectedFont = fontSelector.BestMatch();
            NUnit.Framework.Assert.AreEqual("Courier", selectedFont.GetFontName());
        }

        [NUnit.Framework.Test]
        public virtual void SystemFontTest() {
            FontProvider fontProvider = new BasicFontProvider(true, true, true);
            //Not checking system fonts since based on a system running those can be different,
            //so instead just trying to get "Courier" font to check that nothing breaks
            FontSelector fontSelector = fontProvider.GetFontSelector(JavaCollectionsUtil.SingletonList("Courier"), new 
                FontCharacteristics(), null);
            FontInfo selectedFont = fontSelector.BestMatch();
            NUnit.Framework.Assert.AreEqual("Courier", selectedFont.GetFontName());
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.ERROR_LOADING_FONT)]
        public virtual void InvalidShippedFontTest() {
            FontProvider fontProvider = new BasicFontProviderTest.InvalidTestFontProvider();
            FontSelector fontSelector = fontProvider.GetFontSelector(JavaCollectionsUtil.SingletonList("Courier"), new 
                FontCharacteristics(), null);
            FontInfo selectedFont = fontSelector.BestMatch();
            NUnit.Framework.Assert.AreEqual("Courier", selectedFont.GetFontName());
        }

        private class TestFontProvider : BasicFontProvider {
            public TestFontProvider()
                : base() {
            }

            protected internal override void InitShippedFontsResourcePath() {
                //This file is empty since real fonts are pretty large files, we're just checking that addShippedFonts is
                //getting called and doesn't throw log message
                shippedFontResourcePath = "com/itextpdf/styledxmlparser/resolver/font/";
                shippedFontNames = new List<String>();
                shippedFontNames.Add("test.ttf");
            }
        }

        private class InvalidTestFontProvider : BasicFontProvider {
            public InvalidTestFontProvider()
                : base() {
            }

            protected internal override void InitShippedFontsResourcePath() {
                //This file is empty since real fonts are pretty large files, we're just checking that addShippedFonts is
                //getting called and doesn't throw log message
                shippedFontResourcePath = "com/itextpdf/styledxmlparser/resolver/font/";
                shippedFontNames = new List<String>();
                shippedFontNames.Add("noSuchFile.ttf");
            }
        }
    }
}
