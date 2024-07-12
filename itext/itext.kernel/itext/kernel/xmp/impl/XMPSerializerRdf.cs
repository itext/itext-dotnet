//Copyright (c) 2006, Adobe Systems Incorporated
//All rights reserved.
//
//        Redistribution and use in source and binary forms, with or without
//        modification, are permitted provided that the following conditions are met:
//        1. Redistributions of source code must retain the above copyright
//        notice, this list of conditions and the following disclaimer.
//        2. Redistributions in binary form must reproduce the above copyright
//        notice, this list of conditions and the following disclaimer in the
//        documentation and/or other materials provided with the distribution.
//        3. All advertising materials mentioning features or use of this software
//        must display the following acknowledgement:
//        This product includes software developed by the Adobe Systems Incorporated.
//        4. Neither the name of the Adobe Systems Incorporated nor the
//        names of its contributors may be used to endorse or promote products
//        derived from this software without specific prior written permission.
//
//        THIS SOFTWARE IS PROVIDED BY ADOBE SYSTEMS INCORPORATED ''AS IS'' AND ANY
//        EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
//        WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
//        DISCLAIMED. IN NO EVENT SHALL ADOBE SYSTEMS INCORPORATED BE LIABLE FOR ANY
//        DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
//        (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
//        LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
//        ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
//        (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
//        SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
//
//        http://www.adobe.com/devnet/xmp/library/eula-xmp-library-java.html
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using iText.Commons.Utils;
using iText.IO.Util;
using iText.Kernel.XMP.Options;

namespace iText.Kernel.XMP.Impl
{
	/// <summary>Serializes the <code>XMPMeta</code>-object using the standard RDF serialization format.
	/// 	</summary>
	/// <remarks>
	/// Serializes the <code>XMPMeta</code>-object using the standard RDF serialization format.
	/// The output is written to an <code>OutputStream</code>
	/// according to the <code>SerializeOptions</code>.
	/// </remarks>
	/// <since>11.07.2006</since>
	public class XMPSerializerRdf
	{
		/// <summary>default padding</summary>
		private const int DEFAULT_PAD = 2048;

		private const String PACKET_HEADER = "<?xpacket begin=\"\uFEFF\" id=\"W5M0MpCehiHzreSzNTczkc9d\"?>";

		/// <summary>The w/r is missing inbetween</summary>
		private const String PACKET_TRAILER = "<?xpacket end=\"";

		private const String PACKET_TRAILER2 = "\"?>";

		private const String RDF_XMPMETA_START = "<x:xmpmeta xmlns:x=\"adobe:ns:meta/\" x:xmptk=\"";

		private const String RDF_XMPMETA_END = "</x:xmpmeta>";

		private const String RDF_RDF_START = "<rdf:RDF xmlns:rdf=\"http://www.w3.org/1999/02/22-rdf-syntax-ns#\">";

		private const String RDF_RDF_END = "</rdf:RDF>";

		private const String RDF_SCHEMA_START = "<rdf:Description rdf:about=";

		private const String RDF_SCHEMA_END = "</rdf:Description>";

		private const String RDF_STRUCT_START = "<rdf:Description";

		private const String RDF_STRUCT_END = "</rdf:Description>";

		private const String RDF_EMPTY_STRUCT = "<rdf:Description/>";

		//\cond DO_NOT_DOCUMENT	
		/// <summary>a set of all rdf attribute qualifier</summary>
		internal static readonly ICollection<String> RDF_ATTR_QUALIFIER = new HashSet<String
			>(JavaUtil.ArraysAsList(XMPConst.XML_LANG, "rdf:resource",
			"rdf:ID", "rdf:bagID", "rdf:nodeID"));
		//\endcond
		
		/// <summary>the metadata object to be serialized.</summary>
		private XMPMetaImpl xmp;

		/// <summary>the output stream to serialize to</summary>
		private CountOutputStream outputStream;

		/// <summary>this writer is used to do the actual serialization</summary>
		private StreamWriter writer;

		/// <summary>the stored serialization options</summary>
		private SerializeOptions options;

		/// <summary>
		/// the size of one unicode char, for UTF-8 set to 1
		/// (Note: only valid for ASCII chars lower than 0x80),
		/// set to 2 in case of UTF-16
		/// </summary>
		private int unicodeSize = 1;

		/// <summary>
		/// the padding in the XMP Packet, or the length of the complete packet in
		/// case of option <em>exactPacketLength</em>.
		/// </summary>
		private int padding;

		// UTF-8
		/// <summary>The actual serialization.</summary>
		/// <param name="xmp">the metadata object to be serialized</param>
		/// <param name="output">outputStream the output stream to serialize to</param>
		/// <param name="options">the serialization options</param>
		public virtual void Serialize(XMPMeta xmp, Stream output, SerializeOptions options)
		{
			try
			{
				outputStream = new CountOutputStream(output);
				this.xmp = (XMPMetaImpl)xmp;
				this.options = options;
				this.padding = options.GetPadding();
				writer = new StreamWriter(outputStream, new EncodingNoPreamble(IanaEncodings.GetEncodingEncoding(options.GetEncoding())));
				CheckOptionsConsistence();
				// serializes the whole packet, but don't write the tail yet 
				// and flush to make sure that the written bytes are calculated correctly
				String tailStr = SerializeAsRDF();
				writer.Flush();
				// adds padding
				AddPadding(tailStr.Length);
				// writes the tail
				Write(tailStr);
				writer.Flush();
				outputStream.Dispose();
			}
			catch (System.IO.IOException)
			{
				throw new XMPException("Error writing to the OutputStream", XMPError.UNKNOWN);
			}
		}

