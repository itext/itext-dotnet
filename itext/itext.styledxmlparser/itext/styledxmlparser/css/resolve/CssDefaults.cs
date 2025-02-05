/*
This file is part of the iText (R) project.
Copyright (c) 1998-2025 Apryse Group NV
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

namespace iText.StyledXmlParser.Css.Resolve {
    /// <summary>Helper class that allows you to get the default values of CSS properties.</summary>
    public class CssDefaults {
        /// <summary>A map with properties and their default values.</summary>
        private static readonly IDictionary<String, String> defaultValues = new Dictionary<String, String>();

        static CssDefaults() {
            defaultValues.Put(CommonCssConstants.COLOR, "black");
            // not specified, varies from browser to browser
            defaultValues.Put(CommonCssConstants.OPACITY, "1");
            defaultValues.Put(CommonCssConstants.BACKGROUND_ATTACHMENT, CommonCssConstants.SCROLL);
            defaultValues.Put(CommonCssConstants.BACKGROUND_BLEND_MODE, CommonCssConstants.NORMAL);
            defaultValues.Put(CommonCssConstants.BACKGROUND_COLOR, CommonCssConstants.TRANSPARENT);
            defaultValues.Put(CommonCssConstants.BACKGROUND_IMAGE, CommonCssConstants.NONE);
            defaultValues.Put(CommonCssConstants.BACKGROUND_POSITION, "0% 0%");
            defaultValues.Put(CommonCssConstants.BACKGROUND_POSITION_X, "0%");
            defaultValues.Put(CommonCssConstants.BACKGROUND_POSITION_Y, "0%");
            defaultValues.Put(CommonCssConstants.BACKGROUND_REPEAT, CommonCssConstants.REPEAT);
            defaultValues.Put(CommonCssConstants.BACKGROUND_CLIP, CommonCssConstants.BORDER_BOX);
            defaultValues.Put(CommonCssConstants.BACKGROUND_ORIGIN, CommonCssConstants.PADDING_BOX);
            defaultValues.Put(CommonCssConstants.BACKGROUND_SIZE, CommonCssConstants.AUTO);
            defaultValues.Put(CommonCssConstants.BORDER_BOTTOM_COLOR, CommonCssConstants.CURRENTCOLOR);
            defaultValues.Put(CommonCssConstants.BORDER_LEFT_COLOR, CommonCssConstants.CURRENTCOLOR);
            defaultValues.Put(CommonCssConstants.BORDER_RIGHT_COLOR, CommonCssConstants.CURRENTCOLOR);
            defaultValues.Put(CommonCssConstants.BORDER_TOP_COLOR, CommonCssConstants.CURRENTCOLOR);
            defaultValues.Put(CommonCssConstants.BORDER_BOTTOM_STYLE, CommonCssConstants.NONE);
            defaultValues.Put(CommonCssConstants.BORDER_LEFT_STYLE, CommonCssConstants.NONE);
            defaultValues.Put(CommonCssConstants.BORDER_RIGHT_STYLE, CommonCssConstants.NONE);
            defaultValues.Put(CommonCssConstants.BORDER_TOP_STYLE, CommonCssConstants.NONE);
            defaultValues.Put(CommonCssConstants.BORDER_BOTTOM_WIDTH, CommonCssConstants.MEDIUM);
            defaultValues.Put(CommonCssConstants.BORDER_LEFT_WIDTH, CommonCssConstants.MEDIUM);
            defaultValues.Put(CommonCssConstants.BORDER_RIGHT_WIDTH, CommonCssConstants.MEDIUM);
            defaultValues.Put(CommonCssConstants.BORDER_TOP_WIDTH, CommonCssConstants.MEDIUM);
            defaultValues.Put(CommonCssConstants.BORDER_WIDTH, CommonCssConstants.MEDIUM);
            defaultValues.Put(CommonCssConstants.BORDER_IMAGE, CommonCssConstants.NONE);
            defaultValues.Put(CommonCssConstants.BORDER_RADIUS, "0");
            defaultValues.Put(CommonCssConstants.BORDER_BOTTOM_LEFT_RADIUS, "0");
            defaultValues.Put(CommonCssConstants.BORDER_BOTTOM_RIGHT_RADIUS, "0");
            defaultValues.Put(CommonCssConstants.BORDER_TOP_LEFT_RADIUS, "0");
            defaultValues.Put(CommonCssConstants.BORDER_TOP_RIGHT_RADIUS, "0");
            defaultValues.Put(CommonCssConstants.BOX_SHADOW, CommonCssConstants.NONE);
            defaultValues.Put(CommonCssConstants.FLEX_BASIS, CommonCssConstants.AUTO);
            defaultValues.Put(CommonCssConstants.FLEX_DIRECTION, CommonCssConstants.ROW);
            defaultValues.Put(CommonCssConstants.FLEX_GROW, "0");
            defaultValues.Put(CommonCssConstants.FLEX_SHRINK, "1");
            defaultValues.Put(CommonCssConstants.FLEX_WRAP, CommonCssConstants.NOWRAP);
            defaultValues.Put(CommonCssConstants.FLOAT, CommonCssConstants.NONE);
            defaultValues.Put(CommonCssConstants.FONT_FAMILY, "times");
            defaultValues.Put(CommonCssConstants.FONT_SIZE, CommonCssConstants.MEDIUM);
            defaultValues.Put(CommonCssConstants.FONT_STYLE, CommonCssConstants.NORMAL);
            defaultValues.Put(CommonCssConstants.FONT_VARIANT, CommonCssConstants.NORMAL);
            defaultValues.Put(CommonCssConstants.FONT_WEIGHT, CommonCssConstants.NORMAL);
            defaultValues.Put(CommonCssConstants.HEIGHT, CommonCssConstants.AUTO);
            defaultValues.Put(CommonCssConstants.HYPHENS, CommonCssConstants.MANUAL);
            defaultValues.Put(CommonCssConstants.LINE_HEIGHT, CommonCssConstants.NORMAL);
            defaultValues.Put(CommonCssConstants.LIST_STYLE_TYPE, CommonCssConstants.DISC);
            defaultValues.Put(CommonCssConstants.LIST_STYLE_IMAGE, CommonCssConstants.NONE);
            defaultValues.Put(CommonCssConstants.LIST_STYLE_POSITION, CommonCssConstants.OUTSIDE);
            defaultValues.Put(CommonCssConstants.MARGIN_BOTTOM, "0");
            defaultValues.Put(CommonCssConstants.MARGIN_LEFT, "0");
            defaultValues.Put(CommonCssConstants.MARGIN_RIGHT, "0");
            defaultValues.Put(CommonCssConstants.MARGIN_TOP, "0");
            defaultValues.Put(CommonCssConstants.MIN_HEIGHT, "0");
            defaultValues.Put(CommonCssConstants.OUTLINE_COLOR, CommonCssConstants.CURRENTCOLOR);
            defaultValues.Put(CommonCssConstants.OUTLINE_STYLE, CommonCssConstants.NONE);
            defaultValues.Put(CommonCssConstants.OUTLINE_WIDTH, CommonCssConstants.MEDIUM);
            defaultValues.Put(CommonCssConstants.PADDING_BOTTOM, "0");
            defaultValues.Put(CommonCssConstants.PADDING_LEFT, "0");
            defaultValues.Put(CommonCssConstants.PADDING_RIGHT, "0");
            defaultValues.Put(CommonCssConstants.PADDING_TOP, "0");
            defaultValues.Put(CommonCssConstants.PAGE_BREAK_AFTER, CommonCssConstants.AUTO);
            defaultValues.Put(CommonCssConstants.PAGE_BREAK_BEFORE, CommonCssConstants.AUTO);
            defaultValues.Put(CommonCssConstants.PAGE_BREAK_INSIDE, CommonCssConstants.AUTO);
            defaultValues.Put(CommonCssConstants.POSITION, CommonCssConstants.STATIC);
            defaultValues.Put(CommonCssConstants.QUOTES, "\"\\00ab\" \"\\00bb\"");
            defaultValues.Put(CommonCssConstants.TEXT_ALIGN, CommonCssConstants.START);
            defaultValues.Put(CommonCssConstants.TEXT_DECORATION, CommonCssConstants.NONE);
            defaultValues.Put(CommonCssConstants.TEXT_DECORATION_LINE, CommonCssConstants.NONE);
            defaultValues.Put(CommonCssConstants.TEXT_DECORATION_STYLE, CommonCssConstants.SOLID);
            defaultValues.Put(CommonCssConstants.TEXT_DECORATION_COLOR, CommonCssConstants.CURRENTCOLOR);
            defaultValues.Put(CommonCssConstants.TEXT_TRANSFORM, CommonCssConstants.NONE);
            defaultValues.Put(CommonCssConstants.WHITE_SPACE, CommonCssConstants.NORMAL);
            defaultValues.Put(CommonCssConstants.WIDTH, CommonCssConstants.AUTO);
            defaultValues.Put(CommonCssConstants.ORPHANS, "2");
            defaultValues.Put(CommonCssConstants.WIDOWS, "2");
            defaultValues.Put(CommonCssConstants.JUSTIFY_CONTENT, CommonCssConstants.FLEX_START);
            defaultValues.Put(CommonCssConstants.ALIGN_ITEMS, CommonCssConstants.STRETCH);
        }

        // Other css properties default values will be added as needed
        /// <summary>Gets the default value of a property.</summary>
        /// <param name="property">the property</param>
        /// <returns>the default value</returns>
        public static String GetDefaultValue(String property) {
            String defaultVal = defaultValues.Get(property);
            if (defaultVal == null) {
                ILogger logger = ITextLogManager.GetLogger(typeof(CssDefaults));
                logger.LogError(MessageFormatUtil.Format(iText.StyledXmlParser.Logs.StyledXmlParserLogMessageConstant.DEFAULT_VALUE_OF_CSS_PROPERTY_UNKNOWN
                    , property));
            }
            return defaultVal;
        }
    }
}
