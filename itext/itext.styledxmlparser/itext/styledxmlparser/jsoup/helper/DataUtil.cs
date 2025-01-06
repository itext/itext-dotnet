/*
    This file is part of the iText (R) project.
Copyright (c) 1998-2025 Apryse Group NV
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
using System.IO.Compression;
using System.Text;
using System.Text.RegularExpressions;
using iText.Commons.Utils;
using iText.IO.Util;
using iText.StyledXmlParser.Jsoup.Internal;
using iText.StyledXmlParser.Jsoup.Nodes;
using iText.StyledXmlParser.Jsoup.Select;

namespace iText.StyledXmlParser.Jsoup.Helper {
    /// <summary>Internal static utilities for handling data.</summary>
    public sealed class DataUtil {
        private static readonly Regex charsetPattern = iText.Commons.Utils.StringUtil.RegexCompile("(?i)\\bcharset=\\s*(?:\"|')?([^\\s,;\"']*)"
            );

        public static readonly System.Text.Encoding UTF_8 = EncodingUtil.GetEncoding("UTF-8");

        // Don't use StandardCharsets, as those only appear in Android API 19, and we target 10.
        internal static readonly String defaultCharsetName = UTF_8.Name();

        // used if not found in header or meta charset
        private const int firstReadBufferSize = 1024 * 5;

        internal const int bufferSize = 1024 * 32;

        internal const int boundaryLength = 32;

        private DataUtil() {
        }

        /// <summary>Loads and parses a file to a Document.</summary>
        /// <remarks>
        /// Loads and parses a file to a Document. Files that are compressed with gzip (and end in
        /// <c>.gz</c>
        /// or
        /// <c>.z</c>
        /// )
        /// are supported in addition to uncompressed files.
        /// </remarks>
        /// <param name="in">file to load</param>
        /// <param name="charsetName">
        /// (optional) character set of input; specify
        /// <see langword="null"/>
        /// to attempt to autodetect. A BOM in
        /// the file will always override this setting.
        /// </param>
        /// <param name="baseUri">base URI of document, to resolve relative links against</param>
        /// <returns>Document</returns>
        public static Document Load(FileInfo @in, String charsetName, String baseUri) {
            Stream stream = FileUtil.GetInputStreamForFile(@in.FullName);
            String name = Normalizer.LowerCase(@in.Name);
            if (name.EndsWith(".gz") || name.EndsWith(".z")) {
                // unfortunately file input streams don't support marks (why not?), so we will close and reopen after read
                // gzip magic bytes
                bool zipped = (stream.Read() == 0x1f && stream.Read() == 0x8b);
                stream.Dispose();
                stream = zipped ?
                    CreateSeekableStream(new GZipStream(FileUtil.GetInputStreamForFile(@in.FullName), CompressionMode.Decompress))
                    : FileUtil.GetInputStreamForFile(@in.FullName);
            }
            return ParseInputStream(stream, charsetName, baseUri, iText.StyledXmlParser.Jsoup.Parser.Parser.HtmlParser
                ());
        }

        /// <summary>Parses a Document from an input steam.</summary>
        /// <param name="in">input stream to parse. You will need to close it.</param>
        /// <param name="charsetName">character set of input</param>
        /// <param name="baseUri">base URI of document, to resolve relative links against</param>
        /// <returns>Document</returns>
        public static Document Load(Stream @in, String charsetName, String baseUri) {
            return ParseInputStream(CreateSeekableStream(@in), charsetName, baseUri, iText.StyledXmlParser.Jsoup.Parser.Parser.HtmlParser());
        }

        /// <summary>Parses a Document from an input steam, using the provided Parser.</summary>
        /// <param name="in">input stream to parse. You will need to close it.</param>
        /// <param name="charsetName">character set of input</param>
        /// <param name="baseUri">base URI of document, to resolve relative links against</param>
        /// <param name="parser">
        /// alternate
        /// <see cref="iText.StyledXmlParser.Jsoup.Parser.Parser.XmlParser()">parser</see>
        /// to use.
        /// </param>
        /// <returns>Document</returns>
        public static Document Load(Stream @in, String charsetName, String baseUri, iText.StyledXmlParser.Jsoup.Parser.Parser
             parser) {
            return ParseInputStream(CreateSeekableStream(@in), charsetName, baseUri, parser);
        }

        internal static Stream CreateSeekableStream(Stream input)
        {
            MemoryStream memoryStream = new MemoryStream();
            input.CopyTo(memoryStream);
            memoryStream.Position = 0;
            return memoryStream;
        }

        /// <summary>Writes the input stream to the output stream.</summary>
        /// <remarks>Writes the input stream to the output stream. Doesn't close them.</remarks>
        /// <param name="in">input stream to read from</param>
        /// <param name="out">output stream to write to</param>
        internal static void CrossStreams(Stream @in, Stream @out) {
            byte[] buffer = new byte[bufferSize];
            int len;
            while ((len = @in.Read(buffer)) != -1) {
                @out.Write(buffer, 0, len);
            }
        }

        internal static Document ParseInputStream(Stream input, String charsetName, String baseUri, iText.StyledXmlParser.Jsoup.Parser.Parser
             parser) {
            if (input == null) {
                // empty body
                return new Document(baseUri);
            }
            input = ConstrainableInputStream.Wrap(input, bufferSize, 0);
            Document doc = null;
            // read the start of the stream and look for a BOM or meta charset
            long currentPosition = input.Position;
            ByteBuffer firstBytes = ReadToByteBuffer(input, firstReadBufferSize - 1);
            // -1 because we read one more to see if completed. First read is < buffer size, so can't be invalid.
            bool fullyRead = (input.Read() == -1);
            input.Position = currentPosition;
            // look for BOM - overrides any other header or input
            DataUtil.BomCharset bomCharset = DetectCharsetFromBom(firstBytes);
            if (bomCharset != null) {
                charsetName = bomCharset.charset;
            }
            if (charsetName == null) {
                // determine from meta. safe first parse as UTF-8
                try {
                    String defaultDecoded = UTF_8.Decode(firstBytes);
                    doc = parser.ParseInput(new StringReader(defaultDecoded), baseUri);
                }
                catch (UncheckedIOException e) {
                    throw e.IoException();
                }
                // look for <meta http-equiv="Content-Type" content="text/html;charset=gb2312"> or HTML5 <meta charset="gb2312">
                Elements metaElements = doc.Select("meta[http-equiv=content-type], meta[charset]");
                String foundCharset = null;
                // if not found, will keep utf-8 as best attempt
                foreach (iText.StyledXmlParser.Jsoup.Nodes.Element meta in metaElements) {
                    if (meta.HasAttr("http-equiv")) {
                        foundCharset = GetCharsetFromContentType(meta.Attr("content"));
                    }
                    if (foundCharset == null && meta.HasAttr("charset")) {
                        foundCharset = meta.Attr("charset");
                    }
                    if (foundCharset != null) {
                        break;
                    }
                }
                // look for <?xml encoding='ISO-8859-1'?>
                if (foundCharset == null && doc.ChildNodeSize() > 0) {
                    iText.StyledXmlParser.Jsoup.Nodes.Node first = doc.ChildNode(0);
                    XmlDeclaration decl = null;
                    if (first is XmlDeclaration) {
                        decl = (XmlDeclaration)first;
                    }
                    else {
                        if (first is Comment) {
                            Comment comment = (Comment)first;
                            if (comment.IsXmlDeclaration()) {
                                decl = comment.AsXmlDeclaration();
                            }
                        }
                    }
                    if (decl != null) {
                        if (decl.Name().EqualsIgnoreCase("xml")) {
                            foundCharset = decl.Attr("encoding");
                        }
                    }
                }
                foundCharset = ValidateCharset(foundCharset);
                if (foundCharset != null && !foundCharset.EqualsIgnoreCase(defaultCharsetName)) {
                    // need to re-decode. (case insensitive check here to match how validate works)
                    foundCharset = iText.Commons.Utils.StringUtil.ReplaceAll(foundCharset.Trim(), "[\"']", "");
                    charsetName = foundCharset;
                    doc = null;
                }
                else {
                    if (!fullyRead) {
                        doc = null;
                    }
                }
            }
            else {
                // specified by content type header (or by user on file load)
                Validate.NotEmpty(charsetName, "Must set charset arg to character set of file to parse. Set to null to attempt to detect from HTML"
                    );
            }
            if (doc == null) {
                if (charsetName == null) {
                    charsetName = defaultCharsetName;
                }
                Encoding charset = charsetName.Equals(defaultCharsetName) ? UTF_8 : EncodingUtil.GetEncoding(charsetName);
                StreamReader reader = new StreamReader(input, charset);
                try {
                    doc = parser.ParseInput(reader, baseUri);
                }
                catch (UncheckedIOException e) {
                    // io exception when parsing (not seen before because reading the stream as we go)
                    throw e.IoException();
                }
                doc.OutputSettings().Charset(charset);
                if (!charset.CanEncode('a')) {
                    // some charsets can read but not encode; switch to an encodable charset and update the meta el
                    doc.Charset(UTF_8);
                }
            }
            input.Dispose();
            return doc;
        }

        /// <summary>Read the input stream into a byte buffer.</summary>
        /// <remarks>
        /// Read the input stream into a byte buffer. To deal with slow input streams, you may interrupt the thread this
        /// method is executing on. The data read until being interrupted will be available.
        /// </remarks>
        /// <param name="inStream">the input stream to read from</param>
        /// <param name="maxSize">the maximum size in bytes to read from the stream. Set to 0 to be unlimited.</param>
        /// <returns>the filled byte buffer</returns>
        public static ByteBuffer ReadToByteBuffer(Stream inStream, int maxSize) {
            Validate.IsTrue(maxSize >= 0, "maxSize must be 0 (unlimited) or larger");
            ConstrainableInputStream input = ConstrainableInputStream.Wrap(inStream, bufferSize, maxSize);
            return input.ReadToByteBuffer(maxSize);
        }

        /// <summary>Parse out a charset from a content type header.</summary>
        /// <remarks>
        /// Parse out a charset from a content type header. If the charset is not supported, returns null (so the default
        /// will kick in.)
        /// </remarks>
        /// <param name="contentType">e.g. "text/html; charset=EUC-JP"</param>
        /// <returns>"EUC-JP", or null if not found. Charset is trimmed and uppercased.</returns>
        internal static String GetCharsetFromContentType(String contentType) {
            if (contentType == null) {
                return null;
            }
            Matcher m = Matcher.Match(charsetPattern, contentType);
            if (m.Find()) {
                String charset = m.Group(1).Trim();
                charset = charset.Replace("charset=", "");
                return ValidateCharset(charset);
            }
            return null;
        }

        private static String ValidateCharset(String cs) {
            if (cs == null || cs.Length == 0) {
                return null;
            }
            cs = iText.Commons.Utils.StringUtil.ReplaceAll(cs.Trim(), "[\"']", "");
            if (PortUtil.CharsetIsSupported(cs)) {
                return cs;
            }
            cs = cs.ToUpper(System.Globalization.CultureInfo.InvariantCulture);
            if (PortUtil.CharsetIsSupported(cs)) {
                return cs;
            }
            return null;
        }

        private static DataUtil.BomCharset DetectCharsetFromBom(ByteBuffer byteData) {
            // .mark and rewind used to return Buffer, now ByteBuffer, so cast for backward compat
            byteData.Mark();
            byte[] bom = new byte[4];
            if (byteData.Remaining() >= bom.Length) {
                byteData.Get(bom);
                byteData.Rewind();
            }
            if (bom[0] == 0x00 && bom[1] == 0x00 && bom[2] == (byte)0xFE && bom[3] == (byte)0xFF || 
                        // BE
                        bom[0] == (byte)0xFF && bom[1] == (byte)0xFE && bom[2] == 0x00 && bom[3] == 0x00) {
                // LE
                return new DataUtil.BomCharset("UTF-32", false);
            }
            else {
                // and I hope it's on your system
                if (bom[0] == (byte)0xFE && bom[1] == (byte)0xFF || 
                                // BE
                                bom[0] == (byte)0xFF && bom[1] == (byte)0xFE) {
                    return new DataUtil.BomCharset("UTF-16", false);
                }
                else {
                    // in all Javas
                    if (bom[0] == (byte)0xEF && bom[1] == (byte)0xBB && bom[2] == (byte)0xBF) {
                        return new DataUtil.BomCharset("UTF-8", true);
                    }
                }
            }
            // in all Javas
            // 16 and 32 decoders consume the BOM to determine be/le; utf-8 should be consumed here
            return null;
        }

        private class BomCharset {
            internal readonly String charset;

            internal readonly bool offset;

            public BomCharset(String charset, bool offset) {
                this.charset = charset;
                this.offset = offset;
            }
        }
    }
}
