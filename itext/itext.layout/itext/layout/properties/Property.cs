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
namespace iText.Layout.Properties {
    /// <summary>
    /// An enum of property names that are used for graphical properties of layout
    /// elements.
    /// </summary>
    /// <remarks>
    /// An enum of property names that are used for graphical properties of layout
    /// elements. The
    /// <see cref="iText.Layout.IPropertyContainer"/>
    /// performs the same function as an
    /// <see cref="System.Collections.IDictionary{K, V}"/>
    /// , with the values of
    /// <see cref="Property"/>
    /// as its potential keys.
    /// </remarks>
    public sealed class Property {
        public const int ACTION = 1;

        public const int ALIGN_CONTENT = 130;

        public const int ALIGN_ITEMS = 134;

        public const int ALIGN_SELF = 129;

        // This property is needed for form field appearance with right-to-left text. By setting true we avoid writing
        // /ActualText and /ReversedChars to form field appearance streams because this resulted in Acrobat showing
        // an empty appearance in such cases.
        public const int APPEARANCE_STREAM_LAYOUT = 82;

        public const int AREA_BREAK_TYPE = 2;

        public const int AUTO_SCALE = 3;

        public const int AUTO_SCALE_HEIGHT = 4;

        public const int AUTO_SCALE_WIDTH = 5;

        public const int BACKGROUND = 6;

        public const int BACKGROUND_IMAGE = 90;

        public const int BASE_DIRECTION = 7;

        public const int BOLD_SIMULATION = 8;

        public const int BORDER = 9;

        public const int BORDER_BOTTOM = 10;

        public const int BORDER_BOTTOM_LEFT_RADIUS = 113;

        public const int BORDER_BOTTOM_RIGHT_RADIUS = 112;

        public const int BORDER_COLLAPSE = 114;

        public const int BORDER_LEFT = 11;

        public const int BORDER_RADIUS = 101;

        public const int BORDER_RIGHT = 12;

        public const int BORDER_TOP = 13;

        public const int BORDER_TOP_LEFT_RADIUS = 110;

        public const int BORDER_TOP_RIGHT_RADIUS = 111;

        public const int BOTTOM = 14;

        public const int BOX_SIZING = 105;

        public const int CAPTION_SIDE = 119;

        public const int CHARACTER_SPACING = 15;

        public const int CLEAR = 100;

        public const int COLLAPSING_MARGINS = 89;

        public const int COLSPAN = 16;

        public const int COLUMN_COUNT = 138;

        public const int DESTINATION = 17;

        public const int FILL_AVAILABLE_AREA = 86;

        public const int FILL_AVAILABLE_AREA_ON_SPLIT = 87;

        public const int FIRST_LINE_INDENT = 18;

        public const int FLEX_BASIS = 131;

        public const int FLEX_GROW = 132;

        public const int FLEX_SHRINK = 127;

        public const int FLEX_WRAP = 128;

        public const int FLEX_DIRECTION = 139;

        public const int FLOAT = 99;

        public const int FLUSH_ON_DRAW = 19;

        /// <summary>Font family as String or PdfFont shall be set.</summary>
        /// <seealso cref="iText.IO.Font.Constants.StandardFontFamilies"/>
        public const int FONT = 20;

        public const int FONT_COLOR = 21;

        public const int FONT_KERNING = 22;

        /// <summary>String value.</summary>
        /// <remarks>
        /// String value. 'normal'|'italic'|'oblique'
        /// Note, this property will be applied only if
        /// <see cref="FONT"/>
        /// has String[] value.
        /// </remarks>
        public const int FONT_STYLE = 94;

        /// <summary>String value.</summary>
        /// <remarks>
        /// String value. 'normal'|'bold'|number
        /// Note, this property will be applied only if
        /// <see cref="FONT"/>
        /// has String[] value.
        /// </remarks>
        public const int FONT_WEIGHT = 95;

        public const int FONT_SCRIPT = 23;

        /// <summary>
        /// Shall be instance of
        /// <see cref="iText.Layout.Font.FontProvider"/>
        /// </summary>
        public const int FONT_PROVIDER = 91;

        /// <summary>
        /// Shall be instance of
        /// <see cref="iText.Layout.Font.FontSet"/>.
        /// </summary>
        public const int FONT_SET = 98;

        public const int FONT_SIZE = 24;

        public const int FORCED_PLACEMENT = 26;

        public const int FULL = 25;

        public const int HEIGHT = 27;

        public const int HORIZONTAL_ALIGNMENT = 28;

        public const int HORIZONTAL_BORDER_SPACING = 115;

        /// <summary>Value of 1 is equivalent to no scaling</summary>
        public const int HORIZONTAL_SCALING = 29;

        public const int HYPHENATION = 30;

        public const int ID = 126;

        public const int IGNORE_FOOTER = 96;

        public const int IGNORE_HEADER = 97;

        public const int ITALIC_SIMULATION = 31;

