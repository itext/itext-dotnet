using System;

namespace iText.IO.Util {
    /// <summary>This file is a helper class for internal usage only.</summary>
    /// <remarks>
    /// This file is a helper class for internal usage only.
    /// Be aware that it's API and functionality may be changed in future.
    /// </remarks>
    public sealed class ExceptionUtil {
        private ExceptionUtil() {
        }

        public static bool IsOutOfRange(Exception e) {
            return e is IndexOutOfRangeException || e is ArgumentOutOfRangeException;
        }
    }
}
