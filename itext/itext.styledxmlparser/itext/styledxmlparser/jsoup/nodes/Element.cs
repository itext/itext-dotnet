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
using System.Text;
using System.Text.RegularExpressions;
using iText.IO.Util;
using iText.StyledXmlParser.Jsoup.Helper;
using iText.StyledXmlParser.Jsoup.Select;

namespace iText.StyledXmlParser.Jsoup.Nodes {
    /// <summary>
    /// A HTML element consists of a tag name, attributes, and child nodes (including text nodes and
    /// other elements).
    /// </summary>
    /// <remarks>
    /// A HTML element consists of a tag name, attributes, and child nodes (including text nodes and
    /// other elements).
    /// From an Element, you can extract data, traverse the node graph, and manipulate the HTML.
    /// </remarks>
    /// <author>Jonathan Hedley, jonathan@hedley.net</author>
    public class Element : iText.StyledXmlParser.Jsoup.Nodes.Node {
        private iText.StyledXmlParser.Jsoup.Parser.Tag tag;

        private static readonly Regex classSplit = iText.IO.Util.StringUtil.RegexCompile("\\s+");

        /// <summary>Create a new, standalone Element.</summary>
        /// <remarks>Create a new, standalone Element. (Standalone in that is has no parent.)</remarks>
        /// <param name="tag">tag of this element</param>
        /// <param name="baseUri">the base URI</param>
        /// <param name="attributes">initial attributes</param>
        /// <seealso cref="AppendChild(Node)"/>
        /// <seealso cref="AppendElement(System.String)"/>
        public Element(iText.StyledXmlParser.Jsoup.Parser.Tag tag, String baseUri, Attributes attributes)
            : base(baseUri, attributes) {
            Validate.NotNull(tag);
            this.tag = tag;
        }

        /// <summary>Create a new Element from a tag and a base URI.</summary>
        /// <param name="tag">element tag</param>
        /// <param name="baseUri">
        /// the base URI of this element. It is acceptable for the base URI to be an empty
        /// string, but not null.
        /// </param>
        /// <seealso cref="iText.StyledXmlParser.Jsoup.Parser.Tag.ValueOf(System.String)"/>
        public Element(iText.StyledXmlParser.Jsoup.Parser.Tag tag, String baseUri)
            : this(tag, baseUri, new Attributes()) {
        }

        public override String NodeName() {
            return tag.GetName();
        }

        /// <summary>Get the name of the tag for this element.</summary>
        /// <remarks>
        /// Get the name of the tag for this element. E.g.
        /// <c>div</c>
        /// </remarks>
        /// <returns>the tag name</returns>
        public virtual String TagName() {
            return tag.GetName();
        }

        /// <summary>Change the tag of this element.</summary>
        /// <remarks>
        /// Change the tag of this element. For example, convert a
        /// <c>&lt;span&gt;</c>
        /// to a
        /// <c>&lt;div&gt;</c>
        /// with
        /// <c>el.tagName("div");</c>.
        /// </remarks>
        /// <param name="tagName">new tag name for this element</param>
        /// <returns>this element, for chaining</returns>
        public virtual iText.StyledXmlParser.Jsoup.Nodes.Element TagName(String tagName) {
            Validate.NotEmpty(tagName, "Tag name must not be empty.");
            tag = iText.StyledXmlParser.Jsoup.Parser.Tag.ValueOf(tagName);
            return this;
        }

        /// <summary>Get the Tag for this element.</summary>
        /// <returns>the tag object</returns>
        public virtual iText.StyledXmlParser.Jsoup.Parser.Tag Tag() {
            return tag;
        }

        /// <summary>Test if this element is a block-level element.</summary>
        /// <remarks>
        /// Test if this element is a block-level element. (E.g.
        /// <c>&lt;div&gt; == true</c>
        /// or an inline element
        /// <c>&lt;p&gt; == false</c>
        /// ).
        /// </remarks>
        /// <returns>true if block, false if not (and thus inline)</returns>
        public virtual bool IsBlock() {
            return tag.IsBlock();
        }

        /// <summary>
        /// Get the
        /// <c>id</c>
        /// attribute of this element.
        /// </summary>
        /// <returns>The id attribute, if present, or an empty string if not.</returns>
        public virtual String Id() {
            return attributes.Get("id");
        }

        /// <summary>Set an attribute value on this element.</summary>
        /// <remarks>
        /// Set an attribute value on this element. If this element already has an attribute with the
        /// key, its value is updated; otherwise, a new attribute is added.
        /// </remarks>
        /// <returns>this element</returns>
        public override iText.StyledXmlParser.Jsoup.Nodes.Node Attr(String attributeKey, String attributeValue) {
            base.Attr(attributeKey, attributeValue);
            return this;
        }

        /// <summary>Set a boolean attribute value on this element.</summary>
        /// <remarks>
        /// Set a boolean attribute value on this element. Setting to <c>true</c> sets the attribute value to "" and
        /// marks the attribute as boolean so no value is written out. Setting to <c>false</c> removes the attribute
        /// with the same key if it exists.
        /// </remarks>
        /// <param name="attributeKey">the attribute key</param>
        /// <param name="attributeValue">the attribute value</param>
        /// <returns>this element</returns>
        public virtual iText.StyledXmlParser.Jsoup.Nodes.Element Attr(String attributeKey, bool attributeValue) {
            attributes.Put(attributeKey, attributeValue);
            return this;
        }

        /// <summary>Get this element's HTML5 custom data attributes.</summary>
        /// <remarks>
        /// Get this element's HTML5 custom data attributes. Each attribute in the element that has a key
        /// starting with "data-" is included the dataset.
        /// <para />
        /// E.g., the element
        /// <c>&lt;div data-package="jsoup" data-language="Java" class="group"&gt;...</c>
        /// has the dataset
        /// <c>package=jsoup, language=java</c>.
        /// <para />
        /// This map is a filtered view of the element's attribute map. Changes to one map (add, remove, update) are reflected
        /// in the other map.
        /// <para />
        /// You can find elements that have data attributes using the
        /// <c>[^data-]</c>
        /// attribute key prefix selector.
        /// </remarks>
        /// <returns>
        /// a map of
        /// <c>key=value</c>
        /// custom data attributes.
        /// </returns>
        public virtual IDictionary<String, String> Dataset() {
            return attributes.Dataset();
        }

        public sealed override iText.StyledXmlParser.Jsoup.Nodes.Node Parent() {
            return parentNode;
        }

        /// <summary>Get this element's parent and ancestors, up to the document root.</summary>
        /// <returns>this element's stack of parents, closest first.</returns>
        public virtual Elements Parents() {
            Elements parents = new Elements();
            AccumulateParents(this, parents);
            return parents;
        }

        private static void AccumulateParents(iText.StyledXmlParser.Jsoup.Nodes.Element el, Elements parents) {
            iText.StyledXmlParser.Jsoup.Nodes.Element parent = (iText.StyledXmlParser.Jsoup.Nodes.Element)el.Parent();
            if (parent != null && !parent.TagName().Equals("#root")) {
                parents.Add(parent);
                AccumulateParents(parent, parents);
            }
        }

