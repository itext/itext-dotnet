/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: iText Software.

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
using System.Text;
using iText.Commons.Utils;
using iText.StyledXmlParser.Jsoup;
using iText.StyledXmlParser.Jsoup.Helper;
using iText.StyledXmlParser.Jsoup.Select;

namespace iText.StyledXmlParser.Jsoup.Nodes {
    /// <summary>The base, abstract Node model.</summary>
    /// <remarks>The base, abstract Node model. Elements, Documents, Comments etc are all Node instances.</remarks>
    /// <author>Jonathan Hedley, jonathan@hedley.net</author>
    public abstract class Node
#if !NETSTANDARD2_0
 : ICloneable
#endif
 {
        internal static readonly IList<iText.StyledXmlParser.Jsoup.Nodes.Node> EmptyNodes = JavaCollectionsUtil.EmptyList
            <iText.StyledXmlParser.Jsoup.Nodes.Node>();

        internal const String EmptyString = "";

        internal iText.StyledXmlParser.Jsoup.Nodes.Node parentNode;

        // Nodes don't always have parents
        internal int siblingIndex;

        /// <summary>Default constructor.</summary>
        /// <remarks>Default constructor. Doesn't setup base uri, children, or attributes; use with caution.</remarks>
        protected internal Node() {
        }

        /// <summary>Get the node name of this node.</summary>
        /// <remarks>Get the node name of this node. Use for debugging purposes and not logic switching (for that, use instanceof).
        ///     </remarks>
        /// <returns>node name</returns>
        public abstract String NodeName();

        /// <summary>Check if this Node has an actual Attributes object.</summary>
        protected internal abstract bool HasAttributes();

        /// <summary>Checks if this node has a parent.</summary>
        /// <remarks>
        /// Checks if this node has a parent. Nodes won't have parents if (e.g.) they are newly created and not added as a child
        /// to an existing node, or if they are a
        /// <see cref="ShallowClone()"/>
        /// . In such cases,
        /// <see cref="Parent()"/>
        /// will return
        /// <see langword="null"/>.
        /// </remarks>
        /// <returns>if this node has a parent.</returns>
        public virtual bool HasParent() {
            return parentNode != null;
        }

        /// <summary>Get an attribute's value by its key.</summary>
        /// <remarks>
        /// Get an attribute's value by its key. <b>Case insensitive</b>
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
            if (!HasAttributes()) {
                return EmptyString;
            }
            String val = Attributes().GetIgnoreCase(attributeKey);
            if (val.Length > 0) {
                return val;
            }
            else {
                if (attributeKey.StartsWith("abs:")) {
                    return AbsUrl(attributeKey.Substring("abs:".Length));
                }
                else {
                    return "";
                }
            }
        }

        /// <summary>Get all of the element's attributes.</summary>
        /// <returns>attributes (which implements iterable, in same order as presented in original HTML).</returns>
        public abstract iText.StyledXmlParser.Jsoup.Nodes.Attributes Attributes();

        /// <summary>Set an attribute (key=value).</summary>
        /// <remarks>
        /// Set an attribute (key=value). If the attribute already exists, it is replaced. The attribute key comparison is
        /// <b>case insensitive</b>. The key will be set with case sensitivity as set in the parser settings.
        /// </remarks>
        /// <param name="attributeKey">The attribute key.</param>
        /// <param name="attributeValue">The attribute value.</param>
        /// <returns>this (for chaining)</returns>
        public virtual iText.StyledXmlParser.Jsoup.Nodes.Node Attr(String attributeKey, String attributeValue) {
            attributeKey = NodeUtils.Parser(this).Settings().NormalizeAttribute(attributeKey);
            Attributes().PutIgnoreCase(attributeKey, attributeValue);
            return this;
        }