		/// <summary>Calculates the padding according to the options and write it to the stream.
		/// 	</summary>
		/// <param name="tailLength">the length of the tail string</param>
		private void AddPadding(int tailLength)
		{
			if (options.GetExactPacketLength())
			{
				// the string length is equal to the length of the UTF-8 encoding
				int minSize = outputStream.GetBytesWritten() + tailLength * unicodeSize;
				if (minSize > padding)
				{
					throw new XMPException("Can't fit into specified packet size", XMPError.BADSERIALIZE
						);
				}
				padding -= minSize;
			}
			// Now the actual amount of padding to add.
			// fix rest of the padding according to Unicode unit size.
			padding /= unicodeSize;
			int newlineLen = options.GetNewline().Length;
			if (padding >= newlineLen)
			{
				padding -= newlineLen;
				// Write this newline last.
				while (padding >= (100 + newlineLen))
				{
					WriteChars(100, ' ');
					WriteNewline();
					padding -= (100 + newlineLen);
				}
				WriteChars(padding, ' ');
				WriteNewline();
			}
			else
			{
				WriteChars(padding, ' ');
			}
		}

		/// <summary>Checks if the supplied options are consistent.</summary>
		protected internal virtual void CheckOptionsConsistence()
		{
			if (options.GetEncodeUTF16BE() | options.GetEncodeUTF16LE())
			{
				unicodeSize = 2;
			}
			if (options.GetExactPacketLength())
			{
				if (options.GetOmitPacketWrapper() | options.GetIncludeThumbnailPad())
				{
					throw new XMPException("Inconsistent options for exact size serialize", XMPError.
						BADOPTIONS);
				}
				if ((options.GetPadding() & (unicodeSize - 1)) != 0)
				{
					throw new XMPException("Exact size must be a multiple of the Unicode element", XMPError
						.BADOPTIONS);
				}
			}
			else
			{
				if (options.GetReadOnlyPacket())
				{
					if (options.GetOmitPacketWrapper() | options.GetIncludeThumbnailPad())
					{
						throw new XMPException("Inconsistent options for read-only packet", XMPError.BADOPTIONS
							);
					}
					padding = 0;
				}
				else
				{
					if (options.GetOmitPacketWrapper())
					{
						if (options.GetIncludeThumbnailPad())
						{
							throw new XMPException("Inconsistent options for non-packet serialize", XMPError.
								BADOPTIONS);
						}
						padding = 0;
					}
					else
					{
						if (padding == 0)
						{
							padding = DEFAULT_PAD * unicodeSize;
						}
						if (options.GetIncludeThumbnailPad())
						{
							if (!xmp.DoesPropertyExist(XMPConst.NS_XMP, "Thumbnails"))
							{
								padding += 10000 * unicodeSize;
							}
						}
					}
				}
			}
		}

		/// <summary>Writes the (optional) packet header and the outer rdf-tags.</summary>
		/// <returns>Returns the packet end processing instraction to be written after the padding.
		/// 	</returns>
		private String SerializeAsRDF()
		{
			int level = 0;
			// Write the packet header PI.
			if (!options.GetOmitPacketWrapper())
			{
				WriteIndent(level);
				Write(PACKET_HEADER);
				WriteNewline();
			}
			// Write the x:xmpmeta element's start tag.
			if (!options.GetOmitXmpMetaElement())
			{
				WriteIndent(level);
				Write(RDF_XMPMETA_START);
				// Note: this flag can only be set by unit tests
				if (!options.GetOmitVersionAttribute())
				{
					Write(XMPMetaFactory.GetVersionInfo().GetMessage());
				}
				Write("\">");
				WriteNewline();
				level++;
			}
			// Write the rdf:RDF start tag.
			WriteIndent(level);
			Write(RDF_RDF_START);
			WriteNewline();
			// Write all of the properties.
			if (options.GetUseCanonicalFormat())
			{
				SerializeCanonicalRDFSchemas(level);
			}
			else
			{
				SerializeCompactRDFSchemas(level);
			}
			// Write the rdf:RDF end tag.
			WriteIndent(level);
			Write(RDF_RDF_END);
			WriteNewline();
			// Write the xmpmeta end tag.
			if (!options.GetOmitXmpMetaElement())
			{
				level--;
				WriteIndent(level);
				Write(RDF_XMPMETA_END);
				WriteNewline();
			}
			// Write the packet trailer PI into the tail string as UTF-8.
			String tailStr = "";
			if (!options.GetOmitPacketWrapper())
			{
				for (level = options.GetBaseIndent(); level > 0; level--)
				{
					tailStr += options.GetIndent();
				}
				tailStr += PACKET_TRAILER;
				tailStr += options.GetReadOnlyPacket() ? 'r' : 'w';
				tailStr += PACKET_TRAILER2;
			}
			return tailStr;
		}

