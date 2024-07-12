/*
This file is part of the iText (R) project.
Copyright (c) 1998-2024 Apryse Group NV
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
    /// <see cref="iText.StyledXmlParser.Css.Resolve.Shorthand.IShorthandResolver"/>
    /// implementation for grid items column/row start and end positions.
    /// </summary>
    public abstract class GridItemShorthandResolver : IShorthandResolver {
        private static readonly ILogger LOGGER = ITextLogManager.GetLogger(typeof(iText.StyledXmlParser.Css.Resolve.Shorthand.Impl.GridItemShorthandResolver
            ));

        private readonly String propertyTemplate;

        /// <summary>Creates a new shorthand resolver for provided shorthand template</summary>
        /// <param name="shorthand">shorthand from which template will be created.</param>
        protected internal GridItemShorthandResolver(String shorthand) {
            this.propertyTemplate = shorthand + "-{0}";
        }

        public virtual IList<CssDeclaration> ResolveShorthand(String shorthandExpression) {
            shorthandExpression = shorthandExpression.Trim();
            if (String.IsNullOrEmpty(shorthandExpression)) {
                LOGGER.LogWarning(MessageFormatUtil.Format(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.SHORTHAND_PROPERTY_CANNOT_BE_EMPTY
                    , propertyTemplate.JSubstring(0, propertyTemplate.Length - 4)));
                return new List<CssDeclaration>();
            }
            if (CssTypesValidationUtils.IsInitialOrInheritOrUnset(shorthandExpression) || CommonCssConstants.AUTO.Equals
                (shorthandExpression)) {
                return new List<CssDeclaration>();
            }
            String[] values = iText.Commons.Utils.StringUtil.Split(shorthandExpression, "/");
            if (values.Length == 1) {
                if (shorthandExpression.StartsWith("span")) {
                    return JavaCollectionsUtil.SingletonList(new CssDeclaration(MessageFormatUtil.Format(propertyTemplate, "start"
                        ), values[0]));
                }
                return JavaUtil.ArraysAsList(new CssDeclaration(MessageFormatUtil.Format(propertyTemplate, "start"), values
                    [0]), new CssDeclaration(MessageFormatUtil.Format(propertyTemplate, "end"), values[0]));
            }
            return JavaUtil.ArraysAsList(new CssDeclaration(MessageFormatUtil.Format(propertyTemplate, "start"), values
                [0]), new CssDeclaration(MessageFormatUtil.Format(propertyTemplate, "end"), values[1]));
        }
    }
}
