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
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Commons.Utils;
using iText.StyledXmlParser.Css;
using iText.StyledXmlParser.Css.Resolve.Shorthand;
using iText.StyledXmlParser.Css.Util;

namespace iText.StyledXmlParser.Css.Resolve.Shorthand.Impl {
    /// <summary>
    /// Abstract
    /// <see cref="iText.StyledXmlParser.Css.Resolve.Shorthand.IShorthandResolver"/>
    /// implementation for box definitions.
    /// </summary>
    public abstract class AbstractBoxShorthandResolver : IShorthandResolver {
        /// <summary>The template for -left properties.</summary>
        private const String _0_LEFT_1 = "{0}-left{1}";

        /// <summary>The template for -right properties.</summary>
        private const String _0_RIGHT_1 = "{0}-right{1}";

        /// <summary>The template for -bottom properties.</summary>
        private const String _0_BOTTOM_1 = "{0}-bottom{1}";

        /// <summary>The template for -top properties.</summary>
        private const String _0_TOP_1 = "{0}-top{1}";

        /// <summary>Gets the prefix of a property.</summary>
        /// <returns>the prefix</returns>
        protected internal abstract String GetPrefix();

        /// <summary>Gets the postfix of a property.</summary>
        /// <returns>the postfix</returns>
        protected internal abstract String GetPostfix();

        /* (non-Javadoc)
        * @see com.itextpdf.styledxmlparser.css.resolve.shorthand.IShorthandResolver#resolveShorthand(java.lang.String)
        */
        public virtual IList<CssDeclaration> ResolveShorthand(String shorthandExpression) {
            IList<String> props = CssUtils.ExtractShorthandProperties(shorthandExpression)[0];
            IList<CssDeclaration> resolvedDecl = new List<CssDeclaration>();
            String topProperty = MessageFormatUtil.Format(_0_TOP_1, GetPrefix(), GetPostfix());
            String rightProperty = MessageFormatUtil.Format(_0_RIGHT_1, GetPrefix(), GetPostfix());
            String bottomProperty = MessageFormatUtil.Format(_0_BOTTOM_1, GetPrefix(), GetPostfix());
            String leftProperty = MessageFormatUtil.Format(_0_LEFT_1, GetPrefix(), GetPostfix());
            if (props.Count == 1) {
                resolvedDecl.Add(new CssDeclaration(topProperty, props[0]));
                resolvedDecl.Add(new CssDeclaration(rightProperty, props[0]));
                resolvedDecl.Add(new CssDeclaration(bottomProperty, props[0]));
                resolvedDecl.Add(new CssDeclaration(leftProperty, props[0]));
            }
            else {
                foreach (String prop in props) {
                    if (CommonCssConstants.INHERIT.Equals(prop) || CommonCssConstants.INITIAL.Equals(prop)) {
                        ILogger logger = ITextLogManager.GetLogger(typeof(AbstractBoxShorthandResolver));
                        logger.LogWarning(MessageFormatUtil.Format(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.INVALID_CSS_PROPERTY_DECLARATION
                            , shorthandExpression));
                        return JavaCollectionsUtil.EmptyList<CssDeclaration>();
                    }
                }
                if (props.Count == 2) {
                    resolvedDecl.Add(new CssDeclaration(topProperty, props[0]));
                    resolvedDecl.Add(new CssDeclaration(rightProperty, props[1]));
                    resolvedDecl.Add(new CssDeclaration(bottomProperty, props[0]));
                    resolvedDecl.Add(new CssDeclaration(leftProperty, props[1]));
                }
                else {
                    if (props.Count == 3) {
                        resolvedDecl.Add(new CssDeclaration(topProperty, props[0]));
                        resolvedDecl.Add(new CssDeclaration(rightProperty, props[1]));
                        resolvedDecl.Add(new CssDeclaration(bottomProperty, props[2]));
                        resolvedDecl.Add(new CssDeclaration(leftProperty, props[1]));
                    }
                    else {
                        if (props.Count == 4) {
                            resolvedDecl.Add(new CssDeclaration(topProperty, props[0]));
                            resolvedDecl.Add(new CssDeclaration(rightProperty, props[1]));
                            resolvedDecl.Add(new CssDeclaration(bottomProperty, props[2]));
                            resolvedDecl.Add(new CssDeclaration(leftProperty, props[3]));
                        }
                    }
                }
            }
            return resolvedDecl;
        }
    }
}
