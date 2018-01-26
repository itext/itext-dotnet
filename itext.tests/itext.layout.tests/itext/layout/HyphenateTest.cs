/*
This file is part of the iText (R) project.
Copyright (c) 1998-2017 iText Group NV
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
using System.Collections.Generic;
using System.Text;
using iText.IO.Util;
using iText.Layout.Hyphenation;
using iText.Test;

namespace iText.Layout {
    public class HyphenateTest : ExtendedITextTest {
        private IList<HyphenateTest.TestParams> @params = iText.IO.Util.JavaUtil.ArraysAsList(new HyphenateTest.TestParams
            ("af"), new HyphenateTest.TestParams("as", false), new HyphenateTest.TestParams("bg", false), new HyphenateTest.TestParams
            ("bn", false), new HyphenateTest.TestParams("ca", false), new HyphenateTest.TestParams("cop", false), 
            new HyphenateTest.TestParams("cs"), new HyphenateTest.TestParams("cy"), new HyphenateTest.TestParams("da"
            ), new HyphenateTest.TestParams("de"), new HyphenateTest.TestParams("de_1901"), new HyphenateTest.TestParams
            ("de_CH"), new HyphenateTest.TestParams("de_DR"), new HyphenateTest.TestParams("el", false), new HyphenateTest.TestParams
            ("el_Polyton", false), new HyphenateTest.TestParams("en"), new HyphenateTest.TestParams("en_GB"), new 
            HyphenateTest.TestParams("en_US"), new HyphenateTest.TestParams("eo"), new HyphenateTest.TestParams("es"
            , false), new HyphenateTest.TestParams("et", false), new HyphenateTest.TestParams("eu", false), new HyphenateTest.TestParams
            ("fi", false), new HyphenateTest.TestParams("fr"), new HyphenateTest.TestParams("ga"), new HyphenateTest.TestParams
            ("gl"), new HyphenateTest.TestParams("grc", false), new HyphenateTest.TestParams("gu", false), new HyphenateTest.TestParams
            ("hi", false), new HyphenateTest.TestParams("hr"), new HyphenateTest.TestParams("hsb"), new HyphenateTest.TestParams
            ("hu", false), new HyphenateTest.TestParams("hy", false), new HyphenateTest.TestParams("ia"), new HyphenateTest.TestParams
            ("id"), new HyphenateTest.TestParams("is"), new HyphenateTest.TestParams("it"), new HyphenateTest.TestParams
            ("kmr"), new HyphenateTest.TestParams("kn", false), new HyphenateTest.TestParams("la"), new HyphenateTest.TestParams
            ("lo", false), new HyphenateTest.TestParams("lt", false), new HyphenateTest.TestParams("lv", false), new 
            HyphenateTest.TestParams("ml", false), new HyphenateTest.TestParams("mn", false), new HyphenateTest.TestParams
            ("mr", false), new HyphenateTest.TestParams("nb"), new HyphenateTest.TestParams("nl"), new HyphenateTest.TestParams
            ("nn"), new HyphenateTest.TestParams("no"), new HyphenateTest.TestParams("or", false), new HyphenateTest.TestParams
            ("pa", false), new HyphenateTest.TestParams("pl"), new HyphenateTest.TestParams("pt"), new HyphenateTest.TestParams
            ("ro"), new HyphenateTest.TestParams("ru", "здравствуй"), new HyphenateTest.TestParams("sa"), new HyphenateTest.TestParams
            ("sk"), new HyphenateTest.TestParams("sl"), new HyphenateTest.TestParams("sr_Cyrl", false), new HyphenateTest.TestParams
            ("sr_Latn"), new HyphenateTest.TestParams("sv", false), new HyphenateTest.TestParams("ta", false), new 
            HyphenateTest.TestParams("te", false), new HyphenateTest.TestParams("tk"), new HyphenateTest.TestParams
            ("tr", false), new HyphenateTest.TestParams("uk", "здравствуй"), new HyphenateTest.TestParams("zh_Latn"
            ));

        private IList<String> errors = new List<String>();

        [NUnit.Framework.Test]
        public virtual void RunTest() {
            foreach (HyphenateTest.TestParams param in @params) {
                TryHyphenate(param.lang, param.testWorld, param.shouldPass);
            }
            NUnit.Framework.Assert.IsTrue(errors.IsEmpty(), BuildReport());
        }

        private void TryHyphenate(String lang, String testWorld, bool shouldPass) {
            String[] parts = iText.IO.Util.StringUtil.Split(lang, "_");
            lang = parts[0];
            String country = (parts.Length == 2) ? parts[1] : null;
            HyphenationConfig config = new HyphenationConfig(lang, country, 3, 3);
            iText.Layout.Hyphenation.Hyphenation result = config.Hyphenate(testWorld);
            if ((result == null) == shouldPass) {
                errors.Add(MessageFormatUtil.Format("\nLanguage: {0}, error on hyphenate({1}), shouldPass: {2}", lang, testWorld
                    , shouldPass));
            }
        }

        private String BuildReport() {
            StringBuilder builder = new StringBuilder();
            builder.Append("There are ").Append(errors.Count).Append(" errors.");
            foreach (String message in errors) {
                builder.Append(message);
            }
            return builder.ToString();
        }

        private class TestParams {
            internal String lang;

            internal String testWorld;

            internal bool shouldPass;

            public TestParams(String lang, String testWorld, bool shouldPass) {
                this.lang = lang;
                this.testWorld = testWorld;
                this.shouldPass = shouldPass;
            }

            public TestParams(String lang, String testWorld)
                : this(lang, testWorld, true) {
            }

            public TestParams(String lang, bool shouldPass)
                : this(lang, "country", shouldPass) {
            }

            public TestParams(String lang)
                : this(lang, "country", true) {
            }
        }
    }
}
