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
using iText.StyledXmlParser.Css.Validate;

namespace iText.StyledXmlParser.Css.Resolve.Shorthand.Impl {
    /// <summary>
    /// <see cref="iText.StyledXmlParser.Css.Resolve.Shorthand.IShorthandResolver"/>
    /// implementation for background-position.
    /// </summary>
    public class BackgroundPositionShorthandResolver : IShorthandResolver {
        private static readonly ILogger LOGGER = ITextLogManager.GetLogger(typeof(BackgroundPositionShorthandResolver
            ));

        private const int POSITION_VALUES_MAX_COUNT = 2;

        /// <summary><inheritDoc/></summary>
        public virtual IList<CssDeclaration> ResolveShorthand(String shorthandExpression) {
            if (CssTypesValidationUtils.IsInitialOrInheritOrUnset(shorthandExpression)) {
                return JavaUtil.ArraysAsList(new CssDeclaration(CommonCssConstants.BACKGROUND_POSITION_X, shorthandExpression
                    ), new CssDeclaration(CommonCssConstants.BACKGROUND_POSITION_Y, shorthandExpression));
            }
            if (String.IsNullOrEmpty(shorthandExpression.Trim())) {
                LOGGER.LogWarning(MessageFormatUtil.Format(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.SHORTHAND_PROPERTY_CANNOT_BE_EMPTY
                    , CommonCssConstants.BACKGROUND_POSITION));
                return new List<CssDeclaration>();
            }
            IList<IList<String>> propsList = CssUtils.ExtractShorthandProperties(shorthandExpression);
            IDictionary<String, String> resolvedProps = new Dictionary<String, String>();
            IDictionary<String, String> values = new Dictionary<String, String>();
            foreach (IList<String> props in propsList) {
                if (props.IsEmpty()) {
                    LOGGER.LogWarning(MessageFormatUtil.Format(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.SHORTHAND_PROPERTY_CANNOT_BE_EMPTY
                        , CommonCssConstants.BACKGROUND_POSITION));
                    return new List<CssDeclaration>();
                }
                if (!ParsePositionShorthand(props, values)) {
                    LOGGER.LogWarning(MessageFormatUtil.Format(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.INVALID_CSS_PROPERTY_DECLARATION
                        , shorthandExpression));
                    return new List<CssDeclaration>();
                }
                UpdateValue(resolvedProps, values, CommonCssConstants.BACKGROUND_POSITION_X);
                UpdateValue(resolvedProps, values, CommonCssConstants.BACKGROUND_POSITION_Y);
                values.Clear();
            }
            if (!CheckProperty(resolvedProps, CommonCssConstants.BACKGROUND_POSITION_X) || !CheckProperty(resolvedProps
                , CommonCssConstants.BACKGROUND_POSITION_Y)) {
                return new List<CssDeclaration>();
            }
            return JavaUtil.ArraysAsList(new CssDeclaration(CommonCssConstants.BACKGROUND_POSITION_X, resolvedProps.Get
                (CommonCssConstants.BACKGROUND_POSITION_X)), new CssDeclaration(CommonCssConstants.BACKGROUND_POSITION_Y
                , resolvedProps.Get(CommonCssConstants.BACKGROUND_POSITION_Y)));
        }

        private static bool CheckProperty(IDictionary<String, String> resolvedProps, String key) {
            if (!CssDeclarationValidationMaster.CheckDeclaration(new CssDeclaration(key, resolvedProps.Get(key)))) {
                LOGGER.LogWarning(MessageFormatUtil.Format(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.INVALID_CSS_PROPERTY_DECLARATION
                    , resolvedProps.Get(key)));
                return false;
            }
            return true;
        }

        private static void UpdateValue(IDictionary<String, String> resolvedProps, IDictionary<String, String> values
            , String key) {
            if (values.Get(key) == null) {
                if (resolvedProps.Get(key) == null) {
                    resolvedProps.Put(key, CommonCssConstants.CENTER);
                }
                else {
                    resolvedProps.Put(key, resolvedProps.Get(key) + "," + CommonCssConstants.CENTER);
                }
            }
            else {
                if (resolvedProps.Get(key) == null) {
                    resolvedProps.Put(key, values.Get(key));
                }
                else {
                    resolvedProps.Put(key, resolvedProps.Get(key) + "," + values.Get(key));
                }
            }
        }

        private static bool ParsePositionShorthand(IList<String> valuesToParse, IDictionary<String, String> parsedValues
            ) {
            foreach (String positionValue in valuesToParse) {
                if (!ParseNonNumericValue(positionValue, parsedValues)) {
                    return false;
                }
            }
            for (int i = 0; i < valuesToParse.Count; i++) {
                if (TypeOfValue(valuesToParse[i]) == BackgroundPositionShorthandResolver.BackgroundPositionType.NUMERIC &&
                     !ParseNumericValue(i, valuesToParse, parsedValues)) {
                    return false;
                }
            }
            return true;
        }

        private static bool ParseNumericValue(int i, IList<String> positionValues, IDictionary<String, String> values
            ) {
            if (values.Get(CommonCssConstants.BACKGROUND_POSITION_X) == null || values.Get(CommonCssConstants.BACKGROUND_POSITION_Y
                ) == null) {
                return ParseShortNumericValue(i, positionValues, values, positionValues[i]);
            }
            if (i == 0) {
                return false;
            }
            return ParseLargeNumericValue(positionValues[i - 1], values, positionValues[i]);
        }

