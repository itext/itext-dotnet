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
using System.Text.RegularExpressions;
using iText.IO.Util;
using iText.StyledXmlParser.Jsoup;
using iText.StyledXmlParser.Jsoup.Helper;
using iText.StyledXmlParser.Jsoup.Nodes;

namespace iText.StyledXmlParser.Jsoup.Select {
    /// <summary>Evaluates that an element matches the selector.</summary>
    public abstract class Evaluator {
        protected internal Evaluator() {
        }

        /// <summary>Test if the element meets the evaluator's requirements.</summary>
        /// <param name="root">Root of the matching subtree</param>
        /// <param name="element">tested element</param>
        /// <returns>
        /// Returns <tt>true</tt> if the requirements are met or
        /// <tt>false</tt> otherwise
        /// </returns>
        public abstract bool Matches(iText.StyledXmlParser.Jsoup.Nodes.Element root, iText.StyledXmlParser.Jsoup.Nodes.Element
             element);

        /// <summary>Evaluator for tag name</summary>
        public sealed class Tag : Evaluator {
            private String tagName;

            public Tag(String tagName) {
                this.tagName = tagName;
            }

            public override bool Matches(iText.StyledXmlParser.Jsoup.Nodes.Element root, iText.StyledXmlParser.Jsoup.Nodes.Element
                 element) {
                return (element.TagName().Equals(tagName));
            }

            public override String ToString() {
                return MessageFormatUtil.Format("{0}", tagName);
            }
        }

        /// <summary>Evaluator for element id</summary>
        public sealed class ID : Evaluator {
            private String id;

            public ID(String id) {
                this.id = id;
            }

            public override bool Matches(iText.StyledXmlParser.Jsoup.Nodes.Element root, iText.StyledXmlParser.Jsoup.Nodes.Element
                 element) {
                return (id.Equals(element.Id()));
            }

            public override String ToString() {
                return MessageFormatUtil.Format("#{0}", id);
            }
        }

        /// <summary>Evaluator for element class</summary>
        public sealed class Class : Evaluator {
            private String className;

            public Class(String className) {
                this.className = className;
            }

            public override bool Matches(iText.StyledXmlParser.Jsoup.Nodes.Element root, iText.StyledXmlParser.Jsoup.Nodes.Element
                 element) {
                return (element.HasClass(className));
            }

            public override String ToString() {
                return MessageFormatUtil.Format(".{0}", className);
            }
        }

        /// <summary>Evaluator for attribute name matching</summary>
        public sealed class Attribute : Evaluator {
            private String key;

            public Attribute(String key) {
                this.key = key;
            }

            public override bool Matches(iText.StyledXmlParser.Jsoup.Nodes.Element root, iText.StyledXmlParser.Jsoup.Nodes.Element
                 element) {
                return element.HasAttr(key);
            }

            public override String ToString() {
                return MessageFormatUtil.Format("[{0}]", key);
            }
        }

        /// <summary>Evaluator for attribute name prefix matching</summary>
        public sealed class AttributeStarting : Evaluator {
            private String keyPrefix;

            public AttributeStarting(String keyPrefix) {
                this.keyPrefix = keyPrefix;
            }

            public override bool Matches(iText.StyledXmlParser.Jsoup.Nodes.Element root, iText.StyledXmlParser.Jsoup.Nodes.Element
                 element) {
                IList<iText.StyledXmlParser.Jsoup.Nodes.Attribute> values = element.Attributes().AsList();
                foreach (iText.StyledXmlParser.Jsoup.Nodes.Attribute attribute in values) {
                    if (attribute.Key.StartsWith(keyPrefix)) {
                        return true;
                    }
                }
                return false;
            }

            public override String ToString() {
                return MessageFormatUtil.Format("[^{0}]", keyPrefix);
            }
        }

        /// <summary>Evaluator for attribute name/value matching</summary>
        public sealed class AttributeWithValue : Evaluator.AttributeKeyPair {
            public AttributeWithValue(String key, String value)
                : base(key, value) {
            }

            public override bool Matches(iText.StyledXmlParser.Jsoup.Nodes.Element root, iText.StyledXmlParser.Jsoup.Nodes.Element
                 element) {
                return element.HasAttr(key) && value.EqualsIgnoreCase(element.Attr(key).Trim());
            }

            public override String ToString() {
                return MessageFormatUtil.Format("[{0}={1}]", key, value);
            }
        }

