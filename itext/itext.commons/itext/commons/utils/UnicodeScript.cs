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
namespace iText.Commons.Utils {
    public enum UnicodeScript {
        /**
        * Unicode script "Common".
        */
        COMMON,

        /**
         * Unicode script "Latin".
         */
        LATIN,

        /**
         * Unicode script "Greek".
         */
        GREEK,

        /**
         * Unicode script "Cyrillic".
         */
        CYRILLIC,

        /**
         * Unicode script "Armenian".
         */
        ARMENIAN,

        /**
         * Unicode script "Hebrew".
         */
        HEBREW,

        /**
         * Unicode script "Arabic".
         */
        ARABIC,

        /**
         * Unicode script "Syriac".
         */
        SYRIAC,

        /**
         * Unicode script "Thaana".
         */
        THAANA,

        /**
         * Unicode script "Devanagari".
         */
        DEVANAGARI,

        /**
         * Unicode script "Bengali".
         */
        BENGALI,

        /**
         * Unicode script "Gurmukhi".
         */
        GURMUKHI,

        /**
         * Unicode script "Gujarati".
         */
        GUJARATI,

        /**
         * Unicode script "Oriya".
         */
        ORIYA,

        /**
         * Unicode script "Tamil".
         */
        TAMIL,

        /**
         * Unicode script "Telugu".
         */
        TELUGU,

        /**
         * Unicode script "Kannada".
         */
        KANNADA,

        /**
         * Unicode script "Malayalam".
         */
        MALAYALAM,

        /**
         * Unicode script "Sinhala".
         */
        SINHALA,

        /**
         * Unicode script "Thai".
         */
        THAI,

        /**
         * Unicode script "Lao".
         */
        LAO,

        /**
         * Unicode script "Tibetan".
         */
        TIBETAN,

        /**
         * Unicode script "Myanmar".
         */
        MYANMAR,

        /**
         * Unicode script "Georgian".
         */
        GEORGIAN,

        /**
         * Unicode script "Hangul".
         */
        HANGUL,

        /**
         * Unicode script "Ethiopic".
         */
        ETHIOPIC,

        /**
         * Unicode script "Cherokee".
         */
        CHEROKEE,

        /**
         * Unicode script "Canadian_Aboriginal".
         */
        CANADIAN_ABORIGINAL,

        /**
         * Unicode script "Ogham".
         */
        OGHAM,

        /**
         * Unicode script "Runic".
         */
        RUNIC,

        /**
         * Unicode script "Khmer".
         */
        KHMER,

        /**
         * Unicode script "Mongolian".
         */
        MONGOLIAN,

        /**
         * Unicode script "Hiragana".
         */
        HIRAGANA,

        /**
         * Unicode script "Katakana".
         */
        KATAKANA,

        /**
         * Unicode script "Bopomofo".
         */
        BOPOMOFO,

        /**
         * Unicode script "Han".
         */
        HAN,

        /**
         * Unicode script "Yi".
         */
        YI,

        /**
         * Unicode script "Old_Italic".
         */
        OLD_ITALIC,

        /**
         * Unicode script "Gothic".
         */
        GOTHIC,

        /**
         * Unicode script "Deseret".
         */
        DESERET,

        /**
         * Unicode script "Inherited".
         */
        INHERITED,

        /**
         * Unicode script "Tagalog".
         */
        TAGALOG,

        /**
         * Unicode script "Hanunoo".
         */
        HANUNOO,

        /**
         * Unicode script "Buhid".
         */
        BUHID,

        /**
         * Unicode script "Tagbanwa".
         */
        TAGBANWA,

        /**
         * Unicode script "Limbu".
         */
        LIMBU,

        /**
         * Unicode script "Tai_Le".
         */
        TAI_LE,

        /**
         * Unicode script "Linear_B".
         */
        LINEAR_B,

        /**
         * Unicode script "Ugaritic".
         */
        UGARITIC,

        /**
         * Unicode script "Shavian".
         */
        SHAVIAN,

        /**
         * Unicode script "Osmanya".
         */
        OSMANYA,

        /**
         * Unicode script "Cypriot".
         */
        CYPRIOT,

        /**
         * Unicode script "Braille".
         */
        BRAILLE,

        /**
         * Unicode script "Buginese".
         */
        BUGINESE,

