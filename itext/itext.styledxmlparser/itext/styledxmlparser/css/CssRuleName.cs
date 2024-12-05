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

namespace iText.StyledXmlParser.Css {
    /// <summary>Class containing possible CSS rule names.</summary>
    public sealed class CssRuleName {
        /// <summary>
        /// Creates a new
        /// <see cref="CssRuleName"/>
        /// instance.
        /// </summary>
        private CssRuleName() {
        }

        /// <summary>The Constant BOTTOM_CENTER.</summary>
        public const String BOTTOM_CENTER = "bottom-center";

        /// <summary>The Constant BOTTOM_LEFT.</summary>
        public const String BOTTOM_LEFT = "bottom-left";

        /// <summary>The Constant BOTTOM_LEFT_CORNER.</summary>
        public const String BOTTOM_LEFT_CORNER = "bottom-left-corner";

        /// <summary>The Constant BOTTOM_RIGHT.</summary>
        public const String BOTTOM_RIGHT = "bottom-right";

        /// <summary>The Constant BOTTOM_RIGHT_CORNER.</summary>
        public const String BOTTOM_RIGHT_CORNER = "bottom-right-corner";

        /// <summary>The Constant CHARSET.</summary>
        public const String CHARSET = "charset";

        /// <summary>The Constant IMPORT.</summary>
        public const String IMPORT = "import";

        /// <summary>The Constant LAYER.</summary>
        public const String LAYER = "layer";

        /// <summary>The Constant LEFT_BOTTOM.</summary>
        public const String LEFT_BOTTOM = "left-bottom";

        /// <summary>The Constant LEFT_MIDDLE.</summary>
        public const String LEFT_MIDDLE = "left-middle";

        /// <summary>The Constant LEFT_TOP.</summary>
        public const String LEFT_TOP = "left-top";

        /// <summary>The Constant FONT_FACE.</summary>
        public const String FONT_FACE = "font-face";

        /// <summary>The Constant MEDIA.</summary>
        public const String MEDIA = "media";

        /// <summary>The Constant PAGE.</summary>
        public const String PAGE = "page";

        /// <summary>The Constant RIGHT_BOTTOM.</summary>
        public const String RIGHT_BOTTOM = "right-bottom";

        /// <summary>The Constant RIGHT_MIDDLE.</summary>
        public const String RIGHT_MIDDLE = "right-middle";

        /// <summary>The Constant RIGHT_TOP.</summary>
        public const String RIGHT_TOP = "right-top";

        /// <summary>The Constant TOP_CENTER.</summary>
        public const String TOP_CENTER = "top-center";

        /// <summary>The Constant TOP_LEFT.</summary>
        public const String TOP_LEFT = "top-left";

        /// <summary>The Constant TOP_LEFT_CORNER.</summary>
        public const String TOP_LEFT_CORNER = "top-left-corner";

        /// <summary>The Constant TOP_RIGHT.</summary>
        public const String TOP_RIGHT = "top-right";

        /// <summary>The Constant TOP_RIGHT_CORNER.</summary>
        public const String TOP_RIGHT_CORNER = "top-right-corner";
    }
}
