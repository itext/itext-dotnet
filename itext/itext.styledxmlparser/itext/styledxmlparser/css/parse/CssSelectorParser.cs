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
using System.Text;
using iText.Commons.Datastructures;
using iText.Commons.Utils;
using iText.StyledXmlParser.Css.Selector.Item;
using iText.StyledXmlParser.Exceptions;

namespace iText.StyledXmlParser.Css.Parse {
    /// <summary>Utility class for parsing CSS selectors.</summary>
    public sealed class CssSelectorParser {
        /// <summary>legacy pseudo-elements (first-line, first-letter, before, after).</summary>
        private static readonly IList<String> LEGACY_PSEUDO_ELEMENTS = JavaUtil.ArraysAsList("first-line", "first-letter"
            , "before", "after");

        /// <summary>
        /// Private constructor for the
        /// <c>CssSelectorParser</c>
        /// utility class.
        /// </summary>
        /// <remarks>
        /// Private constructor for the
        /// <c>CssSelectorParser</c>
        /// utility class.
        /// This constructor prevents instantiation of the class since it only provides
        /// static methods and is not meant to be instantiated.
        /// </remarks>
        private CssSelectorParser() {
        }

        // Utility class, no instances allowed.
        /// <summary>
        /// Parses the given CSS selector string into a list of
        /// <see cref="iText.StyledXmlParser.Css.Selector.Item.ICssSelectorItem"/>
        /// objects.
        /// </summary>
        /// <remarks>
        /// Parses the given CSS selector string into a list of
        /// <see cref="iText.StyledXmlParser.Css.Selector.Item.ICssSelectorItem"/>
        /// objects.
        /// This method processes the selector string character by character, handling state transitions
        /// and escape sequences to generate a structured representation of the selector components.
        /// </remarks>
        /// <param name="selector">the CSS selector string to be parsed</param>
        /// <returns>
        /// a list of
        /// <see cref="iText.StyledXmlParser.Css.Selector.Item.ICssSelectorItem"/>
        /// objects representing the components of the parsed selector
        /// </returns>
        public static IList<ICssSelectorItem> ParseSelectorItems(String selector) {
            IList<ICssSelectorItem> selectorItems = new List<ICssSelectorItem>();
            CssSelectorParser.State state = new CssSelectorParser.NoneState();
            for (int i = 0; i < selector.Length; ++i) {
                char c = selector[i];
                CssSelectorParser.State nextState;
                //process escape sequence if necessary
                bool isEscaped = false;
                if (c == '\\') {
                    Tuple2<int, char> escapedSeq = ProcessEscape(i, selector);
                    i = (int)escapedSeq.GetFirst();
                    c = (char)escapedSeq.GetSecond();
                    isEscaped = true;
                    //Only tags can start with an escaped character
                    nextState = TrySwitchToTagState(state);
                }
                else {
                    nextState = TrySwitchState(state, c);
                }
                if (state != nextState) {
                    state.Process(selectorItems);
                    state = nextState;
                }
                state.AddChar(c, isEscaped);
            }
            state.Process(selectorItems);
            return selectorItems;
        }

        /// <summary>Processes a possible escape sequence in the given source string, starting from the specified index.
        ///     </summary>
        /// <remarks>
        /// Processes a possible escape sequence in the given source string, starting from the specified index.
        /// This method extracts and decodes an escape sequence based on its hexadecimal representation
        /// or returns the character directly if no valid escape sequence is found.
        /// </remarks>
        /// <param name="start">the starting index in the source string to begin processing the escape sequence</param>
        /// <param name="source">the source string containing the potential escape sequence</param>
        /// <returns>
        /// a
        /// <see cref="iText.Commons.Datastructures.Tuple2{T1, T2}"/>
        /// object where the first element is the index after the processed escape sequence,
        /// and the second element is the decoded character or a replacement character ('\uFFFD') if the escape
        /// sequence is invalid
        /// </returns>
        private static Tuple2<int, char> ProcessEscape(int start, String source) {
            if (start + 1 >= source.Length) {
                return new Tuple2<int, char>(start, '\uFFFD');
            }
            StringBuilder pendingUnicodeSequence = new StringBuilder();
            int i = start + 1;
            for (; i < source.Length; ++i) {
                char c = source[i];
                if (IsHexDigit(c) && pendingUnicodeSequence.Length < 6) {
                    pendingUnicodeSequence.Append(c);
                }
                else {
                    break;
                }
            }
            if (pendingUnicodeSequence.Length == 0) {
                // Not a hex sequence, just an escaped character e.g. `\.`
                return new Tuple2<int, char>(i, source[start + 1]);
            }
            int lastConsumedIndex = i - 1;
            int codePoint = Convert.ToInt32(pendingUnicodeSequence.ToString(), 16);
            // Consume one whitespace character after the sequence if present
            // See CSS Syntax Module Level 3 4.3.7
            if (i < source.Length && iText.IO.Util.TextUtil.IsWhiteSpace(source[i])) {
                lastConsumedIndex = i;
            }
            if (JavaUtil.IsValidCodePoint(codePoint) && codePoint != 0) {
                return new Tuple2<int, char>(lastConsumedIndex, (char)codePoint);
            }
            else {
                return new Tuple2<int, char>(lastConsumedIndex, '\uFFFD');
            }
        }

