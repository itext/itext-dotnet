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
using System.IO;
using Java.IO;
using Javax.Xml;
using Javax.Xml.Parsers;
using Org.W3c.Dom;
using Org.Xml.Sax;
using iTextSharp.Kernel.Xmp;
using iTextSharp.Kernel.Xmp.Options;

namespace iTextSharp.Kernel.Xmp.Impl
{
	/// <summary>
	/// This class replaces the <code>ExpatAdapter.cpp</code> and does the
	/// XML-parsing and fixes the prefix.
	/// </summary>
	/// <remarks>
	/// This class replaces the <code>ExpatAdapter.cpp</code> and does the
	/// XML-parsing and fixes the prefix. After the parsing several normalisations
	/// are applied to the XMPTree.
	/// </remarks>
	/// <since>01.02.2006</since>
	public class XMPMetaParser
	{
		private static readonly Object XMP_RDF = new Object();

		/// <summary>the DOM Parser Factory, options are set</summary>
		private static DocumentBuilderFactory factory = CreateDocumentBuilderFactory();

		/// <summary>Hidden constructor, initialises the SAX parser handler.</summary>
		private XMPMetaParser()
		{
		}

		// EMPTY
		/// <summary>
		/// Parses the input source into an XMP metadata object, including
		/// de-aliasing and normalisation.
		/// </summary>
		/// <param name="input">
		/// the input can be an <code>InputStream</code>, a <code>String</code> or
		/// a byte buffer containing the XMP packet.
		/// </param>
		/// <param name="options">the parse options</param>
		/// <returns>Returns the resulting XMP metadata object</returns>
		/// <exception cref="iTextSharp.Kernel.Xmp.XMPException">Thrown if parsing or normalisation fails.
		/// 	</exception>
		public static XMPMeta Parse(Object input, ParseOptions options)
		{
			ParameterAsserts.AssertNotNull(input);
			options = options != null ? options : new ParseOptions();
			Document document = ParseXml(input, options);
			bool xmpmetaRequired = options.GetRequireXMPMeta();
			Object[] result = new Object[3];
			result = FindRootNode(document, xmpmetaRequired, result);
			if (result != null && result[1] == XMP_RDF)
			{
				XMPMetaImpl xmp = ParseRDF.Parse((Node)result[0]);
				xmp.SetPacketHeader((String)result[2]);
				// Check if the XMP object shall be normalized
				if (!options.GetOmitNormalization())
				{
					return XMPNormalizer.Process(xmp, options);
				}
				else
				{
					return xmp;
				}
			}
			else
			{
				// no appropriate root node found, return empty metadata object
				return new XMPMetaImpl();
			}
		}

		/// <summary>Parses the raw XML metadata packet considering the parsing options.</summary>
		/// <remarks>
		/// Parses the raw XML metadata packet considering the parsing options.
		/// Latin-1/ISO-8859-1 can be accepted when the input is a byte stream
		/// (some old toolkits versions such packets). The stream is
		/// then wrapped in another stream that converts Latin-1 to UTF-8.
		/// <p>
		/// If control characters shall be fixed, a reader is used that fixes the chars to spaces
		/// (if the input is a byte stream is has to be read as character stream).
		/// <p>
		/// Both options reduce the performance of the parser.
		/// </remarks>
		/// <param name="input">
		/// the input can be an <code>InputStream</code>, a <code>String</code> or
		/// a byte buffer containing the XMP packet.
		/// </param>
		/// <param name="options">the parsing options</param>
		/// <returns>Returns the parsed XML document or an exception.</returns>
		/// <exception cref="iTextSharp.Kernel.Xmp.XMPException">Thrown if the parsing fails for different reasons
		/// 	</exception>
		private static Document ParseXml(Object input, ParseOptions options)
		{
			if (input is Stream)
			{
				return ParseXmlFromInputStream((Stream)input, options);
			}
			else
			{
				if (input is byte[])
				{
					return ParseXmlFromBytebuffer(new ByteBuffer((byte[])input), options);
				}
				else
				{
					return ParseXmlFromString((String)input, options);
				}
			}
		}

