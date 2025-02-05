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
using System.Globalization;

namespace iText.Commons.Utils {
    /// <summary>
    /// This file is a helper class for internal usage only.
    /// Be aware that its API and functionality may be changed in future.
    /// </summary>
    public static class UnicodeScriptUtil {
        public const int MAX_CODE_POINT = 0X10FFFF;

        private static readonly UnicodeScript[] Scripts = {
            UnicodeScript.COMMON,
            UnicodeScript.LATIN,
            UnicodeScript.COMMON,
            UnicodeScript.LATIN,
            UnicodeScript.COMMON,
            UnicodeScript.LATIN,
            UnicodeScript.COMMON,
            UnicodeScript.LATIN,
            UnicodeScript.COMMON,
            UnicodeScript.LATIN,
            UnicodeScript.COMMON,
            UnicodeScript.LATIN,
            UnicodeScript.COMMON,
            UnicodeScript.LATIN,
            UnicodeScript.COMMON,
            UnicodeScript.LATIN,
            UnicodeScript.COMMON,
            UnicodeScript.BOPOMOFO,
            UnicodeScript.COMMON,
            UnicodeScript.INHERITED,
            UnicodeScript.GREEK,
            UnicodeScript.COMMON,
            UnicodeScript.GREEK,
            UnicodeScript.COMMON,
            UnicodeScript.GREEK,
            UnicodeScript.COMMON,
            UnicodeScript.GREEK,
            UnicodeScript.COMMON,
            UnicodeScript.GREEK,
            UnicodeScript.COPTIC,
            UnicodeScript.GREEK,
            UnicodeScript.CYRILLIC,
            UnicodeScript.INHERITED,
            UnicodeScript.CYRILLIC,
            UnicodeScript.ARMENIAN,
            UnicodeScript.COMMON,
            UnicodeScript.ARMENIAN,
            UnicodeScript.HEBREW,
            UnicodeScript.ARABIC,
            UnicodeScript.COMMON,
            UnicodeScript.ARABIC,
            UnicodeScript.COMMON,
            UnicodeScript.ARABIC,
            UnicodeScript.COMMON,
            UnicodeScript.ARABIC,
            UnicodeScript.COMMON,
            UnicodeScript.ARABIC,
            UnicodeScript.INHERITED,
            UnicodeScript.ARABIC,
            UnicodeScript.COMMON,
            UnicodeScript.ARABIC,
            UnicodeScript.INHERITED,
            UnicodeScript.ARABIC,
            UnicodeScript.COMMON,
            UnicodeScript.ARABIC,
            UnicodeScript.SYRIAC,
            UnicodeScript.ARABIC,
            UnicodeScript.THAANA,
            UnicodeScript.NKO,
            UnicodeScript.SAMARITAN,
            UnicodeScript.MANDAIC,
            UnicodeScript.ARABIC,
            UnicodeScript.DEVANAGARI,
            UnicodeScript.INHERITED,
            UnicodeScript.DEVANAGARI,
            UnicodeScript.COMMON,
            UnicodeScript.DEVANAGARI,
            UnicodeScript.BENGALI,
            UnicodeScript.GURMUKHI,
            UnicodeScript.GUJARATI,
            UnicodeScript.ORIYA,
            UnicodeScript.TAMIL,
            UnicodeScript.TELUGU,
            UnicodeScript.KANNADA,
            UnicodeScript.MALAYALAM,
            UnicodeScript.SINHALA,
            UnicodeScript.THAI,
            UnicodeScript.COMMON,
            UnicodeScript.THAI,
            UnicodeScript.LAO,
            UnicodeScript.TIBETAN,
            UnicodeScript.COMMON,
            UnicodeScript.TIBETAN,
            UnicodeScript.MYANMAR,
            UnicodeScript.GEORGIAN,
            UnicodeScript.COMMON,
            UnicodeScript.GEORGIAN,
            UnicodeScript.HANGUL,
            UnicodeScript.ETHIOPIC,
            UnicodeScript.CHEROKEE,
            UnicodeScript.CANADIAN_ABORIGINAL,
            UnicodeScript.OGHAM,
            UnicodeScript.RUNIC,
            UnicodeScript.COMMON,
            UnicodeScript.RUNIC,
            UnicodeScript.TAGALOG,
            UnicodeScript.HANUNOO,
            UnicodeScript.COMMON,
            UnicodeScript.BUHID,
            UnicodeScript.TAGBANWA,
            UnicodeScript.KHMER,
            UnicodeScript.MONGOLIAN,
            UnicodeScript.COMMON,
            UnicodeScript.MONGOLIAN,
            UnicodeScript.COMMON,
            UnicodeScript.MONGOLIAN,
            UnicodeScript.CANADIAN_ABORIGINAL,
            UnicodeScript.LIMBU,
            UnicodeScript.TAI_LE,
            UnicodeScript.NEW_TAI_LUE,
            UnicodeScript.KHMER,
            UnicodeScript.BUGINESE,
            UnicodeScript.TAI_THAM,
            UnicodeScript.BALINESE,
            UnicodeScript.SUNDANESE,
            UnicodeScript.BATAK,
            UnicodeScript.LEPCHA,
            UnicodeScript.OL_CHIKI,
            UnicodeScript.SUNDANESE,
            UnicodeScript.INHERITED,
            UnicodeScript.COMMON,
            UnicodeScript.INHERITED,
            UnicodeScript.COMMON,
            UnicodeScript.INHERITED,
            UnicodeScript.COMMON,
            UnicodeScript.INHERITED,
            UnicodeScript.COMMON,
            UnicodeScript.INHERITED,
            UnicodeScript.COMMON,
            UnicodeScript.LATIN,
            UnicodeScript.GREEK,
            UnicodeScript.CYRILLIC,
            UnicodeScript.LATIN,
            UnicodeScript.GREEK,
            UnicodeScript.LATIN,
            UnicodeScript.GREEK,
            UnicodeScript.LATIN,
            UnicodeScript.CYRILLIC,
            UnicodeScript.LATIN,
            UnicodeScript.GREEK,
            UnicodeScript.INHERITED,
            UnicodeScript.LATIN,
            UnicodeScript.GREEK,
            UnicodeScript.COMMON,
            UnicodeScript.INHERITED,
            UnicodeScript.COMMON,
            UnicodeScript.LATIN,
            UnicodeScript.COMMON,
            UnicodeScript.LATIN,
            UnicodeScript.COMMON,
            UnicodeScript.LATIN,
            UnicodeScript.COMMON,
            UnicodeScript.INHERITED,
            UnicodeScript.COMMON,
            UnicodeScript.GREEK,
            UnicodeScript.COMMON,
            UnicodeScript.LATIN,
            UnicodeScript.COMMON,
            UnicodeScript.LATIN,
            UnicodeScript.COMMON,
            UnicodeScript.LATIN,
            UnicodeScript.COMMON,
            UnicodeScript.LATIN,
            UnicodeScript.COMMON,
            UnicodeScript.BRAILLE,
            UnicodeScript.COMMON,
            UnicodeScript.GLAGOLITIC,
            UnicodeScript.LATIN,
            UnicodeScript.COPTIC,
            UnicodeScript.GEORGIAN,
            UnicodeScript.TIFINAGH,
            UnicodeScript.ETHIOPIC,
            UnicodeScript.CYRILLIC,
            UnicodeScript.COMMON,
            UnicodeScript.HAN,
            UnicodeScript.COMMON,
            UnicodeScript.HAN,
            UnicodeScript.COMMON,
            UnicodeScript.HAN,
            UnicodeScript.COMMON,
            UnicodeScript.HAN,
            UnicodeScript.INHERITED,
            UnicodeScript.HANGUL,
            UnicodeScript.COMMON,
            UnicodeScript.HAN,
            UnicodeScript.COMMON,
            UnicodeScript.HIRAGANA,
            UnicodeScript.INHERITED,
            UnicodeScript.COMMON,
            UnicodeScript.HIRAGANA,
            UnicodeScript.COMMON,
            UnicodeScript.KATAKANA,
            UnicodeScript.COMMON,
            UnicodeScript.KATAKANA,
            UnicodeScript.BOPOMOFO,
            UnicodeScript.HANGUL,
            UnicodeScript.COMMON,
            UnicodeScript.BOPOMOFO,
            UnicodeScript.COMMON,
            UnicodeScript.KATAKANA,
            UnicodeScript.HANGUL,
            UnicodeScript.COMMON,
            UnicodeScript.HANGUL,
            UnicodeScript.COMMON,
            UnicodeScript.KATAKANA,
            UnicodeScript.COMMON,
            UnicodeScript.HAN,
            UnicodeScript.COMMON,
            UnicodeScript.HAN,
            UnicodeScript.YI,
            UnicodeScript.LISU,
            UnicodeScript.VAI,
            UnicodeScript.CYRILLIC,
            UnicodeScript.BAMUM,
            UnicodeScript.COMMON,
            UnicodeScript.LATIN,
            UnicodeScript.COMMON,
            UnicodeScript.LATIN,
            UnicodeScript.SYLOTI_NAGRI,
            UnicodeScript.COMMON,
            UnicodeScript.PHAGS_PA,
            UnicodeScript.SAURASHTRA,
            UnicodeScript.DEVANAGARI,
            UnicodeScript.KAYAH_LI,
            UnicodeScript.REJANG,
            UnicodeScript.HANGUL,
            UnicodeScript.JAVANESE,
            UnicodeScript.CHAM,
            UnicodeScript.MYANMAR,
            UnicodeScript.TAI_VIET,
            UnicodeScript.MEETEI_MAYEK,
            UnicodeScript.ETHIOPIC,
            UnicodeScript.MEETEI_MAYEK,
            UnicodeScript.HANGUL,
            UnicodeScript.UNKNOWN,
            UnicodeScript.HAN,
            UnicodeScript.LATIN,
            UnicodeScript.ARMENIAN,
            UnicodeScript.HEBREW,
            UnicodeScript.ARABIC,
            UnicodeScript.COMMON,
            UnicodeScript.ARABIC,
            UnicodeScript.COMMON,
            UnicodeScript.INHERITED,
            UnicodeScript.COMMON,
            UnicodeScript.INHERITED,
            UnicodeScript.COMMON,
            UnicodeScript.ARABIC,
            UnicodeScript.COMMON,
            UnicodeScript.LATIN,
            UnicodeScript.COMMON,
            UnicodeScript.LATIN,
            UnicodeScript.COMMON,
            UnicodeScript.KATAKANA,
            UnicodeScript.COMMON,
            UnicodeScript.KATAKANA,
            UnicodeScript.COMMON,
            UnicodeScript.HANGUL,
            UnicodeScript.COMMON,
            UnicodeScript.LINEAR_B,
            UnicodeScript.COMMON,
            UnicodeScript.GREEK,
            UnicodeScript.COMMON,
            UnicodeScript.INHERITED,
            UnicodeScript.LYCIAN,
            UnicodeScript.CARIAN,
            UnicodeScript.OLD_ITALIC,
            UnicodeScript.GOTHIC,
            UnicodeScript.UGARITIC,
            UnicodeScript.OLD_PERSIAN,
            UnicodeScript.DESERET,
            UnicodeScript.SHAVIAN,
            UnicodeScript.OSMANYA,
            UnicodeScript.CYPRIOT,
            UnicodeScript.IMPERIAL_ARAMAIC,
            UnicodeScript.PHOENICIAN,
            UnicodeScript.LYDIAN,
            UnicodeScript.MEROITIC_HIEROGLYPHS,
            UnicodeScript.MEROITIC_CURSIVE,
            UnicodeScript.KHAROSHTHI,
            UnicodeScript.OLD_SOUTH_ARABIAN,
            UnicodeScript.AVESTAN,
            UnicodeScript.INSCRIPTIONAL_PARTHIAN,
            UnicodeScript.INSCRIPTIONAL_PAHLAVI,
            UnicodeScript.OLD_TURKIC,
            UnicodeScript.ARABIC,
            UnicodeScript.BRAHMI,
            UnicodeScript.KAITHI,
            UnicodeScript.SORA_SOMPENG,
            UnicodeScript.CHAKMA,
            UnicodeScript.SHARADA,
            UnicodeScript.TAKRI,
            UnicodeScript.CUNEIFORM,
            UnicodeScript.EGYPTIAN_HIEROGLYPHS,
            UnicodeScript.BAMUM,
            UnicodeScript.MIAO,
            UnicodeScript.KATAKANA,
            UnicodeScript.HIRAGANA,
            UnicodeScript.COMMON,
            UnicodeScript.INHERITED,
            UnicodeScript.COMMON,
            UnicodeScript.INHERITED,
            UnicodeScript.COMMON,
            UnicodeScript.INHERITED,
            UnicodeScript.COMMON,
            UnicodeScript.INHERITED,
            UnicodeScript.COMMON,
            UnicodeScript.GREEK,
            UnicodeScript.COMMON,
            UnicodeScript.ARABIC,
            UnicodeScript.COMMON,
            UnicodeScript.HIRAGANA,
            UnicodeScript.COMMON,
            UnicodeScript.HAN,
            UnicodeScript.COMMON,
            UnicodeScript.INHERITED,
            UnicodeScript.UNKNOWN
        };