        /// <summary>Get a child element of this element, by its 0-based index number.</summary>
        /// <remarks>
        /// Get a child element of this element, by its 0-based index number.
        /// <para />
        /// Note that an element can have both mixed Nodes and Elements as children. This method inspects
        /// a filtered list of children that are elements, and the index is based on that filtered list.
        /// </remarks>
        /// <param name="index">the index number of the element to retrieve</param>
        /// <returns>
        /// the child element, if it exists, otherwise throws an
        /// <c>IndexOutOfBoundsException</c>
        /// </returns>
        /// <seealso cref="Node.ChildNode(int)"/>
        public virtual iText.StyledXmlParser.Jsoup.Nodes.Element Child(int index) {
            return Children()[index];
        }

        /// <summary>Get this element's child elements.</summary>
        /// <remarks>
        /// Get this element's child elements.
        /// <para />
        /// This is effectively a filter on
        /// <see cref="Node.ChildNodes()"/>
        /// to get Element nodes.
        /// </remarks>
        /// <returns>
        /// child elements. If this element has no children, returns an
        /// empty list.
        /// </returns>
        /// <seealso cref="Node.ChildNodes()"/>
        public virtual Elements Children() {
            // create on the fly rather than maintaining two lists. if gets slow, memoize, and mark dirty on change
            IList<iText.StyledXmlParser.Jsoup.Nodes.Element> elements = new List<iText.StyledXmlParser.Jsoup.Nodes.Element
                >(childNodes.Count);
            foreach (iText.StyledXmlParser.Jsoup.Nodes.Node node in childNodes) {
                if (node is iText.StyledXmlParser.Jsoup.Nodes.Element) {
                    elements.Add((iText.StyledXmlParser.Jsoup.Nodes.Element)node);
                }
            }
            return new Elements(elements);
        }

        /// <summary>Get this element's child text nodes.</summary>
        /// <remarks>
        /// Get this element's child text nodes. The list is unmodifiable but the text nodes may be manipulated.
        /// <para />
        /// This is effectively a filter on
        /// <see cref="Node.ChildNodes()"/>
        /// to get Text nodes.
        /// </remarks>
        /// <returns>
        /// child text nodes. If this element has no text nodes, returns an
        /// empty list.
        /// <para />
        /// For example, with the input HTML:
        /// <c>&lt;p&gt;One &lt;span&gt;Two&lt;/span&gt; Three &lt;br&gt; Four&lt;/p&gt;</c>
        /// with the
        /// <c>p</c>
        /// element selected:
        /// <list type="bullet">
        /// <item><description>
        /// <c>p.text()</c>
        /// =
        /// <c>"One Two Three Four"</c>
        /// </description></item>
        /// <item><description>
        /// <c>p.ownText()</c>
        /// =
        /// <c>"One Three Four"</c>
        /// </description></item>
        /// <item><description>
        /// <c>p.children()</c>
        /// =
        /// <c>Elements[&lt;span&gt;, &lt;br&gt;]</c>
        /// </description></item>
        /// <item><description>
        /// <c>p.childNodes()</c>
        /// =
        /// <c>List&lt;Node&gt;["One ", &lt;span&gt;, " Three ", &lt;br&gt;, " Four"]</c>
        /// </description></item>
        /// <item><description>
        /// <c>p.textNodes()</c>
        /// =
        /// <c>List&lt;TextNode&gt;["One ", " Three ", " Four"]</c>
        /// </description></item>
        /// </list>
        /// </returns>
        public virtual IList<TextNode> TextNodes() {
            IList<TextNode> textNodes = new List<TextNode>();
            foreach (iText.StyledXmlParser.Jsoup.Nodes.Node node in childNodes) {
                if (node is TextNode) {
                    textNodes.Add((TextNode)node);
                }
            }
            return JavaCollectionsUtil.UnmodifiableList(textNodes);
        }

        /// <summary>Get this element's child data nodes.</summary>
        /// <remarks>
        /// Get this element's child data nodes. The list is unmodifiable but the data nodes may be manipulated.
        /// <para />
        /// This is effectively a filter on
        /// <see cref="Node.ChildNodes()"/>
        /// to get Data nodes.
        /// </remarks>
        /// <returns>
        /// child data nodes. If this element has no data nodes, returns an
        /// empty list.
        /// </returns>
        /// <seealso cref="Data()"/>
        public virtual IList<DataNode> DataNodes() {
            IList<DataNode> dataNodes = new List<DataNode>();
            foreach (iText.StyledXmlParser.Jsoup.Nodes.Node node in childNodes) {
                if (node is DataNode) {
                    dataNodes.Add((DataNode)node);
                }
            }
            return JavaCollectionsUtil.UnmodifiableList(dataNodes);
        }

        /// <summary>
        /// Find elements that match the
        /// <see cref="iText.StyledXmlParser.Jsoup.Select.Selector"/>
        /// CSS query, with this element as the starting context.
        /// </summary>
        /// <remarks>
        /// Find elements that match the
        /// <see cref="iText.StyledXmlParser.Jsoup.Select.Selector"/>
        /// CSS query, with this element as the starting context. Matched elements
        /// may include this element, or any of its children.
        /// <para />
        /// This method is generally more powerful to use than the DOM-type
        /// <c>getElementBy*</c>
        /// methods, because
        /// multiple filters can be combined, e.g.:
        /// <list type="bullet">
        /// <item><description>
        /// <c>el.select("a[href]")</c>
        /// - finds links (
        /// <c>a</c>
        /// tags with
        /// <c>href</c>
        /// attributes)
        /// </description></item>
        /// <item><description>
        /// <c>el.select("a[href*=example.com]")</c>
        /// - finds links pointing to example.com (loosely)
        /// </description></item>
        /// </list>
        /// See the query syntax documentation in
        /// <see cref="iText.StyledXmlParser.Jsoup.Select.Selector"/>.
        /// </remarks>
        /// <param name="cssQuery">
        /// a
        /// <see cref="iText.StyledXmlParser.Jsoup.Select.Selector"/>
        /// CSS-like query
        /// </param>
        /// <returns>elements that match the query (empty if none match)</returns>
        /// <seealso cref="iText.StyledXmlParser.Jsoup.Select.Selector"/>
        public virtual Elements Select(String cssQuery) {
            return Selector.Select(cssQuery, this);
        }

        /// <summary>Add a node child node to this element.</summary>
        /// <param name="child">node to add.</param>
        /// <returns>this element, so that you can add more child nodes or elements.</returns>
        public virtual iText.StyledXmlParser.Jsoup.Nodes.Element AppendChild(iText.StyledXmlParser.Jsoup.Nodes.Node
             child) {
            Validate.NotNull(child);
            // was - Node#addChildren(child). short-circuits an array create and a loop.
            ReparentChild(child);
            EnsureChildNodes();
            childNodes.Add(child);
            child.SetSiblingIndex(childNodes.Count - 1);
            return this;
        }

        /// <summary>Add a node to the start of this element's children.</summary>
        /// <param name="child">node to add.</param>
        /// <returns>this element, so that you can add more child nodes or elements.</returns>
        public virtual iText.StyledXmlParser.Jsoup.Nodes.Element PrependChild(iText.StyledXmlParser.Jsoup.Nodes.Node
             child) {
            return InsertChild(0, child);
        }

