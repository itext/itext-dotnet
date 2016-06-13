using System;
using System.Globalization;

namespace iText.IO.Util {
    /// <summary>
    /// This file is a helper class for internal usage only.
    /// Be aware that it's API and functionality may be changed in future.
    /// </summary>
    public class DecimalFormatUtil
    {
        public static String FormatNumber(double d, String pattern)
        {
            return d.ToString(pattern, CultureInfo.InvariantCulture);
        }
    }
}