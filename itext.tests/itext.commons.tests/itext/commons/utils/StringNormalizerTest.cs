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
using iText.Test;

namespace iText.Commons.Utils {
    [NUnit.Framework.Category("UnitTest")]
    public class StringNormalizerTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void ToLowerCaseTest() {
            NUnit.Framework.Assert.IsNull(StringNormalizer.ToLowerCase(null));
            NUnit.Framework.Assert.AreEqual("some string", StringNormalizer.ToLowerCase("SoMe StRiNg"));
            NUnit.Framework.Assert.AreEqual("some string", StringNormalizer.ToLowerCase("SOME STRING"));
        }

        [NUnit.Framework.Test]
        public virtual void ToUpperCaseTest() {
            NUnit.Framework.Assert.IsNull(StringNormalizer.ToUpperCase(null));
            NUnit.Framework.Assert.AreEqual("SOME STRING", StringNormalizer.ToUpperCase("SoMe StRiNg"));
            NUnit.Framework.Assert.AreEqual("SOME STRING", StringNormalizer.ToUpperCase("some string"));
        }

        [NUnit.Framework.Test]
        public virtual void NormalizeTest() {
            NUnit.Framework.Assert.IsNull(StringNormalizer.Normalize(null));
            NUnit.Framework.Assert.AreEqual("some   string", StringNormalizer.Normalize(" \t\nSoMe   StRiNg  "));
            NUnit.Framework.Assert.AreEqual("some   string", StringNormalizer.Normalize(" \t\nSOME   STRING  "));
        }
    }
}
