/*
$Id$

This file is part of the iText (R) project.
Copyright (c) 1998-2016 iText Group NV
Authors: Bruno Lowagie, Paulo Soares, et al.

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
using System.Collections.Generic;
using System.IO;
using System.Text;
using Java.IO;
using iTextSharp.Kernel;
using iTextSharp.Kernel.Pdf;
using iTextSharp.Kernel.Pdf.Canvas.Parser;
using iTextSharp.Kernel.Pdf.Canvas.Parser.Data;
using iTextSharp.Kernel.Pdf.Canvas.Parser.Listener;
using iTextSharp.Kernel.Pdf.Tagging;

namespace iTextSharp.Kernel.Utils
{
	/// <summary>Converts a tagged PDF document into an XML file.</summary>
	public class TaggedPdfReaderTool
	{
		protected internal PdfDocument document;

		protected internal StreamWriter @out;

		protected internal String rootTag;

		protected internal IDictionary<PdfDictionary, IDictionary<int, String>> parsedTags
			 = new Dictionary<PdfDictionary, IDictionary<int, String>>();

		public TaggedPdfReaderTool(PdfDocument document)
		{
			// key - page dictionary; value pairs of mcid and text in them
			this.document = document;
		}

		/// <exception cref="System.IO.IOException"/>
		public virtual void ConvertToXml(Stream os)
		{
			ConvertToXml(os, "UTF-8");
		}

		/// <exception cref="System.IO.IOException"/>
		public virtual void ConvertToXml(Stream os, String charset)
		{
			OutputStreamWriter outs = new OutputStreamWriter(os, charset);
			@out = new StreamWriter(outs);
			if (rootTag != null)
			{
				@out.WriteLine("<" + rootTag + ">");
			}
			// get the StructTreeRoot from the document
			PdfStructTreeRoot structTreeRoot = document.GetStructTreeRoot();
			if (structTreeRoot == null)
			{
				throw new PdfException(PdfException.DocumentDoesntContainStructTreeRoot);
			}
			// Inspect the child or children of the StructTreeRoot
			InspectKids(structTreeRoot.GetKids());
			if (rootTag != null)
			{
				@out.Write("</" + rootTag + ">");
			}
			@out.Flush();
			@out.Close();
		}

		public virtual iTextSharp.Kernel.Utils.TaggedPdfReaderTool SetRootTag(String rootTagName
			)
		{
			this.rootTag = rootTagName;
			return this;
		}

		protected internal virtual void InspectKids(IList<IPdfStructElem> kids)
		{
			if (kids == null)
			{
				return;
			}
			foreach (IPdfStructElem kid in kids)
			{
				InspectKid(kid);
			}
		}

		protected internal virtual void InspectKid(IPdfStructElem kid)
		{
			if (kid is PdfStructElem)
			{
				PdfStructElem structElemKid = (PdfStructElem)kid;
				PdfName s = structElemKid.GetRole();
				String tagN = s.GetValue();
				String tag = FixTagName(tagN);
				@out.Write("<");
				@out.Write(tag);
				InspectAttributes(structElemKid);
				@out.WriteLine(">");
				PdfString alt = (structElemKid).GetAlt();
				if (alt != null)
				{
					@out.Write("<alt><![CDATA[");
					@out.Write(iTextSharp.IO.Util.StringUtil.ReplaceAll(alt.GetValue(), "[\\000]*", ""
						));
					@out.WriteLine("]]></alt>");
				}
				InspectKids(structElemKid.GetKids());
				@out.Write("</");
				@out.Write(tag);
				@out.WriteLine(">");
			}
			else
			{
				if (kid is PdfMcr)
				{
					ParseTag((PdfMcr)kid);
				}
				else
				{
					@out.Write(" <flushedKid/> ");
				}
			}
		}

		protected internal virtual void InspectAttributes(PdfStructElem kid)
		{
			PdfObject attrObj = kid.GetAttributes(false);
			if (attrObj != null)
			{
				PdfDictionary attrDict;
				if (attrObj is PdfArray)
				{
					attrDict = ((PdfArray)attrObj).GetAsDictionary(0);
				}
				else
				{
					attrDict = (PdfDictionary)attrObj;
				}
				foreach (KeyValuePair<PdfName, PdfObject> entry in attrDict.EntrySet())
				{
					@out.Write(' ');
					String attrName = entry.Key.GetValue();
					@out.Write(char.ToLowerCase(attrName[0]) + attrName.Substring(1));
					@out.Write("=\"");
					@out.Write(entry.Value.ToString());
					@out.Write("\"");
				}
			}
		}

		protected internal virtual void ParseTag(PdfMcr kid)
		{
			int mcid = kid.GetMcid();
			PdfDictionary pageDic = kid.GetPageObject();
			String tagContent = "";
			if (mcid != -1)
			{
				if (!parsedTags.ContainsKey(pageDic))
				{
					TaggedPdfReaderTool.MarkedContentEventListener listener = new TaggedPdfReaderTool.MarkedContentEventListener
						(this);
					PdfCanvasProcessor processor = new PdfCanvasProcessor(listener);
					PdfPage page = document.GetPage(pageDic);
					processor.ProcessContent(page.GetContentBytes(), page.GetResources());
					parsedTags[pageDic] = listener.GetMcidContent();
				}
				if (parsedTags[pageDic].ContainsKey(mcid))
				{
					tagContent = parsedTags[pageDic][mcid];
				}
			}
			else
			{
				PdfObjRef objRef = (PdfObjRef)kid;
				PdfObject @object = objRef.GetReferencedObject();
				if (@object.IsDictionary())
				{
					PdfName subtype = ((PdfDictionary)@object).GetAsName(PdfName.Subtype);
					tagContent = subtype.ToString();
				}
			}
			@out.Write(EscapeXML(tagContent, true));
		}

		protected internal static String FixTagName(String tag)
		{
			StringBuilder sb = new StringBuilder();
			for (int k = 0; k < tag.Length; ++k)
			{
				char c = tag[k];
				bool nameStart = c == ':' || (c >= 'A' && c <= 'Z') || c == '_' || (c >= 'a' && c
					 <= 'z') || (c >= '\u00c0' && c <= '\u00d6') || (c >= '\u00d8' && c <= '\u00f6')
					 || (c >= '\u00f8' && c <= '\u02ff') || (c >= '\u0370' && c <= '\u037d') || (c >=
					 '\u037f' && c <= '\u1fff') || (c >= '\u200c' && c <= '\u200d') || (c >= '\u2070'
					 && c <= '\u218f') || (c >= '\u2c00' && c <= '\u2fef') || (c >= '\u3001' && c <=
					 '\ud7ff') || (c >= '\uf900' && c <= '\ufdcf') || (c >= '\ufdf0' && c <= '\ufffd'
					);
				bool nameMiddle = c == '-' || c == '.' || (c >= '0' && c <= '9') || c == '\u00b7'
					 || (c >= '\u0300' && c <= '\u036f') || (c >= '\u203f' && c <= '\u2040') || nameStart;
				if (k == 0)
				{
					if (!nameStart)
					{
						c = '_';
					}
				}
				else
				{
					if (!nameMiddle)
					{
						c = '-';
					}
				}
				sb.Append(c);
			}
			return sb.ToString();
		}

		/// <summary>
		/// NOTE: copied from itext5 XMLUtils class
		/// <p>
		/// Escapes a string with the appropriated XML codes.
		/// </summary>
		/// <param name="s">the string to be escaped</param>
		/// <param name="onlyASCII">codes above 127 will always be escaped with &amp;#nn; if <CODE>true</CODE>
		/// 	</param>
		/// <returns>the escaped string</returns>
		/// <since>5.0.6</since>
		protected internal static String EscapeXML(String s, bool onlyASCII)
		{
			char[] cc = s.ToCharArray();
			int len = cc.Length;
			StringBuilder sb = new StringBuilder();
			for (int k = 0; k < len; ++k)
			{
				int c = cc[k];
				switch (c)
				{
					case '<':
					{
						sb.Append("&lt;");
						break;
					}

					case '>':
					{
						sb.Append("&gt;");
						break;
					}

					case '&':
					{
						sb.Append("&amp;");
						break;
					}

					case '"':
					{
						sb.Append("&quot;");
						break;
					}

					case '\'':
					{
						sb.Append("&apos;");
						break;
					}

					default:
					{
						if (IsValidCharacterValue(c))
						{
							if (onlyASCII && c > 127)
							{
								sb.Append("&#").Append(c).Append(';');
							}
							else
							{
								sb.Append((char)c);
							}
						}
						break;
					}
				}
			}
			return sb.ToString();
		}

		/// <summary>Checks if a character value should be escaped/unescaped.</summary>
		/// <param name="c">a character value</param>
		/// <returns>true if it's OK to escape or unescape this value</returns>
		public static bool IsValidCharacterValue(int c)
		{
			return (c == 0x9 || c == 0xA || c == 0xD || c >= 0x20 && c <= 0xD7FF || c >= 0xE000
				 && c <= 0xFFFD || c >= 0x10000 && c <= 0x10FFFF);
		}

		private class MarkedContentEventListener : IEventListener
		{
			private IDictionary<int, ITextExtractionStrategy> contentByMcid = new Dictionary<
				int, ITextExtractionStrategy>();

			public virtual IDictionary<int, String> GetMcidContent()
			{
				IDictionary<int, String> content = new Dictionary<int, String>();
				foreach (int id in this.contentByMcid.Keys)
				{
					content[id] = this.contentByMcid[id].GetResultantText();
				}
				return content;
			}

			public virtual void EventOccurred(IEventData data, EventType type)
			{
				switch (type)
				{
					case EventType.RENDER_TEXT:
					{
						TextRenderInfo textInfo = (TextRenderInfo)data;
						int mcid = textInfo.GetMcid();
						if (mcid != -1)
						{
							ITextExtractionStrategy textExtractionStrategy = this.contentByMcid[mcid];
							if (textExtractionStrategy == null)
							{
								textExtractionStrategy = new LocationTextExtractionStrategy();
								this.contentByMcid[mcid] = textExtractionStrategy;
							}
							textExtractionStrategy.EventOccurred(data, type);
						}
						break;
					}

					default:
					{
						break;
					}
				}
			}

			public virtual ICollection<EventType> GetSupportedEvents()
			{
				return null;
			}

			internal MarkedContentEventListener(TaggedPdfReaderTool _enclosing)
			{
				this._enclosing = _enclosing;
			}

			private readonly TaggedPdfReaderTool _enclosing;
		}
	}
}
