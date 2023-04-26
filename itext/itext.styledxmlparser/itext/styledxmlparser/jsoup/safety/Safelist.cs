/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 Apryse Group NV
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
using System.Text.RegularExpressions;
using iText.StyledXmlParser.Jsoup.Helper;
using iText.StyledXmlParser.Jsoup.Internal;
using iText.StyledXmlParser.Jsoup.Nodes;

namespace iText.StyledXmlParser.Jsoup.Safety {
    /*
    Thank you to Ryan Grove (wonko.com) for the Ruby HTML cleaner http://github.com/rgrove/sanitize/, which inspired
    this safe-list configuration, and the initial defaults.
    */
    /// <summary>Safe-lists define what HTML (elements and attributes) to allow through the cleaner.</summary>
    /// <remarks>
    /// Safe-lists define what HTML (elements and attributes) to allow through the cleaner. Everything else is removed.
    /// <para />
    /// Start with one of the defaults:
    /// <list type="bullet">
    /// <item><description>
    /// <see cref="None()"/>
    /// </description></item>
    /// <item><description>
    /// <see cref="SimpleText()"/>
    /// </description></item>
    /// <item><description>
    /// <see cref="Basic()"/>
    /// </description></item>
    /// <item><description>
    /// <see cref="BasicWithImages()"/>
    /// </description></item>
    /// <item><description>
    /// <see cref="Relaxed()"/>
    /// </description></item>
    /// </list>
    /// <para />
    /// If you need to allow more through (please be careful!), tweak a base safelist with:
    /// <list type="bullet">
    /// <item><description>
    /// <see cref="AddTags(System.String[])"/>
    /// </description></item>
    /// <item><description>
    /// <see cref="AddAttributes(System.String, System.String[])"/>
    /// </description></item>
    /// <item><description>
    /// <see cref="AddEnforcedAttribute(System.String, System.String, System.String)"/>
    /// </description></item>
    /// <item><description>
    /// <see cref="AddProtocols(System.String, System.String, System.String[])"/>
    /// </description></item>
    /// </list>
    /// <para />
    /// You can remove any setting from an existing safelist with:
    /// <list type="bullet">
    /// <item><description>
    /// <see cref="RemoveTags(System.String[])"/>
    /// </description></item>
    /// <item><description>
    /// <see cref="RemoveAttributes(System.String, System.String[])"/>
    /// </description></item>
    /// <item><description>
    /// <see cref="RemoveEnforcedAttribute(System.String, System.String)"/>
    /// </description></item>
    /// <item><description>
    /// <see cref="RemoveProtocols(System.String, System.String, System.String[])"/>
    /// </description></item>
    /// </list>
    /// <para />
    /// The cleaner and these safelists assume that you want to clean a <c>body</c> fragment of HTML (to add user
    /// supplied HTML into a templated page), and not to clean a full HTML document. If the latter is the case, either wrap the
    /// document HTML around the cleaned body HTML, or create a safelist that allows <c>html</c> and <c>head</c>
    /// elements as appropriate.
    /// <para />
    /// If you are going to extend a safelist, please be very careful. Make sure you understand what attributes may lead to
    /// XSS attack vectors. URL attributes are particularly vulnerable and require careful validation. See
    /// the <a href="https://owasp.org/www-community/xss-filter-evasion-cheatsheet">XSS Filter Evasion Cheat Sheet</a> for some
    /// XSS attack examples (that jsoup will safegaurd against the default Cleaner and Safelist configuration).
    /// </remarks>
    public class Safelist {
        private static readonly Regex SPACE_PATTERN = iText.Commons.Utils.StringUtil.RegexCompile("\\s");

        private ICollection<Safelist.TagName> tagNames;

        // tags allowed, lower case. e.g. [p, br, span]
        private IDictionary<Safelist.TagName, ICollection<Safelist.AttributeKey>> attributes;

