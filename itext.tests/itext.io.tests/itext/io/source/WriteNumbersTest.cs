/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 Apryse Group NV
Authors: Apryse Software.

This program is offered under a commercial and under the AGPL license.
For commercial licensing, contact us at https://itextpdf.com/sales.  For AGPL licensing, see below.

AGPL licensing:
This program is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/
using System;
using iText.Commons.Utils;
using iText.IO.Util;
using iText.Test;
using iText.Test.Attributes;

namespace iText.IO.Source {
    [NUnit.Framework.Category("UnitTest")]
    public class WriteNumbersTest : ExtendedITextTest {
        public static double Round(double value, int places) {
            return MathematicUtil.Round(value * Math.Pow(10, places)) / Math.Pow(10, places);
        }

        [NUnit.Framework.Test]
        public virtual void WriteNumber1Test() {
            Random rnd = new Random();
            for (int i = 0; i < 100000; i++) {
                double d = (double)rnd.Next(2120000000) / 100000;
                d = Round(d, 2);
                if (d < 1.02) {
                    i--;
                    continue;
                }
                byte[] actuals = ByteUtils.GetIsoBytes(d);
                byte[] expecteds = DecimalFormatUtil.FormatNumber(d, "0.##").GetBytes(iText.Commons.Utils.EncodingUtil.ISO_8859_1
                    );
                String message = "Expects: " + iText.Commons.Utils.JavaUtil.GetStringForBytes(expecteds) + ", actual: " + 
                    iText.Commons.Utils.JavaUtil.GetStringForBytes(actuals) + " \\\\ " + d;
                NUnit.Framework.Assert.AreEqual(expecteds, actuals, message);
            }
        }

        [NUnit.Framework.Test]
        public virtual void WriteNumber2Test() {
            Random rnd = new Random();
            for (int i = 0; i < 100000; i++) {
                double d = (double)rnd.Next(1000000) / 1000000;
                d = Round(d, 5);
                if (Math.Abs(d) < 0.000015) {
                    continue;
                }
                byte[] actuals = ByteUtils.GetIsoBytes(d);
                byte[] expecteds = DecimalFormatUtil.FormatNumber(d, "0.#####").GetBytes(iText.Commons.Utils.EncodingUtil.ISO_8859_1
                    );
                String message = "Expects: " + iText.Commons.Utils.JavaUtil.GetStringForBytes(expecteds) + ", actual: " + 
                    iText.Commons.Utils.JavaUtil.GetStringForBytes(actuals) + " \\\\ " + d;
                NUnit.Framework.Assert.AreEqual(expecteds, actuals, message);
            }
        }

        [NUnit.Framework.Test]
        public virtual void WriteNumber3Test() {
            Random rnd = new Random();
            for (int i = 0; i < 100000; i++) {
                double d = rnd.NextDouble();
                if (d < 32700) {
                    d *= 100000;
                }
                d = Round(d, 0);
                byte[] actuals = ByteUtils.GetIsoBytes(d);
                byte[] expecteds = DecimalFormatUtil.FormatNumber(d, "0").GetBytes(iText.Commons.Utils.EncodingUtil.ISO_8859_1
                    );
                String message = "Expects: " + iText.Commons.Utils.JavaUtil.GetStringForBytes(expecteds) + ", actual: " + 
                    iText.Commons.Utils.JavaUtil.GetStringForBytes(actuals) + " \\\\ " + d;
                NUnit.Framework.Assert.AreEqual(expecteds, actuals, message);
            }
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.ATTEMPT_PROCESS_NAN)]
        public virtual void WriteNanTest() {
            double d = double.NaN;
            byte[] actuals = ByteUtils.GetIsoBytes(d);
            byte[] expecteds = DecimalFormatUtil.FormatNumber(0, "0.##").GetBytes(iText.Commons.Utils.EncodingUtil.ISO_8859_1
                );
            String message = "Expects: " + iText.Commons.Utils.JavaUtil.GetStringForBytes(expecteds) + ", actual: " + 
                iText.Commons.Utils.JavaUtil.GetStringForBytes(actuals) + " \\\\ " + d;
            NUnit.Framework.Assert.AreEqual(expecteds, actuals, message);
        }

        [NUnit.Framework.Test]
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.ATTEMPT_PROCESS_NAN)]
        public virtual void WriteNanHighPrecisionTest() {
            double d = double.NaN;
            byte[] actuals = ByteUtils.GetIsoBytes(d, null, true);
            byte[] expecteds = DecimalFormatUtil.FormatNumber(0, "0.##").GetBytes(iText.Commons.Utils.EncodingUtil.ISO_8859_1
                );
            String message = "Expects: " + iText.Commons.Utils.JavaUtil.GetStringForBytes(expecteds) + ", actual: " + 
                iText.Commons.Utils.JavaUtil.GetStringForBytes(actuals) + " \\\\ " + d;
            NUnit.Framework.Assert.AreEqual(expecteds, actuals, message);
        }
    }
}
