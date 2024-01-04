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

namespace iText.StyledXmlParser.Css.Media {
    /// <summary>
    /// Utilities class that parses
    /// <see cref="System.String"/>
    /// values into
    /// <see cref="MediaQuery"/>
    /// or
    /// <see cref="MediaExpression"/>
    /// values.
    /// </summary>
    public sealed class MediaQueryParser {
        /// <summary>
        /// Creates a
        /// <see cref="MediaQueryParser"/>
        /// instance.
        /// </summary>
        private MediaQueryParser() {
        }

        /// <summary>
        /// Parses a
        /// <see cref="System.String"/>
        /// into a
        /// <see cref="System.Collections.IList{E}"/>
        /// of
        /// <see cref="MediaQuery"/>
        /// values.
        /// </summary>
        /// <param name="mediaQueriesStr">
        /// the media queries in the form of a
        /// <see cref="System.String"/>
        /// </param>
        /// <returns>
        /// the resulting
        /// <see cref="System.Collections.IList{E}"/>
        /// of
        /// <see cref="MediaQuery"/>
        /// values
        /// </returns>
        internal static IList<MediaQuery> ParseMediaQueries(String mediaQueriesStr) {
            String[] mediaQueryStrs = iText.Commons.Utils.StringUtil.Split(mediaQueriesStr, ",");
            IList<MediaQuery> mediaQueries = new List<MediaQuery>();
            foreach (String mediaQueryStr in mediaQueryStrs) {
                MediaQuery mediaQuery = ParseMediaQuery(mediaQueryStr);
                if (mediaQuery != null) {
                    mediaQueries.Add(mediaQuery);
                }
            }
            return mediaQueries;
        }

        /// <summary>
        /// Parses a
        /// <see cref="System.String"/>
        /// into a
        /// <see cref="MediaQuery"/>
        /// value.
        /// </summary>
        /// <param name="mediaQueryStr">
        /// the media query in the form of a
        /// <see cref="System.String"/>
        /// </param>
        /// <returns>
        /// the resulting
        /// <see cref="MediaQuery"/>
        /// value
        /// </returns>
        internal static MediaQuery ParseMediaQuery(String mediaQueryStr) {
            mediaQueryStr = mediaQueryStr.Trim().ToLowerInvariant();
            bool only = false;
            bool not = false;
            if (mediaQueryStr.StartsWith(MediaRuleConstants.ONLY)) {
                only = true;
                mediaQueryStr = mediaQueryStr.Substring(MediaRuleConstants.ONLY.Length).Trim();
            }
            else {
                if (mediaQueryStr.StartsWith(MediaRuleConstants.NOT)) {
                    not = true;
                    mediaQueryStr = mediaQueryStr.Substring(MediaRuleConstants.NOT.Length).Trim();
                }
            }
            int indexOfSpace = mediaQueryStr.IndexOf(' ');
            String firstWord = indexOfSpace != -1 ? mediaQueryStr.JSubstring(0, indexOfSpace) : mediaQueryStr;
            String mediaType = null;
            IList<MediaExpression> mediaExpressions = null;
            if (only || not || MediaType.IsValidMediaType(firstWord)) {
                mediaType = firstWord;
                mediaExpressions = ParseMediaExpressions(mediaQueryStr.Substring(firstWord.Length), true);
            }
            else {
                mediaExpressions = ParseMediaExpressions(mediaQueryStr, false);
            }
            return new MediaQuery(mediaType, mediaExpressions, only, not);
        }

        /// <summary>
        /// Parses a
        /// <see cref="System.String"/>
        /// into a list of
        /// <see cref="MediaExpression"/>
        /// values.
        /// </summary>
        /// <param name="mediaExpressionsStr">
        /// the media expressions in the form of a
        /// <see cref="System.String"/>
        /// </param>
        /// <param name="shallStartWithAnd">indicates if the media expression shall start with "and"</param>
        /// <returns>
        /// the resulting list of
        /// <see cref="MediaExpression"/>
        /// values
        /// </returns>
        private static IList<MediaExpression> ParseMediaExpressions(String mediaExpressionsStr, bool shallStartWithAnd
            ) {
            mediaExpressionsStr = mediaExpressionsStr.Trim();
            bool startsWithEnd = mediaExpressionsStr.StartsWith(MediaRuleConstants.AND);
            bool firstExpression = true;
            String[] mediaExpressionStrs = iText.Commons.Utils.StringUtil.Split(mediaExpressionsStr, MediaRuleConstants
                .AND);
            IList<MediaExpression> expressions = new List<MediaExpression>();
            foreach (String mediaExpressionStr in mediaExpressionStrs) {
                MediaExpression expression = ParseMediaExpression(mediaExpressionStr);
                if (expression != null) {
                    if (firstExpression) {
                        if (shallStartWithAnd && !startsWithEnd) {
                            throw new InvalidOperationException("Expected 'and' while parsing media expression");
                        }
                    }
                    firstExpression = false;
                    expressions.Add(expression);
                }
            }
            return expressions;
        }

        /// <summary>
        /// Parses a
        /// <see cref="System.String"/>
        /// into a
        /// <see cref="MediaExpression"/>
        /// value.
        /// </summary>
        /// <param name="mediaExpressionStr">
        /// the media expression in the form of a
        /// <see cref="System.String"/>
        /// </param>
        /// <returns>
        /// the resulting
        /// <see cref="MediaExpression"/>
        /// value
        /// </returns>
        private static MediaExpression ParseMediaExpression(String mediaExpressionStr) {
            mediaExpressionStr = mediaExpressionStr.Trim();
            if (!mediaExpressionStr.StartsWith("(") || !mediaExpressionStr.EndsWith(")")) {
                return null;
            }
            mediaExpressionStr = mediaExpressionStr.JSubstring(1, mediaExpressionStr.Length - 1);
            if (mediaExpressionStr.Length == 0) {
                return null;
            }
            int colonPos = mediaExpressionStr.IndexOf(':');
            String mediaFeature;
            String value = null;
            if (colonPos == -1) {
                mediaFeature = mediaExpressionStr;
            }
            else {
                mediaFeature = mediaExpressionStr.JSubstring(0, colonPos).Trim();
                value = mediaExpressionStr.Substring(colonPos + 1).Trim();
            }
            return new MediaExpression(mediaFeature, value);
        }
    }
}
