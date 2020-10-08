using System;
using System.Collections.Generic;
using iText.IO.Source;
using iText.Test;

namespace iText.IO.Font.Otf {
    public class OtfReadCommonTest : ExtendedITextTest {
        private static readonly String RESOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/io/font/otf/OtfReadCommonTest/";

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
    }
}
