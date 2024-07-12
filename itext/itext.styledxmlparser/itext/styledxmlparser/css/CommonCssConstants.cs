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

namespace iText.StyledXmlParser.Css {
    /// <summary>
    /// Class containing possible CSS property keys and values, pseudo element keys,
    /// units of measurement, and so on.
    /// </summary>
    public class CommonCssConstants {
        static CommonCssConstants() {
            IDictionary<String, String> keywordValues = new Dictionary<String, String>();
            keywordValues.Put(CommonCssConstants.XX_SMALL, "9px");
            keywordValues.Put(CommonCssConstants.X_SMALL, "10px");
            keywordValues.Put(CommonCssConstants.SMALL, "13px");
            keywordValues.Put(CommonCssConstants.MEDIUM, "16px");
            keywordValues.Put(CommonCssConstants.LARGE, "18px");
            keywordValues.Put(CommonCssConstants.X_LARGE, "24px");
            keywordValues.Put(CommonCssConstants.XX_LARGE, "32px");
            FONT_ABSOLUTE_SIZE_KEYWORDS_VALUES = JavaCollectionsUtil.UnmodifiableMap(keywordValues);
        }

        // properties
        /// <summary>The Constant ALIGN_CONTENT.</summary>
        public const String ALIGN_CONTENT = "align-content";

        /// <summary>The Constant ALIGN_ITEMS.</summary>
        public const String ALIGN_ITEMS = "align-items";

        /// <summary>The Constant ALIGN_SELF.</summary>
        public const String ALIGN_SELF = "align-self";

        /// <summary>The constant ATTRIBUTE.</summary>
        public const String ATTRIBUTE = "attr";

        /// <summary>The Constant BACKGROUND.</summary>
        public const String BACKGROUND = "background";

        /// <summary>The Constant BACKGROUND_ATTACHMENT.</summary>
        public const String BACKGROUND_ATTACHMENT = "background-attachment";

        /// <summary>The Constant BACKGROUND_BLEND_MODE.</summary>
        public const String BACKGROUND_BLEND_MODE = "background-blend-mode";

        /// <summary>The Constant BACKGROUND_CLIP.</summary>
        public const String BACKGROUND_CLIP = "background-clip";

        /// <summary>The Constant BACKGROUND_COLOR.</summary>
        public const String BACKGROUND_COLOR = "background-color";

        /// <summary>The Constant BACKGROUND_IMAGE.</summary>
        public const String BACKGROUND_IMAGE = "background-image";

        /// <summary>The Constant BACKGROUND_ORIGIN.</summary>
        public const String BACKGROUND_ORIGIN = "background-origin";

        /// <summary>The Constant BACKGROUND_POSITION.</summary>
        public const String BACKGROUND_POSITION = "background-position";

        /// <summary>The Constant BACKGROUND_POSITION_X.</summary>
        public const String BACKGROUND_POSITION_X = "background-position-x";

        /// <summary>The Constant BACKGROUND_POSITION_Y.</summary>
        public const String BACKGROUND_POSITION_Y = "background-position-y";

        /// <summary>The Constant BACKGROUND_REPEAT.</summary>
        public const String BACKGROUND_REPEAT = "background-repeat";

        /// <summary>The Constant BACKGROUND_SIZE.</summary>
        public const String BACKGROUND_SIZE = "background-size";

        /// <summary>The Constant BORDER.</summary>
        public const String BORDER = "border";

        /// <summary>The Constant BORDER_BOTTOM.</summary>
        public const String BORDER_BOTTOM = "border-bottom";

        /// <summary>The Constant BORDER_BOTTOM_COLOR.</summary>
        public const String BORDER_BOTTOM_COLOR = "border-bottom-color";

        /// <summary>The Constant BORDER_BOTTOM_LEFT_RADIUS.</summary>
        public const String BORDER_BOTTOM_LEFT_RADIUS = "border-bottom-left-radius";

        /// <summary>The Constant BORDER_BOTTOM_RIGHT_RADIUS.</summary>
        public const String BORDER_BOTTOM_RIGHT_RADIUS = "border-bottom-right-radius";

        /// <summary>The Constant BORDER_BOTTOM_STYLE.</summary>
        public const String BORDER_BOTTOM_STYLE = "border-bottom-style";

        /// <summary>The Constant BORDER_BOTTOM_WIDTH.</summary>
        public const String BORDER_BOTTOM_WIDTH = "border-bottom-width";

        /// <summary>The Constant BORDER_COLLAPSE.</summary>
        public const String BORDER_COLLAPSE = "border-collapse";

        /// <summary>The Constant BORDER_COLOR.</summary>
        public const String BORDER_COLOR = "border-color";

        /// <summary>The Constant BORDER_IMAGE.</summary>
        public const String BORDER_IMAGE = "border-image";

        /// <summary>The Constant BORDER_LEFT.</summary>
        public const String BORDER_LEFT = "border-left";

        /// <summary>The Constant BORDER_LEFT_COLOR.</summary>
        public const String BORDER_LEFT_COLOR = "border-left-color";

        /// <summary>The Constant BORDER_LEFT_STYLE.</summary>
        public const String BORDER_LEFT_STYLE = "border-left-style";

        /// <summary>The Constant BORDER_LEFT_WIDTH.</summary>
        public const String BORDER_LEFT_WIDTH = "border-left-width";

        /// <summary>The Constant BORDER_RADIUS.</summary>
        public const String BORDER_RADIUS = "border-radius";

        /// <summary>The Constant BORDER_RIGHT.</summary>
        public const String BORDER_RIGHT = "border-right";

        /// <summary>The Constant BORDER_RIGHT_COLOR.</summary>
        public const String BORDER_RIGHT_COLOR = "border-right-color";

        /// <summary>The Constant BORDER_RIGHT_STYLE.</summary>
        public const String BORDER_RIGHT_STYLE = "border-right-style";

        /// <summary>The Constant BORDER_RIGHT_WIDTH.</summary>
        public const String BORDER_RIGHT_WIDTH = "border-right-width";

