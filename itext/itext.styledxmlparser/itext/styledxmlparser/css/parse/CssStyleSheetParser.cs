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
