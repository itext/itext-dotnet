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
using iText.IO.Font.Cmap;
using iText.Test;

namespace iText.IO.Font {
    [NUnit.Framework.Category("IntegrationTest")]
    public class CjkResourceLoaderTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void GetCompatibleCidFont() {
            String expected = "HeiseiMin-W3";
            String compatibleCidFont = CjkResourceLoader.GetCompatibleCidFont("78-RKSJ-V");
            NUnit.Framework.Assert.AreEqual(expected, compatibleCidFont);
        }

        [NUnit.Framework.Test]
        public virtual void GetCompatibleCmaps() {
            ICollection<String> compatibleCmaps = CjkResourceLoader.GetCompatibleCmaps("HeiseiKakuGo-W5");
            NUnit.Framework.Assert.AreEqual(66, compatibleCmaps.Count);
            NUnit.Framework.Assert.IsTrue(compatibleCmaps.Contains("78-RKSJ-V"));
        }

        [NUnit.Framework.Test]
        public virtual void GetRegistryNames() {
            IDictionary<String, ICollection<String>> registryNames = CjkResourceLoader.GetRegistryNames();
            NUnit.Framework.Assert.AreEqual(9, registryNames.Count);
            NUnit.Framework.Assert.IsTrue(registryNames.ContainsKey("Adobe_Japan1"));
            NUnit.Framework.Assert.IsTrue(registryNames.Get("Adobe_Japan1").Contains("78-RKSJ-V"));
        }

        [NUnit.Framework.Test]
        public virtual void GetCid2UniCMap() {
            CMapCidUni cid2UniCmap = CjkResourceLoader.GetCid2UniCmap("UniJIS-UTF16-H");
            NUnit.Framework.Assert.AreEqual(0x00b5, cid2UniCmap.Lookup(159));
        }

        [NUnit.Framework.Test]
        public virtual void GetUni2CidCMap() {
            CMapCodepointToCid uni2CidCmap = CjkResourceLoader.GetCodepointToCidCmap("UniJIS-UTF16-H");
            NUnit.Framework.Assert.AreEqual(159, uni2CidCmap.Lookup(0x00b5));
        }

        [NUnit.Framework.Test]
        public virtual void GetByte2CidCMap() {
            CMapByteCid byte2CidCmap = CjkResourceLoader.GetByte2CidCmap("78ms-RKSJ-H");
            int byteCode = 0x94e0;
            char cid = (char)7779;
            byte[] byteCodeBytes = new byte[] { (byte)((byteCode & 0xFF00) >> 8), (byte)(byteCode & 0xFF) };
            String actual = byte2CidCmap.DecodeSequence(byteCodeBytes, 0, 2);
            String expected = new String(new char[] { cid });
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void GetCid2ByteCMap() {
            CMapCidToCodepoint cid2Byte = CjkResourceLoader.GetCidToCodepointCmap("78ms-RKSJ-H");
            int byteCode = 0x94e0;
            int cid = 7779;
            byte[] actual = cid2Byte.Lookup(cid);
            byte[] expected = new byte[] { (byte)((byteCode & 0xFF00) >> 8), (byte)(byteCode & 0xFF) };
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }
    }
}