        /// <summary>Evaluator for attribute name != value matching</summary>
        public sealed class AttributeWithValueNot : Evaluator.AttributeKeyPair {
            public AttributeWithValueNot(String key, String value)
                : base(key, value) {
            }

            public override bool Matches(iText.StyledXmlParser.Jsoup.Nodes.Element root, iText.StyledXmlParser.Jsoup.Nodes.Element
                 element) {
                return !value.EqualsIgnoreCase(element.Attr(key));
            }

            public override String ToString() {
                return MessageFormatUtil.Format("[{0}!={1}]", key, value);
            }
        }

        /// <summary>Evaluator for attribute name/value matching (value prefix)</summary>
        public sealed class AttributeWithValueStarting : Evaluator.AttributeKeyPair {
            public AttributeWithValueStarting(String key, String value)
                : base(key, value) {
            }

            public override bool Matches(iText.StyledXmlParser.Jsoup.Nodes.Element root, iText.StyledXmlParser.Jsoup.Nodes.Element
                 element) {
                return element.HasAttr(key) && element.Attr(key).ToLowerInvariant().StartsWith(value);
            }

            // value is lower case already
            public override String ToString() {
                return MessageFormatUtil.Format("[{0}^={1}]", key, value);
            }
        }

        /// <summary>Evaluator for attribute name/value matching (value ending)</summary>
        public sealed class AttributeWithValueEnding : Evaluator.AttributeKeyPair {
            public AttributeWithValueEnding(String key, String value)
                : base(key, value) {
            }

            public override bool Matches(iText.StyledXmlParser.Jsoup.Nodes.Element root, iText.StyledXmlParser.Jsoup.Nodes.Element
                 element) {
                return element.HasAttr(key) && element.Attr(key).ToLowerInvariant().EndsWith(value);
            }

            // value is lower case
            public override String ToString() {
                return MessageFormatUtil.Format("[{0}$={1}]", key, value);
            }
        }

        /// <summary>Evaluator for attribute name/value matching (value containing)</summary>
        public sealed class AttributeWithValueContaining : Evaluator.AttributeKeyPair {
            public AttributeWithValueContaining(String key, String value)
                : base(key, value) {
            }

            public override bool Matches(iText.StyledXmlParser.Jsoup.Nodes.Element root, iText.StyledXmlParser.Jsoup.Nodes.Element
                 element) {
                return element.HasAttr(key) && element.Attr(key).ToLowerInvariant().Contains(value);
            }

            // value is lower case
            public override String ToString() {
                return MessageFormatUtil.Format("[{0}*={1}]", key, value);
            }
        }

        /// <summary>Evaluator for attribute name/value matching (value regex matching)</summary>
        public sealed class AttributeWithValueMatching : Evaluator {
            internal String key;

            internal Regex pattern;

            public AttributeWithValueMatching(String key, Regex pattern) {
                this.key = key.Trim().ToLowerInvariant();
                this.pattern = pattern;
            }

            public override bool Matches(iText.StyledXmlParser.Jsoup.Nodes.Element root, iText.StyledXmlParser.Jsoup.Nodes.Element
                 element) {
                return element.HasAttr(key) && PortUtil.HasMatch(pattern, element.Attr(key));
            }

            public override String ToString() {
                return MessageFormatUtil.Format("[{0}~={1}]", key, pattern.ToString());
            }
        }

        /// <summary>Abstract evaluator for attribute name/value matching</summary>
        public abstract class AttributeKeyPair : Evaluator {
            internal String key;

            internal String value;

            public AttributeKeyPair(String key, String value) {
                Validate.NotEmpty(key);
                Validate.NotEmpty(value);
                this.key = key.Trim().ToLowerInvariant();
                if (value.StartsWith("\"") && value.EndsWith("\"") || value.StartsWith("'") && value.EndsWith("'")) {
                    value = value.JSubstring(1, value.Length - 1);
                }
                this.value = value.Trim().ToLowerInvariant();
            }
        }

        /// <summary>Evaluator for any / all element matching</summary>
        public sealed class AllElements : Evaluator {
            public override bool Matches(iText.StyledXmlParser.Jsoup.Nodes.Element root, iText.StyledXmlParser.Jsoup.Nodes.Element
                 element) {
                return true;
            }

            public override String ToString() {
                return "*";
            }
        }

