/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: Bruno Lowagie, Paulo Soares, et al.

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License version 3
as published by the Free Software Foundation with the addition of the
following permission added to Section 15 as permitted in Section 7(a):
FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
OF THIRD PARTY RIGHTS

This program is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU Affero General Public License for more details.
You should have received a copy of the GNU Affero General Public License
along with this program; if not, see http://www.gnu.org/licenses or write to
the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
Boston, MA, 02110-1301 USA, or download the license from the following URL:
http://itextpdf.com/terms-of-use/

The interactive user interfaces in modified source and object code versions
of this program must display Appropriate Legal Notices, as required under
Section 5 of the GNU Affero General Public License.

In accordance with Section 7(b) of the GNU Affero General Public License,
a covered work must retain the producer line in every PDF that is created
or manipulated using iText.

You can be released from the requirements of the license by purchasing
a commercial license. Buying such a license is mandatory as soon as you
develop commercial activities involving the iText software without
disclosing the source code of your own applications.
These activities include: offering paid services to customers as an ASP,
serving PDFs on the fly in a web application, shipping iText with a closed
source product.

For more information, please contact iText Software Corp. at this
address: sales@itextpdf.com
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
