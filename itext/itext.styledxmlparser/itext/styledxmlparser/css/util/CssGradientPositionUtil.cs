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
using System.Text.RegularExpressions;
using iText.Commons.Internal.Runtime;
using iText.Commons.Utils;
using iText.Kernel.Colors.Gradients;
using iText.Layout.Properties;
using iText.StyledXmlParser.Css;
using iText.StyledXmlParser.Css.Resolve.Shorthand.Impl;
using iText.StyledXmlParser.Exceptions;

namespace iText.StyledXmlParser.Css.Util {
//\cond DO_NOT_DOCUMENT
    /// <summary>
    /// CSS
    /// <c>&lt;position&gt;</c>
    /// parsing for gradient functions, e.g.
    /// <c>radial-gradient(... at &lt;position&gt;)</c>.
    /// </summary>
    internal sealed class CssGradientPositionUtil {
        private static readonly Regex WHITESPACE_SPLIT_REGEX = iText.Commons.Utils.StringUtil.RegexCompile("\\s+");

        private CssGradientPositionUtil() {
        }

//\cond DO_NOT_DOCUMENT
        /// <summary>
        /// Parses a CSS
        /// <c>&lt;position&gt;</c>
        /// value and sets it as the center of the radial gradient.
        /// </summary>
        /// <param name="builder">the radial gradient builder to set the center strategy on</param>
        /// <param name="positionValue">the CSS position value</param>
        /// <param name="emValue">the current element's em value</param>
        /// <param name="remValue">the current root em value</param>
        /// <param name="fullValue">
        /// the full CSS value which contains the position, used in the exception message
        /// if the position is invalid
        /// </param>
        internal static void ParseAndSetRadialGradientCenter(StrategyBasedRadialGradientBuilder builder, String positionValue
            , float emValue, float remValue, String fullValue) {
            IList<CssDeclaration> resolved = new BackgroundPositionShorthandResolver().ResolveShorthand(positionValue);
            if (resolved.IsEmpty()) {
                throw new StyledXMLParserException(MessageFormatUtil.Format(StyledXMLParserException.INVALID_GRADIENT_FUNCTION_ARGUMENTS_LIST
                    , fullValue));
            }
            String positionX = null;
            String positionY = null;
            foreach (CssDeclaration declaration in resolved) {
                if (CommonCssConstants.BACKGROUND_POSITION_X.Equals(declaration.GetProperty())) {
                    positionX = declaration.GetExpression();
                }
                else {
                    if (CommonCssConstants.BACKGROUND_POSITION_Y.Equals(declaration.GetProperty())) {
                        positionY = declaration.GetExpression();
                    }
                }
            }
            if (positionX == null || positionY == null) {
                throw new StyledXMLParserException(MessageFormatUtil.Format(StyledXMLParserException.INVALID_GRADIENT_FUNCTION_ARGUMENTS_LIST
                    , fullValue));
            }
            CssGradientPositionUtil.ParsedPositionX parsedX = ParsePositionX(positionX, emValue, remValue, fullValue);
            CssGradientPositionUtil.ParsedPositionY parsedY = ParsePositionY(positionY, emValue, remValue, fullValue);
            BackgroundPosition position = new BackgroundPosition().SetPositionX(parsedX.position).SetXShift(parsedX.shift
                ).SetPositionY(parsedY.position).SetYShift(parsedY.shift);
            builder.SetCenterStrategy(position.GetPositionX() != BackgroundPosition.PositionX.RIGHT, GetPositionOffset
                (position.GetPositionX(), position.GetXShift()), position.GetPositionX() == BackgroundPosition.PositionX
                .CENTER || position.GetXShift().IsPercentValue(), position.GetPositionY() == BackgroundPosition.PositionY
                .BOTTOM, GetPositionOffset(position.GetPositionY(), position.GetYShift()), position.GetPositionY() == 
                BackgroundPosition.PositionY.CENTER || position.GetYShift().IsPercentValue());
        }
//\endcond

        private static CssGradientPositionUtil.ParsedPositionX ParsePositionX(String expression, float emValue, float
             remValue, String fullValue) {
            String[] parts = iText.Commons.Utils.StringUtil.Split(WHITESPACE_SPLIT_REGEX, expression.Trim());
            if (parts.Length == 1) {
                if (CommonCssConstants.LEFT.Equals(parts[0])) {
                    return new CssGradientPositionUtil.ParsedPositionX(BackgroundPosition.PositionX.LEFT, new UnitValue(UnitValue
                        .PERCENT, 0));
                }
                if (CommonCssConstants.RIGHT.Equals(parts[0])) {
                    return new CssGradientPositionUtil.ParsedPositionX(BackgroundPosition.PositionX.RIGHT, new UnitValue(UnitValue
                        .PERCENT, 0));
                }
                if (CommonCssConstants.CENTER.Equals(parts[0])) {
                    return new CssGradientPositionUtil.ParsedPositionX(BackgroundPosition.PositionX.CENTER, new UnitValue(UnitValue
                        .PERCENT, 0));
                }
                UnitValue shift = CssDimensionParsingUtils.ParseLengthValueToPt(parts[0], emValue, remValue);
                if (shift == null) {
                    throw new StyledXMLParserException(MessageFormatUtil.Format(StyledXMLParserException.INVALID_GRADIENT_FUNCTION_ARGUMENTS_LIST
                        , fullValue));
                }
                return new CssGradientPositionUtil.ParsedPositionX(BackgroundPosition.PositionX.LEFT, shift);
            }
            if (parts.Length == 2 && (CommonCssConstants.LEFT.Equals(parts[0]) || CommonCssConstants.RIGHT.Equals(parts
                [0]))) {
                UnitValue shift = CssDimensionParsingUtils.ParseLengthValueToPt(parts[1], emValue, remValue);
                if (shift == null) {
                    throw new StyledXMLParserException(MessageFormatUtil.Format(StyledXMLParserException.INVALID_GRADIENT_FUNCTION_ARGUMENTS_LIST
                        , fullValue));
                }
                BackgroundPosition.PositionX position = CommonCssConstants.LEFT.Equals(parts[0]) ? BackgroundPosition.PositionX
                    .LEFT : BackgroundPosition.PositionX.RIGHT;
                return new CssGradientPositionUtil.ParsedPositionX(position, shift);
            }
            throw new StyledXMLParserException(MessageFormatUtil.Format(StyledXMLParserException.INVALID_GRADIENT_FUNCTION_ARGUMENTS_LIST
                , fullValue));
        }

