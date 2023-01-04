/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: iText Software.

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
using iText.StyledXmlParser.Jsoup.Helper;
using iText.StyledXmlParser.Jsoup.Nodes;
using iText.StyledXmlParser.Jsoup.Safety;

namespace iText.StyledXmlParser.Jsoup {
    /// <summary>The core public access point to the jsoup functionality.</summary>
    /// <author>Jonathan Hedley</author>
    public class Jsoup {
        private Jsoup() {
        }

        /// <summary>Parse HTML into a Document.</summary>
        /// <remarks>Parse HTML into a Document. The parser will make a sensible, balanced document tree out of any HTML.
        ///     </remarks>
        /// <param name="html">HTML to parse</param>
        /// <param name="baseUri">
        /// The URL where the HTML was retrieved from. Used to resolve relative URLs to absolute URLs, that occur
        /// before the HTML declares a
        /// <c>&lt;base href&gt;</c>
        /// tag.
        /// </param>
        /// <returns>sane HTML</returns>
        public static Document Parse(String html, String baseUri) {
            return iText.StyledXmlParser.Jsoup.Parser.Parser.Parse(html, baseUri);
        }

        /// <summary>Parse HTML into a Document, using the provided Parser.</summary>
        /// <remarks>
        /// Parse HTML into a Document, using the provided Parser. You can provide an alternate parser, such as a simple XML
        /// (non-HTML) parser.
        /// </remarks>
        /// <param name="html">HTML to parse</param>
        /// <param name="baseUri">
        /// The URL where the HTML was retrieved from. Used to resolve relative URLs to absolute URLs, that occur
        /// before the HTML declares a
        /// <c>&lt;base href&gt;</c>
        /// tag.
        /// </param>
        /// <param name="parser">
        /// alternate
        /// <see cref="iText.StyledXmlParser.Jsoup.Parser.Parser.XmlParser()">parser</see>
        /// to use.
        /// </param>
        /// <returns>sane HTML</returns>
        public static Document Parse(String html, String baseUri, iText.StyledXmlParser.Jsoup.Parser.Parser parser
            ) {
            return parser.ParseInput(html, baseUri);
        }

        /// <summary>Parse HTML into a Document.</summary>
        /// <remarks>
        /// Parse HTML into a Document. As no base URI is specified, absolute URL detection relies on the HTML including a
        /// <c>&lt;base href&gt;</c>
        /// tag.
        /// </remarks>
        /// <param name="html">HTML to parse</param>
        /// <returns>sane HTML</returns>
        /// <seealso cref="Parse(System.String, System.String)"/>
        public static Document Parse(String html) {
            return iText.StyledXmlParser.Jsoup.Parser.Parser.Parse(html, "");
        }

        /// <summary>Parse the contents of a file as HTML.</summary>
        /// <param name="in">file to load HTML from</param>
        /// <param name="charsetName">
        /// (optional) character set of file contents. Set to
        /// <see langword="null"/>
        /// to determine from
        /// <c>http-equiv</c>
        /// meta tag, if
        /// present, or fall back to
        /// <c>UTF-8</c>
        /// (which is often safe to do).
        /// </param>
        /// <param name="baseUri">The URL where the HTML was retrieved from, to resolve relative links against.</param>
        /// <returns>sane HTML</returns>
        public static Document Parse(FileInfo @in, String charsetName, String baseUri) {
            return DataUtil.Load(@in, charsetName, baseUri);
        }

        /// <summary>Parse the contents of a file as HTML.</summary>
        /// <remarks>Parse the contents of a file as HTML. The location of the file is used as the base URI to qualify relative URLs.
        ///     </remarks>
        /// <param name="in">file to load HTML from</param>
        /// <param name="charsetName">
        /// (optional) character set of file contents. Set to
        /// <see langword="null"/>
        /// to determine from
        /// <c>http-equiv</c>
        /// meta tag, if
        /// present, or fall back to
        /// <c>UTF-8</c>
        /// (which is often safe to do).
        /// </param>
        /// <returns>sane HTML</returns>
        /// <seealso cref="Parse(System.IO.FileInfo, System.String, System.String)"/>
        public static Document Parse(FileInfo @in, String charsetName) {
            return DataUtil.Load(@in, charsetName, @in.FullName);
        }

        /// <summary>Read an input stream, and parse it to a Document.</summary>
        /// <param name="in">input stream to read. Make sure to close it after parsing.</param>
        /// <param name="charsetName">
        /// (optional) character set of file contents. Set to
        /// <see langword="null"/>
        /// to determine from
        /// <c>http-equiv</c>
        /// meta tag, if
        /// present, or fall back to
        /// <c>UTF-8</c>
        /// (which is often safe to do).
        /// </param>
        /// <param name="baseUri">The URL where the HTML was retrieved from, to resolve relative links against.</param>
        /// <returns>sane HTML</returns>
        public static Document Parse(Stream @in, String charsetName, String baseUri) {
            return DataUtil.Load(@in, charsetName, baseUri);
        }

