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
using System.IO;
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
        private static readonly ILogger LOGGER = ITextLogManager.GetLogger(typeof(iText.StyledXmlParser.Css.Parse.Syntax.CssParserStateController
            ));

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

        /// <summary>The current state.</summary>
        private IParserState currentState;

        /// <summary>Indicates if the current rule is supported.</summary>
        private bool isCurrentRuleSupported = true;

        /// <summary>The previous active state (excluding comments).</summary>
        private IParserState previousActiveState;

        /// <summary>A buffer to store temporary results.</summary>
        private readonly StringBuilder buffer = new StringBuilder();

        /// <summary>The current selector.</summary>
        private String currentSelector;

        /// <summary>The style sheet.</summary>
        private readonly CssStyleSheet styleSheet;

        /// <summary>The style sheet from import CSS rules.</summary>
        /// <remarks>
        /// The style sheet from import CSS rules. It is used to store styles from import
        /// separately to avoid
        /// <see cref="iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.IMPORT_MUST_COME_BEFORE"/>
        /// on check whether were styles before import or not.
        /// </remarks>
        private readonly CssStyleSheet styleSheetFromImport;

        /// <summary>The nested At-rules.</summary>
        private readonly Stack<CssNestedAtRule> nestedAtRules;

        /// <summary>The stored properties without selector.</summary>
        private readonly Stack<IList<CssDeclaration>> storedPropertiesWithoutSelector;

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

        /// <summary>The resource resolver.</summary>
        private readonly ResourceResolver resourceResolver;

        /// <summary>
        /// Creates a new
        /// <see cref="CssParserStateController"/>
        /// instance.
        /// </summary>
        [System.ObsoleteAttribute(@"use CssParserStateController(System.String) constructor")]
        public CssParserStateController()
            : this("") {
        }

        /// <summary>
        /// Creates a new
        /// <see cref="CssParserStateController"/>
        /// instance.
        /// </summary>
        /// <param name="baseUrl">the base URL</param>
        public CssParserStateController(String baseUrl)
            : this((baseUrl == null || String.IsNullOrEmpty(baseUrl)) ? null : new ResourceResolver(baseUrl, new NoDuplicatesResourceRetriever
                ())) {
        }

        private CssParserStateController(ResourceResolver resourceResolver) {
            this.resourceResolver = resourceResolver;
            styleSheet = new CssStyleSheet();
            styleSheetFromImport = new CssStyleSheet();
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
            CssStyleSheet parsingResult = new CssStyleSheet();
            parsingResult.AppendCssStyleSheet(styleSheet);
            parsingResult.AppendCssStyleSheet(styleSheetFromImport);
            return parsingResult;
        }