        /// <summary>Determines if the provided character is a valid hexadecimal digit.</summary>
        /// <param name="c">the character to check</param>
        /// <returns>true if the character is a valid hexadecimal digit, otherwise false</returns>
        private static bool IsHexDigit(char c) {
            return (47 < c && c < 58) || (64 < c && c < 71) || (96 < c && c < 103);
        }

        /// <summary>
        /// Attempts to switch the current parser state to
        /// <see cref="TagState"/>
        /// based on the provided current state.
        /// </summary>
        /// <remarks>
        /// Attempts to switch the current parser state to
        /// <see cref="TagState"/>
        /// based on the provided current state.
        /// If the current state is an instance of
        /// <see cref="NoneState"/>
        /// or
        /// <see cref="SeparatorState"/>
        /// , a new
        /// <see cref="TagState"/>
        /// instance is created and returned. Otherwise, the original state is returned unchanged.
        /// </remarks>
        /// <param name="state">the current state of the parser</param>
        /// <returns>
        /// the new state, which may either remain the same or switch to
        /// <see cref="TagState"/>
        /// </returns>
        private static CssSelectorParser.State TrySwitchToTagState(CssSelectorParser.State state) {
            if (state is CssSelectorParser.NoneState || state is CssSelectorParser.SeparatorState) {
                return new CssSelectorParser.TagState();
            }
            return state;
        }

        /// <summary>Attempts to switch the current state based on the provided character.</summary>
        /// <remarks>
        /// Attempts to switch the current state based on the provided character.
        /// If the current state is not ready for a switch, the original state is returned.
        /// Determines connections between states.
        /// </remarks>
        /// <param name="state">the current state of the parser</param>
        /// <param name="c">the character that determines whether and to which state to switch</param>
        /// <returns>
        /// the new state after evaluating the character; if no switch is performed,
        /// the original state is returned
        /// </returns>
        private static CssSelectorParser.State TrySwitchState(CssSelectorParser.State state, char c) {
            if (!state.IsReadyForSwitch(c)) {
                return state;
            }
            switch (c) {
                case '.': {
                    return new CssSelectorParser.ClassState();
                }

                case '#': {
                    return new CssSelectorParser.IdState();
                }

                case ':': {
                    return new CssSelectorParser.PseudoState();
                }

                case '[': {
                    return new CssSelectorParser.AttributeState();
                }

                case ' ':
                case '+':
                case '>':
                case '~':
                case ',':
                case '|': {
                    return new CssSelectorParser.SeparatorState();
                }

                case '*': {
                    return new CssSelectorParser.TagState();
                }

                default: {
                    return state is CssSelectorParser.SeparatorState || state is CssSelectorParser.NoneState ? new CssSelectorParser.TagState
                        () : state;
                }
            }
        }

        /// <summary>Represents the state of a CSS selector parser.</summary>
        /// <remarks>
        /// Represents the state of a CSS selector parser. Each state defines a specific behavior for
        /// processing characters, transitioning between states, and modifying the selector items list.
        /// This interface is intended to be implemented by various state types, each accounting for
        /// different selector parsing behaviors such as handling tags, separators, or escape sequences.
        /// </remarks>
        private interface State {
            /// <summary>
            /// Determines whether the current state is ready to transition to another state
            /// based on the provided character.
            /// </summary>
            /// <param name="c">the character that is being evaluated to decide state transition</param>
            /// <returns>true if the state is ready to switch to a new state, false otherwise</returns>
            bool IsReadyForSwitch(char c);

            /// <summary>Adds a character to the current state's processing logic.</summary>
            /// <param name="c">the character to be added</param>
            /// <param name="isEscaped">true if the character is escaped, false otherwise</param>
            void AddChar(char c, bool isEscaped);

