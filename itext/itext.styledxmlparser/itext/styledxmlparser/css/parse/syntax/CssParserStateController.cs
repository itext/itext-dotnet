/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: Bruno Lowagie, Paulo Soares, et al.

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
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Commons.Utils;
using iText.StyledXmlParser.Css;
using iText.StyledXmlParser.Css.Parse;
using iText.StyledXmlParser.Css.Util;
using iText.StyledXmlParser.Resolver.Resource;

namespace iText.StyledXmlParser.Css.Parse.Syntax {
    /// <summary>State machine that will parse content into a style sheet.</summary>
    public sealed class CssParserStateController {
        /// <summary>The current state.</summary>
        private IParserState currentState;

        /// <summary>Indicates if the current rule is supported.</summary>
        private bool isCurrentRuleSupported = true;

        /// <summary>The previous active state (excluding comments).</summary>
        private IParserState previousActiveState;

        /// <summary>A buffer to store temporary results.</summary>
        private StringBuilder buffer = new StringBuilder();

        /// <summary>The current selector.</summary>
        private String currentSelector;

        /// <summary>The style sheet.</summary>
        private CssStyleSheet styleSheet;

        /// <summary>The nested At-rules.</summary>
        private Stack<CssNestedAtRule> nestedAtRules;

        /// <summary>The stored properties without selector.</summary>
        private Stack<IList<CssDeclaration>> storedPropertiesWithoutSelector;

        /// <summary>Set of the supported rules.</summary>
        private static readonly ICollection<String> SUPPORTED_RULES = JavaCollectionsUtil.UnmodifiableSet(new HashSet
            <String>(JavaUtil.ArraysAsList(CssRuleName.MEDIA, CssRuleName.PAGE, CssRuleName.TOP_LEFT_CORNER, CssRuleName
            .TOP_LEFT, CssRuleName.TOP_CENTER, CssRuleName.TOP_RIGHT, CssRuleName.TOP_RIGHT_CORNER, CssRuleName.BOTTOM_LEFT_CORNER
            , CssRuleName.BOTTOM_LEFT, CssRuleName.BOTTOM_CENTER, CssRuleName.BOTTOM_RIGHT, CssRuleName.BOTTOM_RIGHT_CORNER
            , CssRuleName.LEFT_TOP, CssRuleName.LEFT_MIDDLE, CssRuleName.LEFT_BOTTOM, CssRuleName.RIGHT_TOP, CssRuleName
            .RIGHT_MIDDLE, CssRuleName.RIGHT_BOTTOM, CssRuleName.FONT_FACE)));

        /// <summary>Set of conditional group rules.</summary>
        private static readonly ICollection<String> CONDITIONAL_GROUP_RULES = JavaCollectionsUtil.UnmodifiableSet(
            new HashSet<String>(JavaUtil.ArraysAsList(CssRuleName.MEDIA)));

        /// <summary>The comment start state.</summary>
        private readonly IParserState commentStartState;

        /// <summary>The commend end state.</summary>
        private readonly IParserState commendEndState;

        /// <summary>The commend inner state.</summary>
        private readonly IParserState commendInnerState;

        /// <summary>The unknown state.</summary>
        private readonly IParserState unknownState;

        /// <summary>The rule state.</summary>
        private readonly IParserState ruleState;

        /// <summary>The properties state.</summary>
        private readonly IParserState propertiesState;

        /// <summary>The conditional group at rule block state.</summary>
        private readonly IParserState conditionalGroupAtRuleBlockState;

        /// <summary>The At-rule block state.</summary>
        private readonly IParserState atRuleBlockState;

        /// <summary>The URI resolver.</summary>
        private UriResolver uriResolver;

        /// <summary>
        /// Creates a new
        /// <see cref="CssParserStateController"/>
        /// instance.
        /// </summary>
        public CssParserStateController()
            : this("") {
        }

        /// <summary>
        /// Creates a new
        /// <see cref="CssParserStateController"/>
        /// instance.
        /// </summary>
        /// <param name="baseUrl">the base URL</param>
        public CssParserStateController(String baseUrl) {
            if (baseUrl != null && baseUrl.Length > 0) {
                this.uriResolver = new UriResolver(baseUrl);
            }
            styleSheet = new CssStyleSheet();
            nestedAtRules = new Stack<CssNestedAtRule>();
            storedPropertiesWithoutSelector = new Stack<IList<CssDeclaration>>();
            commentStartState = new CommentStartState(this);
            commendEndState = new CommentEndState(this);
            commendInnerState = new CommentInnerState(this);
            unknownState = new UnknownState(this);
            ruleState = new RuleState(this);
            propertiesState = new BlockState(this);
            atRuleBlockState = new AtRuleBlockState(this);
            conditionalGroupAtRuleBlockState = new ConditionalGroupAtRuleBlockState(this);
            currentState = unknownState;
        }

        /// <summary>Process a character using the current state.</summary>
        /// <param name="ch">the character</param>
        public void Process(char ch) {
            currentState.Process(ch);
        }

