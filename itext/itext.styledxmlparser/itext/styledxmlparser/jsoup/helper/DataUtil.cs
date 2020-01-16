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
using System.Text;
using System.Text.RegularExpressions;
using iText.IO.Util;
using iText.StyledXmlParser.Jsoup;
using iText.StyledXmlParser.Jsoup.Nodes;

namespace iText.StyledXmlParser.Jsoup.Helper {
    /// <summary>Internal static utilities for handling data.</summary>
    public sealed class DataUtil {
        private static readonly Regex charsetPattern = iText.IO.Util.StringUtil.RegexCompile("(?i)\\bcharset=\\s*(?:\"|')?([^\\s,;\"']*)"
            );

        internal const String defaultCharset = "UTF-8";

        private const int bufferSize = 0x20000;

        private const int UNICODE_BOM = 0xFEFF;

        private static readonly char[] mimeBoundaryChars = "-_1234567890abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ"
            .ToCharArray();

        internal const int boundaryLength = 32;

        private DataUtil() {
        }

        // used if not found in header or meta charset
        // ~130K.
        /// <summary>Loads a file to a Document.</summary>
        /// <param name="in">file to load</param>
        /// <param name="charsetName">character set of input</param>
        /// <param name="baseUri">base URI of document, to resolve relative links against</param>
        /// <returns>Document</returns>
        public static Document Load(FileInfo @in, String charsetName, String baseUri) {
            ByteBuffer byteData = ReadFileToByteBuffer(@in);
            return ParseByteData(byteData, charsetName, baseUri, iText.StyledXmlParser.Jsoup.Parser.Parser.HtmlParser(
                ));
        }

