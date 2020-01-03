/*
This file is part of the iText (R) project.
Copyright (c) 1998-2020 iText Group NV
Authors: iText Software.

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
using System;
using System.Collections.Generic;
using iText.StyledXmlParser.Jsoup.Helper;

namespace iText.StyledXmlParser.Jsoup.Parser {
    /// <summary>HTML Tag capabilities.</summary>
    /// <author>Jonathan Hedley, jonathan@hedley.net</author>
    public class Tag {
        private static readonly IDictionary<String, iText.StyledXmlParser.Jsoup.Parser.Tag> tags = new Dictionary<
            String, iText.StyledXmlParser.Jsoup.Parser.Tag>();

        // map of known tags
        private String tagName;

        private bool isBlock = true;

        // block or inline
        private bool formatAsBlock = true;

        // should be formatted as a block
        private bool canContainBlock = true;

        // Can this tag hold block level tags?
        private bool canContainInline = true;

        // only pcdata if not
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
            this.tagName = tagName.ToLowerInvariant();
        }

        /// <summary>Get this tag's name.</summary>
        /// <returns>the tag's name</returns>
        public virtual String GetName() {
            return tagName;
        }

        /// <summary>Get a Tag by name.</summary>
        /// <remarks>
        /// Get a Tag by name. If not previously defined (unknown), returns a new generic tag, that can do anything.
        /// <para />
        /// Pre-defined tags (P, DIV etc) will be ==, but unknown tags are not registered and will only .equals().
        /// </remarks>
        /// <param name="tagName">Name of tag, e.g. "p". Case insensitive.</param>
        /// <returns>The tag, either defined or new generic.</returns>
        public static iText.StyledXmlParser.Jsoup.Parser.Tag ValueOf(String tagName) {
            Validate.NotNull(tagName);
            iText.StyledXmlParser.Jsoup.Parser.Tag tag = tags.Get(tagName);
            if (tag == null) {
                tagName = tagName.Trim().ToLowerInvariant();
                Validate.NotEmpty(tagName);
                tag = tags.Get(tagName);
                if (tag == null) {
                    // not defined: create default; go anywhere, do anything! (incl be inside a <p>)
                    tag = new iText.StyledXmlParser.Jsoup.Parser.Tag(tagName);
                    tag.isBlock = false;
                    tag.canContainBlock = true;
                }
            }
            return tag;
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

        /// <summary>Gets if this tag can contain block tags.</summary>
        /// <returns>if tag can contain block tags</returns>
        public virtual bool CanContainBlock() {
            return canContainBlock;
        }

        /// <summary>Gets if this tag is an inline tag.</summary>
        /// <returns>if this tag is an inline tag.</returns>
        public virtual bool IsInline() {
            return !isBlock;
        }

        /// <summary>Gets if this tag is a data only tag.</summary>
        /// <returns>if this tag is a data only tag</returns>
        public virtual bool IsData() {
            return !canContainInline && !IsEmpty();
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
        /// <returns>if preserve whitepace</returns>
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
            if (canContainBlock != tag.canContainBlock) {
                return false;
            }
            if (canContainInline != tag.canContainInline) {
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
            result = 31 * result + (canContainBlock ? 1 : 0);
            result = 31 * result + (canContainInline ? 1 : 0);
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

        // internal static initialisers:
        // prepped from http://www.w3.org/TR/REC-html40/sgml/dtd.html and other sources
        private static readonly String[] blockTags = new String[] { "html", "head", "body", "frameset", "script", 
            "noscript", "style", "meta", "link", "title", "frame", "noframes", "section", "nav", "aside", "hgroup"
            , "header", "footer", "p", "h1", "h2", "h3", "h4", "h5", "h6", "ul", "ol", "pre", "div", "blockquote", 
            "hr", "address", "figure", "figcaption", "form", "fieldset", "ins", "del", "s", "dl", "dt", "dd", "li"
            , "table", "caption", "thead", "tfoot", "tbody", "colgroup", "col", "tr", "th", "td", "video", "audio"
            , "canvas", "details", "menu", "plaintext", "template", "article", "main", "svg", "math" };

        private static readonly String[] inlineTags = new String[] { "object", "base", "font", "tt", "i", "b", "u"
            , "big", "small", "em", "strong", "dfn", "code", "samp", "kbd", "var", "cite", "abbr", "time", "acronym"
            , "mark", "ruby", "rt", "rp", "a", "img", "br", "wbr", "map", "q", "sub", "sup", "bdo", "iframe", "embed"
            , "span", "input", "select", "textarea", "label", "button", "optgroup", "option", "legend", "datalist"
            , "keygen", "output", "progress", "meter", "area", "param", "source", "track", "summary", "command", "device"
            , "area", "basefont", "bgsound", "menuitem", "param", "source", "track", "data", "bdi" };

        private static readonly String[] emptyTags = new String[] { "meta", "link", "base", "frame", "img", "br", 
            "wbr", "embed", "hr", "input", "keygen", "col", "command", "device", "area", "basefont", "bgsound", "menuitem"
            , "param", "source", "track" };

        private static readonly String[] formatAsInlineTags = new String[] { "title", "a", "p", "h1", "h2", "h3", 
            "h4", "h5", "h6", "pre", "address", "li", "th", "td", "script", "style", "ins", "del", "s" };

        private static readonly String[] preserveWhitespaceTags = new String[] { "pre", "plaintext", "title", "textarea"
             };

        // script is not here as it is a data node, which always preserve whitespace
        // todo: I think we just need submit tags, and can scrub listed
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
                tag.canContainBlock = false;
                tag.formatAsBlock = false;
                Register(tag);
            }
            // mods:
            foreach (String tagName in emptyTags) {
                iText.StyledXmlParser.Jsoup.Parser.Tag tag = tags.Get(tagName);
                Validate.NotNull(tag);
                tag.canContainBlock = false;
                tag.canContainInline = false;
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