        /// <summary>Gets the resulting style sheet.</summary>
        /// <returns>the resulting style sheet</returns>
        public CssStyleSheet GetParsingResult() {
            return styleSheet;
        }

        /// <summary>Appends a character to the buffer.</summary>
        /// <param name="ch">the character</param>
        internal void AppendToBuffer(char ch) {
            buffer.Append(ch);
        }

        /// <summary>Gets the contents of the buffer.</summary>
        /// <returns>the buffer contents</returns>
        internal String GetBufferContents() {
            return buffer.ToString();
        }

        /// <summary>Resets the buffer.</summary>
        internal void ResetBuffer() {
            buffer.Length = 0;
        }

        /// <summary>Enter the previous active state.</summary>
        internal void EnterPreviousActiveState() {
            SetState(previousActiveState);
        }

        /// <summary>Enter the comment start state.</summary>
        internal void EnterCommentStartState() {
            SaveActiveState();
            SetState(commentStartState);
        }

        /// <summary>Enter the comment end state.</summary>
        internal void EnterCommentEndState() {
            SetState(commendEndState);
        }

        /// <summary>Enter the comment inner state.</summary>
        internal void EnterCommentInnerState() {
            SetState(commendInnerState);
        }

        /// <summary>Enter the rule state.</summary>
        internal void EnterRuleState() {
            SetState(ruleState);
        }

        /// <summary>Enter the unknown state if nested blocks are finished.</summary>
        internal void EnterUnknownStateIfNestedBlocksFinished() {
            if (nestedAtRules.Count == 0) {
                SetState(unknownState);
            }
            else {
                EnterRuleStateBasedOnItsType();
            }
        }

        /// <summary>Enter the rule state, based on whether the current state is unsupported or conditional.</summary>
        internal void EnterRuleStateBasedOnItsType() {
            if (CurrentAtRuleIsConditionalGroupRule()) {
                EnterConditionalGroupAtRuleBlockState();
            }
            else {
                EnterAtRuleBlockState();
            }
        }

        /// <summary>Enter the unknown state.</summary>
        internal void EnterUnknownState() {
            SetState(unknownState);
        }

        /// <summary>Enter the At-rule block state.</summary>
        internal void EnterAtRuleBlockState() {
            SetState(atRuleBlockState);
        }

        /// <summary>Enter the conditional group At-rule block state.</summary>
        internal void EnterConditionalGroupAtRuleBlockState() {
            SetState(conditionalGroupAtRuleBlockState);
        }

        /// <summary>Enter the properties state.</summary>
        internal void EnterPropertiesState() {
            SetState(propertiesState);
        }

        /// <summary>Store the current selector.</summary>
        internal void StoreCurrentSelector() {
            currentSelector = buffer.ToString();
            buffer.Length = 0;
        }

        /// <summary>Store the current properties.</summary>
        internal void StoreCurrentProperties() {
            if (isCurrentRuleSupported) {
                ProcessProperties(currentSelector, buffer.ToString());
            }
            currentSelector = null;
            buffer.Length = 0;
        }

        /// <summary>Store the current properties without selector.</summary>
        internal void StoreCurrentPropertiesWithoutSelector() {
            if (isCurrentRuleSupported) {
                ProcessProperties(buffer.ToString());
            }
            buffer.Length = 0;
        }

        /// <summary>Store the semicolon At-rule.</summary>
        internal void StoreSemicolonAtRule() {
            if (isCurrentRuleSupported) {
                ProcessSemicolonAtRule(buffer.ToString());
            }
            buffer.Length = 0;
        }

        /// <summary>Finish the At-rule block.</summary>
        internal void FinishAtRuleBlock() {
            IList<CssDeclaration> storedProps = storedPropertiesWithoutSelector.Pop();
            CssNestedAtRule atRule = nestedAtRules.Pop();
            if (isCurrentRuleSupported) {
                ProcessFinishedAtRuleBlock(atRule);
                if (!storedProps.IsEmpty()) {
                    atRule.AddBodyCssDeclarations(storedProps);
                }
            }
            isCurrentRuleSupported = IsCurrentRuleSupported();
            buffer.Length = 0;
        }

        /// <summary>Push the block preceding At-rule.</summary>
        internal void PushBlockPrecedingAtRule() {
            nestedAtRules.Push(CssNestedAtRuleFactory.CreateNestedRule(buffer.ToString()));
            storedPropertiesWithoutSelector.Push(new List<CssDeclaration>());
            isCurrentRuleSupported = IsCurrentRuleSupported();
            buffer.Length = 0;
        }

        /// <summary>Save the active state.</summary>
        private void SaveActiveState() {
            previousActiveState = currentState;
        }

        /// <summary>Sets the current state.</summary>
        /// <param name="state">the new state</param>
        private void SetState(IParserState state) {
            currentState = state;
        }