        /// <summary>Inserts the given child node into this element at the specified index.</summary>
        /// <remarks>
        /// Inserts the given child node into this element at the specified index. Current node will be shifted to the
        /// right. The inserted nodes will be moved from their current parent. To prevent moving, copy the node first.
        /// </remarks>
        /// <param name="index">
        /// 0-based index to insert children at. Specify
        /// <c>0</c>
        /// to insert at the start,
        /// <c>-1</c>
        /// at the
        /// end
        /// </param>
        /// <param name="child">child node to insert</param>
        /// <returns>this element, for chaining.</returns>
        public virtual iText.StyledXmlParser.Jsoup.Nodes.Element InsertChild(int index, iText.StyledXmlParser.Jsoup.Nodes.Node
             child) {
            if (index == -1) {
                return AppendChild(child);
            }
            Validate.NotNull(child);
            AddChildren(index, child);
            return this;
        }

        /// <summary>Inserts the given child nodes into this element at the specified index.</summary>
        /// <remarks>
        /// Inserts the given child nodes into this element at the specified index. Current nodes will be shifted to the
        /// right. The inserted nodes will be moved from their current parent. To prevent moving, copy the nodes first.
        /// </remarks>
        /// <param name="index">
        /// 0-based index to insert children at. Specify
        /// <c>0</c>
        /// to insert at the start,
        /// <c>-1</c>
        /// at the
        /// end
        /// </param>
        /// <param name="children">child nodes to insert</param>
        /// <returns>this element, for chaining.</returns>
        public virtual iText.StyledXmlParser.Jsoup.Nodes.Element InsertChildren<_T0>(int index, ICollection<_T0> children
            )
            where _T0 : iText.StyledXmlParser.Jsoup.Nodes.Node {
            Validate.NotNull(children, "Children collection to be inserted must not be null.");
            int currentSize = ChildNodeSize();
            if (index < 0) {
                index += currentSize + 1;
            }
            // roll around
            Validate.IsTrue(index >= 0 && index <= currentSize, "Insert position out of bounds.");
            List<iText.StyledXmlParser.Jsoup.Nodes.Node> nodes = new List<iText.StyledXmlParser.Jsoup.Nodes.Node>(children
                );
            iText.StyledXmlParser.Jsoup.Nodes.Node[] nodeArray = nodes.ToArray(new iText.StyledXmlParser.Jsoup.Nodes.Node
                [nodes.Count]);
            AddChildren(index, nodeArray);
            return this;
        }

        /// <summary>Create a new element by tag name, and add it as the last child.</summary>
        /// <param name="tagName">
        /// the name of the tag (e.g.
        /// <c>div</c>
        /// ).
        /// </param>
        /// <returns>
        /// the new element, to allow you to add content to it, e.g.:
        /// <c>parent.appendElement("h1").attr("id", "header").text("Welcome");</c>
        /// </returns>
        public virtual iText.StyledXmlParser.Jsoup.Nodes.Element AppendElement(String tagName) {
            iText.StyledXmlParser.Jsoup.Nodes.Element child = new iText.StyledXmlParser.Jsoup.Nodes.Element(iText.StyledXmlParser.Jsoup.Parser.Tag
                .ValueOf(tagName), BaseUri());
            AppendChild(child);
            return child;
        }

        /// <summary>Create a new element by tag name, and add it as the first child.</summary>
        /// <param name="tagName">
        /// the name of the tag (e.g.
        /// <c>div</c>
        /// ).
        /// </param>
        /// <returns>
        /// the new element, to allow you to add content to it, e.g.:
        /// <c>parent.prependElement("h1").attr("id", "header").text("Welcome");</c>
        /// </returns>
        public virtual iText.StyledXmlParser.Jsoup.Nodes.Element PrependElement(String tagName) {
            iText.StyledXmlParser.Jsoup.Nodes.Element child = new iText.StyledXmlParser.Jsoup.Nodes.Element(iText.StyledXmlParser.Jsoup.Parser.Tag
                .ValueOf(tagName), BaseUri());
            PrependChild(child);
            return child;
        }

        /// <summary>Create and append a new TextNode to this element.</summary>
        /// <param name="text">the unencoded text to add</param>
        /// <returns>this element</returns>
        public virtual iText.StyledXmlParser.Jsoup.Nodes.Element AppendText(String text) {
            Validate.NotNull(text);
            TextNode node = new TextNode(text, BaseUri());
            AppendChild(node);
            return this;
        }

        /// <summary>Create and prepend a new TextNode to this element.</summary>
        /// <param name="text">the unencoded text to add</param>
        /// <returns>this element</returns>
        public virtual iText.StyledXmlParser.Jsoup.Nodes.Element PrependText(String text) {
            Validate.NotNull(text);
            TextNode node = new TextNode(text, BaseUri());
            PrependChild(node);
            return this;
        }

        /// <summary>Add inner HTML to this element.</summary>
        /// <remarks>Add inner HTML to this element. The supplied HTML will be parsed, and each node appended to the end of the children.
        ///     </remarks>
        /// <param name="html">HTML to add inside this element, after the existing HTML</param>
        /// <returns>this element</returns>
        /// <seealso cref="Html(System.String)"/>
        public virtual iText.StyledXmlParser.Jsoup.Nodes.Element Append(String html) {
            Validate.NotNull(html);
            IList<iText.StyledXmlParser.Jsoup.Nodes.Node> nodes = iText.StyledXmlParser.Jsoup.Parser.Parser.ParseFragment
                (html, this, BaseUri());
            AddChildren(nodes.ToArray(new iText.StyledXmlParser.Jsoup.Nodes.Node[nodes.Count]));
            return this;
        }

        /// <summary>Add inner HTML into this element.</summary>
        /// <remarks>Add inner HTML into this element. The supplied HTML will be parsed, and each node prepended to the start of the element's children.
        ///     </remarks>
        /// <param name="html">HTML to add inside this element, before the existing HTML</param>
        /// <returns>this element</returns>
        /// <seealso cref="Html(System.String)"/>
        public virtual iText.StyledXmlParser.Jsoup.Nodes.Element Prepend(String html) {
            Validate.NotNull(html);
            IList<iText.StyledXmlParser.Jsoup.Nodes.Node> nodes = iText.StyledXmlParser.Jsoup.Parser.Parser.ParseFragment
                (html, this, BaseUri());
            AddChildren(0, nodes.ToArray(new iText.StyledXmlParser.Jsoup.Nodes.Node[nodes.Count]));
            return this;
        }

        /// <summary>Insert the specified HTML into the DOM before this element (as a preceding sibling).</summary>
        /// <param name="html">HTML to add before this element</param>
        /// <returns>this element, for chaining</returns>
        /// <seealso cref="After(System.String)"/>
        public override iText.StyledXmlParser.Jsoup.Nodes.Node Before(String html) {
            return base.Before(html);
        }