        /**
         * Unicode script "Coptic".
         */
        COPTIC,

        /**
         * Unicode script "New_Tai_Lue".
         */
        NEW_TAI_LUE,

        /**
         * Unicode script "Glagolitic".
         */
        GLAGOLITIC,

        /**
         * Unicode script "Tifinagh".
         */
        TIFINAGH,

        /**
         * Unicode script "Syloti_Nagri".
         */
        SYLOTI_NAGRI,

        /**
         * Unicode script "Old_Persian".
         */
        OLD_PERSIAN,

        /**
         * Unicode script "Kharoshthi".
         */
        KHAROSHTHI,

        /**
         * Unicode script "Balinese".
         */
        BALINESE,

        /**
         * Unicode script "Cuneiform".
         */
        CUNEIFORM,

        /**
         * Unicode script "Phoenician".
         */
        PHOENICIAN,

        /**
         * Unicode script "Phags_Pa".
         */
        PHAGS_PA,

        /**
         * Unicode script "Nko".
         */
        NKO,

        /**
         * Unicode script "Sundanese".
         */
        SUNDANESE,

        /**
         * Unicode script "Batak".
         */
        BATAK,

        /**
         * Unicode script "Lepcha".
         */
        LEPCHA,

        /**
         * Unicode script "Ol_Chiki".
         */
        OL_CHIKI,

        /**
         * Unicode script "Vai".
         */
        VAI,

        /**
         * Unicode script "Saurashtra".
         */
        SAURASHTRA,

        /**
         * Unicode script "Kayah_Li".
         */
        KAYAH_LI,

        /**
         * Unicode script "Rejang".
         */
        REJANG,

        /**
         * Unicode script "Lycian".
         */
        LYCIAN,

        /**
         * Unicode script "Carian".
         */
        CARIAN,

        /**
         * Unicode script "Lydian".
         */
        LYDIAN,

        /**
         * Unicode script "Cham".
         */
        CHAM,

        /**
         * Unicode script "Tai_Tham".
         */
        TAI_THAM,

        /**
         * Unicode script "Tai_Viet".
         */
        TAI_VIET,

        /**
         * Unicode script "Avestan".
         */
        AVESTAN,

        /**
         * Unicode script "Egyptian_Hieroglyphs".
         */
        EGYPTIAN_HIEROGLYPHS,

        /**
         * Unicode script "Samaritan".
         */
        SAMARITAN,

        /**
         * Unicode script "Mandaic".
         */
        MANDAIC,

        /**
         * Unicode script "Lisu".
         */
        LISU,

        /**
         * Unicode script "Bamum".
         */
        BAMUM,

        /**
         * Unicode script "Javanese".
         */
        JAVANESE,

        /**
         * Unicode script "Meetei_Mayek".
         */
        MEETEI_MAYEK,

        /**
         * Unicode script "Imperial_Aramaic".
         */
        IMPERIAL_ARAMAIC,

        /**
         * Unicode script "Old_South_Arabian".
         */
        OLD_SOUTH_ARABIAN,

        /**
         * Unicode script "Inscriptional_Parthian".
         */
        INSCRIPTIONAL_PARTHIAN,

        /**
         * Unicode script "Inscriptional_Pahlavi".
         */
        INSCRIPTIONAL_PAHLAVI,

        /**
         * Unicode script "Old_Turkic".
         */
        OLD_TURKIC,

        /**
         * Unicode script "Brahmi".
         */
        BRAHMI,

        /**
         * Unicode script "Kaithi".
         */
        KAITHI,

        /**
         * Unicode script "Meroitic Hieroglyphs".
         */
        MEROITIC_HIEROGLYPHS,

        /**
         * Unicode script "Meroitic Cursive".
         */
        MEROITIC_CURSIVE,

        /**
         * Unicode script "Sora Sompeng".
         */
        SORA_SOMPENG,

        /**
         * Unicode script "Chakma".
         */
        CHAKMA,

        /**
         * Unicode script "Sharada".
         */
        SHARADA,

        /**
         * Unicode script "Takri".
         */
        TAKRI,

        /**
         * Unicode script "Miao".
         */
        MIAO,

        /**
         * Unicode script "Unknown".
         */
        UNKNOWN
    }
}
