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
using System.Collections.Generic;
using System.Text;
using iText.Commons.Utils;
using iText.IO.Font;
using iText.IO.Source;

namespace iText.IO.Font.Cmap {
    public class CMapContentParser {
        /// <summary>Commands have this type.</summary>
        public const int COMMAND_TYPE = 200;

        /// <summary>Holds value of property tokeniser.</summary>
        private PdfTokenizer tokeniser;

        /// <summary>Creates a new instance of PdfContentParser</summary>
        /// <param name="tokeniser">the tokeniser with the content</param>
        public CMapContentParser(PdfTokenizer tokeniser) {
            this.tokeniser = tokeniser;
        }

        /// <summary>Parses a single command from the content.</summary>
        /// <remarks>
        /// Parses a single command from the content. Each command is output as an array of arguments
        /// having the command itself as the last element. The returned array will be empty if the
        /// end of content was reached.
        /// </remarks>
        /// <param name="ls">
        /// an
        /// <c>ArrayList</c>
        /// to use. It will be cleared before using.
        /// </param>
        public virtual void Parse(IList<CMapObject> ls) {
            ls.Clear();
            CMapObject ob;
            while ((ob = ReadObject()) != null) {
                ls.Add(ob);
                // TokenType.Other or CMapObject.Literal means a command
                if (ob.IsLiteral()) {
                    break;
                }
            }
        }

        /// <summary>Reads a dictionary.</summary>
        /// <remarks>
        /// Reads a dictionary. The tokeniser must be positioned past the
        /// <c>"&lt;&lt;"</c>
        /// token.
        /// </remarks>
        /// <returns>the dictionary</returns>
        public virtual CMapObject ReadDictionary() {
            IDictionary<String, CMapObject> dic = new Dictionary<String, CMapObject>();
            while (true) {
                if (!NextValidToken()) {
                    throw new iText.IO.Exceptions.IOException("Unexpected end of file.");
                }
                if (tokeniser.GetTokenType() == PdfTokenizer.TokenType.EndDic) {
                    break;
                }
                if (tokeniser.GetTokenType() == PdfTokenizer.TokenType.Other && "def".Equals(tokeniser.GetStringValue())) {
                    continue;
                }
                if (tokeniser.GetTokenType() != PdfTokenizer.TokenType.Name) {
                    throw new iText.IO.Exceptions.IOException("Dictionary key {0} is not a name.").SetMessageParams(tokeniser.
                        GetStringValue());
                }
                String name = tokeniser.GetStringValue();
                CMapObject obj = ReadObject();
                if (obj.IsToken()) {
                    if (obj.ToString().Equals(">>")) {
                        tokeniser.ThrowError(iText.IO.Exceptions.IOException.UnexpectedGtGt);
                    }
                    if (obj.ToString().Equals("]")) {
                        tokeniser.ThrowError(iText.IO.Exceptions.IOException.UnexpectedCloseBracket);
                    }
                }
                dic.Put(name, obj);
            }
            return new CMapObject(CMapObject.DICTIONARY, dic);
        }

        /// <summary>Reads an array.</summary>
        /// <remarks>Reads an array. The tokeniser must be positioned past the "[" token.</remarks>
        /// <returns>an array</returns>
        public virtual CMapObject ReadArray() {
            IList<CMapObject> array = new List<CMapObject>();
            while (true) {
                CMapObject obj = ReadObject();
                if (obj.IsToken()) {
                    if (obj.ToString().Equals("]")) {
                        break;
                    }
                    if (obj.ToString().Equals(">>")) {
                        tokeniser.ThrowError(iText.IO.Exceptions.IOException.UnexpectedGtGt);
                    }
                }
                array.Add(obj);
            }
            return new CMapObject(CMapObject.ARRAY, array);
        }

