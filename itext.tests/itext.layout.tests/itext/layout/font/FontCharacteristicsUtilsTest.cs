/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: iText Software.

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License version 3
as published by the Free Software Foundation with the addition of the
following permission added to Section 15 as permitted in Section 7(a):
FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
OF THIRD PARTY RIGHTS

This program is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU Affero General Public License for more details.
You should have received a copy of the GNU Affero General Public License
along with this program; if not, see http://www.gnu.org/licenses or write to
the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
Boston, MA, 02110-1301 USA, or download the license from the following URL:
http://itextpdf.com/terms-of-use/

The interactive user interfaces in modified source and object code versions
of this program must display Appropriate Legal Notices, as required under
Section 5 of the GNU Affero General Public License.

In accordance with Section 7(b) of the GNU Affero General Public License,
a covered work must retain the producer line in every PDF that is created
or manipulated using iText.

You can be released from the requirements of the license by purchasing
a commercial license. Buying such a license is mandatory as soon as you
develop commercial activities involving the iText software without
disclosing the source code of your own applications.
These activities include: offering paid services to customers as an ASP,
serving PDFs on the fly in a web application, shipping iText with a closed
source product.

For more information, please contact iText Software Corp. at this
address: sales@itextpdf.com
*/
using iText.Test;

namespace iText.Layout.Font {
    [NUnit.Framework.Category("UnitTest")]
    public class FontCharacteristicsUtilsTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void TestNormalizingThinFontWeight() {
            NUnit.Framework.Assert.AreEqual(100, FontCharacteristicsUtils.NormalizeFontWeight((short)-10000));
            NUnit.Framework.Assert.AreEqual(100, FontCharacteristicsUtils.NormalizeFontWeight((short)0));
            NUnit.Framework.Assert.AreEqual(100, FontCharacteristicsUtils.NormalizeFontWeight((short)50));
            NUnit.Framework.Assert.AreEqual(100, FontCharacteristicsUtils.NormalizeFontWeight((short)100));
        }

        [NUnit.Framework.Test]
        public virtual void TestNormalizingHeavyFontWeight() {
            NUnit.Framework.Assert.AreEqual(900, FontCharacteristicsUtils.NormalizeFontWeight((short)900));
            NUnit.Framework.Assert.AreEqual(900, FontCharacteristicsUtils.NormalizeFontWeight((short)1600));
            NUnit.Framework.Assert.AreEqual(900, FontCharacteristicsUtils.NormalizeFontWeight((short)23000));
        }

        [NUnit.Framework.Test]
        public virtual void TestNormalizingNormalFontWeight() {
            NUnit.Framework.Assert.AreEqual(200, FontCharacteristicsUtils.NormalizeFontWeight((short)220));
            NUnit.Framework.Assert.AreEqual(400, FontCharacteristicsUtils.NormalizeFontWeight((short)456));
            NUnit.Framework.Assert.AreEqual(500, FontCharacteristicsUtils.NormalizeFontWeight((short)550));
            NUnit.Framework.Assert.AreEqual(600, FontCharacteristicsUtils.NormalizeFontWeight((short)620));
            NUnit.Framework.Assert.AreEqual(700, FontCharacteristicsUtils.NormalizeFontWeight((short)780));
        }

        [NUnit.Framework.Test]
        public virtual void TestParsingIncorrectFontWeight() {
            NUnit.Framework.Assert.AreEqual((short)-1, FontCharacteristicsUtils.ParseFontWeight(""));
            NUnit.Framework.Assert.AreEqual((short)-1, FontCharacteristicsUtils.ParseFontWeight(null));
            NUnit.Framework.Assert.AreEqual((short)-1, FontCharacteristicsUtils.ParseFontWeight("dfgdgdfgdfgdf"));
            NUnit.Framework.Assert.AreEqual((short)-1, FontCharacteristicsUtils.ParseFontWeight("italic"));
        }

        [NUnit.Framework.Test]
        public virtual void TestParsingNumberFontWeight() {
            NUnit.Framework.Assert.AreEqual((short)100, FontCharacteristicsUtils.ParseFontWeight("-1"));
            NUnit.Framework.Assert.AreEqual((short)100, FontCharacteristicsUtils.ParseFontWeight("50"));
            NUnit.Framework.Assert.AreEqual((short)300, FontCharacteristicsUtils.ParseFontWeight("360"));
            NUnit.Framework.Assert.AreEqual((short)900, FontCharacteristicsUtils.ParseFontWeight("25000"));
        }

        [NUnit.Framework.Test]
        public virtual void TestParseAllowedFontWeight() {
            NUnit.Framework.Assert.AreEqual((short)400, FontCharacteristicsUtils.ParseFontWeight("normal"));
            NUnit.Framework.Assert.AreEqual((short)700, FontCharacteristicsUtils.ParseFontWeight("bold"));
        }
    }
}
