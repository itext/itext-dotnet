/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 Apryse Group NV
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

namespace iText.StyledXmlParser.Css.Resolve.Shorthand.Impl {
    /// <summary>
    /// <see cref="AbstractBorderShorthandResolver"/>
    /// implementation for borders.
    /// </summary>
    public class BorderShorthandResolver : AbstractBorderShorthandResolver {
        /* (non-Javadoc)
        * @see com.itextpdf.styledxmlparser.css.resolve.shorthand.impl.AbstractBorderShorthandResolver#getPrefix()
        */
        protected internal override String GetPrefix() {
            return CommonCssConstants.BORDER;
        }

        /* (non-Javadoc)
        * @see com.itextpdf.styledxmlparser.css.resolve.shorthand.impl.AbstractBorderShorthandResolver#resolveShorthand(java.lang.String)
        */
        public override IList<CssDeclaration> ResolveShorthand(String shorthandExpression) {
            IList<CssDeclaration> preResolvedProps = base.ResolveShorthand(shorthandExpression);
            IList<CssDeclaration> resolvedProps = new List<CssDeclaration>();
            foreach (CssDeclaration prop in preResolvedProps) {
                IShorthandResolver shorthandResolver = ShorthandResolverFactory.GetShorthandResolver(prop.GetProperty());
                if (shorthandResolver != null) {
                    resolvedProps.AddAll(shorthandResolver.ResolveShorthand(prop.GetExpression()));
                }
                else {
                    ILogger logger = ITextLogManager.GetLogger(typeof(BorderShorthandResolver));
                    logger.LogError(MessageFormatUtil.Format("Cannot find a shorthand resolver for the \"{0}\" property. " + "Expected border-width, border-style or border-color properties."
                        , prop.GetProperty()));
                }
            }
            return resolvedProps;
        }
    }
}
