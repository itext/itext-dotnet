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
using iText.StyledXmlParser.Jsoup.Helper;
using iText.StyledXmlParser.Jsoup.Internal;

namespace iText.StyledXmlParser.Jsoup.Parser {
    /// <summary>HTML Tag capabilities.</summary>
    public class Tag
#if !NETSTANDARD2_0
 : ICloneable
#endif
 {
        private static readonly IDictionary<String, iText.StyledXmlParser.Jsoup.Parser.Tag> tags = new Dictionary<
            String, iText.StyledXmlParser.Jsoup.Parser.Tag>();

        // map of known tags
        private String tagName;

        private String normalName;

        // always the lower case version of this tag, regardless of case preservation mode
        private bool isBlock = true;

        // block
        private bool formatAsBlock = true;

        // should be formatted as a block
        private bool empty = false;

        // can hold nothing; e.g. img
        private bool selfClosing = false;

        // can self close (<foo />). used for unknown tags that self close, without forcing them as empty.
        private bool preserveWhitespace = false;

        // for pre, textarea, script etc
        private bool formList = false;

        // a control that appears in forms: input, textarea, output etc
        private bool formSubmit = false;

        // a control that can be submitted in a form: input etc
        private Tag(String tagName) {
            this.tagName = tagName;
            normalName = Normalizer.LowerCase(tagName);
        }

        /// <summary>Get this tag's name.</summary>
        /// <returns>the tag's name</returns>
        public virtual String GetName() {
            return tagName;
        }

        /// <summary>Get this tag's normalized (lowercased) name.</summary>
        /// <returns>the tag's normal name.</returns>
        public virtual String NormalName() {
            return normalName;
        }

        /// <summary>Get a Tag by name.</summary>
        /// <remarks>
        /// Get a Tag by name. If not previously defined (unknown), returns a new generic tag, that can do anything.
        /// <para />
        /// Pre-defined tags (P, DIV etc) will be ==, but unknown tags are not registered and will only .equals().
        /// </remarks>
        /// <param name="tagName">Name of tag, e.g. "p". Case insensitive.</param>
        /// <param name="settings">used to control tag name sensitivity</param>
        /// <returns>The tag, either defined or new generic.</returns>
        public static iText.StyledXmlParser.Jsoup.Parser.Tag ValueOf(String tagName, ParseSettings settings) {
            Validate.NotNull(tagName);
            iText.StyledXmlParser.Jsoup.Parser.Tag tag = tags.Get(tagName);
            if (tag == null) {
                tagName = settings.NormalizeTag(tagName);
                // the name we'll use
                Validate.NotEmpty(tagName);
                String normalName = Normalizer.LowerCase(tagName);
                // the lower-case name to get tag settings off
                tag = tags.Get(normalName);
                if (tag == null) {
                    // not defined: create default; go anywhere, do anything! (incl be inside a <p>)
                    tag = new iText.StyledXmlParser.Jsoup.Parser.Tag(tagName);
                    tag.isBlock = false;
                }
                else {
                    if (settings.PreserveTagCase() && !tagName.Equals(normalName)) {
                        tag = (iText.StyledXmlParser.Jsoup.Parser.Tag)tag.Clone();
                        // get a new version vs the static one, so name update doesn't reset all
                        tag.tagName = tagName;
                    }
                }
            }
            return tag;
        }

        /// <summary>Get a Tag by name.</summary>
        /// <remarks>
        /// Get a Tag by name. If not previously defined (unknown), returns a new generic tag, that can do anything.
        /// <para />
        /// Pre-defined tags (P, DIV etc) will be ==, but unknown tags are not registered and will only .equals().
        /// </remarks>
        /// <param name="tagName">Name of tag, e.g. "p". <b>Case sensitive</b>.</param>
        /// <returns>The tag, either defined or new generic.</returns>
        public static iText.StyledXmlParser.Jsoup.Parser.Tag ValueOf(String tagName) {
            return ValueOf(tagName, ParseSettings.preserveCase);
        }

        /// <summary>Gets if this is a block tag.</summary>
        /// <returns>if block tag</returns>
        public virtual bool IsBlock() {
            return isBlock;
        }

        /// <summary>Gets if this tag should be formatted as a block (or as inline)</summary>
        /// <returns>if should be formatted as block or inline</returns>
        public virtual bool FormatAsBlock() {
            return formatAsBlock;
        }

        /// <summary>Gets if this tag is an inline tag.</summary>
        /// <returns>if this tag is an inline tag.</returns>
        public virtual bool IsInline() {
            return !isBlock;
        }

        /// <summary>Get if this is an empty tag</summary>
        /// <returns>if this is an empty tag</returns>
        public virtual bool IsEmpty() {
            return empty;
        }

        /// <summary>Get if this tag is self closing.</summary>
        /// <returns>if this tag should be output as self closing.</returns>
        public virtual bool IsSelfClosing() {
            return empty || selfClosing;
        }

        /// <summary>Get if this is a pre-defined tag, or was auto created on parsing.</summary>
        /// <returns>if a known tag</returns>
        public virtual bool IsKnownTag() {
            return tags.ContainsKey(tagName);
        }

        /// <summary>Check if this tagname is a known tag.</summary>
        /// <param name="tagName">name of tag</param>
        /// <returns>if known HTML tag</returns>
        public static bool IsKnownTag(String tagName) {
            return tags.ContainsKey(tagName);
        }

        /// <summary>Get if this tag should preserve whitespace within child text nodes.</summary>
        /// <returns>if preserve whitespace</returns>
        public virtual bool PreserveWhitespace() {
            return preserveWhitespace;
        }

        /// <summary>Get if this tag represents a control associated with a form.</summary>
        /// <remarks>Get if this tag represents a control associated with a form. E.g. input, textarea, output</remarks>
        /// <returns>if associated with a form</returns>
        public virtual bool IsFormListed() {
            return formList;
        }

        /// <summary>Get if this tag represents an element that should be submitted with a form.</summary>
        /// <remarks>Get if this tag represents an element that should be submitted with a form. E.g. input, option</remarks>
        /// <returns>if submittable with a form</returns>
        public virtual bool IsFormSubmittable() {
            return formSubmit;
        }

        internal virtual iText.StyledXmlParser.Jsoup.Parser.Tag SetSelfClosing() {
            selfClosing = true;
            return this;
        }

        public override bool Equals(Object o) {
            if (this == o) {
                return true;
            }
            if (!(o is iText.StyledXmlParser.Jsoup.Parser.Tag)) {
                return false;
            }
            iText.StyledXmlParser.Jsoup.Parser.Tag tag = (iText.StyledXmlParser.Jsoup.Parser.Tag)o;
            if (!tagName.Equals(tag.tagName)) {
                return false;
            }
            if (empty != tag.empty) {
                return false;
            }
            if (formatAsBlock != tag.formatAsBlock) {
                return false;
            }
            if (isBlock != tag.isBlock) {
                return false;
            }
            if (preserveWhitespace != tag.preserveWhitespace) {
                return false;
            }
            if (selfClosing != tag.selfClosing) {
                return false;
            }
            if (formList != tag.formList) {
                return false;
            }
            return formSubmit == tag.formSubmit;
        }

        public override int GetHashCode() {
            int result = tagName.GetHashCode();
            result = 31 * result + (isBlock ? 1 : 0);
            result = 31 * result + (formatAsBlock ? 1 : 0);
            result = 31 * result + (empty ? 1 : 0);
            result = 31 * result + (selfClosing ? 1 : 0);
            result = 31 * result + (preserveWhitespace ? 1 : 0);
            result = 31 * result + (formList ? 1 : 0);
            result = 31 * result + (formSubmit ? 1 : 0);
            return result;
        }

        public override String ToString() {
            return tagName;
        }

        public virtual Object Clone() {
            iText.StyledXmlParser.Jsoup.Parser.Tag newTag = new iText.StyledXmlParser.Jsoup.Parser.Tag(this.tagName);
            newTag.normalName = this.normalName;
            newTag.empty = this.empty;
            newTag.formatAsBlock = this.formatAsBlock;
            newTag.formList = this.formList;
            newTag.formSubmit = this.formSubmit;
            newTag.preserveWhitespace = this.preserveWhitespace;
            newTag.selfClosing = this.selfClosing;
            newTag.isBlock = this.isBlock;
            return newTag;
        }

        // internal static initialisers:
        // prepped from http://www.w3.org/TR/REC-html40/sgml/dtd.html and other sources
        private static readonly String[] blockTags = new String[] { "html", "head", "body", "frameset", "script", 
            "noscript", "style", "meta", "link", "title", "frame", "noframes", "section", "nav", "aside", "hgroup"
            , "header", "footer", "p", "h1", "h2", "h3", "h4", "h5", "h6", "ul", "ol", "pre", "div", "blockquote", 
            "hr", "address", "figure", "figcaption", "form", "fieldset", "ins", "del", "dl", "dt", "dd", "li", "table"
            , "caption", "thead", "tfoot", "tbody", "colgroup", "col", "tr", "th", "td", "video", "audio", "canvas"
            , "details", "menu", "plaintext", "template", "article", "main", "svg", "math", "center" };

        private static readonly String[] inlineTags = new String[] { "object", "base", "font", "tt", "i", "b", "u"
            , "big", "small", "em", "strong", "dfn", "code", "samp", "kbd", "var", "cite", "abbr", "time", "acronym"
            , "mark", "ruby", "rt", "rp", "a", "img", "br", "wbr", "map", "q", "sub", "sup", "bdo", "iframe", "embed"
            , "span", "input", "select", "textarea", "label", "button", "optgroup", "option", "legend", "datalist"
            , "keygen", "output", "progress", "meter", "area", "param", "source", "track", "summary", "command", "device"
            , "area", "basefont", "bgsound", "menuitem", "param", "source", "track", "data", "bdi", "s" };

        private static readonly String[] emptyTags = new String[] { "meta", "link", "base", "frame", "img", "br", 
            "wbr", "embed", "hr", "input", "keygen", "col", "command", "device", "area", "basefont", "bgsound", "menuitem"
            , "param", "source", "track" };

        private static readonly String[] formatAsInlineTags = new String[] { "title", "a", "p", "h1", "h2", "h3", 
            "h4", "h5", "h6", "pre", "address", "li", "th", "td", "script", "style", "ins", "del", "s" };

        private static readonly String[] preserveWhitespaceTags = new String[] { "pre", "plaintext", "title", "textarea"
             };

        // script is not here as it is a data node, which always preserve whitespace
        private static readonly String[] formListedTags = new String[] { "button", "fieldset", "input", "keygen", 
            "object", "output", "select", "textarea" };

        private static readonly String[] formSubmitTags = new String[] { "input", "keygen", "object", "select", "textarea"
             };

        static Tag() {
            // creates
            foreach (String tagName in blockTags) {
                iText.StyledXmlParser.Jsoup.Parser.Tag tag = new iText.StyledXmlParser.Jsoup.Parser.Tag(tagName);
                Register(tag);
            }
            foreach (String tagName in inlineTags) {
                iText.StyledXmlParser.Jsoup.Parser.Tag tag = new iText.StyledXmlParser.Jsoup.Parser.Tag(tagName);
                tag.isBlock = false;
                tag.formatAsBlock = false;
                Register(tag);
            }
            // mods:
            foreach (String tagName in emptyTags) {
                iText.StyledXmlParser.Jsoup.Parser.Tag tag = tags.Get(tagName);
                Validate.NotNull(tag);
                tag.empty = true;
            }
            foreach (String tagName in formatAsInlineTags) {
                iText.StyledXmlParser.Jsoup.Parser.Tag tag = tags.Get(tagName);
                Validate.NotNull(tag);
                tag.formatAsBlock = false;
            }
            foreach (String tagName in preserveWhitespaceTags) {
                iText.StyledXmlParser.Jsoup.Parser.Tag tag = tags.Get(tagName);
                Validate.NotNull(tag);
                tag.preserveWhitespace = true;
            }
            foreach (String tagName in formListedTags) {
                iText.StyledXmlParser.Jsoup.Parser.Tag tag = tags.Get(tagName);
                Validate.NotNull(tag);
                tag.formList = true;
            }
            foreach (String tagName in formSubmitTags) {
                iText.StyledXmlParser.Jsoup.Parser.Tag tag = tags.Get(tagName);
                Validate.NotNull(tag);
                tag.formSubmit = true;
            }
        }

        private static void Register(iText.StyledXmlParser.Jsoup.Parser.Tag tag) {
            tags.Put(tag.tagName, tag);
        }
    }
}
