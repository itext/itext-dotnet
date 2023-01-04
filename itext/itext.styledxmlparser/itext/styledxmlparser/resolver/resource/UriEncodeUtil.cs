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
address: sales@itextpdf.com */
using System;
using System.Collections;
using System.IO;
using System.Text;
using iText.Commons.Utils;
using iText.StyledXmlParser.Exceptions;

namespace iText.StyledXmlParser.Resolver.Resource
{
    internal class UriEncodeUtil
    {

        /// <summary>
        /// Set of 256 characters with the bits that don't need encoding set to on.
        /// </summary>
        internal static BitArray unreservedAndReserved;


        /// <summary>
        /// The difference between the value a character in lower cases and the upper case character value.
        /// </summary>
        internal const int caseDiff = ('a' - 'A');

        /// <summary>
        /// The default encoding ("UTF-8").
        /// </summary>
        internal static String dfltEncName = "UTF-8";

        static UriEncodeUtil()
        {
            unreservedAndReserved = new BitArray(256);
            int i;
            for (i = 'a'; i <= 'z'; i++)
            {
                unreservedAndReserved.Set(i, true);
            }
            for (i = 'A'; i <= 'Z'; i++)
            {
                unreservedAndReserved.Set(i, true);
            }
            for (i = '0'; i <= '9'; i++)
            {
                unreservedAndReserved.Set(i, true);
            }
            unreservedAndReserved.Set('-', true);
            unreservedAndReserved.Set('_', true);
            unreservedAndReserved.Set('.', true);
            unreservedAndReserved.Set('~', true);

            unreservedAndReserved.Set(':', true);
            unreservedAndReserved.Set('/', true);
            unreservedAndReserved.Set('?', true);
            unreservedAndReserved.Set('#', true);
            unreservedAndReserved.Set('[', true);
            unreservedAndReserved.Set(']', true);
            unreservedAndReserved.Set('@', true);
            unreservedAndReserved.Set('!', true);
            unreservedAndReserved.Set('$', true);
            unreservedAndReserved.Set('&', true);
            unreservedAndReserved.Set('\'', true);
            unreservedAndReserved.Set('\\', true);
            unreservedAndReserved.Set('(', true);
            unreservedAndReserved.Set(')', true);
            unreservedAndReserved.Set('*', true);
            unreservedAndReserved.Set('+', true);
            unreservedAndReserved.Set(',', true);
            unreservedAndReserved.Set(';', true);
            unreservedAndReserved.Set('=', true);

        }

        /// <summary>
        /// Encodes a <see cref="String"/> in the default encoding and default uri scheme to an HTML-encoded <see cref="String"/>.
        /// </summary>
        /// <param name="s">the original string</param>
        /// <returns>the encoded string</returns>
        public static String Encode(String s)
        {
            return Encode(s, dfltEncName);
        }

        /// <summary>
        /// Encodes a <see cref="String"/> in a specific encoding and specific uri scheme to an HTML-encoded <see cref="String"/>.
        /// </summary>
        /// <param name="s">the original string</param>
        /// <param name="enc">the encoding</param>
        /// <returns>the encoded string</returns>
        public static String Encode(String s, String enc)
        {
            bool needToChange = false;
            StringBuilder @out = new StringBuilder(s.Length);
            Encoding charset;
            BinaryWriter charArrayWriter = new BinaryWriter(new MemoryStream());
            if (enc == null)
            {
                throw new StyledXMLParserException(StyledXMLParserException.UnsupportedEncodingException);
            }
            charset = EncodingUtil.GetEncoding(enc);
            int i = 0;
            bool firstHash = true;
            int strLength = s.Length;
            while (i < strLength)
            {
                int c = (int)s[i];
                if ('\\' == c)
                {
                    @out.Append('/');
                    needToChange = true;
                    i++;
                }
                else if ('%' == c)
                {
                    int v = -1;
                    if (i + 2 < s.Length)
                    {
                        try
                        {
                            v = System.Convert.ToInt32(s.Substring(i + 1, 2), 16);
                        }
                        catch (FormatException)
                        {
                            v = -1;
                        }
                        if (v >= 0)
                            @out.Append((char)c);
                    }
                    if (v < 0)
                    {
                        // here we assume percent sign to be used not for encoding of other characters, i.e. not for its reserved purpose
                        // which means percent sign should be encoded itself. %25 code stands for percent sign.
                        needToChange = true;
                        @out.Append("%25");
                    }
                    i++;
                }
                else if ('#' == c)
                {
                    // we want only the first hash to be left without percent encoding because C# encodes this way
                    if (firstHash)
                    {
                        @out.Append((char)c);
                        firstHash = false;
                    }
                    else
                    {
                        @out.Append("%23");
                        needToChange = true;
                    }
                    i++;

                }
                else if (c < unreservedAndReserved.Length && unreservedAndReserved.Get(c))
                {
                    @out.Append((char)c);
                    i++;
                }
                else
                {
                    int numOfChars = 0;
                    charArrayWriter.BaseStream.Position = 0;
                    do
                    {
                        // convert to external encoding before hex conversion
                        charArrayWriter.Write((char)c);
                        numOfChars++;
                        /*
                        * If this character represents the start of a Unicode
                        * surrogate pair, then pass in two characters. It's not
                        * clear what should be done if a bytes reserved in the
                        * surrogate pairs range occurs outside of a legal
                        * surrogate pair. For now, just treat it as if it were
                        * any other character.
                        */
                        if (c >= 0xD800 && c <= 0xDBFF)
                        {
                            /*
                            System.out.println(Integer.toHexString(c)
                            + " is high surrogate");
                            */
                            if ((i + 1) < s.Length)
                            {
                                int d = (int)s[i + 1];
                                /*
                                System.out.println("\tExamining "
                                + Integer.toHexString(d));
                                */
                                if (d >= 0xDC00 && d <= 0xDFFF)
                                {
                                    /*
                                    System.out.println("\t"
                                    + Integer.toHexString(d)
                                    + " is low surrogate");
                                    */
                                    charArrayWriter.Write(d);
                                    i++;
                                    numOfChars++;
                                }
                            }
                        }
                        i++;
                    }
                    while (i < strLength && ((c = (int)s[i]) >= unreservedAndReserved.Length || !unreservedAndReserved.Get(c)));
                    BinaryReader binReader = new BinaryReader(charArrayWriter.BaseStream);
                    binReader.BaseStream.Position = 0;
                    char[] chars = binReader.ReadChars(numOfChars);
                    String str = new String(chars);
                    byte[] ba = str.GetBytes(charset);
                    for (int j = 0; j < ba.Length; j++)
                    {
                        @out.Append('%');
                        char ch = CharacterForDigit((ba[j] >> 4) & 0xF, 16);
                        // converting to use uppercase letter as part of
                        // the hex value if ch is a letter.
                        if (char.IsLetter(ch))
                        {
                            ch -= (char)caseDiff;
                        }
                        @out.Append(ch);
                        ch = CharacterForDigit(ba[j] & 0xF, 16);
                        if (char.IsLetter(ch))
                        {
                            ch -= (char)caseDiff;
                        }
                        @out.Append(ch);
                    }
                    charArrayWriter.Flush();
                    needToChange = true;
                }
            }

            return (needToChange ? @out.ToString() : s);
        }

        private static char CharacterForDigit(int digit, int radix)
        {
            if ((digit >= radix) || (digit < 0))
            {
                return '\0';
            }
            if ((radix < 2) || (radix > 36))
            {
                return '\0';
            }
            if (digit < 10)
            {
                return (char)('0' + digit);
            }
            return (char)('a' - 10 + digit);
        }
    }
}
