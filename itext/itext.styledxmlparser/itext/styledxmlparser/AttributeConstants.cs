/*
This file is part of the iText (R) project.
Copyright (c) 1998-2018 iText Group NV
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

namespace iText.StyledXmlParser {
    /// <summary>Class that bundles a series of attribute constants.</summary>
    public sealed class AttributeConstants {
        /// <summary>
        /// Creates a new
        /// <see cref="AttributeConstants"/>
        /// instance.
        /// </summary>
        private AttributeConstants() {
        }

        /// <summary>The Constant ALIGN.</summary>
        public const String ALIGN = "align";

        /// <summary>The Constant ALT.</summary>
        public const String ALT = "alt";

        /// <summary>The Constant APPLICATION_NAME.</summary>
        public const String APPLICATION_NAME = "application-name";

        /// <summary>The Constant AUTHOR.</summary>
        public const String AUTHOR = "author";

        /// <summary>The Constant BGCOLOR.</summary>
        public const String BGCOLOR = "bgcolor";

        /// <summary>The Constant BORDER.</summary>
        public const String BORDER = "border";

        /// <summary>The Constant CLASS.</summary>
        public const String CLASS = "class";

        /// <summary>The Constant CLASS.</summary>
        public const String CELLPADDING = "cellpadding";

        /// <summary>The Constant CLASS.</summary>
        public const String CELLSPACING = "cellspacing";

        /// <summary>The Constant COLOR.</summary>
        public const String COLOR = "color";

        /// <summary>The Constant COLS.</summary>
        public const String COLS = "cols";

        /// <summary>The Constant COLSPAN.</summary>
        public const String COLSPAN = "colspan";

        /// <summary>The Constant CONTENT.</summary>
        public const String CONTENT = "content";

        /// <summary>The Constant DATA</summary>
        public const String DATA = "data";

        /// <summary>The Constant DESCRIPTION.</summary>
        public const String DESCRIPTION = "description";

        /// <summary>The Constant DIR.</summary>
        public const String DIR = "dir";

        /// <summary>The Constant FACE.</summary>
        public const String FACE = "face";

        /// <summary>The Constant HEIGHT.</summary>
        public const String HEIGHT = "height";

        /// <summary>The Constant HREF.</summary>
        public const String HREF = "href";

        /// <summary>The Constant HSPACE.</summary>
        public const String HSPACE = "hspace";

        /// <summary>The Constant ID.</summary>
        public const String ID = "id";

        /// <summary>The Constant KEYWORDS.</summary>
        public const String KEYWORDS = "keywords";

        /// <summary>The Constant LANG.</summary>
        public const String LANG = "lang";

        /// <summary>The Constant MEDIA.</summary>
        public const String MEDIA = "media";

        /// <summary>The Constant NAME.</summary>
        public const String NAME = "name";

        /// <summary>The Constant NOSHADE.</summary>
        public const String NOSHADE = "noshade";

        /// <summary>The Constant NUMBER.</summary>
        public const String NUMBER = "number";

        /// <summary>The Constant REL.</summary>
        public const String REL = "rel";

        /// <summary>The Constant ROWS.</summary>
        public const String ROWS = "rows";

        /// <summary>The Constant ROWSPAN.</summary>
        public const String ROWSPAN = "rowspan";

        /// <summary>The Constant SIZE.</summary>
        public const String SIZE = "size";

        /// <summary>The Constant SPAN.</summary>
        public const String SPAN = "span";

        /// <summary>The Constant SRC.</summary>
        public const String SRC = "src";

        /// <summary>The Constant STYLE.</summary>
        public const String STYLE = "style";

        /// <summary>The Constant TYPE.</summary>
        public const String TYPE = "type";

        /// <summary>The Constant VALIGN.</summary>
        public const String VALIGN = "valign";

        /// <summary>The Constant VALUE.</summary>
        public const String VALUE = "value";

        /// <summary>The Constant VSPACE.</summary>
        public const String VSPACE = "vspace";

        /// <summary>The Constant WIDTH.</summary>
        public const String WIDTH = "width";

        /// <summary>The Constant TITLE.</summary>
        public const String TITLE = "title";

        /// <summary>The Constant _1.</summary>
        public const String _1 = "1";

        /// <summary>The Constant A.</summary>
        public const String A = "A";

        /// <summary>The Constant a.</summary>
        public const String a = "a";

        /// <summary>The Constant BOTTOM.</summary>
        public const String BOTTOM = "bottom";

        /// <summary>The Constant BUTTON.</summary>
        public const String BUTTON = "button";

        /// <summary>The Constant CENTER.</summary>
        public const String CENTER = "center";

        /// <summary>The Constant CHECKBOX.</summary>
        public const String CHECKBOX = "checkbox";

        /// <summary>The Constant CHECKED.</summary>
        public const String CHECKED = "checked";

        /// <summary>The Constant EMAIL.</summary>
        public const String EMAIL = "email";

        /// <summary>The Constant I.</summary>
        public const String I = "I";

        /// <summary>The Constant i.</summary>
        public const String i = "i";

        /// <summary>The Constant LEFT.</summary>
        public const String LEFT = "left";

        /// <summary>The Constant LTR.</summary>
        public const String LTR = "ltr";

        /// <summary>The Constant MIDDLE.</summary>
        public const String MIDDLE = "middle";

        /// <summary>The Constant PASSWORD.</summary>
        public const String PASSWORD = "password";

        /// <summary>The Constant RADIO.</summary>
        public const String RADIO = "radio";

        /// <summary>The Constant RIGHT.</summary>
        public const String RIGHT = "right";

        /// <summary>The Constant RTL.</summary>
        public const String RTL = "rtl";

        /// <summary>The Constant STYLESHEET.</summary>
        public const String STYLESHEET = "stylesheet";

        /// <summary>The Constant SUBMIT.</summary>
        public const String SUBMIT = "submit";

        /// <summary>The Constant TEXT.</summary>
        public const String TEXT = "text";

        /// <summary>The Constant TOP.</summary>
        public const String TOP = "top";

        /// <summary>The Constant start</summary>
        public const String START = "start";

        public const String PLACEHOLDER = "placeholder";

        /// <summary>The Constant PARENT_TABLE_BORDER.</summary>
        public const String PARENT_TABLE_BORDER = "parenttableborder";

        public sealed class ObjectTypes {
            public const String SVGIMAGE = "image/svg+xml";

            internal ObjectTypes(AttributeConstants _enclosing) {
                this._enclosing = _enclosing;
            }

            private readonly AttributeConstants _enclosing;
            // attribute values
            // iText custom attributes
        }
    }
}