		/// <summary>Serializes the metadata in pretty-printed manner.</summary>
		/// <param name="level">indent level</param>
		private void SerializeCanonicalRDFSchemas(int level)
		{
			if (xmp.GetRoot().GetChildrenLength() > 0)
			{
				StartOuterRDFDescription(xmp.GetRoot(), level);
				for (IEnumerator it = xmp.GetRoot().IterateChildren(); it.MoveNext(); )
				{
					XMPNode currSchema = (XMPNode)it.Current;
					SerializeCanonicalRDFSchema(currSchema, level);
				}
				EndOuterRDFDescription(level);
			}
			else
			{
				WriteIndent(level + 1);
				Write(RDF_SCHEMA_START);
				// Special case an empty XMP object.
				WriteTreeName();
				Write("/>");
				WriteNewline();
			}
		}

		private void WriteTreeName()
		{
			Write('"');
			String name = xmp.GetRoot().GetName();
			if (name != null)
			{
				AppendNodeValue(name, true);
			}
			Write('"');
		}

		/// <summary>Serializes the metadata in compact manner.</summary>
		/// <param name="level">indent level to start with</param>
		private void SerializeCompactRDFSchemas(int level)
		{
			// Begin the rdf:Description start tag.
			WriteIndent(level + 1);
			Write(RDF_SCHEMA_START);
			WriteTreeName();
			// Write all necessary xmlns attributes.
			ICollection<String> usedPrefixes = new HashSet<String>();
			usedPrefixes.Add("xml");
			usedPrefixes.Add("rdf");
			for (IEnumerator it = xmp.GetRoot().IterateChildren(); it.MoveNext(); )
			{
				XMPNode schema = (XMPNode)it.Current;
				DeclareUsedNamespaces(schema, usedPrefixes, level + 3);
			}
			// Write the top level "attrProps" and close the rdf:Description start tag.
			bool allAreAttrs = true;
			for (IEnumerator it_1 = xmp.GetRoot().IterateChildren(); it_1.MoveNext(); )
			{
				XMPNode schema = (XMPNode)it_1.Current;
				allAreAttrs &= SerializeCompactRDFAttrProps(schema, level + 2);
			}
			if (!allAreAttrs)
			{
				Write('>');
				WriteNewline();
			}
			else
			{
				Write("/>");
				WriteNewline();
				return;
			}
			// ! Done if all properties in all schema are written as attributes.
			// Write the remaining properties for each schema.
			for (IEnumerator it_2 = xmp.GetRoot().IterateChildren(); it_2.MoveNext(); )
			{
				XMPNode schema = (XMPNode)it_2.Current;
				SerializeCompactRDFElementProps(schema, level + 2);
			}
			// Write the rdf:Description end tag.
			WriteIndent(level + 1);
			Write(RDF_SCHEMA_END);
			WriteNewline();
		}

		/// <summary>Write each of the parent's simple unqualified properties as an attribute.
		/// 	</summary>
		/// <remarks>
		/// Write each of the parent's simple unqualified properties as an attribute. Returns true if all
		/// of the properties are written as attributes.
		/// </remarks>
		/// <param name="parentNode">the parent property node</param>
		/// <param name="indent">the current indent level</param>
		/// <returns>Returns true if all properties can be rendered as RDF attribute.</returns>
		private bool SerializeCompactRDFAttrProps(XMPNode parentNode, int indent)
		{
			bool allAreAttrs = true;
			for (IEnumerator it = parentNode.IterateChildren(); it.MoveNext(); )
			{
				XMPNode prop = (XMPNode)it.Current;
				if (CanBeRDFAttrProp(prop))
				{
					WriteNewline();
					WriteIndent(indent);
					Write(prop.GetName());
					Write("=\"");
					AppendNodeValue(prop.GetValue(), true);
					Write('"');
				}
				else
				{
					allAreAttrs = false;
				}
			}
			return allAreAttrs;
		}

