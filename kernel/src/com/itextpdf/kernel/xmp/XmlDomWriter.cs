/*
$Id: 097949a2c1adaa2ddf070f1dcc19535d2f80107b $

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
using System.IO;
using java.io;
using org.w3c.dom;

namespace com.itextpdf.kernel.xmp
{
	/// <summary>This class writes the DOM structure of the XML to the specified output.</summary>
	public class XmlDomWriter
	{
		/// <summary>Print writer.</summary>
		protected internal StreamWriter fOut;

		/// <summary>Canonical output.</summary>
		protected internal bool fCanonical;

		/// <summary>Processing XML 1.1 document.</summary>
		protected internal bool fXML11;

		/// <summary>Default constructor.</summary>
		public XmlDomWriter()
		{
		}

		/// <summary>Creates an XmlDomWriter.</summary>
		/// <param name="canonical">should the writer write canonical output or not</param>
		public XmlDomWriter(bool canonical)
		{
			//
			// Constructors
			//
			// <init>()
			fCanonical = canonical;
		}

		// <init>(boolean)
		//
		// Public methods
		//
		/// <summary>Sets whether output is canonical.</summary>
		public virtual void SetCanonical(bool canonical)
		{
			fCanonical = canonical;
		}

		// setCanonical(boolean)
		/// <summary>Sets the output stream for printing.</summary>
		/// <exception cref="System.ArgumentException"/>
		public virtual void SetOutput(Stream stream, String encoding)
		{
			if (encoding == null)
			{
				encoding = "UTF8";
			}
			TextWriter writer = new OutputStreamWriter(stream, encoding);
			fOut = new StreamWriter(writer);
		}

		// setOutput(OutputStream,String)
		/// <summary>Sets the output writer.</summary>
		public virtual void SetOutput(TextWriter writer)
		{
			fOut = writer is StreamWriter ? (StreamWriter)writer : new StreamWriter(writer);
		}

		// setOutput(java.io.Writer)
		/// <summary>Writes the specified node, recursively.</summary>
		public virtual void Write(Node node)
		{
			// is there anything to do?
			if (node == null)
			{
				return;
			}
			short type = node.GetNodeType();
			switch (type)
			{
				case Node.DOCUMENT_NODE:
				{
					Document document = (Document)node;
					fXML11 = false;
					//"1.1".equals(getVersion(document));
					if (!fCanonical)
					{
						if (fXML11)
						{
							fOut.Write("<?xml version=\"1.1\" encoding=\"UTF-8\"?>");
						}
						else
						{
							fOut.Write("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
						}
						fOut.Write("\n");
						fOut.Flush();
						Write(document.GetDoctype());
					}
					Write(document.GetDocumentElement());
					break;
				}

				case Node.DOCUMENT_TYPE_NODE:
				{
					DocumentType doctype = (DocumentType)node;
					fOut.Write("<!DOCTYPE ");
					fOut.Write(doctype.GetName());
					String publicId = doctype.GetPublicId();
					String systemId = doctype.GetSystemId();
					if (publicId != null)
					{
						fOut.Write(" PUBLIC '");
						fOut.Write(publicId);
						fOut.Write("' '");
						fOut.Write(systemId);
						fOut.Write('\'');
					}
					else
					{
						if (systemId != null)
						{
							fOut.Write(" SYSTEM '");
							fOut.Write(systemId);
							fOut.Write('\'');
						}
					}
					String internalSubset = doctype.GetInternalSubset();
					if (internalSubset != null)
					{
						fOut.WriteLine(" [");
						fOut.Write(internalSubset);
						fOut.Write(']');
					}
					fOut.WriteLine('>');
					break;
				}

				case Node.ELEMENT_NODE:
				{
					fOut.Write('<');
					fOut.Write(node.GetNodeName());
					Attr[] attrs = SortAttributes(node.GetAttributes());
					for (int i = 0; i < attrs.Length; i++)
					{
						Attr attr = attrs[i];
						fOut.Write(' ');
						fOut.Write(attr.GetNodeName());
						fOut.Write("=\"");
						NormalizeAndPrint(attr.GetNodeValue(), true);
						fOut.Write('"');
					}
					fOut.Write('>');
					fOut.Flush();
					Node child = node.GetFirstChild();
					while (child != null)
					{
						Write(child);
						child = child.GetNextSibling();
					}
					break;
				}

				case Node.ENTITY_REFERENCE_NODE:
				{
					if (fCanonical)
					{
						Node child = node.GetFirstChild();
						while (child != null)
						{
							Write(child);
							child = child.GetNextSibling();
						}
					}
					else
					{
						fOut.Write('&');
						fOut.Write(node.GetNodeName());
						fOut.Write(';');
						fOut.Flush();
					}
					break;
				}

				case Node.CDATA_SECTION_NODE:
				{
					if (fCanonical)
					{
						NormalizeAndPrint(node.GetNodeValue(), false);
					}
					else
					{
						fOut.Write("<![CDATA[");
						fOut.Write(node.GetNodeValue());
						fOut.Write("]]>");
					}
					fOut.Flush();
					break;
				}

				case Node.TEXT_NODE:
				{
					NormalizeAndPrint(node.GetNodeValue(), false);
					fOut.Flush();
					break;
				}

				case Node.PROCESSING_INSTRUCTION_NODE:
				{
					fOut.Write("<?");
					fOut.Write(node.GetNodeName());
					String data = node.GetNodeValue();
					if (data != null && data.Length > 0)
					{
						fOut.Write(' ');
						fOut.Write(data);
					}
					fOut.Write("?>");
					fOut.Flush();
					break;
				}

				case Node.COMMENT_NODE:
				{
					if (!fCanonical)
					{
						fOut.Write("<!--");
						String comment = node.GetNodeValue();
						if (comment != null && comment.Length > 0)
						{
							fOut.Write(comment);
						}
						fOut.Write("-->");
						fOut.Flush();
					}
					break;
				}
			}
			if (type == Node.ELEMENT_NODE)
			{
				fOut.Write("</");
				fOut.Write(node.GetNodeName());
				fOut.Write('>');
				fOut.Flush();
			}
		}

		// write(Node)
		/// <summary>Returns a sorted list of attributes.</summary>
		protected internal virtual Attr[] SortAttributes(NamedNodeMap attrs)
		{
			int len = (attrs != null) ? attrs.GetLength() : 0;
			Attr[] array = new Attr[len];
			for (int i = 0; i < len; i++)
			{
				array[i] = (Attr)attrs.Item(i);
			}
			for (int i_1 = 0; i_1 < len - 1; i_1++)
			{
				String name = array[i_1].GetNodeName();
				int index = i_1;
				for (int j = i_1 + 1; j < len; j++)
				{
					String curName = array[j].GetNodeName();
					if (string.CompareOrdinal(curName, name) < 0)
					{
						name = curName;
						index = j;
					}
				}
				if (index != i_1)
				{
					Attr temp = array[i_1];
					array[i_1] = array[index];
					array[index] = temp;
				}
			}
			return array;
		}

		// sortAttributes(NamedNodeMap):Attr[]
		//
		// Protected methods
		//
		/// <summary>Normalizes and prints the given string.</summary>
		protected internal virtual void NormalizeAndPrint(String s, bool isAttValue)
		{
			int len = (s != null) ? s.Length : 0;
			for (int i = 0; i < len; i++)
			{
				char c = s[i];
				NormalizeAndPrint(c, isAttValue);
			}
		}

		// normalizeAndPrint(String,boolean)
		/// <summary>Normalizes and print the given character.</summary>
		protected internal virtual void NormalizeAndPrint(char c, bool isAttValue)
		{
			switch (c)
			{
				case '<':
				{
					fOut.Write("&lt;");
					break;
				}

				case '>':
				{
					fOut.Write("&gt;");
					break;
				}

				case '&':
				{
					fOut.Write("&amp;");
					break;
				}

				case '"':
				{
					// A '"' that appears in character data
					// does not need to be escaped.
					if (isAttValue)
					{
						fOut.Write("&quot;");
					}
					else
					{
						fOut.Write("\"");
					}
					break;
				}

				case '\r':
				{
					// If CR is part of the document's content, it
					// must not be printed as a literal otherwise
					// it would be normalized to LF when the document
					// is reparsed.
					fOut.Write("&#xD;");
					break;
				}

				case '\n':
				{
					if (fCanonical)
					{
						fOut.Write("&#xA;");
						break;
					}
					goto default;
				}

				default:
				{
					// else, default print char
					// In XML 1.1, control chars in the ranges [#x1-#x1F, #x7F-#x9F] must be escaped.
					//
					// Escape space characters that would be normalized to #x20 in attribute values
					// when the document is reparsed.
					//
					// Escape NEL (0x85) and LSEP (0x2028) that appear in content
					// if the document is XML 1.1, since they would be normalized to LF
					// when the document is reparsed.
					if (fXML11 && ((c >= 0x01 && c <= 0x1F && c != 0x09 && c != 0x0A) || (c >= 0x7F &&
						 c <= 0x9F) || c == 0x2028) || isAttValue && (c == 0x09 || c == 0x0A))
					{
						fOut.Write("&#x");
						fOut.Write(com.itextpdf.io.util.JavaUtil.IntegerToHexString(c).ToUpper());
						fOut.Write(";");
					}
					else
					{
						fOut.Write(c);
					}
					break;
				}
			}
		}
	}
}
