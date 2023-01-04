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
    /// implementation for borders.
    /// </summary>
    public abstract class AbstractBorderShorthandResolver : IShorthandResolver {
        /// <summary>The template for -width properties.</summary>
        private const String _0_WIDTH = "{0}-width";

        /// <summary>The template for -style properties.</summary>
        private const String _0_STYLE = "{0}-style";

        /// <summary>The template for -color properties.</summary>
        private const String _0_COLOR = "{0}-color";

        /// <summary>Gets the prefix of a property.</summary>
        /// <returns>the prefix</returns>
        protected internal abstract String GetPrefix();

        /* (non-Javadoc)
        * @see com.itextpdf.styledxmlparser.css.resolve.shorthand.IShorthandResolver#resolveShorthand(java.lang.String)
        */
        public virtual IList<CssDeclaration> ResolveShorthand(String shorthandExpression) {
            String widthPropName = MessageFormatUtil.Format(_0_WIDTH, GetPrefix());
            String stylePropName = MessageFormatUtil.Format(_0_STYLE, GetPrefix());
            String colorPropName = MessageFormatUtil.Format(_0_COLOR, GetPrefix());
            if (CommonCssConstants.INITIAL.Equals(shorthandExpression) || CommonCssConstants.INHERIT.Equals(shorthandExpression
                )) {
                return JavaUtil.ArraysAsList(new CssDeclaration(widthPropName, shorthandExpression), new CssDeclaration(stylePropName
                    , shorthandExpression), new CssDeclaration(colorPropName, shorthandExpression));
            }
            IList<String> props = CssUtils.ExtractShorthandProperties(shorthandExpression)[0];
            String borderColorValue = null;
            String borderStyleValue = null;
            String borderWidthValue = null;
            foreach (String value in props) {
                if (CommonCssConstants.INITIAL.Equals(value) || CommonCssConstants.INHERIT.Equals(value)) {
                    ILogger logger = ITextLogManager.GetLogger(typeof(AbstractBorderShorthandResolver));
                    logger.LogWarning(MessageFormatUtil.Format(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.INVALID_CSS_PROPERTY_DECLARATION
                        , shorthandExpression));
                    return JavaCollectionsUtil.EmptyList<CssDeclaration>();
                }
                if (CommonCssConstants.BORDER_WIDTH_VALUES.Contains(value) || CssTypesValidationUtils.IsNumber(value) || CssTypesValidationUtils
                    .IsMetricValue(value) || CssTypesValidationUtils.IsRelativeValue(value)) {
                    borderWidthValue = value;
                }
                else {
                    if (CommonCssConstants.BORDER_STYLE_VALUES.Contains(value) || value.Equals(CommonCssConstants.AUTO)) {
                        // AUTO property value is needed for outline property only
                        borderStyleValue = value;
                    }
                    else {
                        if (CssTypesValidationUtils.IsColorProperty(value)) {
                            borderColorValue = value;
                        }
                    }
                }
            }
            IList<CssDeclaration> resolvedDecl = new List<CssDeclaration>();
            resolvedDecl.Add(new CssDeclaration(widthPropName, borderWidthValue == null ? CommonCssConstants.INITIAL : 
                borderWidthValue));
            resolvedDecl.Add(new CssDeclaration(stylePropName, borderStyleValue == null ? CommonCssConstants.INITIAL : 
                borderStyleValue));
            resolvedDecl.Add(new CssDeclaration(colorPropName, borderColorValue == null ? CommonCssConstants.INITIAL : 
                borderColorValue));
            return resolvedDecl;
        }
    }
}
