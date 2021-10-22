using System;

namespace iText.Commons.Utils
{
    public class MathematicUtil
    {
        public static double Round(double a)
        {
            return Math.Round(a, MidpointRounding.AwayFromZero);
        }
    }
}