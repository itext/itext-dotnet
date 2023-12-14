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
using System.Collections.Generic;
using System.Reflection;
using iText.IO.Font.Constants;
using iText.IO.Util;
using iText.Test;
using NUnit.Framework;

namespace iText.IO.Font {
    public class FontProgramTest : ExtendedITextTest {
        private const String notExistingFont = "some-font.ttf";

        [Test]
        public virtual void ExceptionMessageTest() {
            Exception e = Assert.Catch(typeof(System.IO.IOException), () => FontProgramFactory.CreateFont(notExistingFont));
            Assert.AreEqual(MessageFormatUtil.Format(iText.IO.IOException._1NotFoundAsFileOrResource, notExistingFont), e.Message);
        }

        [Test]
        public virtual void BoldTest() {
            FontProgram fp = FontProgramFactory.CreateFont(StandardFonts.HELVETICA);
            fp.SetBold(true);
            Assert.IsTrue((fp.GetPdfFontFlags() & (1 << 18)) != 0, "Bold expected");
            fp.SetBold(false);
            Assert.IsTrue((fp.GetPdfFontFlags() & (1 << 18)) == 0, "Not Bold expected");
        }

        [Test]
        public virtual void FontCacheTest()
        {
            FontProgramFactory.ClearRegisteredFonts();
            FontProgramFactory.ClearRegisteredFontFamilies();
            int cacheSize = -1;
            try
            {
                FieldInfo f = typeof(FontCache).GetField("fontCache", BindingFlags.NonPublic | BindingFlags.Static);
                IDictionary<FontCacheKey, FontProgram> cachedFonts = (IDictionary<FontCacheKey, FontProgram>)f.GetValue(null);
                cachedFonts.Clear();
                FontProgramFactory.RegisterFontDirectory(TestUtil.GetParentProjectDirectory(TestContext
                                                             .CurrentContext.TestDirectory) + "/resources/itext/io/font/otf/");
                cacheSize = cachedFonts.Count;
            }
            catch (Exception) { }

            foreach (String s in FontProgramFactory.GetRegisteredFonts())
                Console.WriteLine(s);

            Assert.AreEqual(43, FontProgramFactory.GetRegisteredFonts().Count);
            Assert.IsTrue(FontProgramFactory.GetRegisteredFonts().Contains("free sans lihavoitu"));
            Assert.AreEqual(0, cacheSize);
        }

        [Test]
        public void RegisterDirectoryType1Test()
        {
            FontProgramFactory.RegisterFontDirectory(TestUtil.GetParentProjectDirectory(TestContext
                                                         .CurrentContext.TestDirectory) + "/resources/itext/io/font/type1/");
            FontProgram computerModern = FontProgramFactory.CreateRegisteredFont("computer modern");
            FontProgram cmr10 = FontProgramFactory.CreateRegisteredFont("cmr10");
            Assert.NotNull(computerModern);
            Assert.NotNull(cmr10);
        }
    }
}
