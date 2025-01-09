/*
This file is part of the iText (R) project.
Copyright (c) 1998-2025 Apryse Group NV
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
using System.Collections.Generic;
using iText.Commons.Utils;
using iText.Test;

namespace iText.Kernel.Pdf.Function.Utils {
    [NUnit.Framework.Category("IntegrationTest")]
    public class SampleExtractorTest : ExtendedITextTest {
        private const String PARAMETERS_NAME_PATTERN = "{0}bitsPerSample";

        private static readonly byte[] SAMPLES = new byte[] { 0x01, 0x23, 0x45, 0x67, (byte)0x89, (byte)0xab, (byte
            )0xcd, (byte)0xef };

        public static ICollection<Object[]> SamplesInfo() {
            return JavaUtil.ArraysAsList(new Object[][] { new Object[] { 1, new long[] { 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 
                1, 0, 0, 0, 1, 1, 0, 1, 0, 0, 0, 1, 0, 1, 0, 1, 1, 0, 0, 1, 1, 1, 1, 0, 0, 0, 1, 0, 0, 1, 1, 0, 1, 0, 
                1, 0, 1, 1, 1, 1, 0, 0, 1, 1, 0, 1, 1, 1, 1, 0, 1, 1, 1, 1 } }, new Object[] { 2, new long[] { 0, 0, 0
                , 1, 0, 2, 0, 3, 1, 0, 1, 1, 1, 2, 1, 3, 2, 0, 2, 1, 2, 2, 2, 3, 3, 0, 3, 1, 3, 2, 3, 3 } }, new Object
                [] { 4, new long[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 } }, new Object[] { 8, new long
                [] { 1, (2 << 4) | 3, (4 << 4) | 5, (6 << 4) | 7, (8 << 4) | 9, (10 << 4) | 11, (12 << 4) | 13, (14 <<
                 4) | 15 } }, new Object[] { 12, new long[] { (1 << 4) | 2, (3 << 8) | (4 << 4) | 5, (6 << 8) | (7 << 
                4) | 8, (9 << 8) | 10 << 4 | 11, (12 << 8) | (13 << 4) | 14 } }, new Object[] { 16, new long[] { (1 <<
                 8) | (2 << 4) | 3, (4 << 12) | (5 << 8) | (6 << 4) | 7, (8 << 12) | (9 << 8) | (10 << 4) | 11, (12 <<
                 12) | (13 << 8) | (14 << 4) | 15 } }, new Object[] { 24, new long[] { (1 << 16) | (2 << 12) | (3 << 8
                ) | (4 << 4) | 5, (6 << 20) | (7 << 16) | (8 << 12) | (9 << 8) | (10 << 4) | 11 } }, new Object[] { 32
                , new long[] { (1 << 24) | (2 << 20) | (3 << 16) | (4 << 12) | (5 << 8) | (6 << 4) | 7, (8L << 28) | (
                9 << 24) | (10 << 20) | (11 << 16) | (12 << 12) | (13 << 8) | (14 << 4) | 15 } } });
        }

        [NUnit.Framework.TestCaseSource("SamplesInfo")]
        public virtual void TestSamplesExtraction(int bitsPerSample, long[] expected) {
            long[] actual = new long[(SAMPLES.Length << 3) / bitsPerSample];
            NUnit.Framework.Assert.AreEqual(expected.Length, actual.Length);
            AbstractSampleExtractor extractor = AbstractSampleExtractor.CreateExtractor(bitsPerSample);
            for (int i = 0; i < actual.Length; ++i) {
                actual[i] = extractor.Extract(SAMPLES, i);
            }
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }
    }
}
