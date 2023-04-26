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
using System.IO;
using iText.Commons.Utils;
using iText.Test;

namespace iText.IO.Source {
    [NUnit.Framework.Category("UnitTest")]
    public class RAFRandomAccessSourceTest : ExtendedITextTest {
        private static readonly String SOURCE_FILE = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/io/source/RAF.txt";

        private readonly byte[] content = "Hello, world!".GetBytes();

        [NUnit.Framework.Test]
        public virtual void GetByIndexTest() {
            FileInfo file = new FileInfo(SOURCE_FILE);
            using (FileStream raf = FileUtil.GetRandomAccessFile(file)) {
                RAFRandomAccessSource source = new RAFRandomAccessSource(raf);
                for (int i = 0; i < content.Length; i++) {
                    NUnit.Framework.Assert.AreEqual(content[i], source.Get(i));
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void GetByIndexOutOfBoundsTest() {
            FileInfo file = new FileInfo(SOURCE_FILE);
            int indexOutOfBounds = content.Length;
            using (FileStream raf = FileUtil.GetRandomAccessFile(file)) {
                RAFRandomAccessSource source = new RAFRandomAccessSource(raf);
                NUnit.Framework.Assert.AreNotEqual(-1, source.Get(indexOutOfBounds - 1));
                NUnit.Framework.Assert.AreEqual(-1, source.Get(indexOutOfBounds));
            }
        }

        [NUnit.Framework.Test]
        public virtual void GetArrayByIndexesTest() {
            FileInfo file = new FileInfo(SOURCE_FILE);
            int beginIndex = 7;
            int length = 5;
            using (FileStream raf = FileUtil.GetRandomAccessFile(file)) {
                RAFRandomAccessSource source = new RAFRandomAccessSource(raf);
                byte[] dest = new byte[24];
                int read = source.Get(beginIndex, dest, 0, length);
                NUnit.Framework.Assert.AreEqual(length, read);
                for (int i = 0; i < length; i++) {
                    NUnit.Framework.Assert.AreEqual(content[beginIndex + i], dest[i]);
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void GetArrayByIndexesNotEnoughBytesTest() {
            FileInfo file = new FileInfo(SOURCE_FILE);
            int beginIndex = 7;
            int length = 24;
            int expectedLength = 6;
            using (FileStream raf = FileUtil.GetRandomAccessFile(file)) {
                RAFRandomAccessSource source = new RAFRandomAccessSource(raf);
                byte[] dest = new byte[24];
                int read = source.Get(beginIndex, dest, 0, length);
                NUnit.Framework.Assert.AreEqual(expectedLength, read);
                for (int i = 0; i < expectedLength; i++) {
                    NUnit.Framework.Assert.AreEqual(content[beginIndex + i], dest[i]);
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void GetArrayByIndexesWithOffsetTest() {
            FileInfo file = new FileInfo(SOURCE_FILE);
            int beginIndex = 7;
            int length = 5;
            int offset = 2;
            using (FileStream raf = FileUtil.GetRandomAccessFile(file)) {
                RAFRandomAccessSource source = new RAFRandomAccessSource(raf);
                byte[] dest = new byte[24];
                int read = source.Get(beginIndex, dest, offset, length);
                NUnit.Framework.Assert.AreEqual(length, read);
                for (int i = 0; i < length; i++) {
                    NUnit.Framework.Assert.AreEqual(content[beginIndex + i], dest[offset + i]);
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void GetArrayByIndexesOutOfBounds() {
            FileInfo file = new FileInfo(SOURCE_FILE);
            int beginIndex = content.Length;
            int length = 5;
            using (FileStream raf = FileUtil.GetRandomAccessFile(file)) {
                RAFRandomAccessSource source = new RAFRandomAccessSource(raf);
                byte[] dest = new byte[24];
                int read = source.Get(beginIndex, dest, 0, length);
                NUnit.Framework.Assert.AreEqual(-1, read);
                for (int i = 0; i < dest.Length; i++) {
                    NUnit.Framework.Assert.AreEqual(0, dest[i]);
                }
            }
        }
    }
}
