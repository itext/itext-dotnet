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
using System.Text;
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Commons.Utils;
using iText.IO.Exceptions;

namespace iText.IO.Source {
    public class PdfTokenizer : IDisposable {
        public enum TokenType {
            Number,
            String,
            Name,
            Comment,
            StartArray,
            EndArray,
            StartDic,
            EndDic,
            Ref,
            Obj,
            EndObj,
            Other,
            EndOfFile
        }

        public static readonly bool[] delims = new bool[] { true, true, false, false, false, false, false, false, 
            false, false, true, true, false, true, true, false, false, false, false, false, false, false, false, false
            , false, false, false, false, false, false, false, false, false, true, false, false, false, false, true
            , false, false, true, true, false, false, false, false, false, true, false, false, false, false, false
            , false, false, false, false, false, false, false, true, false, true, false, false, false, false, false
            , false, false, false, false, false, false, false, false, false, false, false, false, false, false, false
            , false, false, false, false, false, false, false, false, true, false, true, false, false, false, false
            , false, false, false, false, false, false, false, false, false, false, false, false, false, false, false
            , false, false, false, false, false, false, false, false, false, false, false, false, false, false, false
            , false, false, false, false, false, false, false, false, false, false, false, false, false, false, false
            , false, false, false, false, false, false, false, false, false, false, false, false, false, false, false
            , false, false, false, false, false, false, false, false, false, false, false, false, false, false, false
            , false, false, false, false, false, false, false, false, false, false, false, false, false, false, false
            , false, false, false, false, false, false, false, false, false, false, false, false, false, false, false
            , false, false, false, false, false, false, false, false, false, false, false, false, false, false, false
            , false, false, false, false, false, false, false, false, false, false, false, false, false, false, false
            , false, false, false, false, false, false, false, false, false, false, false, false, false, false, false
            , false, false, false, false, false, false, false, false };

        public static readonly byte[] Obj = ByteUtils.GetIsoBytes("obj");

        public static readonly byte[] R = ByteUtils.GetIsoBytes("R");

        public static readonly byte[] Xref = ByteUtils.GetIsoBytes("xref");

        public static readonly byte[] Startxref = ByteUtils.GetIsoBytes("startxref");

        public static readonly byte[] Stream = ByteUtils.GetIsoBytes("stream");

        public static readonly byte[] Trailer = ByteUtils.GetIsoBytes("trailer");

        public static readonly byte[] N = ByteUtils.GetIsoBytes("n");

        public static readonly byte[] F = ByteUtils.GetIsoBytes("f");

        public static readonly byte[] Null = ByteUtils.GetIsoBytes("null");

        public static readonly byte[] True = ByteUtils.GetIsoBytes("true");

        public static readonly byte[] False = ByteUtils.GetIsoBytes("false");

        protected internal PdfTokenizer.TokenType type;

        protected internal int reference;

        protected internal int generation;

        protected internal bool hexString;

        protected internal ByteBuffer outBuf;

        private readonly RandomAccessFileOrArray file;

        /// <summary>Streams are closed automatically.</summary>
        private bool closeStream = true;

        /// <summary>
        /// Creates a PdfTokenizer for the specified
        /// <see cref="RandomAccessFileOrArray"/>.
        /// </summary>
        /// <remarks>
        /// Creates a PdfTokenizer for the specified
        /// <see cref="RandomAccessFileOrArray"/>.
        /// The beginning of the file is read to determine the location of the header, and the data source is adjusted
        /// as necessary to account for any junk that occurs in the byte source before the header
        /// </remarks>
        /// <param name="file">the source</param>
        public PdfTokenizer(RandomAccessFileOrArray file) {
            this.file = file;
            this.outBuf = new ByteBuffer();
        }

        public virtual void Seek(long pos) {
            file.Seek(pos);
        }

        public virtual void ReadFully(byte[] bytes) {
            file.ReadFully(bytes);
        }

        public virtual long GetPosition() {
            return file.GetPosition();
        }

        public virtual void Close() {
            if (closeStream) {
                file.Close();
            }
        }

