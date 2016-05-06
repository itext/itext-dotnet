using System;
using System.Globalization;

namespace com.itextpdf.io.util
{
    public class DecimalFormatUtil
    {
        public static String FormatNumber(double d, String pattern)
        {
            return d.ToString(pattern, CultureInfo.InvariantCulture);
        }
    }
}