        /// <summary>
        /// Evaluator for matching by sibling index number (e
        /// <literal></literal>
        /// idx)
        /// </summary>
        public sealed class IndexLessThan : Evaluator.IndexEvaluator {
            public IndexLessThan(int index)
                : base(index) {
            }

            public override bool Matches(iText.StyledXmlParser.Jsoup.Nodes.Element root, iText.StyledXmlParser.Jsoup.Nodes.Element
                 element) {
                return element.ElementSiblingIndex() < index;
            }

            public override String ToString() {
                return MessageFormatUtil.Format(":lt({0})", index);
            }
        }

        /// <summary>
        /// Evaluator for matching by sibling index number (e
        /// <literal>&gt;</literal>
        /// idx)
        /// </summary>
        public sealed class IndexGreaterThan : Evaluator.IndexEvaluator {
            public IndexGreaterThan(int index)
                : base(index) {
            }

            public override bool Matches(iText.StyledXmlParser.Jsoup.Nodes.Element root, iText.StyledXmlParser.Jsoup.Nodes.Element
                 element) {
                return element.ElementSiblingIndex() > index;
            }

            public override String ToString() {
                return MessageFormatUtil.Format(":gt({0})", index);
            }
        }

        /// <summary>Evaluator for matching by sibling index number (e = idx)</summary>
        public sealed class IndexEquals : Evaluator.IndexEvaluator {
            public IndexEquals(int index)
                : base(index) {
            }

            public override bool Matches(iText.StyledXmlParser.Jsoup.Nodes.Element root, iText.StyledXmlParser.Jsoup.Nodes.Element
                 element) {
                return element.ElementSiblingIndex() == index;
            }

            public override String ToString() {
                return MessageFormatUtil.Format(":eq({0})", index);
            }
        }

        /// <summary>Evaluator for matching the last sibling (css :last-child)</summary>
        public sealed class IsLastChild : Evaluator {
            public override bool Matches(iText.StyledXmlParser.Jsoup.Nodes.Element root, iText.StyledXmlParser.Jsoup.Nodes.Element
                 element) {
                iText.StyledXmlParser.Jsoup.Nodes.Element p = (iText.StyledXmlParser.Jsoup.Nodes.Element)element.Parent();
                return p != null && !(p is Document) && element.ElementSiblingIndex() == p.Children().Count - 1;
            }

            public override String ToString() {
                return ":last-child";
            }
        }

        public sealed class IsFirstOfType : Evaluator.IsNthOfType {
            public IsFirstOfType()
                : base(0, 1) {
            }

            public override String ToString() {
                return ":first-of-type";
            }
        }

        public sealed class IsLastOfType : Evaluator.IsNthLastOfType {
            public IsLastOfType()
                : base(0, 1) {
            }

            public override String ToString() {
                return ":last-of-type";
            }
        }

        public abstract class CssNthEvaluator : Evaluator {
            protected internal readonly int a;

            protected internal readonly int b;

            public CssNthEvaluator(int a, int b) {
                this.a = a;
                this.b = b;
            }

            public CssNthEvaluator(int b)
                : this(0, b) {
            }

            public override bool Matches(iText.StyledXmlParser.Jsoup.Nodes.Element root, iText.StyledXmlParser.Jsoup.Nodes.Element
                 element) {
                iText.StyledXmlParser.Jsoup.Nodes.Element p = (iText.StyledXmlParser.Jsoup.Nodes.Element)element.Parent();
                if (p == null || (p is Document)) {
                    return false;
                }
                int pos = CalculatePosition(root, element);
                if (a == 0) {
                    return pos == b;
                }
                return (pos - b) * a >= 0 && (pos - b) % a == 0;
            }

            public override String ToString() {
                if (a == 0) {
                    return MessageFormatUtil.Format(":{0}({1})", GetPseudoClass(), b);
                }
                if (b == 0) {
                    return MessageFormatUtil.Format(":{0}({1}n)", GetPseudoClass(), a);
                }
                return MessageFormatUtil.Format(":{0}({1}n{2" + PortUtil.SignedNumberFormat + "})", GetPseudoClass(), a, b
                    );
            }

            protected internal abstract String GetPseudoClass();

            protected internal abstract int CalculatePosition(iText.StyledXmlParser.Jsoup.Nodes.Element root, iText.StyledXmlParser.Jsoup.Nodes.Element
                 element);
        }