		/// <summary>
		/// Parses XML from an
		/// <see cref="System.IO.Stream"/>
		/// ,
		/// fixing the encoding (Latin-1 to UTF-8) and illegal control character optionally.
		/// </summary>
		/// <param name="stream">an <code>InputStream</code></param>
		/// <param name="options">the parsing options</param>
		/// <returns>Returns an XML DOM-Document.</returns>
		/// <exception cref="iTextSharp.Kernel.Xmp.XMPException">Thrown when the parsing fails.
		/// 	</exception>
		private static Document ParseXmlFromInputStream(Stream stream, ParseOptions options
			)
		{
			if (!options.GetAcceptLatin1() && !options.GetFixControlChars())
			{
				return ParseInputSource(new InputSource(stream));
			}
			else
			{
				// load stream into bytebuffer
				try
				{
					ByteBuffer buffer = new ByteBuffer(stream);
					return ParseXmlFromBytebuffer(buffer, options);
				}
				catch (System.IO.IOException e)
				{
					throw new XMPException("Error reading the XML-file", XMPError.BADSTREAM, e);
				}
			}
		}

		/// <summary>
		/// Parses XML from a byte buffer,
		/// fixing the encoding (Latin-1 to UTF-8) and illegal control character optionally.
		/// </summary>
		/// <param name="buffer">a byte buffer containing the XMP packet</param>
		/// <param name="options">the parsing options</param>
		/// <returns>Returns an XML DOM-Document.</returns>
		/// <exception cref="iTextSharp.Kernel.Xmp.XMPException">Thrown when the parsing fails.
		/// 	</exception>
		private static Document ParseXmlFromBytebuffer(ByteBuffer buffer, ParseOptions options
			)
		{
			InputSource source = new InputSource(buffer.GetByteStream());
			try
			{
				return ParseInputSource(source);
			}
			catch (XMPException e)
			{
				if (e.GetErrorCode() == XMPError.BADXML || e.GetErrorCode() == XMPError.BADSTREAM)
				{
					if (options.GetAcceptLatin1())
					{
						buffer = Latin1Converter.Convert(buffer);
					}
					if (options.GetFixControlChars())
					{
						try
						{
							String encoding = buffer.GetEncoding();
							TextReader fixReader = new FixASCIIControlsReader(new InputStreamReader(buffer.GetByteStream
								(), encoding));
							return ParseInputSource(new InputSource(fixReader));
						}
						catch (ArgumentException)
						{
							// can normally not happen as the encoding is provided by a util function
							throw new XMPException("Unsupported Encoding", XMPError.INTERNALFAILURE, e);
						}
					}
					source = new InputSource(buffer.GetByteStream());
					return ParseInputSource(source);
				}
				else
				{
					throw;
				}
			}
		}

		/// <summary>
		/// Parses XML from a
		/// <see cref="System.String"/>
		/// ,
		/// fixing the illegal control character optionally.
		/// </summary>
		/// <param name="input">a <code>String</code> containing the XMP packet</param>
		/// <param name="options">the parsing options</param>
		/// <returns>Returns an XML DOM-Document.</returns>
		/// <exception cref="iTextSharp.Kernel.Xmp.XMPException">Thrown when the parsing fails.
		/// 	</exception>
		private static Document ParseXmlFromString(String input, ParseOptions options)
		{
			InputSource source = new InputSource(new StringReader(input));
			try
			{
				return ParseInputSource(source);
			}
			catch (XMPException e)
			{
				if (e.GetErrorCode() == XMPError.BADXML && options.GetFixControlChars())
				{
					source = new InputSource(new FixASCIIControlsReader(new StringReader(input)));
					return ParseInputSource(source);
				}
				else
				{
					throw;
				}
			}
		}

		/// <summary>Runs the XML-Parser.</summary>
		/// <param name="source">an <code>InputSource</code></param>
		/// <returns>Returns an XML DOM-Document.</returns>
		/// <exception cref="iTextSharp.Kernel.Xmp.XMPException">Wraps parsing and I/O-exceptions into an XMPException.
		/// 	</exception>
		private static Document ParseInputSource(InputSource source)
		{
			try
			{
				DocumentBuilder builder = factory.NewDocumentBuilder();
				builder.SetErrorHandler(null);
				return builder.Parse(source);
			}
			catch (SAXException e)
			{
				throw new XMPException("XML parsing failure", XMPError.BADXML, e);
			}
			catch (ParserConfigurationException e)
			{
				throw new XMPException("XML Parser not correctly configured", XMPError.UNKNOWN, e
					);
			}
			catch (System.IO.IOException e)
			{
				throw new XMPException("Error reading the XML-file", XMPError.BADSTREAM, e);
			}
		}

