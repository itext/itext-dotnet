/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: iText Software.

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License version 3
as published by the Free Software Foundation with the addition of the
following permission added to Section 15 as permitted in Section 7(a):
FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
OF THIRD PARTY RIGHTS

This program is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU Affero General Public License for more details.
You should have received a copy of the GNU Affero General Public License
along with this program; if not, see http://www.gnu.org/licenses or write to
the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
Boston, MA, 02110-1301 USA, or download the license from the following URL:
http://itextpdf.com/terms-of-use/

The interactive user interfaces in modified source and object code versions
of this program must display Appropriate Legal Notices, as required under
Section 5 of the GNU Affero General Public License.

In accordance with Section 7(b) of the GNU Affero General Public License,
a covered work must retain the producer line in every PDF that is created
or manipulated using iText.

You can be released from the requirements of the license by purchasing
a commercial license. Buying such a license is mandatory as soon as you
develop commercial activities involving the iText software without
disclosing the source code of your own applications.
These activities include: offering paid services to customers as an ASP,
serving PDFs on the fly in a web application, shipping iText with a closed
source product.

For more information, please contact iText Software Corp. at this
address: sales@itextpdf.com
*/
using System;
using iText.Layout.Hyphenation;
using iText.Test;

namespace iText.Layout {
    [NUnit.Framework.Category("IntegrationTest")]
    public class HyphenateResultTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void UkraineHyphenTest() {
            //здравствуйте
            TestHyphenateResult("uk", "\u0437\u0434\u0440\u0430\u0432\u0441\u0442\u0432\u0443\u0439", new int[] { 5 });
        }

        [NUnit.Framework.Test]
        public virtual void UkraineNoneHyphenTest() {
            //день
            TestHyphenateResult("uk", "\u0434\u0435\u043D\u044C", null);
        }

        [NUnit.Framework.Test]
        public virtual void ParenthesisTest01() {
            //Annuitätendarlehen
            TestHyphenateResult("de", "((:::(\"|;Annuitätendarlehen|\")))", new int[] { 5, 7, 10, 13, 15 });
        }

        [NUnit.Framework.Test]
        public virtual void HindiHyphResult() {
            //लाभहानि
            TestHyphenateResult("hi", "लाभहानि", new int[] { 3 });
        }

        [NUnit.Framework.Test]
        public virtual void SpacesTest01() {
            //Annuitätendarlehen
            TestHyphenateResult("de", "    Annuitätendarlehen", new int[] { 5, 7, 10, 13, 15 });
        }

        [NUnit.Framework.Test]
        public virtual void SoftHyphenTest01() {
            //Ann\u00ADuit\u00ADätendarl\u00ADehen
            TestHyphenateResult("de", "Ann\u00ADuit\u00ADätendarl\u00ADehen", new int[] { 3, 7, 16 });
        }

        [NUnit.Framework.Test]
        public virtual void StackoverflowTestDe() {
            //https://stackoverflow.com/
            TestHyphenateResult("de", "https://stackoverflow.com/", new int[] { 3, 14, 17 });
        }

        [NUnit.Framework.Test]
        public virtual void StackoverflowTestEn() {
            //https://stackoverflow.com/
            TestHyphenateResult("en", "https://stackoverflow.com/", new int[] { 13, 17 });
        }

        [NUnit.Framework.Test]
        public virtual void NonBreakingHyphenTest01() {
            //99\u2011verheiratet
            TestHyphenateResult("de", "999\u2011verheiratet", new int[] { 3, 6, 8 });
        }

        [NUnit.Framework.Test]
        public virtual void NonBreakingHyphenTest02() {
            //honorificabilitudinitatibus
            TestHyphenateResult("en", "honorificabilitudinitatibus", new int[] { 3, 5, 6, 9, 11, 13, 15, 19, 21, 22, 24
                 });
        }

        [NUnit.Framework.Test]
        public virtual void NonBreakingHyphenTest02A() {
            //honorificabil\u2011itudinitatibus
            TestHyphenateResult("en", "honorificabil\u2011itudinitatibus", new int[] { 3, 5, 6, 9, 11, 20, 22, 23, 25 }
                );
        }

        [NUnit.Framework.Test]
        public virtual void NumberTest01() {
            //123456789
            TestHyphenateResult("en", "123456789", null);
        }

        private void TestHyphenateResult(String lang, String testWorld, int[] expectedHyphenatePoints) {
            String[] parts = iText.Commons.Utils.StringUtil.Split(lang, "_");
            lang = parts[0];
            String country = (parts.Length == 2) ? parts[1] : null;
            HyphenationConfig config = new HyphenationConfig(lang, country, 3, 3);
            iText.Layout.Hyphenation.Hyphenation result = config.Hyphenate(testWorld);
            if (result != null) {
                NUnit.Framework.Assert.AreEqual(expectedHyphenatePoints, result.GetHyphenationPoints());
            }
            else {
                NUnit.Framework.Assert.IsNull(expectedHyphenatePoints);
            }
        }
    }
}
