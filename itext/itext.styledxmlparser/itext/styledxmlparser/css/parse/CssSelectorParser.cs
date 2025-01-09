/*
This file is part of the iText (R) project.
Copyright (c) 1998-2025 Apryse Group NV
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
using iText.Commons.Utils;
using iText.StyledXmlParser.Css.Selector.Item;
using iText.StyledXmlParser.Css.Util;
using iText.StyledXmlParser.Exceptions;

namespace iText.StyledXmlParser.Css.Parse {
    /// <summary>Utilities class to parse a CSS selector.</summary>
    public sealed class CssSelectorParser {
        /// <summary>Set of legacy pseudo elements (first-line, first-letter, before, after).</summary>
        private static readonly ICollection<String> LEGACY_PSEUDO_ELEMENTS;

        static CssSelectorParser() {
            // HashSet is required in order to autoport correctly in .Net
            HashSet<String> tempSet = new HashSet<String>();
            tempSet.Add("first-line");
            tempSet.Add("first-letter");
            tempSet.Add("before");
            tempSet.Add("after");
            LEGACY_PSEUDO_ELEMENTS = JavaCollectionsUtil.UnmodifiableSet(tempSet);
        }

        /// <summary>The pattern string for selectors.</summary>
        private const String SELECTOR_PATTERN_STR = "(\\*)|([_a-zA-Z][\\w-]*)|(\\.[_a-zA-Z][\\w-]*)|(#[_a-z][\\w-]*)|(\\[[_a-zA-Z][\\w-]*(([~^$*|])?=((\"[^\"]+\")|([^\"]+)|('[^']+')|(\"\")|('')))?\\])|(::?[a-zA-Z-]*)|( )|(\\+)|(>)|(~)";

        /// <summary>The pattern for selectors.</summary>
        private static readonly Regex selectorPattern = iText.Commons.Utils.StringUtil.RegexCompile(SELECTOR_PATTERN_STR
            );

        /// <summary>
        /// Creates a new
        /// <see cref="CssSelectorParser"/>
        /// instance.
        /// </summary>
        private CssSelectorParser() {
        }

        /// <summary>Parses the selector items.</summary>
        /// <param name="selector">
        /// the selectors in the form of a
        /// <see cref="System.String"/>
        /// </param>
        /// <returns>
        /// the resulting list of
        /// <see cref="iText.StyledXmlParser.Css.Selector.Item.ICssSelectorItem"/>
        /// </returns>
        public static IList<ICssSelectorItem> ParseSelectorItems(String selector) {
            IList<ICssSelectorItem> selectorItems = new List<ICssSelectorItem>();
            Matcher match = iText.Commons.Utils.Matcher.Match(selectorPattern, selector);
            bool tagSelectorDescription = false;
            while (match.Find()) {
                String selectorItem = match.Group(0);
                char firstChar = selectorItem[0];
                switch (firstChar) {
                    case '#': {
                        selectorItems.Add(new CssIdSelectorItem(selectorItem.Substring(1)));
                        break;
                    }

                    case '.': {
                        selectorItems.Add(new CssClassSelectorItem(selectorItem.Substring(1)));
                        break;
                    }

                    case '[': {
                        selectorItems.Add(new CssAttributeSelectorItem(selectorItem));
                        break;
                    }

                    case ':': {
                        AppendPseudoSelector(selectorItems, selectorItem, match, selector);
                        break;
                    }

                    case ' ':
                    case '+':
                    case '>':
                    case '~': {
                        if (selectorItems.Count == 0) {
                            throw new ArgumentException(MessageFormatUtil.Format(StyledXmlParserExceptionMessage.INVALID_TOKEN_AT_THE_BEGINNING_OF_SELECTOR
                                , firstChar));
                        }
                        ICssSelectorItem lastItem = selectorItems[selectorItems.Count - 1];
                        CssSeparatorSelectorItem curItem = new CssSeparatorSelectorItem(firstChar);
                        if (lastItem is CssSeparatorSelectorItem) {
                            if (curItem.GetSeparator() == ' ') {
                                break;
                            }
                            else {
                                if (((CssSeparatorSelectorItem)lastItem).GetSeparator() == ' ') {
                                    selectorItems[selectorItems.Count - 1] = curItem;
                                }
                                else {
                                    throw new ArgumentException(MessageFormatUtil.Format("Invalid selector description. Two consequent characters occurred: {0}, {1}"
                                        , ((CssSeparatorSelectorItem)lastItem).GetSeparator(), curItem.GetSeparator()));
                                }
                            }
                        }
                        else {
                            selectorItems.Add(curItem);
                            tagSelectorDescription = false;
                        }
                        break;
                    }

                    default: {
                        //and case '*':
                        if (tagSelectorDescription) {
                            throw new InvalidOperationException("Invalid selector string");
                        }
                        tagSelectorDescription = true;
                        selectorItems.Add(new CssTagSelectorItem(selectorItem));
                        break;
                    }
                }
            }
            if (selectorItems.Count == 0) {
                throw new ArgumentException("Selector declaration is invalid");
            }
            return selectorItems;
        }

        /// <summary>Resolves a pseudo selector and appends it to list.</summary>
        /// <param name="selectorItems">list of items to which new selector will be added to</param>
        /// <param name="pseudoSelector">the pseudo selector</param>
        /// <param name="match">
        /// the corresponding
        /// <see cref="iText.Commons.Utils.Matcher"/>.
        /// </param>
        /// <param name="source">is the original source</param>
        private static void AppendPseudoSelector(IList<ICssSelectorItem> selectorItems, String pseudoSelector, Matcher
             match, String source) {
            pseudoSelector = pseudoSelector.ToLowerInvariant();
            pseudoSelector = HandleBracketsOfPseudoSelector(pseudoSelector, match, source);
            /*
            This :: notation is introduced by the current document in order to establish a discrimination between
            pseudo-classes and pseudo-elements.
            For compatibility with existing style sheets, user agents must also accept the previous one-colon
            notation for pseudo-elements introduced in CSS levels 1 and 2 (namely, :first-line, :first-letter, :before and :after).
            This compatibility is not allowed for the new pseudo-elements introduced in this specification.
            */
            if (pseudoSelector.StartsWith("::")) {
                selectorItems.Add(new CssPseudoElementSelectorItem(pseudoSelector.Substring(2)));
            }
            else {
                if (pseudoSelector.StartsWith(":") && LEGACY_PSEUDO_ELEMENTS.Contains(pseudoSelector.Substring(1))) {
                    selectorItems.Add(new CssPseudoElementSelectorItem(pseudoSelector.Substring(1)));
                }
                else {
                    ICssSelectorItem pseudoClassSelectorItem = CssPseudoClassSelectorItem.Create(pseudoSelector.Substring(1));
                    if (pseudoClassSelectorItem == null) {
                        throw new ArgumentException(MessageFormatUtil.Format(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant
                            .UNSUPPORTED_PSEUDO_CSS_SELECTOR, pseudoSelector));
                    }
                    selectorItems.Add(pseudoClassSelectorItem);
                }
            }
        }

        /// <summary>Resolves a pseudo selector if it contains brackets.</summary>
        /// <remarks>
        /// Resolves a pseudo selector if it contains brackets. Updates internal state of
        /// <see cref="iText.Commons.Utils.Matcher"/>
        /// if necessary.
        /// </remarks>
        /// <param name="pseudoSelector">the pseudo selector</param>
        /// <param name="match">
        /// the corresponding
        /// <see cref="iText.Commons.Utils.Matcher"/>.
        /// </param>
        /// <param name="source">is the original source</param>
        private static String HandleBracketsOfPseudoSelector(String pseudoSelector, Matcher match, String source) {
            int start = match.Start() + pseudoSelector.Length;
            if (start < source.Length && source[start] == '(') {
                int bracketDepth = 1;
                int curr = start + 1;
                while (bracketDepth > 0 && curr < source.Length) {
                    if (source[curr] == '(') {
                        ++bracketDepth;
                    }
                    else {
                        if (source[curr] == ')') {
                            --bracketDepth;
                        }
                        else {
                            if (source[curr] == '"' || source[curr] == '\'') {
                                curr = CssUtils.FindNextUnescapedChar(source, source[curr], curr + 1);
                            }
                        }
                    }
                    ++curr;
                }
                if (bracketDepth == 0) {
                    match.Region(curr, source.Length);
                    pseudoSelector += source.JSubstring(start, curr);
                }
            }
            return pseudoSelector;
        }
    }
}
