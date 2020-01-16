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
using iText.StyledXmlParser.Jsoup;
using iText.StyledXmlParser.Jsoup.Helper;
using iText.StyledXmlParser.Jsoup.Parser;

namespace iText.StyledXmlParser.Jsoup.Select {
    /// <summary>Parses a CSS selector into an Evaluator tree.</summary>
    public class QueryParser {
        private static readonly String[] combinators = new String[] { ",", ">", "+", "~", " " };

        private static readonly String[] AttributeEvals = new String[] { "=", "!=", "^=", "$=", "*=", "~=" };

        private TokenQueue tq;

        private String query;

        private IList<Evaluator> evals = new List<Evaluator>();

        /// <summary>Create a new QueryParser.</summary>
        /// <param name="query">CSS query</param>
        private QueryParser(String query) {
            this.query = query;
            this.tq = new TokenQueue(query);
        }

        /// <summary>Parse a CSS query into an Evaluator.</summary>
        /// <param name="query">CSS query</param>
        /// <returns>Evaluator</returns>
        public static Evaluator Parse(String query) {
            iText.StyledXmlParser.Jsoup.Select.QueryParser p = new iText.StyledXmlParser.Jsoup.Select.QueryParser(query
                );
            return p.Parse();
        }

        /// <summary>Parse the query</summary>
        /// <returns>Evaluator</returns>
        internal virtual Evaluator Parse() {
            tq.ConsumeWhitespace();
            if (tq.MatchesAny(combinators)) {
                // if starts with a combinator, use root as elements
                evals.Add(new StructuralEvaluator.Root());
                Combinator(tq.Consume());
            }
            else {
                FindElements();
            }
            while (!tq.IsEmpty()) {
                // hierarchy and extras
                bool seenWhite = tq.ConsumeWhitespace();
                if (tq.MatchesAny(combinators)) {
                    Combinator(tq.Consume());
                }
                else {
                    if (seenWhite) {
                        Combinator(' ');
                    }
                    else {
                        // E.class, E#id, E[attr] etc. AND
                        FindElements();
                    }
                }
            }
            // take next el, #. etc off queue
            if (evals.Count == 1) {
                return evals[0];
            }
            return new CombiningEvaluator.And(evals);
        }

        private void Combinator(char combinator) {
            tq.ConsumeWhitespace();
            String subQuery = ConsumeSubQuery();
            // support multi > childs
            Evaluator rootEval;
            // the new topmost evaluator
            Evaluator currentEval;
            // the evaluator the new eval will be combined to. could be root, or rightmost or.
            Evaluator newEval = Parse(subQuery);
            // the evaluator to add into target evaluator
            bool replaceRightMost = false;
            if (evals.Count == 1) {
                rootEval = currentEval = evals[0];
                // make sure OR (,) has precedence:
                if (rootEval is CombiningEvaluator.OR && combinator != ',') {
                    currentEval = ((CombiningEvaluator.OR)currentEval).RightMostEvaluator();
                    replaceRightMost = true;
                }
            }
            else {
                rootEval = currentEval = new CombiningEvaluator.And(evals);
            }
            evals.Clear();
            // for most combinators: change the current eval into an AND of the current eval and the new eval
            if (combinator == '>') {
                currentEval = new CombiningEvaluator.And(newEval, new StructuralEvaluator.ImmediateParent(currentEval));
            }
            else {
                if (combinator == ' ') {
                    currentEval = new CombiningEvaluator.And(newEval, new StructuralEvaluator.Parent(currentEval));
                }
                else {
                    if (combinator == '+') {
                        currentEval = new CombiningEvaluator.And(newEval, new StructuralEvaluator.ImmediatePreviousSibling(currentEval
                            ));
                    }
                    else {
                        if (combinator == '~') {
                            currentEval = new CombiningEvaluator.And(newEval, new StructuralEvaluator.PreviousSibling(currentEval));
                        }
                        else {
                            if (combinator == ',') {
                                // group or.
                                CombiningEvaluator.OR or;
                                if (currentEval is CombiningEvaluator.OR) {
                                    or = (CombiningEvaluator.OR)currentEval;
                                    or.Add(newEval);
                                }
                                else {
                                    or = new CombiningEvaluator.OR();
                                    or.Add(currentEval);
                                    or.Add(newEval);
                                }
                                currentEval = or;
                            }
                            else {
                                throw new Selector.SelectorParseException("Unknown combinator: " + combinator);
                            }
                        }
                    }
                }
            }
            if (replaceRightMost) {
                ((CombiningEvaluator.OR)rootEval).ReplaceRightMostEvaluator(currentEval);
            }
            else {
                rootEval = currentEval;
            }
            evals.Add(rootEval);
        }