        /// <summary>The Constant BORDER_SPACING.</summary>
        public const String BORDER_SPACING = "border-spacing";

        /// <summary>The Constant BORDER_STYLE.</summary>
        public const String BORDER_STYLE = "border-style";

        /// <summary>The Constant BORDER_TOP.</summary>
        public const String BORDER_TOP = "border-top";

        /// <summary>The Constant BORDER_TOP_COLOR.</summary>
        public const String BORDER_TOP_COLOR = "border-top-color";

        /// <summary>The Constant BORDER_TOP_LEFT_RADIUS.</summary>
        public const String BORDER_TOP_LEFT_RADIUS = "border-top-left-radius";

        /// <summary>The Constant BORDER_TOP_RIGHT_RADIUS.</summary>
        public const String BORDER_TOP_RIGHT_RADIUS = "border-top-right-radius";

        /// <summary>The Constant BORDER_TOP_STYLE.</summary>
        public const String BORDER_TOP_STYLE = "border-top-style";

        /// <summary>The Constant BORDER_TOP_WIDTH.</summary>
        public const String BORDER_TOP_WIDTH = "border-top-width";

        /// <summary>The Constant BORDER_WIDTH.</summary>
        public const String BORDER_WIDTH = "border-width";

        /// <summary>The Constant BOX_SHADOW.</summary>
        public const String BOX_SHADOW = "box-shadow";

        /// <summary>The Constant BREAK_ALL.</summary>
        public const String BREAK_ALL = "break-all";

        /// <summary>The Constant CAPTION_SIDE.</summary>
        public const String CAPTION_SIDE = "caption-side";

        /// <summary>The Constant COLOR.</summary>
        public const String COLOR = "color";

        /// <summary>The Constant COLOR_DODGE.</summary>
        public const String COLOR_DODGE = "color-dodge";

        /// <summary>The Constant COLOR_BURN.</summary>
        public const String COLOR_BURN = "color-burn";

        /// <summary>The Constant COLUMN_GAP.</summary>
        public const String COLUMN_GAP = "column-gap";

        /// <summary>The Constant COLUMN_RULE_WIDTH.</summary>
        public const String COLUMN_RULE_WIDTH = "column-rule-width";

        /// <summary>The Constant COLUMN_RULE_STYLE.</summary>
        public const String COLUMN_RULE_STYLE = "column-rule-style";

        /// <summary>The Constant COLUMN_RULE_COLOR.</summary>
        public const String COLUMN_RULE_COLOR = "column-rule-color";

        /// <summary>The Constant COLUMNS</summary>
        public const String COLUMNS = "columns";

        /// <summary>The Constant COLUMNS</summary>
        public const String COLUMN_RULE = "column-rule";

        /// <summary>The Constant DARKEN.</summary>
        public const String DARKEN = "darken";

        /// <summary>The Constant DIFFERENCE.</summary>
        public const String DIFFERENCE = "difference";

        /// <summary>The Constant DIRECTION.</summary>
        public const String DIRECTION = "direction";

        /// <summary>The Constant DISPLAY.</summary>
        public const String DISPLAY = "display";

        /// <summary>The Constant DENSE.</summary>
        public const String DENSE = "dense";

        /// <summary>The Constant EMPTY_CELLS.</summary>
        public const String EMPTY_CELLS = "empty-cells";

        /// <summary>The Constant EXCLUSION.</summary>
        public const String EXCLUSION = "exclusion";

        /// <summary>The Constant FLEX.</summary>
        public const String FLEX = "flex";

        /// <summary>The Constant FLEX_BASIS.</summary>
        public const String FLEX_BASIS = "flex-basis";

        /// <summary>The Constant FLEX_DIRECTION.</summary>
        public const String FLEX_DIRECTION = "flex-direction";

        /// <summary>The Constant FLEX_GROW.</summary>
        public const String FLEX_GROW = "flex-grow";

        /// <summary>The Constant FLEX_FLOW.</summary>
        public const String FLEX_FLOW = "flex-flow";

        /// <summary>The Constant FLEX_SHRINK.</summary>
        public const String FLEX_SHRINK = "flex-shrink";

        /// <summary>The Constant FLEX_WRAP.</summary>
        public const String FLEX_WRAP = "flex-wrap";

        /// <summary>The Constant FLOAT.</summary>
        public const String FLOAT = "float";

        /// <summary>The Constant FONT.</summary>
        public const String FONT = "font";

        /// <summary>The Constant FONT_FAMILY.</summary>
        public const String FONT_FAMILY = "font-family";

        /// <summary>The Constant FONT_FEATURE_SETTINGS.</summary>
        public const String FONT_FEATURE_SETTINGS = "font-feature-settings";

        /// <summary>The Constant FONT_KERNING.</summary>
        public const String FONT_KERNING = "font-kerning";

        /// <summary>The Constant FONT_LANGUAGE_OVERRIDE.</summary>
        public const String FONT_LANGUAGE_OVERRIDE = "font-language-override";

        /// <summary>The Constant FONT_SIZE.</summary>
        public const String FONT_SIZE = "font-size";

        /// <summary>The Constant FONT_SIZE_ADJUST.</summary>
        public const String FONT_SIZE_ADJUST = "font-size-adjust";

        /// <summary>The Constant FONT_STRETCH.</summary>
        public const String FONT_STRETCH = "font-stretch";

        /// <summary>The Constant FONT_STYLE.</summary>
        public const String FONT_STYLE = "font-style";

        /// <summary>The Constant FONT_SYNTHESIS.</summary>
        public const String FONT_SYNTHESIS = "font-synthesis";

        /// <summary>The Constant FONT_VARIANT.</summary>
        public const String FONT_VARIANT = "font-variant";

        /// <summary>The Constant FONT_VARIANT_ALTERNATES.</summary>
        public const String FONT_VARIANT_ALTERNATES = "font-variant-alternates";

