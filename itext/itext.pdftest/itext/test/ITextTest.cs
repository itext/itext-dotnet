using System;
using System.IO;
using System.Text;
using iText.IO.Font;
using iText.IO.Util;
using NUnit.Framework;

namespace iText.Test {
    public abstract class ITextTest {
        //protected readonly ILogger LOGGER = LoggerFactory.GetLogger(gett);

        [OneTimeSetUp]
        public static void SetUpFixture() {
            ResourceUtil.AddToResourceSearch(TestContext.CurrentContext.TestDirectory + "/itext.hyph.dll");
            ResourceUtil.AddToResourceSearch(TestContext.CurrentContext.TestDirectory + "/itext.font_asian.dll");
        }

        public static void CreateDestinationFolder(String path) {
            Directory.CreateDirectory(path);
        }

        public static void CreateOrClearDestinationFolder(String path) {
            Directory.CreateDirectory(path);
            foreach (String f in Directory.GetFiles(path)) {
                File.Delete(f);
            }
        }

        public static void DeleteDirectory(String path) {
            if (Directory.Exists(path)) {
                foreach (string d in Directory.GetDirectories(path)) {
                    DeleteDirectory(d);
                    Directory.Delete(d);
                }
                foreach (string f in Directory.GetFiles(path)) {
                    File.Delete(f);
                }
                Directory.Delete(path);
            }
        }

        protected virtual byte[] ReadFile(String filename) {
            return File.ReadAllBytes(filename);
        }

        protected virtual String CreateStringByEscaped(byte[] bytes)
        {
            String[] chars = PdfEncodings.ConvertToString(bytes, null).Substring(1).Split('#');
            StringBuilder buf = new StringBuilder(chars.Length);
            foreach (String ch in chars)
            {
                if (ch.Length == 0) continue;
                int b = Convert.ToInt32(ch, 16);
                buf.Append((char) b);
            }
            return buf.ToString();
        }
    }
}