        public virtual long Length() {
            return file.Length();
        }

        public virtual int Read() {
            return file.Read();
        }

        public virtual String ReadString(int size) {
            StringBuilder buf = new StringBuilder();
            int ch;
            while ((size--) > 0) {
                ch = Read();
                if (ch == -1) {
                    break;
                }
                buf.Append((char)ch);
            }
            return buf.ToString();
        }

        public virtual PdfTokenizer.TokenType GetTokenType() {
            return type;
        }

        public virtual byte[] GetByteContent() {
            return outBuf.ToByteArray();
        }

        public virtual String GetStringValue() {
            return iText.Commons.Utils.JavaUtil.GetStringForBytes(outBuf.GetInternalBuffer(), 0, outBuf.Size());
        }

        public virtual byte[] GetDecodedStringContent() {
            return DecodeStringContent(outBuf.GetInternalBuffer(), 0, outBuf.Size() - 1, IsHexString());
        }

        public virtual bool TokenValueEqualsTo(byte[] cmp) {
            if (cmp == null) {
                return false;
            }
            int size = cmp.Length;
            if (outBuf.Size() != size) {
                return false;
            }
            for (int i = 0; i < size; i++) {
                if (cmp[i] != outBuf.GetInternalBuffer()[i]) {
                    return false;
                }
            }
            return true;
        }

        public virtual int GetObjNr() {
            return reference;
        }

        public virtual int GetGenNr() {
            return generation;
        }

        public virtual void BackOnePosition(int ch) {
            if (ch != -1) {
                file.PushBack((byte)ch);
            }
        }

        public virtual int GetHeaderOffset() {
            String str = ReadString(1024);
            int idx = str.IndexOf("%PDF-", StringComparison.Ordinal);
            if (idx < 0) {
                idx = str.IndexOf("%FDF-", StringComparison.Ordinal);
                if (idx < 0) {
                    throw new iText.IO.Exceptions.IOException(IoExceptionMessageConstant.PDF_HEADER_NOT_FOUND, this);
                }
            }
            return idx;
        }

        public virtual String CheckPdfHeader() {
            file.Seek(0);
            String str = ReadString(1024);
            int idx = str.IndexOf("%PDF-", StringComparison.Ordinal);
            if (idx != 0) {
                throw new iText.IO.Exceptions.IOException(IoExceptionMessageConstant.PDF_HEADER_NOT_FOUND, this);
            }
            return str.JSubstring(idx + 1, idx + 8);
        }

        public virtual void CheckFdfHeader() {
            file.Seek(0);
            String str = ReadString(1024);
            int idx = str.IndexOf("%FDF-", StringComparison.Ordinal);
            if (idx != 0) {
                throw new iText.IO.Exceptions.IOException(IoExceptionMessageConstant.FDF_STARTXREF_NOT_FOUND, this);
            }
        }

        public virtual long GetStartxref() {
            int arrLength = 1024;
            long fileLength = file.Length();
            long pos = fileLength - arrLength;
            if (pos < 1) {
                pos = 1;
            }
            while (pos > 0) {
                file.Seek(pos);
                String str = ReadString(arrLength);
                int idx = str.LastIndexOf("startxref");
                if (idx >= 0) {
                    return pos + idx;
                }
                // 9 = "startxref".length()
                pos = pos - arrLength + 9;
            }
            throw new iText.IO.Exceptions.IOException(IoExceptionMessageConstant.PDF_STARTXREF_NOT_FOUND, this);
        }