        /// <summary>The Constant FONT_VARIANT_CAPS.</summary>
        public const String FONT_VARIANT_CAPS = "font-variant-caps";

        /// <summary>The Constant FONT_VARIANT_EAST_ASIAN.</summary>
        public const String FONT_VARIANT_EAST_ASIAN = "font-variant-east-asian";

        /// <summary>The Constant FONT_VARIANT_LIGATURES.</summary>
        public const String FONT_VARIANT_LIGATURES = "font-variant-ligatures";

        /// <summary>The Constant FONT_VARIANT_NUMERIC.</summary>
        public const String FONT_VARIANT_NUMERIC = "font-variant-numeric";

        /// <summary>The Constant FONT_VARIANT_POSITION.</summary>
        public const String FONT_VARIANT_POSITION = "font-variant-position";

        /// <summary>The Constant FONT_WEIGHT.</summary>
        public const String FONT_WEIGHT = "font-weight";

        /// <summary>The Constant FR.</summary>
        public const String FR = "fr";

        /// <summary>The Constant GAP.</summary>
        public const String GAP = "gap";

        /// <summary>The Constant GRID.</summary>
        public const String GRID = "grid";

        /// <summary>The Constant GRID_COLUMN.</summary>
        public const String GRID_COLUMN = "grid-column";

        /// <summary>The Constant GRID_ROW.</summary>
        public const String GRID_ROW = "grid-row";

        /// <summary>The Constant GRID_TEMPLATE.</summary>
        public const String GRID_TEMPLATE = "grid-template";

        /// <summary>The Constant GRID_COLUMN_END.</summary>
        public const String GRID_COLUMN_END = "grid-column-end";

        /// <summary>The Constant GRID_COLUMN_GAP.</summary>
        public const String GRID_COLUMN_GAP = "grid-column-gap";

        /// <summary>The Constant GRID_COLUMN_START.</summary>
        public const String GRID_COLUMN_START = "grid-column-start";

        /// <summary>The Constant GRID_GAP.</summary>
        public const String GRID_GAP = "grid-gap";

        /// <summary>The Constant GRID_ROW_END.</summary>
        public const String GRID_ROW_END = "grid-row-end";

        /// <summary>The Constant GRID_ROW_GAP.</summary>
        public const String GRID_ROW_GAP = "grid-row-gap";

        /// <summary>The Constant GRID_ROW_START.</summary>
        public const String GRID_ROW_START = "grid-row-start";

        /// <summary>The Constant GRID_TEMPLATE_AREAS.</summary>
        public const String GRID_TEMPLATE_AREAS = "grid-template-areas";

        /// <summary>The Constant GRID_TEMPLATE_COLUMNS.</summary>
        public const String GRID_TEMPLATE_COLUMNS = "grid-template-columns";

        /// <summary>The Constant GRID_TEMPLATE_ROWS.</summary>
        public const String GRID_TEMPLATE_ROWS = "grid-template-rows";

        /// <summary>The Constant GRID_AUTO_ROWS.</summary>
        public const String GRID_AUTO_ROWS = "grid-auto-rows";

        /// <summary>The Constant GRID_AUTO_COLUMNS.</summary>
        public const String GRID_AUTO_COLUMNS = "grid-auto-columns";

        /// <summary>The Constant GRID_AUTO_FLOW.</summary>
        public const String GRID_AUTO_FLOW = "grid-auto-flow";

        /// <summary>The Constant AUTO_FLOW.</summary>
        public const String AUTO_FLOW = "auto-flow";

        /// <summary>The Constant HANGING_PUNCTUATION.</summary>
        public const String HANGING_PUNCTUATION = "hanging-punctuation";

        /// <summary>The Constant HARD_LIGHT.</summary>
        public const String HARD_LIGHT = "hard-light";

        /// <summary>The Constant HUE.</summary>
        public const String HUE = "hue";

        /// <summary>The Constant HYPHENS.</summary>
        public const String HYPHENS = "hyphens";

        /// <summary>The Constant INLINE-BLOCK</summary>
        public const String INLINE_BLOCK = "inline-block";

        /// <summary>The Constant JUSTIFY_CONTENT.</summary>
        public const String JUSTIFY_CONTENT = "justify-content";

        /// <summary>The Constant JUSTIFY_ITEMS.</summary>
        public const String JUSTIFY_ITEMS = "justify-items";

        /// <summary>The Constant KEEP_ALL.</summary>
        public const String KEEP_ALL = "keep-all";

        /// <summary>The Constant LETTER_SPACING.</summary>
        public const String LETTER_SPACING = "letter-spacing";

        /// <summary>The Constant LINE_HEIGHT.</summary>
        public const String LINE_HEIGHT = "line-height";

        /// <summary>The Constant LIST_STYLE.</summary>
        public const String LIST_STYLE = "list-style";

        /// <summary>The Constant LIST_STYLE_IMAGE.</summary>
        public const String LIST_STYLE_IMAGE = "list-style-image";

        /// <summary>The Constant LIST_STYLE_POSITION.</summary>
        public const String LIST_STYLE_POSITION = "list-style-position";

        /// <summary>The Constant LIST_STYLE_TYPE.</summary>
        public const String LIST_STYLE_TYPE = "list-style-type";

        /// <summary>The Constant MARGIN.</summary>
        public const String MARGIN = "margin";

        /// <summary>The Constant MARGIN_BOTTOM.</summary>
        public const String MARGIN_BOTTOM = "margin-bottom";

        /// <summary>The Constant MARGIN_LEFT.</summary>
        public const String MARGIN_LEFT = "margin-left";

        /// <summary>The Constant MARGIN_RIGHT.</summary>
        public const String MARGIN_RIGHT = "margin-right";

        /// <summary>The Constant MARGIN_TOP.</summary>
        public const String MARGIN_TOP = "margin-top";

        /// <summary>The Constant MIN_HEIGHT.</summary>
        public const String MIN_HEIGHT = "min-height";

