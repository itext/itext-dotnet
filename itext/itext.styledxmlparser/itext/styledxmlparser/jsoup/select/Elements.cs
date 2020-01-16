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
using iText.StyledXmlParser.Jsoup.Helper;
using iText.StyledXmlParser.Jsoup.Nodes;
using iText.IO.Util;

namespace iText.StyledXmlParser.Jsoup.Select {
    /// <summary>
    /// A list of
    /// <see cref="iText.StyledXmlParser.Jsoup.Nodes.Element"/>
    /// s, with methods that act on every element in the list.
    /// <p>
    /// To get an
    /// <c>Elements</c>
    /// object, use the
    /// <see cref="iText.StyledXmlParser.Jsoup.Nodes.Element.Select(System.String)"/>
    /// method.
    /// </summary>
    /// <author>Jonathan Hedley, jonathan@hedley.net</author>
    public class Elements : List<Element> {
        public Elements() {
        }

        public Elements(int initialCapacity)
            : base(initialCapacity) {
        }

        public Elements(ICollection<Element> elements)
            : base(elements) {
        }

        public Elements(IList<Element> elements)
            : base(elements) {
        }

        public Elements(params Element[] elements)
            : base(iText.IO.Util.JavaUtil.ArraysAsList(elements)) {
        }

        /// <summary>Creates a deep copy of these elements.</summary>
        /// <returns>a deep copy</returns>
        public Object Clone() {
            iText.StyledXmlParser.Jsoup.Select.Elements clone = new iText.StyledXmlParser.Jsoup.Select.Elements(Count);
            foreach (Element e in this) {
                clone.Add((Element)e.Clone());
            }
            return clone;
        }

        // attribute methods
        /// <summary>Get an attribute value from the first matched element that has the attribute.</summary>
        /// <param name="attributeKey">The attribute key.</param>
        /// <returns>
        /// The attribute value from the first matched element that has the attribute.. If no elements were matched (isEmpty() == true),
        /// or if the no elements have the attribute, returns empty string.
        /// </returns>
        /// <seealso cref="HasAttr(System.String)"/>
        public virtual String Attr(String attributeKey) {
            foreach (Element element in this) {
                if (element.HasAttr(attributeKey)) {
                    return element.Attr(attributeKey);
                }
            }
            return "";
        }

        /// <summary>Checks if any of the matched elements have this attribute set.</summary>
        /// <param name="attributeKey">attribute key</param>
        /// <returns>true if any of the elements have the attribute; false if none do.</returns>
        public virtual bool HasAttr(String attributeKey) {
            foreach (Element element in this) {
                if (element.HasAttr(attributeKey)) {
                    return true;
                }
            }
            return false;
        }

        /// <summary>Set an attribute on all matched elements.</summary>
        /// <param name="attributeKey">attribute key</param>
        /// <param name="attributeValue">attribute value</param>
        /// <returns>this</returns>
        public virtual iText.StyledXmlParser.Jsoup.Select.Elements Attr(String attributeKey, String attributeValue) {
            foreach (Element element in this) {
                element.Attr(attributeKey, attributeValue);
            }
            return this;
        }

        /// <summary>Remove an attribute from every matched element.</summary>
        /// <param name="attributeKey">The attribute to remove.</param>
        /// <returns>this (for chaining)</returns>
        public virtual iText.StyledXmlParser.Jsoup.Select.Elements RemoveAttr(String attributeKey) {
            foreach (Element element in this) {
                element.RemoveAttr(attributeKey);
            }
            return this;
        }

        /// <summary>
        /// Add the class name to every matched element's
        /// <c>class</c>
        /// attribute.
        /// </summary>
        /// <param name="className">class name to add</param>
        /// <returns>this</returns>
        public virtual iText.StyledXmlParser.Jsoup.Select.Elements AddClass(String className) {
            foreach (Element element in this) {
                element.AddClass(className);
            }
            return this;
        }

        /// <summary>
        /// Remove the class name from every matched element's
        /// <c>class</c>
        /// attribute, if present.
        /// </summary>
        /// <param name="className">class name to remove</param>
        /// <returns>this</returns>
        public virtual iText.StyledXmlParser.Jsoup.Select.Elements RemoveClass(String className) {
            foreach (Element element in this) {
                element.RemoveClass(className);
            }
            return this;
        }