        // tag -> attribute[]. allowed attributes [href] for a tag.
        private IDictionary<Safelist.TagName, IDictionary<Safelist.AttributeKey, Safelist.AttributeValue>> enforcedAttributes;

        // always set these attribute values
        private IDictionary<Safelist.TagName, IDictionary<Safelist.AttributeKey, ICollection<Safelist.Protocol>>> 
            protocols;

        // allowed URL protocols for attributes
        private bool preserveRelativeLinks;

        // option to preserve relative links
        /// <summary>This safelist allows only text nodes: all HTML will be stripped.</summary>
        /// <returns>safelist</returns>
        public static iText.StyledXmlParser.Jsoup.Safety.Safelist None() {
            return new iText.StyledXmlParser.Jsoup.Safety.Safelist();
        }

        /// <summary>This safelist allows only simple text formatting: <c>b, em, i, strong, u</c>.</summary>
        /// <remarks>
        /// This safelist allows only simple text formatting: <c>b, em, i, strong, u</c>. All other HTML (tags and
        /// attributes) will be removed.
        /// </remarks>
        /// <returns>safelist</returns>
        public static iText.StyledXmlParser.Jsoup.Safety.Safelist SimpleText() {
            return new iText.StyledXmlParser.Jsoup.Safety.Safelist().AddTags("b", "em", "i", "strong", "u");
        }

        /// <summary>
        /// <para />
        /// This safelist allows a fuller range of text nodes: <c>a, b, blockquote, br, cite, code, dd, dl, dt, em, i, li,
        /// ol, p, pre, q, small, span, strike, strong, sub, sup, u, ul</c>, and appropriate attributes.
        /// </summary>
        /// <remarks>
        /// <para />
        /// This safelist allows a fuller range of text nodes: <c>a, b, blockquote, br, cite, code, dd, dl, dt, em, i, li,
        /// ol, p, pre, q, small, span, strike, strong, sub, sup, u, ul</c>, and appropriate attributes.
        /// <para />
        /// Links (<c>a</c> elements) can point to <c>http, https, ftp, mailto</c>, and have an enforced
        /// <c>rel=nofollow</c> attribute.
        /// <para />
        /// Does not allow images.
        /// </remarks>
        /// <returns>safelist</returns>
        public static iText.StyledXmlParser.Jsoup.Safety.Safelist Basic() {
            return new iText.StyledXmlParser.Jsoup.Safety.Safelist().AddTags("a", "b", "blockquote", "br", "cite", "code"
                , "dd", "dl", "dt", "em", "i", "li", "ol", "p", "pre", "q", "small", "span", "strike", "strong", "sub"
                , "sup", "u", "ul").AddAttributes("a", "href").AddAttributes("blockquote", "cite").AddAttributes("q", 
                "cite").AddProtocols("a", "href", "ftp", "http", "https", "mailto").AddProtocols("blockquote", "cite", 
                "http", "https").AddProtocols("cite", "cite", "http", "https").AddEnforcedAttribute("a", "rel", "nofollow"
                );
        }

        /// <summary>
        /// This safelist allows the same text tags as
        /// <see cref="Basic()"/>
        /// , and also allows <c>img</c> tags, with appropriate
        /// attributes, with <c>src</c> pointing to <c>http</c> or <c>https</c>.
        /// </summary>
        /// <returns>safelist</returns>
        public static iText.StyledXmlParser.Jsoup.Safety.Safelist BasicWithImages() {
            return Basic().AddTags("img").AddAttributes("img", "align", "alt", "height", "src", "title", "width").AddProtocols
                ("img", "src", "http", "https");
        }

