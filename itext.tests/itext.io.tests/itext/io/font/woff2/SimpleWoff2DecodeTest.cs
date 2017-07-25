using System;

namespace iText.IO.Font.Woff2 {
    public class SimpleWoff2DecodeTest : Woff2DecodeTest {
        private static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/io/font/woff2/SimpleWoff2Decode/";

        private static readonly String targetFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory + "/test/itext/io/font/woff2/SimpleWoff2Decode/";

        [NUnit.Framework.OneTimeSetUp]
        public static void SetUp() {
            if (DEBUG) {
                CreateOrClearDestinationFolder(targetFolder);
            }
        }

        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void SimpleTTFTest() {
            RunTest("NotoSansCJKtc-Regular");
        }

        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void BigTTCTest() {
            RunTest("NotoSansCJK-Regular");
        }

        /// <exception cref="System.IO.IOException"/>
        private void RunTest(String fontName) {
            RunTest(fontName, sourceFolder, targetFolder, true);
        }
    }
}