        /// <summary>Insert the specified node into the DOM before this node (as a preceding sibling).</summary>
        /// <param name="node">to add before this element</param>
        /// <returns>this Element, for chaining</returns>
        /// <seealso cref="After(Node)"/>
        public override iText.StyledXmlParser.Jsoup.Nodes.Node Before(iText.StyledXmlParser.Jsoup.Nodes.Node node) {
            return base.Before(node);
        }

        /// <summary>Insert the specified HTML into the DOM after this element (as a following sibling).</summary>
        /// <param name="html">HTML to add after this element</param>
        /// <returns>this element, for chaining</returns>
        /// <seealso cref="Before(System.String)"/>
        public override iText.StyledXmlParser.Jsoup.Nodes.Node After(String html) {
            return (iText.StyledXmlParser.Jsoup.Nodes.Element)base.After(html);
        }

        /// <summary>Insert the specified node into the DOM after this node (as a following sibling).</summary>
        /// <param name="node">to add after this element</param>
        /// <returns>this element, for chaining</returns>
        /// <seealso cref="Before(Node)"/>
        public override iText.StyledXmlParser.Jsoup.Nodes.Node After(iText.StyledXmlParser.Jsoup.Nodes.Node node) {
            return (iText.StyledXmlParser.Jsoup.Nodes.Element)base.After(node);
        }

        /// <summary>Remove all of the element's child nodes.</summary>
        /// <remarks>Remove all of the element's child nodes. Any attributes are left as-is.</remarks>
        /// <returns>this element</returns>
        public virtual iText.StyledXmlParser.Jsoup.Nodes.Element Empty() {
            childNodes.Clear();
            return this;
        }

        /// <summary>Wrap the supplied HTML around this element.</summary>
        /// <param name="html">
        /// HTML to wrap around this element, e.g.
        /// <c>&lt;div class="head"&gt;&lt;/div&gt;</c>
        /// . Can be arbitrarily deep.
        /// </param>
        /// <returns>this element, for chaining.</returns>
        public override iText.StyledXmlParser.Jsoup.Nodes.Node Wrap(String html) {
            return base.Wrap(html);
        }

        /// <summary>Get a CSS selector that will uniquely select this element.</summary>
        /// <remarks>
        /// Get a CSS selector that will uniquely select this element.
        /// <para />
        /// If the element has an ID, returns #id;
        /// otherwise returns the parent (if any) CSS selector, followed by
        /// <literal>'&gt;'</literal>
        /// ,
        /// followed by a unique selector for the element (tag.class.class:nth-child(n)).
        /// </remarks>
        /// <returns>the CSS Path that can be used to retrieve the element in a selector.</returns>
        public virtual String CssSelector() {
            if (Id().Length > 0) {
                return "#" + Id();
            }
            // Translate HTML namespace ns:tag to CSS namespace syntax ns|tag
            String tagName = TagName().Replace(':', '|');
            StringBuilder selector = new StringBuilder(tagName);
            String classes = iText.StyledXmlParser.Jsoup.Helper.StringUtil.Join(ClassNames(), ".");
            if (classes.Length > 0) {
                selector.Append('.').Append(classes);
            }
            if (Parent() == null || Parent() is Document) {
                // don't add Document to selector, as will always have a html node
                return selector.ToString();
            }
            selector.Insert(0, " > ");
            if (((iText.StyledXmlParser.Jsoup.Nodes.Element)Parent()).Select(selector.ToString()).Count > 1) {
                selector.Append(MessageFormatUtil.Format(":nth-child({0})", ElementSiblingIndex() + 1));
            }
            return ((iText.StyledXmlParser.Jsoup.Nodes.Element)Parent()).CssSelector() + selector.ToString();
        }

        /// <summary>Get sibling elements.</summary>
        /// <remarks>
        /// Get sibling elements. If the element has no sibling elements, returns an empty list. An element is not a sibling
        /// of itself, so will not be included in the returned list.
        /// </remarks>
        /// <returns>sibling elements</returns>
        public virtual Elements SiblingElements() {
            if (parentNode == null) {
                return new Elements(0);
            }
            IList<iText.StyledXmlParser.Jsoup.Nodes.Element> elements = ((iText.StyledXmlParser.Jsoup.Nodes.Element)Parent
                ()).Children();
            Elements siblings = new Elements(elements.Count - 1);
            foreach (iText.StyledXmlParser.Jsoup.Nodes.Element el in elements) {
                if (el != this) {
                    siblings.Add(el);
                }
            }
            return siblings;
        }

        /// <summary>Gets the next sibling element of this element.</summary>
        /// <remarks>
        /// Gets the next sibling element of this element. E.g., if a
        /// <c>div</c>
        /// contains two
        /// <c>p</c>
        /// s,
        /// the
        /// <c>nextElementSibling</c>
        /// of the first
        /// <c>p</c>
        /// is the second
        /// <c>p</c>.
        /// <para />
        /// This is similar to
        /// <see cref="Node.NextSibling()"/>
        /// , but specifically finds only Elements
        /// </remarks>
        /// <returns>the next element, or null if there is no next element</returns>
        /// <seealso cref="PreviousElementSibling()"/>
        public virtual iText.StyledXmlParser.Jsoup.Nodes.Element NextElementSibling() {
            if (parentNode == null) {
                return null;
            }
            IList<iText.StyledXmlParser.Jsoup.Nodes.Element> siblings = ((iText.StyledXmlParser.Jsoup.Nodes.Element)Parent
                ()).Children();
            int index = IndexInList(this, siblings);
            Validate.IsTrue(index >= 0);
            //Validate.notNull(index);
            if (siblings.Count > index + 1) {
                return siblings[index + 1];
            }
            else {
                return null;
            }
        }

        /// <summary>Gets the previous element sibling of this element.</summary>
        /// <returns>the previous element, or null if there is no previous element</returns>
        /// <seealso cref="NextElementSibling()"/>
        public virtual iText.StyledXmlParser.Jsoup.Nodes.Element PreviousElementSibling() {
            if (parentNode == null) {
                return null;
            }
            IList<iText.StyledXmlParser.Jsoup.Nodes.Element> siblings = ((iText.StyledXmlParser.Jsoup.Nodes.Element)Parent
                ()).Children();
            int index = IndexInList(this, siblings);
            Validate.IsTrue(index >= 0);
            if (index > 0) {
                return siblings[index - 1];
            }
            else {
                return null;
            }
        }

        /// <summary>Gets the first element sibling of this element.</summary>
        /// <returns>the first sibling that is an element (aka the parent's first element child)</returns>
        public virtual iText.StyledXmlParser.Jsoup.Nodes.Element FirstElementSibling() {
            // todo: should firstSibling() exclude this?
            IList<iText.StyledXmlParser.Jsoup.Nodes.Element> siblings = ((iText.StyledXmlParser.Jsoup.Nodes.Element)Parent
                ()).Children();
            return siblings.Count > 1 ? siblings[0] : null;
        }

        /// <summary>Get the list index of this element in its element sibling list.</summary>
        /// <remarks>
        /// Get the list index of this element in its element sibling list. I.e. if this is the first element
        /// sibling, returns 0.
        /// </remarks>
        /// <returns>position in element sibling list</returns>
        public virtual int ElementSiblingIndex() {
            if (Parent() == null) {
                return 0;
            }
            return IndexInList(this, ((iText.StyledXmlParser.Jsoup.Nodes.Element)Parent()).Children());
        }

