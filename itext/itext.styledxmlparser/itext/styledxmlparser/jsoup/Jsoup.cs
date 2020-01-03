/*
This file is part of the iText (R) project.
Copyright (c) 1998-2020 iText Group NV
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

        /// <summary>Parse XML into a Document.</summary>
        /// <remarks>Parse XML into a Document. The parser will make a sensible, balanced document tree out of any HTML.
        ///     </remarks>
        /// <param name="xml">XML to parse</param>
        /// <param name="baseUri">
        /// The URL where the HTML was retrieved from. Used to resolve relative URLs to absolute URLs, that occur
        /// before the HTML declares a
        /// <c>&lt;base href&gt;</c>
        /// tag.
        /// </param>
        /// <returns>sane XML</returns>
        public static Document ParseXML(String xml, String baseUri) {
            return iText.StyledXmlParser.Jsoup.Parser.Parser.ParseXml(xml, baseUri);
        }

        /// <summary>Parse XML into a Document.</summary>
        /// <remarks>Parse XML into a Document. The parser will make a sensible, balanced document tree out of any HTML.
        ///     </remarks>
        /// <param name="xml">XML to parse</param>
        /// <returns>sane XML</returns>
        public static Document ParseXML(String xml) {
            return iText.StyledXmlParser.Jsoup.Parser.Parser.ParseXml(xml, "");
        }

        /// <summary>Parse XML into a Document.</summary>
        /// <remarks>Parse XML into a Document. The parser will make a sensible, balanced document tree out of any HTML.
        ///     </remarks>
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
        /// <returns>sane XML</returns>
        public static Document ParseXML(Stream @in, String charsetName, String baseUri) {
            return Parse(@in, charsetName, baseUri, iText.StyledXmlParser.Jsoup.Parser.Parser.XmlParser());
        }

        /// <summary>Parse XML into a Document.</summary>
        /// <remarks>Parse XML into a Document. The parser will make a sensible, balanced document tree out of any HTML.
        ///     </remarks>
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
        /// <returns>sane XML</returns>
        public static Document ParseXML(Stream @in, String charsetName) {
            return ParseXML(@in, charsetName, "");
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
        /// Get safe HTML from untrusted input HTML, by parsing input HTML and filtering it through a white-list of permitted
        /// tags and attributes.
        /// </summary>
        /// <param name="bodyHtml">input untrusted HTML (body fragment)</param>
        /// <param name="baseUri">URL to resolve relative URLs against</param>
        /// <param name="whitelist">white-list of permitted HTML elements</param>
        /// <returns>safe HTML (body fragment)</returns>
        /// <seealso cref="iText.StyledXmlParser.Jsoup.Safety.Cleaner.Clean(iText.StyledXmlParser.Jsoup.Nodes.Document)
        ///     "/>
        public static String Clean(String bodyHtml, String baseUri, Whitelist whitelist) {
            Document dirty = ParseBodyFragment(bodyHtml, baseUri);
            Cleaner cleaner = new Cleaner(whitelist);
            Document clean = cleaner.Clean(dirty);
            return clean.Body().Html();
        }

        /// <summary>
        /// Get safe HTML from untrusted input HTML, by parsing input HTML and filtering it through a white-list of permitted
        /// tags and attributes.
        /// </summary>
        /// <param name="bodyHtml">input untrusted HTML (body fragment)</param>
        /// <param name="whitelist">white-list of permitted HTML elements</param>
        /// <returns>safe HTML (body fragment)</returns>
        /// <seealso cref="iText.StyledXmlParser.Jsoup.Safety.Cleaner.Clean(iText.StyledXmlParser.Jsoup.Nodes.Document)
        ///     "/>
        public static String Clean(String bodyHtml, Whitelist whitelist) {
            return Clean(bodyHtml, "", whitelist);
        }

        /// <summary>
        /// Get safe HTML from untrusted input HTML, by parsing input HTML and filtering it through a white-list of
        /// permitted
        /// tags and attributes.
        /// </summary>
        /// <param name="bodyHtml">input untrusted HTML (body fragment)</param>
        /// <param name="baseUri">URL to resolve relative URLs against</param>
        /// <param name="whitelist">white-list of permitted HTML elements</param>
        /// <param name="outputSettings">document output settings; use to control pretty-printing and entity escape modes
        ///     </param>
        /// <returns>safe HTML (body fragment)</returns>
        /// <seealso cref="iText.StyledXmlParser.Jsoup.Safety.Cleaner.Clean(iText.StyledXmlParser.Jsoup.Nodes.Document)
        ///     "/>
        public static String Clean(String bodyHtml, String baseUri, Whitelist whitelist, OutputSettings outputSettings
            ) {
            Document dirty = ParseBodyFragment(bodyHtml, baseUri);
            Cleaner cleaner = new Cleaner(whitelist);
            Document clean = cleaner.Clean(dirty);
            clean.OutputSettings(outputSettings);
            return clean.Body().Html();
        }

        /// <summary>Test if the input HTML has only tags and attributes allowed by the Whitelist.</summary>
        /// <remarks>
        /// Test if the input HTML has only tags and attributes allowed by the Whitelist. Useful for form validation. The input HTML should
        /// still be run through the cleaner to set up enforced attributes, and to tidy the output.
        /// </remarks>
        /// <param name="bodyHtml">HTML to test</param>
        /// <param name="whitelist">whitelist to test against</param>
        /// <returns>true if no tags or attributes were removed; false otherwise</returns>
        /// <seealso cref="Clean(System.String, iText.StyledXmlParser.Jsoup.Safety.Whitelist)"/>
        public static bool IsValid(String bodyHtml, Whitelist whitelist) {
            Document dirty = ParseBodyFragment(bodyHtml, "");
            Cleaner cleaner = new Cleaner(whitelist);
            return cleaner.IsValid(dirty);
        }
    }
}