        /// <summary>
        /// Toggle the class name on every matched element's
        /// <c>class</c>
        /// attribute.
        /// </summary>
        /// <param name="className">class name to add if missing, or remove if present, from every element.</param>
        /// <returns>this</returns>
        public virtual iText.StyledXmlParser.Jsoup.Select.Elements ToggleClass(String className) {
            foreach (Element element in this) {
                element.ToggleClass(className);
            }
            return this;
        }

        /// <summary>
        /// Determine if any of the matched elements have this class name set in their
        /// <c>class</c>
        /// attribute.
        /// </summary>
        /// <param name="className">class name to check for</param>
        /// <returns>true if any do, false if none do</returns>
        public virtual bool HasClass(String className) {
            foreach (Element element in this) {
                if (element.HasClass(className)) {
                    return true;
                }
            }
            return false;
        }

        /// <summary>Get the form element's value of the first matched element.</summary>
        /// <returns>The form element's value, or empty if not set.</returns>
        /// <seealso cref="iText.StyledXmlParser.Jsoup.Nodes.Element.Val()"/>
        public virtual String Val() {
            if (Count > 0) {
                return First().Val();
            }
            else {
                return "";
            }
        }

        /// <summary>Set the form element's value in each of the matched elements.</summary>
        /// <param name="value">The value to set into each matched element</param>
        /// <returns>this (for chaining)</returns>
        public virtual iText.StyledXmlParser.Jsoup.Select.Elements Val(String value) {
            foreach (Element element in this) {
                element.Val(value);
            }
            return this;
        }

        /// <summary>Get the combined text of all the matched elements.</summary>
        /// <remarks>
        /// Get the combined text of all the matched elements.
        /// <p>
        /// Note that it is possible to get repeats if the matched elements contain both parent elements and their own
        /// children, as the Element.text() method returns the combined text of a parent and all its children.
        /// </remarks>
        /// <returns>string of all text: unescaped and no HTML.</returns>
        /// <seealso cref="iText.StyledXmlParser.Jsoup.Nodes.Element.Text()"/>
        public virtual String Text() {
            StringBuilder sb = new StringBuilder();
            foreach (Element element in this) {
                if (sb.Length != 0) {
                    sb.Append(" ");
                }
                sb.Append(element.Text());
            }
            return sb.ToString();
        }

        public virtual bool HasText() {
            foreach (Element element in this) {
                if (element.HasText()) {
                    return true;
                }
            }
            return false;
        }

        /// <summary>Get the combined inner HTML of all matched elements.</summary>
        /// <returns>string of all element's inner HTML.</returns>
        /// <seealso cref="Text()"/>
        /// <seealso cref="OuterHtml()"/>
        public virtual String Html() {
            StringBuilder sb = new StringBuilder();
            foreach (Element element in this) {
                if (sb.Length != 0) {
                    sb.Append("\n");
                }
                sb.Append(element.Html());
            }
            return sb.ToString();
        }

        /// <summary>Get the combined outer HTML of all matched elements.</summary>
        /// <returns>string of all element's outer HTML.</returns>
        /// <seealso cref="Text()"/>
        /// <seealso cref="Html()"/>
        public virtual String OuterHtml() {
            StringBuilder sb = new StringBuilder();
            foreach (Element element in this) {
                if (sb.Length != 0) {
                    sb.Append("\n");
                }
                sb.Append(element.OuterHtml());
            }
            return sb.ToString();
        }

        /// <summary>Get the combined outer HTML of all matched elements.</summary>
        /// <remarks>
        /// Get the combined outer HTML of all matched elements. Alias of
        /// <see cref="OuterHtml()"/>
        /// .
        /// </remarks>
        /// <returns>string of all element's outer HTML.</returns>
        /// <seealso cref="Text()"/>
        /// <seealso cref="Html()"/>
        public override String ToString() {
            return OuterHtml();
        }

