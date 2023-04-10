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
using iText.Commons.Utils;
using iText.IO.Exceptions;
using iText.IO.Font.Constants;
using iText.Test;

namespace iText.IO.Font {
    [NUnit.Framework.Category("UnitTest")]
    public class FontProgramTest : ExtendedITextTest {
        private const String notExistingFont = "some-font.ttf";

        [NUnit.Framework.Test]
        public virtual void ExceptionMessageTest() {
            Exception e = NUnit.Framework.Assert.Catch(typeof(System.IO.IOException), () => FontProgramFactory.CreateFont
                (notExistingFont));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(IoExceptionMessageConstant.NOT_FOUND_AS_FILE_OR_RESOURCE
                , notExistingFont), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void BoldTest() {
            FontProgram fp = FontProgramFactory.CreateFont(StandardFonts.HELVETICA);
            fp.SetBold(true);
            NUnit.Framework.Assert.IsTrue((fp.GetPdfFontFlags() & (1 << 18)) != 0, "Bold expected");
            fp.SetBold(false);
            NUnit.Framework.Assert.IsTrue((fp.GetPdfFontFlags() & (1 << 18)) == 0, "Not Bold expected");
        }

        [NUnit.Framework.Test]
        public virtual void RegisterDirectoryOpenTypeTest() {
            FontProgramFactory.ClearRegisteredFonts();
            FontProgramFactory.ClearRegisteredFontFamilies();
            FontCache.ClearSavedFonts();
            FontProgramFactory.RegisterFontDirectory(iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
                .CurrentContext.TestDirectory) + "/resources/itext/io/font/otf/");
            NUnit.Framework.Assert.AreEqual(43, FontProgramFactory.GetRegisteredFonts().Count);
            NUnit.Framework.Assert.IsNull(FontCache.GetFont(iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
                .CurrentContext.TestDirectory) + "/resources/itext/io/font/otf/FreeSansBold.ttf"));
            NUnit.Framework.Assert.IsTrue(FontProgramFactory.GetRegisteredFonts().Contains("free sans lihavoitu"));
        }

        [NUnit.Framework.Test]
        public virtual void RegisterDirectoryType1Test() {
            FontProgramFactory.RegisterFontDirectory(iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
                .CurrentContext.TestDirectory) + "/resources/itext/io/font/type1/");
            FontProgram computerModern = FontProgramFactory.CreateRegisteredFont("computer modern");
            FontProgram cmr10 = FontProgramFactory.CreateRegisteredFont("cmr10");
            NUnit.Framework.Assert.IsNotNull(computerModern);
            NUnit.Framework.Assert.IsNotNull(cmr10);
        }
    }
}
