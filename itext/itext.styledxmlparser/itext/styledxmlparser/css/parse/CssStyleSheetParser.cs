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
using System.IO;
using iText.StyledXmlParser;
using iText.StyledXmlParser.Css;
using iText.StyledXmlParser.Css.Parse.Syntax;

namespace iText.StyledXmlParser.Css.Parse {
    /// <summary>Utilities class to parse a CSS style sheet.</summary>
    public sealed class CssStyleSheetParser {
        /// <summary>
        /// Creates a new
        /// <see cref="CssStyleSheetParser"/>.
        /// </summary>
        private CssStyleSheetParser() {
        }

        /// <summary>
        /// Parses a stream into a
        /// <see cref="CssRuleSetParser"/>.
        /// </summary>
        /// <param name="stream">the stream</param>
        /// <param name="baseUrl">the base url</param>
        /// <returns>
        /// the resulting
        /// <see cref="iText.StyledXmlParser.Css.CssStyleSheet"/>
        /// </returns>
        public static CssStyleSheet Parse(Stream stream, String baseUrl) {
            CssParserStateController controller = new CssParserStateController(baseUrl);
            // TODO determine charset correctly DEVSIX-1458
            TextReader br = PortUtil.WrapInBufferedReader(new StreamReader(stream, System.Text.Encoding.UTF8));
            char[] buffer = new char[8192];
            int length;
            while ((length = br.Read(buffer, 0, buffer.Length)) > 0) {
                for (int i = 0; i < length; i++) {
                    controller.Process(buffer[i]);
                }
            }
            return controller.GetParsingResult();
        }

        /// <summary>
        /// Parses a stream into a
        /// <see cref="iText.StyledXmlParser.Css.CssStyleSheet"/>.
        /// </summary>
        /// <param name="stream">the stream</param>
        /// <returns>
        /// the resulting
        /// <see cref="iText.StyledXmlParser.Css.CssStyleSheet"/>
        /// </returns>
        public static CssStyleSheet Parse(Stream stream) {
            return Parse(stream, null);
        }

        /// <summary>
        /// Parses a string into a
        /// <see cref="iText.StyledXmlParser.Css.CssStyleSheet"/>.
        /// </summary>
        /// <param name="data">the style sheet data</param>
        /// <param name="baseUrl">the base url</param>
        /// <returns>
        /// the resulting
        /// <see cref="iText.StyledXmlParser.Css.CssStyleSheet"/>
        /// </returns>
        public static CssStyleSheet Parse(String data, String baseUrl) {
            MemoryStream stream = new MemoryStream(data.GetBytes(System.Text.Encoding.UTF8));
            try {
                return Parse(stream, baseUrl);
            }
            catch (System.IO.IOException) {
                return null;
            }
        }

        /// <summary>
        /// Parses a string into a
        /// <see cref="iText.StyledXmlParser.Css.CssStyleSheet"/>.
        /// </summary>
        /// <param name="data">the data</param>
        /// <returns>
        /// the resulting
        /// <see cref="iText.StyledXmlParser.Css.CssStyleSheet"/>
        /// </returns>
        public static CssStyleSheet Parse(String data) {
            return Parse(data, null);
        }
    }
}