        // Parses shorthand with one or less background-position keywords.
        private static bool ParseShortNumericValue(int i, IList<String> positionValues, IDictionary<String, String
            > values, String value) {
            if (positionValues.Count > POSITION_VALUES_MAX_COUNT) {
                return false;
            }
            if (values.Get(CommonCssConstants.BACKGROUND_POSITION_X) == null) {
                if (i != 0) {
                    return false;
                }
                values.Put(CommonCssConstants.BACKGROUND_POSITION_X, value);
                return true;
            }
            if (i == 0) {
                if (TypeOfValue(positionValues[i + 1]) == BackgroundPositionShorthandResolver.BackgroundPositionType.CENTER
                    ) {
                    values.Put(CommonCssConstants.BACKGROUND_POSITION_X, value);
                    values.Put(CommonCssConstants.BACKGROUND_POSITION_Y, CommonCssConstants.CENTER);
                    return true;
                }
                return false;
            }
            values.Put(CommonCssConstants.BACKGROUND_POSITION_Y, value);
            return true;
        }

        // Parses shorthand with two background-position keywords.
        private static bool ParseLargeNumericValue(String prevValue, IDictionary<String, String> values, String value
            ) {
            if (TypeOfValue(prevValue) == BackgroundPositionShorthandResolver.BackgroundPositionType.HORIZONTAL_POSITION
                ) {
                values.Put(CommonCssConstants.BACKGROUND_POSITION_X, values.Get(CommonCssConstants.BACKGROUND_POSITION_X) 
                    + " " + value);
                return true;
            }
            if (TypeOfValue(prevValue) == BackgroundPositionShorthandResolver.BackgroundPositionType.VERTICAL_POSITION
                ) {
                values.Put(CommonCssConstants.BACKGROUND_POSITION_Y, values.Get(CommonCssConstants.BACKGROUND_POSITION_Y) 
                    + " " + value);
                return true;
            }
            return false;
        }

        private static bool ParseNonNumericValue(String positionValue, IDictionary<String, String> values) {
            switch (TypeOfValue(positionValue)) {
                case BackgroundPositionShorthandResolver.BackgroundPositionType.HORIZONTAL_POSITION: {
                    return ParseHorizontal(positionValue, values);
                }

                case BackgroundPositionShorthandResolver.BackgroundPositionType.VERTICAL_POSITION: {
                    return ParseVertical(positionValue, values);
                }

                case BackgroundPositionShorthandResolver.BackgroundPositionType.CENTER: {
                    return ParseCenter(positionValue, values);
                }

                default: {
                    return true;
                }
            }
        }

        private static bool ParseHorizontal(String positionValue, IDictionary<String, String> values) {
            if (values.Get(CommonCssConstants.BACKGROUND_POSITION_X) == null) {
                values.Put(CommonCssConstants.BACKGROUND_POSITION_X, positionValue);
                return true;
            }
            if (CommonCssConstants.CENTER.Equals(values.Get(CommonCssConstants.BACKGROUND_POSITION_X)) && values.Get(CommonCssConstants
                .BACKGROUND_POSITION_Y) == null) {
                values.Put(CommonCssConstants.BACKGROUND_POSITION_X, positionValue);
                values.Put(CommonCssConstants.BACKGROUND_POSITION_Y, CommonCssConstants.CENTER);
                return true;
            }
            return false;
        }

        private static bool ParseVertical(String positionValue, IDictionary<String, String> values) {
            if (values.Get(CommonCssConstants.BACKGROUND_POSITION_Y) == null) {
                values.Put(CommonCssConstants.BACKGROUND_POSITION_Y, positionValue);
                return true;
            }
            return false;
        }

        private static bool ParseCenter(String positionValue, IDictionary<String, String> values) {
            if (values.Get(CommonCssConstants.BACKGROUND_POSITION_X) == null) {
                values.Put(CommonCssConstants.BACKGROUND_POSITION_X, positionValue);
                return true;
            }
            if (values.Get(CommonCssConstants.BACKGROUND_POSITION_Y) == null) {
                values.Put(CommonCssConstants.BACKGROUND_POSITION_Y, positionValue);
                return true;
            }
            return false;
        }

        private static BackgroundPositionShorthandResolver.BackgroundPositionType TypeOfValue(String value) {
            if (CommonCssConstants.LEFT.Equals(value) || CommonCssConstants.RIGHT.Equals(value)) {
                return BackgroundPositionShorthandResolver.BackgroundPositionType.HORIZONTAL_POSITION;
            }
            if (CommonCssConstants.TOP.Equals(value) || CommonCssConstants.BOTTOM.Equals(value)) {
                return BackgroundPositionShorthandResolver.BackgroundPositionType.VERTICAL_POSITION;
            }
            if (CommonCssConstants.CENTER.Equals(value)) {
                return BackgroundPositionShorthandResolver.BackgroundPositionType.CENTER;
            }
            return BackgroundPositionShorthandResolver.BackgroundPositionType.NUMERIC;
        }

        private enum BackgroundPositionType {
            NUMERIC,
            HORIZONTAL_POSITION,
            VERTICAL_POSITION,
            CENTER
        }
    }
}
