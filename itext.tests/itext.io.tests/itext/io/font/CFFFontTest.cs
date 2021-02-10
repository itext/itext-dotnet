using System;
using iText.IO.Source;
using iText.Test;

namespace iText.IO.Font {
    public class CFFFontTest : ExtendedITextTest {
        private static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/io/font/otf/CFFFontTest/";

        [NUnit.Framework.Test]
        public virtual void SeekTest() {
            RandomAccessFileOrArray raf = new RandomAccessFileOrArray(new RandomAccessSourceFactory().CreateBestSource
                (SOURCE_FOLDER + "NotoSansCJKjp-Bold.otf"));
            byte[] cff = new byte[16014190];
            try {
                raf.Seek(283192);
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
            byte[] cff = new byte[16014190];
            try {
                raf.Seek(283192);
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
