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
using iText.Commons.Utils;

namespace iText.StyledXmlParser.Css {
    /// <summary>
    /// Implementation of
    /// <see cref="CssSemicolonAtRule"/>
    /// for
    /// <c>import</c>
    /// rule.
    /// </summary>
    public class CssImportAtRule : CssSemicolonAtRule {
        /// <summary>
        /// The list of rules which are allowed to be before
        /// <c>import</c>
        /// rule declaration in CSS stylesheet.
        /// </summary>
        public static readonly ICollection<String> ALLOWED_RULES_BEFORE = JavaCollectionsUtil.UnmodifiableSet(new 
            HashSet<String>(JavaUtil.ArraysAsList(CssRuleName.CHARSET, CssRuleName.IMPORT, CssRuleName.LAYER)));

        /// <summary>
        /// Creates a new
        /// <see cref="CssImportAtRule"/>
        /// instance.
        /// </summary>
        /// <param name="ruleParameters">the rule parameters</param>
        public CssImportAtRule(String ruleParameters)
            : base(CssRuleName.IMPORT, ruleParameters) {
        }
    }
}