        public virtual void NextValidToken() {
            int level = 0;
            byte[] n1 = null;
            byte[] n2 = null;
            long ptr = 0;
            while (NextToken()) {
                if (type == PdfTokenizer.TokenType.Comment) {
                    continue;
                }
                switch (level) {
                    case 0: {
                        if (type != PdfTokenizer.TokenType.Number) {
                            return;
                        }
                        ptr = file.GetPosition();
                        n1 = GetByteContent();
                        ++level;
                        break;
                    }

                    case 1: {
                        if (type != PdfTokenizer.TokenType.Number) {
                            file.Seek(ptr);
                            type = PdfTokenizer.TokenType.Number;
                            outBuf.Reset().Append(n1);
                            return;
                        }
                        n2 = GetByteContent();
                        ++level;
                        break;
                    }

                    case 2: {
                        if (type == PdfTokenizer.TokenType.Other) {
                            if (TokenValueEqualsTo(R)) {
                                System.Diagnostics.Debug.Assert(n2 != null);
                                type = PdfTokenizer.TokenType.Ref;
                                try {
                                    reference = Convert.ToInt32(iText.Commons.Utils.JavaUtil.GetStringForBytes(n1), System.Globalization.CultureInfo.InvariantCulture
                                        );
                                    generation = Convert.ToInt32(iText.Commons.Utils.JavaUtil.GetStringForBytes(n2), System.Globalization.CultureInfo.InvariantCulture
                                        );
                                }
                                catch (Exception) {
                                    //warn about incorrect reference number
                                    //Exception: NumberFormatException for java, FormatException or OverflowException for .NET
                                    ILogger logger = ITextLogManager.GetLogger(typeof(PdfTokenizer));
                                    logger.LogError(MessageFormatUtil.Format(iText.IO.Logs.IoLogMessageConstant.INVALID_INDIRECT_REFERENCE, iText.Commons.Utils.JavaUtil.GetStringForBytes
                                        (n1), iText.Commons.Utils.JavaUtil.GetStringForBytes(n2)));
                                    reference = -1;
                                    generation = 0;
                                }
                                return;
                            }
                            else {
                                if (TokenValueEqualsTo(Obj)) {
                                    System.Diagnostics.Debug.Assert(n2 != null);
                                    type = PdfTokenizer.TokenType.Obj;
                                    reference = Convert.ToInt32(iText.Commons.Utils.JavaUtil.GetStringForBytes(n1), System.Globalization.CultureInfo.InvariantCulture
                                        );
                                    generation = Convert.ToInt32(iText.Commons.Utils.JavaUtil.GetStringForBytes(n2), System.Globalization.CultureInfo.InvariantCulture
                                        );
                                    return;
                                }
                            }
                        }
                        file.Seek(ptr);
                        type = PdfTokenizer.TokenType.Number;
                        outBuf.Reset().Append(n1);
                        return;
                    }
                }
            }
            // if the level 1 check returns EOF,
            // then we are still looking at a number - set the type back to Number
            if (level == 1) {
                type = PdfTokenizer.TokenType.Number;
                outBuf.Reset().Append(n1);
            }
        }