        /// <summary>
        /// This safelist allows a full range of text and structural body HTML: <c>a, b, blockquote, br, caption, cite,
        /// code, col, colgroup, dd, div, dl, dt, em, h1, h2, h3, h4, h5, h6, i, img, li, ol, p, pre, q, small, span, strike, strong, sub,
        /// sup, table, tbody, td, tfoot, th, thead, tr, u, ul</c>
        /// </summary>
        /// <remarks>
        /// This safelist allows a full range of text and structural body HTML: <c>a, b, blockquote, br, caption, cite,
        /// code, col, colgroup, dd, div, dl, dt, em, h1, h2, h3, h4, h5, h6, i, img, li, ol, p, pre, q, small, span, strike, strong, sub,
        /// sup, table, tbody, td, tfoot, th, thead, tr, u, ul</c>
        /// <para />
        /// Links do not have an enforced <c>rel=nofollow</c> attribute, but you can add that if desired.
        /// </remarks>
        /// <returns>safelist</returns>
        public static iText.StyledXmlParser.Jsoup.Safety.Safelist Relaxed() {
            return new iText.StyledXmlParser.Jsoup.Safety.Safelist().AddTags("a", "b", "blockquote", "br", "caption", 
                "cite", "code", "col", "colgroup", "dd", "div", "dl", "dt", "em", "h1", "h2", "h3", "h4", "h5", "h6", 
                "i", "img", "li", "ol", "p", "pre", "q", "small", "span", "strike", "strong", "sub", "sup", "table", "tbody"
                , "td", "tfoot", "th", "thead", "tr", "u", "ul").AddAttributes("a", "href", "title").AddAttributes("blockquote"
                , "cite").AddAttributes("col", "span", "width").AddAttributes("colgroup", "span", "width").AddAttributes
                ("img", "align", "alt", "height", "src", "title", "width").AddAttributes("ol", "start", "type").AddAttributes
                ("q", "cite").AddAttributes("table", "summary", "width").AddAttributes("td", "abbr", "axis", "colspan"
                , "rowspan", "width").AddAttributes("th", "abbr", "axis", "colspan", "rowspan", "scope", "width").AddAttributes
                ("ul", "type").AddProtocols("a", "href", "ftp", "http", "https", "mailto").AddProtocols("blockquote", 
                "cite", "http", "https").AddProtocols("cite", "cite", "http", "https").AddProtocols("img", "src", "http"
                , "https").AddProtocols("q", "cite", "http", "https");
        }

        /// <summary>Create a new, empty safelist.</summary>
        /// <remarks>Create a new, empty safelist. Generally it will be better to start with a default prepared safelist instead.
        ///     </remarks>
        /// <seealso cref="Basic()"/>
        /// <seealso cref="BasicWithImages()"/>
        /// <seealso cref="SimpleText()"/>
        /// <seealso cref="Relaxed()"/>
        public Safelist() {
            tagNames = new HashSet<Safelist.TagName>();
            attributes = new Dictionary<Safelist.TagName, ICollection<Safelist.AttributeKey>>();
            enforcedAttributes = new Dictionary<Safelist.TagName, IDictionary<Safelist.AttributeKey, Safelist.AttributeValue
                >>();
            protocols = new Dictionary<Safelist.TagName, IDictionary<Safelist.AttributeKey, ICollection<Safelist.Protocol
                >>>();
            preserveRelativeLinks = false;
        }

        /// <summary>Deep copy an existing Safelist to a new Safelist.</summary>
        /// <param name="copy">the Safelist to copy</param>
        public Safelist(iText.StyledXmlParser.Jsoup.Safety.Safelist copy)
            : this() {
            tagNames.AddAll(copy.tagNames);
            attributes.AddAll(copy.attributes);
            enforcedAttributes.AddAll(copy.enforcedAttributes);
            protocols.AddAll(copy.protocols);
            preserveRelativeLinks = copy.preserveRelativeLinks;
        }

        /// <summary>Add a list of allowed elements to a safelist.</summary>
        /// <remarks>Add a list of allowed elements to a safelist. (If a tag is not allowed, it will be removed from the HTML.)
        ///     </remarks>
        /// <param name="tags">tag names to allow</param>
        /// <returns>this (for chaining)</returns>
        public virtual iText.StyledXmlParser.Jsoup.Safety.Safelist AddTags(params String[] tags) {
            Validate.NotNull(tags);
            foreach (String tagName in tags) {
                Validate.NotEmpty(tagName);
                tagNames.Add(Safelist.TagName.ValueOf(tagName));
            }
            return this;
        }