		/// <summary>
		/// Recursively handles the "value" for a node that must be written as an RDF
		/// property element.
		/// </summary>
		/// <remarks>
		/// Recursively handles the "value" for a node that must be written as an RDF
		/// property element. It does not matter if it is a top level property, a
		/// field of a struct, or an item of an array. The indent is that for the
		/// property element. The patterns bwlow ignore attribute qualifiers such as
		/// xml:lang, they don't affect the output form.
		/// <blockquote>
		/// <pre>
		/// &lt;ns:UnqualifiedStructProperty-1
		/// ... The fields as attributes, if all are simple and unqualified
		/// /&gt;
		/// &lt;ns:UnqualifiedStructProperty-2 rdf:parseType=&quot;Resource&quot;&gt;
		/// ... The fields as elements, if none are simple and unqualified
		/// &lt;/ns:UnqualifiedStructProperty-2&gt;
		/// &lt;ns:UnqualifiedStructProperty-3&gt;
		/// &lt;rdf:Description
		/// ... The simple and unqualified fields as attributes
		/// &gt;
		/// ... The compound or qualified fields as elements
		/// &lt;/rdf:Description&gt;
		/// &lt;/ns:UnqualifiedStructProperty-3&gt;
		/// &lt;ns:UnqualifiedArrayProperty&gt;
		/// &lt;rdf:Bag&gt; or Seq or Alt
		/// ... Array items as rdf:li elements, same forms as top level properties
		/// &lt;/rdf:Bag&gt;
		/// &lt;/ns:UnqualifiedArrayProperty&gt;
		/// &lt;ns:QualifiedProperty rdf:parseType=&quot;Resource&quot;&gt;
		/// &lt;rdf:value&gt; ... Property &quot;value&quot;
		/// following the unqualified forms ... &lt;/rdf:value&gt;
		/// ... Qualifiers looking like named struct fields
		/// &lt;/ns:QualifiedProperty&gt;
		/// </pre>
		/// </blockquote>
		/// *** Consider numbered array items, but has compatibility problems.
		/// Consider qualified form with rdf:Description and attributes.
		/// </remarks>
		/// <param name="parentNode">the parent node</param>
		/// <param name="indent">the current indent level</param>
		private void SerializeCompactRDFElementProps(XMPNode parentNode, int indent)
		{
			for (IEnumerator it = parentNode.IterateChildren(); it.MoveNext(); )
			{
				XMPNode node = (XMPNode)it.Current;
				if (CanBeRDFAttrProp(node))
				{
					continue;
				}
				bool emitEndTag = true;
				bool indentEndTag = true;
				// Determine the XML element name, write the name part of the start tag. Look over the
				// qualifiers to decide on "normal" versus "rdf:value" form. Emit the attribute
				// qualifiers at the same time.
				String elemName = node.GetName();
				if (XMPConst.ARRAY_ITEM_NAME.Equals(elemName))
				{
					elemName = "rdf:li";
				}
				WriteIndent(indent);
				Write('<');
				Write(elemName);
				bool hasGeneralQualifiers = false;
				bool hasRDFResourceQual = false;
				for (IEnumerator iq = node.IterateQualifier(); iq.MoveNext(); )
				{
					XMPNode qualifier = (XMPNode)iq.Current;
					if (!RDF_ATTR_QUALIFIER.Contains(qualifier.GetName()))
					{
						hasGeneralQualifiers = true;
					}
					else
					{
						hasRDFResourceQual = "rdf:resource".Equals(qualifier.GetName());
						Write(' ');
						Write(qualifier.GetName());
						Write("=\"");
						AppendNodeValue(qualifier.GetValue(), true);
						Write('"');
					}
				}
				// Process the property according to the standard patterns.
				if (hasGeneralQualifiers)
				{
					SerializeCompactRDFGeneralQualifier(indent, node);
				}
				else
				{
					// This node has only attribute qualifiers. Emit as a property element.
					if (!node.GetOptions().IsCompositeProperty())
					{
						bool[] result = SerializeCompactRDFSimpleProp(node);
						emitEndTag = result[0];
						indentEndTag = result[1];
					}
					else
					{
						if (node.GetOptions().IsArray())
						{
							SerializeCompactRDFArrayProp(node, indent);
						}
						else
						{
							emitEndTag = SerializeCompactRDFStructProp(node, indent, hasRDFResourceQual);
						}
					}
				}
				// Emit the property element end tag.
				if (emitEndTag)
				{
					if (indentEndTag)
					{
						WriteIndent(indent);
					}
					Write("</");
					Write(elemName);
					Write('>');
					WriteNewline();
				}
			}
		}

		/// <summary>Serializes a simple property.</summary>
		/// <param name="node">an XMPNode</param>
		/// <returns>Returns an array containing the flags emitEndTag and indentEndTag.</returns>
		private bool[] SerializeCompactRDFSimpleProp(XMPNode node)
		{
			// This is a simple property.
			bool emitEndTag = true;
			bool indentEndTag = true;
			if (node.GetOptions().IsURI())
			{
				Write(" rdf:resource=\"");
				AppendNodeValue(node.GetValue(), true);
				Write("\"/>");
				WriteNewline();
				emitEndTag = false;
			}
			else
			{
				if (node.GetValue() == null || node.GetValue().Length == 0)
				{
					Write("/>");
					WriteNewline();
					emitEndTag = false;
				}
				else
				{
					Write('>');
					AppendNodeValue(node.GetValue(), false);
					indentEndTag = false;
				}
			}
			return new bool[] { emitEndTag, indentEndTag };
		}

		/// <summary>Serializes an array property.</summary>
		/// <param name="node">an XMPNode</param>
		/// <param name="indent">the current indent level</param>
		private void SerializeCompactRDFArrayProp(XMPNode node, int indent)
		{
			// This is an array.
			Write('>');
			WriteNewline();
			EmitRDFArrayTag(node, true, indent + 1);
			if (node.GetOptions().IsArrayAltText())
			{
				XMPNodeUtils.NormalizeLangArray(node);
			}
			SerializeCompactRDFElementProps(node, indent + 2);
			EmitRDFArrayTag(node, false, indent + 1);
		}