            /// <summary>Processes a list of CSS selector items.</summary>
            /// <remarks>
            /// Processes a list of CSS selector items. The implementation of this method
            /// performs operations specific to the current state of the CSS selector parser,
            /// which can include modifying, validating, or analyzing the provided selector items.
            /// </remarks>
            /// <param name="selectorItems">the list of CSS selector items to be processed or modified</param>
            void Process(IList<ICssSelectorItem> selectorItems);
        }

        /// <summary>
        /// Represents a "None" state in the
        /// <c>CssSelectorParser</c>.
        /// </summary>
        /// <remarks>
        /// Represents a "None" state in the
        /// <c>CssSelectorParser</c>
        /// . This state acts as a default or
        /// placeholder state where no specific processing or state changes occur. It adheres to the
        /// <c>State</c>
        /// interface but does not modify any selector items or perform any transitions.
        /// </remarks>
        private sealed class NoneState : CssSelectorParser.State {
            public bool IsReadyForSwitch(char c) {
                return true;
            }

            public void AddChar(char c, bool isEscaped) {
            }

            //Nothing to do here
            public void Process(IList<ICssSelectorItem> selectorItems) {
            }
            //Nothing to do here
        }

        /// <summary>Represents a state in the CSS selector parser responsible for processing tag selectors.</summary>
        /// <remarks>
        /// Represents a state in the CSS selector parser responsible for processing tag selectors.
        /// This state accumulates characters to define a tag name and, upon processing, creates
        /// a
        /// <see cref="iText.StyledXmlParser.Css.Selector.Item.CssTagSelectorItem"/>
        /// to add to the selector items list.
        /// <para />
        /// This is a private static inner class of the CSS selector parser and implements the
        /// <see cref="State"/>
        /// interface to define specific behaviors for handling tag selector components in a CSS string.
        /// </remarks>
        private sealed class TagState : CssSelectorParser.State {
            private readonly StringBuilder data = new StringBuilder();

            public bool IsReadyForSwitch(char c) {
                return true;
            }

            public void AddChar(char c, bool isEscaped) {
                data.Append(c);
            }

            public void Process(IList<ICssSelectorItem> selectorItems) {
                if (!selectorItems.IsEmpty() && !(selectorItems[selectorItems.Count - 1] is CssSeparatorSelectorItem)) {
                    throw new ArgumentException(MessageFormatUtil.Format(StyledXmlParserExceptionMessage.INVALID_SELECTOR_STRING
                        , data.ToString()));
                }
                selectorItems.Add(new CssTagSelectorItem(data.ToString()));
            }
        }

        /// <summary>Represents a state in the CSS selector parser responsible for processing combinators and separators.
        ///     </summary>
        /// <remarks>
        /// Represents a state in the CSS selector parser responsible for processing combinators and separators.
        /// It is designed to handle the state where a separator character (e.g., space, comma) is encountered
        /// during the parsing process. This state aids in managing validation, transitioning, and the placement
        /// of separators within the parsed CSS selectors.
        /// <para />
        /// An instance of SeparatorState implements the
        /// <c>State</c>
        /// interface and therefore defines the behavior
        /// for processing characters, validating transitions, and modifying the list of selector items.
        /// </remarks>
        private sealed class SeparatorState : CssSelectorParser.State {
            private char data = '\0';

            public bool IsReadyForSwitch(char c) {
                return true;
            }

            public void AddChar(char c, bool isEscaped) {
                if (data != '\0') {
                    throw new ArgumentException(MessageFormatUtil.Format(StyledXmlParserExceptionMessage.INVALID_SELECTOR_STRING
                        , "" + data + c));
                }
                data = c;
            }