        /// <summary>Remove a list of allowed elements from a safelist.</summary>
        /// <remarks>Remove a list of allowed elements from a safelist. (If a tag is not allowed, it will be removed from the HTML.)
        ///     </remarks>
        /// <param name="tags">tag names to disallow</param>
        /// <returns>this (for chaining)</returns>
        public virtual iText.StyledXmlParser.Jsoup.Safety.Safelist RemoveTags(params String[] tags) {
            Validate.NotNull(tags);
            foreach (String tag in tags) {
                Validate.NotEmpty(tag);
                Safelist.TagName tagName = Safelist.TagName.ValueOf(tag);
                if (tagNames.Remove(tagName)) {
                    // Only look in sub-maps if tag was allowed
                    attributes.JRemove(tagName);
                    enforcedAttributes.JRemove(tagName);
                    protocols.JRemove(tagName);
                }
            }
            return this;
        }

        /// <summary>Add a list of allowed attributes to a tag.</summary>
        /// <remarks>
        /// Add a list of allowed attributes to a tag. (If an attribute is not allowed on an element, it will be removed.)
        /// <para />
        /// E.g.: <c>addAttributes("a", "href", "class")</c> allows <c>href</c> and <c>class</c> attributes
        /// on <c>a</c> tags.
        /// <para />
        /// To make an attribute valid for <b>all tags</b>, use the pseudo tag <c>:all</c>, e.g.
        /// <c>addAttributes(":all", "class")</c>.
        /// </remarks>
        /// <param name="tag">The tag the attributes are for. The tag will be added to the allowed tag list if necessary.
        ///     </param>
        /// <param name="attributes">List of valid attributes for the tag</param>
        /// <returns>this (for chaining)</returns>
        public virtual iText.StyledXmlParser.Jsoup.Safety.Safelist AddAttributes(String tag, params String[] attributes
            ) {
            Validate.NotEmpty(tag);
            Validate.NotNull(attributes);
            Validate.IsTrue(attributes.Length > 0, "No attribute names supplied.");
            Safelist.TagName tagName = Safelist.TagName.ValueOf(tag);
            tagNames.Add(tagName);
            ICollection<Safelist.AttributeKey> attributeSet = new HashSet<Safelist.AttributeKey>();
            foreach (String key in attributes) {
                Validate.NotEmpty(key);
                attributeSet.Add(Safelist.AttributeKey.ValueOf(key));
            }
            if (this.attributes.ContainsKey(tagName)) {
                ICollection<Safelist.AttributeKey> currentSet = this.attributes.Get(tagName);
                currentSet.AddAll(attributeSet);
            }
            else {
                this.attributes.Put(tagName, attributeSet);
            }
            return this;
        }