        public static readonly int[] ScriptStarts = {
            0x0000, // 0000..0040; COMMON
            0x0041, // 0041..005A; LATIN
            0x005B, // 005B..0060; COMMON
            0x0061, // 0061..007A; LATIN
            0x007B, // 007B..00A9; COMMON
            0x00AA, // 00AA..00AA; LATIN
            0x00AB, // 00AB..00B9; COMMON
            0x00BA, // 00BA..00BA; LATIN
            0x00BB, // 00BB..00BF; COMMON
            0x00C0, // 00C0..00D6; LATIN
            0x00D7, // 00D7..00D7; COMMON
            0x00D8, // 00D8..00F6; LATIN
            0x00F7, // 00F7..00F7; COMMON
            0x00F8, // 00F8..02B8; LATIN
            0x02B9, // 02B9..02DF; COMMON
            0x02E0, // 02E0..02E4; LATIN
            0x02E5, // 02E5..02E9; COMMON
            0x02EA, // 02EA..02EB; BOPOMOFO
            0x02EC, // 02EC..02FF; COMMON
            0x0300, // 0300..036F; INHERITED
            0x0370, // 0370..0373; GREEK
            0x0374, // 0374..0374; COMMON
            0x0375, // 0375..037D; GREEK
            0x037E, // 037E..0383; COMMON
            0x0384, // 0384..0384; GREEK
            0x0385, // 0385..0385; COMMON
            0x0386, // 0386..0386; GREEK
            0x0387, // 0387..0387; COMMON
            0x0388, // 0388..03E1; GREEK
            0x03E2, // 03E2..03EF; COPTIC
            0x03F0, // 03F0..03FF; GREEK
            0x0400, // 0400..0484; CYRILLIC
            0x0485, // 0485..0486; INHERITED
            0x0487, // 0487..0530; CYRILLIC
            0x0531, // 0531..0588; ARMENIAN
            0x0589, // 0589..0589; COMMON
            0x058A, // 058A..0590; ARMENIAN
            0x0591, // 0591..05FF; HEBREW
            0x0600, // 0600..060B; ARABIC
            0x060C, // 060C..060C; COMMON
            0x060D, // 060D..061A; ARABIC
            0x061B, // 061B..061D; COMMON
            0x061E, // 061E..061E; ARABIC
            0x061F, // 061F..061F; COMMON
            0x0620, // 0620..063F; ARABIC
            0x0640, // 0640..0640; COMMON
            0x0641, // 0641..064A; ARABIC
            0x064B, // 064B..0655; INHERITED
            0x0656, // 0656..065F; ARABIC
            0x0660, // 0660..0669; COMMON
            0x066A, // 066A..066F; ARABIC
            0x0670, // 0670..0670; INHERITED
            0x0671, // 0671..06DC; ARABIC
            0x06DD, // 06DD..06DD; COMMON
            0x06DE, // 06DE..06FF; ARABIC
            0x0700, // 0700..074F; SYRIAC
            0x0750, // 0750..077F; ARABIC
            0x0780, // 0780..07BF; THAANA
            0x07C0, // 07C0..07FF; NKO
            0x0800, // 0800..083F; SAMARITAN
            0x0840, // 0840..089F; MANDAIC
            0x08A0, // 08A0..08FF; ARABIC
            0x0900, // 0900..0950; DEVANAGARI
            0x0951, // 0951..0952; INHERITED
            0x0953, // 0953..0963; DEVANAGARI
            0x0964, // 0964..0965; COMMON
            0x0966, // 0966..0980; DEVANAGARI
            0x0981, // 0981..0A00; BENGALI
            0x0A01, // 0A01..0A80; GURMUKHI
            0x0A81, // 0A81..0B00; GUJARATI
            0x0B01, // 0B01..0B81; ORIYA
            0x0B82, // 0B82..0C00; TAMIL
            0x0C01, // 0C01..0C81; TELUGU
            0x0C82, // 0C82..0CF0; KANNADA
            0x0D02, // 0D02..0D81; MALAYALAM
            0x0D82, // 0D82..0E00; SINHALA
            0x0E01, // 0E01..0E3E; THAI
            0x0E3F, // 0E3F..0E3F; COMMON
            0x0E40, // 0E40..0E80; THAI
            0x0E81, // 0E81..0EFF; LAO
            0x0F00, // 0F00..0FD4; TIBETAN
            0x0FD5, // 0FD5..0FD8; COMMON
            0x0FD9, // 0FD9..0FFF; TIBETAN
            0x1000, // 1000..109F; MYANMAR
            0x10A0, // 10A0..10FA; GEORGIAN
            0x10FB, // 10FB..10FB; COMMON
            0x10FC, // 10FC..10FF; GEORGIAN
            0x1100, // 1100..11FF; HANGUL
            0x1200, // 1200..139F; ETHIOPIC
            0x13A0, // 13A0..13FF; CHEROKEE
            0x1400, // 1400..167F; CANADIAN_ABORIGINAL
            0x1680, // 1680..169F; OGHAM
            0x16A0, // 16A0..16EA; RUNIC
            0x16EB, // 16EB..16ED; COMMON
            0x16EE, // 16EE..16FF; RUNIC
            0x1700, // 1700..171F; TAGALOG
            0x1720, // 1720..1734; HANUNOO
            0x1735, // 1735..173F; COMMON
            0x1740, // 1740..175F; BUHID
            0x1760, // 1760..177F; TAGBANWA
            0x1780, // 1780..17FF; KHMER
            0x1800, // 1800..1801; MONGOLIAN
            0x1802, // 1802..1803; COMMON
            0x1804, // 1804..1804; MONGOLIAN
            0x1805, // 1805..1805; COMMON
            0x1806, // 1806..18AF; MONGOLIAN
            0x18B0, // 18B0..18FF; CANADIAN_ABORIGINAL
            0x1900, // 1900..194F; LIMBU
            0x1950, // 1950..197F; TAI_LE
            0x1980, // 1980..19DF; NEW_TAI_LUE
            0x19E0, // 19E0..19FF; KHMER
            0x1A00, // 1A00..1A1F; BUGINESE
            0x1A20, // 1A20..1AFF; TAI_THAM
            0x1B00, // 1B00..1B7F; BALINESE
            0x1B80, // 1B80..1BBF; SUNDANESE
            0x1BC0, // 1BC0..1BFF; BATAK
            0x1C00, // 1C00..1C4F; LEPCHA
            0x1C50, // 1C50..1CBF; OL_CHIKI
            0x1CC0, // 1CC0..1CCF; SUNDANESE
            0x1CD0, // 1CD0..1CD2; INHERITED
            0x1CD3, // 1CD3..1CD3; COMMON
            0x1CD4, // 1CD4..1CE0; INHERITED
            0x1CE1, // 1CE1..1CE1; COMMON
            0x1CE2, // 1CE2..1CE8; INHERITED
            0x1CE9, // 1CE9..1CEC; COMMON
            0x1CED, // 1CED..1CED; INHERITED
            0x1CEE, // 1CEE..1CF3; COMMON
            0x1CF4, // 1CF4..1CF4; INHERITED
            0x1CF5, // 1CF5..1CFF; COMMON
            0x1D00, // 1D00..1D25; LATIN
            0x1D26, // 1D26..1D2A; GREEK
            0x1D2B, // 1D2B..1D2B; CYRILLIC
            0x1D2C, // 1D2C..1D5C; LATIN
            0x1D5D, // 1D5D..1D61; GREEK
            0x1D62, // 1D62..1D65; LATIN
            0x1D66, // 1D66..1D6A; GREEK
            0x1D6B, // 1D6B..1D77; LATIN
            0x1D78, // 1D78..1D78; CYRILLIC
            0x1D79, // 1D79..1DBE; LATIN
            0x1DBF, // 1DBF..1DBF; GREEK
            0x1DC0, // 1DC0..1DFF; INHERITED
            0x1E00, // 1E00..1EFF; LATIN
            0x1F00, // 1F00..1FFF; GREEK
            0x2000, // 2000..200B; COMMON
            0x200C, // 200C..200D; INHERITED
            0x200E, // 200E..2070; COMMON
            0x2071, // 2071..2073; LATIN
            0x2074, // 2074..207E; COMMON
            0x207F, // 207F..207F; LATIN
            0x2080, // 2080..208F; COMMON
            0x2090, // 2090..209F; LATIN
            0x20A0, // 20A0..20CF; COMMON
            0x20D0, // 20D0..20FF; INHERITED
            0x2100, // 2100..2125; COMMON
            0x2126, // 2126..2126; GREEK
            0x2127, // 2127..2129; COMMON
            0x212A, // 212A..212B; LATIN
            0x212C, // 212C..2131; COMMON
            0x2132, // 2132..2132; LATIN
            0x2133, // 2133..214D; COMMON
            0x214E, // 214E..214E; LATIN
            0x214F, // 214F..215F; COMMON
            0x2160, // 2160..2188; LATIN
            0x2189, // 2189..27FF; COMMON
            0x2800, // 2800..28FF; BRAILLE
            0x2900, // 2900..2BFF; COMMON
            0x2C00, // 2C00..2C5F; GLAGOLITIC
            0x2C60, // 2C60..2C7F; LATIN
            0x2C80, // 2C80..2CFF; COPTIC
            0x2D00, // 2D00..2D2F; GEORGIAN
            0x2D30, // 2D30..2D7F; TIFINAGH
            0x2D80, // 2D80..2DDF; ETHIOPIC
            0x2DE0, // 2DE0..2DFF; CYRILLIC
            0x2E00, // 2E00..2E7F; COMMON
            0x2E80, // 2E80..2FEF; HAN
            0x2FF0, // 2FF0..3004; COMMON
            0x3005, // 3005..3005; HAN
            0x3006, // 3006..3006; COMMON
            0x3007, // 3007..3007; HAN
            0x3008, // 3008..3020; COMMON
            0x3021, // 3021..3029; HAN
            0x302A, // 302A..302D; INHERITED
            0x302E, // 302E..302F; HANGUL
            0x3030, // 3030..3037; COMMON
            0x3038, // 3038..303B; HAN
            0x303C, // 303C..3040; COMMON
            0x3041, // 3041..3098; HIRAGANA
            0x3099, // 3099..309A; INHERITED
            0x309B, // 309B..309C; COMMON
            0x309D, // 309D..309F; HIRAGANA
            0x30A0, // 30A0..30A0; COMMON
            0x30A1, // 30A1..30FA; KATAKANA
            0x30FB, // 30FB..30FC; COMMON
            0x30FD, // 30FD..3104; KATAKANA
            0x3105, // 3105..3130; BOPOMOFO
            0x3131, // 3131..318F; HANGUL
            0x3190, // 3190..319F; COMMON
            0x31A0, // 31A0..31BF; BOPOMOFO
            0x31C0, // 31C0..31EF; COMMON
            0x31F0, // 31F0..31FF; KATAKANA
            0x3200, // 3200..321F; HANGUL
            0x3220, // 3220..325F; COMMON
            0x3260, // 3260..327E; HANGUL
            0x327F, // 327F..32CF; COMMON
            0x32D0, // 32D0..3357; KATAKANA
            0x3358, // 3358..33FF; COMMON
            0x3400, // 3400..4DBF; HAN
            0x4DC0, // 4DC0..4DFF; COMMON
            0x4E00, // 4E00..9FFF; HAN
            0xA000, // A000..A4CF; YI
            0xA4D0, // A4D0..A4FF; LISU
            0xA500, // A500..A63F; VAI
            0xA640, // A640..A69F; CYRILLIC
            0xA6A0, // A6A0..A6FF; BAMUM
            0xA700, // A700..A721; COMMON
            0xA722, // A722..A787; LATIN
            0xA788, // A788..A78A; COMMON
            0xA78B, // A78B..A7FF; LATIN
            0xA800, // A800..A82F; SYLOTI_NAGRI
            0xA830, // A830..A83F; COMMON
            0xA840, // A840..A87F; PHAGS_PA
            0xA880, // A880..A8DF; SAURASHTRA
            0xA8E0, // A8E0..A8FF; DEVANAGARI
            0xA900, // A900..A92F; KAYAH_LI
            0xA930, // A930..A95F; REJANG
            0xA960, // A960..A97F; HANGUL
            0xA980, // A980..A9FF; JAVANESE
            0xAA00, // AA00..AA5F; CHAM
            0xAA60, // AA60..AA7F; MYANMAR
            0xAA80, // AA80..AADF; TAI_VIET
            0xAAE0, // AAE0..AB00; MEETEI_MAYEK
            0xAB01, // AB01..ABBF; ETHIOPIC
            0xABC0, // ABC0..ABFF; MEETEI_MAYEK
            0xAC00, // AC00..D7FB; HANGUL
            0xD7FC, // D7FC..F8FF; UNKNOWN
            0xF900, // F900..FAFF; HAN
            0xFB00, // FB00..FB12; LATIN
            0xFB13, // FB13..FB1C; ARMENIAN
            0xFB1D, // FB1D..FB4F; HEBREW
            0xFB50, // FB50..FD3D; ARABIC
            0xFD3E, // FD3E..FD4F; COMMON
            0xFD50, // FD50..FDFC; ARABIC
            0xFDFD, // FDFD..FDFF; COMMON
            0xFE00, // FE00..FE0F; INHERITED
            0xFE10, // FE10..FE1F; COMMON
            0xFE20, // FE20..FE2F; INHERITED
            0xFE30, // FE30..FE6F; COMMON
            0xFE70, // FE70..FEFE; ARABIC
            0xFEFF, // FEFF..FF20; COMMON
            0xFF21, // FF21..FF3A; LATIN
            0xFF3B, // FF3B..FF40; COMMON
            0xFF41, // FF41..FF5A; LATIN
            0xFF5B, // FF5B..FF65; COMMON
            0xFF66, // FF66..FF6F; KATAKANA
            0xFF70, // FF70..FF70; COMMON
            0xFF71, // FF71..FF9D; KATAKANA
            0xFF9E, // FF9E..FF9F; COMMON
            0xFFA0, // FFA0..FFDF; HANGUL
            0xFFE0, // FFE0..FFFF; COMMON
            0x10000, // 10000..100FF; LINEAR_B
            0x10100, // 10100..1013F; COMMON
            0x10140, // 10140..1018F; GREEK
            0x10190, // 10190..101FC; COMMON
            0x101FD, // 101FD..1027F; INHERITED
            0x10280, // 10280..1029F; LYCIAN
            0x102A0, // 102A0..102FF; CARIAN
            0x10300, // 10300..1032F; OLD_ITALIC
            0x10330, // 10330..1037F; GOTHIC
            0x10380, // 10380..1039F; UGARITIC
            0x103A0, // 103A0..103FF; OLD_PERSIAN
            0x10400, // 10400..1044F; DESERET
            0x10450, // 10450..1047F; SHAVIAN
            0x10480, // 10480..107FF; OSMANYA
            0x10800, // 10800..1083F; CYPRIOT
            0x10840, // 10840..108FF; IMPERIAL_ARAMAIC
            0x10900, // 10900..1091F; PHOENICIAN
            0x10920, // 10920..1097F; LYDIAN
            0x10980, // 10980..1099F; MEROITIC_HIEROGLYPHS
            0x109A0, // 109A0..109FF; MEROITIC_CURSIVE
            0x10A00, // 10A00..10A5F; KHAROSHTHI
            0x10A60, // 10A60..10AFF; OLD_SOUTH_ARABIAN
            0x10B00, // 10B00..10B3F; AVESTAN
            0x10B40, // 10B40..10B5F; INSCRIPTIONAL_PARTHIAN
            0x10B60, // 10B60..10BFF; INSCRIPTIONAL_PAHLAVI
            0x10C00, // 10C00..10E5F; OLD_TURKIC
            0x10E60, // 10E60..10FFF; ARABIC
            0x11000, // 11000..1107F; BRAHMI
            0x11080, // 11080..110CF; KAITHI
            0x110D0, // 110D0..110FF; SORA_SOMPENG
            0x11100, // 11100..1117F; CHAKMA
            0x11180, // 11180..1167F; SHARADA
            0x11680, // 11680..116CF; TAKRI
            0x12000, // 12000..12FFF; CUNEIFORM
            0x13000, // 13000..167FF; EGYPTIAN_HIEROGLYPHS
            0x16800, // 16800..16A38; BAMUM
            0x16F00, // 16F00..16F9F; MIAO
            0x1B000, // 1B000..1B000; KATAKANA
            0x1B001, // 1B001..1CFFF; HIRAGANA
            0x1D000, // 1D000..1D166; COMMON
            0x1D167, // 1D167..1D169; INHERITED
            0x1D16A, // 1D16A..1D17A; COMMON
            0x1D17B, // 1D17B..1D182; INHERITED
            0x1D183, // 1D183..1D184; COMMON
            0x1D185, // 1D185..1D18B; INHERITED
            0x1D18C, // 1D18C..1D1A9; COMMON
            0x1D1AA, // 1D1AA..1D1AD; INHERITED
            0x1D1AE, // 1D1AE..1D1FF; COMMON
            0x1D200, // 1D200..1D2FF; GREEK
            0x1D300, // 1D300..1EDFF; COMMON
            0x1EE00, // 1EE00..1EFFF; ARABIC
            0x1F000, // 1F000..1F1FF; COMMON
            0x1F200, // 1F200..1F200; HIRAGANA
            0x1F201, // 1F210..1FFFF; COMMON
            0x20000, // 20000..E0000; HAN
            0xE0001, // E0001..E00FF; COMMON
            0xE0100, // E0100..E01EF; INHERITED
            0xE01F0 // E01F0..10FFFF; UNKNOWN
        };

