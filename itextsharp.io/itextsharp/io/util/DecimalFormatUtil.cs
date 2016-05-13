using System;
using System.Globalization;

namespace iTextSharp.IO.Util {
    public class DecimalFormatUtil
    {
        public static String FormatNumber(double d, String pattern)
        {
            return d.ToString(pattern, CultureInfo.InvariantCulture);
        }
    }
}