		/// <summary>Serializes a struct property.</summary>
		/// <param name="node">an XMPNode</param>
		/// <param name="indent">the current indent level</param>
		/// <param name="hasRDFResourceQual">Flag if the element has resource qualifier</param>
		/// <returns>Returns true if an end flag shall be emitted.</returns>
		private bool SerializeCompactRDFStructProp(XMPNode node, int indent, bool hasRDFResourceQual
			)
		{
			// This must be a struct.
			bool hasAttrFields = false;
			bool hasElemFields = false;
			bool emitEndTag = true;
			for (IEnumerator ic = node.IterateChildren(); ic.MoveNext(); )
			{
				XMPNode field = (XMPNode)ic.Current;
				if (CanBeRDFAttrProp(field))
				{
					hasAttrFields = true;
				}
				else
				{
					hasElemFields = true;
				}
				if (hasAttrFields && hasElemFields)
				{
					break;
				}
			}
			// No sense looking further.
			if (hasRDFResourceQual && hasElemFields)
			{
				throw new XMPException("Can't mix rdf:resource qualifier and element fields", XMPError
					.BADRDF);
			}
			if (!node.HasChildren())
			{
				// Catch an empty struct as a special case. The case
				// below would emit an empty
				// XML element, which gets reparsed as a simple property
				// with an empty value.
				Write(" rdf:parseType=\"Resource\"/>");
				WriteNewline();
				emitEndTag = false;
			}
			else
			{
				if (!hasElemFields)
				{
					// All fields can be attributes, use the
					// emptyPropertyElt form.
					SerializeCompactRDFAttrProps(node, indent + 1);
					Write("/>");
					WriteNewline();
					emitEndTag = false;
				}
				else
				{
					if (!hasAttrFields)
					{
						// All fields must be elements, use the
						// parseTypeResourcePropertyElt form.
						Write(" rdf:parseType=\"Resource\">");
						WriteNewline();
						SerializeCompactRDFElementProps(node, indent + 1);
					}
					else
					{
						// Have a mix of attributes and elements, use an inner rdf:Description.
						Write('>');
						WriteNewline();
						WriteIndent(indent + 1);
						Write(RDF_STRUCT_START);
						SerializeCompactRDFAttrProps(node, indent + 2);
						Write(">");
						WriteNewline();
						SerializeCompactRDFElementProps(node, indent + 1);
						WriteIndent(indent + 1);
						Write(RDF_STRUCT_END);
						WriteNewline();
					}
				}
			}
			return emitEndTag;
		}

		/// <summary>Serializes the general qualifier.</summary>
		/// <param name="node">the root node of the subtree</param>
		/// <param name="indent">the current indent level</param>
		private void SerializeCompactRDFGeneralQualifier(int indent, XMPNode node)
		{
			// The node has general qualifiers, ones that can't be
			// attributes on a property element.
			// Emit using the qualified property pseudo-struct form. The
			// value is output by a call
			// to SerializePrettyRDFProperty with emitAsRDFValue set.
			Write(" rdf:parseType=\"Resource\">");
			WriteNewline();
			SerializeCanonicalRDFProperty(node, false, true, indent + 1);
			for (IEnumerator iq = node.IterateQualifier(); iq.MoveNext(); )
			{
				XMPNode qualifier = (XMPNode)iq.Current;
				SerializeCanonicalRDFProperty(qualifier, false, false, indent + 1);
			}
		}

		/// <summary>
		/// Serializes one schema with all contained properties in pretty-printed
		/// manner.<br />
		/// Each schema's properties are written to a single
		/// rdf:Description element.
		/// </summary>
		/// <remarks>
		/// Serializes one schema with all contained properties in pretty-printed
		/// manner.<br />
		/// Each schema's properties are written to a single
		/// rdf:Description element. All of the necessary namespaces are declared in
		/// the rdf:Description element. The baseIndent is the base level for the
		/// entire serialization, that of the x:xmpmeta element. An xml:lang
		/// qualifier is written as an attribute of the property start tag, not by
		/// itself forcing the qualified property form.
		/// <blockquote>
		/// <pre>
		/// &lt;rdf:Description rdf:about=&quot;TreeName&quot; xmlns:ns=&quot;URI&quot; ... &gt;
		/// ... The actual properties of the schema, see SerializePrettyRDFProperty
		/// &lt;!-- ns1:Alias is aliased to ns2:Actual --&gt;  ... If alias comments are wanted
		/// &lt;/rdf:Description&gt;
		/// </pre>
		/// </blockquote>
		/// </remarks>
		/// <param name="schemaNode">a schema node</param>
		/// <param name="level"/>
		private void SerializeCanonicalRDFSchema(XMPNode schemaNode, int level)
		{
			// Write each of the schema's actual properties.
			for (IEnumerator it = schemaNode.IterateChildren(); it.MoveNext(); )
			{
				XMPNode propNode = (XMPNode)it.Current;
				SerializeCanonicalRDFProperty(propNode, options.GetUseCanonicalFormat(), false, level
					 + 2);
			}
		}