        /**
        * Returns the enum constant representing the Unicode script of which
        * the given character (Unicode code point) is assigned to.
         *
        * @param   codePoint the character (Unicode code point) in question.
        * @return  The {@code UnicodeScript} constant representing the
        *          Unicode script of which this character is assigned to.
        *
        * @exception IllegalArgumentException if the specified
        * {@code codePoint} is an invalid Unicode code point.
        * @see Character#isValidCodePoint(int)
        *
        */

        public static UnicodeScript Of(int codePoint) {
            if (!IsValidCodePoint(codePoint))
                throw new Exception();
            string value = Char.ConvertFromUtf32(codePoint);
            UnicodeCategory type = CharUnicodeInfo.GetUnicodeCategory(value, 0);
            // leave SURROGATE and PRIVATE_USE for table lookup
            if (type == UnicodeCategory.OtherNotAssigned) {
                return UnicodeScript.UNKNOWN;
            }
            int index = Array.BinarySearch(ScriptStarts, codePoint);
            if (index < 0) {
                index = -index - 2;
            }
            return Scripts[index];
        }

        public static bool IsValidCodePoint(int codePoint) {
            // Optimized form of:
            //     codePoint >= MIN_CODE_POINT && codePoint <= MAX_CODE_POINT
            int plane = (int) ((uint) codePoint >> 16);
            return plane < (int) ((uint) (MAX_CODE_POINT + 1) >> 16);
        }
    }
}