        /// <summary>Read an input stream, and parse it to a Document.</summary>
        /// <remarks>
        /// Read an input stream, and parse it to a Document. You can provide an alternate parser, such as a simple XML
        /// (non-HTML) parser.
        /// </remarks>
        /// <param name="in">input stream to read. Make sure to close it after parsing.</param>
        /// <param name="charsetName">
        /// (optional) character set of file contents. Set to
        /// <see langword="null"/>
        /// to determine from
        /// <c>http-equiv</c>
        /// meta tag, if
        /// present, or fall back to
        /// <c>UTF-8</c>
        /// (which is often safe to do).
        /// </param>
        /// <param name="baseUri">The URL where the HTML was retrieved from, to resolve relative links against.</param>
        /// <param name="parser">
        /// alternate
        /// <see cref="iText.StyledXmlParser.Jsoup.Parser.Parser.XmlParser()">parser</see>
        /// to use.
        /// </param>
        /// <returns>sane HTML</returns>
        public static Document Parse(Stream @in, String charsetName, String baseUri, iText.StyledXmlParser.Jsoup.Parser.Parser
             parser) {
            return DataUtil.Load(@in, charsetName, baseUri, parser);
        }

        /// <summary>
        /// Parse a fragment of HTML, with the assumption that it forms the
        /// <c>body</c>
        /// of the HTML.
        /// </summary>
        /// <param name="bodyHtml">body HTML fragment</param>
        /// <param name="baseUri">URL to resolve relative URLs against.</param>
        /// <returns>sane HTML document</returns>
        /// <seealso cref="iText.StyledXmlParser.Jsoup.Nodes.Document.Body()"/>
        public static Document ParseBodyFragment(String bodyHtml, String baseUri) {
            return iText.StyledXmlParser.Jsoup.Parser.Parser.ParseBodyFragment(bodyHtml, baseUri);
        }

        /// <summary>
        /// Parse a fragment of HTML, with the assumption that it forms the
        /// <c>body</c>
        /// of the HTML.
        /// </summary>
        /// <param name="bodyHtml">body HTML fragment</param>
        /// <returns>sane HTML document</returns>
        /// <seealso cref="iText.StyledXmlParser.Jsoup.Nodes.Document.Body()"/>
        public static Document ParseBodyFragment(String bodyHtml) {
            return iText.StyledXmlParser.Jsoup.Parser.Parser.ParseBodyFragment(bodyHtml, "");
        }

        /// <summary>
        /// Get safe HTML from untrusted input HTML, by parsing input HTML and filtering it through an allow-list of safe
        /// tags and attributes.
        /// </summary>
        /// <param name="bodyHtml">input untrusted HTML (body fragment)</param>
        /// <param name="baseUri">URL to resolve relative URLs against</param>
        /// <param name="safelist">list of permitted HTML elements</param>
        /// <returns>safe HTML (body fragment)</returns>
        /// <seealso cref="iText.StyledXmlParser.Jsoup.Safety.Cleaner.Clean(iText.StyledXmlParser.Jsoup.Nodes.Document)
        ///     "/>
        public static String Clean(String bodyHtml, String baseUri, Safelist safelist) {
            Document dirty = ParseBodyFragment(bodyHtml, baseUri);
            Cleaner cleaner = new Cleaner(safelist);
            Document clean = cleaner.Clean(dirty);
            return clean.Body().Html();
        }

        /// <summary>
        /// Use
        /// <see cref="Clean(System.String, System.String, iText.StyledXmlParser.Jsoup.Safety.Safelist)"/>
        /// instead.
        /// </summary>
        [System.ObsoleteAttribute(@"as of 1.14.1.")]
        public static String Clean(String bodyHtml, String baseUri, Whitelist safelist) {
            return Clean(bodyHtml, baseUri, (Safelist)safelist);
        }

