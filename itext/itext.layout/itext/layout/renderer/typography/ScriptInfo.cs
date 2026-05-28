using System;
using System.Collections.Generic;
using iText.Commons.Utils;

namespace iText.Layout.Renderer.Typography {
    /// <summary>Class containing information about script support in pdfCalligraph.</summary>
    /// <remarks>
    /// Class containing information about script support in pdfCalligraph.
    /// It contains information about supported scripts and their requirements.
    /// <para />
    /// This class is intended for internal usage.
    /// </remarks>
    public sealed class ScriptInfo {
        private static readonly ScriptInfoData scriptInfoData;

        static ScriptInfo() {
            scriptInfoData = new ScriptInfoData(80);
            //supported scripts
            ScriptRequirements marksOnly = new ScriptRequirements(JavaCollectionsUtil.EmptyList<String>(), JavaUtil.ArraysAsList
                ("mark", "mkmk"), false);
            scriptInfoData.AddRequirements(UnicodeScript.ARABIC, new ScriptRequirements(JavaCollectionsUtil.SingletonList
                <String>("arab"), JavaUtil.ArraysAsList("init", "medi", "fina", "rlig", "rclt", "isol"), JavaUtil.ArraysAsList
                ("mark", "mkmk"), true, true));
            scriptInfoData.AddRequirements(UnicodeScript.ARMENIAN, marksOnly.WithOtfScriptNames("armn"));
            scriptInfoData.AddRequirements(UnicodeScript.CYRILLIC, marksOnly.WithOtfScriptNames("cyrl"));
            scriptInfoData.AddRequirements(UnicodeScript.GEORGIAN, marksOnly.WithOtfScriptNames("geor"));
            scriptInfoData.AddRequirements(UnicodeScript.GREEK, marksOnly.WithOtfScriptNames("grek"));
            scriptInfoData.AddRequirements(UnicodeScript.LATIN, marksOnly.WithOtfScriptNames("latn"));
            scriptInfoData.AddRequirements(UnicodeScript.RUNIC, marksOnly.WithOtfScriptNames("runr"));
            scriptInfoData.AddRequirements(UnicodeScript.OGHAM, marksOnly.WithOtfScriptNames("ogam"));
            ScriptRequirements indicReqs = new ScriptRequirements(JavaUtil.ArraysAsList("akhn", "blw", "half", "pres", 
                "abvs", "blw", "haln", "pstf", "abvm", "calt", "hist", "psts"), JavaCollectionsUtil.EmptyList<String>(
                ), false);
            scriptInfoData.AddRequirements(UnicodeScript.DEVANAGARI, indicReqs.WithOtfScriptNames("dev2", "deva"));
            scriptInfoData.AddRequirements(UnicodeScript.TAMIL, indicReqs.WithOtfScriptNames("tml2", "taml"));
            scriptInfoData.AddRequirements(UnicodeScript.GURMUKHI, indicReqs.WithOtfScriptNames("gur2", "guru"));
            scriptInfoData.AddRequirements(UnicodeScript.ORIYA, indicReqs.WithOtfScriptNames("ory2", "orya"));
            scriptInfoData.AddRequirements(UnicodeScript.BENGALI, indicReqs.WithOtfScriptNames("bng2", "beng"));
            scriptInfoData.AddRequirements(UnicodeScript.MALAYALAM, indicReqs.WithOtfScriptNames("mlm2", "mlym"));
            scriptInfoData.AddRequirements(UnicodeScript.TELUGU, indicReqs.WithOtfScriptNames("tel2", "telu"));
            scriptInfoData.AddRequirements(UnicodeScript.GUJARATI, indicReqs.WithOtfScriptNames("gjr2", "gurj"));
            scriptInfoData.AddRequirements(UnicodeScript.KANNADA, indicReqs.WithOtfScriptNames("knd2", "knda"));
            scriptInfoData.AddRequirements(UnicodeScript.SINHALA, indicReqs.WithOtfScriptNames("sinh"));
            scriptInfoData.AddRequirements(UnicodeScript.KHMER, indicReqs.WithOtfScriptNames("khmr").WithIsHardcoded(true
                ));
            scriptInfoData.AddRequirements(UnicodeScript.HEBREW, new ScriptRequirements(JavaCollectionsUtil.SingletonList
                <String>("hebr"), JavaCollectionsUtil.EmptyList<String>(), JavaUtil.ArraysAsList("mark", "mkmk"), true
                , true));
            scriptInfoData.AddRequirements(UnicodeScript.MYANMAR, new ScriptRequirements(JavaCollectionsUtil.SingletonList
                ("mym2"), JavaUtil.ArraysAsList("locl", "ccmp", "rphf", "pref", "blwf", "pstf", "rlig", "clig"), JavaCollectionsUtil
                .EmptyList<String>(), true, true));
            scriptInfoData.AddRequirements(UnicodeScript.THAI, new ScriptRequirements(JavaCollectionsUtil.SingletonList
                <String>("thai"), JavaCollectionsUtil.EmptyList<String>(), JavaCollectionsUtil.EmptyList<String>(), true
                , true));
            scriptInfoData.AddRequirements(UnicodeScript.TIBETAN, new ScriptRequirements(JavaCollectionsUtil.SingletonList
                <String>("tibt"), JavaCollectionsUtil.EmptyList<String>(), JavaCollectionsUtil.EmptyList<String>(), true
                , true));
            //non supported scripts
            scriptInfoData.AddRequirements(UnicodeScript.BALINESE, ScriptRequirements.CreateUnsupported(JavaCollectionsUtil
                .SingletonList<String>("bali")));
            scriptInfoData.AddRequirements(UnicodeScript.BOPOMOFO, ScriptRequirements.CreateUnsupported(JavaCollectionsUtil
                .SingletonList<String>("bopo")));
            scriptInfoData.AddRequirements(UnicodeScript.BRAILLE, ScriptRequirements.CreateUnsupported(JavaCollectionsUtil
                .SingletonList<String>("brai")));
            scriptInfoData.AddRequirements(UnicodeScript.BUGINESE, ScriptRequirements.CreateUnsupported(JavaCollectionsUtil
                .SingletonList<String>("bugi")));
            scriptInfoData.AddRequirements(UnicodeScript.BUHID, ScriptRequirements.CreateUnsupported(JavaCollectionsUtil
                .SingletonList<String>("buhd")));
            // SCRIPT_REQ_FEATURE.put(UnicodeScript.BYZANTINE_MUSIC, ScriptRequirements.createUnsupported(Collections
            // .singletonList("byzm")));
            // SCRIPT_REQ_FEATURE.put(UnicodeScript.Canadian SYLLABICS, ScriptRequirements.createUnsupported(Collections
            // .singletonList("cans")));
            scriptInfoData.AddRequirements(UnicodeScript.CARIAN, ScriptRequirements.CreateUnsupported(JavaCollectionsUtil
                .SingletonList<String>("cari")));
            scriptInfoData.AddRequirements(UnicodeScript.CHAM, ScriptRequirements.CreateUnsupported(JavaCollectionsUtil
                .SingletonList<String>("cham")));
            scriptInfoData.AddRequirements(UnicodeScript.CHEROKEE, ScriptRequirements.CreateUnsupported(JavaCollectionsUtil
                .SingletonList<String>("cher")));
            // SCRIPT_REQ_FEATURE.put(UnicodeScript.CJK IDEOGRAPHIC, ScriptRequirements.createUnsupported(Collections
            // .singletonList("hani")));
            scriptInfoData.AddRequirements(UnicodeScript.COPTIC, ScriptRequirements.CreateUnsupported(JavaCollectionsUtil
                .SingletonList<String>("copt")));
            // SCRIPT_REQ_FEATURE.put(UnicodeScript.Cypriot SYLLABARY, ScriptRequirements.createUnsupported(Collections
            // .singletonList("cprt")));
            scriptInfoData.AddRequirements(UnicodeScript.DESERET, ScriptRequirements.CreateUnsupported(JavaCollectionsUtil
                .SingletonList<String>("dsrt")));
            scriptInfoData.AddRequirements(UnicodeScript.ETHIOPIC, ScriptRequirements.CreateUnsupported(JavaCollectionsUtil
                .SingletonList<String>("ethi")));
            scriptInfoData.AddRequirements(UnicodeScript.GLAGOLITIC, ScriptRequirements.CreateUnsupported(JavaCollectionsUtil
                .SingletonList<String>("glag")));
            scriptInfoData.AddRequirements(UnicodeScript.GOTHIC, ScriptRequirements.CreateUnsupported(JavaCollectionsUtil
                .SingletonList<String>("goth")));
            scriptInfoData.AddRequirements(UnicodeScript.HANGUL, ScriptRequirements.CreateUnsupported(JavaUtil.ArraysAsList
                ("hang", "jamo")));
            scriptInfoData.AddRequirements(UnicodeScript.HANUNOO, ScriptRequirements.CreateUnsupported(JavaCollectionsUtil
                .SingletonList<String>("hano")));
            scriptInfoData.AddRequirements(UnicodeScript.HIRAGANA, ScriptRequirements.CreateUnsupported(JavaCollectionsUtil
                .SingletonList<String>("kana")));
            scriptInfoData.AddRequirements(UnicodeScript.JAVANESE, ScriptRequirements.CreateUnsupported(JavaCollectionsUtil
                .SingletonList<String>("java")));
            scriptInfoData.AddRequirements(UnicodeScript.KATAKANA, ScriptRequirements.CreateUnsupported(JavaCollectionsUtil
                .SingletonList<String>("kana")));
            scriptInfoData.AddRequirements(UnicodeScript.KAYAH_LI, ScriptRequirements.CreateUnsupported(JavaCollectionsUtil
                .SingletonList<String>("kali")));
            scriptInfoData.AddRequirements(UnicodeScript.KHAROSHTHI, ScriptRequirements.CreateUnsupported(JavaCollectionsUtil
                .SingletonList<String>("khar")));
            scriptInfoData.AddRequirements(UnicodeScript.LAO, ScriptRequirements.CreateUnsupported(JavaCollectionsUtil
                .SingletonList<String>("lao ")).WithIsHardcoded(true));
            scriptInfoData.AddRequirements(UnicodeScript.LEPCHA, ScriptRequirements.CreateUnsupported(JavaCollectionsUtil
                .SingletonList<String>("lepc")));
            scriptInfoData.AddRequirements(UnicodeScript.LIMBU, ScriptRequirements.CreateUnsupported(JavaCollectionsUtil
                .SingletonList<String>("limb")));
            scriptInfoData.AddRequirements(UnicodeScript.LINEAR_B, ScriptRequirements.CreateUnsupported(JavaCollectionsUtil
                .SingletonList<String>("linb")));
            scriptInfoData.AddRequirements(UnicodeScript.LYCIAN, ScriptRequirements.CreateUnsupported(JavaCollectionsUtil
                .SingletonList<String>("lyci")));
            scriptInfoData.AddRequirements(UnicodeScript.LYDIAN, ScriptRequirements.CreateUnsupported(JavaCollectionsUtil
                .SingletonList<String>("lydi")));
            // SCRIPT_REQ_FEATURE.put(UnicodeScript.Mathematical Alphanumeric SYMBOLS, ScriptRequirements
            // .createUnsupported(Collections.<String>singletonList("math")));
            scriptInfoData.AddRequirements(UnicodeScript.MONGOLIAN, ScriptRequirements.CreateUnsupported(JavaCollectionsUtil
                .SingletonList<String>("mong")));
            // SCRIPT_REQ_FEATURE.put(UnicodeScript.Musical SYMBOLS, ScriptRequirements.createUnsupported(Collections
            // .singletonList("musc")));
            scriptInfoData.AddRequirements(UnicodeScript.NEW_TAI_LUE, ScriptRequirements.CreateUnsupported(JavaCollectionsUtil
                .SingletonList<String>("talu")));
            scriptInfoData.AddRequirements(UnicodeScript.NKO, ScriptRequirements.CreateUnsupported(JavaCollectionsUtil
                .SingletonList<String>("nko ")));
            scriptInfoData.AddRequirements(UnicodeScript.OL_CHIKI, ScriptRequirements.CreateUnsupported(JavaCollectionsUtil
                .SingletonList<String>("olck")));
            scriptInfoData.AddRequirements(UnicodeScript.OLD_ITALIC, ScriptRequirements.CreateUnsupported(JavaCollectionsUtil
                .SingletonList<String>("ital")));
            // "Old Persian Cuneiform" script name
            scriptInfoData.AddRequirements(UnicodeScript.OLD_PERSIAN, ScriptRequirements.CreateUnsupported(JavaCollectionsUtil
                .SingletonList<String>("xpeo")));
            scriptInfoData.AddRequirements(UnicodeScript.OSMANYA, ScriptRequirements.CreateUnsupported(JavaCollectionsUtil
                .SingletonList<String>("osma")));
            scriptInfoData.AddRequirements(UnicodeScript.PHAGS_PA, ScriptRequirements.CreateUnsupported(JavaCollectionsUtil
                .SingletonList<String>("phag")));
            scriptInfoData.AddRequirements(UnicodeScript.PHOENICIAN, ScriptRequirements.CreateUnsupported(JavaCollectionsUtil
                .SingletonList<String>("phnx")));
            scriptInfoData.AddRequirements(UnicodeScript.REJANG, ScriptRequirements.CreateUnsupported(JavaCollectionsUtil
                .SingletonList<String>("rjng")));
            scriptInfoData.AddRequirements(UnicodeScript.SAURASHTRA, ScriptRequirements.CreateUnsupported(JavaCollectionsUtil
                .SingletonList<String>("saur")));
            scriptInfoData.AddRequirements(UnicodeScript.SHAVIAN, ScriptRequirements.CreateUnsupported(JavaCollectionsUtil
                .SingletonList<String>("shaw")));
            // SCRIPT_REQ_FEATURE.put(UnicodeScript.Sumero-Akkadian CUNEIFORM, ScriptRequirements.createUnsupported
            // (Collections.<String>singletonList("xsux")));
            scriptInfoData.AddRequirements(UnicodeScript.SUNDANESE, ScriptRequirements.CreateUnsupported(JavaCollectionsUtil
                .SingletonList<String>("sund")));
            scriptInfoData.AddRequirements(UnicodeScript.SYLOTI_NAGRI, ScriptRequirements.CreateUnsupported(JavaCollectionsUtil
                .SingletonList<String>("sylo")));
            scriptInfoData.AddRequirements(UnicodeScript.SYRIAC, ScriptRequirements.CreateUnsupported(JavaCollectionsUtil
                .SingletonList<String>("syrc")));
            scriptInfoData.AddRequirements(UnicodeScript.TAGALOG, ScriptRequirements.CreateUnsupported(JavaCollectionsUtil
                .SingletonList<String>("tglg")));
            scriptInfoData.AddRequirements(UnicodeScript.TAGBANWA, ScriptRequirements.CreateUnsupported(JavaCollectionsUtil
                .SingletonList<String>("tagb")));
            scriptInfoData.AddRequirements(UnicodeScript.TAI_LE, ScriptRequirements.CreateUnsupported(JavaCollectionsUtil
                .SingletonList<String>("tale")));
            scriptInfoData.AddRequirements(UnicodeScript.THAANA, ScriptRequirements.CreateUnsupported(JavaCollectionsUtil
                .SingletonList<String>("thaa")));
            scriptInfoData.AddRequirements(UnicodeScript.TIFINAGH, ScriptRequirements.CreateUnsupported(JavaCollectionsUtil
                .SingletonList<String>("tfng")));
            // "Ugaritic Cuneiform" script name
            scriptInfoData.AddRequirements(UnicodeScript.UGARITIC, ScriptRequirements.CreateUnsupported(JavaCollectionsUtil
                .SingletonList<String>("ugar")));
            scriptInfoData.AddRequirements(UnicodeScript.VAI, ScriptRequirements.CreateUnsupported(JavaCollectionsUtil
                .SingletonList<String>("vai ")));
            scriptInfoData.AddRequirements(UnicodeScript.YI, ScriptRequirements.CreateUnsupported(JavaCollectionsUtil.
                SingletonList<String>("yi  ")));
        }