        // if we hit here, the file is either corrupt (stream ended unexpectedly),
        // or the last token ended exactly at the end of a stream.  This last
        // case can occur inside an Object Stream.
        public virtual bool NextToken() {
            int ch;
            outBuf.Reset();
            do {
                ch = file.Read();
            }
            while (ch != -1 && IsWhitespace(ch));
            if (ch == -1) {
                type = PdfTokenizer.TokenType.EndOfFile;
                return false;
            }
            switch (ch) {
                case '[': {
                    type = PdfTokenizer.TokenType.StartArray;
                    break;
                }

                case ']': {
                    type = PdfTokenizer.TokenType.EndArray;
                    break;
                }

                case '/': {
                    type = PdfTokenizer.TokenType.Name;
                    while (true) {
                        ch = file.Read();
                        if (delims[ch + 1]) {
                            break;
                        }
                        outBuf.Append(ch);
                    }
                    BackOnePosition(ch);
                    break;
                }

                case '>': {
                    ch = file.Read();
                    if (ch != '>') {
                        ThrowError(IoExceptionMessageConstant.GT_NOT_EXPECTED);
                    }
                    type = PdfTokenizer.TokenType.EndDic;
                    break;
                }

                case '<': {
                    int v1 = file.Read();
                    if (v1 == '<') {
                        type = PdfTokenizer.TokenType.StartDic;
                        break;
                    }
                    type = PdfTokenizer.TokenType.String;
                    hexString = true;
                    int v2 = 0;
                    while (true) {
                        while (IsWhitespace(v1)) {
                            v1 = file.Read();
                        }
                        if (v1 == '>') {
                            break;
                        }
                        outBuf.Append(v1);
                        v1 = ByteBuffer.GetHex(v1);
                        if (v1 < 0) {
                            break;
                        }
                        v2 = file.Read();
                        while (IsWhitespace(v2)) {
                            v2 = file.Read();
                        }
                        if (v2 == '>') {
                            break;
                        }
                        outBuf.Append(v2);
                        v2 = ByteBuffer.GetHex(v2);
                        if (v2 < 0) {
                            break;
                        }
                        v1 = file.Read();
                    }
                    if (v1 < 0 || v2 < 0) {
                        ThrowError(IoExceptionMessageConstant.ERROR_READING_STRING);
                    }
                    break;
                }

                case '%': {
                    type = PdfTokenizer.TokenType.Comment;
                    do {
                        ch = file.Read();
                    }
                    while (ch != -1 && ch != '\r' && ch != '\n');
                    break;
                }

                case '(': {
                    type = PdfTokenizer.TokenType.String;
                    hexString = false;
                    int nesting = 0;
                    while (true) {
                        ch = file.Read();
                        if (ch == -1) {
                            break;
                        }
                        if (ch == '(') {
                            ++nesting;
                        }
                        else {
                            if (ch == ')') {
                                --nesting;
                                if (nesting == -1) {
                                    break;
                                }
                            }
                            else {
                                if (ch == '\\') {
                                    outBuf.Append('\\');
                                    ch = file.Read();
                                    if (ch < 0) {
                                        break;
                                    }
                                }
                            }
                        }
                        outBuf.Append(ch);
                    }
                    if (ch == -1) {
                        ThrowError(IoExceptionMessageConstant.ERROR_READING_STRING);
                    }
                    break;
                }

                default: {
                    if (ch == '-' || ch == '+' || ch == '.' || (ch >= '0' && ch <= '9')) {
                        type = PdfTokenizer.TokenType.Number;
                        bool isReal = false;
                        int numberOfMinuses = 0;
                        if (ch == '-') {
                            // Take care of number like "--234". If Acrobat can read them so must we.
                            do {
                                ++numberOfMinuses;
                                ch = file.Read();
                            }
                            while (ch == '-');
                            outBuf.Append('-');
                        }
                        else {
                            outBuf.Append(ch);
                            // We don't need to check if the number is real over here
                            // as we need to know that fact only in case if there are any minuses.
                            ch = file.Read();
                        }
                        while (ch >= '0' && ch <= '9') {
                            outBuf.Append(ch);
                            ch = file.Read();
                        }
                        if (ch == '.') {
                            isReal = true;
                            outBuf.Append(ch);
                            ch = file.Read();
                            //verify if there is minus after '.'
                            //In that case just ignore minus chars and everything after as Adobe Reader does
                            int numberOfMinusesAfterDot = 0;
                            if (ch == '-') {
                                numberOfMinusesAfterDot++;
                                ch = file.Read();
                            }
                            while (ch >= '0' && ch <= '9') {
                                if (numberOfMinusesAfterDot == 0) {
                                    outBuf.Append(ch);
                                }
                                ch = file.Read();
                            }
                        }
                        if (numberOfMinuses > 1 && !isReal) {
                            // Numbers of integer type and with more than one minus before them
                            // are interpreted by Acrobat as zero.
                            outBuf.Reset();
                            outBuf.Append('0');
                        }
                    }
                    else {
                        type = PdfTokenizer.TokenType.Other;
                        do {
                            outBuf.Append(ch);
                            ch = file.Read();
                        }
                        while (!delims[ch + 1]);
                    }
                    if (ch != -1) {
                        BackOnePosition(ch);
                    }
                    break;
                }
            }
            return true;
        }

        public virtual long GetLongValue() {
            return Convert.ToInt64(GetStringValue(), System.Globalization.CultureInfo.InvariantCulture);
        }

