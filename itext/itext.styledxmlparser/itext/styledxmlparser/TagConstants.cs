/*
This file is part of the iText (R) project.
Copyright (c) 1998-2017 iText Group NV
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
    /// <summary>Class that bundles a series of tag constants.</summary>
    public sealed class TagConstants {
        /// <summary>
        /// Creates a new
        /// <see cref="TagConstants"/>
        /// instance.
        /// </summary>
        private TagConstants() {
        }

        /// <summary>The Constant A.</summary>
        public const String A = "a";

        /// <summary>The Constant ABBR.</summary>
        public const String ABBR = "abbr";

        /// <summary>The Constant ADDRESS.</summary>
        public const String ADDRESS = "address";

        /// <summary>The Constant ARTICLE.</summary>
        public const String ARTICLE = "article";

        /// <summary>The Constant ASIDE.</summary>
        public const String ASIDE = "aside";

        /// <summary>The Constant B.</summary>
        public const String B = "b";

        /// <summary>The Constant BDI.</summary>
        public const String BDI = "bdi";

        /// <summary>The Constant BDO.</summary>
        public const String BDO = "bdo";

        /// <summary>The Constant BIG.</summary>
        public const String BIG = "big";

        /// <summary>The Constant BLOCKQUOTE.</summary>
        public const String BLOCKQUOTE = "blockquote";

        /// <summary>The Constant BODY.</summary>
        public const String BODY = "body";

        /// <summary>The Constant BR.</summary>
        public const String BR = "br";

        /// <summary>The Constant BUTTON.</summary>
        public const String BUTTON = "button";

        /// <summary>The Constant CAPTION.</summary>
        public const String CAPTION = "caption";

        /// <summary>The Constant CENTER.</summary>
        public const String CENTER = "center";

        /// <summary>The Constant CITE.</summary>
        public const String CITE = "cite";

        /// <summary>The Constant CODE.</summary>
        public const String CODE = "code";

        /// <summary>The Constant COL.</summary>
        public const String COL = "col";

        /// <summary>The Constant COLGROUP.</summary>
        public const String COLGROUP = "colgroup";

        /// <summary>The Constant DD.</summary>
        public const String DD = "dd";

        /// <summary>The Constant DEL.</summary>
        public const String DEL = "del";

        /// <summary>The Constant DFN.</summary>
        public const String DFN = "dfn";

        /// <summary>The Constant DL.</summary>
        public const String DL = "dl";

        /// <summary>The Constant DT.</summary>
        public const String DT = "dt";

        /// <summary>The Constant DIV.</summary>
        public const String DIV = "div";

        /// <summary>The Constant EM.</summary>
        public const String EM = "em";

        /// <summary>The Constant FIELDSET.</summary>
        public const String FIELDSET = "fieldset";

        /// <summary>The Constant FIGCAPTION.</summary>
        public const String FIGCAPTION = "figcaption";

        /// <summary>The Constant FIGURE.</summary>
        public const String FIGURE = "figure";

        /// <summary>The Constant FONT.</summary>
        public const String FONT = "font";

        /// <summary>The Constant FOOTER.</summary>
        public const String FOOTER = "footer";

        /// <summary>The Constant FORM.</summary>
        public const String FORM = "form";

        /// <summary>The Constant H1.</summary>
        public const String H1 = "h1";

        /// <summary>The Constant H2.</summary>
        public const String H2 = "h2";

        /// <summary>The Constant H3.</summary>
        public const String H3 = "h3";

        /// <summary>The Constant H4.</summary>
        public const String H4 = "h4";

        /// <summary>The Constant H5.</summary>
        public const String H5 = "h5";

        /// <summary>The Constant H6.</summary>
        public const String H6 = "h6";

        /// <summary>The Constant HR.</summary>
        public const String HR = "hr";

        /// <summary>The Constant HEAD.</summary>
        public const String HEAD = "head";

        /// <summary>The Constant HEADER.</summary>
        public const String HEADER = "header";

        /// <summary>The Constant HTML.</summary>
        public const String HTML = "html";

        /// <summary>The Constant I.</summary>
        public const String I = "i";

        /// <summary>The Constant IMG.</summary>
        public const String IMG = "img";

        /// <summary>The Constant INPUT.</summary>
        public const String INPUT = "input";

        /// <summary>The Constant INS.</summary>
        public const String INS = "ins";

        /// <summary>The Constant KBD.</summary>
        public const String KBD = "kbd";

        /// <summary>The Constant LABEL.</summary>
        public const String LABEL = "label";

        /// <summary>The Constant LEGEND.</summary>
        public const String LEGEND = "legend";

        /// <summary>The Constant LI.</summary>
        public const String LI = "li";

        /// <summary>The Constant LINK.</summary>
        public const String LINK = "link";

        /// <summary>The Constant MAIN.</summary>
        public const String MAIN = "main";

        /// <summary>The Constant MARK.</summary>
        public const String MARK = "mark";

        /// <summary>The Constant MARQUEE.</summary>
        public const String MARQUEE = "marquee";

        /// <summary>The Constant META.</summary>
        public const String META = "meta";

        /// <summary>The Constant NAV.</summary>
        public const String NAV = "nav";

        /// <summary>The Constant OL.</summary>
        public const String OL = "ol";

        /// <summary>The Constant P.</summary>
        public const String P = "p";

        /// <summary>The Constant PRE.</summary>
        public const String PRE = "pre";

        /// <summary>The Constant Q.</summary>
        public const String Q = "q";

        /// <summary>The Constant S.</summary>
        public const String S = "s";

        /// <summary>The Constant SAMP.</summary>
        public const String SAMP = "samp";

        /// <summary>The Constant SCRIPT.</summary>
        public const String SCRIPT = "script";

        /// <summary>The Constant SECTION.</summary>
        public const String SECTION = "section";

        /// <summary>The Constant SELECT.</summary>
        public const String SELECT = "select";

        /// <summary>The Constant SMALL.</summary>
        public const String SMALL = "small";

        /// <summary>The Constant SPAN.</summary>
        public const String SPAN = "span";

        /// <summary>The Constant STRIKE.</summary>
        public const String STRIKE = "strike";

        /// <summary>The Constant STRONG.</summary>
        public const String STRONG = "strong";

        /// <summary>The Constant STYLE.</summary>
        public const String STYLE = "style";

        /// <summary>The Constant SUB.</summary>
        public const String SUB = "sub";

        /// <summary>The Constant SUP.</summary>
        public const String SUP = "sup";

        /// <summary>The Constant TABLE.</summary>
        public const String TABLE = "table";

        /// <summary>The Constant TBODY.</summary>
        public const String TBODY = "tbody";

        /// <summary>The Constant TEXTAREA.</summary>
        public const String TEXTAREA = "textarea";

        /// <summary>The Constant TD.</summary>
        public const String TD = "td";

        /// <summary>The Constant TFOOT.</summary>
        public const String TFOOT = "tfoot";

        /// <summary>The Constant TH.</summary>
        public const String TH = "th";

        /// <summary>The Constant THEAD.</summary>
        public const String THEAD = "thead";

        /// <summary>The Constant TIME.</summary>
        public const String TIME = "time";

        /// <summary>The Constant TITLE.</summary>
        public const String TITLE = "title";

        /// <summary>The Constant TR.</summary>
        public const String TR = "tr";

        /// <summary>The Constant TT.</summary>
        public const String TT = "tt";

        /// <summary>The Constant U.</summary>
        public const String U = "u";

        /// <summary>The Constant UL.</summary>
        public const String UL = "ul";

        /// <summary>The Constant VAR.</summary>
        public const String VAR = "var";
    }
}