        public const int JUSTIFY_CONTENT = 133;

        public const int KEEP_TOGETHER = 32;

        public const int KEEP_WITH_NEXT = 81;

        public const int LEADING = 33;

        public const int LEFT = 34;

        public const int LINE_DRAWER = 35;

        public const int LINE_HEIGHT = 124;

        public const int LINK_ANNOTATION = 88;

        public const int LIST_START = 36;

        public const int LIST_SYMBOL = 37;

        public const int LIST_SYMBOL_ALIGNMENT = 38;

        public const int LIST_SYMBOL_INDENT = 39;

        public const int LIST_SYMBOL_ORDINAL_VALUE = 120;

        public const int LIST_SYMBOL_PRE_TEXT = 41;

        public const int LIST_SYMBOL_POSITION = 83;

        public const int LIST_SYMBOL_POST_TEXT = 42;

        public const int LIST_SYMBOLS_INITIALIZED = 40;

        public const int MARGIN_BOTTOM = 43;

        public const int MARGIN_LEFT = 44;

        public const int MARGIN_RIGHT = 45;

        public const int MARGIN_TOP = 46;

        public const int MAX_HEIGHT = 84;

        public const int MAX_WIDTH = 79;

        public const int META_INFO = 135;

        public const int MIN_HEIGHT = 85;

        public const int MIN_WIDTH = 80;

        public const int NO_SOFT_WRAP_INLINE = 118;

        public const int OBJECT_FIT = 125;

        public const int OPACITY = 92;

        public const int ORPHANS_CONTROL = 121;

        public const int OUTLINE = 106;

        public const int OUTLINE_OFFSET = 107;

        public const int OVERFLOW_WRAP = 102;

        public const int OVERFLOW_X = 103;

        public const int OVERFLOW_Y = 104;

        public const int PADDING_BOTTOM = 47;

        public const int PADDING_LEFT = 48;

        public const int PADDING_RIGHT = 49;

        public const int PADDING_TOP = 50;

        public const int PAGE_NUMBER = 51;

        public const int POSITION = 52;

        public const int RENDERING_MODE = 123;

        public const int RIGHT = 54;

        public const int ROTATION_ANGLE = 55;

        public const int ROTATION_INITIAL_HEIGHT = 56;

        public const int ROTATION_INITIAL_WIDTH = 57;

        public const int ROTATION_POINT_X = 58;

        public const int ROTATION_POINT_Y = 59;

        public const int ROWSPAN = 60;

        public const int SPACING_RATIO = 61;

        public const int SPLIT_CHARACTERS = 62;

        public const int STROKE_COLOR = 63;

        public const int STROKE_WIDTH = 64;

        public const int SKEW = 65;

        public const int TABLE_LAYOUT = 93;

        public const int TAB_ANCHOR = 66;

        public const int TAB_DEFAULT = 67;

        public const int TAB_LEADER = 68;

        public const int TAB_STOPS = 69;

        public const int TAGGING_HELPER = 108;

        public const int TAGGING_HINT_KEY = 109;

        public const int TEXT_ALIGNMENT = 70;

        /// <summary>
        /// Use values from
        /// <see cref="iText.Kernel.Pdf.Canvas.PdfCanvasConstants.TextRenderingMode"/>.
        /// </summary>
        public const int TEXT_RENDERING_MODE = 71;

        public const int TEXT_RISE = 72;

        public const int TOP = 73;

        public const int TRANSFORM = 53;

        public const int TYPOGRAPHY_CONFIG = 117;

        public const int UNDERLINE = 74;

        public const int VERTICAL_ALIGNMENT = 75;

        public const int VERTICAL_BORDER_SPACING = 116;

        public const int INLINE_VERTICAL_ALIGNMENT = 136;

        /// <summary>Value of 1 is equivalent to no scaling</summary>
        public const int VERTICAL_SCALING = 76;

        public const int WIDOWS_CONTROL = 122;

        public const int WIDTH = 77;

        public const int WORD_SPACING = 78;

        public const int ADD_MARKED_CONTENT_TEXT = 137;

        /// <summary>
        /// Some properties must be passed to
        /// <see cref="iText.Layout.IPropertyContainer"/>
        /// objects that
        /// are lower in the document's hierarchy.
        /// </summary>
        /// <remarks>
        /// Some properties must be passed to
        /// <see cref="iText.Layout.IPropertyContainer"/>
        /// objects that
        /// are lower in the document's hierarchy. Most inherited properties are
        /// related to textual operations. Indicates whether or not this type of property is inheritable.
        /// </remarks>
        private static readonly bool[] INHERITED_PROPERTIES;

        private const int MAX_INHERITED_PROPERTY_ID = 138;