        /// <summary>Remove a list of allowed attributes from a tag.</summary>
        /// <remarks>
        /// Remove a list of allowed attributes from a tag. (If an attribute is not allowed on an element, it will be removed.)
        /// <para />
        /// E.g.: <c>removeAttributes("a", "href", "class")</c> disallows <c>href</c> and <c>class</c>
        /// attributes on <c>a</c> tags.
        /// <para />
        /// To make an attribute invalid for <b>all tags</b>, use the pseudo tag <c>:all</c>, e.g.
        /// <c>removeAttributes(":all", "class")</c>.
        /// </remarks>
        /// <param name="tag">The tag the attributes are for.</param>
        /// <param name="attributes">List of invalid attributes for the tag</param>
        /// <returns>this (for chaining)</returns>
        public virtual iText.StyledXmlParser.Jsoup.Safety.Safelist RemoveAttributes(String tag, params String[] attributes
            ) {
            Validate.NotEmpty(tag);
            Validate.NotNull(attributes);
            Validate.IsTrue(attributes.Length > 0, "No attribute names supplied.");
            Safelist.TagName tagName = Safelist.TagName.ValueOf(tag);
            ICollection<Safelist.AttributeKey> attributeSet = new HashSet<Safelist.AttributeKey>();
            foreach (String key in attributes) {
                Validate.NotEmpty(key);
                attributeSet.Add(Safelist.AttributeKey.ValueOf(key));
            }
            if (tagNames.Contains(tagName) && this.attributes.ContainsKey(tagName)) {
                // Only look in sub-maps if tag was allowed
                ICollection<Safelist.AttributeKey> currentSet = this.attributes.Get(tagName);
                currentSet.RemoveAll(attributeSet);
                if (currentSet.IsEmpty()) {
                    // Remove tag from attribute map if no attributes are allowed for tag
                    this.attributes.JRemove(tagName);
                }
            }
            if (tag.Equals(":all")) {
                // Attribute needs to be removed from all individually set tags
                foreach (Safelist.TagName name in this.attributes.Keys) {
                    ICollection<Safelist.AttributeKey> currentSet = this.attributes.Get(name);
                    currentSet.RemoveAll(attributeSet);
                    if (currentSet.IsEmpty()) {
                        // Remove tag from attribute map if no attributes are allowed for tag
                        this.attributes.JRemove(name);
                    }
                }
            }
            return this;
        }

        /// <summary>Add an enforced attribute to a tag.</summary>
        /// <remarks>
        /// Add an enforced attribute to a tag. An enforced attribute will always be added to the element. If the element
        /// already has the attribute set, it will be overridden with this value.
        /// <para />
        /// E.g.: <c>addEnforcedAttribute("a", "rel", "nofollow")</c> will make all <c>a</c> tags output as
        /// <c>&lt;a href="..." rel="nofollow"&gt;</c>
        /// </remarks>
        /// <param name="tag">The tag the enforced attribute is for. The tag will be added to the allowed tag list if necessary.
        ///     </param>
        /// <param name="attribute">The attribute name</param>
        /// <param name="value">The enforced attribute value</param>
        /// <returns>this (for chaining)</returns>
        public virtual iText.StyledXmlParser.Jsoup.Safety.Safelist AddEnforcedAttribute(String tag, String attribute
            , String value) {
            Validate.NotEmpty(tag);
            Validate.NotEmpty(attribute);
            Validate.NotEmpty(value);
            Safelist.TagName tagName = Safelist.TagName.ValueOf(tag);
            tagNames.Add(tagName);
            Safelist.AttributeKey attrKey = Safelist.AttributeKey.ValueOf(attribute);
            Safelist.AttributeValue attrVal = Safelist.AttributeValue.ValueOf(value);
            if (enforcedAttributes.ContainsKey(tagName)) {
                enforcedAttributes.Get(tagName).Put(attrKey, attrVal);
            }
            else {
                IDictionary<Safelist.AttributeKey, Safelist.AttributeValue> attrMap = new Dictionary<Safelist.AttributeKey
                    , Safelist.AttributeValue>();
                attrMap.Put(attrKey, attrVal);
                enforcedAttributes.Put(tagName, attrMap);
            }
            return this;
        }

        /// <summary>Remove a previously configured enforced attribute from a tag.</summary>
        /// <param name="tag">The tag the enforced attribute is for.</param>
        /// <param name="attribute">The attribute name</param>
        /// <returns>this (for chaining)</returns>
        public virtual iText.StyledXmlParser.Jsoup.Safety.Safelist RemoveEnforcedAttribute(String tag, String attribute
            ) {
            Validate.NotEmpty(tag);
            Validate.NotEmpty(attribute);
            Safelist.TagName tagName = Safelist.TagName.ValueOf(tag);
            if (tagNames.Contains(tagName) && enforcedAttributes.ContainsKey(tagName)) {
                Safelist.AttributeKey attrKey = Safelist.AttributeKey.ValueOf(attribute);
                IDictionary<Safelist.AttributeKey, Safelist.AttributeValue> attrMap = enforcedAttributes.Get(tagName);
                attrMap.JRemove(attrKey);
                if (attrMap.IsEmpty()) {
                    // Remove tag from enforced attribute map if no enforced attributes are present
                    enforcedAttributes.JRemove(tagName);
                }
            }
            return this;
        }