        /// <summary>The Constant MULTIPLY.</summary>
        public const String MULTIPLY = "multiply";

        /// <summary>The Constant OPACITY.</summary>
        public const String OPACITY = "opacity";

        /// <summary>The Constant ORDER.</summary>
        public const String ORDER = "order";

        /// <summary>The Constant OPRPHANS.</summary>
        public const String ORPHANS = "orphans";

        /// <summary>The Constant OUTLINE.</summary>
        public const String OUTLINE = "outline";

        /// <summary>The Constant OUTLINE_COLOR.</summary>
        public const String OUTLINE_COLOR = "outline-color";

        /// <summary>The Constant OUTLINE_STYLE.</summary>
        public const String OUTLINE_STYLE = "outline-style";

        /// <summary>The Constant OUTLINE_WIDTH.</summary>
        public const String OUTLINE_WIDTH = "outline-width";

        /// <summary>The Constant OVERFLOW_WRAP.</summary>
        public const String OVERFLOW_WRAP = "overflow-wrap";

        /// <summary>The Constant OVERFLOW.</summary>
        public const String OVERFLOW = "overflow";

        /// <summary>The Constant PADDING.</summary>
        public const String PADDING = "padding";

        /// <summary>The Constant PADDING_BOTTOM.</summary>
        public const String PADDING_BOTTOM = "padding-bottom";

        /// <summary>The Constant PADDING_LEFT.</summary>
        public const String PADDING_LEFT = "padding-left";

        /// <summary>The Constant PADDING_RIGHT.</summary>
        public const String PADDING_RIGHT = "padding-right";

        /// <summary>The Constant PADDING_TOP.</summary>
        public const String PADDING_TOP = "padding-top";

        /// <summary>The Constant PAGE_BREAK_AFTER.</summary>
        public const String PAGE_BREAK_AFTER = "page-break-after";

        /// <summary>The Constant PAGE_BREAK_BEFORE.</summary>
        public const String PAGE_BREAK_BEFORE = "page-break-before";

        /// <summary>The Constant PAGE_BREAK_INSIDE.</summary>
        public const String PAGE_BREAK_INSIDE = "page-break-inside";

        /// <summary>The Constant PLACE_ITEMS.</summary>
        public const String PLACE_ITEMS = "place-items";

        /// <summary>The Constant POSITION.</summary>
        public const String POSITION = "position";

        /// <summary>The Constant QUOTES.</summary>
        public const String QUOTES = "quotes";

        /// <summary>The Constant TAB_SIZE.</summary>
        public const String TAB_SIZE = "tab-size";

        /// <summary>The Constant TEXT_ALIGN.</summary>
        public const String TEXT_ALIGN = "text-align";

        /// <summary>The Constant TEXT_ALIGN_LAST.</summary>
        public const String TEXT_ALIGN_LAST = "text-align-last";

        /// <summary>The Constant TEXT_COMBINE_UPRIGHT.</summary>
        public const String TEXT_COMBINE_UPRIGHT = "text-combine-upright";

        /// <summary>The Constant TEXT_DECORATION.</summary>
        public const String TEXT_DECORATION = "text-decoration";

        /// <summary>The Constant TEXT_DECORATION_LINE.</summary>
        public const String TEXT_DECORATION_LINE = "text-decoration-line";

        /// <summary>The Constant TEXT_DECORATION_STYLE.</summary>
        public const String TEXT_DECORATION_STYLE = "text-decoration-style";

        /// <summary>The Constant TEXT_DECORATION_COLOR.</summary>
        public const String TEXT_DECORATION_COLOR = "text-decoration-color";

        /// <summary>The Constant TEXT_INDENT.</summary>
        public const String TEXT_INDENT = "text-indent";

        /// <summary>The Constant TEXT_JUSTIFY.</summary>
        public const String TEXT_JUSTIFY = "text-justify";

        /// <summary>The Constant TEXT_ORIENTATION.</summary>
        public const String TEXT_ORIENTATION = "text-orientation";

        /// <summary>The Constant TEXT_SHADOW.</summary>
        public const String TEXT_SHADOW = "text-shadow";

        /// <summary>The Constant TEXT_TRANSFORM.</summary>
        public const String TEXT_TRANSFORM = "text-transform";

        /// <summary>The Constant TEXT_UNDERLINE_POSITION.</summary>
        public const String TEXT_UNDERLINE_POSITION = "text-underline-position";

        /// <summary>The Constant TRANSFORM.</summary>
        public const String TRANSFORM = "transform";

        /// <summary>The Constant UNICODE_BIDI.</summary>
        public const String UNICODE_BIDI = "unicode-bidi";

        /// <summary>The Constant VISIBILITY.</summary>
        public const String VISIBILITY = "visibility";

        /// <summary>The Constant WHITE_SPACE.</summary>
        public const String WHITE_SPACE = "white-space";

        /// <summary>The Constant WIDOWS.</summary>
        public const String WIDOWS = "widows";

        /// <summary>The Constant WIDTH.</summary>
        public const String WIDTH = "width";

        /// <summary>The Constant HEIGHT.</summary>
        public const String HEIGHT = "height";

        /// <summary>The Constant WORDWRAP.</summary>
        public const String WORDWRAP = "word-wrap";

        /// <summary>The Constant WORD_BREAK.</summary>
        public const String WORD_BREAK = "word-break";

        /// <summary>The Constant WORD_SPACING.</summary>
        public const String WORD_SPACING = "word-spacing";

        /// <summary>The Constant WRITING_MODE.</summary>
        public const String WRITING_MODE = "writing-mode";

        // property values
        /// <summary>The Constant ANYWHERE.</summary>
        public const String ANYWHERE = "anywhere";

        /// <summary>The Constant ALWAYS.</summary>
        public const String ALWAYS = "always";

        /// <summary>The Constant ARMENIAN.</summary>
        public const String ARMENIAN = "armenian";

        /// <summary>The Constant AVOID.</summary>
        public const String AVOID = "avoid";

