using System;
using System.IO;
using iText.IO.Font.Woff2;

namespace iText.IO.Font.Woff2.W3c {
    public abstract class W3CWoff2DecodeTest : Woff2DecodeTest {
        private static readonly String baseSourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/io/font/woff2/w3c/";

        private static readonly String baseDestinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/io/font/woff2/w3c/";

        protected internal abstract String GetFontName();

        protected internal abstract String GetTestInfo();

        protected internal abstract bool IsFontValid();

        [NUnit.Framework.SetUp]
        public virtual void SetUp() {
            if (IsDebug()) {
                CreateOrClearDestinationFolder(GetDestinationFolder());
            }
        }

        /// <exception cref="System.IO.IOException"/>
        [NUnit.Framework.Test]
        public virtual void RunTest() {
            System.Console.Out.Write("\n" + GetTestInfo() + "\n");
            RunTest(GetFontName(), GetSourceFolder(), GetDestinationFolder(), IsFontValid());
        }

        private String GetDestinationFolder() {
            String localPackage = GetLocalPackage();
            return baseDestinationFolder + localPackage + Path.DirectorySeparatorChar + GetTestClassName() + Path.DirectorySeparatorChar;
        }

        private String GetSourceFolder() {
            String localPackage = GetLocalPackage();
            return baseSourceFolder + localPackage + Path.DirectorySeparatorChar;
        }

        private String GetTestClassName() {
            return GetType().Name;
        }

        private String GetLocalPackage() {
            String packageName = GetType().Namespace.ToString();
            String basePackageName = typeof(W3CWoff2DecodeTest).Namespace.ToString();
            return packageName.Substring(basePackageName.Length).Replace('.', Path.DirectorySeparatorChar);
        }
    }
}
