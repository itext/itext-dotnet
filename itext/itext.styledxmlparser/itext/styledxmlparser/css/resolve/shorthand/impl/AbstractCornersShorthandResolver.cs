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
using System.Collections.Generic;
using iText.Commons.Utils;
using iText.StyledXmlParser.Css;
using iText.StyledXmlParser.Css.Resolve.Shorthand;

namespace iText.StyledXmlParser.Css.Resolve.Shorthand.Impl {
    /// <summary>
    /// Abstract
    /// <see cref="iText.StyledXmlParser.Css.Resolve.Shorthand.IShorthandResolver"/>
    /// implementation for corners definitions.
    /// </summary>
    public abstract class AbstractCornersShorthandResolver : IShorthandResolver {
        /// <summary>The template for -bottom-left properties.</summary>
        private const String _0_BOTTOM_LEFT_1 = "{0}-bottom-left{1}";

        /// <summary>The template for -bottom-right properties.</summary>
        private const String _0_BOTTOM_RIGHT_1 = "{0}-bottom-right{1}";

        /// <summary>The template for -top-left properties.</summary>
        private const String _0_TOP_LEFT_1 = "{0}-top-left{1}";

        /// <summary>The template for -top-right properties.</summary>
        private const String _0_TOP_RIGHT_1 = "{0}-top-right{1}";

        /// <summary>Gets the prefix of a property.</summary>
        /// <returns>the prefix</returns>
        protected internal abstract String GetPrefix();

        /// <summary>Gets the postfix of a property.</summary>
        /// <returns>the postfix</returns>
        protected internal abstract String GetPostfix();

        /* (non-Javadoc)
        * @see com.itextpdf.styledxmlparser.css.resolve.shorthand.IShorthandResolver#resolveShorthand(java.lang.String)
        */
        public virtual IList<CssDeclaration> ResolveShorthand(String shorthandExpression) {
            String[] props = iText.Commons.Utils.StringUtil.Split(shorthandExpression, "\\/");
            String[][] properties = new String[props.Length][];
            for (int i = 0; i < props.Length; i++) {
                properties[i] = iText.Commons.Utils.StringUtil.Split(props[i].Trim(), "\\s+");
            }
            String[] resultExpressions = new String[4];
            for (int i = 0; i < resultExpressions.Length; i++) {
                resultExpressions[i] = "";
            }
            IList<CssDeclaration> resolvedDecl = new List<CssDeclaration>();
            String topLeftProperty = MessageFormatUtil.Format(_0_TOP_LEFT_1, GetPrefix(), GetPostfix());
            String topRightProperty = MessageFormatUtil.Format(_0_TOP_RIGHT_1, GetPrefix(), GetPostfix());
            String bottomRightProperty = MessageFormatUtil.Format(_0_BOTTOM_RIGHT_1, GetPrefix(), GetPostfix());
            String bottomLeftProperty = MessageFormatUtil.Format(_0_BOTTOM_LEFT_1, GetPrefix(), GetPostfix());
            for (int i = 0; i < properties.Length; i++) {
                if (properties[i].Length == 1) {
                    resultExpressions[0] += properties[i][0] + " ";
                    resultExpressions[1] += properties[i][0] + " ";
                    resultExpressions[2] += properties[i][0] + " ";
                    resultExpressions[3] += properties[i][0] + " ";
                }
                else {
                    if (properties[i].Length == 2) {
                        resultExpressions[0] += properties[i][0] + " ";
                        resultExpressions[1] += properties[i][1] + " ";
                        resultExpressions[2] += properties[i][0] + " ";
                        resultExpressions[3] += properties[i][1] + " ";
                    }
                    else {
                        if (properties[i].Length == 3) {
                            resultExpressions[0] += properties[i][0] + " ";
                            resultExpressions[1] += properties[i][1] + " ";
                            resultExpressions[2] += properties[i][2] + " ";
                            resultExpressions[3] += properties[i][1] + " ";
                        }
                        else {
                            if (properties[i].Length == 4) {
                                resultExpressions[0] += properties[i][0] + " ";
                                resultExpressions[1] += properties[i][1] + " ";
                                resultExpressions[2] += properties[i][2] + " ";
                                resultExpressions[3] += properties[i][3] + " ";
                            }
                        }
                    }
                }
            }
            resolvedDecl.Add(new CssDeclaration(topLeftProperty, resultExpressions[0]));
            resolvedDecl.Add(new CssDeclaration(topRightProperty, resultExpressions[1]));
            resolvedDecl.Add(new CssDeclaration(bottomRightProperty, resultExpressions[2]));
            resolvedDecl.Add(new CssDeclaration(bottomLeftProperty, resultExpressions[3]));
            return resolvedDecl;
        }
    }
}