        /// <summary>Parses a Document from an input steam.</summary>
        /// <param name="in">input stream to parse. You will need to close it.</param>
        /// <param name="charsetName">character set of input</param>
        /// <param name="baseUri">base URI of document, to resolve relative links against</param>
        /// <returns>Document</returns>
        public static Document Load(Stream @in, String charsetName, String baseUri) {
            ByteBuffer byteData = ReadToByteBuffer(@in);
            return ParseByteData(byteData, charsetName, baseUri, iText.StyledXmlParser.Jsoup.Parser.Parser.HtmlParser(
                ));
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
            ByteBuffer byteData = ReadToByteBuffer(@in);
            return ParseByteData(byteData, charsetName, baseUri, parser);
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

        // reads bytes first into a buffer, then decodes with the appropriate charset. done this way to support
        // switching the chartset midstream when a meta http-equiv tag defines the charset.
        // todo - this is getting gnarly. needs a rewrite.
        internal static Document ParseByteData(ByteBuffer byteData, String charsetName, String baseUri, iText.StyledXmlParser.Jsoup.Parser.Parser
             parser) {
            String docData;
            Document doc = null;
            // look for BOM - overrides any other header or input
            charsetName = DetectCharsetFromBom(byteData, charsetName);
            if (charsetName == null) {
                // determine from meta. safe first parse as UTF-8
                // look for <meta http-equiv="Content-Type" content="text/html;charset=gb2312"> or HTML5 <meta charset="gb2312">
                docData = EncodingUtil.GetEncoding(defaultCharset).Decode(byteData).ToString();
                doc = parser.ParseInput(docData, baseUri);
                iText.StyledXmlParser.Jsoup.Nodes.Element meta = doc.Select("meta[http-equiv=content-type], meta[charset]"
                    ).First();
                String foundCharset = null;
                // if not found, will keep utf-8 as best attempt
                if (meta != null) {
                    if (meta.HasAttr("http-equiv")) {
                        foundCharset = GetCharsetFromContentType(meta.Attr("content"));
                    }
                    if (foundCharset == null && meta.HasAttr("charset")) {
                        foundCharset = meta.Attr("charset");
                    }
                }
                // look for <?xml encoding='ISO-8859-1'?>
                if (foundCharset == null && doc.ChildNode(0) is XmlDeclaration) {
                    XmlDeclaration prolog = (XmlDeclaration)doc.ChildNode(0);
                    if (prolog.Name().Equals("xml")) {
                        foundCharset = prolog.Attr("encoding");
                    }
                }
                foundCharset = ValidateCharset(foundCharset);
                if (foundCharset != null && !foundCharset.Equals(defaultCharset)) {
                    // need to re-decode
                    foundCharset = iText.IO.Util.StringUtil.ReplaceAll(foundCharset.Trim(), "[\"']", "");
                    charsetName = foundCharset;
                    byteData.Rewind();
                    docData = EncodingUtil.GetEncoding(foundCharset).Decode(byteData).ToString();
                    doc = null;
                }
            }
            else {
                // specified by content type header (or by user on file load)
                Validate.NotEmpty(charsetName, "Must set charset arg to character set of file to parse. Set to null to attempt to detect from HTML"
                    );
                docData = EncodingUtil.GetEncoding(charsetName).Decode(byteData).ToString();
            }
            if (doc == null) {
                doc = parser.ParseInput(docData, baseUri);
                doc.OutputSettings().Charset(charsetName);
            }
            return doc;
        }

        /// <summary>Read the input stream into a byte buffer.</summary>
        /// <param name="inStream">the input stream to read from</param>
        /// <param name="maxSize">the maximum size in bytes to read from the stream. Set to 0 to be unlimited.</param>
        /// <returns>the filled byte buffer</returns>
        internal static ByteBuffer ReadToByteBuffer(Stream inStream, int maxSize) {
            Validate.IsTrue(maxSize >= 0, "maxSize must be 0 (unlimited) or larger");
            bool capped = maxSize > 0;
            byte[] buffer = new byte[bufferSize];
            MemoryStream outStream = new MemoryStream(bufferSize);
            int read;
            int remaining = maxSize;
            while (true) {
                read = inStream.Read(buffer);
                if (read == -1) {
                    break;
                }
                if (capped) {
                    if (read > remaining) {
                        outStream.Write(buffer, 0, remaining);
                        break;
                    }
                    remaining -= read;
                }
                outStream.Write(buffer, 0, read);
            }
            return ByteBuffer.Wrap(outStream.ToArray());
        }

        internal static ByteBuffer ReadToByteBuffer(Stream inStream) {
            return ReadToByteBuffer(inStream, 0);
        }

        internal static ByteBuffer ReadFileToByteBuffer(FileInfo file) {
            FileStream randomAccessFile = null;
            try {
                randomAccessFile = PortUtil.GetReadOnlyRandomAccesFile(file);
                byte[] bytes = new byte[(int)randomAccessFile.Length];
                randomAccessFile.ReadFully(bytes);
                return ByteBuffer.Wrap(bytes);
            }
            finally {
                if (randomAccessFile != null) {
                    randomAccessFile.Dispose();
                }
            }
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
            Match m = iText.IO.Util.StringUtil.Match(charsetPattern, contentType);
            if (PortUtil.IsSuccessful(m)) {
                String charset = iText.IO.Util.StringUtil.Group(m, 1).Trim();
                charset = charset.Replace("charset=", "");
                return ValidateCharset(charset);
            }
            return null;
        }

        private static String ValidateCharset(String cs) {
            if (cs == null || cs.Length == 0) {
                return null;
            }
            cs = iText.IO.Util.StringUtil.ReplaceAll(cs.Trim(), "[\"']", "");
            if (PortUtil.CharsetIsSupported(cs)) {
                return cs;
            }
            StringBuilder upperCase = new StringBuilder();
            for (int i = 0; i < cs.Length; i++) {
                upperCase.Append(char.ToUpper(cs[i]));
            }
            cs = upperCase.ToString();
            if (PortUtil.CharsetIsSupported(cs)) {
                return cs;
            }
            // if our this charset matching fails.... we just take the default
            return null;
        }

        /// <summary>Creates a random string, suitable for use as a mime boundary</summary>
        internal static String MimeBoundary() {
            StringBuilder mime = new StringBuilder(boundaryLength);
            Random rand = new Random();
            for (int i = 0; i < boundaryLength; i++) {
                mime.Append(mimeBoundaryChars[rand.Next(mimeBoundaryChars.Length)]);
            }
            return mime.ToString();
        }

        private static String DetectCharsetFromBom(ByteBuffer byteData, String charsetName) {
            byteData.Mark();
            byte[] bom = new byte[4];
            if (byteData.Remaining() >= bom.Length) {
                byteData.Get(bom);
                byteData.Rewind();
            }
            if (bom[0] == 0x00 && bom[1] == 0x00 && bom[2] == (byte)0xFE && bom[3] == (byte)0xFF || bom[0] == (byte)0xFF
                 && bom[1] == (byte)0xFE && bom[2] == 0x00 && bom[3] == 0x00) {
                // BE
                // LE
                charsetName = "UTF-32";
            }
            else {
                // and I hope it's on your system
                if (bom[0] == (byte)0xFE && bom[1] == (byte)0xFF || bom[0] == (byte)0xFF && bom[1] == (byte)0xFE) {
                    // BE
                    charsetName = "UTF-16";
                }
                else {
                    // in all Javas
                    if (bom[0] == (byte)0xEF && bom[1] == (byte)0xBB && bom[2] == (byte)0xBF) {
                        charsetName = "UTF-8";
                        // in all Javas
                        byteData.Position(3);
                    }
                }
            }
            // 16 and 32 decoders consume the BOM to determine be/le; utf-8 should be consumed here
            return charsetName;
        }
    }
}
