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
using System.Text;
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
            String normalizedValue = cssValue.ToLowerInvariant().Trim();
            return normalizedValue.EndsWith(")") && (normalizedValue.StartsWith(LINEAR_GRADIENT_FUNCTION_SUFFIX) || normalizedValue
                .StartsWith(REPEATING_LINEAR_GRADIENT_FUNCTION_SUFFIX));
        }

        /// <summary>Parses the provided linear gradient or repeating linear gradient function</summary>
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
            if (IsCssLinearGradientValue(cssGradientValue)) {
                cssGradientValue = cssGradientValue.ToLowerInvariant().Trim();
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
                    IList<String> argumentsList = new List<String>();
                    StringBuilder buff = new StringBuilder();
                    CssDeclarationValueTokenizer tokenizer = new CssDeclarationValueTokenizer(argumentsPart);
                    CssDeclarationValueTokenizer.Token nextToken;
                    while ((nextToken = tokenizer.GetNextValidToken()) != null) {
                        if (nextToken.GetType() == CssDeclarationValueTokenizer.TokenType.COMMA) {
                            if (buff.Length != 0) {
                                argumentsList.Add(buff.ToString().Trim());
                                buff = new StringBuilder();
                            }
                        }
                        else {
                            buff.Append(" ").Append(nextToken.GetValue());
                        }
                    }
                    if (buff.Length != 0) {
                        argumentsList.Add(buff.ToString().Trim());
                    }
                    if (argumentsList.IsEmpty()) {
                        throw new StyledXMLParserException(MessageFormatUtil.Format(StyledXMLParserException.INVALID_GRADIENT_FUNCTION_ARGUMENTS_LIST
                            , cssGradientValue));
                    }
                    return ParseCssLinearGradient(argumentsList, isRepeating, emValue, remValue);
                }
            }
            return null;
        }

        private static StrategyBasedLinearGradientBuilder ParseCssLinearGradient(IList<String> argumentsList, bool
             isRepeating, float emValue, float remValue) {
            StrategyBasedLinearGradientBuilder builder = new StrategyBasedLinearGradientBuilder();
            GradientSpreadMethod gradientSpreadMethod = isRepeating ? GradientSpreadMethod.REPEAT : GradientSpreadMethod
                .PAD;
            builder.SetSpreadMethod(gradientSpreadMethod);
            int colorStopListStartIndex;
            String firstArgument = argumentsList[0];
            if (CssTypesValidationUtils.IsAngleValue(firstArgument)) {
                double radAngle = CssDimensionParsingUtils.ParseAngle(firstArgument);
                // we need to negate the angle as css specifies the clockwise rotation angle
                builder.SetGradientDirectionAsCentralRotationAngle(-radAngle);
                colorStopListStartIndex = 1;
            }
            else {
                if (firstArgument.StartsWith("to ")) {
                    StrategyBasedLinearGradientBuilder.GradientStrategy gradientStrategy = ParseDirection(firstArgument);
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

        private static void AddStopColors(AbstractLinearGradientBuilder builder, IList<String> argumentsList, int 
            stopsStartIndex, float emValue, float remValue) {
            GradientColorStop lastCreatedStopColor = null;
            int lastStopIndex = argumentsList.Count - 1;
            for (int i = stopsStartIndex; i <= lastStopIndex; ++i) {
                String argument = argumentsList[i];
                IList<String> elementsList = new List<String>();
                CssDeclarationValueTokenizer tokenizer = new CssDeclarationValueTokenizer(argument);
                CssDeclarationValueTokenizer.Token nextToken;
                while ((nextToken = tokenizer.GetNextValidToken()) != null) {
                    elementsList.Add(nextToken.GetValue());
                }
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
                        builder.AddColorStop(lastCreatedStopColor);
                    }
                    else {
                        for (int j = 1; j < elementsList.Count; ++j) {
                            if (CssTypesValidationUtils.IsNumber(elementsList[j])) {
                                // the numeric value is invalid in linear gradient function.
                                // So check it here as parsing method will use the default pt metric
                                throw new StyledXMLParserException(MessageFormatUtil.Format(StyledXMLParserException.INVALID_GRADIENT_COLOR_STOP_VALUE
                                    , argument));
                            }
                            UnitValue offset = CssDimensionParsingUtils.ParseLengthValueToPt(elementsList[j], emValue, remValue);
                            if (offset == null) {
                                throw new StyledXMLParserException(MessageFormatUtil.Format(StyledXMLParserException.INVALID_GRADIENT_COLOR_STOP_VALUE
                                    , argument));
                            }
                            lastCreatedStopColor = CreateStopColor(rgba, offset);
                            builder.AddColorStop(lastCreatedStopColor);
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
                    UnitValue hint = CssDimensionParsingUtils.ParseLengthValueToPt(elementsList[0], emValue, remValue);
                    if (hint == null) {
                        throw new StyledXMLParserException(MessageFormatUtil.Format(StyledXMLParserException.INVALID_GRADIENT_COLOR_STOP_VALUE
                            , argument));
                    }
                    if (hint.GetUnitType() == UnitValue.PERCENT) {
                        lastCreatedStopColor.SetHint(hint.GetValue() / 100, GradientColorStop.HintOffsetType.RELATIVE_ON_GRADIENT);
                    }
                    else {
                        lastCreatedStopColor.SetHint(hint.GetValue(), GradientColorStop.HintOffsetType.ABSOLUTE_ON_GRADIENT);
                    }
                }
            }
        }

        private static StrategyBasedLinearGradientBuilder.GradientStrategy ParseDirection(String argument) {
            String[] elementsList = iText.Commons.Utils.StringUtil.Split(argument, "\\s+");
            if (elementsList.Length < 2) {
                throw new StyledXMLParserException(MessageFormatUtil.Format(StyledXMLParserException.INVALID_GRADIENT_TO_SIDE_OR_CORNER_STRING
                    , argument));
            }
            int topCount = 0;
            int bottomCount = 0;
            int leftCount = 0;
            int rightCount = 0;
            for (int i = 1; i < elementsList.Length; ++i) {
                if (CommonCssConstants.TOP.Equals(elementsList[i])) {
                    ++topCount;
                }
                else {
                    if (CommonCssConstants.BOTTOM.Equals(elementsList[i])) {
                        ++bottomCount;
                    }
                    else {
                        if (CommonCssConstants.LEFT.Equals(elementsList[i])) {
                            ++leftCount;
                        }
                        else {
                            if (CommonCssConstants.RIGHT.Equals(elementsList[i])) {
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
            // TODO: DEVSIX-4136 when opacity would be implemented - check the 4th element of
            //  the rgba array and use it as the opacity
            return new GradientColorStop(rgba, offsetValue, offsetType);
        }
    }
}
