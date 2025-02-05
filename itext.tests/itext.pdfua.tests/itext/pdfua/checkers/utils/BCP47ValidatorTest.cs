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
using iText.Test;

namespace iText.Pdfua.Checkers.Utils {
    [NUnit.Framework.Category("UnitTest")]
    public class BCP47ValidatorTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void SimpleLanguageSubtagTest() {
            NUnit.Framework.Assert.IsTrue(BCP47Validator.Validate("de"));
            NUnit.Framework.Assert.IsTrue(BCP47Validator.Validate("fr"));
            //example of a grandfathered tag
            NUnit.Framework.Assert.IsTrue(BCP47Validator.Validate("i-enochian"));
        }

        [NUnit.Framework.Test]
        public virtual void LanguageSubtagAndScriptSubtagTest() {
            //Chinese written using the Traditional Chinese script
            NUnit.Framework.Assert.IsTrue(BCP47Validator.Validate("zh-Hant"));
            //Chinese written using the Simplified Chinese script
            NUnit.Framework.Assert.IsTrue(BCP47Validator.Validate("zh-Hans"));
            //Serbian written using the Cyrillic script
            NUnit.Framework.Assert.IsTrue(BCP47Validator.Validate("sr-Cyrl"));
            //Serbian written using the Latin script
            NUnit.Framework.Assert.IsTrue(BCP47Validator.Validate("sr-Latn"));
        }

        [NUnit.Framework.Test]
        public virtual void ExtLangSubtagsAndPrimaryLangSubtagsTest() {
            //Chinese, Mandarin, Simplified script, as used in China
            NUnit.Framework.Assert.IsTrue(BCP47Validator.Validate("zh-cmn-Hans-CN"));
            //Mandarin Chinese, Simplified script, as used in China
            NUnit.Framework.Assert.IsTrue(BCP47Validator.Validate("cmn-Hans-CN"));
            //Chinese, Cantonese, as used in Hong Kong SAR
            NUnit.Framework.Assert.IsTrue(BCP47Validator.Validate("zh-yue-HK"));
            NUnit.Framework.Assert.IsTrue(BCP47Validator.Validate("sr-Latn"));
        }

        [NUnit.Framework.Test]
        public virtual void LanguageScriptRegionsTest() {
            //Chinese written using the Simplified script as used in mainland China
            NUnit.Framework.Assert.IsTrue(BCP47Validator.Validate("zh-Hans-CN"));
            //Serbian written using the Latin script as used in Serbia
            NUnit.Framework.Assert.IsTrue(BCP47Validator.Validate("sr-Latn-RS"));
        }

        [NUnit.Framework.Test]
        public virtual void LanguageVariantTest() {
            //Resian dialect of Slovenian
            NUnit.Framework.Assert.IsTrue(BCP47Validator.Validate("sl-rozaj"));
            //San Giorgio dialect of Resian dialect of Slovenian
            NUnit.Framework.Assert.IsTrue(BCP47Validator.Validate("sl-rozaj-biske"));
            //Nadiza dialect of Slovenian
            NUnit.Framework.Assert.IsTrue(BCP47Validator.Validate("sl-nedis"));
        }

        [NUnit.Framework.Test]
        public virtual void LanguageRegionVariantTest() {
            //German as used in Switzerland using the 1901 variant [orthography]
            NUnit.Framework.Assert.IsTrue(BCP47Validator.Validate("de-CH-1901"));
            //Slovenian as used in Italy, Nadiza dialect
            NUnit.Framework.Assert.IsTrue(BCP47Validator.Validate("sl-IT-nedis"));
        }

        [NUnit.Framework.Test]
        public virtual void LanguageScriptRegionVariantTest() {
            //Eastern Armenian written in Latin script, as used in Italy
            NUnit.Framework.Assert.IsTrue(BCP47Validator.Validate("hy-Latn-IT-arevela"));
        }

        [NUnit.Framework.Test]
        public virtual void LanguageRegionTest() {
            //German for Germany
            NUnit.Framework.Assert.IsTrue(BCP47Validator.Validate("de-DE"));
            //English as used in the United States
            NUnit.Framework.Assert.IsTrue(BCP47Validator.Validate("en-US"));
            //Spanish appropriate for the Latin America and Caribbean region using the UN region code
            NUnit.Framework.Assert.IsTrue(BCP47Validator.Validate("es-419"));
            //Invalid, two region tags
            NUnit.Framework.Assert.IsFalse(BCP47Validator.Validate("de-419-DE"));
            //use of a single-character subtag in primary position; note
            //that there are a few grandfathered tags that start with "i-" that
            //are valid
            NUnit.Framework.Assert.IsFalse(BCP47Validator.Validate("a-DE"));
        }

        [NUnit.Framework.Test]
        public virtual void PrivateUseSubtagsTest() {
            NUnit.Framework.Assert.IsTrue(BCP47Validator.Validate("de-CH-x-phonebk"));
            NUnit.Framework.Assert.IsTrue(BCP47Validator.Validate("az-Arab-x-AZE-derbend"));
        }

        [NUnit.Framework.Test]
        public virtual void PrivateUseRegistryValuesTest() {
            //private use using the singleton 'x'
            NUnit.Framework.Assert.IsTrue(BCP47Validator.Validate("x-whatever"));
            //all private tags
            NUnit.Framework.Assert.IsTrue(BCP47Validator.Validate("qaa-Qaaa-QM-x-southern"));
            //German, with a private script
            NUnit.Framework.Assert.IsTrue(BCP47Validator.Validate("de-Qaaa"));
            //Serbian, Latin script, private region
            NUnit.Framework.Assert.IsTrue(BCP47Validator.Validate("sr-Latn-QM"));
            //Serbian, private script, for Serbia
            NUnit.Framework.Assert.IsTrue(BCP47Validator.Validate("sr-Qaaa-RS"));
        }

        [NUnit.Framework.Test]
        public virtual void TagsWithExtensions() {
            NUnit.Framework.Assert.IsTrue(BCP47Validator.Validate("en-US-u-islamcal"));
            NUnit.Framework.Assert.IsTrue(BCP47Validator.Validate("zh-CN-a-myext-x-private"));
            NUnit.Framework.Assert.IsTrue(BCP47Validator.Validate("en-a-myext-b-another"));
        }
    }
}