		/// <summary>Find the XML node that is the root of the XMP data tree.</summary>
		/// <remarks>
		/// Find the XML node that is the root of the XMP data tree. Generally this
		/// will be an outer node, but it could be anywhere if a general XML document
		/// is parsed (e.g. SVG). The XML parser counted all rdf:RDF and
		/// pxmp:XMP_Packet nodes, and kept a pointer to the last one. If there is
		/// more than one possible root use PickBestRoot to choose among them.
		/// <p>
		/// If there is a root node, try to extract the version of the previous XMP
		/// toolkit.
		/// <p>
		/// Pick the first x:xmpmeta among multiple root candidates. If there aren't
		/// any, pick the first bare rdf:RDF if that is allowed. The returned root is
		/// the rdf:RDF child if an x:xmpmeta element was chosen. The search is
		/// breadth first, so a higher level candiate is chosen over a lower level
		/// one that was textually earlier in the serialized XML.
		/// </remarks>
		/// <param name="root">the root of the xml document</param>
		/// <param name="xmpmetaRequired">
		/// flag if the xmpmeta-tag is still required, might be set
		/// initially to <code>true</code>, if the parse option "REQUIRE_XMP_META" is set
		/// </param>
		/// <param name="result">The result array that is filled during the recursive process.
		/// 	</param>
		/// <returns>
		/// Returns an array that contains the result or <code>null</code>.
		/// The array contains:
		/// <ul>
		/// <li>[0] - the rdf:RDF-node
		/// <li>[1] - an object that is either XMP_RDF or XMP_PLAIN (the latter is decrecated)
		/// <li>[2] - the body text of the xpacket-instruction.
		/// </ul>
		/// </returns>
		private static Object[] FindRootNode(Node root, bool xmpmetaRequired, Object[] result
			)
		{
			// Look among this parent's content for x:xapmeta or x:xmpmeta.
			// The recursion for x:xmpmeta is broader than the strictly defined choice, 
			// but gives us smaller code.
			NodeList children = root.GetChildNodes();
			for (int i = 0; i < children.GetLength(); i++)
			{
				root = children.Item(i);
				if (Node.PROCESSING_INSTRUCTION_NODE == root.GetNodeType() && XMPConst.XMP_PI.Equals
					(((ProcessingInstruction)root).GetTarget()))
				{
					// Store the processing instructions content
					if (result != null)
					{
						result[2] = ((ProcessingInstruction)root).GetData();
					}
				}
				else
				{
					if (Node.TEXT_NODE != root.GetNodeType() && Node.PROCESSING_INSTRUCTION_NODE != root
						.GetNodeType())
					{
						String rootNS = root.GetNamespaceURI();
						String rootLocal = root.GetLocalName();
						if ((XMPConst.TAG_XMPMETA.Equals(rootLocal) || XMPConst.TAG_XAPMETA.Equals(rootLocal
							)) && XMPConst.NS_X.Equals(rootNS))
						{
							// by not passing the RequireXMPMeta-option, the rdf-Node will be valid
							return FindRootNode(root, false, result);
						}
						else
						{
							if (!xmpmetaRequired && "RDF".Equals(rootLocal) && XMPConst.NS_RDF.Equals(rootNS))
							{
								if (result != null)
								{
									result[0] = root;
									result[1] = XMP_RDF;
								}
								return result;
							}
							else
							{
								// continue searching
								Object[] newResult = FindRootNode(root, xmpmetaRequired, result);
								if (newResult != null)
								{
									return newResult;
								}
								else
								{
									continue;
								}
							}
						}
					}
				}
			}
			// no appropriate node has been found
			return null;
		}

		//     is extracted here in the C++ Toolkit		
		/// <returns>
		/// Creates, configures and returnes the document builder factory for
		/// the Metadata Parser.
		/// </returns>
		private static DocumentBuilderFactory CreateDocumentBuilderFactory()
		{
			DocumentBuilderFactory factory = DocumentBuilderFactory.NewInstance();
			factory.SetNamespaceAware(true);
			factory.SetIgnoringComments(true);
			try
			{
				// honor System parsing limits, e.g.
				// System.setProperty("entityExpansionLimit", "10");
				factory.SetFeature(XMLConstants.FEATURE_SECURE_PROCESSING, true);
			}
			catch (Exception)
			{
			}
			// Ignore IllegalArgumentException and ParserConfigurationException
			// in case the configured XML-Parser does not implement the feature.
			return factory;
		}
	}
}
