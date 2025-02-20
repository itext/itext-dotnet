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
using iText.Commons.Utils;

namespace iText.StyledXmlParser.Css {
    /// <summary>
    /// A
    /// <see cref="CssAtRule"/>
    /// implementation.
    /// </summary>
    public class CssSemicolonAtRule : CssAtRule {
        /// <summary>The rule parameters.</summary>
        private readonly String ruleParams;

        /// <summary>
        /// Creates a new
        /// <see cref="CssSemicolonAtRule"/>
        /// instance.
        /// </summary>
        /// <param name="ruleDeclaration">the rule declaration</param>
        [System.ObsoleteAttribute(@"use CssSemicolonAtRule(System.String, System.String) constructor instead")]
        public CssSemicolonAtRule(String ruleDeclaration)
            : base(CssAtRuleFactory
                        // After removing the constructor, make CssAtRuleFactory.extractRuleNameFromDeclaration private
                        .ExtractRuleNameFromDeclaration(ruleDeclaration.Trim())) {
            this.ruleParams = ruleDeclaration.Trim().Substring(ruleName.Length).Trim();
        }

        /// <summary>
        /// Creates a new
        /// <see cref="CssSemicolonAtRule"/>
        /// instance.
        /// </summary>
        /// <param name="ruleName">the rule name</param>
        /// <param name="ruleParams">the rule params</param>
        public CssSemicolonAtRule(String ruleName, String ruleParams)
            : base(ruleName) {
            this.ruleParams = ruleParams;
        }

        /// <summary>Gets the rule params.</summary>
        /// <returns>the rule params</returns>
        public virtual String GetRuleParams() {
            return ruleParams;
        }

        /* (non-Javadoc)
        * @see java.lang.Object#toString()
        */
        public override String ToString() {
            return MessageFormatUtil.Format("@{0} {1};", ruleName, ruleParams);
        }
    }
}