		/// <summary>Writes all used namespaces of the subtree in node to the output.</summary>
		/// <remarks>
		/// Writes all used namespaces of the subtree in node to the output.
		/// The subtree is recursivly traversed.
		/// </remarks>
		/// <param name="node">the root node of the subtree</param>
		/// <param name="usedPrefixes">a set containing currently used prefixes</param>
		/// <param name="indent">the current indent level</param>
		private void DeclareUsedNamespaces(XMPNode node, ICollection<String> usedPrefixes
			, int indent)
		{
			if (node.GetOptions().IsSchemaNode())
			{
				// The schema node name is the URI, the value is the prefix.
				String prefix = node.GetValue().JSubstring(0, node.GetValue().Length - 1);
				DeclareNamespace(prefix, node.GetName(), usedPrefixes, indent);
			}
			else
			{
				if (node.GetOptions().IsStruct())
				{
					for (IEnumerator it = node.IterateChildren(); it.MoveNext(); )
					{
						XMPNode field = (XMPNode)it.Current;
						DeclareNamespace(field.GetName(), null, usedPrefixes, indent);
					}
				}
			}
			for (IEnumerator it_1 = node.IterateChildren(); it_1.MoveNext(); )
			{
				XMPNode child = (XMPNode)it_1.Current;
				DeclareUsedNamespaces(child, usedPrefixes, indent);
			}
			for (IEnumerator it_2 = node.IterateQualifier(); it_2.MoveNext(); )
			{
				XMPNode qualifier = (XMPNode)it_2.Current;
				DeclareNamespace(qualifier.GetName(), null, usedPrefixes, indent);
				DeclareUsedNamespaces(qualifier, usedPrefixes, indent);
			}
		}

		/// <summary>Writes one namespace declaration to the output.</summary>
		/// <param name="prefix">a namespace prefix (without colon) or a complete qname (when namespace == null)
		/// 	</param>
		/// <param name="namespace">the a namespace</param>
		/// <param name="usedPrefixes">a set containing currently used prefixes</param>
		/// <param name="indent">the current indent level</param>
		private void DeclareNamespace(String prefix, String @namespace, ICollection<String
			> usedPrefixes, int indent)
		{
			if (@namespace == null)
			{
				// prefix contains qname, extract prefix and lookup namespace with prefix
				QName qname = new QName(prefix);
				if (qname.HasPrefix())
				{
					prefix = qname.GetPrefix();
					// add colon for lookup
					@namespace = XMPMetaFactory.GetSchemaRegistry().GetNamespaceURI(prefix + ":");
					// prefix w/o colon
					DeclareNamespace(prefix, @namespace, usedPrefixes, indent);
				}
				else
				{
					return;
				}
			}
			if (string.IsNullOrEmpty(prefix))
			{
				return;
			}
			if (!usedPrefixes.Contains(prefix))
			{
				WriteNewline();
				WriteIndent(indent);
				Write("xmlns:");
				Write(prefix);
				Write("=\"");
				Write(@namespace);
				Write('"');
				usedPrefixes.Add(prefix);
			}
		}

		/// <summary>Start the outer rdf:Description element, including all needed xmlns attributes.
		/// 	</summary>
		/// <remarks>
		/// Start the outer rdf:Description element, including all needed xmlns attributes.
		/// Leave the element open so that the compact form can add property attributes.
		/// </remarks>
		private void StartOuterRDFDescription(XMPNode schemaNode, int level)
		{
			WriteIndent(level + 1);
			Write(RDF_SCHEMA_START);
			WriteTreeName();
			ICollection<String> usedPrefixes = new HashSet<String>();
			usedPrefixes.Add("xml");
			usedPrefixes.Add("rdf");
			DeclareUsedNamespaces(schemaNode, usedPrefixes, level + 3);
			Write('>');
			WriteNewline();
		}

		/// <summary>Write the &lt;/rdf:Description&gt; end tag.</summary>
		private void EndOuterRDFDescription(int level)
		{
			WriteIndent(level + 1);
			Write(RDF_SCHEMA_END);
			WriteNewline();
		}

