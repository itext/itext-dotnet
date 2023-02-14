using System;

namespace iText.Commons.Utils {
    public sealed class StringSplitUtil {
        private StringSplitUtil() {
        }

        public static String[] SplitKeepTrailingWhiteSpace(String data, char toSplitOn) {
            return data.Split(toSplitOn);
        }
    }
}
