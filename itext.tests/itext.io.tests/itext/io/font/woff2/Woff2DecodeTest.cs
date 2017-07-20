using System;
using System.IO;
using iText.Test;

namespace iText.IO.Font.Woff2 {
    public abstract class Woff2DecodeTest : ExtendedITextTest {
        protected internal static bool DEBUG = true;

        protected internal virtual bool IsDebug() {
            return DEBUG;
        }

        /// <exception cref="System.IO.IOException"/>
        protected internal void RunTest(String fileName, String sourceFolder, String targetFolder, bool isFontValid
            ) {
            String inFile = fileName + ".woff2";
            String outFile = fileName + ".ttf";
            String cmpFile = "cmp_" + fileName + ".ttf";
            byte[] @in = null;
            byte[] @out = null;
            byte[] cmp = null;
            try {
                @in = ReadFile(sourceFolder + inFile);
                if (isFontValid) {
                    NUnit.Framework.Assert.IsTrue(Woff2Converter.IsWoff2Font(@in));
                }
                @out = Woff2Converter.Convert(@in);
                cmp = ReadFile(sourceFolder + cmpFile);
                NUnit.Framework.Assert.IsTrue(isFontValid, "Only valid fonts should reach this");
                NUnit.Framework.Assert.AreEqual(cmp, @out);
            }
            catch (FontCompressionException e) {
                if (isFontValid) {
                    throw;
                }
            }
            finally {
                if (IsDebug()) {
                    SaveFile(@in, targetFolder + inFile);
                    SaveFile(@out, targetFolder + outFile);
                    SaveFile(cmp, targetFolder + cmpFile);
                }
            }
        }

        /// <exception cref="System.IO.IOException"/>
        protected internal void SaveFile(byte[] content, String fileName) {
            if (content != null) {
                Stream os = new FileStream(fileName, FileMode.Create);
                os.Write(content);
                os.Dispose();
            }
        }
    }
}
