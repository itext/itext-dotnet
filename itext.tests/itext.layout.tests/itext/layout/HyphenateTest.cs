/*
This file is part of the iText (R) project.
Copyright (c) 1998-2024 Apryse Group NV
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
using System.Text;
using iText.Commons.Utils;
using iText.Layout.Hyphenation;
using iText.Test;

namespace iText.Layout {
    [NUnit.Framework.Category("IntegrationTest")]
    public class HyphenateTest : ExtendedITextTest {
        private IList<HyphenateTest.TestParams> @params = JavaUtil.ArraysAsList(new HyphenateTest.TestParams("af")
            , 
                //নমস্কাৰ
                new HyphenateTest.TestParams("as", "\u09A8\u09AE\u09B8\u09CD\u0995\u09BE\u09F0"), 
                //Здравей
                new HyphenateTest.TestParams("bg", "\u0417\u0434\u0440\u0430\u0432\u0435\u0439"), 
                //আলাইকুম
                new HyphenateTest.TestParams("bn", "\u0986\u09B2\u09BE\u0987\u0995\u09C1\u09AE"), new HyphenateTest.TestParams
            ("ca", "Benvinguts"), 
                //ⲘⲉⲧⲢⲉⲙ̀ⲛⲭⲏⲙⲓ
                new HyphenateTest.TestParams("cop", "\u2C98\u2C89\u2CA7\u2CA2\u2C89\u2C99\u0300\u2C9B\u2CAD\u2C8F\u2C99\u2C93"
            ), new HyphenateTest.TestParams("cs"), new HyphenateTest.TestParams("cy"), new HyphenateTest.TestParams
            ("da"), new HyphenateTest.TestParams("de"), new HyphenateTest.TestParams("de_DE", "14\u00a0Tagen 14\u00a0Tagen 14\u00a0Tagen "
            ), new HyphenateTest.TestParams("de_DE", "14\u20110Tagen 14\u2011Tagen 14\u20110Tagen "), new HyphenateTest.TestParams
            ("de_1901"), new HyphenateTest.TestParams("de_CH"), new HyphenateTest.TestParams("de_DR"), 
                //καλημέρα
                new HyphenateTest.TestParams("el", "\u03BA\u03B1\u03BB\u03B7\u03BC\u03AD\u03C1\u03B1"), 
                //καλημέρα
                new HyphenateTest.TestParams("el_Polyton", "\u03BA\u03B1\u03BB\u03B7\u03BC\u03AD\u03C1\u03B1"), new HyphenateTest.TestParams
            ("en"), new HyphenateTest.TestParams("en_GB"), new HyphenateTest.TestParams("en_US"), new HyphenateTest.TestParams
            ("eo"), new HyphenateTest.TestParams("es", "gracias"), new HyphenateTest.TestParams("et", "Vabandust")
            , new HyphenateTest.TestParams("eu", "euskara"), 
                //Näkemiin
                new HyphenateTest.TestParams("fi", "N\u00E4kemiin"), new HyphenateTest.TestParams("fr"), new HyphenateTest.TestParams
            ("ga"), new HyphenateTest.TestParams("gl"), 
                //καλημέρα
                new HyphenateTest.TestParams("grc", "\u03BA\u03B1\u03BB\u03B7\u03BC\u03AD\u03C1\u03B1"), 
                //ગુજરાતી
                new HyphenateTest.TestParams("gu", "\u0A97\u0AC1\u0A9C\u0AB0\u0ABE\u0AA4\u0AC0"), 
                //सुप्रभातम्
                new HyphenateTest.TestParams("hi", "\u0938\u0941\u092A\u094D\u0930\u092D\u093E\u0924\u092E\u094D"), new HyphenateTest.TestParams
            ("hr"), new HyphenateTest.TestParams("hsb"), new HyphenateTest.TestParams("hu", "sziasztok"), 
                //շնորհակալություն
                new HyphenateTest.TestParams("hy", "\u0577\u0576\u0578\u0580\u0570\u0561\u056F\u0561\u056C\u0578\u0582\u0569\u0575\u0578\u0582\u0576"
            ), new HyphenateTest.TestParams("ia"), new HyphenateTest.TestParams("id"), new HyphenateTest.TestParams
            ("is"), new HyphenateTest.TestParams("it"), new HyphenateTest.TestParams("kmr"), 
                //ಕನ್ನಡ
                new HyphenateTest.TestParams("kn", "\u0C95\u0CA8\u0CCD\u0CA8\u0CA1"), new HyphenateTest.TestParams("la"), 
                //ຍິນດີຕ້ອນຮັບ
                new HyphenateTest.TestParams("lo", "\u0E8D\u0EB4\u0E99\u0E94\u0EB5\u0E95\u0EC9\u0EAD\u0E99\u0EAE\u0EB1\u0E9A"
            ), new HyphenateTest.TestParams("lt", "Labanakt"), new HyphenateTest.TestParams("lv", "Labvakar"), 
                //സ്വാഗതം
                new HyphenateTest.TestParams("ml", "\u0D38\u0D4D\u0D35\u0D3E\u0D17\u0D24\u0D02"), 
                //Өршөөгөөрэй
                new HyphenateTest.TestParams("mn", "\u04E8\u0440\u0448\u04E9\u04E9\u0433\u04E9\u04E9\u0440\u044D\u0439"), 
                //नमस्कार
                new HyphenateTest.TestParams("mr", "\u0928\u092E\u0938\u094D\u0915\u093E\u0930"), new HyphenateTest.TestParams
            ("nb"), new HyphenateTest.TestParams("nl"), new HyphenateTest.TestParams("nn"), new HyphenateTest.TestParams
            ("no"), 
                //ନମସ୍କାର
                new HyphenateTest.TestParams("or", "\u0B28\u0B2E\u0B38\u0B4D\u0B15\u0B3E\u0B30"), 
                //ਨਮਸਕਾਰ
                new HyphenateTest.TestParams("pa", "\u0A28\u0A2E\u0A38\u0A15\u0A3E\u0A30"), new HyphenateTest.TestParams("pl"
            ), new HyphenateTest.TestParams("pt"), new HyphenateTest.TestParams("ro"), 
                //здравствуй
                new HyphenateTest.TestParams("ru", "\u0437\u0434\u0440\u0430\u0432\u0441\u0442\u0432\u0443\u0439"), new HyphenateTest.TestParams
            ("sa"), new HyphenateTest.TestParams("sk"), new HyphenateTest.TestParams("sl"), 
                //Добродошли
                new HyphenateTest.TestParams("sr_Cyrl", "\u0414\u043E\u0431\u0440\u043E\u0434\u043E\u0448\u043B\u0438"), new 
            HyphenateTest.TestParams("sr_Latn"), 
                //Välkommen
                new HyphenateTest.TestParams("sv", "V\u00E4lkommen"), 
                //வாருங்கள்
                new HyphenateTest.TestParams("ta", "\u0BB5\u0BBE\u0BB0\u0BC1\u0B99\u0BCD\u0B95\u0BB3\u0BCD"), 
                //సుస్వాగతం
                new HyphenateTest.TestParams("te", "\u0C38\u0C41\u0C38\u0C4D\u0C35\u0C3E\u0C17\u0C24\u0C02"), new HyphenateTest.TestParams
            ("tk"), new HyphenateTest.TestParams("tr", "Merhaba"), 
                //здравствуй
                new HyphenateTest.TestParams("uk", "\u0437\u0434\u0440\u0430\u0432\u0441\u0442\u0432\u0443\u0439"), new HyphenateTest.TestParams
            ("zh_Latn"));

        private IList<String> errors = new List<String>();

        [NUnit.Framework.Test]
        public virtual void RunTest() {
            // This test can't be sped up because it uses of a lot of input data
            foreach (HyphenateTest.TestParams param in @params) {
                TryHyphenate(param.lang, param.testWorld, param.shouldPass);
            }
            NUnit.Framework.Assert.IsTrue(errors.IsEmpty(), BuildReport());
        }

        private void TryHyphenate(String lang, String testWorld, bool shouldPass) {
            String[] parts = iText.Commons.Utils.StringUtil.Split(lang, "_");
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