        /// <summary>
        /// Configure this Safelist to preserve relative links in an element's URL attribute, or convert them to absolute
        /// links.
        /// </summary>
        /// <remarks>
        /// Configure this Safelist to preserve relative links in an element's URL attribute, or convert them to absolute
        /// links. By default, this is <b>false</b>: URLs will be  made absolute (e.g. start with an allowed protocol, like
        /// e.g.
        /// <c>http://</c>.
        /// <para />
        /// Note that when handling relative links, the input document must have an appropriate
        /// <c>base URI</c>
        /// set when
        /// parsing, so that the link's protocol can be confirmed. Regardless of the setting of the
        /// <c>
        /// preserve relative
        /// links
        /// </c>
        /// option, the link must be resolvable against the base URI to an allowed protocol; otherwise the attribute
        /// will be removed.
        /// </remarks>
        /// <param name="preserve">
        /// 
        /// <see langword="true"/>
        /// to allow relative links,
        /// <see langword="false"/>
        /// (default) to deny
        /// </param>
        /// <returns>this Safelist, for chaining.</returns>
        /// <seealso cref="AddProtocols(System.String, System.String, System.String[])"/>
        public virtual iText.StyledXmlParser.Jsoup.Safety.Safelist PreserveRelativeLinks(bool preserve) {
            preserveRelativeLinks = preserve;
            return this;
        }

        /// <summary>Add allowed URL protocols for an element's URL attribute.</summary>
        /// <remarks>
        /// Add allowed URL protocols for an element's URL attribute. This restricts the possible values of the attribute to
        /// URLs with the defined protocol.
        /// <para />
        /// E.g.: <c>addProtocols("a", "href", "ftp", "http", "https")</c>
        /// <para />
        /// To allow a link to an in-page URL anchor (i.e. <c>&lt;a href="#anchor"&gt;</c>, add a <c>#</c>:<br />
        /// E.g.: <c>addProtocols("a", "href", "#")</c>
        /// </remarks>
        /// <param name="tag">Tag the URL protocol is for</param>
        /// <param name="attribute">Attribute name</param>
        /// <param name="protocols">List of valid protocols</param>
        /// <returns>this, for chaining</returns>
        public virtual iText.StyledXmlParser.Jsoup.Safety.Safelist AddProtocols(String tag, String attribute, params 
            String[] protocols) {
            Validate.NotEmpty(tag);
            Validate.NotEmpty(attribute);
            Validate.NotNull(protocols);
            Safelist.TagName tagName = Safelist.TagName.ValueOf(tag);
            Safelist.AttributeKey attrKey = Safelist.AttributeKey.ValueOf(attribute);
            IDictionary<Safelist.AttributeKey, ICollection<Safelist.Protocol>> attrMap;
            ICollection<Safelist.Protocol> protSet;
            if (this.protocols.ContainsKey(tagName)) {
                attrMap = this.protocols.Get(tagName);
            }
            else {
                attrMap = new Dictionary<Safelist.AttributeKey, ICollection<Safelist.Protocol>>();
                this.protocols.Put(tagName, attrMap);
            }
            if (attrMap.ContainsKey(attrKey)) {
                protSet = attrMap.Get(attrKey);
            }
            else {
                protSet = new HashSet<Safelist.Protocol>();
                attrMap.Put(attrKey, protSet);
            }
            foreach (String protocol in protocols) {
                Validate.NotEmpty(protocol);
                Safelist.Protocol prot = Safelist.Protocol.ValueOf(protocol);
                protSet.Add(prot);
            }
            return this;
        }