        static Property() {
            INHERITED_PROPERTIES = new bool[MAX_INHERITED_PROPERTY_ID + 1];
            INHERITED_PROPERTIES[iText.Layout.Properties.Property.APPEARANCE_STREAM_LAYOUT] = true;
            INHERITED_PROPERTIES[iText.Layout.Properties.Property.BASE_DIRECTION] = true;
            INHERITED_PROPERTIES[iText.Layout.Properties.Property.BOLD_SIMULATION] = true;
            INHERITED_PROPERTIES[iText.Layout.Properties.Property.CAPTION_SIDE] = true;
            INHERITED_PROPERTIES[iText.Layout.Properties.Property.CHARACTER_SPACING] = true;
            INHERITED_PROPERTIES[iText.Layout.Properties.Property.COLLAPSING_MARGINS] = true;
            INHERITED_PROPERTIES[iText.Layout.Properties.Property.FIRST_LINE_INDENT] = true;
            INHERITED_PROPERTIES[iText.Layout.Properties.Property.FONT] = true;
            INHERITED_PROPERTIES[iText.Layout.Properties.Property.FONT_COLOR] = true;
            INHERITED_PROPERTIES[iText.Layout.Properties.Property.FONT_KERNING] = true;
            INHERITED_PROPERTIES[iText.Layout.Properties.Property.FONT_PROVIDER] = true;
            INHERITED_PROPERTIES[iText.Layout.Properties.Property.FONT_SET] = true;
            INHERITED_PROPERTIES[iText.Layout.Properties.Property.FONT_SCRIPT] = true;
            INHERITED_PROPERTIES[iText.Layout.Properties.Property.FONT_SIZE] = true;
            INHERITED_PROPERTIES[iText.Layout.Properties.Property.FONT_STYLE] = true;
            INHERITED_PROPERTIES[iText.Layout.Properties.Property.FONT_WEIGHT] = true;
            INHERITED_PROPERTIES[iText.Layout.Properties.Property.FORCED_PLACEMENT] = true;
            INHERITED_PROPERTIES[iText.Layout.Properties.Property.HYPHENATION] = true;
            INHERITED_PROPERTIES[iText.Layout.Properties.Property.ITALIC_SIMULATION] = true;
            INHERITED_PROPERTIES[iText.Layout.Properties.Property.KEEP_TOGETHER] = true;
            INHERITED_PROPERTIES[iText.Layout.Properties.Property.LEADING] = true;
            INHERITED_PROPERTIES[iText.Layout.Properties.Property.NO_SOFT_WRAP_INLINE] = true;
            INHERITED_PROPERTIES[iText.Layout.Properties.Property.ORPHANS_CONTROL] = true;
            INHERITED_PROPERTIES[iText.Layout.Properties.Property.SPACING_RATIO] = true;
            INHERITED_PROPERTIES[iText.Layout.Properties.Property.SPLIT_CHARACTERS] = true;
            INHERITED_PROPERTIES[iText.Layout.Properties.Property.STROKE_COLOR] = true;
            INHERITED_PROPERTIES[iText.Layout.Properties.Property.STROKE_WIDTH] = true;
            INHERITED_PROPERTIES[iText.Layout.Properties.Property.TEXT_ALIGNMENT] = true;
            INHERITED_PROPERTIES[iText.Layout.Properties.Property.TEXT_RENDERING_MODE] = true;
            INHERITED_PROPERTIES[iText.Layout.Properties.Property.TEXT_RISE] = true;
            INHERITED_PROPERTIES[iText.Layout.Properties.Property.UNDERLINE] = true;
            INHERITED_PROPERTIES[iText.Layout.Properties.Property.WIDOWS_CONTROL] = true;
            INHERITED_PROPERTIES[iText.Layout.Properties.Property.WORD_SPACING] = true;
            INHERITED_PROPERTIES[iText.Layout.Properties.Property.TAGGING_HELPER] = true;
            INHERITED_PROPERTIES[iText.Layout.Properties.Property.TYPOGRAPHY_CONFIG] = true;
            INHERITED_PROPERTIES[iText.Layout.Properties.Property.RENDERING_MODE] = true;
            INHERITED_PROPERTIES[iText.Layout.Properties.Property.LINE_HEIGHT] = true;
            INHERITED_PROPERTIES[iText.Layout.Properties.Property.OVERFLOW_WRAP] = true;
            INHERITED_PROPERTIES[iText.Layout.Properties.Property.META_INFO] = true;
            INHERITED_PROPERTIES[iText.Layout.Properties.Property.ADD_MARKED_CONTENT_TEXT] = true;
        }

        private Property() {
        }

        /// <summary>
        /// This method checks whether a Property, in order to be picked up by the
        /// rendering engine, must be defined on the current element or renderer
        /// (<c>return false</c>), or may be defined in one of its parent
        /// elements or renderers (<c>return true</c>).
        /// </summary>
        /// <param name="property">the ID, defined in this class, of the property to check</param>
        /// <returns>whether the property type is inheritable</returns>
        public static bool IsPropertyInherited(int property) {
            return property >= 0 && property <= MAX_INHERITED_PROPERTY_ID && INHERITED_PROPERTIES[property];
        }
    }
}