        /// <summary>
        /// Get safe HTML from untrusted input HTML, by parsing input HTML and filtering it through a safe-list of permitted
        /// tags and attributes.
        /// </summary>
        /// <remarks>
        /// Get safe HTML from untrusted input HTML, by parsing input HTML and filtering it through a safe-list of permitted
        /// tags and attributes.
        /// <para />
        /// Note that as this method does not take a base href URL to resolve attributes with relative URLs against, those
        /// URLs will be removed, unless the input HTML contains a
        /// <c>&lt;base href&gt; tag</c>
        /// . If you wish to preserve those, use
        /// the
        /// <see>Jsoup#clean(String html, String baseHref, Safelist)</see>
        /// method instead, and enable
        /// <see>Safelist#preserveRelativeLinks(boolean true)</see>.
        /// </remarks>
        /// <param name="bodyHtml">input untrusted HTML (body fragment)</param>
        /// <param name="safelist">list of permitted HTML elements</param>
        /// <returns>safe HTML (body fragment)</returns>
        /// <seealso cref="iText.StyledXmlParser.Jsoup.Safety.Cleaner.Clean(iText.StyledXmlParser.Jsoup.Nodes.Document)
        ///     "/>
        public static String Clean(String bodyHtml, Safelist safelist) {
            return Clean(bodyHtml, "", safelist);
        }

        /// <summary>
        /// Use
        /// <see cref="Clean(System.String, iText.StyledXmlParser.Jsoup.Safety.Safelist)"/>
        /// instead.
        /// </summary>
        [System.ObsoleteAttribute(@"as of 1.14.1.")]
        public static String Clean(String bodyHtml, Whitelist safelist) {
            return Clean(bodyHtml, (Safelist)safelist);
        }

        /// <summary>
        /// Get safe HTML from untrusted input HTML, by parsing input HTML and filtering it through a safe-list of
        /// permitted tags and attributes.
        /// </summary>
        /// <remarks>
        /// Get safe HTML from untrusted input HTML, by parsing input HTML and filtering it through a safe-list of
        /// permitted tags and attributes.
        /// <para />The HTML is treated as a body fragment; it's expected the cleaned HTML will be used within the body of an
        /// existing document. If you want to clean full documents, use
        /// <see cref="iText.StyledXmlParser.Jsoup.Safety.Cleaner.Clean(iText.StyledXmlParser.Jsoup.Nodes.Document)"/>
        /// instead, and add
        /// structural tags (<c>html, head, body</c> etc) to the safelist.
        /// </remarks>
        /// <param name="bodyHtml">input untrusted HTML (body fragment)</param>
        /// <param name="baseUri">URL to resolve relative URLs against</param>
        /// <param name="safelist">list of permitted HTML elements</param>
        /// <param name="outputSettings">document output settings; use to control pretty-printing and entity escape modes
        ///     </param>
        /// <returns>safe HTML (body fragment)</returns>
        /// <seealso cref="iText.StyledXmlParser.Jsoup.Safety.Cleaner.Clean(iText.StyledXmlParser.Jsoup.Nodes.Document)
        ///     "/>
        public static String Clean(String bodyHtml, String baseUri, Safelist safelist, OutputSettings outputSettings
            ) {
            Document dirty = ParseBodyFragment(bodyHtml, baseUri);
            Cleaner cleaner = new Cleaner(safelist);
            Document clean = cleaner.Clean(dirty);
            clean.OutputSettings(outputSettings);
            return clean.Body().Html();
        }

        /// <summary>
        /// Use
        /// <see cref="Clean(System.String, System.String, iText.StyledXmlParser.Jsoup.Safety.Safelist, iText.StyledXmlParser.Jsoup.Nodes.OutputSettings)
        ///     "/>
        /// instead.
        /// </summary>
        [System.ObsoleteAttribute(@"as of 1.14.1.")]
        public static String Clean(String bodyHtml, String baseUri, Whitelist safelist, OutputSettings outputSettings
            ) {
            return Clean(bodyHtml, baseUri, (Safelist)safelist, outputSettings);
        }

        /// <summary>Test if the input body HTML has only tags and attributes allowed by the Safelist.</summary>
        /// <remarks>
        /// Test if the input body HTML has only tags and attributes allowed by the Safelist. Useful for form validation.
        /// <para />The input HTML should still be run through the cleaner to set up enforced attributes, and to tidy the output.
        /// <para />Assumes the HTML is a body fragment (i.e. will be used in an existing HTML document body.)
        /// </remarks>
        /// <param name="bodyHtml">HTML to test</param>
        /// <param name="safelist">safelist to test against</param>
        /// <returns>true if no tags or attributes were removed; false otherwise</returns>
        /// <seealso cref="Clean(System.String, iText.StyledXmlParser.Jsoup.Safety.Safelist)"/>
        public static bool IsValid(String bodyHtml, Safelist safelist) {
            return new Cleaner(safelist).IsValidBodyHtml(bodyHtml);
        }

        /// <summary>
        /// Use
        /// <see cref="IsValid(System.String, iText.StyledXmlParser.Jsoup.Safety.Safelist)"/>
        /// instead.
        /// </summary>
        [System.ObsoleteAttribute(@"as of 1.14.1.")]
        public static bool IsValid(String bodyHtml, Whitelist safelist) {
            return IsValid(bodyHtml, (Safelist)safelist);
        }
    }
}
