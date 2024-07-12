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
using iText.StyledXmlParser.Jsoup.Internal;
using iText.StyledXmlParser.Jsoup.Nodes;

namespace iText.StyledXmlParser.Jsoup.Parser {
    /// <summary>Controls parser settings, to optionally preserve tag and/or attribute name case.</summary>
    public class ParseSettings {
        /// <summary>HTML default settings: both tag and attribute names are lower-cased during parsing.</summary>
        public static readonly iText.StyledXmlParser.Jsoup.Parser.ParseSettings htmlDefault;

        /// <summary>Preserve both tag and attribute case.</summary>
        public static readonly iText.StyledXmlParser.Jsoup.Parser.ParseSettings preserveCase;

        static ParseSettings() {
            htmlDefault = new iText.StyledXmlParser.Jsoup.Parser.ParseSettings(false, false);
            preserveCase = new iText.StyledXmlParser.Jsoup.Parser.ParseSettings(true, true);
        }

        private readonly bool preserveTagCase;

        private readonly bool preserveAttributeCase;

        /// <summary>Returns true if preserving tag name case.</summary>
        public virtual bool PreserveTagCase() {
            return preserveTagCase;
        }

        /// <summary>Returns true if preserving attribute case.</summary>
        public virtual bool PreserveAttributeCase() {
            return preserveAttributeCase;
        }

        /// <summary>Define parse settings.</summary>
        /// <param name="tag">preserve tag case?</param>
        /// <param name="attribute">preserve attribute name case?</param>
        public ParseSettings(bool tag, bool attribute) {
            preserveTagCase = tag;
            preserveAttributeCase = attribute;
        }

//\cond DO_NOT_DOCUMENT
        internal ParseSettings(iText.StyledXmlParser.Jsoup.Parser.ParseSettings copy)
            : this(copy.preserveTagCase, copy.preserveAttributeCase) {
        }
//\endcond

        /// <summary>Normalizes a tag name according to the case preservation setting.</summary>
        public virtual String NormalizeTag(String name) {
            name = name.Trim();
            if (!preserveTagCase) {
                name = Normalizer.LowerCase(name);
            }
            return name;
        }

        /// <summary>Normalizes an attribute according to the case preservation setting.</summary>
        public virtual String NormalizeAttribute(String name) {
            name = name.Trim();
            if (!preserveAttributeCase) {
                name = Normalizer.LowerCase(name);
            }
            return name;
        }

//\cond DO_NOT_DOCUMENT
        internal virtual Attributes NormalizeAttributes(Attributes attributes) {
            if (attributes != null && !preserveAttributeCase) {
                attributes.Normalize();
            }
            return attributes;
        }
//\endcond
    }
}