		/// <summary>Recursively handles the "value" for a node.</summary>
		/// <remarks>
		/// Recursively handles the "value" for a node. It does not matter if it is a
		/// top level property, a field of a struct, or an item of an array. The
		/// indent is that for the property element. An xml:lang qualifier is written
		/// as an attribute of the property start tag, not by itself forcing the
		/// qualified property form. The patterns below mostly ignore attribute
		/// qualifiers like xml:lang. Except for the one struct case, attribute
		/// qualifiers don't affect the output form.
		/// <blockquote>
		/// <pre>
		/// &lt;ns:UnqualifiedSimpleProperty&gt;value&lt;/ns:UnqualifiedSimpleProperty&gt;
		/// &lt;ns:UnqualifiedStructProperty&gt; (If no rdf:resource qualifier)
		/// &lt;rdf:Description&gt;
		/// ... Fields, same forms as top level properties
		/// &lt;/rdf:Description&gt;
		/// &lt;/ns:UnqualifiedStructProperty&gt;
		/// &lt;ns:ResourceStructProperty rdf:resource=&quot;URI&quot;
		/// ... Fields as attributes
		/// &gt;
		/// &lt;ns:UnqualifiedArrayProperty&gt;
		/// &lt;rdf:Bag&gt; or Seq or Alt
		/// ... Array items as rdf:li elements, same forms as top level properties
		/// &lt;/rdf:Bag&gt;
		/// &lt;/ns:UnqualifiedArrayProperty&gt;
		/// &lt;ns:QualifiedProperty&gt;
		/// &lt;rdf:Description&gt;
		/// &lt;rdf:value&gt; ... Property &quot;value&quot; following the unqualified
		/// forms ... &lt;/rdf:value&gt;
		/// ... Qualifiers looking like named struct fields
		/// &lt;/rdf:Description&gt;
		/// &lt;/ns:QualifiedProperty&gt;
		/// </pre>
		/// </blockquote>
		/// </remarks>
		/// <param name="node">the property node</param>
		/// <param name="emitAsRDFValue">property shall be rendered as attribute rather than tag
		/// 	</param>
		/// <param name="useCanonicalRDF">
		/// use canonical form with inner description tag or
		/// the compact form with rdf:ParseType=&quot;resource&quot; attribute.
		/// </param>
		/// <param name="indent">the current indent level</param>
		private void SerializeCanonicalRDFProperty(XMPNode node, bool useCanonicalRDF, bool
			 emitAsRDFValue, int indent)
		{
			bool emitEndTag = true;
			bool indentEndTag = true;
			// Determine the XML element name. Open the start tag with the name and
			// attribute qualifiers.
			String elemName = node.GetName();
			if (emitAsRDFValue)
			{
				elemName = "rdf:value";
			}
			else
			{
				if (XMPConst.ARRAY_ITEM_NAME.Equals(elemName))
				{
					elemName = "rdf:li";
				}
			}
			WriteIndent(indent);
			Write('<');
			Write(elemName);
			bool hasGeneralQualifiers = false;
			bool hasRDFResourceQual = false;
			for (IEnumerator it = node.IterateQualifier(); it.MoveNext(); )
			{
				XMPNode qualifier = (XMPNode)it.Current;
				if (!RDF_ATTR_QUALIFIER.Contains(qualifier.GetName()))
				{
					hasGeneralQualifiers = true;
				}
				else
				{
					hasRDFResourceQual = "rdf:resource".Equals(qualifier.GetName());
					if (!emitAsRDFValue)
					{
						Write(' ');
						Write(qualifier.GetName());
						Write("=\"");
						AppendNodeValue(qualifier.GetValue(), true);
						Write('"');
					}
				}
			}
			// Process the property according to the standard patterns.
			if (hasGeneralQualifiers && !emitAsRDFValue)
			{
				// This node has general, non-attribute, qualifiers. Emit using the
				// qualified property form.
				// ! The value is output by a recursive call ON THE SAME NODE with
				// emitAsRDFValue set.
				if (hasRDFResourceQual)
				{
					throw new XMPException("Can't mix rdf:resource and general qualifiers", XMPError.
						BADRDF);
				}
				// Change serialization to canonical format with inner rdf:Description-tag
				// depending on option
				if (useCanonicalRDF)
				{
					Write(">");
					WriteNewline();
					indent++;
					WriteIndent(indent);
					Write(RDF_STRUCT_START);
					Write(">");
				}
				else
				{
					Write(" rdf:parseType=\"Resource\">");
				}
				WriteNewline();
				SerializeCanonicalRDFProperty(node, useCanonicalRDF, true, indent + 1);
				for (IEnumerator it_1 = node.IterateQualifier(); it_1.MoveNext(); )
				{
					XMPNode qualifier = (XMPNode)it_1.Current;
					if (!RDF_ATTR_QUALIFIER.Contains(qualifier.GetName()))
					{
						SerializeCanonicalRDFProperty(qualifier, useCanonicalRDF, false, indent + 1);
					}
				}
				if (useCanonicalRDF)
				{
					WriteIndent(indent);
					Write(RDF_STRUCT_END);
					WriteNewline();
					indent--;
				}
			}
			else
			{
				// This node has no general qualifiers. Emit using an unqualified form.
				if (!node.GetOptions().IsCompositeProperty())
				{
					// This is a simple property.
					if (node.GetOptions().IsURI())
					{
						Write(" rdf:resource=\"");
						AppendNodeValue(node.GetValue(), true);
						Write("\"/>");
						WriteNewline();
						emitEndTag = false;
					}
					else
					{
						if (node.GetValue() == null || "".Equals(node.GetValue()))
						{
							Write("/>");
							WriteNewline();
							emitEndTag = false;
						}
						else
						{
							Write('>');
							AppendNodeValue(node.GetValue(), false);
							indentEndTag = false;
						}
					}
				}
				else
				{
					if (node.GetOptions().IsArray())
					{
						// This is an array.
						Write('>');
						WriteNewline();
						EmitRDFArrayTag(node, true, indent + 1);
						if (node.GetOptions().IsArrayAltText())
						{
							XMPNodeUtils.NormalizeLangArray(node);
						}
						for (IEnumerator it_1 = node.IterateChildren(); it_1.MoveNext(); )
						{
							XMPNode child = (XMPNode)it_1.Current;
							SerializeCanonicalRDFProperty(child, useCanonicalRDF, false, indent + 2);
						}
						EmitRDFArrayTag(node, false, indent + 1);
					}
					else
					{
						if (!hasRDFResourceQual)
						{
							// This is a "normal" struct, use the rdf:parseType="Resource" form.
							if (!node.HasChildren())
							{
								// Change serialization to canonical format with inner rdf:Description-tag
								// if option is set
								if (useCanonicalRDF)
								{
									Write(">");
									WriteNewline();
									WriteIndent(indent + 1);
									Write(RDF_EMPTY_STRUCT);
								}
								else
								{
									Write(" rdf:parseType=\"Resource\"/>");
									emitEndTag = false;
								}
								WriteNewline();
							}
							else
							{
								// Change serialization to canonical format with inner rdf:Description-tag
								// if option is set
								if (useCanonicalRDF)
								{
									Write(">");
									WriteNewline();
									indent++;
									WriteIndent(indent);
									Write(RDF_STRUCT_START);
									Write(">");
								}
								else
								{
									Write(" rdf:parseType=\"Resource\">");
								}
								WriteNewline();
								for (IEnumerator it_1 = node.IterateChildren(); it_1.MoveNext(); )
								{
									XMPNode child = (XMPNode)it_1.Current;
									SerializeCanonicalRDFProperty(child, useCanonicalRDF, false, indent + 1);
								}
								if (useCanonicalRDF)
								{
									WriteIndent(indent);
									Write(RDF_STRUCT_END);
									WriteNewline();
									indent--;
								}
							}
						}
						else
						{
							// This is a struct with an rdf:resource attribute, use the
							// "empty property element" form.
							for (IEnumerator it_1 = node.IterateChildren(); it_1.MoveNext(); )
							{
								XMPNode child = (XMPNode)it_1.Current;
								if (!CanBeRDFAttrProp(child))
								{
									throw new XMPException("Can't mix rdf:resource and complex fields", XMPError.BADRDF
										);
								}
								WriteNewline();
								WriteIndent(indent + 1);
								Write(' ');
								Write(child.GetName());
								Write("=\"");
								AppendNodeValue(child.GetValue(), true);
								Write('"');
							}
							Write("/>");
							WriteNewline();
							emitEndTag = false;
						}
					}
				}
			}
			// Emit the property element end tag.
			if (emitEndTag)
			{
				if (indentEndTag)
				{
					WriteIndent(indent);
				}
				Write("</");
				Write(elemName);
				Write('>');
				WriteNewline();
			}
		}

