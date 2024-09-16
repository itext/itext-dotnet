using System;

namespace iText.IO.Font {
//\cond DO_NOT_DOCUMENT
    internal class ResourceTestUtil {
        public static String NormalizeResourceName(String resourceName) {
            string result = resourceName.Replace("toUnicode/", "ToUnicode.");
            return result;
        }
    }
//\endcond
}
