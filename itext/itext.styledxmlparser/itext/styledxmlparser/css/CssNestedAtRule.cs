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
using iText.Commons.Utils;
using iText.StyledXmlParser.Css.Media;
using iText.StyledXmlParser.Node;

namespace iText.StyledXmlParser.Css {
    /// <summary>
    /// Class to store a nested CSS at-rule
    /// Nested at-rules are a subset of nested statements, which can be used
    /// as a statement of a style sheet as well as inside of conditional group rules.
    /// </summary>
    public class CssNestedAtRule : CssAtRule {
        /// <summary>The rule parameters.</summary>
        private String ruleParameters;

        /// <summary>The body.</summary>
        protected internal IList<CssStatement> body;

        /// <summary>
        /// Creates a
        /// <see cref="CssNestedAtRule"/>
        /// instance
        /// with an empty body.
        /// </summary>
        /// <param name="ruleName">the rule name</param>
        /// <param name="ruleParameters">the rule parameters</param>
        public CssNestedAtRule(String ruleName, String ruleParameters)
            : base(ruleName) {
            this.ruleParameters = ruleParameters;
            this.body = new List<CssStatement>();
        }

        /// <summary>Adds a CSS statement to body.</summary>
        /// <param name="statement">a CSS statement</param>
        public virtual void AddStatementToBody(CssStatement statement) {
            this.body.Add(statement);
        }

        /// <summary>Adds CSS statements to the body.</summary>
        /// <param name="statements">a list of CSS statements</param>
        public virtual void AddStatementsToBody(ICollection<CssStatement> statements) {
            this.body.AddAll(statements);
        }

        /// <summary>Adds the body CSS declarations.</summary>
        /// <param name="cssDeclarations">a list of CSS declarations</param>
        public virtual void AddBodyCssDeclarations(IList<CssDeclaration> cssDeclarations) {
        }

        // ignore by default
        /* (non-Javadoc)
        * @see com.itextpdf.styledxmlparser.css.CssStatement#getCssRuleSets(com.itextpdf.styledxmlparser.html.node.INode, com.itextpdf.styledxmlparser.css.media.MediaDeviceDescription)
        */
        public override IList<CssRuleSet> GetCssRuleSets(INode node, MediaDeviceDescription deviceDescription) {
            IList<CssRuleSet> result = new List<CssRuleSet>();
            foreach (CssStatement childStatement in body) {
                result.AddAll(childStatement.GetCssRuleSets(node, deviceDescription));
            }
            return result;
        }

        /// <summary>Gets the list of CSS statements.</summary>
        /// <returns>the list of CSS statements</returns>
        public virtual IList<CssStatement> GetStatements() {
            return body;
        }

        /* (non-Javadoc)
        * @see java.lang.Object#toString()
        */
        public override String ToString() {
            StringBuilder sb = new StringBuilder();
            sb.Append(MessageFormatUtil.Format("@{0} {1} ", ruleName, ruleParameters));
            sb.Append("{");
            sb.Append("\n");
            for (int i = 0; i < body.Count; i++) {
                sb.Append("    ");
                sb.Append(body[i].ToString().Replace("\n", "\n    "));
                if (i != body.Count - 1) {
                    sb.Append("\n");
                }
            }
            sb.Append("\n}");
            return sb.ToString();
        }

        public virtual String GetRuleParameters() {
            return ruleParameters;
        }
    }
}