        public virtual int GetIntValue() {
            return Convert.ToInt32(GetStringValue(), System.Globalization.CultureInfo.InvariantCulture);
        }

        public virtual bool IsHexString() {
            return this.hexString;
        }

        public virtual bool IsCloseStream() {
            return closeStream;
        }

        public virtual void SetCloseStream(bool closeStream) {
            this.closeStream = closeStream;
        }

        public virtual RandomAccessFileOrArray GetSafeFile() {
            return file.CreateView();
        }

        /// <summary>Resolve escape symbols or hexadecimal symbols.</summary>
        /// <remarks>
        /// Resolve escape symbols or hexadecimal symbols.
        /// <para />
        /// NOTE Due to PdfReference 1.7 part 3.2.3 String value contain ASCII characters,
        /// so we can convert it directly to byte array.
        /// </remarks>
        /// <param name="content">string bytes to be decoded</param>
        /// <param name="from">given start index</param>
        /// <param name="to">given end index</param>
        /// <param name="hexWriting">
        /// true if given string is hex-encoded, e.g. '&lt;69546578…&gt;'.
        /// False otherwise, e.g. '((iText( some version)…)'
        /// </param>
        /// <returns>
        /// byte[] for decrypting or for creating
        /// <see cref="System.String"/>.
        /// </returns>
        protected internal static byte[] DecodeStringContent(byte[] content, int from, int to, bool hexWriting) {
            ByteBuffer buffer = new ByteBuffer(to - from + 1);
            // <6954657874ae...>
            if (hexWriting) {
                for (int i = from; i <= to; ) {
                    int v1 = ByteBuffer.GetHex(content[i++]);
                    if (i > to) {
                        buffer.Append(v1 << 4);
                        break;
                    }
                    int v2 = content[i++];
                    v2 = ByteBuffer.GetHex(v2);
                    buffer.Append((v1 << 4) + v2);
                }
            }
            else {
                // ((iText\( some version)...)
                for (int i = from; i <= to; ) {
                    int ch = content[i++];
                    if (ch == '\\') {
                        bool lineBreak = false;
                        ch = content[i++];
                        switch (ch) {
                            case 'n': {
                                ch = '\n';
                                break;
                            }

                            case 'r': {
                                ch = '\r';
                                break;
                            }

                            case 't': {
                                ch = '\t';
                                break;
                            }

                            case 'b': {
                                ch = '\b';
                                break;
                            }

                            case 'f': {
                                ch = '\f';
                                break;
                            }

                            case '(':
                            case ')':
                            case '\\': {
                                break;
                            }

                            case '\r': {
                                lineBreak = true;
                                if (i <= to && content[i++] != '\n') {
                                    i--;
                                }
                                break;
                            }

                            case '\n': {
                                lineBreak = true;
                                break;
                            }

                            default: {
                                if (ch < '0' || ch > '7') {
                                    break;
                                }
                                int octal = ch - '0';
                                ch = content[i++];
                                if (ch < '0' || ch > '7') {
                                    i--;
                                    ch = octal;
                                    break;
                                }
                                octal = (octal << 3) + ch - '0';
                                ch = content[i++];
                                if (ch < '0' || ch > '7') {
                                    i--;
                                    ch = octal;
                                    break;
                                }
                                octal = (octal << 3) + ch - '0';
                                ch = octal & 0xff;
                                break;
                            }
                        }
                        if (lineBreak) {
                            continue;
                        }
                    }
                    else {
                        if (ch == '\r') {
                            // in this case current char is '\n' and we have to skip next '\n' if it presents.
                            ch = '\n';
                            if (i <= to && content[i++] != '\n') {
                                i--;
                            }
                        }
                    }
                    buffer.Append(ch);
                }
            }
            return buffer.ToByteArray();
        }

