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
using iText.IO.Font.Cmap;
using iText.Test;
using iText.Test.Attributes;

namespace iText.IO.Font {
    [NUnit.Framework.Category("IntegrationTest")]
    public class LoadAllAsianFontsTest : ExtendedITextTest {
        [NUnit.Framework.TestCaseSource("Data")]
        // TODO DEVSIX-8619 All cmap parsing errors should be fixed and this logging should then be removed
        [LogMessage(iText.IO.Logs.IoLogMessageConstant.UNKNOWN_ERROR_WHILE_PROCESSING_CMAP, Ignore = true)]
        public virtual void TestAsianFonts(String cmapName, String ordering) {
            CheckFontAsianCmap(cmapName, ordering);
        }

        public static ICollection<Object[]> Data() {
            IList<Object[]> result = new List<Object[]>();
            result.Add(new Object[] { "78-EUC-H", "Japan1" });
            result.Add(new Object[] { "78-EUC-V", "Japan1" });
            result.Add(new Object[] { "78-H", "Japan1" });
            result.Add(new Object[] { "78-RKSJ-H", "Japan1" });
            result.Add(new Object[] { "78-RKSJ-V", "Japan1" });
            result.Add(new Object[] { "78-V", "Japan1" });
            result.Add(new Object[] { "78ms-RKSJ-H", "Japan1" });
            result.Add(new Object[] { "78ms-RKSJ-V", "Japan1" });
            result.Add(new Object[] { "83pv-RKSJ-H", "Japan1" });
            result.Add(new Object[] { "90ms-RKSJ-H", "Japan1" });
            result.Add(new Object[] { "90ms-RKSJ-V", "Japan1" });
            result.Add(new Object[] { "90msp-RKSJ-H", "Japan1" });
            result.Add(new Object[] { "90msp-RKSJ-V", "Japan1" });
            result.Add(new Object[] { "90pv-RKSJ-H", "Japan1" });
            result.Add(new Object[] { "90pv-RKSJ-V", "Japan1" });
            result.Add(new Object[] { "Add-H", "Japan1" });
            result.Add(new Object[] { "Add-RKSJ-H", "Japan1" });
            result.Add(new Object[] { "Add-RKSJ-V", "Japan1" });
            result.Add(new Object[] { "Add-V", "Japan1" });
            result.Add(new Object[] { "Adobe-CNS1-0", "CNS1" });
            result.Add(new Object[] { "Adobe-CNS1-1", "CNS1" });
            result.Add(new Object[] { "Adobe-CNS1-2", "CNS1" });
            result.Add(new Object[] { "Adobe-CNS1-3", "CNS1" });
            result.Add(new Object[] { "Adobe-CNS1-4", "CNS1" });
            result.Add(new Object[] { "Adobe-CNS1-5", "CNS1" });
            result.Add(new Object[] { "Adobe-CNS1-6", "CNS1" });
            result.Add(new Object[] { "Adobe-CNS1-7", "CNS1" });
            result.Add(new Object[] { "Adobe-GB1-0", "GB1" });
            result.Add(new Object[] { "Adobe-GB1-1", "GB1" });
            result.Add(new Object[] { "Adobe-GB1-2", "GB1" });
            result.Add(new Object[] { "Adobe-GB1-3", "GB1" });
            result.Add(new Object[] { "Adobe-GB1-4", "GB1" });
            result.Add(new Object[] { "Adobe-GB1-5", "GB1" });
            result.Add(new Object[] { "Adobe-Japan1-0", "Japan1" });
            result.Add(new Object[] { "Adobe-Japan1-1", "Japan1" });
            result.Add(new Object[] { "Adobe-Japan1-2", "Japan1" });
            result.Add(new Object[] { "Adobe-Japan1-3", "Japan1" });
            result.Add(new Object[] { "Adobe-Japan1-4", "Japan1" });
            result.Add(new Object[] { "Adobe-Japan1-5", "Japan1" });
            result.Add(new Object[] { "Adobe-Japan1-6", "Japan1" });
            result.Add(new Object[] { "Adobe-Japan1-7", "Japan1" });
            result.Add(new Object[] { "Adobe-Korea1-0", "Korea1" });
            result.Add(new Object[] { "Adobe-Korea1-1", "Korea1" });
            result.Add(new Object[] { "Adobe-Korea1-2", "Korea1" });
            result.Add(new Object[] { "Adobe-KR-0", "KR" });
            result.Add(new Object[] { "Adobe-KR-1", "KR" });
            result.Add(new Object[] { "Adobe-KR-2", "KR" });
            result.Add(new Object[] { "Adobe-KR-3", "KR" });
            result.Add(new Object[] { "Adobe-KR-4", "KR" });
            result.Add(new Object[] { "Adobe-KR-5", "KR" });
            result.Add(new Object[] { "Adobe-KR-6", "KR" });
            result.Add(new Object[] { "Adobe-KR-7", "KR" });
            result.Add(new Object[] { "Adobe-KR-8", "KR" });
            result.Add(new Object[] { "Adobe-KR-9", "KR" });
            result.Add(new Object[] { "B5-H", "CNS1" });
            result.Add(new Object[] { "B5-V", "CNS1" });
            result.Add(new Object[] { "B5pc-H", "CNS1" });
            result.Add(new Object[] { "B5pc-V", "CNS1" });
            result.Add(new Object[] { "CNS1-H", "CNS1" });
            result.Add(new Object[] { "CNS1-V", "CNS1" });
            result.Add(new Object[] { "CNS2-H", "CNS1" });
            result.Add(new Object[] { "CNS2-V", "CNS1" });
            result.Add(new Object[] { "CNS-EUC-H", "CNS1" });
            result.Add(new Object[] { "CNS-EUC-V", "CNS1" });
            result.Add(new Object[] { "ETen-B5-H", "CNS1" });
            result.Add(new Object[] { "ETen-B5-V", "CNS1" });
            result.Add(new Object[] { "ETenms-B5-H", "CNS1" });
            result.Add(new Object[] { "ETenms-B5-V", "CNS1" });
            result.Add(new Object[] { "ETHK-B5-H", "CNS1" });
            result.Add(new Object[] { "ETHK-B5-V", "CNS1" });
            result.Add(new Object[] { "EUC-H", "Japan1" });
            result.Add(new Object[] { "EUC-V", "Japan1" });
            result.Add(new Object[] { "Ext-H", "Japan1" });
            result.Add(new Object[] { "Ext-RKSJ-H", "Japan1" });
            result.Add(new Object[] { "Ext-RKSJ-V", "Japan1" });
            result.Add(new Object[] { "Ext-V", "Japan1" });
            result.Add(new Object[] { "GB-EUC-H", "GB1" });
            result.Add(new Object[] { "GB-EUC-V", "GB1" });
            result.Add(new Object[] { "GBK2K-H", "GB1" });
            result.Add(new Object[] { "GBK2K-V", "GB1" });
            result.Add(new Object[] { "GBK-EUC-H", "GB1" });
            result.Add(new Object[] { "GBK-EUC-V", "GB1" });
            result.Add(new Object[] { "GBKp-EUC-H", "GB1" });
            result.Add(new Object[] { "GBKp-EUC-V", "GB1" });
            result.Add(new Object[] { "GBpc-EUC-H", "GB1" });
            result.Add(new Object[] { "GBpc-EUC-V", "GB1" });
            result.Add(new Object[] { "GBT-EUC-H", "GB1" });
            result.Add(new Object[] { "GBT-EUC-V", "GB1" });
            result.Add(new Object[] { "GBT-H", "GB1" });
            result.Add(new Object[] { "GBT-V", "GB1" });
            result.Add(new Object[] { "GBTpc-EUC-H", "GB1" });
            result.Add(new Object[] { "GBTpc-EUC-V", "GB1" });
            result.Add(new Object[] { "H", "Japan1" });
            result.Add(new Object[] { "Hankaku", "Japan1" });
            result.Add(new Object[] { "Hiragana", "Japan1" });
            result.Add(new Object[] { "HKdla-B5-H", "CNS1" });
            result.Add(new Object[] { "HKdla-B5-V", "CNS1" });
            result.Add(new Object[] { "HKdlb-B5-H", "CNS1" });
            result.Add(new Object[] { "HKdlb-B5-V", "CNS1" });
            result.Add(new Object[] { "HKgccs-B5-H", "CNS1" });
            result.Add(new Object[] { "HKgccs-B5-V", "CNS1" });
            result.Add(new Object[] { "HKm314-B5-H", "CNS1" });
            result.Add(new Object[] { "HKm314-B5-V", "CNS1" });
            result.Add(new Object[] { "HKm471-B5-H", "CNS1" });
            result.Add(new Object[] { "HKm471-B5-V", "CNS1" });
            result.Add(new Object[] { "HKscs-B5-H", "CNS1" });
            result.Add(new Object[] { "HKscs-B5-V", "CNS1" });
            result.Add(new Object[] { "Identity-H", "Identity" });
            result.Add(new Object[] { "Identity-V", "Identity" });
            result.Add(new Object[] { "Katakana", "Japan1" });
            result.Add(new Object[] { "KSC-EUC-H", "Korea1" });
            result.Add(new Object[] { "KSC-EUC-V", "Korea1" });
            result.Add(new Object[] { "KSC-H", "Korea1" });
            result.Add(new Object[] { "KSC-Johab-H", "Korea1" });
            result.Add(new Object[] { "KSC-Johab-V", "Korea1" });
            result.Add(new Object[] { "KSC-V", "Korea1" });
            result.Add(new Object[] { "KSCms-UHC-H", "Korea1" });
            result.Add(new Object[] { "KSCms-UHC-HW-H", "Korea1" });
            result.Add(new Object[] { "KSCms-UHC-HW-V", "Korea1" });
            result.Add(new Object[] { "KSCms-UHC-V", "Korea1" });
            result.Add(new Object[] { "KSCpc-EUC-H", "Korea1" });
            result.Add(new Object[] { "KSCpc-EUC-V", "Korea1" });
            result.Add(new Object[] { "NWP-H", "Japan1" });
            result.Add(new Object[] { "NWP-V", "Japan1" });
            result.Add(new Object[] { "RKSJ-H", "Japan1" });
            result.Add(new Object[] { "RKSJ-V", "Japan1" });
            result.Add(new Object[] { "Roman", "Japan1" });
            result.Add(new Object[] { "UniAKR-UTF8-H", "KR" });
            result.Add(new Object[] { "UniAKR-UTF16-H", "KR" });
            result.Add(new Object[] { "UniAKR-UTF32-H", "KR" });
            result.Add(new Object[] { "UniCNS-UCS2-H", "CNS1" });
            result.Add(new Object[] { "UniCNS-UCS2-V", "CNS1" });
            result.Add(new Object[] { "UniCNS-UTF8-H", "CNS1" });
            result.Add(new Object[] { "UniCNS-UTF8-V", "CNS1" });
            result.Add(new Object[] { "UniCNS-UTF16-H", "CNS1" });
            result.Add(new Object[] { "UniCNS-UTF16-V", "CNS1" });
            result.Add(new Object[] { "UniCNS-UTF32-H", "CNS1" });
            result.Add(new Object[] { "UniCNS-UTF32-V", "CNS1" });
            result.Add(new Object[] { "UniGB-UCS2-H", "GB1" });
            result.Add(new Object[] { "UniGB-UCS2-V", "GB1" });
            result.Add(new Object[] { "UniGB-UTF8-H", "GB1" });
            result.Add(new Object[] { "UniGB-UTF8-V", "GB1" });
            result.Add(new Object[] { "UniGB-UTF16-H", "GB1" });
            result.Add(new Object[] { "UniGB-UTF16-V", "GB1" });
            result.Add(new Object[] { "UniGB-UTF32-H", "GB1" });
            result.Add(new Object[] { "UniGB-UTF32-V", "GB1" });
            result.Add(new Object[] { "UniJIS2004-UTF8-H", "Japan1" });
            result.Add(new Object[] { "UniJIS2004-UTF8-V", "Japan1" });
            result.Add(new Object[] { "UniJIS2004-UTF16-H", "Japan1" });
            result.Add(new Object[] { "UniJIS2004-UTF16-V", "Japan1" });
            result.Add(new Object[] { "UniJIS2004-UTF32-H", "Japan1" });
            result.Add(new Object[] { "UniJIS2004-UTF32-V", "Japan1" });
            result.Add(new Object[] { "UniJIS-UCS2-H", "Japan1" });
            result.Add(new Object[] { "UniJIS-UCS2-HW-H", "Japan1" });
            result.Add(new Object[] { "UniJIS-UCS2-HW-V", "Japan1" });
            result.Add(new Object[] { "UniJIS-UCS2-V", "Japan1" });
            result.Add(new Object[] { "UniJIS-UTF8-H", "Japan1" });
            result.Add(new Object[] { "UniJIS-UTF8-V", "Japan1" });
            result.Add(new Object[] { "UniJIS-UTF16-H", "Japan1" });
            result.Add(new Object[] { "UniJIS-UTF16-V", "Japan1" });
            result.Add(new Object[] { "UniJIS-UTF32-H", "Japan1" });
            result.Add(new Object[] { "UniJIS-UTF32-V", "Japan1" });
            result.Add(new Object[] { "UniJISPro-UCS2-HW-V", "Japan1" });
            result.Add(new Object[] { "UniJISPro-UCS2-V", "Japan1" });
            result.Add(new Object[] { "UniJISPro-UTF8-V", "Japan1" });
            result.Add(new Object[] { "UniJISX0213-UTF32-H", "Japan1" });
            result.Add(new Object[] { "UniJISX0213-UTF32-V", "Japan1" });
            result.Add(new Object[] { "UniJISX02132004-UTF32-H", "Japan1" });
            result.Add(new Object[] { "UniJISX02132004-UTF32-V", "Japan1" });
            result.Add(new Object[] { "UniKS-UCS2-H", "Korea1" });
            result.Add(new Object[] { "UniKS-UCS2-V", "Korea1" });
            result.Add(new Object[] { "UniKS-UTF8-H", "Korea1" });
            result.Add(new Object[] { "UniKS-UTF8-V", "Korea1" });
            result.Add(new Object[] { "UniKS-UTF16-H", "Korea1" });
            result.Add(new Object[] { "UniKS-UTF16-V", "Korea1" });
            result.Add(new Object[] { "UniKS-UTF32-H", "Korea1" });
            result.Add(new Object[] { "UniKS-UTF32-V", "Korea1" });
            result.Add(new Object[] { "V", "Japan1" });
            result.Add(new Object[] { "WP-Symbol", "Japan1" });
            result.Add(new Object[] { ResourceTestUtil.NormalizeResourceName("toUnicode/Adobe-CNS1-UCS2"), "Adobe_CNS1_UCS2"
                 });
            result.Add(new Object[] { ResourceTestUtil.NormalizeResourceName("toUnicode/Adobe-GB1-UCS2"), "Adobe_GB1_UCS2"
                 });
            result.Add(new Object[] { ResourceTestUtil.NormalizeResourceName("toUnicode/Adobe-Japan1-UCS2"), "Adobe_Japan1_UCS2"
                 });
            result.Add(new Object[] { ResourceTestUtil.NormalizeResourceName("toUnicode/Adobe-Korea1-UCS2"), "Adobe_Korea1_UCS2"
                 });
            result.Add(new Object[] { ResourceTestUtil.NormalizeResourceName("toUnicode/Adobe-KR-UCS2"), "Adobe_KR_UCS2"
                 });
            return result;
        }

        private void CheckFontAsianCmap(String cmapName, String ordering) {
            AbstractCMap cmap = CjkResourceLoader.GetUni2CidCmap(cmapName);
            NUnit.Framework.Assert.IsTrue(cmapName.EndsWith(cmap.GetName()));
            NUnit.Framework.Assert.AreEqual(ordering, cmap.GetOrdering());
        }
    }
}