        /// <summary>Update the tag name of each matched element.</summary>
        /// <remarks>
        /// Update the tag name of each matched element. For example, to change each
        /// <c>&lt;i&gt;</c>
        /// to a
        /// <c>&lt;em&gt;</c>
        /// , do
        /// <c>doc.select("i").tagName("em");</c>
        /// </remarks>
        /// <param name="tagName">the new tag name</param>
        /// <returns>this, for chaining</returns>
        /// <seealso cref="iText.StyledXmlParser.Jsoup.Nodes.Element.TagName(System.String)"/>
        public virtual iText.StyledXmlParser.Jsoup.Select.Elements TagName(String tagName) {
            foreach (Element element in this) {
                element.TagName(tagName);
            }
            return this;
        }

        /// <summary>Set the inner HTML of each matched element.</summary>
        /// <param name="html">HTML to parse and set into each matched element.</param>
        /// <returns>this, for chaining</returns>
        /// <seealso cref="iText.StyledXmlParser.Jsoup.Nodes.Element.Html(System.String)"/>
        public virtual iText.StyledXmlParser.Jsoup.Select.Elements Html(String html) {
            foreach (Element element in this) {
                element.Html(html);
            }
            return this;
        }

        /// <summary>Add the supplied HTML to the start of each matched element's inner HTML.</summary>
        /// <param name="html">HTML to add inside each element, before the existing HTML</param>
        /// <returns>this, for chaining</returns>
        /// <seealso cref="iText.StyledXmlParser.Jsoup.Nodes.Element.Prepend(System.String)"/>
        public virtual iText.StyledXmlParser.Jsoup.Select.Elements Prepend(String html) {
            foreach (Element element in this) {
                element.Prepend(html);
            }
            return this;
        }

        /// <summary>Add the supplied HTML to the end of each matched element's inner HTML.</summary>
        /// <param name="html">HTML to add inside each element, after the existing HTML</param>
        /// <returns>this, for chaining</returns>
        /// <seealso cref="iText.StyledXmlParser.Jsoup.Nodes.Element.Append(System.String)"/>
        public virtual iText.StyledXmlParser.Jsoup.Select.Elements Append(String html) {
            foreach (Element element in this) {
                element.Append(html);
            }
            return this;
        }

        /// <summary>Insert the supplied HTML before each matched element's outer HTML.</summary>
        /// <param name="html">HTML to insert before each element</param>
        /// <returns>this, for chaining</returns>
        /// <seealso cref="iText.StyledXmlParser.Jsoup.Nodes.Element.Before(System.String)"/>
        public virtual iText.StyledXmlParser.Jsoup.Select.Elements Before(String html) {
            foreach (Element element in this) {
                element.Before(html);
            }
            return this;
        }

        /// <summary>Insert the supplied HTML after each matched element's outer HTML.</summary>
        /// <param name="html">HTML to insert after each element</param>
        /// <returns>this, for chaining</returns>
        /// <seealso cref="iText.StyledXmlParser.Jsoup.Nodes.Element.After(System.String)"/>
        public virtual iText.StyledXmlParser.Jsoup.Select.Elements After(String html) {
            foreach (Element element in this) {
                element.After(html);
            }
            return this;
        }

        /// <summary>Wrap the supplied HTML around each matched elements.</summary>
        /// <remarks>
        /// Wrap the supplied HTML around each matched elements. For example, with HTML
        /// <c>&lt;p&gt;&lt;b&gt;This&lt;/b&gt; is &lt;b&gt;Jsoup&lt;/b&gt;&lt;/p&gt;</c>
        /// ,
        /// <code>doc.select("b").wrap("&lt;i&gt;&lt;/i&gt;");</code>
        /// becomes
        /// <c>&lt;p&gt;&lt;i&gt;&lt;b&gt;This&lt;/b&gt;&lt;/i&gt; is &lt;i&gt;&lt;b&gt;jsoup&lt;/b&gt;&lt;/i&gt;&lt;/p&gt;
        ///     </c>
        /// </remarks>
        /// <param name="html">
        /// HTML to wrap around each element, e.g.
        /// <c>&lt;div class="head"&gt;&lt;/div&gt;</c>
        /// . Can be arbitrarily deep.
        /// </param>
        /// <returns>this (for chaining)</returns>
        /// <seealso cref="iText.StyledXmlParser.Jsoup.Nodes.Element.Wrap(System.String)"/>
        public virtual iText.StyledXmlParser.Jsoup.Select.Elements Wrap(String html) {
            Validate.NotEmpty(html);
            foreach (Element element in this) {
                element.Wrap(html);
            }
            return this;
        }

