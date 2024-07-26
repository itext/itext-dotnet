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
        private IList<String> errors = new List<String>();

        public static IEnumerable<Object[]> HyphenationProperties() {
            return JavaUtil.ArraysAsList(new Object[][] { new Object[] { "African", "af", "country" }, new Object[] { 
                "Assamese", "as", "\u09A8\u09AE\u09B8\u09CD\u0995\u09BE\u09F0" }, new Object[] { "Bulgarian", "bg", "\u0417\u0434\u0440\u0430\u0432\u0435\u0439"
                 }, new Object[] { "Bengali", "bn", "\u0986\u09B2\u09BE\u0987\u0995\u09C1\u09AE" }, new Object[] { "Catalan"
                , "ca", "Benvinguts" }, new Object[] { "Coptic", "cop", "\u2C98\u2C89\u2CA7\u2CA2\u2C89\u2C99\u0300\u2C9B\u2CAD\u2C8F\u2C99\u2C93"
                 }, new Object[] { "Czech", "cs", "country" }, new Object[] { "Welsh", "cy", "country" }, new Object[]
                 { "Danish", "da", "country" }, new Object[] { "German", "de", "country" }, new Object[] { "German Belgium"
                , "de_DE", "14\u00a0Tagen 14\u00a0Tagen 14\u00a0Tagen " }, new Object[] { "German Germany", "de_DE", "14\u20110Tagen 14\u2011Tagen 14\u20110Tagen "
                 }, new Object[] { "German Traditional", "de_1901", "country" }, new Object[] { "Swiss German", "de_CH"
                , "country" }, new Object[] { "New German orthography", "de_DR", "country" }, new Object[] { "Modern Greek"
                , "el", "\u03BA\u03B1\u03BB\u03B7\u03BC\u03AD\u03C1\u03B1" }, new Object[] { "Greek Polytonic", "el_Polyton"
                , "\u03BA\u03B1\u03BB\u03B7\u03BC\u03AD\u03C1\u03B1" }, new Object[] { "English", "en", "country" }, new 
                Object[] { "English Great Britain", "en_GB", "country" }, new Object[] { "English United States", "en_US"
                , "country" }, new Object[] { "Esperanto", "eo", "country" }, new Object[] { "Spanish", "es", "gracias"
                 }, new Object[] { "Estonian", "et", "Vabandust" }, new Object[] { "Basque", "eu", "euskara" }, new Object
                [] { "Finnish", "fi", "N\u00E4kemiin" }, new Object[] { "French", "fr", "country" }, new Object[] { "Irish"
                , "ga", "country" }, new Object[] { "Galician", "gl", "country" }, new Object[] { "Ancient Greek", "grc"
                , "\u03BA\u03B1\u03BB\u03B7\u03BC\u03AD\u03C1\u03B1" }, new Object[] { "Gujarati", "gu", "\u0A97\u0AC1\u0A9C\u0AB0\u0ABE\u0AA4\u0AC0"
                 }, new Object[] { "Hindi", "hi", "\u0938\u0941\u092A\u094D\u0930\u092D\u093E\u0924\u092E\u094D" }, new 
                Object[] { "Croatian", "hr", "country" }, new Object[] { "Upper Sorbian", "hsb", "country" }, new Object
                [] { "Hungarian", "hu", "sziasztok" }, new Object[] { "Armenian", "hy", "\u0577\u0576\u0578\u0580\u0570\u0561\u056F\u0561\u056C\u0578\u0582\u0569\u0575\u0578\u0582\u0576"
                 }, new Object[] { "Interlingua", "ia", "country" }, new Object[] { "Indonesian", "id", "country" }, new 
                Object[] { "Icelandic", "is", "country" }, new Object[] { "Italian", "it", "country" }, new Object[] { 
                "Kurmanji", "kmr", "country" }, new Object[] { "Kannada", "kn", "\u0C95\u0CA8\u0CCD\u0CA8\u0CA1" }, new 
                Object[] { "Latin", "la", "country" }, new Object[] { "Lao", "lo", "\u0E8D\u0EB4\u0E99\u0E94\u0EB5\u0E95\u0EC9\u0EAD\u0E99\u0EAE\u0EB1\u0E9A"
                 }, new Object[] { "Lithuanian", "lt", "Labanakt" }, new Object[] { "Latvian", "lv", "Labvakar" }, new 
                Object[] { "Malayalam", "ml", "\u0D38\u0D4D\u0D35\u0D3E\u0D17\u0D24\u0D02" }, new Object[] { "Mongolian"
                , "mn", "\u04E8\u0440\u0448\u04E9\u04E9\u0433\u04E9\u04E9\u0440\u044D\u0439" }, new Object[] { "Marathi"
                , "mr", "\u0928\u092E\u0938\u094D\u0915\u093E\u0930" }, new Object[] { "Norwegian Bokmål", "nb", "country"
                 }, new Object[] { "Dutch; Flemish", "nl", "country" }, new Object[] { "Norwegian Nynorsk", "nn", "country"
                 }, new Object[] { "Norwegian", "no", "country" }, new Object[] { "Oriya", "or", "\u0B28\u0B2E\u0B38\u0B4D\u0B15\u0B3E\u0B30"
                 }, new Object[] { "Panjabi; Punjabi", "pa", "\u0A28\u0A2E\u0A38\u0A15\u0A3E\u0A30" }, new Object[] { 
                "Polish", "pl", "country" }, new Object[] { "Portuguese", "pt", "country" }, new Object[] { "Romanian; Moldavian; Moldovan"
                , "ro", "country" }, new Object[] { "Russian", "ru", "\u0437\u0434\u0440\u0430\u0432\u0441\u0442\u0432\u0443\u0439"
                 }, new Object[] { "Sanskrit", "sa", "country" }, new Object[] { "Slovak", "sk", "country" }, new Object
                [] { "Slovenian", "sl", "country" }, new Object[] { "Serbian Cyrillic", "sr_Cyrl", "\u0414\u043E\u0431\u0440\u043E\u0434\u043E\u0448\u043B\u0438"
                 }, new Object[] { "Serbian Latin", "sr_Latn", "country" }, new Object[] { "Swedish", "sv", "V\u00E4lkommen"
                 }, new Object[] { "Tamil", "ta", "\u0BB5\u0BBE\u0BB0\u0BC1\u0B99\u0BCD\u0B95\u0BB3\u0BCD" }, new Object
                [] { "Telugu", "te", "\u0C38\u0C41\u0C38\u0C4D\u0C35\u0C3E\u0C17\u0C24\u0C02" }, new Object[] { "Turkmen"
                , "tk", "country" }, new Object[] { "Turkish", "tr", "Merhaba" }, new Object[] { "Ukrainian", "uk", "\u0437\u0434\u0440\u0430\u0432\u0441\u0442\u0432\u0443\u0439"
                 }, new Object[] { "Chinese Latin", "zh_Latn", "country" } });
        }

        [NUnit.Framework.TestCaseSource("HyphenationProperties")]
        public virtual void RunTest(String name, String lang, String testWord) {
            errors.Clear();
            TryHyphenate(lang, testWord);
            NUnit.Framework.Assert.IsTrue(errors.IsEmpty(), BuildReport());
        }

        private void TryHyphenate(String lang, String testWorld) {
            String[] parts = iText.Commons.Utils.StringUtil.Split(lang, "_");
            lang = parts[0];
            String country = (parts.Length == 2) ? parts[1] : null;
            HyphenationConfig config = new HyphenationConfig(lang, country, 3, 3);
            iText.Layout.Hyphenation.Hyphenation result = config.Hyphenate(testWorld);
            if (result == null) {
                errors.Add(MessageFormatUtil.Format("\nLanguage: {0}, error on hyphenate({1})", lang, testWorld));
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
    }
}
