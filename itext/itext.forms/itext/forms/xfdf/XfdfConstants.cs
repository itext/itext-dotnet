/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: iText Software.

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

namespace iText.Forms.Xfdf {
    /// <summary>Class containing constants to be used in XFDF processing.</summary>
    public sealed class XfdfConstants {
        public const String TEXT = "text";

        public const String HIGHLIGHT = "highlight";

        public const String UNDERLINE = "underline";

        public const String STRIKEOUT = "strikeout";

        public const String SQUIGGLY = "squiggly";

        public const String LINE = "line";

        public const String CIRCLE = "circle";

        public const String SQUARE = "square";

        public const String CARET = "caret";

        public const String POPUP = "popup";

        public const String POLYGON = "polygon";

        public const String POLYLINE = "polyline";

        public const String STAMP = "stamp";

        public const String INK = "ink";

        public const String FREETEXT = "freetext";

        public const String FILEATTACHMENT = "fileattachment";

        public const String SOUND = "sound";

        public const String LINK = "link";

        public const String REDACT = "redact";

        public const String PROJECTION = "projection";

        public const String PAGE = "page";

        public const String COLOR = "color";

        public const String DATE = "date";

        public const String FLAGS = "flags";

        public const String NAME = "name";

        public const String RECT = "rect";

        public const String TITLE = "title";

        public const String CREATION_DATE = "creationdate";

        public const String OPACITY = "opacity";

        public const String SUBJECT = "subject";

        public const String ICON = "icon";

        public const String STATE = "state";

        public const String STATE_MODEL = "statemodel";

        public const String IN_REPLY_TO = "inreplyto";

        public const String REPLY_TYPE = "replyType";

        public const String CONTENTS = "contents";

        public const String CONTENTS_RICHTEXT = "contents-richtext";

        public const String EMPTY_F_LEMENT = "Empty f element, no href attribute found.";

        public const String FIELDS = "fields";

        public const String FIELD = "field";

        public const String F = "f";

        public const String HREF = "href";

        public const String IDS = "ids";

        public const String ANNOTS = "annots";

        public const String ANNOT = "annot";

        public const String VALUE = "value";

        public const String COORDS = "coords";

        public const String WIDTH = "width";

        public const String DASHES = "dashes";

        public const String STYLE = "style";

        public const String INTERIOR_COLOR = "interior-color";

        public const String FRINGE = "fringe";

        public const String APPEARANCE = "appearance";

        public const String JUSTIFICATION = "justification";

        public const String INTENT = "intent";

        public const String START = "start";

        public const String END = "end";

        public const String HEAD = "head";

        public const String TAIL = "tail";

        public const String LEADER_EXTENDED = "leaderExtended";

        public const String LEADER_LENGTH = "leaderLength";

        public const String CAPTION = "caption";

        public const String LEADER_OFFSET = "leader-offset";

        public const String CAPTION_STYLE = "caption-style";

        public const String CAPTION_OFFSET_H = "caption-offset-h";

        public const String CAPTION_OFFSET_V = "caption-offset-v";

        public const String OPEN = "open";

        public const String ORIGINAL = "original";

        public const String MODIFIED = "modified";

        public const String EMPTY_IDS_ELEMENT = "Empty ids element, original and/or modified id attributes not found.";

        public const String EMPTY_FIELD_VALUE_ELEMENT = "Field has no value.";

        public const String EMPTY_FIELD_NAME_ELEMENT = "Field has no name attribute.";

        public const String ROTATION = "rotation";

        public const String DEST = "Dest";

        public const String FIT = "Fit";

        public const String FIT_B = "FitB";

        public const String FIT_H = "FitH";

        public const String FIT_V = "FitV";

        public const String FIT_BH = "FitBH";

        public const String FIT_BV = "FitBV";

        public const String FIT_R = "FitR";

        public const String TOP = "Top";

        public const String BOTTOM = "Bottom";

        public const String RIGHT = "Right";

        public const String LEFT = "Left";

        public const String XYZ_CAPITAL = "XYZ";

        public const String XYZ = "xyz";

        public const String NAMED = "Named";

        public const String LAUNCH = "Launch";

        public const String ORIGINAL_NAME = "OriginalName";

        public const String NEW_WINDOW = "NewWindow";

        public const String GO_TO = "GoTo";

        public const String GO_TO_R = "GoToR";

        public const String FILE = "File";

        public const String ON_ACTIVATION = "OnActivation";

        public const String ACTION = "Action";

        public const String URI = "URI";

        public const String IS_MAP = "IsMap";

        public const String INVISIBLE = "invisible";

        public const String HIDDEN = "hidden";

        public const String PRINT = "print";

        public const String NO_ZOOM = "nozoom";

        public const String NO_ROTATE = "norotate";

        public const String NO_VIEW = "noview";

        public const String READ_ONLY = "readonly";

        public const String LOCKED = "locked";

        public const String TOGGLE_NO_VIEW = "togglenoview";

        public const String VERTICES = "vertices";

        public const String PAGE_CAPITAL = "Page";

        public const String BORDER_STYLE_ALT = "BorderStyleAlt";

        public const String H_CORNER_RADIUS = "HCornerRadius";

        public const String V_CORNER_RADIUS = "VCornerRadius";

        public const String WIDTH_CAPITAL = "Width";

        public const String DASH_PATTERN = "DashPattern";

        public const String NAME_CAPITAL = "Name";

        public const String DEFAULT_APPEARANCE = "defaultappearance";

        public const String DEFAULT_STYLE = "defaultstyle";

        private XfdfConstants() {
        }
    }
}
