/*
This file is part of the iText (R) project.
Copyright (c) 1998-2022 iText Group NV
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
using System.Linq;
using System.Text;
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Commons.Utils;
using iText.StyledXmlParser.Css;
using iText.StyledXmlParser.Css.Resolve.Shorthand;
using iText.StyledXmlParser.Css.Util;

namespace iText.StyledXmlParser.Css.Resolve.Shorthand.Impl {
    /// <summary>
    /// <see cref="iText.StyledXmlParser.Css.Resolve.Shorthand.IShorthandResolver"/>
    /// implementation for fonts.
    /// </summary>
    public class FontShorthandResolver : IShorthandResolver {
        /// <summary>Unsupported shorthand values.</summary>
        private static readonly ICollection<String> UNSUPPORTED_VALUES_OF_FONT_SHORTHAND = JavaCollectionsUtil.UnmodifiableSet
            (new HashSet<String>(JavaUtil.ArraysAsList(CommonCssConstants.CAPTION, CommonCssConstants.ICON, CommonCssConstants
            .MENU, CommonCssConstants.MESSAGE_BOX, CommonCssConstants.SMALL_CAPTION, CommonCssConstants.STATUS_BAR
            )));

        /// <summary>Font weight values.</summary>
        private static readonly ICollection<String> FONT_WEIGHT_NOT_DEFAULT_VALUES = JavaCollectionsUtil.UnmodifiableSet
            (new HashSet<String>(JavaUtil.ArraysAsList(CommonCssConstants.BOLD, CommonCssConstants.BOLDER, CommonCssConstants
            .LIGHTER, "100", "200", "300", "400", "500", "600", "700", "800", "900")));

        /// <summary>Font size values.</summary>
        private static readonly ICollection<String> FONT_SIZE_VALUES = JavaCollectionsUtil.UnmodifiableSet(new HashSet
            <String>(JavaUtil.ArraysAsList(CommonCssConstants.MEDIUM, CommonCssConstants.XX_SMALL, CommonCssConstants
            .X_SMALL, CommonCssConstants.SMALL, CommonCssConstants.LARGE, CommonCssConstants.X_LARGE, CommonCssConstants
            .XX_LARGE, CommonCssConstants.SMALLER, CommonCssConstants.LARGER)));

        /* (non-Javadoc)
        * @see com.itextpdf.styledxmlparser.css.resolve.shorthand.IShorthandResolver#resolveShorthand(java.lang.String)
        */
        public virtual IList<CssDeclaration> ResolveShorthand(String shorthandExpression) {
            if (UNSUPPORTED_VALUES_OF_FONT_SHORTHAND.Contains(shorthandExpression)) {
                ILogger logger = ITextLogManager.GetLogger(typeof(FontShorthandResolver));
                logger.LogError(MessageFormatUtil.Format("The \"{0}\" value of CSS shorthand property \"font\" is not supported"
                    , shorthandExpression));
            }
            if (CommonCssConstants.INITIAL.Equals(shorthandExpression) || CommonCssConstants.INHERIT.Equals(shorthandExpression
                )) {
                return JavaUtil.ArraysAsList(new CssDeclaration(CommonCssConstants.FONT_STYLE, shorthandExpression), new CssDeclaration
                    (CommonCssConstants.FONT_VARIANT, shorthandExpression), new CssDeclaration(CommonCssConstants.FONT_WEIGHT
                    , shorthandExpression), new CssDeclaration(CommonCssConstants.FONT_SIZE, shorthandExpression), new CssDeclaration
                    (CommonCssConstants.LINE_HEIGHT, shorthandExpression), new CssDeclaration(CommonCssConstants.FONT_FAMILY
                    , shorthandExpression));
            }
            String fontStyleValue = null;
            String fontVariantValue = null;
            String fontWeightValue = null;
            String fontSizeValue = null;
            String lineHeightValue = null;
            String fontFamilyValue = null;
            String[] props = iText.Commons.Utils.StringUtil.Split(shorthandExpression, ",");
            String shExprFixed = String.Join(",", JavaUtil.ArraysToEnumerable(props).Select((str) => str.Trim()).ToList
                ());
            IList<String> properties = GetFontProperties(shExprFixed);
            foreach (String value in properties) {
                int slashSymbolIndex = value.IndexOf('/');
                if (CommonCssConstants.ITALIC.Equals(value) || CommonCssConstants.OBLIQUE.Equals(value)) {
                    fontStyleValue = value;
                }
                else {
                    if (CommonCssConstants.SMALL_CAPS.Equals(value)) {
                        fontVariantValue = value;
                    }
                    else {
                        if (FONT_WEIGHT_NOT_DEFAULT_VALUES.Contains(value)) {
                            fontWeightValue = value;
                        }
                        else {
                            if (slashSymbolIndex > 0) {
                                fontSizeValue = value.JSubstring(0, slashSymbolIndex);
                                lineHeightValue = value.JSubstring(slashSymbolIndex + 1, value.Length);
                            }
                            else {
                                if (FONT_SIZE_VALUES.Contains(value) || CssTypesValidationUtils.IsMetricValue(value) || CssTypesValidationUtils
                                    .IsNumber(value) || CssTypesValidationUtils.IsRelativeValue(value)) {
                                    fontSizeValue = value;
                                }
                                else {
                                    fontFamilyValue = value;
                                }
                            }
                        }
                    }
                }
            }
            IList<CssDeclaration> cssDeclarations = JavaUtil.ArraysAsList(new CssDeclaration(CommonCssConstants.FONT_STYLE
                , fontStyleValue == null ? CommonCssConstants.INITIAL : fontStyleValue), new CssDeclaration(CommonCssConstants
                .FONT_VARIANT, fontVariantValue == null ? CommonCssConstants.INITIAL : fontVariantValue), new CssDeclaration
                (CommonCssConstants.FONT_WEIGHT, fontWeightValue == null ? CommonCssConstants.INITIAL : fontWeightValue
                ), new CssDeclaration(CommonCssConstants.FONT_SIZE, fontSizeValue == null ? CommonCssConstants.INITIAL
                 : fontSizeValue), new CssDeclaration(CommonCssConstants.LINE_HEIGHT, lineHeightValue == null ? CommonCssConstants
                .INITIAL : lineHeightValue), new CssDeclaration(CommonCssConstants.FONT_FAMILY, fontFamilyValue == null
                 ? CommonCssConstants.INITIAL : fontFamilyValue));
            return cssDeclarations;
        }

        /// <summary>Gets the font properties.</summary>
        /// <param name="shorthandExpression">the shorthand expression</param>
        /// <returns>the font properties</returns>
        private IList<String> GetFontProperties(String shorthandExpression) {
            bool doubleQuotesAreSpotted = false;
            bool singleQuoteIsSpotted = false;
            IList<String> properties = new List<String>();
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < shorthandExpression.Length; i++) {
                char currentChar = shorthandExpression[i];
                if (currentChar == '\"') {
                    doubleQuotesAreSpotted = !doubleQuotesAreSpotted;
                    sb.Append(currentChar);
                }
                else {
                    if (currentChar == '\'') {
                        singleQuoteIsSpotted = !singleQuoteIsSpotted;
                        sb.Append(currentChar);
                    }
                    else {
                        if (!doubleQuotesAreSpotted && !singleQuoteIsSpotted && iText.IO.Util.TextUtil.IsWhiteSpace(currentChar)) {
                            if (sb.Length > 0) {
                                properties.Add(sb.ToString());
                                sb = new StringBuilder();
                            }
                        }
                        else {
                            sb.Append(currentChar);
                        }
                    }
                }
            }
            if (sb.Length > 0) {
                properties.Add(sb.ToString());
            }
            return properties;
        }
    }
}