        /// <summary>The Constant AUTO.</summary>
        public const String AUTO = "auto";

        /// <summary>The Constant BASELINE.</summary>
        public const String BASELINE = "baseline";

        /// <summary>The Constant BLINK.</summary>
        public const String BLINK = "blink";

        /// <summary>The Constant BOLD.</summary>
        public const String BOLD = "bold";

        /// <summary>The Constant BOLDER.</summary>
        public const String BOLDER = "bolder";

        /// <summary>The Constant BORDER_BOX.</summary>
        public const String BORDER_BOX = "border-box";

        /// <summary>The Constant BOTTOM.</summary>
        public const String BOTTOM = "bottom";

        /// <summary>The Constant BREAK_WORD.</summary>
        public const String BREAK_WORD = "break-word";

        /// <summary>The Constant CAPTION.</summary>
        public const String CAPTION = "caption";

        /// <summary>The Constant CENTER.</summary>
        public const String CENTER = "center";

        /// <summary>The Constant CIRCLE.</summary>
        public const String CIRCLE = "circle";

        /// <summary>The Constant CJK_IDEOGRAPHIC.</summary>
        public const String CJK_IDEOGRAPHIC = "cjk-ideographic";

        /// <summary>The Constant CLOSE_QUOTE.</summary>
        public const String CLOSE_QUOTE = "close-quote";

        /// <summary>The Constant COLUMN.</summary>
        public const String COLUMN = "column";

        /// <summary>The Constant COLUMN_REVERSE.</summary>
        public const String COLUMN_REVERSE = "column-reverse";

        /// <summary>The Constant COLUMN_COUNT.</summary>
        public const String COLUMN_COUNT = "column-count";

        /// <summary>The Constant COLUMN_WIDTH.</summary>
        public const String COLUMN_WIDTH = "column-width";

        /// <summary>The Constant CONTAIN.</summary>
        public const String CONTAIN = "contain";

        /// <summary>The Constant CONTENT.</summary>
        public const String CONTENT = "content";

        /// <summary>The Constant CONTENT_BOX.</summary>
        public const String CONTENT_BOX = "content-box";

        /// <summary>The Constant COVER.</summary>
        public const String COVER = "cover";

        /// <summary>The Constant CURRENTCOLOR.</summary>
        public const String CURRENTCOLOR = "currentcolor";

        /// <summary>The Constant DASHED.</summary>
        public const String DASHED = "dashed";

        /// <summary>The Constant DECIMAL.</summary>
        public const String DECIMAL = "decimal";

        /// <summary>The Constant DECIMAL_LEADING_ZERO.</summary>
        public const String DECIMAL_LEADING_ZERO = "decimal-leading-zero";

        /// <summary>The Constant DEG.</summary>
        public const String DEG = "deg";

        /// <summary>The Constant DISC.</summary>
        public const String DISC = "disc";

        /// <summary>The Constant DOTTED.</summary>
        public const String DOTTED = "dotted";

        /// <summary>The Constant DOUBLE.</summary>
        public const String DOUBLE = "double";

        /// <summary>The Constant EACH_LINE.</summary>
        public const String EACH_LINE = "each-line";

        /// <summary>The Constant END.</summary>
        public const String END = "end";

        /// <summary>The Constant FIRST.</summary>
        public const String FIRST = "first";

        /// <summary>The Constant FIT_CONTENT.</summary>
        public const String FIT_CONTENT = "fit-content";

        /// <summary>The Constant FIXED.</summary>
        public const String FIXED = "fixed";

        /// <summary>The Constant FLEX_END.</summary>
        public const String FLEX_END = "flex-end";

        /// <summary>The Constant FLEX_START.</summary>
        public const String FLEX_START = "flex-start";

        /// <summary>The Constant GEORGIAN.</summary>
        public const String GEORGIAN = "georgian";

        /// <summary>The Constant GRAD.</summary>
        public const String GRAD = "grad";

        /// <summary>The Constant GROOVE.</summary>
        public const String GROOVE = "groove";

        /// <summary>The Constant HANGING.</summary>
        public const String HANGING = "hanging";

        /// <summary>The Constant HEBREW.</summary>
        public const String HEBREW = "hebrew";

        /// <summary>The Constant HIDDEN.</summary>
        public const String HIDDEN = "hidden";

        /// <summary>The Constant HIRAGANA.</summary>
        public const String HIRAGANA = "hiragana";

        /// <summary>The Constant HIRAGANA_IROHA.</summary>
        public const String HIRAGANA_IROHA = "hiragana-iroha";

        /// <summary>The Constant ICON.</summary>
        public const String ICON = "icon";

        /// <summary>The Constant INHERIT.</summary>
        public const String INHERIT = "inherit";

        /// <summary>The Constant INITIAL.</summary>
        public const String INITIAL = "initial";

        /// <summary>The Constant INSET.</summary>
        public const String INSET = "inset";

        /// <summary>The Constant INSIDE.</summary>
        public const String INSIDE = "inside";

        /// <summary>The Constant ITALIC.</summary>
        public const String ITALIC = "italic";

        /// <summary>The Constant LARGE.</summary>
        public const String LARGE = "large";

        /// <summary>The Constant LARGER.</summary>
        public const String LARGER = "larger";

        /// <summary>The Constant LAST.</summary>
        public const String LAST = "last";

        /// <summary>The Constant value LEGACY.</summary>
        public const String LEGACY = "legacy";

        /// <summary>The Constant LEFT.</summary>
        public const String LEFT = "left";

        /// <summary>The Constant LIGHTEN.</summary>
        public const String LIGHTEN = "lighten";

        /// <summary>The Constant LIGHTER.</summary>
        public const String LIGHTER = "lighter";

        /// <summary>The Constant value LINE_THROUGH.</summary>
        public const String LINE_THROUGH = "line-through";

        /// <summary>The Constant LOCAL.</summary>
        public const String LOCAL = "local";

        /// <summary>The Constant LOWER_ALPHA.</summary>
        public const String LOWER_ALPHA = "lower-alpha";