        /// <summary>Test if this element has an attribute.</summary>
        /// <remarks>Test if this element has an attribute. <b>Case insensitive</b></remarks>
        /// <param name="attributeKey">The attribute key to check.</param>
        /// <returns>true if the attribute exists, false if not.</returns>
        public virtual bool HasAttr(String attributeKey) {
            Validate.NotNull(attributeKey);
            if (!HasAttributes()) {
                return false;
            }
            if (attributeKey.StartsWith("abs:")) {
                String key = attributeKey.Substring("abs:".Length);
                if (Attributes().HasKeyIgnoreCase(key) && !String.IsNullOrEmpty(AbsUrl(key))) {
                    return true;
                }
            }
            return Attributes().HasKeyIgnoreCase(attributeKey);
        }

        /// <summary>Remove an attribute from this node.</summary>
        /// <param name="attributeKey">The attribute to remove.</param>
        /// <returns>this (for chaining)</returns>
        public virtual iText.StyledXmlParser.Jsoup.Nodes.Node RemoveAttr(String attributeKey) {
            Validate.NotNull(attributeKey);
            if (HasAttributes()) {
                Attributes().RemoveIgnoreCase(attributeKey);
            }
            return this;
        }

        /// <summary>Clear (remove) all of the attributes in this node.</summary>
        /// <returns>this, for chaining</returns>
        public virtual iText.StyledXmlParser.Jsoup.Nodes.Node ClearAttributes() {
            if (HasAttributes()) {
                iText.StyledXmlParser.Jsoup.Nodes.Attributes attributes = Attributes();
                foreach (iText.StyledXmlParser.Jsoup.Nodes.Attribute attribute in attributes) {
                    attributes.Remove(attribute.Key);
                }
            }
            return this;
        }

        /// <summary>Get the base URI that applies to this node.</summary>
        /// <remarks>
        /// Get the base URI that applies to this node. Will return an empty string if not defined. Used to make relative links
        /// absolute.
        /// </remarks>
        /// <returns>base URI</returns>
        /// <seealso cref="AbsUrl(System.String)"/>
        public abstract String BaseUri();

        /// <summary>Set the baseUri for just this node (not its descendants), if this Node tracks base URIs.</summary>
        /// <param name="baseUri">new URI</param>
        protected internal abstract void DoSetBaseUri(String baseUri);

        /// <summary>Update the base URI of this node and all of its descendants.</summary>
        /// <param name="baseUri">base URI to set</param>
        public virtual void SetBaseUri(String baseUri) {
            Validate.NotNull(baseUri);
            DoSetBaseUri(baseUri);
        }

