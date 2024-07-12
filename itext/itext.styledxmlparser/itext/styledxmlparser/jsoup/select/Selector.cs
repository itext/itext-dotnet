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
using iText.Commons.Utils;
using iText.StyledXmlParser.Jsoup.Helper;

namespace iText.StyledXmlParser.Jsoup.Select {
    /// <summary>CSS-like element selector, that finds elements matching a query.</summary>
    /// <remarks>
    /// CSS-like element selector, that finds elements matching a query.
    /// <h2>Selector syntax</h2>
    /// <para />
    /// A selector is a chain of simple selectors, separated by combinators. Selectors are <b>case insensitive</b> (including against
    /// elements, attributes, and attribute values).
    /// <para />
    /// The universal selector (*) is implicit when no element selector is supplied (i.e.
    /// <c>*.header</c>
    /// and
    /// <c>.header</c>
    /// is equivalent).
    /// <style>table.syntax tr td {vertical-align: top; padding-right: 2em; padding-top:0.5em; padding-bottom:0.5em; } table.syntax tr:hover{background-color: #eee;} table.syntax {border-spacing: 0px 0px;}</style>
    /// <table summary="" class="syntax"><colgroup><col span="1" style="width: 20%;"><col span="1" style="width: 40%;"><col span="1" style="width: 40%;"></colgroup>
    /// <tr><th align="left">Pattern</th><th align="left">Matches</th><th align="left">Example</th></tr>
    /// <tr><td><c>*</c></td><td>any element</td><td><c>*</c></td></tr>
    /// <tr><td><c>tag</c></td><td>elements with the given tag name</td><td><c>div</c></td></tr>
    /// <tr><td><c>*|E</c></td><td>elements of type E in any namespace <i>ns</i></td><td><c>*|name</c> finds <c>&lt;fb:name&gt;</c> elements</td></tr>
    /// <tr><td><c>ns|E</c></td><td>elements of type E in the namespace <i>ns</i></td><td><c>fb|name</c> finds <c>&lt;fb:name&gt;</c> elements</td></tr>
    /// <tr><td><c>#id</c></td><td>elements with attribute ID of "id"</td><td><c>div#wrap</c>, <c>#logo</c></td></tr>
    /// <tr><td><c>.class</c></td><td>elements with a class name of "class"</td><td><c>div.left</c>, <c>.result</c></td></tr>
    /// <tr><td><c>[attr]</c></td><td>elements with an attribute named "attr" (with any value)</td><td><c>a[href]</c>, <c>[title]</c></td></tr>
    /// <tr><td><c>[^attrPrefix]</c></td><td>elements with an attribute name starting with "attrPrefix". Use to find elements with HTML5 datasets</td><td><c>[^data-]</c>, <c>div[^data-]</c></td></tr>
    /// <tr><td><c>[attr=val]</c></td><td>elements with an attribute named "attr", and value equal to "val"</td><td><c>img[width=500]</c>, <c>a[rel=nofollow]</c></td></tr>
    /// <tr><td><c>[attr=&amp;quot;val&amp;quot;]</c></td><td>elements with an attribute named "attr", and value equal to "val"</td><td><c>span[hello="Cleveland"][goodbye="Columbus"]</c>, <c>a[rel=&amp;quot;nofollow&amp;quot;]</c></td></tr>
    /// <tr><td><c>[attr^=valPrefix]</c></td><td>elements with an attribute named "attr", and value starting with "valPrefix"</td><td><c>a[href^=http:]</c></td></tr>
    /// <tr><td><c>[attr$=valSuffix]</c></td><td>elements with an attribute named "attr", and value ending with "valSuffix"</td><td><c>img[src$=.png]</c></td></tr>
    /// <tr><td><c>[attr*=valContaining]</c></td><td>elements with an attribute named "attr", and value containing "valContaining"</td><td><c>a[href*=/search/]</c></td></tr>
    /// <tr><td><c>[attr~=<em>regex</em>]</c></td><td>elements with an attribute named "attr", and value matching the regular expression</td><td><c>img[src~=(?i)\\.(png|jpe?g)]</c></td></tr>
    /// <tr><td></td><td>The above may be combined in any order</td><td><c>div.header[title]</c></td></tr>
    /// <tr><td colspan="3"><h3>Combinators</h3></td></tr>
    /// <tr><td><c>E F</c></td><td>an F element descended from an E element</td><td><c>div a</c>, <c>.logo h1</c></td></tr>
    /// <tr><td><c>E
    /// <c>&gt;</c>
    /// F</c></td><td>an F direct child of E</td><td><c>ol
    /// <c>&gt;</c>
    /// li</c></td></tr>
    /// <tr><td><c>E + F</c></td><td>an F element immediately preceded by sibling E</td><td><c>li + li</c>, <c>div.head + div</c></td></tr>
    /// <tr><td><c>E ~ F</c></td><td>an F element preceded by sibling E</td><td><c>h1 ~ p</c></td></tr>
    /// <tr><td><c>E, F, G</c></td><td>all matching elements E, F, or G</td><td><c>a[href], div, h3</c></td></tr>
    /// <tr><td colspan="3"><h3>Pseudo selectors</h3></td></tr>
    /// <tr><td><c>:lt(<em>n</em>)</c></td><td>elements whose sibling index is less than <em>n</em></td><td><c>td:lt(3)</c> finds the first 3 cells of each row</td></tr>
    /// <tr><td><c>:gt(<em>n</em>)</c></td><td>elements whose sibling index is greater than <em>n</em></td><td><c>td:gt(1)</c> finds cells after skipping the first two</td></tr>
    /// <tr><td><c>:eq(<em>n</em>)</c></td><td>elements whose sibling index is equal to <em>n</em></td><td><c>td:eq(0)</c> finds the first cell of each row</td></tr>
    /// <tr><td><c>:has(<em>selector</em>)</c></td><td>elements that contains at least one element matching the <em>selector</em></td><td><c>div:has(p)</c> finds <c>div</c>s that contain <c>p</c> elements.<br /><c>div:has(&gt; a)</c> selects <c>div</c> elements that have at least one direct child <c>a</c> element.</td></tr>
    /// <tr><td><c>:not(<em>selector</em>)</c></td><td>elements that do not match the <em>selector</em>. See also
    /// <see cref="Elements.Not(System.String)"/>
    /// </td><td><c>div:not(.logo)</c> finds all divs that do not have the "logo" class.<para /><c>div:not(:has(div))</c> finds divs that do not contain divs.</td></tr>
    /// <tr><td><c>:contains(<em>text</em>)</c></td><td>elements that contains the specified text. The search is case insensitive. The text may appear in the found element, or any of its descendants.</td><td><c>p:contains(jsoup)</c> finds p elements containing the text "jsoup".</td></tr>
    /// <tr><td><c>:matches(<em>regex</em>)</c></td><td>elements whose text matches the specified regular expression. The text may appear in the found element, or any of its descendants.</td><td><c>td:matches(\\d+)</c> finds table cells containing digits. <c>div:matches((?i)login)</c> finds divs containing the text, case insensitively.</td></tr>
    /// <tr><td><c>:containsOwn(<em>text</em>)</c></td><td>elements that directly contain the specified text. The search is case insensitive. The text must appear in the found element, not any of its descendants.</td><td><c>p:containsOwn(jsoup)</c> finds p elements with own text "jsoup".</td></tr>
    /// <tr><td><c>:matchesOwn(<em>regex</em>)</c></td><td>elements whose own text matches the specified regular expression. The text must appear in the found element, not any of its descendants.</td><td><c>td:matchesOwn(\\d+)</c> finds table cells directly containing digits. <c>div:matchesOwn((?i)login)</c> finds divs containing the text, case insensitively.</td></tr>
    /// <tr><td><c>:containsData(<em>data</em>)</c></td><td>elements that contains the specified <em>data</em>. The contents of
    /// <c>script</c>
    /// and
    /// <c>style</c>
    /// elements, and
    /// <c>comment</c>
    /// nodes (etc) are considered data nodes, not text nodes. The search is case insensitive. The data may appear in the found element, or any of its descendants.</td><td><c>script:contains(jsoup)</c> finds script elements containing the data "jsoup".</td></tr>
    /// <tr><td></td><td>The above may be combined in any order and with other selectors</td><td><c>.light:contains(name):eq(0)</c></td></tr>
    /// <tr><td><c>:matchText</c></td><td>treats text nodes as elements, and so allows you to match against and select text nodes.<para /><b>Note</b> that using this selector will modify the DOM, so you may want to
    /// <c>clone</c>
    /// your document before using.</td><td>
    /// <c>p:matchText:firstChild</c>
    /// with input
    /// <c>&lt;p&gt;One&lt;br /&gt;Two&lt;/p&gt;</c>
    /// will return one
    /// <see cref="iText.StyledXmlParser.Jsoup.Nodes.PseudoTextElement"/>
    /// with text "
    /// <c>One</c>
    /// ".</td></tr>
    /// <tr><td colspan="3"><h3>Structural pseudo selectors</h3></td></tr>
    /// <tr><td><c>:root</c></td><td>The element that is the root of the document. In HTML, this is the <c>html</c> element</td><td><c>:root</c></td></tr>
    /// <tr><td><c>:nth-child(<em>a</em>n+<em>b</em>)</c></td><td><para />elements that have <c><em>a</em>n+<em>b</em>-1</c> siblings <b>before</b> it in the document tree, for any positive integer or zero value of <c>n</c>, and has a parent element. For values of <c>a</c> and <c>b</c> greater than zero, this effectively divides the element's children into groups of a elements (the last group taking the remainder), and selecting the <em>b</em>th element of each group. For example, this allows the selectors to address every other row in a table, and could be used to alternate the color of paragraph text in a cycle of four. The <c>a</c> and <c>b</c> values must be integers (positive, negative, or zero). The index of the first child of an element is 1.
    /// In addition to this, <c>:nth-child()</c> can take <c>odd</c> and <c>even</c> as arguments instead. <c>odd</c> has the same signification as <c>2n+1</c>, and <c>even</c> has the same signification as <c>2n</c>.</td><td><c>tr:nth-child(2n+1)</c> finds every odd row of a table. <c>:nth-child(10n-1)</c> the 9th, 19th, 29th, etc, element. <c>li:nth-child(5)</c> the 5h li</td></tr>
    /// <tr><td><c>:nth-last-child(<em>a</em>n+<em>b</em>)</c></td><td>elements that have <c><em>a</em>n+<em>b</em>-1</c> siblings <b>after</b> it in the document tree. Otherwise like <c>:nth-child()</c></td><td><c>tr:nth-last-child(-n+2)</c> the last two rows of a table</td></tr>
    /// <tr><td><c>:nth-of-type(<em>a</em>n+<em>b</em>)</c></td><td>pseudo-class notation represents an element that has <c><em>a</em>n+<em>b</em>-1</c> siblings with the same expanded element name <em>before</em> it in the document tree, for any zero or positive integer value of n, and has a parent element</td><td><c>img:nth-of-type(2n+1)</c></td></tr>
    /// <tr><td><c>:nth-last-of-type(<em>a</em>n+<em>b</em>)</c></td><td>pseudo-class notation represents an element that has <c><em>a</em>n+<em>b</em>-1</c> siblings with the same expanded element name <em>after</em> it in the document tree, for any zero or positive integer value of n, and has a parent element</td><td><c>img:nth-last-of-type(2n+1)</c></td></tr>
    /// <tr><td><c>:first-child</c></td><td>elements that are the first child of some other element.</td><td><c>div
    /// <c>&gt;</c>
    /// p:first-child</c></td></tr>
    /// <tr><td><c>:last-child</c></td><td>elements that are the last child of some other element.</td><td><c>ol
    /// <c>&gt;</c>
    /// li:last-child</c></td></tr>
    /// <tr><td><c>:first-of-type</c></td><td>elements that are the first sibling of its type in the list of children of its parent element</td><td><c>dl dt:first-of-type</c></td></tr>
    /// <tr><td><c>:last-of-type</c></td><td>elements that are the last sibling of its type in the list of children of its parent element</td><td><c>tr
    /// <c>&gt;</c>
    /// td:last-of-type</c></td></tr>
    /// <tr><td><c>:only-child</c></td><td>elements that have a parent element and whose parent element have no other element children</td><td></td></tr>
    /// <tr><td><c>:only-of-type</c></td><td> an element that has a parent element and whose parent element has no other element children with the same expanded element name</td><td></td></tr>
    /// <tr><td><c>:empty</c></td><td>elements that have no children at all</td><td></td></tr>
    /// </table>
    /// </remarks>
    /// <seealso cref="iText.StyledXmlParser.Jsoup.Nodes.Element.Select(System.String)"/>
    public class Selector {
        // not instantiable
        private Selector() {
        }

