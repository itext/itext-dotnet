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
