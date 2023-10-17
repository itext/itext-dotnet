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
using iText.IO.Font.Cmap;
using iText.Test;

namespace iText.IO.Font {
    [NUnit.Framework.Category("UnitTest")]
    public class CjkResourceLoaderNoFontAsianTest : ExtendedITextTest {
        // Android-Conversion-Skip-File (TODO DEVSIX-7376 investigate why CjkResourceLoaderNoFontAsianTest is skipped on Android)
        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            // Here we mimic the absence of font asian
            CjkResourceLoader.SetCmapLocation(new CjkResourceLoaderNoFontAsianTest.DummyCMapLocationResource());
        }

        [NUnit.Framework.OneTimeTearDown]
        public static void AfterClass() {
            CjkResourceLoader.SetCmapLocation(new CMapLocationResource());
        }

        [NUnit.Framework.Test]
        public virtual void GetCompatibleCidFontNoFontAsian() {
            // Without font-asian module in the class path
            // any value passed into a method is expected to return null.
            NUnit.Framework.Assert.IsNull(CjkResourceLoader.GetCompatibleCidFont("78-RKSJ-V"));
        }

        [NUnit.Framework.Test]
        public virtual void IsPredefinedCidFontNoFontAsian() {
            // Without font-asian module in the class path
            // any value passed into a method is expected to return false.
            NUnit.Framework.Assert.IsFalse(CjkResourceLoader.IsPredefinedCidFont("KozMinPro-Regular"));
        }

        [NUnit.Framework.Test]
        public virtual void GetCompatibleCmapsNoFontAsian() {
            // Without font-asian module in the class path
            // any value passed into a method is expected to return null.
            NUnit.Framework.Assert.IsNull(CjkResourceLoader.GetCompatibleCmaps("HeiseiKakuGo-W5"));
        }

        [NUnit.Framework.Test]
        public virtual void GetRegistryNamesNoFontAsian() {
            // Without font-asian module in the class path
            // registry names collection is expected to be empty.
            NUnit.Framework.Assert.IsTrue(CjkResourceLoader.GetRegistryNames().IsEmpty());
        }

        [NUnit.Framework.Test]
        public virtual void GetCid2UniCMapNoFontAsian() {
            // Without font-asian module in the class path
            // no CMap can be found.
            NUnit.Framework.Assert.Catch(typeof(iText.IO.Exceptions.IOException), () => CjkResourceLoader.GetCid2UniCmap
                ("UniJIS-UTF16-H"));
        }

        [NUnit.Framework.Test]
        public virtual void GetUni2CidCMapNoFontAsian() {
            // Without font-asian module in the class path
            // no CMap can be found.
            NUnit.Framework.Assert.Catch(typeof(iText.IO.Exceptions.IOException), () => CjkResourceLoader.GetUni2CidCmap
                ("UniJIS-UTF16-H"));
        }

        [NUnit.Framework.Test]
        public virtual void GetByte2CidCMapNoFontAsian() {
            // Without font-asian module in the class path
            // no CMap can be found.
            NUnit.Framework.Assert.Catch(typeof(iText.IO.Exceptions.IOException), () => CjkResourceLoader.GetByte2CidCmap
                ("78ms-RKSJ-H"));
        }

        [NUnit.Framework.Test]
        public virtual void GetCid2ByteCMapNoFontAsian() {
            // Without font-asian module in the class path
            // no CMap can be found.
            NUnit.Framework.Assert.Catch(typeof(iText.IO.Exceptions.IOException), () => CjkResourceLoader.GetCidToCodepointCmap
                ("78ms-RKSJ-H"));
        }

        private class DummyCMapLocationResource : CMapLocationResource {
            public override String GetLocationPath() {
                return base.GetLocationPath() + "dummy/path/";
            }
        }
    }
}