        /// <summary>Remove allowed URL protocols for an element's URL attribute.</summary>
        /// <remarks>
        /// Remove allowed URL protocols for an element's URL attribute. If you remove all protocols for an attribute, that
        /// attribute will allow any protocol.
        /// <para />
        /// E.g.: <c>removeProtocols("a", "href", "ftp")</c>
        /// </remarks>
        /// <param name="tag">Tag the URL protocol is for</param>
        /// <param name="attribute">Attribute name</param>
        /// <param name="removeProtocols">List of invalid protocols</param>
        /// <returns>this, for chaining</returns>
        public virtual iText.StyledXmlParser.Jsoup.Safety.Safelist RemoveProtocols(String tag, String attribute, params 
            String[] removeProtocols) {
            Validate.NotEmpty(tag);
            Validate.NotEmpty(attribute);
            Validate.NotNull(removeProtocols);
            Safelist.TagName tagName = Safelist.TagName.ValueOf(tag);
            Safelist.AttributeKey attr = Safelist.AttributeKey.ValueOf(attribute);
            // make sure that what we're removing actually exists; otherwise can open the tag to any data and that can
            // be surprising
            Validate.IsTrue(protocols.ContainsKey(tagName), "Cannot remove a protocol that is not set.");
            IDictionary<Safelist.AttributeKey, ICollection<Safelist.Protocol>> tagProtocols = protocols.Get(tagName);
            Validate.IsTrue(tagProtocols.ContainsKey(attr), "Cannot remove a protocol that is not set.");
            ICollection<Safelist.Protocol> attrProtocols = tagProtocols.Get(attr);
            foreach (String protocol in removeProtocols) {
                Validate.NotEmpty(protocol);
                attrProtocols.Remove(Safelist.Protocol.ValueOf(protocol));
            }
            if (attrProtocols.IsEmpty()) {
                // Remove protocol set if empty
                tagProtocols.JRemove(attr);
                if (tagProtocols.IsEmpty()) {
                    // Remove entry for tag if empty
                    protocols.JRemove(tagName);
                }
            }
            return this;
        }

        /// <summary>Test if the supplied tag is allowed by this safelist</summary>
        /// <param name="tag">test tag</param>
        /// <returns>true if allowed</returns>
        protected internal virtual bool IsSafeTag(String tag) {
            return tagNames.Contains(Safelist.TagName.ValueOf(tag));
        }

        /// <summary>Test if the supplied attribute is allowed by this safelist for this tag</summary>
        /// <param name="tagName">tag to consider allowing the attribute in</param>
        /// <param name="el">element under test, to confirm protocol</param>
        /// <param name="attr">attribute under test</param>
        /// <returns>true if allowed</returns>
        protected internal virtual bool IsSafeAttribute(String tagName, iText.StyledXmlParser.Jsoup.Nodes.Element 
            el, iText.StyledXmlParser.Jsoup.Nodes.Attribute attr) {
            Safelist.TagName tag = Safelist.TagName.ValueOf(tagName);
            Safelist.AttributeKey key = Safelist.AttributeKey.ValueOf(attr.Key);
            ICollection<Safelist.AttributeKey> okSet = attributes.Get(tag);
            if (okSet != null && okSet.Contains(key)) {
                if (protocols.ContainsKey(tag)) {
                    IDictionary<Safelist.AttributeKey, ICollection<Safelist.Protocol>> attrProts = protocols.Get(tag);
                    // ok if not defined protocol; otherwise test
                    return !attrProts.ContainsKey(key) || TestValidProtocol(el, attr, attrProts.Get(key));
                }
                else {
                    // attribute found, no protocols defined, so OK
                    return true;
                }
            }
            // might be an enforced attribute?
            IDictionary<Safelist.AttributeKey, Safelist.AttributeValue> enforcedSet = enforcedAttributes.Get(tag);
            if (enforcedSet != null) {
                Attributes expect = GetEnforcedAttributes(tagName);
                String attrKey = attr.Key;
                if (expect.HasKeyIgnoreCase(attrKey)) {
                    return expect.GetIgnoreCase(attrKey).Equals(attr.Value);
                }
            }
            // no attributes defined for tag, try :all tag
            return !tagName.Equals(":all") && IsSafeAttribute(":all", el, attr);
        }

