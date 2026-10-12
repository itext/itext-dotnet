/*
This file is part of the iText (R) project.
Copyright (c) 1998-2026 Apryse Group NV
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
using System.Text;
using iText.Commons.Internal.Runtime;
using iText.Commons.Utils;
using iText.Kernel.Colors.Gradients;
using iText.Layout.Properties;
using iText.StyledXmlParser.Css;
using iText.StyledXmlParser.Css.Parse;
using iText.StyledXmlParser.Exceptions;

namespace iText.StyledXmlParser.Css.Util {
    /// <summary>Utilities class for CSS gradient functions parsing.</summary>
    public sealed class CssGradientUtil {
        private const String LINEAR_GRADIENT_FUNCTION_SUFFIX = "linear-gradient(";

        private const String REPEATING_LINEAR_GRADIENT_FUNCTION_SUFFIX = "repeating-" + LINEAR_GRADIENT_FUNCTION_SUFFIX;

        private const String RADIAL_GRADIENT_FUNCTION_SUFFIX = "radial-gradient(";

        private const String REPEATING_RADIAL_GRADIENT_FUNCTION_SUFFIX = "repeating-" + RADIAL_GRADIENT_FUNCTION_SUFFIX;

        private static readonly IList<String> RADIAL_EXTENT_KEYWORDS = JavaCollectionsUtil.UnmodifiableList(JavaUtil.ArraysAsList
            (CommonCssConstants.CLOSEST_CORNER, CommonCssConstants.CLOSEST_SIDE, CommonCssConstants.FARTHEST_CORNER
            , CommonCssConstants.FARTHEST_SIDE));

        private static readonly IDictionary<String, StrategyBasedRadialGradientBuilder.GradientStrategy> RADIAL_EXTENT_STRATEGIES
             = CreateRadialExtentStrategies();

        private CssGradientUtil() {
        }

        /// <summary>Checks whether the provided value is a linear gradient or repeating linear gradient function.</summary>
        /// <remarks>
        /// Checks whether the provided value is a linear gradient or repeating linear gradient function.
        /// This method does not check the validity of arguments list.
        /// </remarks>
        /// <param name="cssValue">the value to check</param>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if the provided argument is the linear gradient
        /// or repeating linear gradient function (even if the arguments list is invalid)
        /// </returns>
        public static bool IsCssLinearGradientValue(String cssValue) {
            if (cssValue == null) {
                return false;
            }
            String normalizedValue = StringNormalizer.Normalize(cssValue);
            return normalizedValue.EndsWith(")") && (normalizedValue.StartsWith(LINEAR_GRADIENT_FUNCTION_SUFFIX) || normalizedValue
                .StartsWith(REPEATING_LINEAR_GRADIENT_FUNCTION_SUFFIX));
        }

        /// <summary>Checks whether the provided value is a radial gradient or repeating radial gradient function.</summary>
        /// <remarks>
        /// Checks whether the provided value is a radial gradient or repeating radial gradient function.
        /// This method does not check the validity of arguments list.
        /// </remarks>
        /// <param name="cssValue">the value to check</param>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if the provided argument is the radial gradient
        /// or repeating radial gradient function (even if the arguments list is invalid)
        /// </returns>
        public static bool IsCssRadialGradientValue(String cssValue) {
            if (cssValue == null) {
                return false;
            }
            String normalizedValue = StringNormalizer.Normalize(cssValue);
            return normalizedValue.EndsWith(")") && (normalizedValue.StartsWith(RADIAL_GRADIENT_FUNCTION_SUFFIX) || normalizedValue
                .StartsWith(REPEATING_RADIAL_GRADIENT_FUNCTION_SUFFIX));
        }

        /// <summary>Checks whether the provided value is a gradient function (linear or radial).</summary>
        /// <remarks>
        /// Checks whether the provided value is a gradient function (linear or radial).
        /// This method does not check the validity of arguments list.
        /// </remarks>
        /// <param name="cssValue">the value to check</param>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if the provided argument is a gradient function
        /// </returns>
        public static bool IsCssGradientValue(String cssValue) {
            return IsCssLinearGradientValue(cssValue) || IsCssRadialGradientValue(cssValue);
        }

        /// <summary>Parses the provided gradient function (linear or radial).</summary>
        /// <param name="cssGradientValue">the value to parse</param>
        /// <param name="emValue">the current element's em value</param>
        /// <param name="remValue">the current element's rem value</param>
        /// <returns>
        /// the
        /// <see cref="iText.Kernel.Colors.Gradients.IGradientBuilder"/>
        /// constructed from the parsed gradient
        /// or
        /// <see langword="null"/>
        /// if the argument value is not a gradient function
        /// </returns>
        public static IGradientBuilder ParseCssGradient(String cssGradientValue, float emValue, float remValue) {
            if (IsCssLinearGradientValue(cssGradientValue)) {
                return ParseCssLinearGradient(cssGradientValue, emValue, remValue);
            }
            else {
                if (IsCssRadialGradientValue(cssGradientValue)) {
                    return ParseCssRadialGradient(cssGradientValue, emValue, remValue);
                }
            }
            return null;
        }

        /// <summary>Parses the provided linear gradient or repeating linear gradient function.</summary>
        /// <param name="cssGradientValue">the value to parse</param>
        /// <param name="emValue">the current element's em value</param>
        /// <param name="remValue">the current element's rem value</param>
        /// <returns>
        /// the
        /// <see cref="iText.Kernel.Colors.Gradients.StrategyBasedLinearGradientBuilder"/>
        /// constructed from the parsed linear gradient
        /// or
        /// <see langword="null"/>
        /// if the argument value is not a linear gradient or repeating linear gradient
        /// function
        /// </returns>
        public static StrategyBasedLinearGradientBuilder ParseCssLinearGradient(String cssGradientValue, float emValue
            , float remValue) {
            if (!IsCssLinearGradientValue(cssGradientValue)) {
                return null;
            }
            cssGradientValue = StringNormalizer.Normalize(cssGradientValue);
            bool isRepeating = false;
            String argumentsPart = null;
            if (cssGradientValue.StartsWith(LINEAR_GRADIENT_FUNCTION_SUFFIX)) {
                argumentsPart = cssGradientValue.JSubstring(LINEAR_GRADIENT_FUNCTION_SUFFIX.Length, cssGradientValue.Length
                     - 1);
                isRepeating = false;
            }
            else {
                if (cssGradientValue.StartsWith(REPEATING_LINEAR_GRADIENT_FUNCTION_SUFFIX)) {
                    argumentsPart = cssGradientValue.JSubstring(REPEATING_LINEAR_GRADIENT_FUNCTION_SUFFIX.Length, cssGradientValue
                        .Length - 1);
                    isRepeating = true;
                }
            }
            if (argumentsPart != null) {
                IList<String> argumentsList = ExtractGradientArguments(argumentsPart, cssGradientValue);
                return ParseCssLinearGradient(argumentsList, isRepeating, emValue, remValue);
            }
            return null;
        }

        /// <summary>Parses the provided radial gradient or repeating radial gradient function.</summary>
        /// <param name="cssGradientValue">the value to parse</param>
        /// <param name="emValue">the current element's em value</param>
        /// <param name="remValue">the current element's rem value</param>
        /// <returns>
        /// the
        /// <see cref="iText.Kernel.Colors.Gradients.StrategyBasedRadialGradientBuilder"/>
        /// constructed from the parsed radial gradient
        /// or
        /// <see langword="null"/>
        /// if the argument value is not a radial gradient or repeating radial gradient function
        /// </returns>
        public static StrategyBasedRadialGradientBuilder ParseCssRadialGradient(String cssGradientValue, float emValue
            , float remValue) {
            if (!IsCssRadialGradientValue(cssGradientValue)) {
                return null;
            }
            cssGradientValue = StringNormalizer.Normalize(cssGradientValue);
            bool isRepeating;
            String argumentsPart;
            if (cssGradientValue.StartsWith(RADIAL_GRADIENT_FUNCTION_SUFFIX)) {
                argumentsPart = cssGradientValue.JSubstring(RADIAL_GRADIENT_FUNCTION_SUFFIX.Length, cssGradientValue.Length
                     - 1);
                isRepeating = false;
            }
            else {
                if (cssGradientValue.StartsWith(REPEATING_RADIAL_GRADIENT_FUNCTION_SUFFIX)) {
                    argumentsPart = cssGradientValue.JSubstring(REPEATING_RADIAL_GRADIENT_FUNCTION_SUFFIX.Length, cssGradientValue
                        .Length - 1);
                    isRepeating = true;
                }
                else {
                    return null;
                }
            }
            IList<String> argumentsList = ExtractGradientArguments(argumentsPart, cssGradientValue);
            return ParseCssRadialGradient(argumentsList, isRepeating, emValue, remValue, cssGradientValue);
        }

        private static StrategyBasedRadialGradientBuilder ParseCssRadialGradient(IList<String> argumentsList, bool
             isRepeating, float emValue, float remValue, String originalCssValue) {
            StrategyBasedRadialGradientBuilder builder = new StrategyBasedRadialGradientBuilder();
            builder.SetSpread(isRepeating ? GradientSpreadMethod.REPEAT : GradientSpreadMethod.PAD);
            String firstArgument = argumentsList[0];
            IList<String> firstArgumentTokens = Tokenize(firstArgument);
            int colorStopListStartIndex = 0;
            if (firstArgumentTokens.IsEmpty()) {
                if (argumentsList.Count <= 1) {
                    throw new StyledXMLParserException(MessageFormatUtil.Format(StyledXMLParserException.INVALID_GRADIENT_FUNCTION_ARGUMENTS_LIST
                        , originalCssValue));
                }
            }
            else {
                if (!CssTypesValidationUtils.IsColorProperty(firstArgumentTokens[0])) {
                    if (argumentsList.Count <= 1) {
                        throw new StyledXMLParserException(MessageFormatUtil.Format(StyledXMLParserException.INVALID_GRADIENT_FUNCTION_ARGUMENTS_LIST
                            , originalCssValue));
                    }
                    ParseRadialShapeSizeAndPosition(builder, firstArgumentTokens, firstArgument, emValue, remValue);
                    colorStopListStartIndex = 1;
                }
            }
            AddStopColors(builder, argumentsList, colorStopListStartIndex, emValue, remValue);
            return builder;
        }

        private static IList<String> ExtractGradientArguments(String argumentsPart, String cssGradientValue) {
            IList<String> argumentsList = new List<String>();
            StringBuilder buff = new StringBuilder();
            CssDeclarationValueTokenizer tokenizer = new CssDeclarationValueTokenizer(argumentsPart);
            CssDeclarationValueTokenizer.Token nextToken;
            while ((nextToken = tokenizer.GetNextValidToken()) != null) {
                if (nextToken.GetType() == CssDeclarationValueTokenizer.TokenType.COMMA) {
                    String argument = buff.ToString().Trim();
                    if (String.IsNullOrEmpty(argument)) {
                        throw new StyledXMLParserException(MessageFormatUtil.Format(StyledXMLParserException.INVALID_GRADIENT_FUNCTION_ARGUMENTS_LIST
                            , cssGradientValue));
                    }
                    argumentsList.Add(argument);
                    buff.Length = 0;
                }
                else {
                    buff.Append(" ").Append(nextToken.GetValue());
                }
            }
            String lastArgument = buff.ToString().Trim();
            if (String.IsNullOrEmpty(lastArgument)) {
                throw new StyledXMLParserException(MessageFormatUtil.Format(StyledXMLParserException.INVALID_GRADIENT_FUNCTION_ARGUMENTS_LIST
                    , cssGradientValue));
            }
            argumentsList.Add(lastArgument);
            return argumentsList;
        }

        private static StrategyBasedLinearGradientBuilder ParseCssLinearGradient(IList<String> argumentsList, bool
             isRepeating, float emValue, float remValue) {
            StrategyBasedLinearGradientBuilder builder = new StrategyBasedLinearGradientBuilder();
            GradientSpreadMethod gradientSpreadMethod = isRepeating ? GradientSpreadMethod.REPEAT : GradientSpreadMethod
                .PAD;
            builder.SetSpread(gradientSpreadMethod);
            int colorStopListStartIndex;
            String firstArgument = argumentsList[0];
            IList<String> firstArgumentTokens = Tokenize(firstArgument);
            if (firstArgumentTokens.Count == 1 && CssTypesValidationUtils.IsAngleValue(firstArgumentTokens[0])) {
                double radAngle = CssDimensionParsingUtils.ParseAngle(firstArgumentTokens[0]);
                // we need to negate the angle as css specifies the clockwise rotation angle
                builder.SetGradientDirectionAsCentralRotationAngle(-radAngle);
                colorStopListStartIndex = 1;
            }
            else {
                if (firstArgumentTokens.Count > 1 && "to".Equals(firstArgumentTokens[0])) {
                    StrategyBasedLinearGradientBuilder.GradientStrategy gradientStrategy = ParseDirection(firstArgumentTokens, 
                        firstArgument);
                    builder.SetGradientDirectionAsStrategy(gradientStrategy);
                    colorStopListStartIndex = 1;
                }
                else {
                    // default angle = `to bottom`
                    builder.SetGradientDirectionAsStrategy(StrategyBasedLinearGradientBuilder.GradientStrategy.TO_BOTTOM);
                    colorStopListStartIndex = 0;
                }
            }
            AddStopColors(builder, argumentsList, colorStopListStartIndex, emValue, remValue);
            return builder;
        }

        private static IList<String> Tokenize(String value) {
            IList<String> tokens = new List<String>();
            CssDeclarationValueTokenizer tokenizer = new CssDeclarationValueTokenizer(value);
            CssDeclarationValueTokenizer.Token nextToken;
            while ((nextToken = tokenizer.GetNextValidToken()) != null) {
                tokens.Add(nextToken.GetValue());
            }
            return tokens;
        }

        private static String JoinTokens(IList<String> tokens) {
            StringBuilder result = new StringBuilder();
            foreach (String token in tokens) {
                if (result.Length != 0) {
                    result.Append(' ');
                }
                result.Append(token);
            }
            return result.ToString();
        }

        private static void ParseRadialShapeSizeAndPosition(StrategyBasedRadialGradientBuilder builder, IList<String
            > tokens, String firstArgument, float emValue, float remValue) {
            int atIndex = -1;
            for (int i = 0; i < tokens.Count; ++i) {
                if (CommonCssConstants.AT.Equals(tokens[i])) {
                    if (atIndex != -1) {
                        throw new StyledXMLParserException(MessageFormatUtil.Format(StyledXMLParserException.INVALID_GRADIENT_FUNCTION_ARGUMENTS_LIST
                            , firstArgument));
                    }
                    atIndex = i;
                }
            }
            IList<String> shapeSizeTokens;
            if (atIndex == -1) {
                shapeSizeTokens = tokens;
            }
            else {
                if (atIndex == tokens.Count - 1) {
                    throw new StyledXMLParserException(MessageFormatUtil.Format(StyledXMLParserException.INVALID_GRADIENT_FUNCTION_ARGUMENTS_LIST
                        , firstArgument));
                }
                String position = JoinTokens(tokens.SubList(atIndex + 1, tokens.Count));
                CssGradientPositionUtil.ParseAndSetRadialGradientCenter(builder, position, emValue, remValue, firstArgument
                    );
                shapeSizeTokens = tokens.SubList(0, atIndex);
            }
            CssGradientUtil.RadialSize radialSize = ParseRadialSize(shapeSizeTokens, firstArgument, emValue, remValue);
            String radialShape = ParseRadialShape(shapeSizeTokens, radialSize, firstArgument);
            CombineRadialShapeAndSize(builder, radialShape, radialSize);
        }

        private static CssGradientUtil.RadialSize ParseRadialSize(IList<String> shapeSizeTokens, String fullValue, 
            float emValue, float remValue) {
            String extentKeyword = null;
            IList<UnitValue> radii = new List<UnitValue>();
            foreach (String token in shapeSizeTokens) {
                if (CommonCssConstants.CIRCLE.Equals(token) || CommonCssConstants.ELLIPSE.Equals(token)) {
                    continue;
                }
                if (RADIAL_EXTENT_KEYWORDS.Contains(token)) {
                    if (extentKeyword != null || !radii.IsEmpty()) {
                        throw new StyledXMLParserException(MessageFormatUtil.Format(StyledXMLParserException.INVALID_GRADIENT_FUNCTION_ARGUMENTS_LIST
                            , fullValue));
                    }
                    extentKeyword = token;
                }
                else {
                    if (extentKeyword != null || radii.Count == 2) {
                        throw new StyledXMLParserException(MessageFormatUtil.Format(StyledXMLParserException.INVALID_GRADIENT_FUNCTION_ARGUMENTS_LIST
                            , fullValue));
                    }
                    radii.Add(ParseRadialRadius(token, emValue, remValue, fullValue));
                }
            }
            return new CssGradientUtil.RadialSize(extentKeyword, radii);
        }

        private static String ParseRadialShape(IList<String> shapeSizeTokens, CssGradientUtil.RadialSize radialSize
            , String fullValue) {
            String shape = null;
            foreach (String token in shapeSizeTokens) {
                if (CommonCssConstants.CIRCLE.Equals(token) || CommonCssConstants.ELLIPSE.Equals(token)) {
                    if (shape != null) {
                        throw new StyledXMLParserException(MessageFormatUtil.Format(StyledXMLParserException.INVALID_GRADIENT_FUNCTION_ARGUMENTS_LIST
                            , fullValue));
                    }
                    shape = token;
                }
            }
            int radiusCount = radialSize.radii.Count;
            if (shape == null) {
                shape = radiusCount == 1 ? CommonCssConstants.CIRCLE : CommonCssConstants.ELLIPSE;
            }
            // Radius count can be 0, e.g. in "radial-gradient(circle, red, blue)".
            if ((CommonCssConstants.CIRCLE.Equals(shape) && radiusCount == 2) || (CommonCssConstants.ELLIPSE.Equals(shape
                ) && radiusCount == 1)) {
                throw new StyledXMLParserException(MessageFormatUtil.Format(StyledXMLParserException.INVALID_GRADIENT_FUNCTION_ARGUMENTS_LIST
                    , fullValue));
            }
            return shape;
        }

        private static void CombineRadialShapeAndSize(StrategyBasedRadialGradientBuilder builder, String shape, CssGradientUtil.RadialSize
             radialSize) {
            bool isCircular = CommonCssConstants.CIRCLE.Equals(shape);
            if (radialSize.extentKeyword != null) {
                // FromCenterStrategy is used because extent keywords
                // measure the radius from the gradient center to a side or corner.
                builder.SetRadiusFromCenterStrategy(isCircular, RADIAL_EXTENT_STRATEGIES.Get(radialSize.extentKeyword));
            }
            else {
                if (radialSize.radii.IsEmpty()) {
                    builder.SetRadiusFromCenterStrategy(isCircular, StrategyBasedRadialGradientBuilder.GradientStrategy.FARTHEST_CORNER
                        );
                }
                else {
                    if (radialSize.radii.Count == 1) {
                        UnitValue radius = radialSize.radii[0];
                        if (radius.GetUnitType() == UnitValue.PERCENT) {
                            builder.SetRadiusRelativeToScaledDiagonal(radius.GetValue() / 100D);
                        }
                        else {
                            builder.SetRadiusRelativeToBoundingBoxSize(radius.GetValue(), false, radius.GetValue(), false);
                        }
                    }
                    else {
                        UnitValue xRadius = radialSize.radii[0];
                        UnitValue yRadius = radialSize.radii[1];
                        bool xRelative = xRadius.GetUnitType() == UnitValue.PERCENT;
                        bool yRelative = yRadius.GetUnitType() == UnitValue.PERCENT;
                        builder.SetRadiusRelativeToBoundingBoxSize(xRelative ? (xRadius.GetValue() / 100D) : xRadius.GetValue(), xRelative
                            , yRelative ? (yRadius.GetValue() / 100D) : yRadius.GetValue(), yRelative);
                    }
                }
            }
        }

        private static UnitValue ParseRadialRadius(String token, float emValue, float remValue, String fullValue) {
            if (CssTypesValidationUtils.IsNumber(token)) {
                try {
                    if (Double.Parse(token, System.Globalization.CultureInfo.InvariantCulture) == 0D) {
                        return new UnitValue(UnitValue.POINT, 0);
                    }
                }
                catch (FormatException) {
                }
                // Fall through to the common invalid-value path.
                throw new StyledXMLParserException(MessageFormatUtil.Format(StyledXMLParserException.INVALID_GRADIENT_FUNCTION_ARGUMENTS_LIST
                    , fullValue));
            }
            UnitValue value = CssDimensionParsingUtils.ParseLengthValueToPt(token, emValue, remValue);
            if (value == null || value.GetValue() < 0) {
                throw new StyledXMLParserException(MessageFormatUtil.Format(StyledXMLParserException.INVALID_GRADIENT_FUNCTION_ARGUMENTS_LIST
                    , fullValue));
            }
            return value;
        }

        private static void AddStopColors<_T0>(AbstractGradientBuilder<_T0> builder, IList<String> argumentsList, 
            int stopsStartIndex, float emValue, float remValue) {
            GradientColorStop lastCreatedStopColor = null;
            int lastStopIndex = argumentsList.Count - 1;
            for (int i = stopsStartIndex; i <= lastStopIndex; ++i) {
                String argument = argumentsList[i];
                IList<String> elementsList = Tokenize(argument);
                // cases: color, color + offset, color + offset + offset, offset (hint)
                if (elementsList.IsEmpty() || elementsList.Count > 3) {
                    throw new StyledXMLParserException(MessageFormatUtil.Format(StyledXMLParserException.INVALID_GRADIENT_COLOR_STOP_VALUE
                        , argument));
                }
                if (CssTypesValidationUtils.IsColorProperty(elementsList[0])) {
                    float[] rgba = CssDimensionParsingUtils.ParseRgbaColor(elementsList[0]);
                    if (elementsList.Count == 1) {
                        UnitValue offset = i == stopsStartIndex ? new UnitValue(UnitValue.PERCENT, 0f) : i == lastStopIndex ? new 
                            UnitValue(UnitValue.PERCENT, 100f) : null;
                        lastCreatedStopColor = CreateStopColor(rgba, offset);
                        builder.AddStopColor(lastCreatedStopColor);
                    }
                    else {
                        for (int j = 1; j < elementsList.Count; ++j) {
                            UnitValue offset = ParseColorStopOffset(elementsList[j], emValue, remValue, argument);
                            lastCreatedStopColor = CreateStopColor(rgba, offset);
                            builder.AddStopColor(lastCreatedStopColor);
                        }
                    }
                }
                else {
                    // it should be a color hint case
                    if (elementsList.Count != 1 || lastCreatedStopColor == null || lastCreatedStopColor.GetHintOffsetType() !=
                         GradientColorStop.HintOffsetType.NONE || i == lastStopIndex) {
                        // hint is not a single value, or no color at the beginning,
                        // or two hints in a row, or hint as a last value
                        throw new StyledXMLParserException(MessageFormatUtil.Format(StyledXMLParserException.INVALID_GRADIENT_COLOR_STOP_VALUE
                            , argument));
                    }
                    UnitValue hint = ParseColorStopOffset(elementsList[0], emValue, remValue, argument);
                    if (hint.GetUnitType() == UnitValue.PERCENT) {
                        lastCreatedStopColor.SetHint(hint.GetValue() / 100, GradientColorStop.HintOffsetType.RELATIVE_ON_GRADIENT);
                    }
                    else {
                        lastCreatedStopColor.SetHint(hint.GetValue(), GradientColorStop.HintOffsetType.ABSOLUTE_ON_GRADIENT);
                    }
                }
            }
        }

        private static UnitValue ParseColorStopOffset(String value, float emValue, float remValue, String fullValue
            ) {
            if (CssTypesValidationUtils.IsNumber(value)) {
                try {
                    if (Double.Parse(value, System.Globalization.CultureInfo.InvariantCulture) == 0D) {
                        return new UnitValue(UnitValue.POINT, 0);
                    }
                }
                catch (FormatException) {
                }
                // Fall through to the invalid color-stop path.
                throw new StyledXMLParserException(MessageFormatUtil.Format(StyledXMLParserException.INVALID_GRADIENT_COLOR_STOP_VALUE
                    , fullValue));
            }
            UnitValue offset = CssDimensionParsingUtils.ParseLengthValueToPt(value, emValue, remValue);
            if (offset == null) {
                throw new StyledXMLParserException(MessageFormatUtil.Format(StyledXMLParserException.INVALID_GRADIENT_COLOR_STOP_VALUE
                    , fullValue));
            }
            return offset;
        }

        private static StrategyBasedLinearGradientBuilder.GradientStrategy ParseDirection(IList<String> tokens, String
             argument) {
            if (tokens.Count < 2) {
                throw new StyledXMLParserException(MessageFormatUtil.Format(StyledXMLParserException.INVALID_GRADIENT_TO_SIDE_OR_CORNER_STRING
                    , argument));
            }
            int topCount = 0;
            int bottomCount = 0;
            int leftCount = 0;
            int rightCount = 0;
            for (int i = 1; i < tokens.Count; ++i) {
                String token = tokens[i];
                if (CommonCssConstants.TOP.Equals(token)) {
                    ++topCount;
                }
                else {
                    if (CommonCssConstants.BOTTOM.Equals(token)) {
                        ++bottomCount;
                    }
                    else {
                        if (CommonCssConstants.LEFT.Equals(token)) {
                            ++leftCount;
                        }
                        else {
                            if (CommonCssConstants.RIGHT.Equals(token)) {
                                ++rightCount;
                            }
                            else {
                                throw new StyledXMLParserException(MessageFormatUtil.Format(StyledXMLParserException.INVALID_GRADIENT_TO_SIDE_OR_CORNER_STRING
                                    , argument));
                            }
                        }
                    }
                }
            }
            if (topCount == 1 && bottomCount == 0) {
                if (leftCount == 1 && rightCount == 0) {
                    return StrategyBasedLinearGradientBuilder.GradientStrategy.TO_TOP_LEFT;
                }
                else {
                    if (leftCount == 0 && rightCount == 1) {
                        return StrategyBasedLinearGradientBuilder.GradientStrategy.TO_TOP_RIGHT;
                    }
                    else {
                        if (leftCount == 0 && rightCount == 0) {
                            return StrategyBasedLinearGradientBuilder.GradientStrategy.TO_TOP;
                        }
                    }
                }
            }
            else {
                if (topCount == 0 && bottomCount == 1) {
                    if (leftCount == 1 && rightCount == 0) {
                        return StrategyBasedLinearGradientBuilder.GradientStrategy.TO_BOTTOM_LEFT;
                    }
                    else {
                        if (leftCount == 0 && rightCount == 1) {
                            return StrategyBasedLinearGradientBuilder.GradientStrategy.TO_BOTTOM_RIGHT;
                        }
                        else {
                            if (leftCount == 0 && rightCount == 0) {
                                return StrategyBasedLinearGradientBuilder.GradientStrategy.TO_BOTTOM;
                            }
                        }
                    }
                }
                else {
                    if (topCount == 0 && bottomCount == 0) {
                        if (leftCount == 1 && rightCount == 0) {
                            return StrategyBasedLinearGradientBuilder.GradientStrategy.TO_LEFT;
                        }
                        else {
                            if (leftCount == 0 && rightCount == 1) {
                                return StrategyBasedLinearGradientBuilder.GradientStrategy.TO_RIGHT;
                            }
                        }
                    }
                }
            }
            throw new StyledXMLParserException(MessageFormatUtil.Format(StyledXMLParserException.INVALID_GRADIENT_TO_SIDE_OR_CORNER_STRING
                , argument));
        }

        private static GradientColorStop CreateStopColor(float[] rgba, UnitValue offset) {
            GradientColorStop.OffsetType offsetType;
            double offsetValue;
            if (offset == null) {
                offsetType = GradientColorStop.OffsetType.AUTO;
                offsetValue = 0;
            }
            else {
                if (offset.GetUnitType() == UnitValue.POINT) {
                    offsetType = GradientColorStop.OffsetType.ABSOLUTE;
                    offsetValue = offset.GetValue();
                }
                else {
                    offsetType = GradientColorStop.OffsetType.RELATIVE;
                    offsetValue = offset.GetValue() / 100;
                }
            }
            return new GradientColorStop(rgba, offsetValue, offsetType);
        }

        private static IDictionary<String, StrategyBasedRadialGradientBuilder.GradientStrategy> CreateRadialExtentStrategies
            () {
            IDictionary<String, StrategyBasedRadialGradientBuilder.GradientStrategy> strategies = new Dictionary<String
                , StrategyBasedRadialGradientBuilder.GradientStrategy>();
            strategies.Put(CommonCssConstants.CLOSEST_CORNER, StrategyBasedRadialGradientBuilder.GradientStrategy.CLOSEST_CORNER
                );
            strategies.Put(CommonCssConstants.CLOSEST_SIDE, StrategyBasedRadialGradientBuilder.GradientStrategy.CLOSEST_SIDE
                );
            strategies.Put(CommonCssConstants.FARTHEST_CORNER, StrategyBasedRadialGradientBuilder.GradientStrategy.FARTHEST_CORNER
                );
            strategies.Put(CommonCssConstants.FARTHEST_SIDE, StrategyBasedRadialGradientBuilder.GradientStrategy.FARTHEST_SIDE
                );
            return JavaCollectionsUtil.UnmodifiableMap(strategies);
        }

//\cond DO_NOT_DOCUMENT
        internal sealed class RadialSize {
//\cond DO_NOT_DOCUMENT
            internal readonly String extentKeyword;
//\endcond

//\cond DO_NOT_DOCUMENT
            internal readonly IList<UnitValue> radii;
//\endcond

//\cond DO_NOT_DOCUMENT
            internal RadialSize(String extentKeyword, IList<UnitValue> radii) {
                this.extentKeyword = extentKeyword;
                this.radii = new List<UnitValue>(radii);
            }
//\endcond
        }
//\endcond
    }
}