        /// <summary>Resolve escape symbols or hexadecimal symbols.</summary>
        /// <remarks>
        /// Resolve escape symbols or hexadecimal symbols.
        /// <br />
        /// NOTE Due to PdfReference 1.7 part 3.2.3 String value contain ASCII characters,
        /// so we can convert it directly to byte array.
        /// </remarks>
        /// <param name="content">string bytes to be decoded</param>
        /// <param name="hexWriting">
        /// true if given string is hex-encoded, e.g. '&lt;69546578…&gt;'.
        /// False otherwise, e.g. '((iText( some version)…)'
        /// </param>
        /// <returns>
        /// byte[] for decrypting or for creating
        /// <see cref="System.String"/>.
        /// </returns>
        public static byte[] DecodeStringContent(byte[] content, bool hexWriting) {
            return DecodeStringContent(content, 0, content.Length - 1, hexWriting);
        }

        /// <summary>Is a certain character a whitespace? Currently checks on the following: '0', '9', '10', '12', '13', '32'.
        ///     </summary>
        /// <remarks>
        /// Is a certain character a whitespace? Currently checks on the following: '0', '9', '10', '12', '13', '32'.
        /// <br />
        /// The same as calling
        /// <see cref="IsWhitespace(int, bool)">isWhiteSpace(ch, true)</see>.
        /// </remarks>
        /// <param name="ch">int</param>
        /// <returns>boolean</returns>
        public static bool IsWhitespace(int ch) {
            return IsWhitespace(ch, true);
        }

        /// <summary>Checks whether a character is a whitespace.</summary>
        /// <remarks>Checks whether a character is a whitespace. Currently checks on the following: '0', '9', '10', '12', '13', '32'.
        ///     </remarks>
        /// <param name="ch">int</param>
        /// <param name="isWhitespace">boolean</param>
        /// <returns>boolean</returns>
        protected internal static bool IsWhitespace(int ch, bool isWhitespace) {
            return ((isWhitespace && ch == 0) || ch == 9 || ch == 10 || ch == 12 || ch == 13 || ch == 32);
        }

        protected internal static bool IsDelimiter(int ch) {
            return (ch == '(' || ch == ')' || ch == '<' || ch == '>' || ch == '[' || ch == ']' || ch == '/' || ch == '%'
                );
        }

        protected internal static bool IsDelimiterWhitespace(int ch) {
            return delims[ch + 1];
        }

        /// <summary>Helper method to handle content errors.</summary>
        /// <remarks>
        /// Helper method to handle content errors. Add file position to
        /// <c>PdfRuntimeException</c>.
        /// </remarks>
        /// <param name="error">message.</param>
        /// <param name="messageParams">error params.</param>
        public virtual void ThrowError(String error, params Object[] messageParams) {
            throw new iText.IO.Exceptions.IOException(IoExceptionMessageConstant.ERROR_AT_FILE_POINTER, new iText.IO.Exceptions.IOException
                (error).SetMessageParams(messageParams)).SetMessageParams(file.GetPosition());
        }

        /// <summary>
        /// Checks whether
        /// <paramref name="line"/>
        /// equals to 'trailer'.
        /// </summary>
        /// <param name="line">for check</param>
        /// <returns>true, if line is equals to 'trailer', otherwise false</returns>
        public static bool CheckTrailer(ByteBuffer line) {
            if (Trailer.Length > line.Size()) {
                return false;
            }
            for (int i = 0; i < Trailer.Length; i++) {
                if (Trailer[i] != line.Get(i)) {
                    return false;
                }
            }
            return true;
        }

        /// <summary>Reads data into the provided byte[].</summary>
        /// <remarks>
        /// Reads data into the provided byte[]. Checks on leading whitespace.
        /// See
        /// <see cref="IsWhitespace(int)">isWhiteSpace(int)</see>
        /// or
        /// <see cref="IsWhitespace(int, bool)">isWhiteSpace(int, boolean)</see>
        /// for a list of whitespace characters.
        /// <br />
        /// The same as calling
        /// <see cref="ReadLineSegment(ByteBuffer, bool)">readLineSegment(input, true)</see>.
        /// </remarks>
        /// <param name="buffer">
        /// a
        /// <see cref="ByteBuffer"/>
        /// to which the result of reading will be saved
        /// </param>
        /// <returns>true, if something was read or if the end of the input stream is not reached</returns>
        public virtual bool ReadLineSegment(ByteBuffer buffer) {
            return ReadLineSegment(buffer, true);
        }