        /// <summary>Reads a pdf object.</summary>
        /// <returns>the pdf object</returns>
        public virtual CMapObject ReadObject() {
            if (!NextValidToken()) {
                return null;
            }
            PdfTokenizer.TokenType type = tokeniser.GetTokenType();
            switch (type) {
                case PdfTokenizer.TokenType.StartDic: {
                    return ReadDictionary();
                }

                case PdfTokenizer.TokenType.StartArray: {
                    return ReadArray();
                }

                case PdfTokenizer.TokenType.String: {
                    CMapObject obj;
                    if (tokeniser.IsHexString()) {
                        obj = new CMapObject(CMapObject.HEX_STRING, PdfTokenizer.DecodeStringContent(tokeniser.GetByteContent(), true
                            ));
                    }
                    else {
                        obj = new CMapObject(CMapObject.STRING, PdfTokenizer.DecodeStringContent(tokeniser.GetByteContent(), false
                            ));
                    }
                    return obj;
                }

                case PdfTokenizer.TokenType.Name: {
                    return new CMapObject(CMapObject.NAME, DecodeName(tokeniser.GetByteContent()));
                }

                case PdfTokenizer.TokenType.Number: {
                    CMapObject numObject = new CMapObject(CMapObject.NUMBER, null);
                    try {
                        numObject.SetValue((int)Double.Parse(tokeniser.GetStringValue(), System.Globalization.CultureInfo.InvariantCulture
                            ));
                    }
                    catch (FormatException) {
                        numObject.SetValue(int.MinValue);
                    }
                    return numObject;
                }

                case PdfTokenizer.TokenType.Other: {
                    return new CMapObject(CMapObject.LITERAL, tokeniser.GetStringValue());
                }

                case PdfTokenizer.TokenType.EndArray: {
                    return new CMapObject(CMapObject.TOKEN, "]");
                }

                case PdfTokenizer.TokenType.EndDic: {
                    return new CMapObject(CMapObject.TOKEN, ">>");
                }

                default: {
                    return new CMapObject(0, "");
                }
            }
        }

        /// <summary>Reads the next token skipping over the comments.</summary>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if a token was read,
        /// <see langword="false"/>
        /// if the end of content was reached.
        /// </returns>
        public virtual bool NextValidToken() {
            while (tokeniser.NextToken()) {
                if (tokeniser.GetTokenType() == PdfTokenizer.TokenType.Comment) {
                    continue;
                }
                return true;
            }
            return false;
        }

        // TODO: Duplicates PdfName.generateValue (REFACTOR)
        protected internal static String DecodeName(byte[] content) {
            StringBuilder buf = new StringBuilder();
            try {
                for (int k = 0; k < content.Length; ++k) {
                    char c = (char)content[k];
                    if (c == '#') {
                        byte c1 = content[k + 1];
                        byte c2 = content[k + 2];
                        c = (char)((ByteBuffer.GetHex(c1) << 4) + ByteBuffer.GetHex(c2));
                        k += 2;
                    }
                    buf.Append(c);
                }
            }
            catch (IndexOutOfRangeException) {
            }
            // empty on purpose
            return buf.ToString();
        }

        private static String ToHex4(int n) {
            String s = "0000" + JavaUtil.IntegerToHexString(n);
            return s.Substring(s.Length - 4);
        }

        /// <summary>Gets an hex string in the format "&lt;HHHH&gt;".</summary>
        /// <param name="n">the number</param>
        /// <returns>the hex string</returns>
        public static String ToHex(int n) {
            if (n < 0x10000) {
                return "<" + ToHex4(n) + ">";
            }
            n -= 0x10000;
            int high = n / 0x400 + 0xd800;
            int low = n % 0x400 + 0xdc00;
            return "[<" + ToHex4(high) + ToHex4(low) + ">]";
        }

        public static String DecodeCMapObject(CMapObject cMapObject) {
            if (cMapObject.IsHexString()) {
                return PdfEncodings.ConvertToString(((String)cMapObject.GetValue()).GetBytes(), PdfEncodings.UNICODE_BIG_UNMARKED
                    );
            }
            else {
                return (String)cMapObject.GetValue();
            }
        }
    }
}