            /// <summary>
            /// Processes a list of CSS selector items by validating and updating the state
            /// of separator characters (e.g., spaces, commas) within the selector list.
            /// </summary>
            /// <remarks>
            /// Processes a list of CSS selector items by validating and updating the state
            /// of separator characters (e.g., spaces, commas) within the selector list.
            /// <para />
            /// If the list is empty, an exception is thrown indicating an invalid initial state.
            /// Handles proper appending or replacement of separator items and ensures no
            /// invalid consecutive separators are added.
            /// </remarks>
            /// <param name="selectorItems">
            /// the list of
            /// <c>ICssSelectorItem</c>
            /// objects representing
            /// the current state of CSS selectors being parsed and processed.
            /// Must not be empty.
            /// </param>
            public void Process(IList<ICssSelectorItem> selectorItems) {
                if (selectorItems.IsEmpty()) {
                    throw new ArgumentException(MessageFormatUtil.Format(StyledXmlParserExceptionMessage.INVALID_SELECTOR_STRING
                        , data));
                }
                ICssSelectorItem lastItem = selectorItems[selectorItems.Count - 1];
                CssSeparatorSelectorItem curItem = new CssSeparatorSelectorItem(data);
                if (lastItem is CssSeparatorSelectorItem) {
                    if (((CssSeparatorSelectorItem)lastItem).GetSeparator() == ' ') {
                        selectorItems[selectorItems.Count - 1] = curItem;
                    }
                    else {
                        if (((CssSeparatorSelectorItem)lastItem).GetSeparator() != ' ' && data != ' ') {
                            throw new ArgumentException(MessageFormatUtil.Format(StyledXmlParserExceptionMessage.INVALID_SELECTOR_STRING
                                , "" + ((CssSeparatorSelectorItem)lastItem).GetSeparator() + curItem.GetSeparator()));
                        }
                    }
                }
                else {
                    selectorItems.Add(curItem);
                }
            }
        }

        /// <summary>Represents a state in the CSS selector parser responsible for processing class selectors.</summary>
        /// <remarks>
        /// Represents a state in the CSS selector parser responsible for processing class selectors.
        /// This state accumulates characters to define a class name and, upon processing, creates
        /// a
        /// <see cref="iText.StyledXmlParser.Css.Selector.Item.CssClassSelectorItem"/>
        /// to add to the selector items list.
        /// <para />
        /// This is a private static inner class of the CSS selector parser and implements the
        /// <see cref="State"/>
        /// interface to define specific behaviors for handling class selector components in a CSS string.
        /// </remarks>
        private sealed class ClassState : CssSelectorParser.State {
            private readonly StringBuilder data = new StringBuilder();

            public bool IsReadyForSwitch(char c) {
                return true;
            }

            public void AddChar(char c, bool isEscaped) {
                data.Append(c);
            }

            public void Process(IList<ICssSelectorItem> selectorItems) {
                selectorItems.Add(new CssClassSelectorItem(data.ToString().Substring(1)));
            }
        }

        /// <summary>Represents the state of the CSS selector parser for processing ID selectors.</summary>
        /// <remarks>
        /// Represents the state of the CSS selector parser for processing ID selectors.
        /// This state manages the collection of characters for an ID-based selector
        /// and processes it into a
        /// <see cref="iText.StyledXmlParser.Css.Selector.Item.CssIdSelectorItem"/>
        /// when the parsing is completed.
        /// </remarks>
        private sealed class IdState : CssSelectorParser.State {
            private readonly StringBuilder data = new StringBuilder();

            public bool IsReadyForSwitch(char c) {
                return true;
            }

            public void AddChar(char c, bool isEscaped) {
                data.Append(c);
            }

            public void Process(IList<ICssSelectorItem> selectorItems) {
                selectorItems.Add(new CssIdSelectorItem(data.ToString().Substring(1)));
            }
        }

        /// <summary>
        /// Represents an abstract state for managing the parsing and processing of CSS selector
        /// elements that involve function-like syntax.
        /// </summary>
        /// <remarks>
        /// Represents an abstract state for managing the parsing and processing of CSS selector
        /// elements that involve function-like syntax. It provides common functionality for
        /// handling character-based transitions such as detecting closures and string literals.
        /// This class is intended to be extended by other concrete state implementations that
        /// require specific behavior for selectors involving functions or brackets.
        /// </remarks>
        private abstract class FunctionState : CssSelectorParser.State {
            protected internal char closure;

            protected internal bool inString = false;

            protected internal bool isReadyForSwitch = false;

            /// <summary>Constructs a new FunctionState object with the specified closure character.</summary>
            /// <remarks>
            /// Constructs a new FunctionState object with the specified closure character.
            /// The closure character determines the character that signifies the termination
            /// of the function-like construct being processed by this state.
            /// </remarks>
            /// <param name="closure">the character that represents the closure for this state</param>
            public FunctionState(char closure) {
                this.closure = closure;
            }

            public virtual bool IsReadyForSwitch(char c) {
                return isReadyForSwitch;
            }