        /// <summary>css-compatible Evaluator for :eq (css :nth-child)</summary>
        /// <seealso cref="IndexEquals"/>
        public sealed class IsNthChild : Evaluator.CssNthEvaluator {
            public IsNthChild(int a, int b)
                : base(a, b) {
            }

            protected internal override int CalculatePosition(iText.StyledXmlParser.Jsoup.Nodes.Element root, iText.StyledXmlParser.Jsoup.Nodes.Element
                 element) {
                return element.ElementSiblingIndex() + 1;
            }

            protected internal override String GetPseudoClass() {
                return "nth-child";
            }
        }

        /// <summary>css pseudo class :nth-last-child)</summary>
        /// <seealso cref="IndexEquals"/>
        public sealed class IsNthLastChild : Evaluator.CssNthEvaluator {
            public IsNthLastChild(int a, int b)
                : base(a, b) {
            }

            protected internal override int CalculatePosition(iText.StyledXmlParser.Jsoup.Nodes.Element root, iText.StyledXmlParser.Jsoup.Nodes.Element
                 element) {
                return ((iText.StyledXmlParser.Jsoup.Nodes.Element)element.Parent()).Children().Count - element.ElementSiblingIndex
                    ();
            }

            protected internal override String GetPseudoClass() {
                return "nth-last-child";
            }
        }

        /// <summary>css pseudo class nth-of-type</summary>
        public class IsNthOfType : Evaluator.CssNthEvaluator {
            public IsNthOfType(int a, int b)
                : base(a, b) {
            }

            protected internal override int CalculatePosition(iText.StyledXmlParser.Jsoup.Nodes.Element root, iText.StyledXmlParser.Jsoup.Nodes.Element
                 element) {
                int pos = 0;
                Elements family = ((iText.StyledXmlParser.Jsoup.Nodes.Element)element.Parent()).Children();
                foreach (iText.StyledXmlParser.Jsoup.Nodes.Element el in family) {
                    if (el.Tag().Equals(element.Tag())) {
                        pos++;
                    }
                    if (el == element) {
                        break;
                    }
                }
                return pos;
            }

            protected internal override String GetPseudoClass() {
                return "nth-of-type";
            }
        }

        public class IsNthLastOfType : Evaluator.CssNthEvaluator {
            public IsNthLastOfType(int a, int b)
                : base(a, b) {
            }

            protected internal override int CalculatePosition(iText.StyledXmlParser.Jsoup.Nodes.Element root, iText.StyledXmlParser.Jsoup.Nodes.Element
                 element) {
                int pos = 0;
                Elements family = ((iText.StyledXmlParser.Jsoup.Nodes.Element)element.Parent()).Children();
                for (int i = element.ElementSiblingIndex(); i < family.Count; i++) {
                    if (family[i].Tag().Equals(element.Tag())) {
                        pos++;
                    }
                }
                return pos;
            }

            protected internal override String GetPseudoClass() {
                return "nth-last-of-type";
            }
        }

        /// <summary>Evaluator for matching the first sibling (css :first-child)</summary>
        public sealed class IsFirstChild : Evaluator {
            public override bool Matches(iText.StyledXmlParser.Jsoup.Nodes.Element root, iText.StyledXmlParser.Jsoup.Nodes.Element
                 element) {
                iText.StyledXmlParser.Jsoup.Nodes.Element p = (iText.StyledXmlParser.Jsoup.Nodes.Element)element.Parent();
                return p != null && !(p is Document) && element.ElementSiblingIndex() == 0;
            }

            public override String ToString() {
                return ":first-child";
            }
        }

        /// <summary>css3 pseudo-class :root</summary>
        /// <seealso><a href="http://www.w3.org/tr/selectors/#root-pseudo">:root selector</a></seealso>
        public sealed class IsRoot : Evaluator {
            public override bool Matches(iText.StyledXmlParser.Jsoup.Nodes.Element root, iText.StyledXmlParser.Jsoup.Nodes.Element
                 element) {
                iText.StyledXmlParser.Jsoup.Nodes.Element r = root is Document ? root.Child(0) : root;
                return element == r;
            }

            public override String ToString() {
                return ":root";
            }
        }

