/*
This file is part of the iText (R) project.
Copyright (c) 1998-2021 iText Group NV
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
using iText.IO.Util;
using iText.StyledXmlParser.Jsoup;
using iText.StyledXmlParser.Jsoup.Helper;
using iText.StyledXmlParser.Jsoup.Select;

namespace iText.StyledXmlParser.Jsoup.Nodes {
    /// <summary>The base, abstract Node model.</summary>
    /// <remarks>The base, abstract Node model. Elements, Documents, Comments etc are all Node instances.</remarks>
    /// <author>Jonathan Hedley, jonathan@hedley.net</author>
    public abstract class Node
#if !NETSTANDARD2_0 && !NET5_0
 : ICloneable
#endif
    {
        private static readonly IList<iText.StyledXmlParser.Jsoup.Nodes.Node> EMPTY_NODES = JavaCollectionsUtil.EmptyList
            <iText.StyledXmlParser.Jsoup.Nodes.Node>();

        internal iText.StyledXmlParser.Jsoup.Nodes.Node parentNode;

        internal IList<iText.StyledXmlParser.Jsoup.Nodes.Node> childNodes;

        internal iText.StyledXmlParser.Jsoup.Nodes.Attributes attributes;

        internal String baseUri;

        internal int siblingIndex;

        /// <summary>Create a new Node.</summary>
        /// <param name="baseUri">base URI</param>
        /// <param name="attributes">attributes (not null, but may be empty)</param>
        protected internal Node(String baseUri, iText.StyledXmlParser.Jsoup.Nodes.Attributes attributes) {
            Validate.NotNull(baseUri);
            Validate.NotNull(attributes);
            childNodes = EMPTY_NODES;
            this.baseUri = baseUri.Trim();
            this.attributes = attributes;
        }

        protected internal Node(String baseUri)
            : this(baseUri, new iText.StyledXmlParser.Jsoup.Nodes.Attributes()) {
        }

        /// <summary>Default constructor.</summary>
        /// <remarks>Default constructor. Doesn't setup base uri, children, or attributes; use with caution.</remarks>
        protected internal Node() {
            childNodes = EMPTY_NODES;
            attributes = null;
        }

        /// <summary>Get the node name of this node.</summary>
        /// <remarks>Get the node name of this node. Use for debugging purposes and not logic switching (for that, use instanceof).
        ///     </remarks>
        /// <returns>node name</returns>
        public abstract String NodeName();

        /// <summary>Get an attribute's value by its key.</summary>
        /// <remarks>
        /// Get an attribute's value by its key.
        /// <para />
        /// To get an absolute URL from an attribute that may be a relative URL, prefix the key with <c><b>abs</b></c>,
        /// which is a shortcut to the
        /// <see cref="AbsUrl(System.String)"/>
        /// method.
        /// <para />
        /// E.g.:
        /// <blockquote><c>String url = a.attr("abs:href");</c></blockquote>
        /// </remarks>
        /// <param name="attributeKey">The attribute key.</param>
        /// <returns>The attribute, or empty string if not present (to avoid nulls).</returns>
        /// <seealso cref="Attributes()"/>
        /// <seealso cref="HasAttr(System.String)"/>
        /// <seealso cref="AbsUrl(System.String)"/>
        public virtual String Attr(String attributeKey) {
            Validate.NotNull(attributeKey);
            if (attributes.HasKey(attributeKey)) {
                return attributes.Get(attributeKey);
            }
            else {
                if (attributeKey.ToLowerInvariant().StartsWith("abs:")) {
                    return AbsUrl(attributeKey.Substring("abs:".Length));
                }
                else {
                    return "";
                }
            }
        }

        /// <summary>Get all of the element's attributes.</summary>
        /// <returns>attributes (which implements iterable, in same order as presented in original HTML).</returns>
        public virtual iText.StyledXmlParser.Jsoup.Nodes.Attributes Attributes() {
            return attributes;
        }

        /// <summary>Set an attribute (key=value).</summary>
        /// <remarks>Set an attribute (key=value). If the attribute already exists, it is replaced.</remarks>
        /// <param name="attributeKey">The attribute key.</param>
        /// <param name="attributeValue">The attribute value.</param>
        /// <returns>this (for chaining)</returns>
        public virtual iText.StyledXmlParser.Jsoup.Nodes.Node Attr(String attributeKey, String attributeValue) {
            attributes.Put(attributeKey, attributeValue);
            return this;
        }

        /// <summary>Test if this element has an attribute.</summary>
        /// <param name="attributeKey">The attribute key to check.</param>
        /// <returns>true if the attribute exists, false if not.</returns>
        public virtual bool HasAttr(String attributeKey) {
            Validate.NotNull(attributeKey);
            if (attributeKey.StartsWith("abs:")) {
                String key = attributeKey.Substring("abs:".Length);
                if (attributes.HasKey(key) && !AbsUrl(key).Equals("")) {
                    return true;
                }
            }
            return attributes.HasKey(attributeKey);
        }

        /// <summary>Remove an attribute from this element.</summary>
        /// <param name="attributeKey">The attribute to remove.</param>
        /// <returns>this (for chaining)</returns>
        public virtual iText.StyledXmlParser.Jsoup.Nodes.Node RemoveAttr(String attributeKey) {
            Validate.NotNull(attributeKey);
            attributes.Remove(attributeKey);
            return this;
        }

        /// <summary>Get the base URI of this node.</summary>
        /// <returns>base URI</returns>
        public virtual String BaseUri() {
            return baseUri;
        }

        /// <summary>Update the base URI of this node and all of its descendants.</summary>
        /// <param name="baseUri">base URI to set</param>
        public virtual void SetBaseUri(String baseUri) {
            Validate.NotNull(baseUri);
            Traverse(new _NodeVisitor_188(baseUri));
        }

        private sealed class _NodeVisitor_188 : NodeVisitor {
            public _NodeVisitor_188(String baseUri) {
                this.baseUri = baseUri;
            }

            public void Head(iText.StyledXmlParser.Jsoup.Nodes.Node node, int depth) {
                node.baseUri = baseUri;
            }

            public void Tail(iText.StyledXmlParser.Jsoup.Nodes.Node node, int depth) {
            }

            private readonly String baseUri;
        }

        /// <summary>
        /// Get an absolute URL from a URL attribute that may be relative (i.e. an <c>&lt;a href&gt;</c> or
        /// <c>&lt;img src&gt;</c>).
        /// </summary>
        /// <remarks>
        /// Get an absolute URL from a URL attribute that may be relative (i.e. an <c>&lt;a href&gt;</c> or
        /// <c>&lt;img src&gt;</c>).
        /// <para />
        /// E.g.: <c>String absUrl = linkEl.absUrl("href");</c>
        /// <para />
        /// If the attribute value is already absolute (i.e. it starts with a protocol, like
        /// <c>http://</c> or <c>https://</c> etc), and it successfully parses as a URL, the attribute is
        /// returned directly. Otherwise, it is treated as a URL relative to the element's
        /// <see cref="baseUri"/>
        /// , and made
        /// absolute using that.
        /// <para />
        /// As an alternate, you can use the
        /// <see cref="Attr(System.String)"/>
        /// method with the <c>abs:</c> prefix, e.g.:
        /// <c>String absUrl = linkEl.attr("abs:href");</c>
        /// </remarks>
        /// <param name="attributeKey">The attribute key</param>
        /// <returns>
        /// An absolute URL if one could be made, or an empty string (not null) if the attribute was missing or
        /// could not be made successfully into a URL.
        /// </returns>
        /// <seealso cref="Attr(System.String)"/>
        /// <seealso cref="System.Uri.URL(System.Uri, System.String)"/>
        public virtual String AbsUrl(String attributeKey) {
            Validate.NotEmpty(attributeKey);
            if (!HasAttr(attributeKey)) {
                return "";
            }
            else {
                // nothing to make absolute with
                return iText.StyledXmlParser.Jsoup.Helper.StringUtil.Resolve(baseUri, Attr(attributeKey));
            }
        }

        /// <summary>Get a child node by its 0-based index.</summary>
        /// <param name="index">index of child node</param>
        /// <returns>
        /// the child node at this index. Throws a
        /// <c>IndexOutOfBoundsException</c>
        /// if the index is out of bounds.
        /// </returns>
        public virtual iText.StyledXmlParser.Jsoup.Nodes.Node ChildNode(int index) {
            return childNodes[index];
        }

        /// <summary>Get this node's children.</summary>
        /// <remarks>
        /// Get this node's children. Presented as an unmodifiable list: new children can not be added, but the child nodes
        /// themselves can be manipulated.
        /// </remarks>
        /// <returns>list of children. If no children, returns an empty list.</returns>
        public virtual IList<iText.StyledXmlParser.Jsoup.Nodes.Node> ChildNodes() {
            return JavaCollectionsUtil.UnmodifiableList(childNodes);
        }

        /// <summary>Returns a deep copy of this node's children.</summary>
        /// <remarks>
        /// Returns a deep copy of this node's children. Changes made to these nodes will not be reflected in the original
        /// nodes
        /// </remarks>
        /// <returns>a deep copy of this node's children</returns>
        public virtual IList<iText.StyledXmlParser.Jsoup.Nodes.Node> ChildNodesCopy() {
            IList<iText.StyledXmlParser.Jsoup.Nodes.Node> children = new List<iText.StyledXmlParser.Jsoup.Nodes.Node>(
                childNodes.Count);
            foreach (iText.StyledXmlParser.Jsoup.Nodes.Node node in childNodes) {
                children.Add((iText.StyledXmlParser.Jsoup.Nodes.Node)node.Clone());
            }
            return children;
        }

        /// <summary>Get the number of child nodes that this node holds.</summary>
        /// <returns>the number of child nodes that this node holds.</returns>
        public int ChildNodeSize() {
            return childNodes.Count;
        }

        protected internal virtual iText.StyledXmlParser.Jsoup.Nodes.Node[] ChildNodesAsArray() {
            return childNodes.ToArray(new iText.StyledXmlParser.Jsoup.Nodes.Node[ChildNodeSize()]);
        }

        /// <summary>Gets this node's parent node.</summary>
        /// <returns>parent node; or null if no parent.</returns>
        public virtual iText.StyledXmlParser.Jsoup.Nodes.Node Parent() {
            return parentNode;
        }

        /// <summary>Gets this node's parent node.</summary>
        /// <remarks>Gets this node's parent node. Node overridable by extending classes, so useful if you really just need the Node type.
        ///     </remarks>
        /// <returns>parent node; or null if no parent.</returns>
        public iText.StyledXmlParser.Jsoup.Nodes.Node ParentNode() {
            return parentNode;
        }

        /// <summary>Gets the Document associated with this Node.</summary>
        /// <returns>the Document associated with this Node, or null if there is no such Document.</returns>
        public virtual Document OwnerDocument() {
            if (this is Document) {
                return (Document)this;
            }
            else {
                if (parentNode == null) {
                    return null;
                }
                else {
                    return parentNode.OwnerDocument();
                }
            }
        }

        /// <summary>Remove (delete) this node from the DOM tree.</summary>
        /// <remarks>Remove (delete) this node from the DOM tree. If this node has children, they are also removed.</remarks>
        public virtual void Remove() {
            Validate.NotNull(parentNode);
            parentNode.RemoveChild(this);
        }

        /// <summary>Insert the specified HTML into the DOM before this node (i.e. as a preceding sibling).</summary>
        /// <param name="html">HTML to add before this node</param>
        /// <returns>this node, for chaining</returns>
        /// <seealso cref="After(System.String)"/>
        public virtual iText.StyledXmlParser.Jsoup.Nodes.Node Before(String html) {
            AddSiblingHtml(siblingIndex, html);
            return this;
        }

        /// <summary>Insert the specified node into the DOM before this node (i.e. as a preceding sibling).</summary>
        /// <param name="node">to add before this node</param>
        /// <returns>this node, for chaining</returns>
        /// <seealso cref="After(Node)"/>
        public virtual iText.StyledXmlParser.Jsoup.Nodes.Node Before(iText.StyledXmlParser.Jsoup.Nodes.Node node) {
            Validate.NotNull(node);
            Validate.NotNull(parentNode);
            parentNode.AddChildren(siblingIndex, node);
            return this;
        }

        /// <summary>Insert the specified HTML into the DOM after this node (i.e. as a following sibling).</summary>
        /// <param name="html">HTML to add after this node</param>
        /// <returns>this node, for chaining</returns>
        /// <seealso cref="Before(System.String)"/>
        public virtual iText.StyledXmlParser.Jsoup.Nodes.Node After(String html) {
            AddSiblingHtml(siblingIndex + 1, html);
            return this;
        }

        /// <summary>Insert the specified node into the DOM after this node (i.e. as a following sibling).</summary>
        /// <param name="node">to add after this node</param>
        /// <returns>this node, for chaining</returns>
        /// <seealso cref="Before(Node)"/>
        public virtual iText.StyledXmlParser.Jsoup.Nodes.Node After(iText.StyledXmlParser.Jsoup.Nodes.Node node) {
            Validate.NotNull(node);
            Validate.NotNull(parentNode);
            parentNode.AddChildren(siblingIndex + 1, node);
            return this;
        }

        private void AddSiblingHtml(int index, String html) {
            Validate.NotNull(html);
            Validate.NotNull(parentNode);
            iText.StyledXmlParser.Jsoup.Nodes.Element context = Parent() is iText.StyledXmlParser.Jsoup.Nodes.Element ? 
                (iText.StyledXmlParser.Jsoup.Nodes.Element)Parent() : null;
            IList<iText.StyledXmlParser.Jsoup.Nodes.Node> nodes = iText.StyledXmlParser.Jsoup.Parser.Parser.ParseFragment
                (html, context, BaseUri());
            parentNode.AddChildren(index, nodes.ToArray(new iText.StyledXmlParser.Jsoup.Nodes.Node[nodes.Count]));
        }

        /// <summary>Wrap the supplied HTML around this node.</summary>
        /// <param name="html">
        /// HTML to wrap around this element, e.g.
        /// <c>&lt;div class="head"&gt;&lt;/div&gt;</c>
        /// . Can be arbitrarily deep.
        /// </param>
        /// <returns>this node, for chaining.</returns>
        public virtual iText.StyledXmlParser.Jsoup.Nodes.Node Wrap(String html) {
            Validate.NotEmpty(html);
            iText.StyledXmlParser.Jsoup.Nodes.Element context = Parent() is iText.StyledXmlParser.Jsoup.Nodes.Element ? 
                (iText.StyledXmlParser.Jsoup.Nodes.Element)Parent() : null;
            IList<iText.StyledXmlParser.Jsoup.Nodes.Node> wrapChildren = iText.StyledXmlParser.Jsoup.Parser.Parser.ParseFragment
                (html, context, BaseUri());
            iText.StyledXmlParser.Jsoup.Nodes.Node wrapNode = wrapChildren[0];
            if (wrapNode == null || !(wrapNode is iText.StyledXmlParser.Jsoup.Nodes.Element)) {
                // nothing to wrap with; noop
                return null;
            }
            iText.StyledXmlParser.Jsoup.Nodes.Element wrap = (iText.StyledXmlParser.Jsoup.Nodes.Element)wrapNode;
            iText.StyledXmlParser.Jsoup.Nodes.Element deepest = GetDeepChild(wrap);
            parentNode.ReplaceChild(this, wrap);
            deepest.AddChildren(this);
            // remainder (unbalanced wrap, like <div></div><p></p> -- The <p> is remainder
            if (wrapChildren.Count > 0) {
                for (int i = 0; i < wrapChildren.Count; i++) {
                    iText.StyledXmlParser.Jsoup.Nodes.Node remainder = wrapChildren[i];
                    remainder.parentNode.RemoveChild(remainder);
                    wrap.AppendChild(remainder);
                }
            }
            return this;
        }

        /// <summary>Removes this node from the DOM, and moves its children up into the node's parent.</summary>
        /// <remarks>
        /// Removes this node from the DOM, and moves its children up into the node's parent. This has the effect of dropping
        /// the node but keeping its children.
        /// <para />
        /// For example, with the input html:
        /// <para />
        /// <c>&lt;div&gt;One &lt;span&gt;Two &lt;b&gt;Three&lt;/b&gt;&lt;/span&gt;&lt;/div&gt;</c>
        /// <para />
        /// Calling
        /// <c>element.unwrap()</c>
        /// on the
        /// <c>span</c>
        /// element will result in the html:
        /// <para />
        /// <c>&lt;div&gt;One Two &lt;b&gt;Three&lt;/b&gt;&lt;/div&gt;</c>
        /// <para />
        /// and the
        /// <c>"Two "</c>
        /// 
        /// <see cref="TextNode"/>
        /// being returned.
        /// </remarks>
        /// <returns>the first child of this node, after the node has been unwrapped. Null if the node had no children.
        ///     </returns>
        /// <seealso cref="Remove()"/>
        /// <seealso cref="Wrap(System.String)"/>
        public virtual iText.StyledXmlParser.Jsoup.Nodes.Node Unwrap() {
            Validate.NotNull(parentNode);
            iText.StyledXmlParser.Jsoup.Nodes.Node firstChild = childNodes.Count > 0 ? childNodes[0] : null;
            parentNode.AddChildren(siblingIndex, this.ChildNodesAsArray());
            this.Remove();
            return firstChild;
        }

        private iText.StyledXmlParser.Jsoup.Nodes.Element GetDeepChild(iText.StyledXmlParser.Jsoup.Nodes.Element el
            ) {
            IList<iText.StyledXmlParser.Jsoup.Nodes.Element> children = el.Children();
            if (children.Count > 0) {
                return GetDeepChild(children[0]);
            }
            else {
                return el;
            }
        }

        /// <summary>Replace this node in the DOM with the supplied node.</summary>
        /// <param name="in">the node that will will replace the existing node.</param>
        public virtual void ReplaceWith(iText.StyledXmlParser.Jsoup.Nodes.Node @in) {
            Validate.NotNull(@in);
            Validate.NotNull(parentNode);
            parentNode.ReplaceChild(this, @in);
        }

        protected internal virtual void SetParentNode(iText.StyledXmlParser.Jsoup.Nodes.Node parentNode) {
            if (this.parentNode != null) {
                this.parentNode.RemoveChild(this);
            }
            this.parentNode = parentNode;
        }

        protected internal virtual void ReplaceChild(iText.StyledXmlParser.Jsoup.Nodes.Node @out, iText.StyledXmlParser.Jsoup.Nodes.Node
             @in) {
            Validate.IsTrue(@out.parentNode == this);
            Validate.NotNull(@in);
            if (@in.parentNode != null) {
                @in.parentNode.RemoveChild(@in);
            }
            int index = @out.siblingIndex;
            childNodes[index] = @in;
            @in.parentNode = this;
            @in.SetSiblingIndex(index);
            @out.parentNode = null;
        }

        protected internal virtual void RemoveChild(iText.StyledXmlParser.Jsoup.Nodes.Node @out) {
            Validate.IsTrue(@out.parentNode == this);
            int index = @out.siblingIndex;
            childNodes.JRemoveAt(index);
            ReindexChildren(index);
            @out.parentNode = null;
        }

        protected internal virtual void AddChildren(params iText.StyledXmlParser.Jsoup.Nodes.Node[] children) {
            //most used. short circuit addChildren(int), which hits reindex children and array copy
            foreach (iText.StyledXmlParser.Jsoup.Nodes.Node child in children) {
                ReparentChild(child);
                EnsureChildNodes();
                childNodes.Add(child);
                child.SetSiblingIndex(childNodes.Count - 1);
            }
        }

        protected internal virtual void AddChildren(int index, params iText.StyledXmlParser.Jsoup.Nodes.Node[] children
            ) {
            Validate.NoNullElements(children);
            EnsureChildNodes();
            for (int i = children.Length - 1; i >= 0; i--) {
                iText.StyledXmlParser.Jsoup.Nodes.Node @in = children[i];
                ReparentChild(@in);
                childNodes.Add(index, @in);
                ReindexChildren(index);
            }
        }

        protected internal virtual void EnsureChildNodes() {
            if (childNodes == EMPTY_NODES) {
                childNodes = new List<iText.StyledXmlParser.Jsoup.Nodes.Node>(4);
            }
        }

        protected internal virtual void ReparentChild(iText.StyledXmlParser.Jsoup.Nodes.Node child) {
            if (child.parentNode != null) {
                child.parentNode.RemoveChild(child);
            }
            child.SetParentNode(this);
        }

        private void ReindexChildren(int start) {
            for (int i = start; i < childNodes.Count; i++) {
                childNodes[i].SetSiblingIndex(i);
            }
        }

        /// <summary>Retrieves this node's sibling nodes.</summary>
        /// <remarks>
        /// Retrieves this node's sibling nodes. Similar to
        /// <see cref="ChildNodes()">node.parent.childNodes()</see>
        /// , but does not
        /// include this node (a node is not a sibling of itself).
        /// </remarks>
        /// <returns>node siblings. If the node has no parent, returns an empty list.</returns>
        public virtual IList<iText.StyledXmlParser.Jsoup.Nodes.Node> SiblingNodes() {
            if (parentNode == null) {
                return JavaCollectionsUtil.EmptyList<iText.StyledXmlParser.Jsoup.Nodes.Node>();
            }
            IList<iText.StyledXmlParser.Jsoup.Nodes.Node> nodes = parentNode.childNodes;
            IList<iText.StyledXmlParser.Jsoup.Nodes.Node> siblings = new List<iText.StyledXmlParser.Jsoup.Nodes.Node>(
                nodes.Count - 1);
            foreach (iText.StyledXmlParser.Jsoup.Nodes.Node node in nodes) {
                if (node != this) {
                    siblings.Add(node);
                }
            }
            return siblings;
        }

        /// <summary>Get this node's next sibling.</summary>
        /// <returns>next sibling, or null if this is the last sibling</returns>
        public virtual iText.StyledXmlParser.Jsoup.Nodes.Node NextSibling() {
            if (parentNode == null) {
                return null;
            }
            // root
            IList<iText.StyledXmlParser.Jsoup.Nodes.Node> siblings = parentNode.childNodes;
            int index = siblingIndex + 1;
            if (siblings.Count > index) {
                return siblings[index];
            }
            else {
                return null;
            }
        }

        /// <summary>Get this node's previous sibling.</summary>
        /// <returns>the previous sibling, or null if this is the first sibling</returns>
        public virtual iText.StyledXmlParser.Jsoup.Nodes.Node PreviousSibling() {
            if (parentNode == null) {
                return null;
            }
            // root
            if (siblingIndex > 0) {
                return parentNode.childNodes[siblingIndex - 1];
            }
            else {
                return null;
            }
        }

        /// <summary>Get the list index of this node in its node sibling list.</summary>
        /// <remarks>
        /// Get the list index of this node in its node sibling list. I.e. if this is the first node
        /// sibling, returns 0.
        /// </remarks>
        /// <returns>position in node sibling list</returns>
        /// <seealso cref="Element.ElementSiblingIndex()"/>
        public virtual int SiblingIndex() {
            return siblingIndex;
        }

        protected internal virtual void SetSiblingIndex(int siblingIndex) {
            this.siblingIndex = siblingIndex;
        }

        /// <summary>Perform a depth-first traversal through this node and its descendants.</summary>
        /// <param name="nodeVisitor">the visitor callbacks to perform on each node</param>
        /// <returns>this node, for chaining</returns>
        public virtual iText.StyledXmlParser.Jsoup.Nodes.Node Traverse(NodeVisitor nodeVisitor) {
            Validate.NotNull(nodeVisitor);
            NodeTraversor traversor = new NodeTraversor(nodeVisitor);
            traversor.Traverse(this);
            return this;
        }

        /// <summary>Get the outer HTML of this node.</summary>
        /// <returns>HTML</returns>
        public virtual String OuterHtml() {
            StringBuilder accum = new StringBuilder(128);
            OuterHtml(accum);
            return accum.ToString();
        }

        protected internal virtual void OuterHtml(StringBuilder accum) {
            new NodeTraversor(new Node.OuterHtmlVisitor(accum, GetOutputSettings())).Traverse(this);
        }

        // if this node has no document (or parent), retrieve the default output settings
        internal virtual OutputSettings GetOutputSettings() {
            return OwnerDocument() != null ? OwnerDocument().OutputSettings() : (new Document("")).OutputSettings();
        }

        /// <summary>Get the outer HTML of this node.</summary>
        /// <param name="accum">accumulator to place HTML into</param>
        internal abstract void OuterHtmlHead(StringBuilder accum, int depth, OutputSettings @out);

        internal abstract void OuterHtmlTail(StringBuilder accum, int depth, OutputSettings @out);

        /// <summary>
        /// Write this node and its children to the given
        /// <see cref="System.Text.StringBuilder"/>.
        /// </summary>
        /// <param name="appendable">
        /// the
        /// <see cref="System.Text.StringBuilder"/>
        /// to write to.
        /// </param>
        /// <returns>
        /// the supplied
        /// <see cref="System.Text.StringBuilder"/>
        /// , for chaining.
        /// </returns>
        public virtual StringBuilder Html(StringBuilder appendable) {
            OuterHtml(appendable);
            return appendable;
        }

        public override String ToString() {
            return OuterHtml();
        }

        protected internal virtual void Indent(StringBuilder accum, int depth, OutputSettings @out) {
            accum.Append("\n").Append(iText.StyledXmlParser.Jsoup.Helper.StringUtil.Padding(depth * @out.IndentAmount(
                )));
        }

        /// <summary>Check if this node is the same instance of another (object identity test).</summary>
        /// <param name="o">other object to compare to</param>
        /// <returns>true if the content of this node is the same as the other</returns>
        /// <seealso cref="HasSameValue(System.Object)">to compare nodes by their value</seealso>
        public override bool Equals(Object o) {
            // implemented just so that javadoc is clear this is an identity test
            return this == o;
        }

        /// <summary>Check if this node is has the same content as another node.</summary>
        /// <remarks>
        /// Check if this node is has the same content as another node. A node is considered the same if its name, attributes and content match the
        /// other node; particularly its position in the tree does not influence its similarity.
        /// </remarks>
        /// <param name="o">other object to compare to</param>
        /// <returns>true if the content of this node is the same as the other</returns>
        public virtual bool HasSameValue(Object o) {
            if (this == o) {
                return true;
            }
            if (o == null || GetType() != o.GetType()) {
                return false;
            }
            return this.OuterHtml().Equals(((iText.StyledXmlParser.Jsoup.Nodes.Node)o).OuterHtml());
        }

        /// <summary>Create a stand-alone, deep copy of this node, and all of its children.</summary>
        /// <remarks>
        /// Create a stand-alone, deep copy of this node, and all of its children. The cloned node will have no siblings or
        /// parent node. As a stand-alone object, any changes made to the clone or any of its children will not impact the
        /// original node.
        /// <para />
        /// The cloned node may be adopted into another Document or node structure using
        /// <see cref="Element.AppendChild(Node)"/>.
        /// </remarks>
        /// <returns>stand-alone cloned node</returns>
        public virtual Object Clone() {
            iText.StyledXmlParser.Jsoup.Nodes.Node thisClone = DoClone(null);
            // splits for orphan
            // Queue up nodes that need their children cloned (BFS).
            LinkedList<iText.StyledXmlParser.Jsoup.Nodes.Node> nodesToProcess = new LinkedList<iText.StyledXmlParser.Jsoup.Nodes.Node
                >();
            nodesToProcess.Add(thisClone);
            while (!nodesToProcess.IsEmpty()) {
                iText.StyledXmlParser.Jsoup.Nodes.Node currParent = nodesToProcess.JRemove();
                for (int i = 0; i < currParent.childNodes.Count; i++) {
                    iText.StyledXmlParser.Jsoup.Nodes.Node childClone = currParent.childNodes[i].DoClone(currParent);
                    currParent.childNodes[i] = childClone;
                    nodesToProcess.Add(childClone);
                }
            }
            return thisClone;
        }

        /*
        * Return a clone of the node using the given parent (which can be null).
        * Not a deep copy of children.
        */
        protected internal virtual iText.StyledXmlParser.Jsoup.Nodes.Node DoClone(iText.StyledXmlParser.Jsoup.Nodes.Node
             parent) {
            iText.StyledXmlParser.Jsoup.Nodes.Node clone;
            clone = (iText.StyledXmlParser.Jsoup.Nodes.Node)MemberwiseClone();
            clone.parentNode = parent;
            // can be null, to create an orphan split
            clone.siblingIndex = parent == null ? 0 : siblingIndex;
            clone.attributes = attributes != null ? (iText.StyledXmlParser.Jsoup.Nodes.Attributes)attributes.Clone() : 
                null;
            clone.baseUri = baseUri;
            clone.childNodes = new List<iText.StyledXmlParser.Jsoup.Nodes.Node>(childNodes.Count);
            foreach (iText.StyledXmlParser.Jsoup.Nodes.Node child in childNodes) {
                clone.childNodes.Add(child);
            }
            return clone;
        }

        private class OuterHtmlVisitor : NodeVisitor {
            private StringBuilder accum;

            private OutputSettings @out;

            internal OuterHtmlVisitor(StringBuilder accum, OutputSettings @out) {
                this.accum = accum;
                this.@out = @out;
            }

            public virtual void Head(iText.StyledXmlParser.Jsoup.Nodes.Node node, int depth) {
                try {
                    node.OuterHtmlHead(accum, depth, @out);
                }
                catch (System.IO.IOException exception) {
                    throw new SerializationException(exception);
                }
            }

            public virtual void Tail(iText.StyledXmlParser.Jsoup.Nodes.Node node, int depth) {
                if (!node.NodeName().Equals("#text")) {
                    // saves a void hit.
                    try {
                        node.OuterHtmlTail(accum, depth, @out);
                    }
                    catch (System.IO.IOException exception) {
                        throw new SerializationException(exception);
                    }
                }
            }
        }
    }
}
