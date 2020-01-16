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
using iText.StyledXmlParser.Jsoup.Nodes;

namespace iText.StyledXmlParser.Jsoup.Safety {
    /*
    Thank you to Ryan Grove (wonko.com) for the Ruby HTML cleaner http://github.com/rgrove/sanitize/, which inspired
    this whitelist configuration, and the initial defaults.
    */
    /// <summary>Whitelists define what HTML (elements and attributes) to allow through the cleaner.</summary>
    /// <remarks>
    /// Whitelists define what HTML (elements and attributes) to allow through the cleaner. Everything else is removed.
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
    /// If you need to allow more through (please be careful!), tweak a base whitelist with:
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
    /// You can remove any setting from an existing whitelist with:
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
    /// The cleaner and these whitelists assume that you want to clean a <c>body</c> fragment of HTML (to add user
    /// supplied HTML into a templated page), and not to clean a full HTML document. If the latter is the case, either wrap the
    /// document HTML around the cleaned body HTML, or create a whitelist that allows <c>html</c> and <c>head</c>
    /// elements as appropriate.
    /// <para />
    /// If you are going to extend a whitelist, please be very careful. Make sure you understand what attributes may lead to
    /// XSS attack vectors. URL attributes are particularly vulnerable and require careful validation. See
    /// http://ha.ckers.org/xss.html for some XSS attack examples.
    /// </remarks>
    /// <author>Jonathan Hedley</author>
    public class Whitelist {
        private ICollection<Whitelist.TagName> tagNames;

        // tags allowed, lower case. e.g. [p, br, span]
        private IDictionary<Whitelist.TagName, ICollection<Whitelist.AttributeKey>> attributes;

        // tag -> attribute[]. allowed attributes [href] for a tag.
        private IDictionary<Whitelist.TagName, IDictionary<Whitelist.AttributeKey, Whitelist.AttributeValue>> enforcedAttributes;

        // always set these attribute values
        private IDictionary<Whitelist.TagName, IDictionary<Whitelist.AttributeKey, ICollection<Whitelist.Protocol>
            >> protocols;

        // allowed URL protocols for attributes
        private bool preserveRelativeLinks;

        // option to preserve relative links
        /// <summary>This whitelist allows only text nodes: all HTML will be stripped.</summary>
        /// <returns>whitelist</returns>
        public static iText.StyledXmlParser.Jsoup.Safety.Whitelist None() {
            return new iText.StyledXmlParser.Jsoup.Safety.Whitelist();
        }

        /// <summary>This whitelist allows only simple text formatting: <c>b, em, i, strong, u</c>.</summary>
        /// <remarks>
        /// This whitelist allows only simple text formatting: <c>b, em, i, strong, u</c>. All other HTML (tags and
        /// attributes) will be removed.
        /// </remarks>
        /// <returns>whitelist</returns>
        public static iText.StyledXmlParser.Jsoup.Safety.Whitelist SimpleText() {
            return new iText.StyledXmlParser.Jsoup.Safety.Whitelist().AddTags("b", "em", "i", "strong", "u");
        }

        /// <summary>
        /// This whitelist allows a fuller range of text nodes: <c>a, b, blockquote, br, cite, code, dd, dl, dt, em, i, li,
        /// ol, p, pre, q, small, span, strike, strong, sub, sup, u, ul</c>, and appropriate attributes.
        /// </summary>
        /// <remarks>
        /// This whitelist allows a fuller range of text nodes: <c>a, b, blockquote, br, cite, code, dd, dl, dt, em, i, li,
        /// ol, p, pre, q, small, span, strike, strong, sub, sup, u, ul</c>, and appropriate attributes.
        /// <para />
        /// Links (<c>a</c> elements) can point to <c>http, https, ftp, mailto</c>, and have an enforced
        /// <c>rel=nofollow</c> attribute.
        /// <para />
        /// Does not allow images.
        /// </remarks>
        /// <returns>whitelist</returns>
        public static iText.StyledXmlParser.Jsoup.Safety.Whitelist Basic() {
            return new iText.StyledXmlParser.Jsoup.Safety.Whitelist().AddTags("a", "b", "blockquote", "br", "cite", "code"
                , "dd", "dl", "dt", "em", "i", "li", "ol", "p", "pre", "q", "small", "span", "strike", "strong", "sub"
                , "sup", "u", "ul").AddAttributes("a", "href").AddAttributes("blockquote", "cite").AddAttributes("q", 
                "cite").AddProtocols("a", "href", "ftp", "http", "https", "mailto").AddProtocols("blockquote", "cite", 
                "http", "https").AddProtocols("cite", "cite", "http", "https").AddEnforcedAttribute("a", "rel", "nofollow"
                );
        }

