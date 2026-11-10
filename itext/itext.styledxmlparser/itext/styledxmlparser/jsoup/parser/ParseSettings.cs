/*
This file is part of jsoup, see NOTICE.txt in the root of the repository.
It may contain modifications beyond the original version.
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
