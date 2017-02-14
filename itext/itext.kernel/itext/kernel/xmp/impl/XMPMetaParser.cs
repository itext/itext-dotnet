
//Copyright (c) 2006, Adobe Systems Incorporated\' 
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
using System.Text;
using System.Xml;
using iText.IO.Util;
using iText.Kernel.XMP;
using iText.Kernel.XMP.Options;

namespace iText.Kernel.XMP.Impl
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
		/// <exception cref="iText.Kernel.XMP.XMPException">Thrown if parsing or normalisation fails.
		/// 	</exception>
		public static XMPMeta Parse(Object input, ParseOptions options)
		{
			ParameterAsserts.AssertNotNull(input);
			options = options ?? new ParseOptions();

			XmlDocument document = ParseXml(input, options);

			bool xmpmetaRequired = options.GetRequireXMPMeta();
			object[] result = new object[3];
			result = FindRootNode(document, xmpmetaRequired, result);

			if (result != null && result[1] == XMP_RDF) {
				XMPMetaImpl xmp = ParseRdf.Parse((XmlNode) result[0]);
				xmp.SetPacketHeader((string) result[2]);

				// Check if the XMP object shall be normalized
				if (!options.GetOmitNormalization()) {
					return XMPNormalizer.Process(xmp, options);
				}
				return xmp;
			}
			// no appropriate root node found, return empty metadata object
			return new XMPMetaImpl();
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
		/// <exception cref="iText.Kernel.XMP.XMPException">Thrown if the parsing fails for different reasons
		/// 	</exception>
		private static XmlDocument ParseXml(Object input, ParseOptions options) {
			if (input is Stream) {
				return ParseXmlFromInputStream((Stream) input, options);
			}
			if (input is byte[]) {
				return ParseXmlFromBytebuffer(new ByteBuffer((byte[]) input), options);
			}
			return ParseXmlFromString((string) input, options);
		}

		/// <summary>
		/// Parses XML from an <seealso cref="Stream"/>,
		/// fixing the encoding (Latin-1 to UTF-8) and illegal control character optionally.
		/// </summary>
		/// <param name="stream"> an <code>InputStream</code> </param>
		/// <param name="options"> the parsing options </param>
		/// <returns> Returns an XML DOM-Document. </returns>
		/// <exception cref="XMPException"> Thrown when the parsing fails. </exception>
		private static XmlDocument ParseXmlFromInputStream(Stream stream, ParseOptions options) {
			if (!options.GetAcceptLatin1() && !options.GetFixControlChars()) {
				XmlDocument doc = new XmlDocument();
				doc.Load(GetSecureXmlReader(stream));
				return doc;
			}
			// load stream into bytebuffer
			try {
				ByteBuffer buffer = new ByteBuffer(stream);
				return ParseXmlFromBytebuffer(buffer, options);
			}
			catch (IOException e) {
				throw new XMPException("Error reading the XML-file", XMPError.BADSTREAM, e);
			}
		}

		/// <summary>
		/// Parses XML from a byte buffer,
		/// fixing the encoding (Latin-1 to UTF-8) and illegal control character optionally.
		/// </summary>
		/// <param name="buffer">a byte buffer containing the XMP packet</param>
		/// <param name="options">the parsing options</param>
		/// <returns>Returns an XML DOM-Document.</returns>
		/// <exception cref="iText.Kernel.XMP.XMPException">Thrown when the parsing fails.
		/// 	</exception>
		private static XmlDocument ParseXmlFromBytebuffer(ByteBuffer buffer, ParseOptions options) {
			try {
				XmlDocument doc = new XmlDocument();
				doc.Load(GetSecureXmlReader(buffer.GetByteStream()));
				return doc;
			} catch (XmlException e) {
				XmlDocument doc = new XmlDocument();
				if (options.GetAcceptLatin1()) {
					buffer = Latin1Converter.Convert(buffer);
				}

				if (options.GetFixControlChars()) {
					try {
						StreamReader streamReader = new StreamReader(buffer.GetByteStream(), EncodingUtil.GetEncoding(buffer.GetEncoding()));
                        FixASCIIControlsReader fixReader = new FixASCIIControlsReader(streamReader);
						doc.Load(GetSecureXmlReader(fixReader));
						return doc;
					} catch (Exception) {
						// can normally not happen as the encoding is provided by a util function
						throw new XMPException("Unsupported Encoding", XMPError.INTERNALFAILURE, e);
					}
				}
				doc.Load(buffer.GetByteStream());
				return doc;
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
		/// <exception cref="iText.Kernel.XMP.XMPException">Thrown when the parsing fails.
		/// 	</exception>
		private static XmlDocument ParseXmlFromString(string input, ParseOptions options) {
			try {

				XmlDocument doc = new XmlDocument();
				doc.Load(GetSecureXmlReader(input));
				return doc;
			}
			catch (XMPException e) {
				if (e.GetErrorCode() == XMPError.BADXML && options.GetFixControlChars()) {
					XmlDocument doc = new XmlDocument();
					doc.Load(GetSecureXmlReader(new FixASCIIControlsReader(new StringReader(input))));
					return doc;
				}
				throw e;
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
		private static Object[] FindRootNode(XmlNode root, bool xmpmetaRequired, Object[] result) {
			// Look among this parent's content for x:xapmeta or x:xmpmeta.
			// The recursion for x:xmpmeta is broader than the strictly defined choice, 
			// but gives us smaller code.
			XmlNodeList children = root.ChildNodes;
			for (int i = 0; i < children.Count; i++) {
				root = children[i];
				if (XmlNodeType.ProcessingInstruction == root.NodeType &&
					XMPConst.XMP_PI.Equals(((XmlProcessingInstruction) root).Target)) {
					// Store the processing instructions content
					if (result != null) {
						result[2] = ((XmlProcessingInstruction) root).Data;
					}
				}
				else if (XmlNodeType.Text != root.NodeType && XmlNodeType.ProcessingInstruction != root.NodeType) {
					string rootNs = root.NamespaceURI;
					string rootLocal = root.LocalName;
					if ((XMPConst.TAG_XMPMETA.Equals(rootLocal) || XMPConst.TAG_XAPMETA.Equals(rootLocal)) &&
						XMPConst.NS_X.Equals(rootNs)) {
						// by not passing the RequireXMPMeta-option, the rdf-Node will be valid
						return FindRootNode(root, false, result);
					}
					if (!xmpmetaRequired && "RDF".Equals(rootLocal) && XMPConst.NS_RDF.Equals(rootNs)) {
						if (result != null) {
							result[0] = root;
							result[1] = XMP_RDF;
						}
						return result;
					}
					// continue searching
					object[] newResult = FindRootNode(root, xmpmetaRequired, result);
					if (newResult != null) {
						return newResult;
					}
				}
			}

			// no appropriate node has been found
			return null;
			//     is extracted here in the C++ Toolkit		
		}

		//Security stuff. Protecting against XEE attacks as described here: https://www.owasp.org/index.php/XML_External_Entity_%28XXE%29_Processing
		private static XmlReaderSettings GetSecureReaderSettings()
		{
			XmlReaderSettings settings = new XmlReaderSettings();
			settings.DtdProcessing = DtdProcessing.Prohibit;
			return settings;
		}

		private static XmlReader GetSecureXmlReader(Stream stream) {
			return XmlReader.Create(stream, GetSecureReaderSettings());
		}

		private static XmlReader GetSecureXmlReader(TextReader textReader)
		{
			return XmlReader.Create(textReader, GetSecureReaderSettings());
		}

		private static XmlReader GetSecureXmlReader(String str)
		{
			return XmlReader.Create(new StringReader(str), GetSecureReaderSettings());
		}
	}
}
