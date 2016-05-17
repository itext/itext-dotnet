using System;
using System.IO;

namespace iTextSharp.Test {
    public abstract class ITextTest {
        //protected readonly ILogger LOGGER = LoggerFactory.GetLogger(gett);

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

        protected virtual String CreateStringByEscaped(byte[] bytes) {
            throw new NotImplementedException();
//            // TODO
//    String[] chars = (new String(bytes)).substring(1).split("#");
//    StringBuilder buf = new StringBuilder(chars.length);
//    for (String ch : chars)
//    {
//        if (ch.length() == 0) continue;
//        Integer b = Integer.parseInt(ch, 16);
//        buf.append((char)b.intValue());
//    }
//    return buf.toString();
//}
        }
    }
}