        private static CssGradientPositionUtil.ParsedPositionY ParsePositionY(String expression, float emValue, float
             remValue, String fullValue) {
            String[] parts = iText.Commons.Utils.StringUtil.Split(WHITESPACE_SPLIT_REGEX, expression.Trim());
            if (parts.Length == 1) {
                if (CommonCssConstants.TOP.Equals(parts[0])) {
                    return new CssGradientPositionUtil.ParsedPositionY(BackgroundPosition.PositionY.TOP, new UnitValue(UnitValue
                        .PERCENT, 0));
                }
                if (CommonCssConstants.BOTTOM.Equals(parts[0])) {
                    return new CssGradientPositionUtil.ParsedPositionY(BackgroundPosition.PositionY.BOTTOM, new UnitValue(UnitValue
                        .PERCENT, 0));
                }
                if (CommonCssConstants.CENTER.Equals(parts[0])) {
                    return new CssGradientPositionUtil.ParsedPositionY(BackgroundPosition.PositionY.CENTER, new UnitValue(UnitValue
                        .PERCENT, 0));
                }
                UnitValue shift = CssDimensionParsingUtils.ParseLengthValueToPt(parts[0], emValue, remValue);
                if (shift == null) {
                    throw new StyledXMLParserException(MessageFormatUtil.Format(StyledXMLParserException.INVALID_GRADIENT_FUNCTION_ARGUMENTS_LIST
                        , fullValue));
                }
                return new CssGradientPositionUtil.ParsedPositionY(BackgroundPosition.PositionY.TOP, shift);
            }
            if (parts.Length == 2 && (CommonCssConstants.TOP.Equals(parts[0]) || CommonCssConstants.BOTTOM.Equals(parts
                [0]))) {
                UnitValue shift = CssDimensionParsingUtils.ParseLengthValueToPt(parts[1], emValue, remValue);
                if (shift == null) {
                    throw new StyledXMLParserException(MessageFormatUtil.Format(StyledXMLParserException.INVALID_GRADIENT_FUNCTION_ARGUMENTS_LIST
                        , fullValue));
                }
                BackgroundPosition.PositionY position = CommonCssConstants.TOP.Equals(parts[0]) ? BackgroundPosition.PositionY
                    .TOP : BackgroundPosition.PositionY.BOTTOM;
                return new CssGradientPositionUtil.ParsedPositionY(position, shift);
            }
            throw new StyledXMLParserException(MessageFormatUtil.Format(StyledXMLParserException.INVALID_GRADIENT_FUNCTION_ARGUMENTS_LIST
                , fullValue));
        }

        private static double GetPositionOffset(BackgroundPosition.PositionX position, UnitValue shift) {
            return position == BackgroundPosition.PositionX.CENTER ? 0.5D : GetShiftValue(shift);
        }

        private static double GetPositionOffset(BackgroundPosition.PositionY position, UnitValue shift) {
            return position == BackgroundPosition.PositionY.CENTER ? 0.5D : GetShiftValue(shift);
        }

        private static double GetShiftValue(UnitValue shift) {
            return shift.IsPercentValue() ? (shift.GetValue() / 100D) : shift.GetValue();
        }

//\cond DO_NOT_DOCUMENT
        internal sealed class ParsedPositionX {
//\cond DO_NOT_DOCUMENT
            internal readonly BackgroundPosition.PositionX position;
//\endcond

//\cond DO_NOT_DOCUMENT
            internal readonly UnitValue shift;
//\endcond

//\cond DO_NOT_DOCUMENT
            internal ParsedPositionX(BackgroundPosition.PositionX position, UnitValue shift) {
                this.position = position;
                this.shift = shift;
            }
//\endcond
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal sealed class ParsedPositionY {
//\cond DO_NOT_DOCUMENT
            internal readonly BackgroundPosition.PositionY position;
//\endcond

//\cond DO_NOT_DOCUMENT
            internal readonly UnitValue shift;
//\endcond

//\cond DO_NOT_DOCUMENT
            internal ParsedPositionY(BackgroundPosition.PositionY position, UnitValue shift) {
                this.position = position;
                this.shift = shift;
            }
//\endcond
        }
//\endcond
    }
//\endcond
}
