/*
This file is part of the iText (R) project.
Copyright (c) 1998-2026 Apryse Group NV
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
using iText.Test;

namespace iText.Kernel.Contrast {
    [NUnit.Framework.Category("UnitTest")]
    public class WCagCheckerTest : ExtendedITextTest {
        public static Object[][] WcagAAATestData() {
            return new Object[][] { 
                        // Normal text (<18pt)
                        new Object[] { 12.0, 7.0, true }, 
                        // exact minimum
                        new Object[] { 12.0, 6.99, false }, 
                        // just below minimum
                        new Object[] { 16.0, 8.0, true }, new Object[] { 16.0, 4.5, false }, 
                        // Large text (>=18.66pt is our threshold for large text)
                        new Object[] { 18.66, 4.5, true }, 
                        // exact minimum
                        new Object[] { 18.66, 4.49, false }, new Object[] { 24.0, 5.0, true }, new Object[] { 24.0, 3.0, false } };
        }

        public static Object[][] WcagAATestData() {
            return new Object[][] { 
                        // Normal text (<18pt)
                        new Object[] { 12.0, 4.5, true }, 
                        // exact minimum
                        new Object[] { 12.0, 4.49, false }, new Object[] { 16.0, 5.0, true }, new Object[] { 16.0, 3.0, false }, 
                        // Large text (>=18.66pt is our threshold for large text)
                        new Object[] { 18.66, 3.0, true }, 
                        // exact minimum
                        new Object[] { 18.66, 2.99, false }, new Object[] { 24.0, 4.5, true }, new Object[] { 24.0, 2.0, false } };
        }

        [NUnit.Framework.TestCaseSource("WcagAAATestData")]
        public virtual void TestWCAGAAACompliance(double fontSize, double contrastRatio, bool expectedCompliant) {
            bool result = WCagChecker.IsTextWcagAAACompliant(fontSize, contrastRatio);
            NUnit.Framework.Assert.AreEqual(expectedCompliant, result, String.Format("Font size %.2fpt with contrast %.2f should %s WCAG AAA"
                , fontSize, contrastRatio, expectedCompliant ? "pass" : "fail"));
        }

        [NUnit.Framework.TestCaseSource("WcagAATestData")]
        public virtual void TestWCAGAACompliance(double fontSize, double contrastRatio, bool expectedCompliant) {
            bool result = WCagChecker.IsTextWcagAACompliant(fontSize, contrastRatio);
            NUnit.Framework.Assert.AreEqual(expectedCompliant, result, String.Format("Font size %.2fpt with contrast %.2f should %s WCAG AA"
                , fontSize, contrastRatio, expectedCompliant ? "pass" : "fail"));
        }
    }
}
