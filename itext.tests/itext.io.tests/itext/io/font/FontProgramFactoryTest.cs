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
using System.IO;
using iText.IO.Font.Constants;
using iText.Test;

namespace iText.IO.Font {
    [NUnit.Framework.Category("IntegrationTest")]
    public class FontProgramFactoryTest : ExtendedITextTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/io/font/FontProgramFactoryTest/";

        [NUnit.Framework.Test]
        public virtual void CreateRegisteredFontTest() {
            NUnit.Framework.Assert.IsNull(FontProgramFactory.CreateRegisteredFont(null, FontStyles.NORMAL));
            NUnit.Framework.Assert.IsNotNull(FontProgramFactory.CreateRegisteredFont("helvetica", FontStyles.UNDEFINED
                ));
            NUnit.Framework.Assert.IsNotNull(FontProgramFactory.CreateRegisteredFont("helvetica", FontStyles.BOLD));
            NUnit.Framework.Assert.IsNotNull(FontProgramFactory.CreateRegisteredFont("helvetica", FontStyles.ITALIC));
        }

        [NUnit.Framework.Test]
        public virtual void RegisterFontFamilyTest() {
            FontProgramFactory.RegisterFontFamily("somefont", "somefont", null);
            NUnit.Framework.Assert.IsNull(FontProgramFactory.CreateRegisteredFont("somefont", FontStyles.UNDEFINED));
            FontProgramFactory.RegisterFontFamily("somefont", "somefont regular", null);
            NUnit.Framework.Assert.IsNull(FontProgramFactory.CreateRegisteredFont("somefont", FontStyles.UNDEFINED));
        }

        [NUnit.Framework.Test]
        public virtual void CreateTrueTypeWoffFontTest() {
            byte[] fontBytes = File.ReadAllBytes(System.IO.Path.Combine(SOURCE_FOLDER + "SourceSerif4-Black.woff"));
            TrueTypeFont woffFont = FontProgramFactory.CreateTrueTypeFont(fontBytes, false);
            NUnit.Framework.Assert.IsNotNull(woffFont);
            NUnit.Framework.Assert.AreEqual(1463, woffFont.bBoxes.Length);
        }

        [NUnit.Framework.Test]
        public virtual void TryToCreateTrueTypeWoff2FontTest() {
            byte[] fontBytes = File.ReadAllBytes(System.IO.Path.Combine(SOURCE_FOLDER + "valid.woff2"));
            TrueTypeFont woff2Font = FontProgramFactory.CreateTrueTypeFont(fontBytes, false);
            NUnit.Framework.Assert.IsNotNull(woff2Font);
            NUnit.Framework.Assert.AreEqual(4, woff2Font.CountOfGlyphs());
        }
    }
}
