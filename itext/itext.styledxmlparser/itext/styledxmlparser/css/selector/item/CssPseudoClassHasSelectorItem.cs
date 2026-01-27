/*
This file is part of the iText (R) project.
Copyright (c) 1998-2026 Apryse Group NV
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
using iText.StyledXmlParser.Css;
using iText.StyledXmlParser.Css.Selector;
using iText.StyledXmlParser.Node;

namespace iText.StyledXmlParser.Css.Selector.Item {
//\cond DO_NOT_DOCUMENT
    /// <summary>A class representing a CSS pseudo-class selector item for the ":has()" CSS pseudo-class.</summary>
    /// <remarks>
    /// A class representing a CSS pseudo-class selector item for the ":has()" CSS pseudo-class.
    /// This class is responsible for evaluating whether a given node matches the conditions
    /// defined in the ":has()" selector.
    /// <para />
    /// The primary responsibility of this class is handling the logic for matching nodes based on
    /// argument selectors and compiled versions of those selectors. The ":has()" pseudo-class takes
    /// other selectors as its arguments, and it matches elements that have descendants or other
    /// relative elements matching the provided criteria.
    /// </remarks>
    internal class CssPseudoClassHasSelectorItem : CssPseudoClassSelectorItem {
        private readonly IList<CssPseudoClassHasSelectorItem.CompiledHasArgument> compiledArguments;

//\cond DO_NOT_DOCUMENT
        internal CssPseudoClassHasSelectorItem(IList<ICssSelector> argumentSelectors, String argumentsString)
            : base(CommonCssConstants.HAS, argumentsString) {
            this.compiledArguments = CompileArguments(argumentSelectors);
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal CssPseudoClassHasSelectorItem(ICssSelector argumentsSelector)
            : this(JavaCollectionsUtil.SingletonList(argumentsSelector), argumentsSelector.ToString()) {
        }
//\endcond

        public override int GetSpecificity() {
            int maxSpecificity = 0;
            foreach (CssPseudoClassHasSelectorItem.CompiledHasArgument compiled in compiledArguments) {
                if (compiled.GetSelector() != null) {
                    maxSpecificity = Math.Max(maxSpecificity, compiled.GetSelector().CalculateSpecificity());
                }
            }
            return maxSpecificity;
        }

        public override bool Matches(INode node) {
            if (!CssSelectorItemTraversalUtil.IsValidElementNode(node)) {
                return false;
            }
            foreach (CssPseudoClassHasSelectorItem.CompiledHasArgument compiledArgument in compiledArguments) {
                if (MatchesCompiled(node, compiledArgument)) {
                    return true;
                }
            }
            return false;
        }

        private static bool MatchesCompiled(INode node, CssPseudoClassHasSelectorItem.CompiledHasArgument compiled
            ) {
            return compiled.GetRelativeSteps() != null ? MatchesRelativeSelector(node, compiled.GetRelativeSteps()) : 
                HasDescendantMatching(node, compiled.GetSelector());
        }

        private static IList<CssPseudoClassHasSelectorItem.CompiledHasArgument> CompileArguments(IList<ICssSelector
            > argumentSelectors) {
            IList<CssPseudoClassHasSelectorItem.CompiledHasArgument> result = new List<CssPseudoClassHasSelectorItem.CompiledHasArgument
                >(argumentSelectors.Count);
            foreach (ICssSelector selector in argumentSelectors) {
                CssPseudoClassHasSelectorItem.CompiledHasArgument compiled;
                if (!(selector is CssSelector)) {
                    compiled = new CssPseudoClassHasSelectorItem.CompiledHasArgument(selector, null);
                }
                else {
                    CssSelector cssSelector = (CssSelector)selector;
                    IList<ICssSelectorItem> items = cssSelector.GetSelectorItems();
                    if (items.IsEmpty()) {
                        compiled = new CssPseudoClassHasSelectorItem.CompiledHasArgument(null, null);
                    }
                    else {
                        if (items[0] is CssSeparatorSelectorItem) {
                            IList<CssPseudoClassHasSelectorItem.RelativeStep> steps = CompileRelativeSteps(items);
                            compiled = new CssPseudoClassHasSelectorItem.CompiledHasArgument(cssSelector, steps);
                        }
                        else {
                            compiled = new CssPseudoClassHasSelectorItem.CompiledHasArgument(cssSelector, null);
                        }
                    }
                }
                result.Add(compiled);
            }
            return result;
        }

        private static IList<CssPseudoClassHasSelectorItem.RelativeStep> CompileRelativeSteps(IList<ICssSelectorItem
            > relativeItems) {
            int i = 0;
            IList<CssPseudoClassHasSelectorItem.RelativeStep> steps = new List<CssPseudoClassHasSelectorItem.RelativeStep
                >();
            while (i < relativeItems.Count) {
                ICssSelectorItem item = relativeItems[i];
                if (!(item is CssSeparatorSelectorItem)) {
                    return JavaCollectionsUtil.EmptyList<CssPseudoClassHasSelectorItem.RelativeStep>();
                }
                i++;
                int seqStart = i;
                while (i < relativeItems.Count && !(relativeItems[i] is CssSeparatorSelectorItem)) {
                    i++;
                }
                if (seqStart == i) {
                    return JavaCollectionsUtil.EmptyList<CssPseudoClassHasSelectorItem.RelativeStep>();
                }
                CssSelector sequenceSelector = new CssSelector(relativeItems.SubList(seqStart, i));
                steps.Add(new CssPseudoClassHasSelectorItem.RelativeStep(((CssSeparatorSelectorItem)item).GetSeparator(), 
                    sequenceSelector));
            }
            return steps;
        }

        private static bool MatchesRelativeSelector(INode scope, IList<CssPseudoClassHasSelectorItem.RelativeStep>
             steps) {
            if (steps.IsEmpty()) {
                return false;
            }
            IList<INode> currentScopes = JavaCollectionsUtil.SingletonList(scope);
            foreach (CssPseudoClassHasSelectorItem.RelativeStep step in steps) {
                IList<INode> nextScopes = new List<INode>();
                foreach (INode currentScope in currentScopes) {
                    FillNextScopesByCombinator(currentScope, step.GetCombinator(), step.GetSelector(), nextScopes);
                }
                if (nextScopes.IsEmpty()) {
                    return false;
                }
                currentScopes = nextScopes;
            }
            return true;
        }

        private static void FillNextScopesByCombinator(INode scope, char combinator, CssSelector sequenceSelector, 
            IList<INode> nextScopes) {
            switch (combinator) {
                case '>': {
                    foreach (INode child in scope.ChildNodes()) {
                        if (child is IElementNode && sequenceSelector.Matches(child)) {
                            nextScopes.Add(child);
                        }
                    }
                    return;
                }

                case ' ': {
                    CssSelectorItemTraversalUtil.ForEachDescendantElement(scope, (candidate) => {
                        if (sequenceSelector.Matches(candidate)) {
                            nextScopes.Add(candidate);
                        }
                    }
                    );
                    return;
                }

                case '+': {
                    INode next = CssSelectorItemTraversalUtil.GetNextElementSibling(scope);
                    if (next != null && sequenceSelector.Matches(next)) {
                        nextScopes.Add(next);
                    }
                    return;
                }

                case '~': {
                    CssSelectorItemTraversalUtil.ForEachFollowingElementSibling(scope, (sibling) => {
                        if (sequenceSelector.Matches(sibling)) {
                            nextScopes.Add(sibling);
                        }
                    }
                    );
                    return;
                }

                default: {
                    break;
                }
            }
        }

        private static bool HasDescendantMatching(INode scope, ICssSelector selector) {
            if (selector == null) {
                return false;
            }
            if (selector is CssSelector) {
                CssSelector cssSelector = (CssSelector)selector;
                return CssSelectorItemTraversalUtil.AnyDescendantElementMatches(scope, (candidate) => cssSelector.MatchesWithinScope
                    (candidate, scope));
            }
            return CssSelectorItemTraversalUtil.AnyDescendantElementMatches(scope, (candidate) => selector.Matches(candidate
                ));
        }

        private sealed class CompiledHasArgument {
            private readonly ICssSelector selector;

            private readonly IList<CssPseudoClassHasSelectorItem.RelativeStep> relativeSteps;

            public CompiledHasArgument(ICssSelector selector, IList<CssPseudoClassHasSelectorItem.RelativeStep> relativeSteps
                ) {
                this.selector = selector;
                this.relativeSteps = relativeSteps;
            }

            public ICssSelector GetSelector() {
                return selector;
            }

            public IList<CssPseudoClassHasSelectorItem.RelativeStep> GetRelativeSteps() {
                return relativeSteps;
            }
        }

        private sealed class RelativeStep {
            private readonly char combinator;

            private readonly CssSelector selector;

            public RelativeStep(char combinator, CssSelector selector) {
                this.combinator = combinator;
                this.selector = selector;
            }

            public char GetCombinator() {
                return combinator;
            }

            public CssSelector GetSelector() {
                return selector;
            }
        }
    }
//\endcond
}