        /// <summary>The Constant LOWER_GREEK.</summary>
        public const String LOWER_GREEK = "lower-greek";

        /// <summary>The Constant LOWER_LATIN.</summary>
        public const String LOWER_LATIN = "lower-latin";

        /// <summary>The Constant LOWER_ROMAN.</summary>
        public const String LOWER_ROMAN = "lower-roman";

        /// <summary>The Constant LUMINOSITY.</summary>
        public const String LUMINOSITY = "luminosity";

        /// <summary>The Constant MANUAL.</summary>
        public const String MANUAL = "manual";

        /// <summary>The Constant MATRIX.</summary>
        public const String MATRIX = "matrix";

        /// <summary>The Constant MEDIUM.</summary>
        public const String MEDIUM = "medium";

        /// <summary>The Constant MENU.</summary>
        public const String MENU = "menu";

        /// <summary>The Constant MAX_CONTENT.</summary>
        public const String MAX_CONTENT = "max-content";

        /// <summary>The Constant MIN_CONTENT.</summary>
        public const String MIN_CONTENT = "min-content";

        /// <summary>The Constant MESSAGE_BOX.</summary>
        public const String MESSAGE_BOX = "message-box";

        /// <summary>The Constant NOWRAP.</summary>
        public const String NOWRAP = "nowrap";

        /// <summary>The Constant NO_OPEN_QUOTE.</summary>
        public const String NO_OPEN_QUOTE = "no-open-quote";

        /// <summary>The Constant NO_CLOSE_QUOTE.</summary>
        public const String NO_CLOSE_QUOTE = "no-close-quote";

        /// <summary>The Constant NO_REPEAT.</summary>
        public const String NO_REPEAT = "no-repeat";

        /// <summary>The Constant NONE.</summary>
        public const String NONE = "none";

        /// <summary>The Constant NORMAL.</summary>
        public const String NORMAL = "normal";

        /// <summary>The Constant OBLIQUE.</summary>
        public const String OBLIQUE = "oblique";

        /// <summary>The Constant OPEN_QUOTE.</summary>
        public const String OPEN_QUOTE = "open-quote";

        /// <summary>The Constant OUTSIDE.</summary>
        public const String OUTSIDE = "outside";

        /// <summary>The Constant OUTSET.</summary>
        public const String OUTSET = "outset";

        /// <summary>The Constant value OVERLAY.</summary>
        public const String OVERLAY = "overlay";

        /// <summary>The Constant value OVERLINE.</summary>
        public const String OVERLINE = "overline";

        /// <summary>The Constant PADDING_BOX.</summary>
        public const String PADDING_BOX = "padding-box";

        /// <summary>The Constant RAD.</summary>
        public const String RAD = "rad";

        /// <summary>The Constant REPEAT.</summary>
        public const String REPEAT = "repeat";

        /// <summary>The Constant REPEAT_X.</summary>
        public const String REPEAT_X = "repeat-x";

        /// <summary>The Constant REPEAT_Y.</summary>
        public const String REPEAT_Y = "repeat-y";

        /// <summary>The Constant RIDGE.</summary>
        public const String RIDGE = "ridge";

        /// <summary>The Constant RIGHT.</summary>
        public const String RIGHT = "right";

        /// <summary>The Constant ROTATE.</summary>
        public const String ROTATE = "rotate";

        /// <summary>The Constant ROUND.</summary>
        public const String ROUND = "round";

        /// <summary>The Constant ROW.</summary>
        public const String ROW = "row";

        /// <summary>The Constant ROW_GAP.</summary>
        public const String ROW_GAP = "row-gap";

        /// <summary>The Constant ROW_REVERSE.</summary>
        public const String ROW_REVERSE = "row-reverse";

        /// <summary>The Constant SAFE.</summary>
        public const String SAFE = "safe";

        /// <summary>The Constant SATURATION.</summary>
        public const String SATURATION = "saturation";

        /// <summary>The Constant SCALE.</summary>
        public const String SCALE = "scale";

        /// <summary>The Constant SCALE_X.</summary>
        public const String SCALE_X = "scalex";

        /// <summary>The Constant SCALE_Y.</summary>
        public const String SCALE_Y = "scaley";

        /// <summary>The Constant SCREEN.</summary>
        public const String SCREEN = "screen";

        /// <summary>The Constant SCROLL.</summary>
        public const String SCROLL = "scroll";

        /// <summary>The Constant value SELF_END.</summary>
        public const String SELF_END = "self-end";

        /// <summary>The Constant SELF_START.</summary>
        public const String SELF_START = "self-start";

        /// <summary>The Constant SKEW.</summary>
        public const String SKEW = "skew";

        /// <summary>The Constant SKEW_X.</summary>
        public const String SKEW_X = "skewx";

        /// <summary>The Constant SKEW_Y.</summary>
        public const String SKEW_Y = "skewy";

        /// <summary>The Constant SMALL.</summary>
        public const String SMALL = "small";

        /// <summary>The Constant SMALL_CAPS.</summary>
        public const String SMALL_CAPS = "small-caps";

        /// <summary>The Constant SMALL_CAPTION.</summary>
        public const String SMALL_CAPTION = "small-caption";

        /// <summary>The Constant SMALLER.</summary>
        public const String SMALLER = "smaller";

        /// <summary>The Constant SOFT_LIGHT.</summary>
        public const String SOFT_LIGHT = "soft-light";

        /// <summary>The Constant SOLID.</summary>
        public const String SOLID = "solid";

        /// <summary>The Constant SPACE.</summary>
        public const String SPACE = "space";

        /// <summary>The Constant SPACE_AROUND.</summary>
        public const String SPACE_AROUND = "space-around";

        /// <summary>The Constant SPACE_BETWEEN.</summary>
        public const String SPACE_BETWEEN = "space-between";

        /// <summary>The Constant SPACE_EVENLY.</summary>
        public const String SPACE_EVENLY = "space-evenly";

