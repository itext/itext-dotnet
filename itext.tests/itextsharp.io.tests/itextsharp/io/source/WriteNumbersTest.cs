using System;
using iTextSharp.IO.Util;

namespace iTextSharp.IO.Source
{
    public class WriteNumbersTest
    {
        public static double Round(double value, int places)
        {
            return Math.Round(value * Math.Pow(10, places)) / Math.Pow(10, places);
        }

        [NUnit.Framework.Test]
        public virtual void WriteNumber1Test()
        {
            Random rnd = new Random();
            for (int i = 0; i < 100000; i++)
            {
                double d = (double)rnd.Next(2120000000) / 100000;
                d = Round(d, 2);
                if (d < 1.02)
                {
                    i--;
                    continue;
                }
                byte[] actuals = ByteUtils.GetIsoBytes(d);
                byte[] expecteds = DecimalFormatUtil.FormatNumber(d, "0.##").GetBytes();
                String message = "Expects: " + iTextSharp.IO.Util.JavaUtil.GetStringForBytes(expecteds) + ", actual: " + iTextSharp.IO.Util.JavaUtil.GetStringForBytes
                    (actuals) + " \\\\ " + d;
                NUnit.Framework.Assert.AreEqual(expecteds, actuals, message);
            }
        }

        [NUnit.Framework.Test]
        public virtual void WriteNumber2Test()
        {
            Random rnd = new Random();
            for (int i = 0; i < 100000; i++)
            {
                double d = (double)rnd.Next(1000000) / 1000000;
                d = Round(d, 5);
                if (Math.Abs(d) < 0.000015)
                {
                    continue;
                }
                byte[] actuals = ByteUtils.GetIsoBytes(d);
                byte[] expecteds = DecimalFormatUtil.FormatNumber(d, "0.#####").GetBytes();
                String message = "Expects: " + iTextSharp.IO.Util.JavaUtil.GetStringForBytes(expecteds) + ", actual: " + iTextSharp.IO.Util.JavaUtil.GetStringForBytes
                    (actuals) + " \\\\ " + d;
                NUnit.Framework.Assert.AreEqual(expecteds, actuals, message);
            }
        }

        [NUnit.Framework.Test]
        public virtual void WriteNumber3Test()
        {
            Random rnd = new Random();
            for (int i = 0; i < 100000; i++)
            {
                double d = rnd.NextDouble();
                if (d < 32700)
                {
                    d *= 100000;
                }
                d = Round(d, 0);
                byte[] actuals = ByteUtils.GetIsoBytes(d);
                byte[] expecteds = DecimalFormatUtil.FormatNumber(d, "0").GetBytes();
                String message = "Expects: " + iTextSharp.IO.Util.JavaUtil.GetStringForBytes(expecteds) + ", actual: " + iTextSharp.IO.Util.JavaUtil.GetStringForBytes
                    (actuals) + " \\\\ " + d;
                NUnit.Framework.Assert.AreEqual(expecteds, actuals, message);
            }
        }
    }
}
