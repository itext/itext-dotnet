/*

This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: Bruno Lowagie, Paulo Soares, et al.

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