        /// <summary>Removes the matched elements from the DOM, and moves their children up into their parents.</summary>
        /// <remarks>
        /// Removes the matched elements from the DOM, and moves their children up into their parents. This has the effect of
        /// dropping the elements but keeping their children.
        /// <p>
        /// This is useful for e.g removing unwanted formatting elements but keeping their contents.
        /// E.g. with HTML: <p>
        /// <c>&lt;div&gt;&lt;font&gt;One&lt;/font&gt; &lt;font&gt;&lt;a href="/"&gt;Two&lt;/a&gt;&lt;/font&gt;&lt;/div&gt;
        ///     </c>
        /// <p>
        /// <c>doc.select("font").unwrap();</c>
        /// <p>HTML =
        /// <c>&lt;div&gt;One &lt;a href="/"&gt;Two&lt;/a&gt;&lt;/div&gt;</c>
        /// </remarks>
        /// <returns>this (for chaining)</returns>
        /// <seealso cref="iText.StyledXmlParser.Jsoup.Nodes.Node.Unwrap()"/>
        public virtual iText.StyledXmlParser.Jsoup.Select.Elements Unwrap() {
            foreach (Element element in this) {
                element.Unwrap();
            }
            return this;
        }

        /// <summary>Empty (remove all child nodes from) each matched element.</summary>
        /// <remarks>
        /// Empty (remove all child nodes from) each matched element. This is similar to setting the inner HTML of each
        /// element to nothing.
        /// <p>
        /// E.g. HTML:
        /// <c>&lt;div&gt;&lt;p&gt;Hello &lt;b&gt;there&lt;/b&gt;&lt;/p&gt; &lt;p&gt;now&lt;/p&gt;&lt;/div&gt;</c>
        /// <br />
        /// <code>doc.select("p").empty();</code><br />
        /// HTML =
        /// <c>&lt;div&gt;&lt;p&gt;&lt;/p&gt; &lt;p&gt;&lt;/p&gt;&lt;/div&gt;</c>
        /// </remarks>
        /// <returns>this, for chaining</returns>
        /// <seealso cref="iText.StyledXmlParser.Jsoup.Nodes.Element.Empty()"/>
        /// <seealso cref="Remove()"/>
        public virtual iText.StyledXmlParser.Jsoup.Select.Elements Empty() {
            foreach (Element element in this) {
                element.Empty();
            }
            return this;
        }

        /// <summary>Remove each matched element from the DOM.</summary>
        /// <remarks>
        /// Remove each matched element from the DOM. This is similar to setting the outer HTML of each element to nothing.
        /// <p>
        /// E.g. HTML:
        /// <c>&lt;div&gt;&lt;p&gt;Hello&lt;/p&gt; &lt;p&gt;there&lt;/p&gt; &lt;img /&gt;&lt;/div&gt;</c>
        /// <br />
        /// <code>doc.select("p").remove();</code><br />
        /// HTML =
        /// <c>&lt;div&gt; &lt;img /&gt;&lt;/div&gt;</c>
        /// <p>
        /// Note that this method should not be used to clean user-submitted HTML; rather, use
        /// <see cref="iText.StyledXmlParser.Jsoup.Safety.Cleaner"/>
        /// to clean HTML.
        /// </remarks>
        /// <returns>this, for chaining</returns>
        /// <seealso cref="iText.StyledXmlParser.Jsoup.Nodes.Element.Empty()"/>
        /// <seealso cref="Empty()"/>
        public virtual iText.StyledXmlParser.Jsoup.Select.Elements Remove() {
            foreach (Element element in this) {
                element.Remove();
            }
            return this;
        }

        // filters
        /// <summary>Find matching elements within this element list.</summary>
        /// <param name="query">
        /// A
        /// <see cref="Selector"/>
        /// query
        /// </param>
        /// <returns>the filtered list of elements, or an empty list if none match.</returns>
        public virtual iText.StyledXmlParser.Jsoup.Select.Elements Select(String query) {
            return Selector.Select(query, this);
        }