            /// <summary>Updates the internal flags based on the provided character and its escape status.</summary>
            /// <remarks>
            /// Updates the internal flags based on the provided character and its escape status.
            /// This method detects if the character is part of a string literal or
            /// if it marks the closure of the current function-like construct.
            /// </remarks>
            /// <param name="c">the character to be added</param>
            /// <param name="isEscaped">a boolean indicating whether the character is escaped</param>
            public virtual void AddChar(char c, bool isEscaped) {
                if ((c == '"' || c == '\'') && !isEscaped) {
                    inString = !inString;
                }
                if (c == closure && !isEscaped && !inString) {
                    isReadyForSwitch = true;
                }
            }

            public abstract void Process(IList<ICssSelectorItem> arg1);
        }

        /// <summary>
        /// Represents a state in the
        /// <see cref="CssSelectorParser"/>
        /// responsible for parsing attribute selectors.
        /// </summary>
        /// <remarks>
        /// Represents a state in the
        /// <see cref="CssSelectorParser"/>
        /// responsible for parsing attribute selectors.
        /// This class extends
        /// <see cref="FunctionState"/>
        /// and processes attribute selector data, constructing
        /// an
        /// <see cref="iText.StyledXmlParser.Css.Selector.Item.CssAttributeSelectorItem"/>
        /// instance to be added to the list of selector items.
        /// The state operates until a closing bracket (']') is encountered.
        /// </remarks>
        private sealed class AttributeState : CssSelectorParser.FunctionState {
            private readonly StringBuilder data = new StringBuilder();

            public AttributeState()
                : base(']') {
            }

            public override void Process(IList<ICssSelectorItem> selectorItems) {
                selectorItems.Add(new CssAttributeSelectorItem(data.ToString()));
            }

            public override void AddChar(char c, bool isEscaped) {
                base.AddChar(c, isEscaped);
                data.Append(c);
            }
        }

        /// <summary>
        /// Represents a state used in the CSS Selector parsing process for handling pseudo-classes
        /// and pseudo-elements.
        /// </summary>
        /// <remarks>
        /// Represents a state used in the CSS Selector parsing process for handling pseudo-classes
        /// and pseudo-elements. This state monitors character input to determine whether to transition
        /// to another state and processes pseudo-selectors, differentiating between legacy pseudo-elements,
        /// modern pseudo-elements, and pseudo-classes.
        /// </remarks>
        private sealed class PseudoState : CssSelectorParser.FunctionState {
            private readonly StringBuilder data = new StringBuilder();

            private bool isFunction = false;

            public PseudoState()
                : base(')') {
            }

            public override bool IsReadyForSwitch(char c) {
                return (!isFunction && data.Length > 1) || base.IsReadyForSwitch(c);
            }

            public override void AddChar(char c, bool isEscaped) {
                base.AddChar(c, isEscaped);
                c = isFunction ? c : char.ToLower(c);
                if (c == '(' && !isEscaped) {
                    isFunction = true;
                }
                data.Append(c);
            }

            /// <summary>Processes a list of CSS selector items to handle pseudo-elements and pseudo-classes.</summary>
            /// <remarks>
            /// Processes a list of CSS selector items to handle pseudo-elements and pseudo-classes.
            /// Differentiates between modern pseudo-elements (indicated by "::"), legacy pseudo-elements,
            /// and pseudo-classes. Adds the appropriate selector item to the provided list or throws
            /// an exception if the pseudo-class is unsupported.
            /// </remarks>
            /// <param name="selectorItems">
            /// the list of
            /// <see cref="iText.StyledXmlParser.Css.Selector.Item.ICssSelectorItem"/>
            /// to which processed pseudo-element
            /// or pseudo-class selectors will be added
            /// </param>
            public override void Process(IList<ICssSelectorItem> selectorItems) {
                String pseudoElement = data.ToString();
                if (pseudoElement.StartsWith("::")) {
                    selectorItems.Add(new CssPseudoElementSelectorItem(pseudoElement.Substring(2)));
                }
                else {
                    if (pseudoElement.StartsWith(":") && LEGACY_PSEUDO_ELEMENTS.Contains(pseudoElement.Substring(1))) {
                        selectorItems.Add(new CssPseudoElementSelectorItem(pseudoElement.Substring(1)));
                    }
                    else {
                        ICssSelectorItem pseudoClassSelectorItem = CssPseudoClassSelectorItem.Create(pseudoElement.Substring(1));
                        if (pseudoClassSelectorItem == null) {
                            throw new ArgumentException(MessageFormatUtil.Format(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant
                                .UNSUPPORTED_PSEUDO_CSS_SELECTOR, pseudoElement));
                        }
                        selectorItems.Add(pseudoClassSelectorItem);
                    }
                }
            }
        }
    }
}
