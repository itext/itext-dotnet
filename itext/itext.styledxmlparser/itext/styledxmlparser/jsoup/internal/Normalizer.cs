/*
This file is part of jsoup, see NOTICE.txt in the root of the repository.
It may contain modifications beyond the original version.
*/
using System;

namespace iText.StyledXmlParser.Jsoup.Internal {
    /// <summary>Util methods for normalizing strings.</summary>
    /// <remarks>Util methods for normalizing strings. Jsoup internal use only, please don't depend on this API.</remarks>
    public sealed class Normalizer {
        public static String LowerCase(String input) {
            return input != null ? input.ToLower(System.Globalization.CultureInfo.InvariantCulture) : "";
        }

        public static String Normalize(String input) {
            return LowerCase(input).Trim();
        }

        public static String Normalize(String input, bool isStringLiteral) {
            return isStringLiteral ? LowerCase(input) : Normalize(input);
        }
    }
}