//\cond DO_NOT_DOCUMENT
        /// <summary>Appends a character to the buffer.</summary>
        /// <param name="ch">the character</param>
        internal void AppendToBuffer(char ch) {
            buffer.Append(ch);
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Gets the contents of the buffer.</summary>
        /// <returns>the buffer contents</returns>
        internal String GetBufferContents() {
            return buffer.ToString();
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Resets the buffer.</summary>
        internal void ResetBuffer() {
            buffer.Length = 0;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Enter the previous active state.</summary>
        internal void EnterPreviousActiveState() {
            SetState(previousActiveState);
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Enter the comment start state.</summary>
        internal void EnterCommentStartState() {
            SaveActiveState();
            SetState(commentStartState);
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Enter the comment end state.</summary>
        internal void EnterCommentEndState() {
            SetState(commendEndState);
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Enter the comment inner state.</summary>
        internal void EnterCommentInnerState() {
            SetState(commendInnerState);
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Enter the rule state.</summary>
        internal void EnterRuleState() {
            SetState(ruleState);
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Enter the unknown state if nested blocks are finished.</summary>
        internal void EnterUnknownStateIfNestedBlocksFinished() {
            if (nestedAtRules.Count == 0) {
                SetState(unknownState);
            }
            else {
                EnterRuleStateBasedOnItsType();
            }
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Enter the rule state, based on whether the current state is unsupported or conditional.</summary>
        internal void EnterRuleStateBasedOnItsType() {
            if (CurrentAtRuleIsConditionalGroupRule()) {
                EnterConditionalGroupAtRuleBlockState();
            }
            else {
                EnterAtRuleBlockState();
            }
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Enter the unknown state.</summary>
        internal void EnterUnknownState() {
            SetState(unknownState);
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Enter the At-rule block state.</summary>
        internal void EnterAtRuleBlockState() {
            SetState(atRuleBlockState);
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Enter the conditional group At-rule block state.</summary>
        internal void EnterConditionalGroupAtRuleBlockState() {
            SetState(conditionalGroupAtRuleBlockState);
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Enter the properties state.</summary>
        internal void EnterPropertiesState() {
            SetState(propertiesState);
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Store the current selector.</summary>
        internal void StoreCurrentSelector() {
            currentSelector = buffer.ToString();
            buffer.Length = 0;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Store the current properties.</summary>
        internal void StoreCurrentProperties() {
            if (isCurrentRuleSupported) {
                ProcessProperties(currentSelector, buffer.ToString());
            }
            currentSelector = null;
            buffer.Length = 0;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Store the current properties without selector.</summary>
        internal void StoreCurrentPropertiesWithoutSelector() {
            if (isCurrentRuleSupported) {
                ProcessProperties(buffer.ToString());
            }
            buffer.Length = 0;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Store the semicolon At-rule.</summary>
        internal void StoreSemicolonAtRule() {
            if (isCurrentRuleSupported) {
                ProcessSemicolonAtRule(buffer.ToString());
            }
            buffer.Length = 0;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
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
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Push the block preceding At-rule.</summary>
        internal void PushBlockPrecedingAtRule() {
            nestedAtRules.Push(CssAtRuleFactory.CreateNestedRule(buffer.ToString()));
            storedPropertiesWithoutSelector.Push(new List<CssDeclaration>());
            isCurrentRuleSupported = IsCurrentRuleSupported();
            buffer.Length = 0;
        }
//\endcond

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
                if (nestedAtRules.IsEmpty()) {
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
            if (!storedPropertiesWithoutSelector.IsEmpty()) {
                IList<CssDeclaration> cssDeclarations = CssRuleSetParser.ParsePropertyDeclarations(properties);
                NormalizeDeclarationURIs(cssDeclarations);
                storedPropertiesWithoutSelector.Peek().AddAll(cssDeclarations);
            }
        }

        /// <summary>Normalizes the declaration URIs.</summary>
        /// <param name="declarations">the declarations</param>
        private void NormalizeDeclarationURIs(IList<CssDeclaration> declarations) {
            // This is the case when css has no location and thus urls should not be resolved against base css location
            if (this.resourceResolver == null) {
                return;
            }
            foreach (CssDeclaration declaration in declarations) {
                if (declaration.GetExpression().Contains("url(")) {
                    NormalizeSingleDeclarationURI(declaration);
                }
            }
        }

        private void NormalizeSingleDeclarationURI(CssDeclaration declaration) {
            CssDeclarationValueTokenizer tokenizer = new CssDeclarationValueTokenizer(declaration.GetExpression());
            CssDeclarationValueTokenizer.Token token;
            StringBuilder normalizedDeclaration = new StringBuilder();
            while ((token = tokenizer.GetNextValidToken()) != null) {
                String strToAppend;
                if (token.GetType() == CssDeclarationValueTokenizer.TokenType.FUNCTION && token.GetValue().StartsWith("url("
                    )) {
                    String url = token.GetValue().Trim();
                    url = url.JSubstring(4, url.Length - 1).Trim();
                    url = CssUtils.ExtractUnquotedString(url);
                    if (CssTypesValidationUtils.IsInlineData(url) || url.StartsWith("#")) {
                        strToAppend = token.GetValue().Trim();
                    }
                    else {
                        String finalUrl = url;
                        try {
                            finalUrl = resourceResolver.ResolveAgainstBaseUri(url).ToExternalForm();
                        }
                        catch (UriFormatException) {
                        }
                        strToAppend = MessageFormatUtil.Format("url({0})", finalUrl);
                    }
                }
                else {
                    if (token.GetType() == CssDeclarationValueTokenizer.TokenType.STRING && token.GetStringQuote() != 0) {
                        // If we parse string with quotes, save them
                        strToAppend = token.GetStringQuote() + token.GetValue() + token.GetStringQuote();
                    }
                    else {
                        strToAppend = token.GetValue();
                    }
                }
                if (normalizedDeclaration.Length > 0 && token.GetType() != CssDeclarationValueTokenizer.TokenType.COMMA) {
                    // Don't add space at the start and before comma
                    normalizedDeclaration.Append(' ');
                }
                normalizedDeclaration.Append(strToAppend);
            }
            declaration.SetExpression(normalizedDeclaration.ToString());
        }

        /// <summary>Processes the semicolon At-rule.</summary>
        /// <param name="ruleStr">the rule str</param>
        private void ProcessSemicolonAtRule(String ruleStr) {
            CssSemicolonAtRule atRule = CssAtRuleFactory.CreateSemicolonAtRule(ruleStr);
            if (atRule is CssImportAtRule) {
                bool isPositionCorrect = true;
                foreach (CssStatement statement in styleSheet.GetStatements()) {
                    if (statement is CssAtRule) {
                        String ruleName = ((CssAtRule)statement).GetRuleName();
                        if (!CssImportAtRule.ALLOWED_RULES_BEFORE.Contains(ruleName)) {
                            isPositionCorrect = false;
                            break;
                        }
                    }
                    else {
                        isPositionCorrect = false;
                        break;
                    }
                }
                if (isPositionCorrect) {
                    if (resourceResolver == null) {
                        LOGGER.LogError(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.IMPORT_RULE_URL_CAN_NOT_BE_RESOLVED
                            );
                        return;
                    }
                    String externalCss = CssUtils.ExtractUrl(atRule.GetRuleParams());
                    try {
                        using (Stream stream = resourceResolver.RetrieveResourceAsInputStream(externalCss)) {
                            if (stream != null) {
                                ResourceResolver newResourceResolver = new ResourceResolver(resourceResolver.ResolveAgainstBaseUri(externalCss
                                    ).ToExternalForm(), resourceResolver.GetRetriever());
                                iText.StyledXmlParser.Css.Parse.Syntax.CssParserStateController controller = new iText.StyledXmlParser.Css.Parse.Syntax.CssParserStateController
                                    (newResourceResolver);
                                CssStyleSheet externalStyleSheet = CssStyleSheetParser.Parse(stream, controller);
                                styleSheetFromImport.AppendCssStyleSheet(externalStyleSheet);
                            }
                        }
                    }
                    catch (System.IO.IOException e) {
                        LOGGER.LogError(e, iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.UNABLE_TO_PROCESS_EXTERNAL_CSS_FILE
                            );
                    }
                }
                else {
                    LOGGER.LogWarning(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.IMPORT_MUST_COME_BEFORE);
                }
            }
            else {
                styleSheet.AddStatement(atRule);
            }
        }

        /// <summary>Processes the finished At-rule block.</summary>
        /// <param name="atRule">the at rule</param>
        private void ProcessFinishedAtRuleBlock(CssNestedAtRule atRule) {
            if (nestedAtRules.IsEmpty()) {
                styleSheet.AddStatement(atRule);
            }
            else {
                nestedAtRules.Peek().AddStatementToBody(atRule);
            }
        }

        /// <summary>Checks if is current rule is supported.</summary>
        /// <returns>true, if the current rule is supported</returns>
        private bool IsCurrentRuleSupported() {
            bool isSupported = nestedAtRules.IsEmpty() || SUPPORTED_RULES.Contains(nestedAtRules.Peek().GetRuleName());
            if (!isSupported) {
                LOGGER.LogError(MessageFormatUtil.Format(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.RULE_IS_NOT_SUPPORTED
                    , nestedAtRules.Peek().GetRuleName()));
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