        /// <summary>Reads data into the provided byte[].</summary>
        /// <remarks>
        /// Reads data into the provided byte[]. Checks on leading whitespace.
        /// See
        /// <see cref="IsWhitespace(int)">isWhiteSpace(int)</see>
        /// or
        /// <see cref="IsWhitespace(int, bool)">isWhiteSpace(int, boolean)</see>
        /// for a list of whitespace characters.
        /// </remarks>
        /// <param name="buffer">
        /// a
        /// <see cref="ByteBuffer"/>
        /// to which the result of reading will be saved
        /// </param>
        /// <param name="isNullWhitespace">
        /// boolean to indicate whether '0' is whitespace or not.
        /// If in doubt, use true or overloaded method
        /// <see cref="ReadLineSegment(ByteBuffer)">readLineSegment(input)</see>
        /// </param>
        /// <returns>true, if something was read or if the end of the input stream is not reached</returns>
        public virtual bool ReadLineSegment(ByteBuffer buffer, bool isNullWhitespace) {
            int c;
            bool eol = false;
            // ssteward, pdftk-1.10, 040922:
            // skip initial whitespace; added this because PdfReader.rebuildXref()
            // assumes that line provided by readLineSegment does not have init. whitespace;
            while (IsWhitespace((c = Read()), isNullWhitespace)) {
            }
            bool prevWasWhitespace = false;
            while (!eol) {
                switch (c) {
                    case -1:
                    case '\n': {
                        eol = true;
                        break;
                    }

                    case '\r': {
                        eol = true;
                        long cur = GetPosition();
                        if ((Read()) != '\n') {
                            Seek(cur);
                        }
                        break;
                    }

                    case 9:
                    //whitespaces
                    case 12:
                    case 32: {
                        if (prevWasWhitespace) {
                            break;
                        }
                        prevWasWhitespace = true;
                        buffer.Append((byte)c);
                        break;
                    }

                    default: {
                        prevWasWhitespace = false;
                        buffer.Append((byte)c);
                        break;
                    }
                }
                // break loop? do it before we read() again
                if (eol || buffer.Size() == buffer.Capacity()) {
                    eol = true;
                }
                else {
                    c = Read();
                }
            }
            if (buffer.Size() == buffer.Capacity()) {
                eol = false;
                while (!eol) {
                    switch (c = Read()) {
                        case -1:
                        case '\n': {
                            eol = true;
                            break;
                        }

                        case '\r': {
                            eol = true;
                            long cur = GetPosition();
                            if ((Read()) != '\n') {
                                Seek(cur);
                            }
                            break;
                        }
                    }
                }
            }
            return !(c == -1 && buffer.IsEmpty());
        }

        /// <summary>Check whether line starts with object declaration.</summary>
        /// <param name="lineTokenizer">tokenizer, built by single line.</param>
        /// <returns>object number and generation if check is successful, otherwise - null.</returns>
        public static int[] CheckObjectStart(PdfTokenizer lineTokenizer) {
            try {
                lineTokenizer.Seek(0);
                if (!lineTokenizer.NextToken() || lineTokenizer.GetTokenType() != PdfTokenizer.TokenType.Number) {
                    return null;
                }
                int num = lineTokenizer.GetIntValue();
                if (!lineTokenizer.NextToken() || lineTokenizer.GetTokenType() != PdfTokenizer.TokenType.Number) {
                    return null;
                }
                int gen = lineTokenizer.GetIntValue();
                if (!lineTokenizer.NextToken()) {
                    return null;
                }
                if (!JavaUtil.ArraysEquals(Obj, lineTokenizer.GetByteContent())) {
                    return null;
                }
                return new int[] { num, gen };
            }
            catch (Exception) {
            }
            // empty on purpose
            return null;
        }

        void System.IDisposable.Dispose() {
            Close();
        }
    }
}