        /// <summary>
        /// Get an absolute URL from a URL attribute that may be relative (such as an <c>&lt;a href&gt;</c> or
        /// <c>&lt;img src&gt;</c>).
        /// </summary>
        /// <remarks>
        /// Get an absolute URL from a URL attribute that may be relative (such as an <c>&lt;a href&gt;</c> or
        /// <c>&lt;img src&gt;</c>).
        /// <para />
        /// E.g.: <c>String absUrl = linkEl.absUrl("href");</c>
        /// <para />
        /// If the attribute value is already absolute (i.e. it starts with a protocol, like
        /// <c>http://</c> or <c>https://</c> etc), and it successfully parses as a URL, the attribute is
        /// returned directly. Otherwise, it is treated as a URL relative to the element's
        /// <see cref="BaseUri()"/>
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
            if (!(HasAttributes() && Attributes().HasKeyIgnoreCase(attributeKey))) {
                // not using hasAttr, so that we don't recurse down hasAttr->absUrl
                return "";
            }
            return iText.StyledXmlParser.Jsoup.Internal.StringUtil.Resolve(BaseUri(), Attributes().GetIgnoreCase(attributeKey
                ));
        }

        protected internal abstract IList<iText.StyledXmlParser.Jsoup.Nodes.Node> EnsureChildNodes();

        /// <summary>Get a child node by its 0-based index.</summary>
        /// <param name="index">index of child node</param>
        /// <returns>
        /// the child node at this index. Throws a
        /// <c>IndexOutOfBoundsException</c>
        /// if the index is out of bounds.
        /// </returns>
        public virtual iText.StyledXmlParser.Jsoup.Nodes.Node ChildNode(int index) {
            return EnsureChildNodes()[index];
        }

        /// <summary>Get this node's children.</summary>
        /// <remarks>
        /// Get this node's children. Presented as an unmodifiable list: new children can not be added, but the child nodes
        /// themselves can be manipulated.
        /// </remarks>
        /// <returns>list of children. If no children, returns an empty list.</returns>
        public virtual IList<iText.StyledXmlParser.Jsoup.Nodes.Node> ChildNodes() {
            if (ChildNodeSize() == 0) {
                return EmptyNodes;
            }
            IList<iText.StyledXmlParser.Jsoup.Nodes.Node> children = EnsureChildNodes();
            IList<iText.StyledXmlParser.Jsoup.Nodes.Node> rewrap = new List<iText.StyledXmlParser.Jsoup.Nodes.Node>(children
                .Count);
            // wrapped so that looping and moving will not throw a CME as the source changes
            rewrap.AddAll(children);
            return JavaCollectionsUtil.UnmodifiableList(rewrap);
        }

        /// <summary>Returns a deep copy of this node's children.</summary>
        /// <remarks>
        /// Returns a deep copy of this node's children. Changes made to these nodes will not be reflected in the original
        /// nodes
        /// </remarks>
        /// <returns>a deep copy of this node's children</returns>
        public virtual IList<iText.StyledXmlParser.Jsoup.Nodes.Node> ChildNodesCopy() {
            IList<iText.StyledXmlParser.Jsoup.Nodes.Node> nodes = EnsureChildNodes();
            List<iText.StyledXmlParser.Jsoup.Nodes.Node> children = new List<iText.StyledXmlParser.Jsoup.Nodes.Node>(nodes
                .Count);
            foreach (iText.StyledXmlParser.Jsoup.Nodes.Node node in nodes) {
                children.Add((iText.StyledXmlParser.Jsoup.Nodes.Node)node.Clone());
            }
            return children;
        }

        /// <summary>Get the number of child nodes that this node holds.</summary>
        /// <returns>the number of child nodes that this node holds.</returns>
        public abstract int ChildNodeSize();

        protected internal virtual iText.StyledXmlParser.Jsoup.Nodes.Node[] ChildNodesAsArray() {
            return EnsureChildNodes().ToArray(new iText.StyledXmlParser.Jsoup.Nodes.Node[0]);
        }

        /// <summary>Delete all this node's children.</summary>
        /// <returns>this node, for chaining</returns>
        public abstract iText.StyledXmlParser.Jsoup.Nodes.Node Empty();

        /// <summary>Gets this node's parent node.</summary>
        /// <returns>parent node; or null if no parent.</returns>
        /// <seealso cref="HasParent()"/>
        public virtual iText.StyledXmlParser.Jsoup.Nodes.Node Parent() {
            return parentNode;
        }

        /// <summary>Gets this node's parent node.</summary>
        /// <remarks>Gets this node's parent node. Not overridable by extending classes, so useful if you really just need the Node type.
        ///     </remarks>
        /// <returns>parent node; or null if no parent.</returns>
        public iText.StyledXmlParser.Jsoup.Nodes.Node ParentNode() {
            return parentNode;
        }

        /// <summary>Get this node's root node; that is, its topmost ancestor.</summary>
        /// <remarks>
        /// Get this node's root node; that is, its topmost ancestor. If this node is the top ancestor, returns
        /// <c>this</c>.
        /// </remarks>
        /// <returns>topmost ancestor.</returns>
        public virtual iText.StyledXmlParser.Jsoup.Nodes.Node Root() {
            iText.StyledXmlParser.Jsoup.Nodes.Node node = this;
            while (node.parentNode != null) {
                node = node.parentNode;
            }
            return node;
        }

        /// <summary>Gets the Document associated with this Node.</summary>
        /// <returns>the Document associated with this Node, or null if there is no such Document.</returns>
        public virtual Document OwnerDocument() {
            iText.StyledXmlParser.Jsoup.Nodes.Node root = Root();
            return (root is Document) ? (Document)root : null;
        }

        /// <summary>Remove (delete) this node from the DOM tree.</summary>
        /// <remarks>Remove (delete) this node from the DOM tree. If this node has children, they are also removed.</remarks>
        public virtual void Remove() {
            Validate.NotNull(parentNode);
            parentNode.RemoveChild(this);
        }

        /// <summary>Insert the specified HTML into the DOM before this node (as a preceding sibling).</summary>
        /// <param name="html">HTML to add before this node</param>
        /// <returns>this node, for chaining</returns>
        /// <seealso cref="After(System.String)"/>
        public virtual iText.StyledXmlParser.Jsoup.Nodes.Node Before(String html) {
            AddSiblingHtml(siblingIndex, html);
            return this;
        }

        /// <summary>Insert the specified node into the DOM before this node (as a preceding sibling).</summary>
        /// <param name="node">to add before this node</param>
        /// <returns>this node, for chaining</returns>
        /// <seealso cref="After(Node)"/>
        public virtual iText.StyledXmlParser.Jsoup.Nodes.Node Before(iText.StyledXmlParser.Jsoup.Nodes.Node node) {
            Validate.NotNull(node);
            Validate.NotNull(parentNode);
            parentNode.AddChildren(siblingIndex, node);
            return this;
        }

        /// <summary>Insert the specified HTML into the DOM after this node (as a following sibling).</summary>
        /// <param name="html">HTML to add after this node</param>
        /// <returns>this node, for chaining</returns>
        /// <seealso cref="Before(System.String)"/>
        public virtual iText.StyledXmlParser.Jsoup.Nodes.Node After(String html) {
            AddSiblingHtml(siblingIndex + 1, html);
            return this;
        }

        /// <summary>Insert the specified node into the DOM after this node (as a following sibling).</summary>
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
            IList<iText.StyledXmlParser.Jsoup.Nodes.Node> nodes = NodeUtils.Parser(this).ParseFragmentInput(html, context
                , BaseUri());
            parentNode.AddChildren(index, nodes.ToArray(new iText.StyledXmlParser.Jsoup.Nodes.Node[0]));
        }

        /// <summary>Wrap the supplied HTML around this node.</summary>
        /// <param name="html">
        /// HTML to wrap around this node, e.g.
        /// <c>&lt;div class="head"&gt;&lt;/div&gt;</c>
        /// . Can be arbitrarily deep. If
        /// the input HTML does not parse to a result starting with an Element, this will be a no-op.
        /// </param>
        /// <returns>this node, for chaining.</returns>
        public virtual iText.StyledXmlParser.Jsoup.Nodes.Node Wrap(String html) {
            Validate.NotEmpty(html);
            // Parse context - parent (because wrapping), this, or null
            iText.StyledXmlParser.Jsoup.Nodes.Element context = parentNode != null && parentNode is iText.StyledXmlParser.Jsoup.Nodes.Element
                 ? (iText.StyledXmlParser.Jsoup.Nodes.Element)parentNode : this is iText.StyledXmlParser.Jsoup.Nodes.Element
                 ? (iText.StyledXmlParser.Jsoup.Nodes.Element)this : null;
            IList<iText.StyledXmlParser.Jsoup.Nodes.Node> wrapChildren = NodeUtils.Parser(this).ParseFragmentInput(html
                , context, BaseUri());
            iText.StyledXmlParser.Jsoup.Nodes.Node wrapNode = wrapChildren[0];
            if (!(wrapNode is iText.StyledXmlParser.Jsoup.Nodes.Element)) {
                // nothing to wrap with; noop
                return this;
            }
            iText.StyledXmlParser.Jsoup.Nodes.Element wrap = (iText.StyledXmlParser.Jsoup.Nodes.Element)wrapNode;
            iText.StyledXmlParser.Jsoup.Nodes.Element deepest = GetDeepChild(wrap);
            if (parentNode != null) {
                parentNode.ReplaceChild(this, wrap);
            }
            deepest.AddChildren(this);
            // side effect of tricking wrapChildren to lose first
            // remainder (unbalanced wrap, like <div></div><p></p> -- The <p> is remainder
            if (wrapChildren.Count > 0) {
                //noinspection ForLoopReplaceableByForEach (beacause it allocates an Iterator which is wasteful here)
                for (int i = 0; i < wrapChildren.Count; i++) {
                    iText.StyledXmlParser.Jsoup.Nodes.Node remainder = wrapChildren[i];
                    // if no parent, this could be the wrap node, so skip
                    if (wrap == remainder) {
                        continue;
                    }
                    if (remainder.parentNode != null) {
                        remainder.parentNode.RemoveChild(remainder);
                    }
                    wrap.After(remainder);
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
        /// Calling
        /// <c>element.unwrap()</c>
        /// on the
        /// <c>span</c>
        /// element will result in the html:
        /// <para />
        /// <c>&lt;div&gt;One Two &lt;b&gt;Three&lt;/b&gt;&lt;/div&gt;</c>
        /// and the
        /// <c>"Two "</c>
        /// 
        /// <see cref="TextNode"/>
        /// being returned.
        /// </remarks>
        /// <returns>the first child of this node, after the node has been unwrapped. @{code Null} if the node had no children.
        ///     </returns>
        /// <seealso cref="Remove()"/>
        /// <seealso cref="Wrap(System.String)"/>
        public virtual iText.StyledXmlParser.Jsoup.Nodes.Node Unwrap() {
            Validate.NotNull(parentNode);
            IList<iText.StyledXmlParser.Jsoup.Nodes.Node> childNodes = EnsureChildNodes();
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

        internal virtual void NodelistChanged() {
        }

        // Element overrides this to clear its shadow children elements
        /// <summary>Replace this node in the DOM with the supplied node.</summary>
        /// <param name="in">the node that will will replace the existing node.</param>
        public virtual void ReplaceWith(iText.StyledXmlParser.Jsoup.Nodes.Node @in) {
            Validate.NotNull(@in);
            Validate.NotNull(parentNode);
            parentNode.ReplaceChild(this, @in);
        }

        protected internal virtual void SetParentNode(iText.StyledXmlParser.Jsoup.Nodes.Node parentNode) {
            Validate.NotNull(parentNode);
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
            EnsureChildNodes()[index] = @in;
            @in.parentNode = this;
            @in.SetSiblingIndex(index);
            @out.parentNode = null;
        }

        protected internal virtual void RemoveChild(iText.StyledXmlParser.Jsoup.Nodes.Node @out) {
            Validate.IsTrue(@out.parentNode == this);
            int index = @out.siblingIndex;
            EnsureChildNodes().JRemoveAt(index);
            ReindexChildren(index);
            @out.parentNode = null;
        }

        protected internal virtual void AddChildren(params iText.StyledXmlParser.Jsoup.Nodes.Node[] children) {
            //most used. short circuit addChildren(int), which hits reindex children and array copy
            IList<iText.StyledXmlParser.Jsoup.Nodes.Node> nodes = EnsureChildNodes();
            foreach (iText.StyledXmlParser.Jsoup.Nodes.Node child in children) {
                ReparentChild(child);
                nodes.Add(child);
                child.SetSiblingIndex(nodes.Count - 1);
            }
        }

        protected internal virtual void AddChildren(int index, params iText.StyledXmlParser.Jsoup.Nodes.Node[] children
            ) {
            Validate.NotNull(children);
            if (children.Length == 0) {
                return;
            }
            IList<iText.StyledXmlParser.Jsoup.Nodes.Node> nodes = EnsureChildNodes();
            // fast path - if used as a wrap (index=0, children = child[0].parent.children - do inplace
            iText.StyledXmlParser.Jsoup.Nodes.Node firstParent = children[0].Parent();
            if (firstParent != null && firstParent.ChildNodeSize() == children.Length) {
                bool sameList = true;
                IList<iText.StyledXmlParser.Jsoup.Nodes.Node> firstParentNodes = firstParent.EnsureChildNodes();
                // identity check contents to see if same
                int i = children.Length;
                while (i-- > 0) {
                    if (children[i] != firstParentNodes[i]) {
                        sameList = false;
                        break;
                    }
                }
                if (sameList) {
                    // moving, so OK to empty firstParent and short-circuit
                    firstParent.Empty();
                    nodes.AddAll(index, JavaUtil.ArraysAsList(children));
                    i = children.Length;
                    while (i-- > 0) {
                        children[i].parentNode = this;
                    }
                    ReindexChildren(index);
                    return;
                }
            }
            Validate.NoNullElements(children);
            foreach (iText.StyledXmlParser.Jsoup.Nodes.Node child in children) {
                ReparentChild(child);
            }
            nodes.AddAll(index, JavaUtil.ArraysAsList(children));
            ReindexChildren(index);
        }

        protected internal virtual void ReparentChild(iText.StyledXmlParser.Jsoup.Nodes.Node child) {
            child.SetParentNode(this);
        }

        private void ReindexChildren(int start) {
            IList<iText.StyledXmlParser.Jsoup.Nodes.Node> childNodes = EnsureChildNodes();
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
            IList<iText.StyledXmlParser.Jsoup.Nodes.Node> nodes = parentNode.EnsureChildNodes();
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
        /// <returns>next sibling, or @{code null} if this is the last sibling</returns>
        public virtual iText.StyledXmlParser.Jsoup.Nodes.Node NextSibling() {
            if (parentNode == null) {
                return null;
            }
            // root
            IList<iText.StyledXmlParser.Jsoup.Nodes.Node> siblings = parentNode.EnsureChildNodes();
            int index = siblingIndex + 1;
            if (siblings.Count > index) {
                return siblings[index];
            }
            else {
                return null;
            }
        }

        /// <summary>Get this node's previous sibling.</summary>
        /// <returns>the previous sibling, or @{code null} if this is the first sibling</returns>
        public virtual iText.StyledXmlParser.Jsoup.Nodes.Node PreviousSibling() {
            if (parentNode == null) {
                return null;
            }
            // root
            if (siblingIndex > 0) {
                return parentNode.EnsureChildNodes()[siblingIndex - 1];
            }
            else {
                return null;
            }
        }

        /// <summary>Get the list index of this node in its node sibling list.</summary>
        /// <remarks>
        /// Get the list index of this node in its node sibling list. E.g. if this is the first node
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
            NodeTraversor.Traverse(nodeVisitor, this);
            return this;
        }

        /// <summary>Perform a depth-first filtering through this node and its descendants.</summary>
        /// <param name="nodeFilter">the filter callbacks to perform on each node</param>
        /// <returns>this node, for chaining</returns>
        public virtual iText.StyledXmlParser.Jsoup.Nodes.Node Filter(NodeFilter nodeFilter) {
            Validate.NotNull(nodeFilter);
            NodeTraversor.Filter(nodeFilter, this);
            return this;
        }

        /// <summary>Get the outer HTML of this node.</summary>
        /// <remarks>
        /// Get the outer HTML of this node. For example, on a
        /// <c>p</c>
        /// element, may return
        /// <c>&lt;p&gt;Para&lt;/p&gt;</c>.
        /// </remarks>
        /// <returns>outer HTML</returns>
        /// <seealso cref="Element.Html()"/>
        /// <seealso cref="Element.Text()"/>
        public virtual String OuterHtml() {
            StringBuilder accum = iText.StyledXmlParser.Jsoup.Internal.StringUtil.BorrowBuilder();
            OuterHtml(accum);
            return iText.StyledXmlParser.Jsoup.Internal.StringUtil.ReleaseBuilder(accum);
        }

        protected internal virtual void OuterHtml(StringBuilder accum) {
            NodeTraversor.Traverse(new Node.OuterHtmlVisitor(accum, NodeUtils.OutputSettings(this)), this);
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

        /// <summary>Gets this node's outer HTML.</summary>
        /// <returns>outer HTML.</returns>
        /// <seealso cref="OuterHtml()"/>
        public override String ToString() {
            return OuterHtml();
        }

        protected internal virtual void Indent(StringBuilder accum, int depth, OutputSettings @out) {
            accum.Append('\n').Append(iText.StyledXmlParser.Jsoup.Internal.StringUtil.Padding(depth * @out.IndentAmount
                ()));
        }

        /// <summary>Check if this node is the same instance of another (object identity test).</summary>
        /// <remarks>
        /// Check if this node is the same instance of another (object identity test).
        /// <para />
        /// For an node value equality check, see
        /// <see cref="HasSameValue(System.Object)"/>
        /// </remarks>
        /// <param name="o">other object to compare to</param>
        /// <returns>true if the content of this node is the same as the other</returns>
        /// <seealso cref="HasSameValue(System.Object)"/>
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
        /// <returns>a stand-alone cloned node, including clones of any children</returns>
        /// <seealso cref="ShallowClone()"/>
        public virtual Object Clone() {
            iText.StyledXmlParser.Jsoup.Nodes.Node thisClone = DoClone(null);
            // splits for orphan
            // Queue up nodes that need their children cloned (BFS).
            LinkedList<iText.StyledXmlParser.Jsoup.Nodes.Node> nodesToProcess = new LinkedList<iText.StyledXmlParser.Jsoup.Nodes.Node
                >();
            nodesToProcess.Add(thisClone);
            while (!nodesToProcess.IsEmpty()) {
                iText.StyledXmlParser.Jsoup.Nodes.Node currParent = nodesToProcess.JRemove();
                int size = currParent.ChildNodeSize();
                for (int i = 0; i < size; i++) {
                    IList<iText.StyledXmlParser.Jsoup.Nodes.Node> childNodes = currParent.EnsureChildNodes();
                    iText.StyledXmlParser.Jsoup.Nodes.Node childClone = childNodes[i].DoClone(currParent);
                    childNodes[i] = childClone;
                    nodesToProcess.Add(childClone);
                }
            }
            return thisClone;
        }

        /// <summary>Create a stand-alone, shallow copy of this node.</summary>
        /// <remarks>
        /// Create a stand-alone, shallow copy of this node. None of its children (if any) will be cloned, and it will have
        /// no parent or sibling nodes.
        /// </remarks>
        /// <returns>a single independent copy of this node</returns>
        /// <seealso cref="Clone()"/>
        public virtual iText.StyledXmlParser.Jsoup.Nodes.Node ShallowClone() {
            return DoClone(null);
        }

        /*
        * Return a clone of the node using the given parent (which can be null).
        * Not a deep copy of children.
        */
        protected internal virtual iText.StyledXmlParser.Jsoup.Nodes.Node DoClone(iText.StyledXmlParser.Jsoup.Nodes.Node
             parent) {
            iText.StyledXmlParser.Jsoup.Nodes.Node clone = (iText.StyledXmlParser.Jsoup.Nodes.Node)MemberwiseClone();
            clone.parentNode = parent;
            // can be null, to create an orphan split
            clone.siblingIndex = parent == null ? 0 : siblingIndex;
            return clone;
        }

        private class OuterHtmlVisitor : NodeVisitor {
            private readonly StringBuilder accum;

            private readonly OutputSettings @out;

            internal OuterHtmlVisitor(StringBuilder accum, OutputSettings @out) {
                this.accum = accum;
                this.@out = @out;
                @out.PrepareEncoder();
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