        /// <summary>
        /// This whitelist allows the same text tags as
        /// <see cref="Basic()"/>
        /// , and also allows <c>img</c> tags, with appropriate
        /// attributes, with <c>src</c> pointing to <c>http</c> or <c>https</c>.
        /// </summary>
        /// <returns>whitelist</returns>
        public static iText.StyledXmlParser.Jsoup.Safety.Whitelist BasicWithImages() {
            return Basic().AddTags("img").AddAttributes("img", "align", "alt", "height", "src", "title", "width").AddProtocols
                ("img", "src", "http", "https");
        }

        /// <summary>
        /// This whitelist allows a full range of text and structural body HTML: <c>a, b, blockquote, br, caption, cite,
        /// code, col, colgroup, dd, div, dl, dt, em, h1, h2, h3, h4, h5, h6, i, img, li, ol, p, pre, q, small, span, strike, strong, sub,
        /// sup, table, tbody, td, tfoot, th, thead, tr, u, ul</c>
        /// </summary>
        /// <remarks>
        /// This whitelist allows a full range of text and structural body HTML: <c>a, b, blockquote, br, caption, cite,
        /// code, col, colgroup, dd, div, dl, dt, em, h1, h2, h3, h4, h5, h6, i, img, li, ol, p, pre, q, small, span, strike, strong, sub,
        /// sup, table, tbody, td, tfoot, th, thead, tr, u, ul</c>
        /// <para />
        /// Links do not have an enforced <c>rel=nofollow</c> attribute, but you can add that if desired.
        /// </remarks>
        /// <returns>whitelist</returns>
        public static iText.StyledXmlParser.Jsoup.Safety.Whitelist Relaxed() {
            return new iText.StyledXmlParser.Jsoup.Safety.Whitelist().AddTags("a", "b", "blockquote", "br", "caption", 
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

        /// <summary>Create a new, empty whitelist.</summary>
        /// <remarks>Create a new, empty whitelist. Generally it will be better to start with a default prepared whitelist instead.
        ///     </remarks>
        /// <seealso cref="Basic()"/>
        /// <seealso cref="BasicWithImages()"/>
        /// <seealso cref="SimpleText()"/>
        /// <seealso cref="Relaxed()"/>
        public Whitelist() {
            tagNames = new HashSet<Whitelist.TagName>();
            attributes = new Dictionary<Whitelist.TagName, ICollection<Whitelist.AttributeKey>>();
            enforcedAttributes = new Dictionary<Whitelist.TagName, IDictionary<Whitelist.AttributeKey, Whitelist.AttributeValue
                >>();
            protocols = new Dictionary<Whitelist.TagName, IDictionary<Whitelist.AttributeKey, ICollection<Whitelist.Protocol
                >>>();
            preserveRelativeLinks = false;
        }

        /// <summary>Add a list of allowed elements to a whitelist.</summary>
        /// <remarks>Add a list of allowed elements to a whitelist. (If a tag is not allowed, it will be removed from the HTML.)
        ///     </remarks>
        /// <param name="tags">tag names to allow</param>
        /// <returns>this (for chaining)</returns>
        public virtual iText.StyledXmlParser.Jsoup.Safety.Whitelist AddTags(params String[] tags) {
            Validate.NotNull(tags);
            foreach (String tagName in tags) {
                Validate.NotEmpty(tagName);
                tagNames.Add(Whitelist.TagName.ValueOf(tagName));
            }
            return this;
        }

        /// <summary>Remove a list of allowed elements from a whitelist.</summary>
        /// <remarks>Remove a list of allowed elements from a whitelist. (If a tag is not allowed, it will be removed from the HTML.)
        ///     </remarks>
        /// <param name="tags">tag names to disallow</param>
        /// <returns>this (for chaining)</returns>
        public virtual iText.StyledXmlParser.Jsoup.Safety.Whitelist RemoveTags(params String[] tags) {
            Validate.NotNull(tags);
            foreach (String tag in tags) {
                Validate.NotEmpty(tag);
                Whitelist.TagName tagName = Whitelist.TagName.ValueOf(tag);
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
        /// <param name="keys">List of valid attributes for the tag</param>
        /// <returns>this (for chaining)</returns>
        public virtual iText.StyledXmlParser.Jsoup.Safety.Whitelist AddAttributes(String tag, params String[] keys
            ) {
            Validate.NotEmpty(tag);
            Validate.NotNull(keys);
            Validate.IsTrue(keys.Length > 0, "No attributes supplied.");
            Whitelist.TagName tagName = Whitelist.TagName.ValueOf(tag);
            if (!tagNames.Contains(tagName)) {
                tagNames.Add(tagName);
            }
            ICollection<Whitelist.AttributeKey> attributeSet = new HashSet<Whitelist.AttributeKey>();
            foreach (String key in keys) {
                Validate.NotEmpty(key);
                attributeSet.Add(Whitelist.AttributeKey.ValueOf(key));
            }
            if (attributes.ContainsKey(tagName)) {
                ICollection<Whitelist.AttributeKey> currentSet = attributes.Get(tagName);
                currentSet.AddAll(attributeSet);
            }
            else {
                attributes.Put(tagName, attributeSet);
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
        /// <param name="keys">List of invalid attributes for the tag</param>
        /// <returns>this (for chaining)</returns>
        public virtual iText.StyledXmlParser.Jsoup.Safety.Whitelist RemoveAttributes(String tag, params String[] keys
            ) {
            Validate.NotEmpty(tag);
            Validate.NotNull(keys);
            Validate.IsTrue(keys.Length > 0, "No attributes supplied.");
            Whitelist.TagName tagName = Whitelist.TagName.ValueOf(tag);
            ICollection<Whitelist.AttributeKey> attributeSet = new HashSet<Whitelist.AttributeKey>();
            foreach (String key in keys) {
                Validate.NotEmpty(key);
                attributeSet.Add(Whitelist.AttributeKey.ValueOf(key));
            }
            if (tagNames.Contains(tagName) && attributes.ContainsKey(tagName)) {
                // Only look in sub-maps if tag was allowed
                ICollection<Whitelist.AttributeKey> currentSet = attributes.Get(tagName);
                currentSet.RemoveAll(attributeSet);
                if (currentSet.IsEmpty()) {
                    // Remove tag from attribute map if no attributes are allowed for tag
                    attributes.JRemove(tagName);
                }
            }
            if (tag.Equals(":all")) {
                // Attribute needs to be removed from all individually set tags
                foreach (Whitelist.TagName name in attributes.Keys) {
                    ICollection<Whitelist.AttributeKey> currentSet = attributes.Get(name);
                    currentSet.RemoveAll(attributeSet);
                    if (currentSet.IsEmpty()) {
                        // Remove tag from attribute map if no attributes are allowed for tag
                        attributes.JRemove(name);
                    }
                }
            }
            return this;
        }

        /// <summary>Add an enforced attribute to a tag.</summary>
        /// <remarks>
        /// Add an enforced attribute to a tag. An enforced attribute will always be added to the element. If the element
        /// already has the attribute set, it will be overridden.
        /// <para />
        /// E.g.: <c>addEnforcedAttribute("a", "rel", "nofollow")</c> will make all <c>a</c> tags output as
        /// <c>&lt;a href="..." rel="nofollow"&gt;</c>
        /// </remarks>
        /// <param name="tag">The tag the enforced attribute is for. The tag will be added to the allowed tag list if necessary.
        ///     </param>
        /// <param name="key">The attribute key</param>
        /// <param name="value">The enforced attribute value</param>
        /// <returns>this (for chaining)</returns>
        public virtual iText.StyledXmlParser.Jsoup.Safety.Whitelist AddEnforcedAttribute(String tag, String key, String
             value) {
            Validate.NotEmpty(tag);
            Validate.NotEmpty(key);
            Validate.NotEmpty(value);
            Whitelist.TagName tagName = Whitelist.TagName.ValueOf(tag);
            if (!tagNames.Contains(tagName)) {
                tagNames.Add(tagName);
            }
            Whitelist.AttributeKey attrKey = Whitelist.AttributeKey.ValueOf(key);
            Whitelist.AttributeValue attrVal = Whitelist.AttributeValue.ValueOf(value);
            if (enforcedAttributes.ContainsKey(tagName)) {
                enforcedAttributes.Get(tagName).Put(attrKey, attrVal);
            }
            else {
                IDictionary<Whitelist.AttributeKey, Whitelist.AttributeValue> attrMap = new Dictionary<Whitelist.AttributeKey
                    , Whitelist.AttributeValue>();
                attrMap.Put(attrKey, attrVal);
                enforcedAttributes.Put(tagName, attrMap);
            }
            return this;
        }

        /// <summary>Remove a previously configured enforced attribute from a tag.</summary>
        /// <param name="tag">The tag the enforced attribute is for.</param>
        /// <param name="key">The attribute key</param>
        /// <returns>this (for chaining)</returns>
        public virtual iText.StyledXmlParser.Jsoup.Safety.Whitelist RemoveEnforcedAttribute(String tag, String key
            ) {
            Validate.NotEmpty(tag);
            Validate.NotEmpty(key);
            Whitelist.TagName tagName = Whitelist.TagName.ValueOf(tag);
            if (tagNames.Contains(tagName) && enforcedAttributes.ContainsKey(tagName)) {
                Whitelist.AttributeKey attrKey = Whitelist.AttributeKey.ValueOf(key);
                IDictionary<Whitelist.AttributeKey, Whitelist.AttributeValue> attrMap = enforcedAttributes.Get(tagName);
                attrMap.JRemove(attrKey);
                if (attrMap.IsEmpty()) {
                    // Remove tag from enforced attribute map if no enforced attributes are present
                    enforcedAttributes.JRemove(tagName);
                }
            }
            return this;
        }

        /// <summary>
        /// Configure this Whitelist to preserve relative links in an element's URL attribute, or convert them to absolute
        /// links.
        /// </summary>
        /// <remarks>
        /// Configure this Whitelist to preserve relative links in an element's URL attribute, or convert them to absolute
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
        /// <returns>this Whitelist, for chaining.</returns>
        /// <seealso cref="AddProtocols(System.String, System.String, System.String[])"/>
        public virtual iText.StyledXmlParser.Jsoup.Safety.Whitelist PreserveRelativeLinks(bool preserve) {
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
        /// <param name="key">Attribute key</param>
        /// <param name="protocols">List of valid protocols</param>
        /// <returns>this, for chaining</returns>
        public virtual iText.StyledXmlParser.Jsoup.Safety.Whitelist AddProtocols(String tag, String key, params String
            [] protocols) {
            Validate.NotEmpty(tag);
            Validate.NotEmpty(key);
            Validate.NotNull(protocols);
            Whitelist.TagName tagName = Whitelist.TagName.ValueOf(tag);
            Whitelist.AttributeKey attrKey = Whitelist.AttributeKey.ValueOf(key);
            IDictionary<Whitelist.AttributeKey, ICollection<Whitelist.Protocol>> attrMap;
            ICollection<Whitelist.Protocol> protSet;
            if (this.protocols.ContainsKey(tagName)) {
                attrMap = this.protocols.Get(tagName);
            }
            else {
                attrMap = new Dictionary<Whitelist.AttributeKey, ICollection<Whitelist.Protocol>>();
                this.protocols.Put(tagName, attrMap);
            }
            if (attrMap.ContainsKey(attrKey)) {
                protSet = attrMap.Get(attrKey);
            }
            else {
                protSet = new HashSet<Whitelist.Protocol>();
                attrMap.Put(attrKey, protSet);
            }
            foreach (String protocol in protocols) {
                Validate.NotEmpty(protocol);
                Whitelist.Protocol prot = Whitelist.Protocol.ValueOf(protocol);
                protSet.Add(prot);
            }
            return this;
        }

        /// <summary>Remove allowed URL protocols for an element's URL attribute.</summary>
        /// <remarks>
        /// Remove allowed URL protocols for an element's URL attribute.
        /// <para />
        /// E.g.: <c>removeProtocols("a", "href", "ftp")</c>
        /// </remarks>
        /// <param name="tag">Tag the URL protocol is for</param>
        /// <param name="key">Attribute key</param>
        /// <param name="protocols">List of invalid protocols</param>
        /// <returns>this, for chaining</returns>
        public virtual iText.StyledXmlParser.Jsoup.Safety.Whitelist RemoveProtocols(String tag, String key, params 
            String[] protocols) {
            Validate.NotEmpty(tag);
            Validate.NotEmpty(key);
            Validate.NotNull(protocols);
            Whitelist.TagName tagName = Whitelist.TagName.ValueOf(tag);
            Whitelist.AttributeKey attrKey = Whitelist.AttributeKey.ValueOf(key);
            if (this.protocols.ContainsKey(tagName)) {
                IDictionary<Whitelist.AttributeKey, ICollection<Whitelist.Protocol>> attrMap = this.protocols.Get(tagName);
                if (attrMap.ContainsKey(attrKey)) {
                    ICollection<Whitelist.Protocol> protSet = attrMap.Get(attrKey);
                    foreach (String protocol in protocols) {
                        Validate.NotEmpty(protocol);
                        Whitelist.Protocol prot = Whitelist.Protocol.ValueOf(protocol);
                        protSet.Remove(prot);
                    }
                    if (protSet.IsEmpty()) {
                        // Remove protocol set if empty
                        attrMap.JRemove(attrKey);
                        if (attrMap.IsEmpty()) {
                            // Remove entry for tag if empty
                            this.protocols.JRemove(tagName);
                        }
                    }
                }
            }
            return this;
        }

        /// <summary>Test if the supplied tag is allowed by this whitelist</summary>
        /// <param name="tag">test tag</param>
        /// <returns>true if allowed</returns>
        protected internal virtual bool IsSafeTag(String tag) {
            return tagNames.Contains(Whitelist.TagName.ValueOf(tag));
        }

        /// <summary>Test if the supplied attribute is allowed by this whitelist for this tag</summary>
        /// <param name="tagName">tag to consider allowing the attribute in</param>
        /// <param name="el">element under test, to confirm protocol</param>
        /// <param name="attr">attribute under test</param>
        /// <returns>true if allowed</returns>
        protected internal virtual bool IsSafeAttribute(String tagName, iText.StyledXmlParser.Jsoup.Nodes.Element 
            el, iText.StyledXmlParser.Jsoup.Nodes.Attribute attr) {
            Whitelist.TagName tag = Whitelist.TagName.ValueOf(tagName);
            Whitelist.AttributeKey key = Whitelist.AttributeKey.ValueOf(attr.Key);
            if (attributes.ContainsKey(tag)) {
                if (attributes.Get(tag).Contains(key)) {
                    if (protocols.ContainsKey(tag)) {
                        IDictionary<Whitelist.AttributeKey, ICollection<Whitelist.Protocol>> attrProts = protocols.Get(tag);
                        // ok if not defined protocol; otherwise test
                        return !attrProts.ContainsKey(key) || TestValidProtocol(el, attr, attrProts.Get(key));
                    }
                    else {
                        // attribute found, no protocols defined, so OK
                        return true;
                    }
                }
            }
            // no attributes defined for tag, try :all tag
            return !tagName.Equals(":all") && IsSafeAttribute(":all", el, attr);
        }

        private bool TestValidProtocol(iText.StyledXmlParser.Jsoup.Nodes.Element el, iText.StyledXmlParser.Jsoup.Nodes.Attribute
             attr, ICollection<Whitelist.Protocol> protocols) {
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
            foreach (Whitelist.Protocol protocol in protocols) {
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
                if (value.ToLowerInvariant().StartsWith(prot)) {
                    return true;
                }
            }
            return false;
        }

        private bool IsValidAnchor(String value) {
            return value.StartsWith("#") && !value.Matches(".*\\s.*");
        }

        internal virtual Attributes GetEnforcedAttributes(String tagName) {
            Attributes attrs = new Attributes();
            Whitelist.TagName tag = Whitelist.TagName.ValueOf(tagName);
            if (enforcedAttributes.ContainsKey(tag)) {
                IDictionary<Whitelist.AttributeKey, Whitelist.AttributeValue> keyVals = enforcedAttributes.Get(tag);
                foreach (KeyValuePair<Whitelist.AttributeKey, Whitelist.AttributeValue> entry in keyVals) {
                    attrs.Put(entry.Key.ToString(), entry.Value.ToString());
                }
            }
            return attrs;
        }

        // named types for config. All just hold strings, but here for my sanity.
        internal class TagName : Whitelist.TypedValue {
            internal TagName(String value)
                : base(value) {
            }

            internal static Whitelist.TagName ValueOf(String value) {
                return new Whitelist.TagName(value);
            }
        }

        internal class AttributeKey : Whitelist.TypedValue {
            internal AttributeKey(String value)
                : base(value) {
            }

            internal static Whitelist.AttributeKey ValueOf(String value) {
                return new Whitelist.AttributeKey(value);
            }
        }

        internal class AttributeValue : Whitelist.TypedValue {
            internal AttributeValue(String value)
                : base(value) {
            }

            internal static Whitelist.AttributeValue ValueOf(String value) {
                return new Whitelist.AttributeValue(value);
            }
        }

        internal class Protocol : Whitelist.TypedValue {
            internal Protocol(String value)
                : base(value) {
            }

            internal static Whitelist.Protocol ValueOf(String value) {
                return new Whitelist.Protocol(value);
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
                Whitelist.TypedValue other = (Whitelist.TypedValue)obj;
                if (value == null) {
                    if (other.value != null) {
                        return false;
                    }
                }
                else {
                    if (!value.Equals(other.value)) {
                        return false;
                    }
                }
                return true;
            }

            public override String ToString() {
                return value;
            }
        }
    }
}