        /// <summary>Find elements matching selector.</summary>
        /// <param name="query">CSS selector</param>
        /// <param name="root">root element to descend into</param>
        /// <returns>matching elements, empty if none</returns>
        public static Elements Select(String query, iText.StyledXmlParser.Jsoup.Nodes.Element root) {
            Validate.NotEmpty(query);
            return Select(QueryParser.Parse(query), root);
        }

        /// <summary>Find elements matching selector.</summary>
        /// <param name="evaluator">CSS selector</param>
        /// <param name="root">root element to descend into</param>
        /// <returns>matching elements, empty if none</returns>
        public static Elements Select(Evaluator evaluator, iText.StyledXmlParser.Jsoup.Nodes.Element root) {
            Validate.NotNull(evaluator);
            Validate.NotNull(root);
            return Collector.Collect(evaluator, root);
        }

        /// <summary>Find elements matching selector.</summary>
        /// <param name="query">CSS selector</param>
        /// <param name="roots">root elements to descend into</param>
        /// <returns>matching elements, empty if none</returns>
        public static Elements Select(String query, IEnumerable<iText.StyledXmlParser.Jsoup.Nodes.Element> roots) {
            Validate.NotEmpty(query);
            Validate.NotNull(roots);
            Evaluator evaluator = QueryParser.Parse(query);
            Elements elements = new Elements();
            IdentityDictionary<iText.StyledXmlParser.Jsoup.Nodes.Element, bool?> seenElements = new IdentityDictionary
                <iText.StyledXmlParser.Jsoup.Nodes.Element, bool?>();
            // dedupe elements by identity, not equality
            foreach (iText.StyledXmlParser.Jsoup.Nodes.Element root in roots) {
                Elements found = Select(evaluator, root);
                foreach (iText.StyledXmlParser.Jsoup.Nodes.Element el in found) {
                    if (seenElements.Put(el, true) == null) {
                        elements.Add(el);
                    }
                }
            }
            return elements;
        }

//\cond DO_NOT_DOCUMENT
        // exclude set. package open so that Elements can implement .not() selector.
        internal static Elements FilterOut(ICollection<iText.StyledXmlParser.Jsoup.Nodes.Element> elements, ICollection
            <iText.StyledXmlParser.Jsoup.Nodes.Element> outs) {
            Elements output = new Elements();
            foreach (iText.StyledXmlParser.Jsoup.Nodes.Element el in elements) {
                bool found = false;
                foreach (iText.StyledXmlParser.Jsoup.Nodes.Element @out in outs) {
                    if (el.Equals(@out)) {
                        found = true;
                        break;
                    }
                }
                if (!found) {
                    output.Add(el);
                }
            }
            return output;
        }
//\endcond

        /// <summary>Find the first element that matches the query.</summary>
        /// <param name="cssQuery">CSS selector</param>
        /// <param name="root">root element to descend into</param>
        /// <returns>the matching element, or <b>null</b> if none.</returns>
        public static iText.StyledXmlParser.Jsoup.Nodes.Element SelectFirst(String cssQuery, iText.StyledXmlParser.Jsoup.Nodes.Element
             root) {
            Validate.NotEmpty(cssQuery);
            return Collector.FindFirst(QueryParser.Parse(cssQuery), root);
        }

        public class SelectorParseException : InvalidOperationException {
            public SelectorParseException(String msg, params Object[] @params)
                : base(MessageFormatUtil.Format(msg, @params)) {
            }
        }
    }
}