        /// <summary>Gets the last element sibling of this element</summary>
        /// <returns>the last sibling that is an element (aka the parent's last element child)</returns>
        public virtual iText.StyledXmlParser.Jsoup.Nodes.Element LastElementSibling() {
            IList<iText.StyledXmlParser.Jsoup.Nodes.Element> siblings = ((iText.StyledXmlParser.Jsoup.Nodes.Element)Parent
                ()).Children();
            return siblings.Count > 1 ? siblings[siblings.Count - 1] : null;
        }

        private static int IndexInList<E>(iText.StyledXmlParser.Jsoup.Nodes.Element search, IList<E> elements)
            where E : iText.StyledXmlParser.Jsoup.Nodes.Element {
            Validate.NotNull(search);
            Validate.NotNull(elements);
            for (int i = 0; i < elements.Count; i++) {
                E element = elements[i];
                if (element == search) {
                    return i;
                }
            }
            return -1;
        }

        // DOM type methods
        /// <summary>Finds elements, including and recursively under this element, with the specified tag name.</summary>
        /// <param name="tagName">The tag name to search for (case insensitively).</param>
        /// <returns>a matching unmodifiable list of elements. Will be empty if this element and none of its children match.
        ///     </returns>
        public virtual Elements GetElementsByTag(String tagName) {
            Validate.NotEmpty(tagName);
            tagName = tagName.ToLowerInvariant().Trim();
            return Collector.Collect(new Evaluator.Tag(tagName), this);
        }

        /// <summary>Find an element by ID, including or under this element.</summary>
        /// <remarks>
        /// Find an element by ID, including or under this element.
        /// <para />
        /// Note that this finds the first matching ID, starting with this element. If you search down from a different
        /// starting point, it is possible to find a different element by ID. For unique element by ID within a Document,
        /// use
        /// <see cref="GetElementById(System.String)"/>
        /// </remarks>
        /// <param name="id">The ID to search for.</param>
        /// <returns>The first matching element by ID, starting with this element, or null if none found.</returns>
        public virtual iText.StyledXmlParser.Jsoup.Nodes.Element GetElementById(String id) {
            Validate.NotEmpty(id);
            Elements elements = Collector.Collect(new Evaluator.ID(id), this);
            if (elements.Count > 0) {
                return elements[0];
            }
            else {
                return null;
            }
        }

        /// <summary>Find elements that have this class, including or under this element.</summary>
        /// <remarks>
        /// Find elements that have this class, including or under this element. Case insensitive.
        /// <para />
        /// Elements can have multiple classes (e.g.
        /// <c>&lt;div class="header round first"&gt;</c>
        /// . This method
        /// checks each class, so you can find the above with
        /// <c>el.getElementsByClass("header");</c>.
        /// </remarks>
        /// <param name="className">the name of the class to search for.</param>
        /// <returns>elements with the supplied class name, empty if none</returns>
        /// <seealso cref="HasClass(System.String)"/>
        /// <seealso cref="ClassNames()"/>
        public virtual Elements GetElementsByClass(String className) {
            Validate.NotEmpty(className);
            return Collector.Collect(new Evaluator.Class(className), this);
        }

        /// <summary>Find elements that have a named attribute set.</summary>
        /// <remarks>Find elements that have a named attribute set. Case insensitive.</remarks>
        /// <param name="key">
        /// name of the attribute, e.g.
        /// <c>href</c>
        /// </param>
        /// <returns>elements that have this attribute, empty if none</returns>
        public virtual Elements GetElementsByAttribute(String key) {
            Validate.NotEmpty(key);
            key = key.Trim().ToLowerInvariant();
            return Collector.Collect(new Evaluator.Attribute(key), this);
        }

        /// <summary>Find elements that have an attribute name starting with the supplied prefix.</summary>
        /// <remarks>
        /// Find elements that have an attribute name starting with the supplied prefix. Use
        /// <c>data-</c>
        /// to find elements
        /// that have HTML5 datasets.
        /// </remarks>
        /// <param name="keyPrefix">
        /// name prefix of the attribute e.g.
        /// <c>data-</c>
        /// </param>
        /// <returns>elements that have attribute names that start with with the prefix, empty if none.</returns>
        public virtual Elements GetElementsByAttributeStarting(String keyPrefix) {
            Validate.NotEmpty(keyPrefix);
            keyPrefix = keyPrefix.Trim().ToLowerInvariant();
            return Collector.Collect(new Evaluator.AttributeStarting(keyPrefix), this);
        }

        /// <summary>Find elements that have an attribute with the specific value.</summary>
        /// <remarks>Find elements that have an attribute with the specific value. Case insensitive.</remarks>
        /// <param name="key">name of the attribute</param>
        /// <param name="value">value of the attribute</param>
        /// <returns>elements that have this attribute with this value, empty if none</returns>
        public virtual Elements GetElementsByAttributeValue(String key, String value) {
            return Collector.Collect(new Evaluator.AttributeWithValue(key, value), this);
        }

        /// <summary>Find elements that either do not have this attribute, or have it with a different value.</summary>
        /// <remarks>Find elements that either do not have this attribute, or have it with a different value. Case insensitive.
        ///     </remarks>
        /// <param name="key">name of the attribute</param>
        /// <param name="value">value of the attribute</param>
        /// <returns>elements that do not have a matching attribute</returns>
        public virtual Elements GetElementsByAttributeValueNot(String key, String value) {
            return Collector.Collect(new Evaluator.AttributeWithValueNot(key, value), this);
        }

        /// <summary>Find elements that have attributes that start with the value prefix.</summary>
        /// <remarks>Find elements that have attributes that start with the value prefix. Case insensitive.</remarks>
        /// <param name="key">name of the attribute</param>
        /// <param name="valuePrefix">start of attribute value</param>
        /// <returns>elements that have attributes that start with the value prefix</returns>
        public virtual Elements GetElementsByAttributeValueStarting(String key, String valuePrefix) {
            return Collector.Collect(new Evaluator.AttributeWithValueStarting(key, valuePrefix), this);
        }

        /// <summary>Find elements that have attributes that end with the value suffix.</summary>
        /// <remarks>Find elements that have attributes that end with the value suffix. Case insensitive.</remarks>
        /// <param name="key">name of the attribute</param>
        /// <param name="valueSuffix">end of the attribute value</param>
        /// <returns>elements that have attributes that end with the value suffix</returns>
        public virtual Elements GetElementsByAttributeValueEnding(String key, String valueSuffix) {
            return Collector.Collect(new Evaluator.AttributeWithValueEnding(key, valueSuffix), this);
        }

        /// <summary>Find elements that have attributes whose value contains the match string.</summary>
        /// <remarks>Find elements that have attributes whose value contains the match string. Case insensitive.</remarks>
        /// <param name="key">name of the attribute</param>
        /// <param name="match">substring of value to search for</param>
        /// <returns>elements that have attributes containing this text</returns>
        public virtual Elements GetElementsByAttributeValueContaining(String key, String match) {
            return Collector.Collect(new Evaluator.AttributeWithValueContaining(key, match), this);
        }