        /// <summary>The Constant SQUARE.</summary>
        public const String SQUARE = "square";

        /// <summary>The Constant START.</summary>
        public const String START = "start";

        /// <summary>The Constant STATIC.</summary>
        public const String STATIC = "static";

        /// <summary>The Constant STATUS_BAR.</summary>
        public const String STATUS_BAR = "status-bar";

        /// <summary>The Constant STRETCH.</summary>
        public const String STRETCH = "stretch";

        /// <summary>The Constant STRING.</summary>
        public const String STRING = "string";

        /// <summary>The Constant THICK.</summary>
        public const String THICK = "thick";

        /// <summary>The Constant THIN.</summary>
        public const String THIN = "thin";

        /// <summary>The Constant TOP.</summary>
        public const String TOP = "top";

        /// <summary>The Constant TRANSLATE.</summary>
        public const String TRANSLATE = "translate";

        /// <summary>The Constant TRANSLATE_X.</summary>
        public const String TRANSLATE_X = "translatex";

        /// <summary>The Constant TRANSLATE_Y.</summary>
        public const String TRANSLATE_Y = "translatey";

        /// <summary>The Constant TRANSPARENT.</summary>
        public const String TRANSPARENT = "transparent";

        /// <summary>The Constant UNDEFINED_NAME.</summary>
        public const String UNDEFINED_NAME = "undefined";

        /// <summary>The Constant value UNDERLINE</summary>
        public const String UNDERLINE = "underline";

        /// <summary>The Constant UNSAFE.</summary>
        public const String UNSAFE = "unsafe";

        /// <summary>The Constant value UNSET.</summary>
        public const String UNSET = "unset";

        /// <summary>The Constant UPPER_ALPHA.</summary>
        public const String UPPER_ALPHA = "upper-alpha";

        /// <summary>The Constant UPPER_LATIN.</summary>
        public const String UPPER_LATIN = "upper-latin";

        /// <summary>The Constant UPPER_ROMAN.</summary>
        public const String UPPER_ROMAN = "upper-roman";

        /// <summary>The Constant value VISIBLE.</summary>
        public const String VISIBLE = "visible";

        /// <summary>The Constant value WAVY.</summary>
        public const String WAVY = "wavy";

        /// <summary>The Constant WRAP.</summary>
        public const String WRAP = "wrap";

        /// <summary>The Constant WRAP_REVERSE.</summary>
        public const String WRAP_REVERSE = "wrap-reverse";

        /// <summary>The Constant X_LARGE.</summary>
        public const String X_LARGE = "x-large";

        /// <summary>The Constant X_SMALL.</summary>
        public const String X_SMALL = "x-small";

        /// <summary>The Constant XX_LARGE.</summary>
        public const String XX_LARGE = "xx-large";

        /// <summary>The Constant XX_SMALL.</summary>
        public const String XX_SMALL = "xx-small";

        // properties possible values
        /// <summary>The Constant BACKGROUND_SIZE_VALUES.</summary>
        public static readonly ICollection<String> BACKGROUND_SIZE_VALUES = JavaCollectionsUtil.UnmodifiableSet(new 
            HashSet<String>(JavaUtil.ArraysAsList(AUTO, COVER, CONTAIN)));

        /// <summary>The Constant BACKGROUND_ORIGIN_OR_CLIP_VALUES.</summary>
        public static readonly ICollection<String> BACKGROUND_ORIGIN_OR_CLIP_VALUES = JavaCollectionsUtil.UnmodifiableSet
            (new HashSet<String>(JavaUtil.ArraysAsList(PADDING_BOX, BORDER_BOX, CONTENT_BOX)));

        /// <summary>The Constant BACKGROUND_REPEAT_VALUES.</summary>
        public static readonly ICollection<String> BACKGROUND_REPEAT_VALUES = JavaCollectionsUtil.UnmodifiableSet(
            new HashSet<String>(JavaUtil.ArraysAsList(REPEAT, NO_REPEAT, REPEAT_X, REPEAT_Y, ROUND, SPACE)));

        /// <summary>The Constant BACKGROUND_ATTACHMENT_VALUES.</summary>
        public static readonly ICollection<String> BACKGROUND_ATTACHMENT_VALUES = JavaCollectionsUtil.UnmodifiableSet
            (new HashSet<String>(JavaUtil.ArraysAsList(FIXED, SCROLL, LOCAL)));

        /// <summary>The Constant BACKGROUND_POSITION_VALUES.</summary>
        public static readonly ICollection<String> BACKGROUND_POSITION_VALUES = JavaCollectionsUtil.UnmodifiableSet
            (new HashSet<String>(JavaUtil.ArraysAsList(LEFT, CENTER, BOTTOM, TOP, RIGHT)));

        /// <summary>The Constant BACKGROUND_POSITION_X_VALUES.</summary>
        public static readonly ICollection<String> BACKGROUND_POSITION_X_VALUES = JavaCollectionsUtil.UnmodifiableSet
            (new HashSet<String>(JavaUtil.ArraysAsList(LEFT, CENTER, RIGHT)));

        /// <summary>The Constant BACKGROUND_POSITION_Y_VALUES.</summary>
        public static readonly ICollection<String> BACKGROUND_POSITION_Y_VALUES = JavaCollectionsUtil.UnmodifiableSet
            (new HashSet<String>(JavaUtil.ArraysAsList(CENTER, BOTTOM, TOP)));

        /// <summary>The Constant BORDER_WIDTH_VALUES.</summary>
        public static readonly ICollection<String> BORDER_WIDTH_VALUES = JavaCollectionsUtil.UnmodifiableSet(new HashSet
            <String>(JavaUtil.ArraysAsList(new String[] { THIN, MEDIUM, THICK })));

