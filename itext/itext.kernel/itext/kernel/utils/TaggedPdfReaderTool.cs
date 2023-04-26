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
using System.IO;
using System.Text;
using iText.Commons.Utils;
using iText.IO.Exceptions;
using iText.Kernel.Exceptions;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Data;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using iText.Kernel.Pdf.Tagging;

namespace iText.Kernel.Utils {
    /// <summary>Converts a tagged PDF document into an XML file.</summary>
    public class TaggedPdfReaderTool {
        protected internal PdfDocument document;

        protected internal StreamWriter @out;

        protected internal String rootTag;

        // key - page dictionary; value - a mapping of mcids to text in them
        protected internal IDictionary<PdfDictionary, IDictionary<int, String>> parsedTags = new Dictionary<PdfDictionary
            , IDictionary<int, String>>();

        /// <summary>
        /// Constructs a
        /// <see cref="TaggedPdfReaderTool"/>
        /// via a given
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>.
        /// </summary>
        /// <param name="document">the document to read tag structure from</param>
        public TaggedPdfReaderTool(PdfDocument document) {
            this.document = document;
        }

        /// <summary>Checks if a character value should be escaped/unescaped.</summary>
        /// <param name="c">a character value</param>
        /// <returns>true if it's OK to escape or unescape this value</returns>
        public static bool IsValidCharacterValue(int c) {
            return (c == 0x9 || c == 0xA || c == 0xD || c >= 0x20 && c <= 0xD7FF || c >= 0xE000 && c <= 0xFFFD || c >=
                 0x10000 && c <= 0x10FFFF);
        }

        /// <summary>Converts the current tag structure into an XML file with default encoding (UTF-8).</summary>
        /// <param name="os">the output stream to save XML file to</param>
        public virtual void ConvertToXml(Stream os) {
            ConvertToXml(os, "UTF-8");
        }

        /// <summary>Converts the current tag structure into an XML file with provided encoding.</summary>
        /// <param name="os">the output stream to save XML file to</param>
        /// <param name="charset">the charset of the resultant XML file</param>
        public virtual void ConvertToXml(Stream os, String charset) {
            @out = new StreamWriter(os, EncodingUtil.GetEncoding(charset));
            if (rootTag != null) {
                @out.Write("<" + rootTag + ">" + Environment.NewLine);
            }
            // get the StructTreeRoot from the document
            PdfStructTreeRoot structTreeRoot = document.GetStructTreeRoot();
            if (structTreeRoot == null) {
                throw new PdfException(KernelExceptionMessageConstant.DOCUMENT_DOES_NOT_CONTAIN_STRUCT_TREE_ROOT);
            }
            // Inspect the child or children of the StructTreeRoot
            InspectKids(structTreeRoot.GetKids());
            if (rootTag != null) {
                @out.Write("</" + rootTag + ">");
            }
            @out.Flush();
            @out.Dispose();
        }

        /// <summary>Sets the name of the root tag of the resultant XML file</summary>
        /// <param name="rootTagName">the name of the root tag</param>
        /// <returns>this object</returns>
        public virtual iText.Kernel.Utils.TaggedPdfReaderTool SetRootTag(String rootTagName) {
            this.rootTag = rootTagName;
            return this;
        }

        protected internal virtual void InspectKids(IList<IStructureNode> kids) {
            if (kids == null) {
                return;
            }
            foreach (IStructureNode kid in kids) {
                InspectKid(kid);
            }
        }

        protected internal virtual void InspectKid(IStructureNode kid) {
            try {
                if (kid is PdfStructElem) {
                    PdfStructElem structElemKid = (PdfStructElem)kid;
                    PdfName s = structElemKid.GetRole();
                    String tagN = s.GetValue();
                    String tag = FixTagName(tagN);
                    @out.Write("<");
                    @out.Write(tag);
                    InspectAttributes(structElemKid);
                    @out.Write(">" + Environment.NewLine);
                    PdfString alt = (structElemKid).GetAlt();
                    if (alt != null) {
                        @out.Write("<alt><![CDATA[");
                        @out.Write(iText.Commons.Utils.StringUtil.ReplaceAll(alt.GetValue(), "[\\000]*", ""));
                        @out.Write("]]></alt>" + Environment.NewLine);
                    }
                    InspectKids(structElemKid.GetKids());
                    @out.Write("</");
                    @out.Write(tag);
                    @out.Write(">" + Environment.NewLine);
                }
                else {
                    if (kid is PdfMcr) {
                        ParseTag((PdfMcr)kid);
                    }
                    else {
                        @out.Write(" <flushedKid/> ");
                    }
                }
            }
            catch (System.IO.IOException e) {
                throw new iText.IO.Exceptions.IOException(IoExceptionMessageConstant.UNKNOWN_IO_EXCEPTION, e);
            }
        }

        protected internal virtual void InspectAttributes(PdfStructElem kid) {
            PdfObject attrObj = kid.GetAttributes(false);
            if (attrObj != null) {
                PdfDictionary attrDict;
                if (attrObj is PdfArray) {
                    attrDict = ((PdfArray)attrObj).GetAsDictionary(0);
                }
                else {
                    attrDict = (PdfDictionary)attrObj;
                }
                try {
                    foreach (PdfName key in attrDict.KeySet()) {
                        @out.Write(' ');
                        String attrName = key.GetValue();
                        @out.Write(char.ToLower(attrName[0]) + attrName.Substring(1));
                        @out.Write("=\"");
                        @out.Write(attrDict.Get(key, false).ToString());
                        @out.Write("\"");
                    }
                }
                catch (System.IO.IOException e) {
                    throw new iText.IO.Exceptions.IOException(IoExceptionMessageConstant.UNKNOWN_IO_EXCEPTION, e);
                }
            }
        }

