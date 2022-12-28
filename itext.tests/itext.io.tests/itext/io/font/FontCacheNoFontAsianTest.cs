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
using System;
using iText.IO.Font.Otf;
using iText.Test;

namespace iText.IO.Font {
    [NUnit.Framework.Category("UnitTest")]
    public class FontCacheNoFontAsianTest : ExtendedITextTest {
        [NUnit.Framework.SetUp]
        public virtual void Before() {
            FontCache.ClearSavedFonts();
        }

        [NUnit.Framework.Test]
        public virtual void ClearFontCacheTest() {
            String fontName = "FreeSans.ttf";
            NUnit.Framework.Assert.IsNull(FontCache.GetFont(fontName));
            FontProgram fontProgram = new FontCacheNoFontAsianTest.FontProgramMock();
            FontCache.SaveFont(fontProgram, fontName);
            NUnit.Framework.Assert.AreEqual(fontProgram, FontCache.GetFont(fontName));
            FontCache.ClearSavedFonts();
            NUnit.Framework.Assert.IsNull(FontCache.GetFont(fontName));
        }

        [NUnit.Framework.Test]
        public virtual void FontStringTtcCacheKeyTest() {
            String fontName = "Font.ttc";
            FontCacheKey ttc0 = FontCacheKey.Create(fontName, 0);
            FontCacheKey ttc1 = FontCacheKey.Create(fontName, 1);
            NUnit.Framework.Assert.IsNull(FontCache.GetFont(ttc0));
            NUnit.Framework.Assert.IsNull(FontCache.GetFont(ttc1));
            FontProgram fontProgram = new FontCacheNoFontAsianTest.FontProgramMock();
            FontCache.SaveFont(fontProgram, ttc1);
            NUnit.Framework.Assert.IsNull(FontCache.GetFont(ttc0));
            NUnit.Framework.Assert.AreEqual(fontProgram, FontCache.GetFont(ttc1));
        }

        [NUnit.Framework.Test]
        public virtual void FontBytesTtcCacheKeyTest() {
            byte[] fontBytes = "SupposedTtcFontData".GetBytes(System.Text.Encoding.UTF8);
            byte[] otherFontBytes = "DifferentTtcFontBytes".GetBytes(System.Text.Encoding.UTF8);
            byte[] normalFontBytes = "NormalFontBytes".GetBytes(System.Text.Encoding.UTF8);
            FontCacheKey ttc0 = FontCacheKey.Create(fontBytes, 1);
            FontCacheKey otherTtc0 = FontCacheKey.Create(otherFontBytes, 1);
            FontCacheKey normal = FontCacheKey.Create(normalFontBytes);
            NUnit.Framework.Assert.IsNull(FontCache.GetFont(ttc0));
            NUnit.Framework.Assert.IsNull(FontCache.GetFont(otherTtc0));
            NUnit.Framework.Assert.IsNull(FontCache.GetFont(normal));
            FontProgram otherTtc0MockFontProgram = new FontCacheNoFontAsianTest.FontProgramMock();
            FontProgram normalMockFontProgram = new FontCacheNoFontAsianTest.FontProgramMock();
            FontCache.SaveFont(otherTtc0MockFontProgram, otherTtc0);
            FontCache.SaveFont(normalMockFontProgram, normal);
            NUnit.Framework.Assert.IsNull(FontCache.GetFont(ttc0));
            NUnit.Framework.Assert.AreEqual(otherTtc0MockFontProgram, FontCache.GetFont(otherTtc0));
            NUnit.Framework.Assert.AreEqual(normalMockFontProgram, FontCache.GetFont(normal));
        }

        [NUnit.Framework.Test]
        public virtual void GetCompatibleCidFontNoFontAsian() {
            // Without font-asian module in the class path
            // any value passed into a method is expected to return null.
            NUnit.Framework.Assert.IsNull(FontCache.GetCompatibleCidFont("78-RKSJ-V"));
        }

        [NUnit.Framework.Test]
        public virtual void IsPredefinedCidFontNoFontAsian() {
            // Without font-asian module in the class path
            // any value passed into a method is expected to return false.
            NUnit.Framework.Assert.IsFalse(FontCache.IsPredefinedCidFont("78-RKSJ-V"));
        }

        [NUnit.Framework.Test]
        public virtual void GetCompatibleCmapsNoFontAsian() {
            // Without font-asian module in the class path
            // any value passed into a method is expected to return null.
            NUnit.Framework.Assert.IsNull(FontCache.GetCompatibleCmaps("HeiseiKakuGo-W5"));
        }

        [NUnit.Framework.Test]
        public virtual void GetRegistryNamesNoFontAsian() {
            // Without font-asian module in the class path
            // registry names collection is expected to be empty.
            NUnit.Framework.Assert.IsTrue(FontCache.GetRegistryNames().IsEmpty());
        }

        [NUnit.Framework.Test]
        public virtual void GetCid2UniCMapNoFontAsian() {
            // Without font-asian module in the class path
            // no CMap can be found.
            NUnit.Framework.Assert.Catch(typeof(iText.IO.Exceptions.IOException), () => FontCache.GetCid2UniCmap("UniJIS-UTF16-H"
                ));
        }

        [NUnit.Framework.Test]
        public virtual void GetUni2CidCMapNoFontAsian() {
            // Without font-asian module in the class path
            // no CMap can be found.
            NUnit.Framework.Assert.Catch(typeof(iText.IO.Exceptions.IOException), () => FontCache.GetUni2CidCmap("UniJIS-UTF16-H"
                ));
        }

        [NUnit.Framework.Test]
        public virtual void GetByte2CidCMapNoFontAsian() {
            // Without font-asian module in the class path
            // no CMap can be found.
            NUnit.Framework.Assert.Catch(typeof(iText.IO.Exceptions.IOException), () => FontCache.GetByte2CidCmap("78ms-RKSJ-H"
                ));
        }

        [NUnit.Framework.Test]
        public virtual void GetCid2ByteCMapNoFontAsian() {
            // Without font-asian module in the class path
            // no CMap can be found.
            NUnit.Framework.Assert.Catch(typeof(iText.IO.Exceptions.IOException), () => FontCache.GetCid2Byte("78ms-RKSJ-H"
                ));
        }

        private class FontProgramMock : FontProgram {
            public override int GetPdfFontFlags() {
                return 0;
            }

            public override int GetKerning(Glyph first, Glyph second) {
                return 0;
            }
        }
    }
}
