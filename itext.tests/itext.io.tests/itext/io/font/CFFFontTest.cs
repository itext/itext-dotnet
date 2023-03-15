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
using iText.IO.Source;
using iText.Test;

namespace iText.IO.Font {
    [NUnit.Framework.Category("UnitTest")]
    public class CFFFontTest : ExtendedITextTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/io/font/sharedFontsResourceFiles/";

        [NUnit.Framework.Test]
        public virtual void SeekTest() {
            RandomAccessFileOrArray raf = new RandomAccessFileOrArray(new RandomAccessSourceFactory().CreateBestSource
                (SOURCE_FOLDER + "NotoSansCJKjp-Bold.otf"));
            int offsetToCff = 259880;
            int cffLength = 16023217;
            byte[] cff = new byte[cffLength];
            try {
                raf.Seek(offsetToCff);
                raf.ReadFully(cff);
            }
            finally {
                raf.Close();
            }
            CFFFont cffFont = new CFFFont(cff);
            cffFont.Seek(0);
            // Get int (bin 0000 0001 0000 0000  0000 0100 0000 0011)
            NUnit.Framework.Assert.AreEqual(16778243, cffFont.GetInt());
            cffFont.Seek(0);
            // Gets the first short (bin 0000 0001 0000 0000)
            NUnit.Framework.Assert.AreEqual(256, cffFont.GetShort());
            cffFont.Seek(2);
            // Gets the second short (bin 0000 0100 0000 0011)
            NUnit.Framework.Assert.AreEqual(1027, cffFont.GetShort());
        }

        [NUnit.Framework.Test]
        public virtual void GetPositionTest() {
            RandomAccessFileOrArray raf = new RandomAccessFileOrArray(new RandomAccessSourceFactory().CreateBestSource
                (SOURCE_FOLDER + "NotoSansCJKjp-Bold.otf"));
            int offsetToCff = 259880;
            int cffLength = 16023217;
            byte[] cff = new byte[cffLength];
            try {
                raf.Seek(offsetToCff);
                raf.ReadFully(cff);
            }
            finally {
                raf.Close();
            }
            CFFFont cffFont = new CFFFont(cff);
            cffFont.Seek(0);
            NUnit.Framework.Assert.AreEqual(0, cffFont.GetPosition());
            cffFont.Seek(16);
            NUnit.Framework.Assert.AreEqual(16, cffFont.GetPosition());
        }
    }
}
