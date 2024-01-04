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
using iText.Commons.Utils;
using iText.StyledXmlParser.Css;

namespace iText.StyledXmlParser.Css.Resolve {
    /// <summary>Helper class that allows you to check if a property is inheritable.</summary>
    public class CssInheritance : IStyleInheritance {
        /// <summary>
        /// Set of inheritable properties
        /// in accordance with "http://www.w3schools.com/cssref/"
        /// and "https://developer.mozilla.org/en-US/docs/Web/CSS/Reference"
        /// </summary>
        private static readonly ICollection<String> INHERITABLE_PROPERTIES = JavaCollectionsUtil.UnmodifiableSet(new 
            HashSet<String>(JavaUtil.ArraysAsList(CommonCssConstants.COLOR, CommonCssConstants.VISIBILITY, CommonCssConstants
            .HANGING_PUNCTUATION, CommonCssConstants.HYPHENS, CommonCssConstants.LETTER_SPACING, CommonCssConstants
            .LINE_HEIGHT, CommonCssConstants.OVERFLOW_WRAP, CommonCssConstants.TAB_SIZE, CommonCssConstants.TEXT_ALIGN
            , CommonCssConstants.TEXT_ALIGN_LAST, CommonCssConstants.TEXT_INDENT, CommonCssConstants.TEXT_JUSTIFY, 
            CommonCssConstants.TEXT_TRANSFORM, CommonCssConstants.WHITE_SPACE, CommonCssConstants.WORD_BREAK, CommonCssConstants
            .WORD_SPACING, CommonCssConstants.WORDWRAP, CommonCssConstants.TEXT_SHADOW, CommonCssConstants.TEXT_UNDERLINE_POSITION
            , CommonCssConstants.FONT, CommonCssConstants.FONT_FAMILY, CommonCssConstants.FONT_FEATURE_SETTINGS, CommonCssConstants
            .FONT_KERNING, CommonCssConstants.FONT_LANGUAGE_OVERRIDE, CommonCssConstants.FONT_SIZE, CommonCssConstants
            .FONT_SIZE_ADJUST, CommonCssConstants.FONT_STRETCH, CommonCssConstants.FONT_STYLE, CommonCssConstants.
            FONT_SYNTHESIS, CommonCssConstants.FONT_VARIANT, CommonCssConstants.FONT_VARIANT_ALTERNATES, CommonCssConstants
            .FONT_VARIANT_CAPS, CommonCssConstants.FONT_VARIANT_EAST_ASIAN, CommonCssConstants.FONT_VARIANT_LIGATURES
            , CommonCssConstants.FONT_VARIANT_NUMERIC, CommonCssConstants.FONT_VARIANT_POSITION, CommonCssConstants
            .FONT_WEIGHT, CommonCssConstants.DIRECTION, CommonCssConstants.TEXT_ORIENTATION, CommonCssConstants.TEXT_COMBINE_UPRIGHT
            , CommonCssConstants.UNICODE_BIDI, CommonCssConstants.WRITING_MODE, CommonCssConstants.BORDER_COLLAPSE
            , CommonCssConstants.BORDER_SPACING, CommonCssConstants.CAPTION_SIDE, CommonCssConstants.EMPTY_CELLS, 
            CommonCssConstants.LIST_STYLE, CommonCssConstants.LIST_STYLE_IMAGE, CommonCssConstants.LIST_STYLE_POSITION
            , CommonCssConstants.LIST_STYLE_TYPE, CommonCssConstants.QUOTES, CommonCssConstants.ORPHANS, CommonCssConstants
            .WIDOWS)));

        // Color Properties
        // Basic Box Properties
        // Text Properties
        // Text Decoration Properties
        // Font Properties
        // Writing Modes Properties
        // Table Properties
        // Lists and Counters Properties
        // Generated Content for Paged Media
        /// <summary>Checks if a property is inheritable.</summary>
        /// <param name="cssProperty">the CSS property</param>
        /// <returns>true, if the property is inheritable</returns>
        public virtual bool IsInheritable(String cssProperty) {
            return INHERITABLE_PROPERTIES.Contains(cssProperty);
        }
    }
}