        /// <summary>Find elements that have attributes whose values match the supplied regular expression.</summary>
        /// <param name="key">name of the attribute</param>
        /// <param name="pattern">compiled regular expression to match against attribute values</param>
        /// <returns>elements that have attributes matching this regular expression</returns>
        public virtual Elements GetElementsByAttributeValueMatching(String key, Regex pattern) {
            return Collector.Collect(new Evaluator.AttributeWithValueMatching(key, pattern), this);
        }

        /// <summary>Find elements that have attributes whose values match the supplied regular expression.</summary>
        /// <param name="key">name of the attribute</param>
        /// <param name="regex">
        /// regular expression to match against attribute values.
        /// You can use <a href="http://java.sun.com/docs/books/tutorial/essential/regex/pattern.html#embedded">embedded flags</a>
        /// (such as (?i) and (?m) to control regex options.
        /// </param>
        /// <returns>elements that have attributes matching this regular expression</returns>
        public virtual Elements GetElementsByAttributeValueMatching(String key, String regex) {
            Regex pattern;
            try {
                pattern = iText.IO.Util.StringUtil.RegexCompile(regex);
            }
            catch (ArgumentException e) {
                throw new ArgumentException("Pattern syntax error: " + regex, e);
            }
            return GetElementsByAttributeValueMatching(key, pattern);
        }

        /// <summary>Find elements whose sibling index is less than the supplied index.</summary>
        /// <param name="index">0-based index</param>
        /// <returns>elements less than index</returns>
        public virtual Elements GetElementsByIndexLessThan(int index) {
            return Collector.Collect(new Evaluator.IndexLessThan(index), this);
        }

        /// <summary>Find elements whose sibling index is greater than the supplied index.</summary>
        /// <param name="index">0-based index</param>
        /// <returns>elements greater than index</returns>
        public virtual Elements GetElementsByIndexGreaterThan(int index) {
            return Collector.Collect(new Evaluator.IndexGreaterThan(index), this);
        }

        /// <summary>Find elements whose sibling index is equal to the supplied index.</summary>
        /// <param name="index">0-based index</param>
        /// <returns>elements equal to index</returns>
        public virtual Elements GetElementsByIndexEquals(int index) {
            return Collector.Collect(new Evaluator.IndexEquals(index), this);
        }

        /// <summary>Find elements that contain the specified string.</summary>
        /// <remarks>
        /// Find elements that contain the specified string. The search is case insensitive. The text may appear directly
        /// in the element, or in any of its descendants.
        /// </remarks>
        /// <param name="searchText">to look for in the element's text</param>
        /// <returns>elements that contain the string, case insensitive.</returns>
        /// <seealso cref="Text()"/>
        public virtual Elements GetElementsContainingText(String searchText) {
            return Collector.Collect(new Evaluator.ContainsText(searchText), this);
        }

        /// <summary>Find elements that directly contain the specified string.</summary>
        /// <remarks>
        /// Find elements that directly contain the specified string. The search is case insensitive. The text must appear directly
        /// in the element, not in any of its descendants.
        /// </remarks>
        /// <param name="searchText">to look for in the element's own text</param>
        /// <returns>elements that contain the string, case insensitive.</returns>
        /// <seealso cref="OwnText()"/>
        public virtual Elements GetElementsContainingOwnText(String searchText) {
            return Collector.Collect(new Evaluator.ContainsOwnText(searchText), this);
        }

        /// <summary>Find elements whose text matches the supplied regular expression.</summary>
        /// <param name="pattern">regular expression to match text against</param>
        /// <returns>elements matching the supplied regular expression.</returns>
        /// <seealso cref="Text()"/>
        public virtual Elements GetElementsMatchingText(Regex pattern) {
            return Collector.Collect(new MatchesElement(pattern), this);
        }

        /// <summary>Find elements whose text matches the supplied regular expression.</summary>
        /// <param name="regex">
        /// regular expression to match text against.
        /// You can use <a href="http://java.sun.com/docs/books/tutorial/essential/regex/pattern.html#embedded">embedded flags</a>
        /// (such as (?i) and (?m) to control regex options.
        /// </param>
        /// <returns>elements matching the supplied regular expression.</returns>
        /// <seealso cref="Text()"/>
        public virtual Elements GetElementsMatchingText(String regex) {
            Regex pattern;
            try {
                pattern = iText.IO.Util.StringUtil.RegexCompile(regex);
            }
            catch (ArgumentException e) {
                throw new ArgumentException("Pattern syntax error: " + regex, e);
            }
            return GetElementsMatchingText(pattern);
        }

        /// <summary>Find elements whose own text matches the supplied regular expression.</summary>
        /// <param name="pattern">regular expression to match text against</param>
        /// <returns>elements matching the supplied regular expression.</returns>
        /// <seealso cref="OwnText()"/>
        public virtual Elements GetElementsMatchingOwnText(Regex pattern) {
            return Collector.Collect(new Evaluator.MatchesOwn(pattern), this);
        }

        /// <summary>Find elements whose text matches the supplied regular expression.</summary>
        /// <param name="regex">
        /// regular expression to match text against.
        /// You can use <a href="http://java.sun.com/docs/books/tutorial/essential/regex/pattern.html#embedded">embedded flags</a>
        /// (such as (?i) and (?m) to control regex options.
        /// </param>
        /// <returns>elements matching the supplied regular expression.</returns>
        /// <seealso cref="OwnText()"/>
        public virtual Elements GetElementsMatchingOwnText(String regex) {
            Regex pattern;
            try {
                pattern = iText.IO.Util.StringUtil.RegexCompile(regex);
            }
            catch (ArgumentException e) {
                throw new ArgumentException("Pattern syntax error: " + regex, e);
            }
            return GetElementsMatchingOwnText(pattern);
        }

        /// <summary>Find all elements under this element (including self, and children of children).</summary>
        /// <returns>all elements</returns>
        public virtual Elements GetAllElements() {
            return Collector.Collect(new Evaluator.AllElements(), this);
        }

        /// <summary>Gets the combined text of this element and all its children.</summary>
        /// <remarks>
        /// Gets the combined text of this element and all its children. Whitespace is normalized and trimmed.
        /// <para />
        /// For example, given HTML
        /// <c>&lt;p&gt;Hello  &lt;b&gt;there&lt;/b&gt; now! &lt;/p&gt;</c>
        /// ,
        /// <c>p.text()</c>
        /// returns
        /// <c>"Hello there now!"</c>
        /// </remarks>
        /// <returns>unencoded text, or empty string if none.</returns>
        /// <seealso cref="OwnText()"/>
        /// <seealso cref="TextNodes()"/>
        public virtual String Text() {
            StringBuilder accum = new StringBuilder();
            new NodeTraversor(new _NodeVisitor_955(accum)).Traverse(this);
            return accum.ToString().Trim();
        }

        private sealed class _NodeVisitor_955 : NodeVisitor {
            public _NodeVisitor_955(StringBuilder accum) {
                this.accum = accum;
            }

