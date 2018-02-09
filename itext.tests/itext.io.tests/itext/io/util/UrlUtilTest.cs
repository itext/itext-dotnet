using System;
using System.IO;
using iText.Test;

namespace iText.IO.Util {
    public class UrlUtilTest : ExtendedITextTest {
        private static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/io/UrlUtilTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(destinationFolder);
        }

        // Tests that after invocation of the getFinalURL method for local files, no handles are left open and the file is free to be removed
        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void GetFinalURLDoesNotLockFileTest() {
            FileInfo tempFile = FileUtil.CreateTempFile(destinationFolder);
            UrlUtil.GetFinalURL(UrlUtil.ToURL(tempFile.FullName));
            NUnit.Framework.Assert.IsTrue(FileUtil.DeleteFile(tempFile));
        }
    }
}