        protected internal virtual void ParseTag(PdfMcr kid) {
            int mcid = kid.GetMcid();
            PdfDictionary pageDic = kid.GetPageObject();
            String tagContent = "";
            if (mcid != -1) {
                if (!parsedTags.ContainsKey(pageDic)) {
                    TaggedPdfReaderTool.MarkedContentEventListener listener = new TaggedPdfReaderTool.MarkedContentEventListener
                        (this);
                    PdfCanvasProcessor processor = new PdfCanvasProcessor(listener);
                    PdfPage page = document.GetPage(pageDic);
                    processor.ProcessContent(page.GetContentBytes(), page.GetResources());
                    parsedTags.Put(pageDic, listener.GetMcidContent());
                }
                if (parsedTags.Get(pageDic).ContainsKey(mcid)) {
                    tagContent = parsedTags.Get(pageDic).Get(mcid);
                }
            }
            else {
                PdfObjRef objRef = (PdfObjRef)kid;
                PdfObject @object = objRef.GetReferencedObject();
                if (@object.IsDictionary()) {
                    PdfName subtype = ((PdfDictionary)@object).GetAsName(PdfName.Subtype);
                    tagContent = subtype.ToString();
                }
            }
            try {
                @out.Write(EscapeXML(tagContent, true));
            }
            catch (System.IO.IOException e) {
                throw new iText.IO.Exceptions.IOException(IoExceptionMessageConstant.UNKNOWN_IO_EXCEPTION, e);
            }
        }

        protected internal static String FixTagName(String tag) {
            StringBuilder sb = new StringBuilder();
            for (int k = 0; k < tag.Length; ++k) {
                char c = tag[k];
                bool nameStart = c == ':' || (c >= 'A' && c <= 'Z') || c == '_' || (c >= 'a' && c <= 'z') || (c >= '\u00c0'
                     && c <= '\u00d6') || (c >= '\u00d8' && c <= '\u00f6') || (c >= '\u00f8' && c <= '\u02ff') || (c >= '\u0370'
                     && c <= '\u037d') || (c >= '\u037f' && c <= '\u1fff') || (c >= '\u200c' && c <= '\u200d') || (c >= '\u2070'
                     && c <= '\u218f') || (c >= '\u2c00' && c <= '\u2fef') || (c >= '\u3001' && c <= '\ud7ff') || (c >= '\uf900'
                     && c <= '\ufdcf') || (c >= '\ufdf0' && c <= '\ufffd');
                bool nameMiddle = c == '-' || c == '.' || (c >= '0' && c <= '9') || c == '\u00b7' || (c >= '\u0300' && c <=
                     '\u036f') || (c >= '\u203f' && c <= '\u2040') || nameStart;
                if (k == 0) {
                    if (!nameStart) {
                        c = '_';
                    }
                }
                else {
                    if (!nameMiddle) {
                        c = '-';
                    }
                }
                sb.Append(c);
            }
            return sb.ToString();
        }

        /// <summary>
        /// NOTE: copied from itext5 XMLUtils class
        /// Escapes a string with the appropriated XML codes.
        /// </summary>
        /// <param name="s">the string to be escaped</param>
        /// <param name="onlyASCII">codes above 127 will always be escaped with &amp;#nn; if <c>true</c></param>
        /// <returns>the escaped string</returns>
        protected internal static String EscapeXML(String s, bool onlyASCII) {
            char[] cc = s.ToCharArray();
            int len = cc.Length;
            StringBuilder sb = new StringBuilder();
            for (int k = 0; k < len; ++k) {
                int c = cc[k];
                switch (c) {
                    case '<': {
                        sb.Append("&lt;");
                        break;
                    }

                    case '>': {
                        sb.Append("&gt;");
                        break;
                    }

                    case '&': {
                        sb.Append("&amp;");
                        break;
                    }

                    case '"': {
                        sb.Append("&quot;");
                        break;
                    }

                    case '\'': {
                        sb.Append("&apos;");
                        break;
                    }

                    default: {
                        if (IsValidCharacterValue(c)) {
                            if (onlyASCII && c > 127) {
                                sb.Append("&#").Append(c).Append(';');
                            }
                            else {
                                sb.Append((char)c);
                            }
                        }
                        break;
                    }
                }
            }
            return sb.ToString();
        }

        private class MarkedContentEventListener : IEventListener {
            private IDictionary<int, ITextExtractionStrategy> contentByMcid = new Dictionary<int, ITextExtractionStrategy
                >();

            public virtual IDictionary<int, String> GetMcidContent() {
                IDictionary<int, String> content = new Dictionary<int, String>();
                foreach (int id in this.contentByMcid.Keys) {
                    content.Put(id, this.contentByMcid.Get(id).GetResultantText());
                }
                return content;
            }

            public virtual void EventOccurred(IEventData data, EventType type) {
                switch (type) {
                    case EventType.RENDER_TEXT: {
                        TextRenderInfo textInfo = (TextRenderInfo)data;
                        int mcid = textInfo.GetMcid();
                        if (mcid != -1) {
                            ITextExtractionStrategy textExtractionStrategy = this.contentByMcid.Get(mcid);
                            if (textExtractionStrategy == null) {
                                textExtractionStrategy = new LocationTextExtractionStrategy();
                                this.contentByMcid.Put(mcid, textExtractionStrategy);
                            }
                            textExtractionStrategy.EventOccurred(data, type);
                        }
                        break;
                    }

                    default: {
                        break;
                    }
                }
            }

            public virtual ICollection<EventType> GetSupportedEvents() {
                return null;
            }

            internal MarkedContentEventListener(TaggedPdfReaderTool _enclosing) {
                this._enclosing = _enclosing;
            }

            private readonly TaggedPdfReaderTool _enclosing;
        }
    }
}