        public sealed class IsOnlyChild : Evaluator {
            public override bool Matches(iText.StyledXmlParser.Jsoup.Nodes.Element root, iText.StyledXmlParser.Jsoup.Nodes.Element
                 element) {
                iText.StyledXmlParser.Jsoup.Nodes.Element p = (iText.StyledXmlParser.Jsoup.Nodes.Element)element.Parent();
                return p != null && !(p is Document) && element.SiblingElements().Count == 0;
            }

            public override String ToString() {
                return ":only-child";
            }
        }

        public sealed class IsOnlyOfType : Evaluator {
            public override bool Matches(iText.StyledXmlParser.Jsoup.Nodes.Element root, iText.StyledXmlParser.Jsoup.Nodes.Element
                 element) {
                iText.StyledXmlParser.Jsoup.Nodes.Element p = (iText.StyledXmlParser.Jsoup.Nodes.Element)element.Parent();
                if (p == null || p is Document) {
                    return false;
                }
                int pos = 0;
                Elements family = p.Children();
                foreach (iText.StyledXmlParser.Jsoup.Nodes.Element el in family) {
                    if (el.Tag().Equals(element.Tag())) {
                        pos++;
                    }
                }
                return pos == 1;
            }

            public override String ToString() {
                return ":only-of-type";
            }
        }

        public sealed class IsEmpty : Evaluator {
            public override bool Matches(iText.StyledXmlParser.Jsoup.Nodes.Element root, iText.StyledXmlParser.Jsoup.Nodes.Element
                 element) {
                IList<iText.StyledXmlParser.Jsoup.Nodes.Node> family = element.ChildNodes();
                foreach (iText.StyledXmlParser.Jsoup.Nodes.Node n in family) {
                    if (!(n is Comment || n is XmlDeclaration || n is DocumentType)) {
                        return false;
                    }
                }
                return true;
            }

            public override String ToString() {
                return ":empty";
            }
        }

        /// <summary>Abstract evaluator for sibling index matching</summary>
        /// <author>ant</author>
        public abstract class IndexEvaluator : Evaluator {
            internal int index;

            public IndexEvaluator(int index) {
                this.index = index;
            }
        }

        /// <summary>Evaluator for matching Element (and its descendants) text</summary>
        public sealed class ContainsText : Evaluator {
            private String searchText;

            public ContainsText(String searchText) {
                this.searchText = searchText.ToLowerInvariant();
            }

            public override bool Matches(iText.StyledXmlParser.Jsoup.Nodes.Element root, iText.StyledXmlParser.Jsoup.Nodes.Element
                 element) {
                return (element.Text().ToLowerInvariant().Contains(searchText));
            }

            public override String ToString() {
                return MessageFormatUtil.Format(":contains({0}", searchText);
            }
        }

        /// <summary>Evaluator for matching Element's own text</summary>
        public sealed class ContainsOwnText : Evaluator {
            private String searchText;

            public ContainsOwnText(String searchText) {
                this.searchText = searchText.ToLowerInvariant();
            }

            public override bool Matches(iText.StyledXmlParser.Jsoup.Nodes.Element root, iText.StyledXmlParser.Jsoup.Nodes.Element
                 element) {
                return (element.OwnText().ToLowerInvariant().Contains(searchText));
            }

            public override String ToString() {
                return MessageFormatUtil.Format(":containsOwn({0}", searchText);
            }
        }

        /// <summary>Evaluator for matching Element's own text with regex</summary>
        public sealed class MatchesOwn : Evaluator {
            private Regex pattern;

            public MatchesOwn(Regex pattern) {
                this.pattern = pattern;
            }

            public override bool Matches(iText.StyledXmlParser.Jsoup.Nodes.Element root, iText.StyledXmlParser.Jsoup.Nodes.Element
                 element) {
                return PortUtil.HasMatch(pattern, element.OwnText());
            }

            public override String ToString() {
                return MessageFormatUtil.Format(":matchesOwn({0}", pattern);
            }
        }
    }

    /// <summary>Evaluator for matching Element (and its descendants) text with regex</summary>
    public sealed class MatchesElement : Evaluator {
        private Regex pattern;

        public MatchesElement(Regex pattern) {
            this.pattern = pattern;
        }

        public override bool Matches(iText.StyledXmlParser.Jsoup.Nodes.Element root, iText.StyledXmlParser.Jsoup.Nodes.Element
             element) {
            return PortUtil.HasMatch(pattern, element.Text());
        }

        public override String ToString() {
            return MessageFormatUtil.Format(":matches({0}", pattern);
        }
    }
}