        private bool TestValidProtocol(iText.StyledXmlParser.Jsoup.Nodes.Element el, iText.StyledXmlParser.Jsoup.Nodes.Attribute
             attr, ICollection<Safelist.Protocol> protocols) {
            // try to resolve relative urls to abs, and optionally update the attribute so output html has abs.
            // rels without a baseuri get removed
            String value = el.AbsUrl(attr.Key);
            if (value.Length == 0) {
                value = attr.Value;
            }
            // if it could not be made abs, run as-is to allow custom unknown protocols
            if (!preserveRelativeLinks) {
                attr.SetValue(value);
            }
            foreach (Safelist.Protocol protocol in protocols) {
                String prot = protocol.ToString();
                if (prot.Equals("#")) {
                    // allows anchor links
                    if (IsValidAnchor(value)) {
                        return true;
                    }
                    else {
                        continue;
                    }
                }
                prot += ":";
                if (Normalizer.LowerCase(value).StartsWith(prot)) {
                    return true;
                }
            }
            return false;
        }

        private bool IsValidAnchor(String value) {
            return value.StartsWith("#") && !iText.Commons.Utils.Matcher.Match(SPACE_PATTERN, value).Find();
        }

        internal virtual Attributes GetEnforcedAttributes(String tagName) {
            Attributes attrs = new Attributes();
            Safelist.TagName tag = Safelist.TagName.ValueOf(tagName);
            if (enforcedAttributes.ContainsKey(tag)) {
                IDictionary<Safelist.AttributeKey, Safelist.AttributeValue> keyVals = enforcedAttributes.Get(tag);
                foreach (KeyValuePair<Safelist.AttributeKey, Safelist.AttributeValue> entry in keyVals) {
                    attrs.Put(entry.Key.ToString(), entry.Value.ToString());
                }
            }
            return attrs;
        }

        // named types for config. All just hold strings, but here for my sanity.
        internal class TagName : Safelist.TypedValue {
            internal TagName(String value)
                : base(value) {
            }

            internal static Safelist.TagName ValueOf(String value) {
                return new Safelist.TagName(value);
            }
        }

        internal class AttributeKey : Safelist.TypedValue {
            internal AttributeKey(String value)
                : base(value) {
            }

            internal static Safelist.AttributeKey ValueOf(String value) {
                return new Safelist.AttributeKey(value);
            }
        }

        internal class AttributeValue : Safelist.TypedValue {
            internal AttributeValue(String value)
                : base(value) {
            }

            internal static Safelist.AttributeValue ValueOf(String value) {
                return new Safelist.AttributeValue(value);
            }
        }

        internal class Protocol : Safelist.TypedValue {
            internal Protocol(String value)
                : base(value) {
            }

            internal static Safelist.Protocol ValueOf(String value) {
                return new Safelist.Protocol(value);
            }
        }

        internal abstract class TypedValue {
            private String value;

            internal TypedValue(String value) {
                Validate.NotNull(value);
                this.value = value;
            }

            public override int GetHashCode() {
                int prime = 31;
                int result = 1;
                result = prime * result + ((value == null) ? 0 : value.GetHashCode());
                return result;
            }

            public override bool Equals(Object obj) {
                if (this == obj) {
                    return true;
                }
                if (obj == null) {
                    return false;
                }
                if (GetType() != obj.GetType()) {
                    return false;
                }
                Safelist.TypedValue other = (Safelist.TypedValue)obj;
                if (value == null) {
                    return other.value == null;
                }
                else {
                    return value.Equals(other.value);
                }
            }

            public override String ToString() {
                return value;
            }
        }
    }
}