            public void Head(iText.StyledXmlParser.Jsoup.Nodes.Node node, int depth) {
                if (node is TextNode) {
                    TextNode textNode = (TextNode)node;
                    iText.StyledXmlParser.Jsoup.Nodes.Element.AppendNormalisedText(accum, textNode);
                }
                else {
                    if (node is iText.StyledXmlParser.Jsoup.Nodes.Element) {
                        iText.StyledXmlParser.Jsoup.Nodes.Element element = (iText.StyledXmlParser.Jsoup.Nodes.Element)node;
                        if (accum.Length > 0 && (element.IsBlock() || element.tag.GetName().Equals("br")) && !TextNode.LastCharIsWhitespace
                            (accum)) {
                            accum.Append(" ");
                        }
                    }
                }
            }

            public void Tail(iText.StyledXmlParser.Jsoup.Nodes.Node node, int depth) {
            }

            private readonly StringBuilder accum;
        }

        /// <summary>Gets the text owned by this element only; does not get the combined text of all children.</summary>
        /// <remarks>
        /// Gets the text owned by this element only; does not get the combined text of all children.
        /// <para />
        /// For example, given HTML
        /// <c>&lt;p&gt;Hello &lt;b&gt;there&lt;/b&gt; now!&lt;/p&gt;</c>
        /// ,
        /// <c>p.ownText()</c>
        /// returns
        /// <c>"Hello now!"</c>
        /// ,
        /// whereas
        /// <c>p.text()</c>
        /// returns
        /// <c>"Hello there now!"</c>.
        /// Note that the text within the
        /// <c>b</c>
        /// element is not returned, as it is not a direct child of the
        /// <c>p</c>
        /// element.
        /// </remarks>
        /// <returns>unencoded text, or empty string if none.</returns>
        /// <seealso cref="Text()"/>
        /// <seealso cref="TextNodes()"/>
        public virtual String OwnText() {
            StringBuilder sb = new StringBuilder();
            OwnText(sb);
            return sb.ToString().Trim();
        }

        private void OwnText(StringBuilder accum) {
            foreach (iText.StyledXmlParser.Jsoup.Nodes.Node child in childNodes) {
                if (child is TextNode) {
                    TextNode textNode = (TextNode)child;
                    AppendNormalisedText(accum, textNode);
                }
                else {
                    if (child is iText.StyledXmlParser.Jsoup.Nodes.Element) {
                        AppendWhitespaceIfBr((iText.StyledXmlParser.Jsoup.Nodes.Element)child, accum);
                    }
                }
            }
        }

        private static void AppendNormalisedText(StringBuilder accum, TextNode textNode) {
            String text = textNode.GetWholeText();
            if (PreserveWhitespace(textNode.parentNode)) {
                accum.Append(text);
            }
            else {
                iText.StyledXmlParser.Jsoup.Helper.StringUtil.AppendNormalisedWhitespace(accum, text, TextNode.LastCharIsWhitespace
                    (accum));
            }
        }

        private static void AppendWhitespaceIfBr(iText.StyledXmlParser.Jsoup.Nodes.Element element, StringBuilder 
            accum) {
            if (element.tag.GetName().Equals("br") && !TextNode.LastCharIsWhitespace(accum)) {
                accum.Append(" ");
            }
        }

        internal static bool PreserveWhitespace(iText.StyledXmlParser.Jsoup.Nodes.Node node) {
            // looks only at this element and one level up, to prevent recursion & needless stack searches
            if (node != null && node is iText.StyledXmlParser.Jsoup.Nodes.Element) {
                iText.StyledXmlParser.Jsoup.Nodes.Element element = (iText.StyledXmlParser.Jsoup.Nodes.Element)node;
                return element.tag.PreserveWhitespace() || element.Parent() != null && ((iText.StyledXmlParser.Jsoup.Nodes.Element
                    )element.Parent()).tag.PreserveWhitespace();
            }
            return false;
        }

        /// <summary>Set the text of this element.</summary>
        /// <remarks>Set the text of this element. Any existing contents (text or elements) will be cleared</remarks>
        /// <param name="text">unencoded text</param>
        /// <returns>this element</returns>
        public virtual iText.StyledXmlParser.Jsoup.Nodes.Element Text(String text) {
            Validate.NotNull(text);
            Empty();
            TextNode textNode = new TextNode(text, baseUri);
            AppendChild(textNode);
            return this;
        }