        private String ConsumeSubQuery() {
            StringBuilder sq = new StringBuilder();
            while (!tq.IsEmpty()) {
                if (tq.Matches("(")) {
                    sq.Append("(").Append(tq.ChompBalanced('(', ')')).Append(")");
                }
                else {
                    if (tq.Matches("[")) {
                        sq.Append("[").Append(tq.ChompBalanced('[', ']')).Append("]");
                    }
                    else {
                        if (tq.MatchesAny(combinators)) {
                            break;
                        }
                        else {
                            sq.Append(tq.Consume());
                        }
                    }
                }
            }
            return sq.ToString();
        }

        private void FindElements() {
            if (tq.MatchChomp("#")) {
                ById();
            }
            else {
                if (tq.MatchChomp(".")) {
                    ByClass();
                }
                else {
                    if (tq.MatchesWord()) {
                        ByTag();
                    }
                    else {
                        if (tq.Matches("[")) {
                            ByAttribute();
                        }
                        else {
                            if (tq.MatchChomp("*")) {
                                AllElements();
                            }
                            else {
                                if (tq.MatchChomp(":lt(")) {
                                    IndexLessThan();
                                }
                                else {
                                    if (tq.MatchChomp(":gt(")) {
                                        IndexGreaterThan();
                                    }
                                    else {
                                        if (tq.MatchChomp(":eq(")) {
                                            IndexEquals();
                                        }
                                        else {
                                            if (tq.Matches(":has(")) {
                                                Has();
                                            }
                                            else {
                                                if (tq.Matches(":contains(")) {
                                                    Contains(false);
                                                }
                                                else {
                                                    if (tq.Matches(":containsOwn(")) {
                                                        Contains(true);
                                                    }
                                                    else {
                                                        if (tq.Matches(":matches(")) {
                                                            Matches(false);
                                                        }
                                                        else {
                                                            if (tq.Matches(":matchesOwn(")) {
                                                                Matches(true);
                                                            }
                                                            else {
                                                                if (tq.Matches(":not(")) {
                                                                    Not();
                                                                }
                                                                else {
                                                                    if (tq.MatchChomp(":nth-child(")) {
                                                                        CssNthChild(false, false);
                                                                    }
                                                                    else {
                                                                        if (tq.MatchChomp(":nth-last-child(")) {
                                                                            CssNthChild(true, false);
                                                                        }
                                                                        else {
                                                                            if (tq.MatchChomp(":nth-of-type(")) {
                                                                                CssNthChild(false, true);
                                                                            }
                                                                            else {
                                                                                if (tq.MatchChomp(":nth-last-of-type(")) {
                                                                                    CssNthChild(true, true);
                                                                                }
                                                                                else {
                                                                                    if (tq.MatchChomp(":first-child")) {
                                                                                        evals.Add(new Evaluator.IsFirstChild());
                                                                                    }
                                                                                    else {
                                                                                        if (tq.MatchChomp(":last-child")) {
                                                                                            evals.Add(new Evaluator.IsLastChild());
                                                                                        }
                                                                                        else {
                                                                                            if (tq.MatchChomp(":first-of-type")) {
                                                                                                evals.Add(new Evaluator.IsFirstOfType());
                                                                                            }
                                                                                            else {
                                                                                                if (tq.MatchChomp(":last-of-type")) {
                                                                                                    evals.Add(new Evaluator.IsLastOfType());
                                                                                                }
                                                                                                else {
                                                                                                    if (tq.MatchChomp(":only-child")) {
                                                                                                        evals.Add(new Evaluator.IsOnlyChild());
                                                                                                    }
                                                                                                    else {
                                                                                                        if (tq.MatchChomp(":only-of-type")) {
                                                                                                            evals.Add(new Evaluator.IsOnlyOfType());
                                                                                                        }
                                                                                                        else {
                                                                                                            if (tq.MatchChomp(":empty")) {
                                                                                                                evals.Add(new Evaluator.IsEmpty());
                                                                                                            }
                                                                                                            else {
                                                                                                                if (tq.MatchChomp(":root")) {
                                                                                                                    evals.Add(new Evaluator.IsRoot());
                                                                                                                }
                                                                                                                else {
                                                                                                                    // unhandled
                                                                                                                    throw new Selector.SelectorParseException("Could not parse query " + PortUtil.EscapedSingleBracket + "{0}"
                                                                                                                         + PortUtil.EscapedSingleBracket + ": unexpected token at " + PortUtil.EscapedSingleBracket + "{1}" + 
                                                                                                                        PortUtil.EscapedSingleBracket, query, tq.Remainder());
                                                                                                                }
                                                                                                            }
                                                                                                        }
                                                                                                    }
                                                                                                }
                                                                                            }
                                                                                        }
                                                                                    }
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void ById() {
            String id = tq.ConsumeCssIdentifier();
            Validate.NotEmpty(id);
            evals.Add(new Evaluator.ID(id));
        }

        private void ByClass() {
            String className = tq.ConsumeCssIdentifier();
            Validate.NotEmpty(className);
            evals.Add(new Evaluator.Class(className.Trim().ToLowerInvariant()));
        }

        private void ByTag() {
            String tagName = tq.ConsumeElementSelector();
            Validate.NotEmpty(tagName);
            // namespaces: if element name is "abc:def", selector must be "abc|def", so flip:
            if (tagName.Contains("|")) {
                tagName = tagName.Replace("|", ":");
            }
            evals.Add(new Evaluator.Tag(tagName.Trim().ToLowerInvariant()));
        }

        private void ByAttribute() {
            TokenQueue cq = new TokenQueue(tq.ChompBalanced('[', ']'));
            // content queue
            String key = cq.ConsumeToAny(AttributeEvals);
            // eq, not, start, end, contain, match, (no val)
            Validate.NotEmpty(key);
            cq.ConsumeWhitespace();
            if (cq.IsEmpty()) {
                if (key.StartsWith("^")) {
                    evals.Add(new Evaluator.AttributeStarting(key.Substring(1)));
                }
                else {
                    evals.Add(new Evaluator.Attribute(key));
                }
            }
            else {
                if (cq.MatchChomp("=")) {
                    evals.Add(new Evaluator.AttributeWithValue(key, cq.Remainder()));
                }
                else {
                    if (cq.MatchChomp("!=")) {
                        evals.Add(new Evaluator.AttributeWithValueNot(key, cq.Remainder()));
                    }
                    else {
                        if (cq.MatchChomp("^=")) {
                            evals.Add(new Evaluator.AttributeWithValueStarting(key, cq.Remainder()));
                        }
                        else {
                            if (cq.MatchChomp("$=")) {
                                evals.Add(new Evaluator.AttributeWithValueEnding(key, cq.Remainder()));
                            }
                            else {
                                if (cq.MatchChomp("*=")) {
                                    evals.Add(new Evaluator.AttributeWithValueContaining(key, cq.Remainder()));
                                }
                                else {
                                    if (cq.MatchChomp("~=")) {
                                        evals.Add(new Evaluator.AttributeWithValueMatching(key, iText.IO.Util.StringUtil.RegexCompile(cq.Remainder
                                            ())));
                                    }
                                    else {
                                        throw new Selector.SelectorParseException("Could not parse attribute query " + PortUtil.EscapedSingleBracket
                                             + "{0}" + PortUtil.EscapedSingleBracket + ": unexpected token at " + PortUtil.EscapedSingleBracket + 
                                            "{1}" + PortUtil.EscapedSingleBracket, query, cq.Remainder());
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void AllElements() {
            evals.Add(new Evaluator.AllElements());
        }

        // pseudo selectors :lt, :gt, :eq
        private void IndexLessThan() {
            evals.Add(new Evaluator.IndexLessThan(ConsumeIndex()));
        }

        private void IndexGreaterThan() {
            evals.Add(new Evaluator.IndexGreaterThan(ConsumeIndex()));
        }

        private void IndexEquals() {
            evals.Add(new Evaluator.IndexEquals(ConsumeIndex()));
        }

        //pseudo selectors :first-child, :last-child, :nth-child, ...
        private static readonly Regex NTH_AB = iText.IO.Util.StringUtil.RegexCompile("((\\+|-)?(\\d+)?)n(\\s*(\\+|-)?\\s*\\d+)?"
            , System.Text.RegularExpressions.RegexOptions.IgnoreCase);

        private static readonly Regex NTH_B = iText.IO.Util.StringUtil.RegexCompile("(\\+|-)?(\\d+)");

        private void CssNthChild(bool backwards, bool ofType) {
            String argS = tq.ChompTo(")").Trim().ToLowerInvariant();
            Match mAB = iText.IO.Util.StringUtil.Match(NTH_AB, argS);
            Match mB = iText.IO.Util.StringUtil.Match(NTH_B, argS);
            int a;
            int b;
            if ("odd".Equals(argS)) {
                a = 2;
                b = 1;
            }
            else {
                if ("even".Equals(argS)) {
                    a = 2;
                    b = 0;
                }
                else {
                    if (mAB.Success) {
                        a = iText.IO.Util.StringUtil.Group(mAB, 3) != null ? Convert.ToInt32(iText.IO.Util.StringUtil.Group(mAB, 1
                            ).ReplaceFirst("^\\+", "")) : 1;
                        b = iText.IO.Util.StringUtil.Group(mAB, 4) != null ? Convert.ToInt32(iText.IO.Util.StringUtil.Group(mAB, 4
                            ).ReplaceFirst("^\\+", "")) : 0;
                    }
                    else {
                        if (mB.Success) {
                            a = 0;
                            b = Convert.ToInt32(iText.IO.Util.StringUtil.Group(mB).ReplaceFirst("^\\+", ""));
                        }
                        else {
                            throw new Selector.SelectorParseException("Could not parse nth-index " + PortUtil.EscapedSingleBracket + "{0}"
                                 + PortUtil.EscapedSingleBracket + ": unexpected format", argS);
                        }
                    }
                }
            }
            if (ofType) {
                if (backwards) {
                    evals.Add(new Evaluator.IsNthLastOfType(a, b));
                }
                else {
                    evals.Add(new Evaluator.IsNthOfType(a, b));
                }
            }
            else {
                if (backwards) {
                    evals.Add(new Evaluator.IsNthLastChild(a, b));
                }
                else {
                    evals.Add(new Evaluator.IsNthChild(a, b));
                }
            }
        }

        private int ConsumeIndex() {
            String indexS = tq.ChompTo(")").Trim();
            Validate.IsTrue(iText.StyledXmlParser.Jsoup.Helper.StringUtil.IsNumeric(indexS), "Index must be numeric");
            return Convert.ToInt32(indexS);
        }

        // pseudo selector :has(el)
        private void Has() {
            tq.Consume(":has");
            String subQuery = tq.ChompBalanced('(', ')');
            Validate.NotEmpty(subQuery, ":has(el) subselect must not be empty");
            evals.Add(new StructuralEvaluator.Has(Parse(subQuery)));
        }

        // pseudo selector :contains(text), containsOwn(text)
        private void Contains(bool own) {
            tq.Consume(own ? ":containsOwn" : ":contains");
            String searchText = TokenQueue.Unescape(tq.ChompBalanced('(', ')'));
            Validate.NotEmpty(searchText, ":contains(text) query must not be empty");
            if (own) {
                evals.Add(new Evaluator.ContainsOwnText(searchText));
            }
            else {
                evals.Add(new Evaluator.ContainsText(searchText));
            }
        }

        // :matches(regex), matchesOwn(regex)
        private void Matches(bool own) {
            tq.Consume(own ? ":matchesOwn" : ":matches");
            String regex = tq.ChompBalanced('(', ')');
            // don't unescape, as regex bits will be escaped
            Validate.NotEmpty(regex, ":matches(regex) query must not be empty");
            if (own) {
                evals.Add(new Evaluator.MatchesOwn(iText.IO.Util.StringUtil.RegexCompile(regex)));
            }
            else {
                evals.Add(new MatchesElement(iText.IO.Util.StringUtil.RegexCompile(regex)));
            }
        }

        // :not(selector)
        private void Not() {
            tq.Consume(":not");
            String subQuery = tq.ChompBalanced('(', ')');
            Validate.NotEmpty(subQuery, ":not(selector) subselect must not be empty");
            evals.Add(new StructuralEvaluator.Not(Parse(subQuery)));
        }
    }
}