        /// <summary>
        /// Remove elements from this list that match the
        /// <see cref="Selector"/>
        /// query.
        /// <p>
        /// E.g. HTML:
        /// <c>&lt;div class=logo&gt;One&lt;/div&gt; &lt;div&gt;Two&lt;/div&gt;</c>
        /// <br />
        /// <code>Elements divs = doc.select("div").not(".logo");</code><br />
        /// Result:
        /// <c>divs: [&lt;div&gt;Two&lt;/div&gt;]</c>
        /// <p>
        /// </summary>
        /// <param name="query">the selector query whose results should be removed from these elements</param>
        /// <returns>a new elements list that contains only the filtered results</returns>
        public virtual iText.StyledXmlParser.Jsoup.Select.Elements Not(String query) {
            iText.StyledXmlParser.Jsoup.Select.Elements @out = Selector.Select(query, this);
            return Selector.FilterOut(this, @out);
        }

        /// <summary>Get the <i>nth</i> matched element as an Elements object.</summary>
        /// <remarks>
        /// Get the <i>nth</i> matched element as an Elements object.
        /// <p>
        /// See also
        /// <see cref="System.Collections.Generic.List{E}.Get(int)"/>
        /// to retrieve an Element.
        /// </remarks>
        /// <param name="index">the (zero-based) index of the element in the list to retain</param>
        /// <returns>Elements containing only the specified element, or, if that element did not exist, an empty list.
        ///     </returns>
        public virtual iText.StyledXmlParser.Jsoup.Select.Elements Eq(int index) {
            return Count > index ? new iText.StyledXmlParser.Jsoup.Select.Elements(this[index]) : new iText.StyledXmlParser.Jsoup.Select.Elements();
        }

        /// <summary>Test if any of the matched elements match the supplied query.</summary>
        /// <param name="query">A selector</param>
        /// <returns>true if at least one element in the list matches the query.</returns>
        public virtual bool Is(String query) {
            iText.StyledXmlParser.Jsoup.Select.Elements children = Select(query);
            return !children.IsEmpty();
        }

        /// <summary>Get all of the parents and ancestor elements of the matched elements.</summary>
        /// <returns>all of the parents and ancestor elements of the matched elements</returns>
        public virtual iText.StyledXmlParser.Jsoup.Select.Elements Parents() {
            ICollection<Element> combo = new LinkedHashSet<Element>();
            foreach (Element e in this) {
                combo.AddAll(e.Parents());
            }
            return new iText.StyledXmlParser.Jsoup.Select.Elements(combo);
        }

        // list-like methods
        /// <summary>Get the first matched element.</summary>
        /// <returns>The first matched element, or <code>null</code> if contents is empty.</returns>
        public virtual Element First() {
            return this.IsEmpty() ? null : this[0];
        }

        /// <summary>Get the last matched element.</summary>
        /// <returns>The last matched element, or <code>null</code> if contents is empty.</returns>
        public virtual Element Last() {
            return this.IsEmpty() ? null : this[Count - 1];
        }

        /// <summary>Perform a depth-first traversal on each of the selected elements.</summary>
        /// <param name="nodeVisitor">the visitor callbacks to perform on each node</param>
        /// <returns>this, for chaining</returns>
        public virtual iText.StyledXmlParser.Jsoup.Select.Elements Traverse(NodeVisitor nodeVisitor) {
            Validate.NotNull(nodeVisitor);
            NodeTraversor traversor = new NodeTraversor(nodeVisitor);
            foreach (Element el in this) {
                traversor.Traverse(el);
            }
            return this;
        }

        /// <summary>
        /// Get the
        /// <see cref="iText.StyledXmlParser.Jsoup.Nodes.FormElement"/>
        /// forms from the selected elements, if any.
        /// </summary>
        /// <returns>
        /// a list of
        /// <see cref="iText.StyledXmlParser.Jsoup.Nodes.FormElement"/>
        /// s pulled from the matched elements. The list will be empty if the elements contain
        /// no forms.
        /// </returns>
        public virtual IList<FormElement> Forms() {
            List<FormElement> forms = new List<FormElement>();
            foreach (Element el in this) {
                if (el is FormElement) {
                    forms.Add((FormElement)el);
                }
            }
            return forms;
        }
    }
}