        /// <summary>Test if this element has any text content (that is not just whitespace).</summary>
        /// <returns>true if element has non-blank text content.</returns>
        public virtual bool HasText() {
            foreach (iText.StyledXmlParser.Jsoup.Nodes.Node child in childNodes) {
                if (child is TextNode) {
                    TextNode textNode = (TextNode)child;
                    if (!textNode.IsBlank()) {
                        return true;
                    }
                }
                else {
                    if (child is iText.StyledXmlParser.Jsoup.Nodes.Element) {
                        iText.StyledXmlParser.Jsoup.Nodes.Element el = (iText.StyledXmlParser.Jsoup.Nodes.Element)child;
                        if (el.HasText()) {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>Get the combined data of this element.</summary>
        /// <remarks>
        /// Get the combined data of this element. Data is e.g. the inside of a
        /// <c>script</c>
        /// tag.
        /// </remarks>
        /// <returns>the data, or empty string if none</returns>
        /// <seealso cref="DataNodes()"/>
        public virtual String Data() {
            StringBuilder sb = new StringBuilder();
            foreach (iText.StyledXmlParser.Jsoup.Nodes.Node childNode in childNodes) {
                if (childNode is DataNode) {
                    DataNode data = (DataNode)childNode;
                    sb.Append(data.GetWholeData());
                }
                else {
                    if (childNode is iText.StyledXmlParser.Jsoup.Nodes.Element) {
                        iText.StyledXmlParser.Jsoup.Nodes.Element element = (iText.StyledXmlParser.Jsoup.Nodes.Element)childNode;
                        String elementData = element.Data();
                        sb.Append(elementData);
                    }
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// Gets the literal value of this element's "class" attribute, which may include multiple class names, space
        /// separated.
        /// </summary>
        /// <remarks>
        /// Gets the literal value of this element's "class" attribute, which may include multiple class names, space
        /// separated. (E.g. on <c>&lt;div class="header gray"&gt;</c> returns, "<c>header gray</c>")
        /// </remarks>
        /// <returns>The literal class attribute, or <b>empty string</b> if no class attribute set.</returns>
        public virtual String ClassName() {
            return Attr("class").Trim();
        }

        /// <summary>Get all of the element's class names.</summary>
        /// <remarks>
        /// Get all of the element's class names. E.g. on element
        /// <c>&lt;div class="header gray"&gt;</c>
        /// ,
        /// returns a set of two elements
        /// <c>"header", "gray"</c>
        /// . Note that modifications to this set are not pushed to
        /// the backing
        /// <c>class</c>
        /// attribute; use the
        /// <see cref="ClassNames(System.Collections.Generic.ICollection{E})"/>
        /// method to persist them.
        /// </remarks>
        /// <returns>set of classnames, empty if no class attribute</returns>
        public virtual ICollection<String> ClassNames() {
            String[] names = iText.IO.Util.StringUtil.Split(classSplit, ClassName());
            ICollection<String> classNames = new LinkedHashSet<String>(JavaUtil.ArraysAsList(names));
            classNames.Remove("");
            // if classNames() was empty, would include an empty class
            return classNames;
        }

        /// <summary>
        /// Set the element's
        /// <c>class</c>
        /// attribute to the supplied class names.
        /// </summary>
        /// <param name="classNames">set of classes</param>
        /// <returns>this element, for chaining</returns>
        public virtual iText.StyledXmlParser.Jsoup.Nodes.Element ClassNames(ICollection<String> classNames) {
            Validate.NotNull(classNames);
            attributes.Put("class", iText.StyledXmlParser.Jsoup.Helper.StringUtil.Join(classNames, " "));
            return this;
        }

        /// <summary>Tests if this element has a class.</summary>
        /// <remarks>Tests if this element has a class. Case insensitive.</remarks>
        /// <param name="className">name of class to check for</param>
        /// <returns>true if it does, false if not</returns>
        public virtual bool HasClass(String className) {
            /*
            Used by common .class selector, so perf tweaked to reduce object creation vs hitting classnames().
            
            Wiki: 71, 13 (5.4x)
            CNN: 227, 91 (2.5x)
            Alterslash: 59, 4 (14.8x)
            Jsoup: 14, 1 (14x)
            */
            String classAttr = attributes.Get("class");
            if (classAttr.Equals("") || classAttr.Length < className.Length) {
                return false;
            }
            String[] classes = iText.IO.Util.StringUtil.Split(classSplit, classAttr);
            foreach (String name in classes) {
                if (className.EqualsIgnoreCase(name)) {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Add a class name to this element's
        /// <c>class</c>
        /// attribute.
        /// </summary>
        /// <param name="className">class name to add</param>
        /// <returns>this element</returns>
        public virtual iText.StyledXmlParser.Jsoup.Nodes.Element AddClass(String className) {
            Validate.NotNull(className);
            ICollection<String> classes = ClassNames();
            classes.Add(className);
            ClassNames(classes);
            return this;
        }

        /// <summary>
        /// Remove a class name from this element's
        /// <c>class</c>
        /// attribute.
        /// </summary>
        /// <param name="className">class name to remove</param>
        /// <returns>this element</returns>
        public virtual iText.StyledXmlParser.Jsoup.Nodes.Element RemoveClass(String className) {
            Validate.NotNull(className);
            ICollection<String> classes = ClassNames();
            classes.Remove(className);
            ClassNames(classes);
            return this;
        }

        /// <summary>
        /// Toggle a class name on this element's
        /// <c>class</c>
        /// attribute: if present, remove it; otherwise add it.
        /// </summary>
        /// <param name="className">class name to toggle</param>
        /// <returns>this element</returns>
        public virtual iText.StyledXmlParser.Jsoup.Nodes.Element ToggleClass(String className) {
            Validate.NotNull(className);
            ICollection<String> classes = ClassNames();
            if (classes.Contains(className)) {
                classes.Remove(className);
            }
            else {
                classes.Add(className);
            }
            ClassNames(classes);
            return this;
        }

        /// <summary>Get the value of a form element (input, textarea, etc).</summary>
        /// <returns>the value of the form element, or empty string if not set.</returns>
        public virtual String Val() {
            if (TagName().Equals("textarea")) {
                return Text();
            }
            else {
                return Attr("value");
            }
        }

        /// <summary>Set the value of a form element (input, textarea, etc).</summary>
        /// <param name="value">value to set</param>
        /// <returns>this element (for chaining)</returns>
        public virtual iText.StyledXmlParser.Jsoup.Nodes.Element Val(String value) {
            if (TagName().Equals("textarea")) {
                Text(value);
            }
            else {
                Attr("value", value);
            }
            return this;
        }

        internal override void OuterHtmlHead(StringBuilder accum, int depth, OutputSettings @out) {
            if (@out.PrettyPrint() && (tag.FormatAsBlock() || (Parent() != null && ((iText.StyledXmlParser.Jsoup.Nodes.Element
                )Parent()).Tag().FormatAsBlock()) || @out.Outline())) {
                if (accum is StringBuilder) {
                    if (((StringBuilder)accum).Length > 0) {
                        Indent(accum, depth, @out);
                    }
                }
                else {
                    Indent(accum, depth, @out);
                }
            }
            accum.Append("<").Append(TagName());
            attributes.Html(accum, @out);
            // selfclosing includes unknown tags, isEmpty defines tags that are always empty
            if (childNodes.IsEmpty() && tag.IsSelfClosing()) {
                if (@out.Syntax() == iText.StyledXmlParser.Jsoup.Nodes.Syntax.html && tag.IsEmpty()) {
                    accum.Append('>');
                }
                else {
                    accum.Append(" />");
                }
            }
            else {
                // <img> in html, <img /> in xml
                accum.Append(">");
            }
        }

        internal override void OuterHtmlTail(StringBuilder accum, int depth, OutputSettings @out) {
            if (!(childNodes.IsEmpty() && tag.IsSelfClosing())) {
                if (@out.PrettyPrint() && (!childNodes.IsEmpty() && (tag.FormatAsBlock() || (@out.Outline() && (childNodes
                    .Count > 1 || (childNodes.Count == 1 && !(childNodes[0] is TextNode))))))) {
                    Indent(accum, depth, @out);
                }
                accum.Append("</").Append(TagName()).Append(">");
            }
        }

        /// <summary>Retrieves the element's inner HTML.</summary>
        /// <remarks>
        /// Retrieves the element's inner HTML. E.g. on a
        /// <c>&lt;div&gt;</c>
        /// with one empty
        /// <c>&lt;p&gt;</c>
        /// , would return
        /// <c>&lt;p&gt;&lt;/p&gt;</c>
        /// . (Whereas
        /// <see cref="Node.OuterHtml()"/>
        /// would return
        /// <c>&lt;div&gt;&lt;p&gt;&lt;/p&gt;&lt;/div&gt;</c>
        /// .)
        /// </remarks>
        /// <returns>String of HTML.</returns>
        /// <seealso cref="Node.OuterHtml()"/>
        public virtual String Html() {
            StringBuilder accum = new StringBuilder();
            Html(accum);
            return GetOutputSettings().PrettyPrint() ? accum.ToString().Trim() : accum.ToString();
        }

        /// <summary><inheritDoc/></summary>
        public override StringBuilder Html(StringBuilder appendable) {
            foreach (iText.StyledXmlParser.Jsoup.Nodes.Node node in childNodes) {
                node.OuterHtml(appendable);
            }
            return appendable;
        }

        /// <summary>Set this element's inner HTML.</summary>
        /// <remarks>Set this element's inner HTML. Clears the existing HTML first.</remarks>
        /// <param name="html">HTML to parse and set into this element</param>
        /// <returns>this element</returns>
        /// <seealso cref="Append(System.String)"/>
        public virtual iText.StyledXmlParser.Jsoup.Nodes.Element Html(String html) {
            Empty();
            Append(html);
            return this;
        }

        public override String ToString() {
            return OuterHtml();
        }

        public override Object Clone() {
            return base.Clone();
        }
    }
}
