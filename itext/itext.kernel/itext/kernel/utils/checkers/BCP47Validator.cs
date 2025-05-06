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
using System.Text.RegularExpressions;

namespace iText.Kernel.Utils.Checkers {
    /// <summary>This class is a validator for IETF BCP 47 language tag (RFC 5646).</summary>
    public sealed class BCP47Validator {
        private const String REGULAR = "(art-lojban|cel-gaulish|no-bok|no-nyn|zh-guoyu|zh-hakka|zh-min|zh-min-nan|zh-xiang)";

        private const String IRREGULAR = "(en-GB-oed|i-ami|i-bnn|i-default|i-enochian|i-hak|i-klingon|i-lux|" + "i-mingo|i-navajo|i-pwn|i-tao|i-tay|i-tsu|sgn-BE-FR|sgn-BE-NL|sgn-CH-DE)";

        private const String GRANDFATHERED = "(?<grandfathered>" + IRREGULAR + "|" + REGULAR + ")";

        private const String PRIVATE_USE = "(?<privateUse>x(-[A-Za-z0-9]{1,8})+)";

        private const String SINGLETON = "[0-9A-WY-Za-wy-z]";

        private const String EXTENSION = "(?<extension>" + SINGLETON + "(-[A-Za-z0-9]{2,8})+)";

        private const String VARIANT = "(?<variant>[A-Za-z0-9]{5,8}|[0-9][A-Za-z0-9]{3})";

        private const String REGION = "(?<region>[A-Za-z]{2}|[0-9]{3})";

        private const String SCRIPT = "(?<script>[A-Za-z]{4})";

        private const String EXTLANG = "(?<extlang>[A-Za-z]{3}(-[A-Za-z]{3}){0,2})";

        private const String LANGUAGE = "(?<language>([A-Za-z]{2,3}(-" + EXTLANG + ")?)|[A-Za-z]{4}|[A-Za-z]{5,8})";

        private const String LANGTAG = "(" + LANGUAGE + "(-" + SCRIPT + ")?" + "(-" + REGION + ")?" + "(-" + VARIANT
             + ")*" + "(-" + EXTENSION + ")*" + "(-" + PRIVATE_USE + ")?" + ")";

        // Java regex polices doesn't allow duplicate named capture groups,
        // so we have to change the 2nd use <privateUse> to ?<privateUse1>.
        private static readonly Regex LANGUAGE_TAG_PATTERN = iText.Commons.Utils.StringUtil.RegexCompile("^(" + GRANDFATHERED
             + "|" + LANGTAG + "|" + PRIVATE_USE.Replace("privateUse", "privateUse1") + ")$");

        private BCP47Validator() {
        }

        // Private constructor will prevent the instantiation of this class directly.
        /// <summary>Validate language tag against RFC 5646.</summary>
        /// <param name="languageTag">language tag string</param>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if it is a valid tag,
        /// <see langword="false"/>
        /// otherwise
        /// </returns>
        public static bool Validate(String languageTag) {
            return iText.Commons.Utils.Matcher.Match(LANGUAGE_TAG_PATTERN, languageTag).Matches();
        }
    }
}