		/// <summary>Writes the array start and end tags.</summary>
		/// <param name="arrayNode">an array node</param>
		/// <param name="isStartTag">flag if its the start or end tag</param>
		/// <param name="indent">the current indent level</param>
		private void EmitRDFArrayTag(XMPNode arrayNode, bool isStartTag, int indent)
		{
			if (isStartTag || arrayNode.HasChildren())
			{
				WriteIndent(indent);
				Write(isStartTag ? "<rdf:" : "</rdf:");
				if (arrayNode.GetOptions().IsArrayAlternate())
				{
					Write("Alt");
				}
				else
				{
					if (arrayNode.GetOptions().IsArrayOrdered())
					{
						Write("Seq");
					}
					else
					{
						Write("Bag");
					}
				}
				if (isStartTag && !arrayNode.HasChildren())
				{
					Write("/>");
				}
				else
				{
					Write(">");
				}
				WriteNewline();
			}
		}

		/// <summary>Serializes the node value in XML encoding.</summary>
		/// <remarks>
		/// Serializes the node value in XML encoding. Its used for tag bodies and
		/// attributes. <em>Note:</em> The attribute is always limited by quotes,
		/// thats why <code>&amp;apos;</code> is never serialized. <em>Note:</em>
		/// Control chars are written unescaped, but if the user uses others than tab, LF
		/// and CR the resulting XML will become invalid.
		/// </remarks>
		/// <param name="value">the value of the node</param>
		/// <param name="forAttribute">flag if value is an attribute value</param>
		private void AppendNodeValue(String value, bool forAttribute)
		{
			if (value == null)
			{
				value = "";
			}
			Write(Utils.EscapeXML(value, forAttribute, true));
		}

		/// <summary>
		/// A node can be serialized as RDF-Attribute, if it meets the following conditions:
		/// <ul>
		/// <li>is not array item</li>
		/// <li>don't has qualifier</li>
		/// <li>is no URI</li>
		/// <li>is no composite property</li>
		/// </ul>
		/// </summary>
		/// <param name="node">an XMPNode</param>
		/// <returns>Returns true if the node serialized as RDF-Attribute</returns>
		private bool CanBeRDFAttrProp(XMPNode node)
		{
			return !node.HasQualifier() && !node.GetOptions().IsURI() && !node.GetOptions().IsCompositeProperty
				() && !node.GetOptions().ContainsOneOf(PropertyOptions.SEPARATE_NODE) && !XMPConst
				.ARRAY_ITEM_NAME.Equals(node.GetName());
		}

		/// <summary>Writes indents and automatically includes the baseindend from the options.
		/// 	</summary>
		/// <param name="times">number of indents to write</param>
		private void WriteIndent(int times)
		{
			for (int i = options.GetBaseIndent() + times; i > 0; i--)
			{
				writer.Write(options.GetIndent());
			}
		}

		/// <summary>Writes a char to the output.</summary>
		/// <param name="c">a char</param>
		private void Write(char c)
		{
			writer.Write(c);
		}

		/// <summary>Writes a String to the output.</summary>
		/// <param name="str">a String</param>
		private void Write(String str)
		{
			writer.Write(str);
		}

		/// <summary>Writes an amount of chars, mostly spaces</summary>
		/// <param name="number">number of chars</param>
		/// <param name="c">a char</param>
		private void WriteChars(int number, char c)
		{
			for (; number > 0; number--)
			{
				writer.Write(c);
			}
		}

		/// <summary>Writes a newline according to the options.</summary>
		private void WriteNewline()
		{
			writer.Write(options.GetNewline());
		}
	}
}
