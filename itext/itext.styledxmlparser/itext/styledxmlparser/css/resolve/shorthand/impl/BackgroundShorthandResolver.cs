/*
This file is part of the iText (R) project.
Copyright (c) 1998-2020 iText Group NV
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
using Common.Logging;
using iText.IO.Util;
using iText.StyledXmlParser.Css;
using iText.StyledXmlParser.Css.Resolve.Shorthand;
using iText.StyledXmlParser.Css.Util;

namespace iText.StyledXmlParser.Css.Resolve.Shorthand.Impl {
    /// <summary>
    /// <see cref="iText.StyledXmlParser.Css.Resolve.Shorthand.IShorthandResolver"/>
    /// implementation for backgrounds.
    /// </summary>
    public class BackgroundShorthandResolver : IShorthandResolver {
        /// <summary>The Constant UNDEFINED_TYPE.</summary>
        private const int UNDEFINED_TYPE = -1;

        /// <summary>The Constant BACKGROUND_COLOR_TYPE.</summary>
        private const int BACKGROUND_COLOR_TYPE = 0;

        /// <summary>The Constant BACKGROUND_IMAGE_TYPE.</summary>
        private const int BACKGROUND_IMAGE_TYPE = 1;

        /// <summary>The Constant BACKGROUND_POSITION_TYPE.</summary>
        private const int BACKGROUND_POSITION_TYPE = 2;

        /// <summary>The Constant BACKGROUND_POSITION_OR_SIZE_TYPE.</summary>
        private const int BACKGROUND_POSITION_OR_SIZE_TYPE = 3;

        // might have the same type, but position always precedes size
        /// <summary>The Constant BACKGROUND_REPEAT_TYPE.</summary>
        private const int BACKGROUND_REPEAT_TYPE = 4;

        /// <summary>The Constant BACKGROUND_ORIGIN_OR_CLIP_TYPE.</summary>
        private const int BACKGROUND_ORIGIN_OR_CLIP_TYPE = 5;

        // have the same possible values but apparently origin values precedes clip value
        /// <summary>The Constant BACKGROUND_CLIP_TYPE.</summary>
        private const int BACKGROUND_CLIP_TYPE = 6;

        /// <summary>The Constant BACKGROUND_ATTACHMENT_TYPE.</summary>
        private const int BACKGROUND_ATTACHMENT_TYPE = 7;

        // With CSS3, you can apply multiple backgrounds to elements. These are layered atop one another
        // with the first background you provide on top and the last background listed in the back. Only
        // the last background can include a background color.
        /* (non-Javadoc)
        * @see com.itextpdf.styledxmlparser.css.resolve.shorthand.IShorthandResolver#resolveShorthand(java.lang.String)
        */
        public virtual IList<CssDeclaration> ResolveShorthand(String shorthandExpression) {
            if (CommonCssConstants.INITIAL.Equals(shorthandExpression) || CommonCssConstants.INHERIT.Equals(shorthandExpression
                )) {
                return JavaUtil.ArraysAsList(new CssDeclaration(CommonCssConstants.BACKGROUND_COLOR, shorthandExpression), 
                    new CssDeclaration(CommonCssConstants.BACKGROUND_IMAGE, shorthandExpression), new CssDeclaration(CommonCssConstants
                    .BACKGROUND_POSITION, shorthandExpression), new CssDeclaration(CommonCssConstants.BACKGROUND_SIZE, shorthandExpression
                    ), new CssDeclaration(CommonCssConstants.BACKGROUND_REPEAT, shorthandExpression), new CssDeclaration(CommonCssConstants
                    .BACKGROUND_ORIGIN, shorthandExpression), new CssDeclaration(CommonCssConstants.BACKGROUND_CLIP, shorthandExpression
                    ), new CssDeclaration(CommonCssConstants.BACKGROUND_ATTACHMENT, shorthandExpression));
            }
            IList<String> commaSeparatedExpressions = SplitMultipleBackgrounds(shorthandExpression);
            // TODO ignore multiple backgrounds at the moment
            String backgroundExpression = commaSeparatedExpressions[0];
            String[] resolvedProps = new String[8];
            String[] props = iText.IO.Util.StringUtil.Split(backgroundExpression, "\\s+");
            bool slashEncountered = false;
            foreach (String value in props) {
                int slashCharInd = value.IndexOf('/');
                if (slashCharInd > 0 && !value.Contains("url(")) {
                    slashEncountered = true;
                    String value1 = value.JSubstring(0, slashCharInd);
                    String value2 = value.JSubstring(slashCharInd + 1, value.Length);
                    PutPropertyBasedOnType(ResolvePropertyType(value1), value1, resolvedProps, false);
                    PutPropertyBasedOnType(ResolvePropertyType(value2), value2, resolvedProps, true);
                }
                else {
                    PutPropertyBasedOnType(ResolvePropertyType(value), value, resolvedProps, slashEncountered);
                }
            }
            for (int i = 0; i < resolvedProps.Length; ++i) {
                if (resolvedProps[i] == null) {
                    resolvedProps[i] = CommonCssConstants.INITIAL;
                }
            }
            IList<CssDeclaration> cssDeclarations = JavaUtil.ArraysAsList(new CssDeclaration(CommonCssConstants.BACKGROUND_COLOR
                , resolvedProps[BACKGROUND_COLOR_TYPE]), new CssDeclaration(CommonCssConstants.BACKGROUND_IMAGE, resolvedProps
                [BACKGROUND_IMAGE_TYPE]), new CssDeclaration(CommonCssConstants.BACKGROUND_POSITION, resolvedProps[BACKGROUND_POSITION_TYPE
                ]), new CssDeclaration(CommonCssConstants.BACKGROUND_SIZE, resolvedProps[BACKGROUND_POSITION_OR_SIZE_TYPE
                ]), new CssDeclaration(CommonCssConstants.BACKGROUND_REPEAT, resolvedProps[BACKGROUND_REPEAT_TYPE]), new 
                CssDeclaration(CommonCssConstants.BACKGROUND_ORIGIN, resolvedProps[BACKGROUND_ORIGIN_OR_CLIP_TYPE]), new 
                CssDeclaration(CommonCssConstants.BACKGROUND_CLIP, resolvedProps[BACKGROUND_CLIP_TYPE]), new CssDeclaration
                (CommonCssConstants.BACKGROUND_ATTACHMENT, resolvedProps[BACKGROUND_ATTACHMENT_TYPE]));
            return cssDeclarations;
        }

        /// <summary>Resolves the property type.</summary>
        /// <param name="value">the value</param>
        /// <returns>the property type value</returns>
        private int ResolvePropertyType(String value) {
            if (value.Contains("url(") || CommonCssConstants.NONE.Equals(value)) {
                return BACKGROUND_IMAGE_TYPE;
            }
            else {
                if (CommonCssConstants.BACKGROUND_REPEAT_VALUES.Contains(value)) {
                    return BACKGROUND_REPEAT_TYPE;
                }
                else {
                    if (CommonCssConstants.BACKGROUND_ATTACHMENT_VALUES.Contains(value)) {
                        return BACKGROUND_ATTACHMENT_TYPE;
                    }
                    else {
                        if (CommonCssConstants.BACKGROUND_POSITION_VALUES.Contains(value)) {
                            return BACKGROUND_POSITION_TYPE;
                        }
                        else {
                            if (CssUtils.IsNumericValue(value) || CssUtils.IsMetricValue(value) || CssUtils.IsRelativeValue(value)) {
                                return BACKGROUND_POSITION_OR_SIZE_TYPE;
                            }
                            else {
                                if (CommonCssConstants.BACKGROUND_SIZE_VALUES.Contains(value)) {
                                    return BACKGROUND_POSITION_OR_SIZE_TYPE;
                                }
                                else {
                                    if (CssUtils.IsColorProperty(value)) {
                                        return BACKGROUND_COLOR_TYPE;
                                    }
                                    else {
                                        if (CommonCssConstants.BACKGROUND_ORIGIN_OR_CLIP_VALUES.Contains(value)) {
                                            return BACKGROUND_ORIGIN_OR_CLIP_TYPE;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return UNDEFINED_TYPE;
        }

        /// <summary>Registers a property based on its type.</summary>
        /// <param name="type">the property type</param>
        /// <param name="value">the property value</param>
        /// <param name="resolvedProps">the resolved properties</param>
        /// <param name="slashEncountered">indicates whether a slash was encountered</param>
        private void PutPropertyBasedOnType(int type, String value, String[] resolvedProps, bool slashEncountered) {
            if (type == UNDEFINED_TYPE) {
                ILog logger = LogManager.GetLogger(typeof(BackgroundShorthandResolver));
                logger.Error(MessageFormatUtil.Format(iText.StyledXmlParser.LogMessageConstant.WAS_NOT_ABLE_TO_DEFINE_BACKGROUND_CSS_SHORTHAND_PROPERTIES
                    , value));
                return;
            }
            if (type == BACKGROUND_POSITION_OR_SIZE_TYPE && !slashEncountered) {
                type = BACKGROUND_POSITION_TYPE;
            }
            if (type == BACKGROUND_ORIGIN_OR_CLIP_TYPE && resolvedProps[BACKGROUND_ORIGIN_OR_CLIP_TYPE] != null) {
                type = BACKGROUND_CLIP_TYPE;
            }
            if ((type == BACKGROUND_POSITION_OR_SIZE_TYPE || type == BACKGROUND_POSITION_TYPE) && resolvedProps[type] 
                != null) {
                resolvedProps[type] += " " + value;
            }
            else {
                resolvedProps[type] = value;
            }
        }

        /// <summary>Splits multiple backgrounds.</summary>
        /// <param name="shorthandExpression">the shorthand expression</param>
        /// <returns>the list of backgrounds</returns>
        private IList<String> SplitMultipleBackgrounds(String shorthandExpression) {
            IList<String> commaSeparatedExpressions = new List<String>();
            bool isInsideParentheses = false;
            // in order to avoid split inside rgb/rgba color definition
            int prevStart = 0;
            for (int i = 0; i < shorthandExpression.Length; ++i) {
                if (shorthandExpression[i] == ',' && !isInsideParentheses) {
                    commaSeparatedExpressions.Add(shorthandExpression.JSubstring(prevStart, i));
                    prevStart = i + 1;
                }
                else {
                    if (shorthandExpression[i] == '(') {
                        isInsideParentheses = true;
                    }
                    else {
                        if (shorthandExpression[i] == ')') {
                            isInsideParentheses = false;
                        }
                    }
                }
            }
            if (commaSeparatedExpressions.IsEmpty()) {
                commaSeparatedExpressions.Add(shorthandExpression);
            }
            return commaSeparatedExpressions;
        }
    }
}
