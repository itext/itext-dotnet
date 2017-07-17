using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iText.IO.Util
{
    public static class MathUtil
    {
        public static double ToRadians(double angdeg)
        {
            return angdeg / 180.0 * Math.PI;
        }
    }
}
