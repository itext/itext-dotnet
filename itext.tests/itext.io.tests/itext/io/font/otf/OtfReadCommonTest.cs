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
using System.Collections.Generic;
using iText.IO.Source;
using iText.Test;

namespace iText.IO.Font.Otf {
    [NUnit.Framework.Category("UnitTest")]
    public class OtfReadCommonTest : ExtendedITextTest {
        private static readonly String RESOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/io/font/otf/OtfReadCommonTest/";

        private static readonly String RESOURCE_FOLDER_2 = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/io/font/otf" + "/GposLookupType7Test/";

        [NUnit.Framework.Test]
        public virtual void TestReadCoverageFormat1() {
            // Based on Example 5 from the specification
            // https://docs.microsoft.com/en-us/typography/opentype/spec/chapter2
            // 0001 0005 0038 003B 0041 1042 A04A
            String path = RESOURCE_FOLDER + "coverage-format-1.bin";
            RandomAccessFileOrArray rf = new RandomAccessFileOrArray(new RandomAccessSourceFactory().CreateBestSource(
                path));
            IList<int> glyphIds = OtfReadCommon.ReadCoverageFormat(rf, 0);
            NUnit.Framework.Assert.AreEqual(5, glyphIds.Count);
            NUnit.Framework.Assert.AreEqual(0x38, (int)glyphIds[0]);
            NUnit.Framework.Assert.AreEqual(0x3B, (int)glyphIds[1]);
            NUnit.Framework.Assert.AreEqual(0x41, (int)glyphIds[2]);
            NUnit.Framework.Assert.AreEqual(0x1042, (int)glyphIds[3]);
            NUnit.Framework.Assert.AreEqual(0xA04A, (int)glyphIds[4]);
        }

        [NUnit.Framework.Test]
        public virtual void TestReadCoverageFormat2() {
            // Based on Example 6 from the specification
            // https://docs.microsoft.com/en-us/typography/opentype/spec/chapter2
            // 0002 0001 A04E A057 0000
            String path = RESOURCE_FOLDER + "coverage-format-2.bin";
            RandomAccessFileOrArray rf = new RandomAccessFileOrArray(new RandomAccessSourceFactory().CreateBestSource(
                path));
            IList<int> glyphIds = OtfReadCommon.ReadCoverageFormat(rf, 0);
            NUnit.Framework.Assert.AreEqual(10, glyphIds.Count);
            NUnit.Framework.Assert.AreEqual(0xA04E, (int)glyphIds[0]);
            NUnit.Framework.Assert.AreEqual(0xA057, (int)glyphIds[9]);
        }

        [NUnit.Framework.Test]
        public virtual void TestConversionGlyphToTextSpace() {
            OtfReadCommonTest.OpenTypeFontTableReaderTest gposTableReader = new OtfReadCommonTest.OpenTypeFontTableReaderTest
                (this, new OtfReadCommonTest.RandomAccesArrayTest(this, null), 0, null, null, 1);
            // at 15 we fill up all values
            GposValueRecord valueRecord = OtfReadCommon.ReadGposValueRecord(gposTableReader, 15);
            NUnit.Framework.Assert.AreEqual(2000, valueRecord.GetXAdvance());
            NUnit.Framework.Assert.AreEqual(2000, valueRecord.GetXPlacement());
            NUnit.Framework.Assert.AreEqual(2000, valueRecord.GetYAdvance());
            NUnit.Framework.Assert.AreEqual(2000, valueRecord.GetYPlacement());
        }

//\cond DO_NOT_DOCUMENT
        internal class OpenTypeFontTableReaderTest : OpenTypeFontTableReader {
            protected internal OpenTypeFontTableReaderTest(OtfReadCommonTest _enclosing, RandomAccessFileOrArray rf, int
                 tableLocation, OpenTypeGdefTableReader gdef, IDictionary<int, Glyph> indexGlyphMap, int unitsPerEm)
                : base(rf, tableLocation, gdef, indexGlyphMap, unitsPerEm) {
                this._enclosing = _enclosing;
            }

            protected internal override OpenTableLookup ReadLookupTable(int lookupType, int lookupFlag, int[] subTableLocations
                ) {
                return null;
            }

            private readonly OtfReadCommonTest _enclosing;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal class RandomAccesArrayTest : RandomAccessFileOrArray {
            /// <summary>Creates a RandomAccessFileOrArray that wraps the specified byte source.</summary>
            /// <remarks>
            /// Creates a RandomAccessFileOrArray that wraps the specified byte source.  The byte source will be closed when
            /// this RandomAccessFileOrArray is closed.
            /// </remarks>
            /// <param name="byteSource">the byte source to wrap</param>
            public RandomAccesArrayTest(OtfReadCommonTest _enclosing, IRandomAccessSource byteSource)
                : base(byteSource) {
                this._enclosing = _enclosing;
            }

            public override short ReadShort() {
                return 2;
            }

            public override long Skip(long n) {
                return 2;
            }

            private readonly OtfReadCommonTest _enclosing;
        }
//\endcond
    }
}