        /// <summary>The Constant BORDER_STYLE_VALUES.</summary>
        public static readonly ICollection<String> BORDER_STYLE_VALUES = JavaCollectionsUtil.UnmodifiableSet(new HashSet
            <String>(JavaUtil.ArraysAsList(new String[] { NONE, HIDDEN, DOTTED, DASHED, SOLID, DOUBLE, GROOVE, RIDGE
            , INSET, OUTSET })));

        /// <summary>The Constant BLEND_MODE_VALUES.</summary>
        public static readonly ICollection<String> BLEND_MODE_VALUES = JavaCollectionsUtil.UnmodifiableSet(new HashSet
            <String>(JavaUtil.ArraysAsList(new String[] { NORMAL, MULTIPLY, SCREEN, OVERLAY, DARKEN, LIGHTEN, COLOR_DODGE
            , COLOR_BURN, HARD_LIGHT, SOFT_LIGHT, DIFFERENCE, EXCLUSION, HUE, SATURATION, COLOR, LUMINOSITY })));

        /// <summary>The Constant FONT_ABSOLUTE_SIZE_KEYWORDS.</summary>
        public static readonly IDictionary<String, String> FONT_ABSOLUTE_SIZE_KEYWORDS_VALUES;

        /// <summary>The Constant METRIC_MEASUREMENTS.</summary>
        public static readonly String[] METRIC_MEASUREMENTS_VALUES = new String[] { CommonCssConstants.PX, CommonCssConstants
            .IN, CommonCssConstants.CM, CommonCssConstants.MM, CommonCssConstants.PC, CommonCssConstants.PT, CommonCssConstants
            .Q };

        // pseudo-classes
        /// <summary>The Constant ACTIVE.</summary>
        public const String ACTIVE = "active";

        /// <summary>The Constant CHECKED.</summary>
        public const String CHECKED = "checked";

        /// <summary>The Constant DISABLED.</summary>
        public const String DISABLED = "disabled";

        /// <summary>The Constant EMPTY.</summary>
        public const String EMPTY = "empty";

        /// <summary>The Constant ENABLED.</summary>
        public const String ENABLED = "enabled";

        /// <summary>The Constant FIRST_CHILD.</summary>
        public const String FIRST_CHILD = "first-child";

        /// <summary>The Constant FIRST_OF_TYPE.</summary>
        public const String FIRST_OF_TYPE = "first-of-type";

        /// <summary>The Constant FOCUS.</summary>
        public const String FOCUS = "focus";

        /// <summary>The Constant HOVER.</summary>
        public const String HOVER = "hover";

        /// <summary>The Constant IN_RANGE.</summary>
        public const String IN_RANGE = "in-range";

        /// <summary>The Constant INVALID.</summary>
        public const String INVALID = "invalid";

        /// <summary>The Constant LANG.</summary>
        public const String LANG = "lang";

        /// <summary>The Constant LAST_CHILD.</summary>
        public const String LAST_CHILD = "last-child";

        /// <summary>The Constant LAST_OF_TYPE.</summary>
        public const String LAST_OF_TYPE = "last-of-type";

        /// <summary>The Constant LINK.</summary>
        public const String LINK = "link";

        /// <summary>The Constant NTH_CHILD.</summary>
        public const String NTH_CHILD = "nth-child";

        /// <summary>The Constant NOT.</summary>
        public const String NOT = "not";

        /// <summary>The Constant NTH_LAST_CHILD.</summary>
        public const String NTH_LAST_CHILD = "nth-last-child";

        /// <summary>The Constant NTH_LAST_OF_TYPE.</summary>
        public const String NTH_LAST_OF_TYPE = "nth-last-of-type";

        /// <summary>The Constant NTH_OF_TYPE.</summary>
        public const String NTH_OF_TYPE = "nth-of-type";

        /// <summary>The Constant ONLY_OF_TYPE.</summary>
        public const String ONLY_OF_TYPE = "only-of-type";

        /// <summary>The Constant ONLY_CHILD.</summary>
        public const String ONLY_CHILD = "only-child";

        /// <summary>The Constant OPTIONAL.</summary>
        public const String OPTIONAL = "optional";

        /// <summary>The Constant OUT_OF_RANGE.</summary>
        public const String OUT_OF_RANGE = "out-of-range";

        /// <summary>The Constant READ_ONLY.</summary>
        public const String READ_ONLY = "read-only";

        /// <summary>The Constant READ_WRITE.</summary>
        public const String READ_WRITE = "read-write";

        /// <summary>The Constant REQUIRED.</summary>
        public const String REQUIRED = "required";

        /// <summary>The Constant ROOT.</summary>
        public const String ROOT = "root";

        /// <summary>The Constant TARGET.</summary>
        public const String TARGET = "target";

        /// <summary>The Constant URL.</summary>
        public const String URL = "url";

        /// <summary>The Constant VALID.</summary>
        public const String VALID = "valid";

        /// <summary>The Constant VISITED.</summary>
        public const String VISITED = "visited";

        // units of measurement
        /// <summary>The Constant CM.</summary>
        public const String CM = "cm";

        /// <summary>The Constant EM.</summary>
        public const String EM = "em";

        /// <summary>The Constant EX.</summary>
        public const String EX = "ex";

        /// <summary>The Constant IN.</summary>
        public const String IN = "in";

        /// <summary>The Constant MM.</summary>
        public const String MM = "mm";

        /// <summary>The Constant PC.</summary>
        public const String PC = "pc";

        /// <summary>The Constant PERCENTAGE.</summary>
        public const String PERCENTAGE = "%";

        /// <summary>The Constant PT.</summary>
        public const String PT = "pt";

        /// <summary>The Constant PX.</summary>
        public const String PX = "px";

        /// <summary>The Constant REM.</summary>
        public const String REM = "rem";

        /// <summary>The Constant Q.</summary>
        public const String Q = "q";

        // units of resolution
        /// <summary>The Constant DPCM.</summary>
        public const String DPCM = "dpcm";

        /// <summary>The Constant DPI.</summary>
        public const String DPI = "dpi";

        /// <summary>The Constant DPPX.</summary>
        public const String DPPX = "dppx";
    }
}
