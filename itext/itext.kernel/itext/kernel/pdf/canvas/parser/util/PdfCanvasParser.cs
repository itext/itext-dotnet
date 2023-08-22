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
using System.Collections.Generic;
using iText.Commons.Utils;
using iText.IO.Source;
using iText.Kernel.Exceptions;
using iText.Kernel.Pdf;

namespace iText.Kernel.Pdf.Canvas.Parser.Util {
    /// <summary>Parses the page or form XObject content.</summary>
    public class PdfCanvasParser {
        /// <summary>Holds value of property tokeniser.</summary>
        private PdfTokenizer tokeniser;

        private PdfResources currentResources;

        /// <summary>Creates a new instance of PdfContentParser</summary>
        /// <param name="tokeniser">the tokeniser with the content</param>
        public PdfCanvasParser(PdfTokenizer tokeniser) {
            this.tokeniser = tokeniser;
        }

        /// <summary>Creates a new instance of PdfContentParser</summary>
        /// <param name="tokeniser">the tokeniser with the content</param>
        /// <param name="currentResources">
        /// current resources of the content stream.
        /// It is optional parameter, which is used for performance improvements of specific cases of
        /// inline images parsing.
        /// </param>
        public PdfCanvasParser(PdfTokenizer tokeniser, PdfResources currentResources) {
            this.tokeniser = tokeniser;
            this.currentResources = currentResources;
        }

        /// <summary>Parses a single command from the content.</summary>
        /// <remarks>
        /// Parses a single command from the content. Each command is output as an array of arguments
        /// having the command itself as the last element. The returned array will be empty if the
        /// end of content was reached.
        /// <br />
        /// A specific behaviour occurs when inline image is encountered (BI command):
        /// in that case, parser would continue parsing until it meets EI - end of the inline image;
        /// as a result in this case it will return an array with inline image dictionary and image bytes
        /// encapsulated in PdfStream object as first element and EI command as second element.
        /// </remarks>
        /// <param name="ls">
        /// an <c>ArrayList</c> to use. It will be cleared before using. If it's
        /// <c>null</c> will create a new <c>ArrayList</c>
        /// </param>
        /// <returns>the same <c>ArrayList</c> given as argument or a new one</returns>
        public virtual IList<PdfObject> Parse(IList<PdfObject> ls) {
            if (ls == null) {
                ls = new List<PdfObject>();
            }
            else {
                ls.Clear();
            }
            PdfObject ob = null;
            while ((ob = ReadObject()) != null) {
                ls.Add(ob);
                if (tokeniser.GetTokenType() == PdfTokenizer.TokenType.Other) {
                    if ("BI".Equals(ob.ToString())) {
                        PdfStream inlineImageAsStream = InlineImageParsingUtils.Parse(this, currentResources.GetResource(PdfName.ColorSpace
                            ));
                        ls.Clear();
                        ls.Add(inlineImageAsStream);
                        ls.Add(new PdfLiteral("EI"));
                    }
                    break;
                }
            }
            return ls;
        }

        /// <summary>Gets the tokeniser.</summary>
        /// <returns>the tokeniser.</returns>
        public virtual PdfTokenizer GetTokeniser() {
            return this.tokeniser;
        }

        /// <summary>Sets the tokeniser.</summary>
        /// <param name="tokeniser">the tokeniser</param>
        public virtual void SetTokeniser(PdfTokenizer tokeniser) {
            this.tokeniser = tokeniser;
        }

        /// <summary>Reads a dictionary.</summary>
        /// <remarks>Reads a dictionary. The tokeniser must be positioned past the "&lt;&lt;" token.</remarks>
        /// <returns>the dictionary</returns>
        public virtual PdfDictionary ReadDictionary() {
            PdfDictionary dic = new PdfDictionary();
            while (true) {
                if (!NextValidToken()) {
                    throw new PdfException(KernelExceptionMessageConstant.UNEXPECTED_END_OF_FILE);
                }
                if (tokeniser.GetTokenType() == PdfTokenizer.TokenType.EndDic) {
                    break;
                }
                if (tokeniser.GetTokenType() != PdfTokenizer.TokenType.Name) {
                    tokeniser.ThrowError(KernelExceptionMessageConstant.THIS_DICTIONARY_KEY_IS_NOT_A_NAME, tokeniser.GetStringValue
                        ());
                }
                PdfName name = new PdfName(tokeniser.GetStringValue());
                PdfObject obj = ReadObject();
                dic.Put(name, obj);
            }
            return dic;
        }

        /// <summary>Reads an array.</summary>
        /// <remarks>Reads an array. The tokeniser must be positioned past the "[" token.</remarks>
        /// <returns>an array</returns>
        public virtual PdfArray ReadArray() {
            PdfArray array = new PdfArray();
            while (true) {
                PdfObject obj = ReadObject();
                if (!obj.IsArray() && tokeniser.GetTokenType() == PdfTokenizer.TokenType.EndArray) {
                    break;
                }
                if (tokeniser.GetTokenType() == PdfTokenizer.TokenType.EndDic && obj.GetObjectType() != PdfObject.DICTIONARY
                    ) {
                    tokeniser.ThrowError(MessageFormatUtil.Format(KernelExceptionMessageConstant.UNEXPECTED_TOKEN, ">>"));
                }
                array.Add(obj);
            }
            return array;
        }

        /// <summary>Reads a pdf object.</summary>
        /// <returns>the pdf object</returns>
        public virtual PdfObject ReadObject() {
            if (!NextValidToken()) {
                return null;
            }
            PdfTokenizer.TokenType type = tokeniser.GetTokenType();
            switch (type) {
                case PdfTokenizer.TokenType.StartDic: {
                    PdfDictionary dic = ReadDictionary();
                    return dic;
                }

                case PdfTokenizer.TokenType.StartArray: {
                    return ReadArray();
                }

                case PdfTokenizer.TokenType.String: {
                    PdfString str = new PdfString(tokeniser.GetDecodedStringContent()).SetHexWriting(tokeniser.IsHexString());
                    return str;
                }

                case PdfTokenizer.TokenType.Name: {
                    return new PdfName(tokeniser.GetByteContent());
                }

                case PdfTokenizer.TokenType.Number: {
                    //use PdfNumber(byte[]) here, as in this case number parsing won't happen until it's needed.
                    return new PdfNumber(tokeniser.GetByteContent());
                }

                default: {
                    return new PdfLiteral(tokeniser.GetByteContent());
                }
            }
        }

        /// <summary>Reads the next token skipping over the comments.</summary>
        /// <returns><c>true</c> if a token was read, <c>false</c> if the end of content was reached</returns>
        public virtual bool NextValidToken() {
            while (tokeniser.NextToken()) {
                if (tokeniser.GetTokenType() == PdfTokenizer.TokenType.Comment) {
                    continue;
                }
                return true;
            }
            return false;
        }
    }
}