        /// <summary>Processes the properties.</summary>
        /// <param name="selector">the selector</param>
        /// <param name="properties">the properties</param>
        private void ProcessProperties(String selector, String properties) {
            IList<CssRuleSet> ruleSets = CssRuleSetParser.ParseRuleSet(selector, properties);
            foreach (CssRuleSet ruleSet in ruleSets) {
                NormalizeDeclarationURIs(ruleSet.GetNormalDeclarations());
                NormalizeDeclarationURIs(ruleSet.GetImportantDeclarations());
            }
            foreach (CssRuleSet ruleSet in ruleSets) {
                if (nestedAtRules.Count == 0) {
                    styleSheet.AddStatement(ruleSet);
                }
                else {
                    nestedAtRules.Peek().AddStatementToBody(ruleSet);
                }
            }
        }

        /// <summary>Processes the properties.</summary>
        /// <param name="properties">the properties</param>
        private void ProcessProperties(String properties) {
            if (storedPropertiesWithoutSelector.Count > 0) {
                IList<CssDeclaration> cssDeclarations = CssRuleSetParser.ParsePropertyDeclarations(properties);
                NormalizeDeclarationURIs(cssDeclarations);
                storedPropertiesWithoutSelector.Peek().AddAll(cssDeclarations);
            }
        }

        /// <summary>Normalizes the declaration URIs.</summary>
        /// <param name="declarations">the declarations</param>
        private void NormalizeDeclarationURIs(IList<CssDeclaration> declarations) {
            // This is the case when css has no location and thus urls should not be resolved against base css location
            if (this.uriResolver == null) {
                return;
            }
            foreach (CssDeclaration declaration in declarations) {
                if (declaration.GetExpression().Contains("url(")) {
                    CssDeclarationValueTokenizer tokenizer = new CssDeclarationValueTokenizer(declaration.GetExpression());
                    CssDeclarationValueTokenizer.Token token;
                    StringBuilder normalizedDeclaration = new StringBuilder();
                    while ((token = tokenizer.GetNextValidToken()) != null) {
                        String strToAppend;
                        if (token.GetType() == CssDeclarationValueTokenizer.TokenType.FUNCTION && token.GetValue().StartsWith("url("
                            )) {
                            String url = token.GetValue().Trim();
                            url = url.JSubstring(4, url.Length - 1).Trim();
                            if (CssTypesValidationUtils.IsBase64Data(url)) {
                                strToAppend = token.GetValue().Trim();
                            }
                            else {
                                if (url.StartsWith("'") && url.EndsWith("'") || url.StartsWith("\"") && url.EndsWith("\"")) {
                                    url = url.JSubstring(1, url.Length - 1);
                                }
                                url = url.Trim();
                                String finalUrl = url;
                                try {
                                    finalUrl = uriResolver.ResolveAgainstBaseUri(url).ToExternalForm();
                                }
                                catch (UriFormatException) {
                                }
                                strToAppend = MessageFormatUtil.Format("url({0})", finalUrl);
                            }
                        }
                        else {
                            strToAppend = token.GetValue();
                        }
                        if (normalizedDeclaration.Length > 0) {
                            normalizedDeclaration.Append(' ');
                        }
                        normalizedDeclaration.Append(strToAppend);
                    }
                    declaration.SetExpression(normalizedDeclaration.ToString());
                }
            }
        }

        /// <summary>Processes the semicolon At-rule.</summary>
        /// <param name="ruleStr">the rule str</param>
        private void ProcessSemicolonAtRule(String ruleStr) {
            CssSemicolonAtRule atRule = new CssSemicolonAtRule(ruleStr);
            styleSheet.AddStatement(atRule);
        }

        /// <summary>Processes the finished At-rule block.</summary>
        /// <param name="atRule">the at rule</param>
        private void ProcessFinishedAtRuleBlock(CssNestedAtRule atRule) {
            if (nestedAtRules.Count != 0) {
                nestedAtRules.Peek().AddStatementToBody(atRule);
            }
            else {
                styleSheet.AddStatement(atRule);
            }
        }

        /// <summary>Checks if is current rule is supported.</summary>
        /// <returns>true, if the current rule is supported</returns>
        private bool IsCurrentRuleSupported() {
            bool isSupported = nestedAtRules.IsEmpty() || SUPPORTED_RULES.Contains(nestedAtRules.Peek().GetRuleName());
            if (!isSupported) {
                ITextLogManager.GetLogger(GetType()).LogError(MessageFormatUtil.Format(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant
                    .RULE_IS_NOT_SUPPORTED, nestedAtRules.Peek().GetRuleName()));
            }
            return isSupported;
        }

        /// <summary>Checks if the current At-rule is a conditional group rule (or if it's unsupported).</summary>
        /// <returns>true, if the current At-rule is unsupported or conditional</returns>
        private bool CurrentAtRuleIsConditionalGroupRule() {
            return !isCurrentRuleSupported || (nestedAtRules.Count > 0 && CONDITIONAL_GROUP_RULES.Contains(nestedAtRules
                .Peek().GetRuleName()));
        }
    }
}