        private ScriptInfo() {
        }

        // do nothing
        /// <summary>Checks if the script is supported by pdfCalligraph.</summary>
        /// <remarks>
        /// Checks if the script is supported by pdfCalligraph.
        /// Supported script have requirements set in
        /// <see cref="ScriptRequirements"/>.
        /// </remarks>
        /// <param name="script">the script to check</param>
        /// <returns><c>true</c> if the script is supported by pdfCalligraph and <c>false</c> otherwise</returns>
        public static bool ScriptSupported(UnicodeScript? script) {
            return scriptInfoData.ScriptSupported(script);
        }

        /// <summary>Returns requirements for the script.</summary>
        /// <remarks>Returns requirements for the script. Only supported script have the requirements set.</remarks>
        /// <param name="script">the script to get requirements for</param>
        /// <returns>requirements for the script. Only supported script have the requirements set</returns>
        public static ScriptRequirements GetRequirements(UnicodeScript? script) {
            return scriptInfoData.Get(script);
        }

        /// <summary>Returns a set if scripts supported by pdfCalligraph.</summary>
        /// <returns>a set if scripts supported by pdfCalligraph</returns>
        public static ICollection<UnicodeScript> GetSupportedScripts() {
            return scriptInfoData.GetSupportedScripts();
        }
    }
}
